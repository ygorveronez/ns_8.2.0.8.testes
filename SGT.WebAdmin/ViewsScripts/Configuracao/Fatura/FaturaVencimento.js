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
/// <reference path="../../../js/plugin/promise/promise.js" />

var ConfiguracaoFaturaVencimentoModel = function (instancia, enable) {

    this.Codigo = PropertyEntity();

    this.DiaInicial = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.DiaInicial.getRequiredFieldDescription(), val: ko.observable(""), getType: typesKnockout.int, visible: ko.observable(true), required: ko.observable(true), maxlength: 2 });
    this.DiaFinal = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.DiaFinal.getRequiredFieldDescription(), val: ko.observable(""), getType: typesKnockout.int, visible: ko.observable(true), required: ko.observable(true), maxlength: 2 });
    this.DiaVencimento = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.int, def: "", text: Localization.Resources.Configuracao.Fatura.DiaVencimento.getRequiredFieldDescription(), required: ko.observable(true), visible: ko.observable(true), maxlength: 2 });

    this.Vencimentos = PropertyEntity({ idGrid: guid() });

    this.Adicionar = PropertyEntity({ eventClick: instancia.Adicionar, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(enable) });
    this.Atualizar = PropertyEntity({ eventClick: instancia.Atualizar, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false), enable: ko.observable(enable) });
    this.Excluir = PropertyEntity({ eventClick: instancia.Excluir, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false), enable: ko.observable(enable) });
    this.Cancelar = PropertyEntity({ eventClick: instancia.Cancelar, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false), enable: ko.observable(enable) });
};

function ConfiguracaoFaturaVencimento(idContent, enable, knoutFaturaVencimento, callback) {

    var instancia = this;
    var gridVencimentos;

    this.ObterListaFaturaVencimentos = function () {
        return knoutFaturaVencimento.val();
    };

    this.LimparListaFaturaVencimentos = function () {
        instancia.LimparCampos();
        instancia.RecarregarGrid();
    };

    this.Load = function (callback) {

        $.get("Content/Static/Configuracao/FaturaVencimento.html?dyn=" + guid(), function (data) {

            $("#" + idContent).html(data);

            instancia.Configuracao = new ConfiguracaoFaturaVencimentoModel(instancia, enable);
            KoBindings(instancia.Configuracao, idContent);

            var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: instancia.Editar }] };

            if (!enable)
                menuOpcoes = null;

            var header = [
                { data: "DiaInicial", title: Localization.Resources.Configuracao.Fatura.DiaInicial, width: "30%" },
                { data: "DiaFinal", title: Localization.Resources.Configuracao.Fatura.DiaFinal, width: "30%" },
                { data: "DiaVencimento", title: Localization.Resources.Configuracao.Fatura.DiaVencimento, width: "30%" },
                { data: "Codigo", visible: false }
            ];

            gridVencimentos = new BasicDataTable(instancia.Configuracao.Vencimentos.idGrid, header, menuOpcoes, { column: 0, dir: orderDir.asc });
            instancia.RecarregarGrid();

            if (callback != null)
                callback();

        });
    };

    this.RecarregarGrid = function () {
        gridVencimentos.CarregarGrid(knoutFaturaVencimento.val());
    };

    this.Editar = function (row) {
        var data = { Data: row }
        PreencherObjetoKnout(instancia.Configuracao, data);
        instancia.Configuracao.Atualizar.visible(true);
        instancia.Configuracao.Cancelar.visible(true);
        instancia.Configuracao.Excluir.visible(true);
        instancia.Configuracao.Adicionar.visible(false);
    };

    this.Adicionar = function () {
        if (ValidarCamposObrigatorios(instancia.Configuracao)) {
            if (instancia.PermitirAdicionarVencimento()) {
                var lista = knoutFaturaVencimento.val();
                var data = ConverterObjetoFaturaVencimento(instancia.Configuracao);
                lista.push(data);
                instancia.LimparCampos();
                instancia.RecarregarGrid();
                knoutFaturaVencimento.val(lista);
            }
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    };

    this.Atualizar = function () {
        if (ValidarCamposObrigatorios(instancia.Configuracao)) {
            if (instancia.PermitirAdicionarVencimento()) {
                var data = ConverterObjetoFaturaVencimento(instancia.Configuracao);
                var lista = knoutFaturaVencimento.val();
                for (var i = 0; i < lista.length; i++) {
                    if (data.Codigo == lista[i].Codigo) {
                        lista[i] = data;
                        break;
                    }
                }
                knoutFaturaVencimento.val(lista);

                instancia.LimparCampos();
                instancia.RecarregarGrid();
            }
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    };

    this.Excluir = function () {
        var data = ConverterObjetoFaturaVencimento(instancia.Configuracao);
        var lista = knoutFaturaVencimento.val();
        for (var i = 0; i < lista.length; i++) {
            if (data.Codigo == lista[i].Codigo) {
                lista.splice(i, 1);
                break;
            }
        }
        knoutFaturaVencimento.val(lista);
        instancia.LimparCampos();
        instancia.RecarregarGrid();
    };

    this.Cancelar = function () {
        instancia.LimparCampos();
    };

    this.LimparCampos = function () {
        instancia.Configuracao.Atualizar.visible(false);
        instancia.Configuracao.Cancelar.visible(false);
        instancia.Configuracao.Excluir.visible(false);
        instancia.Configuracao.Adicionar.visible(true);
        LimparCampos(instancia.Configuracao);
    };

    this.PermitirAdicionarVencimento = function () {
        if (instancia.Configuracao.DiaInicial.val() > 31 || instancia.Configuracao.DiaFinal.val() > 31 || instancia.Configuracao.DiaVencimento.val() > 31) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Configuracao.Fatura.DiaInvalido, Localization.Resources.Configuracao.Fatura.DiaNaoPodeSerMaiorQueTrincaUm);
            return false;
        }
        else if (Globalize.parseFloat(instancia.Configuracao.DiaInicial.val()) > Globalize.parseFloat(instancia.Configuracao.DiaFinal.val())) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Configuracao.Fatura.DiaInvalido, Localization.Resources.Configuracao.Fatura.DiaInicialMaiorQueFinal);
            return false;
        }

        return true;
    };

    return instancia.Load(callback);
};

function ConverterObjetoFaturaVencimento(instancia) {

    var objetoFaturaVencimentoConfig = {
        Codigo: instancia.Codigo.val(),
        DiaInicial: instancia.DiaInicial.val(),
        DiaFinal: instancia.DiaFinal.val(),
        DiaVencimento: instancia.DiaVencimento.val()
    }
    return objetoFaturaVencimentoConfig;
}
