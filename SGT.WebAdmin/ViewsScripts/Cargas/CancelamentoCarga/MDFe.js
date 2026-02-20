/// <reference path="../../Enumeradores/EnumSituacaoMDFe.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
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
/// <reference path="MDFeAverbacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _mdfe, _gridMDFe, _gridCargaMDFeAverbacao;

var MDFe = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CancelamentoCarga = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.BuscarMDFes = PropertyEntity({ eventClick: ConsultarMDFesCarga, type: types.event, text: Localization.Resources.Cargas.CancelamentoCarga.BuscarAtualizarMDFe, visible: ko.observable(true), idGrid: guid() });
    this.BuscarAverbacoes = PropertyEntity({ eventClick: BuscarAverbacoesMDFesCargaClick, type: types.event, text: Localization.Resources.Cargas.CancelamentoCarga.BuscarAtualizarMDFe, visible: ko.observable(true), idGrid: guid() });
};

//*******EVENTOS*******

function LoadMDFe() {
    _mdfe = new MDFe();
    KoBindings(_mdfe, "knockoutMDFe");
}

function ConsultarMDFesCarga(e) {
    _mdfe.Carga.val(_cancelamento.Carga.codEntity());
    _mdfe.CancelamentoCarga.val(_cancelamento.Codigo.val());

    var baixarDAMDFE = { descricao: Localization.Resources.Cargas.CancelamentoCarga.BaixarDAMDFE, id: guid(), metodo: BaixarDAMDFEClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadMDFe };
    var baixarXML = { descricao: Localization.Resources.Cargas.CancelamentoCarga.BaixarXML, id: guid(), metodo: BaixarXMLMDFeClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadMDFe };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [baixarDAMDFE, baixarXML], tamanho: 7 };

    _gridMDFe = new GridView(_mdfe.BuscarMDFes.idGrid, "CancelamentoCargaMDFe/Pesquisa", _mdfe, menuOpcoes, { column: 2, dir: orderDir.desc }, null);

    _gridMDFe.CarregarGrid();

    BuscarCargasMDFeAverbacao();
}

function BaixarXMLMDFeClick(e) {
    var data = { MDFe: e.Codigo };
    executarDownload("CancelamentoCargaMDFe/DownloadXML", data);
}

function BaixarDAMDFEClick(e) {
    var data = { MDFe: e.Codigo };
    executarDownload("CancelamentoCargaMDFe/DownloadDAMDFE", data);
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