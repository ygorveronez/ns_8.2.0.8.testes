/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
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
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/ProgramacaoAlocacao.js" />
/// <reference path="../../Consultas/ProgramacaoEspecialidade.js" />
/// <reference path="../../Consultas/ProgramacaoSituacao.js" />
/// <reference path="../../Consultas/ProgramacaoLicenciamento.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumSituacaoVeiculo.js" />
/// <reference path="../../Cargas/Carga/DadosCarga/Carga.js" />
/// <reference path="../../Enumeradores/EnumTipoEntidadeProgramacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridProgramacaoVeiculo;
var _pesquisaProgramacaoVeiculo;
var _programacaoVeiculo;
var _programaMotorista;
var _gridReboques;
var _PermissoesPersonalizadas;

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

var PesquisaProgramacaoVeiculo = function () {
    this.Inicio = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });
    this.Limite = PropertyEntity({ val: ko.observable(_itensPorPagina), def: _itensPorPagina, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });

    this.NumeroFrota = PropertyEntity({ text: "Nº Frota: ", visible: ko.observable(true), maxlength: 30, val: ko.observable("") });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Reboque:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ProgramacaoSituacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Situação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ProgramacaoLicenciamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Licenciamento:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ProgramacaoAlocacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Alocação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ProgramacaoEspecialidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Especialidade:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.HabilitarPainel = PropertyEntity({ getType: typesKnockout.bool, eventChange: HabilitarPainelOnChange, val: ko.observable(false), def: false, text: "Habilitar visualização em formato de painel?", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _pesquisouNovamente = true;
            _pesquisaProgramacaoVeiculo.Inicio.val(0);
            _pesquisaProgramacaoVeiculo.Limite.val(_itensPorPagina);
            _gridProgramacaoVeiculo.CarregarGrid(buscarSumarizadores);
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
};

var ProgramacaoVeiculo = function () {
    this.ProgramacaoVeiculo = PropertyEntity({ type: types.event, text: "Painel", idGrid: guid(), visible: ko.observable(true) });
    this.Status = ko.observableArray();
    this.Adicionar = PropertyEntity({ eventClick: AdicionarVeiculoClick, type: types.event, text: "Adicionar Veículo", visible: ko.observable(true) });
};

var ProgramaMotorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });

    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Veículo:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: true });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: false });
    this.ProgramacaoLicenciamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Licenciamento:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: false });
    this.ProgramacaoAlocacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Alocação:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: true });
    this.ProgramacaoEspecialidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Especialidade:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: true });
    this.Pallets = PropertyEntity({ getType: typesKnockout.int, maxlength: 5, text: "Nº Pallets:", configInt: { precision: 0, allowZero: false, thousands: "" }, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.ProgramacaoSituacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Situação:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: true });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: false });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: false });

    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: ko.observable(false) });
    this.DataTermino = PropertyEntity({ text: "Data Término: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });

    this.Salvar = PropertyEntity({ eventClick: SalvarClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
};

//*******EVENTOS*******
function loadProgramacaoVeiculo() {
    //-- Knouckout
    // Instancia pesquisa
    LimparTimeOutPainel();
    _pesquisaProgramacaoVeiculo = new PesquisaProgramacaoVeiculo();
    KoBindings(_pesquisaProgramacaoVeiculo, "knockoutPesquisaProgramacaoVeiculo", false, _pesquisaProgramacaoVeiculo.Pesquisar.id);

    _programacaoVeiculo = new ProgramacaoVeiculo();
    KoBindings(_programacaoVeiculo, "knockoutProgramacaoVeiculo", false);

    _programaMotorista = new ProgramaMotorista();
    KoBindings(_programaMotorista, "knoutProgramacaoVeiculo");

    if (_CONFIGURACAO_TMS.NaoUtilizarDataTerminoProgramacaoVeiculo)
        _programaMotorista.DataTermino.visible(false);
    else
        _programaMotorista.DataTermino.visible(true);

    new BuscarVeiculos(_pesquisaProgramacaoVeiculo.Veiculo, null, null, null, null, null, null, null, null, null, null, "0");
    new BuscarVeiculos(_pesquisaProgramacaoVeiculo.Reboque, null, null, null, null, null, null, null, null, null, null, "1");
    new BuscarModelosVeicularesCarga(_pesquisaProgramacaoVeiculo.ModeloVeicular);
    new BuscarProgramacaoSituacao(_pesquisaProgramacaoVeiculo.ProgramacaoSituacao, null, [EnumTipoEntidadeProgramacao.Todos, EnumTipoEntidadeProgramacao.Veiculo]);
    new BuscarMotoristas(_pesquisaProgramacaoVeiculo.Motorista);
    new BuscarProgramacaoLicenciamento(_pesquisaProgramacaoVeiculo.ProgramacaoLicenciamento, null);
    new BuscarProgramacaoAlocacao(_pesquisaProgramacaoVeiculo.ProgramacaoAlocacao, null, [EnumTipoEntidadeProgramacao.Todos, EnumTipoEntidadeProgramacao.Veiculo]);
    new BuscarProgramacaoEspecialidade(_pesquisaProgramacaoVeiculo.ProgramacaoEspecialidade, null, [EnumTipoEntidadeProgramacao.Todos, EnumTipoEntidadeProgramacao.Veiculo]);

    new BuscarVeiculos(_programaMotorista.Veiculo, null, null, null, null, null, null, null, null, null, null, "0");
    new BuscarMotoristas(_programaMotorista.Motorista);
    new BuscarProgramacaoLicenciamento(_programaMotorista.ProgramacaoLicenciamento, null);
    new BuscarProgramacaoAlocacao(_programaMotorista.ProgramacaoAlocacao, null, [EnumTipoEntidadeProgramacao.Todos, EnumTipoEntidadeProgramacao.Veiculo]);
    new BuscarProgramacaoEspecialidade(_programaMotorista.ProgramacaoEspecialidade, null, [EnumTipoEntidadeProgramacao.Todos, EnumTipoEntidadeProgramacao.Veiculo]);
    new BuscarProgramacaoSituacao(_programaMotorista.ProgramacaoSituacao, null, [EnumTipoEntidadeProgramacao.Todos, EnumTipoEntidadeProgramacao.Veiculo]);
    new BuscarClientes(_programaMotorista.Cliente);
    new BuscarLocalidades(_programaMotorista.Destino);

    // Inicia busca
    buscarProgramacaoVeiculo();

    $(window).one('hashchange', function () {
        LimparTimeOutPainel();
    });

    $('#divModalProgramacaoVeiculo').on('hidden.bs.modal', function () {
        _naoPaginar = false;
    });
}

function AdicionarVeiculoClick(e, sender) {
    limparCampos();
    _naoPaginar = true;

    Global.abrirModal('divModalProgramacaoVeiculo');
}

function HabilitarPainelOnChange(e, sender) {
    _pesquisouNovamente = true;
    _pesquisaProgramacaoVeiculo.Inicio.val(0);
    _pesquisaProgramacaoVeiculo.Limite.val(_itensPorPagina);
    if ($('#' + e.HabilitarPainel.id).is(':checked')) {
        _pesquisaProgramacaoVeiculo.HabilitarPainel.val(true);
        _habilitarPainel = true;
        _paginaAtual = 1;
        carregarProgramacaoVeiculo(_paginaAtual, _paginou, executarPesquisaTimeOut);
    } else {

        $('#knockoutProgramacaoVeiculo').removeClass('ocultarPaginacao');
        _habilitarPainel = false;
        _pesquisaProgramacaoVeiculo.HabilitarPainel.val(false);
        _gridProgramacaoVeiculo.CarregarGrid(buscarSumarizadores);
    }
}

function RetornoBuscarOrdemServico(data) {
    _programaMotorista.OrdemServicoFrota.val(data.Numero);
    _programaMotorista.OrdemServicoFrota.codEntity(data.Codigo);
}

//*******MÉTODOS*******
function buscarProgramacaoVeiculo() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarProgramacaoVeiculoClick, tamanho: "15", icone: "" };
    var auditar = { descricao: "Auditoria", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("ProgramacaoVeiculo", "Codigo"), tamanho: "20", icone: "", visibilidade: VisibilidadeAuditoriaProgramacaoVeiculo };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [editar, auditar],
        tamanho: "5",
        descricao: "Opções"
    };

    var configExportacao = {
        url: "ProgramacaoVeiculo/ExportarPesquisa",
        titulo: "Programações de Veículo"
    };

    // Inicia Grid de busca
    _gridProgramacaoVeiculo = new GridViewExportacao(_programacaoVeiculo.ProgramacaoVeiculo.idGrid, "ProgramacaoVeiculo/Pesquisa", _pesquisaProgramacaoVeiculo, menuOpcoes, configExportacao, null, _itensPorPagina);
    _pesquisaProgramacaoVeiculo.Inicio.val(0);
    _pesquisaProgramacaoVeiculo.Limite.val(_itensPorPagina);
    _gridProgramacaoVeiculo.CarregarGrid(buscarSumarizadores);
}

