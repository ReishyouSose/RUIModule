using Terraria.GameInput;

namespace RUIModule.RUISys
{
    public class RUIManager
    {
        /// <summary>
        /// 存放着所有<see cref="ContainerElement"/>实例的字典
        /// </summary>
        public Dictionary<string, ContainerElement> Elements { get; private set; }

        /// <summary>
        /// 访问顺序
        /// </summary>
        public List<string> CallOrder { get; private set; }

        public string hoverTip = string.Empty;
        public int hoverTipMaxWidth;
        public RenderTarget2D Render { get; private set; }
        /// <summary>
        /// 交互部件缓存
        /// </summary>
        private List<BaseUIElement> interactContainerElementsBuffer;

        /// <summary>
        /// 记录需要触发MouseLeftUp事件的部件
        /// </summary>
        private readonly List<BaseUIElement> needCallMouseLeftUpElements;

        /// <summary>
        /// 记录需要触发MouseRightUp事件的部件
        /// </summary>
        private readonly List<BaseUIElement> needCallMouseRightUpElements;

        private readonly List<BaseUIElement> needCallNonMouseLeftUpElements;
        private readonly List<BaseUIElement> needCallNonMouseRightUpElements;

        /// <summary>
        /// 缓存鼠标左键状态
        /// </summary>
        private bool mouseLeftDown = false;

        /// <summary>
        /// 缓存鼠标右键状态
        /// </summary>
        private bool mouseRightDown = false;

        /// <summary>
        /// 鼠标右键冷却
        /// </summary>
        private readonly KeyCooldown mouseLeftCooldown;

        /// <summary>
        /// 鼠标左键冷却
        /// </summary>
        private readonly KeyCooldown mouseRightCooldown;

        /// <summary>
        /// 额外的绘制，位于UI上层
        /// </summary>
        public event Action<SpriteBatch> ExtraDrawOver;

        public RUIManager()
        {
            Elements = [];
            CallOrder = [];
            interactContainerElementsBuffer = [];
            needCallMouseLeftUpElements = [];
            needCallMouseRightUpElements = [];
            needCallNonMouseLeftUpElements = [];
            needCallNonMouseRightUpElements = [];
            mouseLeftCooldown = new KeyCooldown(() =>
            {
                return Main.mouseLeft;
            });
            mouseRightCooldown = new KeyCooldown(() =>
            {
                return Main.mouseRight;
            });
        }

        /// <summary>
        /// 反射加载所有ContainerElement
        /// </summary>
        public void Load()
        {
            IEnumerable<Type> containers = ModLoader.Mods.SelectMany(m => m.GetType().Assembly.GetTypes())
                .Where(c => !c.IsAbstract && c.IsSubclassOf(typeof(ContainerElement)));
            ContainerElement element;
            foreach (Type c in containers)
            {
                element = (ContainerElement)Activator.CreateInstance(c);
                if (element.AutoLoad)
                {
                    Register(element);
                }
            }
            ReSetRender();
        }

