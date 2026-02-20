/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Produto.js" />

var _cadastroSuprimentoGas, _suprimentoGas, _gridSuprimentoGas, _CRUDCadastroSuprimentoGas;

var SuprimentoGas = function () {
    this.HabilitarSolicitacao = PropertyEntity({ getType: typesKnockout.bool, val: _pessoa.HabilitarSolicitacaoGas.val, def: false, text: Localization.Resources.Filiais.Filial.HabilitarSolicitacao });
    
    this.Grid = PropertyEntity({ type: types.local });
    this.Adicionar = PropertyEntity({ type: types.event, text: Localization.Resources.Filiais.Filial.AdicionarSuprimentoGas, eventClick: abrirModalSuprimentoGasClick });
}

var CadastroSuprimentoGas = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });

    this.HoraLimiteSolicitacao = PropertyEntity({ getType: typesKnockout.time, val: ko.observable(""), text: Localization.Resources.Filiais.Filial.HoraLimiteSolicitacao.getFieldDescription() });
    this.HoraLimiteGerente = PropertyEntity({ getType: typesKnockout.time, val: ko.observable(""), text: Localization.Resources.Filiais.Filial.HoraLimiteGerente.getFieldDescription() });
    this.HoraBloqueioSolicitacao = PropertyEntity({ getType: typesKnockout.time, val: ko.observable(""), text: Localization.Resources.Filiais.Filial.HoraBloqueioSolicitacao.getFieldDescription() });
    
    this.SupridorPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, visible: ko.observable(true), text: "Supridor Padrão:", idBtnSearch: guid() });
    this.ProdutoPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, visible: ko.observable(true), text: Localization.Resources.Filiais.Filial.ProdutoPadrao.getFieldDescription(), idBtnSearch: guid() });
    this.ModeloVeicularPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, visible: ko.observable(true), text: Localization.Resources.Filiais.Filial.ModeloVeicularPadrao.getFieldDescription(), idBtnSearch: guid() });
    this.TipoCargaPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, visible: ko.observable(true), text: Localization.Resources.Filiais.Filial.TipoCargaPadrao.getFieldDescription(), idBtnSearch: guid() });
    this.TipoOperacaoPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, visible: ko.observable(true), text: Localization.Resources.Filiais.Filial.TipoOperacaoPadrao.getFieldDescription(), idBtnSearch: guid() });

    this.Capacidade = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable(0), def: 0, text: Localization.Resources.Filiais.Filial.CapacidadeToneladas.getFieldDescription() });
    this.Lastro = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable(0), def: 0, text: "Lastro:" });
    this.EstoqueMinimo = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable(0), def: 0, text: "Estoque Mínimo:" });
    this.EstoqueMaximo = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable(0), def: 0, text: "Estoque Máximo:" });

    this.NotificarPorEmailLimite = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: Localization.Resources.Filiais.Filial.NotificarPorEmail.getFieldDescription() });
    this.NotificarPorEmailGerente = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: Localization.Resources.Filiais.Filial.NotificarPorEmail.getFieldDescription() });
    this.NotificarPorEmailBloqueio = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: Localization.Resources.Filiais.Filial.NotificarPorEmail.getFieldDescription() });
}

var CRUDCadastroSuprimentoGas = function () {
    this.Salvar = PropertyEntity({ eventClick: salvarSuprimentoGasClick, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(true) });
}

