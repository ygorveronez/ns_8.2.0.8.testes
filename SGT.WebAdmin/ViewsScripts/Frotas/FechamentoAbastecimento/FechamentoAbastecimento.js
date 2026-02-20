/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Equipamento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFechamentoAbastecimento.js" />
/// <reference path="Abastecimento.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridFechamentoAbastecimento;
var _fechamentoAbastecimento;
var _crudFechamentoAbastecimento;
var _pesquisaFechamentoAbastecimento;

var _situacaoFechamentoAbastecimento = [
    { text: "Todas", value: EnumSituacaoFechamentoAbastecimento.Todas },
    { text: "Pendente", value: EnumSituacaoFechamentoAbastecimento.Pendente },
    { text: "Finalizado", value: EnumSituacaoFechamentoAbastecimento.Finalizado },
];

var PesquisaFechamentoAbastecimento = function () {
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFechamentoAbastecimento.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Equipamento:", idBtnSearch: guid() });

    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoFechamentoAbastecimento.Todas), options: _situacaoFechamentoAbastecimento, def: EnumSituacaoFechamentoAbastecimento.Todas });
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

var FechamentoAbastecimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Fechamento = PropertyEntity({ idGrid: guid(), visibleFade: ko.observable(false) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Veículo:"), idBtnSearch: guid(), issue: 143, enable: ko.observable(true), required: ko.observable(false) });
    this.Posto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Posto:", idBtnSearch: guid(), issue: 143, enable: ko.observable(true) });
    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Equipamento:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoFechamentoAbastecimento.Pendente), options: _situacaoFechamentoAbastecimento, def: EnumSituacaoFechamentoAbastecimento.Pendente, text: "Situação do Fechamento: " });

    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.NaoAdicionar = PropertyEntity({ idGrid: guid(), getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array() });
};

