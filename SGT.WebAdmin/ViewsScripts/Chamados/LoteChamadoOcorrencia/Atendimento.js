/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Ocorrencia.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../Consultas/CTe.js" />
/// <reference path="../../Enumeradores/EnumTipoIrregularidade.js" />
/// <reference path="../../Enumeradores/EnumAcaoTratativaIrregularidade.js" />

// #region Objetos Globais do Arquivo

var _pesquisaChamadoOcorrenciaPendente;
var _gridAtendimentosPendentes;
var _atendimento;

// #endregion Objetos Globais do Arquivo

// #region Classes

var Atendimento = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial:", val: ko.observable(0), getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final:", val: ko.observable(0), getType: typesKnockout.int });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(""), def: "" });

    this.DataFinal.dateRangeInit = this.DataInicial;
    this.DataInicial.dateRangeLimit = this.DataFinal;

    this.NumeroCarga = PropertyEntity({ text: "Número Carga:", val: ko.observable("") });

    this.GrupoMotivoAtendimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo Motivo Atendimento:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.MotivoChamado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo Chamado:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente/Fornecedor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.NotaFiscal = PropertyEntity({ text: "Nota Fiscal:", val: ko.observable(0), getType: typesKnockout.int });


    this.Pesquisar = PropertyEntity({ eventClick: atualizarGridAtendimentosPendentes, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Selecionar Todos", visible: ko.observable(true) });

    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.SituacaoLote = PropertyEntity({ val: ko.observable(EnumSituacaoLoteChamadoOcorrencia.EmEdicao) });

    this.GerarLote = PropertyEntity({ eventClick: gerarLoteClick, type: types.event, text: "Gerar Lote", id: guid(), visible: ko.observable(false) });
    this.SalvarEdicao = PropertyEntity({ eventClick: salvarEdicaoClick, type: types.event, text: "Salvar Edição", id: guid(), visible: ko.observable(false) });
    this.DescartarEdicao = PropertyEntity({ eventClick: descartarEdicaoClick, type: types.event, text: "Descartar Edição", id: guid(), visible: ko.observable(false) });
    this.ExcluirLote = PropertyEntity({ eventClick: excluirLoteClick, type: types.event, text: "Excluir Lote", id: guid(), visible: ko.observable(false) });
};

function gerarLoteClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Ao confirmar a geração, este lote será enviado para aprovação e não poderá ser editado a não ser que seja rejeitado", function () {
        let registroSelecionados = _gridAtendimentosPendentes.ObterMultiplosSelecionados().map(x => x.Codigo);
        executarReST("LoteChamadoOcorrencia/GerarLote", { Atendimentos: JSON.stringify(registroSelecionados), Codigo: _atendimento.Codigo.val() }, (arg) => {
            if (!arg.Success)
                return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

            exibirMensagem(tipoMensagem.ok, "Sucesso", "Lote gerado com sucesso");
            LimparCamposAtendimento();
        })
    }, null, "Gerar lote", "Prefiro revisar");
}

function salvarEdicaoClick() {
    let registroSelecionados = _gridAtendimentosPendentes.ObterMultiplosSelecionados().map(x => x.Codigo);
    executarReST("LoteChamadoOcorrencia/SalvarEdicao", { Atendimentos: JSON.stringify(registroSelecionados), Codigo: _atendimento.Codigo.val() }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", "Edição salva com sucesso");
        LimparCamposAtendimento();
    })
}

function excluirLoteClick() {
    executarReST("LoteChamadoOcorrencia/ExcluirLote", { Codigo: _atendimento.Codigo.val() }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", "Lote excluído com sucesso");
        LimparCamposAtendimento();
    })
}


function descartarEdicaoClick() {
    LimparCamposAtendimento();
}


function atualizarGridAtendimentosPendentes() {
    _gridAtendimentosPendentes.CarregarGrid();
}

// #endregion Classes

// #region Funções de Inicialização


function loadAtendimentoLote() {
    _atendimento = new Atendimento();
    KoBindings(_atendimento, "knoutAtendimento", false, _atendimento.Pesquisar.id);

    HeaderAuditoria("LoteChamadoOcorrencia");

    BuscarTransportadores(_atendimento.Transportador, null, null, true);
    BuscarClientes(_atendimento.Cliente);
    BuscarMotivoChamado(_atendimento.MotivoChamado);
    BuscarGrupoMotivoChamado(_atendimento.MotivoChamado);

    loadGridAtendimentosPendentes();
}

function loadGridAtendimentosPendentes() {
    let detalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: exibirDetalhesLoteChamadoOcorrenciaClick, tamanho: "20", icone: "", visibilidade: true };

    let menuOpcoes = {
        tipo: TypeOptionMenu.list,

        opcoes: [detalhes],
        tamanho: 7
    };

    let multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _atendimento.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    let configExportacao = {
        url: "LoteChamadoOcorrencia/ExportarPesquisaAtendimentosPendentes",
        titulo: "Chamados"
    };

    _gridAtendimentosPendentes = new GridViewExportacao("grid-atendimentos-pendentes", "LoteChamadoOcorrencia/PesquisarAtendimentos", _atendimento, menuOpcoes, configExportacao, null, 10, multiplaEscolha, 50);
    _gridAtendimentosPendentes.SetPermitirEdicaoColunas(true);
    _gridAtendimentosPendentes.SetPermitirReordenarColunas(true);
    _gridAtendimentosPendentes.SetSalvarPreferenciasGrid(true);
    _gridAtendimentosPendentes.CarregarGrid();
}

// #endregion Funções de Inicialização

function exibirMultiplasOpcoes() {
    let existemRegistrosSelecionados = _gridAtendimentosPendentes.ObterMultiplosSelecionados().length > 0;
    let selecionadoTodos = _atendimento.SelecionarTodos.val();
    let emEdicao = _atendimento.SituacaoLote.val() == EnumSituacaoLoteChamadoOcorrencia.EmEdicao;

    _atendimento.GerarLote.visible((existemRegistrosSelecionados || selecionadoTodos) && emEdicao);
    _atendimento.SalvarEdicao.visible((existemRegistrosSelecionados || selecionadoTodos) && emEdicao);
    _atendimento.ExcluirLote.visible(emEdicao && _atendimento.Codigo.val() > 0);
    _atendimento.DescartarEdicao.visible(_atendimento.Codigo.val() > 0 || existemRegistrosSelecionados);
    _pesquisaLoteChamadoOcorrencia.ExibirFiltros.visibleFade(false);
}

function exibirDetalhesLoteChamadoOcorrenciaClick(registroSelecionado) {
    executarReST("LoteChamadoOcorrencia/ObterDetalhesAtendimento", { Codigo: registroSelecionado.Codigo }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);
        limparDetalhesAtendimento();
        preencherDadosModalDetalhes(arg.Data);
        Global.abrirModal("divModalDetalheLoteChamadoOcorrencia");
    })
}

function LimparCamposAtendimento() {
    _loteChamadoOcorrencia.Codigo.val(0);
    _atendimento.Codigo.val(0);
    _atendimento.SituacaoLote.val(EnumSituacaoLoteChamadoOcorrencia.EmEdicao);
    _loteChamadoOcorrencia.Situacao.val(EnumSituacaoLoteChamadoOcorrencia.EmEdicao);
    _gridAtendimentosPendentes.AtualizarRegistrosSelecionados([]);
    _gridAtendimentosPendentes.CarregarGrid();
    _gridLoteChamadoOcorrencia.CarregarGrid();
    exibirMultiplasOpcoes();
}
