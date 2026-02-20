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
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/ModeloVeiculo.js" />
/// <reference path="../../Consultas/Equipamento.js" />
/// <reference path="../../Consultas/MarcaVeiculo.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/OrdemServico.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Enumeradores/EnumSituacaoVeiculo.js" />
/// <reference path="../../Cargas/Carga/DadosCarga/Carga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPainelVeiculo;
var _pesquisaPainelVeiculo;
var _painelVeiculo;
var _indicacaoVeiculo;
//var _gridReboques;

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
var _painelVeiculoModalIndicacaoVeiculo;

var _tipoVeiculoPesquisa = [
    { text: "Todos", value: "-1" },
    { text: "Tração", value: "0" },
    { text: "Reboque", value: "1" }];

var _tipoProprietarioPesquisa = [{ text: "Todos", value: "A" },
{ text: "Própria", value: "P" },
{ text: "Terceiros", value: "T" }];

var _simNao = [
    { text: "Sim", value: true },
    { text: "Não", value: false }
];

var _situacaoVeiculo = [
    { text: "Todos", value: EnumSituacaoVeiculo.Todos },
    { text: "Disponível", value: EnumSituacaoVeiculo.Disponivel },
    { text: "Em Manutenção", value: EnumSituacaoVeiculo.EmManutencao },
    { text: "Em Viagem", value: EnumSituacaoVeiculo.EmViagem },
    { text: "Em Fila", value: EnumSituacaoVeiculo.EmFila },
    { text: "Indisponível", value: EnumSituacaoVeiculo.Indisponivel },
];

var _tiposOrdemServico = [];

