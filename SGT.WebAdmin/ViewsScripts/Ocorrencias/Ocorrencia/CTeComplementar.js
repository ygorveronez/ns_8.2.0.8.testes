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
/// <reference path="../../Enumeradores/EnumSituacaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumTipoEnvioXMLOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoEmissao.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/SerieTransportador.js" />
/// <reference path="Ocorrencia.js" />
/// <reference path="EtapasOcorrencia.js" />
/// <reference path="DocumentoComplementar.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCTeComplementar = null, _CodigoCTeCancelamento = null, _cartaCorrecaoCTe = null;
var _protocoloIntegracao;
var _edicaoOutrosDocumentos;

var ProtocoloIntegracao = function () {
    this.CodigoProtocolo = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Protocolo.getFieldDescription(), val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: true });
};

var EdicaoOutrosDocumentos = function () {
    this.CodigoCTe = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoOcorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Numero = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Numero.getRequiredFieldDescription(), val: ko.observable(0), def: 0, getType: typesKnockout.int, required: true });
    this.Serie = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Serie.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Empresa.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });

    this.Confirmar = PropertyEntity({ eventClick: confirmarEdicaoOutrosDocumentosClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar });
    this.Cancelar = PropertyEntity({ eventClick: cancelarEdicaoOutrosDocumentosClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar });
};

//*******EVENTOS*******

function carregarGridCTeComplementares() {

    _protocoloIntegracao = new ProtocoloIntegracao();
    KoBindings(_protocoloIntegracao, "knoutModalProtocoloIntegracao");

    _edicaoOutrosDocumentos = new EdicaoOutrosDocumentos();
    KoBindings(_edicaoOutrosDocumentos, "knockoutModalEditarOutrosDocumentos");

    new BuscarSeriesCTeTransportador(_edicaoOutrosDocumentos.Serie, null, null, null, null, _edicaoOutrosDocumentos.Empresa);

    var baixarXMLNFSe = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.BaixarXML, id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadDANFSE };
    var baixarDANFSE = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.BaixarDANFSE, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadDANFSE };
    var EnviarXMLCancelamento = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.EnviarXMLCancelamento, id: guid(), metodo: enviarXMLCancelamentoClick, icone: "", visibilidade: VisibilidadeEnviarXMLCancelamento };
    var baixarDACTE = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.BaixarDACTE, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var baixarPDF = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.BaixarPDF, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeDownloadOutrosDoc };
    var baixarXML = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.BaixarXML, id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var retornoSefaz = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.MensagemSefaz, id: guid(), metodo: retoronoSefazClick, icone: "", visibilidade: VisibilidadeMensagemSefaz };
    var emitir = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Emitir, id: guid(), metodo: function (datagrid) { emitirCTeClick(datagrid); }, icone: "", visibilidade: VisibilidadeRejeicao };
    let sincronizarDocumento = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.SincronizarDocumento, id: guid(), metodo: function (datagrid) { sincronizarCTeClick(datagrid); }, icone: "", visibilidade: VisibilidadeSincronizarDocumento };
    var emitirCCe = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.CartaCorrecao, id: guid(), metodo: EmitirCartaCorrecaoCTeClick, icone: "", visibilidade: VisibilidadeOpcaoCartaCorrecaoCTe };
    var visualizar = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Detalhes, id: guid(), metodo: detalhesCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var protocoloIntegracao = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.ProtocoloIntegracao, id: guid(), metodo: protocoloIntegracaoClick, icone: "", visibilidade: VisibilidadeProtocoloIntegracao };
    var editarOutrosDocumentos = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.EditarOutrosDocumentos, id: guid(), metodo: editarOutrosDocumentosOcorrenciaClick, icone: "", visibilidade: VisibilidadeOpcaoTipoDocumentoOutros };
    var autorizarOutrosDocumentos = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.AutorizarOutrosDocumentos, id: guid(), metodo: autorizarOutrosDocumentosOcorrenciaClick, icone: "", visibilidade: VisibilidadeOpcaoTipoDocumentoOutros };

    var menuOpcoes = null;

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Terceiros) {
        menuOpcoes = {
            tipo: TypeOptionMenu.list, descricao: Localization.Resources.Ocorrencias.Ocorrencia.Opcoes, tamanho: 7,
            opcoes: [baixarDACTE, baixarDANFSE, baixarXML, baixarXMLNFSe, baixarPDF, retornoSefaz, visualizar, emitir, sincronizarDocumento, EnviarXMLCancelamento, protocoloIntegracao, editarOutrosDocumentos, autorizarOutrosDocumentos]
        };

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_EmitirCartaCorrecao, _PermissoesPersonalizadasOcorrencia))
            menuOpcoes.opcoes.push(emitirCCe);
    } else {
        menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Ocorrencias.Ocorrencia.Opcoes, tamanho: 7, opcoes: [baixarDACTE, baixarDANFSE, baixarXML, baixarXMLNFSe, baixarPDF, retornoSefaz, emitir, sincronizarDocumento] };
    }

    _gridCTeComplementar = new GridView(_documentoComplementar.CTesComplementares.idGrid, "Ocorrencia/ConsultarCTesOcorrencia", _ocorrencia, menuOpcoes, null, null, null, null, null, null, 10);
}

