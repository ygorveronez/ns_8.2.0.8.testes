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
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="Ocorrencia.js" />
/// <reference path="EtapasOcorrencia.js" />
/// <reference path="CTeComplementar.js" />
/// <reference path="NFSComplementar.js" />
/// <reference path="PreCTeComplemento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _documentoComplementar;

var DocumentoComplementar = function () {
    this.DocumentoComplementar = PropertyEntity({ type: types.local, visible: ko.observable(false), text: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.CTeComplementar), message: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.NaoENecessarioEmitirDocumentosComplementares) });
    this.CTesComplementares = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(false) });
    this.NFSManualComplementar = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(false) });

    this.BuscarNovamenteDocumentosComplementares = PropertyEntity({ eventClick: function () { BuscarDocumentosComplementares(_ocorrencia.CTeFilialEmissora.val()); }, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.BuscarAtualizar, visible: ko.observable(true) });
    this.EmitirNovamenteRejeitados = PropertyEntity({ eventClick: reemitirCTesRejeitadosClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.ReemitirCTesRejeitados, visible: ko.observable(false) });
    this.AutorizarOutrosDocumentos = PropertyEntity({ eventClick: autorizarCTesOutrosDocumentosClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.AutorizarOutrosDocumentos, visible: ko.observable(false) });

    this.ImportarCTes = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(false) });
    this.PreCTesComplementares = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(false) });
    this.Dropzone = PropertyEntity({ type: types.local, idTab: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.MensagemPendencia = _ocorrencia.MensagemPendencia;
    this.ReenviarIntegracao = PropertyEntity({ eventClick: function (e, sender) { reenviarIntegracao(e, sender); }, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.ReenviarIntegracao, visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadCTeComplementar() {
    _documentoComplementar = new DocumentoComplementar();
    KoBindings(_documentoComplementar, "knockoutDocumentoComplementar");
    carregarGridCTeComplementares();
    carregarGridNFSManualComplementar();
    carregarGridPreCTeComplementares();
}

//*******MÉTODOS*******

function BuscarDocumentosComplementares(cteFilialEmissora) {
    var exibirAprovadoresAutomatico = (_ocorrencia.SituacaoOcorrencia.val() == EnumSituacaoOcorrencia.AgAutorizacaoEmissao)

    _ocorrencia.CTeFilialEmissora.val(cteFilialEmissora);

    _ocorrenciaAutorizacaoEmissao.UsuariosAutorizadores.visible(false);
    _ocorrenciaAutorizacaoEmissao.SituacaoSolicitacaoCredito.visible(false);

    _documentoComplementar.CTesComplementares.visible(false);
    _documentoComplementar.NFSManualComplementar.visible(false);
    _documentoComplementar.PreCTesComplementares.visible(false);
    _documentoComplementar.DocumentoComplementar.visible(false);
    _documentoComplementar.DocumentoComplementar.message(Localization.Resources.Ocorrencias.Ocorrencia.NaoENecessarioEmitirDocumentosComplementares);

    if (_ocorrencia.PossuiNFSManual.val()) {
        PreencherGridNFSManualComplementar();
    }

    if (!_ocorrencia.AgImportacaoCTe.val() || cteFilialEmissora) {
        if (_ocorrencia.ComponenteFrete.codEntity() > 0) {
            if (_ocorrencia.DadosModeloDocumentoFiscal.val() != null) {
                if (_ocorrencia.DadosModeloDocumentoFiscal.val().Numero == "39")
                    PreencherGridDocumentosComplementar();
                else if (_ocorrencia.DadosModeloDocumentoFiscal.val().Numero == "57")
                    PreencherGridCTesComplementar();
                else {
                    PreencherGridCTesComplementar();
                    _documentoComplementar.DocumentoComplementar.text(Localization.Resources.Ocorrencias.Ocorrencia.ZeroComplementar.format(_ocorrencia.DadosModeloDocumentoFiscal.val().Abreviacao));
                }
            }
            else
                PreencherGridCTesComplementar();
        }
    } else {
        PreencherGridPreCTesComplementar();
    }

    AtualizarDadosControleSaldo();

    if (exibirAprovadoresAutomatico)
        exibirAprovadoresClick();
}

function PreencherEtapaEmissaoDocumentoComplementar() {
    //if ((_ocorrencia.SituacaoOcorrencia.val() == EnumSituacaoOcorrencia.AgEmissaoCTeComplementar || _ocorrencia.SituacaoOcorrencia.val() == EnumSituacaoOcorrencia.EmEmissaoCTeComplementar) && _ocorrencia.EmiteNFSeFora.val()) {
    if (_ocorrencia.SituacaoOcorrencia.val() == EnumSituacaoOcorrencia.EmEmissaoCTeComplementar && _ocorrencia.EmiteNFSeFora.val()) {
        _documentoComplementar.Dropzone.visible(false);
    }
}

function PreencherGridDocumentosComplementar() {
    _documentoComplementar.DocumentoComplementar.visible(true);
    _documentoComplementar.DocumentoComplementar.text("Documentos Complementar");
    _gridCTeComplementar.CarregarGrid();
    _documentoComplementar.CTesComplementares.visible(true);
}

function PreencherGridNFSManualComplementar() {
    _documentoComplementar.CTesComplementares.visible(false);
    _documentoComplementar.NFSManualComplementar.visible(true);
    _gridNFSManualComplementar.CarregarGrid();

    if (!_ocorrencia.NFSManualPendenteGeracao.val()) {
        PreencherGridCTesComplementar();
        _documentoComplementar.CTesComplementares.visible(true);
    }
}

function PreencherGridCTesComplementar() {
    _documentoComplementar.EmitirNovamenteRejeitados.visible(false);
    _documentoComplementar.AutorizarOutrosDocumentos.visible(false);

    _documentoComplementar.DocumentoComplementar.text(Localization.Resources.Ocorrencias.Ocorrencia.CTeComplementar);
    _gridCTeComplementar.CarregarGrid(function (retornoGrid) {
        if ((retornoGrid.data.length > 0) || (_ocorrencia.TipoOcorrencia.tipoEmissaoDocumentoOcorrencia == EnumTipoEmissaoDocumentoOcorrencia.Todos)) {
            _documentoComplementar.DocumentoComplementar.visible(true);
            _documentoComplementar.EmitirNovamenteRejeitados.visible(retornoGrid.data.some(elemento => elemento.SituacaoCTe == "R"));
            _documentoComplementar.AutorizarOutrosDocumentos.visible(retornoGrid.data.some(elemento => elemento.SituacaoCTe == "S"));
        }
        else {
            if (_ocorrencia.CTeFilialEmissora.val() && (_ocorrencia.TipoOcorrencia.tipoEmissaoDocumentoOcorrencia == EnumTipoEmissaoDocumentoOcorrencia.SomenteSubcontratada))
                _documentoComplementar.DocumentoComplementar.message(Localization.Resources.Ocorrencias.Ocorrencia.EsseTipoOcorrenciaNaoEmiteComplementoCTeFilialEmissora);
            else if (!_ocorrencia.CTeFilialEmissora.val() && (_ocorrencia.TipoOcorrencia.tipoEmissaoDocumentoOcorrencia == EnumTipoEmissaoDocumentoOcorrencia.SomenteFilialEmissora))
                _documentoComplementar.DocumentoComplementar.message(Localization.Resources.Ocorrencias.Ocorrencia.EsseTipoOcorrenciaNaoEmiteComplementoCTeSubcontratada);
            else
                _documentoComplementar.DocumentoComplementar.visible(true);
        }

        _documentoComplementar.CTesComplementares.visible(true);
    });
}

function LimparCamposDocumentosComplementar() {
    _documentoComplementar.ReenviarIntegracao.visible(false);
    _documentoComplementar.PreCTesComplementares.visible(false);
    _documentoComplementar.DocumentoComplementar.visible(false);
    LimparCampos(_documentoComplementar);
}

function reemitirCTesRejeitadosClick() {
    executarReST("CargaCTe/ReemitirCTesRejeitadosOcorrencia", { Codigo: _ocorrencia.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                BuscarDocumentosComplementares(_ocorrencia.CTeFilialEmissora.val());
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function autorizarCTesOutrosDocumentosClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.DesejaRealmenteProsseguir, function () {
        executarReST("OcorrenciaEmissao/AutorizarOutrosDocumentosPorOcorrencia", { CodigoOcorrencia: _ocorrencia.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AutorizadoSucesso);
                    BuscarDocumentosComplementares(_ocorrencia.CTeFilialEmissora.val());
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    });
}





// INTEGRACOES
function CarregarIntegracoes(data) {
    if (data.EmiteNFSeFora && data.ErroIntegracaoComGPA) {
        _documentoComplementar.ReenviarIntegracao.visible(true);
        $("#" + _etapaOcorrencia.Etapa3.idTab + " .step").attr("class", "step red");
    }
}

function reenviarIntegracao(e, sender) {
    executarReST("Ocorrencia/ReenviarIntegracao", { Codigo: _ocorrencia.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.Ocorrencia.ReenviarIntegracao, Localization.Resources.Ocorrencias.Ocorrencia.IntegracaoReenviadaComSucesso);
                _documentoComplementar.ReenviarIntegracao.visible(false);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}