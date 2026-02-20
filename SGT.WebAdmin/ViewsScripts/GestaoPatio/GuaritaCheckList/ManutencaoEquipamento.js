/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoOrdemServico.js" />
/// <reference path="../../Consultas/ServicoVeiculo.js" />
/// <reference path="../../Consultas/Equipamento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoGuaritaCheckList.js" />
/// <reference path="../../Enumeradores/EnumTipoManutencaoServicoVeiculoOrdemServicoFrota.js" />
/// <reference path="GuaritaCheckList.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _guaritaCheckListManutencaoEquipamento, _adicionarServicoCheckListManutencaoEquipamento;

var GuaritaCheckListManutencaoEquipamento = function () {
    this.NumeroOrdemServico = PropertyEntity({ text: "OS Equipamento: ", enable: ko.observable(false), required: ko.observable(false) });
    this.GerarOS = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Deseja gerar uma Ordem de Serviço para o equipamento com os serviços lançados?", enable: ko.observable(true), visible: ko.observable(true) });
    this.DataProgramada = PropertyEntity({ text: "*Data Programada: ", getType: typesKnockout.date, type: types.date, enable: ko.observable(true), required: ko.observable(false) });
    this.ObservacaoOS = PropertyEntity({ text: "Observação para a OS:", maxlength: 1000, val: ko.observable(""), enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: _guaritaCheckList.Situacao.val, options: _guaritaCheckList.Situacao.options, def: _guaritaCheckList.Situacao.def, text: "Situação:", enable: ko.observable(true) });

    this.EquipamentoServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Equipamento:", idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.TipoOrdemServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo da OS:", idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.LocalManutencao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local da Manutenção:", idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });

    this.Servicos = ko.observableArray();

    this.AdicionarManutencao = PropertyEntity({ eventClick: AdicionarManutencaoEquipamentoClick, type: types.event, text: "Adicionar Manutenção", icon: "fal fa-plus", id: guid(), visible: ko.observable(true) });
    this.BuscarServicos = PropertyEntity({ eventClick: BuscarServicosEquipamentoClick, type: types.event, text: "Buscar Serviços", icon: "fal fa-gear", id: guid(), visible: ko.observable(true) });
};

var AdicionarServicoCheckListManutencaoEquipamento = function () {
    this.GuaritaCheckList = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Equipamento = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Serviço:", idBtnSearch: guid(), required: true });
    this.UltimaManutencao = PropertyEntity({ maxlength: 1000, text: "Última Manutenção:", visible: ko.observable(false) });
    this.TempoEstimado = PropertyEntity({ maxlength: 6, text: "Tempo Estimado (min):", visible: ko.observable(true), getType: typesKnockout.int, val: ko.observable(0) });
    this.Observacao = PropertyEntity({ maxlength: 1000, text: "Observação:" });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarServicoEquipamentoClick, type: types.event, text: "Adicionar", icon: "fal fa-plus", idGrid: guid() });

    this.Servico.codEntity.subscribe(function (novoValor) {
        if (novoValor == null || novoValor == 0) {
            _adicionarServicoCheckListManutencao.UltimaManutencao.val("");
            _adicionarServicoCheckListManutencao.UltimaManutencao.visible(false);
        }
    });

    this.Servico.val.subscribe(function (novoValor) {
        if (novoValor == null || novoValor.trim() == "")
            _adicionarServicoCheckListManutencao.Servico.codEntity(0);
    });
};

var ManutencaoEquipamentoGuaritaCheckList = function (dados) {
    var editar = false;
    if (_guaritaCheckList.Situacao.val() == EnumSituacaoGuaritaCheckList.Aberto)
        editar = true;

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(dados.Codigo), def: dados.Codigo });
    this.Servico = PropertyEntity({ text: "Serviço:", val: ko.observable(dados.Servico.Descricao) });
    this.CustoMedio = PropertyEntity({ text: "Custo Médio:", val: ko.observable(Globalize.format(dados.CustoMedio, "n2")) });
    this.CustoEstimado = PropertyEntity({ getType: typesKnockout.decimal, text: "Custo Estimado:", val: ko.observable(Globalize.format(dados.CustoEstimado, "n2")), enable: ko.observable(editar) });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable(dados.Observacao), enable: ko.observable(editar) });
    this.TipoManutencao = PropertyEntity({ text: ko.observable(dados.DescricaoTipoManutencao), val: ko.observable(dados.TipoManutencao) });

    var ultimaManutencao = "";;
    if (dados.DataUltimaManutencao != null && dados.DataUltimaManutencao != "")
        ultimaManutencao = dados.QuilometragemUltimaManutencao + "km em " + dados.DataUltimaManutencao;
    else
        ultimaManutencao = "Nenhuma realizada";

    this.UltimaManutencao = PropertyEntity({ text: "Última Manutenção:", val: ko.observable(ultimaManutencao) });
    this.TempoEstimado = PropertyEntity({ text: "Tempo Estimado (min):", visible: ko.observable(true), val: ko.observable(Globalize.format(dados.TempoEstimado, "n0")) });

    var textoRibbon = "Corretiva",
        classeRibbon = "ribbon-tms ribbon-tms-red";

    if (dados.TipoManutencao == EnumTipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva) {
        classeRibbon = "ribbon-tms ribbon-tms-green";
        textoRibbon = "Preventiva";
    }

    this.Ribbon = PropertyEntity({ text: textoRibbon, cssClass: classeRibbon, visible: true });

    this.Atualizar = PropertyEntity({ eventClick: AtualizarManutencaoEquipamentoClick, type: types.event, text: "Salvar", icon: "fal fa-save", id: guid(), visible: ko.observable(editar) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirManutencaoEquipamentoClick, type: types.event, text: "Excluir", icon: "fal fa-times", id: guid(), visible: ko.observable(editar) });
};

