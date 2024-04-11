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
            public override void DrawChildren(SpriteBatch sb)
            {
                base.DrawChildren(sb);
                return;
                if (edgeBlur > 0)
                {
                    var gd = Main.graphics.GraphicsDevice;
                    var old = gd.PresentationParameters.RenderTargetUsage;
                    gd.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

                    gd.SetRenderTarget(RUIManager.render);
                    gd.Clear(Color.Transparent);

                    base.DrawChildren(sb);



                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    gd.SetRenderTarget(null);
                    Effect eff = AssetLoader.edgeBlur;
                    eff.Parameters["type"].SetValue(edgeBlur - 1);
                    eff.Parameters["resolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                    Rectangle hide = ParentElement.HitBox(false);
                    Vector4 rect = new(hide.Left, hide.Top, hide.Right, hide.Bottom);
                    eff.Parameters["outer"].SetValue(rect);
                    eff.Parameters["inner"].SetValue(rect + new Vector4(edgeX, edgeY, -edgeX, -edgeY));
                    eff.CurrentTechnique.Passes[0].Apply();
                    sb.Draw(RUIManager.render, Vector2.Zero, Color.White);

                    gd.PresentationParameters.RenderTargetUsage = old;
                    sb.End();
                    //启用画笔，传参：延迟绘制（纹理合批优化），alpha颜色混合模式，各向异性采样，不启用深度模式，UI大小矩阵
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                        DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);

                    DrawRec(sb, hide, 1, Color.White, false);
                    DrawRec(sb, hide.Modified(edgeX, edgeY, -edgeX * 2, -edgeY * 2), 1, Color.Red, false);
                }
                else base.DrawChildren(sb);
            }
        }
        private InnerPanel _innerPanel;
        public bool forceUpdateX;
        public bool forceUpdateY;
        /// <summary>
        /// 0纵1横2全，配合<see cref="spaceX"/>与<see cref="spaceY"/>使用自动间隔
        /// </summary>
        public bool[] autoPos;
        public int spaceX;
        public int spaceY;
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
            autoPos = new bool[3];
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
            base.Update(gt);
            if (Vscroll != null && (verticalWhellValue != Vscroll.WheelValue || forceUpdateY))
            {
                verticalWhellValue = Vscroll.WheelValue;
                float maxY = MovableSize.Y;
                _innerPanel.Info.Top.Pixel = -MathHelper.Lerp(innerPanelMinLocation.Y, maxY, verticalWhellValue);
                Calculation();
                if (forceUpdateY) forceUpdateY = false;
            }
            if (Hscroll != null && (horizontalWhellValue != Hscroll.WheelValue || forceUpdateX))
            {
                horizontalWhellValue = Hscroll.WheelValue;
                float maxX = MovableSize.X;
                _innerPanel.Info.Left.Pixel = -MathHelper.Lerp(innerPanelMinLocation.X, maxX, horizontalWhellValue);
                Calculation();
                if (forceUpdateX) forceUpdateX = false;
            }
        }
        public bool AddElement(BaseUIElement element)
        {
            bool flag = _innerPanel.Register(element);
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
            if (autoPos[0])
            {
                int i = 0;
                foreach (BaseUIElement uie in InnerUIE)
                {
                    uie.SetPos(0, i);
                    i += uie.Height + spaceY;
                }
                return;
            }
            if (autoPos[1])
            {
                int i = 0;
                foreach (BaseUIElement uie in InnerUIE)
                {
                    uie.SetPos(i, 0);
                    i += uie.Width + spaceX;
                }
                return;
            }
            if (autoPos[2])
            {
                int x = 0, y = 0;
                foreach (BaseUIElement uie in InnerUIE)
                {
                    uie.SetPos(x, y);
                    x += uie.Width + spaceX;
                    y += uie.Height + spaceY;
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
