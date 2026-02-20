/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Globais.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/ColetaEntrega.js" />
/// <reference path="../../Consultas/MotivoRejeicao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoGestaoDadosColeta.js" />
/// <reference path="GestaoDadosColeta.js" />

// #region Objetos Globais do Arquivo

var _gestaoDadosColetaDadosNfe;
var _resumoGestaoDadosColetaDadosNfe;
var _gestaoDadosColetaRetornoConfirmacao;
let _fotoFotosDadosGestaoDadosColetaDadosNfeDragg = null;
var _motivoRejeicao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var GestaoDadosColetaDadosNfe = function () {
    var camposAprovacaoObrigatorios = (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe);
    var prefixoCampoObrigatorio = camposAprovacaoObrigatorios ? "*" : "";

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoGestaoDadosColeta.AguardandoAprovacao), def: EnumSituacaoGestaoDadosColeta.AguardandoAprovacao });

    this.Coleta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), text: ko.observable("*Coleta:"), idBtnSearch: guid(), required: false, visible: ko.observable(true), modeloVeicular: ko.observable(0) });
    this.ArquivoFoto = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: ko.observable("Arquivo:"), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.FotoNotaFiscal = PropertyEntity({});
    this.GuidArquivo = PropertyEntity({});
    this.Chave = PropertyEntity({ getType: typesKnockout.int, text: "Chave:", visible: ko.observable(true), enable: ko.observable(true), required: false, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.Numero = PropertyEntity({ getType: typesKnockout.int, text: prefixoCampoObrigatorio + "Número:", visible: ko.observable(true), enable: ko.observable(true), required: camposAprovacaoObrigatorios, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.Serie = PropertyEntity({ getType: typesKnockout.int, text: prefixoCampoObrigatorio + "Série:", visible: ko.observable(true), enable: ko.observable(true), required: camposAprovacaoObrigatorios, maxlength: 3 });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.date, text: prefixoCampoObrigatorio + "Data Emissão:", visible: ko.observable(true), enable: ko.observable(true), required: camposAprovacaoObrigatorios });
    this.Peso = PropertyEntity({ getType: typesKnockout.decimal, text: prefixoCampoObrigatorio + "Peso:", visible: ko.observable(true), enable: ko.observable(true), required: camposAprovacaoObrigatorios });
    this.Volumes = PropertyEntity({ getType: typesKnockout.int, text: "Volumes:", visible: ko.observable(true), enable: ko.observable(true), required: false });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, text: prefixoCampoObrigatorio + "Valor:", visible: ko.observable(true), enable: ko.observable(true), required: camposAprovacaoObrigatorios });
    this.Emitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), text: prefixoCampoObrigatorio + "Emitente:", idBtnSearch: guid(), required: camposAprovacaoObrigatorios, visible: ko.observable(true), enable: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), text: prefixoCampoObrigatorio + "Destinatário:", idBtnSearch: guid(), required: camposAprovacaoObrigatorios, visible: ko.observable(true), enable: ko.observable(true) });

    this.ErroRetornoConfirmacaoColeta = PropertyEntity({ text: "Erro: ", val: ko.observable(""), getType: typesKnockout.string });
    this.IdExternoRetornoConfirmacaoColeta = PropertyEntity({ text: "Id Externo: ", val: ko.observable(""), getType: typesKnockout.string });
    this.DataRetornoConfirmacaoColeta = PropertyEntity({ text: "Data: ", val: ko.observable(""), getType: typesKnockout.string });
    this.OperacaoRetornoConfirmacaoColeta = PropertyEntity({ text: "Operação: ", val: ko.observable(""), getType: typesKnockout.string });

    this.ArquivoFoto.val.subscribe(function (nomeArquivoFotoSelecionado) {
        if (nomeArquivoFotoSelecionado)
            enviarFotosDadosGestaoDadosColetaDadosNfe();
    });

    this.DownloadFoto = PropertyEntity({ type: types.event, eventClick: downloadFotoFotosDadosGestaoDadosColetaDadosNfeClick });
    this.RemoverFoto = PropertyEntity({ eventClick: removerFotoFotosDadosGestaoDadosColetaDadosNfeClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Remover), visible: ko.observable(true) });
    this.SalvarDadosNota = PropertyEntity({ type: types.event, eventClick: salvarDadosGestaoDadosColetaDadosNfe, text: "Salvar", visible: ko.observable(false) });
    this.AprovarDadosNota = PropertyEntity({ type: types.event, eventClick: aprovarDadosGestaoDadosColetaDadosNfeClick, text: "Aprovar", visible: ko.observable(false) });
    this.RejeitarDadosNota = PropertyEntity({ type: types.event, eventClick: exibirModalMotivosRejeicao, text: "Rejeitar", visible: ko.observable(false) });
}

