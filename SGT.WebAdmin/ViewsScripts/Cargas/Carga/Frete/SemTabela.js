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
/// <reference path="TabelaCliente.js" />
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

//*******MAPEAMENTO KNOUCKOUT*******


var _HTMLDetalheFreteSemTabelaFrete;


var FreteSemTabelaFrete = function () {
    this.ValorFreteAPagar = PropertyEntity({ type: types.local, visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorDoFrete });
    this.Impostos = PropertyEntity({ type: types.local, visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.Impostos });
    this.Aliquotas = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.Aliquotas });
    this.CSTs = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.CSTs });
    this.TaxaDocumentacao = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.Taxa });
    this.ValorTotal = PropertyEntity({ type: types.local, visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorTotal });
    this.ValorFreteLiquido = PropertyEntity({ type: types.local, visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorFreteLiquido });
    this.Descricao = PropertyEntity({ type: types.local, visible: ko.observable(true) });
    this.ValorFreteTabela = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.ValorDoFretePelaTabela });
    this.DetalhesFrete = PropertyEntity({ eventClick: detalhesFreteClick, type: types.event, text: Localization.Resources.Cargas.Carga.VerDetalhes, visible: ko.observable(true) });
    this.ValorMercadoria = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.ValorDaMercadoria });
    this.Peso = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.Peso });
    this.ValorFreteNegociado = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.ValorFreteNegociado });
    this.Moeda = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.Moeda });
    this.ValorCotacaoMoeda = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.Cotacao });
    this.ValorTotalMoeda = PropertyEntity({ type: types.local, visible: ko.observable(false), text: Localization.Resources.Cargas.Carga.ValorEmMoeda });

    this.PercentualEmRelacaoTabela = PropertyEntity({ visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.PercentualEmRelacaoTabela });
    this.PercentualEmRelacaoValorFrete = PropertyEntity({ visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.PercentualEmRelacaoValorFrete });
    this.CustoFrete = PropertyEntity({ type: types.local, val: ko.observable(""), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.CustoFrete });
};

//*******EVENTOS*******


function loadCargaFreteNaoEncontrado() {
    $.get("Content/Static/Carga/DetalhesFreteSemTabela.html?dyn=" + guid(), function (data) {
        _HTMLDetalheFreteSemTabelaFrete = data;
    });
}


//*******MÉTODOS*******

