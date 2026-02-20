using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Linq;

namespace Dominio.Relatorios.Embarcador.DataSource.CTe.CTes
{
    public sealed class CTeSubcontratado
    {
        #region Propriedades

        public int Codigo { get; set; }
        public int NumeroCTe { get; set; }
        public int SerieCTe { get; set; }
        public string NumeroCarga { get; set; }
        public Dominio.Enumeradores.TipoDocumento Modelo { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal AliquotaISS { get; set; }
        public decimal ValorISS { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorReceber { get; set; }
        public decimal ValorTotalSemImposto { get; set; }
        public string Veiculo { get; set; }
        public int CFOP { get; set; }
        public int RPS { get; set; }
        public string StatusCTe { get; set; }
        public string UsuarioSolicitante { get; set; }
        public string ContaContabil { get; set; }
        public string NumeroCargaAgrupamento { get; set; }
        public string PreCarga { get; set; }
        public string NumeroPedido { get; set; }
        public string NumeroPedidoInterno { get; set; }
        public long NumeroMinuta { get; set; }
        public string NumeroNotaFiscal { get; set; }
        public string ChaveNotaFiscal { get; set; }
        public string CpfMotorista { get; set; }
        public string DataCriacaoCarga { get; set; }
        public StatusTitulo StatusTitulo { get; set; }
        public string NumeroFatura { get; set; }
        public string NumeroPreFatura { get; set; }
        public string DataNFEmissao { get; set; }
        public string NumeroDocumentoAnterior { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataAutorizacao { get; set; }
        public DateTime DataPagamento { get; set; }
        public string DataImportacao { get; set; }
        public string DataVinculoCarga { get; set; }
        public string DataCancelamento { get; set; }
        public string DataAnulacao { get; set; }
        public string DataEntrega { get; set; }
        public string DataFatura { get; set; }
        public string DataVencimento { get; set; }

        public string CodigoRemetente { get; set; }
        public string CPFCNPJRemetente { get; set; }
        public string IERemetente { get; set; }
        public string Remetente { get; set; }
        public string LocalidadeRemetente { get; set; }
        public string UFRemetente { get; set; }
        public string CodigoEnderecoRemetente { get; set; }
        public string EnderecoRemetente { get; set; }
        public string GrupoRemetente { get; set; }
        public string CategoriaRemetente { get; set; }
        public string CodigoDocumentoRemetente { get; set; }

        public string CodigoExpedidor { get; set; }
        public string CPFCNPJExpedidor { get; set; }
        public string IEExpedidor { get; set; }
        public string Expedidor { get; set; }
        public string LocalidadeExpedidor { get; set; }
        public string UFExpedidor { get; set; }
        public string CodigoDocumentoExpedidor { get; set; }

        public string CodigoRecebedor { get; set; }
        public string CPFCNPJRecebedor { get; set; }
        public string IERecebedor { get; set; }
        public string Recebedor { get; set; }
        public string LocalidadeRecebedor { get; set; }
        public string UFRecebedor { get; set; }
        public string CodigoDocumentoRecebedor { get; set; }

        public string CodigoDestinatario { get; set; }
        public string CPFCNPJDestinatario { get; set; }
        public string IEDestinatario { get; set; }
        public string Destinatario { get; set; }
        public string LocalidadeDestinatario { get; set; }
        public string UFDestinatario { get; set; }
        public string GrupoDestinatario { get; set; }
        public string CategoriaDestinatario { get; set; }
        public string CodigoDocumentoDestinatario { get; set; }

        public string CPFCNPJTomador { get; set; }
        public string IETomador { get; set; }
        public string Tomador { get; set; }
        public string UFTomador { get; set; }
        public string CodigoDocumentoTomador { get; set; }

        public string TipoDeCarga { get; set; }
        public int IBGEInicioPrestacao { get; set; }
        public string InicioPrestacao { get; set; }
        public string UFInicioPrestacao { get; set; }
        public int IBGEFimPrestacao { get; set; }
        public string FimPrestacao { get; set; }
        public string UFFimPrestacao { get; set; }
        private string CNPJTransportador { get; set; }
        public string RazaoSocialTransportador { get; set; }
        public string NomeFantasiaTransportador { get; set; }
        public decimal AliquotaICMS { get; set; }
        public decimal BaseCalculoICMS { get; set; }
        public decimal ValorISSRetido { get; set; }
        public decimal ValorSemImposto { get; set; }
        public decimal ValorMercadoria { get; set; }
        public string CSTIBSCBS { get; set; }
        public string ClassificacaoTributariaIBSCBS { get; set; }
        public decimal BaseCalculoIBSCBS { get; set; }
        public decimal AliquotaIBSEstadual { get; set; }
        public decimal PercentualReducaoIBSEstadual { get; set; }
        public decimal ValorReducaoIBSEstadual { get; set; }
        public decimal ValorIBSEstadual { get; set; }
        public decimal AliquotaIBSMunicipal { get; set; }
        public decimal PercentualReducaoIBSMunicipal { get; set; }
        public decimal ValorReducaoIBSMunicipal { get; set; }
        public decimal ValorIBSMunicipal { get; set; }
        public decimal AliquotaCBS { get; set; }
        public decimal PercentualReducaoCBS { get; set; }
        public decimal ValorReducaoCBS { get; set; }
        public decimal ValorCBS { get; set; }
        public string Observacao { get; set; }
        public string GrupoTomador { get; set; }
        public string Rotas { get; set; }
        public string Ocorrencia { get; set; }
        public string TipoOcorrencia { get; set; }
        public string ContratoFrete { get; set; }
        public string TipoOperacao { get; set; }
        public string NumeroOCADocumentoOriginario { get; set; }
        public string ChaveCTe { get; set; }
        public string ProtocoloAutorizacao { get; set; }
        public string ProtocoloInutilizacaoCancelamento { get; set; }
        public string Motorista { get; set; }
        public string ProcImportacao { get; set; }
        public decimal PesoKg { get; set; }
        public decimal PesoLiquidoKg { get; set; }
        public int Volumes { get; set; }
        public decimal MetrosCubicos { get; set; }
        public decimal Pallets { get; set; }
        public string Pago { get; set; }
        public string RetornoSefaz { get; set; }
        public string NomeProprietarioVeiculo { get; set; }
        public string Frota { get; set; }
        public string DataOcorrenciaFinal { get; set; }
        public string VeiculoUltimoMDFe { get; set; }
        public string NumeroUltimoMDFe { get; set; }
        public string DescricaoUltimaOcorrencia { get; set; }
        public string CodigoEnderecoDestinatario { get; set; }
        public string EnderecoDestinatario { get; set; }
        public string BairroDestinatario { get; set; }
        public string CEPDestinatario { get; set; }
        public decimal ValorPrestacao { get; set; }
        public string Operador { get; set; }
        public string DataColeta { get; set; }
        public string ModeloVeicular { get; set; }
        public string Log { get; set; }
        public string DataPrevistaEntrega { get; set; }
        public TipoCTE TipoCTe { get; set; }
        public Dominio.Enumeradores.TipoServico TipoServico { get; set; }
        public string TipoServicoMultimodal { get; set; }
        public bool Afretamento { get; set; }
        public string NumeroProtocoloANTAQ { get; set; }
        public string NumeroManifesto { get; set; }
        public string NumeroManifestoFeeder { get; set; }
        public string NumeroCEMercante { get; set; }
        public string NumeroCEANTAQ { get; set; }
        public TipoTomador TipoTomador { get; set; }
        public TipoPagamento TipoPagamento { get; set; }
        public string NumeroDTNatura { get; set; }
        public decimal KmRodado { get; set; }
        public decimal ValorKMContrato { get; set; }
        public decimal ValorKMExcedenteContrato { get; set; }
        public decimal ValorFreteFranquiaKM { get; set; }
        public decimal ValorFreteFranquiaKMExcedido { get; set; }
        public decimal KmConsumido { get; set; }
        public decimal KmConsumidoExcedente { get; set; }
        public decimal AliquotaCOFINS { get; set; }
        public decimal AliquotaPIS { get; set; }
        public string CNPJFilial { get; set; }
        public string Filial { get; set; }
        public string NumeroDI { get; set; }
        public string NumeroDTA { get; set; }
        public string NumeroPedidoNotaFiscal { get; set; }
        public string CodigoReferencia { get; set; }
        public string CodigoImportacao { get; set; }
        public string SegmentoVeiculo { get; set; }
        public string NumeroValePedagio { get; set; }
        public decimal ValorValePedagio { get; set; }
        public string TabelaFrete { get; set; }
        public string TabelaFreteCliente { get; set; }
        public string SituacaoCarga { get; set; }
        public string NumeroFechamentoFrete { get; set; }
        public string NumeroEscrituracao { get; set; }
        public string NumeroPagamento { get; set; }
        public string NumeroContabilizacao { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroOS { get; set; }
        public string NumeroControle { get; set; }
        public string TipoProposta { get; set; }
        public string NumeroProposta { get; set; }
        public string TipoCarga { get; set; }
        public int QuantidadeNF { get; set; }
        public string Viagem { get; set; }
        public string NumeroLacre { get; set; }
        public string Tara { get; set; }
        public string Container { get; set; }
        public string TipoContainer { get; set; }
        public string NumeroBoleto { get; set; }
        public string DataBoleto { get; set; }
        public string PortoOrigem { get; set; }
        public string PortoDestino { get; set; }
        public string PortoTransbordo { get; set; }
        public string NavioTransbordo { get; set; }
        public string PossuiCartaCorrecao { get; set; }
        public string FoiAnulado { get; set; }
        public string PossuiCTeComplementar { get; set; }
        public string FoiSubstituido { get; set; }
        public string ETS { get; set; }
        public string ETA { get; set; }
        public string MotivoCancelamento { get; set; }
        public string NumeroLoteCancelamento { get; set; }
        public decimal AliquotaICMSInterna { get; set; }
        public decimal PercentualICMSPartilha { get; set; }
        public decimal ValorICMSUFOrigem { get; set; }
        public decimal ValorICMSUFDestino { get; set; }
        public decimal ValorICMSFCPFim { get; set; }
        public string CaracteristicaTransporteCTe { get; set; }
        public string ProdutoPredominante { get; set; }
        public string CentroResultado { get; set; }
        public string RotaFrete { get; set; }
        public decimal ValorSemTributo { get; set; }
        public string NumeroCTeSubstituto { get; set; }
        public string NumeroCTeAnulacao { get; set; }
        public string NumeroCTeComplementar { get; set; }
        public string NumeroCTeDuplicado { get; set; }
        public string NumeroCTeOriginal { get; set; }
        public string NumeroControleCTeSubstituto { get; set; }
        public string NumeroControleCTeAnulacao { get; set; }
        public string NumeroControleCTeComplementar { get; set; }
        public string NumeroControleCTeDuplicado { get; set; }
        public string NumeroControleCTeOriginal { get; set; }
        public string NumeroCIOT { get; set; }
        public string NumeroDocumentoOriginario { get; set; }
        public string DataInicioViagem { get; set; }
        public int QuantidadeContainer { get; set; }
        public decimal Taxa { get; set; }
        public string NumeroCTeTerceiroOcorrencia { get; set; }
        public string NumeroDocumentoRecebedor { get; set; }
        public string NumeroPedidoCliente { get; set; }
        public decimal QuantidadeTotalProduto { get; set; }
        public string ModeloVeiculo { get; set; }
        public string TipoCarroceria { get; set; }
        public decimal DistanciaCargaAgrupada { get; set; }
        public string OperadorResponsavelCancelamento { get; set; }
        public string VeiculoTracao { get; set; }
        public string VeiculoReboque { get; set; }
        public decimal KMRota { get; set; }
        public string DataConfirmacaoDocumento { get; set; }
        public string RuaRecebedor { get; set; }
        public string NumeroRecebedor { get; set; }
        public string BairroRecebedor { get; set; }
        public string LacresCargaLacre { get; set; }
        public decimal PalletsPedido { get; set; }
        public string RegraICMS { get; set; }
        private string CST { get; set; }
        public int CTeSubcontratacao { get; set; }
        public int SerieCTeSubcontracao { get; set; }
        public string ChaveCTeSubcontratacao { get; set; }
        public string ObservacaoCTeSubcontratacao { get; set; }
        private string CNPJTransportadorSubcontratacao { get; set; }
        public string RazaoSocialTransportadorSubcontratacao { get; set; }
        public string NomeFantasiaTransportadorSubcontratacao { get; set; }
        public decimal ValorICMSSubcontratacao { get; set; }
        public decimal ValorFreteSubcontratacao { get; set; }
        public decimal ValorReceberSubcontratacao { get; set; }
        public decimal ValorPrestacaoSubcontratacao { get; set; }
        public decimal ValorTotalSemImpostoSubcontratacao { get; set; }
        private string StatusCTeSubcontratado { get; set; }
        public string CMDID { get; set; }
        public string CodigoNavio { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DocumentoDescricao
        {
            get { return Modelo.ObterDescricao(); }
        }

        public string CNPJTransportadorFormatada
        {
            get { return CNPJTransportador != null ? CNPJTransportador.ObterCpfOuCnpjFormatado() : string.Empty; }
        }

        public string CNPJTransportadorSubcontratacaoFormatada
        {
            get { return CNPJTransportadorSubcontratacao != null ? CNPJTransportadorSubcontratacao.ObterCpfOuCnpjFormatado() : string.Empty; }
        }

        public string DataPagamentoFormatada
        {
            get { return DataPagamento != DateTime.MinValue ? DataPagamento.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string DescricaoStatusTitulo
        {
            get { return StatusTitulo.ObterDescricao(); }
        }

        public string DescricaoAfretamento
        {
            get { return Afretamento ? "Sim" : "Não"; }
        }

        public string DescricaoTipoCTe
        {
            get { return TipoCTe.ObterDescricao(); }
        }

        public string DescricaoTipoPagamento
        {
            get { return TipoPagamento.ObterDescricao(); }
        }

        public string DescricaoTipoServico
        {
            get { return TipoServico.ObterDescricao(); }
        }

        public string DescricaoTipoTomador
        {
            get
            {
                switch (TipoTomador)
                {
                    case Dominio.Enumeradores.TipoTomador.Remetente:
                        return "Remetente";
                    case Dominio.Enumeradores.TipoTomador.Expedidor:
                        return "Expedidor";
                    case Dominio.Enumeradores.TipoTomador.Recebedor:
                        return "Recebedor";
                    case Dominio.Enumeradores.TipoTomador.Destinatario:
                        return "Destinatário";
                    case Dominio.Enumeradores.TipoTomador.Outros:
                        return "Outro";
                    default:
                        return "";
                }
            }
        }

        public decimal ValorCOFINS
        {
            get { return Math.Round(BaseCalculoICMS * (AliquotaCOFINS / 100), 2, MidpointRounding.AwayFromZero); }
        }

        public decimal ValorPIS
        {
            get { return Math.Round(BaseCalculoICMS * (AliquotaPIS / 100), 2, MidpointRounding.AwayFromZero); }
        }

        public int AnoEmissao
        {
            get { return DataEmissao.Year; }
        }

        public int MesEmissao
        {
            get { return DataEmissao.Month; }
        }

        public string SituacaoCargaFormatada
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SituacaoCarga))
                    return SituacaoCarga;

                string[] listaSituacoes = SituacaoCarga.Split(',');

                return string.Join(", ", (from situacao in listaSituacoes select SituacaoCargaHelper.ObterDescricao((SituacaoCarga)situacao.ToInt())));
            }
        }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataAutorizacaoFormatada
        {
            get { return DataAutorizacao != DateTime.MinValue ? DataAutorizacao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string CSTFormatada
        {
            get { return TipoICMSHelper.ObterTipoDescricao(CST); }
        }

        public string DescricaoStatusCTeSubcontratado
        {
            get
            {
                switch (StatusCTeSubcontratado)
                {
                    case "P":
                        return "Pendente";
                    case "E":
                        return "Enviado";
                    case "R":
                        return "Rejeição";
                    case "A":
                        return "Autorizado";
                    case "C":
                        return "Cancelado";
                    case "I":
                        return "Inutilizado";
                    case "D":
                        return "Denegado";
                    case "S":
                        return "Em Digitação";
                    case "K":
                        return "Em Cancelamento";
                    case "L":
                        return "Em Inutilização";
                    case "Z":
                        return "Anulado Gerencialmente";
                    case "X":
                        return "Aguardando Assinatura";
                    case "V":
                        return "Aguardando Assinatura Cancelamento";
                    case "B":
                        return "Aguardando Assinatura Inutilização";
                    case "M":
                        return "Aguardando Emissão e-mail";
                    case "F":
                        return "Contingência FSDA";
                    case "Q":
                        return "Contingência EPEC";
                    case "Y":
                        return "Aguardando Finalizar Carga Integração";
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
    }
}
