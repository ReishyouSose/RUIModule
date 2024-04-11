namespace RUIModule.RUIElements
{
    public class UISideBar : UIPanel
    {
        public int dir;
        public bool open = true;
        public bool canOpen = true;
        public const int max = 30;
        public float factor = max;

        public List<BaseUIElement> List { get; private set; }
        private float? baseX = null, baseY = null, baseL = null, baseT = null;
        public UIImage button;
        /// <param name="dir">0123顺转四向</param>
        public UISideBar(float x, float y, int dir = 0) : base(x, y)
        {
            canDrag = false;
            baseX ??= x;
            baseY ??= y;
            button = new(AssetLoader.Side, 20, 42);
            this.dir = dir;
            switch (dir)
            {
                case 0:
                    SetCenter(Width, 0, 1, 0.5f);
                    button.SetCenter(button.Width / 2f, 0, 1, 0.5f);
                    break;
                case 1:
                    SetCenter(0, Height, 0.5f, 1);
                    button.SetSize(42, 20);
                    button.SetCenter(0, button.Height / 2f, 0.5f, 1);
                    break;
                case 2:
                    SetCenter(-Width, 0, 0, 0.5f);
                    button.SetCenter(-button.Width / 2f, 0, 0, 0.5f);
                    break;
                case 3:
                    SetCenter(0, -Height, 0.5f, 0);
                    button.SetSize(42, 20);
                    button.SetCenter(0, -button.Height / 2f, 0.5f);
                    break;
            }
            button.ReDraw = (sb) =>
            {
                Texture2D tex = button.Tex;
                Vector2 origin = new(9, 36);
                float rot = MathHelper.TwoPi / 4f * (dir % 2);
                SpriteEffects se = dir >= 2 ? SpriteEffects.FlipHorizontally : 0;
                Rectangle rec = new(button.ContainsPoint(Main.MouseScreen.ToPoint()) && canOpen ? 36 : 0, open ? 72 : 0, 18, 72);
                sb.Draw(tex, button.Center(), rec, color * 2f, rot, origin, 1f, se, 0);
                rec.X += 18;
                sb.Draw(tex, button.Center(), rec, Color.White, rot, origin, 1f, se, 0);
            };
            button.Events.OnLeftClick += (evt) =>
            {
                if (!open)
                {
                    if (canOpen)
                    {
                        open = true;
                    }
                }
                else
                {
                    open = false;
                }

                button.Info.CanBeInteract = false;
            };
            Register(button);
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
            float oldf = factor;
            baseL ??= Info.Left.Pixel;
            baseT ??= Info.Top.Pixel;
            if (!canOpen && open)
            {
                open = false;
            }

            if (open)
            {
                if (factor < max)
                {
                    factor++;
                }
                if (factor == max)
                {
                    button.Info.CanBeInteract = true;
                    if (List != null && List.Count > 0)
                    {
                        for (int i = 0; i < List.Count; i++)
                        {
                            List[i].Info.IsVisible = true;
                        }
                    }
                }
            }
            else
            {
                if (factor > 0)
                {
                    factor--;
                }

                if (factor == max - 1)
                {
                    button.Info.CanBeInteract = true;
                    if (List != null && List.Count > 0)
                    {
                        for (int i = 0; i < List.Count; i++)
                        {
                            List[i].Info.IsVisible = false;
                        }
                    }
                }
            }
            if (oldf != factor)
            {
                double lerp = Math.Pow(factor / max, open ? 0.5f : 2);
                switch (dir)
                {
                    case 0:
                        Info.Width.Pixel = (int)(baseX * lerp);
                        break;
                    case 1:
                        Info.Height.Pixel = (int)(baseY * lerp);
                        break;
                    case 2:
                        Info.Left.Pixel = (int)(baseL * lerp);
                        Info.Width.Pixel = (int)(baseX * lerp);
                        break;
                    case 3:
                        Info.Top.Pixel = (int)(baseT * lerp);
                        Info.Height.Pixel = (int)(baseY * lerp);
                        break;
                }
                Calculation();
            }
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            int dis = Tex.Width / 3;
            Rectangle[] coords = Rec3x3(dis, dis);
            Matrix matrix = Main.UIScaleMatrix;
            Rectangle rec = HitBox();
            Vector2 size = new(Tex.Width / 6f);
            /*Main.graphics.PreferMultiSampling = true;
            Main.graphics.ApplyChanges();*/
            RasterizerState overflowHiddenRasterizerState = new()
            {
                CullMode = CullMode.None,
                ScissorTestEnable = true
            };
            //关闭画笔以便修改绘制参数
            sb.End();
            //修改光栅化状态
            sb.GraphicsDevice.RasterizerState = overflowHiddenRasterizerState;
            //设定gd是画笔绑定的图像设备
            GraphicsDevice gd = sb.GraphicsDevice;
            //储存绘制原剪切矩形
            Rectangle scissorRectangle = gd.ScissorRectangle;

            //此为限定绘制框，需要矩阵修正
            Rectangle drawrec = HiddenOverflowRectangle;
            drawrec.Width += dis;
            drawrec.Height += dis;
            drawrec.X -= dis;
            drawrec.Y -= dis;
            drawrec = drawrec.ScaleRec(matrix);

            switch (dir)
            {
                case 0: drawrec.X = rec.X; break;
                case 1: drawrec.Y = rec.Y; break;
                case 2: drawrec.Width = rec.Left - drawrec.Left + rec.Width; break;
                case 3: drawrec.Height = rec.Top - drawrec.Top + rec.Height; break;
            }
            //修改GD剪切矩形为原剪切矩形与现剪切矩形的交集
            gd.ScissorRectangle = Rectangle.Intersect(gd.ScissorRectangle, drawrec);
            //启用画笔，传参：延迟绘制（纹理合批优化），alpha颜色混合模式，各向异性采样，不启用深度模式，UI大小矩阵
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                DepthStencilState.None, overflowHiddenRasterizerState, null, Main.UIScaleMatrix);

            sb.Draw(Tex, rec, coords[4], color * 0.4f);
            if (dir != 1)
            {
                sb.Draw(Tex, NewRec(rec.TopLeft() - new Vector2(0, dis / 2), rec.Width, dis), coords[1], Color.White);
            }

            if (dir != 0)
            {
                sb.Draw(Tex, NewRec(rec.TopLeft() - new Vector2(dis / 2, 0), dis, rec.Height), coords[3], Color.White);
            }

            if (dir != 2)
            {
                sb.Draw(Tex, NewRec(rec.TopRight() - new Vector2(dis / 2, 0), dis, rec.Height), coords[5], Color.White);
            }

            if (dir != 3)
            {
                sb.Draw(Tex, NewRec(rec.BottomLeft() - new Vector2(0, dis / 2), rec.Width, dis), coords[7], Color.White);
            }

            if (dir is 2 or 3)
            {
                sb.SimpleDraw(Tex, rec.TopLeft(), coords[0], size);
            }

            if (dir is 0 or 3)
            {
                sb.SimpleDraw(Tex, rec.TopRight(), coords[2], size);
            }

            if (dir is 1 or 2)
            {
                sb.SimpleDraw(Tex, rec.BottomLeft(), coords[6], size);
            }

            if (dir is 0 or 1)
            {
                sb.SimpleDraw(Tex, rec.BottomRight(), coords[8], size);
            }
            //关闭画笔
            sb.End();
            //修改光栅化状态
            gd.RasterizerState = overflowHiddenRasterizerState;
            //将剪切矩形换回原剪切矩形
            gd.ScissorRectangle = scissorRectangle;
            //启用画笔
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                DepthStencilState.None, overflowHiddenRasterizerState, null, Main.UIScaleMatrix);
        }
        public void SetChildrenList(List<BaseUIElement> list)
        {
            if (List != null && List.Count > 0)
            {
                for (int i = 0; i < List.Count; i++)
                {
                    Remove(List[i]);
                }
            }
            if (list == null || list.Count == 0)
            {
                return;
            }

            List = list;
            for (int i = 0; i < list.Count; i++)
            {
                Register(list[i]);
                list[i].Info.IsVisible = open;
                list[i].PostInitialization();
            }
        }
    }
}
