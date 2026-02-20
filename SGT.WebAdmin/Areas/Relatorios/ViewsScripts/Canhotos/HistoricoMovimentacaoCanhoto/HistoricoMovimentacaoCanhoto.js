/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Empresa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/MotivoRejeicaoAuditoria.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />


//*******MAPEAMENTO KNOUCKOUT*******



var _gridHistoricoMovimentacaoCanhoto, _pesquisaHistoricoMovimentacaoCanhoto, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioHistoricoMovimentacaoCanhoto;

var PesquisaHistoricoMovimentacaoCanhoto = function () {
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.NumeroCanhoto = PropertyEntity({ text: "Número Canhoto: " })
    this.Emitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(''), text: "Emitente ", idBtnSearch: guid() })
    this.DataConfirmacaoEntrega = PropertyEntity({ text: "Data Confirmação Entrega:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataRecebimentoFisico = PropertyEntity({ text: "Data Recibimento Físico:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataReversao = PropertyEntity({ text: "Data Reversao:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataAprovacao = PropertyEntity({ text: "Data Aprovação:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataUpload = PropertyEntity({ text: "Data Upload:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataRejeicao = PropertyEntity({ text: "Data Rejeição:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.MotivoRejeicao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo Rejeição", idBtnSearch: guid() });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário ", idBtnSearch: guid() });
   


};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel"});
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridHistoricoMovimentacaoCanhoto.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

};

//*******EVENTOS*******

function LoadHistoricoMovimentacaoCanhoto() {
    _pesquisaHistoricoMovimentacaoCanhoto = new PesquisaHistoricoMovimentacaoCanhoto();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridHistoricoMovimentacaoCanhoto = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/HistoricoMovimentacaoCanhoto/Pesquisa", _pesquisaHistoricoMovimentacaoCanhoto, null, null, 10);
    _gridHistoricoMovimentacaoCanhoto.SetPermitirEdicaoColunas(true);


    _relatorioHistoricoMovimentacaoCanhoto = new RelatorioGlobal("Relatorios/HistoricoMovimentacaoCanhoto/BuscarDadosRelatorio", _gridHistoricoMovimentacaoCanhoto, function () {
        _relatorioHistoricoMovimentacaoCanhoto.loadRelatorio(function () {
            KoBindings(_pesquisaHistoricoMovimentacaoCanhoto, "knockoutPesquisaHistoricoMovimentacaoCanhoto", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDBaixarRelatorio", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaHistoricoMovimentacaoCanhoto", false);


            new BuscarMotivoRejeicao(_pesquisaHistoricoMovimentacaoCanhoto.MotivoRejeicao, retornoBusquedaMotivo);
            new BuscarFuncionario(_pesquisaHistoricoMovimentacaoCanhoto.Usuario, retornoBusquedaUsuario);
            new BuscarEmpresa(_pesquisaHistoricoMovimentacaoCanhoto.Emitente, retornoBuscaEmitente)

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaHistoricoMovimentacaoCanhoto);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioHistoricoMovimentacaoCanhoto.gerarRelatorio("Relatorios/HistoricoMovimentacaoCanhoto/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioHistoricoMovimentacaoCanhoto.gerarRelatorio("Relatorios/HistoricoMovimentacaoCanhoto/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}


function retornoBusquedaMotivo(data) {
    _pesquisaHistoricoMovimentacaoCanhoto.MotivoRejeicao.codEntity(data.Codigo);
    _pesquisaHistoricoMovimentacaoCanhoto.MotivoRejeicao.val(data.Descricao);
}
function retornoBusquedaUsuario(data) {
    _pesquisaHistoricoMovimentacaoCanhoto.Usuario.codEntity(data.Codigo);
    _pesquisaHistoricoMovimentacaoCanhoto.Usuario.val(data.Nome);
}
function retornoBuscaEmitente(data) {
    const cnpjSemFormato = data.CNPJ_Formatado.replace(/\D+/g, '');
    console.log(cnpjSemFormato, data)
    _pesquisaHistoricoMovimentacaoCanhoto.Emitente.codEntity(cnpjSemFormato);
    _pesquisaHistoricoMovimentacaoCanhoto.Emitente.val(data.Descricao);
}


