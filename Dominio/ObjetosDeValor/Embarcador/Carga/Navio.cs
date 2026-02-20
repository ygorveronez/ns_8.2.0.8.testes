using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class Navio
    {
        public int Codigo { get; set; }
        public string CodigoDocumento { get; set; }
        public string CodigoIntegracao { get; set; }
        public string Descricao { get; set; }
        public string CodigoIRIN { get; set; }
        public string CodigoEmbarcacao { get; set; }
        public string CodigoIMO { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ObjetosDeValor.Embarcador.Enumeradores.TipoEmbarcacao TipoEmbarcacao { get; set; }
        public bool InativarCadastro { get; set; }
        public bool Atualizar { get; set; }
        public bool NavioIntegracaoBooking { get; set; }
        public string CodigoNavio { get; set; }
    }
}
