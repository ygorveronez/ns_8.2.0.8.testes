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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoOrdemServico.js" />
/// <reference path="../../Consultas/ServicoVeiculo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoGuaritaCheckList.js" />
/// <reference path="../../Enumeradores/EnumTipoManutencaoServicoVeiculoOrdemServicoFrota.js" />
/// <reference path="GuaritaCheckList.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _guaritaCheckListManutencao, _adicionarServicoCheckListManutencao;

var GuaritaCheckListManutencao = function () {
    this.GerarOS = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Deseja gerar uma Ordem de Serviço para o veículo com os serviços lançados?", enable: ko.observable(true), visible: ko.observable(true) });
    this.DataProgramada = PropertyEntity({ text: "*Data Programada: ", getType: typesKnockout.date, type: types.date, enable: ko.observable(true), required: ko.observable(false) });
    this.ObservacaoOS = PropertyEntity({ text: "Observação para a OS:", maxlength: 1000, val: ko.observable(""), enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: _guaritaCheckList.Situacao.val, options: _guaritaCheckList.Situacao.options, def: _guaritaCheckList.Situacao.def, text: "Situação:", enable: ko.observable(true) });

    this.TipoOrdemServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo da OS:", idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.LocalManutencao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local da Manutenção:", idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });

    this.Servicos = ko.observableArray();

    this.AdicionarManutencao = PropertyEntity({ eventClick: AdicionarManutencaoClick, type: types.event, text: "Adicionar Manutenção", icon: "fal fa-plus", id: guid(), visible: ko.observable(true) });
    this.BuscarServicos = PropertyEntity({ eventClick: BuscarServicosClick, type: types.event, text: "Buscar Serviços", icon: "fal fa-gear", id: guid(), visible: ko.observable(true) });
};

var AdicionarServicoCheckListManutencao = function () {
    this.GuaritaCheckList = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Serviço:", idBtnSearch: guid(), required: true });
    this.UltimaManutencao = PropertyEntity({ maxlength: 1000, text: "Última Manutenção:", visible: ko.observable(false) });
    this.TempoEstimado = PropertyEntity({ maxlength: 6, text: "Tempo Estimado (min):", visible: ko.observable(true), getType: typesKnockout.int, val: ko.observable(0) });
    this.Observacao = PropertyEntity({ maxlength: 1000, text: "Observação:" });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarServicoClick, type: types.event, text: "Adicionar", icon: "fal fa-plus", idGrid: guid() });

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

var ManutencaoGuaritaCheckList = function (dados) {
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

    this.Atualizar = PropertyEntity({ eventClick: AtualizarManutencaoClick, type: types.event, text: "Salvar", icon: "fal fa-save", id: guid(), visible: ko.observable(editar) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirManutencaoClick, type: types.event, text: "Excluir", icon: "fal fa-times", id: guid(), visible: ko.observable(editar) });
};

//*******EVENTOS*******

function loadGuaritaCheckListManutencao() {
    _guaritaCheckListManutencao = new GuaritaCheckListManutencao();
    KoBindings(_guaritaCheckListManutencao, "knockoutGuaritaCheckListManutencao");

    _adicionarServicoCheckListManutencao = new AdicionarServicoCheckListManutencao();
    KoBindings(_adicionarServicoCheckListManutencao, "knockoutAdicionarServico");

    new BuscarClientes(_guaritaCheckListManutencao.LocalManutencao, null, null, [EnumModalidadePessoa.Fornecedor]);
    new BuscarTipoOrdemServico(_guaritaCheckListManutencao.TipoOrdemServico);
    new BuscarServicoVeiculo(_adicionarServicoCheckListManutencao.Servico, RetornoConsultaServicoVeiculo);
}

function AdicionarManutencaoClick() {
    LimparCampos(_adicionarServicoCheckListManutencao);
    Global.abrirModal("knockoutAdicionarServico");
}

function BuscarServicosClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente buscar os serviços do veículo?", function () {
        executarReST("GuaritaCheckListServico/BuscarServicosParaManutencao", { GuaritaCheckList: _guaritaCheckList.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    BuscarManutencoesGuaritaCheckList();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function AdicionarServicoClick() {
    _adicionarServicoCheckListManutencao.GuaritaCheckList.val(_guaritaCheckList.Codigo.val());
    Salvar(_adicionarServicoCheckListManutencao, "GuaritaCheckListServico/Adicionar", function (r) {
        if (r.Success) {
            if (r.Data) {
                AdicionarManutencaoALista(r.Data);
                Global.fecharModal('knockoutAdicionarServico');
                LimparCampos(_adicionarServicoCheckListManutencao);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Manutenção adicionada com sucesso!");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function AtualizarManutencaoClick(e, sender) {
    Salvar(e, "GuaritaCheckListServico/Atualizar", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Serviço atualizado com sucesso!");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function ExcluirManutencaoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir a manutenção " + e.Servico.val() + "?", function () {
        ExcluirPorCodigo(e, "GuaritaCheckListServico/Excluir", function (r) {
            if (r.Success) {
                if (r.Data) {
                    _guaritaCheckListManutencao.Servicos.remove(function (item) { return item.Codigo.val() == e.Codigo.val() });
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Serviço excluído com sucesso!");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function BuscarManutencoesGuaritaCheckList() {
    executarReST("GuaritaCheckListServico/BuscarPorGuaritaCheckList", { GuaritaCheckList: _guaritaCheckList.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _guaritaCheckListManutencao.Servicos.removeAll();
                for (var i = 0; i < r.Data.length; i++)
                    AdicionarManutencaoALista(r.Data[i]);

                if (_guaritaCheckList.Situacao.val() != EnumSituacaoGuaritaCheckList.Aberto)
                    _guaritaCheckListManutencao.AdicionarManutencao.visible(false);
                BloquearBuscaNovosServicos();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

//*******METODOS*******

function RetornoConsultaServicoVeiculo(dados) {
    _adicionarServicoCheckListManutencao.Servico.codEntity(dados.Codigo);
    _adicionarServicoCheckListManutencao.Servico.val(dados.Descricao);
    _adicionarServicoCheckListManutencao.TempoEstimado.val(dados.TempoEstimado);

    executarReST("GuaritaCheckListServico/BuscarDadosUltimaExecucao", { GuaritaCheckList: _guaritaCheckList.Codigo.val(), Servico: dados.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _adicionarServicoCheckListManutencao.UltimaManutencao.visible(true);
                if (r.Data.Codigo > 0)
                    _adicionarServicoCheckListManutencao.UltimaManutencao.val(r.Data.Quilometragem + "km / " + r.Data.Data);
                else
                    _adicionarServicoCheckListManutencao.UltimaManutencao.val("Não há manutenção realizada com este serviço para este veículo.");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function AdicionarManutencaoALista(dados) {
    var manutencao = new ManutencaoGuaritaCheckList(dados);

    _guaritaCheckListManutencao.Servicos.push(manutencao);

    $("#" + manutencao.CustoEstimado.id).maskMoney(ConfigDecimal());
}

function BloquearAlteracaoServicos() {
    for (var i = 0; i < _guaritaCheckListManutencao.Servicos().length; i++) {
        var servico = _guaritaCheckListManutencao.Servicos()[i];

        SetarEnableCamposKnockout(servico, false);

        servico.Atualizar.visible(false);
        servico.Excluir.visible(false);
    }

    SetarEnableCamposKnockout(_guaritaCheckListManutencao, false);
    _guaritaCheckListManutencao.AdicionarManutencao.visible(false);
}

function BloquearBuscaNovosServicos() {
    if (_guaritaCheckListManutencao.Servicos().length > 0 || _guaritaCheckList.Situacao.val() != EnumSituacaoGuaritaCheckList.Aberto)
        _guaritaCheckListManutencao.BuscarServicos.visible(false);
    else
        _guaritaCheckListManutencao.BuscarServicos.visible(true);
}

function validaCamposObrigatoriosGuaritaCheckListManutencao() {
    _guaritaCheckListManutencao.DataProgramada.required(false);
    _guaritaCheckListManutencao.TipoOrdemServico.required(false);

    if (_guaritaCheckListManutencao.GerarOS.val()) {
        _guaritaCheckListManutencao.DataProgramada.required(true);
        _guaritaCheckListManutencao.TipoOrdemServico.required(true);
    }

    return ValidarCamposObrigatorios(_guaritaCheckListManutencao);
}

function limparCamposGuaritaCheckListManutencao() {
    _guaritaCheckListManutencao.Servicos.removeAll();

    _guaritaCheckListManutencao.AdicionarManutencao.visible(true);
    _guaritaCheckListManutencao.BuscarServicos.visible(true);

    LimparCampos(_guaritaCheckListManutencao);
    SetarEnableCamposKnockout(_guaritaCheckListManutencao, true);
}