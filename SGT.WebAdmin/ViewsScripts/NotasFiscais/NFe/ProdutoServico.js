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
/// <reference path="../../Enumeradores/EnumCSTICMS.js" />
/// <reference path="../../Enumeradores/EnumUnidadeMedida.js" />
/// <reference path="../../Enumeradores/EnumOrigemMercadoria.js" />

var _tipoItem = [
    { value: 1, text: "Produto" },
    { value: 2, text: "Serviço" }
];

var _cstICMSNormal = [
    { text: "Selecione", value: 0 },
    { text: "00 - Tributada integralmente", value: EnumCSTICMS.CST00 },
    { text: "10 - Tributada e com cobrança do ICMS por substituição tributária", value: EnumCSTICMS.CST10 },
    { text: "20 - Com redução de base de cálculo", value: EnumCSTICMS.CST20 },
    { text: "30 - Isenta ou não tributada e com cobrança do ICMS por substituição", value: EnumCSTICMS.CST30 },
    { text: "40 - Isenta", value: EnumCSTICMS.CST40 },
    { text: "41 - Não Tributada", value: EnumCSTICMS.CST41 },
    { text: "50 - Suspensão", value: EnumCSTICMS.CST50 },
    { text: "51 - Diferimento", value: EnumCSTICMS.CST51 },
    { text: "60 - ICMS cobrado anteriormente por substituição tributária", value: EnumCSTICMS.CST60 },
    { text: "61 - Tributação monofásica sobre combustíveis cobrada anteriormente", value: EnumCSTICMS.CST61 },
    { text: "70 - Com redução de base de cálculo e cobrança do ICMS por substituição tributária", value: EnumCSTICMS.CST70 },
    { text: "90 - Outras", value: EnumCSTICMS.CST90 }
];

var _cstICMSSimples = [
    { text: "Selecione", value: 0 },
    { text: "101 - Tributada pelo Simples Nacional com permissão de crédito", value: EnumCSTICMS.CSOSN101 },
    { text: "102 - Tributada pelo Simples Nacional sem permissão de crédito", value: EnumCSTICMS.CSOSN102 },
    { text: "103 - Isenção do ICMS no Simples Nacional para faixa de receita bruta", value: EnumCSTICMS.CSOSN103 },
    { text: "201 - Tributada pelo Simples Nacional com permissão de crédito e com cobrança do ICMS por substituição tributária", value: EnumCSTICMS.CSOSN201 },
    { text: "202 - Tributada pelo Simples Nacional sem permissão de crédito e com cobrança do ICMS por substituição tributária", value: EnumCSTICMS.CSOSN202 },
    { text: "203 - Isenção do ICMS no Simples Nacional para faixa de receita bruta e com cobrança do ICMS por substituicao tributaria", value: EnumCSTICMS.CSOSN203 },
    { text: "300 - Imune", value: EnumCSTICMS.CSOSN300 },
    { text: "400 - Nao tributada pelo Simples Nacional", value: EnumCSTICMS.CSOSN400 },
    { text: "500 - ICMS cobrado anteriormente por substituicao tributaria (substituido) ou por antecipacao", value: EnumCSTICMS.CSOSN500 },
    { text: "900 - Outros", value: EnumCSTICMS.CSOSN900 },
    { text: "61 - Tributação monofásica sobre combustíveis cobrada anteriormente", value: EnumCSTICMS.CST61 },
];

var _cstICMS = [
    { text: "--", value: 0 },
    { text: "00", value: EnumCSTICMS.CST00 },
    { text: "10", value: EnumCSTICMS.CST10 },
    { text: "20", value: EnumCSTICMS.CST20 },
    { text: "30", value: EnumCSTICMS.CST30 },
    { text: "40", value: EnumCSTICMS.CST40 },
    { text: "41", value: EnumCSTICMS.CST41 },
    { text: "50", value: EnumCSTICMS.CST50 },
    { text: "51", value: EnumCSTICMS.CST51 },
    { text: "60", value: EnumCSTICMS.CST60 },    
    { text: "70", value: EnumCSTICMS.CST70 },
    { text: "90", value: EnumCSTICMS.CST90 },
    { text: "101", value: EnumCSTICMS.CSOSN101 },
    { text: "102", value: EnumCSTICMS.CSOSN102 },
    { text: "103", value: EnumCSTICMS.CSOSN103 },
    { text: "201", value: EnumCSTICMS.CSOSN201 },
    { text: "202", value: EnumCSTICMS.CSOSN202 },
    { text: "203", value: EnumCSTICMS.CSOSN203 },
    { text: "300", value: EnumCSTICMS.CSOSN300 },
    { text: "400", value: EnumCSTICMS.CSOSN400 },
    { text: "500", value: EnumCSTICMS.CSOSN500 },
    { text: "900", value: EnumCSTICMS.CSOSN900 },
    { text: "61", value: EnumCSTICMS.CST61 }
];