function EmitirCartaCorrecaoCTeClick(data) {
    if (_cartaCorrecaoCTe === null)
        _cartaCorrecaoCTe = new CartaCorrecaoCTe();

    _cartaCorrecaoCTe.Load(data.CodigoCTE);
}

function baixarXMLCTeClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadXML", data);
}

function baixarDacteClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa, CodigoOcorrencia: _ocorrencia.Codigo.val() };
    executarDownload("CargaCTe/DownloadDacte", data);
}

function detalhesCTeClick(e, sender) {
    var codigo = parseInt(e.CodigoCTE);
    exibirDetalhesCTe(codigo);
}

function exibirDetalhesCTe(codigo) {
    var permissoesSomenteLeituraCTe = new Array();
    permissoesSomenteLeituraCTe.push(EnumPermissoesEdicaoCTe.Nenhuma);
    var instancia = new EmissaoCTe(codigo, function () {
        instancia.CRUDCTe.Emitir.visible(false);
        instancia.CRUDCTe.Salvar.visible(false);
        instancia.CRUDCTe.Salvar.eventClick = function () {
            var objetoCTe = ObterObjetoCTe(instancia);
            SalvarCTe(objetoCTe, e.Codigo, instancia);
        }
    }, permissoesSomenteLeituraCTe);
}

function selecionarNotasComplementares(dataRow, row, table) {
    carregarGridNotasComplementar(dataRow, row, table);
    Global.abrirModal("divModalNotasComplementares");
}

//*******MÉTODOS*******

function enviarXMLCancelamentoClick(e) {
    _CodigoCTeCancelamento = e.CodigoCTE;
    _TipoEnvioXMLOcorrencia = EnumTipoEnvioXMLOcorrencia.xmlCancelamento;
    $("#" + _knoutArquivo.Arquivo.id).trigger("click");
}

