/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoLancamentoNFSManual.js" />
/// <reference path="AutorizarRegras.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _lancamentoNFS;
var _pesquisaNFS;
var _rejeicao;
var _valores;
var _autorizacao;
var _gridNFS;
var _gridLancamentoNFSManualDesconto;


var _situacaoLancamentoNFS = [
    { text: "Todas", value: EnumSituacaoLancamentoNFSManual.Todas },
    { text: "Ag Aprovação", value: EnumSituacaoLancamentoNFSManual.AgAprovacao }
];

var RejeitarSelecionados = function () {
    this.Justificativa = PropertyEntity({ text: "*Justificativa:", type: types.entity, codEntity: ko.observable(0), required: true, enable: true, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarNFSSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var LancamentoNFS = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(true), val: ko.observable("") });
    this.NumeroCarga = PropertyEntity({ text: "Número da Carga: ", visible: ko.observable(true), val: ko.observable("") });
    this.Serie = PropertyEntity({ text: "Série: ", visible: ko.observable(true), val: ko.observable("") });
    this.DataCriacao = PropertyEntity({ text: "Data de Lançamento: ", visible: ko.observable(true), val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", visible: ko.observable(false), val: ko.observable("") });
    this.Transportador = PropertyEntity({ text: "Transportador: ", visible: ko.observable(true), val: ko.observable("") });
    this.Filial = PropertyEntity({ text: "Filial: ", visible: ko.observable(true), val: ko.observable("") });
    this.Tomador = PropertyEntity({ text: "Tomador: ", visible: ko.observable(true), val: ko.observable("") });
    this.ValorFrete = PropertyEntity({ text: "Valor Frete: ", visible: ko.observable(true), val: ko.observable("") });
    this.ValorFreteBruto = PropertyEntity({ text: "Valor Frete Bruto: ", val: ko.observable("") });
    this.Descontos = PropertyEntity({ text: "Descontos: ", val: ko.observable(""), popover: "<strong>Clique aqui para visualizar os detalhes</strong>", eventClick: exibirDetalhesDescontosClick });
    this.AliquotaISS = PropertyEntity({ text: "Aliquota ISS: ", visible: ko.observable(true), val: ko.observable("") });
    this.ValorISS = PropertyEntity({ text: "Valor ISS: ", visible: ko.observable(true), val: ko.observable("") });
    this.ValorBaseCalculo = PropertyEntity({ text: "Valor Base Cálculo: ", visible: ko.observable(true), val: ko.observable("") });
    this.ValorRetido = PropertyEntity({ text: "Valor ISS Retido: ", visible: ko.observable(true), val: ko.observable("") });
    this.PercentualRetencao = PropertyEntity({ text: "Percentual de Retenção: ", visible: ko.observable(true), val: ko.observable("") });

    this.DownloadAnexo = PropertyEntity({ eventClick: downloadAnexoClick, type: types.event, text: "Baixar Anexo", visible: ko.observable(true) });
    this.DownloadXML = PropertyEntity({ eventClick: downloadXMLClick, type: types.event, text: "Baixar XML", visible: ko.observable(true) });
    this.DownloadDANFSE = PropertyEntity({ eventClick: downloadDANFSEClick, type: types.event, text: "Baixar DANFSE", visible: ko.observable(true) });

    // Campos para filtro
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
}

var PesquisaNFS = function () {
    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.int, val: ko.observable(""), def: "" });

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoLancamentoNFSManual.AgAprovacao), options: _situacaoLancamentoNFS, def: EnumSituacaoLancamentoNFSManual.AgAprovacao, text: "Situação: " });
    this.Situacao.val.subscribe(function () {
        exibirMultiplasOpcoes();
    });

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });

    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });
    this.NumeroCarga = PropertyEntity({ text: "Número Carga:", getType: typesKnockout.int, val: ko.observable(""), def: "" });
    this.Solicitacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Solicitação:", idBtnSearch: guid() });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, buscaAvancada: true, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), cssClass: "btn btn-default", icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: true
    });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            BuscarLancamentosNFS();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasAvariasClick, text: "Aprovar Solicitações", visible: ko.observable(false) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasAvariasClick, text: "Rejeitar Solicitações", visible: ko.observable(false) });
}

//*******EVENTOS*******
function downloadAnexoClick(e) {
    executarDownload("NFSManual/DownloadAnexo", { Codigo: e.Codigo.val() });
}

function downloadXMLClick(e) {
    executarDownload("NFSManual/DownloadXML", { Codigo: e.Codigo.val() });
}

function downloadDANFSEClick(e) {
    executarDownload("NFSManual/DownloadDANFSE", { Codigo: e.Codigo.val() });
}

