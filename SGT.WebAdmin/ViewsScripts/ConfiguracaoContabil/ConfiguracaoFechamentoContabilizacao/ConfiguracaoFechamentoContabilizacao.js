/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDConfiguracaoFechamentoContabilizacao;
var _configuracaoFechamentoContabilizacao;
var _pesquisaConfiguracaoFechamentoContabilizacao;
var _gridConfiguracaoFechamentoContabilizacao;

/*
 * Declaração das Classes
 */

var CRUDConfiguracaoFechamentoContabilizacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var ConfiguracaoFechamentoContabilizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.MesReferencia = PropertyEntity({ text: "*Mês Referência:", options: Global.ObterOpcoesInteiro(1, 12), getType: typesKnockout.int, def: 1, val: ko.observable(1) });
    this.AnoReferencia = PropertyEntity({ text: "*Ano Referência:", getType: typesKnockout.int, val: ko.observable(""), maxlength: 4, configInt: { precision: 0, allowZero: false, thousands: "" }  });
    this.UltimoDiaEnvio = PropertyEntity({ text: "*Último dia para envio: ", getType: typesKnockout.date, val: ko.observable(""), def: "" });
};

var PesquisaConfiguracaoFechamentoContabilizacao = function () {
    this.MesReferencia = PropertyEntity({ text: "Mês Referência:", options: Global.ObterOpcoesInteiro(1, 12, true), getType: typesKnockout.int, def: "", val: ko.observable("") });
    this.AnoReferencia = PropertyEntity({ text: "Ano Referência:", getType: typesKnockout.int, val: ko.observable(""), maxlength: 4, configInt: { precision: 0, allowZero: false, thousands: "" } });

    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridConfiguracaoFechamentoContabilizacao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridConfiguracaoFechamentoContabilizacao() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "ConfiguracaoFechamentoContabilizacao/ExportarPesquisa", titulo: "Configuração de Fechamento para Contabilização" };

    _gridConfiguracaoFechamentoContabilizacao = new GridViewExportacao(_pesquisaConfiguracaoFechamentoContabilizacao.Pesquisar.idGrid, "ConfiguracaoFechamentoContabilizacao/Pesquisa", _pesquisaConfiguracaoFechamentoContabilizacao, menuOpcoes, configuracoesExportacao, { column: 3, dir: orderDir.desc });
    _gridConfiguracaoFechamentoContabilizacao.CarregarGrid();
}

function LoadConfiguracaoFechamentoContabilizacao() {
    _configuracaoFechamentoContabilizacao = new ConfiguracaoFechamentoContabilizacao();
    KoBindings(_configuracaoFechamentoContabilizacao, "knockoutConfiguracaoFechamentoContabilizacao");

    HeaderAuditoria("ConfiguracaoFechamentoContabilizacao", _configuracaoFechamentoContabilizacao);

    _CRUDConfiguracaoFechamentoContabilizacao = new CRUDConfiguracaoFechamentoContabilizacao();
    KoBindings(_CRUDConfiguracaoFechamentoContabilizacao, "knockoutCRUDConfiguracaoFechamentoContabilizacao");

    _pesquisaConfiguracaoFechamentoContabilizacao = new PesquisaConfiguracaoFechamentoContabilizacao();
    KoBindings(_pesquisaConfiguracaoFechamentoContabilizacao, "knockoutPesquisaConfiguracaoFechamentoContabilizacao", false, _pesquisaConfiguracaoFechamentoContabilizacao.Pesquisar.id);

    LoadGridConfiguracaoFechamentoContabilizacao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function AdicionarClick(e, sender) {
    Salvar(_configuracaoFechamentoContabilizacao, "ConfiguracaoFechamentoContabilizacao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridConfiguracaoFechamentoContabilizacao();
                LimparCamposConfiguracaoFechamentoContabilizacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_configuracaoFechamentoContabilizacao, "ConfiguracaoFechamentoContabilizacao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridConfiguracaoFechamentoContabilizacao();
                LimparCamposConfiguracaoFechamentoContabilizacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposConfiguracaoFechamentoContabilizacao();
}

function EditarClick(registroSelecionado) {
    LimparCamposConfiguracaoFechamentoContabilizacao();

    _configuracaoFechamentoContabilizacao.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_configuracaoFechamentoContabilizacao, "ConfiguracaoFechamentoContabilizacao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaConfiguracaoFechamentoContabilizacao.ExibirFiltros.visibleFade(false);

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
    exibirConfirmacao("Confirmação", "Deseja realmente excluir este registro?", function () {
        ExcluirPorCodigo(_configuracaoFechamentoContabilizacao, "ConfiguracaoFechamentoContabilizacao/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    RecarregarGridConfiguracaoFechamentoContabilizacao();
                    LimparCamposConfiguracaoFechamentoContabilizacao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

/*
 * Declaração das Funções
 */

function ControlarBotoesHabilitados(isEdicao) {
    _CRUDConfiguracaoFechamentoContabilizacao.Atualizar.visible(isEdicao);
    _CRUDConfiguracaoFechamentoContabilizacao.Excluir.visible(isEdicao);
    _CRUDConfiguracaoFechamentoContabilizacao.Cancelar.visible(isEdicao);
    _CRUDConfiguracaoFechamentoContabilizacao.Adicionar.visible(!isEdicao);
}

function LimparCamposConfiguracaoFechamentoContabilizacao() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_configuracaoFechamentoContabilizacao);
}

function RecarregarGridConfiguracaoFechamentoContabilizacao() {
    _gridConfiguracaoFechamentoContabilizacao.CarregarGrid();
}