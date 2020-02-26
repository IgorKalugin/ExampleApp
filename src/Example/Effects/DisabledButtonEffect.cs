using System.Linq;
using Xamarin.Forms;

namespace Example.Effects
{
    public class DisabledButtonEffect : RoutingEffect
    {
        // I made the default value equal to Color.Transparent. It would be better if the type was of nullable Color, but it cannot be compiled in that case :/
        public static readonly BindableProperty DisabledButtonTextColorProperty
            = BindableProperty.CreateAttached("DisabledButtonTextColor", typeof(Color), typeof(DisabledButtonEffect), Color.Transparent, propertyChanged: OnDisabledButtonTextColorChanged);
        
        public static Color GetDisabledButtonTextColor(BindableObject view)
        {
            return (Color)view.GetValue(DisabledButtonTextColorProperty);
        }

        public static void SetDisabledButtonTextColor(BindableObject view, Color value)
        {
            view.SetValue(DisabledButtonTextColorProperty, value);
        }

        private static void OnDisabledButtonTextColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (View)bindable;
            var value = (Color)newValue;
            if (value != Color.Transparent)
            {
                view.Effects.Add(new DisabledButtonEffect());
                return;
            }

            var toRemove = view.Effects.FirstOrDefault(e => e is DisabledButtonEffect);
            if (toRemove != null)
            {
                view.Effects.Remove(toRemove);
            }
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        public DisabledButtonEffect() : base($"Example.{nameof(DisabledButtonEffect)}")
        {
        }
    }
}