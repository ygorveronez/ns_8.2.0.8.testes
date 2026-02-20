/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Globais.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/Container.js" />
/// <reference path="../../../Consultas/LocalRetiradaContainer.js" />

// #region Objetos Globais do Arquivo

var _buscaLocalRetiradaContainer;
var _cargaRetiradaContainer;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CargaRetiradaContainer = function () {
    var self = this;

    this.Local = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.LocalRetirada.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.LocalSugerido = PropertyEntity({ col: 12, type: types.entity, codEntity: self.Local.codEntity, val: self.Local.val, entityDescription: self.Local.entityDescription, idBtnSearch: guid(), visible: false });
    this.ContainerTipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), text: Localization.Resources.Cargas.Carga.ContainerTipo.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, enable: false });
    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Container, idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.Mensagem = PropertyEntity({});

    this.Atualizar = PropertyEntity({ eventClick: atualizarCargaRetiradaContainerClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(true) });
    this.BuscarLocalDisponivel = PropertyEntity({ eventClick: buscarLocalDisponivelClick, type: types.event, text: Localization.Resources.Cargas.Carga.BuscarLocalRetiradaDisponivel, visible: ko.observable(true) });
    this.LiberarSemRetiradaContainer = PropertyEntity({ eventClick: liberarCargaSemRetiradaContainerClick, type: types.event, text: Localization.Resources.Cargas.Carga.LiberarSemRetiradaContainer, visible: ko.observable(true) });
    this.RemoverContainerCarga = PropertyEntity({ eventClick: removerContainerCargaRetiradaClick, type: types.event, text: Localization.Resources.Cargas.Carga.RemoverContainer, visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadCargaRetiradaContainer() {
    _cargaRetiradaContainer = new CargaRetiradaContainer();
    KoBindings(_cargaRetiradaContainer, "knockoutInformarRetiradaContainer");

    new BuscarClientes(_cargaRetiradaContainer.Local, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
    new BuscarContainers(_cargaRetiradaContainer.Container, null, null, false, null, _cargaRetiradaContainer.ContainerTipo, _cargaRetiradaContainer.Local, EnumStatusColetaContainer.EmAreaEsperaVazio);
    _buscaLocalRetiradaContainer = new BuscarLocalRetiradaContainer(_cargaRetiradaContainer.LocalSugerido, null, _cargaRetiradaContainer.ContainerTipo);

    loadCargaRetiradaContainerAnexo();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function atualizarCargaRetiradaContainerClick() {
    var dados = {
        Carga: _cargaAtual.Codigo.val(),
        Local: _cargaRetiradaContainer.Local.codEntity(),
        Container: _cargaRetiradaContainer.Container.codEntity()
    };

    executarReST("Carga/InformarRetiradaContainer", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.RetiradaDeContainerAtualizaComSucesso);
                fecharModalCargaRetiradaContainer();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function buscarLocalDisponivelClick() {
    _buscaLocalRetiradaContainer.abrirBusca();
}

function liberarCargaSemRetiradaContainerClick() {
    executarReST("Carga/LiberarSemRetiradaContainer", { Carga: _cargaAtual.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CargaLiberadaSemRetiradaContainerComSucesso);
                fecharModalCargaRetiradaContainer();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function removerContainerCargaRetiradaClick() {
    exibirConfirmacao("Confirmação", "Tem certeza que deseja remover o container? Fazendo isso o container volta a situação anterior e a carga anterior.", function () {
        executarReST("ColetaContainer/RemoverContainerRetiradaContainer", { Carga: _cargaAtual.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.RetiradaDeContainerAtualizaComSucesso);
                    fecharModalCargaRetiradaContainer();
                } else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            } else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, null);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirCargaRetiradaContainer() {

    executarReST("Carga/ObterRetiradaContainer", { Carga: _cargaAtual.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_cargaRetiradaContainer, retorno);

                _cargaRetiradaContainer.Local.enable(retorno.Data.PermitirEditar);
                _cargaRetiradaContainer.Container.enable(retorno.Data.PermitirEditar);

                _cargaRetiradaContainer.Atualizar.visible(retorno.Data.PermitirEditar);
                _cargaRetiradaContainer.BuscarLocalDisponivel.visible(retorno.Data.PermitirEditar);
                _cargaRetiradaContainer.LiberarSemRetiradaContainer.visible(retorno.Data.PermitirEditar && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermitirLiberarSemRetiradaContainer, _PermissoesPersonalizadasCarga));

                _cargaRetiradaContainerAnexo.CodigoContainer.val(_cargaRetiradaContainer.Container.codEntity());
                _cargaRetiradaContainerAnexo.CodigoCarga.val(_cargaAtual.Codigo.val());

                _gridCargaRetiradaContainerAnexo.CarregarGrid();

                if (retorno.Data.PermiteRemoverContainer) {
                    _cargaRetiradaContainer.RemoverContainerCarga.visible(true);
                } else {
                    _cargaRetiradaContainer.RemoverContainerCarga.visible(false);
                }

                setarvisibilidadeBotoesRetirada();

                exibirModalCargaRetiradaContainer();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirModalCargaRetiradaContainer() {
    Global.abrirModal("divModalInformarRetiradaContainer");
    $("#divModalInformarRetiradaContainer").one('hidden.bs.modal', function () {
        LimparCampos(_cargaRetiradaContainer);
    });
}

function fecharModalCargaRetiradaContainer() {
    Global.fecharModal("divModalInformarRetiradaContainer");
}

function setarvisibilidadeBotoesRetirada() {
    //PARA REDESPACHO NAO DEVE APARECER OS BOTOES;
    if (_cargaAtual.CargaRedespacho.val() || _cargaAtual.TipoContratacaoCarga.val() == EnumTipoContratacaoCarga.SVMProprio || _cargaAtual.TipoContratacaoCarga.val() == EnumTipoContratacaoCarga.Redespacho || _cargaAtual.TipoContratacaoCarga.val() == EnumTipoContratacaoCarga.RedespachoIntermediario || !string.IsNullOrWhiteSpace(_cargaAtual.Redespacho.val())) {
        _cargaRetiradaContainer.Atualizar.visible(true);
        _cargaRetiradaContainer.BuscarLocalDisponivel.visible(false);
        _cargaRetiradaContainer.LiberarSemRetiradaContainer.visible(false);
    }
}

// #endregion Funções Privadas
