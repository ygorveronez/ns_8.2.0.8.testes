/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Enumeradores/EnumTipoTransportadorCentroCarregamento.js" />
/// <reference path="CentroCarregamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _centroCarregamentoTransportador;
var _centroCarregamentoTransportadorCadastro;
var _crudCentroCarregamentoTransportadorCadastro;
var _gridCentroCarregamentoTransportador;
var _gridCentroCarregamentoTransportadorClientesDestino;
var _gridCentroCarregamentoTransportadorTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras;
var _gridCentroCarregamentoTransportadorLocalidadesDestino;
var _gridCentroCarregamentoTransportadorTipoCarga;

/*
 * Declaração das Classes
 */

var CRUDCentroCarregamentoTransportadorCadastro = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarCentroCarregamentoTransportadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarCentroCarregamentoTransportadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirCentroCarregamentoTransportadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var CentroCarregamentoTransportador = function () {
    var self = this;

    this.PermitirMatrizSelecionarFilial = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.PermitirMatrizSelecionarFilial, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.LiberarCargaManualmenteParaTransportadores = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.LiberarCargasMaualmenteParaAsTransportadoras, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.LiberarCargaAutomaticamenteParaTransportadoras = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.LiberarCargasAutomaticamenteParaAsTransportadoras, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.LiberarCargaAutomaticamenteParaTransportadorasForaRota = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.LiberarCargasAutomaticamenteParaAsTransportadorasForaDaRota, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.AguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.AguardarConfirmacaoDoTransportador, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.PermitirLiberarCargaTransportadorExclusivo = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.PermitirLiberarCargasParaTransportadorExclusivo, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.TipoTransportadorCentroCarregamento = PropertyEntity({ val: ko.observable(EnumTipoTransportadorCentroCarregamento.Todos), options: EnumTipoTransportadorCentroCarregamento.obterOpcoes(), text: Localization.Resources.Logistica.CentroCarregamento.TipoDeTransportador.getFieldDescription(), def: EnumTipoTransportadorCentroCarregamento.Todos });
    this.TipoTransportadorSecundarioCentroCarregamento = PropertyEntity({ val: ko.observable(EnumTipoTransportadorCentroCarregamento.Nenhum), options: EnumTipoTransportadorCentroCarregamento.obterOpcoesSecundario(), text: Localization.Resources.Logistica.CentroCarregamento.TipoDeTransportadorSecundario.getFieldDescription(), def: EnumTipoTransportadorCentroCarregamento.Nenhum, visible: ko.observable(false) });
    this.ListaCentroCarregamentoTransportador = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), idBtnSearch: guid() });
    this.ListaTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras = PropertyEntity({ type: types.local, text: Localization.Resources.Logistica.CentroCarregamento.AdicionarTipoDeCarga, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid(), idBtnSearch: guid() });
    this.TempoAguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.TempoParaAguardarConfirmacaoDoTransportadorMinutos.getFieldDescription(), maxlength: 7, visible: ko.observable(false) });
    this.TempoAguardarAprovacaoTransportador = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.TempoAguardarAprovacaoTransportador.getFieldDescription(), maxlength: 7, visible: ko.observable(false) });
    this.TempoAguardarInteresseTransportadorParaCargaLiberadaAutomaticamente = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.TempoParaAguardarInteresseDoTransportadorMinutos.getFieldDescription(), maxlength: 7, visible: ko.observable(false) });

    this.ListaCentroCarregamentoTransportador.val.subscribe(function () {
        recarregarGridCentroCarregamentoTransportador();
    });

    this.TipoTransportadorCentroCarregamento.val.subscribe(function (tipoSelecionado) {
        var tipoPorPrioridadeDeRota = tipoSelecionado == EnumTipoTransportadorCentroCarregamento.PorPrioridadeDeRota;

        self.TempoAguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente.visible(tipoPorPrioridadeDeRota);
        self.TempoAguardarAprovacaoTransportador.visible(tipoPorPrioridadeDeRota);
        self.LiberarCargaAutomaticamenteParaTransportadorasForaRota.visible(tipoPorPrioridadeDeRota);
        self.AguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente.visible(tipoPorPrioridadeDeRota);
        self.TipoTransportadorSecundarioCentroCarregamento.visible(tipoPorPrioridadeDeRota);

        if (!tipoPorPrioridadeDeRota) {
            self.TempoAguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente.val("");
            self.TempoAguardarAprovacaoTransportador.val("");
            self.LiberarCargaAutomaticamenteParaTransportadorasForaRota.val(false);
            self.AguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente.val(false);
            self.TipoTransportadorSecundarioCentroCarregamento.val(EnumTipoTransportadorCentroCarregamento.Nenhum);
        }
    });

    this.LiberarCargaManualmenteParaTransportadores.val.subscribe(function (valor) {
        if (valor) {
            self.LiberarCargaAutomaticamenteParaTransportadoras.val(false);
            self.LiberarCargaAutomaticamenteParaTransportadoras.visible(false);
        }
        else
            self.LiberarCargaAutomaticamenteParaTransportadoras.visible(true);
    });

    this.LiberarCargaAutomaticamenteParaTransportadoras.val.subscribe(function (valor) {
        if (valor) {
            self.LiberarCargaManualmenteParaTransportadores.val(false);
            self.LiberarCargaManualmenteParaTransportadores.visible(false);
        }
        else
            self.LiberarCargaManualmenteParaTransportadores.visible(true);
    });

    this.TipoTransportadorSecundarioCentroCarregamento.val.subscribe(function (tipoSelecionado) {
        let nenhumTipoSelecionado = tipoSelecionado == EnumTipoTransportadorCentroCarregamento.Nenhum;

        self.LiberarCargaAutomaticamenteParaTransportadorasForaRota.visible(nenhumTipoSelecionado);
        self.TempoAguardarInteresseTransportadorParaCargaLiberadaAutomaticamente.visible(!nenhumTipoSelecionado);

        if (!nenhumTipoSelecionado) {
            self.LiberarCargaAutomaticamenteParaTransportadorasForaRota.val(false);
        }
        else {
            self.TempoAguardarInteresseTransportadorParaCargaLiberadaAutomaticamente.val("");
        }
    });

    this.LiberarCargaAutomaticamenteParaTransportadorasForaRota.val.subscribe(function (novoValor) {
        if (novoValor) {
            self.TipoTransportadorSecundarioCentroCarregamento.visible(false);
            self.TipoTransportadorSecundarioCentroCarregamento.val(EnumTipoTransportadorCentroCarregamento.Nenhum);
        }
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarCentroCarregamentoTransportadorModalClick, type: types.event, text: Localization.Resources.Logistica.CentroCarregamento.AdicionarTransportador });
}

