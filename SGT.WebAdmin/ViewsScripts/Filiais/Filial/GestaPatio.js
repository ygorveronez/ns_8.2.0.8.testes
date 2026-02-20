/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="../../Enumeradores/EnumTipoFluxoGestaoPatio.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="../../Consultas/FilialBalanca.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gestaoPatio;
var $boxesSequenciaPatio = $("#knockoutGestaoPatio .boxes-sequencia-patio");
var $templateAltura = $("#style-altura-template");
var $styleAltura = $("#style-altura");
var _opcoesChecklistImpressao = [
    { value: 0, text: Localization.Resources.Gerais.Geral.Padrao }
];

/*
 * Declaração das Classes
 */

var GestaoPatio = function () {
    var configIntTempo = { precision: 0, allowZero: true };

    this.InformarDocaCarregamento = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.InformarDocaCarregamento), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.InformarDoca, def: false });
    this.InformarDocaCarregamento.val.subscribe(desabilitarBoxInformarDocaCarregamento);
    this.InformarDocaCarregamentoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.InformarDocaCarregamentoTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.InformarDocaCarregamentoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.InformarDocaCarregamentoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeInformarDocaCarregamentoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.InformarDocaCarregamentoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.InformarDocaCarregamentoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.InformarDocaCarregamentoPermiteTransportadorLancarHorarios = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteTransportadorLancarHorariosPatio, val: ko.observable(false), def: false });
    this.InformarDocaCarregamentoPermiteInformarDadosLaudo = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteInformarDadosLaudo, val: ko.observable(false), def: false });
    this.InformarDocaCarregamentoGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.MontagemCarga = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.MontagemCarga), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.MontagemCarga, def: false });
    this.MontagemCarga.val.subscribe(desabilitarBoxMontagemCarga);
    this.MontagemCargaInformarDoca = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.InformarDoca, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.MontagemCargaTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.MontagemCargaTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.MontagemCargaDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.MontagemCargaBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeMontagemCargaClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.MontagemCargaHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.MontagemCargaCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.MontagemCargaPermiteInformarQuantidadeCaixas = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteInformarQuantidadeCaixas, val: ko.observable(false), def: false });
    this.MontagemCargaPermiteInformarQuantidadeItens = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteInformarQuantidadeItens, val: ko.observable(false), def: false });
    this.MontagemCargaPermiteInformarQuantidadePallets = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteInformarQuantidadePallets, val: ko.observable(false), def: false });
    this.MontagemCargaGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.ChegadaVeiculo = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.ChegadaVeiculo), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.ChegadaVeiculo, def: false });
    this.ChegadaVeiculo.val.subscribe(desabilitarBoxChegadaVeiculo);
    this.ChegadaVeiculoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoChegada.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.ChegadaVeiculoTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.ChegadaVeiculoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.ChegadaVeiculoPermiteImprimirRelacaoDeProdutos = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteImprimirRelacaoProdutos, val: ko.observable(false), def: false });
    this.ChegadaVeiculoPreencherDataSaida = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PreencherDataSaida, val: ko.observable(false), def: false });
    this.ChegadaVeiculoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeChegadaVeiculoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.ChegadaVeiculoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.ChegadaVeiculoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.ChegadaVeiculoPermiteTransportadorLancarHorarios = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteTransportadorLancarHorariosPatio, val: ko.observable(false), def: false });
    this.ChegadaVeiculoImprimirComprovanteModeloColetaOutbound = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.ImprimirComprovanteNoModeloDeColetaOutbound, val: ko.observable(false), def: false });
    this.ChegadaVeiculoGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.GuaritaEntrada = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.GuaritaEntrada), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.Guarita, def: false });
    this.GuaritaEntrada.val.subscribe(desabilitarBoxGuaritaEntrada);
    this.GuaritaEntradaInformarDoca = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.InformarDoca, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.GuaritaEntradaPermiteInformacoesPesagem = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformacoesPesagem, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaEntradaPermiteInformacoesProdutor = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformacoesProdutor, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaEntradaPermiteInformarAnexoPesagem = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformarAnexosPesagem, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaEntradaPermiteInformarPressaoPesagem = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformarPressaoPesagem, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaEntradaPermiteInformarQuantidadeCaixasPesagem = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformarQuantidadeCaixasPesagem, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaEntradaPermiteDenegarChegada = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteDenegarChegada, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaEntradaExibirHorarioExato = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.ExibirHorarioExato, val: ko.observable(true), def: true, visible: false });
    this.GuaritaEntradaTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.GuaritaEntradaTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.GuaritaEntradaDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.GuaritaEntradaHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.GuaritaEntradaCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.GuaritaEntradaPermiteTransportadorLancarHorarios = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteTransportadorLancarHorariosPatio, val: ko.observable(false), def: false });
    this.GuaritaEntradaBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeGuaritaEntradaClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.GuaritaEntradaPermiteInformarDadosDevolucao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteInformarDadosDevolucao, val: ko.observable(false), def: false });
    this.BalancaGuaritaEntrada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Filiais.Filial.Balanca.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });
    this.GuaritaEntradaGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.GuaritaEntradaTipoIntegracaoBalanca = PropertyEntity({ val: ko.observable(""), def: "", options: EnumTipoIntegracao.obterOpcoesIntegracaoBalancaGestaoPatio(), text: Localization.Resources.Gerais.Geral.Integracao.getFieldDescription(), visible: ko.observable(false) });

    this.CheckList = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.Checklist), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.CheckList, def: false });
    this.CheckList.val.subscribe(desabilitarBoxCheckList);
    this.CheckListInformarDoca = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.InformarDoca, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.CheckListTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.CheckListTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.CheckListDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.CheckListBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeCheckListClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.CheckListPermiteSalvarSemPreencher = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteSalvarSemPreencher, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.TipoCheckListImpressao = PropertyEntity({ text: Localization.Resources.Filiais.Filial.TipoChecklist, val: ko.observable(false), visible: ko.observable(false), enable: ko.observable(false) });
    this.CheckListHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.CheckListCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.CheckListPermiteImpressaoApenasComCheckListFinalizada = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteImpressaoApenasChecklistFinalizada, val: ko.observable(false), def: false });
    this.CheckListCancelarPatioAoReprovar = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.CancelarPatioReprovarChecklist, val: ko.observable(false), def: false });
    this.CheckListNotificarPorEmailReprovacao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.NotificarEmailReprovacaoChecklist, val: ko.observable(false), def: false });
    this.CheckListNaoExigeObservacaoAoReprovar = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.NaoExigirObservacaoReprovar, val: ko.observable(false), def: false });
    this.CheckListPermitePreencherCheckListAntesDeChegarNaEtapa = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermitePreencherChecklistAntesChegarEtapa, val: ko.observable(false), def: false });
    this.CheckListPermiteTransportadorLancarHorarios = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteTransportadorLancarHorariosPatio, val: ko.observable(false), def: false });
    this.CheckListEmails = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.EmailsSeparadosPontoEVirgula.getFieldDescription(), val: ko.observable(""), maxlength: 300 });
    this.CheckListAssinaturaMotorista = PropertyEntity({ text: Localization.Resources.Filiais.Filial.CheckListAssinaturaMotorista, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CheckListAssinaturaCarregador = PropertyEntity({ text: Localization.Resources.Filiais.Filial.CheckListAssinaturaCarregador, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CheckListAssinaturaResponsavelAprovacao = PropertyEntity({ text: Localization.Resources.Filiais.Filial.CheckListAssinaturaResponsavelAprovacao, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CheckListExigirAnexo = PropertyEntity({ text: Localization.Resources.Filiais.Filial.ExigirAnexo, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CheckListGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.CheckListGerarNovoPedidoAoTerminoFluxo = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.CheckListGerarNovoPedidoAoTerminoFluxo, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.CheckListTipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoDeOperacao, idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(false) });
    this.CheckListDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destinatario, idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(false) });
    this.CheckListExigirAnexo = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.ExigirAnexo, val: ko.observable(false), def: false });
    this.CheckListUtilizarVigencia = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.UtilizarVigenciaChecklist, val: ko.observable(false), def: false });

    this.TravaChave = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.TravaChave), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.TravamentoChave, def: false });
    this.TravaChave.val.subscribe(desabilitarBoxTravaChave);
    this.TravaChaveInformarDoca = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.InformarDoca, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.TravaChaveTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.TravaChaveTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.TravaChaveDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.TravaChaveBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeTravaChaveClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.TravaChaveHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.TravaChaveCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.TravaChavePermiteTransportadorLancarHorarios = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteTransportadorLancarHorariosPatio, val: ko.observable(false), def: false });
    this.TravaChaveExigirAnexo = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.ExigirAnexo, val: ko.observable(false), def: false });
    this.TravaChaveGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.Expedicao = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.Expedicao), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.Expedicao, def: false });
    this.Expedicao.val.subscribe(desabilitarBoxExpedicao);
    this.ExpedicaoInformarDoca = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.InformarDoca, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ExpedicaoInformarInicioCarregamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.InformarInicioCarregamento, val: ko.observable(false), def: false });
    this.ExpedicaoInformarTerminoCarregamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.InformarTerminoCarregamento, val: ko.observable(false), def: false });
    this.ExpedicaoConfirmarPlaca = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.ConfirmarPlaca, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ExpedicaoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.ExpedicaoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.ExpedicaoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeExpedicaoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.ExpedicaoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.ExpedicaoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.ExpedicaoGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.LiberaChave = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.LiberaChave), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.LiberacaoChave, def: false });
    this.LiberaChave.val.subscribe(desabilitarBoxLiberaChave);
    this.LiberaChaveTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.LiberaChaveTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.LiberaChaveDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.LiberaChaveBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeLiberaChaveClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.LiberaChaveHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.LiberaChaveCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.LiberaChaveBloquearLiberacaoEtapaAnterior = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.BloquearLiberacaoChaveCasoFluxoEstejaEmEtapaAnterior, val: ko.observable(false), def: false });
    this.LiberaChaveInformarNumeroDePaletes = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.InformarNumeroDePaletesObrigatorio, val: ko.observable(false), def: false });
    this.LiberaChaveSolicitarAssinaturaMotorista = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.SolicitarAssinaturaMotorista, val: ko.observable(false), def: false });
    this.LiberaChaveGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.LiberaChaveExigirAnexo = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.ExigirAnexo, val: ko.observable(false), def: false });

    this.Faturamento = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.ControlarFaturamento), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.Faturamento, def: false });
    this.Faturamento.val.subscribe(desabilitarBoxFaturamento);
    this.FaturamentoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.FaturamentoTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.FaturamentoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.FaturamentoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeFaturamentoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.FaturamentoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.FaturamentoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.FaturamentoGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.GuaritaSaida = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.GuaritaSaida), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.InicioViagem, def: false });
    this.GuaritaSaida.val.subscribe(desabilitarBoxGuaritaSaida);
    this.GuaritaSaidaPermiteInformacoesPesagem = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformacoesPesagem, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaSaidaPermiteInformarLacrePesagem = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformarLacrePesagem, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaSaidaPermiteInformarPercentualRefugoPesagem = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformarPercentualRefugoPesagem, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaSaidaPermiteAnexosPesagem = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteAnexosPesagemFinal, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaSaidaIniciarEmissaoDocumentosTransporte = PropertyEntity({ text: Localization.Resources.Filiais.Filial.GuaritaSaidaIniciarEmissaoDocumentosTransporte, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaSaidaTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.GuaritaSaidaDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.GuaritaSaidaBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeGuaritaSaidaClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.GuaritaSaidaHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.ImprimirTicketBalanca = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.ImprimirTicketBalanca, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaSaidaCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.BalancaGuaritaSaida = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Filiais.Filial.Balanca.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });
    this.GuaritaSaidaGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.GuaritaSaidaExigirAnexo = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.ExigirAnexo, val: ko.observable(false), def: false });
    this.GuaritaSaidaTipoIntegracaoBalanca = PropertyEntity({ val: ko.observable(""), def: "", options: EnumTipoIntegracao.obterOpcoesIntegracaoBalancaGestaoPatio(), text: Localization.Resources.Gerais.Geral.Integracao.getFieldDescription(), visible: ko.observable(false) });
    this.GuaritaSaidaExigirAnexo = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.ExigirAnexo, val: ko.observable(false), def: false });

    this.Posicao = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.Posicao), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.Posicao, def: false });
    this.Posicao.val.subscribe(desabilitarBoxPosicao);
    this.PosicaoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.PosicaoTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.PosicaoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.PosicaoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodePosicaoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.PosicaoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.PosicaoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.PosicaoGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.ChegadaLoja = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.ChegadaDestinatario), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.ChegadaLoja, def: false });
    this.ChegadaLoja.val.subscribe(desabilitarBoxChegadaLoja);
    this.ChegadaLojaTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.ChegadaLojaTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.ChegadaLojaDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.ChegadaLojaBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeChegadaLojaClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.ChegadaLojaHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.ChegadaLojaCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.ChegadaLojaGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.DeslocamentoPatio = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.DeslocamentoPatio), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.DeslocamentoPatio, def: false });
    this.DeslocamentoPatio.val.subscribe(desabilitarBoxDeslocamentoPatio);
    this.DeslocamentoPatioPermiteInformacoesPesagem = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformacoesPesagem, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.DeslocamentoPatioPermiteInformacoesLoteInterno = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformacoesLoteInterno, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.DeslocamentoPatioPermiteInformarQuantidade = PropertyEntity({ text: Localization.Resources.Filiais.Filial.DeslocamentoPatioPermiteInformarQuantidade, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.DeslocamentoPatioTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.DeslocamentoPatioTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.DeslocamentoPatioDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.DeslocamentoPatioBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeDeslocamentoPatioClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.DeslocamentoPatioHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.DeslocamentoPatioCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.DeslocamentoPatioGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.SaidaLoja = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.SaidaDestinatario), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.SaidaLoja, def: false });
    this.SaidaLoja.val.subscribe(desabilitarBoxSaidaLoja);
    this.SaidaLojaTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.SaidaLojaTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.SaidaLojaDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.SaidaLojaBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeSaidaLojaClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.SaidaLojaHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.SaidaLojaCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.SaidaLojaGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.FimViagem = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.FimViagem), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.FimViagem, def: false });
    this.FimViagem.val.subscribe(desabilitarBoxFimViagem);
    this.FimViagemTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.FimViagemTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.FimViagemDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.FimViagemBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeFimViagemClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.FimViagemHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.FimViagemCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.FimViagemGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.InicioHigienizacao = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.InicioHigienizacao), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.InicioHigienizacao, def: false });
    this.InicioHigienizacao.val.subscribe(desabilitarBoxInicioHigienizacao);
    this.InicioHigienizacaoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.InicioHigienizacaoTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.InicioHigienizacaoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.InicioHigienizacaoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeInicioHigienizacaoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.InicioHigienizacaoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.InicioHigienizacaoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.InicioHigienizacaoGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.FimHigienizacao = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.FimHigienizacao), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.FimHigienizacao, def: false });
    this.FimHigienizacao.val.subscribe(desabilitarBoxFimHigienizacao);
    this.FimHigienizacaoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.FimHigienizacaoTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.FimHigienizacaoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.FimHigienizacaoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeFimHigienizacaoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(true), permiteBaixarQRCode: false });
    this.FimHigienizacaoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.FimHigienizacaoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.FimHigienizacaoGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.InicioCarregamento = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.InicioCarregamento), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.InicioCarregamento, def: false });
    this.InicioCarregamento.val.subscribe(desabilitarBoxInicioCarregamento);
    this.InicioCarregamentoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.InicioCarregamentoTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.InicioCarregamentoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.InicioCarregamentoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeInicioCarregamentoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.InicioCarregamentoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.InicioCarregamentoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.InicioCarregamentoPermiteInformarPesagem = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteInformarPesagem, val: ko.observable(false), def: false });
    this.InicioCarregamentoGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.FimCarregamento = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.FimCarregamento), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.FimCarregamento, def: false });
    this.FimCarregamento.val.subscribe(desabilitarBoxFimCarregamento);
    this.FimCarregamentoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.FimCarregamentoTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.FimCarregamentoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.FimCarregamentoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeFimCarregamentoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.FimCarregamentoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.FimCarregamentoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.FimCarregamentoPermiteInformarPesagem = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteInformarPesagem, val: ko.observable(false), def: false });
    this.FimCarregamentoGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.SeparacaoMercadoria = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.SeparacaoMercadoria), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.SeparacaoMercadoria, def: false });
    this.SeparacaoMercadoria.val.subscribe(desabilitarBoxSeparacaoMercadoria);
    this.SeparacaoMercadoriaTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.SeparacaoMercadoriaTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.SeparacaoMercadoriaDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.SeparacaoMercadoriaBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeSeparacaoMercadoriaClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.SeparacaoMercadoriaHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.SeparacaoMercadoriaCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.SeparacaoMercadoriaPermiteInformarDadosCarregadores = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteInformarDadosCarregadores, val: ko.observable(false), def: false });
    this.SeparacaoMercadoriaPermiteInformarDadosSeparadores = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteInformarDadosSeparadores, val: ko.observable(false), def: false });
    this.SeparacaoMercadoriaGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.SolicitacaoVeiculo = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.SolicitacaoVeiculo), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.SolicitacaoVeiculo, def: false });
    this.SolicitacaoVeiculo.val.subscribe(desabilitarBoxSolicitacaoVeiculo);
    this.SolicitacaoVeiculoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.SolicitacaoVeiculoTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.SolicitacaoVeiculoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.SolicitacaoVeiculoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeSolicitacaoVeiculoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.SolicitacaoVeiculoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.SolicitacaoVeiculoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.SolicitacaoVeiculoGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.SolicitacaoVeiculoPermitirInformarDadosTransporteCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteInformarDadosTransporteCarga, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.NaoPermitirEnviarSMS = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.NaoPermitirEnviarSMS, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.SolicitacaoVeiculoHabilitarIntegracaoPager = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracaoPager, val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.DocumentoFiscal = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.DocumentoFiscal), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.DocumentoFiscal, def: false });
    this.DocumentoFiscal.val.subscribe(desabilitarBoxDocumentoFiscal);
    this.DocumentoFiscalTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.DocumentoFiscalTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.DocumentoFiscalDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.DocumentoFiscalBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeDocumentoFiscalClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(true), permiteBaixarQRCode: false });
    this.DocumentoFiscalHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.DocumentoFiscalVincularNotaFiscal = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.VincularNotaFiscal, val: ko.observable(false), def: false });
    this.DocumentoFiscalCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.DocumentoFiscalGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.DocumentosTransporte = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.DocumentosTransporte), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.DocumentosTransporte, def: false });
    this.DocumentosTransporte.val.subscribe(desabilitarBoxDocumentosTransporte);
    this.DocumentosTransporteTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.DocumentosTransporteTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.DocumentosTransporteDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.DocumentosTransporteBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeDocumentosTransporteClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(true), permiteBaixarQRCode: false });
    this.DocumentosTransporteHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.DocumentosTransporteCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.DocumentosTransporteGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.InicioDescarregamento = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.InicioDescarregamento), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.InicioDescarregamento, def: false });
    this.InicioDescarregamento.val.subscribe(desabilitarBoxInicioDescarregamento);
    this.InicioDescarregamentoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.InicioDescarregamentoTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.InicioDescarregamentoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.InicioDescarregamentoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeInicioDescarregamentoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.InicioDescarregamentoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.InicioDescarregamentoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.InicioDescarregamentoPermiteInformarPesagem = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteInformarPesagem, val: ko.observable(false), def: false });
    this.InicioDescarregamentoGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.FimDescarregamento = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.FimDescarregamento), textDefault: ko.observable(""), val: ko.observable(false), enum: "Etapa_" + EnumEtapaFluxoGestaoPatio.FimDescarregamento, def: false });
    this.FimDescarregamento.val.subscribe(desabilitarBoxFimDescarregamento);
    this.FimDescarregamentoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.FimDescarregamentoTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.FimDescarregamentoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.FimDescarregamentoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeFimDescarregamentoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(true), permiteBaixarQRCode: false });
    this.FimDescarregamentoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.FimDescarregamentoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.FimDescarregamentoPermiteInformarPesagem = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteInformarPesagem, val: ko.observable(false), def: false });
    this.FimDescarregamentoGerarIntegracaoP44 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.IntegracaoP44, val: ko.observable(false), def: false, visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGestaoPatio() {
    _gestaoPatio = new GestaoPatio();
    KoBindings(_gestaoPatio, "knockoutGestaoPatio");

    loadGestaoPatioBoxOrdenavel();
    loadGestaoPatioConfiguracaoEtapas();
    limparCamposGestaoPatio();

    verificaVisibilidadeIntegracaoP44();

    new BuscarFilialBalanca(_gestaoPatio.BalancaGuaritaEntrada, null, null, _filial.Codigo);
    new BuscarFilialBalanca(_gestaoPatio.BalancaGuaritaSaida, null, null, _filial.Codigo);
    new BuscarTiposOperacao(_gestaoPatio.CheckListTipoOperacao);
    new BuscarClientes(_gestaoPatio.CheckListDestinatario);

    $('a[href="#tabGestaoPatioConfiguracoes"]').on('shown.bs.tab', ControlarAlturarBoxes);
    $('a[href="#knockoutGestaoPatio"]').on('shown.bs.tab', ControlarAlturarBoxes);
}

