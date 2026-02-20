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
/// <reference path="../../Consultas/Empresa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _arquivoEBS, _arquivoEBSFinanceiro, _gridNotasEntrada, _arquivoEBSBaixas, _arquivoEBSComissaoMotorista;

var _DocumentoEmEBS = [
    { text: "Todos documentos", value: 0 },
    { text: "Somente documentos SEM arquivo EBS gerado", value: 1 },
    { text: "Somente documentos COM arquivo EBS gerado", value: 2 } //enumerador para que?
];

var _ProdutoEmEBS = [
    { text: "Todos produtos", value: 0 },
    { text: "Somente produtos SEM arquivo EBS gerado", value: 1 },
    { text: "Somente produtos COM arquivo EBS gerado", value: 2 } //enumerador para que? foda-se o padrão do sistema...
];

var _TipoEBSComissao = [
    { text: "Comissões", value: EnumTipoEBSComissao.Comissao },
    { text: "Diárias", value: EnumTipoEBSComissao.Diaria },
    { text: "Média", value: EnumTipoEBSComissao.Media },
    { text: "Produtividade", value: EnumTipoEBSComissao.Produtividade }
];

var _opcoesTipoSistemaContabil = [
    { text: "EBS", value: EnumTipoSistemaContabil.EBS },
    { text: "Questor", value: EnumTipoSistemaContabil.Questor }
];