var CentroCarregamentoTransportadorCadastro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: Localization.Resources.Logistica.CentroCarregamento.Transportador.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportadores = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), required: ko.observable(false), text: Localization.Resources.Logistica.CentroCarregamento.Transportadores.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.ListaClienteDestino = PropertyEntity({ type: types.local, text: Localization.Resources.Logistica.CentroCarregamento.AdicionarPessoa, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid(), idBtnSearch: guid() });
    this.ListaLocalidadeDestino = PropertyEntity({ type: types.local, text: Localization.Resources.Logistica.CentroCarregamento.AdicionarLocalidade, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid(), idBtnSearch: guid() });
    this.ListaTipoCarga = PropertyEntity({ type: types.local, text: Localization.Resources.Logistica.CentroCarregamento.AdicionarTipoDeCarga, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid(), idBtnSearch: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadCentroCarregamentoTransportador() {
    _centroCarregamentoTransportador = new CentroCarregamentoTransportador();
    KoBindings(_centroCarregamentoTransportador, "knockoutCentroCarregamentoTransportador");

    _centroCarregamentoTransportadorCadastro = new CentroCarregamentoTransportadorCadastro();
    KoBindings(_centroCarregamentoTransportadorCadastro, "knockoutCentroCarregamentoTransportadorCadastro");

    _crudCentroCarregamentoTransportadorCadastro = new CRUDCentroCarregamentoTransportadorCadastro();
    KoBindings(_crudCentroCarregamentoTransportadorCadastro, "knockoutCRUDCentroCarregamentoTransportadorCadastro");

    loadGridCentroCarregamentoTransportador();
    loadGridCentroCarregamentoTransportadorClientesDestino();
    loadGridCentroCarregamentoTransportadorTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras();
    loadGridCentroCarregamentoTransportadorLocalidadesDestino();
    loadGridCentroCarregamentoTransportadorTipoCarga();

    new BuscarTransportadores(_centroCarregamentoTransportadorCadastro.Transportador);
    new BuscarTransportadores(_centroCarregamentoTransportadorCadastro.Transportadores,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        500
    );
}

function loadGridCentroCarregamentoTransportador() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarCentroCarregamentoTransportadorClick }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoTransportador", visible: false },
        { data: "DescricaoTransportador", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }
    ];

    _gridCentroCarregamentoTransportador = new BasicDataTable(_centroCarregamentoTransportador.ListaCentroCarregamentoTransportador.idGrid, header, menuOpcoes);
    _gridCentroCarregamentoTransportador.CarregarGrid([]);
}

