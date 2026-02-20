/// <reference path="../../Consultas/ApoliceSeguro.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
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
/// <reference path="../../Consultas/RateioFormula.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoCTeDocumentos.js" />
/// <reference path="../../../js/plugin/promise/promise.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />

var ConfiguracaoEmissaoComponentesModel = function (instancia, enable) {

    this.Codigo = PropertyEntity();
    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.ComponenteFrete.ComponentesDeFrete.getRequiredFieldDescription(), required: true, idBtnSearch: guid(), enable: ko.observable(enable), issue: 85 });
    this.CobrarOutroDocumento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(enable) });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.ComponenteFrete.CobrarValorEmOutroDocumentoFiscal.getRequiredFieldDescription(), enable: ko.observable(false), required: false, idBtnSearch: guid(), issue: 370 });

    this.UsarOutraFormulaRateio = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(enable) });
    this.FormulaRateioFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.ComponenteFrete.UsarOutraFormulaDeRateio.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(false), required: false, visible: ko.observable(true), issue: 257 });
    this.IncluirICMS = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Consultas.ComponenteFrete.IncluirICMS, getType: typesKnockout.bool, enable: ko.observable(enable) });
    this.IncluirIntegralmenteContratoFreteTerceiro = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Consultas.ComponenteFrete.DeveSerInclusoIntegralmenteNoContratoDeFreteDoTerceiro, getType: typesKnockout.bool, enable: ko.observable(enable) });
    
    this.ImprimirOutraDescricaoCTe = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(enable) });
    this.DescricaoCTe = PropertyEntity({ text: Localization.Resources.Consultas.ComponenteFrete.ExibirOutraDescricaoComponente, required: false, enable: ko.observable(false) });

    this.Componentes = PropertyEntity({ idGrid: guid() });

    this.Adicionar = PropertyEntity({ eventClick: instancia.Adicionar, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(enable) });
    this.Atualizar = PropertyEntity({ eventClick: instancia.Atualizar, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false), enable: ko.observable(enable) });
    this.Excluir = PropertyEntity({ eventClick: instancia.Excluir, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false), enable: ko.observable(enable) });
    this.Cancelar = PropertyEntity({ eventClick: instancia.Cancelar, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false), enable: ko.observable(enable) });
}


