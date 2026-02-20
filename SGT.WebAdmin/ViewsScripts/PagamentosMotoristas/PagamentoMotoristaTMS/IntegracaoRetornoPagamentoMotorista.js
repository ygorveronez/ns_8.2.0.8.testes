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
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />
/// <reference path="PagamentoMotoristaTMS.js" />

var _gridIntegracaoRetorno;
var _integracaoRetorno;

var IntegracaoRetorno = function () {

    this.PagamentoMotorista = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridIntegracaoRetorno.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
}

function LoadIntegracaoRetorno(pagamentoMotorista, idKnockoutIntegracaoRetorno) {

    _integracaoRetorno = new IntegracaoRetorno();

    _integracaoRetorno.PagamentoMotorista.val(pagamentoMotorista.Codigo.val());

    KoBindings(_integracaoRetorno, idKnockoutIntegracaoRetorno);
    
    ConfigurarPesquisaIntegracaoRetorno();
}

function ConfigurarPesquisaIntegracaoRetorno() {
    var download = { descricao: "Download", id: guid(), metodo: DownloadIntegracaoRetorno, tamanho: "20", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [download,] };

    _gridIntegracaoRetorno = new GridView(_integracaoRetorno.Pesquisar.idGrid, "PagamentoMotoristaTMS/PesquisaRetorno", _integracaoRetorno, menuOpcoes);

    _gridIntegracaoRetorno.CarregarGrid();
}

function DownloadIntegracaoRetorno(data) {
    executarDownload("PagamentoMotoristaTMS/DownloadRetorno", { Codigo: data.Codigo });
}