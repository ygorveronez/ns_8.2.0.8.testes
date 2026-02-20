/// <autosync enabled="true" />
/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
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
/// <reference path="Configuracao.js" />
/// <reference path="DadosEmissao.js" />
/// <reference path="Geral.js" />
/// <reference path="Lacre.js" />
/// <reference path="LocaisPrestacao.js" />
/// <reference path="Observacao.js" />
/// <reference path="Passagem.js" />
/// <reference path="Percurso.js" />
/// <reference path="Rota.js" />
/// <reference path="../DadosTransporte/DadosTransporte.js" />
/// <reference path="../DadosTransporte/Motorista.js" />
/// <reference path="../DadosTransporte/Tipo.js" />
/// <reference path="../DadosTransporte/Transportador.js" />
/// <reference path="../Documentos/CTe.js" />
/// <reference path="../Documentos/MDFe.js" />
/// <reference path="../Documentos/NFS.js" />
/// <reference path="../Documentos/PreCTe.js" />
/// <reference path="../DocumentosEmissao/CargaPedidoDocumentoCTe.js" />
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
/// <reference path="../Integracao/Integracao.js" />
/// <reference path="../Integracao/IntegracaoCarga.js" />
/// <reference path="../Integracao/IntegracaoCTe.js" />
/// <reference path="../Integracao/IntegracaoEDI.js" />
/// <reference path="../Terceiro/ContratoFrete.js" />
/// <reference path="../DadosCarga/SignalR.js" />
/// <reference path="../DadosCarga/Carga.js" />
/// <reference path="../DadosCarga/DataCarregamento.js" />
/// <reference path="../DadosCarga/Leilao.js" />
/// <reference path="../DadosCarga/Operador.js" />
/// <reference path="../../../Consultas/Tranportador.js" />
/// <reference path="../../../Consultas/Localidade.js" />
/// <reference path="../../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/Motorista.js" />
/// <reference path="../../../Consultas/Veiculo.js" />
/// <reference path="../../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../Consultas/TipoOperacao.js" />
/// <reference path="../../../Consultas/Filial.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/Usuario.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/RotaFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="../../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="../../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoContratacaoCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../../Enumeradores/EnumTipoEmissaoCTeParticipantes.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />

//*******MAPEAMENTO*******

var _configuracaoEmissaoCTe, _cargaDadosEmissaoSeguro;