//*******EVENTOS*******

function loadGuaritaCheckListManutencaoEquipamento() {
    _guaritaCheckListManutencaoEquipamento = new GuaritaCheckListManutencaoEquipamento();
    KoBindings(_guaritaCheckListManutencaoEquipamento, "knockoutGuaritaCheckListManutencaoEquipamento");

    _adicionarServicoCheckListManutencaoEquipamento = new AdicionarServicoCheckListManutencaoEquipamento();
    KoBindings(_adicionarServicoCheckListManutencaoEquipamento, "knockoutAdicionarServicoEquipamento");

    new BuscarClientes(_guaritaCheckListManutencaoEquipamento.LocalManutencao, null, null, [EnumModalidadePessoa.Fornecedor]);
    new BuscarTipoOrdemServico(_guaritaCheckListManutencaoEquipamento.TipoOrdemServico);
    new BuscarEquipamentos(_guaritaCheckListManutencaoEquipamento.EquipamentoServico);
    new BuscarServicoVeiculo(_adicionarServicoCheckListManutencaoEquipamento.Servico, RetornoConsultaServicoEquipamento);
}

function AdicionarManutencaoEquipamentoClick() {
    LimparCampos(_adicionarServicoCheckListManutencaoEquipamento);
    Global.abrirModal("knockoutAdicionarServicoEquipamento");
}

