/// <reference path="../../Enumeradores/EnumSituacaoAcompanhamentoArquivo.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoAcompanhamento.js" />
/// <reference path="../../Consultas/TermoQuitacaoFinaceiro.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />

//#region Variaveis Globais
var _pequisarAcompanhamentoContaAPagar;
var _gridAcompanhamentoContaAPagar;
var _divModalArquivoConta;
var _divModalLogErros;
var _gridLogErros;
//#endregion

//#region Constructores
var PequisarAcompanhamentoContaAPagar = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "", required: ko.observable(false), enable: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(""), def: "", required: ko.observable(false), enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoAcompanhamentoArquivo.Todos), options: EnumSituacaoAcompanhamentoArquivo.obterOpcoesPesquisa(), def: EnumSituacaoAcompanhamentoArquivo.Todos, required: false, enable: ko.observable(true) });
    this.TermoQuitacaoFinanceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Termo Quitação:", idBtnSearch: guid() });
    this.Importar = PropertyEntity({
        eventClick: importarArquivoCSV, type: types.event, text: "Importar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAcompanhamentoContaAPagar.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

function ModalLogErrors() {
    this.Grid = PropertyEntity({ idGrid: guid() })
    this.Codigo = PropertyEntity({ val: ko.observable(0) })
}
function ModalArquivoConta() {
    this.TipoDocumento = PropertyEntity({ text: "Tipo Documento: ", val: ko.observable(EnumTipoDocumentoAcompanhamento.SemTipo), options: EnumTipoDocumentoAcompanhamento.obterOpcoes(), def: EnumTipoDocumentoAcompanhamento.SemTipo, required: false, enable: ko.observable(true) });
    this.Documento = PropertyEntity({ text: ko.observable("Selecione Documento"), val: ko.observable("") });
    this.Documento.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _divModalArquivoConta.Documento.text(nomeArquivo);
    });
    this.Importar = PropertyEntity({ eventClick: EnviarArquivoDeConta, type: types.event, text: "Importar", visible: ko.observable(true) })
}


function loadAcompanhamentoContaAPagar() {
    _pequisarAcompanhamentoContaAPagar = new PequisarAcompanhamentoContaAPagar();
    KoBindings(_pequisarAcompanhamentoContaAPagar, "knockoutPesquisaAcompanhamentoContasAPagar");

    _divModalArquivoConta = new ModalArquivoConta();
    KoBindings(_divModalArquivoConta, "knockouModalArquivoConta");

    _divModalLogErros = new ModalLogErrors();
    KoBindings(_divModalLogErros, "knockouModalLogErros");

    new BuscarTermosQuitacaoFinanceiro(_pequisarAcompanhamentoContaAPagar.TermoQuitacaoFinanceiro,retornoTermoQuitacao);

    BuscarArquivosEmProcessamento();
}

function BuscarArquivosEmProcessamento() {
    var download = { descricao: "Download", id: guid(), metodo: baixaArquivo, tamanho: "15", icone: "" };
    var logErros = { descricao: "Log de erros", id: guid(), metodo: carregarLogerros, tamanho: "15", icone: "" };
    var reprocessar = { descricao: "Reprocessar", id: guid(), metodo: reprocessarArquivoContaPagar, tamanho: "15", icone: "" };
    var cancelamento = { descricao: "Cancelamento de Processamento", id: guid(), metodo: cancelarProcessamentoArquivo, tamanho: "15", icone: "" };

    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(download);
    menuOpcoes.opcoes.push(logErros);
    menuOpcoes.opcoes.push(reprocessar);
    menuOpcoes.opcoes.push(cancelamento);

    _gridAcompanhamentoContaAPagar = new GridView(_pequisarAcompanhamentoContaAPagar.Pesquisar.idGrid, "AcompanhamentoConta/Pesquisa", _pequisarAcompanhamentoContaAPagar, menuOpcoes);
    _gridAcompanhamentoContaAPagar.CarregarGrid();
}

function BuscarLogErros() {

    _gridLogErros = new GridView(_divModalLogErros.Grid.idGrid, "AcompanhamentoConta/LogErros", _divModalLogErros);
    _gridLogErros.CarregarGrid();
}

//#endregion

//#region Funções Auxiliares
function importarArquivoCSV() {
    LimparCampos(_divModalArquivoConta);
    Global.abrirModal("divModalAdicionarArquivo");
}
function FecharModalImportacao() {
    LimparCampos(_divModalArquivoConta);
    Global.fecharModal("divModalAdicionarArquivo");
}

function EnviarArquivoDeConta() {
    var file = document.getElementById(_divModalArquivoConta.Documento.id);
    var formData = new FormData();

    formData.append("File", file.files[0]);
    enviarArquivo("AcompanhamentoConta/ImportarPlanilhaParaProcessamento?callback=?", { TipoRegistro: _divModalArquivoConta.TipoDocumento.val() }, formData, function (arg) {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
        _gridAcompanhamentoContaAPagar.CarregarGrid();
        FecharModalImportacao();
    })
}

function cancelarProcessamentoArquivo(e) {
    executarReST("AcompanhamentoConta/CancelamentoProcessamento", { Codigo: e.Codigo }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
        _gridAcompanhamentoContaAPagar.CarregarGrid();
    })
}

function baixaArquivo(e) {
    executarDownload("AcompanhamentoConta/DownloadArquivos", { Codigo: e.Codigo })
}
function reprocessarArquivoContaPagar(e) {
    executarReST("AcompanhamentoConta/ReprocessarArquivo", { Codigo: e.Codigo }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
        _gridAcompanhamentoContaAPagar.CarregarGrid();
    })
}

function carregarLogerros(e) {
    _divModalLogErros.Codigo.val(e.Codigo);
    Global.abrirModal("divModalLogErros");
    BuscarLogErros();
}


function retornoTermoQuitacao(arg) {
    _pequisarAcompanhamentoContaAPagar.TermoQuitacaoFinanceiro.codEntity(arg.Codigo);
    _pequisarAcompanhamentoContaAPagar.TermoQuitacaoFinanceiro.val(arg.NumeroTermo);
}

//#endregion