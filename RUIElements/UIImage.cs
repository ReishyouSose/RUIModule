using ReLogic.Content;

namespace RUIModule.RUIElements
{
    public class UIImage : BaseUIElement
    {
        /// <summary>
        /// 计算实际位置时的方式
        /// </summary>
        public enum CalculationStyle
        {
            /// <summary>
            /// 默认
            /// </summary>
            None,
            /// <summary>
            /// 锁定宽高比，以宽为参考轴
            /// </summary>
            LockAspectRatioMainWidth,
            /// <summary>
            /// 锁定宽高比，以高为参考轴
            /// </summary>
            LockAspectRatioMainHeight,
        }
        public Texture2D Tex;
        public Color color;
        public CalculationStyle Style = CalculationStyle.None;
        /// <summary>
        /// 0是铺满，1是中心
        /// </summary>
        public int DrawStyle { get; set; }
        public UIImage(Texture2D tex, Vector2? size = null, Color? color = null)
        {
            Tex = tex;
            this.color = color ?? Color.White;
            size ??= tex?.Size();
            if (size != null) SetSize(size.Value.X, size.Value.Y);
        }
        public UIImage(Texture2D tex, float x, float y, float Xpercent = 0, float Ypercent = 0, Color? color = null)
        {
            Tex = tex;
            this.color = color ?? Color.White;
            Info.Width.Set(x, Xpercent);
            Info.Height.Set(y, Ypercent);
            Calculation();
        }

        public override void DrawSelf(SpriteBatch sb)
        {
            if (Tex == null) return;
            if (DrawStyle == 0)
            {
                sb.Draw(Tex, Info.TotalHitBox, color);
            }
            else if (DrawStyle == 1)
            {
                sb.SimpleDraw(Tex, Center(), null, Tex.Size() / 2f);
            }
        }
        public void ChangeColor(Color color) => this.color = color;
        public override void Calculation()
        {
            float aspectRatio = Tex == null ? 1 : (float)Tex.Width / Tex.Height;
            if (Style == CalculationStyle.LockAspectRatioMainWidth)
            {
                Info.Height = Info.Width / aspectRatio;
            }
            else if (Style == CalculationStyle.LockAspectRatioMainHeight)
            {
                Info.Width = Info.Height * aspectRatio;
            }
            base.Calculation();
        }
        public void ChangeImage(string texKey) => Tex = ModContent.Request<Texture2D>(texKey, AssetRequestMode.ImmediateLoad).Value;
        public void ChangeImage(Texture2D tex, bool reSize = false)
        {
            Tex = tex;
            if (reSize) SetSize(tex.Size());
        }
    }
}
