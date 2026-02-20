/// <reference path="../../Enumeradores/EnumSituacaoMDFe.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _statusMDFe = [
    { text: "Todas", value: "-1" },
    { text: "Autorizado", value: EnumSituacaoMDFe.Autorizado },
    { text: "Cancelado", value: EnumSituacaoMDFe.Cancelado },
    { text: "Em Cancelamento", value: EnumSituacaoMDFe.EmCancelamento },
    { text: "Em Digitação", value: EnumSituacaoMDFe.EmDigitacao },
    { text: "Em Encerramento", value: EnumSituacaoMDFe.EmEncerramento },
    { text: "Encerrado", value: EnumSituacaoMDFe.Encerrado },
    { text: "Enviado", value: EnumSituacaoMDFe.Enviado },
    { text: "Pendente", value: EnumSituacaoMDFe.Pendente },
    { text: "Rejeição", value: EnumSituacaoMDFe.Rejeicao },
];

var _gridControleEmissaoMDFe;
var _pesquisaMDFe;

var ControleEmissaoMDFe = function () {
    this.NumeroCarga = PropertyEntity({ text: "Número Carga:", getType: typesKnockout.string });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid() });
    this.Status = PropertyEntity({ text: "Situação:", val: ko.observable("-1"), options: _statusMDFe, def: "" });

    this.DataFinal.dateRangeInit = this.DataInicial;
    this.DataInicial.dateRangeLimit = this.DataFinal;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridControleEmissaoMDFe.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid()
    });
}
//*******EVENTOS*******

function LoadControleEmissaoMDFe() {
    _pesquisaMDFe = new ControleEmissaoMDFe();
    KoBindings(_pesquisaMDFe, "knockoutControleEmissaoMDFe", false, _pesquisaMDFe.Pesquisar.id);

    new BuscarTransportadores(_pesquisaMDFe.Empresa);

    BuscarMDFes();
}

function BaixarXMLClick(e) {
    var data = { MDFe: e.Codigo };
    executarDownload("ControleEmissaoMDFe/DownloadXML", data);
}

function BaixarDAMDFEClick(e) {
    var data = { MDFe: e.Codigo };
    executarDownload("ControleEmissaoMDFe/DownloadDAMDFE", data);
}

function ReemitirClick(e) {
    var data = { Codigo: e.Codigo };
    executarReST("ControleEmissaoMDFe/Reemitir", data, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "MDF-e enviado para emissão.");
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

function BuscarMDFes() {
    var downloadXML = { descricao: "Download XML", id: guid(), evento: "onclick", metodo: BaixarXMLClick, tamanho: "20", icone: "" };
    var downloadDAMDFE = { descricao: "Download PDF", id: guid(), evento: "onclick", metodo: BaixarDAMDFEClick, tamanho: "20", icone: "" };
    var reemitir = { descricao: "Reemitir", id: guid(), evento: "onclick", metodo: ReemitirClick, visibilidade: visibilidadeReemitir, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [downloadXML, downloadDAMDFE, reemitir],
        descricao: "Opções",
        tamanho: 7
    };

    _gridControleEmissaoMDFe = new GridView(_pesquisaMDFe.Pesquisar.idGrid, "ControleEmissaoMDFe/Pesquisa", _pesquisaMDFe, menuOpcoes, { column: 3, dir: orderDir.desc }, 10);
    _gridControleEmissaoMDFe.CarregarGrid();
}
