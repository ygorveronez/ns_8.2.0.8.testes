/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CategoriaResponsavel.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="ResponsavelCargaFilial.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDResponsavelCarga;
var _gridResponsavelCarga;
var _responsavelCarga;
var _pesquisaResponsavelCarga;

/*
 * Declaração das Classes
 */

var CRUDResponsavelCarga = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo" });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var ResponsavelCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CategoriaResponsavel = PropertyEntity({ text: "*Categoria de Responsável:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid(), required: true });
    this.Funcionario = PropertyEntity({ text: "*Funcionário:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid(), required: true });
    this.VigenciaInicial = PropertyEntity({ text: "Vigência Inicial: ", getType: typesKnockout.date });
    this.VigenciaFinal = PropertyEntity({ text: "Vigência Final: ", dateRangeInit: this.VigenciaInicial, getType: typesKnockout.date });

    this.VigenciaInicial.dateRangeLimit = this.VigenciaFinal;
    this.VigenciaFinal.dateRangeInit = this.VigenciaInicial;
}

var PesquisaResponsavelCarga = function () {
    this.CategoriaResponsavel = PropertyEntity({ text: "Categoria de Responsável:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.Funcionario = PropertyEntity({ text: "Funcionário:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.VigenciaInicial = PropertyEntity({ text: "Vigência Inicial: ", getType: typesKnockout.date });
    this.VigenciaFinal = PropertyEntity({ text: "Vigência Final: ", dateRangeInit: this.VigenciaInicial, getType: typesKnockout.date });

    this.VigenciaInicial.dateRangeLimit = this.VigenciaFinal;
    this.VigenciaFinal.dateRangeInit = this.VigenciaInicial;

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridResponsavelCarga, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridResponsavelCarga() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "ResponsavelCarga/ExportarPesquisa", titulo: "Eventos de Monitoramento" };

    _gridResponsavelCarga = new GridViewExportacao(_pesquisaResponsavelCarga.Pesquisar.idGrid, "ResponsavelCarga/Pesquisa", _pesquisaResponsavelCarga, menuOpcoes, configuracoesExportacao);
    _gridResponsavelCarga.CarregarGrid();
}

function loadResponsavelCarga() {
    _responsavelCarga = new ResponsavelCarga();
    KoBindings(_responsavelCarga, "knockoutResponsavelCarga");

    HeaderAuditoria("ResponsavelCarga", _responsavelCarga);

    _CRUDResponsavelCarga = new CRUDResponsavelCarga();
    KoBindings(_CRUDResponsavelCarga, "knockoutCRUDResponsavelCarga");

    _pesquisaResponsavelCarga = new PesquisaResponsavelCarga();
    KoBindings(_pesquisaResponsavelCarga, "knockoutPesquisaResponsavelCarga", false, _pesquisaResponsavelCarga.Pesquisar.id);

    new BuscarCategoriaResponsavel(_pesquisaResponsavelCarga.CategoriaResponsavel);
    new BuscarCategoriaResponsavel(_responsavelCarga.CategoriaResponsavel);
    new BuscarFuncionario(_pesquisaResponsavelCarga.Funcionario);
    new BuscarFuncionario(_responsavelCarga.Funcionario);

    loadResponsavelCargaFilial();
    loadGridResponsavelCarga();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) 
        $("#liResponsavelCargaFilial").hide();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick() {
    if (ValidarCamposObrigatorios(_responsavelCarga)) {
        executarReST("ResponsavelCarga/Adicionar", obterResponsavelCargaSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                    recarregarGridResponsavelCarga();
                    limparCamposResponsavelCarga();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
    else
        exibirMensagemCamposObrigatorio();
}

function atualizarClick() {
    if (ValidarCamposObrigatorios(_responsavelCarga)) {
        executarReST("ResponsavelCarga/Atualizar", obterResponsavelCargaSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                    recarregarGridResponsavelCarga();
                    limparCamposResponsavelCarga();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
    else
        exibirMensagemCamposObrigatorio();
}

function cancelarClick() {
    limparCamposResponsavelCarga();
}

function editarClick(registroSelecionado) {
    limparCamposResponsavelCarga();

    executarReST("ResponsavelCarga/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaResponsavelCarga.ExibirFiltros.visibleFade(false);

                PreencherObjetoKnout(_responsavelCarga, { Data: retorno.Data.Dados });
                preencherResponsavelCargaFilial(retorno.Data.Filiais);

                controlarBotoesHabilitados();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_responsavelCarga, "ResponsavelCarga/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridResponsavelCarga();
                    limparCamposResponsavelCarga();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados() {
    var isEdicao = _responsavelCarga.Codigo.val() > 0;

    _CRUDResponsavelCarga.Atualizar.visible(isEdicao);
    _CRUDResponsavelCarga.Excluir.visible(isEdicao);
    _CRUDResponsavelCarga.Adicionar.visible(!isEdicao);
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function limparCamposResponsavelCarga() {
    LimparCampos(_responsavelCarga);
    limparCamposResponsavelCargaFilial();
    controlarBotoesHabilitados();

    $("#tabResponsavelCargaDados").click();
}

function obterResponsavelCargaSalvar() {
    var responsavelCarga = RetornarObjetoPesquisa(_responsavelCarga);

    responsavelCarga["Filiais"] = obterResponsavelCargaFilialSalvar();

    return responsavelCarga;
}

function recarregarGridResponsavelCarga() {
    _gridResponsavelCarga.CarregarGrid();
}