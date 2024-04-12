﻿namespace RUIModule.RUIElements
{
    public class UIDropDownList<T> : BaseUIElement where T : BaseUIElement
    {
        public readonly UIVnlPanel showArea, expandArea;
        public readonly UIContainerPanel expandView;
        private readonly VerticalScrollbar scroll;
        public readonly UIImage expandButton;
        public readonly Func<T, T> clone;
        public BaseUIElement lockUI;
        public float buttonXoffset;
        public bool Expanding { get; private set; }
        public UIDropDownList(BaseUIElement parent, BaseUIElement lockUI, Func<T, T> clone)
        {
            showArea = new(0, 0, opacity: 1);
            showArea.Info.HiddenOverflow = true;
            showArea.Info.RightMargin.Pixel = 40;
            parent.Register(showArea);

            expandArea = new(0, 0, opacity: 1);
            expandArea.Info.SetMargin(10);
            expandArea.Info.IsVisible = false;
            parent.Register(expandArea);

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
            Rectangle rec = showArea.HitBox();
            Texture2D tex = Expanding ? AssetLoader.Fold : AssetLoader.Unfold;
            Vector2 origin = tex.Size() / 2f;
            Vector2 pos = rec.TopRight() + new Vector2(-origin.X - buttonXoffset, rec.Height / 2f);
            sb.SimpleDraw(tex, pos, null, origin);
        }
    }
}
