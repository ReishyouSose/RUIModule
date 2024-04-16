namespace RUIModule.RUIElements
{
    public class UIText : BaseUIElement
    {
        public string text;
        public Color color;
        public Vector2 scale;
        public int drawStyle;
        public float spread = 1.5f;
        private int maxWidth;
        private DynamicSpriteFont font;
        public Vector2 TextSize { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="drawStyle">0是从左上角，1是从中心，2是从左中</param>
        /// <param name="maxWidth">-2则为使用相对父级的宽度来限制</param>
        public UIText(string text, Color? color = null, Vector2? scale = null, int drawStyle = 0, int maxWidth = -1, DynamicSpriteFont font = null)
        {
            this.text = text;
            this.color = color ?? Color.White;
            this.scale = scale ?? Vector2.One;
            this.drawStyle = drawStyle;
            this.maxWidth = maxWidth;
            this.font = font ?? FontAssets.MouseText.Value;
            TextSize = ChatManager.GetStringSize(this.font, text, Vector2.One, Math.Min(-1, maxWidth));
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            Vector2 drawPos = new();
            Vector2 origin = new();
            Rectangle hitbox = HitBox();
            switch (drawStyle)
            {
                case 0:
                    drawPos = hitbox.TopLeft();
                    origin = Vector2.Zero;
                    break;
                case 1:
                    drawPos = hitbox.Center();
                    origin = TextSize / 2f;
                    break;
                case 2:
                    drawPos = hitbox.TopLeft() + Vector2.UnitY * hitbox.Height / 2f;
                    origin = new Vector2(0, TextSize.Y / 2f);
                    break;
            }
            ChatManager.DrawColorCodedStringWithShadow(sb, font, text, drawPos, color, 0, origin, scale, maxWidth == -2 ? Width : maxWidth, spread);
        }
        public override void Calculation()
        {
            base.Calculation();
        }
        public void SetMaxWidth(int maxWidth)
        {
            this.maxWidth = maxWidth;
            TextSize = ChatManager.GetStringSize(font, text, Vector2.One, maxWidth);
        }
        public void ChangeText(string text, bool resetSize = true)
        {
            this.text = text;
            TextSize = ChatManager.GetStringSize(font, text, Vector2.One, maxWidth);
            if (resetSize)
            {
                SetSize(TextSize);
            }
        }
        public void HoverToGold()
        {
            Events.OnMouseOver += evt => color = Color.Gold;
            Events.OnMouseOut += evt => color = Color.White;
        }
    }
}
