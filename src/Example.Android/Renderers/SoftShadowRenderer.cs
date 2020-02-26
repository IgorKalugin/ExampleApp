using Android.Content;
using Example.Controls;
using Example.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using FrameRenderer = Xamarin.Forms.Platform.Android.AppCompat.FrameRenderer;

[assembly: ExportRenderer (typeof(SoftShadow), typeof(SoftShadowRenderer))]
namespace Example.Droid.Renderers
{
    public class SoftShadowRenderer : FrameRenderer
    {
        public SoftShadowRenderer(Context context) : base(context)
        {
        }
        
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            Control.CardElevation = 10;
        }
    }
}