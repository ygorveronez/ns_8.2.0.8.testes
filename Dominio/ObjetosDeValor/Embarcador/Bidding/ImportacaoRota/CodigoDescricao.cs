using System.Text.Json.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding.ImportacaoRota
{
    public class CodigoDescricao
    {
        public string Codigo { get; }

        public string Descricao { get; }

        [JsonConstructor]
        public CodigoDescricao(string codigo, string descricao)
        {
            Codigo = codigo;
            Descricao = descricao;
        }
    }
}
