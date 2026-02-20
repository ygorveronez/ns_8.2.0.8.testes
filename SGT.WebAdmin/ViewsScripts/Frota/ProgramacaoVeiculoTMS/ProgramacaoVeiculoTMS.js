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
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/ProgramacaoAlocacao.js" />
/// <reference path="../../Consultas/ProgramacaoEspecialidade.js" />
/// <reference path="../../Consultas/ProgramacaoSituacaoTMS.js" />
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

var _timeOutPainel;

var _paginasPrevistas = 1;
var _paginaAtual = 1;
var _paginou = false;
var _tempoParaTroca = 30000;
//var _tempoParaTroca = 9000; arqui é para debugar
var _pesquisouNovamente = false;
var _itensPorPagina = 20;
var _naoPaginar = false;
var _mensagemMotoristaSituacao;

var PesquisaProgramacaoVeiculo = function () {
    this.Inicio = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });
    this.Limite = PropertyEntity({ val: ko.observable(_itensPorPagina), def: _itensPorPagina, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });

    this.NumeroFrota = PropertyEntity({ text: "Nº Frota: ", visible: ko.observable(true), maxlength: 30, val: ko.observable("") });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Reboque:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ProgramacaoSituacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Situação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Estado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Estado:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataCadastroPlanejamentoInicial = PropertyEntity({ text: "Data Cadastro de Planejamento Inicial:", getType: typesKnockout.date });
    this.DataCadastroPlanejamentoFinal = PropertyEntity({ text: "Data Cadastro de Planejamento Final:", getType: typesKnockout.date });
    this.DataDisponibilidadeInicial = PropertyEntity({ text: "Data Disponibilidade Inicial:", getType: typesKnockout.date });
    this.DataDisponibilidadeFinal = PropertyEntity({ text: "Data Disponibilidade Final:", getType: typesKnockout.date });
    this.FuncionarioResponsavelCavalo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário Responsável do Cavalo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.MarcaCavalo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Marca Cavalo:", idBtnSearch: guid(), visible: ko.observable(true) });

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
    this.ProgramacaoSituacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Situação:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: true });
    this.CidadeEstado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cidade/Estado:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: true });

    this.DataDisponivelInicio = PropertyEntity({ text: "Data Disponível Inicio: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: ko.observable(false) });
    this.DataDisponivelFim = PropertyEntity({ text: "Data Disponível Fim: ", getType: typesKnockout.dateTime, enable: ko.observable(true), required: ko.observable(false) });
    this.Folga = PropertyEntity({ text: "Folga: ", getType: typesKnockout.int, enable: ko.observable(0), required: ko.observable(false) });
    this.Observacao = PropertyEntity({ text: "Observação: ", getType: typesKnockout.string, required: ko.observable(false), maxlength: 300 });

    this.Salvar = PropertyEntity({ eventClick: SalvarClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
};

//*******EVENTOS*******
function loadProgramacaoVeiculoTMS() {
    //-- Knouckout
    // Instancia pesquisa
    LimparTimeOutPainel();
    _pesquisaProgramacaoVeiculo = new PesquisaProgramacaoVeiculo();
    KoBindings(_pesquisaProgramacaoVeiculo, "knockoutPesquisaProgramacaoVeiculoTMS", false, _pesquisaProgramacaoVeiculo.Pesquisar.id);

    _programacaoVeiculo = new ProgramacaoVeiculo();
    KoBindings(_programacaoVeiculo, "knockoutProgramacaoVeiculoTMS", false);

    _programaMotorista = new ProgramaMotorista();
    KoBindings(_programaMotorista, "knoutProgramacaoVeiculoTMS");

    HeaderAuditoria("ProgramacaoVeiculoTMS", _programacaoVeiculo);

    new BuscarVeiculos(_pesquisaProgramacaoVeiculo.Veiculo, null, null, null, null, null, null, null, null, null, null, "0");
    new BuscarVeiculos(_pesquisaProgramacaoVeiculo.Reboque, null, null, null, null, null, null, null, null, null, null, "1");
    new BuscarModelosVeicularesCarga(_pesquisaProgramacaoVeiculo.ModeloVeicular);
    new BuscarProgramacaoSituacaoTMS(_pesquisaProgramacaoVeiculo.ProgramacaoSituacao, null, [EnumTipoEntidadeProgramacao.Todos, EnumTipoEntidadeProgramacao.Veiculo]);
    new BuscarMotoristas(_pesquisaProgramacaoVeiculo.Motorista);
    new BuscarEstados(_pesquisaProgramacaoVeiculo.Estado);
    new BuscarFuncionario(_pesquisaProgramacaoVeiculo.FuncionarioResponsavelCavalo);
    new BuscarMarcasVeiculo(_pesquisaProgramacaoVeiculo.MarcaCavalo);

    new BuscarVeiculos(_programaMotorista.Veiculo, null, null, null, _programaMotorista.Motorista, null, null, null, null, null, null, "0");
    new BuscarMotoristas(_programaMotorista.Motorista);
    new BuscarProgramacaoSituacaoTMS(_programaMotorista.ProgramacaoSituacao, null, [EnumTipoEntidadeProgramacao.Todos, EnumTipoEntidadeProgramacao.Veiculo]);
    new BuscarLocalidades(_programaMotorista.CidadeEstado);

    // Inicia busca
    buscarProgramacaoVeiculo();

    $(window).one('hashchange', function () {
        LimparTimeOutPainel();
    });

    $('#divModalProgramacaoVeiculoTMS').on('hidden.bs.modal', function () {
        _naoPaginar = false;
    });
}

function AdicionarVeiculoClick(e, sender) {
    limparCampos();
    _naoPaginar = true;

    Global.abrirModal('divModalProgramacaoVeiculoTMS');
    _programaMotorista.Veiculo.get$().focus();
}

function RetornoBuscarOrdemServico(data) {
    _programaMotorista.OrdemServicoFrota.val(data.Numero);
    _programaMotorista.OrdemServicoFrota.codEntity(data.Codigo);
}

//*******MÉTODOS*******
function buscarProgramacaoVeiculo() {
    //-- Grid
    // Opcoes
    let editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarProgramacaoVeiculoClick, tamanho: "15", icone: "" };
    let auditar = { descricao: "Auditar", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("ProgramacaoVeiculoTMS", null, _programacaoVeiculo), tamanho: "10", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };

    // Menu
    let menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [editar, auditar],
        tamanho: "5",
        descricao: "Opções"
    };

    let configExportacao = {
        url: "ProgramacaoVeiculoTMS/ExportarPesquisa",
        titulo: "Programações de Veículo"
    };

    // Inicia Grid de busca
    _gridProgramacaoVeiculo = new GridViewExportacao(_programacaoVeiculo.ProgramacaoVeiculo.idGrid, "ProgramacaoVeiculoTMS/Pesquisa", _pesquisaProgramacaoVeiculo, menuOpcoes, configExportacao, null, _itensPorPagina);
    _gridProgramacaoVeiculo.SetGroup({ enable: true, propAgrupa: "EstadoNome", dirOrdena: orderDir.asc });
    _pesquisaProgramacaoVeiculo.Inicio.val(0);
    _pesquisaProgramacaoVeiculo.Limite.val(_itensPorPagina);
    _gridProgramacaoVeiculo.CarregarGrid(buscarSumarizadores);
}

function SalvarClick(e, sender) {

    _mensagemMotoristaSituacao = "";
    ValidarMotoristaSituacao(_programaMotorista.Motorista).then(function () {
        if (_mensagemMotoristaSituacao != "") {
            exibirConfirmacao("Confirmação", _mensagemMotoristaSituacao, function () {
                Salvar(_programaMotorista, "ProgramacaoVeiculoTMS/Salvar", function (arg) {
                    if (arg.Success) {
                        if (arg.Data) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado");
                            Global.fecharModal("divModalProgramacaoVeiculoTMS");


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
            });
        } else {
            Salvar(_programaMotorista, "ProgramacaoVeiculoTMS/Salvar", function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado");
                        Global.fecharModal("divModalProgramacaoVeiculoTMS");


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
           
    });
}

function editarProgramacaoVeiculoClick(e) {
    limparCampos();
    // Seta o codigo do objeto
    _programaMotorista.Codigo.val(e.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_programaMotorista, "ProgramacaoVeiculoTMS/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _naoPaginar = true;

                Global.abrirModal('divModalProgramacaoVeiculoTMS');
                _programaMotorista.Veiculo.get$().focus();
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

    executarReST("ProgramacaoVeiculoTMS/BuscarSumarizadores", {
        NumeroFrota: _pesquisaProgramacaoVeiculo.NumeroFrota.val(),
        Veiculo: _pesquisaProgramacaoVeiculo.Veiculo.codEntity(),
        Reboque: _pesquisaProgramacaoVeiculo.Reboque.codEntity(),
        ModeloVeicular: _pesquisaProgramacaoVeiculo.ModeloVeicular.codEntity(),
        ProgramacaoSituacao: _pesquisaProgramacaoVeiculo.ProgramacaoSituacao.codEntity(),
        Motorista: _pesquisaProgramacaoVeiculo.Motorista.codEntity(),
        Motorista: _pesquisaProgramacaoVeiculo.Estado.codEntity()
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
    if (!_naoPaginar) {

        if (retornoData !== null && retornoData !== undefined)
            _paginasPrevistas = retornoData.recordsTotal / (_itensPorPagina);

        validarPaginacao();
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

}

function retornoVeiculo(data) {
    _programaMotorista.Motorista.val(data.Motorista);
    _programaMotorista.Motorista.codEntity(data.CodigoMotorista);
}

function ValidarMotoristaSituacao(e) {
    let p = new promise.Promise();
    let data = { Codigo: e.codEntity() }
    executarReST("Motorista/ValidarMotoristaSituacao", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.ExibirConfirmacaoMotoristaSituacao) {
                    _mensagemMotoristaSituacao = arg.Msg;
                } else
                    _mensagemMotoristaSituacao = "";
            } else {
                _mensagemMotoristaSituacao = "";
            }
        } else {
            _mensagemMotoristaSituacao  = "";
        }

        p.done();
    });

    return p;
}