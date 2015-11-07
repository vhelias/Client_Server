using System;
using Android.Content;

namespace libsuperuser_net
{
	public class Application:Android.App.Application
	{
		public static void Toast(Context context, string message) {
			// this is a static method so it is easier to call,
			// as the context checking and casting is done for you

			if (context == null) return;

			if (!(context is Android.App.Application)) {
				context = context.ApplicationContext;
			}

			var appContext = context as libsuperuser_net.Application;
			if (appContext != null) {
				string m = message;
				appContext.RunInApplicationThread (() => {
					Android.Widget.Toast.MakeText(appContext, m, Android.Widget.ToastLength.Long).Show();
				});
			}
				
		}

		public void RunInApplicationThread(Action action){
			action.Invoke ();
		}

		public override void OnCreate ()
		{
			base.OnCreate ();
			try{

				// workaround bug in AsyncTask, can show up (for example) when you toast from a service
				// this makes sure AsyncTask's internal handler is created from the right (main) thread
				Activator.CreateInstance<Android.OS.AsyncTask>();
			}catch{
				// will never happen
			}
		}
	}
}

