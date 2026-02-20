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
/// <reference path="../../../../js/plugin/dropzone/dropzone.js" />
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
/// <reference path="CargaPedidoDocumentoCTe.js" />
/// <reference path="ConsultaReceita.js" />
/// <reference path="CTe.js" />
/// <reference path="Documentos.js" />
/// <reference path="DropZone.js" />
/// <reference path="EtapaDocumentos.js" />
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
/// <reference path="../../../Enumeradores/EnumTipoConsultaPortalFazenda.js" />
/// <reference path="../../../Enumeradores/EnumPermissoesEdicaoCTe.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoMinutaAvon.js" />
/// <reference path="../../../Enumeradores/EnumTipoOperacaoNotaFiscal.js" />
/// <reference path="../../../Enumeradores/EnumClassificacaoNFe.js" />
/// <reference path="../../../Enumeradores/EnumTipoEmissaoNotaFiscal.js" />
/// <reference path="NotaFiscalDimensao.js" />
/// <reference path="NotaFiscalProduto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _notaFiscal, _notaFiscalEdicao, _CRUDNotaFiscalEdicao, _CRUDNotaFiscal, _gridCartaCorrecao, _pesquisaCartaCorrecao;
var NotaFiscal = function () {

    this.CargaPedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Chave = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.ChaveDeAcesso.getRequiredFieldDescription(), def: "", maxlength: 54, required: ko.observable(true), visible: ko.observable(true), idBtnSearch: guid() });
    this.TipoDocumento = PropertyEntity({ val: ko.observable(EnumTipoDocumento.NFe), def: EnumTipoDocumento.NFe });

    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoOperacaoNotaFiscal.Saida), visible: ko.observable(true), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.TipoNFe.getRequiredFieldDescription(), options: EnumTipoOperacaoNotaFiscal.obterOpcoes(), def: EnumTipoOperacaoNotaFiscal.Saida });
    this.Numero = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.Numero.getRequiredFieldDescription(), def: "", getType: typesKnockout.int, maxlength: 12, required: true, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-6 col-lg-6") });
    this.NumeroOutroDocumento = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.NumeroOutroDocumento.getFieldDescription(), def: "", maxlength: 20, required: ko.observable(false), visible: ko.observable(false) });

    this.Serie = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.Serie.getRequiredFieldDescription(), def: "", getType: typesKnockout.int, maxlength: 10, required: ko.observable(true), visible: ko.observable(true) });
    this.Modelo = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.Modelo.getRequiredFieldDescription(), def: "", maxlength: 2, required: ko.observable(true), visible: ko.observable(true) });

    this.Descricao = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), def: "", maxlength: 100, required: ko.observable(false), visible: ko.observable(false) });

    this.Valor = PropertyEntity({ val: ko.observable(""), enable: ko.observable(!_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira), text: Localization.Resources.Cargas.Carga.Valor.getFieldDescription(), def: "", getType: typesKnockout.decimal, maxlength: 15 });
    this.Peso = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.Peso.getRequiredFieldDescription(), def: "", getType: typesKnockout.decimal, maxlength: 15, required: true, configDecimal: { precision: 3, allowZero: false, allowNegative: false } });
    this.PesoLiquido = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.PesoLiquido.getFieldDescription(), def: "", getType: typesKnockout.decimal, maxlength: 15 });
    this.DataEmissao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataDeEmissao.getFieldDescription(), enable: ko.observable(true), val: ko.observable(), getType: typesKnockout.dateTime, required: ko.observable(false) });
    this.Volumes = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.Volumes.getFieldDescription(), def: "", getType: typesKnockout.int, maxlength: 10 });
    this.BaseCalculoICMS = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.BaseCalculoICMS.getFieldDescription(), def: "", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true) });
    this.ValorICMS = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorICMS.getFieldDescription(), def: "", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true) });
    this.BaseCalculoST = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.BaseCalculoST.getFieldDescription(), def: "", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true) });
    this.ValorST = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorST.getFieldDescription(), def: "", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true) });
    this.ValorTotalProdutos = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorTotalProdutos.getFieldDescription(), def: "", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true) });
    this.ValorDesconto = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorDeDesconto.getFieldDescription(), def: "", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true) });
    this.ValorIPI = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorIPi.getFieldDescription(), def: "", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true) });
    this.ValorPIS = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorPIS.getFieldDescription(), def: "", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true) });
    this.ValorCOFINS = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorCOFINS.getFieldDescription(), def: "", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true) });

    this.AliquotaImpostoSuspenso = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.AliquotaImpostoSuspenso.getFieldDescription(), def: "", getType: typesKnockout.decimal, maxlength: 15, required: ko.observable(_cargaAtual.TipoOperacao.ObrigatorioInformarAliquotaImpostoSuspensoeValor), visible: ko.observable(true), configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorImpostoSuspenso = PropertyEntity({ val: ko.observable(""), enable: ko.observable(_cargaAtual.TipoOperacao.ObrigatorioInformarAliquotaImpostoSuspensoeValor), text: Localization.Resources.Cargas.Carga.ValorImpostoSuspenso.getFieldDescription(), required: ko.observable(_cargaAtual.TipoOperacao.ObrigatorioInformarAliquotaImpostoSuspensoeValor), def: "", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true) });
    this.ValorCommodities = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorCommodities.getFieldDescription(), def: "", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true) });

    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Destinatario.getFieldDescription(), idBtnSearch: guid(), cssClass: ko.observable("col-12"), required: ko.observable(false) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Remetente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.PermiteInformarRemetenteLancamentoNotaManualCarga) });

    this.NCM = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.NCM.getFieldDescription(), def: "", maxlength: 4, required: false, visible: ko.observable(false) });
    this.CFOP = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.CFOP.getFieldDescription(), def: "", maxlength: 4, required: false, visible: ko.observable(false) });
    this.NumeroControleCliente = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.NumeroControleCliente.getFieldDescription(), def: "", maxlength: 50, required: false, visible: ko.observable(false) });
    this.NumeroReferenciaEDI = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.NumeroReferenciaEDI.getFieldDescription(), def: "", maxlength: 50, required: false, visible: ko.observable(false) });
    this.NumeroCanhoto = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.NumeroCanhoto.getFieldDescription(), def: "", maxlength: 50, required: false, visible: ko.observable(false) });
    this.PINSUFRAMA = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.PINSUFRAMA.getFieldDescription(), def: "", maxlength: 50, required: false, visible: ko.observable(false) });

    this.Embarque = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.Embarque.getFieldDescription(), def: "", maxlength: 150, required: false, visible: ko.observable(false) });
    this.MasterBL = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.MasterBL.getFieldDescription(), def: "", maxlength: 150, required: false, visible: ko.observable(false) });
    this.NumeroDI = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.DI.getFieldDescription(), def: "", maxlength: 150, required: false, visible: ko.observable(false) });

    this.Moeda = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Moeda.getFieldDescription(), visible: ko.observable(_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), def: EnumMoedaCotacaoBancoCentral.Real, issue: 0 });
    this.ValorTotalMoeda = PropertyEntity({ enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira), text: Localization.Resources.Cargas.Carga.ValorEmMoeda.getFieldDescription(), def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, maxlength: 15, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorCotacaoMoeda = PropertyEntity({ enable: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira), text: Localization.Resources.Cargas.Carga.CotacaoDaMoeda.getFieldDescription(), def: "1,0000000000", val: ko.observable("1,0000000000"), getType: typesKnockout.decimal, maxlength: 22, configDecimal: { precision: 10, allowZero: false, allowNegative: false } });

    this.Produtos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.FacturaFake = PropertyEntity({ text: Localization.Resources.Cargas.Carga.FacturaFake.getFieldDescription(), val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });

    this.Moeda.val.subscribe(function (tipoMoeda) {
        ObterCotacaoMoeda(tipoMoeda);
    });

    this.ValorTotalMoeda.val.subscribe(function (novoValor) {
        ConverterValorMoeda();
    });

    if (_cargaAtual.Mercosul.val()) {
        this.DataEmissao.val.subscribe(function (novoValor) {
            ObterCotacaoMoedaPorData();
        });
    }

    this.AliquotaImpostoSuspenso.val.subscribe(function (novoValor) {
        updateValorImpostoSuspenso();
    });
    this.ValorImpostoSuspenso.val.subscribe(function (novoValor) {
        updateAliquotaImpostoSuspenso();
    });
    this.Valor.val.subscribe(function (novoValor) {
        updateValorImpostoSuspenso();
    });
};

