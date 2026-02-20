/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumRotaFreteClasse.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRotaFreteAbastecimento;
var _gridRotaFreteAbastecimento;
var _rotaFreteAbastecimento;
var _pesquisaRotaFreteAbastecimento;

/*
 * Declaração das Classes
 */

var CRUDRotaFreteAbastecimento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: true });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var RotaFreteAbastecimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.RotaEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFreteAbastecimento.RotaEmbarcador, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), required: true });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFreteAbastecimento.ModeloVeicular, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), required: false });

    this.PreAbastecimentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
}

var PesquisaRotaFreteAbastecimento = function () {
    this.RotaEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFreteAbastecimento.RotaEmbarcador, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFreteAbastecimento.ModeloVeicular, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridRotaFreteAbastecimento, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridRotaFreteAbastecimento() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "RotaFreteAbastecimento/ExportarPesquisa", titulo: Localization.Resources.Logistica.RotaFreteAbastecimento.VinculosPreAbastecimentoComRota };

    _gridRotaFreteAbastecimento = new GridViewExportacao(_pesquisaRotaFreteAbastecimento.Pesquisar.idGrid, "RotaFreteAbastecimento/Pesquisa", _pesquisaRotaFreteAbastecimento, menuOpcoes, configuracoesExportacao);
    _gridRotaFreteAbastecimento.CarregarGrid();
}

function loadRotaFreteAbastecimento() {
    _rotaFreteAbastecimento = new RotaFreteAbastecimento();
    KoBindings(_rotaFreteAbastecimento, "knockoutRotaFreteAbastecimento");

    HeaderAuditoria("RotaFreteAbastecimento", _rotaFreteAbastecimento);

    _CRUDRotaFreteAbastecimento = new CRUDRotaFreteAbastecimento();
    KoBindings(_CRUDRotaFreteAbastecimento, "knockoutCRUDRotaFreteAbastecimento");

    _pesquisaRotaFreteAbastecimento = new PesquisaRotaFreteAbastecimento();
    KoBindings(_pesquisaRotaFreteAbastecimento, "knockoutPesquisaRotaFreteAbastecimento", false, _pesquisaRotaFreteAbastecimento.Pesquisar.id);

    loadGridRotaFreteAbastecimento();

    new BuscarRotasFrete(_rotaFreteAbastecimento.RotaEmbarcador);
    new BuscarRotasFrete(_pesquisaRotaFreteAbastecimento.RotaEmbarcador);

    new BuscarModelosVeicularesCarga(_rotaFreteAbastecimento.ModeloVeicular);
    new BuscarModelosVeicularesCarga(_pesquisaRotaFreteAbastecimento.ModeloVeicular);

    LoadPreAbastecimento();

}

function PreencherListasDeSelecaoRotaFreteAbastecimento() {

    _rotaFreteAbastecimento.PreAbastecimentos.val(JSON.stringify(_gridPreAbastecimento.BuscarRegistros()));
}
/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    PreencherListasDeSelecaoRotaFreteAbastecimento();
    Salvar(_rotaFreteAbastecimento, "RotaFreteAbastecimento/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);

                recarregarGridRotaFreteAbastecimento();
                limparCamposRotaFreteAbastecimento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    PreencherListasDeSelecaoRotaFreteAbastecimento();
    Salvar(_rotaFreteAbastecimento, "RotaFreteAbastecimento/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                recarregarGridRotaFreteAbastecimento();
                limparCamposRotaFreteAbastecimento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposRotaFreteAbastecimento();
}

function editarClick(registroSelecionado) {
    limparCamposRotaFreteAbastecimento();

    _rotaFreteAbastecimento.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_rotaFreteAbastecimento, "RotaFreteAbastecimento/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaRotaFreteAbastecimento.ExibirFiltros.visibleFade(false);
                controlarBotoesHabilitados();
                RecarregarGridPreAbastecimento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, null);
}

function excluirClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, (Localization.Resources.Logistica.RotaFreteAbastecimento.DesejaRealmenteExcluirVinculo), function () {
        ExcluirPorCodigo(_rotaFreteAbastecimento, "RotaFreteAbastecimento/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    recarregarGridRotaFreteAbastecimento();
                    limparCamposRotaFreteAbastecimento();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados() {
    var isEdicao = _rotaFreteAbastecimento.Codigo.val() > 0;

    _CRUDRotaFreteAbastecimento.Atualizar.visible(isEdicao);
    _CRUDRotaFreteAbastecimento.Excluir.visible(isEdicao);
    _CRUDRotaFreteAbastecimento.Adicionar.visible(!isEdicao);
}

function limparCamposRotaFreteAbastecimento() {
    LimparCampos(_rotaFreteAbastecimento);
    controlarBotoesHabilitados();
    LimparCamposPreAbastecimentoRotaFreteAbastecimento();
}

function recarregarGridRotaFreteAbastecimento() {
    _gridRotaFreteAbastecimento.CarregarGrid();
}