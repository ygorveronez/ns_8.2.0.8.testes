/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/TipoOrdemServico.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="SinistroEtapaDados.js" />
/// <reference path="SinistroEtapaManutencaoOrdemServico.js" />

var _etapaManutencaoSinistro,
    _crudManutencaoSinistro,
    _adicionarManutencao;

var ManutencaoSinistro = function () {
    this.Sinistro = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    //Resumo
    this.CodigoOrdemServico = PropertyEntity({ getType: typesKnockout.int, def: 0, val: ko.observable(0) });
    this.NumeroOrdemServico = PropertyEntity({ text: "N° da O.S.: ", val: ko.observable("") });
    this.SituacaoOrdemServico = PropertyEntity({ text: "Situação: ", val: ko.observable("") });
    this.DataProgramacaoOrdemServico = PropertyEntity({ text: "Data Programação: ", val: ko.observable("") });
    this.InformacaoTipoOrdemServico = PropertyEntity({ text: "Tipo da O.S.: ", val: ko.observable("") });
    this.LocalOrdemServico = PropertyEntity({ text: "Local: ", val: ko.observable("") });

    //Preenchimento
    this.DataProgramada = PropertyEntity({ text: "*Data Programada: ", getType: typesKnockout.date, type: types.date, enable: ko.observable(true), required: ko.observable(false) });
    this.ObservacaoOS = PropertyEntity({ text: "Observação para a OS:", maxlength: 1000, val: ko.observable(""), enable: ko.observable(true) });
    this.TipoOrdemServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo da OS:", idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.LocalManutencao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local da Manutenção:", idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });

    //Listas
    this.Servicos = ko.observableArray();
    this.OrdensServico = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.AdicionarManutencao = PropertyEntity({ eventClick: adicionarManutencaoClick, type: types.event, text: "Adicionar Manutenção", icon: "fal fa-plus", idGrid: guid(), visible: ko.observable(true) });
};

var CRUDManutencaoSinistro = function () {
    this.CriarOrdemServico = PropertyEntity({ text: "Confirmar Ordem de Serviço", type: types.event, eventClick: criarOrdemServicoClick, idBtn: guid(), visible: ko.observable(true) });
    this.AvancarEtapa = PropertyEntity({ eventClick: avancarEtapaManutencaoClick, type: types.event, text: "Avançar", visible: ko.observable(false) });
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaManutencaoClick, type: types.event, text: "Voltar Etapa", visible: ko.observable(false) });
};

var AdicionarServicoSinistro = function () {
    this.Sinistro = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Serviço:", idBtnSearch: guid(), required: true });
    this.UltimaManutencao = PropertyEntity({ maxlength: 1000, text: "Última Manutenção:", visible: ko.observable(false) });
    this.TempoEstimado = PropertyEntity({ maxlength: 6, text: "Tempo Estimado (min):", visible: ko.observable(true), getType: typesKnockout.int, val: ko.observable(0) });
    this.Observacao = PropertyEntity({ maxlength: 1000, text: "Observação:" });

    this.Adicionar = PropertyEntity({ eventClick: adicionarServicoClick, type: types.event, text: "Adicionar", icon: "fal fa-plus", idGrid: guid() });

    this.Servico.codEntity.subscribe(function (novoValor) {
        if (novoValor == null || novoValor == 0) {
            _adicionarManutencao.UltimaManutencao.val("");
            _adicionarManutencao.UltimaManutencao.visible(false);
        }
    });

    this.Servico.val.subscribe(function (novoValor) {
        if (novoValor == null || novoValor.trim() == "")
            _adicionarManutencao.Servico.codEntity(0);
    });
};

