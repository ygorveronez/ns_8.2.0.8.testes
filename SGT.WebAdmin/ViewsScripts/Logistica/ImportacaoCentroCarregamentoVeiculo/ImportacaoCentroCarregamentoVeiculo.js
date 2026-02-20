/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="../../../js/Importacao/componente.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _importacaoCentroCarregamentoVeiculo;

/*
 * Declaração das Classes
 */

var ImportacaoCentroCarregamentoVeiculo = function () {
    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "ImportacaoCentroCarregamentoVeiculo/Importar",
        UrlConfiguracao: "ImportacaoCentroCarregamentoVeiculo/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O017_CentroCarregamentoVeiculo,
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadImportacaoCentroCarregamentoVeiculo() {
    _importacaoCentroCarregamentoVeiculo = new ImportacaoCentroCarregamentoVeiculo();
    KoBindings(_importacaoCentroCarregamentoVeiculo, "knockoutImportacaoCentroCarregamentoVeiculo");
}