function enviarXMLCancelamento(formData) {
    var data = {
        CodigoCTe: _CodigoCTeCancelamento
    };
    enviarArquivo("CTe/EnviarXMLCancelamentoCTe?callback=?", data, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarDocumentosComplementares(_ocorrencia.CTeFilialEmissora.val());
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function emitirCTeClick(e) {
    if (e.Status == "Rejeição") {
        var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
        executarReST("CargaCTe/EmitirNovamente", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    BuscarDocumentosComplementares(_ocorrencia.CTeFilialEmissora.val());
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Ocorrencias.Ocorrencia.SituacaoNaoPermiteEmissao, Localization.Resources.Ocorrencias.Ocorrencia.AtualSituacaoCTeNaoPermiteQueEleSejaEmitidoNovamente.format(e.Status));
    }
}

function sincronizarCTeClick(e) {
    if (e.SituacaoCTe == EnumStatusCTe.ENVIADO || e.SituacaoCTe == EnumStatusCTe.EMCANCELAMENTO) {
        var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
        executarReST("CargaCTe/SincronizarDocumentoEmProcessamento", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    BuscarDocumentosComplementares(_ocorrencia.CTeFilialEmissora.val());
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.SituacaoNaoPermiteEmissao, Localization.Resources.Cargas.Carga.AtualSituacaoDoCteNaoPermiteQueEleSejaEmitidoNovamente.format(e.Status));
    }
}

function protocoloIntegracaoClick(data) {
    LimparDadosProtocoloIntegracao();

    _protocoloIntegracao.CodigoProtocolo.val(data.CodigoCTE);

    Global.abrirModal('divModalProtocoloIntegracao');
}

function editarOutrosDocumentosOcorrenciaClick(data) {
    LimparCamposEdicaoOutrosDocumentos();

    _edicaoOutrosDocumentos.CodigoCTe.val(data.CodigoCTE);
    _edicaoOutrosDocumentos.CodigoOcorrencia.val(_ocorrencia.Codigo.val());
    _edicaoOutrosDocumentos.Numero.val(data.Numero);
    _edicaoOutrosDocumentos.Serie.codEntity(data.CodigoSerie);
    _edicaoOutrosDocumentos.Serie.val(data.Serie);
    _edicaoOutrosDocumentos.Empresa.codEntity(data.CodigoEmpresa);
    _edicaoOutrosDocumentos.Empresa.val(data.Empresa);

    Global.abrirModal("divModalEditarOutrosDocumentos");
}

function autorizarOutrosDocumentosOcorrenciaClick(data) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.DesejaRealmenteProsseguir, function () {
        var dados = { CodigoCTe: data.CodigoCTE, CodigoOcorrencia: _ocorrencia.Codigo.val() };
        executarReST("OcorrenciaEmissao/AutorizarOutrosDocumentos", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AutorizadoSucesso);
                    BuscarDocumentosComplementares(_ocorrencia.CTeFilialEmissora.val());
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function confirmarEdicaoOutrosDocumentosClick() {
    Salvar(_edicaoOutrosDocumentos, "OcorrenciaEmissao/SalvarOutrosDocumentos", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AutorizadoSucesso);
                Global.fecharModal("divModalEditarOutrosDocumentos");
                LimparCamposEdicaoOutrosDocumentos();

                BuscarDocumentosComplementares(_ocorrencia.CTeFilialEmissora.val());
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function cancelarEdicaoOutrosDocumentosClick() {
    Global.fecharModal("divModalEditarOutrosDocumentos");
}

function VisibilidadeMensagemSefaz(data) {
    if (data.RetornoSefaz != "" && (data.NumeroModeloDocumentoFiscal == "57" || data.NumeroModeloDocumentoFiscal == "39")) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeEnviarXMLCancelamento(data) {
    if (_ocorrencia.CTeEmitidoNoEmbarcador.val() && _ocorrencia.SituacaoOcorrencia.val() == EnumSituacaoOcorrencia.RejeicaoCancelamento)
        return true;
    else
        return false;
}

function VisibilidadeRejeicao(data) {
    if (data.SituacaoCTe == EnumStatusCTe.REJEICAO) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeSincronizarDocumento(data) {
    if (data.HabilitarSincronizarDocumento == true) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeOpcaoDownload(data) {
    if (data.CodigoCTE > 0 && (data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO) && data.NumeroModeloDocumentoFiscal == "57") {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeOpcaoDownloadDANFSE(data) {
    if (data.CodigoCTE > 0 && ((data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.ANULADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO) && data.NumeroModeloDocumentoFiscal == "39")) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeDownloadOutrosDoc(data) {
    return data.CodigoCTE > 0 && data.NumeroModeloDocumentoFiscal != "57" && data.NumeroModeloDocumentoFiscal != "39" && data.SituacaoCTe == EnumStatusCTe.AUTORIZADO;
}

function VisibilidadeOpcaoEditar(data) {
    if (data.CodigoCTE > 0 && ((data.SituacaoCTe == EnumStatusCTe.EMDIGITACAO || data.SituacaoCTe == EnumStatusCTe.REJEICAO) && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeOpcaoNotasComplementares(data) {
    if (data.CodigoCTE > 0 && data.CTeGlobalizado) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeOpcaoCartaCorrecaoCTe(data) {
    return data.SituacaoCTe == EnumStatusCTe.AUTORIZADO && data.NumeroModeloDocumentoFiscal == "57";
}

function VisibilidadeProtocoloIntegracao(data) {
    return _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && (data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.NFSe || data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.CTe);
}

function VisibilidadeOpcaoTipoDocumentoOutros(data) {
    return data.SituacaoCTe == EnumStatusCTe.EMDIGITACAO && data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.Outros && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS;
}

function retoronoSefazClick(e, sender) {
    $('#PMensagemRetornoSefaz').html(e.RetornoSefaz);
    Global.abrirModal('divModalRetornoSefaz');
}

function LimparDadosProtocoloIntegracao() {
    LimparCampo(_protocoloIntegracao);
}

function LimparCamposEdicaoOutrosDocumentos() {
    LimparCampo(_edicaoOutrosDocumentos);
}