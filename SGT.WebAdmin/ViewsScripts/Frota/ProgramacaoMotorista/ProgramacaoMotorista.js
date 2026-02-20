/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/ProgramacaoAlocacao.js" />
/// <reference path="../../Consultas/ProgramacaoEspecialidade.js" />
/// <reference path="../../Consultas/ProgramacaoSituacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoVeiculo.js" />
/// <reference path="../../Cargas/Carga/DadosCarga/Carga.js" />
/// <reference path="../../Enumeradores/EnumTipoEntidadeProgramacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridProgramacaoMotorista;
var _pesquisaProgramacaoMotorista;
var _programacaoMotorista;
var _programaMotorista;
var _gridReboques;

var _timeOutPainel;

var _paginasPrevistas = 1;
var _paginaAtual = 1;
var _paginou = false;
var _tempoParaTroca = 30000;
//var _tempoParaTroca = 9000; arqui é para debugar
var _pesquisouNovamente = false;
var _itensPorPagina = 20;
var _naoPaginar = false;
var _habilitarPainel = false;

var PesquisaProgramacaoMotorista = function () {
    this.Inicio = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });
    this.Limite = PropertyEntity({ val: ko.observable(_itensPorPagina), def: _itensPorPagina, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });

    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ProgramacaoEspecialidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Especialidade:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ProgramacaoSituacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Situação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ProgramacaoAlocacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Alocação:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.HabilitarPainel = PropertyEntity({ getType: typesKnockout.bool, eventChange: HabilitarPainelOnChange, val: ko.observable(false), def: false, text: "Habilitar visualização em formato de painel?", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _pesquisouNovamente = true;
            _pesquisaProgramacaoMotorista.Inicio.val(0);
            _pesquisaProgramacaoMotorista.Limite.val(_itensPorPagina);
            _gridProgramacaoMotorista.CarregarGrid(buscarSumarizadores);
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false)
    });
}

var ProgramacaoMotorista = function () {
    this.ProgramacaoMotorista = PropertyEntity({ type: types.event, text: "Painel", idGrid: guid(), visible: ko.observable(true) });
    this.Status = ko.observableArray();
}

var ProgramaMotorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });

    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motorista:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(false), required: true });
    this.ProgramacaoAlocacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Alocação:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: true });
    this.ProgramacaoEspecialidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Especialidade:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: true });
    this.ProgramacaoSituacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Situação:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: true });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: false });

    this.CategoriaCNH = PropertyEntity({ text: "Cat. CNH: ", getType: typesKnockout.string, enable: ko.observable(false), required: ko.observable(false) });
    this.VencimentoCNH = PropertyEntity({ text: "Vencimento CNH: ", getType: typesKnockout.date, enable: ko.observable(false), required: ko.observable(false) });
    this.VencimentoMoop = PropertyEntity({ text: "Vencimento Moop: ", getType: typesKnockout.date, enable: ko.observable(false), required: ko.observable(false) });
    this.DataAdmissao = PropertyEntity({ text: "Admissão: ", getType: typesKnockout.dateTime, enable: ko.observable(false), required: ko.observable(false) });
    this.DataInicioFerias = PropertyEntity({ text: "Inicio de Férias: ", getType: typesKnockout.date, enable: ko.observable(true), required: ko.observable(false) });
    this.DataFimFerias = PropertyEntity({ text: "Fim de Férias: ", getType: typesKnockout.date, enable: ko.observable(true), required: ko.observable(false) });

    this.Salvar = PropertyEntity({ eventClick: SalvarClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
}

//*******EVENTOS*******
function loadProgramacaoMotorista() {
    //-- Knouckout
    // Instancia pesquisa
    LimparTimeOutPainel();
    _pesquisaProgramacaoMotorista = new PesquisaProgramacaoMotorista();
    KoBindings(_pesquisaProgramacaoMotorista, "knockoutPesquisaProgramacaoMotorista", false, _pesquisaProgramacaoMotorista.Pesquisar.id);

    _programacaoMotorista = new ProgramacaoMotorista();
    KoBindings(_programacaoMotorista, "knockoutProgramacaoMotorista", false);

    _programaMotorista = new ProgramaMotorista();
    KoBindings(_programaMotorista, "knoutProgramacaoMotorista");

    new BuscarMotoristas(_pesquisaProgramacaoMotorista.Motorista);
    new BuscarProgramacaoEspecialidade(_pesquisaProgramacaoMotorista.ProgramacaoEspecialidade, null, [EnumTipoEntidadeProgramacao.Todos, EnumTipoEntidadeProgramacao.Motorista]);
    new BuscarProgramacaoSituacao(_pesquisaProgramacaoMotorista.ProgramacaoSituacao, null, [EnumTipoEntidadeProgramacao.Todos, EnumTipoEntidadeProgramacao.Motorista]);
    new BuscarProgramacaoAlocacao(_pesquisaProgramacaoMotorista.ProgramacaoAlocacao, null, [EnumTipoEntidadeProgramacao.Todos, EnumTipoEntidadeProgramacao.Motorista]);

    new BuscarMotoristas(_programaMotorista.Motorista);
    new BuscarProgramacaoEspecialidade(_programaMotorista.ProgramacaoEspecialidade, null, [EnumTipoEntidadeProgramacao.Todos, EnumTipoEntidadeProgramacao.Motorista]);
    new BuscarProgramacaoSituacao(_programaMotorista.ProgramacaoSituacao, null, [EnumTipoEntidadeProgramacao.Todos, EnumTipoEntidadeProgramacao.Motorista]);
    new BuscarProgramacaoAlocacao(_programaMotorista.ProgramacaoAlocacao, null, [EnumTipoEntidadeProgramacao.Todos, EnumTipoEntidadeProgramacao.Motorista]);
    new BuscarClientes(_programaMotorista.Cliente);

    // Inicia busca
    buscarProgramacaoMotorista();

    $(window).one('hashchange', function () {
        LimparTimeOutPainel();
    });

    $('#divModalProgramacaoMotorista').on('hidden.bs.modal', function () {
        _naoPaginar = false;
    });
}

function HabilitarPainelOnChange(e, sender) {
    _pesquisouNovamente = true;
    _pesquisaProgramacaoMotorista.Inicio.val(0);
    _pesquisaProgramacaoMotorista.Limite.val(_itensPorPagina);
    if ($('#' + e.HabilitarPainel.id).is(':checked')) {
        _pesquisaProgramacaoMotorista.HabilitarPainel.val(true);
        _habilitarPainel = true;
        _paginaAtual = 1;
        carregarProgramacaoMotorista(_paginaAtual, _paginou, executarPesquisaTimeOut);
    } else {

        $('#knockoutProgramacaoMotorista').removeClass('ocultarPaginacao');
        _habilitarPainel = false;
        _pesquisaProgramacaoMotorista.HabilitarPainel.val(false);
        _gridProgramacaoMotorista.CarregarGrid(buscarSumarizadores);
    }
}

function RetornoBuscarOrdemServico(data) {
    _programaMotorista.OrdemServicoFrota.val(data.Numero);
    _programaMotorista.OrdemServicoFrota.codEntity(data.Codigo);
}

//*******MÉTODOS*******
function buscarProgramacaoMotorista() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarProgramacaoMotoristaClick, tamanho: "15", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [editar],
        tamanho: "5",
        descricao: "Opções"
    };

    // Inicia Grid de busca
    _gridProgramacaoMotorista = new GridView(_programacaoMotorista.ProgramacaoMotorista.idGrid, "ProgramacaoMotorista/Pesquisa", _pesquisaProgramacaoMotorista, menuOpcoes, null, _itensPorPagina);
    _pesquisaProgramacaoMotorista.Inicio.val(0);
    _pesquisaProgramacaoMotorista.Limite.val(_itensPorPagina);
    _gridProgramacaoMotorista.CarregarGrid(buscarSumarizadores);
}

