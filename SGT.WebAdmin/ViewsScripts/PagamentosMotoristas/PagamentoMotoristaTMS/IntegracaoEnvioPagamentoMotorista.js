/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPagamentoMotorista.js" />
/// <reference path="PagamentoMotoristaTMS.js" />
/// <reference path="IntegracaoRetornoPagamentoMotorista.js" />

var _gridIntegracaoEnvio;
var _integracaoEnvio;

var _situacaoIntegracaoEnvio = [
    { value: "", text: "Todas" },
    { value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: "Aguardando Integração" },
    { value: EnumSituacaoIntegracaoCarga.Integrado, text: "Integrado" },
    { value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: "Falha na Integração" }
];

var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};
var IntegracaoEnvio = function () {
    this.PagamentoMotorista = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoEnvio, text: "Situação:", def: "" });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoEnvio();
            _gridIntegracaoEnvio.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosIntegracaoEnvio();
        }, type: types.event, text: "Reenviar Todos", idGrid: guid(), visible: ko.observable(false)
    });

    this.AvancarSemIntegracao = PropertyEntity({
        eventClick: function (e) {
            AvancarSemIntegracaoEnvio();
        }, type: types.event, text: "Avançar sem Integração", idGrid: guid(), visible: ko.observable(false)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoEnvio();
        }, type: types.event, text: "Obter Totais", idGrid: guid(), visible: ko.observable(true)
    });
};

function LoadIntegracaoEnvio(pagamentoMotorista, idKnockoutIntegracaoEnvio) {
    _integracaoEnvio = new IntegracaoEnvio();

    _integracaoEnvio.PagamentoMotorista.val(pagamentoMotorista.Codigo.val());
    _integracaoEnvio.Situacao.val(pagamentoMotorista.Situacao.val());
    _integracaoEnvio.AvancarSemIntegracao.visible(pagamentoMotorista.Situacao.val() === EnumSituacaoPagamentoMotorista.FalhaIntegracao);

    KoBindings(_integracaoEnvio, idKnockoutIntegracaoEnvio);

    ObterTotaisIntegracaoEnvio();
    ConfigurarPesquisaIntegracaoEnvio();
}

function ConfigurarPesquisaIntegracaoEnvio() {
    const download = { descricao: "Histórico de Integração", id: guid(), metodo: hitoricoIntegracaoClick, tamanho: "20", icone: "" };
    const reenviar = { descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracaoEnvio, tamanho: "20", icone: "", visibilidade: PodeReenviarIntegracao };

    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [download, reenviar] };

    _gridIntegracaoEnvio = new GridView(_integracaoEnvio.Pesquisar.idGrid, "PagamentoMotoristaTMS/PesquisaEnvio", _integracaoEnvio, menuOpcoes);

    _gridIntegracaoEnvio.CarregarGrid();
}

function PodeReenviarIntegracao(data) {
    return data.PodeReenviar;
}

function hitoricoIntegracaoClick(integracao){
    BuscarHistoricoIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracao");
}

function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "PagamentoMotoristaTMS/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoClick(historico){
    executarDownload("PagamentoMotoristaTMS/DownloadArquivosHistoricoIntegracao", { Codigo: historico.Codigo });
}

function DownloadIntegracaoEnvio(data) {
    executarDownload("PagamentoMotoristaTMS/DownloadEnvio", { Codigo: data.Codigo });
}

function ReenviarIntegracaoEnvio(data) {
    executarReST("PagamentoMotoristaTMS/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                buscarPagamentoMotoristaPorCodigo(AtualizarTotalizadores);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ReenviarTodosIntegracaoEnvio() {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todos os pagamentos?", function () {
        executarReST("PagamentoMotoristaTMS/ReenviarTodos", { PagamentoMotorista: _integracaoEnvio.PagamentoMotorista.val(), Situacao: _integracaoEnvio.Situacao.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                    buscarPagamentoMotoristaPorCodigo(AtualizarTotalizadores);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function AvancarSemIntegracaoEnvio() {
    exibirConfirmacao("Atenção!", "Deseja realmente avançar sem integração?", function () {
        executarReST("PagamentoMotoristaTMS/AvancarSemIntegracao", { CodigoPagamentoMotorista: _integracaoEnvio.PagamentoMotorista.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Avanço efetuado com sucesso.");

                    if (!string.IsNullOrWhiteSpace(r.Data.MensagemRetorno))
                        exibirMensagem(tipoMensagem.aviso, "Aviso", r.Data.MensagemRetorno);

                    buscarPagamentoMotoristaPorCodigo(AtualizarTotalizadores);
                    setarEtapaInicioPagamentoMotorista();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function AtualizarTotalizadores() {
    ObterTotaisIntegracaoEnvio();
    _gridIntegracaoEnvio.CarregarGrid();
    _gridIntegracaoRetorno.CarregarGrid();
    $("#" + _etapaPagamentoMotorista.Etapa3.idTab).click();
    finalizarRequisicao();
}

function ObterTotaisIntegracaoEnvio() {
    executarReST("PagamentoMotoristaTMS/ObterTotaisEnvio", { PagamentoMotorista: _integracaoEnvio.PagamentoMotorista.val() }, function (r) {
        if (r.Success) {
            _integracaoEnvio.TotalGeral.val(r.Data.TotalGeral);
            _integracaoEnvio.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoEnvio.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoEnvio.TotalIntegrado.val(r.Data.TotalIntegrado);
            _integracaoEnvio.ReenviarTodos.visible(r.Data.PodeReenviarTodos);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

