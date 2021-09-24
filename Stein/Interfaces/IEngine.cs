namespace Stein.Interfaces
{
    public interface IEngine
    {
        //public void RegisterPartial(string name, string body);
        public void ClaimPartials(string directory);

        public IRenderer CompileTemplate(string body);
    }
}