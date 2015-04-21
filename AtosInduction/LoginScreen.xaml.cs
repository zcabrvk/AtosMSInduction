using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AtosInduction.Resources;
using Microsoft.Phone.Net.NetworkInformation;
using Windows.Storage;
using Windows.Storage.Streams;
using Newtonsoft.Json;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Threading;
using System.IO;

namespace AtosInduction
{
    public partial class LoginScreen : PhoneApplicationPage
    {
        private const string tokenFileName = "tokens.json";
        private const string clientSecret = "19sLbhJNc_3EDfGaRwg-Bw48";
        private const string clientId = "476414864576-vupkm8vrpk144jg4fqjqo00qhksblfv0.apps.googleusercontent.com";
        private static readonly OAuthAuthorization auth = new OAuthAuthorization("https://accounts.google.com/o/oauth2/auth","https://accounts.google.com/o/oauth2/token");
        private static bool keepLogged = false;
        private static TokenPair tokens;

        public LoginScreen()
        {
            InitializeComponent();
        }

        //empty navigation stack
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            while(NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
        }

        public static async Task<string> getAccessToken()
        {
            if (tokens != null)
            {
                if (await testAccessToken(tokens.AccessToken))
                    return tokens.AccessToken;
                else
                    return await requestNewToken(tokens.RefreshToken);
            }
            else
                return await getAccessTokenFromFile();
        }

        private static async Task<bool> testAccessToken(string accessToken)
        {
            //test if token is still valid!
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/oauth2/v1/tokeninfo?fields=audience&access_token=" + accessToken);
            using (HttpWebResponse response = await request.GetResponseAsync())
            {
                if (response.StatusCode == HttpStatusCode.OK) //successful query!
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        string str = streamReader.ReadToEnd();
                        if (str.Substring(str.IndexOf(':') + 3, clientId.Length).CompareTo(clientId) == 0) //double-check authenticity of token to avoid confused deputy problem
                            return true;
                        else
                            return false;
                    }
                }
                else
                    return false;
            }
        }

        private static async Task<string> getAccessTokenFromFile()
        {
            StorageFile tokenFile = await getTokenFile();
            TokenPair tokenPair;
            using (IRandomAccessStream textStream = await tokenFile.OpenReadAsync())
            {
                using (DataReader textReader = new DataReader(textStream))
                {
                    uint textLength = (uint)textStream.Size;
                    await textReader.LoadAsync(textLength);
                    string json = textReader.ReadString(textLength);
                    tokenPair = JsonConvert.DeserializeObject<TokenPair>(json);
                }
            }

            if (await testAccessToken(tokenPair.AccessToken)) //successful query!
            {
                LoginScreen.tokens = tokenPair;
                return tokenPair.AccessToken;
            }
            else
                return await requestNewToken(tokenPair.RefreshToken);
        }

        private async Task RequireAccess()
        {
            TokenPair tokenPair = await auth.Authorize(
                clientId,
                clientSecret,
                new string[] { GoogleScopes.GooglePlus });
            LoginScreen.tokens = tokenPair;
            if (keepLogged)
            {
                string json = JsonConvert.SerializeObject(tokenPair);
                StorageFile tokenFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(LoginScreen.tokenFileName,
                                             CreationCollisionOption.ReplaceExisting);

                using (IRandomAccessStream textStream = await tokenFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (DataWriter textWriter = new DataWriter(textStream))
                    {
                        textWriter.WriteString(json);
                        await textWriter.StoreAsync();
                    }
                }
            }
        }

        private static async Task<string> requestNewToken(string refreshToken)
        {
            // Request a new access token using the refresh token (when the access token was expired)
            TokenPair tokenPair = await auth.RefreshAccessToken(
                clientId,
                clientSecret,
                refreshToken);
            if(tokenPair.AccessToken == null)
            {
                throw new Exception("Request of new access token with refresh token failed!");
            }

            LoginScreen.tokens = tokenPair;
            return tokenPair.AccessToken;
        }

        private static async Task<StorageFile> getTokenFile()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile tokenFile;
            try
            {
                tokenFile = await folder.GetFileAsync(LoginScreen.tokenFileName);
            }
            catch(Exception) {
                tokenFile = null;
            }
            return tokenFile;
        }

        public static async Task<bool> isThereTokenFile()
        {
            StorageFile tokenFile = await LoginScreen.getTokenFile();

            if (tokenFile != null)
            {
                return true;
            }
            else
                return false;
        }

        private async Task Login()
        {
            if(!App.loggedin)
            {
                bool successful = false;
                if (await isThereTokenFile())
                {
                    try
                    {
                        await getAccessTokenFromFile();
                        successful = true;
                    } catch (Exception)
                    {
                        successful = false;
                    }
                }
                if (!successful)
                {
                    try
                    {
                        await RequireAccess();
                    } catch(Exception)
                    {
                        App.loggedin = false; //Access failed
                    }
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Login();
            if(App.loggedin)
                NavigationService.Navigate(new Uri("/PivotMainPage.xaml", UriKind.Relative));
        }

        public static async void deleteTokenFile()
        {
            await Task.Run(async () => {
                if(await isThereTokenFile())
                {
                    StorageFile file = await getTokenFile();
                    await file.DeleteAsync();
                }
            });
        }

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            keepLogged = !keepLogged;
            if(keepLogged == false)
            {
                deleteTokenFile();
            }
        }
    }
}