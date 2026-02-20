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

var _gridLoteChamadoOcorrencia;
var _pesquisaLoteChamadoOcorrencia;
var _rejeicao;
var _PermissoesPersonalizadasOcorrencia = {};
var _loteChamadoOcorrencia;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaLoteChamadoOcorrencia = function () {
    this.NumeroLote = PropertyEntity({ text: "Número Lote:", val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(1), options: EnumSituacaoLoteChamadoOcorrencia.obterOpcoes(), def: "", visible: ko.observable(true), getType: typesKnockout.selectMultiple });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.DataCriacaoInicial = PropertyEntity({ text: "Data de Criação Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataCriacaoFinal = PropertyEntity({ text: "Data de Criação Final:", getType: typesKnockout.date, val: ko.observable(""), def: "" });

    this.DataCriacaoFinal.dateRangeInit = this.DataCriacaoInicial;
    this.DataCriacaoInicial.dateRangeLimit = this.DataCriacaoFinal;
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Selecionar Todos", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({ eventClick: atualizarGridLoteChamadoOcorrencias, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var LoteChamadoOcorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoLoteChamadoOcorrencia.EmEdicao) });

};



// #endregion Classes

// #region Funções de Inicialização

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.UsarAlcadaAprovacaoLoteChamadoOcorrencias)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaLoteChamadoOcorrencia.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaLoteChamadoOcorrencia.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function atualizarGridLoteChamadoOcorrencias() {
    _gridLoteChamadoOcorrencia.CarregarGrid();
}

function loadLoteChamadoOcorrencia() {
    _pesquisaLoteChamadoOcorrencia = new PesquisaLoteChamadoOcorrencia();
    KoBindings(_pesquisaLoteChamadoOcorrencia, "knockoutPesquisaLoteChamadoOcorrencia", false, _pesquisaLoteChamadoOcorrencia.Pesquisar.id);

    _loteChamadoOcorrencia = new LoteChamadoOcorrencia();

    HeaderAuditoria("LoteChamadoOcorrencia");

    BuscarTransportadores(_pesquisaLoteChamadoOcorrencia.Transportador, null, null, true);

    loadGridLoteChamadoOcorrencia();
    loadAtendimentoLote();
    LoadEtapaLoteChamadoOcorrencia();
    loadDetalhesAtendimento();
}

function loadGridLoteChamadoOcorrencia() {
    let menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarLoteChamadoOcorrencia }] };

    let configExportacao = {
        url: "LoteChamadoOcorrencia/ExportarPesquisa",
        titulo: "Chamados"
    };

    _gridLoteChamadoOcorrencia = new GridViewExportacao("grid-lote-chamado-ocorrencia", "LoteChamadoOcorrencia/Pesquisa", _pesquisaLoteChamadoOcorrencia, menuOpcoes, configExportacao, null, 10, null, 50);
    _gridLoteChamadoOcorrencia.SetPermitirReordenarColunas(true);
    _gridLoteChamadoOcorrencia.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos



function editarLoteChamadoOcorrencia(e) {
    executarReST("LoteChamadoOcorrencia/BuscarPorCodigo", { Codigo: e.Codigo }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);
        _atendimento.Codigo.val(e.Codigo);
        _atendimento.SituacaoLote.val(arg.Data.Situacao);
        _gridAtendimentosPendentes.AtualizarRegistrosSelecionados(arg.Data.AtendimentosSelecionados);
        _gridAtendimentosPendentes.CarregarGrid();
        exibirMultiplasOpcoes();
        SetarEtapaLoteChamadoOcorrencia();
        DefinirTab();
    })
}



// #endregion Funções Associadas a Eventos
