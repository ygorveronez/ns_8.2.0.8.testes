using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Pedido
{
    public class RelatorioPedido
    {
        #region Propriedades

        public string CanalEntrega { get; set; }
        public decimal CapacidadePesoModeloVeicular { get; set; }
        public string CategoriaDestinatario { get; set; }
        public string CategoriaRemetente { get; set; }
        public double CNPJDestinatario { get; set; }
        public double CNPJExpedidor { get; set; }
        public double CNPJRemetente { get; set; }
        public double CNPJRecebedor { get; set; }
        public int Codigo { get; set; }
        public string CodigoPedidoCliente { get; set; }
        public string Companhia { get; set; }
        public string CTes { get; set; }
        public decimal CubagemPedido { get; set; }
        public string DataCadastro { get; set; }
        public string DataCriacaoCarga { get; set; }
        public DateTime DataCancel { get; set; }
        public DateTime DataCarregamento { get; set; }
        public DateTime DataChip { get; set; }
        public DateTime DataColeta { get; set; }
        public DateTime DataEmbarque { get; set; }
        public string DataETA { get; set; }
        public string DataFimEmissaoDocumentos { get; set; }
        public string DataInclusaoBooking { get; set; }
        public string DataInclusaoPCP { get; set; }
        public DateTime DataInicioEmissaoDocumentos { get; set; }
        public string DataRetiradaCtrn { get; set; }
        public string DataTerminoCarregamento { get; set; }
        public string EnderecoPrincipalDestinatario { get; set; }
        public string BairroDestinatario { get; set; }
        public string ComplementoDestinatario { get; set; }
        public string NumeroDestinatario { get; set; }
        public string PrevisaoEntregaTransportador { get; set; }
        public decimal ValorCustoFrete { get; set; }
        public string CustoFrete { get; set; }
        public string DataAgendamento { get; set; }
        public string NovaDataAgendamento { get; set; }
        public string DeliveryTerm { get; set; }
        public string Destinatario { get; set; }
        public string Destino { get; set; }
        public string Expedidor { get; set; }
        public string ExpedidorNomeFantasia { get; set; }
        public string Fatura { get; set; }
        public string Filial { get; set; }
        public decimal FreteLiquido { get; set; }
        public string GrupoDestinatario { get; set; }
        public string GrupoPessoas { get; set; }
        public string GrupoRemetente { get; set; }
        public decimal ICMS { get; set; }
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
        public string IdAutorizacao { get; set; }
        public decimal ISS { get; set; }
        public string ModeloVeicular { get; set; }
        public string Motoristas { get; set; }
        public string NotasFiscais { get; set; }
        public int VolumeNF { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroCarga { get; set; }
        public string NumeroNavio { get; set; }
        public string NumeroPedido { get; set; }
        public string NumeroPedidoInterno { get; set; }
        public string ObservacaoCTe { get; set; }
        public string Ordem { get; set; }
        public string Origem { get; set; }
        public decimal Paletes { get; set; }
        public decimal Peso { get; set; }
        public decimal PesoPedido { get; set; }
        public string PortoChegada { get; set; }
        public string PortoSaida { get; set; }
        private DateTime PrevisaoEntregaPedido { get; set; }
        public DateTime PrevisaoEntregaAnterior { get; set; }
        public ResponsavelAlteracaoDataPedido PrevisaoEntregaAnteriorResponsavel { get; set; }
        public string PrevisaoEntregaAnteriorObservacao { get; set; }
        public DateTime PrevisaoSaida { get; set; }
        public DateTime PrevisaoSaidaAnterior { get; set; }
        public ResponsavelAlteracaoDataPedido PrevisaoSaidaAnteriorResponsavel { get; set; }
        public string PrevisaoSaidaAnteriorObservacao { get; set; }
        public string Recebedor { get; set; }
        public string Remetente { get; set; }
        public string Restricoes { get; set; }
        public string Reserva { get; set; }
        public string Resumo { get; set; }
        public string RotaFrete { get; set; }
        public string SenhaAgendamento { get; set; }
        public string SenhaAgendamentoCliente { get; set; }
        public SituacaoPedido SituacaoPedido { get; set; }
        public string Temperatura { get; set; }
        public string TipoCarga { get; set; }
        public string TipoDestinatario { get; set; }
        public string TipoEmbarque { get; set; }
        public string TipoExpedidor { get; set; }
        public string TipoOperacao { get; set; }
        public string TipoRecebedor { get; set; }
        public string TipoRemetente { get; set; }
        public Dominio.Enumeradores.TipoTomador TipoTomadorPagador { get; set; }
        public string TipoSeparacao { get; set; }
        public decimal TotalReceber { get; set; }
        public string Transbordo { get; set; }
        public string Transportador { get; set; }
        public string PaisDestino { get; set; }
        public string UFDestino { get; set; }
        public string PaisOrigem { get; set; }
        public string UFOrigem { get; set; }
        public decimal ValorMercadoria { get; set; }
        public string Veiculos { get; set; }
        public int Volume { get; set; }
        public decimal QtdeItensProdutos { get; set; }
        public string ObservacaoInterna { get; set; }
        public string ObservacaoCarga { get; set; }
        private SituacaoCarga SituacaoCarga { get; set; }
        private DateTime DataInicioViagem { get; set; }
        private SituacaoEntrega SituacaoEntrega { get; set; }
        private DateTime DataEntrega { get; set; }
        public string PedidoComAgenda { get; set; }
        public int Saldo { get; set; }
        public DateTime DataFimJanela { get; set; }
        public string Gerente { get; set; }
        public string Vendedor { get; set; }
        public string Supervisor { get; set; }
        public string NumeroPedidoCliente { get; set; }
        public decimal GrossSales { get; set; }
        private bool PossuiIsca { get; set; }
        public string Observacao { get; set; }
        public string NumeroOrdem { get; set; }
        private bool PossuiEtiquetagem { get; set; }
        private DateTime DataAlocacaoPedido { get; set; }
        private DateTime DataAlocacaoISIS { get; set; }
        public decimal ValorNota { get; set; }
        public int Carregamento { get; set; }
        public int OrdemCarregamento { get; set; }
        public string CodIntegracaoDestinatarioPedido { get; set; }
        public string CodIntegracaoTipoCargaPedido { get; set; }
        public string CodIntegracaoProdutoEmbarcadorPedido { get; set; }
        public string DescricaoProdutoEmbarcadorPedido { get; set; }
        private decimal QtdProdutoPedido { get; set; }
        private decimal PesoProdutoPedido { get; set; }
        private bool CanceladoAposVinculoCarga { get; set; }
        public decimal ValorFreteNegociado { get; set; }
        public decimal ValorFreteTransportadorTerceiro { get; set; }
        public decimal ValorFreteToneladaTerceiro { get; set; }
        public decimal ValorFreteToneladaNegociado { get; set; }
        public decimal ValorPedagioRota { get; set; }
        public decimal ValorTotalPedido { get; set; }
        public DateTime DataSalvamentoDadosTransporte { get; set; }
        public DateTime DataConfirmacaoEnvioDocumentos { get; set; }
        public DateTime DataConfirmacaoValorFrete { get; set; }
        public DateTime DataEnvioCTeOcorrencia { get; set; }
        public DateTime PrevisaoEntregaReprogramada { get; set; }
        public DateTime DataPrevista { get; set; }
        public int IdAgrupador { get; set; }
        public string GerenteRegional { get; set; }
        public string CodigoSAP { get; set; }
        public string CodigoIntegracao { get; set; }
        public string CodigoArmador { get; set; }
        public string CodigoDespachante { get; set; }
        public string CodigoInLand { get; set; }
        public string CodigoPortoDestino { get; set; }
        public string CodigoPortoOrigem { get; set; }
        public DateTime DataDeadLineCarga { get; set; }
        public DateTime DataDeadLineDraf { get; set; }
        public DateTime DataEstufagem { get; set; }
        public DateTime DataETATransbordo { get; set; }
        public DateTime DataETS { get; set; }
        public string DescricaoDespachante { get; set; }
        public string DescricaoInLand { get; set; }
        public string DescricaoPortoDestino { get; set; }
        public string DescricaoPortoOrigem { get; set; }
        public string Incoterm { get; set; }
        public string NomeNavio { get; set; }
        public string NomeNavioTransbordo { get; set; }
        public string NumeroContainer { get; set; }
        public string NumeroEXP { get; set; }
        public string NumeroLacre { get; set; }
        public string OrdemEmbarque { get; set; }
        public PagamentoMaritimo PagamentoMaritimo { get; set; }
        public string PaisPortoDestino { get; set; }
        public string PaisPortoOrigem { get; set; }
        public bool PossuiGenset { get; set; }
        public string SiglaPaisPortoDestino { get; set; }
        public string SiglaPaisPortoOrigem { get; set; }
        public string TerminalOrigem { get; set; }
        public string TipoContainer { get; set; }
        public string ViaTransporte { get; set; }
        public string ViagemNavio { get; set; }
        public string PortoDestino { get; set; }
        public string PortoOrigem { get; set; }
        private DateTime DataETAPortoOrigem { get; set; }
        private DateTime DataETSPortoOrigem { get; set; }
        private DateTime DataETAPortoDestino { get; set; }
        private DateTime DataETSPortoDestino { get; set; }
        private DateTime DataInclusaoPedido { get; set; }
        public TipoPropostaMultimodal TipoPropostaMultiModal { get; set; }
        public decimal QuantidadeTotalProduto { get; set; }
        private string NumeroRedespacho { get; set; }
        public DateTime DataEntradaNoRaio { get; set; }
        public string OperadorPedido { get; set; }
        public string CentroResultado { get; set; }
        public string NaturezaOP { get; set; }
        public string EmailSolicitante { get; set; }
        public string TipoServicoPedido { get; set; }
        private DateTime DataInicioJanela { get; set; }
        public string CodIntegracaoRemetente { get; set; }
        public string CodIntegracaoDestinatario { get; set; }
        public bool AguardandoIntegracao { get; set; }
        public int CargaPossuiAnexos { get; set; }
        public DateTime DataDeCriacaoPedidoERP { get; set; }
        public int QuantidadePacotes { get; set; }
        public int QuantidadePacotesColetados { get; set; }
        public decimal KmRota { get; set; }
        public string Autorizado { get; set; }
        public string Autorizador { get; set; }
        public string MotivoPedido { get; set; }
        public string MotivoAutorizacaoPedido { get; set; }
        public bool PedidoBloqueado { get; set; }
        public DateTime DataEntregaOcorrenciaPedido { get; set; }
        public int Protocolo { get; set; }
        public int CentroDeCustoViagemCodigo { get; set; }
        public string CentroDeCustoViagemDescricao { get; set; }
        public string CPFMotoristas { get; set; }
        public double CNPJTomadorOutros { get; set; }
        public string TomadorOutros { get; set; }
        public string TipoTomadorOutros { get; set; }
        public string NumeroCarregamento { get; set; }
        private bool Substituicao { get; set; }
        public string NotasFiscaisPedido { get; set; }
        public decimal QtdPalletsCarregado { get; set; }
        public string LinhaSeparacao { get; set; }
        public string MotivoCancelamento { get; set; }
        public string UsuarioCancelamento { get; set; }
        public string EscritorioVenda { get; set; }
        public string EquipeVendas { get; set; }
        public string CanalVenda { get; set; }
        public string TipoMercado { get; set; }
        public CargaCritica CargaCritica { get; set; }

        public DateTime DataVinculoTracao { get; set; }
        public DateTime DataVinculoReboque { get; set; }
        public DateTime DataVinculoMotorista { get; set; }
        public LocalVinculo LocalVinculo { get; set; }

        public string DescricaoLocalVinculo
        {
            get
            {
                return this.LocalVinculo.ObterDescricao();
            }
        }

        #endregion

        #region Propriedades - Componentes

        public decimal ValorComponente1 { get; set; }
        public decimal ValorComponente2 { get; set; }
        public decimal ValorComponente3 { get; set; }
        public decimal ValorComponente4 { get; set; }
        public decimal ValorComponente5 { get; set; }
        public decimal ValorComponente6 { get; set; }
        public decimal ValorComponente7 { get; set; }
        public decimal ValorComponente8 { get; set; }
        public decimal ValorComponente9 { get; set; }
        public decimal ValorComponente10 { get; set; }
        public decimal ValorComponente11 { get; set; }
        public decimal ValorComponente12 { get; set; }
        public decimal ValorComponente13 { get; set; }
        public decimal ValorComponente14 { get; set; }
        public decimal ValorComponente15 { get; set; }
        public decimal ValorComponente16 { get; set; }
        public decimal ValorComponente17 { get; set; }
        public decimal ValorComponente18 { get; set; }
        public decimal ValorComponente19 { get; set; }
        public decimal ValorComponente20 { get; set; }
        public decimal ValorComponente21 { get; set; }
        public decimal ValorComponente22 { get; set; }
        public decimal ValorComponente23 { get; set; }
        public decimal ValorComponente24 { get; set; }
        public decimal ValorComponente25 { get; set; }
        public decimal ValorComponente26 { get; set; }
        public decimal ValorComponente27 { get; set; }
        public decimal ValorComponente28 { get; set; }
        public decimal ValorComponente29 { get; set; }
        public decimal ValorComponente30 { get; set; }

        #endregion

        #region Propriedades com Regras
        public string DataVinculoTracaoFormatada
        {
            get
            {
                return DataVinculoTracao != DateTime.MinValue ? DataVinculoTracao.ToString("dd/MM/yyyy HH:mm:ss") : "";
            }
        }

        public string DataVinculoReboqueFormatada
        {
            get
            {
                return DataVinculoReboque != DateTime.MinValue ? DataVinculoReboque.ToString("dd/MM/yyyy HH:mm:ss") : "";
            }
        }

        public string DataVinculoMotoristaFormatada
        {
            get
            {
                return DataVinculoMotorista != DateTime.MinValue ? DataVinculoMotorista.ToString("dd/MM/yyyy HH:mm:ss") : "";
            }
        }

        public string DataInicioEmissaoDocumentosFormatada
        {
            get
            {
                return DataInicioEmissaoDocumentos != DateTime.MinValue ? DataInicioEmissaoDocumentos.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataDeCriacaoPedidoERPFormatada
        {
            get
            {
                return DataDeCriacaoPedidoERP != DateTime.MinValue ? DataDeCriacaoPedidoERP.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string PrevisaoEntregaReprogramadaFormatada
        {
            get
            {
                return PrevisaoEntregaReprogramada != DateTime.MinValue ? PrevisaoEntregaReprogramada.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataPrevistaFormatada
        {
            get
            {
                return DataPrevista != DateTime.MinValue ? DataPrevista.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataSalvamentoDadosTransporteFormatada
        {
            get
            {
                return DataSalvamentoDadosTransporte != DateTime.MinValue ? DataSalvamentoDadosTransporte.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataConfirmacaoEnvioDocumentosFormatada
        {
            get
            {
                return DataConfirmacaoEnvioDocumentos != DateTime.MinValue ? DataConfirmacaoEnvioDocumentos.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataConfirmacaoValorFreteFormatada
        {
            get
            {
                return DataConfirmacaoValorFrete != DateTime.MinValue ? DataConfirmacaoValorFrete.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataEnvioCTeOcorrenciaFormatada
        {
            get
            {
                return DataEnvioCTeOcorrencia != DateTime.MinValue ? DataEnvioCTeOcorrencia.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataEntregaOcorrenciaPedidoFormatada
        {
            get
            {
                return DataEntregaOcorrenciaPedido != DateTime.MinValue ? DataEntregaOcorrenciaPedido.ToString("dd/MM/yyyy HH:mm") : string.Empty;
            }
        }

        public string QtdProdutoPedidoFormatada
        {
            get { return QtdProdutoPedido > 0 ? QtdProdutoPedido.ToString("n2") : string.Empty; }
        }

        public string PesoProdutoPedidoFormatada
        {
            get { return PesoProdutoPedido > 0 ? PesoProdutoPedido.ToString("n2") : string.Empty; }
        }

        public string DataAlocacaoISISFormatada
        {
            get { return DataAlocacaoISIS != DateTime.MinValue ? DataAlocacaoISIS.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataAlocacaoPedidoFormatada
        {
            get { return DataAlocacaoPedido != DateTime.MinValue ? DataAlocacaoPedido.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string PossuiEtiquetagemFormatada
        {
            get
            {
                return PossuiEtiquetagem == true ? "Sim" : "Não";
            }
        }

        public string PossuiIscaFormatada
        {
            get
            {
                return PossuiIsca == true ? "Sim" : "Não";
            }
        }
        public string CargaPossuiAnexosFormatada
        {
            get
            {
                return CargaPossuiAnexos > 0 ? "Sim" : "Não";
            }
        }
        public string CNPJDestinatarioFormatado
        {
            get
            {
                if (this.CNPJDestinatario == 0 || string.IsNullOrWhiteSpace(this.TipoDestinatario)) return "";
                if (this.TipoDestinatario.Equals("E")) return "00.000.000/0000-00";
                return this.TipoDestinatario.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CNPJDestinatario) : String.Format(@"{0:000\.000\.000\-00}", this.CNPJDestinatario);
            }
        }

        public string CNPJExpedidorFormatado
        {
            get
            {
                if (this.CNPJExpedidor == 0 || string.IsNullOrWhiteSpace(this.TipoExpedidor)) return "";
                if (this.TipoExpedidor.Equals("E")) return "00.000.000/0000-00";
                return this.TipoExpedidor.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CNPJExpedidor) : String.Format(@"{0:000\.000\.000\-00}", this.CNPJExpedidor);
            }
        }

        public string CNPJRecebedorFormatado
        {
            get
            {
                if (this.CNPJRecebedor == 0 || string.IsNullOrWhiteSpace(this.TipoRecebedor)) return "";
                if (this.TipoRecebedor.Equals("E")) return "00.000.000/0000-00";
                return this.TipoRecebedor.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CNPJRecebedor) : String.Format(@"{0:000\.000\.000\-00}", this.CNPJRecebedor);
            }
        }

        public string CNPJRemetenteFormatado
        {
            get
            {
                if (this.CNPJRemetente == 0 || string.IsNullOrWhiteSpace(this.TipoRemetente)) return "";
                if (this.TipoRemetente.Equals("E")) return "00.000.000/0000-00";
                return this.TipoRemetente.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CNPJRemetente) : String.Format(@"{0:000\.000\.000\-00}", this.CNPJRemetente);
            }
        }

        public double CNPJTomador
        {
            get
            {
                switch (TipoTomadorPagador)
                {
                    case Dominio.Enumeradores.TipoTomador.Destinatario: return CNPJDestinatario;
                    case Dominio.Enumeradores.TipoTomador.Expedidor: return CNPJExpedidor;
                    case Dominio.Enumeradores.TipoTomador.Recebedor: return CNPJRecebedor;
                    case Dominio.Enumeradores.TipoTomador.Remetente: return CNPJRemetente;
                    case Dominio.Enumeradores.TipoTomador.Outros: return CNPJTomadorOutros;
                    default: return 0d;
                }
            }
        }

        public string CNPJTomadorFormatado
        {
            get
            {
                if (this.CNPJTomador == 0 || string.IsNullOrWhiteSpace(this.TipoTomador)) return "";
                if (this.TipoTomador.Equals("E")) return "00.000.000/0000-00";
                return this.TipoTomador.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CNPJTomador) : String.Format(@"{0:000\.000\.000\-00}", this.CNPJTomador);
            }
        }

        public string DataCancelFormatada
        {
            get { return DataCancel != DateTime.MinValue ? DataCancel.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataCarregamentoString
        {
            get { return DataCarregamento != DateTime.MinValue ? DataCarregamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataChipFormatada
        {
            get { return DataChip != DateTime.MinValue ? DataChip.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataColetaString
        {
            get { return DataColeta != DateTime.MinValue ? DataColeta.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataEmbarqueFormatada
        {
            get { return DataEmbarque == DateTime.MinValue ? "" : DataEmbarque.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string DataInicioViagemFormatada
        {
            get { return DataInicioViagem != DateTime.MinValue ? DataInicioViagem.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataEntregaFormatada
        {
            get { return DataEntrega != DateTime.MinValue ? DataEntrega.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataFimJanelaFormatada
        {
            get { return DataFimJanela != DateTime.MinValue ? DataFimJanela.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DescricaoSituacaoPedido
        {
            get
            {
                if (this.CanceladoAposVinculoCarga)
                    return "Cancelado após vincular carga";

                switch (this.SituacaoPedido)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto: return "Aberto";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado: return "Cancelado";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado: return "Finalizado";
                    default: return "";
                }
            }
        }

        public string DescricaoSituacaoEntrega
        {
            get
            {
                return this.SituacaoEntrega.ObterDescricao();
            }
        }

        public string DescricaoSituacaoCarga
        {
            get
            {
                return this.SituacaoCarga.ObterDescricao();
            }
        }


        public string DescricaoPedidoBloqueado
        {
            get
            {
                return this.PedidoBloqueado.ObterDescricao();
            }
        }
        public string DiaSemana
        {
            get { return DataEmbarque == DateTime.MinValue ? "" : DiaSemanaHelper.ObterDiaSemana(DataEmbarque).ObterDescricao(); }
        }

        public string PrevisaoEntregaPedidoFormatada
        {
            get { return PrevisaoEntregaPedido != DateTime.MinValue ? PrevisaoEntregaPedido.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string PrevisaoEntregaAnteriorString
        {
            get { return PrevisaoEntregaAnterior != DateTime.MinValue ? PrevisaoEntregaAnterior.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string PrevisaoEntregaAnteriorResponsavelDescricao
        {
            get
            {
                return PrevisaoEntregaAnteriorResponsavel.ObterDescricao();
            }
        }

        public string PrevisaoSaidaString
        {
            get { return PrevisaoSaida != DateTime.MinValue ? PrevisaoSaida.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string PrevisaoSaidaAnteriorString
        {
            get { return PrevisaoSaidaAnterior != DateTime.MinValue ? PrevisaoSaidaAnterior.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string PrevisaoSaidaAnteriorResponsavelDescricao
        {
            get
            {
                return PrevisaoSaidaAnteriorResponsavel.ObterDescricao();
            }
        }

        public string TipoTomador
        {
            get
            {
                switch (TipoTomadorPagador)
                {
                    case Dominio.Enumeradores.TipoTomador.Destinatario: return TipoDestinatario;
                    case Dominio.Enumeradores.TipoTomador.Expedidor: return TipoExpedidor;
                    case Dominio.Enumeradores.TipoTomador.Recebedor: return TipoRecebedor;
                    case Dominio.Enumeradores.TipoTomador.Remetente: return TipoRemetente;
                    case Dominio.Enumeradores.TipoTomador.Outros: return TipoTomadorOutros;
                    default: return "";
                }
            }
        }

        public string Tomador
        {
            get
            {
                switch (TipoTomadorPagador)
                {
                    case Dominio.Enumeradores.TipoTomador.Destinatario: return Destinatario;
                    case Dominio.Enumeradores.TipoTomador.Expedidor: return Expedidor;
                    case Dominio.Enumeradores.TipoTomador.Recebedor: return Recebedor;
                    case Dominio.Enumeradores.TipoTomador.Remetente: return Remetente;
                    case Dominio.Enumeradores.TipoTomador.Outros: return TomadorOutros;
                    default: return "";
                }
            }
        }

        public string IdAgrupadorDescricao
        {
            get
            {
                return IdAgrupador > 0 ? IdAgrupador.ToString() : "";
            }
        }

        public string DataDeadLineCargaFormatada
        {
            get
            {
                return DataDeadLineCarga != DateTime.MinValue ? DataDeadLineCarga.ToDateString() : "";
            }
        }

        public string DataDeadLineDrafFormatada
        {
            get
            {
                return DataDeadLineDraf != DateTime.MinValue ? DataDeadLineDraf.ToDateString() : "";
            }
        }

        public string DataEstufagemFormatada
        {
            get
            {
                return DataEstufagem != DateTime.MinValue ? DataEstufagem.ToDateString() : "";
            }
        }

        public string DataETATransbordoFormatada
        {
            get
            {
                return DataETATransbordo != DateTime.MinValue ? DataETATransbordo.ToDateString() : "";
            }
        }

        public string DataETSFormatada
        {
            get
            {
                return DataETS != DateTime.MinValue ? DataETS.ToDateString() : "";
            }
        }

        public string Despachante
        {
            get
            {
                List<string> descricao = new List<string>();

                if (!string.IsNullOrWhiteSpace(CodigoDespachante))
                    descricao.Add(CodigoDespachante);

                if (!string.IsNullOrWhiteSpace(DescricaoDespachante))
                    descricao.Add(DescricaoDespachante);

                return string.Join(" - ", descricao);
            }
        }

        public string InLand
        {
            get
            {
                List<string> descricao = new List<string>();

                if (!string.IsNullOrWhiteSpace(CodigoInLand))
                    descricao.Add(CodigoInLand);

                if (!string.IsNullOrWhiteSpace(DescricaoInLand))
                    descricao.Add(DescricaoInLand);

                return string.Join(" - ", descricao);
            }
        }

        public string PagamentoMaritimoDescricao
        {
            get
            {
                return PagamentoMaritimo.ObterDescricao();
            }
        }

        public string PaisPortoViagemDestino
        {
            get
            {
                List<string> descricao = new List<string>();

                if (!string.IsNullOrWhiteSpace(PaisPortoDestino))
                    descricao.Add(PaisPortoDestino);

                if (!string.IsNullOrWhiteSpace(SiglaPaisPortoDestino))
                    descricao.Add(SiglaPaisPortoDestino);

                return string.Join(" - ", descricao);
            }
        }

        public string PaisPortoViagemOrigem
        {
            get
            {
                List<string> descricao = new List<string>();

                if (!string.IsNullOrWhiteSpace(PaisPortoOrigem))
                    descricao.Add(PaisPortoOrigem);

                if (!string.IsNullOrWhiteSpace(SiglaPaisPortoOrigem))
                    descricao.Add(SiglaPaisPortoOrigem);

                return string.Join(" - ", descricao);
            }
        }

        public string PortoViagemDestino
        {
            get
            {
                List<string> descricao = new List<string>();

                if (!string.IsNullOrWhiteSpace(CodigoPortoDestino))
                    descricao.Add(CodigoPortoDestino);

                if (!string.IsNullOrWhiteSpace(DescricaoPortoDestino))
                    descricao.Add(DescricaoPortoDestino);

                return string.Join(" - ", descricao);
            }
        }

        public string PortoViagemOrigem
        {
            get
            {
                List<string> descricao = new List<string>();

                if (!string.IsNullOrWhiteSpace(CodigoPortoOrigem))
                    descricao.Add(CodigoPortoOrigem);

                if (!string.IsNullOrWhiteSpace(DescricaoPortoOrigem))
                    descricao.Add(DescricaoPortoOrigem);

                return string.Join(" - ", descricao);
            }
        }

        public string PossuiGensetDescricao
        {
            get
            {
                return PossuiGenset ? "Sim" : "Não";
            }
        }

        public string SemanaEstufagem
        {
            get
            {
                if (DataEstufagem == DateTime.MinValue)
                    return "";

                return System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DataEstufagem, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Sunday).ToString();
            }
        }

        public string CPFMotoristasFormatado
        {
            get
            {
                string CPFSFormatado = null;
                if (!String.IsNullOrEmpty(CPFMotoristas))
                {
                    foreach (var x in CPFMotoristas?.Split(','))
                    {
                        CPFSFormatado = string.Concat(CPFSFormatado, CPFSFormatado != null ? "," : "", x.ObterCpfFormatado());

                    }
                }
                return CPFSFormatado;
            }
        }
        public string DataETAPortoOrigemFormatada
        {
            get
            {
                return DataETAPortoOrigem != DateTime.MinValue ? DataETAPortoOrigem.ToDateString() : "";
            }
        }
        public string DataETSPortoOrigemFormatada
        {
            get
            {
                return DataETSPortoOrigem != DateTime.MinValue ? DataETSPortoOrigem.ToDateString() : "";
            }
        }
        public string DataETAPortoDestinoFormatada
        {
            get
            {
                return DataETAPortoDestino != DateTime.MinValue ? DataETAPortoDestino.ToDateString() : "";
            }
        }
        public string DataETSPortoDestinoFormatada
        {
            get
            {
                return DataETSPortoDestino != DateTime.MinValue ? DataETSPortoDestino.ToDateString() : "";
            }
        }
        public string DataInclusaoPedidoFormatada
        {
            get
            {
                return DataInclusaoPedido != DateTime.MinValue ? DataInclusaoPedido.ToDateString() : "";
            }
        }
        public string TipoPropostaMultiModalDescricao
        {
            get
            {
                return TipoPropostaMultiModal.ObterDescricao();
            }
        }

        public string CNPJTomadorOutrosFormatado
        {
            get
            {
                if (this.CNPJTomadorOutros == 0 || string.IsNullOrWhiteSpace(this.TipoTomador)) return "";
                if (this.TipoTomador.Equals("E")) return "00.000.000/0000-00";
                return this.TipoTomador.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CNPJTomadorOutros) : String.Format(@"{0:000\.000\.000\-00}", this.CNPJTomadorOutros);
            }
        }
        public string NumeroRedespachoFormatado
        {
            get { return NumeroRedespacho != "0" ? NumeroRedespacho : ""; }
        }

        public string DataInicioJanelaFormatada
        {
            get
            {
                return DataInicioJanela != DateTime.MinValue ? DataInicioJanela.ToDateString() : "";
            }
        }

        public string SubstituicaoDescricao
        {
            get { return Substituicao.ObterDescricao(); }
        }

        public string AguardandoIntegracaoDescricao
        {
            get
            {
                if (this.AguardandoIntegracao)
                    return AguardandoIntegracaoHelper.ObterDescricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.AguardandoIntegracao.Aguardando);
                else
                    return AguardandoIntegracaoHelper.ObterDescricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.AguardandoIntegracao.Confirmado);
            }
        }
        public string CargaCriticaFormatado
        {
            get { return this.CargaCritica.ObterCargaCriticaFormatado(); }
        }
        #endregion
    }
}
