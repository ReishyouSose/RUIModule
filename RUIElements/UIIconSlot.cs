namespace RUIModule.RUIElements;
public enum IconSlotID
{
    Blue, // 蓝底0
    Red, // 红底1
    Green, // 绿2
    BluePurple, // 蓝紫3
    DarkRed, // 暗红4
    DesiccatedGreen, // 枯绿5
    GrayBlue, // 灰蓝6
    DarkGreen, // 暗绿7
    DeepBlue, // 深蓝8
    SelectedBlue, // 选中蓝底9
    DeepRed, // 深红10
    DarkBlue, // 暗蓝11
    White, // 白12
    GoldBase, // 金底13
    LakeBlue, // 湖蓝14
    WhiteFrame, // 白框15
    SelectedGold, // 选中金底16
    Blank, // 空白17
    SelectedRed  // 选中红底18
}
public class UIIconSlot : BaseUIElement
{
    /// <summary>
    /// 蓝底0，选中9
    /// <br/>红底1，选中18
    /// <br/>金底13，选中16
    /// <br/>绿2，枯绿5，暗绿7
    /// <br/>蓝紫3，深蓝8，湖蓝14，灰蓝6，暗蓝11
    /// <br/>暗红4，深红10
    /// <br/>白12，白框15，空白17
    /// </summary>
    public int slotID;
    public Texture2D icon;
    public Texture2D overrideSlot;
    public Color? borderColor;
    public float scale;
    /// <summary>
    /// 蓝底0，选中9
    /// <br/>红底1，选中18
    /// <br/>金底13，选中16
    /// <br/>绿2，枯绿5，暗绿7
    /// <br/>蓝紫3，深蓝8，湖蓝14，灰蓝6，暗蓝11
    /// <br/>暗红4，深红10
    /// <br/>白12，白框15，空白17
    /// </summary>
    public UIIconSlot(Texture2D icon, int slotID = 0, float scale = 1)
    {
        this.icon = icon;
        this.slotID = slotID;
        int size = (int)(52 * scale);
        SetSize(size, size);
        this.scale = scale;
    }
    public override void DrawSelf(SpriteBatch sb)
    {
        DrawSlot(sb);
        Rectangle hitbox = HitBox();
        if (icon != null)
        {
            Vector2 size = icon.Size();
            sb.SimpleDraw(icon, hitbox.Center(), null, size / 2f, new(size.AutoScale(hitbox.Size() * 0.75f)));
        }
    }
    public void DrawSlot(SpriteBatch sb)
    {
        Texture2D slot;
        Rectangle hitbox = HitBox();
        if (overrideSlot != null)
            slot = overrideSlot;
        else
            slot = AssetLoader.InvSlot[slotID].Value;
        sb.Draw(slot, hitbox, Color.White);
        if (borderColor.HasValue)
            UICornerPanel.VanillaDraw(sb, hitbox, AssetLoader.VnlBd, borderColor.Value, 12, 4);
    }
    public void BorderHoverToGold()
    {
        Events.OnMouseOver += evt => borderColor = Color.Gold;
        Events.OnMouseOut += evt => borderColor = null;
    }
}
