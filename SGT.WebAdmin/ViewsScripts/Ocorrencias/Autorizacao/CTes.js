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
/// <reference path="gridCTes.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _ctes;
var _gridCTes;

var CTes = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CTes = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, idGrid: guid() });
}

//*******EVENTOS*******


function loadCTes() {
    _ctes = new CTes();
    KoBindings(_ctes, "knockoutCTes");

    //-- Grid DetalhesCargaSumarizada
    // Opcao de downlaod
    var baixarXMLNFSe = { descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.BaixarXML, id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadDANFSE };
    var baixarDANFSE = { descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.BaixarDANFSE, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadDANFSE };
    var baixarDACTE = { descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.BaixarDACTE, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var baixarPDF = { descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.BaixarPDF, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeDownloadOutrosDoc };
    var baixarXML = { descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.BaixarXML, id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var retornoSefaz = { descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.MensagemSefaz, id: guid(), metodo: retornoSefazClick, icone: "", visibilidade: VisibilidadeMensagemSefaz };
    var emitir = { descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Emitir, id: guid(), metodo: function (datagrid) { emitirCTeClick(datagrid); }, icone: "", visibilidade: VisibilidadeRejeicao };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Opcoes, tamanho: 7, opcoes: [] };


    var menuOpcoes = {
        tamanho: 7,
        descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Opcoes,
        tipo: TypeOptionMenu.list,
        opcoes: [baixarDACTE, baixarDANFSE, baixarXML, baixarXMLNFSe, baixarPDF, emitir, retornoSefaz]
    };

    // Gera grid
    var linhasPorPaginas = 7;
    _gridCTes = new GridView(_ctes.CTes.idGrid, "AutorizacaoOcorrencia/ConsultarCTesDaCarga", _ctes, menuOpcoes, null);
}




//*******MÉTODOS*******

function limparCTes() {
    LimparCampos(_ctes);
    _gridCTes.CarregarGrid();
}

function CarregarCTes(ocorrencia) {
    _ctes.Codigo.val(ocorrencia);
    _gridCTes.CarregarGrid(function () {
        if (_gridCTes.NumeroRegistros() > 0)
            $("#liCTes").show();
        else
            $("#liCTes").hide();
    });
}
