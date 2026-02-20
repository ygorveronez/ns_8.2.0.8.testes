/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="GerarEscala.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroModeloRestricaoRodagem;
var _CRUDcadastroModeloRestricaoRodagem;
var _gridModeloRestricaoRodagem;
var _modeloRestricaoRodagem;

/*
 * Declaração das Classes
 */

var CRUDCadastroModeloRestricaoRodagem = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarModeloRestricaoRodagemClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarModeloRestricaoRodagemClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirModeloRestricaoRodagemClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var CadastroModeloRestricaoRodagem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo Veícular: ", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.HoraInicioRestricao = PropertyEntity({ text: "*Inicio Restrição: ", getType: typesKnockout.time, required: true });
    this.HoraFimRestricao = PropertyEntity({ text: "*Fim Restrição: ", getType: typesKnockout.time, required: true });

    this.HoraInicioRestricao.dateRangeLimit = this.HoraFimRestricao;
    this.HoraFimRestricao.dateRangeInit = this.HoraInicioRestricao;
}

var ModeloRestricaoRodagem = function () {
    this.ListaModeloRestricaoRodagem = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarModeloRestricaoRodagemModalClick, type: types.event, text: "Adicionar Restrição", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridModeloRestricaoRodagem() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 4, dir: orderDir.asc };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarModeloRestricaoRodagemClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoEditar] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoModeloVeicularCarga", visible: false },
        { data: "HoraInicioRestricao", visible: false },
        { data: "HoraFimRestricao", visible: false },
        { data: "DescricaoModeloVeicularCarga", title: "Modelo Veicular", width: "50%" },
        { data: "HorarioRestricao", title: "Horario de Restrição", width: "30%", className: "text-align-center" }
    ];

    _gridModeloRestricaoRodagem = new BasicDataTable(_modeloRestricaoRodagem.ListaModeloRestricaoRodagem.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridModeloRestricaoRodagem.CarregarGrid([]);
}

function loadModeloRestricaoRodagem() {
    _modeloRestricaoRodagem = new ModeloRestricaoRodagem();
    KoBindings(_modeloRestricaoRodagem, "knockoutModeloRestricaoRodagem");

    _cadastroModeloRestricaoRodagem = new CadastroModeloRestricaoRodagem();
    KoBindings(_cadastroModeloRestricaoRodagem, "knockoutCadastroModeloRestricaoRodagem");

    _CRUDcadastroModeloRestricaoRodagem = new CRUDCadastroModeloRestricaoRodagem();
    KoBindings(_CRUDcadastroModeloRestricaoRodagem, "knockoutCRUDCadastroModeloRestricaoRodagem");

    new BuscarModelosVeicularesCarga(_cadastroModeloRestricaoRodagem.ModeloVeicularCarga);

    loadGridModeloRestricaoRodagem();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarModeloRestricaoRodagemClick() {
    if (!ValidarCamposObrigatorios(_cadastroModeloRestricaoRodagem)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    if (isModeloRestricaoRodagemDuplicado()) {
        exibirMensagem(tipoMensagem.atencao, "Registro Duplicado", "O modelo veicular informado já está cadastrado!");
        return;
    }

    _modeloRestricaoRodagem.ListaModeloRestricaoRodagem.val().push(obterCadastroModeloRestricaoRodagemSalvar());

    recarregarGridModeloRestricaoRodagem();
    fecharModalCadastroModeloRestricaoRodagem();
}

function adicionarModeloRestricaoRodagemModalClick() {
    _cadastroModeloRestricaoRodagem.Codigo.val(guid());
    _cadastroModeloRestricaoRodagem.ModeloVeicularCarga.enable(true);

    controlarBotoesCadastroModeloRestricaoRodagemHabilitados(false);

    exibirModalCadastroModeloRestricaoRodagem();
}

function atualizarModeloRestricaoRodagemClick() {
    if (!ValidarCamposObrigatorios(_cadastroModeloRestricaoRodagem)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    if (isModeloRestricaoRodagemDuplicado()) {
        exibirMensagem(tipoMensagem.atencao, "Registro Duplicado", "O modelo veicular informado já está cadastrado!");
        return;
    }

    var listaModeloRestricaoRodagem = obterListaModeloRestricaoRodagem();

    for (var i = 0; i < listaModeloRestricaoRodagem.length; i++) {
        if (_cadastroModeloRestricaoRodagem.Codigo.val() == listaModeloRestricaoRodagem[i].Codigo) {
            listaModeloRestricaoRodagem.splice(i, 1, obterCadastroModeloRestricaoRodagemSalvar());
            break;
        }
    }

    _modeloRestricaoRodagem.ListaModeloRestricaoRodagem.val(listaModeloRestricaoRodagem);

    recarregarGridModeloRestricaoRodagem();
    fecharModalCadastroModeloRestricaoRodagem();
}

function editarModeloRestricaoRodagemClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroModeloRestricaoRodagem, { Data: registroSelecionado });

    _cadastroModeloRestricaoRodagem.ModeloVeicularCarga.codEntity(registroSelecionado.CodigoModeloVeicularCarga);
    _cadastroModeloRestricaoRodagem.ModeloVeicularCarga.entityDescription(registroSelecionado.DescricaoModeloVeicularCarga);
    _cadastroModeloRestricaoRodagem.ModeloVeicularCarga.val(registroSelecionado.DescricaoModeloVeicularCarga);
    _cadastroModeloRestricaoRodagem.ModeloVeicularCarga.enable(false);

    controlarBotoesCadastroModeloRestricaoRodagemHabilitados(true);
    exibirModalCadastroModeloRestricaoRodagem();
}

function excluirModeloRestricaoRodagemClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a restrição de rodagem?", function () {
        removerModeloRestricaoRodagem(_cadastroModeloRestricaoRodagem.Codigo.val());
        fecharModalCadastroModeloRestricaoRodagem();
    });
}