var ManutencaoServicoSinistro = function (dados) {
    var editar = false;
    if (_etapaDadosSinistro.Etapa.val() === EnumEtapaSinistro.Manutencao)
        editar = true;

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(dados.Codigo), def: dados.Codigo });
    this.Servico = PropertyEntity({ text: "Serviço:", val: ko.observable(dados.Servico.Descricao) });
    this.CustoMedio = PropertyEntity({ text: "Custo Médio:", val: ko.observable(Globalize.format(dados.CustoMedio, "n2")) });
    this.CustoEstimado = PropertyEntity({ getType: typesKnockout.decimal, text: "Custo Estimado:", val: ko.observable(Globalize.format(dados.CustoEstimado, "n2")), enable: ko.observable(editar) });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable(dados.Observacao), enable: ko.observable(editar) });
    this.TipoManutencao = PropertyEntity({ text: ko.observable(dados.DescricaoTipoManutencao), val: ko.observable(dados.TipoManutencao) });

    var ultimaManutencao = "";
    if (dados.DataUltimaManutencao != null && dados.DataUltimaManutencao != "")
        ultimaManutencao = dados.QuilometragemUltimaManutencao + "km em " + dados.DataUltimaManutencao;
    else
        ultimaManutencao = "Nenhuma realizada";

    this.UltimaManutencao = PropertyEntity({ text: "Última Manutenção:", val: ko.observable(ultimaManutencao) });
    this.TempoEstimado = PropertyEntity({ text: "Tempo Estimado (min):", visible: ko.observable(true), val: ko.observable(Globalize.format(dados.TempoEstimado, "n0")) });

    var textoRibbon = "Corretiva";
    var classeRibbon = "ribbon-tms ribbon-tms-red";

    if (dados.TipoManutencao == EnumTipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva) {
        classeRibbon = "ribbon-tms ribbon-tms-green";
        textoRibbon = "Preventiva";
    }

    this.Ribbon = PropertyEntity({ text: ko.observable(textoRibbon), cssClass: ko.observable(classeRibbon), visible: true });

    this.Atualizar = PropertyEntity({ eventClick: atualizarManutencaoClick, type: types.event, text: "Salvar", icon: "fal fa-save", idGrid: guid(), visible: ko.observable(editar) });
    this.Excluir = PropertyEntity({ eventClick: excluirManutencaoClick, type: types.event, text: "Excluir", icon: "fal fa-times", idGrid: guid(), visible: ko.observable(editar) });
};

function loadEtapaManutencaoSinistro() {
    _etapaManutencaoSinistro = new ManutencaoSinistro();
    KoBindings(_etapaManutencaoSinistro, "knockoutFluxoSinistroManutencao");

    _crudManutencaoSinistro = new CRUDManutencaoSinistro();
    KoBindings(_crudManutencaoSinistro, "knockoutCRUDFluxoSinistroManutencao");

    _adicionarManutencao = new AdicionarServicoSinistro();
    KoBindings(_adicionarManutencao, "knockoutAdicionarManutencao");

    new BuscarTipoOrdemServico(_etapaManutencaoSinistro.TipoOrdemServico);
    new BuscarClientes(_etapaManutencaoSinistro.LocalManutencao, retornoConsultaLocalManutencao, true, [EnumModalidadePessoa.Fornecedor]);

    new BuscarServicoVeiculo(_adicionarManutencao.Servico, retornoConsultaServicoVeiculo);

    loadManutencaoSinistroOrdemServico();
}

