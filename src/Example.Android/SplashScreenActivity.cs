using Android.App;
using Android.OS;

namespace Example.Droid
{
    [Activity(Label = "Example", MainLauncher = true, NoHistory = true, Theme = "@style/SplashScreen")]
    public class SplashScreenActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StartActivity(typeof(MainActivity));
        }
    }
}