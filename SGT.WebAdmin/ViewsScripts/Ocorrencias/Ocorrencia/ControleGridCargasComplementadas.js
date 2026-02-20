/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Ocorrencia.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _gridCargasComplemento = null;

// Controla os dias em cargas por periodo
// É gravado {"CodigoVeiculo": QuantidadeDias} nessa relação
// Isso é nessário para calcular o valor da ocorrencia e salvar os dados
var ControleGridCargasComplementadas = {};



//*******MÉTODOS*******

function BuscarCargasPorPeriodo(callback) {
    var anexoDaCarga = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Anexar, id: guid(), metodo: anexarCargaSumarizada, icone: "" };
    var detalhar = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Detalhar, id: guid(), metodo: detalhesCargaSumarizada, icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: Localization.Resources.Ocorrencias.Ocorrencia.Opcoes,
        tamanho: 7,
        opcoes: [detalhar, anexoDaCarga]
    };

    var configExport = {
        url: "CargaOcorrencia/ExportarDetalhesCargas",
        titulo: Localization.Resources.Ocorrencias.Ocorrencia.DetalhesCargas
    };

    var ko_ocorrencia = {
        Ocorrencia: _ocorrencia.Codigo,
        TipoOcorrencia: _ocorrencia.TipoOcorrencia,
        PeriodoInicio: _ocorrencia.PeriodoInicio,
        PeriodoFim: _ocorrencia.PeriodoFim,
        Empresa: _ocorrencia.Empresa,
        CargasComplementadas: koControleGridCargasComplementadas(),
        Filial: _ocorrencia.Filial
    };

    var editarColuna = {
        permite: _ocorrencia.Codigo.val() == 0,
        atualizarRow: true,
        callback: AtualizarControleGridCargasComplementadas,
    };

    LimparControleGridCargasComplementadas();

    if (_gridCargasComplemento == null)
        _gridCargasComplemento = new GridView(_ocorrencia.CargasParaComplemento.idGrid, "CargaOcorrencia/ConsultarCargasPorPeriodo", ko_ocorrencia, menuOpcoes, null, null, null, null, null, null, null, editarColuna, configExport);
    _gridCargasComplemento.CarregarGrid(callback);
}

function LimparControleGridCargasComplementadas() {
    ControleGridCargasComplementadas = {};
}

function AtualizarControleGridCargasComplementadas(dataRow, row, head, callbackTabPress) {
    var dias = parseInt(dataRow.QuantidadeDias);

    if (dias == 0)
        return ExibirErroDataRow(_gridCargasComplemento, row, Localization.Resources.Ocorrencias.Ocorrencia.QuantidadeDiasDeveSerMaiorQueZero, tipoMensagem.aviso, "Aviso");

    ControleGridCargasComplementadas[dataRow.CodigoVeiculo] = dias;
    ContarLinhasECalcularValorTipoOcorrencia();
    _gridCargasComplemento.CarregarGrid();
}

function GetControleGridCargasComplementadas() {
    return JSON.stringify(ControleGridCargasComplementadas);
}

function koControleGridCargasComplementadas() {
    // Retorna uma mapeamento como se fosse um knout
    // Apenas converte o objeto ControleGridCargasComplementadas[codigo] -> dias em um objeto {Veiculo: codigo, Dias: dia}
    var _converteCodigos = function () {
        var mapeamento = [];
        for (var codigo in ControleGridCargasComplementadas)
            mapeamento.push({
                Veiculo: codigo,
                Dias: ControleGridCargasComplementadas[codigo]
            });

        return JSON.stringify(mapeamento);
    };
    return {
        val: _converteCodigos,
        type: types.map
    }
}