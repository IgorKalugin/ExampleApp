using Android.Content;
using Example.Controls;
using Example.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AView = Android.Views.View;

[assembly: ExportRenderer(typeof(StatusBar), typeof(StatusBarRenderer))]
namespace Example.Droid.Renderers
{
    public class StatusBarRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<StatusBar, AView>
    {
        public StatusBarRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<StatusBar> e)
        {
            base.OnElementChanged(e);
            
            if (Control == null)
            {
                var view = new AView(Context);
                SetNativeControl(view);
            }

            if (Element != null)
            {
                Element.HeightRequest = 0;
            }
        }
    }
}