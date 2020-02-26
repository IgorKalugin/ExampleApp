using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content.Res;
using Android.Util;
using Xamarin.Forms.Platform.Android;

namespace Example.Droid.Controls
{
    public class SentenceFormsEditText : FormsEditText
    {
        private GradientDrawable underline;
        private int underlineWidth;
        
        public SentenceFormsEditText(Context context) : base(context)
        {
            Init();
        }

        private Color underlineColor;
        public Color UnderlineColor
        {
            get => underlineColor;
            set
            {
                underlineColor = value;
                underline.SetStroke(underlineWidth, UnderlineColor);
            }
        }

        private void Init()
        {
            SetPadding(0,0,0,0);

            var backgroundDrawable = (LayerDrawable)ResourcesCompat.GetDrawable(Resources, Resource.Drawable.sentence_picker_background, null);
            underline = (GradientDrawable)backgroundDrawable.GetDrawable(0);
            underlineWidth = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 2, Context.Resources.DisplayMetrics);
            underline.SetStroke(underlineWidth, UnderlineColor);
            this.SetBackground(backgroundDrawable);
        }
    }
}