function loadGridCentroCarregamentoTransportadorClientesDestino() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirClienteDestinoClick }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }
    ];

    _gridCentroCarregamentoTransportadorClientesDestino = new BasicDataTable(_centroCarregamentoTransportadorCadastro.ListaClienteDestino.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_centroCarregamentoTransportadorCadastro.ListaClienteDestino, undefined, undefined, undefined, undefined, _gridCentroCarregamentoTransportadorClientesDestino);
    _centroCarregamentoTransportadorCadastro.ListaClienteDestino.basicTable = _gridCentroCarregamentoTransportadorClientesDestino;

    _gridCentroCarregamentoTransportadorClientesDestino.CarregarGrid([]);
}

function loadGridCentroCarregamentoTransportadorTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirTipoCargaBloquearLiberacaoAutomaticaParaTransportadorasClick }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }
    ];

    _gridCentroCarregamentoTransportadorTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras = new BasicDataTable(_centroCarregamentoTransportador.ListaTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposdeCarga(_centroCarregamentoTransportador.ListaTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras, undefined, undefined, _gridCentroCarregamentoTransportadorTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras);
    _centroCarregamentoTransportador.ListaTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras.basicTable = _gridCentroCarregamentoTransportadorTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras;

    _gridCentroCarregamentoTransportadorTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras.CarregarGrid([]);
}

function loadGridCentroCarregamentoTransportadorLocalidadesDestino() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirLocalidadeDestinoClick }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "60%" },
        { data: "Estado", title: Localization.Resources.Gerais.Geral.Estado, width: "20%" }
    ];

    _gridCentroCarregamentoTransportadorLocalidadesDestino = new BasicDataTable(_centroCarregamentoTransportadorCadastro.ListaLocalidadeDestino.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarLocalidades(_centroCarregamentoTransportadorCadastro.ListaLocalidadeDestino, undefined, undefined, undefined, _gridCentroCarregamentoTransportadorLocalidadesDestino);
    _centroCarregamentoTransportadorCadastro.ListaLocalidadeDestino.basicTable = _gridCentroCarregamentoTransportadorLocalidadesDestino;

    _gridCentroCarregamentoTransportadorLocalidadesDestino.CarregarGrid([]);
}

