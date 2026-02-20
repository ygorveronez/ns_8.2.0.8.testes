/// <reference path="../../Consultas/Almoxarifado.js" />


/*
 * Declaração de Objetos Globais do Arquivo
 */

var _pneuImportar;
var param = false;
/*
 * Declaração das Classes
 */

var PneuImportar = function () {
    this.AtualizarRegistros = PropertyEntity({ getType: typesKnockout.bool, text: "Substituição e atualização de dados", val: ko.observable(false), def: false, visible: ko.observable(true), enable: true });
    this.AtualizarRegistros.val.subscribe((novoValor) => {
        param = novoValor; 
    });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "Pneu/Importar",
        UrlConfiguracao: "Pneu/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O079_Pneu,
        ParametrosRequisicao: function () {
            var parametros = { Atualizar: param };
           return parametros;
        },
        CallbackImportacao: function () {
            recarregarGridPneu();
        }
    });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadPneuImportar() {
    _pneuImportar = new PneuImportar();
    KoBindings(_pneuImportar, "knockoutPneuImportar");
    
}
