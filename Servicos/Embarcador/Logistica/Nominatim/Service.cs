using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Servicos.Embarcador.Logistica.Nominatim
{
    public class Service
    {
        public Service()
        {
            this.format = "json";
            //this.url_servico = "https://nominatim.openstreetmap.org/{0}?";
            this.url_servico = "http://20.195.231.113:8080/nominatim/{0}?";
        }

        public Service(string urlServico)
        {
            this.format = "json";
            this.url_servico = urlServico;
        }

        public string format { get; set; }
        public string url_servico { get; set; }

        //public async Task<RootObject> Reverse(double lat, double lng)
        //{
        //    //https://nominatim.openstreetmap.org/reverse?format=json&lat=-17.16337&lon=-49.21260
        //    string url = string.Format(url_servico, "reverse");
        //    string parametros = string.Format("format={0}&lat={1}&lon={2}", format, lat.ToString(), lng.ToString());
        //    try
        //    {
        //        var client = HttpClientFactoryWrapper.GetClient(nameof(Service));
        //        
        //            var uri = new Uri(string.Concat(url, parametros.Replace(",", ".")));

        //            var request = new HttpRequestMessage()
        //            {
        //                RequestUri = uri,
        //                Method = HttpMethod.Get,
        //            };
        //            request.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
        //            request.Headers.Add("Referer", "http://www.google.com");

        //            HttpResponseMessage response = await client.SendAsync(request);
        //            if (!response.IsSuccessStatusCode)
        //                throw new Exception("Nominatim reverse failed with status code: " + response.StatusCode);
        //            else
        //            {
        //                var content = await response.Content.ReadAsStringAsync();
        //                return JsonConvert.DeserializeObject<RootObject>(content);
        //            }

        //    }
        //    catch (Exception ex) { throw ex; }
        //}

        //public async Task<RootObject> Geocoding(string address)
        //{
        //    //https://nominatim.openstreetmap.org/search?q=RUA%20RIO%20GRANDE%20DO%20SUL,355E,SANTO%20ANTONIO,CHAPEC%C3%93-SC&format=json&polygon=1&addressdetails=2
        //    string url = string.Format(url_servico, "search");
        //    string parametros = string.Format("q={0}&format={1}&polygon=1&addressdetails=1", format, address);
        //    try
        //    {
        //       var client = HttpClientFactoryWrapper.GetClient(nameof(Service));
        //            var uri = new Uri(string.Concat(url, parametros));

        //            var request = new HttpRequestMessage()
        //            {
        //                RequestUri = uri,
        //                Method = HttpMethod.Get,
        //            };
        //            request.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
        //            request.Headers.Add("Referer", "http://www.google.com");

        //            HttpResponseMessage response = await client.SendAsync(request);
        //            if (!response.IsSuccessStatusCode)
        //                throw new Exception("Nominatim reverse failed with status code: " + response.StatusCode);
        //            else
        //            {
        //                var content = await response.Content.ReadAsStringAsync();
        //                return JsonConvert.DeserializeObject<RootObject>(content);
        //            }
        //    }
        //    catch (Exception ex) { throw ex; }
        //}

        private string GetRequest(string url)
        {
            WebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            //ServicePointManager.Expect100Continue = true;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, error) => { return true; };

            ((System.Net.HttpWebRequest)request).UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            ((System.Net.HttpWebRequest)request).Referer = "http://www.google.com";
            string contents = "";
            using (System.Net.HttpWebResponse resp = (System.Net.HttpWebResponse)request.GetResponse())
            {
                if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    using (var responseStream = resp.GetResponseStream())
                    using (var responseStreamReader = new StreamReader(responseStream))
                    {
                        contents = responseStreamReader.ReadToEnd();
                    }
                }
                return contents;
            }
        }

        public RootObject Geocoding(string address)
        {
            //https://nominatim.openstreetmap.org/search?q=RUA%20RIO%20GRANDE%20DO%20SUL,355E,SANTO%20ANTONIO,CHAPEC%C3%93-SC&format=json&polygon=1&addressdetails=2
            string url = string.Format(url_servico, "search");
            string parametros = string.Format("q={0}&format={1}&polygon=1&addressdetails=1", address, format);
            try
            {
                string content = GetRequest(string.Concat(url, parametros));
                List<RootObject> resultado = JsonConvert.DeserializeObject<List<RootObject>>(content);

                if ((resultado?.Count ?? 0) == 0)
                    Servicos.Log.TratarErro("1 - " + address, "Nominatim");
                else if ((resultado?.Count ?? 0) > 1)
                {
                    string resultados = string.Empty;
                    foreach (RootObject point in resultado)
                        resultados += (!string.IsNullOrEmpty(resultados) ? Environment.NewLine : "") + point.lat + " " + point.lon + " - " + point.display_name;

                    Servicos.Log.TratarErro("0 - {" + resultado.Count + "} " + address + Environment.NewLine + resultados, "Nominatim");
                }
                return (resultado?.Count ?? 0) == 0 ? null : resultado[0];
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("2 - " + ex.Message, "Nominatim");
                return null;
            }
        }

        public RootObject GeocodingQueryParameters(string queryString, bool gerarLogNotFound = true)
        {
            //https://nominatim.openstreetmap.org/search?q=RUA%20RIO%20GRANDE%20DO%20SUL,355E,SANTO%20ANTONIO,CHAPEC%C3%93-SC&format=json&polygon=1&addressdetails=2
            string url = string.Format(url_servico, "search");
            string parametros = string.Format("{0}&format={1}&polygon=1&addressdetails=1", queryString, format);
            try
            {
                string content = GetRequest(string.Concat(url, parametros));
                List<RootObject> resultado = JsonConvert.DeserializeObject<List<RootObject>>(content);

                if ((resultado?.Count ?? 0) == 0 && gerarLogNotFound)
                    Servicos.Log.TratarErro("1 - " + queryString, "Nominatim");
                else if ((resultado?.Count ?? 0) > 1)
                {
                    string resultados = string.Empty;
                    foreach (RootObject point in resultado)
                        resultados += (!string.IsNullOrEmpty(resultados) ? Environment.NewLine : "") + point.lat + " " + point.lon + " - " + point.display_name;

                    Servicos.Log.TratarErro("0 - {" + resultado.Count + "} " + queryString + Environment.NewLine + resultados, "Nominatim");
                }
                return (resultado?.Count ?? 0) == 0 ? null : resultado[0];
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("2 - " + ex.Message, "Nominatim");
                return null;
            }
        }

        public RootObject GeocodingByPostalCode(string postalCode)
        {
            postalCode = postalCode.Replace(",", "").Replace(".", "").Replace("-", "").Replace(" ", "");
            postalCode = postalCode.Insert(5, "-");

            //https://nominatim.openstreetmap.org/search?postalcode=83660-000&format=json&polygon=1&addressdetails=2
            string url = string.Format(url_servico, "search");
            string parametros = string.Format("postalcode={0}&format={1}&polygon=1&addressdetails=1", postalCode, format);
            try
            {
                string content = GetRequest(string.Concat(url, parametros));
                List<RootObject> resultado = JsonConvert.DeserializeObject<List<RootObject>>(content);

                if ((resultado?.Count ?? 0) == 0)
                    Servicos.Log.TratarErro("1 - GeocodingByPostalCode - " + postalCode, "Nominatim");
                else if ((resultado?.Count ?? 0) > 1)
                {
                    string resultados = string.Empty;
                    foreach (RootObject point in resultado)
                        resultados += (!string.IsNullOrEmpty(resultados) ? Environment.NewLine : "") + point.lat + " " + point.lon + " - " + point.display_name;

                    Servicos.Log.TratarErro("0 - GeocodingByPostalCode - {" + resultado.Count + "} " + postalCode + Environment.NewLine + resultados, "Nominatim");
                }
                return (resultado?.Count ?? 0) == 0 ? null : resultado[0];
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("2 - GeocodingByPostalCode - " + ex.Message, "Nominatim");
                return null;
            }
        }

        public RootObject Reverse(double lat, double lng)
        {
            //https://nominatim.openstreetmap.org/reverse?format=json&lat=-17.16337&lon=-49.21260
            string url = string.Format(url_servico, "reverse");
            string parametros = string.Format("format={0}&lat={1}&lon={2}", format, lat.ToString(), lng.ToString());
            try
            {
                string content = GetRequest(string.Concat(url, parametros));
                return JsonConvert.DeserializeObject<RootObject>(content);
            }
            catch { return null; }
        }
    }
}
