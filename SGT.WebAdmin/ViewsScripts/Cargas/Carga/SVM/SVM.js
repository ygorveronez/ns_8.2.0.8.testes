
var _HTMLDadosSVM = "", _etapaSVM, _gridDadosSVM;

var EtapaSVM = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Download = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Download });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusCTe.TODOS), enable: ko.observable(true), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.SituacaoDoCte.getFieldDescription(), options: EnumStatusCTe.obterOpcoes(), def: EnumStatusCTe.TODOS });
    this.NumeroNF = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDaNotaFiscal.getFieldDescription(), val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.NumeroDocumento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoDocumento.getFieldDescription(), val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });

    this.DownloadLoteXMLCTe = PropertyEntity({ eventClick: DownloadLoteXMLCTeSVMClick, type: types.event, text: Localization.Resources.Cargas.Carga.XmlDosCtesNfses, idGrid: guid(), visible: ko.observable(true) });
    this.DownloadLoteDACTE = PropertyEntity({ eventClick: DownloadLoteDACTESVMClick, type: types.event, text: Localization.Resources.Cargas.Carga.PdfDosCtesNfses, idGrid: guid(), visible: ko.observable(true) });
    this.DownloadLoteDocumentos = PropertyEntity({ eventClick: DownloadLoteDocumentosCTeSVMClick, type: types.event, text: Localization.Resources.Cargas.Carga.PdfTodosDocumentos, idGrid: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDadosSVM.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });
};

function buscarDadosSVMClick(e) {
    _cargaAtual = e;

    let strknoutEtapaSVM = "knoutEtapaSVM" + e.EtapaSVM.idGrid;
    let html = _HTMLDadosSVM.replace("#knoutEtapaSVM", strknoutEtapaSVM);

    $("#" + e.EtapaSVM.idGrid).html(html);

    _etapaSVM = new EtapaSVM();
    KoBindings(_etapaSVM, strknoutEtapaSVM);

    _etapaSVM.Carga.val(_cargaAtual.Codigo.val());

    montarGridDadosSVM();
}

function montarGridDadosSVM() {
    //var cancelar = { descricao: Localization.Resources.Gerais.Geral.Cancelar, id: guid(), metodo: cancelarSVMClick, icone: "" };
    var baixarDACTE = { descricao: "Baixar o DACTE (PDF)", id: guid(), metodo: baixarDacteSVMClick, icone: "" };
    var baixarXML = { descricao: "Baixar o XML", id: guid(), metodo: baixarXMLCTeSVMClick, icone: "" };
    var mensagemSEFAZ = { descricao: "Mensagem Sefaz", id: guid(), metodo: retornoSefazSVMClick, icone: "" };
    var visualizar = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), metodo: detalhesCTeSVMClick, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [baixarDACTE, baixarXML, mensagemSEFAZ, visualizar] };

    _gridDadosSVM = new GridView(_etapaSVM.Pesquisar.idGrid, "CargaSVMManual/Pesquisa", _etapaSVM, menuOpcoes);
    _gridDadosSVM.CarregarGrid();
}


function retoronoSefazClick(e, sender) {
    $('#PMensagemRetornoSefaz').html(e.RetornoSefaz);
    Global.abrirModal("divModalRetornoSefaz");
}

function baixarXMLCTeSVMClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadXML", data);
}

function baixarDacteSVMClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadDacte", data);
}

function detalhesCTeSVMClick(e, sender) {
    var codigo = parseInt(e.CodigoCTE);
    var permissoesSomenteLeituraCTe = new Array();
    permissoesSomenteLeituraCTe.push(EnumPermissoesEdicaoCTe.Nenhuma);
    var instancia = new EmissaoCTe(codigo, function () {
        instancia.CRUDCTe.Emitir.visible(false);
        instancia.CRUDCTe.Salvar.visible(false);
        instancia.CRUDCTe.Salvar.eventClick = function () {
            var objetoCTe = ObterObjetoCTe(instancia);
            SalvarCTe(objetoCTe, e.Codigo, instancia);
        }
    }, permissoesSomenteLeituraCTe);
}

//*******ETAPA*******

function EtapaSVMDesabilitada(e) {
    $("#" + e.EtapaSVM.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaSVM.idTab + " .step").attr("class", "step");
    e.EtapaSVM.eventClick = function (e) { buscarDadosSVMClick(e) };
}

function EtapaSVMLiberada(e) {
    $("#" + e.EtapaSVM.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaSVM.idTab + " .step").attr("class", "step yellow");
    e.EtapaSVM.eventClick = function (e) { buscarDadosSVMClick(e) };
}

function EtapaSVMProblema(e) {
    $("#" + e.EtapaSVM.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaSVM.idTab + " .step").attr("class", "step red");
    e.EtapaSVM.eventClick = function (e) { buscarDadosSVMClick(e) };
}

function EtapaSVMAguardando(e) {
    $("#" + e.EtapaSVM.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaSVM.idTab + " .step").attr("class", "step yellow");
    e.EtapaSVM.eventClick = function (e) { buscarDadosSVMClick(e) };
}

function EtapaSVMAprovada(e) {
    $("#" + e.EtapaSVM.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaSVM.idTab + " .step").attr("class", "step green");
    e.EtapaSVM.eventClick = function (e) { buscarDadosSVMClick(e) };
}

function retornoSefazSVMClick(e, sender) {
    $('#PMensagemRetornoSefaz').html(e.RetornoSefaz);
    Global.abrirModal("divModalRetornoSefaz");
}

function DownloadLoteXMLCTeSVMClick(e) {
    executarDownload("CargaCTe/DownloadLoteXML", { Carga: e.Carga.val(), CTesSubContratacaoFilialEmissora: _cargaCTe.CTesSubContratacaoFilialEmissora.val(), CTesSemSubContratacaoFilialEmissora: _cargaCTe.CTesSemSubContratacaoFilialEmissora.val() });
}

function DownloadLoteDACTESVMClick(e) {
    executarDownload("CargaCTe/DownloadLoteDACTE", { Carga: e.Carga.val(), CTesSubContratacaoFilialEmissora: _cargaCTe.CTesSubContratacaoFilialEmissora.val(), CTesSemSubContratacaoFilialEmissora: _cargaCTe.CTesSemSubContratacaoFilialEmissora.val() });
}

function DownloadLoteDocumentosCTeSVMClick(e) {
    executarReST("CargaCTe/DownloadLoteDocumentos", { Carga: e.Carga.val(), CTesSubContratacaoFilialEmissora: _cargaCTe.CTesSubContratacaoFilialEmissora.val(), CTesSemSubContratacaoFilialEmissora: _cargaCTe.CTesSemSubContratacaoFilialEmissora.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.SolicitacaoRealizadaComSucessoFavorAguardeArquivoSerGerado, 20000);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}