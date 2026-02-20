/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="SinistroEtapaDados.js" />

var _etapaAcompanhamentoSinistro,
    _gridHistoricoAcompanhamentoSinistro,
    _CRUDAcompanhamentoSinistro;

var AcompanhamentoSinistro = function () {
    this.AdicionarHistorico = PropertyEntity({ eventClick: adicionarHistoricoModalClick, type: types.event, text: "Adicionar Histórico", visible: ko.observable(true), idBtnSearch: guid(), enable: ko.observable(true) });

    this.Historico = PropertyEntity({ text: "Historico", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observableArray(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos = PropertyEntity({ type: types.local, val: ko.observable([]) });

    this.Historico.val.subscribe(function () {
        recarregarGridHistorico();
    });
};

var CRUDAcompanhamentoSinistro = function () {
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaAcompanhamentoClick, type: types.event, text: "Voltar Etapa", visible: ko.observable(true) });
};

function loadEtapaAcompanhamentoSinistro() {
    _etapaAcompanhamentoSinistro = new AcompanhamentoSinistro();
    KoBindings(_etapaAcompanhamentoSinistro, "knockoutFluxoSinistroAcompanhamento");

    _CRUDAcompanhamentoSinistro = new CRUDAcompanhamentoSinistro();
    KoBindings(_CRUDAcompanhamentoSinistro, "knockoutCRUDFluxoSinistroAcompanhamento");

    loadGridAcompanhamentoSinistro();
    loadAcompanhamentoHistoricoSinistro();
}

function loadGridAcompanhamentoSinistro() {
    var linhasPorPagina = 5;

    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarHistoricoSinistroClick, icone: "", visibilidade: obterVisibilidadeOpcaoEditarHistorico };
    var opcaoExcluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: removerHistoricoSinistroClick, icone: "", visibilidade: obterVisibilidadeOpcaoExcluirHistorico };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoEditar, opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Tipo", visible: false },
        { data: "TipoDescricao", title: "Tipo", width: "20%", className: "text-align-left" },
        { data: "Data", title: "Data", width: "20%", className: "text-align-center" },
        { data: "Observacao", title: "Observação", width: "20%", className: "text-align-left" }
    ];

    _gridHistoricoAcompanhamentoSinistro = new BasicDataTable(_etapaAcompanhamentoSinistro.Historico.idGrid, header, menuOpcoes, null, null, linhasPorPagina);
    _gridHistoricoAcompanhamentoSinistro.CarregarGrid([]);
}

function obterVisibilidadeOpcaoEditarHistorico() {
    return _etapaDadosSinistro.Situacao.val() === EnumSituacaoEtapaFluxoSinistro.Aberto;
}

function obterVisibilidadeOpcaoExcluirHistorico() {
    return _etapaDadosSinistro.Situacao.val() === EnumSituacaoEtapaFluxoSinistro.Aberto;
}

function voltarEtapaAcompanhamentoClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente voltar para a etapa Indicação Pagamento?", function () {
        executarReST("Sinistro/VoltarEtapa", { Codigo: _etapaDadosSinistro.Codigo.val(), Etapa: EnumEtapaSinistro.IndicacaoPagador }, function (retorno) {
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

function editarHistoricoSinistroClick(registroSelecionado) {
    var anexosDoHistorico = _etapaAcompanhamentoSinistro.Anexos.val().filter(function (anexo) {
        return anexo.CodigoHistorico == registroSelecionado.Codigo;
    });

    _acompanhamentoAdicionarHistorico.Sinistro.val(_etapaDadosSinistro.Codigo.val());
    _acompanhamentoAdicionarHistorico.Anexos.val(anexosDoHistorico);
    _acompanhamentoAdicionarHistorico.Adicionar.visible(false);

    PreencherObjetoKnout(_acompanhamentoAdicionarHistorico, { Data: registroSelecionado });

    $("#divModalAdicionarHistorico")
        .modal('show')
        .on("hidden.bs.modal", function () {
            _acompanhamentoAdicionarHistorico.Anexos.val([]);
            LimparCampos(_acompanhamentoAdicionarHistorico);
        });
}

function removerHistoricoSinistroClick(registroSelecionado) {
    executarReST("SinistroHistorico/Excluir", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Msg);

                var registros = _etapaAcompanhamentoSinistro.Historico.val();

                for (var i = 0; i < registros.length; i++) {
                    if (registros[i].Codigo == registroSelecionado.Codigo) {
                        registros.splice(i, 1);
                        break;
                    }
                }

                var anexosRestantes = _etapaAcompanhamentoSinistro.Anexos.val().filter(function (anexo) {
                    return anexo.CodigoHistorico != registroSelecionado.Codigo;
                });

                _etapaAcompanhamentoSinistro.Anexos.val(anexosRestantes);

                _etapaAcompanhamentoSinistro.Historico.val(registros);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        } else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function preencherEtapaAcompanhamento(dados) {
    _etapaAcompanhamentoSinistro.Historico.val(dados.Historicos);
    _etapaAcompanhamentoSinistro.Anexos.val(dados.Anexos);

    if (_etapaDadosSinistro.Situacao.val() !== EnumSituacaoEtapaFluxoSinistro.Aberto) {
        SetarEnableCamposKnockout(_etapaAcompanhamentoSinistro, false);

        _CRUDAcompanhamentoSinistro.VoltarEtapa.visible(false);
    }
}

function limparCamposSinistroEtapaAcompanhamento() {
    SetarEnableCamposKnockout(_etapaAcompanhamentoSinistro, true);

    _CRUDAcompanhamentoSinistro.VoltarEtapa.visible(true);
}

function adicionarHistoricoModalClick() {
    _acompanhamentoAdicionarHistorico.Sinistro.val(_etapaDadosSinistro.Codigo.val());
    _acompanhamentoAdicionarHistorico.Adicionar.visible(true);

    $("#divModalAdicionarHistorico")
        .modal('show')
        .on("hidden.bs.modal", function () {
            _acompanhamentoAdicionarHistorico.Anexos.val([]);
            LimparCampos(_acompanhamentoAdicionarHistorico);
        });
}

function recarregarGridHistorico() {
    _gridHistoricoAcompanhamentoSinistro.CarregarGrid(_etapaAcompanhamentoSinistro.Historico.val());
}