#region
using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
#endregion
namespace ProcessDashboard.Droid.Fragments
{
    public class Login : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            var lf = inflater.Inflate(Resource.Layout.Login, container, false);
            var login = lf.FindViewById<Button>(Resource.Id.signin_button);
            login.Click += Login_Click;
            return lf;
        }

        private void Login_Click(object sender, EventArgs e)
        {
            // Login logic

            // Switch to next screen
            ((MainActivity) Activity).SwitchToFragment(MainActivity.FragmentTypes.Home);
        }
    }
}