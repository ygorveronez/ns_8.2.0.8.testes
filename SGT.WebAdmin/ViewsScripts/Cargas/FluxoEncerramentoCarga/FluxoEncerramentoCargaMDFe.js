//*******MAPEAMENTO KNOUCKOUT*******

var _mdfe, _gridMDFe;

var MDFe = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.EncerramentoCarga = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.BuscarMDFes = PropertyEntity({ eventClick: ConsultarMDFes, type: types.event, text: "Buscar / Atualizar MDF-es", visible: ko.observable(true), idGrid: guid() });
};

//*******EVENTOS*******

function LoadMDFe() {
    _mdfe = new MDFe();
    KoBindings(_mdfe, "tabMDFe");
}

function ConsultarMDFes() {
    _mdfe.Carga.val(_fluxoEncerramentoCarga.Carga.codEntity());
    _mdfe.EncerramentoCarga.val(_fluxoEncerramentoCarga.Codigo.val());

    var baixarDAMDFE = { descricao: "Baixar DAMFE", id: guid(), metodo: BaixarDAMDFEClick, icone: "" };
    var baixarXML = { descricao: "Baixar XML", id: guid(), metodo: BaixarXMLMDFeClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [baixarDAMDFE, baixarXML], tamanho: 7 };

    _gridMDFe = new GridView(_mdfe.BuscarMDFes.idGrid, "FluxoEncerramentoCargaMDFe/Pesquisa", _mdfe, menuOpcoes, { column: 2, dir: orderDir.desc });

    _gridMDFe.CarregarGrid();
}

function BaixarXMLMDFeClick(e) {
    var data = { MDFe: e.Codigo };
    executarDownload("FluxoEncerramentoCargaMDFe/DownloadXML", data);
}

function BaixarDAMDFEClick(e) {
    var data = { MDFe: e.Codigo };
    executarDownload("FluxoEncerramentoCargaMDFe/DownloadDAMDFE", data);
}

//*******MÉTODOS*******

function LimparCamposMDFe() {
    LimparCampos(_mdfe);
}