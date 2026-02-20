/// <reference path="../../../js/libs/jquery-2.1.1.js" />
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


//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaEntregaIntegracao;
var _gridEntregaIntegracao;
var _buscandoApenasNaoReconhecidos = true;
var _atualizarAoFechar = false;
var _dataGridCarregada = [];
var _indexCarregamentoAberto = -1;
var _modalHistoricoIntegracao;



var _situacaoIntegracaoCarregamento = [{ value: "", text: "Todas" },
{ value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: "Aguardando Integração" },
{ value: EnumSituacaoIntegracaoCarga.AgRetorno, text: "Aguardando Retorno" },
{ value: EnumSituacaoIntegracaoCarga.Integrado, text: "Integrado" },
{ value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: "Falha na Integração" }];

var _pesquisaHistoricoIntegracao;

var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}


var PesquisaEntregaIntegracao = function () {
    var dataAtual = moment().add(-2, 'days').format("DD/MM/YYYY");
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.NumeroCarga = PropertyEntity({ val: ko.observable(""), text: "Número Carga: " });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoCarregamento, text: "Situação: " });
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
function loadCargaEntregaIntegracao() {
    _pesquisaEntregaIntegracao = new PesquisaEntregaIntegracao();
    KoBindings(_pesquisaEntregaIntegracao, "knockoutPesquisaEntregaIntegracao", false, _pesquisaEntregaIntegracao.Pesquisar.id);

    BuscarIntegracoesEntrega();

    _modalHistoricoIntegracao = new bootstrap.Modal(document.getElementById("divModalHistoricoIntegracao"), { backdrop: true, keyboard: true });
}




//*******MÉTODOS*******
function BuscarIntegracoesEntrega() {


    var auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("CargaEntregaIntegracao"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [auditar] };
    menuOpcoes.opcoes.push({ descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracao, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: "Histórico de Integração", id: guid(), metodo: ExibirHistoricoIntegracao, tamanho: "20", icone: "" });

    _gridEntregaIntegracao = new GridView(_pesquisaEntregaIntegracao.Pesquisar.idGrid, "CargaEntregaIntegracao/Pesquisa", _pesquisaEntregaIntegracao, menuOpcoes, null, 10);
    RecarregarGrid();
}

function ReenviarIntegracao(data) {
    executarReST("CargaEntregaIntegracao/Reenviar", { Codigo: data.Codigo }, function (r) {
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
    _modalHistoricoIntegracao.show();
}


function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracao, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "CargaEntregaIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracao(historicoConsulta) {
    executarDownload("CargaEntregaIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
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