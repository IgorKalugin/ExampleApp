using Xamarin.Forms;

namespace Example.Controls
{
    public class CheckBox : View
    {
        // ReSharper disable MemberCanBePrivate.Global
        public readonly BindableProperty IsCheckedProperty =
            BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(CheckBox), false);

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }
        // ReSharper restore MemberCanBePrivate.Global
    }
}