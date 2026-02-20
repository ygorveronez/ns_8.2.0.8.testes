/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />

var _documentos;

var Documentos = function () {
    this.Documentos = PropertyEntity({ type: types.local, text: "Documentos", val: ko.observable(""), idGrid: guid() });
}



//*******EVENTOS*******

function loadDocumentos() {
    _documentos = new Documentos();
    KoBindings(_documentos, "knockoutDocumentos");

    GridDocumentos();
}




//*******MÉTODOS*******
function GridDocumentos() {
    var baixarPDF = { descricao: "Download", tamanho: 7, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeDownloadOutrosDoc };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [baixarPDF]
    };

    var ko_ocorrencia = {
        Codigo: _aceite.Ocorrencia
    };
   
    _gridCTeComplementar = new GridView(_documentos.Documentos.idGrid, "Ocorrencia/ConsultarCTesOcorrencia", ko_ocorrencia, menuOpcoes, null);
}

function VisibilidadeDownloadOutrosDoc(data) {
    return (data.NumeroModeloDocumentoFiscal != "57" && data.NumeroModeloDocumentoFiscal != "39");
}

function baixarDacteClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadDacte", data);
}