function loadGestaoPatioBoxOrdenavel() {
    $boxesSequenciaPatio.sortable({
        forcePlaceholderSize: true,
        items: '> .box',
        helper: "clone",
        placeholder: 'col col-sm-12 col-md-4 box',
        start: function (evt, ui) {
            var $placeHolder = $("<div></div>", { class: "box-placeholder" });
            ui.placeholder.html($placeHolder);
        }
    });
}

function loadGestaoPatioConfiguracaoEtapas() {
    executarReST("Filial/ConfiguracaoGestaoPatio", { Tipo: EnumTipoFluxoGestaoPatio.Origem }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                var etapas = arg.Data.Etapas;

                for (var i in etapas) {
                    switch (etapas[i].Etapa) {
                        case EnumEtapaFluxoGestaoPatio.InformarDoca:
                            _gestaoPatio.InformarDocaCarregamento.text(etapas[i].Descricao);
                            _gestaoPatio.InformarDocaCarregamento.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.InformarDocaCarregamentoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.ChegadaVeiculo:
                            _gestaoPatio.ChegadaVeiculo.text(etapas[i].Descricao);
                            _gestaoPatio.ChegadaVeiculo.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.ChegadaVeiculoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.Guarita:
                            _gestaoPatio.GuaritaEntrada.text(etapas[i].Descricao);
                            _gestaoPatio.GuaritaEntrada.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.GuaritaEntradaBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.CheckList:
                            _gestaoPatio.CheckList.text(etapas[i].Descricao);
                            _gestaoPatio.CheckList.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.CheckListBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            _gestaoPatio.CheckListPermiteSalvarSemPreencher.visible(!etapas[i].CheckListPermiteSalvarSemPreencher);
                            break;
                        case EnumEtapaFluxoGestaoPatio.TravamentoChave:
                            _gestaoPatio.TravaChave.text(etapas[i].Descricao);
                            _gestaoPatio.TravaChave.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.TravaChaveBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.Expedicao:
                            _gestaoPatio.Expedicao.text(etapas[i].Descricao);
                            _gestaoPatio.Expedicao.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.ExpedicaoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.LiberacaoChave:
                            _gestaoPatio.LiberaChave.text(etapas[i].Descricao);
                            _gestaoPatio.LiberaChave.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.LiberaChaveBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.Faturamento:
                            _gestaoPatio.Faturamento.text(etapas[i].Descricao);
                            _gestaoPatio.Faturamento.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.FaturamentoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.InicioViagem:
                            _gestaoPatio.GuaritaSaida.text(etapas[i].Descricao);
                            _gestaoPatio.GuaritaSaida.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.GuaritaSaidaBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.Posicao:
                            _gestaoPatio.Posicao.text(etapas[i].Descricao);
                            _gestaoPatio.Posicao.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.PosicaoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.ChegadaLoja:
                            _gestaoPatio.ChegadaLoja.text(etapas[i].Descricao);
                            _gestaoPatio.ChegadaLoja.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.ChegadaLojaBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.DeslocamentoPatio:
                            _gestaoPatio.DeslocamentoPatio.text(etapas[i].Descricao);
                            _gestaoPatio.DeslocamentoPatio.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.DeslocamentoPatioBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.SaidaLoja:
                            _gestaoPatio.SaidaLoja.text(etapas[i].Descricao);
                            _gestaoPatio.SaidaLoja.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.SaidaLojaBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.FimViagem:
                            _gestaoPatio.FimViagem.text(etapas[i].Descricao);
                            _gestaoPatio.FimViagem.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.FimViagemBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.InicioHigienizacao:
                            _gestaoPatio.InicioHigienizacao.text(etapas[i].Descricao);
                            _gestaoPatio.InicioHigienizacao.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.InicioHigienizacaoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.FimHigienizacao:
                            _gestaoPatio.FimHigienizacao.text(etapas[i].Descricao);
                            _gestaoPatio.FimHigienizacao.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.FimHigienizacaoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.InicioCarregamento:
                            _gestaoPatio.InicioCarregamento.text(etapas[i].Descricao);
                            _gestaoPatio.InicioCarregamento.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.InicioCarregamentoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.FimCarregamento:
                            _gestaoPatio.FimCarregamento.text(etapas[i].Descricao);
                            _gestaoPatio.FimCarregamento.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.FimCarregamentoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.SeparacaoMercadoria:
                            _gestaoPatio.SeparacaoMercadoria.text(etapas[i].Descricao);
                            _gestaoPatio.SeparacaoMercadoria.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.SeparacaoMercadoriaBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.SolicitacaoVeiculo:
                            _gestaoPatio.SolicitacaoVeiculo.text(etapas[i].Descricao);
                            _gestaoPatio.SolicitacaoVeiculo.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.SolicitacaoVeiculoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.DocumentoFiscal:
                            _gestaoPatio.DocumentoFiscal.text(etapas[i].Descricao);
                            _gestaoPatio.DocumentoFiscal.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.DocumentoFiscalBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.DocumentosTransporte:
                            _gestaoPatio.DocumentosTransporte.text(etapas[i].Descricao);
                            _gestaoPatio.DocumentosTransporte.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.DocumentosTransporteBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.MontagemCarga:
                            _gestaoPatio.MontagemCarga.text(etapas[i].Descricao);
                            _gestaoPatio.MontagemCarga.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.MontagemCargaBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.InicioDescarregamento:
                            _gestaoPatio.InicioDescarregamento.text(etapas[i].Descricao);
                            _gestaoPatio.InicioDescarregamento.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.InicioDescarregamentoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.FimDescarregamento:
                            _gestaoPatio.FimDescarregamento.text(etapas[i].Descricao);
                            _gestaoPatio.FimDescarregamento.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatio.FimDescarregamentoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                    }
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function ControlarAlturarBoxes() {
    $styleAltura.html("");

    var maiorAltura = 0;

    $boxesSequenciaPatio.find(".box .card").each(function () {
        var $el = $(this);
        var height = $el.height();

        maiorAltura = height > maiorAltura ? height : maiorAltura;
    });

    var style = $templateAltura.html();
    style = style.replace(/{{altura}}/, maiorAltura);
    style = style.replace(/{{alturaplaceholder}}/, maiorAltura + 80);

    $styleAltura.html(style);
}

function baixarQRCodeCheckListClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.CheckList);
}

function baixarQRCodeChegadaLojaClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.ChegadaLoja);
}

function baixarQRCodeChegadaVeiculoClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.ChegadaVeiculo);
}

function baixarQRCodeDeslocamentoPatioClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.DeslocamentoPatio);
}

function baixarQRCodeDocumentoFiscalClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.DocumentoFiscal);
}

function baixarQRCodeDocumentosTransporteClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.DocumentosTransporte);
}

function baixarQRCodeExpedicaoClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.Expedicao);
}

function baixarQRCodeFaturamentoClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.Faturamento);
}

function baixarQRCodeFimCarregamentoClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.FimCarregamento);
}

function baixarQRCodeFimDescarregamentoClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.FimDescarregamento);
}

function baixarQRCodeFimHigienizacaoClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.FimHigienizacao);
}

function baixarQRCodeFimViagemClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.FimViagem);
}

function baixarQRCodeGuaritaEntradaClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.Guarita);
}

function baixarQRCodeGuaritaSaidaClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.InicioViagem);
}

function baixarQRCodeInformarDocaCarregamentoClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.InformarDoca);
}

function baixarQRCodeInicioCarregamentoClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.InicioCarregamento);
}

function baixarQRCodeInicioDescarregamentoClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.InicioDescarregamento);
}

function baixarQRCodeInicioHigienizacaoClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.InicioHigienizacao);
}

function baixarQRCodeLiberaChaveClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.LiberacaoChave);
}

function baixarQRCodeMontagemCargaClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.MontagemCarga);
}

function baixarQRCodePosicaoClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.Posicao);
}

function baixarQRCodeSaidaLojaClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.SaidaLoja);
}

function baixarQRCodeSeparacaoMercadoriaClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.SeparacaoMercadoria);
}

function baixarQRCodeSolicitacaoVeiculoClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.SolicitacaoVeiculo);
}

function baixarQRCodeTravaChaveClick() {
    baixarQRCode(EnumEtapaFluxoGestaoPatio.TravamentoChave);
}

/*
 * Declaração das Funções Públicas
 */

function GetSetGestaoPatio(data) {
    if (arguments.length > 0)
        SetGestaoPatio(data);
    else
        return GetGestaoPatio();
}

function GetSetOrdemGestaoPatio(data) {
    if (arguments.length > 0)
        SetaOrdem(data);
    else
        return OrdemGestaoPatio();
}

function limparCamposGestaoPatio() {
    LimparCampos(_gestaoPatio);
    _gestaoPatio.TipoCheckListImpressao.val(0);
    controlarExibicaoBotoesBaixarQRCode();
}

/*
 * Declaração das Funções Privadas
 */