function retornoConsultaServicoVeiculo(dados) {
    _adicionarManutencao.Servico.codEntity(dados.Codigo);
    _adicionarManutencao.Servico.val(dados.Descricao);
    _adicionarManutencao.TempoEstimado.val(dados.TempoEstimado);

    executarReST("SinistroManutencao/BuscarDadosUltimaExecucao", { Sinistro: _etapaDadosSinistro.Codigo.val(), Servico: dados.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _adicionarManutencao.UltimaManutencao.visible(true);
                if (r.Data.Codigo > 0)
                    _adicionarManutencao.UltimaManutencao.val(r.Data.Quilometragem + "km / " + r.Data.Data);
                else
                    _adicionarManutencao.UltimaManutencao.val("Não há manutenção realizada com este serviço para este veículo.");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function retornoConsultaLocalManutencao(dados) {
    _etapaManutencaoSinistro.LocalManutencao.codEntity(dados.Codigo);
    _etapaManutencaoSinistro.LocalManutencao.val(dados.Nome + " (" + dados.Localidade + ")");
}

function criarOrdemServicoClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente avançar a etapa da manutenção?", function () {
        _etapaManutencaoSinistro.Sinistro.val(_etapaDadosSinistro.Codigo.val());
        _etapaManutencaoSinistro.OrdensServico.val(JSON.stringify(_etapaManutencaoSinistroOrdemServico.OrdemServico.basicTable.BuscarRegistros()));
        Salvar(_etapaManutencaoSinistro, "SinistroManutencao/CriarOrdemServico", function (r) {
            if (r.Success) {
                if (r.Data) {
                    recarregarGridSinistro();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Etapa avançada com sucesso!");
                    limparFluxoSinistro();
                    CarregarDadosSinistro(r.Data.Codigo);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function avancarEtapaManutencaoClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente avançar a etapa de Manutenção?", function () {
        executarReST("Sinistro/AvancarEtapa", { Codigo: _etapaDadosSinistro.Codigo.val(), Etapa: EnumEtapaSinistro.IndicacaoPagador }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    limparFluxoSinistro();                    
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Fluxo avançado com sucesso.");
                    CarregarDadosSinistro(retorno.Data.Codigo);
                } else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function voltarEtapaManutencaoClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente voltar para a etapa Documentação?", function () {
        executarReST("Sinistro/VoltarEtapa", { Codigo: _etapaDadosSinistro.Codigo.val(), Etapa: EnumEtapaSinistro.Documentacao }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Fluxo voltado com sucesso.");

                    recarregarGridSinistro();
                    editarSinistroClick({ Codigo: _etapaDadosSinistro.Codigo.val() });
                } else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function adicionarManutencaoClick() {
    LimparCampos(_adicionarManutencao);
    Global.abrirModal('divModalAdicionarManutencao');
}

function adicionarServicoClick() {
    _adicionarManutencao.Sinistro.val(_etapaDadosSinistro.Codigo.val());
    Salvar(_adicionarManutencao, "SinistroManutencao/Adicionar", function (r) {
        if (r.Success) {
            if (r.Data) {
                AdicionarManutencaoALista(r.Data);
                Global.fecharModal('divModalAdicionarManutencao');
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Manutenção adicionada com sucesso!");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function atualizarManutencaoClick(e) {
    Salvar(e, "SinistroManutencao/Atualizar", function (r) {
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

function excluirManutencaoClick(e) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir a manutenção " + e.Servico.val() + "?", function () {
        ExcluirPorCodigo(e, "SinistroManutencao/Excluir", function (r) {
            if (r.Success) {
                if (r.Data) {
                    _etapaManutencaoSinistro.Servicos.remove(function (item) { return item.Codigo.val() == e.Codigo.val() });
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

function BuscarManutencoesSinistro() {
    executarReST("SinistroManutencao/BuscarPorSinistro", { Sinistro: _etapaDadosSinistro.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _etapaManutencaoSinistro.Servicos.removeAll();
                for (var i = 0; i < r.Data.length; i++)
                    AdicionarManutencaoALista(r.Data[i]);

                BloquearAlteracaoServicos();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function AdicionarManutencaoALista(dados) {
    var manutencao = new ManutencaoServicoSinistro(dados);

    _etapaManutencaoSinistro.Servicos.push(manutencao);

    $("#" + manutencao.CustoEstimado.id).maskMoney({ precision: 2, allowZero: false, allowNegative: false });
}

function BloquearAlteracaoServicos() {
    if (_etapaDadosSinistro.Etapa.val() === EnumEtapaSinistro.Manutencao && _etapaManutencaoSinistro.CodigoOrdemServico.val() == 0)
        return;

    for (var i = 0; i < _etapaManutencaoSinistro.Servicos().length; i++) {
        var servico = _etapaManutencaoSinistro.Servicos()[i];

        SetarEnableCamposKnockout(servico, false);

        servico.Atualizar.visible(false);
        servico.Excluir.visible(false);
    }

    SetarEnableCamposKnockout(_etapaManutencaoSinistro, false);
    _etapaManutencaoSinistro.AdicionarManutencao.visible(false);
    _crudManutencaoSinistro.CriarOrdemServico.visible(false);
    _crudManutencaoSinistro.VoltarEtapa.visible(false);

    bloquearCamposManutencaoSinistroOrdemServico();

    if (_etapaDadosSinistro.Etapa.val() === EnumEtapaSinistro.Manutencao && _etapaManutencaoSinistro.CodigoOrdemServico.val() > 0) {
        _crudManutencaoSinistro.AvancarEtapa.visible(true);
        _crudManutencaoSinistro.VoltarEtapa.visible(true);
    }
}

function preencherEtapaManutencao(dados) {
    PreencherObjetoKnout(_etapaManutencaoSinistro, { Data: dados });
    recarregarGridManutencaoOrdemServico();

    BloquearAlteracaoServicos();
    BuscarManutencoesSinistro();
}

function limparCamposSinistroEtapaManutencao() {
    _etapaManutencaoSinistro.Servicos.removeAll();

    _etapaManutencaoSinistro.AdicionarManutencao.visible(true);
    _crudManutencaoSinistro.CriarOrdemServico.visible(true);
    _crudManutencaoSinistro.AvancarEtapa.visible(false);
    _crudManutencaoSinistro.VoltarEtapa.visible(true);

    LimparCampos(_etapaManutencaoSinistro);
    SetarEnableCamposKnockout(_etapaManutencaoSinistro, true);
    limparCamposManutencaoSinistroOrdemServico();
}