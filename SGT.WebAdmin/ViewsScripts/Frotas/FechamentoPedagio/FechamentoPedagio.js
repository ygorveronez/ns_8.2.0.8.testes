/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFechamentoPedagio.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Pedagio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFechamentoPedagio;
var _fechamentoPedagio;
var _crudFechamentoPedagio;
var _pesquisaFechamentoPedagio;

var _globalDataRow;
var _globalRow;

var _situacaoFechamentoPedagio = [
    { text: "Todas", value: EnumSituacaoFechamentoPedagio.Todas },
    { text: "Pendente", value: EnumSituacaoFechamentoPedagio.Pendente },
    { text: "Finalizado", value: EnumSituacaoFechamentoPedagio.Finalizado },
];

var PesquisaFechamentoPedagio = function () {
    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            _gridFechamentoPedagio.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoFechamentoPedagio.Todas), options: _situacaoFechamentoPedagio, def: EnumSituacaoFechamentoPedagio.Todas });
    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Fim: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

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

var FechamentoPedagio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), issue: 143, enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _situacaoFechamentoPedagio, def: 0 });

    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.NaoAdicionar = PropertyEntity({ idGrid: guid(), getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array() });
};

var CRUDFechamentoPedagio = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Gerar Fechamento", visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar o Fechamento", visible: ko.observable(false), enable: ko.observable(true) });

    this.GerarNovoFechamento = PropertyEntity({ eventClick: gerarNovoFechamentoClick, type: types.event, text: "Limpar (Gerar Novo Fechamento)", visible: ko.observable(false), enable: ko.observable(true) });
    this.FinalizarFechamento = PropertyEntity({ eventClick: finalizarFechamentoClick, type: types.event, text: "Finalizar o Fechamento", visible: ko.observable(false), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadFechamentoPedagio() {
    // Pesquisa de fechamentos
    _pesquisaFechamentoPedagio = new PesquisaFechamentoPedagio();
    KoBindings(_pesquisaFechamentoPedagio, "knockoutPesquisaFechamentoPedagio", false, _pesquisaFechamentoPedagio.Pesquisar.id);

    // Aba Dados do fechamento
    _fechamentoPedagio = new FechamentoPedagio();
    KoBindings(_fechamentoPedagio, "knockoutDadosFechamento");

    // Controle dos botoes
    _crudFechamentoPedagio = new CRUDFechamentoPedagio();
    KoBindings(_crudFechamentoPedagio, "knockoutCRUDFechamentoPedagio");

    HeaderAuditoria("FechamentoPedagio", _fechamentoPedagio);

    new BuscarVeiculos(_fechamentoPedagio.Veiculo);
    new BuscarVeiculos(_pesquisaFechamentoPedagio.Veiculo);

    buscarFechamentoPedagio();
    loadPedagio();
}

function finalizarFechamentoClick(e, sender) {
    exibirConfirmacao("Confirmação", "realmente deseja fechar os pedágios?", function () {
        Salvar(_fechamentoPedagio, "FechamentoPedagio/FecharPedagio", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Pedágios Fechados Com sucesso.");
                    limparCamposFechamentoPedagio()
                    _gridFechamentoPedagio.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
                _gridPedagios.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function cancelarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar o fechamento?", function () {
        Salvar(_fechamentoPedagio, "FechamentoPedagio/CancelarFechamento", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelada com Sucesso.");
                    _gridFechamentoPedagio.CarregarGrid();
                    limparCamposFechamentoPedagio();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function atualizarClick(e, sender) {
    Salvar(_fechamentoPedagio, "FechamentoPedagio/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function gerarNovoFechamentoClick(e) {
    limparCamposFechamentoPedagio();
}

function adicionarClick(e, sender) {
    //iniciarControleManualRequisicao();
    Salvar(_fechamentoPedagio, "FechamentoPedagio/GerarFechamentoPedagios", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _fechamentoPedagio.Codigo.val(arg.Data);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "cadastrado");
                PreecherDadosPedagios(function () {
                    $("#myTab a:eq(1)").tab("show");
                });
                _gridFechamentoPedagio.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                finalizarControleManualRequisicao();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            finalizarControleManualRequisicao();
        }
    }, sender, function () {
        finalizarControleManualRequisicao();
    });
}

//*******MÉTODOS*******

function limparCamposFechamentoPedagio() {
    _crudFechamentoPedagio.Adicionar.visible(true);
    ocultarCamposPadroes();

    _fechamentoPedagio.DataInicio.enable(true);
    _fechamentoPedagio.DataFim.enable(true);
    _fechamentoPedagio.Veiculo.enable(true);
    LimparDetalhesPedagios();

    LimparCampos(_fechamentoPedagio);
    //validarPermissoesPersonalizadas();
    resetarTabs();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}

function ocultarCamposPadroes() {
    _crudFechamentoPedagio.FinalizarFechamento.visible(false);
    _crudFechamentoPedagio.GerarNovoFechamento.visible(false);
    _crudFechamentoPedagio.Cancelar.visible(false);
}

function buscarFechamentoPedagio() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarFechamentoPedagio, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridFechamentoPedagio = new GridView(_pesquisaFechamentoPedagio.Pesquisar.idGrid, "FechamentoPedagio/Pesquisar", _pesquisaFechamentoPedagio, menuOpcoes, null);
    _gridFechamentoPedagio.CarregarGrid();
}

function editarFechamentoPedagio(fechamentoPedagioGrid, callbackGrid) {
    // Limpa Dados e Lista
    limparCamposFechamentoPedagio();
    _fechamentoPedagio.Codigo.val(fechamentoPedagioGrid.Codigo);

    // Busca os Pedagios a partir do fechamento
    _gridFechamentoPedagio.CarregarGrid(function () {
        PreecherDadosPedagios(function () {
            //finalizarControleManualRequisicao();
            $("#myTab a:eq(1)").tab("show");
            _pesquisaFechamentoPedagio.ExibirFiltros.visibleFade(false);
        });
    });
}

function desativarEditarGridFuncionarioPedagio() {
    var editarColuna = { permite: false, callback: null, atualizarRow: false };
    _gridPedagios.SetarEditarColunas(editarColuna);
}

function habilitarEditarGridFuncionarioPedagio() {
    var editarColuna = { permite: true, callback: callbackEditarColuna, atualizarRow: false };
    _gridPedagios.SetarEditarColunas(editarColuna);
}

function PreecherDadosPedagios(callback) {
    BuscarPorCodigo(_fechamentoPedagio, "FechamentoPedagio/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            $("#liPedagios").show();
            _fechamentoPedagio.Veiculo.enable(false);
            _fechamentoPedagio.DataInicio.enable(false);
            _fechamentoPedagio.DataFim.enable(false);

            ocultarCamposPadroes();

            _crudFechamentoPedagio.GerarNovoFechamento.visible(true);
            _crudFechamentoPedagio.Adicionar.visible(false);

            habilitarEditarGridFuncionarioPedagio();

            var carragerGridPedagios = false;
            var auxSituacao = _fechamentoPedagio.Situacao.val();

            // Pendente
            if (auxSituacao == EnumSituacaoFechamentoPedagio.Pendente) {
                _crudFechamentoPedagio.Cancelar.visible(true);
                _crudFechamentoPedagio.FinalizarFechamento.visible(true);
                carragerGridPedagios = true;

                // Finalizado
            } else if (auxSituacao == EnumSituacaoFechamentoPedagio.Finalizado) {
                desativarEditarGridFuncionarioPedagio();
                carragerGridPedagios = true;

                // Falha na geracao
            } else if (auxSituacao == EnumSituacaoFechamentoPedagio.FalhaNaGeracao) {
                _crudFechamentoPedagio.Cancelar.visible(true);
                _crudFechamentoPedagio.MandarGerarNovamente.visible(true);
                //_comissaoFuncionarioMotorista.MensagemFalhaGeracao.val(_comissaoFuncionario.MensagemFalhaGeracao.val());
                //_comissaoFuncionarioMotorista.Motorista.visibleFalha(true);

                // Aguardando Geracao ou Em Geracao
            } else if (auxSituacao == EnumSituacaoFechamentoPedagio.AgGeracao || auxSituacao == EnumSituacaoFechamentoPedagio.EmGeracao) {
                SetarPercentualProcessamento(_fechamentoPedagio.PercentualGerado.val());

                // Cancelada
            } else if (auxSituacao == EnumSituacaoFechamentoPedagio.Cancelada) {
                desativarEditarGridFuncionarioPedagio();
                carragerGridPedagios = true;
            }

            //validarPermissoesPersonalizadas();
            if (!carragerGridPedagios) {
                if (callback != null)
                    callback();
            } else {
                //_comissaoFuncionarioMotorista.Motorista.visible(true);
                carregarPedagios(callback);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}