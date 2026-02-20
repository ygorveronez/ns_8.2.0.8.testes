/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumTipoAnexoGuia.js" />
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridGuias;
var _pesquisaGuias;
var _knoutGuia;
var _knoutModalGuiaValidacaoManual;
var _permissoesPersonalizadas;
var _atualizarGridAoFecharModal = false;
var _statusValidacao = [{
    text: "Aprovado",
    value: true
}, {
    text: "Rejeitado",
    value: false
}]
var aprovado = {
    text: "Aprovado",
    value: true
}
/*
Declaração das Classes
 */


var PesquisaGuias = function () {
    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Inicial:", getType: typesKnockout.date, val: ko.observable(), cssClass: ko.observable("col col-xs-6 col-lg-3") });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data Emissão Final:", getType: typesKnockout.date });
    this.Carga = PropertyEntity({ text: "Número Carga:" });
    this.Status = PropertyEntity({ text: "Status:", val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoGuia.obterOpcoes(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: ko.observable(true), text: "Veículo:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.SerieCte = PropertyEntity({ text: "Série CT-e:", getType: typesKnockout.int, configInt: { precision: 0, allowZero: true, thousands: "" }, maxlength: 10 });
    this.NroCte = PropertyEntity({ text: "Número CT-e:", getType: typesKnockout.int, configInt: { precision: 0, allowZero: true, thousands: "" }, maxlength: 10 });
    this.ChaveCte = PropertyEntity({ text: "Chave CT-e:", getType: typesKnockout.text, maxlength: 44 });
    this.ReenviarTodosRejeitados = PropertyEntity({ eventClick: reenviarTodasGuiasRejeitados, enable: ko.observable(true), type: types.event, text: "Reenviar todos rejeitados", visible: ko.observable(true) });

    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            recarregarGridGuias();
        },
        type: types.event,
        text: "Pesquisar",
        idGrid: guid(),
        visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa.getFieldDescription(), idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

};

var Guias = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), required: false });
}

var ModalGuiaValidacaoManual = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.OCRGuiaValidacaoManual = PropertyEntity({ text: "OCR Guia", val: ko.observable(aprovado), options: _statusValidacao, def: aprovado });
    this.OCRComprovanteValidacaoManual = PropertyEntity({ text: "OCR Comprovante", val: ko.observable(aprovado), options: _statusValidacao, def: aprovado });
    this.ValidadaTodasInformacoesManualmente = PropertyEntity({ text: "Guia x Comprovante x Sistema", val: ko.observable(aprovado), options: _statusValidacao, def: aprovado });
    this.Observacao = PropertyEntity({ text: "Observações", val: ko.observable(""), def: "" });

    this.Confirmar = PropertyEntity({ eventClick: confirmarValidacaoManual, type: types.event, text: "Confirmar", idGrid: guid(), visible: ko.observable(true) })
    this.Cancelar = PropertyEntity({ eventClick: fecharModalValidacaoManual, type: types.event, text: "Cancelar", idGrid: guid(), visible: ko.observable(true) })
}

/*
 * Declaração das Funções Públicas
 */

function recarregarGridGuias() {
    _gridGuias.CarregarGrid();
}

/*
 * Declaração das Funções Privadas
 */

function loadGuias() {
    _pesquisaGuias = new PesquisaGuias();
    KoBindings(_pesquisaGuias, "knockoutPesquisaGuias", false, _pesquisaGuias.Pesquisar.id);

    _knoutGuia = new Guias();
    KoBindings(_knoutGuia, "knoutEnviarArquivoGuia")

    _knoutComprovante = new Guias();
    KoBindings(_knoutComprovante, "knoutEnviarArquivoComprovante")

    _knoutModalGuiaValidacaoManual = new ModalGuiaValidacaoManual();
    KoBindings(_knoutModalGuiaValidacaoManual, "knockoutGuiaRecolhimentoValidarManual")

    new BuscarTransportadores(_pesquisaGuias.Transportador);
    new BuscarVeiculos(_pesquisaGuias.Veiculo);
    new BuscarMotorista(_pesquisaGuias.Motorista);

    loadGuiaRecolhimento();
    buscarGuias();

    $("#" + _knoutGuia.Arquivo.id).click(function () {
        $(this).val("");
    });

    $("#" + _knoutGuia.Arquivo.id).on("change", enviarImagemGuia);

    $("#" + _knoutComprovante.Arquivo.id).click(function () {
        $(this).val("");
    });

    $("#" + _knoutComprovante.Arquivo.id).on("change", enviarImagemComprovante);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        _pesquisaGuias.Transportador.visible(false);
    }
}


