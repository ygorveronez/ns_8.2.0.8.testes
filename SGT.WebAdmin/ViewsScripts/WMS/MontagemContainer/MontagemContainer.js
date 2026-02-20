/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/TipoContainer.js" />
/// <reference path="../../Consultas/Porto.js" />
/// <reference path="../../Consultas/Container.js" />
/// <reference path="../../Enumeradores/EnumStatusMontagemContainer.js" />
/// <reference path="MontagemContainerNotaFiscal.js" />

//#region Propriedades Globais

var _montagemContainer;
var _pesquisaMontagemContainer;
var _gridPesquisaMontagemContainer;
var _gridMontagemContainerNotaFiscal;
var _montagemContainerInformacoes;
var _CRUDMontagemContainer;

//#endregion

//#region Mapeamento e Load

var MontagemContainer = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusMontagemContainer.Todos), def: EnumStatusMontagemContainer.Todos, type: types.map });
    this.TipoContainer = PropertyEntity({ codEntity: ko.observable(0), required: true, type: types.entity, text: "*Tipo de Container:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.PortoOrigem = PropertyEntity({ codEntity: ko.observable(0), required: true, type: types.entity, text: "*Porto Origem:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.PortoDestino = PropertyEntity({ codEntity: ko.observable(0), required: true, type: types.entity, text: "*Porto Destino:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Container = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Container:", idBtnSearch: guid() });
    this.NumeroBooking = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), required: false, text: "N° Booking:", maxlength: 150 });
};

var CRUDMontagemContainer = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarMontagemContainerClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarMontagemContainerClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: limparCamposMontagemContainer, type: types.event, text: "Limpar", visible: ko.observable(true) });

    this.Reabrir = PropertyEntity({ eventClick: reabrirMontagemContainerClick, type: types.event, text: "Reabrir", visible: ko.observable(false) });
    this.Finalizar = PropertyEntity({ eventClick: finalizarMontagemContainerClick, type: types.event, text: "Finalizar", visible: ko.observable(false) });
}

var MontagemContainerInformacoes = function () {
    this.StatusMontagemContainer = PropertyEntity({ val: ko.observable(""), text: "Status: " });
    this.IDMontagemContainer = PropertyEntity({ val: ko.observable("0"), text: "ID Montagem Container: " });
    this.PesoContainer = PropertyEntity({ val: ko.observable("0"), text: "Peso do Container: " });
    this.TaraContainer = PropertyEntity({ val: ko.observable("0"), text: "Tara do Container: " });
    this.MetroCubicoContainer = PropertyEntity({ val: ko.observable("0"), text: "M³ do Container: " });
    this.PesoNotas = PropertyEntity({ val: ko.observable("0"), text: "Peso das Notas: " });
    this.MetroCubicoNotas = PropertyEntity({ val: ko.observable("0"), text: "M³ das Notas: " });
    this.QuantidadeVolumesNotas = PropertyEntity({ val: ko.observable("0"), text: "Qtd. Volumes das Notas: " });
    this.PorcentagemComposicaoContainerPeso = PropertyEntity({ val: ko.observable("0%"), text: "% de Composição do Container (Peso): ", cssClass: ko.observable("progress-bar bg-color-pastel-green") });
    this.PorcentagemComposicaoContainerMetragem = PropertyEntity({ val: ko.observable("0%"), text: "% de Composição do Container (Metragem Cúbica): ", cssClass: ko.observable("progress-bar bg-color-pastel-green") });
}

var PesquisaMontagemContainer = function () {
    this.NumeroBooking = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), required: false, text: "N° Booking:", maxlength: 150 });
    this.TipoContainer = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Tipo de Container:", idBtnSearch: guid() });
    this.Container = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Container:", idBtnSearch: guid() });
    this.Status = PropertyEntity({ type: types.map, val: ko.observable(EnumStatusMontagemContainer.Todos), options: EnumStatusMontagemContainer.obterOpcoesPesquisa(), text: "Status:" });
    this.IDMontagemContainer = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(""), text: "ID Montagem Container:" });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridMontagemContainer, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

function loadMontagemContainer() {
    _montagemContainer = new MontagemContainer();
    _pesquisaMontagemContainer = new PesquisaMontagemContainer();
    _montagemContainerInformacoes = new MontagemContainerInformacoes();
    _CRUDMontagemContainer = new CRUDMontagemContainer();

    KoBindings(_montagemContainer, "knockoutCadastroMontagemContainer");
    KoBindings(_pesquisaMontagemContainer, "knockoutPesquisaMontagemContainer");
    KoBindings(_CRUDMontagemContainer, "knockoutCRUDMontagemContainer");
    KoBindings(_montagemContainerInformacoes, "knockoutMontagemContainerInformacoes");

    new BuscarTiposContainer(_montagemContainer.TipoContainer, retornoTipoContainer);
    new BuscarPorto(_montagemContainer.PortoOrigem);
    new BuscarPorto(_montagemContainer.PortoDestino);
    new BuscarContainers(_montagemContainer.Container);
    new BuscarContainers(_pesquisaMontagemContainer.Container);
    new BuscarTiposContainer(_pesquisaMontagemContainer.TipoContainer);

    loadMontagemContainerNotaFiscal();
    loadGridPesquisaMontagemContainer();
}

function loadGridPesquisaMontagemContainer() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarMontagemContainerClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridPesquisaMontagemContainer = new GridView(_pesquisaMontagemContainer.Pesquisar.idGrid, "MontagemContainer/Pesquisa", _pesquisaMontagemContainer, menuOpcoes);
    _gridPesquisaMontagemContainer.CarregarGrid();
}

//#endregion

