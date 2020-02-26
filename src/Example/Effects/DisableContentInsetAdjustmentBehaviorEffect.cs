using System.Linq;
using Xamarin.Forms;

namespace Example.Effects
{
    public class DisableContentInsetAdjustmentBehaviorEffect : RoutingEffect
    {
        public static readonly BindableProperty DisableContentInsetAdjustmentProperty
            = BindableProperty.CreateAttached("DisableContentInsetAdjustment", typeof(bool), typeof(DisableContentInsetAdjustmentBehaviorEffect), false, propertyChanged: OnChanged);
        
        public static bool GetDisableContentInsetAdjustment(BindableObject view)
        {
            return (bool)view.GetValue(DisableContentInsetAdjustmentProperty);
        }

        public static void SetDisableContentInsetAdjustment(BindableObject view, bool value)
        {
            view.SetValue(DisableContentInsetAdjustmentProperty, value);
        }

        private static void OnChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (View)bindable;
            var effect = (bool)newValue;
            if (effect)
            {
                view.Effects.Add(new DisableContentInsetAdjustmentBehaviorEffect());
                return;
            }

            var toRemove = view.Effects.FirstOrDefault(e => e is DisableContentInsetAdjustmentBehaviorEffect);
            if (toRemove != null)
            {
                view.Effects.Remove(toRemove);
            }
        }
        
        public DisableContentInsetAdjustmentBehaviorEffect() : base ($"Example.{nameof(DisableContentInsetAdjustmentBehaviorEffect)}")
        {
        }
    }
}