/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />

// #region Objetos Globais do Arquivo

var _cadastroValorFreteMinimo;
var _crudCadastroValorFreteMinimo;
var _valorFreteMinimo;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CadastroValorFreteMinimo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ClientesDestino = PropertyEntity({ type: types.event, text: "Adicionar Cliente", idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(true) });
    this.ClientesOrigem = PropertyEntity({ type: types.event, text: "Adicionar Cliente", idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(true) });
    this.EstadosDestino = PropertyEntity({ type: types.event, text: "Adicionar Estado", idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(true) });
    this.EstadosOrigem = PropertyEntity({ type: types.event, text: "Adicionar Estado", idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(true) });
    this.LocalidadesDestino = PropertyEntity({ type: types.event, text: "Adicionar Destino", idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(true) });
    this.LocalidadesOrigem = PropertyEntity({ type: types.event, text: "Adicionar Origem", idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(true) });
    this.ModelosVeicularesCarga = PropertyEntity({ type: types.event, text: "Adicionar Modelo Veicular de Carga", idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(true) });
    this.TiposCarga = PropertyEntity({ type: types.event, text: "Adicionar Tipo de Carga", idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(true) });
    this.ValorMinimo = PropertyEntity({ text: "*Valor Mínimo:", getType: typesKnockout.decimal, required: true , enable: ko.observable(true)});
};

var CRUDCadastroValorFreteMinimo = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarValorFreteMinimoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarValorFreteMinimoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirValorFreteMinimoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var ValorFreteMinimo = function () {
    this.ValorFreteMinimo = PropertyEntity({ text: "Valor de Frete Mínimo:", val: ko.observable(""), def: "", getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.ListaValorFreteMinimo = PropertyEntity({ getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), idBtnSearch: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarValorFreteMinimoModalClick, type: types.event, text: "Adicionar Valor de Frete Mínimo", visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridValorFreteMinimo() {
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarValorFreteMinimoClick }
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoEditar] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Origens", title: "Origens", width: "20%" },
        { data: "Destinos", title: "Destinos", width: "20%" },
        { data: "TiposCarga", title: "Tipos de Carga", width: "20%" },
        { data: "ModelosVeicularesCarga", title: "Modelos Veiculares de Carga", width: "20%" },
        { data: "ValorMinimo", title: "Valor Mínimo", width: "10%", className: "text-align-right" }
    ];

    _valorFreteMinimo.ListaValorFreteMinimo.basicTable = new BasicDataTable(_valorFreteMinimo.ListaValorFreteMinimo.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _valorFreteMinimo.ListaValorFreteMinimo.basicTable.CarregarGrid(new Array());
}

function loadGridValorFreteMinimoDestino(knoutDestino) {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: function (registroSelecionado) { excluirDestinoClick(knoutDestino, registroSelecionado); } }
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    knoutDestino.basicTable = new BasicDataTable(knoutDestino.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    knoutDestino.basicTable.CarregarGrid(new Array());
}

function loadGridValorFreteMinimoModeloVeicularCarga() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirModeloVeicularCargaClick }
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _cadastroValorFreteMinimo.ModelosVeicularesCarga.basicTable = new BasicDataTable(_cadastroValorFreteMinimo.ModelosVeicularesCarga.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _cadastroValorFreteMinimo.ModelosVeicularesCarga.basicTable.CarregarGrid(new Array());
}

function loadGridValorFreteMinimoOrigem(knoutOrigem) {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: function (registroSelecionado) { excluirOrigemClick(knoutOrigem, registroSelecionado); } }
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    knoutOrigem.basicTable = new BasicDataTable(knoutOrigem.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    knoutOrigem.basicTable.CarregarGrid(new Array());
}

function loadGridValorFreteMinimoTipoCarga() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirTipoCargaClick }
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _cadastroValorFreteMinimo.TiposCarga.basicTable = new BasicDataTable(_cadastroValorFreteMinimo.TiposCarga.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _cadastroValorFreteMinimo.TiposCarga.basicTable.CarregarGrid(new Array());
}

