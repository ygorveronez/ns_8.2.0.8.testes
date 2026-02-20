/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridVigenciaAnexo;

/*
 * Declaração das Funções de Inicialização
 */

function loadGridVigenciaAnexo() {
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadVigenciaAnexoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload] };

    _gridVigenciaAnexo = new GridView("grid-vigencia-anexo", "VigenciaTabelaFreteAnexo/PesquisaAnexoReajusteTabelaFrete", _ajusteTabela, menuOpcoes);
    _gridVigenciaAnexo.CarregarGrid();
}

function loadVigenciaAnexo() {
    loadGridVigenciaAnexo();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function downloadVigenciaAnexoClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("VigenciaTabelaFreteAnexo/DownloadAnexo", dados);
}

/*
 * Declaração das Funções Públicas
 */

function recarregarGridVigenciaAnexo() {
    _gridVigenciaAnexo.CarregarGrid();
}
