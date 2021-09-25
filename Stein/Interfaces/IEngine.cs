namespace Stein.Interfaces
{
    public interface IEngine
    {
        public void ClaimPartials(string directory);

        public IRenderer CompileTemplate(string body);
    }
}