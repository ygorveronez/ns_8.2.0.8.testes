/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Anexo.js" />
/// <reference path="../../Consultas/Veiculo.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _pagamento;
var _pagamentoConfigDecimal = { precision: 3, allowZero: false, allowNegative: false };

var Pagamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Valor = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal });

    this.Numero = PropertyEntity({ type: types.map, getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Nº Pagamento:", enable: ko.observable(false) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "* Agregado:", idBtnSearch: guid(), required: true, enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4"), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.TomadorFatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador da fatura:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.NumeroFatura = PropertyEntity({ getType: typesKnockout.int, text: "Nº Fatura:", val: ko.observable(0), def: 0, maxlength: 15, configInt: { precision: 0, allowZero: true, thousands: "" }, visible: ko.observable(false) });

    this.DataInicial = PropertyEntity({ type: types.map, getType: typesKnockout.date, val: ko.observable(""), def: "", text: "Data Inicial Emissão CT-e:", enable: ko.observable(true), required: false });
    this.DataFinal = PropertyEntity({ type: types.map, getType: typesKnockout.date, val: ko.observable(""), def: "", text: "Data Final Emissão CT-e:", enable: ko.observable(true), required: false });
    this.DataInicialOcorrencia = PropertyEntity({ type: types.map, getType: typesKnockout.date, val: ko.observable(""), def: "", text: "Data Inicial Ocorrência Finalizadora:", enable: ko.observable(true), required: false });
    this.DataFinalOcorrencia = PropertyEntity({ type: types.map, getType: typesKnockout.date, val: ko.observable(""), def: "", text: "Data Final Ocorrência Finalizadora:", enable: ko.observable(true), required: false });
    this.DataPagamento = PropertyEntity({ type: types.map, getType: typesKnockout.date, val: ko.observable(""), def: "", text: "* Data Pagamento:", enable: ko.observable(true), required: true });

    this.CompetenciaMes = PropertyEntity({ text: "Competência:", options: EnumMes.obterOpcoes(), val: ko.observable(EnumMes.Janeiro), def: EnumMes.Janeiro, visible: ko.observable(false) });
    this.CompetenciaQuinzena = PropertyEntity({ text: "Quinzena:", options: EnumQuinzena.obterOpcoes(), val: ko.observable(EnumQuinzena.Primeira), def: EnumQuinzena.Primeira, visible: ko.observable(false) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(_CONFIGURACAO_TMS.LimitarOperacaoPorEmpresa), visible: ko.observable(_CONFIGURACAO_TMS.LimitarOperacaoPorEmpresa) });
    this.DescricaoCompetencia = PropertyEntity({ getType: typesKnockout.string, text: "Descrição da competência:", val: ko.observable(""), def: "", maxlength: 200, visible: ko.observable(false) });

    this.Observacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Observação:", enable: ko.observable(true), required: false, maxlength: 2000 });

    this.ListaDocumentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaAdiantamentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaAbastecimentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaAnexos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaAnexosNovos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaAnexosExcluidos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
}


//*******EVENTOS*******
function loadPagamento() {
    _pagamento = new Pagamento();
    KoBindings(_pagamento, "knockoutPagamentoAgregado");

    new BuscarClientes(_pagamento.Cliente);
    loadPagamentoAgregadoAnexo();

    new BuscarVeiculos(_pagamento.Veiculo, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, _pagamento.Cliente);
    new BuscarClientes(_pagamento.TomadorFatura);
    new BuscarTransportadores(_pagamento.Empresa, null, null, null, null, null, null, null, null, null, true);

    if (_CONFIGURACAO_TMS.UtilizarNovoLayoutPagamentoAgregado) {
        _pagamento.Veiculo.visible(true);
    }

    if (_CONFIGURACAO_TMS.HabilitarLayoutFaturaPagamentoAgregado) {
        _pagamento.NumeroFatura.visible(true);
        _pagamento.TomadorFatura.visible(true);
        _pagamento.CompetenciaMes.visible(true);
        _pagamento.CompetenciaQuinzena.visible(true);
        _pagamento.DescricaoCompetencia.visible(true);
    }

}


//*******MÉTODOS*******
function EditarDadosPagamento(data) {
    _pagamento.Codigo.val(data.Codigo);
    if (data.DadosPagamento != null) {
        PreencherObjetoKnout(_pagamento, { Data: data.DadosPagamento });
    } else {
        _pagamento.Emitir.visible(false);
    }
}

function AcrescentarValor(valor) {
    var valorAtual = parseFloat(_pagamento.Valor.val().replace(".", "").replace(",", "."));
    valorAtual += valor;
    _pagamento.Valor.val(mvalor(valorAtual.toFixed(2).toString()));
}

function DiminuirValor(valor) {
    var valorAtual = parseFloat(_pagamento.Valor.val().replace(".", "").replace(",", "."));
    valorAtual -= valor;
    _pagamento.Valor.val(mvalor(valorAtual.toFixed(2).toString()));
}

function ControleCamposPagamento(status) {
    _pagamento.Cliente.enable(status);
    _pagamento.DataInicial.enable(status);
    _pagamento.DataFinal.enable(status);
    _pagamento.DataInicialOcorrencia.enable(status);
    _pagamento.DataFinalOcorrencia.enable(status);
    _pagamento.DataPagamento.enable(status);
    _pagamento.Observacao.enable(status);
}

function LimparCamposDadosPagamento() {
    LimparCampos(_pagamento);
    ControleCamposPagamento(true);
}

function mvalor(v) {
    v = v.replace(/\D/g, "");
    v = v.replace(/(\d)(\d{8})$/, "$1.$2");
    v = v.replace(/(\d)(\d{5})$/, "$1.$2");

    v = v.replace(/(\d)(\d{2})$/, "$1,$2");
    return v;
}

function preencherListasSelecao() {
    _pagamento.ListaDocumentos.list = new Array();
    _pagamento.ListaAdiantamentos.list = new Array();
    _pagamento.ListaAbastecimentos.list = new Array();

    var documentos = new Array();
    var adiantamentos = new Array();
    var abastecimentos = new Array();

    $.each(_documento.Documentos.basicTable.BuscarRegistros(), function (i, doc) {
        documentos.push({ Documento: doc });
    });

    $.each(_documento.Adiantamentos.basicTable.BuscarRegistros(), function (i, adiantamento) {
        adiantamentos.push({ Adiantamento: adiantamento });
    });

    $.each(_documento.Abastecimentos.basicTable.BuscarRegistros(), function (i, abastecimento) {
        abastecimentos.push({ Abastecimento: abastecimento });
    });

    _pagamento.ListaDocumentos.val(JSON.stringify(documentos))
    _pagamento.ListaAdiantamentos.val(JSON.stringify(adiantamentos))
    _pagamento.ListaAbastecimentos.val(JSON.stringify(abastecimentos))
}