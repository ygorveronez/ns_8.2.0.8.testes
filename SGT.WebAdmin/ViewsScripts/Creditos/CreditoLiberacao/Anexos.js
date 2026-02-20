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
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="CTeComplementar.js" />
/// <reference path="../../Creditos/ControleSaldo/ControleSaldo.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="EtapasOcorrencia.js" />
/// <reference path="Autorizacao.js" />
/// <reference path="ResumoOcorrencia.js" />
/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../Enumeradores/EnumTipoTomador.js" />
/// <reference path="../../Consultas/CTe.js" />

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

    //-- Grid Anexos
    // Opcao de downlaod
    var download = {
        descricao: "Download",
        id: "clasEditar",
        evento: "onclick",
        metodo: downloadAnexoClick,
        tamanho: 3,
        icone: ""
    };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    // Gera grid
    var linhasPorPaginas = 7;
    _gridAnexos = new GridView(_anexos.Anexos.idGrid, "OcorrenciaAnexos/Pesquisa", _anexos, menuOpcoes, null);
    _gridAnexos.CarregarGrid();
}

function downloadAnexoClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("OcorrenciaAnexos/DownloadAnexo", data);
}

function gerenciarAnexosClick() {
    Global.abrirModal('divModalGerenciarAnexos');
    _gridAnexos.CarregarGrid();
}




//*******MÉTODOS*******

function limparAnexos() {
    LimparCampos(_anexos);
    _gridAnexos.CarregarGrid();
}

function CarregarAnexos(ocorrencia) {
    _anexos.Codigo.val(ocorrencia);
    _gridAnexos.CarregarGrid();
}