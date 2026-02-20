/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Globais.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../ViewsScripts/Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumOrientacaoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumOrdemAgrupamento.js" />
/// <reference path="AutomatizarGeracaoRelatorio.js" />

// #region Objetos Globais do Arquivo


var _fontes = [
    { text: "Arial", value: "Arial" },
    { text: "Times New Roman", value: "Times New Roman" },
    { text: "Calibri", value: "Calibri" }
];

var _tamanhoFonte = [
    { text: "5", value: "5" },
    { text: "6", value: "6" },
    { text: "7", value: "7" },
    { text: "8", value: "8" },
    { text: "9", value: "9" },
    { text: "10", value: "10" },
    { text: "11", value: "11" },
    { text: "12", value: "12" },
    { text: "13", value: "13" },
    { text: "14", value: "14" }
];

// #endregion Objetos Globais do Arquivo

// #region Classes

var MapeamentoRelatorioGlobal = function () {
    this.Report = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: ko.observable(true), visibleFade: ko.observable(false), idFade: guid(), text: Localization.Resources.Gerais.Geral.Relatorio.getFieldDescription(), idBtnSearch: guid() });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoControleRelatorios = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Titulo = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Titulo.getFieldDescription() });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
    this.Padrao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Gerais.Geral.TornarEsteRelatorioPadraoParaConsulta, issue: 853, visible: ko.observable(true) });
    this.ExibirSumarios = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Gerais.Geral.ExibirSumarios, issue: 845, visible: ko.observable(true) });
    this.CortarLinhas = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool, text: Localization.Resources.Gerais.Geral.OcultarConteudoExcedente, issue: 849 });
    this.FundoListrado = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Gerais.Geral.ExibirConteudoComFundoListrado, issue: 855 });
    this.TamanhoPadraoFonte = PropertyEntity({ val: ko.observable("10"), def: "10", getType: typesKnockout.int, options: _tamanhoFonte, text: Localization.Resources.Gerais.Geral.TamanhoDaFonte.getFieldDescription() });
    this.FontePadrao = PropertyEntity({ val: ko.observable("Arial"), def: "Arial", options: _fontes, text: Localization.Resources.Gerais.Geral.FontePadrao.getFieldDescription() });
    this.AgruparRelatorio = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Gerais.Geral.AgruparRelatorio, issue: 844, visible: ko.observable(true) });
    this.PropriedadeAgrupa = PropertyEntity({ val: ko.observable(""), def: "", options: new Array(), text: Localization.Resources.Gerais.Geral.AgruparPor.getFieldDescription(), visible: ko.observable(true) });
    this.OrdemAgrupamento = PropertyEntity({ val: ko.observable(EnumOrdemAgrupamento.Decrescente), def: EnumOrdemAgrupamento.Decrescente, options: EnumOrdemAgrupamento.obterOpcoes(), text: Localization.Resources.Gerais.Geral.OrdemDoAgrupamento.getFieldDescription(), visible: ko.observable(true) });
    this.PropriedadeOrdena = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Gerais.Geral.PropriedadeOrdena.getFieldDescription() });
    this.OrdemOrdenacao = PropertyEntity({ val: ko.observable(EnumOrdemAgrupamento.Decrescente), def: EnumOrdemAgrupamento.Decrescente, options: EnumOrdemAgrupamento.obterOpcoes(), text: Localization.Resources.Gerais.Geral.OrdemDaOrdenacao.getFieldDescription() });
    this.TipoArquivoRelatorio = PropertyEntity({ val: ko.observable(EnumTipoArquivoRelatorio.PDF), def: EnumTipoArquivoRelatorio.PDF, getType: typesKnockout.int });
    this.OrientacaoRelatorio = PropertyEntity({ text: Localization.Resources.Gerais.Geral.OrientacaoRelatorio.getFieldDescription(), val: ko.observable(EnumOrientacaoRelatorio.Retrato), def: EnumOrientacaoRelatorio.Retrato, options: EnumOrientacaoRelatorio.obterOpcoes(), getType: typesKnockout.int, visible: ko.observable(true) });
    this.Grid = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid(), idTab: guid() });
    this.NovoRelatorio = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool, text: Localization.Resources.Gerais.Geral.SalvarComoUmNovoRelatorio, issue: 852, visible: ko.observable(true) });
    this.OcultarDetalhe = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Gerais.Geral.OcultarDetalhesItensDoRelatorio, issue: 850, visible: ko.observable(true) });
    this.NovaPaginaAposAgrupamento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Gerais.Geral.CriarNovaPaginaAposAgrupamento, visible: ko.observable(true) });
    this.RelatorioParaTodosUsuarios = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Gerais.Geral.SalvarConfiguracaoDisponivelParaTodosUsuarios, issue: 854, visible: ko.observable(true) });
    this.GridPreview;

    this.ConfirmarSalvarRelatorio = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, idGrid: guid() });
    this.DesativarRelatorio = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.DesativarRelatorio, idGrid: guid() });
    this.SalvarRelatorio = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.SalvarConfiguracoes, enable: !_FormularioSomenteLeitura, idGrid: guid(), visible: ko.observable(true) });
};

