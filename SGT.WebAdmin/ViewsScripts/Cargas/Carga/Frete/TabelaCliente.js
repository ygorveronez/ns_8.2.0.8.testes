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
/// <reference path="../../../Creditos/ControleSaldo/ControleSaldo.js" />
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
/// <reference path="../DocumentosEmissao/CargaPedidoDocumentoCTe.js" />
/// <reference path="../DocumentosEmissao/ConsultaReceita.js" />
/// <reference path="../DocumentosEmissao/CTe.js" />
/// <reference path="../DocumentosEmissao/Documentos.js" />
/// <reference path="../DocumentosEmissao/DropZone.js" />
/// <reference path="../DocumentosEmissao/EtapaDocumentos.js" />
/// <reference path="../DocumentosEmissao/NotaFiscal.js" />
/// <reference path="Complemento.js" />
/// <reference path="Componente.js" />
/// <reference path="EtapaFrete.js" />
/// <reference path="Frete.js" />
/// <reference path="SemTabela.js" />
/// <reference path="TabelaComissao.js" />
/// <reference path="TabelaRota.js" />
/// <reference path="TabelaSubContratacao.js" />
/// <reference path="TabelaTerceiros.js" />
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
/// <reference path="../../../Enumeradores/EnumTipoTabelaFrete.js" />
/// <reference path="../../../Consultas/ComponenteFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoFreteCliente.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoFreteComissao.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoComplementoFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoCalculoTabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var FretePorFreteClienteUtilizado = function () {
    this.Tabela = PropertyEntity({ type: types.local, visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.TabelaDeFrete });
    this.ContratoFrete = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.ContratoDeFrete });
    this.Origem = PropertyEntity({ type: types.local, visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.Origem });
    this.Destino = PropertyEntity({ type: types.local, visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.Destino });
    this.TipoCalculo = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.TipoDeCalculoUtilizado });
    this.DataVigenciaTabelaFrete = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.DataVigencia });
    this.ValorFreteAPagar = PropertyEntity({ type: types.local, text: Localization.Resources.Cargas.Carga.ValorDoFrete });
    this.Impostos = PropertyEntity({ type: types.local, text: Localization.Resources.Cargas.Carga.Impostos });
    this.Aliquotas = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.Aliquotas });
    this.CSTs = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.CSTs });
    this.TaxaDocumentacao = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.Taxa});
    this.ValorTotal = PropertyEntity({ type: types.local, visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorTotalDaPrestacao });
    this.ValorFreteLiquido = PropertyEntity({ type: types.local, visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorFreteLiquido });
    this.ValorFreteTabelaFrete = PropertyEntity({ type: types.local, visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorCalculadoPelaTabela });
    this.ValorFreteNegociado = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.ValorFreteNegociado });
    this.ValorContratoFrete = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.ValorContratoDeFrete });
    this.DetalhesFrete = PropertyEntity({ eventClick: detalhesFreteClick, type: types.event, text: Localization.Resources.Cargas.Carga.VerDetalhes, visible: ko.observable(true) });
    this.ValorMercadoria = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.ValorDaMercadoria });
    this.Peso = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.Peso });
    this.Moeda = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.Moeda });
    this.ValorCotacaoMoeda = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.Cotacao });
    this.ValorTotalMoeda = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.ValorEmMoeda });
    this.ValorTotalMoedaPagar = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.ValorTotalEmMoeda });
    this.CEPDestinoDiasUteis = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.PrazoDiasUteis });
    this.PercentualBonificacaoTransportador = PropertyEntity({ type: types.local, visible: ko.observable(false), text: ko.observable(Localization.Resources.Cargas.Carga.PercentualBonificacaoTransportador), textDef: Localization.Resources.Cargas.Carga.PercentualBonificacaoTransportador  });
    this.PercentualEmRelacaoTabela = PropertyEntity({ visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.PercentualEmRelacaoTabela });
    this.PercentualEmRelacaoValorFrete = PropertyEntity({ visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.PercentualEmRelacaoValorFrete });
    this.CustoFrete = PropertyEntity({ visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.CustoFrete });
    this.ValorFreteOperador = PropertyEntity({ type: types.local, visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorFreteOperador });
    this.DiferencaRelacaoValorFrete = PropertyEntity({ type: types.local, visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.DiferencaRelacaoValorFrete });
};

