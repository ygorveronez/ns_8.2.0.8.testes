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
/// <reference path="../../Consultas/Tranportador.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _alteracaoMassa;
var _pesquisaAlteracaoMassa;
var _gridTransportador;

var AlteracaoMassa = function () {
    this.DataEnvio = PropertyEntity({ val: ko.observable(""), text: "Data Envio: " });

    this.AdicionarTransportador = PropertyEntity({ type: types.event, text: "Adicionar", visible: ko.observable(true), idBtnSearch: guid() });
    this.Transportadores = PropertyEntity({ getType: typesKnockout.basicTable, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });
    this.Transportadores.val.subscribe(function () {
        _gridTransportador.CarregarGrid(_alteracaoMassa.Transportadores.val());
    });

    this.LiberacaoParaPagamentoAutomatico = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Integração Automática CTE - GOLD" });

    // CRUD
    this.Enviar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Enviar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.ImportarPlanilha = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn btn-primary botaoDentroSmartAdmimForm",
        UrlImportacao: "AlteracaoMassa/ImportarParaProcessar",
        UrlConfiguracao: "AlteracaoMassa/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O023_AlteracaoMassaTransportadores,
        CallbackImportacao: function (arg) {
            if (arg.Data.Retorno != null)
                _alteracaoMassa.Transportadores.val(arg.Data.Retorno);
        },
        FecharModalSeSucesso: true
    });
}


//*******EVENTOS*******
function loadAlteracaoMassa() {
    //-- Knouckout
    // Instancia objeto principal
    _alteracaoMassa = new AlteracaoMassa();
    KoBindings(_alteracaoMassa, "knockoutAlteracaoMassa");

    LoadGridTransportadores();

    // Instancia buscas
    new BuscarTransportadores(_alteracaoMassa.AdicionarTransportador, null, null, null, _gridTransportador);
}

function LoadGridTransportadores() {
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
                metodo: RemoverTransportadorClick
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "RazaoSocial", title: "Razão Social", width: "32%", className: "text-align-left" },
        { data: "CNPJ", title: "CNPJ/CPF", width: "16%", className: "text-align-left" },
        { data: "Telefone", title: "Telefone", width: "11%", className: "text-align-left" },
        { data: "Localidade", title: "Localidade", width: "18%", className: "text-align-left" },
        { data: "LiberacaoParaPagamentoAutomatico", title: "Integração Automática CTE - GOLD", width: "11%", className: "text-align-center" }
    ];

    // Grid
    _gridTransportador = new BasicDataTable(_alteracaoMassa.Transportadores.idGrid, header, menuOpcoes, null, null, 10);
    _gridTransportador.CarregarGrid([]);
    _alteracaoMassa.Transportadores.basicTable = _gridTransportador;
}

function adicionarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Tem certeza que deseja processar os transportadores selecionadas?", function () {
        Salvar(_alteracaoMassa, "AlteracaoMassa/Adicionar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                    LimparCamposAlteracaoMassa();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function RemoverTransportadorClick(data) {
    var dataGrid = _gridTransportador.BuscarRegistros();

    for (var i = 0; i < dataGrid.length; i++) {
        if (data.Codigo == dataGrid[i].Codigo) {
            dataGrid.splice(i, 1);
            break;
        }
    }

    _alteracaoMassa.Transportadores.val(dataGrid);
}

function cancelarClick(e) {
    LimparCamposAlteracaoMassa();
}



//*******MÉTODOS*******
function LimparCamposAlteracaoMassa() {
    LimparCampos(_alteracaoMassa);
}