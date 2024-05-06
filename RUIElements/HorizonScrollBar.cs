using Microsoft.Xna.Framework.Input;

namespace RUIModule.RUIElements
{
    public class HorizontalScrollbar : BaseUIElement
    {
        public bool useScrollWheel = true;
        public bool drawBorder = true;
        private readonly Texture2D border;
        private readonly Texture2D inner;
        private Rectangle InnerRec;
        private bool isMouseDown = false;
        private int scissor;
        private int waitW;
        private int whell = 0;
        private float alpha = 0f;
        private float mouseX;
        private float innerX;
        private float oldMovableX;
        private float real;
        public float Real => real;
        private float wait = 0f;
        private Vector2 mapping;
        private bool isDragging;
        private float previousMouseX;
        public int MoveLimit;
        public bool canDrag;
        public int? WheelPixel { get; private set; }
        public float WheelValue => real;
        public float ViewMovableX => View.MovableSize.X;
        public UIContainerPanel View { get; set; }
        public HorizontalScrollbar(int? wheelPixel = 52, bool drawBorder = false, bool canDrag = true)
        {
            border = AssetLoader.HScrollBD;
            inner = AssetLoader.HScrollInner;
            WheelPixel = wheelPixel;
            this.drawBorder = drawBorder;
            if (drawBorder)
                alpha = 1;
            SetScissor(6);
            Info.Height.Set(20f, 0f);
            Info.Top.Set(-(drawBorder ? 30 : 25), 1f);
            Info.Width.Set(-scissor * 2, 1f);
            Info.Left.Set(scissor, 0f);
            Info.LeftMargin.Pixel = scissor / 2f;
            Info.RightMargin.Pixel = scissor / 2f;
            Info.IsSensitive = true;
            this.canDrag = canDrag;
        }
        public void SetScissor(int scissor) => this.scissor = scissor;
        public override void OnInitialization()
        {
            base.OnInitialization();
            Calculation();
            InnerRec = HitBox(false);
        }
        public override void LoadEvents()
        {
            base.LoadEvents();
            View.Events.OnLeftDown += element =>
            {
                if (canDrag)
                {
                    isDragging = true;
                    previousMouseX = Main.mouseX;
                }
            };
            Events.OnLeftDown += element =>
            {
                if (!isMouseDown)
                {
                    isMouseDown = true;
                    int recW = InnerRec.Width;
                    int mx = Main.mouseX;
                    int left = InnerLeft;
                    if (!InnerRec.Contains(Main.MouseScreen.ToPoint()))
                    {
                        if (mx < left + recW / 2f)
                        {
                            innerX = mx - left;
                        }
                        else if (mx > InnerRight - recW / 2f)
                        {
                            innerX = mx - (InnerRight - recW);
                        }
                        else
                        {
                            innerX = recW / 2f;
                        }
                    }
                    else
                    {
                        innerX = mx - InnerRec.X;
                    }
                    mapping = new(left + innerX, left + InnerWidth - recW + innerX);
                }
            };
            Events.OnLeftUp += element => isMouseDown = false;
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);

        }
        public override void DrawSelf(SpriteBatch sb)
        {
            if (View == null)
            {
                return;
            }
            bool isMouseHover = ParentElement.GetCanHitBox().Contains(Main.MouseScreen.ToPoint());
            if (!drawBorder)
            {
                if (ViewMovableX > 0 && (isMouseHover || isMouseDown || isDragging) && alpha < 1f)
                {
                    alpha += 0.04f;
                }

                if (!(isMouseHover || isMouseDown) && alpha > 0f)
                {
                    alpha -= 0.04f;
                }
            }
            if (!View.Info.IsLocked)
            {
                MouseState state = Mouse.GetState();
                if (!isMouseHover)
                {
                    whell = state.ScrollWheelValue;
                }
                if (ViewMovableX > 0)
                {
                    if (useScrollWheel && isMouseHover && whell != state.ScrollWheelValue)
                    {
                        if (WheelPixel.HasValue)
                        {
                            wait -= WheelPixel.Value / ViewMovableX * Math.Sign(state.ScrollWheelValue - whell);
                        }
                        else
                        {
                            wait -= (state.ScrollWheelValue - whell) / 10f / (Info.Size.X - 26f);
                        }
                        whell = state.ScrollWheelValue;
                    }
                    if (isMouseDown && mouseX != Main.mouseX && ViewMovableX > 0)
                    {
                        wait = Utils.GetLerpValue(mapping.X, mapping.Y, Main.mouseX, true);
                        mouseX = Main.mouseX;
                    }
                }
            }
            waitW = (int)(InnerWidth * (InnerWidth / (ViewMovableX + InnerWidth)));

            if (isDragging && ViewMovableX > 0)
            {
                if (Main.mouseLeft)
                {
                    float offsetX = (Main.mouseX - previousMouseX) / ViewMovableX;
                    wait = Math.Clamp(wait - offsetX, 0f, 1f);
                    previousMouseX = Main.mouseX;
                }
                else
                    isDragging = false;
            }

            if (oldMovableX != ViewMovableX)
            {
                if (oldMovableX == 0 || ViewMovableX == 0)
                    real = 0;
                else
                    real /= ViewMovableX / oldMovableX;
                real = Math.Clamp(real, 0f, 1f);
                wait = real;
                oldMovableX = ViewMovableX;
            }
            wait = Math.Clamp(wait, 0f, 1f);
            InnerRec = InnerRec.Order(InnerLeft + (int)(real * (InnerWidth - waitW)), InnerTop);

            if (InnerRec.Width != waitW)
            {
                int d = waitW - InnerRec.Width;
                if (d > 0)
                {
                    d = Math.Max(1, (int)(d * 0.2f));
                }
                else
                {
                    d = Math.Min(-1, (int)(d * 0.2f));
                }
                InnerRec.Width += d;
            }
            if (wait != real)
            {
                real += (wait - real) / 6f;
                real = Math.Clamp(real, 0f, 1f);
                Calculation();
            }
            InnerRec.Width = Math.Min(InnerRec.Width, Width - scissor);
            if (drawBorder)
                DrawBar(sb, border, HitBox(), Color.White);
            if (ViewMovableX > 0 || drawBorder)
                DrawBar(sb, inner, InnerRec, Color.White * alpha);
        }
        private void DrawBar(SpriteBatch spriteBatch, Texture2D tex, Rectangle rec, Color color)
        {
            spriteBatch.Draw(tex, new Rectangle(rec.X - scissor, rec.Y, scissor, rec.Height), new Rectangle(0, 0, scissor, tex.Height), color);
            spriteBatch.Draw(tex, new Rectangle(rec.X, rec.Y, rec.Width, rec.Height), new Rectangle(scissor, 0, tex.Width - 2 * scissor, tex.Height), color);
            spriteBatch.Draw(tex, new Rectangle(rec.X + rec.Width, rec.Y, scissor, rec.Height), new Rectangle(tex.Width - scissor, 0, scissor, tex.Height), color);
        }
        public void ReSet() => real = wait = 0;
        public void ForceSetPixel(float pixel)
        {
            if (ViewMovableX == 0)
                return;
            real = wait = pixel / ViewMovableX;
            Calculation();
        }
        public void MoveView(int pixel, int limit = 0)
        {
            if (ViewMovableX == 0)
                return;
            if (limit > 0)
                pixel = Math.Clamp(pixel, -limit, limit);
            MoveView(pixel / ViewMovableX);
        }
        public void MoveView(float percent)
        {
            wait += percent;
            Calculation();
        }
    }
}
