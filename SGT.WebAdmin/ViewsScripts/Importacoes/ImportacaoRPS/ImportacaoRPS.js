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

//*******MAPEAMENTO KNOUCKOUT*******

var _importacaoRPS;
var _tabelaFrete;

var ImportacaoTabelaRPS = function () {
    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",

        UrlImportacao: "ImportacaoRPS/Importar",
        UrlConfiguracao: "ImportacaoRPS/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O010_TabelaRPS,
    });
}

//*******EVENTOS*******

function loadImportacaoRPS() {
    _importacaoRPS = new ImportacaoTabelaRPS();
    KoBindings(_importacaoRPS, "knockoutImportacaoRPS");

}

function importarClick(e, sender) {
    var file = document.getElementById(_importacaoRPS.Arquivo.id);

    var formData = new FormData();
    formData.append("upload", file.files[0]);
    
    enviarArquivo("ImportacaoRPS/Importar?callback=?", {}, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo importado com sucesso");
            } else {
                $("#knockoutImportacaoTabelaFrete").before('<p class="alert alert-info no-margin"><button class="close" data-dismiss="alert">×</button><i class="fa-fw fa fa-info"></i><strong>Atenção!</strong> Alguns registros não foram importados:<br/>' + arg.Msg.replace(/\n/g, "<br />") + '</p>');
                _importacaoRPS.Arquivo.val("");
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}