var ArquivoEBS = function () {
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true), required: true, enable: ko.observable(false) });
    this.ModelosDocumento = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, url: "ModeloDocumentoFiscal/ObterTodos", params: { Tipo: 0, Ativo: _statusPesquisa.Todos, OpcaoSemGrupo: false }, text: "Modelos de Documentos: ", options: ko.observable(new Array()), visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-6 col-lg-6") });

    this.DataEntradaInicial = PropertyEntity({ text: "Data Inicial de Entrada: ", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataEntradaFinal = PropertyEntity({ text: "Data Final de Entrada: ", dateRangeInit: this.DataEntradaInicial, getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataEmissaoInicial = PropertyEntity({ text: "Data Inicial de Emissão: ", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data Final de Emissão: ", dateRangeInit: this.DataEmissaoInicial, getType: typesKnockout.date, enable: ko.observable(true) });
    this.DocumentoEmEBS = PropertyEntity({ val: ko.observable(1), options: _DocumentoEmEBS, text: "Documento gerado em arquivo EBS: ", def: 1, required: true, enable: ko.observable(true) });
    this.ProdutoEmEBS = PropertyEntity({ val: ko.observable(1), options: _ProdutoEmEBS, text: "Produto gerado em arquivo EBS: ", def: 1, required: true, enable: ko.observable(true) });

    this.PesquisarNotasEntrada = PropertyEntity({ eventClick: PesquisarNotasEntradaClick, type: types.event, text: "Filtrar", visible: ko.observable(true), enable: ko.observable(true) });
    this.NotasEntrada = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(true), def: true, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(true), enable: ko.observable(true) });
    this.ListaNotas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.ArquivoProdutos = PropertyEntity({ eventClick: ArquivoProdutosClick, type: types.event, text: "Gerar EBS dos Produtos", visible: ko.observable(true) });
    this.ArquivoNotas = PropertyEntity({ eventClick: ArquivoNotasClick, type: types.event, text: "Gerar EBS de Notas Fiscais de Entrada", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var ArquivoEBSFinanceiro = function () {
    this.DataMovimentoInicial = PropertyEntity({ text: "Data Inicial do Movimento: ", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataMovimentoFinal = PropertyEntity({ text: "Data Final do Movimento: ", dateRangeInit: this.DataMovimentoInicial, getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataBaseInicial = PropertyEntity({ text: "Data Inicial da Base: ", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataBaseFinal = PropertyEntity({ text: "Data Final da Base: ", dateRangeInit: this.DataBaseInicial, getType: typesKnockout.date, enable: ko.observable(true) });

    this.ArquivoContas = PropertyEntity({ eventClick: ArquivoFinanceiroClick, type: types.event, text: "Gerar EBS dos Pagamentos e Recebimentos", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarEBSFinanceiroClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var ArquivoEBSBaixas = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial da Baixa: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataFinal = PropertyEntity({ text: "Data Final da Baixa: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });

    this.GerarArquivo = PropertyEntity({ eventClick: GerarArquivoEBSBaixasClick, type: types.event, text: "Gerar EBS das Baixas", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarArquivoEBSBaixasClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var ArquivoEBSComissaoMotorista = function () {
    this.CodigoEvento = PropertyEntity({ text: "*Código do Evento: ", getType: typesKnockout.int, val: ko.observable(""), def: "", required: true, maxlength: 3 });
    this.ComissaoMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Comissão de Motorista:", idBtnSearch: guid(), issue: 0, required: true });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoEBSComissao.Comissao), options: _TipoEBSComissao, text: "*Tipo: ", def: EnumTipoEBSComissao.Comissao, required: true, enable: ko.observable(true) });
    this.TipoSistemaContabil = PropertyEntity({ val: ko.observable(EnumTipoSistemaContabil.EBS), options: _opcoesTipoSistemaContabil, text: "*Tipo Sistema Contábil: ", def: EnumTipoSistemaContabil.EBS, required: true, enable: ko.observable(true) });

    this.GerarArquivo = PropertyEntity({ eventClick: GerarArquivoEBSComissaoMotoristaClick, type: types.event, text: "Gerar EBS das Comissões de Motoristas", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarArquivoEBSComissaoMotoristaClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadArquivoEBS() {
    _arquivoEBS = new ArquivoEBS();
    KoBindings(_arquivoEBS, "knockoutGeracaoArquivoEBS");

    new BuscarEmpresa(_arquivoEBS.Filial);

    _arquivoEBSBaixas = new ArquivoEBSBaixas();
    KoBindings(_arquivoEBSBaixas, "knockoutArquivoEBSBaixas");

    _arquivoEBSComissaoMotorista = new ArquivoEBSComissaoMotorista();
    KoBindings(_arquivoEBSComissaoMotorista, "knockoutArquivoEBSComissaoMotorista");

    BuscarComissoesMotoristas(_arquivoEBSComissaoMotorista.ComissaoMotorista, null);

    buscarNotasEntrada();
}

function PesquisarNotasEntradaClick(e, sender) {
    _gridNotasEntrada.CarregarGrid();
}

function ArquivoProdutosClick(e, sender) {
    var data = {
        DataEntradaInicial: _arquivoEBS.DataEntradaInicial.val(),
        DataEntradaFinal: _arquivoEBS.DataEntradaFinal.val(),
        DataEmissaoInicial: _arquivoEBS.DataEmissaoInicial.val(),
        DataEmissaoFinal: _arquivoEBS.DataEmissaoFinal.val(),
        Filial: _arquivoEBS.Filial.codEntity(),
        ModelosDocumento: JSON.stringify(_arquivoEBS.ModelosDocumento.val()),
        ProdutoEmEBS: _arquivoEBS.ProdutoEmEBS.val()
    };

    executarDownload("ArquivoEBS/DownloadEBSProduto", data);
}

function ArquivoNotasClick(e, sender) {
    var notasSelecionadas = null;

    if (_arquivoEBS.SelecionarTodos.val()) {
        notasSelecionadas = _gridNotasEntrada.ObterMultiplosNaoSelecionados();
    } else {
        notasSelecionadas = _gridNotasEntrada.ObterMultiplosSelecionados();
    }

    var codigosNotas = new Array();

    for (var i = 0; i < notasSelecionadas.length; i++)
        codigosNotas.push(notasSelecionadas[i].DT_RowId);

    if (codigosNotas && (codigosNotas.length > 0 || _arquivoEBS.SelecionarTodos.val())) {

        var data = {
            DataEntradaInicial: _arquivoEBS.DataEntradaInicial.val(),
            DataEntradaFinal: _arquivoEBS.DataEntradaFinal.val(),
            DataEmissaoInicial: _arquivoEBS.DataEmissaoInicial.val(),
            DataEmissaoFinal: _arquivoEBS.DataEmissaoFinal.val(),
            Filial: _arquivoEBS.Filial.codEntity(),
            ModelosDocumento: JSON.stringify(_arquivoEBS.ModelosDocumento.val()),
            ListaNotas: JSON.stringify(codigosNotas),
            SelecionarTodos: _arquivoEBS.SelecionarTodos.val(),
            DocumentoEmEBS: _arquivoEBS.DocumentoEmEBS.val()
        };

        executarDownload("ArquivoEBS/DownloadEBSNotaEntrada", data);
    }
}

function cancelarClick(e) {
    limparCamposArquivoEBS();
}

function ArquivoFinanceiroClick(e, sender) {
}

function cancelarEBSFinanceiroClick(e) {
    limparCamposArquivoEBSFinanceiro();
}

function GerarArquivoEBSBaixasClick(e) {
    var data = {
        DataInicial: _arquivoEBSBaixas.DataInicial.val(),
        DataFinal: _arquivoEBSBaixas.DataFinal.val()
    };

    executarDownload("ArquivoEBS/DownloadEBSBaixas", data);
}

function CancelarArquivoEBSBaixasClick() {
    LimparCampos(_arquivoEBSBaixas);
}

function GerarArquivoEBSComissaoMotoristaClick(e) {
    var data = {
        CodigoEvento: _arquivoEBSComissaoMotorista.CodigoEvento.val(),
        ComissaoMotorista: _arquivoEBSComissaoMotorista.ComissaoMotorista.codEntity(),
        Tipo: _arquivoEBSComissaoMotorista.Tipo.val(),
        TipoSistemaContabil: _arquivoEBSComissaoMotorista.TipoSistemaContabil.val()
    };

    executarDownload("ArquivoEBS/DownloadEBSComissaoMotorista", data);
}

function CancelarArquivoEBSComissaoMotoristaClick() {
    LimparCampos(_arquivoEBSComissaoMotorista);
}

//*******MÉTODOS*******

function buscarNotasEntrada() {
    var somenteLeitura = false;

    _arquivoEBS.SelecionarTodos.visible(true);
    _arquivoEBS.SelecionarTodos.val(true);

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _arquivoEBS.SelecionarTodos,
        somenteLeitura: somenteLeitura
    };

    _gridNotasEntrada = new GridView(_arquivoEBS.NotasEntrada.idGrid, "ArquivoEBS/PesquisaNotasEntrada", _arquivoEBS, null, null, null, null, null, null, multiplaescolha);
    _gridNotasEntrada.CarregarGrid();
}

function limparCamposArquivoEBS() {
    LimparCampos(_arquivoEBS);
}

function limparCamposArquivoEBSFinanceiro() {
    LimparCampos(_arquivoEBSFinanceiro);
}