var CRUDNotaFiscal = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarNFeClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(PodeEditarValoresDaNota() || _cargaAtual.PermitirTransportadorEnviarNotasFiscais) });
};

var NotaFiscalEdicao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CargaPedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.ChaveVenda = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.ChaveDeVenda.getRequiredFieldDescription(), def: "", maxlength: 54, required: ko.observable(false), visible: ko.observable(false), idBtnSearch: guid() });
    this.Peso = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.Peso.getRequiredFieldDescription(), def: "", getType: typesKnockout.decimal, maxlength: 15, required: true, configDecimal: { precision: 3, allowZero: false, allowNegative: false } });
    this.PesoLiquido = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.PesoLiquido.getFieldDescription(), def: "", getType: typesKnockout.decimal, maxlength: 15, required: true, configDecimal: { precision: 3, allowZero: false, allowNegative: false } });

    this.Altura = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.Altura.getFieldDescription(), visible: ko.observable(false), enable: ko.observable(true), getType: typesKnockout.decimal, maxlength: 15, configDecimal: { precision: 3, allowZero: false, allowNegative: false } });
    this.Largura = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.Largura.getFieldDescription(), visible: ko.observable(false), enable: ko.observable(true), getType: typesKnockout.decimal, maxlength: 15, configDecimal: { precision: 3, allowZero: false, allowNegative: false } });
    this.Comprimento = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.Comprimento.getFieldDescription(), visible: ko.observable(false), enable: ko.observable(true), getType: typesKnockout.decimal, maxlength: 15, configDecimal: { precision: 3, allowZero: false, allowNegative: false } });
    this.MetrosCubicos = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.MetrosCubicos.getFieldDescription(), enable: ko.observable(PodeEditarValoresDaNota()), getType: typesKnockout.decimal, maxlength: 15, configDecimal: { precision: 6, allowZero: false, allowNegative: false } });

    this.Pallets = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.NumeroPallets.getFieldDescription(), enable: ko.observable(PodeEditarValoresDaNota()), getType: typesKnockout.decimal, maxlength: 15, configDecimal: { precision: 3, allowZero: false, allowNegative: false } });

    this.PalletsControle = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.PalletsControleDePallets.getFieldDescription(), enable: ko.observable(true), getType: typesKnockout.int, maxlength: 7, configDecimal: { precision: 0, allowZero: true, allowNegative: false }, visible: ko.observable(_cargaAtual.GrupoPessoa.controlaPallets) });
    this.Volumes = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.Volumes.getFieldDescription(), enable: ko.observable(PodeEditarValoresDaNota()), getType: typesKnockout.int, maxlength: 7, configInt: { precision: 0, allowZero: true, thousand: '' } });

    this.NCM = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.NCM.getFieldDescription(), def: "", maxlength: 4, required: false, visible: ko.observable(false) });
    this.CFOP = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.CFOP.getFieldDescription(), def: "", maxlength: 4, required: false, visible: ko.observable(false) });
    this.NumeroControleCliente = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.NumeroControleCliente.getFieldDescription(), def: "", maxlength: 50, required: false, visible: ko.observable(false) });
    this.NumeroReferenciaEDI = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.NumeroReferenciaEDI.getFieldDescription(), def: "", maxlength: 50, required: false, visible: ko.observable(false) });
    this.NumeroCanhoto = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.NumeroCanhoto.getFieldDescription(), def: "", maxlength: 50, required: false, visible: ko.observable(false) });
    this.PINSUFRAMA = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.PINSUFRAMA.getFieldDescription(), def: "", maxlength: 50, required: false, visible: ko.observable(false) });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Expedidor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Recebedor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });

    var opcoesClassificacaoNF = _cargaAtual.ClassificacaoNFeRemessaVenda.val() ? EnumClassificacaoNFe.obterOpcoesTipoOperacaoRemessaVenda() : EnumClassificacaoNFe.obterOpcoes();
    this.ClassificacaoNFe = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ClassificacaoNFe.getFieldDescription(), val: ko.observable(EnumClassificacaoNFe.SemClassificacao), options: opcoesClassificacaoNF, def: EnumClassificacaoNFe.SemClassificacao, visible: ko.observable(_CONFIGURACAO_TMS.ExibirClassificacaoNFe || _cargaAtual.ClassificacaoNFeRemessaVenda.val()), enable: ko.observable(true) });
    this.SegundoCodigoBarras = PropertyEntity({ text: Localization.Resources.Cargas.Carga.SegundoCodigoDeBarrasNFeContingencia.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), def: "", maxlength: 36, visible: ko.observable(false) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Destinatario.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Remetente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.PermiteInformarRemetenteLancamentoNotaManualCarga), enable: ko.observable(true) });

    this.Embarque = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.Embarque.getFieldDescription(), def: "", maxlength: 150, required: false, visible: ko.observable(false) });
    this.MasterBL = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.MasterBL.getFieldDescription(), def: "", maxlength: 150, required: false, visible: ko.observable(false) });
    this.NumeroDI = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.DI.getFieldDescription(), def: "", maxlength: 150, required: false, visible: ko.observable(false) });

    this.ValorFrete = PropertyEntity({ val: ko.observable("0,00"), enable: ko.observable(PodeEditarValoresDaNota()), text: Localization.Resources.Cargas.Carga.ValorDoFrete.getFieldDescription(), def: "0,00", getType: typesKnockout.decimal, maxlength: 15, required: true, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, visible: ko.observable(true) });
    this.ValorNotaFiscal = PropertyEntity({ val: ko.observable("0,00"), enable: ko.observable(PodeEditarValoresDaNota()), text: Localization.Resources.Cargas.Carga.ValorNotaFiscal.getFieldDescription(), def: "0,00", getType: typesKnockout.decimal, maxlength: 15, required: true, configDecimal: { precision: 2, allowZero: false, allowNegative: false }, visible: ko.observable(false) });
    this.ValorTotalProdutos = PropertyEntity({ val: ko.observable("0,00"), enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorTotalProdutos.getFieldDescription(), def: "0,00", getType: typesKnockout.decimal, maxlength: 15, required: true, configDecimal: { precision: 2, allowZero: false, allowNegative: false }, visible: ko.observable(false) });
    this.FacturaFake = PropertyEntity({ text: Localization.Resources.Cargas.Carga.FacturaFake.getFieldDescription(), val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });

    this.Dimensoes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
};

