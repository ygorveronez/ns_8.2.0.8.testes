/// <reference path="../../Consultas/ServicoVeiculo.js" />
/// <reference path="../../Consultas/GrupoServico.js" />
/// <reference path="OrdemServico.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _servicoOrdemServico, _adicionarServicoOrdemServico;

var ServicoOrdemServico = function () {

    this.Servicos = ko.observableArray();
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.AdicionarManutencao = PropertyEntity({ eventClick: AdicionarManutencaoClick, type: types.event, text: "Adicionar Manutenção", icon: "fal fa-plus", idGrid: guid(), visible: ko.observable(true) });
    this.AdicionarServico = PropertyEntity({ type: types.multiplesEntities, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), text: "Adicionar Serviços", icon: "fal fa-plus-circle", idGrid: guid(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.ExcluirTodosServicos = PropertyEntity({ eventClick: ExcluirTodosServicosClick, type: types.event, text: "Excluir todos os Serviços", icon: "fa fa-ban", idGrid: guid() });


};

var AdicionarServicoOrdemServico = function () {
    this.OrdemServico = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Serviço:", idBtnSearch: guid(), required: true });
    this.UltimaManutencao = PropertyEntity({ maxlength: 1000, text: "Última Manutenção:", visible: ko.observable(false) });
    this.TempoEstimado = PropertyEntity({ maxlength: 6, text: "Tempo Estimado (min):", visible: ko.observable(true), getType: typesKnockout.int, val: ko.observable(0) });
    this.Observacao = PropertyEntity({ maxlength: 1000, text: "Observação:" });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarServicoClick, type: types.event, text: "Adicionar", icon: "fal fa-plus", idGrid: guid() });

    this.Servico.codEntity.subscribe(function (novoValor) {
        if (novoValor == null || novoValor == 0) {
            _adicionarServicoOrdemServico.UltimaManutencao.val("");
            _adicionarServicoOrdemServico.UltimaManutencao.visible(false);
        }
    });

    this.Servico.val.subscribe(function (novoValor) {
        if (novoValor == null || novoValor.trim() == "")
            _adicionarServicoOrdemServico.Servico.codEntity(0);
    });
};

var ManutencaoOrdemServico = function (dados) {


    var editar = false;
    if (_ordemServico.Situacao.val() === EnumSituacaoOrdemServicoFrota.Finalizada)
        editar = false;
    else if (_ordemServico.Situacao.val() === EnumSituacaoOrdemServicoFrota.EmManutencao)
        editar = false;
    else {
        editar = true;
    }

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(dados.Codigo), def: dados.Codigo });
    this.Servico = PropertyEntity({ text: "Serviço:", val: ko.observable(dados.Servico.Descricao) });
    this.CustoMedio = PropertyEntity({ text: "Custo Médio:", val: ko.observable(Globalize.format(dados.CustoMedio, "n2")) });
    this.CustoEstimado = PropertyEntity({ getType: typesKnockout.decimal, text: "Custo Estimado:", val: ko.observable(Globalize.format(dados.CustoEstimado, "n2")), enable: ko.observable(editar) });
    this.Observacao = PropertyEntity({ maxlength: 1000, text: "Observação:", val: ko.observable(dados.Observacao), enable: ko.observable(editar) });
    this.TipoManutencao = PropertyEntity({ text: ko.observable(dados.DescricaoTipoManutencao), val: ko.observable(dados.TipoManutencao) });
    this.NaoExecutado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(dados.NaoExecutado) });

    var ultimaManutencao = "";
    if (dados.DataUltimaManutencao != null && dados.DataUltimaManutencao != "")
        ultimaManutencao = dados.QuilometragemUltimaManutencao + "km em " + dados.DataUltimaManutencao;
    else
        ultimaManutencao = "Nenhuma realizada";

    this.UltimaManutencao = PropertyEntity({ text: "Última Manutenção:", val: ko.observable(ultimaManutencao) });
    this.TempoEstimado = PropertyEntity({ text: "Tempo Estimado (min):", visible: ko.observable(true), val: ko.observable(Globalize.format(dados.TempoEstimado, "n0")) });

    var textoRibbon = "Corretiva";
    var classeRibbon = "ribbon-tms ribbon-tms-red";

    if (dados.NaoExecutado) {
        classeRibbon = "ribbon-tms ribbon-tms-yellow";
        textoRibbon = "Não Execut.";
    }
    else if (dados.TipoManutencao == EnumTipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva) {
        classeRibbon = "ribbon-tms ribbon-tms-green";
        textoRibbon = "Preventiva";
    }

    this.Ribbon = PropertyEntity({ text: ko.observable(textoRibbon), cssClass: ko.observable(classeRibbon), visible: true });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarManutencaoClick, type: types.event, text: "Salvar", icon: "fal fa-save", idGrid: guid(), visible: ko.observable(editar) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirManutencaoClick, type: types.event, text: "Excluir", icon: "fal fa-times", idGrid: guid(), visible: ko.observable(editar) });
    this.NaoExecutar = PropertyEntity({ eventClick: NaoExecutarManutencaoClick, type: types.event, text: "Não Executar", icon: "fal fa-exclamation", idGrid: guid(), visible: ko.observable(editar && !dados.NaoExecutado) });


};