function SalvarClick(e, sender) {
    Salvar(_programaMotorista, "ProgramacaoVeiculo/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado");
                Global.fecharModal("divModalProgramacaoVeiculo");


                _naoPaginar = false;
                executarPesquisaTimeOut();
                limparCampos();
                _gridProgramacaoVeiculo.CarregarGrid(buscarSumarizadores);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, null);
}

function editarProgramacaoVeiculoClick(e) {
    limparCampos();
    // Seta o codigo do objeto
    _programaMotorista.Codigo.val(e.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_programaMotorista, "ProgramacaoVeiculo/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _naoPaginar = true;

                Global.abrirModal('divModalProgramacaoVeiculo');
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

    executarReST("ProgramacaoVeiculo/BuscarSumarizadores", {
        NumeroFrota: _pesquisaProgramacaoVeiculo.NumeroFrota.val(),
        Veiculo: _pesquisaProgramacaoVeiculo.Veiculo.codEntity(),
        Reboque: _pesquisaProgramacaoVeiculo.Reboque.codEntity(),
        ModeloVeicular: _pesquisaProgramacaoVeiculo.ModeloVeicular.codEntity(),
        ProgramacaoSituacao: _pesquisaProgramacaoVeiculo.ProgramacaoSituacao.codEntity(),
        Motorista: _pesquisaProgramacaoVeiculo.Motorista.codEntity(),
        ProgramacaoLicenciamento: _pesquisaProgramacaoVeiculo.ProgramacaoLicenciamento.codEntity(),
        ProgramacaoAlocacao: _pesquisaProgramacaoVeiculo.ProgramacaoAlocacao.codEntity(),
        ProgramacaoEspecialidade: _pesquisaProgramacaoVeiculo.ProgramacaoEspecialidade.codEntity()
    }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _programacaoVeiculo.Status(retorno.Data.Status.slice());
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function VisibilidadeAuditoriaProgramacaoVeiculo() {
    return VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ProgramacaoVeiculo_PermiteVisualizarAuditoria, _PermissoesPersonalizadas);
}

function limparSumarizadores() {
    //_programacaoVeiculo.Status.removeAll();
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
                carregarProgramacaoVeiculo(_paginaAtual, _paginou, executarPesquisaTimeOut);
            }
        }, _tempoParaTroca);
    }
}

function carregarProgramacaoVeiculo(page, paginou, callback) {

    if (_pesquisouNovamente) {
        page = 1;
        paginou = false;
    }

    _pesquisaProgramacaoVeiculo.Inicio.val((_itensPorPagina) * (page - 1));
    _pesquisaProgramacaoVeiculo.Limite.val(_itensPorPagina);
    _gridProgramacaoVeiculo.CarregarGrid(callback);
    buscarSumarizadores();

    if (_habilitarPainel) {
        $('#knockoutProgramacaoVeiculo').addClass('ocultarPaginacao');
    }
}