var PesquisaCartaCorrecao = function () {
    this.Chave = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.string });    
};


var CRUDNotaFiscalEdicao = function () {
    this.Atualizar = PropertyEntity({ eventClick: atualizarNFeClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadCargaDocumentoNF(idElemento) {

    if (_notaFiscal == null) {
        _notaFiscal = new NotaFiscal();
        KoBindings(_notaFiscal, "knoutDetalhesAdicionarNotasCarga");

        new BuscarClientes(_notaFiscal.Destinatario, null, true);
        new BuscarClientes(_notaFiscal.Remetente, null, true);

        _CRUDNotaFiscal = new CRUDNotaFiscal();
        KoBindings(_CRUDNotaFiscal, "knoutCRUDAdicionarNotasCarga");
    }

    _notaFiscalEdicao = new NotaFiscalEdicao();
    KoBindings(_notaFiscalEdicao, "knockoutDetalhes_" + idElemento + "_knoutDocumentosParaEmissao");

    new BuscarClientes(_notaFiscalEdicao.Expedidor, null, true);
    new BuscarClientes(_notaFiscalEdicao.Recebedor, null, true);
    new BuscarClientes(_notaFiscalEdicao.Destinatario, null, true);
    new BuscarClientes(_notaFiscalEdicao.Remetente, null, true);

    _CRUDNotaFiscalEdicao = new CRUDNotaFiscalEdicao();
    KoBindings(_CRUDNotaFiscalEdicao, "divCRUD_" + idElemento + "_knoutDocumentosParaEmissao");

    configurarLayoutNotaFiscalPorTipoSistema();

    LoadCargaNotaFiscalDimensao(idElemento);
    LoadCargaNotaFiscalProduto();
    ObterConfiguracaoTipoOperacaoParaValidacao();
    _pesquisaCartaCorrecao = new PesquisaCartaCorrecao();
    loadGridCartaCorrecao();
    //loadCargaOrganizacao();
}

function loadGridCartaCorrecao() {

    var opcaoDownloadDANFE = { descricao: Localization.Resources.Cargas.Carga.DownloadDANFECCe, id: guid(), evento: "onclick", metodo: downloadDACCeDocumentoEmissaoClick, tamanho: "20", icone: "" };
    var opcaoDownloadXML = { descricao: Localization.Resources.Cargas.Carga.DownloadXMLCCe, id: guid(), evento: "onclick", metodo: downloadXmlCCeDocumentoEmissaoClick, tamanho: "20", icone: "" };
    var menuOpcoes = { descricao: Localization.Resources.Gerais.Geral.Opcoes, tipo: TypeOptionMenu.list, opcoes: [opcaoDownloadDANFE, opcaoDownloadXML] };
    _gridCartaCorrecao = new GridView("gridCartaCorrecao", "DocumentoDestinadoEmpresa/ObterCartaCorrecoes", _pesquisaCartaCorrecao, menuOpcoes, { column: 1, dir: orderDir.desc });
}

function ConsultarChaveNFe() {
    if (_notaFiscal.Chave.val() != "") {
        let chaveNFe = _notaFiscal.Chave.val();
        chaveNFe = chaveNFe.replace(".", "").replace(" ", "");
        if (chaveNFe != "" && chaveNFe.length == 44) {
            let numero = chaveNFe.substring(25, 34).replace(/^0+/, '');
            let serie = chaveNFe.substring(22, 25).replace(/^0+/, '');
            let modelo = chaveNFe.substring(20, 22).replace(/^0+/, '');

            _notaFiscal.Numero.val(numero);
            _notaFiscal.Serie.val(serie);
            _notaFiscal.Modelo.val(modelo);
        }
    }
}

function exibirGridCartaCorrecao(row) {    
    _pesquisaCartaCorrecao.Chave.val(row.Chave);
    _gridCartaCorrecao.CarregarGrid();
    Global.abrirModal("divCartaCorrecao");
}

function configurarLayoutNotaFiscalPorTipoSistema() {
    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal || _CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) {
        _notaFiscal.NCM.visible(true);
        _notaFiscal.CFOP.visible(true);
        _notaFiscal.NumeroControleCliente.visible(true);
        _notaFiscal.NumeroReferenciaEDI.visible(true);
        _notaFiscal.NumeroCanhoto.visible(true);
        _notaFiscal.PINSUFRAMA.visible(true);
        _notaFiscal.Embarque.visible(true);
        _notaFiscal.MasterBL.visible(true);
        _notaFiscal.NumeroDI.visible(true);

        _notaFiscalEdicao.NCM.visible(true);
        _notaFiscalEdicao.CFOP.visible(true);
        _notaFiscalEdicao.NumeroControleCliente.visible(true);
        _notaFiscalEdicao.NumeroReferenciaEDI.visible(true);
        _notaFiscalEdicao.NumeroCanhoto.visible(true);
        _notaFiscalEdicao.PINSUFRAMA.visible(true);
        _notaFiscalEdicao.Expedidor.visible(true);
        _notaFiscalEdicao.Recebedor.visible(true);
        _notaFiscalEdicao.Embarque.visible(true);
        _notaFiscalEdicao.MasterBL.visible(true);
        _notaFiscalEdicao.NumeroDI.visible(true);
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _notaFiscal.NumeroControleCliente.visible(true);
        _notaFiscal.NumeroReferenciaEDI.visible(true);
        _notaFiscalEdicao.NumeroControleCliente.visible(true);
        _notaFiscalEdicao.NumeroReferenciaEDI.visible(true);
        _notaFiscalEdicao.ValorTotalProdutos.visible(true);
        _notaFiscalEdicao.ValorNotaFiscal.visible(true);
        _notaFiscalEdicao.Destinatario.visible(true);
        _notaFiscalEdicao.Remetente.visible(true);
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _notaFiscalEdicao.ValorFrete.visible(false);
        _notaFiscalEdicao.ValorNotaFiscal.visible(true);
    }

    if (_cargaAtual.Mercosul.val()) {
        _notaFiscalEdicao.FacturaFake.visible(true);
    }
}



function ObterCotacaoMoedaPorData() {
    var dados = {
        DataEmissao: _notaFiscal.DataEmissao.val(),
        Moeda: _notaFiscal.Moeda.val()
    };

    if (_notaFiscal.Moeda.val() === EnumMoedaCotacaoBancoCentral.Real) {
        _notaFiscal.ValorCotacaoMoeda.val("1,0000000000");
        ConverterValorMoeda();
    } else {
        executarReST("Cotacao/ConverterMoedaEstrangeiraPedidoPorData", dados, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _notaFiscal.ValorCotacaoMoeda.val(r.Data);
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                    _notaFiscal.ValorCotacaoMoeda.val("0,0000000000");
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                _notaFiscal.ValorCotacaoMoeda.val("0,0000000000");
            }

            ConverterValorMoeda();
        });
    }
}

