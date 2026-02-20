/// <reference path="../../Enumeradores/EnumStatusCTe.js" />
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
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="CTeAverbacao.js" />
/// <reference path="ValePedagio.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _cte;
var _gridCTe;
var _knoutArquivo;
var _CodigoCTeCancelamento;

var Arquivo = function () {
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), required: false, visible: ko.observable(false) });
};

var CTe = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CancelamentoCarga = PropertyEntity({ val: ko.observable(0), def: 0 });    

    this.BuscarCTes = PropertyEntity({ eventClick: ExecutaConsultarCTesCarga, type: types.event, text: Localization.Resources.Cargas.CancelamentoCarga.BuscarAtualizarCTe, visible: ko.observable(true), idGrid: guid() });
    this.BuscarAverbacoes = PropertyEntity({ eventClick: BuscarAverbacoesCTesCargaClick, type: types.event, text: Localization.Resources.Cargas.CancelamentoCarga.BuscarAtualizarAverbações, visible: ko.observable(true), idGrid: guid() });
    this.BuscarValePedagio = PropertyEntity({ eventClick: BuscarValePedagioCargaClick, type: types.event, text: Localization.Resources.Cargas.CancelamentoCarga.BuscarAtualizarValePedagio, visible: ko.observable(true), idGrid: guid() });
    this.SincronizarTodosDocumento = PropertyEntity({ eventClick: sincronizarTodosDocumentoClick, type: types.event, text: Localization.Resources.Cargas.Carga.SincronizarTodosDocumento, idGrid: guid(), visible: ko.observable(false) });
};

//*******EVENTOS*******

function LoadCTe() {
    _cte = new CTe();
    KoBindings(_cte, "knockoutCTe");
   
    _knoutArquivo = new Arquivo();
    KoBindings(_knoutArquivo, "knoutEnviarArquivo");
    $("#" + _knoutArquivo.Arquivo.id).on("change", enviarXMLCTeClick);
}

function enviarXMLCancelamentoClick(e) {
    _CodigoCTeCancelamento = e.Codigo;
    $("#" + _knoutArquivo.Arquivo.id).trigger("click");
}

