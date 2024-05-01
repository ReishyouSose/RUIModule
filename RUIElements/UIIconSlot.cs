namespace RUIModule.RUIElements;

public class UIIconSlot : BaseUIElement
{
    public int slotID;
    public Texture2D icon;
    public Texture2D overrideSlot;
    public Color? borderColor;
    /// <summary>
    /// 蓝底0，选中9
    /// <br/>红底1，选中18
    /// <br/>金底13，选中16
    /// <br/>绿2，枯绿5，暗绿7
    /// <br/>蓝紫3，深蓝8，湖蓝14，灰蓝6，暗蓝11
    /// <br/>暗红4，深红10
    /// <br/>白12，白框15，空白17
    /// </summary>
    public UIIconSlot(Texture2D icon, int slotID = 0)
    {
        this.icon = icon;
        this.slotID = slotID;
        SetSize(52, 52);
    }
    public override void DrawSelf(SpriteBatch sb)
    {
        DrawSlot(sb);
        if (icon != null)
        {
            sb.Draw(icon, HitBox().Center(), null, Color.White, 0, icon.Size() / 2f, 1f, 0, 0);
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
            UIVnlPanel.VanillaDraw(sb, hitbox, AssetLoader.VnlBd, borderColor.Value, 12, 4);
    }
    public void BorderHoverToGold()
    {
        Events.OnMouseOver += evt => borderColor = Color.Gold;
        Events.OnMouseOut += evt => borderColor = null;
    }
}
