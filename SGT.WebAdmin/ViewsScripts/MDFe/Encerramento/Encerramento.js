/// <reference path="../../Enumeradores/EnumTipoXMLMDFe.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Empresa.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoMDFe.js" />
/// <reference path="Cancelamento.js" />
/// <reference path="InclusaoMotorista.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridMDFe;
var _pesquisaMDFe;

var _situacaoMDFe = [
    { text: "Todas", value: "" },
    { text: "Autorizado", value: EnumSituacaoMDFe.Autorizado },
    { text: "Cancelado", value: EnumSituacaoMDFe.Cancelado },
    { text: "Em cancelamento", value: EnumSituacaoMDFe.EmCancelamento },
    { text: "Em digitação", value: EnumSituacaoMDFe.EmDigitacao },
    { text: "Em encerramento", value: EnumSituacaoMDFe.EmEncerramento },
    { text: "Emitido em contingência", value: EnumSituacaoMDFe.EmitidoContingencia },
    { text: "Encerrado", value: EnumSituacaoMDFe.Encerrado },
    { text: "Enviado", value: EnumSituacaoMDFe.Enviado },
    { text: "Pendente", value: EnumSituacaoMDFe.Pendente },
    { text: "Rejeição", value: EnumSituacaoMDFe.Rejeicao },
];

var PesquisaMDFe = function () {

    var data = moment(new Date).format("DD/MM/YYYY");

    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoMDFe, def: "", text: "Situação:" });

    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(data), def: data });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(data), def: data });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.NumeroInicial = PropertyEntity({ text: "Número Inicial:", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final:", getType: typesKnockout.int, val: ko.observable(""), def: "" });
    this.EstadoCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Estado de Carregamento:", idBtnSearch: guid() });
    this.EstadoDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Estado de Descarregamento:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMDFe.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadEncerramentoMDFe() {

    _pesquisaMDFe = new PesquisaMDFe();
    KoBindings(_pesquisaMDFe, "knockoutPesquisaMDFe", _pesquisaMDFe.Pesquisar.id);

    HeaderAuditoria("ManifestoEletronicoDeDocumentosFiscais");

    new BuscarEstados(_pesquisaMDFe.EstadoCarregamento);
    new BuscarEstados(_pesquisaMDFe.EstadoDescarregamento);
    new BuscarVeiculos(_pesquisaMDFe.Veiculo);

    loadCancelamento();
    loadEdiFiscal();
    LoadInclusaoMotoristaMDFe();

    buscarMDFes();
}

//*******MÉTODOS*******

function encerrarMDFe(mdfeGrid) {
    exibirConfirmacao("Atenção!", "Deseja realmente encerrar o MDF-e " + mdfeGrid.Numero + "?", function () {
        executarReST("Encerramento/Encerrar", { MDFe: mdfeGrid.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "MDF-e enviado para encerramento com sucesso!");
                    _gridMDFe.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function baixarDAMDFE(mdfeGrid) {
    executarDownload("Encerramento/DownloadDAMDFE", { MDFe: mdfeGrid.Codigo });
}

function baixarXMLAutorizacao(mdfeGrid) {
    baixarXML(mdfeGrid, EnumTipoXMLMDFe.Autorizacao);
}

function baixarXMLCancelamento(mdfeGrid) {
    baixarXML(mdfeGrid, EnumTipoXMLMDFe.Cancelamento);
}

function baixarXMLEncerramento(mdfeGrid) {
    baixarXML(mdfeGrid, EnumTipoXMLMDFe.Encerramento);
}

function baixarXML(mdfe, tipo) {
    executarDownload("Encerramento/DownloadXML", { MDFe: mdfe.Codigo, Tipo: tipo });
}

function buscarMDFes() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [
            { descricao: "Encerrar", id: guid(), evento: "onclick", metodo: encerrarMDFe, tamanho: "10", icone: "" },
            { descricao: "DAMDFE", id: guid(), evento: "onclick", metodo: baixarDAMDFE, tamanho: "10", icone: "" },
            { descricao: "Cancelar", id: guid(), evento: "onclick", metodo: abrirCancelamento, tamanho: "10", icone: "", visibilidade: CancelarMDFVisibilidade },
            { descricao: "XML de Autorização", id: guid(), evento: "onclick", metodo: baixarXMLAutorizacao, tamanho: "10", icone: "" },
            { descricao: "XML de Cancelamento", id: guid(), evento: "onclick", metodo: baixarXMLCancelamento, tamanho: "10", icone: "" },
            { descricao: "XML de Encerramento", id: guid(), evento: "onclick", metodo: baixarXMLEncerramento, tamanho: "10", icone: "" },
            { descricao: "EDI Fiscal", id: guid(), evento: "onclick", metodo: abrirEdiFiscal, tamanho: "10", icone: "", visibilidade: CancelarMDFVisibilidade },
            { descricao: "Adicionar Motorista", id: guid(), evento: "onclick", metodo: AbrirInclusaoMotoristaMDFeClick, tamanho: "10", icone: "", visibilidade: VisibilidadeEncerramentoMDFe },
            { descricao: "Auditoria", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("ManifestoEletronicoDeDocumentosFiscais", "Codigo"), tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoAuditoria }
        ],
        tamanho: 10
    };

    _gridMDFe = new GridView(_pesquisaMDFe.Pesquisar.idGrid, "Encerramento/Pesquisa", _pesquisaMDFe, menuOpcoes, { column: 1, dir: orderDir.desc }, 10);
    _gridMDFe.CarregarGrid();
}

