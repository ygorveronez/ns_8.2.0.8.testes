/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDModalidadeContratoFinanciamento;
var _ModalidadeContratoFinanciamento;
var _pesquisaModalidadeContratoFinanciamento;
var _gridModalidadeContratoFinanciamento;

/*
 * Declaração das Classes
 */

var CRUDModalidadeContratoFinanciamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var ModalidadeContratoFinanciamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
};

var PesquisaModalidadeContratoFinanciamento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridModalidadeContratoFinanciamento, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridModalidadeContratoFinanciamento() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "ModalidadeContratoFinanciamento/ExportarPesquisa", titulo: "Modalidade Contrato Financiamento" };

    _gridModalidadeContratoFinanciamento = new GridViewExportacao(_pesquisaModalidadeContratoFinanciamento.Pesquisar.idGrid, "ModalidadeContratoFinanciamento/Pesquisa", _pesquisaModalidadeContratoFinanciamento, menuOpcoes, configuracoesExportacao);
    _gridModalidadeContratoFinanciamento.CarregarGrid();
}

function LoadModalidadeContratoFinanciamento() {
    _ModalidadeContratoFinanciamento = new ModalidadeContratoFinanciamento();
    KoBindings(_ModalidadeContratoFinanciamento, "knockoutModalidadeContratoFinanciamento");

    HeaderAuditoria("ModalidadeContratoFinanciamento", _ModalidadeContratoFinanciamento);

    _CRUDModalidadeContratoFinanciamento = new CRUDModalidadeContratoFinanciamento();
    KoBindings(_CRUDModalidadeContratoFinanciamento, "knockoutCRUDModalidadeContratoFinanciamento");

    _pesquisaModalidadeContratoFinanciamento = new PesquisaModalidadeContratoFinanciamento();
    KoBindings(_pesquisaModalidadeContratoFinanciamento, "knockoutPesquisaModalidadeContratoFinanciamento", false, _pesquisaModalidadeContratoFinanciamento.Pesquisar.id);

    LoadGridModalidadeContratoFinanciamento();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AdicionarClick(e, sender) {
    Salvar(_ModalidadeContratoFinanciamento, "ModalidadeContratoFinanciamento/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridModalidadeContratoFinanciamento();
                LimparCamposModalidadeContratoFinanciamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_ModalidadeContratoFinanciamento, "ModalidadeContratoFinanciamento/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridModalidadeContratoFinanciamento();
                LimparCamposModalidadeContratoFinanciamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposModalidadeContratoFinanciamento();
}

function EditarClick(registroSelecionado) {
    LimparCamposModalidadeContratoFinanciamento();

    _ModalidadeContratoFinanciamento.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_ModalidadeContratoFinanciamento, "ModalidadeContratoFinanciamento/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaModalidadeContratoFinanciamento.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_ModalidadeContratoFinanciamento, "ModalidadeContratoFinanciamento/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    RecarregarGridModalidadeContratoFinanciamento();
                    LimparCamposModalidadeContratoFinanciamento();
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
    _CRUDModalidadeContratoFinanciamento.Atualizar.visible(isEdicao);
    _CRUDModalidadeContratoFinanciamento.Excluir.visible(isEdicao);
    _CRUDModalidadeContratoFinanciamento.Cancelar.visible(isEdicao);
    _CRUDModalidadeContratoFinanciamento.Adicionar.visible(!isEdicao);
}

function LimparCamposModalidadeContratoFinanciamento() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_ModalidadeContratoFinanciamento);
}

function RecarregarGridModalidadeContratoFinanciamento() {
    _gridModalidadeContratoFinanciamento.CarregarGrid();
}