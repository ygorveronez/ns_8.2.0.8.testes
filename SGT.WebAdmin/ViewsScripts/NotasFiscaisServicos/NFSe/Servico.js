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
/// <reference path="NFSe.js" />
/// <reference path="../../Consultas/ServicoNFSe.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Enumeradores/EnumExigibilidadeISS.js" />

var _exigibilidadeISS = [
    { text: "Exigível", value: EnumExigibilidadeISS.Exigivel },
    { text: "Não incidência", value: EnumExigibilidadeISS.NaoInicidencia },
    { text: "Isenção", value: EnumExigibilidadeISS.Isencao },
    { text: "Exportação", value: EnumExigibilidadeISS.Exportacao },
    { text: "Imunidade", value: EnumExigibilidadeISS.Imunidade },
    { text: "Suspensa por Decisão Judicial", value: EnumExigibilidadeISS.SuspensaDecisaoJudicial },
    { text: "Suspensa por Processo Administrativo", value: EnumExigibilidadeISS.SuspensaProcessoAdministrativo }
];

var Servico = function (nfse, listaServico) {

    var instancia = this;

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, enable: ko.observable(true) });

    this.ServicoItem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Serviço:", idBtnSearch: guid(), required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.LocalidadeServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Localidade Serviço:", idBtnSearch: guid(), required: true, enable: ko.observable(true), visible: ko.observable(true) });

    this.Quantidade = PropertyEntity({ def: "", val: ko.observable(""), text: "*Quantidade:", getType: typesKnockout.decimal, maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: false } });
    this.ValorUnitario = PropertyEntity({ def: "", val: ko.observable(""), text: "*Valor Unitário:", getType: typesKnockout.decimal, maxlength: 22, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: false } });
    this.DescontoIncondicional = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Desconto Incondicional:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.DescontoCondicional = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Desconto Condicional:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.Deducao = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Deduções:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorTotalItem = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "*Valor Total:", getType: typesKnockout.decimal, maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.BaseISS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base ISS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaISS = PropertyEntity({ def: "0,0000", val: ko.observable("0,0000"), text: "Alíquota ISS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 4, allowZero: true } });
    this.ValorISS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor ISS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } })
    this.NBS = PropertyEntity({def: "",val: ko.observable(""),text: "NBS:", maxlength: 9, required: false, visible: ko.observable(true), enable: ko.observable(true)});
    this.CodigoIndicadorOperacao = PropertyEntity({def: "",val: ko.observable(""),text: "Código Indicador Operação:",maxlength: 6,required: false,visible: ko.observable(true),enable: ko.observable(true)});
    this.CSTIBSCBS = PropertyEntity({def: "",val: ko.observable(""),text: "CST IBS/CBS:",maxlength: 5,required: false,visible: ko.observable(true),enable: ko.observable(true)});
    this.ClassificacaoTributariaIBSCBS = PropertyEntity({def: "",val: ko.observable(""),text: "Classificação Tributária IBS/CBS:",maxlength: 8,required: false,visible: ko.observable(true),enable: ko.observable(true)});
    this.BaseCalculoIBSCBS = PropertyEntity({def: "0,000",val: ko.observable("0,000"),text: "Base Cálculo IBS/CBS:",getType: typesKnockout.decimal,maxlength: 18,required: false,visible: ko.observable(true),enable: ko.observable(true),configDecimal: { precision: 2, allowZero: true }});
    this.AliquotaIBSEstadual = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% Alíquota:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.PercentualReducaoIBSEstadual = PropertyEntity({ def: "0,000", val: ko.observable("0,000"), text: "% Percentual Redução:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 3, allowZero: true } });
    this.ValorIBSEstadual = PropertyEntity({ def: "0,000", val: ko.observable("0,000"), text: "Valor:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 3, allowZero: true } });
    this.ValorIBSEstadualBruto = PropertyEntity({ def: "0,000", val: ko.observable("0,000"), text: "Valor Bruto:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 3, allowZero: true } });
    this.AliquotaIBSMunicipal = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% Alíquota:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.PercentualReducaoIBSMunicipal = PropertyEntity({ def: "0,000", val: ko.observable("0,000"), text: "% Percentual Redução:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 3, allowZero: true } });
    this.ValorIBSMunicipalBruto = PropertyEntity({ def: "0,000", val: ko.observable("0,000"), text: "Valor Bruto:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 3, allowZero: true } });
    this.ValorIBSMunicipal = PropertyEntity({ def: "0,000", val: ko.observable("0,000"), text: "Valor:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 3, allowZero: true } });
    this.AliquotaCBS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% Alíquota:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.PercentualReducaoCBS = PropertyEntity({ def: "0,000", val: ko.observable("0,000"), text: "Percentual Redução:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 3, allowZero: true } });
    this.ValorCBS = PropertyEntity({ def: "0,000", val: ko.observable("0,000"), text: "Valor:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 3, allowZero: true } });
    this.ValorCBSBruto = PropertyEntity({ def: "0,000", val: ko.observable("0,000"), text: "Valor Bruto:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 3, allowZero: true } });
    this.AliquotaEfetivaIBSEstadual = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% Alíquota Efetiva:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaEfetivaIBSMunicipal = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% Alíquota Efetiva:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaEfetivaCBS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% Alíquota Efetiva:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.Exigibilidade = PropertyEntity({ val: ko.observable(EnumExigibilidadeISS.Exigivel), def: EnumExigibilidadeISS.Exigivel, options: _exigibilidadeISS, text: "*Exigibilidade ISS:", required: true, enable: ko.observable(true) });

    this.CSTPIS = PropertyEntity({ type: types.map, val: ko.observable(""), def: "", getType: typesKnockout.string, text: "CST PIS:", maxlength: 5, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.CSTCOFINS = PropertyEntity({ type: types.map, val: ko.observable(""), def: "", getType: typesKnockout.string, text: "CST COFINS:", maxlength: 5, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.BasePIS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base PIS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.BaseCOFINS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base COFINS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaPIS = PropertyEntity({ def: "0,0000", val: ko.observable("0,0000"), text: "Alíquota PIS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 4, allowZero: true } });
    this.AliquotaCOFINS = PropertyEntity({ def: "0,0000", val: ko.observable("0,0000"), text: "Alíquota COFINS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 4, allowZero: true } });
    this.ValorPIS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor PIS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorCOFINS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor COFINS:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.Discriminacao = PropertyEntity({ text: "Discriminação: ", required: false, maxlength: 255, enable: ko.observable(true) });

    this.SalvarItemNFSe = PropertyEntity({ eventClick: function () { instancia.AdicionarServico(); }, type: types.event, text: "Salvar Item", visible: ko.observable(true), enable: ko.observable(true) });

    this.AdicionarServico = function () {
        var valido = ValidarCamposObrigatorios(instancia);
        nfse.NFSe.Pessoa.requiredClass("form-control");
        nfse.NFSe.NaturezaOperacao.requiredClass("form-control");

        if (valido) {
            if (nfse.NFSe.Pessoa.codEntity() == 0 || nfse.NFSe.Pessoa.val() == "") {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe a Pessoa antes de lançar o Serviço!");
                $("#tabNFSe_" + nfse.NFSe.IdModal.val() + " a:eq(0)").tab("show");
                nfse.NFSe.Pessoa.requiredClass("form-control is-invalid");
                return;
            } else if (nfse.NFSe.NaturezaOperacao.codEntity() == 0 || nfse.NFSe.NaturezaOperacao.val() == "") {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe a Natureza de Operação antes de lançar o Serviço!");
                $("#tabNFSe_" + nfse.NFSe.IdModal.val() + " a:eq(0)").tab("show");
                nfse.NFSe.NaturezaOperacao.requiredClass("form-control is-invalid");
                return;
            }
        }

        if (valido) {
            if (instancia.Codigo.val() > "0" && instancia.Codigo.val() != undefined) {
                for (var i = 0; i < nfse.Servicos.length; i++) {
                    if (instancia.Codigo.val() == nfse.Servicos[i].Codigo) {
                        nfse.Servicos.splice(i, 1);
                        break;
                    }
                }
            }
            instancia.Codigo.val(guid());
            nfse.Servicos.push({
                Codigo: instancia.Codigo.val(),
                CodigoServico: instancia.ServicoItem.codEntity(),
                Descricao: instancia.ServicoItem.val(),
                CodigoLocalidade: instancia.LocalidadeServico.codEntity(),
                DescricaoLocalidade: instancia.LocalidadeServico.val(),
                Qtd: instancia.Quantidade.val(),
                ValorUnitario: instancia.ValorUnitario.val(),
                Deducao: instancia.Deducao.val(),
                DescontoIncondicional: instancia.DescontoIncondicional.val(),
                DescontoCondicional: instancia.DescontoCondicional.val(),
                ValorTotal: instancia.ValorTotalItem.val(),
                BCISS: instancia.BaseISS.val(),
                AliquotaISS: instancia.AliquotaISS.val(),
                ValorISS: instancia.ValorISS.val(),

                CSTPIS: instancia.CSTPIS.val(),
                CSTCOFINS: instancia.CSTCOFINS.val(),
                BasePIS: instancia.BasePIS.val(),
                BaseCOFINS: instancia.BaseCOFINS.val(),
                AliquotaPIS: instancia.AliquotaPIS.val(),
                AliquotaCOFINS: instancia.AliquotaCOFINS.val(),
                ValorPIS: instancia.ValorPIS.val(),
                ValorCOFINS: instancia.ValorCOFINS.val(),

                CodigoExigibilidade: instancia.Exigibilidade.val(),
                Discriminacao: instancia.Discriminacao.val(),
                NBS: instancia.NBS.val(),
                CodigoIndicadorOperacao: instancia.CodigoIndicadorOperacao.val(),
                CSTIBSCBS: instancia.CSTIBSCBS.val(),
                ClassificacaoTributariaIBSCBS: instancia.ClassificacaoTributariaIBSCBS.val(),
                BaseCalculoIBSCBS: instancia.BaseCalculoIBSCBS.val(),
                AliquotaIBSEstadual: instancia.AliquotaIBSEstadual.val(),
                PercentualReducaoIBSEstadual: instancia.PercentualReducaoIBSEstadual.val(),
                ValorIBSEstadual: instancia.ValorIBSEstadual.val(),
                AliquotaIBSMunicipal: instancia.AliquotaIBSMunicipal.val(),
                PercentualReducaoIBSMunicipal: instancia.PercentualReducaoIBSMunicipal.val(),
                ValorIBSMunicipal: instancia.ValorIBSMunicipal.val(),
                AliquotaCBS: instancia.AliquotaCBS.val(),
                PercentualReducaoCBS: instancia.PercentualReducaoCBS.val(),
                ValorCBS: instancia.ValorCBS.val(),
                AliquotaEfetivaIBSEstadual: instancia.AliquotaEfetivaIBSEstadual.val(),
                AliquotaEfetivaIBSMunicipal: instancia.AliquotaEfetivaIBSMunicipal.val(),
                AliquotaEfetivaCBS: instancia.AliquotaEfetivaCBS.val(),
                ValorIBSEstadualBruto: instancia.ValorIBSEstadualBruto.val(),
                ValorIBSMunicipalBruto: instancia.ValorIBSMunicipalBruto.val(),
                ValorCBSBruto: instancia.ValorCBSBruto.val()
            });

            instancia.SomarValores(nfse);

            listaServico.RecarregarGrid();

            LimparCampos(instancia);
            $("#tabItensNFSe_" + nfse.NFSe.IdModal.val() + " a:eq(0)").tab("show");

            if (nfse.NFSe.CidadePrestacao.codEntity() > 0) {
                instancia.LocalidadeServico.codEntity(nfse.NFSe.CidadePrestacao.codEntity());
                instancia.LocalidadeServico.val(nfse.NFSe.CidadePrestacao.val());
            }

            $("#" + instancia.ServicoItem.id).focus();
        } else {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        }
    };

    this.SomarValores = function (nfse) {
        var baseISSItem = Globalize.parseFloat(instancia.BaseISS.val());
        var baseISSTotal = Globalize.parseFloat(nfse.Valor.BaseISS.val());
        var baseISS = baseISSItem + baseISSTotal;
        nfse.Valor.BaseISS.val(Globalize.format(baseISS, "n2"));

        var aliquotaISSItem = Globalize.parseFloat(instancia.AliquotaISS.val());
        /*var aliquotaISSTotal = Globalize.parseFloat(nfse.Valor.AliquotaISS.val());
        var aliquotaISS = aliquotaISSItem + aliquotaISSTotal;
        nfse.Valor.AliquotaISS.val(Globalize.format(aliquotaISS, "n2"));*/
        nfse.Valor.AliquotaISS.val(Globalize.format(aliquotaISSItem, "n4"));

        var valorISSItem = Globalize.parseFloat(instancia.ValorISS.val());
        var valorISSTotal = Globalize.parseFloat(nfse.Valor.ValorISS.val());
        var valorISS = valorISSItem + valorISSTotal;
        nfse.Valor.ValorISS.val(Globalize.format(valorISS, "n2"));

        var baseDeducaoItem = Globalize.parseFloat(instancia.Deducao.val());
        var baseDeducaoTotal = Globalize.parseFloat(nfse.Valor.BaseDeducao.val());
        var baseDeducao = baseDeducaoItem + baseDeducaoTotal;
        nfse.Valor.BaseDeducao.val(Globalize.format(baseDeducao, "n2"));

        var valorDescontoIncondicionalItem = Globalize.parseFloat(instancia.DescontoIncondicional.val());
        var valorDescontoIncondicionalTotal = Globalize.parseFloat(nfse.Valor.ValorDescontoIncondicional.val());
        var valorDescontoIncondicional = valorDescontoIncondicionalItem + valorDescontoIncondicionalTotal;
        nfse.Valor.ValorDescontoIncondicional.val(Globalize.format(valorDescontoIncondicional, "n2"));

        var valorDescontoCondicionalItem = Globalize.parseFloat(instancia.DescontoCondicional.val());
        var valorDescontoCondicionalTotal = Globalize.parseFloat(nfse.Valor.ValorDescontoCondicional.val());
        var valorDescontoCondicional = valorDescontoCondicionalItem + valorDescontoCondicionalTotal;
        nfse.Valor.ValorDescontoCondicional.val(Globalize.format(valorDescontoCondicional, "n2"));

        var valorTotalItem = Globalize.parseFloat(instancia.ValorTotalItem.val());
        var valorTotalServicosTotal = Globalize.parseFloat(nfse.Valor.ValorTotalServicos.val());
        var valorTotalServicos = valorTotalItem + valorTotalServicosTotal;
        nfse.Valor.ValorTotalServicos.val(Globalize.format(valorTotalServicos, "n2"));

        var valorISSRetido = 0;
        if (nfse.Valor.RetencaoISS.val() === EnumSimNao.Sim) {
            var valorISSRetidoTotal = Globalize.parseFloat(nfse.Valor.ValorRetencaoISS.val());
            var valorRetencaoCalculadoItem = baseISSItem * (aliquotaISSItem / 100);
            valorISSRetido = valorISSRetidoTotal + valorRetencaoCalculadoItem;
            if (valorISSRetido < 0)
                valorISSRetido = 0;
            nfse.Valor.ValorRetencaoISS.val(Globalize.format(valorISSRetido, "n2"));
        }

        var valorliquidoTotal = valorTotalServicos - valorISSRetido;
        nfse.Valor.ValorTotalLiquido.val(Globalize.format(valorliquidoTotal, "n2"));

        nfse.Valor.RetencaoISS.enable(true);



        var basePis = Globalize.parseFloat(instancia.BasePIS.val());
        var basePisTotal = Globalize.parseFloat(nfse.Valor.BasePIS.val());
        var novaBasePis = basePisTotal + basePis;
        nfse.Valor.BasePIS.val(Globalize.format(novaBasePis, "n2"));

        var baseCofins = Globalize.parseFloat(instancia.BaseCOFINS.val());
        var baseCofinsTotal = Globalize.parseFloat(nfse.Valor.BaseCOFINS.val());
        var novaBaseCofins = baseCofinsTotal + baseCofins;
        nfse.Valor.BaseCOFINS.val(Globalize.format(novaBaseCofins, "n2"));

        nfse.Valor.AliquotaPIS.val(Globalize.format(instancia.AliquotaPIS.val(), "n4"));
        nfse.Valor.AliquotaCOFINS.val(Globalize.format(instancia.AliquotaCOFINS.val(), "n4"));

        var aliquotaPIS = Globalize.parseFloat(instancia.AliquotaPIS.val()) / 100;
        var valorPIS = novaBasePis * aliquotaPIS;
        nfse.Valor.ValorPIS.val(Globalize.format(valorPIS, "n2"));

        var aliquotaCOFINS = Globalize.parseFloat(instancia.AliquotaCOFINS.val()) / 100;
        var valorCOFINS = novaBaseCofins * aliquotaCOFINS;
        nfse.Valor.ValorCOFINS.val(Globalize.format(valorCOFINS, "n2"));
    };

    this.Load = function () {
        KoBindings(instancia, nfse.IdKnockoutServico);

        new BuscarServicoNFSe(instancia.ServicoItem, null, instancia.RetornoServicoNFSe);
        new BuscarLocalidadesBrasil(instancia.LocalidadeServico);
    };

    this.DestivarServico = function () {
        DesabilitarCamposInstanciasNFSe(instancia);
    };

    this.HabilitarServico = function () {
        HabilitarCamposInstanciasNFSe(instancia);
    };

    this.RetornoServicoNFSe = function (dado) {
        instancia.ServicoItem.val(dado.Descricao);
        instancia.ServicoItem.codEntity(dado.Codigo);
        instancia.AliquotaISS.val(dado.Aliquota);

        var valorUnitario = Globalize.parseFloat(dado.ValorServico);
        if (valorUnitario > 0)
            instancia.ValorUnitario.val(Globalize.format(valorUnitario, "n2"));
        else
            instancia.ValorUnitario.val("");
    };

    this.BaseISS.val.subscribe(function () {
        CalcularValorISS(instancia, nfse);
    });

    this.AliquotaISS.val.subscribe(function () {
        CalcularValorISS(instancia, nfse);
    });
};

function CalcularTotalItem(instancia) {
    var quantidade = Globalize.parseFloat(instancia.Quantidade.val());
    var valorUnitario = Globalize.parseFloat(instancia.ValorUnitario.val());
    var descontoIncondicional = Globalize.parseFloat(instancia.DescontoIncondicional.val());
    var deducao = Globalize.parseFloat(instancia.Deducao.val());

    if (quantidade > 0 && valorUnitario > 0) {
        var valorTotal = (quantidade * valorUnitario) - descontoIncondicional - deducao;
        instancia.ValorTotalItem.val(Globalize.format(valorTotal, "n2"));
        instancia.BaseISS.val(Globalize.format(valorTotal, "n2"));
    }
    else {
        instancia.ValorTotalItem.val(Globalize.format(0, "n2"));
        instancia.BaseISS.val(Globalize.format(0, "n2"));
    }
}

function CalcularValorISS(instancia, nfse) {
    var baseISS = Globalize.parseFloat(instancia.BaseISS.val());
    var aliquotaISS = Globalize.parseFloat(instancia.AliquotaISS.val());

    if (baseISS > 0 && aliquotaISS > 0 && nfse.Valor.RetencaoISS.val() == EnumSimNao.Nao) {
        var valor = baseISS * (aliquotaISS / 100);
        instancia.ValorISS.val(Globalize.format(valor, "n2"));
    } else
        instancia.ValorISS.val(Globalize.format(0, "n2"));
}