function loadGridCentroCarregamentoTransportadorTipoCarga() {
    let menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirTransportadorTipoCargaClick }] };
    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }
    ];

    _gridCentroCarregamentoTransportadorTipoCarga = new BasicDataTable(_centroCarregamentoTransportadorCadastro.ListaTipoCarga.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    BuscarTiposdeCarga(_centroCarregamentoTransportadorCadastro.ListaTipoCarga, null, null, _gridCentroCarregamentoTransportadorTipoCarga);
    _centroCarregamentoTransportadorCadastro.ListaTipoCarga.basicTable = _gridCentroCarregamentoTransportadorTipoCarga;

    _gridCentroCarregamentoTransportadorTipoCarga.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarCentroCarregamentoTransportadorClick() {

    if (!ValidarCamposObrigatorios(_centroCarregamentoTransportadorCadastro)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return false;
    }

    adicionarCentroCarregamentoTransportador();

    recarregarGridCentroCarregamentoTransportador();
}

function adicionarCentroCarregamentoTransportadorModalClick() {
    _centroCarregamentoTransportadorCadastro.Transportadores.required(true);
    _centroCarregamentoTransportadorCadastro.Transportadores.visible(true);

    _centroCarregamentoTransportadorCadastro.Transportador.required(false);
    _centroCarregamentoTransportadorCadastro.Transportador.visible(false);

    controlarBotoesCentroCarregamentoTransportadorCadastroHabilitados(false);
    exibirModalCadastroCentroCarregamentoTransportador();
}

function atualizarCentroCarregamentoTransportadorClick() {
    const transportador = obterUnicoTransportador();
    if (validarCentroCarregamentoTransportador(transportador)) {
        const listaCentroCarregamentoTransportador = obterListaCentroCarregamentoTransportador();

        for (let i = 0; i < listaCentroCarregamentoTransportador.length; i++) {
            if (_centroCarregamentoTransportadorCadastro.Codigo.val() == listaCentroCarregamentoTransportador[i].Codigo) {
                listaCentroCarregamentoTransportador.splice(i, 1, obterCentroCarregamentoTransportadorSalvar(transportador));
                break;
            }
        }

        _centroCarregamentoTransportador.ListaCentroCarregamentoTransportador.val(listaCentroCarregamentoTransportador)

        fecharModalCadastroCentroCarregamentoTransportador();
    }
}

function editarCentroCarregamentoTransportadorClick(registroSelecionado) {
    _centroCarregamentoTransportadorCadastro.Transportadores.required(false);
    _centroCarregamentoTransportadorCadastro.Transportadores.visible(false);

    _centroCarregamentoTransportadorCadastro.Transportador.required(true);
    _centroCarregamentoTransportadorCadastro.Transportador.visible(true);

    var centroCarregamentoTransportador = obterCentroCarregamentoTransportadorPorCodigo(registroSelecionado.Codigo);

    if (centroCarregamentoTransportador) {
        _centroCarregamentoTransportadorCadastro.Codigo.val(centroCarregamentoTransportador.Codigo);
        _centroCarregamentoTransportadorCadastro.Transportador.codEntity(centroCarregamentoTransportador.Transportador.Codigo);
        _centroCarregamentoTransportadorCadastro.Transportador.entityDescription(centroCarregamentoTransportador.Transportador.Descricao);
        _centroCarregamentoTransportadorCadastro.Transportador.val(centroCarregamentoTransportador.Transportador.Descricao);
        _gridCentroCarregamentoTransportadorClientesDestino.CarregarGrid(centroCarregamentoTransportador.ClientesDestino);
        _gridCentroCarregamentoTransportadorLocalidadesDestino.CarregarGrid(centroCarregamentoTransportador.LocalidadesDestino);
        _gridCentroCarregamentoTransportadorTipoCarga.CarregarGrid(centroCarregamentoTransportador.TiposCarga);

        controlarBotoesCentroCarregamentoTransportadorCadastroHabilitados(true);
        exibirModalCadastroCentroCarregamentoTransportador();
    }
}

function excluirCentroCarregamentoTransportadorClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Logistica.CentroCarregamento.RealmenteDesejaExcluirTransportador, function () {
        var listaCentroCarregamentoTransportador = obterListaCentroCarregamentoTransportador();

        for (var i = 0; i < listaCentroCarregamentoTransportador.length; i++) {
            if (_centroCarregamentoTransportadorCadastro.Codigo.val() == listaCentroCarregamentoTransportador[i].Codigo)
                listaCentroCarregamentoTransportador.splice(i, 1);
        }

        _centroCarregamentoTransportador.ListaCentroCarregamentoTransportador.val(listaCentroCarregamentoTransportador);

        fecharModalCadastroCentroCarregamentoTransportador();
    });
}

function excluirClienteDestinoClick(registroSelecionado) {
    var listaClienteDestino = obterListaClienteDestino();

    for (var i = 0; i < listaClienteDestino.length; i++) {
        if (registroSelecionado.Codigo == listaClienteDestino[i].Codigo)
            listaClienteDestino.splice(i, 1);
    }

    _gridCentroCarregamentoTransportadorClientesDestino.CarregarGrid(listaClienteDestino);
}

