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

var _gestaoPatioDestino;
var $boxesSequenciaPatioDestino = $("#knockoutGestaoPatioDestino .boxes-sequencia-patio");
var $templateAlturaDestino = $("#style-altura-template-destino");
var $styleAlturaDestino = $("#style-altura-destino");
var _opcoesChecklistImpressaoDestino = [
    { value: 0, text: "Padrão" }
];

/*
 * Declaração das Classes
 */

var GestaoPatioDestino = function () {
    var configIntTempo = { precision: 0, allowZero: true };

    this.InformarDocaCarregamento = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.InformarDocaCarregamento), textDefault: ko.observable(""), val: ko.observable(false), enum: "EtapaDestino_" + EnumEtapaFluxoGestaoPatio.InformarDoca, def: false });
    this.InformarDocaCarregamento.val.subscribe(desabilitarBoxInformarDocaCarregamentoDestino);
    this.InformarDocaCarregamentoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.InformarDocaCarregamentoTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.InformarDocaCarregamentoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.InformarDocaCarregamentoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeInformarDocaCarregamentoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.InformarDocaCarregamentoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.InformarDocaCarregamentoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.InformarDocaCarregamentoPermiteTransportadorLancarHorarios = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteTransportadorLancarHorariosPatio, val: ko.observable(false), def: false });
    this.InformarDocaCarregamentoPermiteInformarDadosLaudo = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteInformarDadosLaudo, val: ko.observable(false), def: false });

    this.ChegadaVeiculo = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.ChegadaVeiculo), textDefault: ko.observable(""), val: ko.observable(false), enum: "EtapaDestino_" + EnumEtapaFluxoGestaoPatio.ChegadaVeiculo, def: false });
    this.ChegadaVeiculo.val.subscribe(desabilitarBoxChegadaVeiculoDestino);
    this.ChegadaVeiculoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoChegada.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.ChegadaVeiculoTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.ChegadaVeiculoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.ChegadaVeiculoPermiteImprimirRelacaoDeProdutos = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteImprimirRelacaoProdutos, val: ko.observable(false), def: false });
    this.ChegadaVeiculoPreencherDataSaida = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PreencherDataSaida, val: ko.observable(false), def: false });
    this.ChegadaVeiculoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeChegadaVeiculoDestinoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.ChegadaVeiculoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.ChegadaVeiculoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });

    this.GuaritaEntrada = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.GuaritaEntrada), textDefault: ko.observable(""), val: ko.observable(false), enum: "EtapaDestino_" + EnumEtapaFluxoGestaoPatio.Guarita, def: false });
    this.GuaritaEntrada.val.subscribe(desabilitarBoxGuaritaEntradaDestino);
    this.GuaritaEntradaInformarDoca = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.InformarDoca, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.GuaritaEntradaPermiteInformacoesPesagem = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformacoesPesagem, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaEntradaPermiteInformacoesProdutor = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformacoesProdutor, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaEntradaPermiteInformarAnexoPesagem = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformarAnexosPesagem, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaEntradaPermiteInformarPressaoPesagem = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformarPressaoPesagem, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaEntradaPermiteInformarQuantidadeCaixasPesagem = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformarQuantidadeCaixasPesagem, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaEntradaExibirHorarioExato = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.ExibirHorarioExato, val: ko.observable(true), def: true, visible: false });
    this.GuaritaEntradaTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.GuaritaEntradaTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.GuaritaEntradaDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.GuaritaEntradaHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.GuaritaEntradaCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.GuaritaEntradaBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeGuaritaEntradaDestinoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.GuaritaEntradaPermiteInformarDadosDevolucao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteInformarDadosDevolucao, val: ko.observable(false), def: false });
    this.BalancaGuaritaEntrada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Filiais.Filial.Balanca.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });
    this.GuaritaEntradaTipoIntegracaoBalanca = PropertyEntity({ val: ko.observable(""), def: "", options: EnumTipoIntegracao.obterOpcoesIntegracaoBalancaGestaoPatio(), text: Localization.Resources.Gerais.Geral.Integracao.getFieldDescription(), visible: ko.observable(false) });

    this.CheckList = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.Checklist), textDefault: ko.observable(""), val: ko.observable(false), enum: "EtapaDestino_" + EnumEtapaFluxoGestaoPatio.CheckList, def: false });
    this.CheckList.val.subscribe(desabilitarBoxCheckListDestino);
    this.CheckListInformarDoca = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.InformarDoca, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.CheckListTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.CheckListTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.CheckListDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.CheckListBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeCheckListDestinoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.CheckListPermiteSalvarSemPreencher = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteSalvarSemPreencher, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.TipoCheckListImpressao = PropertyEntity({ text: Localization.Resources.Filiais.Filial.TipoChecklist.getFieldDescription(), val: ko.observable(), options: ko.observable(), visible: ko.observable(false), enable: ko.observable(false) });
    this.CheckListHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.CheckListCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.CheckListPermiteImpressaoApenasComCheckListFinalizada = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteImpressaoApenasChecklistFinalizada, val: ko.observable(false), def: false });
    this.CheckListCancelarPatioAoReprovar = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.CancelarPatioReprovarChecklist, val: ko.observable(false), def: false });
    this.CheckListNotificarPorEmailReprovacao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.NotificarEmailReprovacaoChecklist, val: ko.observable(false), def: false });
    this.CheckListNaoExigeObservacaoAoReprovar = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.NaoExigirObservacaoReprovar, val: ko.observable(false), def: false });
    this.CheckListPermitePreencherCheckListAntesDeChegarNaEtapa = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermitePreencherChecklistAntesChegarEtapa, val: ko.observable(false), def: false });
    this.CheckListEmails = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.EmailsSeparadosPontoEVirgula.getFieldDescription(), val: ko.observable(""), maxlength: 300 });

    this.TravaChave = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.TravaChave), textDefault: ko.observable(""), val: ko.observable(false), enum: "EtapaDestino_" + EnumEtapaFluxoGestaoPatio.TravamentoChave, def: false });
    this.TravaChave.val.subscribe(desabilitarBoxTravaChaveDestino);
    this.TravaChaveInformarDoca = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.InformarDoca, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.TravaChaveTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.TravaChaveTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.TravaChaveDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.TravaChaveBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeTravaChaveDestinoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.TravaChaveHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.TravaChaveCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });

    this.LiberaChave = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.LiberacaoChave), textDefault: ko.observable(""), val: ko.observable(false), enum: "EtapaDestino_" + EnumEtapaFluxoGestaoPatio.LiberacaoChave, def: false });
    this.LiberaChave.val.subscribe(desabilitarBoxLiberaChaveDestino);
    this.LiberaChaveTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.LiberaChaveTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.LiberaChaveDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.LiberaChaveBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeLiberaChaveDestinoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.LiberaChaveHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.LiberaChaveCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.LiberaChaveSolicitarAssinaturaMotorista = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.SolicitarAssinaturaMotorista, val: ko.observable(false), def: false });

    this.Faturamento = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.ControlarFaturamento), textDefault: ko.observable(""), val: ko.observable(false), enum: "EtapaDestino_" + EnumEtapaFluxoGestaoPatio.Faturamento, def: false });
    this.Faturamento.val.subscribe(desabilitarBoxFaturamentoDestino);
    this.FaturamentoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.FaturamentoTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.FaturamentoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.FaturamentoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeFaturamentoDestinoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.FaturamentoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.FaturamentoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });

    this.GuaritaSaida = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.GuaritaSaida), textDefault: ko.observable(""), val: ko.observable(false), enum: "EtapaDestino_" + EnumEtapaFluxoGestaoPatio.InicioViagem, def: false });
    this.GuaritaSaida.val.subscribe(desabilitarBoxGuaritaSaidaDestino);
    this.GuaritaSaidaPermiteInformacoesPesagem = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformacoesPesagem, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaSaidaPermiteInformarLacrePesagem = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformarLacrePesagem, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaSaidaPermiteInformarPercentualRefugoPesagem = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformarPercentualRefugoPesagem, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaSaidaPermiteAnexosPesagem = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteAnexosPesagemFinal, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaSaidaIniciarEmissaoDocumentosTransporte = PropertyEntity({ text: Localization.Resources.Filiais.Filial.GuaritaSaidaIniciarEmissaoDocumentosTransporte, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.GuaritaSaidaTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.GuaritaSaidaDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.GuaritaSaidaBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeGuaritaSaidaDestinoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.GuaritaSaidaHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.GuaritaSaidaCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.BalancaGuaritaSaida = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Filiais.Filial.Balanca.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });
    this.GuaritaSaidaTipoIntegracaoBalanca = PropertyEntity({ val: ko.observable(""), def: "", options: EnumTipoIntegracao.obterOpcoesIntegracaoBalancaGestaoPatio(), text: Localization.Resources.Gerais.Geral.Integracao.getFieldDescription(), visible: ko.observable(false) });

    this.ChegadaLoja = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.ChegadaDestinatario), textDefault: ko.observable(""), val: ko.observable(false), enum: "EtapaDestino_" + EnumEtapaFluxoGestaoPatio.ChegadaLoja, def: false });
    this.ChegadaLoja.val.subscribe(desabilitarBoxChegadaLojaDestino);
    this.ChegadaLojaTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.ChegadaLojaTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.ChegadaLojaDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.ChegadaLojaBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeChegadaLojaDestinoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.ChegadaLojaHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.ChegadaLojaCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });

    this.DeslocamentoPatio = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.DeslocamentoPatio), textDefault: ko.observable(""), val: ko.observable(false), enum: "EtapaDestino_" + EnumEtapaFluxoGestaoPatio.DeslocamentoPatio, def: false });
    this.DeslocamentoPatio.val.subscribe(desabilitarBoxDeslocamentoPatioDestino);
    this.DeslocamentoPatioPermiteInformacoesPesagem = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformacoesPesagem, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.DeslocamentoPatioPermiteInformarQuantidade = PropertyEntity({ text: Localization.Resources.Filiais.Filial.DeslocamentoPatioPermiteInformarQuantidade, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.DeslocamentoPatioPermiteInformacoesLoteInterno = PropertyEntity({ text: Localization.Resources.Filiais.Filial.PermiteInformacoesLoteInterno, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.DeslocamentoPatioTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.DeslocamentoPatioTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.DeslocamentoPatioDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.DeslocamentoPatioBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeDeslocamentoPatioDestinoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.DeslocamentoPatioHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.DeslocamentoPatioCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });

    this.SaidaLoja = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.SaidaDestinatario), textDefault: ko.observable(""), val: ko.observable(false), enum: "EtapaDestino_" + EnumEtapaFluxoGestaoPatio.SaidaLoja, def: false });
    this.SaidaLoja.val.subscribe(desabilitarBoxSaidaLojaDestino);
    this.SaidaLojaTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.SaidaLojaTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.SaidaLojaDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.SaidaLojaBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeSaidaLojaDestinoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.SaidaLojaHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.SaidaLojaCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });

    this.FimViagem = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.FimViagem), textDefault: ko.observable(""), val: ko.observable(false), enum: "EtapaDestino_" + EnumEtapaFluxoGestaoPatio.FimViagem, def: false });
    this.FimViagem.val.subscribe(desabilitarBoxFimViagemDestino);
    this.FimViagemTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.FimViagemTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.FimViagemDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.FimViagemBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeFimViagemDestinoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.FimViagemHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.FimViagemCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });

    this.InicioHigienizacao = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.InicioHigienizacao), textDefault: ko.observable(""), val: ko.observable(false), enum: "EtapaDestino_" + EnumEtapaFluxoGestaoPatio.InicioHigienizacao, def: false });
    this.InicioHigienizacao.val.subscribe(desabilitarBoxInicioHigienizacaoDestino);
    this.InicioHigienizacaoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.InicioHigienizacaoTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.InicioHigienizacaoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.InicioHigienizacaoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeInicioHigienizacaoDestinoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.InicioHigienizacaoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.InicioHigienizacaoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });

    this.FimHigienizacao = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.FimHigienizacao), textDefault: ko.observable(""), val: ko.observable(false), enum: "EtapaDestino_" + EnumEtapaFluxoGestaoPatio.FimHigienizacao, def: false });
    this.FimHigienizacao.val.subscribe(desabilitarBoxFimHigienizacaoDestino);
    this.FimHigienizacaoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.FimHigienizacaoTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.FimHigienizacaoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.FimHigienizacaoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeFimHigienizacaoDestinoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(true), permiteBaixarQRCode: false });
    this.FimHigienizacaoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.FimHigienizacaoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });

    this.SolicitacaoVeiculo = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.SolicitacaoVeiculo), textDefault: ko.observable(""), val: ko.observable(false), enum: "EtapaDestino_" + EnumEtapaFluxoGestaoPatio.SolicitacaoVeiculo, def: false });
    this.SolicitacaoVeiculo.val.subscribe(desabilitarBoxSolicitacaoVeiculoDestino);
    this.SolicitacaoVeiculoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.SolicitacaoVeiculoTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.SolicitacaoVeiculoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.SolicitacaoVeiculoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeSolicitacaoVeiculoDestinoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.SolicitacaoVeiculoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.SolicitacaoVeiculoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });

    this.InicioDescarregamento = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.InicioDescarregamento), textDefault: ko.observable(""), val: ko.observable(false), enum: "EtapaDestino_" + EnumEtapaFluxoGestaoPatio.InicioDescarregamento, def: false });
    this.InicioDescarregamento.val.subscribe(desabilitarBoxInicioDescarregamentoDestino);
    this.InicioDescarregamentoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.InicioDescarregamentoTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.InicioDescarregamentoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.InicioDescarregamentoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeInicioDescarregamentoDestinoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.InicioDescarregamentoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.InicioDescarregamentoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.InicioDescarregamentoPermiteInformarPesagem = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteInformarPesagem, val: ko.observable(false), def: false });

    this.FimDescarregamento = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.FimDescarregamento), textDefault: ko.observable(""), val: ko.observable(false), enum: "EtapaDestino_" + EnumEtapaFluxoGestaoPatio.FimDescarregamento, def: false });
    this.FimDescarregamento.val.subscribe(desabilitarBoxFimDescarregamentoDestino);
    this.FimDescarregamentoTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.FimDescarregamentoTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.FimDescarregamentoDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.FimDescarregamentoBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeFimDescarregamentoDestinoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(true), permiteBaixarQRCode: false });
    this.FimDescarregamentoHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.FimDescarregamentoCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.FimDescarregamentoPermiteInformarPesagem = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteInformarPesagem, val: ko.observable(false), def: false });

    this.AvaliacaoDescarga = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Filiais.Filial.AvaliacaoDescarga), textDefault: ko.observable(""), val: ko.observable(false), enum: "EtapaDestino_" + EnumEtapaFluxoGestaoPatio.AvaliacaoDescarga, def: false });
    this.AvaliacaoDescarga.val.subscribe(desabilitarBoxAvaliacaoDestino);
    this.AvaliacaoDescargaInformarDoca = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.InformarDoca, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.AvaliacaoDescargaTempo = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoEtapa.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.AvaliacaoDescargaTempoPermanencia = PropertyEntity({ configInt: configIntTempo, getType: typesKnockout.int, text: Localization.Resources.Filiais.Filial.TempoMaximoPermanencia.getFieldDescription(), val: ko.observable("0"), def: "0" });
    this.AvaliacaoDescargaDescricao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.DescricaoEtapa.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.AvaliacaoDescargaBaixarQRCode = PropertyEntity({ eventClick: baixarQRCodeAvaliacaoDescargaDestinoClick, type: types.event, text: Localization.Resources.Filiais.Filial.BaixarQRCode, visible: ko.observable(false), permiteBaixarQRCode: false });
    this.AvaliacaoDescargaPermiteSalvarSemPreencher = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteSalvarSemPreencher, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.AvaliacaoDescargaHabilitarIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarIntegracao, val: ko.observable(false), def: false });
    this.AvaliacaoDescargaCodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), val: ko.observable("") });
    this.AvaliacaoDescargaPermiteImpressaoApenasComAvaliacaoDescargaFinalizada = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermiteImpressaoApenasChecklistFinalizada, val: ko.observable(false), def: false });
    this.AvaliacaoDescargaCancelarPatioAoReprovar = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.CancelarPatioReprovarChecklist, val: ko.observable(false), def: false });
    this.AvaliacaoDescargaNotificarPorEmailReprovacao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.NotificarViaEmailReprovacaoDaAvaliacaoDeDescarga, val: ko.observable(false), def: false });
    this.AvaliacaoDescargaNaoExigeObservacaoAoReprovar = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.NaoExigirObservacaoReprovar, val: ko.observable(false), def: false });
    this.AvaliacaoDescargaPermitePreencherAvaliacaoDescargaAntesDeChegarNaEtapa = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.PermitePreencherAvaliacaoDeDescargaAntesDeChegarNaEtapa, val: ko.observable(false), def: false });
    this.AvaliacaoDescargaEmails = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Filiais.Filial.EmailsSeparadosPontoEVirgula.getFieldDescription(), val: ko.observable(""), maxlength: 300 });
    this.AvaliacaoDescargaAssinaturaMotorista = PropertyEntity({ text: Localization.Resources.Filiais.Filial.CheckListAssinaturaMotorista, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AvaliacaoDescargaAssinaturaCarregador = PropertyEntity({ text: Localization.Resources.Filiais.Filial.CheckListAssinaturaCarregador, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AvaliacaoDescargaAssinaturaResponsavelAprovacao = PropertyEntity({ text: Localization.Resources.Filiais.Filial.CheckListAssinaturaResponsavelAprovacao, getType: typesKnockout.bool, val: ko.observable(false), def: false });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGestaoPatioDestino() {
    _gestaoPatioDestino = new GestaoPatioDestino();
    KoBindings(_gestaoPatioDestino, "knockoutGestaoPatioDestino");

    loadGestaoPatioDestinoBoxOrdenavel();
    loadGestaoPatioDestinoConfiguracaoEtapas();
    limparCamposGestaoPatioDestino();

    new BuscarFilialBalanca(_gestaoPatioDestino.BalancaGuaritaEntrada, null, null, null, _sequenciaGestaoPatio.Filial);
    new BuscarFilialBalanca(_gestaoPatioDestino.BalancaGuaritaSaida, null, null, null, _sequenciaGestaoPatio.Filial);

    $('a[href="#knockoutGestaoPatioDestino"]').on('shown.bs.tab', ControlarAlturarBoxesDestino);

    if (_CONFIGURACAO_TMS.GerarFluxoPatioDestino)
        $("#liGestaoPatioDestino").show();
}

function loadGestaoPatioDestinoBoxOrdenavel() {
    $boxesSequenciaPatioDestino.sortable({
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

function loadGestaoPatioDestinoConfiguracaoEtapas() {
    executarReST("Filial/ConfiguracaoGestaoPatio", { Tipo: EnumTipoFluxoGestaoPatio.Destino }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                var etapas = arg.Data.Etapas;

                for (var i in etapas) {
                    switch (etapas[i].Etapa) {
                        case EnumEtapaFluxoGestaoPatio.InformarDoca:
                            _gestaoPatioDestino.InformarDocaCarregamento.text(etapas[i].Descricao);
                            _gestaoPatioDestino.InformarDocaCarregamento.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatioDestino.InformarDocaCarregamentoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.ChegadaVeiculo:
                            _gestaoPatioDestino.ChegadaVeiculo.text(etapas[i].Descricao);
                            _gestaoPatioDestino.ChegadaVeiculo.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatioDestino.ChegadaVeiculoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.Guarita:
                            _gestaoPatioDestino.GuaritaEntrada.text(etapas[i].Descricao);
                            _gestaoPatioDestino.GuaritaEntrada.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatioDestino.GuaritaEntradaBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.CheckList:
                            _gestaoPatioDestino.CheckList.text(etapas[i].Descricao);
                            _gestaoPatioDestino.CheckList.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatioDestino.CheckListBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            _gestaoPatioDestino.CheckListPermiteSalvarSemPreencher.visible(!etapas[i].CheckListPermiteSalvarSemPreencher);
                            break;
                        case EnumEtapaFluxoGestaoPatio.TravamentoChave:
                            _gestaoPatioDestino.TravaChave.text(etapas[i].Descricao);
                            _gestaoPatioDestino.TravaChave.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatioDestino.TravaChaveBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.LiberacaoChave:
                            _gestaoPatioDestino.LiberaChave.text(etapas[i].Descricao);
                            _gestaoPatioDestino.LiberaChave.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatioDestino.LiberaChaveBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.Faturamento:
                            _gestaoPatioDestino.Faturamento.text(etapas[i].Descricao);
                            _gestaoPatioDestino.Faturamento.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatioDestino.FaturamentoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.InicioViagem:
                            _gestaoPatioDestino.GuaritaSaida.text(etapas[i].Descricao);
                            _gestaoPatioDestino.GuaritaSaida.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatioDestino.GuaritaSaidaBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.ChegadaLoja:
                            _gestaoPatioDestino.ChegadaLoja.text(etapas[i].Descricao);
                            _gestaoPatioDestino.ChegadaLoja.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatioDestino.ChegadaLojaBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.DeslocamentoPatio:
                            _gestaoPatioDestino.DeslocamentoPatio.text(etapas[i].Descricao);
                            _gestaoPatioDestino.DeslocamentoPatio.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatioDestino.DeslocamentoPatioBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.SaidaLoja:
                            _gestaoPatioDestino.SaidaLoja.text(etapas[i].Descricao);
                            _gestaoPatioDestino.SaidaLoja.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatioDestino.SaidaLojaBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.FimViagem:
                            _gestaoPatioDestino.FimViagem.text(etapas[i].Descricao);
                            _gestaoPatioDestino.FimViagem.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatioDestino.FimViagemBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.InicioHigienizacao:
                            _gestaoPatioDestino.InicioHigienizacao.text(etapas[i].Descricao);
                            _gestaoPatioDestino.InicioHigienizacao.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatioDestino.InicioHigienizacaoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.FimHigienizacao:
                            _gestaoPatioDestino.FimHigienizacao.text(etapas[i].Descricao);
                            _gestaoPatioDestino.FimHigienizacao.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatioDestino.FimHigienizacaoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.SolicitacaoVeiculo:
                            _gestaoPatioDestino.SolicitacaoVeiculo.text(etapas[i].Descricao);
                            _gestaoPatioDestino.SolicitacaoVeiculo.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatioDestino.SolicitacaoVeiculoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.InicioDescarregamento:
                            _gestaoPatioDestino.InicioDescarregamento.text(etapas[i].Descricao);
                            _gestaoPatioDestino.InicioDescarregamento.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatioDestino.InicioDescarregamentoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.FimDescarregamento:
                            _gestaoPatioDestino.FimDescarregamento.text(etapas[i].Descricao);
                            _gestaoPatioDestino.FimDescarregamento.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatioDestino.FimDescarregamentoBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            break;
                        case EnumEtapaFluxoGestaoPatio.AvaliacaoDescarga:
                            _gestaoPatioDestino.AvaliacaoDescarga.text(etapas[i].Descricao);
                            _gestaoPatioDestino.AvaliacaoDescarga.textDefault(EnumEtapaFluxoGestaoPatio.obterDescricao(etapas[i].Etapa));
                            _gestaoPatioDestino.AvaliacaoDescargaBaixarQRCode.permiteBaixarQRCode = etapas[i].PermiteQRCode;
                            _gestaoPatioDestino.AvaliacaoDescargaPermiteSalvarSemPreencher.visible(!etapas[i].AvaliacaoDescargaPermiteSalvarSemPreencher);
                            break;
                    }
                }
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function ControlarAlturarBoxesDestino() {
    $styleAlturaDestino.html("");

    var maiorAltura = 0;

    $boxesSequenciaPatioDestino.find(".box .card").each(function () {
        var $el = $(this);
        var height = $el.height();

        maiorAltura = height > maiorAltura ? height : maiorAltura;
    });

    var style = $templateAlturaDestino.html();
    style = style.replace(/{{altura}}/, maiorAltura);
    style = style.replace(/{{alturaplaceholder}}/, maiorAltura + 80);

    $styleAlturaDestino.html(style);
}

function baixarQRCodeCheckListDestinoClick() {
    baixarQRCodeDestino(EnumEtapaFluxoGestaoPatio.CheckList);
}

function baixarQRCodeChegadaLojaDestinoClick() {
    baixarQRCodeDestino(EnumEtapaFluxoGestaoPatio.ChegadaLoja);
}

function baixarQRCodeChegadaVeiculoDestinoClick() {
    baixarQRCodeDestino(EnumEtapaFluxoGestaoPatio.ChegadaVeiculo);
}

function baixarQRCodeDeslocamentoPatioDestinoClick() {
    baixarQRCodeDestino(EnumEtapaFluxoGestaoPatio.DeslocamentoPatio);
}

function baixarQRCodeFaturamentoDestinoClick() {
    baixarQRCodeDestino(EnumEtapaFluxoGestaoPatio.Faturamento);
}

function baixarQRCodeFimDescarregamentoDestinoClick() {
    baixarQRCodeDestino(EnumEtapaFluxoGestaoPatio.FimDescarregamento);
}

function baixarQRCodeFimHigienizacaoDestinoClick() {
    baixarQRCodeDestino(EnumEtapaFluxoGestaoPatio.FimHigienizacao);
}

function baixarQRCodeFimViagemDestinoClick() {
    baixarQRCodeDestino(EnumEtapaFluxoGestaoPatio.FimViagem);
}

function baixarQRCodeGuaritaEntradaDestinoClick() {
    baixarQRCodeDestino(EnumEtapaFluxoGestaoPatio.Guarita);
}

function baixarQRCodeGuaritaSaidaDestinoClick() {
    baixarQRCodeDestino(EnumEtapaFluxoGestaoPatio.InicioViagem);
}

function baixarQRCodeInformarDocaCarregamentoDestinoClick() {
    baixarQRCodeDestino(EnumEtapaFluxoGestaoPatio.InformarDoca);
}

function baixarQRCodeInicioDescarregamentoDestinoClick() {
    baixarQRCodeDestino(EnumEtapaFluxoGestaoPatio.InicioDescarregamento);
}

function baixarQRCodeInicioHigienizacaoDestinoClick() {
    baixarQRCodeDestino(EnumEtapaFluxoGestaoPatio.InicioHigienizacao);
}

function baixarQRCodeLiberaChaveDestinoClick() {
    baixarQRCodeDestino(EnumEtapaFluxoGestaoPatio.LiberacaoChave);
}

function baixarQRCodeSaidaLojaDestinoClick() {
    baixarQRCodeDestino(EnumEtapaFluxoGestaoPatio.SaidaLoja);
}

function baixarQRCodeSolicitacaoVeiculoDestinoClick() {
    baixarQRCodeDestino(EnumEtapaFluxoGestaoPatio.SolicitacaoVeiculo);
}

function baixarQRCodeTravaChaveDestinoClick() {
    baixarQRCodeDestino(EnumEtapaFluxoGestaoPatio.TravamentoChave);
}

function baixarQRCodeAvaliacaoDescargaDestinoClick() {
    baixarQRCodeDestino(EnumEtapaFluxoGestaoPatio.AvaliacaoDescarga);
}

/*
 * Declaração das Funções Públicas
 */

function GetSetGestaoPatioDestino(data) {
    if (arguments.length > 0)
        SetGestaoPatioDestino(data);
    else
        return GetGestaoPatioDestino();
}

function GetSetOrdemGestaoPatioDestino(data) {
    if (arguments.length > 0)
        SetaOrdemDestino(data);
    else
        return OrdemGestaoPatioDestino();
}

function limparCamposGestaoPatioDestino() {
    LimparCampos(_gestaoPatioDestino);
    _gestaoPatioDestino.TipoCheckListImpressao.val(0);
    controlarExibicaoBotoesBaixarQRCodeDestino();
}

/*
 * Declaração das Funções Privadas
 */

function baixarQRCodeDestino(etapaFluxoGestaoPatio) {
    if (isCadastroEmEdicao())
        executarDownload("Filial/BaixarQrCodeEtapa", { Filial: _sequenciaGestaoPatio.Filial.codEntity(), Etapa: etapaFluxoGestaoPatio, Tipo: EnumTipoFluxoGestaoPatio.Destino });
}

function controlarExibicaoBotaoBaixarQRCodeDestino(knout, exibir) {
    if (exibir)
        knout.visible(knout.permiteBaixarQRCode);
    else
        knout.visible(false);
}

function controlarExibicaoBotoesBaixarQRCodeDestino() {
    var isEdicao = isCadastroEmEdicao();

    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.CheckListBaixarQRCode, (isEdicao && _gestaoPatioDestino.CheckList.val()));
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.ChegadaLojaBaixarQRCode, (isEdicao && _gestaoPatioDestino.ChegadaLoja.val()));
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.ChegadaVeiculoBaixarQRCode, (isEdicao && _gestaoPatioDestino.ChegadaVeiculo.val()));
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.DeslocamentoPatioBaixarQRCode, (isEdicao && _gestaoPatioDestino.DeslocamentoPatio.val()));
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.FaturamentoBaixarQRCode, (isEdicao && _gestaoPatioDestino.Faturamento.val()));
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.FimDescarregamentoBaixarQRCode, (isEdicao && _gestaoPatioDestino.FimDescarregamento.val()));
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.FimHigienizacaoBaixarQRCode, (isEdicao && _gestaoPatioDestino.FimHigienizacao.val()));
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.FimViagemBaixarQRCode, (isEdicao && _gestaoPatioDestino.FimViagem.val()));
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.GuaritaEntradaBaixarQRCode, (isEdicao && _gestaoPatioDestino.GuaritaEntrada.val()));
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.GuaritaSaidaBaixarQRCode, (isEdicao && _gestaoPatioDestino.GuaritaSaida.val()));
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.InformarDocaCarregamentoBaixarQRCode, (isEdicao && _gestaoPatioDestino.InformarDocaCarregamento.val()));
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.InicioDescarregamentoBaixarQRCode, (isEdicao && _gestaoPatioDestino.InicioDescarregamento.val()));
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.InicioHigienizacaoBaixarQRCode, (isEdicao && _gestaoPatioDestino.InicioHigienizacao.val()));
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.LiberaChaveBaixarQRCode, (isEdicao && _gestaoPatioDestino.LiberaChave.val()));
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.SaidaLojaBaixarQRCode, (isEdicao && _gestaoPatioDestino.SaidaLoja.val()));
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.TravaChaveBaixarQRCode, (isEdicao && _gestaoPatioDestino.TravaChave.val()));
}

