/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="FluxoPatio.js" />

// #region Objetos Globais do Arquivo

var _documentoFiscalFluxoPatio;
var _numeroDocumentoFiscalFluxoPatio;
var _gridNumeroDocumentoFiscalFluxoPatio;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DocumentoFiscalFluxoPatio = function () {
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

    this.AdicionarNumerosDocumentos = PropertyEntity({ eventClick: adicionarNumeroDocumentoFiscalFluxoPatioModalClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(false), idGrid: guid() });
    this.AvancarEtapa = PropertyEntity({ eventClick: avancarEtapaDocumentoFiscalClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(false) });
    this.DetalhesPesagem = PropertyEntity({ eventClick: detalhesPesagemDocumentoFiscalClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.InformacoesDePesagem, visible: ko.observable(false) });
    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaDocumentoFiscalClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ObservacoesDaEtapa, visible: _configuracaoGestaoPatio.HabilitarObservacaoEtapa });
    this.ReabrirFluxo = PropertyEntity({ eventClick: reabrirFluxoDocumentoFiscalClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ReabrirFluxo, visible: ko.observable(false) });
    this.RejeitarEtapa = PropertyEntity({ eventClick: rejeitarEtapaDocumentoFiscalClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.RejeitarEtapa, visible: ko.observable(false) });
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaDocumentoFiscalClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, visible: ko.observable(false) });
};

var NumeroDocumentoFiscalFluxoPatio = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.NumeroDocumento = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.NumeroDoDocumento.getRequiredFieldDescription(), val: ko.observable(""), def: "", required: true, maxlength: 20 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarNumeroDocumentoFiscalFluxoPatioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarNumeroDocumentoFiscalFluxoPatioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirNumeroDocumentoFiscalFluxoPatioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadDocumentoFiscalFluxoPatio() {
    _documentoFiscalFluxoPatio = new DocumentoFiscalFluxoPatio();
    KoBindings(_documentoFiscalFluxoPatio, "knockoutDocumentoFiscalFluxoPatio");

    _numeroDocumentoFiscalFluxoPatio = new NumeroDocumentoFiscalFluxoPatio();
    KoBindings(_numeroDocumentoFiscalFluxoPatio, "knockoutNumeroDocumentoFiscalFluxoPatio");

    if (_configuracaoGestaoPatio.OcultarTransportador)
        _documentoFiscalFluxoPatio.Transportador.visible(false);

    loadGridDocumentoFiscalFluxoPatio();
}

function loadGridDocumentoFiscalFluxoPatio() {
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarNumeroDocumentoFiscalFluxoPatioClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoEditar] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "NumeroDocumento", title: Localization.Resources.GestaoPatio.FluxoPatio.NumeroDoDocumento, width: "80%" }
    ];

    _gridNumeroDocumentoFiscalFluxoPatio = new BasicDataTable(_documentoFiscalFluxoPatio.AdicionarNumerosDocumentos.idGrid, header, menuOpcoes);
    _gridNumeroDocumentoFiscalFluxoPatio.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarNumeroDocumentoFiscalFluxoPatioClick() {
    var numeroDocumentoFiscalFluxoPatio = obterNumeroDocumentoFiscalFluxoPatioSalvar();

    if (!numeroDocumentoFiscalFluxoPatio)
        return;

    var numerosDocumentoFiscalFluxoPatio = obterNumerosDocumentoFiscalFluxoPatio();

    numerosDocumentoFiscalFluxoPatio.push(numeroDocumentoFiscalFluxoPatio);

    _gridNumeroDocumentoFiscalFluxoPatio.CarregarGrid(numerosDocumentoFiscalFluxoPatio);
    fecharModalCadastroNumeroDocumentoFiscalFluxoPatio();
}

function adicionarNumeroDocumentoFiscalFluxoPatioModalClick() {
    _numeroDocumentoFiscalFluxoPatio.Codigo.val(guid());

    controlarBotoesCadastroNumeroDocumentoFiscalFluxoPatioHabilitados(false);
    exibirModalCadastroNumeroDocumentoFiscalFluxoPatio();
}

function atualizarNumeroDocumentoFiscalFluxoPatioClick() {
    var numeroDocumentoFiscalFluxoPatio = obterNumeroDocumentoFiscalFluxoPatioSalvar();

    if (!numeroDocumentoFiscalFluxoPatio)
        return;

    var numerosDocumentoFiscalFluxoPatio = obterNumerosDocumentoFiscalFluxoPatio();

    for (var i = 0; i < numerosDocumentoFiscalFluxoPatio.length; i++) {
        if (numeroDocumentoFiscalFluxoPatio.Codigo == numerosDocumentoFiscalFluxoPatio[i].Codigo) {
            numerosDocumentoFiscalFluxoPatio.splice(i, 1, numeroDocumentoFiscalFluxoPatio);
            break;
        }
    }

    _gridNumeroDocumentoFiscalFluxoPatio.CarregarGrid(numerosDocumentoFiscalFluxoPatio);
    fecharModalCadastroNumeroDocumentoFiscalFluxoPatio();
}