function preencherRetornoFreteSemTabela(e, retorno) {
    e.TipoFreteEscolhido.val(EnumTipoFreteEscolhido.Operador);
    PreecherInformacaoValorFrete(e, retorno.valorFreteAPagar);
    var valorTotalPrestacao = retorno.NaoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador ? retorno.valorFrete : (retorno.valorFrete + retorno.valorICMS + retorno.valorISS - retorno.ValorRetencaoISS + BuscarValorTotalComponentes(retorno));
    var freteSemTabelaFrete = new FreteSemTabelaFrete();

    if (_cargaAtual.CargaTransbordo.val() === true && _CONFIGURACAO_TMS.TipoContratoFreteTerceiro == EnumTipoContratoFreteTerceiro.PorPagamentoAgregado) {
        freteSemTabelaFrete.Descricao.val(Localization.Resources.Cargas.Carga.CargaDeTransbordoNaoNecessarioValorDoFrete);
    }
    else if (_cargaAtual.CargaSVM.val() === true) {
        freteSemTabelaFrete.Descricao.val(Localization.Resources.Cargas.Carga.CargaDeSVMNaoSeraRealizadoCalculoNesteMomentoFavorAvanceProximaEtapa);

        freteSemTabelaFrete.ValorFreteAPagar.visible(false);
        freteSemTabelaFrete.Impostos.visible(false);
        freteSemTabelaFrete.Aliquotas.visible(false);
        freteSemTabelaFrete.CSTs.visible(false);
        freteSemTabelaFrete.TaxaDocumentacao.visible(false);
        freteSemTabelaFrete.ValorTotal.visible(false);
        freteSemTabelaFrete.ValorFreteLiquido.visible(false);
        freteSemTabelaFrete.ValorFreteTabela.visible(false);
        freteSemTabelaFrete.DetalhesFrete.visible(false);
        freteSemTabelaFrete.ValorMercadoria.visible(false);
        freteSemTabelaFrete.Peso.visible(false);
        freteSemTabelaFrete.PercentualEmRelacaoValorFrete.visible(false);
        freteSemTabelaFrete.PercentualEmRelacaoTabela.visible(false);
    }
    else if (_cargaAtual.CargaDestinadaCTeComplementar.val()) {
        freteSemTabelaFrete.Descricao.val(Localization.Resources.Cargas.Carga.CargaComplementoValores);

        freteSemTabelaFrete.ValorFreteAPagar.visible(false);
        freteSemTabelaFrete.Impostos.visible(false);
        freteSemTabelaFrete.Aliquotas.visible(false);
        freteSemTabelaFrete.CSTs.visible(false);
        freteSemTabelaFrete.TaxaDocumentacao.visible(false);
        freteSemTabelaFrete.ValorTotal.visible(false);
        freteSemTabelaFrete.ValorFreteLiquido.visible(true);
        freteSemTabelaFrete.ValorFreteTabela.visible(false);
        freteSemTabelaFrete.DetalhesFrete.visible(false);
        freteSemTabelaFrete.ValorMercadoria.visible(false);
        freteSemTabelaFrete.Peso.visible(false);
        freteSemTabelaFrete.PercentualEmRelacaoValorFrete.visible(false);
        freteSemTabelaFrete.PercentualEmRelacaoTabela.visible(false);
    }
    else if (_cargaAtual.SaldoInsuficienteContratoFreteCliente.val() === true) {
        freteSemTabelaFrete.Descricao.val(Localization.Resources.Cargas.Carga.NecesarioInformarFreteManualmentePendenciaContratoFreteCliente);
    }
    else {
        freteSemTabelaFrete.Descricao.val(Localization.Resources.Cargas.Carga.FreteInformadoManualmentePoisNaoExistiaUmaTabelaDeFreteConfiguradaParaEstaCarga);
    }

    freteSemTabelaFrete.ValorFreteAPagar.val(Globalize.format(retorno.valorFreteAPagar, "n2"));
    freteSemTabelaFrete.ValorFreteLiquido.val(Globalize.format(retorno.ValorFreteLiquido, "n2"));
    freteSemTabelaFrete.Impostos.val(Globalize.format(retorno.valorICMS + retorno.valorISS, "n2"));
    freteSemTabelaFrete.CustoFrete.val(retorno.CustoFrete);
    freteSemTabelaFrete.ValorTotal.val(Globalize.format(valorTotalPrestacao, "n2"));
    e.ValorFreteTabelaFrete.val(0);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        freteSemTabelaFrete.ValorFreteNegociado.val(Globalize.format(retorno.ValorFreteNegociado, "n2"));
        freteSemTabelaFrete.ValorFreteNegociado.visible(true);
    }

    if (retorno.Moeda !== null && retorno.Moeda !== EnumMoedaCotacaoBancoCentral.Real) {
        freteSemTabelaFrete.Moeda.val(EnumMoedaCotacaoBancoCentral.obterDescricao(retorno.Moeda));
        freteSemTabelaFrete.ValorCotacaoMoeda.val(Globalize.format(retorno.ValorCotacaoMoeda, "n10"));
        freteSemTabelaFrete.ValorTotalMoeda.val(Globalize.format(retorno.ValorTotalMoeda, "n2"));

        freteSemTabelaFrete.Moeda.visible(true);
        freteSemTabelaFrete.ValorCotacaoMoeda.visible(true);
        freteSemTabelaFrete.ValorTotalMoeda.visible(true);
    }

    if (_CONFIGURACAO_TMS.ExibirAliquotaEtapaFreteCarga && ((retorno.valorICMS + retorno.valorISS) > 0)) {
        var aliquotas = "";
        if (retorno.aliquotaICMS > 0)
            aliquotas = "ICMS: " + Globalize.format(retorno.aliquotaICMS, "n2") + "% ";
        if (retorno.aliquotaISS > 0)
            aliquotas += "ISS: " + Globalize.format(retorno.aliquotaISS, "n2") + "%";
        freteSemTabelaFrete.Aliquotas.val(aliquotas);
        if (aliquotas != "")
            freteSemTabelaFrete.Aliquotas.visible(true);
    }

    if (_cargaAtual.CargaSVM.val() !== true) {
        if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal && ((retorno.valorICMS + retorno.valorISS) > 0)) {
            var aliquotas = "";
            if (retorno.aliquotaICMS > 0)
                aliquotas = "ICMS: " + Globalize.format(retorno.aliquotaICMS, "n2") + "% ";
            if (retorno.aliquotaISS > 0)
                aliquotas += "ISS: " + Globalize.format(retorno.aliquotaISS, "n2") + "%";
            freteSemTabelaFrete.Aliquotas.val(aliquotas);
            if (aliquotas != "")
                freteSemTabelaFrete.Aliquotas.visible(true);

            if (retorno.csts != undefined && retorno.csts != null && retorno.csts != "") {
                freteSemTabelaFrete.CSTs.val(retorno.csts);
                freteSemTabelaFrete.CSTs.visible(true);
            }

            if (retorno.taxaDocumentacao != undefined && retorno.taxaDocumentacao != null && retorno.taxaDocumentacao != "") {
                freteSemTabelaFrete.TaxaDocumentacao.val(retorno.taxaDocumentacao);
                freteSemTabelaFrete.TaxaDocumentacao.visible(true);
            }

            if (retorno.valorMercadoria > 0) {
                freteSemTabelaFrete.ValorMercadoria.val(Globalize.format(retorno.valorMercadoria, "n2"));
                freteSemTabelaFrete.ValorMercadoria.visible(true);
            }

            if (retorno.peso > 0) {
                freteSemTabelaFrete.Peso.val(Globalize.format(retorno.peso, "n2"));
                freteSemTabelaFrete.Peso.visible(true);
            }

        } else if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {

            if (retorno.csts != undefined && retorno.csts != null && retorno.csts != "") {
                freteSemTabelaFrete.CSTs.val(retorno.csts);
                freteSemTabelaFrete.CSTs.visible(true);
            }

            if (retorno.taxaDocumentacao != undefined && retorno.taxaDocumentacao != null && retorno.taxaDocumentacao != "") {
                freteSemTabelaFrete.TaxaDocumentacao.val(retorno.taxaDocumentacao);
                freteSemTabelaFrete.TaxaDocumentacao.visible(true);
            }

            if (retorno.valorMercadoria > 0) {
                freteSemTabelaFrete.ValorMercadoria.val(Globalize.format(retorno.valorMercadoria, "n2"));
                freteSemTabelaFrete.ValorMercadoria.visible(true);
            }

            if (retorno.peso > 0) {
                freteSemTabelaFrete.Peso.val(Globalize.format(retorno.peso, "n2"));
                freteSemTabelaFrete.Peso.visible(true);
            }
        }
    }

    preecherDetalhesTiposFrete(e, _HTMLDetalheFreteSemTabelaFrete, freteSemTabelaFrete, retorno);

    preencherDetalhesFrete(retorno, false);

}


