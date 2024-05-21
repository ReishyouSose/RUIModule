namespace RUIModule.RUIElements
{
    public class UIContainerPanel : BaseUIElement
    {
        private class InnerPanel : BaseUIElement
        {
            public int edgeBlur;
            public int edgeX;
            public int edgeY;
            public override Rectangle HiddenOverflowRectangle => ParentElement.HiddenOverflowRectangle;
            //public override Rectangle GetCanHitBox() => Rectangle.Intersect(ParentElement.GetCanHitBox(), ParentElement.Info.TotalHitBox);
            public InnerPanel()
            {
                Info.Width.Percent = 1f;
                Info.Height.Percent = 1f;
            }
        }
        private InnerPanel _innerPanel;
        public bool forceUpdateX;
        public bool forceUpdateY;
        /// <summary>
        /// 0纵1横
        public int?[] autoPos;
        public List<BaseUIElement> InnerUIE => _innerPanel.ChildrenElements;

        public VerticalScrollbar Vscroll { get; private set; }
        public HorizontalScrollbar Hscroll { get; private set; }
        private float verticalWhellValue;
        private float horizontalWhellValue;
        private Vector2 innerPanelMinLocation;
        private Vector2 innerPanelMaxLocation;
        public Vector2 MovableSize
        {
            get
            {
                float maxX = Math.Max(innerPanelMinLocation.X, innerPanelMaxLocation.X - _innerPanel.Info.TotalSize.X);
                float maxY = Math.Max(innerPanelMinLocation.Y, innerPanelMaxLocation.Y - _innerPanel.Info.TotalSize.Y);
                return new((int)Math.Round(maxX), (int)Math.Round(maxY));
            }
        }
        public UIContainerPanel(float margin = 0)
        {
            Info.HiddenOverflow = true;
            Info.SetMargin(margin);
            if (_innerPanel == null)
            {
                _innerPanel = new InnerPanel();
                Register(_innerPanel);
                _innerPanel.overrideGetCanHitBox = new(_innerPanel.ParentElement.GetCanHitBox);
            }
            autoPos = new int?[2];
        }
        public void SetVerticalScrollbar(VerticalScrollbar scrollbar)
        {
            Vscroll = scrollbar;
            scrollbar.View = this;
        }

        public void SetHorizontalScrollbar(HorizontalScrollbar scrollbar)
        {
            Hscroll = scrollbar;
            scrollbar.View = this;
        }

        public override void OnInitialization()
        {
            base.OnInitialization();
            if (_innerPanel == null)
            {
                _innerPanel = new InnerPanel();
                Register(_innerPanel);
            }
            Info.IsSensitive = true;
        }
        public override void Update(GameTime gt)
        {
            List<BaseUIElement> needRemoves = [];
            foreach (BaseUIElement uie in InnerUIE)
            {
                if (uie.Info.NeedRemove)
                {
                    needRemoves.Add(uie);
                }
            }
            if (needRemoves.Count > 0)
            {
                foreach (BaseUIElement uie in needRemoves)
                {
                    RemoveElement(uie);
                }
                Calculation();
            }
            base.Update(gt);
            if (Vscroll != null && (verticalWhellValue != Vscroll.WheelValue || forceUpdateY))
            {
                verticalWhellValue = Vscroll.WheelValue;
                float maxY = MovableSize.Y;
                _innerPanel.Info.Top.Pixel = -MathHelper.Lerp(innerPanelMinLocation.Y, maxY, verticalWhellValue);
                Calculation();
                if (forceUpdateY)
                    forceUpdateY = false;
            }
            if (Hscroll != null && (horizontalWhellValue != Hscroll.WheelValue || forceUpdateX))
            {
                horizontalWhellValue = Hscroll.WheelValue;
                float maxX = MovableSize.X;
                _innerPanel.Info.Left.Pixel = -MathHelper.Lerp(innerPanelMinLocation.X, maxX, horizontalWhellValue);
                Calculation();
                if (forceUpdateX)
                    forceUpdateX = false;
            }
        }
        public bool AddElement(BaseUIElement element, int index = -1)
        {
            bool flag = _innerPanel.Register(element, index);
            if (flag)
            {
                Calculation();
            }
            return flag;
        }
        public bool RemoveElement(BaseUIElement element)
        {
            bool flag = _innerPanel.Remove(element);
            if (flag)
            {
                Calculation();
            }
            return flag;
        }
        public void ClearAllElements()
        {
            _innerPanel.ChildrenElements.Clear();
            Hscroll?.ReSet();
            Vscroll?.ReSet();
            Calculation();
        }

        private void CalculationInnerPanelSize()
        {
            innerPanelMinLocation = Vector2.Zero;
            innerPanelMaxLocation = Vector2.Zero;
            Vector2 v = Vector2.Zero;
            _innerPanel.ForEach(element =>
            {
                if (element.IsVisible)
                {
                    v.X = element.Info.TotalLocation.X - _innerPanel.Info.Location.X;
                    v.Y = element.Info.TotalLocation.Y - _innerPanel.Info.Location.Y;
                    if (innerPanelMinLocation.X > v.X)
                    {
                        innerPanelMinLocation.X = v.X;
                    }

                    if (innerPanelMinLocation.Y > v.Y)
                    {
                        innerPanelMinLocation.Y = v.Y;
                    }

                    v.X = element.Info.TotalLocation.X + element.Info.TotalSize.X - _innerPanel.Info.Location.X;
                    v.Y = element.Info.TotalLocation.Y + element.Info.TotalSize.Y - _innerPanel.Info.Location.Y;

                    if (innerPanelMaxLocation.X < v.X)
                    {
                        innerPanelMaxLocation.X = v.X;
                    }

                    if (innerPanelMaxLocation.Y < v.Y)
                    {
                        innerPanelMaxLocation.Y = v.Y;
                    }
                }
            });
        }
        public override void Calculation()
        {
            base.Calculation();
            AutoPosInnerUIE();
            CalculationInnerPanelSize();
            _innerPanel.Calculation();
            Hscroll?.Calculation();
            Vscroll?.Calculation();
        }
        private void AutoPosInnerUIE()
        {
            if (autoPos[0].HasValue && autoPos[1].HasValue)
            {
                int x = 0, y = 0;
                foreach (BaseUIElement uie in InnerUIE)
                {
                    int w = uie.Width;
                    if (x + w > InnerWidth)
                    {
                        x = 0;
                        y += uie.Height + autoPos[0]!.Value;
                    }
                    uie.SetPos(x, y);
                    x += w + autoPos[1]!.Value;
                }
                return;
            }
            if (autoPos[0].HasValue)
            {
                int i = 0;
                foreach (BaseUIElement uie in InnerUIE)
                {
                    uie.Info.Top.Pixel = i;
                    i += uie.Height + autoPos[0]!.Value;
                }
                return;
            }
            if (autoPos[1].HasValue)
            {
                int i = 0;
                foreach (BaseUIElement uie in InnerUIE)
                {
                    uie.Info.Left.Pixel = i;
                    i += uie.Width + autoPos[1]!.Value;
                }
            }
        }
        public void SetEdgeBlur(int type, int x, int y)
        {
            _innerPanel.edgeBlur = type + 1;
            _innerPanel.edgeX = x;
            _innerPanel.edgeY = y;
        }
    }
}
