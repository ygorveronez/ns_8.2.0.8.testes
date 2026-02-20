/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="ValorParametroOcorrencia.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _pernoite;
var _pernoiteValor;
var _gridPernoiteValores;

var Pernoite = function () {
    this.TipoOcorrencia = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Ocorrência:", idBtnSearch: guid(), visible: ko.observable(true), required: function () {
            return HandleRequiredTipoOcorrencia(_gridPernoiteValores);
        } });
    this.ComponenteFrete = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: "*Componente Frete:", idBtnSearch: guid(), visible: ko.observable(true), required: function () {
            return HandleRequiredTipoOcorrencia(_gridPernoiteValores);
        } });
}

var PernoiteValor = function () {
    this.Valores = PropertyEntity({ idGrid: guid(), type: types.local });

    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0) });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo Veicular:", idBtnSearch: guid(), visible: ko.observable(true), required: true });
    this.Valor = PropertyEntity({ text: "Valor:", getType: typesKnockout.decimal });

    this.Adicionar = PropertyEntity({ text: "Adicionar", getType: typesKnockout.event, eventClick: AdicionarPernoite, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ text: "Atualizar", getType: typesKnockout.event, eventClick: AtualizarPernoite, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ text: "Excluir", getType: typesKnockout.event, eventClick: ExcluirCamposCRUDPernoite, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ text: "Cancelar", getType: typesKnockout.event, eventClick: LimparCamposCRUDPernoite, visible: ko.observable(true) });

    this.Data = PropertyEntity({
        Get: function () { return _dados.PernoiteValores.list.slice(); },
        Set: function (data) { _dados.PernoiteValores.list = data.slice(); },
        Grid: function () { return _gridPernoiteValores; }
    });
}

//*******EVENTOS*******
function LoadPernoite() {
    _pernoite = new Pernoite();
    KoBindings(_pernoite, "knockoutPernoite");

    _pernoiteValor = new PernoiteValor();
    KoBindings(_pernoiteValor, "knockoutPernoiteValor");

    new BuscarModelosVeicularesCarga(_pernoiteValor.ModeloVeicular);
    new BuscarComponentesDeFrete(_pernoite.ComponenteFrete);
    new BuscarTipoOcorrencia(_pernoite.TipoOcorrencia);

    CarregarGridPernoiteValores();

    _dados.PernoiteTipoOcorrencia = _pernoite.TipoOcorrencia;
    _dados.PernoiteComponenteFrete = _pernoite.ComponenteFrete;
}

function AdicionarPernoite(ko) {
    var objeto = SalvarObjetoPernoite(ko, true);

    var dados = ko.Data.Get();

    if (!ValidarCamposObrigatorios(ko))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Preencha os campos obrigatórios.");

    if (!ValidarDuplicidadeModeloVeicular(ko, objeto))
        return exibirMensagem(tipoMensagem.atencao, "Duplicidade", "Já existe um cadastro para esse modelo veicular.");

    dados.push(objeto);
    ko.Data.Set(dados);

    LimparCamposCRUDPernoite(ko);
    RenderizaGridPernoite(ko);
}

function AtualizarPernoite(ko) {
    var objeto = SalvarObjetoPernoite(ko, false);

    var dados = ko.Data.Get();

    if (!ValidarCamposObrigatorios(ko))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Preencha os campos obrigatórios.");

    if (!ValidarDuplicidadeModeloVeicular(ko, objeto))
        return exibirMensagem(tipoMensagem.atencao, "Duplicidade", "Já existe um cadastro para esse modelo veicular.");

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo.val == objeto.Codigo.val) {
            dados[i] = objeto;
            break;
        }
    }

    ko.Data.Set(dados);

    LimparCamposCRUDPernoite(ko);
    RenderizaGridPernoite(ko);
}

function ExcluirCamposCRUDPernoite(ko) {
    var dados = ko.Data.Get();

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo.val == ko.Codigo.val()) {
            dados.splice(i, 1);
            break;
        }
    }

    ko.Data.Set(dados);

    LimparCamposCRUDPernoite(ko);
    RenderizaGridPernoite(ko);
}


//*******METODOS*******
function CarregarGridPernoiteValores() {
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
                    EditarPernoite(_pernoiteValor, data);
                }
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicular", title: "Modelo", width: "60%", className: "text-align-left" },
        { data: "Valor", title: "Valor", width: "30%", className: "text-align-right" },
    ];

    // Grid
    _gridPernoiteValores = new BasicDataTable(_pernoiteValor.Valores.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.desc }, null, 10);
    _gridPernoiteValores.CarregarGrid([]);
}

function LimparCamposPernoite() {
    LimparCampos(_pernoite);
    LimparCampos(_pernoiteValor);

    RenderizaGridEstadia(_pernoiteValor);
}

function LimparCamposCRUDPernoite(ko) {
    LimparCampoEntity(ko.ModeloVeicular);
    ko.Codigo.val(ko.Codigo.def);
    ko.Valor.val(ko.Valor.def);
    LimparCamposCRUDAba(ko);
}

function EditarPernoite(ko, data) {
    var obj = null;
    ko.Data.Get().forEach(function (item) {
        if (item.Codigo.val == data.Codigo)
            obj = item;
    });

    if (obj != null) {
        PreencherEditarListEntity(ko, obj);

        ko.Adicionar.visible(false);
        ko.Atualizar.visible(true);
        ko.Excluir.visible(true);
    }
}

function RenderizaGridPernoite(ko) {
    var data = ko.Data.Get().map(function (item) {
        var obj = {
            Codigo: item.Codigo.val,
            ModeloVeicular: item.ModeloVeicular.val,
            Valor: item.Valor.val
        };
        
        return obj;
    });

    ko.Data.Grid().CarregarGrid(data);
}

function SalvarObjetoPernoite(ko, novo) {
    var objeto = SalvarListEntity(ko);

    if (novo)
        objeto.Codigo.val = guid();

    return objeto;
}