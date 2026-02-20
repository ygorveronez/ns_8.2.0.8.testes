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
/// <reference path="../../Enumeradores/EnumTipoParametroOcorrencia.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridParametrosOcorrencia;
var _parametroOcorrencia;
var _pesquisaParametroOcorrencia;

var PesquisaParametroOcorrencia = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridParametrosOcorrencia.CarregarGrid();
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

var ParametroOcorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoParametroOcorrencia.Data), options: EnumTipoParametroOcorrencia.obterOpcoes(), def: EnumTipoParametroOcorrencia.Data, text: "*Tipo: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });

    this.DescricaoParametro = PropertyEntity({ text: ko.observable("") });
    this.DescricaoParametroFinal = PropertyEntity({ text: ko.observable(""), visible: ko.observable(false) });
    
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadParametroOcorrencia() {
    _parametroOcorrencia = new ParametroOcorrencia();
    KoBindings(_parametroOcorrencia, "knockoutCadastroParametroOcorrencia");

    _pesquisaParametroOcorrencia = new PesquisaParametroOcorrencia();
    KoBindings(_pesquisaParametroOcorrencia, "knockoutPesquisaParametroOcorrencia", false, _pesquisaParametroOcorrencia.Pesquisar.id);

    HeaderAuditoria("ParametroOcorrencia", _parametroOcorrencia);

    buscarParametrosOcorrencia();

    _parametroOcorrencia.Tipo.val.subscribe(onChangeTipoParametro);
    onChangeTipoParametro();
}

function adicionarClick(e, sender) {
    Salvar(e, "ParametroOcorrencia/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridParametrosOcorrencia.CarregarGrid();
                limparCamposParametroOcorrencia();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "ParametroOcorrencia/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridParametrosOcorrencia.CarregarGrid();
                limparCamposParametroOcorrencia();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirMensagem(tipoMensagem.aviso, "aviso", "Não é possível excluir um parâmetro, caso necessário deve-se inativar");
}

function cancelarClick(e) {
    limparCamposParametroOcorrencia();
}

//*******MÉTODOS*******
function buscarParametrosOcorrencia() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarParametroOcorrencia, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridParametrosOcorrencia = new GridView(_pesquisaParametroOcorrencia.Pesquisar.idGrid, "ParametroOcorrencia/Pesquisa", _pesquisaParametroOcorrencia, menuOpcoes, null);
    _gridParametrosOcorrencia.CarregarGrid();
}

function editarParametroOcorrencia(parametroOcorrencia) {
    limparCamposParametroOcorrencia();
    _parametroOcorrencia.Codigo.val(parametroOcorrencia.Codigo);
    BuscarPorCodigo(_parametroOcorrencia, "ParametroOcorrencia/BuscarPorCodigo", function (arg) {
        _pesquisaParametroOcorrencia.ExibirFiltros.visibleFade(false);
        _parametroOcorrencia.Atualizar.visible(true);
        _parametroOcorrencia.Cancelar.visible(true);
        _parametroOcorrencia.Excluir.visible(true);
        _parametroOcorrencia.Adicionar.visible(false);
    }, null);
}

function limparCamposParametroOcorrencia() {
    _parametroOcorrencia.Atualizar.visible(false);
    _parametroOcorrencia.Cancelar.visible(false);
    _parametroOcorrencia.Excluir.visible(false);
    _parametroOcorrencia.Adicionar.visible(true);
    LimparCampos(_parametroOcorrencia);
}

function onChangeTipoParametro() {
    _parametroOcorrencia.DescricaoParametro.text("Descrição parâmetro:");
    _parametroOcorrencia.DescricaoParametroFinal.text("");
    _parametroOcorrencia.DescricaoParametroFinal.visible(false);

    if (_parametroOcorrencia.Tipo.val() == EnumTipoParametroOcorrencia.Periodo) {
        _parametroOcorrencia.DescricaoParametro.text("Descrição parâmetro inicial:");
        _parametroOcorrencia.DescricaoParametroFinal.text("Descrição parâmetro final:");
        _parametroOcorrencia.DescricaoParametroFinal.visible(true);
    }
}
