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
var _nfses;
var _gridNFSes;

var NFSes = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NFSes = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, idGrid: guid() });
}

//*******EVENTOS*******


function loadNFSes() {
    _nfses = new NFSes();
    KoBindings(_nfses, "knockoutNFSes");

    //-- Grid NFSes
    // Opcao de downlaod
    var baixarDANFSE = { descricao: "Baixar DANFSE", id: guid(), metodo: baixarDacteClick, icone: "" };
    var baixarXML = { descricao: "Baixar XML", id: guid(), metodo: baixarXMLCTeClick, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [baixarDANFSE, baixarXML] };

    // Gera grid
    var linhasPorPaginas = 7;
    _gridNFSes = new GridView(_nfses.NFSes.idGrid, "NFSManual/PesquisaNFSGerados", _nfses, menuOpcoes, null);
    _gridNFSes.CarregarGrid();
}

function baixarXMLCTeClick(e) {
    if ("CodigoNFSe" in e) {
        executarDownload("NFSManual/DownloadXML", {
            Codigo: _lancamentoNFS.Codigo.val()
        });
    } else {
        executarDownload("CargaCTe/DownloadXML", {
            CodigoCTe: e.CodigoCTE,
            CodigoEmpresa: e.CodigoEmpresa
        });
    }
}

function baixarDacteClick(e) {
    if ("CodigoNFSe" in e) {
        executarDownload("NFSManual/DownloadDANFSE", {
            Codigo: _lancamentoNFS.Codigo.val()
        });
    } else {
        executarDownload("CargaCTe/DownloadDacte", {
            CodigoCTe: e.CodigoCTE,
            CodigoEmpresa: e.CodigoEmpresa
        });
    }
}




//*******MÉTODOS*******

function limparNFSes() {
    LimparCampos(_nfses);
    _gridNFSes.CarregarGrid();
}

function CarregarNFSes(lancamento) {
    _nfses.Codigo.val(lancamento);
    _gridNFSes.CarregarGrid(function () {
        if (_gridNFSes.NumeroRegistros() > 0)
            $("#liNFSes").show();
        else
            $("#liNFSes").hide();
    });

    obterAnexosDoServer(lancamento);
}

function obterAnexosDoServer(codigo) {
    executarReST("AnexoLancamentoNFSManual/ObterAnexo", { Codigo: codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _listaAnexos.Anexos.val(retorno.Data.Anexos);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

