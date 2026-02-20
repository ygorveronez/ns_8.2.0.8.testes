namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca
{
    public class Recolhimento
    {
        public int Pai { get; set; }

        public string Item { get; set; }

        public string Descricao { get; set; }

        public decimal Quantidade { get; set; }

        public decimal QuantidadePlanejada { get; set; }

        public decimal Valor { get; set; }
    }
}