var CargaDadosEmissaoSeguro = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idTab: guid(), enable: ko.observable(true) });

    this.ConfiguracaoEmissaoCTe = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "" });

    this.MensagemAutorizarSeguro = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.AutorizarSeguro = PropertyEntity({ eventClick: autorizarSeguroClick, type: types.event, text: Localization.Resources.Cargas.Carga.AutorizarSeguro, visible: ko.observable(false), enable: ko.observable(true) });

    this.Atualizar = PropertyEntity({ eventClick: atualizarCargaDadosEmissaoSeguro, type: types.event, text: Localization.Resources.Cargas.Carga.AtualizarSeguro, visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadCargaDadosEmissaoSeguro(callback) {
    _cargaDadosEmissaoSeguro = new CargaDadosEmissaoSeguro();
    KoBindings(_cargaDadosEmissaoSeguro, "tabSeguro_" + _cargaAtual.DadosEmissaoFrete.id);
    $("#tabSeguro_" + _cargaAtual.DadosEmissaoFrete.id + "_li").show();

    _cargaDadosEmissaoSeguro.Pedido.enable(_cargaAtual.EtapaFreteEmbarcador.enable());
    _cargaDadosEmissaoSeguro.AutorizarSeguro.enable(_cargaAtual.EtapaFreteEmbarcador.enable());
    _cargaDadosEmissaoSeguro.Atualizar.enable(_cargaAtual.EtapaFreteEmbarcador.enable());

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AlterarDadosSeguro, _PermissoesPersonalizadasCarga))
        _cargaDadosEmissaoSeguro.Atualizar.enable(false);

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AutorizarSeguro, _PermissoesPersonalizadasCarga))
        _cargaDadosEmissaoSeguro.AutorizarSeguro.enable(false);

    _configuracaoEmissaoCTe = new ConfiguracaoEmissaoCTe(_cargaDadosEmissaoSeguro.ConfiguracaoEmissaoCTe.id, _cargaDadosEmissaoSeguro.ConfiguracaoEmissaoCTe, null, null, _cargaAtual.EtapaFreteEmbarcador.enable(), function () {
        _configuracaoEmissaoCTe.Configuracao.ArquivoImportacaoNotasFiscais.visible(false);
        _configuracaoEmissaoCTe.Configuracao.FormulaRateioFrete.visible(false);
        _configuracaoEmissaoCTe.Configuracao.TipoRateioDocumentos.visible(false);
        _configuracaoEmissaoCTe.Configuracao.TipoIntegracao.visible(false);
        _configuracaoEmissaoCTe.Configuracao.ModeloDocumentoFiscal.visible(false);
        _configuracaoEmissaoCTe.Configuracao.ComponentesFrete.visible(false);
        _configuracaoEmissaoCTe.Configuracao.TipoEmissaoCTeParticipantes.visible(false);
        _configuracaoEmissaoCTe.Configuracao.EmpresaEmissora.visible(false);
        _configuracaoEmissaoCTe.Configuracao.CTeEmitidoNoEmbarcador.visible(false);
        _configuracaoEmissaoCTe.Configuracao.NaoValidarNotaFiscalExistente.visible(false);
        _configuracaoEmissaoCTe.Configuracao.AgruparMovimentoFinanceiroPorPedido.visible(false);
        _configuracaoEmissaoCTe.Configuracao.ExigirNumeroPedido.visible(false);
        _configuracaoEmissaoCTe.Configuracao.ValePedagioObrigatorio.visible(false);
        _configuracaoEmissaoCTe.Configuracao.TipoEmissaoIntramunicipal.visible(false);
        _configuracaoEmissaoCTe.Configuracao.ModeloDocumentoFiscalEmissaoMunicipal.visible(false);
        _configuracaoEmissaoCTe.Configuracao.NaoEmitirMDFe.visible(false);
        _configuracaoEmissaoCTe.Configuracao.BloquearDiferencaValorFreteEmbarcador.visible(false);
        _configuracaoEmissaoCTe.Configuracao.EmitirComplementoDiferencaFreteEmbarcador.visible(false);
        _configuracaoEmissaoCTe.Configuracao.GerarMDFeTransbordoSemConsiderarOrigem.visible(false);
        _configuracaoEmissaoCTe.Configuracao.ImportarRedespachoIntermediario.visible(false);
        _configuracaoEmissaoCTe.Configuracao.DescricaoComponenteFreteEmbarcador.visible(false);
        _configuracaoEmissaoCTe.Configuracao.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao.visible(false);
        _configuracaoEmissaoCTe.Configuracao.DescricaoItemPesoCTeSubcontratacao.visible(false);
        _configuracaoEmissaoCTe.Configuracao.CaracteristicaTransporteCTe.visible(false);
        _configuracaoEmissaoCTe.Configuracao.ObservacaoEmissaoCarga.visible(false);
        _configuracaoEmissaoCTe.Configuracao.ValorMaximoEmissaoPendentePagamento.visible(false);
        _configuracaoEmissaoCTe.Configuracao.TipoEnvioEmail.visible(false);
        _configuracaoEmissaoCTe.Configuracao.ClientesBloquearEmissaoDosDestinatario.visible(false);
        _configuracaoEmissaoCTe.Configuracao.TipoPropostaMultimodal.visible(false);
        _configuracaoEmissaoCTe.Configuracao.TipoServicoMultimodal.visible(false);
        _configuracaoEmissaoCTe.Configuracao.ModalPropostaMultimodal.visible(false);
        _configuracaoEmissaoCTe.Configuracao.TipoCobrancaMultimodal.visible(false);
        _configuracaoEmissaoCTe.Configuracao.BloquearEmissaoDeEntidadeSemCadastro.visible(false);
        _configuracaoEmissaoCTe.Configuracao.BloquearEmissaoDosDestinatario.visible(false);
        _configuracaoEmissaoCTe.Configuracao.TipoOcorrenciaCTeEmitidoEmbarcador.visible(false);
        _configuracaoEmissaoCTe.Configuracao.DisponibilizarDocumentosParaPagamento.visible(false);
        _configuracaoEmissaoCTe.Configuracao.ProvisionarDocumentos.visible(false);
        _configuracaoEmissaoCTe.Configuracao.DisponibilizarDocumentosParaLoteEscrituracao.visible(false);
        _configuracaoEmissaoCTe.Configuracao.EscriturarSomenteDocumentosEmitidosParaNFe.visible(false);
        _configuracaoEmissaoCTe.Configuracao.GerarCIOTParaTodasAsCargas.visible(false);
        _configuracaoEmissaoCTe.Configuracao.TipoOcorrenciaComplementoSubcontratacao.visible(false);
        _configuracaoEmissaoCTe.Configuracao.TempoCarregamento.visible(false);
        _configuracaoEmissaoCTe.Configuracao.TempoDescarregamento.visible(false);
        _configuracaoEmissaoCTe.Configuracao.ValorLimiteFaturamento.visible(false);
        _configuracaoEmissaoCTe.Configuracao.DiasEmAbertoAposVencimento.visible(false);
        _configuracaoEmissaoCTe.Configuracao.ApoliceSeguro.cssClass("col col-xs-12 col-sm-12 col-md-12 col-lg-12");
        _configuracaoEmissaoCTe.Configuracao.ApoliceSeguro.visible(true);
        _configuracaoEmissaoCTe.Configuracao.AverbarCTeImportadoDoEmbarcador.visible(false);

        $("#divObservacoesEmissaoCarga").hide();

        var tabSeguro = $("#tabSeguro_" + _cargaAtual.DadosEmissaoFrete.id);
        tabSeguro.find("fieldset").css("padding", "15px 10px");
        tabSeguro.find("header").remove();
        tabSeguro.find(".well").removeClass("well");
        tabSeguro.find(".insideNoPaddignTable").css("margin-bottom", "0");
        tabSeguro.find(".insideNoPaddignTable").parent().closest("section").css("margin-bottom", "0");

        if (callback != null)
            callback();
    }, _cargaAtual.Empresa);
}

