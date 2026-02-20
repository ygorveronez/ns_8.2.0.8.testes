/// <reference path="../../Consultas/CentroResultado.js" />
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

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCentroCusto, _centroCusto;

var CentroCusto = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de Custo:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Percentual = PropertyEntity({ text: "*Percentual:", getType: typesKnockout.decimal, maxlength: 6, required: true, enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarCentroCustoClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
};

var CentroCustoMap = function () {
    this.Codigo = PropertyEntity({ val: 0, def: 0 });
    this.CodigoCentroResultado = PropertyEntity({ val: 0, def: 0 });
    this.CentroResultado = PropertyEntity({ type: types.map, val: "", def: "" });
    this.Percentual = PropertyEntity({ type: types.map, val: "", def: "", getType: typesKnockout.decimal });
};

//*******EVENTOS*******

function LoadCentroCusto() {

    _centroCusto = new CentroCusto();
    KoBindings(_centroCusto, "knockoutCentrosCustos");

    new BuscarCentroResultado(_centroCusto.CentroResultado);

    var _editableConfig = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.decimal,
        numberMask: ConfigDecimal()
    };

    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            RemoverCentroCustoClick(_documentoEntrada.CentroCustos, data);
        }, tamanho: "10", icone: ""
    };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoCentroResultado", visible: false },
        { data: "CentroResultado", title: "Centro de Custo", width: "60%" },
        { data: "Percentual", title: "Percentual", width: "10%", editableCell: _editableConfig }
    ];

    var editarResposta = {
        permite: true,
        callback: SalvarRetornoGrid,
        atualizarRow: true
    };

    _gridCentroCusto = new BasicDataTable(_centroCusto.Grid.id, header, menuOpcoes, { column: 2, dir: orderDir.asc }, null, null, null, null, editarResposta);

    RecarregarGridCentroCusto();
}

function SalvarRetornoGrid(dataRow) {
    var data = GetCentrosCustos();

    for (var i in data) {
        if (data[i].Codigo.val === dataRow.Codigo) {
            data[i].CentroResultado.val = dataRow.CentroResultado;
            data[i].CodigoCentroResultado.val = dataRow.CodigoCentroResultado;
            data[i].Percentual.val = dataRow.Percentual;
            break;
        }
    }

    SetCentrosCustos(data);
}

function GetCentrosCustos() {
    return _documentoEntrada.CentroCustos.list.slice();
}

function SetCentrosCustos(data) {
    return _documentoEntrada.CentroCustos.list = data.slice();
}

function GetCentrosCustosToJson() {
    var data = GetCentrosCustosToObj();
    return JSON.stringify(data);
}

function GetCentrosCustosToObj() {
    var data = [];

    $.each(GetCentrosCustos(), function (i, centro) {
        var itemGrid = {};

        itemGrid.DT_Enable = true;
        itemGrid.Codigo = centro.Codigo.val;
        itemGrid.CentroResultado = centro.CentroResultado.val;
        itemGrid.CodigoCentroResultado = centro.CodigoCentroResultado.val;
        itemGrid.Percentual = centro.Percentual.val;

        data.push(itemGrid);
    });

    return data;
}

function RecarregarGridCentroCusto() {

    var data = GetCentrosCustosToObj();
    _gridCentroCusto.CarregarGrid(data);
}

function AdicionarCentroCustoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_centroCusto);

    if (valido) {
        if (Globalize.parseFloat(_centroCusto.Percentual.val()) > 100) {
            exibirMensagem(tipoMensagem.atencao, "Atenção", "Informe um percentual menor que 100%");
            return;
        }

        executarReST("CentroResultado/BuscarCentroResultadoAnalitico", { Codigo: _centroCusto.CentroResultado.codEntity(), Percentual: _centroCusto.Percentual.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    var retorno = arg.Data.Centros;
                    for (var j = 0; j < retorno.length; j++) {
                        var dado = retorno[j];

                        _centroCusto.Codigo.val(guid());

                        var itemGrid = new CentroCustoMap();
                        itemGrid.Codigo.val = guid();
                        itemGrid.CentroResultado.val = dado.CentroResultado;
                        itemGrid.CodigoCentroResultado.val = dado.CodigoCentroResultado;
                        itemGrid.Percentual.val = dado.Percentual.val;

                        _documentoEntrada.CentroCustos.list.push(itemGrid);
                    }

                    RecarregarGridCentroCusto();
                    LimparCamposCentroCusto();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function RemoverCentroCustoClick(e, sender) {
    if (_documentoEntrada.Situacao.val() !== EnumSituacaoDocumentoEntrada.Aberto) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este documento não aberto para a edição.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o centro de custo " + sender.CentroResultado + "?", function () {
        var data = GetCentrosCustos();

        for (var i in data) {
            if (data[i].Codigo.val === sender.Codigo) {
                data.splice(i, 1);
                break;
            }
        }

        SetCentrosCustos(data);
        RecarregarGridCentroCusto();
    });
}

function LimparCamposCentroCusto() {
    _centroCusto.Adicionar.visible(true);
    LimparCampos(_centroCusto);
}