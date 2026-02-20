/// <reference path="../../Consultas/LocalArmazenamentoProduto.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOrdemServicoFrota.js" />
/// <reference path="../../Enumeradores/EnumTipoCargoFuncionario.js" />
/// <reference path="../../Enumeradores/EnumServicoVeiculoExecutado.js" />
/// <reference path="OrdemServico.js" />

////*******MAPEAMENTO KNOUCKOUT*******

var _fechamentoOrdemServico, _gridProdutosFechamentoOrdemServico, _gridDocumentosFechamentoOrdemServico, _produtoFechamentoOrdemServico, _gridServicosFechamentoOrdemServico;
var _tempoServicoExecucao, _gridServicosTempoExecucao;
var _observacaoServicoFechamento;

var FechamentoOrdemServico = function () {
    this.OrdemServico = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.DataFechamento = PropertyEntity({ val: ko.observable(""), def: "", text: "Data do Fechamento:", visible: ko.observable(false), getType: typesKnockout.dateTime });
    this.Operador = PropertyEntity({ val: ko.observable(""), def: "", text: "Operador:", visible: ko.observable(false) });
    this.ValorOrcado = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", text: "Valor Orçado:" });
    this.ValorRealizado = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", text: "Valor Realizado:" });
    this.DiferencaValorOrcadoRealizado = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", text: "Diferença Valor Orçado x Realizado:" });
    this.ValorTotal = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", text: "Valor Total:" });
    this.TipoOficina = PropertyEntity({ val: ko.observable(""), def: "", text: "Tipo da Oficina:" });

    this.Desconto = PropertyEntity({ text: "Desconto:", val: ko.observable(""), def: "", getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.DataFechamentoEditavel = PropertyEntity({ text: "Data de Fechamento:", getType: typesKnockout.dateTime, enable: ko.observable(true) });

    this.Atualizar = PropertyEntity({ eventClick: AtualizarFechamentoClick, type: types.event, text: "Salvar", icon: "fal fa-save", idGrid: guid(), visible: ko.observable(false) });

    this.CodigoBarras = PropertyEntity({ type: types.map, required: false, eventClick: LancarProdutoClick, enable: ko.observable(true) });
    this.AdicionarProduto = PropertyEntity({ eventClick: AbrirTelaProdutoFechamentoClick, type: types.event, text: "Adicionar Produto", icon: "fal fa-plus", idGrid: guid(), visible: ko.observable(false) });
    this.Produtos = PropertyEntity({ type: types.map, idGrid: guid(), visible: false });
    this.Servicos = PropertyEntity({ type: types.map, idGrid: guid(), visible: false });

    this.Documentos = PropertyEntity({ type: types.map, idGrid: guid(), visible: false });
};

var ProdutoFechamentoOrdemServico = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.OrdemServico = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, text: "*Valor:", val: ko.observable(""), def: "", required: true, enable: false });
    this.ValorUnitario = PropertyEntity({ getType: typesKnockout.decimal, text: "*Valor Unitário:", val: ko.observable(""), def: "", required: true, enable: false });
    this.Quantidade = PropertyEntity({ getType: typesKnockout.decimal, text: "*Quantidade:", val: ko.observable(""), def: "", required: true, enable: ko.observable(true) });

    this.Produto = PropertyEntity({ val: ko.observable(""), type: types.entity, codEntity: ko.observable(0), text: "*Produto:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.LerCodigoBarrasCamera = PropertyEntity({ type: types.map, text: "Abrir câmera:", required: false, eventClick: AbrirCamera, enable: ko.observable(true), visible: ko.observable(window.mobileAndTabletCheck()) });

    this.FinalidadeProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Finalidade do Produto:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local de Armazenamento:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.UtilizaMultiplosLocaisArmazenamento) });
    this.Origem = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(EnumTipoLancamento.Manual), def: EnumTipoLancamento.Manual });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarProdutoFechamentoClick, type: types.event, text: "Adicionar", icon: "fal fa-plus", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarProdutoFechamentoClick, type: types.event, text: "Atualizar", icon: "fal fa-save", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirProdutoFechamentoClick, type: types.event, text: "Excluir", icon: "fal fa-close", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarProdutoFechamentoClick, type: types.event, text: "Cancelar", icon: "fal fa-rotate-left", visible: ko.observable(true) });

    this.Quantidade.val.subscribe(function () {
        AtualizarValorTotalProdutoFechamento();
    });
};