var PesquisaPainelVeiculo = function () {
    this.Inicio = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });
    this.Limite = PropertyEntity({ val: ko.observable(_itensPorPagina), def: _itensPorPagina, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });

    this.Placa = PropertyEntity({ text: "Placa: " });
    this.NumeroFrota = PropertyEntity({ text: "Nº Frota: ", visible: ko.observable(true), maxlength: 30 });
    this.TipoVeiculo = PropertyEntity({ val: ko.observable("-1"), options: _tipoVeiculoPesquisa, def: "-1", text: "Tipo Veículo: " });
    this.SituacaoVeiculo = PropertyEntity({ val: ko.observable(EnumSituacaoVeiculo.Todos), options: _situacaoVeiculo, def: EnumSituacaoVeiculo.Todos, text: "Situação do Veículo: " });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular de Carga:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.MarcaVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Marca:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoPropriedade = PropertyEntity({ val: ko.observable("P"), def: "P", options: _tipoProprietarioPesquisa, text: "Tipo da Propriedade: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.DataSituacao = PropertyEntity({ text: "Data da Situação: ", getType: typesKnockout.date });
    this.DataInicioDisponivel = PropertyEntity({ text: "Prev. Disponível: ", getType: typesKnockout.date });
    this.DataFimDisponivel = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.LocalPrevisto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local Previsão:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Proprietario = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: "Proprietário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoFrota = PropertyEntity({ val: ko.observable(EnumTipoFrota.NaoDefinido), options: EnumTipoFrota.obterOpcoes(), def: EnumTipoFrota.NaoDefinido, text: "Tipo de Frota:", enable: ko.observable(true) });
    this.CentroCarregamento = PropertyEntity({ text: "Centro de Carregamento:", required: ko.observable(true), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });

    this.HabilitarPainel = PropertyEntity({ getType: typesKnockout.bool, eventChange: HabilitarPainelOnChange, val: ko.observable(false), def: false, text: "Habilitar visualização em formato de painel?", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _pesquisouNovamente = true;
            _pesquisaPainelVeiculo.Inicio.val(0);
            _pesquisaPainelVeiculo.Limite.val(_itensPorPagina);
            _gridPainelVeiculo.CarregarGrid();
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

var PainelVeiculo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });
    this.PainelVeiculo = PropertyEntity({ type: types.event, text: "Painel", idGrid: guid(), visible: ko.observable(true) });
}

var IndicacaoVeiculo = function () {
    this.SituacaoAtual = PropertyEntity({ val: ko.observable(EnumSituacaoVeiculo.Disponivel), def: EnumSituacaoVeiculo.Disponivel, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });
    this.IndicacaoVeiculoVazio = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, idFade: guid(), visibleFade: ko.observable(false) });
    this.IndicacaoAvisoCarregamento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, idFade: guid(), visibleFade: ko.observable(false) });
    this.IndicacaoViagem = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, idFade: guid(), visibleFade: ko.observable(false) });
    this.IndicacaoManutencao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, idFade: guid(), visibleFade: ko.observable(false) });

    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid(), visible: ko.observable(true), enable: false });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.LocalAtual = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local Atual/Previsão:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataHoraIndicacao = PropertyEntity({ text: "Data e Hora: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });

    this.VeiculoVazio = PropertyEntity({ val: ko.observable(true), options: _simNao, def: true, text: "Veículo Vazio? ", visible: ko.observable(true) });
    this.AvisadoCarregamento = PropertyEntity({ val: ko.observable(true), options: _simNao, def: true, text: "Avisado p/ Carregamento? ", visible: ko.observable(true) });

    this.ListaReboques = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
}

const CRUDIndicacaoVeiculo = function () {
    this.Salvar = PropertyEntity({ eventClick: SalvarClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
}

//*******EVENTOS*******
function loadPainelVeiculo() {
    //-- Knouckout
    // Instancia pesquisa
    LimparTimeOutPainel();
    _pesquisaPainelVeiculo = new PesquisaPainelVeiculo();
    KoBindings(_pesquisaPainelVeiculo, "knockoutPesquisaPainelVeiculo", false, _pesquisaPainelVeiculo.Pesquisar.id);

    _painelVeiculo = new PainelVeiculo();
    KoBindings(_painelVeiculo, "knockoutPainelVeiculo", false);

    _indicacaoVeiculo = new IndicacaoVeiculo();
    KoBindings(_indicacaoVeiculo, "knoutPainelVeiculoIndicacao");

    _CRUDIndicacaoVeiculo = new CRUDIndicacaoVeiculo();
    KoBindings(_CRUDIndicacaoVeiculo, "knockoutCRUDIndicacao");

    HeaderAuditoria("SituacaoVeiculo", _painelVeiculo);

    _painelVeiculoModalIndicacaoVeiculo = new bootstrap.Modal(document.getElementById("divModalIndicacaoVeiculo"), { backdrop: 'static' })

    $("#" + _pesquisaPainelVeiculo.Placa.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });

    new BuscarModelosVeiculo(_pesquisaPainelVeiculo.ModeloVeiculo);
    new BuscarClientes(_pesquisaPainelVeiculo.Proprietario, retornoProprietario);
    new BuscarEmpresa(_pesquisaPainelVeiculo.Transportador);
    new BuscarMarcasVeiculo(_pesquisaPainelVeiculo.MarcaVeiculo);
    new BuscarModelosVeicularesCarga(_pesquisaPainelVeiculo.ModeloVeicularCarga);
    new BuscarMotoristas(_pesquisaPainelVeiculo.Motorista);
    new BuscarLocalidades(_pesquisaPainelVeiculo.LocalPrevisto);
    new BuscarCentrosCarregamento(_pesquisaPainelVeiculo.CentroCarregamento);
    new BuscarMotoristas(_indicacaoVeiculo.Motorista);
    new BuscarLocalidades(_indicacaoVeiculo.LocalAtual);

    loadPainelVeiculoViagem();
    loadPainelVeiculoManutencao();
    loadPainelVeiculoVeiculosVinculados();
    loadPainelVeiculoLavacao();
    loadHistoricoSituacoesVeiculo();

    // Inicia busca
    buscarPainelVeiculo();

    $(window).one('hashchange', function () {
        LimparTimeOutPainel();
    });

    $('#divModalIndicacaoVeiculo').on('hidden.bs.modal', function () {
        _naoPaginar = false;
    });
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe)
        _pesquisaPainelVeiculo.Transportador.visible(false);
}

function HabilitarPainelOnChange(e, sender) {
    _pesquisouNovamente = true;
    _pesquisaPainelVeiculo.Inicio.val(0);
    _pesquisaPainelVeiculo.Limite.val(_itensPorPagina);
    if ($('#' + e.HabilitarPainel.id).is(':checked')) {
        _pesquisaPainelVeiculo.HabilitarPainel.val(true);
        _habilitarPainel = true;
        _paginaAtual = 1;
        carregarPainelVeiculo(_paginaAtual, _paginou, executarPesquisaTimeOut);
    } else {

        $('#knockoutPainelVeiculo').removeClass('ocultarPaginacao');
        _habilitarPainel = false;
        _pesquisaPainelVeiculo.HabilitarPainel.val(false);
        _gridPainelVeiculo.CarregarGrid();
    }
}
function retornoProprietario(row) {
    _pesquisaPainelVeiculo.Proprietario.codEntity(row.Codigo);
    _pesquisaPainelVeiculo.Proprietario.val(row.Nome);
}

//*******MÉTODOS*******
function buscarPainelVeiculo() {
    //-- Grid
    // Opcoes
    const veiculoVazio = { descricao: "Veículo Vazio", id: "clasEditarVeiculoVazio", evento: "onclick", metodo: veiculoVazioClick, tamanho: "15", icone: "" };
    const indicacaoCarregamento = { descricao: "Aviso p/ Carregamento", id: "clasEditarIndicacaoCarregamento", evento: "onclick", metodo: indicacaoCarregamentoClick, tamanho: "15", icone: "" };
    const emViagem = { descricao: "Viagem", id: "clasEditarEmViagem", evento: "onclick", metodo: emViagemClick, tamanho: "15", icone: "", visibilidade: VisibilidadeEmViagem };
    const emManutencao = { descricao: "Manutenção", id: "clasEditarEmManutencao", evento: "onclick", metodo: emManutencaoClick, tamanho: "15", icone: "", visibilidade: VisibilidadeEmManutencao };
    const lavacao = { descricao: "Lavação", id: "clasEditarLavacao", evento: "onclick", metodo: lavacaoClick, tamanho: "15", icone: "" };
    const historicoSituacoes = { descricao: "Histórico de Situações", evento: "onclick", metodo: exibirModalsituacoesVeiculo, tamanho: "15", icone: "" };

    // Menu
    const menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [veiculoVazio, indicacaoCarregamento, emViagem, emManutencao, lavacao, historicoSituacoes],
        tamanho: "5",
        descricao: "Opções"
    };

    // Inicia Grid de busca
    _gridPainelVeiculo = new GridView(_painelVeiculo.PainelVeiculo.idGrid, "PainelVeiculo/Pesquisa", _pesquisaPainelVeiculo, menuOpcoes, null, _itensPorPagina);
    //_pesquisouNovamente = true;
    _pesquisaPainelVeiculo.Inicio.val(0);
    _pesquisaPainelVeiculo.Limite.val(_itensPorPagina);
    _gridPainelVeiculo.SetPermitirEdicaoColunas(true);
    _gridPainelVeiculo.SetSalvarPreferenciasGrid(true);
    _gridPainelVeiculo.CarregarGrid();
}