var ResumoGestaoDadosColetaDadosNfe = function () {
    this.Carga = PropertyEntity({ val: ko.observable(""), text: "Carga: " });
    this.CodigoCarga = PropertyEntity({ val: ko.observable("") });
    this.Cliente = PropertyEntity({ val: ko.observable(""), text: "Cliente: " });
    this.Endereco = PropertyEntity({ val: ko.observable(""), text: "Endereço do Cliente: " });
    this.Pedidos = PropertyEntity({ val: ko.observable(""), text: "Pedidos: " });
    this.Transportador = PropertyEntity({ val: ko.observable(""), text: "Transportador: " });
    this.Origem = PropertyEntity({ val: ko.observable(""), text: "Origem: " });
    this.Destino = PropertyEntity({ val: ko.observable(""), text: "Destino: " });
}

var MotivoRejeicao = function () {
    this.MotivoRejeicao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), idBtnSearch: guid(), required: false, visible: ko.observable(true) });
    this.RejeitarComMotivo = PropertyEntity({ type: types.event, eventClick: rejeitarDadosGestaoDadosColetaDadosNfeClick, text: "Rejeitar", visible: ko.observable(true) });
    this.CancelarRejeitar = PropertyEntity({ type: types.event, eventClick: cancelarDadosGestaoDadosColetaDadosNfeClick, text: "Cancelar", visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização


function loadGestaoDadosColetaDadosNfe() {
    _gestaoDadosColetaDadosNfe = new GestaoDadosColetaDadosNfe();
    KoBindings(_gestaoDadosColetaDadosNfe, "knockoutGestaoDadosColetaDadosNfe");

    _resumoGestaoDadosColetaDadosNfe = new ResumoGestaoDadosColetaDadosNfe();
    KoBindings(_resumoGestaoDadosColetaDadosNfe, "knockoutResumoDadosNfe");

    _motivoRejeicao = new MotivoRejeicao();
    KoBindings(_motivoRejeicao, "knockoutMotivoRejeicao");

    BuscarColetas(_gestaoDadosColetaDadosNfe.Coleta, preencherDadosColeta);
    BuscarClientes(_gestaoDadosColetaDadosNfe.Emitente);
    BuscarClientes(_gestaoDadosColetaDadosNfe.Destinatario);
    BuscarMotivos(_motivoRejeicao.MotivoRejeicao);

    loadMapaGestaoDadosColetaDadosNfe();
    loadGestaoDadosColetaDadosNFeIntegracao();

    _fotoFotosDadosGestaoDadosColetaDadosNfeDragg = $.draggImagem({
        container: ".container-gestao-dados-coleta-dragg",
        image: ".imagem-coleta img",
    });
}

function loadMapaGestaoDadosColetaDadosNfe() {
    var opcoesmapa = new OpcoesMapa(false, false);

    _gestaoDadosColetaDadosNfe.Mapa = new MapaGoogle("map", false, opcoesmapa);
    _gestaoDadosColetaDadosNfe.Mapa.direction.setZoom(4);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function salvarDadosGestaoDadosColetaDadosNfe() {
    if (!ValidarCamposObrigatorios(_gestaoDadosColetaDadosNfe)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    if (!_gestaoDadosColetaDadosNfe.GuidArquivo.val()) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, "Por favor, informe a foto da NF-e.");
        return;
    }

    var data = {
        Coleta: _gestaoDadosColetaDadosNfe.Coleta.codEntity(),
        Chave: _gestaoDadosColetaDadosNfe.Chave.val(),
        Numero: _gestaoDadosColetaDadosNfe.Numero.val(),
        Serie: _gestaoDadosColetaDadosNfe.Serie.val(),
        DataEmissao: _gestaoDadosColetaDadosNfe.DataEmissao.val(),
        Peso: _gestaoDadosColetaDadosNfe.Peso.val(),
        Volumes: _gestaoDadosColetaDadosNfe.Volumes.val(),
        Valor: _gestaoDadosColetaDadosNfe.Valor.val(),
        Emitente: _gestaoDadosColetaDadosNfe.Emitente.codEntity(),
        Destinatario: _gestaoDadosColetaDadosNfe.Destinatario.codEntity(),
        GuidArquivo: _gestaoDadosColetaDadosNfe.GuidArquivo.val()
    };

    executarReST("GestaoDadosColeta/AdicionarGestaoDadosColetaDadosNfe", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
                Global.fecharModal('divModalAdicionarGestaoDadosColetaDadosNfe');
                recarregarGridGestaoDadosColeta();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function aprovarDadosGestaoDadosColetaDadosNfeClick() {
    if (!ValidarCamposObrigatorios(_gestaoDadosColetaDadosNfe)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    var data = {
        Codigo: _gestaoDadosColetaDadosNfe.Codigo.val(),
        Coleta: _gestaoDadosColetaDadosNfe.Coleta.codEntity(),
        Chave: _gestaoDadosColetaDadosNfe.Chave.val(),
        Numero: _gestaoDadosColetaDadosNfe.Numero.val(),
        Serie: _gestaoDadosColetaDadosNfe.Serie.val(),
        DataEmissao: _gestaoDadosColetaDadosNfe.DataEmissao.val(),
        Peso: _gestaoDadosColetaDadosNfe.Peso.val(),
        Volumes: _gestaoDadosColetaDadosNfe.Volumes.val(),
        Valor: _gestaoDadosColetaDadosNfe.Valor.val(),
        Emitente: _gestaoDadosColetaDadosNfe.Emitente.codEntity(),
        Destinatario: _gestaoDadosColetaDadosNfe.Destinatario.codEntity(),
        GuidArquivo: _gestaoDadosColetaDadosNfe.GuidArquivo.val()
    };

    executarReST("GestaoDadosColeta/AprovacaoGestaoDadosColetaDadosNFe", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
                Global.fecharModal("divModalAdicionarGestaoDadosColetaDadosNfe");
                recarregarGridGestaoDadosColeta();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function downloadFotoFotosDadosGestaoDadosColetaDadosNfeClick() {
    executarDownload("GestaoDadosColeta/DownloadFoto", { Codigo: _gestaoDadosColetaDadosNfe.Codigo.val() });
}

function rejeitarDadosGestaoDadosColetaDadosNfeClick() {
    executarReST("GestaoDadosColeta/RejeitarGestaoDadosColetaDadosNFe", { Codigo: _gestaoDadosColetaDadosNfe.Codigo.val(), CodigoMotivoRejeicao: _motivoRejeicao.MotivoRejeicao.codEntity(), CodigoCarga: _resumoGestaoDadosColetaDadosNfe.CodigoCarga.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
                Global.fecharModal("divModalAdicionarGestaoDadosColetaDadosNfe");
                Global.fecharModal("divModalMotivoRejeicao");
                recarregarGridGestaoDadosColeta();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function removerFotoFotosDadosGestaoDadosColetaDadosNfeClick() {
    executarReST("GestaoDadosColeta/ExcluirFoto", { GuidArquivo: _gestaoDadosColetaDadosNfe.GuidArquivo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _gestaoDadosColetaDadosNfe.ArquivoFoto.val("");
                _gestaoDadosColetaDadosNfe.FotoNotaFiscal.val("");
                setTimeout(_fotoFotosDadosGestaoDadosColetaDadosNfeDragg.top, 500);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirModalAdicionarGestaoDadosColetaDadosNfe() {
    limparCamposGestaoDadosColetaDadosNfe();
    exibirModalGestaoDadosColetaDadosNfe();
}

function exibirModalEditarGestaoDadosColetaDadosNfe(registroSelecionado) {
    limparCamposGestaoDadosColetaDadosNfe();

    executarReST("GestaoDadosColeta/BuscarDadosNfePorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_resumoGestaoDadosColetaDadosNfe, { Data: retorno.Data.GestaoDadosColeta });
                PreencherObjetoKnout(_gestaoDadosColetaDadosNfe, { Data: retorno.Data.DadosAprovacaoNFe });
                preencherPosicaoMapaGestaoDadosColetaDadosNfe(retorno.Data.Geolocalizacao);
                preencherGestaoDadosColetaDadosNFeIntegracao(registroSelecionado.Codigo, retorno.Data.DadosAprovacaoNFe.Situacao);
                exibirModalGestaoDadosColetaDadosNfe();
                exibirAbaDadosRetornoConfirmacao(retorno.Data.DadosAprovacaoNFe.DataRetornoConfirmacaoColeta);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarCamposGestaoDadosColetaDadosNfe() {
    var registroEmEdicao = _gestaoDadosColetaDadosNfe.Codigo.val() > 0;
    var permitirAprovacao = (_gestaoDadosColetaDadosNfe.Situacao.val() == EnumSituacaoGestaoDadosColeta.AguardandoAprovacao) && (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe);

    if (registroEmEdicao) {
        $('#divDetalhesColeta').show();

        _gestaoDadosColetaDadosNfe.SalvarDadosNota.visible(false);
        _gestaoDadosColetaDadosNfe.AprovarDadosNota.visible(permitirAprovacao);
        _gestaoDadosColetaDadosNfe.RejeitarDadosNota.visible(permitirAprovacao);
    }
    else {
        $('#divDetalhesColeta').hide();

        _gestaoDadosColetaDadosNfe.SalvarDadosNota.visible(true);
        _gestaoDadosColetaDadosNfe.AprovarDadosNota.visible(false);
        _gestaoDadosColetaDadosNfe.RejeitarDadosNota.visible(false);
    }

    var habilitarCamposAprovacao = !registroEmEdicao || permitirAprovacao;

    _gestaoDadosColetaDadosNfe.Coleta.visible(!registroEmEdicao);
    _gestaoDadosColetaDadosNfe.Coleta.required = !registroEmEdicao;
    _gestaoDadosColetaDadosNfe.Chave.enable(habilitarCamposAprovacao);
    _gestaoDadosColetaDadosNfe.Numero.enable(habilitarCamposAprovacao);
    _gestaoDadosColetaDadosNfe.Serie.enable(habilitarCamposAprovacao);
    _gestaoDadosColetaDadosNfe.DataEmissao.enable(habilitarCamposAprovacao);
    _gestaoDadosColetaDadosNfe.Peso.enable(habilitarCamposAprovacao);
    _gestaoDadosColetaDadosNfe.Volumes.enable(habilitarCamposAprovacao);
    _gestaoDadosColetaDadosNfe.Valor.enable(habilitarCamposAprovacao);
    _gestaoDadosColetaDadosNfe.Emitente.enable(habilitarCamposAprovacao);
    _gestaoDadosColetaDadosNfe.Destinatario.enable(habilitarCamposAprovacao);
}

function enviarFotosDadosGestaoDadosColetaDadosNfe() {
    var formData = obterFormDataFotoNotaFiscalGestaoDadosColetaDadosNfe();

    if (formData) {
        enviarArquivo("GestaoDadosColeta/AdicionarFoto?callback=?", null, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _gestaoDadosColetaDadosNfe.GuidArquivo.val(retorno.Data.GuidArquivo);
                    _gestaoDadosColetaDadosNfe.FotoNotaFiscal.val(retorno.Data.FotoNotaFiscal);
                    setTimeout(_fotoFotosDadosGestaoDadosColetaDadosNfeDragg.top, 500);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

function exibirModalGestaoDadosColetaDadosNfe() {
    controlarCamposGestaoDadosColetaDadosNfe();
    Global.abrirModal('divModalAdicionarGestaoDadosColetaDadosNfe');
    setTimeout(_fotoFotosDadosGestaoDadosColetaDadosNfeDragg.top, 500);
}

function exibirAbaDadosRetornoConfirmacao(visible) {
    $('#liTabGestaoDadosColetaRetornoConfirmacao').hide();
    if (visible)
        $('#liTabGestaoDadosColetaRetornoConfirmacao').show();
}

function limparCamposGestaoDadosColetaDadosNfe() {
    LimparCampos(_gestaoDadosColetaDadosNfe);
    limparGestaoDadosColetaDadosNFeIntegracao();
    $('#liTabMapaGestaoDadosColetaDadosNfe').hide();
    Global.ResetarAbas();
}

function obterFormDataFotoNotaFiscalGestaoDadosColetaDadosNfe() {
    var arquivo = document.getElementById(_gestaoDadosColetaDadosNfe.ArquivoFoto.id);

    if (arquivo.files.length > 0) {
        var formData = new FormData();

        formData.append("ArquivoFoto", arquivo.files[0]);

        return formData;
    }

    return undefined;
}

function preencherPosicaoMapaGestaoDadosColetaDadosNfe(dadosGeolocalizacao) {
    _gestaoDadosColetaDadosNfe.Mapa.clear();

    if (dadosGeolocalizacao.PossuiGeolocalizacao) {
        var marker = new ShapeMarker();

        marker.setPosition(dadosGeolocalizacao.Latitude, dadosGeolocalizacao.Longitude);
        marker.icon = _gestaoDadosColetaDadosNfe.Mapa.draw.icons.truck();

        _gestaoDadosColetaDadosNfe.Mapa.draw.addShape(marker);

        if (dadosGeolocalizacao.PossuiLocalizacaoCliente)
            preencherLocalizacaoCliente(dadosGeolocalizacao);

        $('#liTabMapaGestaoDadosColetaDadosNfe').show();
    }
    else
        $('#liTabMapaGestaoDadosColetaDadosNfe').hide();
}

function preencherLocalizacaoCliente(dadosGeolocalizacao) {

    var markerLocalizacaoCliente = new ShapeMarker();

    var posicaoCliente = { lat: parseFloat(dadosGeolocalizacao.LocalizacaoCliente.Latitude), lng: parseFloat(dadosGeolocalizacao.LocalizacaoCliente.Longitude) };

    markerLocalizacaoCliente.setPosition(posicaoCliente.lat, posicaoCliente.lng);

    var shapeCircle = new ShapeCircle();
    shapeCircle.type = google.maps.drawing.OverlayType.CIRCLE;
    shapeCircle.fillColor = "#FF0000";
    shapeCircle.radius = parseInt(dadosGeolocalizacao.LocalizacaoCliente.RaioCliente);
    shapeCircle.center = posicaoCliente;

    _gestaoDadosColetaDadosNfe.Mapa.draw.addShape(markerLocalizacaoCliente);
    _gestaoDadosColetaDadosNfe.Mapa.draw.addShape(shapeCircle);
}

function preencherDadosColeta(dadosColeta) {
    if (dadosColeta.Remetente) {
        let dados = JSON.parse(dadosColeta.Remetente);
        _gestaoDadosColetaDadosNfe.Emitente.codEntity(dados.Codigo);
        _gestaoDadosColetaDadosNfe.Emitente.val(dados.Descricao);
    }
    if (dadosColeta.Destinatario) {
        let dados = JSON.parse(dadosColeta.Destinatario);
        _gestaoDadosColetaDadosNfe.Destinatario.codEntity(dados.Codigo);
        _gestaoDadosColetaDadosNfe.Destinatario.val(dados.Descricao);
    }

    _gestaoDadosColetaDadosNfe.Coleta.codEntity(dadosColeta.Codigo);
    _gestaoDadosColetaDadosNfe.Coleta.val(dadosColeta.NumeroPedidos);
}

function exibirModalMotivosRejeicao() {
    Global.abrirModal('divModalMotivoRejeicao');
}
function cancelarDadosGestaoDadosColetaDadosNfeClick() {
    Global.fecharModal('divModalMotivoRejeicao');
    LimparCampo(_motivoRejeicao);
}
// #endregion Funções Privadas
