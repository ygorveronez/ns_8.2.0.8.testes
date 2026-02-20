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
/// <reference path="Regiao.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridLocalidades;
var _localidade;

var Localidade = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Localidade = PropertyEntity({ type: types.event, text: "Adicionar Localidade", idBtnSearch: guid() });
    this.Estado = PropertyEntity({ col: 3, val: ko.observable(EnumEstado.Acre), def: EnumEstado.Acre, options: EnumEstado.obterOpcoes(), text: "Adicionar localidade por estado:" });

    this.Adicionar = PropertyEntity({ eventClick: adicionarCidadePorEstado, type: types.event, text: "Adicionar", visible: ko.observable(true) });

};

//*******EVENTOS*******

function loadLocalidade() {
    _localidade = new Localidade();
    KoBindings(_localidade, "knockoutLocalidade");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirLocalidadeClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", title: "Código", width: "10%", className: "text-align-rigth" },
        { data: "Descricao", title: "Descrição", width: "65%", className: "text-align-left" }
    ];

    _gridLocalidades = new BasicDataTable(_localidade.Grid.id, header, menuOpcoes);

    new BuscarLocalidades(_localidade.Localidade, "Busca Localidade", "Localidades", null, _gridLocalidades);

    _localidade.Localidade.basicTable = _gridLocalidades;

    recarregarGridLocalidades();
}

//*******MÉTODOS*******

function recarregarGridLocalidades() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_regiao.Localidades.val())) {

        $.each(_regiao.Localidades.val(), function (i, Localidade) {
            var localidadeGrid = new Object();

            localidadeGrid.Codigo = Localidade.Codigo;
            localidadeGrid.Descricao = Localidade.Descricao;

            data.push(localidadeGrid);
        });

    }
    _gridLocalidades.CarregarGrid(data);
}

function excluirLocalidadeClick(data) {
    var localidadeGrid = _localidade.Localidade.basicTable.BuscarRegistros();

    for (var i = 0; i < localidadeGrid.length; i++) {
        if (data.Codigo == localidadeGrid[i].Codigo) {
            localidadeGrid.splice(i, 1);
            break;
        }
    }

    _localidade.Localidade.basicTable.CarregarGrid(localidadeGrid);
}

function limparCamposLocalidades() {
    LimparCampos(_localidade);
    _localidade.Localidade.basicTable.CarregarGrid(new Array());
}
function adicionarCidadePorEstado(e) {
    executarReST("/Regiao/ObterCidades", { Estado: e.Estado.val() }, (arg) => {

        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);;

        _gridLocalidades.CarregarGrid(arg.Data);
        exibirMensagem(tipoMensagem.ok, "Sucesso", "Localidades adicionadas com sucesso")

    })

}