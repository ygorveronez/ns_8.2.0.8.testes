/// <reference path="../../Enumeradores/EnumStatusCTe.js" />
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
/// <reference path="CIOT.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _cte, _carga, _acrescimoDesconto;
var _gridCTe, _gridCargas, _gridAcrescimoDesconto;

var CTe = function () {
    this.CIOT = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.BuscarCTes = PropertyEntity({ eventClick: ConsultarCTesCIOT, type: types.event, text: "Buscar / Atualizar CT-e(s)", visible: ko.observable(true), idGrid: guid() });
};

var Carga = function () {
    this.GridCargas = PropertyEntity({ visible: ko.observable(false), idGrid: guid() });
};

var AcrescimoDesconto = function () {
    this.GridAcrescimoDesconto = PropertyEntity({ visible: ko.observable(false), idGrid: guid() });
};

//*******EVENTOS*******

function LoadCTe() {
    _cte = new CTe();
    KoBindings(_cte, "tabCTes");

    _carga = new Carga();
    KoBindings(_carga, "tabCargas");

    _acrescimoDesconto = new AcrescimoDesconto();
    KoBindings(_acrescimoDesconto, "tabAcrescimoDesconto");
}

function ConsultarCTesCIOT(e) {
    _cte.CIOT.val(_CIOT.Codigo.val());

    var baixarDACTE = { descricao: "Baixar DACTE", id: guid(), metodo: BaixarDACTEClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadCTe };
    var baixarXML = { descricao: "Baixar XML", id: guid(), metodo: BaixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadCTe };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [baixarDACTE, baixarXML], tamanho: 7 };

    _gridCTe = new GridView(_cte.BuscarCTes.idGrid, "CIOTCTe/Pesquisa", _cte, menuOpcoes, { column: 3, dir: orderDir.desc }, null);
    _gridCTe.CarregarGrid();

    _gridCargas = new GridView(_carga.GridCargas.idGrid, "CIOTCTe/PesquisaCargas", _cte);
    _gridCargas.onBeforeGridLoad(VisibilidadeAbaCargas);
    _gridCargas.CarregarGrid();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _gridAcrescimoDesconto = new GridView(_acrescimoDesconto.GridAcrescimoDesconto.idGrid, "CIOTCTe/PesquisaAcrescimoDescontoContratoFrete", _cte);
        _gridAcrescimoDesconto.onBeforeGridLoad(VisibilidadeAbaAcrescimoDesconto);
        _gridAcrescimoDesconto.CarregarGrid();
    }
}

function BaixarXMLCTeClick(e) {
    var data = { CTe: e.Codigo };
    executarDownload("CIOTCTe/DownloadXML", data);
}

function BaixarDACTEClick(e) {
    var data = { CTe: e.Codigo };
    executarDownload("CIOTCTe/DownloadDACTE", data);
}

//*******MÉTODOS*******

function VisibilidadeAbaCargas(data) {
    $("#liTabCTes").hide();
    $("#liTabCargas").hide();

    if (data.recordsTotal > 0) {
        $("#liTabCargas").show();
        $("#liTabCargas a").click();
    } else {
        $("#liTabCTes").show();
        $("#liTabCTes a").click();
    }
}

function VisibilidadeAbaAcrescimoDesconto(data) {
    $("#liTabAcrescimoDesconto").hide();
    if (data.recordsTotal > 0)
        $("#liTabAcrescimoDesconto").show();
}

function LimparCamposCTe() {
    LimparCampos(_cte);
}

function VisibilidadeOpcaoDownloadCTe(data) {
    if (data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO || data.SituacaoCTe == EnumStatusCTe.ANULADO)
        return true;
    else
        return false;
}