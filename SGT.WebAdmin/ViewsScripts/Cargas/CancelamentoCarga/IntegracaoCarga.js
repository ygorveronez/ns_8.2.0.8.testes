/// <autosync enabled="true" />
/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Global/SignalR/SignalR.js" />
/// <reference path="../../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../DadosEmissao/Configuracao.js" />
/// <reference path="../DadosEmissao/DadosEmissao.js" />
/// <reference path="../DadosEmissao/Geral.js" />
/// <reference path="../DadosEmissao/Lacre.js" />
/// <reference path="../DadosEmissao/LocaisPrestacao.js" />
/// <reference path="../DadosEmissao/Observacao.js" />
/// <reference path="../DadosEmissao/Passagem.js" />
/// <reference path="../DadosEmissao/Percurso.js" />
/// <reference path="../DadosEmissao/Rota.js" />
/// <reference path="../DadosEmissao/Seguro.js" />
/// <reference path="../DadosTransporte/DadosTransporte.js" />
/// <reference path="../DadosTransporte/Motorista.js" />
/// <reference path="../DadosTransporte/Tipo.js" />
/// <reference path="../DadosTransporte/Transportador.js" />
/// <reference path="../Documentos/CTe.js" />
/// <reference path="../Documentos/MDFe.js" />
/// <reference path="../Documentos/NFS.js" />
/// <reference path="../Documentos/PreCTe.js" />
/// <reference path="../DocumentosEmissao/CargaCancelamentoPedidoDocumentoCTe.js" />
/// <reference path="../DocumentosEmissao/ConsultaReceita.js" />
/// <reference path="../DocumentosEmissao/CTe.js" />
/// <reference path="../DocumentosEmissao/Documentos.js" />
/// <reference path="../DocumentosEmissao/DropZone.js" />
/// <reference path="../DocumentosEmissao/EtapaDocumentos.js" />
/// <reference path="../DocumentosEmissao/NotaFiscal.js" />
/// <reference path="../Frete/Complemento.js" />
/// <reference path="../Frete/Componente.js" />
/// <reference path="../Frete/EtapaFrete.js" />
/// <reference path="../Frete/Frete.js" />
/// <reference path="../Frete/SemTabela.js" />
/// <reference path="../Frete/TabelaCliente.js" />
/// <reference path="../Frete/TabelaComissao.js" />
/// <reference path="../Frete/TabelaRota.js" />
/// <reference path="../Frete/TabelaSubContratacao.js" />
/// <reference path="../Frete/TabelaTerceiros.js" />
/// <reference path="../Impressao/Impressao.js" />
/// <reference path="Integracao.js" />
/// <reference path="IntegracaoCargaCancelamento.js" />
/// <reference path="IntegracaoCTe.js" />
/// <reference path="IntegracaoEDI.js" />
/// <reference path="Avon/IntegracaoMinutaAvon.js" />
/// <reference path="Avon/IntegracaoMinutaAvonSignalR.js" />
/// <reference path="../Terceiro/ContratoFrete.js" />
/// <reference path="../DadosCargaCancelamento/SignalR.js" />
/// <reference path="../DadosCargaCancelamento/CargaCancelamento.js" />
/// <reference path="../DadosCargaCancelamento/DataCarregamento.js" />
/// <reference path="../DadosCargaCancelamento/Leilao.js" />
/// <reference path="../DadosCargaCancelamento/Operador.js" />
/// <reference path="../../../Consultas/Tranportador.js" />
/// <reference path="../../../Consultas/Localidade.js" />
/// <reference path="../../../Consultas/ModeloVeicularCargaCancelamento.js" />
/// <reference path="../../../Consultas/TipoCargaCancelamento.js" />
/// <reference path="../../../Consultas/Motorista.js" />
/// <reference path="../../../Consultas/Veiculo.js" />
/// <reference path="../../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../Consultas/TipoOperacao.js" />
/// <reference path="../../../Consultas/Filial.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/Usuario.js" />
/// <reference path="../../../Consultas/TipoCargaCancelamento.js" />
/// <reference path="../../../Consultas/RotaFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacoesCargaCancelamento.js" />
/// <reference path="../../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="../../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="../../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoContratacaoCargaCancelamento.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../../Enumeradores/EnumTipoEmissaoCTeParticipantes.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />

