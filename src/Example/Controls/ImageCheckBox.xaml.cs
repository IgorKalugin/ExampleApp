using System;
using ReactiveUI;
using Xamarin.Forms;

namespace Example.Controls
{
    public partial class ImageCheckBox
    {
        // ReSharper disable MemberCanBePrivate.Global
        public static readonly BindableProperty IsCheckedProperty =
            BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(ImageCheckBox));
        
        public static readonly BindableProperty CheckedImageProperty =
            BindableProperty.Create(nameof(CheckedImage), typeof(string), typeof(ImageCheckBox));

        public static readonly BindableProperty UncheckedImageProperty =
            BindableProperty.Create(nameof(UncheckedImage), typeof(string), typeof(ImageCheckBox));
        
        public static readonly BindableProperty CornerRadiusProperty = 
            BindableProperty.Create(nameof(CornerRadius), typeof(int), typeof(ImageCheckBox));

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public string CheckedImage
        {
            get => (string)GetValue(CheckedImageProperty);
            set => SetValue(CheckedImageProperty, value);
        }

        public string UncheckedImage
        {
            get => (string)GetValue(UncheckedImageProperty);
            set => SetValue(UncheckedImageProperty, value);
        }

        public int CornerRadius
        {
            get => (int)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        // ReSharper restore MemberCanBePrivate.Global
        
        public ImageCheckBox()
        {
            InitializeComponent();

            this.WhenAnyValue(view => view.IsChecked, view => view.CheckedImage, view => view.UncheckedImage)
                .Subscribe(_ => UpdateImage());

            this.WhenAnyValue(view => view.CornerRadius)
                .Subscribe(cornerRadius => imageBtn.CornerRadius = cornerRadius);

            imageBtn.Events().Clicked
                .Subscribe(_ => IsChecked = !IsChecked);
        }

        private void UpdateImage()
        {
            var image = IsChecked ? CheckedImage : UncheckedImage;
            imageBtn.SetValue(ImageButton.SourceProperty, image);
        }
    }
}