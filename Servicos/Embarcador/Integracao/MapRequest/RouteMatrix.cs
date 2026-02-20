using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;



namespace Servicos.Embarcador.Integracao.MapRequest
{
    public class RouteMatrix : ServicoBase
    {
        public RouteMatrix(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        private string key = "FUq6EsAdVp5HuCVBfOf0AVyAAaV2EP4B";
        public Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.routeMatrix BuscarDistancias(List<Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.location> pontos)
        {
            string jsonLocalidade = Newtonsoft.Json.JsonConvert.SerializeObject(pontos);
            string url = "http://www.mapquestapi.com/directions/v2/routematrix?key=" + key + "&outFormat=json&inFormat=json"; //&json={location:" + jsonLocalidade + ",options:{thumbMaps:false}}";

            HttpWebRequest requestMapRequest = (HttpWebRequest)WebRequest.Create(url);

            string postData = "{locations: " + jsonLocalidade + ",options:{unit:k, allToAll: false}}";
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

            Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.routeMatrix routeMatrix = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.routeMatrix>(responseString);

            response.Close();
            response.Dispose();

            return routeMatrix;
        }
    }
}
