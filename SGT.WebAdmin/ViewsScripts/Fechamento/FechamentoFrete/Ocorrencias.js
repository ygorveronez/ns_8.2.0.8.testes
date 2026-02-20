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
var _ocorrencias;
var _gridOcorrencias;
var _modalOcorrenciaFechamentoFrete;

var Ocorrencias = function () {
    this.Fechamento = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Ocorrencias = PropertyEntity({ type: types.map, idGrid: guid() });
}

//*******EVENTOS*******
function LoadOcorrenciasFechamento() {

    _ocorrencias = new Ocorrencias();
    KoBindings(_ocorrencias, "knockoutOcorrencias");

    _pesquisaOcorrencia = new PesquisaOcorrencia();

    CarregarGridOcorrenciasFechamento();
    carregarLancamentoOcorrencia("conteudoOcorrencia", "modaisOcorrencia", null, false);

    _modalOcorrenciaFechamentoFrete = new bootstrap.Modal(document.getElementById("divModalOcorrencia"), { backdrop: 'static', keyboard: true });
}

//*******METODOS*******
function CarregarGridOcorrenciasFechamento() {
    let editar = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: editarOcorrenciaFechamento, tamanho: "10", icone: "" };
    let menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

    _gridOcorrencias = new GridView(_ocorrencias.Ocorrencias.idGrid, "FechamentoFreteOcorrencia/Pesquisa", _ocorrencias, menuOpcoes);
}

function EditarOcorrenciasFechamento(data) {
    _ocorrencias.Fechamento.val(data.Codigo);
    _gridOcorrencias.CarregarGrid();
}

function editarOcorrenciaFechamento(data) {
    limparCamposOcorrencia();
    _ocorrencia.Codigo.val(data.Codigo);
    buscarOcorrenciaPorCodigo(function () {
        _modalOcorrenciaFechamentoFrete.show();
    });
}