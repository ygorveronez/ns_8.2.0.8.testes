/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDTipoLocalManutencao;
var _tipoLocalManutencao;
var _pesquisaTipoLocalManutencao;
var _gridTipoLocalManutencao;

/*
 * Declaração das Classes
 */

var CRUDTipoLocalManutencao = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var TipoLocalManutencao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 400 });
    this.Situacao = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração:", val: ko.observable(""), def: "", visible: ko.observable(true), maxlength: 100 });
};

var PesquisaTipoLocalManutencao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridTipoLocalManutencao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridTipoLocalManutencao() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "TipoLocalManutenção/ExportarPesquisa", titulo: "Tipos de Local da Manutenção" };

    _gridTipoLocalManutencao = new GridViewExportacao(_pesquisaTipoLocalManutencao.Pesquisar.idGrid, "TipoLocalManutencao/Pesquisa", _pesquisaTipoLocalManutencao, menuOpcoes, configuracoesExportacao);
    _gridTipoLocalManutencao.CarregarGrid();
}

function LoadTipoLocalManutencao() {
    _tipoLocalManutencao = new TipoLocalManutencao();
    KoBindings(_tipoLocalManutencao, "knockoutTipoLocalManutencao");

    _CRUDTipoLocalManutencao = new CRUDTipoLocalManutencao();
    KoBindings(_CRUDTipoLocalManutencao, "knockoutCRUDTipoLocalManutencao");

    _pesquisaTipoLocalManutencao = new PesquisaTipoLocalManutencao();
    KoBindings(_pesquisaTipoLocalManutencao, "knockoutPesquisaTipoLocalManutencao", false, _pesquisaTipoLocalManutencao.Pesquisar.id);

    LoadGridTipoLocalManutencao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AdicionarClick(e, sender) {
    Salvar(_tipoLocalManutencao, "TipoLocalManutencao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridTipoLocalManutencao();
                LimparCamposTipoLocalManutencao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_tipoLocalManutencao, "TipoLocalManutencao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridTipoLocalManutencao();
                LimparCamposTipoLocalManutencao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposTipoLocalManutencao();
}

function EditarClick(registroSelecionado) {
    LimparCamposTipoLocalManutencao();

    _tipoLocalManutencao.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_tipoLocalManutencao, "TipoLocalManutencao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaTipoLocalManutencao.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_tipoLocalManutencao, "TipoLocalManutencao/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso!");

                    RecarregarGridTipoLocalManutencao();
                    LimparCamposTipoLocalManutencao();
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
    _CRUDTipoLocalManutencao.Atualizar.visible(isEdicao);
    _CRUDTipoLocalManutencao.Excluir.visible(isEdicao);
    _CRUDTipoLocalManutencao.Cancelar.visible(isEdicao);
    _CRUDTipoLocalManutencao.Adicionar.visible(!isEdicao);
}

function LimparCamposTipoLocalManutencao() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_tipoLocalManutencao);
}

function RecarregarGridTipoLocalManutencao() {
    _gridTipoLocalManutencao.CarregarGrid();
}