function preencherCargaDadosEmissaoSeguro(dados) {

    if (_cargaDadosEmissaoSeguro == null)
        return;

    PreencherObjetoKnout(_cargaDadosEmissaoSeguro, dados);

    var dadosConfiguracaoEmissao = {
        ApolicesSeguro: dados.Data.ApolicesSeguro,
        FormulaRateioFrete: { Descricao: "", Codigo: 0 },
        TipoRateioDocumentos: 0,
        ModeloDocumentoFiscal: { Descricao: "", Codigo: 0 },
        EmpresaEmissora: { Descricao: "", Codigo: 0 },
        ComponentesFrete: new Array(),
        ArquivoImportacaoNotasFiscais: { Descricao: "", Codigo: 0 },
        ModeloDocumentoFiscalEmissaoMunicipal: { Descricao: "", Codigo: 0 },
        PercentualBloquearDiferencaValorFreteEmbarcador: "0,00",
        TipoOcorrenciaComplementoDiferencaFreteEmbarcador: { Descricao: "", Codigo: 0 },
        TipoOcorrenciaSemTabelaFrete: { Descricao: "", Codigo: 0 },
        EmitenteImportacaoRedespachoIntermediario: { Descricao: "", Codigo: 0 },
        ExpedidorImportacaoRedespachoIntermediario: { Descricao: "", Codigo: 0 },
        RecebedorImportacaoRedespachoIntermediario: { Descricao: "", Codigo: 0 },
        TipoOcorrenciaCTeEmitidoEmbarcador: { Descricao: "", Codigo: 0 },
        ClientesBloqueados: new Array(),
        TipoOcorrenciaComplementoSubcontratacao: { Descricao: "", Codigo: 0 },

    };
    _cargaDadosEmissaoSeguro.AutorizarSeguro.visible(dados.Data.ApolicesAgurandoAutorizacao);
    if (dados.Data.MensagemAutorizarSeguro != "") {
        _cargaDadosEmissaoSeguro.MensagemAutorizarSeguro.visible(true);
        var html = dados.Data.MensagemAutorizarSeguro;
        $("#" + _cargaDadosEmissaoSeguro.MensagemAutorizarSeguro.id).html(html);
    } else {
        _cargaDadosEmissaoSeguro.MensagemAutorizarSeguro.visible(false);
    }

    if (_configuracaoEmissaoCTe != null)
        _configuracaoEmissaoCTe.SetarValores(dadosConfiguracaoEmissao);
}

function atualizarCargaDadosEmissaoSeguro(e, sender) {
    e.Carga.val(_cargaAtual.Codigo.val());
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteAtualizarSeguroDaCarga, function () {
        Salvar(e, "DadosEmissaoSeguro/AtualizarSeguro", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.SeguroAtualizadoComSucesso);
                    obterDadosEmissaoGeralCarga();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function autorizarSeguroClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteAutorizarSeguroDaCarga, function () {
        Salvar(e, "DadosEmissaoSeguro/AutorizarSeguro", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.SeguroAutorizadoComSucesso);
                    _cargaDadosEmissaoSeguro.AutorizarSeguro.visible(false);
                    _cargaDadosEmissaoSeguro.MensagemAutorizarSeguro.visible(false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}