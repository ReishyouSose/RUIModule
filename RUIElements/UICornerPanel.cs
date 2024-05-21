namespace RUIModule.RUIElements;

public class UICornerPanel : UIBottom
{
    public Texture2D Tex;
    public Color color;
    public bool drawBoeder = true;
    public float opacity = 0.5f;
    public int cornerSize;
    public int barSize;
    public Color borderColor;
    public UICornerPanel(int cornerSize = 12, int barSize = 4, Color? color = null, float opacity = 0.7f) : base()
    {
        this.cornerSize = cornerSize;
        this.barSize = barSize;
        this.color = color ?? new Color(63, 82, 151);
        this.opacity = opacity;
        borderColor = Color.Black;
    }
    public override void DrawSelf(SpriteBatch sb)
    {
        Rectangle rec = HitBox();
        if (drawBoeder)
            VanillaDraw(sb, rec, AssetLoader.VnlBd, borderColor, cornerSize, barSize);
        if (opacity == 0)
            return;
        VanillaDraw(sb, rec, AssetLoader.VnlBg, color * opacity, cornerSize, barSize);
    }
    public static void VanillaDraw(SpriteBatch spriteBatch, Rectangle rec, Texture2D texture, Color color, int cornerSize, int barSize)
    {
        Point point = new(rec.X, rec.Y);
        Point point2 = new(point.X + rec.Width - cornerSize, point.Y + rec.Height - cornerSize);
        int width = point2.X - point.X - cornerSize;
        int height = point2.Y - point.Y - cornerSize;
        spriteBatch.Draw(texture, new Rectangle(point.X, point.Y, cornerSize, cornerSize), new Rectangle(0, 0, cornerSize, cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y, cornerSize, cornerSize), new Rectangle(cornerSize + barSize, 0, cornerSize, cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(point.X, point2.Y, cornerSize, cornerSize), new Rectangle(0, cornerSize + barSize, cornerSize, cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(point2.X, point2.Y, cornerSize, cornerSize), new Rectangle(cornerSize + barSize, cornerSize + barSize, cornerSize, cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(point.X + cornerSize, point.Y, width, cornerSize), new Rectangle(cornerSize, 0, barSize, cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(point.X + cornerSize, point2.Y, width, cornerSize), new Rectangle(cornerSize, cornerSize + barSize, barSize, cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(point.X, point.Y + cornerSize, cornerSize, height), new Rectangle(0, cornerSize, cornerSize, barSize), color);
        spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y + cornerSize, cornerSize, height), new Rectangle(cornerSize + barSize, cornerSize, cornerSize, barSize), color);
        spriteBatch.Draw(texture, new Rectangle(point.X + cornerSize, point.Y + cornerSize, width, height), new Rectangle(cornerSize, cornerSize, barSize, barSize), color);
    }
    public void BorderHoverToGold()
    {
        Events.OnMouseOver += evt => borderColor = Color.Gold;
        Events.OnMouseOut += evt => borderColor = Color.Black;
    }
}

