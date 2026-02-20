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
/// <reference path="../../Consultas/TipoDespesaFinanceira.js" />
/// <reference path="../../Enumeradores/EnumMes.js" />
/// <reference path="../DespesaMensal/DespesaMensal.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridDespesaMensalProcessamento;
var _despesaMensalProcessamento;
var _pesquisaDespesaMensalProcessamento;
var _gridSelecaoDespesaMensal;

var PesquisaDespesaMensalProcessamento = function () {
    this.TipoDespesa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Despesa:", idBtnSearch: guid() });
    this.Mes = PropertyEntity({ val: ko.observable(EnumMes.Todos), options: EnumMes.obterOpcoesPesquisa(), def: EnumMes.Todos, text: "Mês de processamento:" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDespesaMensalProcessamento.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var DespesaMensalProcessamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Mes = PropertyEntity({ text: "*Mês para processamento: ", val: ko.observable(moment().format("MM")), options: EnumMes.obterOpcoes(), def: moment().format("MM"), required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoDespesa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Despesa:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.DataPagamento = PropertyEntity({ text: "*Data Pagamento: ", getType: typesKnockout.date, required: true, enable: ko.observable(true), visible: ko.observable(true) });

    this.BuscarDespesa = PropertyEntity({ eventClick: buscarDespesasClick, type: types.event, text: "Buscar Despesas", enable: ko.observable(true), visible: ko.observable(true) });
    this.AdicionarNovaDespesa = PropertyEntity({ eventClick: adicionarNovaDespesaMensalClick, type: types.event, text: "Adicionar Nova Despesa", enable: ko.observable(true), visible: ko.observable(true) });

    this.Despesas = PropertyEntity({ idGrid: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(true), enable: ko.observable(true) });
    this.FazendoABusca = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool });
    this.DespesasSelecionadas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.ValorTotalPagar = PropertyEntity({ type: types.map, val: ko.observable("0,00"), def: "0,00", text: "Valor Total a Pagar R$:", enable: ko.observable(true) });
    this.QuantidadeDespesas = PropertyEntity({ type: types.map, val: ko.observable(0), def: 0, text: "Quantidade Despesas Selecionadas:", enable: ko.observable(true) });
}

var CRUDDespesaMensalProcessamento = function () {
    this.Processar = PropertyEntity({ eventClick: processarClick, type: types.event, text: "Processar", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar / Novo Processamento", visible: ko.observable(true) });
};

//*******EVENTOS*******


function loadDespesaMensalProcessamento() {
    _despesaMensalProcessamento = new DespesaMensalProcessamento();
    KoBindings(_despesaMensalProcessamento, "knockoutCadastroDespesaMensalProcessamento");

    HeaderAuditoria("DespesaMensalProcessamento", _despesaMensalProcessamento);

    _crudDespesaMensalProcessamento = new CRUDDespesaMensalProcessamento();
    KoBindings(_crudDespesaMensalProcessamento, "knockoutCRUDDespesaMensalProcessamento");

    _pesquisaDespesaMensalProcessamento = new PesquisaDespesaMensalProcessamento();
    KoBindings(_pesquisaDespesaMensalProcessamento, "knockoutPesquisaDespesaMensalProcessamento", false, _pesquisaDespesaMensalProcessamento.Pesquisar.id);

    new BuscarTipoDespesaFinanceira(_pesquisaDespesaMensalProcessamento.TipoDespesa);
    new BuscarTipoDespesaFinanceira(_despesaMensalProcessamento.TipoDespesa);

    var somenteLeitura = false;
    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: function () {
            AtualizarTotalizadoresDespesaMensalSelecionado();
        },
        callbackNaoSelecionado: function () {
            AtualizarTotalizadoresDespesaMensalSelecionado();
        },
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _despesaMensalProcessamento.SelecionarTodos,
        somenteLeitura: somenteLeitura
    }
    var editarColuna = { permite: true, callback: null, atualizarRow: true };

    _gridSelecaoDespesaMensal = new GridView(_despesaMensalProcessamento.Despesas.idGrid, "DespesaMensalProcessamento/PesquisaDespesasMensais", _despesaMensalProcessamento, null, null, null, null, null, null, multiplaescolha, null, editarColuna);
    _gridSelecaoDespesaMensal.CarregarGrid();

    buscarDespesaMensalProcessamento();
    carregarLancamentoDespesaMensal("conteudoDespesaMensal");
}

function processarClick(e, sender) {
    var despesasSelecionadas = _gridSelecaoDespesaMensal.ObterMultiplosSelecionados();
    if (despesasSelecionadas.length > 0) {
        var dataGrid = new Array();
        var valido = true;

        $.each(despesasSelecionadas, function (i, despesa) {

            var obj = new Object();
            obj.Codigo = despesa.Codigo;
            obj.ValorPago = despesa.ValorPago;
            if (Globalize.parseFloat(despesa.ValorPago) <= 0) {
                exibirMensagem(tipoMensagem.atencao, "Atenção", "Existe despesa sem o valor pago informado!");
                valido = false;
                return;
            }

            dataGrid.push(obj);
        });

        if (valido) {
            _despesaMensalProcessamento.DespesasSelecionadas.val(JSON.stringify(dataGrid));
            Salvar(_despesaMensalProcessamento, "DespesaMensalProcessamento/Processar", function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Processado com sucesso");
                        _gridDespesaMensalProcessamento.CarregarGrid();
                        limparCamposDespesaMensalProcessamento();
                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            }, sender);
        }
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Favor selecionar ao menos uma despesa para processar!");
}

function limparClick(e) {
    limparCamposDespesaMensalProcessamento();
}

function adicionarNovaDespesaMensalClick(e, sender) {
    limparCamposDespesaMensal();
    Global.abrirModal('divModalDespesaMensal');
}

function buscarDespesasClick() {
    _despesaMensalProcessamento.SelecionarTodos.val(false);
    _despesaMensalProcessamento.FazendoABusca.val(true);
    _gridSelecaoDespesaMensal.CarregarGrid();
}

//*******MÉTODOS*******

function AtualizarTotalizadoresDespesaMensalSelecionado() {
    if (_despesaMensalProcessamento.Codigo.val() == 0) {
        var despesasSelecionadas = _gridSelecaoDespesaMensal.ObterMultiplosSelecionados();
        var valorTotalSelecionado = 0.0;
        _despesaMensalProcessamento.QuantidadeDespesas.val(despesasSelecionadas.length);

        if (despesasSelecionadas.length > 0) {
            $.each(despesasSelecionadas, function (i, despesaMensal) {
                valorTotalSelecionado = valorTotalSelecionado + Globalize.parseFloat(despesaMensal.ValorPago);
                _despesaMensalProcessamento.ValorTotalPagar.val(Globalize.format(valorTotalSelecionado, "n2"));
            });
        } else
            _despesaMensalProcessamento.ValorTotalPagar.val("0,00");
    }
}

function buscarDespesaMensalProcessamento() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarDespesaMensalProcessamento, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridDespesaMensalProcessamento = new GridView(_pesquisaDespesaMensalProcessamento.Pesquisar.idGrid, "DespesaMensalProcessamento/Pesquisa", _pesquisaDespesaMensalProcessamento, menuOpcoes, null);
    _gridDespesaMensalProcessamento.CarregarGrid();
}

function editarDespesaMensalProcessamento(despesaMensalProcessamentoGrid) {
    _despesaMensalProcessamento.Codigo.val(despesaMensalProcessamentoGrid.Codigo);
    BuscarPorCodigo(_despesaMensalProcessamento, "DespesaMensalProcessamento/BuscarPorCodigo", function (arg) {
        _pesquisaDespesaMensalProcessamento.ExibirFiltros.visibleFade(false);
        _crudDespesaMensalProcessamento.Processar.visible(false);
        _despesaMensalProcessamento.BuscarDespesa.visible(false);
        _despesaMensalProcessamento.SelecionarTodos.visible(false);

        _despesaMensalProcessamento.TipoDespesa.enable(false);
        _despesaMensalProcessamento.Mes.enable(false);
        _despesaMensalProcessamento.DataPagamento.enable(false);

        _gridSelecaoDespesaMensal.CarregarGrid();
    }, null);
}

function limparCamposDespesaMensalProcessamento() {
    _crudDespesaMensalProcessamento.Processar.visible(true);
    _despesaMensalProcessamento.BuscarDespesa.visible(true);
    _despesaMensalProcessamento.SelecionarTodos.visible(true);

    _despesaMensalProcessamento.TipoDespesa.enable(true);
    _despesaMensalProcessamento.Mes.enable(true);
    _despesaMensalProcessamento.DataPagamento.enable(true);

    LimparCampos(_despesaMensalProcessamento);
    _gridSelecaoDespesaMensal.CarregarGrid();
}