/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/CFOP.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/Servico.js" />
/// <reference path="../../Consultas/SerieEmpresa.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Atividade.js" />
/// <reference path="../../Consultas/NaturezaOperacao.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoNFe.js" />
/// <reference path="../../Enumeradores/EnumFinalidadeNFe.js" />
/// <reference path="../../Enumeradores/EnumIndicadorPresencaNFe.js" />
/// <reference path="../../Enumeradores/EnumIndicadorIntermediadorNFe.js" />
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
/// <reference path="ImpostoCOFINS.js" />
/// <reference path="ImpostoICMS.js" />
/// <reference path="ImpostoICMSMonoRet.js" />
/// <reference path="ImpostoICMSDesonerado.js" />
/// <reference path="ImpostoICMSST.js" />
/// <reference path="ImpostoDIFAL.js" />
/// <reference path="ImpostoII.js" />
/// <reference path="ImpostoIPI.js" />
/// <reference path="ImpostoISS.js" />
/// <reference path="ImpostoPIS.js" />
/// <reference path="ListaProdutoServico.js" />
/// <reference path="Observacao.js" />
/// <reference path="Parcelamento.js" />
/// <reference path="RetiradaEntrega.js" />
/// <reference path="ProdutoServico.js" />
/// <reference path="Referencia.js" />
/// <reference path="ExportacaoCompra.js" />
/// <reference path="Totalizador.js" />
/// <reference path="Transporte.js" />
/// <reference path="InformacoesAdicionaisProdutoServico.js" />
/// <reference path="LoteProduto.js" />
/// <reference path="CombustivelProduto.js" />
/// <reference path="ExportacaoProdutoServico.js" />

var _statusNFe = [
    { value: 1, text: "Emitido" },
    { value: 2, text: "Inutilizado" },
    { value: 3, text: "Cancelado" },
    { value: 4, text: "Autorizado" },
    { value: 5, text: "Denegado" }
];

var _HTMLEmissaoNFe = "";

var PrincipalNFe = function () {
    var $this = this;

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, enable: ko.observable(true) });
    this.CasasQuantidadeProdutoNFe = PropertyEntity({ getType: typesKnockout.int, enable: ko.observable(true), val: ko.observable(4), def: ko.observable(4) });
    this.CasasValorProdutoNFe = PropertyEntity({ getType: typesKnockout.int, enable: ko.observable(true), val: ko.observable(5), def: ko.observable(5) });
    this.CodigoPedidoVenda = PropertyEntity({ getType: typesKnockout.string, enable: ko.observable(true), val: ko.observable("") });
    this.CodigoDocumentoEntrada = PropertyEntity({ getType: typesKnockout.string, enable: ko.observable(true), val: ko.observable("") });
    this.SubtraiDescontoBaseICMS = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false) });
    this.IdModal = PropertyEntity({ getType: typesKnockout.string, enable: ko.observable(true), val: ko.observable("") });

    this.Numero = PropertyEntity({ getType: typesKnockout.int, text: "*Número:", enable: ko.observable(false) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.Serie = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Série:", idBtnSearch: guid(), required: ko.observable(true), enable: ko.observable(true) });
    this.TipoEmissao = PropertyEntity({ val: ko.observable(EnumTipoEmissaoNFe.Saida), def: EnumTipoEmissaoNFe.Saida, options: EnumTipoEmissaoNFe.obterOpcoes(), text: "*Tipo Emissão:", required: true, enable: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.dateTime, text: "*Data de Emissão:", required: true, enable: ko.observable(true) });
    this.DataSaida = PropertyEntity({ getType: typesKnockout.dateTime, text: "Data de Saída:", required: false, enable: ko.observable(true) });
    this.Chave = PropertyEntity({ getType: typesKnockout.string, text: "Chave:", enable: ko.observable(false), visible: ko.observable(true) });
    this.Status = PropertyEntity({ val: ko.observable(1), def: 1, options: _statusNFe, text: "Status:", required: false, enable: ko.observable(false) });
    this.Protocolo = PropertyEntity({ getType: typesKnockout.string, text: "Protocolo:", enable: ko.observable(false), visible: ko.observable(true) });
    this.DataProcessamento = PropertyEntity({ getType: typesKnockout.dateTime, text: "Data de Processamento:", required: false, enable: ko.observable(false) });
    this.DataPrestacaoServico = PropertyEntity({ getType: typesKnockout.dateTime, text: "*Data de Prestação do Serviço:", required: true, enable: ko.observable(true) });
    this.Finalidade = PropertyEntity({ val: ko.observable(EnumFinalidadeNFe.Normal), text: "*Finalidade:", options: EnumFinalidadeNFe.obterOpcoes(), def: EnumFinalidadeNFe.Normal, required: true, enable: ko.observable(true) });
    this.IndicadorPresenca = PropertyEntity({ val: ko.observable(EnumIndicadorPresencaNFe.NaoSeAplica), text: "*Indicador de Presença:", options: EnumIndicadorPresencaNFe.obterOpcoes(), def: EnumIndicadorPresencaNFe.NaoSeAplica, required: true, enable: ko.observable(true) });
    this.IndicadorIntermediador = PropertyEntity({ val: ko.observable(EnumIndicadorIntermediadorNFe.SemIntermediador), text: "Indicador de Intermediador:", options: EnumIndicadorIntermediadorNFe.obterOpcoesCadastro(), def: EnumIndicadorIntermediadorNFe.SemIntermediador, required: ko.observable(false), enable: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pessoa:", idBtnSearch: guid(), required: true, enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-12 col-lg-12") });
    this.Intermediador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Intermediador da Transação:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.NaturezaOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Natureza da Operação:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Atividade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Atividade:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.CidadePrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cidade Prestação do Serviço:", idBtnSearch: guid(), required: false, enable: ko.observable(true) });

    this.IndicadorPresenca.val.subscribe(function (novoValor) {
        if (novoValor === EnumIndicadorPresencaNFe.NaoSeAplica || novoValor === EnumIndicadorPresencaNFe.PresencialForaEmpresa)
            $this.IndicadorIntermediador.val(EnumIndicadorIntermediadorNFe.Todos);
        else
            $this.IndicadorIntermediador.val(EnumIndicadorIntermediadorNFe.SemIntermediador);
    });

    this.IndicadorIntermediador.val.subscribe(function (novoValor) {
        if (novoValor === EnumIndicadorIntermediadorNFe.SitePlataformaTerceiros) {
            $this.Pessoa.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-6");
            $this.Intermediador.required(true);
            $this.Intermediador.visible(true);
        } else {
            $this.Pessoa.cssClass("col col-xs-12 col-sm-12 col-md-12 col-lg-12");
            $this.Intermediador.required(false);
            $this.Intermediador.visible(false);
            LimparCampoEntity($this.Intermediador);
        }
    });
};

