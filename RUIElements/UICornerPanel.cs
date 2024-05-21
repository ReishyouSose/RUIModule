namespace RUIModule.RUIElements
{
    public class UICornerPanel : UIBottom
    {
        public Texture2D bg;
        public bool drawBorder;
        public Texture2D border;
        public int cornerSize;
        public int barSize;
        public float opacity;
        public Color color;
        public UICornerPanel(float x, float y, Color? color) : base(x, y)
        {
            bg = AssetLoader.VnlBg;
            border = AssetLoader.VnlBd;
            this.color = color ?? new Color(63, 82, 151);
            opacity = 0.7f;
            Info.CanBeInteract = true;
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            Rectangle rec = HitBox();
            if (drawBorder) CornerDraw(sb, border, rec);
            CornerDraw(sb, bg, rec);
        }
        private void CornerDraw(SpriteBatch sb, Texture2D tex, Rectangle rec)
        {
            Point point = new(rec.X, rec.Y);
            Point point2 = new(point.X + rec.Width - cornerSize, point.Y + rec.Height - cornerSize);
            int width = point2.X - point.X - cornerSize;
            int height = point2.Y - point.Y - cornerSize;
            sb.Draw(tex, new Rectangle(point.X, point.Y, cornerSize, cornerSize), new Rectangle(0, 0, cornerSize, cornerSize), color);
            sb.Draw(tex, new Rectangle(point2.X, point.Y, cornerSize, cornerSize), new Rectangle(cornerSize + barSize, 0, cornerSize, cornerSize), color);
            sb.Draw(tex, new Rectangle(point.X, point2.Y, cornerSize, cornerSize), new Rectangle(0, cornerSize + barSize, cornerSize, cornerSize), color);
            sb.Draw(tex, new Rectangle(point2.X, point2.Y, cornerSize, cornerSize), new Rectangle(cornerSize + barSize, cornerSize + barSize, cornerSize, cornerSize), color);
            sb.Draw(tex, new Rectangle(point.X + cornerSize, point.Y, width, cornerSize), new Rectangle(cornerSize, 0, barSize, cornerSize), color);
            sb.Draw(tex, new Rectangle(point.X + cornerSize, point2.Y, width, cornerSize), new Rectangle(cornerSize, cornerSize + barSize, barSize, cornerSize), color);
            sb.Draw(tex, new Rectangle(point.X, point.Y + cornerSize, cornerSize, height), new Rectangle(0, cornerSize, cornerSize, barSize), color);
            sb.Draw(tex, new Rectangle(point2.X, point.Y + cornerSize, cornerSize, height), new Rectangle(cornerSize + barSize, cornerSize, cornerSize, barSize), color);
            sb.Draw(tex, new Rectangle(point.X + cornerSize, point.Y + cornerSize, width, height), new Rectangle(cornerSize, cornerSize, barSize, barSize), color);

        }
    }
}
