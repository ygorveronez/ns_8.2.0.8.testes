///// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _gridDocumentosAgrupados;



//*******EVENTOS*******
function loadDocumentosAgrupados() {
    //_ocorrencia.Veiculo.codEntity.subscribe(veiculoBlur);

    CarregarGridDocumentosAgrupados();
}


//*******MÉTODOS*******
function CarregarGridDocumentosAgrupados() {
    var detalhes = {
        descricao: Localization.Resources.Ocorrencias.Ocorrencia.Detalhes, id: guid(), metodo: detalhesDocumentosAgrupados, icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: Localization.Resources.Ocorrencias.Ocorrencia.Opcoes,
        tamanho: 7,
        opcoes: [detalhes]
    };

    var ko_documentosAgrupados = {
        Ocorrencia: _ocorrencia.Codigo,
        Veiculo: _ocorrencia.Veiculo,
        PeriodoInicio: _ocorrencia.PeriodoInicio,
        PeriodoFim: _ocorrencia.PeriodoFim,
        ContratoFreteTransportador: _ocorrencia.ContratoFreteTransportador,
    };
    _gridDocumentosAgrupados = new GridView(_ocorrencia.DocumentosAgrupadosDoVeiculo.idGrid, "CargaOcorrencia/ConsultarDocumentosAgrupadosVeiculo", ko_documentosAgrupados, menuOpcoes);
}

function RecarregarGridDocumentosAgrupados(){
    if (_ocorrencia.Veiculo.codEntity() != "" && _ocorrencia.Veiculo.val() != "")
        _gridDocumentosAgrupados.CarregarGrid();
}