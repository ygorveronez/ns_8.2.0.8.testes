/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumTipoLancamentoNotificacaoMobile.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDFilaCarregamentoNotificacao;
var _filaCarregamentoNotificacao;
var _pesquisaFilaCarregamentoNotificacao;
var _pesquisaFilaCarregamentoNotificacaoMotorista;
var _gridFilaCarregamentoNotificacao;
var _gridFilaCarregamentoNotificacaoMotorista;

/*
 * Declaração das Classes
 */

var CRUDFilaCarregamentoNotificacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Limpar / Nova" });
}

var FilaCarregamentoNotificacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Assunto = PropertyEntity({ text: "*Assunto:", getType: typesKnockout.string, required: true, enable: ko.observable(true) });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Mensagem = PropertyEntity({ text: "*Mensagem:", getType: typesKnockout.string, maxlength: 500, required: true, enable: ko.observable(true) });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.multiplesEntities, multiplesEntitiesConfig: { propDescricao: "Nome", propCodigo: "Codigo" }, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador", idBtnSearch: guid(), enable: ko.observable(true) });

    this.CentroCarregamento.codEntity.subscribe(limparCampoMotorista);
    this.ModeloVeicularCarga.codEntity.subscribe(limparCampoMotorista);
    this.Transportador.codEntity.subscribe(limparCampoMotorista);
}

var PesquisaFilaCarregamentoNotificacao = function () {
    this.Assunto = PropertyEntity({ text: "Assunto:", getType: typesKnockout.string });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento:", idBtnSearch: guid() });
    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoLancamentoNotificacaoMobile.Manual), options: EnumTipoLancamentoNotificacaoMobile.obterOpcoesPesquisa(), def: EnumTipoLancamentoNotificacaoMobile.Manual, text: "Tipo: " });

    this.CentroCarregamento.codEntity.subscribe(limparCampoMotoristaPesquisa);
    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false)
    });

    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridFilaCarregamentoNotificacao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var PesquisaFilaCarregamentoNotificacaoMotorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadFilaCarregamentoNotificacao() {
    _filaCarregamentoNotificacao = new FilaCarregamentoNotificacao();
    KoBindings(_filaCarregamentoNotificacao, "knockoutCadastroNotificacao");

    _CRUDFilaCarregamentoNotificacao = new CRUDFilaCarregamentoNotificacao();
    KoBindings(_CRUDFilaCarregamentoNotificacao, "knockoutCRUDNotificacao");

    _pesquisaFilaCarregamentoNotificacao = new PesquisaFilaCarregamentoNotificacao();
    KoBindings(_pesquisaFilaCarregamentoNotificacao, "knockoutPesquisaNotificacao", false, _pesquisaFilaCarregamentoNotificacao.Pesquisar.id);

    _pesquisaFilaCarregamentoNotificacaoMotorista = new PesquisaFilaCarregamentoNotificacaoMotorista();

    new BuscarCentrosCarregamento(_pesquisaFilaCarregamentoNotificacao.CentroCarregamento);
    new BuscarMotoristasMobile(_pesquisaFilaCarregamentoNotificacao.Motorista, null, _pesquisaFilaCarregamentoNotificacao.CentroCarregamento);

    new BuscarCentrosCarregamento(_filaCarregamentoNotificacao.CentroCarregamento);
    new BuscarModelosVeicularesCarga(_filaCarregamentoNotificacao.ModeloVeicularCarga);
    new BuscarTransportadores(_filaCarregamentoNotificacao.Transportador);
    new BuscarMotoristasMobile(_filaCarregamentoNotificacao.Motorista, null, _filaCarregamentoNotificacao.CentroCarregamento, _filaCarregamentoNotificacao.ModeloVeicularCarga, _filaCarregamentoNotificacao.Transportador);

    loadGridFilaCarregamentoNotificacao();
    loadGridFilaCarregamentoNotificacaoMotorista();
}

function loadGridFilaCarregamentoNotificacao() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "FilaCarregamentoNotificacao/ExportarPesquisa", titulo: "Notificações" };

    _gridFilaCarregamentoNotificacao = new GridViewExportacao(_pesquisaFilaCarregamentoNotificacao.Pesquisar.idGrid, "FilaCarregamentoNotificacao/Pesquisa", _pesquisaFilaCarregamentoNotificacao, menuOpcoes, configuracoesExportacao);
    _gridFilaCarregamentoNotificacao.CarregarGrid();
}

function loadGridFilaCarregamentoNotificacaoMotorista() {
    _gridFilaCarregamentoNotificacaoMotorista = new GridView("grid-notificacao-motorista", "FilaCarregamentoNotificacao/BuscarMotoristas", _pesquisaFilaCarregamentoNotificacaoMotorista);
    _gridFilaCarregamentoNotificacaoMotorista.CarregarGrid();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_filaCarregamentoNotificacao, "FilaCarregamentoNotificacao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Notificação cadastrada com sucesso.");

                recarregarGridFilaCarregamentoNotificacao();
                limparCamposFilaCarregamentoNotificacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposFilaCarregamentoNotificacao();
}

function editarClick(notificacaoSelecionada) {
    executarReST("FilaCarregamentoNotificacao/BuscarPorCodigo", { Codigo: notificacaoSelecionada.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_filaCarregamentoNotificacao, retorno);

                _pesquisaFilaCarregamentoNotificacao.ExibirFiltros.visibleFade(false);

                setarFocoAbaNotificacao();
                controlarComponentesHabilitados();
                exibirAbaMotoristas();
                recarregarGridFilaCarregamentoNotificacaoMotorista();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados(isNovaNotificacao) {
    _CRUDFilaCarregamentoNotificacao.Adicionar.visible(isNovaNotificacao);
}

function controlarCamposNotificacaoHabilitados(isNovaNotificacao) {
    _filaCarregamentoNotificacao.Assunto.enable(isNovaNotificacao);
    _filaCarregamentoNotificacao.CentroCarregamento.enable(isNovaNotificacao);
    _filaCarregamentoNotificacao.Mensagem.enable(isNovaNotificacao);
}

function controlarComponentesHabilitados() {
    var isNovaNotificacao = _filaCarregamentoNotificacao.Codigo.val() == 0;

    controlarCamposNotificacaoHabilitados(isNovaNotificacao);
    controlarBotoesHabilitados(isNovaNotificacao);
}

function exibirAbaMotoristas() {
    $("#aba-motorista").show();
}

function limparCampoMotorista() {
    _filaCarregamentoNotificacao.Motorista.multiplesEntities([]);
}

function limparCampoMotoristaPesquisa() {
    LimparCampoEntity(_pesquisaFilaCarregamentoNotificacao.Motorista); 
}

function limparCamposFilaCarregamentoNotificacao() {
    LimparCampos(_filaCarregamentoNotificacao);
    setarFocoAbaNotificacao();
    controlarComponentesHabilitados();
    ocultarAbaMotoristas();
    recarregarGridFilaCarregamentoNotificacaoMotorista();
}

function ocultarAbaMotoristas() {
    $("#aba-motorista").hide();
}

function recarregarGridFilaCarregamentoNotificacao() {
    _gridFilaCarregamentoNotificacao.CarregarGrid();
}

function recarregarGridFilaCarregamentoNotificacaoMotorista() {
    _pesquisaFilaCarregamentoNotificacaoMotorista.Codigo.val(_filaCarregamentoNotificacao.Codigo.val());

    _gridFilaCarregamentoNotificacaoMotorista.CarregarGrid();
}

function setarFocoAbaNotificacao() {
    $("#tabCadastroNotificacao").click();
}
