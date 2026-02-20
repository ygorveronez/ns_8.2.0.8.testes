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
/// <reference path="Pessoa.js" />

var _pessoaComponentes;
var _gridPessoaComponentes;

//*******MAPEAMENTO KNOUCKOUT*******

var PessoaComponenteMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.CodigoFilial = PropertyEntity({ type: types.map, val: "" });
    this.Filial = PropertyEntity({ type: types.map, val: "" });
    this.CodigoTransportadora = PropertyEntity({ type: types.map, val: "" });
    this.Transportadora = PropertyEntity({ type: types.map, val: "" });
    this.CodigoComponente = PropertyEntity({ type: types.map, val: "" });
    this.Componente = PropertyEntity({ type: types.map, val: "" });
    this.ValorComponente = PropertyEntity({ type: types.map, val: "" });
}

var PessoaComponente = function () {
    this.CodigoComponente = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Filial.getRequiredFieldDescription(), idBtnSearch: guid(), issue: 70, visible: ko.observable(true), required: false, enable: ko.observable(true) });
    this.Transportadora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Transportadora.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), required: false, enable: ko.observable(true) });
    this.Componente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Pessoas.Pessoa.ComponenteFrete.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.ValorComponente = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: true, text: Localization.Resources.Pessoas.Pessoa.Valor.getRequiredFieldDescription(), maxlength: 7, enable: ko.observable(true) });
    this.GridPessoaComponentes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });

    this.AdicionarComponente = PropertyEntity({ eventClick: adicionarPessoaComponenteClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: !_FormularioSomenteLeitura });
    this.AtualizarComponente = PropertyEntity({ eventClick: atualizarPessoaComponenteClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.ExcluirComponente = PropertyEntity({ eventClick: excluirPessoaComponenteClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.CancelarComponente = PropertyEntity({ eventClick: LimparCamposPessoaComponentes, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
}

//*******EVENTOS*******

function loadPessoaComponente() {
    _pessoaComponentes = new PessoaComponente();
    KoBindings(_pessoaComponentes, "knoutComponente");

    new BuscarComponentesDeFrete(_pessoaComponentes.Componente);
    new BuscarFilial(_pessoaComponentes.Filial);
    new BuscarTransportadores(_pessoaComponentes.Transportadora);

    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarPessoaComponente, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoFilial", visible: false },
        { data: "Filial", title: Localization.Resources.Pessoas.Pessoa.Filial, width: "70%", className: "text-align-left" },
        { data: "CodigoTransportadora", visible: false },
        { data: "Transportadora", title: Localization.Resources.Pessoas.Pessoa.Transportadora, width: "70%", className: "text-align-left" },
        { data: "CodigoComponente", visible: false },
        { data: "Componente", title: Localization.Resources.Pessoas.Pessoa.Componente, width: "70%", className: "text-align-left" },
        { data: "ValorComponente", title: Localization.Resources.Pessoas.Pessoa.Valor, width: "20%", className: "text-align-left" }
    ];

    _gridPessoaComponentes = new BasicDataTable(_pessoaComponentes.GridPessoaComponentes.idGrid, header, menuOpcoes);
    recarregarGridPessoaComponentes();
}

function adicionarPessoaComponenteClick() {
    var valido = ValidarCamposObrigatorios(_pessoaComponentes);

    if (Globalize.parseFloat(_pessoaComponentes.ValorComponente.val()) <= 0) {
        valido = false;
        _pessoaComponentes.ValorComponente.requiredClass("form-control");
        _pessoaComponentes.ValorComponente.requiredClass("form-control is-invalid");
    }

    if (valido) {
        var existe = false;
        $.each(_pessoa.ListaComponentes.list, function (i, pessoaComponente) {
            if (pessoaComponente.CodigoComponente.val == _pessoaComponentes.Componente.codEntity() && pessoaComponente.CodigoFilial == _pessoaComponentes.Componente.codEntity() && pessoaComponente.CodigoTransportadora == _pessoaComponentes.Transportadora.codEntity()) {
                existe = true;
                return;
            }
        });

        if (existe) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.Pessoa.ComponenteJaExistente, Localization.Resources.Pessoas.Pessoa.ComponenteJaEstaCadastrado.format(_pessoaComponentes.Componente.val()));
            return;
        }

        var pessoaComponente = new PessoaComponenteMap();
        pessoaComponente.Codigo.val = guid();
        pessoaComponente.CodigoFilial.val = _pessoaComponentes.Filial.codEntity();
        pessoaComponente.Filial.val = _pessoaComponentes.Filial.val();
        pessoaComponente.CodigoTransportadora.val = _pessoaComponentes.Transportadora.codEntity();
        pessoaComponente.Transportadora.val = _pessoaComponentes.Transportadora.val();
        pessoaComponente.CodigoComponente.val = _pessoaComponentes.Componente.codEntity();
        pessoaComponente.Componente.val = _pessoaComponentes.Componente.val();
        pessoaComponente.ValorComponente.val = _pessoaComponentes.ValorComponente.val();

        _pessoa.ListaComponentes.list.push(pessoaComponente);
        recarregarGridPessoaComponentes();
        LimparCamposPessoaComponentes();
        $("#" + _pessoaComponente.Componente.id).focus();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function atualizarPessoaComponenteClick() {
    var valido = ValidarCamposObrigatorios(_pessoaComponentes);

    if (Globalize.parseFloat(_pessoaComponentes.ValorComponente.val()) <= 0) {
        valido = false;
        _pessoaComponentes.ValorComponente.requiredClass("form-control");
        _pessoaComponentes.ValorComponente.requiredClass("form-control is-invalid");
    }

    if (valido) {
        $.each(_pessoa.ListaComponentes.list, function (i, pessoaComponente) {
            if (pessoaComponente.Codigo.val == _pessoaComponentes.CodigoComponente.val()) {
                pessoaComponente.ValorComponente.val = _pessoaComponentes.ValorComponente.val();

                return false;
            }
        });
        recarregarGridPessoaComponentes();
        LimparCamposPessoaComponentes();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function excluirPessoaComponenteClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pessoas.Pessoa.RealmenteDesejaExcluirComponente + " " + _pessoaComponentes.Componente.val() + "?", function () {
        var listaAtualizada = new Array();
        $.each(_pessoa.ListaComponentes.list, function (i, pessoaComponente) {
            if (pessoaComponente.Codigo.val != _pessoaComponentes.CodigoComponente.val()) {
                listaAtualizada.push(pessoaComponente);
            }
        });
        _pessoa.ListaComponentes.list = listaAtualizada;
        recarregarGridPessoaComponentes();
        LimparCamposPessoaComponentes();
    });
}

//*******MÉTODOS*******

function recarregarGridPessoaComponentes() {
    var data = new Array();
    $.each(_pessoa.ListaComponentes.list, function (i, componente) {
        var pessoaComponente = new Object();

        pessoaComponente.Codigo = componente.Codigo.val;
        pessoaComponente.CodigoFilial = componente.CodigoFilial.val;
        pessoaComponente.Filial = componente.Filial.val;
        pessoaComponente.CodigoTransportadora = componente.CodigoTransportadora.val;
        pessoaComponente.Transportadora = componente.Transportadora.val;
        pessoaComponente.CodigoComponente = componente.CodigoComponente.val;
        pessoaComponente.Componente = componente.Componente.val;
        pessoaComponente.ValorComponente = componente.ValorComponente.val;

        data.push(pessoaComponente);
    });
    _gridPessoaComponentes.CarregarGrid(data);
}

function editarPessoaComponente(data) {
    LimparCamposPessoaComponentes();
    $.each(_pessoa.ListaComponentes.list, function (i, pessoaComponente) {
        if (pessoaComponente.Codigo.val == data.Codigo) {
            _pessoaComponentes.CodigoComponente.val(pessoaComponente.Codigo.val);
            _pessoaComponentes.Filial.codEntity(pessoaComponente.CodigoFilial.val);
            _pessoaComponentes.Filial.val(pessoaComponente.Filial.val);
            _pessoaComponentes.Transportadora.codEntity(pessoaComponente.CodigoTransportadora.val);
            _pessoaComponentes.Transportadora.val(pessoaComponente.Transportadora.val);
            _pessoaComponentes.Componente.codEntity(pessoaComponente.CodigoComponente.val);
            _pessoaComponentes.Componente.val(pessoaComponente.Componente.val);
            _pessoaComponentes.ValorComponente.val(pessoaComponente.ValorComponente.val);

            _pessoaComponentes.Componente.enable(false);
            _pessoaComponentes.Filial.enable(false);
            _pessoaComponentes.Transportadora.enable(false);

            return false;
        }
    });

    _pessoaComponentes.AdicionarComponente.visible(false);
    _pessoaComponentes.AtualizarComponente.visible(true);
    _pessoaComponentes.ExcluirComponente.visible(true);
    _pessoaComponentes.CancelarComponente.visible(true);
}

function LimparCamposPessoaComponentes() {
    LimparCampoEntity(_pessoaComponentes.Componente);
    LimparCampoEntity(_pessoaComponentes.Filial);
    LimparCampoEntity(_pessoaComponentes.Transportadora);

    _pessoaComponentes.ValorComponente.val("0,00");
    _pessoaComponentes.Filial.requiredClass("form-control");
    _pessoaComponentes.Transportadora.requiredClass("form-control");
    _pessoaComponentes.Componente.requiredClass("form-control");
    _pessoaComponentes.ValorComponente.requiredClass("form-control");

    _pessoaComponentes.Componente.enable(true);
    _pessoaComponentes.Filial.enable(true);
    _pessoaComponentes.Transportadora.enable(true);

    _pessoaComponentes.AdicionarComponente.visible(true);
    _pessoaComponentes.AtualizarComponente.visible(false);
    _pessoaComponentes.ExcluirComponente.visible(false);
    _pessoaComponentes.CancelarComponente.visible(false);
}