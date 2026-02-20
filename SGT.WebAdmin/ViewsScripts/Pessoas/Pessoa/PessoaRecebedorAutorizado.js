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

var _pessoaRecebedoresAutorizados;
var _gridPessoaRecebedoresAutorizados;

//*******MAPEAMENTO KNOUCKOUT*******

var PessoaRecebedorAutorizadoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Nome = PropertyEntity({ type: types.map, val: "" });
    this.CPF = PropertyEntity({ type: types.map, val: "" });
    this.Foto = PropertyEntity({ type: types.map, val: "" });
}

var PessoaRecebedorAutorizado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Nome = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.NomeCompleto.getRequiredFieldDescription()), required: true, maxlength: 100, enable: ko.observable(true) });
    this.CPF = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.CPF.getRequiredFieldDescription()), required: true, getType: typesKnockout.cpf, enable: ko.observable(true), visible: ko.observable(true) });
    this.Foto = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Foto.getRequiredFieldDescription()), required: true, enable: ko.observable(true) });

    this.GridPessoaRecebedoresAutorizados = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });

    this.AdicionarRecebedorAutorizado = PropertyEntity({ eventClick: adicionarPessoaRecebedorAutorizadoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: !_FormularioSomenteLeitura });
    this.AtualizarRecebedorAutorizado = PropertyEntity({ eventClick: atualizarPessoaRecebedorAutorizadoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.ExcluirRecebedorAutorizado = PropertyEntity({ eventClick: excluirPessoaRecebedorAutorizadoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.CancelarRecebedorAutorizado = PropertyEntity({ eventClick: LimparCamposPessoaRecebedoresAutorizados, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
}

//*******EVENTOS*******

function loadPessoaRecebedorAutorizado() {
    _pessoaRecebedoresAutorizados = new PessoaRecebedorAutorizado();
    KoBindings(_pessoaRecebedoresAutorizados, "knockoutRecebedorAutorizado");

    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarPessoaRecebedorAutorizado, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "Nome", title: Localization.Resources.Pessoas.Pessoa.Nome, width: "40%", className: "text-align-left" },
        { data: "CPF", title: Localization.Resources.Pessoas.Pessoa.CPF, width: "40%", className: "text-align-left" },
    ];

    _gridPessoaRecebedoresAutorizados = new BasicDataTable(_pessoaRecebedoresAutorizados.GridPessoaRecebedoresAutorizados.idGrid, header, menuOpcoes);
    recarregarGridPessoaRecebedoresAutorizados();
}

function adicionarPessoaRecebedorAutorizadoClick() {
    var valido = ValidarCamposObrigatorios(_pessoaRecebedoresAutorizados);
    if (valido) {
        var existe = false;
        $.each(_pessoa.ListaRecebedoresAutorizados.list, function (i, pessoaRecebedorAutorizado) {
            if (pessoaRecebedorAutorizado.CPF.val == _pessoaRecebedoresAutorizados.CPF.val()) {
                existe = true;
                return;
            }
        });

        if (existe) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.Pessoa.RecebedorJaExistente, Localization.Resources.Pessoas.Pessoas.RecebedorJaEstaCadastrado.format(_pessoaRecebedoresAutorizados.Nome.val()));
            return;
        }

        var pessoaRecebedorAutorizado = new PessoaRecebedorAutorizadoMap();
        pessoaRecebedorAutorizado.Codigo.val = guid();
        pessoaRecebedorAutorizado.Nome.val = _pessoaRecebedoresAutorizados.Nome.val();
        pessoaRecebedorAutorizado.CPF.val = _pessoaRecebedoresAutorizados.CPF.val();
        pessoaRecebedorAutorizado.Foto.val = _pessoaRecebedoresAutorizados.Foto.val();

        _pessoa.ListaRecebedoresAutorizados.list.push(pessoaRecebedorAutorizado);
        recarregarGridPessoaRecebedoresAutorizados();
        LimparCamposPessoaRecebedoresAutorizados();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function atualizarPessoaRecebedorAutorizadoClick() {
    var valido = ValidarCamposObrigatorios(_pessoaRecebedoresAutorizados);
    if (valido) {
        $.each(_pessoa.ListaRecebedoresAutorizados.list, function (i, pessoaRecebedorAutorizado) {
            if (pessoaRecebedorAutorizado.Codigo.val == _pessoaRecebedoresAutorizados.Codigo.val()) {
                pessoaRecebedorAutorizado.Nome.val = _pessoaRecebedoresAutorizados.Nome.val();
                pessoaRecebedorAutorizado.CPF.val = _pessoaRecebedoresAutorizados.CPF.val();
                pessoaRecebedorAutorizado.Foto.val = _pessoaRecebedoresAutorizados.Foto.val();

                return false;
            }
        });
        recarregarGridPessoaRecebedoresAutorizados();
        LimparCamposPessoaRecebedoresAutorizados();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function excluirPessoaRecebedorAutorizadoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pessoas.Pessoa.RealmenteDesejaExcluirRecebedorAutorizado + _pessoaRecebedoresAutorizados.Nome.val() + "?", function () {
        var listaAtualizada = new Array();
        $.each(_pessoa.ListaRecebedoresAutorizados.list, function (i, pessoaRecebedorAutorizado) {
            if (pessoaRecebedorAutorizado.Codigo.val != _pessoaRecebedoresAutorizados.Codigo.val()) {
                listaAtualizada.push(pessoaRecebedorAutorizado);
            }
        });
        _pessoa.ListaRecebedoresAutorizados.list = listaAtualizada;
        recarregarGridPessoaRecebedoresAutorizados();
        LimparCamposPessoaRecebedoresAutorizados();
    });
}

//*******MÉTODOS*******

function recarregarGridPessoaRecebedoresAutorizados() {
    var data = new Array();
    $.each(_pessoa.ListaRecebedoresAutorizados.list, function (i, pessoa) {
        var pessoaRecebedorAutorizado = new Object();

        pessoaRecebedorAutorizado.Codigo = pessoa.Codigo.val;
        pessoaRecebedorAutorizado.Nome = pessoa.Nome.val;
        pessoaRecebedorAutorizado.CPF = pessoa.CPF.val;
        pessoaRecebedorAutorizado.Foto = pessoa.Foto.val;

        data.push(pessoaRecebedorAutorizado);
    });
    _gridPessoaRecebedoresAutorizados.CarregarGrid(data);
}

function editarPessoaRecebedorAutorizado(data) {
    LimparCamposPessoaRecebedoresAutorizados();
    $.each(_pessoa.ListaRecebedoresAutorizados.list, function (i, pessoaRecebedorAutorizado) {
        if (pessoaRecebedorAutorizado.Codigo.val == data.Codigo) {
            _pessoaRecebedoresAutorizados.Codigo.val(pessoaRecebedorAutorizado.Codigo.val);
            _pessoaRecebedoresAutorizados.Nome.val(pessoaRecebedorAutorizado.Nome.val);
            _pessoaRecebedoresAutorizados.CPF.val(pessoaRecebedorAutorizado.CPF.val);
            _pessoaRecebedoresAutorizados.Foto.val(pessoaRecebedorAutorizado.Foto.val);

            return false;
        }
    });

    _pessoaRecebedoresAutorizados.AdicionarRecebedorAutorizado.visible(false);
    _pessoaRecebedoresAutorizados.AtualizarRecebedorAutorizado.visible(true);
    _pessoaRecebedoresAutorizados.ExcluirRecebedorAutorizado.visible(true);
    _pessoaRecebedoresAutorizados.CancelarRecebedorAutorizado.visible(true);
}

function LimparCamposPessoaRecebedoresAutorizados() {
    _pessoaRecebedoresAutorizados.Codigo.val(0);
    _pessoaRecebedoresAutorizados.Nome.val("");
    _pessoaRecebedoresAutorizados.CPF.val("");
    _pessoaRecebedoresAutorizados.Foto.val("");
    $('#nomeFoto').val('');

    _pessoaRecebedoresAutorizados.AdicionarRecebedorAutorizado.visible(true);
    _pessoaRecebedoresAutorizados.AtualizarRecebedorAutorizado.visible(false);
    _pessoaRecebedoresAutorizados.ExcluirRecebedorAutorizado.visible(false);
    _pessoaRecebedoresAutorizados.CancelarRecebedorAutorizado.visible(false);
}