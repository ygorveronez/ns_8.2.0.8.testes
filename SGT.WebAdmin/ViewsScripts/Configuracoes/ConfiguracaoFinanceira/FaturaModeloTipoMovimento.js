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
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />

//*******MAPEAMENTO*******

var _gridGeracaoMovimentosFinanceirosPorModeloDocumento;
var _gridGeracaoMovimentosFinanceirosPorModeloDocumentoReversao;

// grid do Habilitar geração de Movimento Financeiro por Modelo de Documento

function LoadGridsModeloTipoMovimento() {
    new BuscarTipoMovimento(_configuracaoFatura.TipoMovimentoModeloDocumento);
    new BuscarModeloDocumentoFiscal(_configuracaoFatura.TipoModeloDocumentoFiscal);
    new BuscarTipoMovimento(_configuracaoFatura.TipoMovimentoModeloDocumentoReversao);
    new BuscarModeloDocumentoFiscal(_configuracaoFatura.TipoModeloDocumentoFiscalReversao);

    LoadGridHabilitarGeracaoMovimentoFinanceiroPorModeloDocumento();
    LoadGridHabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversao();
}

function AdicionarHabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoClick() {
    _configuracaoFatura.TipoMovimentoModeloDocumento.required(true);
    _configuracaoFatura.TipoModeloDocumentoFiscal.required(true);

    if (!ValidarCamposObrigatorios(_configuracaoFatura)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        _configuracaoFatura.TipoMovimentoModeloDocumento.required(false);
        _configuracaoFatura.TipoModeloDocumentoFiscal.required(false);
        return;
    }

    _configuracaoFatura.Codigo.val(guid());
    _configuracaoFatura.GeracaoMovimentosFinanceirosPorModeloDocumento.list.push(SalvarListEntity(_configuracaoFatura));

    _configuracaoFatura.TipoMovimentoModeloDocumento.required(false);
    _configuracaoFatura.TipoModeloDocumentoFiscal.required(false);

    LimparCamposHabilitarGeracaoMovimentoFinanceiroPorModeloDocumento();
}

function ExcluirHabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoClick(data) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o registro?", function () {
        for (var i = 0; i < _configuracaoFatura.GeracaoMovimentosFinanceirosPorModeloDocumento.list.length; i++) {
            if (data.Codigo == _configuracaoFatura.GeracaoMovimentosFinanceirosPorModeloDocumento.list[i].Codigo.val) {
                _configuracaoFatura.GeracaoMovimentosFinanceirosPorModeloDocumento.list.splice(i, 1);
                break;
            }
        }

        LimparCamposHabilitarGeracaoMovimentoFinanceiroPorModeloDocumento();
    });
}

function LoadGridHabilitarGeracaoMovimentoFinanceiroPorModeloDocumento() {
    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: function (data) { ExcluirHabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoClick(data); }, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "TipoMovimento", title: "Tipo Movimento", width: "40%" },
        { data: "Modelo", title: "Modelo", width: "40%" }
    ];

    _gridGeracaoMovimentosFinanceirosPorModeloDocumento = new BasicDataTable(_configuracaoFatura.GridHabilitarGeracaoMovimentoFinanceiroPorModeloDocumento.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridHabilitarGeracaoMovimentoFinanceiroPorModeloDocumento();
}

function RecarregarGridHabilitarGeracaoMovimentoFinanceiroPorModeloDocumento() {
    var data = new Array();

    $.each(_configuracaoFatura.GeracaoMovimentosFinanceirosPorModeloDocumento.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.Modelo = item.TipoModeloDocumentoFiscal.val;
        itemGrid.TipoMovimento = item.TipoMovimentoModeloDocumento.val;

        data.push(itemGrid);
    });

    _gridGeracaoMovimentosFinanceirosPorModeloDocumento.CarregarGrid(data);
}

function LimparCamposHabilitarGeracaoMovimentoFinanceiroPorModeloDocumento() {
    LimparCampo(_configuracaoFatura.TipoMovimentoModeloDocumento);
    LimparCampo(_configuracaoFatura.TipoModeloDocumentoFiscal);
    RecarregarGridHabilitarGeracaoMovimentoFinanceiroPorModeloDocumento();
}

// grid do Habilitar geração de Movimento Financeiro de Reversão por Modelo de Documento

function AdicionarHabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversaoClick() {
    _configuracaoFatura.TipoMovimentoModeloDocumentoReversao.required(true);
    _configuracaoFatura.TipoModeloDocumentoFiscalReversao.required(true);

    if (!ValidarCamposObrigatorios(_configuracaoFatura)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        _configuracaoFatura.TipoMovimentoModeloDocumentoReversao.required(false);
        _configuracaoFatura.TipoModeloDocumentoFiscalReversao.required(false);
        return;
    }

    _configuracaoFatura.TipoMovimentoModeloDocumentoReversao.required(false);
    _configuracaoFatura.TipoModeloDocumentoFiscalReversao.required(false);

    _configuracaoFatura.Codigo.val(guid());
    _configuracaoFatura.GeracaoMovimentosFinanceirosPorModeloDocumentoReversao.list.push(SalvarListEntity(_configuracaoFatura));

    LimparCamposHabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversao();
}

function ExcluirHabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversaoClick(data) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o registro?", function () {
        for (var i = 0; i < _configuracaoFatura.GeracaoMovimentosFinanceirosPorModeloDocumentoReversao.list.length; i++) {
            if (data.Codigo == _configuracaoFatura.GeracaoMovimentosFinanceirosPorModeloDocumentoReversao.list[i].Codigo.val) {
                _configuracaoFatura.GeracaoMovimentosFinanceirosPorModeloDocumentoReversao.list.splice(i, 1);
                break;
            }
        }

        LimparCamposHabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversao();
    });
}

function LoadGridHabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversao() {
    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: function (data) { ExcluirHabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversaoClick(data); }, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "TipoMovimento", title: "Tipo Movimento", width: "40%" },
        { data: "Modelo", title: "Modelo", width: "40%" }
    ];

    _gridGeracaoMovimentosFinanceirosPorModeloDocumentoReversao = new BasicDataTable(_configuracaoFatura.GridHabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversao.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridHabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversao();
}

function RecarregarGridHabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversao() {
    var data = new Array();

    $.each(_configuracaoFatura.GeracaoMovimentosFinanceirosPorModeloDocumentoReversao.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.Modelo = item.TipoMovimentoModeloDocumentoReversao.val;
        itemGrid.TipoMovimento = item.TipoModeloDocumentoFiscalReversao.val;

        data.push(itemGrid);
    });

    _gridGeracaoMovimentosFinanceirosPorModeloDocumentoReversao.CarregarGrid(data);
}

function LimparCamposHabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversao() {
    LimparCampo(_configuracaoFatura.TipoMovimentoModeloDocumentoReversao);
    LimparCampo(_configuracaoFatura.TipoModeloDocumentoFiscalReversao);
    RecarregarGridHabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversao();
}