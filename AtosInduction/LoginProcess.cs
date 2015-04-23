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
    static class LoginProcess
    {
        private const string tokenFileName = "tokens.json";
        private const string clientSecret = "19sLbhJNc_3EDfGaRwg-Bw48";
        private const string clientId = "476414864576-vupkm8vrpk144jg4fqjqo00qhksblfv0.apps.googleusercontent.com";
        private static readonly OAuthAuthorization auth = new OAuthAuthorization("https://accounts.google.com/o/oauth2/auth", "https://accounts.google.com/o/oauth2/token");
        private static TokenPair tokens;

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
                tokens = tokenPair;
                return tokenPair.AccessToken;
            }
            else
                return await requestNewToken(tokenPair.RefreshToken);
        }

        public static async Task RequireAccess(bool keepLogged)
        {
            TokenPair tokenPair = await auth.Authorize(
                clientId,
                clientSecret,
                new string[] { GoogleScopes.GooglePlus });
            tokens = tokenPair;
            if (keepLogged)
            {
                string json = JsonConvert.SerializeObject(tokenPair);
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
        }

        private static async Task<string> requestNewToken(string refreshToken)
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

        private static async Task<StorageFile> getTokenFile()
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

        public static async Task<bool> isThereTokenFile()
        {
            StorageFile tokenFile = await getTokenFile();

            if (tokenFile != null)
            {
                return true;
            }
            else
                return false;
        }

        public static async void deleteTokenFile()
        {
            await Task.Run(async () =>
            {
                if (await isThereTokenFile())
                {
                    StorageFile file = await getTokenFile();
                    await file.DeleteAsync();
                }
            });
        }
    }
}
