/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/MotivoChamado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaControleChamadoTMS;
var _gridControleChamadoTMS;

var PesquisaControleChamadoTMS = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.PrimeiraDataDoMesAtual()) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Carga = PropertyEntity({ text: "Número Carga:" });
    this.NumeroCTe = PropertyEntity({ text: "Número CT-e:", getType: typesKnockout.int, maxlength: 10 });
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 10 });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 10 });
    this.MotivoChamado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo do Chamado:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            buscarControleChamadoTMS();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

function loadControleChamadoTMS() {
    _pesquisaControleChamadoTMS = new PesquisaControleChamadoTMS();
    KoBindings(_pesquisaControleChamadoTMS, "knockoutPesquisaControleChamadoTMS", false, _pesquisaControleChamadoTMS.Pesquisar.idBtnSearch);

    new BuscarMotivoChamado(_pesquisaControleChamadoTMS.MotivoChamado);
    new BuscarMotorista(_pesquisaControleChamadoTMS.Motorista);
    new BuscarVeiculos(_pesquisaControleChamadoTMS.Veiculo);

    loadDocumentoAnalise();

    buscarControleChamadoTMS();
}

function informarDocumentoClick(e) {
    _documentoAnalise.Codigo.val(e.Codigo);

    Global.abrirModal("divModalDocumentoAnalise");
}

function buscarControleChamadoTMS() {
    if (_gridControleChamadoTMS != null) {
        _gridControleChamadoTMS.Destroy();
        _gridControleChamadoTMS = null;
    }

    var informarDocumento = { descricao: "Informar Documento", id: guid(), tamanho: 9, metodo: informarDocumentoClick };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [informarDocumento] };

    _gridControleChamadoTMS = new GridView(_pesquisaControleChamadoTMS.Pesquisar.idGrid, "ControleChamadoTMS/Consultar", _pesquisaControleChamadoTMS, menuOpcoes, null, 10);
    _gridControleChamadoTMS.CarregarGrid();
}