function VisibilidadeEmViagem(dataRow) {
    return dataRow.SituacaoVeiculo === EnumSituacaoVeiculo.EmViagem || dataRow.SituacaoVeiculo === EnumSituacaoVeiculo.Disponivel;
}

function VisibilidadeEmManutencao(dataRow) {
    return dataRow.SituacaoVeiculo === EnumSituacaoVeiculo.EmManutencao || dataRow.SituacaoVeiculo === EnumSituacaoVeiculo.Disponivel || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe;
}

function SalvarClick(e, sender) {
    if (ValidarCamposPainelVeiculo()) {
        preencherListaReboques();
        Salvar(_indicacaoVeiculo, "PainelVeiculo/Salvar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado");
                    _painelVeiculoModalIndicacaoVeiculo.hide();
                    _naoPaginar = false;
                    if (!_habilitarPainel)
                        _gridPainelVeiculo.CarregarGrid();
                    executarPesquisaTimeOut();
                    limparCampos();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender, null);
    } else {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Favor informe os campos obrigatórios.");
    }
}

function ValidarCamposPainelVeiculo() {
    var retorno = true;
    if (_indicacaoVeiculo.IndicacaoViagem.val() === true) {
        if (_indicacaoVeiculo.SituacaoAtual.val() === EnumSituacaoVeiculo.EmViagem) {
            _painelVeiculoViagem.DataHoraRetornoViagem.required(true);
            _painelVeiculoViagem.LocalidadeRetornoViagem.required(true);
        } else {
            _painelVeiculoViagem.DataHoraSaidaInicioViagem.required(true);
            _painelVeiculoViagem.DataHoraPrevisaoRetornoInicioViagem.required(true);
            _painelVeiculoViagem.LocalidadeDestinoInicioViagem.required(true);
        }
        if (retorno === true)
            retorno = ValidarCamposObrigatorios(_painelVeiculoViagem);
    } else if (_indicacaoVeiculo.IndicacaoManutencao.val() === true) {
        if (_indicacaoVeiculo.SituacaoAtual.val() === EnumSituacaoVeiculo.EmManutencao) {
            _painelVeiculoManutencao.DataHoraSaidaManutencao.required(true);
            _painelVeiculoManutencao.OrdemServicoFrota.required(true);
        } else {
            _painelVeiculoManutencao.DataHoraEntradaManutencao.required(true);
            _painelVeiculoManutencao.DataHoraPrevisaoSaidaManutencao.required(true);
        }
        if (retorno === true)
            retorno = ValidarCamposObrigatorios(_painelVeiculoManutencao);
    }
    return retorno;
}

