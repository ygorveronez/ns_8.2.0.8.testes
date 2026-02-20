var _importacaoEDIFatura;
var _modalImportacaoEDIFatura;
//*******MAPEAMENTO KNOUCKOUT*******

var ImportacaoEDIFatura = function () {
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Grupo de Pessoas:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), issue: 58 });
    this.Arquivo = PropertyEntity({ type: types.file, text: "*Arquivo:", val: ko.observable(""), visible: ko.observable(true) });

    this.Importar = PropertyEntity({ eventClick: EnviarArquivoEDIClick, type: types.event, text: "Importar", visible: ko.observable(true), enable: ko.observable(true), icon: "fal fa-chevron-down" });
    this.Fechar = PropertyEntity({ eventClick: FecharTelaImportacaoEDIFatura, type: types.event, text: "Fechar", visible: ko.observable(true), enable: ko.observable(true), icon: "fal fa-window-close" });
}


//*******EVENTOS*******

function LoadImportacaoEDIFatura() {
    _importacaoEDIFatura = new ImportacaoEDIFatura();
    KoBindings(_importacaoEDIFatura, "knockoutImportacaoEDI");

    _modalImportacaoEDIFatura = new bootstrap.Modal(document.getElementById("knockoutImportacaoEDI"), { backdrop: 'static', keyboard: true });

    new BuscarGruposPessoas(_importacaoEDIFatura.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    _modalImportacaoEDI = new bootstrap.Modal(document.getElementById("knockoutImportacaoEDI"), { backdrop: true, keyboard: true });
}


//*******MÉTODOS*******

function AbrirTelaImportacaoEDIFatura() {
    _modalImportacaoEDIFatura.show();
    $("#knockoutImportacaoEDI").one('hidden.bs.modal', function () {
        LimparCamposImportacaoEDI();
    });
}

function EnviarArquivoEDIClick() {
    var file = document.getElementById(_importacaoEDIFatura.Arquivo.id);
    var formData = new FormData();

    formData.append("upload", file.files[0]);

    enviarArquivo("Fatura/ImportarPreFatura?callback=?", { GrupoPessoas: _importacaoEDIFatura.GrupoPessoas.codEntity() }, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Pré fatura(s) importada(s) com sucesso.");

                _gridFatura.CarregarGrid();

                FecharTelaImportacaoEDIFatura();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });

}

function FecharTelaImportacaoEDIFatura() {
    _modalImportacaoEDIFatura.hide();
}

function LimparCamposImportacaoEDI() {
    LimparCampos(_importacaoEDIFatura);
    _importacaoEDIFatura.Arquivo.val("");
}