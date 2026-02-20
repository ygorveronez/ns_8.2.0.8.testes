/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="../CheckList/CheckList.js" />
/// <reference path="FluxoPatio.js" />
/// <reference path="ObservacoesEtapas.js" />

// #region Objetos Globais do Arquivo

var _mapeamentoEtapaChecklist;
var _$spanTituloCheckListPrev;
var _$spanTituloCheckListRealiz;

// #endregion Objetos Globais do Arquivo

// #region Funções de Inicialização

function OnLoadDocaCheckList() {
    AdicionaSpanDataCheckList();
}

// #endregion Funções de Inicialização

var MapeamentoEtapaChecklist = function () {
    this.EtapaCheckList = PropertyEntity({ text: ko.observable("Etapa do Checklist:")});
};

// #region Funções Públicas

function ExibirDetalhesCheckListFluxoPatio(knoutFluxo, opt) {
    _callbackCheckListAtualizado = AtualizarCheckListGestaoPatio;
    _fluxoAtual = knoutFluxo;

    executarReST("CheckList/BuscarPorCodigo", { FluxoGestaoPatio: knoutFluxo.Codigo.val(), EtapaCheckList: EnumEtapaChecklist.Checklist }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                var primeiraEtapa = _fluxoAtual.EtapaAtual.val() == 0;

                limparCamposCheckList();
                PreencherObjetoKnout(_checkList, arg);
                preecherRetornoCheckLit(arg, primeiraEtapa);

                _checkList.RetornosGR(arg.Data.RetornosGR);
                _checkList.EtapaAntecipada.val(!opt.etapaLiberada);

                if (arg.Data.DataFimCheckListPrevista != "") {
                    _$spanTituloCheckListPrev.find("span").text(arg.Data.DataFimCheckListPrevista);
                    _$spanTituloCheckListRealiz.find("span").text(arg.Data.DataFimCheckList);
                }
                else {
                    _$spanTituloCheckListPrev.hide();
                    _$spanTituloCheckListRealiz.hide();
                }

                if (edicaoEtapaFluxoPatioBloqueada())
                    ocultarBotoesEtapa(_checkList);

                AjustesNomesEtapas(EnumEtapaChecklist.Checklist);
                                
                Global.abrirModal('divModalDetalhesCheckList');
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function ExibirDetalhesAvaliacaoDescargaFluxoPatio(knoutFluxo, opt) {
    _callbackCheckListAtualizado = AtualizarCheckListGestaoPatio;
    _fluxoAtual = knoutFluxo;

    executarReST("CheckList/BuscarPorCodigo", { FluxoGestaoPatio: knoutFluxo.Codigo.val(), EtapaCheckList: EnumEtapaChecklist.AvaliacaoDescarga }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                var primeiraEtapa = _fluxoAtual.EtapaAtual.val() == 0;

                limparCamposCheckList();
                PreencherObjetoKnout(_checkList, arg);
                preecherRetornoCheckLit(arg, primeiraEtapa);

                _checkList.RetornosGR(arg.Data.RetornosGR);

                if (arg.Data.DataFimCheckListPrevista != "") {
                    _$spanTituloCheckListPrev.find("span").text(arg.Data.DataFimCheckListPrevista);
                    _$spanTituloCheckListRealiz.find("span").text(arg.Data.DataFimCheckList);
                }
                else {
                    _$spanTituloCheckListPrev.hide();
                    _$spanTituloCheckListRealiz.hide();
                }

                if (edicaoEtapaFluxoPatioBloqueada())
                    ocultarBotoesEtapa(_checkList);

                AjustesNomesEtapas(EnumEtapaChecklist.AvaliacaoDescarga);

                Global.abrirModal('divModalDetalhesCheckList');
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function AtualizarCheckListGestaoPatio() {
    atualizarFluxoPatio();
    Global.fecharModal('divModalDetalhesCheckList');
}

function AdicionaSpanDataCheckList() {
    var $div = $(".fluxo-patio-modal").show();

    var _htmlCampos = function (name, id) {
        return [
            '<section class="col col-12" id="' + id + '">',
            '<label>',
            '<b>' + name + ': </b>',
            '<span></span>',
            '</label>',
            '</section>'
        ].join('');
    }

    $div.find(".fields-container").html(
        _htmlCampos(Localization.Resources.GestaoPatio.FluxoPatio.PrevisaoTermino, 'previsao-termino') +
        _htmlCampos(Localization.Resources.GestaoPatio.FluxoPatio.CheckListConcluido, 'checklist-concluido')
    );

    _$spanTituloCheckListPrev = $div.find("#previsao-termino");
    _$spanTituloCheckListRealiz = $div.find("#checklist-concluido");
}

function AjustesNomesEtapas(etapaChecklist) {
    if (etapaChecklist == EnumEtapaChecklist.AvaliacaoDescarga) {
        _mapeamentoEtapaChecklist = new MapeamentoEtapaChecklist();
        KoBindings(_mapeamentoEtapaChecklist, "knockoutMapeamentoEtapaChecklist");

        _checkList.EtapaCheckList.val(etapaChecklist);
        _checkList.PreencherChecklist.text("Preencher Avaliação Descarga");
        _mapeamentoEtapaChecklist.EtapaCheckList.text("Avaliação Descarga");
    } else {
        _mapeamentoEtapaChecklist = new MapeamentoEtapaChecklist();
        KoBindings(_mapeamentoEtapaChecklist, "knockoutMapeamentoEtapaChecklist");

        _checkList.EtapaCheckList.val(etapaChecklist);
        _checkList.PreencherChecklist.text("Preencher Checklist");
        _mapeamentoEtapaChecklist.EtapaCheckList.text("Checklist");
    }
}

// #endregion Funções Privadas
