/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Configuracao/Sistema/OperadorLogistica.js" />
/// <reference path="../../Enumeradores/EnumConsultaPorEntregaStatus.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridConsultaEntregaAtrasada, _pesquisaConsultaEntregaAtrasada, _CRUDRelatorio, _CRUDFiltrosRelatorio;
var _etapaAtualFluxo;
var _CRUDResponsavelAtraso;

var PesquisaConsultaEntregaAtrasada = function () {
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operação:", idBtnSearch: guid() });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.NumeroCarga = PropertyEntity({ type: types.string, val: ko.observable(""), text: "Numero da Carga:" });
    this.NumeroNota = PropertyEntity({ type: types.int, val: ko.observable(0), text: "Numero da Nota:" });
    this.Placa = PropertyEntity({ type: types.string, val: ko.observable(""), text: "Placa:" });
    this.DataPrevisaoEntregaInicial = PropertyEntity({ text: "Data Previsão Entrega: ", getType: typesKnockout.date });
    this.DataPrevisaoEntregaFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });

    this.DataPrevisaoEntregaInicial.dateRangeLimit = this.DataPrevisaoEntregaFinal;
    this.DataPrevisaoEntregaFinal.dateRangeInit = this.DataPrevisaoEntregaInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConsultaEntregaAtrasada.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: "gridConsultaEntregaAtrasada", visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function LoadConsultaEntregaAtrasada() {
    _pesquisaConsultaEntregaAtrasada = new PesquisaConsultaEntregaAtrasada();
    KoBindings(_pesquisaConsultaEntregaAtrasada, "knockoutPesquisaConsultaEntregaAtrasada", false, _pesquisaConsultaEntregaAtrasada.Pesquisar.idGrid);
    buscarDetalhesOperador(function () {

        loadGridConsulta();

        new BuscarTransportadores(_pesquisaConsultaEntregaAtrasada.Transportador);
        new BuscarTiposOperacao(_pesquisaConsultaEntregaAtrasada.TipoOperacao);
        new BuscarClientes(_pesquisaConsultaEntregaAtrasada.Cliente);
        loadCRUDResponsavelAtraso();

        loadMonitoramentoControleEntrega(function () {
            registraComponente();
            loadEtapasControleEntrega();

            isMobile = $(window).width() <= 980;
            _containerControleEntrega = new ContainerControleEntrega();
            KoBindings(_containerControleEntrega, "knoutContainerControleEntrega");
        });
    });
}

function loadMonitoramentoControleEntrega(callback) {
    carregarHTMLComponenteControleEntrega(callback);
}


function loadGridConsulta() {
    var draggableRows = false;
    var draggableRows = false;
    var limiteRegistros = 10;
    var totalRegistrosPorPagina = 10;

    var opcaoDefinirResponsavel = { descricao: "Definir Responsável", id: guid(), evento: "onclick", metodo: definirResponsavelClick, tamanho: "10", icone: "", visibilidade: definirVisibilidadeResponsavel };
    var opcaoDetalhesEntrega = { descricao: "Detalhes da entrega", id: guid(), evento: "onclick", metodo: visualizarDetalhesEntregaClick, tamanho: "10", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [opcaoDetalhesEntrega, opcaoDefinirResponsavel], tamanho: 5, };

    _gridConsultaEntregaAtrasada = new GridView("gridConsultaEntregaAtrasada", "ConsultaEntregaAtrasada/Pesquisa", _pesquisaConsultaEntregaAtrasada, menuOpcoes, null, totalRegistrosPorPagina, null, true, draggableRows, undefined, limiteRegistros, undefined, null, undefined, undefined);
    _gridConsultaEntregaAtrasada.SetPermitirEdicaoColunas(true);
    _gridConsultaEntregaAtrasada.SetSalvarPreferenciasGrid(true);
    _gridConsultaEntregaAtrasada.SetHabilitarScrollHorizontal(true, 100);
    _gridConsultaEntregaAtrasada.CarregarGrid();
}


function definirResponsavelClick(filaSelecionada) {
    executarReST("TipoResponsavelAtrasoEntrega/BuscarTiposResponsavelAtrasoAtivos", { Codigo: filaSelecionada.CodigoEntrega }, function (arg) {
        if (arg.Success) {
            console.log(arg.Data);
            if (arg.Data !== false) {
                console.log(arg.Data);
                _CRUDResponsavelAtraso.CodigoEntrega.val(filaSelecionada.CodigoEntrega);
                _CRUDResponsavelAtraso.TipoResponsavel.options(arg.Data.TipoResponsavel);

                ExibirModalResponsavelAtraso();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function ExibirModalResponsavelAtraso() {
    Global.abrirModal('divModalResponsavelAtraso');
}

function loadCRUDResponsavelAtraso() {
    _CRUDResponsavelAtraso = new _CRUDResponsavelAtraso();
    KoBindings(_CRUDResponsavelAtraso, "knockoutCRUDResponsavelAtraso");
    limparCamposResponsavel();
}

var _CRUDResponsavelAtraso = function () {
    this.Confirmar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: confirmarResponsavelClick, text: "Confirmar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: cancelarResponsavelClick, text: "Cancelar", visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.TipoResponsavel = PropertyEntity({ text: "*Tipo Responsavel: ", val: ko.observable(true), options: ko.observable([]), def: 1 });
    this.CodigoEntrega = PropertyEntity();
};

function limparCamposResponsavel() {
    LimparCampos(_CRUDResponsavelAtraso);
    _CRUDResponsavelAtraso.TipoResponsavel.options([]);
}

function confirmarResponsavelClick(e, sender) {

    Salvar(_CRUDResponsavelAtraso, "ConsultaEntregaAtrasada/AdicionarResponsavelEntregaAtraso", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                Global.fecharModal("divModalResponsavelAtraso");
                limparCamposResponsavel();
                _gridConsultaEntregaAtrasada.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarResponsavelClick() {
    Global.fecharModal('divModalResponsavelAtraso');
    limparCamposResponsavel();
}


function visualizarDetalhesEntregaClick(filaSelecionada) {
    atualizaTituloModalCarga(filaSelecionada);
    var carga = filaSelecionada.Codigo;

    if (carga == 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Não existe carga para este veículo.");
        return;
    }

    executarReST("/ControleEntrega/ObterControleEntregaPorcarga", { Carga: carga }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                $("#knoutContainerControleEntrega").one('hidden.bs.modal', function () {
                });
                _containerControleEntrega.Entregas.val([arg.Data.Entregas]);
                _etapaAtualFluxo = new Entrega(arg.Data.Entregas);

                //AQUI ABRE DIRETO A ENTREGA, MAS É NECESSARIO Q TER OS OBJETOS DO FLUXO _containerControleEntrega.Entregas
                PreencherObjetoKnout(_etapaAtualFluxo, { Data: arg.Data.Entregas });
                exibirDetalhesEntrega(_etapaAtualFluxo, { Codigo: filaSelecionada.CodigoEntrega })

            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}


function definirVisibilidadeResponsavel(data) {
    if (data.TipoResponsavel <= 0) {
        return true;
    } else
        return false;
}