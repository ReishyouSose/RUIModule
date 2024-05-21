using Terraria.UI;

namespace RUIModule.RUISys;

public class RUISystem : ModSystem
{
    public static RUIManager Ins { get; private set; }
    public static Dictionary<string, ContainerElement> UIs => Ins.Elements;
    public static RenderTarget2D Render => Ins.Render;
    private Vector2 resolution;
    private bool invOpen;
    public RUISystem()
    {
        Main.QueueMainThreadAction(() =>
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            AssetLoader.Load();
            Ins = new RUIManager();
            Ins.Load();
        });
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
               Mod.Name + ":RUISystem",
               delegate
               {
                   var sb = Main.spriteBatch;
                   UISpbState(sb, false);
                   Ins.Draw(sb);
                   return true;
               },
               InterfaceScaleType.UI)
           );
        }
    }
}