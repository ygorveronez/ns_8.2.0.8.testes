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
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoDocumentoCarga.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/JustificativaCancelamentoCarga.js" />
/// <reference path="../../Consultas/NotaFiscal.js" />
/// <reference path="./LoteComprovanteEntrega.js" />
/// <reference path="./ImagemNotaFiscal.js" />
/// <reference path="./Geolocalizacao.js" />


var _gridIntegracaoComprovante;
var _gridHistoricoIntegracaoCarga;
var _integracaoComprovanteEntrega;
var _pesquisaHistoricoIntegracaoCarga;

var _situacaoIntegracaoCarga = [{ value: "", text: "Todas" },
{ value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: "Aguardando Integração" },
{ value: EnumSituacaoIntegracaoCarga.AgRetorno, text: "Aguardando Retorno" },
{ value: EnumSituacaoIntegracaoCarga.Integrado, text: "Integrado" },
{ value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: "Falha na Integração" }];

var ContainerComprovanteEntrega = function () {

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Lote" });
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Carga" });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoCarga, text: "Situação:", def: "", issue: 272 });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Total Geral:" });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Integração:" });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Aguardando Retorno:" });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Problemas na Integração:" });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Integrados:" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridIntegracaoComprovante.CarregarGrid();
            ObterTotaisIntegracao();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosIntegracao();
        }, type: types.event, text: "Reenviar Todos", idGrid: guid(), visible: ko.observable(false)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracao();
        }, type: types.event, text: "Obter Totais", idGrid: guid(), visible: ko.observable(true)
    });
}

function LoadIntegracaoComprovanteEntrega(dadosComprovanteEntrega, idKnockoutIntegracaoCarga) {

    _integracaoComprovanteEntrega = new ContainerComprovanteEntrega();
    _integracaoComprovanteEntrega.Carga.val(dadosComprovanteEntrega.Carga.val());
    _integracaoComprovanteEntrega.Codigo.val(dadosComprovanteEntrega.Codigo.val());

    KoBindings(_integracaoComprovanteEntrega, idKnockoutIntegracaoCarga);
    _wizardLoteComprovanteEntrega.Etapa2.eventClick = function () { };
    _wizardLoteComprovanteEntrega.Etapa1.eventClick = function () { TornarEtapa1() };
}

function EfetuarPesquisaIntegracaoLote(codigo) {

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [] };
    menuOpcoes.opcoes.push({ descricao: "Reenviar", id: guid(), metodo: ReenviarIntegracaoCarga, tamanho: "20", icone: "", visibilidade: true });
    menuOpcoes.opcoes.push({ descricao: "Download", id: guid(), metodo: DownloadArquivos, tamanho: "20", icone: "", visibilidade: true });

    _integracaoComprovanteEntrega.Codigo.val(codigo);

    _gridIntegracaoComprovante = new GridView(_integracaoComprovanteEntrega.Pesquisar.idGrid, "LoteComprovanteEntregaXMLNotaFiscalIntegracao/Pesquisa", _integracaoComprovanteEntrega, menuOpcoes);
    _gridIntegracaoComprovante.CarregarGrid();

    ObterTotaisIntegracao();

    $("#" + _wizardLoteComprovanteEntrega.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#" + _wizardLoteComprovanteEntrega.Etapa1.idTab + " .step").attr("class", "step green");
    _CRUDLoteComprovanteEntrega.Adicionar.visible(false);
    _CRUDLoteComprovanteEntrega.Limpar.visible(false);
}

function TornarEtapa1() {
    _CRUDLoteComprovanteEntrega.Adicionar.visible(true);
    _CRUDLoteComprovanteEntrega.Limpar.visible(true);
}

function DownloadArquivos(data) {
    executarDownload("LoteComprovanteEntregaXMLNotaFiscalIntegracao/DownloadArquivosIntegracaoComprovanteEntrega", { Codigo: data.Codigo });
}

function ReenviarIntegracaoCarga(data) {
    executarReST("LoteComprovanteEntregaXMLNotaFiscalIntegracao/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
            _gridIntegracaoComprovante.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ReenviarTodosIntegracao() {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todas as integrações?", function () {
        executarReST("LoteComprovanteEntregaXMLNotaFiscalIntegracao/ReenviarTodos", { Codigo: _integracaoComprovanteEntrega.Codigo.val(), Tipo: _integracaoComprovanteEntrega.Tipo.val(), Situacao: _integracaoComprovanteEntrega.Situacao.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reenvio solicitado com sucesso.");
                _gridIntegracaoComprovante.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function ObterTotaisIntegracao() {
    executarReST("LoteComprovanteEntregaXMLNotaFiscalIntegracao/ObterTotais", { Codigo: _integracaoComprovanteEntrega.Codigo.val() }, function (r) {
        if (r.Success) {
            _integracaoComprovanteEntrega.TotalGeral.val(r.Data.TotalGeral);
            _integracaoComprovanteEntrega.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoComprovanteEntrega.TotalAguardandoRetorno.val(r.Data.TotalAguardandoRetorno);
            _integracaoComprovanteEntrega.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoComprovanteEntrega.TotalIntegrado.val(r.Data.TotalIntegrado);

            if (r.Data.TotalProblemaIntegracao > 0) {
                $("#" + _wizardLoteComprovanteEntrega.Etapa2.idTab + " .step").attr("class", "step red");
            } else if (r.Data.TotalIntegrado == r.Data.TotalGeral) {
                $("#" + _wizardLoteComprovanteEntrega.Etapa2.idTab + " .step").attr("class", "step green");
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function SignalRComprovantesEntregaStatus(retorno) {
    if (_integracaoComprovanteEntrega != null) {
        if (_integracaoComprovanteEntrega.Codigo.val() = retorno.Codigo) {
            _gridIntegracaoComprovante.CarregarGrid();
            ObterTotaisIntegracao();
        }
    }
}