function excluirTipoCargaBloquearLiberacaoAutomaticaParaTransportadorasClick(registroSelecionado) {
    var listaTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras = obterListaTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras();

    for (var i = 0; i < listaTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras.length; i++) {
        if (registroSelecionado.Codigo == listaTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras[i].Codigo)
            listaTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras.splice(i, 1);
    }

    _gridCentroCarregamentoTransportadorTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras.CarregarGrid(listaTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras);
}

function excluirLocalidadeDestinoClick(registroSelecionado) {
    var listaLocalidadeDestino = obterListaLocalidadeDestino();

    for (var i = 0; i < listaLocalidadeDestino.length; i++) {
        if (registroSelecionado.Codigo == listaLocalidadeDestino[i].Codigo)
            listaLocalidadeDestino.splice(i, 1);
    }

    _gridCentroCarregamentoTransportadorLocalidadesDestino.CarregarGrid(listaLocalidadeDestino);
}

function excluirTransportadorTipoCargaClick(registroSelecionado) {
    let listaTipoCarga = obterListaTransportadorTipoCarga();

    for (let i = 0; i < listaTipoCarga.length; i++) {
        if (registroSelecionado.Codigo == listaTipoCarga[i].Codigo)
            listaTipoCarga.splice(i, 1);
    }

    _gridCentroCarregamentoTransportadorTipoCarga.CarregarGrid(listaTipoCarga);
}

/*
 * Declaração das Funções Públicas
 */

function preencherCentroCarregamentoTransportador(dadosTransportador) {
    PreencherObjetoKnout(_centroCarregamentoTransportador, { Data: dadosTransportador });

    _centroCarregamentoTransportador.ListaCentroCarregamentoTransportador.val(dadosTransportador.ListaCentroCarregamentoTransportador);
    _gridCentroCarregamentoTransportadorTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras.CarregarGrid(dadosTransportador.TiposCargaBloquearLiberacaoAutomaticaParaTransportadoras);
}

function preencherCentroCarregamentoTransportadorSalvar(centroCarregamento) {
    centroCarregamento["PermitirMatrizSelecionarFilial"] = _centroCarregamentoTransportador.PermitirMatrizSelecionarFilial.val();
    centroCarregamento["LiberarCargaManualmenteParaTransportadores"] = _centroCarregamentoTransportador.LiberarCargaManualmenteParaTransportadores.val();
    centroCarregamento["LiberarCargaAutomaticamenteParaTransportadoras"] = _centroCarregamentoTransportador.LiberarCargaAutomaticamenteParaTransportadoras.val();
    centroCarregamento["LiberarCargaAutomaticamenteParaTransportadorasForaRota"] = _centroCarregamentoTransportador.LiberarCargaAutomaticamenteParaTransportadorasForaRota.val();
    centroCarregamento["AguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente"] = _centroCarregamentoTransportador.AguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente.val();
    centroCarregamento["PermitirLiberarCargaTransportadorExclusivo"] = _centroCarregamentoTransportador.PermitirLiberarCargaTransportadorExclusivo.val();
    centroCarregamento["TempoAguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente"] = _centroCarregamentoTransportador.TempoAguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente.val();
    centroCarregamento["TempoAguardarAprovacaoTransportador"] = _centroCarregamentoTransportador.TempoAguardarAprovacaoTransportador.val();
    centroCarregamento["TempoAguardarInteresseTransportadorParaCargaLiberadaAutomaticamente"] = _centroCarregamentoTransportador.TempoAguardarInteresseTransportadorParaCargaLiberadaAutomaticamente.val();
    centroCarregamento["TipoTransportadorCentroCarregamento"] = _centroCarregamentoTransportador.TipoTransportadorCentroCarregamento.val();
    centroCarregamento["TipoTransportadorSecundarioCentroCarregamento"] = _centroCarregamentoTransportador.TipoTransportadorSecundarioCentroCarregamento.val();
    centroCarregamento["ListaCentroCarregamentoTransportador"] = obterListaCentroCarregamentoTransportadorSalvar();
    centroCarregamento["ListaTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras"] = obterListaTipoCargaBloquearLiberacaoAutomaticaParaTransportadorasSalvar();
}

