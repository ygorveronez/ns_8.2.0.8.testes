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
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTitulo.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _gridCTeTituloReceber;
var _pesquisaCTeTituloReceber;
var _modalObservacao;

var _StatusTituloReceber = [
    { text: "Todos", value: EnumSituacaoTitulo.Todos },
    { text: "Em aberto", value: EnumSituacaoTitulo.EmAberto },
    { text: "Não provisionado", value: EnumSituacaoTitulo.Atrazado },
    { text: "Quitado", value: EnumSituacaoTitulo.Quitado },
    { text: "Bloqueado", value: EnumSituacaoTitulo.Bloqueado }
];

var PesquisaCTeTituloReceber = function () {

    this.NumeroCTe = PropertyEntity({ text: "Número CT-e:", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, visible: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "Data Inicial (Emissão CT-e):", val: ko.observable(Global.PrimeiraDataDoMesAtual()), def: Global.PrimeiraDataDoMesAtual(), getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Final (Emissão CT-e):", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid(), visible: ko.observable(false) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.StatusTitulo = PropertyEntity({ val: ko.observable(EnumSituacaoTitulo.EmAberto), options: _StatusTituloReceber, def: EnumSituacaoTitulo.EmAberto, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCTeTituloReceber.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadCTeTituloReceber() {

    _pesquisaCTeTituloReceber = new PesquisaCTeTituloReceber();
    KoBindings(_pesquisaCTeTituloReceber, "knockoutPesquisaCTeTituloReceber", false, _pesquisaCTeTituloReceber.Pesquisar.id);
    new BuscarTransportadores(_pesquisaCTeTituloReceber.Empresa);
    buscarCTeTituloRecebers();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _pesquisaCTeTituloReceber.Empresa.visible(true);
    }

    _modalObservacao = new bootstrap.Modal(document.getElementById("divModalObservacao"), { backdrop: true, keyboard: true });
}

function cancelarClick(e) {
    limparCamposCTeTituloReceber();
}

//*******MÉTODOS*******

function buscarCTeTituloRecebers() {
    var baixarDACTE = { descricao: "Baixar DACTE", id: guid(), metodo: baixarDacteClick, icone: "" };
    var baixarXML = { descricao: "Baixar XML", id: guid(), metodo: baixarXMLCTeClick, icone: "" };
    var verObservacao = { descricao: "Observação", id: guid(), metodo: observacaoClick, icone: "" };
    var auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("Titulo", "CodigoTitulo"), icone: "", visibilidade: PermiteAuditar }
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [baixarDACTE, baixarXML, verObservacao, auditar] };
    var configuracoesExportacao = { url: "CTeTituloReceber/ExportarPesquisa", titulo: "CT-es a Receber" };

    _gridCTeTituloReceber = new GridViewExportacao("grid-pesquisa-cteTituloReceber", "CTeTituloReceber/Pesquisa", _pesquisaCTeTituloReceber, menuOpcoes, configuracoesExportacao, null, 10);
    _gridCTeTituloReceber.SetPermitirEdicaoColunas(true);
    _gridCTeTituloReceber.SetSalvarPreferenciasGrid(true);
    _gridCTeTituloReceber.CarregarGrid();
}

function limparCamposCTeTituloReceber() {
    _cteTituloReceber.Atualizar.visible(false);
    _cteTituloReceber.Cancelar.visible(false);
    _cteTituloReceber.Excluir.visible(false);
    _cteTituloReceber.Adicionar.visible(true);
    LimparCampos(_cteTituloReceber);
}

function observacaoClick(e, sender) {
    $('#PMensagemObservacao').html(e.Observacao);
    _modalObservacao.show();
}

function baixarXMLCTeClick(e) {
    var data = { CodigoCTe: e.Codigo, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadXML", data);
}

function baixarDacteClick(e) {
    var data = { CodigoCTe: e.Codigo, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadDacte", data);
}
