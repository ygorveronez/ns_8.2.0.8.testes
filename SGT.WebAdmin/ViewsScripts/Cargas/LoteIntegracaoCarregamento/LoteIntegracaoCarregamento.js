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


//*******MAPEAMENTO KNOUCKOUT*******

var _carregamentoIntegracao;
var _gridSelecaoCarregamentos;
var _dataGridCarregada = [];
var _indexCarregamentoAberto = -1;
var _pesquisaEntregaIntegracao;
var _gridEntregaIntegracao;

var _situacaoIntegracaoCarregamento = [{ value: "", text: "Todas" },
{ value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: "Aguardando Integração" },
{ value: EnumSituacaoIntegracaoCarga.AgRetorno, text: "Aguardando Retorno" },
{ value: EnumSituacaoIntegracaoCarga.Integrado, text: "Integrado" },
{ value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: "Falha na Integração" }];
var dataAtual = moment().add(-2, 'days').format("DD/MM/YYYY");

var PesquisaIntegracao = function () {
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.NumeroCarregamento = PropertyEntity({ val: ko.observable(""), text: "Número Carregamento: " });
    this.Carga = PropertyEntity({ val: ko.observable(""), text: "Carga: " });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoCarregamento, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridEntregaIntegracao.CarregarGrid();
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

var PesquisaCarregamento = function () {
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.NumeroCarregamento = PropertyEntity({ val: ko.observable(""), text: "Número Carregamento: " });
    this.Carga = PropertyEntity({ val: ko.observable(""), text: "Carga: " });
    this.Emitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Emitente:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.ListaCodigosCarregamentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });

    this.Pesquisar = PropertyEntity({ eventClick: function (e) { _gridSelecaoCarregamentos.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: ko.observable(true) });
    this.GerarIntegracao = PropertyEntity({ eventClick: GerarIntegracaoCarregamentos, type: types.event, text: "Gerar Integração", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadLoteIntegracaoCarregamento() {
    _carregamentoIntegracao = new PesquisaCarregamento();
    KoBindings(_carregamentoIntegracao, "knockoutPesquisaCarregamento", false, _carregamentoIntegracao.Pesquisar.id);

    _pesquisaEntregaIntegracao = new PesquisaIntegracao();
    KoBindings(_pesquisaEntregaIntegracao, "knockoutPesquisaIntegracoes", false, _pesquisaEntregaIntegracao.Pesquisar.id);

    new BuscarClientes(_carregamentoIntegracao.Emitente);
    BuscarIntegracoesCarregamento();
    BuscarCarregamentos();

    loadEtapaLoteIntegracao();
    loadIntegracaoLoteCarregamentos();

}


//*******MÉTODOS*******

function BuscarIntegracoesCarregamento() {
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [] };
    menuOpcoes.opcoes.push({ descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracao, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: "Histórico de Integração", id: guid(), metodo: ExibirHistoricoIntegracao, tamanho: "20", icone: "" });

    var configExportacao = {
        url: "LoteIntegracaoCarregamento/ExportarPesquisaIntegracoes",
        titulo: "Integrações Lote Carregamento"
    };

    _gridEntregaIntegracao = new GridViewExportacao(_pesquisaEntregaIntegracao.Pesquisar.idGrid, "LoteIntegracaoCarregamento/PesquisarIntegracoes", _pesquisaEntregaIntegracao, menuOpcoes, configExportacao, null, 10);
    _gridEntregaIntegracao.CarregarGrid();
}

function ReenviarIntegracao(data) {
    executarReST("LoteIntegracaoCarregamento/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
            _gridEntregaIntegracao.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ExibirHistoricoIntegracao(integracao) {
    BuscarHistoricoIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracao");
}


function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracao, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "LoteIntegracaoCarregamento/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracao(historicoConsulta) {
    executarDownload("LoteIntegracaoCarregamento/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function BuscarCarregamentos() {
    var menuOpcoes = null;

    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: function () { },
        callbackNaoSelecionado: function () { },
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _carregamentoIntegracao.SelecionarTodos,
        somenteLeitura: false
    };

    var configExportacao = {
        url: "LoteIntegracaoCarregamento/ExportarPesquisaCarregamentos",
        titulo: "Integrações Lote Carregamento"
    };

    _gridSelecaoCarregamentos = new GridViewExportacao(_carregamentoIntegracao.ListaCodigosCarregamentos.idGrid, "LoteIntegracaoCarregamento/PesquisaCarregamentos", _carregamentoIntegracao, menuOpcoes, configExportacao, null, 10, multiplaescolha);
    _gridSelecaoCarregamentos.CarregarGrid();
}

function CancelarClick() {
    setarEtapaInicio();
    LimparRegistrosSelecionados();
}

function LimparRegistrosSelecionados() {
    _carregamentoIntegracao.ListaCodigosCarregamentos.val("");
    _carregamentoIntegracao.SelecionarTodos.val(false);
    _gridSelecaoCarregamentos.AtualizarRegistrosSelecionados([]);
    _gridSelecaoCarregamentos.AtualizarRegistrosNaoSelecionados([]);
    _gridSelecaoCarregamentos.CarregarGrid();
}


function GerarIntegracaoCarregamentos(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente gerar integração do(s) carregamento(s) selecionado(s)?", function () {
        if (!PreencherListasSelecao())
            return;

        var dados = RetornarObjetoPesquisa(_carregamentoIntegracao);

        dados.SelecionarTodos = _carregamentoIntegracao.SelecionarTodos.val();
        dados.CarregamentosSelecionados = JSON.stringify(_gridSelecaoCarregamentos.ObterMultiplosSelecionados());
        dados.CarregamentosNaoSelecionados = JSON.stringify(_gridSelecaoCarregamentos.ObterMultiplosNaoSelecionados());

        executarReST("LoteIntegracaoCarregamento/CriarRegistroIntegracao", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Integração carregamentos em lote criado com sucesso");

                    _integracaoLoteCarregamento.Codigo.val(retorno.Data.Codigo);
                    $("#step1").removeClass('active');
                    $("#step2").addClass('active');
                    ConsultaIntegracao();
                    //BuscarNFSPorCodigo(retorno.Data.Codigo);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
};

function PreencherListasSelecao() {
    var carregamentosSelecionados = null;
    var codigoscarregamentos = new Array();

    if (_carregamentoIntegracao.SelecionarTodos.val())
        carregamentosSelecionados = _gridSelecaoCarregamentos.ObterMultiplosNaoSelecionados();
    else
        carregamentosSelecionados = _gridSelecaoCarregamentos.ObterMultiplosSelecionados();

    for (var i = 0; i < carregamentosSelecionados.length; i++)
        codigoscarregamentos.push(carregamentosSelecionados[i].DT_RowId);

    if (codigoscarregamentos && (codigoscarregamentos.length > 0 || _carregamentoIntegracao.SelecionarTodos.val())) {
        _carregamentoIntegracao.ListaCodigosCarregamentos.val(JSON.stringify(codigoscarregamentos));
        return true;
    } else {
        _carregamentoIntegracao.ListaCodigosCarregamentos.val("");
        exibirMensagem(tipoMensagem.atencao, "Atenção!", "É necessário selecionar ao menos um carregamento para gerar a integração.");
        return false;
    }
}