function enviarXMLCTeClick() {
    var file = document.getElementById(_knoutArquivo.Arquivo.id);
    if (file.files.length > 0) {
        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.CancelamentoCarga.DesejaEnviarArquivo + file.files[0].name + "?", function () {
            var formData = new FormData();
            formData.append("upload", file.files[0]);
            var data = {
                CodigoCTe: _CodigoCTeCancelamento
            };
            enviarArquivo("CTe/EnviarXMLCancelamentoCTe?callback=?", data, formData, function (arg) {
                if (arg.Success) {
                    if (arg.Data !== false) {
                        _gridCTe.CarregarGrid();
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });
        });
    }
}

function ConsultarCTesCarga(e) {
    ExecutaConsultarCTesCarga(e);
    buscarCargasCTeAverbacao();
    buscarCargasValePedagio();
}

function ExecutaConsultarCTesCarga(e) {
    _cte.Carga.val(_cancelamento.Carga.codEntity());
    _cte.CancelamentoCarga.val(_cancelamento.Codigo.val());
       
    var baixarXMLNFSe = { descricao: Localization.Resources.Cargas.CancelamentoCarga.BaixarXML, id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadDANFSE };
    var baixarDANFSE = { descricao: Localization.Resources.Cargas.CancelamentoCarga.BaixarDANFSE, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadDANFSE };
    var enviarXMLCancelamento = { descricao: Localization.Resources.Cargas.CancelamentoCarga.EnviarXMLCancelamento, id: guid(), metodo: enviarXMLCancelamentoClick, icone: "", visibilidade: VisibilidadeEnviarXMLCancelamento };
    var baixarDACTE = { descricao: Localization.Resources.Cargas.CancelamentoCarga.BaixarDACTE, id: guid(), metodo: BaixarDACTEClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadCTe };
    var baixarPDF = { descricao: Localization.Resources.Cargas.CancelamentoCarga.BaixarPDF, id: guid(), metodo: BaixarPDFClick, icone: "", visibilidade: VisibilidadeDownloadOutrosDocumentosCTe };
    var baixarXML = { descricao: Localization.Resources.Cargas.CancelamentoCarga.BaixarXML, id: guid(), metodo: BaixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadCTe };
    var baixarXMLCancelamento = { descricao: Localization.Resources.Cargas.CancelamentoCarga.BaixarXMLCancelamento, id: guid(), metodo: BaixarXMLCancelamentoClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadXMLCancelamento };
    var baixarXMLInutilizacao = { descricao: Localization.Resources.Cargas.CancelamentoCarga.BaixarXMLInutilizacao, id: guid(), metodo: BaixarXMLInutilizacaoClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadXMLInutilizacao };
    var informarConcelamentoPrefeitura = { descricao: Localization.Resources.Cargas.CancelamentoCarga.CancelamentoNaPrefeitura, id: guid(), metodo: informarCancelamentoPrefeituraClick, icone: "", visibilidade: VisibilidadeOpcaoInformarConcelamentoPrefeitura };
    var uploadDesacordo = { descricao: Localization.Resources.Cargas.CancelamentoCarga.UploadDesacordo, id: guid(), metodo: VerificarDesacordo, icone: "", visibilidade: true };
    var reverterAnulacao = { descricao: Localization.Resources.Cargas.CancelamentoCarga.ReverterAnulacao, id: guid(), metodo: ReverterAnulacaoGerencialCteClick, icone: "", visibilidade: VisibilidadeOpcaoReverterAnulacao };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [baixarDACTE, baixarDANFSE, baixarPDF, baixarXML, baixarXMLNFSe, enviarXMLCancelamento, baixarXMLCancelamento, baixarXMLInutilizacao, informarConcelamentoPrefeitura, uploadDesacordo, reverterAnulacao], tamanho: 7 };

    _gridCTe = new GridView(_cte.BuscarCTes.idGrid, "CancelamentoCargaCTe/Pesquisa", _cte, menuOpcoes, { column: 3, dir: orderDir.desc }, null);

    _gridCTe.CarregarGrid(function (dataRetorno) {
        if (dataRetorno.data.length > 0) {
            var habilitar = dataRetorno.data.some(function(item) { return item.HabilitarSincronizarDocumento === true; });
            _cte.SincronizarTodosDocumento.visible(habilitar);
        } else {
            _cte.SincronizarTodosDocumento.visible(false);
        }
    });
}

