namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete
{
    public class CargaComposicaoFreteItem
    {
        public string Agrupamento { get; set; }

        public string Descricao { get; set; }

        public string DescricaoAgrupamento { get; set; }

        public string Placa { get; set; }
        public string Transportador { get; set; }
        public string Motorista { get; set; }
        public string Formula { get; set; }

        public string TipoCampoValor { get; set; }

        public string TipoParametro { get; set; }

        public decimal Valor { get; set; }

        public decimal ValorCalculado { get; set; }

        public string ValoresFormula { get; set; }
        public string Cliente { get; set; }

        public string Origem { get; set; }
        public string Destino { get; set; }
        public string CodigoTabela { get; set; }
    }
}