//*******EVENTOS*******

var _HTMLDetalheFretePorFreteCliente;

function loadCargaFreteCliente() {
    $.get("Content/Static/Carga/DetalhesFretePorCliente.html?dyn=" + guid(), function (data) {
        _HTMLDetalheFretePorFreteCliente = data;
    });
}

//*******MÉTODOS*******

function preencherRetornoFreteCliente(e, retorno, freteFilialEmissora) {
    var freteUtilizado = retorno.dadosRetornoTipoFrete;
    var detalhePorFreteCliente = new FretePorFreteClienteUtilizado();
    var valorTotalPrestacao = retorno.valorFreteAPagarComICMSeISS;

    detalhePorFreteCliente.Tabela.val(freteUtilizado.Tabela);
    detalhePorFreteCliente.ContratoFrete.val(freteUtilizado.ContratoFrete);

    if (freteUtilizado.Tabela == "" || freteUtilizado.Tabela == null)
        detalhePorFreteCliente.Tabela.visible(false);
    else
        detalhePorFreteCliente.Tabela.visible(true);

    if (freteUtilizado.ContratoFrete != "")
        detalhePorFreteCliente.ContratoFrete.visible(true);
    else
        detalhePorFreteCliente.ContratoFrete.visible(false);

    if (freteFilialEmissora == null)
        freteFilialEmissora = false;

    detalhePorFreteCliente.ValorFreteLiquido.visible(!freteFilialEmissora);
    detalhePorFreteCliente.Impostos.val(Globalize.format(retorno.valorICMS + retorno.valorISS, "n2"));
    detalhePorFreteCliente.ValorFreteLiquido.val(Globalize.format(retorno.ValorFreteLiquido, "n2"));
    detalhePorFreteCliente.ValorTotal.val(Globalize.format(valorTotalPrestacao, "n2"));
    detalhePorFreteCliente.ValorFreteAPagar.val(Globalize.format(retorno.valorFreteAPagar, "n2"));
    detalhePorFreteCliente.ValorFreteTabelaFrete.val(Globalize.format(retorno.valorFreteTabelaFrete, "n2"));
    detalhePorFreteCliente.ValorFreteOperador.val(Globalize.format(retorno.ValorFreteOperador, "n2"));
    detalhePorFreteCliente.DiferencaRelacaoValorFrete.val(Globalize.format(retorno.DiferencaRelacaoValorFrete, "n2"));

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        detalhePorFreteCliente.ValorFreteNegociado.val(Globalize.format(retorno.ValorFreteNegociado, "n2"));
        detalhePorFreteCliente.ValorFreteNegociado.visible(true);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe && e.TipoOperacao.naoExibirDetalhesDoFretePortalTransportador) {
        detalhePorFreteCliente.ValorFreteTabelaFrete.visible(false);
        detalhePorFreteCliente.ValorTotal.visible(false);
    }

    if (retorno.Moeda !== null && retorno.Moeda !== EnumMoedaCotacaoBancoCentral.Real) {
        detalhePorFreteCliente.Moeda.val(EnumMoedaCotacaoBancoCentral.obterDescricao(retorno.Moeda));
        detalhePorFreteCliente.ValorCotacaoMoeda.val(Globalize.format(retorno.ValorCotacaoMoeda, "n10"));
        detalhePorFreteCliente.ValorTotalMoeda.val(Globalize.format(retorno.ValorTotalMoeda, "n2")); 
        detalhePorFreteCliente.ValorTotalMoedaPagar.val(Globalize.format(retorno.ValorTotalMoedaPagar, "n2"));

        e.Moeda.val(retorno.Moeda);
        e.ValorCotacaoMoeda.val(Globalize.format(retorno.ValorCotacaoMoeda, "n10"));
        e.ValorTotalMoeda.val(Globalize.format(retorno.ValorTotalMoeda, "n2"));
        e.ValorTotalMoedaPagar.val(Globalize.format(retorno.ValorTotalMoedaPagar, "n2"));

        detalhePorFreteCliente.Moeda.visible(true);
        detalhePorFreteCliente.ValorCotacaoMoeda.visible(true);
        detalhePorFreteCliente.ValorTotalMoeda.visible(true);
        detalhePorFreteCliente.ValorTotalMoedaPagar.visible(true);
    }

    if (retorno.valorFreteContratoFrete > 0) {
        detalhePorFreteCliente.ValorContratoFrete.val(Globalize.format(retorno.valorFreteContratoFrete, "n2"));
        detalhePorFreteCliente.ValorContratoFrete.visible(true);
    }
    else {
        detalhePorFreteCliente.ValorContratoFrete.visible(false);
    }

    e.ValorFreteTabelaFrete.val(retorno.valorFreteTabelaFrete);
    e.TipoCalculoTabelaFrete.val(freteUtilizado.TipoCalculo);

    if (freteUtilizado.TipoCalculo !== EnumTipoCalculoTabelaFrete.PorCarga) {
        detalhePorFreteCliente.Origem.visible(false);
        detalhePorFreteCliente.Destino.visible(false);
        detalhePorFreteCliente.TipoCalculo.visible(true);
    }

    detalhePorFreteCliente.TipoCalculo.val(EnumTipoCalculoTabelaFrete.obterDescricao(freteUtilizado.TipoCalculo));
    detalhePorFreteCliente.DataVigenciaTabelaFrete.val(retorno.DataVigenciaTabelaFrete);

    detalhePorFreteCliente.Origem.val(freteUtilizado.Origem);
    detalhePorFreteCliente.Destino.val(freteUtilizado.Destino);

    if (freteUtilizado.Origem == "" || freteUtilizado.Origem == null)
        detalhePorFreteCliente.Origem.visible(false);
    else
        detalhePorFreteCliente.Origem.visible(true);

    if (freteUtilizado.Destino == "" || freteUtilizado.Destino == null)
        detalhePorFreteCliente.Destino.visible(false);
    else
        detalhePorFreteCliente.Destino.visible(true);

    detalhePorFreteCliente.CEPDestinoDiasUteis.val(retorno.CEPDestinoDiasUteis);
    detalhePorFreteCliente.CEPDestinoDiasUteis.visible(retorno.CEPDestinoDiasUteis > 0);
    detalhePorFreteCliente.PercentualBonificacaoTransportador.text((retorno.DescricaoBonificacaoTransportador || detalhePorFreteCliente.PercentualBonificacaoTransportador.textDef) + ":");
    detalhePorFreteCliente.PercentualBonificacaoTransportador.val(Globalize.format(retorno.PercentualBonificacaoTransportador, "n2"));
    detalhePorFreteCliente.PercentualBonificacaoTransportador.visible(retorno.PercentualBonificacaoTransportador != 0);
    detalhePorFreteCliente.CustoFrete.val(retorno.CustoFrete);

    preecherDetalhesTiposFrete(e, _HTMLDetalheFretePorFreteCliente, detalhePorFreteCliente, retorno);
    PreecherInformacaoValorFrete(e, retorno.valorFreteAPagar);
    preencherDetalhesFrete(retorno, false);

    if (freteUtilizado.TipoCalculo !== EnumTipoCalculoTabelaFrete.PorDocumentoEmitido) {

        if (freteUtilizado.PermiteAlterarValorFrete) {
            if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AlterarValorFreteApenasComTabelaFrete, _PermissoesPersonalizadasCarga)) {

                e.AtualizarValorFrete.enable(true);
                e.ValorFreteOperador.enable(true);
            }
            else {
                if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AlterarValorFrete, _PermissoesPersonalizadasCarga)) {
                    e.AtualizarValorFrete.enable(true);
                    e.ValorFreteOperador.enable(true);
                }
                else {
                    e.AtualizarValorFrete.enable(false);
                    e.ValorFreteOperador.enable(false);
                }
            }
        }
        else {
            e.ValorFreteOperador.visible(freteUtilizado.PermiteAlterarValorFrete);
            e.AtualizarValorFrete.visible(freteUtilizado.PermiteAlterarValorFrete);
        }
        
    }
    else {
        e.ValorFreteOperador.visible(false);
        e.AtualizarValorFrete.visible(false);
    }

    if (_CONFIGURACAO_TMS.ExibirAliquotaEtapaFreteCarga && ((retorno.valorICMS + retorno.valorISS) > 0)) {
        var aliquotas = "";
        if (retorno.aliquotaICMS > 0)
            aliquotas = "ICMS: " + Globalize.format(retorno.aliquotaICMS, "n2") + "% ";
        if (retorno.aliquotaISS > 0)
            aliquotas += "ISS: " + Globalize.format(retorno.aliquotaISS, "n2") + "%";
        detalhePorFreteCliente.Aliquotas.val(aliquotas);
        if (aliquotas != "")
            detalhePorFreteCliente.Aliquotas.visible(true);
    }

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal && ((retorno.valorICMS + retorno.valorISS) > 0)) {
        var aliquotas = "";
        if (retorno.aliquotaICMS > 0)
            aliquotas = "ICMS: " + Globalize.format(retorno.aliquotaICMS, "n2") + "% ";
        if (retorno.aliquotaISS > 0)
            aliquotas += "ISS: " + Globalize.format(retorno.aliquotaISS, "n2") + "%";
        detalhePorFreteCliente.Aliquotas.val(aliquotas);
        if (aliquotas != "")
            detalhePorFreteCliente.Aliquotas.visible(true);

        if (retorno.csts != undefined && retorno.csts != null && retorno.csts != "") {
            detalhePorFreteCliente.CSTs.val(retorno.csts);
            detalhePorFreteCliente.CSTs.visible(true);
        }
        if (retorno.taxaDocumentacao != undefined && retorno.taxaDocumentacao != null && retorno.taxaDocumentacao != "") {
            detalhePorFreteCliente.TaxaDocumentacao.val(retorno.taxaDocumentacao);
            detalhePorFreteCliente.TaxaDocumentacao.visible(true);
        }
        if (retorno.valorMercadoria > 0) {
            detalhePorFreteCliente.ValorMercadoria.val(Globalize.format(retorno.valorMercadoria, "n2"));
            detalhePorFreteCliente.ValorMercadoria.visible(true);
        }
        if (retorno.peso > 0) {
            detalhePorFreteCliente.Peso.val(Globalize.format(retorno.peso, "n2"));
            detalhePorFreteCliente.Peso.visible(true);
        }

    }
    else if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        if (retorno.csts != undefined && retorno.csts != null && retorno.csts != "") {
            detalhePorFreteCliente.CSTs.val(retorno.csts);
            detalhePorFreteCliente.CSTs.visible(true);
        }
        if (retorno.taxaDocumentacao != undefined && retorno.taxaDocumentacao != null && retorno.taxaDocumentacao != "") {
            detalhePorFreteCliente.TaxaDocumentacao.val(retorno.taxaDocumentacao);
            detalhePorFreteCliente.TaxaDocumentacao.visible(true);
        }
        if (retorno.valorMercadoria > 0) {
            detalhePorFreteCliente.ValorMercadoria.val(Globalize.format(retorno.valorMercadoria, "n2"));
            detalhePorFreteCliente.ValorMercadoria.visible(true);
        }
        if (retorno.peso > 0) {
            detalhePorFreteCliente.Peso.val(Globalize.format(retorno.peso, "n2"));
            detalhePorFreteCliente.Peso.visible(true);
        }
    }
}
