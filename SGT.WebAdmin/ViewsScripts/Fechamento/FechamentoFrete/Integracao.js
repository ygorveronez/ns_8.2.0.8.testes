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

var _integracaoFechamento;
var _historicoIntegracaoFechamento;
var _gridIntegracaoFechamento;
var _gridHistoricoIntegracaoFechamento;
var _modalHistoricoIntegracao;

var _situacaoIntegracaoFechamento = [
    { value: "", text: "Todas" },
    { value: EnumSituacaoIntegracao.AgIntegracao, text: "Aguardando Integração" },
    { value: EnumSituacaoIntegracao.Integrado, text: "Integrado" },
    { value: EnumSituacaoIntegracao.ProblemaIntegracao, text: "Falha na Integração" }
];

var IntegracaoFechamento = function () {
    this.FechamentoFrete = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(true)  });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoFechamento, text: "Situação:", def: "", issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoFechamento();
            _gridIntegracaoFechamento.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosIntegracaoFechamento();
        }, type: types.event, text: "Reenviar Todos", idGrid: guid(), visible: ko.observable(true)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoFechamento();
        }, type: types.event, text: "Obter Totais", idGrid: guid(), visible: ko.observable(true)
    });
}

var HistoricoIntegracaoFechamento = function () {
    this.Integracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({});
}

//*******EVENTOS*******
function loadIntegracaoFechamento() {
    _integracaoFechamento = new IntegracaoFechamento();
    KoBindings(_integracaoFechamento, "knockoutIntegracaoFechamento");

    _historicoIntegracaoFechamento = new HistoricoIntegracaoFechamento();
    KoBindings(_historicoIntegracaoFechamento, "knockoutHistoricoIntegracaoFechamento");

    CarregarHistoricoIntegracaoFechamento();

    _modalHistoricoIntegracao = new bootstrap.Modal(document.getElementById("divModalHistoricoIntegracao"), { backdrop: 'static', keyboard: true });
}

function DownloadIntegracaoFechamento(data) {
    executarDownload("FechamentoFreteIntegracao/Download", { Codigo: data.Codigo });
}

function ReenviarIntegracaoFechamento(data) {
    executarReST("FechamentoFreteIntegracao/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
            _gridIntegracaoFechamento.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function HistoricoIntegracaoFechamentoClick(data) {
    _historicoIntegracaoFechamento.Integracao.val(data.Codigo);
    _gridHistoricoIntegracaoFechamento.CarregarGrid(function () {
        _modalHistoricoIntegracao.show();
    });
}
function DownloadArquivosHistoricoIntegracaoFechamentoFrete(historicoConsulta) {
    executarDownload("FechamentoFreteIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo, Integracao: _historicoIntegracaoFechamento.Integracao.val() });
}


//*******MÉTODOS*******
function CarregaIntegracaoFechamento() {
    _integracaoFechamento.FechamentoFrete.val(_fechamentoFrete.Codigo.val());
    ObterTotaisIntegracaoFechamento();
    ConfigurarPesquisaIntegracaoFechamento();
}

function ObterTotaisIntegracaoFechamento() {
    executarReST("FechamentoFreteIntegracao/ObterTotais", { FechamentoFrete: _integracaoFechamento.FechamentoFrete.val() }, function (r) {
        if (r.Success) {
            _integracaoFechamento.TotalGeral.val(r.Data.TotalGeral);
            _integracaoFechamento.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoFechamento.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoFechamento.TotalIntegrado.val(r.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ReenviarTodosIntegracaoFechamento() {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todas as integrações?", function () {
        executarReST("FechamentoFreteIntegracao/ReenviarTodos", { FechamentoFrete: _integracaoFechamento.FechamentoFrete.val(), Situacao: _integracaoFechamento.Situacao.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                _gridIntegracaoFechamento.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function CarregarHistoricoIntegracaoFechamento() {
    var editar = { descricao: "Download Arquivos", id: "clasEditar", evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoFechamentoFrete, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridHistoricoIntegracaoFechamento = new GridView(_historicoIntegracaoFechamento.Grid.id, "FechamentoFreteIntegracao/PesquisaHistorico", _historicoIntegracaoFechamento, menuOpcoes);
}

function ConfigurarPesquisaIntegracaoFechamento() {
    var historico = { descricao: "Histórico", id: guid(), metodo: HistoricoIntegracaoFechamentoClick, tamanho: "20", icone: "" };
    var download = { descricao: "Download", id: guid(), metodo: DownloadIntegracaoFechamento, tamanho: "20", icone: "" };
    var reenviar = { descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracaoFechamento, tamanho: "20", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [historico, reenviar, download] };

    _gridIntegracaoFechamento = new GridView(_integracaoFechamento.Pesquisar.idGrid, "FechamentoFreteIntegracao/Pesquisa", _integracaoFechamento, menuOpcoes);
    _gridIntegracaoFechamento.CarregarGrid();
}