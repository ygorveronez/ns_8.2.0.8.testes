/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
// #region Objetos Globais do Arquivo

var _crudConfiguracaoIntegracaoTecnologiaMonitoramento;
var _gridConfiguracaoIntegracaoTecnologiaMonitoramento;
var _pesquisaConfiguracaoIntegracaoTecnologiaMonitoramento;
var _configuracaoIntegracaoTecnologiaMonitoramento;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CRUDConfiguracaoIntegracaoTecnologiaMonitoramento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
};

var PesquisaConfiguracaoIntegracaoTecnologiaMonitoramento = function () {
    this.Tipo = PropertyEntity({ val: ko.observable(""), options: EnumTipoIntegracao.obterOpcoesMonitoramentoPesquisa(), def: "", text: "Tipo:" });
    this.Habilitada = PropertyEntity({ val: ko.observable(""), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: "", text: "Habilitada:" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConfiguracaoIntegracaoTecnologiaMonitoramento.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var ConfiguracaoIntegracaoTecnologiaMonitoramento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.CodigoIntegracao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), maxlength: 300 });
    this.Tipo = PropertyEntity({ val: ko.observable(""), options: EnumTipoIntegracao.obterOpcoesMonitoramento(), def: "", text: Localization.Resources.Gerais.Geral.Tipo.getRequiredFieldDescription(), required: true });
    this.Habilitada = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: "Habilitada" });
    this.ProcessarSensores = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: "Processar Sensores" });
    this.TempoSleepThread = PropertyEntity({ getType: typesKnockout.int, text: "Tempo Sleep Thread".getRequiredFieldDescription(), required: true, configInt: { allowZero: true } });
    this.TempoSleepEntreContas = PropertyEntity({ getType: typesKnockout.int, text: "Tempo Sleep entre Contas".getRequiredFieldDescription(), required: true, configInt: { allowZero: true } });
    this.MinutosDiferencaMinimaEntrePosicoes = PropertyEntity({ getType: typesKnockout.int, text: "Minutos Diferença Mínima entre Posições".getRequiredFieldDescription(), required: true, configInt: { allowZero: true } });

    this.Contas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Opcoes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Monitorar = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
};

// #endregion Classes

// #region Funções de Inicialização

