//*******MAPEAMENTO KNOUCKOUT*******

var ImportacaoNOTFIS = function () {
    this.LayoutEDI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.LayoutEDI.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: true });
    this.TipoOcorrenciaImportacaoNOTFIS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.TipoOcorrencia.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: true });
    this.ArquivoNOTFIS = PropertyEntity({ type: types.event, idFile: guid(), accept: ".txt", text: Localization.Resources.Ocorrencias.Ocorrencia.EnviarOcoren, icon: "fal fa-file", visible: ko.observable(true) });
    

    this.UploadNOTFIS = PropertyEntity({
        eventClick: function (e) {
            ImportarNOTFISClick();
        }, type: types.event, text: "Importar", idGrid: guid(), icon: "fa fa-upload"
    });
};
var _modalKnockoutImportacaoNOTFIS;

//*******EVENTOS*******

function LoadImportacaoNOTFIS() {
    _importacaoNOTFIS = new ImportacaoNOTFIS();
    KoBindings(_importacaoNOTFIS, "knockoutImportacaoNOTFIS");

    new BuscarLayoutsEDI(_importacaoNOTFIS.LayoutEDI, null, null, null, null, [EnumTipoLayoutEDI.NOTFIS]);

    BuscarTipoOcorrencia(_importacaoNOTFIS.TipoOcorrenciaImportacaoNOTFIS);

    _modalKnockoutImportacaoNOTFIS = new bootstrap.Modal(document.getElementById("knockoutImportacaoNOTFIS"), { backdrop: true, keyboard: true });
}

function AbrirTelaImportacaoNOTFIS() {
    _modalKnockoutImportacaoNOTFIS.show();
}

function ImportarNOTFISClick() {
    
    var arquivo = document.getElementById(_importacaoNOTFIS.ArquivoNOTFIS.id);

    if (arquivo.files.length > 0) {
        
        if (ValidarCamposObrigatorios(_importacaoNOTFIS)) {

            var formData = new FormData();

            formData.append("LayoutEDI", _importacaoNOTFIS.LayoutEDI.codEntity());
            formData.append("TipoOcorrenciaImportacaoNOTFIS", _importacaoNOTFIS.TipoOcorrenciaImportacaoNOTFIS.codEntity());
            formData.append("ArquivoNOTFIS", arquivo.files[0]);
            
            enviarArquivo("Ocorrencia/ImportarNOTFIS?callback=?", null, formData, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.Sucesso);
                    }
                    else
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, retorno.Msg);
            });

        } else {
            exibirCamposObrigatorio();
        }
    }
}