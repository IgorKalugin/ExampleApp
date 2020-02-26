using Xamarin.Forms;

namespace Example.Utils
{
    public static class ResourcesUtils
    {
        public static T GetResource<T>(string resourceKey)
        {
            if (Application.Current.Resources.TryGetValue(resourceKey, out var value))
            {
                if (value is T resource)
                {
                    return resource;
                }
            }

            return default(T);
        }
    }
}