function veiculoVazioClick(e) {
    limparCampos();
    // Seta o codigo do objeto
    _indicacaoVeiculo.Codigo.val(e.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_indicacaoVeiculo, "PainelVeiculo/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _naoPaginar = true;
                RecarregarListaReboques();
                _indicacaoVeiculo.AvisadoCarregamento.visible(false);
                _indicacaoVeiculo.VeiculoVazio.visible(true);

                _indicacaoVeiculo.IndicacaoVeiculoVazio.val(true);
                _indicacaoVeiculo.IndicacaoAvisoCarregamento.val(false);
                _indicacaoVeiculo.IndicacaoManutencao.val(false);
                _indicacaoVeiculo.IndicacaoViagem.val(false);

                _painelVeiculoModalIndicacaoVeiculo.show();
                $("#liTabManutencao").hide();
                $("#liTabViagem").hide();
                $("#liTabLavacao").hide();
                _indicacaoVeiculo.VeiculoVazio.get$().focus();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function indicacaoCarregamentoClick(e) {
    limparCampos();
    // Seta o codigo do objeto
    _indicacaoVeiculo.Codigo.val(e.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_indicacaoVeiculo, "PainelVeiculo/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.PainelVeiculoViagem != null)
                    PreencherObjetoKnout(_painelVeiculoViagem, { Data: arg.Data.PainelVeiculoViagem });

                _naoPaginar = true;
                RecarregarListaReboques();
                _indicacaoVeiculo.AvisadoCarregamento.visible(true);
                _indicacaoVeiculo.VeiculoVazio.visible(false);

                _indicacaoVeiculo.IndicacaoVeiculoVazio.val(false);
                _indicacaoVeiculo.IndicacaoAvisoCarregamento.val(true);
                _indicacaoVeiculo.IndicacaoManutencao.val(false);
                _indicacaoVeiculo.IndicacaoViagem.val(false);

                _painelVeiculoModalIndicacaoVeiculo.show();
                $("#liTabManutencao").hide();
                $("#liTabViagem").hide();
                $("#liTabLavacao").hide();
                _indicacaoVeiculo.AvisadoCarregamento.get$().focus();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function limparCampos() {
    Global.ResetarAbas();
    //$("#tabIndicacao a:first").tab("show");
    $("#liTabManutencao").hide();
    $("#liTabViagem").hide();
    $("#liTabLavacao").hide();
    _indicacaoVeiculo.LocalAtual.visible(true);
    _indicacaoVeiculo.DataHoraIndicacao.visible(true);
    LimparCampos(_indicacaoVeiculo);

    _painelVeiculoViagem.DataHoraSaidaInicioViagem.enable(true);
    _painelVeiculoViagem.DataHoraPrevisaoRetornoInicioViagem.enable(true);
    _painelVeiculoViagem.LocalidadeDestinoInicioViagem.enable(true);
    _painelVeiculoViagem.DataHoraRetornoViagem.enable(true);
    _painelVeiculoViagem.LocalidadeRetornoViagem.enable(true);

    _painelVeiculoManutencao.DataHoraEntradaManutencao.enable(true);
    _painelVeiculoManutencao.DataHoraPrevisaoSaidaManutencao.enable(true);
    _painelVeiculoManutencao.DataHoraSaidaManutencao.enable(true);
    _painelVeiculoManutencao.OrdemServicoFrota.enable(true);

    _painelVeiculoViagem.DataHoraRetornoViagem.required(false);
    _painelVeiculoViagem.LocalidadeRetornoViagem.required(false);
    _painelVeiculoViagem.DataHoraSaidaInicioViagem.required(false);
    _painelVeiculoViagem.DataHoraPrevisaoRetornoInicioViagem.required(false);
    _painelVeiculoViagem.LocalidadeDestinoInicioViagem.required(false);

    _painelVeiculoManutencao.DataHoraSaidaManutencao.required(false);
    _painelVeiculoManutencao.OrdemServicoFrota.required(false);
    _painelVeiculoManutencao.DataHoraEntradaManutencao.required(false);
    _painelVeiculoManutencao.DataHoraPrevisaoSaidaManutencao.required(false);

    RecarregarListaReboques();
}

function LimparTimeOutPainel() {
    if (_timeOutPainel != null)
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
                carregarPainelVeiculo(_paginaAtual, _paginou, executarPesquisaTimeOut);
            }
        }, _tempoParaTroca);
    }
}

