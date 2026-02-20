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
/// <reference path="TransportadorTerceiro.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridAcrescimoDescontoAutomatico;
var _acrescimoDescontoAutomatico;

var AcrescimoDescontoAutomatico = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Regra = PropertyEntity({ type: types.event, text: Localization.Resources.Pessoas.Pessoa.AdicionarAcrescimoDescontoAutomatico, idBtnSearch: guid() });
}


//*******EVENTOS*******

function loadAcrescimoDescontoAutomatico() {

    _acrescimoDescontoAutomatico = new AcrescimoDescontoAutomatico();
    KoBindings(_acrescimoDescontoAutomatico, "knockoutAcrescimoDescontoAutomatico");


    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Pessoas.Pessoa.Excluir, id: guid(), metodo: function (data) {
                excluirAcrescimoDescontoAutomaticoClick(_acrescimoDescontoAutomatico.Regra, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Pessoas.Pessoa.Descricao, width: "20%" },
        { data: "Justificativa", title: Localization.Resources.Pessoas.Pessoa.Justificativa, width: "20%" },
        { data: "Valor", title: Localization.Resources.Pessoas.Pessoa.Valor, width: "10%" },
        { data: "TipoValor", title: Localization.Resources.Pessoas.Pessoa.TipoValor, width: "10%" },
        { data: "TipoCalculo", title: Localization.Resources.Pessoas.Pessoa.TipoCalculo, width: "20%" },
        { data: "Observacoes", title: Localization.Resources.Pessoas.Pessoa.Observacoes, width: "20%" },

    ];

    _gridAcrescimoDescontoAutomatico = new BasicDataTable(_acrescimoDescontoAutomatico.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarAcrescimoDescontoAutomatico(_acrescimoDescontoAutomatico.Regra, null, _gridAcrescimoDescontoAutomatico);
    _acrescimoDescontoAutomatico.Regra.basicTable = _gridAcrescimoDescontoAutomatico;

    recarregarGridAcrescimoDescontoAutomatico();
}

function recarregarGridAcrescimoDescontoAutomatico() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_pessoa.AcrescimoDescontoAutomatico.val())) {

        $.each(_pessoa.AcrescimoDescontoAutomatico.val(), function (i, acrescimoDescontoAutomatico) {
            var acrescimoDescontoAutomaticoGrid = new Object();

            acrescimoDescontoAutomaticoGrid.Codigo = acrescimoDescontoAutomatico.Codigo;
            acrescimoDescontoAutomaticoGrid.Descricao = acrescimoDescontoAutomatico.Descricao;
            acrescimoDescontoAutomaticoGrid.Justificativa = acrescimoDescontoAutomatico.Justificativa.Descricao;
            acrescimoDescontoAutomaticoGrid.Valor = acrescimoDescontoAutomatico.Valor;
            acrescimoDescontoAutomaticoGrid.TipoValor = acrescimoDescontoAutomatico.TipoValor;
            acrescimoDescontoAutomaticoGrid.TipoCalculo = acrescimoDescontoAutomatico.TipoCalculo;
            acrescimoDescontoAutomaticoGrid.Observacoes = acrescimoDescontoAutomatico.Observacoes;

            data.push(acrescimoDescontoAutomaticoGrid);
        });
    }
    _pessoa.AcrescimoDescontoAutomatico.val(RetornarObjetoPesquisa(_acrescimoDescontoAutomatico));
    _gridAcrescimoDescontoAutomatico.CarregarGrid(data);
}


function excluirAcrescimoDescontoAutomaticoClick(knoutAcrescimoDescontoAutomatico, data) {
    var acrescimoDescontoAutomaticoGrid = knoutAcrescimoDescontoAutomatico.basicTable.BuscarRegistros();

    for (var i = 0; i < acrescimoDescontoAutomaticoGrid.length; i++) {
        if (data.Codigo == acrescimoDescontoAutomaticoGrid[i].Codigo) {
            acrescimoDescontoAutomaticoGrid.splice(i, 1);
            break;
        }
    }

    knoutAcrescimoDescontoAutomatico.basicTable.CarregarGrid(acrescimoDescontoAutomaticoGrid);
}

function limparCamposAcrescimoDescontoAutomatico() {
    LimparCampos(_acrescimoDescontoAutomatico);
}