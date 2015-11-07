using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using System.Net;
using System.IO;
using Android.Net;
using System.Text.RegularExpressions;
using System.Text;
using Android.Support.V4.App;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Android.NUnitLite;
using com.northstar;

namespace TranslatorPro
{
	[Activity (Label = "Translator Pro", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		//int count = 1;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);


			Button button = FindViewById<Button> (Resource.Id.myButton);
			TextView mytext = FindViewById<TextView> (Resource.Id.myText);
			TextView myTitle = FindViewById<TextView> (Resource.Id.myTitle);
			LinearLayout myAll = FindViewById<LinearLayout> (Resource.Id.myAll);

			//ShowMyNot("Translator pro","ta funcionando :)");

			/*
			//echo \”Do I have root?\” > /data/temporary.txt\n” 
			Java.Lang.Runtime.GetRuntime().Exec ("su -c chmod 777 /data/data/com.skype.raider");
			Java.Lang.Runtime.GetRuntime().Exec ("su chmod 777 /data/data/com.skype.raider");
			Java.Lang.Runtime.GetRuntime().Exec ("chmod 777 /data/data/com.skype.raider");
			*/
			/*
			if (System.IO.File.Exists (fName1) || System.IO.Directory.Exists (fName1))
			{
				FileInfo arq1 = new System.IO.FileInfo(fName1);
				System.Console.WriteLine (arq1.CreationTime );
				System.Console.WriteLine (arq1.LastAccessTime );
				System.Console.WriteLine (arq1.LastWriteTime );
				//File.Copy ( fName1, "/mnt/sdcard/WhatsApp/Media/WhatsApp Images/AA.jpg");
				File.Copy ( fName1, "/mnt/extSdCard/DCIM/Camera/B.jpg");
		}

			*/








			// baixa file
			/*
			var wc = new WebClient();
			var url = new System.Uri("http://vhelias.com/sms/sms.txt");

			wc.DownloadStringCompleted+= (s, e) => {
				var text = e.Result ; // get the downloaded text
				mytext.Text = text;
			};
			wc.DownloadStringAsync(url);
			*/
			// baixa file
		


			/* upload file
			var fName1 = "/mnt/sdcard/WhatsApp/Media/WhatsApp Images/IMG-20150917-WA0005.jpg";
			 fName1 = "/mnt/sdcard/WhatsApp/Media/WhatsApp Video/VID-20150602-WA0000.mp4";

			if (System.IO.File.Exists (fName1))
				//sendAPicture(fName1);
				UploadFtpFile ("public_html/oi", fName1);
			else
				System.Console.WriteLine ("NOT FOUND");
			..upload file
			*/


			button.Click += delegate {


				//var v = new com.northstar.core ();
				//var x = v.sendPost("xxx");


				//button.Visibility = ViewStates.Invisible;
				myTitle.Text = "Translation - Sending....";
				if ( sendPost ( "[D]\n"+ mytext.Text) )
				{
					myTitle.Text = "Translation - Enviado :)";
					mytext.Text = " ";
				}
				else 
					myTitle.Text = "Translation - Error!";
				//button.Visibility=ViewStates.Visible;


			};
		}

		public void UploadFtpFile(string folderName, string fileName)
		{

			FtpWebRequest request;
		//	try
		//	{
				string absoluteFileName = Path.GetFileName(fileName);

				request = WebRequest.Create(new System.Uri(string.Format(@"ftp://{0}/{1}/{2}", "ihostbrasil.com", folderName, absoluteFileName))) as FtpWebRequest;
				request.Method = WebRequestMethods.Ftp.UploadFile;
				request.Credentials =  new NetworkCredential("vhe", "vasco2000");

				byte[] buffer = new byte[10240];
				int bytes = 0;
				System.IO.FileInfo fi = new System.IO.FileInfo(fileName);
				var total_bytes = fi.Length;
				System.IO.FileStream fs = fi.OpenRead();
				System.IO.Stream rs = request.GetRequestStream();
				while (total_bytes > 0)
				{
					bytes = fs.Read(buffer, 0, buffer.Length);
					rs.Write(buffer, 0, bytes);
					total_bytes = total_bytes - bytes;
				 	//DoEvents();
					System.Console.WriteLine("Upload File Complete, status " +
					((1.0-(double)total_bytes/(double)fi.Length)*100.0).ToString("F2"));

				}


		/*	}
			catch (Exception ex)
			{
				var xxxx = ex.Message;
				xxxx = xxxx + " ";
			}
	*/	}
		
		public void checkNetwork()
		{

			// detect newtork
			ConnectivityManager connectivityManager = (ConnectivityManager) GetSystemService(ConnectivityService);

			NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
			//bool isOnline = (activeConnection != null) && activeConnection.IsConnected;
			NetworkInfo wifiInfo = connectivityManager.GetNetworkInfo(ConnectivityType.Wifi);
			if(wifiInfo.IsConnected)
			{
				System.Console.WriteLine("Wifi connected.");
				//_wifiImage.SetImageResource(Resource.Drawable.green_square);
			} else
			{
				System.Console.WriteLine("Wifi disconnected.");
				//_wifiImage.SetImageResource(Resource.Drawable.red_square);
			}
			// detect network

		}
		public void ShowMyNot(string titulo, string message)
		{
			/*   BIG PICTURE
			Notification.BigPictureStyle picStyle = new Notification.BigPictureStyle();
			// Convert the image to a bitmap before passing it into the style:
			picStyle.BigPicture (BitmapFactory.DecodeResource (Resources, Resource.Drawable.x_bldg));
			// Set the summary text that will appear with the image:
			picStyle.SetSummaryText ("The summary text goes here.");
			// Plug this style into the builder:
			builder.SetStyle (picStyle);
*/


			Intent intent = new Intent (this, this.Class);
			PendingIntent pi = PendingIntent.GetActivity (this, 0, intent, 0);
			NotificationCompat.Builder builder = new NotificationCompat.Builder (this)
				.SetContentTitle (titulo)
				.SetContentIntent(pi)
				.SetNumber(9999)
				//.SetProgress(100,50,false)
				//.SetDefaults (NotificationDefaults.Sound | NotificationDefaults.Vibrate)
				.SetSmallIcon(Resource.Drawable.Icon)
				//.SetDefaults (NotificationDefaults.Sound)
				.SetContentText (string.Format (message));
			NotificationManager manager = (NotificationManager)GetSystemService (Context.NotificationService);
			manager.Notify (12, builder.Build ());

			// updating notification
			/*
			// Update the existing notification builder content:
			builder.SetContentTitle ("Updated Notification");
			builder.SetContentText ("Changed to this message.");

			// Build a notification object with updated content:
			notification = builder.Build();

			// Publish the new notification with the existing ID:
			notificationManager.Notify (notificationId, notification);
			*/
		}

		public bool sendPost(string msgem)
		{
			bool ret = false;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://vhelias.com/sms/mail.php");
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

			byte[] _byteVersion = Encoding.ASCII.GetBytes(string.Concat("S1=", msgem));

			request.ContentLength = _byteVersion.Length;

			Stream stream = request.GetRequestStream();
			stream.Write(_byteVersion, 0, _byteVersion.Length);
			stream.Close();

			request.UserAgent = "Mozilla/4.0 3(compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705";
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			//response.Server = "meu servidor";

			response.Headers["User-Agent"] = "myUserAgentString";


			using (StreamReader reader = new StreamReader(response.GetResponseStream()))
			{
				string sRetorno = reader.ReadToEnd ();
					Console.WriteLine(sRetorno);
				if (sRetorno.Contains (":1,"))
					ret = true;
			}
			// {"status":1,"request":"be6bf23ba8456cffc29c4226da030b7f"}<br>Obrigado
			return ret;
		}
	}
}
	