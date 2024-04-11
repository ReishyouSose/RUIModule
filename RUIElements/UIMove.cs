namespace RUIModule.RUIElements
{
    public class UIMove : UIImage
    {
        private bool dragging;
        private Vector2 startPoint;
        public UIMove(Texture2D tex = null) : base(tex ?? AssetLoader.Move) { }
        public override void OnInitialization()
        {
            base.OnInitialization();
            SetSize(Tex.Width / 3, Tex.Height);
        }
        public override void LoadEvents()
        {
            Events.OnLeftDown += element =>
            {
                if (!dragging)
                {
                    dragging = true;
                    startPoint = Main.MouseScreen;
                }
            };
            Events.OnLeftClick += element => dragging = false;
            Events.OnLeftDoubleClick += element => dragging = false;
            Events.OnMouseOut += element => dragging = false;
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (dragging && startPoint != Main.MouseScreen)
            {
                Vector2 offestValue = Main.MouseScreen - startPoint;
                ParentElement.Info.Left.Pixel += offestValue.X;
                ParentElement.Info.Top.Pixel += offestValue.Y;
                startPoint = Main.MouseScreen;
                ParentElement.Calculation();
            }
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            sb.SimpleDraw(Tex, HitBox().TopLeft(), new Rectangle(ChooseFrame() * Width, 0, Width, Height), Vector2.Zero);
        }
        private int ChooseFrame() => dragging ? 2 : Info.IsMouseHover ? 1 : 0;
    }
}