function getPorcetagem(valor, percentual) {
    if (valor && percentual) {
        return (percentual / 100) * valor;
    }
    return 0;
}
function getPercentual(valorPrincipal, valorSecundario) {
    if (valorPrincipal && valorSecundario) {
        return (valorSecundario / valorPrincipal) * 100;
    }
    return 0;
}
function updateValorImpostoSuspenso() {
    if (_notaFiscal.AliquotaImpostoSuspenso.val() && _notaFiscal.Valor.val()) {
        var aliquotaImpostoSuspenso = Globalize.parseFloat(_notaFiscal.AliquotaImpostoSuspenso.val());
        var valor = Globalize.parseFloat(_notaFiscal.Valor.val());
        _notaFiscal.ValorImpostoSuspenso.val(Globalize.format(getPorcetagem(valor, aliquotaImpostoSuspenso), "n2"));
    }
}
function updateAliquotaImpostoSuspenso() {
    if (_notaFiscal.ValorImpostoSuspenso.val() && _notaFiscal.Valor.val()) {
        var valorImpostoSuspenso = Globalize.parseFloat(_notaFiscal.ValorImpostoSuspenso.val());
        var valor = Globalize.parseFloat(_notaFiscal.Valor.val());
        _notaFiscal.AliquotaImpostoSuspenso.val(Globalize.format(getPercentual(valor,valorImpostoSuspenso), "n2"));
    }
}
function ObterCotacaoMoeda(tipoMoeda) {
    var dados = {
        CargaPedido: _documentoEmissao.CargaPedido.val(),
        Moeda: tipoMoeda
    };

    if (tipoMoeda === EnumMoedaCotacaoBancoCentral.Real) {
        _notaFiscal.ValorCotacaoMoeda.val("1,0000000000");
        ConverterValorMoeda();
    } else {
        executarReST("Cotacao/ConverterMoedaEstrangeiraPedido", dados, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _notaFiscal.ValorCotacaoMoeda.val(r.Data);
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                    _notaFiscal.ValorCotacaoMoeda.val("0,0000000000");
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                _notaFiscal.ValorCotacaoMoeda.val("0,0000000000");
            }

            ConverterValorMoeda();
        });
    }
}

