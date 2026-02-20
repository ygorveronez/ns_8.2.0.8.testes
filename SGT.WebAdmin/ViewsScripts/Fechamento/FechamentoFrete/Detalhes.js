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
var _historicoCargas;
var _detalheContrato;
var _gridHistoricoCargas;
var _gridDetalheContratoAcordos;
var _modalDetalhesContrato;
var _modalDetalhesCarga;

var HistoricoCargas = function () {
    this.Grid = PropertyEntity({ type: types.local, idGrid: guid() });
}

var DetalheContrato = function () {
    this.Mensal = PropertyEntity({ type: types.local, idGrid: guid() });

    this.TotalPorCavalo = PropertyEntity({ type: types.map, text: "Total KM por Cavalo:" });
    this.TotalKM = PropertyEntity({ type: types.map, text: "Total KM:" });
    this.ContratoMensal = PropertyEntity({ type: types.map, text: "Valor do Contrato Mensal:" });
    this.ValorKM = PropertyEntity({ type: types.map, text: "Valor por KM:" });
    this.ValorKmExcedente = PropertyEntity({ type: types.map, text: "Valor KM Excedente:" });
    this.KMConsumido = PropertyEntity({ type: types.map, text: "KM Consumido:" });
    this.ValorPago = PropertyEntity({ type: types.map, text: "Valor Pago:" });
}


//*******EVENTOS*******
function LoadDetalhes() {
    _historicoCargas = new HistoricoCargas();
    KoBindings(_historicoCargas, "knockoutDetalhesCargas");

    _detalheContrato = new DetalheContrato();
    KoBindings(_detalheContrato, "knockoutDetalhesContrato");

    GridHistoricoCargas();
    GridDetalheContrato();

    _modalDetalhesContrato = new bootstrap.Modal(document.getElementById("divModalDetalhesContrato"), { backdrop: 'static', keyboard: true });
    _modalDetalhesCarga = new bootstrap.Modal(document.getElementById("divModalDetalhesCargas"), { backdrop: 'static', keyboard: true })
}

function verContratoClick() {
    _modalDetalhesContrato.show();
}

function verHistoricoClick() {
    _modalDetalhesCarga.show();
}


//*******MÉTODOS*******
function GridHistoricoCargas() {

    var downloadDocumentos = {
        id: guid(),
        descricao: "Download Documentos",
        metodo: downloadDocumentosCarga
    };

    var removerDoFechamento = {
        id: guid(),
        descricao: "Remover",
        visibilidade: visibilidadeRemoverDoFechamento,
        metodo: adicionarCargaAoFechamento
    };

    var adicionarAoFechamento = {
        id: guid(),
        descricao: "Adicionar",
        visibilidade: visibilidadeAdicionarAoFechamento,
        metodo: removerCargaDoFechamento
    };
    
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 7,
        opcoes: [removerDoFechamento, adicionarAoFechamento, downloadDocumentos]
    };

    _gridHistoricoCargas = new GridView(_historicoCargas.Grid.idGrid, "FechamentoFrete/CargasFechamento", _dadosFechamento, menuOpcoes);
}

function GridDetalheContrato() {
    _gridDetalheContratoAcordos = new GridView(_detalheContrato.Mensal.idGrid, "FechamentoFrete/AcordosContrato", _dadosFechamento);
}

function removerCargaDoFechamento(data, row) {
    if (CargaRemovidaDoFechamento(data.Codigo)) {
        var index = $.inArray(data.Codigo, _dadosFechamento.CargasRemovidas.list);
        _dadosFechamento.CargasRemovidas.list.splice(index, 1);
        data.DT_RowColor = "";
        _gridHistoricoCargas.AtualizarDataRow(row, data);
    }
}

function downloadDocumentosCarga(data, row) {
    executarDownload("CargaImpressaoDocumentos/DownloadLotePDF", { Carga: data.Codigo });
}

function adicionarCargaAoFechamento(data, row) {
    if (!CargaRemovidaDoFechamento(data.Codigo)) {
        data.DT_RowColor = "#ccc";
        _dadosFechamento.CargasRemovidas.list.push(data.Codigo);
        _gridHistoricoCargas.AtualizarDataRow(row, data);
    }
}

function visibilidadeRemoverDoFechamento(data) {
    if (_fechamentoFrete.Situacao.val() != EnumSituacaoFechamentoFrete.Aberto)
        return false;
    return !CargaRemovidaDoFechamento(data.Codigo);
}

function visibilidadeAdicionarAoFechamento(data) {
    if (_fechamentoFrete.Situacao.val() != EnumSituacaoFechamentoFrete.Aberto)
        return false;
    return CargaRemovidaDoFechamento(data.Codigo);
}

function GetSetCargasRemovidas() {
    if (arguments.length > 0) {
        _dadosFechamento.CargasRemovidas.list = arguments[0].slice();
    } else {
        return JSON.stringify(_dadosFechamento.CargasRemovidas.list);
    }
}

function CargaRemovidaDoFechamento(codigo) {
    return $.inArray(codigo, _dadosFechamento.CargasRemovidas.list) >= 0;
}