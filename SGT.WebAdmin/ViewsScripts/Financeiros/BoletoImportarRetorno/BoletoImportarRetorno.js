/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumModalidadePessoa.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _importacaoDeRetorno;

var ImportacaoDeRetorno = function () {    
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo de Retorno:", val: ko.observable(""), visible: ko.observable(true) });
    this.Enviar = PropertyEntity({ eventClick: importarClick, type: types.event, text: "Importar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadImportacaoDeRetorno() {
    _importacaoDeRetorno = new ImportacaoDeRetorno();
    KoBindings(_importacaoDeRetorno, "knockoutImportacaoDeRetorno");

}

function importarClick(e, sender) {
    var file = document.getElementById(_importacaoDeRetorno.Arquivo.id);

    var formData = new FormData();
    formData.append("upload", file.files[0]);

    enviarArquivo("BoletoImportarRetorno/EnviarRetorno?callback=?", { Codigo: 0 }, formData, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo enviado ao servidor, visualizar o resultado pelo relatório de retorno.");
            _importacaoDeRetorno.Arquivo.val("");
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function limparCamposImportacaoDeRetorno() {
    LimparCampos(_importacaoDeRetorno);
}
