namespace HR.Business.Interfaces
{
    public interface IRazorService
    {
        string CreateText(string jsonString, string templateName);
        bool IsTemplateCached(string templateName);
        void CacheTemplate(string templateName, string template);
    }
}
