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
/// <reference path="../../Configuracao/ConfiguracaoTMS.js" />
/// <reference path="Campo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridArquivoImportacaoNotaFiscal, _arquivoImportacaoNotaFiscal, _pesquisaArquivoImportacaoNotaFiscal, _crudArquivoImportacaoNotaFiscal;

var PesquisaArquivoImportacaoNotaFiscal = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridArquivoImportacaoNotaFiscal.CarregarGrid();
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

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var ArquivoImportacaoNotaFiscal = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });

    this.ListaColunas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
}

var CRUDArquivoImportacaoNotaFiscal = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadArquivoImportacaoNotaFiscal() {

    _arquivoImportacaoNotaFiscal = new ArquivoImportacaoNotaFiscal();
    KoBindings(_arquivoImportacaoNotaFiscal, "knockoutCadastro");

    _crudArquivoImportacaoNotaFiscal = new CRUDArquivoImportacaoNotaFiscal();
    KoBindings(_crudArquivoImportacaoNotaFiscal, "knockoutCRUD");

    _pesquisaArquivoImportacaoNotaFiscal = new PesquisaArquivoImportacaoNotaFiscal();
    KoBindings(_pesquisaArquivoImportacaoNotaFiscal, "knockoutPesquisaArquivoImportacaoNotaFiscal", false, _pesquisaArquivoImportacaoNotaFiscal.Pesquisar.id);

    HeaderAuditoria("ArquivoImportacaoNotaFiscal", _arquivoImportacaoNotaFiscal, "Codigo", {
        ArquivoImportacaoNotaFiscalCampo: "Campo"
    });

    buscarArquivoImportacaoNotaFiscal();

    loadCampo();
}

function adicionarClick(e, sender) {
    Salvar(_arquivoImportacaoNotaFiscal, "ArquivoImportacaoNotaFiscal/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridArquivoImportacaoNotaFiscal.CarregarGrid();
                limparCamposArquivoImportacaoNotaFiscal();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, function () {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        resetarTabs();
    });
}

function atualizarClick(e, sender) {
    Salvar(_arquivoImportacaoNotaFiscal, "ArquivoImportacaoNotaFiscal/Atualizar", function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
            _gridArquivoImportacaoNotaFiscal.CarregarGrid();
            limparCamposArquivoImportacaoNotaFiscal();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, function () {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        resetarTabs();
    });
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a tabela frete " + _arquivoImportacaoNotaFiscal.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_arquivoImportacaoNotaFiscal, "ArquivoImportacaoNotaFiscal/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridArquivoImportacaoNotaFiscal.CarregarGrid();
                    limparCamposArquivoImportacaoNotaFiscal();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposArquivoImportacaoNotaFiscal();
}

//*******MÉTODOS*******

function buscarArquivoImportacaoNotaFiscal() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarArquivoImportacaoNotaFiscal, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridArquivoImportacaoNotaFiscal = new GridView(_pesquisaArquivoImportacaoNotaFiscal.Pesquisar.idGrid, "ArquivoImportacaoNotaFiscal/Pesquisa", _pesquisaArquivoImportacaoNotaFiscal, menuOpcoes, null);
    _gridArquivoImportacaoNotaFiscal.CarregarGrid();
}

function editarArquivoImportacaoNotaFiscal(arquivoGrid) {
    limparCamposArquivoImportacaoNotaFiscal();
    _arquivoImportacaoNotaFiscal.Codigo.val(arquivoGrid.Codigo);

    BuscarPorCodigo(_arquivoImportacaoNotaFiscal, "ArquivoImportacaoNotaFiscal/BuscarPorCodigo", function (arg) {
        _pesquisaArquivoImportacaoNotaFiscal.ExibirFiltros.visibleFade(false);
        _crudArquivoImportacaoNotaFiscal.Atualizar.visible(true);
        _crudArquivoImportacaoNotaFiscal.Cancelar.visible(true);
        _crudArquivoImportacaoNotaFiscal.Excluir.visible(true);
        _crudArquivoImportacaoNotaFiscal.Adicionar.visible(false);

        resetarTabs();

        recarregarGridCampo();
        
    }, null);
}

function limparCamposArquivoImportacaoNotaFiscal() {
    _crudArquivoImportacaoNotaFiscal.Atualizar.visible(false);
    _crudArquivoImportacaoNotaFiscal.Cancelar.visible(false);
    _crudArquivoImportacaoNotaFiscal.Excluir.visible(false);
    _crudArquivoImportacaoNotaFiscal.Adicionar.visible(true);
  
    LimparCampos(_arquivoImportacaoNotaFiscal);

    resetarTabs();
    
    limparCamposCampo();

    recarregarGridCampo();
}

function resetarTabs() {
    $("#tabArquivoImportacaoNotaFiscal a:first").tab("show");
}