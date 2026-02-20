/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/Cliente.js" />

// #region Objetos Globais do Arquivo

var _cadastroMotoristasUtilizados;
var _gridMotoristasUtilizados;
var _motoristasUtilizados;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CadastroMotoristasUtilizados = function () {
    this.Codigo = PropertyEntity({});
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tomador:", required: true, idBtnSearch: guid() });
    this.Quantidade = PropertyEntity({ getType: typesKnockout.decimal, text: "*Quantidade:", required: true, maxlength: 8 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarMotoristaUtilizadoClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarMotoristaUtilizadoClick, type: types.event, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirMotoristaUtilizadoClick, type: types.event, text: "Excluir", visible: ko.observable(false), enable: ko.observable(true) });
}

var MotoristasUtilizados = function () {
    this.ListaMotoristasUtilizados = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarMotoristaUtilizadoModalClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridMotoristasUtilizados() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 0, dir: orderDir.asc };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarMotoristaUtilizadoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Tomador", title: "Tomador", width: "25%" },
        { data: "Quantidade", title: "Quantidade", width: "15%", className: "text-align-center" },
        { data: "Valor", title: "Valor", width: "10%", className: "text-align-right" },
        { data: "Total", title: "Total", width: "10%", className: "text-align-right" }
    ];

    _gridMotoristasUtilizados = new BasicDataTable(_motoristasUtilizados.ListaMotoristasUtilizados.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridMotoristasUtilizados.CarregarGrid([]);
}

function loadMotoristasUtilizados() {
    _motoristasUtilizados = new MotoristasUtilizados();
    KoBindings(_motoristasUtilizados, "knockoutMotoristasUtilizados");

    _cadastroMotoristasUtilizados = new CadastroMotoristasUtilizados();
    KoBindings(_cadastroMotoristasUtilizados, "knockoutCadastroMotoristasUtilizados");

    new BuscarClientes(_cadastroMotoristasUtilizados.Tomador);

    loadGridMotoristasUtilizados();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarMotoristaUtilizadoClick() {
    if (!ValidarCamposObrigatorios(_cadastroMotoristasUtilizados)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios");
        return;
    }

    if (!validarCadastroMotoristaUtilizadoDuplicado())
        return;

    _motoristasUtilizados.ListaMotoristasUtilizados.val().push(obterCadastroMotoristaUtilizadoSalvar());
    recarregarGridMotoristasUtilizados();
    fecharModalCadastroMotoristaUtilizado();
}

function adicionarMotoristaUtilizadoModalClick() {
    _cadastroMotoristasUtilizados.Codigo.val(guid());

    controlarBotoesCadastroMotoristaUtilizadoHabilitados(false);
    exibirModalCadastroMotoristaUtilizado();
}

function atualizarMotoristaUtilizadoClick() {
    if (!ValidarCamposObrigatorios(_cadastroMotoristasUtilizados)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios");
        return;
    }

    if (!validarCadastroMotoristaUtilizadoDuplicado())
        return;

    var listaMotoristasUtilizados = obterListaMotoristasUtilizados();

    for (var i = 0; i < listaMotoristasUtilizados.length; i++) {
        if (_cadastroMotoristasUtilizados.Codigo.val() == listaMotoristasUtilizados[i].Codigo) {
            listaMotoristasUtilizados.splice(i, 1, obterCadastroMotoristaUtilizadoSalvar());
            break;
        }
    }
    _motoristasUtilizados.ListaMotoristasUtilizados.val(listaMotoristasUtilizados);
    recarregarGridMotoristasUtilizados();
    fecharModalCadastroMotoristaUtilizado();

}

function editarMotoristaUtilizadoClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroMotoristasUtilizados, { Data: registroSelecionado });

    _cadastroMotoristasUtilizados.Tomador.codEntity(registroSelecionado.CodigoTomador);
    _cadastroMotoristasUtilizados.Tomador.val(registroSelecionado.Tomador);

    controlarBotoesCadastroMotoristaUtilizadoHabilitados(true);
    exibirModalCadastroMotoristaUtilizado();
}

function excluirMotoristaUtilizadoClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir o registro?", function () {
        var listaMotoristasUtilizados = obterListaMotoristasUtilizados();

        for (var i = 0; i < listaMotoristasUtilizados.length; i++) {
            if (_cadastroMotoristasUtilizados.Codigo.val() == listaMotoristasUtilizados[i].Codigo) {
                listaMotoristasUtilizados.splice(i, 1);
                break;
            }
        }

        _motoristasUtilizados.ListaMotoristasUtilizados.val(listaMotoristasUtilizados);

        recarregarGridMotoristasUtilizados();
        fecharModalCadastroMotoristaUtilizado();
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposMotoristasUtilizados() {
    preencherMotoristasUtilizados([]);
}

function preencherMotoristasUtilizados(dadosMotoristasUtilizados) {
    _motoristasUtilizados.ListaMotoristasUtilizados.val(dadosMotoristasUtilizados);
    _motoristasUtilizados.Adicionar.visible(_fechamentoFrete.Codigo.val() == 0);

    recarregarGridMotoristasUtilizados();
}

function preencherMotoristasUtilizadosSalvar(dadosFechamento) {
    dadosFechamento["MotoristasUtilizados"] = obterListaMotoristasUtilizadosSalvar();
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarBotoesCadastroMotoristaUtilizadoHabilitados(isEdicao) {
    _cadastroMotoristasUtilizados.Adicionar.visible(!isEdicao);
    _cadastroMotoristasUtilizados.Atualizar.visible(isEdicao);
    _cadastroMotoristasUtilizados.Excluir.visible(isEdicao);
}

function exibirModalCadastroMotoristaUtilizado() {
    Global.abrirModal('divModalCadastroMotoristasUtilizados');
    $("#divModalCadastroMotoristasUtilizados").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroMotoristasUtilizados);
    });
}

function fecharModalCadastroMotoristaUtilizado() {
    Global.fecharModal('divModalCadastroMotoristasUtilizados');
}

function obterCadastroMotoristaUtilizadoSalvar() {
    var quantidade = Globalize.parseFloat(_cadastroMotoristasUtilizados.Quantidade.val());
    var valorPorMotorista = Globalize.parseFloat(_dadosFechamento.ValorPorMotorista.val());

    return {
        Codigo: _cadastroMotoristasUtilizados.Codigo.val(),
        CodigoTomador: _cadastroMotoristasUtilizados.Tomador.codEntity(),
        Tomador: _cadastroMotoristasUtilizados.Tomador.val(),
        Quantidade: _cadastroMotoristasUtilizados.Quantidade.val(),
        Valor: _dadosFechamento.ValorPorMotorista.val(),
        Total: Globalize.format((quantidade * valorPorMotorista), "n2")
    };
}

function obterListaMotoristasUtilizados() {
    return _motoristasUtilizados.ListaMotoristasUtilizados.val().slice();
}

function obterListaMotoristasUtilizadosSalvar() {
    var listaMotoristasUtilizados = obterListaMotoristasUtilizados();

    return JSON.stringify(listaMotoristasUtilizados);
}

function recarregarGridMotoristasUtilizados() {
    var listaMotoristasUtilizados = obterListaMotoristasUtilizados();

    _gridMotoristasUtilizados.CarregarGrid(listaMotoristasUtilizados, (_fechamentoFrete.Codigo.val() == 0));
}

function validarCadastroMotoristaUtilizadoDuplicado() {
    var listaMotoristasUtilizados = obterListaMotoristasUtilizados();

    for (var i = 0; i < listaMotoristasUtilizados.length; i++) {
        var motoristaUtilizado = listaMotoristasUtilizados[i];

        if (
            (_cadastroMotoristasUtilizados.Codigo.val() != motoristaUtilizado.Codigo) &&
            (_cadastroMotoristasUtilizados.Tomador.codEntity() == motoristaUtilizado.CodigoTomador)
        ) {
            exibirMensagem(tipoMensagem.atencao, "Registro Duplicado", "O registro já foi adicionado, favor verificar!");
            return false;
        }
    }

    return true;
}

// #endregion Funções Privadas
