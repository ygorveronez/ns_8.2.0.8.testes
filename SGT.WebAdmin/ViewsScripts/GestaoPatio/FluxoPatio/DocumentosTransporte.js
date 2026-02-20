/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="FluxoPatio.js" />

// #region Objetos Globais do Arquivo

var _documentosTransporteFluxoPatio;
var _botoesDocumentosTransporteFluxoPatio;
var _documentosTransporteFluxoPatioPreenchimento;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DocumentosTransporteFluxoPatio = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.PreCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.NumeroCarga = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Carga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.NumeroPreCarga = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.PreCarga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.CargaData = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Data.getFieldDescription(), val: ko.observable(""), def: "" });
    this.CargaHora = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Hora.getFieldDescription(), val: ko.observable(""), def: "" });

    this.CodigoIntegracaoDestinatario = PropertyEntity({ val: ko.observable(""), def: "" });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Destinatario.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Remetente = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Fornecedor.getFieldDescription(), val: ko.observable(""), def: "" });
    this.TipoCarga = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.TipoDeCarga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.TipoOperacao = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.TipoDaOperacao.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Transportador = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Transportador.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Veiculo.getFieldDescription(), val: ko.observable(""), def: "" });
};

var DocumentosTransporteFluxoPatioPreenchimento = function () {
    this.NumeroCTe = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.NumeroDoCTe.getFieldDescription(), val: ko.observable(""), def: "", required: false, enable: ko.observable(true), maxlength: 50 });
    this.NumeroMDFe = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.NumeroDoMDFe.getFieldDescription(), val: ko.observable(""), def: "", required: false, enable: ko.observable(true), maxlength: 50 });
    this.Brix = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Brix.getFieldDescription(), getType: typesKnockout.decimal, required: false, enable: ko.observable(true), maxlength: 50 });
    this.Ratio = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Ratio.getFieldDescription(), getType: typesKnockout.decimal, required: false, enable: ko.observable(true), maxlength: 50 });
    this.Oleo = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Oleo.getFieldDescription(), getType: typesKnockout.decimal, required: false, enable: ko.observable(true), maxlength: 50 });
};

