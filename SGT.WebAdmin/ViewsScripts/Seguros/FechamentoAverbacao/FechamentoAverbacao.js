/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFechamentoAverbacoes.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridFechamentoAverbacao;
var _fechamentoAverbacao;
var _CRUDFechamentoAverbacao;
var _pesquisaFechamentoAverbacao;

var FechamentoAverbacao = function () {
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoFechamentoAverbacoes.Todas), def: EnumSituacaoFechamentoAverbacoes.Todas });
}

var CRUDFechamentoAverbacao = function () {
    this.GerarFechamento = PropertyEntity({ eventClick: gerarFechamentoClick, type: types.event, text: "Gerar Fechamento", idGrid: guid(), visible: ko.observable(true) });
    this.Finalizar = PropertyEntity({ eventClick: finalizarClick, type: types.event, text: "Finalizar", idGrid: guid(), visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", idGrid: guid(), visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: limparFechamentoClick, type: types.event, text: "Limpar", idGrid: guid(), visible: ko.observable(false) });
}

var PesquisaFechamentoAverbacao = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int });

    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(false) });
    this.DataFim = PropertyEntity({ text: "Data Fim: ", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(false) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:",issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) { 
            _gridFechamentoAverbacao.CarregarGrid();
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


//*******EVENTOS*******

function loadFechamentoAverbacao() {
    _fechamentoAverbacao = new FechamentoAverbacao();

    _CRUDFechamentoAverbacao = new CRUDFechamentoAverbacao();
    KoBindings(_CRUDFechamentoAverbacao, "knockoutCRUD");

    _pesquisaFechamentoAverbacao = new PesquisaFechamentoAverbacao();
    KoBindings(_pesquisaFechamentoAverbacao, "knockoutPesquisaFechamentoAverbacao", false, _pesquisaFechamentoAverbacao.Pesquisar.id);

    loadEtapasFechamentoAverbacao();
    loadDadosFechamento();
    loadAverbacoes();

    HeaderAuditoria("FechamentoAverbacao", _fechamentoAverbacao);

    // Inicia as buscas
    new BuscarTransportadores(_pesquisaFechamentoAverbacao.Transportador);
    
    BuscarFechamentos();
}

function finalizarClick(e, sender) {
    exibirConfirmacao("Finalizar Fechamento", "Você tem certeza que deseja finalizar o fechamento?", function () {
        Salvar(_dadosFechamento, "FechamentoAverbacao/Finalizar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Finalizado com sucesso.");
                    _gridFechamentoAverbacao.CarregarGrid();
                    LimparCamposFechamento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function cancelarClick(e, sender) {
    exibirConfirmacao("Cancelar Fechamento", "Você tem certeza que deseja cancelar o fechamento?", function () {
        Salvar(_dadosFechamento, "FechamentoAverbacao/Cancelar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelado com sucesso.");
                    _gridFechamentoAverbacao.CarregarGrid();
                    LimparCamposFechamento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function gerarFechamentoClick(e, sender) {
    Salvar(_dadosFechamento, "FechamentoAverbacao/GerarFechamento", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridFechamentoAverbacao.CarregarGrid();
                BuscarFechamentoPorCodigo(arg.Data.Codigo);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function limparFechamentoClick() {
    LimparCamposFechamento();
}

//*******MÉTODOS*******
function BuscarFechamentos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarFechamentoAverbacao, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridFechamentoAverbacao = new GridView(_pesquisaFechamentoAverbacao.Pesquisar.idGrid, "FechamentoAverbacao/Pesquisa", _pesquisaFechamentoAverbacao, menuOpcoes);
    _gridFechamentoAverbacao.CarregarGrid();
}

function editarFechamentoAverbacao(itemGrid) {
    // Limpa os campos
    LimparCamposFechamento();

    // Esconde filtros
    _pesquisaFechamentoAverbacao.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarFechamentoPorCodigo(itemGrid.Codigo);
}

function BuscarFechamentoPorCodigo(codigo) {
    executarReST("FechamentoAverbacao/BuscarPorCodigo", {Codigo: codigo}, function (arg) {
        if (arg.Data != null) {
            // -- Fechamento Averbacao
            EditarFechamento(arg.Data);

            // -- Dados Fechamento 
            EditarDadosFechamento(arg.Data);

            // -- Averbacoes
            EditarAverbacoes(arg.Data);

            SetarEtapaFechamento();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function EditarFechamento(data) {
    _fechamentoAverbacao.Situacao.val(data.Situacao);
    _CRUDFechamentoAverbacao.GerarFechamento.visible(false);
    _CRUDFechamentoAverbacao.Limpar.visible(true);
    if (data.Situacao != EnumSituacaoFechamentoAverbacoes.Cancelada)
        _CRUDFechamentoAverbacao.Cancelar.visible(true);
    if (data.Situacao != EnumSituacaoFechamentoAverbacoes.Fechada && data.Situacao != EnumSituacaoFechamentoAverbacoes.Cancelada)
        _CRUDFechamentoAverbacao.Finalizar.visible(true);
}

function LimparCamposFechamento() {
    LimparCampos(_fechamentoAverbacao);
    _CRUDFechamentoAverbacao.GerarFechamento.visible(true);
    _CRUDFechamentoAverbacao.Finalizar.visible(false);
    _CRUDFechamentoAverbacao.Cancelar.visible(false);
    _CRUDFechamentoAverbacao.Limpar.visible(false);
    SetarEtapaInicioFechamento();

    LimparCamposDadosFechamento();
    LimparCamposAverbacoes();
}