function obterFormDataAnexos(anexos) {
    if (anexos.length > 0) {
        var formData = new FormData();

        anexos.forEach(function (anexo) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        });

        return formData;
    }

    return undefined;
}


function uploadGuiaClick(e) {
    _knoutGuia.Codigo.val(e.Codigo)

    $("#" + _knoutGuia.Arquivo.id).trigger("click");
}

function uploadComprovanteClick(e) {
    _knoutComprovante.Codigo.val(e.Codigo)

    $("#" + _knoutComprovante.Arquivo.id).trigger("click");
}

function exibirReenviar() {
    return !_ConfiguracoesGuiasRecolhimento.VisualizarGNRESemValidacaoDocumentos;
}

function exibirCancelar() {
    return !_ConfiguracoesGuiasRecolhimento.VisualizarGNRESemValidacaoDocumentos;
}

function exibirUploadGuia(data) {

    if (data.OCRGuiaValidado == "Validado")
        return false;
    if (_ConfiguracoesGuiasRecolhimento.VisualizarGNRESemValidacaoDocumentos)
        return true;

    return !data.GuiaAnexada
}

function exibirUploadComprovante(data) {
    if (data.OCRComprovanteValidado == "Validado")
        return false;
    if (_ConfiguracoesGuiasRecolhimento.VisualizarGNRESemValidacaoDocumentos)
        return true;

    return !data.ComprovanteAnexado
}

function exibirVerGuia(data) {
    return data.GuiaAnexada
}

function exibirVerComprovante(data) {
    return data.ComprovanteAnexado
}

function abrirModalGuiaAnexo() {
    abrirGuiaRecolhimentoClick();
}

function abrirModalComprovanteAnexo() {
    abrirGuiaRecolhimentoClick();
}


//#region Requisições

function reenviarTodasGuiasRejeitados(e) {

    exibirConfirmacao("Deseja realmente realizar esta ação?", "Ao aceitar serão enviados todos os registro rejeitados", function () {
        executarReST("GuiaNacionalRecolhimentoTributoEstual/ReenviarTodosPendentes", {}, function (arg) {
            if (!arg.Success)
                return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            recarregarGridGuias();
            return exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
        });
    });
}

function reenviarGuiRecolhimento(e) {
    executarReST("GuiaNacionalRecolhimentoTributoEstual/Reenviar", { Codigo: e.Codigo }, function (arg) {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        recarregarGridGuias();
        return exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
    });
}

function cancelarGuiRecolhimento(e) {
    executarReST("GuiaNacionalRecolhimentoTributoEstual/Cancelar", { Codigo: e.Codigo }, function (arg) {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        recarregarGridGuias();
        return exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
    });
}

