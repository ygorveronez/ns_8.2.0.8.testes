using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet
{
    public sealed class ConsultaMovimentacaoPallet
    {
        #region Propriedades

        public int Codigo { get; set; }

        public int NumeroNotaFiscal { get; set; }

        public string Carga { get; set; }

        public int QuantidadePallets { get; set; }

        public DateTime DataRecebimentoNotaFiscal { get; set; }

        public DateTime DataRecebimento { get; set; }

        public RegraPallet RegraPallet { get; set; }

        public SituacaoGestaoPallet Situacao { get; set; }

        public ResponsavelPallet ResponsavelMovimentacaoPallet { get; set; }

        public string CidadeOrigem { get; set; }
        public string UFOrigem { get; set; }

        public string CidadeDestino { get; set; }
        public string UFDestino { get; set; }

        public string Filial { get; set; }
        public string CNPJFilial { get; set; }

        public string Transportador { get; set; }
        public string CNPJTransportador { get; set; }

        public string NomeCliente { get; set; }
        public double CNPJCliente { get; set; }

        public string Observacao { get; set; }

        public int CodigoCargaPedido { get; set; }

        public TipoLancamento TipoLancamento { get; set; }
        public bool QuebraRegra { get; set; }
        public TipoEntradaSaida TipoMovimentacao { get; set; }

        public TipoGestaoDevolucao TipoDevolucao { get; set; }
        public string SituacaoDevolucao { get; set; }

        public int DiasLimiteParaDevolucao { get; set; }
        public string SerieNfe { get; set; }
        public string TiposTomador { get; set; }
        public string TiposModal { get; set; }
        public string CfopNfe { get; set; }
        public DateTime? DataEmissaoNfe { get; set; }
        public DateTime? DataVencimenoValePallet { get; set; }
        public string NumerosPedidoCliente { get; set; }
        public string CanaisVenda { get; set; }
        public string PlacaTracao { get; set; }
        public string PlacasReboque { get; set; }
        public SituacaoDigitalizacaoCanhoto SituacaoCanhoto { get; set; }
        public string SituacaoValePallet { get; set; }
        public DateTime? DataRecebimentoCanhoto { get; set; }
        public DateTime? DataDigitalizacaoCanhoto { get; set; }
        public DateTime? DataRecebimentoValePallet { get; set; }
        public int? NumeroAtendimento { get; set; }
        public SituacaoChamado SituacaoAtendimento { get; set; }
        public string EscritorioVendas { get; set; }
        public DateTime? DataLaudo { get; set; }
        public long? NumeroLaudo { get; set; }
        public int NumeroNotaFiscalPermuta { get; set; }
        public string SerieNotaFiscalPermuta { get; set; }
        public int NumeroNotaFiscalDevolucao { get; set; }
        public string SerieNotaFiscalDevolucao { get; set; }
        public OrigemGestaoDevolucao OrigemNotaFiscalDevolucao { get; set; }
        public string ResponsavelDevolucao { get; set; }
        public DateTime? DataColeta{ get; set; }
        public DateTime? DataPrevisaoChegada { get; set; }
        public string EnderecoColeta { get; set; }
        public string CidadeColeta { get; set; }
        public string CentroDescarregamento { get; set; }
        public DateTime? DataRecebimentoNFD { get; set; }
        public long CodigoDevolucao { get; set; }
        public string PeriodoDescarregamentoHoraInicio { get; set; }
        public string PeriodoDescarregamentoHoraTermino { get; set; }
        public DateTime? DataDescarregamento { get; set; }
     
        #endregion

        #region Propriedades com Regras

        public string RegraPalletDescricao
        {
            get { return RegraPallet.ObterDescricao(); }
        }

        public string SituacaoDescricao
        {
            get { return Situacao.ObterDescricao(); }
        }

        public string ResponsavelPendencia => $"{NomeCliente} ({CNPJCliente})";

        public string DescricaoQuebraRegra
        {
            get { return QuebraRegra.ObterDescricao(); }
        }

        public string DescricaoTipoLancamento
        {
            get { return TipoLancamento.ObterDescricao(); }
        }

        public string DescricaoTipoMovimentacao
        {
            get { return TipoMovimentacao.ObterDescricao(); }
        }

        public string DescricaoFilial
        {
            get
            {
                string descricao = string.Empty;

                if (!string.IsNullOrWhiteSpace(Filial))
                    descricao += Filial;

                if (!string.IsNullOrWhiteSpace(CNPJFilial))
                    descricao += " (" + CNPJFilial.ObterCpfOuCnpjFormatado() + ")";

                return descricao;
            }
        }

        public string DescricaoTransportador
        {
            get
            {
                string descricao = string.Empty;

                if (!string.IsNullOrWhiteSpace(Transportador))
                    descricao += Transportador;

                if (!string.IsNullOrWhiteSpace(CNPJTransportador))
                    descricao += " (" + CNPJTransportador.ObterCpfOuCnpjFormatado() + ")";

                return descricao;
            }
        }

        public string DescricaoCliente
        {
            get
            {
                string descricao = string.Empty;

                if (!string.IsNullOrWhiteSpace(NomeCliente))
                    descricao += NomeCliente;

                if (CNPJCliente != 0)
                    descricao += " (" + CNPJCliente.ToString().ObterCpfOuCnpjFormatado() + ")";

                return descricao;
            }
        }

        public string DescricaoTipoGestaoDevolucao
        {
            get { return TipoDevolucao.ObterDescricao(); }
        }

        public string LeadTimeDevolucaoFormatado
        {
            get { return DataRecebimentoNotaFiscal.AddDays(DiasLimiteParaDevolucao).ToDateTimeString() ?? string.Empty; }
        }

        public string Origem
        {
            get
            {
                string descricao = string.Empty;

                if (!string.IsNullOrWhiteSpace(CidadeOrigem))
                    descricao += CidadeOrigem;

                if (!string.IsNullOrWhiteSpace(UFOrigem))
                    descricao += " - " + UFOrigem;

                return descricao;
            }
        }

        public string Destino
        {
            get
            {
                string descricao = string.Empty;

                if (!string.IsNullOrWhiteSpace(CidadeDestino))
                    descricao += CidadeDestino;

                if (!string.IsNullOrWhiteSpace(UFDestino))
                    descricao += " - " + UFDestino;

                return descricao;
            }
        }

        public string TiposTomadorDescricao
        {
            get
            {
                if (TiposTomador is null)
                {
                    return null;
                }
                string[] tiposArrayString = TiposTomador.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                IEnumerable<string> tiposDescricao = tiposArrayString.Select(tipoString => ((TipoTomador)int.Parse(tipoString)).ObterDescricao());
                return string.Join(", ", tiposDescricao);
            }
        }

        public string TiposModalDescricao
        {
            get 
            {
                if (TiposModal is null)
                {
                    return null;
                }
                string[] tiposArrayString = TiposModal.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                IEnumerable<string> tiposDescricao = tiposArrayString.Select(tipoString => ((TipoCobrancaMultimodal)int.Parse(tipoString)).ObterDescricao());
                return string.Join(", ", tiposDescricao);
            }
        }

        public int? DiasVencer
        {
            get
            {
                DateTime? dataVencimento = null;
                if (ResponsavelMovimentacaoPallet == ResponsavelPallet.Cliente)
                {
                    dataVencimento = DataVencimenoValePallet;
                }
                if (ResponsavelMovimentacaoPallet == ResponsavelPallet.Transportador && DataEmissaoNfe.HasValue)
                {
                    dataVencimento = DataEmissaoNfe.Value.AddDays(30);
                }
                if (!dataVencimento.HasValue)
                {
                    return null;
                }
                return dataVencimento.Value.Date.Subtract(DateTime.Today.Date).Days;
            }
        }

        public string DataAgendamentoDevolucao
        {
            get
            {
                if (DataDescarregamento.HasValue && !string.IsNullOrWhiteSpace(PeriodoDescarregamentoHoraInicio) && !string.IsNullOrWhiteSpace(PeriodoDescarregamentoHoraTermino))
                {
                    return $"{DataDescarregamento:dd/MM} - {PeriodoDescarregamentoHoraInicio} até {PeriodoDescarregamentoHoraTermino}";
                }

                return "-";
            }
        }

        public string SituacaoCanhotoDescricao => SituacaoCanhoto.ObterDescricao();
        public string SituacaoAtendimentoDescricao => SituacaoAtendimento.ObterDescricao();
        public string OrigemNotaFiscalDevolucaoDescricao => OrigemNotaFiscalDevolucao.ObterDescricao();

        #endregion Propriedades com Regras

    }
}
