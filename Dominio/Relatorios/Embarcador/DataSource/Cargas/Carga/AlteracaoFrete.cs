using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga
{
    public class AlteracaoFrete
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string CodigosAgrupados { get; set; }
        public string Filial { get; set; }
        public string ModeloVeiculo { get; set; }
        public string NumeroCarga { get; set; }
        public string OperadorCarga { get; set; }
        public string Rota { get; set; }
        public string TipoCarga { get; set; }
        public string TipoOperacao { get; set; }
        public string Transportador { get; set; }
        public decimal ValorAcrescimo { get; set; }
        public decimal ValorFrete { get; set; }
        public string Veiculos { get; set; }
        public string UsuarioAprovacao { get; set; }
        public string MotivoSolicitacaoFrete { get; set; }
        public string StatusTituloCTe { get; set; }

        #endregion

        #region Propriedades Privadas

        private decimal AliquotaICMS { get; set; }
        private decimal AliquotaISS { get; set; }
        private string CNPJFilial { get; set; }
        private string CNPJTransportador { get; set; }
        private DateTime DataCarregamento { get; set; }
        private SituacaoAlteracaoFreteCarga SituacaoAlteracaoFrete { get; set; }
        private SituacaoCarga SituacaoCarga { get; set; }
        private TipoFreteEscolhido TipoFreteEscolhido { get; set; }
        private decimal ValorTabelaFrete { get; set; }
        private decimal ValorTotalComponentes { get; set; }
        private decimal ValorFreteNegociado { get; set; }
        private decimal ValorComplementoFrete { get; set; }

        #endregion

        #region Propriedades com Regras

        public string CNPJFilialFormatado
        {
            get { return CNPJFilial.ObterCnpjFormatado(); }
        }

        public string CNPJTransportadorFormatado
        {
            get { return CNPJTransportador.ObterCnpjFormatado(); }
        }

        public string DataCarregamentoFormatada
        {
            get { return DataCarregamento != DateTime.MinValue ? DataCarregamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string Negociacao
        {
            get
            {
                if (TipoFreteEscolhido == TipoFreteEscolhido.Operador)
                {
                    if (ValorFrete > ValorTabelaFrete)
                        return "Acima da Tabela";
                    else if (ValorFrete < ValorTabelaFrete)
                        return "Abaixo da Tabela";
                    else
                        return "Igual a Tabela";
                }
                else
                    return TipoFreteEscolhido.ObterDescricao();
            }
        }

        public decimal PercentualDiferenca
        {
            get
            {
                decimal valorTabelaLiquido = ValorTabela;
                if (TipoFreteEscolhido == TipoFreteEscolhido.Operador)
                    return valorTabelaLiquido > 0 ? (((ValorFrete - valorTabelaLiquido) * 100) / valorTabelaLiquido) : 0;
                else
                    return 0;
            }
        }

        public string SituacaoAlteracaoFreteFormatada
        {
            get { return SituacaoAlteracaoFrete.ObterDescricao(); }
        }

        public string SituacaoCargaFormatada
        {
            get { return SituacaoCarga.ObterDescricao(); }
        }

        public decimal ValorTabela
        {
            get { return ValorTabelaFrete - (ValorTabelaFrete * (AliquotaICMS / 100)) - (ValorTabelaFrete * (AliquotaISS / 100)) - ValorTotalComponentes; }
        }

        public decimal DiferencaValorPagoTabela
        {
            get { return ValorTabela - ValorFreteNegociado + ValorComplementoFrete; }
        }

        #endregion
    }
}
