/// <reference path="../../Consultas/Filial.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoProvisao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridProvisao;
var _provisao;
var _CRUDProvisao;
var _pesquisaProvisao;
var _mostrarReenviarLote = false;

var Provisao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0, text: "Situação: " });
    this.SituacaoNoCancelamento = PropertyEntity({ val: ko.observable(EnumSituacaoProvisao.Todos), options: EnumSituacaoProvisao.obterOpcoes(), def: EnumSituacaoProvisao.Todos, text: "Situação: " });
    this.GerandoMovimentoFinanceiroProvisao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.AgruparProvisoesPorNotaFiscalFechamentoMensal = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.MotivoRejeicaoFechamentoProvisao = PropertyEntity({});
}

var CRUDProvisao = function () {
    this.Limpar = PropertyEntity({ eventClick: limparProvisaoClick, type: types.event, text: "Limpar (Iniciar Nova Provisão)", idGrid: guid(), visible: ko.observable(false) });
}

var PesquisaProvisao = function () {

    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, visible: true, val: ko.observable() });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, visible: true });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.Numero = PropertyEntity({ text: "Número Provisão:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });
    this.NumeroDOC = PropertyEntity({ text: "Número NF-e:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });

    this.Ocorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Ocorrência:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid(), visible: ko.observable(true), required: true });

    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.LocalidadePrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local da Prestação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoProvisao.Todas), options: EnumSituacaoProvisao.obterOpcoesPesquisa(), def: EnumSituacaoProvisao.Todas, text: "Situação: " });
    this.TipoProvisao = PropertyEntity({ text: "Tipo Provisão", val: ko.observable(EnumTipoProvisao.Todas), options: EnumTipoProvisao.obterOpcoesPesquisa(), def: EnumTipoProvisao.Todas })

    this.ReenviarLotes = PropertyEntity({ eventClick: reenviarLotes, type: types.event, text: "Reenviar Lotes", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false) });
    this.ListaProvisoesSelecionadas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Situacao.val.subscribe(function (valor) {
        if (valor == 3)
            _mostrarReenviarLote = true;
        else
            _mostrarReenviarLote = false;
    });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _pesquisaProvisao.SelecionarTodos.val(false);
            _pesquisaProvisao.ReenviarLotes.visible(_mostrarReenviarLote);
            _pesquisaProvisao.SelecionarTodos.visible(_mostrarReenviarLote);
            _gridProvisao.CarregarGrid();
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
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

//*******EVENTOS*******

function loadProvisao() {
    _provisao = new Provisao();
    HeaderAuditoria("Provisao", _provisao, null, {
        DocumentoProvisao: "Número Documento"
    });

    _CRUDProvisao = new CRUDProvisao();
    KoBindings(_CRUDProvisao, "knockoutCRUD");

    _pesquisaProvisao = new PesquisaProvisao();
    KoBindings(_pesquisaProvisao, "knockoutPesquisaProvisao", false, _pesquisaProvisao.Pesquisar.id);

    loadFechamentoProvisao();
    loadSelecaoDocumentos();
    loadEtapasProvisao();
    loadIntegracao();
    loadIntegracaoCarga();
    loadResumo();
    LoadSignalRProvisao();

    //BuscarHTMLINtegracaoProvisao();
    // Inicia as buscas
    new BuscarTransportadores(_pesquisaProvisao.Empresa);
    new BuscarCargas(_pesquisaProvisao.Carga, null, null, null, null, null, null, null, null, true);
    new BuscarOcorrencias(_pesquisaProvisao.Ocorrencia);
    new BuscarClientes(_pesquisaProvisao.Tomador);
    new BuscarLocalidades(_pesquisaProvisao.LocalidadePrestacao);
    new BuscarFilial(_pesquisaProvisao.Filial);

    BuscarProvisao();
    obterConfiguracoesProvisaoPorNotaFiscal();
}

function limparProvisaoClick(e, sender) {
    LimparCamposProvisao();
    GridSelecaoDocumentos();
}

//*******MÉTODOS*******
function BuscarProvisao() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarProvisao, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaProvisao.SelecionarTodos,
        somenteLeitura: false,
    }

    var configExportacao = {
        url: "Provisao/ExportarPesquisa",
        titulo: "Provisão"
    };

    _gridProvisao = new GridView(_pesquisaProvisao.Pesquisar.idGrid, "Provisao/Pesquisa", _pesquisaProvisao, menuOpcoes, null, null, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridProvisao.CarregarGrid();
}

