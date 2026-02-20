/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDTipoDespesa;
var _tipoDespesa;
var _pesquisaTipoDespesa;
var _gridTipoDespesa;

/*
 * Declaração das Classes
 */

var CRUDTipoDespesa = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var TipoDespesa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 400 });
    this.Situacao = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });

    this.GrupoDespesa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Grupo de Despesas:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: true });
};

var PesquisaTipoDespesa = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.GrupoDespesa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Despesas:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridTipoDespesa, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridTipoDespesa() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "TipoDespesaFinanceira/ExportarPesquisa", titulo: "Tipos de Despesas" };

    _gridTipoDespesa = new GridViewExportacao(_pesquisaTipoDespesa.Pesquisar.idGrid, "TipoDespesaFinanceira/Pesquisa", _pesquisaTipoDespesa, menuOpcoes, configuracoesExportacao);
    _gridTipoDespesa.CarregarGrid();
}

function LoadTipoDespesa() {
    _tipoDespesa = new TipoDespesa();
    KoBindings(_tipoDespesa, "knockoutTipoDespesa");

    HeaderAuditoria("TipoDespesa", _tipoDespesa);

    _CRUDTipoDespesa = new CRUDTipoDespesa();
    KoBindings(_CRUDTipoDespesa, "knockoutCRUDTipoDespesa");

    _pesquisaTipoDespesa = new PesquisaTipoDespesa();
    KoBindings(_pesquisaTipoDespesa, "knockoutPesquisaTipoDespesa", false, _pesquisaTipoDespesa.Pesquisar.id);

    new BuscarGrupoDespesaFinanceira(_pesquisaTipoDespesa.GrupoDespesa);
    new BuscarGrupoDespesaFinanceira(_tipoDespesa.GrupoDespesa);

    LoadGridTipoDespesa();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AdicionarClick(e, sender) {
    Salvar(_tipoDespesa, "TipoDespesaFinanceira/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridTipoDespesa();
                LimparCamposTipoDespesa();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_tipoDespesa, "TipoDespesaFinanceira/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridTipoDespesa();
                LimparCamposTipoDespesa();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposTipoDespesa();
}

function EditarClick(registroSelecionado) {
    LimparCamposTipoDespesa();

    _tipoDespesa.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_tipoDespesa, "TipoDespesaFinanceira/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaTipoDespesa.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_tipoDespesa, "TipoDespesaFinanceira/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    RecarregarGridTipoDespesa();
                    LimparCamposTipoDespesa();
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
    _CRUDTipoDespesa.Atualizar.visible(isEdicao);
    _CRUDTipoDespesa.Excluir.visible(isEdicao);
    _CRUDTipoDespesa.Cancelar.visible(isEdicao);
    _CRUDTipoDespesa.Adicionar.visible(!isEdicao);
}

function LimparCamposTipoDespesa() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_tipoDespesa);
}

function RecarregarGridTipoDespesa() {
    _gridTipoDespesa.CarregarGrid();
}