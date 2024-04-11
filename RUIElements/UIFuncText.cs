namespace RUIModule.RUIElements
{
    public class UIFuncText : BaseUIElement
    {
        /// <summary>
        /// 0是中心绘制，1是左上角起始，2是左中
        /// </summary>
        public int Style { get; set; }
        public Color Color;
        public Vector2 scale;
        private readonly Func<string> text;
        public Vector2 size;
        public string Text => text();

        public UIFuncText(Func<string> t, Vector2 scale = default)
        {
            text = t;
            Color = Color.White;
            this.scale = scale == default ? Vector2.One : scale;
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            size = font.GetStringSize(Text, this.scale);
            SetSize(size);
        }

        public override void DrawSelf(SpriteBatch sb)
        {
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            size = font.GetStringSize(Text, scale);
            Vector2 offY = Vector2.UnitY * TextYoffset * scale.Y;
            string t = Text;
            if (Style == 0)
            {
                sb.DrawStr(font, t, Center() + offY, size / 2f, scale);
            }
            else if (Style == 1)
            {
                sb.DrawStr(font, t, Pos() + offY, Vector2.Zero, scale);
            }
            else if (Style == 2)
            {
                sb.DrawStr(font, t, Pos() + offY + Vector2.UnitY * Height / 2, new Vector2(0, size.Y / 2f), scale);
            }
        }
    }
}
