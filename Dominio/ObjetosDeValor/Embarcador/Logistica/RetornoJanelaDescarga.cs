using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class RetornoJanelaDescarga
    {
        #region Propriedades

        public bool AgendaExtra { get; set; }
        public string Categoria { get; set; }
        public string CentroDescarregamento { get; set; }
        public int Codigo { get; set; }
        public int CodigoAgendamentoColeta { get; set; }
        public int CodigoAgendamentoPallet { get; set; }
        public int CodigoAgendamentoColetaPedido { get; set; }
        public int CodigoCarga { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public int CodigoMonitoramento { get; set; }
        public int CodigoPedido { get; set; }
        public int CodigoSituacaoCadastrada { get; set; }
        public bool ExibirJanelaDescargaPorPedido { get; set; }
        public double CPFCNPJDestinatario { get; set; }
        public double CPFCNPJFornecedor { get; set; }
        public DateTime DataAgendamentoColeta { get; set; }
        public DateTime DataAgendamentoPallet { get; set; }
        public DateTime DataEmissaoCTe { get; set; }
        public DateTime DataEntradaRaio { get; set; }
        public DateTime DataLancamento { get; set; }
        public DateTime DataProximaEntregaReprogramada { get; set; }
        public DateTime DataValidadePedido { get; set; }
        public string DescricaoSituacaoCadastrada { get; set; }
        public string DestinatarioCodigoIntegracao { get; set; }
        public string DestinatarioNome { get; set; }
        public string DestinatarioNomeFantasia { get; set; }
        public bool DestinatarioPontoTransbordo { get; set; }
        public string DestinatarioTipo { get; set; }
        public string DT_FontColor { get; set; }
        public string DT_RowColor { get; set; }
        public string Filial { get; set; }
        public string FornecedorCodigoIntegracao { get; set; }
        public string FornecedorNome { get; set; }
        public string FornecedorNomeFantasia { get; set; }
        public bool FornecedorPontoTransbordo { get; set; }
        public string FornecedorTipo { get; set; }
        public DateTime InicioDescarregamento { get; set; }
        public decimal? KmAteDestino { get; set; }
        public string Modalidade { get; set; }
        public string ModeloVeicular { get; set; }
        public string Motorista { get; set; }
        public string NotasFiscaisEntrega { get; set; }
        public string NumeroPedido { get; set; }
        public string ObservacaoPedido { get; set; }
        public string OrigemCarga { get; set; }
        public bool PermiteInformarAcaoParcial { get; set; }
        public decimal Peso { get; set; }
        public decimal PesoLiquido { get; set; }
        public string PrevisaoEntrega { get; set; }
        public int? QtdItens { get; set; }
        public bool SemHorario { get; set; }
        public SituacaoCargaJanelaDescarregamento Situacao { get; set; }
        public SituacaoCarga SituacaoCarga { get; set; }
        public string StatusViagem { get; set; }
        public DateTime TerminoDescarregamento { get; set; }
        public TipoAcaoParcial TipoAcaoParcial { get; set; }
        public string TipoDeCarga { get; set; }
        public string TipoOperacao { get; set; }
        public string Transportador { get; set; }
        public string Veiculo { get; set; }
        public int? VolumeEmCx { get; set; }
        public int QuantidadeArquivoIntegracao { get; set; }
        public bool RastreadorOnline { get; set; }
        public string SenhaAgendamentoColeta { get; set; }
        public string SenhaAgendamentoPallet { get; set; }
        public string StatusBuscaSenhaAutomatica { get; set; }
        public string ObservacaoFluxoPatio { get; set; }
        private DateTime DataPrevisaoChegada { get; set; }
        private int CodigoSequenciaGestaoPatioDestino { get; set; }
        public string Remetente { get; set; }
        public string SetPointTransp { get; set; }
        public string TipoCargaTaura { get; set; }
        public string RangeTempTransp { get; set; }
        public int QuantidadeCaixas { get; set; }
        public decimal Valor { get; set; }
        public int? QtdProdutos { get; set; }
        public string UnidadeMedidaAgendamento { get; set; }
        public int QuantidadeNotas { get; set; }
        public int QuantidadeNotasAVIPED { get; set; }
        public string Recebedor { get; set; }
        public string Motivo { get; set; }
        public int QuantidadeNaoComparecimento { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public string DataAgendamento
        {
            get
            {
                if (DataAgendamentoColeta != DateTime.MinValue)
                    return DataAgendamentoColeta.ToDateString();

                if (DataAgendamentoPallet != DateTime.MinValue)
                    return DataAgendamentoPallet.ToDateString();

                return string.Empty;
            }
        }

        public string DataDescarregamento
        {
            get
            {
                return InicioDescarregamento.ToDateString();
            }
        }

        public string DataEmissaoCTeFormatado
        {
            get
            {
                return DataEmissaoCTe != DateTime.MinValue ? DataEmissaoCTe.ToDateTimeString() : "";
            }
        }

        public string DataEntradaRaioFormatada
        {
            get
            {
                return DataEntradaRaio != DateTime.MinValue ? DataEntradaRaio.ToDateString() : "";
            }
        }

        public string DataFimJanela
        {
            get
            {
                return DataValidadePedido != DateTime.MinValue ? DataValidadePedido.ToDateString() : "";
            }
        }

        public string DataLancamentoDescricao
        {
            get
            {
                return DataLancamento.ToDateTimeString();
            }
        }

        public string DataProximaEntregaReprogramadaFormatada
        {
            get
            {
                return DataProximaEntregaReprogramada != DateTime.MinValue ? DataProximaEntregaReprogramada.ToDateString() : "";
            }
        }

        public string DescricaoSituacao
        {
            get
            {
                return Situacao.ObterDescricao();
            }
        }

        public string Destinatario
        {
            get
            {
                string descricao = "";
                string nome = DestinatarioNome;

                if (DestinatarioPontoTransbordo)
                    nome = DestinatarioNomeFantasia;

                if (!string.IsNullOrWhiteSpace(DestinatarioCodigoIntegracao))
                    descricao += DestinatarioCodigoIntegracao + " - ";

                if (!string.IsNullOrWhiteSpace(nome))
                    descricao += nome;

                if (!string.IsNullOrWhiteSpace(DestinatarioTipo))
                    descricao += $" ({CPFCNPJDestinatario.ToString().ObterCpfOuCnpjFormatado(DestinatarioTipo)})";

                return descricao;
            }
        }

        public string DisponibilidadeVeiculo
        {
            get { return DataPrevisaoChegada != DateTime.MinValue ? DataPrevisaoChegada.ToDateTimeString() : string.Empty; }
        }

        public string AgendaExtraDescricao { get { return this.AgendaExtra ? "Sim" : "Não"; } }


        public string Excedente
        {
            get
            {
                return SemHorario ? "Sim" : "Não";
            }
        }

        public string Fornecedor
        {
            get
            {
                string descricao = "";
                string nome = FornecedorNome;

                if (FornecedorPontoTransbordo)
                    nome = FornecedorNomeFantasia;

                if (!string.IsNullOrWhiteSpace(FornecedorCodigoIntegracao))
                    descricao += FornecedorCodigoIntegracao + " - ";

                if (!string.IsNullOrWhiteSpace(nome))
                    descricao += nome;

                if (!string.IsNullOrWhiteSpace(FornecedorTipo))
                    descricao += $" ({CPFCNPJFornecedor.ToString().ObterCpfOuCnpjFormatado(FornecedorTipo)})";

                return descricao;
            }
        }

        public string HoraAgendamento
        {
            get
            {
                if (DataAgendamentoColeta != DateTime.MinValue)
                    return DataAgendamentoColeta.ToTimeString(showSeconds: true);

                if (DataAgendamentoPallet != DateTime.MinValue)
                    return DataAgendamentoPallet.ToTimeString(showSeconds: true);

                return string.Empty;
            }
        }

        public string HoraDescarregamento
        {
            get
            {
                return SemHorario && !ExibirHoraAgendadaParaCargasExcedentesJanelaDescarga ? "" : InicioDescarregamento.ToTimeString();
            }
        }

        public string KmAteDestinoFormatado
        {
            get
            {
                return KmAteDestino.HasValue ? string.Format("{0:n1} Km", KmAteDestino) : "";
            }
        }

        public bool PermitirInformarObservacaoFluxoPatio
        {
            get { return CodigoSequenciaGestaoPatioDestino > 0; }
        }

        public string PesoFormatado
        {
            get
            {
                return Peso.ToString("n3");
            }
        }

        public string PesoLiquidoFormatado
        {
            get
            {
                return PesoLiquido.ToString("n3");
            }
        }

        public bool ExibirHoraAgendadaParaCargasExcedentesJanelaDescarga { get; set; }

        public string SenhaAgendamento
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SenhaAgendamentoColeta))
                    return SenhaAgendamentoColeta;

                if (!string.IsNullOrWhiteSpace(SenhaAgendamentoPallet))
                    return SenhaAgendamentoPallet;

                return string.Empty;
            }
        }

        #endregion Propriedades com Regras
    }
}
