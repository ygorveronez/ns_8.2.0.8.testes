/// <reference path="TipoMovimento.js" />
/// <reference path="../../Consultas/TipoDespesaFinanceira.js" />

var _tipoDespesa;
var _gridTiposDespesa;

var TipoDespesa = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.TipoDespesa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Adicionar Tipo de Despesa", idBtnSearch: guid(), visible: ko.observable(true) });
};

function loadTipoDespesa() {
    _tipoDespesa = new TipoDespesa();
    KoBindings(_tipoDespesa, "tabTipoDespesa");

    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: function (data) { excluirTiposDespesa(_gridTiposDespesa, data) }, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir], tamanho: 5 };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" },
    ];
    _gridTiposDespesa = new BasicDataTable(_tipoDespesa.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridTiposDespesa.CarregarGrid([]);
    _tipoDespesa.TipoDespesa.BasicDataTable = _gridTiposDespesa;

    new BuscarTipoDespesaFinanceira(_tipoDespesa.TipoDespesa, null, _gridTiposDespesa);

    if (_CONFIGURACAO_TMS.AtivarControleDespesas)
        $("#liTabTipoDespesa").show();
}

function obterTiposDespesaSalvar() {
    var listaTiposDespesa = _tipoDespesa.TipoDespesa.BasicDataTable.BuscarRegistros();
    var listaTiposDespesaRetornar = new Array();

    for (var i = 0; i < listaTiposDespesa.length; i++) {
        listaTiposDespesaRetornar.push({
            Codigo: listaTiposDespesa[i].Codigo
        });
    }

    return JSON.stringify(listaTiposDespesaRetornar);
}

function excluirTiposDespesa(knout, data) {
    var dados = knout.BuscarRegistros();

    for (var i = 0; i < dados.length; i++) {
        if (data.Codigo == dados[i].Codigo) {
            dados.splice(i, 1);
            break;
        }
    }

    knout.CarregarGrid(dados);
}

function preencherListaTiposDespesa(data) {
    _gridTiposDespesa.CarregarGrid(data.TiposDespesa);
}

function limparListaTiposDespesa() {
    _gridTiposDespesa.CarregarGrid([]);
}