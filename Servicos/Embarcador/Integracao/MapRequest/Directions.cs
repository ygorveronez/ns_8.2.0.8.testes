using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;


namespace Servicos.Embarcador.Integracao.MapRequest
{
    public class Directions : ServicoBase
    {
        public Directions(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        private string key = "FUq6EsAdVp5HuCVBfOf0AVyAAaV2EP4B";
        public Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.route OtimizarRota(List<Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.location> pontos, string tipoRota)
        {
            string jsonLocalidade = Newtonsoft.Json.JsonConvert.SerializeObject(pontos);
            string url = "http://www.mapquestapi.com/directions/v2/optimizedroute?key=" + key + "&outFormat=json&inFormat=json"; //&json={location:" + jsonLocalidade + ",options:{thumbMaps:false}}";
            
            HttpWebRequest requestMapRequest = (HttpWebRequest)WebRequest.Create(url);

            string postData = "{locations: " + jsonLocalidade + ",options:{unit:k, routeType: " + tipoRota + "}}";
            var data = Encoding.UTF8.GetBytes(postData);

            requestMapRequest.Method = "POST";
            requestMapRequest.AllowAutoRedirect = true;
            requestMapRequest.ContentLength = data.Length;

            using (var stream = requestMapRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)requestMapRequest.GetResponse();

            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.Route route = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.Route>(responseString);
            
            response.Close();
            response.Dispose();

            return route.route;
        }

    }
}
