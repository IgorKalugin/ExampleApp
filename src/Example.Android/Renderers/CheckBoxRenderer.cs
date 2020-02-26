using System.ComponentModel;
using Android.Content;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ACheckBox = Android.Widget.CheckBox;
using CheckBox = Example.Controls.CheckBox;
using CheckBoxRenderer = Example.Droid.Renderers.CheckBoxRenderer;

[assembly: ExportRenderer (typeof(CheckBox), typeof(CheckBoxRenderer))]
namespace Example.Droid.Renderers
{
    public class CheckBoxRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<CheckBox, ACheckBox>
    {
        public CheckBoxRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CheckBox> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                Control.CheckedChange -= ControlOnCheckedChange;
            }

            if (Control == null)
            {
                var view = new ACheckBox(Context);
                SetNativeControl(view);
            }
            
            if (e.NewElement != null)
            {
                Control.CheckedChange += ControlOnCheckedChange;
            }
        }
        
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == nameof(Element.IsChecked))
            {
                Control.Checked = Element.IsChecked;
            }
        }
        
        private void ControlOnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Element.IsChecked = e.IsChecked;
        }
    }
}