function loadConfiguracaoIntegracaoTecnologiaMonitoramento() {
    _configuracaoIntegracaoTecnologiaMonitoramento = new ConfiguracaoIntegracaoTecnologiaMonitoramento();
    KoBindings(_configuracaoIntegracaoTecnologiaMonitoramento, "knockoutDetalhes");

    _crudConfiguracaoIntegracaoTecnologiaMonitoramento = new CRUDConfiguracaoIntegracaoTecnologiaMonitoramento();
    KoBindings(_crudConfiguracaoIntegracaoTecnologiaMonitoramento, "knockoutCRUDConfiguracaoIntegracaoTecnologiaMonitoramento");

    _pesquisaConfiguracaoIntegracaoTecnologiaMonitoramento = new PesquisaConfiguracaoIntegracaoTecnologiaMonitoramento();
    KoBindings(_pesquisaConfiguracaoIntegracaoTecnologiaMonitoramento, "knockoutPesquisaConfiguracaoIntegracaoTecnologiaMonitoramento", false, _pesquisaConfiguracaoIntegracaoTecnologiaMonitoramento.Pesquisar.id);

    HeaderAuditoria("ConfiguracaoIntegracaoTecnologiaMonitoramento", _configuracaoIntegracaoTecnologiaMonitoramento);

    buscarConfiguracaoIntegracaoTecnologiaMonitoramentos();

    loadConfiguracaoIntegracaoTecnologiaMonitoramentoConta();
    loadConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar();
    loadConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick(e, sender) {
    Salvar(_configuracaoIntegracaoTecnologiaMonitoramento, "ConfiguracaoIntegracaoTecnologiaMonitoramento/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);

                if (typeof enviarArquivosAnexados === 'function') {
                    enviarArquivosAnexados(retorno.Data.Codigo);
                }

                _gridConfiguracaoIntegracaoTecnologiaMonitoramento.CarregarGrid();
                limparCamposConfiguracaoIntegracaoTecnologiaMonitoramento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function atualizarClick(e, sender) {
    Salvar(_configuracaoIntegracaoTecnologiaMonitoramento, "ConfiguracaoIntegracaoTecnologiaMonitoramento/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                _gridConfiguracaoIntegracaoTecnologiaMonitoramento.CarregarGrid();
                limparCamposConfiguracaoIntegracaoTecnologiaMonitoramento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function cancelarClick(e) {
    limparCamposConfiguracaoIntegracaoTecnologiaMonitoramento();
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.RealmenteDesejaExcluirORegistro, function () {
        ExcluirPorCodigo(_configuracaoIntegracaoTecnologiaMonitoramento, "ConfiguracaoIntegracaoTecnologiaMonitoramento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridConfiguracaoIntegracaoTecnologiaMonitoramento.CarregarGrid();
                    limparCamposConfiguracaoIntegracaoTecnologiaMonitoramento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function buscarConfiguracaoIntegracaoTecnologiaMonitoramentos() {
    let editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarConfiguracaoIntegracaoTecnologiaMonitoramento, tamanho: "15", icone: "" };
    let menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: "10", opcoes: [editar] };
    let configuracoesExportacao = { url: "ConfiguracaoIntegracaoTecnologiaMonitoramento/ExportarPesquisa", titulo: "Configuracoes-Integracao-Tecnologia-Monitoramento" };

    _gridConfiguracaoIntegracaoTecnologiaMonitoramento = new GridViewExportacao(_pesquisaConfiguracaoIntegracaoTecnologiaMonitoramento.Pesquisar.idGrid, "ConfiguracaoIntegracaoTecnologiaMonitoramento/Pesquisa", _pesquisaConfiguracaoIntegracaoTecnologiaMonitoramento, menuOpcoes, configuracoesExportacao);
    _gridConfiguracaoIntegracaoTecnologiaMonitoramento.CarregarGrid();
}

function editarConfiguracaoIntegracaoTecnologiaMonitoramento(objetoGrid) {
    limparCamposConfiguracaoIntegracaoTecnologiaMonitoramento();

    _configuracaoIntegracaoTecnologiaMonitoramento.Codigo.val(objetoGrid.Codigo);

    BuscarPorCodigo(_configuracaoIntegracaoTecnologiaMonitoramento, "ConfiguracaoIntegracaoTecnologiaMonitoramento/BuscarPorCodigo", function (retorno) {

        _pesquisaConfiguracaoIntegracaoTecnologiaMonitoramento.ExibirFiltros.visibleFade(false);

        _crudConfiguracaoIntegracaoTecnologiaMonitoramento.Atualizar.visible(true);
        _crudConfiguracaoIntegracaoTecnologiaMonitoramento.Cancelar.visible(true);
        _crudConfiguracaoIntegracaoTecnologiaMonitoramento.Excluir.visible(true);
        _crudConfiguracaoIntegracaoTecnologiaMonitoramento.Adicionar.visible(false);

        RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoConta();
        RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar();
        RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao();

    }, null);
}

function limparCamposConfiguracaoIntegracaoTecnologiaMonitoramento() {
    _crudConfiguracaoIntegracaoTecnologiaMonitoramento.Atualizar.visible(false);
    _crudConfiguracaoIntegracaoTecnologiaMonitoramento.Cancelar.visible(true);
    _crudConfiguracaoIntegracaoTecnologiaMonitoramento.Excluir.visible(false);
    _crudConfiguracaoIntegracaoTecnologiaMonitoramento.Adicionar.visible(true);

    LimparCampos(_configuracaoIntegracaoTecnologiaMonitoramento);

    LimparCamposConfiguracaoIntegracaoTecnologiaMonitoramentoConta();
    LimparCamposConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar();
    LimparCamposConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao();

    Global.ResetarAbas();

    RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoConta();
    RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar();
    RecarregarGridConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao();
}

// #endregion Funções Privadas