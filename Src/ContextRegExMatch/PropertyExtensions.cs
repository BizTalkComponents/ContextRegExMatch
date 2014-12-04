namespace Shared.PipelineComponents.ContextRegExMatch
{
    public static class PropertyExtensions
    {
        public static string PropertyName(this string property)
        {
            return property.Substring(property.LastIndexOf('#') + 1);
        }

        public static string PropertyNamespace(this string property)
        {
            return property.Substring(0, property.IndexOf('#'));
        }
    }
}
