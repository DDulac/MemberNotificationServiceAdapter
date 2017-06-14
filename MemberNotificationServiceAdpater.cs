using System;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.IO;
using System.Net;

namespace MemberNotificaitonServiceAdapter
{
	/// <summary>
	/// WCF API Service Contract Public Interface. The interface exposes the service through a service contract
	/// </summary>
	[ServiceContract]
	public interface IMemberNotificationService
	{
		[OperationContract]
		string NotifyAmazonOSMember(string registrationId, string applicationId, string senderId, string message);

		[OperationContract]
		string NotifyAppleOSMember(string registrationId, string applicationId, string senderId, string message);

		[OperationContract]
		string NotifyBlackberryOSMember(string registrationId, string applicationId, string senderId, string message);

		[OperationContract]
		string NotifyFirefoxOSMember(string registrationId, string applicationId, string senderId, string message);

		[OperationContract]
		string NotifyGoogleOSMember(string registrationId, string applicationId, string senderId, string message);

		[OperationContract]
		string NotifyWindowsOSMember(string registrationId, string applicationId, string senderId, string message);
	}

	/// <summary>
	/// WCF API Service. The service class containing the details of how to do the work described by the WCF API [ServiceContract] and  [OperationContract]
	/// </summary>
	public class MemberNotificationService : IMemberNotificationService
	{
		public string NotifyAmazonOSMember(string registrationId, string applicationId, string senderId, string message)
		{
			return string.Format("NotifyAmazonOSMember not implemented, message: {0}", message);
		}

		public string NotifyAppleOSMember(string registrationId, string applicationId, string senderId, string message)
		{
			return string.Format("NotifyAppleOSMember not implemented, message: {0}", message);
		}

		public string NotifyBlackberryOSMember(string registrationId, string applicationId, string senderId, string message)
		{
			return string.Format("NotifyBlackberryOSMember not implemented, message: {0}", message);
		}

		public string NotifyFirefoxOSMember(string registrationId, string applicationId, string senderId, string message)
		{
			return string.Format("NotifyFirefoxOSMember not implemented, message: {0}", message);
		}

		/// <summary>
		/// Notify GoogleOS Member. Sends notification to members with devices running GoogleOS (Android) that registered after installing a Google Play Application
		/// </summary>
		/// <param name="registrationId">Device registration id</param>
		/// <param name="applicationId">Application id</param>
		/// <param name="senderId">GCM/FCM sender id</param>
		/// <param name="message">GCM/FCM notification content</param>
		/// <returns>Result of notification passed as a HTTP Web Request</returns>
		public string NotifyGoogleOSMember(string registrationId, string applicationId, string senderId, string message)
		{
			string retrunValue = String.Empty;

			// Configure a HTTP Web Request to the GCM/FCM notification server
			WebRequest googleNotification = WebRequest.Create("https://android.googleapis.com/gcm/send");
			googleNotification.Method = "post";
			googleNotification.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
			googleNotification.Headers.Add(string.Format("Authorization: key={0}", applicationId));
			googleNotification.Headers.Add(string.Format("Sender: id={0}", senderId));

			string postDataToServer = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.messageId=" + message + "&data.time=" + System.DateTime.Now.ToString() + "®istration_id=" + registrationId + "";
			Byte[] byteArray = Encoding.UTF8.GetBytes(postDataToServer);
			googleNotification.ContentLength = byteArray.Length;

			try
			{
				// Stream a Google request
				Stream googleApiStream = googleNotification.GetRequestStream();

				// Write the request
				googleApiStream.Write(byteArray, 0, byteArray.Length);

				// Close the reqest
				googleApiStream.Close();

				// Stream the Google response
				WebResponse googleNotificationApiResponse = googleNotification.GetResponse();
				googleApiStream = googleNotificationApiResponse.GetResponseStream();

				// Read the response
				StreamReader googleResponseReader = new StreamReader(googleApiStream);
				retrunValue = googleResponseReader.ReadToEnd();

				// Close the response reader
				googleResponseReader.Close();

				// Close the Google API Stream
				googleApiStream.Close();

				// Close the response
				googleNotificationApiResponse.Close();
			}
			catch (Exception ex)
			{
				retrunValue = ex.ToString();
			}
			finally
			{
			}

			return retrunValue;
		}

		public string NotifyWindowsOSMember(string registrationId, string applicationId, string senderId, string message)
		{
			return string.Format("NotifyWindowsOSMember not implemented, message: {0}", message);
		}
	}

	/// <summary>
	/// DDulac
	/// WCF API Service Host Program
	/// Act as host to the WCF API Service under development
	/// Ensure this project is running before executing WcfTestClient.exe, if not you will have issues finding the servic endpoint
	/// 
	/// W3C Push Notification Specification *draft, working definition*
	/// https://www.w3.org/TR/push-api/
	/// https://w3c.github.io/push-api/
	/// 
	/// HTTP response codes
	/// https://developer.mozilla.org/en-US/docs/Web/HTTP/Status
	/// 
	/// FirefoxOSPush API "This is an experimental technology"
	/// Send to sync server pattern, notifications have no payload. The device is told to sync data from the server
	/// Push has been enabled by default on Firefox for Android version 48
	/// https://mozilla-push-service.readthedocs.io/en/latest/
	/// https://developer.mozilla.org/en-US/docs/Web/API/Push_API
	/// https://developer.mozilla.org/en-US/docs/Web/API/PushMessageData
	/// 
	/// FirefoxOSPush Push assigns each remote recipient a unique identifier. {UAID}s are UUIDs in lower case, dashed format. (e.g. ‘01234567-abcd-abcd-abcd-01234567abcd’) This value is assigned during Registration
	/// FirefoxOSPush Push assigns a unique identifier for each subscription for a given {UAID}. Like {UAID}s, {CHID}s are UUIDs in lower case, dashed format. This value is assigned during Channel Subscription
	/// FirefoxOSPush Push assigns each messageId for a given Channel Subscription a unique identifier. This value is assigned during Send Notification
	/// 
	/// GoogleOSPush API
	/// Send to sync server pattern, notifications have no payload. The device is told to sync data from the server
	/// https://android.googleapis.com/gcm/send
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine(" WCF API Services\n");
			Console.WriteLine(" To test this service:\n     Launch a Visual Studio Developer command prompt console window\n     On the command line execute WcfTestClient.exe\n     On the WCF Client dialog\n     Select File\n     Select Add Service\n     Enter WCF API service endpoint you are testing \n     Select OK\n     Test\n\n");

			Uri memberNotificationServiceAddress = new Uri("http://localhost:8080/MemberNotificationService");
			using (ServiceHost host = new ServiceHost(typeof(MemberNotificationService), memberNotificationServiceAddress))
			{
				ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
				smb.HttpGetEnabled = true;
				smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
				host.Description.Behaviors.Add(smb);
				host.Open();
				Console.WriteLine(" Service Address: {0}\n", memberNotificationServiceAddress);
				Console.WriteLine(" Leave this host server running to test the service, then press any key to shut down the host server");
				Console.ReadLine();
				host.Close();
			}
		}
	}
}