using Xamarin.Forms;

namespace Example.Controls
{
    // we can use 9-patched image as a shadow instead of frame
    public class SoftShadow : Frame
    {
        public SoftShadow()
        {
            HasShadow = false;
            IsClippedToBounds = false;
        }
    }
}