var CRUDFechamentoAbastecimento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Gerar Fechamento", visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar o Fechamento", visible: ko.observable(false), enable: ko.observable(true) });

    this.GerarNovoFechamento = PropertyEntity({ eventClick: gerarNovoFechamentoClick, type: types.event, text: "Limpar (Gerar Novo Fechamento)", visible: ko.observable(false), enable: ko.observable(true) });
    this.FinalizarFechamento = PropertyEntity({ eventClick: finalizarFechamentoClick, type: types.event, text: "Finalizar o Fechamento", visible: ko.observable(false), enable: ko.observable(true) });
    this.ReabrirFechamento = PropertyEntity({ eventClick: reAbrirFechamentoClick, type: types.event, text: "Reabrir o Fechamento", visible: ko.observable(false), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadFechamentoAbastecimento() {
    // Pesqusia de fechamentos
    _pesquisaFechamentoAbastecimento = new PesquisaFechamentoAbastecimento();
    KoBindings(_pesquisaFechamentoAbastecimento, "knockoutPesquisaFechamentoAbastecimento", false, _pesquisaFechamentoAbastecimento.Pesquisar.id);

    // Aba Dados do fechamento
    _fechamentoAbastecimento = new FechamentoAbastecimento();
    KoBindings(_fechamentoAbastecimento, "knockoutDadosFechamento");

    // Controle dos botoes
    _crudFechamentoAbastecimento = new CRUDFechamentoAbastecimento();
    KoBindings(_crudFechamentoAbastecimento, "knockoutCRUDFechamentoAbastecimento");

    HeaderAuditoria("FechamentoAbastecimento", _fechamentoAbastecimento);

    new BuscarVeiculos(_pesquisaFechamentoAbastecimento.Veiculo);
    new BuscarEquipamentos(_pesquisaFechamentoAbastecimento.Equipamento);

    new BuscarVeiculos(_fechamentoAbastecimento.Veiculo);
    new BuscarClientes(_fechamentoAbastecimento.Posto, null, false, [EnumModalidadePessoa.Fornecedor]);
    new BuscarEquipamentos(_fechamentoAbastecimento.Equipamento);
    new BuscarTransportadores(_fechamentoAbastecimento.Empresa, null, null, null, null, null, null, null, null, null, true);

    if (_CONFIGURACAO_TMS.BloquearFechamentoAbastecimentoSemplaca) {
        _fechamentoAbastecimento.Veiculo.required(true);
        _fechamentoAbastecimento.Veiculo.text("*Veículo:");
    }

    if (_CONFIGURACAO_TMS.LimitarOperacaoPorEmpresa)
        _fechamentoAbastecimento.Empresa.visible(true);

    buscarFechamentoAbastecimento();
    loadAbastecimentos();
}

function visibleEditar(dataRow) {
    return EnumSituacaoFechamentoAbastecimento.Pendente == _fechamentoAbastecimento.Situacao.val();
}

function visibleAdicionar(dataRow) {
    return EnumSituacaoFechamentoAbastecimento.Pendente == _fechamentoAbastecimento.Situacao.val() && !dataRow.DT_Enable;
}

function visibleRemover(dataRow) {
    return EnumSituacaoFechamentoAbastecimento.Pendente == _fechamentoAbastecimento.Situacao.val() && dataRow.DT_Enable;
}

function finalizarFechamentoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja fechar os abastecimentos?", function () {
        Salvar(_fechamentoAbastecimento, "FechamentoAbastecimento/FecharAbastecimento", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Abastecimentos Fechados Com sucesso.");
                    limparCamposFechamentoAbastecimento();
                    _gridFechamentoAbastecimento.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
                _gridAbastecimentos.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function cancelarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar o fechamento?", function () {
        Salvar(_fechamentoAbastecimento, "FechamentoAbastecimento/CancelarFechamento", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelada com Sucesso.");
                    _gridFechamentoAbastecimento.CarregarGrid();
                    limparCamposFechamentoAbastecimento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function gerarNovoFechamentoClick(e) {
    limparCamposFechamentoAbastecimento();
}

function reAbrirFechamentoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja reabrir o fechamento?", function () {
        Salvar(_fechamentoAbastecimento, "FechamentoAbastecimento/ReabrirFechamento", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Reaberto com Sucesso.");
                    _gridAbastecimentos.CarregarGrid(function () {
                        PreecherDadosAbastecimentos(function () {
                            $("#myTab a:eq(0)").tab("show");
                        });
                    });
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function adicionarClick(e, sender) {
    if (ValidarCamposObrigatorios(_fechamentoAbastecimento)) {
        Salvar(_fechamentoAbastecimento, "FechamentoAbastecimento/GerarFechamentoAbastecimentos", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    _fechamentoAbastecimento.Codigo.val(arg.Data);
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado");
                    BuscarProcessamentosPendentes();
                    PreecherDadosAbastecimentos(function () {
                        $("#myTab a:eq(1)").tab("show");
                    });
                    _gridFechamentoAbastecimento.CarregarGrid();
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
    else
        exibirCamposObrigatorio();
}

//*******MÉTODOS*******

function limparCamposFechamentoAbastecimento() {
    _crudFechamentoAbastecimento.Adicionar.visible(true);
    ocultarCamposPadroes();

    SetarEnableCamposKnockout(_fechamentoAbastecimento, true);
    LimparDetalhesAbastecimentos();

    LimparCampos(_pesquisaAbastecimentos);
    LimparCampos(_fechamentoAbastecimento);
    _fechamentoAbastecimento.Fechamento.visibleFade(false);
    resetarTabs();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}

function ExibirErroDataRow(row, mensagem, tipoMensagem, titulo) {
    _gridAbastecimentos.DesfazerAlteracaoDataRow(row);
    exibirMensagem(tipoMensagem, titulo, mensagem);
}

function ocultarCamposPadroes() {
    _crudFechamentoAbastecimento.FinalizarFechamento.visible(false);
    _crudFechamentoAbastecimento.GerarNovoFechamento.visible(false);
    _crudFechamentoAbastecimento.Cancelar.visible(false);
    _crudFechamentoAbastecimento.ReabrirFechamento.visible(false);
}

function buscarFechamentoAbastecimento() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarFechamentoAbastecimento, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridFechamentoAbastecimento = new GridView(_pesquisaFechamentoAbastecimento.Pesquisar.idGrid, "FechamentoAbastecimento/Pesquisar", _pesquisaFechamentoAbastecimento, menuOpcoes, null);
    _gridFechamentoAbastecimento.CarregarGrid();
}

function editarFechamentoAbastecimento(fechamentoAbastecimentoGrid, callbackGrid) {
    // Limpa Dados e Lista
    limparCamposFechamentoAbastecimento();
    _fechamentoAbastecimento.Codigo.val(fechamentoAbastecimentoGrid.Codigo);
    // Busca os abastecimentos a partir do fechamento
    PreecherDadosAbastecimentos(function () {
        _pesquisaFechamentoAbastecimento.ExibirFiltros.visibleFade(false);
    });
}

function desativarEditarGridFuncionarioAbastecimento() {
    var editarColuna = { permite: false, callback: null, atualizarRow: false };
    _gridAbastecimentos.SetarEditarColunas(editarColuna);
}

function habilitarEditarGridFuncionarioAbastecimento() {
    var editarColuna = { permite: true, callback: callbackEditarColuna, atualizarRow: false };
    _gridAbastecimentos.SetarEditarColunas(editarColuna);
}

function PreecherDadosAbastecimentos(callback) {
    BuscarPorCodigo(_fechamentoAbastecimento, "FechamentoAbastecimento/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                $("#liAbastecimentos").show();
                SetarEnableCamposKnockout(_fechamentoAbastecimento, false);

                ocultarCamposPadroes();

                _crudFechamentoAbastecimento.GerarNovoFechamento.visible(true);
                _crudFechamentoAbastecimento.Adicionar.visible(false);

                /*_comissaoFuncionarioMotorista.Motorista.visible(false);
                _comissaoFuncionarioMotorista.Motorista.visibleGeracao(false);
                _comissaoFuncionarioMotorista.Motorista.visibleFalha(false);
    
                habilitarEditarGridDocumentos();*/
                habilitarEditarGridFuncionarioAbastecimento();

                var carragerGridAbastecimentos = false;
                var auxSituacao = _fechamentoAbastecimento.Situacao.val();

                // Pendente
                if (auxSituacao == EnumSituacaoFechamentoAbastecimento.Pendente) {
                    _crudFechamentoAbastecimento.Cancelar.visible(true);
                    _crudFechamentoAbastecimento.FinalizarFechamento.visible(true);
                    carragerGridAbastecimentos = true;

                    // Finalizado
                } else if (auxSituacao == EnumSituacaoFechamentoAbastecimento.Finalizado) {
                    _crudFechamentoAbastecimento.ReabrirFechamento.visible(true);
                    desativarEditarGridFuncionarioAbastecimento();
                    carragerGridAbastecimentos = true;

                    // Falha na geracao
                } else if (auxSituacao == EnumSituacaoFechamentoAbastecimento.FalhaNaGeracao) {
                    _crudFechamentoAbastecimento.Cancelar.visible(true);
                    _crudFechamentoAbastecimento.MandarGerarNovamente.visible(true);
                    //_comissaoFuncionarioMotorista.MensagemFalhaGeracao.val(_comissaoFuncionario.MensagemFalhaGeracao.val());
                    //_comissaoFuncionarioMotorista.Motorista.visibleFalha(true);

                    // Aguardando Geracao ou Em Geracao
                } else if (auxSituacao == EnumSituacaoFechamentoAbastecimento.AgGeracao || auxSituacao == EnumSituacaoFechamentoAbastecimento.EmGeracao) {
                    SetarPercentualProcessamento(_fechamentoAbastecimento.PercentualGerado.val());

                    // Cancelada
                } else if (auxSituacao == EnumSituacaoFechamentoAbastecimento.Cancelada) {
                    desativarEditarGridFuncionarioAbastecimento();
                    carragerGridAbastecimentos = true;
                }

                //validarPermissoesPersonalizadas();
                if (!carragerGridAbastecimentos) {
                    if (callback != null)
                        callback();
                } else {
                    //_comissaoFuncionarioMotorista.Motorista.visible(true);
                    carregarAbastecimentos(callback);
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}