var BotoesDocumentosTransporteFluxoPatio = function () {
    this.AvancarEtapa = PropertyEntity({ eventClick: avancarEtapaDocumentosTransporteClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(false) });
    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaDocumentosTransporteClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ObservacoesDaEtapa, visible: _configuracaoGestaoPatio.HabilitarObservacaoEtapa });
    this.ReabrirFluxo = PropertyEntity({ eventClick: reabrirFluxoDocumentosTransporteClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ReabrirFluxo, visible: ko.observable(false) });
    this.RejeitarEtapa = PropertyEntity({ eventClick: rejeitarEtapaDocumentosTransporteClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.RejeitarEtapa, visible: ko.observable(false) });
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaDocumentosTransporteClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadDocumentosTransporteFluxoPatio() {
    _documentosTransporteFluxoPatio = new DocumentosTransporteFluxoPatio();
    KoBindings(_documentosTransporteFluxoPatio, "knockoutDocumentosTransporteFluxoPatio");

    _botoesDocumentosTransporteFluxoPatio = new BotoesDocumentosTransporteFluxoPatio();
    KoBindings(_botoesDocumentosTransporteFluxoPatio, "knockoutBotoesDocumentosTransporteFluxoPatio");

    _documentosTransporteFluxoPatioPreenchimento = new DocumentosTransporteFluxoPatioPreenchimento();
    KoBindings(_documentosTransporteFluxoPatioPreenchimento, "knockoutDocumentosTransporteFluxoPatioPreenchimento");

    if (_configuracaoGestaoPatio.OcultarTransportador)
        _documentosTransporteFluxoPatio.Transportador.visible(false);

    if (!_cargaAtual) {
        _cargaAtual = new Carga();
        _cargaAtual.CargaSVM.val(false);
    }

    _cargaCTe = new CargaCTEs();
    _cargaCTe.CTesSemSubContratacaoFilialEmissora.val(false);
    _cargaCTe.Carga = _documentosTransporteFluxoPatio.Carga;
    KoBindings(_cargaCTe, "knockoutDocumentosTransporteFluxoPatioCTes");

    buscarCargasCTe(() => {
        _cargaCTe.DownloadLoteXMLCTe.visible(true);
        _cargaCTe.DownloadLoteDACTE.visible(true);
        _cargaCTe.DownloadLoteDocumentos.visible(true);
    }, obterMenuOpcoesCargasCTeFluxoPatio());
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function avancarEtapaDocumentosTransporteClick() {
    executarReST("DocumentosTransporte/AvancarEtapa", { Codigo: _documentosTransporteFluxoPatio.Codigo.val(), NumeroCTe: _documentosTransporteFluxoPatioPreenchimento.NumeroCTe.val(), NumeroMDFe: _documentosTransporteFluxoPatioPreenchimento.NumeroMDFe.val(), Brix: _documentosTransporteFluxoPatioPreenchimento.Brix.val(), Ratio: _documentosTransporteFluxoPatioPreenchimento.Ratio.val(), Oleo: _documentosTransporteFluxoPatioPreenchimento.Oleo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.DocumentosDeTransporteFinalizadosComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesDocumentosTransporte();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function observacoesEtapaDocumentosTransporteClick() {
    buscarObservacoesEtapa(EnumEtapaFluxoGestaoPatio.DocumentosTransporte);
}

function reabrirFluxoDocumentosTransporteClick() {
    executarReST("DocumentosTransporte/ReabrirFluxo", { Codigo: _documentosTransporteFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.FluxoReabertoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesDocumentosTransporte();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function rejeitarEtapaDocumentosTransporteClick() {
    executarReST("DocumentosTransporte/RejeitarEtapa", { Codigo: _documentosTransporteFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.DocumentosDeTransporteRejeitadoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesDocumentosTransporte();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function voltarEtapaDocumentosTransporteClick() {
    executarReST("DocumentosTransporte/VoltarEtapa", { Codigo: _documentosTransporteFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRetornadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesDocumentosTransporte();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirDetalhesDocumentosTransporte(knoutFluxo, opt) {
    _fluxoAtual = knoutFluxo;

    executarReST("DocumentosTransporte/BuscarPorCodigo", { FluxoGestaoPatio: knoutFluxo.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _botoesDocumentosTransporteFluxoPatio.AvancarEtapa.visible(false);
                _botoesDocumentosTransporteFluxoPatio.ReabrirFluxo.visible(false);
                _botoesDocumentosTransporteFluxoPatio.RejeitarEtapa.visible(false);
                _botoesDocumentosTransporteFluxoPatio.VoltarEtapa.visible(false);

                PreencherObjetoKnout(_documentosTransporteFluxoPatio, retorno);

                var permiteEditarEtapa = VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarDocumentoTransporte, _PermissoesPersonalizadasFluxoPatio) && retorno.Data.PermitirEditarEtapa;

                _documentosTransporteFluxoPatioPreenchimento.NumeroCTe.enable(permiteEditarEtapa);
                _documentosTransporteFluxoPatioPreenchimento.NumeroMDFe.enable(permiteEditarEtapa);
                _botoesDocumentosTransporteFluxoPatio.AvancarEtapa.visible(permiteEditarEtapa);
                _documentosTransporteFluxoPatioPreenchimento.Brix.enable(permiteEditarEtapa);
                _documentosTransporteFluxoPatioPreenchimento.Oleo.enable(permiteEditarEtapa);
                _documentosTransporteFluxoPatioPreenchimento.Ratio.enable(permiteEditarEtapa);

                if (_configuracaoGestaoPatio.DocumentosTransportePermiteVoltar && permiteEditarEtapa) {
                    var primeiraEtapa = _fluxoAtual.EtapaAtual.val() == 0;

                    _botoesDocumentosTransporteFluxoPatio.VoltarEtapa.visible(!primeiraEtapa)

                    if (knoutFluxo.EtapaFluxoGestaoPatioAtual.val() == EnumEtapaFluxoGestaoPatio.DocumentosTransporte) {
                        _botoesDocumentosTransporteFluxoPatio.RejeitarEtapa.visible(_configuracaoGestaoPatio.PermitirRejeicaoFluxo && _fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Aguardando);
                        _botoesDocumentosTransporteFluxoPatio.ReabrirFluxo.visible(_fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Rejeitado);
                    }
                }

                exibirModalDetalhesDocumentosTransporte();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirModalDetalhesDocumentosTransporte() {
    if (edicaoEtapaFluxoPatioBloqueada())
        ocultarBotoesEtapa(_documentosTransporteFluxoPatio);

    Global.abrirModal('divModalDocumentosTransporteFluxoPatio');
    _gridCargaCTe.CarregarGrid();

    $(window).one('keyup', function (e) {
        if (e.keyCode == 27)
            fecharModalDetalhesDocumentosTransporte();
    });

    $("#divModalDocumentosTransporteFluxoPatio").one('hidden.bs.modal', function () {
        LimparCampos(_documentosTransporteFluxoPatio);
    });
}

function fecharModalDetalhesDocumentosTransporte() {
    Global.fecharModal('divModalDocumentosTransporteFluxoPatio');
}

function obterMenuOpcoesCargasCTeFluxoPatio() {
    var baixarEDI = { descricao: Localization.Resources.Cargas.Carga.BaixarEdi, id: guid(), metodo: baixarEDIClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadEDI };
    var baixarXMLNFSe = { descricao: Localization.Resources.Cargas.Carga.BaixarXml, id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadDANFSE };
    var baixarDANFSE = { descricao: Localization.Resources.Cargas.Carga.BaixarDanfse, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadDANFSE };
    var baixarDACTEComp = { descricao: Localization.Resources.Cargas.Carga.BaixarDacteComp, id: guid(), metodo: baixarDacteCompClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var baixarDACTE = { descricao: Localization.Resources.Cargas.Carga.BaixarDacte, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var baixarPDF = { descricao: Localization.Resources.Cargas.Carga.BaixarPdf, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeDownloadOutrosDoc };
    var baixarXML = { descricao: Localization.Resources.Cargas.Carga.BaixarXml, id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var retornoSefaz = { descricao: Localization.Resources.Cargas.Carga.MensagemSefaz, id: guid(), metodo: retoronoSefazClick, icone: "", visibilidade: VisibilidadeMensagemSefaz };
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarCTeClick, icone: "", visibilidade: VisibilidadeOpcaoEditar };
    var visualizar = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), metodo: detalhesCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var auditar = { descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), metodo: OpcaoAuditoria("CargaCTe"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var protocoloIntegracao = { descricao: Localization.Resources.Cargas.Carga.ProtocoloDeIntegracao, id: guid(), metodo: protocoloIntegracaoClick, icone: "", visibilidade: VisibilidadeProtocoloIntegracao };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [] };

    if (!_cargaAtual.PossuiOcultarInformacoesCarga.val()) {
        menuOpcoes.opcoes.push(baixarDACTE);
        menuOpcoes.opcoes.push(baixarXML);
        menuOpcoes.opcoes.push(baixarDANFSE);
        menuOpcoes.opcoes.push(baixarXMLNFSe);
        menuOpcoes.opcoes.push(baixarPDF);
        menuOpcoes.opcoes.push(visualizar);
        menuOpcoes.opcoes.push(baixarDACTEComp);
    }

    menuOpcoes.opcoes.push(baixarEDI);
    menuOpcoes.opcoes.push(retornoSefaz);
    menuOpcoes.opcoes.push(editar);
    menuOpcoes.opcoes.push(auditar);
    menuOpcoes.opcoes.push(protocoloIntegracao);

    return menuOpcoes;
}

// #endregion Funções Privadas
