/// <reference path="../../Consultas/BoletoConfiguracao.js" />
/// <reference path="../../Enumeradores/EnumAnaliticoSintetico.js" />
/// <reference path="../../Enumeradores/EnumReceitaDespesa.js" />
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
/// <reference path="../../Enumeradores/EnumGrupoDeResultado.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridPagamentoEletronicoComandoRetorno;
var _pagamentoEletronicoComandoRetorno;
var _pesquisaPagamentoEletronicoComandoRetorno;

var PesquisaPagamentoEletronicoComandoRetorno = function () {
    this.Comando = PropertyEntity({ text: "Comando: " });
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.BoletoConfiguracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Configuração Banco:", idBtnSearch: guid(), val: ko.observable("") });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPagamentoEletronicoComandoRetorno.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var PagamentoEletronicoComandoRetorno = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Comando = PropertyEntity({ text: "*Comando: ", required: false, maxlength: 20 });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 1000 });
    this.BoletoConfiguracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Configuração Banco:", idBtnSearch: guid(), required: true, val: ko.observable("") });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });    

    this.ComandoDeLiquidacao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Liquidar título quando retornar com este comando?", def: false, visible: ko.observable(true) });
    this.ComandoDeEstorno = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Estornar o título quando retornar com este comando?", def: false, visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadPagamentoEletronicoComandoRetorno() {

    _pagamentoEletronicoComandoRetorno = new PagamentoEletronicoComandoRetorno();
    KoBindings(_pagamentoEletronicoComandoRetorno, "knockoutCadastroPagamentoEletronicoComandoRetorno");

    HeaderAuditoria("PagamentoEletronicoComandoRetorno", _pagamentoEletronicoComandoRetorno);

    _pesquisaPagamentoEletronicoComandoRetorno = new PesquisaPagamentoEletronicoComandoRetorno();
    KoBindings(_pesquisaPagamentoEletronicoComandoRetorno, "knockoutPesquisaPagamentoEletronicoComandoRetorno", false, _pesquisaPagamentoEletronicoComandoRetorno.Pesquisar.id);

    new BuscarBoletoConfiguracao(_pesquisaPagamentoEletronicoComandoRetorno.BoletoConfiguracao, retornoConfiguracaoBoletoPesquisa, true);
    new BuscarBoletoConfiguracao(_pagamentoEletronicoComandoRetorno.BoletoConfiguracao, retornoConfiguracaoBoleto, true);

    buscarPagamentoEletronicoComandoRetornos();
}

function retornoConfiguracaoBoletoPesquisa(data) {
    _pesquisaPagamentoEletronicoComandoRetorno.BoletoConfiguracao.val(data.DescricaoBanco);
    _pesquisaPagamentoEletronicoComandoRetorno.BoletoConfiguracao.codEntity(data.Codigo);
}

function retornoConfiguracaoBoleto(data) {
    _pagamentoEletronicoComandoRetorno.BoletoConfiguracao.val(data.DescricaoBanco);
    _pagamentoEletronicoComandoRetorno.BoletoConfiguracao.codEntity(data.Codigo);
}

function adicionarClick(e, sender) {
    Salvar(e, "PagamentoEletronicoComandoRetorno/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridPagamentoEletronicoComandoRetorno.CarregarGrid();
                limparCamposPagamentoEletronicoComandoRetorno();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {            
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "PagamentoEletronicoComandoRetorno/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridPagamentoEletronicoComandoRetorno.CarregarGrid();
                limparCamposPagamentoEletronicoComandoRetorno();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o comando de retorno " + _pagamentoEletronicoComandoRetorno.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_pagamentoEletronicoComandoRetorno, "PagamentoEletronicoComandoRetorno/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridPagamentoEletronicoComandoRetorno.CarregarGrid();
                limparCamposPagamentoEletronicoComandoRetorno();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposPagamentoEletronicoComandoRetorno();
}

//*******MÉTODOS*******


function buscarPagamentoEletronicoComandoRetornos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPagamentoEletronicoComandoRetorno, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "PagamentoEletronicoComandoRetorno/ExportarPesquisa",
        titulo: "Comando Retorno"
    };

    _gridPagamentoEletronicoComandoRetorno = new GridViewExportacao(_pesquisaPagamentoEletronicoComandoRetorno.Pesquisar.idGrid, "PagamentoEletronicoComandoRetorno/Pesquisa", _pesquisaPagamentoEletronicoComandoRetorno, menuOpcoes, configExportacao, { column: 2, dir: orderDir.asc });
    _gridPagamentoEletronicoComandoRetorno.CarregarGrid();
}

function editarPagamentoEletronicoComandoRetorno(pagamentoEletronicoComandoRetornoGrid) {
    limparCamposPagamentoEletronicoComandoRetorno();
    _pagamentoEletronicoComandoRetorno.Codigo.val(pagamentoEletronicoComandoRetornoGrid.Codigo);
    BuscarPorCodigo(_pagamentoEletronicoComandoRetorno, "PagamentoEletronicoComandoRetorno/BuscarPorCodigo", function (arg) {
        _pesquisaPagamentoEletronicoComandoRetorno.ExibirFiltros.visibleFade(false);
        _pagamentoEletronicoComandoRetorno.Atualizar.visible(true);
        _pagamentoEletronicoComandoRetorno.Cancelar.visible(true);
        _pagamentoEletronicoComandoRetorno.Excluir.visible(true);
        _pagamentoEletronicoComandoRetorno.Adicionar.visible(false);
    }, null);
}

function limparCamposPagamentoEletronicoComandoRetorno() {
    _pagamentoEletronicoComandoRetorno.Atualizar.visible(false);
    _pagamentoEletronicoComandoRetorno.Cancelar.visible(false);
    _pagamentoEletronicoComandoRetorno.Excluir.visible(false);
    _pagamentoEletronicoComandoRetorno.Adicionar.visible(true);
    LimparCampos(_pagamentoEletronicoComandoRetorno);
}
