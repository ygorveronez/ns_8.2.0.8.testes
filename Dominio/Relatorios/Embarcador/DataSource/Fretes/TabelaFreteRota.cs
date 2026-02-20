namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public class TabelaFreteRota
    {
        public int Codigo { get; set; }

        public string Origem { get; set; }
        public string Destino { get; set; }
        public string Descricao { get; set; }
        public string CodigoEmbarcador { get; set; }

        public string TipoCarga { get; set; }

        public string Veiculo { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal Pedagio { get; set; }

        public decimal Imposto { get; set; }
        public int Aliquota { get; set; }
        public decimal FreteBruto { get; set; }
        public int Km { get; set; }
        public decimal ValorPorKmLiquido { get; set; }
        public decimal ValorPorKmBruto { get; set; }

    }
}