        /// <summary>
        /// 执行逻辑
        /// </summary>
        /// <param name="gt"></param>
        public void Update(GameTime gt)
        {
            if (CallOrder.Count == 0 || Elements.Count == 0)
            {
                return;
            }

            hoverTip = string.Empty;
            hoverTipMaxWidth = 0;
            List<BaseUIElement> interact = [];
            List<BaseUIElement> allChild = [];
            ContainerElement child;
            Point mousePos = Main.MouseScreen.ToPoint();
            string focusKey = string.Empty;
            foreach (string key in CallOrder)
            {
                child = Elements[key];
                child?.PreUpdate(gt);
                if (child != null && child.IsVisible)
                {
                    child.Update(gt);
                    interact = child.GetElementsContainsPoint(mousePos);
                    allChild = child.GetAllChilds();
                    if (interact.Count > 0)
                    {
                        focusKey = key;
                        break;
                    }
                }
            }

            if (interact.Count > 0)
            {
                Main.LocalPlayer.mouseInterface = true;
                PlayerInput.LockVanillaMouseScroll("RUIManager");
            }

            foreach (BaseUIElement ce in interact)
            {
                if (!interactContainerElementsBuffer.Contains(ce))
                {
                    ce.Events.MouseOver(ce);
                    ce.Info.IsMouseHover = true;
                }
            }

            foreach (BaseUIElement ce in interactContainerElementsBuffer)
            {
                if (!interact.Contains(ce))
                {
                    ce.Events.MouseOut(ce);
                    ce.Info.IsMouseHover = false;
                }
            }

            interactContainerElementsBuffer = interact;
            foreach (BaseUIElement ce in interactContainerElementsBuffer)
            {
                if (ce.Info.IsMouseHover)
                {
                    ce.Events.MouseHover(ce);
                }
            }


            if (mouseLeftDown != Main.mouseLeft)
            {
                if (Main.mouseLeft)
                {
                    interact.ForEach(x => x.Events.LeftDown(x));
                    IEnumerable<BaseUIElement> nonInteract = allChild.Except(interact);
                    foreach (BaseUIElement uie in nonInteract)
                    {
                        uie.Events.NonLeftDown(uie);
                    }
                    needCallMouseLeftUpElements.AddRange(interact);
                    needCallNonMouseLeftUpElements.AddRange(nonInteract);
                }
                else
                {
                    if (mouseLeftCooldown.IsCoolDown())
                    {
                        needCallMouseLeftUpElements.ForEach(x => x.Events.LeftClick(x));
                        needCallNonMouseLeftUpElements.ForEach(x => x.Events.NonLeftClick(x));
                        mouseLeftCooldown.ResetCoolDown();
                    }
                    else
                    {
                        needCallMouseLeftUpElements.ForEach(x => x.Events.LeftDoubleClick(x));
                        needCallNonMouseLeftUpElements.ForEach(x => x.Events.NonLeftDoubleClick(x));
                        mouseLeftCooldown.CoolDown();
                    }
                    needCallMouseLeftUpElements.ForEach(x => x.Events.LeftUp(x));
                    needCallNonMouseLeftUpElements.ForEach(x => x.Events.NonLeftUp(x));
                    needCallMouseLeftUpElements.Clear();
                    needCallNonMouseLeftUpElements.Clear();
                }
                mouseLeftDown = Main.mouseLeft;
                SetContainerTop(focusKey);
            }

            if (mouseRightDown != Main.mouseRight)
            {
                if (Main.mouseRight)
                {
                    interact.ForEach(x => x.Events.RightDown(x));
                    IEnumerable<BaseUIElement> nonInteract = allChild.Except(interact);
                    foreach (BaseUIElement uie in nonInteract)
                    {
                        uie.Events.NonRightDown(uie);
                    }
                    needCallMouseRightUpElements.AddRange(interact);
                    needCallNonMouseRightUpElements.AddRange(nonInteract);
                }
                else
                {
                    if (mouseRightCooldown.IsCoolDown())
                    {
                        needCallMouseRightUpElements.ForEach(x => x.Events.RightClick(x));
                        needCallNonMouseRightUpElements.ForEach(x => x.Events.NonRightClick(x));
                        mouseRightCooldown.ResetCoolDown();
                    }
                    else
                    {
                        needCallMouseRightUpElements.ForEach(x => x.Events.RightDoubleClick(x));
                        needCallNonMouseRightUpElements.ForEach(x => x.Events.NonRightDoubleClick(x));
                        mouseRightCooldown.CoolDown();
                    }
                    needCallMouseRightUpElements.ForEach(x => x.Events.RightUp(x));
                    needCallNonMouseRightUpElements.ForEach(x => x.Events.NonRightUp(x));
                    needCallMouseRightUpElements.Clear();
                    needCallNonMouseRightUpElements.Clear();
                }
                mouseRightDown = Main.mouseRight;
                SetContainerTop(focusKey);
            }

            mouseLeftCooldown.Update();
            mouseRightCooldown.Update();
        }
        public void Close()
        {
            foreach (ContainerElement ce in Elements.Values)
            {
                if (ce.CloseWhenPlayerCloseInv && ce.IsVisible)
                {
                    ce.Info.IsVisible = false;
                    ce.OnCloseByInv();
                }
            }
        }
        public void ReSetRender()
        {
            Render = new(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight,
                false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        }
        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="sb">画笔</param>
        public void Draw(SpriteBatch sb)
        {
            if (CallOrder.Count == 0 || Elements.Count == 0)
            {
                return;
            }
            ExtraDrawOver?.Invoke(sb);
            ContainerElement child;
            for (int i = CallOrder.Count - 1; i >= 0; i--)
            {
                child = Elements[CallOrder[i]];
                if (child != null && child.IsVisible)
                {
                    child.Draw(sb);
                }
            }
            DrawHoverTip(sb);
        }
        private void DrawHoverTip(SpriteBatch sb)
        {
            if (hoverTip == string.Empty)
                return;
            var font = FontAssets.MouseText.Value;
            Vector2 pos = Main.MouseScreen + new Vector2(16);
            if (hoverTipMaxWidth > 0)
                hoverTip = font.CreateWrappedText(hoverTip, hoverTipMaxWidth);
            Vector2 size = ChatManager.GetStringSize(font, hoverTip, Vector2.One);
            float overX = pos.X + size.X + 20 - Main.screenWidth;
            float overY = pos.Y + size.Y + 10 - Main.screenHeight;
            if (overX > 0)
                pos.X -= overX;
            if (overY > 0)
                pos.Y -= overY;
            Rectangle drawRec = NewRec(pos, size.X + 20, size.Y + 10);
            UICornerPanel.VanillaDraw(sb, drawRec, AssetLoader.VnlBd, Color.Black, 12, 4);
            UICornerPanel.VanillaDraw(sb, drawRec, AssetLoader.VnlBg, VnlColor, 12, 4);
            ChatManager.DrawColorCodedStringWithShadow(sb, font, hoverTip,
                drawRec.TopLeft() + new Vector2(10), Color.White, 0, Vector2.Zero, Vector2.One, -1);
        }

        /// <summary>
        /// 添加子元素
        /// </summary>
        /// <param name="element">需要添加的子元素</param>
        /// <returns>成功时返回true，否则返回false</returns>
        public bool Register(ContainerElement element)
        {
            return Register(element.Name, element);
        }

        /// <summary>
        /// 添加子元素
        /// </summary>
        /// <param name="name">需要添加的子元素的Name</param>
        /// <param name="element">需要添加的子元素</param>
        /// <returns>成功时返回true，否则返回false</returns>
        public bool Register(string name, ContainerElement element)
        {
            if (element == null || Elements.ContainsKey(name) || CallOrder.Contains(name))
            {
                return false;
            }

            Elements.Add(name, element);
            CallOrder.Add(element.Name);
            if (!element.SkipInMenu)
                element.OnInitialization();
            element.PostInitialization();
            element.Calculation();
            return true;
        }

        /// <summary>
        /// 移除子元素
        /// </summary>
        /// <param name="name">需要移除的子元素的Key</param>
        /// <returns>成功时返回true，否则返回false</returns>
        public bool Remove(string name)
        {
            if (CallOrder.Count == 0 || Elements.Count == 0 || !(Elements.ContainsKey(name) || CallOrder.Contains(name)))
            {
                return false;
            }

            Elements.Remove(name);
            CallOrder.Remove(name);
            return true;
        }

        /// <summary>
        /// 将所有容器相对坐标计算为具体坐标
        /// </summary>
        public void Calculation()
        {
            foreach (ContainerElement child in Elements.Values)
            {
                child?.Calculation();
            }
        }
        public void SaveAndQuit()
        {
            foreach (ContainerElement child in Elements.Values)
            {
                child?.OnSaveAndQuit();
            }
        }

        /// <summary>
        /// 将容器置顶
        /// </summary>
        /// <param name="name">需要置顶的容器Name</param>
        /// <returns>成功返回true，否则返回false</returns>
        public bool SetContainerTop(string name)
        {
            if (CallOrder.Count == 0 || Elements.Count == 0 || !(Elements.ContainsKey(name) || CallOrder.Contains(name)))
            {
                return false;
            }

            if (CallOrder[0] == name)
            {
                return true;
            }

            CallOrder.Remove(name);
            CallOrder.Insert(0, name);
            return true;
        }

        /// <summary>
        /// 交换两个容器的顺序
        /// </summary>
        /// <param name="name1">容器1的Name</param>
        /// <param name="name2">容器2的Name</param>
        /// <returns>是否交换成功。成功则返回true，否则返回false</returns>
        public bool ExchangeContainer(string name1, string name2)
        {
            if (CallOrder.Count == 0 || Elements.Count == 0 || !(Elements.ContainsKey(name1) || CallOrder.Contains(name1)) ||
                !(Elements.ContainsKey(name2) || CallOrder.Contains(name2)))
            {
                return false;
            }

            int index1 = CallOrder.FindIndex(x => x == name1);
            int index2 = CallOrder.FindIndex(x => x == name2);
            CallOrder.Remove(name1);
            CallOrder.Remove(name2);
            CallOrder.Insert(index1, name2);
            CallOrder.Insert(index2, name1);
            return true;
        }

        /// <summary>
        /// 寻找开启的顶部容器索引
        /// </summary>
        /// <returns>开启的顶部容器索引</returns>
        public int FindTopContainer()
        {
            return CallOrder.FindIndex(x => Elements[x].IsVisible);
        }
        /// <summary>
        /// 分辨率改变时
        /// </summary>
        public void OnResolutionChange()
        {
            ReSetRender();
            Calculation();
            foreach (KeyValuePair<string, ContainerElement> key in Elements)
            {
                key.Value.OnResolutionChange?.Invoke();
            }
        }
    }
}
