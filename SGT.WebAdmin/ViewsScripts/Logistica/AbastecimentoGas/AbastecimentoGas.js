/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAprovacaoSolicitacaoGas.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Auditoria.js" />

var _abastecimentoGas;
var _pesquisaAbastecimentoGas;
var _CRUDAbastecimentoGas;
var _gridPesquisaAbastecimentoGas;
var _telaParcial = false;

var PesquisaAbastecimentoGas = function () {
    this.DataSolicitacaoInicial = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.date, text: "Data Solicitação Inicial:", visible: ko.observable(true), enable: ko.observable(true) });
    this.DataSolicitacaoFinal = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.date, text: "Data Solicitação Final:", visible: ko.observable(true), enable: ko.observable(true) });

    this.DataSolicitacaoFinal.dateRangeInit = this.DataSolicitacaoInicial;
    this.DataSolicitacaoInicial.dateRangeLimit = this.DataSolicitacaoFinal;

    this.Base = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "Base:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoAprovacaoSolicitacaoGas.Todas), options: EnumSituacaoAprovacaoSolicitacaoGas.obterOpcoesPesquisaSolicitacaoGas(), def: EnumSituacaoAprovacaoSolicitacaoGas.Todas, text: "Situação: " });

    this.Pesquisar = PropertyEntity({ eventClick: pesquisarAbastecimentoGas, type: types.event, text: "Pesquisar", visible: ko.observable(true), idGrid: guid() });
    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
};

var AbastecimentoGas = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });

    this.DataMedicaoEntidade = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string, text: "Data Medição: ", visible: ko.observable(false) });

    this.Base = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "*Base:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), required: true });
    this.Produto = PropertyEntity({ codEntity: ko.observable(0), type: types.entity, text: "*Produto:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false), required: true });
    this.DataMedicao = PropertyEntity({ val: ko.observable(new Array()), options: obterDatasMedicoes(), def: new Array(), text: "*Data Medição:", visible: ko.observable(true), required: true });

    this.Base.codEntity.subscribe(function (valor) {
        _abastecimentoGas.Produto.visible(valor > 0);
    });

    this.Abertura = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: "*Abertura (ton):", visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true, allowNegative: false }, required: true });
    this.PrevisaoBombeio = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: "*Previsão de Bombeio (ton):", visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true, allowNegative: false }, required: true });
    this.PrevisaoTransferenciaRecebida = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: "*Previsão de Transferência Recebida (ton):", visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true, allowNegative: false }, required: true });
    this.PrevisaoDemandaDomiciliar = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: "*Previsão Demanda Domiciliar (ton):", visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true, allowNegative: false }, required: true });
    this.PrevisaoDemandaEmpresarial = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: "*Previsão Demanda Empresarial (ton):", visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true, allowNegative: false }, required: true });
    this.EstoqueUltrasystem = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: "*Demanda Ultrasystem (ton):", visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true, allowNegative: false }, required: true });
    this.PrevisaoTransferenciaEnviada = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: "*Previsão Transferência Enviada (ton):", visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true, allowNegative: false }, required: true });
    this.DensidadeAberturaDia = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: "*Densidade da abertura do dia (kg/m³):", visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true, allowNegative: false }, required: true });
    this.PrevisaoFechamento = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: "Previsão Fechamento (ton):", visible: ko.observable(true), enable: false, configDecimal: { precision: 2, allowZero: true, allowNegative: false }, required: false });

    this.VolumeRodoviarioCarregamentoProximoDia = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: "*Volume rodoviário para carregamento no próximo dia (ton):", visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true, allowNegative: false }, required: true });
    this.PrevisaoBombeioProximoDia = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: "*Previsão Bombeio para o próximo dia (ton):", visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true, allowNegative: false }, required: true });
    this.DisponibilidadeTransferenciaProximoDia = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: "*Disponibilidade de Transferência para o próximo dia (ton):", visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true, allowNegative: false }, required: true });

    this.AdicionalVolumeRodoviarioCarregamentoProximoDia = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: "Adicional volume rodoviário para carregamento no próximo dia (ton):", visible: ko.observable(false), configDecimal: { precision: 2, allowZero: true, allowNegative: false }, required: false });
    this.AdicionalDisponibilidadeTransferenciaProximoDia = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.decimal, text: "Adicional disponibilidade de Transferência para o próximo dia (ton):", visible: ko.observable(false), configDecimal: { precision: 2, allowZero: true, allowNegative: false }, required: false });

    this.HorarioLimite = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.HorarioLimiteComJustificativa = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.AbastecimentoDuplicado = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
};

