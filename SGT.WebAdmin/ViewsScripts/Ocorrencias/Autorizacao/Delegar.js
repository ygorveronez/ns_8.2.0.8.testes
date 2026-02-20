/// <reference path="Autorizacao.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />>
/// <reference path="../../Enumeradores/EnumSituacaoOcorrencia.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _delegarSelecionados;
var _delegar;

var DelegarSelecionados = function () {
    this.UsuarioDelegado = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.UsuarioResponsavel.getRequiredFieldDescription(), type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Observacao.getRequiredFieldDescription(), maxlength: 5000, required: ko.observable(false), visible: ko.observable(false) });

    this.Delegar = PropertyEntity({ eventClick: delegarOcorrenciasSelecionadosClick, type: types.event, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Delegar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarDelegarSelecionadosClick, type: types.event, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Cancelar, visible: ko.observable(true) });
};

var Delegar = function () {
    this.UsuarioDelegado = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.UsuarioResponsavel.getRequiredFieldDescription(), type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Observacao.getRequiredFieldDescription(), maxlength: 5000, required: ko.observable(false), visible: ko.observable(false) });

    this.Delegar = PropertyEntity({ eventClick: delegarClick, type: types.event, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Delegar, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadDelegar() {
    _delegar = new Delegar();
    KoBindings(_delegar, "knockoutDelegar");

    _delegarSelecionados = new DelegarSelecionados();
    KoBindings(_delegarSelecionados, "knockoutDelegarOcorrencia");

    new BuscarFuncionario(_delegar.UsuarioDelegado);
    new BuscarFuncionario(_delegarSelecionados.UsuarioDelegado);

    if (_CONFIGURACAO_TMS.SomenteAutorizadoresPodemDelegarOcorrencia) {
        _delegar.Observacao.required(true);
        _delegar.Observacao.visible(true);

        _delegarSelecionados.Observacao.required(true);
        _delegarSelecionados.Observacao.visible(true);
    }
}

function delegarClick() {
    if (!ValidarCamposObrigatorios(_delegar)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);
        return;
    }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.VoceRealmenteDesejaDelegarOcorrencia, function () {
        var dados = {
            Ocorrencia: _ocorrencia.Codigo.val(),
            UsuarioDelegado: _delegar.UsuarioDelegado.codEntity(),
            Observacao: _delegar.Observacao.val()
        };

        executarReST("AutorizacaoOcorrencia/DelegarOcorrencia", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                    buscarOcorrencias();
                    _gridRegras.CarregarGrid();
                    LimparDelegar();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        })
    });
}

function delegarOcorrenciasSelecionadosClick() {
    if (!ValidarCamposObrigatorios(_delegarSelecionados)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);
        return;
    }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.VoceRealmenteDesejaDelegarTodasOcorrencias, function () {
        var dados = RetornarObjetoPesquisa(_pesquisaOcorrencias);
        var delegar = RetornarObjetoPesquisa(_delegarSelecionados);

        dados.UsuarioDelegado = delegar.UsuarioDelegado;
        dados.Observacao = delegar.Observacao;
        dados.SelecionarTodos = _pesquisaOcorrencias.SelecionarTodos.val();
        dados.OcorrenciasSelecionadas = JSON.stringify(_gridOcorrencia.ObterMultiplosSelecionados());
        dados.OcorrenciasNaoSelecionadas = JSON.stringify(_gridOcorrencia.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoOcorrencia/DelegarMultiplasOcorrencias", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                    buscarOcorrencias();
                    cancelarDelegarSelecionadosClick();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        })
    });
}

function cancelarDelegarSelecionadosClick() {
    LimparCampos(_delegarSelecionados);
    Global.fecharModal("divModalDelegarOcorrencia");
}

//*******MÉTODOS*******

function CarregarDelegar(situacao) {
    var situacaoPermiteSelecaoDelegar = !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar && (situacao == EnumSituacaoOcorrencia.AgAprovacao || situacao == EnumSituacaoOcorrencia.AgAutorizacaoEmissao || situacao == EnumSituacaoOcorrencia.AutorizacaoPendente);
    if (situacaoPermiteSelecaoDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
    LimparCampos(_delegar);
}

function LimparDelegar() {
    LimparCampos(_delegar);
}