function enviarImagemGuia() {
    var arquivo = document.getElementById(_knoutGuia.Arquivo.id);

    if (arquivo.files.length == 0)
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

    var anexo = {
        Codigo: guid(),
        Descricao: _knoutGuia.Arquivo.val(),
        NomeArquivo: "Anexo",
        Arquivo: arquivo.files[0]
    };


    var formData = obterFormDataAnexos([anexo]);

    enviarArquivo("GuiaNacionalRecolhimentoTributoEstual/UploadImagem?callback=?", { Codigo: _knoutGuia.Codigo.val(), TipoAnexo: EnumTipoAnexoGuia.Guia }, formData, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Enviado com sucesso!");
                recarregarGridGuias();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function enviarImagemComprovante() {
    var arquivo = document.getElementById(_knoutComprovante.Arquivo.id);

    if (arquivo.files.length == 0)
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

    var anexo = {
        Codigo: guid(),
        Descricao: _knoutComprovante.Arquivo.val(),
        NomeArquivo: "Anexo",
        Arquivo: arquivo.files[0]
    };


    var formData = obterFormDataAnexos([anexo]);

    enviarArquivo("GuiaNacionalRecolhimentoTributoEstual/UploadImagem?callback=?", { Codigo: _knoutComprovante.Codigo.val(), TipoAnexo: EnumTipoAnexoGuia.Comprovante }, formData, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Enviado com sucesso!");
                recarregarGridGuias();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function ValidarManualmente(e) {
    _knoutModalGuiaValidacaoManual.Codigo.val(e.Codigo);
    _knoutModalGuiaValidacaoManual.Observacao.val(e.Observacao);
    _knoutModalGuiaValidacaoManual.ValidadaTodasInformacoesManualmente.val(e.ValidouTodasInformacoesManualmente);
    _knoutModalGuiaValidacaoManual.OCRComprovanteValidacaoManual.val(e.ComprovanteValidadoManualmente);
    _knoutModalGuiaValidacaoManual.OCRGuiaValidacaoManual.val(e.GuiaValidadaManualmente);
    Global.abrirModal("divModalValidarManual");
}


function buscarGuias() {
    console.log(VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Guias_PermiteAprovarRejeitarOCRGuiasManualmente, _permissoesPersonalizadas) || _CONFIGURACAO_TMS.UsuarioAdministrador)
    var reenviar = { descricao: Localization.Resources.Gerais.Geral.Reenviar, id: guid(), metodo: reenviarGuiRecolhimento, icone: "", visibilidade: exibirReenviar };
    //var download = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: () => { }, icone: "", visibilidade: true };
    var cancelar = { descricao: Localization.Resources.Gerais.Geral.Cancelar, id: guid(), metodo: cancelarGuiRecolhimento, icone: "", visibilidade: exibirCancelar };

    var uploadGuia = { descricao: "Anexar GUIA", id: guid(), metodo: uploadGuiaClick, icone: "", visibilidade: exibirUploadGuia };

    var uploadComprovante = { descricao: "Anexar Comprovante", id: guid(), metodo: uploadComprovanteClick, icone: "", visibilidade: exibirUploadComprovante };

    var exibirGuia = { descricao: "Ver GUIA", id: guid(), metodo: abrirGuiaRecolhimentoClick, icone: "", visibilidade: exibirVerGuia };

    var exibirComprovante = { descricao: "Ver Comprovante", id: guid(), metodo: abrirComprovanteClick, icone: "", visibilidade: exibirVerComprovante };

    let descricaoValidarManual = _ConfiguracoesGuiasRecolhimento.VisualizarGNRESemValidacaoDocumentos ? Localization.Resources.Gerais.Geral.Confirmar : "Aprovar/Rejeitar guias"

    var validarManual = { descricao: descricaoValidarManual, id: guid(), metodo: ValidarManualmente, icone: "", visibilidade: visibleOpcao};

    var configExportacao = {
        url: "GuiaNacionalRecolhimentoTributoEstual/ExportarGuias",
        titulo: "Guias Recolhimento"
    };

    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [reenviar, cancelar, uploadGuia, exibirGuia, uploadComprovante, exibirComprovante, validarManual] };

    _gridGuias = new GridView("grid-pesquisa-guias", "GuiaNacionalRecolhimentoTributoEstual/ConsultarGuias", _pesquisaGuias, menuOpcoes, null, 10, null, true, false, null, null, null, configExportacao);
    _gridGuias.CarregarGrid();
}


function ModalOcultou() {
    _indexGuiaAberto = -1;
}

function visibleOpcao() {
    if (_FormularioSomenteLeitura)
        return false;

    if (!_permissoesPersonalizadas.length && !_CONFIGURACAO_TMS.UsuarioAdministrador)
        return false;

    return (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Guias_PermiteAprovarRejeitarOCRGuiasManualmente, _permissoesPersonalizadas) || _CONFIGURACAO_TMS.UsuarioAdministrador)
}

function confirmarValidacaoManual(e) {
    let data = {
        Codigo: e.Codigo.val(),
        OCRGuiaValidacaoManual: e.OCRGuiaValidacaoManual.val(),
        OCRComprovanteValidacaoManual: e.OCRComprovanteValidacaoManual.val(),
        ValidadaTodasInformacoesManualmente: e.ValidadaTodasInformacoesManualmente.val(),
        Observacao: e.Observacao.val()
    }
    executarReST("GuiaNacionalRecolhimentoTributoEstual/ValidacaoManual", data, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
        fecharModalValidacaoManual();
        _gridGuias.CarregarGrid();
    })
}
function fecharModalValidacaoManual() {
    Global.fecharModal("divModalValidarManual")
}