var _tipoIntegracaoCargaCancelamento = new Array();
var _gridIntegracaoCargaCancelamento;
var _gridHistoricoIntegracaoCargaCancelamento;
var _integracaoCargaCancelamento;
var _pesquisaHistoricoIntegracaoCargaCancelamento;

var _situacaoIntegracaoCarga = [{ value: "", text: Localization.Resources.Gerais.Geral.Todas },
    { value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao },
    { value: EnumSituacaoIntegracaoCarga.AgRetorno, text: Localization.Resources.Gerais.Geral.AguardandoRetorno },
    { value: EnumSituacaoIntegracaoCarga.Integrado, text: Localization.Resources.Gerais.Geral.Integrado },
    { value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: Localization.Resources.Gerais.Geral.Falha }];

var PesquisaHistoricoIntegracaoCargaCancelamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var IntegracaoCargaCancelamento = function () {

    this.CargaCancelamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ val: ko.observable(""), options: _tipoIntegracaoCargaCancelamento, text: Localization.Resources.Gerais.Geral.Integracao.getFieldDescription(), def: "", issue: 267, visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoIntegracaoCarga, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Total.getFieldDescription() });
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoRetorno.getFieldDescription()  });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.ProblemasNaIntegracao.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Integrado.getFieldDescription() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridIntegracaoCargaCancelamento.CarregarGrid();
            ObterTotaisIntegracaoCargaCancelamento();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarTodos = PropertyEntity({
        eventClick: function (e) {
            ReenviarTodosIntegracaoCargaCancelamento();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.ReenviarTodos, idGrid: guid(), visible: ko.observable(false)
    });

    this.ObterTotais = PropertyEntity({
        eventClick: function (e) {
            ObterTotaisIntegracaoCargaCancelamento();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.ObterTotais, idGrid: guid(), visible: ko.observable(true)
    });
}

function LoadIntegracaoCargaCancelamento(cargaCancelamento, idKnockoutIntegracaoCargaCancelamento) {

    _integracaoCargaCancelamento = new IntegracaoCargaCancelamento();
    _integracaoCargaCancelamento.CargaCancelamento.val(cargaCancelamento.Codigo.val());

    KoBindings(_integracaoCargaCancelamento, idKnockoutIntegracaoCargaCancelamento);


    //if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaCancelamento_ReenviarIntegracoes, _PermissoesPersonalizadasCargaCancelamento))
    //    _integracaoCargaCancelamento.ReenviarTodos.visible(true);

    ObterTotaisIntegracaoCargaCancelamento();
    ConfigurarPesquisaIntegracaoCargaCancelamento();
}

function ConfigurarPesquisaIntegracaoCargaCancelamento() {

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [] };

    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Gerais.Geral.Reenviar, id: guid(), metodo: ReenviarIntegracaoCargaCancelamento, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoReenviarIntegracaoCargaCancelamento });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Gerais.Geral.HistoricoIntegracao, id: guid(), metodo: ExibirHistoricoIntegracaoCargaCancelamento, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), metodo: OpcaoAuditoria("CargaCancelamentoCargaCancelamentoIntegracao"), tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoAuditoria });

    _gridIntegracaoCargaCancelamento = new GridView(_integracaoCargaCancelamento.Pesquisar.idGrid, "CancelamentoCargaIntegracaoCarga/Pesquisa", _integracaoCargaCancelamento, menuOpcoes);
    _gridIntegracaoCargaCancelamento.CarregarGrid();
}

function VisibilidadeOpcaoReenviarIntegracaoCargaCancelamento(data) {
    //if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaCancelamento_ReenviarIntegracoes, _PermissoesPersonalizadasCargaCancelamento) && data.SituacaoIntegracao != EnumSituacaoIntegracao.ProblemaIntegracao)
    //    return false;

    if (data.Tipo == EnumTipoIntegracao.KMM && data.SituacaoIntegracao != EnumSituacaoIntegracao.ProblemaIntegracao)
        return false;


    if (data.SituacaoIntegracao != EnumSituacaoIntegracao.ProblemaIntegracao)
        return false;

    return true;
}

