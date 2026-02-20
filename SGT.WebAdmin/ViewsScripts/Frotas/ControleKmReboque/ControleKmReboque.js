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
/// <reference path="../../Enumeradores/TipoMovimentoKmReboque.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridHistoricoVinculoKmReboque;
var _historicoVinculoKmReboque;
var _historicoVinculoKmReboqueBotoes;
var _pesquisaHistoricoVinculoKmReboque;

var _TipoMovimento = [
    { text: "Todos", value: 0 },
    { text: "Vínculo", value: EnumReceitaDespesa.Receita },
    { text: "Desvínculo", value: EnumReceitaDespesa.Despesa }
];

var PesquisaHistoricoVinculoKmReboque = function () {

    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Reboque:", idBtnSearch: guid() });
    this.DataCriacaoInicial = PropertyEntity({ text: "Data Criação Inicial: ", getType: typesKnockout.date });
    this.DataCriacaoFinal = PropertyEntity({ text: "Data Criação Final: ", getType: typesKnockout.date });
    this.DataAlteracaoInicial = PropertyEntity({ text: "Data Alteração Inicial: ", getType: typesKnockout.date });
    this.DataAlteracaoFinal = PropertyEntity({ text: "Data Alteração Final: ", getType: typesKnockout.date });    
    this.Tipo = PropertyEntity({ val: ko.observable(0), options: _TipoMovimento, def: 0, text: "Tipo: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridHistoricoVinculoKmReboque.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var HistoricoVinculoKmReboque = function () {

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), issue: 143, enable: ko.observable(true) });    
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Reboque:", idBtnSearch: guid(), issue: 143, enable: ko.observable(true) });    
    this.Tipo = PropertyEntity({ val: ko.observable(0), options: _TipoMovimento, def: 0, text: "Tipo: ", enable: ko.observable(true) });
    this.DataCriacao = PropertyEntity({ text: "Data Criação:", getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataAlteracao = PropertyEntity({ text: "Data Alteração:", getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(true) });
    this.KM = PropertyEntity({ text: "*Quilometragem:", required: true, getType: typesKnockout.int, maxlength: 10, enable: ko.observable(true), configInt: { precision: 0, allowZero: true }, def: "0", val: ko.observable("0"), visible: ko.observable(true) });   
    
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });    
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });    
}

//*******EVENTOS*******


function loadHistoricoVinculoKmReboque() {

    _historicoVinculoKmReboque = new HistoricoVinculoKmReboque();
    KoBindings(_historicoVinculoKmReboque, "knockoutCadastroControleKmReboque");

    HeaderAuditoria("HistoricoVinculoKmReboque", _historicoVinculoKmReboque);

    _pesquisaHistoricoVinculoKmReboque = new PesquisaHistoricoVinculoKmReboque();
    KoBindings(_pesquisaHistoricoVinculoKmReboque, "knockoutPesquisaControleKmReboque", false, _pesquisaHistoricoVinculoKmReboque.Pesquisar.id);    

    buscarHistoricoVinculoKmReboques();
    $("#divCadastroControleKmReboque").hide();
}

function atualizarClick(e, sender) {
    Salvar(_historicoVinculoKmReboque, "ControleKmReboque/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridHistoricoVinculoKmReboque.CarregarGrid();
                limparCamposHistoricoVinculoKmReboque();
                $("#divCadastroControleKmReboque").hide();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function cancelarClick(e) {
    limparCamposHistoricoVinculoKmReboque();
    $("#divCadastroControleKmReboque").hide();
}

//*******MÉTODOS*******

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function buscarHistoricoVinculoKmReboques() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarHistoricoVinculoKmReboque, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "ControleKmReboque/ExportarPesquisa",
        titulo: "Controle KM Reboque"
    };

    _gridHistoricoVinculoKmReboque = new GridViewExportacao(_pesquisaHistoricoVinculoKmReboque.Pesquisar.idGrid, "ControleKmReboque/Pesquisa", _pesquisaHistoricoVinculoKmReboque, menuOpcoes, configExportacao, { column: 2, dir: orderDir.asc });
    _gridHistoricoVinculoKmReboque.CarregarGrid();
}

function editarHistoricoVinculoKmReboque(historicoVinculoKmReboqueGrid) {
    $("#divCadastroControleKmReboque").show();
    limparCamposHistoricoVinculoKmReboque();
    _historicoVinculoKmReboque.Codigo.val(historicoVinculoKmReboqueGrid.Codigo);
    BuscarPorCodigo(_historicoVinculoKmReboque, "ControleKmReboque/BuscarPorCodigo", function (arg) {
        _pesquisaHistoricoVinculoKmReboque.ExibirFiltros.visibleFade(false);
        _historicoVinculoKmReboque.Atualizar.visible(true);
        _historicoVinculoKmReboque.Cancelar.visible(true);
        desabilitarCampos(false);
    }, null);
}

function limparCamposHistoricoVinculoKmReboque() {
    _historicoVinculoKmReboque.Atualizar.visible(false);
    _historicoVinculoKmReboque.Cancelar.visible(false);
    LimparCampos(_historicoVinculoKmReboque);
}

function desabilitarCampos(habilitar) {
    _historicoVinculoKmReboque.DataCriacao.enable(habilitar);
    _historicoVinculoKmReboque.DataAlteracao.enable(habilitar);
    _historicoVinculoKmReboque.Veiculo.enable(habilitar);
    _historicoVinculoKmReboque.Reboque.enable(habilitar);
    _historicoVinculoKmReboque.Tipo.enable(habilitar);
}