var TempoServicoExecucao = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.OrdemServico = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Manutencao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Serviço:", idBtnSearch: guid(), required: true, enable: ko.observable(false) });
    this.Mecanico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Mecânico:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });

    this.Data = PropertyEntity({ getType: typesKnockout.date, text: "*Data:", val: ko.observable(""), def: "", required: true, enable: ko.observable(true) });
    this.HoraInicio = PropertyEntity({ getType: typesKnockout.time, text: "*Hora Inicial:", val: ko.observable(""), def: "", required: true, enable: ko.observable(true) });
    this.HoraFim = PropertyEntity({ getType: typesKnockout.time, text: "Hora Final:", val: ko.observable(""), def: "", required: false, enable: ko.observable(true) });
    this.Tempo = PropertyEntity({ getType: typesKnockout.int, text: "Tempo (min):", val: ko.observable(""), def: "", required: false, enable: ko.observable(false) });

    this.ServicosTempoExecucao = PropertyEntity({ type: types.map, idGrid: guid(), visible: false });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarTempoServicoExecucaoClick, type: types.event, text: "Adicionar", icon: "fal fa-plus", visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarTempoServicoExecucaoClick, type: types.event, text: "Atualizar", icon: "fal fa-save", visible: ko.observable(false), enable: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirTempoServicoExecucaoClick, type: types.event, text: "Excluir", icon: "fal fa-close", visible: ko.observable(false), enable: ko.observable(true) });
    this.Concluir = PropertyEntity({ eventClick: CancelarTempoServicoExecucaoClick, type: types.event, text: "Concluir", icon: "fal fa-rotate-left", visible: ko.observable(true) });

    this.HoraInicio.val.subscribe(function () {
        AtualizarTempoServico();
    });
    this.HoraFim.val.subscribe(function () {
        AtualizarTempoServico();
    });
};

var ObservacaoServicoFechamento = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Observacao = PropertyEntity({ text: "*Observação:", val: ko.observable(""), def: "", maxlength: 2000, required: true, enable: ko.observable(true) });

    this.Salvar = PropertyEntity({ eventClick: SalvarObservacaoServicoFechamentoClick, type: types.event, text: "Salvar", enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarObservacaoServicoFechamentoClick, type: types.event, text: "Cancelar" });
};

var LiberacaoManutencao = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Motivo = PropertyEntity({ text: "Motivo:", val: ko.observable(""), def: "", required: ko.observable(false), maxlength: 250 });

    this.Confirmar = PropertyEntity({ eventClick: LiberarVeiculoDaManutencaoClick, type: types.event, text: "Confirmar", icon: "fal fa-chevron-down", idGrid: guid() });
    this.Limpar = PropertyEntity({ eventClick: FecharLiberacaoManutencaoClick, type: types.event, text: "Voltar", icon: "fal fa-circle", idGrid: guid() });
};

////*******EVENTOS*******

function LoadFechamentoOrdemServico() {
    _fechamentoOrdemServico = new FechamentoOrdemServico();
    KoBindings(_fechamentoOrdemServico, "knockoutFechamento");

    _produtoFechamentoOrdemServico = new ProdutoFechamentoOrdemServico();
    KoBindings(_produtoFechamentoOrdemServico, "knockoutProdutoFechamento");

    _tempoServicoExecucao = new TempoServicoExecucao();
    KoBindings(_tempoServicoExecucao, "knockoutTempoServicoExecucao");

    _observacaoServicoFechamento = new ObservacaoServicoFechamento();
    KoBindings(_observacaoServicoFechamento, "knockoutObservacaoServicoFechamento");

    _liberacaoManutencao = new LiberacaoManutencao();
    KoBindings(_liberacaoManutencao, "knockoutLiberacaoManutencao");

    new BuscarFuncionario(_tempoServicoExecucao.Mecanico, null, null, null, null, [EnumTipoCargoFuncionario.Mecanico]);

    new BuscarFinalidadeProdutoOrdemServico(_produtoFechamentoOrdemServico.FinalidadeProduto);
    new BuscarProdutoTMSComEstoqueAgrupado(_produtoFechamentoOrdemServico.Produto, RetornoConultaProdutoFechamento);
    new BuscarLocalArmazenamentoProduto(_produtoFechamentoOrdemServico.LocalArmazenamento);

    CarregarGridDocumentosFechamento();
    CarregarGridServicosTempoExecucao();

    _fechamentoOrdemServico.CodigoBarras.get$()
        .on("keydown", function (e) {
            var ENTER_KEY = 13;
            var key = e.which || e.keyCode || 0;
            if (key === ENTER_KEY)
                LancarProdutoClick();
        });
}

function LancarProdutoClick(e, sender) {
    setTimeout(function () {
        var erro = false;
        var codigoBarrasLocalizar = _fechamentoOrdemServico.CodigoBarras.val();

        if (codigoBarrasLocalizar == "") {
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Código de barras não informado.");
            LimparCampoCodigoBarrasSetFocus()
            erro = true;
            return;
        }

        if (!erro) {
            var data = {
                CodigoBarrasLocalizar: codigoBarrasLocalizar
            };

            executarReST("FechamentoOrdemServico/RetornarProdutoPorCodigoBarras", data, function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        PreencherObjetoKnout(_produtoFechamentoOrdemServico, { Data: arg.Data });
                        _produtoFechamentoOrdemServico.OrdemServico.val(_ordemServico.Codigo.val());
                        AdicionarProdutoFechamentoClick();
                        LimparCampoCodigoBarrasSetFocus();
                    }
                    else {
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                        LimparCampoCodigoBarrasSetFocus();
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    LimparCampoCodigoBarrasSetFocus();
                }
            });
        }
    }, 100);
}