var CRUDConfiguracaoRelatorioGlobal = function () {
    this.AutomatizarGeracaoRelatorio = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.AutomatizarGeracao, icon: "fal fa-history", enable: !_FormularioSomenteLeitura, visible: ko.observable(false) });
    this.ExibirConfiguracoes = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Configuracoes, icon: "fal fa-cogs", enable: !_FormularioSomenteLeitura, visible: ko.observable(true) });
};

var RelatorioGlobal = function (urlBuscarDadosrelatorio, gridPreview, callback, parametrosBuscarDadosRelatorio, idCRUD, knoutPesquisa, exibirBotaoSalvar) {
    this._knoutRelatorio = new MapeamentoRelatorioGlobal();
    this._knoutCRUDRelatorio = new CRUDConfiguracaoRelatorioGlobal();
    this._exibirBotaoSalvar = exibirBotaoSalvar;
    this._gridPreview = gridPreview;
    this._idConteudo = guid();
    this._idCRUD = idCRUD;
    this._knoutPesquisa = knoutPesquisa;
    this._parametrosBuscarDadosRelatorio = parametrosBuscarDadosRelatorio;
    this._urlBuscarDadosrelatorio = urlBuscarDadosrelatorio;
    this._automatizarGeracaoRelatorio = new AutomatizarGeracaoRelatorio(this._idConteudo, this);
    this._buscarDadosRelatorio(callback);
}

