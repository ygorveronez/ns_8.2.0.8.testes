/// <reference path="TipoMovimento.js" />

var _gridCentroResultado;
var _centroResultado;
var _centroResultadoTipoMovimento;

var CentroResultadoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ type: types.map, val: "" });
};

var CentroResultadoTipoMovimento = function () {
    this.CentroResultado = PropertyEntity({ type: types.listEntity, list: new Array(), text: "*Centro de Resultado:", codEntity: ko.observable(0), idBtnSearch: guid(), idGrid: guid() });
    this.AdicionarCentroResultado = PropertyEntity({ eventClick: adicionarCentroResultadoClick, type: types.event, text: "Adicionar Centro de Resultado", visible: ko.observable(true) });

}

function loadCentroResultado() {
    _centroResultadoTipoMovimento = new CentroResultadoTipoMovimento();
    KoBindings(_centroResultadoTipoMovimento, "tabCentroResultado");

    _tipoMovimento.CentroResultado = _centroResultadoTipoMovimento.CentroResultado;
    _tipoMovimento.AdicionarCentroResultado = _centroResultadoTipoMovimento.AdicionarCentroResultado;

    new BuscarCentroResultado(_tipoMovimento.CentroResultado, "Selecione o Centro de Resultado", "Centros de Resultado", retornoCentroResultado, EnumAnaliticoSintetico.Analitico);

    var editar = { descricao: "Remover", id: guid(), evento: "onclick", metodo: excluirCentroResultadoClick, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "90%" }
    ];
    _gridCentroResultado = new BasicDataTable(_tipoMovimento.CentroResultado.idGrid, header, menuOpcoes);

    recarregarGridCentroResultado();
}

function retornoCentroResultado(data) {
    _centroResultado = new CentroResultadoMap();
    _centroResultado.Codigo.val = data.Codigo;
    _centroResultado.Descricao.val = data.Descricao;

    _tipoMovimento.CentroResultado.codEntity(data.Codigo);
    _tipoMovimento.CentroResultado.val(data.Descricao);
}

function recarregarGridCentroResultado() {
    var data = new Array();
    $.each(_tipoMovimento.CentroResultado.list, function (i, CentroResultado) {
        var obj = new Object();
        obj.Codigo = CentroResultado.Codigo.val;
        obj.Descricao = CentroResultado.Descricao.val;
        data.push(obj);
    });
    _gridCentroResultado.CarregarGrid(data);
}

function adicionarCentroResultadoClick(e, sender) {
    _tipoMovimento.CentroResultado.requiredClass("form-control");
    var tudoCerto = ValidarCampoObrigatorioEntity(_tipoMovimento.CentroResultado);
    tudoCerto = _tipoMovimento.CentroResultado.codEntity() > 0;
    if (tudoCerto) {
        var existe = false;
        $.each(_tipoMovimento.CentroResultado.list, function (i, CentroResultado) {
            if (CentroResultado.Codigo.val == _tipoMovimento.CentroResultado.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _tipoMovimento.CentroResultado.list.push(_centroResultado);
            recarregarGridCentroResultado();
            $("#" + _tipoMovimento.CentroResultado.id).focus();
            _tipoMovimento.CentroResultado.requiredClass("form-control");
        } else {
            exibirMensagem("aviso", "Centro de Resultado já informado", "O centro de resultado " + _tipoMovimento.CentroResultado.val() + " já foi informado para este tipo de movimento.");
        }
        LimparCampoEntity(_tipoMovimento.CentroResultado);
    } else {
        exibirMensagem("atencao", "Campos Obrigatórios", "Informe os campos obrigatórios");
        _tipoMovimento.CentroResultado.requiredClass("form-control is-invalid");
    }
}

function excluirCentroResultadoClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o centro de resultado " + data.Descricao + "?", function () {
        var listaAtualizada = new Array();
        $.each(_tipoMovimento.CentroResultado.list, function (i, CentroResultado) {
            if (CentroResultado.Codigo.val != data.Codigo) {
                listaAtualizada.push(CentroResultado);
            }
        });
        _tipoMovimento.CentroResultado.list = listaAtualizada;
        recarregarGridCentroResultado();
    });
}