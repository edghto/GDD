using System;
using System.Threading;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using System.Collections.Generic;
using Google.Apis.Services;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Google.Apis.Util.Store;
using System.IO;
using Google.Apis.Upload;

namespace GDrive
{
    public class Proxy : Generic.IWebAuthenticationContinuable
    {
        private const string CREDITENTIALS_JSON = "ms-appx:///Assets/client_secrets.json";
        private const string APPLICATION_NAME = "GDriveTest";

        public bool IsAuthorized
        {
            get; private set;
        }


        #region Implementation of interface Generic.IWebAuthenticationContinuable
        public async void ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args)
        {
            await ContinueWebAuthenticationAsync(args);
        }
        public async Task ContinueWebAuthenticationAsync(WebAuthenticationBrokerContinuationEventArgs args)
        {
            await PasswordVaultDataStore.Default.StoreAsync<SerializableWebAuthResult>(
                SerializableWebAuthResult.Name,
                new SerializableWebAuthResult(args.WebAuthenticationResult));
        }
        #endregion


        #region Singleton pattern stuff
        private static Proxy instance;
        private Proxy() { }

        public static Proxy GetInstance()
        {
            if (instance == null)
            {
                instance = new Proxy();
            }

            return instance;
        }
        #endregion


        #region Concrete implementation
        private DriveService service;
        private object mutex = new object();
        //private Semaphore sem;

        public async Task InitAsync()
        {
            await PasswordVaultDataStore.Default.DeleteAsync<SerializableWebAuthResult>(
                SerializableWebAuthResult.Name);
        }

        public bool Disconnect()
        {
            return true;
        }

        public bool Connect()
        {
            return ConnectAsync().Result;
        }

        public async Task<bool> ConnectAsync()
        {
            bool result = true;

            try
            {
                UserCredential credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new Uri(CREDITENTIALS_JSON),
                    new[] { DriveService.Scope.Drive },
                    "user",
                    CancellationToken.None);
                
                service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credentials,
                    ApplicationName = APPLICATION_NAME,
                });

                if (credentials != null && service != null)
                {
                    IsAuthorized = true;
                }
            }
            catch(Exception e)
            {
                result = false;
#if DEBUG
                System.Diagnostics.Debug.WriteLine("An error occurred: " + e.Message);
#endif
            }

            return result;
        }

        public IList<File> GetListing(string directoryId)
        {
            string queryString = String.Format("'{0}' in parents and trashed=false", directoryId);
            IList<File> files = new List<File>();

            lock (mutex)
            {
                if (service == null)
                    return new List<Google.Apis.Drive.v2.Data.File>();

                FilesResource.ListRequest request = service.Files.List();
                if (directoryId.Length > 0)
                    request.Q = queryString;
                FileList fileList = request.Execute();
                files = fileList.Items;
            }

            return files;
        }

        public Stream Download(Google.Apis.Drive.v2.Data.File file)
        {
            lock (mutex)
            {
                if (!String.IsNullOrEmpty(file.DownloadUrl))
                {
                    try
                    {
                        var x = service.HttpClient.GetStreamAsync(file.DownloadUrl);
                        return x.Result;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task Upload(Stream stream, Google.Apis.Drive.v2.Data.File file)
        {
            try
            {
                FilesResource.InsertMediaUpload request = service.Files.Insert(file, stream, file.MimeType);
                request.ProgressChanged += Upload_ProgressChanged;
                request.ResponseReceived += Upload_ResponseReceived;
                var progress = await request.UploadAsync();
                //sem = new Semaphore(0, 1, "GDrive_FileUpload");
                //await Task.FromResult(sem.WaitOne());
            }
            catch (Exception e)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("An error occurred: " + e.Message);
#endif
            }
        }
        #endregion


        void Upload_ProgressChanged(IUploadProgress progress)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(progress.Status + " " + progress.BytesSent);
#endif
        }

        void Upload_ResponseReceived(File file)
        {
            //sem.Release();
#if DEBUG
            System.Diagnostics.Debug.WriteLine(file.Title + " was uploaded successfully");
#endif
        }
    }
}