RelatorioGlobal.prototype = {
    atualizarDadosRelatorio: function (parametrosBuscarDadosRelatorio, callback) {
        this._parametrosBuscarDadosRelatorio = parametrosBuscarDadosRelatorio;
        this._buscarDadosRelatorio(callback);
    },
    gerarRelatorio: function (urlGerar, tipoArquivo) {
        var self = this;

        if ((self._knoutRelatorio.Grid.val() != null) && (self._gridPreview != null))
            self._knoutRelatorio.Grid.val(self._gridPreview.GetGrid());

        self._knoutRelatorio.TipoArquivoRelatorio.val(tipoArquivo);

        $.each(self._knoutPesquisa, function (i, prop) {
            if (prop.getType == typesKnockout.report) {
                self._knoutPesquisa[i].val(RetornarObjetoPesquisa(self._knoutRelatorio));
                return false;
            }
        });

        Salvar(self._knoutPesquisa, urlGerar, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    BuscarProcessamentosPendentes(function () {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.VoceSolicitouGeracaoDoRelatorioAssimQueTerminarVoceSeraNotificado, 5000);
                    });
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    },
    loadRelatorio: function (callback) {
        var self = this;

        $.get("Content/Static/Relatorios/Relatorios.html?dyn=" + guid(), function (data) {
            var html = data.replace(/#KnoutRelatorio/g, self._idConteudo);

            $("#divConfigRelatorio").html(html);

            KoBindings(self._knoutRelatorio, self._idConteudo, false);

            self._modalWindow = new bootstrap.Modal(document.getElementById(self._idConteudo + '-modal-salvar-relatorio'), { keyboard: true, backdrop: 'static', });

            if (self._idCRUD != null)
                KoBindings(self._knoutCRUDRelatorio, self._idCRUD);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe || self._exibirBotaoSalvar === false) {
                self._knoutRelatorio.NovoRelatorio.visible(false);
                self._knoutRelatorio.RelatorioParaTodosUsuarios.visible(false);
                self._knoutRelatorio.Padrao.visible(false);
            }

            $("#" + self._knoutRelatorio.AgruparRelatorio.id).click(function () { self._alterarAgrupamento(); });

            self._knoutRelatorio.ConfirmarSalvarRelatorio.eventClick = function () { self._confirmarSalvarRelatorio(); };
            self._knoutRelatorio.DesativarRelatorio.eventClick = function () { self._desativarRelatorio(); };
            self._knoutRelatorio.SalvarRelatorio.eventClick = function () { self._exibirModalSalvarRelatorio(); };
            self._knoutRelatorio.PropriedadeAgrupa.eventChange = function () { self._alterarAgrupamento(); };
            self._knoutRelatorio.OrdemAgrupamento.eventChange = function () { self._alterarAgrupamento(); };

            if (self._idCRUD != null) {
                self._automatizarGeracaoRelatorio.carregar(self._knoutRelatorio.CodigoControleRelatorios.val());
                self._knoutCRUDRelatorio.AutomatizarGeracaoRelatorio.visible(self._isPermitirAutomatizarGeracaoRelatorio());
                self._knoutCRUDRelatorio.AutomatizarGeracaoRelatorio.eventClick = function () { self._exibirModalAutomatizarGeracaoRelatorio(); };
                self._knoutCRUDRelatorio.ExibirConfiguracoes.eventClick = function () { self._knoutRelatorio.Report.visibleFade(!self._knoutRelatorio.Report.visibleFade()); };
            }

            if (callback instanceof Function)
                callback();

            if (Boolean(self._knoutPesquisa) && Boolean(self._knoutPesquisa.TipoRelatorio)) {
                new BuscarRelatorio(self._knoutPesquisa.TipoRelatorio, self._knoutRelatorio.CodigoControleRelatorios.val(), function (arg) {
                    self._knoutPesquisa.TipoRelatorio.codEntity(arg.Codigo);
                    self._knoutPesquisa.TipoRelatorio.val(arg.Descricao);
                    self._knoutRelatorio.Report.codEntity(arg.Codigo);
                    self._knoutRelatorio.Report.val(arg.Descricao);
                    self._knoutRelatorio.Codigo.val(self._knoutRelatorio.Report.codEntity());

                    self._buscarDadosRelatorio(null);
                });
            }

            LocalizeCurrentPage();
        });
    },
    obterGridPreview: function () {
        return this._gridPreview;
    },
    obterKnoutPesquisa: function () {
        return this._knoutPesquisa;
    },
    obterKnoutRelatorio: function () {
        return this._knoutRelatorio;
    },
    obterUrl: function () {
        var urlSeparada = this._urlBuscarDadosrelatorio.split('/');
        var urlSeparadaSemAcao = urlSeparada.slice(0, -1);

        return urlSeparadaSemAcao.join('/');
    },
    _alterarAgrupamento: function () {
        if (this._knoutRelatorio.Grid.val() != null) {
            this._knoutRelatorio.Grid.val().group = { enable: this._knoutRelatorio.AgruparRelatorio.val(), propAgrupa: this._knoutRelatorio.PropriedadeAgrupa.val(), dirOrdena: this._knoutRelatorio.OrdemAgrupamento.val() };

            if (this._gridPreview != null)
                this._gridPreview.SetGroup(this._knoutRelatorio.Grid.val().group);
        }
    },
    _buscarDadosRelatorio: function (callback) {
        var self = this;
        var data = $.extend({}, { Codigo: self._knoutRelatorio.Codigo.val() }, self._parametrosBuscarDadosRelatorio);

        executarReST(self._urlBuscarDadosrelatorio, data, function (retorno) {
            if (retorno.Success) {
                PreencherObjetoKnout(self._knoutRelatorio, retorno);

                if (Boolean(self._knoutPesquisa) && Boolean(self._knoutPesquisa.TipoRelatorio)) {
                    self._knoutPesquisa.TipoRelatorio.codEntity(retorno.Data.Report.Codigo);
                    self._knoutPesquisa.TipoRelatorio.val(retorno.Data.Report.Descricao);
                }

                if (self._knoutRelatorio.Grid.val() != null) {
                    self._knoutRelatorio.Grid.val().order = new Array();
                    self._knoutRelatorio.Grid.val().order.push({ column: self._knoutRelatorio.Grid.val().indiceColunaOrdena, dir: self._knoutRelatorio.Grid.val().dirOrdena });
                    self._knoutRelatorio.PropriedadeAgrupa.options = new Array();

                    $.each(self._knoutRelatorio.Grid.val().header, function (i, head) {
                        if (head.enableGroup)
                            self._knoutRelatorio.PropriedadeAgrupa.options.push({ text: head.title, value: head.data });
                    });

                    if (self._gridPreview != null)
                        self._gridPreview.SetGrid(self._knoutRelatorio.Grid.val());
                }

                if (callback instanceof Function)
                    callback();
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        })
    },
    _confirmarSalvarRelatorio: function () {
        var self = this;

        if (!self._knoutRelatorio.Descricao) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Gerais.Geral.InformeUmaDescricaoParaRelatorio);
            return;
        }

        if ((self._knoutRelatorio.Grid.val() != null) && (self._gridPreview != null))
            self._knoutRelatorio.Grid.val(self._gridPreview.GetGrid());

        var data = { Relatorio: JSON.stringify(obterObjetoRelatorioParaConsulta(self._knoutRelatorio)) };

        executarReST("Relatorios/Relatorio/SalvarRelatorio", data, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.RelatorioFoiSalvoComSucessoPodeSerUsadoEmUmaProximaConsulta);
                    self._fecharModalSalvarRelatorio();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    },
    _desativarRelatorio: function () {
        var self = this;

        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.RealmenteDesejaDesativarRelatorio.format(self._knoutRelatorio.Descricao.val()), function () {
            Salvar(self._knoutRelatorio, "Relatorios/Relatorio/DesativarRelatorio", function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.RelatorioFoiDesativadoComSucesso);
                        self._fecharModalSalvarRelatorio();
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            });
        });
    },
    _exibirModalAutomatizarGeracaoRelatorio: function () {
        this._knoutRelatorio.Report.visibleFade(false);
        this._automatizarGeracaoRelatorio.exibirModal();
    },
    _exibirModalSalvarRelatorio: function () {
        this._knoutRelatorio.Padrao.val(false);
        this._knoutRelatorio.NovoRelatorio.val(true);
        this._knoutRelatorio.RelatorioParaTodosUsuarios.val(false);

        this._modalWindow.show();
    },
    _fecharModalSalvarRelatorio: function () {
        this._modalWindow.hide();
    },
    _isPermitirAutomatizarGeracaoRelatorio: function () {
        return (
            _CONFIGURACAO_TMS.ConfiguracaoRelatorio.UtilizaAutomacaoEnvioRelatorio &&
            (
                _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador ||
                _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS
            )
        );
    }
}

// #endregion Classes

// #region Funções Públicas

function obterObjetoRelatorioParaConsulta(knoutRelatorio) {
    var tempReport = $.extend({}, RetornarObjetoPesquisa(knoutRelatorio));

    if (tempReport.Grid != null) {
        tempReport.PropriedadeAgrupa = tempReport.Grid.group.enable ? tempReport.Grid.group.propAgrupa : "";
        tempReport.OrdemAgrupamento = tempReport.Grid.group.dirOrdena;

        $.each(tempReport.Grid.header, function (i, head) {
            if (i == tempReport.Grid.order[0].column) {
                tempReport.PropriedadeOrdena = head.data;
                return false;
            }
        });

        tempReport.OrdemOrdenacao = tempReport.Grid.order[0].dir;
        tempReport.Grid = JSON.stringify(tempReport.Grid);
    }

    return tempReport;
}

// #endregion Funções Públicas
