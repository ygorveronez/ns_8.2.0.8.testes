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
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoTerminalImportacao.js" />
/// <reference path="CotacaoPedido.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _importacao;
var _gridDI;

var DIMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.NumeroDI = PropertyEntity({ type: types.map, val: "" });
    this.CodigoImportacao = PropertyEntity({ type: types.map, val: "" });
    this.CodigoReferencia = PropertyEntity({ type: types.map, val: "" });
    this.ValorCarga = PropertyEntity({ type: types.map, val: "" });
    this.Volume = PropertyEntity({ type: types.map, val: "" });
    this.Peso = PropertyEntity({ type: types.map, val: "" });
}

var Importacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroContainer = PropertyEntity({ text: "Nº Container: ", required: false, visible: ko.observable(true), maxlength: 1000, enable: ko.observable(true) });
    this.NumeroBL = PropertyEntity({ text: "BL: ", required: false, visible: ko.observable(true), maxlength: 1000, enable: ko.observable(true) });
    this.NumeroNavio = PropertyEntity({ text: "Navio: ", required: false, visible: ko.observable(true), maxlength: 1000, enable: ko.observable(true) });
    this.Porto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Porto:"), idBtnSearch: guid(), enable: ko.observable(true) });
    this.TipoTerminalImportacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Terminal de Vazio:"), idBtnSearch: guid(), enable: ko.observable(true) });

    this.EnderecoEntregaImportacao = PropertyEntity({ text: "Endereço: ", required: false, visible: ko.observable(true), maxlength: 1000, enable: ko.observable(true) });
    this.BairroEntregaImportacao = PropertyEntity({ text: "Bairro: ", required: false, visible: ko.observable(true), maxlength: 1000, enable: ko.observable(true) });
    this.CEPEntregaImportacao = PropertyEntity({ text: "CEP: ", required: false, visible: ko.observable(true), maxlength: 1000, enable: ko.observable(true) });
    this.LocalidadeEntregaImportacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Município:"), idBtnSearch: guid(), enable: ko.observable(true) });

    this.DataVencimentoArmazenamentoImportacao = PropertyEntity({ text: ko.observable("Data vencimento do armazenamento: "), getType: typesKnockout.date, required: false, enable: ko.observable(true) });
    this.ArmadorImportacao = PropertyEntity({ text: "Armador: ", required: false, visible: ko.observable(true), maxlength: 1000, enable: ko.observable(true) });

    this.NumeroDI = PropertyEntity({ text: "*Nº DI: ", required: false, visible: ko.observable(true), maxlength: 1000, enable: ko.observable(true) });
    this.CodigoImportacao = PropertyEntity({ text: "*Cód. Importação: ", required: false, visible: ko.observable(true), maxlength: 1000, enable: ko.observable(true) });
    this.CodigoReferencia = PropertyEntity({ text: "*Cód. Referencia: ", required: false, visible: ko.observable(true), maxlength: 1000, enable: ko.observable(true) });
    this.ValorCarga = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "*Valor Carga:", required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.Volume = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "*Volume:", required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.Peso = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "*Peso:", required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.GridDI = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true), enable: ko.observable(true) });
    this.ListaDI = PropertyEntity({ val: ko.observable(""), def: "" });
    this.AdicionarDI = PropertyEntity({ eventClick: adicionarDIClick, type: types.event, text: ko.observable("Adicionar DI"), visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadImportacao() {
    _importacao = new Importacao();
    KoBindings(_importacao, "knockoutImportacao");
    $("#" + _importacao.CEPEntregaImportacao.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });
    
    new BuscarClientes(_importacao.Porto);
    new BuscarLocalidades(_importacao.LocalidadeEntregaImportacao);
    new BuscarTipoTerminalImportacao(_importacao.TipoTerminalImportacao);    

    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            excluirDI(data)
        }, tamanho: "10", icone: ""
    };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    var header = [{ data: "Codigo", visible: false },
    { data: "NumeroDI", title: "Nº DI", width: "15%" },
    { data: "CodigoImportacao", title: "Cód. Importação", width: "15%" },
    { data: "CodigoReferencia", title: "Cód. Referência", width: "15%" },
    { data: "ValorCarga", title: "Valor Carga", width: "8%" },
    { data: "Volume", title: "Volume", width: "8%" },
    { data: "Peso", title: "Peso", width: "8%" }];

    _gridDI = new BasicDataTable(_importacao.GridDI.idGrid, header, menuOpcoes);
    recarregarGridDI();
}

function adicionarDIClick(e, sender) {    

    var tudoCerto = true;
    if (_importacao.NumeroDI.val() === "")
        tudoCerto = false;
    if (_importacao.CodigoImportacao.val() === "")
        tudoCerto = false;
    if (_importacao.CodigoReferencia.val() === "")
        tudoCerto = false;
    if (_importacao.ValorCarga.val() === "")
        tudoCerto = false;
    if (_importacao.Volume.val() === "")
        tudoCerto = false;
    if (_importacao.Peso.val() === "")
        tudoCerto = false;

    if (tudoCerto) {
        var map = new Object();

        map.Codigo = guid();
        map.NumeroDI = _importacao.NumeroDI.val();
        map.CodigoImportacao = _importacao.CodigoImportacao.val();
        map.CodigoReferencia = _importacao.CodigoReferencia.val();
        map.ValorCarga = _importacao.ValorCarga.val();
        map.Volume = _importacao.Volume.val();
        map.Peso = _importacao.Peso.val();

        _importacao.GridDI.list.push(map);

        recarregarGridDI();
        limparDadosDI();
        $("#" + _importacao.NumeroDI.id).focus();
    } else {

        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios no laçamento da DI!");
    }
}

function excluirDI(e) {
    if (_cotacaoPedido.SituacaoPedido.val() !== EnumSituacaoPedido.Aberto) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "O status da cotação não permite remover a DI!");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja remover a DI " + e.NumeroDI +"?", function () {
        $.each(_importacao.GridDI.list, function (i, di) {
            if (di !== null && di.Codigo !== null && e !== null && e.Codigo !== null && e.Codigo === di.Codigo)
                _importacao.GridDI.list.splice(i, 1);
        });
        recarregarGridDI();
    });
}

//*******MÉTODOS*******

function recarregarGridDI() {
    var data = new Array();
    $.each(_importacao.GridDI.list, function (i, DI) {
        var obj = new Object();

        obj.Codigo = DI.Codigo;
        obj.NumeroDI = DI.NumeroDI;
        obj.CodigoImportacao = DI.CodigoImportacao;
        obj.CodigoReferencia = DI.CodigoReferencia;
        obj.ValorCarga = DI.ValorCarga;
        obj.Volume = DI.Volume;
        obj.Peso = DI.Peso;

        data.push(obj);
    });
    _gridDI.CarregarGrid(data);
}

function VerificarDadosImportacao() {
    if (ValidarCamposObrigatorios(_importacao)) {
      
        return true;
    } else {
        $("#myTab a:eq(4)").tab("show");
        exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
        return false;
    }
}

function limparDadosDI() {
    _importacao.NumeroDI.val("");
    _importacao.CodigoImportacao.val("");
    _importacao.CodigoReferencia.val("");
    _importacao.ValorCarga.val("");
    _importacao.Volume.val("");
    _importacao.Peso.val("");
}

function limparDI() {
    LimparCampos(_importacao);
    recarregarGridDI();
}