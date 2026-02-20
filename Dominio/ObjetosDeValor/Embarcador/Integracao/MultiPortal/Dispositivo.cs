using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.MultiPortal
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Dispositivo
    {
        public long id { get; set; }
        public int fabricanteId { get; set; }
        public string fabricante { get; set; }
        public long numero { get; set; }
        public string numeroStr { get; set; }
        public string skywave { get; set; }
        public double tensaoBateria { get; set; }
        public string dispositivoPrincipal { get; set; }
        public string movel { get; set; }
        public string observacao { get; set; }
        public long dataCadastro { get; set; }
        public string usuarioCadastro { get; set; }
        public long dataAtualizacaoIccid { get; set; }
        public string inicioVinculo { get; set; }
        public string fimVinculo { get; set; }
        public string serialHexa { get; set; }
        public List<Chip> chips { get; set; }
        public List<Posicao> posicoes { get; set; }
    }
}
