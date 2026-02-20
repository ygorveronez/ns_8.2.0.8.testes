/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/TipoDespesaFinanceira.js" />
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
/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Enumeradores/EnumAnaliticoSintetico.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoMovimento.js" />
/// <reference path="../../Enumeradores/EnumTipoGeracaoMovimento.js" />
/// <reference path="../../Enumeradores/EnumFinalidadeTipoMovimento.js" />
/// <reference path="../../Enumeradores/EnumTipoLancamentoFinanceiroSemOrcamento.js" />
/// <reference path="../../Enumeradores/TipoConsolidacaoMovimentoFinanceiro.js" />
/// <reference path="../../Enumeradores/EnumMoedaCotacaoBancoCentral.js" />
/// <reference path="../RateioDespesaVeiculo/RateioDespesaVeiculo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridMovimentoFinanceiro;
var _movimentoFinanceiro;
var _CRUDMovimentoFinanceiro;
var _pesquisaMovimentoFinanceiro;
var _PermissoesPersonalizadas;
var _tipoLancamentoFinanceiroSemOrcamento;

var _SituacaoMovimento = [
    { text: "Todos", value: TipoConsolidacaoMovimentoFinanceiro.Todos },
    { text: "Consolidado", value: TipoConsolidacaoMovimentoFinanceiro.Consolidado },
    { text: "Não consolidado", value: TipoConsolidacaoMovimentoFinanceiro.NaoConsolidado }
]

var _TipoGeracao = [
    { text: "Manual", value: EnumTipoGeracaoMovimento.Manual },
    { text: "Automática", value: EnumTipoGeracaoMovimento.Automatica }
];

var PesquisaMovimentoFinanceiro = function () {
    this.Codigo = PropertyEntity({ text: "Código: ", val: ko.observable(""), getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, def: ko.observable("") });
    this.DataMovimentoInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataMovimentoFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataBase = PropertyEntity({ text: "Data Base: ", getType: typesKnockout.date });
    this.ValorMovimento = PropertyEntity({ text: "Valor: ", getType: typesKnockout.decimal });
    this.NumeroDocumento = PropertyEntity({ text: "Nº Documento: " });
    this.Observacao = PropertyEntity({ text: "Observação: " });

    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Movimento:", idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid() });
    this.PlanoDebito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Plano de Débito:", idBtnSearch: guid() });
    this.PlanoCredito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Plano de Crédito:", idBtnSearch: guid() });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid() });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa:", idBtnSearch: guid() });

    this.SituacaoMovimento = PropertyEntity({ val: ko.observable(TipoConsolidacaoMovimentoFinanceiro.Todos), options: _SituacaoMovimento, text: "Situação do Movimento: ", def: TipoConsolidacaoMovimentoFinanceiro.Todos });
    this.TipoDocumento = PropertyEntity({ val: ko.observable(EnumTipoDocumentoMovimento.Todos), options: EnumTipoDocumentoMovimento.obterOpcoesPesquisa(), text: "Tipo do Documento: ", def: EnumTipoDocumentoMovimento.Todos });
    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Todas), options: EnumMoedaCotacaoBancoCentral.obterOpcoesPesquisa(), def: EnumMoedaCotacaoBancoCentral.Todas, text: "Moeda: ", visible: ko.observable(false) });

    this.DataMovimentoInicial.dateRangeLimit = this.DataMovimentoFinal;
    this.DataMovimentoFinal.dateRangeInit = this.DataMovimentoInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMovimentoFinanceiro.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
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