function baixarXMLCTeClick(e) {
    var data = { CodigoCTe: e.Codigo, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadXML", data);
}

function baixarDacteClick(e) {
    var data = { CodigoCTe: e.Codigo, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadDacte", data);
}

function BaixarXMLCTeClick(e) {
    var data = { CTe: e.Codigo };
    executarDownload("CancelamentoCargaCTe/DownloadXML", data);
}

function BaixarDACTEClick(e) {
    var data = { CTe: e.Codigo };
    executarDownload("CancelamentoCargaCTe/DownloadDACTE", data);
}

function BaixarPDFClick(e) {
    var data = { CTe: e.Codigo };
    executarDownload("CancelamentoCargaCTe/DownloadPDF", data);
}

function BaixarXMLCancelamentoClick(e) {
    var data = { CTe: e.Codigo };
    executarDownload("CancelamentoCargaCTe/DownloadXMLCancelamento", data);
}

function BaixarXMLInutilizacaoClick(e) {
    var data = { CTe: e.Codigo };
    executarDownload("CancelamentoCargaCTe/DownloadXMLInutilizacao", data);
}

function informarCancelamentoPrefeituraClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.CancelamentoCarga.InformarCancelamentoPrefeitura, function () {
        executarReST("CancelamentoCargaCTe/InformarCancelamentoPrefeitura", { Cancelamento: _cancelamento.Codigo.val(), CTe: e.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.CancelamentoCarga.CancelamentoPrefeituraInformado);
                    _gridCTe.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function ReverterAnulacaoGerencialCteClick(e) {
    
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Deseja realmente executar essa ação?", function () {
        executarReST("CancelamentoCargaCTe/ReverterAnulacaoGerencialCTe", { CodigoCte: e.Codigo, Cancelamento: _cancelamento.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Alterado com sucesso.");
                    _gridCTe.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function LiberarCancelamentoComCTeNaoInutilizadoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.CancelamentoCarga.CancelamentoSemInutilizarCTesRejeitados, function () {
        executarReST("CancelamentoCargaCTe/LiberarSemInutilizarCTes", { Codigo: _cancelamento.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CancelamentoLiberado);
                    BuscarCancelamentoPorCodigo(false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

//*******MÉTODOS*******

function LimparCamposCTe() {
    LimparCampos(_cte);
}

function VisibilidadeOpcaoDownloadDANFSE(data) {
    if ((data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.ANULADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO) && data.NumeroModeloDocumentoFiscal == "39") {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeOpcaoDownloadCTe(data) {
    if ((data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO || data.SituacaoCTe == EnumStatusCTe.ANULADO) && data.NumeroModeloDocumentoFiscal == "57")
        return true;
    else
        return false;
}

function VisibilidadeDownloadOutrosDocumentosCTe(data) {
    if (data.NumeroModeloDocumentoFiscal != "57" && data.NumeroModeloDocumentoFiscal != "39")
        return true;
    else
        return false;
}

function VisibilidadeEnviarXMLCancelamento(data) {
    if (_cancelamento.Situacao.val() == EnumSituacaoCancelamentoCarga.RejeicaoCancelamento && data.SituacaoCTe == EnumStatusCTe.AUTORIZADO)
        return true;
    else
        return false;
}

function VisibilidadeOpcaoDownloadXMLCancelamento(data) {
    if (data.SituacaoCTe == EnumStatusCTe.CANCELADO && data.NumeroModeloDocumentoFiscal == "57") {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeOpcaoDownloadXMLInutilizacao(data) {
    if (data.SituacaoCTe == EnumStatusCTe.INUTILIZADO && data.NumeroModeloDocumentoFiscal == "57") {
        return true;
    } else {
        return false;
    }
}
function VisibilidadeOpcaoReverterAnulacao(data) {  
    if (data.SituacaoCTe == EnumStatusCTe.ANULADO && data.Status == "Anulado Gerencialmente" && _CONFIGURACAO_TMS.PermitirReverterAnulacaoGerencialTelaCancelamento) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeOpcaoInformarConcelamentoPrefeitura(data) {
    return (_cancelamento.Situacao.val() === EnumSituacaoCancelamentoCarga.RejeicaoCancelamento && data.SituacaoCTe == EnumStatusCTe.AUTORIZADO && data.TipoDocumentoEmissao === EnumTipoDocumentoEmissao.NFSe);
}

function VisibilidadUploadDesacordo(data) {
    return data.SituacaoCTe == EnumStatusCTe.CANCELADO;
}

function VerificarDesacordo(e) {

    let input = document.createElement('input');
    input.type = 'file';
    input.accept = '.xml';

    input.onchange = function () {
        let file = this.files[0];
        let formData = new FormData();
        formData.append('file', file);
        enviarArquivo("CancelamentoCarga/UploadDesacordoCTe", { Codigo: _cancelamento.Codigo.val(), CodigoCte: e.Codigo  }, formData, (arg) => {
            if (!arg.Success)
                return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

            return exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
        })
    };

    input.click();
}

function sincronizarTodosDocumentoClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteSincronizarTodosDocumentoDestaCarga, function () {
        sincronizarTodosDocumento({ Carga: _cancelamento.Carga.codEntity(), CancelamentoCarga: _cancelamento.Codigo.val() });
    });
}

function sincronizarTodosDocumento(data) {
    executarReST("CancelamentoCargaCTe/SincronizarLoteDocumentoEmProcessamento", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridCTe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}