/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="CancelamentoOcorrencia.js" />
/// <reference path="DadosCancelamento.js" />
/// <reference path="Documentos.js" />
/// <reference path="EtapasCancelamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _documentos;
var _gridDocumentos;
var _knoutArquivo;
var _CodigoCTeCancelamento;
var _fileCTeCancelamento;

var Arquivo = function () {
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), required: false, visible: ko.observable(false) });
}
var Documentos = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Documentos = PropertyEntity({ type: types.map, idGrid: guid() });
    this.MensagemRejeicaoCancelamento = PropertyEntity({ type: types.map });
    this.BuscarNovamenteDocumentosComplementares = PropertyEntity({ eventClick: BuscarNovamenteDocumentosComplementaresClick, type: types.event, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.BuscarAtualizar, visible: ko.observable(false) });
    this.LiberarCancelamentoComCTeNaoInutilizado = PropertyEntity({ eventClick: LiberarCancelamentoComCTeNaoInutilizadoClick, type: types.event, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.LiberarCancelamentoSemInutilizarCTes, visible: ko.observable(false) });
    this.ReenviarCancelamento = PropertyEntity({ eventClick: ReenviarCancelamentoClick, type: types.event, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.ReenviarCancelamento, visible: ko.observable(false) });
    this.ReenviarCancelamentoComoAnulacao = PropertyEntity({ eventClick: ReenviarCancelamentoOcorrenciaComoAnulacaoClick, type: types.event, text: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.AnularOcorrencia, visible: ko.observable(false) });
}

//*******EVENTOS*******
function loadDocumentos() {
    _documentos = new Documentos();
    KoBindings(_documentos, "knockoutDocumentos");

    _knoutArquivo = new Arquivo();
    KoBindings(_knoutArquivo, "knoutEnviarArquivo");

    $("#" + _knoutArquivo.Arquivo.id).on("change", enviarXMLCTeClick);
    _fileCTeCancelamento = document.getElementById(_knoutArquivo.Arquivo.id);
    GerarGridDocumentos();
}


//*******MÉTODOS*******
function GerarGridDocumentos() {
    var baixarXMLNFSe = { descricao: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.BaixarXML, id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadDANFSE };
    var baixarDANFSE = { descricao: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.BaixarDANFSE, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadDANFSE };
    var baixarDACTE = { descricao: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.BaixarDACTE, id: guid(), metodo: BaixarDACTEClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadCTe };
    var baixarPDF = { descricao: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.BaixarPDF, id: guid(), metodo: BaixarPDFClick, icone: "", visibilidade: VisibilidadeDownloadOutrosDocumentosCTe };
    var baixarXML = { descricao: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.BaixarXML, id: guid(), metodo: BaixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadCTe };
    var enviarXMLCancelamento = { descricao: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.EnviarXMLCancelamento, id: guid(), metodo: enviarXMLCancelamentoClick, icone: "", visibilidade: VisibilidadeEnviarXMLCancelamentoOcorrencia };
    var retornoSefaz = { descricao: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.MensagemSefaz, id: guid(), metodo: retoronoSefazClick, icone: "", visibilidade: VisibilidadeMensagemSefaz };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Opcoes,
        tamanho: 7,
        opcoes: [baixarDACTE, baixarDANFSE, baixarPDF, baixarXML, baixarXMLNFSe, enviarXMLCancelamento, retornoSefaz]
    };

    var ko_documentos = {
        Codigo: _dadosCancelamento.Ocorrencia
    };

    _gridDocumentos = new GridView(_documentos.Documentos.idGrid, "OcorrenciaCancelamento/ConsultarCTesOcorrencia", ko_documentos, menuOpcoes);
}


function LiberarCancelamentoComCTeNaoInutilizadoClick() {
    exibirConfirmacao("Atenção!", Localization.Resources.Ocorrencias.OcorrenciaCancelamento.RealmenteDesejaLiberarCancelamentoSemInutilizar, function () {
        executarReST("OcorrenciaCancelamento/LiberarSemInutilizarCTes", { Codigo: _cancelamentoOcorrencia.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Sucesso, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.CancelamentoLiberadoSucesso);
                    BuscarCancelamentoPorCodigo(_cancelamentoOcorrencia.Codigo.val());
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Falha, arg.Msg);
            }
        });
    });
}

function BuscarNovamenteDocumentosComplementaresClick() {
    _gridDocumentos.CarregarGrid();
}

function ReenviarCancelamentoClick(e, sender) {
    executarReST("OcorrenciaCancelamento/Reenviar", { Codigo: _documentos.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Sucesso, (_dadosCancelamento.Tipo.val() == EnumTipoCancelamentoOcorrencia.Cancelamento ? Localization.Resources.Ocorrencias.OcorrenciaCancelamento.CancelamentoReenviado : Localization.Resources.Ocorrencias.OcorrenciaCancelamento.AnulacaoReenviada) + Localization.Resources.Ocorrencias.OcorrenciaCancelamento.ComSucesso);
                BuscarCancelamentoPorCodigo(_documentos.Codigo.val());
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Falha, r.Msg);
        }
    });
}

