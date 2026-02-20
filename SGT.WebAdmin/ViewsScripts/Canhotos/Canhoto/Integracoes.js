/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="Canhoto.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaIntegracaoCanhoto;
var _gridIntegracaoCanhoto;
var _buscandoApenasNaoReconhecidos = true;
var _atualizarAoFechar = false;
var _dataGridCarregada = [];
var _indexCanhotoAberto = -1;
var _modalHistoricoIntegracao;

var _situacaoIntegracaoCanhoto = [{ value: "", text: "Todas" },
{ value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: "Aguardando Integração" },
{ value: EnumSituacaoIntegracaoCarga.AgRetorno, text: "Aguardando Retorno" },
{ value: EnumSituacaoIntegracaoCarga.Integrado, text: "Integrado" },
{ value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: "Falha na Integração" }];

var _pesquisaHistoricoIntegracao;

var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}
var PesquisaIntegracaoCanhoto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCanhoto = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoCanhoto, text: "Situação: " });
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
function loadCanhotoIntegracao(e) {
    _pesquisaIntegracaoCanhoto = new PesquisaIntegracaoCanhoto();
    KoBindings(_pesquisaIntegracaoCanhoto, "knockoutPesquisaCanhoto", false, _pesquisaIntegracaoCanhoto.Pesquisar.id);

    _pesquisaIntegracaoCanhoto.CodigoCanhoto.val(e.Codigo);
    BuscarIntegracaoCanhoto();

    _modalHistoricoIntegracao = new bootstrap.Modal(document.getElementById("divModalHistoricoIntegracao"), { backdrop: true, keyboard: true });
}


//*******MÉTODOS*******
function BuscarIntegracaoCanhoto() {
    var linhasPorPaginas = 5;

    var auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("CanhotoIntegracao"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Canhotos.Canhoto.Opcoes, tamanho: 7, opcoes: [auditar] };
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Canhotos.Canhoto.Reenviar, id: guid(), metodo: ReenviarIntegracao, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Canhotos.Canhoto.HistoricoIntegracoes, id: guid(), metodo: ExibirHistoricoIntegracao, tamanho: "20", icone: "" });


    _gridIntegracaoCanhoto = new GridView(_pesquisaIntegracaoCanhoto.Pesquisar.idGrid, "CanhotoIntegracao/Pesquisa", _pesquisaIntegracaoCanhoto, menuOpcoes, null, linhasPorPaginas);

    RecarregarGrid();
}
function IntegracoesClick(e) {
    Global.abrirModal("divModalIntegracoes");
    loadCanhotoIntegracao(e);
}

function ReenviarIntegracao(data) {
    executarReST("CanhotoIntegracao/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.ReenvioSolicitadoComSucesso);
            _gridIntegracaoCanhoto.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
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

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "CanhotoIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracao(historicoConsulta) {
    executarDownload("CanhotoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function GridCarregada(data) {
    if (_buscandoApenasNaoReconhecidos) {
        _dataGridCarregada = data.data;
    }
}

function RecarregarGrid(cb) {
    _gridIntegracaoCanhoto.CarregarGrid(function (data) {
        GridCarregada(data);

        if (cb != null) cb();
    });
}