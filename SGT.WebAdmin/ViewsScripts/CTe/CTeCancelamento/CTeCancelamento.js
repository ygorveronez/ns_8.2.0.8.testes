//*******MAPEAMENTO KNOUCKOUT*******

var _statusCTe = [
    { text: "Todos", value: "" },
    { text: "Autorizado", value: EnumStatusCTe.AUTORIZADO },
    { text: "Cancelado", value: EnumStatusCTe.CANCELADO },
    { text: "Anulado", value: EnumStatusCTe.ANULADO }
];

var _gridConsultaCTe;
var _pesquisaCTe;
var _cancelamentoCTe;
var _modalCancelamentoCTe;

var CancelamentoCTe = function () {
    this.Justificativa = PropertyEntity({ text: "*Justificativa:", required: true });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Cancelar = PropertyEntity({
        eventClick: function (e) {
            CancelarClick();
        }, type: types.event, text: "Cancelar", idGrid: guid(), icon: "fa fa-chevron-down"
    });
}

var PesquisaCTe = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial:", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final:", getType: typesKnockout.int });
    this.DataEmissaoInicial = PropertyEntity({ text: "Data de Emissão Inicial:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data de Emissão Final:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid() });
    this.Status = PropertyEntity({ text: "Situação:", val: ko.observable(""), options: _statusCTe, def: "" });

    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;
    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConsultaCTe.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid()
    });
}

//*******EVENTOS*******

function LoadCTeCancelamento() {
    _pesquisaCTe = new PesquisaCTe();
    KoBindings(_pesquisaCTe, "knockoutCTeCancelamento", false, _pesquisaCTe.Pesquisar.id);

    _cancelamentoCTe = new CancelamentoCTe();
    KoBindings(_cancelamentoCTe, "divModalCancelamentoCTe");

    new BuscarClientes(_pesquisaCTe.Remetente);
    new BuscarClientes(_pesquisaCTe.Destinatario);
    new BuscarLocalidadesBrasil(_pesquisaCTe.Origem);
    new BuscarLocalidadesBrasil(_pesquisaCTe.Destino);

    BuscarCTes();
    _modalCancelamentoCTe = new bootstrap.Modal(document.getElementById("divModalCancelamentoCTe"), { backdrop: true, keyboard: true });
}

function CancelarClick() {
    Salvar(_cancelamentoCTe, "CTeCancelamento/Cancelar", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelamento / Anulação realizado com sucesso.");
                _modalCancelamentoCTe.hide();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

//*******MÉTODOS*******

function BuscarCTes() {
    var cancelar = { descricao: "Cancelar/Anular", id: guid(), evento: "onclick", metodo: AbrirTelaCancelamentoCTeClick, tamanho: "20", icone: "", visibilidade: VisualizarOpcaoCancelamento };
    var auditar = { descricao: "Auditoria", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("ConhecimentoDeTransporteEletronico", "Codigo"), tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [cancelar, auditar],
        tamanho: 7
    };

    _gridConsultaCTe = new GridView(_pesquisaCTe.Pesquisar.idGrid, "CTeCancelamento/Pesquisa", _pesquisaCTe, menuOpcoes, { column: 4, dir: orderDir.desc }, 10);
    _gridConsultaCTe.CarregarGrid();
}

function VisualizarOpcaoCancelamento(dados) {
    if (dados.Situacao == EnumStatusCTe.AUTORIZADO)
        return true;

    return false;
}

function AbrirTelaCancelamentoCTeClick(dados) {
    LimparCampos(_cancelamentoCTe);
    _cancelamentoCTe.Codigo.val(dados.Codigo);
    _modalCancelamentoCTe.show();
}
