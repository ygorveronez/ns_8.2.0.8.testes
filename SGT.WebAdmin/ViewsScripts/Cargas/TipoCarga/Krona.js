/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/IdentificacaoMercadoriaKrona.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _krona;

/*
 * Declaração das Classes
 */

var Krona = function () {
    this.IdentificacaoMercadoriaKrona = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Identificação de Mercadoria: ", idBtnSearch: guid() });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadKrona() {
    _krona = new Krona();
    KoBindings(_krona, "knockoutKrona");

    new BuscarIdentificacaoMercadoriaKrona(_krona.IdentificacaoMercadoriaKrona);

    _tipoCarga.IdentificacaoMercadoriaKrona = _krona.IdentificacaoMercadoriaKrona;
}
