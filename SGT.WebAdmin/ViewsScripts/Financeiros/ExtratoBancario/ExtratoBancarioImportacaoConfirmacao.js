/// <reference path="../../Consultas/TipoOrdemServico.js" />
/// <reference path="../../Consultas/ServicoVeiculo.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Equipamento.js" />
/// <reference path="../../Consultas/Produto.js" />

/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/ExtratoBancarioTipoLancamento.js" />
/// <reference path="../../Enumeradores/EnumAnaliticoSintetico.js" />
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
/// <reference path="../../Enumeradores/EnumTipoDocumentoMovimento.js" />
/// <reference path="../../Enumeradores/EnumTipoGeracaoMovimento.js" />
/// <reference path="../../Enumeradores/EnumFinalidadeTipoMovimento.js" />
/// <reference path="../../Enumeradores/EnumDebitoCredito.js" />
/// <reference path="../RateioDespesaVeiculo/RateioDespesaVeiculo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridImportacaoConfirmacao;
var _importacaoInformacao;
var _CRUDImportacaoConfirmacao;

var ImportacaoConfirmacao = function () {
    this.SaldoInicial = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.SaldoFinal = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.Itens = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    //Grid da exibição
    this.Grid = PropertyEntity({ type: types.local });
};

var CRUDImportacaoConfirmacao = function () {
    this.Confirmar = PropertyEntity({ eventClick: importacaoConfirmacaoClick, type: types.event, text: "Confirmar" });
    this.Cancelar = PropertyEntity({ eventClick: limparImportacaoConfirmacaoClick, type: types.event, text: "Cancelar" });
};

//*******EVENTOS*******

function LoadImportacaoConfirmacao() {
    _importacaoInformacao = new ImportacaoConfirmacao();
    KoBindings(_importacaoInformacao, "knockoutImportacaoConfirmacao");

    _CRUDImportacaoConfirmacao = new CRUDImportacaoConfirmacao();
    KoBindings(_CRUDImportacaoConfirmacao, "knockoutCRUDImportacaoConfirmacao");

    LoadGridImportacaoConfirmacao();
}

function LoadGridImportacaoConfirmacao() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "Data", title: "Data", width: "7%" },
        { data: "Valor", title: "Valor", width: "10%" },
        { data: "DebitoCredito", title: "Débito/Crédito", width: "10%" },
        { data: "TipoDocumento", title: "Tipo do Documento", width: "12%" },
        { data: "NumeroDocumento", title: "Número do Documento", width: "10%" },
        { data: "PlanoConta", title: "Plano de Conta", width: "10%" },
        { data: "TipoLancamento", title: "Tipo de Lançamento", width: "10%" },
        { data: "CodigoTipoLancamento", title: "Código Tipo Lançamento", width: "12%" },
        { data: "Observacao", title: "Observação", width: "19%" }
    ];

    _gridImportacaoConfirmacao = new BasicDataTable(_importacaoInformacao.Grid.id, header, false, { column: 0, dir: orderDir.asc }, null, 100);
}

function RecarregarGridImportacaoConfirmacao() {
    var data = new Array();

    $.each(_importacaoInformacao.Itens.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.Data = item.Data.val;
        itemGrid.Valor = item.Valor.val;
        itemGrid.DebitoCredito = item.DebitoCredito.val;
        itemGrid.TipoDocumento = item.TipoDocumento.val;
        itemGrid.NumeroDocumento = item.NumeroDocumento.val;
        itemGrid.PlanoConta = item.PlanoConta.val;
        itemGrid.TipoLancamento = item.TipoLancamento.val;
        itemGrid.CodigoTipoLancamento = item.CodigoTipoLancamento.val;
        itemGrid.Observacao = item.Observacao.val;

        data.push(itemGrid);
    });

    _gridImportacaoConfirmacao.CarregarGrid(data);
}

function LimparCamposImportacaoConfirmacao() {
    LimparCampos(_importacaoInformacao);
    RecarregarGridImportacaoConfirmacao();
}

function limparImportacaoConfirmacaoClick(e) {
    Global.fecharModal('divModalImportacaoConfirmacao');
    LimparCamposImportacaoConfirmacao();
}

function importacaoConfirmacaoClick(e, sender) {
    var file = document.getElementById(_extratoBancario.Arquivo.id);

    var formData = new FormData();
    formData.append("upload", file.files[0]);

    var parametros = {
        CodigoPlanoConta: _extratoBancario.PlanoContaImportacao.codEntity(),
        CodigoEmpresa: _extratoBancario.EmpresaImportacao.codEntity(),
        ApenasLeitura: false
    };
    enviarArquivo("ExtratoBancario/ImportarExtratoBancario?callback=?", parametros, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                _gridExtratoBancario.CarregarGrid();
                resetarTabs();
                Global.fecharModal('divModalImportacaoConfirmacao');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}