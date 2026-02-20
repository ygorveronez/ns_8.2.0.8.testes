/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoEscalaVeiculo.js" />
/// <reference path="Adicionar.js" />
/// <reference path="Detalhes.js" />
/// <reference path="Suspensao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridEscalaVeiculo;
var _pesquisaEscalaVeiculo;

/*
 * Declaração das Classes
 */

var PesquisaEscalaVeiculo = function () {
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoEscalaVeiculo.EmEscala), options: EnumSituacaoEscalaVeiculo.obterOpcoesPesquisa(), def: EnumSituacaoEscalaVeiculo.EmEscala, text: "Situação: " });
    this.SomenteVeiculosDataPrevisaoRetornoExcedida = PropertyEntity({ text: "Somente Veículos com Previsão de Retorno Excedida", getType: typesKnockout.bool, val: ko.observable(false) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: adicionarEscalaVeiculo, text: "Adicionar" });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.Pesquisar = PropertyEntity({
        type: types.event, eventClick: function () {
            _pesquisaEscalaVeiculo.ExibirFiltros.visibleFade(false);
            recarregarGridEscalaVeiculo();
        }, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadEscalaVeiculo() {
    _pesquisaEscalaVeiculo = new PesquisaEscalaVeiculo();
    KoBindings(_pesquisaEscalaVeiculo, "knockoutPesquisaEscalaVeiculo");

    new BuscarModelosVeicularesCarga(_pesquisaEscalaVeiculo.ModeloVeicularCarga);
    new BuscarVeiculos(_pesquisaEscalaVeiculo.Veiculo, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, true);

    loadGridEscalaVeiculo();
    loadEscalaVeiculoAdicionar();
    loadEscalaVeiculoDetalhe();
    loadEscalaVeiculoSuspensao();
}

function loadGridEscalaVeiculo() {
    var opcaoDetalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: exibirEscalaVeiculoDetalhe, tamanho: "10", icone: "" };
    var opcaoRemoverSuspensao = { descricao: "Remover Suspensão", id: guid(), evento: "onclick", metodo: removerSuspensaoEscalaVeiculoClick, tamanho: "10", icone: "", visibilidade: isExibirOpcaoRemoverSuspensaoEscalaVeiculo };
    var opcaoSuspender = { descricao: "Suspender", id: guid(), evento: "onclick", metodo: suspenderEscalaVeiculo, tamanho: "10", icone: "", visibilidade: isExibirOpcaoSuspenderEscalaVeiculo };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [opcaoDetalhes, opcaoRemoverSuspensao, opcaoSuspender], tamanho: 10, };
    var configuracaoExportacao = { url: "EscalaVeiculo/ExportarPesquisa", titulo: "Veículos em Escala" };

    _gridEscalaVeiculo = new GridView(_pesquisaEscalaVeiculo.Pesquisar.idGrid, "EscalaVeiculo/Pesquisa", _pesquisaEscalaVeiculo, menuOpcoes, null, 25, null, null, null, null, null, null, configuracaoExportacao);
    _gridEscalaVeiculo.CarregarGrid();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function removerSuspensaoEscalaVeiculoClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Deseja realmente remover a suspensão do veículo?", function () {
        executarReST("EscalaVeiculo/RemoverSuspensao", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Suspensão do veículo removida com sucesso");
                    recarregarGridEscalaVeiculo();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

/*
 * Declaração das Funções Públicas
 */

function recarregarGridEscalaVeiculo() {
    _gridEscalaVeiculo.CarregarGrid();
}

/*
 * Declaração das Funções Privadas
 */

function isExibirOpcaoRemoverSuspensaoEscalaVeiculo(registroSelecionado) {
    return registroSelecionado.Situacao == EnumSituacaoEscalaVeiculo.Suspenso;
}

function isExibirOpcaoSuspenderEscalaVeiculo(registroSelecionado) {
    return registroSelecionado.Situacao == EnumSituacaoEscalaVeiculo.EmEscala;
}
