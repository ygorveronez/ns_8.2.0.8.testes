/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Enumeradores/EnumAbaGerarEscala.js" />
/// <reference path="../../Enumeradores/EnumSituacaoEscala.js" />
/// <reference path="../../Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="DadosEscala.js" />
/// <reference path="ExpedicaoEscala.js" />
/// <reference path="GerarEscalaEtapa.js" />
/// <reference path="VeiculoEscala.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDGerarEscala;
var _gerarEscala;
var _pesquisaGerarEscala;
var _gridGerarEscala;

/*
 * Declaração das Classes
 */

var GerarEscala = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.SituacaoEscala = PropertyEntity({ val: ko.observable(EnumSituacaoEscala.Todas), options: EnumSituacaoEscala.obterOpcoesPesquisa(), def: EnumSituacaoEscala.Todas, visible: false });
}

var PesquisaGerarEscala = function () {
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.NumeroEscala = PropertyEntity({ type: types.map, text: "Número da Escala:", val: ko.observable("") });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoEscala.Todas), options: EnumSituacaoEscala.obterOpcoesPesquisa(), def: EnumSituacaoEscala.Todas, text: "Situação: " });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento:", idBtnSearch: guid() });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridGerarEscala.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var CRUDGerarEscala = function () {
    this.AtualizarExpedicao = PropertyEntity({ eventClick: atualizarExpedicaoClick, type: types.event, text: "Salvar", visible: ko.observable(false) });
    this.AtualizarVeiculoEscala = PropertyEntity({ eventClick: atualizarVeiculoEscalaClick, type: types.event, text: "Salvar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarGeracaoEscalaClick, type: types.event, text: "Cancelar / Nova" });
    this.CriarEscala = PropertyEntity({ eventClick: adicionarEscalaClick, type: types.event, text: "Criar Escala", visible: ko.observable(true) });
    this.Finalizar = PropertyEntity({ eventClick: finalizarGeracaoEscalaClick, type: types.event, text: "Finalizar Escala", visible: ko.observable(false) });
    this.FinalizarExpedicao = PropertyEntity({ eventClick: finalizarExpedicaoClick, type: types.event, text: "Finalizar Expedição", visible: ko.observable(false) });
    this.GerarExcel = PropertyEntity({ eventClick: gerarExcelClick, type: types.event, text: "Gerar Planilha Excel", visible: ko.observable(false) });
    this.GerarPDF = PropertyEntity({ eventClick: gerarPDFClick, type: types.event, text: "Gerar PDF", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGerarEscala() {
    _gerarEscala = new GerarEscala();

    _pesquisaGerarEscala = new PesquisaGerarEscala();
    KoBindings(_pesquisaGerarEscala, "knockoutPesquisaGerarEscala");

    _CRUDGerarEscala = new CRUDGerarEscala();
    KoBindings(_CRUDGerarEscala, "knockoutCRUD");

    HeaderAuditoria("GeracaoEscala");

    new BuscarCentrosCarregamento(_pesquisaGerarEscala.CentroCarregamento);
    new BuscarProdutos(_pesquisaGerarEscala.Produto);

    loadGerarEscalaEtapa();
    loadDadosEscala();
    loadExpedicaoEscala();
    loadVeiculoEscala();
    loadGridGerarEscala();
}

function loadGridGerarEscala() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{ descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarGeracaoEscalaClick, tamanho: "15", icone: "" }] };

    _gridGerarEscala = new GridView(_pesquisaGerarEscala.Pesquisar.idGrid, "GerarEscala/Pesquisa", _pesquisaGerarEscala, menuOpcoes, null);
    _gridGerarEscala.CarregarGrid();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarEscalaClick() {
    adicionarEscala();
}

function atualizarExpedicaoClick() {
    atualizarExpedicaoEscala();
}

function atualizarVeiculoEscalaClick() {
    atualizarVeiculoEscala();
}

function cancelarGeracaoEscalaClick() {
    limparCamposGerarEscala();
}

function editarGeracaoEscalaClick(registroSelecionado) {
    editarGerarEscala(registroSelecionado.Codigo);
}

function finalizarExpedicaoClick() {
    finalizarExpedicaoEscala();
}

function finalizarGeracaoEscalaClick() {
    finalizarEscala();
}

function gerarExcelClick() {
    executarDownload("GerarEscala/Exportar", { Codigo: _gerarEscala.Codigo.val(), Tipo: EnumTipoArquivoRelatorio.XLS });
}

function gerarPDFClick() {
    executarDownload("GerarEscala/Exportar", { Codigo: _gerarEscala.Codigo.val(), Tipo: EnumTipoArquivoRelatorio.PDF });
}

/*
 * Declaração das Funções Públicas
 */

function controlarBotoesHabilitados() {
    var botaoAtualizarExpedicaoVisivel = false;
    var botaoAtualizarVeiculoEscalaVisivel = false;
    var botaoCriarEscalaVisivel = false;
    var botaoGerarExcel = false;
    var botaoGerarPDF = false;
    var botaoFinalizarVisivel = false;
    var botaoFinalizarExpedicaoVisivel = false;

    switch (_gerarEscala.SituacaoEscala.val()) {
        case EnumSituacaoEscala.Todas:
            botaoCriarEscalaVisivel = true;
            break;

        case EnumSituacaoEscala.EmCriacao:
            if (_abaAtiva === EnumAbaGerarEscala.Expedicao) {
                botaoAtualizarExpedicaoVisivel = true;
                botaoFinalizarExpedicaoVisivel = true;
            }
            break;

        case EnumSituacaoEscala.AgVeiculos:
            if (_abaAtiva === EnumAbaGerarEscala.Veiculo) {
                botaoAtualizarVeiculoEscalaVisivel = true;
                botaoFinalizarVisivel = true;
            }

            botaoGerarExcel = true;
            botaoGerarPDF = true;
            break;

        case EnumSituacaoEscala.Finalizada:
            botaoGerarExcel = true;
            botaoGerarPDF = true;
            break;
    }

    _CRUDGerarEscala.AtualizarExpedicao.visible(botaoAtualizarExpedicaoVisivel);
    _CRUDGerarEscala.AtualizarVeiculoEscala.visible(botaoAtualizarVeiculoEscalaVisivel);
    _CRUDGerarEscala.CriarEscala.visible(botaoCriarEscalaVisivel);
    _CRUDGerarEscala.Finalizar.visible(botaoFinalizarVisivel);
    _CRUDGerarEscala.FinalizarExpedicao.visible(botaoFinalizarExpedicaoVisivel);
    _CRUDGerarEscala.GerarExcel.visible(botaoGerarExcel);
    _CRUDGerarEscala.GerarPDF.visible(botaoGerarPDF);
}

function editarGerarEscala(codigo) {
    limparCamposGerarEscala();

    executarReST("GerarEscala/BuscarPorCodigo", { Codigo: codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaGerarEscala.ExibirFiltros.visibleFade(false);

                PreencherObjetoKnout(_gerarEscala, retorno);
                preencherDadosEscala(retorno.Data.DadosEscala);
                preencherExpedicaoEscala(retorno.Data.ExpedicoesEscala);
                preencherVeiculoEscala(retorno.Data.OrigensDestinosEscala);
                preencherVeiculoEscalaVeiculo();
                setarEtapas();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function isPermitirEditarDadosEscala() {
    return _gerarEscala.SituacaoEscala.val() == EnumSituacaoEscala.Todas;
}

function isPermitirEditarExpedicaoEscala() {
    return _gerarEscala.SituacaoEscala.val() == EnumSituacaoEscala.EmCriacao;
}

function isPermitirEditarVeiculoEscala() {
    return _gerarEscala.SituacaoEscala.val() == EnumSituacaoEscala.AgVeiculos;
}

function recarregarGridGerarEscala() {
    _gridGerarEscala.CarregarGrid();
}

/*
 * Declaração das Funções Privadas
 */

function limparCamposGerarEscala() {
    LimparCampos(_gerarEscala);
    limparCamposDadosEscala();
    limparCamposExpedicaoEscala();
    limparCamposVeiculoEscala();
    setarEtapaInicial();
    ocultarMensagemDadosNaoSalvos();
}

function exibirMensagemDadosNaoSalvos() {
    $("#mensagem-dados-nao-salvos").show();
}

function ocultarMensagemDadosNaoSalvos() {
    $("#mensagem-dados-nao-salvos").hide();
}
