namespace RUIModule.RUIElements
{
    public class UIDropDownList<T> : BaseUIElement where T : BaseUIElement
    {
        public readonly UIVnlPanel showArea, expandArea;
        public readonly UIContainerPanel expandView;
        private readonly VerticalScrollbar scroll;
        public readonly UIImage expandButton;
        public readonly Func<T, T> clone;
        public BaseUIElement lockUI;
        public bool Expanding { get; private set; }
        public UIDropDownList(BaseUIElement lockUI, Func<T, T> clone)
        {
            showArea = new(0, 0, opacity: 1);
            showArea.Info.HiddenOverflow = true;
            showArea.Info.RightMargin.Pixel = 40;
            Register(showArea);

            expandArea = new(0, 0, opacity: 1);
            expandArea.Info.SetMargin(10);
            expandArea.Info.IsVisible = false;
            Register(expandArea);

            expandView = new();
            expandView.SetSize(-30, 0, 1, 1);
            expandArea.Register(expandView);

            scroll = new(52, true, false);
            scroll.Info.Left.Pixel += 10;
            expandView.SetVerticalScrollbar(scroll);
            expandArea.Register(scroll);

            this.lockUI = lockUI;
            this.clone = clone;
            Info.IsSensitive = true;
        }
        public override void LoadEvents()
        {
            Events.OnLeftDown += evt => Expand();
            Events.UnLeftDown += evt =>
            {
                if (Expanding) Expand();
            };
        }
        public override bool ContainsPoint(Point point)
        {
            if (Expanding)
            {
                return (GetParentElementIsHiddenOverflow() ? GetCanHitBox() : Info.TotalHitBox)
                    .Modified(0, 0, 0, expandArea.Height).Contains(point);
            }
            return base.ContainsPoint(point);
        }
        public void Expand()
        {
            expandArea.Info.IsVisible = Expanding = !Expanding;
            lockUI?.LockInteract(!Expanding);
        }
        public void SetWhellPixel(int pixel) => scroll.WheelPixel = pixel;
        public void ChangeShowElement(T uie)
        {
            showArea.RemoveAll();
            showArea.Register(clone(uie));
        }
        public void AddElement(T uie)
        {
            uie.Events.OnLeftDown += evt => ChangeShowElement(uie);
            expandView.AddElement(uie);
        }

        public void ClearAllElements()
        {
            showArea.RemoveAll();
            expandView.ClearAllElements();
        }

        public override void DrawChildren(SpriteBatch sb)
        {
            base.DrawChildren(sb);
            sb.Draw(Expanding ? AssetLoader.Fold : AssetLoader.Unfold, showArea.HitBox().TopRight() + new Vector2(-30, 4), Color.White);
        }
    }
}
