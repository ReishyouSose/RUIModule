namespace RUIModule.RUIElements
{
    public class UIPanel : UIBottom
    {
        public Texture2D Tex;
        public Color color;
        public float opacity = 0.5f;
        public UIPanel(Color? color = null, float opacity = 0.5f) : base()
        {
            Tex = AssetLoader.BackGround;
            canDrag = false;
            this.color = color ?? Color.White;
            this.opacity = opacity;
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            Rectangle rec = HitBox();
            int dis = Tex.Width / 3;
            Rectangle[] coords = Rec3x3(dis, dis);
            Vector2 size = new(Tex.Width / 6f);
            sb.Draw(TextureAssets.MagicPixel.Value, rec, new(0, 0, 1, 1), color * opacity);
            sb.Draw(Tex, NewRec(rec.TopLeft() - new Vector2(0, dis / 2), rec.Width, dis), coords[1], Color.White);
            sb.Draw(Tex, NewRec(rec.TopLeft() - new Vector2(dis / 2, 0), dis, rec.Height), coords[3], Color.White);
            sb.Draw(Tex, NewRec(rec.TopRight() - new Vector2(dis / 2, 0), dis, rec.Height), coords[5], Color.White);
            sb.Draw(Tex, NewRec(rec.BottomLeft() - new Vector2(0, dis / 2), rec.Width, dis), coords[7], Color.White);
            sb.SimpleDraw(Tex, rec.TopLeft(), coords[0], size);
            sb.SimpleDraw(Tex, rec.TopRight(), coords[2], size);
            sb.SimpleDraw(Tex, rec.BottomLeft(), coords[6], size);
            sb.SimpleDraw(Tex, rec.BottomRight(), coords[8], size);
        }
    }
}
