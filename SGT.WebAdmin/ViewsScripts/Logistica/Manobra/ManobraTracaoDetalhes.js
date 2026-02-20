/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridManobraTracaoDetalhes;
var _manobraTracaoDetalhes;
var _pesquisaHistoricoManobraTracao;

/*
 * Declaração das Classes
 */

var PesquisaHistoricoManobraTracao = function () {
    this.CentroCarregamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var ManobraTracaoDetalhes = function () {
    this.AcoesRealizadas = PropertyEntity({ text: "Ações Realizadas:" });
    this.Motorista = PropertyEntity({ text: "Motorista Atual:" });
    this.Placa = PropertyEntity({});
    this.TempoMedioAcao = PropertyEntity({ text: "Tempo Médio Ação:" });
    this.TempoTotalOcioso = PropertyEntity({ text: "Tempo Total Ocioso:" });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadManobraTracaoDetalhes() {
    _manobraTracaoDetalhes = new ManobraTracaoDetalhes();
    KoBindings(_manobraTracaoDetalhes, "knockoutManobraTracaoDetalhes");

    _pesquisaHistoricoManobraTracao = new PesquisaHistoricoManobraTracao();
    
    loadGridManobraTracaoDetalhes();
}

function loadGridManobraTracaoDetalhes() {
    var limiteRegistros = 25;
    var menuOpcoes = null;
    var totalRegistrosPorPagina = 5;

    _gridManobraTracaoDetalhes = new GridView("grid-historico-manobra-tracao", "ManobraTracao/PesquisaHistorico", _pesquisaHistoricoManobraTracao, menuOpcoes, null, totalRegistrosPorPagina, null, false, false, undefined, limiteRegistros);
    _gridManobraTracaoDetalhes.CarregarGrid();
}

/*
 * Declaração das Funções Públicas
 */

function exibirManobraTracaoDetalhes(registroSelecionado) {
    executarReST("ManobraTracao/ObterDetalhes", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_manobraTracaoDetalhes, retorno);

                _pesquisaHistoricoManobraTracao.CentroCarregamento.val(_pesquisaManobraAuxiliar.CentroCarregamento.codEntity());
                _pesquisaHistoricoManobraTracao.Tracao.val(registroSelecionado.Tracao);

                _gridManobraTracaoDetalhes.CarregarGrid();

                exibirModalManobraTracaoDetalhes();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções
 */

function exibirModalManobraTracaoDetalhes() {
    Global.abrirModal('divModalDetalhesManobraTracao');
    $("#divModalDetalhesManobraTracao").one('hidden.bs.modal', function () {
        limparManobraTracaoDetalhes();
    });
}

function limparManobraTracaoDetalhes() {
    LimparCampos(_pesquisaHistoricoManobraTracao);

    _gridManobraTracaoDetalhes.CarregarGrid();
}