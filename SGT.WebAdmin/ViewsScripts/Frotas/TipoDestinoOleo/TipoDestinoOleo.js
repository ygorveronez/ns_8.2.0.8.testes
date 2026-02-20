/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDTipoDestinoOleo;
var _tipoDestinoOleo;
var _pesquisaTipoDestinoOleo;
var _gridTipoDestinoOleo;

/*
 * Declaração das Classes
 */

var CRUDTipoDestinoOleo = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var TipoDestinoOleo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
};

var PesquisaTipoDestinoOleo = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridTipoDestinoOleo, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridTipoDestinoOleo() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridTipoDestinoOleo = new GridViewExportacao(_pesquisaTipoDestinoOleo.Pesquisar.idGrid, "TipoDestinoOleo/Pesquisa", _pesquisaTipoDestinoOleo, menuOpcoes);
    _gridTipoDestinoOleo.CarregarGrid();
}

function LoadTipoDestinoOleo() {
    _tipoDestinoOleo = new TipoDestinoOleo();
    KoBindings(_tipoDestinoOleo, "knockoutTipoDestinoOleo");

    _CRUDTipoDestinoOleo = new CRUDTipoDestinoOleo();
    KoBindings(_CRUDTipoDestinoOleo, "knockoutCRUDTipoDestinoOleo");

    _pesquisaTipoDestinoOleo = new PesquisaTipoDestinoOleo();
    KoBindings(_pesquisaTipoDestinoOleo, "knockoutPesquisaTipoDestinoOleo", false, _pesquisaTipoDestinoOleo.Pesquisar.id);

    LoadGridTipoDestinoOleo();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AdicionarClick(e, sender) {
    Salvar(_tipoDestinoOleo, "TipoDestinoOleo/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridTipoDestinoOleo();
                LimparCamposTipoDestinoOleo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_tipoDestinoOleo, "TipoDestinoOleo/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridTipoDestinoOleo();
                LimparCamposTipoDestinoOleo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposTipoDestinoOleo();
}

function EditarClick(registroSelecionado) {
    LimparCamposTipoDestinoOleo();

    _tipoDestinoOleo.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_tipoDestinoOleo, "TipoDestinoOleo/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaTipoDestinoOleo.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                ControlarBotoesHabilitados(isEdicao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function ExcluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_tipoDestinoOleo, "TipoDestinoOleo/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso!");

                    RecarregarGridTipoDestinoOleo();
                    LimparCamposTipoDestinoOleo();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function ControlarBotoesHabilitados(isEdicao) {
    _CRUDTipoDestinoOleo.Atualizar.visible(isEdicao);
    _CRUDTipoDestinoOleo.Excluir.visible(isEdicao);
    _CRUDTipoDestinoOleo.Cancelar.visible(isEdicao);
    _CRUDTipoDestinoOleo.Adicionar.visible(!isEdicao);
}

function LimparCamposTipoDestinoOleo() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_tipoDestinoOleo);
}

function RecarregarGridTipoDestinoOleo() {
    _gridTipoDestinoOleo.CarregarGrid();
}