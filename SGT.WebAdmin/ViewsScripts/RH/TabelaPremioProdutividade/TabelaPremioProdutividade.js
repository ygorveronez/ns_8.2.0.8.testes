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
/// <reference path="../../Consultas/GrupoPessoa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _tabelaPremioProdutividade;
var _pesquisaTabelaPremioProdutividade;
var _gridTabelaPremioProdutividade;
var _gridGrupoPessoas;

var TabelaPremioProdutividade = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Percentual = PropertyEntity({ getType: typesKnockout.decimal, required: ko.observable(true), text: ko.observable("*% Prêmio Produtividade:"), maxlength: 10, visible: ko.observable(true), enable: false });
    this.DataInicio = PropertyEntity({ text: "*Data Vigência de: ", getType: typesKnockout.date, required: true });
    this.DataFim = PropertyEntity({ text: "*Até: ", getType: typesKnockout.date, required: true });
    this.Status = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true });

    this.Grid = PropertyEntity({ type: types.local });
    this.GrupoPessoas = PropertyEntity({ type: types.event, text: "Adicionar Grupo de Pessoa", idBtnSearch: guid() });

    this.GruposPessoas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaTabelaPremioProdutividade = function () {
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas: ", idBtnSearch: guid(), visible: true });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTabelaPremioProdutividade.CarregarGrid();
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

//*******EVENTOS*******
function loadTabelaPremioProdutividade() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaTabelaPremioProdutividade = new PesquisaTabelaPremioProdutividade();
    KoBindings(_pesquisaTabelaPremioProdutividade, "knockoutPesquisaTabelaPremioProdutividade", false, _pesquisaTabelaPremioProdutividade.Pesquisar.id);

    // Instancia objeto principal
    _tabelaPremioProdutividade = new TabelaPremioProdutividade();
    KoBindings(_tabelaPremioProdutividade, "knockoutTabelaPremioProdutividade");

    HeaderAuditoria("TabelaPremioProdutividade", _tabelaPremioProdutividade);

    new BuscarGruposPessoas(_pesquisaTabelaPremioProdutividade.GrupoPessoas);

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirGrupoPessoasClick(_tabelaPremioProdutividade.GrupoPessoas, data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridGrupoPessoas = new BasicDataTable(_tabelaPremioProdutividade.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarGruposPessoas(_tabelaPremioProdutividade.GrupoPessoas, null, null,_gridGrupoPessoas);

    _tabelaPremioProdutividade.GrupoPessoas.basicTable = _gridGrupoPessoas;
    _tabelaPremioProdutividade.GrupoPessoas.basicTable.CarregarGrid(new Array());

    // Inicia busca
    buscarTabelaPremioProdutividade();
    RecarregarGridGrupoPessoas();
}

function RecarregarGridGrupoPessoas() {
    if (_tabelaPremioProdutividade.GruposPessoas.val() != null)
        _gridGrupoPessoas.CarregarGrid(_tabelaPremioProdutividade.GruposPessoas.val());
    else
        _gridGrupoPessoas.CarregarGrid(new Array());
}

function ExcluirGrupoPessoasClick(knoutGrupoPessoas, data) {

    var grupoPessoasGrid = knoutGrupoPessoas.basicTable.BuscarRegistros();

    for (var i = 0; i < grupoPessoasGrid.length; i++) {
        if (data.Codigo == grupoPessoasGrid[i].Codigo) {
            grupoPessoasGrid.splice(i, 1);
            break;
        }
    }

    knoutGrupoPessoas.basicTable.CarregarGrid(grupoPessoasGrid);
}

function adicionarClick(e, sender) {
    obterTabelaPremioProdutividadeSalvar();
    Salvar(_tabelaPremioProdutividade, "TabelaPremioProdutividade/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridTabelaPremioProdutividade.CarregarGrid();
                limparCamposTabelaPremioProdutividade();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    obterTabelaPremioProdutividadeSalvar();
    Salvar(_tabelaPremioProdutividade, "TabelaPremioProdutividade/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTabelaPremioProdutividade.CarregarGrid();
                limparCamposTabelaPremioProdutividade();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_tabelaPremioProdutividade, "TabelaPremioProdutividade/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTabelaPremioProdutividade.CarregarGrid();
                    limparCamposTabelaPremioProdutividade();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTabelaPremioProdutividade();
}

function editarTabelaPremioProdutividadeClick(itemGrid) {
    // Limpa os campos
    limparCamposTabelaPremioProdutividade();

    // Seta o codigo do objeto
    _tabelaPremioProdutividade.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_tabelaPremioProdutividade, "TabelaPremioProdutividade/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                RecarregarGridGrupoPessoas();
                // Esconde pesqusia
                _pesquisaTabelaPremioProdutividade.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _tabelaPremioProdutividade.Atualizar.visible(true);
                _tabelaPremioProdutividade.Excluir.visible(true);
                _tabelaPremioProdutividade.Cancelar.visible(true);
                _tabelaPremioProdutividade.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******
function buscarTabelaPremioProdutividade() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTabelaPremioProdutividadeClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridTabelaPremioProdutividade = new GridView(_pesquisaTabelaPremioProdutividade.Pesquisar.idGrid, "TabelaPremioProdutividade/Pesquisa", _pesquisaTabelaPremioProdutividade, menuOpcoes, null);
    _gridTabelaPremioProdutividade.CarregarGrid();
}

function obterTabelaPremioProdutividadeSalvar() {
    _tabelaPremioProdutividade.GruposPessoas.val(JSON.stringify(_tabelaPremioProdutividade.GrupoPessoas.basicTable.BuscarRegistros()));
}

function limparCamposTabelaPremioProdutividade() {
    _tabelaPremioProdutividade.Atualizar.visible(false);
    _tabelaPremioProdutividade.Cancelar.visible(false);
    _tabelaPremioProdutividade.Excluir.visible(false);
    _tabelaPremioProdutividade.Adicionar.visible(true);
    LimparCampos(_tabelaPremioProdutividade);
    _tabelaPremioProdutividade.GruposPessoas.list = new Array();
    _gridGrupoPessoas.CarregarGrid(new Array());
}