function BuscarServicosEquipamentoClick() {
    if (validaEquipamentoOperacoesController()) {
        exibirConfirmacao("Atenção!", "Deseja realmente buscar os serviços do equipamento?", function () {
            executarReST("GuaritaCheckListServico/BuscarServicosParaManutencaoEquipamento", { GuaritaCheckList: _guaritaCheckList.Codigo.val(), Equipamento: _guaritaCheckListManutencaoEquipamento.EquipamentoServico.codEntity() }, function (r) {
                if (r.Success) {
                    if (r.Data) {
                        BuscarManutencoesEquipamentoGuaritaCheckList();
                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
                }
            });
        });
    }
}

function AdicionarServicoEquipamentoClick() {
    if (validaEquipamentoOperacoesController()) {
        _adicionarServicoCheckListManutencaoEquipamento.GuaritaCheckList.val(_guaritaCheckList.Codigo.val());
        _adicionarServicoCheckListManutencaoEquipamento.Equipamento.val(_guaritaCheckListManutencaoEquipamento.EquipamentoServico.codEntity());

        Salvar(_adicionarServicoCheckListManutencaoEquipamento, "GuaritaCheckListServico/AdicionarEquipamento", function (r) {
            if (r.Success) {
                if (r.Data) {
                    AdicionarManutencaoEquipamentoALista(r.Data);
                    Global.fecharModal('knockoutAdicionarServicoEquipamento');
                    LimparCampos(_adicionarServicoCheckListManutencaoEquipamento);
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Manutenção de Equipamento adicionada com sucesso!");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function AtualizarManutencaoEquipamentoClick(e, sender) {
    Salvar(e, "GuaritaCheckListServico/AtualizarEquipamento", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Serviço de Equipamento atualizado com sucesso!");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function ExcluirManutencaoEquipamentoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir a manutenção do equipamento " + e.Servico.val() + "?", function () {
        ExcluirPorCodigo(e, "GuaritaCheckListServico/ExcluirEquipamento", function (r) {
            if (r.Success) {
                if (r.Data) {
                    _guaritaCheckListManutencaoEquipamento.Servicos.remove(function (item) { return item.Codigo.val() == e.Codigo.val() });
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Serviço de Equipamento excluído com sucesso!");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function BuscarManutencoesEquipamentoGuaritaCheckList() {
    executarReST("GuaritaCheckListServico/BuscarPorGuaritaCheckListEquipamento", { GuaritaCheckList: _guaritaCheckList.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _guaritaCheckListManutencaoEquipamento.Servicos.removeAll();
                for (var i = 0; i < r.Data.length; i++)
                    AdicionarManutencaoEquipamentoALista(r.Data[i]);

                if (_guaritaCheckList.Situacao.val() != EnumSituacaoGuaritaCheckList.Aberto)
                    _guaritaCheckListManutencaoEquipamento.AdicionarManutencao.visible(false);
                BloquearBuscaNovosServicosEquipamento();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

//*******METODOS*******

function RetornoConsultaServicoEquipamento(dados) {
    if (validaEquipamentoOperacoesController()) {
        _adicionarServicoCheckListManutencaoEquipamento.Servico.codEntity(dados.Codigo);
        _adicionarServicoCheckListManutencaoEquipamento.Servico.val(dados.Descricao);
        _adicionarServicoCheckListManutencaoEquipamento.TempoEstimado.val(dados.TempoEstimado);

        executarReST("GuaritaCheckListServico/BuscarDadosUltimaExecucaoEquipamento", {
            GuaritaCheckList: _guaritaCheckList.Codigo.val(), Servico: dados.Codigo,
            Equipamento: _guaritaCheckListManutencaoEquipamento.EquipamentoServico.codEntity()
        }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _adicionarServicoCheckListManutencaoEquipamento.UltimaManutencao.visible(true);
                    if (r.Data.Codigo > 0)
                        _adicionarServicoCheckListManutencaoEquipamento.UltimaManutencao.val(r.Data.Quilometragem + "km / " + r.Data.Data);
                    else
                        _adicionarServicoCheckListManutencaoEquipamento.UltimaManutencao.val("Não há manutenção realizada com este serviço para este equipamento.");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function AdicionarManutencaoEquipamentoALista(dados) {
    var manutencao = new ManutencaoEquipamentoGuaritaCheckList(dados);

    _guaritaCheckListManutencaoEquipamento.Servicos.push(manutencao);

    $("#" + manutencao.CustoEstimado.id).maskMoney(ConfigDecimal());
}

function BloquearAlteracaoServicosEquipamento() {
    for (var i = 0; i < _guaritaCheckListManutencaoEquipamento.Servicos().length; i++) {
        var servico = _guaritaCheckListManutencaoEquipamento.Servicos()[i];

        SetarEnableCamposKnockout(servico, false);

        servico.Atualizar.visible(false);
        servico.Excluir.visible(false);
    }

    SetarEnableCamposKnockout(_guaritaCheckListManutencaoEquipamento, false);
    _guaritaCheckListManutencaoEquipamento.AdicionarManutencao.visible(false);
}

function BloquearBuscaNovosServicosEquipamento() {
    if (_guaritaCheckListManutencaoEquipamento.Servicos().length > 0 || _guaritaCheckList.Situacao.val() != EnumSituacaoGuaritaCheckList.Aberto) {
        _guaritaCheckListManutencaoEquipamento.BuscarServicos.visible(false);
        _guaritaCheckListManutencaoEquipamento.EquipamentoServico.enable(false);
    }
    else {
        _guaritaCheckListManutencaoEquipamento.BuscarServicos.visible(true);
        _guaritaCheckListManutencaoEquipamento.EquipamentoServico.enable(true);
    }
}

function validaEquipamentoOperacoesController() {
    _guaritaCheckListManutencaoEquipamento.EquipamentoServico.required(true);
    var valido = ValidarCamposObrigatorios(_guaritaCheckListManutencaoEquipamento);

    if (!valido)
        exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, informe o Equipamento");

    return valido;
}

function validaCamposObrigatoriosGuaritaCheckListManutencaoEquipamento() {
    _guaritaCheckListManutencaoEquipamento.EquipamentoServico.required(false);
    _guaritaCheckListManutencaoEquipamento.DataProgramada.required(false);
    _guaritaCheckListManutencaoEquipamento.TipoOrdemServico.required(false);

    if (_guaritaCheckListManutencaoEquipamento.GerarOS.val()) {
        _guaritaCheckListManutencaoEquipamento.EquipamentoServico.required(true);
        _guaritaCheckListManutencaoEquipamento.DataProgramada.required(true);
        _guaritaCheckListManutencaoEquipamento.TipoOrdemServico.required(true);
    }

    return ValidarCamposObrigatorios(_guaritaCheckListManutencaoEquipamento);
}

function limparCamposGuaritaCheckListManutencaoEquipamento() {
    _guaritaCheckListManutencaoEquipamento.Servicos.removeAll();

    _guaritaCheckListManutencaoEquipamento.AdicionarManutencao.visible(true);
    _guaritaCheckListManutencaoEquipamento.BuscarServicos.visible(true);

    LimparCampos(_guaritaCheckListManutencaoEquipamento);
    SetarEnableCamposKnockout(_guaritaCheckListManutencaoEquipamento, true);
}