//*******EVENTOS*******

function LoadServicoOrdemServico() {

    _servicoOrdemServico = new ServicoOrdemServico();
    KoBindings(_servicoOrdemServico, "knockoutServico");

    _adicionarServicoOrdemServico = new AdicionarServicoOrdemServico();
    KoBindings(_adicionarServicoOrdemServico, "knockoutAdicionarServico");

    new BuscarServicoVeiculo(_adicionarServicoOrdemServico.Servico, RetornoConsultaServicoVeiculo, null, _ordemServico.GrupoServico);

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", visible: false }
    ];
    var _gridSelecaoServicos = new BasicDataTable(_servicoOrdemServico.AdicionarServico.idGrid, header);
    new BuscarServicoVeiculo(_servicoOrdemServico.AdicionarServico, RetornoConsultaMultiplosServicoVeiculo, _gridSelecaoServicos);
    _servicoOrdemServico.AdicionarServico.basicTable = _gridSelecaoServicos;
    _gridSelecaoServicos.CarregarGrid([]);
}

function AdicionarManutencaoClick() {
    LimparCampos(_adicionarServicoOrdemServico);
    Global.abrirModal("knockoutAdicionarServico");
}

function AdicionarServicoClick() {
    _adicionarServicoOrdemServico.OrdemServico.val(_ordemServico.Codigo.val());
    Salvar(_adicionarServicoOrdemServico, "ServicoOrdemServico/Adicionar", function (r) {
        if (r.Success) {
            if (r.Data) {
                AdicionarManutencaoALista(r.Data);
                Global.fecharModal('knockoutAdicionarServico');
                LimparCampos(_adicionarServicoOrdemServico);
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
    Salvar(e, "ServicoOrdemServico/Atualizar", function (r) {
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
        ExcluirPorCodigo(e, "ServicoOrdemServico/Excluir", function (r) {
            if (r.Success) {
                if (r.Data) {
                    _servicoOrdemServico.Servicos.remove(function (item) { return item.Codigo.val() == e.Codigo.val() });
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

function ExcluirTodosServicosClick(e, sender) {

    exibirConfirmacao("Atenção!", "Deseja realmente excluir todos os Servicos?", function () {
        executarReST("ServicoOrdemServico/ExcluirTodosServicos", { OrdemServico: _ordemServico.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Serviços excluído com sucesso!");
                    _servicoOrdemServico.Servicos.removeAll();
                } else {

                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function NaoExecutarManutencaoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente informar que o serviço " + e.Servico.val() + " não foi executado?", function () {
        executarReST("ServicoOrdemServico/InformarNaoExecucao", { Codigo: e.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    for (var i = 0; i < _servicoOrdemServico.Servicos().length; i++) {
                        var servico = _servicoOrdemServico.Servicos()[i];
                        if (servico.Codigo.val() == e.Codigo.val()) {
                            servico.NaoExecutado.val(true);
                            servico.Ribbon.text("Não Execut.");
                            servico.Ribbon.cssClass("ribbon-tms ribbon-tms-yellow");
                            servico.NaoExecutar.visible(false);
                        }
                    }
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Não execução informada com sucesso!");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function BuscarManutencoesOrdemServico() {
    executarReST("ServicoOrdemServico/BuscarPorOrdemServico", { OrdemServico: _ordemServico.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _servicoOrdemServico.Servicos.removeAll();
                for (var i = 0; i < r.Data.length; i++)
                    AdicionarManutencaoALista(r.Data[i]);

                if (_ordemServico.Situacao.val() == EnumSituacaoOrdemServicoFrota.Finalizada) {
                    _servicoOrdemServico.AdicionarManutencao.visible(false);
                    _servicoOrdemServico.AdicionarServico.visible(false);
                } else if (_ordemServico.Situacao.val() == EnumSituacaoOrdemServicoFrota.EmManutencao) {
                    _servicoOrdemServico.AdicionarManutencao.visible(false);
                    _servicoOrdemServico.AdicionarServico.visible(false);
                } else {
                    _servicoOrdemServico.AdicionarManutencao.visible(true);
                    _servicoOrdemServico.AdicionarServico.visible(true);
                }


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
    _adicionarServicoOrdemServico.Servico.codEntity(dados.Codigo);
    _adicionarServicoOrdemServico.Servico.val(dados.Descricao);
    _adicionarServicoOrdemServico.TempoEstimado.val(dados.TempoEstimado);

    executarReST("ServicoOrdemServico/BuscarDadosUltimaExecucao", { OrdemServico: _ordemServico.Codigo.val(), Servico: dados.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _adicionarServicoOrdemServico.UltimaManutencao.visible(true);
                if (r.Data.Codigo > 0)
                    _adicionarServicoOrdemServico.UltimaManutencao.val(r.Data.Quilometragem + "km / " + r.Data.Data);
                else
                    _adicionarServicoOrdemServico.UltimaManutencao.val("Não há manutenção realizada com este serviço para este veículo.");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function RetornoConsultaMultiplosServicoVeiculo(dados) {
    executarReST("ServicoOrdemServico/AdicionarMultiplos", { Codigo: _ordemServico.Codigo.val(), Servicos: JSON.stringify(dados.map(o => o.Codigo)) }, function (r) {
        if (r.Success) {
            if (r.Data) {
                var data = r.Data;
                for (var i = 0; i < data.length; i++) {
                    AdicionarManutencaoALista(data[i]);
                }

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Serviços adicionados com sucesso!");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function AdicionarManutencaoALista(dados) {
    var manutencao = new ManutencaoOrdemServico(dados);

    _servicoOrdemServico.Servicos.push(manutencao);

    $("#" + manutencao.CustoEstimado.id).maskMoney(ConfigDecimal());
}

function BloquearAlteracaoServicos() {
    for (var i = 0; i < _servicoOrdemServico.Servicos().length; i++) {
        var servico = _servicoOrdemServico.Servicos()[i];

        SetarEnableCamposKnockout(servico, false);

        servico.Atualizar.visible(false);
        servico.Excluir.visible(false);
        servico.NaoExecutar.visible(false);
    }

    _servicoOrdemServico.AdicionarManutencao.visible(false);
    _servicoOrdemServico.AdicionarServico.visible(false);
}

function LiberarAlteracaoServicos() {
    for (var i = 0; i < _servicoOrdemServico.Servicos().length; i++) {
        var servico = _servicoOrdemServico.Servicos()[i];

        SetarEnableCamposKnockout(servico, true);

        servico.Atualizar.visible(true);
        servico.Excluir.visible(true);
        if (!servico.NaoExecutado.val())
            servico.NaoExecutar.visible(true);
    }

    _servicoOrdemServico.AdicionarManutencao.visible(true);
    _servicoOrdemServico.AdicionarServico.visible(true);
}

function LimparCamposServicoOrdemServico() {
    _servicoOrdemServico.Servicos.removeAll();
    _servicoOrdemServico.AdicionarManutencao.visible(true);
    _servicoOrdemServico.AdicionarServico.visible(true);

    LimparCampos(_servicoOrdemServico);
}