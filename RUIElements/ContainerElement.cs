namespace RUIModule.RUIElements
{
    public abstract class ContainerElement : BaseUIElement
    {
        public virtual string Name { get => GetType().FullName; }
        public virtual bool AutoLoad { get => true; }
        public virtual bool CloseWhenPlayerCloseInv => true;
        public ContainerElement()
        {
            Info.IsVisible = false;
        }
        public override void OnInitialization()
        {
            base.OnInitialization();
            Info.Width.Set(0f, 1f);
            Info.Height.Set(0f, 1f);
            Info.CanBeInteract = false;
            Info.IsVisible = false;
        }
    }
}
