using Terraria.UI;

namespace RUIModule.RUISys;

public class RUIManager : ModSystem
{
    public static Mod mod;
    public static RUISystem Ins { get; private set; }
    public static Dictionary<string, ContainerElement> UIEs => Ins.Elements;
    public static RenderTarget2D render;
    private Vector2 resolution;
    public RUIManager()
    {
        Main.QueueMainThreadAction(() =>
        {
            AssetLoader.Load();
            Ins = new RUISystem();
            Ins.Load(Mod);
            Main.OnResolutionChanged += v2 =>
            {
                Ins.OnResolutionChange();
                render = new(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            };
            render = new(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
        });
    }
    public override void UpdateUI(GameTime gameTime)
    {
        //Main.NewText("up");
        if (resolution != ScrResolution)
        {
            Ins.OnResolutionChange();
            resolution = ScrResolution;
        }
        Ins.Update(gameTime);
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