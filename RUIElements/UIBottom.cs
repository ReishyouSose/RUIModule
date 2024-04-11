namespace RUIModule.RUIElements
{
    public class UIBottom : BaseUIElement
    {
        public bool canDrag;
        private bool dragging;
        private Vector2 startPoint;
        public UIBottom(float x, float y, float xp = 0, float yp = 0)
        {
            startPoint = Vector2.Zero;
            SetSize(x, y, xp, yp);
        }
        public override void LoadEvents()
        {
            base.LoadEvents();
            Events.OnLeftDown += element =>
            {
                if (canDrag && !dragging)
                {
                    dragging = true;
                    startPoint = Main.MouseScreen;
                }
            };
            Events.OnLeftClick += element =>
            {
                if (canDrag)
                {
                    dragging = false;
                }
            };
            Events.OnLeftDoubleClick += element =>
            {
                if (canDrag)
                {
                    dragging = false;
                }
            };
            Events.OnMouseOut += element =>
            {
                if (canDrag)
                {
                    dragging = false;
                }
            };
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (canDrag && startPoint != Main.MouseScreen && dragging)
            {
                Vector2 offestValue = Main.MouseScreen - startPoint;
                Info.Left.Pixel += offestValue.X;
                Info.Top.Pixel += offestValue.Y;
                startPoint = Main.MouseScreen;
                Calculation();
            }
        }
    }
}