function limparCamposCentroCarregamentoTransportador() {
    LimparCampos(_centroCarregamentoTransportador);

    _centroCarregamentoTransportador.ListaCentroCarregamentoTransportador.val([]);
    _gridCentroCarregamentoTransportadorTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras.CarregarGrid([]);
}

/*
 * Declaração das Funções
 */

function controlarBotoesCentroCarregamentoTransportadorCadastroHabilitados(isEdicao) {
    _crudCentroCarregamentoTransportadorCadastro.Atualizar.visible(isEdicao);
    _crudCentroCarregamentoTransportadorCadastro.Excluir.visible(isEdicao);
    _crudCentroCarregamentoTransportadorCadastro.Adicionar.visible(!isEdicao);
}

function exibirModalCadastroCentroCarregamentoTransportador() {
    Global.abrirModal('divModalCadastroCentroCarregamentoTransportador');
    $("#divModalCadastroCentroCarregamentoTransportador").one('hidden.bs.modal', function () {
        limparCamposCentroCarregamentoTransportadorCadastro();
    });
}

function fecharModalCadastroCentroCarregamentoTransportador() {
    Global.fecharModal('divModalCadastroCentroCarregamentoTransportador');
}

function limparCamposCentroCarregamentoTransportadorCadastro() {
    LimparCampos(_centroCarregamentoTransportadorCadastro);

    _gridCentroCarregamentoTransportadorClientesDestino.CarregarGrid([]);
    _gridCentroCarregamentoTransportadorLocalidadesDestino.CarregarGrid([]);
    _gridCentroCarregamentoTransportadorTipoCarga.CarregarGrid([]);
}

function obterCentroCarregamentoTransportadorPorCodigo(codigo) {
    var listaCentroCarregamentoTransportador = obterListaCentroCarregamentoTransportador();

    for (var i = 0; i < listaCentroCarregamentoTransportador.length; i++) {
        var centroCarregamentoTransportador = listaCentroCarregamentoTransportador[i];

        if (codigo == centroCarregamentoTransportador.Codigo)
            return centroCarregamentoTransportador;
    }

    return undefined;
}

function obterCentroCarregamentoTransportadorSalvar(transportador) {
    var listaClienteDestino = obterListaClienteDestino();
    var listaLocalidadeDestino = obterListaLocalidadeDestino();
    let listaTipoCarga = obterListaTransportadorTipoCarga();

    return {
        Codigo: guid(),
        Transportador: {
            Codigo: transportador.Codigo,
            Descricao: transportador.Descricao
        },
        ClientesDestino: listaClienteDestino,
        LocalidadesDestino: listaLocalidadeDestino,
        TiposCarga: listaTipoCarga,
    };
}

function obterListaCentroCarregamentoTransportador() {
    return _centroCarregamentoTransportador.ListaCentroCarregamentoTransportador.val().slice();
}

function obterListaCentroCarregamentoTransportadorSalvar() {
    var listaCentroCarregamentoTransportador = obterListaCentroCarregamentoTransportador();
    var listaCentroCarregamentoTransportadorSalvar = new Array();

    for (var i = 0; i < listaCentroCarregamentoTransportador.length; i++) {
        var centroCarregamentoTransportador = listaCentroCarregamentoTransportador[i];
        var listaClientesDestinoSalvar = new Array();
        var listaLocalidadesDestinoSalvar = new Array();
        var listaTipoCargaSalvar = new Array();

        for (var j = 0; j < centroCarregamentoTransportador.ClientesDestino.length; j++)
            listaClientesDestinoSalvar.push(centroCarregamentoTransportador.ClientesDestino[j].Codigo);

        for (var j = 0; j < centroCarregamentoTransportador.LocalidadesDestino.length; j++)
            listaLocalidadesDestinoSalvar.push(centroCarregamentoTransportador.LocalidadesDestino[j].Codigo);

        for (var j = 0; j < centroCarregamentoTransportador.TiposCarga.length; j++)
            listaTipoCargaSalvar.push(centroCarregamentoTransportador.TiposCarga[j].Codigo);

        listaCentroCarregamentoTransportadorSalvar.push({
            Codigo: centroCarregamentoTransportador.Codigo,
            Transportador: centroCarregamentoTransportador.Transportador.Codigo,
            ClientesDestino: listaClientesDestinoSalvar,
            LocalidadesDestino: listaLocalidadesDestinoSalvar,
            TiposCarga: listaTipoCargaSalvar,
        });
    }

    return JSON.stringify(listaCentroCarregamentoTransportadorSalvar);
}

