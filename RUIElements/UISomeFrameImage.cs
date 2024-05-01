namespace RUIModule.RUIElements
{
    public class UI2FrameImage : UIImage
    {
        public UI2FrameImage(Texture2D tex) : base(tex, new(tex.Width / 2, tex.Height), Color.White) { }
        public override void DrawSelf(SpriteBatch sb)
        {
            sb.SimpleDraw(Tex, HitBox().TopLeft(), new Rectangle((Info.IsMouseHover ? 1 : 0) * Width, 0, Width, Height), Vector2.Zero);
        }
    }
    public class UI3FrameImage : UIImage
    {
        public readonly Func<UI3FrameImage, bool> activator;
        public UI3FrameImage(Texture2D tex, Func<UI3FrameImage, bool> activator) : base(tex, new(tex.Width / 3, tex.Height), Color.White)
        {
            this.activator = activator;
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            sb.SimpleDraw(Tex, HitBox().TopLeft(), new Rectangle((activator.Invoke(this) ? 2 : Info.IsMouseHover ? 1 : 0) * Width, 0, Width, Height), Vector2.Zero);
        }
    }
    public class UIClose : UI2FrameImage
    {
        public UIClose(Texture2D tex = null) : base(tex ?? AssetLoader.VnlClose) { }
    }
}
