using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Google
{
    public class GoogleMaps : ServicoBase
    {        
        public GoogleMaps(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        private string key = "AIzaSyDRRjOpBK_JMU9RYd7MHBUglFrBhD3N6WM";
        public Dominio.ObjetosDeValor.Embarcador.Integracao.Google.routes OtimizarRota(List<Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.location> pontos, string tipoRota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao)
        {
            string origem = pontos[0].latLng.lat.ToString().Replace(",",".") + "," + pontos[0].latLng.lng.ToString().Replace(",", ".");

            string waypoint = "";
            for (int i = 1; i < pontos.Count - 1; i++)
            {
                if (i == pontos.Count - 2)
                    waypoint += pontos[i].latLng.lat.ToString().Replace(",", ".") + "," + pontos[i].latLng.lng.ToString().Replace(",", ".");
                else
                    waypoint += pontos[i].latLng.lat.ToString().Replace(",", ".") + "," + pontos[i].latLng.lng.ToString().Replace(",", ".") + "|";
            }

            string destino = pontos[pontos.Count - 1].latLng.lat.ToString().Replace(",", ".") + "," + pontos[pontos.Count - 1].latLng.lng.ToString().Replace(",", ".");

            string url = "https://maps.googleapis.com/maps/api/directions/json?optimize=true&origin=" + origem + "&destination=" + destino + "&waypoints=" + waypoint + "&key=" + key;

            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string responseString = new StreamReader(resp.GetResponseStream()).ReadToEnd();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Google.Route route = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Google.Route>(responseString);
            sr.Dispose();
            sr.Close();

            return route.routes.FirstOrDefault();
        }
      

    }
   

}