function loadValorFreteMinimo() {
    _valorFreteMinimo = new ValorFreteMinimo();
    KoBindings(_valorFreteMinimo, "knockoutValorFreteMinimo");

    _cadastroValorFreteMinimo = new CadastroValorFreteMinimo();
    KoBindings(_cadastroValorFreteMinimo, "knockoutCadastroValorFreteMinimo");

    _crudCadastroValorFreteMinimo = new CRUDCadastroValorFreteMinimo();
    KoBindings(_crudCadastroValorFreteMinimo, "knockoutCRUDCadastroValorFreteMinimo");

    loadGridValorFreteMinimo();
    loadValorFreteMinimoOrigens();
    loadValorFreteMinimoDestinos();
    loadValorFreteMinimoTiposCarga();
    loadValorFreteMinimoModelosVeicularesCarga();
}

function loadValorFreteMinimoDestinos() {
    loadGridValorFreteMinimoDestino(_cadastroValorFreteMinimo.LocalidadesDestino);
    loadGridValorFreteMinimoDestino(_cadastroValorFreteMinimo.ClientesDestino);
    loadGridValorFreteMinimoDestino(_cadastroValorFreteMinimo.EstadosDestino);

    new BuscarLocalidades(_cadastroValorFreteMinimo.LocalidadesDestino, null, null, null, _cadastroValorFreteMinimo.LocalidadesDestino.basicTable, controlarExibicaoAbasDestino);
    new BuscarClientes(_cadastroValorFreteMinimo.ClientesDestino, null, false, null, null, _cadastroValorFreteMinimo.ClientesDestino.basicTable, null, null, null, null, controlarExibicaoAbasDestino);
    new BuscarEstados(_cadastroValorFreteMinimo.EstadosDestino, null, _cadastroValorFreteMinimo.EstadosDestino.basicTable, controlarExibicaoAbasDestino);
}

function loadValorFreteMinimoModelosVeicularesCarga() {
    loadGridValorFreteMinimoModeloVeicularCarga();

    new BuscarModelosVeicularesCarga(_cadastroValorFreteMinimo.ModelosVeicularesCarga, null, null, null, null, null, _cadastroValorFreteMinimo.ModelosVeicularesCarga.basicTable);
}

function loadValorFreteMinimoOrigens() {
    loadGridValorFreteMinimoOrigem(_cadastroValorFreteMinimo.LocalidadesOrigem);
    loadGridValorFreteMinimoOrigem(_cadastroValorFreteMinimo.ClientesOrigem);
    loadGridValorFreteMinimoOrigem(_cadastroValorFreteMinimo.EstadosOrigem);

    new BuscarLocalidades(_cadastroValorFreteMinimo.LocalidadesOrigem, null, null, null, _cadastroValorFreteMinimo.LocalidadesOrigem.basicTable, controlarExibicaoAbasOrigem);
    new BuscarClientes(_cadastroValorFreteMinimo.ClientesOrigem, null, false, null, null, _cadastroValorFreteMinimo.ClientesOrigem.basicTable, null, null, null, null, controlarExibicaoAbasOrigem);
    new BuscarEstados(_cadastroValorFreteMinimo.EstadosOrigem, null, _cadastroValorFreteMinimo.EstadosOrigem.basicTable, controlarExibicaoAbasOrigem);
}

