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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Porto.js" />
/// <reference path="../../Consultas/TipoTerminalImportacao.js" />
/// <reference path="../../Consultas/PedidoViagemNavio.js" />
/// <reference path="../../Enumeradores/EnumTipoPropostaFeeder.js" />
/// <reference path="CTe.js" />

var InformacaoModal = function (cte) {

    var instancia = this;

    this.NumeroControle = PropertyEntity({ text: Localization.Resources.CTes.CTe.NumeroControle.getFieldDescription(), maxlength: 150, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroBooking = PropertyEntity({ text: Localization.Resources.CTes.CTe.NumeroBooking.getFieldDescription(), maxlength: 150, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.DescricaoCarrier = PropertyEntity({ text: Localization.Resources.CTes.CTe.DescricaoCarrier.getFieldDescription(), maxlength: 150, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoPropostaFeeder = PropertyEntity({ val: ko.observable(""), options: EnumTipoPropostaFeeder.obterOpcoes(), def: "", text: Localization.Resources.CTes.CTe.PropostaFeeder.getFieldDescription(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.OcorreuSinistroAvaria = PropertyEntity({ text: Localization.Resources.CTes.CTe.OcorreuSinistroAvaria, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });

    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.PortoDeOrigem.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.PortoPassagemUm = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.PortoDePassagemUm.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.PortoPassagemDois = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.PortoDePassagemDois.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.PortoPassagemTres = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.PortoDePassagemTres.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.PortoPassagemQuatro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.PortoDePassagemQuatro.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.PortoPassagemCinco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.PortoDePassagemCinco.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.PortoDeDestino.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.TerminalDeOrigem.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.TerminalDeDestino.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.Viagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.Viagem.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, cte.IdKnockoutInformacaoModal);

        new BuscarPorto(instancia.PortoOrigem);
        new BuscarPorto(instancia.PortoPassagemUm);
        new BuscarPorto(instancia.PortoPassagemDois);
        new BuscarPorto(instancia.PortoPassagemTres);
        new BuscarPorto(instancia.PortoPassagemQuatro);
        new BuscarPorto(instancia.PortoPassagemCinco);
        new BuscarPorto(instancia.PortoDestino);
        new BuscarTipoTerminalImportacao(instancia.TerminalOrigem);
        new BuscarTipoTerminalImportacao(instancia.TerminalDestino);
        new BuscarPedidoViagemNavio(instancia.Viagem);
    };

    this.DestivarInformacaoModal = function () {
        DesabilitarCamposInstanciasCTe(instancia);
    };
};