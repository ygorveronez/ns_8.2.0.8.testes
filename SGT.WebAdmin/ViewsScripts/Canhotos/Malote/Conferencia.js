/// <reference path="Malote.js" />
/// <reference path="../../Consultas/MotivoInconsistenciaDigitacao.js" />
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

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _conferencia;
var _motivoInconsistenciaDigitacao;
var _modalDivConferencia;
var _modalMotivoInconsistenciaDigitacao;

/*
 * Declaração das Classes
 */

var Conferencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Imagens = PropertyEntity({ val: ko.observableArray([]), });
    this.Expandir = PropertyEntity({ eventClick: expandirClick, type: types.event });
    this.Rejeitar = PropertyEntity({ eventClick: exibirModalMotivoInconsistenciaDigitacaoClick, type: types.event, text: "Rejeitar" });
    this.Validar = PropertyEntity({ eventClick: validarCanhotoClick, type: types.event, text: "Validar" });
}

var MotivoInconsistenciaDigitacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motivo:", idBtnSearch: guid(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: rejeitarCanhotoClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadConferencia() {
    _conferencia = new Conferencia();
    KoBindings(_conferencia, "knockoutConferencia");

    _motivoInconsistenciaDigitacao = new MotivoInconsistenciaDigitacao();
    KoBindings(_motivoInconsistenciaDigitacao, "knockoutMotivoInconsistenciaDigitacao");

    new BuscarMotivoInconsistenciaDigitacao(_motivoInconsistenciaDigitacao.Motivo);

    $.draggImagem({
        container: "#ModalDivConferencia",
        scroll: ".container-drag",
        image: ".container-drag img",
    });
    _modalDivConferencia = new bootstrap.Modal(document.getElementById("ModalDivConferencia"), { backdrop: true, keyboard: true });
    _modalMotivoInconsistenciaDigitacao = new bootstrap.Modal(document.getElementById("divModalMotivoInconsistenciaDigitacao"), { backdrop: true, keyboard: true });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function conferenciaClick() {
    executarReST("MaloteCanhoto/ObterCanhotos", { Codigo: _malote.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data !== false) {
                PreencherObjetoKnout(_conferencia, retorno);

                exibirModalConferencia();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function expandirClick(imagemSelecionada) {
    $("#ModalDivConferenciaDetalhes .img-canhoto").attr('src', 'data:image/png;base64,' + imagemSelecionada.Miniatura);
    _modalDivConferencia.show();
}

function exibirModalMotivoInconsistenciaDigitacaoClick(imagemSelecionada) {
    _motivoInconsistenciaDigitacao.Codigo.val(imagemSelecionada.Codigo);

    exibirModalMotivoInconsistenciaDigitacao();
}

function rejeitarCanhotoClick() {
    if (ValidarCamposObrigatorios(_motivoInconsistenciaDigitacao)) {
        var imagens = _conferencia.Imagens.val();

        for (var i = 0; i < imagens.length; i++) {
            if (imagens[i].Codigo == _motivoInconsistenciaDigitacao.Codigo.val()) {
                executarReST("MaloteCanhoto/DescartarCanhoto", RetornarObjetoPesquisa(_motivoInconsistenciaDigitacao), function (retorno) {
                    if (retorno.Success) {
                        if (retorno.Data !== false) {
                            var imagemRejeitada = $.extend(true, {}, imagens[i]);

                            imagemRejeitada.Rejeitado = true;

                            _conferencia.Imagens.val.replace(imagens[i], imagemRejeitada);

                            fecharModalMotivoInconsistenciaDigitacao();
                        }
                        else
                            exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                    }
                    else
                        exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
                }, null);

                break;
            }
        }
    }
}

function validarCanhotoClick(imagemSelecionada) {
    var imagens = _conferencia.Imagens.val();

    for (var i = 0; i < imagens.length; i++) {
        if (imagens[i].Codigo == imagemSelecionada.Codigo) {
            executarReST("MaloteCanhoto/ValidarCanhoto", { Codigo: imagemSelecionada.Codigo }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data !== false) {
                        var imagemRejeitada = $.extend(true, {}, imagens[i]);

                        imagemRejeitada.Rejeitado = false;

                        _conferencia.Imagens.val.replace(imagens[i], imagemRejeitada);
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }, null);

            break;
        }
    }
}

/*
 * Declaração das Funções
 */

function exibirModalConferencia() {
    _modalDivConferencia.show();
    $("#ModalDivConferencia").one('hidden.bs.modal', function () {
        LimparCampos(_conferencia);
    });
}

function exibirModalMotivoInconsistenciaDigitacao() {
    _modalMotivoInconsistenciaDigitacao.show();
    $("#divModalMotivoInconsistenciaDigitacao").one('hidden.bs.modal', function () {
        LimparCampos(_motivoInconsistenciaDigitacao);
    });
}

function fecharModalMotivoInconsistenciaDigitacao() {
    _modalMotivoInconsistenciaDigitacao.hide();
}