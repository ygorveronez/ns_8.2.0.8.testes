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
/// <reference path="NFe.js" />

var Totalizador = function (nfe) {

    var instancia = this;

    this.BaseICMS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "BC ICMS:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorICMS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ICMS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorICMSDesonerado = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ICMS Desonerado:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorII = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor II:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.BaseICMSST = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "BC ICMS ST:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorICMSST = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "ICMS ST:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorTotalProdutos = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Total Produtos:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });

    this.ValorFrete = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Frete:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.RatearValorFrete = PropertyEntity({ type: types.event, text: "Ratear Frete?", visible: ko.observable(true), enable: ko.observable(true) });

    this.ValorSeguro = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Seguro:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.RatearValorSeguro = PropertyEntity({ type: types.event, text: "Ratear Seguro?", visible: ko.observable(true), enable: ko.observable(true) });

    this.ValorDesconto = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Desconto:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.RatearValorDesconto = PropertyEntity({ type: types.event, text: "Ratear Desconto?", visible: ko.observable(true), enable: ko.observable(true) });

    this.ValorOutrasDespesas = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Outras Despesas:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.RatearValorOutrasDepesas = PropertyEntity({ type: types.event, text: "Ratear Outras?", visible: ko.observable(true), enable: ko.observable(true) });

    this.ValorIPI = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor IPI:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorFCPICMS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor FCP ICMS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorFCPICMSST = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor FCP ICMS ST:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorIPIDevolvido = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor IPI Devolvido:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorTotalNFe = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Total:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe), configDecimal: { precision: 2, allowZero: true } });

    this.ValorTotalServicos = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Total Serviços:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.BaseISS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "BC ISS:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorISS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ISS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.BaseDeducao = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "BC Dedução:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorOutrasRetencoes = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Outras Retenções:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorDescontoIncondicional = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Desconto Incondicional:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorDescontoCondicional = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Desconto Condicional:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorRetencaoISS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Retenção ISS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.BasePIS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "BC PIS:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorPIS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor PIS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.BaseCOFINS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor COFINS:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorCOFINS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor COFINS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorFCP = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor FCP:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorICMSDestino = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ICMS Destino:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorICMSRemetente = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ICMS Remetente:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.BCICMSSTRetido = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "BC ICMS ST Retido:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorICMSSTRetido = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ICMS ST Retido:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutTotalizador);

        instancia.RatearValorFrete.eventClick = function (e) {
            instancia.RatearValorFrete();
        };

        instancia.RatearValorSeguro.eventClick = function (e) {
            instancia.RatearValorSeguro();
        };

        instancia.RatearValorDesconto.eventClick = function (e) {
            instancia.RatearValorDesconto();
        };

        instancia.RatearValorOutrasDepesas.eventClick = function (e) {
            instancia.RatearValorOutrasDespesas();
        };
    };

    this.RatearValorFrete = function () {
        var valorFreteTotal = Globalize.parseFloat(instancia.ValorFrete.val());
        var valorTotalNFeTotal = Globalize.parseFloat(instancia.ValorTotalNFe.val());

        if (valorFreteTotal > 0 && nfe.ProdutosServicos.length > 0) {
            exibirConfirmacao("Confirmação", "Realmente deseja ratear o valor do Frete aos itens?", function () {
                var valorFreteAnterior = 0;
                var acumuladorFreteProduto = 0;
                var valorFreteItem = 0;
                var valorTotalNFe = Globalize.parseFloat(instancia.ValorTotalProdutos.val());
                valorTotalNFe += Globalize.parseFloat(instancia.ValorTotalServicos.val());

                for (var i = 0; i < nfe.ProdutosServicos.length; i++) {
                    valorFreteAnterior += Globalize.parseFloat(nfe.ProdutosServicos[i].ValorFrete);
                    var valorTotalItem = Globalize.parseFloat(nfe.ProdutosServicos[i].ValorTotal);
                    valorFreteItem = parseFloat(((valorFreteTotal * valorTotalItem) / valorTotalNFe).toFixed(2));
                    acumuladorFreteProduto += valorFreteItem;
                    nfe.ProdutosServicos[i].ValorFrete = Globalize.format(valorFreteItem, "n2");
                }

                acumuladorFreteProduto = parseFloat(acumuladorFreteProduto.toFixed(2));
                if (acumuladorFreteProduto !== valorFreteTotal) {
                    var diferenca = parseFloat((valorFreteTotal - acumuladorFreteProduto).toFixed(2));
                    if (diferenca > 0) {
                        valorFreteItem = Globalize.parseFloat(nfe.ProdutosServicos[0].ValorFrete);
                        valorFreteItem = valorFreteItem + diferenca;
                        nfe.ProdutosServicos[0].ValorFrete = Globalize.format(valorFreteItem, "n2");
                    } else if (diferenca < 0) {
                        diferenca = diferenca * -1;
                        for (var j = 0; j < nfe.ProdutosServicos.length; j++) {
                            valorFreteItem = Globalize.parseFloat(nfe.ProdutosServicos[j].ValorFrete);
                            if (valorFreteItem > diferenca) {
                                valorFreteItem = valorFreteItem - diferenca;
                                nfe.ProdutosServicos[j].ValorFrete = Globalize.format(valorFreteItem, "n2");
                                break;
                            }
                        }
                    }
                }

                valorTotalNFe = valorTotalNFeTotal + valorFreteTotal - valorFreteAnterior;
                instancia.ValorTotalNFe.val(Globalize.format(valorTotalNFe, "n2"));

            }, function () {
                instancia.RemoverValorFrete();
            });
        } else
            instancia.RemoverValorFrete();
    };

    this.RemoverValorFrete = function () {
        instancia.ValorFrete.val("0,00");
        var valorTotalNFeTotal = Globalize.parseFloat(instancia.ValorTotalNFe.val());
        var valorAplicado = 0;
        for (var i = 0; i < nfe.ProdutosServicos.length; i++) {
            valorAplicado += Globalize.parseFloat(nfe.ProdutosServicos[i].ValorFrete);
            nfe.ProdutosServicos[i].ValorFrete = "0,00";
        }
        if (valorAplicado > 0)
            instancia.ValorTotalNFe.val(Globalize.format(valorTotalNFeTotal - valorAplicado, "n2"));
    };

    this.RatearValorSeguro = function () {
        var valorSeguroTotal = Globalize.parseFloat(instancia.ValorSeguro.val());
        var valorTotalNFeTotal = Globalize.parseFloat(instancia.ValorTotalNFe.val());

        if (valorSeguroTotal > 0 && nfe.ProdutosServicos.length > 0) {
            exibirConfirmacao("Confirmação", "Realmente deseja ratear o valor do Seguro aos itens?", function () {
                var valorSeguroAnterior = 0;
                var valorSeguroItem = 0;
                var acumuladorProduto = 0;
                var valorTotalNFe = Globalize.parseFloat(instancia.ValorTotalProdutos.val());
                valorTotalNFe += Globalize.parseFloat(instancia.ValorTotalServicos.val());

                for (var i = 0; i < nfe.ProdutosServicos.length; i++) {
                    valorSeguroAnterior += Globalize.parseFloat(nfe.ProdutosServicos[i].ValorSeguro);
                    var valorTotalItem = Globalize.parseFloat(nfe.ProdutosServicos[i].ValorTotal);
                    valorSeguroItem = parseFloat(((valorSeguroTotal * valorTotalItem) / valorTotalNFe).toFixed(2));
                    acumuladorProduto += valorSeguroItem;
                    nfe.ProdutosServicos[i].ValorSeguro = Globalize.format(valorSeguroItem, "n2");
                }

                acumuladorProduto = parseFloat(acumuladorProduto.toFixed(2));
                if (acumuladorProduto !== valorSeguroTotal) {
                    var diferenca = parseFloat((valorSeguroTotal - acumuladorProduto).toFixed(2));
                    if (diferenca > 0) {
                        valorSeguroItem = Globalize.parseFloat(nfe.ProdutosServicos[0].ValorSeguro);
                        valorSeguroItem = valorSeguroItem + diferenca;
                        nfe.ProdutosServicos[0].ValorSeguro = Globalize.format(valorSeguroItem, "n2");
                    } else if (diferenca < 0) {
                        diferenca = diferenca * -1;
                        for (var j = 0; j < nfe.ProdutosServicos.length; j++) {
                            valorSeguroItem = Globalize.parseFloat(nfe.ProdutosServicos[j].ValorSeguro);
                            if (valorSeguroItem > diferenca) {
                                valorSeguroItem = valorSeguroItem - diferenca;
                                nfe.ProdutosServicos[j].ValorSeguro = Globalize.format(valorSeguroItem, "n2");
                                break;
                            }
                        }
                    }
                }

                valorTotalNFe = valorTotalNFeTotal + valorSeguroTotal - valorSeguroAnterior;
                instancia.ValorTotalNFe.val(Globalize.format(valorTotalNFe, "n2"));

            }, function () {
                instancia.RemoverValorSeguro();
            });
        } else
            instancia.RemoverValorSeguro();
    };

    this.RemoverValorSeguro = function () {
        instancia.ValorSeguro.val("0,00");
        var valorTotalNFeTotal = Globalize.parseFloat(instancia.ValorTotalNFe.val());
        var valorAplicado = 0;
        for (var i = 0; i < nfe.ProdutosServicos.length; i++) {
            valorAplicado += Globalize.parseFloat(nfe.ProdutosServicos[i].ValorSeguro);
            nfe.ProdutosServicos[i].ValorSeguro = "0,00";
        }
        if (valorAplicado > 0)
            instancia.ValorTotalNFe.val(Globalize.format(valorTotalNFeTotal - valorAplicado, "n2"));
    };

    this.RatearValorDesconto = function () {
        var valorDescontoTotal = Globalize.parseFloat(instancia.ValorDesconto.val());
        var valorTotalNFeTotal = Globalize.parseFloat(instancia.ValorTotalNFe.val());

        if (valorDescontoTotal > 0 && nfe.ProdutosServicos.length > 0) {
            exibirConfirmacao("Confirmação", "Realmente deseja ratear o valor do Desconto aos itens?", function () {
                var valorDescontoAnterior = 0;
                var valorDescontoItem = 0;
                var acumuladorProduto = 0;
                var valorTotalNFe = Globalize.parseFloat(instancia.ValorTotalProdutos.val());
                valorTotalNFe += Globalize.parseFloat(instancia.ValorTotalServicos.val());

                for (var i = 0; i < nfe.ProdutosServicos.length; i++) {
                    valorDescontoAnterior += Globalize.parseFloat(nfe.ProdutosServicos[i].ValorDesconto);
                    var valorTotalItem = Globalize.parseFloat(nfe.ProdutosServicos[i].ValorTotal);
                    valorDescontoItem = parseFloat(((valorDescontoTotal * valorTotalItem) / valorTotalNFe).toFixed(2));
                    acumuladorProduto += valorDescontoItem;
                    nfe.ProdutosServicos[i].ValorDesconto = Globalize.format(valorDescontoItem, "n2");
                }

                acumuladorProduto = parseFloat(acumuladorProduto.toFixed(2));
                if (acumuladorProduto !== valorDescontoTotal) {
                    var diferenca = parseFloat((valorDescontoTotal - acumuladorProduto).toFixed(2));
                    if (diferenca > 0) {
                        valorDescontoItem = Globalize.parseFloat(nfe.ProdutosServicos[0].ValorDesconto);
                        valorDescontoItem = valorDescontoItem + diferenca;
                        nfe.ProdutosServicos[0].ValorDesconto = Globalize.format(valorDescontoItem, "n2");
                    } else if (diferenca < 0) {
                        diferenca = diferenca * -1;
                        for (var j = 0; j < nfe.ProdutosServicos.length; j++) {
                            valorDescontoItem = Globalize.parseFloat(nfe.ProdutosServicos[j].ValorDesconto);
                            if (valorDescontoItem > diferenca) {
                                valorDescontoItem = valorDescontoItem - diferenca;
                                nfe.ProdutosServicos[j].ValorDesconto = Globalize.format(valorDescontoItem, "n2");
                                break;
                            }
                        }
                    }
                }

                valorTotalNFe = valorTotalNFeTotal - valorDescontoTotal + valorDescontoAnterior;
                instancia.ValorTotalNFe.val(Globalize.format(valorTotalNFe, "n2"));

            }, function () {
                instancia.RemoverValorDesconto();
            });
        } else
            instancia.RemoverValorDesconto();
    };

    this.RemoverValorDesconto = function () {
        instancia.ValorDesconto.val("0,00");
        var valorTotalNFeTotal = Globalize.parseFloat(instancia.ValorTotalNFe.val());
        var valorAplicado = 0;
        for (var i = 0; i < nfe.ProdutosServicos.length; i++) {
            valorAplicado += Globalize.parseFloat(nfe.ProdutosServicos[i].ValorDesconto);
            nfe.ProdutosServicos[i].ValorDesconto = "0,00";
        }
        if (valorAplicado > 0)
            instancia.ValorTotalNFe.val(Globalize.format(valorTotalNFeTotal + valorAplicado, "n2"));
    };

    this.RatearValorOutrasDespesas = function () {
        var valorOutrasDespesasTotal = Globalize.parseFloat(instancia.ValorOutrasDespesas.val());
        var valorTotalNFeTotal = Globalize.parseFloat(instancia.ValorTotalNFe.val());

        if (valorOutrasDespesasTotal > 0 && nfe.ProdutosServicos.length > 0) {
            exibirConfirmacao("Confirmação", "Realmente deseja ratear o valor de Outras Despesas aos itens?", function () {
                var valorOutrasDespesasAnterior = 0;
                var valorOutrasDespesasItem = 0;
                var acumuladorProduto = 0;
                var valorTotalNFe = Globalize.parseFloat(instancia.ValorTotalProdutos.val());
                valorTotalNFe += Globalize.parseFloat(instancia.ValorTotalServicos.val());

                for (var i = 0; i < nfe.ProdutosServicos.length; i++) {
                    valorOutrasDespesasAnterior += Globalize.parseFloat(nfe.ProdutosServicos[i].ValorOutras);
                    var valorTotalItem = Globalize.parseFloat(nfe.ProdutosServicos[i].ValorTotal);
                    valorOutrasDespesasItem = parseFloat(((valorOutrasDespesasTotal * valorTotalItem) / valorTotalNFe).toFixed(2));
                    acumuladorProduto += valorOutrasDespesasItem;
                    nfe.ProdutosServicos[i].ValorOutras = Globalize.format(valorOutrasDespesasItem, "n2");
                }

                acumuladorProduto = parseFloat(acumuladorProduto.toFixed(2));
                if (acumuladorProduto !== valorOutrasDespesasTotal) {
                    var diferenca = parseFloat((valorOutrasDespesasTotal - acumuladorProduto).toFixed(2));
                    if (diferenca > 0) {
                        valorOutrasDespesasItem = Globalize.parseFloat(nfe.ProdutosServicos[0].ValorOutras);
                        valorOutrasDespesasItem = valorOutrasDespesasItem + diferenca;
                        nfe.ProdutosServicos[0].ValorOutras = Globalize.format(valorOutrasDespesasItem, "n2");
                    } else if (diferenca < 0) {
                        diferenca = diferenca * -1;
                        for (var j = 0; j < nfe.ProdutosServicos.length; j++) {
                            valorOutrasDespesasItem = Globalize.parseFloat(nfe.ProdutosServicos[j].ValorOutras);
                            if (valorOutrasDespesasItem > diferenca) {
                                valorOutrasDespesasItem = valorOutrasDespesasItem - diferenca;
                                nfe.ProdutosServicos[j].ValorOutras = Globalize.format(valorOutrasDespesasItem, "n2");
                                break;
                            }
                        }
                    }
                }

                valorTotalNFe = valorTotalNFeTotal + valorOutrasDespesasTotal - valorOutrasDespesasAnterior;
                instancia.ValorTotalNFe.val(Globalize.format(valorTotalNFe, "n2"));

            }, function () {
                instancia.RemoverValorOutrasDespesas();
            });
        } else
            instancia.RemoverValorOutrasDespesas();
    };

    this.RemoverValorOutrasDespesas = function () {
        instancia.ValorOutrasDespesas.val("0,00");
        var valorTotalNFeTotal = Globalize.parseFloat(instancia.ValorTotalNFe.val());
        var valorAplicado = 0;
        for (var i = 0; i < nfe.ProdutosServicos.length; i++) {
            valorAplicado += Globalize.parseFloat(nfe.ProdutosServicos[i].ValorOutras);
            nfe.ProdutosServicos[i].ValorOutras = "0,00";
        }
        if (valorAplicado > 0)
            instancia.ValorTotalNFe.val(Globalize.format(valorTotalNFeTotal - valorAplicado, "n2"));
    };

    this.DestivarTotalizador = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    };
};