function ConverterValorMoeda() {
    var valorCotacaoMoeda = Globalize.parseFloat(_notaFiscal.ValorCotacaoMoeda.val());
    var valorTotalMoeda = Globalize.parseFloat(_notaFiscal.ValorTotalMoeda.val());

    if (isNaN(valorCotacaoMoeda))
        valorCotacaoMoeda = 0;
    if (isNaN(valorTotalMoeda))
        valorTotalMoeda = 0;

    var valorTotalConvertido = valorCotacaoMoeda * valorTotalMoeda;

    if (valorTotalConvertido > 0)
        _notaFiscal.Valor.val(Globalize.format(valorTotalConvertido, "n2"));
    else
        _notaFiscal.Valor.val("");
}

function adicionarNFeClick(e, sender) {
    _notaFiscal.CargaPedido.val(_documentoEmissao.CargaPedido.val());
    _notaFiscal.TipoDocumento.val(_documentoEmissao.TipoDocumento.val());
    Salvar(_notaFiscal, "DocumentoNF/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                veririficarSeCargaMudouTipoContratacao(arg.Data);
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                carregarGridDocumentosParaEmissao();
                limparNotasFiscaisEmissao();
                verificarAtualizacaoCarga();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 200000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function DownloadDANFEDocumentoEmissaoClick(e) {
    executarDownload("DocumentoNF/DownloadDANFE", { Pedido: _documentoEmissao.Pedido.val(), CargaPedido: _documentoEmissao.CargaPedido.val(), Codigo: e.Codigo });
}

function downloadXmlDocumentoEmissaoClick(e) {
    executarDownload("DocumentoNF/DownloadXml", { CargaPedido: _documentoEmissao.CargaPedido.val(), Codigo: e.Codigo });
}

function downloadXmlCCeDocumentoEmissaoClick(e) {
    executarDownload("DocumentoNF/DownloadXmlCCe", { CargaPedido: _documentoEmissao.CargaPedido.val(), Codigo: e.Codigo });
}

function downloadDACCeDocumentoEmissaoClick(e) {
    executarDownload("DocumentoNF/DownloadDACCe", { CargaPedido: _documentoEmissao.CargaPedido.val(), Codigo: e.Codigo });
}

function downloadTodosXmlDocumentoEmissaoClick() {
    executarDownloadArquivo("DocumentoNF/DownloadTodosXmlPorCarga", { Carga: _cargaAtual.Codigo.val() });
}

function excluirNFeClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaExcluirNFe.format(e.Numero), function () {
        var data = { Pedido: _documentoEmissao.Pedido.val(), CargaPedido: _documentoEmissao.CargaPedido.val(), Codigo: e.Codigo };
        executarReST("DocumentoNF/Excluir", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    verificarAtualizacaoCarga();
                    if (arg.Data == _TipoContratacaoCargaAtual) {
                        carregarGridDocumentosParaEmissao();
                        limparCamposDocumentosParaEmissao();
                    } else {
                        _cargaAtual.TipoContratacaoCarga.val(arg.Data);
                        carregarDocumentosParaEmissaoPedido(0);
                    }
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function excluirNotasFiscaisClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteExcluirTodasAsNotasFiscais, function () {
        executarReST("DocumentoNF/ExcluirNotasFiscais", { CargaPedido: _documentoEmissao.CargaPedido.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.AsNotasFiscaisForamExcluidasComSucesso);
                    carregarGridDocumentosParaEmissao();
                    verificarAtualizacaoCarga();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function atualizarNFeClick(e, sender) {
    if ((_CONFIGURACAO_TMS.ExibirClassificacaoNFe || _cargaAtual.ClassificacaoNFeRemessaVenda.val()) && _notaFiscalEdicao.ClassificacaoNFe.val() === EnumClassificacaoNFe.SemClassificacao) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Cargas.Carga.NecessarioInformarClassificacao);
        return;
    }

    _notaFiscalEdicao.CargaPedido.val(_documentoEmissao.CargaPedido.val());

    Salvar(_notaFiscalEdicao, "DocumentoNF/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.NotaFiscalAtualizadaComSucesso);
                carregarGridDocumentosParaEmissao();
                verificarAtualizacaoCarga();
                Global.fecharModal("divEdicaoNotaFiscal_" + _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao");
                LimparCampos(_notaFiscalEdicao);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 200000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function abrirTelaEdicaoNFe(e, sender) {
    LimparCamposCargaNotaFiscalDimensao();
    LimparCampos(_notaFiscalEdicao);

    _notaFiscalEdicao.Codigo.val(e.Codigo);

    BuscarPorCodigo(_notaFiscalEdicao, "DocumentoNF/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                var data = arg.Data;
                RecarregarGridCargaNotaFiscalDimensao();

                if (_cargaAtual.SituacaoCarga.val() != EnumSituacoesCarga.AgNFe) {
                    SetarEnableCamposKnockout(_notaFiscalEdicao, false);

                    _CRUDNotaFiscalEdicao.Atualizar.visible(false);
                }

                var habilitarChaveVenda = _cargaAtual.TipoOperacao.exigeChaveVenda;
                _notaFiscalEdicao.ChaveVenda.visible(habilitarChaveVenda);
                _notaFiscalEdicao.ChaveVenda.required(habilitarChaveVenda);

                if (data.TipoEmissao === EnumTipoEmissaoNotaFiscal.ContingenciaFSDA || data.TipoEmissao === EnumTipoEmissaoNotaFiscal.ContingenciaFSIA)
                    _notaFiscalEdicao.SegundoCodigoBarras.visible(true);

                if (data.TipoDocumento === EnumTipoDocumento.NFe) {
                    _notaFiscalEdicao.Destinatario.enable(false);
                    _notaFiscalEdicao.Remetente.enable(false);
                }

                Global.abrirModal("divEdicaoNotaFiscal_" + _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function verificarAtualizacaoCarga() {
    if (_cargaAtual.Mercosul.val()) {
        executarReST("Carga/BuscarCargaPorCodigo", { Carga: _cargaAtual.Codigo.val() }, function (arg) {
            if (arg.Success) {
                atualizarDadosCarga(_cargaAtual, arg.Data);
            }
        });
    }
}

//*******METODOS*******

function iniciarEnvioNotaFiscalManual() {

    _notaFiscal.FacturaFake.visible(false);
    _notaFiscal.Descricao.required(false);
    _notaFiscal.DataEmissao.required(false);

    if (_documentoEmissao.TipoDocumento.val() === EnumTipoDocumento.NFe) {
        $("#TituloNotasCarga").text(Localization.Resources.Cargas.Carga.NotasFiscais);
        _notaFiscal.Serie.visible(true);
        _notaFiscal.Modelo.visible(true);
        _notaFiscal.Chave.visible(true);
        _notaFiscal.Descricao.visible(false);
        _notaFiscal.Tipo.visible(true);
        _notaFiscal.Serie.required(false);
        _notaFiscal.Modelo.required(true);
        _notaFiscal.Chave.required(true);
        _notaFiscal.Descricao.required(false);
        _notaFiscal.NumeroOutroDocumento.visible(false);
        _notaFiscal.Numero.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-6");
    } else {
        _notaFiscal.Serie.visible(false);
        _notaFiscal.Modelo.visible(false);
        _notaFiscal.Chave.visible(false);
        _notaFiscal.Tipo.visible(false);
        _notaFiscal.Descricao.visible(true);
        _notaFiscal.Serie.required(false);
        _notaFiscal.Modelo.required(false);
        _notaFiscal.Chave.required(false);
        _notaFiscal.Descricao.required(true);

        if (_CONFIGURACAO_TMS.UtilizarNumeroOutroDocumento) {
            _notaFiscal.NumeroOutroDocumento.visible(true);
            _notaFiscal.Numero.cssClass("col col-xs-12 col-sm-12 col-md-3 col-lg-3");
        }
    }

    Global.abrirModal("divModalAdicionarNotasCarga");

    $("#divModalAdicionarNotasCarga").on('hidden.bs.modal', function () {
        limparNotasFiscaisEmissao();
    });

    if ((_cargaAtual.Mercosul.val() || (_cargaAtual.Internacional.val() && _documentoEmissao.TipoDocumento.val() === EnumTipoDocumento.Outros)) && !_pedidoConsultaDocumento.EtapaNfMercosul.val()) {
        $("#TituloNotasCarga").text("Faturas");

        _notaFiscal.Serie.visible(false);
        _notaFiscal.Serie.required(false);
        _notaFiscal.Modelo.visible(false);
        _notaFiscal.Modelo.required(false);
        _notaFiscal.Chave.visible(false);
        _notaFiscal.Chave.required(false);
        _notaFiscal.Tipo.visible(false);
        _notaFiscal.Descricao.visible(false);
        _notaFiscal.Descricao.required(false);
        _notaFiscal.Destinatario.required(false);
        _notaFiscal.FacturaFake.visible(true);

        _notaFiscal.NumeroControleCliente.visible(false);
        _notaFiscal.NumeroReferenciaEDI.visible(false);

        _notaFiscal.BaseCalculoICMS.visible(false);
        _notaFiscal.ValorICMS.visible(false);
        _notaFiscal.BaseCalculoST.visible(false);
        _notaFiscal.ValorST.visible(false);
        _notaFiscal.ValorTotalProdutos.visible(false);
        _notaFiscal.ValorDesconto.visible(false);
        _notaFiscal.ValorIPI.visible(false);
        _notaFiscal.ValorPIS.visible(false);
        _notaFiscal.ValorCOFINS.visible(false);
        //_notaFiscal.AliquotaImpostoSuspenso.visible(false);
        //_notaFiscal.ValorImpostoSuspenso.visible(false);
        _notaFiscal.DataEmissao.required(true);

        _notaFiscal.Numero.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-6");
        _notaFiscal.Destinatario.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-6");
    }

}

function limparNotasFiscaisEmissao() {
    LimparCampos(_notaFiscal);
    LimparCamposCargaNotaFiscalProduto();
    RecarregarGridCargaNotaFiscalProduto();
    Global.ResetarAba("divModalAdicionarNotasCarga");
}

function PodeEditarValoresDaNota() {
    return !(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
}

function ObterConfiguracaoTipoOperacaoParaValidacao() {
    executarReST("Carga/ObterConfiguracaoTipoOperacao", { CodigoCarga: _cargaAtual.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _notaFiscalEdicao.Peso.enable(r.Data.NaoPermitirEditarPesoBrutoDaNotaFiscalEletronica);
                _documentoEmissao.BuscarDocOSMae.visible(r.Data.BuscarDocumentosEAverbacaoPelaOSMae);
            }
        }
    });
}