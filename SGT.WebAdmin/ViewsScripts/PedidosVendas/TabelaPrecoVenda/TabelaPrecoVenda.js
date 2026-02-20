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
/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/GrupoProdutoTMS.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTabelaPrecoVenda;
var _tabelaPrecoVenda;
var _pesquisaTabelaPrecoVenda;

var PesquisaTabelaPrecoVenda = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo Produto:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTabelaPrecoVenda.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var TabelaPrecoVenda = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.DataInicioVigencia = PropertyEntity({ text: "*Data Início Vigência: ", getType: typesKnockout.date, required: ko.observable(true) });
    this.DataFimVigencia = PropertyEntity({ text: "*Data Fim Vigência: ", getType: typesKnockout.date, required: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "*Valor:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: ko.observable(true) });

    this.Status = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.TipoPessoa = PropertyEntity({ text: "Tipo Pessoa:", val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, enable: ko.observable(true), eventChange: function () { tipoPessoaTabelaPrecoChange() } });

    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pessoa:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Grupo Pessoa:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Grupo Produto:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
}

var CRUDTabelaPrecoVenda = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadTabelaPrecoVenda() {
    _tabelaPrecoVenda = new TabelaPrecoVenda();
    KoBindings(_tabelaPrecoVenda, "knockoutCadastroTabelaPrecoVenda");

    HeaderAuditoria("TabelaPrecoVenda", _tabelaPrecoVenda);

    _crudTabelaPrecoVenda = new CRUDTabelaPrecoVenda();
    KoBindings(_crudTabelaPrecoVenda, "knockoutCRUDTabelaPrecoVenda");

    _pesquisaTabelaPrecoVenda = new PesquisaTabelaPrecoVenda();
    KoBindings(_pesquisaTabelaPrecoVenda, "knockoutPesquisaTabelaPrecoVenda", false, _pesquisaTabelaPrecoVenda.Pesquisar.id);

    new BuscarGruposProdutosTMS(_pesquisaTabelaPrecoVenda.GrupoProduto, null);
    new BuscarClientes(_tabelaPrecoVenda.Pessoa);
    new BuscarGruposPessoas(_tabelaPrecoVenda.GrupoPessoa);
    new BuscarGruposProdutosTMS(_tabelaPrecoVenda.GrupoProduto, null);

    buscarTabelaPrecoVenda();
}

function adicionarClick(e, sender) {
    Salvar(_tabelaPrecoVenda, "TabelaPrecoVenda/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTabelaPrecoVenda.CarregarGrid();
                limparCamposTabelaPrecoVenda();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tabelaPrecoVenda, "TabelaPrecoVenda/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTabelaPrecoVenda.CarregarGrid();
                limparCamposTabelaPrecoVenda();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a tabela de preço?", function () {
        ExcluirPorCodigo(_tabelaPrecoVenda, "TabelaPrecoVenda/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridTabelaPrecoVenda.CarregarGrid();
                limparCamposTabelaPrecoVenda();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTabelaPrecoVenda();
}

function tipoPessoaTabelaPrecoChange() {
    if (_tabelaPrecoVenda.TipoPessoa.val() == EnumProdutoServico.Produto) {
        _tabelaPrecoVenda.GrupoPessoa.visible(false);
        _tabelaPrecoVenda.GrupoPessoa.required(false);
        _tabelaPrecoVenda.Pessoa.visible(true);
        _tabelaPrecoVenda.Pessoa.required(true);
        LimparCampoEntity(_tabelaPrecoVenda.GrupoPessoa);
    }
    else {
        _tabelaPrecoVenda.GrupoPessoa.visible(true);
        _tabelaPrecoVenda.GrupoPessoa.required(true);
        _tabelaPrecoVenda.Pessoa.visible(false);
        _tabelaPrecoVenda.Pessoa.required(false);
        LimparCampoEntity(_tabelaPrecoVenda.Pessoa);
    }
}

//*******MÉTODOS*******


function buscarTabelaPrecoVenda() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTabelaPrecoVenda, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTabelaPrecoVenda = new GridView(_pesquisaTabelaPrecoVenda.Pesquisar.idGrid, "TabelaPrecoVenda/Pesquisa", _pesquisaTabelaPrecoVenda, menuOpcoes, null);
    _gridTabelaPrecoVenda.CarregarGrid();
}

function editarTabelaPrecoVenda(tabelaPrecoVendaGrid) {
    limparCamposTabelaPrecoVenda();
    _tabelaPrecoVenda.Codigo.val(tabelaPrecoVendaGrid.Codigo);
    BuscarPorCodigo(_tabelaPrecoVenda, "TabelaPrecoVenda/BuscarPorCodigo", function (arg) {
        _pesquisaTabelaPrecoVenda.ExibirFiltros.visibleFade(false);
        _crudTabelaPrecoVenda.Atualizar.visible(true);
        _crudTabelaPrecoVenda.Cancelar.visible(true);
        _crudTabelaPrecoVenda.Excluir.visible(true);
        _crudTabelaPrecoVenda.Adicionar.visible(false);

        tipoPessoaTabelaPrecoChange();
    }, null);
}

function limparCamposTabelaPrecoVenda() {
    _crudTabelaPrecoVenda.Atualizar.visible(false);
    _crudTabelaPrecoVenda.Cancelar.visible(false);
    _crudTabelaPrecoVenda.Excluir.visible(false);
    _crudTabelaPrecoVenda.Adicionar.visible(true);
    LimparCampos(_tabelaPrecoVenda);

    tipoPessoaTabelaPrecoChange();
}