/// <reference path="../../Enumeradores/EnumStatusCTe.js" /> 

//*******MAPEAMENTO KNOUCKOUT*******

var _statusCTe = [
    { text: "Todas", value: "" },
    { text: "Autorizado", value: EnumStatusCTe.AUTORIZADO },
    { text: "Cancelado", value: EnumStatusCTe.CANCELADO },
    { text: "Denegado", value: EnumStatusCTe.DENEGADO },
    { text: "Em Digitação", value: EnumStatusCTe.EmDigitaçãEMDIGITACAO },
    { text: "Enviado", value: EnumStatusCTe.ENVIADO },
    { text: "Inutilizado", value: EnumStatusCTe.INUTILIZADO },
    { text: "Pendente", value: EnumStatusCTe.PENDENTE },
    { text: "Rejeição", value: EnumStatusCTe.REJEICAO },
    { text: "Em Cancelamento", value: EnumStatusCTe.EMCANCELAMENTO },
    { text: "Em Inutilização", value: EnumStatusCTe.EMINUTILIZACAO },
    { text: "Aguardando Finalizar Carga", value: EnumStatusCTe.AGUARDANDOFINALIZARCARGA },
];

var _gridControleEmissaoCTe;
var _pesquisaCTe;

var ControleEmissaoCTe = function () {
    this.NumeroCarga = PropertyEntity({ text: "Número Carga:", getType: typesKnockout.string });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid() });
    this.Status = PropertyEntity({ text: "Situação:", val: ko.observable(""), options: _statusCTe, def: "" });
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial:", getType: typesKnockout.int, val: ko.observable(0), def: ko.observable(0) });
    this.NumeroFinal = PropertyEntity({ text: "Número Final:", getType: typesKnockout.int, val: ko.observable(0), def: ko.observable(0) });
    this.Serie = PropertyEntity({ text: "Serie:", getType: typesKnockout.int, val: ko.observable(0), def: ko.observable(0) });

    this.DataFinal.dateRangeInit = this.DataInicial;
    this.DataInicial.dateRangeLimit = this.DataFinal;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridControleEmissaoCTe.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid()
    });
}
//*******EVENTOS*******

function LoadControleEmissaoCTe() {
    _pesquisaCTe = new ControleEmissaoCTe();
    KoBindings(_pesquisaCTe, "knockoutControleEmissaoCTe", false, _pesquisaCTe.Pesquisar.id);

    new BuscarTransportadores(_pesquisaCTe.Empresa);

    BuscarCTes();
}

function BaixarXMLClick(e) {
    var data = { CTe: e.Codigo };
    executarDownload("ControleEmissaoCTe/DownloadXML", data);
}

function BaixarDACTEClick(e) {
    var data = { CTe: e.Codigo };
    executarDownload("ControleEmissaoCTe/DownloadDACTE", data);
}

function ReemitirClick(e) {
    var data = { Codigo: e.Codigo };
    executarReST("ControleEmissaoCTe/Reemitir", data, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "CT-e enviado para emissão.");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function RemoverConsultaClick(e) {
    var data = { Codigo: e.Codigo };
    executarReST("ControleEmissaoCTe/RemoverConsulta", data, function (r) {
        if (r.Success) {
            if (r.Data) {
                _gridControleEmissaoCTe.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo efetuado com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function visibilidadeReemitir(e) {
    return true;
}

function VisibilidadeOpcaoAuditoriaCTe() {
    return PermiteAuditar();
}

//*******MÉTODOS*******

function BuscarCTes() {
    var downloadXML = { descricao: "Download XML", id: guid(), evento: "onclick", metodo: BaixarXMLClick, tamanho: "20", icone: "" };
    var downloadDACTE = { descricao: "Download PDF", id: guid(), evento: "onclick", metodo: BaixarDACTEClick, tamanho: "20", icone: "" };
    var reemitir = { descricao: "Reemitir", id: guid(), evento: "onclick", metodo: ReemitirClick, visibilidade: visibilidadeReemitir, tamanho: "20", icone: "" };
    var removerConsulta = { descricao: "Remover Consulta", id: guid(), evento: "onclick", metodo: RemoverConsultaClick, tamanho: "20", icone: "" };
    var auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("ConhecimentoDeTransporteEletronico"), icone: "", visibilidade: VisibilidadeOpcaoAuditoriaCTe };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [downloadXML, downloadDACTE, reemitir, removerConsulta, auditar],
        descricao: "Opções",
        tamanho: 7
    };

    _gridControleEmissaoCTe = new GridView(_pesquisaCTe.Pesquisar.idGrid, "ControleEmissaoCTe/Pesquisa", _pesquisaCTe, menuOpcoes, { column: 3, dir: orderDir.desc }, 10);
    _gridControleEmissaoCTe.CarregarGrid();
}