function SalvarClick(e, sender) {
    Salvar(_programaMotorista, "ProgramacaoMotorista/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado");
                Global.fecharModal("divModalProgramacaoMotorista");


                _naoPaginar = false;
                executarPesquisaTimeOut();
                limparCampos();
                _gridProgramacaoMotorista.CarregarGrid(buscarSumarizadores);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, null);
}

function editarProgramacaoMotoristaClick(e) {
    limparCampos();
    // Seta o codigo do objeto
    _programaMotorista.Codigo.val(e.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_programaMotorista, "ProgramacaoMotorista/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _naoPaginar = true;

                Global.abrirModal('divModalProgramacaoMotorista');
                _programaMotorista.ProgramacaoAlocacao.get$().focus();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function buscarSumarizadores() {
    limparSumarizadores();

    executarReST("ProgramacaoMotorista/BuscarSumarizadores", {
        Motorista: _pesquisaProgramacaoMotorista.Motorista.codEntity(),
        ProgramacaoEspecialidade: _pesquisaProgramacaoMotorista.ProgramacaoEspecialidade.codEntity(),
        ProgramacaoSituacao: _pesquisaProgramacaoMotorista.ProgramacaoSituacao.codEntity(),
        ProgramacaoAlocacao: _pesquisaProgramacaoMotorista.ProgramacaoAlocacao.codEntity()
    }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _programacaoMotorista.Status(retorno.Data.Status.slice());
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function limparSumarizadores() {
    //_programacaoMotorista.Status.removeAll();
}

function limparCampos() {
    LimparCampos(_programaMotorista);
}

function LimparTimeOutPainel() {
    if (_timeOutPainel !== null)
        clearTimeout(_timeOutPainel);
}

function validarPaginacao() {
    if (_paginaAtual >= _paginasPrevistas) {
        _paginaAtual = 1;
        _paginou = false;
    } else {
        _paginaAtual++;
        _paginou = true;
    }
}

function executarPesquisaTimeOut(retornoData) {
    if (!_naoPaginar && _habilitarPainel) {

        if (retornoData !== null && retornoData !== undefined)
            _paginasPrevistas = retornoData.recordsTotal / (_itensPorPagina);

        validarPaginacao();
        _timeOutPainel = setTimeout(function () {
            if (_habilitarPainel) {
                _pesquisouNovamente = false;
                carregarProgramacaoMotorista(_paginaAtual, _paginou, executarPesquisaTimeOut);
            }
        }, _tempoParaTroca);
    }
}

function carregarProgramacaoMotorista(page, paginou, callback) {

    if (_pesquisouNovamente) {
        page = 1;
        paginou = false;
    }

    _pesquisaProgramacaoMotorista.Inicio.val((_itensPorPagina) * (page - 1));
    _pesquisaProgramacaoMotorista.Limite.val(_itensPorPagina);
    _gridProgramacaoMotorista.CarregarGrid(callback);
    buscarSumarizadores();

    if (_habilitarPainel) {
        $('#knockoutProgramacaoMotorista').addClass('ocultarPaginacao');
    }
}