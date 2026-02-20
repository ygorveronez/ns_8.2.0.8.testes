using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public class ConsultaTabelaFrete
    {
        #region Atributos

        private int _numeroCasasDecimais = 5;
        private bool _gerandoExcel = false;

        #endregion

        #region Propriedades

        public int Codigo { get; set; }
        public int CodigoIntegracao { get; set; }
        public bool isRelatorio { get; set; }
        public bool isCSV { get; set; }

        public string DescricaoEmpresa
        {
            get
            {
                //if (!string.IsNullOrWhiteSpace(Empresa))
                if (!string.IsNullOrWhiteSpace(CNPJTransportador))
                {
                    string cnpjcpf = String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(CNPJTransportador));
                    return CodigoIntegracaoTransportador + " - " + Empresa + " (" + cnpjcpf + ")";
                }
                else
                    return "";
            }
        }
        public string Empresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string CodigoIntegracaoTransportador { get; set; }
        public string CodigoIntegracaoRemetente { get; set; }
        public double CPFCNPJRemetente { get; set; }
        public string Remetente { get; set; }
        public string NomeFantasiaRemetente { get; set; }
        public string DescricaoRemetente
        {
            get
            {
                return Remetente;
            }
        }

        public string CodigoIntegracaoDestinatario { get; set; }
        public double CPFCNPJDestinatario { get; set; }
        public string Destinatario { get; set; }
        public string NomeFantasiaDestinatario { get; set; }
        public string DescricaoDestinatario
        {
            get
            {
                return Destinatario;
            }
        }

        public double CPFCNPJTomador { get; set; }
        public string Tomador { get; set; }

        public string CNPJTransportador { get; set; }
        public string CNPJTransportadorFormatado
        {
            get
            {
                return !string.IsNullOrWhiteSpace(CNPJTransportador) ? CNPJTransportador.ObterCnpjFormatado() : "";
            }
        }

        public string Transportador { get; set; }
        public string TransportadorTerceiro { get; set; }

        public string Origem { get; set; }
        public string RegiaoOrigem { get; set; }
        public string EstadoOrigem { get; set; }
        public string CEPOrigem { get; set; }
        public string PaisOrigem { get; set; }
        public string RotaFreteOrigem { get; set; }

        public string Destino { get; set; }
        public string RegiaoDestino { get; set; }
        public string EstadoDestino { get; set; }
        public string CEPDestino { get; set; }
        public string PaisDestino { get; set; }
        public string RotaFrete { get; set; }

        public string TipoOperacao { get; set; }
        public string TabelaFrete { get; set; }
        public string GrupoPessoas { get; set; }
        public string DataAlteracao { get; set; }
        public string UsuarioAlteracao { get; set; }

        public TipoPagamentoEmissao TipoPagamento { get; set; }
        public string DescricaoTipoPagamento
        {
            get { return TipoPagamento.ObterDescricao(); }
        }

        public TipoTabelaFreteCliente Tipo { get; set; }
        public int LeadTime { get; set; }

        private TipoGrupoCarga GrupoCarga { get; set; }
        private bool GerenciarCapacidade { get; set; }
        private EstruturaTabela EstruturaTabela { get; set; }
        public string DescricaoGrupoCarga
        {
            get { return GrupoCarga.ObterDescricao(); }
        }
        public string DescricaoGerenciarCapacidade
        {
            get { return GerenciarCapacidade.ObterDescricao(); }
        }
        public string DescricaoEstruturaTabela
        {
            get { return EstruturaTabela.ObterDescricao(); }
        }
        public int PrazoDiasUteis { get; set; }
        public string CanalEntrega { get; set; }
        public string CanalVenda { get; set; }
        public int ItemCodigo { get; set; }
        public string ItemCodigoFormatado {
            get
            {
                return (ItemCodigo > 0) ? ItemCodigo.ToString() : "";
            }
        }
        public string ItemCodigoRetornoIntegracao { get; set; }
        public string StatusAprovacao { get; set; }
        public string MensagemRetornoIntegracao { get; set; }
        public string StatusAssinaturaContrato { get; set; }
        public string ContratoExternoID { get; set; }
        public int NumeroContrato { get; set; }
        public bool SituacaoTabela { get; set; }
        public string CodigoIntegracaoTabelaFreteCliente { get; set; }
        public string DescricaoSituacaoTabela
        {
            get
            {
                return (SituacaoTabela) ? "Ativo" : "Inativo";
            }
        }
        public int TabelaComVinculoCarga { get; set; }
        public string DescricaoTabelaComVinculoCarga
        {
            get
            {
                return (TabelaComVinculoCarga > 0) ? "Sim" : "Não";
            }
        }

        #endregion

        #region Propriedades - Vigência

        public string DataInicial { get; set; }
        public string DataFinal { get; set; }

        #endregion Propriedades - Vigência

        #region Propriedades - Parâmetro Base

        public decimal ValorParametroBase { get; set; }
        public decimal ValorParametroBase1 { get; set; }
        public decimal ValorParametroBase2 { get; set; }
        public string DescricaoParametroBase { get; set; }
        public TipoParametroBaseTabelaFrete TipoParametroBase { get; set; }
        public int TipoValorParametroBase { get; set; }

        public string ParametroBase
        {
            get
            {
                switch (TipoParametroBase)
                {
                    case TipoParametroBaseTabelaFrete.TipoCarga:
                    case TipoParametroBaseTabelaFrete.ModeloTracao:
                    case TipoParametroBaseTabelaFrete.ModeloReboque:
                        return DescricaoParametroBase;

                    case TipoParametroBaseTabelaFrete.Distancia:
                        if ((TipoDistanciaTabelaFrete)TipoValorParametroBase == TipoDistanciaTabelaFrete.PorFaixaDistanciaPercorrida)
                        {
                            if (ValorParametroBase1 <= 0 && ValorParametroBase2 > 0)
                                return "Até " + ValorParametroBase2.ToString("n2") + " KM";
                            else if (ValorParametroBase1 > 0 && ValorParametroBase2 > 0)
                                return "De " + ValorParametroBase1.ToString("n2") + " até " + ValorParametroBase2.ToString("n2") + " KM";
                            else
                                return "Acima de " + ValorParametroBase1.ToString("n2") + " KM";
                        }
                        else
                        {
                            return "A cada " + ValorParametroBase.ToString("n2") + " KM";
                        }

                    case TipoParametroBaseTabelaFrete.NumeroEntrega:
                        if ((TipoNumeroEntregaTabelaFrete)TipoValorParametroBase == TipoNumeroEntregaTabelaFrete.PorFaixaEntrega)
                        {
                            if (ValorParametroBase1 <= 0 && ValorParametroBase2 > 0)
                                return "Até " + ValorParametroBase2.ToString("n0") + " entregas";
                            else if (ValorParametroBase1 > 0 && ValorParametroBase2 > 0)
                                return "De " + ValorParametroBase1.ToString("n0") + " até " + ValorParametroBase2.ToString("n0") + " entregas";
                            else
                                return "Acima de " + ValorParametroBase1.ToString("n0") + " entregas";
                        }
                        else
                        {
                            return "Por entrega";
                        }
                    case TipoParametroBaseTabelaFrete.Pallets:
                        if ((TipoNumeroPalletsTabelaFrete)TipoValorParametroBase == TipoNumeroPalletsTabelaFrete.PorFaixaPallets)
                        {
                            if (ValorParametroBase1 <= 0 && ValorParametroBase2 > 0)
                                return "Até " + ValorParametroBase2.ToString("n0") + " pallets";
                            else if (ValorParametroBase1 > 0 && ValorParametroBase2 > 0)
                                return "De " + ValorParametroBase1.ToString("n0") + " até " + ValorParametroBase2.ToString("n0") + " pallets";
                            else
                                return "Acima de " + ValorParametroBase1.ToString("n0") + " pallets";
                        }
                        else
                        {
                            return "Por Pallet";
                        }
                    case TipoParametroBaseTabelaFrete.Peso:
                        if ((EnumTipoPesoTabelaFrete)TipoValorParametroBase == EnumTipoPesoTabelaFrete.PorFaixaPesoTransportado)
                        {
                            if (ValorParametroBase1 <= 0 && ValorParametroBase2 > 0)
                                return "Até " + ValorParametroBase2.ToString("n4") + " " + DescricaoParametroBase;
                            else if (ValorParametroBase1 > 0 && ValorParametroBase2 > 0)
                                return "De " + ValorParametroBase1.ToString("n4") + " até " + ValorParametroBase2.ToString("n4") + " " + DescricaoParametroBase;
                            else
                                return "Acima de " + ValorParametroBase1.ToString("n4") + " " + DescricaoParametroBase;
                        }
                        else
                        {
                            return "A cada " + ValorParametroBase.ToString("n4") + " " + DescricaoParametroBase;
                        }

                    default:
                        return string.Empty;
                }
            }
        }

        #endregion Propriedades - Parâmetro Base

        #region Propriedades - Tipo de Carga

        public int CodigoItemTipoCarga { get; set; }
        public decimal AntigoValorTipoCarga { get; set; }
        public string TipoCarga { get; set; }
        public decimal ValorTipoCarga { get; set; }
        public TipoCampoValorTabelaFrete TipoValorTipoCarga { get; set; }

        public decimal PercentualDiferencaAtualizacaoValorTipoCarga
        {
            get
            {
                return ValorTipoCarga.PercentageDifferenceTo(AntigoValorTipoCarga);
            }
        }

        public string DescricaoValorTipoCarga
        {
            get
            {
                return _gerandoExcel ? ValorTipoCarga.ToString($"n{_numeroCasasDecimais}") : ObterTipoValor(TipoValorTipoCarga, CodigoItemTipoCarga) + " " + ValorTipoCarga.ToString($"n{_numeroCasasDecimais}");
            }
        }

        public string DescricaoAntigoValorTipoCarga
        {
            get
            {
                return AntigoValorTipoCarga.ToString($"n{_numeroCasasDecimais}");
            }
        }

        #endregion Propriedades - Tipo de Carga

        #region Propriedades - Modelos de Tração

        public int CodigoItemModeloTracao { get; set; }
        public string ModeloTracao { get; set; }
        public decimal ValorModeloTracao { get; set; }
        public decimal AntigoValorModeloTracao { get; set; }
        public TipoCampoValorTabelaFrete TipoValorModeloTracao { get; set; }

        public decimal PercentualDiferencaAtualizacaoValorModeloTracao
        {
            get
            {
                return AntigoValorModeloTracao.PercentageDifferenceTo(ValorModeloTracao);
            }
        }

        public string DescricaoValorModeloTracao
        {
            get
            {
                return _gerandoExcel ? ValorModeloTracao.ToString($"n{_numeroCasasDecimais}") : ObterTipoValor(TipoValorModeloTracao, CodigoItemModeloTracao) + " " + ValorModeloTracao.ToString($"n{_numeroCasasDecimais}");
            }
        }

        public string DescricaoAntigoValorModeloTracao
        {
            get
            {
                return AntigoValorModeloTracao.ToString($"n{_numeroCasasDecimais}");
            }
        }

        #endregion Propriedades - Modelos de Tração

        #region Propriedades - Modelos de Reboque

        public int CodigoItemModeloReboque { get; set; }
        public string ModeloReboque { get; set; }
        public decimal ValorModeloReboque { get; set; }
        public decimal AntigoValorModeloReboque { get; set; }
        public TipoCampoValorTabelaFrete TipoValorModeloReboque { get; set; }

        public decimal PercentualDiferencaAtualizacaoValorModeloReboque
        {
            get
            {
                return AntigoValorModeloReboque.PercentageDifferenceTo(ValorModeloReboque);
            }
        }

        public string DescricaoValorModeloReboque
        {
            get
            {
                return _gerandoExcel ? ValorModeloReboque.ToString($"n{_numeroCasasDecimais}") : ObterTipoValor(TipoValorModeloReboque, CodigoItemModeloReboque) + " " + ValorModeloReboque.ToString($"n{_numeroCasasDecimais}");
            }
        }

        public string DescricaoAntigoValorModeloReboque
        {
            get
            {
                return AntigoValorModeloReboque.ToString($"n{_numeroCasasDecimais}");
            }
        }

        #endregion Propriedades - Modelos de Reboque

        #region Propriedades - Entregas

        public int CodigoItemNumeroEntrega { get; set; }
        public int CodigoItemNumeroEntregaExcedente { get; set; }
        public int NumeroInicialEntrega { get; set; }
        public int NumeroFinalEntrega { get; set; }
        public decimal ValorEntrega { get; set; }
        public decimal AntigoValorEntrega { get; set; }
        public decimal ValorEntregaExcedente { get; set; }
        public decimal AntigoValorEntregaExcedente { get; set; }
        public TipoNumeroEntregaTabelaFrete TipoEntrega { get; set; }
        public TipoCampoValorTabelaFrete TipoValorEntrega { get; set; }

        public decimal PercentualDiferencaAtualizacaoValorEntregaExcedente
        {
            get
            {
                return AntigoValorEntregaExcedente.PercentageDifferenceTo(ValorEntregaExcedente);
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorEntrega
        {
            get
            {
                return AntigoValorEntrega.PercentageDifferenceTo(ValorEntrega);
            }
        }

        public string NumeroEntrega
        {
            get
            {
                if (TipoEntrega == TipoNumeroEntregaTabelaFrete.PorFaixaEntrega)
                {
                    if (NumeroInicialEntrega <= 0 && NumeroFinalEntrega > 0)
                        return "Até " + NumeroFinalEntrega.ToString("n0") + " entregas";
                    else if (NumeroInicialEntrega > 0 && NumeroFinalEntrega > 0)
                        return "De " + NumeroInicialEntrega.ToString("n0") + " até " + NumeroFinalEntrega.ToString("n0") + " entregas";
                    else
                        return "Acima de " + NumeroInicialEntrega.ToString("n0") + " entregas";
                }
                else
                {
                    return "Por entrega";
                }
            }
        }

        public string DescricaoValorEntrega
        {
            get
            {
                return _gerandoExcel ? ValorEntrega.ToString($"n{_numeroCasasDecimais}") : ObterTipoValor(TipoValorEntrega, CodigoItemNumeroEntrega) + " " + ValorEntrega.ToString($"n{_numeroCasasDecimais}");
            }
        }

        public string DescricaoAntigoValorEntrega
        {
            get
            {
                return AntigoValorEntrega.ToString($"n{_numeroCasasDecimais}");
            }
        }

        public string DescricaoValorEntregaExcedente
        {
            get
            {
                return _gerandoExcel ? ValorEntregaExcedente.ToString($"n{_numeroCasasDecimais}") : $"{ObterTipoValor(TipoCampoValorTabelaFrete.ValorFixo, CodigoItemNumeroEntregaExcedente, "EntregaExcedente")} {ValorEntregaExcedente.ToString($"n{_numeroCasasDecimais}")}";
            }
        }

        public string DescricaoAntigoValorEntregaExcedente
        {
            get
            {
                return AntigoValorEntregaExcedente.ToString($"n{_numeroCasasDecimais}");
            }
        }

        #endregion Propriedades - Entregas

        #region Propriedades - Tipo de Embalagem

        public int CodigoItemTipoEmbalagem { get; set; }
        public string TipoEmbalagem { get; set; }
        public decimal ValorTipoEmbalagem { get; set; }
        public decimal AntigoValorTipoEmbalagem { get; set; }
        public TipoCampoValorTabelaFrete TipoValorTipoEmbalagem { get; set; }

        public decimal PercentualDiferencaAtualizacaoValorTipoEmbalagem
        {
            get
            {
                return AntigoValorTipoEmbalagem.PercentageDifferenceTo(ValorTipoEmbalagem);
            }
        }

        public string DescricaoValorTipoEmbalagem
        {
            get
            {
                return _gerandoExcel ? ValorTipoEmbalagem.ToString($"n{_numeroCasasDecimais}") : ObterTipoValor(TipoValorTipoEmbalagem, CodigoItemTipoEmbalagem) + " " + ValorTipoEmbalagem.ToString($"n{_numeroCasasDecimais}");
            }
        }

        public string DescricaoAntigoValorTipoEmbalagem
        {
            get
            {
                return AntigoValorTipoEmbalagem.ToString($"n{_numeroCasasDecimais}");
            }
        }

        #endregion Propriedades - Tipo de Embalagem

        #region Propriedades - Tempo

        public int CodigoItemTempo { get; set; }
        public string HoraInicialTempo { get; set; }
        public string HoraFinalTempo { get; set; }
        public decimal ValorTempo { get; set; }
        public decimal AntigoValorTempo { get; set; }
        public TipoCampoValorTabelaFrete TipoValorTempo { get; set; }

        public decimal PercentualDiferencaAtualizacaoValorTempo
        {
            get
            {
                return AntigoValorTempo.PercentageDifferenceTo(ValorTempo);
            }
        }

        public string HoraTempo
        {
            get
            {
                return HoraInicialTempo + " até " + HoraFinalTempo;
            }
        }

        public string DescricaoValorTempo
        {
            get
            {
                return _gerandoExcel ? ValorTempo.ToString($"n{_numeroCasasDecimais}") : ObterTipoValor(TipoValorTempo, CodigoItemTempo) + " " + ValorTempo.ToString($"n{_numeroCasasDecimais}");
            }
        }

        public string DescricaoAntigoValorTempo
        {
            get
            {
                return AntigoValorTempo.ToString($"n{_numeroCasasDecimais}");
            }
        }

        #endregion Propriedades - Tempo

        #region Propriedades - Pallets

        public int CodigoItemNumeroPallets { get; set; }
        public int CodigoItemNumeroPalletsExcedente { get; set; }
        public int NumeroInicialPallets { get; set; }
        public int NumeroFinalPallets { get; set; }
        public decimal ValorPallets { get; set; }
        public decimal AntigoValorPallets { get; set; }
        public decimal ValorPalletsExcedente { get; set; }
        public decimal AntigoValorPalletsExcedente { get; set; }
        public TipoNumeroPalletsTabelaFrete TipoPallets { get; set; }
        public TipoCampoValorTabelaFrete TipoValorPallets { get; set; }

        public decimal PercentualDiferencaAtualizacaoValorPalletsExcedente
        {
            get
            {
                return AntigoValorPalletsExcedente.PercentageDifferenceTo(ValorPalletsExcedente);
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorPallets
        {
            get
            {
                return AntigoValorPallets.PercentageDifferenceTo(ValorPallets);
            }
        }

        public string NumeroPallets
        {
            get
            {
                if (TipoPallets == TipoNumeroPalletsTabelaFrete.PorFaixaPallets)
                {
                    if (NumeroInicialPallets <= 0 && NumeroFinalPallets > 0)
                        return "Até " + NumeroFinalPallets.ToString("n0") + " pallets";
                    else if (NumeroInicialPallets > 0 && NumeroFinalPallets > 0)
                        return "De " + NumeroInicialPallets.ToString("n0") + " até " + NumeroFinalPallets.ToString("n0") + " pallets";
                    else
                        return "Acima de " + NumeroInicialPallets.ToString("n0") + " pallets";
                }
                else
                {
                    return "Por pallet";
                }
            }
        }

        public string DescricaoValorPallets
        {
            get
            {
                return _gerandoExcel ? ValorPallets.ToString($"n{_numeroCasasDecimais}") : ObterTipoValor(TipoValorPallets, CodigoItemNumeroPallets) + " " + ValorPallets.ToString($"n{_numeroCasasDecimais}");
            }
        }

        public string DescricaoAntigoValorPallets
        {
            get
            {
                return AntigoValorPallets.ToString($"n{_numeroCasasDecimais}");
            }
        }

        public string DescricaoValorPalletsExcedente
        {
            get
            {
                return _gerandoExcel ? ValorPalletsExcedente.ToString($"n{_numeroCasasDecimais}") : $"{ObterTipoValor(TipoCampoValorTabelaFrete.ValorFixo, CodigoItemNumeroPalletsExcedente, "PalletsExcedente")} {ValorPalletsExcedente.ToString($"n{_numeroCasasDecimais}")}";
            }
        }

        public string DescricaoAntigoValorPalletsExcedente
        {
            get
            {
                return AntigoValorPalletsExcedente.ToString($"n{_numeroCasasDecimais}");
            }
        }

        #endregion Propriedades - Pallets

        #region Propriedades - Ajudante

        public int CodigoItemNumeroAjudante { get; set; }
        public int CodigoItemNumeroAjudanteExcedente { get; set; }
        public int NumeroInicialAjudante { get; set; }
        public int NumeroFinalAjudante { get; set; }
        public decimal ValorAjudante { get; set; }
        public decimal AntigoValorAjudante { get; set; }
        public decimal ValorAjudanteExcedente { get; set; }
        public decimal AntigoValorAjudanteExcedente { get; set; }
        public TipoCobrancaAjudanteTabelaFrete TipoAjudante { get; set; }
        public TipoCampoValorTabelaFrete TipoValorAjudante { get; set; }

        public decimal PercentualDiferencaAtualizacaoValorAjudanteExcedente
        {
            get
            {
                return AntigoValorAjudanteExcedente.PercentageDifferenceTo(ValorAjudanteExcedente);
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorAjudante
        {
            get
            {
                return AntigoValorAjudante.PercentageDifferenceTo(ValorAjudante);
            }
        }

        public string NumeroAjudante
        {
            get
            {
                if (TipoAjudante == TipoCobrancaAjudanteTabelaFrete.PorFaixaAjudantes)
                {
                    if (NumeroInicialAjudante <= 0 && NumeroFinalAjudante > 0)
                        return "Até " + NumeroFinalAjudante.ToString("n0") + " ajudantes";
                    else if (NumeroInicialAjudante > 0 && NumeroFinalAjudante > 0)
                        return "De " + NumeroInicialAjudante.ToString("n0") + " até " + NumeroFinalAjudante.ToString("n0") + " ajudantes";
                    else
                        return "Acima de " + NumeroInicialAjudante.ToString("n0") + " ajudantes";
                }
                else
                {
                    return "Por ajudante";
                }
            }
        }

        public string DescricaoValorAjudante
        {
            get
            {
                return _gerandoExcel ? ValorAjudante.ToString($"n{_numeroCasasDecimais}") : ObterTipoValor(TipoValorAjudante, CodigoItemNumeroAjudante) + " " + ValorAjudante.ToString($"n{_numeroCasasDecimais}");
            }
        }

        public string DescricaoAntigoValorAjudante
        {
            get
            {
                return AntigoValorAjudante.ToString($"n{_numeroCasasDecimais}");
            }
        }

        public string DescricaoValorAjudanteExcedente
        {
            get
            {
                return _gerandoExcel ? ValorAjudanteExcedente.ToString($"n{_numeroCasasDecimais}") : $"{ObterTipoValor(TipoCampoValorTabelaFrete.ValorFixo, CodigoItemNumeroAjudanteExcedente, "AjudanteExcedente")} {ValorAjudanteExcedente.ToString($"n{_numeroCasasDecimais}")}";
            }
        }

        public string DescricaoAntigoValorAjudanteExcedente
        {
            get
            {
                return AntigoValorAjudanteExcedente.ToString($"n{_numeroCasasDecimais}");
            }
        }

        #endregion Propriedades - Ajudante

        #region Propriedades - Peso

        public int CodigoItemPeso { get; set; }
        public int CodigoItemPesoExcedente { get; set; }
        public decimal Peso { get; set; }
        public decimal PesoInicial { get; set; }
        public decimal PesoFinal { get; set; }
        public string UnidadeMedida { get; set; }
        public decimal ValorPeso { get; set; }
        public decimal AntigoValorPeso { get; set; }
        public decimal ValorPesoExcedente { get; set; }
        public decimal AntigoValorPesoExcedente { get; set; }
        public EnumTipoPesoTabelaFrete TipoPeso { get; set; }
        public TipoCampoValorTabelaFrete TipoValorPeso { get; set; }

        public decimal PercentualDiferencaAtualizacaoValorPesoExcedente
        {
            get
            {
                return AntigoValorPesoExcedente.PercentageDifferenceTo(ValorPesoExcedente);
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorPeso
        {
            get
            {
                return AntigoValorPeso.PercentageDifferenceTo(ValorPeso);
            }
        }

        public string DescricaoPeso
        {
            get
            {
                if (TipoPeso == EnumTipoPesoTabelaFrete.PorFaixaPesoTransportado)
                {
                    if (PesoInicial <= 0 && PesoFinal > 0)
                        return "Até " + PesoFinal.ToString("n4") + " " + UnidadeMedida;
                    else if (PesoInicial > 0 && PesoFinal > 0)
                        return "De " + PesoInicial.ToString("n4") + " até " + PesoFinal.ToString("n4") + " " + UnidadeMedida;
                    else
                        return "Acima de " + PesoInicial.ToString("n4") + " " + UnidadeMedida;
                }
                else
                {
                    return "A cada " + Peso.ToString("n2") + " " + UnidadeMedida;
                }
            }
        }

        public string DescricaoValorPeso
        {
            get
            {
                return _gerandoExcel ? ValorPeso.ToString($"n{_numeroCasasDecimais}") : ObterTipoValor(TipoValorPeso, CodigoItemPeso) + " " + ValorPeso.ToString($"n{_numeroCasasDecimais}");
            }
        }

        public string DescricaoAntigoValorPeso
        {
            get
            {
                return AntigoValorPeso.ToString($"n{_numeroCasasDecimais}");
            }
        }

        public string DescricaoValorPesoExcedente
        {
            get
            {
                return _gerandoExcel ? ValorPesoExcedente.ToString($"n{_numeroCasasDecimais}") : $"{ObterTipoValor(TipoCampoValorTabelaFrete.ValorFixo, CodigoItemPesoExcedente, "PesoExcedente")} {ValorPesoExcedente.ToString($"n{_numeroCasasDecimais}")}";
            }
        }

        public string DescricaoAntigoValorPesoExcedente
        {
            get
            {
                return AntigoValorPesoExcedente.ToString($"n{_numeroCasasDecimais}");
            }
        }

        #endregion Propriedades - Peso

        #region Propriedades - Distância

        public int CodigoItemDistancia { get; set; }
        public int CodigoItemDistanciaExcedente { get; set; }
        public decimal Distancia { get; set; }
        public decimal DistanciaInicial { get; set; }
        public decimal DistanciaFinal { get; set; }
        public decimal ValorDistancia { get; set; }
        public decimal AntigoValorDistancia { get; set; }
        public decimal ValorDistanciaExcedente { get; set; }
        public decimal AntigoValorDistanciaExcedente { get; set; }
        public TipoDistanciaTabelaFrete TipoDistancia { get; set; }
        public TipoCampoValorTabelaFrete TipoValorDistancia { get; set; }

        public decimal PercentualDiferencaAtualizacaoValorDistanciaExcedente
        {
            get
            {
                return AntigoValorDistanciaExcedente.PercentageDifferenceTo(ValorDistanciaExcedente);
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorDistancia
        {
            get
            {
                return AntigoValorDistancia.PercentageDifferenceTo(ValorDistancia);
            }
        }

        public string DescricaoDistancia
        {
            get
            {
                if (TipoDistancia == TipoDistanciaTabelaFrete.PorFaixaDistanciaPercorrida)
                {
                    if (DistanciaInicial <= 0 && DistanciaFinal > 0)
                        return "Até " + DistanciaFinal.ToString("n2") + " KM";
                    else if (DistanciaInicial > 0 && DistanciaFinal > 0)
                        return "De " + DistanciaInicial.ToString("n2") + " até " + DistanciaFinal.ToString("n2") + " KM";
                    else
                        return "Acima de " + DistanciaInicial.ToString("n2") + " KM";
                }
                else
                {
                    return "A cada " + Distancia.ToString("n2") + " KM";
                }
            }
        }

        public string DescricaoValorDistancia
        {
            get
            {
                return _gerandoExcel ? ValorDistancia.ToString($"n{_numeroCasasDecimais}") : $"{ObterTipoValor(TipoValorDistancia, CodigoItemDistancia)} {ValorDistancia.ToString($"n{_numeroCasasDecimais}")}";
            }
        }

        public string DescricaoAntigoValorDistancia
        {
            get
            {
                return AntigoValorDistancia.ToString($"n{_numeroCasasDecimais}");
            }
        }

        public string DescricaoValorDistanciaExcedente
        {
            get
            {
                return _gerandoExcel ? ValorDistanciaExcedente.ToString($"n{_numeroCasasDecimais}") : $"{ObterTipoValor(TipoCampoValorTabelaFrete.ValorFixo, CodigoItemDistanciaExcedente, "DistanciaExcedente")} {ValorDistanciaExcedente.ToString($"n{_numeroCasasDecimais}")}";
            }
        }

        public string DescricaoAntigoValorDistanciaExcedente
        {
            get
            {
                return AntigoValorDistanciaExcedente.ToString($"n{_numeroCasasDecimais}");
            }
        }

        #endregion Propriedades - Distância

        #region Propriedades - Valor Mínimo

        public decimal ValorMinimo { get; set; }
        public decimal AntigoValorMinimo { get; set; }

        public decimal PercentualDiferencaAtualizacaoValorMinimo
        {
            get
            {
                return AntigoValorMinimo.PercentageDifferenceTo(ValorMinimo);
            }
        }

        #endregion Propriedades - Valor Mínimo

        #region Propriedades - Valor Máximo

        public decimal ValorMaximo { get; set; }
        public decimal AntigoValorMaximo { get; set; }

        public decimal PercentualDiferencaAtualizacaoValorMaximo
        {
            get
            {
                return AntigoValorMaximo.PercentageDifferenceTo(ValorMaximo);
            }
        }

        #endregion Propriedades - Valor Máximo

        #region Propriedades - Valor Base

        public int CodigoValorBase { get; set; }
        public decimal ValorBase { get; set; }
        public decimal AntigoValorBase { get; set; }

        public decimal PercentualDiferencaAtualizacaoValorBase
        {
            get
            {
                return AntigoValorBase.PercentageDifferenceTo(ValorBase);
            }
        }

        public string DescricaoValorBase
        {
            get
            {
                return _gerandoExcel ? ValorBase.ToString($"n{_numeroCasasDecimais}") : $"{ObterTipoValor(TipoCampoValorTabelaFrete.ValorFixo, CodigoValorBase, "ValorBase")} {ValorBase.ToString($"n{_numeroCasasDecimais}")}";
            }
        }

        #endregion Propriedades - Valor Base

        #region Propriedades - Valor Total

        public string ValorTotal
        {
            get
            {
                return ValorTotalDecimal.ToString($"n{_numeroCasasDecimais}");
            }
        }

        public decimal ValorTotalDecimal
        {
            get
            {
                decimal valorTotal = 0m;

                SomarValorTotal(ref valorTotal, TipoValorComponente1, ValorComponente1);
                SomarValorTotal(ref valorTotal, TipoValorComponente2, ValorComponente2);
                SomarValorTotal(ref valorTotal, TipoValorComponente3, ValorComponente3);
                SomarValorTotal(ref valorTotal, TipoValorComponente4, ValorComponente4);
                SomarValorTotal(ref valorTotal, TipoValorComponente5, ValorComponente5);
                SomarValorTotal(ref valorTotal, TipoValorComponente6, ValorComponente6);
                SomarValorTotal(ref valorTotal, TipoValorComponente7, ValorComponente7);
                SomarValorTotal(ref valorTotal, TipoValorComponente8, ValorComponente8);
                SomarValorTotal(ref valorTotal, TipoValorComponente9, ValorComponente9);
                SomarValorTotal(ref valorTotal, TipoValorComponente10, ValorComponente10);
                SomarValorTotal(ref valorTotal, TipoValorComponente11, ValorComponente11);
                SomarValorTotal(ref valorTotal, TipoValorComponente12, ValorComponente12);
                SomarValorTotal(ref valorTotal, TipoValorComponente13, ValorComponente13);
                SomarValorTotal(ref valorTotal, TipoValorComponente14, ValorComponente14);
                SomarValorTotal(ref valorTotal, TipoValorComponente15, ValorComponente15);

                SomarValorTotal(ref valorTotal, TipoValorDistancia, ValorDistancia);
                SomarValorTotal(ref valorTotal, TipoValorAjudante, ValorAjudante);
                SomarValorTotal(ref valorTotal, TipoValorEntrega, ValorEntrega);
                SomarValorTotal(ref valorTotal, TipoValorTipoEmbalagem, ValorTipoEmbalagem);
                SomarValorTotal(ref valorTotal, TipoValorModeloReboque, ValorModeloReboque);
                SomarValorTotal(ref valorTotal, TipoValorModeloTracao, ValorModeloTracao);
                SomarValorTotal(ref valorTotal, TipoValorPallets, ValorPallets);
                SomarValorTotal(ref valorTotal, TipoValorPeso, ValorPeso);
                SomarValorTotal(ref valorTotal, TipoValorTempo, ValorTempo);
                SomarValorTotal(ref valorTotal, TipoValorTipoCarga, ValorTipoCarga);

                valorTotal += ValorBase;

                return valorTotal;
            }
        }

        public string AntigoValorTotal
        {
            get
            {
                return AntigoValorTotalDecimal.ToString($"n{_numeroCasasDecimais}");
            }
        }

        public decimal AntigoValorTotalDecimal
        {
            get
            {
                decimal valorTotal = 0m;

                SomarValorTotal(ref valorTotal, TipoValorComponente1, AntigoValorComponente1);
                SomarValorTotal(ref valorTotal, TipoValorComponente2, AntigoValorComponente2);
                SomarValorTotal(ref valorTotal, TipoValorComponente3, AntigoValorComponente3);
                SomarValorTotal(ref valorTotal, TipoValorComponente4, AntigoValorComponente4);
                SomarValorTotal(ref valorTotal, TipoValorComponente5, AntigoValorComponente5);
                SomarValorTotal(ref valorTotal, TipoValorComponente6, AntigoValorComponente6);
                SomarValorTotal(ref valorTotal, TipoValorComponente7, AntigoValorComponente7);
                SomarValorTotal(ref valorTotal, TipoValorComponente8, AntigoValorComponente8);
                SomarValorTotal(ref valorTotal, TipoValorComponente9, AntigoValorComponente9);
                SomarValorTotal(ref valorTotal, TipoValorComponente10, AntigoValorComponente10);
                SomarValorTotal(ref valorTotal, TipoValorComponente11, AntigoValorComponente11);
                SomarValorTotal(ref valorTotal, TipoValorComponente12, AntigoValorComponente12);
                SomarValorTotal(ref valorTotal, TipoValorComponente13, AntigoValorComponente13);
                SomarValorTotal(ref valorTotal, TipoValorComponente14, AntigoValorComponente14);
                SomarValorTotal(ref valorTotal, TipoValorComponente15, AntigoValorComponente15);

                SomarValorTotal(ref valorTotal, TipoValorDistancia, AntigoValorDistancia);
                SomarValorTotal(ref valorTotal, TipoValorAjudante, AntigoValorAjudante);
                SomarValorTotal(ref valorTotal, TipoValorEntrega, AntigoValorEntrega);
                SomarValorTotal(ref valorTotal, TipoValorTipoEmbalagem, AntigoValorTipoEmbalagem);
                SomarValorTotal(ref valorTotal, TipoValorModeloReboque, AntigoValorModeloReboque);
                SomarValorTotal(ref valorTotal, TipoValorModeloTracao, AntigoValorModeloTracao);
                SomarValorTotal(ref valorTotal, TipoValorPallets, AntigoValorPallets);
                SomarValorTotal(ref valorTotal, TipoValorPeso, AntigoValorPeso);
                SomarValorTotal(ref valorTotal, TipoValorTempo, AntigoValorTempo);
                SomarValorTotal(ref valorTotal, TipoValorTipoCarga, AntigoValorTipoCarga);

                valorTotal += AntigoValorBase;

                return valorTotal;
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorTotal
        {
            get
            {
                return AntigoValorTotalDecimal.PercentageDifferenceTo(ValorTotalDecimal);
            }
        }

        #endregion Propriedades - Valor Total

        #region Propriedades - Utiliza Contrato Frete Transportador

        public int CodigoContrato { get; set; }

        public string DescricaoContrato { get; set; }

        public decimal PisTransportador { get; set; }

        public decimal PisTransportadorCalculo
        {
            get
            {
                if (ValorTotalDecimal == 0)
                    return 0;
                else
                    return PisTransportador * ValorTotalDecimal / 100;
            }
        }

        public string PisTransportadorFormatado { get => PisTransportadorCalculo.ToString($"n{_numeroCasasDecimais}"); }

        public decimal ValorLiquidoEmbarcador
        {
            get
            {
                return ValorTotalDecimal - PisEmbarcador - CofinsEmbarcador;
            }
        }

        public string ValorLiquidoEmbarcadorFormatado { get => ValorLiquidoEmbarcador.ToString($"n{_numeroCasasDecimais}"); }

        public decimal CofinsEmbarcador
        {
            get
            {
                return ValorTotalDecimal * 0.076m;
            }
        }

        public string CofinsEmbarcadorFormatado { get => CofinsEmbarcador.ToString($"n{_numeroCasasDecimais}"); }

        public decimal PisEmbarcador
        {
            get
            {
                return ValorTotalDecimal * 0.0165m;
            }
        }

        public string PisEmbarcadorFormatado { get => PisEmbarcador.ToString($"n{_numeroCasasDecimais}"); }

        public decimal CofinsTransportador { get; set; }

        public decimal CofinsTransportadorCalculo
        {
            get
            {
                if (ValorTotalDecimal == 0)
                    return 0;
                else
                    return CofinsTransportador * ValorTotalDecimal / 100;
            }
        }

        public string CofinsTransportadorFormatado { get => CofinsTransportadorCalculo.ToString($"n{_numeroCasasDecimais}"); }

        public decimal ValorLiquidoTransportador
        {
            get
            {
                return ValorTotalDecimal - CofinsTransportadorCalculo - PisTransportadorCalculo;
            }
        }

        public string ValorLiquidoTransportadorFormatado { get => ValorLiquidoTransportador.ToString($"n{_numeroCasasDecimais}"); }

        public string CapacidadeOTMPorTabelaFreteCliente { get; set; }
        public string CapacidadeOTMPorModeloVeicularCarga { get; set; }
        public decimal PercentualRotaPorTabelaFreteCliente { get; set; }
        public decimal PercentualRotaPorModeloVeicularCarga { get; set; }
        public int QuantidadeEntregasPorTabelaFreteCliente { get; set; }
        public int QuantidadeEntregasPorModeloVeicularCarga { get; set; }

        public string DescricaoCapacidadeOTM
        {
            get
            {
                bool? capacidadeOTM = (CapacidadeOTMPorModeloVeicularCarga ?? CapacidadeOTMPorTabelaFreteCliente).ToNullableBool();

                return capacidadeOTM?.ObterDescricao() ?? string.Empty;
            }
        }

        public int QuantidadeEntregas
        {
            get
            {
                return (QuantidadeEntregasPorModeloVeicularCarga > 0) ? QuantidadeEntregasPorModeloVeicularCarga : QuantidadeEntregasPorTabelaFreteCliente;
            }
        }

        public decimal PercentualRota
        {
            get
            {
                return (PercentualRotaPorModeloVeicularCarga > 0) ? PercentualRotaPorModeloVeicularCarga : PercentualRotaPorTabelaFreteCliente;
            }
        }

        public string PercentualRotaFormatado
        {
            get
            {
                return PercentualRota.ToString($"n{_numeroCasasDecimais}");
            }
        }

        public PontoPlanejamentoTransporte PontoPlanejamentoTransporte { get; set; }
        public string PontoPlanejamentoTransporteDescricao { get => PontoPlanejamentoTransporte.ObterDescricao(); }

        public string GrupoDaCarga { get { return GrupoDaCargaEnumerado.ObterDescricao(); } }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGrupoCarga GrupoDaCargaEnumerado { get; set; }

        #endregion Propriedades - Utiliza Contrato Frete Transportador

        #region Propriedades - Componentes

        public int CodigoValorComponente1 { get; set; }
        public int CodigoValorComponente2 { get; set; }
        public int CodigoValorComponente3 { get; set; }
        public int CodigoValorComponente4 { get; set; }
        public int CodigoValorComponente5 { get; set; }
        public int CodigoValorComponente6 { get; set; }
        public int CodigoValorComponente7 { get; set; }
        public int CodigoValorComponente8 { get; set; }
        public int CodigoValorComponente9 { get; set; }
        public int CodigoValorComponente10 { get; set; }
        public int CodigoValorComponente11 { get; set; }
        public int CodigoValorComponente12 { get; set; }
        public int CodigoValorComponente13 { get; set; }
        public int CodigoValorComponente14 { get; set; }
        public int CodigoValorComponente15 { get; set; }

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

        public decimal AntigoValorComponente1 { get; set; }
        public decimal AntigoValorComponente2 { get; set; }
        public decimal AntigoValorComponente3 { get; set; }
        public decimal AntigoValorComponente4 { get; set; }
        public decimal AntigoValorComponente5 { get; set; }
        public decimal AntigoValorComponente6 { get; set; }
        public decimal AntigoValorComponente7 { get; set; }
        public decimal AntigoValorComponente8 { get; set; }
        public decimal AntigoValorComponente9 { get; set; }
        public decimal AntigoValorComponente10 { get; set; }
        public decimal AntigoValorComponente11 { get; set; }
        public decimal AntigoValorComponente12 { get; set; }
        public decimal AntigoValorComponente13 { get; set; }
        public decimal AntigoValorComponente14 { get; set; }
        public decimal AntigoValorComponente15 { get; set; }

        public TipoCampoValorTabelaFrete TipoValorComponente1 { get; set; }
        public TipoCampoValorTabelaFrete TipoValorComponente2 { get; set; }
        public TipoCampoValorTabelaFrete TipoValorComponente3 { get; set; }
        public TipoCampoValorTabelaFrete TipoValorComponente4 { get; set; }
        public TipoCampoValorTabelaFrete TipoValorComponente5 { get; set; }
        public TipoCampoValorTabelaFrete TipoValorComponente6 { get; set; }
        public TipoCampoValorTabelaFrete TipoValorComponente7 { get; set; }
        public TipoCampoValorTabelaFrete TipoValorComponente8 { get; set; }
        public TipoCampoValorTabelaFrete TipoValorComponente9 { get; set; }
        public TipoCampoValorTabelaFrete TipoValorComponente10 { get; set; }
        public TipoCampoValorTabelaFrete TipoValorComponente11 { get; set; }
        public TipoCampoValorTabelaFrete TipoValorComponente12 { get; set; }
        public TipoCampoValorTabelaFrete TipoValorComponente13 { get; set; }
        public TipoCampoValorTabelaFrete TipoValorComponente14 { get; set; }
        public TipoCampoValorTabelaFrete TipoValorComponente15 { get; set; }

        public decimal PercentualDiferencaAtualizacaoValorComponente1
        {
            get
            {
                return AntigoValorComponente1.PercentageDifferenceTo(ValorComponente1);
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorComponente2
        {
            get
            {
                return AntigoValorComponente2.PercentageDifferenceTo(ValorComponente2);
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorComponente3
        {
            get
            {
                return AntigoValorComponente3.PercentageDifferenceTo(ValorComponente3);
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorComponente4
        {
            get
            {
                return AntigoValorComponente4.PercentageDifferenceTo(ValorComponente4);
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorComponente5
        {
            get
            {
                return AntigoValorComponente5.PercentageDifferenceTo(ValorComponente5);
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorComponente6
        {
            get
            {
                return AntigoValorComponente6.PercentageDifferenceTo(ValorComponente6);
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorComponente7
        {
            get
            {
                return AntigoValorComponente7.PercentageDifferenceTo(ValorComponente7);
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorComponente8
        {
            get
            {
                return AntigoValorComponente8.PercentageDifferenceTo(ValorComponente8);
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorComponente9
        {
            get
            {
                return AntigoValorComponente9.PercentageDifferenceTo(ValorComponente9);
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorComponente10
        {
            get
            {
                return AntigoValorComponente10.PercentageDifferenceTo(ValorComponente10);
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorComponente11
        {
            get
            {
                return AntigoValorComponente11.PercentageDifferenceTo(ValorComponente11);
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorComponente12
        {
            get
            {
                return AntigoValorComponente12.PercentageDifferenceTo(ValorComponente12);
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorComponente13
        {
            get
            {
                return AntigoValorComponente13.PercentageDifferenceTo(ValorComponente13);
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorComponente14
        {
            get
            {
                return AntigoValorComponente14.PercentageDifferenceTo(ValorComponente14);
            }
        }

        public decimal PercentualDiferencaAtualizacaoValorComponente15
        {
            get
            {
                return AntigoValorComponente15.PercentageDifferenceTo(ValorComponente15);
            }
        }

        public string DescricaoValorComponente1
        {
            get
            {
                if (ValorComponente1 > 0)
                    return _gerandoExcel ? ValorComponente1.ToString(ObterFormatoComponente(TipoValorComponente1)) : ObterTipoValor(TipoValorComponente1, CodigoValorComponente1) + " " + ValorComponente1.ToString(ObterFormatoComponente(TipoValorComponente1));

                return string.Empty;

            }
        }
        public string DescricaoValorComponente2
        {
            get
            {
                if (ValorComponente2 > 0)
                    return _gerandoExcel ? ValorComponente2.ToString(ObterFormatoComponente(TipoValorComponente2)) : ObterTipoValor(TipoValorComponente2, CodigoValorComponente2) + " " + ValorComponente2.ToString(ObterFormatoComponente(TipoValorComponente2));

                return string.Empty;
            }
        }
        public string DescricaoValorComponente3
        {
            get
            {
                if (ValorComponente3 > 0)
                    return _gerandoExcel ? ValorComponente3.ToString(ObterFormatoComponente(TipoValorComponente3)) : ObterTipoValor(TipoValorComponente3, CodigoValorComponente3) + " " + ValorComponente3.ToString(ObterFormatoComponente(TipoValorComponente3));

                return string.Empty;
            }
        }
        public string DescricaoValorComponente4
        {
            get
            {
                if (ValorComponente4 > 0)
                    return _gerandoExcel ? ValorComponente4.ToString(ObterFormatoComponente(TipoValorComponente4)) : ObterTipoValor(TipoValorComponente4, CodigoValorComponente4) + " " + ValorComponente4.ToString(ObterFormatoComponente(TipoValorComponente4));

                return string.Empty;
            }
        }
        public string DescricaoValorComponente5
        {
            get
            {
                if (ValorComponente5 > 0)
                    return _gerandoExcel ? ValorComponente5.ToString(ObterFormatoComponente(TipoValorComponente5)) : ObterTipoValor(TipoValorComponente5, CodigoValorComponente5) + " " + ValorComponente5.ToString(ObterFormatoComponente(TipoValorComponente5));

                return string.Empty;
            }
        }
        public string DescricaoValorComponente6
        {
            get
            {
                if (ValorComponente6 > 0)
                    return _gerandoExcel ? ValorComponente6.ToString(ObterFormatoComponente(TipoValorComponente6)) : ObterTipoValor(TipoValorComponente6, CodigoValorComponente6) + " " + ValorComponente6.ToString(ObterFormatoComponente(TipoValorComponente6));

                return string.Empty;
            }
        }
        public string DescricaoValorComponente7
        {
            get
            {
                if (ValorComponente7 > 0)
                    return _gerandoExcel ? ValorComponente7.ToString(ObterFormatoComponente(TipoValorComponente7)) : ObterTipoValor(TipoValorComponente7, CodigoValorComponente7) + " " + ValorComponente7.ToString(ObterFormatoComponente(TipoValorComponente7));

                return string.Empty;
            }
        }
        public string DescricaoValorComponente8
        {
            get
            {
                if (ValorComponente8 > 0)
                    return _gerandoExcel ? ValorComponente8.ToString(ObterFormatoComponente(TipoValorComponente8)) : ObterTipoValor(TipoValorComponente8, CodigoValorComponente8) + " " + ValorComponente8.ToString(ObterFormatoComponente(TipoValorComponente8));

                return string.Empty;
            }
        }
        public string DescricaoValorComponente9
        {
            get
            {
                if (ValorComponente9 > 0)
                    return _gerandoExcel ? ValorComponente9.ToString(ObterFormatoComponente(TipoValorComponente9)) : ObterTipoValor(TipoValorComponente9, CodigoValorComponente9) + " " + ValorComponente9.ToString(ObterFormatoComponente(TipoValorComponente9));

                return string.Empty;
            }
        }
        public string DescricaoValorComponente10
        {
            get
            {
                if (ValorComponente10 > 0)
                    return _gerandoExcel ? ValorComponente10.ToString(ObterFormatoComponente(TipoValorComponente10)) : ObterTipoValor(TipoValorComponente10, CodigoValorComponente10) + " " + ValorComponente10.ToString(ObterFormatoComponente(TipoValorComponente10));

                return string.Empty;
            }
        }
        public string DescricaoValorComponente11
        {
            get
            {
                if (ValorComponente11 > 0)
                    return _gerandoExcel ? ValorComponente11.ToString(ObterFormatoComponente(TipoValorComponente11)) : ObterTipoValor(TipoValorComponente11, CodigoValorComponente11) + " " + ValorComponente11.ToString(ObterFormatoComponente(TipoValorComponente11));

                return string.Empty;
            }
        }
        public string DescricaoValorComponente12
        {
            get
            {
                if (ValorComponente12 > 0)
                    return _gerandoExcel ? ValorComponente12.ToString(ObterFormatoComponente(TipoValorComponente12)) : ObterTipoValor(TipoValorComponente12, CodigoValorComponente12) + " " + ValorComponente12.ToString(ObterFormatoComponente(TipoValorComponente12));

                return string.Empty;
            }
        }
        public string DescricaoValorComponente13
        {
            get
            {
                if (ValorComponente13 > 0)
                    return _gerandoExcel ? ValorComponente13.ToString(ObterFormatoComponente(TipoValorComponente13)) : ObterTipoValor(TipoValorComponente13, CodigoValorComponente13) + " " + ValorComponente13.ToString(ObterFormatoComponente(TipoValorComponente13));

                return string.Empty;
            }
        }
        public string DescricaoValorComponente14
        {
            get
            {
                if (ValorComponente14 > 0)
                    return _gerandoExcel ? ValorComponente14.ToString(ObterFormatoComponente(TipoValorComponente14)) : ObterTipoValor(TipoValorComponente14, CodigoValorComponente14) + " " + ValorComponente14.ToString(ObterFormatoComponente(TipoValorComponente14));

                return string.Empty;
            }
        }
        public string DescricaoValorComponente15
        {
            get
            {
                if (ValorComponente15 > 0)
                    return _gerandoExcel ? ValorComponente15.ToString(ObterFormatoComponente(TipoValorComponente15)) : ObterTipoValor(TipoValorComponente15, CodigoValorComponente15) + " " + ValorComponente15.ToString(ObterFormatoComponente(TipoValorComponente15));

                return string.Empty;
            }
        }
        public string DescricaoAntigoValorComponente1
        {
            get
            {
                if (AntigoValorComponente1 > 0)
                    return AntigoValorComponente1.ToString(ObterFormatoComponente(TipoValorComponente1));

                return string.Empty;

            }
        }
        public string DescricaoAntigoValorComponente2
        {
            get
            {
                if (AntigoValorComponente2 > 0)
                    return AntigoValorComponente2.ToString(ObterFormatoComponente(TipoValorComponente2));

                return string.Empty;
            }
        }
        public string DescricaoAntigoValorComponente3
        {
            get
            {
                if (AntigoValorComponente3 > 0)
                    return AntigoValorComponente3.ToString(ObterFormatoComponente(TipoValorComponente3));

                return string.Empty;
            }
        }
        public string DescricaoAntigoValorComponente4
        {
            get
            {
                if (AntigoValorComponente4 > 0)
                    return AntigoValorComponente4.ToString(ObterFormatoComponente(TipoValorComponente4));

                return string.Empty;
            }
        }
        public string DescricaoAntigoValorComponente5
        {
            get
            {
                if (AntigoValorComponente5 > 0)
                    return AntigoValorComponente5.ToString(ObterFormatoComponente(TipoValorComponente5));

                return string.Empty;
            }
        }
        public string DescricaoAntigoValorComponente6
        {
            get
            {
                if (AntigoValorComponente6 > 0)
                    return AntigoValorComponente6.ToString(ObterFormatoComponente(TipoValorComponente6));

                return string.Empty;
            }
        }
        public string DescricaoAntigoValorComponente7
        {
            get
            {
                if (AntigoValorComponente7 > 0)
                    return AntigoValorComponente7.ToString(ObterFormatoComponente(TipoValorComponente7));

                return string.Empty;
            }
        }
        public string DescricaoAntigoValorComponente8
        {
            get
            {
                if (AntigoValorComponente8 > 0)
                    return AntigoValorComponente8.ToString(ObterFormatoComponente(TipoValorComponente8));

                return string.Empty;
            }
        }
        public string DescricaoAntigoValorComponente9
        {
            get
            {
                if (AntigoValorComponente9 > 0)
                    return AntigoValorComponente9.ToString(ObterFormatoComponente(TipoValorComponente9));

                return string.Empty;
            }
        }
        public string DescricaoAntigoValorComponente10
        {
            get
            {
                if (AntigoValorComponente10 > 0)
                    return AntigoValorComponente10.ToString(ObterFormatoComponente(TipoValorComponente10));

                return string.Empty;
            }
        }
        public string DescricaoAntigoValorComponente11
        {
            get
            {
                if (AntigoValorComponente11 > 0)
                    return AntigoValorComponente11.ToString(ObterFormatoComponente(TipoValorComponente11));

                return string.Empty;
            }
        }
        public string DescricaoAntigoValorComponente12
        {
            get
            {
                if (AntigoValorComponente12 > 0)
                    return AntigoValorComponente12.ToString(ObterFormatoComponente(TipoValorComponente12));

                return string.Empty;
            }
        }
        public string DescricaoAntigoValorComponente13
        {
            get
            {
                if (AntigoValorComponente13 > 0)
                    return AntigoValorComponente13.ToString(ObterFormatoComponente(TipoValorComponente13));

                return string.Empty;
            }
        }
        public string DescricaoAntigoValorComponente14
        {
            get
            {
                if (AntigoValorComponente14 > 0)
                    return AntigoValorComponente14.ToString(ObterFormatoComponente(TipoValorComponente14));

                return string.Empty;
            }
        }
        public string DescricaoAntigoValorComponente15
        {
            get
            {
                if (AntigoValorComponente15 > 0)
                    return AntigoValorComponente15.ToString(ObterFormatoComponente(TipoValorComponente15));

                return string.Empty;
            }
        }

        #endregion Propriedades - Componentes

        #region Métodos Privados

        private string ObterFormatoComponente(TipoCampoValorTabelaFrete TipoValorComponente)
        {
            return $"n{_numeroCasasDecimais}";
        }

        private string ObterTipoValor(TipoCampoValorTabelaFrete tipoValor, int codigoItem, string info = "")
        {
            if (isCSV)
                return "";

            switch (tipoValor)
            {
                case TipoCampoValorTabelaFrete.AumentoPercentual:
                    return isRelatorio ? "%" : "<span class='spnTipoValorItem' data-codigo-item='" + codigoItem.ToString() + "' data-tipo-valor='" + tipoValor.ToString("d") + "' data-info='" + info + "'>%</span>";
                case TipoCampoValorTabelaFrete.AumentoValor:
                    return isRelatorio ? "+" : "<span class='fa fa-plus spnTipoValorItem' data-codigo-item='" + codigoItem.ToString() + "' data-tipo-valor='" + tipoValor.ToString("d") + "' data-info='" + info + "'></span>";
                case TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal:
                    return isRelatorio ? "% NF" : "<span class='fa fa-barcode spnTipoValorItem' data-codigo-item='" + codigoItem.ToString() + "' data-tipo-valor='" + tipoValor.ToString("d") + "' data-info='" + info + "'></span>";
                case TipoCampoValorTabelaFrete.ValorFixo:
                case TipoCampoValorTabelaFrete.ValorFixoArredondadoParaCima:
                    return isRelatorio ? "$" : "<span class='fa fa-dollar spnTipoValorItem' data-codigo-item='" + codigoItem.ToString() + "' data-tipo-valor='" + tipoValor.ToString("d") + "' data-info='" + info + "' aaaaaa></span>";
                default:
                    return string.Empty;
            }
        }

        private void SomarValorTotal(ref decimal valorTotal, TipoCampoValorTabelaFrete tipoValor, decimal valorSomar)
        {
            if (tipoValor.IsPermiteSomar())
                valorTotal += valorSomar;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void SetGerandoExcel(bool gerandoExcel)
        {
            _gerandoExcel = gerandoExcel;
        }

        public void SetNumeroCasasDecimais(int numeroCasaDecimais)
        {
            _numeroCasasDecimais = numeroCasaDecimais;
        }

        #endregion Métodos Públicos
    }
}
