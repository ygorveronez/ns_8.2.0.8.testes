/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDTipoOperacaoCampoValorPadrao;
var _tipoOperacaoCampoValorPadrao;
var _pesquisaTipoOperacaoCampoValorPadrao;
var _gridTipoOperacaoValorPadrao;

/*
 * Declaração das Classes
 */

var CRUDTipoOperacaoValorPadrao = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var TipoOperacaoValorPadrao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Campo:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    
    this.Habilitar = PropertyEntity({ text: "Habilitar", getType: typesKnockout.bool, val: ko.observable(false) });

    this.Campo = PropertyEntity({ type: types.event, text: "Adicionar Campo", idBtnSearch: guid(), enable: ko.observable(true) });
    this.GridCampo = PropertyEntity({ type: types.local });
   // this.Campos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

var PesquisaTipoOperacaoValorObrigatorio = function () {
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Descrição:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridTipoOperacaoValorPadrao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridTipoOperacaoValorObrigatorio() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (registroSelecionado) { EditarClick(registroSelecionado, false); }, tamanho: "10", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "TipoOperacaoValorPadrao/ExportarPesquisa", titulo: "Campos Tipo Operação Valor Padrão" };

    _gridTipoOperacaoValorPadrao = new GridViewExportacao(_pesquisaTipoOperacaoCampoValorPadrao.Pesquisar.idGrid, "TipoOperacaoValorPadrao/Pesquisa", _pesquisaTipoOperacaoCampoValorPadrao, menuOpcoes, configuracoesExportacao);
    _gridTipoOperacaoValorPadrao.CarregarGrid();
}

function LoadTipoOperacaoValorPadrao() {
    _tipoOperacaoCampoValorPadrao = new TipoOperacaoValorPadrao();
    KoBindings(_tipoOperacaoCampoValorPadrao, "knockoutTipoOperacaoValorPadrao");

    HeaderAuditoria("TipoOperacaoValorPadrao", _tipoOperacaoCampoValorPadrao);

    _CRUDTipoOperacaoCampoValorPadrao = new CRUDTipoOperacaoValorPadrao();
    KoBindings(_CRUDTipoOperacaoCampoValorPadrao, "knockoutCRUDTipoOperacaoValorPadrao");

    _pesquisaTipoOperacaoCampoValorPadrao = new PesquisaTipoOperacaoValorObrigatorio();
    KoBindings(_pesquisaTipoOperacaoCampoValorPadrao, "knockoutPesquisaTipoOperacaoValorPadrao", false, _pesquisaTipoOperacaoCampoValorPadrao.Pesquisar.id);

    new BuscarCampoTipoOperacao(_tipoOperacaoCampoValorPadrao.TipoOperacao);


    new BuscarTiposOperacao(_pesquisaTipoOperacaoCampoValorPadrao.TipoOperacao);

    LoadCampoTipoOperacao();
    LoadGridTipoOperacaoValorObrigatorio();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AdicionarClick(e, sender) {

    Salvar(_tipoOperacaoCampoValorPadrao, "TipoOperacaoValorPadrao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridTipoOperacaoValorPadrao();
                LimparCamposTipoOperacaoValorPadrao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {

    Salvar(_tipoOperacaoCampoValorPadrao, "TipoOperacaoValorPadrao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");

                RecarregarGridTipoOperacaoValorPadrao();
                LimparCamposTipoOperacaoValorPadrao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposTipoOperacaoValorPadrao();
}

function EditarClick(registroSelecionado, duplicar) {
    LimparCamposTipoOperacaoValorPadrao();

    executarReST("TipoOperacaoValorPadrao/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo, Duplicar: duplicar }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_tipoOperacaoCampoValorPadrao, retorno);
              //  _pesquisaTipoOperacaoCampoValorPadrao.ExibirFiltros.visibleFade(false);

                ControlarBotoesHabilitados(!duplicar);

             //   _tipoOperacaoCampoValorPadrao.Campo.basicTable.CarregarGrid(_tipoOperacaoCampoValorPadrao.Campos.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function ExcluirClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir este registro?", function () {
        ExcluirPorCodigo(_tipoOperacaoCampoValorPadrao, "TipoOperacaoValorPadrao/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");

                    RecarregarGridTipoOperacaoValorPadrao();
                    LimparCamposTipoOperacaoValorPadrao();
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
    _CRUDTipoOperacaoCampoValorPadrao.Atualizar.visible(isEdicao);
    _CRUDTipoOperacaoCampoValorPadrao.Excluir.visible(isEdicao);
    _CRUDTipoOperacaoCampoValorPadrao.Cancelar.visible(isEdicao);
    _CRUDTipoOperacaoCampoValorPadrao.Adicionar.visible(!isEdicao);
}

function LimparCamposTipoOperacaoValorPadrao() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_tipoOperacaoCampoValorPadrao);

    _tipoOperacaoCampoValorPadrao.Campo.basicTable.CarregarGrid([]);
}

function RecarregarGridTipoOperacaoValorPadrao() {
    _gridTipoOperacaoValorPadrao.CarregarGrid();
}