using System.Text.Json.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding.ImportacaoRota
{
    public class Tomador
    {
        public double Codigo { get; }

        public string Descricao { get; }

        [JsonConstructor]
        public Tomador(double codigo, string descricao)
        {
            Codigo = codigo;
            Descricao = descricao;
        }
    }
}
