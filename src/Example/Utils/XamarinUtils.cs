using Xamarin.Forms;

namespace Example.Utils
{
    public static class XamarinUtils
    {
        public static T GetFirstParent<T>(Element element)
            where T : class
        {
            var parent = element.Parent;
            while (parent != null)
            {
                var parentOfType = parent as T;
                if (parentOfType != null)
                {
                    return parentOfType;
                }

                parent = parent.Parent;
            }

            return null;
        }
    }
}