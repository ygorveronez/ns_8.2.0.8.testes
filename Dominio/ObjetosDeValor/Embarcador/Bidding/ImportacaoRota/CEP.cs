using System.Text.Json.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding.ImportacaoRota
{
    public class CEP
    {
        public string Codigo { get; set; }

        public string Descricao { get; set; }

        public string CEPInicial { get; set; }

        public string CEPFinal { get; set; }

        [JsonConstructor]
        public CEP(string codigo, string descricao, string cepInicial, string cepFinal)
        {
            Codigo = codigo;
            Descricao = descricao;
            CEPInicial = cepInicial;
            CEPFinal = cepFinal;
        }
    }
}
