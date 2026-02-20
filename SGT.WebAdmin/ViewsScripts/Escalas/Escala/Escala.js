/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Consultas/CentroDesarregamento.js" />
/// <reference path="../../Enumeradores/ClassificacaoEscala.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _escala;
var _pesquisaEscala;
var _gridEscala;

var _classificacaoEscala = [
    { text: "A", value: EnumClassificacaoEscala.A },
    { text: "B", value: EnumClassificacaoEscala.B },
    { text: "C", value: EnumClassificacaoEscala.C },
];

var _pesquisaClassificacaoEscala = [
    { text: "Todas", value: "" },
    { text: "A", value: EnumClassificacaoEscala.A },
    { text: "B", value: EnumClassificacaoEscala.B },
    { text: "C", value: EnumClassificacaoEscala.C },
];

var Escala = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Classificacao = PropertyEntity({ text: "Classificação: ", val: ko.observable(EnumClassificacaoEscala.A), options: _classificacaoEscala, def: EnumClassificacaoEscala.A, visible: ko.observable(true) });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.CentroCarregamento = PropertyEntity({ text: "*Centro de Carregamento (origem):", required: ko.observable(true), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroDescarregamento = PropertyEntity({ text: "*Centro de Descarregamento (destino):", required: ko.observable(true), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaEscala = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status:", val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.Classificacao = PropertyEntity({ text: "Classificação: ", val: ko.observable(""), options: _pesquisaClassificacaoEscala, def: "" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridEscala.CarregarGrid();
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
function loadEscalas() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaEscala = new PesquisaEscala();
    KoBindings(_pesquisaEscala, "knockoutPesquisaEscalas", false, _pesquisaEscala.Pesquisar.id);

    // Instancia ProdutoAvaria
    _escala = new Escala();
    KoBindings(_escala, "knockoutEscalas");

    HeaderAuditoria("Escala", _escala);

    // Instancia buscas
    new BuscarCentrosCarregamento(_escala.CentroCarregamento);
    new BuscarCentrosDescarregamento(_escala.CentroDescarregamento);

    // Inicia busca
    buscarEscala();
}

function adicionarClick(e, sender) {
    Salvar(_escala, "Escala/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridEscala.CarregarGrid();
                limparCamposEscala();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_escala, "Escala/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridEscala.CarregarGrid();
                limparCamposEscala();
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
        ExcluirPorCodigo(_escala, "Escala/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridEscala.CarregarGrid();
                    limparCamposEscala();
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
    limparCamposEscala();
}

function editarEscalaClick(itemGrid) {
    // Limpa os campos
    limparCamposEscala();

    // Seta o codigo do ProdutoAvaria
    _escala.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_escala, "Escala/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaEscala.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _escala.Atualizar.visible(true);
                _escala.Excluir.visible(true);
                _escala.Cancelar.visible(true);
                _escala.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarEscala() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarEscalaClick, tamanho: "10", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    // Inicia Grid de busca
    _gridEscala = new GridView(_pesquisaEscala.Pesquisar.idGrid, "Escala/Pesquisa", _pesquisaEscala, menuOpcoes);
    _gridEscala.CarregarGrid();
}

function limparCamposEscala() {
    _escala.Atualizar.visible(false);
    _escala.Cancelar.visible(false);
    _escala.Excluir.visible(false);
    _escala.Adicionar.visible(true);
    LimparCampos(_escala);
}