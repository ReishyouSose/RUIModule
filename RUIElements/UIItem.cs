using Terraria.UI;

namespace RUIModule.RUIElements
{
    public class UIItem : BaseUIElement
    {
        public Item item;
        public float scale;
        /// <summary>
        /// 是否忽视堆叠显示限制
        /// </summary>
        public bool Ignore;
        public UIItem(int itemid = -1, int stack = 1, float scale = 0.75f)
        {
            Main.instance.LoadItem(itemid);
            item = ContentSamples.ItemsByType[itemid].Clone();
            item.stack = stack;
            this.scale = scale;
            SetSize(24, 24);
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            //调用原版物品介绍
            if (Info.IsMouseHover)
            {
                Main.hoverItemName = item.Name;
                Main.HoverItem = item;
            }
            float old = Main.inventoryScale;
            ref float scale = ref Main.inventoryScale;
            scale = this.scale;
            ItemSlot.Draw(sb, ref item, 14, HitBox().TopLeft() + new Vector2(-7, 26 * scale - 30));
            scale = old;
            /*if (itemid >= 0)
            {
                Rectangle frame = Main.itemAnimations[itemid] != null ? Main.itemAnimations[itemid].GetFrame(TextureAssets.Item[itemid].Value) : Item.GetDrawHitbox(itemid, null);
                //绘制物品贴图
                Vector2 center = Center();
                sb.SimpleDraw(TextureAssets.Item[itemid].Value, center, frame, frame.Size() / 2f, scale * frame.AutoScale(24));

                //绘制物品左下角那个代表数量的数字
                if (stack > 1 || Ignore)
                {
                    ChatManager.DrawColorCodedString(sb, FontAssets.MouseText.Value, stack.ToString(),
                            new Vector2(HitBox().X + 10, HitBox().Y + HitBox().Height - 20),
                            Color.White, 0f, Vector2.Zero, scale * 0.8f);
                }
            }*/
        }
    }
}
