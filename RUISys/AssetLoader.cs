using ReLogic.Content;

namespace RUIModule.RUISys
{
    public static class AssetLoader
    {
        private static Texture2D[] baseAssets;
        public static ref Texture2D BackGround => ref baseAssets[0];
        public static ref Texture2D Close => ref baseAssets[1];
        public static ref Texture2D Adjust => ref baseAssets[2];
        public static ref Texture2D Side => ref baseAssets[3];
        public static ref Texture2D Slot => ref baseAssets[4];
        public static ref Texture2D VScrollBD => ref baseAssets[5];
        public static ref Texture2D VScrollInner => ref baseAssets[6];
        public static ref Texture2D HScrollBD => ref baseAssets[7];
        public static ref Texture2D HScrollInner => ref baseAssets[8];
        public static ref Texture2D Unfold => ref baseAssets[9];
        public static ref Texture2D Fold => ref baseAssets[10];
        public static ref Texture2D VnlClose => ref baseAssets[11];
        public static ref Texture2D VnlAdjust => ref baseAssets[12];
        public static ref Texture2D Move => ref baseAssets[13];
        public static ref Texture2D Increase => ref baseAssets[14];
        public static ref Texture2D Decrease => ref baseAssets[15];
        public static ref Texture2D VnlBg => ref baseAssets[16];
        public static ref Texture2D VnlBd => ref baseAssets[17];
        public static Dictionary<string, Texture2D> ExtraAssets;
        public static event Action<Dictionary<string, Texture2D>> ExtraLoad;
        public static Effect edgeBlur;
        /// <summary>
        /// 蓝底0，选中9
        /// <br/>红底1，选中18
        /// <br/>金底13，选中16
        /// <br/>绿2，枯绿5，暗绿7
        /// <br/>蓝紫3，深蓝8，湖蓝14，灰蓝6，暗蓝11
        /// <br/>暗红4，深红10
        /// <br/>白12，白框15，空白17
        /// </summary>
        public static Asset<Texture2D>[] InvSlot;
        public static void Load()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string path = "RUIModule.Assets.";
            string[] files = new string[]
            {
                "BackGround",
                "Close",
                "Adjust",
                "Side",
                "Slot",
                "VScrollBD",
                "VScrollInner",
                "HScrollBD",
                "HScrollInner",
                "Unfold",
                "Fold",
                "Close2",
                "Adjust2",
                "Move",
                "Increase",
                "Decrease"
            };
            Texture2D LoadT2D(string fileName)
            {
                Texture2D texture;
                using (Stream? stream = assembly.GetManifestResourceStream(path + fileName + ".png"))
                {
                    texture = Texture2D.FromStream(Main.graphics.GraphicsDevice, stream);
                }
                return texture;
            }
            int count = files.Length;
            baseAssets = new Texture2D[count + 2];
            for (int i = 0; i < count; i++)
            {
                baseAssets[i] = LoadT2D(files[i]);
            }
            VnlBg = T2D("Terraria/Images/UI/PanelBackground");
            VnlBd = T2D("Terraria/Images/UI/PanelBorder");
            ExtraAssets = new();
            ExtraLoad?.Invoke(ExtraAssets);
            InvSlot = new Asset<Texture2D>[]
            {
                TextureAssets.InventoryBack,
                TextureAssets.InventoryBack2,
                TextureAssets.InventoryBack3,
                TextureAssets.InventoryBack4,
                TextureAssets.InventoryBack5,
                TextureAssets.InventoryBack6,
                TextureAssets.InventoryBack7,
                TextureAssets.InventoryBack8,
                TextureAssets.InventoryBack9,
                TextureAssets.InventoryBack10,
                TextureAssets.InventoryBack11,
                TextureAssets.InventoryBack12,
                TextureAssets.InventoryBack13,
                TextureAssets.InventoryBack14,
                TextureAssets.InventoryBack15,
                TextureAssets.InventoryBack16,
                TextureAssets.InventoryBack17,
                TextureAssets.InventoryBack18,
                TextureAssets.InventoryBack19,
            };
            /*using (Stream? stream = assembly.GetManifestResourceStream(path + "EdgeBlur.xnb"))
            {
                if (stream != null)
                {
                    using BinaryReader reader = new(stream);
                    byte[] effData = reader.ReadBytes((int)stream.Length);
                    EdgeBlur = new(Main.graphics.GraphicsDevice, effData);
                }
            }*/
        }
    }
}
