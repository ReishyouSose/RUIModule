namespace RUIModule.RUIElements
{
    public class UIAdjust : UI3FrameImage
    {
        private bool dragging;
        private Vector2 startPos;
        private float minX, minY, maxX, maxY;
        public UIAdjust(Texture2D? tex = null)
            : base(tex ?? AssetLoader.VnlAdjust, x => x is UIAdjust adjust && adjust.dragging) { }
        public override void OnInitialization()
        {
            base.OnInitialization();
            SetPos(-Width, -Height, 1, 1, false);
            BaseUIElement pe = ParentElement;
            minX = pe.Width;
            minY = pe.Height;
            maxX = minX * 2;
            maxY = minY * 2;
        }
        public override void LoadEvents()
        {
            Events.OnLeftDown += evt =>
            {
                dragging = true;
                startPos = Main.MouseScreen;
            };
            Events.OnLeftUp += evt =>
            {
                dragging = false;
                ParentElement.Calculation();
            };
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (dragging)
            {
                Vector2 pos = Main.MouseScreen;
                BaseUIElement pe = ParentElement;
                if (startPos.X != pos.X)
                {
                    float right = pe.Left + pe.Width;
                    float offset = pos.X - startPos.X;
                    if (CanMove(offset, pos.X, right))
                    {
                        Clamp(ref pe.Info.Width.Pixel, pos.X - startPos.X, minX, maxX);
                        pe.Calculation();
                    }
                }
                if (startPos.Y != pos.Y)
                {
                    float bottom = pe.Top + pe.Height;
                    float offset = pos.Y - startPos.Y;
                    if (CanMove(offset, pos.Y, bottom))
                    {
                        Clamp(ref pe.Info.Height.Pixel, pos.Y - startPos.Y, minY, maxY);
                        pe.Calculation();
                    }
                }
                startPos = pos;
            }
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            sb.SimpleDraw(Tex, HitBox().TopLeft(), new Rectangle((dragging ? 2 : Info.IsMouseHover ? 1 : 0)
                * Width, 0, Width, Height), Vector2.Zero);
        }
        private static bool CanMove(float offset, float mouse, float origin) => (offset > 0 && mouse > origin) || (offset < 0 && mouse < origin);
        private static void Clamp(ref float value, float offset, float min, float max)
        {
            value = Math.Clamp(value + offset, min, max);
        }
        public void SetAdjustRange(float minX, float minY, float maxX, float maxY)
        {
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;
        }
    }
}