var MovimentoFinanceiro = function () {
    this.Codigo = PropertyEntity({ text: "Código: ", val: ko.observable(""), getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, def: ko.observable(""), enable: false });
    this.DataMovimento = PropertyEntity({ text: "*Data: ", required: true, getType: typesKnockout.date });
    this.DataBase = PropertyEntity({ text: "*Data Base: ", required: true, getType: typesKnockout.date });
    this.ValorMovimento = PropertyEntity({ text: "*Valor: ", required: true, getType: typesKnockout.decimal });
    this.TipoDocumento = PropertyEntity({ val: ko.observable(EnumTipoDocumentoMovimento.Manual), options: EnumTipoDocumentoMovimento.obterOpcoes(), text: "*Tipo do Documento: ", def: EnumTipoDocumentoMovimento.Manual, required: true });
    this.NumeroDocumento = PropertyEntity({ text: "*Nº Documento: ", required: true });
    this.TipoGeracao = PropertyEntity({ val: ko.observable(EnumTipoGeracaoMovimento.Manual), options: _TipoGeracao, text: "Tipo Geração: ", def: EnumTipoGeracaoMovimento.Manual, required: false, visible: false });

    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: _CONFIGURACAO_TMS.ExigirTipoMovimentoLancamentoMovimentoFinanceiroManual ? "*Tipo de Movimento:" : "Tipo de Movimento:", idBtnSearch: guid(), required: _CONFIGURACAO_TMS.ExigirTipoMovimentoLancamentoMovimentoFinanceiroManual, enable: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid(), required: false, val: ko.observable(""), enable: ko.observable(false), cssClass: ko.observable("col col-xs-2 col-sm-12 col-md-12 col-lg-12") });
    this.PlanoDebito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Plano de Entrada:", idBtnSearch: guid(), required: true, val: ko.observable(""), enable: ko.observable(false) });
    this.PlanoCredito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Plano de Saída:", idBtnSearch: guid(), required: true, val: ko.observable(""), enable: ko.observable(false) });
    this.TipoDespesaFinanceira = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Despesa:", idBtnSearch: guid(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(false) });
    this.Observacao = PropertyEntity({ text: "Observação: ", required: false, maxlength: 500 });

    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), required: false, val: ko.observable(""), enable: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa:", idBtnSearch: guid(), required: false, val: ko.observable(""), enable: ko.observable(true) });

    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(true) });
    this.DataBaseCRT = PropertyEntity({ text: "Data Base CRT: ", required: false, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorMoedaCotacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ text: "Valor Original Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(false), visible: ko.observable(false) });

    this.MoedaCotacaoBancoCentral.val.subscribe(function () {
        CalcularMoedaEstrangeira();
    });

    this.DataBaseCRT.val.subscribe(function () {
        CalcularMoedaEstrangeira();
    });

    this.ValorMoedaCotacao.val.subscribe(function () {
        ConverterValor();
    });

    this.ValorOriginalMoedaEstrangeira.val.subscribe(function () {
        ConverterValorOriginalMoeda();
    });
};

var CRUDMovimentoFinanceiro = function () {
    this.Adicionar = PropertyEntity({ eventClick: salvarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: salvarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Limpar Campos", visible: ko.observable(true) });
    this.ImprimirRecibo = PropertyEntity({ eventClick: ImprimirReciboClick, type: types.event, text: "Imprimir Recibo", visible: ko.observable(false) });
    this.SalvarObservacao = PropertyEntity({ eventClick: salvarObservacaoClick, type: types.event, text: "Salvar Observação", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadMovimentoFinanceiro() {

    _movimentoFinanceiro = new MovimentoFinanceiro();
    KoBindings(_movimentoFinanceiro, "knockoutCadastroMovimentoFinanceiro");

    HeaderAuditoria("MovimentoFinanceiro", _movimentoFinanceiro);

    _pesquisaMovimentoFinanceiro = new PesquisaMovimentoFinanceiro();
    KoBindings(_pesquisaMovimentoFinanceiro, "knockoutPesquisaMovimentoFinanceiro", false, _pesquisaMovimentoFinanceiro.Pesquisar.id);

    _CRUDMovimentoFinanceiro = new CRUDMovimentoFinanceiro();
    KoBindings(_CRUDMovimentoFinanceiro, "knockoutCRUDCadastroMovimentoFinanceiro");

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.MovimentoFinanceiro_InformarCentroResultado, _PermissoesPersonalizadas)) {
        new BuscarCentroResultado(_movimentoFinanceiro.CentroResultado, "Selecione o Centro de Resultado", "Centros de Resultado", null, EnumAnaliticoSintetico.Analitico, _movimentoFinanceiro.TipoMovimento);
        _movimentoFinanceiro.CentroResultado.enable(true);
    }

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.MovimentoFinanceiro_InformarPlanoEntradaSaida, _PermissoesPersonalizadas) || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        new BuscarPlanoConta(_movimentoFinanceiro.PlanoDebito, "Selecione a Conta Analítica (Entrada)", "Contas Analíticas (Entrada)", null, EnumAnaliticoSintetico.Analitico);
        new BuscarPlanoConta(_movimentoFinanceiro.PlanoCredito, "Selecione a Conta Analítica (Saída)", "Contas Analíticas (Saída)", null, EnumAnaliticoSintetico.Analitico);
        _movimentoFinanceiro.PlanoDebito.enable(true);
        _movimentoFinanceiro.PlanoCredito.enable(true);
    }

    new BuscarTipoMovimento(_movimentoFinanceiro.TipoMovimento, null, null, RetornoTipoMovimento, null, EnumFinalidadeTipoMovimento.MovimentoFinanceiro);

    new BuscarCentroResultado(_pesquisaMovimentoFinanceiro.CentroResultado, "Selecione o Centro de Resultado", "Centros de Resultado", null, EnumAnaliticoSintetico.Analitico);
    new BuscarPlanoConta(_pesquisaMovimentoFinanceiro.PlanoDebito, "Selecione a Conta Analítica (Entrada)", "Contas Analíticas (Entrada)", null, EnumAnaliticoSintetico.Analitico);
    new BuscarPlanoConta(_pesquisaMovimentoFinanceiro.PlanoCredito, "Selecione a Conta Analítica (Saída)", "Contas Analíticas (Saída)", null, EnumAnaliticoSintetico.Analitico);
    new BuscarTipoMovimento(_pesquisaMovimentoFinanceiro.TipoMovimento, null, null, null, null, EnumFinalidadeTipoMovimento.MovimentoFinanceiro);

    new BuscarClientes(_pesquisaMovimentoFinanceiro.Pessoa);
    new BuscarGruposPessoas(_pesquisaMovimentoFinanceiro.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Ambos);

    new BuscarClientes(_movimentoFinanceiro.Pessoa, RetornoBuscarClientes);
    new BuscarGruposPessoas(_movimentoFinanceiro.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Ambos);
    new BuscarTipoDespesaFinanceira(_movimentoFinanceiro.TipoDespesaFinanceira);

    CarregaUsuarioLogado();

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        _pesquisaMovimentoFinanceiro.MoedaCotacaoBancoCentral.visible(true);
        _movimentoFinanceiro.MoedaCotacaoBancoCentral.visible(true);
        _movimentoFinanceiro.DataBaseCRT.visible(true);
        _movimentoFinanceiro.ValorMoedaCotacao.visible(true);
        _movimentoFinanceiro.ValorOriginalMoedaEstrangeira.visible(true);
    }

    if (_CONFIGURACAO_TMS.AtivarControleDespesas) {
        _movimentoFinanceiro.TipoDespesaFinanceira.visible(true);
        _movimentoFinanceiro.CentroResultado.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-6");
    }

    carregarDespesaVeiculo("conteudoDespesaVeiculo", buscarMovimentoFinanceiros);
}

function RetornoBuscarClientes(data) {
    _movimentoFinanceiro.Pessoa.val(data.Nome);
    _movimentoFinanceiro.Pessoa.codEntity(data.Codigo);
    if (data.CodigoGrupo > 0) {
        LimparCampoEntity(_movimentoFinanceiro.GrupoPessoa);
        _movimentoFinanceiro.GrupoPessoa.val(data.DescricaoGrupo);
        _movimentoFinanceiro.GrupoPessoa.codEntity(data.CodigoGrupo);
    }
}

function RetornoTipoMovimento(data) {
    if (data != null) {
        _movimentoFinanceiro.TipoMovimento.codEntity(data.Codigo);
        _movimentoFinanceiro.TipoMovimento.val(data.Descricao);

        if (data.CodigoDebito > 0) {
            _movimentoFinanceiro.PlanoDebito.codEntity(data.CodigoDebito);
            _movimentoFinanceiro.PlanoDebito.val(data.PlanoDebito);
        }
        if (data.CodigoCredito > 0) {
            _movimentoFinanceiro.PlanoCredito.codEntity(data.CodigoCredito);
            _movimentoFinanceiro.PlanoCredito.val(data.PlanoCredito);
        }
        if (data.CodigoResultado > 0) {
            _movimentoFinanceiro.CentroResultado.codEntity(data.CodigoResultado);
            _movimentoFinanceiro.CentroResultado.val(data.CentroResultado);
        } else {
            LimparCampoEntity(_movimentoFinanceiro.CentroResultado);
        }
    }
}

function ValidarPlanoOrcamentarioEmpresa(cb) {
    var data = {
        TipoMovimento: _movimentoFinanceiro.TipoMovimento.codEntity(),
        DataEmissao: _movimentoFinanceiro.DataMovimento.val()
    };

    executarReST("PlanoOrcamentario/ValidaPlanoOrcamentarioEmpresa", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != true && arg.Data.Mensagem != "") {
                if (arg.Data.TipoLancamentoFinanceiroSemOrcamento == EnumTipoLancamentoFinanceiroSemOrcamento.Avisar) {
                    exibirConfirmacao("Confirmação", arg.Data.Mensagem, cb);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            } else
                cb();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function ValidarMovimentoFinanceiroDuplicado(cb) {
    var data = {
        Codigo: _movimentoFinanceiro.Codigo.val(),
        DataMovimento: _movimentoFinanceiro.DataMovimento.val(),
        ValorMovimento: _movimentoFinanceiro.ValorMovimento.val(),
        NumeroDocumento: _movimentoFinanceiro.NumeroDocumento.val()
    };

    executarReST("MovimentoFinanceiro/ValidaMovimentoFinanceiroDuplicado", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (!arg.Data.MovimentoDuplicado)
                    cb();
                else {
                    exibirConfirmacao("Movimento Financeiro Duplicado", "Já existe um movimento lançado com a mesma Data, Valor e Número Documento. Tem certeza que deseja continuar?", cb);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function adicionarClick(e, sender) {
    Salvar(_movimentoFinanceiro, "MovimentoFinanceiro/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_CONFIGURACAO_TMS.AbrirRateioDespesaVeiculoAutomaticamente && !arg.Data.NaoGerarRateioDeDespesaPorVeiculo) {

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso, favor digite o lançamento de rateio.");
                    _gridMovimentoFinanceiro.CarregarGrid();
                    LimparCamposParcialmente();

                    LimparCamposRateioDespesa();
                    Global.abrirModal('divModalDespesaVeiculo');
                    _rateioDespesa.MovimentoFinanceiro.val(arg.Data.Codigo);
                    _rateioDespesa.NumeroDocumento.val(arg.Data.NumeroDocumento);
                    _rateioDespesa.TipoDocumento.val(arg.Data.TipoDocumento);
                    _rateioDespesa.Valor.val(arg.Data.Valor);
                    _rateioDespesa.Pessoa.val(arg.Data.Pessoa.Descricao);
                    _rateioDespesa.Pessoa.codEntity(arg.Data.Pessoa.Codigo);

                } else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                    _gridMovimentoFinanceiro.CarregarGrid();
                    LimparCamposParcialmente();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function ImprimirReciboClick(e, sender) {
    var data = { Codigo: _movimentoFinanceiro.Codigo.val(), MovimentoManual: true };
    executarReST("MovimentoFinanceiro/GerarRecibo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    })
}

function atualizarClick(e, sender) {
    if (_movimentoFinanceiro.TipoGeracao.val() == EnumTipoGeracaoMovimento.Automatica) {
        exibirMensagem(tipoMensagem.aviso, "Atenção", "Não é permitido a alteração de movimentos gerados automaticamente.");
        return;
    }

    Salvar(_movimentoFinanceiro, "MovimentoFinanceiro/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMovimentoFinanceiro.CarregarGrid();
                limparCamposMovimentoFinanceiro();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function salvarClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_movimentoFinanceiro);

    if (valido) {
        var _HandlePlanoOrcamentario = function () {
            var _HandleMovimentoFinanceiroDuplicado = function () {
                if (_movimentoFinanceiro.Codigo.val() > 0)
                    atualizarClick(e, sender);
                else
                    adicionarClick(e, sender);
            };

            if (_CONFIGURACAO_TMS.SolicitarConfirmacaoMovimentoFinanceiroDuplicado)
                ValidarMovimentoFinanceiroDuplicado(_HandleMovimentoFinanceiroDuplicado);
            else
                _HandleMovimentoFinanceiroDuplicado();
        };

        ValidarPlanoOrcamentarioEmpresa(_HandlePlanoOrcamentario);
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function excluirClick(e, sender) {
    if (_movimentoFinanceiro.TipoGeracao.val() == EnumTipoGeracaoMovimento.Automatica) {
        exibirMensagem(tipoMensagem.aviso, "Atenção", "Não é permitido a exclusão de movimentos gerados automaticamente.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir o movimento financeiro selecionado?", function () {
        ExcluirPorCodigo(_movimentoFinanceiro, "MovimentoFinanceiro/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMovimentoFinanceiro.CarregarGrid();
                    limparCamposMovimentoFinanceiro();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposMovimentoFinanceiro();
}

function salvarObservacaoClick(e) {
    exibirConfirmacao("Atenção!", "Deseja realmente alterar a observação do Movimento Financeiro " + _movimentoFinanceiro.Codigo.val() + "?", function () {
        executarReST("MovimentoFinanceiro/SalvarObservacao", {
            Codigo: _movimentoFinanceiro.Codigo.val(),
            Observacao: _movimentoFinanceiro.Observacao.val()
        }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Observação alterada com sucesso.");
                    _gridMovimentoFinanceiro.CarregarGrid();
                    limparCamposMovimentoFinanceiro();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

//*******MÉTODOS*******

function CalcularMoedaEstrangeira() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira && _movimentoFinanceiro.DataBaseCRT.val() != null && _movimentoFinanceiro.DataBaseCRT.val() != undefined && _movimentoFinanceiro.DataBaseCRT.val() != "") {
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: _movimentoFinanceiro.MoedaCotacaoBancoCentral.val(), DataBaseCRT: _movimentoFinanceiro.DataBaseCRT.val() }, function (r) {
            if (r.Success) {
                if (r.Data != null && r.Data != undefined && r.Data > 0)
                    _movimentoFinanceiro.ValorMoedaCotacao.val(Globalize.format(r.Data, "n10"));
                ConverterValor();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function ConverterValor() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_movimentoFinanceiro.ValorMoedaCotacao.val());
        var valor = Globalize.parseFloat(_movimentoFinanceiro.ValorMovimento.val());
        if (valor > 0 && valorMoedaCotacao > 0) {
            _movimentoFinanceiro.ValorOriginalMoedaEstrangeira.val(Globalize.format(valor / valorMoedaCotacao, "n2"));
        }
        //else
            //_movimentoFinanceiro.ValorOriginalMoedaEstrangeira.val("0,00");
    }
}

function ConverterValorOriginalMoeda() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_movimentoFinanceiro.ValorMoedaCotacao.val());
        var valorOriginalMoeda = Globalize.parseFloat(_movimentoFinanceiro.ValorOriginalMoedaEstrangeira.val());
        if (valorOriginalMoeda > 0 && valorMoedaCotacao > 0) {
            _movimentoFinanceiro.ValorMovimento.val(Globalize.format(valorOriginalMoeda * valorMoedaCotacao, "n2"));
        }
        //else
            //_movimentoFinanceiro.ValorMovimento.val("0,00");
    }
}

function buscarMovimentoFinanceiros() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMovimentoFinanceiro, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "MovimentoFinanceiro/ExportarPesquisa",
        titulo: "Movimentação Financeira"
    };

    _gridMovimentoFinanceiro = new GridViewExportacao(_pesquisaMovimentoFinanceiro.Pesquisar.idGrid, "MovimentoFinanceiro/Pesquisa", _pesquisaMovimentoFinanceiro, menuOpcoes, configExportacao, { column: 0, dir: orderDir.desc });
    _gridMovimentoFinanceiro.CarregarGrid();
}

function editarMovimentoFinanceiro(MovimentoFinanceiroGrid) {
    limparCamposMovimentoFinanceiro();
    _movimentoFinanceiro.Codigo.val(MovimentoFinanceiroGrid.Codigo);
    BuscarPorCodigo(_movimentoFinanceiro, "MovimentoFinanceiro/BuscarPorCodigo", function (arg) {
        _pesquisaMovimentoFinanceiro.ExibirFiltros.visibleFade(false);
        _CRUDMovimentoFinanceiro.Atualizar.visible(true);
        _CRUDMovimentoFinanceiro.ImprimirRecibo.visible(true);
        _CRUDMovimentoFinanceiro.Excluir.visible(true);
        _CRUDMovimentoFinanceiro.Adicionar.visible(false);
        _CRUDMovimentoFinanceiro.SalvarObservacao.visible(true);
    }, null);
}

function limparCamposMovimentoFinanceiro() {
    _CRUDMovimentoFinanceiro.Atualizar.visible(false);
    _CRUDMovimentoFinanceiro.ImprimirRecibo.visible(false);
    _CRUDMovimentoFinanceiro.Excluir.visible(false);
    _CRUDMovimentoFinanceiro.Adicionar.visible(true);
    _CRUDMovimentoFinanceiro.SalvarObservacao.visible(false);
    LimparCampos(_movimentoFinanceiro);
    _movimentoFinanceiro.Codigo.val("");

    if (_tipoLancamentoFinanceiroSemOrcamento != EnumTipoLancamentoFinanceiroSemOrcamento.Liberar) {
        _movimentoFinanceiro.TipoMovimento.required(true);
        _movimentoFinanceiro.TipoMovimento.text("*Tipo de Movimento:");
    }
}

function LimparCamposParcialmente() {
    _CRUDMovimentoFinanceiro.Atualizar.visible(false);
    _CRUDMovimentoFinanceiro.ImprimirRecibo.visible(false);
    _CRUDMovimentoFinanceiro.Excluir.visible(false);
    _CRUDMovimentoFinanceiro.Adicionar.visible(true);
    _CRUDMovimentoFinanceiro.SalvarObservacao.visible(false);

    _movimentoFinanceiro.NumeroDocumento.val("");
    _movimentoFinanceiro.ValorMovimento.val("");
    _movimentoFinanceiro.Observacao.val("");
    _movimentoFinanceiro.Codigo.val("");
    LimparCampoEntity(_movimentoFinanceiro.GrupoPessoa);
    LimparCampoEntity(_movimentoFinanceiro.Pessoa);

    $("#" + _movimentoFinanceiro.ValorMovimento.id).focus();
}

function CarregaUsuarioLogado() {
    executarReST("Usuario/DadosUsuarioLogado", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false && arg.Data != null) {
                _tipoLancamentoFinanceiroSemOrcamento = arg.Data.TipoLancamentoFinanceiroSemOrcamento;

                if (_tipoLancamentoFinanceiroSemOrcamento != EnumTipoLancamentoFinanceiroSemOrcamento.Liberar) {
                    _movimentoFinanceiro.TipoMovimento.required(true);
                    _movimentoFinanceiro.TipoMovimento.text("*Tipo de Movimento:");
                }
            }
        }
    });
}