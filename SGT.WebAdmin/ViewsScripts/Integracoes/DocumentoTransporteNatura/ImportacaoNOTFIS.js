var _notfisDocumentoTransporteNatura;

var NOTFISDocumentoTransporteNatura = function () {
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Arquivos = ko.observableArray([]);

    this.Importar = PropertyEntity({ eventClick: ImportarNOTFISClick, type: types.event, text: "Importar", icon: "fal fa-upload", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: FecharImportacaoNOTFISClick, type: types.event, text: "Fechar", icon: "fal fa-window-close", visible: ko.observable(true) });
}

function LoadNOTFISDocumentoTransporteNatura() {
    _notfisDocumentoTransporteNatura = new NOTFISDocumentoTransporteNatura();
    KoBindings(_notfisDocumentoTransporteNatura, "knockoutImportacaoEDI");

    $("#" + _pesquisaDocumentoTransporteNatura.ArquivoNOTFIS.id).on("change", function () {
        AbrirTelaImportacaoNOTFIS(this.files);
    });

    new BuscarEmpresa(_notfisDocumentoTransporteNatura.Empresa);
}

function ImportarNOTFISClick() {
    if (_notfisDocumentoTransporteNatura.Empresa.codEntity() <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Empresa é obrigatório!", "Selecione uma empresa/filial para iniciar a importação.");
        return;
    }

    _notfisDocumentoTransporteNatura.Importar.visible(false);
    _notfisDocumentoTransporteNatura.Fechar.visible(false);

    ImportarArquivo(_notfisDocumentoTransporteNatura.Arquivos().length, 0);
}

function ImportarArquivo(quantidade, i) {
    _notfisDocumentoTransporteNatura.Arquivos()[i].Enviando(true);

    var formData = new FormData();
    formData.append("upload", _notfisDocumentoTransporteNatura.Arquivos()[i].Arquivo);

    enviarArquivo("DocumentoTransporteNatura/ImportarNOTFIS?callback=?", { Empresa: _notfisDocumentoTransporteNatura.Empresa.codEntity(), ID: _notfisDocumentoTransporteNatura.Arquivos()[i].ID }, formData, function (arg) {

        _notfisDocumentoTransporteNatura.Arquivos()[i].Enviando(false);

        if (arg.Success && arg.Data) {
            if (!arg.Data.Sucesso) {
                _notfisDocumentoTransporteNatura.Arquivos()[i].Class("danger");
                _notfisDocumentoTransporteNatura.Arquivos()[i].Mensagem(arg.Data.Mensagem);
            } else {
                _notfisDocumentoTransporteNatura.Arquivos()[i].Class("success");
                _notfisDocumentoTransporteNatura.Arquivos()[i].Mensagem("Importado com sucesso.");
            }
        } else {
            _notfisDocumentoTransporteNatura.Arquivos()[i].Class("danger");
            _notfisDocumentoTransporteNatura.Arquivos()[i].Mensagem(arg.Msg);
        }

        if (quantidade > (i + 1))
            ImportarArquivo(quantidade, i + 1);
        else {
            _gridDocumentoTransporteNatura.CarregarGrid();
            _notfisDocumentoTransporteNatura.Fechar.visible(true);
        }

    }, null, false);
}

function HumanFileSize(size) {
    if (size == 0)
        return "0.00b"
    var i = Math.floor(Math.log(size) / Math.log(1024));
    return Globalize.format((size / Math.pow(1024, i)) * 1, "n2") + " " + ["b", "kb", "mb", "gb", "tb"][i];
};

function AbrirTelaImportacaoNOTFIS(files) {
    LimparCamposImportacaoNOTFIS();

    for (var i = 0; i < files.length; i++) {
        var file = files[i];

        _notfisDocumentoTransporteNatura.Arquivos.push({ ID: guid(), Arquivo: file, Descricao: file.name, Tamanho: HumanFileSize(file.size), Class: ko.observable("info"), Mensagem: ko.observable(""), Enviando: ko.observable(false) });
    }

    if (_notfisDocumentoTransporteNatura.Arquivos().length > 0) {
        var modalImportacaoEdi = new bootstrap.Modal(document.getElementById("knockoutImportacaoEDI"), { backdrop: true, keyboard: true });
        modalImportacaoEdi.show();
    }

    $("#" + _pesquisaDocumentoTransporteNatura.ArquivoNOTFIS.id).val("");
}

function FecharImportacaoNOTFISClick() {
    Global.fecharModal('knockoutImportacaoEDI');
}

function LimparCamposImportacaoNOTFIS() {
    LimparCampos(_notfisDocumentoTransporteNatura);
    _notfisDocumentoTransporteNatura.Importar.visible(true);
    _notfisDocumentoTransporteNatura.Fechar.visible(true);
    _notfisDocumentoTransporteNatura.Arquivos.removeAll();
}
