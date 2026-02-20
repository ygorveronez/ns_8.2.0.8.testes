/// <autosync enabled="true" />
/// <reference path="../../../../wwwroot/js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.blockui.js" />
/// <reference path="../../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.globalize.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Global/SignalR/SignalR.js" />
/// <reference path="../../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../DadosEmissao/Configuracao.js" />
/// <reference path="../DadosEmissao/DadosEmissao.js" />
/// <reference path="../DadosEmissao/Geral.js" />
/// <reference path="../DadosEmissao/Lacre.js" />
/// <reference path="../DadosEmissao/LocaisPrestacao.js" />
/// <reference path="../DadosEmissao/Observacao.js" />
/// <reference path="../DadosEmissao/Passagem.js" />
/// <reference path="../DadosEmissao/Percurso.js" />
/// <reference path="../DadosEmissao/Rota.js" />
/// <reference path="../DadosEmissao/Seguro.js" />
/// <reference path="../DadosTransporte/DadosTransporte.js" />
/// <reference path="../DadosTransporte/Motorista.js" />
/// <reference path="../DadosTransporte/Ajudante.js" />
/// <reference path="../DadosTransporte/Tipo.js" />
/// <reference path="../DadosTransporte/Transportador.js" />
/// <reference path="../DadosTransporte/LiberacaoSemIntegracaoGR.js" />
/// <reference path="../DadosTransporte/IntegracaoCargaTransportador.js" />
/// <reference path="../DadosTransporte/VeiculosHUB.js" />
/// <reference path="../Documentos/CTe.js" />
/// <reference path="../Mercante/EtapaMercante.js" />
/// <reference path="../Documentos/MDFe.js" />
/// <reference path="../Documentos/MDFeAquaviario.js" />
/// <reference path="../SVM/SVM.js" />
/// <reference path="../Documentos/NFS.js" />
/// <reference path="../Documentos/PreCTe.js" />
/// <reference path="../DocumentosEmissao/CargaPedidoDocumentoCTe.js" />
/// <reference path="../DocumentosEmissao/EtapaContainer.js" />
/// <reference path="../DocumentosEmissao/ConsultaReceita.js" />
/// <reference path="../DocumentosEmissao/CTe.js" />
/// <reference path="../DocumentosEmissao/Documentos.js" />
/// <reference path="../DocumentosEmissao/DropZone.js" />
/// <reference path="../DocumentosEmissao/EtapaDocumentos.js" />
/// <reference path="../DocumentosEmissao/CargaColetaContainer.js" />
/// <reference path="../DocumentosEmissao/NotaFiscal.js" />
/// <reference path="../Faturamento/EtapaFaturamento.js" />
/// <reference path="../IntegracaoFaturamento/EtapaIntegracaoFaturamento.js" />
/// <reference path="../Frete/Complemento.js" />
/// <reference path="../Frete/Componente.js" />
/// <reference path="../Frete/EtapaFrete.js" />
/// <reference path="../Frete/Frete.js" />
/// <reference path="../Frete/SemTabela.js" />
/// <reference path="../Frete/TabelaCliente.js" />
/// <reference path="../Frete/TabelaComissao.js" />
/// <reference path="../Frete/TabelaRota.js" />
/// <reference path="../Frete/TabelaSubContratacao.js" />
/// <reference path="../Frete/TabelaTerceiros.js" />
/// <reference path="../Impressao/Impressao.js" />
/// <reference path="../Integracao/Integracao.js" />
/// <reference path="../Integracao/IntegracaoCarga.js" />
/// <reference path="../Integracao/IntegracaoCTe.js" />
/// <reference path="../Integracao/IntegracaoEDI.js" />
/// <reference path="../Terceiro/ContratoFrete.js" />
/// <reference path="DataCarregamento.js" />
/// <reference path="DataRetornoCD.js" />
/// <reference path="Leilao.js" />
/// <reference path="Operador.js" />
/// <reference path="SignalR.js" />
/// <reference path="Auditoria.js" />
/// <reference path="../../../Configuracao/Sistema/OperadorLogistica.js" />
/// <reference path="../../../Consultas/ApoliceSeguro.js" />
/// <reference path="../../../Consultas/Tranportador.js" />
/// <reference path="../../../Consultas/Localidade.js" />
/// <reference path="../../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/Motorista.js" />
/// <reference path="../../../Consultas/Veiculo.js" />
/// <reference path="../../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../Consultas/TipoContainer.js" />
/// <reference path="../../../Consultas/TipoOperacao.js" />
/// <reference path="../../../Consultas/Filial.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/Usuario.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/RotaFrete.js" />
/// <reference path="../../../Consultas/CanalEntrega.js" />
/// <reference path="../../../Consultas/CIOT.js" />
/// <reference path="../../../Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../../Consultas/CentroResultado.js" />
/// <reference path="../../../Consultas/Regiao.js" />
/// <reference path="../../../localidades/mesoregiao/mesoregiao.js" />
/// <reference path="../../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../Enumeradores/EnumEtapaCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoCargaMercante.js" />
/// <reference path="../../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="../../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="../../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoContratacaoCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../../Enumeradores/EnumTipoEmissaoCTeParticipantes.js" />
/// <reference path="../../../Enumeradores/EnumTipoMensagemAlerta.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoIntegracao.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoImpressaoDiarioBordo.js" />
/// <reference path="../../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../Enumeradores/EnumTipoServicoCarga.js" />
/// <reference path="../../../Enumeradores/EnumTipoPagamentoValePedagio.js" />
/// <reference path="../../../Enumeradores/EnumTipoAreaVeiculo.js" />
/// <reference path="../../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="../../../Logistica/Tracking/Tracking.lib.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _cargaAtual; //seta temporariamente a carga para alteração
var _carga;
var _pesquisaCarga;
var _listaKnoutsCarga;
var _consultaCargaIntegracaoEmbarcador;
var _ehMultiEmbarcador = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador;
var _ehTransportador = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe;
var _gridCarga;
var _configuracoesIntegracaoCarga;
var _knoutAlterarTipoPagamentoValePedagio;
var _gridPlacaCarregamento;
var PesquisaCarga = function () {
    this.Codigo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDaCarga.getFieldDescription(), visible: ko.observable(false) });
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDaCarga.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.CodigoPedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroPedidoEmbarcador, val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.NumeroPedidoNFe = PropertyEntity({ visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.NumeroPedidoNFe.getFieldDescription() });
    this.NumeroBooking = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoBooking.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Serie = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Serie.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroOS = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDaOrdemDeServico.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroControle = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroControleCte.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.PedidoViagemNavio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.NavioViagemDirecao.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.PedidoEmpresaResponsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.EmpresaResponsavel.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.PedidoCentroCusto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.CentroDeCusto.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Container.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable((_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) ? true : false) });
    this.ContainerTMS = PropertyEntity({ type: types.entity, idBtnSearch: guid(), text: Localization.Resources.Cargas.Carga.Container.getFieldDescription(), codEntity: ko.observable(0), visible: ko.observable((_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) ? true : false) });
    this.DataInicioAverbacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataInicioAverbacao.getFieldDescription(), getType: typesKnockout.date });
    this.DataFimAverbacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataLimiteAverbacao.getFieldDescription(), dateRangeInit: this.DataInicioAverbacao, getType: typesKnockout.date });
    this.DataInicioEmissao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataInicioEmissao.getFieldDescription(), getType: typesKnockout.date });
    this.DataFimEmissao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataLimiteEmissao.getFieldDescription(), dateRangeInit: this.DataInicioEmissao, getType: typesKnockout.date });
    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.PortoOrigem.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.PortoDestino.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CargaPerigosa = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: Localization.Resources.Cargas.Carga.SomenteCargasPerigosas, visible: ko.observable(true) });
    this.CargaTrocaDeNota = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: Localization.Resources.Cargas.Carga.SomenteCargasComTrocaDeNota, visible: ko.observable(true) });
    this.RaizCNPJ = PropertyEntity({ text: Localization.Resources.Cargas.Carga.RaizCnpjTomador.getFieldDescription(), val: ko.observable("") });
    this.FormaIntegracaoNotas = PropertyEntity({ text: Localization.Resources.Cargas.Carga.FormaIntegracaoNotas.getFieldDescription(), options: EnumFormaIntegracao.obterOpcoesPesquisa(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroPedido = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoPedido.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.NumeroPedidoCliente = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoPedidoCliente.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.NumeroPedidoTrocado = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoPedidoTrocado.getFieldDescription(), visible: _CONFIGURACAO_TMS.PermitirTrocarPedidoCarga });
    this.NumeroCTe = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoCte.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.NumeroCTeSubcontratacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoCteSubcontratacao.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.NumeroMDFe = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoManifesto.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.NumeroNF = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDaNotaFiscal.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.Empresa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.Carga.Transportador.getFieldDescription()), idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Veiculo.getFieldDescription(), issue: 143, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Motorista.getFieldDescription(), issue: 145, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Operador.getFieldDescription(), issue: 602, idBtnSearch: guid(), visible: false, cssSemClearClass: "" });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), required: false, text: Localization.Resources.Cargas.Carga.TipoDeOperacao.getFieldDescription(), issue: 121, visible: ko.observable(true), idBtnSearch: guid(), cssClass: ko.observable("col col-sm-3 col-md-3 col-lg-3"), tipoImpressaoDiarioBordo: EnumTipoImpressaoDiarioBordo.Nenhum });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Origem.getFieldDescription(), issue: 16, idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Destino.getFieldDescription(), issue: 16, idBtnSearch: guid() });
    this.EstadoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.EstadoDeOrigem.getFieldDescription(), issue: 0, idBtnSearch: guid() });
    this.EstadoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.EstadoDeDestino.getFieldDescription(), issue: 0, idBtnSearch: guid() });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.ModeloVeicularDaCarga.getFieldDescription(), issue: 44, required: true, idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.TipoDeCarga.getFieldDescription(), issue: 53, required: true, idBtnSearch: guid() });

    this.Rota = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Rota.getFieldDescription(), required: false, idBtnSearch: guid() });

    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Filial.getFieldDescription(), issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });
    this.FilialVenda = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.FilialVenda.getFieldDescription(), issue: 71, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Carregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Carregamento.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.GrupoDeClientes.getFieldDescription(), issue: 58, idBtnSearch: guid(), visible: ko.observable(false) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Remetente.getFieldDescription(), issue: 52, idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Destinatario.getFieldDescription(), issue: 52, idBtnSearch: guid() });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Expedidor.getFieldDescription(), issue: 52, idBtnSearch: guid() });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Recebedor.getFieldDescription(), issue: 52, idBtnSearch: guid() });
    this.DataInicio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataInicio.getFieldDescription(), getType: _CONFIGURACAO_TMS.HabilitarHoraFiltroDataInicialFinalRelatorioCargas ? typesKnockout.dateTime : typesKnockout.date, val: ko.observable(_CONFIGURACAO_TMS.ConfiguracaoPaginacaoDataLimite ?? "") });
    this.DataFim = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataLimite.getFieldDescription(), dateRangeInit: this.DataInicio, getType: _CONFIGURACAO_TMS.HabilitarHoraFiltroDataInicialFinalRelatorioCargas ? typesKnockout.dateTime : typesKnockout.date, val: ko.observable("") });
    this.DataCarregamentoInicio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataInicioCarregamento.getFieldDescription(), getType: typesKnockout.date });
    this.DataCarregamentoFim = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataLimiteCarregamento.getFieldDescription(), dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.SomenteTerceiros = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: Localization.Resources.Cargas.Carga.SomenteCargasDeTerceiros, issue: 56, visible: ko.observable(false) });
    this.CargaRelacionadas = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: Localization.Resources.Cargas.Carga.CargaRelacionadas, visible: ko.observable(true) });
    this.CargasDoRedespacho = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: Localization.Resources.Cargas.Carga.BuscarCargasDoRedespacho });
    this.SomenteAgrupadas = PropertyEntity({ type: types.bool, val: ko.observable(false), visible: ko.observable(true), def: false, text: Localization.Resources.Cargas.Carga.SomenteCargasQueForamAgrupadas });
    this.SomenteCargasReentrega = PropertyEntity({ type: types.bool, val: ko.observable(false), visible: ko.observable(true), def: false, text: Localization.Resources.Cargas.Carga.SomenteCargasDeReentrega });
    this.TipoContratacaoCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoDeServicoDeCarga.getFieldDescription(), options: EnumTipoContratacaoCarga.ObterOpcoesPesquisa(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.CanalEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.CanalEntrega.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.FuncionarioVendedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Vendedor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CargaIntegracaoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.CargasDoEmbarcador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.DeliveryTerm = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DeliveryTerm.getFieldDescription(), maxlength: 150 });
    this.IdAutorizacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.IdDeAutorizacao.getFieldDescription(), maxlength: 150 });
    this.NumeroTransporte = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroTransporte.getFieldDescription(), maxlength: 150, visible: _ehMultiEmbarcador });
    this.Ordem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Ordem.getFieldDescription(), maxlength: 50 });
    this.DataInclusaoBookingInicial = PropertyEntity({ text: Localization.Resources.Cargas.Carga.InclusaoBookingInicio.getFieldDescription(), getType: typesKnockout.date });
    this.DataInclusaoBookingLimite = PropertyEntity({ text: Localization.Resources.Cargas.Carga.InclusaoBookingLimite.getFieldDescription(), dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInclusaoPCPInicial = PropertyEntity({ text: Localization.Resources.Cargas.Carga.InclusaoPcpInicio.getFieldDescription(), getType: typesKnockout.date });
    this.DataInclusaoPCPLimite = PropertyEntity({ text: Localization.Resources.Cargas.Carga.InclusaoPcpLimite.getFieldDescription(), dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.NumeroEXP = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroExp.getFieldDescription(), maxlength: 150 });
    this.ObservacaoCTe = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ObservacaoPedidoCte.getFieldDescription(), maxlength: 150 });
    this.TransportadorTerceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Terceiro.getFieldDescription(), issue: 56, idBtnSearch: guid() });
    this.RotaEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.RotaEmbarcador.getFieldDescription(), maxlength: 150 });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.ModeloDeDocumentoFiscal.getFieldDescription(), idBtnSearch: guid() });
    this.CargasAguardandoImportacaoCTe = PropertyEntity({ text: Localization.Resources.Cargas.Carga.CargasAguardandoImportacaoDeCte.getFieldDescription(), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.OperadorInsercao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.OperadorInsercao.getFieldDescription(), idBtnSearch: guid(), visible: true });
    this.CIOT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.CIOT.getFieldDescription(), idBtnSearch: guid(), visible: true });
    this.NumeroOe = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroOrdemEmbarque.getFieldDescription(), val: ko.observable(""), getType: typesKnockout.string });
    this.CodigoPedidoCliente = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoPedidoNoCliente.getFieldDescription(), val: ko.observable(""), getType: typesKnockout.string });
    this.SomenteCargasComValePedagio = PropertyEntity({ type: types.bool, val: ko.observable(false), visible: ko.observable(true), def: false, text: Localization.Resources.Cargas.Carga.SomenteCargasComValePedagio });
    this.PossuiPendencia = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PossuiPendencia.getFieldDescription(), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.SomenteCargasComFacturaFake = PropertyEntity({ type: types.bool, val: ko.observable(false), visible: ko.observable((_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) ? true : false), def: false, text: Localization.Resources.Cargas.Carga.SomenteCargasComFacturaFake });
    this.SomenteCargasComDocumentoOriginarioVinculado = PropertyEntity({ type: types.bool, val: ko.observable(false), visible: ko.observable(true), def: false, text: Localization.Resources.Cargas.Carga.SomenteCargasComDocumentoOriginarioVinculado });
    this.CargaTrechoSumarizada = PropertyEntity({ text: Localization.Resources.Cargas.Carga.CargaDeTrecho.getFieldDescription(), options: EnumCargaTrechoSumarizada.obterOpcoesPesquisa(), val: ko.observable(_CONFIGURACAO_TMS.PossuiTipoOperacaoConsolidacao ? EnumCargaTrechoSumarizada.Agrupadora : ""), def: "", issue: 0, visible: ko.observable(_CONFIGURACAO_TMS.PossuiTipoOperacaoConsolidacao) });
    this.NumeroOT = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroOT.getFieldDescription(), type: types.string, maxlength: 120, val: ko.observable(""), visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), required: false, text: Localization.Resources.Cargas.Carga.CentroDeResultado.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable((_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)) });
    this.CategoriaOS = PropertyEntity({ getType: typesKnockout.selectMultiple, text: Localization.Resources.Cargas.Carga.CategoriaOS.getFieldDescription(), options: EnumCategoriaOS.obterOpcoesPesquisa(), val: ko.observable([]), def: [], visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) });
    this.TipoOSConvertido = PropertyEntity({ getType: typesKnockout.selectMultiple, text: Localization.Resources.Cargas.Carga.TipoOSConvertido.getFieldDescription(), options: EnumTipoOSConvertido.obterOpcoesPesquisa(), val: ko.observable([]), def: [], visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) });
    this.TipoOS = PropertyEntity({ getType: typesKnockout.selectMultiple, text: Localization.Resources.Cargas.Carga.TipoOS.getFieldDescription(), options: EnumTipoOS.obterOpcoesPesquisa(), val: ko.observable([]), def: [], visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) });
    this.DirecionamentoCustoExtra = PropertyEntity({ getType: typesKnockout.selectMultiple, text: Localization.Resources.Cargas.Carga.DirecionamentoCustoExtra.getFieldDescription(), options: EnumDirecionamentoCustoExtra.obterOpcoesPesquisa(), val: ko.observable([]), def: [], visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadeProjetoNFTP) });
    this.StatusCustoExtra = PropertyEntity({ getType: typesKnockout.selectMultiple, text: Localization.Resources.Cargas.Carga.StatusCustoExtra.getFieldDescription(), options: EnumStatusCustoExtra.obterOpcoesPesquisa(), val: ko.observable([]), def: [], visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadeProjetoNFTP) });
    this.SomenteCargasNaoValidadasNaGR = PropertyEntity({ type: types.bool, val: ko.observable(false), visible: ko.observable(true), def: false, text: Localization.Resources.Cargas.Carga.SomenteCargasNaoValidadasNaGR });
    this.SomenteCargasCriticas = PropertyEntity({ type: types.bool, val: ko.observable(false), visible: ko.observable(true), def: false, text: Localization.Resources.Cargas.Carga.SomenteCargasCriticas });
    this.SomenteCargasSemCIOT = PropertyEntity({ type: types.bool, val: ko.observable(false), visible: ko.observable(true), def: false, text: Localization.Resources.Cargas.Carga.SomenteCargasSemCIOT });
    this.CanalVenda = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.CanalVenda.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoCobrancaMultimodal = PropertyEntity({ getType: typesKnockout.selectMultiple, text: Localization.Resources.Cargas.Carga.TipoCobrancaMultimodal.getFieldDescription(), options: EnumTipoCobrancaMultimodal.obterOpcoes(), val: ko.observable([]), def: [], visible: ko.observable(true) });
    this.PossuiValePedagio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PossuiValePedagio.getFieldDescription(), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.Mesorregiao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Mesorregiao.getFieldDescription(), issue: 71, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Regiao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Regiao.getFieldDescription(), issue: 71, idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroPreCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroPreCarga.getFieldDescription(), maxlength: 50 });
    this.NumeroContainerVeiculo = PropertyEntity({ text: "Container Veículo: ", maxlength: 50 });

    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.DataInicioEmissao.dateRangeLimit = this.DataFimEmissao;
    this.DataFimEmissao.dateRangeInit = this.DataInicioEmissao;
    this.DataInicioAverbacao.dateRangeLimit = this.DataFimAverbacao;
    this.DataFimAverbacao.dateRangeInit = this.DataInicioAverbacao;
    this.DataInclusaoBookingInicial.dateRangeLimit = this.DataInclusaoBookingLimite;
    this.DataInclusaoBookingLimite.dateRangeInit = this.DataInclusaoBookingInicial;
    this.DataInclusaoPCPInicial.dateRangeLimit = this.DataInclusaoPCPLimite;
    this.DataInclusaoPCPLimite.dateRangeInit = this.DataInclusaoPCPInicial;

    this.DataCarregamentoInicio.dateRangeLimit = this.DataCarregamentoFim;
    this.DataCarregamentoFim.dateRangeInit = this.DataCarregamentoInicio;

    this.AtualizarCargas = PropertyEntity({
        eventClick: atualizarCargasClick, type: types.event, text: Localization.Resources.Cargas.Carga.CargasNovas, idGrid: guid(), visible: ko.observable(true)
    });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            buscarCargas(1, false);
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.LimparFiltros = PropertyEntity({
        eventClick: function (e) {
            limparFiltrosConsultaCarga();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.LimparFiltros, idGrid: guid(), visible: ko.observable(true)
    });


    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Cargas.Carga.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    var opcoesSituacaoCarga = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS ? EnumSituacoesCarga.obterOpcoesPesquisaTMSMultiplas() : EnumSituacoesCarga.obterOpcoesPesquisaMultiplas();

    if (_operadorLogistica.OperadorSupervisor) {
        this.Operador.visible = true;
        opcoesSituacaoCarga = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador ? EnumSituacoesCarga.obterOpcoesPesquisaSupervisorMultiplas() : EnumSituacoesCarga.obterOpcoesPesquisaTMSMultiplas();
    } else {
        this.Operador.cssSemClearClass = "clearfix visible-md visible-sm";
        this.TipoOperacao.cssClass("col col-sm-6 col-md-6 col-lg-6");
    }

    this.Situacoes = PropertyEntity({ text: Localization.Resources.Cargas.Carga.SituacaoDaCarga.getFieldDescription(), val: ko.observable(EnumSituacoesCarga.NaLogistica), options: opcoesSituacaoCarga, def: EnumSituacoesCarga.NaLogistica, required: true, issue: 533, visible: ko.observable(true), getType: typesKnockout.selectMultiple });
    this.SituacaoCargaMercante = PropertyEntity({ text: Localization.Resources.Cargas.Carga.SituacaoDaCarga.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumSituacaoCargaMercante.obterOpcoes(), def: [], visible: ko.observable(false) });

    if (_CONFIGURACAO_TMS.DiasAnterioresPesquisaCarga > 0) {
        this.DataInicio.val(Global.Data(EnumTipoOperacaoDate.Subtract, _CONFIGURACAO_TMS.DiasAnterioresPesquisaCarga, EnumTipoOperacaoObjetoDate.Days));
        this.Situacoes.val(EnumSituacoesCarga.Todas);
    }

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.OrdenacaoAsc = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DirecaoOrdenacao = PropertyEntity({
        eventClick: function (e) {
            if (e.OrdenacaoAsc.val()) {
                e.OrdenacaoAsc.val(false);
                e.DirecaoOrdenacao.icon("fal fa-arrow-down");
            } else {
                e.OrdenacaoAsc.val(true);
                e.DirecaoOrdenacao.icon("fal fa-arrow-up");
            }

            buscarCargas(1, false);
        }, type: types.event, text: "", idFade: guid(), icon: ko.observable("fal fa-arrow-down")
    });

    this.DownloadDocumentosCarga = PropertyEntity({
        eventClick: DownloadDocumentosCargaClick, type: types.event, text: Localization.Resources.Cargas.Carga.DownloadPdfDocumentosDasCargas, visible: ko.observable(false)
    });

    this.BuscarCargasIntegracaoEmbarcador = PropertyEntity({
        eventClick: null, type: types.event, text: Localization.Resources.Cargas.Carga.BuscarCargasDoEmbarcador, visible: ko.observable(false)
    });

    this.ConsultarCargasIntegracaoEmbarcador = PropertyEntity({
        eventClick: AbrirConsultaCargasIntegracaoEmbarcadorClick, type: types.event, text: Localization.Resources.Cargas.Carga.CargasImportadasDoEmbarcador, visible: ko.observable(false)
    });

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        this.Filial.visible(false);
        this.FilialVenda.visible(false);
        this.Carregamento.visible(false);
        this.GrupoPessoa.visible(true);
        this.Empresa.text(Localization.Resources.Cargas.Carga.EmpresaFilial.getFieldDescription());
        this.SomenteTerceiros.visible(true);
        this.SomenteAgrupadas.visible(false);
        this.Situacoes.val([EnumSituacoesCarga.NaLogistica]);

    }

    if (_CONFIGURACAO_TMS.ImportarCargasMultiEmbarcador) {
        this.ConsultarCargasIntegracaoEmbarcador.visible(true);
        //    this.BuscarCargasIntegracaoEmbarcador.visible(true);
    }

    if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        $("#divPesquisaMultimodal").hide();
        $("#divPesquisaMultimodalAvancada").hide();
    }
    else {
        this.DownloadDocumentosCarga.visible(false);
    }

    this.VincularVeiculoMotorista = PropertyEntity({
        type: types.local,
        text: "Vincular Dados Transporte",
        visible: ko.observable(false),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default waves-effect waves-themed ms-2",
        UrlImportacao: "Carga/ImportarVeiculoMotoristas",
        UrlConfiguracao: "Carga/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O050_ImportacaoCarga,
        CallbackImportacao: function () {
            //_gridCarga.CarregarGrid();
        }
    });

};

var Carga = function () {
    var self = this;

    //****DADOS CARGA ***
    this.TarjaCancelada = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Cancelada });
    this.TarjaAnulada = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Anulada });
    this.TipoDeCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoDeCarga.getFieldDescription() });
    this.InformarOperador = PropertyEntity({ text: Localization.Resources.Cargas.InformarOperador });
    this.Placas = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Placas.getFieldDescription() });
    this.Motoristas = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Motoristas.getFieldDescription() });
    this.EmpresaResponsavel = PropertyEntity({ text: Localization.Resources.Cargas.Carga.EmpresaResponsavel.getFieldDescription() });
    this.QuantidadeDeVolumesReal = PropertyEntity({ text: Localization.Resources.Cargas.Carga.QuantidadeDeVolumesReal.getFieldDescription() });
    this.SeparacaoMercadoria = PropertyEntity({ text: Localization.Resources.Cargas.Carga.SeparacaoDeMercadoria.getFieldDescription() });
    this.MensagemRejeicaoCancelamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.MotivoRejeicaoCancelamento.getFieldDescription() });
    this.CodigoDeCargaOrigemCancelada = PropertyEntity({ text: Localization.Resources.Cargas.Carga.EstaCargaUmaCargaOriginadaDaCargaCanceladaNumero.getFieldDescription() });
    this.ValoresFrete = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ValoresDeFrete });
    this.FreteFilialEmissora = PropertyEntity({ text: Localization.Resources.Cargas.Carga.FreteFilialEmissora });
    this.ComplementosFrete = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ComplementosDeFrete });
    this.ComponentesFrete = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ComponentesDoFrete });
    this.ValorPedagio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ValorDePedagio });
    this.FreteTerceiro = PropertyEntity({ text: Localization.Resources.Cargas.Carga.FreteDoTerceiro });
    this.SolicitacaoFrete = PropertyEntity({ text: Localization.Resources.Cargas.Carga.SolicitacaoDeFrete });
    this.Aprovacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Aprovacao });
    this.Informacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Informacao });
    this.NaoFoiPossivelRoteirizarCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NaoFoiPossivelRoteirizarCarga });
    this.CalculandoValoresFrete = PropertyEntity({ text: Localization.Resources.Cargas.Carga.AguardeCalculandoOsValoresDoFrete });
    this.ConsultandoValoresPedagio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.AguardeConsultandoOsValoresDoPedagio });
    this.AguardandoCargaSegundoTrecho = PropertyEntity({ text: Localization.Resources.Cargas.Carga.AguardandoCargaDeSegundoTrechoSerCriadaParaCalcularFrete });
    this.CtesEstaoSendoGerado = PropertyEntity({ text: Localization.Resources.Cargas.Carga.OsCtesEstaoSendoGerados });
    this.ImpressoesCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ImpressoesDaCarga });
    this.Transportador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Transportador });
    this.MdfesEmEncerramento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.MdfesEmEncerramento });
    this.Integracao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Integracao });
    this.DownloadDocumentos = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DownloadDosDocumentos, visible: ko.observable(true) });
    this.EmpresaEtapaUm = PropertyEntity({ visible: ko.observable(false) });

    this.IntegracaoFrete = PropertyEntity({ text: "Integração" });

    this.ScrollTo = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool, idGrid: guid() });

    this.QuantidadeDocumentosGerados = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, idGrid: guid() });
    this.QuantidadeDocumentosTotal = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, idGrid: guid() });

    this.TipoServicoCarga = PropertyEntity({ val: ko.observable(EnumTipoServicoCarga.NaoInformado), def: EnumTipoServicoCarga.NaoInformado });
    this.NaoPermitirAcessarDocumentosAntesCargaEmTransporte = PropertyEntity({ val: ko.observable(false), def: false });
    this.PossuiMontagemContainer = PropertyEntity({ val: ko.observable(false), def: false });
    this.ObrigatorioVincularContainerCarga = PropertyEntity({ val: ko.observable(false), def: false });
    this.PermiteInformarQuantidadePaletes = PropertyEntity({ val: ko.observable(false), def: false });
    this.NumeroPedidoEmbarcador = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.NumeroDoPedidoNoEmbarcador.getFieldDescription() });
    this.NumeroPedido = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.NumeroDoPedido.getFieldDescription() });
    this.NumeroPedidoCliente = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.NumeroDoPedidoCliente.getFieldDescription() });

    this.TipoPagamentoValePedagio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoPagamentoValePedagio, val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.DescricaoTipoPagamentoValePedagio = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.AlterarTipoPagamentoValePedagio = PropertyEntity({ eventClick: alterarTipoPagamentoValePedagioClick, visible: ko.observable(true), type: types.event, text: Localization.Resources.Gerais.Geral.Alterar });

    //this.NumeroPedidoNFe = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.NumeroPedidoNFe.getFieldDescription() });
    this.Descricao = PropertyEntity({ type: types.local });
    this.Origem = PropertyEntity({ type: types.local, text: Localization.Resources.Cargas.Carga.Origem.getFieldDescription() });
    this.Destino = PropertyEntity({ type: types.local, text: Localization.Resources.Cargas.Carga.Destino.getFieldDescription() });
    this.Km = PropertyEntity({ type: types.local, text: "Km", visible: ko.observable(false) });
    this.Filial = PropertyEntity({ type: types.local, text: Localization.Resources.Cargas.Carga.Filial.getFieldDescription(), visible: (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.FilialVenda = PropertyEntity({ type: types.local, text: Localization.Resources.Cargas.Carga.FilialVenda.getFieldDescription(), visible: ko.observable(true) });
    this.DataCriacaoCarga = PropertyEntity({ type: types.local, text: Localization.Resources.Cargas.Carga.DataCriacao.getFieldDescription() });
    this.NumeroCargaOriginais = PropertyEntity({ type: types.local, visible: ko.observable(false), tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.AgrupamentoDasCargas.getFieldDescription()) });//this.NumeroCargaOriginais = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.VerDetalhesCargaAgrupada = PropertyEntity({ eventClick: verDetalhesCargaAgrupadaClick, type: types.event, text: Localization.Resources.Gerais.Geral.VerDetalhes, visible: ko.observable(false) });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DescricaoCarga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.CargaTrechoSumarizada = PropertyEntity({ text: Localization.Resources.Cargas.Carga.CargaDeTrecho.getFieldDescription(), val: ko.observable(""), def: "" });
    this.FalhaIntegracaoHUB = PropertyEntity({ text: Localization.Resources.Cargas.Carga.FalhaIntegracaoHUB.getFieldDescription(), visible: ko.observable(false), eventClick: ClickRedirecionarHUBOfertas });
    this.TagEnviadoHUBOfertas = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TagEnviadoHUBOfertas.getFieldDescription(), visible: ko.observable(false) });
    this.CodigoJanelaCarregamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoJanelaDescarregamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EscolherHorarioCarregamentoPorLista = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(!_CONFIGURACAO_TMS.PermitirSalvarApenasTransportadorEtapaUmCarga), text: ko.observable(Localization.Resources.Cargas.Carga.Transportador.getRequiredFieldDescription()), issue: 69, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true), tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.Transportador.getFieldDescription()), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-6 col-lg-6") });
    this.RaizCNPJEmpresa = PropertyEntity({});
    this.ValorFrete = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ValorFrete.getFieldDescription(), required: false, idGrid: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), required: true, text: Localization.Resources.Cargas.Carga.Motorista.getRequiredFieldDescription(), issue: 145, idBtnSearch: guid(), motoristas: ko.observable("") });
    this.Monitoramento = PropertyEntity({ type: types.string, val: ko.observable(""), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.Monitoramento.getFieldDescription() });
    this.UltimaPosicaoRastreador = PropertyEntity({ type: types.bool, val: ko.observable(false) });
    this.RastreadorOnlineOffline = PropertyEntity({ visible: ko.observable(true) });
    this.StatusMonitoramento = PropertyEntity({ type: types.string, val: ko.observable(""), visible: ko.observable(true),
        eventClick: function (e) {
                sessionStorage.setItem('codigoCarga', this.CodigoCargaEmbarcador.val());
                window.open("#Logistica/MonitoramentoNovo", '_blank');
        }
    });
    this.ValidarMotoristaTelerisco = PropertyEntity({ type: types.string, val: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoTelerisco) });
    this.ValidarIntegracaoPlacaBRK = PropertyEntity({ type: types.string, val: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoBRKVeiculoEMotorista) });
    this.ValidarMotoristaBRK = PropertyEntity({ type: types.string, val: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoBRKVeiculoEMotorista) });
    this.MotoristaValidadoBrasilRisk = PropertyEntity({ type: types.bool, val: ko.observable(false), visible: ko.observable(false) });
    this.PlacaValidadoBrasilRisk = PropertyEntity({ type: types.bool, val: ko.observable(false), visible: ko.observable(false) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), visible: ko.observable(true), required: false, text: ko.observable(Localization.Resources.Cargas.Carga.Veiculo.getRequiredFieldDescription()), issue: 143, idBtnSearch: guid(), placas: ko.observable(""), cssClass: ko.observable("col col-12"), popover: "<strong>" + Localization.Resources.Cargas.Carga.CliqueAquiPraVisualizarOsDetalhes + "</strong>", detalhesClick: exibirDetalhesVeiculoClick, verDetalhesVisible: ko.observable(false), verDetalhesText: Localization.Resources.Cargas.Carga.VerDetalhes });
    this.Frota = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.Frota.getFieldDescription() });
    this.EstaEmParqueamento = PropertyEntity({ visible: ko.observable(false), val: ko.observable(false) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: true, text: Localization.Resources.Cargas.Carga.TipoDeCarga.getRequiredFieldDescription(), issue: 53, idBtnSearch: guid() });
    this.ObservacaoCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ObservacaoCarga.getFieldDescription(), val: ko.observable(""), visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) });
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, visible: ko.observable(true), enable: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum), text: ko.observable(Localization.Resources.Cargas.Carga.Setor.getRequiredFieldDescription()), issue: 44, tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.Setor.getFieldDescription()), idBtnSearch: guid() });
    this.TipoContainer = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: false, text: Localization.Resources.Cargas.Carga.TipoDeContainer.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, enable: ko.observable(true), text: ko.observable(Localization.Resources.Cargas.Carga.TipoDeOperacao.getFieldDescription()), issue: 121, visible: ko.observable(true), idBtnSearch: guid(), idExtra: guid() });
    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Container.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.GrupoDePessoaPrincipal.getFieldDescription(), visible: ko.observable(false), idBtnSearch: guid(), idExtra: guid() });
    this.FaixaTemperatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, enable: ko.observable(true), text: ko.observable(Localization.Resources.Cargas.Carga.FaixaDeTemperatura.getFieldDescription()), visible: ko.observable(false), idBtnSearch: guid() });
    this.SubContratante = PropertyEntity({ type: types.map, text: Localization.Resources.Cargas.Carga.CargaContratadaPor.getFieldDescription(), visible: ko.observable(true) });
    this.SubContratado = PropertyEntity({ type: types.map, text: Localization.Resources.Cargas.Carga.Subcontratado.getFieldDescription(), visible: ko.observable(false) });
    this.ContratoFreteTerceiro = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, visible: ko.observable(false) });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, visible: ko.observable(true), enable: ko.observable(true), text: ko.observable(Localization.Resources.Cargas.Carga.ModeloVeicular.getRequiredFieldDescription()), issue: 44, tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.ModeloVeicular.getFieldDescription()), eventChange: modeloVeicularCargaBlur, idBtnSearch: guid() });
    this.NumeroReboques = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.CodTipoCargaOriginal = PropertyEntity({ type: types.local, val: 0 });
    this.CodModeloVeicularCargaOriginal = PropertyEntity({ type: types.local, val: 0 });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Cargas.Carga.OperadorLogisticoArmazem.getFieldDescription(), idBtnSearch: guid() });
    this.Peso = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Peso.getFieldDescription() });
    this.PesoTotalComPallet = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PesoTotalComPallet.getFieldDescription(), visible: ko.observable(false) });
    this.PesoTotalNF = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PesoNotaFiscal.getFieldDescription(), visible: ko.observable(true) });
    this.PesoCubadoNF = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PesoMetroCubadoNotaFiscal.getFieldDescription(), visible: ko.observable(true) });
    this.ValorMercadoria = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ValorNotaFiscal.getFieldDescription(), visible: ko.observable(true) });
    this.PesoLiquido = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PesoLiquido.getFieldDescription() });
    this.PreCalculoFrete = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PreCalculoFrete.getFieldDescription() });
    this.PesoReentrega = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PesoReentrega.getFieldDescription(), visible: ko.observable(false) });
    this.NumeroCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroCarregamento.getFieldDescription(), visible: ko.observable(false) });
    this.DivisoriaIntegracao = PropertyEntity({ val: ko.observable(true), required: false, text: Localization.Resources.Cargas.Carga.Divisoria.getFieldDescription(), visible: ko.observable(true) });
    this.CargaPerigosaIntegracao = PropertyEntity({ val: ko.observable(true), required: false, text: Localization.Resources.Cargas.Carga.CargaPerigosa.getFieldDescription(), id: guid(), visible: ko.observable(true) });
    this.PesoTotal = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PesoTotal.getFieldDescription(), visible: ko.observable(false) });
    this.Ativo = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true });
    this.CargaTipoConsolidacao = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true });
    this.PossuiPendencia = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.OrigemFretePelaJanelaTransportador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.PermiteImportarDocumentosManualmente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.PermitirTransportadorEnviarNotasFiscais = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.PermitirTransportadorSolicitarNotasFiscais = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.PermiteInformarIsca = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.ExigeInformarIsca = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.ExibirValorUnitarioDoProduto = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.PermitirTransbordarNotasDeOutrasCargas = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.AgConfirmacaoUtilizacaoCredito = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.AutoTipoCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.AutoModeloVeicular = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.SituacaoCarga = PropertyEntity({ type: types.map, required: false });
    this.SituacaoAutorizacaoIntegracaoCTe = PropertyEntity({ type: types.map, required: false });
    this.SituacaoAlteracaoFreteCarga = PropertyEntity({ type: types.map, required: false });
    this.DescricaoSituacaoCarga = PropertyEntity({ type: types.map, required: false, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), visible: ko.observable(false) });
    this.Solicitante = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Solicitante.getFieldDescription(), visible: ko.observable(false) });

    this.Operador = PropertyEntity({ type: types.map, required: false, visible: false, text: Localization.Resources.Cargas.Carga.Operador.getFieldDescription() });
    this.AlterarOperador = PropertyEntity({ eventClick: alterarOperadorClick, visible: ko.observable(VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_NaoPermiteAlterarOperador, _PermissoesPersonalizadasCarga) ? true : false), type: types.event, text: Localization.Resources.Cargas.Carga.AlterarOperador });
    this.DataRetornoCD = PropertyEntity({ type: types.map, required: false, visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.DataRetornoCD.getFieldDescription() });
    this.AlterarDataRetornoCD = PropertyEntity({ eventClick: alterarDataRetornoCDClick, visible: ko.observable(true), type: types.event, text: Localization.Resources.Gerais.Geral.Alterar });
    this.DataCarregamento = PropertyEntity({ type: types.map, required: false, visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.DataCarregamento.getFieldDescription() });
    this.PreCalculodeFrete = PropertyEntity({ type: types.map, required: false, visible: ko.observable(true), text: "Pre Calculo de Frete:" });
    this.NotasFilhasRecibidas = PropertyEntity({ type: types.map, required: false, visible: ko.observable(true), text: "NF Filha Recebida:" });
    this.ReenviarPreCalculo = PropertyEntity({ eventClick: recalcularPrecalculo, enable: ko.observable(true), visible: ko.observable(true), type: types.event, text: "Recalcular" });
    this.AlterarDataCarregamento = PropertyEntity({ eventClick: alterarDataCarregamentoClick, visible: ko.observable(true), type: types.event, text: Localization.Resources.Gerais.Geral.Alterar });
    this.ExibirDatasCarregamento = PropertyEntity({ eventClick: exibirDatasCarregamentoClick, enable: ko.observable(true), type: types.event, text: Localization.Resources.Gerais.Geral.VerDetalhes });

    this.OperadorInsercao = PropertyEntity({ type: types.map, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.OperadorInsercao.getFieldDescription() });
    this.PrevisaoEntregaTransportador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PrevisaoEntregaTransportador.getFieldDescription(), getType: typesKnockout.dateTime, required: false, visible: ko.observable(true) });
    this.ValorCustoFrete = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ValorCustoFrete.getFieldDescription(), getType: typesKnockout.decimal, required: false, visible: ko.observable(true) });
    this.DataInicioViagem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataInicioViagem.getFieldDescription(), visible: ko.observable(false) });
    this.Redespacho = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Redespacho.getFieldDescription(), visible: ko.observable(false) });
    this.Onda = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Onda.getFieldDescription(), visible: ko.observable(false) });
    this.ClusterRota = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ClusterRota.getFieldDescription(), visible: ko.observable(false) });
    this.QuantidadeVolumes = PropertyEntity({ text: Localization.Resources.Cargas.Carga.QuantidadeVolumes.getFieldDescription(), visible: ko.observable(false) });

    this.TipoTrecho = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoTrecho.getFieldDescription(), visible: ko.observable(false) });
    this.Regiao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Regioes.getFieldDescription(), visible: ko.observable(false) });
    this.Mesorregiao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Mesorregioes.getFieldDescription(), visible: ko.observable(false) });
    this.NumeroPreCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroPreCarga.getFieldDescription(), visible: ko.observable(false) });
    this.IdentificadorDeRota = PropertyEntity({ text: Localization.Resources.Cargas.Carga.IdentificadorDeRota.getFieldDescription(), visible: ko.observable(false) });

    this.ExternalDT1 = PropertyEntity({ text: "ExternalID1:", visible: ko.observable(false) });
    this.ExternalDT2 = PropertyEntity({ text: "ExternalID2:", visible: ko.observable(false) });
    this.AlterarExternalID1 = PropertyEntity({ eventClick: function (e) { alterarExternalId(e, 1) }, visible: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.Carga.AlterarExternalID1 });
    this.AlterarExternalID2 = PropertyEntity({ eventClick: function (e) { alterarExternalId(e, 2) }, visible: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.Carga.AlterarExternalID2 });

    this.ZonaTransporte = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ZonasDeTransporte.getFieldDescription(), visible: ko.observable(false) });
    this.QuantidadeVolumesNF = PropertyEntity({ text: Localization.Resources.Cargas.Carga.QuantidadeVolumesNotaFiscal.getFieldDescription(), visible: ko.observable(false) });
    this.DataPrevisaoInicioViagem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataPrevisaoInicioViagem.getFieldDescription(), visible: ko.observable(false) });
    this.ValorTotalProdutos = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ValorTotalProdutos.getFieldDescription(), visible: ko.observable(false) });
    this.QuantidadeNFEs = PropertyEntity({ text: Localization.Resources.Cargas.Carga.QuantidadeNFs.getFieldDescription(), visible: ko.observable(false) });
    this.VolumesCaixasNFEs = PropertyEntity({ text: Localization.Resources.Cargas.Carga.VolumeNFs.getFieldDescription(), visible: ko.observable(false) });
    this.ProvedoresOS = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ProvedoresOS.getFieldDescription(), visible: ko.observable(false) });
    this.IdMontagemContainer = PropertyEntity({ text: Localization.Resources.Cargas.Carga.IdMontagemContainer.getFieldDescription(), visible: ko.observable(false) });
    this.NumeroContainerCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroContainerCarga.getFieldDescription(), visible: ko.observable(false) });
    this.ExigeTermoAceiteTransportador = PropertyEntity({ val: ko.observable(false), def: false, visible: ko.observable(false), getType: typesKnockout.bool });
    this.CodigoCargaJanelaCarregamentoTransportador = PropertyEntity({ val: ko.observable(0), def: 0, visible: ko.observable(false), getType: typesKnockout.int });
    this.TermoAceite = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.RotaEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.RotaEmbarcador.getFieldDescription(), visible: ko.observable(false) });
    this.DataUltimaLiberacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataUltimaLiberacao.getFieldDescription() });
    this.UsuarioCriacaoRemessa = PropertyEntity({ text: Localization.Resources.Cargas.Carga.UsuarioCriacaoRemessa.getFieldDescription() });
    this.NumeroOrdem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroOrdem.getFieldDescription() });
    this.Cubagem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Cubagem.getFieldDescription() });
    this.RegiaoDestino = PropertyEntity({ text: Localization.Resources.Cargas.Carga.RegiaoDestino.getFieldDescription() });
    this.NumeroEntregasFinais = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroEntregasFinais.getFieldDescription(), visible: ko.observable(false) });
    this.ObservacaoLocalEntrega = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ObservacaoLocalDeEntrega.getFieldDescription() });
    this.PLPsCorreios = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PLP.getFieldDescription(), visible: ko.observable(true) });
    this.NumerosEtiquetasCorreios = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Objeto.getFieldDescription(), visible: ko.observable(true) });
    this.LocalParqueamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.LocalParqueamento.getFieldDescription() });
    this.CategoriaCargaEmbarcador = PropertyEntity({ text: "Tipo de Carga Taura", visible: ko.observable(false) });
    this.SetPointVeiculo = PropertyEntity({ text: "SetPoint Transp.", visible: ko.observable(false) });
    this.RangeTempCarga = PropertyEntity({ text: "Range Temp. Transp.", visible: ko.observable(false) });
    this.ValorFreteSimulacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.FreteSimulado.getFieldDescription(), visible: ko.observable(false) });
    this.ValorToneladaSimulado = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ValorToneladaSimulado.getFieldDescription(), visible: ko.observable(false) });
    this.SituacaoRecebimentoNotas = PropertyEntity({ text: Localization.Resources.Cargas.Carga.SituacaoRecebimentoNotas.getFieldDescription(), val: ko.observable(EnumFormaIntegracao.Todas), def: EnumFormaIntegracao.Todas, visible: ko.observable(false) });
    this.CategoriaOS = PropertyEntity({ text: Localization.Resources.Cargas.Carga.CategoriaOS.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) });
    this.TipoOSConvertido = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoOSConvertido.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) });
    this.TipoOS = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoOS.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) });
    this.DirecionamentoCustoExtra = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DirecionamentoCustoExtra.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) });
    this.StatusCustoExtra = PropertyEntity({ text: Localization.Resources.Cargas.Carga.StatusCustoExtra.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.UtilizarDirecionamentoCustoExtra = PropertyEntity({ val: ko.observable(false), def: false, visible: ko.observable(false), getType: typesKnockout.bool });
    this.CanalVenda = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.CanalVenda.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoCobrancaMultimodal = PropertyEntity({ getType: typesKnockout.selectMultiple, text: Localization.Resources.Cargas.Carga.TipoCobrancaMultimodal.getFieldDescription(), options: EnumTipoCobrancaMultimodal.obterOpcoes(), val: ko.observable([]), def: [], visible: ko.observable(true) });

    this.ControleVeiculosCheckList = PropertyEntity({ eventClick: ControleVeiculosCheckList, type: types.event, text: Localization.Resources.Cargas.Carga.ControleDeVeiculosCheckList, visible: ko.observable(false) });
    this.ControleDivisoesCapacidade = PropertyEntity({ eventClick: ControleDivisoesCapacidadeClick, type: types.event, text: Localization.Resources.Cargas.Carga.ControleDivisoesCapacidade, visible: ko.observable(false) });
    this.AdicionarAnexos = PropertyEntity({ eventClick: adicionarAnexosCargaGuaritaClick, type: types.event, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.Anexos });
    this.DetalhesPedidos = PropertyEntity({ eventClick: detalhesCargaPedidosClick, type: types.event, text: Localization.Resources.Cargas.Carga.VerDetalhesDosPedidos, visible: ko.observable(true) });
    this.PercursoCargaMDFe = PropertyEntity({ eventClick: percursoMDFeClick, type: types.event, text: Localization.Resources.Cargas.Carga.PassagensEntreEstados, visible: ko.observable(true), visibleTMS: ko.observable(false), idTMS: guid() });
    this.ConsultarVeiculosSugeridosHUB = PropertyEntity({ eventClick: exibirModalVeiculosSugeridosHUB, type: types.event, text: Localization.Resources.Cargas.Carga.ConsultarVeiculosSugeridosHUB, visible: ko.observable(false) });
    this.SugereIntegracaoHUBSaibaMais = PropertyEntity({ eventClick: redirecionarParaSaibaMaisSugestaoHUB, type: types.event, visible: ko.observable(true) });
    this.DataRetiradaCtrnVeiculo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.DataRetiradaCTRN.getFieldDescription()), getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.DataRetiradaCtrnReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.DataRetiradaCTRN.getFieldDescription()), getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.DataRetiradaCtrnSegundoReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.DataRetiradaCTRN.getFieldDescription()), getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.DataRetiradaCtrnTerceiroReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.DataRetiradaCTRN.getFieldDescription()), getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.GensetVeiculo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.Genset.getFieldDescription()), maxlength: 100, enable: ko.observable(true), visible: ko.observable(false) });
    this.GensetReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.Genset.getFieldDescription()), maxlength: 100, enable: ko.observable(true), visible: ko.observable(false) });
    this.GensetSegundoReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.Genset.getFieldDescription()), maxlength: 100, enable: ko.observable(true), visible: ko.observable(false) });
    this.GensetTerceiroReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.Genset.getFieldDescription()), maxlength: 100, enable: ko.observable(true), visible: ko.observable(false) });
    this.MaxGrossVeiculo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.MaxGross.getFieldDescription()), getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(false) });
    this.MaxGrossReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.MaxGross.getFieldDescription()), getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(false) });
    this.MaxGrossSegundoReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.MaxGross.getFieldDescription()), getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(false) });
    this.MaxGrossTerceiroReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.MaxGross.getFieldDescription()), getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(false) });
    this.NumeroContainerVeiculo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.NumeroContainerDoVeiculo.getFieldDescription()), maxlength: 1000, enable: ko.observable(() => { if (!this.Veiculo.val()) { return false } return true }), visible: ko.observable(false), required: ko.observable(false) });
    this.NumeroContainerReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.NumeroContainerDoVeiculo.getFieldDescription()), maxlength: 1000, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.NumeroContainerSegundoReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.NumeroContainerDoVeiculo.getFieldDescription()), maxlength: 1000, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.NumeroContainerTerceiroReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.NumeroContainerDoVeiculo.getFieldDescription()), maxlength: 1000, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.TaraContainerVeiculo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.TaraContainer.getFieldDescription()), getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(false) });
    this.TaraContainerReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.TaraContainer.getFieldDescription()), getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(false) });
    this.TaraContainerSegundoReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.TaraContainer.getFieldDescription()), getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(false) });
    this.TaraContainerTerceiroReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.TaraContainer.getFieldDescription()), getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(false) });
    this.ApoliceSeguro = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.Carga.ApoliceDeSeguro.getFieldDescription()), entityDescription: ko.observable(""), multiplesEntities: ko.observableArray([]), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.ApoliceSeguroTransportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: this.ApoliceSeguro.text, entityDescription: this.ApoliceSeguro.entityDescription, multiplesEntities: this.ApoliceSeguro.multiplesEntities, val: this.ApoliceSeguro.val, idBtnSearch: guid(), enable: this.ApoliceSeguro.enable, visible: this.ApoliceSeguro.visible });
    this.InicioCarregamento = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.DataDoCarregamento.getFieldDescription()), getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.InicioCarregamentoTransportador = PropertyEntity({ text: this.InicioCarregamento.text, getType: typesKnockout.dateTime, val: this.InicioCarregamento.val, enable: this.InicioCarregamento.enable, visible: this.InicioCarregamento.visible, required: this.InicioCarregamento.required });
    this.TerminoCarregamento = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.DataDoDescarregamento.getFieldDescription()), getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.TerminoCarregamentoTransportador = PropertyEntity({ text: this.TerminoCarregamento.text, getType: typesKnockout.dateTime, val: this.TerminoCarregamento.val, enable: this.TerminoCarregamento.enable, visible: this.TerminoCarregamento.visible, required: this.TerminoCarregamento.required });
    this.RejeitadaPeloTransportador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.PossuiGenset = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.LiberadaEtapaFaturamentoBloqueada = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.PermitirInformarAnexoContainerCarga = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PermiteTransportadorAvancarEtapaEmissao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PermitirSelecionarNotasCompativeis = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.ExibirMensagensAlerta = PropertyEntity({ eventClick: exibirMensagemAlertaCargaClick, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: false });
    this.MensagemAlertaComBloqueio = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.MensagensAlerta = PropertyEntity({ val: ko.observableArray() });
    this.PossuiOcultarInformacoesCarga = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PermitirBuscarNFesEmillenium = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.PedidoViagemNavio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, visible: ko.observable(false), enable: ko.observable(true), text: ko.observable(Localization.Resources.Cargas.Carga.NavioViagemDirecao.getRequiredFieldDescription()), tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.NavioViagemDirecao.getFieldDescription()), idBtnSearch: guid(), val: ko.observable("") });
    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, visible: ko.observable(false), enable: ko.observable(true), text: ko.observable(Localization.Resources.Cargas.Carga.PortoOrigem.getRequiredFieldDescription()), tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.PortoOrigem.getFieldDescription()), idBtnSearch: guid(), val: ko.observable("") });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, visible: ko.observable(false), enable: ko.observable(true), text: ko.observable(Localization.Resources.Cargas.Carga.PortoDestino.getRequiredFieldDescription()), tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.PortoDestino.getFieldDescription()), idBtnSearch: guid(), val: ko.observable("") });
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, visible: ko.observable(false), enable: ko.observable(true), text: ko.observable(Localization.Resources.Cargas.Carga.TerminalOrigem.getRequiredFieldDescription()), tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.TerminalOrigem.getFieldDescription()), idBtnSearch: guid(), val: ko.observable("") });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, visible: ko.observable(false), enable: ko.observable(true), text: ko.observable(Localization.Resources.Cargas.Carga.TerminalDestino.getRequiredFieldDescription()), tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.TerminalDestino.getFieldDescription()), idBtnSearch: guid(), val: ko.observable("") });
    this.CargaTakeOrPay = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.CargaDemurrage = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.CargaDetention = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.CargaSVM = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true });
    this.PossuiIntegracaoIntercement = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.CargaDestinadaCTeComplementar = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.PossuiIntegracaoMichelin = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.CargaColeta = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true });
    this.CargaSVMTerceiro = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true });
    this.Cliente = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.Cliente.getFieldDescription(), val: ko.observable("") });
    this.NumeroBooking = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.NumeroDoBooking.getFieldDescription(), val: ko.observable("") });
    this.TipoContainerDescricao = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.TipoDoContainer.getFieldDescription(), val: ko.observable("") });
    this.NumeroContainer = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.Containers.getFieldDescription(), val: ko.observable("") });
    this.DescricaoTransbordos = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.Transbordos.getFieldDescription(), val: ko.observable("") });
    this.PedidoEmpresaResponsavel = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.Empresa.getFieldDescription(), val: ko.observable("") });
    this.PedidoCentroCusto = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.CentroDeCusto.getFieldDescription(), val: ko.observable("") });
    this.CargaTrocaDeNota = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true });
    this.Reentrega = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true });
    this.PermitirTransportadorInformarObservacaoImpressaoCarga = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true });
    this.PermiteInformarTransportadorEtapaUmQuandoNotaFiscalNaoRecebidaAntesCalculoFrete = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.PermiteInformarMotoristasSomenteEtapaUmQuandoNotasFiscaisNaoSaoRecebidasAntesCalcularFrete = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.Navio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.NavioRebocador.getRequiredFieldDescription(), idBtnSearch: guid(), val: ko.observable("") });
    this.Balsa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.Balsa.getRequiredFieldDescription(), idBtnSearch: guid(), val: ko.observable("") });
    this.TransportadorSubcontratado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true), text: '*Transportador Subcontratado', idBtnSearch: guid(), val: ko.observable("") });
    this.CalcularFretePeloBIDPedidoOrigem = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.InformarValorFreteTerceiroManualmente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });

    this.QtdContainerCarga = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.QuantidadeCntrNaCarga.getFieldDescription(), val: ko.observable("") });
    this.ModalCarga = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.Modal.getFieldDescription(), val: ko.observable("") });
    this.PortoOrigemCarga = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.PortoOrigem.getFieldDescription(), val: ko.observable("") });
    this.PortoDestinoCarga = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.PortoDestino.getFieldDescription(), val: ko.observable("") });
    this.RecebedorCarga = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.Recebedor.getFieldDescription(), val: ko.observable("") });
    this.Ordem = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.Ordem.getFieldDescription(), val: ko.observable("") });
    this.NumeroPager = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.NumeroPager.getFieldDescription()), maxlength: 100, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataBaseCRT = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataBaseCRT.getFieldDescription(), getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.TipoCarregamento.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.CentroDeResultado.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.LocalCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Local de Carregamento:"), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });

    this.QuantidadePaletes = PropertyEntity({ text: Localization.Resources.Cargas.Carga.QuantidadePaletes.getFieldDescription(), getType: typesKnockout.int, maxlength: 4, val: ko.observable(""), visible: ko.observable(false), enable: ko.observable(true) });

    this.AlterarFaixaTemperatura = PropertyEntity({ eventClick: AlterarFaixaTemperaturaClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Alterar), visible: ko.observable(false) });

    this.AdicionarMotoristas = PropertyEntity({ idBtnSearch: guid(), type: types.event, text: Localization.Resources.Cargas.Carga.InformarMotoristas, visible: ko.observable(true), enable: ko.observable(true), idGrid: guid() });
    this.AjustarPecentualExecucao = PropertyEntity({ eventClick: ajusteDePercentualExecucaoClick, text: "Ajuste % de Execução", type: types.event, visible: ko.observable(false), enable: ko.observable(true) });

    this.AdicionarAjudantes = PropertyEntity({ idBtnSearch: guid(), type: types.event, text: Localization.Resources.Cargas.Carga.InformarAjudantes, visible: ko.observable(false), enable: ko.observable(true), idGrid: guid() });

    this.InformarPlacaCarregamento = PropertyEntity({ idGrid: guid(), enable: ko.observable(true), visible: ko.observable(false) });

    this.PlacasCarragamento = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.ExigePlacaTracao = PropertyEntity({ visible: ko.observable(false), val: ko.observable(false) });
    this.NaoPermitirAlterarMotoristaAposAverbacaoContainer = PropertyEntity({ visible: ko.observable(false), val: ko.observable(false) });

    this.NumeroLacre = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Numero.getFieldDescription(), maxlength: 60, required: true });
    this.AdicionarLacre = PropertyEntity({ eventClick: AdicionarCargaTransportadorLacreClick, type: types.event, text: Localization.Resources.Cargas.Carga.AdicionarLacre, enable: ko.observable(true), idGrid: guid() });

    this.SalvarDadosCarga = PropertyEntity({ eventClick: salvarDadosCargaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(true), enable: ko.observable(true) });
    this.SalvarDadosTransporte = PropertyEntity({ eventClick: SalvarDadosTransporteSemLiberarTeleriscoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(true), enable: ko.observable(true) });
    this.SalvarDadosTransporteESolicitarNFes = PropertyEntity({ eventClick: SalvarDadosTransporteSemLiberarTeleriscoSolicitandoNFesClick, type: types.event, text: Localization.Resources.Cargas.Carga.SolicitarAsNotasFiscaisDaCarga, visible: ko.observable(false), enable: ko.observable(true) });
    this.LiberarComLicencaInvalida = PropertyEntity({ eventClick: LiberarComLicencaInvalidaClick, type: types.event, text: Localization.Resources.Cargas.Carga.LiberarComLicencaInvalida, visible: ko.observable(false), enable: ko.observable(true) });
    this.DisponibilizarParaTransportadorCarga = PropertyEntity({ eventClick: disponibilizarParaTransportadorClick, type: types.event, text: Localization.Resources.Cargas.Carga.DisponibilizarParaTransportador, visible: ko.observable(false) });
    this.LiberarComProblemaIntegracaoGrMotoristaVeiculo = PropertyEntity({ eventClick: LiberarComProblemaIntegracaoGrMotoristaVeiculoClick, enable: ko.observable(true), idContainer: guid(), idText: guid(), visible: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.Carga.LiberarSemIntegracaoGR });
    this.DetalhesLiberarComProblemaIntegracaoGrMotoristaVeiculo = PropertyEntity({ eventClick: DetalhesLiberarComProblemaIntegracaoGrMotoristaVeiculoClick, enable: ko.observable(true), visible: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.Carga.DetalhesLiberacaoSemIntegracaoGR });
    this.LiberarCargaSemPlanejamento = PropertyEntity({ eventClick: LiberarCargaSemPlanejamentoClick, enable: ko.observable(true), idContainer: guid(), idText: guid(), visible: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.Carga.LiberarCargaSemPlanejamento });
    this.LiberadaComCargaSemPlanejamento = PropertyEntity({ visible: ko.observable(false), val: ko.observable(false) });
    this.InformarRetiradaContainer = PropertyEntity({ eventClick: informarRetiradaContainerClick, visible: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.Carga.RetiradaDeContainer });
    this.VeiculosInvalidosCarga = PropertyEntity({ idContainer: guid(), idText: guid(), visible: ko.observable(false), text: ko.observable("") });
    this.ProblemaLicencaInvalida = PropertyEntity({ idContainer: guid(), idText: guid(), visible: ko.observable(false) });
    this.LiberadoComProblemaIntegracaoGrMotoristaVeiculo = PropertyEntity({ getType: typesKnockout.bool, type: types.map });
    this.SalvarDadosTransporteSemSolicitarNFes = PropertyEntity({ getType: typesKnockout.bool, type: types.map });
    this.ProtocoloIntegracaoGR = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.LiberadaComLicencaInvalida = PropertyEntity({ getType: typesKnockout.bool, type: types.map });
    this.AgrupadaPosEmissaoDocumento = PropertyEntity({ getType: typesKnockout.bool, type: types.map });

    this.TransferenciaContainer = PropertyEntity({ getType: typesKnockout.bool, type: types.map });
    this.RemetenteTrasferencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: false, enable: ko.observable(true), val: ko.observable("") }); /*Utilizado para carga de transferencia Container.. buscar containers apenas de um local.*/

    this.ExibirCalculoFreteCargaAgrupada = PropertyEntity({ getType: typesKnockout.bool, type: types.map });

    this.ExcecaoCab = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.ExcecaoCab.getFieldDescription(), val: ko.observable("") });

    //this.ImprimirDadosTransporte = PropertyEntity({ eventClick: ImprimirDadosTransporteClick, type: types.event, text: "Imprimir", visible: ko.observable(true), enable: ko.observable(true) });

    this.ImprimirDadosTransporte = PropertyEntity({
        eventClick: ImprimirDadosTransporteClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.DescricaoCarga), visible: ko.observable(true), enable: ko.observable(true), icon: "fal fa-eye", verImpressoes: Localization.Resources.Cargas.Carga.Impressoes
    });
    this.ImprimirPlanoViagem = PropertyEntity({
        eventClick: ImprimirPlanoViagemTransporteClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.PlanoDeViagem), visible: ko.observable(true), enable: ko.observable(true), icon: "fal fa-paper-plane"
    });
    this.ImprimirFichaMotorista = PropertyEntity({
        eventClick: ImprimirFichaMotoristaClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.FichaMotorista), visible: ko.observable(true), enable: ko.observable(true), icon: "fal fa-list-alt"
    });
    this.ImprimirOrdemColeta = PropertyEntity({
        eventClick: ImprimirOrdemColetaClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.OrdemDeColeta), visible: ko.observable(true), enable: ko.observable(true), icon: "fal fa-car"
    });

    this.ExtrasDadosCarga = PropertyEntity({ type: types.local, visible: ko.observable(false) });

    this.BuscarNovamenteMDFe = PropertyEntity({ eventClick: buscarMDFeEmEncerramentoClick, type: types.event, text: Localization.Resources.Cargas.Carga.BuscarAtualizar, visible: ko.observable(true) });
    this.CargaRedespacho = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.PossuiCTeAnteriorFilialEmissora = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.UtilizarCTesAnterioresComoCTeFilialEmissora = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.ExclusivaDeSubcontratacaoOuRedespacho = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.CargaTransbordo = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });

    this.TipoContratacaoCarga = PropertyEntity({ getType: typesKnockout.int, type: types.map, required: false });
    this.EmissaoLiberada = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.AguardarIntegracaoEtapaTransportador = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.ValidarLicencaMotorista = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });

    this.DataInicioEmissao = PropertyEntity({ type: types.map, text: Localization.Resources.Cargas.Carga.DataInicioDeEmissao.getFieldDescription(), visible: ko.observable(false) });
    this.DataAvancouSegundaEtapa = PropertyEntity({ visible: ko.observable(false) });

    this.ExigeNotaFiscalParaCalcularFrete = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });

    this.NaoExigeVeiculoParaEmissao = PropertyEntity({ getType: typesKnockout.bool });
    this.CargaDeComplemento = PropertyEntity({ getType: typesKnockout.bool });
    this.CargaDePreCarga = PropertyEntity({ getType: typesKnockout.bool });
    this.CargaAgrupada = PropertyEntity({ getType: typesKnockout.bool });

    this.ExigeConfirmacaoTracao = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), visible: ko.observable(false), required: false, cssClass: ko.observable("col col-12"), text: ko.observable(Localization.Resources.Cargas.Carga.VeiculoCarreta.getRequiredFieldDescription()), idBtnSearch: guid(), popover: "<strong>" + Localization.Resources.Cargas.Carga.CliqueAquiPraVisualizarOsDetalhes + "</strong>", detalhesClick: exibirDetalhesReboqueClick, verDetalhesVisible: ko.observable(false), verDetalhesText: Localization.Resources.Cargas.Carga.VerDetalhes });
    this.SegundoReboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), visible: ko.observable(false), required: false, cssClass: ko.observable("col col-12"), text: ko.observable(Localization.Resources.Cargas.Carga.VeiculoCarretaDois.getRequiredFieldDescription()), idBtnSearch: guid(), popover: "<strong>" + Localization.Resources.Cargas.Carga.CliqueAquiPraVisualizarOsDetalhes + "</strong>", detalhesClick: exibirDetalhesSegundoReboqueClick, verDetalhesVisible: ko.observable(false), verDetalhesText: Localization.Resources.Cargas.Carga.VerDetalhes });
    this.TerceiroReboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), visible: ko.observable(false), required: false, cssClass: ko.observable("col col-12"), text: ko.observable(Localization.Resources.Cargas.Carga.VeiculoCarretaTres.getRequiredFieldDescription()), idBtnSearch: guid(), popover: "<strong>" + Localization.Resources.Cargas.Carga.CliqueAquiPraVisualizarOsDetalhes + "</strong>", detalhesClick: exibirDetalhesSegundoReboqueClick, verDetalhesVisible: ko.observable(false), verDetalhesText: Localization.Resources.Cargas.Carga.VerDetalhes });
    this.CodigoContainerReboque = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoContainerSegundoReboque = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoContainerTerceiroReboque = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoContainerVeiculo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.ContainerReboqueAnexo = new CargaVeiculoContainerAnexo();
    this.ContainerSegundoReboqueAnexo = new CargaVeiculoContainerAnexo();
    this.ContainerTerceiroReboqueAnexo = new CargaVeiculoContainerAnexo();
    this.ContainerVeiculoAnexo = new CargaVeiculoContainerAnexo();
    this.ExigirDataRetiradaCtrnVeiculos = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.ExigirNumeroContainerVeiculos = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });

    this.JustificativaAutorizacaoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(false), visible: ko.observable(false), required: false, text: ko.observable("Justificativa Custo Extra"), idBtnSearch: guid(), placas: ko.observable(""), cssClass: ko.observable("col col-12"), popover: "<strong>" + Localization.Resources.Cargas.Carga.CliqueAquiPraVisualizarOsDetalhes + "</strong>", detalhesClick: exibirDetalhesVeiculoClick, verDetalhesVisible: ko.observable(false), verDetalhesText: Localization.Resources.Cargas.Carga.VerDetalhes });

    this.HorarioLimiteConfirmacaoMotorista = PropertyEntity({ val: ko.observable(0), def: 0, visible: ko.observable(false) });
    this.MotoristaRecusouCarga = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });

    this.ConfirmacaoMotorista = PropertyEntity({ eventClick: function () { confirmarMotoristaCarga(self); }, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.ConfirmarMotorista.getFieldDescription()), icon: "fal fa-user", visible: ko.observable(false), enable: ko.observable(true) });

    this.ProcessandoDocumentosFiscais = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.CTesEmDigitacao = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.AgGeracaoCTesAnteriorFilialEmissora = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.EmEmissaoCTeSubContratacaoFilialEmissora = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.AverbandoCTes = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.AverbouTodosCTes = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.EmitindoCTes = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.AutorizouTodosCTes = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.PossuiNFs = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.possuiCTe = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.possuiNFSManual = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.AgNFSManual = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.CargaEmitidaParcialmente = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });

    this.PercentualSeparacaoMercadoria = PropertyEntity({ getType: typesKnockout.map, type: types.map, required: false, cssClass: ko.observable("progress-bar bg-color-yellow") });

    this.PossuiCTeSubcontratacaoFilialEmissora = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.SeparacaoConferida = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.PossuiSeparacao = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.PossuiRotaDefinida = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.ObrigatorioInformarAnexoSolicitacaoFrete = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });

    this.PossuiAverbacaoMDFe = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.possuiAverbacaoCTe = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.ProblemaMDFe = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.ProblemaAverbacaoMDFe = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.ProblemaAverbacaoCTe = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.ProblemaIntegracaoCIOT = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.LiberadoComProblemaCIOT = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });

    this.ProblemaIntegracaoValePedagio = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.PossuiIntegracaoValePedagio = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.LiberadoComProblemaValePedagio = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.IntegrandoValePedagio = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.LiberadoComProblemaPagamentoMotorista = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.ProblemaIntegracaoPagamentoMotorista = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });

    this.LiberadaSemTodosPreCTes = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.AgImportacaoCTe = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.ProblemaNFS = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.ProblemaCTE = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.ProblemaEmissaoNFeRemessa = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.NaoPermitirLiberarSemValePedagio = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.ClassificacaoNFeRemessaVenda = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });

    this.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.EmiteMDFeFilialEmissora = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.NaoGerarMDFe = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.PendenteGerarCargaDistribuidor = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.CalcularFreteCliente = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });

    this.EmissaoDocumentosAutorizada = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });

    this.CodigoMotoristaVeiculo = PropertyEntity({});
    this.NomeMotoristaVeiculo = PropertyEntity({});

    this.FreteDeTerceiro = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });

    this.MotivoPendencia = PropertyEntity({ type: types.map, required: false });

    this.TipoIntegracaoMercadoLivre = PropertyEntity({ val: ko.observable(EnumTipoIntegracaoMercadoLivre.HandlingUnit), options: EnumTipoIntegracaoMercadoLivre.obterOpcoes(), def: EnumTipoIntegracaoMercadoLivre.HandlingUnit, enable: ko.observable(true) });

    this.InfoTipoFreteEscolhido = PropertyEntity({ val: ko.observable(""), type: types.local, visible: ko.observable(false) });
    this.MotivoDoCancelamento = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.MotivoDoCancelamento.getFieldDescription()), type: types.local, visible: ko.observable(false) });
    this.MensagemRejeicaoDeCancelamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NotaDeCancelamento.getFieldDescription(), type: types.local, visible: ko.observable(false) });
    this.CodigoCargaOrigemCancelada = PropertyEntity({ text: Localization.Resources.Cargas.Carga.CodigoCargaOrigemCancelamento, type: types.local, visible: ko.observable(false) });
    this.DivCarga = PropertyEntity({ type: types.local });
    this.AguardandoNFs = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.SolicitarNFEsCarga = PropertyEntity({ eventClick: solicitarNFsClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.SolicitarAsNotasFiscaisDaCarga), idTab: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.AutorizarEmissaoDocumentos = PropertyEntity({ eventClick: validarFreteIniciarEmissaoClick, visibleBTN: ko.observable(true), type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.IrParaEtapaDeEmissao), idTab: guid(), visible: ko.observable(false), enable: ko.observable(false), visibleCTeProcessamento: ko.observable(false) });
    this.LiberarSemConfirmacaoERP = PropertyEntity({ eventClick: liberarSemConfirmacaoERPClick, type: types.event, text: Localization.Resources.Cargas.Carga.LiberarMesmoSemConfirmacaoDoERP, idTab: guid(), visible: ko.observable(false), enable: ko.observable(false), visibleCTeProcessamento: ko.observable(false) });

    this.RelacaoEntrega = PropertyEntity({
        eventClick: RelacaoEntregaClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.RelacaoDeEntrega), visible: ko.observable(false), enable: ko.observable(true), icon: "fal fa-eye"
    });

    this.RelacaoEmbarque = PropertyEntity({
        eventClick: RelacaoEmbarqueClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.RelacaoDeEmbarque), visible: ko.observable(_CONFIGURACAO_TMS.ExibirClassificacaoNFe), enable: ko.observable(true), icon: "fal fa-eye"
    });

    this.SalvarDadosTransportador = PropertyEntity({ eventClick: salvarDadosTransportadorClick, enable: ko.observable(true), type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(true) });
    this.DisponibilizarParaTransportador = PropertyEntity({ eventClick: disponibilizarParaTransportadorClick, type: types.event, text: Localization.Resources.Cargas.Carga.DisponibilizarParaTransportador, visible: ko.observable(false) });

    this.ObservacaoEmissaoCarga = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.ObservacaoEmissaoCargaTomador = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.ObservacaoEmissaoCargaTipoOperacao = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.ValorTotalMercadoriaPedidos = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ValorTotalMercadoriaDosPedidos.getFieldDescription(), visible: ko.observable(true) });
    this.CodigoCargaVeiculoContainer = PropertyEntity({ val: ko.observable(0), def: 0, visible: ko.observable(false), getType: typesKnockout.int });
    this.ExistePacote = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PermiteConsultarPorPacotesLoggi = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    //****DADOS NF-e ***
    this.QuantidadePedidosEtapaNFe = PropertyEntity({ text: Localization.Resources.Cargas.Carga.QuantidadePedidosEtapaNFe, val: ko.observable(""), def: "" });
    this.PedidoComNFe = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PedidoComNFe, options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Cargas.Carga.PedidoComNFeIntegrada, Localization.Resources.Cargas.Carga.PedidoQueNaoIntegrouNFe), val: ko.observable(""), def: "" });
    this.PesquisarEtapaNFe = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Pesquisar, eventClick: PesquisarEtapaNFeClick, type: types.event });
    this.ExibirFiltrosEtapaNFe = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltrosEtapaNFe.visibleFade()) {
                e.ExibirFiltrosEtapaNFe.visibleFade(false);
            } else {
                e.ExibirFiltrosEtapaNFe.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Cargas.Carga.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    //****DADOS FRETE ***
    this.TipoFreteEscolhido = PropertyEntity({ type: types.map, required: false });
    this.ValorFreteOperador = PropertyEntity({ type: types.local, text: ko.observable(Localization.Resources.Cargas.Carga.AlterarValorDoFrete.getFieldDescription()), getType: typesKnockout.decimal, visible: ko.observable(true), enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-8 col-lg-6") });
    this.Moeda = PropertyEntity({ enable: ko.observable(false), text: Localization.Resources.Cargas.Carga.Moeda.getFieldDescription(), visible: ko.observable(false), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), def: EnumMoedaCotacaoBancoCentral.Real });
    this.ValorTotalMoeda = PropertyEntity({ enable: ko.observable(true), visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.ValorEmMoeda.getFieldDescription(), def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, maxlength: 15, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorTotalMoedaPagar = PropertyEntity({ enable: ko.observable(true), visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.ValorTotalEmMoeda.getFieldDescription(), def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, maxlength: 15, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorCotacaoMoeda = PropertyEntity({ enable: ko.observable(false), visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.CotacaoDaMoeda.getFieldDescription(), def: "1,0000000000", val: ko.observable("1,0000000000"), getType: typesKnockout.decimal, maxlength: 22, configDecimal: { precision: 10, allowZero: false, allowNegative: false } });
    this.MotivoPendenciaFrete = PropertyEntity({ options: EnumMotivoPendenciaFrete.obterOpcoes(), val: ko.observable(EnumMoedaCotacaoBancoCentral.NenhumPendencia), def: EnumMoedaCotacaoBancoCentral.NenhumPendencia, issue: 0, visible: ko.observable(true) });

    //this.Moeda.val.subscribe(function (tipoMoeda) { ObterCotacaoMoedaCarga(self, tipoMoeda); });
    this.ValorTotalMoeda.val.subscribe(function () { ConverterValorMoedaCarga(self); });

    this.PermiteAdicionarAnexosGuarita = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.TipoCalculoTabelaFrete = PropertyEntity({ type: types.local });
    this.ValorFreteTabelaFrete = PropertyEntity({ type: types.local, getType: typesKnockout.decimal });
    this.AtualizarValorFrete = PropertyEntity({ type: types.event, eventClick: atualizarValorFreteClick, text: Localization.Resources.Cargas.Carga.AtualizarValor.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true), enable: ko.observable(true) });
    this.RecalcularFrete = PropertyEntity({ eventClick: recalcularFreteClick, type: types.event, text: Localization.Resources.Cargas.Carga.CalcularFreteNovamente, visible: ko.observable(false), enable: ko.observable(true) });
    this.RecalcularFreteBID = PropertyEntity({ eventClick: recalcularFreteBIDClick, type: types.event, text: Localization.Resources.Cargas.Carga.RecalcularFreteBID, visible: ko.observable(false), enable: ko.observable(true) });
    this.ValorPorPedido = PropertyEntity({ eventClick: ValorPorPedidoClick, type: types.event, text: Localization.Resources.Cargas.Carga.ValorPorPedido, visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorPorPedidoFilialEmissora = PropertyEntity({ eventClick: ValorPorPedidoFilialEmissoraClick, type: types.event, text: Localization.Resources.Cargas.Carga.ValorPorPedidoFilialEmissora, visible: ko.observable(true), enable: ko.observable(true) });

    this.ExcluirPreCalculo = PropertyEntity({ eventClick: excluirPreCalculoClick, type: types.event, text: Localization.Resources.Cargas.Carga.RemoverPreCalculo, visible: ko.observable(false), enable: ko.observable(true) });

    this.RateiarValorNota = PropertyEntity({ eventClick: rateiarValorNotaClick, type: types.event, text: Localization.Resources.Cargas.Carga.RatearValorNota, visible: ko.observable(false), enable: ko.observable(true) });

    this.UtilizarContratoFrete = PropertyEntity({ eventClick: UtilizarContratoFreteClick, type: types.event, text: Localization.Resources.Cargas.Carga.CalcularFreteNovamente, visible: ko.observable(false), enable: ko.observable(false) });
    this.RoteirizarCargaNovamente = PropertyEntity({ eventClick: RoteirizarCargaNovamenteClick, type: types.event, text: Localization.Resources.Cargas.Carga.SolicitarRoteirizacao, visible: ko.observable(false), enable: ko.observable(false) });

    this.LiberarEmissaoDiferencaValorFrete = PropertyEntity({ eventClick: LiberarEmissaoDiferencaValorFreteClick, type: types.event, text: Localization.Resources.Cargas.Carga.AutorizarEmissaoComDiferecaNoFrete, visible: ko.observable(false), enable: ko.observable(true) });
    this.AdicionarComplementoFrete = PropertyEntity({ eventClick: adicionarComplementoFreteClick, idGrid: guid(), type: types.event, text: Localization.Resources.Cargas.Carga.AdicionarComplementoDeFrete, visible: ko.observable(_operadorLogistica.PermiteAdicionarComplementosDeFrete), visibleFade: ko.observable(false), enable: ko.observable(true) });
    this.ComponenteFrete = PropertyEntity({ eventClick: adicionarComponenteFreteClick, idGrid: guid(), type: types.event, text: Localization.Resources.Cargas.Carga.AdicionarComponente, visible: ko.observable(_operadorLogistica.PermiteAdicionarComplementosDeFrete), visibleFade: ko.observable(false), enable: ko.observable(true), idDadosEmissao: guid() });
    this.DadosEmissaoFrete = PropertyEntity({ visible: ko.observable(false) });
    this.CargaPercursos = PropertyEntity({ type: types.listEntity, list: new Array(), visible: ko.observable(true), codEntity: ko.observable(0), defCodEntity: 0, text: Localization.Resources.Cargas.Carga.PercursoDaCarga.getFieldDescription() });
    this.Pedidos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, list: new Array(), visible: ko.observable(true) });
    this.Transbordos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, list: new Array(), visible: ko.observable(true) });
    this.VerDetalhesFreteCarga = PropertyEntity({ eventClick: verDetalhesFreteNaCargaClick, type: types.event, text: Localization.Resources.Gerais.Geral.VerDetalhes, visible: ko.observable(false) });
    this.VerSimulacaoFrete = PropertyEntity({ eventClick: verSimulacaoFreteClick, type: types.event, text: Localization.Resources.Cargas.Carga.VerSimulacaoFrete, visible: ko.observable(false) });
    this.AguardandoEmissaoDocumentoAnterior = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.bool, def: "", visible: ko.observable(false) });

    this.DadosRoteirizacao = PropertyEntity({ visible: ko.observable(true) });

    this.AgSelecaoRotaOperador = PropertyEntity({ getType: typesKnockout.bool, type: types.map, required: false });
    this.Rota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Rota.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.AlterarRotaFreteCarga = PropertyEntity({ eventClick: AlterarRotaFreteClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Alterar), visible: ko.observable(false), maisDeUmRotaEncontradaText: Localization.Resources.Cargas.Carga.MaisDeUmaRotaEncontrada });
    this.VisualizarRotaMapa = PropertyEntity({ eventClick: VisualizarRotaMapaClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.VisualizarRota), visible: ko.observable(false) });

    this.Carregamento = PropertyEntity({ eventClick: CarregamentoClick, text: Localization.Resources.Cargas.Carga.Carregamento.getFieldDescription(), type: types.entity, codEntity: ko.observable(0) });
    this.ModeloVeicularCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ModeloVeicularDoCarregamento.getFieldDescription() });

    this.ObservacaoCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ObservacaoDoCarregamento.getFieldDescription(), val: ko.observable("") });
    this.AlterarObservacaoCarregamento = PropertyEntity({ eventClick: alterarObservacaoCarregamentoClick, visible: ko.observable(true), type: types.event, text: Localization.Resources.Gerais.Geral.Alterar });

    this.OrdemEmbarque = PropertyEntity({ text: Localization.Resources.Cargas.Carga.OrdemDeEmbarque.getFieldDescription() });
    this.PossuiOrdemEmbarque = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.OrdemEmbarqueDetalhes = PropertyEntity({ eventClick: exibirDetalhesOrdemEmbarqueClick, type: types.event, text: Localization.Resources.Gerais.Geral.VerDetalhes });
    this.OrdemEmbarquePendente = PropertyEntity({ text: Localization.Resources.Cargas.Carga.OrdemDeEmbarque.getFieldDescription() });
    this.ObservacaoTransportador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ObservacaoParaTransportador.getFieldDescription(), type: types.string, maxlength: 1000, required: false, val: ko.observable(""), visible: ko.observable(_ehMultiEmbarcador || _ehTransportador), enable: ko.observable(_ehMultiEmbarcador) });
    this.ObservacaoCarregamentoRoteirizacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ObservacaoCarregamentoRoteirizacao.getFieldDescription(), type: types.string, maxlength: 1000, required: false, val: ko.observable(""), visible: ko.observable(false) });
    this.ObservacaoInformadaPeloTransportador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ObservacaoInformadaPeloTransportador.getFieldDescription(), type: types.string, maxlength: 400, required: false, val: ko.observable(""), visible: ko.observable(true) });
    this.RetornoAE = PropertyEntity({ text: Localization.Resources.Cargas.Carga.RetornoAE.getFieldDescription(), type: types.string, maxlength: 400, required: false, val: ko.observable(""), visible: ko.observable(true) });
    this.VerMotivoSituacaoAE = PropertyEntity({ eventClick: verMotivoSituacaoAEClick, type: types.event, visible: ko.observable(false), text: "Ver Detalhes da AE" });
    this.ExigirInformarContainer = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.QuebraContainer = PropertyEntity({ text: "Quebra de Container", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false), cssClass: ko.observable("text-danger") });
    this.ExigirInformarRetiradaContainer = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NumeroOT = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroOT.getFieldDescription(), type: types.string, maxlength: 120, val: ko.observable(""), visible: ko.observable(true) });
    this.LinhaSeparacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.LinhaSeparacao.getFieldDescription(), val: ko.observable("") });
    this.PendenciaTransportadorContribuinte = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PendenciaValorLimiteApolice = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CargaCritica = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    //Etapa Mercosul
    this.Mercosul = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EmitindoCRT = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PossuiFacturaFake = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.Internacional = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.CargaTipoInformarDadosNotaCte = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    //Etapa Container
    this.PossuiOperacaoContainer = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });


    //Etapa Integração
    this.ModificarTimelineIntegracaoCarga = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    //Etapa Mercante
    this.Mercante = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TodosCTesComMercante = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TodosCTesComManifesto = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    //Etapa MDF-e Aquaviário
    this.MDFeAquaviario = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.OcultarMDFeRodoviario = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PossuiMDFeAquaviarioGeradoMasNaoVinculado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PossuiMDFeAquaviarioRejeitado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PossuiMDFeAquaviarioPendente = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ObrigatoriedadeCIOTEmissaoMDFe = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ProprietarioTAC = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PossuiCIOT = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.VeiculoPropriedadeTerceiro = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    
    this.CIOTGeradoAutomaticamente = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PossuiMDFeAquaviarioAutorizado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.CargaPortoPorto = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CargaPortoPortoPendenciaDocumento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CargaPortoPortoTimelineHabilitado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarTimelineCargaFeeder = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    //Etapa SVM
    this.CargaPortaPortaTimelineHabilitado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PossuiCTePendenteSVM = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PossuiSVMPendenteAutorizacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CargaSVMProprioTimelineHabilitado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    //Etapa Faturamento
    this.Faturamento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TodosCTesForamFaturados = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ContemFaturamento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    //Etapa Integração Faturamento
    this.IntegracaoFaturamento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    //****CTES E AVERBACAO***
    this.DadosCTes = PropertyEntity({ visible: ko.observable(false) });

    //**** ETAPAS ***
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("12.5%") });

    this.OcultarConteudo = PropertyEntity({ type: types.local, eventClick: MinimizarAbasClick, visible: ko.observable(false) });

    this.EtapaInicioEmbarcador = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.DescricaoCarga), type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: etapaInicioEmbarcadorClick,
        step: ko.observable(1),
        icon: ko.observable("fal fa-edit"),
        tooltip: ko.observable(Localization.Resources.Cargas.Carga.OndeSeInformaTipoDeCargaModeloVeicularDeCargaQueIraTransportarMercadoria),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.DescricaoCarga)
    });

    this.EtapaInicioTMS = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.DescricaoCarga), type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: EtapaInicioTMSClick,
        step: ko.observable(1),
        icon: ko.observable("fal fa-edit"),
        tooltip: ko.observable(Localization.Resources.Cargas.Carga.OndeOsDadosDaCargaSaoInformados),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.DadosDaCarga),
        verDetalhesAbaDetalhes: Localization.Resources.Gerais.Geral.Detalhes,
        verDetalhesAbaLacres: Localization.Resources.Cargas.Carga.Lacres,
        verDetalhesAbaIntegracoes: Localization.Resources.Cargas.Carga.Integracoes,
        verPreCheckin: "Pre-Checkin",
        verLacre: Localization.Resources.Cargas.Carga.Lacres,
        verDetalhesAbaObservacao: Localization.Resources.Cargas.Carga.Observacao,
        verDetalhesAbaSequenciaZonaTransporte: Localization.Resources.Cargas.Carga.ZonasDeTransporte,
        verIntegracoesVeiculoMotorista: "Integrações Veículo e Motorista"
    });

    this.EtapaFreteEmbarcador = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.Frete), type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-6 col-lg-6"), idTab: guid(), eventClick: verificarFreteClick,
        step: ko.observable(2),
        icon: ko.observable("fal fa-dollar-sign"),
        tooltip: ko.observable(Localization.Resources.Cargas.Carga.OndeCalculoDoFreteFeitoComBaseNasTabelasDeFretesCadastradas),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.CalculoDeFrete)
    });

    this.EtapaFreteTMS = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.Frete), type: types.local, enable: ko.observable(true), visible: ko.observable(false), idTerceiros: guid(), idGrid: guid(), idTab: guid(), eventClick: verificarFreteClick,
        step: ko.observable(3),
        icon: ko.observable("fal fa-dollar-sign"),
        tooltip: ko.observable(Localization.Resources.Cargas.Carga.OndeCalculoDoFreteFeitoComBaseNasTabelasDeFretesCadastradas),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.CalculoDeFrete)
    });

    this.EtapaDadosTransportador = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.Transportador), type: types.local, enable: ko.observable(true), visible: ko.observable(false), idGrid: guid(), idTab: guid(), eventClick: etapaTransporteClick,
        step: ko.observable(3),
        icon: ko.observable("fal fa-edit"),
        tooltip: ko.observable(Localization.Resources.Cargas.Carga.OndeOsDadosDoTransportadorSaoInformados),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.Transportador)
    });

    this.EtapaContainer = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.Container), type: types.local, enable: ko.observable(true), visible: ko.observable(false), eventClick: LoadEtapaContainer, idGrid: guid(), idTab: guid(),
        step: ko.observable(2),
        icon: ko.observable("fal fa-container-storage"),
        tooltip: ko.observable(Localization.Resources.Cargas.Carga.OndeSaoEmitidosDocumentosContainer),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.Container)
    });

    this.EtapaNotaFiscal = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.NFe), type: types.local, enable: ko.observable(true), visible: ko.observable(false), eventClick: buscarDocumentosClick, idGrid: guid(), idTab: guid(),
        step: ko.observable(4),
        icon: ko.observable("fal fa-barcode-alt"),
        tooltip: ko.observable(Localization.Resources.Cargas.Carga.OndeAsNotasFiscaisDaCargaSaoEnviadas),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.NFe)
    });

    this.EtapaCTeNFs = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.Cte), type: types.local, enable: ko.observable(true), visible: ko.observable(false), eventClick: buscarCTesClick, idGrid: guid(), idTab: guid(),
        step: ko.observable(5),
        icon: ko.observable("fal fa-qrcode"),
        tooltip: ko.observable(Localization.Resources.Cargas.Carga.OndeOsCtesNecessariosParaTransporteSaoEmitidos),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.Cte)
    });

    this.EtapaMDFe = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.Mdfe), type: types.local, enable: ko.observable(true), visible: ko.observable(false), eventClick: buscarMDFeClick, idGrid: guid(), idTab: guid(),
        step: ko.observable(6),
        icon: ko.observable("fal fa-truck"),
        tooltip: ko.observable(Localization.Resources.Cargas.Carga.OndeOsMdfesQuandoNecessariosParaTransporteSaoEmitidos),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.Mdfe)
    });

    this.EtapaCTeFilialEmissora = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.CteFilialEmissora), type: types.local, enable: ko.observable(true), visible: ko.observable(false), eventClick: buscarCTesClick, idGrid: guid(), idTab: guid(),
        step: ko.observable(5),
        icon: ko.observable("fal fa-qrcode"),
        tooltip: ko.observable(Localization.Resources.Cargas.Carga.OndeOsCtesDaFilialEmissoraSaoEmitidosOuImportadosParaOsCtesGerados),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.CteFilialEmissora)
    });

    this.EtapaMDFeFilialEmissora = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.Mdfe), type: types.local, enable: ko.observable(true), visible: ko.observable(false), eventClick: buscarMDFeClick, idGrid: guid(), idTab: guid(),
        step: ko.observable(6),
        icon: ko.observable("fal fa-truck"),
        tooltip: ko.observable(Localization.Resources.Cargas.Carga.OndeOsMdfesQuandoNecessariosParaTransporteSaoEmitidos),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.Mdfe)
    });

    this.EtapaDocumentosMercosul = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.NFe), type: types.local, enable: ko.observable(true), visible: ko.observable(false), eventClick: buscarDocumentosClick, idGrid: guid(), idTab: guid(),
        step: ko.observable(6),
        icon: ko.observable("fal fa-file"),
        tooltip: ko.observable(Localization.Resources.Cargas.Carga.OndeAsNotasFiscaisDaCargaSaoEnviadas),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.NFe)
    });

    this.EtapaIntegracao = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.Integracao), type: types.local, enable: ko.observable(true), visible: ko.observable(false), eventClick: ocultarTodasAbas, idGrid: guid(), idTab: guid(),
        step: ko.observable(7),
        icon: ko.observable("fal fa-link"),
        tooltip: ko.observable(Localization.Resources.Cargas.Carga.DisponibilizacaoDasInformacoesDosDocumentosViaIntegracaoEntreSistemasOuArquivosDeIntegracao),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.IntegracaoDosDocumentos)
    });

    this.EtapaImpressao = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.Impressao), type: types.local, enable: ko.observable(true), visible: ko.observable(false), eventClick: ocultarTodasAbas, idGrid: guid(), idTab: guid(),
        step: ko.observable(8),
        icon: ko.observable("fal fa-print"),
        tooltip: ko.observable(Localization.Resources.Cargas.Carga.ConfirmacaoQueOsDocumentosForamTodosImpresso),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.ImpressaoDosDocumentos)
    });

    this.EtapaIntegracaoFilialEmissora = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.IntegracaoFilialEmissora), type: types.local, enable: ko.observable(true), visible: ko.observable(false), eventClick: ocultarTodasAbas, idGrid: guid(), idTab: guid(),
        step: ko.observable(5),
        icon: ko.observable("fal fa-link"),
        tooltip: ko.observable(Localization.Resources.Cargas.Carga.DisponibilizacaoDasInformacoesDosDocumentosViaIntegracaoEntreSistemasOuArquivosDeIntegracao),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.IntegracaoDosDocumentos)
    });

    this.EtapaSubContratacao = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.ContratoDeFrete), type: types.local, enable: ko.observable(true), visible: ko.observable(false), eventClick: verificarSubContratacaoClick, idGrid: guid(), idTab: guid(),
        step: ko.observable(9),
        icon: ko.observable("fal fa-users"),
        tooltip: ko.observable(Localization.Resources.Cargas.Carga.DisponibilizacaoDasInformacoesDosContratosDeFretesComTerceiros),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.ContratoDeFrete)
    });

    this.EtapaTransbordo = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.Transbordo), type: types.local, enable: ko.observable(true), visible: ko.observable(false), eventClick: verificarTransbordoClick, idGrid: guid(), idTab: guid(),
        step: ko.observable(10),
        icon: ko.observable("fal fa-truck"),
        tooltip: ko.observable(Localization.Resources.Cargas.Carga.OndePodeSerFeitoTransbordoDaMercadoria),
        tooltipTitle: ko.observable(Localization.Resources.Cargas.Carga.Transbordo)
    });

    this.EtapaMercante = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.Mercante), type: types.local, enable: ko.observable(true), visible: ko.observable(false), eventClick: buscarDadosMercanteClick, idGrid: guid(), idTab: guid(),
        step: ko.observable(7),
        icon: ko.observable("fal fa-ship"),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("")
    });

    this.EtapaMDFeAquaviario = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.Mdfe), type: types.local, enable: ko.observable(true), visible: ko.observable(false), eventClick: buscarMDFeClick, idGrid: guid(), idTab: guid(),
        step: ko.observable(6),
        icon: ko.observable("fal fa-file-alt"),
        tooltip: ko.observable(),
        tooltipTitle: ko.observable()
    });

    this.EtapaSVM = PropertyEntity({
        text: ko.observable("SVM"), type: types.local, enable: ko.observable(true), visible: ko.observable(false), eventClick: buscarDadosSVMClick, idGrid: guid(), idTab: guid(),
        step: ko.observable(6),
        icon: ko.observable("fal fa-ship"),
        tooltip: ko.observable(),
        tooltipTitle: ko.observable()
    });

    this.EtapaFaturamento = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.Faturamento), type: types.local, enable: ko.observable(true), visible: ko.observable(false), eventClick: buscarDadosFaturamentoClick, idGrid: guid(), idTab: guid(),
        step: ko.observable(8),
        icon: ko.observable("fal fa-search-dollar"),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("")
    });

    this.EtapaIntegracaoFaturamento = PropertyEntity({
        text: ko.observable(Localization.Resources.Cargas.Carga.IntegracaoFaturamento), type: types.local, enable: ko.observable(true), visible: ko.observable(false), eventClick: buscarDadosIntegracaoFaturamentoClick, idGrid: guid(), idTab: guid(),
        step: ko.observable(9),
        icon: ko.observable("fal fa-link"),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("")
    });

    this.AlterarMoedaCarga = PropertyEntity({ eventClick: AbrirTelaAlteracaoMoedaCarga, enable: ko.observable(true), visible: ko.observable(false), type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.AlterarMoedaDaCarga) });
    this.RetornarParaEtapaNFeTMS = PropertyEntity({ eventClick: retornarParaEtapaNFeClick, enable: ko.observable(true), visible: ko.observable(false), type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.RetornarParaNfe) });
    this.RetornarEtapaNFe = PropertyEntity({ eventClick: RetornarEtapaNFeClick, enable: ko.observable(true), visible: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.Carga.RetornarParaNfe });
    this.BuscarNFesEmillenium = PropertyEntity({ eventClick: BuscarNFesEmilleniumClick, enable: ko.observable(true), visible: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.Carga.BuscarNFesEmillenium });
    this.LiberarEmissaoSemNF = PropertyEntity({ eventClick: LiberarEmissaoSemNFClick, enable: ko.observable(true), visible: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.Carga.LiberarEmissaoSemNFe });
    this.LiberarEmissaoSemIntegracaoEtapaTransportador = PropertyEntity({ eventClick: LiberarEmissaoSemIntegracaoEtapaTransportadorClick, enable: ko.observable(true), visible: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.Carga.LiberarSemIntegracaoEtapaTransportador });
    this.CargaDesabilitada = PropertyEntity({ enable: ko.observable(true) });
    this.EtapaDadosTransportadorTranportadoraSugerida = PropertyEntity({ visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.TransportadorasSugeridas });
    this.EtapaDadosTransportadorPreCheckin = PropertyEntity({ visible: ko.observable(false), text: "Pre-Checkin", });
    this.DownloadTodosXmlNotasFiscais = PropertyEntity({ eventClick: downloadTodosXmlNotasFiscaisClick, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.PermitirDownloadXmlEtapaNfe), type: types.event, text: Localization.Resources.Cargas.Carga.DownloadXmlDasNotas });
    this.ObrigarInformarRICnaColetaDeConteiner = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ConferenciaDeFrete = PropertyEntity({ eventClick: conferenciaDeFreteClick, enable: ko.observable(true), visible: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.Carga.ConferenciaDeFrete });
    this.AnexosEtapaNF = PropertyEntity({ eventClick: MostrarModalAnexoEtapaNFe, type: types.event, text: Localization.Resources.Gerais.Geral.Anexos, visible: ko.observable(true) });

    var confirmarImpressao = false;
    if (_CONFIGURACAO_TMS.PermitirConfirmacaoImpressaoME || _operadorLogistica.OperadorSupervisor)
        confirmarImpressao = true;

    //**** IMPRESSÃO ***

    this.PossuiConfiguracaoImpressora = PropertyEntity({ val: ko.observable(false), idGrid: guid() });
    this.SaldoInsuficienteContratoFreteCliente = PropertyEntity({ val: ko.observable(false) });
    this.CanhotoAvulso = PropertyEntity({ visible: ko.observable(false), idGrid: guid() });
    this.ConfirmarImpressao = PropertyEntity({
        val: _CONFIGURACAO_TMS.PermitirConfirmacaoImpressaoME == true ? Localization.Resources.Cargas.Carga.NecessarioConfirmacaoDeQueOsDocumentosForamImpressoFacaIssoClicandoNoBotaoAbaixo : Localization.Resources.Cargas.Carga.AguardandoConfirmacaoDaImpressaoDosDocumentos,
        eventClick: confirmarImpressaoClick, type: types.event,
        text: Localization.Resources.Cargas.Carga.ConfirmarImpressaoDosDocumentos, visible: ko.observable(confirmarImpressao), enable: ko.observable(false)
    });

    //this.EnviarParaImpressao = PropertyEntity({
    //    eventClick: enviarParaImpressaoClick, type: types.event,
    //    text: "Enviar documentos para Impressão", visible: ko.observable(confirmarImpressao), visible: ko.observable(false), enable: ko.observable(true)
    //});

    this.EnviarNotasBoletos = PropertyEntity({ eventClick: EnviarNotasBoletosClick, type: types.event, text: Localization.Resources.Cargas.Carga.NotasBoletos, visible: ko.observable(false) });
    this.EnviarCTes = PropertyEntity({ eventClick: EnviarCTesClick, type: types.event, text: Localization.Resources.Cargas.Carga.Ctes, visible: ko.observable(false) });
    this.EnviarMDFes = PropertyEntity({ eventClick: EnviarMDFesClick, type: types.event, text: Localization.Resources.Cargas.Carga.Mdfes, visible: ko.observable(false) });
    this.AnexosCarga = PropertyEntity({ eventClick: AnexosCargaClick, type: types.event, text: Localization.Resources.Cargas.Carga.Anexos, visible: ko.observable(true) });

    this.RelatorioDeTroca = PropertyEntity({ eventClick: RelatorioDeTrocaClick, type: types.event, text: Localization.Resources.Cargas.Carga.RelatorioTroca, visible: ko.observable(false) });
    this.RelatorioDeEntrega = PropertyEntity({ eventClick: RelatorioDeEntregaClick, type: types.event, text: Localization.Resources.Cargas.Carga.RelatorioEntrega, visible: ko.observable(false) });
    this.RelatorioBoletimViagem = PropertyEntity({ eventClick: RelatorioBoletimViagem, type: types.event, text: Localization.Resources.Cargas.Carga.BoletimDeViagem, visible: ko.observable(false) });
    this.RelatorioDiarioBordo = PropertyEntity({ eventClick: relatorioDiarioBordoClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.DiarioDeBordo), visible: ko.observable(false) });
    this.RelatorioPlanoViagem = PropertyEntity({ eventClick: relatorioPlanoViagemClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.PlanoDeViagem), visible: ko.observable(false) });
    this.DownloadNFeBoleto = PropertyEntity({ eventClick: DownloadNFeBoletoClick, type: types.event, text: Localization.Resources.Cargas.Carga.DownloadNotasBoletos, visible: ko.observable(false) });
    this.RelatorioDeRomaneio = PropertyEntity({ eventClick: RelatorioDeRomaneioClick, type: types.event, text: Localization.Resources.Cargas.Carga.RelatorioDeRomaneio, visible: ko.observable(false) });
    this.RelatorioPedidoPacote = PropertyEntity({ eventClick: RelatorioPedidoPacoteClick, type: types.event, text: Localization.Resources.Cargas.Carga.RelatorioPedidoPacote, visible: ko.observable(false) });
    this.ImprimirCRT = PropertyEntity({ eventClick: ImprimirCRTClick, type: types.event, text: Localization.Resources.Cargas.Carga.ImprimirCRT, visible: ko.observable(false) });
    this.RelatorioDeEmbarque = PropertyEntity({ eventClick: RelatorioDeEmbarqueClick, type: types.event, text: Localization.Resources.Cargas.Carga.RelatorioDeEmbarque, visible: ko.observable(false), val: ko.observable(true) });
    this.ObservacaoRelatorioDeEmbarque = PropertyEntity({ eventClick: exibirObservacaoDoRelatorioEmbarque, type: types.event, text: Localization.Resources.Cargas.Carga.ObservacaoRelatorioDeEmbarque, visible: ko.observable(true) });

    this.DownloadLotePDF = PropertyEntity({
        eventClick: DownloadLotePDFClick, type: types.event, text: Localization.Resources.Cargas.Carga.PdfDosDocumentos, idGrid: guid(), visible: ko.observable(true)
    });

    this.DownloadDocumentosZIP = PropertyEntity({
        eventClick: DownloadDocumentosZIPClick, type: types.event, text: Localization.Resources.Cargas.Carga.DownloadDocumentosArquivoCompactado, idGrid: guid(), visible: ko.observable(true)
    });

    // Detalhes
    this.AuditarCarga = PropertyEntity({ eventClick: exibirAuditoriaCargaClick, type: types.event, text: Localization.Resources.Cargas.Carga.AuditoriaCarga });
    this.AuditarJanelaCarregamento = PropertyEntity({ eventClick: exibirAuditoriaJanelaCarregamentoClick, type: types.event, text: Localization.Resources.Cargas.Carga.AuditoriaJanelaDeCarregamento, visible: ko.observable(false) });
    this.AuditarJanelaDescarregamento = PropertyEntity({ eventClick: exibirAuditoriaJanelaDescarregamentoClick, type: types.event, text: Localization.Resources.Cargas.Carga.AuditoriaJanelaDeDescarregamento, visible: ko.observable(false) });
    this.AuditarCargaVeiculoContainer = PropertyEntity({ eventClick: exibirAuditoriaCargaVeiculoContainerClick, type: types.event, text: Localization.Resources.Cargas.Carga.AuditoriaAnexos, visible: ko.observable(false) });
    this.DetalhesCarga = PropertyEntity({ eventClick: exibirDetalhesCargaClick, type: types.event, text: Localization.Resources.Cargas.Carga.DetalhesDosPedidos, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe ? !_CONFIGURACAO_TMS.NaoHabilitarDetalhesCarga : true) });

    // Opções
    this.CancelarCarga = PropertyEntity({ eventClick: cancelarCargaClick, type: types.event, text: Localization.Resources.Cargas.Carga.CancelarCarga, visible: ko.observable(false) });
    this.ConsultarPacotes = PropertyEntity({ eventClick: function (e, sender) { consultarPacotesClick(e, sender, true) }, type: types.event, text: Localization.Resources.Cargas.Carga.ConsultarPacotes, visible: ko.observable(false) });
    this.ImprimirMinuta = PropertyEntity({ eventClick: imprimirMinutaClick, type: types.event, text: Localization.Resources.Cargas.Carga.ImprimirMinuta, visible: ko.observable(false) });

    this.ImagemOnlineOffline = ko.computed(() => {

        let hover = EnumStatusAcompanhamento.obterDescricao(this.RastreadorOnlineOffline.val() ?? 1);
        if (this.RastreadorOnlineOffline.val() == 0)
            return '<div class="no-signal" title="' + hover + '"></div>';

        let icone = ObterIconeStatusTracking(this.RastreadorOnlineOffline.val(), 20);
        hover = EnumStatusAcompanhamento.obterDescricao(this.RastreadorOnlineOffline.val());

        return '<div class="mutable" title="' + hover + '">' + icone + '</div>';
    }, this)

    this.Opcoes = PropertyEntity({
        text: Localization.Resources.Gerais.Geral.Opcoes, visible: ko.computed(function () {
            return (
                this.CancelarCarga.visible() ||
                this.ConsultarPacotes.visible() ||
                this.ImprimirMinuta.visible()
            );
        }, this)
    });
};

var AlterarTipoPagamentoValePedagio = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoPagamentoValePedagio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoPagamentoValePedagio, val: ko.observable(EnumTipoPagamentoValePedagio.Cartao), options: EnumTipoPagamentoValePedagio.obterOpcoes(), enable: ko.observable(true) });

    this.ConfirmarAlteracao = PropertyEntity({ type: types.event, eventClick: confirmarAlterarTipoPagamentoValePedagioClick, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
}

function ocultarTodasEtapasClick(e) {

}

//*******EVENTOS*******

var _HTMLCarga;
var _HTMLPendenciasCarga;
var _HTMLIntegracaoCarga;
var _HTMLIntegracaoFaturamento;
var _HTMLIntegracaoCargaTransportador;
var _HTMLOperacaoContainer;
function loadCarga() {
    buscarDetalhesOperador(function () {
        obterConfiguracoesIntegracaoCarga(function () {

            _pesquisaCarga = new PesquisaCarga();
            KoBindings(_pesquisaCarga, "knotPesquisa", false, _pesquisaCarga.Pesquisar.id);

            $("#knotPesquisa").show();

            $("#" + _pesquisaCarga.RaizCNPJ.id).mask("00.000.000", { selectOnFocus: true, clearIfNotMatch: true });

            new BuscarTransportadores(_pesquisaCarga.Empresa, null, null, true);
            new BuscarVeiculos(_pesquisaCarga.Veiculo, null, null, null, null, null, null, null, false);
            new BuscarMotoristas(_pesquisaCarga.Motorista, null, null, null, null, null, null, null, null, true);
            new BuscarTiposdeCarga(_pesquisaCarga.TipoCarga);
            new BuscarTiposOperacao(_pesquisaCarga.TipoOperacao, null, _pesquisaCarga.GrupoPessoa, null, null, null, null, null, null, null, null, true);
            new BuscarModelosVeicularesCarga(_pesquisaCarga.ModeloVeicularCarga);
            new BuscarPedidoViagemNavio(_pesquisaCarga.PedidoViagemNavio);
            new BuscarPedidoEmpresaResponsavel(_pesquisaCarga.PedidoEmpresaResponsavel);
            new BuscarPedidoCentroCusto(_pesquisaCarga.PedidoCentroCusto);
            new BuscarContainers(_pesquisaCarga.Container);
            new BuscarContainers(_pesquisaCarga.ContainerTMS);
            new BuscarPorto(_pesquisaCarga.PortoOrigem);
            new BuscarPorto(_pesquisaCarga.PortoDestino);
            new BuscarFuncionario(_pesquisaCarga.FuncionarioVendedor);
            new BuscarCanaisEntrega(_pesquisaCarga.CanalEntrega);
            new BuscarCanaisVenda(_pesquisaCarga.CanalVenda);
            new BuscarRotasFrete(_pesquisaCarga.Rota);


            new BuscarFilial(_pesquisaCarga.Filial);
            new BuscarFilial(_pesquisaCarga.FilialVenda);
            new BuscarRegioes(_pesquisaCarga.Regiao);
            new BuscarMesoRegiao(_pesquisaCarga.Mesorregiao);


            if (_CONFIGURACAO_TMS.TipoMontagemCargaPadrao === EnumTipoMontagemCarga.AgruparCargas)
                new BuscarCarregamento(_pesquisaCarga.Carregamento, null, null, null, _CONFIGURACAO_TMS.TipoMontagemCargaPadrao);
            else
                new BuscarCarregamento(_pesquisaCarga.Carregamento, null, null);

            new BuscarGruposPessoas(_pesquisaCarga.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarClientes(_pesquisaCarga.Remetente);
            new BuscarClientes(_pesquisaCarga.Destinatario);
            new BuscarClientes(_pesquisaCarga.Expedidor);
            new BuscarClientes(_pesquisaCarga.Recebedor);
            new BuscarOperador(_pesquisaCarga.Operador);
            new BuscarLocalidades(_pesquisaCarga.Origem, Localization.Resources.Cargas.Carga.BuscarCidadeDeOrigem, Localization.Resources.Cargas.Carga.CidadesDeOrigem);
            new BuscarLocalidades(_pesquisaCarga.Destino, Localization.Resources.Cargas.Carga.BuscarCidadeDeDestino, Localization.Resources.Cargas.Carga.CidadesDeDestino);
            new BuscarEstados(_pesquisaCarga.EstadoOrigem);
            new BuscarEstados(_pesquisaCarga.EstadoDestino);
            new BuscarCargasIntegracaoEmbarcador(_pesquisaCarga.CargaIntegracaoEmbarcador, null, null, true);
            new BuscarClientes(_pesquisaCarga.TransportadorTerceiro, null, false, [EnumModalidadePessoa.TransportadorTerceiro]);
            new BuscarModeloDocumentoFiscal(_pesquisaCarga.ModeloDocumentoFiscal, null, null, null, null, true);
            new BuscarOperador(_pesquisaCarga.OperadorInsercao);
            new BuscarCIOT(_pesquisaCarga.CIOT);
            new BuscarCentroResultado(_pesquisaCarga.CentroResultado);

            if (!_RequisicaoIniciada)
                iniciarRequisicao();

            if (_CONFIGURACAO_TMS.PermitirVincularVeiculoMotoristaViaPlanilha)
                _pesquisaCarga.VincularVeiculoMotorista.visible(true);

            if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
                _pesquisaCarga.FormaIntegracaoNotas.visible(false);

            configurarCamposIntegracaoCarga();
            AuditoriaCarga();
            LoadConexaoSignalRCarga();
            LoadConexaoSignalRMinutaAvon();
            LoadConexaoSignalRMercadoLivre();
            loadControleSaldo();
            loadDetalhePedido();
            loadAceiteTermoTransportador();
            loadObservacaoRelatorioEmbarque();

            ObterTiposIntegracaoCTe();
            ObterTiposIntegracaoCarga();
            obterTiposIntegracaoCargaTransportador();

            carregarConteudosHTML(function () {
                $('#FileEnviarCTeDoPreCTe').change(EnviarCTeDoPreCTeClick);
                $('#FileEnviarCTeDoPreCTeContrato').change(EnviarCTeDoPreCTeContratoClick);

                _pesquisaCarga.Codigo.val(_notificacaoGlobal.CodigoObjeto.val());
                _notificacaoGlobal.CodigoObjeto.val(0);

                buscarCargas(1, false);

                loadLiberacaoSemIntegracaoGR();
                LoadCargaTabelaTerceiroValor();
                loadDetalheCargaAprovacaoFrete();
                loadSolicitacaoFrete();
                loadOrdemEmbarque();
                loadCargaRetiradaContainer();
                loadCargaAgrupadaDados();
                loadSimulacaoFreteDetalheCarga();
                LoadDetalhesSituacaoAE();
                LoadCTes();
            });
        });
    });
}

function obterConfiguracoesIntegracaoCarga(callback) {
    executarReST("Carga/ObterConfiguracoesIntegracao", null, function (retorno) {
        if (retorno.Success) {
            _configuracoesIntegracaoCarga = retorno.Data;

            if (_configuracoesIntegracaoCarga.AjustarLayoutFiltrosTelaCargaIntercab) {
                $.get("Content/Static/Carga/PesquisaCargaMercante.html?dyn=" + guid(), function (dataHtml) {
                    $("#idPesquisaCarga").html(dataHtml);
                    callback();
                });
                return;
            }

            callback();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function configurarCamposIntegracaoCarga() {
    if (_configuracoesIntegracaoCarga.AtivarPreFiltrosTelaCargaIntercab || _configuracoesIntegracaoCarga.AtivarNovosFiltrosConsultaCargaIntercab) {
        _pesquisaCarga.Situacoes.visible(false);
        _pesquisaCarga.Situacoes.val(EnumSituacoesCarga.Todas);
        _pesquisaCarga.Situacoes.def = EnumSituacoesCarga.Todas;

        _pesquisaCarga.SituacaoCargaMercante.visible(true);
    }

    if (_configuracoesIntegracaoCarga.AtivarPreFiltrosTelaCargaIntercab) {
        if (_configuracoesIntegracaoCarga.QuantidadeDiasParaDataInicialIntercab > 0)
            _pesquisaCarga.DataInicio.val(Global.Data(EnumTipoOperacaoDate.Subtract, _configuracoesIntegracaoCarga.QuantidadeDiasParaDataInicialIntercab, EnumTipoOperacaoObjetoDate.Days));

        _pesquisaCarga.SituacaoCargaMercante.val(_configuracoesIntegracaoCarga.SituacoesCargaIntercab);
        $("#" + _pesquisaCarga.SituacaoCargaMercante.id).trigger("change");
    }
}

function limparFiltrosConsultaCarga() {
    LimparCampos(_pesquisaCarga);
}

function atualizarCargasClick(e, sender) {
    sender.stopPropagation();

    AtualizarDadosControleSaldo();
    buscarCargas(1, false);
}

function BuscarPermissoesEdicaoCTe(e, sender) {
    executarReST("CargaCTe/BuscarPermissoesCargaCTes", null, function (arg) {
        _PermissoesEdicaoDoCTe = arg.Data;
    });
}

function AlterarFaixaTemperaturaClick(carga) {
    carga.FaixaTemperatura.entityDescription(carga.FaixaTemperatura.val());
    var buscaFaixaTemperatura = new BuscarFaixaTemperatura(carga.FaixaTemperatura, function (faixaTemperatura) {
        executarReST("Carga/AlterarFaixaTemperatura", { Carga: carga.Codigo.val(), FaixaTemperatura: faixaTemperatura.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    carga.FaixaTemperatura.val(faixaTemperatura.Descricao);
                    carga.FaixaTemperatura.codEntity(faixaTemperatura.Codigo);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);

    }, carga.TipoOperacao, carga.Codigo.val());

    buscaFaixaTemperatura.AbrirBusca();
}

function exibirDetalhesCargaClick(cargaSelecionada) {
    _cargaAtual = cargaSelecionada;
    _detalhePedidoContainer.ExibirValorUnitarioDoProduto.val(_cargaAtual.ExibirValorUnitarioDoProduto.val());
    exibirDetalhesPedidos(cargaSelecionada.Codigo.val());
}

function detalhesCargaPedidosClick() {
    exibirDetalhesPedidosPorPedidos(cargaSelecionada.Codigo.val());
}

function ControleVeiculosCheckList(cargaSelecionada) {
    executarDownload("Carga/ControleVeiculosCheckList", { Codigo: cargaSelecionada.Codigo.val() });
}

function ControleDivisoesCapacidadeClick(cargaSelecionada) {
    executarDownload("CargaAgrupada/RelatorioControleDivisoesCapacidade", { Codigo: cargaSelecionada.Codigo.val() });
}

function adicionarAnexosCargaGuaritaClick(e) {
    AnexosCargaClick(e);
}

function cancelarCargaClick(cargaSelecionada) {
    CODIGO_CARGA_PARA_CANCELAMENTO_TELA_CARGA = cargaSelecionada.Codigo.val();
    var origin = window.location.origin;
    var win = window.open(origin + "/#Cargas/CancelamentoCarga?cid=" + CODIGO_CARGA_PARA_CANCELAMENTO_TELA_CARGA, '_blank');
    if (win)
        win.focus();

    //location.pathname = "";
    //location.href = "/#Cargas/CancelamentoCarga";
}

//*******MÉTODOS*******

function scrollToAnchor(aid) {
    var aTag = $("#" + aid);
    $('html,body').animate({ scrollTop: aTag.offset().top }, 'slow');
}

function ocultarAbas(e) {
    //    $("#fdsCargas .tab-pane.active").attr("class", "tab-pane");
    //    $("#fdsCargas li.active").removeAttr("class");
}

function redirecionarParaSaibaMaisSugestaoHUB(e) {
    // TODO: Precisa de revisão em retorno do cliente sobre Integração HUB
    clarity("set", "botão_saiba_mais_sugestaoCarga", "saiba_mais");
    const externalUrl = "https://google.com.br/";
    window.open(externalUrl, '_blank');
}

function ocultarTodasAbas(e) {
    ocultarAbas(e);

    if (e.ScrollTo.val() !== false)
        scrollToAnchor(e.DivCarga.id);

    $(".iconeMinimizarCargas").hide();
    $("#" + e.OcultarConteudo.id).show();
}

function MinimizarAbasClick(e) {
    ocultarAbas(e);
    $("#" + e.OcultarConteudo.id).hide();
}

function DownloadDocumentosCargaClick(e, sender) {
    var data = RetornarObjetoPesquisa(_pesquisaCarga);
    executarReST("Carga/DownloadLoteDocumentos", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SolicitacaoRelizadaComSucessoFavorAguardeArquivoSerGerado, 20000);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.falha, arg.Msg);
        }
    });
}

function buscarCargas(page, paginou, callback) {
    _configuracaoEmissaoCTe = null;
    _gridCompentesDeFrete = null;
    validarEntidadesPesquisa();
    carregarFiltrosPesquisaInicialCarga();

    var itensPorPagina = 10;

    if (CODIGO_CARGA_PESQUISA_TELA_CARGA > 0)
        _pesquisaCarga.CodigoCarga.val(CODIGO_CARGA_PESQUISA_TELA_CARGA);

    var data = RetornarObjetoPesquisa(_pesquisaCarga);

    data["inicio"] = itensPorPagina * (page - 1);
    data["limite"] = itensPorPagina;
    executarReST("Carga/PesquisaCargas", data, function (e) {
        _pesquisaCarga.Codigo.val(0);
        if (e.Success) {
            _pesquisaCarga.ExibirFiltros.visibleFade(false);
            _listaKnoutsCarga = new Array();
            $("#fdsCargas").html("");

            $.each(e.Data, function (i, carga) {
                var knoutCarga = GerarTagHTMLDaCarga("fdsCargas", carga, null, false);
                _listaKnoutsCarga.push(knoutCarga);
            });

            LocalizeCurrentPage();

            if (!paginou) {
                if (e.QuantidadeRegistros > 0) {
                    $("#divPagination").html('<ul style="float:right" id="paginacao" class="pagination"></ul>');
                    var paginas = Math.ceil((e.QuantidadeRegistros / itensPorPagina));
                    $('#paginacao').twbsPagination({
                        first: Localization.Resources.Cargas.Carga.Primeiro,
                        prev: Localization.Resources.Cargas.Carga.Anterior,
                        next: Localization.Resources.Cargas.Carga.Proximo,
                        last: Localization.Resources.Cargas.Carga.Ultimo,
                        totalPages: paginas,
                        visiblePages: 5,
                        onPageClick: function (event, pg) {
                            if (pg != page) {
                                buscarCargas(pg, true);
                                page = pg;
                            }
                        }
                    });
                } else {
                    $("#divPagination").html('<div class="pesquisa-carga-sem-registros"><span>' + Localization.Resources.Gerais.Geral.NenhumRegistroEncontrado + '</span></div>');
                }
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, e.Msg);
        }
    });

}

function exibirMensagemAlertaCargaClick(e) {
    e.ExibirMensagensAlerta.visibleFade(!e.ExibirMensagensAlerta.visibleFade());
}

function confirmarMensagemAlertaCargaClick(carga, mensagemAlerta) {
    var mensagemConfirmacao = Localization.Resources.Cargas.Carga.DesejaRealmenteConfirmarLeituraDaMensagemDeAlerta;

    if (mensagemAlerta.Tipo == EnumTipoMensagemAlerta.AlteracaoDadosPreCarga)
        mensagemConfirmacao = Localization.Resources.Cargas.Carga.DesejaRealmenteConfirmarAsAlteracoesDaPreCarga;

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, mensagemConfirmacao, function () {
        executarReST("Carga/ConfirmarMensagemAlerta", { Codigo: mensagemAlerta.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    var mensagensAlerta = carga.MensagensAlerta.val();

                    for (var i = 0; i < mensagensAlerta.length; i++) {
                        if (mensagensAlerta[i].Codigo == mensagemAlerta.Codigo)
                            mensagensAlerta.splice(i, 1);
                    }

                    carga.MensagensAlerta.val(mensagensAlerta);
                    carga.MensagemAlertaComBloqueio.val(isPossuiMensagemAlertaComBloqueio(mensagensAlerta));
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function exibirDetalhesReboqueClick(cargaSelecionada) {
    exibirCargaVeiculoContainerAnexo(cargaSelecionada.ContainerReboqueAnexo, cargaSelecionada.CodigoContainerReboque.val());
}

function exibirDetalhesSegundoReboqueClick(cargaSelecionada) {
    exibirCargaVeiculoContainerAnexo(cargaSelecionada.ContainerSegundoReboqueAnexo, cargaSelecionada.CodigoContainerSegundoReboque.val());
}

function exibirDetalhesTerceiroReboqueClick(cargaSelecionada) {
    exibirCargaVeiculoContainerAnexo(cargaSelecionada.ContainerTerceiroReboqueAnexo, cargaSelecionada.CodigoContainerTerceiroReboque.val());
}

function exibirDetalhesOrdemEmbarqueClick(cargaSelecionada) {
    exibirDetalhesOrdemEmbarque(cargaSelecionada);
}

function exibirDetalhesVeiculoClick(cargaSelecionada) {
    exibirCargaVeiculoContainerAnexo(cargaSelecionada.ContainerVeiculoAnexo, cargaSelecionada.CodigoContainerVeiculo.val());
}

function GerarTagHTMLDaCarga(idElemento, carga, scrollTo, localize) {
    var knoutCarga = new Carga();

    knoutCarga.DivCarga.id = knoutCarga.Codigo.idGrid;
    knoutCarga.ScrollTo.val(scrollTo);
    
    $("#" + idElemento).append("<div id='conteudo_" + knoutCarga.DivCarga.id + "'></div>");

    preencherDadosCarga(knoutCarga, carga, localize);

    if (!_detalhePedidoContainer)
        loadDetalhePedido();

    if (!_cadastroCargaVeiculoContainerAnexo)
        loadCargaVeiculoContainerAnexo();

    if (!_ordemEmbarqueContainer)
        loadOrdemEmbarque();

    if (!_cargaRetiradaContainer)
        loadCargaRetiradaContainer();

    if (!_cargaAgrupadaDadosContainer)
        loadCargaAgrupadaDados();

    if (!_liberacaoSemIntegracaoGR)
        loadLiberacaoSemIntegracaoGR();

    if (!string.IsNullOrWhiteSpace(_CONFIGURACAO_TMS.DocumentoImpressaoPadraoCarga))
        knoutCarga.ControleVeiculosCheckList.visible(true);

    if (_CONFIGURACAO_TMS.PermitirAgrupamentoDeCargasOrdenavel)
        knoutCarga.ControleDivisoesCapacidade.visible(true);

    $("#" + knoutCarga.GensetVeiculo.id).mask("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", { selectOnFocus: true, optional: true });
    $("#" + knoutCarga.GensetReboque.id).mask("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", { selectOnFocus: true, optional: true });
    $("#" + knoutCarga.GensetSegundoReboque.id).mask("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", { selectOnFocus: true, optional: true });
    $("#" + knoutCarga.GensetTerceiroReboque.id).mask("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", { selectOnFocus: true, optional: true });

    return knoutCarga;
}

function exibirEtapaCarga(idElemento, carga, etapa) {
    var knoutCarga = new Carga();

    knoutCarga.DivCarga.id = knoutCarga.Codigo.idGrid;

    $("#" + idElemento).html('');
    $("#" + idElemento).append("<div id='conteudo_" + knoutCarga.DivCarga.id + "' class='d-none'></div>");

    preencherDadosCarga(knoutCarga, carga);

    if (!_detalhePedidoContainer)
        loadDetalhePedido();

    if (!_cadastroCargaVeiculoContainerAnexo)
        loadCargaVeiculoContainerAnexo();

    if (!_ordemEmbarqueContainer)
        loadOrdemEmbarque();

    if (!_cargaRetiradaContainer)
        loadCargaRetiradaContainer();

    if (!_cargaAgrupadaDadosContainer)
        loadCargaAgrupadaDados();

    $("#" + knoutCarga.DivCarga.id + '_cabecalho').hide();
    $('#' + knoutCarga.DivCarga.id).removeAttr("style");
    $("#" + knoutCarga.DivCarga.id + '_container_interno').addClass("fieldset-no-padding");
    $('#conteudo_' + knoutCarga.DivCarga.id).removeClass("d-none");
    $("#" + knoutCarga[EnumEtapaCarga.obterNomeEtapa(etapa)].idGrid).click();
    $('#' + knoutCarga.DivCarga.id).removeClass();

    $("#" + knoutCarga.GensetVeiculo.id).mask("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", { selectOnFocus: true, optional: true });
    $("#" + knoutCarga.GensetReboque.id).mask("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", { selectOnFocus: true, optional: true });
    $("#" + knoutCarga.GensetSegundoReboque.id).mask("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", { selectOnFocus: true, optional: true });
    $("#" + knoutCarga.GensetTerceiroReboque.id).mask("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", { selectOnFocus: true, optional: true });

    return knoutCarga;
}

function VerificarSeCargaEstaNaLogistica(knoutCarga) {
    var situacao = knoutCarga.SituacaoCarga.val();
    if (situacao == EnumSituacoesCarga.Nova || situacao == EnumSituacoesCarga.CalculoFrete || situacao == EnumSituacoesCarga.AgTransportador) {
        if (situacao == EnumSituacoesCarga.CalculoFrete) {
            if (!knoutCarga.EmissaoDocumentosAutorizada.val())
                return true;
            else
                return false;
        } else {
            return true;
        }
    }
    else {
        if (knoutCarga.ExigeNotaFiscalParaCalcularFrete.val() && situacao == EnumSituacoesCarga.AgNFe)
            return true;
        else
            return false;
    }
}

function PreecherInformacaoValorFrete(knoutCarga, valor) {
    knoutCarga.ValorFrete.val(Globalize.format(valor, "n2"))
    knoutCarga.ValorFreteTabelaFrete.val(valor);

    if (valor > 0) {
        knoutCarga.VerDetalhesFreteCarga.visible(true);

        if (knoutCarga.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Operador) {
            knoutCarga.InfoTipoFreteEscolhido.val(Localization.Resources.Cargas.Carga.ValorInformadoPeloOperador);
            knoutCarga.InfoTipoFreteEscolhido.visible(true);
        }
        else if (knoutCarga.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Embarcador) {
            knoutCarga.InfoTipoFreteEscolhido.val(Localization.Resources.Cargas.Carga.ValorInformadoPeloEmbarcador);
            knoutCarga.InfoTipoFreteEscolhido.visible(true);
        }
        else
            knoutCarga.InfoTipoFreteEscolhido.visible(false);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
            if ((knoutCarga.SituacaoCarga.val() == EnumSituacoesCarga.Nova || knoutCarga.SituacaoCarga.val()) == EnumSituacoesCarga.AgNFe) {
                knoutCarga.InfoTipoFreteEscolhido.val(Localization.Resources.Cargas.Carga.PreviaDoFrete);
                knoutCarga.InfoTipoFreteEscolhido.visible(true);
            }
        }

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
            knoutCarga.InfoTipoFreteEscolhido.visible(!knoutCarga.TipoOperacao.naoExibirDetalhesDoFretePortalTransportador);
            knoutCarga.VerDetalhesFreteCarga.visible(!knoutCarga.TipoOperacao.naoExibirDetalhesDoFretePortalTransportador);
        }

        if (knoutCarga.TipoOperacao.PermiteRealizarImpressaoCarga && _CONFIGURACAO_TMS.RelatorioEntregaPorPedido) {
            knoutCarga.RelacaoEntrega.visible(true);
        }

    }
    else if (knoutCarga.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Cliente) {
        knoutCarga.InfoTipoFreteEscolhido.val(Localization.Resources.Cargas.Carga.FretePorContaDoCliente);
        knoutCarga.InfoTipoFreteEscolhido.visible(true);
        knoutCarga.ValorFrete.val("");
    }
    else if (knoutCarga.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Embarcador && knoutCarga.TipoOperacao.permitirValorFreteInformadoPeloEmbarcadorZerado) {
        knoutCarga.InfoTipoFreteEscolhido.val(Localization.Resources.Cargas.Carga.ValorInformadoPeloEmbarcador);
        knoutCarga.InfoTipoFreteEscolhido.visible(true);
    }
    else {
        knoutCarga.VerDetalhesFreteCarga.visible(false);
        knoutCarga.InfoTipoFreteEscolhido.visible(false);
        knoutCarga.ValorFrete.val(Localization.Resources.Cargas.Carga.Pendente);
    }
}

function IniciarBindKnoutCarga(knoutCarga, carga) {
    const objetosEstender = ['Configuracoes', 'Validacoes'];

    for (let c = 0; c < objetosEstender.length; c++)
        $.extend(carga, carga[objetosEstender[c]]);

    knoutCarga.PermiteInformarQuantidadePaletes.val(carga.PermiteInformarQuantidadePaletes);
    knoutCarga.NaoPermitirAcessarDocumentosAntesCargaEmTransporte.val(carga.NaoPermitirAcessarDocumentosAntesCargaEmTransporte);
    knoutCarga.TipoServicoCarga.val(carga.TipoServicoCarga);
    knoutCarga.PossuiMontagemContainer.val(carga.PossuiMontagemContainer);
    knoutCarga.ObrigatorioVincularContainerCarga.val(carga.ObrigatorioVincularContainerCarga);
    knoutCarga.NumeroBooking.val(carga.NumeroBooking);
    knoutCarga.Cliente.val(carga.Cliente);
    knoutCarga.CodigoMotoristaVeiculo.val(carga.CodigoMotoristaVeiculo);
    knoutCarga.NomeMotoristaVeiculo.val(carga.NomeMotoristaVeiculo);
    knoutCarga.ExcecaoCab.val(carga.ExcecaoCab);

    knoutCarga.Codigo.val(carga.Codigo);
    knoutCarga.CodigoCargaEmbarcador.val(carga.CodigoCargaEmbarcador);
    knoutCarga.CodigoJanelaCarregamento.val(carga.CodigoJanelaCarregamento);
    knoutCarga.CodigoJanelaDescarregamento.val(carga.CodigoJanelaDescarregamento);
    knoutCarga.CodigoCargaVeiculoContainer.val(carga.CodigoCargaVeiculoContainer);
    knoutCarga.EscolherHorarioCarregamentoPorLista.val(carga.EscolherHorarioCarregamentoPorLista);
    knoutCarga.Cliente.val(carga.Cliente);

    knoutCarga.Moeda.val(carga.Moeda);
    knoutCarga.ValorCotacaoMoeda.val(Globalize.format(carga.ValorCotacaoMoeda, "n10"));
    knoutCarga.ValorTotalMoeda.val(Globalize.format(carga.ValorTotalMoeda, "n2"));

    knoutCarga.Mercosul.val(carga.DadosMercosul.Mercosul);
    knoutCarga.EmitindoCRT.val(carga.DadosMercosul.EmitindoCRT);
    knoutCarga.PossuiFacturaFake.val(carga.DadosMercosul.PossuiFacturaFake);
    knoutCarga.Internacional.val(carga.DadosMercosul.Internacional);

    knoutCarga.CargaTipoInformarDadosNotaCte.val(carga.CargaTipoInformarDadosNotaCte);
    knoutCarga.PossuiOcultarInformacoesCarga.val(carga.PossuiOcultarInformacoesCarga);
    knoutCarga.BuscarNFesEmillenium.visible(carga.PermitirBuscarNFesEmillenium);

    knoutCarga.DataCriacaoCarga.val(carga.DataCriacaoCarga);
    knoutCarga.Origem.val(carga.Origem);
    knoutCarga.Destino.val(carga.Destino);
    knoutCarga.CargaTrechoSumarizada.val(carga.CargaTrechoSumarizada);

    knoutCarga.DataAvancouSegundaEtapa.val(carga.DataAvancouSegundaEtapa);

    knoutCarga.Km.val(carga.KM);
    if (carga.KM > -1)
        knoutCarga.Km.visible(true);

    knoutCarga.Filial.val(carga.Filial + carga.NumeroDocaCarga);

    if (carga.FilialVenda == null)
        knoutCarga.FilialVenda.visible(false);

    knoutCarga.FilialVenda.val(carga.FilialVenda);
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        knoutCarga.Descricao.val(Localization.Resources.Cargas.Carga.DescricaoCarga + " " + carga.CodigoCargaEmbarcador + " (" + carga.DataCriacaoCarga + ") : " + carga.OrigemDestinos);
    else
        knoutCarga.Descricao.val(Localization.Resources.Cargas.Carga.DescricaoCarga + " " + carga.CodigoCargaEmbarcador + " - " + carga.Filial + carga.NumeroDocaCarga + " (" + carga.DataCriacaoCarga + ") : " + carga.OrigemDestinos);

    if (carga.CargaAgrupada) {
        knoutCarga.NumeroCargaOriginais.val(carga.NumeroCargaOriginais);
        knoutCarga.NumeroCargaOriginais.visible(true);
        knoutCarga.VerDetalhesCargaAgrupada.visible(carga.CargaDeComplemento);
    }

    knoutCarga.Peso.val(Globalize.format(carga.PesoTotal, "n2"));
    knoutCarga.PesoTotalComPallet.val(Globalize.format(carga.PesoTotalComPallet, "n2"));
    knoutCarga.PesoTotalComPallet.visible(carga.PesoTotal != carga.PesoTotalComPallet);
    knoutCarga.ValorMercadoria.val(carga.ValorMercadoria > 0 ? Globalize.format(carga.ValorMercadoria, "n2") : "");
    knoutCarga.ValorTotalMercadoriaPedidos.val(carga.ValorTotalMercadoriaPedidos > 0 ? Globalize.format(carga.ValorTotalMercadoriaPedidos, "n2") : "");

    knoutCarga.PesoReentrega.val(carga.PesoTotalReentrega > 0 ? Globalize.format(carga.PesoTotalReentrega, "n2") : "");
    knoutCarga.ObservacaoTransportador.val(carga.ObservacaoTransportador > 0 ? Globalize.format(carga.ObservacaoTransportador, "n2") : "");
    knoutCarga.PesoTotal.val(carga.PesoTotalReentrega > 0 ? Globalize.format(carga.PesoTotalReentrega + carga.PesoTotal, "n2") : "");
    knoutCarga.NumeroCarregamento.val(carga.NumeroCarregamento);
    knoutCarga.CargaPerigosaIntegracao.val(carga.CargaPerigosaIntegracao);
    knoutCarga.DivisoriaIntegracao.val(carga.DivisoriaIntegracao);

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
        knoutCarga.PesoTotalNF.val(carga.PesoTotalNF > 0 ? Globalize.format(carga.PesoTotalNF, "n2") : "");
    else
        knoutCarga.PesoTotalNF.visible(false);

    knoutCarga.PesoCubadoNF.val(carga.PesoCubadoNF > 0 ? Globalize.format(carga.PesoCubadoNF, "n2") : "");

    knoutCarga.PesoLiquido.val(carga.PesoLiquidoTotal > 0 ? Globalize.format(carga.PesoLiquidoTotal, "n2") : "");
    knoutCarga.TipoFreteEscolhido.val(carga.TipoFreteEscolhido);

    knoutCarga.SituacaoCarga.val(carga.SituacaoCarga);

    if (_CONFIGURACAO_TMS.ExibirFaixaTemperaturaNaCarga) {
        knoutCarga.FaixaTemperatura.visible(true);
        knoutCarga.AlterarFaixaTemperatura.visible(EnumSituacoesCarga.isPermiteAlterarFaixaTemperatura(carga.SituacaoCarga));
    }

    knoutCarga.DataInicioViagem.val(carga.DataInicioViagem);
    if (_CONFIGURACAO_TMS.NaoPermitirCancelarCargaComInicioViagem)
        knoutCarga.DataInicioViagem.visible(!string.IsNullOrWhiteSpace(carga.DataInicioViagem));
    else
        knoutCarga.DataInicioViagem.visible(false);

    knoutCarga.Redespacho.val(carga.Redespacho);
    knoutCarga.Redespacho.visible(!string.IsNullOrWhiteSpace(carga.Redespacho));

    knoutCarga.Onda.val(carga.Onda);
    knoutCarga.ClusterRota.val(carga.ClusterRota);
    knoutCarga.QuantidadeVolumes.val(carga.QuantidadeVolumes);
    knoutCarga.TipoTrecho.val(carga.TipoTrecho);
    knoutCarga.Regiao.val(carga.Regiao);
    knoutCarga.Mesorregiao.val(carga.Mesorregiao);
    knoutCarga.NumeroPreCarga.val(carga.NumeroPreCarga);
    knoutCarga.IdentificadorDeRota.val(carga.IdentificadorDeRota);

    knoutCarga.ExternalDT2.val(carga.ExternalDT2);
    knoutCarga.ExternalDT1.val(carga.ExternalDT1);

    if (_CONFIGURACAO_TMS.UtilizaControlePercentualExecucao && (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AlterarPercentualExecucao, _PermissoesPersonalizadasCarga))) {
        knoutCarga.AjustarPecentualExecucao.visible(true);
    }

    if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermiteAlterarExternalId, _PermissoesPersonalizadasCarga)) {
        knoutCarga.AlterarExternalID1.visible(true);
        knoutCarga.AlterarExternalID2.visible(true);
    }
    knoutCarga.QuantidadeVolumesNF.val(carga.QuantidadeVolumesNF);
    knoutCarga.DataPrevisaoInicioViagem.val(carga.DataPrevisaoInicioViagem);
    knoutCarga.ValorTotalProdutos.val(carga.ValorTotalProdutos > 0 ? Globalize.format(carga.ValorTotalProdutos, "n2") : "");
    knoutCarga.QuantidadeNFEs.val(carga.QuantidadeNFEs);
    knoutCarga.VolumesCaixasNFEs.val(carga.VolumesCaixasNFEs);
    knoutCarga.ProvedoresOS.val(carga.ProvedoresOS);
    knoutCarga.ZonaTransporte.val(carga.ZonaTransporte);
    knoutCarga.NumeroContainerCarga.val(carga.NumeroContainerCarga);
    knoutCarga.IdMontagemContainer.val(carga.IdMontagemContainer);
    knoutCarga.ExigeTermoAceiteTransportador.val(carga.ExigeTermoAceiteTransportador);
    knoutCarga.CodigoCargaJanelaCarregamentoTransportador.val(carga.CodigoCargaJanelaCarregamentoTransportador);
    knoutCarga.PermiteTransportadorAvancarEtapaEmissao.val(carga.PermiteTransportadorAvancarEtapaEmissao);
    knoutCarga.TermoAceite.val(carga.TermoAceite);
    knoutCarga.RotaEmbarcador.val(carga.RotaEmbarcador);
    knoutCarga.DataUltimaLiberacao.val(carga.DataUltimaLiberacao);
    knoutCarga.ValorFreteSimulacao.val(carga.ValorFreteSimulacao);
    knoutCarga.ValorToneladaSimulado.val(carga.ValorToneladaSimulado);
    knoutCarga.SituacaoRecebimentoNotas.val(carga.SituacaoRecebimentoNotas);
    knoutCarga.CategoriaOS.val(carga.CategoriaOS);
    knoutCarga.TipoOSConvertido.val(carga.TipoOSConvertido);
    knoutCarga.TipoOS.val(carga.TipoOS);
    knoutCarga.DirecionamentoCustoExtra.val(carga.DirecionamentoCustoExtra);
    knoutCarga.UtilizarDirecionamentoCustoExtra.val(carga.UtilizarDirecionamentoCustoExtra);
    knoutCarga.StatusCustoExtra.val(carga.StatusCustoExtra);
    knoutCarga.CanalVenda.val(carga.CanalVenda);
    knoutCarga.TipoCobrancaMultimodal.val(carga.TipoCobrancaMultimodal);
    knoutCarga.RetornoAE.val(carga.RetornoAE);
    knoutCarga.QuebraContainer.val(carga.QuebraContainer);
    knoutCarga.CargaCritica.val(carga.CargaCritica);
    knoutCarga.VerMotivoSituacaoAE.visible(!string.IsNullOrWhiteSpace(carga.RetornoAE));
    knoutCarga.UsuarioCriacaoRemessa.val(carga.UsuarioCriacaoRemessa);
    knoutCarga.NumeroOrdem.val(carga.NumeroOrdem);
    knoutCarga.Cubagem.val(carga.Cubagem);
    knoutCarga.RegiaoDestino.val(carga.RegiaoDestino);
    knoutCarga.NumeroEntregasFinais.val(carga.NumeroEntregasFinais + " " + Localization.Resources.Cargas.Carga.Entrega + " ");
    knoutCarga.NumeroEntregasFinais.visible(carga.NumeroEntregasFinais > 0);
    knoutCarga.ObservacaoLocalEntrega.val(carga.ObservacaoLocalEntrega);
    knoutCarga.LocalParqueamento.val(carga.LocalParqueamento);
    knoutCarga.PrevisaoEntregaTransportador.val(carga.PrevisaoEntregaTransportador);
    knoutCarga.ValorCustoFrete.val(Globalize.format(carga.ValorCustoFrete, "n2"))
    knoutCarga.PLPsCorreios.val(carga.PLPsCorreios);
    knoutCarga.NumerosEtiquetasCorreios.val(carga.NumerosEtiquetasCorreios);
    knoutCarga.TransferenciaContainer.val(carga.TransferenciaContainer);
    knoutCarga.RemetenteTrasferencia.codEntity(carga.RemetenteTrasferencia);
    knoutCarga.RemetenteTrasferencia.val(carga.RemetenteTrasferencia);
    knoutCarga.CategoriaCargaEmbarcador.val(carga.CategoriaCargaEmbarcador);
    knoutCarga.SetPointVeiculo.val(carga.SetPointVeiculo);
    knoutCarga.RangeTempCarga.val(carga.RangeTempCarga);
    knoutCarga.DescricaoSituacaoCarga.val(carga.DescricaoSituacaoCarga);
    knoutCarga.SituacaoAlteracaoFreteCarga.val(carga.SituacaoAlteracaoFreteCarga);
    knoutCarga.SituacaoAutorizacaoIntegracaoCTe.val(carga.SituacaoAutorizacaoIntegracaoCTe);
    knoutCarga.DataCarregamento.val(carga.DataCarregamento);
    knoutCarga.PreCalculodeFrete.val(carga.PreCalculodeFrete);
    knoutCarga.NotasFilhasRecibidas.val(carga.NotasFilhasRecibidas);
    knoutCarga.AlterarDataCarregamento.visible(!carga.NaoPermitirAlterarDataCarregamentoCarga);
    knoutCarga.ExibirDatasCarregamento.enable(!carga.NaoPermitirAlterarDataCarregamentoCarga);
    knoutCarga.DataRetornoCD.val(carga.DataRetornoCD);
    knoutCarga.DataRetornoCD.visible(carga.PermitirAlterarDataRetornoCDCarga || carga.DataRetornoCD != "");
    knoutCarga.AlterarDataRetornoCD.visible(carga.PermitirAlterarDataRetornoCDCarga);
    knoutCarga.LocalCarregamento.val(carga.LocalCarregamento.Descricao);
    knoutCarga.LocalCarregamento.codEntity(carga.LocalCarregamento.Codigo);

    //Dados de Transporte
    knoutCarga.Empresa.codEntity(carga.DadosTransporte.Empresa.Codigo);
    knoutCarga.Empresa.val(carga.DadosTransporte.Empresa.Descricao);
    knoutCarga.NumeroPager.val(carga.DadosTransporte.NumeroPager);
    knoutCarga.NumeroPager.visible(_CONFIGURACAO_TMS.ExibirNumeroPagerEtapaInicialCarga);
    knoutCarga.DataBaseCRT.val(carga.DadosTransporte.DataBaseCRT);
    knoutCarga.DataBaseCRT.visible(_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira);
    knoutCarga.TipoContainerDescricao.val(carga.DadosTransporte.TipoContainerDescricao);
    knoutCarga.InicioCarregamento.val(carga.DadosTransporte.InicioCarregamento);
    knoutCarga.TerminoCarregamento.val(carga.DadosTransporte.TerminoCarregamento);
    knoutCarga.TipoCarga.val(carga.DadosTransporte.TipoCarga.Descricao);
    knoutCarga.TipoCarga.codEntity(carga.DadosTransporte.TipoCarga.Codigo);
    knoutCarga.TipoContainer.val(carga.DadosTransporte.TipoContainer.Descricao);
    knoutCarga.TipoContainer.codEntity(carga.DadosTransporte.TipoContainer.Codigo);
    knoutCarga.CodTipoCargaOriginal.val = carga.DadosTransporte.TipoCarga.Codigo;
    knoutCarga.ModeloVeicularCarga.val(carga.DadosTransporte.ModeloVeicularCarga.Descricao);
    knoutCarga.NumeroReboques.val(carga.DadosTransporte.ModeloVeicularCarga.NumeroReboques);
    knoutCarga.PortoOrigem.val(carga.DadosTransporte.PortoOrigem.Descricao);
    knoutCarga.PortoOrigem.codEntity(carga.DadosTransporte.PortoOrigem.Codigo);
    knoutCarga.PortoDestino.val(carga.DadosTransporte.PortoDestino.Descricao);
    knoutCarga.PortoDestino.codEntity(carga.DadosTransporte.PortoDestino.Codigo);
    knoutCarga.TerminalOrigem.val(carga.DadosTransporte.TerminalOrigem.Descricao);
    knoutCarga.TerminalOrigem.codEntity(carga.DadosTransporte.TerminalOrigem.Codigo);
    knoutCarga.TerminalDestino.val(carga.DadosTransporte.TerminalDestino.Descricao);
    knoutCarga.TerminalDestino.codEntity(carga.DadosTransporte.TerminalDestino.Codigo);
    knoutCarga.ModeloVeicularCarga.codEntity(carga.DadosTransporte.ModeloVeicularCarga.Codigo);
    knoutCarga.CodModeloVeicularCargaOriginal.val = carga.DadosTransporte.ModeloVeicularCarga.Codigo;
    knoutCarga.PedidoViagemNavio.val(carga.DadosTransporte.PedidoViagemNavio.Descricao);
    knoutCarga.PedidoViagemNavio.codEntity(carga.DadosTransporte.PedidoViagemNavio.Codigo);
    knoutCarga.QuantidadePaletes.visible(knoutCarga.PermiteInformarQuantidadePaletes.val() === true);
    knoutCarga.QuantidadePaletes.val(carga.DadosTransporte.QuantidadePaletes);
    knoutCarga.HorarioLimiteConfirmacaoMotorista.val(carga.DadosTransporte.HorarioLimiteConfirmacaoMotorista);
    knoutCarga.Empresa.PossuiInformacoesIMO = carga.DadosTransporte.PossuiInformacoesIMO;
    knoutCarga.MotoristaRecusouCarga.val(carga.DadosTransporte.MotoristaRecusouCarga);
    knoutCarga.TipoCarregamento.val(carga.DadosTransporte.TipoCarregamento.Descricao);
    knoutCarga.TipoCarregamento.codEntity(carga.DadosTransporte.TipoCarregamento.Codigo);
    knoutCarga.CentroResultado.val(carga.DadosTransporte.CentroResultado.Descricao);
    knoutCarga.CentroResultado.codEntity(carga.DadosTransporte.CentroResultado.Codigo);
    knoutCarga.JustificativaAutorizacaoCarga.val(carga.DadosTransporte.JustificativaAutorizacaoCarga.Descricao);
    knoutCarga.JustificativaAutorizacaoCarga.codEntity(carga.DadosTransporte.JustificativaAutorizacaoCarga.Codigo);
    knoutCarga.Setor.val(carga.DadosTransporte.Setor.Descricao);
    knoutCarga.Setor.codEntity(carga.DadosTransporte.Setor.Codigo);
    knoutCarga.ObservacaoCarga.val(carga.DadosTransporte.ObservacaoCarga);

    if (knoutCarga.PedidoViagemNavio.codEntity() > 0)
        knoutCarga.PedidoViagemNavio.enable(false);

    knoutCarga.Pedidos.val = carga.Pedidos;
    knoutCarga.Transbordos.val = carga.Transbordos;
    knoutCarga.TipoContratacaoCarga.val(carga.TipoContratacaoCarga);
    knoutCarga.ExigeNotaFiscalParaCalcularFrete.val(carga.ExigeNotaFiscalParaCalcularFrete);
    knoutCarga.EmissaoLiberada.val(carga.EmissaoLiberada);
    knoutCarga.AguardarIntegracaoEtapaTransportador.val(carga.AguardarIntegracaoEtapaTransportador);
    knoutCarga.ValidarLicencaMotorista.val(carga.ValidarLicencaMotorista);
    knoutCarga.NaoExigeVeiculoParaEmissao.val(carga.NaoExigeVeiculoParaEmissao);
    knoutCarga.CargaDeComplemento.val(carga.CargaDeComplemento);
    knoutCarga.CargaDePreCarga.val(carga.CargaDePreCarga);
    knoutCarga.CargaAgrupada.val(carga.CargaAgrupada);
    knoutCarga.CIOTGeradoAutomaticamente.val(carga.CIOTGeradoAutomaticamente);
    knoutCarga.ObrigatoriedadeCIOTEmissaoMDFe.val(carga.ObrigatoriedadeCIOTEmissaoMDFe);
    knoutCarga.PossuiCIOT.val(carga.PossuiCIOT);
    knoutCarga.ProprietarioTAC.val(carga.ProprietarioTAC);
    knoutCarga.VeiculoPropriedadeTerceiro.val(carga.VeiculoPropriedadeTerceiro);

    knoutCarga.ExigeConfirmacaoTracao.val(carga.ExigeConfirmacaoTracao);
    knoutCarga.RejeitadaPeloTransportador.val(carga.RejeitadaPeloTransportador);
    knoutCarga.PossuiGenset.val(carga.PossuiGenset);
    knoutCarga.LiberadaEtapaFaturamentoBloqueada.val(carga.LiberadaEtapaFaturamentoBloqueada);
    knoutCarga.PermitirInformarAnexoContainerCarga.val(carga.PermitirInformarAnexoContainerCarga);
    knoutCarga.ExibirMensagensAlerta.visibleFade(carga.ExibirMensagensAlerta);
    knoutCarga.MensagensAlerta.val(carga.MensagensAlerta);
    knoutCarga.MensagemAlertaComBloqueio.val(isPossuiMensagemAlertaComBloqueio(carga.MensagensAlerta));

    knoutCarga.ApoliceSeguro.multiplesEntities(carga.ApoliceSeguro);

    knoutCarga.AutoTipoCarga.val(carga.AutoTipoCarga);
    knoutCarga.AutoModeloVeicular.val(carga.AutoModeloVeicular);
    knoutCarga.FreteDeTerceiro.val(carga.FreteDeTerceiro);

    knoutCarga.Operador.val(carga.Operador);
    knoutCarga.OperadorInsercao.val(carga.OperadorInsercao);

    knoutCarga.Rota.val(carga.Rota.Descricao);
    knoutCarga.Rota.codEntity(carga.Rota.Codigo);

    knoutCarga.Carregamento.val(carga.Carregamento.Descricao);
    knoutCarga.Carregamento.codEntity(carga.Carregamento.Codigo);
    knoutCarga.ModeloVeicularCarregamento.val(carga.ModeloVeicularCarregamento);
    knoutCarga.ObservacaoCarregamento.val(carga.ObservacaoCarregamento);
    knoutCarga.OrdemEmbarque.val(carga.OrdemEmbarque);
    knoutCarga.PossuiOrdemEmbarque.val(carga.PossuiOrdemEmbarque);
    knoutCarga.OrdemEmbarquePendente.val(carga.OrdemEmbarquePendente);

    if (carga.LinhaSeparacao != null)
        knoutCarga.LinhaSeparacao.val(carga.LinhaSeparacao);

    if (_operadorLogistica.OperadorSupervisor) {
        knoutCarga.Operador.visible = true;

        if (knoutCarga.OperadorInsercao.val() != "")
            knoutCarga.OperadorInsercao.visible(true);
        else
            knoutCarga.OperadorInsercao.visible(false);
    }
    else if (carga.TipoOperacao.ExibirOperadorInsercaoCargaNoPortalTransportador && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        if (knoutCarga.OperadorInsercao.val() != "")
            knoutCarga.OperadorInsercao.visible(true);
        else
            knoutCarga.OperadorInsercao.visible(false);
    }

    knoutCarga.ObrigarInformarRICnaColetaDeConteiner.val(carga.TipoOperacao.ObrigarInformarRICnaColetaDeConteiner);
    knoutCarga.RaizCNPJEmpresa.val(carga.RaizCNPJEmpresa);
    knoutCarga.PossuiPendencia.val(carga.PossuiPendencia);
    knoutCarga.OrigemFretePelaJanelaTransportador.val(carga.OrigemFretePelaJanelaTransportador);
    knoutCarga.PermiteImportarDocumentosManualmente.val(carga.PermiteImportarDocumentosManualmente);
    knoutCarga.PermitirTransbordarNotasDeOutrasCargas.val(carga.PermitirTransbordarNotasDeOutrasCargas);
    knoutCarga.PermitirSelecionarNotasCompativeis.val(carga.PermitirSelecionarNotasCompativeis);
    knoutCarga.PermitirTransportadorEnviarNotasFiscais.val(carga.PermitirTransportadorEnviarNotasFiscais);
    knoutCarga.PermitirTransportadorSolicitarNotasFiscais.val(carga.PermitirTransportadorSolicitarNotasFiscais);
    knoutCarga.PermitirTransportadorInformarObservacaoImpressaoCarga.val(carga.PermitirTransportadorInformarObservacaoImpressaoCarga);
    knoutCarga.PermiteInformarTransportadorEtapaUmQuandoNotaFiscalNaoRecebidaAntesCalculoFrete.val(carga.TipoOperacao.PermiteInformarTransportadorEtapaUmQuandoNotaFiscalNaoRecebidaAntesCalculoFrete);
    knoutCarga.PermiteInformarMotoristasSomenteEtapaUmQuandoNotasFiscaisNaoSaoRecebidasAntesCalcularFrete.val(carga.TipoOperacao.PermiteInformarMotoristasSomenteEtapaUmQuandoNotasFiscaisNaoSaoRecebidasAntesCalcularFrete);
    knoutCarga.CalcularFretePeloBIDPedidoOrigem.val(carga.TipoOperacao.CalcularFretePeloBIDPedidoOrigem);
    knoutCarga.InformarValorFreteTerceiroManualmente.val(carga.TipoOperacao.InformarValorFreteTerceiroManualmente);
    knoutCarga.PermiteInformarIsca.val(carga.PermiteInformarIsca);
    knoutCarga.ExigeInformarIsca.val(carga.ExigeInformarIsca);
    knoutCarga.ExibirValorUnitarioDoProduto.val(carga.ExibirValorUnitarioDoProduto);

    knoutCarga.SubContratante.val(carga.SubContratante);
    knoutCarga.DataInicioEmissao.val(carga.DataInicioEmissao);

    knoutCarga.ContratoFreteTerceiro.val(carga.ContratoFreteTerceiro);

    if (knoutCarga.CalcularFretePeloBIDPedidoOrigem.val()) {
        knoutCarga.RecalcularFreteBID.visible(true);
    }
    else {
        knoutCarga.RecalcularFrete.visible(true);
    }

    if (_CONFIGURACAO_TMS.TipoContratoFreteTerceiro == EnumTipoContratoFreteTerceiro.PorCarga && !carga.CargaTransbordo) {
        if (carga.ContratoFreteTerceiro != null)
            knoutCarga.SubContratado.val(carga.ContratoFreteTerceiro.Terceiro);
    } else {
        if (carga.Terceiro != null)
            knoutCarga.SubContratado.val(carga.Terceiro.Descricao);
    }

    knoutCarga.EmitindoCTes.val(carga.EmitindoCTes);
    knoutCarga.AverbandoCTes.val(carga.AverbandoCTes);
    knoutCarga.AverbouTodosCTes.val(carga.AverbouTodosCTes);
    knoutCarga.AutorizouTodosCTes.val(carga.AutorizouTodosCTes);
    knoutCarga.CTesEmDigitacao.val(carga.CTesEmDigitacao);
    knoutCarga.ProcessandoDocumentosFiscais.val(carga.ProcessandoDocumentosFiscais);
    knoutCarga.AgGeracaoCTesAnteriorFilialEmissora.val(carga.AgGeracaoCTesAnteriorFilialEmissora);
    knoutCarga.EmEmissaoCTeSubContratacaoFilialEmissora.val(carga.EmEmissaoCTeSubContratacaoFilialEmissora);
    knoutCarga.ProblemaCTE.val(carga.ProblemaCTE);
    knoutCarga.ProblemaEmissaoNFeRemessa.val(carga.ProblemaEmissaoNFeRemessa);
    knoutCarga.NaoPermitirLiberarSemValePedagio.val(carga.NaoPermitirLiberarSemValePedagio);
    knoutCarga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora.val(carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora);
    knoutCarga.EmiteMDFeFilialEmissora.val(carga.EmiteMDFeFilialEmissora);
    knoutCarga.ObrigatorioInformarAnexoSolicitacaoFrete.val(carga.ObrigatorioInformarAnexoSolicitacaoFrete);
    knoutCarga.NaoGerarMDFe.val(carga.NaoGerarMDFe);
    knoutCarga.ProblemaNFS.val(carga.ProblemaNFS);
    knoutCarga.PossuiNFs.val(carga.PossuiNFs);
    knoutCarga.possuiCTe.val(carga.possuiCTe);
    knoutCarga.possuiNFSManual.val(carga.possuiNFSManual);
    knoutCarga.AgNFSManual.val(carga.AgNFSManual);
    knoutCarga.CargaEmitidaParcialmente.val(carga.CargaEmitidaParcialmente);
    knoutCarga.AgSelecaoRotaOperador.val(carga.AgSelecaoRotaOperador);
    knoutCarga.ExistePacote.val(carga.ExistePacote);
    knoutCarga.PermiteConsultarPorPacotesLoggi.val(carga.TipoOperacao.PermiteConsultarPorPacotesLoggi);
    knoutCarga.ClassificacaoNFeRemessaVenda.val(carga.ClassificacaoNFeRemessaVenda);

    knoutCarga.PercentualSeparacaoMercadoria.val(carga.PercentualSeparacaoMercadoria);
    if (carga.SeparacaoMercadoriaConfirmada)
        knoutCarga.PercentualSeparacaoMercadoria.cssClass("progress-bar bg-color-teal");

    knoutCarga.PossuiCTeSubcontratacaoFilialEmissora.val(carga.PossuiCTeSubcontratacaoFilialEmissora);
    knoutCarga.possuiAverbacaoCTe.val(carga.possuiAverbacaoCTe);
    knoutCarga.PossuiAverbacaoMDFe.val(carga.PossuiAverbacaoMDFe);

    knoutCarga.SeparacaoConferida.val(carga.SeparacaoConferida);
    knoutCarga.PossuiSeparacao.val(carga.PossuiSeparacao);
    knoutCarga.PossuiRotaDefinida.val(carga.PossuiRotaDefinida);


    knoutCarga.ProblemaMDFe.val(carga.ProblemaMDFe);
    knoutCarga.ProblemaAverbacaoCTe.val(carga.ProblemaAverbacaoCTe);
    knoutCarga.ProblemaIntegracaoCIOT.val(carga.ProblemaIntegracaoCIOT);
    knoutCarga.LiberadoComProblemaCIOT.val(carga.LiberadoComProblemaCIOT);
    knoutCarga.ProblemaAverbacaoMDFe.val(carga.ProblemaAverbacaoMDFe);

    knoutCarga.ProblemaIntegracaoValePedagio.val(carga.ProblemaIntegracaoValePedagio);
    knoutCarga.PossuiIntegracaoValePedagio.val(carga.PossuiIntegracaoValePedagio);
    knoutCarga.LiberadoComProblemaValePedagio.val(carga.LiberadoComProblemaValePedagio);
    knoutCarga.IntegrandoValePedagio.val(carga.IntegrandoValePedagio);

    knoutCarga.LiberadoComProblemaPagamentoMotorista.val(carga.LiberadoComProblemaPagamentoMotorista);
    knoutCarga.ProblemaIntegracaoPagamentoMotorista.val(carga.ProblemaIntegracaoPagamentoMotorista);
    knoutCarga.ProtocoloIntegracaoGR.val(carga.ProtocoloIntegracaoGR);

    knoutCarga.LiberadaSemTodosPreCTes.val(carga.LiberadaSemTodosPreCTes);
    knoutCarga.AgImportacaoCTe.val(carga.AgImportacaoCTe);
    knoutCarga.PendenteGerarCargaDistribuidor.val(carga.PendenteGerarCargaDistribuidor);
    knoutCarga.CalcularFreteCliente.val(carga.CalcularFreteCliente);

    knoutCarga.ExibirCalculoFreteCargaAgrupada.val(carga.ExibirCalculoFreteCargaAgrupada);

    if (carga.CalcularFreteCliente) {
        $("#spanliDetalhesFreteFilialEmissora").text(Localization.Resources.Cargas.Carga.DetalhesDoFreteDoCliente);
        $("#spanliDetalhesFreteFilialEmissora").text(Localization.Resources.Cargas.Carga.ComposicaoDoFreteDoCliente);
    }

    knoutCarga.EmissaoDocumentosAutorizada.val(carga.EmissaoDocumentosAutorizada);

    knoutCarga.MotivoPendencia.val(carga.MotivoPendencia);
    knoutCarga.CargaTransbordo.val(carga.CargaTransbordo);
    knoutCarga.ExigirInformarContainer.val(carga.ExigirInformarContainer);
    knoutCarga.ExigirInformarRetiradaContainer.val(carga.ExigirInformarRetiradaContainer);
    knoutCarga.TipoIntegracaoMercadoLivre.val(carga.TipoIntegracaoMercadoLivre);

    knoutCarga.Container.val(carga.Container.Descricao);
    knoutCarga.Container.codEntity(carga.Container.Codigo);

    knoutCarga.TipoOperacao.val(carga.TipoOperacao.Descricao);
    knoutCarga.TipoOperacao.codEntity(carga.TipoOperacao.Codigo);
    knoutCarga.TipoOperacao.tipoImpressaoDiarioBordo = carga.TipoOperacao.TipoImpressaoDiarioBordo;
    knoutCarga.TipoOperacao.exigeChaveVenda = carga.TipoOperacao.ExigeChaveVenda;
    knoutCarga.TipoOperacao.solicitarNotasFiscaisAoSalvarDadosTransportador = carga.TipoOperacao.SolicitarNotasFiscaisAoSalvarDadosTransportador;
    knoutCarga.TipoOperacao.permitirValorFreteInformadoPeloEmbarcadorZerado = carga.TipoOperacao.PermitirValorFreteInformadoPeloEmbarcadorZerado;
    knoutCarga.TipoOperacao.utilizarPlanoViagem = carga.TipoOperacao.UtilizarPlanoViagem;
    knoutCarga.TipoOperacao.PermiteRealizarImpressaoCarga = carga.TipoOperacao.PermiteRealizarImpressaoCarga;
    knoutCarga.TipoOperacao.PossuiConsolidacao = carga.TipoOperacao.PossuiConsolidacao;
    knoutCarga.TipoOperacao.naoExibirDetalhesDoFretePortalTransportador = carga.TipoOperacao.NaoExibirDetalhesDoFretePortalTransportador;
    knoutCarga.TipoOperacao.imprimirRelatorioRomaneioEtapaImpressaoCarga = carga.TipoOperacao.ImprimirRelatorioRomaneioEtapaImpressaoCarga;
    knoutCarga.TipoOperacao.naoPermiteAvancarCargaSemDataPrevisaoDeEntrega = carga.TipoOperacao.NaoPermiteAvancarCargaSemDataPrevisaoDeEntrega;
    knoutCarga.TipoOperacao.imprimirCRT = carga.TipoOperacao.ImprimirCRT;
    knoutCarga.TipoOperacao.permitirConsultaDeValoresPedagioSemParar = carga.TipoOperacao.PermitirConsultaDeValoresPedagioSemParar;
    knoutCarga.TipoOperacao.PermiteEscolherDestinacaoDoComplementoDeFrete = carga.TipoOperacao.PermiteEscolherDestinacaoDoComplementoDeFrete;
    knoutCarga.TipoOperacao.validarSeCargaPossuiVinculoComPreCarga = carga.TipoOperacao.ValidarSeCargaPossuiVinculoComPreCarga;
    knoutCarga.TipoOperacao.permitirAdicionarObservacaoNaEtapaUmDaCarga = carga.TipoOperacao.PermitirAdicionarObservacaoNaEtapaUmDaCarga;
    knoutCarga.TipoOperacao.exibirFiltroDePedidosEtapaNotaFiscal = carga.TipoOperacao.ExibirFiltroDePedidosEtapaNotaFiscal;
    knoutCarga.TipoOperacao.TipoConsolidacaoPrechekin = carga.TipoOperacao.TipoPrechekin;
    knoutCarga.TipoOperacao.TipoMilkrun = carga.TipoOperacao.TipoMilkrun;
    knoutCarga.TipoOperacao.AlertarTransportadorNaoIMOCargasPerigosas = carga.TipoOperacao.AlertarTransportadorNaoIMOCargasPerigosas;
    knoutCarga.TipoOperacao.PossuiIntegracaoLogiun = carga.TipoOperacao.PossuiIntegracaoLogiun;
    knoutCarga.TipoOperacao.PermitirTransportadorInformeNotasCompativeis = carga.TipoOperacao.PermitirTransportadorInformeNotasCompativeis;
    knoutCarga.TipoOperacao.TipoCobrancaMultimodal = carga.TipoOperacao.TipoCobrancaMultimodal;
    knoutCarga.TipoOperacao.NecessitaInformarPlacaCarregamento = carga.TipoOperacao.NecessitaInformarPlacaCarregamento;
    knoutCarga.TipoOperacao.GerarPagamentoMotoristaAutomaticamenteComValorAdiantamentoContainer = carga.TipoOperacao.GerarPagamentoMotoristaAutomaticamenteComValorAdiantamentoContainer;
    knoutCarga.TipoOperacao.ObrigatorioInformarAliquotaImpostoSuspensoeValor = carga.TipoOperacao.ObrigatorioInformarAliquotaImpostoSuspensoeValor;
    knoutCarga.TipoOperacao.ComprarValePedagioEtapaContainer = carga.TipoOperacao.ComprarValePedagioEtapaContainer;
    knoutCarga.TipoOperacao.AverbarContainerComAverbacaoCarga = carga.TipoOperacao.AverbarContainerComAverbacaoCarga;

    knoutCarga.TipoOperacao.PermitirSelecionarPreCargaNaCarga = carga.TipoOperacao.PermitirSelecionarPreCargaNaCarga;
    knoutCarga.TipoOperacao.InformarTransportadorSubcontratadoEtapaUm = carga.TipoOperacao.InformarTransportadorSubcontratadoEtapaUm;

    knoutCarga.TipoOperacao.IncrementaCodigoPorTipoOperacao = carga.TipoOperacao.IncrementaCodigoPorTipoOperacao;
    knoutCarga.TipoOperacao.AdicionaPrefixoCodigoCarga = carga.TipoOperacao.AdicionaPrefixoCodigoCarga;
    knoutCarga.TipoOperacao.ExigeConformacaoFreteAntesEmissao = carga.TipoOperacao.ExigeConformacaoFreteAntesEmissao;

    knoutCarga.FaixaTemperatura.val(carga.FaixaTemperatura.Descricao);
    knoutCarga.FaixaTemperatura.codEntity(carga.FaixaTemperatura.Codigo);

    knoutCarga.GrupoPessoa.controlaPallets = carga.GrupoPessoa.ControlaPallets;
    knoutCarga.Solicitante.val(carga.Solicitante);

    knoutCarga.CargaRedespacho.val(carga.CargaRedespacho);
    knoutCarga.PossuiCTeAnteriorFilialEmissora.val(carga.PossuiCTeAnteriorFilialEmissora);
    knoutCarga.UtilizarCTesAnterioresComoCTeFilialEmissora.val(carga.UtilizarCTesAnterioresComoCTeFilialEmissora);
    knoutCarga.ExclusivaDeSubcontratacaoOuRedespacho.val(carga.ExclusivaDeSubcontratacaoOuRedespacho);
    knoutCarga.PossuiIntegracaoIntercement.val(carga.PossuiIntegracaoIntercement);
    knoutCarga.CargaDestinadaCTeComplementar.val(carga.CargaDestinadaCTeComplementar);
    knoutCarga.PossuiIntegracaoMichelin.val(carga.PossuiIntegracaoMichelin);
    knoutCarga.CargaTakeOrPay.val(carga.CargaTakeOrPay);
    knoutCarga.CargaDemurrage.val(carga.CargaDemurrage);
    knoutCarga.CargaDetention.val(carga.CargaDetention);
    knoutCarga.CargaSVM.val(carga.CargaSVM);
    knoutCarga.CargaSVMTerceiro.val(carga.CargaSVMTerceiro);
    knoutCarga.CargaTrocaDeNota.val(carga.CargaTrocaDeNota);
    knoutCarga.Reentrega.val(carga.Reentrega);

    knoutCarga.CargaColeta.val(carga.CargaColeta);

    knoutCarga.PossuiConfiguracaoImpressora.val(carga.PossuiConfiguracaoImpressora);
    knoutCarga.SaldoInsuficienteContratoFreteCliente.val(carga.SaldoInsuficienteContratoFreteCliente);

    knoutCarga.ObservacaoEmissaoCarga.val(carga.ObservacaoEmissaoCarga);
    knoutCarga.ObservacaoEmissaoCarga.visible(!string.IsNullOrWhiteSpace(carga.ObservacaoEmissaoCarga));
    knoutCarga.ObservacaoEmissaoCargaTomador.val(carga.ObservacaoEmissaoCargaTomador);
    knoutCarga.ObservacaoEmissaoCargaTipoOperacao.val(carga.ObservacaoEmissaoCargaTipoOperacao);

    knoutCarga.ModalCarga.val(carga.ModalCarga);
    knoutCarga.ModalCarga.visible(!string.IsNullOrWhiteSpace(carga.ModalCarga));

    knoutCarga.QtdContainerCarga.val(carga.QtdContainerCarga);
    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
        knoutCarga.QtdContainerCarga.visible(!string.IsNullOrWhiteSpace(carga.QtdContainerCarga));
    else
        knoutCarga.QtdContainerCarga.visible(false);

    knoutCarga.Ordem.val(carga.Ordem);
    knoutCarga.Ordem.visible(!string.IsNullOrWhiteSpace(carga.Ordem));

    knoutCarga.PortoOrigemCarga.val(carga.PortoOrigemCarga);
    knoutCarga.PortoOrigemCarga.visible(!string.IsNullOrWhiteSpace(carga.PortoOrigemCarga));

    knoutCarga.PortoDestinoCarga.val(carga.PortoDestinoCarga);
    knoutCarga.PortoDestinoCarga.visible(!string.IsNullOrWhiteSpace(carga.PortoDestinoCarga));

    knoutCarga.RecebedorCarga.val(carga.RecebedorCarga);
    knoutCarga.RecebedorCarga.visible(!string.IsNullOrWhiteSpace(carga.RecebedorCarga));

    knoutCarga.Navio.val(carga.DadosTransporte.Navio.Descricao);
    knoutCarga.Navio.codEntity(carga.DadosTransporte.Navio.Codigo);

    knoutCarga.Balsa.val(carga.DadosTransporte.Balsa.Descricao);
    knoutCarga.Balsa.codEntity(carga.DadosTransporte.Balsa.Codigo);

    knoutCarga.TransportadorSubcontratado.val(carga.DadosTransporte.TransportadorSubcontratado.Descricao);
    knoutCarga.TransportadorSubcontratado.codEntity(carga.DadosTransporte.TransportadorSubcontratado.Codigo);

    //Intercab
    knoutCarga.Mercante.val(carga.DadosIntercab.Mercante);
    knoutCarga.TodosCTesComMercante.val(carga.DadosIntercab.TodosCTesComMercante);
    knoutCarga.TodosCTesComManifesto.val(carga.DadosIntercab.TodosCTesComManifesto);
    knoutCarga.MDFeAquaviario.val(carga.DadosIntercab.MDFeAquaviario);
    knoutCarga.OcultarMDFeRodoviario.val(carga.DadosIntercab.OcultarMDFeRodoviario);

    knoutCarga.PossuiMDFeAquaviarioGeradoMasNaoVinculado.val(carga.DadosIntercab.PossuiMDFeAquaviarioGeradoMasNaoVinculado);
    knoutCarga.PossuiMDFeAquaviarioRejeitado.val(carga.DadosIntercab.PossuiMDFeAquaviarioRejeitado);
    knoutCarga.PossuiMDFeAquaviarioPendente.val(carga.DadosIntercab.PossuiMDFeAquaviarioPendente);
    knoutCarga.PossuiMDFeAquaviarioAutorizado.val(carga.DadosIntercab.PossuiMDFeAquaviarioAutorizado);
    knoutCarga.CargaPortoPorto.val(carga.DadosIntercab.CargaPortoPorto);
    knoutCarga.CargaPortoPortoPendenciaDocumento.val(carga.DadosIntercab.CargaPortoPortoPendenciaDocumento);
    knoutCarga.CargaPortoPortoTimelineHabilitado.val(carga.DadosIntercab.CargaPortoPortoTimelineHabilitado);
    knoutCarga.HabilitarTimelineCargaFeeder.val(carga.DadosIntercab.HabilitarTimelineCargaFeeder);

    knoutCarga.CargaPortaPortaTimelineHabilitado.val(carga.DadosIntercab.CargaPortaPortaTimelineHabilitado);
    knoutCarga.PossuiCTePendenteSVM.val(carga.DadosIntercab.PossuiCTePendenteSVM);
    knoutCarga.PossuiSVMPendenteAutorizacao.val(carga.DadosIntercab.PossuiSVMPendenteAutorizacao);
    knoutCarga.CargaSVMProprioTimelineHabilitado.val(carga.DadosIntercab.CargaSVMProprioTimelineHabilitado);

    knoutCarga.Faturamento.val(carga.DadosIntercab.Faturamento);
    knoutCarga.IntegracaoFaturamento.val(carga.DadosIntercab.IntegracaoFaturamento);
    knoutCarga.ModificarTimelineIntegracaoCarga.val(carga.DadosIntercab.ModificarTimelineIntegracaoCarga);
    knoutCarga.NumeroOT.val(carga.NumeroOT);

    knoutCarga.MotivoPendenciaFrete.val(carga.MotivoPendenciaFrete);

    knoutCarga.DescricaoSituacaoCarga.visible(!carga.DadosIntercab.AtivarNovosFiltrosConsultaCarga);
    knoutCarga.PendenciaTransportadorContribuinte.val(carga.PendenciaTransportadorContribuinte);
    knoutCarga.PendenciaValorLimiteApolice.val(carga.PendenciaValorLimiteApolice);

    var codigoVeiculo = carga.Veiculo.Codigo;
    var placas = carga.Veiculo.Descricao;
    var placasComModelo = carga.Veiculo.Descricao;
    var frota = !string.IsNullOrWhiteSpace(carga.Veiculo.NumeroFrota) ? (carga.Veiculo.NumeroFrota + ", ") : "";
    var placaReboque = "";
    var codigoReboque = 0;
    var codigoContainerReboque = "";
    var placaSegundoReboque = "";
    var placaTerceiroReboque = "";
    var codigoSegundoReboque = 0;
    var codigoTerceiroReboque = 0;
    var codigoContainerSegundoReboque = "";
    var codigoContainerTerceiroReboque = "";
    var dataRetiradaCtrnReboque = "";
    var dataRetiradaCtrnSegundoReboque = "";
    var dataRetiradaCtrnTerceiroReboque = "";
    var gensetReboque = "";
    var gensetSegundoReboque = "";
    var gensetTerceiroReboque = "";
    var maxGrossReboque = "";
    var maxGrossSegundoReboque = "";
    var maxGrossTerceiroReboque = "";
    var numeroContainerReboque = "";
    var numeroContainerSegundoReboque = "";
    var numeroContainerTerceiroReboque = "";
    var taraContainerReboque = "";
    var taraContainerSegundoReboque = "";
    var taraContainerTerceiroReboque = "";
    var anexosReboque = [];
    var anexosSegundoReboque = [];
    var anexosTerceiroReboque = [];

    if (carga.Veiculo.ModeloVeicularCarga != "") {
        placasComModelo += " (" + carga.Veiculo.ModeloVeicularCarga + ")";
    }

    if (!_CONFIGURACAO_TMS.PermiteEmissaoCargaSomenteComTracao && knoutCarga.ExigeConfirmacaoTracao.val()) {
        var veiculo = { Codigo: 0, Descricao: "" };
        var reboques = [];

        veiculo = { Codigo: carga.Veiculo.Codigo, Descricao: carga.Veiculo.Descricao };

        if (carga.Veiculo.VeiculosVinculados != null && carga.Veiculo.VeiculosVinculados.length > 0) {
            $.each(carga.Veiculo.VeiculosVinculados, function (i, veiculoVinculado) {
                reboques.push({
                    Codigo: veiculoVinculado.Codigo,
                    CodigoContainer: veiculoVinculado.CodigoContainer,
                    Descricao: veiculoVinculado.Descricao,
                    DataRetiradaCtrn: veiculoVinculado.DataRetiradaCtrn,
                    Genset: veiculoVinculado.Genset,
                    MaxGross: veiculoVinculado.MaxGross,
                    NumeroContainer: veiculoVinculado.NumeroContainer,
                    TaraContainer: veiculoVinculado.TaraContainer,
                    Anexos: veiculoVinculado.Anexos
                });
            });
        }

        if (reboques.length > 0) {
            placaReboque = reboques[0].Descricao;
            codigoReboque = reboques[0].Codigo;
            codigoContainerReboque = reboques[0].CodigoContainer;
            dataRetiradaCtrnReboque = reboques[0].DataRetiradaCtrn;
            gensetReboque = reboques[0].Genset;
            maxGrossReboque = reboques[0].MaxGross;
            numeroContainerReboque = reboques[0].NumeroContainer;
            taraContainerReboque = reboques[0].TaraContainer;
            anexosReboque = reboques[0].Anexos;

            if (knoutCarga.NumeroReboques.val() >= 2 && reboques.length > 1) {
                placaSegundoReboque = reboques[1].Descricao;
                codigoSegundoReboque = reboques[1].Codigo;
                codigoContainerSegundoReboque = reboques[1].CodigoContainer;
                dataRetiradaCtrnSegundoReboque = reboques[1].DataRetiradaCtrn;
                gensetSegundoReboque = reboques[1].Genset;
                maxGrossSegundoReboque = reboques[1].MaxGross;
                numeroContainerSegundoReboque = reboques[1].NumeroContainer;
                taraContainerSegundoReboque = reboques[1].TaraContainer;
                anexosSegundoReboque = reboques[1].Anexos;
            }

            if (knoutCarga.NumeroReboques.val() == 3 && reboques.length > 2 && knoutCarga.SegundoReboque.val() != null) {
                placaTerceiroReboque = reboques[2].Descricao;
                codigoTerceiroReboque = reboques[2].Codigo;
                codigoContainerTerceiroReboque = reboques[2].CodigoContainer;
                dataRetiradaCtrnTerceiroReboque = reboques[2].DataRetiradaCtrn;
                gensetTerceiroReboque = reboques[2].Genset;
                maxGrossTerceiroReboque = reboques[2].MaxGross;
                numeroContainerTerceiroReboque = reboques[2].NumeroContainer;
                taraContainerTerceiroReboque = reboques[2].TaraContainer;
                anexosTerceiroReboque = reboques[2].Anexos;
            }
        }

        placasComModelo = [veiculo.Descricao, placaReboque, placaSegundoReboque, placaTerceiroReboque].filter(function (p) { return p != "" }).join(", ");
        codigoVeiculo = veiculo.Codigo;
        placas = veiculo.Descricao;
    }
    else if (carga.Veiculo.VeiculosVinculados != null) {
        $.each(carga.Veiculo.VeiculosVinculados, function (i, veiculoVinculado) {
            placas += (placas != "" ? ", " : "") + veiculoVinculado.Descricao;
            placasComModelo += ", " + veiculoVinculado.Descricao;
            if (!string.IsNullOrWhiteSpace(veiculoVinculado.ModeloVeicularCarga)) {
                placas += " (" + veiculoVinculado.ModeloVeicularCarga + ")";
                placasComModelo += " (" + veiculoVinculado.ModeloVeicularCarga + ")";
            }
            if (!string.IsNullOrWhiteSpace(veiculoVinculado.NumeroFrota))
                frota += veiculoVinculado.NumeroFrota + ", ";
        });
    }

    frota = string.Left(frota, frota.length - 2);

    knoutCarga.ExigirDataRetiradaCtrnVeiculos.val(carga.ExigirDataRetiradaCtrnVeiculos);
    knoutCarga.ExigirNumeroContainerVeiculos.val(carga.ExigirNumeroContainerVeiculos);
    knoutCarga.PlacasCarragamento.val(carga.Veiculo.VeiculosVinculados);
    knoutCarga.CodigoContainerVeiculo.val(carga.CodigoContainerVeiculo);
    knoutCarga.Veiculo.placas(placasComModelo);
    knoutCarga.Veiculo.val(placas);

    if (carga.UltimaPosicao) {
        knoutCarga.Monitoramento.val(carga.UltimaPosicao.Data);
        knoutCarga.UltimaPosicaoRastreador.val(carga.UltimaPosicao.Rastreador);
        knoutCarga.RastreadorOnlineOffline.val(carga.UltimaPosicao.RastreadorOnlineOffline);
        knoutCarga.StatusMonitoramento.val(carga.UltimaPosicao.StatusMonitoramento);
    } else {
        knoutCarga.RastreadorOnlineOffline.val(EnumStatusAcompanhamento.ComMonitoramentoSemPosicao);
    }

    knoutCarga.Veiculo.entityDescription(placas);
    knoutCarga.DataRetiradaCtrnVeiculo.val(carga.DataRetiradaCtrnVeiculo);
    knoutCarga.GensetVeiculo.val(carga.GensetVeiculo);
    knoutCarga.MaxGrossVeiculo.val(carga.MaxGrossVeiculo);
    knoutCarga.NumeroContainerVeiculo.val(carga.NumeroContainerVeiculo);
    knoutCarga.TaraContainerVeiculo.val(carga.TaraContainerVeiculo);

    knoutCarga.CodigoContainerReboque.val(codigoContainerReboque);
    knoutCarga.Reboque.val(placaReboque);
    knoutCarga.Reboque.entityDescription(placaReboque);
    knoutCarga.Reboque.codEntity(codigoReboque);
    knoutCarga.DataRetiradaCtrnReboque.val(dataRetiradaCtrnReboque);
    knoutCarga.GensetReboque.val(gensetReboque);
    knoutCarga.MaxGrossReboque.val(maxGrossReboque);
    knoutCarga.NumeroContainerReboque.val(numeroContainerReboque);
    knoutCarga.TaraContainerReboque.val(taraContainerReboque);
    knoutCarga.ValidarMotoristaTelerisco.val(carga.ValidarMotoristaTelerisco);
    knoutCarga.ValidarIntegracaoPlacaBRK.val(carga.ValidarIntegracaoPlacaBRK);
    knoutCarga.ValidarMotoristaBRK.val(carga.ValidarMotoristaBRK);
    knoutCarga.MotoristaValidadoBrasilRisk.val(carga.MotoristaValidadoBrasilRisk);
    knoutCarga.PlacaValidadoBrasilRisk.val(carga.PlacaValidadoBrasilRisk);

    knoutCarga.CodigoContainerSegundoReboque.val(codigoContainerSegundoReboque);
    knoutCarga.SegundoReboque.val(placaSegundoReboque);
    knoutCarga.SegundoReboque.entityDescription(placaSegundoReboque);
    knoutCarga.SegundoReboque.codEntity(codigoSegundoReboque);
    knoutCarga.DataRetiradaCtrnSegundoReboque.val(dataRetiradaCtrnSegundoReboque);
    knoutCarga.GensetSegundoReboque.val(gensetSegundoReboque);
    knoutCarga.MaxGrossSegundoReboque.val(maxGrossSegundoReboque);
    knoutCarga.NumeroContainerSegundoReboque.val(numeroContainerSegundoReboque);
    knoutCarga.TaraContainerSegundoReboque.val(taraContainerSegundoReboque);

    knoutCarga.CodigoContainerTerceiroReboque.val(codigoContainerTerceiroReboque);
    knoutCarga.TerceiroReboque.val(placaTerceiroReboque);
    knoutCarga.TerceiroReboque.entityDescription(placaTerceiroReboque);
    knoutCarga.TerceiroReboque.codEntity(codigoTerceiroReboque);
    knoutCarga.DataRetiradaCtrnTerceiroReboque.val(dataRetiradaCtrnTerceiroReboque);
    knoutCarga.GensetTerceiroReboque.val(gensetTerceiroReboque);
    knoutCarga.MaxGrossTerceiroReboque.val(maxGrossTerceiroReboque);
    knoutCarga.NumeroContainerTerceiroReboque.val(numeroContainerTerceiroReboque);
    knoutCarga.TaraContainerTerceiroReboque.val(taraContainerTerceiroReboque);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) {
        knoutCarga.ContainerReboqueAnexo.preencherAnexos(anexosReboque);
        knoutCarga.ContainerSegundoReboqueAnexo.preencherAnexos(anexosSegundoReboque);
        knoutCarga.ContainerVeiculoAnexo.preencherAnexos(carga.AnexosVeiculo);
    }

    knoutCarga.Frota.val(frota);
    knoutCarga.EstaEmParqueamento.val(carga.Veiculo.EstaEmParqueamento);
    knoutCarga.Veiculo.codEntity(codigoVeiculo);
    knoutCarga.AgConfirmacaoUtilizacaoCredito.val(carga.AgConfirmacaoUtilizacaoCredito);
    knoutCarga.AutorizarEmissaoDocumentos.visibleCTeProcessamento(false);
    knoutCarga.AguardandoNFs.val(carga.AguardandoNFs);
    PreecherInformacaoValorFrete(knoutCarga, carga.ValorFrete);

    knoutCarga.VerSimulacaoFrete.visible(carga.PossuiSimulacaoFrete);
    knoutCarga.ObservacaoInformadaPeloTransportador.val(carga.ObservacaoInformadaPeloTransportador);
    knoutCarga.ObservacaoCarregamentoRoteirizacao.val(carga.ObservacaoCarregamentoRoteirizacao);

    if (_ehMultiEmbarcador || _ehTransportador) knoutCarga.ObservacaoTransportador.val(carga.ObservacaoTransportador);

    setaNumeroPedidoCliente(carga.Pedidos, knoutCarga);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        var numerosPedidosEmbarcador = new Array();
        var numerosBooking = new Array();
        var numeroContaineres = new Array();
        var transbordos = new Array();
        var empresasResponsaveis = new Array();
        var centrosCustos = new Array();

        for (var i = 0; i < carga.Pedidos.length; i++) {
            var numeroPedidoEmbarcador = carga.Pedidos[i].NumeroPedidoEmbarcador;
            var numeroBooking = carga.Pedidos[i].NumeroBooking;
            var numeroContainer = carga.Pedidos[i].NumeroContainer;
            var empresaResponsavel = carga.Pedidos[i].EmpresaResponsavel;
            var centroCusto = carga.Pedidos[i].CentroCusto;

            if (!string.IsNullOrWhiteSpace(numeroPedidoEmbarcador))
                numerosPedidosEmbarcador.push(numeroPedidoEmbarcador);

            if (!string.IsNullOrWhiteSpace(numeroBooking)) {
                if (numerosBooking.indexOf(numeroBooking) <= -1)
                    numerosBooking.push(numeroBooking);
            }

            if (!string.IsNullOrWhiteSpace(numeroContainer))
                numeroContaineres.push(numeroContainer);

            if (!string.IsNullOrWhiteSpace(empresaResponsavel))
                empresasResponsaveis.push(empresaResponsavel);

            if (!string.IsNullOrWhiteSpace(centroCusto))
                centrosCustos.push(centroCusto);
        }

        if (carga.Transbordos != null && carga.Transbordos != undefined) {
            for (var i = 0; i < carga.Transbordos.length; i++) {
                var transbordo = carga.Transbordos[i].Sequencia + " - " + carga.Transbordos[i].Navio + " - " + carga.Transbordos[i].Porto;

                if (!string.IsNullOrWhiteSpace(transbordo))
                    transbordos.push(transbordo);
            }
        }

        if (numerosPedidosEmbarcador.length > 0) {
            knoutCarga.NumeroPedidoEmbarcador.val(numerosPedidosEmbarcador.join(", "));
            knoutCarga.NumeroPedidoEmbarcador.visible(true);
        }

        if (numerosBooking.length > 0) {
            knoutCarga.NumeroBooking.val(numerosBooking.join(", "));
            knoutCarga.NumeroBooking.visible(true);
        }

        if (numeroContaineres.length > 0) {
            knoutCarga.NumeroContainer.val(numeroContaineres.join(", "));
            knoutCarga.NumeroContainer.visible(true);
        }

        if (transbordos.length > 0) {
            knoutCarga.DescricaoTransbordos.val(transbordos.join(", "));
            knoutCarga.DescricaoTransbordos.visible(true);
        }

        if (empresasResponsaveis.length > 0) {
            knoutCarga.PedidoEmpresaResponsavel.val(empresasResponsaveis.join(", "));
            knoutCarga.PedidoEmpresaResponsavel.visible(true);
        }

        if (centrosCustos.length > 0) {
            knoutCarga.PedidoCentroCusto.val(centrosCustos.join(", "));
            knoutCarga.PedidoCentroCusto.visible(true);
        }
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && carga.ExibirNumeroPedidoNosDetalhesDaCarga) {
        var numerosPedidosEmbarcador = new Array();

        for (var i = 0; i < carga.Pedidos.length; i++) {
            var numeroPedidoEmbarcador = carga.Pedidos[i].NumeroPedidoEmbarcador;

            if (!string.IsNullOrWhiteSpace(numeroPedidoEmbarcador))
                numerosPedidosEmbarcador.push(numeroPedidoEmbarcador);
        }

        if (numerosPedidosEmbarcador.length > 0) {
            knoutCarga.NumeroPedidoEmbarcador.val(numerosPedidosEmbarcador.join(", "));
            knoutCarga.NumeroPedidoEmbarcador.visible(true);
        }
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && carga.TipoOperacao.HabilitarTipoPagamentoValePedagio) {
        knoutCarga.TipoPagamentoValePedagio.visible(true);
        knoutCarga.DescricaoTipoPagamentoValePedagio.visible(true);
        knoutCarga.TipoPagamentoValePedagio.val(carga.TipoOperacao.TipoPagamentoValePedagio);
        knoutCarga.DescricaoTipoPagamentoValePedagio.val(EnumTipoPagamentoValePedagio.obterDescricao(carga.TipoOperacao.TipoPagamentoValePedagio));
    }

    if (carga.possuiCTe && carga.PossuiNFs) {
        knoutCarga.EtapaCTeNFs.text(Localization.Resources.Cargas.Carga.CtesNfse);
        knoutCarga.EtapaCTeNFs.tooltip(Localization.Resources.Cargas.Carga.OndeOsCtesAsNfsesNecessariosParaTransporteSaoEmitidosOuImportados);
        knoutCarga.EtapaCTeNFs.tooltipTitle(Localization.Resources.Cargas.Carga.CtesNfse);
    } else if (carga.possuiCTe && carga.possuiNFSManual) {
        knoutCarga.EtapaCTeNFs.text(Localization.Resources.Cargas.Carga.CteNfsManual);
        knoutCarga.EtapaCTeNFs.tooltip(Localization.Resources.Cargas.Carga.OndeOsCtesNecessariosParaTransporteSaoEmitidosOuImportadosAsNotasFiscaisDeServicoGeradasManualenteSaoListadas);
        knoutCarga.EtapaCTeNFs.tooltipTitle(Localization.Resources.Cargas.Carga.CteNfsManual);
    } else if (carga.PossuiNFs) {
        knoutCarga.EtapaCTeNFs.text(Localization.Resources.Cargas.Carga.Nfse);
        knoutCarga.EtapaCTeNFs.tooltip(Localization.Resources.Cargas.Carga.OndeAsNfsesNecessariosParaTransporteSaoEmitidasOuImportadas);
        knoutCarga.EtapaCTeNFs.tooltipTitle(Localization.Resources.Cargas.Carga.Nfse);
    } else if (carga.possuiNFSManual) {
        knoutCarga.EtapaCTeNFs.text(Localization.Resources.Cargas.Carga.NfsManual);
        knoutCarga.EtapaCTeNFs.tooltip(Localization.Resources.Cargas.Carga.OndeAsNotasFiscaisDeServicoGeradasManualmenteSaoListadas);
        knoutCarga.EtapaCTeNFs.tooltipTitle(Localization.Resources.Cargas.Carga.NfsManual);
    }

    if (carga.TipoServicoCarga == EnumTipoServicoCarga.Feeder && _CONFIGURACAO_TMS.ModificarTimelineDeAcordoComTipoServicoDocumento) {
        knoutCarga.EtapaNotaFiscal.text(Localization.Resources.Cargas.Carga.CEMercante);
        knoutCarga.EtapaNotaFiscal.tooltip(Localization.Resources.Cargas.Carga.OndeOsCEMercanteSaoEnviadosParaCarga);
        knoutCarga.EtapaNotaFiscal.tooltipTitle(Localization.Resources.Cargas.Carga.CEMercanteParaAnterior);
        knoutCarga.RetornarParaEtapaNFeTMS.text(Localization.Resources.Cargas.Carga.RetornarParaCEMercante);
    }
    else if (((carga.TipoContratacaoCarga == EnumTipoContratacaoCarga.SVMTerceiro || carga.TipoContratacaoCarga == EnumTipoContratacaoCarga.SVMProprio
        || carga.TipoContratacaoCarga == EnumTipoContratacaoCarga.SubContratada || carga.TipoContratacaoCarga == EnumTipoContratacaoCarga.Redespacho
        || carga.TipoContratacaoCarga == EnumTipoContratacaoCarga.RedespachoIntermediario) && !carga.PossuiCTeSubcontratacaoFilialEmissora)
        || carga.PossuiCTeAnteriorFilialEmissora || carga.ExclusivaDeSubcontratacaoOuRedespacho) {
        knoutCarga.EtapaNotaFiscal.text(Localization.Resources.Cargas.Carga.CteAnterior);
        knoutCarga.EtapaNotaFiscal.tooltip(Localization.Resources.Cargas.Carga.OndeOsCtesAnterioresSaoEnviadosParaCarga);
        knoutCarga.EtapaNotaFiscal.tooltipTitle(Localization.Resources.Cargas.Carga.CteParaAnterior);
        knoutCarga.RetornarParaEtapaNFeTMS.text(Localization.Resources.Cargas.Carga.RetornarParaCteAnterior);
    } else {
        if (knoutCarga.Mercosul.val() === true || knoutCarga.Internacional.val() === true)
            knoutCarga.EtapaNotaFiscal.text(Localization.Resources.Cargas.Carga.Factura);
        else
            knoutCarga.EtapaNotaFiscal.text(Localization.Resources.Cargas.Carga.NFe);

        knoutCarga.EtapaNotaFiscal.tooltip(Localization.Resources.Cargas.Carga.OndeAsNotasFiscaisDaCargaSaoEnviadas);
        knoutCarga.EtapaNotaFiscal.tooltipTitle(Localization.Resources.Cargas.Carga.NotasFiscais);
        knoutCarga.RetornarParaEtapaNFeTMS.text(Localization.Resources.Cargas.Carga.RetornarParaNfe);
    }

    if (carga.CodigoCargaOrigemCancelada > 0) {
        knoutCarga.CodigoCargaOrigemCancelada.val(carga.CodigoCargaOrigemCancelada);
        knoutCarga.CodigoCargaOrigemCancelada.visible(true);
    }

    if (carga.CargaPossuiOutrosNumerosEmbarcador)
        knoutCarga.NumeroCargaOriginais.tooltipTitle(Localization.Resources.Cargas.Carga.OutrosNumerosDaCarga.getFieldDescription());

    configurarOpcoesVisiveisPorCarga(knoutCarga, carga);
}

function InformarEstadosDasEtapas(knoutCarga, carga, idDivCarga) {
    $("#" + idDivCarga + "_ribbonTakeOrPay").hide();
    $("#" + idDivCarga + "_ribbonDemurrage").hide();
    $("#" + idDivCarga + "_ribbonDetention").hide();

    if (carga.Operador == "")
        $("#" + idDivCarga + "_ribbonCargaNova").show();

    if (carga.CargaTransbordo)
        $("#" + idDivCarga + "_ribbonTransbordo").show();

    if (carga.TipoContratacaoCarga == EnumTipoContratacaoCarga.SVMProprio || carga.TipoContratacaoCarga == EnumTipoContratacaoCarga.Redespacho || carga.TipoContratacaoCarga == EnumTipoContratacaoCarga.RedespachoIntermediario || !string.IsNullOrWhiteSpace(carga.Redespacho)) {
        knoutCarga.SubContratante.visible(true);

        if (carga.SituacaoCarga != EnumSituacoesCarga.Cancelada && carga.SituacaoCarga != EnumSituacoesCarga.Anulada && !(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS && carga.LegendaCargaAcordoTipoOperacao == EnumTipoServicoMultimodal.Subcontratacao && carga.VisualizarLegendaCargaAcordoTipoOperacao))
            $("#" + idDivCarga + "_ribbonRedespacho").show();
    }
    else {
        knoutCarga.SubContratante.visible(false);
        $("#" + idDivCarga + "_ribbonRedespacho").hide();
    }

    if (knoutCarga.CargaRedespacho.val()) {
        if (carga.SituacaoCarga != EnumSituacoesCarga.Cancelada && carga.SituacaoCarga != EnumSituacoesCarga.Anulada && !(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS && carga.LegendaCargaAcordoTipoOperacao == EnumTipoServicoMultimodal.Subcontratacao && carga.VisualizarLegendaCargaAcordoTipoOperacao))
            $("#" + idDivCarga + "_ribbonRedespacho").show();
    }

    if (knoutCarga.CargaColeta.val())
        $("#" + idDivCarga + "_ribbonColeta").show();

    if (knoutCarga.CargaSVM.val()) {
        if (carga.SituacaoCarga != EnumSituacoesCarga.Cancelada && carga.SituacaoCarga != EnumSituacoesCarga.Anulada) {
            $("#" + idDivCarga + "_ribbonRedespacho").hide();
            $("#" + idDivCarga + "_ribbonSVMTerceiro").hide();
            $("#" + idDivCarga + "_ribbonTakeOrPay").hide();
            $("#" + idDivCarga + "_ribbonDemurrage").hide();
            $("#" + idDivCarga + "_ribbonDetention").hide();
            $("#" + idDivCarga + "_ribbonSVM").show();
        }
    }

    if (knoutCarga.CargaSVMTerceiro.val()) {
        if (carga.SituacaoCarga != EnumSituacoesCarga.Cancelada && carga.SituacaoCarga != EnumSituacoesCarga.Anulada) {
            $("#" + idDivCarga + "_ribbonRedespacho").hide();
            $("#" + idDivCarga + "_ribbonSVM").hide();
            $("#" + idDivCarga + "_ribbonSVMTerceiro").show();
        }
    }

    if (knoutCarga.CargaTakeOrPay.val()) {
        if (carga.SituacaoCarga != EnumSituacoesCarga.Cancelada && carga.SituacaoCarga != EnumSituacoesCarga.Anulada) {
            $("#" + idDivCarga + "_ribbonRedespacho").hide();
            $("#" + idDivCarga + "_ribbonSVMTerceiro").hide();
            $("#" + idDivCarga + "_ribbonDemurrage").hide();
            $("#" + idDivCarga + "_ribbonDetention").hide();
            $("#" + idDivCarga + "_ribbonSVM").hide();
            $("#" + idDivCarga + "_ribbonTakeOrPay").show();
            if (knoutCarga.CargaDemurrage.val()) {
                $("#" + idDivCarga + "_ribbonTakeOrPay").hide();
                $("#" + idDivCarga + "_ribbonDemurrage").show();
            }
            if (knoutCarga.CargaDetention.val()) {
                $("#" + idDivCarga + "_ribbonTakeOrPay").hide();
                $("#" + idDivCarga + "_ribbonDetention").show();
            }
        }
    }

    if (carga.TipoContratacaoCarga == EnumTipoContratacaoCarga.SubContratada) {
        knoutCarga.SubContratante.visible(true);

        if (carga.SituacaoCarga != EnumSituacoesCarga.Cancelada && carga.SituacaoCarga != EnumSituacoesCarga.Anulada) {
            $("#" + idDivCarga + "_ribbonRedespacho").hide();
            $("#" + idDivCarga + "_ribbonCargaSubcontratada").show();
        }
    }
    else {
        knoutCarga.SubContratante.visible(false);
        $("#" + idDivCarga + "_ribbonCargaSubcontratada").hide();
    }

    if (carga.FreteDeTerceiro) { //&& _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador) {
        knoutCarga.SubContratado.visible(true);

        if (carga.SituacaoCarga != EnumSituacoesCarga.Cancelada && carga.SituacaoCarga != EnumSituacoesCarga.Anulada)
            $("#" + idDivCarga + "_ribbonSubcontratacao").show();
    }
    else {
        knoutCarga.SubContratado.visible(false);
        $("#" + idDivCarga + "_ribbonSubcontratacao").hide();
    }

    if (carga.Desistencia)
        $("#" + idDivCarga + "_ribbonDesistencia").show();
    else
        $("#" + idDivCarga + "_ribbonDesistencia").hide();

    var cargaCancelamento = InformarEstadosDasEtapasCancelamento(knoutCarga, carga, idDivCarga);

    if (!cargaCancelamento) {
        if (carga.SituacaoCarga == EnumSituacoesCarga.Nova)
            InformarEstadosDasEtapasCargaNova(knoutCarga, carga, idDivCarga);
        else {
            if (carga.Operador != "" || (carga.SituacaoCarga != EnumSituacoesCarga.CalculoFrete
                && carga.SituacaoCarga != EnumSituacoesCarga.AgTransportador)) {
                $("#" + idDivCarga + "_ribbonCargaNova").hide();
            }
            SetarSituacaoCarga(knoutCarga, carga, idDivCarga);
        }

        if (carga.CargaDePreCarga)
            $("#" + idDivCarga + "_ribbonPreCarga").show();

        if (carga.CargaTrocaDeNota)
            $("#" + idDivCarga + "_ribbonTrocaNota").show();
        else
            $("#" + idDivCarga + "_ribbonTrocaNota").hide();

        if (carga.Reentrega)
            $("#" + idDivCarga + "_ribbonReentrega").show();
        else
            $("#" + idDivCarga + "_ribbonReentrega").hide();

        if (carga.NaoComparecido != EnumTipoNaoComparecimento.Compareceu) {
            $("#" + idDivCarga + "_ribbonNoShow").show();
            $("#" + idDivCarga + "_ribbonNoShow-text").text(EnumTipoNaoComparecimento.obterDescricao(carga.NaoComparecido));
        }
        else
            $("#" + idDivCarga + "_ribbonNoShow").hide();

        if (knoutCarga.CargaDestinadaCTeComplementar.val()) {
            $("#" + idDivCarga + "_ribbonRedespacho").hide();
            $("#" + idDivCarga + "_ribbonSVMTerceiro").hide();
            $("#" + idDivCarga + "_ribbonTakeOrPay").hide();
            $("#" + idDivCarga + "_ribbonDemurrage").hide();
            $("#" + idDivCarga + "_ribbonDetention").hide();
            $("#" + idDivCarga + "_ribbonSubcontratacao").hide();
            $("#" + idDivCarga + "_ribbonComplementar").show();
        }
        else
            $("#" + idDivCarga + "_ribbonComplementar").hide();
    }
    else
        SetarSituacaoCarga(knoutCarga, carga, idDivCarga);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        knoutCarga.AdicionarComplementoFrete.visible(false);
        knoutCarga.Empresa.visible(true);

        if (carga.VisualizarLegendaCargaAcordoTipoOperacao && !cargaCancelamento) {
            $("#" + idDivCarga + "_ribbonCargaSubcontratadaMultiModalTMS-text").text(Localization.Resources.Cargas.Carga.Subcontratacao);
            $("#" + idDivCarga + "_ribbonVinculadoMultimodalTerceiro-text").text(Localization.Resources.Cargas.Carga.MultimodalTerceiro);

            if (carga.LegendaCargaAcordoTipoOperacao == EnumTipoServicoMultimodal.Subcontratacao)
                $("#" + idDivCarga + "_ribbonCargaSubcontratadaMultiModalTMS").show();
            else if (carga.LegendaCargaAcordoTipoOperacao == EnumTipoServicoMultimodal.VinculadoMultimodalTerceiro)
                $("#" + idDivCarga + "_ribbonVinculadoMultimodalTerceiro").show();
        }
    }

    if (carga.PossuiRotaDefinida && !carga.OcultarVisualizarRotaMapa)
        knoutCarga.VisualizarRotaMapa.visible(true);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
        knoutCarga.AlterarRotaFreteCarga.text(Localization.Resources.Cargas.Carga.BuscarRota);

    if (cargaCancelamento)
        EtapaImpressaoEdicaoDesabilitada(knoutCarga);

    if (_FormularioSomenteLeitura || (carga.OrdemEmbarqueSituacaoIntegracao == EnumSituacaoIntegracao.AgRetorno))
        desabilitarTodasOpcoes(knoutCarga);
}

function SetarSituacaoCarga(knoutCarga, carga, idDivCarga) {
    EtapaTransbordoDesabilitada(knoutCarga);
    var exibeContrato = false;
    switch (carga.SituacaoCarga) {
        case EnumSituacoesCarga.CalculoFrete:
            InformarEstadosDasEtapasCalculoFrete(knoutCarga, carga, idDivCarga);
            break;
        case EnumSituacoesCarga.AgTransportador:
            InformarEstadosDasEtapasAgTransportador(knoutCarga, carga, idDivCarga);
            break;
        case EnumSituacoesCarga.AgNFe:
            InformarEstadosDasEtapasAgNFe(knoutCarga, carga, idDivCarga);
            break;
        case EnumSituacoesCarga.PendeciaDocumentos:
            InformarEstadosDasEtapasPendeciaDocumentos(knoutCarga, carga, idDivCarga);
            break;
        case EnumSituacoesCarga.AgImpressaoDocumentos:
            InformarEstadosDasEtapasAgImpressaoDocumentos(knoutCarga, carga, idDivCarga);
            exibeContrato = true;
            break;
        case EnumSituacoesCarga.AgIntegracao:
            InformarEstadosDasEtapasAgIntegracao(knoutCarga, carga, idDivCarga);
            break;
        case EnumSituacoesCarga.ProntoTransporte:
            InformarEstadosDasEtapasTransporte(knoutCarga, carga, idDivCarga);
            exibeContrato = true;
            break;
        case EnumSituacoesCarga.EmTransporte:
            InformarEstadosDasEtapasTransporte(knoutCarga, carga, idDivCarga);
            exibeContrato = true;
            break;
        case EnumSituacoesCarga.LiberadoPagamento:
            InformarEstadosDasEtapasTransporte(knoutCarga, carga, idDivCarga);
            exibeContrato = true;
            break;
        case EnumSituacoesCarga.Encerrada:
            InformarEstadosDasEtapasTransporte(knoutCarga, carga, idDivCarga);
            exibeContrato = true;
            break;
        case EnumSituacoesCarga.EmTransbordo:
            InformarEstadosDasEtapasTransporte(knoutCarga, carga, idDivCarga);
            break;

    }

    if (_CONFIGURACAO_TMS.TipoContratoFreteTerceiro == EnumTipoContratoFreteTerceiro.PorCarga && carga.FreteDeTerceiro) {
        if (exibeContrato)
            InformarEtapaSubContratacao(knoutCarga, carga, idDivCarga);
        else
            EtapaSubContratacaoDesabilitada(knoutCarga);
    }

}

function InformarEtapaTransbordo(knoutCarga, carga, idDivCarga) {
    if (carga.Transbordo.Problemas) {
        EtapaTransbordoProblema(knoutCarga);
    } else if (carga.Transbordo.Pendencia) {
        EtapaTransbordoAguardando(knoutCarga);
    } else if (carga.Transbordo.Completos) {
        EtapaTransbordoAprovada(knoutCarga);
    } else {
        EtapaTransbordoLiberada(knoutCarga);
    }
}

function InformarEtapaSubContratacao(knoutCarga, carga, idDivCarga) {
    var contratoFrete = carga.ContratoFreteTerceiro;
    if (contratoFrete != null) {
        if (contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.AgAprovacao || contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.Aberto || contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.SemRegra) {
            EtapaSubContratacaoAguardando(knoutCarga);
        } else if (contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.Aprovado || contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.Finalizada) {
            EtapaSubContratacaoAprovada(knoutCarga);
        } else if (contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.Cancelado || contratoFrete.SituacaoContratoFrete == EnumSituacaoContratoFrete.Rejeitado) {
            EtapaSubContratacaoProblema(knoutCarga);
        }
    }
}

function InformarEstadosDasEtapasCargaNova(knoutCarga, carga, idDivCarga) {
    $("#" + idDivCarga + "_ribbonCargaNova").show();

    if (!carga.PossuiPendencia) {
        if (carga.AguardarIntegracaoDadosTransporte === true) {
            EtapaInicioEmbarcadorAguardando(knoutCarga);
            EtapaInicioTMSAguardando(knoutCarga);
        } else if (_CONFIGURACAO_TMS.PossuiIntegracaoBRKVeiculoEMotorista && (!knoutCarga.MotoristaValidadoBrasilRisk.val() || !knoutCarga.PlacaValidadoBrasilRisk.val())) {
            EtapaInicioTMSAlerta(knoutCarga);
        }
        else {
            EtapaInicioEmbarcadorLiberada(knoutCarga);
            EtapaInicioTMSLiberada(knoutCarga);
        }
    } else {
        EtapaInicioEmbarcadorProblema(knoutCarga);
        EtapaInicioTMSProblema(knoutCarga);

        if (carga.MotivoPendencia != "") {
            var html = "<div class='alert alert-info alert-block'>";
            html += "<h4 class='alert-heading'>" + Localization.Resources.Cargas.Carga.Pendencia + "</h4>";
            html += carga.MotivoPendencia;
            html += "</div>";

            $("#" + knoutCarga.EtapaInicioEmbarcador.idGrid + " .MensageCarga").html(html);
            $("#" + knoutCarga.EtapaInicioEmbarcador.idGrid + " .DivMensagemCarga").show();
        } else {
            $("#" + knoutCarga.EtapaInicioEmbarcador.idGrid + " .DivMensagemCarga").hide();
        }
    }

    if (!_CONFIGURACAO_TMS.OcultarBuscaRotaNaCarga && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) {
        knoutCarga.AlterarRotaFreteCarga.visible(true);
    }
}

function InformarEstadosDasEtapasCancelamento(knoutCarga, carga, idDivCarga) {

    var cargaCancelamento = false;

    if (carga.SituacaoCarga == EnumSituacoesCarga.Cancelada) {
        cargaCancelamento = true;
        $("#" + idDivCarga + "_ribbonCargaCancelada").show();
        $("#" + idDivCarga + "_ribbonCargaCancelada_marca").show();
    } else if (carga.SituacaoCarga == EnumSituacoesCarga.EmCancelamento) {
        cargaCancelamento = true;
        $("#" + idDivCarga + "_ribbonCargaEmCancelamentoRibbon").show();
    } else if (carga.SituacaoCarga == EnumSituacoesCarga.RejeicaoCancelamento) {
        cargaCancelamento = true;
        $("#" + idDivCarga + "_ribbonCargaCancelamentoRejeitadoRibbon").show();
        knoutCarga.MensagemRejeicaoDeCancelamento.val(carga.CargaCancelamento.MensagemRejeicaoCancelamento)
        knoutCarga.MensagemRejeicaoDeCancelamento.visible(true);
    } else if (carga.SituacaoCarga == EnumSituacoesCarga.Anulada) {
        knoutCarga.MotivoDoCancelamento.text(Localization.Resources.Cargas.Carga.MotivoDaAnulacao.getFieldDescription());
        cargaCancelamento = true;
        $("#" + idDivCarga + "_ribbonAnulacao").show();
        $("#" + idDivCarga + "_ribbonAnulacao_marca").show();
    }

    if (cargaCancelamento) {
        carga.SituacaoCarga = carga.CargaCancelamento.SituacaoCargaNoCancelamento;
        knoutCarga.MotivoDoCancelamento.val(carga.CargaCancelamento.MotivoDoCancelamento);
        knoutCarga.MotivoDoCancelamento.visible(true);
        $("#" + idDivCarga + "_ribbonRefaturamento").hide();
        $("#" + idDivCarga + "_ribbonRedespacho").hide();
        $("#" + idDivCarga + "_ribbonCargaSubcontratada").hide();
        $("#" + idDivCarga + "_ribbonSubcontratacao").hide();
        $("#" + idDivCarga + "_ribbonTransbordo").hide();
    }

    return cargaCancelamento;
}

function InformarEstadosDasEtapasCalculoFrete(knoutCarga, carga, idDivCarga) {
    if (knoutCarga.ExigeNotaFiscalParaCalcularFrete.val()) {
        if (carga.EmissaoDocumentosAutorizada)
            EtapaFreteTMSLiberada(knoutCarga);
        else
            EtapaFreteTMSAguardando(knoutCarga);
    }
    else {
        if (carga.SituacaoAlteracaoFreteCarga == EnumSituacaoAlteracaoFreteCarga.AguardandoAprovacao)
            EtapaFreteEmbarcadorAguardando(knoutCarga);
        else
            EtapaFreteEmbarcadorLiberada(knoutCarga);
    }

    if (carga.PossuiPendencia && !carga.OrigemFretePelaJanelaTransportador) {

        knoutCarga.ValorFreteOperador.visible(false);

        if (carga.MotivoPendenciaFrete == EnumMotivoPendenciaFrete.AgOperador) {
            EtapaFreteEmbarcadorAguardando(knoutCarga);
        }
        else if (carga.MotivoPendenciaFrete == EnumMotivoPendenciaFrete.DivergenciaPreCalculoFrete) {
            SetarEtapaFretePorExigenciaNotaFiscal(knoutCarga);

            knoutCarga.ValorFreteOperador.visible(false);
            knoutCarga.RecalcularFrete.visible(false);
            knoutCarga.RecalcularFreteBID.visible(false);
            knoutCarga.AtualizarValorFrete.visible(false);
        } else {
            SetarEtapaFretePorExigenciaNotaFiscal(knoutCarga);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
                permitirInformarValorFreteManualmente(knoutCarga);
        }
    }

    if (carga.EmissaoDocumentosAutorizada) {
        knoutCarga.AutorizarEmissaoDocumentos.visibleBTN(false);

        var agCiencia = false;

        $.each(knoutCarga.Pedidos.val, function (i, pedido) {
            if (_CONFIGURACAO_TMS.ExigeInformarCienciaDoEnvioDasNotasAntesDeEmitirDocumentos && !pedido.CienciaDoEnvioDaNotaInformado) {
                knoutCarga.AguardandoEmissaoDocumentoAnterior.val(Localization.Resources.Cargas.Carga.AguardandoErpConfirmarRecebimentoDasInformacoesParaIniciarEmissao);
                agCiencia = true;
                knoutCarga.AutorizarEmissaoDocumentos.visibleCTeProcessamento(false);
                knoutCarga.AguardandoEmissaoDocumentoAnterior.visible(true);

                if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_LiberarCargaSemConfirmacaoERP, _PermissoesPersonalizadasCarga)) {
                    knoutCarga.LiberarSemConfirmacaoERP.visible(true);
                    knoutCarga.LiberarSemConfirmacaoERP.enable(true);
                }

                return false;
            }
        });
        if (!agCiencia) {
            if (!carga.AguardandoEmissaoDocumentoAnterior) {
                knoutCarga.AutorizarEmissaoDocumentos.visibleCTeProcessamento(true);
            }
            else {
                knoutCarga.AutorizarEmissaoDocumentos.visibleCTeProcessamento(false);
                knoutCarga.AguardandoEmissaoDocumentoAnterior.visible(true);
                knoutCarga.AguardandoEmissaoDocumentoAnterior.val(Localization.Resources.Cargas.Carga.AguardandoTerminoDaEmissaoDosDocumentosDoTrechoAnteriorParaIniciarEmissaoDoRedespacho);
            }
        }
        EtapaFreteEmbarcadorEdicaoDesabilitada(knoutCarga);
    } else {
        knoutCarga.AutorizarEmissaoDocumentos.visibleCTeProcessamento(false);
    }
    if (!_CONFIGURACAO_TMS.OcultarBuscaRotaNaCarga && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) {
        knoutCarga.AlterarRotaFreteCarga.visible(true);
    }

    if (carga.CargaPossuiPreCalculoFrete) {
        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermiteExcluirPreCalculoFrete, _PermissoesPersonalizadasCarga)) {
            knoutCarga.ExcluirPreCalculo.visible(true);
            knoutCarga.RecalcularFrete.visible(false);
        }
    }
    knoutCarga.ValorPorPedido.visible(true);
    knoutCarga.ValorPorPedidoFilialEmissora.visible(true);
}

function SetarEtapaFretePorExigenciaNotaFiscal(knoutCarga) {
    if (knoutCarga.ExigeNotaFiscalParaCalcularFrete.val())
        EtapaFreteTMSProblema(knoutCarga);
    else
        EtapaFreteEmbarcadorProblema(knoutCarga);
}

function InformarEstadosDasEtapasAgTransportador(knoutCarga, carga, idDivCarga) {
    if (knoutCarga.Empresa.codEntity() > 0) {
        EtapaDadosTransportadorAprovada(knoutCarga);
    } else
        EtapaDadosTransportadorLiberada(knoutCarga);

    if (carga.PossuiPendencia) {
        EtapaDadosTransportadorProblema(knoutCarga);
    }
    if (!_CONFIGURACAO_TMS.OcultarBuscaRotaNaCarga && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) {
        knoutCarga.AlterarRotaFreteCarga.visible(true);
    }
}

function InformarEstadosDasEtapasAgNFe(knoutCarga, carga, idDivCarga) {
    if (carga.EmissaoLiberada) {
        if (carga.EmissaoDocumentosAutorizada) {
            knoutCarga.AutorizarEmissaoDocumentos.visibleCTeProcessamento(true);
        } else {
            knoutCarga.AutorizarEmissaoDocumentos.visibleCTeProcessamento(false);
        }
        EtapaNotaFiscalLiberada(knoutCarga);
    }
    else
        EtapaNotaFiscalAguardando(knoutCarga);

    if (knoutCarga.AguardarIntegracaoEtapaTransportador.val()) {
        if (knoutCarga.ExigeNotaFiscalParaCalcularFrete.val()) {
            EtapaInicioEmbarcadorProblema(knoutCarga);
            EtapaInicioTMSProblema(knoutCarga);
        }
        else
            EtapaDadosTransportadorProblema(knoutCarga);
    }

    if (knoutCarga.ExigeNotaFiscalParaCalcularFrete.val() && !_CONFIGURACAO_TMS.OcultarBuscaRotaNaCarga && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) {
        knoutCarga.AlterarRotaFreteCarga.visible(true);
    }

    if (knoutCarga.Mercosul.val() === true || knoutCarga.Internacional.val() === true) {
        knoutCarga.EtapaNotaFiscal.text("Factura");
        knoutCarga.EtapaNotaFiscal.tooltip("Factura");
        knoutCarga.EtapaNotaFiscal.tooltipTitle("Factura");

        if (knoutCarga.PossuiFacturaFake.val()) {
            EtapaNotaFiscalComFacturaFake(knoutCarga);
        }
    }

    if ((knoutCarga.CargaPortoPorto.val() || knoutCarga.CargaPortaPortaTimelineHabilitado.val()) && knoutCarga.CargaPortoPortoPendenciaDocumento.val()) {
        EtapaNotaFiscalProblema(knoutCarga);
    }
}

function InformarEstadosDasEtapasPendeciaDocumentos(knoutCarga, carga, idDivCarga) {

    if (carga.PossuiPendencia) {
        if (carga.ProblemaCTE || carga.ProblemaNFS || carga.ProblemaAverbacaoCTe || carga.ProblemaEmissaoNFeRemessa || (carga.ProblemaIntegracaoValePedagio && !carga.LiberadoComProblemaValePedagio) || carga.ProblemaIntegracaoCIOT || (carga.ProblemaIntegracaoPagamentoMotorista && !carga.LiberadoComProblemaPagamentoMotorista) || carga.PendenciaTransportadorContribuinte || carga.RealizandoOperacaoContainer) {

            if (carga.PossuiOperacaoContainer && carga.RealizandoOperacaoContainer) {

                if (carga.ProblemaIntegracaoValePedagio || carga.ProblemaAverbacaoCTe) {
                    EtapaContainerProblema(knoutCarga);
                }
                else if (carga.RealizandoOperacaoContainer) {
                    EtapaContainerAguardando(knoutCarga);
                }
            }
            else if (!carga.PossuiCTeSubcontratacaoFilialEmissora || carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora) {

                EtapaCTeNFsProblema(knoutCarga);
                EtapaMDFeDesabilitada(knoutCarga);
            }
            else
                EtapaCTeFilialEmissoraProblema(knoutCarga);



            if (carga.ProblemaCTE)
                RecarregarGridProblemaCTeSignalR(knoutCarga);
            if (carga.ProblemaAverbacaoCTe)
                RecarregarGridProblemaAverbacaoCTeSignalR(knoutCarga);
            if (carga.ProblemaIntegracaoCIOT)
                RecarregarGridProblemaCIOTSignalR(knoutCarga);
            if (carga.ProblemaNFS)
                RecarregarGridProblemaNFSeSignalR(knoutCarga);
            if (carga.ProblemaIntegracaoValePedagio)
                RecarregarGridProblemaIntegracaoValePedagioSignalR(knoutCarga);
            if (carga.PendenciaTransportadorContribuinte)
                RecarregarGridPendenciaContribuinteCTeSignalR(knoutCarga);
        } else {
            if (carga.PossuiOperacaoContainer && carga.RealizandoOperacaoContainer) {
                EtapaContainerAguardando(knoutCarga);
            } else if (carga.ProblemaMDFe) {
                RecarregarMDFeRejeicaoSignalR(knoutCarga);
            } else if (carga.ProblemaAverbacaoMDFe) {
                RecarregarGridProblemaAverbacaoMDFeSignalR(knoutCarga);
            } else {
                if (!carga.PossuiCTeSubcontratacaoFilialEmissora || carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                    EtapaMDFeAguardando(knoutCarga);
                else
                    EtapaCTeFilialEmissoraAguardando(knoutCarga);
            }

            RecarregarGridsAutorizadosCTeNFSeSignalR(knoutCarga);
        }
    } else if (carga.AguardandoCTEs) {

        if (carga.PossuiOperacaoContainer && carga.RealizandoOperacaoContainer) {
            EtapaContainerAguardando(knoutCarga);
        } else if (!carga.PossuiCTeSubcontratacaoFilialEmissora || carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora) {
            if (!carga.AgGeracaoCTesAnteriorFilialEmissora && !carga.EmEmissaoCTeSubContratacaoFilialEmissora && !carga.DadosMercosul.EmitindoCRT && !carga.DadosMercosul.Mercosul)
                EtapaCTeNFsAguardando(knoutCarga);
            else {
                if (carga.AgGeracaoCTesAnteriorFilialEmissora)
                    EtapaIntegracaoFilialEmissoraAguardando(knoutCarga);
                else
                    EtapaCTeFilialEmissoraAguardando(knoutCarga);

                ExibirInformacaoEmissaoCTesSubContratacaoFilialEmissora(knoutCarga);
            }
        }
        else {
            if (carga.AgGeracaoCTesAnteriorFilialEmissora)
                EtapaIntegracaoFilialEmissoraAguardando(knoutCarga);
            else
                EtapaCTeFilialEmissoraAguardando(knoutCarga);
            if (carga.EmiteMDFeFilialEmissora)
                RecarregarMDFeAutorizadoSignalR(knoutCarga);
            else
                RecarregarGridsAutorizadosCTeNFSeSignalR(knoutCarga);
        }
    } else if (carga.PossuiOperacaoContainer && carga.RealizandoOperacaoContainer) {
        EtapaContainerAguardando(knoutCarga);
    } else {
        EtapaMDFeAguardando(knoutCarga);
        RecarregarGridsAutorizadosCTeNFSeSignalR(knoutCarga);
    }
}

function InformarEstadosDasEtapasAgImpressaoDocumentos(knoutCarga, carga, idDivCarga) {
    RecarregarGridsViaSignalR(knoutCarga);
    EtapaIntegracaoAprovada(knoutCarga);
    EtapaImpressaoLiberada(knoutCarga);
}

function InformarEstadosDasEtapasAgIntegracao(knoutCarga, carga, idDivCarga) {
    EtapaMDFeEdicaoDesabilitada(knoutCarga);
    RecarregarGridsViaSignalR(knoutCarga);

    if (carga.PossuiPendencia) {
        if (!carga.PossuiCTeSubcontratacaoFilialEmissora || carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora || carga.SituacaoAutorizacaoIntegracaoCTe == EnumSituacaoAutorizacaoIntegracaoCTe.Reprovada) {
            EtapaIntegracaoProblema(knoutCarga);
        } else {
            EtapaIntegracaoFilialEmissoraProblema(knoutCarga);
        }
    } else {
        if (!carga.PossuiCTeSubcontratacaoFilialEmissora || carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora) {
            EtapaIntegracaoAguardando(knoutCarga);
        } else {
            EtapaIntegracaoFilialEmissoraAguardando(knoutCarga);
        }
    }
}

function InformarEstadosDasEtapasTransporte(knoutCarga, carga, idDivCarga) {
    RecarregarGridsViaSignalR(knoutCarga);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        EtapaIntegracaoAprovada(knoutCarga);
    } else {
        EtapaImpressaoAprovada(knoutCarga);
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador)
            $("#" + knoutCarga.EtapaImpressao.idGrid + " .DivImpressao").html("<p>" + Localization.Resources.Cargas.Carga.OsDocumentosJaForamImpressos + "</p>");
        else
            $("#" + knoutCarga.EtapaImpressao.idGrid + " .DivImpressao").html("");
    }

    EtapaMDFeEdicaoDesabilitada(knoutCarga);
    InformarEtapaTransbordo(knoutCarga, carga, idDivCarga);

    if (_CONFIGURACAO_TMS.PermiteAlterarRotaEmCargaFinalizada && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe)
        knoutCarga.AlterarRotaFreteCarga.visible(true);
}

function isPossuiMensagemAlertaComBloqueio(mensagensAlerta) {
    for (var i = 0; i < mensagensAlerta.length; i++) {
        if (mensagensAlerta[i].Bloquear)
            return true;
    }

    return false;
}

function RecarregarGridsViaSignalR(knoutCarga) {
    RecarregarGridsAutorizadosCTeNFSeSignalR(knoutCarga);
    RecarregarMDFeAutorizadoSignalR(knoutCarga);
    RecarregarIntegracaoCTeViaSignalR(knoutCarga);
    RecarregarIntegracaoEDIViaSignalR(knoutCarga);
    RecarregarIntegracaoCargaViaSignalR(knoutCarga);
    recarregarIntegracaoCargaTransportadorViaSignalR(knoutCarga);
    RecarregarIntegracaoCargaDadosTransporteViaSignalR(knoutCarga);
}

function atualizarDadosCarga(knoutCarga, carga) {
    IniciarBindKnoutCarga(knoutCarga, carga);
    InformarEstadosDasEtapas(knoutCarga, carga, knoutCarga.DivCarga.id);

    if (knoutCarga.ExigeNotaFiscalParaCalcularFrete.val())
        atualizarGridDadosMotorista(knoutCarga, carga);
    else {
        if ((carga.Motoristas != null) && (carga.Motoristas.length > 0)) {
            knoutCarga.Motorista.val(carga.Motoristas[0].Descricao);
            knoutCarga.Motorista.codEntity(carga.Motoristas[0].Codigo);
            knoutCarga.Motorista.motoristas(carga.Motoristas[0].Descricao);
        }
    }

    atualizarGridDadosAjudante(knoutCarga, carga);

    knoutCarga.ApoliceSeguro.visible(knoutCarga.RejeitadaPeloTransportador.val() && _CONFIGURACAO_TMS.InformaApoliceSeguroMontagemCarga);

    if (_gridCTesParaEmissao != null && _gridCTesParaEmissao != undefined)
        _gridCTesParaEmissao.CarregarGrid();

}

function preencherDadosCarga(knoutCarga, carga, localize) {
    var idDivCarga = knoutCarga.DivCarga.id;
    IniciarBindKnoutCarga(knoutCarga, carga);

    let html = _HTMLCarga.replace(/#idDivCarga/g, idDivCarga)
        .replace(/#tab1TMS/g, "#" + knoutCarga.EtapaInicioTMS.idGrid)
        .replace(/#tab12/g, "#" + knoutCarga.EtapaCTeNFs.idGrid)
        .replace(/#tab1/g, "#" + knoutCarga.EtapaInicioEmbarcador.idGrid)
        .replace(/#tab2/g, "#" + knoutCarga.EtapaFreteEmbarcador.idGrid)
        .replace(/#tab4/g, "#" + knoutCarga.EtapaDadosTransportador.idGrid)
        .replace(/#tab5/g, "#" + knoutCarga.EtapaNotaFiscal.idGrid)
        .replace(/#tab6/g, "#" + knoutCarga.EtapaCTeNFs.idGrid)
        .replace(/#tab7/g, "#" + knoutCarga.EtapaMDFe.idGrid)
        .replace(/#tab8/g, "#" + knoutCarga.EtapaImpressao.idGrid)
        .replace(/#tab9/g, "#" + knoutCarga.EtapaIntegracao.idGrid)
        .replace(/#tabContainer/g, "#" + knoutCarga.EtapaContainer.idGrid)
        .replace(/#tabMercante/g, "#" + knoutCarga.EtapaMercante.idGrid)
        .replace(/#tabDocumentosMercosul/g, "#" + knoutCarga.EtapaDocumentosMercosul.idGrid)
        .replace(/#tabSubContratacao/g, "#" + knoutCarga.EtapaSubContratacao.idGrid)
        .replace(/#tabTransbordo/g, "#" + knoutCarga.EtapaTransbordo.idGrid)
        .replace(/#tabMDFeAquaviario/g, "#" + knoutCarga.EtapaMDFeAquaviario.idGrid)
        .replace(/#tabSVM/g, "#" + knoutCarga.EtapaSVM.idGrid)
        .replace(/#tabFaturamento/g, "#" + knoutCarga.EtapaFaturamento.idGrid)
        .replace(/#tabIntegracaoFaturamento/g, "#" + knoutCarga.EtapaIntegracaoFaturamento.idGrid)
        .replace(/#ribbonCargaNova/g, idDivCarga + "_ribbonCargaNova")
        .replace(/#ribbonCargaEmCancelamentoRibbon/g, idDivCarga + "_ribbonCargaEmCancelamentoRibbon")
        .replace(/#ribbonCargaCancelamentoRejeitadoRibbon/g, idDivCarga + "_ribbonCargaCancelamentoRejeitadoRibbon")
        .replace(/#ribbonCargaCancelada/g, idDivCarga + "_ribbonCargaCancelada")
        .replace(/#ribbonCargaSubcontratada/g, idDivCarga + "_ribbonCargaSubcontratada")
        .replace(/#ribbonSubcontratacao/g, idDivCarga + "_ribbonSubcontratacao")
        .replace(/#ribbonRefaturamento/g, idDivCarga + "_ribbonRefaturamento")
        .replace(/#ribbonRedespacho/g, idDivCarga + "_ribbonRedespacho")
        .replace(/#ribbonSVM/g, idDivCarga + "_ribbonSVM")
        .replace(/#ribbonSVMTerceiro/g, idDivCarga + "_ribbonSVMTerceiro")
        .replace(/#ribbonTakeOrPay/g, idDivCarga + "_ribbonTakeOrPay")
        .replace(/#ribbonDemurrage/g, idDivCarga + "_ribbonDemurrage")
        .replace(/#ribbonDetention/g, idDivCarga + "_ribbonDetention")
        .replace(/#ribbonColeta/g, idDivCarga + "_ribbonColeta")
        .replace(/#ribbonTransbordo/g, idDivCarga + "_ribbonTransbordo")
        .replace(/#ribbonAnulacao/g, idDivCarga + "_ribbonAnulacao")
        .replace(/#ribbonDesistencia/g, idDivCarga + "_ribbonDesistencia")
        .replace(/#ribbonTrocaNota/g, idDivCarga + "_ribbonTrocaNota")
        .replace(/#ribbonPreCarga/g, idDivCarga + "_ribbonPreCarga")
        .replace(/#ribbonReentrega/g, idDivCarga + "_ribbonReentrega")
        .replace(/#ribbonNoShow/g, idDivCarga + "_ribbonNoShow")
        .replace(/#ribbonComplementar/g, idDivCarga + "_ribbonComplementar")
        .replace(/#ribbonVinculadoMultimodalTerceiro/g, idDivCarga + "_ribbonVinculadoMultimodalTerceiro")
        .replace(/#ribbonCargaSubcontratadaMultiModalTMS/g, idDivCarga + "_ribbonCargaSubcontratadaMultiModalTMS")
        .replace(/#DadosEmissaoFrete/g, knoutCarga.DadosEmissaoFrete.id)
        .replace(/#dados_documentos/g, knoutCarga.EtapaNotaFiscal.idGrid)
        .replace(/#dados_transportador/g, knoutCarga.EtapaDadosTransportador.idGrid);

    $("#conteudo_" + idDivCarga).html(html);

    if (localize !== false)
        LocalizeCurrentPage();

    KoBindings(knoutCarga, idDivCarga);

    if (knoutCarga.ExigeNotaFiscalParaCalcularFrete.val()) {
        $("#" + knoutCarga.EtapaInicioEmbarcador.idGrid).html("");
    } else {
        $("#" + knoutCarga.EtapaInicioTMS.idGrid).html("");
    }

    $("#" + knoutCarga.ControleVeiculosCheckList.id).prop("disabled", false);

    EtapaFreteEmbarcadorDesabilitada(knoutCarga);
    EtapaDadosTransportadorDesabilitada(knoutCarga);
    EtapaNotaFiscalDesabilitada(knoutCarga);
    EtapaCTeNFsDesabilitada(knoutCarga);
    EtapaCTeFilialEmissoraDesabilitada(knoutCarga);
    EtapaMDFeDesabilitada(knoutCarga);
    EtapaImpressaoDesabilitada(knoutCarga);
    EtapaIntegracaoDesabilitada(knoutCarga);
    EtapaIntegracaoFilialEmissoraDesabilitada(knoutCarga);
    EtapaNotaFiscalMercosulDesabilitada(knoutCarga);
    EtapaMercanteDesabilitada(knoutCarga);
    EtapaMDFeAquaviarioDesabilitada(knoutCarga);
    EtapaSVMDesabilitada(knoutCarga);
    EtapaFaturamentoDesabilitada(knoutCarga);
    EtapaIntegracaoFaturamentoDesabilitada(knoutCarga);
    EtapaContainerDesabilitada(knoutCarga);

    InformarEstadosDasEtapas(knoutCarga, carga, knoutCarga.DivCarga.id);

    let quantidadeBolas = 8;

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
        quantidadeBolas = 6;

    knoutCarga.EtapaMDFe.visible(true);
    if (knoutCarga.MDFeAquaviario.val() || knoutCarga.OcultarMDFeRodoviario.val()) {
        quantidadeBolas--;
        knoutCarga.EtapaMDFe.visible(false);
    }

    if (carga.PossuiCTeSubcontratacaoFilialEmissora) {
        if (!carga.UtilizarCTesAnterioresComoCTeFilialEmissora) {
            knoutCarga.EtapaCTeFilialEmissora.visible(true);
            quantidadeBolas++;
        }
        knoutCarga.EtapaIntegracaoFilialEmissora.visible(true);
        quantidadeBolas++;
    }

    if (knoutCarga.ModificarTimelineIntegracaoCarga.val())
        knoutCarga.EtapaIntegracao.text(Localization.Resources.Cargas.Carga.IntegracaoCTe);

    if (knoutCarga.Mercante.val() && !knoutCarga.HabilitarTimelineCargaFeeder.val()) {
        quantidadeBolas++;
        knoutCarga.EtapaMercante.visible(true);
        knoutCarga.EtapaMercante.step(quantidadeBolas);

        if (knoutCarga.TodosCTesComMercante.val() || knoutCarga.TodosCTesComManifesto.val())
            EtapaMercanteAprovada(knoutCarga);
        else
            EtapaMercanteLiberada(knoutCarga);
    } else {
        $("#" + knoutCarga.EtapaMercante.idTab).closest("li").remove();
    }

    if (knoutCarga.MDFeAquaviario.val()) {
        quantidadeBolas++;
        knoutCarga.EtapaMDFeAquaviario.visible(true);
        knoutCarga.EtapaMDFeAquaviario.step(quantidadeBolas);

        if (knoutCarga.PossuiMDFeAquaviarioRejeitado.val()) {
            EtapaMDFeAquaviarioProblema(knoutCarga);
        }
        else if (knoutCarga.PossuiMDFeAquaviarioGeradoMasNaoVinculado.val())
            EtapaMDFeAquaviarioNaoVinculado(knoutCarga);
        else if (knoutCarga.PossuiMDFeAquaviarioPendente.val()) {
            EtapaMDFeAquaviarioAguardando(knoutCarga);
        }
        else if (knoutCarga.PossuiMDFeAquaviarioAutorizado.val())
            EtapaMDFeAquaviarioAprovada(knoutCarga);
        else
            EtapaMDFeAquaviarioAguardando(knoutCarga);

    } else {
        $("#" + knoutCarga.EtapaMDFeAquaviario.idTab).closest("li").remove();
    }

    if (knoutCarga.CargaPortaPortaTimelineHabilitado.val()) {
        quantidadeBolas++;
        knoutCarga.EtapaSVM.visible(true);
        knoutCarga.EtapaSVM.step(quantidadeBolas);

        if (knoutCarga.PossuiSVMPendenteAutorizacao.val()) {
            EtapaSVMProblema(knoutCarga);
        }
        else if (knoutCarga.PossuiCTePendenteSVM.val()) {
            EtapaSVMAguardando(knoutCarga);
        }
        else if (!knoutCarga.PossuiSVMPendenteAutorizacao.val() && !knoutCarga.PossuiCTePendenteSVM.val())
            EtapaSVMAprovada(knoutCarga);
        else
            EtapaSVMAguardando(knoutCarga);
    } else {
        $("#" + knoutCarga.EtapaSVM.idTab).closest("li").remove();
    }

    if (knoutCarga.Faturamento.val()) {
        quantidadeBolas++;
        knoutCarga.EtapaFaturamento.visible(true);
        knoutCarga.EtapaFaturamento.step(quantidadeBolas);

        if (!carga.ContemFaturamento) {
            EtapaFaturamentoDesabilitada(knoutCarga);
            EtapaIntegracaoFaturamentoDesabilitada(knoutCarga);
        }
        else {
            if (carga.TodosCTesForamFaturados) {
                EtapaFaturamentoAprovada(knoutCarga);
                EtapaIntegracaoFaturamentoAprovada(knoutCarga);
            }
            else {
                EtapaIntegracaoFaturamentoLiberada(knoutCarga);
                EtapaFaturamentoLiberada(knoutCarga);
            }
        }
    } else {
        $("#" + knoutCarga.EtapaFaturamento.idTab).closest("li").remove();
    }

    if (knoutCarga.IntegracaoFaturamento.val()) {
        quantidadeBolas++;
        knoutCarga.EtapaIntegracaoFaturamento.visible(true);
        knoutCarga.EtapaIntegracaoFaturamento.step(quantidadeBolas);
        //if ()
        //    EtapaFaturamentoAprovada(knoutCarga);
        //else
        //EtapaIntegracaoFaturamentoLiberada(knoutCarga);
    } else {
        $("#" + knoutCarga.EtapaIntegracaoFaturamento.idTab).closest("li").remove();
    }

    knoutCarga.PermiteAdicionarAnexosGuarita.val(carga.PermiteAdicionarAnexosGuarita);
    knoutCarga.AdicionarAnexos.visible(carga.PermiteAdicionarAnexosGuarita);

    let numeroEtapa = 2;

    if (knoutCarga.ExigeNotaFiscalParaCalcularFrete.val()) {
        knoutCarga.EtapaInicioTMS.visible(true);
        knoutCarga.EtapaFreteTMS.visible(true);
        knoutCarga.EtapaIntegracao.visible(true);
        knoutCarga.EtapaInicioEmbarcador.visible(false);
        knoutCarga.EtapaFreteEmbarcador.visible(false);
        knoutCarga.EtapaDadosTransportador.visible(false);
        knoutCarga.EtapaImpressao.visible(false);

        if (carga.PossuiOperacaoContainer) {
            knoutCarga.RetornarEtapaNFe.visible(false);
            knoutCarga.PossuiOperacaoContainer.val(carga.PossuiOperacaoContainer);
            knoutCarga.EtapaContainer.visible(true);
            knoutCarga.EtapaContainer.step(numeroEtapa);
            quantidadeBolas++;
            numeroEtapa++;
        }

        knoutCarga.EtapaNotaFiscal.step(numeroEtapa);
        numeroEtapa++;

        knoutCarga.EtapaFreteEmbarcador.step(numeroEtapa);
        knoutCarga.EtapaFreteTMS.step(numeroEtapa);
        numeroEtapa++;

        if (knoutCarga.Internacional.val()) {
            knoutCarga.EtapaCTeNFs.text("CRT/MIC");
            knoutCarga.EtapaCTeNFs.tooltip("CRT/MIC");
            knoutCarga.EtapaCTeNFs.tooltipTitle("CRT/MIC");

            knoutCarga.EtapaNotaFiscal.text("Factura");
            knoutCarga.EtapaNotaFiscal.tooltip("Factura");
            knoutCarga.EtapaNotaFiscal.tooltipTitle("Factura");

            if (knoutCarga.PossuiFacturaFake.val()) {
                EtapaNotaFiscalComFacturaFake(knoutCarga);
            }
        } else if (knoutCarga.Mercosul.val()) {
            knoutCarga.EtapaCTeFilialEmissora.visible(true);
            knoutCarga.EtapaCTeFilialEmissora.text("CRT/MIC");
            knoutCarga.EtapaCTeFilialEmissora.tooltip("CRT/MIC");
            knoutCarga.EtapaCTeFilialEmissora.tooltipTitle("CRT/MIC");
            knoutCarga.EtapaCTeFilialEmissora.step(numeroEtapa);
            quantidadeBolas++;
            numeroEtapa++;

            knoutCarga.EtapaDocumentosMercosul.visible(true);
            knoutCarga.EtapaDocumentosMercosul.step(numeroEtapa);
            quantidadeBolas++;
            numeroEtapa++;

            knoutCarga.EtapaNotaFiscal.text("Factura");
            knoutCarga.EtapaNotaFiscal.tooltip("Factura");
            knoutCarga.EtapaNotaFiscal.tooltipTitle("Factura");

            if (knoutCarga.PossuiFacturaFake.val()) {
                EtapaNotaFiscalComFacturaFake(knoutCarga);
            }
        } else if (carga.PossuiCTeSubcontratacaoFilialEmissora) {
            knoutCarga.EtapaCTeFilialEmissora.step(numeroEtapa);

            if (carga.EmiteMDFeFilialEmissora) {
                if (!carga.UtilizarCTesAnterioresComoCTeFilialEmissora)
                    numeroEtapa++;

                knoutCarga.EtapaMDFeFilialEmissora.visible(true);
                knoutCarga.EtapaMDFe.visible(false);
                knoutCarga.EtapaMDFeFilialEmissora.step(numeroEtapa);
                if (carga.UtilizarCTesAnterioresComoCTeFilialEmissora)
                    knoutCarga.EtapaMDFeFilialEmissora.text(Localization.Resources.Cargas.Carga.MDFeFilialEmissora);
            } else {
                knoutCarga.EtapaMDFe.visible(!knoutCarga.MDFeAquaviario.val());
                knoutCarga.EtapaMDFeFilialEmissora.visible(false);
            }

            numeroEtapa++;

            knoutCarga.EtapaIntegracaoFilialEmissora.step(numeroEtapa);

            numeroEtapa++;
        }

        knoutCarga.EtapaCTeNFs.step(numeroEtapa);
        numeroEtapa++;

        if (!carga.EmiteMDFeFilialEmissora && !knoutCarga.MDFeAquaviario.val() && !knoutCarga.CargaPortaPortaTimelineHabilitado.val()) {
            knoutCarga.EtapaMDFe.step(numeroEtapa);
            numeroEtapa++;
        }

        knoutCarga.EtapaIntegracao.step(numeroEtapa);
        numeroEtapa++;

        $("#" + knoutCarga.EtapaInicioEmbarcador.idTab).closest("li").remove();
        $("#" + knoutCarga.EtapaFreteEmbarcador.idTab).closest("li").remove();
        $("#" + knoutCarga.EtapaDadosTransportador.idTab).closest("li").remove();

        $("#" + knoutCarga.EtapaTransbordo.idTab).closest("li").remove();

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) {
            knoutCarga.EtapaImpressao.step(numeroEtapa);
            quantidadeBolas--;
        }

    }
    else {
        knoutCarga.EtapaFreteTMS.visible(false);
        knoutCarga.EtapaInicioTMS.visible(false);
        knoutCarga.EtapaInicioEmbarcador.visible(true);
        knoutCarga.EtapaFreteEmbarcador.visible(true);
        knoutCarga.EtapaDadosTransportador.visible(true);

        numeroEtapa = 6;
        if (carga.PossuiCTeSubcontratacaoFilialEmissora) {
            knoutCarga.EtapaIntegracaoFilialEmissora.step(numeroEtapa);
            numeroEtapa++;

            if (carga.EmiteMDFeFilialEmissora) {
                knoutCarga.EtapaMDFeFilialEmissora.step(numeroEtapa);
                numeroEtapa++;
                knoutCarga.EtapaCTeNFs.step(numeroEtapa);
                knoutCarga.EtapaMDFeFilialEmissora.visible(true);
                knoutCarga.EtapaMDFe.visible(false);
            } else {
                knoutCarga.EtapaCTeNFs.step(numeroEtapa);
                knoutCarga.EtapaMDFeFilialEmissora.visible(false);
                if (!knoutCarga.MDFeAquaviario.val()) {
                    numeroEtapa++;
                    knoutCarga.EtapaMDFe.step(numeroEtapa);
                    knoutCarga.EtapaMDFe.visible(true);
                }
            }

        }
        numeroEtapa++;
        knoutCarga.EtapaIntegracao.step(numeroEtapa);
        numeroEtapa++;
        knoutCarga.EtapaImpressao.step(numeroEtapa);

        $("#" + knoutCarga.EtapaFreteTMS.idTab).closest("li").remove();
        $("#" + knoutCarga.EtapaInicioTMS.idTab).closest("li").remove();

    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        if (carga.FreteDeTerceiro && _CONFIGURACAO_TMS.TipoContratoFreteTerceiro == EnumTipoContratoFreteTerceiro.PorCarga) {
            knoutCarga.EtapaSubContratacao.visible(true);
            knoutCarga.EtapaSubContratacao.step(numeroEtapa);
            quantidadeBolas++;
            numeroEtapa++;
        } else {
            $("#" + knoutCarga.EtapaSubContratacao.idTab).closest("li").remove();
        }

        knoutCarga.Empresa.text(Localization.Resources.Cargas.Carga.EmpresaFilial.getRequiredFieldDescription());
        knoutCarga.ModeloVeicularCarga.tooltipTitle(Localization.Resources.Cargas.Carga.TipoDeVeiculoSolicitado.getFieldDescription());
        knoutCarga.Empresa.tooltipTitle(Localization.Resources.Cargas.Carga.EmpresaFilial.getFieldDescription());
        knoutCarga.Empresa.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-3");
        knoutCarga.Veiculo.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-6");

        $("#" + knoutCarga.EtapaImpressao.idTab).closest("li").remove();

    }
    else {
        knoutCarga.EtapaIntegracao.visible(true);
        knoutCarga.EtapaImpressao.visible(true);
        $("#" + knoutCarga.EtapaSubContratacao.idTab).closest("li").remove();
        $("#" + knoutCarga.EtapaTransbordo.idTab).closest("li").remove();
    }

    knoutCarga.EtapaNotaFiscal.visible(true);
    knoutCarga.EtapaCTeNFs.visible(true);

    var resultado = 100 / quantidadeBolas;
    knoutCarga.TamanhoEtapa.val(resultado.toString().replace(",", ".") + "%");

    // BuscarTransportadores etapa 3
    BuscarTransportadores(knoutCarga.Empresa, function (data) { callbackBuscarTransportadores(knoutCarga, data); }, null, true);

    BuscarTiposContainer(knoutCarga.TipoContainer);    

    knoutCarga.Container.visible(knoutCarga.NaoExigeVeiculoParaEmissao.val());
    knoutCarga.Container.required = false;

    if (knoutCarga.NaoExigeVeiculoParaEmissao.val()) {
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
            knoutCarga.SalvarDadosTransporte.visible(false);
            knoutCarga.SalvarDadosTransporteESolicitarNFes.visible(false);
            knoutCarga.ImprimirDadosTransporte.visible(false);
            knoutCarga.ImprimirFichaMotorista.visible(false);
            knoutCarga.ImprimirOrdemColeta.visible(false);
        } if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
            knoutCarga.ImprimirDadosTransporte.visible(false);
        }

        ocultarExibicaoCamposVeiculo(knoutCarga);

        //$("#" + knoutCarga.ExtrasDadosCarga.id).html($("#" + knoutCarga.TipoOperacao.idExtra).html());
        //$("#" + knoutCarga.TipoOperacao.idExtra).html("");
        knoutCarga.ExtrasDadosCarga.visible(true);

        knoutCarga.Motorista.visible(false);
        knoutCarga.Motorista.required = false;

        //knoutCarga.ModeloVeicularCarga.visible(false);
        knoutCarga.ModeloVeicularCarga.text(Localization.Resources.Cargas.Carga.ModeloVeicular.getFieldDescription());
        knoutCarga.ModeloVeicularCarga.required = false;

        knoutCarga.AdicionarMotoristas.visible(false);
    }
    else
        controlarExibicaoCamposVeiculoCarga(knoutCarga);

    if (knoutCarga.ExigeNotaFiscalParaCalcularFrete.val()) {
        loadMotoristas(knoutCarga, carga);
        preecherGridDadosAjudante(knoutCarga, carga);

        knoutCarga.AutorizarEmissaoDocumentos.visible(true);
        knoutCarga.PercursoCargaMDFe.visibleTMS(true);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS || _CONFIGURACAO_TMS.PermitirTransportadorRetornarEtapaNFe || _CONFIGURACAO_TMS.PermiteAdicionarNotaManualmente || knoutCarga.PermiteImportarDocumentosManualmente.val())
            knoutCarga.RetornarParaEtapaNFeTMS.visible(true);
        else
            knoutCarga.RetornarParaEtapaNFeTMS.visible(false);

        knoutCarga.SalvarDadosTransporteESolicitarNFes.visible(false);
        if (carga.ProblemaIntegracaoGrMotoristaVeiculo) {
            knoutCarga.LiberarComProblemaIntegracaoGrMotoristaVeiculo.visible(carga.PermitirLiberarComProblemaIntegracaoGrMotoristaVeiculo);
        } else if (carga.ProblemaIntegracaoGrMotoristaVeiculoDetalhes) {
            knoutCarga.DetalhesLiberarComProblemaIntegracaoGrMotoristaVeiculo.visible(carga.PermitirLiberarComProblemaIntegracaoGrMotoristaVeiculo);
        }
        else
            knoutCarga.SalvarDadosTransporteESolicitarNFes.visible(_CONFIGURACAO_TMS.PermitirSalvarDadosTransporteCargaSemSolicitarNFes && carga.SituacaoCarga == EnumSituacoesCarga.Nova);

        if (!string.IsNullOrWhiteSpace(carga.MensagemProblemaIntegracaoGrMotoristaVeiculo)) {
            $("#" + knoutCarga.LiberarComProblemaIntegracaoGrMotoristaVeiculo.idText).text(carga.MensagemProblemaIntegracaoGrMotoristaVeiculo);
            $("#" + knoutCarga.LiberarComProblemaIntegracaoGrMotoristaVeiculo.idContainer).show();
        }
        else
            $("#" + knoutCarga.LiberarComProblemaIntegracaoGrMotoristaVeiculo.idContainer).hide();
    }
    else {
        loadMotoristas(knoutCarga, carga);

        if (knoutCarga.ValidarLicencaMotorista.val())
            BuscarMotoristas(knoutCarga.Motorista, function (m) { AdicionarMotoristaCargaClick(m, knoutCarga, EnumSituacoesCarga.AgTransportador); }, knoutCarga.Empresa, null, true, EnumSituacaoColaborador.Trabalhando);
        else
            BuscarMotoristas(knoutCarga.Motorista, null, knoutCarga.Empresa, null, true, EnumSituacaoColaborador.Trabalhando);

        if ((carga.Motoristas != null) && (carga.Motoristas.length > 0)) {
            knoutCarga.Motorista.val(carga.Motoristas[0].Descricao);
            knoutCarga.Motorista.codEntity(carga.Motoristas[0].Codigo);
            knoutCarga.Motorista.motoristas(carga.Motoristas[0].Descricao);
        }

        if (carga.TipoOperacao.ExigeConformacaoFreteAntesEmissao && _CONFIGURACAO_TMS.ExigirConfirmacaoEtapaFreteNoFluxoNotaAposFrete) {
            knoutCarga.AutorizarEmissaoDocumentos.visible(true);
            knoutCarga.AutorizarEmissaoDocumentos.text(Localization.Resources.Cargas.Carga.IrParaEtapaDeTransportador);
        } else
            knoutCarga.AutorizarEmissaoDocumentos.visible(false);

        knoutCarga.LiberarComProblemaIntegracaoGrMotoristaVeiculo.visible(false);
        knoutCarga.DetalhesLiberarComProblemaIntegracaoGrMotoristaVeiculo.visible(false);
        knoutCarga.SalvarDadosTransporteESolicitarNFes.visible(false);

        if (knoutCarga.PermiteInformarTransportadorEtapaUmQuandoNotaFiscalNaoRecebidaAntesCalculoFrete.val()) {
            knoutCarga.EmpresaEtapaUm.visible(true);
            knoutCarga.Empresa.text(Localization.Resources.Cargas.Carga.Transportador.getFieldDescription());
            knoutCarga.Veiculo.text(Localization.Resources.Cargas.Carga.Veiculo.getFieldDescription());
            knoutCarga.Reboque.text(Localization.Resources.Cargas.Carga.VeiculoCarreta.getFieldDescription());

            // BuscarTransportadores etapa 1
            BuscarTransportadores(knoutCarga.Empresa, function (data) { callbackBuscarTransportadores(knoutCarga, data); }, null, true);
        }
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS && (knoutCarga.CargaPortoPortoTimelineHabilitado.val() || knoutCarga.CargaPortaPortaTimelineHabilitado.val() || knoutCarga.CargaSVMProprioTimelineHabilitado.val()
        || knoutCarga.HabilitarTimelineCargaFeeder.val()))
        knoutCarga.EtapaInicioTMS.text(Localization.Resources.Cargas.Carga.Reserva);

    knoutCarga.ApoliceSeguro.visible(knoutCarga.RejeitadaPeloTransportador.val() && _CONFIGURACAO_TMS.InformaApoliceSeguroMontagemCarga);
    controlarExibicaoDatasCarregamento(knoutCarga);

    var codigoCarga = 0;
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        codigoCarga = carga.Codigo;
        BuscarModelosVeicularesCarga(knoutCarga.ModeloVeicularCarga, function (data) { callbackBuscarModeloVeicular(knoutCarga, data) }, null, null, [EnumTipoModeloVeicularCarga.Geral, EnumTipoModeloVeicularCarga.Reboque, EnumTipoModeloVeicularCarga.Tracao], null, null, null, null, codigoCarga);
        knoutCarga.Frota.visible(true);
    }
    else
        BuscarModelosVeicularesCarga(knoutCarga.ModeloVeicularCarga, function (data) { callbackBuscarModeloVeicular(knoutCarga, data) }, null, knoutCarga.TipoCarga);

    BuscarTiposdeCarga(knoutCarga.TipoCarga, null, null, null, null, codigoCarga, null, null, null, knoutCarga.TipoOperacao);
    BuscarTiposOperacao(knoutCarga.TipoOperacao, function (data) { callbackBuscarTipoOperacao(knoutCarga, data); }, null, null, null, codigoCarga, null, null, null, null, null, null, knoutCarga.TipoCarga);
    BuscarPedidoViagemNavio(knoutCarga.PedidoViagemNavio);
    BuscarPorto(knoutCarga.PortoOrigem);
    BuscarPorto(knoutCarga.PortoDestino);
    BuscarSetorFuncionario(knoutCarga.Setor);
    BuscarTipoTerminalImportacao(knoutCarga.TerminalOrigem);
    BuscarTipoTerminalImportacao(knoutCarga.TerminalDestino);
    BuscarApolicesSeguro(knoutCarga.ApoliceSeguro, null, null, null, null, true, knoutCarga.Empresa, true);
    BuscarApolicesSeguro(knoutCarga.ApoliceSeguroTransportador, null, null, null, null, true, knoutCarga.Empresa, true);
    BuscarTipoCarregamento(knoutCarga.TipoCarregamento);
    BuscarCentroResultado(knoutCarga.CentroResultado);
    BuscarContainers(knoutCarga.Container, null, null, true);

    configurarCampoJustificativaCustoExtra(knoutCarga, carga.TipoOperacao.ObrigatorioJustificarCustoExtra);
    configurarLiberarCargaSemPlanejamento(knoutCarga, carga.TipoOperacao.LiberarCargaSemPlanejamento);

    if (knoutCarga.TipoOperacao.TipoCobrancaMultimodal == EnumTipoCobrancaMultimodal.CTEAquaviario && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) {
        BuscarNavios(knoutCarga.Navio, null, null, EnumTipoEmbarcacao.Rebocador);
        BuscarNavios(knoutCarga.Balsa, null, null, EnumTipoEmbarcacao.Balsa);

        knoutCarga.Navio.visible(true);
        knoutCarga.Navio.required(true);
        knoutCarga.Balsa.visible(true);
        knoutCarga.Balsa.required(true);
    }

    if (knoutCarga.TipoOperacao.InformarTransportadorSubcontratadoEtapaUm) {
        BuscarTransportadores(knoutCarga.TransportadorSubcontratado);

        knoutCarga.TransportadorSubcontratado.visible(true);
        knoutCarga.TransportadorSubcontratado.required(true);
    }

    if (carga.Validacoes.InformarDocaNaEtapaUmDaCarga) {
        BuscarAreaVeiculoPosicao(knoutCarga.LocalCarregamento, null, null, null, null, EnumTipoAreaVeiculo.Doca);

        knoutCarga.LocalCarregamento.visible(true);
        knoutCarga.LocalCarregamento.required(true);
    }

    var filtrarVeiculosSomenteTracao = null;
    if (knoutCarga.ExigeConfirmacaoTracao.val())
        filtrarVeiculosSomenteTracao = "0";

    var tipoPropriedade = "";

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        //Caso seja TMS, vai definir valor padrão como N, desta forma, não trará nenhum veículo se o usuário não tiver nenhuma das permissões
        tipoPropriedade = "N";
        if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermiteInserirVeiculoProprio, _PermissoesPersonalizadasCarga))
            tipoPropriedade = "P";
        if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermiteInserirVeiculoTerceiro, _PermissoesPersonalizadasCarga))
            tipoPropriedade = tipoPropriedade == "N" ? "T" : "A";
    }

    if (_CONFIGURACAO_TMS.FiltrarBuscaVeiculosPorEmpresa) {
        new BuscarVeiculos(knoutCarga.Veiculo, function (data) {
            callbackBuscaVeiculo(knoutCarga, data);
        }, knoutCarga.Empresa, knoutCarga.EstaEmParqueamento.val() ? null : knoutCarga.ModeloVeicularCarga, null, true, null, knoutCarga.TipoCarga, knoutCarga.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Cliente ? false : true, null, carga.Codigo, filtrarVeiculosSomenteTracao, null, null, null, null, null, null, null, null, null, null, null, null, tipoPropriedade);

        new BuscarVeiculos(knoutCarga.Reboque, ValidaReboqueSelecionados(knoutCarga), knoutCarga.Empresa, knoutCarga.ModeloVeicularCarga, null, true, null, null, knoutCarga.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Cliente ? false : true, null, carga.Codigo, "1");
        new BuscarVeiculos(knoutCarga.SegundoReboque, ValidaSegundoReboqueSelecionados(knoutCarga), knoutCarga.Empresa, knoutCarga.ModeloVeicularCarga, null, true, null, null, knoutCarga.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Cliente ? false : true, null, carga.Codigo, "1");
        new BuscarVeiculos(knoutCarga.TerceiroReboque, ValidaTerceiroReboqueSelecionados(knoutCarga), knoutCarga.Empresa, knoutCarga.ModeloVeicularCarga, null, true, null, null, knoutCarga.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Cliente ? false : true, null, carga.Codigo, "1");

        //new BuscarVeiculos(knoutCarga.Tracao, function (data) { callbackBuscaVeiculoTracao(knoutCarga, data) }, knoutCarga.Empresa, knoutCarga.ModeloVeicularCarga, null, true, null, knoutCarga.TipoCarga, knoutCarga.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Cliente ? false : true, null, carga.Codigo, "0");
    }
    else {
        new BuscarVeiculos(knoutCarga.Veiculo, function (data) {
            callbackBuscaVeiculo(knoutCarga, data);
        }, null, knoutCarga.EstaEmParqueamento.val() ? null : knoutCarga.ModeloVeicularCarga, knoutCarga.Motorista, true, null, knoutCarga.TipoCarga, knoutCarga.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Cliente ? false : true, null, carga.Codigo, filtrarVeiculosSomenteTracao, null, null, null, null, null, null, null, null, null, null, null, null, tipoPropriedade, null, knoutCarga.TipoOperacao);

        new BuscarVeiculos(knoutCarga.Reboque, ValidaReboqueSelecionados(knoutCarga), null, null, null, true, null, null, knoutCarga.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Cliente ? false : true, null, carga.Codigo, "1", null, null, null, null, null, null, null, null, null, null, null, null, null, null, knoutCarga.TipoOperacao);
        new BuscarVeiculos(knoutCarga.SegundoReboque, ValidaSegundoReboqueSelecionados(knoutCarga), null, null, null, true, null, null, knoutCarga.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Cliente ? false : true, null, carga.Codigo, "1", null, null, null, null, null, null, null, null, null, null, null, null, null, null, knoutCarga.TipoOperacao);
        new BuscarVeiculos(knoutCarga.TerceiroReboque, ValidaTerceiroReboqueSelecionados(knoutCarga), null, null, null, true, null, null, knoutCarga.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Cliente ? false : true, null, carga.Codigo, "1", null, null, null, null, null, null, null, null, null, null, null, null, null, null, knoutCarga.TipoOperacao);

        //new BuscarVeiculos(knoutCarga.Tracao, function (data) { callbackBuscaVeiculoTracao(knoutCarga, data) }, null, null, null, true, null, null, knoutCarga.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Cliente ? false : true, null, carga.Codigo, "0");

    }

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        ocultarExibicaoCamposVeiculo(knoutCarga);

        knoutCarga.Frota.visible(false);

        knoutCarga.ModeloVeicularCarga.visible(false);
        knoutCarga.ModeloVeicularCarga.required = false;

        knoutCarga.AdicionarMotoristas.visible(false);
        knoutCarga.Motorista.visible(false);
        knoutCarga.Motorista.required = false;

        knoutCarga.PedidoViagemNavio.visible(true);
        knoutCarga.PedidoViagemNavio.required = false;

        var cargaSVM = knoutCarga.CargaSVM.val() || knoutCarga.CargaSVMTerceiro.val();

        knoutCarga.PortoOrigem.visible(cargaSVM);
        knoutCarga.PortoOrigem.required = cargaSVM;

        knoutCarga.PortoDestino.visible(cargaSVM);
        knoutCarga.PortoDestino.required = cargaSVM;

        knoutCarga.TerminalOrigem.visible(cargaSVM);
        knoutCarga.TerminalOrigem.required = cargaSVM;

        knoutCarga.TerminalDestino.visible(cargaSVM);
        knoutCarga.TerminalDestino.required = cargaSVM;

        if (knoutCarga.SituacaoCarga.val() == EnumSituacoesCarga.CalculoFrete ||
            knoutCarga.SituacaoCarga.val() == EnumSituacoesCarga.PendeciaDocumentos ||
            knoutCarga.SituacaoCarga.val() == EnumSituacoesCarga.AgImpressaoDocumentos ||
            knoutCarga.SituacaoCarga.val() == EnumSituacoesCarga.AgIntegracao ||
            knoutCarga.SituacaoCarga.val() == EnumSituacoesCarga.Encerrada)
            knoutCarga.SituacaoRecebimentoNotas.visible(true);
        else
            knoutCarga.SituacaoRecebimentoNotas.visible(false);
    }

    knoutCarga.CargaTipoConsolidacao.val(carga.CargaTipoConsolidacao);

    if (carga.CargaTipoConsolidacao) {
        knoutCarga.TipoContainer.visible(true);
        knoutCarga.Empresa.visible(false);
        knoutCarga.Veiculo.visible(false);
        knoutCarga.AdicionarMotoristas.visible(false);
        knoutCarga.ModeloVeicularCarga.visible(false);
    }

    if (knoutCarga.CargaPerigosaIntegracao.val() == "Sim") {
        $("#" + knoutCarga.CargaPerigosaIntegracao.id).css({ "color": "#000000", "font-weight": "bold", "font-size": "15px" });
    } else {
        $("#" + knoutCarga.CargaPerigosaIntegracao.id).css({ "color": "", "font-weight": "", "font-size": "" });
    }

    if (knoutCarga.TipoOperacao.NecessitaInformarPlacaCarregamento) {
        loadPlacaCarregamento(knoutCarga);
        knoutCarga.InformarPlacaCarregamento.visible(true);
    }

    if (carga.PossuiIntegracaoHUBOfertas) {
        knoutCarga.ConsultarVeiculosSugeridosHUB.visible(true);
        knoutCarga.SugereIntegracaoHUBSaibaMais.visible(false);
        knoutCarga.FalhaIntegracaoHUB.visible(carga.FalhaIntegracaoHUB);
        knoutCarga.TagEnviadoHUBOfertas.visible(carga.EnviadoAoHUBOferta);
    }
}


function modeloVeicularCargaBlur(knoutCarga) {
    knoutCarga.Reboque.visible(true).required = true;
    knoutCarga.SegundoReboque.visible(true).required = true;
    knoutCarga.TerceiroReboque.visible(true).required = true;

}

function callbackBuscarTransportadores(knoutCarga, arg) {
    if (arg.AptaEmissao || knoutCarga.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Cliente) {
        if (arg.CertificadoVencido || knoutCarga.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Cliente || arg.EmissaoDocumentosForaDoSistema) {//quando o frete é de responsabilidade do cliente ou é emitido por fora do sistema pode informar qualquer transportador do ME
            var codigoEmpresa = knoutCarga.Empresa.codEntity;
            var novoCodigoEmpresa = arg.Codigo;
            knoutCarga.Empresa.val(arg.Descricao);
            knoutCarga.Empresa.codEntity(novoCodigoEmpresa);
            knoutCarga.Empresa.PossuiInformacoesIMO = arg.PossuiInformacoesIMO;
            Global.setarFocoProximoCampo(knoutCarga.Empresa.id);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS && codigoEmpresa != novoCodigoEmpresa) {
                LimparCampoEntity(knoutCarga.Veiculo);
                LimparCampoEntity(knoutCarga.Reboque);
                LimparCampoEntity(knoutCarga.SegundoReboque);
                LimparCampoEntity(knoutCarga.TerceiroReboque);
                knoutCarga.Motorista.motoristas("");
                if (knoutCarga.AdicionarMotoristas?.basicTable != null)
                    knoutCarga.AdicionarMotoristas.basicTable.CarregarGrid(new Array());

                if (knoutCarga.AdicionarAjudantes?.basicTable != null)
                    knoutCarga.AdicionarAjudantes.basicTable.CarregarGrid(new Array());
            }
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.CertificadoParaEmissaoDeDocumentosDoTransportadorEstaVencido, 16000)
        }
    } else {
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.NaoPossivelSelecionarUmTransportadorQueNaoEstaAptoEmitirPeloMultiEmbarcador, 16000)

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.NaoPossivelSelecionarUmaEmpresaFilialQueNaoEstaAptoEmitirPeloMultiTms, 16000)
    }
}

function callbackBuscarTipoOperacao(knoutCarga, data) {
    knoutCarga.TipoOperacao.codEntity(data.Codigo);
    knoutCarga.TipoOperacao.val(data.Descricao);
    knoutCarga.AdicionarAjudantes.visible(data.PermitirInformarAjudantesNaCarga);

    if (data.NecessitaInformarPlacaCarregamento) {
        loadPlacaCarregamento(knoutCarga);
    }
    knoutCarga.InformarPlacaCarregamento.visible(data.NecessitaInformarPlacaCarregamento);

    configurarCampoJustificativaCustoExtra(knoutCarga, data.ObrigatorioJustificarCustoExtra);
    configurarLiberarCargaSemPlanejamento(knoutCarga, data.LiberarCargaSemPlanejamento);
}

function callbackBuscarModeloVeicular(knoutCarga, data) {
    knoutCarga.ModeloVeicularCarga.codEntity(data.Codigo);
    knoutCarga.ModeloVeicularCarga.val(data.Descricao);
    knoutCarga.NumeroReboques.val(data.NumeroReboques);

    controlarExibicaoCamposVeiculoCarga(knoutCarga);
}

function ValidaReboqueSelecionados(ko) {
    return function (data) {
        if (ko.SegundoReboque.codEntity() == data.Codigo || ko.TerceiroReboque.codEntity() == data.Codigo) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.VeiculoCarretaUm, Localization.Resources.Cargas.Carga.NaoPossivelSelecionarDuasCarretasIguais);
            data = {
                Codigo: 0,
                Placa: ''
            };
        }

        ko.Reboque.val(data.Placa);
        ko.Reboque.entityDescription(data.Placa);
        ko.Reboque.codEntity(data.Codigo);
    }
}

function ValidaSegundoReboqueSelecionados(ko) {
    return function (data) {
        if (ko.Reboque.codEntity() == data.Codigo) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.VeiculoCarretaDois, Localization.Resources.Cargas.Carga.NaoPossivelSelecionarDuasCarretasIguais);
            data = {
                Codigo: 0,
                Placa: ''
            };
        }

        ko.SegundoReboque.val(data.Placa);
        ko.SegundoReboque.entityDescription(data.Placa);
        ko.SegundoReboque.codEntity(data.Codigo);
    }
}

function ValidaTerceiroReboqueSelecionados(ko) {
    return function (data) {
        if (ko.Reboque.codEntity() == data.Codigo || ko.SegundoReboque.codEntity() == data.Codigo) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.VeiculoCarretaDois, Localization.Resources.Cargas.Carga.NaoPossivelSelecionarDuasCarretasIguais);
            data = {
                Codigo: 0,
                Placa: ''
            };
        }

        ko.TerceiroReboque.val(data.Placa);
        ko.TerceiroReboque.entityDescription(data.Placa);
        ko.TerceiroReboque.codEntity(data.Codigo);
    }
}

function callbackBuscaVeiculo(knoutCarga, data) {
    executarReST("Veiculo/ValidarRotasFreteVeiculo", { CodigoVeiculo: data.Codigo, CodigoCarga: knoutCarga.Codigo.val() }, function (r) {
        if (r.Success && r.Data) {
            knoutCarga.Veiculo.codEntity(data.Codigo);
            Global.setarFocoProximoCampo(knoutCarga.Veiculo.id);

            let placas = data.ConjuntoPlacasComModeloVeicular;
            if (knoutCarga.ExigeConfirmacaoTracao.val())
                placas = data.Placa;

            knoutCarga.Veiculo.val(placas);
            knoutCarga.Veiculo.entityDescription(placas);

            //Quando a carga está em Parqueamento não altera o reboque
            if (!knoutCarga.EstaEmParqueamento.val()) {
                knoutCarga.Reboque.val(data.VeiculosVinculados);
                knoutCarga.Reboque.entityDescription(data.VeiculosVinculados);
                knoutCarga.Reboque.codEntity(data.CodigosVeiculosVinculados);
            }

            knoutCarga.CodigoMotoristaVeiculo.val(data.CodigoMotorista);
            knoutCarga.NomeMotoristaVeiculo.val(data.Motorista);

            if (_CONFIGURACAO_TMS.PreencherMotoristaAutomaticamenteAoInformarVeiculo) {
                knoutCarga.Motorista.val(data.Motorista);
                knoutCarga.Motorista.codEntity(data.CodigoMotorista);
                if (data.CodigoMotorista > 0) {
                    knoutCarga.Motorista.requiredClass("form-control");
                }
            }

            if (knoutCarga.TipoOperacao.NecessitaInformarPlacaCarregamento) {
                loadPlacaCarregamento(knoutCarga);
                knoutCarga.InformarPlacaCarregamento.visible(true);
            }

            preencherDadosVinculadosVeiculo(knoutCarga, data);
        }
        else
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
    });
}

function validarEntidadesPesquisa() {

    if (_pesquisaCarga.Veiculo.val() == "")
        _pesquisaCarga.Veiculo.codEntity(_pesquisaCarga.Veiculo.defCodEntity);

    if (_pesquisaCarga.Empresa.val() == "")
        _pesquisaCarga.Empresa.codEntity(_pesquisaCarga.Empresa.defCodEntity);

    if (_pesquisaCarga.Operador.val() == "")
        _pesquisaCarga.Operador.codEntity(_pesquisaCarga.Operador.defCodEntity);

    if (_pesquisaCarga.ModeloVeicularCarga.val() == "")
        _pesquisaCarga.ModeloVeicularCarga.codEntity(_pesquisaCarga.ModeloVeicularCarga.defCodEntity);

    if (_pesquisaCarga.PedidoViagemNavio.val() == "")
        _pesquisaCarga.PedidoViagemNavio.codEntity(_pesquisaCarga.PedidoViagemNavio.defCodEntity);

    if (_pesquisaCarga.TipoCarga.val() == "")
        _pesquisaCarga.TipoCarga.codEntity(_pesquisaCarga.TipoCarga.defCodEntity);

    if (_pesquisaCarga.TipoOperacao.val() == "")
        _pesquisaCarga.TipoOperacao.codEntity(_pesquisaCarga.TipoOperacao.defCodEntity);

    if (_pesquisaCarga.Remetente.val() == "")
        _pesquisaCarga.Remetente.codEntity(_pesquisaCarga.Remetente.defCodEntity);

    if (_pesquisaCarga.Destinatario.val() == "")
        _pesquisaCarga.Destinatario.codEntity(_pesquisaCarga.Destinatario.defCodEntity);

    if (_pesquisaCarga.Origem.val() == "")
        _pesquisaCarga.Origem.codEntity(_pesquisaCarga.Origem.defCodEntity);

    if (_pesquisaCarga.Destino.val() == "")
        _pesquisaCarga.Destino.codEntity(_pesquisaCarga.Destino.defCodEntity);

    if (_pesquisaCarga.Motorista.val() == "")
        _pesquisaCarga.Motorista.codEntity(_pesquisaCarga.Motorista.defCodEntity);

    if (_pesquisaCarga.GrupoPessoa.val() == "")
        _pesquisaCarga.GrupoPessoa.codEntity(_pesquisaCarga.GrupoPessoa.defCodEntity);

}

function controlarExibicaoCamposVeiculoCarga(knoutCarga) {
    var reboqueVisivel = false;
    var segundoReboqueVisivel = false;
    var terceiroReboqueVisivel = false;

    if (knoutCarga.ExigeConfirmacaoTracao.val()) {
        reboqueVisivel = (knoutCarga.NumeroReboques.val() >= 1);
        segundoReboqueVisivel = (knoutCarga.NumeroReboques.val() > 1);
        terceiroReboqueVisivel = (knoutCarga.NumeroReboques.val() > 2);
    }

    var exibirSomenteCampoVeiculo = (!reboqueVisivel && !segundoReboqueVisivel && !terceiroReboqueVisivel);
    var tamanhoColuna = 9;

    if (_CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga)
        tamanhoColuna -= 2;

    if (_CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga)
        tamanhoColuna -= 2;

    if (!reboqueVisivel)
        LimparCampoEntity(knoutCarga.Reboque);

    if (!segundoReboqueVisivel)
        LimparCampoEntity(knoutCarga.SegundoReboque);

    if (!terceiroReboqueVisivel)
        LimparCampoEntity(knoutCarga.TerceiroReboque);

    knoutCarga.Veiculo.cssClass("col" + (exibirSomenteCampoVeiculo ? " col-" + tamanhoColuna : " col-9"));
    knoutCarga.Veiculo.text(reboqueVisivel ? Localization.Resources.Cargas.Carga.TracaoCavalo.getRequiredFieldDescription() : Localization.Resources.Cargas.Carga.Veiculo.getRequiredFieldDescription());
    knoutCarga.Veiculo.verDetalhesVisible(!reboqueVisivel && knoutCarga.PermitirInformarAnexoContainerCarga.val());

    if (knoutCarga.EstaEmParqueamento.val()) {
        knoutCarga.Veiculo.enable(true);
    }

    knoutCarga.Reboque.cssClass("col col-" + tamanhoColuna);

    knoutCarga.Reboque.required = reboqueVisivel && !_CONFIGURACAO_TMS.PermitirSalvarApenasTransportadorEtapaUmCarga;
    knoutCarga.Reboque.text(segundoReboqueVisivel ? Localization.Resources.Cargas.Carga.VeiculoCarretaUm.getRequiredFieldDescription() : Localization.Resources.Cargas.Carga.VeiculoCarreta.getRequiredFieldDescription());
    knoutCarga.Reboque.visible(reboqueVisivel);
    knoutCarga.Reboque.verDetalhesVisible(reboqueVisivel && knoutCarga.PermitirInformarAnexoContainerCarga.val());

    knoutCarga.SegundoReboque.cssClass("col col-" + tamanhoColuna);
    knoutCarga.SegundoReboque.required = segundoReboqueVisivel && !_CONFIGURACAO_TMS.PermitirSalvarApenasTransportadorEtapaUmCarga;
    knoutCarga.SegundoReboque.visible(segundoReboqueVisivel);
    knoutCarga.SegundoReboque.verDetalhesVisible(segundoReboqueVisivel && knoutCarga.PermitirInformarAnexoContainerCarga.val());

    knoutCarga.TerceiroReboque.cssClass("col col-" + tamanhoColuna);
    knoutCarga.TerceiroReboque.required = terceiroReboqueVisivel && !_CONFIGURACAO_TMS.PermitirSalvarApenasTransportadorEtapaUmCarga;
    knoutCarga.TerceiroReboque.visible(terceiroReboqueVisivel && knoutCarga.SegundoReboque.val() != null);
    knoutCarga.TerceiroReboque.verDetalhesVisible(terceiroReboqueVisivel && knoutCarga.PermitirInformarAnexoContainerCarga.val());

    knoutCarga.DataRetiradaCtrnVeiculo.visible(_CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga && exibirSomenteCampoVeiculo);
    knoutCarga.DataRetiradaCtrnVeiculo.required(_CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga && exibirSomenteCampoVeiculo && knoutCarga.ExigirDataRetiradaCtrnVeiculos.val());
    knoutCarga.DataRetiradaCtrnVeiculo.text(_CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga && exibirSomenteCampoVeiculo && knoutCarga.ExigirDataRetiradaCtrnVeiculos.val() ? Localization.Resources.Cargas.Carga.DataRetiradaCTRN.getRequiredFieldDescription() : Localization.Resources.Cargas.Carga.DataRetiradaCTRN.getFieldDescription());

    knoutCarga.DataRetiradaCtrnReboque.visible(_CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga && knoutCarga.Reboque.visible());
    knoutCarga.DataRetiradaCtrnReboque.required(_CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga && knoutCarga.Reboque.visible() && knoutCarga.ExigirDataRetiradaCtrnVeiculos.val());
    knoutCarga.DataRetiradaCtrnReboque.text(_CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga && knoutCarga.Reboque.visible() && knoutCarga.ExigirDataRetiradaCtrnVeiculos.val() ? Localization.Resources.Cargas.Carga.DataRetiradaCTRN.getRequiredFieldDescription() : Localization.Resources.Cargas.Carga.DataRetiradaCTRN.getFieldDescription());

    knoutCarga.DataRetiradaCtrnSegundoReboque.visible(_CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga && knoutCarga.SegundoReboque.visible());
    knoutCarga.DataRetiradaCtrnSegundoReboque.required(_CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga && knoutCarga.SegundoReboque.visible() && knoutCarga.ExigirDataRetiradaCtrnVeiculos.val());
    knoutCarga.DataRetiradaCtrnSegundoReboque.text(_CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga && knoutCarga.SegundoReboque.visible() && knoutCarga.ExigirDataRetiradaCtrnVeiculos.val() ? Localization.Resources.Cargas.Carga.DataRetiradaCTRN.getRequiredFieldDescription() : Localization.Resources.Cargas.Carga.DataRetiradaCTRN.getFieldDescription());

    knoutCarga.DataRetiradaCtrnTerceiroReboque.visible(_CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga && knoutCarga.TerceiroReboque.visible());
    knoutCarga.DataRetiradaCtrnTerceiroReboque.required(_CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga && knoutCarga.TerceiroReboque.visible() && knoutCarga.ExigirDataRetiradaCtrnVeiculos.val());
    knoutCarga.DataRetiradaCtrnTerceiroReboque.text(_CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga && knoutCarga.TerceiroReboque.visible() && knoutCarga.ExigirDataRetiradaCtrnVeiculos.val() ? Localization.Resources.Cargas.Carga.DataRetiradaCTRN.getRequiredFieldDescription() : Localization.Resources.Cargas.Carga.DataRetiradaCTRN.getFieldDescription());

    knoutCarga.GensetVeiculo.visible(exibirSomenteCampoVeiculo && knoutCarga.PossuiGenset.val());
    knoutCarga.GensetReboque.visible(knoutCarga.Reboque.visible() && knoutCarga.PossuiGenset.val());
    knoutCarga.GensetSegundoReboque.visible(knoutCarga.SegundoReboque.visible() && knoutCarga.PossuiGenset.val());
    knoutCarga.GensetTerceiroReboque.visible(knoutCarga.TerceiroReboque.visible() && knoutCarga.PossuiGenset.val());

    knoutCarga.MaxGrossVeiculo.visible(_CONFIGURACAO_TMS.PermitirInformarMaxGrossCarga && exibirSomenteCampoVeiculo);
    knoutCarga.MaxGrossReboque.visible(_CONFIGURACAO_TMS.PermitirInformarMaxGrossCarga && knoutCarga.Reboque.visible());
    knoutCarga.MaxGrossSegundoReboque.visible(_CONFIGURACAO_TMS.PermitirInformarMaxGrossCarga && knoutCarga.SegundoReboque.visible());
    knoutCarga.MaxGrossTerceiroReboque.visible(_CONFIGURACAO_TMS.PermitirInformarMaxGrossCarga && knoutCarga.TerceiroReboque.visible());

    knoutCarga.NumeroContainerVeiculo.visible(_CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga);
    knoutCarga.NumeroContainerVeiculo.required(_CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga && exibirSomenteCampoVeiculo && knoutCarga.ExigirNumeroContainerVeiculos.val());
    knoutCarga.NumeroContainerVeiculo.text(_CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga && exibirSomenteCampoVeiculo && knoutCarga.ExigirNumeroContainerVeiculos.val() ? Localization.Resources.Cargas.Carga.NumeroContainerDoVeiculo.getRequiredFieldDescription() : Localization.Resources.Cargas.Carga.NumeroContainerDoVeiculo.getFieldDescription());

    knoutCarga.NumeroContainerReboque.visible(_CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga && knoutCarga.Reboque.visible());
    knoutCarga.NumeroContainerReboque.required(_CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga && knoutCarga.Reboque.visible() && knoutCarga.ExigirNumeroContainerVeiculos.val());
    knoutCarga.NumeroContainerReboque.text(_CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga && knoutCarga.Reboque.visible() && knoutCarga.ExigirNumeroContainerVeiculos.val() ? Localization.Resources.Cargas.Carga.NumeroContainerDoReboque.getRequiredFieldDescription() : Localization.Resources.Cargas.Carga.NumeroContainerDoReboque.getFieldDescription());

    knoutCarga.NumeroContainerSegundoReboque.visible(_CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga && knoutCarga.SegundoReboque.visible());
    knoutCarga.NumeroContainerSegundoReboque.required(_CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga && knoutCarga.SegundoReboque.visible() && knoutCarga.ExigirNumeroContainerVeiculos.val());
    knoutCarga.NumeroContainerSegundoReboque.text(_CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga && knoutCarga.SegundoReboque.visible() && knoutCarga.ExigirNumeroContainerVeiculos.val() ? Localization.Resources.Cargas.Carga.NumeroContainerSegundoReboque.getRequiredFieldDescription() : Localization.Resources.Cargas.Carga.NumeroContainerSegundoReboque.getFieldDescription());

    knoutCarga.NumeroContainerTerceiroReboque.visible(_CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga && knoutCarga.TerceiroReboque.visible());
    knoutCarga.NumeroContainerTerceiroReboque.required(_CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga && knoutCarga.TerceiroReboque.visible() && knoutCarga.ExigirNumeroContainerVeiculos.val());
    knoutCarga.NumeroContainerTerceiroReboque.text(_CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga && knoutCarga.TerceiroReboque.visible() && knoutCarga.ExigirNumeroContainerVeiculos.val() ? Localization.Resources.Cargas.Carga.NumeroContainerTerceiroReboque.getRequiredFieldDescription() : Localization.Resources.Cargas.Carga.NumeroContainerTerceiroReboque.getFieldDescription());

    knoutCarga.TaraContainerVeiculo.visible(_CONFIGURACAO_TMS.PermitirInformarTaraContainerCarga && exibirSomenteCampoVeiculo);
    knoutCarga.TaraContainerReboque.visible(_CONFIGURACAO_TMS.PermitirInformarTaraContainerCarga && knoutCarga.Reboque.visible());
    knoutCarga.TaraContainerSegundoReboque.visible(_CONFIGURACAO_TMS.PermitirInformarTaraContainerCarga && knoutCarga.SegundoReboque.visible());
    knoutCarga.TaraContainerTerceiroReboque.visible(_CONFIGURACAO_TMS.PermitirInformarTaraContainerCarga && knoutCarga.TerceiroReboque.visible());
}

function controlarExibicaoDatasCarregamento(knoutCarga) {
    var exibirDatasCarregamento = _CONFIGURACAO_TMS.PermitirInformarDatasCarregamentoCarga && !knoutCarga.CargaDeComplemento.val();

    knoutCarga.InicioCarregamento.visible(exibirDatasCarregamento);
    knoutCarga.InicioCarregamento.required(exibirDatasCarregamento);
    knoutCarga.InicioCarregamento.text((exibirDatasCarregamento ? "*" : "") + Localization.Resources.Cargas.Carga.InicioDoCarregamento.getFieldDescription());

    knoutCarga.TerminoCarregamento.visible(exibirDatasCarregamento);
    knoutCarga.TerminoCarregamento.required(exibirDatasCarregamento);
    knoutCarga.TerminoCarregamento.text((exibirDatasCarregamento ? "*" : "") + Localization.Resources.Cargas.Carga.TerminoDoCarregamento.getFieldDescription());
}

function ocultarExibicaoCamposVeiculo(knoutCarga) {
    knoutCarga.Veiculo.required = false;
    knoutCarga.Veiculo.visible(false);

    knoutCarga.Reboque.required = false;
    knoutCarga.Reboque.visible(false);

    knoutCarga.SegundoReboque.required = false;
    knoutCarga.SegundoReboque.visible(false);
    knoutCarga.TerceiroReboque.required = false;
    knoutCarga.TerceiroReboque.visible(false);

    knoutCarga.DataRetiradaCtrnVeiculo.visible(false);
    knoutCarga.DataRetiradaCtrnVeiculo.required(false);

    knoutCarga.DataRetiradaCtrnReboque.visible(false);
    knoutCarga.DataRetiradaCtrnReboque.required(false);

    knoutCarga.DataRetiradaCtrnSegundoReboque.visible(false);
    knoutCarga.DataRetiradaCtrnSegundoReboque.required(false);

    knoutCarga.DataRetiradaCtrnTerceiroReboque.visible(false);
    knoutCarga.DataRetiradaCtrnTerceiroReboque.required(false);

    knoutCarga.GensetVeiculo.visible(false);
    knoutCarga.GensetReboque.visible(false);
    knoutCarga.GensetSegundoReboque.visible(false);
    knoutCarga.GensetTerceiroReboque.visible(false);

    knoutCarga.MaxGrossVeiculo.visible(false);
    knoutCarga.MaxGrossReboque.visible(false);
    knoutCarga.MaxGrossSegundoReboque.visible(false);
    knoutCarga.MaxGrossTerceiroReboque.visible(false);

    knoutCarga.NumeroContainerVeiculo.visible(false);
    knoutCarga.NumeroContainerVeiculo.required(false);

    knoutCarga.NumeroContainerReboque.visible(false);
    knoutCarga.NumeroContainerReboque.required(false);

    knoutCarga.NumeroContainerSegundoReboque.visible(false);
    knoutCarga.NumeroContainerSegundoReboque.required(false);

    knoutCarga.NumeroContainerTerceiroReboque.visible(false);
    knoutCarga.NumeroContainerTerceiroReboque.required(false);

    knoutCarga.TaraContainerVeiculo.visible(false);
    knoutCarga.TaraContainerReboque.visible(false);
    knoutCarga.TaraContainerSegundoReboque.visible(false);
    knoutCarga.TaraContainerTerceiroReboque.visible(false);
}

function dadosPedidoReferenteFiltros(pedido, listaCargaPedidoNF) {
    if (!_pesquisaCarga)
        return false;

    if (Boolean(pedido.NumeroPedido) && Boolean(_pesquisaCarga.NumeroPedido.val()) && pedido.NumeroPedido == _pesquisaCarga.NumeroPedido.val())
        return true;

    var existe = false;
    if (Boolean(_pesquisaCarga.NumeroNF.val())) {
        $.each(listaCargaPedidoNF.CargaPedido, function (i, cargaPedido) {
            if (cargaPedido.CodigoCargaPedido == pedido.CodigoCargaPedido && cargaPedido.NumeroNF == _pesquisaCarga.NumeroNF.val()) {
                existe = true;
                return false;
            }
        });
    }

    return existe;
}

function retornarCargasPedidoQuePossuemANotaFiltrada(callback) {
    if (!Boolean(_pesquisaCarga))
        return callback();

    if (_pesquisaCarga.NumeroNF == undefined || !Boolean(_pesquisaCarga.NumeroNF.val()) || _cargaAtual.Pedidos == undefined || _cargaAtual.Pedidos.val.length == 0)
        return callback();

    var data = {
        CodigoCarga: _cargaAtual.Codigo.val(),
        NumeroNF: _pesquisaCarga.NumeroNF.val()
    };
    executarReST("DocumentoNF/RetornarCargasPedidoPorNumeroNF", data, function (arg) {
        if (arg.Success) {
            callback(arg.Data);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function carregarFiltrosPesquisaInicialCarga() {
    const codigo = sessionStorage.getItem('codigoCarga');
    if (codigo) {
        _pesquisaCarga.Codigo.val(codigo || '');
        _pesquisaCarga.ExibirFiltros.visibleFade(true);
        sessionStorage.removeItem('codigoCarga');
    }
}

//*******MÉTODOS COMPARTILHADOS*******

function preencherTabsPedidos(idTab, funcaoTab, indice, edicaoHabilitada, knoutAplicarTodosPedidosCarga, callback) {
    if (edicaoHabilitada == null)
        edicaoHabilitada = true;

    retornarCargasPedidoQuePossuemANotaFiltrada(function (listaCargaPedidoNF) {

        var html = "";
        if (_cargaAtual.Pedidos.val.length > 1) {
            $.each(_cargaAtual.Pedidos.val, function (i, pedido) {
                var idLi = idTab + "_" + i;
                let colors = '';

                html += '<li id="' + idLi + '" class="nav-item">';

                if (pedido.PossuiNFeLancada) {
                    if (pedido.CanceladoAposVinculoCarga) {
                        colors = 'gray';
                    }
                    else {
                        colors = 'green';
                    }
                }

                if (pedido.AgInformarRecebedor) colors = 'yellow';

                if (pedido.PossuiNFePesoZerado && (
                    pedido.TipoContratacaoCarga == EnumTipoContratacaoCarga.Normal ||
                    pedido.TipoContratacaoCarga == EnumTipoContratacaoCarga.NormalESubContratada ||
                    _cargaAtual.FreteDeTerceiro.val()
                )
                ) colors = 'red';

                if (dadosPedidoReferenteFiltros(pedido, listaCargaPedidoNF)) {
                    colors = 'purple';
                }

                if (i == indice)
                    html += '<a href="javascript:void(0);" class="nav-link ' + colors + ' active" onclick="' + funcaoTab + '(' + i + ')"><span>';
                else
                    html += '<a href="javascript:void(0);" class="nav-link ' + colors + '" onclick="' + funcaoTab + '(' + i + ')"><span>';

                if (pedido.ReentregaSolicitada)
                    html += '<i class="fal fa-redo"></i>';

                if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
                    if (string.IsNullOrWhiteSpace(pedido.NumeroPedidoEmbarcador))
                        html += "&nbsp;&minus;&nbsp;";
                    else
                        html += pedido.NumeroPedidoEmbarcador;
                }

                if (pedido.NumeroPedido === null || pedido.NumeroPedido === undefined || string.IsNullOrWhiteSpace(pedido.NumeroPedido) || pedido.NumeroPedido === "0")
                    html += "&nbsp;&minus;&nbsp;";
                else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS || _CONFIGURACAO_TMS.PadraoVisualizacaoOperadorLogistico)
                    html += " Nº " + pedido.NumeroPedido;

                if (!string.IsNullOrWhiteSpace(pedido.NumeroContainer))
                    html += " - CT.: " + pedido.NumeroContainer;

                html += '</span>';

                if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS &&
                    (_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.Nova || _cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgNFe) &&
                    (pedido.AdicionadaManualmente === true) &&
                    VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermitirRemoverPedido, _PermissoesPersonalizadasCarga) &&
                    !_FormularioSomenteLeitura) {
                    html += '<button class="inner-btn btn-danger" onclick="ExcluirPedidoAdicionadoManualmente(event, ' + i.toString() + ');"><i class="fal fa-times"></i></button>';
                }
                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS &&
                    (_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.Nova || _cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgNFe) &&
                    (pedido.AdicionadaManualmente === true || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) &&
                    !_FormularioSomenteLeitura) {
                    html += '<button class="inner-btn btn-warning" onclick="EditarPedidoAdicionadoManualmente(event, ' + i.toString() + ');"><i class="fal fa-edit"></i></button>';
                }

                html += '</a></li>';

            });
        }

        var htmlAdicionarNovoPedido = '';
        if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
            htmlAdicionarNovoPedido = '<li><button class="btn btn-primary btn-icon me-2 mb-2" type="button" onclick="abrirModalAdicionarNovoPedido()"><i class="fal fa-plus"></i></button></li>';

        var htmlRemoverPedidos = '<li><button class="btn btn-primary btn-icon me-2 mb-2" type="button" onclick="cancelarPedidosSemNota()"><i class="fal fa-minus"></i></button></li>';

        var htmlAdicionarEditarPedidos = '';
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS || _cargaAtual.PossuiMontagemContainer.val() ||
            (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && _cargaAtual.SituacaoCarga.val() === EnumSituacoesCarga.AgNFe && _cargaAtual.ExigeNotaFiscalParaCalcularFrete.val()))
            htmlAdicionarEditarPedidos = '<li><button class="btn btn-primary btn-icon me-2 mb-2" type="button" onclick="EditarPedidoAdicionadoManualmente(event, 0)"><i class="fal fa-edit"></i></button></li>';
        else
            htmlAdicionarEditarPedidos = '<li><button class="btn btn-primary btn-icon me-2 mb-2" type="button" onclick="abrirModalEditarPedidos()"><i class="fal fa-edit"></i></button></li>';

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
            if (_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.Nova || _cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.CalculoFrete || (_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgNFe && _cargaAtual.ExigeNotaFiscalParaCalcularFrete.val()) || _cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgTransportador) {
                html = htmlAdicionarEditarPedidos + html;
                html += htmlAdicionarNovoPedido;
            }
        } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
            if (_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.Nova || _cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgNFe) {

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS && _cargaAtual.Pedidos.val.length <= 1)
                    html = htmlAdicionarEditarPedidos + html;
                html += htmlAdicionarNovoPedido;

                if (_CONFIGURACAO_TMS.PermitirCancelarPedidosSemDocumentos && _cargaAtual.Pedidos.val.length > 1)
                    html += htmlRemoverPedidos;
            }
        }

        $("#" + idTab).html(html);
        if (knoutAplicarTodosPedidosCarga != null) {
            if (edicaoHabilitada) {//se não permite mais editar os pedidos, apenas para exibição serão exibidas todas as abas
                knoutAplicarTodosPedidosCarga.visible(true);
            } else {
                knoutAplicarTodosPedidosCarga.visible(false);
                knoutAplicarTodosPedidosCarga.val(false);
            }
            if (!knoutAplicarTodosPedidosCarga.val())
                $("#" + idTab).show();
        } else {
            $("#" + idTab).show();
        }

        if (callback != null)
            callback();
    });
}

function desabilitarTodasOpcoes(knoutCarga) {
    knoutCarga.EtapaInicioTMS.enable(false);
    knoutCarga.EtapaInicioEmbarcador.enable(false);
    knoutCarga.EtapaFreteEmbarcador.enable(false);
    knoutCarga.EtapaFreteTMS.enable(false);
    knoutCarga.EtapaDadosTransportador.enable(false);
    knoutCarga.EtapaNotaFiscal.enable(false);
    knoutCarga.EtapaCTeNFs.enable(false);
    knoutCarga.EtapaMDFe.enable(false);
    knoutCarga.EtapaImpressao.enable(false);
    knoutCarga.EtapaIntegracao.enable(false);
    knoutCarga.SalvarDadosCarga.visible(false);
    knoutCarga.RecalcularFrete.visible(false);
    knoutCarga.RecalcularFreteBID.visible(false);
    knoutCarga.ValorPorPedido.visible(false);
    knoutCarga.ValorPorPedidoFilialEmissora.visible(false);
    knoutCarga.ExcluirPreCalculo.visible(false);
    knoutCarga.CargaPercursos.visible(false);
    knoutCarga.AlterarOperador.visible(false);
    knoutCarga.AlterarExternalID1.visible(false);
    knoutCarga.AlterarExternalID2.visible(false);
    knoutCarga.AlterarDataCarregamento.visible(false);
    knoutCarga.Veiculo.enable(false);
    knoutCarga.Reboque.enable(false);
    knoutCarga.SegundoReboque.enable(false);
    knoutCarga.TerceiroReboque.enable(false);
    knoutCarga.Motorista.enable(false);
    knoutCarga.SalvarDadosTransportador.visible(false);
    knoutCarga.DisponibilizarParaTransportadorCarga.visible(false);
    knoutCarga.DisponibilizarParaTransportador.visible(false);
    knoutCarga.AdicionarComplementoFrete.visible(false);
    knoutCarga.ComponenteFrete.visible(false);
    knoutCarga.ValorFreteOperador.visible(false);
    knoutCarga.AlterarRotaFreteCarga.visible(false);
    knoutCarga.VisualizarRotaMapa.visible(false);
    knoutCarga.CargaDesabilitada.enable(false);
    knoutCarga.DataRetiradaCtrnVeiculo.enable(false);
    knoutCarga.DataRetiradaCtrnReboque.enable(false);
    knoutCarga.DataRetiradaCtrnSegundoReboque.enable(false);
    knoutCarga.DataRetiradaCtrnTerceiroReboque.enable(false);
    knoutCarga.GensetVeiculo.enable(false);
    knoutCarga.GensetReboque.enable(false);
    knoutCarga.GensetSegundoReboque.enable(false);
    knoutCarga.GensetTerceiroReboque.enable(false);
    knoutCarga.MaxGrossVeiculo.enable(false);
    knoutCarga.MaxGrossReboque.enable(false);
    knoutCarga.MaxGrossSegundoReboque.enable(false);
    knoutCarga.MaxGrossTerceiroReboque.enable(false);
    knoutCarga.NumeroContainerVeiculo.enable(false);
    knoutCarga.NumeroContainerReboque.enable(false);
    knoutCarga.NumeroContainerSegundoReboque.enable(false);
    knoutCarga.NumeroContainerTerceiroReboque.enable(false);
    knoutCarga.TaraContainerVeiculo.enable(false);
    knoutCarga.TaraContainerReboque.enable(false);
    knoutCarga.TaraContainerSegundoReboque.enable(false);
    knoutCarga.TaraContainerTerceiroReboque.enable(false);
    knoutCarga.ObservacaoTransportador.enable(false);
}

function carregarConteudosHTML(callback) {
    loadCargaFrete();
    BuscarPermissoesEdicaoCTe();
    $.get("Content/Static/Carga/CargaModais.html?dyn=" + guid(), function (data) {
        $("#ModaisCarga").html(data);
        $.get("Content/Static/Carga/Carga.html?dyn=" + guid(), function (data) {
            _HTMLCarga = data;
            $.get("Content/Static/Carga/PendenciasCarga.html?dyn=" + guid(), function (data) {
                _HTMLPendenciasCarga = data;
            });
            $.get("Content/Static/Carga/CargaCTe.html?dyn=" + guid(), function (data) {
                _HTMLCTE = data;
            });
            $.get("Content/Static/Carga/CargaDadosMercante.html?dyn=" + guid(), function (data) {
                _HTMLDadosMercante = data;
            });
            $.get("Content/Static/Carga/CargaMDFe.html?dyn=" + guid(), function (data) {
                _HTMLMDFE = data;
            });
            $.get("Content/Static/Carga/CargaDadosEmissao.html?dyn=" + guid(), function (data) {
                _HTMLDadosEmissao = data;
            });
            $.get("Content/Static/Carga/CargaIntegracao.html?dyn=" + guid(), function (data) {
                _HTMLIntegracaoCarga = data;
            });
            $.get("Content/Static/Carga/CargaDadosIntegracaoFaturamento.html?dyn=" + guid(), function (data) {
                _HTMLIntegracaoFaturamento = data;
            });
            $.get("Content/Static/Carga/CargaIntegracaoTransportador.html?dyn=" + guid(), function (data) {
                _HTMLIntegracaoCargaTransportador = data;
            });
            $.get("Content/Static/Carga/CargaContratoFrete.html?dyn=" + guid(), function (data) {
                _HTMLCargaContratoFrete = data;
            });
            $.get("Content/Static/Carga/CargaTransbordo.html?dyn=" + guid(), function (data) {
                _HTMLTransbordo = data;
            });
            $.get("Content/Static/Carga/CargaMDFeAquaviario.html?dyn=" + guid(), function (data) {
                _HTMLDadosMDFeAquaviario = data;
            });
            $.get("Content/Static/Carga/CargaSVM.html?dyn=" + guid(), function (data) {
                _HTMLDadosSVM = data;
            });
            $.get("Content/Static/Carga/CargaFaturamento.html?dyn=" + guid(), function (data) {
                _HTMLDadosFaturamento = data;
            });
            $.get("Content/Static/Carga/MapaRoteirizacao.html?dyn=" + guid(), function (data) {
                _HTMLMapaRoteirizacao = data;
            });

            $.get("Content/Static/Carga/CargaPedidoDocumentosCTe.html?dyn=" + guid(), function (data) {
                _HTMLCargaPedidoDocumentoCTe = data;
            });

            $.get("Content/Static/Carga/CargaPedidoDocumentoMDFe.html?dyn=" + guid(), function (data) {
                _HTMLCargaPedidoDocumentoMDFe = data;
            });

            $.get("Content/Static/Carga/DetalhesDocumentoEmissao.html?dyn=" + guid(), function (data) {
                _HTMLDetalhesDocumentoEmissao = data;
            });

            $.get("Content/Static/Carga/SimulacaoFrete.html?dyn=" + guid(), function (data) {
                _HTMLSimulacaoFrete = data;
            });

            $.get("Content/Static/Carga/CargaOperacaoContainer.html?dyn=" + guid(), function (data) {
                _HTMLOperacaoContainer = data;
            });

            loadComposicaoFrete();
            LoadAlteracaoMoedaCarga();
            loadCargaNotasPreckin();
            loadPacotes();

            callback();
        });
    });
}

function SalvarDadosTransporteSemLiberarTeleriscoClick(e) {
    e.LiberadoComProblemaIntegracaoGrMotoristaVeiculo.val(false);
    e.SalvarDadosTransporteSemSolicitarNFes.val(e.SalvarDadosTransporteESolicitarNFes.visible());
    SalvarDadosTransporteClick(e);
}

function SalvarDadosTransporteSemLiberarTeleriscoSolicitandoNFesClick(e) {
    e.LiberadoComProblemaIntegracaoGrMotoristaVeiculo.val(false);
    e.SalvarDadosTransporteSemSolicitarNFes.val(false);
    SalvarDadosTransporteClick(e);
}

function LiberarComProblemaIntegracaoGrMotoristaVeiculoClick(e) {
    if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermiteLiberarSemIntegracaoGR, _PermissoesPersonalizadasCarga)) {
        if (_CONFIGURACAO_TMS.ExigirProtocoloLiberacaoSemIntegracaoGR)
            exibirModalLiberacaoSemIntegracaoGR(e);
        else {
            e.LiberadoComProblemaIntegracaoGrMotoristaVeiculo.val(true);
            e.SalvarDadosTransporteSemSolicitarNFes.val(false);
            SalvarDadosTransporteClick(e);
        }
    }
    else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.LiberacaoGr, Localization.Resources.Cargas.Carga.SeuUsuarioNaoPossuiPermissaoParaLiberarCargaSemIntegracaoDaGr);
    }
}

function LiberarComLicencaInvalidaClick(e) {
    e.LiberadaComLicencaInvalida.val(true);
    e.SalvarDadosTransporteSemSolicitarNFes.val(false);
    SalvarDadosTransporteClick(e);
}

function verDetalhesCargaAgrupadaClick(e) {
    exibirDetalhesCargaAgrupada(e)
}

function informarRetiradaContainerClick(e) {
    exibirCargaRetiradaContainer();
}

function setaNumeroPedidoCliente(pedidos, knout) {
    if (pedidos.length == 0)
        return;

    let pedidosComNumeroCliente = pedidos.filter(pedido => Boolean(pedido.CodigoPedidoCliente));

    if (pedidosComNumeroCliente.length == 0)
        return;

    knout.NumeroPedidoCliente.val(pedidosComNumeroCliente.map(pedido => pedido.CodigoPedidoCliente).join(", "));
    knout.NumeroPedidoCliente.visible(true);
}

function preencherDadosVinculadosVeiculo(knoutCarga, data) {
    limparGridMotoristas(knoutCarga);

    if (data != null) {
        if (!knoutCarga.ModeloVeicularCarga.codEntity() > 0 && !knoutCarga.ModeloVeicularCarga.val() != "") {
            knoutCarga.ModeloVeicularCarga.codEntity(data.CodigoModeloVeicularCarga);
            knoutCarga.ModeloVeicularCarga.val(data.ModeloVeicularCarga);
        }

        if (!knoutCarga.Empresa.codEntity() > 0 && !knoutCarga.Empresa.val() != "") {
            knoutCarga.Empresa.codEntity(data.CodigoEmpresa);
            knoutCarga.Empresa.val(data.EmpresaDescricao);
        }

        if (data.CodigoMotorista > 0 && data.NomeMotorista != "" && data.CPFMotorista != "") {
            const motorista = { Codigo: data.CodigoMotorista, Nome: data.NomeMotorista, CPF: data.CPFMotorista };
            if (knoutCarga.SituacaoCarga.val() == EnumSituacoesCarga.Nova) {
                AdicionarMotoristaCargaClick(motorista, knoutCarga, EnumSituacoesCarga.Nova);
            }
            else {
                AdicionarMotoristaCargaClick(motorista, knoutCarga, EnumSituacoesCarga.AgTransportador);
                BuscarMotoristas(knoutCarga.Motorista, null, knoutCarga.Empresa, null, true, EnumSituacaoColaborador.Trabalhando);
            }
        }
        else
        {
            limparGridMotoristas(knoutCarga);
        }
    }
}

function limparGridMotoristas(knoutCarga) {
    var aux = [];
    var gridMotoristas = knoutCarga.AdicionarMotoristas.basicTable;
    gridMotoristas.CarregarGrid(aux);
}

function recalcularPrecalculo(e) {
    let codigoCarga = e.Codigo.val()
    executarReST("CargaFrete/RecalcularPrecalculo", { Carga: codigoCarga }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Error", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Successo", "O pre calculo foi enviado para recalcular novamente")
    });
}
function atualizarResumoCargaTerceiro() {
    if (_cargaAtual.FreteDeTerceiro.val()) {
        executarReST("Carga/BuscarCargaPorCodigo", { Carga: _cargaAtual.Codigo.val() }, function (arg) {
            if (arg.Success) {
                atualizarDadosCarga(_cargaAtual, arg.Data);
            }
        });
    }
}

function configurarCampoJustificativaCustoExtra(knoutCarga, obrigatorioJustificarCustoExtra) {
    if (_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum && obrigatorioJustificarCustoExtra) {
        BuscarJustificativaAutorizacaoCarga(knoutCarga.JustificativaAutorizacaoCarga);
        knoutCarga.JustificativaAutorizacaoCarga.visible(true);
        knoutCarga.JustificativaAutorizacaoCarga.enable(true);
        knoutCarga.Setor.visible(true);
        knoutCarga.Setor.enable(true);
        knoutCarga.ObservacaoCarga.visible(true);

    } else {
        knoutCarga.JustificativaAutorizacaoCarga.visible(false);
        knoutCarga.JustificativaAutorizacaoCarga.enable(false);
        knoutCarga.Setor.visible(false);
        knoutCarga.Setor.enable(false);
        knoutCarga.ObservacaoCarga.visible(false);
    }
    configurarObrigatoridadeCampoJustificativaCustoExtra(knoutCarga.JustificativaAutorizacaoCarga, obrigatorioJustificarCustoExtra);
}

function configurarObrigatoridadeCampoJustificativaCustoExtra(justificativaCarga, obrigatorioJustificarCustoExtra) {
    justificativaCarga.text(obrigatorioJustificarCustoExtra ? "*Justificativa Custo Extra" : "Justificativa Custo Extra");
    justificativaCarga.required = obrigatorioJustificarCustoExtra;
}

function configurarLiberarCargaSemPlanejamento(knoutCarga, liberarCargaSemPlanejamento) {
    knoutCarga.LiberarCargaSemPlanejamento.visible(liberarCargaSemPlanejamento);
}

function LiberarCargaSemPlanejamentoClick(e) {
    e.LiberadaComCargaSemPlanejamento.val(true);
    SalvarDadosTransporteClick(e);
}

function alterarTipoPagamentoValePedagioClick(e) {
    _cargaAtual = e;

    _knoutAlterarTipoPagamentoValePedagio = new AlterarTipoPagamentoValePedagio();
    KoBindings(_knoutAlterarTipoPagamentoValePedagio, "knoutAlterarTipoPagamentoValePedagio");

    _knoutAlterarTipoPagamentoValePedagio.Carga.val(e.Codigo.val());
    _knoutAlterarTipoPagamentoValePedagio.TipoPagamentoValePedagio.val(e.TipoPagamentoValePedagio.val());

    Global.abrirModal("divModalAlterarTipoPagamentoValePedagio");

    $("#divModalAlterarTipoPagamentoValePedagio").one('hidden.bs.modal', function () {
        LimparCampos(_knoutAlterarTipoPagamentoValePedagio);
    });
}

function confirmarAlterarTipoPagamentoValePedagioClick() {
    var dados = {
        Carga: _knoutAlterarTipoPagamentoValePedagio.Carga.val(),
        TipoPagamentoValePedagio: _knoutAlterarTipoPagamentoValePedagio.TipoPagamentoValePedagio.val()
    }

    executarReST("Carga/AlterarTipoPagamentoValePedagio", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _cargaAtual.TipoPagamentoValePedagio.val(_knoutAlterarTipoPagamentoValePedagio.TipoPagamentoValePedagio.val());
                _cargaAtual.DescricaoTipoPagamentoValePedagio.val(EnumTipoPagamentoValePedagio.obterDescricao(_knoutAlterarTipoPagamentoValePedagio.TipoPagamentoValePedagio.val()));

                Global.fecharModal("divModalAlterarTipoPagamentoValePedagio");
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.AlterarTipoPagamentoValePedagioDaCargaSucesso);
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function ClickRedirecionarHUBOfertas(carga) {
    if (carga != undefined) {

        sessionStorage.setItem('CodigoCarga', carga.Codigo.val());
        sessionStorage.setItem('CodigoCargaEmbarcador', carga.CodigoCargaEmbarcador.val());

        window.open("#integracoes/integracaohub", '_blank');
    }
}

function imprimirMinutaClick(e) {
    executarDownload("Carga/ImprimirCheckListMinuta", { Codigo: e.Codigo.val() });
}

$(document).on('shown.bs.tab', function (e) {
    if (_cargaAtual) {
        const tabExibida = $(e.target).attr('href');
        if (tabExibida) {
            if (tabExibida === '#tabDetalhesCarga_' + _cargaAtual.EtapaInicioEmbarcador.idGrid) {
                $('.adicionar-motoristas-detalhes-carga').show();
                $('.salvar-dados-detalhes-carga').show();
            } else {
                $('.adicionar-motoristas-detalhes-carga').hide();
                $('.salvar-dados-detalhes-carga').hide();
            }
        }
    }
});