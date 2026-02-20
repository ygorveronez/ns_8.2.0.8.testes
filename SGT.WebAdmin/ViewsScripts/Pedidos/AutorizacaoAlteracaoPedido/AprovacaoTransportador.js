/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridAprovacaoTransportador;

/*
 * Declaração das Funções de Inicialização
 */

function loadAprovacaoTransportador() {
    loadGridAprovacaoTransportador();
}

function loadGridAprovacaoTransportador() {
    var knoutAlteracaoPedido = {
        Codigo: _alteracaoPedido.Codigo,
    };

    _gridAprovacaoTransportador = new GridView("grid-aprovacao-transportador", "AutorizacaoAlteracaoPedido/RegrasAprovacaoTransportador", knoutAlteracaoPedido);
}

/*
 * Declaração das Funções
 */

function atualizarGridAprovacaoTransportador() {
    _gridAprovacaoTransportador.CarregarGrid(function (retorno) {
        controlarExibicaoAbaAprovacaoTransportador(retorno.data.length);
    });
}
