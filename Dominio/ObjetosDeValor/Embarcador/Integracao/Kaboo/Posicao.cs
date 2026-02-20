using System;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Kaboo
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Posicao
    {
        public string dataJs { get; set; }
        public string dia { get; set; }
        public string direcao { get; set; }
        public string gps { get; set; }
        public string hora { get; set; }
        public string ignicao { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string nome { get; set; }
        public string placa { get; set; }
        public int veiculo { get; set; }
        public int vel { get; set; }
        public virtual DateTime data {
            get
            {
                DateTime data = DateTime.Parse(dia + " " + hora);
                return data;
            }
        }
    }
}
