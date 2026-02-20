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
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="RegraTipoOperacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegraTipoOperacaoRecebedores;
var _regraTipoOperacaoRecebedores;

var RegraTipoOperacaoRecebedores = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Recebedores = PropertyEntity({ type: types.event, text: "Adicionar Recebedor", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadRegraTipoOperacaoRecebedores() {
    _regraTipoOperacaoRecebedores = new RegraTipoOperacaoRecebedores();
    KoBindings(_regraTipoOperacaoRecebedores, "knockoutRegraTipoOperacaoRecebedor");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirRegraTipoOperacaoRecebedoresClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Recebedor", width: "70%" },
    ];

    _gridRegraTipoOperacaoRecebedores = new BasicDataTable(_regraTipoOperacaoRecebedores.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_regraTipoOperacaoRecebedores.Recebedores, null,null,null,null, _gridRegraTipoOperacaoRecebedores);

    RecarregarGridRegraTipoOperacaoRecebedores();
}

function RecarregarGridRegraTipoOperacaoRecebedores() {
    var data = new Array();
    if (_regraTipoOperacao.Recebedores.val() != "") {
        $.each(_regraTipoOperacao.Recebedores.val(), function (i, tipoCarga) {
            var tiposRegraTipoOperacaoRecebedorGrid = new Object();

            tiposRegraTipoOperacaoRecebedorGrid.Codigo = tipoCarga.Codigo;
            tiposRegraTipoOperacaoRecebedorGrid.Descricao = tipoCarga.Descricao;

            data.push(tiposRegraTipoOperacaoRecebedorGrid);
        });
    }
    _gridRegraTipoOperacaoRecebedores.CarregarGrid(data);
}

function ExcluirRegraTipoOperacaoRecebedoresClick(data) {
    var tiposRegraTipoOperacaoRecebedorGrid = _gridRegraTipoOperacaoRecebedores.BuscarRegistros();

    for (var i = 0; i < tiposRegraTipoOperacaoRecebedorGrid.length; i++) {
        if (data.Codigo == tiposRegraTipoOperacaoRecebedorGrid[i].Codigo) {
            tiposRegraTipoOperacaoRecebedorGrid.splice(i, 1);
            break;
        }
    }
    _gridRegraTipoOperacaoRecebedores.CarregarGrid(tiposRegraTipoOperacaoRecebedorGrid);
}

function LimparCamposRegraTipoOperacaoRecebedor() {
    LimparCampos(_regraTipoOperacaoRecebedores);
    _gridRegraTipoOperacaoRecebedores.CarregarGrid(new Array());
}