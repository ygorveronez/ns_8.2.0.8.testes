//#region Variaveis Globais
var _gridIntegracaoGhost;
var _pesquisaIntegracaoGhost;
var _pesquisaHistoricoIntegracao;
var _gridHistoricoIntegracao;


//#endregion


//#region Funções Constructoras

function PesquisaIntegracaoGhost() {
    this.SituacaoIntegracao = PropertyEntity({ text: "Situação:", getType: typesKnockout.selectMultiple, val: ko.observable(""), options: EnumSituacaoIntegracao.obterOpcoes(false), def: "", visible: ko.observable(true) });
    this.TipoDestino = PropertyEntity({ text: "Tipo Destino:", getType: typesKnockout.selectMultiple, val: ko.observable(""), options: EnumTipoDestinoGhost.obterOpcoes(), def: "", visible: ko.observable(true) });
    this.Chave = PropertyEntity({ text: "Driver Ticket:", getType: typesKnockout.string, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataIntegracaoInicial = PropertyEntity({ text: ko.observable("Data Integração Inicial: "), val: ko.observable(''), def: '', getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataIntegracaoFinal = PropertyEntity({ text: ko.observable("Data Integração Final: "), val: ko.observable(''), def: '', getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataIntegracaoInicial.dateRangeLimit = this.DataIntegracaoFinal;
    this.DataIntegracaoFinal.dateRangeInit = this.DataIntegracaoInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridIntegracaoGhost.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            let visible = e.ExibirFiltros.visibleFade() == true;
            e.ExibirFiltros.visibleFade(!visible);
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

function LoadIntegracaoGhost() {
    _pesquisaIntegracaoGhost = new PesquisaIntegracaoGhost();
    KoBindings(_pesquisaIntegracaoGhost, "knockoutPesquisaIntegracaoGhost");

    BuscarIntegracoes();
}

//#endregion

//#region Funções Auxiliares

function BuscarIntegracoes() {
    const reenviar = { descricao: "Reenviar", id: guid(), evento: "onclick", metodo: ReenviarIntegracao, tamanho: "15", icone: "" };
    const baixar = { descricao: "Arquivos Integração", id: guid(), evento: "onclick", metodo: ExibirHistoricoIntegracao, tamanho: "15", icone: "" };

    const menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(reenviar);
    menuOpcoes.opcoes.push(baixar);

    _gridIntegracaoGhost = new GridView(_pesquisaIntegracaoGhost.Pesquisar.idGrid, "IntegracaoGhost/Pesquisar", _pesquisaIntegracaoGhost, menuOpcoes, null);
    _gridIntegracaoGhost.CarregarGrid();
}

function ReenviarIntegracao(data) {
    executarReST("IntegracaoGhost/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
            _gridIntegracaoGhost.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}
function DownloadArquivosHistoricoIntegracao(data) {
    executarDownload("IntegracaoGhost/DownloadArquivosHistoricoIntegracao", { Codigo: data.Codigo });
}
function DownloadArquivosIntegracao(data) {
    executarDownload("IntegracaoGhost/DownloadArquivosIntegracao", { Codigo: data.Codigo });
}
const PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    let download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracao, tamanho: "20", icone: "" };

    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "IntegracaoGhost/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function ExibirHistoricoIntegracao(integracao) {
    BuscarHistoricoIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracao");
}

//#endregion