function obterListaClienteDestino() {
    return _gridCentroCarregamentoTransportadorClientesDestino.BuscarRegistros();
}

function obterListaTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras() {
    return _gridCentroCarregamentoTransportadorTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras.BuscarRegistros();
}

function obterListaLocalidadeDestino() {
    return _gridCentroCarregamentoTransportadorLocalidadesDestino.BuscarRegistros();
}

function obterListaTransportadorTipoCarga() {
    return _gridCentroCarregamentoTransportadorTipoCarga.BuscarRegistros();
}

function obterListaTipoCargaBloquearLiberacaoAutomaticaParaTransportadorasSalvar() {
    var listaTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras = obterListaTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras();
    var listaTipoCargaBloquearLiberacaoAutomaticaParaTransportadorasSalvar = new Array();

    for (var i = 0; i < listaTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras.length; i++) {
        var tipoCargaBloquearLiberacaoAutomaticaParaTransportadoras = listaTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras[i];

        listaTipoCargaBloquearLiberacaoAutomaticaParaTransportadorasSalvar.push({
            Codigo: tipoCargaBloquearLiberacaoAutomaticaParaTransportadoras.Codigo
        });
    }

    return JSON.stringify(listaTipoCargaBloquearLiberacaoAutomaticaParaTransportadorasSalvar);
}

function recarregarGridCentroCarregamentoTransportador() {
    var listaCentroCarregamentoTransportador = obterListaCentroCarregamentoTransportador();
    var listaCentroCarregamentoTransportadorCarregar = new Array();

    for (var i = 0; i < listaCentroCarregamentoTransportador.length; i++) {
        var centroCarregamentoTransportador = listaCentroCarregamentoTransportador[i];

        listaCentroCarregamentoTransportadorCarregar.push({
            Codigo: centroCarregamentoTransportador.Codigo,
            CodigoTransportador: centroCarregamentoTransportador.Transportador.Codigo,
            DescricaoTransportador: centroCarregamentoTransportador.Transportador.Descricao
        });
    }

    _gridCentroCarregamentoTransportador.CarregarGrid(listaCentroCarregamentoTransportadorCarregar);
}

function validarCentroCarregamentoTransportador(transportador) {
    const listaCentroCarregamentoTransportador = obterListaCentroCarregamentoTransportador();

    for (let i = 0; i < listaCentroCarregamentoTransportador.length; i++) {
        const centroCarregamentoTransportador = listaCentroCarregamentoTransportador[i];

        if (centroCarregamentoTransportador.Codigo != _centroCarregamentoTransportadorCadastro.Codigo.val()
            && centroCarregamentoTransportador.Transportador.Codigo == transportador.Codigo) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Logistica.CentroCarregamento.TransportadorInformadoJaEstaCadastrado);
            return false;
        }
    }

    return true;
}

function adicionarCentroCarregamentoTransportador() {
    let transportadores = obterListaTransportadoresMultiplesEntities();

    transportadores.forEach((transportador) => {
        if (validarCentroCarregamentoTransportador(transportador)) {

            _centroCarregamentoTransportador.ListaCentroCarregamentoTransportador.val().push(obterCentroCarregamentoTransportadorSalvar(transportador));

            fecharModalCadastroCentroCarregamentoTransportador();
        }
    });
}

function obterListaTransportadoresMultiplesEntities() {
    return _centroCarregamentoTransportadorCadastro.Transportadores.multiplesEntities();
}

function obterUnicoTransportador() {
    return {
        Codigo: _centroCarregamentoTransportadorCadastro.Transportador.codEntity(),
        Descricao: _centroCarregamentoTransportadorCadastro.Transportador.val(),
    }
}
