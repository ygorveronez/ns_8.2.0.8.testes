/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
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
/// <reference path="Ocorrencia.js" />

var _ocorrenciaImagem;
var _gridImagens;

var OcorrenciaImagem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Imagens = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadOcorrenciaImagem() {
    _ocorrenciaImagem = new OcorrenciaImagem();
    KoBindings(_ocorrenciaImagem, "knockoutImagens");

    var baixarAnexo = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Baixar, id: guid(), metodo: BaixarAnexoClick, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Ocorrencias.Ocorrencia.Opcoes, tamanho: 15, opcoes: [baixarAnexo] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoOcorrencia", visible: false },
        { data: "Arquivo", title: Localization.Resources.Ocorrencias.Ocorrencia.CaminhoNomeArquivo, width: "70%" }
    ];

    _gridImagens = new BasicDataTable(_ocorrenciaImagem.Imagens.idGrid, header, menuOpcoes);

    recarregarGridListaImagens();
}

function BaixarAnexoClick(data) {
    executarDownload("OcorrenciaImagem/DownloadImagem", { CodigoImagem: data.Codigo });
}

//*******MÉTODOS*******

function recarregarGridListaImagens() {

    var data = new Array();

    $.each(_ocorrencia.ListaImagens.list, function (i, listaImagens) {
        var listaImagensGrid = new Object();

        listaImagensGrid.Codigo = listaImagens.Codigo.val;
        listaImagensGrid.CodigoOcorrencia = _ocorrencia.Codigo.val();
        listaImagensGrid.Arquivo = listaImagens.Arquivo.val;

        data.push(listaImagensGrid);
    });

    _gridImagens.CarregarGrid(data);
}

function limparCamposOcorrenciaImagem() {
    LimparCampos(_ocorrenciaImagem);
}