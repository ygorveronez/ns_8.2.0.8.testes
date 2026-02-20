/// <reference path="../../../js/Global/Globais.js" />
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
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/CFOP.js" />
/// <reference path="../../Consultas/SerieTransportador.js" />
/// <reference path="../../Enumeradores/EnumTipoTomador.js" />
/// <reference path="../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../Enumeradores/EnumTipoCTe.js" />
/// <reference path="../../Enumeradores/EnumTipoServicoCTe.js" />
/// <reference path="../../Enumeradores/EnumTipoImpressaoCTe.js" />
/// <reference path="../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../Enumeradores/EnumTipoCFOP.js" />
/// <reference path="../../Enumeradores/EnumPermissoesEdicaoCTe.js" />
/// <reference path="../../Enumeradores/EnumModalidadePessoa.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="../../Enumeradores/EnumIndicadorIETomador.js" />
/// <reference path="../../Enumeradores/EnumTipoModal.js" />
/// <reference path="Documento.js" />
/// <reference path="Participante.js" />
/// <reference path="Rodoviario.js" />
/// <reference path="Veiculo.js" />
/// <reference path="Motorista.js" />
/// <reference path="InformacaoCarga.js" />
/// <reference path="QuantidadeCarga.js" />
/// <reference path="Seguro.js" />
/// <reference path="DocumentoTransporteAnteriorPapel.js" />
/// <reference path="DocumentoTransporteAnteriorEletronico.js" />
/// <reference path="Duplicata.js" />
/// <reference path="DuplicataAutomatica.js" />
/// <reference path="Observacao.js" />
/// <reference path="TotalServico.js" />
/// <reference path="ComponentePrestacaoServico.js" />
/// <reference path="ObservacaoGeral.js" />
/// <reference path="ModalDutoviario.js" />
/// <reference path="ModalMultimodal.js" />
/// <reference path="ModalFerroviario.js" />
/// <reference path="ModalAquaviario.js" />
/// <reference path="ModalAereo.js" />
/// <reference path="ModalAereoManuseio.js" />
/// <reference path="ModalAquaviarioBalsa.js" />
/// <reference path="ModalAquaviarioContainer.js" />
/// <reference path="ModalAquaviarioContainerDocumento.js" />
/// <reference path="ModalFerroviarioFerrovia.js" />
/// <reference path="InformacaoModal.js" />
/// <reference path="CTeSubstituicao.js" />
/// <reference path="CTeAnulacao.js" />
/// <reference path="EntregaSimplificado.js" />
/// <reference path="EntregaSimplificadoDocumento.js" />
/// <reference path="EntregaSimplificadoDocumentoTransporteAnterior.js" />
/// <reference path="EntregaSimplificadoComponentePrestacaoServico.js" />

var _HTMLEmissaoCTe = "";

var DetalhesCTe = function (emissaoCTe) {
    var $this = this;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(true) });
    this.CodigoDuplicado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(true) });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.Transportador.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), required: true, enable: ko.observable(true) });
    this.Terceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.TransportadorEmitente.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false), required: false, enable: ko.observable(true) });

    this.Numero = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.CTes.CTe.Numero.getFieldDescription(), enable: ko.observable(false), visible: ko.observable(true) });
    this.NumeroTerceiro = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.CTes.CTe.Numero.getRequiredFieldDescription(), enable: ko.observable(true), required: false, visible: ko.observable(false) });

    this.Serie = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.Serie.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.SerieTerceiro = PropertyEntity({ text: Localization.Resources.CTes.CTe.Serie.getRequiredFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(false), enable: ko.observable(true) });

    this.Chave = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.CTes.CTe.Chave.getFieldDescription(), enable: ko.observable(false), visible: ko.observable(false) });
    this.ChaveTerceiro = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.CTes.CTe.Chave.getFieldDescription(), enable: ko.observable(true), visible: ko.observable(false), required: false });

    this.LocalidadeEmissao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.LocalidadeDeEmissao.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.LocalidadeInicioPrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.InicioDaPrestacao.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.EstadoInicioPrestacao = PropertyEntity({});
    this.LocalidadeTerminoPrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.TerminoDaPrestacao.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoCTe.Normal), def: EnumTipoCTe.Normal, options: EnumTipoCTe.ObterOpcoes(), text: Localization.Resources.CTes.CTe.Tipo.getRequiredFieldDescription(), required: true, enable: ko.observable(false) });
    this.TipoPagamento = PropertyEntity({ val: ko.observable(0), def: 0, options: EnumTipoPagamento.obterOpcoes(), text: Localization.Resources.CTes.CTe.Pagamento.getRequiredFieldDescription(), required: true, enable: ko.observable(true) });
    this.TipoServico = PropertyEntity({ val: ko.observable(0), def: 0, options: EnumTipoServicoCTe.obterOpcoes(), text: Localization.Resources.CTes.CTe.Servico.getRequiredFieldDescription(), required: true, enable: ko.observable(true) });
    this.TipoImpressao = PropertyEntity({ val: ko.observable(1), def: 1, options: EnumTipoImpressaoCTe.obterOpcoes(), text: Localization.Resources.CTes.CTe.Impressao.getRequiredFieldDescription(), required: true, enable: ko.observable(true) });
    this.TipoTomador = PropertyEntity({ val: ko.observable(0), def: 0, options: EnumTipoTomador.obterOpcoes(), text: Localization.Resources.CTes.CTe.Tomador.getRequiredFieldDescription(), required: true, enable: ko.observable(true) });
    this.CFOP = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.CFOP.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.dateTime, text: Localization.Resources.CTes.CTe.DataDeEmissao.getRequiredFieldDescription(), required: true, enable: ko.observable(true) });
    this.RecebedorRetira = PropertyEntity({ text: Localization.Resources.CTes.CTe.RecebedorRetiraNoAeroportoFilialPortoOuEstacaoDeDestino, val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true) });
    this.RecebedorRetiraDetalhes = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.CTes.CTe.DetalhesDaRetiradaDoRecebedor.getRequiredFieldDescription(), enable: ko.observable(true), maxlength: 160 });
    this.Status = PropertyEntity({ visible: false, enable: ko.observable(true) });
    this.IndicadorTomador = PropertyEntity({ val: ko.observable(EnumIndicadorIETomador.Contribuinte), def: EnumIndicadorIETomador.Contribuinte, options: EnumIndicadorIETomador.obterOpcoes(), text: Localization.Resources.CTes.CTe.IndicadorIETomador.getRequiredFieldDescription(), required: true, enable: ko.observable(true) });
    this.Globalizado = PropertyEntity({ val: ko.observable(EnumSimNao.Nao), def: EnumSimNao.Nao, options: EnumSimNao.obterOpcoes(), text: Localization.Resources.CTes.CTe.Globalizado.getRequiredFieldDescription(), required: true, enable: ko.observable(true) });
    this.TipoModal = PropertyEntity({ val: ko.observable(EnumTipoModal.Rodoviario), options: EnumTipoModal.obterOpcoes(), def: EnumTipoModal.Rodoviario, text: Localization.Resources.CTes.CTe.TipoModal.getRequiredFieldDescription(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.ProtocoloAutorizacao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.CTes.CTe.ProtocoloDeAutorizacao.getRequiredFieldDescription(), enable: ko.observable(false), visible: ko.observable(false) });
    this.ProtocoloCancelamentoInutilizacao = PropertyEntity({ getType: typesKnockout.string, text: ko.observable(Localization.Resources.CTes.CTe.ProtocoloDeCancelamentoInutilizacao.getFieldDescription()), enable: ko.observable(false), visible: ko.observable(false) });
    this.JustificativaCancelamento = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.CTes.CTe.JustificativaDeCancelamento.getFieldDescription(), enable: ko.observable(false), visible: ko.observable(false) });

    this.TipoPagamento.val.subscribe(function (novoValor) {
        if (novoValor === EnumTipoPagamento.Pago)
            $this.TipoTomador.val(EnumTipoTomador.Remetente);
        else if (novoValor === EnumTipoPagamento.A_Pagar)
            $this.TipoTomador.val(EnumTipoTomador.Destinatario);
        else
            $this.TipoTomador.val(EnumTipoTomador.Outros);
    });

    this.TipoTomador.val.subscribe(function (novoValor) {
        emissaoCTe.Seguro.ObterSeguroTomador();
    });

    this.Tipo.val.subscribe(function (novoValor) {
        $("#liTabCTeOutros_" + emissaoCTe.IdModal).hide();
        $("#liCTeSubstituicao_" + emissaoCTe.IdModal).hide();
        $("#liCTeComplementar_" + emissaoCTe.IdModal).hide();
        $("#liCTeAnulacao_" + emissaoCTe.IdModal).hide();
        $("#liTabEntregaSimplificado_" + emissaoCTe.IdModal).hide();
        $("#liTabDocumento_" + emissaoCTe.IdModal).show();

        if (novoValor === EnumTipoCTe.Complementar) {
            if ($this.Codigo.val() === 0 && _CONFIGURACAO_TMS.PermiteEmitirCTeComplementarManualmente !== true) {
                $this.Tipo.val(EnumTipoCTe.Normal);
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.CTes.CTe.NaoPossivelEmitirUmCTeComplementarManualmente);
                return;
            }

            $("#liTabCTeOutros_" + emissaoCTe.IdModal).show();
            $("#liCTeComplementar_" + emissaoCTe.IdModal).show();
            Global.ExibirAba("tabCTeComplementar_" + emissaoCTe.IdModal);

        } else if (novoValor === EnumTipoCTe.Substituicao) {
            if ($this.Codigo.val() === 0 && _CONFIGURACAO_TMS.UtilizaEmissaoMultimodal !== true) {
                $this.Tipo.val(EnumTipoCTe.Normal);
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.CTes.CTe.NaoPossivelEmitirUmCTeDeSubstituicaoManualmente);
                return;
            }

            $("#liTabCTeOutros_" + emissaoCTe.IdModal).show();
            $("#liCTeSubstituicao_" + emissaoCTe.IdModal).show();
            Global.ExibirAba("tabCTeSubstituicao_" + emissaoCTe.IdModal);
        } else if (novoValor === EnumTipoCTe.Anulacao) {
            if ($this.Codigo.val() === 0 && _CONFIGURACAO_TMS.UtilizaEmissaoMultimodal !== true) {
                $this.Tipo.val(EnumTipoCTe.Normal);
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.CTes.CTe.NaoPossivelEmitirUmCTeDeAnulacaoMnualmente);
                return;
            }

            $("#liTabCTeOutros_" + emissaoCTe.IdModal).show();
            $("#liCTeAnulacao_" + emissaoCTe.IdModal).show();
            Global.ExibirAba("tabCTeAnulacao_" + emissaoCTe.IdModal);
        } else if (novoValor === EnumTipoCTe.Simplificado) {
            $("#liTabDocumento_" + emissaoCTe.IdModal).hide();
            $("#liTabDocumentosAnteriores_" + emissaoCTe.IdModal).hide();
            $("#liTabEntregaSimplificado_" + emissaoCTe.IdModal).show();
            emissaoCTe.TotalServico.ValorFrete.enable(false);
            emissaoCTe.TotalServico.ValorPrestacaoServico.enable(false);
            emissaoCTe.TotalServico.ValorReceber.enable(false);
        }
    });

    this.TipoModal.val.subscribe(function (novoValor) {
        $("#liModalRodoviario").hide();
        $("#liModalAereo").hide();
        $("#liModalAquaviario").hide();
        $("#liModalFerroviario").hide();
        $("#liModalDutoviario").hide();
        $("#liModalMultimodal").hide();
        $("#liModalAquaviarioContainer").hide();

        if (novoValor === EnumTipoModal.Aereo) {
            $("#liModalAereo").show();
            Global.ExibirAba("tabModalAereo_" + emissaoCTe.IdModal);
        }
        else if (novoValor === EnumTipoModal.Aquaviario) {
            $("#liModalAquaviarioContainer").show();
            $("#liModalAquaviario").show();
            Global.ExibirAba("tabModalAquaviario_" + emissaoCTe.IdModal);
            Global.ExibirAba("tabModalAquaviarioContainer_" + emissaoCTe.IdModal);
        }
        else if (novoValor === EnumTipoModal.Ferroviario) {
            $("#liModalFerroviario").show();
            Global.ExibirAba("tabModalFerroviario_" + emissaoCTe.IdModal);
        }
        else if (novoValor === EnumTipoModal.Dutoviario) {
            $("#liModalDutoviario").show();
            Global.ExibirAba("tabModalDutoviario_" + emissaoCTe.IdModal);
        }
        else if (novoValor === EnumTipoModal.Multimodal) {
            $("#liModalAquaviarioContainer").show();
            $("#liModalMultimodal").show();            
            Global.ExibirAba("tabModalMultimodal_" + emissaoCTe.IdModal);
            Global.ExibirAba("tabModalAquaviarioContainer_" + emissaoCTe.IdModal);
        }
        else {
            $("#liModalRodoviario").show();
            Global.ExibirAba("tabModalRodoviario_" + emissaoCTe.IdModal);
        }
    });
};

