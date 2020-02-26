using System;

namespace Example.Services.SettingsService
{
    public interface ISettingsService
    {
        #region Notifications
        
        /// <summary>
        /// Gets the limit of notifications on current platform
        /// </summary>
        int NotificationsCountLimit { get; }
        
        /// <summary>
        /// Gets the interval of contiguous notifications
        /// </summary>
        TimeSpan ContiguousNotificationsInterval { get; }
        
        /// <summary>
        /// Gets the interval of notifications update throttle (to not update them too often)
        /// </summary>
        TimeSpan NotificationsUpdateThrottle { get; }
        
        /// <summary>
        /// Gets the distance in meters from Example device locations which will be counted as their nearby status  
        /// </summary>
        float NearbyDistance { get; }
        
        /// <summary>
        /// Get the proximity notification delay
        /// </summary>
        TimeSpan ProximityNotificationDelay { get; }
        
        #endregion
        
        /// <summary>
        /// Gets the runtime platform
        /// </summary>
        /// <remarks>We use this property instead of static field Device.RuntimePlatform to inject it in tests</remarks>
        string RuntimePlatform { get; }
        
        /// <summary>
        /// Gets base url to show embedded html pages on platform 
        /// </summary>
        string BaseUrl { get; }
        
        #region Service
        
        /// <summary>
        /// Gets the service address
        /// </summary>
        string ServiceAddress { get; }
        
        /// <summary>
        /// Gets the client secret to access the service on behalf of the application
        /// </summary>
        string ApplicationClientSecret { get; }
        
        /// <summary>
        /// Gets client id to access the service on behalf of the application 
        /// </summary>
        int ApplicationClientId { get; }
        
        /// <summary>
        /// Gets the client secret to access the service on behalf of user
        /// </summary>
        string UserClientSecret { get; }
        
        /// <summary>
        /// Gets client id to access the service on behalf of user 
        /// </summary>
        int UserClientId { get; }
        
        #endregion
        
        #region Feedback
        
        /// <summary>
        /// Gets the mailgun api key
        /// </summary>
        string MailgunApiKey { get; }
        
        /// <summary>
        /// Gets the mailgun domain to send feedbacks
        /// </summary>
        string MailgunDomain { get; }
        
        /// <summary>
        /// Gets the email which will be used as a feedbacks receiver
        /// </summary>
        string FeedbackEmail { get; }
        
        #endregion
    }
}