/// <reference path="../../Enumeradores/EnumFormaTipoMovimento.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/ProcessoMovimento.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Enumeradores/EnumAnaliticoSintetico.js" />
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

//*******MAPEAMENTO KNOUCKOUT*******

var _gridConfiguracaoMovimento;
var _configuracaoMovimento;
var _pesquisaConfiguracaoMovimento;

var PesquisaConfiguracaoMovimento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.ProcessoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Processo de Movimento:", idBtnSearch: guid() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConfiguracaoMovimento.CarregarGrid();
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

var ConfiguracaoMovimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });
    this.ProcessoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Processo de Movimento:", idBtnSearch: guid() });
    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Tipo de Movimento:", idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Centro de Resultado:", idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Empresa/Filial:", idBtnSearch: guid() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadConfiguracaoMovimento() {

    _configuracaoMovimento = new ConfiguracaoMovimento();
    KoBindings(_configuracaoMovimento, "knockoutCadastroConfiguracaoMovimento");

    _pesquisaConfiguracaoMovimento = new PesquisaConfiguracaoMovimento();
    KoBindings(_pesquisaConfiguracaoMovimento, "knockoutPesquisaConfiguracaoMovimento", false, _pesquisaConfiguracaoMovimento.Pesquisar.id);

    HeaderAuditoria("ConfiguracaoMovimento", _configuracaoMovimento);

    new BuscarProcessoMovimento(_configuracaoMovimento.ProcessoMovimento);
    new BuscarProcessoMovimento(_pesquisaConfiguracaoMovimento.ProcessoMovimento);
    new BuscarTipoMovimento(_configuracaoMovimento.TipoMovimento, null, null, RetornoTipoMovimento, null, EnumFormaTipoMovimento.Automatico);
    new BuscarCentroResultado(_configuracaoMovimento.CentroResultado, "Selecione as Contas Analiticas", "Contas Analiticas", null, EnumAnaliticoSintetico.Analitico, _configuracaoMovimento.TipoMovimento);
    new BuscarEmpresa(_configuracaoMovimento.Empresa, RetornoEmpresa);

    buscarConfiguracaoMovimentos();
}

function RetornoEmpresa(data) {
    if (data != null) {
        _configuracaoMovimento.Empresa.codEntity(data.Codigo);
        _configuracaoMovimento.Empresa.val(data.RazaoSocial);
    } else
        LimparCampoEntity(_configuracaoMovimento.Empresa);
}

function RetornoTipoMovimento(data) {
    if (data != null) {
        _configuracaoMovimento.TipoMovimento.codEntity(data.Codigo);
        _configuracaoMovimento.TipoMovimento.val(data.Descricao);

        if (data.CodigoResultado > 0) {
            _configuracaoMovimento.CentroResultado.codEntity(data.CodigoResultado);
            _configuracaoMovimento.CentroResultado.val(data.CentroResultado);
        } else {
            LimparCampoEntity(_configuracaoMovimento.CentroResultado);
        }
    }
}

function adicionarClick(e, sender) {
    Salvar(e, "ConfiguracaoMovimento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridConfiguracaoMovimento.CarregarGrid();
                limparCamposConfiguracaoMovimento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "ConfiguracaoMovimento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridConfiguracaoMovimento.CarregarGrid();
                limparCamposConfiguracaoMovimento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a configuração do movimento " + _configuracaoMovimento.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_configuracaoMovimento, "ConfiguracaoMovimento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridConfiguracaoMovimento.CarregarGrid();
                limparCamposConfiguracaoMovimento();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposConfiguracaoMovimento();
}

//*******MÉTODOS*******


function buscarConfiguracaoMovimentos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarConfiguracaoMovimento, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridConfiguracaoMovimento = new GridView(_pesquisaConfiguracaoMovimento.Pesquisar.idGrid, "ConfiguracaoMovimento/Pesquisa", _pesquisaConfiguracaoMovimento, menuOpcoes, null);
    _gridConfiguracaoMovimento.CarregarGrid();
}

function editarConfiguracaoMovimento(configuracaoMovimentoGrid) {
    limparCamposConfiguracaoMovimento();
    _configuracaoMovimento.Codigo.val(configuracaoMovimentoGrid.Codigo);
    BuscarPorCodigo(_configuracaoMovimento, "ConfiguracaoMovimento/BuscarPorCodigo", function (arg) {
        _pesquisaConfiguracaoMovimento.ExibirFiltros.visibleFade(false);
        _configuracaoMovimento.Atualizar.visible(true);
        _configuracaoMovimento.Cancelar.visible(true);
        _configuracaoMovimento.Excluir.visible(true);
        _configuracaoMovimento.Adicionar.visible(false);
    }, null);
}

function limparCamposConfiguracaoMovimento() {
    _configuracaoMovimento.Atualizar.visible(false);
    _configuracaoMovimento.Cancelar.visible(false);
    _configuracaoMovimento.Excluir.visible(false);
    _configuracaoMovimento.Adicionar.visible(true);
    LimparCampos(_configuracaoMovimento);
}
