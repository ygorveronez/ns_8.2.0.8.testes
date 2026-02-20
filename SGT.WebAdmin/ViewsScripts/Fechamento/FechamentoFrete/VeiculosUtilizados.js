/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />

// #region Objetos Globais do Arquivo

var _cadastroVeiculosUtilizados;
var _gridVeiculosUtilizados;
var _veiculosUtilizados;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CadastroVeiculosUtilizados = function () {
    this.Codigo = PropertyEntity({});
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo Veicular de Carga:", required: true, idBtnSearch: guid() });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tomador:", required: true, idBtnSearch: guid() });
    this.Quantidade = PropertyEntity({ getType: typesKnockout.decimal, text: "*Quantidade:", required: true, maxlength: 8 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarVeiculoUtilizadoClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarVeiculoUtilizadoClick, type: types.event, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirVeiculoUtilizadoClick, type: types.event, text: "Excluir", visible: ko.observable(false), enable: ko.observable(true) });
}

var VeiculosUtilizados = function () {
    this.ListaVeiculosUtilizados = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarVeiculoUtilizadoModalClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridVeiculosUtilizados() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 0, dir: orderDir.asc };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarVeiculoUtilizadoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "ModeloVeicularCarga", title: "Modelo Veicular", width: "30%" },
        { data: "Tomador", title: "Tomador", width: "25%" },
        { data: "Quantidade", title: "Quantidade", width: "15%", className: "text-align-center" },
        { data: "Valor", title: "Valor", width: "10%", className: "text-align-right" },
        { data: "Total", title: "Total", width: "10%", className: "text-align-right" }
    ];

    _gridVeiculosUtilizados = new BasicDataTable(_veiculosUtilizados.ListaVeiculosUtilizados.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridVeiculosUtilizados.CarregarGrid([]);
}

function loadVeiculosUtilizados() {
    _veiculosUtilizados = new VeiculosUtilizados();
    KoBindings(_veiculosUtilizados, "knockoutVeiculosUtilizados");

    _cadastroVeiculosUtilizados = new CadastroVeiculosUtilizados();
    KoBindings(_cadastroVeiculosUtilizados, "knockoutCadastroVeiculosUtilizados");

    new BuscarModelosVeicularesCarga(_cadastroVeiculosUtilizados.ModeloVeicularCarga);
    new BuscarClientes(_cadastroVeiculosUtilizados.Tomador);

    loadGridVeiculosUtilizados();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarVeiculoUtilizadoClick() {
    if (!ValidarCamposObrigatorios(_cadastroVeiculosUtilizados)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios");
        return;
    }

    if (!validarCadastroVeiculoUtilizadoDuplicado())
        return;

    _veiculosUtilizados.ListaVeiculosUtilizados.val().push(obterCadastroVeiculoUtilizadoSalvar());
    recarregarGridVeiculosUtilizados();
    fecharModalCadastroVeiculoUtilizado();
}

function adicionarVeiculoUtilizadoModalClick() {
    _cadastroVeiculosUtilizados.Codigo.val(guid());

    controlarBotoesCadastroVeiculoUtilizadoHabilitados(false);
    exibirModalCadastroVeiculoUtilizado();
}

function atualizarVeiculoUtilizadoClick() {
    if (!ValidarCamposObrigatorios(_cadastroVeiculosUtilizados)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios");
        return;
    }

    if (!validarCadastroVeiculoUtilizadoDuplicado())
        return;

    var listaVeiculosUtilizados = obterListaVeiculosUtilizados();

    for (var i = 0; i < listaVeiculosUtilizados.length; i++) {
        if (_cadastroVeiculosUtilizados.Codigo.val() == listaVeiculosUtilizados[i].Codigo) {
            listaVeiculosUtilizados.splice(i, 1, obterCadastroVeiculoUtilizadoSalvar());
            break;
        }
    }
    _veiculosUtilizados.ListaVeiculosUtilizados.val(listaVeiculosUtilizados);
    recarregarGridVeiculosUtilizados();
    fecharModalCadastroVeiculoUtilizado();

}

function editarVeiculoUtilizadoClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroVeiculosUtilizados, { Data: registroSelecionado });

    _cadastroVeiculosUtilizados.ModeloVeicularCarga.codEntity(registroSelecionado.CodigoModeloVeicularCarga);
    _cadastroVeiculosUtilizados.ModeloVeicularCarga.val(registroSelecionado.ModeloVeicularCarga);
    _cadastroVeiculosUtilizados.Tomador.codEntity(registroSelecionado.CodigoTomador);
    _cadastroVeiculosUtilizados.Tomador.val(registroSelecionado.Tomador);

    controlarBotoesCadastroVeiculoUtilizadoHabilitados(true);
    exibirModalCadastroVeiculoUtilizado();
}

function excluirVeiculoUtilizadoClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir o registro?", function () {
        var listaVeiculosUtilizados = obterListaVeiculosUtilizados();

        for (var i = 0; i < listaVeiculosUtilizados.length; i++) {
            if (_cadastroVeiculosUtilizados.Codigo.val() == listaVeiculosUtilizados[i].Codigo) {
                listaVeiculosUtilizados.splice(i, 1);
                break;
            }
        }

        _veiculosUtilizados.ListaVeiculosUtilizados.val(listaVeiculosUtilizados);

        recarregarGridVeiculosUtilizados();
        fecharModalCadastroVeiculoUtilizado();
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposVeiculosUtilizados() {
    preencherVeiculosUtilizados([]);
}

function preencherVeiculosUtilizados(dadosVeiculosUtilizados) {
    _veiculosUtilizados.ListaVeiculosUtilizados.val(dadosVeiculosUtilizados);
    _veiculosUtilizados.Adicionar.visible(_fechamentoFrete.Codigo.val() == 0);

    recarregarGridVeiculosUtilizados();
}

function preencherVeiculosUtilizadosSalvar(dadosFechamento) {
    dadosFechamento["VeiculosUtilizados"] = obterListaVeiculosUtilizadosSalvar();
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarBotoesCadastroVeiculoUtilizadoHabilitados(isEdicao) {
    _cadastroVeiculosUtilizados.Adicionar.visible(!isEdicao);
    _cadastroVeiculosUtilizados.Atualizar.visible(isEdicao);
    _cadastroVeiculosUtilizados.Excluir.visible(isEdicao);
}

function exibirModalCadastroVeiculoUtilizado() {
    Global.abrirModal('divModalCadastroVeiculosUtilizados');
    $("#divModalCadastroVeiculosUtilizados").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroVeiculosUtilizados);
    });
}

function fecharModalCadastroVeiculoUtilizado() {
    Global.fecharModal('divModalCadastroVeiculosUtilizados');
}

function obterCadastroVeiculoUtilizadoSalvar() {
    var acordosPorModeloVeicular = _dadosFechamento.Acordos.filter(o => o.CodigoModeloVeicular == _cadastroVeiculosUtilizados.ModeloVeicularCarga.codEntity())
    var quantidade = Globalize.parseFloat(_cadastroVeiculosUtilizados.Quantidade.val());
    var valorPorModeloVeicular = 0;

    if (acordosPorModeloVeicular.length > 0)
        valorPorModeloVeicular = Globalize.parseFloat(acordosPorModeloVeicular[0].Valor);

    return {
        Codigo: _cadastroVeiculosUtilizados.Codigo.val(),
        CodigoModeloVeicularCarga: _cadastroVeiculosUtilizados.ModeloVeicularCarga.codEntity(),
        CodigoTomador: _cadastroVeiculosUtilizados.Tomador.codEntity(),
        ModeloVeicularCarga: _cadastroVeiculosUtilizados.ModeloVeicularCarga.val(),
        Tomador: _cadastroVeiculosUtilizados.Tomador.val(),
        Quantidade: _cadastroVeiculosUtilizados.Quantidade.val(),
        Valor: Globalize.format(valorPorModeloVeicular, "n2"),
        Total: Globalize.format((quantidade * valorPorModeloVeicular), "n2")
    };
}

function obterListaVeiculosUtilizados() {
    return _veiculosUtilizados.ListaVeiculosUtilizados.val().slice();
}

function obterListaVeiculosUtilizadosSalvar() {
    var listaVeiculosUtilizados = obterListaVeiculosUtilizados();

    return JSON.stringify(listaVeiculosUtilizados);
}

function recarregarGridVeiculosUtilizados() {
    var listaVeiculosUtilizados = obterListaVeiculosUtilizados();

    _gridVeiculosUtilizados.CarregarGrid(listaVeiculosUtilizados, (_fechamentoFrete.Codigo.val() == 0));
}

function validarCadastroVeiculoUtilizadoDuplicado() {
    var listaVeiculosUtilizados = obterListaVeiculosUtilizados();

    for (var i = 0; i < listaVeiculosUtilizados.length; i++) {
        var veiculoUtilizado = listaVeiculosUtilizados[i];

        if (
            (_cadastroVeiculosUtilizados.Codigo.val() != veiculoUtilizado.Codigo) &&
            (_cadastroVeiculosUtilizados.ModeloVeicularCarga.codEntity() == veiculoUtilizado.CodigoModeloVeicularCarga) &&
            (_cadastroVeiculosUtilizados.Tomador.codEntity() == veiculoUtilizado.CodigoTomador)
        ) {
            exibirMensagem(tipoMensagem.atencao, "Registro Duplicado", "O registro já foi adicionado, favor verificar!");
            return false;
        }
    }

    return true;
}

// #endregion Funções Privadas
