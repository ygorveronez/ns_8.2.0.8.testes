/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="TabelaProdutividade.js" />
/// <reference path="../../Consultas/Justificativa.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _valor;
var _gridValor;

var Valor = function () {
    this.Valores = PropertyEntity({ idGrid: guid(), type: types.local });

    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0) });
    this.ValorInicial = PropertyEntity({ text: "*Valor Inicial:", getType: typesKnockout.decimal, required: true });
    this.ValorFinal = PropertyEntity({ text: "*Valor Final:", getType: typesKnockout.decimal, required: true });
    this.Valor = PropertyEntity({ text: "*Valor Bonificação:", getType: typesKnockout.decimal, required: true });

    this.Adicionar = PropertyEntity({ text: "Adicionar", getType: typesKnockout.event, eventClick: AdicionarValor, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ text: "Atualizar", getType: typesKnockout.event, eventClick: AtualizarValor, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ text: "Excluir", getType: typesKnockout.event, eventClick: ExcluirCamposCRUDValor, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ text: "Cancelar", getType: typesKnockout.event, eventClick: LimparCamposCRUDValor, visible: ko.observable(true) });

    this.Data = PropertyEntity({
        Get: function () { return _tabelaProdutividade.Valores.list.slice(); },
        Set: function (data) { _tabelaProdutividade.Valores.list = data.slice(); },
        Grid: function () { return _gridValor }
    });
}

//*******EVENTOS*******
function LoadValor() {
    _valor = new Valor();
    KoBindings(_valor, "knockoutValores");

    CarregarGridValores();
}

function AdicionarValor(ko) {
    var objeto = SalvarObjetoValor(ko, true);

    var dados = ko.Data.Get();

    if (!ValidarCamposObrigatorios(ko))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Preencha os campos obrigatórios.");

    dados.push(objeto);
    ko.Data.Set(dados);

    LimparCamposCRUDValor(ko);
    RenderizaGridValor(ko);
}

function AtualizarValor(ko) {
    var objeto = SalvarObjetoValor(ko, false);

    var dados = ko.Data.Get();

    if (!ValidarCamposObrigatorios(ko))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Preencha os campos obrigatórios.");

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo.val === objeto.Codigo.val) {
            dados[i] = objeto;
            break;
        }
    }

    ko.Data.Set(dados);

    LimparCamposCRUDValor(ko);
    RenderizaGridValor(ko);
}

function ExcluirCamposCRUDValor(ko) {
    var dados = ko.Data.Get();

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo.val === ko.Codigo.val()) {
            dados.splice(i, 1);
            break;
        }
    }

    ko.Data.Set(dados);

    LimparCamposCRUDValor(ko);
    RenderizaGridValor(ko);
}

//*******METODOS*******
function CarregarGridValores() {
    //-- Grid
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: "Editar",
                id: guid(),
                evento: "onclick",
                tamanho: "10",
                icone: "",
                metodo: function (data) {
                    EditarValor(_valor, data);
                }
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "ValorInicial", title: "Valor Inicial", width: "30%", className: "text-align-right" },
        { data: "ValorFinal", title: "Valor Final", width: "30%", className: "text-align-right" },
        { data: "Valor", title: "Valor Bonificação", width: "30%", className: "text-align-right" }
    ];

    // Grid
    _gridValor = new BasicDataTable(_valor.Valores.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.desc }, null, 10);
    _gridValor.CarregarGrid([]);
}


function LimparCamposValor() {
    LimparCampos(_valor);

    RenderizaGridValor(_valor);
}

function LimparCamposCRUDValor(ko) {

    ko.Codigo.val(ko.Codigo.def);    
    ko.ValorInicial.val(ko.ValorInicial.def);
    ko.ValorFinal.val(ko.ValorFinal.def);
    ko.Valor.val(ko.Valor.def);
    LimparCamposCRUDAba(ko);
}

function EditarValor(ko, data) {
    var obj = null;
    ko.Data.Get().forEach(function (item) {
        if (item.Codigo.val === data.Codigo)
            obj = item;
    });

    if (obj !== null) {
        PreencherEditarListEntity(ko, obj);

        ko.Adicionar.visible(false);
        ko.Atualizar.visible(true);
        ko.Excluir.visible(true);
    }
}

function RenderizaGridValor(ko) {
    var data = ko.Data.Get().map(function (item) {
        var obj = {
            Codigo: item.Codigo.val,
            ValorInicial: item.ValorInicial.val,
            ValorFinal: item.ValorFinal.val,
            Valor: item.Valor.val
        };

        return obj;
    });

    ko.Data.Grid().CarregarGrid(data);
}

function SalvarObjetoValor(ko, novo) {
    var objeto = SalvarListEntity(ko);

    if (novo) {
        objeto.Codigo.val = guid();
    }

    return objeto;
}

function HandleRequiredComponente(ko) {
    return function () {
        return ko.Data.Grid().BuscarRegistros().length > 0;
    };
}

function LimparCamposCRUDAba(ko) {
    ko.Adicionar.visible(true);
    ko.Atualizar.visible(false);
    ko.Excluir.visible(false);
    ko.Cancelar.visible(true);
}