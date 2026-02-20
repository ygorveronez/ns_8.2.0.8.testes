using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class Container
    {
        public int Codigo { get; set; }
        public int CodigoIntegracao { get; set; }
        public string Descricao { get; set; }
        public string Numero { get; set; }
        public decimal PesoLiquido { get; set; }
        public int Tara { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeContainer TipoPropriedade { get; set; }
        public TipoContainer TipoContainer { get; set; }
        public bool InativarCadastro { get; set; }
        public bool Atualizar { get; set; }

        public string Lacre1 { get; set; }
        public string Lacre2 { get; set; }
        public string Lacre3 { get; set; }
        public decimal Volume { get; set; }
        public decimal DencidadeProduto { get; set; }
    }
}
