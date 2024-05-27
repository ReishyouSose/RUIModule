using Terraria.Audio;
using Terraria.UI;

namespace RUIModule.RUIElements
{
    public delegate bool CheckPutSlotCondition(Item mouseItem);
    public delegate void ExchangeItemHandler(BaseUIElement target);
    public class UIItemSlot : BaseUIElement
    {
        /// <summary>
        /// 框贴图
        /// </summary>
        public UIIconSlot Slot { get; private set; }
        /// <summary>
        /// 是否可以放置物品
        /// </summary>
        public CheckPutSlotCondition CanPutInSlot { get; set; } = new(x => false);
        /// <summary>
        /// 是否可以拿去物品
        /// </summary>
        public CheckPutSlotCondition CanTakeOutSlot { get; set; } = new(x => false);
        /// <summary>
        /// 物品无限拿取
        /// </summary>
        public bool Infinity;
        public bool IgnoreOne;
        public bool drawStack = true;
        /// <summary>
        /// 框内物品
        /// </summary>
        public Item item;
        public readonly float scale;
        /// <summary>
        /// 更改物品时调用
        /// </summary>
        public event ExchangeItemHandler PostExchangeItem;
        public void ExchangeItem() => PostExchangeItem?.Invoke(this);
        /// <summary>
        /// 玩家拿取物品时调用
        /// </summary>
        public event ExchangeItemHandler OnPickItem;
        public void PickItem() => OnPickItem?.Invoke(this);

        /// <summary>
        /// 玩家放入物品时调用
        /// </summary>
        public event ExchangeItemHandler OnPutItem;
        public void PutItem() => OnPutItem?.Invoke(this);

        /// <param name="texture"></param>
        public UIItemSlot(Item item = null, float scale = 1f)
        {
            Slot = new(null, 0, scale);
            Register(Slot);
            this.item = item?.Clone() ?? new Item();
            if (item != null)
            {
                Main.instance.LoadItem(item.type);
            }
            this.scale = scale;
            int size = (int)(52 * scale);
            SetSize(size, size);
            Info.IsSensitive = true;
        }
        public override void LoadEvents()
        {
            base.LoadEvents();
            Events.OnLeftDown += element =>
            {
                //当鼠标没物品，框里有物品的时候
                ref Item mi = ref Main.mouseItem;
                if (mi.type == ItemID.None && item != null && item.type != ItemID.None)
                {
                    //如果可以拿起物品
                    if (CanTakeOutSlot == null || CanTakeOutSlot(item))
                    {
                        //开启背包
                        Main.playerInventory = true;
                        //拿出物品
                        mi = item.Clone();
                        if (!Infinity)
                        {
                            item = new Item();
                            item.SetDefaults(0, true);
                        }

                        //调用委托
                        PickItem();

                        //触发放物品声音
                        SoundEngine.PlaySound(SoundID.Grab);
                    }
                }
                //当鼠标有物品，框里没物品的时候
                else if (mi.type != ItemID.None && (item == null || item.type == ItemID.None))
                {
                    //如果可以放入物品
                    if (CanPutInSlot == null || CanPutInSlot(mi))
                    {
                        //放入物品
                        item = mi.Clone();
                        mi = new Item();
                        mi.SetDefaults(0, true);

                        //调用委托
                        PutItem();

                        //触发放物品声音
                        SoundEngine.PlaySound(SoundID.Grab);
                    }
                }
                //当鼠标和框都有物品时
                else if (mi.type != ItemID.None && item != null && item.type != ItemID.None)
                {
                    //如果不能放入物品
                    if (!(CanPutInSlot == null || CanPutInSlot(mi)))
                    {
                        //中断函数
                        return;
                    }

                    //如果框里的物品和鼠标的相同
                    if (mi.type == item.type)
                    {
                        if (mi.stack == mi.maxStack || item.stack == item.maxStack)
                        {
                            (mi, item) = (item, mi);
                        }
                        else
                        {
                            //框里的物品数量加上鼠标物品数量
                            item.stack += mi.stack;
                            //如果框里物品数量大于数量上限
                            if (item.stack > item.maxStack)
                            {
                                //计算鼠标物品数量，并将框内物品数量修改为数量上限
                                int exceed = item.stack - item.maxStack;
                                item.stack = item.maxStack;
                                mi.stack = exceed;
                            }
                            //反之
                            else
                            {
                                //清空鼠标物品
                                mi = new Item();
                            }
                        }
                    }
                    //如果可以放入物品也能拿出物品
                    else if ((CanPutInSlot == null || CanPutInSlot(mi))
                        && (CanTakeOutSlot == null || CanTakeOutSlot(item)))
                    {
                        //交换框内物品和鼠标物品
                        Item tmp = mi.Clone();
                        mi = item;
                        item = tmp;
                    }

                    //调用委托
                    ExchangeItem();

                    //触发放物品声音
                    SoundEngine.PlaySound(SoundID.Grab);
                }
                //反之
                else
                {
                    //中断函数
                    return;
                }
            };
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void DrawSelf(SpriteBatch sb)
        {
            Slot.DrawSelf(sb);
            //调用原版的介绍绘制
            if (Info.IsMouseHover && item != null && item.type != ItemID.None)
            {
                Main.hoverItemName = item.Name;
                Main.HoverItem = item.Clone();
            }
            float invScale = Main.inventoryScale;
            Main.inventoryScale = scale;
            ItemSlot.Draw(sb, ref item, 14, HitBox().TopLeft());
            Main.inventoryScale = invScale;
            //获取当前UI部件的信息
            /*Rectangle DrawRectangle = Info.TotalHitBox;
            DrawRectangle.Width = 52;
            DrawRectangle.Height = 52;
            if (item?.IsAir == false)
            {
                Rectangle frame = Main.itemAnimations[item.type] != null ? Main.itemAnimations[item.type]
                    .GetFrame(TextureAssets.Item[item.type].Value) : Item.GetDrawHitbox(item.type, null);
                //绘制物品贴图
                 sb.Draw(TextureAssets.Item[item.type].Value, new Vector2(DrawRectangle.X + DrawRectangle.Width / 2,
                     DrawRectangle.Y + DrawRectangle.Height / 2), frame, Color.White * Opacity, 0f,
                     new Vector2(frame.Width, frame.Height) / 2f, 1 * frame.Size().AutoScale(52 * 0.75f), 0, 0);

                 //绘制物品左下角那个代表数量的数字
                 if (drawStack && (item.stack > 1 || IgnoreOne))
                 {
                     sb.DrawString(font, item.stack.ToString(), new Vector2(DrawRectangle.X + 10, DrawRectangle.Y + DrawRectangle.Height - 20), StackColor * Opacity, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
                 }
            }*/
        }
        public override void DrawChildren(SpriteBatch sb) { }
    }
}
