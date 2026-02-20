/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDGrupoDespesa;
var _grupoDespesa;
var _pesquisaGrupoDespesa;
var _gridGrupoDespesa;

/*
 * Declaração das Classes
 */

var CRUDGrupoDespesa = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var GrupoDespesa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 400 });
    this.Situacao = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
};

var PesquisaGrupoDespesa = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridGrupoDespesa, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridGrupoDespesa() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "GrupoDespesaFinanceira/ExportarPesquisa", titulo: "Grupos de Despesas" };

    _gridGrupoDespesa = new GridViewExportacao(_pesquisaGrupoDespesa.Pesquisar.idGrid, "GrupoDespesaFinanceira/Pesquisa", _pesquisaGrupoDespesa, menuOpcoes, configuracoesExportacao);
    _gridGrupoDespesa.CarregarGrid();
}

function LoadGrupoDespesa() {
    _grupoDespesa = new GrupoDespesa();
    KoBindings(_grupoDespesa, "knockoutGrupoDespesa");

    HeaderAuditoria("GrupoDespesa", _grupoDespesa);

    _CRUDGrupoDespesa = new CRUDGrupoDespesa();
    KoBindings(_CRUDGrupoDespesa, "knockoutCRUDGrupoDespesa");

    _pesquisaGrupoDespesa = new PesquisaGrupoDespesa();
    KoBindings(_pesquisaGrupoDespesa, "knockoutPesquisaGrupoDespesa", false, _pesquisaGrupoDespesa.Pesquisar.id);

    LoadGridGrupoDespesa();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AdicionarClick(e, sender) {
    Salvar(_grupoDespesa, "GrupoDespesaFinanceira/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridGrupoDespesa();
                LimparCamposGrupoDespesa();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_grupoDespesa, "GrupoDespesaFinanceira/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridGrupoDespesa();
                LimparCamposGrupoDespesa();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposGrupoDespesa();
}

function EditarClick(registroSelecionado) {
    LimparCamposGrupoDespesa();

    _grupoDespesa.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_grupoDespesa, "GrupoDespesaFinanceira/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaGrupoDespesa.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_grupoDespesa, "GrupoDespesaFinanceira/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    RecarregarGridGrupoDespesa();
                    LimparCamposGrupoDespesa();
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
    _CRUDGrupoDespesa.Atualizar.visible(isEdicao);
    _CRUDGrupoDespesa.Excluir.visible(isEdicao);
    _CRUDGrupoDespesa.Cancelar.visible(isEdicao);
    _CRUDGrupoDespesa.Adicionar.visible(!isEdicao);
}

function LimparCamposGrupoDespesa() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_grupoDespesa);
}

function RecarregarGridGrupoDespesa() {
    _gridGrupoDespesa.CarregarGrid();
}