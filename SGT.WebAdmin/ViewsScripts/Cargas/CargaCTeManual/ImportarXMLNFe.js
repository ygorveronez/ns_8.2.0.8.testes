/// <reference path="CargaCTeManual.js" />
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
/// <reference path="Carga.js" />
/// <reference path="CargaCTe.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="Canhoto.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../CTe/CTe/CTe.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _ArquivosParaImportar, _ImportarXMLNFe;

var ImportarXMLNFe = function () {
    this.ImportarXMLNFe = PropertyEntity({ eventClick: ImportarXMLNFeClick, type: types.event, text: "Importar XML de NF-e" });
}

//*******EVENTOS*******

function LoadImportarXMLNFe() {

    _ImportarXMLNFe = new ImportarXMLNFe();
    KoBindings(_ImportarXMLNFe, "divModalImportarXMLNFe");

    $("#" + _carga.ArquivoNFe.id).on("change", function () {
        AbrirTelaImportacaoXMLNFe(this.files);
    });

    $("#" + _carga.ArquivoNFe.id).click(function () {
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
}

function ImportarXMLNFeClick() {

    let formData = new FormData();

    for (let i = 0; i < _ArquivosParaImportar.length; i++)
        formData.append("upload" + i, _ArquivosParaImportar[i]);

    enviarArquivo("CargaCTeManual/ObterInformacoesXMLNFe", {}, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {

                let sucesso = true;
                let documentos = new Array();

                if (arg.Data.length > 0) {
                    for (let i = 0; i < arg.Data.length; i++) {
                        let ret = arg.Data[i];

                        if (!ret.Sucesso) {
                            sucesso = false;
                            $("#xmlNFeImportar_" + ret.Indice).removeClass("bg-info-100").addClass("bg-danger-100");
                            $("#xmlNFeImportar_" + ret.Indice + " .situacaoImportacaoXMLNFe").text(ret.Mensagem).prop("title", ret.Mensagem);
                        } else {
                            if (ret.Documento.QuantidadesCarga == null || ret.Documento.QuantidadesCarga.length == 0)
                                ret.Documento.QuantidadesCarga = new Array();

                            ret.Documento.QuantidadesCarga.push({
                                Codigo: guid(),
                                UnidadeMedida: { Descricao: ret.Documento.UnidadeMedidaVolumes.Descricao, Codigo: ret.Documento.UnidadeMedidaVolumes.Codigo },
                                TipoMedida: ret.Documento.UnidadeMedidaVolumes.Descricao,
                                Quantidade: ret.Documento.Volume
                            });

                            ret.Documento.QuantidadesCarga.push({
                                Codigo: guid(),
                                UnidadeMedida: { Descricao: ret.Documento.UnidadeMedida.Descricao, Codigo: ret.Documento.UnidadeMedida.Codigo },
                                TipoMedida: ret.Documento.UnidadeMedida.Descricao,
                                Quantidade: ret.Documento.Peso
                            });

                            $("#xmlNFeImportar_" + ret.Indice).removeClass("bg-info-100").addClass("bg-success-100");
                            $("#xmlNFeImportar_" + ret.Indice + " .situacaoImportacaoXMLNFe").text("Arquivo válido.");
                            documentos.push(ret.Documento);
                        }
                    }
                }

                if (sucesso) {
                    Global.fecharModal('divModalImportarXMLNFe');
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
    var i = Math.floor(Math.log(size) / Math.log(1024));
    return Globalize.format((size / Math.pow(1024, i)) * 1, "n2") + " " + ["b", "kb", "mb", "gb", "tb"][i];
};

function AbrirTelaImportacaoXMLNFe(files) {
    _ArquivosParaImportar = new Array();
    $("#tblXMLNFeImportar tbody").html("");

    for (var i = 0; i < files.length; i++) {
        var file = files[i];

        if (file.type == "text/xml") {
            $("#tblXMLNFeImportar tbody").append('<tr id="xmlNFeImportar_' + i + '" class="bg-info-100"><td>' + (i + 1) + '</td><td>' + file.name + '</td><td>' + HumanFileSize(file.size) + '</td><td class="situacaoImportacaoXMLNFe">Não enviado.</td></tr>');
            _ArquivosParaImportar.push(file);
        }
    }

    if (_ArquivosParaImportar.length > 0)
        Global.abrirModal("divModalImportarXMLNFe");
    else
        exibirMensagem(tipoMensagem.atencao, "Atenção!", "Somente são aceitos arquivos XML.");

    var fileControl = $("#" + _carga.ArquivoNFe.id);
    fileControl.replaceWith(fileControl = fileControl.clone(true));
}