function ReenviarCancelamentoOcorrenciaComoAnulacaoClick(e, sender) {
    exibirConfirmacao("Atenção!", Localization.Resources.Ocorrencias.OcorrenciaCancelamento.DesejaRealmenteAnularEstaOcorrenciaIrreversivel, function () {
        executarReST("OcorrenciaCancelamento/ReenviarCancelamentoComoAnulacao", { Codigo: _documentos.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Sucesso, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.AnulacaoEnviadoComSucesso);
                    BuscarCancelamentoPorCodigo(_documentos.Codigo.val());
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Falha, r.Msg);
            }
        });
    });
}

function EditarDocumentos(data) {
    _dadosCancelamento.Codigo.val(data.Codigo);
    if (data.Documentos != null) {
        PreencherObjetoKnout(_documentos, { Data: data.Documentos });
        _gridDocumentos.CarregarGrid();
    }

    if (_cancelamentoOcorrencia.Situacao.val() == EnumSituacaoCancelamentoOcorrencia.RejeicaoCancelamento) {
        _documentos.ReenviarCancelamento.visible(true);

        if (_dadosCancelamento.Tipo.val() == EnumTipoCancelamentoOcorrencia.Cancelamento && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.OcorrenciaCancelamento_ReenviarCancelamentoComoAnulacao, _PermissoesPersonalizadasOcorrenciaCancelamento)) {
            _documentos.ReenviarCancelamentoComoAnulacao.visible(true);
        } else {
            _documentos.ReenviarCancelamentoComoAnulacao.visible(false);
        }
    }
    else {
        _documentos.ReenviarCancelamento.visible(false);
        _documentos.ReenviarCancelamentoComoAnulacao.visible(false);
    }
}

function enviarXMLCancelamentoClick(e) {
    _CodigoCTeCancelamento = e.CodigoCTE;
    $("#" + _knoutArquivo.Arquivo.id).trigger("click");
}
function enviarXMLCTeClick() {
    if (_fileCTeCancelamento.files.length > 0) {
        exibirConfirmacao(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Confirmacao, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.RealmenteDesejaEnviarAquivo.format(_fileCTeCancelamento.files[0].name), function () {
            var formData = new FormData();
            formData.append("upload", _fileCTeCancelamento.files[0]);
            var data = {
                CodigoCTe: _CodigoCTeCancelamento
            };
            _fileCTeCancelamento.value = null;
            enviarArquivo("CTe/EnviarXMLCancelamentoCTe?callback=?", data, formData, function (arg) {
                if (arg.Success) {
                    if (arg.Data !== false) {
                        _gridDocumentos.CarregarGrid();
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Atencao, arg.Msg, 20000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Falha, arg.Msg);
                }
            });
        });
    }
}


function VisibilidadeOpcaoDownloadDANFSE(data) {
    return ((data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.ANULADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO) && data.NumeroModeloDocumentoFiscal == "39");
}

function VisibilidadeOpcaoDownloadCTe(data) {
    return ((data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO || data.SituacaoCTe == EnumStatusCTe.ANULADO) && data.NumeroModeloDocumentoFiscal == "57");
}

function VisibilidadeDownloadOutrosDocumentosCTe(data) {
    return (data.NumeroModeloDocumentoFiscal != "57" && data.NumeroModeloDocumentoFiscal != "39");
}

function VisibilidadeEnviarXMLCancelamento(data) {
    return (_cancelamentoOcorrencia.Situacao.val() == EnumSituacaoCancelamentoOcorrencia.RejeicaoCancelamento && data.SituacaoCTe == EnumStatusCTe.AUTORIZADO);
}
function VisibilidadeEnviarXMLCancelamentoOcorrencia(data) {
    return (_cancelamentoOcorrencia.Situacao.val() == EnumSituacaoCancelamentoOcorrencia.RejeicaoCancelamento || EnumSituacaoCancelamentoOcorrencia.Cancelada && data.SituacaoCTe == EnumStatusCTe.AUTORIZADO);
}
function VisibilidadeMensagemSefaz(data) {
    return (data.RetornoSefaz != "" && (data.NumeroModeloDocumentoFiscal == "57" || data.NumeroModeloDocumentoFiscal == "39"));
}

function baixarXMLCTeClick(e) {
    executarDownload("CargaCTe/DownloadXML", { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa });
}

function baixarDacteClick(e) {
    executarDownload("CargaCTe/DownloadDacte", { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa });
}

function BaixarXMLCTeClick(e) {
    executarDownload("CancelamentoCargaCTe/DownloadXML", { CTe: e.CodigoCTE });
}

function BaixarDACTEClick(e) {
    executarDownload("CancelamentoCargaCTe/DownloadDACTE", { CTe: e.CodigoCTE });
}

function BaixarPDFClick(e) {
    executarDownload("CancelamentoCargaCTe/DownloadPDF", { CTe: e.CodigoCTE });
}

function retoronoSefazClick(e, sender) {
    $('#PMensagemRetornoSefaz').html(e.RetornoSefaz);
    Global.abrirModal('divModalRetornoSefaz');
}