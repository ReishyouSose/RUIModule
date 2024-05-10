namespace RUIModule.RUIElements
{
    public class UI2FrameImage : UIImage
    {
        public int frameOffset;
        public UI2FrameImage(Texture2D tex) : base(tex, new(tex.Width / 2, tex.Height), Color.White) { }
        private Rectangle ChooseFrame()
        {
            int i = Info.IsMouseHover ? 1 : 0;
            if (scissors == null)
                return new(i * (Width + frameOffset), 0, Width, Height);
            else
            {
                Rectangle r = scissors.Value;
                return new(r.Left + i * (Width + frameOffset), r.Top, Width, Height);
            }
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            sb.SimpleDraw(Tex, HitBox().TopLeft(), ChooseFrame(), Vector2.Zero);
        }
    }
    public class UI3FrameImage : UIImage
    {
        public int frameOffset;
        public readonly Func<UI3FrameImage, bool> activator;
        public UI3FrameImage(Texture2D tex, Func<UI3FrameImage, bool> activator) : base(tex, new(tex.Width / 3, tex.Height), Color.White)
        {
            this.activator = activator;
        }
        private Rectangle ChooseFrame()
        {
            int i = activator.Invoke(this) ? 2 : Info.IsMouseHover ? 1 : 0;
            if (scissors == null)
                return new(i * (Width + frameOffset), 0, Width, Height);
            else
            {
                Rectangle r = scissors.Value;
                return new(r.Left + i * (Width + frameOffset), r.Top, Width, Height);
            }
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            sb.SimpleDraw(Tex, HitBox().TopLeft(), ChooseFrame(), Vector2.Zero);
        }
    }
    public class UIClose : UI2FrameImage
    {
        public UIClose(Texture2D tex = null) : base(tex ?? AssetLoader.VnlClose) { }
    }
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
