using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content.Res;
using Android.Util;
using Xamarin.Forms.Platform.Android;

namespace Example.Droid.Controls
{
    public class SentencePickerEditText : PickerEditText
    {
        private GradientDrawable underline;
        private int underlineWidth;

        public SentencePickerEditText(Context context) : base(context)
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
            
            var arrow = ResourcesCompat.GetDrawable(Resources, Resource.Drawable.sentence_picker_arrow, null);
            var bounds = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 20, Context.Resources.DisplayMetrics);
            arrow.SetBounds(0, 0, bounds, bounds);
            SetCompoundDrawables(null, null, arrow, null);
        }
    }
}