function loadAutorizacaoNFSManual() {
    _lancamentoNFS = new LancamentoNFS();
    KoBindings(_lancamentoNFS, "knockoutLancamentoNFS");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoMultiplasNFS");

    _pesquisaNFS = new PesquisaNFS();
    KoBindings(_pesquisaNFS, "knockoutPesquisaLancamentos");

    // Busca componentes pesquisa
    new BuscarFuncionario(_pesquisaNFS.Usuario);
    new BuscarFilial(_pesquisaNFS.Filial);
    new BuscarTransportadores(_pesquisaNFS.Transportador);
    new BuscarClientes(_pesquisaNFS.Tomador);
    new BuscarMotivoRejeicaoLancamentoNFS(_rejeicao.Justificativa);

    $('#' + _lancamentoNFS.Descontos.id + '-detalhes').popover();

    // Load modulos
    loadRegras();
    loadNFSes();
    loadGridLancamentoNFSManualDesconto();

    // Busca 
    BuscarLancamentosNFS();
}

function loadGridLancamentoNFSManualDesconto() {
    _gridLancamentoNFSManualDesconto = new GridView("grid-lancamento-nfs-manual-desconto", "NFSManual/PesquisaDescontos", _lancamentoNFS);
}

function rejeitarNFSSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as solicitações selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaNFS);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Justificativa = rejeicao.Justificativa;
        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaNFS.SelecionarTodos.val();
        dados.ItensSelecionadas = JSON.stringify(_gridNFS.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionadas = JSON.stringify(_gridNFS.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoNFS/ReprovarMultiplosLancamentos", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçadas de solicitações foram reprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçada de solicitação foi reprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    BuscarLancamentosNFS();
                    cancelarRejeicaoSelecionadosClick();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function cancelarRejeicaoSelecionadosClick() {
    LimparCampos(_rejeicao);
    Global.fecharModal('divModalRejeitarMultiplasNFS');
}

function rejeitarMultiplasAvariasClick() {
    LimparCampos(_rejeicao);
    Global.abrirModal('divModalRejeitarMultiplasNFS');
}

function exibirDetalhesDescontosClick() {    
    Global.abrirModal('divModalDadosEmissaoDescontos');
}


//*******MÉTODOS*******


function BuscarLancamentosNFS() {
    //-- Cabecalho
    var detalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharAutorizacao,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    //-- Reseta
    _pesquisaNFS.SelecionarTodos.val(false);
    _pesquisaNFS.AprovarTodas.visible(false);
    _pesquisaNFS.RejeitarTodas.visible(false);

    //-- Multi escolha
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaNFS.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configExportacao = {
        url: "AutorizacaoNFS/ExportarPesquisa",
        titulo: "Autorização Lançamento NFS"
    };

    _gridNFS = new GridView(_pesquisaNFS.Pesquisar.idGrid, "AutorizacaoNFS/Pesquisa", _pesquisaNFS, menuOpcoes, null, 25, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridNFS.CarregarGrid();
}

function exibirMultiplasOpcoes(e) {
    var situacaoPesquisa = _pesquisaNFS.Situacao.val();
    var possuiSelecionado = _gridNFS.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaNFS.SelecionarTodos.val();
    var situacaoPermiteSelecao = (situacaoPesquisa == EnumSituacaoLancamentoNFSManual.AgAprovacao);

    // Esconde todas opções
    _pesquisaNFS.AprovarTodas.visible(false);
    _pesquisaNFS.RejeitarTodas.visible(false);

    if (possuiSelecionado || selecionadoTodos) {
        if (situacaoPermiteSelecao) {
            _pesquisaNFS.AprovarTodas.visible(true);
            _pesquisaNFS.RejeitarTodas.visible(true);
        }
    }
}

function aprovarMultiplasAvariasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as solicitações selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaNFS);

        dados.SelecionarTodos = _pesquisaNFS.SelecionarTodos.val();
        dados.ItensSelecionadas = JSON.stringify(_gridNFS.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionadas = JSON.stringify(_gridNFS.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoNFS/AprovarMultiplosLancamentos", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçadas foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçada foi aprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    BuscarLancamentosNFS();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function detalharAutorizacao(itemGrid) {
    LimparCamposAutorizacao();
    var pesquisa = RetornarObjetoPesquisa(_pesquisaNFS);
    _lancamentoNFS.Codigo.val(itemGrid.Codigo);
    _lancamentoNFS.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_lancamentoNFS, "AutorizacaoNFS/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                AtualizarGridRegras();
                CarregarNFSes(arg.Data.Codigo);
                recarregarGridLancamentoNFSManualDesconto();

                if (arg.Data.PossuiAnexo)
                    _lancamentoNFS.DownloadAnexo.visible(true);
                else
                    _lancamentoNFS.DownloadAnexo.visible(false);

                // Abre modal 
                Global.abrirModal("divModalNFS");
                $("#divModalNFS").one('hidden.bs.modal', function () {
                    LimparCamposAutorizacao();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, null);
}

function LimparCamposAutorizacao() {
    resetarTabs();
    LimparRegras();
    limparNFSes();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}

function recarregarGridLancamentoNFSManualDesconto() {
    _gridLancamentoNFSManualDesconto.CarregarGrid();
}
