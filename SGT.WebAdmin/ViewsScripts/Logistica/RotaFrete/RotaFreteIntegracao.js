/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaRotaFreteIntegracao;
var _gridRotaFreteIntegracao;
var _pesquisaHistoricoIntegracao;
var _dataGridCarregada = [];

var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

//*******MÉTODOS*******

function BuscarHistoricoIntegracaoRotaFrete() {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(_rotaFrete.Codigo.val());

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoRotaFrete, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "RotaFrete/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoRotaFrete(historicoConsulta) {
    executarDownload("RotaFrete/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo, CodigoRotaFrete: _rotaFrete.Codigo.val() });
}
