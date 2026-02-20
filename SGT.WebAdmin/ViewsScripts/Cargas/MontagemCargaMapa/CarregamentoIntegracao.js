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

var _pesquisaCarregamento;
var _gridCarregamento;
var _buscandoApenasNaoReconhecidos = true;
var _atualizarAoFechar = false;
var _dataGridCarregada = [];
var _indexCarregamentoAberto = -1;

var _pesquisaHistoricoIntegracao;

var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}


var PesquisaCarregamento = function () {
    var dataAtual = moment().add(-2, 'days').format("DD/MM/YYYY");
    this.DataInicio = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.DataInicial.getFieldDescription(), val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.DataFinal.getFieldDescription(), getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.NumeroCarregamento = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroCarregamentoSemAbreviacao.getFieldDescription() });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: Localization.Resources.Cargas.MontagemCargaMapa.Situacao.getFieldDescription() });
    this.Emitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Emitente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            RecarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadMontagemCargaCarregamentoIntegracao() {
    _pesquisaCarregamento = new PesquisaCarregamento();
    KoBindings(_pesquisaCarregamento, "knockoutPesquisaCarregamento", false, _pesquisaCarregamento.Pesquisar.id);

    new BuscarClientes(_pesquisaCarregamento.Emitente);
    BuscarIntegracoesCarregamento();
}


//*******MÉTODOS*******
function BuscarIntegracoesCarregamento() {
    var auditar = { descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), metodo: OpcaoAuditoria("MontagemCargaCarregamentoIntegracao"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [auditar] };
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Gerais.Geral.Reenviar, id: guid(), metodo: ReenviarIntegracao, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Gerais.Geral.HistoricoIntegracao, id: guid(), metodo: ExibirHistoricoIntegracao, tamanho: "20", icone: "" });

    _gridCarregamento = new GridView(_pesquisaCarregamento.Pesquisar.idGrid, "MontagemCargaCarregamentoIntegracao/Pesquisa", _pesquisaCarregamento, menuOpcoes, null, 10);
    RecarregarGrid();
}

function ReenviarIntegracao(data) {
    executarReST("MontagemCargaCarregamentoIntegracao/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ReenvioSolicitadoComSucesso);
            _gridCarregamento.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
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

    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracao, tamanho: "20", icone: "" };

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
    _gridCarregamento.CarregarGrid(function (data) {
        GridCarregada(data);

        if (cb != null) cb();
    });
}