var CRUDNFe = function () {
    this.Emitir = PropertyEntity({ type: types.event, text: "Autorizar Emissão", visible: ko.observable(true), enable: ko.observable(true) });
    this.Salvar = PropertyEntity({ type: types.event, text: "Salvar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, text: "Cancelar", visible: ko.observable(true), enable: ko.observable(true) });
};

function EmissaoNFe(codigoNFe, callbackInit, permissoes, codigosPedidoVenda, codigosDocumentoEntrada) {

    var emissaoNFe = this;

    this.LoadEmissaoNFe = function () {
        emissaoNFe.IdModal = guid();
        emissaoNFe.IdKnockoutNFe = "knockoutNFe_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutCRUDNFe = "knockoutCRUDNFe_" + emissaoNFe.IdModal;

        emissaoNFe.IdKnockoutProdutoServico = "knockoutProdutoServico_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutListaProdutoServico = "knockoutListaProdutoServico_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutImpostoICMSMonoRet = "knockoutImpostoICMSMonoRet_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutImpostoICMS = "knockoutImpostoICMS_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutImpostoICMSST = "knockoutImpostoICMSST_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutImpostoDIFAL = "knockoutImpostoDIFAL_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutImpostoPIS = "knockoutImpostoPIS_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutImpostoCOFINS = "knockoutImpostoCOFINS_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutImpostoIPI = "knockoutImpostoIPI_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutImpostoISS = "knockoutImpostoISS_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutImpostoII = "knockoutImpostoII_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutImpostoICMSDesonerado = "knockoutImpostoICMSDesonerado_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutInformacoesAdicionaisProdutoServico = "knockoutInformacoesAdicionais_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutLotes = "knockoutLotes_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutCombustivelProduto = "knockoutCombustivel_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutExportacaoProdutoServico = "knockoutExportacao_" + emissaoNFe.IdModal;

        emissaoNFe.IdKnockoutTotalizador = "knockoutTotalizador_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutObservacao = "knockoutObservacao_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutExportacaoCompra = "knockoutExportacaoCompra_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutTransporte = "knockoutTransporte_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutReferencia = "knockoutReferencia_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutParcelamento = "knockoutParcelamento_" + emissaoNFe.IdModal;
        emissaoNFe.IdKnockoutRetiradaEntrega = "knockoutRetiradaEntrega_" + emissaoNFe.IdModal;

        emissaoNFe.NFe = new PrincipalNFe();
        emissaoNFe.NFe.IdModal.val(emissaoNFe.IdModal);
        emissaoNFe.NFe.DataEmissao.val(moment().format("DD/MM/YYYY HH:mm"));
        emissaoNFe.NFe.DataSaida.val(moment().format("DD/MM/YYYY HH:mm"));
        emissaoNFe.NFe.DataPrestacaoServico.val(moment().format("DD/MM/YYYY HH:mm"));

        emissaoNFe.ListaProdutoServico = new ListaProdutoServico(emissaoNFe);
        emissaoNFe.ImpostoICMSMonoRet = new ImpostoICMSMonoRet(emissaoNFe);
        emissaoNFe.ImpostoICMS = new ImpostoICMS(emissaoNFe);
        emissaoNFe.ImpostoCOFINS = new ImpostoCOFINS(emissaoNFe);
        emissaoNFe.ImpostoIPI = new ImpostoIPI(emissaoNFe);
        emissaoNFe.ImpostoPIS = new ImpostoPIS(emissaoNFe, emissaoNFe.ImpostoCOFINS, emissaoNFe.ImpostoIPI);
        emissaoNFe.ImpostoISS = new ImpostoISS(emissaoNFe);
        emissaoNFe.ImpostoII = new ImpostoII(emissaoNFe);
        emissaoNFe.ImpostoICMSDesonerado = new ImpostoICMSDesonerado(emissaoNFe);
        emissaoNFe.ImpostoICMSST = new ImpostoICMSST(emissaoNFe, emissaoNFe.ImpostoIPI, emissaoNFe.ListaProdutoServico);
        emissaoNFe.ImpostoDIFAL = new ImpostoDIFAL(emissaoNFe);
        //emissaoNFe.InformacoesAdicionaisProdutoServico = new InformacoesAdicionaisProdutoServico(emissaoNFe);
        emissaoNFe.LoteProduto = new LoteProduto(emissaoNFe);
        emissaoNFe.CombustivelProduto = new CombustivelProduto(emissaoNFe);
        emissaoNFe.ExportacaoProdutoServico = new ExportacaoProdutoServico(emissaoNFe);

        emissaoNFe.Totalizador = new Totalizador(emissaoNFe);
        emissaoNFe.Observacao = new Observacao(emissaoNFe);
        emissaoNFe.ExportacaoCompra = new ExportacaoCompra(emissaoNFe);
        emissaoNFe.Transporte = new Transporte(emissaoNFe);
        emissaoNFe.Referencia = new Referencia(emissaoNFe);
        emissaoNFe.DetalheParcela = new DetalheParcela(emissaoNFe);
        emissaoNFe.Parcelamento = new Parcelamento(emissaoNFe, emissaoNFe.Totalizador, emissaoNFe.DetalheParcela);
        emissaoNFe.RetiradaEntrega = new RetiradaEntrega(emissaoNFe);

        emissaoNFe.CRUDNFe = new CRUDNFe();

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
            emissaoNFe.NFe.Empresa.visible(true);
            emissaoNFe.NFe.Empresa.required(true);
        }

        if (codigoNFe != null && codigoNFe > 0)
            emissaoNFe.NFe.Codigo.val(codigoNFe);

        emissaoNFe.RenderizarModalNFe(emissaoNFe, callbackInit);
        emissaoNFe.NFe.TipoEmissao.eventChange = emissaoNFe.ImpostoPIS.TipoEmissaoChange;
    };

    this.SetarPermissoes = function () {

        if (permissoes != null) {
            //Desenvolver
        }
    };

    this.DestivarNFe = function () {
        DesabilitarCamposInstanciasNFe(emissaoNFe.NFe);
    };

    this.BuscarDadosPadroesEmpresa = function () {
        executarReST("NotaFiscalEletronica/BuscarDadosEmpresa", null, function (r) {
            if (r.Success) {
                emissaoNFe.NFe.CasasQuantidadeProdutoNFe.val(r.Data.CasasQuantidadeProdutoNFe);
                emissaoNFe.NFe.CasasValorProdutoNFe.val(r.Data.CasasValorProdutoNFe);
                emissaoNFe.NFe.SubtraiDescontoBaseICMS.val(r.Data.SubtraiDescontoBaseICMS);

                if (codigoNFe != null && codigoNFe > 0)
                    emissaoNFe.NFe.Codigo.val(codigoNFe);
                else {
                    if (r.Data.Serie != null) {
                        emissaoNFe.NFe.Serie.codEntity(r.Data.Serie.Codigo);
                        emissaoNFe.NFe.Serie.val(r.Data.Serie.Descricao);
                    }
                    if (r.Data.Empresa != null) {
                        emissaoNFe.NFe.Empresa.codEntity(r.Data.Empresa.Codigo);
                        emissaoNFe.NFe.Empresa.val(r.Data.Empresa.Descricao);
                    }
                    if (r.Data.CidadeEmpresa != null) {
                        emissaoNFe.NFe.CidadePrestacao.codEntity(r.Data.CidadeEmpresa.Codigo);
                        emissaoNFe.NFe.CidadePrestacao.val(r.Data.CidadeEmpresa.Descricao);
                    }
                    if (r.Data.ProximoNumero > 0 && r.Data.Serie != null) {
                        emissaoNFe.NFe.Numero.val(r.Data.ProximoNumero);
                    }
                }

                emissaoNFe.InformacoesAdicionaisProdutoServico = new InformacoesAdicionaisProdutoServico(emissaoNFe);
                emissaoNFe.InformacoesAdicionaisProdutoServico.Load();
                emissaoNFe.ProdutoServico = new ProdutoServico(emissaoNFe, emissaoNFe.ListaProdutoServico, emissaoNFe.ImpostoICMS, emissaoNFe.ImpostoICMSST, emissaoNFe.ImpostoPIS,
                    emissaoNFe.ImpostoCOFINS, emissaoNFe.ImpostoII, emissaoNFe.ImpostoIPI, emissaoNFe.ImpostoISS, emissaoNFe.ImpostoICMSDesonerado, emissaoNFe.ImpostoDIFAL,
                    emissaoNFe.InformacoesAdicionaisProdutoServico, emissaoNFe.LoteProduto, emissaoNFe.CombustivelProduto, emissaoNFe.ExportacaoProdutoServico, emissaoNFe.ImpostoICMSMonoRet);
                emissaoNFe.ProdutoServico.Load();
                if (r.Data.EmpresaSimples)
                    emissaoNFe.ProdutoServico.CSTICMS.options(_cstICMSSimples);
                else
                    emissaoNFe.ProdutoServico.CSTICMS.options(_cstICMSNormal);

                if (r.Data.CasasQuantidadeProdutoNFe === 0) {
                    emissaoNFe.ProdutoServico.Quantidade.def = "0";
                    emissaoNFe.ProdutoServico.Quantidade.val("0");
                    emissaoNFe.ProdutoServico.Quantidade.configDecimal = { precision: 0, allowZero: true };

                    emissaoNFe.InformacoesAdicionaisProdutoServico.QuantidadeTributavel.def = "0";
                    emissaoNFe.InformacoesAdicionaisProdutoServico.QuantidadeTributavel.val("0");
                    emissaoNFe.InformacoesAdicionaisProdutoServico.QuantidadeTributavel.configDecimal = { precision: 0, allowZero: true };
                } else if (r.Data.CasasQuantidadeProdutoNFe === 1) {
                    emissaoNFe.ProdutoServico.Quantidade.def = "0,0";
                    emissaoNFe.ProdutoServico.Quantidade.val("0,0");
                    emissaoNFe.ProdutoServico.Quantidade.configDecimal = { precision: 1, allowZero: true };

                    emissaoNFe.InformacoesAdicionaisProdutoServico.QuantidadeTributavel.def = "0,0";
                    emissaoNFe.InformacoesAdicionaisProdutoServico.QuantidadeTributavel.val("0,0");
                    emissaoNFe.InformacoesAdicionaisProdutoServico.QuantidadeTributavel.configDecimal = { precision: 1, allowZero: true };
                } else if (r.Data.CasasQuantidadeProdutoNFe === 2) {
                    emissaoNFe.ProdutoServico.Quantidade.def = "0,00";
                    emissaoNFe.ProdutoServico.Quantidade.val("0,00");
                    emissaoNFe.ProdutoServico.Quantidade.configDecimal = { precision: 2, allowZero: true };

                    emissaoNFe.InformacoesAdicionaisProdutoServico.QuantidadeTributavel.def = "0,00";
                    emissaoNFe.InformacoesAdicionaisProdutoServico.QuantidadeTributavel.val("0,00");
                    emissaoNFe.InformacoesAdicionaisProdutoServico.QuantidadeTributavel.configDecimal = { precision: 2, allowZero: true };
                } else if (r.Data.CasasQuantidadeProdutoNFe === 3) {
                    emissaoNFe.ProdutoServico.Quantidade.def = "0,000";
                    emissaoNFe.ProdutoServico.Quantidade.val("0,000");
                    emissaoNFe.ProdutoServico.Quantidade.configDecimal = { precision: 3, allowZero: true };

                    emissaoNFe.InformacoesAdicionaisProdutoServico.QuantidadeTributavel.def = "0,000";
                    emissaoNFe.InformacoesAdicionaisProdutoServico.QuantidadeTributavel.val("0,000");
                    emissaoNFe.InformacoesAdicionaisProdutoServico.QuantidadeTributavel.configDecimal = { precision: 3, allowZero: true };
                } else {
                    emissaoNFe.ProdutoServico.Quantidade.def = "0,0000";
                    emissaoNFe.ProdutoServico.Quantidade.val("0,0000");
                    emissaoNFe.ProdutoServico.Quantidade.configDecimal = { precision: 4, allowZero: true };

                    emissaoNFe.InformacoesAdicionaisProdutoServico.QuantidadeTributavel.def = "0,0000";
                    emissaoNFe.InformacoesAdicionaisProdutoServico.QuantidadeTributavel.val("0,0000");
                    emissaoNFe.InformacoesAdicionaisProdutoServico.QuantidadeTributavel.configDecimal = { precision: 4, allowZero: true };
                }

                if (r.Data.CasasValorProdutoNFe === 0) {
                    emissaoNFe.ProdutoServico.ValorUnitario.def = "0";
                    emissaoNFe.ProdutoServico.ValorUnitario.val("0");
                    emissaoNFe.ProdutoServico.ValorUnitario.configDecimal = { precision: 0, allowZero: true };

                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.def = "0";
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.val("0");
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.configDecimal = { precision: 0, allowZero: true };
                } else if (r.Data.CasasValorProdutoNFe === 1) {
                    emissaoNFe.ProdutoServico.ValorUnitario.def = "0,0";
                    emissaoNFe.ProdutoServico.ValorUnitario.val("0,0");
                    emissaoNFe.ProdutoServico.ValorUnitario.configDecimal = { precision: 1, allowZero: true };

                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.def = "0,0";
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.val("0,0");
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.configDecimal = { precision: 1, allowZero: true };
                } else if (r.Data.CasasValorProdutoNFe === 2) {
                    emissaoNFe.ProdutoServico.ValorUnitario.def = "0,00";
                    emissaoNFe.ProdutoServico.ValorUnitario.val("0,00");
                    emissaoNFe.ProdutoServico.ValorUnitario.configDecimal = { precision: 2, allowZero: true };

                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.def = "0,00";
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.val("0,00");
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.configDecimal = { precision: 2, allowZero: true };
                } else if (r.Data.CasasValorProdutoNFe === 3) {
                    emissaoNFe.ProdutoServico.ValorUnitario.def = "0,000";
                    emissaoNFe.ProdutoServico.ValorUnitario.val("0,000");
                    emissaoNFe.ProdutoServico.ValorUnitario.configDecimal = { precision: 3, allowZero: true };

                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.def = "0,000";
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.val("0,000");
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.configDecimal = { precision: 3, allowZero: true };
                } else if (r.Data.CasasValorProdutoNFe === 4) {
                    emissaoNFe.ProdutoServico.ValorUnitario.def = "0,0000";
                    emissaoNFe.ProdutoServico.ValorUnitario.val("0,0000");
                    emissaoNFe.ProdutoServico.ValorUnitario.configDecimal = { precision: 4, allowZero: true };

                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.def = "0,0000";
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.val("0,0000");
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.configDecimal = { precision: 4, allowZero: true };
                } else if (r.Data.CasasValorProdutoNFe === 5) {
                    emissaoNFe.ProdutoServico.ValorUnitario.def = "0,00000";
                    emissaoNFe.ProdutoServico.ValorUnitario.val("0,00000");
                    emissaoNFe.ProdutoServico.ValorUnitario.configDecimal = { precision: 5, allowZero: true };

                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.def = "0,00000";
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.val("0,00000");
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.configDecimal = { precision: 5, allowZero: true };
                } else if (r.Data.CasasValorProdutoNFe === 6) {
                    emissaoNFe.ProdutoServico.ValorUnitario.def = "0,000000";
                    emissaoNFe.ProdutoServico.ValorUnitario.val("0,000000");
                    emissaoNFe.ProdutoServico.ValorUnitario.configDecimal = { precision: 6, allowZero: true };

                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.def = "0,000000";
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.val("0,000000");
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.configDecimal = { precision: 6, allowZero: true };
                } else if (r.Data.CasasValorProdutoNFe === 7) {
                    emissaoNFe.ProdutoServico.ValorUnitario.def = "0,0000000";
                    emissaoNFe.ProdutoServico.ValorUnitario.val("0,0000000");
                    emissaoNFe.ProdutoServico.ValorUnitario.configDecimal = { precision: 7, allowZero: true };

                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.def = "0,0000000";
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.val("0,0000000");
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.configDecimal = { precision: 7, allowZero: true };
                } else if (r.Data.CasasValorProdutoNFe === 8) {
                    emissaoNFe.ProdutoServico.ValorUnitario.def = "0,00000000";
                    emissaoNFe.ProdutoServico.ValorUnitario.val("0,00000000");
                    emissaoNFe.ProdutoServico.ValorUnitario.configDecimal = { precision: 8, allowZero: true };

                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.def = "0,00000000";
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.val("0,00000000");
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.configDecimal = { precision: 8, allowZero: true };
                } else if (r.Data.CasasValorProdutoNFe === 9) {
                    emissaoNFe.ProdutoServico.ValorUnitario.def = "0,000000000";
                    emissaoNFe.ProdutoServico.ValorUnitario.val("0,000000000");
                    emissaoNFe.ProdutoServico.ValorUnitario.configDecimal = { precision: 9, allowZero: true };

                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.def = "0,000000000";
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.val("0,000000000");
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.configDecimal = { precision: 9, allowZero: true };
                } else if (r.Data.CasasValorProdutoNFe === 10) {
                    emissaoNFe.ProdutoServico.ValorUnitario.def = "0,0000000000";
                    emissaoNFe.ProdutoServico.ValorUnitario.val("0,0000000000");
                    emissaoNFe.ProdutoServico.ValorUnitario.configDecimal = { precision: 10, allowZero: true };

                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.def = "0,0000000000";
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.val("0,0000000000");
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.configDecimal = { precision: 10, allowZero: true };
                } else {
                    emissaoNFe.ProdutoServico.ValorUnitario.def = "0,00000";
                    emissaoNFe.ProdutoServico.ValorUnitario.val("0,00000");
                    emissaoNFe.ProdutoServico.ValorUnitario.configDecimal = { precision: 5, allowZero: true };

                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.def = "0,00000";
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.val("0,00000");
                    emissaoNFe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.configDecimal = { precision: 5, allowZero: true };
                }

            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    };

    this.BuscarDadosNFe = function () {
        executarReST("NotaFiscalEletronica/BuscarPorCodigo", { Codigo: emissaoNFe.NFe.Codigo.val() }, function (r) {
            if (r.Success) {

                PreencherObjetoKnout(emissaoNFe.NFe, { Data: r.Data.NFe });

                var statusNFe = r.Data.NFe.Status;

                if (statusNFe == EnumStatusNFe.REJEICAO || statusNFe == EnumStatusNFe.EMDIGITACAO) {
                    emissaoNFe.CRUDNFe.Emitir.visible(true);
                    emissaoNFe.CRUDNFe.Salvar.visible(true);
                }

                if (r.Data.Totalizador != null)
                    PreencherObjetoKnout(emissaoNFe.Totalizador, { Data: r.Data.Totalizador });
                if (r.Data.Observacao != null)
                    PreencherObjetoKnout(emissaoNFe.Observacao, { Data: r.Data.Observacao });
                if (r.Data.ExportacaoCompra != null)
                    PreencherObjetoKnout(emissaoNFe.ExportacaoCompra, { Data: r.Data.ExportacaoCompra });
                if (r.Data.Transporte != null)
                    PreencherObjetoKnout(emissaoNFe.Transporte, { Data: r.Data.Transporte });
                if (r.Data.RetiradaEntrega != null)
                    PreencherObjetoKnout(emissaoNFe.RetiradaEntrega, { Data: r.Data.RetiradaEntrega });
                if (r.Data.Referencia != null)
                    PreencherObjetoKnout(emissaoNFe.Referencia, { Data: r.Data.Referencia });

                emissaoNFe.ProdutosServicos = new Array();
                if (r.Data.ProdutosServicos != null)
                    emissaoNFe.ProdutosServicos = r.Data.ProdutosServicos;
                emissaoNFe.ListaProdutoServico.RecarregarGrid();

                emissaoNFe.LotesProdutos = new Array();
                if (r.Data.LotesProdutos != null)
                    emissaoNFe.LotesProdutos = r.Data.LotesProdutos;
                emissaoNFe.LoteProduto.RecarregarGrid();

                emissaoNFe.Parcelas = new Array();
                if (r.Data.Parcelas != null)
                    emissaoNFe.Parcelas = r.Data.Parcelas;
                emissaoNFe.Parcelamento.RecarregarGrid();

                emissaoNFe.Referencia.TipoDocumentoChange(false);
                emissaoNFe.ImpostoPIS.TipoEmissaoChange();

                emissaoNFe.AlterarEstadoNFe(emissaoNFe);
                emissaoNFe.SetarPermissoes();

                emissaoNFe.NFe.Serie.enable(false);
                emissaoNFe.NFe.Empresa.enable(false);

                if (callbackInit != null)
                    callbackInit();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    };

    this.CarregarDadosPedido = function () {
        executarReST("NotaFiscalEletronica/CarregarPedidoVendaPorCodigo", { Codigos: JSON.stringify(codigosPedidoVenda) }, function (r) {
            if (r.Success) {

                PreencherObjetoKnout(emissaoNFe.NFe, { Data: r.Data.NFe });
                emissaoNFe.NFe.CodigoPedidoVenda.val(JSON.stringify(codigosPedidoVenda));

                //if (r.Data.Totalizador != null)
                //    PreencherObjetoKnout(emissaoNFe.Totalizador, { Data: r.Data.Totalizador });
                if (r.Data.Observacao != null)
                    PreencherObjetoKnout(emissaoNFe.Observacao, { Data: r.Data.Observacao });

                emissaoNFe.ProdutosServicos = new Array();
                if (r.Data.ProdutosServicos != null) {
                    for (var i = 0; i < r.Data.ProdutosServicos.length; i++) {
                        var item = r.Data.ProdutosServicos[i];

                        CalcularImpostoIPIPedido(item);
                        CalcularImpostoCOFINSPedido(item);
                        CalcularImpostoPISPedido(item);
                        CalcularImpostoISSPedido(item);
                        CalcularImpostoICMSPedido(item);
                        CalcularImpostoDIFALPedido(item);
                        CalcularImpostoFCPPedido(item);
                        CalcularImpostoICMSSTPedido(item);

                        emissaoNFe.ProdutosServicos[i] = item;

                        emissaoNFe.SomarTotalizadoresPedido(emissaoNFe, item);
                    }
                }
                emissaoNFe.ListaProdutoServico.RecarregarGrid();

                emissaoNFe.Referencia.TipoDocumentoChange(false);
                emissaoNFe.ImpostoPIS.TipoEmissaoChange();

                emissaoNFe.Parcelas = new Array();
                if (r.Data.Parcelas != null)
                    emissaoNFe.Parcelas = r.Data.Parcelas;
                emissaoNFe.Parcelamento.RecarregarGrid();

                emissaoNFe.AlterarEstadoNFe(emissaoNFe);
                emissaoNFe.SetarPermissoes();

                if (callbackInit != null)
                    callbackInit();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    };

    this.CarregarDadosDocumentoEntrada = function () {
        executarReST("NotaFiscalEletronica/CarregarDocumentoEntradaPorCodigo", { Codigos: JSON.stringify(codigosDocumentoEntrada) }, function (r) {
            if (r.Success) {

                PreencherObjetoKnout(emissaoNFe.NFe, { Data: r.Data.NFe });
                emissaoNFe.NFe.CodigoDocumentoEntrada.val(JSON.stringify(codigosDocumentoEntrada));

                emissaoNFe.ProdutosServicos = new Array();
                if (r.Data.ProdutosServicos != null) {
                    for (var i = 0; i < r.Data.ProdutosServicos.length; i++) {
                        var item = r.Data.ProdutosServicos[i];

                        CalcularImpostoIPIPedido(item);
                        CalcularImpostoCOFINSPedido(item);
                        CalcularImpostoPISPedido(item);
                        CalcularImpostoISSPedido(item);
                        CalcularImpostoICMSPedido(item);
                        CalcularImpostoDIFALPedido(item);
                        CalcularImpostoFCPPedido(item);
                        CalcularImpostoICMSSTPedido(item);

                        emissaoNFe.ProdutosServicos[i] = item;

                        emissaoNFe.SomarTotalizadoresPedido(emissaoNFe, item);
                    }
                }
                emissaoNFe.ListaProdutoServico.RecarregarGrid();

                emissaoNFe.Referencia.TipoDocumentoChange(false);
                emissaoNFe.ImpostoPIS.TipoEmissaoChange();

                emissaoNFe.AlterarEstadoNFe(emissaoNFe);
                emissaoNFe.SetarPermissoes();

                if (callbackInit != null)
                    callbackInit();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    };

    this.SomarTotalizadoresPedido = function (nfe, item) {
        var baseICMSItem = Globalize.parseFloat(item.BCICMS);
        var baseICMSTotal = Globalize.parseFloat(nfe.Totalizador.BaseICMS.val());
        var baseICMS = baseICMSItem + baseICMSTotal;
        nfe.Totalizador.BaseICMS.val(Globalize.format(baseICMS, "n2"));

        var valorICMSItem = Globalize.parseFloat(item.ValorICMS);
        var valorICMSTotal = Globalize.parseFloat(nfe.Totalizador.ValorICMS.val());
        var valorICMS = valorICMSItem + valorICMSTotal;
        nfe.Totalizador.ValorICMS.val(Globalize.format(valorICMS, "n2"));

        var valorICMSDesoneradoItem = Globalize.parseFloat(item.ValorICMSDesonerado);
        var valorICMSDesoneradoTotal = Globalize.parseFloat(nfe.Totalizador.ValorICMSDesonerado.val());
        var valorICMSDesonerado = valorICMSDesoneradoItem + valorICMSDesoneradoTotal;
        nfe.Totalizador.ValorICMSDesonerado.val(Globalize.format(valorICMSDesonerado, "n2"));

        var valorIIItem = Globalize.parseFloat(item.ValorII);
        var valorIITotal = Globalize.parseFloat(nfe.Totalizador.ValorII.val());
        var valorII = valorIIItem + valorIITotal;
        nfe.Totalizador.ValorII.val(Globalize.format(valorII, "n2"));

        var baseICMSSTItem = Globalize.parseFloat(item.BCICMSST);
        var baseICMSSTTotal = Globalize.parseFloat(nfe.Totalizador.BaseICMSST.val());
        var baseICMSST = baseICMSSTItem + baseICMSSTTotal;
        nfe.Totalizador.BaseICMSST.val(Globalize.format(baseICMSST, "n2"));

        var valorICMSSTItem = Globalize.parseFloat(item.ValorICMSST);
        var valorICMSSTTotal = Globalize.parseFloat(nfe.Totalizador.ValorICMSST.val());
        var valorICMSST = valorICMSSTItem + valorICMSSTTotal;
        nfe.Totalizador.ValorICMSST.val(Globalize.format(valorICMSST, "n2"));

        var valorTotalItem = Globalize.parseFloat(item.ValorTotal);
        if (item.CodigoProduto > 0) {
            var valorTotalProdutosTotal = Globalize.parseFloat(nfe.Totalizador.ValorTotalProdutos.val());
            var valorTotalProdutos = valorTotalItem + valorTotalProdutosTotal;
            nfe.Totalizador.ValorTotalProdutos.val(Globalize.format(valorTotalProdutos, "n2"));
        }
        else {
            var valorTotalServicosTotal = Globalize.parseFloat(nfe.Totalizador.ValorTotalServicos.val());
            var valorTotalServicos = valorTotalItem + valorTotalServicosTotal;
            nfe.Totalizador.ValorTotalServicos.val(Globalize.format(valorTotalServicos, "n2"));
        }

        var valorFreteItem = Globalize.parseFloat(item.ValorFrete);
        var valorFreteTotal = Globalize.parseFloat(nfe.Totalizador.ValorFrete.val());
        var valorFrete = valorFreteItem + valorFreteTotal;
        nfe.Totalizador.ValorFrete.val(Globalize.format(valorFrete, "n2"));

        var valorSeguroItem = Globalize.parseFloat(item.ValorSeguro);
        var valorSeguroTotal = Globalize.parseFloat(nfe.Totalizador.ValorSeguro.val());
        var valorSeguro = valorSeguroItem + valorSeguroTotal;
        nfe.Totalizador.ValorSeguro.val(Globalize.format(valorSeguro, "n2"));

        var valorDescontoItem = Globalize.parseFloat(item.ValorDesconto);
        var valorDescontoTotal = Globalize.parseFloat(nfe.Totalizador.ValorDesconto.val());
        var valorDesconto = valorDescontoItem + valorDescontoTotal;
        nfe.Totalizador.ValorDesconto.val(Globalize.format(valorDesconto, "n2"));

        var valorOutrasDespesasItem = Globalize.parseFloat(item.ValorOutras);
        var valorOutrasDespesasTotal = Globalize.parseFloat(nfe.Totalizador.ValorOutrasDespesas.val());
        var valorOutrasDespesas = valorOutrasDespesasItem + valorOutrasDespesasTotal;
        nfe.Totalizador.ValorOutrasDespesas.val(Globalize.format(valorOutrasDespesas, "n2"));

        var valorIPIItem = Globalize.parseFloat(item.ValorIPI);
        var valorIPITotal = Globalize.parseFloat(nfe.Totalizador.ValorIPI.val());
        var valorIPI = valorIPIItem + valorIPITotal;
        nfe.Totalizador.ValorIPI.val(Globalize.format(valorIPI, "n2"));

        var valorFCPICMSItem = Globalize.parseFloat(item.ValorFCPICMS);
        var valorFCPICMSTotal = Globalize.parseFloat(nfe.Totalizador.ValorFCPICMS.val());
        var valorFCPICMS = valorFCPICMSItem + valorFCPICMSTotal;
        nfe.Totalizador.ValorFCPICMS.val(Globalize.format(valorFCPICMS, "n2"));

        var valorFCPICMSSTItem = Globalize.parseFloat(item.ValorFCPICMSST);
        var valorFCPICMSSTTotal = Globalize.parseFloat(nfe.Totalizador.ValorFCPICMSST.val());
        var valorFCPICMSST = valorFCPICMSSTItem + valorFCPICMSSTTotal;
        nfe.Totalizador.ValorFCPICMSST.val(Globalize.format(valorFCPICMSST, "n2"));

        var valorIPIDevolvidoItem = Globalize.parseFloat(item.ValorIPIDevolvido);
        var valorIPIDevolvidoTotal = Globalize.parseFloat(nfe.Totalizador.ValorIPIDevolvido.val());
        var valorIPIDevolvido = valorIPIDevolvidoItem + valorIPIDevolvidoTotal;
        nfe.Totalizador.ValorIPIDevolvido.val(Globalize.format(valorIPIDevolvido, "n2"));

        var baseISSItem = Globalize.parseFloat(item.BCISS);
        var baseISSTotal = Globalize.parseFloat(nfe.Totalizador.BaseISS.val());
        var baseISS = baseISSItem + baseISSTotal;
        nfe.Totalizador.BaseISS.val(Globalize.format(baseISS, "n2"));

        var valorISSItem = Globalize.parseFloat(item.ValorISS);
        var valorISSTotal = Globalize.parseFloat(nfe.Totalizador.ValorISS.val());
        var valorISS = valorISSItem + valorISSTotal;
        nfe.Totalizador.ValorISS.val(Globalize.format(valorISS, "n2"));

        var baseDeducaoItem = Globalize.parseFloat(item.BaseDeducaoISS);
        var baseDeducaoTotal = Globalize.parseFloat(nfe.Totalizador.BaseDeducao.val());
        var baseDeducao = baseDeducaoItem + baseDeducaoTotal;
        nfe.Totalizador.BaseDeducao.val(Globalize.format(baseDeducao, "n2"));

        var valorOutrasRetencoesItem = Globalize.parseFloat(item.OutrasRetencoesISS);
        var valorOutrasRetencoesTotal = Globalize.parseFloat(nfe.Totalizador.ValorOutrasRetencoes.val());
        var valorOutrasRetencoes = valorOutrasRetencoesItem + valorOutrasRetencoesTotal;
        nfe.Totalizador.ValorOutrasRetencoes.val(Globalize.format(valorOutrasRetencoes, "n2"));

        var valorDescontoIncondicionalItem = Globalize.parseFloat(item.DescontoIncondicional);
        var valorDescontoIncondicionalTotal = Globalize.parseFloat(nfe.Totalizador.ValorDescontoIncondicional.val());
        var valorDescontoIncondicional = valorDescontoIncondicionalItem + valorDescontoIncondicionalTotal;
        nfe.Totalizador.ValorDescontoIncondicional.val(Globalize.format(valorDescontoIncondicional, "n2"));

        var valorDescontoCondicionalItem = Globalize.parseFloat(item.DescontoCondicional);
        var valorDescontoCondicionalTotal = Globalize.parseFloat(nfe.Totalizador.ValorDescontoCondicional.val());
        var valorDescontoCondicional = valorDescontoCondicionalItem + valorDescontoCondicionalTotal;
        nfe.Totalizador.ValorDescontoCondicional.val(Globalize.format(valorDescontoCondicional, "n2"));

        var valorRetencaoISSItem = Globalize.parseFloat(item.RetencaoISS);
        var valorRetencaoISSTotal = Globalize.parseFloat(nfe.Totalizador.ValorRetencaoISS.val());
        var valorRetencaoISS = valorRetencaoISSItem + valorRetencaoISSTotal;
        nfe.Totalizador.ValorRetencaoISS.val(Globalize.format(valorRetencaoISS, "n2"));

        var basePISItem = Globalize.parseFloat(item.BCPIS);
        var basePISTotal = Globalize.parseFloat(nfe.Totalizador.BasePIS.val());
        var basePIS = basePISItem + basePISTotal;
        nfe.Totalizador.BasePIS.val(Globalize.format(basePIS, "n2"));

        var valorPISItem = Globalize.parseFloat(item.ValorPIS);
        var valorPISTotal = Globalize.parseFloat(nfe.Totalizador.ValorPIS.val());
        var valorPIS = valorPISItem + valorPISTotal;
        nfe.Totalizador.ValorPIS.val(Globalize.format(valorPIS, "n2"));

        var baseCOFINSItem = Globalize.parseFloat(item.BCCOFINS);
        var baseCOFINSTotal = Globalize.parseFloat(nfe.Totalizador.BaseCOFINS.val());
        var baseCOFINS = baseCOFINSItem + baseCOFINSTotal;
        nfe.Totalizador.BaseCOFINS.val(Globalize.format(baseCOFINS, "n2"));

        var valorCOFINSItem = Globalize.parseFloat(item.ValorCOFINS);
        var valorCOFINSTotal = Globalize.parseFloat(nfe.Totalizador.ValorCOFINS.val());
        var valorCOFINS = valorCOFINSItem + valorCOFINSTotal;
        nfe.Totalizador.ValorCOFINS.val(Globalize.format(valorCOFINS, "n2"));

        var valorFCPItem = Globalize.parseFloat(item.ValorFCP);
        var valorFCPTotal = Globalize.parseFloat(nfe.Totalizador.ValorFCP.val());
        var valorFCP = valorFCPItem + valorFCPTotal;
        nfe.Totalizador.ValorFCP.val(Globalize.format(valorFCP, "n2"));

        //DIFAL
        var valorICMSDestinoItem = Globalize.parseFloat(item.ValorICMSDestino);
        var valorICMSDestinoTotal = Globalize.parseFloat(nfe.Totalizador.ValorICMSDestino.val());
        var valorICMSDestino = valorICMSDestinoItem + valorICMSDestinoTotal;
        nfe.Totalizador.ValorICMSDestino.val(Globalize.format(valorICMSDestino, "n2"));

        var valorICMSRemetenteItem = Globalize.parseFloat(item.ValorICMSRemetente);
        var valorICMSRemetenteTotal = Globalize.parseFloat(nfe.Totalizador.ValorICMSRemetente.val());
        var valorICMSRemetente = valorICMSRemetenteItem + valorICMSRemetenteTotal;
        nfe.Totalizador.ValorICMSRemetente.val(Globalize.format(valorICMSRemetente, "n2"));

        //DRCST
        var bCICMSSTRetidoItem = Globalize.parseFloat(item.BCICMSSTRetido);
        var bCICMSSTRetidoTotal = Globalize.parseFloat(nfe.Totalizador.BCICMSSTRetido.val());
        nfe.Totalizador.BCICMSSTRetido.val(Globalize.format(bCICMSSTRetidoItem + bCICMSSTRetidoTotal, "n2"));

        var valorICMSSTRetidoItem = Globalize.parseFloat(item.ValorICMSSTRetido);
        var valorICMSSTRetidoTotal = Globalize.parseFloat(nfe.Totalizador.ValorICMSSTRetido.val());
        nfe.Totalizador.ValorICMSSTRetido.val(Globalize.format(valorICMSSTRetidoItem + valorICMSSTRetidoTotal, "n2"));

        //TOTAL NF
        var _valorFrete = Globalize.parseFloat(item.ValorFrete);
        var _valorSeguro = Globalize.parseFloat(item.ValorSeguro);
        var _valorDesconto = Globalize.parseFloat(item.ValorDesconto);
        var _valorOutrasDespesas = Globalize.parseFloat(item.ValorOutras);
        var _valorICMSDesonerado = Globalize.parseFloat(item.ValorICMSDesonerado);
        var _valorIPI = Globalize.parseFloat(item.ValorIPI);
        var _valorICMSST = Globalize.parseFloat(item.ValorICMSST);
        var _valorII = Globalize.parseFloat(item.ValorII);
        var _valorFCPICMSST = Globalize.parseFloat(item.ValorFCPICMSST);
        var _valorIPIDevolvido = Globalize.parseFloat(item.ValorIPIDevolvido);

        var _valorTotalNFeItem = Globalize.parseFloat(item.ValorTotal);
        var _valorTotalNFeTotal = Globalize.parseFloat(nfe.Totalizador.ValorTotalNFe.val());

        var valorTotalNFe = _valorTotalNFeItem + _valorTotalNFeTotal + _valorICMSST + _valorIPI + _valorOutrasDespesas - _valorDesconto - _valorICMSDesonerado + _valorSeguro + _valorFrete + _valorII + _valorFCPICMSST + _valorIPIDevolvido;
        nfe.Totalizador.ValorTotalNFe.val(Globalize.format(valorTotalNFe, "n2"));
    };

    this.FecharModal = function () {
        emissaoNFe.ModalNFe.hide();
    };

    this.Destroy = function () {
        if (emissaoNFe.ModalNFe)
            emissaoNFe.ModalNFe.dispose();

        $("#" + emissaoNFe.IdModal).remove();
        emissaoNFe = null;
    };

    this.RenderizarModalNFe = function () {
        emissaoNFe.ObterHTMLEmissaoNFe().then(function () {
            var html = _HTMLEmissaoNFe.replace(/#divModalNFe/g, emissaoNFe.IdModal);
            $('#js-page-content').append(html);
            emissaoNFe.CRUDNFe.Cancelar.eventClick = function (e) {
                emissaoNFe.FecharModal();
            };

            KoBindings(emissaoNFe.NFe, emissaoNFe.IdKnockoutNFe);
            KoBindings(emissaoNFe.CRUDNFe, emissaoNFe.IdKnockoutCRUDNFe);

            new BuscarAtividades(emissaoNFe.NFe.Atividade);
            new BuscarClientes(emissaoNFe.NFe.Pessoa, function (data) {
                emissaoNFe.NFe.Pessoa.codEntity(data.Codigo);
                emissaoNFe.NFe.Pessoa.val(data.Nome + " (" + data.Localidade + ")");
                if (data.CodigoAtividade != null && data.CodigoAtividade > 0) {
                    emissaoNFe.NFe.Atividade.codEntity(data.CodigoAtividade);
                    emissaoNFe.NFe.Atividade.val(data.Atividade);
                }
                LimparCampoEntity(emissaoNFe.NFe.NaturezaOperacao);
            }, true);
            new BuscarLocalidadesBrasil(emissaoNFe.NFe.CidadePrestacao);
            new BuscarNaturezasOperacoesNotaFiscal(emissaoNFe.NFe.NaturezaOperacao, null, null, function (data) {
                emissaoNFe.NFe.NaturezaOperacao.codEntity(data.Codigo);
                if (data.CFOP != null && data.CFOP > 0)
                    emissaoNFe.NFe.NaturezaOperacao.val(data.Descricao + " (" + data.CFOP + ")");
                else
                    emissaoNFe.NFe.NaturezaOperacao.val(data.Descricao);
            }, emissaoNFe.NFe.Pessoa, emissaoNFe.NFe.Empresa, emissaoNFe.NFe.IndicadorPresenca, null, emissaoNFe.NFe.TipoEmissao);
            new BuscarSerieEmpresa(emissaoNFe.NFe.Serie, null, null, function (data) {
                if (data != null) {
                    if (data.ProximoNumero > 0) {
                        emissaoNFe.NFe.Numero.val(data.ProximoNumero);
                    }
                    emissaoNFe.NFe.Serie.val(data.Descricao);
                    emissaoNFe.NFe.Serie.codEntity(data.Codigo);
                }
            }, emissaoNFe.NFe.Empresa, 3);
            new BuscarEmpresa(emissaoNFe.NFe.Empresa, function (data) {
                emissaoNFe.NFe.Empresa.codEntity(data.Codigo);
                emissaoNFe.NFe.Empresa.val(data.RazaoSocial);
                emissaoNFe.NFe.Numero.val("");
                LimparCampoEntity(emissaoNFe.NFe.Serie);
            }, null);
            new BuscarClientes(emissaoNFe.NFe.Intermediador, function (data) {
                emissaoNFe.NFe.Intermediador.codEntity(data.Codigo);
                emissaoNFe.NFe.Intermediador.val(data.Nome);
            }, true, null, null, null, null, null, null, "J");

            if (!emissaoNFe.ModalNFe)
                emissaoNFe.ModalNFe = new bootstrap.Modal(document.getElementById(emissaoNFe.IdModal), { keyboard: false, backdrop: 'static' });

            emissaoNFe.ModalNFe.show();

            $('#' + emissaoNFe.IdModal).on('hidden.bs.modal', function () {
                emissaoNFe.Destroy();
            });

            //if (emissaoNFe.ProdutoServico != undefined)
            //    emissaoNFe.ProdutoServico.Load();
            emissaoNFe.ListaProdutoServico.Load();
            emissaoNFe.ImpostoICMS.Load();
            emissaoNFe.ImpostoICMSMonoRet.Load();
            emissaoNFe.ImpostoICMSST.Load();
            emissaoNFe.ImpostoDIFAL.Load();
            emissaoNFe.ImpostoPIS.Load();
            emissaoNFe.ImpostoCOFINS.Load();
            emissaoNFe.ImpostoIPI.Load();
            emissaoNFe.ImpostoISS.Load();
            emissaoNFe.ImpostoII.Load();
            emissaoNFe.ImpostoICMSDesonerado.Load();
            //emissaoNFe.InformacoesAdicionaisProdutoServico.Load();
            emissaoNFe.LoteProduto.Load();
            emissaoNFe.CombustivelProduto.Load();
            emissaoNFe.ExportacaoProdutoServico.Load();

            emissaoNFe.Totalizador.Load();
            emissaoNFe.Observacao.Load();
            emissaoNFe.ExportacaoCompra.Load();
            emissaoNFe.Transporte.Load();
            emissaoNFe.Referencia.Load();
            emissaoNFe.DetalheParcela.Load();
            emissaoNFe.Parcelamento.Load();
            emissaoNFe.RetiradaEntrega.Load();

            emissaoNFe.BuscarDadosPadroesEmpresa();

            if (emissaoNFe.NFe.Codigo.val() != "" && emissaoNFe.NFe.Codigo.val() > 0) {
                emissaoNFe.BuscarDadosNFe();
            } else if (codigosPedidoVenda != null && codigosPedidoVenda.length > 0) {
                emissaoNFe.CarregarDadosPedido();
            } else if (codigosDocumentoEntrada != null && codigosDocumentoEntrada.length > 0) {
                emissaoNFe.CarregarDadosDocumentoEntrada();
            } else if (callbackInit != null) {
                emissaoNFe.SetarPermissoes();
                callbackInit();
            }

        });
    };

    this.ObterHTMLEmissaoNFe = function () {
        var p = new promise.Promise();
        if (_HTMLEmissaoNFe == "") {
            $.get("Content/Static/NotaFiscal/NotaFiscalEletronica.html?dyn=" + emissaoNFe.IdModal, function (data) {
                _HTMLEmissaoNFe = data;
                p.done();
            });
        } else {
            p.done();
        }
        return p;
    };

    this.AlterarEstadoNFe = function () {
        if (emissaoNFe.NFe.Status.val() == EnumStatusNFe.AUTORIZADO ||
            emissaoNFe.NFe.Status.val() == EnumStatusNFe.CANCELADO ||
            emissaoNFe.NFe.Status.val() == EnumStatusNFe.EMCANCELAMENTO) {
            emissaoNFe.NFe.Protocolo.visible(true);
            emissaoNFe.NFe.Chave.visible(true);
        }

        //if (emissaoNFe.NFe.Status.val() == EnumStatusNFe.CANCELADO) {
        //    emissaoNFe.NFe.ProtocoloCancelamentoInutilizacao.text("Protocolo de Cancelamento:");
        //    emissaoNFe.NFe.ProtocoloCancelamentoInutilizacao.visible(true);
        //    emissaoNFe.NFe.JustificativaCancelamento.visible(true);
        //} else if (emissaoNFe.NFe.Status.val() == EnumStatusNFe.CANCELADO) {
        //    emissaoNFe.NFe.ProtocoloCancelamentoInutilizacao.text("Protocolo de Inutilização:");
        //    emissaoNFe.NFe.ProtocoloCancelamentoInutilizacao.visible(true);
        //}
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

    //emissaoNFe.LoadEmissaoNFe();
    //if (_tipoAliquotaICMSNFe == null || _tipoAliquotaICMSNFe.length <= 0)
    //    ObterAliquotasICMS().then(function () { emissaoNFe.LoadEmissaoNFe(); });
    //else {
    //    setTimeout(function () {
    //        emissaoNFe.LoadEmissaoNFe();
    //    }, 50);
    //}
    setTimeout(function () {
        emissaoNFe.LoadEmissaoNFe();
    }, 50);
}

function DesabilitarCamposInstanciasNFe(instancia) {
    $.each(instancia, function (i, knout) {
        if (knout.enable != null) {
            if (knout.enable == true || knout.enable == false)
                knout.enable = false;
            else
                knout.enable(false);
        }
    });
}

function HabilitarCamposInstanciasNFe(instancia) {
    $.each(instancia, function (i, knout) {
        if (knout.enable != null) {
            if (knout.enable == false || knout.enable == true)
                knout.enable = true;
            else
                knout.enable(true);
        }
    });
}

function ObterObjetoNFe(emissaoNFe) {
    var nfe = new Object();
    var codigoSerie = emissaoNFe.NFe.Serie.codEntity();
    var numeroSerie = emissaoNFe.NFe.Serie.val();
    nfe.NFe = RetornarObjetoPesquisa(emissaoNFe.NFe);
    nfe.NFe.Serie = codigoSerie;
    emissaoNFe.NFe.Serie.codEntity(codigoSerie);
    emissaoNFe.NFe.Serie.val(numeroSerie);
    nfe.ProdutosServicos = emissaoNFe.ProdutosServicos;
    nfe.LotesProdutos = emissaoNFe.LotesProdutos;
    nfe.Totalizador = RetornarObjetoPesquisa(emissaoNFe.Totalizador);
    nfe.Transporte = RetornarObjetoPesquisa(emissaoNFe.Transporte);
    nfe.Referencia = RetornarObjetoPesquisa(emissaoNFe.Referencia);
    nfe.Observacao = RetornarObjetoPesquisa(emissaoNFe.Observacao);
    nfe.ExportacaoCompra = RetornarObjetoPesquisa(emissaoNFe.ExportacaoCompra);
    nfe.Parcelas = emissaoNFe.Parcelas;
    nfe.RetiradaEntrega = RetornarObjetoPesquisa(emissaoNFe.RetiradaEntrega);
    
    return JSON.stringify(nfe);
}

function CalcularImpostoIPIPedido(instancia) {
    var base = Globalize.parseFloat(instancia.BCIPI);
    var reducaoBase = Globalize.parseFloat(instancia.ReducaoBCIPI);
    var aliquota = Globalize.parseFloat(instancia.AliquotaIPI);

    if (base > 0 && aliquota > 0) {
        if (reducaoBase > 0) {
            base = (base - (base * (reducaoBase / 100)));
            instancia.BCIPI = Globalize.format(base, "n2");
        }

        var valor = base * (aliquota / 100);
        instancia.ValorIPI = Globalize.format(valor, "n2");
    } else
        instancia.ValorIPI = "0,00";
}

function CalcularImpostoCOFINSPedido(instancia) {
    var base = Globalize.parseFloat(instancia.BCCOFINS);
    var reducaoBase = Globalize.parseFloat(instancia.ReducaoBCCOFINS);
    var aliquota = Globalize.parseFloat(instancia.AliquotaCOFINS);

    if (base > 0 && aliquota > 0) {
        if (reducaoBase > 0) {
            base = (base - (base * (reducaoBase / 100)));
            instancia.BCCOFINS = Globalize.format(base, "n2");
        }

        var valor = base * (aliquota / 100);
        instancia.ValorCOFINS = Globalize.format(valor, "n2");
    } else
        instancia.ValorCOFINS = "0,00";
}

function CalcularImpostoPISPedido(instancia) {
    var basePIS = Globalize.parseFloat(instancia.BCPIS);
    var reducaoBasePIS = Globalize.parseFloat(instancia.ReducaoBCPIS);
    var aliquotaPIS = Globalize.parseFloat(instancia.AliquotaPIS);

    if (basePIS > 0 && aliquotaPIS > 0) {
        if (reducaoBasePIS > 0) {
            basePIS = (basePIS - (basePIS * (reducaoBasePIS / 100)));
            instancia.BCPIS = Globalize.format(basePIS, "n2");
        }

        var valorPIS = basePIS * (aliquotaPIS / 100);
        instancia.ValorPIS = Globalize.format(valorPIS, "n2");
    }
    else
        instancia.ValorPIS = "0,00";
}

function CalcularImpostoISSPedido(instancia) {
    var base = Globalize.parseFloat(instancia.BCISS);
    var aliquota = Globalize.parseFloat(instancia.AliquotaISS);

    if (base > 0 && aliquota > 0) {
        var valor = 0;
        valor = base * (aliquota / 100);
        instancia.ValorISS = Globalize.format(valor, "n2");
    } else
        instancia.ValorISS = "0,00";
}

function CalcularImpostoICMSPedido(instancia) {
    var baseICMS = Globalize.parseFloat(instancia.BCICMS);
    var reducaoICMS = Globalize.parseFloat(instancia.ReducaoBCICMS);
    var aliquotaICMS = Globalize.parseFloat(instancia.AliquotaICMS);

    if (baseICMS > 0 && aliquotaICMS > 0) {
        if (reducaoICMS > 0) {
            baseICMS = (baseICMS - (baseICMS * (reducaoICMS / 100)));
            instancia.BCICMS = Globalize.format(baseICMS, "n2");
        }

        var valorICMS = baseICMS * (aliquotaICMS / 100);
        instancia.ValorICMS = Globalize.format(valorICMS, "n2");
    }
}

function CalcularImpostoDIFALPedido(instancia) {
    var baseICMSDestino = Globalize.parseFloat(instancia.BCICMSDestino);
    var aliquotaICMSDestino = Globalize.parseFloat(instancia.AliquotaICMSDestino);
    var aliquotaICMSInterno = Globalize.parseFloat(instancia.AliquotaICMSInterno);
    var percentualPartilha = Globalize.parseFloat(instancia.PercentualPartilha);

    if (baseICMSDestino > 0 && aliquotaICMSDestino > 0 && aliquotaICMSInterno > 0 && percentualPartilha > 0 && aliquotaICMSDestino <= aliquotaICMSInterno) {
        var difal = 0;
        difal = baseICMSDestino * ((aliquotaICMSInterno - aliquotaICMSDestino) / 100);

        var valorICMSDestino = 0;
        var valorICMSRemetente = 0;

        valorICMSRemetente = difal * ((percentualPartilha - 100) / 100);
        valorICMSDestino = difal * (percentualPartilha / 100);

        instancia.ValorICMSDestino = Globalize.format(valorICMSDestino, "n2");
        instancia.ValorICMSRemetente = Globalize.format(valorICMSRemetente, "n2");
    } else {
        instancia.ValorICMSDestino = "0,00";
        instancia.ValorICMSRemetente = "0,00";
    }
}

function CalcularImpostoFCPPedido(instancia) {
    var baseFCP = Globalize.parseFloat(instancia.BaseFCPDestino);
    var percentualFCP = Globalize.parseFloat(instancia.AliquotaFCP);

    if (baseFCP > 0 && percentualFCP > 0) {
        var valorFCP = 0;
        valorFCP = baseFCP * (percentualFCP / 100);
        instancia.ValorFCP = Globalize.format(valorFCP, "n2");
    } else
        instancia.ValorFCP = "0,00";
}

function CalcularImpostoICMSSTPedido(instancia) {
    var valorIPI = Globalize.parseFloat(instancia.ValorIPI);
    var valorFrete = Globalize.parseFloat(instancia.ValorFrete);
    var valorSeguro = Globalize.parseFloat(instancia.ValorSeguro);
    var valorOutras = Globalize.parseFloat(instancia.ValorOutras);
    var valorDesconto = Globalize.parseFloat(instancia.ValorDesconto);
    var valorTotalItem = Globalize.parseFloat(instancia.ValorTotal);
    var base = Globalize.parseFloat(instancia.BCICMSST);
    var reducaoBase = Globalize.parseFloat(instancia.ReducaoBCICMSST);
    var mva = Globalize.parseFloat(instancia.PercentualMVA);
    var aliquotaInterna = Globalize.parseFloat(instancia.AliquotaICMSST);
    var aliquotaInterestadual = Globalize.parseFloat(instancia.AliquotaInterestadual);

    if (base > 0 && aliquotaInterna > 0 && mva > 0 && aliquotaInterestadual > 0 && valorTotalItem > 0) {
        var valorICMSInterestadual = 0;
        valorICMSInterestadual = valorTotalItem + valorFrete + valorSeguro + valorOutras - valorDesconto;
        valorICMSInterestadual = valorICMSInterestadual * (aliquotaInterestadual / 100);

        if (reducaoBase > 0)
            mva = mva * (reducaoBase / 100);

        base = (valorTotalItem + valorIPI + valorFrete + valorSeguro + valorOutras - valorDesconto);

        var baseICMSST = 0;
        baseICMSST = base * (1 + (mva / 100));

        var valorICMST = 0;
        valorICMST = (baseICMSST * (aliquotaInterna / 100)) - valorICMSInterestadual;

        instancia.ValorICMSST = Globalize.format(valorICMST, "n2");
        instancia.BaseICMSST = Globalize.format(baseICMSST, "n2");
    } else
        instancia.ValorICMSST = "0,00";
}