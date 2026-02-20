//*******MAPEAMENTO KNOUCKOUT*******

var _gridCentroResultado, _centroResultado;

var CentroResultado = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de Resultado:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Percentual = PropertyEntity({ text: "*Percentual:", getType: typesKnockout.decimal, maxlength: 6, required: true, enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarCentroResultadoClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
};

var CentroResultadoMap = function () {
    this.Codigo = PropertyEntity({ val: 0, def: 0 });
    this.CodigoCentroResultado = PropertyEntity({ val: 0, def: 0 });
    this.CentroResultado = PropertyEntity({ type: types.map, val: "", def: "" });
    this.Percentual = PropertyEntity({ type: types.map, val: "", def: "", getType: typesKnockout.decimal });
};

//*******EVENTOS*******

function LoadCentroResultado() {

    _centroResultado = new CentroResultado();
    KoBindings(_centroResultado, "tabCentrosResultado");

    new BuscarCentroResultado(_centroResultado.CentroResultado);

    var _editableConfig = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.decimal,
        numberMask: ConfigDecimal()
    };

    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            RemoverCentroResultadoClick(_tituloFinanceiro.CentrosResultado, data);
        }, tamanho: "10", icone: ""
    };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    var header = [{ data: "Codigo", visible: false },
    { data: "CodigoCentroResultado", visible: false },
    { data: "CentroResultado", title: "Centro de Resultado", width: "60%" },
    { data: "Percentual", title: "Percentual", width: "10%", editableCell: _editableConfig }];

    var editarResposta = {
        permite: true,
        callback: SalvarRetornoGrid,
        atualizarRow: true
    };

    _gridCentroResultado = new BasicDataTable(_centroResultado.Grid.id, header, menuOpcoes, { column: 2, dir: orderDir.asc }, null, null, null, null, editarResposta);

    RecarregarGridCentrosResultado();
}

function SalvarRetornoGrid(dataRow) {
    var data = GetCentrosResultado();

    for (var i in data) {
        if (data[i].Codigo.val === dataRow.Codigo) {
            data[i].CentroResultado.val = dataRow.CentroResultado;
            data[i].CodigoCentroResultado.val = dataRow.CodigoCentroResultado;
            data[i].Percentual.val = dataRow.Percentual;
            break;
        }
    }

    SetCentrosResultado(data);
}

function GetCentrosResultado() {
    return _tituloFinanceiro.CentrosResultado.list.slice();
}

function SetCentrosResultado(data) {
    return _tituloFinanceiro.CentrosResultado.list = data.slice();
}

function GetCentrosResultadoToJson() {
    var data = GetCentrosResultadoToObj();
    return JSON.stringify(data);
}

function GetCentrosResultadoToObj() {
    var data = [];

    $.each(GetCentrosResultado(), function (i, centro) {
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

function RecarregarGridCentrosResultado() {

    var data = GetCentrosResultadoToObj();
    _gridCentroResultado.CarregarGrid(data);
}

function AdicionarCentroResultadoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_centroResultado);

    if (valido) {
        if (Globalize.parseFloat(_centroResultado.Percentual.val()) > 100) {
            exibirMensagem(tipoMensagem.atencao, "Atenção", "Informe um percentual menor ou igual a 100%.");
            return;
        }

        executarReST("CentroResultado/BuscarCentroResultadoAnalitico", { Codigo: _centroResultado.CentroResultado.codEntity(), Percentual: _centroResultado.Percentual.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    var retorno = arg.Data.Centros;
                    for (var j = 0; j < retorno.length; j++) {
                        var dado = retorno[j];

                        _centroResultado.Codigo.val(guid());

                        var itemGrid = new CentroResultadoMap();
                        itemGrid.Codigo.val = guid();
                        itemGrid.CentroResultado.val = dado.CentroResultado;
                        itemGrid.CodigoCentroResultado.val = dado.CodigoCentroResultado;
                        itemGrid.Percentual.val = dado.Percentual.val;

                        _tituloFinanceiro.CentrosResultado.list.push(itemGrid);
                    }

                    RecarregarGridCentrosResultado();
                    LimparCamposCentroResultado();
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

function RemoverCentroResultadoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir o centro de resultado " + sender.CentroResultado + "?", function () {
        var data = GetCentrosResultado();

        for (var i in data) {
            if (data[i].Codigo.val === sender.Codigo) {
                data.splice(i, 1);
                break;
            }
        }

        SetCentrosResultado(data);
        RecarregarGridCentrosResultado();
    });
}

function LimparCamposCentroResultado() {
    LimparCampos(_centroResultado);
}