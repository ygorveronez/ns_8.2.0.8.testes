/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDContratoFreteValorPadrao;
var _contratoFreteValorPadrao;
var _pesquisaContratoFreteValorPadrao;
var _gridContratoFreteValorPadrao;

/*
 * Declaração das Classes
 */

var CRUDContratoFreteValorPadrao = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var ContratoFreteValorPadrao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 0, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 400 });
    this.TransportadorTerceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador Terceiro:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: true });
    this.Valor = PropertyEntity({ text: "*Valor:", issue: 0, required: true, getType: typesKnockout.decimal, val: ko.observable(""), maxlength: 10 });
    this.Situacao = PropertyEntity({ text: "*Situação: ", issue: 0, val: ko.observable(true), options: _status, def: true });
    this.ApenasQuandoEmitirCIOT = PropertyEntity({ text: "Apenas quando gerar CIOT", issue: 0, val: ko.observable(false), def: false });
};

var PesquisaContratoFreteValorPadrao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridContratoFreteValorPadrao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridContratoFreteValorPadrao() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "ContratoFreteValorPadrao/ExportarPesquisa", titulo: "Grupos de Despesas" };

    _gridContratoFreteValorPadrao = new GridViewExportacao(_pesquisaContratoFreteValorPadrao.Pesquisar.idGrid, "ContratoFreteValorPadrao/Pesquisa", _pesquisaContratoFreteValorPadrao, menuOpcoes, configuracoesExportacao);
    _gridContratoFreteValorPadrao.CarregarGrid();
}

function LoadContratoFreteValorPadrao() {
    _contratoFreteValorPadrao = new ContratoFreteValorPadrao();
    KoBindings(_contratoFreteValorPadrao, "knockoutContratoFreteValorPadrao");

    HeaderAuditoria("ContratoFreteValorPadrao", _contratoFreteValorPadrao);

    _CRUDContratoFreteValorPadrao = new CRUDContratoFreteValorPadrao();
    KoBindings(_CRUDContratoFreteValorPadrao, "knockoutCRUDContratoFreteValorPadrao");

    _pesquisaContratoFreteValorPadrao = new PesquisaContratoFreteValorPadrao();
    KoBindings(_pesquisaContratoFreteValorPadrao, "knockoutPesquisaContratoFreteValorPadrao", false, _pesquisaContratoFreteValorPadrao.Pesquisar.id);

    new BuscarJustificativas(_contratoFreteValorPadrao.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.ContratoFrete, EnumTipoFinalidadeJustificativa.Todas]);
    new BuscarClientes(_contratoFreteValorPadrao.TransportadorTerceiro, null, false, [EnumModalidadePessoa.TransportadorTerceiro]);

    LoadGridContratoFreteValorPadrao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AdicionarClick(e, sender) {
    Salvar(_contratoFreteValorPadrao, "ContratoFreteValorPadrao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridContratoFreteValorPadrao();
                LimparCamposContratoFreteValorPadrao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_contratoFreteValorPadrao, "ContratoFreteValorPadrao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridContratoFreteValorPadrao();
                LimparCamposContratoFreteValorPadrao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposContratoFreteValorPadrao();
}

function EditarClick(registroSelecionado) {
    LimparCamposContratoFreteValorPadrao();

    _contratoFreteValorPadrao.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_contratoFreteValorPadrao, "ContratoFreteValorPadrao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaContratoFreteValorPadrao.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_contratoFreteValorPadrao, "ContratoFreteValorPadrao/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    RecarregarGridContratoFreteValorPadrao();
                    LimparCamposContratoFreteValorPadrao();
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
    _CRUDContratoFreteValorPadrao.Atualizar.visible(isEdicao);
    _CRUDContratoFreteValorPadrao.Excluir.visible(isEdicao);
    _CRUDContratoFreteValorPadrao.Cancelar.visible(isEdicao);
    _CRUDContratoFreteValorPadrao.Adicionar.visible(!isEdicao);
}

function LimparCamposContratoFreteValorPadrao() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_contratoFreteValorPadrao);
}

function RecarregarGridContratoFreteValorPadrao() {
    _gridContratoFreteValorPadrao.CarregarGrid();
}