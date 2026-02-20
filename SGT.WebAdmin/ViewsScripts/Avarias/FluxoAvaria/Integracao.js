/// <reference path="Lotes.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoLote.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _integracao;
var _gridIntegracao;

var _situacaoIntegracao = [{ value: "", text: "Todas" },
{ value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: "Aguardando Integração" },
{ value: EnumSituacaoIntegracaoCarga.Integrado, text: "Integrado" },
{ value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: "Falha na Integração" }];

var Integracao = function () {
    this.Lote = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracao, text: "Situação:", def: "", issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoEDI();
            _gridIntegracao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosIntegracao();
        }, type: types.event, text: "Reenviar Todos", idGrid: guid(), visible: ko.observable(true)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoEDI();
        }, type: types.event, text: "Obter Totais", idGrid: guid(), visible: ko.observable(true)
    });
}

//*******EVENTOS*******
function loadIntegracao() {
    _integracao = new Integracao();
    KoBindings(_integracao, "knockoutIntegracoesLotes");
}

function voltarEtapaClick() {
    var dados = {
        Codigo: _fluxoAvaria.Lote.val()
    }
    executarReST("Lotes/VoltarEtapa", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Lote pendente de aprovação.");
                editarLote({ Codigo: _fluxoAvaria.Lote.val() });
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function DownloadIntegracaoEDI(data) {
    executarDownload("LoteIntegracaoEDI/Download", { Codigo: data.Codigo });
}

function ReenviarIntegracaoEDI(data) {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar o arquivo EDI?", function () {
        executarReST("LoteIntegracaoEDI/Reenviar", { Codigo: data.Codigo }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                _gridIntegracao.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}


//*******MÉTODOS*******
function CarregaIntegracao() {
    _integracao.Lote.val(_fluxoAvaria.Lote.val());
    ObterTotaisIntegracaoEDI();
    ConfigurarPesquisaIntegracaoEDI();
}


function ObterTotaisIntegracaoEDI() {
    executarReST("LoteIntegracaoEDI/ObterTotais", { Lote: _integracao.Lote.val() }, function (r) {
        if (r.Success) {
            _integracao.TotalGeral.val(r.Data.TotalGeral);
            _integracao.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracao.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracao.TotalIntegrado.val(r.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ReenviarTodosIntegracao() {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todas as integrações de EDI?", function () {
        executarReST("LoteIntegracaoEDI/ReenviarTodos", { Lote: _integracao.Lote.val(), Situacao: _integracao.Situacao.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                _gridIntegracao.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}


function ConfigurarPesquisaIntegracaoEDI() {
    var download = { descricao: "Download", id: guid(), metodo: DownloadIntegracaoEDI, tamanho: "20", icone: "" };
    var reenviar = { descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracaoEDI, tamanho: "20", icone: "" };
    var auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("Lote"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };


    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [download, reenviar, auditar] };

    _gridIntegracao = new GridView(_integracao.Pesquisar.idGrid, "LoteIntegracaoEDI/PesquisaIntegracoesLoteAvaria", _integracao, menuOpcoes);
    _gridIntegracao.CarregarGrid();
}