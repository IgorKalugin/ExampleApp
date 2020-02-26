using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Splat;
using Xamarin.Forms;

namespace Example.Services.SettingsService
{
    public abstract class BaseSettingsService : ISettingsService
    {
        protected BaseSettingsService(IReadonlyDependencyResolver dr)
        {
                ServiceAddress = "http://192.168.88.105:8080";
                ApplicationClientSecret = "here was a secret key";
                ApplicationClientId = 2;
                UserClientSecret = "here was a secret key";
                UserClientId = 2;
                FeedbackEmail = "IEKALUGiN@gmail.com";
        }
        
        public abstract int NotificationsCountLimit { get; }
        
        public virtual TimeSpan ContiguousNotificationsInterval { get; } = TimeSpan.FromMinutes(15);
        
        public virtual TimeSpan NotificationsUpdateThrottle { get; } = TimeSpan.FromSeconds(3);

        public virtual float NearbyDistance { get; } = 100;

        public virtual TimeSpan ProximityNotificationDelay { get; } = TimeSpan.FromSeconds(30);

        public string RuntimePlatform => Device.RuntimePlatform;
        
        public abstract string BaseUrl { get; }
        
        public string ServiceAddress { get; }
        public string ApplicationClientSecret { get; }
        public int ApplicationClientId { get; }
        public string UserClientSecret { get; }
        public int UserClientId { get; }

        public string MailgunApiKey { get; }
        public string MailgunDomain { get; }
        public string FeedbackEmail { get; }

      
    }
}