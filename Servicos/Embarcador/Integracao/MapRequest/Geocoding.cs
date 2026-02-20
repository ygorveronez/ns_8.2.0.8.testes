using System.IO;
using System.Net;
using System.Text;

namespace Servicos.Embarcador.Integracao.MapRequest
{
    public class Geocoding
    {
        private string key = "FUq6EsAdVp5HuCVBfOf0AVyAAaV2EP4B";

        public Geocoding() { }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.location BuscarLocalidade(Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.location localidade)
        {
            string jsonLocalidade = Newtonsoft.Json.JsonConvert.SerializeObject(localidade);
            string url = "http://www.mapquestapi.com/geocoding/v1/address?key=" + key + "&outFormat=json&inFormat=json";

            HttpWebRequest requestMapRequest = (HttpWebRequest)WebRequest.Create(url);

            string postData = "{location: " + jsonLocalidade + ",options:{thumbMaps:false}}";
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

            dynamic dynRetorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseString);

            response.Close();
            response.Dispose();

            if (dynRetorno != null)
            {
                int codigoRetorno = (int)dynRetorno.info.statuscode;
                if (codigoRetorno == 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.location retornoLoca = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.location>(Newtonsoft.Json.JsonConvert.SerializeObject(dynRetorno.results[0].locations[0]));
                    return retornoLoca;
                }
            }
            return null;
        }

        public Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas BuscarCoordenadasEndereco(Dominio.Entidades.Localidade localidade, string endereco, string numero)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas coordenadas = new Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas();

            Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.location location = new Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.location();
            location.adminArea1 = localidade.CodigoIBGE != 9999999 ? "BR" : localidade.Pais.Abreviacao;
            location.city = localidade.Descricao;
            location.state = localidade.Estado.Sigla;
            if (numero.ToLower().Trim() == "sn" || numero.ToLower().Trim() == "s/n" || numero.ToLower().Trim() == "s\n")
                numero = "";

            if (!string.IsNullOrEmpty(numero))
                location.street = endereco + " " + numero;
            else
                location.street = endereco;

            Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.location retorno = BuscarLocalidade(location);

            if (retorno != null && retorno.adminArea1Type.ToLower() == "country" && retorno.adminArea1.ToUpper() == localidade.Pais.Abreviacao.ToUpper() && ( localidade.CodigoIBGE == 9999999 || (retorno.adminArea3Type.ToLower() == "state" && Utilidades.String.RemoveDiacritics(retorno.adminArea3.ToLower().Replace(" ", "")) == Utilidades.String.RemoveDiacritics(localidade.Estado.Nome.ToLower().Replace(" ", "")))))
            {
                coordenadas.tipoLocalizacao = validarTipoLocalizacaoRetornoAPI(retorno);

                if (coordenadas.tipoLocalizacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.naoEncontrado)
                {
                    coordenadas.latitude = retorno.latLng.lat.ToString().Replace(",", ".");
                    coordenadas.longitude = retorno.latLng.lng.ToString().Replace(",", ".");
                }
            }
            else
            {
                coordenadas.tipoLocalizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.naoEncontrado;
            }

            return coordenadas;
        }


        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao validarTipoLocalizacaoRetornoAPI(Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.location location)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao tipoLoc = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.naoEncontrado;
            if (location != null)
            {
                switch (location.geocodeQuality)
                {
                    case "POINT":
                        tipoLoc = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.ponto;
                        break;
                    case "ADDRESS":
                        tipoLoc = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.endereco;
                        break;
                    case "STREET":
                        tipoLoc = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.rua;
                        break;
                    case "INTERSECTION":
                        tipoLoc = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.esquina;
                        break;
                    case "CITY":
                        tipoLoc = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.cidade;
                        break;
                    case "ZIP":
                        tipoLoc = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.cidade;
                        break;
                    case "NEIGHBORHOOD":
                        tipoLoc = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.vizinhanca;
                        break;
                    default:
                        break;
                }
            }
            return tipoLoc;
        }


    }
}
