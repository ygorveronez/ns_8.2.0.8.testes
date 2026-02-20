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


//*******MAPEAMENTO KNOUCKOUT*******

var _agruparTabelaFreteCliente;
var _gridAgruparTabelaFreteCliente;

var AgruparTabelaFreteCliente = function () {
    // Codigo da entidade
    this.TabelaFrete = PropertyEntity({ idGrid: guid()});
    this.CodigoIntegracao = PropertyEntity({ text: "Código: " });
    this.Buscar = PropertyEntity({ eventClick: BuscarTabelas, type: types.event, text: "Buscar", visible: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Selecionar Todos", visible: ko.observable(true) });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Processar", visible: ko.observable(true) });
}


//*******EVENTOS*******
function loadAgruparTabelaFreteCliente() {
    //-- Knouckout
    // Instancia objeto principal
    _agruparTabelaFreteCliente = new AgruparTabelaFreteCliente();
    KoBindings(_agruparTabelaFreteCliente, "knockoutAgruparTabelaFreteCliente");

    CarregarGrid();
}

function adicionarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Você realmente deseja agrupar as tabelas de frete selecionadas selecionados?", function () {
        var dados = {
            CodigoIntegracao: _agruparTabelaFreteCliente.CodigoIntegracao.val(),
            SelecionarTodos: _agruparTabelaFreteCliente.SelecionarTodos.val(),
            ItensSelecionados: JSON.stringify(_gridAgruparTabelaFreteCliente.ObterMultiplosSelecionados()),
            ItensNaoSelecionados: JSON.stringify(_gridAgruparTabelaFreteCliente.ObterMultiplosNaoSelecionados())
        }

        executarReST("AgruparTabelaFreteCliente/Processar", dados, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Agrupado com sucesso.");
                LimparCamposAgruparTabelaFreteCliente();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function cancelarClick(e) {
    LimparCamposAgruparTabelaFreteCliente();
}



//*******MÉTODOS*******
function CarregarGrid() {
    var multiplaEscolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _agruparTabelaFreteCliente.SelecionarTodos,
        somenteLeitura: false
    };

    _gridAgruparTabelaFreteCliente = new GridView(_agruparTabelaFreteCliente.TabelaFrete.idGrid, "AgruparTabelaFreteCliente/Pesquisa", _agruparTabelaFreteCliente, null, null, 25, null, null, null, multiplaEscolha);
    _gridAgruparTabelaFreteCliente.CarregarGrid();
}

function BuscarTabelas() {
    _agruparTabelaFreteCliente.SelecionarTodos.val(false);

    _gridAgruparTabelaFreteCliente.CarregarGrid(function () {
        setTimeout(function() {
            _agruparTabelaFreteCliente.SelecionarTodos.get$().click();
        }, 50);
    });
}

function LimparCamposAgruparTabelaFreteCliente() {
    LimparCampos(_agruparTabelaFreteCliente);
    _gridAgruparTabelaFreteCliente.CarregarGrid();
}