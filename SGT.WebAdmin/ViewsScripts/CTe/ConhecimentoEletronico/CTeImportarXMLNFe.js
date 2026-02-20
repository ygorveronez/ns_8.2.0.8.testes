/// <reference path="ConhecimentoEletronico.js" />
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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../CTe/CTe/CTe.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _ArquivosParaImportar, _CTeImportarXMLNFe, _modalCTeImportarXMLNFe;

var CTeImportarXMLNFe = function () {
    this.CTeImportarXMLNFe = PropertyEntity({ eventClick: CTeImportarXMLNFeClick, type: types.event, text: "Importar XML de NF-e" });
}

//*******EVENTOS*******

function LoadCTeImportarXMLNFe() {

    _CTeImportarXMLNFe = new CTeImportarXMLNFe();
    KoBindings(_CTeImportarXMLNFe, "divModalCTeImportarXMLNFe");

    $("#" + _pesquisaConhecimentoEletronico.ArquivoNFe.id).on("change", function () {
        AbrirTelaImportacaoXMLNFe(this.files);
    });

    $("#" + _pesquisaConhecimentoEletronico.ArquivoNFe.id).click(function () {
        $('[data-toggle="dropdown"]').parent().removeClass('open');
    });

    $("#knockoutCadastroCargaCTeManual").on('drag dragstart dragend dragover dragenter dragleave drop', function (e) {
        e.preventDefault();
        e.stopPropagation();
    })
        .on('dragover dragenter', function () {
            $("#knockoutCadastroCargaCTeManual").addClass('is-dragover');
        })
        .on('dragleave dragend drop', function () {
            $("#knockoutCadastroCargaCTeManual").removeClass('is-dragover');
        })
        .on('drop', function (e) {
            AbrirTelaImportacaoXMLNFe(e.originalEvent.dataTransfer.files);
        });

    _modalCTeImportarNFeSEFAZ = new bootstrap.Modal(document.getElementById("divModalCTeImportarXMLNFe"), { backdrop: 'static', keyboard: true });
}

function CTeImportarXMLNFeClick() {

    let formData = new FormData();

    for (var i = 0; i < _ArquivosParaImportar.length; i++)
        formData.append("upload" + i, _ArquivosParaImportar[i]);

    enviarArquivo("ConhecimentoEletronico/ObterInformacoesXMLNFe", {}, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {

                let sucesso = true;
                let documentos = new Array();

                for (let i = 0; i < arg.Data.length; i++) {
                    let ret = arg.Data[i];

                    if (!ret.Sucesso) {
                        sucesso = false;
                        $("#xmlNFeImportar_" + ret.Indice).removeClass("info").addClass("danger");
                        $("#xmlNFeImportar_" + ret.Indice + " .situacaoImportacaoXMLNFe").text(ret.Mensagem).prop("title", ret.Mensagem);
                    } else {
                        $("#xmlNFeImportar_" + ret.Indice).removeClass("info").addClass("success");
                        $("#xmlNFeImportar_" + ret.Indice + " .situacaoImportacaoXMLNFe").text("Arquivo válido.");
                        documentos.push(ret.Documento);
                    }
                }

                if (sucesso) {
                    _modalCTeImportarNFeSEFAZ.hide();
                    abrirModalCTe(0, documentos);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function HumanFileSize(size) {
    if (size == 0)
        return "0.00b"
    let i = Math.floor(Math.log(size) / Math.log(1024));
    return Globalize.format((size / Math.pow(1024, i)) * 1, "n2") + " " + ["b", "kb", "mb", "gb", "tb"][i];
};

function AbrirTelaImportacaoXMLNFe(files) {
    _ArquivosParaImportar = new Array();
    $("#tblXMLNFeImportar tbody").html("");

    for (let i = 0; i < files.length; i++) {
        var file = files[i];

        if (file.type == "text/xml") {
            $("#tblXMLNFeImportar tbody").append('<tr id="xmlNFeImportar_' + i + '" class="info"><td>' + (i + 1) + '</td><td>' + file.name + '</td><td>' + HumanFileSize(file.size) + '</td><td class="situacaoImportacaoXMLNFe">Não enviado.</td></tr>');
            _ArquivosParaImportar.push(file);
        }
    }

    if (_ArquivosParaImportar.length > 0)
        _modalCTeImportarNFeSEFAZ.show();
    else
        exibirMensagem(tipoMensagem.atencao, "Atenção!", "Somente são aceitos arquivos XML.");

    let fileControl = $("#" + _pesquisaConhecimentoEletronico.ArquivoNFe.id);
    fileControl.replaceWith(fileControl = fileControl.clone(true));
}