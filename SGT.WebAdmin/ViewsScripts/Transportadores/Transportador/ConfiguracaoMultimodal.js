/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumFormaRateioSVM.js" />


var _configuracaoMultimodal;

var ConfiguracaoMultimodal = function () {
    this.FormaRateioSVM = PropertyEntity({ val: ko.observable(EnumFormaRateioSVM.Nenhum), options: EnumFormaRateioSVM.obterOpcoes(), def: EnumFormaRateioSVM.Nenhum, text: Localization.Resources.Transportadores.Transportador.FormaRateioParaEmissaoSVM.getFieldDescription() });
    this.SVMMesmoQueMultimodal = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.MesmoCTeMultimodal, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.SVMTerminaisPortuarioOrigemDestino = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.TerminaisPortuariosOrigemDestinoCarga, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.SVMBUSPortoOrigemDestino = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.BUsPortoOrigemPortoDestinoCarga, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });

    this.FormaRateioSVM.val.subscribe(function (novoValor) {
        _configuracaoMultimodal.SVMMesmoQueMultimodal.val(false);
        _configuracaoMultimodal.SVMTerminaisPortuarioOrigemDestino.val(false);
        _configuracaoMultimodal.SVMBUSPortoOrigemDestino.val(false);

        if (novoValor === EnumFormaRateioSVM.UmCTeMultimodalParaUmCTeAquaviario) {
            _configuracaoMultimodal.SVMMesmoQueMultimodal.visible(true);
            _configuracaoMultimodal.SVMTerminaisPortuarioOrigemDestino.visible(false);
            _configuracaoMultimodal.SVMBUSPortoOrigemDestino.visible(false);

            _configuracaoMultimodal.SVMMesmoQueMultimodal.val(true);
        }
        else if (novoValor === EnumFormaRateioSVM.AgruparPorTerminalOrigemDestino || novoValor === EnumFormaRateioSVM.AgruparPorSacado || novoValor === EnumFormaRateioSVM.AgruparPorTerminalOrigemDestinoSacado) {
            _configuracaoMultimodal.SVMMesmoQueMultimodal.visible(false);
            _configuracaoMultimodal.SVMTerminaisPortuarioOrigemDestino.visible(true);
            _configuracaoMultimodal.SVMBUSPortoOrigemDestino.visible(true);
        }
        else if (novoValor === EnumFormaRateioSVM.AgruparPorBookingRemetenteDestinatarioExpedidorRecebedor || novoValor === EnumFormaRateioSVM.AgruparPorRemetenteDestinatario || novoValor === EnumFormaRateioSVM.AgruparPorBookingRemetenteDestinatarioExpedidorRecebedorSacado || novoValor === EnumFormaRateioSVM.AgruparPorRemetenteDestinatarioSacado) {
            _configuracaoMultimodal.SVMMesmoQueMultimodal.visible(true);
            _configuracaoMultimodal.SVMTerminaisPortuarioOrigemDestino.visible(true);
            _configuracaoMultimodal.SVMBUSPortoOrigemDestino.visible(true);
        } else {
            _configuracaoMultimodal.SVMMesmoQueMultimodal.visible(false);
            _configuracaoMultimodal.SVMTerminaisPortuarioOrigemDestino.visible(false);
            _configuracaoMultimodal.SVMBUSPortoOrigemDestino.visible(false);
        }
    });

    this.SVMMesmoQueMultimodal.val.subscribe(function (novoValor) {
        if (novoValor) {
            //_configuracaoMultimodal.SVMMesmoQueMultimodal.val(false);
            _configuracaoMultimodal.SVMTerminaisPortuarioOrigemDestino.val(false);
            _configuracaoMultimodal.SVMBUSPortoOrigemDestino.val(false);
        }
    });

    this.SVMTerminaisPortuarioOrigemDestino.val.subscribe(function (novoValor) {
        if (novoValor) {
            _configuracaoMultimodal.SVMMesmoQueMultimodal.val(false);
            //_configuracaoMultimodal.SVMTerminaisPortuarioOrigemDestino.val(false);
            _configuracaoMultimodal.SVMBUSPortoOrigemDestino.val(false);
        }
    });

    this.SVMBUSPortoOrigemDestino.val.subscribe(function (novoValor) {
        if (novoValor) {
            _configuracaoMultimodal.SVMMesmoQueMultimodal.val(false);
            _configuracaoMultimodal.SVMTerminaisPortuarioOrigemDestino.val(false);
            //_configuracaoMultimodal.SVMBUSPortoOrigemDestino.val(false);
        }
    });
}

//*******EVENTOS*******

function loadConfiguracoesMultimodal() {
    _configuracaoMultimodal = new ConfiguracaoMultimodal();
    KoBindings(_configuracaoMultimodal, "knockoutCadastroConfiguracaoMultimodal");
}

function limparCamposConfiguracoesMultimodal() {
    LimparCampos(_configuracaoMultimodal);
}