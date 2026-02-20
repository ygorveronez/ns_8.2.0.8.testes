using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet
{
    public class DadosControleSaldoPallet
    {
        #region Propriedades

        public int Codigo { get; set; }

        public int NumeroNota { get; set; }

        public string NumeroCarga { get; set; }

        public int QuantidadePallets { get; set; }

        public DateTime? DataRecebimento { get; set; }

        public RegraPallet RegraPallet { get; set; }

        public SituacaoGestaoPallet SituacaoPallet { get; set; }

        public string Origem { get; set; }

        public string Destino { get; set; }

        public string Filial { get; set; }
        public ResponsavelPallet Responsavel { get; set; }
        public bool QuebraRegra { get; set; }
        public TipoLancamento TipoLancamento { get; set; }
        public TipoEntradaSaida TipoMovimentacao { get; set; }

        public TipoGestaoDevolucao TipoDevolucao { get; set; }
        public string SituacaoDevolucao { get; set; }

        public int DiasLimiteParaDevolucao { get; set; }
        public DateTime? DataRecebimentoNota { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public string RegraPalletFormatada
        {
            get { return RegraPallet.ObterDescricao(); }
        }

        public string SituacaoPalletFormatada
        {
            get { return SituacaoPallet.ObterDescricao(); }
        }

        public string DataRecebimentoFormatada
        {
            get { return DataRecebimento.HasValue ? DataRecebimento.ToDateTimeString() : string.Empty; }
        }

        public string QuebraRegraFormatada
        {
            get { return QuebraRegra.ObterDescricao(); }
        }

        public string ResponsavelFormatada
        {
            get { return Responsavel.ObterDescricao(); }
        }

        public string DescricaoTipoLancamento
        {
            get { return TipoLancamento.ObterDescricao(); }
        }

        public string DescricaoTipoMovimentacao
        {
            get { return TipoMovimentacao.ObterDescricao(); }
        }

        public string DescricaoTipoGestaoDevolucao
        {
            get { return TipoDevolucao.ObterDescricao(); }
        }

        public string LeadTimeDevolucaoFormatado
        {
            get { return DataRecebimentoNota?.AddDays(DiasLimiteParaDevolucao).ToDateTimeString() ?? string.Empty; }
        }

        #endregion Propriedades com Regras
    }
}