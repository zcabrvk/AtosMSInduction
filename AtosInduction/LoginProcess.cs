using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace AtosInduction
{
    class LoginProcess : Database
    {
        private const string tokenFileName = "tokens.json";
        private const string clientSecret = "19sLbhJNc_3EDfGaRwg-Bw48";
        private const string clientId = "476414864576-vupkm8vrpk144jg4fqjqo00qhksblfv0.apps.googleusercontent.com";
        private static readonly OAuthAuthorization auth = new OAuthAuthorization("https://accounts.google.com/o/oauth2/auth", "https://accounts.google.com/o/oauth2/token");
        private static TokenPair tokens;

        public async Task<string> getUserFullName()
        {
            string key;
            try
            {
                key = await getAccessToken();
            }
            catch (Exception)
            {
                key = "";
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/plus/v1/people/me?fields=displayName");
            request.Method = "GET";
            request.Headers["Authorization"] = "Bearer " + key;
            string fullName;

            using (HttpWebResponse response = await request.GetResponseAsync())
            {
                if (response.StatusCode == HttpStatusCode.OK) //successful query!
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        string str = streamReader.ReadToEnd();
                        string name;
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(str).TryGetValue("displayName", out name);
                        fullName = name;
                    }
                }
                else
                    fullName = "";
            }

            return fullName;
        }

        public async Task forceLogout()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://accounts.google.com/o/oauth2/revoke?token=" + await getAccessToken());
            await request.GetResponseAsync();
            if (await areUserDetailsLogged())
                await deleteLoginDetails();
        }

        private async Task<string> getAccessToken()
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

        private async Task<bool> testAccessToken(string accessToken)
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

        public async Task loginFromStoredDetails()
        {
            await getAccessToken();
        }

        public async Task storeLoginDetails()
        {
            if (tokens == null)
                throw new Exception("Cannot store login details since the user is not Logged In");

            string json = JsonConvert.SerializeObject(tokens);
            StorageFile tokenFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(tokenFileName,
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

        private async Task<string> getAccessTokenFromFile()
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
                tokens = tokenPair;
                return tokenPair.AccessToken;
            }
            else
                return await requestNewToken(tokenPair.RefreshToken);
        }

        public async Task performLoginProcess()
        {
            TokenPair tokenPair = await auth.Authorize(
                clientId,
                clientSecret,
                new string[] { GoogleScopes.GooglePlus });
            tokens = tokenPair;
        }

        private async Task<string> requestNewToken(string refreshToken)
        {
            // Request a new access token using the refresh token (when the access token was expired)
            TokenPair tokenPair = await auth.RefreshAccessToken(
                clientId,
                clientSecret,
                refreshToken);
            if (tokenPair.AccessToken == null)
            {
                throw new Exception("Request of new access token with refresh token failed!");
            }

            tokens = tokenPair;
            return tokenPair.AccessToken;
        }

        private async Task<StorageFile> getTokenFile()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile tokenFile;
            try
            {
                tokenFile = await folder.GetFileAsync(tokenFileName);
            }
            catch (Exception)
            {
                tokenFile = null;
            }
            return tokenFile;
        }

        public async Task<bool> areUserDetailsLogged()
        {
            StorageFile tokenFile = await getTokenFile();

            if (tokenFile != null)
            {
                return true;
            }
            else
                return false;
        }

        public async Task deleteLoginDetails()
        {
            await Task.Run(async () =>
            {
                if (await areUserDetailsLogged())
                {
                    StorageFile file = await getTokenFile();
                    await file.DeleteAsync();
                }
            });
        }
    }
}
