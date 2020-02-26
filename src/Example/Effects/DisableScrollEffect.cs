using System.Linq;
using Xamarin.Forms;

namespace Example.Effects
{
    public class DisableScrollEffect : RoutingEffect
    {
        public static readonly BindableProperty DisableScrollProperty
            = BindableProperty.CreateAttached("DisableScroll", typeof(bool), typeof(DisableScrollEffect), false, propertyChanged: OnDisableScrollChanged);
        
        public static bool GetDisableScroll(BindableObject view)
        {
            return (bool)view.GetValue(DisableScrollProperty);
        }

        public static void SetDisableScroll(BindableObject view, bool value)
        {
            view.SetValue(DisableScrollProperty, value);
        }

        private static void OnDisableScrollChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (View)bindable;
            var disableScroll = (bool)newValue;
            if (disableScroll)
            {
                view.Effects.Add(new DisableScrollEffect());
                return;
            }

            var toRemove = view.Effects.FirstOrDefault(e => e is DisableScrollEffect);
            if (toRemove != null)
            {
                view.Effects.Remove(toRemove);
            }
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        public DisableScrollEffect() : base($"Example.{nameof(DisableScrollEffect)}")
        {
        }
    }
}