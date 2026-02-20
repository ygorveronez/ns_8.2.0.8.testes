/// <reference path="ContratoFreteAcrescimoDesconto.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridIntegracoes;
var _integracao;
var _pesquisaIntegracoes;
var _pesquisaHistoricoIntegracao;
var _gridHistoricoIntegracao;
var _CRUDIntegracao, _detalheRejeicao;

/*
 * Declaração das Classes
 */

var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function Integracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Retorno:" });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracao, type: types.event, text: "Obter Totais", idGrid: guid(), visible: ko.observable(true) });
}

function PesquisaIntegracoes() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: "Situação:", def: "", issue: 272 });

    this.Pesquisar = PropertyEntity({ eventClick: carregarIntegracoes, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.ReenviarTodos = PropertyEntity({ eventClick: reenviarTodasIntegracoesClick, type: types.event, text: "Reenviar Todas", visible: ko.observable(false) });
}

var DetalheRejeicao = function () {
    this.MotivoRejeicao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Motivo da rejeição da aplicação do valor no contrato:", enable: ko.observable(true) });
};

var CRUDIntegracao = function () {
    this.CancelarLancamento = PropertyEntity({ eventClick: CancelarLancamentoClick, type: types.event, text: "Cancelar Lançamento", visible: ko.observable(false) });
    this.LiberarIntegracaoRejeitada = PropertyEntity({ eventClick: liberarIntegracaoRejeitadaClick, type: types.event, text: "Liberar mesmo com Integração Rejeitada", visible: ko.observable(false) });
    this.ReaplicarValorRejeitado = PropertyEntity({ eventClick: reaplicarValorRejeitadoClick, type: types.event, text: "Aplicar o valor rejeitado ao contrato", visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridIntegracoes() {
    var linhasPorPaginas = 5;
    var opcaoIntegrar = { descricao: "Integrar", id: guid(), metodo: integrarClick, icone: "" };
    var historico = { descricao: "Histórico de Integração", id: guid(), metodo: exibirHistoricoIntegracoesClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [opcaoIntegrar, historico] };

    _gridIntegracoes = new GridView(_pesquisaIntegracoes.Pesquisar.idGrid, "ContratoFreteAcrescimoDescontoIntegracao/PesquisaIntegracoes", _pesquisaIntegracoes, menuOpcoes, null, linhasPorPaginas);
}

function loadIntegracoes() {
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        $("#tabIntegracoesConteudo").append(html);

        LocalizeCurrentPage();

        _integracao = new Integracao();
        KoBindings(_integracao, "knockoutDadosIntegracao");

        _pesquisaIntegracoes = new PesquisaIntegracoes();
        KoBindings(_pesquisaIntegracoes, "knockoutPesquisaIntegracao", false, _pesquisaIntegracoes.Pesquisar.id);

        _CRUDIntegracao = new CRUDIntegracao();
        KoBindings(_CRUDIntegracao, "knockoutCRUDIntegracao");

        _detalheRejeicao = new DetalheRejeicao();
        KoBindings(_detalheRejeicao, "knockoutDetalheRejeicao");

        loadGridIntegracoes();
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function exibirHistoricoIntegracoesClick(integracao) {
    BuscarHistoricoIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracao");
}

function integrarClick(registroSelecionado) {
    executarReST("ContratoFreteAcrescimoDescontoIntegracao/Integrar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarIntegracoes();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
        }
    });
}

function reenviarTodasIntegracoesClick() {

}

function DownloadArquivosHistoricoIntegracaoClick(historicoConsulta) {
    executarDownload("ContratoFreteAcrescimoDescontoIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo, TipoIntegracao: historicoConsulta.TipoIntegracao });
}

function CancelarLancamentoClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente cancelar o lançamento e reverter os lançamentos gerados?", function () {
        executarReST("ContratoFreteAcrescimoDesconto/Cancelar", { Codigo: _contratoFreteAcrescimoDesconto.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Acréscimo/Desconto do Contrato de Frete cancelado com sucesso.");
                    LimparCamposContratoFreteAcrescimoDesconto();
                    _gridContratoFreteAcrescimoDesconto.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function liberarIntegracaoRejeitadaClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente liberar mesmo com a integração rejeitada?", function () {
        executarReST("ContratoFreteAcrescimoDescontoIntegracao/LiberarComIntegracaoRejeitada", { Codigo: _contratoFreteAcrescimoDesconto.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Integração rejeitada liberada com sucesso.");
                    LimparCamposContratoFreteAcrescimoDesconto();
                    _gridContratoFreteAcrescimoDesconto.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
            }
        });
    });
}

function reaplicarValorRejeitadoClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente tentar aplicar o valor ao contrato?", function () {
        executarReST("ContratoFreteAcrescimoDescontoIntegracao/ReaplicarValorRejeitado", { Codigo: _contratoFreteAcrescimoDesconto.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Valor aplicado com sucesso.");
                    LimparCamposContratoFreteAcrescimoDesconto();
                    _gridContratoFreteAcrescimoDesconto.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
            }
        });
    });
}

/*
 * Declaração das Funções Públicas
 */

function ControleCamposIntegracao() {
    var situacao = _contratoFreteAcrescimoDesconto.Situacao.val();

    $("#knockoutDetalheRejeicao").hide();
    _CRUDIntegracao.CancelarLancamento.visible(false);
    _CRUDIntegracao.LiberarIntegracaoRejeitada.visible(false);
    _CRUDIntegracao.ReaplicarValorRejeitado.visible(false);

    if (situacao === EnumSituacaoContratoFreteAcrescimoDesconto.FalhaIntegracao && (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFreteAcrescimoDesconto_PermiteLiberarComIntegracaoRejeitada, _PermissoesPersonalizadas))) {
        _CRUDIntegracao.LiberarIntegracaoRejeitada.visible(true);
        _CRUDIntegracao.CancelarLancamento.visible(true);
    }
    else if (situacao === EnumSituacaoContratoFreteAcrescimoDesconto.AplicacaoValorRejeitado) {
        $("#knockoutDetalheRejeicao").show();
        _CRUDIntegracao.ReaplicarValorRejeitado.visible(true);
    }
}

function recarregarIntegracoes() {
    _pesquisaIntegracoes.Codigo.val(_contratoFreteAcrescimoDesconto.Codigo.val());

    carregarIntegracoes();
}

/*
 * Declaração das Funções
 */

function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "ContratoFreteAcrescimoDescontoIntegracao/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function carregarIntegracoes() {
    _gridIntegracoes.CarregarGrid();

    carregarTotaisIntegracao();
}

function carregarTotaisIntegracao() {
    executarReST("ContratoFreteAcrescimoDescontoIntegracao/ObterTotaisIntegracoes", { Codigo: _pesquisaIntegracoes.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _integracao.TotalGeral.val(retorno.Data.TotalGeral);
            _integracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _integracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _integracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _integracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
        }
    });
}

function LimparCamposIntegracao() {
    LimparCampos(_detalheRejeicao);
}