/*
 * Declaração das Funções Públicas
 */

function isPossuiRestricaoRodagem(origemDestino, veiculoEscalado) {
    var listaModeloRestricaoRodagem = obterListaModeloRestricaoRodagem();
    
    for (var i = 0; i < listaModeloRestricaoRodagem.length; i++) {
        var modeloRestricaoRodagem = listaModeloRestricaoRodagem[i];

        if (modeloRestricaoRodagem.CodigoModeloVeicularCarga == veiculoEscalado.CodigoModeloVeicularCarga) {
            var dataAtual = Global.DataAtual();
            var inicioRestricao = moment(dataAtual + " " + modeloRestricaoRodagem.HoraInicioRestricao, "DD/MM/YYYY HH:mm");
            var terminoRestricao = moment(dataAtual + " " + modeloRestricaoRodagem.HoraFimRestricao, "DD/MM/YYYY HH:mm");
            var horaCarregamento = moment(dataAtual + " " + veiculoEscalado.HoraCarregamento, "DD/MM/YYYY HH:mm");

            if ((horaCarregamento.diff(inicioRestricao) >= 0) && (horaCarregamento.diff(terminoRestricao) < 0)) {
                exibirMensagem(tipoMensagem.atencao, "Restrição de Rodagem", "O modelo do veículo selecionado possui restrição de rodagem das " + modeloRestricaoRodagem.HorarioRestricao);
                return true;
            }
        }
    }

    return false;
}

function limparCamposModeloRestricaoRodagem() {
    preencherModeloRestricaoRodagem([]);
}

function obterModeloRestricaoRodagemSalvar() {
    var listaModeloRestricaoRodagem = obterListaModeloRestricaoRodagem();
    var listaModeloRestricaoRodagemSalvar = new Array();

    for (var i = 0; i < listaModeloRestricaoRodagem.length; i++) {
        var modeloRestricaoRodagem = listaModeloRestricaoRodagem[i];

        listaModeloRestricaoRodagemSalvar.push({
            Codigo: modeloRestricaoRodagem.Codigo,
            CodigoModeloVeicularCarga: modeloRestricaoRodagem.CodigoModeloVeicularCarga,
            HoraInicioRestricao: modeloRestricaoRodagem.HoraInicioRestricao,
            HoraFimRestricao: modeloRestricaoRodagem.HoraFimRestricao
        });
    }

    return JSON.stringify(listaModeloRestricaoRodagemSalvar);
}