function desabilitarBoxCheckListDestino(habilitar) {
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.CheckListBaixarQRCode, (habilitar && isCadastroEmEdicao()));

    if (!habilitar) {
        _gestaoPatioDestino.CheckListInformarDoca.val(false);
    }
}

function desabilitarBoxChegadaLojaDestino(habilitar) {
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.ChegadaLojaBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxChegadaVeiculoDestino(habilitar) {
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.ChegadaVeiculoBaixarQRCode, (habilitar && isCadastroEmEdicao()));

    if (!habilitar)
        _gestaoPatioDestino.ChegadaVeiculo.val(false);
}

function desabilitarBoxDeslocamentoPatioDestino(habilitar) {
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.DeslocamentoPatioBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxFaturamentoDestino(habilitar) {
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.FaturamentoBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxFimDescarregamentoDestino(habilitar) {
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.FimDescarregamentoBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxFimHigienizacaoDestino(habilitar) {
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.FimHigienizacaoBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxFimViagemDestino(habilitar) {
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.FimViagemBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxGuaritaEntradaDestino(habilitar) {
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.GuaritaEntradaBaixarQRCode, (habilitar && isCadastroEmEdicao()));

    if (!habilitar) {
        _gestaoPatioDestino.GuaritaEntradaInformarDoca.val(false);
        _gestaoPatioDestino.GuaritaEntradaExibirHorarioExato.val(true);
    }
}

function desabilitarBoxGuaritaSaidaDestino(habilitar) {
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.GuaritaSaidaBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxInformarDocaCarregamentoDestino(habilitar) {
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.InformarDocaCarregamentoBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxInicioDescarregamentoDestino(habilitar) {
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.InicioDescarregamentoBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxInicioHigienizacaoDestino(habilitar) {
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.InicioHigienizacaoBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxLiberaChaveDestino(habilitar) {
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.LiberaChaveBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxSaidaLojaDestino(habilitar) {
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.SaidaLojaBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxSolicitacaoVeiculoDestino(habilitar) {
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.SolicitacaoVeiculoBaixarQRCode, (habilitar && isCadastroEmEdicao()));
}

function desabilitarBoxTravaChaveDestino(habilitar) {
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.TravaChaveBaixarQRCode, (habilitar && isCadastroEmEdicao()));

    if (!habilitar)
        _gestaoPatioDestino.TravaChaveInformarDoca.val(false);
}

function desabilitarBoxAvaliacaoDestino(habilitar) {
    controlarExibicaoBotaoBaixarQRCodeDestino(_gestaoPatioDestino.AvaliacaoDescargaBaixarQRCode, (habilitar && isCadastroEmEdicao()));

    if (!habilitar) {
        _gestaoPatioDestino.AvaliacaoDescargaInformarDoca.val(false);
    }
}

function GetGestaoPatioDestino() {
    var json = RetornarObjetoPesquisa(_gestaoPatioDestino);

    return JSON.stringify(json);
}

function OrdemGestaoPatioDestino() {
    var elementos = $boxesSequenciaPatioDestino.sortable("toArray");

    var ordem = elementos.map(function (etapa, index) {
        var id = etapa.split("_");
        return {
            Etapa: id[1],
            Ordem: index
        }
    });

    return JSON.stringify(ordem);
}

function SetaOrdemDestino(ordens) {
    if (ordens == null) return;

    var DOMContainer = $boxesSequenciaPatioDestino[0];

    if (DOMContainer != undefined) {
        for (var i = ordens.length - 1; i > 0; i--) {
            var nodeReference = document.getElementById("EtapaDestino_" + ordens[i].Etapa);
            var newNode = document.getElementById("EtapaDestino_" + ordens[i - 1].Etapa);
            DOMContainer.insertBefore(newNode, nodeReference);
        }
    }
}

function SetGestaoPatioDestino(data) {
    if (data != null) {
        PreencherObjetoKnout(_gestaoPatioDestino, { Data: data });

        controlarExibicaoBotoesBaixarQRCodeDestino();
    }
}
