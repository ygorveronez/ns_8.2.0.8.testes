/// <reference path="../../Consultas/ContratoFrete.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../Enumeradores/EnumSituacaoContratoFreteAcrescimoDesconto.js" />
/// <reference path="ContratoFreteAcrescimoDesconto.js"/>

//*******MAPEAMENTO KNOUCKOUT*******

var _gridContratoFreteAcrescimoDesconto;
var _pesquisaContratoFreteAcrescimoDesconto;

var PesquisaContratoFreteAcrescimoDesconto = function () {
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable(EnumSituacaoContratoFreteAcrescimoDesconto.Todos), options: EnumSituacaoContratoFreteAcrescimoDesconto.obterOpcoesPesquisa(), def: EnumSituacaoContratoFreteAcrescimoDesconto.Todos });

    this.ContratoFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Contrato de Frete:", idBtnSearch: guid() });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Justificativa:", idBtnSearch: guid() });
    this.NumeroCarga = PropertyEntity({ required: true, codEntity: ko.observable(0), type: types.multiplesEntities, text: "Número Carga:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.NumeroCIOT = PropertyEntity({ required: true, codEntity: ko.observable(0), type: types.multiplesEntities, text: "Número CIOT:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.NomeSubcontratado = PropertyEntity({ required: true, codEntity: ko.observable(0), type: types.multiplesEntities, text: "Subcontratado:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.NomeMotorista = PropertyEntity({ required: true, codEntity: ko.observable(0), type: types.multiplesEntities, text: "Motorista:", idBtnSearch: guid(), enable: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridContratoFreteAcrescimoDesconto.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function LoadPesquisaContratoFreteAcrescimoDesconto() {
    _pesquisaContratoFreteAcrescimoDesconto = new PesquisaContratoFreteAcrescimoDesconto();
    KoBindings(_pesquisaContratoFreteAcrescimoDesconto, "knockoutPesquisaContratoFreteAcrescimoDesconto", false, _pesquisaContratoFreteAcrescimoDesconto.Pesquisar.id);

    new BuscarJustificativas(_pesquisaContratoFreteAcrescimoDesconto.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.ContratoFrete]);
    new BuscarContratoFrete(_pesquisaContratoFreteAcrescimoDesconto.ContratoFrete);
    new BuscarCargas(_pesquisaContratoFreteAcrescimoDesconto.NumeroCarga);
    new BuscarCIOT(_pesquisaContratoFreteAcrescimoDesconto.NumeroCIOT);
    new BuscarMotoristas(_pesquisaContratoFreteAcrescimoDesconto.NomeMotorista);
    new BuscarClientes(_pesquisaContratoFreteAcrescimoDesconto.NomeSubcontratado);

    BuscarContratosFreteAcrescimoDesconto();
}

//*******MÉTODOS*******

function BuscarContratosFreteAcrescimoDesconto() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarContratoFreteAcrescimoDesconto, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridContratoFreteAcrescimoDesconto = new GridView(_pesquisaContratoFreteAcrescimoDesconto.Pesquisar.idGrid, "ContratoFreteAcrescimoDesconto/Pesquisa", _pesquisaContratoFreteAcrescimoDesconto, menuOpcoes);

    _gridContratoFreteAcrescimoDesconto.CarregarGrid();
}