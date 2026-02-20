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
/// <reference path="../../Consultas/TipoContainer.js" />
/// <reference path="TipoContainer.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _tipoContainerAssociado, _gridTiposContainerAssociado;

var TipoContainerAssociado = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.TipoContainer = PropertyEntity({ type: types.event, text: "Adicionar Tipo(s) Container", idBtnSearch: guid() });
};

//*******EVENTOS*******

function loadTipoContainerAssociado() {
    _tipoContainerAssociado = new TipoContainerAssociado();
    KoBindings(_tipoContainerAssociado, "knockoutTipoContainerAssociado");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: function (data) { ExcluirTipoContainerAssociadoClick(_tipoContainerAssociado.TipoContainer, data); } }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "60%" },
        { data: "CodigoIntegracao", title: "Código de Integração", width: "30%" }
    ];
    _gridTiposContainerAssociado = new BasicDataTable(_tipoContainerAssociado.Grid.id, header, menuOpcoes);

    new BuscarTiposContainer(_tipoContainerAssociado.TipoContainer, function (r) {
        if (r != null) {
            var tipos = _gridTiposContainerAssociado.BuscarRegistros();
            for (var i = 0; i < r.length; i++)
                tipos.push({
                    Codigo: r[i].Codigo,
                    Descricao: r[i].Descricao,
                    CodigoIntegracao: r[i].CodigoIntegracao
                });

            _gridTiposContainerAssociado.CarregarGrid(tipos);
        }
    }, _gridTiposContainerAssociado);

    _tipoContainerAssociado.TipoContainer.basicTable = _gridTiposContainerAssociado;

    RecarregarGridTipoContainerAssociado();
}

//*******MÉTODOS*******

function RecarregarGridTipoContainerAssociado() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_tipoContainer.TipoContainersAssociado.val())) {
        $.each(_tipoContainer.TipoContainersAssociado.val(), function (i, tipoContainer) {
            var tipoContainerGrid = new Object();

            tipoContainerGrid.Codigo = tipoContainer.TIPOCONTAINER.Codigo;
            tipoContainerGrid.Descricao = tipoContainer.TIPOCONTAINER.Descricao;
            tipoContainerGrid.CodigoIntegracao = tipoContainer.TIPOCONTAINER.CodigoIntegracao;

            data.push(tipoContainerGrid);
        });
    }
        _gridTiposContainerAssociado.CarregarGrid(data);
}

function ExcluirTipoContainerAssociadoClick(knoutTipoContainerAssociado, data) {
    var tipoContainerGrid = knoutTipoContainerAssociado.basicTable.BuscarRegistros();

    for (var i = 0; i < tipoContainerGrid.length; i++) {
        if (data.Codigo === tipoContainerGrid[i].Codigo) {
            tipoContainerGrid.splice(i, 1);
            break;
        }
    }

    knoutTipoContainerAssociado.basicTable.CarregarGrid(tipoContainerGrid);
}

function preencherListasSelecaoTipoContainerAssociado() {
    var tipoContainers = new Array();
    
    $.each(_tipoContainerAssociado.TipoContainer.basicTable.BuscarRegistros(), function (i, tipoContainer) {
        tipoContainers.push({ TIPOCONTAINER: tipoContainer });
    });

    _tipoContainer.TipoContainersAssociado.val(JSON.stringify(tipoContainers));
}

function limparCamposTipoContainerAssociado() {
    LimparCampos(_tipoContainerAssociado);
    RecarregarGridTipoContainerAssociado();
}