var CRUDCTe = function () {
    this.Emitir = PropertyEntity({ type: types.event, text: Localization.Resources.CTes.CTe.AutorizarEmissao, visible: ko.observable(false), enable: ko.observable(true) });
    this.Salvar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(false), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true), enable: ko.observable(true) });
};

function EmissaoCTe(codigoCTe, callbackInit, permissoes, adicionarCTeTerceiro, duplicar, codigoCargaPedido, codigoDocumentoCTeTerceiro) {

    var emissaoCTe = this;

    if (adicionarCTeTerceiro == null)
        adicionarCTeTerceiro = false;

    if (duplicar == null)
        duplicar = false;

    if (codigoCargaPedido == null || codigoCargaPedido == undefined)
        codigoCargaPedido = 0;

    if (codigoDocumentoCTeTerceiro == null || codigoDocumentoCTeTerceiro == undefined)
        codigoDocumentoCTeTerceiro = 0;

    this.LoadEmissaoCTe = function () {

        LoadLocalizationResources("CTes.CTe").then(function () {

            emissaoCTe.IdModal = guid();
            emissaoCTe.IdKnockoutCTe = "knockoutCTe_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutCRUDCTe = "knockoutCRUDCTe_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutRemetente = "knockoutRemetente_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutDestinatario = "knockoutDestinatario_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutExpedidor = "knockoutExpedidor_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutRecebedor = "knockoutRecebedor_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutTomador = "knockoutTomador_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutDocumento = "knockoutDocumento_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutEntregaSimplificado = "knockoutEntregaSimplificado_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutEntregaSimplificadoDocumento = "knockoutEntregaSimplificadoDocumento_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutEntregaSimplificadoDocumentoTransporteAnterior = "knockoutEntregaSimplificadoDocumentoTransporteAnterior_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutEntregaSimplificadoComponentePrestacaoServico = "knockoutEntregaSimplificadoComponentePrestacaoServico_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutRodoviario = "knockoutRodoviario_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutVeiculo = "knockoutVeiculo_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutMotorista = "knockoutMotorista_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutInformacaoCarga = "knockoutInformacaoCarga_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutQuantidadeCarga = "knockoutQuantidadeCarga_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutSeguro = "knockoutSeguro_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutDocumentoTransporteAnteriorPapel = "knockoutDocumentoTransporteAnteriorPapel_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutDocumentoTransporteAnteriorEletronico = "knockoutDocumentoTransporteAnteriorEletronico_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutProdutoPerigoso = "knockoutProdutoPerigoso_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutDuplicata = "knockoutDuplicata_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutDuplicataAutomatica = "knockoutDuplicataAutomatica_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutObservacao = "knockoutObservacao_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutObservacaoFisco = "knockoutObservacaoFisco_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutObservacaoContribuinte = "knockoutObservacaoContribuinte_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutTotalServico = "knockoutTotalServico_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutComponente = "knockoutComponente_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutModalDutoviario = "knockoutModalDutoviario_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutModalMultimodal = "knockoutModalMultimodal_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutModalFerroviario = "knockoutModalFerroviario_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutModalAquaviario = "knockoutModalAquaviario_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutModalAereo = "knockoutModalAereo_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutModalAereoManuseio = "knockoutModalAereoManuseio_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutModalAquaviarioBalsa = "knockoutModalAquaviarioBalsa_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutModalAquaviarioContainer = "knockoutModalAquaviarioContainer_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutModalFerroviarioFerrovia = "knockoutModalFerroviarioFerrovia_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutModalAquaviarioContainerDocumento = "knockoutModalAquaviarioContainerDocumento_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutInformacaoModal = "knockoutInformacaoModal_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutCTeSubstituicao = "knockoutCTeSubstituicao_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutCTeAnulacao = "knockoutCTeAnulacao_" + emissaoCTe.IdModal;
            emissaoCTe.IdKnockoutCTeComplementar = "knockoutCTeComplementar_" + emissaoCTe.IdModal;

            emissaoCTe.CTe = new DetalhesCTe(emissaoCTe);
            emissaoCTe.CTe.DataEmissao.val(moment().format("DD/MM/YYYY HH:mm"));
            emissaoCTe.Documento = new DocumentoCTe(emissaoCTe);
            emissaoCTe.EntregaSimplificado = new EntregaSimplificado(emissaoCTe);
            emissaoCTe.EntregaSimplificadoDocumento = new EntregaSimplificadoDocumento(emissaoCTe);
            emissaoCTe.EntregaSimplificadoDocumentoTransporteAnterior = new EntregaSimplificadoDocumentoTransporteAnterior(emissaoCTe);
            emissaoCTe.EntregaSimplificadoComponentePrestacaoServico = new EntregaSimplificadoComponentePrestacaoServico(emissaoCTe);
            emissaoCTe.Veiculo = new Veiculo(emissaoCTe);
            emissaoCTe.Motorista = new Motorista(emissaoCTe);
            emissaoCTe.QuantidadeCarga = new QuantidadeCarga(emissaoCTe);
            emissaoCTe.Seguro = new Seguro(emissaoCTe);
            emissaoCTe.DocumentoTransporteAnteriorPapel = new DocumentoTransporteAnteriorPapel(emissaoCTe);
            emissaoCTe.DocumentoTransporteAnteriorEletronico = new DocumentoTransporteAnteriorEletronico(emissaoCTe);
            emissaoCTe.InformacaoCarga = new InformacaoCarga(emissaoCTe);
            emissaoCTe.ProdutoPerigoso = new ProdutoPerigoso(emissaoCTe);
            emissaoCTe.DuplicataAutomatica = new DuplicataAutomatica(emissaoCTe);
            emissaoCTe.Duplicata = new Duplicata(emissaoCTe);
            emissaoCTe.Remetente = new Participante(emissaoCTe, emissaoCTe.IdKnockoutRemetente);
            emissaoCTe.Remetente.Cliente.callback = function (p) {
                emissaoCTe.SetarInicioPrestacao(p);
                emissaoCTe.SetarSerie();
            };
            emissaoCTe.Destinatario = new Participante(emissaoCTe, emissaoCTe.IdKnockoutDestinatario);
            emissaoCTe.Destinatario.Cliente.callback = emissaoCTe.SetarTerminoPrestacao;
            emissaoCTe.Expedidor = new Participante(emissaoCTe, emissaoCTe.IdKnockoutExpedidor);
            emissaoCTe.Expedidor.Cliente.callback = emissaoCTe.SetarInicioPrestacao;
            emissaoCTe.Recebedor = new Participante(emissaoCTe, emissaoCTe.IdKnockoutRecebedor);
            emissaoCTe.Recebedor.Cliente.callback = emissaoCTe.SetarTerminoPrestacao;
            emissaoCTe.Tomador = new Participante(emissaoCTe, emissaoCTe.IdKnockoutTomador);

            emissaoCTe.Rodoviario = new Rodoviario(emissaoCTe);
            emissaoCTe.ModalDutoviario = new ModalDutoviario(emissaoCTe);
            emissaoCTe.ModalMultimodal = new ModalMultimodal(emissaoCTe);
            emissaoCTe.ModalFerroviario = new ModalFerroviario(emissaoCTe);
            emissaoCTe.ModalAquaviario = new ModalAquaviario(emissaoCTe);
            emissaoCTe.ModalAereo = new ModalAereo(emissaoCTe);
            emissaoCTe.ModalAereoManuseio = new ModalAereoManuseio(emissaoCTe);
            emissaoCTe.ModalAquaviarioBalsa = new ModalAquaviarioBalsa(emissaoCTe);
            emissaoCTe.ModalAquaviarioContainerDocumento = new ModalAquaviarioContainerDocumento(emissaoCTe);
            emissaoCTe.ModalAquaviarioContainer = new ModalAquaviarioContainer(emissaoCTe, emissaoCTe.ModalAquaviarioContainerDocumento);
            emissaoCTe.ModalFerroviarioFerrovia = new ModalFerroviarioFerrovia(emissaoCTe);
            emissaoCTe.InformacaoModal = new InformacaoModal(emissaoCTe);

            emissaoCTe.ObservacaoGeral = new ObservacaoGeral(emissaoCTe);
            emissaoCTe.TotalServico = new TotalServico(emissaoCTe);
            emissaoCTe.Componente = new ComponentePrestacaoServico(emissaoCTe);
            emissaoCTe.CTeSubstituicao = new CTeSubstituicao(emissaoCTe);
            emissaoCTe.CTeAnulacao = new CTeAnulacao(emissaoCTe);
            emissaoCTe.CTeComplementar = new CTeComplementar(emissaoCTe);

            emissaoCTe.GridObservacaoFisco = null;
            emissaoCTe.ObservacaoFisco = new Observacao(emissaoCTe.GridObservacaoFisco, emissaoCTe.IdKnockoutObservacaoFisco, 60);

            emissaoCTe.GridObservacaoContribuinte = null;
            emissaoCTe.ObservacaoContribuinte = new Observacao(emissaoCTe.GridObservacaoContribuinte, emissaoCTe.IdKnockoutObservacaoContribuinte, 160);

            emissaoCTe.CRUDCTe = new CRUDCTe();

            if (codigoCTe != null && codigoCTe > 0)
                emissaoCTe.CTe.Codigo.val(codigoCTe);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && _CONFIGURACAO_TMS.PermiteEmitirCTeComplementarManualmente === true)
                emissaoCTe.CTe.Tipo.enable(true);
            
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && !_CONFIGURACAO_TMS.PermitirAlterarEmpresaNoCTeManual)
                emissaoCTe.CTe.Empresa.enable(false);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
                emissaoCTe.CTe.Tipo.enable(true);
                emissaoCTe.CTe.Empresa.visible(false);
                emissaoCTe.CTe.Empresa.required = false;
                executarReST("NotaFiscalEletronica/BuscarDadosEmpresa", null, function (r) {
                    if (r.Success) {
                        if (r.Data.Empresa != null) {
                            emissaoCTe.CTe.Empresa.codEntity(r.Data.Empresa.Codigo);
                            emissaoCTe.CTe.Empresa.val(r.Data.Empresa.Descricao);
                            emissaoCTe.ObterInformacoesEmpresa(r.Data.Empresa.Codigo);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                    }
                });
            }
            emissaoCTe.RenderizarModalCTe(emissaoCTe, callbackInit, duplicar, codigoCargaPedido);

            LocalizeCurrentPage();
        });
    };

    this.SetarTerminoPrestacao = function (participante) {
        emissaoCTe.CTe.LocalidadeTerminoPrestacao.codEntity(participante.Localidade.Codigo);
        emissaoCTe.CTe.LocalidadeTerminoPrestacao.val(participante.Localidade.Descricao);
    };

    this.SetarInicioPrestacao = function (participante) {
        emissaoCTe.CTe.LocalidadeInicioPrestacao.codEntity(participante.Localidade.Codigo);
        emissaoCTe.CTe.LocalidadeInicioPrestacao.val(participante.Localidade.Descricao);
        emissaoCTe.CTe.EstadoInicioPrestacao.val(participante.Localidade.Estado);
    };

    this.SetarPermissoes = function () {

        if (permissoes != null) {

            var permissaoTotal = true;
            var somenteLeitura = false;
            if (emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.total))
                permissaoTotal = true;

            if (emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.Nenhuma))
                somenteLeitura = true;

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.CTe) && !permissaoTotal) || somenteLeitura) {

                emissaoCTe.DestivarCTe();

                if (emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.AlterarCFOP)) {
                    emissaoCTe.CTe.CFOP.enable(true);
                    emissaoCTe.CTe.IndicadorTomador.enable(true);
                }

                if (emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.AlterarSerie))
                    emissaoCTe.CTe.Serie.enable(true);

                if (emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.AlterarTipoPagamento))
                    emissaoCTe.CTe.TipoPagamento.enable(true);
            }

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.Documento) && !permissaoTotal) || somenteLeitura) {
                emissaoCTe.Documento.DestivarDocumento();
                emissaoCTe.EntregaSimplificado.DesativarEntregaSimplificado();
            }

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.Veiculo) && !permissaoTotal) || somenteLeitura)
                emissaoCTe.Veiculo.DestivarVeiculo();

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.Motorista) && !permissaoTotal) || somenteLeitura)
                emissaoCTe.Motorista.DestivarMotorista();

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.QuantidadeCarga) && !permissaoTotal) || somenteLeitura)
                emissaoCTe.QuantidadeCarga.DestivarQuantidadeCarga();

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.Seguro) && !permissaoTotal) || somenteLeitura)
                emissaoCTe.Seguro.DestivarSeguro();

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.DocumentoTransporteAnteriorPapel) && !permissaoTotal) || somenteLeitura)
                emissaoCTe.DocumentoTransporteAnteriorPapel.DestivarDocumentoTransporteAnteriorPapel();

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.DocumentoTransporteAnteriorEletronico) && !permissaoTotal) || somenteLeitura)
                emissaoCTe.DocumentoTransporteAnteriorEletronico.DestivarDocumentoTransporteAnteriorEletronico();

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.InformacaoCarga) && !permissaoTotal) || somenteLeitura) {
                emissaoCTe.InformacaoCarga.DestivarInformacaoCarga();
                emissaoCTe.InformacaoCarga.AtivarPermissoesEspecificas(emissaoCTe);
            }

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.ProdutoPerigoso) && !permissaoTotal) || somenteLeitura)
                emissaoCTe.ProdutoPerigoso.DestivarProdutoPerigoso();

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.Duplicata) && !permissaoTotal) || somenteLeitura)
                emissaoCTe.Duplicata.DestivarDuplicata();

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.DuplicataAutomatica) && !permissaoTotal) || somenteLeitura)
                emissaoCTe.DuplicataAutomatica.DestivarDuplicataAutomatica();

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.Remetente) && !permissaoTotal) || somenteLeitura) {
                emissaoCTe.Remetente.DestivarParticipante();
                emissaoCTe.Remetente.AtivarPermissoesEspecificasParticipante(emissaoCTe);
            }

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.Destinatario) && !permissaoTotal) || somenteLeitura) {
                emissaoCTe.Destinatario.DestivarParticipante();
                emissaoCTe.Destinatario.AtivarPermissoesEspecificasParticipante(emissaoCTe);
            }

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.Expedidor) && !permissaoTotal) || somenteLeitura) {
                emissaoCTe.Expedidor.DestivarParticipante();
                emissaoCTe.Expedidor.AtivarPermissoesEspecificasParticipante(emissaoCTe);
            }

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.Recebedor) && !permissaoTotal) || somenteLeitura) {
                emissaoCTe.Recebedor.DestivarParticipante();
                emissaoCTe.Recebedor.AtivarPermissoesEspecificasParticipante(emissaoCTe);
            }

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.Tomador) && !permissaoTotal) || somenteLeitura) {
                emissaoCTe.Tomador.DestivarParticipante();
                emissaoCTe.Tomador.AtivarPermissoesEspecificasParticipante(emissaoCTe);
            }

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.Rodoviario) && !permissaoTotal) || somenteLeitura) {
                emissaoCTe.Rodoviario.DestivarRodoviario();
                emissaoCTe.ModalAereo.DestivarModalAereo();
                emissaoCTe.ModalAquaviario.DestivarModalAquaviario();
                emissaoCTe.ModalFerroviario.DestivarModalFerroviario();
                emissaoCTe.ModalDutoviario.DestivarModalDutoviario();
                emissaoCTe.ModalMultimodal.DestivarModalMultimodal();
                emissaoCTe.InformacaoModal.DestivarInformacaoModal();
                emissaoCTe.CTeSubstituicao.DestivarCTeSubstituicao();
                emissaoCTe.CTeAnulacao.DestivarCTeAnulacao();
                emissaoCTe.CTeComplementar.DesativarCTeComplementar();

                emissaoCTe.ModalAereoManuseio.DestivarManuseio();
                emissaoCTe.ModalAquaviarioBalsa.DestivarBalsa();
                emissaoCTe.ModalAquaviarioContainer.DestivarContainer();
                emissaoCTe.ModalAquaviarioContainerDocumento.DestivarContainerDocumento();
                emissaoCTe.ModalFerroviarioFerrovia.DestivarFerrovia();
            }

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.ObservacaoGeral) && !permissaoTotal) || somenteLeitura)
                emissaoCTe.ObservacaoGeral.DestivarObservacaoGeral();

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.TotalServico) && !permissaoTotal) || somenteLeitura)
                emissaoCTe.TotalServico.DestivarTotalServico();

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.Componente) && !permissaoTotal) || somenteLeitura)
                emissaoCTe.Componente.DestivarComponentePrestacaoServico();

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.ObservacaoFisco) && !permissaoTotal) || somenteLeitura)
                emissaoCTe.ObservacaoFisco.DestivarObservacao();

            if ((!emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.ObservacaoContribuinte) && !permissaoTotal) || somenteLeitura)
                emissaoCTe.ObservacaoContribuinte.DestivarObservacao();
        }
    };

    this.DestivarCTe = function () {
        DesabilitarCamposInstanciasCTe(emissaoCTe.CTe);
    };

    this.BuscarDadosCTe = function () {
        var p = new promise.Promise();

        executarReST("CTe/BuscarPorCodigo", { Codigo: emissaoCTe.CTe.Codigo.val() }, function (r) {
            if (r.Success) {

                PreencherObjetoKnout(emissaoCTe.CTe, { Data: r.Data.CTe });
                PreencherObjetoKnout(emissaoCTe.Rodoviario, { Data: r.Data.Rodoviario });
                PreencherObjetoKnout(emissaoCTe.InformacaoCarga, { Data: r.Data.InformacaoCarga });

                PreencherObjetoKnout(emissaoCTe.ModalAereo, { Data: r.Data.ModalAereo });
                PreencherObjetoKnout(emissaoCTe.ModalAquaviario, { Data: r.Data.ModalAquaviario });
                PreencherObjetoKnout(emissaoCTe.ModalDutoviario, { Data: r.Data.ModalDutoviario });
                PreencherObjetoKnout(emissaoCTe.ModalFerroviario, { Data: r.Data.ModalFerroviario });
                PreencherObjetoKnout(emissaoCTe.ModalMultimodal, { Data: r.Data.ModalMultimodal });
                PreencherObjetoKnout(emissaoCTe.InformacaoModal, { Data: r.Data.InformacaoModal });

                var statusCTe = r.Data.CTe.Status;

                if (duplicar) {
                    emissaoCTe.CTe.CodigoDuplicado.val(emissaoCTe.CTe.Codigo.val());
                    emissaoCTe.CTe.Codigo.val(0);
                    emissaoCTe.CTe.Numero.val("");
                    emissaoCTe.CTe.Status.val(EnumStatusCTe.EMDIGITACAO);
                    emissaoCTe.CTe.ProtocoloAutorizacao.val("");
                    emissaoCTe.CTe.Chave.val("");
                    emissaoCTe.CTe.DataEmissao.val(moment().format("DD/MM/YYYY HH:mm"));
                    emissaoCTe.CTe.Tipo.enable(true);
                    emissaoCTe.CTeSubstituicao.ChaveCTeSubstituido.val(r.Data.CTe.Chave);
                    emissaoCTe.CTeAnulacao.ChaveCTeAnulado.val(r.Data.CTe.Chave);
                } else {
                    emissaoCTe.CTe.CodigoDuplicado.val(0);
                    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
                        emissaoCTe.CTe.Tipo.enable(false);
                }

                if (statusCTe === EnumStatusCTe.REJEICAO || statusCTe === EnumStatusCTe.EMDIGITACAO) {
                    emissaoCTe.CRUDCTe.Emitir.visible(true);
                    emissaoCTe.CRUDCTe.Salvar.visible(true);
                }

                if (r.Data.Remetente != null) {
                    emissaoCTe.Remetente.NaoLimparDados = true;
                    PreencherObjetoKnout(emissaoCTe.Remetente, { Data: r.Data.Remetente });
                    emissaoCTe.Remetente.NaoLimparDados = false;
                }

                if (r.Data.Destinatario != null) {
                    emissaoCTe.Destinatario.NaoLimparDados = true;
                    PreencherObjetoKnout(emissaoCTe.Destinatario, { Data: r.Data.Destinatario });
                    emissaoCTe.Destinatario.NaoLimparDados = false;
                }

                if (r.Data.Expedidor != null) {
                    emissaoCTe.Expedidor.NaoLimparDados = true;
                    PreencherObjetoKnout(emissaoCTe.Expedidor, { Data: r.Data.Expedidor });
                    emissaoCTe.Expedidor.NaoLimparDados = false;
                }

                if (r.Data.Recebedor != null) {
                    emissaoCTe.Recebedor.NaoLimparDados = true;
                    PreencherObjetoKnout(emissaoCTe.Recebedor, { Data: r.Data.Recebedor });
                    emissaoCTe.Recebedor.NaoLimparDados = false;
                }

                if (r.Data.Tomador != null) {
                    emissaoCTe.Tomador.NaoLimparDados = true;
                    PreencherObjetoKnout(emissaoCTe.Tomador, { Data: r.Data.Tomador });
                    emissaoCTe.Tomador.NaoLimparDados = false;
                }

                if (r.Data.Documentos != null && r.Data.Documentos.length > 0) {
                    emissaoCTe.Documento.TipoDocumento.def = r.Data.Documentos[0].TipoDocumento;
                    emissaoCTe.Documento.TipoDocumento.val(r.Data.Documentos[0].TipoDocumento);
                    emissaoCTe.Documentos = r.Data.Documentos;
                    emissaoCTe.Documento.RecarregarGrid();
                }

                if (r.Data.EntregasSimplificado != null && r.Data.EntregasSimplificado.length > 0) {
                    emissaoCTe.EntregasSimplificado = r.Data.EntregasSimplificado;
                    emissaoCTe.EntregaSimplificado.RecarregarGridSimplificadoEntregas();

                    if (r.Data.EntregasSimplificadoDocumentos != null && r.Data.EntregasSimplificadoDocumentos.length > 0) {
                        emissaoCTe.EntregasSimplificadoDocumentos = r.Data.EntregasSimplificadoDocumentos;
                        emissaoCTe.EntregaSimplificadoDocumento.RecarregarGridSimplificadoDocumentos();
                    }

                    if (r.Data.EntregasSimplificadoDocumentosTransporteAnterior != null && r.Data.EntregasSimplificadoDocumentosTransporteAnterior.length > 0) {
                        emissaoCTe.EntregasSimplificadoDocumentosTransporteAnterior = r.Data.EntregasSimplificadoDocumentosTransporteAnterior;
                        emissaoCTe.EntregaSimplificadoDocumentoTransporteAnterior.RecarregarGridSimplificadoDocumentosTransporteAnterior();
                    }

                    if (r.Data.EntregasSimplificadoComponentesPrestacaoServico != null && r.Data.EntregasSimplificadoComponentesPrestacaoServico.length > 0) {
                        emissaoCTe.EntregasSimplificadoComponentesPrestacaoServico = r.Data.EntregasSimplificadoComponentesPrestacaoServico;
                        emissaoCTe.EntregaSimplificadoComponentePrestacaoServico.RecarregarGridSimplificadoComponentesPrestacaoServico();
                    }
                }

                if (r.Data.Veiculos != null) {
                    emissaoCTe.Veiculos = r.Data.Veiculos;
                    emissaoCTe.Veiculo.RecarregarGrid();
                }

                if (r.Data.Motoristas != null) {
                    emissaoCTe.Motoristas = r.Data.Motoristas;
                    emissaoCTe.Motorista.RecarregarGrid();
                }

                if (r.Data.Manuseios != null) {
                    emissaoCTe.Manuseios = r.Data.Manuseios;
                    emissaoCTe.ModalAereoManuseio.RecarregarGrid();
                }

                if (r.Data.Balsas != null) {
                    emissaoCTe.Balsas = r.Data.Balsas;
                    emissaoCTe.ModalAquaviarioBalsa.RecarregarGrid();
                }

                if (r.Data.Containers != null) {
                    emissaoCTe.Containers = r.Data.Containers;
                    emissaoCTe.ModalAquaviarioContainer.RecarregarGrid();
                }

                if (r.Data.ContainerDocumentos != null) {
                    emissaoCTe.ContainerDocumentos = r.Data.ContainerDocumentos;
                    emissaoCTe.ModalAquaviarioContainerDocumento.RecarregarGrid();
                }

                if (r.Data.Ferrovias != null) {
                    emissaoCTe.Ferrovias = r.Data.Ferrovias;
                    emissaoCTe.ModalFerroviarioFerrovia.RecarregarGrid();
                }

                if (r.Data.QuantidadesCarga != null) {
                    emissaoCTe.QuantidadesCarga = r.Data.QuantidadesCarga;
                    emissaoCTe.QuantidadeCarga.RecarregarGrid();
                }

                if (r.Data.Seguros != null) {
                    emissaoCTe.Seguros = r.Data.Seguros;
                    emissaoCTe.Seguro.RecarregarGrid();
                }

                if (r.Data.DocumentosTransporteAnteriorPapel != null) {
                    emissaoCTe.DocumentosTransporteAnteriorPapel = r.Data.DocumentosTransporteAnteriorPapel;
                    emissaoCTe.DocumentoTransporteAnteriorPapel.RecarregarGrid();
                }

                if (r.Data.DocumentosTransporteAnteriorEletronico != null) {
                    emissaoCTe.DocumentosTransporteAnteriorEletronico = r.Data.DocumentosTransporteAnteriorEletronico;
                    emissaoCTe.DocumentoTransporteAnteriorEletronico.RecarregarGrid();
                }

                if (r.Data.ProdutosPerigosos != null) {
                    emissaoCTe.ProdutosPerigosos = r.Data.ProdutosPerigosos;
                    emissaoCTe.ProdutoPerigoso.RecarregarGrid();
                }

                if (r.Data.ObservacoesContribuinte != null) {
                    emissaoCTe.ObservacaoContribuinte.Lista = r.Data.ObservacoesContribuinte;
                    emissaoCTe.ObservacaoContribuinte.RecarregarGrid();
                }

                if (r.Data.ObservacoesFisco != null) {
                    emissaoCTe.ObservacaoFisco.Lista = r.Data.ObservacoesFisco;
                    emissaoCTe.ObservacaoFisco.RecarregarGrid();
                }

                if (r.Data.Componentes != null) {
                    emissaoCTe.Componentes = r.Data.Componentes;
                    emissaoCTe.Componente.RecarregarGrid();
                }

                if (r.Data.Observacoes != null)
                    PreencherObjetoKnout(emissaoCTe.ObservacaoGeral, { Data: r.Data.Observacoes });

                if (r.Data.TotalServico != null) {
                    PreencherObjetoKnout(emissaoCTe.TotalServico, { Data: r.Data.TotalServico });
                    TrocarICMS(emissaoCTe.TotalServico);
                }

                if (r.Data.CTeSubstituicao != null)
                    PreencherObjetoKnout(emissaoCTe.CTeSubstituicao, { Data: r.Data.CTeSubstituicao });

                if (r.Data.CTeAnulacao != null)
                    PreencherObjetoKnout(emissaoCTe.CTeAnulacao, { Data: r.Data.CTeAnulacao });

                if (r.Data.CTeComplementar != null)
                    PreencherObjetoKnout(emissaoCTe.CTeComplementar, { Data: r.Data.CTeComplementar });

                emissaoCTe.AlterarEstadoCTe(emissaoCTe);

            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }

            p.done();
        });

        return p;
    };

    this.BuscarDadosCTeTerceiro = function () {
        var p = new promise.Promise();

        executarReST("DocumentoCTe/BuscarCTeTerceiroPorCodigo", { Codigo: codigoDocumentoCTeTerceiro }, function (r) {
            if (r.Success) {

                PreencherObjetoKnout(emissaoCTe.CTe, { Data: r.Data.CTe });
                PreencherObjetoKnout(emissaoCTe.Rodoviario, { Data: r.Data.Rodoviario });
                PreencherObjetoKnout(emissaoCTe.InformacaoCarga, { Data: r.Data.InformacaoCarga });
                PreencherObjetoKnout(emissaoCTe.ModalAereo, { Data: r.Data.ModalAereo });

                emissaoCTe.CRUDCTe.Salvar.visible(true);

                if (r.Data.Remetente != null) {
                    emissaoCTe.Remetente.NaoLimparDados = true;
                    PreencherObjetoKnout(emissaoCTe.Remetente, { Data: r.Data.Remetente });
                    emissaoCTe.Remetente.NaoLimparDados = false;
                }

                if (r.Data.Destinatario != null) {
                    emissaoCTe.Destinatario.NaoLimparDados = true;
                    PreencherObjetoKnout(emissaoCTe.Destinatario, { Data: r.Data.Destinatario });
                    emissaoCTe.Destinatario.NaoLimparDados = false;
                }

                if (r.Data.Expedidor != null) {
                    emissaoCTe.Expedidor.NaoLimparDados = true;
                    PreencherObjetoKnout(emissaoCTe.Expedidor, { Data: r.Data.Expedidor });
                    emissaoCTe.Expedidor.NaoLimparDados = false;
                }

                if (r.Data.Recebedor != null) {
                    emissaoCTe.Recebedor.NaoLimparDados = true;
                    PreencherObjetoKnout(emissaoCTe.Recebedor, { Data: r.Data.Recebedor });
                    emissaoCTe.Recebedor.NaoLimparDados = false;
                }

                if (r.Data.Tomador != null) {
                    emissaoCTe.Tomador.NaoLimparDados = true;
                    PreencherObjetoKnout(emissaoCTe.Tomador, { Data: r.Data.Tomador });
                    emissaoCTe.Tomador.NaoLimparDados = false;
                }

                if (r.Data.Documentos != null && r.Data.Documentos.length > 0) {
                    emissaoCTe.Documento.TipoDocumento.def = r.Data.Documentos[0].TipoDocumento;
                    emissaoCTe.Documento.TipoDocumento.val(r.Data.Documentos[0].TipoDocumento);
                    emissaoCTe.Documentos = r.Data.Documentos;
                    emissaoCTe.Documento.RecarregarGrid();
                }

                if (r.Data.QuantidadesCarga != null) {
                    emissaoCTe.QuantidadesCarga = r.Data.QuantidadesCarga;
                    emissaoCTe.QuantidadeCarga.RecarregarGrid();
                }

                if (r.Data.Seguros != null) {
                    emissaoCTe.Seguros = r.Data.Seguros;
                    emissaoCTe.Seguro.RecarregarGrid();
                }

                if (r.Data.TotalServico != null) {
                    PreencherObjetoKnout(emissaoCTe.TotalServico, { Data: r.Data.TotalServico });
                    TrocarICMS(emissaoCTe.TotalServico);
                }

                emissaoCTe.AlterarEstadoCTe(emissaoCTe);

            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }

            p.done();
        });

        return p;
    };

    this.FecharModal = function () {
        emissaoCTe.ModalCTe.hide();
    };

    this.Destroy = function () {
        emissaoCTe.ModalCTe.dispose();
        $("#" + emissaoCTe.IdModal).remove();
        emissaoCTe = null;
    };

    this.RenderizarModalCTe = function () {
        LoadLocalizationResources("CTes.CTe").then(function () {
            emissaoCTe.ObterHTMLEmissaoCTe().then(function () {
                emissaoCTe.ObterDadosGeraisControleCTe().then(function () {

                    var html = _HTMLEmissaoCTe.replace(/#divModalCTe/g, emissaoCTe.IdModal);
                    $('#js-page-content').append(html);
                    emissaoCTe.CRUDCTe.Cancelar.eventClick = function (e) {
                        emissaoCTe.FecharModal();
                    };

                    KoBindings(emissaoCTe.CTe, emissaoCTe.IdKnockoutCTe);
                    KoBindings(emissaoCTe.CRUDCTe, emissaoCTe.IdKnockoutCRUDCTe);

                    LocalizeCurrentPage();

                    new BuscarTransportadores(emissaoCTe.CTe.Empresa, function (data) {
                        emissaoCTe.ObterInformacoesEmpresa(data.Codigo);
                    }, null, true);

                    new BuscarClientes(emissaoCTe.CTe.Terceiro, function (data) {
                        emissaoCTe.ObterInformacoesTerceiro(data.Codigo);
                    }, false, [EnumModalidadePessoa.TransportadorTerceiro]);

                    new BuscarSeriesCTeTransportador(emissaoCTe.CTe.Serie, function (dados) {
                        emissaoCTe.CTe.Serie.codEntity(dados.Codigo);
                        emissaoCTe.CTe.Serie.val(dados.Numero);
                        emissaoCTe.SerieSetadaManualmente = true;
                    }, null, null, null, emissaoCTe.CTe.Empresa);

                    new BuscarLocalidades(emissaoCTe.CTe.LocalidadeEmissao);
                    new BuscarLocalidades(emissaoCTe.CTe.LocalidadeInicioPrestacao, null, null, function (r) {
                        emissaoCTe.CTe.LocalidadeInicioPrestacao.codEntity(r.Codigo);
                        emissaoCTe.CTe.LocalidadeInicioPrestacao.val(r.Descricao);
                        emissaoCTe.CTe.EstadoInicioPrestacao.val(r.Estado);
                    });

                    new BuscarLocalidades(emissaoCTe.CTe.LocalidadeTerminoPrestacao);

                    new BuscarCFOPs(emissaoCTe.CTe.CFOP, EnumTipoCFOP.Saida, function (cfop) {
                        emissaoCTe.CTe.CFOP.val(cfop.CodigoCFOP);
                        emissaoCTe.CTe.CFOP.codEntity(cfop.Codigo);
                        Global.setarFocoProximoCampo(emissaoCTe.CTe.CFOP.id);
                    });

                    if (!emissaoCTe.ModalCTe)
                        emissaoCTe.ModalCTe = new bootstrap.Modal(document.getElementById(emissaoCTe.IdModal), { backdrop: 'static', keyboard: true });

                    emissaoCTe.ModalCTe.show();

                    $('#' + emissaoCTe.IdModal).on('hidden.bs.modal', function () {
                        emissaoCTe.Destroy();
                    });

                    emissaoCTe.Documento.Load();
                    emissaoCTe.EntregaSimplificado.Load();
                    emissaoCTe.Remetente.Load();
                    emissaoCTe.Destinatario.Load();
                    emissaoCTe.Expedidor.Load();
                    emissaoCTe.Recebedor.Load();
                    emissaoCTe.Tomador.Load();
                    emissaoCTe.Rodoviario.Load();
                    emissaoCTe.Veiculo.Load();
                    emissaoCTe.Motorista.Load();
                    emissaoCTe.InformacaoCarga.Load();
                    emissaoCTe.QuantidadeCarga.Load();
                    emissaoCTe.Seguro.Load();
                    emissaoCTe.DocumentoTransporteAnteriorPapel.Load();
                    emissaoCTe.DocumentoTransporteAnteriorEletronico.Load();
                    emissaoCTe.ProdutoPerigoso.Load();
                    emissaoCTe.Duplicata.Load();
                    emissaoCTe.DuplicataAutomatica.Load();
                    emissaoCTe.ObservacaoGeral.Load();
                    emissaoCTe.ObservacaoFisco.Load();
                    emissaoCTe.ObservacaoContribuinte.Load();
                    emissaoCTe.TotalServico.Load();
                    emissaoCTe.Componente.Load();
                    emissaoCTe.CTeSubstituicao.Load();
                    emissaoCTe.CTeAnulacao.Load();
                    emissaoCTe.CTeComplementar.Load();

                    emissaoCTe.ModalDutoviario.Load();
                    emissaoCTe.ModalMultimodal.Load();
                    emissaoCTe.ModalFerroviario.Load();
                    emissaoCTe.ModalAquaviario.Load();
                    emissaoCTe.ModalAereo.Load();
                    emissaoCTe.ModalAereoManuseio.Load();
                    emissaoCTe.ModalAquaviarioBalsa.Load();
                    emissaoCTe.ModalAquaviarioContainer.Load();
                    emissaoCTe.ModalFerroviarioFerrovia.Load();
                    emissaoCTe.ModalAquaviarioContainerDocumento.Load();
                    emissaoCTe.InformacaoModal.Load();

                    if (adicionarCTeTerceiro) {
                        emissaoCTe.CTe.Empresa.visible(false);
                        emissaoCTe.CTe.Empresa.required = false;
                        emissaoCTe.CTe.Terceiro.visible(true);
                        emissaoCTe.CTe.Terceiro.required = true;

                        emissaoCTe.CTe.Numero.visible(false);
                        emissaoCTe.CTe.Numero.required = false;
                        emissaoCTe.CTe.NumeroTerceiro.visible(true);
                        emissaoCTe.CTe.NumeroTerceiro.required = true;

                        emissaoCTe.CTe.Serie.visible(false);
                        emissaoCTe.CTe.Serie.required = false;
                        emissaoCTe.CTe.SerieTerceiro.visible(true);
                        emissaoCTe.CTe.SerieTerceiro.required = true;

                        emissaoCTe.CTe.ChaveTerceiro.visible(true);
                        emissaoCTe.CTe.ChaveTerceiro.required = true;
                    }

                    emissaoCTe.CarregarCTe(duplicar).then(function () {
                        emissaoCTe.SetarPermissoes();

                        if (callbackInit != null)
                            callbackInit();

                        if (codigoDocumentoCTeTerceiro == 0)
                            emissaoCTe.SetarObtencaoImpostoAutomatico();
                    });

                    if (codigoCargaPedido > 0 && codigoDocumentoCTeTerceiro == 0) {
                        executarReST("CTe/ObterContainerCargaPedido", { CodigoCargaPedido: codigoCargaPedido }, function (r) {
                            if (r.Success) {
                                if (r.Data != null) {
                                    emissaoCTe.ModalAquaviarioContainer.Lacre1.val(r.Data.Lacre1);
                                    emissaoCTe.ModalAquaviarioContainer.Lacre2.val(r.Data.Lacre2);
                                    emissaoCTe.ModalAquaviarioContainer.Lacre3.val(r.Data.Lacre3);

                                    if (r.Data.CodigoContainer > 0) {
                                        emissaoCTe.ModalAquaviarioContainer.Container.codEntity(r.Data.CodigoContainer);
                                        emissaoCTe.ModalAquaviarioContainer.Container.val(r.Data.NumeroContainer);
                                        emissaoCTe.ModalAquaviarioContainer.AdicionarContainer();

                                        emissaoCTe.InformacaoCarga.Container.val(r.Data.NumeroContainer);
                                        var numeroLacres = "";
                                        if (r.Data.Lacre1 !== null && r.Data.Lacre1 !== undefined && r.Data.Lacre1 !== "")
                                            numeroLacres = r.Data.Lacre1;
                                        if (r.Data.Lacre2 !== null && r.Data.Lacre2 !== undefined && r.Data.Lacre2 !== "") {
                                            if (numeroLacres !== "")
                                                numeroLacres = numeroLacres + " / " + r.Data.Lacre2;
                                        }
                                        if (r.Data.Lacre3 !== null && r.Data.Lacre3 !== undefined && r.Data.Lacre3 !== "") {
                                            if (numeroLacres !== "")
                                                numeroLacres = numeroLacres + " / " + r.Data.Lacre3;
                                        }
                                        emissaoCTe.InformacaoCarga.NumeroLacreContainer.val(numeroLacres);
                                    }
                                }
                            } else {
                                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                            }
                        });
                    }
                });
            });
        });
    };

    this.SetarObtencaoImpostoAutomatico = function () {
        emissaoCTe.CTe.LocalidadeInicioPrestacao.codEntity.subscribe(function () {
            emissaoCTe.ObterImpostoAutomatico();
        });

        emissaoCTe.CTe.LocalidadeTerminoPrestacao.codEntity.subscribe(function () {
            emissaoCTe.ObterImpostoAutomatico();
        });

        emissaoCTe.CTe.Empresa.codEntity.subscribe(function () {
            emissaoCTe.ObterImpostoAutomatico();
        });

        emissaoCTe.CTe.TipoTomador.val.subscribe(function () {
            emissaoCTe.ObterImpostoAutomatico();
        });

        emissaoCTe.Remetente.Atividade.codEntity.subscribe(function () {
            emissaoCTe.ObterImpostoAutomatico();
        });

        emissaoCTe.Destinatario.Atividade.codEntity.subscribe(function () {
            emissaoCTe.ObterImpostoAutomatico();
        });

        emissaoCTe.Expedidor.Atividade.codEntity.subscribe(function () {
            emissaoCTe.ObterImpostoAutomatico();
        });

        emissaoCTe.Recebedor.Atividade.codEntity.subscribe(function () {
            emissaoCTe.ObterImpostoAutomatico();
        });

        emissaoCTe.Tomador.Atividade.codEntity.subscribe(function () {
            emissaoCTe.ObterImpostoAutomatico();
        });
    };

    this.ObterImpostoAutomatico = function () {

        var filtrosImpostoAutomatico = {
            LocalidadeInicioPrestacao: emissaoCTe.CTe.LocalidadeInicioPrestacao.codEntity(),
            LocalidadeTerminoPrestacao: emissaoCTe.CTe.LocalidadeTerminoPrestacao.codEntity(),
            Empresa: emissaoCTe.CTe.Empresa.codEntity(),
            AtividadeRemetente: emissaoCTe.Remetente.Atividade.codEntity(),
            AtividadeDestinatario: emissaoCTe.Destinatario.Atividade.codEntity(),
            AtividadeTomador: emissaoCTe.ObterTomador().Atividade.codEntity()
        };

        if (emissaoCTe.FiltrosImpostoAutomatico != null &&
            emissaoCTe.FiltrosImpostoAutomatico.LocalidadeInicioPrestacao == filtrosImpostoAutomatico.LocalidadeInicioPrestacao &&
            emissaoCTe.FiltrosImpostoAutomatico.LocalidadeTerminoPrestacao == filtrosImpostoAutomatico.LocalidadeTerminoPrestacao &&
            emissaoCTe.FiltrosImpostoAutomatico.Empresa == filtrosImpostoAutomatico.Empresa &&
            emissaoCTe.FiltrosImpostoAutomatico.AtividadeRemetente == filtrosImpostoAutomatico.AtividadeRemetente &&
            emissaoCTe.FiltrosImpostoAutomatico.AtividadeDestinatario == filtrosImpostoAutomatico.AtividadeDestinatario &&
            emissaoCTe.FiltrosImpostoAutomatico.AtividadeTomador == filtrosImpostoAutomatico.AtividadeTomador) {
            return; //se todos os dados forem iguais, já foi pesquisado, então não realiza a pesquisa novamente
        }

        if (filtrosImpostoAutomatico.LocalidadeInicioPrestacao > 0 &&
            filtrosImpostoAutomatico.LocalidadeTerminoPrestacao > 0 &&
            filtrosImpostoAutomatico.Empresa > 0 &&
            filtrosImpostoAutomatico.AtividadeRemetente > 0 &&
            filtrosImpostoAutomatico.AtividadeDestinatario > 0 &&
            filtrosImpostoAutomatico.AtividadeTomador > 0) {
            executarReST("CTe/ObterInformacoesImpostos", filtrosImpostoAutomatico, function (r) {
                if (r.Success) {
                    if (r.Data !== false) {
                        if (r.Data != null) {
                            emissaoCTe.CTe.CFOP.val(r.Data.CFOP.Numero);
                            emissaoCTe.CTe.CFOP.codEntity(r.Data.CFOP.Codigo);
                            emissaoCTe.TotalServico.ICMS.val(r.Data.CST);
                            emissaoCTe.TotalServico.AliquotaICMS.val(r.Data.Aliquota);

                            emissaoCTe.TotalServico.CalcularICMS();
                        }
                        emissaoCTe.FiltrosImpostoAutomatico = filtrosImpostoAutomatico;
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                }
            });
        }
    };

    this.ObterTomador = function () {
        switch (emissaoCTe.CTe.TipoTomador.val()) {
            case EnumTipoTomador.Destinatario:
                return emissaoCTe.Destinatario;
            case EnumTipoTomador.Expedidor:
                return emissaoCTe.Expedidor;
            case EnumTipoTomador.Outros:
                return emissaoCTe.Tomador;
            case EnumTipoTomador.Recebedor:
                return emissaoCTe.Recebedor;
            case EnumTipoTomador.Remetente:
                return emissaoCTe.Remetente;
            default:
                return null;
        }
    };

    this.CarregarCTe = function () {
        var p = new promise.Promise();

        if (emissaoCTe.CTe.Codigo.val() != "" && emissaoCTe.CTe.Codigo.val() > 0) {
            emissaoCTe.BuscarDadosCTe(duplicar).then(function () {
                p.done();
            });
        } else if (codigoDocumentoCTeTerceiro > 0) {
            emissaoCTe.BuscarDadosCTeTerceiro().then(function () {
                p.done();
            });
        } else {
            p.done();
        }

        return p;
    };

    this.ObterInformacoesEmpresa = function (codigoEmpresa) {
        executarReST("CTe/ObterInformacoesEmpresa", { Empresa: codigoEmpresa }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    emissaoCTe.Empresa = r.Data;
                    emissaoCTe.CTe.Empresa.val("(" + r.Data.CNPJ + ") " + r.Data.NomeFantasia);
                    emissaoCTe.CTe.Empresa.codEntity(r.Data.Codigo);
                    emissaoCTe.CTe.LocalidadeEmissao.val(r.Data.Localidade.Descricao);
                    emissaoCTe.CTe.LocalidadeEmissao.codEntity(r.Data.Localidade.Codigo);
                    emissaoCTe.Rodoviario.RNTRC.val(r.Data.RNTRC);
                    emissaoCTe.Rodoviario.PrevisaoEntrega.val(Global.Data(EnumTipoOperacaoDate.Add, (r.Data.DiasParaEntrega > 0 ? r.Data.DiasParaEntrega : 1), EnumTipoOperacaoObjetoDate.Days));
                    if (r.Data.ProdutoPredominante !== null && r.Data.ProdutoPredominante !== undefined && r.Data.ProdutoPredominante != "")
                        emissaoCTe.InformacaoCarga.ProdutoPredominante.val(r.Data.ProdutoPredominante);
                    emissaoCTe.ModalMultimodal.NumeroCOTM.val(r.Data.NumeroCOTM);

                    if (_CONFIGURACAO_TMS.PermitirAlterarEmpresaNoCTeManual) {
                        emissaoCTe.CTe.Serie.codEntity(0);
                        emissaoCTe.CTe.Serie.val("");
                    }
                    emissaoCTe.SetarSerie();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    };

    this.ObterInformacoesTerceiro = function (codigoTerceiro) {
        executarReST("CTe/ObterInformacoesTerceiro", { Codigo: codigoTerceiro }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    emissaoCTe.Terceiro = r.Data;
                    emissaoCTe.CTe.Terceiro.val("(" + r.Data.CNPJ + ") " + r.Data.NomeFantasia);
                    emissaoCTe.CTe.Terceiro.codEntity(r.Data.Codigo);
                    emissaoCTe.CTe.LocalidadeEmissao.val(r.Data.Localidade.Descricao);
                    emissaoCTe.CTe.LocalidadeEmissao.codEntity(r.Data.Localidade.Codigo);
                    emissaoCTe.Rodoviario.RNTRC.val(r.Data.RNTRC);
                    emissaoCTe.Rodoviario.PrevisaoEntrega.val(Global.Data(EnumTipoOperacaoDate.Add, (r.Data.DiasParaEntrega > 0 ? r.Data.DiasParaEntrega : 1), EnumTipoOperacaoObjetoDate.Days));
                    if (r.Data.ProdutoPredominante !== null && r.Data.ProdutoPredominante !== undefined && r.Data.ProdutoPredominante != "")
                        emissaoCTe.InformacaoCarga.ProdutoPredominante.val(r.Data.ProdutoPredominante);

                    emissaoCTe.SetarSerie();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    };

    this.ObterInformacoesModalRodoviario = function (codigosVeiculos, codigosMotoristas) {
        if ((codigosVeiculos != null && codigosVeiculos.length > 0) || (codigosMotoristas != null && codigosMotoristas.length > 0)) {
            executarReST("CTe/ObterInformacoesModalRodoviario", { Veiculos: JSON.stringify(codigosVeiculos), Motoristas: JSON.stringify(codigosMotoristas) }, function (r) {
                if (r.Success) {
                    if (r.Data) {

                        for (var i = 0; i < r.Data.Motoristas.length; i++)
                            emissaoCTe.Motoristas.push(r.Data.Motoristas[i]);
                        emissaoCTe.Motorista.RecarregarGrid();

                        for (var j = 0; j < r.Data.Veiculos.length; j++)
                            emissaoCTe.Veiculos.push(r.Data.Veiculos[j]);
                        emissaoCTe.Veiculo.RecarregarGrid();

                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                }
            });
        }
    };

    this.SetarSerie = function () {
        if (_CONFIGURACAO_TMS.NaoPreencherSerieCTeManual)
            return;
        if (emissaoCTe.SerieSetadaManualmente !== true && emissaoCTe.Empresa != null) {
            if (emissaoCTe.Remetente.Participante != null) {
                if (emissaoCTe.Remetente.Participante.Localidade.Estado != emissaoCTe.Empresa.Localidade.Estado) {
                    if (emissaoCTe.Empresa.SerieInterestadual != null && emissaoCTe.Empresa.SerieInterestadual.Codigo > 0) {
                        emissaoCTe.CTe.Serie.val(emissaoCTe.Empresa.SerieInterestadual.Numero);
                        emissaoCTe.CTe.Serie.codEntity(emissaoCTe.Empresa.SerieInterestadual.Codigo);
                    }
                } else {
                    if (emissaoCTe.Empresa.SerieIntraestadual != null && emissaoCTe.Empresa.SerieIntraestadual.Codigo > 0) {
                        emissaoCTe.CTe.Serie.val(emissaoCTe.Empresa.SerieIntraestadual.Numero);
                        emissaoCTe.CTe.Serie.codEntity(emissaoCTe.Empresa.SerieIntraestadual.Codigo);
                    }
                }
            }
            
            if (emissaoCTe.CTe.Serie.codEntity() <= 0) {
                emissaoCTe.CTe.Serie.val(emissaoCTe.Empresa.Serie.Numero);
                emissaoCTe.CTe.Serie.codEntity(emissaoCTe.Empresa.Serie.Codigo);
            }
        }
    };

    this.ObterHTMLEmissaoCTe = function () {
        var p = new promise.Promise();
        if (_HTMLEmissaoCTe == "") {
            $.get("Content/Static/CTe/CTe.html?dyn=" + emissaoCTe.IdModal, function (data) {
                _HTMLEmissaoCTe = data;
                p.done();
            });
        } else {
            p.done();
        }
        return p;
    };

    this.ObterDadosGeraisControleCTe = function () {
        var p = new promise.Promise();
        if (emissaoCTe.DadosGeraisControleCTe == null) {
            executarReST("CTe/ObterDadosGeraisControleCTe", {}, function (r) {
                if (r.Success) {
                    if (r.Data) {
                        emissaoCTe.DadosGeraisControleCTe = r.Data;
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                }
                p.done();
            });

        } else {
            p.done();
        }
        return p;
    };

    this.AlterarEstadoCTe = function () {
        if (emissaoCTe.CTe.Status.val() === EnumStatusCTe.AUTORIZADO ||
            emissaoCTe.CTe.Status.val() === EnumStatusCTe.CANCELADO ||
            emissaoCTe.CTe.Status.val() === EnumStatusCTe.EMCANCELAMENTO) {
            emissaoCTe.CTe.ProtocoloAutorizacao.visible(true);
            emissaoCTe.CTe.Chave.visible(true);
        }

        if (emissaoCTe.CTe.Status.val() === EnumStatusCTe.CANCELADO) {
            emissaoCTe.CTe.ProtocoloCancelamentoInutilizacao.text(Localization.Resources.CTes.CTe.ProtocoloDeCancelamento.getFieldDescription());
            emissaoCTe.CTe.ProtocoloCancelamentoInutilizacao.visible(true);
            emissaoCTe.CTe.JustificativaCancelamento.visible(true);
        } else if (emissaoCTe.CTe.Status.val() === EnumStatusCTe.INUTILIZADO) {
            emissaoCTe.CTe.ProtocoloCancelamentoInutilizacao.text(Localization.Resources.CTes.CTe.ProtocoloDeInutilizacao.getFieldDescription());
            emissaoCTe.CTe.ProtocoloCancelamentoInutilizacao.visible(true);
        }
    };

    this.VerificarSePossuiPermissao = function (permissao) {
        var existe = false;
        $.each(permissoes, function (i, permissaoDaLista) {
            if (permissao == permissaoDaLista) {
                existe = true;
                return false;
            }
        });
        return existe;
    };

    this.Validar = function () {
        var valido = ValidarCamposObrigatorios(emissaoCTe.CTe);

        if (!valido) {
            $('a[href="#divDetalhes_' + emissaoCTe.IdModal + '"]').tab("show");
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.CTes.CTe.VerifiqueOsCamposObrigatorios);
            return false;
        } else {
            if (emissaoCTe.Documento.Validar() &&
                emissaoCTe.EntregaSimplificado.Validar() &&
                emissaoCTe.Remetente.Validar(true) &&
                emissaoCTe.Destinatario.Validar(true) &&
                emissaoCTe.Expedidor.Validar(emissaoCTe.CTe.TipoTomador.val() === EnumTipoTomador.Expedidor) &&
                emissaoCTe.Recebedor.Validar(emissaoCTe.CTe.TipoTomador.val() === EnumTipoTomador.Recebedor) &&
                emissaoCTe.Tomador.Validar(emissaoCTe.CTe.TipoTomador.val() === EnumTipoTomador.Outros) &&
                emissaoCTe.Rodoviario.Validar() &&
                emissaoCTe.ModalAereo.Validar() &&
                emissaoCTe.ModalAquaviario.Validar() &&
                emissaoCTe.ModalFerroviario.Validar() &&
                emissaoCTe.ModalDutoviario.Validar() &&
                emissaoCTe.ModalMultimodal.Validar() &&
                emissaoCTe.Veiculo.Validar() &&
                emissaoCTe.Motorista.Validar() &&
                emissaoCTe.InformacaoCarga.Validar() &&
                emissaoCTe.QuantidadeCarga.Validar() &&
                emissaoCTe.Seguro.Validar() &&
                emissaoCTe.CTeSubstituicao.Validar() &&
                emissaoCTe.CTeAnulacao.Validar() &&
                emissaoCTe.CTeComplementar.Validar()) {
                return true;
            } else {
                return false;
            }
        }
    };

    if (_tipoAliquotaICMSCTe == null || _tipoAliquotaICMSCTe.length <= 0)
        ObterAliquotasICMS().then(function () { emissaoCTe.LoadEmissaoCTe(); });
    else {
        setTimeout(function () {
            emissaoCTe.LoadEmissaoCTe();
        }, 50);
    }
}

function DesabilitarCamposInstanciasCTe(instancia) {
    $.each(instancia, function (i, knout) {
        if (knout.enable != null) {
            if (knout.enable === true || knout.enable === false)
                knout.enable = false;
            else
                knout.enable(false);
        }
    });
}

function ObterObjetoCTe(emissaoCTe) {
    var cte = new Object();
    cte.CTe = RetornarObjetoPesquisa(emissaoCTe.CTe);
    cte.Documentos = emissaoCTe.Documentos;
    cte.EntregasSimplificado = emissaoCTe.EntregasSimplificado;
    cte.EntregasSimplificadoDocumentos = emissaoCTe.EntregasSimplificadoDocumentos;
    cte.EntregasSimplificadoDocumentosTransporteAnterior = emissaoCTe.EntregasSimplificadoDocumentosTransporteAnterior;
    cte.EntregasSimplificadoComponentesPrestacaoServico = emissaoCTe.EntregasSimplificadoComponentesPrestacaoServico;
    cte.Remetente = RetornarObjetoPesquisa(emissaoCTe.Remetente);
    cte.Destinatario = RetornarObjetoPesquisa(emissaoCTe.Destinatario);
    cte.Expedidor = RetornarObjetoPesquisa(emissaoCTe.Expedidor);
    cte.Recebedor = RetornarObjetoPesquisa(emissaoCTe.Recebedor);
    cte.Tomador = RetornarObjetoPesquisa(emissaoCTe.Tomador);
    cte.Rodoviario = RetornarObjetoPesquisa(emissaoCTe.Rodoviario);
    cte.Veiculos = emissaoCTe.Veiculos;
    cte.Motoristas = emissaoCTe.Motoristas;
    cte.InformacaoCarga = RetornarObjetoPesquisa(emissaoCTe.InformacaoCarga);
    cte.QuantidadesCarga = emissaoCTe.QuantidadesCarga;
    cte.Seguros = emissaoCTe.Seguros;
    cte.DocumentosTransporteAnteriorPapel = emissaoCTe.DocumentosTransporteAnteriorPapel;
    cte.DocumentosTransporteAnteriorEletronico = emissaoCTe.DocumentosTransporteAnteriorEletronico;
    cte.ProdutosPerigosos = emissaoCTe.ProdutosPerigosos;
    cte.Duplicata = RetornarObjetoPesquisa(emissaoCTe.Duplicata);
    cte.DuplicataAutomatica = RetornarObjetoPesquisa(emissaoCTe.DuplicataAutomatica);

    cte.ObservacaoGeral = RetornarObjetoPesquisa(emissaoCTe.ObservacaoGeral);
    cte.ObservacoesFisco = emissaoCTe.ObservacaoFisco.Lista;
    cte.ObservacoesContribuinte = emissaoCTe.ObservacaoContribuinte.Lista;

    cte.TotalServico = RetornarObjetoPesquisa(emissaoCTe.TotalServico);
    cte.Componentes = emissaoCTe.Componentes;

    cte.ModalAereo = RetornarObjetoPesquisa(emissaoCTe.ModalAereo);
    cte.ModalAquaviario = RetornarObjetoPesquisa(emissaoCTe.ModalAquaviario);
    cte.ModalFerroviario = RetornarObjetoPesquisa(emissaoCTe.ModalFerroviario);
    cte.ModalDutoviario = RetornarObjetoPesquisa(emissaoCTe.ModalDutoviario);
    cte.ModalMultimodal = RetornarObjetoPesquisa(emissaoCTe.ModalMultimodal);
    cte.Manuseios = emissaoCTe.Manuseios;
    cte.Balsas = emissaoCTe.Balsas;
    cte.Containers = emissaoCTe.Containers;
    cte.ContainerDocumentos = emissaoCTe.ContainerDocumentos;
    cte.Ferrovias = emissaoCTe.Ferrovias;
    cte.InformacaoModal = RetornarObjetoPesquisa(emissaoCTe.InformacaoModal);
    cte.CTeSubstituicao = RetornarObjetoPesquisa(emissaoCTe.CTeSubstituicao);
    cte.CTeAnulacao = RetornarObjetoPesquisa(emissaoCTe.CTeAnulacao);
    cte.CTeComplementar = RetornarObjetoPesquisa(emissaoCTe.CTeComplementar);

    return JSON.stringify(cte);
}