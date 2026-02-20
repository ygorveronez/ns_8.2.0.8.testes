/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridSolicitacaoFreteAnexo;
var _solicitacaoFrete;

/*
 * Declaração das Classes
 */

var SolicitacaoFrete = function () {
    this.Motivo = PropertyEntity({ text: "Motivo Solicitação de Frete: " });
    this.Observacao = PropertyEntity({ text: "Observação: " });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridSolicitacaoFreteAnexo() {
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadSolicitacaoFreteAnexoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 6, opcoes: [opcaoDownload] };

    _gridSolicitacaoFreteAnexo = new GridView("grid-solicitacao-frete-anexo", "CargaSolicitacaoFreteAnexo/PesquisaAnexo", _carga, menuOpcoes);
    _gridSolicitacaoFreteAnexo.CarregarGrid();
}

function loadSolicitacaoFrete() {
    _solicitacaoFrete = new SolicitacaoFrete();
    KoBindings(_solicitacaoFrete, "knockoutSolicitacaoFrete");

    loadGridSolicitacaoFreteAnexo();

    if (_CONFIGURACAO_TMS.ObrigarMotivoSolicitacaoFrete)
        $("#liSolicitacaoFrete").show();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function downloadSolicitacaoFreteAnexoClick(registroSelecionado) {
    executarDownload("CargaSolicitacaoFreteAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

/*
 * Declaração das Funções Públicas
 */

function limparSolicitacaoFrete() {
    LimparCampos(_solicitacaoFrete);
    recarregarGridSolicitacaoFreteAnexo();
}

function preencherSolicitacaoFrete(dadosSolicitacaoFrete) {
    PreencherObjetoKnout(_solicitacaoFrete, { Data: dadosSolicitacaoFrete });
    recarregarGridSolicitacaoFreteAnexo();
}

/*
 * Declaração das Funções Privadas
 */

function recarregarGridSolicitacaoFreteAnexo() {
    _gridSolicitacaoFreteAnexo.CarregarGrid();
}
