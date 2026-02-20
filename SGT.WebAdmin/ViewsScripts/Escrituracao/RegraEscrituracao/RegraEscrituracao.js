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


//*******MAPEAMENTO KNOUCKOUT*******

var _regraEscrituracao;
var _pesquisaRegraEscrituracao;
var _gridRegraEscrituracao;

var _origemDestinoFilia = [
    { value: true, text: "Filial" },
    { value: false, text: "Não Filial" }
];

var RegraEscrituracao = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });

    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid() });
    this.OrigemFilial = PropertyEntity({ text: "Origem: ", val: ko.observable(true), options: _origemDestinoFilia, def: true });
    this.DestinoFilial = PropertyEntity({ text: "Destino: ", val: ko.observable(true), options: _origemDestinoFilia, def: true });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaRegraEscrituracao = function () {
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _statusPesquisa, def: true });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraEscrituracao.CarregarGrid();
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
function loadRegraEscrituracao() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaRegraEscrituracao = new PesquisaRegraEscrituracao();
    KoBindings(_pesquisaRegraEscrituracao, "knockoutPesquisaRegraEscrituracao", false, _pesquisaRegraEscrituracao.Pesquisar.id);

    // Instancia objeto principal
    _regraEscrituracao = new RegraEscrituracao();
    KoBindings(_regraEscrituracao, "knockoutRegraEscrituracao");

    HeaderAuditoria("RegraEscrituracao", _regraEscrituracao);

    // Instancia buscas
    BuscarClientes(_regraEscrituracao.Remetente);
    BuscarClientes(_regraEscrituracao.Destinatario);

    // Inicia busca
    BuscarRegraEscrituracao();
}

function adicionarClick(e, sender) {
    Salvar(_regraEscrituracao, "RegraEscrituracao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridRegraEscrituracao.CarregarGrid();
                LimparCamposRegraEscrituracao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_regraEscrituracao, "RegraEscrituracao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridRegraEscrituracao.CarregarGrid();
                LimparCamposRegraEscrituracao();
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
        ExcluirPorCodigo(_regraEscrituracao, "RegraEscrituracao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridRegraEscrituracao.CarregarGrid();
                    LimparCamposRegraEscrituracao();
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
    LimparCamposRegraEscrituracao();
}

function editarRegraEscrituracaoClick(itemGrid) {
    // Limpa os campos
    LimparCamposRegraEscrituracao();

    // Seta o codigo do objeto
    _regraEscrituracao.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_regraEscrituracao, "RegraEscrituracao/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaRegraEscrituracao.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _regraEscrituracao.Atualizar.visible(true);
                _regraEscrituracao.Excluir.visible(true);
                _regraEscrituracao.Cancelar.visible(true);
                _regraEscrituracao.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function BuscarRegraEscrituracao() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegraEscrituracaoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridRegraEscrituracao = new GridView(_pesquisaRegraEscrituracao.Pesquisar.idGrid, "RegraEscrituracao/Pesquisa", _pesquisaRegraEscrituracao, menuOpcoes, null);
    _gridRegraEscrituracao.CarregarGrid();
}

function LimparCamposRegraEscrituracao() {
    _regraEscrituracao.Atualizar.visible(false);
    _regraEscrituracao.Cancelar.visible(false);
    _regraEscrituracao.Excluir.visible(false);
    _regraEscrituracao.Adicionar.visible(true);
    LimparCampos(_regraEscrituracao);
}