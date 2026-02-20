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
/// <reference path="../../Consultas/Almoxarifado.js" />
/// <reference path="../../Consultas/BandaRodagemPneu.js" />
/// <reference path="../../Consultas/ModeloPneu.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Enumeradores/EnumTipoAquisicaoPneu.js" />
/// <reference path="../../Enumeradores/EnumVidaPneu.js" />
/// <reference path="../Pneu/Pneu.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pneuLote;

var PneuLote = function () {
    this.IdModal = PropertyEntity({ getType: typesKnockout.string, enable: ko.observable(true), val: ko.observable("") });

    this.QuantidadeDePneu = PropertyEntity({ text: "*Qtde. de Pneus:", getType: typesKnockout.int, val: ko.observable(""), maxlength: 500, required: ko.observable(true), configInt: { precision: 0, allowZero: false }, visible: ko.observable(true) });
    this.TipoLancamento = PropertyEntity({ text: "*Tipo Lançamento: ", val: ko.observable(EnumTipoLancamentoPneu.PorQuantidade), options: EnumTipoLancamentoPneu.obterOpcoes(), def: EnumTipoLancamentoPneu.PorQuantidade });

    this.NumeroInicial = PropertyEntity({ text: "*Numero Inicial:", val: ko.observable(""), visible: ko.observable(false), required: ko.observable(false), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false } });
    this.NumeroFinal = PropertyEntity({ text: "*Numero Final:", val: ko.observable(""), visible: ko.observable(false), required: ko.observable(false), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false } });


    this.DataEntrada = PropertyEntity({ text: "*Data de Entrada: ", getType: typesKnockout.date, required: true });
    this.TipoAquisicao = PropertyEntity({ text: "*Tipo Aquisição: ", val: ko.observable(EnumTipoAquisicaoPneu.PneuNovoReposicao), options: EnumTipoAquisicaoPneu.obterOpcoes(), def: EnumTipoAquisicaoPneu.PneuNovoReposicao });
    this.VidaAtual = PropertyEntity({ text: "*Vida Atual: ", val: ko.observable(EnumVidaPneu.PneuNovo), options: EnumVidaPneu.obterOpcoes(), def: EnumVidaPneu.PneuNovo });

    this.Modelo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo:", idBtnSearch: guid(), required: true });
    this.Almoxarifado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Almoxarifado:", idBtnSearch: guid(), required: true });
    this.BandaRodagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Banda de Rodagem:", idBtnSearch: guid(), required: true });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto:", idBtnSearch: guid(), required: true });
    this.KmAtualRodado = PropertyEntity({ text: "*Km Atual Rodado:", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: true, thousands: "" }, required: true });
    this.ValorAquisicao = PropertyEntity({ text: "*Valor de Aquisição:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, enable: ko.observable(true), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPneuLoteClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarPneuLoteClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });

    this.Sulco = PropertyEntity({ text: "Sulco:", getType: typesKnockout.decimal, val: ko.observable(""), required: false, configInt: { precision: 2, allowZero: true } });
    this.DTO = PropertyEntity({ text: "DOT:", getType: typesKnockout.string, val: ko.observable(""), required: false });
    this.DescricaoNota = PropertyEntity({ maxlength: 500, text: "Descrição da Nota:", required: false, enable: ko.observable(true) });
}

//*******EVENTOS*******

function LancarPneuLote() {
    _pneuLote = new PneuLote();
    _pneuLote.IdModal.val(guid());
    //limparCamposLotePneu();

    _pneuLote.TipoLancamento.val.subscribe(mudarVisivilidadeCampos);

    RenderizarModalPneuLote();
}

function RenderizarModalPneuLote(callback) {
    $.get("Content/Static/Frota/PneuLote.html?dyn=" + _pneuLote.IdModal.val(), function (dataConteudo) {
        dataConteudo = dataConteudo.replace(/#divModalPneuLote/g, _pneuLote.IdModal.val());
        $("#js-page-content").append(dataConteudo);

        KoBindings(_pneuLote, "knockoutPneuLote_" + _pneuLote.IdModal.val());

        new BuscarAlmoxarifado(_pneuLote.Almoxarifado);
        new BuscarBandaRodagemPneu(_pneuLote.BandaRodagem);
        new BuscarModeloPneu(_pneuLote.Modelo);
        new BuscarProdutoTMS(_pneuLote.Produto);
        
        Global.abrirModal(_pneuLote.IdModal.val());

        $('#' + _pneuLote.IdModal.val()).on('hidden.bs.modal', function () {
            $("#" + _pneuLote.IdModal.val()).remove();
        });

        if (callback !== undefined && callback !== null)
            callback();
    });
}

function adicionarPneuLoteClick() {
    Salvar(_pneuLote, "Pneu/AdicionarPneuLote", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                cancelarPneuLoteClick();

                recarregarGridPneu();
                limparCamposPneu();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function cancelarPneuLoteClick() {    
    Global.fecharModal(_pneuLote.IdModal.val());
}

function limparCamposLotePneu() {
    LimparCampos(_pneuLote);
}

const mudarVisivilidadeCampos = (atual) => {
    const visible = atual === 1 ? true : false;
    const zerarValor = "";

    _pneuLote.QuantidadeDePneu.required(visible);
    _pneuLote.QuantidadeDePneu.visible(visible);
    _pneuLote.QuantidadeDePneu.val(zerarValor);

    _pneuLote.NumeroInicial.required(!visible);
    _pneuLote.NumeroInicial.visible(!visible);
    _pneuLote.NumeroInicial.val(zerarValor);

    _pneuLote.NumeroFinal.required(!visible);
    _pneuLote.NumeroFinal.visible(!visible);
    _pneuLote.NumeroFinal.val(zerarValor);

}