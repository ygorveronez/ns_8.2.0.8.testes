/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

// #region Objetos Globais do Arquivo

var _preCargasAtualizadas;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PreCargasAtualizadas = function () {
    this.RegistrosAlterados = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Confirmar = PropertyEntity({ eventClick: confirmarAlteracoesClick, type: types.event, text: Localization.Resources.Cargas.Carga.ConfirmarAlteracoes, visible: ko.observable(true) });
    this.NaoAlterar = PropertyEntity({ eventClick: naoAlterarClick, type: types.event, text: Localization.Resources.Cargas.Carga.NaoAlterar, visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadPreCargaAtualizada() {
    _preCargasAtualizadas = new PreCargasAtualizadas();
    KoBindings(_preCargasAtualizadas, "knockoutPreCargasAlteradas");
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function confirmarAlteracoesClick() {

    executarReST("PreCarga/ConfirmarImportar", { RegistrosAlterados: JSON.stringify(_preCargasAtualizadas.RegistrosAlterados.val()) }, function (arg) {
        if (arg.Success) {
            if (arg.Data === true) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.Carga.Sucesso, Localization.Resources.Cargas.Carga.registrosAlteradosSucesso);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.Carga.Aviso, arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.Carga.Falha, arg.Msg);
        }
        Global.fecharModal("divModalPreCargasAlteradas");
    }, null);
}

function naoAlterarClick() {
    exibirConfirmacao(Localization.Resources.Cargas.Carga.Confimacao, Localization.Resources.Cargas.Carga.RealmenteDesejaFinalizarAtualizarPrePlanejamentoInformado, function () {
        Global.fecharModal("divModalPreCargasAlteradas");
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Publicas

function callbackExibirRegistraoQueSeraoAlterados(registrosAlterados) {
    _preCargasAtualizadas.RegistrosAlterados.val(registrosAlterados);
    var header = [
        { data: "NumeroPreCarga", title: Localization.Resources.Cargas.Carga.PrePlanejamento, width: "15%" },
        { data: "NumeroPedido", title: Localization.Resources.Cargas.Carga.Pedido, width: "15%" },
        { data: "PrevisaoChegadaDocaAnterior", title: Localization.Resources.Cargas.Carga.DeChegadaDoca, width: "15%" },
        { data: "PrevisaoChegadaDoca", title: Localization.Resources.Cargas.Carga.ParaChegadaDoca, width: "15%" },
        { data: "DataPrevisaoInicioViagemAnterior", title: Localization.Resources.Cargas.Carga.DeInicioViagem, width: "15%"},
        { data: "DataPrevisaoInicioViagem", title: Localization.Resources.Cargas.Carga.ParaInicioViagem, width: "15%" },
        { data: "DataPrevisaoFimViagemAnterior", title: Localization.Resources.Cargas.Carga.ParaFimViagem, width: "15%" },
        { data: "DataPrevisaoFimViagem", title: Localization.Resources.Cargas.Carga.DeFimVIagem, width: "15%" },
        { data: "DocaCarregamentoAnterior", title: Localization.Resources.Cargas.Carga.DeDoca, width: "15%" },
        { data: "DocaCarregamento", title: Localization.Resources.Cargas.Carga.ParaDoca, width: "15%" },
        { data: "CargaRetornoAnterior", title: Localization.Resources.Cargas.Carga.DeRetorno, width: "10%" },
        { data: "CargaRetorno", title: Localization.Resources.Cargas.Carga.ParaRetorno, width: "10%" }
    ];

    var gridTipoOperacao = new BasicDataTable(_preCargasAtualizadas.RegistrosAlterados.idGrid, header, null, { column: 1, dir: orderDir.asc });
    gridTipoOperacao.CarregarGrid(registrosAlterados);
    Global.abrirModal('divModalPreCargasAlteradas');
}

// #endregion Funções Públicas
