/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRegrasInclusaoICMS;
var _corPadrao = '#FFFFFF';
var _gridRegrasInclusaoICMS;
var _regrasInclusaoICMS;
var _pesquisaRegrasInclusaoICMS;


/*
 * Declaração das Classes
 */

var CRUDRegrasInclusaoICMS = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo", visible: true });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var RegrasInclusaoICMS = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: "*Tipo de Pessoa", issue: 306, eventChange: TipoPessoaChange, required: ko.observable(true), visible: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pessoa", idBtnSearch: guid(), required: ko.observable(true), issue: 52, visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Grupo de Pessoas", idBtnSearch: guid(), required: ko.observable(false), issue: 58, visible: ko.observable(false) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação: ", idBtnSearch: guid(), required: false });
    this.Situacao = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });

}

var PesquisaRegrasInclusaoICMS = function () {
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação: ", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(true), options: Global.ObterOpcoesPesquisaBooleano("Ativo", "Inativo"), def: true, text: "Situação: " });


    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridRegrasInclusaoICMS, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridRegrasInclusaoICMS() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "RegrasInclusaoICMS/ExportarPesquisa", titulo: "Regras de Inclusao ICMS" };

    _gridRegrasInclusaoICMS = new GridViewExportacao(_pesquisaRegrasInclusaoICMS.Pesquisar.idGrid, "RegrasInclusaoICMS/Pesquisa", _pesquisaRegrasInclusaoICMS, menuOpcoes, configuracoesExportacao);
    _gridRegrasInclusaoICMS.CarregarGrid();
}

function loadRegrasInclusaoICMS() {
    _regrasInclusaoICMS = new RegrasInclusaoICMS();
    KoBindings(_regrasInclusaoICMS, "knockoutRegrasInclusaoICMS");

    HeaderAuditoria("RegrasInclusaoICMS", _regrasInclusaoICMS);
 
    _CRUDRegrasInclusaoICMS = new CRUDRegrasInclusaoICMS();
    KoBindings(_CRUDRegrasInclusaoICMS, "knockoutCRUDRegrasInclusaoICMS");

    _pesquisaRegrasInclusaoICMS = new PesquisaRegrasInclusaoICMS();
    KoBindings(_pesquisaRegrasInclusaoICMS, "knockoutPesquisaRegrasInclusaoICMS", false, _pesquisaRegrasInclusaoICMS.Pesquisar.id);

    new BuscarClientes(_regrasInclusaoICMS.Pessoa);
    new BuscarClientes(_pesquisaRegrasInclusaoICMS.Pessoa);
   
    new BuscarGruposPessoas(_regrasInclusaoICMS.GrupoPessoa);
    new BuscarGruposPessoas(_pesquisaRegrasInclusaoICMS.GrupoPessoa)

    new BuscarTiposOperacao(_regrasInclusaoICMS.TipoOperacao);
    new BuscarTiposOperacao(_pesquisaRegrasInclusaoICMS.TipoOperacao);

    loadGridRegrasInclusaoICMS();

}

/*
 * Declaração das Funções Associadas a Eventos
 */


function TipoPessoaChange(e, sender) {
    if (_regrasInclusaoICMS.TipoPessoa.val() == EnumTipoPessoaGrupo.Pessoa) {
        if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal || !_regrasInclusaoICMS.TipoPessoa.required())
            _regrasInclusaoICMS.Pessoa.required(false);
        else
            _regrasInclusaoICMS.Pessoa.required(true);
        _regrasInclusaoICMS.Pessoa.visible(true);
        _regrasInclusaoICMS.GrupoPessoa.required(false);
        _regrasInclusaoICMS.GrupoPessoa.visible(false);
        LimparCampoEntity(_regrasInclusaoICMS.GrupoPessoa);
    } else if (_regrasInclusaoICMS.TipoPessoa.val() == EnumTipoPessoaGrupo.GrupoPessoa) {
        _regrasInclusaoICMS.Pessoa.required(false);
        _regrasInclusaoICMS.Pessoa.visible(false);
        if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal || !_regrasInclusaoICMS.TipoPessoa.required())
            _regrasInclusaoICMS.GrupoPessoa.required(false);
        else
            _regrasInclusaoICMS.GrupoPessoa.required(true);
        _regrasInclusaoICMS.GrupoPessoa.visible(true);
        LimparCampoEntity(_regrasInclusaoICMS.Pessoa);
    }
}


function adicionarClick(e, sender) {
    if (!ValidarCamposObrigatoriosRegrasInclusaoICMS())
        return;

    var regrasInclusaoICMS = obterRegrasInclusaoICMSSalvar();
    executarReST("RegrasInclusaoICMS/Adicionar", regrasInclusaoICMS, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridRegrasInclusaoICMS.CarregarGrid();
                limparCamposRegrasInclusaoICMS();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    if (!ValidarCamposObrigatoriosRegrasInclusaoICMS())
        return;

    var regrasInclusaoICMS = obterRegrasInclusaoICMSSalvar();
    executarReST("RegrasInclusaoICMS/Atualizar", regrasInclusaoICMS, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridRegrasInclusaoICMS.CarregarGrid();
                limparCamposRegrasInclusaoICMS();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }), sender;
}

function cancelarClick() {
    limparCamposRegrasInclusaoICMS();
}

function editarClick(registroSelecionado) {
    limparCamposRegrasInclusaoICMS();

    _regrasInclusaoICMS.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_regrasInclusaoICMS, "RegrasInclusaoICMS/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaRegrasInclusaoICMS.ExibirFiltros.visibleFade(false);
               
                controlarBotoesHabilitados();
                TipoPessoaChange();
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
        ExcluirPorCodigo(_regrasInclusaoICMS, "RegrasInclusaoICMS/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridRegrasInclusaoICMS();
                    limparCamposRegrasInclusaoICMS();
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

function obterRegrasInclusaoICMSSalvar() {
    

    var regrasInclusaoICMS = RetornarObjetoPesquisa(_regrasInclusaoICMS);

    return regrasInclusaoICMS;
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados() {
    var isEdicao = _regrasInclusaoICMS.Codigo.val() > 0;

    _CRUDRegrasInclusaoICMS.Atualizar.visible(isEdicao);
    _CRUDRegrasInclusaoICMS.Excluir.visible(isEdicao);
    _CRUDRegrasInclusaoICMS.Adicionar.visible(!isEdicao);
}

function limparCamposRegrasInclusaoICMS() {
    LimparCampos(_regrasInclusaoICMS);
   
    controlarBotoesHabilitados();
}

function recarregarGridRegrasInclusaoICMS() {
    _gridRegrasInclusaoICMS.CarregarGrid();
}

function ValidarCamposObrigatoriosRegrasInclusaoICMS() {
    if (!ValidarCamposObrigatorios(_regrasInclusaoICMS)) {
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Por favor, insira os campos obrigatórios.");
        return false;
    }
    return true;
}