function loadValorFreteMinimoTiposCarga() {
    loadGridValorFreteMinimoTipoCarga();

    new BuscarTiposdeCarga(_cadastroValorFreteMinimo.TiposCarga, null, null, _cadastroValorFreteMinimo.TiposCarga.basicTable);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarValorFreteMinimoClick() {
    if (!validarValorFreteMinimo())
        return;

    _valorFreteMinimo.ListaValorFreteMinimo.val().push(obterValorFreteMinimoSalvar());

    recarregarGridValorFreteMinimo();
    fecharModalCadastroValorFreteMinimo();
}

function adicionarValorFreteMinimoModalClick() {
    _cadastroValorFreteMinimo.Codigo.val(guid());

    controlarBotoesCadastroValorFreteMinimoHabilitados(false);
    exibirModalCadastroValorFreteMinimo();
}

function atualizarValorFreteMinimoClick() {
    if (!validarValorFreteMinimo())
        return;

    var listaValorFreteMinimo = _valorFreteMinimo.ListaValorFreteMinimo.val().slice();

    for (var i = 0; i < listaValorFreteMinimo.length; i++) {
        if (_cadastroValorFreteMinimo.Codigo.val() == listaValorFreteMinimo[i].Codigo) {
            listaValorFreteMinimo.splice(i, 1, obterValorFreteMinimoSalvar());
            break;
        }
    }

    _valorFreteMinimo.ListaValorFreteMinimo.val(listaValorFreteMinimo)

    recarregarGridValorFreteMinimo();
    fecharModalCadastroValorFreteMinimo();
}

function editarValorFreteMinimoClick(registroSelecionado) {
    var valorFreteMinimo = obterValorFreteMinimoPorCodigo(registroSelecionado.Codigo);

    if (!valorFreteMinimo)
        return;

    PreencherObjetoKnout(_cadastroValorFreteMinimo, { Data: valorFreteMinimo });

    _cadastroValorFreteMinimo.ClientesDestino.basicTable.CarregarGrid(valorFreteMinimo.ClientesDestino, !_CAMPOS_BLOQUEADOS);
    _cadastroValorFreteMinimo.ClientesOrigem.basicTable.CarregarGrid(valorFreteMinimo.ClientesOrigem, !_CAMPOS_BLOQUEADOS);
    _cadastroValorFreteMinimo.EstadosDestino.basicTable.CarregarGrid(valorFreteMinimo.EstadosDestino, !_CAMPOS_BLOQUEADOS);
    _cadastroValorFreteMinimo.EstadosOrigem.basicTable.CarregarGrid(valorFreteMinimo.EstadosOrigem, !_CAMPOS_BLOQUEADOS);
    _cadastroValorFreteMinimo.LocalidadesDestino.basicTable.CarregarGrid(valorFreteMinimo.LocalidadesDestino, !_CAMPOS_BLOQUEADOS);
    _cadastroValorFreteMinimo.LocalidadesOrigem.basicTable.CarregarGrid(valorFreteMinimo.LocalidadesOrigem, !_CAMPOS_BLOQUEADOS);
    _cadastroValorFreteMinimo.ModelosVeicularesCarga.basicTable.CarregarGrid(valorFreteMinimo.ModelosVeicularesCarga, !_CAMPOS_BLOQUEADOS);
    _cadastroValorFreteMinimo.TiposCarga.basicTable.CarregarGrid(valorFreteMinimo.TiposCarga, !_CAMPOS_BLOQUEADOS);

    controlarBotoesCadastroValorFreteMinimoHabilitados(true);
    controlarExibicaoAbasDestino();
    controlarExibicaoAbasOrigem();
    exibirModalCadastroValorFreteMinimo();
}

function excluirDestinoClick(knoutDestino, destinoSelecionado) {
    var destinos = knoutDestino.basicTable.BuscarRegistros().slice();

    for (var i = 0; i < destinos.length; i++) {
        if (destinoSelecionado.Codigo == destinos[i].Codigo) {
            destinos.splice(i, 1);
            break;
        }
    }

    knoutDestino.basicTable.CarregarGrid(destinos);
    controlarExibicaoAbasDestino();
}

function excluirModeloVeicularCargaClick(modeloVeicularCargaSelecionado) {
    var modelosVeicularesCarga = _cadastroValorFreteMinimo.ModelosVeicularesCarga.basicTable.BuscarRegistros().slice();

    for (var i = 0; i < modelosVeicularesCarga.length; i++) {
        if (modeloVeicularCargaSelecionado.Codigo == modelosVeicularesCarga[i].Codigo) {
            modelosVeicularesCarga.splice(i, 1);
            break;
        }
    }

    _cadastroValorFreteMinimo.ModelosVeicularesCarga.basicTable.CarregarGrid(modelosVeicularesCarga);
}

function excluirOrigemClick(knoutOrigem, origemSelecionada) {
    var origens = knoutOrigem.basicTable.BuscarRegistros().slice();

    for (var i = 0; i < origens.length; i++) {
        if (origemSelecionada.Codigo == origens[i].Codigo) {
            origens.splice(i, 1);
            break;
        }
    }

    knoutOrigem.basicTable.CarregarGrid(origens);
    controlarExibicaoAbasOrigem();
}

function excluirTipoCargaClick(tipoCargaSelecionado) {
    var tiposCarga = _cadastroValorFreteMinimo.TiposCarga.basicTable.BuscarRegistros().slice();

    for (var i = 0; i < tiposCarga.length; i++) {
        if (tipoCargaSelecionado.Codigo == tiposCarga[i].Codigo) {
            tiposCarga.splice(i, 1);
            break;
        }
    }

    _cadastroValorFreteMinimo.TiposCarga.basicTable.CarregarGrid(tiposCarga);
}

function excluirValorFreteMinimoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o valor de frete mínimo?", function () {
        var listaValorFreteMinimo = _valorFreteMinimo.ListaValorFreteMinimo.val().slice();

        for (var i = 0; i < listaValorFreteMinimo.length; i++) {
            if (_cadastroValorFreteMinimo.Codigo.val() == listaValorFreteMinimo[i].Codigo)
                listaValorFreteMinimo.splice(i, 1);
        }

        _valorFreteMinimo.ListaValorFreteMinimo.val(listaValorFreteMinimo);

        recarregarGridValorFreteMinimo();
        fecharModalCadastroValorFreteMinimo();
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function controlarEdicaoValorFreteMinimo() {
    controlarCamposHabilitadosPorKnockout(_cadastroValorFreteMinimo, !_CAMPOS_BLOQUEADOS);
    controlarCamposHabilitadosPorKnockout(_valorFreteMinimo, !_CAMPOS_BLOQUEADOS);
    controlarBotoesVisiveisPorKnockout(_cadastroValorFreteMinimo, !_CAMPOS_BLOQUEADOS);
    controlarBotoesVisiveisPorKnockout(_valorFreteMinimo, !_CAMPOS_BLOQUEADOS);
}

function limparCamposValorFreteMinimo() {
    LimparCampos(_valorFreteMinimo);

    recarregarGridValorFreteMinimo();
}

function preencherValorFreteMinimo(valorFreteMinimo) {
    PreencherObjetoKnout(_valorFreteMinimo, { Data: valorFreteMinimo });

    recarregarGridValorFreteMinimo();
}

function preencherValorFreteMinimoSalvar(contratoFreteTransportador) {
    contratoFreteTransportador["ListaValorFreteMinimo"] = JSON.stringify(_valorFreteMinimo.ListaValorFreteMinimo.val().slice());
    contratoFreteTransportador["ValorFreteMinimo"] = _valorFreteMinimo.ValorFreteMinimo.val();
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarBotoesCadastroValorFreteMinimoHabilitados(isEdicao) {
    _crudCadastroValorFreteMinimo.Atualizar.visible(isEdicao && !_CAMPOS_BLOQUEADOS);
    _crudCadastroValorFreteMinimo.Excluir.visible(isEdicao && !_CAMPOS_BLOQUEADOS);
    _crudCadastroValorFreteMinimo.Adicionar.visible(!isEdicao && !_CAMPOS_BLOQUEADOS);
}

function controlarExibicaoAbasDestino() {
    if (_cadastroValorFreteMinimo.ClientesDestino.basicTable.BuscarRegistros().length > 0) {
        $("#liTabLocalidadesDestino").hide();
        $("#liTabClientesDestino").show();
        $("#liTabEstadosDestino").hide();
        $(".nav-tabs a[href='#tabClientesDestino']").tab('show');
    }
    else if (_cadastroValorFreteMinimo.LocalidadesDestino.basicTable.BuscarRegistros().length > 0) {
        $("#liTabLocalidadesDestino").show();
        $("#liTabClientesDestino").hide();
        $("#liTabEstadosDestino").hide();
        $(".nav-tabs a[href='#tabLocalidadesDestino']").tab('show');
    }
    else if (_cadastroValorFreteMinimo.EstadosDestino.basicTable.BuscarRegistros().length > 0) {
        $("#liTabLocalidadesDestino").hide();
        $("#liTabClientesDestino").hide();
        $("#liTabEstadosDestino").show();
        $(".nav-tabs a[href='#tabEstadosDestino']").tab('show');
    }
    else {
        $("#liTabLocalidadesDestino").show();
        $("#liTabClientesDestino").show();
        $("#liTabEstadosDestino").show();
        $(".nav-tabs a[href='#tabLocalidadesDestino']").tab('show');
    }
}

function controlarExibicaoAbasOrigem() {
    if (_cadastroValorFreteMinimo.ClientesOrigem.basicTable.BuscarRegistros().length > 0) {
        $("#liTabLocalidadesOrigem").hide();
        $("#liTabClientesOrigem").show();
        $("#liTabEstadosOrigem").hide();
        $(".nav-tabs a[href='#tabClientesOrigem']").tab('show');
    }
    else if (_cadastroValorFreteMinimo.LocalidadesOrigem.basicTable.BuscarRegistros().length > 0) {
        $("#liTabLocalidadesOrigem").show();
        $("#liTabClientesOrigem").hide();
        $("#liTabEstadosOrigem").hide();
        $(".nav-tabs a[href='#tabLocalidadesOrigem']").tab('show');
    }
    else if (_cadastroValorFreteMinimo.EstadosOrigem.basicTable.BuscarRegistros().length > 0) {
        $("#liTabLocalidadesOrigem").hide();
        $("#liTabClientesOrigem").hide();
        $("#liTabEstadosOrigem").show();
        $(".nav-tabs a[href='#tabEstadosOrigem']").tab('show');
    }
    else {
        $("#liTabLocalidadesOrigem").show();
        $("#liTabClientesOrigem").show();
        $("#liTabEstadosOrigem").show();
        $(".nav-tabs a[href='#tabLocalidadesOrigem']").tab('show');
    }
}

function exibirModalCadastroValorFreteMinimo() {
    Global.abrirModal('divModalCadastroValorFreteMinimo');
    $("#divModalCadastroValorFreteMinimo").one('hidden.bs.modal', function () {
        limparCamposCadastroValorFreteMinimo();
    });
}

function fecharModalCadastroValorFreteMinimo() {
    Global.fecharModal('divModalCadastroValorFreteMinimo');
}

function limparCamposCadastroValorFreteMinimo() {
    LimparCampos(_cadastroValorFreteMinimo);

    _cadastroValorFreteMinimo.ClientesDestino.basicTable.CarregarGrid(new Array());
    _cadastroValorFreteMinimo.ClientesOrigem.basicTable.CarregarGrid(new Array());
    _cadastroValorFreteMinimo.EstadosDestino.basicTable.CarregarGrid(new Array());
    _cadastroValorFreteMinimo.EstadosOrigem.basicTable.CarregarGrid(new Array());
    _cadastroValorFreteMinimo.LocalidadesDestino.basicTable.CarregarGrid(new Array());
    _cadastroValorFreteMinimo.LocalidadesOrigem.basicTable.CarregarGrid(new Array());
    _cadastroValorFreteMinimo.ModelosVeicularesCarga.basicTable.CarregarGrid(new Array());
    _cadastroValorFreteMinimo.TiposCarga.basicTable.CarregarGrid(new Array());

    controlarExibicaoAbasDestino();
    controlarExibicaoAbasOrigem();

    Global.ResetarAba("valor-frete-minimo-abas");
    Global.ResetarAba("valor-frete-minimo-abas-destino");
    Global.ResetarAba("valor-frete-minimo-abas-origem");
}

function obterValorFreteMinimoPorCodigo(codigo) {
    var listaValorFreteMinimo = _valorFreteMinimo.ListaValorFreteMinimo.val().slice();

    for (var i = 0; i < listaValorFreteMinimo.length; i++) {
        var valorFreteMinimo = listaValorFreteMinimo[i];

        if (codigo == valorFreteMinimo.Codigo)
            return valorFreteMinimo;
    }

    return undefined;
}

function obterValorFreteMinimoSalvar() {
    return {
        Codigo: _cadastroValorFreteMinimo.Codigo.val(),
        ClientesDestino: _cadastroValorFreteMinimo.ClientesDestino.basicTable.BuscarRegistros().slice(),
        ClientesOrigem: _cadastroValorFreteMinimo.ClientesOrigem.basicTable.BuscarRegistros().slice(),
        EstadosDestino: _cadastroValorFreteMinimo.EstadosDestino.basicTable.BuscarRegistros().slice(),
        EstadosOrigem: _cadastroValorFreteMinimo.EstadosOrigem.basicTable.BuscarRegistros().slice(),
        LocalidadesDestino: _cadastroValorFreteMinimo.LocalidadesDestino.basicTable.BuscarRegistros().slice(),
        LocalidadesOrigem: _cadastroValorFreteMinimo.LocalidadesOrigem.basicTable.BuscarRegistros().slice(),
        ModelosVeicularesCarga: _cadastroValorFreteMinimo.ModelosVeicularesCarga.basicTable.BuscarRegistros().slice(),
        TiposCarga: _cadastroValorFreteMinimo.TiposCarga.basicTable.BuscarRegistros().slice(),
        ValorMinimo: _cadastroValorFreteMinimo.ValorMinimo.val()
    };
}

function recarregarGridValorFreteMinimo() {
    var listaValorFreteMinimo = _valorFreteMinimo.ListaValorFreteMinimo.val().slice();
    var listaValorFreteMinimoCarregar = new Array();

    for (var indice = 0; indice < listaValorFreteMinimo.length; indice++) {
        var valorFreteMinimo = listaValorFreteMinimo[indice];
        var destinos = new Array();
        var origens = new Array();
        var tiposCarga = new Array();
        var modelosVeicularesCarga = new Array();

        for (var indiceLocalidadeDestino = 0; indiceLocalidadeDestino < valorFreteMinimo.LocalidadesDestino.length; indiceLocalidadeDestino++)
            destinos.push(valorFreteMinimo.LocalidadesDestino[indiceLocalidadeDestino].Descricao);

        for (var indiceClienteDestino = 0; indiceClienteDestino < valorFreteMinimo.ClientesDestino.length; indiceClienteDestino++)
            destinos.push(valorFreteMinimo.ClientesDestino[indiceClienteDestino].Descricao);

        for (var indiceEstadoDestino = 0; indiceEstadoDestino < valorFreteMinimo.EstadosDestino.length; indiceEstadoDestino++)
            destinos.push(valorFreteMinimo.EstadosDestino[indiceEstadoDestino].Descricao);

        for (var indiceLocalidadeOrigem = 0; indiceLocalidadeOrigem < valorFreteMinimo.LocalidadesOrigem.length; indiceLocalidadeOrigem++)
            origens.push(valorFreteMinimo.LocalidadesOrigem[indiceLocalidadeOrigem].Descricao);

        for (var indiceClienteOrigem = 0; indiceClienteOrigem < valorFreteMinimo.ClientesOrigem.length; indiceClienteOrigem++)
            origens.push(valorFreteMinimo.ClientesOrigem[indiceClienteOrigem].Descricao);

        for (var indiceEstadoOrigem = 0; indiceEstadoOrigem < valorFreteMinimo.EstadosOrigem.length; indiceEstadoOrigem++)
            origens.push(valorFreteMinimo.EstadosOrigem[indiceEstadoOrigem].Descricao);

        for (var indiceTipoCarga = 0; indiceTipoCarga < valorFreteMinimo.TiposCarga.length; indiceTipoCarga++)
            tiposCarga.push(valorFreteMinimo.TiposCarga[indiceTipoCarga].Descricao);

        for (var indiceModeloVeicularCarga = 0; indiceModeloVeicularCarga < valorFreteMinimo.ModelosVeicularesCarga.length; indiceModeloVeicularCarga++)
            modelosVeicularesCarga.push(valorFreteMinimo.ModelosVeicularesCarga[indiceModeloVeicularCarga].Descricao);

        listaValorFreteMinimoCarregar.push({
            Codigo: valorFreteMinimo.Codigo,
            Origens: origens.join(", "),
            Destinos: destinos.join(", "),
            TiposCarga: tiposCarga.join(", "),
            ModelosVeicularesCarga: modelosVeicularesCarga.join(", "),
            ValorMinimo: valorFreteMinimo.ValorMinimo
        });
    }

    _valorFreteMinimo.ListaValorFreteMinimo.basicTable.CarregarGrid(listaValorFreteMinimoCarregar);
}

function validarValorFreteMinimo() {
    if (!ValidarCamposObrigatorios(_cadastroValorFreteMinimo)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return false;
    }

    return true;
}

// #endregion Funções Privadas
