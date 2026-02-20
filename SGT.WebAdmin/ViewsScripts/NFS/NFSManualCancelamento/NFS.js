/// <reference path="../../Enumeradores/EnumSituacaoNFS.js" />
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
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="Ocorrencia.js" />
/// <reference path="EtapasOcorrencia.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _nfs;
var _gridNFS;

var NFS = function () {
    this.LancamentoNFSManual = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.BuscarNFSs = PropertyEntity({ eventClick: ConsultarNFSsCarga, type: types.event, text: "Buscar / Atualizar MDF-e(s)", visible: ko.observable(true), idGrid: guid() });
}

//*******EVENTOS*******

function LoadNFS() {
    _nfs = new NFS();
    KoBindings(_nfs, "knockoutNFS");
}

function ConsultarNFSsCarga(e) {
    _nfs.LancamentoNFSManual.val(_cancelamento.LancamentoNFSManual.codEntity());

    var baixarDANFSE = { descricao: "Baixar DANFSE", id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadNFS };
    var baixarXML = { descricao: "Baixar XML", id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadNFS };
    var desabilitarNFS = { descricao: "Desabilitar NFS Manual", id: guid(), metodo: DesabilitarNFSManualClick, icone: "", visibilidade: VisibilidadeOpcaoDesabilitarNFS };
    var auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("ConhecimentoDeTransporteEletronico"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var reenviarCancelamento = { descricao: "Reenviar Cancelamento", id: guid(), metodo: ReenviarCancelamentoNFSeClick, icone: "", visibilidade: VisibilidadeReenviarCancelamento };
    var informarCancelamentoPrefeitura = { descricao: "Informar Cancelamento Prefeitura", id: guid(), metodo: InformarCancelamentoPrefeituraClick, icone: "", visibilidade: VisibilidadeReenviarCancelamento };
    
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [baixarDANFSE, baixarXML, desabilitarNFS, reenviarCancelamento, informarCancelamentoPrefeitura,  auditar], tamanho: 7 };

    _gridNFS = new GridView(_nfs.BuscarNFSs.idGrid, "NFSManual/ConsultarNFS", _nfs, menuOpcoes);

    _gridNFS.CarregarGrid();
}

function baixarXMLCTeClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadXML", data);
}

function baixarDacteClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadDacte", data);
}

function DesabilitarNFSManualClick(e) {
    executarReST("NFSManualCancelamentoDocumento/DesabilitarDocumento", { Codigo: e.CodigoCTE }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "NFS desabilitada com sucesso.");
                _gridNFS.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function ReenviarCancelamentoNFSeClick(e) {
    executarReST("NFSManualCancelamento/Reenviar", { Codigo: _cancelamento.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelamento reenviado.");
                _gridNFS.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function InformarCancelamentoPrefeituraClick(e) {
    executarReST("NFSManualCancelamento/InformarCancelamentoPrefeitura", { Codigo: _cancelamento.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelamento na prefeitura informado.");
                _gridNFS.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

//*******MÉTODOS*******

function LimparCamposNFS() {
    LimparCampos(_nfs);
}

function VisibilidadeOpcaoDownloadNFS(data) {
    if ((data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.ANULADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO)) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeOpcaoDesabilitarNFS(data) {
    return (data.SituacaoCTe === EnumStatusCTe.ANULADO || data.SituacaoCTe === EnumStatusCTe.CANCELADO) && data.Desabilitado === false;
}

function VisibilidadeReenviarCancelamento(data) {
    return (data.SituacaoCTe === EnumStatusCTe.AUTORIZADO);
}
