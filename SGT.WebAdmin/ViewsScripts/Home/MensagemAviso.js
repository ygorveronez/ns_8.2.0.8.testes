/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../js/Global/Rest.js" />
/// <reference path="../../js/Global/Mensagem.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/bootstrap/bootstrap.js" />
/// <reference path="../../js/libs/jquery.blockui.js" />
/// <reference path="../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../Configuracao/Sistema/ConfiguracaoTMS.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _mensagemAviso;

var MensagemAviso = function () {
    this.Mensagens = ko.observableArray();
    this.Titulo = ko.observableArray();
    this.Mensagem = ko.observableArray();
};

var MensagemModel = function () {
    this.Mensagem = PropertyEntity({});
    this.Titulo = PropertyEntity({});
    this.Anexos = ko.observableArray([]);
    this.DownloadAnexo = downloadAnexoAvisoClick;
}

//*******EVENTOS*******

function LoadMensagemAviso() {
    _mensagemAviso = new MensagemAviso();
    KoBindings(_mensagemAviso, "knockoutMensagemAviso");

    BuscarMensagensAviso();
}
function downloadAnexoAvisoClick(event, codigo) {
    if (event && event.preventDefault) event.preventDefault();

    executarDownload("Home/DownloadAnexoAviso", { Codigo: codigo });

    return false;
}

//*******MÉTODOS*******

function BuscarMensagensAviso() {
    _mensagemAviso.Mensagens.removeAll();
    executarReST("MensagemAviso/BuscarAtivas", {}, function (r) {
        if (r.Success) {
            if (r.Data != null && r.Data !== false && r.Data.length > 0) {
                for (var i = 0; i < r.Data.length; i++) {
                    var mensagem = new MensagemModel();

                    mensagem.Mensagem.val(tratarMensagemAviso(r.Data[i].Mensagem));
                    mensagem.Titulo.val(r.Data[i].Titulo);
                    mensagem.Anexos(r.Data[i].Anexos);

                    _mensagemAviso.Mensagens.push(mensagem);
                }

                $("#knockoutMensagemAviso").show();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    }, null, false);
}

function tratarMensagemAviso(mensagem) {
    if (!mensagem) return '';

    mensagem = mensagem.replace(/(https?:\/\/[^\s]+)|(www\.[^\s]+)/g, function (match) {
        if (match.startsWith("http")) {
            return '<a target="_blank" style="text-decoration: none !important; color: inherit" href="' + match + '">' + match + '</a>';
        } else {
            return '<a target="_blank" style="text-decoration: none !important; color: inherit" href="http://' + match + '">' + match + '</a>';
        }
    });

    mensagem = mensagem.replace(/\n/g, '<br/>');

    return mensagem;
}