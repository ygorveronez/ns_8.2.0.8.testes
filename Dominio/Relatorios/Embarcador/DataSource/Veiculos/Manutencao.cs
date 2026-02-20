using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Veiculos
{
    public class Manutencao
    {
        private DateTime DataEmissao { get; set; }
        public string Fornecedor { get; set; }
        public string Segmento { get; set; }
        public string Placa { get; set; }
        public string TipoMovimento { get; set; }
        public string NaturezaOperacao { get; set; }
        public string Produto { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
        public string Equipamento { get; set; }
        public decimal ValorCustoUnitario { get; set; }
        public decimal ValorCustoTotal { get; set; }
        public string CentroResultado { get; set; }
        public string NumeroDocumento { get; set; }
        public string SerieDocumento { get; set; }
        public double CNPJFornecedor { get; set; }
        public int QuilometragemVeiculo { get; set; }
        public int Horimetro { get; set; }
        private SituacaoDocumentoEntrada SituacaoLancDocEntrada { get; set; }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DescricaoSituacaoLancDocEntrada
        {
            get 
            { return SituacaoLancDocEntrada.ObterDescricao(); }
        }
    }
}
