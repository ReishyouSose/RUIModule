using Microsoft.Xna.Framework.Input;

namespace RUIModule.RUIElements
{
    public class VerticalScrollbar : BaseUIElement
    {
        public bool useScrollWheel = true;
        public bool drawBorder = true;
        private readonly Texture2D Tex;
        private readonly Texture2D innerTex;
        private Rectangle InnerRec;
        private bool isMouseDown = false;
        private int scissor;
        private int waitH;
        private int whell = 0;
        private float alpha = 0f;
        private float mouseY;
        private float innerY;
        private float oldMovableY;
        private float real;
        public float Real => real;
        private float wait = 0f;
        private Vector2 mapping;
        private bool isDragging;
        private float previousMouseY;
        public bool canDrag;
        public int? WheelPixel { get; set; }
        public float WheelValue => real;
        public float ViewMovableY => View.MovableSize.Y;
        public UIContainerPanel View { get; set; }

        public VerticalScrollbar(int? wheelPixel = 52, bool drawBorder = false, bool canDrag = true)
        {
            Tex = AssetLoader.VScrollBD;
            innerTex = AssetLoader.VScrollInner;
            WheelPixel = wheelPixel;
            this.drawBorder = drawBorder;
            if (drawBorder)
                alpha = 1;
            SetScissor(6);
            Info.Width.Set(20f, 0f);
            Info.Left.Set(-(drawBorder ? 30 : 25), 1f);
            Info.Height.Set(-scissor * 2 - 20, 1f);
            Info.Top.Set(scissor + 10, 0f);
            Info.TopMargin.Pixel = scissor / 2f;
            Info.ButtomMargin.Pixel = scissor / 2f;
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
                    previousMouseY = Main.mouseY;
                }
            };
            Events.OnLeftDown += element =>
            {
                if (!isMouseDown)
                {
                    isMouseDown = true;
                    int recH = InnerRec.Height;
                    int my = Main.mouseY;
                    int top = InnerTop;
                    if (!InnerRec.Contains(Main.MouseScreen.ToPoint()))
                    {
                        if (my < top + recH / 2f)
                        {
                            innerY = my - top;
                        }
                        else if (my > InnerBottom - recH / 2f)
                        {
                            innerY = my - (InnerBottom - recH);
                        }
                        else
                        {
                            innerY = recH / 2f;
                        }
                    }
                    else
                    {
                        innerY = my - InnerRec.Y;
                    }
                    mapping = new(top + innerY, top + InnerHeight - recH + innerY);
                }
            };
            Events.OnLeftUp += element =>
            {
                isMouseDown = false;
            };
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

        }
        public override void Calculation()
        {
            base.Calculation();
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            if (View == null)
            {
                return;
            }
            bool isMouseHover = View.GetCanHitBox().Contains(Main.MouseScreen.ToPoint());
            if (!drawBorder)
            {
                if (ViewMovableY > 0 && (isMouseHover || isMouseDown || isDragging) && alpha < 1f)
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
                float height = Info.Size.Y - 26f;
                if (!isMouseHover)
                {
                    whell = state.ScrollWheelValue;
                }
                if (ViewMovableY > 0)
                {
                    if (useScrollWheel && isMouseHover && whell != state.ScrollWheelValue)
                    {
                        if (WheelPixel.HasValue)
                        {
                            wait -= WheelPixel.Value / ViewMovableY * Math.Sign(state.ScrollWheelValue - whell);
                        }
                        else
                        {
                            wait -= (state.ScrollWheelValue - whell) / 6f / height;
                        }
                        whell = state.ScrollWheelValue;
                    }
                    if (isMouseDown && mouseY != Main.mouseY && ViewMovableY > 0)
                    {
                        wait = Utils.GetLerpValue(mapping.X, mapping.Y, Main.mouseY, true);
                        mouseY = Main.mouseY;
                    }
                }
            }
            waitH = (int)(InnerHeight * (InnerHeight / (ViewMovableY + InnerHeight)));

            if (isDragging && ViewMovableY > 0)
            {
                if (Main.mouseLeft)
                {
                    float offsetY = (Main.mouseY - previousMouseY) / ViewMovableY;
                    wait = Math.Clamp(wait - offsetY, 0f, 1f);
                    previousMouseY = Main.mouseY;
                }
                else
                    isDragging = false;
            }

            if (oldMovableY != ViewMovableY)
            {
                if (oldMovableY == 0 || ViewMovableY == 0)
                    real = 0;
                else
                    real /= ViewMovableY / oldMovableY;
                real = Math.Clamp(real, 0f, 1f);
                oldMovableY = ViewMovableY;
            }
            wait = Math.Clamp(wait, 0f, 1f);
            InnerRec = InnerRec.Order(InnerLeft, InnerTop + (int)(real * (InnerHeight - waitH)));

            if (InnerRec.Height != waitH)
            {
                int d = waitH - InnerRec.Height;
                if (d > 0)
                {
                    d = Math.Max(1, (int)(d * 0.2f));
                }
                else
                {
                    d = Math.Min(-1, (int)(d * 0.2f));
                }
                InnerRec.Height += d;
            }
            if (wait != real)
            {
                real += (wait - real) / 6f;
                real = Math.Clamp(real, 0f, 1f);
                Calculation();
            }
            InnerRec.Height = Math.Min(InnerRec.Height, Height - scissor);
            if (drawBorder)
                DrawBar(sb, Tex, HitBox(), Color.White);
            if (ViewMovableY > 0 || drawBorder)
                DrawBar(sb, innerTex, InnerRec, Color.White * alpha);
        }
        private void DrawBar(SpriteBatch spriteBatch, Texture2D tex, Rectangle rec, Color color)
        {
            spriteBatch.Draw(tex, new Rectangle(rec.X, rec.Y - scissor, rec.Width, scissor), new Rectangle(0, 0, tex.Width, scissor), color);
            spriteBatch.Draw(tex, new Rectangle(rec.X, rec.Y, rec.Width, rec.Height), new Rectangle(0, scissor, tex.Width, tex.Height - 2 * scissor), color);
            spriteBatch.Draw(tex, new Rectangle(rec.X, rec.Y + rec.Height, rec.Width, scissor), new Rectangle(0, tex.Height - scissor, tex.Width, scissor), color);
        }
        public void ReSet() => real = wait = 0;
        public void ForceSetPixel(float pixel)
        {
            if (ViewMovableY == 0)
            {
                real = wait = 0;
            }
            else
                real = wait = pixel / ViewMovableY;
            Calculation();
        }
        public void MoveView(int pixel, int limit = 0)
        {
            if (ViewMovableY == 0)
                return;
            if (limit > 0)
                pixel = Math.Clamp(pixel, -limit, limit);
            MoveView(pixel / ViewMovableY);
        }
        public void MoveView(float percent)
        {
            wait += percent;
            Calculation();
        }
    }
}