function carregarPainelVeiculo(page, paginou, callback) {

    if (_pesquisouNovamente) {
        page = 1;
        paginou = false;
    }

    _pesquisaPainelVeiculo.Inicio.val((_itensPorPagina) * (page - 1));
    _pesquisaPainelVeiculo.Limite.val(_itensPorPagina);
    _gridPainelVeiculo.CarregarGrid(callback);

    if (_habilitarPainel) {
        $('#knockoutPainelVeiculo').addClass('ocultarPaginacao');
    }

    //callback();
}



var _situacoesVeiculo;
var _gridSituacoesVeiculo

var SituacoesVeiculo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.GridConsultas = PropertyEntity({ idGrid: guid() });
    this.Pesquisar = PropertyEntity({ eventClick: pesquisarHistoricoSituacoesVeiculo, type: types.event, text: "Pesquisar", visible: ko.observable(true) });
}

function loadHistoricoSituacoesVeiculo() {
    _situacoesVeiculo = new SituacoesVeiculo();
    KoBindings(_situacoesVeiculo, "knockoutHistoricoSituacoesVeiculo");
    _gridSituacoesVeiculo = new GridView(_situacoesVeiculo.GridConsultas.idGrid, "PainelVeiculo/BuscarHistoricoSituacoesVeiculo", _situacoesVeiculo, null, null, 10);
}

function exibirModalsituacoesVeiculo(registroSelecionado) {
    _situacoesVeiculo.Codigo.val(registroSelecionado.Codigo);
    _gridSituacoesVeiculo.CarregarGrid();

    $("#divModalHistoricoSituacoesVeiculo")
        .modal("show").on("show.bs.modal", function () {
        }).on("hidden.bs.modal", function () {
            LimparCampos(_situacoesVeiculo);
        });
}

function pesquisarHistoricoSituacoesVeiculo() {
    _gridSituacoesVeiculo.CarregarGrid();
}