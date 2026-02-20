/// <reference path="../../../js/Global/Globais.js" />

//#region Variaveis Globais
var _gridHistoricoIrregularidades;
var _ModalHistoricoIrregularidades;
//#endregion

//#region Constructores
function ModalHistoricoIrregularidades() {
    this.Grid = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid(), idTab: guid() });
    this.Codigo = PropertyEntity({ val: ko.observable(0) });

    this.Cancelar = PropertyEntity({ eventClick: CancelarHistoricoIrregularidade, type: types.event, text: "Cancelar", idGrid: guid(), visible: ko.observable(true) });
}
//#endregion

//#region Carregamento
function loadHistoricoIrregularidades() {
    _ModalHistoricoIrregularidades = new ModalHistoricoIrregularidades();
    KoBindings(_ModalHistoricoIrregularidades, "knoutHistoricoIrregularidade");
    loadGridHistoricoIrregularidade();
}

function loadGridHistoricoIrregularidade() {
    var opcaoDetalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: abrirDetalhesIrregularidade, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDetalhes] };
    _gridHistoricoIrregularidades = new GridView(_ModalHistoricoIrregularidades.Grid.idGrid, "DocumentoDestinadoEmpresa/BuscarHistoricoIrregularidade", _ModalHistoricoIrregularidades, menuOpcoes);

}
//#endregion

//#region Funções Auxiliares
function CarregarRegistroHistoricoIrregularidade(codigo) {
    _ModalHistoricoIrregularidades.Codigo.val(codigo);
    _gridHistoricoIrregularidades.CarregarGrid();
}

function CancelarHistoricoIrregularidade() {
    Global.fecharModal("divModalVisualizarHistoricoIrregularidade");

}

function abrirDetalhesIrregularidade(e) {
    Global.abrirModal("divModalObservacaoIrregularidade");
    $("#observacaoIrregularidade").html(`<p>${e.Observacao}</p>`);
}
function fecharModalObservacao() {
    $("#observacaoIrregularidade").html("");
    Global.fecharModal("divModalObservacaoIrregularidade");
}
//#endregion