function avancarEtapaDocumentoFiscalClick() {
    var dados = {
        Codigo: _documentoFiscalFluxoPatio.Codigo.val(),
        NumerosDocumentos: obterNumerosDocumentoFiscalFluxoPatioSalvar()
    };

    executarReST("DocumentoFiscal/AvancarEtapa", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.DocumentoFiscalFinalizadoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesDocumentoFiscal();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function detalhesPesagemDocumentoFiscalClick() {
    exibirPesagemDetalhes(_fluxoAtual.Codigo.val());
}

function editarNumeroDocumentoFiscalFluxoPatioClick(registroSelecionado) {
    var numeroDocumentoFiscalFluxoPatio = obterNumeroDocumentoFiscalFluxoPatioPorCodigo(registroSelecionado.Codigo);

    if (!numeroDocumentoFiscalFluxoPatio)
        return;

    PreencherObjetoKnout(_numeroDocumentoFiscalFluxoPatio, { Data: numeroDocumentoFiscalFluxoPatio });
    controlarBotoesCadastroNumeroDocumentoFiscalFluxoPatioHabilitados(true);
    exibirModalCadastroNumeroDocumentoFiscalFluxoPatio();
}

function excluirNumeroDocumentoFiscalFluxoPatioClick() {
    var numerosDocumentoFiscalFluxoPatio = obterNumerosDocumentoFiscalFluxoPatio();

    for (var i = 0; i < numerosDocumentoFiscalFluxoPatio.length; i++) {
        if (numerosDocumentoFiscalFluxoPatio[i].Codigo == _numeroDocumentoFiscalFluxoPatio.Codigo.val())
            numerosDocumentoFiscalFluxoPatio.splice(i, 1);
    }

    _gridNumeroDocumentoFiscalFluxoPatio.CarregarGrid(numerosDocumentoFiscalFluxoPatio);
    fecharModalCadastroNumeroDocumentoFiscalFluxoPatio();
}

function observacoesEtapaDocumentoFiscalClick() {
    buscarObservacoesEtapa(EnumEtapaFluxoGestaoPatio.DocumentoFiscal);
}

function reabrirFluxoDocumentoFiscalClick() {
    executarReST("DocumentoFiscal/ReabrirFluxo", { Codigo: _documentoFiscalFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.FluxoReabertoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesDocumentoFiscal();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function rejeitarEtapaDocumentoFiscalClick() {
    executarReST("DocumentoFiscal/RejeitarEtapa", { Codigo: _documentoFiscalFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.DocumentoFiscalRejeitadoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesDocumentoFiscal();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function voltarEtapaDocumentoFiscalClick() {
    executarReST("DocumentoFiscal/VoltarEtapa", { Codigo: _documentoFiscalFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRetornadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesDocumentoFiscal();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.AtencaFalha, retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirDetalhesDocumentoFiscal(knoutFluxo, opt) {
    _fluxoAtual = knoutFluxo;

    executarReST("DocumentoFiscal/BuscarPorCodigo", { FluxoGestaoPatio: knoutFluxo.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _documentoFiscalFluxoPatio.AdicionarNumerosDocumentos.visible(false);
                _documentoFiscalFluxoPatio.AvancarEtapa.visible(false);
                _documentoFiscalFluxoPatio.DetalhesPesagem.visible(false);
                _documentoFiscalFluxoPatio.ReabrirFluxo.visible(false);
                _documentoFiscalFluxoPatio.RejeitarEtapa.visible(false);
                _documentoFiscalFluxoPatio.VoltarEtapa.visible(false);

                PreencherObjetoKnout(_documentoFiscalFluxoPatio, retorno);

                var permiteEditarEtapa = VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarDocumentoFiscal, _PermissoesPersonalizadasFluxoPatio) && retorno.Data.PermitirEditarEtapa;

                _documentoFiscalFluxoPatio.AdicionarNumerosDocumentos.visible(permiteEditarEtapa);
                _documentoFiscalFluxoPatio.AvancarEtapa.visible(permiteEditarEtapa);
                _documentoFiscalFluxoPatio.DetalhesPesagem.visible(knoutFluxo.GuaritaSaidaPermiteInformacoesPesagem.val());

                if (_configuracaoGestaoPatio.DocumentoFiscalPermiteVoltar && permiteEditarEtapa) {
                    var primeiraEtapa = _fluxoAtual.EtapaAtual.val() == 0;

                    _documentoFiscalFluxoPatio.VoltarEtapa.visible(!primeiraEtapa)

                    if (knoutFluxo.EtapaFluxoGestaoPatioAtual.val() == EnumEtapaFluxoGestaoPatio.DocumentoFiscal) {
                        _documentoFiscalFluxoPatio.RejeitarEtapa.visible(_configuracaoGestaoPatio.PermitirRejeicaoFluxo && _fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Aguardando);
                        _documentoFiscalFluxoPatio.ReabrirFluxo.visible(_fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Rejeitado);
                    }
                }

                if (retorno.Data.DocumentoFiscalPermiteAvancarAutomaticamenteAposNotasFiscaisInseridas) {
                    _documentoFiscalFluxoPatio.AvancarEtapa.visible(false);
                    _documentoFiscalFluxoPatio.AdicionarNumerosDocumentos.visible(false);
                }

                _gridNumeroDocumentoFiscalFluxoPatio.CarregarGrid(retorno.Data.NumerosDocumentos, permiteEditarEtapa);

                exibirModalDetalhesDocumentoFiscal();
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

function controlarBotoesCadastroNumeroDocumentoFiscalFluxoPatioHabilitados(isEdicao) {
    _numeroDocumentoFiscalFluxoPatio.Atualizar.visible(isEdicao);
    _numeroDocumentoFiscalFluxoPatio.Excluir.visible(isEdicao);
    _numeroDocumentoFiscalFluxoPatio.Adicionar.visible(!isEdicao);
}

function exibirModalCadastroNumeroDocumentoFiscalFluxoPatio() {
    Global.abrirModal('divModalNumeroDocumentoFiscalFluxoPatio');
    $("#divModalNumeroDocumentoFiscalFluxoPatio").one('hidden.bs.modal', function () {
        LimparCampos(_numeroDocumentoFiscalFluxoPatio);
    });
}

function exibirModalDetalhesDocumentoFiscal() {
    if (edicaoEtapaFluxoPatioBloqueada())
        ocultarBotoesEtapa(_documentoFiscalFluxoPatio);
        
    Global.abrirModal('divModalDocumentoFiscalFluxoPatio');

    $(window).one('keyup', function (e) {
        if (e.keyCode == 27)
            fecharModalDetalhesDocumentoFiscal();
    });

    $("#divModalDocumentoFiscalFluxoPatio").one('hidden.bs.modal', function () {
        LimparCampos(_documentoFiscalFluxoPatio);

        _gridNumeroDocumentoFiscalFluxoPatio.CarregarGrid([]);
    });
}

function fecharModalCadastroNumeroDocumentoFiscalFluxoPatio() {
    Global.fecharModal('divModalNumeroDocumentoFiscalFluxoPatio');
}

function fecharModalDetalhesDocumentoFiscal() {
    Global.fecharModal('divModalDocumentoFiscalFluxoPatio');
}

function obterNumeroDocumentoFiscalFluxoPatioPorCodigo(codigo) {
    var numerosDocumentoFiscalFluxoPatio = obterNumerosDocumentoFiscalFluxoPatio();

    for (var i = 0; i < numerosDocumentoFiscalFluxoPatio.length; i++) {
        if (numerosDocumentoFiscalFluxoPatio[i].Codigo == codigo)
            return numerosDocumentoFiscalFluxoPatio[i];
    }

    return undefined;
}

function obterNumeroDocumentoFiscalFluxoPatioSalvar() {
    if (!ValidarCamposObrigatorios(_numeroDocumentoFiscalFluxoPatio)) {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Gerais.Geral.CampoObrigatorio);
        return undefined;
    }

    var numerosDocumentoFiscalFluxoPatio = obterNumerosDocumentoFiscalFluxoPatio();

    for (var i = 0; i < numerosDocumentoFiscalFluxoPatio.length; i++) {
        var numeroDocumentoFiscalFluxoPatio = numerosDocumentoFiscalFluxoPatio[i];

        if (
            (numeroDocumentoFiscalFluxoPatio.Codigo != _numeroDocumentoFiscalFluxoPatio.Codigo.val()) &&
            (numeroDocumentoFiscalFluxoPatio.NumeroDocumento == _numeroDocumentoFiscalFluxoPatio.NumeroDocumento.val())
        ) {
            exibirMensagem("atencao", Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.GestaoPatio.FluxoPatio.NumeroDeDocumentoJaFoiAdicionado);
            return undefined;
        }
    }

    return {
        Codigo: _numeroDocumentoFiscalFluxoPatio.Codigo.val(),
        NumeroDocumento: _numeroDocumentoFiscalFluxoPatio.NumeroDocumento.val()
    };
}

function obterNumerosDocumentoFiscalFluxoPatio() {
    return _gridNumeroDocumentoFiscalFluxoPatio.BuscarRegistros().slice();
}

function obterNumerosDocumentoFiscalFluxoPatioSalvar() {
    var numerosDocumentoFiscalFluxoPatio = obterNumerosDocumentoFiscalFluxoPatio();
    var numerosDocumentoFiscalFluxoPatioSalvar = [];

    for (var i = 0; i < numerosDocumentoFiscalFluxoPatio.length; i++)
        numerosDocumentoFiscalFluxoPatioSalvar.push(numerosDocumentoFiscalFluxoPatio[i].NumeroDocumento);

    return JSON.stringify(numerosDocumentoFiscalFluxoPatioSalvar);
}

// #endregion Funções Privadas
