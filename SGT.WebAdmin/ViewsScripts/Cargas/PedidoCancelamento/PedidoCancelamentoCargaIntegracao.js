var _pesquisaPedido;
var _gridPedido;
var _buscandoApenasNaoReconhecidos = true;
var _atualizarAoFechar = false;
var _dataGridCarregada = [];
var _indexCarregamentoAberto = -1;

var _situacaoIntegracaoCarregamento = [{ value: "", text: "Todas" },
{ value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: "Aguardando Integração" },
{ value: EnumSituacaoIntegracaoCarga.AgRetorno, text: "Aguardando Retorno" },
{ value: EnumSituacaoIntegracaoCarga.Integrado, text: "Integrado" },
{ value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: "Falha na Integração" }];

var _pesquisaHistoricoIntegracao;

var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var PesquisaPedido = function () {
    var dataAtual = moment().add(-2, 'days').format("DD/MM/YYYY");
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.NumeroPedido = PropertyEntity({ val: ko.observable(""), text: "Número Pedido: " });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoCarregamento, text: "Situação: " });
    this.Emitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Emitente:", idBtnSearch: guid(), visible: ko.observable(true) });
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
function loadPedidoCancelamentoReservaIntegracao() {
    _pesquisaPedido = new PesquisaPedido();
    KoBindings(_pesquisaPedido, "knockoutPesquisaPedido", false, _pesquisaPedido.Pesquisar.id);

    new BuscarClientes(_pesquisaPedido.Emitente);
    BuscarIntegracoesPedido();
}

function BuscarIntegracoesPedido() {
    var auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("PedidoCancelamentoReservaIntegracao"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [auditar] };
    menuOpcoes.opcoes.push({ descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracao, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: "Histórico de Integração", id: guid(), metodo: ExibirHistoricoIntegracao, tamanho: "20", icone: "" });

    _gridPedido = new GridView(_pesquisaPedido.Pesquisar.idGrid, "PedidoCancelamentoReservaIntegracao/Pesquisa", _pesquisaPedido, menuOpcoes, null, 10);
    RecarregarGrid();
}

function ReenviarIntegracao(data) {
    executarReST("PedidoCancelamentoReservaIntegracao/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
            _gridPedido.CarregarGrid();
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

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "PedidoCancelamentoReservaIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracao(historicoConsulta) {
    executarDownload("PedidoCancelamentoReservaIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function GridCarregada(data) {
    if (_buscandoApenasNaoReconhecidos) {
        _dataGridCarregada = data.data;
    }
}

function RecarregarGrid(cb) {
    _gridPedido.CarregarGrid(function (data) {
        GridCarregada(data);

        if (cb != null) cb();
    });
}