function LimparCampoCodigoBarrasSetFocus() {
    _fechamentoOrdemServico.CodigoBarras.val("");
    _fechamentoOrdemServico.CodigoBarras.get$().focus();
}

function AbrirCamera() {
    LerQRCodeCamera();
}

function AdicionarTempoServicoExecucaoClick(e, sender) {
    Salvar(_tempoServicoExecucao, "FechamentoOrdemServico/AdicionarTempoServicoExecucao", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Dados salvos com sucesso!");
                _tempoServicoExecucao.Atualizar.visible(false);
                _tempoServicoExecucao.Excluir.visible(false);
                _tempoServicoExecucao.Adicionar.visible(true);

                var codigoOrdemServico = _tempoServicoExecucao.OrdemServico.val();
                var servico = _tempoServicoExecucao.Servico.val();
                var codigoServico = _tempoServicoExecucao.Servico.codEntity();
                var codigoManutencao = _tempoServicoExecucao.Manutencao.val();

                LimparCampos(_tempoServicoExecucao);

                _tempoServicoExecucao.Codigo.val(0);
                _tempoServicoExecucao.OrdemServico.val(codigoOrdemServico);
                _tempoServicoExecucao.Manutencao.val(codigoManutencao);
                _tempoServicoExecucao.Servico.val(servico);
                _tempoServicoExecucao.Servico.codEntity(codigoServico);
                _tempoServicoExecucao.Servico.enable(false);

                _gridServicosTempoExecucao.CarregarGrid();
                _gridServicosFechamentoOrdemServico.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function AtualizarTempoServicoExecucaoClick(e, sender) {
    Salvar(_tempoServicoExecucao, "FechamentoOrdemServico/AtualizarTempoServicoExecucao", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Dados salvos com sucesso!");
                _tempoServicoExecucao.Atualizar.visible(false);
                _tempoServicoExecucao.Excluir.visible(false);
                _tempoServicoExecucao.Adicionar.visible(true);

                var codigoOrdemServico = _tempoServicoExecucao.OrdemServico.val();
                var servico = _tempoServicoExecucao.Servico.val();
                var codigoServico = _tempoServicoExecucao.Servico.codEntity();
                var codigoManutencao = _tempoServicoExecucao.Manutencao.val();

                LimparCampos(_tempoServicoExecucao);

                _tempoServicoExecucao.Codigo.val(0);
                _tempoServicoExecucao.OrdemServico.val(codigoOrdemServico);
                _tempoServicoExecucao.Manutencao.val(codigoManutencao);
                _tempoServicoExecucao.Servico.val(servico);
                _tempoServicoExecucao.Servico.codEntity(codigoServico);
                _tempoServicoExecucao.Servico.enable(false);

                _gridServicosTempoExecucao.CarregarGrid();
                _gridServicosFechamentoOrdemServico.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function ExcluirTempoServicoExecucaoClick(e, sender) {
    Salvar(_tempoServicoExecucao, "FechamentoOrdemServico/ExcluirTempoServicoExecucao", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Dados excluidos com sucesso!");
                _tempoServicoExecucao.Atualizar.visible(false);
                _tempoServicoExecucao.Excluir.visible(false);
                _tempoServicoExecucao.Adicionar.visible(true);

                var codigoOrdemServico = _tempoServicoExecucao.OrdemServico.val();
                var servico = _tempoServicoExecucao.Servico.val();
                var codigoServico = _tempoServicoExecucao.Servico.codEntity();
                var codigoManutencao = _tempoServicoExecucao.Manutencao.val();

                LimparCampos(_tempoServicoExecucao);

                _tempoServicoExecucao.Codigo.val(0);
                _tempoServicoExecucao.OrdemServico.val(codigoOrdemServico);
                _tempoServicoExecucao.Manutencao.val(codigoManutencao);
                _tempoServicoExecucao.Servico.val(servico);
                _tempoServicoExecucao.Servico.codEntity(codigoServico);
                _tempoServicoExecucao.Servico.enable(false);

                _gridServicosTempoExecucao.CarregarGrid();
                _gridServicosFechamentoOrdemServico.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function CancelarTempoServicoExecucaoClick(e, sender) {
    Global.fecharModal('knockoutTempoServicoExecucao');
    LimparCampos(_tempoServicoExecucao);
}

function AtualizarTempoServico() {
    if (_tempoServicoExecucao.HoraInicio.val() != "" && _tempoServicoExecucao.HoraInicio.val() != null && _tempoServicoExecucao.HoraInicio.val() != undefined &&
        _tempoServicoExecucao.HoraFim.val() != "" && _tempoServicoExecucao.HoraFim.val() != null && _tempoServicoExecucao.HoraFim.val() != undefined) {

        var startTime = new Date('2012/10/09 ' + _tempoServicoExecucao.HoraInicio.val());
        var endTime = new Date('2012/10/09 ' + _tempoServicoExecucao.HoraFim.val());

        var difference = endTime.getTime() - startTime.getTime();
        var resultInMinutes = Math.round(difference / 60000);
        if (resultInMinutes > 0)
            _tempoServicoExecucao.Tempo.val(resultInMinutes);
        else
            _tempoServicoExecucao.Tempo.val(0);
    }
}

function AtualizarFechamentoClick(e, sender) {
    Salvar(_fechamentoOrdemServico, "FechamentoOrdemServico/Atualizar", function (r) {
        if (r.Success) {
            if (r.Data) {
                PreencherFechamentoOrdemServico(r.Data);
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Dados salvos com sucesso!");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function LiberarVeiculoDaManutencaoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente liberar o veículo/equipamento da manutenção?", function () {
        Salvar(_liberacaoManutencao, "FechamentoOrdemServico/LiberarVeiculoDaManutencao", function (r) {
            if (r.Success) {
                if (r.Data) {
                    PreencherObjetoKnout(_ordemServico, { Data: r.Data.OrdemServico });
                    PreecherResumoOrdemServico(r.Data.OrdemServico);
                    _CRUDOrdemServico.LiberarVeiculoDaManutencao.visible(false);
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Liberado da manutenção com sucesso!");
                    SetarEnableCamposKnockout(_ordemServico, true);
                    SetarEnableCamposKnockout(_servicoOrcamentoOrdemServico, true);
                    SetarEnableCamposKnockout(_aprovacao, true);
                    FecharLiberacaoManutencaoClick()
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function AbrirLiberacaoManutencaoClick(e, sender) {
    LimparCampos(_liberacaoManutencao);
    _liberacaoManutencao.Codigo.val(_ordemServico.Codigo.val());
    Global.abrirModal("knockoutLiberacaoManutencao");
}

function FecharLiberacaoManutencaoClick() {
    LimparCampos(_liberacaoManutencao);
    Global.fecharModal('knockoutLiberacaoManutencao');
}

function FecharOrdemServicoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente fechar a ordem de serviço?", function () {
        Salvar(_fechamentoOrdemServico, "FechamentoOrdemServico/Finalizar", function (r) {
            if (r.Success) {
                if (r.Data) {
                    PreencherObjetoKnout(_ordemServico, { Data: r.Data.OrdemServico });
                    PreecherResumoOrdemServico(r.Data.OrdemServico);
                    PreencherFechamentoOrdemServico(r.Data.Fechamento);
                    BloquearAlteracaoFechamento();
                    Etapa5Aprovada();
                    _CRUDOrdemServico.ReabrirOrdemServico.visible(true);
                    _CRUDOrdemServico.FecharOrdemServico.visible(false);
                    _CRUDOrdemServico.LiberarVeiculoDaManutencao.visible(false);
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Ordem de serviço finalizada com sucesso!");
                    SetarEnableCamposKnockout(_ordemServico, false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function ReabrirOrdemServicoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente reabrir a ordem de serviço?", function () {
        Salvar(_ordemServico, "FechamentoOrdemServico/Reabrir", function (r) {
            if (r.Success) {
                if (r.Data) {
                    PreencherObjetoKnout(_ordemServico, { Data: r.Data.OrdemServico });
                    PreecherResumoOrdemServico(r.Data.OrdemServico);
                    PreencherFechamentoOrdemServico(r.Data.Fechamento);
                    DesbloquearAlteracaoFechamento();
                    _CRUDOrdemServico.FecharOrdemServico.visible(false);
                    _CRUDOrdemServico.LiberarVeiculoDaManutencao.visible(false);
                    _CRUDOrdemServico.ReabrirOrdemServico.visible(false);
                    _ordemServico.Situacao.val(r.Data.Situacao);

                    SetarEnableCamposKnockout(_ordemServico, false);
                    SetarEtapaOrdemServico();

                    switch (_ordemServico.Situacao.val()) {
                        case EnumSituacaoOrdemServicoFrota.EmDigitacao:
                            _CRUDOrdemServico.ConfirmarExecucaoServicos.visible(true);
                            _CRUDOrdemServico.CancelarOrdemServico.visible(true);
                            _CRUDOrdemServico.DownloadOrdemServico.visible(true);
                            _CRUDOrdemServico.AutorizarOrdemServico.visible(false);
                            _CRUDOrdemServico.RejeitarOrdemServico.visible(false);
                            LiberarAlteracaoServicos();
                            Etapa3Desabilitada();
                            $("#" + _etapaOrdemServico.Etapa2.idTab).click();
                            break;
                        case EnumSituacaoOrdemServicoFrota.AgAutorizacao:
                            _CRUDOrdemServico.AutorizarOrdemServico.visible(true);
                            _CRUDOrdemServico.RejeitarOrdemServico.visible(true);
                            _CRUDOrdemServico.CancelarOrdemServico.visible(true);
                            _CRUDOrdemServico.DownloadOrdemServico.visible(true);
                            _CRUDOrdemServico.ReabrirOrdemServico.visible(true);
                            Etapa4Desabilitada();
                            $("#" + _etapaOrdemServico.Etapa3.idTab).click();
                            break;
                        case EnumSituacaoOrdemServicoFrota.EmManutencao:
                            _CRUDOrdemServico.ReabrirOrdemServico.visible(true);
                            _CRUDOrdemServico.CancelarOrdemServico.visible(true);
                            _CRUDOrdemServico.FecharOrdemServico.visible(true);
                            _CRUDOrdemServico.LiberarVeiculoDaManutencao.visible(true);
                            _CRUDOrdemServico.DownloadOrdemServico.visible(true);
                            break;
                        case EnumSituacaoOrdemServicoFrota.AgNotaFiscal:
                            _CRUDOrdemServico.ReabrirOrdemServico.visible(true);
                            _CRUDOrdemServico.CancelarOrdemServico.visible(true);
                            _CRUDOrdemServico.FecharOrdemServico.visible(true);
                            _CRUDOrdemServico.DownloadOrdemServico.visible(true);
                            break;
                        case EnumSituacaoOrdemServicoFrota.Finalizada:
                            _CRUDOrdemServico.ReabrirOrdemServico.visible(true);
                            _CRUDOrdemServico.DownloadOrdemServico.visible(true);
                            break;
                        default:
                            break;
                    }

                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Ordem de serviço reaberta com sucesso!");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function AbrirTelaProdutoFechamentoClick(e, sender) {
    LimparCamposProdutoFechamentoOrdemServico();
    _produtoFechamentoOrdemServico.OrdemServico.val(e.OrdemServico.val());
    Global.abrirModal("knockoutProdutoFechamento");
}

function AdicionarProdutoFechamentoClick(e, sender) {
    Salvar(_produtoFechamentoOrdemServico, "FechamentoOrdemServico/AdicionarProduto", function (r) {
        if (r.Success) {
            if (r.Data) {
                PreencherFechamentoOrdemServico(r.Data);
                FecharModalProdutoFechamento();
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Produto adicionado com sucesso!");
                _gridProdutosFechamentoOrdemServico.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function AtualizarProdutoFechamentoClick(e, sender) {
    Salvar(_produtoFechamentoOrdemServico, "FechamentoOrdemServico/AtualizarProduto", function (r) {
        if (r.Success) {
            if (r.Data) {
                PreencherFechamentoOrdemServico(r.Data);
                FecharModalProdutoFechamento();
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Produto alterado com sucesso!");
                _gridProdutosFechamentoOrdemServico.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function ExcluirProdutoFechamentoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o produto " + _produtoFechamentoOrdemServico.Produto.val() + "?", function () {
        ExcluirPorCodigo(_produtoFechamentoOrdemServico, "FechamentoOrdemServico/ExcluirProduto", function (r) {
            if (r.Success) {
                if (r.Data) {
                    PreencherFechamentoOrdemServico(r.Data);
                    FecharModalProdutoFechamento();
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Produto excluído com sucesso!");
                    _gridProdutosFechamentoOrdemServico.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function CancelarProdutoFechamentoClick(e, sender) {
    FecharModalProdutoFechamento();
}

function EditarProdutoFechamentoClick(produtoGrid) {
    LimparCamposProdutoFechamentoOrdemServico();
    _produtoFechamentoOrdemServico.Codigo.val(produtoGrid.Codigo);
    BuscarPorCodigo(_produtoFechamentoOrdemServico, "FechamentoOrdemServico/BuscarProdutoPorCodigo", function (r) {
        if (r.Success) {
            if (r.Data) {
                if ((_ordemServico.Situacao.val() === EnumSituacaoOrdemServicoFrota.EmManutencao || _ordemServico.Situacao.val() === EnumSituacaoOrdemServicoFrota.AgNotaFiscal)
                    && _fechamentoOrdemServico.TipoOficina.val() == EnumTipoOficina.Interna && _produtoFechamentoOrdemServico.Origem.val() == EnumTipoLancamento.Manual) {
                    _fechamentoOrdemServico.AdicionarProduto.visible(true);
                    _produtoFechamentoOrdemServico.Adicionar.visible(false);
                    _produtoFechamentoOrdemServico.Atualizar.visible(true);
                    _produtoFechamentoOrdemServico.Excluir.visible(true);
                    _produtoFechamentoOrdemServico.FinalidadeProduto.enable(true);
                    _produtoFechamentoOrdemServico.Produto.enable(true);
                    _produtoFechamentoOrdemServico.Quantidade.enable(true);
                    _produtoFechamentoOrdemServico.LocalArmazenamento.enable(true);
                } else {
                    _fechamentoOrdemServico.AdicionarProduto.visible(false);
                    _produtoFechamentoOrdemServico.Adicionar.visible(false);
                    _produtoFechamentoOrdemServico.Atualizar.visible(false);
                    _produtoFechamentoOrdemServico.Excluir.visible(false);
                    _produtoFechamentoOrdemServico.FinalidadeProduto.enable(false);
                    _produtoFechamentoOrdemServico.Produto.enable(false);
                    _produtoFechamentoOrdemServico.Quantidade.enable(false);
                    _produtoFechamentoOrdemServico.LocalArmazenamento.enable(false);
                }

                Global.abrirModal("knockoutProdutoFechamento");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function SalvarObservacaoServicoFechamentoClick() {
    Salvar(_observacaoServicoFechamento, "FechamentoOrdemServico/SalvarObservacaoFechamento", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Dados salvos com sucesso!");

                LimparCampos(_observacaoServicoFechamento);
                Global.fecharModal('divModalObservacaoServicoFechamento');

                _gridServicosFechamentoOrdemServico.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function CancelarObservacaoServicoFechamentoClick() {
    LimparCampos(_observacaoServicoFechamento);
    Global.fecharModal('divModalObservacaoServicoFechamento');
}

////*******METODOS*******

function RetornoConultaProdutoFechamento(produto) {
    _produtoFechamentoOrdemServico.Produto.val(produto.Descricao);
    _produtoFechamentoOrdemServico.Produto.codEntity(produto.Codigo);

    executarReST("Produto/BuscarCustoMedio", { Codigo: produto.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _produtoFechamentoOrdemServico.ValorUnitario.val(Globalize.format(r.Data.CustoMedio, "n2"));
                AtualizarValorTotalProdutoFechamento();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function AtualizarValorTotalProdutoFechamento() {
    var valorUnitario = Globalize.parseFloat(_produtoFechamentoOrdemServico.ValorUnitario.val());
    var quantidade = Globalize.parseFloat(_produtoFechamentoOrdemServico.Quantidade.val());

    if (isNaN(valorUnitario))
        valorUnitario = 0;
    if (isNaN(quantidade))
        quantidade = 0;

    var valorTotal = valorUnitario * quantidade;

    _produtoFechamentoOrdemServico.Valor.val(Globalize.format(valorTotal, "n2"));
}

function FecharModalProdutoFechamento() {
    LimparCamposProdutoFechamentoOrdemServico();
    Global.fecharModal('knockoutProdutoFechamento');
}

function CarregarGridProdutosFechamento() {
    if (_gridProdutosFechamentoOrdemServico != null) {
        _gridProdutosFechamentoOrdemServico.Destroy();
        _gridProdutosFechamentoOrdemServico = null;
    }

    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarProdutoFechamentoClick, tamanho: "5", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

    _gridProdutosFechamentoOrdemServico = new GridView(_fechamentoOrdemServico.Produtos.idGrid, "FechamentoOrdemServico/ConsultarProdutos", _fechamentoOrdemServico, menuOpcoes, { column: 0, dir: orderDir.desc }, 5);
    _gridProdutosFechamentoOrdemServico.CarregarGrid();
}

function CarregarGridServicosTempoExecucao() {
    if (_gridServicosTempoExecucao != null) {
        _gridServicosTempoExecucao.Destroy();
        _gridServicosTempoExecucao = null;
    }

    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarServicosTempoExecucaoClick, tamanho: "15", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

    _gridServicosTempoExecucao = new GridView(_tempoServicoExecucao.ServicosTempoExecucao.idGrid, "FechamentoOrdemServico/ConsultarServicosTempoExecucao", _tempoServicoExecucao, menuOpcoes, { column: 0, dir: orderDir.desc }, 5);
    _gridServicosTempoExecucao.CarregarGrid();
}

function EditarServicosTempoExecucaoClick(data) {
    _tempoServicoExecucao.Codigo.val(data.Codigo);
    BuscarPorCodigo(_tempoServicoExecucao, "FechamentoOrdemServico/BuscarTempoServicoExecucaoPorCodigo", function (r) {
        if (r.Success) {
            if (r.Data) {
                _gridServicosTempoExecucao.CarregarGrid();
                _tempoServicoExecucao.Atualizar.visible(true);
                _tempoServicoExecucao.Excluir.visible(true);
                _tempoServicoExecucao.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function VisibilidadeInformarServicoExecutado(data) {
    return data.ServicoConcluido !== EnumServicoVeiculoExecutado.Executado && (_ordemServico.Situacao.val() === EnumSituacaoOrdemServicoFrota.EmManutencao || _ordemServico.Situacao.val() === EnumSituacaoOrdemServicoFrota.AgNotaFiscal);
}

function VisibilidadeInformarServicoNaoExecutado(data) {
    return data.ServicoConcluido !== EnumServicoVeiculoExecutado.NaoExecutado && (_ordemServico.Situacao.val() === EnumSituacaoOrdemServicoFrota.EmManutencao || _ordemServico.Situacao.val() === EnumSituacaoOrdemServicoFrota.AgNotaFiscal);
}

function VisibilidadeLimparServicoExecutado(data) {
    return data.ServicoConcluido !== EnumServicoVeiculoExecutado.NaoDefinido && (_ordemServico.Situacao.val() === EnumSituacaoOrdemServicoFrota.EmManutencao || _ordemServico.Situacao.val() === EnumSituacaoOrdemServicoFrota.AgNotaFiscal);
}

function CarregarGridServicosFechamento() {
    if (_gridServicosFechamentoOrdemServico != null) {
        _gridServicosFechamentoOrdemServico.Destroy();
        _gridServicosFechamentoOrdemServico = null;
    }

    var editar = { descricao: "Tempo", id: guid(), metodo: EditarServicoFechamentoClick };
    var observacao = { descricao: "Observação", id: guid(), metodo: ObservacaoServicoFechamentoClick };
    var servicoExecutado = { descricao: "Serviço Executado?", id: guid(), metodo: InformarServicoExecutadoClick, visibilidade: VisibilidadeInformarServicoExecutado };
    var servicoNaoExecutado = { descricao: "Serviço não Executado?", id: guid(), metodo: InformarServicoNaoExecutadoClick, visibilidade: VisibilidadeInformarServicoNaoExecutado };
    var limparInfoServico = { descricao: "Limpar Info. Serviço Executado?", id: guid(), metodo: LimparInformacaoServicoConcluidoClick, visibilidade: VisibilidadeLimparServicoExecutado };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [editar, observacao, servicoExecutado, servicoNaoExecutado, limparInfoServico] };

    _gridServicosFechamentoOrdemServico = new GridView(_fechamentoOrdemServico.Servicos.idGrid, "FechamentoOrdemServico/ConsultarServicos", _fechamentoOrdemServico, menuOpcoes, { column: 0, dir: orderDir.desc }, 5);
    _gridServicosFechamentoOrdemServico.CarregarGrid();
}

function EditarServicoFechamentoClick(data) {
    LimparCampos(_tempoServicoExecucao);
    _tempoServicoExecucao.Codigo.val(0);
    _tempoServicoExecucao.OrdemServico.val(data.CodigoOrdemServico);
    _tempoServicoExecucao.Manutencao.val(data.Codigo);

    _tempoServicoExecucao.Servico.val(data.Servico);
    _tempoServicoExecucao.Servico.codEntity(data.CodigoServico);
    _tempoServicoExecucao.Servico.enable(false);

    _tempoServicoExecucao.Adicionar.visible(true);
    _tempoServicoExecucao.Atualizar.visible(false);
    _tempoServicoExecucao.Excluir.visible(false);

    _gridServicosTempoExecucao.CarregarGrid();
    Global.abrirModal("knockoutTempoServicoExecucao");
}

function ObservacaoServicoFechamentoClick(data) {
    LimparCampos(_observacaoServicoFechamento);
    _observacaoServicoFechamento.Codigo.val(data.Codigo);
    _observacaoServicoFechamento.Observacao.val(data.ObservacaoFechamento);
    Global.abrirModal("divModalObservacaoServicoFechamento");
}

function InformarServicoExecutadoClick(data) {
    exibirConfirmacao("Atenção!", "O serviço " + data.Servico + " realmente foi executado?", function () {
        InformarServicoConcluido(data.Codigo, EnumServicoVeiculoExecutado.Executado);
    });
}

function InformarServicoNaoExecutadoClick(data) {
    exibirConfirmacao("Atenção!", "O serviço " + data.Servico + " realmente NÃO foi executado?", function () {
        InformarServicoConcluido(data.Codigo, EnumServicoVeiculoExecutado.NaoExecutado);
    });
}

function LimparInformacaoServicoConcluidoClick(data) {
    exibirConfirmacao("Atenção!", "Deseja realmente limpar a informação de execução do serviço " + data.Servico + "?", function () {
        InformarServicoConcluido(data.Codigo, EnumServicoVeiculoExecutado.NaoDefinido);
    });
}

function InformarServicoConcluido(codigo, enumerador) {
    executarReST("FechamentoOrdemServico/InformarServicoConcluido", { Codigo: codigo, ServicoVeiculoExecutado: enumerador }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Informado com sucesso!");
                _gridServicosFechamentoOrdemServico.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function CarregarGridDocumentosFechamento() {
    _gridDocumentosFechamentoOrdemServico = new GridView(_fechamentoOrdemServico.Documentos.idGrid, "FechamentoOrdemServico/ConsultarDocumentos", _fechamentoOrdemServico, null, { column: 0, dir: orderDir.desc }, 5);
}

function CarregarFechamentoOrcamento() {
    LimparCamposFechamentoOrdemServico();

    executarReST("FechamentoOrdemServico/ObterDetalhesGeraisFechamento", { Codigo: _ordemServico.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {

                PreencherFechamentoOrdemServico(r.Data);

                CarregarGridProdutosFechamento();
                CarregarGridServicosFechamento();
                _gridDocumentosFechamentoOrdemServico.CarregarGrid();

                if (_ordemServico.Situacao.val() !== EnumSituacaoOrdemServicoFrota.EmManutencao && _ordemServico.Situacao.val() !== EnumSituacaoOrdemServicoFrota.AgNotaFiscal) {
                    BloquearAlteracaoFechamento();
                } else {
                    if (_fechamentoOrdemServico.TipoOficina.val() == EnumTipoOficina.Interna)
                        _fechamentoOrdemServico.AdicionarProduto.visible(true);
                    else
                        _fechamentoOrdemServico.AdicionarProduto.visible(false);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function PreencherFechamentoOrdemServico(dados) {
    PreencherObjetoKnout(_fechamentoOrdemServico, { Data: dados });

    if (_ordemServico.Situacao.val() === EnumSituacaoOrdemServicoFrota.Finalizada) {
        _fechamentoOrdemServico.DataFechamento.visible(true);
        _fechamentoOrdemServico.Operador.visible(true);
        $("#liCamposEditaveisFechamento").hide();
    } else {
        _fechamentoOrdemServico.DataFechamento.visible(false);
        _fechamentoOrdemServico.Operador.visible(false);
        $("#liCamposEditaveisFechamento").show();
    }
}

function BloquearAlteracaoFechamento() {
    SetarEnableCamposKnockout(_fechamentoOrdemServico, false);
    SetarEnableCamposKnockout(_tempoServicoExecucao, false);
    SetarEnableCamposKnockout(_observacaoServicoFechamento, false);
    $("#liCamposEditaveisFechamento").hide();

    _fechamentoOrdemServico.Atualizar.visible(false);
    _fechamentoOrdemServico.AdicionarProduto.visible(false);
}

function DesbloquearAlteracaoFechamento() {
    SetarEnableCamposKnockout(_fechamentoOrdemServico, true);
    SetarEnableCamposKnockout(_tempoServicoExecucao, true);
    SetarEnableCamposKnockout(_observacaoServicoFechamento, true);
    _tempoServicoExecucao.Servico.enable(false);
    _tempoServicoExecucao.Tempo.enable(false);

    _fechamentoOrdemServico.Atualizar.visible(true);
    $("#liCamposEditaveisFechamento").show();

    if ((_ordemServico.Situacao.val() === EnumSituacaoOrdemServicoFrota.EmManutencao || _ordemServico.Situacao.val() === EnumSituacaoOrdemServicoFrota.AgNotaFiscal)
        && _fechamentoOrdemServico.TipoOficina.val() == EnumTipoOficina.Interna)
        _fechamentoOrdemServico.AdicionarProduto.visible(true);
}

function LimparCamposFechamentoOrdemServico() {
    SetarEnableCamposKnockout(_fechamentoOrdemServico, true);
    SetarEnableCamposKnockout(_tempoServicoExecucao, true);
    SetarEnableCamposKnockout(_observacaoServicoFechamento, true);
    _tempoServicoExecucao.Servico.enable(false);
    _tempoServicoExecucao.Tempo.enable(false);

    LimparCampos(_fechamentoOrdemServico);

    _fechamentoOrdemServico.Atualizar.visible(true);
}

function LimparCamposProdutoFechamentoOrdemServico() {
    LimparCampos(_produtoFechamentoOrdemServico);
    _produtoFechamentoOrdemServico.Adicionar.visible(true);
    _produtoFechamentoOrdemServico.Atualizar.visible(false);
    _produtoFechamentoOrdemServico.Excluir.visible(false);
}

window.mobileAndTabletCheck = function () {
    let check = false;
    (function (a) { if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino|android|ipad|playbook|silk/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4))) check = true; })(navigator.userAgent || navigator.vendor || window.opera);
    return check;
};