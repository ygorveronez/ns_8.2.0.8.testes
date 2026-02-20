using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT
{
    [JsonObject(ItemNullValueHandling=NullValueHandling.Ignore)]
    public class Posicao
    {
        public long id { get; set; }
        public bool online { get; set; }
        public long eventoId { get; set; }
        public string evento { get; set; }
        public long sequencia { get; set; }
        public string referencia { get; set; }
        public long dataEquipamento { get; set; }
        public long dataGPS { get; set; }
        public long dataGateway { get; set; }
        public long dataProcessamento { get; set; }
        public string tipo { get; set; }
        public string text { get; set; }
        public string binario { get; set; }
        public bool validade { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int velocidade { get; set; }
        public int proa { get; set; }
        public double altitude { get; set; }
        public int hdop { get; set; }
        public int satelites { get; set; }
        public string livre { get; set; }
        public string endereco { get; set; }
        public string motorista { get; set; }
        public long dispositivoid { get; set; }
        public string numerostr { get; set; }
        public string fabricante { get; set; }
        public List<Componente> componentes { get; set; }
    }
}
