/// <reference path="../../Enumeradores/EnumSituacaoMDFe.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="Ocorrencia.js" />
/// <reference path="EtapasOcorrencia.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _mdfe;
var _gridMDFe;

var MDFe = function () {
    this.CargaMDFeManual = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.BuscarMDFes = PropertyEntity({ eventClick: ConsultarMDFesCarga, type: types.event, text: "Buscar / Atualizar MDF-e(s)", visible: ko.observable(true), idGrid: guid() });
}

//*******EVENTOS*******

function LoadMDFe() {
    _mdfe = new MDFe();
    KoBindings(_mdfe, "knockoutMDFe");
}

function ConsultarMDFesCarga(e) {
    _mdfe.CargaMDFeManual.val(_cancelamento.CargaMDFeManual.codEntity());

    var baixarDAMDFE = { descricao: "Baixar DAMDFE", id: guid(), metodo: baixarDAMDFeClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadMDFe };
    var baixarXML = { descricao: "Baixar XML", id: guid(), metodo: baixarXMLMDFeClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadMDFe };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [baixarDAMDFE, baixarXML], tamanho: 7 };

    _gridMDFe = new GridView(_mdfe.BuscarMDFes.idGrid, "CargaMDFeManualMDFe/ConsultarCargaMDFe", _mdfe, menuOpcoes, { column: 4, dir: orderDir.desc }, null);

    _gridMDFe.CarregarGrid();
}

function baixarXMLMDFeClick(e) {
    var data = { CodigoMDFe: e.CodigoMDFE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaMDFe/DownloadXMLAutorizacao", data);
}

function baixarDAMDFeClick(e) {
    var data = { CodigoMDFe: e.CodigoMDFE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaMDFe/DownloadDAMDFE", data);
}

//*******MÉTODOS*******

function LimparCamposMDFe() {
    LimparCampos(_mdfe);
}

function VisibilidadeOpcaoDownloadMDFe(data) {
    if (data.Status == EnumSituacaoMDFe.Autorizado || data.Status == EnumSituacaoMDFe.Cancelado || data.Status == EnumSituacaoMDFe.Encerrado || data.Status == EnumSituacaoMDFe.EmEncerramento)
        return true;
    else
        return false;
}