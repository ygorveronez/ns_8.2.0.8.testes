/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaEntregaIntegracao;
var _gridEntregaIntegracao;
var _buscandoApenasNaoReconhecidos = true;
var _atualizarAoFechar = false;
var _dataGridCarregada = [];
var _indexCarregamentoAberto = -1;

var _situacaoIntegracaoCarregamento = [{ value: "", text: Localization.Resources.Gerais.Geral.Todas },
    { value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao },
    { value: EnumSituacaoIntegracaoCarga.AgRetorno, text: Localization.Resources.Gerais.Geral.AguardandoRetorno },
    { value: EnumSituacaoIntegracaoCarga.Integrado, text: Localization.Resources.Gerais.Geral.Integrado },
    { value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: Localization.Resources.Gerais.Geral.FalhaNaIntegracao }];

var _pesquisaHistoricoIntegracao;

var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var PesquisaCarregamento = function () {
    var dataAtual = moment().add(-2, 'days').format("DD/MM/YYYY");
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.NumeroCarregamento = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.MontagemCarga.NumeroCarregamento.getFieldDescription() });
    this.Protocolo = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.MontagemCarga.Protocolo.getFieldDescription() });
    this.Carga = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.MontagemCarga.Carga.getFieldDescription() });
    this.NumeroExp = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.MontagemCarga.NumeroEXP.getFieldDescription() });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoCarregamento, text: Localization.Resources.Cargas.MontagemCarga.Situacao.getFieldDescription() });
    this.Emitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCarga.Emitente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCarga.Destinatario.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            RecarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

//*******EVENTOS*******

function loadMontagemCargaCarregamentoIntegracao() {
    _pesquisaEntregaIntegracao = new PesquisaCarregamento();
    KoBindings(_pesquisaEntregaIntegracao, "knockoutPesquisaCarregamento", false, _pesquisaEntregaIntegracao.Pesquisar.id);

    new BuscarClientes(_pesquisaEntregaIntegracao.Emitente);
    new BuscarClientes(_pesquisaEntregaIntegracao.Destinatario);

    BuscarIntegracoesCarregamento();
}

//*******MÉTODOS*******

function BuscarIntegracoesCarregamento() {
    var auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("MontagemCargaCarregamentoIntegracao"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [auditar] };
    menuOpcoes.opcoes.push({ descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracao, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: "Histórico de Integração", id: guid(), metodo: ExibirHistoricoIntegracao, tamanho: "20", icone: "" });

    var configExportacao = {
        url: "MontagemCargaCarregamentoIntegracao/ExportarPesquisa",
        titulo: Localization.Resources.Cargas.MontagemCarga.IntegracoesCarregamento
    };

    _gridEntregaIntegracao = new GridViewExportacao(_pesquisaEntregaIntegracao.Pesquisar.idGrid, "MontagemCargaCarregamentoIntegracao/Pesquisa", _pesquisaEntregaIntegracao, menuOpcoes, configExportacao, null, 10);
    RecarregarGrid();
}

function ReenviarIntegracao(data) {
    executarReST("MontagemCargaCarregamentoIntegracao/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
            _gridEntregaIntegracao.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ExibirHistoricoIntegracao(integracao) {
    BuscarHistoricoIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracao");
}

function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracao, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "MontagemCargaCarregamentoIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracao(historicoConsulta) {
    executarDownload("MontagemCargaCarregamentoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function GridCarregada(data) {
    if (_buscandoApenasNaoReconhecidos) {
        _dataGridCarregada = data.data;
    }
}

function RecarregarGrid(cb) {
    _gridEntregaIntegracao.CarregarGrid(function (data) {
        GridCarregada(data);

        if (cb != null) cb();
    });
}