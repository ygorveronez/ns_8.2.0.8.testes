/// <reference path="../../Enumeradores/EnumValorTipoAutomatizacaoTipoCarga.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="TipoCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoAutomatizacaoTipoCarga.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoCargaModeloVeicularAutoConfig;
var _tipoCargaModeloVeicularAutoConfig;
var _pesquisaTipoCargaModeloVeicularAutoConfig;

var TipoCargaModeloVeicularAutoConfig = function () {
    this.GridTipoCargaValor = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.AutoTipoCargaHabilitado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: Localization.Resources.Cargas.TipoCargaModeloVeicularAutoConfig.HabilitarAutomatizacaoTipoCarga, def: false });
    this.TipoAutomatizacaoTipoCarga = PropertyEntity({ val: ko.observable(EnumTipoAutomatizacaoTipoCarga.NaoHabilitado), options: ko.observable(EnumTipoAutomatizacaoTipoCarga.obterOpcoes()), def: EnumTipoAutomatizacaoTipoCarga.NaoHabilitado, text: Localization.Resources.Cargas.TipoCargaModeloVeicularAutoConfig.TipoAutomatizacao.getRequiredFieldDescription(), issue: 1043 });
    this.AutoModeloVeicularHabilitado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: Localization.Resources.Cargas.TipoCargaModeloVeicularAutoConfig.HabilitarAutomatizacaoModeloVeicularCarga, issue: 1044, def: false });
    this.ControlarModeloPorNumeroPaletes = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: Localization.Resources.Cargas.TipoCargaModeloVeicularAutoConfig.ControlarModeloVeicularNumeroPaletes, def: false });
    this.ControlarModeloPorPeso = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: Localization.Resources.Cargas.TipoCargaModeloVeicularAutoConfig.ControlarModeloVeicularPeso, def: false });
    this.ConsiderarToleranciaMenorPesoModelo = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: Localization.Resources.Cargas.TipoCargaModeloVeicularAutoConfig.ConsiderarToleranciaMinimaPeso, def: false });
    this.TiposCarga = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoCarga.getRequiredFieldDescription(), required: true, idBtnSearch: guid() });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar });
    this.AdicionarTiposCarga = PropertyEntity({ eventClick: adicionarTipoCargaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar });

    this.TipoValorTipoCargaValor = PropertyEntity({ val: ko.observable(EnumValorAutomatizacaoTipoCargaValor.Ate), options: ko.observable(EnumValorAutomatizacaoTipoCargaValor.obterOpcoes()), def: EnumValorAutomatizacaoTipoCargaValor.Ate, text: Localization.Resources.Cargas.TipoCargaModeloVeicularAutoConfig.TiposValor.getRequiredFieldDescription() });
    this.ValorTipoCargaValor = PropertyEntity({ getType: typesKnockout.decimal, maxlength: 10, val: ko.observable("0,00"), text: Localization.Resources.Gerais.Geral.Valor.getRequiredFieldDescription(), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.TiposCargaValor = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: Localization.Resources.Cargas.TipoCargaModeloVeicularAutoConfig.TiposCargaValor.getRequiredFieldDescription(), required: true, idBtnSearch: guid() });
    this.CodigoUFDestino = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.UFDestino = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Estado.getFieldDescription(), required: true, idBtnSearch: guid() });
    this.AdicionarTipoCargaValor = PropertyEntity({ eventClick: adicionarTipoCargaValorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar });

    this.TipoAutomatizacaoTipoCarga.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoAutomatizacaoTipoCarga.PorPrioridade) {
            $("#divAutomatizacaoTipoCargaPrioridade").removeClass("d-none");
            $("#divAutomatizacaoTipoCargaValor").addClass("d-none");
        } else if (novoValor == EnumTipoAutomatizacaoTipoCarga.PorValor) {
            $("#divAutomatizacaoTipoCargaPrioridade").addClass("d-none");
            $("#divAutomatizacaoTipoCargaValor").removeClass("d-none");
        } else {
            $("#divAutomatizacaoTipoCargaPrioridade").addClass("d-none");
            $("#divAutomatizacaoTipoCargaValor").addClass("d-none");
        }
    });
}

//*******EVENTOS*******

function loadTipoCargaModeloVeicularAutoConfig() {

    _tipoCargaModeloVeicularAutoConfig = new TipoCargaModeloVeicularAutoConfig();
    KoBindings(_tipoCargaModeloVeicularAutoConfig, "knockoutConfigTipoCargaModeloVeicularAutoConfig");

    HeaderAuditoria("TipoCargaPrioridadeCargaAutoConfig");

    BuscarPorCodigo(_tipoCargaModeloVeicularAutoConfig, "TipoCargaModeloVeicularAutoConfig/BuscarConfig", function (arg) {
        loadTipoCargaValor();
        loadTipoCarga();
        recarregarGridReorder();
        $("#knockoutConfigTipoCargaModeloVeicularAutoConfig").show();
    });
}

function atualizarClick(e, sender) {
    reordenarPosicoesTipoCarga();
    Salvar(e, "TipoCargaModeloVeicularAutoConfig/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AutorizadoSucesso);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}
