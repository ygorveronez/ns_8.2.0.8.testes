using Dominio.ObjetosDeValor.Embarcador.Logistica.GeoLocalizacao;
using Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace Servicos.Embarcador.Logistica
{
    public class GeoCode

    {
        string _key;
        public GeoCode(string key)
        {
            _key = key;

            if (key == "")
                key = "AIzaSyB6e6zUspWGFYrLmABRgI3rsMss_nKW_s4";
        }

        private string GetRequest(string url)
        {

            WebRequest request = HttpWebRequest.Create(url);
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


        public WayPoint BuscarLatLng(string endereco)
        {
            if (string.IsNullOrEmpty(endereco))
                return null;

            string url = string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}", Uri.EscapeDataString(endereco), _key);
            try
            {
                string strresposta = GetRequest(url);

                var resposta = JsonConvert.DeserializeObject<GeoLocalizacao>(strresposta);

                if (resposta.status != "OK")
                {
                    //esse log da a cada 1 segundo. (crie em outro arquivo) seu bostinha.
                    //Servicos.Log.TratarErro($"Erro ao buscar BuscarLatLng {url} - {resposta.status}");
                    return null;
                }

                var waypoint = new WayPoint()
                {
                    Lat = Convert.ToDouble(resposta.results[0].geometry.location.lat),
                    Lng = Convert.ToDouble(resposta.results[0].geometry.location.lng)
                };

                return waypoint;
            }
            catch
            {
                return null;
            }

        }
    }
}
