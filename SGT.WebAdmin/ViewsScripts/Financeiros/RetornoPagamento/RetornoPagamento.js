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

var _retornoPagamento;

var RetornoPagamento = function () {
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo de Retorno:", val: ko.observable(""), visible: ko.observable(true) });
    this.BoletoConfiguracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Configuração Banco:", idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: false });

    this.Enviar = PropertyEntity({ eventClick: importarClick, type: types.event, text: "Importar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadRetornoPagamento() {
    _retornoPagamento = new RetornoPagamento();
    KoBindings(_retornoPagamento, "knockoutRetornoPagamento");

    new BuscarBoletoConfiguracao(_retornoPagamento.BoletoConfiguracao, RetornoBoletoConfiguracao, true);
}

function RetornoBoletoConfiguracao(data) {
    _retornoPagamento.BoletoConfiguracao.val(data.Descricao);
    _retornoPagamento.BoletoConfiguracao.codEntity(data.Codigo);
}

function importarClick(e, sender) {
    _retornoPagamento.BoletoConfiguracao.requiredClass("form-control");
    var valido = true;
    if (_retornoPagamento.BoletoConfiguracao.val() == "" || _retornoPagamento.BoletoConfiguracao.codEntity() == 0) {
        valido = false;
        _retornoPagamento.BoletoConfiguracao.requiredClass("form-control is-invalid");
    }

    if (valido) {
        var file = document.getElementById(_retornoPagamento.Arquivo.id);

        var formData = new FormData();
        formData.append("upload", file.files[0]);

        enviarArquivo("RetornoPagamento/EnviarRetorno?callback=?", { BoletoConfiguracao: _retornoPagamento.BoletoConfiguracao.codEntity() }, formData, function (arg) {
            if (arg.Success) {

                if (arg.Data !== null) {
                    var data = {
                        CodigosRetornos: arg.Data.CodigosRetornos
                    };
                    executarDownload("RetornoPagamento/DownloadRelatorioRetornoPagamento", data);
                    setTimeout(function () {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde a geração do relatório para conferência.");
                    }, 500);
                } else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", "Importação realizada, porem não foi encontrado nenhum registro no arquivo.");

                _retornoPagamento.Arquivo.val("");
                limparCamposRetornoPagamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    } else
        exibirMensagem(tipoMensagem.aviso, "Campos Obrigatórios", "Por favor verifique os campos obrigatórios.");
}

//*******MÉTODOS*******

function limparCamposRetornoPagamento() {
    LimparCampos(_retornoPagamento);
}
