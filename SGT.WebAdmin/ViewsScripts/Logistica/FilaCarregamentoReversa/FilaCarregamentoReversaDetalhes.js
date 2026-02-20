/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridFilaCarregamentoReversaDetalhes;

/*
 * Declaração das Funções de Inicialização
 */

function loadFilaCarregamentoReversaDetalhes() {
    loadGridFilaCarregamentoReversaDetalhes();
}

function loadGridFilaCarregamentoReversaDetalhes() {
    var header = [
        { data: "DataOrdenar", visible: false },
        { data: "Data", title: "Data", width: "20%", className: 'text-align-center', orderable: false },
        { data: "Descricao", title: "Descrição", width: "60%", orderable: false },
        { data: "Usuario", title: "Usuário", width: "20%", orderable: false }
    ];
    var menuOpcoes = null;
    var ordenacao = { column: 0, dir: orderDir.asc };

    _gridFilaCarregamentoReversaDetalhes = new BasicDataTable("grid-historico-fila-carregamento-reversa", header, menuOpcoes, ordenacao);
    _gridFilaCarregamentoReversaDetalhes.CarregarGrid([]);
}

/*
 * Declaração das Funções Públicas
 */

function exibirFilaCarregamentoReversaDetalhes(registroSelecionado) {
    executarReST("FilaCarregamentoReversa/ObterHistorico", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _gridFilaCarregamentoReversaDetalhes.CarregarGrid(retorno.Data);

                exibirModalFilaCarregamentoReversaDetalhes();
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

function exibirModalFilaCarregamentoReversaDetalhes() {
    Global.abrirModal('divModalDetalhesFilaCarregamentoReversa');
    $("#divModalDetalhesFilaCarregamentoReversa").one('hidden.bs.modal', function () {
        _gridFilaCarregamentoReversaDetalhes.CarregarGrid([]);
    });
}