function baixarQRCode(etapaFluxoGestaoPatio) {
    if (isCadastroEmEdicao())
        executarDownload("Filial/BaixarQrCodeEtapa", { Filial: _filial.Codigo.val(), Etapa: etapaFluxoGestaoPatio, Tipo: EnumTipoFluxoGestaoPatio.Origem });
}

function controlarExibicaoBotaoBaixarQRCode(knout, exibir) {
    if (exibir)
        knout.visible(knout.permiteBaixarQRCode);
    else
        knout.visible(false);
}

function controlarExibicaoBotoesBaixarQRCode() {
    var isEdicao = isCadastroEmEdicao();

    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.CheckListBaixarQRCode, (isEdicao && _gestaoPatio.CheckList.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.ChegadaLojaBaixarQRCode, (isEdicao && _gestaoPatio.ChegadaLoja.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.ChegadaVeiculoBaixarQRCode, (isEdicao && _gestaoPatio.ChegadaVeiculo.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.DeslocamentoPatioBaixarQRCode, (isEdicao && _gestaoPatio.DeslocamentoPatio.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.DocumentoFiscalBaixarQRCode, (isEdicao && _gestaoPatio.DocumentoFiscal.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.DocumentosTransporteBaixarQRCode, (isEdicao && _gestaoPatio.DocumentosTransporte.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.ExpedicaoBaixarQRCode, (isEdicao && _gestaoPatio.Expedicao.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.FaturamentoBaixarQRCode, (isEdicao && _gestaoPatio.Faturamento.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.FimCarregamentoBaixarQRCode, (isEdicao && _gestaoPatio.FimCarregamento.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.FimDescarregamentoBaixarQRCode, (isEdicao && _gestaoPatio.FimDescarregamento.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.FimHigienizacaoBaixarQRCode, (isEdicao && _gestaoPatio.FimHigienizacao.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.FimViagemBaixarQRCode, (isEdicao && _gestaoPatio.FimViagem.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.GuaritaEntradaBaixarQRCode, (isEdicao && _gestaoPatio.GuaritaEntrada.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.GuaritaSaidaBaixarQRCode, (isEdicao && _gestaoPatio.GuaritaSaida.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.InformarDocaCarregamentoBaixarQRCode, (isEdicao && _gestaoPatio.InformarDocaCarregamento.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.InicioCarregamentoBaixarQRCode, (isEdicao && _gestaoPatio.InicioCarregamento.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.InicioDescarregamentoBaixarQRCode, (isEdicao && _gestaoPatio.InicioDescarregamento.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.InicioHigienizacaoBaixarQRCode, (isEdicao && _gestaoPatio.InicioHigienizacao.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.LiberaChaveBaixarQRCode, (isEdicao && _gestaoPatio.LiberaChave.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.MontagemCargaBaixarQRCode, (isEdicao && _gestaoPatio.MontagemCarga.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.PosicaoBaixarQRCode, (isEdicao && _gestaoPatio.Posicao.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.SaidaLojaBaixarQRCode, (isEdicao && _gestaoPatio.SaidaLoja.val()));
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.TravaChaveBaixarQRCode, (isEdicao && _gestaoPatio.TravaChave.val()));
}

function desabilitarBoxCheckList(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.CheckListBaixarQRCode, (habilitar && isCadastroEmEdicao()));

    if (!habilitar) {
        _gestaoPatio.CheckListInformarDoca.val(false);
    }
}

function desabilitarBoxChegadaLoja(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.ChegadaLojaBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxChegadaVeiculo(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.ChegadaVeiculoBaixarQRCode, (habilitar && isCadastroEmEdicao()));

    if (!habilitar)
        _gestaoPatio.ChegadaVeiculo.val(false);
}

function desabilitarBoxDeslocamentoPatio(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.DeslocamentoPatioBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxDocumentoFiscal(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.DocumentoFiscalBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxDocumentosTransporte(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.DocumentosTransporteBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxExpedicao(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.ExpedicaoBaixarQRCode, (habilitar && isCadastroEmEdicao()));

    if (!habilitar) {
        _gestaoPatio.ExpedicaoInformarDoca.val(false);
        _gestaoPatio.ExpedicaoInformarInicioCarregamento.val(false);
        _gestaoPatio.ExpedicaoInformarTerminoCarregamento.val(false);
        _gestaoPatio.ExpedicaoConfirmarPlaca.val(false);
    }
}

function desabilitarBoxFaturamento(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.FaturamentoBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxFimCarregamento(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.FimCarregamentoBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxFimDescarregamento(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.FimDescarregamentoBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxFimHigienizacao(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.FimHigienizacaoBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxFimViagem(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.FimViagemBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxGuaritaEntrada(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.GuaritaEntradaBaixarQRCode, (habilitar && isCadastroEmEdicao()));

    if (!habilitar) {
        _gestaoPatio.GuaritaEntradaInformarDoca.val(false);
        _gestaoPatio.GuaritaEntradaExibirHorarioExato.val(true);
    }
}

function desabilitarBoxGuaritaSaida(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.GuaritaSaidaBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxInformarDocaCarregamento(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.InformarDocaCarregamentoBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxInicioCarregamento(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.InicioCarregamentoBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxInicioDescarregamento(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.InicioDescarregamentoBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxInicioHigienizacao(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.InicioHigienizacaoBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxLiberaChave(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.LiberaChaveBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxMontagemCarga(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.MontagemCargaBaixarQRCode, (habilitar && isCadastroEmEdicao()));

    if (!habilitar)
        _gestaoPatio.MontagemCargaInformarDoca.val(false);
}

function desabilitarBoxPosicao(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.PosicaoBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxSaidaLoja(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.SaidaLojaBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxSeparacaoMercadoria(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.SeparacaoMercadoriaBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxSolicitacaoVeiculo(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.SolicitacaoVeiculoBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxTravaChave(habilitar) {
    controlarExibicaoBotaoBaixarQRCode(_gestaoPatio.TravaChaveBaixarQRCode, (habilitar && isCadastroEmEdicao()));

    if (!habilitar)
        _gestaoPatio.TravaChaveInformarDoca.val(false);
}

function GetGestaoPatio() {
    var json = RetornarObjetoPesquisa(_gestaoPatio);

    return JSON.stringify(json);
}

function OrdemGestaoPatio() {
    var elementos = $boxesSequenciaPatio.sortable("toArray");
    var ordem = elementos.map(function (etapa, index) {
        var id = etapa.split("_");
        return {
            Etapa: id[1],
            Ordem: index
        }
    });

    return JSON.stringify(ordem);
}

function SetaOrdem(ordens) {
    if (ordens == null) return;

    var DOMContainer = $boxesSequenciaPatio[0];

    if (DOMContainer != undefined) {
        for (var i = ordens.length - 1; i > 0; i--) {
            var nodeReference = document.getElementById("Etapa_" + ordens[i].Etapa);
            var newNode = document.getElementById("Etapa_" + ordens[i - 1].Etapa);
            DOMContainer.insertBefore(newNode, nodeReference);
        }
    }
}

function SetGestaoPatio(data) {
    if (data != null) {
        PreencherObjetoKnout(_gestaoPatio, { Data: data });

        controlarExibicaoBotoesBaixarQRCode();
    }
}

function preencherGestaoPatio(data) {
    console.log(data)
}

function verificaVisibilidadeIntegracaoP44() {
    _filial.GerarIntegracaoP44.val.subscribe(function (val) {
        _gestaoPatio.InformarDocaCarregamentoGerarIntegracaoP44.visible(val);
        _gestaoPatio.MontagemCargaGerarIntegracaoP44.visible(val);
        _gestaoPatio.ChegadaVeiculoGerarIntegracaoP44.visible(val);
        _gestaoPatio.GuaritaEntradaGerarIntegracaoP44.visible(val);
        _gestaoPatio.CheckListGerarIntegracaoP44.visible(val);
        _gestaoPatio.TravaChaveGerarIntegracaoP44.visible(val);
        _gestaoPatio.ExpedicaoGerarIntegracaoP44.visible(val);
        _gestaoPatio.LiberaChaveGerarIntegracaoP44.visible(val);
        _gestaoPatio.FaturamentoGerarIntegracaoP44.visible(val);
        _gestaoPatio.GuaritaSaidaGerarIntegracaoP44.visible(val);
        _gestaoPatio.PosicaoGerarIntegracaoP44.visible(val);
        _gestaoPatio.ChegadaLojaGerarIntegracaoP44.visible(val);
        _gestaoPatio.DeslocamentoPatioGerarIntegracaoP44.visible(val);
        _gestaoPatio.SaidaLojaGerarIntegracaoP44.visible(val);
        _gestaoPatio.FimViagemGerarIntegracaoP44.visible(val);
        _gestaoPatio.InicioHigienizacaoGerarIntegracaoP44.visible(val);
        _gestaoPatio.FimHigienizacaoGerarIntegracaoP44.visible(val);
        _gestaoPatio.InicioCarregamentoGerarIntegracaoP44.visible(val);
        _gestaoPatio.FimCarregamentoGerarIntegracaoP44.visible(val);
        _gestaoPatio.SeparacaoMercadoriaGerarIntegracaoP44.visible(val);
        _gestaoPatio.SolicitacaoVeiculoGerarIntegracaoP44.visible(val);
        _gestaoPatio.DocumentoFiscalGerarIntegracaoP44.visible(val);
        _gestaoPatio.DocumentosTransporteGerarIntegracaoP44.visible(val);
        _gestaoPatio.InicioDescarregamentoGerarIntegracaoP44.visible(val);
        _gestaoPatio.FimDescarregamentoGerarIntegracaoP44.visible(val);
    });
}
