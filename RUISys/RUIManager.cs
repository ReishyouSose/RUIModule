using Terraria.UI;

namespace RUIModule.RUISys;

public class RUIManager : ModSystem
{
    public static Mod mod;
    public static RUISystem Ins { get; private set; }
    public static Dictionary<string, ContainerElement> UIEs => Ins.Elements;
    private Vector2 resolution;
    private bool invOpen;
    public RUIManager()
    {
        Main.QueueMainThreadAction(() =>
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            AssetLoader.Load();
            Ins = new RUISystem();
            Ins.Load(Mod);
        });
        //Main.OnResolutionChanged += v2 => Ins.OnResolutionChange();
    }
    public override void UpdateUI(GameTime gameTime)
    {
        if (resolution != ScrResolution)
        {
            Ins.OnResolutionChange();
            resolution = ScrResolution;
        }
        Ins.Update(gameTime);
        if (invOpen != Main.playerInventory)
        {
            if (!Main.playerInventory)
                Ins.Close();
            invOpen = Main.playerInventory;
        }
    }
    public override void PreSaveAndQuit() => Ins.SaveAndQuit();
    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (MouseTextIndex != -1)
        {
            layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer(
               Mod.Name + ":RUIManager",
               delegate
               {
                   Ins.Draw(Main.spriteBatch);
                   return true;
               },
               InterfaceScaleType.UI)
           );
        }
    }
}