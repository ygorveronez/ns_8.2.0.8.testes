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
/// <reference path="../../Consultas/Carregamento.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/PreCarga.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carga.js" />
/// <reference path="CarregamentoCarga.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="OrigemDestino.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />
/// <reference path="Carregamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _importacao;

var Importacao = function () {
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Arquivo.getRequiredFieldDescription(), val: ko.observable("") });
    this.ImportarMesmoSemCTeAbsorvidoAnteriormente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Cargas.MontagemCarga.ImportarMesmoSemCTeAbsorvidoAnteriormente, enable: ko.observable(true), visible: ko.observable(true) });
    this.Importar = PropertyEntity({ eventClick: importarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Importar, visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadImportacao() {
    _importacao = new Importacao();
    KoBindings(_importacao, "knoutAreaImportacao");

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
        $("#liImportacao").show();
    else
        $("#liImportacao").hide();
}

function importarClick(e, sender) {

    var file = document.getElementById(_importacao.Arquivo.id);

    var formData = new FormData();
    formData.append("upload", file.files[0]);

    enviarArquivo("MontagemCarga/Importar?callback=?", { ImportarMesmoSemCTeAbsorvidoAnteriormente: _importacao.ImportarMesmoSemCTeAbsorvidoAnteriormente.val() }, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCarga.ArquivoImportadoComSucessoPorFavorAguardeGeracaoDasCargas);
                _importacao.Arquivo.val("");
                limparDadosCarregamento();
                BuscarDadosMontagemCarga();
            } else {
                $("#knoutAreaImportacao").before('<p class="alert alert-info no-margin"><button class="close" data-dismiss="alert">×</button><i class="fal fa-info"></i><strong>' + Localization.Resources.Gerais.Geral.Atencao + '</strong> ' + Localization.Resources.Cargas.MontagemCarga.AlgunsRegistrosNaoForamImportados + ': <br />' + arg.Msg.replace(/\n/g, "<br />") + '</p >');
                _importacao.Arquivo.val("");
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function cancelarClick(e) {
    limparCamposImportacao();
}

//*******MÉTODOS*******

function limparCamposImportacao() {
    //LimparCampos(_importacao);
}