var CRUDAbastecimentoGas = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarAbastecimentoGas, type: types.event, text: "Adicionar", visible: ko.observable(true), id: guid() });
    this.SalvarAdicional = PropertyEntity({ eventClick: salvarAdicionalAbastecimentoGas, type: types.event, text: "Salvar Adicional", visible: ko.observable(false), id: guid() });
    this.Limpar = PropertyEntity({ eventClick: limparCamposAbastecimentoGas, type: types.event, text: "Limpar", visible: ko.observable(true), id: guid() });

    this.VisualizarJustificativa = PropertyEntity({ eventClick: carregarJustificativa, type: types.event, text: "Visualizar Justificativa", visible: ko.observable(false), id: guid() });
};

function loadAbastecimentoGas() {
    loadConteudosHTML().then(function () {
        _abastecimentoGas = new AbastecimentoGas();
        _pesquisaAbastecimentoGas = new PesquisaAbastecimentoGas();
        _CRUDAbastecimentoGas = new CRUDAbastecimentoGas();

        HeaderAuditoria("AbastecimentoGas", _abastecimentoGas);

        KoBindings(_abastecimentoGas, "knockoutAbastecimentoGas");
        KoBindings(_pesquisaAbastecimentoGas, "knockoutPesquisaAbastecimentoGas");
        KoBindings(_CRUDAbastecimentoGas, "knockoutCRUDAbastecimentoGas");

        new BuscarClientes(_abastecimentoGas.Base, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
        new BuscarClientes(_pesquisaAbastecimentoGas.Base, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
        new BuscarProdutos(_abastecimentoGas.Produto, null, null, null, null, null, null, null, null, _abastecimentoGas.Base);

        loadGridAbastecimentoGas();
        loadAutorizacaoSolicitacaoGas();

        setarEventos();
    });
}

function loadModalParcialAbastecimentoGas() {
    loadConteudosHTML().then(function () {
        _abastecimentoGas = new AbastecimentoGas();
        _CRUDAbastecimentoGas = new CRUDAbastecimentoGas();

        HeaderAuditoria("AbastecimentoGas", _abastecimentoGas);

        KoBindings(_abastecimentoGas, "knockoutAbastecimentoGas");
        KoBindings(_CRUDAbastecimentoGas, "knockoutCRUDAbastecimentoGas");

        new BuscarClientes(_abastecimentoGas.Base, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
        new BuscarProdutos(_abastecimentoGas.Produto, null, null, null, null, null, null, null, null, _abastecimentoGas.Base);

        setarEventos();
        _telaParcial = true;
    });
}

function loadConteudosHTML() {
    return new Promise(function (resolve, reject) {
        $.get("Content/Static/Logistica/AbastecimentoGas/AbastecimentoGasPrincipal.html?dyn=" + guid(), function (data) {
            $("#knockoutAbastecimentoGas").html(data);
        }).then(function () {
            $.get("Content/Static/Logistica/AbastecimentoGas/AbastecimentoGasFooter.html?dyn=" + guid(), function (data) {
                $("#knockoutCRUDAbastecimentoGas").html(data);

                resolve();
            });
        })
    });
}

function setarEventos() {
    $(".sum, .sub").on("change", function () {
        var sum = 0
        $(".sum").each(function () {
            sum += Number(this.value.replace(".", "").replace(",", "."));
        });

        $(".sub").each(function () {
            sum -= Number(this.value.replace(".", "").replace(",", "."));
        });

        _abastecimentoGas.PrevisaoFechamento.val(sum.toFixed(2));
    });

    $("#" + _abastecimentoGas.Base.id).on("change", function () {
        if (string.IsNullOrWhiteSpace(_abastecimentoGas.Base.val()))
            _abastecimentoGas.Base.codEntity(0);
    });
}

function loadGridAbastecimentoGas() {
    var carregar = {
        descricao: "Carregar",
        id: guid(),
        evento: "onclick",
        metodo: function (pedidoGrid) { carregarAbastecimentoGasClick(pedidoGrid, false, false); },
        tamanho: "5",
        icone: ""
    };

    var duplicar = {
        descricao: "Duplicar",
        id: guid(),
        evento: "onclick",
        metodo: function (pedidoGrid) { carregarAbastecimentoGasClick(pedidoGrid, true, false); },
        tamanho: "5",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [carregar, duplicar]
    };

    _gridPesquisaAbastecimentoGas = new GridViewExportacao(_pesquisaAbastecimentoGas.Pesquisar.idGrid, "SolicitacaoAbastecimentoGas/Pesquisa", _pesquisaAbastecimentoGas, menuOpcoes);
    _gridPesquisaAbastecimentoGas.CarregarGrid();
}

function pesquisarAbastecimentoGas() {
    _gridPesquisaAbastecimentoGas.CarregarGrid();
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function adicionarAbastecimentoGas() {
    if (!ValidarCamposObrigatorios(_abastecimentoGas) || _abastecimentoGas.DataMedicao.val().length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios e selecione pelo menos uma data.");
        return;
    }

    salvarAbastecimento();
}

function salvarAdicionalAbastecimentoGas() {
    executarReST("SolicitacaoAbastecimentoGas/SalvarQuantidadeAdicional", RetornarObjetoPesquisa(_abastecimentoGas), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Msg);
                limparCamposAbastecimentoGas();
                
                if (_telaParcial)
                    Global.fecharModal("modalSolicitacaoAbastecimentoGas");
            }
            else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
    });
}

function salvarAbastecimento() {
    executarReST("SolicitacaoAbastecimentoGas/ValidarAbastecimentoGasDuplicado", RetornarObjetoPesquisa(_abastecimentoGas), function (retorno) {
        if (retorno.Success) {
            executarReST("SolicitacaoAbastecimentoGas/Adicionar", RetornarObjetoPesquisa(_abastecimentoGas), function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {

                        if (retorno.Data.Situacao != EnumSituacaoAprovacaoSolicitacaoGas.Aprovada)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "A solicitação de abastecimento foi adicionada e está aguardando aprovação.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "A solicitação de abastecimento foi adicionada com sucesso.");

                        limparCamposAbastecimentoGas();
                        _gridPesquisaAbastecimentoGas.CarregarGrid();
                    }
                    else {
                        exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
                }
            });
        } else {
            exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
    });

}

function limparCamposAbastecimentoGas() {
    LimparCampos(_abastecimentoGas);

    SetarEnableCamposKnockout(_abastecimentoGas, true);

    _CRUDAbastecimentoGas.Adicionar.visible(true);
    _CRUDAbastecimentoGas.VisualizarJustificativa.visible(false);
    _CRUDAbastecimentoGas.SalvarAdicional.visible(false);

    _abastecimentoGas.DataMedicao.visible(true);
    _abastecimentoGas.DataMedicaoEntidade.visible(false);

    if (!_telaParcial)
        limparSolicitacaoGasAprovacao();
    else
        _CRUDAbastecimentoGas.Limpar.visible(false);

    _abastecimentoGas.AdicionalVolumeRodoviarioCarregamentoProximoDia.visible(false);
    _abastecimentoGas.AdicionalDisponibilidadeTransferenciaProximoDia.visible(false);
}

function carregarAbastecimentoGasClick(e, duplicar, exibirModalParcial) {
    limparCamposAbastecimentoGas();

    if (duplicar)
        setarAbastecimentoDuplicado(e.Codigo);

    executarReST("SolicitacaoAbastecimentoGas/BuscarPorCodigo", { Codigo: e.Codigo, Duplicar: duplicar }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {

                if (!_telaParcial)
                    exibirFiltrosClick(_pesquisaAbastecimentoGas);

                SetarEnableCamposKnockout(_abastecimentoGas, duplicar);

                PreencherObjetoKnout(_abastecimentoGas, retorno);

                _abastecimentoGas.DataMedicao.visible(duplicar);
                _abastecimentoGas.DataMedicaoEntidade.visible(!duplicar);
                _CRUDAbastecimentoGas.Adicionar.visible(duplicar);

                _abastecimentoGas.AdicionalVolumeRodoviarioCarregamentoProximoDia.visible(retorno.Data.PermiteAdicionarVolumeExtra);
                _abastecimentoGas.AdicionalDisponibilidadeTransferenciaProximoDia.visible(retorno.Data.PermiteAdicionarVolumeExtra);
                _CRUDAbastecimentoGas.SalvarAdicional.visible(retorno.Data.PermiteAdicionarVolumeExtra);

                if (!duplicar && !_telaParcial)
                    preencherSolicitacaoAbastecimentoGasAprovacao(e.Codigo);
                else if (retorno.Data.DataMedicao != null) {
                    var ordemData = obterDatasMedicoes().find(function (elemento) {
                        return elemento.text == retorno.Data.DataMedicao;
                    }).value;

                    _abastecimentoGas.DataMedicao.val(ordemData);
                }

                if (exibirModalParcial)
                    Global.abrirModal('modalSolicitacaoAbastecimentoGas');
            }
            else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function obterDatasMedicoes() {
    var opcao = { text: Global.DataAtual(), value: 0 };

    var datasMedicoes = [];

    datasMedicoes.push(opcao);

    for (var i = 1; i < 6; i++) {
        opcao = { text: moment().add(i, 'days').format("DD/MM/YYYY"), value: i };
        datasMedicoes.push(opcao);
    }

    return datasMedicoes;
}

function setarAbastecimentoDuplicado(codigo) {
    _abastecimentoGas.AbastecimentoDuplicado.val(codigo);
}
