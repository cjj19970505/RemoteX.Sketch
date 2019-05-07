using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;

namespace RemoteX.Sketch.XamExapmple.Droid
{
    [Activity(Label = "RemoteX.Sketch.XamExapmple", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public ManagerManagerFragment ManagerManagerFragment { get; private set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            RequestWindowFeature(WindowFeatures.NoTitle);//设置无标题
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);//设置全屏

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            ManagerManagerFragment = DependencyService.Get<IManagerManager>() as ManagerManagerFragment;
            FragmentManager.BeginTransaction().Add(ManagerManagerFragment, "ManagerManager").Commit();

            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            (ManagerManagerFragment.InputManager as RemoteX.Input.Droid.InputManager).OnTouch(ev);
            return base.DispatchTouchEvent(ev);
            

        }
    }
}