/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumOrigemOcorrencia.js" />
/// <reference path="ContratoFreteTransportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _tiposOcorrencia;
var _gridTiposOcorrencia;

var TiposOcorrencia = function () {
    this.ContratoFreteTransportador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TiposOcorrencia = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, idGrid: guid() });
    this.TiposOcorrencia.val.subscribe(function () {
        _contratoFreteTransportador.TiposOcorrencia.val(JSON.stringify(_tiposOcorrencia.TiposOcorrencia.val()));
        RenderizarGridTiposOcorrencia();
        if (_tiposOcorrencia.TiposOcorrencia.val().length > 0)
            $(".li-controle").show();
        else
            $(".li-controle").hide();
        GerenciarAbasContratoFrete();
    });

    this.Adicionar = PropertyEntity({ type: types.event, text: "Adicionar", visible: ko.observable(true), idBtnSearch: guid() });
}

//*******EVENTOS*******

function LoadTiposOcorrencia() {
    _tiposOcorrencia = new TiposOcorrencia();
    KoBindings(_tiposOcorrencia, "knockoutTiposOcorrencia");
    
    LoadGridTiposOcorrencia();

    // Buscas
    new BuscarTipoOcorrencia(_tiposOcorrencia.Adicionar, AdicionarTipoOcorrenciaAGrid, null, null, null, false, _gridTiposOcorrencia);
}

function AdicionarTipoOcorrenciaAGrid(data) {
    // Pega registros
    var dataGrid = _tiposOcorrencia.TiposOcorrencia.val();

    data.forEach(function (result) {
        var item = {
            Codigo: result.Codigo,
            CodigoProceda: result.CodigoProceda,
            Descricao: result.Descricao,
        };

        dataGrid.push(item);
    });

    _tiposOcorrencia.TiposOcorrencia.val(dataGrid);
}

function RemoverTipoOcorrenciaClick(data) {
    var dataGrid = _tiposOcorrencia.TiposOcorrencia.val();

    for (var i = 0; i < dataGrid.length; i++) {
        if (data.Codigo == dataGrid[i].Codigo) {
            dataGrid.splice(i, 1);
            break;
        }
    }

    _tiposOcorrencia.TiposOcorrencia.val(dataGrid);
}

//*******MÉTODOS*******

function EditarTiposOcorrencia(data) {
    _tiposOcorrencia.ContratoFreteTransportador.val(_contratoFreteTransportador.Codigo.val());

    _tiposOcorrencia.TiposOcorrencia.val(data.TiposOcorrencia);
    RenderizarGridTiposOcorrencia();
}

function LimparCamposTiposOcorrencia() {
    LimparCampos(_tiposOcorrencia);
    _tiposOcorrencia.TiposOcorrencia.val([]);
    RenderizarGridTiposOcorrencia();
}

function LoadGridTiposOcorrencia() {
    //-- Grid
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: "Excluir",
                id: guid(),
                evento: "onclick",
                tamanho: "10",
                icone: "",
                metodo: function (data) {
                    if (_CAMPOS_BLOQUEADOS) return;
                    RemoverTipoOcorrenciaClick(data);
                }
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Ocorrência", width: "50%", className: "text-align-left" },
        { data: "CodigoProceda", title: "Código Proceda", width: "30%", className: "text-align-center" },
    ];

    // Grid
    _gridTiposOcorrencia = new BasicDataTable(_tiposOcorrencia.TiposOcorrencia.idGrid, header, menuOpcoes, null, null, 10);
    _gridTiposOcorrencia.CarregarGrid([]);
}

function RenderizarGridTiposOcorrencia() {
    var TipoOcorrencias = _tiposOcorrencia.TiposOcorrencia.val();

    _gridTiposOcorrencia.CarregarGrid(TipoOcorrencias);
}