//#region Métodos de Click

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function adicionarMontagemContainerClick() {
    if (!ValidarCamposObrigatorios(_montagemContainer)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    var dados = {
        CodigoTipoContainer: _montagemContainer.TipoContainer.codEntity(),
        CodigoContainer: _montagemContainer.Container.codEntity(),
        NumeroBooking: _montagemContainer.NumeroBooking.val(),
        CodigoPortoOrigem: _montagemContainer.PortoOrigem.codEntity(),
        CodigoPortoDestino: _montagemContainer.PortoDestino.codEntity(),
        NotasFiscais: JSON.stringify(_montagemContainerNotaFiscal.GridNotaFiscal.BuscarRegistros())
    };

    executarReST("MontagemContainer/Adicionar", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Montagem de Container adicionada com sucesso.");
                _gridPesquisaMontagemContainer.CarregarGrid();
                limparCamposMontagemContainer();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function atualizarMontagemContainerClick() {
    if (!ValidarCamposObrigatorios(_montagemContainer)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    var dados = {
        Codigo: _montagemContainer.Codigo.val(),
        CodigoTipoContainer: _montagemContainer.TipoContainer.codEntity(),
        CodigoContainer: _montagemContainer.Container.codEntity(),
        NumeroBooking: _montagemContainer.NumeroBooking.val(),
        CodigoPortoOrigem: _montagemContainer.PortoOrigem.codEntity(),
        CodigoPortoDestino: _montagemContainer.PortoDestino.codEntity(),
        NotasFiscais: JSON.stringify(_montagemContainerNotaFiscal.GridNotaFiscal.BuscarRegistros())
    };

    executarReST("MontagemContainer/Atualizar", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Montagem de Container atualizada com sucesso.");
                _gridPesquisaMontagemContainer.CarregarGrid();
                limparCamposMontagemContainer();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function reabrirMontagemContainerClick() {
    var promise = new Promise(
        function (resolve, reject) {
            alterarStatusMontagemContainer(EnumStatusMontagemContainer.Aberto, resolve, reject);
        });

    promise.then(function (status) {
        _montagemContainer.Status.val(status);
        _montagemContainerInformacoes.StatusMontagemContainer.val(EnumStatusMontagemContainer.obterDescricao(status));
        controlarCampos();
        controlarVisibilidadeBotoes();
    }).catch(function (err) {
        throw new Error(err);
    });
}

function finalizarMontagemContainerClick() {
    var promise = new Promise(
        function (resolve, reject) {
            alterarStatusMontagemContainer(EnumStatusMontagemContainer.Finalizado, resolve, reject);
        });

    promise.then(function (status) {
        _montagemContainer.Status.val(status);
        _montagemContainerInformacoes.StatusMontagemContainer.val(EnumStatusMontagemContainer.obterDescricao(status));
        controlarCampos();
        controlarVisibilidadeBotoes();
    }).catch(function (err) {
        throw new Error(err);
    });
}

function editarMontagemContainerClick(registroSelecionado) {
    limparCamposMontagemContainer();

    executarReST("MontagemContainer/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_montagemContainer, { Data: retorno.Data.DadosGerais });
                PreencherObjetoKnout(_montagemContainerInformacoes, { Data: retorno.Data.InformacoesContainer });
                controlarVisibilidadeBotoes();
                controlarCampos();
                _pesquisaMontagemContainer.ExibirFiltros.visibleFade(false);
                _montagemContainerNotaFiscal.GridNotaFiscal.CarregarGrid(retorno.Data.NotasFiscais);
                obterResumoInformacoesMontagemContainer();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

//#endregion

//#region Métodos Privados

function recarregarGridMontagemContainer() {
    _gridPesquisaMontagemContainer.CarregarGrid();
}

function retornoTipoContainer(registro) {
    _montagemContainer.TipoContainer.codEntity(registro.Codigo);
    _montagemContainer.TipoContainer.val(registro.Descricao);
    _montagemContainerInformacoes.PesoContainer.val(registro.PesoContainer);
    _montagemContainerInformacoes.TaraContainer.val(registro.TaraContainer);
    _montagemContainerInformacoes.MetroCubicoContainer.val(registro.MetroCubicoContainer);
}

function limparCamposMontagemContainer() {
    Global.ResetarAbas();
    LimparCampos(_montagemContainer);
    _montagemContainerNotaFiscal.GridNotaFiscal.CarregarGrid([]);

    controlarVisibilidadeBotoes();
    controlarCampos();
}

function controlarVisibilidadeBotoes() {
    _CRUDMontagemContainer.Atualizar.visible(_montagemContainer.Codigo.val() > 0);
    _CRUDMontagemContainer.Adicionar.visible(_montagemContainer.Codigo.val() <= 0);
    _CRUDMontagemContainer.Finalizar.visible(_montagemContainer.Status.val() == EnumStatusMontagemContainer.Aberto);
    _CRUDMontagemContainer.Reabrir.visible(_montagemContainer.Status.val() == EnumStatusMontagemContainer.Finalizado);
}

function controlarCampos() {
    _montagemContainer.TipoContainer.enable(_montagemContainer.Status.val() != EnumStatusMontagemContainer.Finalizado);
    _montagemContainer.PortoOrigem.enable(_montagemContainer.Status.val() != EnumStatusMontagemContainer.Finalizado);
    _montagemContainer.PortoDestino.enable(_montagemContainer.Status.val() != EnumStatusMontagemContainer.Finalizado);
}

function alterarStatusMontagemContainer(status, resolve, reject) {
    var data = {
        Status: status,
        Codigo: _montagemContainer.Codigo.val()
    };

    executarReST("MontagemContainer/AlterarStatus", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Status alterado com sucesso.");
                resolve(status);
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                reject(retorno.Msg);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            reject(retorno.Msg);
        }
    });
}

//#endregion