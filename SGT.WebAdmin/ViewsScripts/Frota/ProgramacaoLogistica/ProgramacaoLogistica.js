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
/// <reference path="../../Consultas/ProgramacaoAlocacao.js" />
/// <reference path="../../Consultas/ProgramacaoEspecialidade.js" />
/// <reference path="../../Consultas/ProgramacaoSituacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoVeiculo.js" />
/// <reference path="../../Cargas/Carga/DadosCarga/Carga.js" />
/// <reference path="../../Enumeradores/EnumTipoEntidadeProgramacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoProgramacaoLogistica.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumEntradaSaida.js" />
/// <reference path="../../Enumeradores/EnumTipoMotorista.js" />
/// <reference path="../../Consultas/ColaboradorSituacao.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/TipoPlotagem.js" />
/// <reference path="../../Consultas/ModeloCarroceria.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridProgramacaoLogistica;
var _pesquisaProgramacaoLogistica;
var _programacaoLogistica;
var _programaMotorista;

var PesquisaProgramacaoLogistica = function () {
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ProgramacaoAlocacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Alocação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.SituacaoColaborador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Situação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoMotorista = PropertyEntity({ val: ko.observable(EnumTipoMotorista.Proprio), options: EnumTipoMotorista.obterOpcoesPesquisa(), def: EnumTipoMotorista.Proprio, text: "Tipo Motorista: ", issue: 640, required: false });
    this.TipoVeiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo do Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.SituacaoVeiculo = PropertyEntity({ val: ko.observable(EnumSituacaoProgramacaoLogistica.EmCarga), options: EnumSituacaoProgramacaoLogistica.obterOpcoes(), def: EnumSituacaoProgramacaoLogistica.EmCarga, text: "Situação do Veículo: ", required: false });
    this.PlotagemVeiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Plotagem:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoCarroceria = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo da Carroceria:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.EmViagem = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, text: "Em Viagem? ", required: false });
    this.TipoGuarita = PropertyEntity({ val: ko.observable(EnumEntradaSaida.Todos), options: EnumEntradaSaida.obterOpcoesPesquisa(), def: EnumEntradaSaida.Todos, text: "Última Entrada/Saída: " });
    this.EmManutencao = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, text: "Em Manutenção? ", required: false });
    this.SomenteMotoristaComOciosidade = PropertyEntity({ text: "Somente motoristas com Ociosidade", val: ko.observable(false), getType: typesKnockout.bool, def: false });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridProgramacaoLogistica.CarregarGrid(buscarSumarizadores);
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false)
    });
};

var ProgramacaoLogistica = function () {
    this.ProgramacaoLogistica = PropertyEntity({ type: types.event, text: "Painel", idGrid: guid(), visible: ko.observable(true) });
    this.Status = ko.observableArray();
    this.StatusManutencao = ko.observableArray();
};

var ProgramaMotorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });

    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motorista:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(false), required: true });
    this.ProgramacaoAlocacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Alocação:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: true });

    this.CategoriaCNH = PropertyEntity({ text: "Cat. CNH: ", getType: typesKnockout.string, enable: ko.observable(false), required: ko.observable(false) });
    this.VencimentoCNH = PropertyEntity({ text: "Vencimento CNH: ", getType: typesKnockout.date, enable: ko.observable(false), required: ko.observable(false) });
    this.VencimentoMoop = PropertyEntity({ text: "Vencimento Moop: ", getType: typesKnockout.date, enable: ko.observable(false), required: ko.observable(false) });
    this.DataAdmissao = PropertyEntity({ text: "Admissão: ", getType: typesKnockout.dateTime, enable: ko.observable(false), required: ko.observable(false) });

    this.Salvar = PropertyEntity({ eventClick: SalvarClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadProgramacaoLogistica() {
    _pesquisaProgramacaoLogistica = new PesquisaProgramacaoLogistica();
    KoBindings(_pesquisaProgramacaoLogistica, "knockoutPesquisaProgramacaoLogistica", false, _pesquisaProgramacaoLogistica.Pesquisar.id);

    _programacaoLogistica = new ProgramacaoLogistica();
    KoBindings(_programacaoLogistica, "knockoutProgramacaoLogistica", false);

    _programaMotorista = new ProgramaMotorista();
    KoBindings(_programaMotorista, "knoutProgramacaoLogistica");

    BuscarMotoristas(_pesquisaProgramacaoLogistica.Motorista);
    BuscarProgramacaoAlocacao(_pesquisaProgramacaoLogistica.ProgramacaoAlocacao, null, [EnumTipoEntidadeProgramacao.Todos, EnumTipoEntidadeProgramacao.Motorista]);
    BuscarSituacoesColaborador(_pesquisaProgramacaoLogistica.SituacaoColaborador);
    BuscarModelosVeicularesCarga(_pesquisaProgramacaoLogistica.TipoVeiculo);
    BuscarVeiculos(_pesquisaProgramacaoLogistica.Veiculo);
    BuscarTipoPlotagem(_pesquisaProgramacaoLogistica.PlotagemVeiculo);
    BuscarModelosCarroceria(_pesquisaProgramacaoLogistica.TipoCarroceria);

    BuscarMotoristas(_programaMotorista.Motorista);
    BuscarProgramacaoAlocacao(_programaMotorista.ProgramacaoAlocacao, null, [EnumTipoEntidadeProgramacao.Todos, EnumTipoEntidadeProgramacao.Motorista]);

    buscarProgramacaoLogistica();
}

//*******MÉTODOS*******

function buscarProgramacaoLogistica() {
    let configExportacao = {
        url: "ProgramacaoLogistica/ExportarPesquisa",
        titulo: "Programação de Motoristas"
    };

    _gridProgramacaoLogistica = new GridView(_programacaoLogistica.ProgramacaoLogistica.idGrid, "ProgramacaoLogistica/Pesquisa", _pesquisaProgramacaoLogistica, null, null, 15, null, null, null, null, null, null, configExportacao);

    _gridProgramacaoLogistica.SetPermitirEdicaoColunas(true);
    _gridProgramacaoLogistica.SetSalvarPreferenciasGrid(true);

    _gridProgramacaoLogistica.CarregarGrid(buscarSumarizadores);
}

function SalvarClick(e, sender) {
    Salvar(_programaMotorista, "ProgramacaoLogistica/Salvar", function (arg) {

        if (!arg.Success) {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            return;
        }

        if (arg.Data) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado");
            Global.fecharModal('divModalProgramacaoLogistica');
            limparCampos();
            _gridProgramacaoLogistica.CarregarGrid(buscarSumarizadores);
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }

    }, sender, null);
}

function editarProgramacaoLogisticaClick(e) {
    limparCampos();
    _programaMotorista.Codigo.val(e.Codigo);

    BuscarPorCodigo(_programaMotorista, "ProgramacaoLogistica/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                Global.abrirModal('divModalProgramacaoLogistica');
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
    executarReST("ProgramacaoLogistica/BuscarSumarizadores", RetornarObjetoPesquisa(_pesquisaProgramacaoLogistica), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _programacaoLogistica.Status(retorno.Data.Status.slice());
                _programacaoLogistica.StatusManutencao(retorno.Data.StatusManutencao.slice());
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function limparCampos() {
    LimparCampos(_programaMotorista);
}