/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="../../../js/Importacao/componente.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cargaDataCarregamentoImportacao;

/*
 * Declaração das Classes
 */

var CargaDataCarregamentoImportacao = function () {
    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "CargaDataCarregamentoImportacao/Importar",
        UrlConfiguracao: "CargaDataCarregamentoImportacao/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O020_CargaDataCarregamentoImportacao,
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadCargaDataCarregamentoImportacao() {
    _cargaDataCarregamentoImportacao = new CargaDataCarregamentoImportacao();
    KoBindings(_cargaDataCarregamentoImportacao, "knockoutCargaDataCarregamentoImportacao");
}