function editarProvisao(itemGrid) {
    // Limpa os campos
    LimparCamposProvisao();

    // Esconde filtros
    _pesquisaProvisao.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarProvisaoPorCodigo(itemGrid.Codigo);
}

function BuscarProvisaoPorCodigo(codigo, callback) {
    executarReST("Provisao/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            // -- Provisao
            EditarProvisao(arg.Data);

            // -- Selecao de Documentos
            EditarSelecaoDocumentos(arg.Data);

            PreecherResumo(arg.Data);

            CarregaIntegracao();

            CarregaIntegracaoCarga();

            SetarEtapasProvisao();

            if (!arg.Data.PossuiIntegracao && !arg.Data.PossuiIntegracaoEDI) {
                $("#provisao-com-integracao-container").hide();
                $("#provisao-sem-integracao-container").show();
            }
            else {
                $("#provisao-com-integracao-container").show();
                $("#provisao-sem-integracao-container").hide();

                $("#liIntegracaoEDI").toggle(arg.Data.PossuiIntegracaoEDI);
                $("#liIntegracaoCarga").toggle(arg.Data.PossuiIntegracao);

                Global.ResetarAba('provisao-com-integracao-container');
            }

            if (_provisao.Situacao.val() === EnumSituacaoProvisao.EmFechamento || _provisao.Situacao.val() === EnumSituacaoProvisao.PendenciaFechamento)
                $("#" + _etapaProvisao.Etapa2.idTab).click();

        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
        if (callback != null)
            callback();
    }, null);
}

function EditarProvisao(data) {
    _provisao.Codigo.val(data.Codigo);
    _provisao.Situacao.val(data.Situacao);
    _provisao.SituacaoNoCancelamento.val(data.SituacaoNoCancelamento);
    _provisao.GerandoMovimentoFinanceiroProvisao.val(data.GerandoMovimentoFinanceiroProvisao);
    _provisao.MotivoRejeicaoFechamentoProvisao.val(data.MotivoRejeicaoFechamentoProvisao);
    AtualizacaoEtapasPorEnumerador(data.TipoProvisao);
    _etapaProvisao.TipoProvisao.enable(false);
    _CRUDProvisao.Limpar.visible(true);
}

function LimparCamposProvisao() {
    LimparCampos(_provisao);
    _CRUDProvisao.Limpar.visible(false);
    _provisao.Situacao.val(EnumSituacaoProvisao.Todas);
    SetarEtapaInicio();
    Global.ResetarSteps();
    Global.ResetarAbas();
    LimparCamposSelecaoDocumentos();
    LimparResumo();
    limparCamposProcessaomentoFechamentoProvisao();
    _etapaProvisao.TipoProvisao.enable(true);

}

function reenviarLotes() {
    exibirConfirmacao("Confirmação", "Realmente deseja reenviar provisões aguardando integração?", function () {
        var provisoesSelecionadas;
        if (_pesquisaProvisao.SelecionarTodos.val())
            provisoesSelecionadas = _gridProvisao.ObterMultiplosNaoSelecionados();
        else
            provisoesSelecionadas = _gridProvisao.ObterMultiplosSelecionados();

        var codigosProvisoes = new Array();
        for (var i = 0; i < provisoesSelecionadas.length; i++)
            codigosProvisoes.push(provisoesSelecionadas[i].DT_RowId);

        if (codigosProvisoes.length > 0 || _pesquisaProvisao.SelecionarTodos.val())
            _pesquisaProvisao.ListaProvisoesSelecionadas.val(JSON.stringify(codigosProvisoes));
        else
            _pesquisaProvisao.ListaProvisoesSelecionadas.val("");

        executarReST("Provisao/ReenviarEmLotes", RetornarObjetoPesquisa(_pesquisaProvisao), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Provisões enviadas para reintegração com sucesso");
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    });
}

function obterConfiguracoesProvisaoPorNotaFiscal() {
    executarReST("Provisao/ObterConfiguracoesProvisaoPorNotaFiscal", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data.AgruparProvisoesPorNotaFiscalFechamentoMensal && arg.Data.EhPrimeiroOuSegundoDiaUtil) {
                _provisao.AgruparProvisoesPorNotaFiscalFechamentoMensal.val(true);
            }
        }
    })
}
