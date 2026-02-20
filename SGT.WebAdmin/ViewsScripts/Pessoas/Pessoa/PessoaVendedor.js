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

var _pessoaVendedores;
var _gridPessoaVendedores;

//*******MAPEAMENTO KNOUCKOUT*******

var PessoaVendedorMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.CodigoFuncionario = PropertyEntity({ type: types.map, val: "" });
    this.CodigoTipoDeCarga = PropertyEntity({ type: types.map, val: "" });
    this.Funcionario = PropertyEntity({ type: types.map, val: "" });
    this.TipoDeCarga = PropertyEntity({ type: types.map, val: "" });
    this.PercentualComissao = PropertyEntity({ type: types.map, val: "" });
    this.DataInicioVigencia = PropertyEntity({ type: types.map, val: "" });
    this.DataFimVigencia = PropertyEntity({ type: types.map, val: "" });
};

var PessoaVendedor = function () {
    this.CodigoVendedor = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Pessoas.Pessoa.Funcionario.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoDeCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: Localization.Resources.Pessoas.Pessoa.TipoDeCarga.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.PercentualComissao = PropertyEntity({ def: "0,00000", val: ko.observable("0,00000"), getType: typesKnockout.decimal, required: true, text: Localization.Resources.Pessoas.Pessoa.PercentualComissao.getRequiredFieldDescription(), configDecimal: { precision: 5}, maxlength: 8, enable: ko.observable(true) });
    this.DataInicioVigencia = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.DataInicioVigencia.getRequiredFieldDescription(), getType: typesKnockout.date, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataFimVigencia = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.DataFimVigencia.getRequiredFieldDescription(), getType: typesKnockout.date, required: true, enable: ko.observable(true), visible: ko.observable(true) });

    this.GridPessoaVendedores = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });

    this.AdicionarVendedor = PropertyEntity({ eventClick: adicionarPessoaVendedorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: !_FormularioSomenteLeitura });
    this.AtualizarVendedor = PropertyEntity({ eventClick: atualizarPessoaVendedorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.ExcluirVendedor = PropertyEntity({ eventClick: excluirPessoaVendedorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.CancelarVendedor = PropertyEntity({ eventClick: LimparCamposPessoaVendedores, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
};

//*******EVENTOS*******

function loadPessoaVendedor() {
    _pessoaVendedores = new PessoaVendedor();
    KoBindings(_pessoaVendedores, "knockoutVendedor");

    new BuscarTiposdeCarga(_pessoaVendedores.TipoDeCarga);
    new BuscarFuncionario(_pessoaVendedores.Funcionario);

    loadGridPessoaVendedor();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _pessoaVendedores.TipoDeCarga.visible(true);
        _pessoaVendedores.TipoDeCarga.required(true);
    }
}

function loadGridPessoaVendedor() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarPessoaVendedor, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoFuncionario", visible: false },
        { data: "CodigoTipoDeCarga", visible: false },
        { data: "Funcionario", title: Localization.Resources.Pessoas.Pessoa.Funcionario, width: "40%", className: "text-align-left" },
        { data: "TipoDeCarga", title: Localization.Resources.Pessoas.Pessoa.TipoDeCarga, width: "40%", className: "text-align-left", visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador },
        { data: "PercentualComissao", title: Localization.Resources.Pessoas.Pessoa.PercentualComissao, width: "15%", className: "text-align-right" },
        { data: "DataInicioVigencia", title: Localization.Resources.Pessoas.Pessoa.DataInicioVigencia, width: "15%", className: "text-align-center" },
        { data: "DataFimVigencia", title: Localization.Resources.Pessoas.Pessoa.DataFimVigencia, width: "15%", className: "text-align-center" }
    ];

    _gridPessoaVendedores = new BasicDataTable(_pessoaVendedores.GridPessoaVendedores.idGrid, header, menuOpcoes);

    recarregarGridPessoaVendedores();
}

function adicionarPessoaVendedorClick() {
    var valido = ValidarCamposObrigatorios(_pessoaVendedores);

    if (Globalize.parseFloat(_pessoaVendedores.PercentualComissao.val()) <= 0) {
        valido = false;
        _pessoaVendedores.PercentualComissao.requiredClass("form-control");
        _pessoaVendedores.PercentualComissao.requiredClass("form-control is-invalid");
    }

    if (valido) {
        var existe = false;
        $.each(_pessoa.ListaVendedores.list, function (i, pessoaVendedor) {
            if (pessoaVendedor.CodigoFuncionario.val == _pessoaVendedores.Funcionario.codEntity()) {
                existe = true;
                return;
            }
        });

        if (existe) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.Pessoa.FuncionarioJaExistente, Localization.Resources.Pessoas.Pessoa.FuncionarioJaEstaCadastrado.format(_pessoaVendedores.Funcionario.val()));
            return;
        }

        var pessoaVendedor = new PessoaVendedorMap();
        pessoaVendedor.Codigo.val = guid();
        pessoaVendedor.CodigoFuncionario.val = _pessoaVendedores.Funcionario.codEntity();
        pessoaVendedor.Funcionario.val = _pessoaVendedores.Funcionario.val();
        pessoaVendedor.CodigoTipoDeCarga.val = _pessoaVendedores.TipoDeCarga.codEntity();
        pessoaVendedor.TipoDeCarga.val = _pessoaVendedores.TipoDeCarga.val();
        pessoaVendedor.PercentualComissao.val = _pessoaVendedores.PercentualComissao.val();
        pessoaVendedor.DataInicioVigencia.val = _pessoaVendedores.DataInicioVigencia.val();
        pessoaVendedor.DataFimVigencia.val = _pessoaVendedores.DataFimVigencia.val();

        _pessoa.ListaVendedores.list.push(pessoaVendedor);
        recarregarGridPessoaVendedores();
        $("#" + _pessoaVendedores.Funcionario.id).focus();
        LimparCamposPessoaVendedores();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function atualizarPessoaVendedorClick() {
    var valido = ValidarCamposObrigatorios(_pessoaVendedores);

    if (Globalize.parseFloat(_pessoaVendedores.PercentualComissao.val()) <= 0) {
        valido = false;
        _pessoaVendedores.PercentualComissao.requiredClass("form-control");
        _pessoaVendedores.PercentualComissao.requiredClass("form-control is-invalid");
    }

    if (valido) {
        $.each(_pessoa.ListaVendedores.list, function (i, pessoaVendedor) {
            if (pessoaVendedor.Codigo.val == _pessoaVendedores.CodigoVendedor.val()) {
                pessoaVendedor.PercentualComissao.val = _pessoaVendedores.PercentualComissao.val();
                pessoaVendedor.DataInicioVigencia.val = _pessoaVendedores.DataInicioVigencia.val();
                pessoaVendedor.DataFimVigencia.val = _pessoaVendedores.DataFimVigencia.val();
                pessoaVendedor.TipoDeCarga.val = _pessoaVendedores.TipoDeCarga.val();
                pessoaVendedor.CodigoTipoDeCarga.val = _pessoaVendedores.TipoDeCarga.codEntity();

                return false;
            }
        });
        recarregarGridPessoaVendedores();
        LimparCamposPessoaVendedores();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function excluirPessoaVendedorClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pessoas.Pessoa.RealmenteDesejaExcluirVendedor + _pessoaVendedores.Funcionario.val() + "?", function () {
        var listaAtualizada = new Array();
        $.each(_pessoa.ListaVendedores.list, function (i, pessoaVendedor) {
            if (pessoaVendedor.Codigo.val != _pessoaVendedores.CodigoVendedor.val()) {
                listaAtualizada.push(pessoaVendedor);
            }
        });
        _pessoa.ListaVendedores.list = listaAtualizada;
        recarregarGridPessoaVendedores();
        LimparCamposPessoaVendedores();
    });
}

//*******MÉTODOS*******

function recarregarGridPessoaVendedores() {
    var data = new Array();
    $.each(_pessoa.ListaVendedores.list, function (i, pessoa) {
        var pessoaVendedor = new Object();

        pessoaVendedor.Codigo = pessoa.Codigo.val;
        pessoaVendedor.CodigoFuncionario = pessoa.CodigoFuncionario.val;
        pessoaVendedor.Funcionario = pessoa.Funcionario.val;
        pessoaVendedor.CodigoTipoDeCarga = pessoa.CodigoTipoDeCarga.val;
        pessoaVendedor.TipoDeCarga = pessoa.TipoDeCarga.val;
        pessoaVendedor.PercentualComissao = pessoa.PercentualComissao.val;
        pessoaVendedor.DataInicioVigencia = pessoa.DataInicioVigencia.val;
        pessoaVendedor.DataFimVigencia = pessoa.DataFimVigencia.val;

        data.push(pessoaVendedor);
    });
    _gridPessoaVendedores.CarregarGrid(data);
}

function editarPessoaVendedor(data) {
    LimparCamposPessoaVendedores();
    $.each(_pessoa.ListaVendedores.list, function (i, pessoaVendedor) {
        if (pessoaVendedor.Codigo.val == data.Codigo) {
            _pessoaVendedores.CodigoVendedor.val(pessoaVendedor.Codigo.val);
            _pessoaVendedores.Funcionario.codEntity(pessoaVendedor.CodigoFuncionario.val);
            _pessoaVendedores.Funcionario.val(pessoaVendedor.Funcionario.val);
            _pessoaVendedores.TipoDeCarga.codEntity(pessoaVendedor.CodigoTipoDeCarga.val);
            _pessoaVendedores.TipoDeCarga.val(pessoaVendedor.TipoDeCarga.val);
            _pessoaVendedores.PercentualComissao.val(pessoaVendedor.PercentualComissao.val);
            _pessoaVendedores.DataInicioVigencia.val(pessoaVendedor.DataInicioVigencia.val);
            _pessoaVendedores.DataFimVigencia.val(pessoaVendedor.DataFimVigencia.val);
            _pessoaVendedores.Funcionario.enable(false);

            return false;
        }
    });

    _pessoaVendedores.AdicionarVendedor.visible(false);
    _pessoaVendedores.AtualizarVendedor.visible(true);
    _pessoaVendedores.ExcluirVendedor.visible(true);
    _pessoaVendedores.CancelarVendedor.visible(true);
}

function LimparCamposPessoaVendedores() {
    LimparCampoEntity(_pessoaVendedores.Funcionario);
    LimparCampoEntity(_pessoaVendedores.TipoDeCarga);
    _pessoaVendedores.PercentualComissao.val("0,00000");
    _pessoaVendedores.PercentualComissao.requiredClass("form-control");
    _pessoaVendedores.DataInicioVigencia.val("");
    _pessoaVendedores.DataFimVigencia.val("");

    _pessoaVendedores.Funcionario.enable(true);

    _pessoaVendedores.AdicionarVendedor.visible(true);
    _pessoaVendedores.AtualizarVendedor.visible(false);
    _pessoaVendedores.ExcluirVendedor.visible(false);
    _pessoaVendedores.CancelarVendedor.visible(false);
}