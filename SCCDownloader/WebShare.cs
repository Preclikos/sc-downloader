using SCCDownloader.WSModels;
using System.Text;
using System.Xml.Serialization;
using XAct;
using XAct.Users;
using SHA1Managed = System.Security.Cryptography.SHA1Managed;

namespace SCCDownloader
{
    public class WebShare
    {
        static protected HttpClient httpClient;
        static readonly String BaseUrl = "https://webshare.cz";

        public WebShare()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public async Task<String> GetLink(string token, string ident, string password)
        {
            var saltParameters = new Dictionary<string, string> { { "wst", token }, { "ident", ident } };
            var saltEncodedContent = new FormUrlEncodedContent(saltParameters);
            var saltResponse = await httpClient.PostAsync("/api/file_password_salt/", saltEncodedContent);

            XmlSerializer saltSerializer = new XmlSerializer(typeof(SaltResponse));
            var saltResult = (SaltResponse)saltSerializer.Deserialize(saltResponse.Content.ReadAsStream());

            if (saltResult.Status != "OK")
            {
                throw new Exception("Salt error");
            }          

            var parameters = new Dictionary<string, string> { { "ident", ident }, { "wst", token }, { "password", HashPassword(password, saltResult.Salt) }, { "download_type", "file_download" } };
            var encodedContent = new FormUrlEncodedContent(parameters);
            var response = await httpClient.PostAsync("/api/file_link/", encodedContent);


            XmlSerializer serializer = new XmlSerializer(typeof(LinkResponse));
            var fileResult = (LinkResponse)serializer.Deserialize(response.Content.ReadAsStream());

            if (fileResult.Status != "OK")
            {
                throw new Exception("Link error");
            }

            return fileResult.Link;
        }

        public async Task<String> GetToken(string userName, string password)
        {
            var salt = await GetSalt(userName);
            var hashedPassword = HashPassword(password, salt);
            var token = await Login(userName, hashedPassword);
            return token;
        }

        private async Task<String> GetSalt(string userName)
        {
            var parameters = new Dictionary<string, string> { { "username_or_email", userName } };
            var encodedContent = new FormUrlEncodedContent(parameters);
            var response = await httpClient.PostAsync("/api/salt/", encodedContent);


            XmlSerializer serializer = new XmlSerializer(typeof(SaltResponse));
            var saltResult = (SaltResponse)serializer.Deserialize(response.Content.ReadAsStream());

            if (saltResult.Status != "OK")
            {
                throw new Exception("Salt error");
            }

            return saltResult.Salt;
        }

        private async Task<String> Login(string userName, string passwordHash)
        {
            var parameters = new Dictionary<string, string> { { "username_or_email", userName }, { "password", passwordHash }, { "keep_logged_in", "1" } };
            var encodedContent = new FormUrlEncodedContent(parameters);
            var response = await httpClient.PostAsync("/api/login/", encodedContent);


            XmlSerializer serializer = new XmlSerializer(typeof(LoginResponse));
            var loginResult = (LoginResponse)serializer.Deserialize(response.Content.ReadAsStream());

            if (loginResult.Status != "OK")
            {
                throw new Exception("Login error");
            }

            return loginResult.Token;
        }

        private String HashPassword(string password, string salt)
        {
            var md5 = MD5Crypt.crypt(password, salt);
            return HashSha1(md5);
        }

        private string HashSha1(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString().ToLower();
            }
        }
    }
}
