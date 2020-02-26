namespace Example.Themes.MainTheme
{
    /* We need xaml.cs file for this resource dictionary because it references other resource dictionaries, which doesn't work correctly in Xamarin.Forms.
     * Internally Xamarin.Forms searches for all merged resources in current assembly of root element, but if resource dictionary doesn't have xaml.cs file,
     * then its assembly is Xamarin.Forms.Core, which doesn't have our resource dictionaries */ 
    public partial class MainTheme
    {
        public MainTheme()
        {
            InitializeComponent();
        }
    }
}