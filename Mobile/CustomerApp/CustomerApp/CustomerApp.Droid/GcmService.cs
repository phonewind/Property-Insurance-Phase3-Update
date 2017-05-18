using Android.App;
using Android.Content;
using Android.Media;
using Android.Support.V4.App;
using Android.Util;
using Gcm.Client;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CustomerApp;

[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]
//GET_ACCOUNTS is only needed for android versions 4.0.3 and below
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
namespace CustomerApp.Droid
{
    [BroadcastReceiver(Permission = Gcm.Client.Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]
    public class PushHandlerBroadcastReceiver : GcmBroadcastReceiverBase<GcmService>
    {
        public static string[] SENDER_IDS = new string[] { Settings.MobileGcmSenderId };
        //public const string TAG = "CustomerApp-GCM";
    }

    [Service]
    public class GcmService : GcmServiceBase
    {
        public static string RegistrationID { get; private set; }

        public GcmService() : base(PushHandlerBroadcastReceiver.SENDER_IDS)
        { }

        public static async Task RegisterWithMobilePushNotifications()
        {
            MobileServiceClient client = MobileServiceHelper.msInstance.Client;

            if (RegistrationID != null && client != null)
            {
                var push = client.GetPush();

                MainActivity.CurrentActivity.RunOnUiThread(async () =>
                {
                    try
                    {
                        var gcmBody = new JObject {
                            {
                                "data",
                                new JObject {
                                    { "message", "$(Message)" }
                                }
                            }
                        };
                        var template = new JObject {
                            {
                                "genericMessage",
                                new JObject {
                                    {"body", gcmBody}
                                }
                            }
                        };

                        await push.RegisterAsync(RegistrationID, template);
                    }
                    catch (Exception ex)
                    {
                        Utils.TraceStatus("RegisterWithMobilePushNotifications: " + ex.Message);
                    }
                });
            }
        }

        protected override async void OnRegistered(Context context, string registrationId)
        {
            RegistrationID = registrationId;

            if (registrationId != null)
                await RegisterWithMobilePushNotifications();
        }

        protected override void OnMessage(Context context, Intent intent)
        {
            var msg = new StringBuilder();

            if (intent != null && intent.Extras != null)
            {
                foreach (var key in intent.Extras.KeySet())
                    msg.AppendLine(key + "=" + intent.Extras.Get(key).ToString());
            }

            //Store the message
            var prefs = GetSharedPreferences(context.PackageName, FileCreationMode.Private);
            var edit = prefs.Edit();
            edit.PutString("last_msg", msg.ToString());
            edit.Commit();

            var message = intent.Extras.GetString("message");
            if (!string.IsNullOrEmpty(message))
                createNotification("Contoso Insurance", message);
            else
                createNotification("Contoso Insurance - unknown message details", msg.ToString());
        }

        void createNotification(string title, string desc)
        {
            var notification = new Notification.Builder(this)
                .SetContentTitle(title)
                .SetContentText(desc)
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                .SetSmallIcon(Android.Resource.Drawable.SymActionEmail)
                .SetAutoCancel(true).Build();

            var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager.Notify(1, notification);

        }

        protected override void OnError(Context context, string errorId)
        {
            Utils.TraceStatus("GCM Error: " + errorId);
        }

        protected override async void OnUnRegistered(Context context, string registrationId)
        {
            MobileServiceClient client = MobileServiceHelper.msInstance.Client;
            var push = client.GetPush();
            await push.UnregisterAsync();
        }
    }

}