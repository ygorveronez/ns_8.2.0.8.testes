/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridManobraDetalhes;

/*
 * Declaração das Funções de Inicialização
 */

function loadManobraDetalhes() {
    loadGridManobraDetalhes();
}

function loadGridManobraDetalhes() {
    var header = [
        { data: "DataOrdenar", visible: false },
        { data: "Data", title: "Data", width: "20%", className: 'text-align-center', orderable: false },
        { data: "Descricao", title: "Descrição", width: "60%", orderable: false },
        { data: "Usuario", title: "Usuário", width: "20%", orderable: false }
    ];
    var menuOpcoes = null;
    var ordenacao = { column: 0, dir: orderDir.asc };

    _gridManobraDetalhes = new BasicDataTable("grid-historico-manobra", header, menuOpcoes, ordenacao);
    _gridManobraDetalhes.CarregarGrid([]);
}

/*
 * Declaração das Funções Públicas
 */

function exibirManobraDetalhes(registroSelecionado) {
    executarReST("Manobra/ObterHistorico", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _gridManobraDetalhes.CarregarGrid(retorno.Data);

                exibirModalManobraDetalhes();
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

function exibirModalManobraDetalhes() {
    Global.abrirModal('divModalDetalhesManobra');
    $("#divModalDetalhesManobra").one('hidden.bs.modal', function () {
        _gridManobraDetalhes.CarregarGrid([]);
    });
}