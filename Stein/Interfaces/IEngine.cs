namespace Stein.Interfaces
{
    public interface IEngine
    {
        public void RegisterPartial(string name, string body);

        public IRenderer CompileTemplate(string body);
    }
}