function ConfiguracaoComponentesDeFrete(idContent, enable, knoutComponenteFrete, callback) {

    var instancia = this;
    var gridComponentes;

    this.ObterListaCompomentes = function () {
        return knoutComponenteFrete.val();
    };

    this.LimparListaComponentes = function () {
        instancia.LimparCampos();
        instancia.RecarregarGrid();
    }

    this.Load = function (callback) {

        $.get("Content/Static/Configuracao/ComponentesFrete.html?dyn=" + guid(), function (data) {

            $("#" + idContent).html(data);

            instancia.Configuracao = new ConfiguracaoEmissaoComponentesModel(instancia, enable);
            KoBindings(instancia.Configuracao, idContent);

            new BuscarRateioFormulas(instancia.Configuracao.FormulaRateioFrete);
            new BuscarModeloDocumentoFiscal(instancia.Configuracao.ModeloDocumentoFiscal, null, null, true);
            new BuscarComponentesDeFrete(instancia.Configuracao.ComponenteFrete);

            var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: instancia.Editar }] };

            if (!enable)
                menuOpcoes = null;

            var header = [{ data: "ComponenteFrete.Descricao", title: Localization.Resources.Consultas.ComponenteFrete.Componente, width: "30%" },
            { data: "FormulaRateioFrete.Descricao", title: Localization.Resources.Consultas.ComponenteFrete.Rateio, width: "20%" },
            { data: "ModeloDocumentoFiscal.Descricao", title: Localization.Resources.Consultas.ComponenteFrete.OutroDocumento, width: "20%" },
            { data: "DescricaoCTe", title: Localization.Resources.Consultas.ComponenteFrete.OutraDescricao, width: "20%" },
            { data: "ComponenteFrete", visible: false },
            { data: "CobrarOutroDocumento", visible: false },
            { data: "ModeloDocumentoFiscal", visible: false },
            { data: "ImprimirOutraDescricaoCTe", visible: false },
            { data: "DescricaoCTe", visible: false },
            { data: "FormulaRateioFrete", visible: false },
            { data: "UsarOutraFormulaRateio", visible: false },
            { data: "Codigo", visible: false }
            ];

            gridComponentes = new BasicDataTable(instancia.Configuracao.Componentes.idGrid, header, menuOpcoes);
            instancia.RecarregarGrid();

            $("#" + instancia.Configuracao.CobrarOutroDocumento.id).click(instancia.CobrarOutroDocumentoClick);
            $("#" + instancia.Configuracao.ImprimirOutraDescricaoCTe.id).click(instancia.ImprimirOutraDescricaoCTeClick);
            $("#" + instancia.Configuracao.UsarOutraFormulaRateio.id).click(instancia.UsarOutraFormulaRateioCTeClick);

            if (callback != null)
                callback();

        });
    };

    this.CobrarOutroDocumentoClick = function () {
        if (instancia.Configuracao.CobrarOutroDocumento.val()) {
            instancia.Configuracao.ModeloDocumentoFiscal.required = true;
            instancia.Configuracao.ModeloDocumentoFiscal.enable(true);
        } else {
            instancia.Configuracao.ModeloDocumentoFiscal.enable(false);
            instancia.Configuracao.ModeloDocumentoFiscal.required = false;
            instancia.Configuracao.ModeloDocumentoFiscal.val("");
            instancia.Configuracao.ModeloDocumentoFiscal.codEntity(0);
        }
    }

    this.UsarOutraFormulaRateioCTeClick = function () {
        if (instancia.Configuracao.UsarOutraFormulaRateio.val()) {
            instancia.Configuracao.FormulaRateioFrete.required = true;
            instancia.Configuracao.FormulaRateioFrete.enable(true);
        } else {
            instancia.Configuracao.FormulaRateioFrete.enable(false);
            instancia.Configuracao.FormulaRateioFrete.required = false;
            instancia.Configuracao.FormulaRateioFrete.val("");
            instancia.Configuracao.FormulaRateioFrete.codEntity(0);
        }
    }

    this.ImprimirOutraDescricaoCTeClick = function () {
        if (instancia.Configuracao.ImprimirOutraDescricaoCTe.val()) {
            instancia.Configuracao.DescricaoCTe.required = true;
            instancia.Configuracao.DescricaoCTe.enable(true);
        } else {
            instancia.Configuracao.DescricaoCTe.enable(false);
            instancia.Configuracao.DescricaoCTe.required = false;
            instancia.Configuracao.DescricaoCTe.val("");
        }
    }

    this.RecarregarGrid = function () {
        gridComponentes.CarregarGrid(knoutComponenteFrete.val());
    };

    this.Editar = function (row) {
        var data = { Data: row }
        PreencherObjetoKnout(instancia.Configuracao, data);
        instancia.Configuracao.Atualizar.visible(true);
        instancia.Configuracao.Cancelar.visible(true);
        instancia.Configuracao.Excluir.visible(true);
        instancia.Configuracao.Adicionar.visible(false);

        if (instancia.Configuracao.ImprimirOutraDescricaoCTe.val())
            instancia.Configuracao.DescricaoCTe.enable(true);
        else
            instancia.Configuracao.DescricaoCTe.enable(false);

        if (instancia.Configuracao.UsarOutraFormulaRateio.val())
            instancia.Configuracao.FormulaRateioFrete.enable(true);
        else
            instancia.Configuracao.FormulaRateioFrete.enable(false);

        if (instancia.Configuracao.CobrarOutroDocumento.val())
            instancia.Configuracao.ModeloDocumentoFiscal.enable(true);
        else
            instancia.Configuracao.ModeloDocumentoFiscal.enable(false);
    };

    this.Adicionar = function () {
        if (ValidarCamposObrigatorios(instancia.Configuracao)) {
            if (instancia.PermitirAdicionarComponente()) {
                var lista = knoutComponenteFrete.val();
                var data = ConverterObjetoComponenteFrete(instancia.Configuracao);
                lista.push(data);
                instancia.LimparCampos();
                instancia.RecarregarGrid();
                knoutComponenteFrete.val(lista);
            }
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    };

    this.Atualizar = function () {
        if (ValidarCamposObrigatorios(instancia.Configuracao)) {
            if (instancia.PermitirAdicionarComponente()) {
                var data = ConverterObjetoComponenteFrete(instancia.Configuracao);
                var lista = knoutComponenteFrete.val();
                for (var i = 0; i < lista.length; i++) {
                    if (data.Codigo == lista[i].Codigo) {
                        lista[i] = data;
                        break;
                    }
                }
                knoutComponenteFrete.val(lista);
            }
            instancia.LimparCampos();
            instancia.RecarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    };

    this.Excluir = function () {
        var data = ConverterObjetoComponenteFrete(instancia.Configuracao);
        var lista = knoutComponenteFrete.val();
        for (var i = 0; i < lista.length; i++) {
            if (data.Codigo == lista[i].Codigo) {
                lista.splice(i, 1);
                break;
            }
        }
        knoutComponenteFrete.val(lista);
        instancia.LimparCampos();
        instancia.RecarregarGrid();
    };

    this.Cancelar = function () {
        instancia.LimparCampos();
    };

    this.LimparCampos = function () {
        instancia.Configuracao.DescricaoCTe.enable(false);
        instancia.Configuracao.FormulaRateioFrete.enable(false);
        instancia.Configuracao.ModeloDocumentoFiscal.enable(false);
        instancia.Configuracao.Atualizar.visible(false);
        instancia.Configuracao.Cancelar.visible(false);
        instancia.Configuracao.Excluir.visible(false);
        instancia.Configuracao.Adicionar.visible(true);
        LimparCampos(instancia.Configuracao);
    };

    this.PermitirAdicionarComponente = function () {
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
            return true;

        var retorno = true;
        for (var i = 0; i < knoutComponenteFrete.val().length; i++) {
            if (instancia.Configuracao.ComponenteFrete.codEntity() == knoutComponenteFrete.val()[i].ComponenteFrete.Codigo && instancia.Configuracao.Codigo.val() != knoutComponenteFrete.val()[i].Codigo) {
                retorno = false;
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Consultas.ComponenteFrete.ComponenteJaAdicionado, Localization.Resources.Consultas.ComponenteFrete.JaExisteUmaConfiguracaoCriadaParaComponente + knoutComponenteFrete.val()[i].ComponenteFrete.Descricao + ".");
                break;
            }
        }
        return retorno;
    };

    return instancia.Load(callback);
}

function ConverterObjetoComponenteFrete(instancia) {

    var objetoComponenteFreteConfig = {
        Codigo: instancia.Codigo.val(),
        ComponenteFrete: { Codigo: instancia.ComponenteFrete.codEntity(), Descricao: instancia.ComponenteFrete.val() },
        ModeloDocumentoFiscal: { Codigo: instancia.ModeloDocumentoFiscal.codEntity(), Descricao: instancia.ModeloDocumentoFiscal.val() },
        FormulaRateioFrete: { Codigo: instancia.FormulaRateioFrete.codEntity(), Descricao: instancia.FormulaRateioFrete.val() },
        CobrarOutroDocumento: instancia.CobrarOutroDocumento.val(),
        ImprimirOutraDescricaoCTe: instancia.ImprimirOutraDescricaoCTe.val(),
        DescricaoCTe: instancia.DescricaoCTe.val(),
        UsarOutraFormulaRateio: instancia.UsarOutraFormulaRateio.val(),
        IncluirICMS: instancia.IncluirICMS.val(),
        IncluirIntegralmenteContratoFreteTerceiro: instancia.IncluirIntegralmenteContratoFreteTerceiro.val(),
    }
    return objetoComponenteFreteConfig;
}