function ObterTiposIntegracaoCargaCancelamento() {
    var p = new promise.Promise();

    executarReST("TipoIntegracao/BuscarTodos", {
        Tipos: JSON.stringify([EnumTipoIntegracao.OpenTech, EnumTipoIntegracao.AngelLira, EnumTipoIntegracao.NOX, EnumTipoIntegracao.Raster])
    }, function (r) {
        if (r.Success) {

            _tipoIntegracaoCargaCancelamento.push({ value: "", text: Localization.Resources.Gerais.Geral.Todas });

            for (var i = 0; i < r.Data.length; i++)
                _tipoIntegracaoCargaCancelamento.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

        p.done();
    });

    return p;
}

function ReenviarIntegracaoCargaCancelamento(data) {
    executarReST("CancelamentoCargaIntegracaoCarga/Reenviar", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Sucesso, Localization.Resources.Gerais.Geral.ReenvioSolicitadoComSucesso);
            _gridIntegracaoCargaCancelamento.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ReenviarTodosIntegracaoCargaCancelamento() {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todas as integrações?", function () {
        executarReST("CancelamentoCargaIntegracaoCarga/ReenviarTodos", { CargaCancelamento: _integracaoCargaCancelamento.CargaCancelamento.val(), Tipo: _integracaoCargaCancelamento.Tipo.val(), Situacao: _integracaoCargaCancelamento.Situacao.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Sucesso, Localization.Resources.Gerais.Geral.ReenvioSolicitadoComSucesso);
                _gridIntegracaoCargaCancelamento.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Falha, r.Msg);
            }
        });
    });
}

function ObterTotaisIntegracaoCargaCancelamento() {
    executarReST("CancelamentoCargaIntegracaoCarga/ObterTotais", { CargaCancelamento: _integracaoCargaCancelamento.CargaCancelamento.val() }, function (r) {
        if (r.Success) {
            _integracaoCargaCancelamento.TotalGeral.val(r.Data.TotalGeral);
            _integracaoCargaCancelamento.TotalAguardandoIntegracao.val(r.Data.TotalAguardandoIntegracao);
            _integracaoCargaCancelamento.TotalAguardandoRetorno.val(r.Data.TotalAguardandoRetorno);
            _integracaoCargaCancelamento.TotalProblemaIntegracao.val(r.Data.TotalProblemaIntegracao);
            _integracaoCargaCancelamento.TotalIntegrado.val(r.Data.TotalIntegrado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Falha, r.Msg);
        }
    });
}

function ExibirHistoricoIntegracaoCargaCancelamento(integracao) {
    BuscarHistoricoIntegracaoCargaCancelamento(integracao);
    Global.abrirModal("divModalHistoricoCancelamentoIntegracaoCTe");
}

function BuscarHistoricoIntegracaoCargaCancelamento(integracao) {
    _pesquisaHistoricoIntegracaoCargaCancelamento = new PesquisaHistoricoIntegracaoCargaCancelamento();
    _pesquisaHistoricoIntegracaoCargaCancelamento.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoCargaCancelamento, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoCargaCancelamento = new GridView("tblHistoricoIntegracaoCancelamentoCTe", "CancelamentoCargaIntegracaoCarga/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoCargaCancelamento, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoCargaCancelamento.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoCargaCancelamento(historicoConsulta) {
    executarDownload("CancelamentoCargaIntegracaoCarga/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

//function RecarregarIntegracaoCargaCancelamentoViaSignalR(knoutCargaCancelamento) {
//    if (_cargaCancelamentoAtual != null && _cargaCancelamentoAtual.DivCargaCancelamento.id == knoutCargaCancelamento.DivCargaCancelamento.id) {
//        if ($("#divIntegracaoCargaCancelamento_" + _cargaCancelamentoAtual.EtapaIntegracao.idGrid).is(":visible") && _gridIntegracaoCargaCancelamento != null) {
//            _gridIntegracaoCargaCancelamento.CarregarGrid();
//            ObterTotaisIntegracaoCargaCancelamento();
//        }
//    }
//}