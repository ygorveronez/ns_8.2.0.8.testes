/// <reference path="../../Consultas/TipoOperacao.js" />
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

/// <reference path="SimuladorFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridLocalidade;
var _localidade;

var Localidade = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Localidade = PropertyEntity({ type: types.event, text: "Adicionar Localidade", idBtnSearch: guid() });
    this.Estado = PropertyEntity({ col: 3, val: ko.observable(EnumEstado.Acre), def: EnumEstado.Acre, options: EnumEstado.obterOpcoesPesquisaSemExterior(), text: Localization.Resources.Consultas.Localidade.UF.getFieldDescription(), visible: ko.observable(true) });
}


//*******EVENTOS*******

function loadLocalidade() {

    _localidade = new Localidade();
    KoBindings(_localidade, "knockoutLocalidade");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirLocalidadeClick(_localidade.Localidade, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: "Descrição", width: "80%" },
    { data: "Latitude", visible: false },
    { data: "Longitude", visible: false }];

    _gridLocalidade = new BasicDataTable(_localidade.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarLocalidades(_localidade.Localidade, null, null, null, _gridLocalidade);
    _localidade.Localidade.basicTable = _gridLocalidade;

    recarregarGridLocalidade();
}

function recarregarGridLocalidade() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_simuladorFrete.Localidades.val())) {

        $.each(_simuladorFrete.Localidades.val(), function (i, localidade) {

            var localidadeGrid = new Object();

            localidadeGrid.Codigo = localidade.Localidade.Codigo;
            localidadeGrid.Descricao = localidade.Localidade.Descricao;
            localidadeGrid.Latitude = localidade.Localidade.Latitude;
            localidadeGrid.Longitude = localidade.Localidade.Longitude;
            data.push(localidadeGrid);

        }
        );
    }
    _gridLocalidade.CarregarGrid(data);
}


function excluirLocalidadeClick(knoutLocalidade, data) {
    var localidadeGrid = knoutLocalidade.basicTable.BuscarRegistros();
    for (var i = 0; i < localidadeGrid.length; i++) {
        if (data.Codigo == localidadeGrid[i].Codigo) {
            localidadeGrid.splice(i, 1);
            break;
        }
    }
    knoutLocalidade.basicTable.CarregarGrid(localidadeGrid);
}

function limparCamposLocalidade() {
    LimparCampos(_localidade);
    _gridLocalidade.CarregarGrid([]);
}