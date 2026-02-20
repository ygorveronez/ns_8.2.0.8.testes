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
var _cadastroVeiculoEscalado;
var _CRUDCadastroVeiculoEscalado;
var _origemDestinoEscala;

/*
 * Declaração das Classes
 */

var CadastroVeiculoEscalado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int, def: 0 });
    this.CodigoVeiculoEscalado = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int, def: 0 });
    this.Empresa = PropertyEntity({ text: "*Empresa: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.HoraCarregamento = PropertyEntity({ text: "*Hora do Carregamento: ", getType: typesKnockout.time, required: true });
    this.ModeloVeicularCarga = PropertyEntity({ text: "*Modelo: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });
    this.Motorista = PropertyEntity({ text: "*Motorista: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true });
    this.Quantidade = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable("0,00"), maxlength: 13, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, visible: false });
    this.Veiculo = PropertyEntity({ text: "*Veículo: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
}

var CRUDCadastroVeiculoEscalado = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarVeiculoEscaladoClick, type: types.event, text: "Adicionar", visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarVeiculoEscaladoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirVeiculoEscaladoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var OrigemDestino = function (origemDestino) {
    this.Codigo = PropertyEntity({ val: ko.observable(origemDestino.Codigo), getType: typesKnockout.int, def: origemDestino.Codigo });
    this.Destino = PropertyEntity({ val: ko.observable(origemDestino.Destino) });
    this.Origem = PropertyEntity({ val: ko.observable(origemDestino.Origem) });
    this.Produto = PropertyEntity({ val: ko.observable(origemDestino.Produto) });
    this.Quantidade = PropertyEntity({ val: ko.observable(origemDestino.Quantidade), getType: typesKnockout.decimal, maxlength: 13 });
    this.VeiculosEscalados = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(origemDestino.VeiculosEscalados), def: new Array(), idGrid: guid(), grid: undefined });

    this.DescricaoOrigemDestino = ko.computed(function () { return this.Origem.val() + " até " + this.Destino.val(); }, this);
    this.DescricaoQuantidade = ko.computed(function () { return this.Quantidade.val() + " (" + origemDestino.UnidadeMedida + ")"; }, this);

    this.Adicionar = PropertyEntity({ eventClick: adicionarVeiculoEscaladoModalClick, type: types.event, text: "Adicionar Veículo", visible: ko.observable(isPermitirEditarVeiculoEscala()) });
}

var OrigemDestinoEscala = function () {
    this.EmEdicao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });
    this.ListaOrigemDestino = ko.observableArray();

    this.SugerirveiculosEscalados = PropertyEntity({ eventClick: sugerirVeiculosEscaladosClick, type: types.event, text: "Sugerir Veículos" });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadVeiculoEscala() {
    _origemDestinoEscala = new OrigemDestinoEscala();
    KoBindings(_origemDestinoEscala, "knockoutOrigemDestinoEscala");

    _cadastroVeiculoEscalado = new CadastroVeiculoEscalado();
    KoBindings(_cadastroVeiculoEscalado, "knockoutCadastroVeiculoEscalado");

    _CRUDCadastroVeiculoEscalado = new CRUDCadastroVeiculoEscalado();
    KoBindings(_CRUDCadastroVeiculoEscalado, "knockoutCRUDCadastroVeiculoEscalado");

    new BuscarMotoristas(_cadastroVeiculoEscalado.Motorista, null, _cadastroVeiculoEscalado.Empresa);
    new BuscarTransportadores(_cadastroVeiculoEscalado.Empresa, callbackEmpresaSelecionada, null, true);
    new BuscarVeiculos(_cadastroVeiculoEscalado.Veiculo, callbackVeiculoSelecionado, _cadastroVeiculoEscalado.Empresa, null, _cadastroVeiculoEscalado.Motorista, true, null, null, true, null, null, null, null, null, null, null, null, true, null, null, true);

    loadVeiculoEscalaVeiculo();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarVeiculoEscaladoClick() {
    if (!ValidarCamposObrigatorios(_cadastroVeiculoEscalado)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    var origemDestino = obterOrigemDestinoPorCodigo(_cadastroVeiculoEscalado.Codigo.val());

    if (!origemDestino)
        return;

    var veiculoEscaladoSalvar = obterCadastroVeiculoEscaladoSalvar(origemDestino);

    if (isVeiculoEscaladoDuplicado(origemDestino, veiculoEscaladoSalvar))
        return;

    if (isPossuiRestricaoRodagem(origemDestino, veiculoEscaladoSalvar))
        return;

    origemDestino.VeiculosEscalados.val().push(veiculoEscaladoSalvar);

    adicionarEscalaVeiculoEscalado(veiculoEscaladoSalvar);
    atualizarOrigemDestino(origemDestino);
    fecharModalCadastroVeiculoEscalado();
    exibirMensagemDadosNaoSalvos();
}

function adicionarVeiculoEscaladoModalClick(origemDestino) {
    _cadastroVeiculoEscalado.Codigo.val(origemDestino.Codigo.val());
    _cadastroVeiculoEscalado.CodigoVeiculoEscalado.val(guid());
    _CRUDCadastroVeiculoEscalado.Adicionar.visible(true);

    exibirModalCadastroVeiculoEscalado();
}

function atualizarVeiculoEscaladoClick() {
    if (!ValidarCamposObrigatorios(_cadastroVeiculoEscalado)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    var origemDestino = obterOrigemDestinoPorCodigo(_cadastroVeiculoEscalado.Codigo.val());

    if (!origemDestino)
        return;

    var veiculoEscaladoSalvar = obterCadastroVeiculoEscaladoSalvar(origemDestino);

    if (isVeiculoEscaladoDuplicado(origemDestino, veiculoEscaladoSalvar))
        return;

    if (isPossuiRestricaoRodagem(origemDestino, veiculoEscaladoSalvar))
        return;

    for (var i = 0; i < origemDestino.VeiculosEscalados.val().length; i++) {
        var veiculoEscalado = origemDestino.VeiculosEscalados.val()[i];

        if (veiculoEscalado.Codigo == veiculoEscaladoSalvar.Codigo) {
            veiculoEscalado.CodigoMotorista = veiculoEscaladoSalvar.CodigoMotorista;
            veiculoEscalado.Motorista = veiculoEscaladoSalvar.Motorista;
            veiculoEscalado.HoraCarregamento = veiculoEscaladoSalvar.HoraCarregamento;

            atualizarOrigemDestino(origemDestino);
            fecharModalCadastroVeiculoEscalado();
            exibirMensagemDadosNaoSalvos();
        }
    }
}

function editarVeiculoEscaladoClick(registroSelecionado, origemDestino) {
    preencherCadastroVeiculoEscalado(registroSelecionado, origemDestino);

    _cadastroVeiculoEscalado.Empresa.enable(false);
    _cadastroVeiculoEscalado.Veiculo.enable(false);
    _CRUDCadastroVeiculoEscalado.Atualizar.visible(true);
    _CRUDCadastroVeiculoEscalado.Excluir.visible(true);

    exibirModalCadastroVeiculoEscalado();
}

function excluirVeiculoEscaladoClick() {
    var origemDestino = obterOrigemDestinoPorCodigo(_cadastroVeiculoEscalado.Codigo.val());

    if (!origemDestino)
        return;

    for (var i = 0; i < origemDestino.VeiculosEscalados.val().length; i++) {
        var veiculoEscalado = origemDestino.VeiculosEscalados.val()[i];

        if (veiculoEscalado.Codigo == _cadastroVeiculoEscalado.CodigoVeiculoEscalado.val()) {
            origemDestino.VeiculosEscalados.val().splice(i, 1);

            excluirEscalaVeiculoEscalado(veiculoEscalado);
            atualizarOrigemDestino(origemDestino);
            fecharModalCadastroVeiculoEscalado();
            exibirMensagemDadosNaoSalvos();
        }
    }
}

function sugerirVeiculosEscaladosClick() {
    executarReST("GerarEscala/SugerirVeiculosEscalados", { Codigo: _gerarEscala.Codigo.val()}, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Sugestão de veículos concluída com sucesso.");
                exibirMensagemDadosNaoSalvos();
                limparCamposVeiculoEscala();
                preencherVeiculoEscala(retorno.Data.OrigensDestinos);
                preencherVeiculoEscalaVeiculo(retorno.Data.ListaVeiculo);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções Públicas
 */

function atualizarVeiculoEscala() {
    var origemDestinoEscalaSalvar = obterOrigemDestinoEscalaSalvar(false);

    salvarVeiculoEscala(origemDestinoEscalaSalvar);
}

function finalizarEscala() {
    var origemDestinoEscalaSalvar = obterOrigemDestinoEscalaSalvar(true);

    salvarVeiculoEscala(origemDestinoEscalaSalvar, function () {
        executarReST("GerarEscala/FinalizarEscala", { Codigo: _gerarEscala.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Escala finalizada com sucesso.");
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

function limparCamposVeiculoEscala() {
    _origemDestinoEscala.EmEdicao.val(false);
    _origemDestinoEscala.ListaOrigemDestino.removeAll();

    removerDroppableVeiculosEscalados();
    limparCamposVeiculoEscalaVeiculo();
}

function preencherVeiculoEscala(dadosVeiculoEscala) {
    _origemDestinoEscala.EmEdicao.val(isPermitirEditarVeiculoEscala());

    for (var i = 0; i < dadosVeiculoEscala.length; i++)
        adicionarOrigemDestino(dadosVeiculoEscala[i]);
    
    adicionarDroppableVeiculosEscalados();
}

/*
 * Declaração das Funções Privadas
 */

function adicionarDroppableVeiculosEscalados() {
    if (!isPermitirEditarVeiculoEscala())
        return;

    $(".container_grid_veiculos_escalados").droppable({
        drop: veiculoSoltado,
        hoverClass: "ui-state-active backgroundDropHover",
    });
}

function adicionarOrigemDestino(escalaOrigemDestino) {
    var origemDestino = new OrigemDestino(escalaOrigemDestino);

    _origemDestinoEscala.ListaOrigemDestino.push(origemDestino);

    var linhasPorPaginas = 5;
    var ordenacao = { column: 5, dir: orderDir.asc };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: function (registroSelecionado) { editarVeiculoEscaladoClick(registroSelecionado, origemDestino); }, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoEmpresa", visible: false },
        { data: "CodigoModeloVeicularCarga", visible: false },
        { data: "CodigoMotorista", visible: false },
        { data: "CodigoVeiculo", visible: false },
        { data: "HoraCarregamento", title: "Hora", width: "10%", className: "text-align-center" },
        { data: "Veiculo", title: "Veículo", width: "14%", className: "text-align-center" },
        { data: "ModeloVeicularCarga", title: "Modelo", width: "16%" },
        { data: "Quantidade", title: "Capacidade", width: "16%", className: "text-align-right" },
        { data: "Empresa", title: "Empresa", width: "16%" },
        { data: "Motorista", title: "Motorista", width: "16%" }
    ];

    origemDestino.VeiculosEscalados.grid = new BasicDataTable(origemDestino.VeiculosEscalados.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    origemDestino.VeiculosEscalados.grid.CarregarGrid(origemDestino.VeiculosEscalados.val().slice(), isPermitirEditarVeiculoEscala());
}

function atualizarOrigemDestino(origemDestino) {
    origemDestino.VeiculosEscalados.grid.CarregarGrid(origemDestino.VeiculosEscalados.val().slice(), isPermitirEditarVeiculoEscala());
}

function callbackEmpresaSelecionada(registroSelecionado) {
    if (registroSelecionado.AptaEmissao) {
        if (registroSelecionado.CertificadoVencido || registroSelecionado.EmissaoDocumentosForaDoSistema) {
            _cadastroVeiculoEscalado.Empresa.val(registroSelecionado.Descricao);
            _cadastroVeiculoEscalado.Empresa.codEntity(registroSelecionado.Codigo);
            Global.setarFocoProximoCampo(_cadastroVeiculoEscalado.Empresa.id);
        }
        else
            exibirMensagem(tipoMensagem.atencao, "Atenção", "O certificado para emissão de documentos do Transportador está vencido.", 16000);
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Não é possivel selecionar um transportador que não está apto a emitir pelo Multi Embarcador.", 16000);
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Não é possivel selecionar um Empresa/Filial que não está apta a emitir pelo MultiTMS.", 16000);
}

function callbackVeiculoSelecionado(registroSelecionado) {
    _cadastroVeiculoEscalado.Veiculo.codEntity(registroSelecionado.Codigo);
    _cadastroVeiculoEscalado.Veiculo.entityDescription(registroSelecionado.Descricao);
    _cadastroVeiculoEscalado.Veiculo.val(registroSelecionado.Descricao);

    _cadastroVeiculoEscalado.ModeloVeicularCarga.codEntity(registroSelecionado.CodigoModeloVeicularCarga);
    _cadastroVeiculoEscalado.ModeloVeicularCarga.entityDescription(registroSelecionado.ModeloVeicularCarga);
    _cadastroVeiculoEscalado.ModeloVeicularCarga.val(registroSelecionado.ModeloVeicularCarga);

    _cadastroVeiculoEscalado.Quantidade.val(registroSelecionado.CapacidadePesoTransporte);
}

function exibirModalCadastroVeiculoEscalado() {
    Global.abrirModal('divModalCadastroVeiculoEscalado');
    $("#divModalCadastroVeiculoEscalado").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroVeiculoEscalado);

        _cadastroVeiculoEscalado.Empresa.enable(true);
        _cadastroVeiculoEscalado.Veiculo.enable(true);

        _CRUDCadastroVeiculoEscalado.Adicionar.visible(false);
        _CRUDCadastroVeiculoEscalado.Atualizar.visible(false);
        _CRUDCadastroVeiculoEscalado.Excluir.visible(false);
    });
}

function fecharModalCadastroVeiculoEscalado() {
    Global.fecharModal('divModalCadastroVeiculoEscalado');
}

function isVeiculoEscaladoDuplicado(origemDestino, veiculoEscaladoSalvar) {
    for (var i = 0; i < origemDestino.VeiculosEscalados.val().length; i++) {
        var veiculoEscalado = origemDestino.VeiculosEscalados.val()[i];

        if (veiculoEscalado.Codigo == veiculoEscaladoSalvar.Codigo)
            continue;

        if (veiculoEscalado.CodigoVeiculo == veiculoEscaladoSalvar.CodigoVeiculo) {
            exibirMensagem(tipoMensagem.atencao, "Registro Duplicado", "O veículo " + veiculoEscaladoSalvar.Veiculo + " já está escalado para a expedição");
            return true;
        }

        if (veiculoEscalado.HoraCarregamento == _cadastroVeiculoEscalado) {
            exibirMensagem(tipoMensagem.atencao, "Registro Duplicado", "Já existe um veículo escalado para a expedição na hora informada");
            return true;
        }
    }

    return false;
}

function obterCadastroVeiculoEscaladoSalvar() {
    return {
        Codigo: _cadastroVeiculoEscalado.CodigoVeiculoEscalado.val(),
        CodigoEmpresa: _cadastroVeiculoEscalado.Empresa.codEntity(),
        CodigoModeloVeicularCarga: _cadastroVeiculoEscalado.ModeloVeicularCarga.codEntity(),
        CodigoMotorista: _cadastroVeiculoEscalado.Motorista.codEntity(),
        CodigoVeiculo: _cadastroVeiculoEscalado.Veiculo.codEntity(),
        Empresa: _cadastroVeiculoEscalado.Empresa.val(),
        ModeloVeicularCarga: _cadastroVeiculoEscalado.ModeloVeicularCarga.val(),
        Motorista: _cadastroVeiculoEscalado.Motorista.val(),
        HoraCarregamento: _cadastroVeiculoEscalado.HoraCarregamento.val(),
        Quantidade: _cadastroVeiculoEscalado.Quantidade.val(),
        Veiculo: _cadastroVeiculoEscalado.Veiculo.val()
    };
}

function obterListaOrigemDestino() {
    return _origemDestinoEscala.ListaOrigemDestino().slice();
}

function obterListaOrigemDestinoSalvar(isFinalizarEscala) {
    var listaOrigemDestino = obterListaOrigemDestino();
    var listaOrigemDestinoSalvar = new Array();

    for (var i = 0; i < listaOrigemDestino.length; i++) {
        var origemDestino = listaOrigemDestino[i];
        var veiculosEscalados = origemDestino.VeiculosEscalados.val();
        var quantidadeTotalVeiculosEscalados = 0;

        if (isFinalizarEscala && (veiculosEscalados.length == 0))
            throw new Error("A expedição " + origemDestino.DescricaoOrigemDestino() + " do produto " + origemDestino.Produto.val() + " não possui nenhum veículo informado");

        for (var j = 0; j < veiculosEscalados.length; j++) {
            var veiculoEscalado = veiculosEscalados[j];
            var quantidade = Globalize.parseFloat(veiculoEscalado.Quantidade);

            if (isFinalizarEscala && (quantidade == 0))
                throw new Error("O veículo " + veiculoEscalado.Veiculo + " da expedição " + origemDestino.DescricaoOrigemDestino() + " do produto " + origemDestino.Produto.val() + " não possui uma capacidade de carregamento maior do que zero");

            quantidadeTotalVeiculosEscalados += quantidade;
        }

        if (isFinalizarEscala && (Globalize.parseFloat(origemDestino.Quantidade.val()) > quantidadeTotalVeiculosEscalados))
            throw new Error("A quantidade total dos veículos (" + Globalize.format(quantidadeTotalVeiculosEscalados, "n2") + ") da expedição " + origemDestino.DescricaoOrigemDestino() + " do produto " + origemDestino.Produto.val() + " é inferior a " + origemDestino.Quantidade.val());

        listaOrigemDestinoSalvar.push({
            Codigo: origemDestino.Codigo.val(),
            VeiculosEscalados: veiculosEscalados
        });
    }

    return JSON.stringify(listaOrigemDestinoSalvar);
}

function obterOrigemDestinoEscalaSalvar(isFinalizarEscala) {
    try {
        return {
            Codigo: _gerarEscala.Codigo.val(),
            OrigensDestinosEscala: obterListaOrigemDestinoSalvar(isFinalizarEscala)
        };
    }
    catch (excecao) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", excecao.message);
        return undefined;
    }
}

function obterOrigemDestinoPorCodigo(codigo) {
    var listaOrigemDestino = obterListaOrigemDestino();

    for (var i = 0; i < listaOrigemDestino.length; i++) {
        var origemDestino = listaOrigemDestino[i];

        if (origemDestino.Codigo.val() == codigo)
            return origemDestino;
    }

    return undefined;
}

function obterOrigemDestinoPorIdGridVeiculosEscalados(idGridVeiculosEscalados) {
    var listaOrigemDestino = obterListaOrigemDestino();

    for (var i = 0; i < listaOrigemDestino.length; i++) {
        var origemDestino = listaOrigemDestino[i];

        if (origemDestino.VeiculosEscalados.idGrid == idGridVeiculosEscalados)
            return origemDestino;
    }

    return undefined;
}

function preencherCadastroVeiculoEscalado(registroSelecionado, origemDestino) {
    _cadastroVeiculoEscalado.Codigo.val(origemDestino.Codigo.val());
    _cadastroVeiculoEscalado.CodigoVeiculoEscalado.val(registroSelecionado.Codigo);
    _cadastroVeiculoEscalado.HoraCarregamento.val(registroSelecionado.HoraCarregamento);
    _cadastroVeiculoEscalado.Quantidade.val(registroSelecionado.Quantidade);

    _cadastroVeiculoEscalado.Empresa.codEntity(registroSelecionado.CodigoEmpresa);
    _cadastroVeiculoEscalado.Empresa.entityDescription(registroSelecionado.Empresa);
    _cadastroVeiculoEscalado.Empresa.val(registroSelecionado.Empresa);

    _cadastroVeiculoEscalado.ModeloVeicularCarga.codEntity(registroSelecionado.CodigoModeloVeicularCarga);
    _cadastroVeiculoEscalado.ModeloVeicularCarga.entityDescription(registroSelecionado.ModeloVeicularCarga);
    _cadastroVeiculoEscalado.ModeloVeicularCarga.val(registroSelecionado.ModeloVeicularCarga);

    _cadastroVeiculoEscalado.Motorista.codEntity(registroSelecionado.CodigoMotorista);
    _cadastroVeiculoEscalado.Motorista.entityDescription(registroSelecionado.Motorista);
    _cadastroVeiculoEscalado.Motorista.val(registroSelecionado.Motorista);

    _cadastroVeiculoEscalado.Veiculo.codEntity(registroSelecionado.CodigoVeiculo);
    _cadastroVeiculoEscalado.Veiculo.entityDescription(registroSelecionado.Veiculo);
    _cadastroVeiculoEscalado.Veiculo.val(registroSelecionado.Veiculo);
}

function veiculoSoltado(event, ui) {
    var idGridVeiculosEscalados = event.target.id.replace("_container", "");
    var origemDestino = obterOrigemDestinoPorIdGridVeiculosEscalados(idGridVeiculosEscalados);

    if (!origemDestino)
        return;

    var codigoVeiculo = ui.draggable[0].id;
    var veiculo = obterVeiculoPorCodigo(codigoVeiculo);

    if (!veiculo)
        return;

    var veiculoEscaladoSalvar = $.extend({}, veiculo, { Codigo: guid(), HoraCarregamento: "" });

    if (isVeiculoEscaladoDuplicado(origemDestino, veiculoEscaladoSalvar))
        return;

    preencherCadastroVeiculoEscalado(veiculoEscaladoSalvar, origemDestino);

    _cadastroVeiculoEscalado.Empresa.enable(false);
    _cadastroVeiculoEscalado.Veiculo.enable(false);
    _CRUDCadastroVeiculoEscalado.Adicionar.visible(true);

    exibirModalCadastroVeiculoEscalado();
}

function removerDroppableVeiculosEscalados() {
    $(".container_grid_veiculos_escalados").droppable("destroy");
}

function salvarVeiculoEscala(origemDestinoEscalaSalvar, callbackSucesso) {
    if (!origemDestinoEscalaSalvar)
        return;

    executarReST("GerarEscala/AtualizarVeiculoEscala", origemDestinoEscalaSalvar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Veículos da escala atualizados com sucesso.");
                ocultarMensagemDadosNaoSalvos();
                limparCamposVeiculoEscala();
                preencherVeiculoEscala(retorno.Data);
                preencherVeiculoEscalaVeiculo();

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
