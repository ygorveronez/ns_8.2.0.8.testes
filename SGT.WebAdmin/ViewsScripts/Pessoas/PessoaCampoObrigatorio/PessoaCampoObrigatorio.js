/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="Campo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDPessoaCampoObrigatorio;
var _pessoaCampoObrigatorio;
var _pesquisaPessoaCampoObrigatorio;
var _gridPessoaCampoObrigatorio;

/*
 * Declaração das Classes
 */

var CRUDPessoaCampoObrigatorio = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var PessoaCampoObrigatorio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Cliente = PropertyEntity({ text: "Cliente", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Fornecedor = PropertyEntity({ text: "Fornecedor", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Terceiro = PropertyEntity({ text: "Transportador Terceiro", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: _status, def: true });

    this.Campo = PropertyEntity({ type: types.event, text: "Adicionar Campo", idBtnSearch: guid(), enable: ko.observable(true) });
    this.GridCampo = PropertyEntity({ type: types.local });
    this.Campos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

var PesquisaPessoaCampoObrigatorio = function () {
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: Global.ObterOpcoesPesquisaBooleano("Ativo", "Inativo"), def: true });

    this.Cliente = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), text: "Cliente: ", def: EnumSimNaoPesquisa.Todos });
    this.Fornecedor = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), text: "Fornecedor: ", def: EnumSimNaoPesquisa.Todos });
    this.Terceiro = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), text: "Transportador Terceiro: ", def: EnumSimNaoPesquisa.Todos });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridPessoaCampoObrigatorio, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridPessoaCampoObrigatorio() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "PessoaCampoObrigatorio/ExportarPesquisa", titulo: "Campos Obrigatórios para o Pessoa" };

    _gridPessoaCampoObrigatorio = new GridViewExportacao(_pesquisaPessoaCampoObrigatorio.Pesquisar.idGrid, "PessoaCampoObrigatorio/Pesquisa", _pesquisaPessoaCampoObrigatorio, menuOpcoes, configuracoesExportacao);
    _gridPessoaCampoObrigatorio.CarregarGrid();
}

function LoadPessoaCampoObrigatorio() {
    _pessoaCampoObrigatorio = new PessoaCampoObrigatorio();
    KoBindings(_pessoaCampoObrigatorio, "knockoutPessoaCampoObrigatorio");

    HeaderAuditoria("PessoaCampoObrigatorio", _pessoaCampoObrigatorio);

    _CRUDPessoaCampoObrigatorio = new CRUDPessoaCampoObrigatorio();
    KoBindings(_CRUDPessoaCampoObrigatorio, "knockoutCRUDPessoaCampoObrigatorio");

    _pesquisaPessoaCampoObrigatorio = new PesquisaPessoaCampoObrigatorio();
    KoBindings(_pesquisaPessoaCampoObrigatorio, "knockoutPesquisaPessoaCampoObrigatorio", false, _pesquisaPessoaCampoObrigatorio.Pesquisar.id);

    LoadCampoPessoa();
    LoadGridPessoaCampoObrigatorio();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AdicionarClick(e, sender) {
    _pessoaCampoObrigatorio.Campos.val(JSON.stringify(_pessoaCampoObrigatorio.Campo.basicTable.BuscarRegistros()));

    Salvar(_pessoaCampoObrigatorio, "PessoaCampoObrigatorio/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridPessoaCampoObrigatorio();
                LimparCamposPessoaCampoObrigatorio();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    _pessoaCampoObrigatorio.Campos.val(JSON.stringify(_pessoaCampoObrigatorio.Campo.basicTable.BuscarRegistros()));

    Salvar(_pessoaCampoObrigatorio, "PessoaCampoObrigatorio/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");

                RecarregarGridPessoaCampoObrigatorio();
                LimparCamposPessoaCampoObrigatorio();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposPessoaCampoObrigatorio();
}

function EditarClick(registroSelecionado) {
    LimparCamposPessoaCampoObrigatorio();

    _pessoaCampoObrigatorio.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_pessoaCampoObrigatorio, "PessoaCampoObrigatorio/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaPessoaCampoObrigatorio.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                ControlarBotoesHabilitados(isEdicao);

                _pessoaCampoObrigatorio.Campo.basicTable.CarregarGrid(_pessoaCampoObrigatorio.Campos.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function ExcluirClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir este registro?", function () {
        ExcluirPorCodigo(_pessoaCampoObrigatorio, "PessoaCampoObrigatorio/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");

                    RecarregarGridPessoaCampoObrigatorio();
                    LimparCamposPessoaCampoObrigatorio();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function ControlarBotoesHabilitados(isEdicao) {
    _CRUDPessoaCampoObrigatorio.Atualizar.visible(isEdicao);
    _CRUDPessoaCampoObrigatorio.Excluir.visible(isEdicao);
    _CRUDPessoaCampoObrigatorio.Cancelar.visible(isEdicao);
    _CRUDPessoaCampoObrigatorio.Adicionar.visible(!isEdicao);
}

function LimparCamposPessoaCampoObrigatorio() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_pessoaCampoObrigatorio);

    _pessoaCampoObrigatorio.Campo.basicTable.CarregarGrid([]);
}

function RecarregarGridPessoaCampoObrigatorio() {
    _gridPessoaCampoObrigatorio.CarregarGrid();
}