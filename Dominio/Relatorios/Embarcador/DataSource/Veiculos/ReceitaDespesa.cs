namespace Dominio.Relatorios.Embarcador.DataSource.Veiculos
{
    public class ReceitaDespesa
    {
        public int Codigo { get; set; }
        public int CodigoVeiculo { get; set; }
        public string PlacaVeiculo { get; set; }
        public string SegmentoVeiculo { get; set; }
        public string ModeloVeicular { get; set; }
        
        public decimal ReceitaTitulo { get; set; }
        public decimal DespesaTitulo { get; set; }
        public decimal ResultadoTitulo
        {
            get
            {
                return ReceitaTitulo - DespesaTitulo;
            }
        }

        public decimal ReceitaAbastecimento { get; set; }
        public decimal DespesaAbastecimento { get; set; }
        public decimal ResultadoAbastecimento
        {
            get
            {
                return ReceitaAbastecimento - DespesaAbastecimento;
            }
        }

        public decimal ReceitaPneu { get; set; }
        public decimal DespesaPneu { get; set; }
        public decimal ResultadoPneu
        {
            get
            {
                return ReceitaPneu - DespesaPneu;
            }
        }

        public decimal ReceitaDocumentoEntrada { get; set; }
        public decimal DespesaDocumentoEntrada { get; set; }
        public decimal ResultadoDocumentoEntrada
        {
            get
            {
                return ReceitaDocumentoEntrada - DespesaDocumentoEntrada;
            }
        }

        public decimal ReceitaCTe { get; set; }
        public decimal DespesaCTe { get; set; }
        public decimal ResultadoCTe
        {
            get
            {
                return ReceitaCTe - DespesaCTe;
            }
        }

        public decimal ReceitaPedagio { get; set; }
        public decimal DespesaPedagio { get; set; }
        public decimal ResultadoPedagio
        {
            get
            {
                return ReceitaPedagio - DespesaPedagio;
            }
        }

        public decimal ReceitaOrdemServico { get; set; }
        public decimal DespesaOrdemServico { get; set; }
        public decimal ResultadoOrdemServico
        {
            get
            {
                return ReceitaOrdemServico - DespesaOrdemServico;
            }
        }

        public decimal ReceitaAcertoViagem { get; set; }
        public decimal DespesaAcertoViagem { get; set; }
        public decimal ResultadoAcertoViagem
        {
            get
            {
                return ReceitaAcertoViagem - DespesaAcertoViagem;
            }
        }

        public decimal ReceitaOutros { get; set; }
        public decimal DespesaOutros { get; set; }
        public decimal ResultadoOutros
        {
            get
            {
                return ReceitaOutros - DespesaOutros;
            }
        }

        public decimal ResultadoGeral
        {
            get
            {
                return ResultadoCTe + ResultadoAbastecimento + ResultadoPedagio + ResultadoAcertoViagem + ResultadoDocumentoEntrada + ResultadoOrdemServico + ResultadoOutros + ResultadoPneu + ResultadoTitulo;
            }
        }
    }
}