function loadSuprimentoDeGas() {
    _cadastroSuprimentoGas = new CadastroSuprimentoGas();
    _suprimentoGas = new SuprimentoGas();
    _CRUDCadastroSuprimentoGas = new CRUDCadastroSuprimentoGas();

    KoBindings(_suprimentoGas, "knockoutSuprimentoDeGas");
    KoBindings(_cadastroSuprimentoGas, "knockoutCadastroSuprimentoDeGas");
    KoBindings(_CRUDCadastroSuprimentoGas, "knockoutCRUDCadastroAbastecimentoGas");

    new BuscarClientes(_cadastroSuprimentoGas.SupridorPadrao, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
    new BuscarTiposOperacao(_cadastroSuprimentoGas.TipoOperacaoPadrao);
    new BuscarTiposdeCarga(_cadastroSuprimentoGas.TipoCargaPadrao);
    new BuscarProdutos(_cadastroSuprimentoGas.ProdutoPadrao);
    new BuscarModelosVeicularesCarga(_cadastroSuprimentoGas.ModeloVeicularPadrao);

    loadGridSuprimentoGas();
}

function loadGridSuprimentoGas() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.list, tamanho: 7,
        opcoes: [
            { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarSuprimentoGasClick },
            { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirSuprimentoGasClick }
        ]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "HoraLimiteSolicitacao", visible: false },
        { data: "HoraLimiteGerente", visible: false },
        { data: "HoraBloqueioSolicitacao", visible: false },
        { data: "NotificarPorEmailLimite", visible: false },
        { data: "NotificarPorEmailGerente", visible: false },
        { data: "NotificarPorEmailBloqueio", visible: false },
        { data: "SupridorPadrao", visible: false },
        { data: "ProdutoPadrao", visible: false },
        { data: "ModeloVeicularPadrao", visible: false },
        { data: "TipoCargaPadrao", visible: false },
        { data: "TipoOperacaoPadrao", visible: false },
        { data: "Capacidade", title: Localization.Resources.Filiais.Filial.CapacidadeToneladas, width: "10%" },
        { data: "Lastro", title: "Lastro", width: "10%" },
        { data: "EstoqueMinimo", title: "Estoque Mínimo", width: "10%" },
        { data: "EstoqueMaximo", title: "Estoque Máximo", width: "10%" },
        { data: "SupridorPadraoDescricao", title: Localization.Resources.Filiais.Filial.SupridorPadrao, width: "10%" },
        { data: "ProdutoPadraoDescricao", title: Localization.Resources.Filiais.Filial.ProdutoPadrao, width: "10%" },
        { data: "ModeloVeicularPadraoDescricao", title: Localization.Resources.Filiais.Filial.ModeloVeicularPadrao, width: "10%" },
        { data: "TipoCargaPadraoDescricao", title: Localization.Resources.Filiais.Filial.TipoCargaPadrao, width: "10%" },
        { data: "TipoOperacaoPadraoDescricao", title: Localization.Resources.Filiais.Filial.TipoOperacaoPadrao, width: "10%" }
    ];

    _gridSuprimentoGas = new BasicDataTable(_suprimentoGas.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridSuprimentoGas.CarregarGrid([]);
}

function editarSuprimentoGasClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroSuprimentoGas, { Data: registroSelecionado });
    
    _cadastroSuprimentoGas.TipoCargaPadrao.val(registroSelecionado.TipoCargaPadraoDescricao);
    _cadastroSuprimentoGas.ProdutoPadrao.val(registroSelecionado.ProdutoPadraoDescricao);
    _cadastroSuprimentoGas.TipoOperacaoPadrao.val(registroSelecionado.TipoOperacaoPadraoDescricao);
    _cadastroSuprimentoGas.ModeloVeicularPadrao.val(registroSelecionado.ModeloVeicularPadraoDescricao);
    _cadastroSuprimentoGas.SupridorPadrao.val(registroSelecionado.SupridorPadraoDescricao);

    _cadastroSuprimentoGas.TipoCargaPadrao.entityDescription(registroSelecionado.TipoCargaPadraoDescricao);
    _cadastroSuprimentoGas.ProdutoPadrao.entityDescription(registroSelecionado.ProdutoPadraoDescricao);
    _cadastroSuprimentoGas.TipoOperacaoPadrao.entityDescription(registroSelecionado.TipoOperacaoPadraoDescricao);
    _cadastroSuprimentoGas.ModeloVeicularPadrao.entityDescription(registroSelecionado.ModeloVeicularPadraoDescricao);
    _cadastroSuprimentoGas.SupridorPadrao.entityDescription(registroSelecionado.SupridorPadraoDescricao);
 
    _cadastroSuprimentoGas.TipoCargaPadrao.codEntity(registroSelecionado.TipoCargaPadrao);
    _cadastroSuprimentoGas.SupridorPadrao.codEntity(registroSelecionado.SupridorPadrao);
    _cadastroSuprimentoGas.ModeloVeicularPadrao.codEntity(registroSelecionado.ModeloVeicularPadrao);
    _cadastroSuprimentoGas.TipoOperacaoPadrao.codEntity(registroSelecionado.TipoOperacaoPadrao);
    _cadastroSuprimentoGas.ProdutoPadrao.codEntity(registroSelecionado.ProdutoPadrao);
    
    abrirModalSuprimentoGasClick();
}

