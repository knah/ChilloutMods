namespace UIExpansionKit.API
{
    public interface ICustomShowableLayoutedMenu<TLayoutParam> : ICustomLayoutedMenu<TLayoutParam>, IShowableMenu where TLayoutParam: struct
    {
        
    }
}