var ProdutoServico = function (nfe, listaProdutoServico, impostoICMS, impostoICMSSST, impostoPIS, impostoCOFINS, impostoII, impostoIPI, impostoISS, impostoICMSDesonerado, impostoDIFAL,
    informacoesAdicionais, lotesProdutos, combustivelProduto, exportacao, impostoICMSMonoRet) {

    var instancia = this;

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, enable: ko.observable(true) });
    this.TipoItem = PropertyEntity({ val: ko.observable(1), def: 1, options: _tipoItem, text: "*Tipo Item:", required: true, enable: ko.observable(true), eventChange: function () { instancia.TipoItemChange(); } });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Serviço:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(false) });

    this.CodigoItem = PropertyEntity({ text: "*Código Item:", getType: typesKnockout.string, maxlength: 60, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.DescricaoItem = PropertyEntity({ text: "*Descrição Item:", getType: typesKnockout.string, maxlength: 120, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.UnidadeMedida = PropertyEntity({ val: ko.observable(EnumUnidadeMedida.Unidade), options: EnumUnidadeMedida.obterOpcoes(), text: "*Unidade de Medida: ", def: EnumUnidadeMedida.Unidade, issue: 88, enable: ko.observable(false), visible: ko.observable(true) });

    this.OrigemMercadoria = PropertyEntity({ text: "Origem:", val: ko.observable(-1), def: -1, options: ko.observable(EnumOrigemMercadoria.obterOpcoesCadastro()), required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.CSTICMS = PropertyEntity({ val: ko.observable(0), def: ko.observable(0), options: ko.observable(_cstICMSSimples), text: "*CST/CSOSN:", required: false, enable: ko.observable(true), visible: ko.observable(true), eventChange: function () { instancia.CSTICMSChange(); } });
    this.CFOP = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*CFOP:", idBtnSearch: guid(), required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Quantidade = PropertyEntity({ def: "0,0000", val: ko.observable("0,0000"), text: "*Quantidade:", getType: typesKnockout.decimal, maxlength: 22, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: nfe.NFe.CasasQuantidadeProdutoNFe.val(), allowZero: true }, eventChange: function () { instancia.CalcularTotalItemNovo(); } });
    this.ValorUnitario = PropertyEntity({ def: "0,00000", val: ko.observable("0,00000"), text: "*Valor Unitário:", getType: typesKnockout.decimal, maxlength: 22, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: nfe.NFe.CasasValorProdutoNFe.val(), allowZero: true }, eventChange: function () { instancia.CalcularTotalItemNovo(); } });
    this.ValorTotalItem = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "*Valor Total:", getType: typesKnockout.decimal, maxlength: 22, required: true, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });

    this.SalvarItemNFe = PropertyEntity({ eventClick: function () { instancia.AdicionarProdutoServico(); }, type: types.event, text: "Salvar Item", visible: ko.observable(true), enable: ko.observable(true) });
    this.CalcularImpostos = PropertyEntity({ eventClick: function () { instancia.CalcularImpostosAutomaticamente(); }, type: types.event, text: "Calc. as tributações?", visible: ko.observable(true), enable: ko.observable(true) });

    this.CalcularImpostosAutomaticamente = function () {
        var valido = true;

        nfe.NFe.Pessoa.requiredClass("form-control");
        nfe.NFe.Atividade.requiredClass("form-control");
        instancia.Quantidade.requiredClass("form-control");
        instancia.ValorUnitario.requiredClass("form-control");
        instancia.ValorTotalItem.requiredClass("form-control");
        instancia.Produto.requiredClass("form-control");
        instancia.Servico.requiredClass("form-control");

        if (valido) {
            if (nfe.NFe.Pessoa.codEntity() == 0 || nfe.NFe.Pessoa.val() == "") {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe a Pessoa antes de buscar as tributações do item!");
                $("#tabNFe_" + nfe.NFe.IdModal.val() + " a:eq(0)").tab("show");
                nfe.NFe.Pessoa.requiredClass("form-control  is-invalid");
                return;
            } else if (nfe.NFe.Atividade.codEntity() == 0 || nfe.NFe.Atividade.val() == "") {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe a Atividade antes de buscar as tributações do item!");
                $("#tabNFe_" + nfe.NFe.IdModal.val() + " a:eq(0)").tab("show");
                nfe.NFe.Atividade.requiredClass("form-control  is-invalid");
                return;
            }
            else if (instancia.ValorTotalItem.val() == "" || instancia.ValorTotalItem.val() <= 0 || Globalize.parseFloat(instancia.ValorTotalItem.val()) <= 0 || Globalize.parseFloat(instancia.Quantidade.val()) <= 0) {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe a Quantidade/Valor Unitário/Valor Total do item!");
                instancia.Quantidade.requiredClass("form-control  is-invalid");
                instancia.ValorUnitario.requiredClass("form-control  is-invalid");
                instancia.ValorTotalItem.requiredClass("form-control  is-invalid");
                return;
            } else if (instancia.TipoItem.val() === 1 && instancia.Produto.codEntity() === 0) {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor selecione o produto!");
                instancia.Produto.requiredClass("form-control  is-invalid");
                return;
            } else if (instancia.TipoItem.val() === 2 && instancia.Servico.codEntity() === 0) {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor selecione o serviço!");
                instancia.Servico.requiredClass("form-control  is-invalid");
                return;
            }
        }
        if (valido) {
            var dados = {
                CodigoPoduto: instancia.Produto.codEntity(),
                CodigoServico: instancia.Servico.codEntity(),
                CodigoCFOP: instancia.CFOP.codEntity(),
                CodigoAtividade: nfe.NFe.Atividade.codEntity(),
                CodigoCliente: nfe.NFe.Pessoa.codEntity(),
                ValorTotalItem: instancia.ValorTotalItem.val(),
                ValorOutras: listaProdutoServico.Outras.val(),
                ValorDesconto: listaProdutoServico.Desconto.val(),
                ValorFrete: listaProdutoServico.Frete.val(),
                ValorSeguro: listaProdutoServico.Seguro.val(),
                ValorIPI: impostoIPI.ValorIPI.val(),
                Quantidade: instancia.Quantidade.val()
            };
            executarReST("GrupoImposto/BuscarTributacaoAutomaticoItem", dados, function (arg) {
                if (arg.Success) {
                    if (arg.Data != false) {
                        if (arg.Data.CSTICMS > 0) {
                            instancia.CSTICMS.val(arg.Data.CSTICMS);
                            instancia.CSTICMSChange();
                        }
                        if (arg.Data.CodigoCFOP > 0) {
                            instancia.CFOP.val(arg.Data.NumeroCFOP);
                            instancia.CFOP.codEntity(arg.Data.CodigoCFOP);
                        }
                        if (arg.Data.CSTCOFINS > 0) {
                            impostoCOFINS.CSTCOFINS.val(arg.Data.CSTCOFINS);
                            impostoCOFINS.BaseCOFINS.val(arg.Data.BaseCOFINS);
                            impostoCOFINS.ReducaoBaseCOFINS.val(arg.Data.ReducaoBaseCOFINS);
                            impostoCOFINS.AliquotaCOFINS.val(arg.Data.AliquotaCOFINS);
                            impostoCOFINS.CalcularImpostoCOFINSInstancia(impostoCOFINS);
                        }

                        if (arg.Data.CSTPIS > 0) {
                            impostoPIS.CSTPIS.val(arg.Data.CSTPIS);
                            impostoPIS.BasePIS.val(arg.Data.BasePIS);
                            impostoPIS.ReducaoBasePIS.val(arg.Data.ReducaoBasePIS);
                            impostoPIS.AliquotaPIS.val(arg.Data.AliquotaPIS);

                            impostoPIS.CalcularImpostoPISInstancia(impostoPIS);
                        }

                        if (arg.Data.CSTIPI > 0) {
                            impostoIPI.CSTIPI.val(arg.Data.CSTIPI);
                            impostoIPI.BaseIPI.val(arg.Data.BaseIPI);
                            impostoIPI.ReducaoBaseIPI.val(arg.Data.ReducaoBaseIPI);
                            impostoIPI.AliquotaIPI.val(arg.Data.AliquotaIPI);

                            impostoIPI.CalcularImpostoIPIInstancia(impostoIPI);
                        }

                        if (Globalize.parseFloat(arg.Data.BaseISS) > 0) {
                            impostoISS.BaseISS.val(arg.Data.BaseISS);
                            impostoISS.AliquotaISS.val(arg.Data.AliquotaISS);

                            impostoISS.CalcularImpostoISSInstancia(impostoISS);
                        }

                        if (arg.Data.CSTICMS > 0 && Globalize.parseFloat(arg.Data.BaseICMS) > 0) {
                            instancia.CSTICMS.val(arg.Data.CSTICMS);
                            impostoICMS.BaseICMS.val(arg.Data.BaseICMS);
                            impostoICMS.ReducaoBaseICMS.val(arg.Data.ReducaoBaseICMS);
                            impostoICMS.AliquotaICMS.val(arg.Data.AliquotaICMS);

                            impostoICMS.CalcularImpostoICMSInstancia(impostoICMS);
                        }

                        if (Globalize.parseFloat(arg.Data.BaseICMSDestino) > 0 && Globalize.parseFloat(arg.Data.AliquotaICMSDestino) > 0) {
                            impostoDIFAL.BaseICMSDestino.val(arg.Data.BaseICMSDestino);
                            impostoDIFAL.AliquotaICMSDestino.val(arg.Data.AliquotaICMSDestino);
                            impostoDIFAL.AliquotaICMSInterno.val(arg.Data.AliquotaICMSInterno);
                            impostoDIFAL.PercentualPartilha.val(arg.Data.PercentualPartilha);
                            impostoDIFAL.PercentualFCP.val(arg.Data.PercentualFCP);
                            if (arg.Data.PercentualFCP != "0,00")
                                impostoDIFAL.BaseFCPDestino.val(arg.Data.BaseICMS);
                            else
                                impostoDIFAL.BaseFCPDestino.val(arg.Data.BaseFCPDestino);

                            impostoDIFAL.CalcularImpostoDIFALInstancia(impostoDIFAL);
                            impostoDIFAL.CalcularImpostoFCPInstancia(impostoDIFAL);
                        }

                        if (arg.Data.CSTICMS > 0 && Globalize.parseFloat(arg.Data.BaseICMSST) > 0) {
                            impostoICMSSST.BaseICMSST.val(arg.Data.BaseICMSST);
                            impostoICMSSST.ReducaoBaseICMSST.val(arg.Data.ReducaoBaseICMSST);
                            impostoICMSSST.PercentualMVA.val(arg.Data.PercentualMVA);
                            impostoICMSSST.AliquotaICMSST.val(arg.Data.AliquotaICMSST);
                            impostoICMSSST.AliquotaInterestadual.val(arg.Data.AliquotaInterestadual);

                            impostoICMSSST.CalcularICMSST();
                        }

                        if (Globalize.parseFloat(arg.Data.BCICMSSTRetido) > 0 || Globalize.parseFloat(arg.Data.ValorICMSSTRetido) > 0) {
                            impostoICMSSST.BCICMSSTRetido.val(arg.Data.BCICMSSTRetido);
                            impostoICMSSST.AliquotaICMSSTRetido.val(arg.Data.AliquotaICMSSTRetido);
                            impostoICMSSST.ValorICMSSTRetido.val(arg.Data.ValorICMSSTRetido);
                        }

                        if (Globalize.parseFloat(arg.Data.BCICMSEfetivo) > 0 || Globalize.parseFloat(arg.Data.ValorICMSEfetivo) > 0) {
                            impostoICMS.BCICMSEfetivo.val(arg.Data.BCICMSEfetivo);
                            impostoICMS.AliquotaICMSEfetivo.val(arg.Data.AliquotaICMSEfetivo);
                            impostoICMS.ReducaoBCICMSEfetivo.val(arg.Data.ReducaoBCICMSEfetivo);
                            impostoICMS.ValorICMSEfetivo.val(arg.Data.ValorICMSEfetivo);
                        }

                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        }
    };

    this.AdicionarProdutoServico = function () {
        var valido = ValidarCamposObrigatorios(instancia);
        nfe.NFe.Pessoa.requiredClass("form-control");
        nfe.NFe.NaturezaOperacao.requiredClass("form-control");
        nfe.NFe.Atividade.requiredClass("form-control");
        instancia.Quantidade.requiredClass("form-control");
        instancia.ValorUnitario.requiredClass("form-control");
        instancia.ValorTotalItem.requiredClass("form-control");
        instancia.CSTICMS.requiredClass("form-control");
        instancia.CFOP.requiredClass("form-control");

        if (valido) { //Campos obrigatórios
            if (nfe.NFe.Pessoa.codEntity() == 0 || nfe.NFe.Pessoa.val() == "") {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe a Pessoa antes de lançar o Produto/Serviço!");
                $("#tabNFe_" + nfe.NFe.IdModal.val() + " a:eq(0)").tab("show");
                nfe.NFe.Pessoa.requiredClass("form-control  is-invalid");
                return;
            } else if (nfe.NFe.NaturezaOperacao.codEntity() == 0 || nfe.NFe.NaturezaOperacao.val() == "") {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe a Natureza de Operação antes de lançar o Produto/Serviço!");
                $("#tabNFe_" + nfe.NFe.IdModal.val() + " a:eq(0)").tab("show");
                nfe.NFe.NaturezaOperacao.requiredClass("form-control  is-invalid");
                return;
            } else if (nfe.NFe.Atividade.codEntity() == 0 || nfe.NFe.Atividade.val() == "") {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe a Atividade antes de lançar o Produto/Serviço!");
                $("#tabNFe_" + nfe.NFe.IdModal.val() + " a:eq(0)").tab("show");
                nfe.NFe.Atividade.requiredClass("form-control  is-invalid");
                return;
            }
            else if (instancia.TipoItem.val() == 1 && instancia.CSTICMS.val() == 0) {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe o CST de ICMS do produto!");
                instancia.CSTICMS.requiredClass("form-control  is-invalid");
                return;
            } else if (instancia.CFOP.codEntity() == 0) {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe a CFOP!");
                instancia.CFOP.requiredClass("form-control  is-invalid");
                return;
            } else if (impostoPIS.CSTPIS.val() == 0) {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe o CST de PIS!");
                $("#tabItensNFe_" + nfe.NFe.IdModal.val() + " a:eq(4)").tab("show");
                impostoPIS.CSTPIS.requiredClass("form-control  is-invalid");
                return;
            } else if (impostoCOFINS.CSTCOFINS.val() == 0) {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe o CST de COFINS!");
                $("#tabItensNFe_" + nfe.NFe.IdModal.val() + " a:eq(5)").tab("show");
                impostoCOFINS.CSTCOFINS.requiredClass("form-control  is-invalid");
                return;
            }
        }

        if (valido) { //Validações gerais
            if (combustivelProduto.CodigoANP.val() != "") {
                var percentualGLP = Globalize.parseFloat(combustivelProduto.PercentualGLP.val());
                var percentualGNN = Globalize.parseFloat(combustivelProduto.PercentualGNN.val());
                var percentualGNI = Globalize.parseFloat(combustivelProduto.PercentualGNI.val());                
                var valorPartidaANP = Globalize.parseFloat(combustivelProduto.ValorPartidaANP.val());

                var percentualCombustivel = percentualGLP + percentualGNN + percentualGNI;
                if (percentualCombustivel > 0 && percentualCombustivel != 100) {
                    valido = false;
                    exibirMensagem(tipoMensagem.atencao, "Lançamento de Combustível", "Soma dos percentuais GLP, GNN ou GNI devem fechar em 100%. Verifique!");
                    $("#tabItensNFe_" + nfe.NFe.IdModal.val() + " a:eq(11)").tab("show");
                    return;
                } else if (percentualCombustivel > 0 && valorPartidaANP == 0) {
                    valido = false;
                    exibirMensagem(tipoMensagem.atencao, "Lançamento de Combustível", "Valor de Partida é obrigatório ao ser informado os percentuais de combustível. Verifique!");
                    $("#tabItensNFe_" + nfe.NFe.IdModal.val() + " a:eq(11)").tab("show");
                    combustivelProduto.ValorPartidaANP.requiredClass("form-control  is-invalid");
                    return;
                }
            }
            //else
                //combustivelProduto.PercentualOrigemComb.val("0,0000");

            if (!string.IsNullOrWhiteSpace(exportacao.ChaveAcessoExportacao.val())) {
                var chaveAcessoExportacao = exportacao.ChaveAcessoExportacao.val().trim().replace(/\s/g, "");

                if (!ValidarChaveAcesso(chaveAcessoExportacao)) {
                    valido = false;
                    exibirMensagem(tipoMensagem.atencao, "Lançamento de Exportação", "A chave da NF-e de exportação é inválida. Verifique!");
                    $("#tabItensNFe_" + nfe.NFe.IdModal.val() + " a:eq(13)").tab("show");
                    return;
                }
            }

            if (lotesProdutos.Codigo.val() > 0) {
                valido = false;
                exibirMensagem("atencao", "Lote em Edição", "Por favor, verifique os lotes pois existe um em edição.");
                return;
            } else if (lotesProdutos.NumeroLote.val() != "") {
                valido = false;
                exibirMensagem("atencao", "Lote em Edição", "Por favor, verifique pois existe um Lote sem salvar o mesmo.");
                return;
            } else {
                var qtdTotalLote = 0;
                for (var i = 0; i < nfe.LotesProdutos.length; i++) {
                    if (nfe.LotesProdutos[i].CodigoItem == "" || nfe.LotesProdutos[i].CodigoItem == instancia.Codigo.val()) {
                        var qtdLote = Globalize.parseFloat(nfe.LotesProdutos[i].QuantidadeLote);
                        qtdTotalLote = qtdTotalLote + qtdLote;
                    }
                }

                var qtdProduto = Globalize.parseFloat(instancia.Quantidade.val());
                if (qtdTotalLote > 0 && qtdTotalLote > qtdProduto) {
                    valido = false;
                    exibirMensagem(tipoMensagem.atencao, "Lançamento de Lotes", "Quantidade total do(s) lote(s) superior a quantidade do item. Verifique!");
                    $("#tabItensNFe_" + nfe.NFe.IdModal.val() + " a:eq(12)").tab("show");
                    return;
                }
            }
        }

        if (valido) {
            if (instancia.Codigo.val() > "0" && instancia.Codigo.val() != undefined) {
                for (var i = 0; i < nfe.ProdutosServicos.length; i++) {
                    if (instancia.Codigo.val() == nfe.ProdutosServicos[i].Codigo) {
                        nfe.ProdutosServicos.splice(i, 1);
                        break;
                    }
                }
            }

            var codigoItem = guid();
            for (var i = 0; i < nfe.LotesProdutos.length; i++) {
                if (nfe.LotesProdutos[i].CodigoItem == "" || nfe.LotesProdutos[i].CodigoItem == instancia.Codigo.val())
                    nfe.LotesProdutos[i].CodigoItem = codigoItem;
            }

            instancia.Codigo.val(codigoItem);
            var descricaoUnidade = "SERV";
            if (instancia.Produto.codEntity() > 0)
                descricaoUnidade = $("#" + instancia.UnidadeMedida.id + "  option:selected").text();

            nfe.ProdutosServicos.push({
                Codigo: instancia.Codigo.val(),
                CodigoProduto: instancia.Produto.codEntity(),
                CodigoServico: instancia.Servico.codEntity(),
                CodigoCFOP: instancia.CFOP.codEntity(),
                CodigoCSTICMS: instancia.CSTICMS.val(),
                OrigemMercadoria: instancia.OrigemMercadoria.val(),
                ValorFrete: listaProdutoServico.Frete.val(),
                ValorSeguro: listaProdutoServico.Seguro.val(),
                ValorOutras: listaProdutoServico.Outras.val(),
                ValorDesconto: listaProdutoServico.Desconto.val(),
                NumeroOrdemCompra: listaProdutoServico.NumeroOrdemCompra.val(),
                NumeroItemOrdemCompra: listaProdutoServico.NumeroItemOrdemCompra.val(),
                Sequencial: "0",

                AliquotaRemICMSRet: impostoICMSMonoRet.AliquotaRemICMSRet.val(),
                ValorICMSMonoRet: impostoICMSMonoRet.ValorICMSMonoRet.val(),

                BCICMS: impostoICMS.BaseICMS.val(),
                ReducaoBCICMS: impostoICMS.ReducaoBaseICMS.val(),
                AliquotaICMS: impostoICMS.AliquotaICMS.val(),
                ValorICMS: impostoICMS.ValorICMS.val(),
                BCICMSEfetivo: impostoICMS.BCICMSEfetivo.val(),
                AliquotaICMSEfetivo: impostoICMS.AliquotaICMSEfetivo.val(),
                ReducaoBCICMSEfetivo: impostoICMS.ReducaoBCICMSEfetivo.val(),
                ValorICMSEfetivo: impostoICMS.ValorICMSEfetivo.val(),

                BCICMSDestino: impostoDIFAL.BaseICMSDestino.val(),
                AliquotaICMSDestino: impostoDIFAL.AliquotaICMSDestino.val(),
                AliquotaICMSInterno: impostoDIFAL.AliquotaICMSInterno.val(),
                PercentualPartilha: impostoDIFAL.PercentualPartilha.val(),
                ValorICMSDestino: impostoDIFAL.ValorICMSDestino.val(),
                ValorICMSRemetente: impostoDIFAL.ValorICMSRemetente.val(),
                AliquotaFCP: impostoDIFAL.PercentualFCP.val(),
                ValorFCP: impostoDIFAL.ValorFCP.val(),

                BCICMSST: impostoICMSSST.BaseICMSST.val(),
                ReducaoBCICMSST: impostoICMSSST.ReducaoBaseICMSST.val(),
                PercentualMVA: impostoICMSSST.PercentualMVA.val(),
                AliquotaICMSST: impostoICMSSST.AliquotaICMSST.val(),
                AliquotaInterestadual: impostoICMSSST.AliquotaInterestadual.val(),
                BCICMSSTRetido: impostoICMSSST.BCICMSSTRetido.val(),
                AliquotaICMSSTRetido: impostoICMSSST.AliquotaICMSSTRetido.val(),
                ValorICMSSTSubstituto: impostoICMSSST.ValorICMSSTSubstituto.val(),
                ValorICMSSTRetido: impostoICMSSST.ValorICMSSTRetido.val(),
                BCICMSSTDestino: impostoICMSSST.BCICMSSTDestino.val(),
                ValorICMSSTDestino: impostoICMSSST.ValorICMSSTDestino.val(),

                CodigoCSTPIS: impostoPIS.CSTPIS.val(),
                BCPIS: impostoPIS.BasePIS.val(),
                ReducaoBCPIS: impostoPIS.ReducaoBasePIS.val(),
                AliquotaPIS: impostoPIS.AliquotaPIS.val(),
                ValorPIS: impostoPIS.ValorPIS.val(),

                CodigoCSTCOFINS: impostoCOFINS.CSTCOFINS.val(),
                BCCOFINS: impostoCOFINS.BaseCOFINS.val(),
                ReducaoBCCOFINS: impostoCOFINS.ReducaoBaseCOFINS.val(),
                AliquotaCOFINS: impostoCOFINS.AliquotaCOFINS.val(),
                ValorCOFINS: impostoCOFINS.ValorCOFINS.val(),

                CodigoCSTIPI: impostoIPI.CSTIPI.val(),
                BCIPI: impostoIPI.BaseIPI.val(),
                ReducaoBCIPI: impostoIPI.ReducaoBaseIPI.val(),
                AliquotaIPI: impostoIPI.AliquotaIPI.val(),

                BCISS: impostoISS.BaseISS.val(),
                AliquotaISS: impostoISS.AliquotaISS.val(),
                ValorISS: impostoISS.ValorISS.val(),
                BaseDeducaoISS: impostoISS.BaseDeducaoISS.val(),
                OutrasRetencoesISS: impostoISS.OutrasRetencoesISS.val(),
                DescontoIncondicional: impostoISS.DescontoIncondicionalISS.val(),
                DescontoCondicional: impostoISS.DescontoCondicionalISS.val(),
                RetencaoISS: impostoISS.RetencaoISS.val(),
                CodigoExigibilidadeISS: impostoISS.ExigibilidadeISS.val(),
                GeraIncentivoFiscal: impostoISS.IncentivoFiscal.val(),
                ProcessoJudicial: impostoISS.ProcessoJudicialISS.val(),

                BCII: impostoII.BaseII.val(),
                DespesaII: impostoII.DespesaII.val(),
                ValorII: impostoII.ValorII.val(),
                ValorIOFII: impostoII.ValorIOFII.val(),
                NumeroDocumentoII: impostoII.NumeroDocumentoII.val(),
                DataRegistroII: impostoII.DataRegistroII.val(),
                LocalDesembaracoII: impostoII.LocalDesembaracoII.val(),
                EstadoDesembaracoII: impostoII.EstadoDesembaracoII.val(),
                DataDesembaracoII: impostoII.DataDesembaracoII.val(),
                CNPJAdquirente: impostoII.CNPJAdquirenteII.val(),
                CodigoViaTransporteII: impostoII.ViaTransporteII.val(),
                ValorFreteMaritimoII: impostoII.ValorFreteMarinhoII.val(),
                CodigoIntermediacaoII: impostoII.IntermediacaoII.val(),

                ValorICMSDesonerado: impostoICMSDesonerado.ValorICMSDesonerado.val(),
                CodigoMotivoDesoneracao: impostoICMSDesonerado.MotivoICMSDesonerado.val(),
                ValorICMSOperacao: impostoICMSDesonerado.ValorICMSOperacao.val(),
                AliquotaICMSOperacao: impostoICMSDesonerado.AliquotaICMSOperacao.val(),
                ValorICMSDeferido: impostoICMSDesonerado.ValorICMSDeferido.val(),

                Descricao: instancia.Produto.codEntity() > 0 ? instancia.Produto.val() : instancia.Servico.val(),
                CSTICMS: _cstICMS[instancia.CSTICMS.val()].text,
                CFOP: instancia.CFOP.val(),
                Qtd: instancia.Quantidade.val(),
                ValorUnitario: instancia.ValorUnitario.val(),
                ValorTotal: instancia.ValorTotalItem.val(),
                ValorST: impostoICMSSST.ValorICMSST.val(),
                ValorIPI: impostoIPI.ValorIPI.val(),
                CodigoItem: instancia.CodigoItem.val(),
                DescricaoItem: instancia.DescricaoItem.val(),
                UnidadeMedida: instancia.UnidadeMedida.val(),
                DescricaoUnidadeMedida: descricaoUnidade,

                BaseFCPICMS: impostoICMS.BaseFCPICMS.val(),
                PercentualFCPICMS: impostoICMS.PercentualFCPICMS.val(),
                ValorFCPICMS: impostoICMS.ValorFCPICMS.val(),
                BaseFCPICMSST: impostoICMSSST.BaseFCPICMSST.val(),
                PercentualFCPICMSST: impostoICMSSST.PercentualFCPICMSST.val(),
                ValorFCPICMSST: impostoICMSSST.ValorFCPICMSST.val(),
                AliquotaFCPICMSST: impostoICMSSST.AliquotaFCPICMSST.val(),
                BaseFCPDestino: impostoDIFAL.BaseFCPDestino.val(),
                PercentualIPIDevolvido: impostoIPI.PercentualIPIDevolvido.val(),
                ValorIPIDevolvido: impostoIPI.ValorIPIDevolvido.val(),

                InformacoesAdicionaisItem: informacoesAdicionais.InformacoesAdicionaisItem.val(),
                IndicadorEscalaRelevante: informacoesAdicionais.IndicadorEscalaRelevante.val(),
                CNPJFabricante: informacoesAdicionais.CNPJFabricante.val(),
                CodigoBeneficioFiscal: informacoesAdicionais.CodigoBeneficioFiscal.val(),
                QuantidadeTributavel: informacoesAdicionais.QuantidadeTributavel.val(),
                ValorUnitarioTributavel: informacoesAdicionais.ValorUnitarioTributavel.val(),
                UnidadeDeMedidaTributavel: informacoesAdicionais.UnidadeDeMedidaTributavel.val(),
                CodigoEANTributavel: informacoesAdicionais.CodigoEANTributavel.val(),
                CodigoNFCI: informacoesAdicionais.CodigoNFCI.val(),
                CodigoLocalArmazenamento: informacoesAdicionais.LocalArmazenamento.codEntity(),
                LocalArmazenamento: informacoesAdicionais.LocalArmazenamento.val(),

                CodigoANP: combustivelProduto.CodigoANP.val(),
                PercentualGLP: combustivelProduto.PercentualGLP.val(),
                PercentualGNN: combustivelProduto.PercentualGNN.val(),
                PercentualGNI: combustivelProduto.PercentualGNI.val(),
                PercentualOrigemComb: combustivelProduto.PercentualOrigemComb.val(),
                PercentualMisturaBiodiesel: combustivelProduto.PercentualMisturaBiodiesel.val(),
                ValorPartidaANP: combustivelProduto.ValorPartidaANP.val(),

                NumeroDrawback: exportacao.NumeroDrawback.val(),
                NumeroRegistroExportacao: exportacao.NumeroRegistroExportacao.val(),
                ChaveAcessoExportacao: exportacao.ChaveAcessoExportacao.val()
            });

            instancia.SomarTotalizadores(nfe, impostoICMS, impostoICMSDesonerado, impostoII, impostoICMSSST, listaProdutoServico, impostoIPI, impostoISS, impostoPIS, impostoCOFINS, impostoDIFAL);

            nfe.Totalizador.ValorFrete.enable(true);
            nfe.Totalizador.ValorSeguro.enable(true);
            nfe.Totalizador.ValorDesconto.enable(true);
            nfe.Totalizador.ValorOutrasDespesas.enable(true);

            listaProdutoServico.RecarregarGrid();
            lotesProdutos.RecarregarGrid();

            LimparCampos(instancia);
            instancia.TipoItemChange();
            $("#tabItensNFe_" + nfe.NFe.IdModal.val() + " a:eq(0)").tab("show");
            $("#" + instancia.TipoItem.id).focus();
        } else {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        }
    };

    this.SomarTotalizadores = function (nfe, impostoICMS, impostoICMSDesonerado, impostoII, impostoICMSSST, listaProdutoServico, impostoIPI, impostoISS, impostoPIS, impostoCOFINS, impostoDIFAL) {
        var baseICMSItem = Globalize.parseFloat(impostoICMS.BaseICMS.val());
        var baseICMSTotal = Globalize.parseFloat(nfe.Totalizador.BaseICMS.val());
        var baseICMS = baseICMSItem + baseICMSTotal;
        nfe.Totalizador.BaseICMS.val(Globalize.format(baseICMS, "n2"));

        var valorICMSItem = Globalize.parseFloat(impostoICMS.ValorICMS.val());
        var valorICMSTotal = Globalize.parseFloat(nfe.Totalizador.ValorICMS.val());
        var valorICMS = valorICMSItem + valorICMSTotal;
        nfe.Totalizador.ValorICMS.val(Globalize.format(valorICMS, "n2"));

        var valorICMSDesoneradoItem = Globalize.parseFloat(impostoICMSDesonerado.ValorICMSDesonerado.val());
        var valorICMSDesoneradoTotal = Globalize.parseFloat(nfe.Totalizador.ValorICMSDesonerado.val());
        var valorICMSDesonerado = valorICMSDesoneradoItem + valorICMSDesoneradoTotal;
        nfe.Totalizador.ValorICMSDesonerado.val(Globalize.format(valorICMSDesonerado, "n2"));

        var valorIIItem = Globalize.parseFloat(impostoII.ValorII.val());
        var valorIITotal = Globalize.parseFloat(nfe.Totalizador.ValorII.val());
        var valorII = valorIIItem + valorIITotal;
        nfe.Totalizador.ValorII.val(Globalize.format(valorII, "n2"));

        var baseICMSSTItem = Globalize.parseFloat(impostoICMSSST.BaseICMSST.val());
        var baseICMSSTTotal = Globalize.parseFloat(nfe.Totalizador.BaseICMSST.val());
        var baseICMSST = baseICMSSTItem + baseICMSSTTotal;
        nfe.Totalizador.BaseICMSST.val(Globalize.format(baseICMSST, "n2"));

        var valorICMSSTItem = Globalize.parseFloat(impostoICMSSST.ValorICMSST.val());
        var valorICMSSTTotal = Globalize.parseFloat(nfe.Totalizador.ValorICMSST.val());
        var valorICMSST = valorICMSSTItem + valorICMSSTTotal;
        nfe.Totalizador.ValorICMSST.val(Globalize.format(valorICMSST, "n2"));

        var valorTotalItem = Globalize.parseFloat(instancia.ValorTotalItem.val());
        if (instancia.Produto.codEntity() > 0) {
            var valorTotalProdutosTotal = Globalize.parseFloat(nfe.Totalizador.ValorTotalProdutos.val());
            var valorTotalProdutos = valorTotalItem + valorTotalProdutosTotal;
            nfe.Totalizador.ValorTotalProdutos.val(Globalize.format(valorTotalProdutos, "n2"));
        }
        else {
            var valorTotalServicosTotal = Globalize.parseFloat(nfe.Totalizador.ValorTotalServicos.val());
            var valorTotalServicos = valorTotalItem + valorTotalServicosTotal;
            nfe.Totalizador.ValorTotalServicos.val(Globalize.format(valorTotalServicos, "n2"));
        }

        var valorFreteItem = Globalize.parseFloat(listaProdutoServico.Frete.val());
        var valorFreteTotal = Globalize.parseFloat(nfe.Totalizador.ValorFrete.val());
        var valorFrete = valorFreteItem + valorFreteTotal;
        nfe.Totalizador.ValorFrete.val(Globalize.format(valorFrete, "n2"));

        var valorSeguroItem = Globalize.parseFloat(listaProdutoServico.Seguro.val());
        var valorSeguroTotal = Globalize.parseFloat(nfe.Totalizador.ValorSeguro.val());
        var valorSeguro = valorSeguroItem + valorSeguroTotal;
        nfe.Totalizador.ValorSeguro.val(Globalize.format(valorSeguro, "n2"));

        var valorDescontoItem = Globalize.parseFloat(listaProdutoServico.Desconto.val());
        var valorDescontoTotal = Globalize.parseFloat(nfe.Totalizador.ValorDesconto.val());
        var valorDesconto = valorDescontoItem + valorDescontoTotal;
        nfe.Totalizador.ValorDesconto.val(Globalize.format(valorDesconto, "n2"));

        var valorOutrasDespesasItem = Globalize.parseFloat(listaProdutoServico.Outras.val());
        var valorOutrasDespesasTotal = Globalize.parseFloat(nfe.Totalizador.ValorOutrasDespesas.val());
        var valorOutrasDespesas = valorOutrasDespesasItem + valorOutrasDespesasTotal;
        nfe.Totalizador.ValorOutrasDespesas.val(Globalize.format(valorOutrasDespesas, "n2"));

        var valorIPIItem = Globalize.parseFloat(impostoIPI.ValorIPI.val());
        var valorIPITotal = Globalize.parseFloat(nfe.Totalizador.ValorIPI.val());
        var valorIPI = valorIPIItem + valorIPITotal;
        nfe.Totalizador.ValorIPI.val(Globalize.format(valorIPI, "n2"));

        var valorFCPICMSItem = Globalize.parseFloat(impostoICMS.ValorFCPICMS.val());
        var valorFCPICMSTotal = Globalize.parseFloat(nfe.Totalizador.ValorFCPICMS.val());
        var valorFCPICMS = valorFCPICMSItem + valorFCPICMSTotal;
        nfe.Totalizador.ValorFCPICMS.val(Globalize.format(valorFCPICMS, "n2"));

        var valorFCPICMSSTItem = Globalize.parseFloat(impostoICMSSST.ValorFCPICMSST.val());
        var valorFCPICMSSTTotal = Globalize.parseFloat(nfe.Totalizador.ValorFCPICMSST.val());
        var valorFCPICMSST = valorFCPICMSSTItem + valorFCPICMSSTTotal;
        nfe.Totalizador.ValorFCPICMSST.val(Globalize.format(valorFCPICMSST, "n2"));

        var valorIPIDevolvidoItem = Globalize.parseFloat(impostoIPI.ValorIPIDevolvido.val());
        var valorIPIDevolvidoTotal = Globalize.parseFloat(nfe.Totalizador.ValorIPIDevolvido.val());
        var valorIPIDevolvido = valorIPIDevolvidoItem + valorIPIDevolvidoTotal;
        nfe.Totalizador.ValorIPIDevolvido.val(Globalize.format(valorIPIDevolvido, "n2"));

        var baseISSItem = Globalize.parseFloat(impostoISS.BaseISS.val());
        var baseISSTotal = Globalize.parseFloat(nfe.Totalizador.BaseISS.val());
        var baseISS = baseISSItem + baseISSTotal;
        nfe.Totalizador.BaseISS.val(Globalize.format(baseISS, "n2"));

        var valorISSItem = Globalize.parseFloat(impostoISS.ValorISS.val());
        var valorISSTotal = Globalize.parseFloat(nfe.Totalizador.ValorISS.val());
        var valorISS = valorISSItem + valorISSTotal;
        nfe.Totalizador.ValorISS.val(Globalize.format(valorISS, "n2"));

        var baseDeducaoItem = Globalize.parseFloat(impostoISS.BaseDeducaoISS.val());
        var baseDeducaoTotal = Globalize.parseFloat(nfe.Totalizador.BaseDeducao.val());
        var baseDeducao = baseDeducaoItem + baseDeducaoTotal;
        nfe.Totalizador.BaseDeducao.val(Globalize.format(baseDeducao, "n2"));

        var valorOutrasRetencoesItem = Globalize.parseFloat(impostoISS.OutrasRetencoesISS.val());
        var valorOutrasRetencoesTotal = Globalize.parseFloat(nfe.Totalizador.ValorOutrasRetencoes.val());
        var valorOutrasRetencoes = valorOutrasRetencoesItem + valorOutrasRetencoesTotal;
        nfe.Totalizador.ValorOutrasRetencoes.val(Globalize.format(valorOutrasRetencoes, "n2"));

        var valorDescontoIncondicionalItem = Globalize.parseFloat(impostoISS.DescontoIncondicionalISS.val());
        var valorDescontoIncondicionalTotal = Globalize.parseFloat(nfe.Totalizador.ValorDescontoIncondicional.val());
        var valorDescontoIncondicional = valorDescontoIncondicionalItem + valorDescontoIncondicionalTotal;
        nfe.Totalizador.ValorDescontoIncondicional.val(Globalize.format(valorDescontoIncondicional, "n2"));

        var valorDescontoCondicionalItem = Globalize.parseFloat(impostoISS.DescontoCondicionalISS.val());
        var valorDescontoCondicionalTotal = Globalize.parseFloat(nfe.Totalizador.ValorDescontoCondicional.val());
        var valorDescontoCondicional = valorDescontoCondicionalItem + valorDescontoCondicionalTotal;
        nfe.Totalizador.ValorDescontoCondicional.val(Globalize.format(valorDescontoCondicional, "n2"));

        var valorRetencaoISSItem = Globalize.parseFloat(impostoISS.RetencaoISS.val());
        var valorRetencaoISSTotal = Globalize.parseFloat(nfe.Totalizador.ValorRetencaoISS.val());
        var valorRetencaoISS = valorRetencaoISSItem + valorRetencaoISSTotal;
        nfe.Totalizador.ValorRetencaoISS.val(Globalize.format(valorRetencaoISS, "n2"));

        var basePISItem = Globalize.parseFloat(impostoPIS.BasePIS.val());
        var basePISTotal = Globalize.parseFloat(nfe.Totalizador.BasePIS.val());
        var basePIS = basePISItem + basePISTotal;
        nfe.Totalizador.BasePIS.val(Globalize.format(basePIS, "n2"));

        var valorPISItem = Globalize.parseFloat(impostoPIS.ValorPIS.val());
        var valorPISTotal = Globalize.parseFloat(nfe.Totalizador.ValorPIS.val());
        var valorPIS = valorPISItem + valorPISTotal;
        nfe.Totalizador.ValorPIS.val(Globalize.format(valorPIS, "n2"));

        var baseCOFINSItem = Globalize.parseFloat(impostoCOFINS.BaseCOFINS.val());
        var baseCOFINSTotal = Globalize.parseFloat(nfe.Totalizador.BaseCOFINS.val());
        var baseCOFINS = baseCOFINSItem + baseCOFINSTotal;
        nfe.Totalizador.BaseCOFINS.val(Globalize.format(baseCOFINS, "n2"));

        var valorCOFINSItem = Globalize.parseFloat(impostoCOFINS.ValorCOFINS.val());
        var valorCOFINSTotal = Globalize.parseFloat(nfe.Totalizador.ValorCOFINS.val());
        var valorCOFINS = valorCOFINSItem + valorCOFINSTotal;
        nfe.Totalizador.ValorCOFINS.val(Globalize.format(valorCOFINS, "n2"));

        var valorFCPItem = Globalize.parseFloat(impostoDIFAL.ValorFCP.val());
        var valorFCPTotal = Globalize.parseFloat(nfe.Totalizador.ValorFCP.val());
        var valorFCP = valorFCPItem + valorFCPTotal;
        nfe.Totalizador.ValorFCP.val(Globalize.format(valorFCP, "n2"));

        //DIFAL
        var valorICMSDestinoItem = Globalize.parseFloat(impostoDIFAL.ValorICMSDestino.val());
        var valorICMSDestinoTotal = Globalize.parseFloat(nfe.Totalizador.ValorICMSDestino.val());
        var valorICMSDestino = valorICMSDestinoItem + valorICMSDestinoTotal;
        nfe.Totalizador.ValorICMSDestino.val(Globalize.format(valorICMSDestino, "n2"));

        var valorICMSRemetenteItem = Globalize.parseFloat(impostoDIFAL.ValorICMSRemetente.val());
        var valorICMSRemetenteTotal = Globalize.parseFloat(nfe.Totalizador.ValorICMSRemetente.val());
        var valorICMSRemetente = valorICMSRemetenteItem + valorICMSRemetenteTotal;
        nfe.Totalizador.ValorICMSRemetente.val(Globalize.format(valorICMSRemetente, "n2"));

        //DRCST
        var bCICMSSTRetidoItem = Globalize.parseFloat(impostoICMSSST.BCICMSSTRetido.val());
        var bCICMSSTRetidoTotal = Globalize.parseFloat(nfe.Totalizador.BCICMSSTRetido.val());
        nfe.Totalizador.BCICMSSTRetido.val(Globalize.format(bCICMSSTRetidoItem + bCICMSSTRetidoTotal, "n2"));

        var valorICMSSTRetidoItem = Globalize.parseFloat(impostoICMSSST.ValorICMSSTRetido.val());
        var valorICMSSTRetidoTotal = Globalize.parseFloat(nfe.Totalizador.ValorICMSSTRetido.val());
        nfe.Totalizador.ValorICMSSTRetido.val(Globalize.format(valorICMSSTRetidoItem + valorICMSSTRetidoTotal, "n2"));

        //TOTAL NF
        var valorFrete = Globalize.parseFloat(listaProdutoServico.Frete.val());
        var valorSeguro = Globalize.parseFloat(listaProdutoServico.Seguro.val());
        var valorDesconto = Globalize.parseFloat(listaProdutoServico.Desconto.val());
        var valorOutrasDespesas = Globalize.parseFloat(listaProdutoServico.Outras.val());
        var valorICMSDesonerado = Globalize.parseFloat(impostoICMSDesonerado.ValorICMSDesonerado.val());
        var valorIPI = Globalize.parseFloat(impostoIPI.ValorIPI.val());
        var valorICMSST = Globalize.parseFloat(impostoICMSSST.ValorICMSST.val());
        var valorII = Globalize.parseFloat(impostoII.ValorII.val());
        var valorFCPICMSST = Globalize.parseFloat(impostoICMSSST.ValorFCPICMSST.val());
        var ValorIPIDevolvido = Globalize.parseFloat(impostoIPI.ValorIPIDevolvido.val());

        var valorTotalNFeItem = Globalize.parseFloat(instancia.ValorTotalItem.val());
        var valorTotalNFeTotal = Globalize.parseFloat(nfe.Totalizador.ValorTotalNFe.val());

        var valorTotalNFe = valorTotalNFeItem + valorTotalNFeTotal + valorICMSST + valorIPI + valorOutrasDespesas - valorDesconto - valorICMSDesonerado + valorSeguro + valorFrete + valorII + valorFCPICMSST + ValorIPIDevolvido;
        nfe.Totalizador.ValorTotalNFe.val(Globalize.format(valorTotalNFe, "n2"));
    };

    this.TipoItemChange = function () {
        LimparCampos(listaProdutoServico);
        LimparCampos(impostoICMS);
        LimparCampos(impostoICMSMonoRet);
        LimparCampos(impostoICMSSST);
        LimparCampos(impostoDIFAL);
        LimparCampos(impostoPIS);
        LimparCampos(impostoCOFINS);
        LimparCampos(impostoII);
        LimparCampos(impostoIPI);
        LimparCampos(impostoISS);
        LimparCampos(impostoICMSDesonerado);
        LimparCampos(informacoesAdicionais);
        LimparCampos(lotesProdutos);
        LimparCampos(combustivelProduto);
        LimparCampos(exportacao);

        instancia.CSTICMS.val(0);
        if (instancia.TipoItem.val() === 1) {
            instancia.UnidadeMedida.visible(true);
            instancia.Servico.required = false;
            instancia.Servico.visible(false);
            LimparCampoEntity(instancia.Servico);

            instancia.CSTICMS.required = true;
            instancia.CSTICMS.visible(true);
            instancia.OrigemMercadoria.visible(true);
            instancia.Produto.required = true;
            instancia.Produto.visible(true);
                        
            impostoISS.DestivarImpostoISS();
            impostoICMS.HabilitarImpostoICMS();
            impostoICMSSST.HabilitarImpostoICMSST();
            impostoDIFAL.HabilitarImpostoDIFAL();
            impostoICMSMonoRet.HabilitarImpostoICMSMonoRet();

            impostoPIS.HabilitarImpostoPIS();
            impostoCOFINS.HabilitarImpostoCOFINS();

            impostoII.HabilitarImpostoII();
            impostoIPI.HabilitarImpostoIPI();
            impostoICMSDesonerado.HabilitarImpostoICMSDesonerado();
            lotesProdutos.HabilitarLoteProduto();
            combustivelProduto.HabilitarCombustivelProduto();
            exportacao.HabilitarExportacaoProdutoServico();
        }
        else {
            instancia.UnidadeMedida.visible(false);
            instancia.Servico.required = true;
            instancia.Servico.visible(true);

            instancia.OrigemMercadoria.visible(false);
            instancia.CSTICMS.visible(false);
            instancia.CSTICMS.required = false;
            instancia.Produto.required = false;
            instancia.Produto.visible(false);
            LimparCampoEntity(instancia.Produto);

            impostoISS.HabilitarImpostoISS();
            impostoICMSMonoRet.DestivarImpostoICMSMonoRet();
            impostoICMS.DestivarImpostoICMS();
            impostoICMSSST.DestivarImpostoICMSST();
            impostoDIFAL.DestivarImpostoDIFAL();

            impostoPIS.HabilitarImpostoPIS();
            impostoCOFINS.HabilitarImpostoCOFINS();

            impostoII.DestivarImpostoII();
            impostoIPI.DestivarImpostoIPI();
            impostoICMSDesonerado.DestivarImpostoICMSDesonerado();
            lotesProdutos.DestivarLoteProduto();
            combustivelProduto.DestivarCombustivelProduto();
            exportacao.DesativarExportacaoProdutoServico();
        }
    };

    this.CSTICMSChange = function () {
        HabilitarCamposICMSMonoRet(impostoICMSMonoRet, false, false);
        if (instancia.CSTICMS.val() === EnumCSTICMS.CST00) {
            HabilitarCamposICMS(impostoICMS, true, false);
            HabilitarCamposICMSST(impostoICMSSST, false, false);
        } else if (instancia.CSTICMS.val() === EnumCSTICMS.CST10) {
            HabilitarCamposICMS(impostoICMS, true, false);
            HabilitarCamposICMSST(impostoICMSSST, true, false);
        } else if (instancia.CSTICMS.val() === EnumCSTICMS.CST20) {
            HabilitarCamposICMS(impostoICMS, true, false);
            HabilitarCamposICMSST(impostoICMSSST, false, false);
        } else if (instancia.CSTICMS.val() === EnumCSTICMS.CST30) {
            HabilitarCamposICMS(impostoICMS, false, false);
            HabilitarCamposICMSST(impostoICMSSST, true, false);
        } else if (instancia.CSTICMS.val() === EnumCSTICMS.CST40 || instancia.CSTICMS.val() === EnumCSTICMS.CST41 || instancia.CSTICMS.val() === EnumCSTICMS.CST50) {
            HabilitarCamposICMS(impostoICMS, false, false);
            HabilitarCamposICMSST(impostoICMSSST, false, false);
        } else if (instancia.CSTICMS.val() === EnumCSTICMS.CST51) {
            HabilitarCamposICMS(impostoICMS, true, false);
            HabilitarCamposICMSST(impostoICMSSST, false, false);
        } else if (instancia.CSTICMS.val() === EnumCSTICMS.CST60) {
            HabilitarCamposICMS(impostoICMS, false, true);
            HabilitarCamposICMSST(impostoICMSSST, false, true);
        } else if (instancia.CSTICMS.val() === EnumCSTICMS.CST61) {
            HabilitarCamposICMSMonoRet(impostoICMSMonoRet, false, true);
            HabilitarCamposICMS(impostoICMS, false, false);
            HabilitarCamposICMSST(impostoICMSSST, false, false);
        } else if (instancia.CSTICMS.val() === EnumCSTICMS.CST70) {
            HabilitarCamposICMS(impostoICMS, true, false);
            HabilitarCamposICMSST(impostoICMSSST, true, false);
        } else if (instancia.CSTICMS.val() === EnumCSTICMS.CST90) {
            HabilitarCamposICMS(impostoICMS, true, false);
            HabilitarCamposICMSST(impostoICMSSST, true, false);
        } else if (instancia.CSTICMS.val() === EnumCSTICMS.CSOSN101) {
            HabilitarCamposICMS(impostoICMS, false, false);
            HabilitarCamposICMSST(impostoICMSSST, false, false);
        } else if (instancia.CSTICMS.val() === EnumCSTICMS.CSOSN102 || instancia.CSTICMS.val() === EnumCSTICMS.CSOSN103 || instancia.CSTICMS.val() === EnumCSTICMS.CSOSN300 || instancia.CSTICMS.val() === EnumCSTICMS.CSOSN400) {
            HabilitarCamposICMS(impostoICMS, false, false);
            HabilitarCamposICMSST(impostoICMSSST, false, false);
        } else if (instancia.CSTICMS.val() === EnumCSTICMS.CSOSN201) {
            HabilitarCamposICMS(impostoICMS, false, false);
            HabilitarCamposICMSST(impostoICMSSST, true, false);
        } else if (instancia.CSTICMS.val() === EnumCSTICMS.CSOSN202 || instancia.CSTICMS.val() === EnumCSTICMS.CSOSN203) {
            HabilitarCamposICMS(impostoICMS, false, false);
            HabilitarCamposICMSST(impostoICMSSST, true, false);
        } else if (instancia.CSTICMS.val() === EnumCSTICMS.CSOSN500) {
            HabilitarCamposICMS(impostoICMS, false, true);
            HabilitarCamposICMSST(impostoICMSSST, false, true);
        } else if (instancia.CSTICMS.val() === EnumCSTICMS.CSOSN900) {
            HabilitarCamposICMS(impostoICMS, true, false);
            HabilitarCamposICMSST(impostoICMSSST, true, false);
        }
    };

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutProdutoServico);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe)
            new BuscarProdutoTMSComEstoque(instancia.Produto, instancia.RetornoProdutoTMS, nfe.NFe.NaturezaOperacao, true);
        else
            new BuscarProdutoTMS(instancia.Produto, instancia.RetornoProdutoTMS, null, nfe.NFe.NaturezaOperacao, null, null, true);

        new BuscarServicoTMS(instancia.Servico, instancia.RetornoServicoTMS, nfe.NFe.Pessoa, nfe.NFe.TipoEmissao, nfe.NFe.NaturezaOperacao);
        new BuscarCFOPNotaFiscal(instancia.CFOP, instancia.RetornoCFOPNotaFiscal, null, null, nfe.NFe.Pessoa, nfe.NFe.TipoEmissao, nfe.NFe.Empresa, nfe.NFe.IndicadorPresenca);

        instancia.TipoItemChange();
    };

    this.RetornoProdutoTMS = function (data) {
        instancia.UnidadeMedida.val(data.CodigoUnidadeMedida);
        instancia.Produto.codEntity(data.Codigo);
        instancia.Produto.val(data.Descricao);
        instancia.OrigemMercadoria.val(data.OrigemMercadoria);

        if (data.ValorVenda != null && data.ValorVenda != "") {
            instancia.ValorUnitario.val(data.ValorVenda);
        }
        if (data.CodigoCFOP != null && data.CodigoCFOP != "" && data.CodigoCFOP > 0) {
            instancia.CFOP.val(data.CFOP);
            instancia.CFOP.codEntity(data.CodigoCFOP);
        }
        instancia.DescricaoItem.val(data.DescricaoNotaFiscal);
        if (data.CodigoProduto != "")
            instancia.CodigoItem.val(data.CodigoProduto);
        else
            instancia.CodigoItem.val(data.Codigo);

        combustivelProduto.CodigoANP.val(data.CodigoANP);
        combustivelProduto.PercentualGLP.val(data.PercentualGLP);
        combustivelProduto.PercentualGNN.val(data.PercentualGNN);
        combustivelProduto.PercentualGNI.val(data.PercentualGNI);
        combustivelProduto.PercentualOrigemComb.val(data.PercentualOrigemCombustivel);        
        combustivelProduto.PercentualMisturaBiodiesel.val(data.PercentualMisturaBiodiesel);        
        combustivelProduto.ValorPartidaANP.val(data.ValorPartidaANP);
        informacoesAdicionais.IndicadorEscalaRelevante.val(data.IndicadorEscalaRelevante);
        informacoesAdicionais.CNPJFabricante.val(data.CNPJFabricante);
        informacoesAdicionais.CodigoBeneficioFiscal.val(data.CodigoBeneficioFiscal);
    };

    this.RetornoServicoTMS = function (data) {
        instancia.Servico.codEntity(data.Codigo);
        instancia.Servico.val(data.DescricaoSemNFE);
        if (data.CodigoCFOP != null && data.CodigoCFOP > 0) {
            instancia.CFOP.codEntity(data.CodigoCFOP);
            instancia.CFOP.val(data.CFOP);
        }
        if (data.AliquotaISS != null && data.AliquotaISS != "") {
            impostoISS.AliquotaISS.val(data.AliquotaISS);
        }
        if (data.ValorVenda != null && data.ValorVenda != "") {
            instancia.ValorUnitario.val(data.ValorVenda);
        }
        instancia.DescricaoItem.val(data.DescricaoNFE);
        instancia.CodigoItem.val(data.Codigo);
    };

    this.RetornoCFOPNotaFiscal = function (data) {
        instancia.CFOP.codEntity(data.Codigo);
        instancia.CFOP.val(data.CodigoCFOP);
    };

    this.DestivarProdutoServico = function () {
        DesabilitarCamposInstanciasNFe(instancia);
        instancia.UnidadeMedida.enable(false);
    };

    this.HabilitarProdutoServico = function () {
        HabilitarCamposInstanciasNFe(instancia);
        instancia.UnidadeMedida.enable(false);
    };

    this.CalcularTotalItemNovo = function () {

        if (nfe.NFe.CasasQuantidadeProdutoNFe.val() === 0) {
            instancia.Quantidade.def = "0";
            instancia.Quantidade.configDecimal = { precision: 0, allowZero: true };
        } else if (nfe.NFe.CasasQuantidadeProdutoNFe.val() === 1) {
            instancia.Quantidade.def = "0,0";
            instancia.Quantidade.configDecimal = { precision: 1, allowZero: true };
        } else if (nfe.NFe.CasasQuantidadeProdutoNFe.val() === 2) {
            instancia.Quantidade.def = "0,00";
            instancia.Quantidade.configDecimal = { precision: 2, allowZero: true };
        } else if (nfe.NFe.CasasQuantidadeProdutoNFe.val() === 3) {
            instancia.Quantidade.def = "0,000";
            instancia.Quantidade.configDecimal = { precision: 3, allowZero: true };
        } else {
            instancia.Quantidade.def = "0,0000";
            instancia.Quantidade.configDecimal = { precision: 4, allowZero: true };
        }

        if (nfe.NFe.CasasValorProdutoNFe.val() === 0) {
            instancia.ValorUnitario.def = "0";
            instancia.ValorUnitario.configDecimal = { precision: 0, allowZero: true };
        } else if (nfe.NFe.CasasValorProdutoNFe.val() === 1) {
            instancia.ValorUnitario.def = "0,0";
            instancia.ValorUnitario.configDecimal = { precision: 1, allowZero: true };
        } else if (nfe.NFe.CasasValorProdutoNFe.val() === 2) {
            instancia.ValorUnitario.def = "0,00";
            instancia.ValorUnitario.configDecimal = { precision: 2, allowZero: true };
        } else if (nfe.NFe.CasasValorProdutoNFe.val() === 3) {
            instancia.ValorUnitario.def = "0,000";
            instancia.ValorUnitario.configDecimal = { precision: 3, allowZero: true };
        } else if (nfe.NFe.CasasValorProdutoNFe.val() === 4) {
            instancia.ValorUnitario.def = "0,0000";
            instancia.ValorUnitario.configDecimal = { precision: 4, allowZero: true };
        } else if (nfe.NFe.CasasValorProdutoNFe.val() === 5) {
            instancia.ValorUnitario.def = "0,00000";
            instancia.ValorUnitario.configDecimal = { precision: 5, allowZero: true };
        } else if (nfe.NFe.CasasValorProdutoNFe.val() === 6) {
            instancia.ValorUnitario.def = "0,000000";
            instancia.ValorUnitario.configDecimal = { precision: 6, allowZero: true };
        } else if (nfe.NFe.CasasValorProdutoNFe.val() === 7) {
            instancia.ValorUnitario.def = "0,0000000";
            instancia.ValorUnitario.configDecimal = { precision: 7, allowZero: true };
        } else if (nfe.NFe.CasasValorProdutoNFe.val() === 8) {
            instancia.ValorUnitario.def = "0,00000000";
            instancia.ValorUnitario.configDecimal = { precision: 8, allowZero: true };
        } else if (nfe.NFe.CasasValorProdutoNFe.val() === 9) {
            instancia.ValorUnitario.def = "0,000000000";
            instancia.ValorUnitario.configDecimal = { precision: 9, allowZero: true };
        } else if (nfe.NFe.CasasValorProdutoNFe.val() === 10) {
            instancia.ValorUnitario.def = "0,0000000000";
            instancia.ValorUnitario.configDecimal = { precision: 10, allowZero: true };
        } else {
            instancia.ValorUnitario.def = "0,00000";
            instancia.ValorUnitario.configDecimal = { precision: 5, allowZero: true };
        }

        var quantidade = Globalize.parseFloat(instancia.Quantidade.val());
        var valorUnitario = Globalize.parseFloat(instancia.ValorUnitario.val());

        if (quantidade > 0 && valorUnitario > 0) {
            var valorTotal = quantidade * valorUnitario;
            instancia.ValorTotalItem.val(Globalize.format(valorTotal, "n2"));
        }
    };
};

function HabilitarCamposICMS(instancia, v, efetivo) {
    LimparCampos(instancia);

    instancia.BaseICMS.enable(v);
    instancia.ReducaoBaseICMS.enable(v);
    instancia.AliquotaICMS.enable(v);
    instancia.ValorICMS.enable(v);

    instancia.BaseFCPICMS.enable(v);
    instancia.PercentualFCPICMS.enable(v);
    instancia.ValorFCPICMS.enable(v);

    instancia.BCICMSEfetivo.enable(efetivo);
    instancia.AliquotaICMSEfetivo.enable(efetivo);
    instancia.ReducaoBCICMSEfetivo.enable(efetivo);
    instancia.ValorICMSEfetivo.enable(efetivo);
}

function HabilitarCamposICMSMonoRet(instancia, v, retido) {
    LimparCampos(instancia);

    instancia.AliquotaRemICMSRet.enable(v);
    instancia.ValorICMSMonoRet.enable(v);
}

function HabilitarCamposICMSST(instancia, v, retido) {
    LimparCampos(instancia);

    instancia.BaseICMSST.enable(v);
    instancia.ReducaoBaseICMSST.enable(v);
    instancia.PercentualMVA.enable(v);
    instancia.AliquotaICMSST.enable(v);
    instancia.AliquotaInterestadual.enable(v);
    instancia.ValorICMSST.enable(v);

    instancia.BaseFCPICMSST.enable(v);
    instancia.PercentualFCPICMSST.enable(v);
    instancia.ValorFCPICMSST.enable(v);
    instancia.AliquotaFCPICMSST.enable(v);

    instancia.BCICMSSTRetido.enable(retido);
    instancia.AliquotaICMSSTRetido.enable(retido);
    instancia.ValorICMSSTSubstituto.enable(retido);
    instancia.ValorICMSSTRetido.enable(retido);
    instancia.BCICMSSTDestino.enable(retido);
    instancia.ValorICMSSTDestino.enable(retido);
}