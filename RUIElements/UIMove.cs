namespace RUIModule.RUIElements
{
    public class UIMove : UI3FrameImage
    {
        private bool dragging;
        private Vector2 startPoint;
        public UIMove(Texture2D tex = null) : base(tex ?? AssetLoader.Move, x => x is UIMove move && move.dragging) { }
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
    }
}
