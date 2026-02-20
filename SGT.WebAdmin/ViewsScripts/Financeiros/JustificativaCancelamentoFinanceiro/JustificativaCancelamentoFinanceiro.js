var _CRUDJustificativaCancelamentoFinanceiro;
var _justificativaCancelamentoFinanceiro;
var _pesquisaJustificativaCancelamentoFinanceiro;
var _gridJustificativaCancelamentoFinanceiro;

var CRUDJustificativaCancelamentoFinanceiro = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var JustificativaCancelamentoFinanceiro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 400 });
    this.Situacao = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
};

var PesquisaJustificativaCancelamentoFinanceiro = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridJustificativaCancelamentoFinanceiro, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

function LoadGridJustificativaCancelamentoFinanceiro() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "JustificativaCancelamentoFinanceiro/ExportarPesquisa", titulo: "Justificativa Cancelamento Carga" };

    _gridJustificativaCancelamentoFinanceiro = new GridViewExportacao(_pesquisaJustificativaCancelamentoFinanceiro.Pesquisar.idGrid, "JustificativaCancelamentoFinanceiro/Pesquisa", _pesquisaJustificativaCancelamentoFinanceiro, menuOpcoes, configuracoesExportacao);
    _gridJustificativaCancelamentoFinanceiro.CarregarGrid();
}

function LoadJustificativaCancelamentoFinanceiro() {
    _justificativaCancelamentoFinanceiro = new JustificativaCancelamentoFinanceiro();
    KoBindings(_justificativaCancelamentoFinanceiro, "knockoutJustificativaCancelamentoFinanceiro");

    HeaderAuditoria("JustificativaCancelamentoFinanceiro", _justificativaCancelamentoFinanceiro);

    _CRUDJustificativaCancelamentoFinanceiro = new CRUDJustificativaCancelamentoFinanceiro();
    KoBindings(_CRUDJustificativaCancelamentoFinanceiro, "knockoutCRUDJustificativaCancelamentoFinanceiro");

    _pesquisaJustificativaCancelamentoFinanceiro = new PesquisaJustificativaCancelamentoFinanceiro();
    KoBindings(_pesquisaJustificativaCancelamentoFinanceiro, "knockoutPesquisaJustificativaCancelamentoFinanceiro", false, _pesquisaJustificativaCancelamentoFinanceiro.Pesquisar.id);

    LoadGridJustificativaCancelamentoFinanceiro();
}

function AdicionarClick(e, sender) {
    Salvar(_justificativaCancelamentoFinanceiro, "JustificativaCancelamentoFinanceiro/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridJustificativaCancelamentoFinanceiro();
                LimparCamposJustificativaCancelamentoFinanceiro();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_justificativaCancelamentoFinanceiro, "JustificativaCancelamentoFinanceiro/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridJustificativaCancelamentoFinanceiro();
                LimparCamposJustificativaCancelamentoFinanceiro();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposJustificativaCancelamentoFinanceiro();
}

function EditarClick(registroSelecionado) {
    LimparCamposJustificativaCancelamentoFinanceiro();

    _justificativaCancelamentoFinanceiro.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_justificativaCancelamentoFinanceiro, "JustificativaCancelamentoFinanceiro/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaJustificativaCancelamentoFinanceiro.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_justificativaCancelamentoFinanceiro, "JustificativaCancelamentoFinanceiro/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    RecarregarGridJustificativaCancelamentoFinanceiro();
                    LimparCamposJustificativaCancelamentoFinanceiro();
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
    _CRUDJustificativaCancelamentoFinanceiro.Atualizar.visible(isEdicao);
    _CRUDJustificativaCancelamentoFinanceiro.Excluir.visible(isEdicao);
    _CRUDJustificativaCancelamentoFinanceiro.Cancelar.visible(isEdicao);
    _CRUDJustificativaCancelamentoFinanceiro.Adicionar.visible(!isEdicao);
}

function LimparCamposJustificativaCancelamentoFinanceiro() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_justificativaCancelamentoFinanceiro);
}

function RecarregarGridJustificativaCancelamentoFinanceiro() {
    _gridJustificativaCancelamentoFinanceiro.CarregarGrid();
}