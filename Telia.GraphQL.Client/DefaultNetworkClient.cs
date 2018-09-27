using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;

namespace Telia.GraphQL
{
    public class DefaultNetworkClient : INetworkClient
    {
        private string endpoint;

        public DefaultNetworkClient(string endpoint)
        {
            this.endpoint = endpoint;
        }

        public string Send(string query)
        {
            var request = (HttpWebRequest)WebRequest.Create(this.endpoint);

            var dataObject = new { query };
            var data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(dataObject));

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            using (var responseStream = response.GetResponseStream())
            {
                using (var streamReader = new StreamReader(responseStream))
                {
                    var dataResponse = JsonConvert.DeserializeObject<dynamic>(streamReader.ReadToEnd());
                    var obj = dataResponse["data"];

                    return JsonConvert.SerializeObject(obj);
                }
            }
        }
    }
}
