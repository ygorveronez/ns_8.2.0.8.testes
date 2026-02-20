/// <reference path="NFe.js" />
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

var ListaProdutoServico = function (nfe) {

    var instancia = this;

    this.Frete = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Frete:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.Seguro = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Seguro:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.Outras = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Outras:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.Desconto = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Desconto:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.NumeroOrdemCompra = PropertyEntity({ text: "Nº Ordem Compra:", getType: typesKnockout.string, maxlength: 15, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroItemOrdemCompra = PropertyEntity({ text: "Nº Item O.C.:", getType: typesKnockout.string, maxlength: 6, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.ItensNFe = PropertyEntity({ type: types.local, id: guid() });

    this.Load = function () {
        nfe.ProdutosServicos = new Array();

        KoBindings(instancia, nfe.IdKnockoutListaProdutoServico);

        var editarItem = { descricao: "Editar", id: guid(), metodo: instancia.Editar, icone: "" };
        var excluirItem = { descricao: "Excluir", id: guid(), metodo: instancia.Excluir, icone: "" };

        var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [editarItem, excluirItem] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "CodigoProduto", visible: false },
            { data: "CodigoServico", visible: false },
            { data: "CodigoCFOP", visible: false },
            { data: "CodigoCSTICMS", visible: false },
            { data: "OrigemMercadoria", visible: false },
            { data: "ValorFrete", visible: false },
            { data: "ValorSeguro", visible: false },
            { data: "ValorOutras", visible: false },
            { data: "ValorDesconto", visible: false },
            { data: "NumeroOrdemCompra", visible: false },
            { data: "NumeroItemOrdemCompra", visible: false },

            { data: "AliquotaRemICMSRet", visible: false },
            { data: "ValorICMSMonoRet", visible: false },

            { data: "BCICMS", visible: false },
            { data: "ReducaoBCICMS", visible: false },
            { data: "AliquotaICMS", visible: false },
            { data: "ValorICMS", visible: false },
            { data: "BCICMSEfetivo", visible: false },
            { data: "AliquotaICMSEfetivo", visible: false },
            { data: "ReducaoBCICMSEfetivo", visible: false },
            { data: "ValorICMSEfetivo", visible: false },

            { data: "BCICMSDestino", visible: false },
            { data: "AliquotaICMSDestino", visible: false },
            { data: "AliquotaICMSInterno", visible: false },
            { data: "PercentualPartilha", visible: false },
            { data: "ValorICMSDestino", visible: false },
            { data: "ValorICMSRemetente", visible: false },
            { data: "AliquotaFCP", visible: false },
            { data: "ValorFCP", visible: false },

            { data: "BCICMSST", visible: false },
            { data: "ReducaoBCICMSST", visible: false },
            { data: "PercentualMVA", visible: false },
            { data: "AliquotaICMSST", visible: false },
            { data: "AliquotaInterestadual", visible: false },
            { data: "BCICMSSTRetido", visible: false },
            { data: "AliquotaICMSSTRetido", visible: false },
            { data: "ValorICMSSTSubstituto", visible: false },
            { data: "ValorICMSSTRetido", visible: false },
            { data: "BCICMSSTDestino", visible: false },
            { data: "ValorICMSSTDestino", visible: false },

            { data: "CodigoCSTPIS", visible: false },
            { data: "BCPIS", visible: false },
            { data: "ReducaoBCPIS", visible: false },
            { data: "AliquotaPIS", visible: false },
            { data: "ValorPIS", visible: false },

            { data: "CodigoCSTCOFINS", visible: false },
            { data: "BCCOFINS", visible: false },
            { data: "ReducaoBCCOFINS", visible: false },
            { data: "AliquotaCOFINS", visible: false },
            { data: "ValorCOFINS", visible: false },

            { data: "CodigoCSTIPI", visible: false },
            { data: "BCIPI", visible: false },
            { data: "ReducaoBCIPI", visible: false },
            { data: "AliquotaIPI", visible: false },

            { data: "BCISS", visible: false },
            { data: "AliquotaISS", visible: false },
            { data: "ValorISS", visible: false },
            { data: "BaseDeducaoISS", visible: false },
            { data: "OutrasRetencoesISS", visible: false },
            { data: "DescontoIncondicional", visible: false },
            { data: "DescontoCondicional", visible: false },
            { data: "RetencaoISS", visible: false },
            { data: "CodigoExigibilidadeISS", visible: false },
            { data: "GeraIncentivoFiscal", visible: false },
            { data: "ProcessoJudicial", visible: false },

            { data: "BCII", visible: false },
            { data: "DespesaII", visible: false },
            { data: "ValorII", visible: false },
            { data: "ValorIOFII", visible: false },
            { data: "NumeroDocumentoII", visible: false },
            { data: "DataRegistroII", visible: false },
            { data: "LocalDesembaracoII", visible: false },
            { data: "EstadoDesembaracoII", visible: false },
            { data: "DataDesembaracoII", visible: false },
            { data: "CNPJAdquirente", visible: false },
            { data: "CodigoViaTransporteII", visible: false },
            { data: "ValorFreteMaritimoII", visible: false },
            { data: "CodigoIntermediacaoII", visible: false },

            { data: "ValorICMSDesonerado", visible: false },
            { data: "CodigoMotivoDesoneracao", visible: false },
            { data: "ValorICMSOperacao", visible: false },
            { data: "AliquotaICMSOperacao", visible: false },
            { data: "ValorICMSDeferido", visible: false },

            { data: "Sequencial", title: "Sequência", width: "6%", orderable: true },
            { data: "Descricao", title: "Produto / Serviço", width: "30%", orderable: true },
            { data: "DescricaoUnidadeMedida", title: "Unidade de Medida", width: "5%" },
            { data: "CSTICMS", title: "CST / CSOSN", width: "5%" },
            { data: "CFOP", title: "CFOP", width: "10%" },
            { data: "Qtd", title: "Quantidade", width: "8%" },
            { data: "ValorUnitario", title: "Valor Unit.", width: "10%" },
            { data: "ValorTotal", title: "Total", width: "10%" },
            { data: "ValorST", title: "ST", width: "5%" },
            { data: "ValorIPI", title: "IPI", width: "5%" },
            { data: "CodigoItem", visible: false },
            { data: "DescricaoItem", visible: false },
            { data: "UnidadeMedida", visible: false },

            { data: "BaseFCPICMS", visible: false },
            { data: "PercentualFCPICMS", visible: false },
            { data: "ValorFCPICMS", visible: false },
            { data: "BaseFCPICMSST", visible: false },
            { data: "PercentualFCPICMSST", visible: false },
            { data: "ValorFCPICMSST", visible: false },
            { data: "AliquotaFCPICMSST", visible: false },
            { data: "BaseFCPDestino", visible: false },
            { data: "PercentualIPIDevolvido", visible: false },
            { data: "ValorIPIDevolvido", visible: false },

            { data: "InformacoesAdicionaisItem", visible: false },
            { data: "IndicadorEscalaRelevante", visible: false },
            { data: "CNPJFabricante", visible: false },
            { data: "CodigoBeneficioFiscal", visible: false },
            { data: "CodigoNFCI", visible: false },
            { data: "CodigoLocalArmazenamento", visible: false },
            { data: "LocalArmazenamento", visible: false },

            { data: "CodigoANP", visible: false },
            { data: "PercentualGLP", visible: false },
            { data: "PercentualGNN", visible: false },
            { data: "PercentualGNI", visible: false },
            { data: "PercentualOrigemComb", visible: false },
            { data: "PercentualMisturaBiodiesel", visible: false },
            { data: "ValorPartidaANP", visible: false },

            { data: "QuantidadeTributavel", visible: false },
            { data: "ValorUnitarioTributavel", visible: false },
            { data: "UnidadeDeMedidaTributavel", visible: false },
            { data: "CodigoEANTributavel", visible: false }
        ];

        nfe.GridProdutoServico = new BasicDataTable(instancia.ItensNFe.id, header, menuOpcoes, { column: 82, dir: orderDir.desc }, null, 5);

        instancia.RecarregarGrid();
    };

    this.DestivarListaProdutoServico = function () {
        DesabilitarCamposInstanciasNFe(instancia);
        nfe.GridProdutoServico.CarregarGrid(nfe.ProdutosServicos, false);
    };

    this.HabilitarListaProdutoServico = function () {
        HabilitarCamposInstanciasNFe(instancia);
    };

    this.Excluir = function (produtoServico) {

        if (nfe.ProdutoServico.Codigo.val() > 0) {
            exibirMensagem("atencao", "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
            return;
        }
        if (nfe.ProdutoServico.TipoItem.val() == 1 && nfe.ProdutoServico.Produto.val() != "") {
            exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um produto sem salvar o mesmo.");
            return;
        }
        if (nfe.ProdutoServico.TipoItem.val() == 2 && nfe.ProdutoServico.Servico.val() != "") {
            exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um serviço sem salvar o mesmo.");
            return;
        }

        exibirConfirmacao("Confirmação", "Realmente deseja excluir o item " + produtoServico.Descricao + "?", function () {
            for (var i = 0; i < nfe.ProdutosServicos.length; i++) {
                if (produtoServico.Codigo == nfe.ProdutosServicos[i].Codigo) {

                    instancia.RemoverValorItemTotalizador(produtoServico);
                    for (var j = 0; j < nfe.LotesProdutos.length; j++) {
                        if (nfe.LotesProdutos[j].CodigoItem == "" || nfe.LotesProdutos[j].CodigoItem == produtoServico.Codigo)
                            nfe.LotesProdutos.splice(j, 1);
                    }

                    nfe.ProdutosServicos.splice(i, 1);
                    break;
                }
            }
            instancia.RecarregarGrid();
        });
    };

    this.Editar = function (produtoServico) {

        if (nfe.ProdutoServico.Codigo.val() > 0) {
            exibirMensagem("atencao", "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
            return;
        }
        if (nfe.ProdutoServico.TipoItem.val() === 1 && nfe.ProdutoServico.Produto.val() != "") {
            exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um produto sem salvar o mesmo.");
            return;
        }
        if (nfe.ProdutoServico.TipoItem.val() === 2 && nfe.ProdutoServico.Servico.val() != "") {
            exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um serviço sem salvar o mesmo.");
            return;
        }

        LimparCampos(nfe.ProdutoServico);
        LimparCampos(nfe.ListaProdutoServico);
        LimparCampos(nfe.ImpostoICMS);
        LimparCampos(nfe.ImpostoICMSST);
        LimparCampos(nfe.ImpostoDIFAL);
        LimparCampos(nfe.ImpostoPIS);
        LimparCampos(nfe.ImpostoCOFINS);
        LimparCampos(nfe.ImpostoII);
        LimparCampos(nfe.ImpostoIPI);
        LimparCampos(nfe.ImpostoISS);
        LimparCampos(nfe.ImpostoICMSDesonerado);
        LimparCampos(nfe.InformacoesAdicionaisProdutoServico);
        LimparCampos(nfe.LoteProduto);
        LimparCampos(nfe.CombustivelProduto);
        LimparCampos(nfe.ExportacaoProdutoServico);

        instancia.RemoverValorItemTotalizador(produtoServico);

        nfe.Totalizador.ValorFrete.enable(false);
        nfe.Totalizador.ValorSeguro.enable(false);
        nfe.Totalizador.ValorDesconto.enable(false);
        nfe.Totalizador.ValorOutrasDespesas.enable(false);

        if (produtoServico.CodigoServico > 0)
            nfe.ProdutoServico.TipoItem.val(2);
        else if (produtoServico.CodigoProduto > 0)
            nfe.ProdutoServico.TipoItem.val(1);
        nfe.ProdutoServico.TipoItemChange();

        nfe.ProdutoServico.Codigo.val(produtoServico.Codigo);
        nfe.ProdutoServico.Produto.codEntity(produtoServico.CodigoProduto);
        nfe.ProdutoServico.Servico.codEntity(produtoServico.CodigoServico);
        nfe.ProdutoServico.CFOP.codEntity(produtoServico.CodigoCFOP);
        nfe.ProdutoServico.CSTICMS.val(produtoServico.CodigoCSTICMS);
        nfe.ProdutoServico.OrigemMercadoria.val(produtoServico.OrigemMercadoria);
        nfe.ProdutoServico.CSTICMSChange();
        if (produtoServico.CodigoProduto > 0)
            nfe.ProdutoServico.Produto.val(produtoServico.Descricao);
        else if (produtoServico.CodigoServico > 0)
            nfe.ProdutoServico.Servico.val(produtoServico.Descricao);
        nfe.ProdutoServico.CFOP.val(produtoServico.CFOP);
        nfe.ProdutoServico.CodigoItem.val(produtoServico.CodigoItem);
        nfe.ProdutoServico.DescricaoItem.val(produtoServico.DescricaoItem);
        nfe.ProdutoServico.UnidadeMedida.val(produtoServico.UnidadeMedida);
        nfe.ProdutoServico.Quantidade.val(Globalize.format(produtoServico.Qtd, "n" + nfe.NFe.CasasQuantidadeProdutoNFe.val()));
        nfe.ProdutoServico.ValorUnitario.val(Globalize.format(produtoServico.ValorUnitario, "n" + + nfe.NFe.CasasValorProdutoNFe.val()));
        nfe.ProdutoServico.ValorTotalItem.val(Globalize.format(produtoServico.ValorTotal, "n2"));

        nfe.ListaProdutoServico.Frete.val(Globalize.format(produtoServico.ValorFrete, "n2"));
        nfe.ListaProdutoServico.Seguro.val(Globalize.format(produtoServico.ValorSeguro, "n2"));
        nfe.ListaProdutoServico.Outras.val(Globalize.format(produtoServico.ValorOutras, "n2"));
        nfe.ListaProdutoServico.Desconto.val(Globalize.format(produtoServico.ValorDesconto, "n2"));
        nfe.ListaProdutoServico.NumeroOrdemCompra.val(produtoServico.NumeroOrdemCompra);
        nfe.ListaProdutoServico.NumeroItemOrdemCompra.val(produtoServico.NumeroItemOrdemCompra);

        nfe.ImpostoICMSMonoRet.AliquotaRemICMSRet.val(Globalize.format(produtoServico.AliquotaRemICMSRet, "n2"));
        nfe.ImpostoICMSMonoRet.ValorICMSMonoRet.val(Globalize.format(produtoServico.ValorICMSMonoRet, "n2"));

        nfe.ImpostoICMS.BaseICMS.val(Globalize.format(produtoServico.BCICMS, "n2"));
        nfe.ImpostoICMS.ReducaoBaseICMS.val(Globalize.format(produtoServico.ReducaoBCICMS, "n4"));
        nfe.ImpostoICMS.AliquotaICMS.val(Globalize.format(produtoServico.AliquotaICMS, "n2"));
        nfe.ImpostoICMS.ValorICMS.val(Globalize.format(produtoServico.ValorICMS, "n2"));
        nfe.ImpostoICMS.BaseFCPICMS.val(Globalize.format(produtoServico.BaseFCPICMS, "n2"));
        nfe.ImpostoICMS.PercentualFCPICMS.val(Globalize.format(produtoServico.PercentualFCPICMS, "n2"));
        nfe.ImpostoICMS.ValorFCPICMS.val(Globalize.format(produtoServico.ValorFCPICMS, "n2"));
        nfe.ImpostoICMS.BCICMSEfetivo.val(Globalize.format(produtoServico.BCICMSEfetivo, "n2"));
        nfe.ImpostoICMS.AliquotaICMSEfetivo.val(Globalize.format(produtoServico.AliquotaICMSEfetivo, "n2"));
        nfe.ImpostoICMS.ReducaoBCICMSEfetivo.val(Globalize.format(produtoServico.ReducaoBCICMSEfetivo, "n2"));
        nfe.ImpostoICMS.ValorICMSEfetivo.val(Globalize.format(produtoServico.ValorICMSEfetivo, "n2"));

        nfe.ImpostoDIFAL.BaseICMSDestino.val(Globalize.format(produtoServico.BCICMSDestino, "n2"));
        nfe.ImpostoDIFAL.AliquotaICMSDestino.val(Globalize.format(produtoServico.AliquotaICMSDestino, "n2"));
        nfe.ImpostoDIFAL.AliquotaICMSInterno.val(Globalize.format(produtoServico.AliquotaICMSInterno, "n2"));
        nfe.ImpostoDIFAL.PercentualPartilha.val(Globalize.format(produtoServico.PercentualPartilha, "n2"));
        nfe.ImpostoDIFAL.ValorICMSDestino.val(Globalize.format(produtoServico.ValorICMSDestino, "n2"));
        nfe.ImpostoDIFAL.ValorICMSRemetente.val(Globalize.format(produtoServico.ValorICMSRemetente, "n2"));
        nfe.ImpostoDIFAL.BaseFCPDestino.val(Globalize.format(produtoServico.BaseFCPDestino, "n2"));
        nfe.ImpostoDIFAL.PercentualFCP.val(Globalize.format(produtoServico.AliquotaFCP, "n2"));
        nfe.ImpostoDIFAL.ValorFCP.val(Globalize.format(produtoServico.ValorFCP, "n2"));

        nfe.ImpostoICMSST.BaseICMSST.val(Globalize.format(produtoServico.BCICMSST, "n2"));
        nfe.ImpostoICMSST.ReducaoBaseICMSST.val(Globalize.format(produtoServico.ReducaoBCICMSST, "n2"));
        nfe.ImpostoICMSST.PercentualMVA.val(Globalize.format(produtoServico.PercentualMVA, "n2"));
        nfe.ImpostoICMSST.AliquotaICMSST.val(Globalize.format(produtoServico.AliquotaICMSST, "n2"));
        nfe.ImpostoICMSST.AliquotaInterestadual.val(Globalize.format(produtoServico.AliquotaInterestadual, "n2"));
        nfe.ImpostoICMSST.ValorICMSST.val(Globalize.format(produtoServico.ValorST, "n2"));
        nfe.ImpostoICMSST.BaseFCPICMSST.val(Globalize.format(produtoServico.BaseFCPICMSST, "n2"));
        nfe.ImpostoICMSST.PercentualFCPICMSST.val(Globalize.format(produtoServico.PercentualFCPICMSST, "n2"));
        nfe.ImpostoICMSST.ValorFCPICMSST.val(Globalize.format(produtoServico.ValorFCPICMSST, "n2"));
        nfe.ImpostoICMSST.AliquotaFCPICMSST.val(Globalize.format(produtoServico.AliquotaFCPICMSST, "n2"));
        nfe.ImpostoICMSST.BCICMSSTRetido.val(Globalize.format(produtoServico.BCICMSSTRetido, "n2"));
        nfe.ImpostoICMSST.AliquotaICMSSTRetido.val(Globalize.format(produtoServico.AliquotaICMSSTRetido, "n2"));
        nfe.ImpostoICMSST.ValorICMSSTSubstituto.val(Globalize.format(produtoServico.ValorICMSSTSubstituto, "n2"));
        nfe.ImpostoICMSST.ValorICMSSTRetido.val(Globalize.format(produtoServico.ValorICMSSTRetido, "n2"));
        nfe.ImpostoICMSST.BCICMSSTDestino.val(Globalize.format(produtoServico.BCICMSSTDestino, "n2"));
        nfe.ImpostoICMSST.ValorICMSSTDestino.val(Globalize.format(produtoServico.ValorICMSSTDestino, "n2"));

        nfe.ImpostoPIS.CSTPIS.val(produtoServico.CodigoCSTPIS);
        nfe.ImpostoPIS.CSTPISChange();
        nfe.ImpostoPIS.BasePIS.val(Globalize.format(produtoServico.BCPIS, "n2"));
        nfe.ImpostoPIS.ReducaoBasePIS.val(Globalize.format(produtoServico.ReducaoBCPIS, "n2"));
        nfe.ImpostoPIS.AliquotaPIS.val(Globalize.format(produtoServico.AliquotaPIS, "n2"));
        nfe.ImpostoPIS.ValorPIS.val(Globalize.format(produtoServico.ValorPIS, "n2"));

        nfe.ImpostoCOFINS.CSTCOFINS.val(produtoServico.CodigoCSTCOFINS);
        nfe.ImpostoCOFINS.CSTCOFINSChange();
        nfe.ImpostoCOFINS.BaseCOFINS.val(Globalize.format(produtoServico.BCCOFINS, "n2"));
        nfe.ImpostoCOFINS.ReducaoBaseCOFINS.val(Globalize.format(produtoServico.ReducaoBCCOFINS, "n2"));
        nfe.ImpostoCOFINS.AliquotaCOFINS.val(Globalize.format(produtoServico.AliquotaCOFINS, "n2"));
        nfe.ImpostoCOFINS.ValorCOFINS.val(Globalize.format(produtoServico.ValorCOFINS, "n2"));

        nfe.ImpostoIPI.CSTIPI.val(produtoServico.CodigoCSTIPI);
        nfe.ImpostoIPI.CSTIPIChange();
        nfe.ImpostoIPI.BaseIPI.val(Globalize.format(produtoServico.BCIPI, "n2"));
        nfe.ImpostoIPI.ReducaoBaseIPI.val(Globalize.format(produtoServico.ReducaoBCIPI, "n2"));
        nfe.ImpostoIPI.AliquotaIPI.val(Globalize.format(produtoServico.AliquotaIPI, "n2"));
        nfe.ImpostoIPI.ValorIPI.val(Globalize.format(produtoServico.ValorIPI, "n2"));
        nfe.ImpostoIPI.PercentualIPIDevolvido.val(Globalize.format(produtoServico.PercentualIPIDevolvido, "n2"));
        nfe.ImpostoIPI.ValorIPIDevolvido.val(Globalize.format(produtoServico.ValorIPIDevolvido, "n2"));

        nfe.ImpostoISS.BaseISS.val(Globalize.format(produtoServico.BCISS, "n2"));
        nfe.ImpostoISS.AliquotaISS.val(Globalize.format(produtoServico.AliquotaISS, "n2"));
        nfe.ImpostoISS.ValorISS.val(Globalize.format(produtoServico.ValorISS, "n2"));
        nfe.ImpostoISS.BaseDeducaoISS.val(Globalize.format(produtoServico.BaseDeducaoISS, "n2"));
        nfe.ImpostoISS.OutrasRetencoesISS.val(Globalize.format(produtoServico.OutrasRetencoesISS, "n2"));
        nfe.ImpostoISS.DescontoIncondicionalISS.val(Globalize.format(produtoServico.DescontoIncondicional, "n2"));
        nfe.ImpostoISS.DescontoCondicionalISS.val(Globalize.format(produtoServico.DescontoCondicional, "n2"));
        nfe.ImpostoISS.RetencaoISS.val(Globalize.format(produtoServico.RetencaoISS, "n2"));
        nfe.ImpostoISS.ExigibilidadeISS.val(produtoServico.CodigoExigibilidadeISS);
        nfe.ImpostoISS.IncentivoFiscal.val(produtoServico.GeraIncentivoFiscal);
        nfe.ImpostoISS.ProcessoJudicialISS.val(produtoServico.ProcessoJudicial);

        nfe.ImpostoII.BaseII.val(Globalize.format(produtoServico.BCII, "n2"));
        nfe.ImpostoII.DespesaII.val(Globalize.format(produtoServico.DespesaII, "n2"));
        nfe.ImpostoII.ValorII.val(Globalize.format(produtoServico.ValorII, "n2"));
        nfe.ImpostoII.ValorIOFII.val(Globalize.format(produtoServico.ValorIOFII, "n2"));
        nfe.ImpostoII.NumeroDocumentoII.val(produtoServico.NumeroDocumentoII);
        nfe.ImpostoII.DataRegistroII.val(produtoServico.DataRegistroII);
        nfe.ImpostoII.LocalDesembaracoII.val(produtoServico.LocalDesembaracoII);
        nfe.ImpostoII.EstadoDesembaracoII.val(produtoServico.EstadoDesembaracoII);
        nfe.ImpostoII.DataDesembaracoII.val(produtoServico.DataDesembaracoII);
        nfe.ImpostoII.CNPJAdquirenteII.val(produtoServico.CNPJAdquirente);
        nfe.ImpostoII.ViaTransporteII.val(produtoServico.CodigoViaTransporteII);
        nfe.ImpostoII.ValorFreteMarinhoII.val(Globalize.format(produtoServico.ValorFreteMaritimoII, "n2"));
        nfe.ImpostoII.IntermediacaoII.val(produtoServico.CodigoIntermediacaoII);

        nfe.ImpostoICMSDesonerado.ValorICMSDesonerado.val(Globalize.format(produtoServico.ValorICMSDesonerado, "n2"));
        nfe.ImpostoICMSDesonerado.MotivoICMSDesonerado.val(produtoServico.CodigoMotivoDesoneracao);
        nfe.ImpostoICMSDesonerado.ValorICMSOperacao.val(Globalize.format(produtoServico.ValorICMSOperacao, "n2"));
        nfe.ImpostoICMSDesonerado.AliquotaICMSOperacao.val(Globalize.format(produtoServico.AliquotaICMSOperacao, "n2"));
        nfe.ImpostoICMSDesonerado.ValorICMSDeferido.val(Globalize.format(produtoServico.ValorICMSDeferido, "n2"));

        nfe.InformacoesAdicionaisProdutoServico.InformacoesAdicionaisItem.val(produtoServico.InformacoesAdicionaisItem);
        nfe.InformacoesAdicionaisProdutoServico.IndicadorEscalaRelevante.val(produtoServico.IndicadorEscalaRelevante);
        nfe.InformacoesAdicionaisProdutoServico.CNPJFabricante.val(produtoServico.CNPJFabricante);
        nfe.InformacoesAdicionaisProdutoServico.CodigoBeneficioFiscal.val(produtoServico.CodigoBeneficioFiscal);
        nfe.InformacoesAdicionaisProdutoServico.QuantidadeTributavel.val(Globalize.format(produtoServico.QuantidadeTributavel, "n" + nfe.NFe.CasasQuantidadeProdutoNFe.val()));
        nfe.InformacoesAdicionaisProdutoServico.ValorUnitarioTributavel.val(Globalize.format(produtoServico.ValorUnitarioTributavel, "n" + + nfe.NFe.CasasValorProdutoNFe.val()));
        nfe.InformacoesAdicionaisProdutoServico.UnidadeDeMedidaTributavel.val(produtoServico.UnidadeDeMedidaTributavel);
        nfe.InformacoesAdicionaisProdutoServico.CodigoEANTributavel.val(produtoServico.CodigoEANTributavel);
        nfe.InformacoesAdicionaisProdutoServico.CodigoNFCI.val(produtoServico.CodigoNFCI);
        nfe.InformacoesAdicionaisProdutoServico.LocalArmazenamento.codEntity(produtoServico.CodigoLocalArmazenamento);
        nfe.InformacoesAdicionaisProdutoServico.LocalArmazenamento.val(produtoServico.LocalArmazenamento);

        nfe.CombustivelProduto.CodigoANP.val(produtoServico.CodigoANP);
        nfe.CombustivelProduto.PercentualGLP.val(Globalize.format(produtoServico.PercentualGLP, "n4"));
        nfe.CombustivelProduto.PercentualGNN.val(Globalize.format(produtoServico.PercentualGNN, "n4"));
        nfe.CombustivelProduto.PercentualGNI.val(Globalize.format(produtoServico.PercentualGNI, "n4"));
        nfe.CombustivelProduto.PercentualOrigemComb.val(Globalize.format(produtoServico.PercentualOrigemComb, "n4"));        
        nfe.CombustivelProduto.PercentualMisturaBiodiesel.val(Globalize.format(produtoServico.PercentualMisturaBiodiesel, "n4"));  
        nfe.CombustivelProduto.ValorPartidaANP.val(Globalize.format(produtoServico.ValorPartidaANP, "n2"));

        nfe.ExportacaoProdutoServico.NumeroDrawback.val(produtoServico.NumeroDrawback);
        nfe.ExportacaoProdutoServico.NumeroRegistroExportacao.val(produtoServico.NumeroRegistroExportacao);
        nfe.ExportacaoProdutoServico.ChaveAcessoExportacao.val(produtoServico.ChaveAcessoExportacao);

        nfe.LoteProduto.CodigoItem.val(produtoServico.Codigo);
        nfe.LoteProduto.RecarregarGrid();

        $("#tabItensNFe_" + nfe.NFe.IdModal.val() + " a:eq(0)").tab("show");
        $("#" + nfe.ProdutoServico.TipoItem.id).focus();
    };

    this.RemoverValorItemTotalizador = function (produtoServico) {
        for (var i = 0; i < nfe.ProdutosServicos.length; i++) {
            if (produtoServico.Codigo == nfe.ProdutosServicos[i].Codigo) {

                var baseICMSItem = Globalize.parseFloat(produtoServico.BCICMS);
                var baseICMSTotal = Globalize.parseFloat(nfe.Totalizador.BaseICMS.val());
                var baseICMS = baseICMSTotal - baseICMSItem;
                nfe.Totalizador.BaseICMS.val(Globalize.format(baseICMS, "n2"));

                var valorICMSItem = Globalize.parseFloat(produtoServico.ValorICMS);
                var valorICMSTotal = Globalize.parseFloat(nfe.Totalizador.ValorICMS.val());
                var valorICMS = valorICMSTotal - valorICMSItem;
                nfe.Totalizador.ValorICMS.val(Globalize.format(valorICMS, "n2"));

                var valorICMSDesoneradoItem = Globalize.parseFloat(produtoServico.ValorICMSDesonerado);
                var valorICMSDesoneradoTotal = Globalize.parseFloat(nfe.Totalizador.ValorICMSDesonerado.val());
                var valorICMSDesonerado = valorICMSDesoneradoTotal - valorICMSDesoneradoItem;
                nfe.Totalizador.ValorICMSDesonerado.val(Globalize.format(valorICMSDesonerado, "n2"));

                var valorIIItem = Globalize.parseFloat(produtoServico.ValorII);
                var valorIITotal = Globalize.parseFloat(nfe.Totalizador.ValorII.val());
                var valorII = valorIITotal - valorIIItem;
                nfe.Totalizador.ValorII.val(Globalize.format(valorII, "n2"));

                var baseICMSSTItem = Globalize.parseFloat(produtoServico.BCICMSST);
                var baseICMSSTTotal = Globalize.parseFloat(nfe.Totalizador.BaseICMSST.val());
                var baseICMSST = baseICMSSTTotal - baseICMSSTItem;
                nfe.Totalizador.BaseICMSST.val(Globalize.format(baseICMSST, "n2"));

                var valorICMSSTItem = Globalize.parseFloat(produtoServico.ValorST);
                var valorICMSSTTotal = Globalize.parseFloat(nfe.Totalizador.ValorICMSST.val());
                var valorICMSST = valorICMSSTTotal - valorICMSSTItem;
                nfe.Totalizador.ValorICMSST.val(Globalize.format(valorICMSST, "n2"));

                var valorTotalItem = Globalize.parseFloat(produtoServico.ValorTotal);
                if (produtoServico.CodigoProduto > 0) {
                    var valorTotalProdutosTotal = Globalize.parseFloat(nfe.Totalizador.ValorTotalProdutos.val());
                    var valorTotalProdutos = valorTotalProdutosTotal - valorTotalItem;
                    nfe.Totalizador.ValorTotalProdutos.val(Globalize.format(valorTotalProdutos, "n2"));
                }
                else {
                    var valorTotalServicosTotal = Globalize.parseFloat(nfe.Totalizador.ValorTotalServicos.val());
                    var valorTotalServicos = valorTotalServicosTotal - valorTotalItem;
                    nfe.Totalizador.ValorTotalServicos.val(Globalize.format(valorTotalServicos, "n2"));
                }

                var valorFreteItem = Globalize.parseFloat(produtoServico.ValorFrete);
                var valorFreteTotal = Globalize.parseFloat(nfe.Totalizador.ValorFrete.val());
                var valorFrete = valorFreteTotal - valorFreteItem;
                nfe.Totalizador.ValorFrete.val(Globalize.format(valorFrete, "n2"));

                var valorSeguroItem = Globalize.parseFloat(produtoServico.ValorSeguro);
                var valorSeguroTotal = Globalize.parseFloat(nfe.Totalizador.ValorSeguro.val());
                var valorSeguro = valorSeguroTotal - valorSeguroItem;
                nfe.Totalizador.ValorSeguro.val(Globalize.format(valorSeguro, "n2"));

                var valorDescontoItem = Globalize.parseFloat(produtoServico.ValorDesconto);
                var valorDescontoTotal = Globalize.parseFloat(nfe.Totalizador.ValorDesconto.val());
                var valorDesconto = valorDescontoTotal - valorDescontoItem;
                nfe.Totalizador.ValorDesconto.val(Globalize.format(valorDesconto, "n2"));

                var valorOutrasDespesasItem = Globalize.parseFloat(produtoServico.ValorOutras);
                var valorOutrasDespesasTotal = Globalize.parseFloat(nfe.Totalizador.ValorOutrasDespesas.val());
                var valorOutrasDespesas = valorOutrasDespesasTotal - valorOutrasDespesasItem;
                nfe.Totalizador.ValorOutrasDespesas.val(Globalize.format(valorOutrasDespesas, "n2"));

                var valorIPIItem = Globalize.parseFloat(produtoServico.ValorIPI);
                var valorIPITotal = Globalize.parseFloat(nfe.Totalizador.ValorIPI.val());
                var valorIPI = valorIPITotal - valorIPIItem;
                nfe.Totalizador.ValorIPI.val(Globalize.format(valorIPI, "n2"));

                var valorFCPICMSItem = Globalize.parseFloat(produtoServico.ValorFCPICMS);
                var valorFCPICMSTotal = Globalize.parseFloat(nfe.Totalizador.ValorFCPICMS.val());
                var valorFCPICMS = valorFCPICMSTotal - valorFCPICMSItem;
                nfe.Totalizador.ValorFCPICMS.val(Globalize.format(valorFCPICMS, "n2"));

                var valorFCPICMSSTItem = Globalize.parseFloat(produtoServico.ValorFCPICMSST);
                var valorFCPICMSSTTotal = Globalize.parseFloat(nfe.Totalizador.ValorFCPICMSST.val());
                var valorFCPICMSST = valorFCPICMSSTTotal - valorFCPICMSSTItem;
                nfe.Totalizador.ValorFCPICMSST.val(Globalize.format(valorFCPICMSST, "n2"));

                var valorIPIDevolvidoItem = Globalize.parseFloat(produtoServico.ValorIPIDevolvido);
                var valorIPIDevolvidoTotal = Globalize.parseFloat(nfe.Totalizador.ValorIPIDevolvido.val());
                var valorIPIDevolvido = valorIPIDevolvidoTotal - valorIPIDevolvidoItem;
                nfe.Totalizador.ValorIPIDevolvido.val(Globalize.format(valorIPIDevolvido, "n2"));

                var baseISSItem = Globalize.parseFloat(produtoServico.BCISS);
                var baseISSTotal = Globalize.parseFloat(nfe.Totalizador.BaseISS.val());
                var baseISS = baseISSTotal - baseISSItem;
                nfe.Totalizador.BaseISS.val(Globalize.format(baseISS, "n2"));

                var valorISSItem = Globalize.parseFloat(produtoServico.ValorISS);
                var valorISSTotal = Globalize.parseFloat(nfe.Totalizador.ValorISS.val());
                var valorISS = valorISSTotal - valorISSItem;
                nfe.Totalizador.ValorISS.val(Globalize.format(valorISS, "n2"));

                var baseDeducaoItem = Globalize.parseFloat(produtoServico.BaseDeducaoISS);
                var baseDeducaoTotal = Globalize.parseFloat(nfe.Totalizador.BaseDeducao.val());
                var baseDeducao = baseDeducaoTotal - baseDeducaoItem;
                nfe.Totalizador.BaseDeducao.val(Globalize.format(baseDeducao, "n2"));

                var valorOutrasRetencoesItem = Globalize.parseFloat(produtoServico.OutrasRetencoesISS);
                var valorOutrasRetencoesTotal = Globalize.parseFloat(nfe.Totalizador.ValorOutrasRetencoes.val());
                var valorOutrasRetencoes = valorOutrasRetencoesTotal - valorOutrasRetencoesItem;
                nfe.Totalizador.ValorOutrasRetencoes.val(Globalize.format(valorOutrasRetencoes, "n2"));

                var valorDescontoIncondicionalItem = Globalize.parseFloat(produtoServico.DescontoIncondicional);
                var valorDescontoIncondicionalTotal = Globalize.parseFloat(nfe.Totalizador.ValorDescontoIncondicional.val());
                var valorDescontoIncondicional = valorDescontoIncondicionalTotal - valorDescontoIncondicionalItem;
                nfe.Totalizador.ValorDescontoIncondicional.val(Globalize.format(valorDescontoIncondicional, "n2"));

                var valorDescontoCondicionalItem = Globalize.parseFloat(produtoServico.DescontoCondicional);
                var valorDescontoCondicionalTotal = Globalize.parseFloat(nfe.Totalizador.ValorDescontoCondicional.val());
                var valorDescontoCondicional = valorDescontoCondicionalTotal - valorDescontoCondicionalItem;
                nfe.Totalizador.ValorDescontoCondicional.val(Globalize.format(valorDescontoCondicional, "n2"));

                var valorRetencaoISSItem = Globalize.parseFloat(produtoServico.RetencaoISS);
                var valorRetencaoISSTotal = Globalize.parseFloat(nfe.Totalizador.ValorRetencaoISS.val());
                var valorRetencaoISS = valorRetencaoISSTotal - valorRetencaoISSItem;
                nfe.Totalizador.ValorRetencaoISS.val(Globalize.format(valorRetencaoISS, "n2"));

                var basePISItem = Globalize.parseFloat(produtoServico.BCPIS);
                var basePISTotal = Globalize.parseFloat(nfe.Totalizador.BasePIS.val());
                var basePIS = basePISTotal - basePISItem;
                nfe.Totalizador.BasePIS.val(Globalize.format(basePIS, "n2"));

                var valorPISItem = Globalize.parseFloat(produtoServico.ValorPIS);
                var valorPISTotal = Globalize.parseFloat(nfe.Totalizador.ValorPIS.val());
                var valorPIS = valorPISTotal - valorPISItem;
                nfe.Totalizador.ValorPIS.val(Globalize.format(valorPIS, "n2"));

                var baseCOFINSItem = Globalize.parseFloat(produtoServico.BCCOFINS);
                var baseCOFINSTotal = Globalize.parseFloat(nfe.Totalizador.BaseCOFINS.val());
                var baseCOFINS = baseCOFINSTotal - baseCOFINSItem;
                nfe.Totalizador.BaseCOFINS.val(Globalize.format(baseCOFINS, "n2"));

                var valorCOFINSItem = Globalize.parseFloat(produtoServico.ValorCOFINS);
                var valorCOFINSTotal = Globalize.parseFloat(nfe.Totalizador.ValorCOFINS.val());
                var valorCOFINS = valorCOFINSTotal - valorCOFINSItem;
                nfe.Totalizador.ValorCOFINS.val(Globalize.format(valorCOFINS, "n2"));

                var valorFCPItem = Globalize.parseFloat(produtoServico.ValorFCP);
                var valorFCPTotal = Globalize.parseFloat(nfe.Totalizador.ValorFCP.val());
                var valorFCP = valorFCPTotal - valorFCPItem;
                nfe.Totalizador.ValorFCP.val(Globalize.format(valorFCP, "n2"));

                //DIFAL
                var valorICMSDestinoItem = Globalize.parseFloat(produtoServico.ValorICMSDestino);
                var valorICMSDestinoTotal = Globalize.parseFloat(nfe.Totalizador.ValorICMSDestino.val());
                var valorICMSDestino = valorICMSDestinoTotal - valorICMSDestinoItem;
                nfe.Totalizador.ValorICMSDestino.val(Globalize.format(valorICMSDestino, "n2"));

                var valorICMSRemetenteItem = Globalize.parseFloat(produtoServico.ValorICMSRemetente);
                var valorICMSRemetenteTotal = Globalize.parseFloat(nfe.Totalizador.ValorICMSRemetente.val());
                var valorICMSRemetente = valorICMSRemetenteTotal - valorICMSRemetenteItem;
                nfe.Totalizador.ValorICMSRemetente.val(Globalize.format(valorICMSRemetente, "n2"));

                //DRCST
                var bCICMSSTRetidoItem = Globalize.parseFloat(produtoServico.BCICMSSTRetido);
                var bCICMSSTRetidoTotal = Globalize.parseFloat(nfe.Totalizador.BCICMSSTRetido.val());
                nfe.Totalizador.BCICMSSTRetido.val(Globalize.format(bCICMSSTRetidoTotal - bCICMSSTRetidoItem, "n2"));

                var valorICMSSTRetidoItem = Globalize.parseFloat(produtoServico.ValorICMSSTRetido);
                var valorICMSSTRetidoTotal = Globalize.parseFloat(nfe.Totalizador.ValorICMSSTRetido.val());
                nfe.Totalizador.ValorICMSSTRetido.val(Globalize.format(valorICMSSTRetidoTotal - valorICMSSTRetidoItem, "n2"));

                //TOTAL NF
                var valorFrete = Globalize.parseFloat(produtoServico.ValorFrete);
                var valorSeguro = Globalize.parseFloat(produtoServico.ValorSeguro);
                var valorDesconto = Globalize.parseFloat(produtoServico.ValorDesconto);
                var valorICMSDesonerado = Globalize.parseFloat(produtoServico.ValorICMSDesonerado);
                var valorOutrasDespesas = Globalize.parseFloat(produtoServico.ValorOutras);
                var valorIPI = Globalize.parseFloat(produtoServico.ValorIPI);
                var valorICMSST = Globalize.parseFloat(produtoServico.ValorST);
                var valorII = Globalize.parseFloat(produtoServico.ValorII);
                var valorFCPICMSST = Globalize.parseFloat(produtoServico.ValorFCPICMSST);
                var ValorIPIDevolvido = Globalize.parseFloat(produtoServico.ValorIPIDevolvido);

                var valorTotalNFeItem = Globalize.parseFloat(produtoServico.ValorTotal);
                var valorTotalNFeTotal = Globalize.parseFloat(nfe.Totalizador.ValorTotalNFe.val());

                var valorTotalNFe = valorTotalNFeTotal - valorTotalNFeItem - valorICMSST - valorIPI - valorOutrasDespesas + valorDesconto + valorICMSDesonerado - valorSeguro - valorFrete - valorII - valorFCPICMSST - ValorIPIDevolvido;
                nfe.Totalizador.ValorTotalNFe.val(Globalize.format(valorTotalNFe, "n2"));

                break;
            }
        }
    };

    this.RecarregarGrid = function () {
        nfe.GridProdutoServico.CarregarGrid(nfe.ProdutosServicos);
    };

    this.ValorDescontoChange = function () {
        nfe.ImpostoICMS.CalcularImpostoICMSInstancia();
    };
};

function DescontoChange(instancia) {
    instancia.ValorDescontoChange();
}