function preencherRetornoFretePorContaCliente(e, retorno) {

    e.TipoFreteEscolhido.val(EnumTipoFreteEscolhido.Cliente);

    PreecherInformacaoValorFrete(e, retorno.valorFreteAPagar);
    var valorTotalPrestacao = retorno.valorFrete + retorno.valorICMS + retorno.valorISS + BuscarValorTotalComponentes(retorno);

    var freteSemTabelaFrete = new FreteSemTabelaFrete();
    freteSemTabelaFrete.DetalhesFrete.visible(false);
    freteSemTabelaFrete.Descricao.val(Localization.Resources.Cargas.Carga.FretePorContaDoCliente);
    freteSemTabelaFrete.ValorFreteAPagar.val(Localization.Resources.Cargas.Carga.PorContaDoCliente);
    freteSemTabelaFrete.Impostos.val(Localization.Resources.Cargas.Carga.PorContaDoCliente);
    freteSemTabelaFrete.ValorFreteLiquido.val(Localization.Resources.Cargas.Carga.PorContaDoCliente);
    freteSemTabelaFrete.ValorTotal.val(Localization.Resources.Cargas.Carga.PorContaDoCliente);
    freteSemTabelaFrete.ValorFreteTabela.val(Localization.Resources.Cargas.Carga.PorContaDoCliente);
    freteSemTabelaFrete.CustoFrete.val(retorno.CustoFrete);
    freteSemTabelaFrete.ValorFreteTabela.visible(false);
    preecherDetalhesTiposFrete(e, _HTMLDetalheFreteSemTabelaFrete, freteSemTabelaFrete, retorno);

    preencherDetalhesFrete(retorno, false);
}


function preencherRetornoFreteEmbarcador(e, retorno) {
    e.TipoFreteEscolhido.val(EnumTipoFreteEscolhido.Embarcador);
    PreecherInformacaoValorFrete(e, retorno.valorFreteAPagar);
    var valorTotalPrestacao = retorno.valorFrete + retorno.valorICMS + retorno.valorISS + BuscarValorTotalComponentes(retorno);
    var freteSemTabelaFrete = new FreteSemTabelaFrete();
    freteSemTabelaFrete.Descricao.val(Localization.Resources.Cargas.Carga.FreteInformadoPeloEmbarcador);
    freteSemTabelaFrete.ValorFreteAPagar.val(Globalize.format(retorno.valorFreteAPagar, "n2"));
    freteSemTabelaFrete.Impostos.val(Globalize.format(retorno.valorICMS + retorno.valorISS, "n2"));
    freteSemTabelaFrete.ValorFreteLiquido.val(Globalize.format(retorno.ValorFreteLiquido, "n2"));
    freteSemTabelaFrete.ValorTotal.val(Globalize.format(valorTotalPrestacao, "n2"));
    freteSemTabelaFrete.CustoFrete.val(retorno.CustoFrete);
    freteSemTabelaFrete.ValorFreteTabela.val(retorno.valorFreteTabelaFrete > 0 ? Globalize.format(retorno.valorFreteTabelaFrete, "n2") : Localization.Resources.Cargas.Carga.ValorNaoDisponivel);
    freteSemTabelaFrete.ValorFreteTabela.visible(true);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        freteSemTabelaFrete.ValorFreteNegociado.val(Globalize.format(retorno.ValorFreteNegociado, "n2"));
        freteSemTabelaFrete.ValorFreteNegociado.visible(true);
    }

    preecherDetalhesTiposFrete(e, _HTMLDetalheFreteSemTabelaFrete, freteSemTabelaFrete, retorno);

    preencherDetalhesFrete(retorno, false);
}