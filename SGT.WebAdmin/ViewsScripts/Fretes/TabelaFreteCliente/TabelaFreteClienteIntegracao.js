/// <reference path="TabelaFreteCliente.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

//#region Variaveis Globais
var _pesquisaTabelaFreteClienteIntegracoes;
var _pesquisaHistoricoIntegracaoTabelaCliente;
//#endregion

//#region Constructores
var PesquisaHistoricoIntegracaoTabelaCliente = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};
//#endregion

//#region Funções Asociadas a eventos
function exibirHistoricoIntegracoesTabelaFreteClienteClick(integracao) {
    BuscarHistoricoIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoTabelaFreteCliente");
}

function DownloadArquivosHistoricoIntegracaoTabelaFreteClienteClick(historicoConsulta) {
    executarDownload("TabelaFreteClienteIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}
//#endregion


/*
 * Declaração das Funções
 */

function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracaoTabelaCliente = new PesquisaHistoricoIntegracaoTabelaCliente();
    _pesquisaHistoricoIntegracaoTabelaCliente.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoTabelaFreteClienteClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoTabelaClienteIntegracao = new GridView("tblHistoricoIntegracaoTabelaFreteCliente", "TabelaFreteClienteIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoTabelaCliente, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoTabelaClienteIntegracao.CarregarGrid();
}