function excluirSuprimentoGasClick(registroSelecionado) {
    var lista = _gridSuprimentoGas.BuscarRegistros();

    for (var i = 0; i < lista.length; i++) {
        if (lista[i].Codigo == registroSelecionado.Codigo) {
            lista.splice(i, 1);
            break;
        }
    }

    _gridSuprimentoGas.CarregarGrid(lista);
}

function abrirModalSuprimentoGasClick() {
    $("#divModalFilialSuprimentoGasCadastro")
        .modal("show")
        .on("hidden.bs.modal", function () {
            LimparCampos(_cadastroSuprimentoGas);
        });
}

function salvarSuprimentoGasClick() {
    var registros = _gridSuprimentoGas.BuscarRegistros();

    var elementoGrid = {
        Codigo: !string.IsNullOrWhiteSpace((_cadastroSuprimentoGas.Codigo.val()).toString()) ? _cadastroSuprimentoGas.Codigo.val() : guid(),
        SupridorPadrao: _cadastroSuprimentoGas.SupridorPadrao.codEntity(),
        ProdutoPadrao: _cadastroSuprimentoGas.ProdutoPadrao.codEntity(),
        ModeloVeicularPadrao: _cadastroSuprimentoGas.ModeloVeicularPadrao.codEntity(),
        TipoCargaPadrao: _cadastroSuprimentoGas.TipoCargaPadrao.codEntity(),
        TipoOperacaoPadrao: _cadastroSuprimentoGas.TipoOperacaoPadrao.codEntity(),
        Capacidade: _cadastroSuprimentoGas.Capacidade.val(),
        Lastro: _cadastroSuprimentoGas.Lastro.val(),
        EstoqueMinimo: _cadastroSuprimentoGas.EstoqueMinimo.val(),
        EstoqueMaximo: _cadastroSuprimentoGas.EstoqueMaximo.val(),
        SupridorPadraoDescricao: _cadastroSuprimentoGas.SupridorPadrao.val(),
        ProdutoPadraoDescricao: _cadastroSuprimentoGas.ProdutoPadrao.val(),
        ModeloVeicularPadraoDescricao: _cadastroSuprimentoGas.ModeloVeicularPadrao.val(),
        TipoCargaPadraoDescricao: _cadastroSuprimentoGas.TipoCargaPadrao.val(),
        TipoOperacaoPadraoDescricao: _cadastroSuprimentoGas.TipoOperacaoPadrao.val(),
        HoraLimiteSolicitacao: _cadastroSuprimentoGas.HoraLimiteSolicitacao.val(),
        HoraLimiteGerente: _cadastroSuprimentoGas.HoraLimiteGerente.val(),
        HoraBloqueioSolicitacao: _cadastroSuprimentoGas.HoraBloqueioSolicitacao.val(),
        NotificarPorEmailLimite: _cadastroSuprimentoGas.NotificarPorEmailLimite.val(),
        NotificarPorEmailGerente: _cadastroSuprimentoGas.NotificarPorEmailGerente.val(),
        NotificarPorEmailBloqueio: _cadastroSuprimentoGas.NotificarPorEmailBloqueio.val()
    };
    
    var atualizando = false;

    for (var i = 0; i < registros.length; i++) {
        if (registros[i].Codigo == elementoGrid.Codigo) {
            atualizando = true;
            registros.splice(i, 1, elementoGrid);
            break;
        }
    }

    if (!atualizando)
        registros.push(elementoGrid);

    _gridSuprimentoGas.CarregarGrid(registros);

    Global.fecharModal("divModalFilialSuprimentoGasCadastro");
}

function limparCamposSuprimentoDeGas() {
    LimparCampos(_cadastroSuprimentoGas);
    _gridSuprimentoGas.CarregarGrid([]);
    LimparCampos(_suprimentoGas);
}

function preencherFilialSuprimentoDeGas(data) {
    _suprimentoGas.HabilitarSolicitacao.val(data.HabilitarSolicitacao);
    _gridSuprimentoGas.CarregarGrid(data.ListaSuprimentosGas);
}

function obterSuprimentoGas() {
    return JSON.stringify(_gridSuprimentoGas.BuscarRegistros());
}