/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumPessoaClasse.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDPessoaClassificacao;
var _corPadrao = '#FFFFFF';
var _gridPessoaClassificacao;
var _pessoaClassificacao;
var _pesquisaPessoaClassificacao;

var _listaCores = [
    { value: '#FFFFFF' },
    { value: '#ED6464' },
    { value: '#ED8664' },
    { value: '#EDA864' },
    { value: '#EDCB64' },
    { value: '#EDED64' },
    { value: '#CBED64' },
    { value: '#A8ED64' },
    { value: '#86ED64' },
    { value: '#64ED64' },
    { value: '#64ED86' },
    { value: '#64EDA8' },
    { value: '#64EDCB' },
    { value: '#64EDED' },
    { value: '#64CBED' },
    { value: '#64A8ED' },
    { value: '#6495ED' },
    { value: '#6486ED' },
    { value: '#6464ED' },
    { value: '#8664ED' },
    { value: '#A864ED' },
    { value: '#CB64ED' },
    { value: '#ED64ED' },
    { value: '#ED64CB' },
    { value: '#ED64A8' },
    { value: '#ED6486' },
    { value: '#8B4513' },
    { value: '#E06F1F' },
    { value: '#EDA978' },
    { value: '#F9E2D2' },
    { value: '#000000' },
    { value: '#708090' },
    { value: '#9AA6B1' },
    { value: '#C5CCD3' },
    { value: '#F1F2F4' }
];

/*
 * Declaração das Classes
 */

var CRUDPessoaClassificacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo", visible: true });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var PessoaClassificacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Classe = PropertyEntity({ text: "Classe: ", val: ko.observable(EnumPessoaClasse.Um), options: EnumPessoaClasse.obterOpcoes(), def: EnumPessoaClasse.Um });
    this.Cor = PropertyEntity({ text: "Cor: ", val: ko.observable(_corPadrao), options: _listaCores });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });

    this.Cor.val.subscribe(definirCor);
}

var PesquisaPessoaClassificacao = function () {
    this.Classe = PropertyEntity({ text: "Classe: ", val: ko.observable(EnumPessoaClasse.Todas), options: EnumPessoaClasse.obterOpcoesPesquisa(), def: EnumPessoaClasse.Todas });
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridPessoaClassificacao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadCores() {
    $("#" + _pessoaClassificacao.Cor.id).colorselector({
        callback: function (value) {
            _pessoaClassificacao.Cor.val(value);
        }
    });

    definirCor(_corPadrao);
}

function loadGridPessoaClassificacao() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "PessoaClassificacao/ExportarPesquisa", titulo: "Classificação de Pessoas" };

    _gridPessoaClassificacao = new GridViewExportacao(_pesquisaPessoaClassificacao.Pesquisar.idGrid, "PessoaClassificacao/Pesquisa", _pesquisaPessoaClassificacao, menuOpcoes, configuracoesExportacao);
    _gridPessoaClassificacao.CarregarGrid();
}

function loadPessoaClassificacao() {
    _pessoaClassificacao = new PessoaClassificacao();
    KoBindings(_pessoaClassificacao, "knockoutPessoaClassificacao");

    HeaderAuditoria("PessoaClassificacao", _pessoaClassificacao);

    _CRUDPessoaClassificacao = new CRUDPessoaClassificacao();
    KoBindings(_CRUDPessoaClassificacao, "knockoutCRUDPessoaClassificacao");

    _pesquisaPessoaClassificacao = new PesquisaPessoaClassificacao();
    KoBindings(_pesquisaPessoaClassificacao, "knockoutPesquisaPessoaClassificacao", false, _pesquisaPessoaClassificacao.Pesquisar.id);

    loadCores();
    loadGridPessoaClassificacao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_pessoaClassificacao, "PessoaClassificacao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridPessoaClassificacao();
                limparCamposPessoaClassificacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_pessoaClassificacao, "PessoaClassificacao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridPessoaClassificacao();
                limparCamposPessoaClassificacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposPessoaClassificacao();
}

function editarClick(registroSelecionado) {
    limparCamposPessoaClassificacao();

    _pessoaClassificacao.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_pessoaClassificacao, "PessoaClassificacao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaPessoaClassificacao.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_pessoaClassificacao, "PessoaClassificacao/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridPessoaClassificacao();
                    limparCamposPessoaClassificacao();
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
    var isEdicao = _pessoaClassificacao.Codigo.val() > 0;

    _CRUDPessoaClassificacao.Atualizar.visible(isEdicao);
    _CRUDPessoaClassificacao.Excluir.visible(isEdicao);
    _CRUDPessoaClassificacao.Adicionar.visible(!isEdicao);
}

function definirCor() {
    $("#" + _pessoaClassificacao.Cor.id).colorselector("setValue", (_pessoaClassificacao.Cor.val() || _corPadrao));
}

function limparCamposPessoaClassificacao() {
    LimparCampos(_pessoaClassificacao);
    controlarBotoesHabilitados();
}

function recarregarGridPessoaClassificacao() {
    _gridPessoaClassificacao.CarregarGrid();
}
