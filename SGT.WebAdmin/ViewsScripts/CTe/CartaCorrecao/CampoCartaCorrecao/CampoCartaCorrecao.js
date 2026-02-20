/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDCampoCartaCorrecao;
var _campoCartaCorrecao;
var _pesquisaCampoCartaCorrecao;
var _gridCampoCartaCorrecao;

/*
 * Declaração das Classes
 */

var CRUDCampoCartaCorrecao = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var CampoCartaCorrecao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.GrupoCampo = PropertyEntity({ text: "Grupo do Campo:", issue: 0, required: false, getType: typesKnockout.string, val: ko.observable("") });
    this.NomeCampo = PropertyEntity({ text: "Campo:", issue: 0, required: false, getType: typesKnockout.string, val: ko.observable("") });
    this.QuantidadeCaracteres = PropertyEntity({ text: "Qtd. Caracteres:", issue: 0, required: false, getType: typesKnockout.int, val: ko.observable(""), visible: ko.observable(true) });
    this.QuantidadeInteiros = PropertyEntity({ text: "Qtd. Inteiros:", issue: 0, required: false, getType: typesKnockout.int, val: ko.observable(""), visible: ko.observable(false) });
    this.QuantidadeDecimais = PropertyEntity({ text: "Qtd. Decimais:", issue: 0, required: false, getType: typesKnockout.int, val: ko.observable(""), visible: ko.observable(false) });
    this.IndicadorRepeticao = PropertyEntity({ text: "Este campo pode se repetir", issue: 0, required: false, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TipoCampo = PropertyEntity({ text: "Tipo do Campo:", options: EnumTipoCampoCCe.ObterOpcoes(), val: ko.observable(EnumTipoCampoCCe.Texto), def: EnumTipoCampoCCe.Texto, issue: 0, visible: ko.observable(true) });

    this.TipoCampo.val.subscribe(function (tipoCampo) {
        TipoCampoChange(tipoCampo);
    });
};

var PesquisaCampoCartaCorrecao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridCampoCartaCorrecao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridCampoCartaCorrecao() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "CampoCartaCorrecao/ExportarPesquisa", titulo: "Campos para a Carta de Correção" };

    _gridCampoCartaCorrecao = new GridViewExportacao(_pesquisaCampoCartaCorrecao.Pesquisar.idGrid, "CampoCartaCorrecao/Pesquisa", _pesquisaCampoCartaCorrecao, menuOpcoes, configuracoesExportacao);
    _gridCampoCartaCorrecao.CarregarGrid();
}

function LoadCampoCartaCorrecao() {
    _campoCartaCorrecao = new CampoCartaCorrecao();
    KoBindings(_campoCartaCorrecao, "knockoutCampoCartaCorrecao");

    HeaderAuditoria("CampoCartaCorrecao", _campoCartaCorrecao);

    _CRUDCampoCartaCorrecao = new CRUDCampoCartaCorrecao();
    KoBindings(_CRUDCampoCartaCorrecao, "knockoutCRUDCampoCartaCorrecao");

    _pesquisaCampoCartaCorrecao = new PesquisaCampoCartaCorrecao();
    KoBindings(_pesquisaCampoCartaCorrecao, "knockoutPesquisaCampoCartaCorrecao", false, _pesquisaCampoCartaCorrecao.Pesquisar.id);

    LoadGridCampoCartaCorrecao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function TipoCampoChange(tipoCampo) {
    _campoCartaCorrecao.QuantidadeCaracteres.visible(tipoCampo === EnumTipoCampoCCe.Texto);
    _campoCartaCorrecao.QuantidadeDecimais.visible(tipoCampo === EnumTipoCampoCCe.Decimal);
    _campoCartaCorrecao.QuantidadeInteiros.visible(tipoCampo === EnumTipoCampoCCe.Decimal || tipoCampo === EnumTipoCampoCCe.Inteiro);
}

function AdicionarClick(e, sender) {
    Salvar(_campoCartaCorrecao, "CampoCartaCorrecao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridCampoCartaCorrecao();
                LimparCamposCampoCartaCorrecao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_campoCartaCorrecao, "CampoCartaCorrecao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridCampoCartaCorrecao();
                LimparCamposCampoCartaCorrecao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposCampoCartaCorrecao();
}

function EditarClick(registroSelecionado) {
    LimparCamposCampoCartaCorrecao();

    _campoCartaCorrecao.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_campoCartaCorrecao, "CampoCartaCorrecao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaCampoCartaCorrecao.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                ControlarBotoesHabilitados(isEdicao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function ExcluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_campoCartaCorrecao, "CampoCartaCorrecao/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    RecarregarGridCampoCartaCorrecao();
                    LimparCamposCampoCartaCorrecao();
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
    _CRUDCampoCartaCorrecao.Atualizar.visible(isEdicao);
    _CRUDCampoCartaCorrecao.Excluir.visible(isEdicao);
    _CRUDCampoCartaCorrecao.Cancelar.visible(isEdicao);
    _CRUDCampoCartaCorrecao.Adicionar.visible(!isEdicao);
}

function LimparCamposCampoCartaCorrecao() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_campoCartaCorrecao);
}

function RecarregarGridCampoCartaCorrecao() {
    _gridCampoCartaCorrecao.CarregarGrid();
}