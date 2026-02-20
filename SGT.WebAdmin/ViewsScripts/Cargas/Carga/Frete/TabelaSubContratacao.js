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
/// <reference path="TabelaCliente.js" />
/// <reference path="TabelaComissao.js" />
/// <reference path="TabelaRota.js" />
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


var FretePorFreteSubcontratacao = function () {
    this.TabelaFrete = PropertyEntity({ type: types.local, text: Localization.Resources.Cargas.Carga.TabelaDeFrete });
    this.PercentualCobrado = PropertyEntity({ type: types.local });
    this.ValorTotalCTesParaSubContratacao = PropertyEntity({ type: types.local, text: Localization.Resources.Cargas.Carga.ValorTotalDosCTesParaSubcontratacao });
    this.ValorTotalICMSCTesParaSubContratacao = PropertyEntity({ type: types.local, text: Localization.Resources.Cargas.Carga.ICMSTotalDosCTesParaSubcontratacao });
    this.ValorFrete = PropertyEntity({ type: types.local, text: Localization.Resources.Cargas.Carga.ValorDoFrete });
    this.ValorFreteTabelaFrete = PropertyEntity({ type: types.local, text: Localization.Resources.Cargas.Carga.ValorCalculadoPelaTabela });
    this.DetalhesFrete = PropertyEntity({ eventClick: detalhesFreteClick, type: types.event, text: Localization.Resources.Cargas.Carga.VerDetalhes, visible: ko.observable(true) });
}


var _HTMLDetalheFretePorSubContratacao;
function loadCargaFreteSubcontratacao() {
    $.get("Content/Static/Carga/DetalhesFreteCTeSubcontratacao.html?dyn=" + guid(), function (data) {
        _HTMLDetalheFretePorSubContratacao = data;
    });
}


function preencherRetornoFreteSubcontratacao(e, retorno) {
    var freteUtilizado = retorno.dadosRetornoTipoFrete;
    var detalheFretePorFreteSubcontratacao = new FretePorFreteSubcontratacao();
    detalheFretePorFreteSubcontratacao.TabelaFrete.val(freteUtilizado.TabelaFrete);
    var valorTotalPrestacao = retorno.valorFrete + retorno.valorICMS + retorno.valorISS + BuscarValorTotalComponentes(retorno);
    detalheFretePorFreteSubcontratacao.ValorTotalICMSCTesParaSubContratacao.val(Globalize.format(freteUtilizado.valorTotalCTesParaSubContratacao, "n2"));
    detalheFretePorFreteSubcontratacao.ValorTotalCTesParaSubContratacao.val(Globalize.format(freteUtilizado.valorTotalICMSCTesParaSubContratacao, "n2"));
    detalheFretePorFreteSubcontratacao.ValorFreteTabelaFrete.val(Globalize.format(retorno.valorFreteTabelaFrete, "n2"));
    detalheFretePorFreteSubcontratacao.ValorFrete.val(Globalize.format(valorTotalPrestacao, "n2"));
    e.ValorFreteTabelaFrete.val(retorno.valorFreteTabelaFrete);
    preecherDetalhesTiposFrete(e, _HTMLDetalheFretePorSubContratacao, detalheFretePorFreteSubcontratacao, retorno);
    PreecherInformacaoValorFrete(e, retorno.valorFreteAPagar);
    preencherDetalhesFrete(retorno, false);
}

