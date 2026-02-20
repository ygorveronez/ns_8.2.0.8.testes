/// <reference path="../../Enumeradores/EnumStatusNFSe.js" /> 

//*******MAPEAMENTO KNOUCKOUT*******

var _statusNFSe = [
    { text: "Todas", value: "" },
    { text: "Em Digitação", value: EnumStatusNFSe.EmDigitacao },
    { text: "Pendente", value: EnumStatusNFSe.Pendente },
    { text: "Enviado", value: EnumStatusNFSe.Enviado },
    { text: "Autorizado", value: EnumStatusNFSe.Autorizado },
    { text: "Em Cancelamento", value: EnumStatusNFSe.EmCancelamento },
    { text: "Cancelado", value: EnumStatusNFSe.Cancelado },
    { text: "Rejeição", value: EnumStatusNFSe.Rejeicao },
];

var _gridControleEmissaoNFSe;
var _pesquisaNFSe;

var ControleEmissaoNFSe = function () {
    this.NumeroCarga = PropertyEntity({ text: "Número Carga:", getType: typesKnockout.string });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid() });
    this.Status = PropertyEntity({ text: "Situação:", val: ko.observable(""), options: _statusNFSe, def: "" });
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial:", getType: typesKnockout.int, val: ko.observable(0), def: ko.observable(0) });
    this.NumeroFinal = PropertyEntity({ text: "Número Final:", getType: typesKnockout.int, val: ko.observable(0), def: ko.observable(0) });
    this.Serie = PropertyEntity({ text: "Serie:", getType: typesKnockout.int, val: ko.observable(0), def: ko.observable(0) });

    this.DataFinal.dateRangeInit = this.DataInicial;
    this.DataInicial.dateRangeLimit = this.DataFinal;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridControleEmissaoNFSe.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid()
    });
}
//*******EVENTOS*******

function LoadControleEmissaoNFSe() {
    _pesquisaNFSe = new ControleEmissaoNFSe();
    KoBindings(_pesquisaNFSe, "knockoutControleEmissaoNFSe", false, _pesquisaNFSe.Pesquisar.id);

    new BuscarTransportadores(_pesquisaNFSe.Empresa);

    BuscarNFSes();
}

function BaixarXMLClick(e) {
    var data = { NFSe: e.Codigo };
    executarDownload("ControleEmissaoNFSe/DownloadXMLNFSe", data);
}

function BaixarDACTEClick(e) {
    var data = { NFSe: e.Codigo };
    executarDownload("ControleEmissaoNFSe/DownloadPDFNFSe", data);
}

function ReemitirClick(e) {
    var data = { Codigo: e.Codigo };
    executarReST("ControleEmissaoNFSe/ReemitirNFSe", data, function (r) {
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

function visibilidadeReemitir(e) {
    return true;
}

//*******MÉTODOS*******

function BuscarNFSes() {
    var downloadXML = { descricao: "Download XML", id: guid(), evento: "onclick", metodo: BaixarXMLClick, tamanho: "20", icone: "" };
    var downloadDACTE = { descricao: "Download PDF", id: guid(), evento: "onclick", metodo: BaixarDACTEClick, tamanho: "20", icone: "" };
    var reemitir = { descricao: "Reemitir", id: guid(), evento: "onclick", metodo: ReemitirClick, visibilidade: visibilidadeReemitir, tamanho: "20", icone: "" };
    
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [downloadXML, downloadDACTE, reemitir],
        descricao: "Opções",
        tamanho: 7
    };

    _gridControleEmissaoNFSe = new GridView(_pesquisaNFSe.Pesquisar.idGrid, "ControleEmissaoNFSe/PesquisaNFSe", _pesquisaNFSe, menuOpcoes, { column: 3, dir: orderDir.desc }, 10);
    _gridControleEmissaoNFSe.CarregarGrid();
}
