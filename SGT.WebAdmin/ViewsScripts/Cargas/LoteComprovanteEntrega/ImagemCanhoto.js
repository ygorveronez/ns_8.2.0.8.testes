/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />

/**
 *  Esse arquivo cuida de tudo que é necessário para gerenciar o modal das imagens dos canhoto
 * */

var _imagemCanhoto;
var _canhotoAtual;

// region: ENTIDADES KNOCKOUT

var ImagemCanhoto = function () {
    this.Imagem = PropertyEntity({ val: ko.observable(), getType: typesKnockout.string });
    this.EnviarImagem = PropertyEntity({ eventClick: EnviarImagemClick, type: types.event, text: ko.observable("Enviar nova imagem"), visible: ko.observable(true), enable: ko.observable(true) });
    
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), required: false });

    this.onChooseFile = () => {
        onChooseImage();
    }
}

// region: ACTIONS

function EnviarImagemClick() {
    // Reseta o input
    var imageInput = document.getElementById(_imagemCanhoto.Arquivo.id);
    imageInput.value = null;

    abrirModalEnviarImagemClick();
}

function loadModalImagemCanhoto() {
    _imagemCanhoto = new ImagemCanhoto();
    KoBindings(_imagemCanhoto, "knockoutImagemCanhoto");
}

async function exibirModalImagemCanhoto(canhoto) {
    // Desabilita botões se for modo de visualização ou o canhoto já tem img
    _imagemCanhoto.EnviarImagem.enable(podeEditarLote() && !canhotoJaTemImagem(canhoto));

    _canhotoAtual = canhoto;
    Global.abrirModal('divModalImagemCanhoto');
    $("#divModalImagemCanhoto").on("hidden.bs.modal", () => {
        _imagemCanhoto.Imagem.val(null);
        recarregarGridCargaEntrega();
        recarregarGridCanhotos();
    });

    let imagemBase64 = await obterBase64Imagem(canhoto);
    _imagemCanhoto.Imagem.val(imagemBase64);
}

function canhotoJaTemImagem(canhoto) {
    return canhoto.Imagem && canhoto.Imagem != '';
}

async function onChooseImage() {
    // Coloca o file que foi importado no canhoto. Esse arquivo será upado para o back após o Lote ser criado.
    var imageInput = document.getElementById(_imagemCanhoto.Arquivo.id);
    let base64 = await toBase64(imageInput.files[0]);
    _canhotoAtual.Imagem = "generico.png";
    _canhotoAtual.ImagemFile = imageInput.files[0];
    _canhotoAtual.ImagemBase64 = base64;
    _imagemCanhoto.Imagem.val(base64);
}

function toBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result);
        reader.onerror = error => reject(error);
    });
}

function obterBase64Imagem(canhoto) {
    return new Promise((resolve) => {
        if (canhoto.ImagemBase64) {
            resolve(canhoto.ImagemBase64)
            return;
        }

        var data = {
            CodigoCanhoto: canhoto.Codigo,
        }
        executarReST("LoteComprovanteEntrega/ObterImagemCanhoto", data, function (r) {
            if (r.Success) {
                if (r.Data) {
                    if (r.Data.Miniatura) {
                        resolve('data:image/png;base64,' + r.Data.Miniatura)
                    } else {
                        resolve(null);
                    }
                }
            } else {
                resolve(null);
            }
        });
    });
}

function abrirModalEnviarImagemClick() {
    $("#" + _imagemCanhoto.Arquivo.id).trigger("click");
}

