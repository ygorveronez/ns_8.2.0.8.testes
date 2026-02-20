/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Porto.js" />
/// <reference path="../../Consultas/TipoTerminalImportacao.js" />
/// <reference path="../../Consultas/PedidoViagemNavio.js" />
/// <reference path="../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../Enumeradores/EnumTipoModal.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _statusCTeAutorizacaoAverbacaoLote = [
    { text: "Rejeição", value: 9 }
];

var _tipoModal = [
    { text: "Aquaviário", value: EnumTipoModal.Aquaviario },
    { text: "Multimodal", value: EnumTipoModal.Multimodal },
    { text: "Rodoviário", value: EnumTipoModal.Rodoviario }
];

var _gridAutorizacaoAverbacaoLote;
var _pesquisaAutorizacaoAverbacaoLote;

var PesquisaAutorizacaoAverbacaoLote = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial:", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final:", getType: typesKnockout.int });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });

    this.TipoModal = PropertyEntity({ text: "Tipo Modal:", val: ko.observable([]), options: _tipoModal, def: [], getType: typesKnockout.selectMultiple });
    this.StatusAverbacao = PropertyEntity({ text: "Status:", val: ko.observable([]), options: _statusCTeAutorizacaoAverbacaoLote, def: [], getType: typesKnockout.selectMultiple });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid() });
    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto de Origem:", idBtnSearch: guid() });
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terminal de Origem:", idBtnSearch: guid() });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terminal de Destino:", idBtnSearch: guid() });
    this.Viagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Viagem:", idBtnSearch: guid() });

    this.NumeroBooking = PropertyEntity({ text: "Número Booking:", getType: typesKnockout.string });
    this.NumeroOS = PropertyEntity({ text: "Número O.S.:", getType: typesKnockout.string });
    this.NumeroControle = PropertyEntity({ text: "Número Controle:", getType: typesKnockout.string });
    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Container:", idBtnSearch: guid() });
    this.NumeroNF = PropertyEntity({ text: "Número NF:", getType: typesKnockout.string });

    this.DataFinal.dateRangeInit = this.DataInicial;
    this.DataInicial.dateRangeLimit = this.DataFinal;

    this.SolicitarAutorizacaoAverbacaoLote = PropertyEntity({
        eventClick: SolicitarAutorizacaoAverbacaoLoteClick, type: types.event, text: "Reenviar solicitação de autorização", icon: ko.observable("fa fa-refresh"), idGrid: guid()
    });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAutorizacaoAverbacaoLote.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid()
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadAutorizacaoAverbacaoLote() {
    _pesquisaAutorizacaoAverbacaoLote = new PesquisaAutorizacaoAverbacaoLote();
    KoBindings(_pesquisaAutorizacaoAverbacaoLote, "knockoutPesquisaAutorizacaoAverbacaoLote", false, _pesquisaAutorizacaoAverbacaoLote.Pesquisar.id);

    new BuscarEmpresa(_pesquisaAutorizacaoAverbacaoLote.Empresa);
    new BuscarPorto(_pesquisaAutorizacaoAverbacaoLote.PortoOrigem);
    new BuscarTipoTerminalImportacao(_pesquisaAutorizacaoAverbacaoLote.TerminalOrigem);
    new BuscarTipoTerminalImportacao(_pesquisaAutorizacaoAverbacaoLote.TerminalDestino);
    new BuscarPedidoViagemNavio(_pesquisaAutorizacaoAverbacaoLote.Viagem);
    new BuscarContainers(_pesquisaAutorizacaoAverbacaoLote.Container);

    buscarAutorizacaoAverbacaoLotes();
}

function SolicitarAutorizacaoAverbacaoLoteClick() {
    Salvar(_pesquisaAutorizacaoAverbacaoLote, "AutorizacaoAverbacaoLote/SolicitarAutorizacao", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridAutorizacaoAverbacaoLote.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação de reenvio efetuada com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function buscarAutorizacaoAverbacaoLotes() {
    _gridAutorizacaoAverbacaoLote = new GridView(_pesquisaAutorizacaoAverbacaoLote.Pesquisar.idGrid, "AutorizacaoAverbacaoLote/Pesquisa", _pesquisaAutorizacaoAverbacaoLote, null, null, 10);
    _gridAutorizacaoAverbacaoLote.CarregarGrid();
}