function preencherModeloRestricaoRodagem(dadosModeloRestricaoRodagem) {
    _modeloRestricaoRodagem.Adicionar.visible(isPermitirEditarDadosEscala());
    _modeloRestricaoRodagem.ListaModeloRestricaoRodagem.val(dadosModeloRestricaoRodagem);

    recarregarGridModeloRestricaoRodagem();
}

/*
 * Declaração das Funções Privadas
 */

function controlarBotoesCadastroModeloRestricaoRodagemHabilitados(isEdicao) {
    _CRUDcadastroModeloRestricaoRodagem.Adicionar.visible(!isEdicao);
    _CRUDcadastroModeloRestricaoRodagem.Atualizar.visible(isEdicao);
    _CRUDcadastroModeloRestricaoRodagem.Excluir.visible(isEdicao);
}

function exibirModalCadastroModeloRestricaoRodagem() {
    Global.abrirModal('divModalCadastroModeloRestricaoRodagem');
    $("#divModalCadastroModeloRestricaoRodagem").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroModeloRestricaoRodagem);
    });
}

function fecharModalCadastroModeloRestricaoRodagem() {
    Global.fecharModal('divModalCadastroModeloRestricaoRodagem');
}

function isModeloRestricaoRodagemDuplicado() {
    var listaModeloRestricaoRodagem = obterListaModeloRestricaoRodagem();

    for (var i = 0; i < listaModeloRestricaoRodagem.length; i++) {
        var modeloRestricaoRodagem = listaModeloRestricaoRodagem[i];

        if ((_cadastroModeloRestricaoRodagem.Codigo.val() != modeloRestricaoRodagem.Codigo) && (_cadastroModeloRestricaoRodagem.ModeloVeicularCarga.codEntity() == modeloRestricaoRodagem.CodigoModeloVeicularCarga))
            return true;
    }

    return false;
}

function obterCadastroModeloRestricaoRodagemSalvar() {
    return {
        Codigo: _cadastroModeloRestricaoRodagem.Codigo.val(),
        CodigoModeloVeicularCarga: _cadastroModeloRestricaoRodagem.ModeloVeicularCarga.codEntity(),
        HoraInicioRestricao: _cadastroModeloRestricaoRodagem.HoraInicioRestricao.val(),
        HoraFimRestricao: _cadastroModeloRestricaoRodagem.HoraFimRestricao.val(),
        DescricaoModeloVeicularCarga: _cadastroModeloRestricaoRodagem.ModeloVeicularCarga.val(),
        HorarioRestricao: _cadastroModeloRestricaoRodagem.HoraInicioRestricao.val() + " até " + _cadastroModeloRestricaoRodagem.HoraFimRestricao.val()
    };
}

function obterListaModeloRestricaoRodagem() {
    return _modeloRestricaoRodagem.ListaModeloRestricaoRodagem.val().slice();
}

function recarregarGridModeloRestricaoRodagem() {
    var listaModeloRestricaoRodagem = obterListaModeloRestricaoRodagem();
    var permitirEditaDadosEscala = isPermitirEditarDadosEscala();

    _gridModeloRestricaoRodagem.CarregarGrid(listaModeloRestricaoRodagem, permitirEditaDadosEscala);
}

function removerModeloRestricaoRodagem(codigo) {
    var listaModeloRestricaoRodagem = obterListaModeloRestricaoRodagem();

    for (var i = 0; i < listaModeloRestricaoRodagem.length; i++) {
        if (codigo == listaModeloRestricaoRodagem[i].Codigo) {
            listaModeloRestricaoRodagem.splice(i, 1);
            break;
        }
    }

    _modeloRestricaoRodagem.ListaModeloRestricaoRodagem.val(listaModeloRestricaoRodagem);

    recarregarGridModeloRestricaoRodagem();
}
