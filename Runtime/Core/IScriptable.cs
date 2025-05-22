namespace StackMedia.Scriptables
{
    public interface IScriptable
    {
        SerializedType[] Types { get; }

        string Comment { get; }
    }
}
