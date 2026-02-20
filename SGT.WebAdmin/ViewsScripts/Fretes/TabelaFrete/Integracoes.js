/// <reference path="TabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

// #region Objetos Globais do Arquivo

var _gridTabelaFreteIntegracoes;
var _tabelaFreteIntegracao;
var _pesquisaTabelaFreteIntegracoes;
var _problemaTabelaFreteIntegracao;
var _pesquisaHistoricoIntegracao;
var _gridHistoricoIntegracao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function PesquisaTabelaFreteIntegracoes() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarIntegracoes, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: "", type: types.event, text: Localization.Resources.Gerais.Geral.ReenviarTodas, visible: ko.observable(false) });
};

function TabelaFreteIntegracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoRetorno.getFieldDescription() });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.TotalGeral.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Integrados.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.ProblemasIntegracao.getFieldDescription() });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracao, type: types.event, text: Localization.Resources.Gerais.Geral.ObterTotais, idGrid: guid(), visible: ko.observable(true) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadGridTabelaFreteIntegracoes() {
    var linhasPorPaginas = 5;

    var historico = { descricao: Localization.Resources.Gerais.Geral.HistoricoIntegracao, id: guid(), metodo: exibirHistoricoIntegracoesClick, icone: "" };
    var reenviarIntegracao = { descricao: Localization.Resources.Gerais.Geral.Reenviar, id: guid(), metodo: reenviarIntegracoesClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [historico, reenviarIntegracao] };

    _gridTabelaFreteIntegracoes = new GridView(_pesquisaTabelaFreteIntegracoes.Pesquisar.idGrid, "TabelaFreteIntegracao/PesquisaTabelaFreteIntegracoes", _pesquisaTabelaFreteIntegracoes, menuOpcoes, null, linhasPorPaginas);
    _gridTabelaFreteIntegracoes.CarregarGrid();
}

function loadTabelaFreteIntegracoes() {
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        $("#tabIntegracoesConteudo").append(html);

        LocalizeCurrentPage();

        _tabelaFreteIntegracao = new TabelaFreteIntegracao();
        KoBindings(_tabelaFreteIntegracao, "knockoutDadosIntegracao");

        _pesquisaTabelaFreteIntegracoes = new PesquisaTabelaFreteIntegracoes();
        KoBindings(_pesquisaTabelaFreteIntegracoes, "knockoutPesquisaIntegracao", false, _pesquisaTabelaFreteIntegracoes.Pesquisar.id);

        loadGridTabelaFreteIntegracoes();
    });
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function exibirHistoricoIntegracoesClick(integracao) {
    BuscarHistoricoIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracao");
}

function DownloadArquivosHistoricoIntegracaoClick(historicoConsulta) {
    executarDownload("TabelaFreteIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function reenviarIntegracoesClick(integracao) {
    executarReST("TabelaFreteIntegracao/ReenviarIntegracao", { Codigo: integracao.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Fretes.TabelaFrete.Sucesso, Localization.Resources.Gerais.Geral.ReenvioSolicitadoComSucesso);
                carregarIntegracoes();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Fretes.TabelaFrete.Falha, retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function recarregarTabelaFreteIntegracoes() {
    if (_pesquisaTabelaFreteIntegracoes != null) {
        _pesquisaTabelaFreteIntegracoes.Codigo.val(_tabelaFrete.Codigo.val());

        controlarExibicaoAbaIntegracoes(false);
        carregarIntegracoes();
    }
}

// #endregion Funções Públicas

// #region Funções Privadas

function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "TabelaFreteIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function carregarIntegracoes() {
    _gridTabelaFreteIntegracoes.CarregarGrid();

    carregarTotaisIntegracao();
}

function carregarTotaisIntegracao() {
    executarReST("TabelaFreteIntegracao/ObterTotaisIntegracoes", { Codigo: _pesquisaTabelaFreteIntegracoes.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _tabelaFreteIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _tabelaFreteIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _tabelaFreteIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _tabelaFreteIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _tabelaFreteIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function controlarExibicaoAbaIntegracoes(IntegracaoOpentech) {
    if (IntegracaoOpentech)
        $("#liTabIntegracoes").show();

    if (_pesquisaTabelaFreteIntegracoes.Codigo.val() > 0 && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
        $("#liTabIntegracoes").show();
    else
        $("#liTabIntegracoes").hide();
}

function fecharModalProblemaIntegracao() {
    Global.fecharModal("divModalMotivoProblemaIntegracao");
}

// #endregion Funções Privadas
