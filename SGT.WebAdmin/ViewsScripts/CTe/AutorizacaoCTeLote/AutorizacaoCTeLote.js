/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Porto.js" />
/// <reference path="../../Consultas/TipoTerminalImportacao.js" />
/// <reference path="../../Consultas/PedidoViagemNavio.js" />
/// <reference path="../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../Enumeradores/EnumTipoModal.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _statusCTeAutorizacaoCTeLote = [
    { text: "Pendente", value: EnumStatusCTe.PENDENTE },
    { text: "Rejeição", value: EnumStatusCTe.REJEICAO },
    { text: "Em Digitação", value: EnumStatusCTe.EMDIGITACAO }
];

var _tipoModal = [
    { text: "Aquaviário", value: EnumTipoModal.Aquaviario },
    { text: "Multimodal", value: EnumTipoModal.Multimodal },
    { text: "Rodoviário", value: EnumTipoModal.Rodoviario }
];

var _gridAutorizacaoCTeLote;
var _pesquisaAutorizacaoCTeLote;

var PesquisaAutorizacaoCTeLote = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial:", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final:", getType: typesKnockout.int });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });

    //this.TipoModal = PropertyEntity({ val: ko.observable(EnumTipoModal.Todos), options: EnumTipoModal.obterOpcoesPesquisa(), def: EnumTipoModal.Todos, text: "Tipo Modal: " });
    this.TipoModal = PropertyEntity({ text: "Tipo Modal:", val: ko.observable([]), options: _tipoModal, def: [], getType: typesKnockout.selectMultiple });
    this.Status = PropertyEntity({ text: "Status:", val: ko.observable([]), options: _statusCTeAutorizacaoCTeLote, def: [], getType: typesKnockout.selectMultiple });

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

    this.SolicitarAutorizacaoCTeLote = PropertyEntity({
        eventClick: SolicitarAutorizacaoCTeLoteClick, type: types.event, text: "Reenviar solicitação de autorização", icon: ko.observable("fa fa-refresh"), idGrid: guid()
    });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAutorizacaoCTeLote.CarregarGrid();
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

function loadAutorizacaoCTeLote() {
    _pesquisaAutorizacaoCTeLote = new PesquisaAutorizacaoCTeLote();
    KoBindings(_pesquisaAutorizacaoCTeLote, "knockoutPesquisaAutorizacaoCTeLote", false, _pesquisaAutorizacaoCTeLote.Pesquisar.id);

    new BuscarEmpresa(_pesquisaAutorizacaoCTeLote.Empresa);
    new BuscarPorto(_pesquisaAutorizacaoCTeLote.PortoOrigem);
    new BuscarTipoTerminalImportacao(_pesquisaAutorizacaoCTeLote.TerminalOrigem);
    new BuscarTipoTerminalImportacao(_pesquisaAutorizacaoCTeLote.TerminalDestino);
    new BuscarPedidoViagemNavio(_pesquisaAutorizacaoCTeLote.Viagem);
    new BuscarContainers(_pesquisaAutorizacaoCTeLote.Container);

    buscarAutorizacaoCTeLotes();
}

function SolicitarAutorizacaoCTeLoteClick() {
    Salvar(_pesquisaAutorizacaoCTeLote, "AutorizacaoCTeLote/SolicitarAutorizacao", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridAutorizacaoCTeLote.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação de reenvio efetuada com sucesso");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function buscarAutorizacaoCTeLotes() {
    _gridAutorizacaoCTeLote = new GridView(_pesquisaAutorizacaoCTeLote.Pesquisar.idGrid, "AutorizacaoCTeLote/Pesquisa", _pesquisaAutorizacaoCTeLote, null, null, 10);
    _gridAutorizacaoCTeLote.CarregarGrid();
}