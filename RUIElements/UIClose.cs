namespace RUIModule.RUIElements
{
    public class UIClose : UIImage
    {
        public UIClose(Texture2D tex = null) : base(tex ?? AssetLoader.VnlClose) { }
        public override void OnInitialization()
        {
            base.OnInitialization();
            SetSize(Tex.Width / 2, Tex.Height);
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            sb.SimpleDraw(Tex, HitBox().TopLeft(), new Rectangle((Info.IsMouseHover ? 1 : 0) * Width, 0, Width, Height), Vector2.Zero);
        }
    }
}
