/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />>

//*******MAPEAMENTO KNOUCKOUT*******


var _anexos;
var _gridAnexos;

var Anexos = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Anexos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, idGrid: guid() });
}

//*******EVENTOS*******


function loadAnexos() {
    _anexos = new Anexos();
    KoBindings(_anexos, "knockoutAnexos");

    GridAnexos();
}

function downloadAnexoClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("SolicitacaoAvaria/DownloadAnexo", data);
}




//*******MÉTODOS*******

function limparAnexos() {
    LimparCampos(_anexos);
    GridAnexos();
}

function GridAnexos() {
    //-- Grid Anexos
    // Opcao de downlaod
    var download = {
        descricao: "Download",
        id: "clasEditar",
        evento: "onclick",
        metodo: downloadAnexoClick,
        tamanho: 5,
        icone: ""
    };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    // Gera grid
    var linhasPorPaginas = 7;
    _gridAnexos = new GridView(_anexos.Anexos.idGrid, "SolicitacaoAvaria/PesquisaAnexo", _anexos, menuOpcoes, null);
}

function CarregarAnexos(solicitacao) {
    _anexos.Codigo.val(solicitacao);
    _gridAnexos.CarregarGrid(function () {
        if (_gridAnexos.NumeroRegistros() > 0)
            $("#liAnexos").show();
        else
            $("#liAnexos").hide();
    });
}