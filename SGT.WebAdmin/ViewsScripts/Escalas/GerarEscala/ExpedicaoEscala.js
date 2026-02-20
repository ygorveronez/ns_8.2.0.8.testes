/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="GerarEscala.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */
var _cadastroExpedicao;
var _CRUDcadastroExpedicao;
var _expedicaoEscala;

/*
 * Declaração das Classes
 */

var CadastroExpedicao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int, def: 0 });
    this.CodigoDestino = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int, def: 0 });
    this.CentroCarregamento = PropertyEntity({ text: "*Centro de Carregamento: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.ClienteDestino = PropertyEntity({ text: "*Destino: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Produto = PropertyEntity({ text: "*Produto: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), UnidadeMedida: "", required: true, enable: ko.observable(true) });
    this.Quantidade = PropertyEntity({ text: ("*Quantidade:"), val: ko.observable(""), getType: typesKnockout.decimal, maxlength: 13, required: true });
}

var CRUDCadastroExpedicao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarExpedicaoClick, type: types.event, text: "Adicionar", visible: ko.observable(false) });
    this.AdicionarDestino = PropertyEntity({ eventClick: adicionarExpedicaoDestinoClick, type: types.event, text: "Adicionar", visible: ko.observable(false) });
    this.AtualizarDestino = PropertyEntity({ eventClick: atualizarExpedicaoDestinoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.ExcluirDestino = PropertyEntity({ eventClick: excluirExpedicaoDestinoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var Expedicao = function (expedicaoEscala) {
    this.Codigo = PropertyEntity({ val: ko.observable(expedicaoEscala.Codigo), getType: typesKnockout.int, def: expedicaoEscala.Codigo });
    this.CentroCarregamento = PropertyEntity({ text: "Centro de Carregamento: ", type: types.entity, codEntity: ko.observable(expedicaoEscala.CentroCarregamento.Codigo), val: ko.observable(expedicaoEscala.CentroCarregamento.Descricao), idBtnSearch: guid() });
    this.Produto = PropertyEntity({ text: "Produto: ", type: types.entity, codEntity: ko.observable(expedicaoEscala.Produto.Codigo), val: ko.observable(expedicaoEscala.Produto.Descricao), idBtnSearch: guid(), UnidadeMedida: expedicaoEscala.Produto.UnidadeMedida });
    this.Quantidade = PropertyEntity({ text: ("Quantidade (" + expedicaoEscala.Produto.UnidadeMedida + "):"), val: ko.observable(expedicaoEscala.Quantidade), getType: typesKnockout.decimal, maxlength: 13 });
    this.Destinos = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(expedicaoEscala.Destinos), def: new Array(), idGrid: guid(), grid: undefined });

    this.Adicionar = PropertyEntity({ eventClick: adicionarExpedicaoDestinoModalClick, type: types.event, text: "Adicionar Destino", visible: ko.observable(isPermitirEditarExpedicaoEscala()) });
    this.Excluir = PropertyEntity({ eventClick: excluirExpedicaoClick, type: types.event, text: "Excluir", visible: ko.observable(isPermitirEditarExpedicaoEscala()) });
}

var ExpedicaoEscala = function () {
    this.ListaExpedicao = ko.observableArray();

    this.Adicionar = PropertyEntity({ eventClick: adicionarExpedicaoModalClick, type: types.event, text: "Adicionar Expedição", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadExpedicaoEscala() {
    _expedicaoEscala = new ExpedicaoEscala();
    KoBindings(_expedicaoEscala, "knockoutExpedicaoEscala");

    _cadastroExpedicao = new CadastroExpedicao();
    KoBindings(_cadastroExpedicao, "knockoutCadastroExpedicao");

    _CRUDcadastroExpedicao = new CRUDCadastroExpedicao();
    KoBindings(_CRUDcadastroExpedicao, "knockoutCRUDCadastroExpedicao");

    new BuscarCentrosCarregamento(_cadastroExpedicao.CentroCarregamento);
    new BuscarClientes(_cadastroExpedicao.ClienteDestino);
    new BuscarProdutos(_cadastroExpedicao.Produto, callbackProdutoSelecionado);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarExpedicaoClick() {
    if (!ValidarCamposObrigatorios(_cadastroExpedicao)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    var listaExpedicao = obterListaExpedicao();

    for (var i = 0; i < listaExpedicao.length; i++) {
        var expedicao = listaExpedicao[i];

        if ((expedicao.CentroCarregamento.codEntity() == _cadastroExpedicao.CentroCarregamento.codEntity()) && (expedicao.Produto.codEntity() == _cadastroExpedicao.Produto.codEntity())) {
            adicionarExpedicaoDestino(expedicao);
            return;
        }
    }

    adicionarExpedicao(obterCadastroExpedicaoSalvar());
    fecharModalCadastroExpedicao();
    exibirMensagemDadosNaoSalvos();
}

function adicionarExpedicaoModalClick() {
    _cadastroExpedicao.Codigo.val(guid());
    _cadastroExpedicao.CodigoDestino.val(guid());

    _CRUDcadastroExpedicao.Adicionar.visible(true);

    exibirModalCadastroExpedicao();
}

function adicionarExpedicaoDestinoClick() {
    if (!ValidarCamposObrigatorios(_cadastroExpedicao)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    var expedicao = obterExpedicaoPorCodigo(_cadastroExpedicao.Codigo.val());

    if (!expedicao)
        return;

    if (adicionarExpedicaoDestino(expedicao))
        exibirMensagemDadosNaoSalvos();
}

function adicionarExpedicaoDestinoModalClick(expedicao) {
    preencherCadastroExpedicao(expedicao);

    _cadastroExpedicao.CodigoDestino.val(guid());

    _CRUDcadastroExpedicao.AdicionarDestino.visible(true);

    exibirModalCadastroExpedicao();
}

function atualizarExpedicaoDestinoClick() {
    if (!ValidarCamposObrigatorios(_cadastroExpedicao)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    var expedicao = obterExpedicaoPorCodigo(_cadastroExpedicao.Codigo.val());

    if (!expedicao)
        return;

    for (var i = 0; i < expedicao.Destinos.val().length; i++) {
        var destino = expedicao.Destinos.val()[i];

        if (destino.Codigo == _cadastroExpedicao.CodigoDestino.val()) {
            destino.Quantidade = _cadastroExpedicao.Quantidade.val();

            atualizarExpedicao(expedicao);
            fecharModalCadastroExpedicao();
            exibirMensagemDadosNaoSalvos();
        }
    }
}

function editarExpedicaoDestinoClick(registroSelecionado, expedicao) {
    preencherCadastroExpedicao(expedicao);

    _cadastroExpedicao.CodigoDestino.val(registroSelecionado.Codigo);
    _cadastroExpedicao.ClienteDestino.codEntity(registroSelecionado.CodigoClienteDestino);
    _cadastroExpedicao.ClienteDestino.entityDescription(registroSelecionado.ClienteDestino);
    _cadastroExpedicao.ClienteDestino.val(registroSelecionado.ClienteDestino);
    _cadastroExpedicao.ClienteDestino.enable(false);
    _cadastroExpedicao.Quantidade.val(registroSelecionado.Quantidade);

    _CRUDcadastroExpedicao.AtualizarDestino.visible(true);
    _CRUDcadastroExpedicao.ExcluirDestino.visible(true);

    exibirModalCadastroExpedicao();
}

function excluirExpedicaoClick(expedicao) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a expedição e seus destinos?", function () {
        var listaExpedicao = obterListaExpedicao();

        for (var i = 0; i < listaExpedicao.length; i++) {
            if (expedicao.Codigo.val() == listaExpedicao[i].Codigo.val()) {
                listaExpedicao.splice(i, 1);
                break;
            }
        }

        _expedicaoEscala.ListaExpedicao(listaExpedicao);
        exibirMensagemDadosNaoSalvos();
    });
}

function excluirExpedicaoDestinoClick() {
    var expedicao = obterExpedicaoPorCodigo(_cadastroExpedicao.Codigo.val());

    if (!expedicao)
        return;

    for (var i = 0; i < expedicao.Destinos.val().length; i++) {
        var destino = expedicao.Destinos.val()[i];

        if (destino.Codigo == _cadastroExpedicao.CodigoDestino.val()) {
            expedicao.Destinos.val().splice(i, 1);

            atualizarExpedicao(expedicao);
            fecharModalCadastroExpedicao();
            exibirMensagemDadosNaoSalvos();
        }
    }
}

/*
 * Declaração das Funções Públicas
 */

function atualizarExpedicaoEscala() {
    var expedicaoEscalaSalvar = obterExpedicaoEscalaSalvar(false);

    salvarExpedicaoEscala(expedicaoEscalaSalvar);
}

function finalizarExpedicaoEscala() {
    var expedicaoEscalaSalvar = obterExpedicaoEscalaSalvar(true);

    salvarExpedicaoEscala(expedicaoEscalaSalvar, function () {
        executarReST("GerarEscala/FinalizarExpedicao", { Codigo: _gerarEscala.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Expedição finalizada com sucesso.");
                    editarGerarEscala(_gerarEscala.Codigo.val());
                    recarregarGridGerarEscala();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function limparCamposExpedicaoEscala() {
    _expedicaoEscala.ListaExpedicao.removeAll();
}

function preencherExpedicaoEscala(dadosExpedicoesEscala) {
    _expedicaoEscala.Adicionar.visible(isPermitirEditarExpedicaoEscala());

    for (var i = 0; i < dadosExpedicoesEscala.length; i++)
        adicionarExpedicao(dadosExpedicoesEscala[i]);
}

/*
 * Declaração das Funções Privadas
 */

function adicionarExpedicao(expedicaoEscala) {
    var expedicao = new Expedicao(expedicaoEscala);

    _expedicaoEscala.ListaExpedicao.push(expedicao);

    var linhasPorPaginas = 5;
    var ordenacao = { column: 2, dir: orderDir.asc };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: function (registroSelecionado) { editarExpedicaoDestinoClick(registroSelecionado, expedicao); }, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoClienteDestino", visible: false },
        { data: "ClienteDestino", title: "Destino", width: "60%" },
        { data: "Quantidade", title: "Quantidade", width: "20%", className: "text-align-right" }
    ];

    expedicao.Destinos.grid = new BasicDataTable(expedicao.Destinos.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    expedicao.Destinos.grid.CarregarGrid(expedicao.Destinos.val().slice(), isPermitirEditarExpedicaoEscala());
}

function adicionarExpedicaoDestino(expedicao) {
    if (isCadastroExpedicaoDestinoDuplicado(expedicao))
        return false;

    expedicao.Destinos.val().push(obterCadastroExpedicaoDestinoSalvar());

    atualizarExpedicao(expedicao);
    fecharModalCadastroExpedicao();
    exibirMensagemDadosNaoSalvos();

    return true;
}

function atualizarExpedicao(expedicao) {
    var quantidadeTotal = 0;

    for (var j = 0; j < expedicao.Destinos.val().length; j++)
        quantidadeTotal += Globalize.parseFloat(expedicao.Destinos.val()[j].Quantidade);

    expedicao.Quantidade.val(Globalize.format(quantidadeTotal, "n2"));
    expedicao.Destinos.grid.CarregarGrid(expedicao.Destinos.val().slice(), isPermitirEditarExpedicaoEscala());
}

function callbackProdutoSelecionado(registroSelecionado) {
    _cadastroExpedicao.Produto.codEntity(registroSelecionado.Codigo);
    _cadastroExpedicao.Produto.entityDescription(registroSelecionado.Descricao);
    _cadastroExpedicao.Produto.val(registroSelecionado.Descricao);
    _cadastroExpedicao.Produto.UnidadeMedida = registroSelecionado.UnidadeMedida;
}

function exibirModalCadastroExpedicao() {
    Global.abrirModal('divModalCadastroExpedicao');
    $("#divModalCadastroExpedicao").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroExpedicao);

        _cadastroExpedicao.CentroCarregamento.enable(true);
        _cadastroExpedicao.ClienteDestino.enable(true);
        _cadastroExpedicao.Produto.enable(true);

        _CRUDcadastroExpedicao.Adicionar.visible(false);
        _CRUDcadastroExpedicao.AdicionarDestino.visible(false);
        _CRUDcadastroExpedicao.AtualizarDestino.visible(false);
        _CRUDcadastroExpedicao.ExcluirDestino.visible(false);
    });
}

function fecharModalCadastroExpedicao() {
    Global.fecharModal('divModalCadastroExpedicao');
}

function isCadastroExpedicaoDestinoDuplicado(expedicao) {
    for (var i = 0; i < expedicao.Destinos.val().length; i++) {
        var destino = expedicao.Destinos.val()[i];

        if (destino.CodigoClienteDestino == _cadastroExpedicao.ClienteDestino.codEntity()) {
            exibirMensagem(tipoMensagem.atencao, "Registro Duplicado", "Já existe uma expedição do produto " + _cadastroExpedicao.Produto.val() + " para o destino informado");
            return true;
        }
    }

    return false;
}

function obterCadastroExpedicaoDestinoSalvar() {
    return {
        Codigo: _cadastroExpedicao.CodigoDestino.val(),
        CodigoClienteDestino: _cadastroExpedicao.ClienteDestino.codEntity(),
        ClienteDestino: _cadastroExpedicao.ClienteDestino.val(),
        Quantidade: _cadastroExpedicao.Quantidade.val()
    };
}

function obterCadastroExpedicaoSalvar() {
    return {
        Codigo: _cadastroExpedicao.Codigo.val(),
        Quantidade: _cadastroExpedicao.Quantidade.val(),
        CentroCarregamento: {
            Codigo: _cadastroExpedicao.CentroCarregamento.codEntity(),
            Descricao: _cadastroExpedicao.CentroCarregamento.val()
        },
        Produto: {
            Codigo: _cadastroExpedicao.Produto.codEntity(),
            Descricao: _cadastroExpedicao.Produto.val(),
            UnidadeMedida: _cadastroExpedicao.Produto.UnidadeMedida
        },
        Destinos: [obterCadastroExpedicaoDestinoSalvar()]
    };
}

function obterExpedicaoEscalaSalvar(isFinalizarExpedicao) {
    try {
        return {
            Codigo: _gerarEscala.Codigo.val(),
            ExpedicoesEscala: obterListaExpedicaoSalvar(isFinalizarExpedicao)
        };
    }
    catch (excecao) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", excecao.message);
        return undefined;
    }
}

function obterExpedicaoPorCodigo(codigo) {
    var listaExpedicao = obterListaExpedicao();

    for (var i = 0; i < listaExpedicao.length; i++) {
        var expedicao = listaExpedicao[i];

        if (expedicao.Codigo.val() == codigo)
            return expedicao;
    }

    return undefined;
}

function obterListaExpedicao() {
    return _expedicaoEscala.ListaExpedicao().slice();
}

function obterListaExpedicaoSalvar(isFinalizarExpedicao) {
    var listaExpedicao = obterListaExpedicao();
    var listaExpedicaoSalvar = new Array();

    for (var i = 0; i < listaExpedicao.length; i++) {
        var expedicao = listaExpedicao[i];
        var destinos = expedicao.Destinos.val();

        if (isFinalizarExpedicao && (destinos.length == 0))
            throw new Error("A expedição do produto " + expedicao.Produto.val() + " do centro de carregamento " + expedicao.CentroCarregamento.val() + " não possui nenhum destino informado");

        listaExpedicaoSalvar.push({
            Codigo: expedicao.Codigo.val(),
            Quantidade: expedicao.Quantidade.val(),
            CentroCarregamento: expedicao.CentroCarregamento.codEntity(),
            Produto: expedicao.Produto.codEntity(),
            Destinos: destinos
        });
    }

    return JSON.stringify(listaExpedicaoSalvar);
}

function preencherCadastroExpedicao(expedicao) {
    _cadastroExpedicao.Codigo.val(expedicao.Codigo.val());
    _cadastroExpedicao.CentroCarregamento.codEntity(expedicao.CentroCarregamento.codEntity());
    _cadastroExpedicao.CentroCarregamento.entityDescription(expedicao.CentroCarregamento.val());
    _cadastroExpedicao.CentroCarregamento.val(expedicao.CentroCarregamento.val());
    _cadastroExpedicao.CentroCarregamento.enable(false);
    _cadastroExpedicao.Produto.codEntity(expedicao.Produto.codEntity());
    _cadastroExpedicao.Produto.entityDescription(expedicao.Produto.val());
    _cadastroExpedicao.Produto.val(expedicao.Produto.val());
    _cadastroExpedicao.Produto.UnidadeMedida = expedicao.Produto.UnidadeMedida;
    _cadastroExpedicao.Produto.enable(false);
}

function salvarExpedicaoEscala(expedicaoEscalaSalvar, callbackSucesso) {
    if (!expedicaoEscalaSalvar)
        return;

    executarReST("GerarEscala/AtualizarExpedicao", expedicaoEscalaSalvar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Expedição atualizada com sucesso.");
                ocultarMensagemDadosNaoSalvos();
                limparCamposExpedicaoEscala();
                preencherExpedicaoEscala(retorno.Data);

                if (callbackSucesso instanceof Function)
                    callbackSucesso();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}
