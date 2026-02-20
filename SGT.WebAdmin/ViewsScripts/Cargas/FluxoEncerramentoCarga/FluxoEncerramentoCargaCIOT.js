//*******MAPEAMENTO KNOUCKOUT*******

var _CIOT, _gridCIOT;

var CIOT = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.EncerramentoCarga = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.BuscarCIOT = PropertyEntity({ eventClick: ConsultarCIOT, type: types.event, text: "Buscar Atualizar CIOTs", visible: ko.observable(true), idGrid: guid() });
};

//*******EVENTOS*******

function LoadCIOT() {
    _CIOT = new CIOT();
    KoBindings(_CIOT, "tabCIOT");
}

function ConsultarCIOT() {
    _CIOT.Carga.val(_fluxoEncerramentoCarga.Carga.codEntity());
    _CIOT.EncerramentoCarga.val(_fluxoEncerramentoCarga.Codigo.val());

    var baixarContrato = { descricao: "Baixar Contrato", id: guid(), metodo: BaixarContratoFrete, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [baixarContrato], tamanho: 7 };

    _gridCIOT = new GridView(_CIOT.BuscarCIOT.idGrid, "FluxoEncerramentoCargaCIOT/Pesquisa", _CIOT, menuOpcoes, { column: 3, dir: orderDir.desc }, null);
    _gridCIOT.CarregarGrid();
}

function BaixarContratoFrete(e) {
    var data = { MDFe: e.Codigo };
    executarDownload("FluxoEncerramentoCargaCIOT/DownloadContrato", data);
}


//*******MÉTODOS*******

function LimparCamposCIOT() {
    LimparCampos(_CIOT);
}