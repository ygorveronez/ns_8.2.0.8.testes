//#region Objetos Globais do Arquivo
var _gridIntegracaoGestaoDevolucao;
var _gridHistoricoIntegracaoGestaoDevolucao;
var _pesquisaHistoricoIntegracao;
var _integracoesGestaoDevolucao;
var _extrato;
// #endregion Objetos Globais do Arquivo

//#region Classes
var IntegracaoGestaoDevolucao = function () {
    this.GestaoDevolucao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Etapa = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral" });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Aguardando Integracao" });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Aguardando Retorno" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Problema Integracao" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Integrado" });

    this.Pesquisar = PropertyEntity({
        text: "Atualizar",
        eventClick: function (e) {
            _gridIntegracaoGestaoDevolucao.CarregarGrid();
            ObterTotaisIntegracaoGestaoDevolucao();
        }, type: types.event, idGrid: guid(), visible: ko.observable(true)
    });
}

var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var Extrato = function () {
    this.Valor = PropertyEntity({ val: ko.observable(0), text: "Valor" });
    this.DataCompensacao = PropertyEntity({ val: ko.observable(0), text: "Data da Compensação", getType: typesKnockout.date });
    this.NumeroCompensacao = PropertyEntity({ val: ko.observable(0), text: "Número da Compensação" });
    this.Fornecedor = PropertyEntity({ val: ko.observable(""), text: "Fornecedor" });
    this.DataVencimento = PropertyEntity({ val: ko.observable(""), text: "Data de vencimento" });
}
//#endregion Classes

// #region Funções Associadas a Eventos
// #endregion Funções Associadas a Eventos

// #region Funções Públicas
function loadGridGestaoDevolucaoIntegracao(idElementoGrid, codigoGestaoDevolucao, etapa = 0, dadosExtrato) {
    $.get("Content/Static/Carga/GestaoDevolucao/GestaoDevolucaoIntegracao.html?dyn=" + guid(), function (data) {
        $('#' + idElementoGrid).html(data);

        _integracoesGestaoDevolucao = new IntegracaoGestaoDevolucao();

        _integracoesGestaoDevolucao.GestaoDevolucao.val(codigoGestaoDevolucao);
        _integracoesGestaoDevolucao.Etapa.val(etapa);

        KoBindings(_integracoesGestaoDevolucao, "knockoutIntegracaoGestaoDevolucaoGrid");

        _extrato = new Extrato();
        KoBindings(_extrato, "knockoutExtrato");

        if (dadosExtrato)
            preencherExtrato(dadosExtrato);
        var menuOpcoes = null;

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
            menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [] };
            menuOpcoes.opcoes.push({ descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracaoGestaoDevolucao, tamanho: "20", icone: "" });
            menuOpcoes.opcoes.push({ descricao: "Histórico De Integracao", id: guid(), metodo: ExibirHistoricoIntegracaoGestaoDevolucao, tamanho: "20", icone: "" });
        }

        _gridIntegracaoGestaoDevolucao = new GridView(_integracoesGestaoDevolucao.Pesquisar.idGrid, `GestaoDevolucao/PesquisaIntegracoes`, _integracoesGestaoDevolucao, menuOpcoes, null, 25, null, null, null, null, null, null, null, null, null, null, callbackColumnGestaoDevolucaoIntegracao);
        _gridIntegracaoGestaoDevolucao.CarregarGrid();
        ObterTotaisIntegracaoGestaoDevolucao();
    });
}

function ExibirHistoricoIntegracaoGestaoDevolucao(integracao) {
    BuscarHistoricoIntegracaoGestaoDevolucao(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoGestaoDevolucao");
}
// #endregion Funções Públicas

// #region Funções de Inicialização
function abrirModalExtrato() {
    Global.abrirModal('divModalExtrato');
}
// #endregion Funções de Inicialização

// #region Funções Privadas
function ReenviarIntegracaoGestaoDevolucao(data) {
    executarReST("GestaoDevolucao/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Reenvio Solicitado Com Sucesso");
            _gridIntegracaoGestaoDevolucao.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}
function ObterTotaisIntegracaoGestaoDevolucao(data) {
    let dados = {
        GestaoDevolucao: _integracoesGestaoDevolucao.GestaoDevolucao.val(),
        Etapa: _integracoesGestaoDevolucao.Etapa.val()
    };
    executarReST("GestaoDevolucao/ObterTotais", dados, function (r) {
        if (r.Success) {
            _integracoesGestaoDevolucao.TotalGeral.val(r.Data.TotalGeral);
            _integracoesGestaoDevolucao.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracoesGestaoDevolucao.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracoesGestaoDevolucao.TotalIntegrado.val(r.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function BuscarHistoricoIntegracaoGestaoDevolucao(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoGestaoDevolucaoClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoGestaoDevolucao = new GridView("tblHistoricoIntegracaoGestaoDevolucao", "GestaoDevolucao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoGestaoDevolucao.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoGestaoDevolucaoClick(registroSelecionado) {
    executarDownload("GestaoDevolucao/DownloadArquivosHistoricoIntegracao", { Codigo: registroSelecionado.Codigo });
}

function callbackColumnGestaoDevolucaoIntegracao(cabecalho, valorColuna) {
    if (cabecalho.name == "Extrato")
        return obterHtmlColunaExtrato(valorColuna);
}

function obterHtmlColunaExtrato(valorColuna) {

    let cor = valorColuna == true ? '<a href="javascript:void(0);" onclick="abrirModalExtrato()" class="link" style="text-decoration: underline !important;">Visualizar</a>' : 'Não recebido';

    return cor;
}

function preencherExtrato(dadosExtrato) {
    _extrato.Valor.val(dadosExtrato.Valor);
    _extrato.DataCompensacao.val(dadosExtrato.DataCompensacao);
    _extrato.NumeroCompensacao.val(dadosExtrato.NumeroCompensacao);
    _extrato.Fornecedor.val(dadosExtrato.Fornecedor);
    _extrato.DataVencimento.val(dadosExtrato.DataVencimento);
}
// #endregion Funções Privadas