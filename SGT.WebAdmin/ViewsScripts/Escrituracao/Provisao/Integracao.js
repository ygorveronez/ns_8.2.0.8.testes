/// <reference path="Provisao.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoProvisao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _integracaoCarga;
var _historicoIntegracaoCarga;
var _gridIntegracaoCarga;
var _gridHistoricoIntegracaoCarga;

var _situacaoIntegracaoCarga = [
    { value: "", text: "Todas" },
    { value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: "Aguardando Integração" },
    { value: EnumSituacaoIntegracaoCarga.Integrado, text: "Integrado" },
    { value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: "Falha na Integração" }
];

var IntegracaoCarga = function () {
    this.Provisao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoCarga, text: "Situação:", def: "", issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoCarga();
            _gridIntegracaoCarga.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosIntegracaoCarga();
        }, type: types.event, text: "Reenviar Todos", idGrid: guid(), visible: ko.observable(true)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoCarga();
        }, type: types.event, text: "Obter Totais", idGrid: guid(), visible: ko.observable(true)
    });
}

var HistoricoIntegracaoCarga = function () {
    this.Integracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ });
}

//*******EVENTOS*******
function loadIntegracaoCarga() {
    _integracaoCarga = new IntegracaoCarga();
    KoBindings(_integracaoCarga, "knockoutIntegracaoCarga");

    _historicoIntegracaoCarga = new HistoricoIntegracaoCarga();
    KoBindings(_historicoIntegracaoCarga, "knockoutHistoricoIntegracaoCarga");
    CarregarHistoricoIntegracaoCarga();
}

function DownloadIntegracaoCarga(data) {
    executarDownload("ProvisaoIntegracao/Download", { Codigo: data.Codigo });
}

function ReenviarIntegracaoCarga(data) {
    executarReST("ProvisaoIntegracao/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
            _gridIntegracaoCarga.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function HistoricoIntegracaoCargaClick(data) {
    _historicoIntegracaoCarga.Integracao.val(data.Codigo);
    _gridHistoricoIntegracaoCarga.CarregarGrid(function () {
        Global.abrirModal('divModalHistoricoIntegracao');
    });
}

function DownloadArquivosHistoricoIntegracaoProvisao(historicoConsulta) {
    executarDownload("ProvisaoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo, Integracao: _historicoIntegracaoCarga.Integracao.val() });
}

//*******MÉTODOS*******
function CarregaIntegracaoCarga() {
    _integracaoCarga.Provisao.val(_provisao.Codigo.val());
    ObterTotaisIntegracaoCarga();
    ConfigurarPesquisaIntegracaoCarga();
}

function ObterTotaisIntegracaoCarga() {
    executarReST("ProvisaoIntegracao/ObterTotais", { Provisao: _integracaoCarga.Provisao.val() }, function (r) {
        if (r.Success) {
            _integracaoCarga.TotalGeral.val(r.Data.TotalGeral);
            _integracaoCarga.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoCarga.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoCarga.TotalIntegrado.val(r.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ReenviarTodosIntegracaoCarga() {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todas as integrações?", function () {
        executarReST("ProvisaoIntegracao/ReenviarTodos", { Provisao: _integracaoCarga.Provisao.val(), Situacao: _integracaoCarga.Situacao.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                _gridIntegracaoCarga.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function CarregarHistoricoIntegracaoCarga() {
    var editar = { descricao: "Download Arquivos", id: "clasEditar", evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoProvisao, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridHistoricoIntegracaoCarga = new GridView(_historicoIntegracaoCarga.Grid.id, "ProvisaoIntegracao/PesquisaHistorico", _historicoIntegracaoCarga, menuOpcoes);
}

function ConfigurarPesquisaIntegracaoCarga() {
    var historico = { descricao: "Histórico", id: guid(), metodo: HistoricoIntegracaoCargaClick, tamanho: "20", icone: "" };
    var download = { descricao: "Download", id: guid(), metodo: DownloadIntegracaoCarga, tamanho: "20", icone: "" };
    var reenviar = { descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracaoCarga, tamanho: "20", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [historico, download, reenviar] };

    _gridIntegracaoCarga = new GridView(_integracaoCarga.Pesquisar.idGrid, "ProvisaoIntegracao/Pesquisa", _integracaoCarga, menuOpcoes);
    _gridIntegracaoCarga.CarregarGrid();
}
