/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Pedido.js" />
/// <reference path="DataCarregamento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCarregamento.js" />
/// <reference path="../../Consultas/Tranportador.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _retiradaProduto;
var _retiradaProdutoTabs;
var _pesquisaRetiradaProduto;
var _gridRetiradaProduto; RetiradaProduto
var _dadosUsuarioLogado;
var _veiculos = [];
var _veiculo;
var _filiais;
var _tipoOperacao;

var RetiradaProduto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Filial = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.Unidade.getRequiredFieldDescription(), required: true, def: true, options: ko.observable([]), eventChange: alterarDataCarregamentoLista, getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true) });
    this.FilialDescricao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.TipoOperacao = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.TipoAgendamento.getRequiredFieldDescription(), required: false, options: ko.observable([]), getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(false) });
    this.TipoOperacaoDescricao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.PlacaVeiculo = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.PlacaDoVeiculo.getRequiredFieldDescription(), required: true, getType: typesKnockout.placa, val: ko.observable(""), enable: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.ModeloDoVeiculo.getRequiredFieldDescription(), required: true, options: ko.observable([]), getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true) });
    this.ModeloVeiculoDescricao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.RetiradaProduto.Transportadora.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
    this.NomeTransportadora = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.Transportadora.getFieldDescription(), required: ko.observable(false), getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(false) });
    this.BotaoVisualizacaoTransportadora = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.RetiradaProduto.NaoPossuiNaoLocalizouTransportadora), eventClick: clickBotaoVisualizacaoTransportadora, visible: ko.observable(true) });
    this.ObservacaoTransportador = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.Observacao, val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(false), maxlength: 400 });

    this.Motorista = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.RetiradaProduto.CPFDoMotorista.getRequiredFieldDescription()), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), getType: typesKnockout.cpf, required: ko.observable(false), visible: ko.observable(true) });
    this.Motorista.val.subscribe(function (novoValor) {
        if (novoValor.length == 14 && _CONFIGURACAO_TMS.Pais == EnumPaises.Brasil) {
            if (!ValidarCPF(novoValor, false)) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.DadosInvalidos, Localization.Resources.Gerais.Geral.CPFInformadoInvalido);
                _retiradaProduto.NomeMotorista.val("");
            }
        } else {
            _retiradaProduto.NomeMotorista.val("");
        }
    });

    this.CpfMotorista = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.RetiradaProduto.CPFDoMotorista.getRequiredFieldDescription()), maxlength: 14, getType: typesKnockout.cpf, required: ko.observable(false), visible: ko.observable(false) });
    this.NomeMotorista = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.RetiradaProduto.NomeDoMotorista.getFieldDescription()), getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(false), required: ko.observable(false) });
    this.BotaoVisualizacaoMotorista = PropertyEntity({ text: ko.observable(Localization.Resources.Pedidos.RetiradaProduto.NaoPossuiNaoLocalizouMotorista), eventClick: clickBotaoVisualizacaoMotorista, visible: ko.observable(true) });
    this.MotoristaCadastrado = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: ko.observable(true) });

    this.EmailNotificacao = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.EmailExtra, getType: typesKnockout.string, val: ko.observable("") });
    this.NumeroCarregamento = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.NumeroAgendamento, getType: typesKnockout.string, val: ko.observable(""), visible: false });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), val: ko.observable(true), options: _status, def: true });
    this.MensagemProblemaCarregamento = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });

    this.DataRetirada = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.DataRetirada, val: ko.observable(""), visible: false });

    this.Data = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.Data.getRequiredFieldDescription(), getType: typesKnockout.string, val: ko.observable(""), visible: false });
    this.Hora = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.Hora.getRequiredFieldDescription(), getType: typesKnockout.string, val: ko.observable(""), visible: false });

    this.Pedidos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TipoOperacaoPedido = PropertyEntity({ getType: typesKnockout.int, val: ko.observable("") });

    this.Produtos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.CapacidadeVeiculo = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.CapacidadeDoVeiculo.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(0) });
    this.PesoTotal = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.PesoTotal.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(0) });
    this.EspacoDisponivel = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.Disponivel.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(0) });
    this.Ocupacao = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.Ocupacao.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(0) });

    this.Proximo = PropertyEntity({ eventClick: DadosBasicosProximaEtapa, type: types.event, text: Localization.Resources.Gerais.Geral.Proximo, visible: ko.observable(true) });
}

var RetiradaProdutoTabs = function () {
    this.Confirmacao = PropertyEntity({ eventClick: confirmacaoTabClick, type: types.event, text: Localization.Resources.Pedidos.RetiradaProduto.Confirmacao, visible: ko.observable(true), enable: ko.observable(false) });
};

var PesquisaRetiradaProduto = function () {
    this.Situacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao, issue: 557, val: ko.observable(true), options: _statusPesquisa, def: true });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRetiradaProduto.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Pedidos.RetiradaProduto.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

function buscaFiliais() {
    return new Promise(function (resolve) {
        executarReST("RetiradaProduto/BuscarFiliais", { Codigo: _retiradaProduto.Codigo }, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _filiais = retorno.Data.Filiais.map(function (d) { return { value: d.Codigo, text: d.Descricao } });
                _retiradaProduto.Filial.options(_filiais);
            }

            resolve();
        });
    });
}

function buscaTipoOperacao() {
    return new Promise(function (resolve) {
        executarReST("RetiradaProduto/BuscarTipoOperacoes", { Codigo: _retiradaProduto.Codigo }, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _tipoOperacao = retorno.Data.TipoOperacaos.map(function (d) { return { value: d.Codigo, text: d.Descricao } });
                _retiradaProduto.TipoOperacao.options(_tipoOperacao);
            }

            resolve();
        });
    });
}

function buscaDadosUsuarioLogado() {
    return new Promise(function (resolve) {
        executarReST("Usuario/DadosUsuarioLogado", { Codigo: _retiradaProduto.Codigo }, function (retorno) {
            if (retorno.Success && retorno.Data)
                _dadosUsuarioLogado = retorno.Data;

            resolve();
        });
    });
}

function buscaModeloVeiculo() {
    return new Promise(function (resolve) {
        executarReST("RetiradaProduto/BuscarModeloVeiculos", { Codigo: _retiradaProduto.Codigo, Filial: _retiradaProduto.Filial.val() }, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _veiculos = retorno.Data.ModeloVeiculos.map(function (d) { return { value: d.Codigo, text: d.Descricao, capacidade: d.CapacidadePesoTransporte } });
                _retiradaProduto.ModeloVeiculo.options(_veiculos);
            }

            resolve();
        });
    });
}

function obterFilial() {
    for (var i = 0; i < _filiais.length; i++) {
        if (_retiradaProduto.Filial.val() == _filiais[i].value) {
            _pedido.Filial.val(_filiais[i].value);
            return _filiais[i].text;
        }
    }
}

function obterTipoOperacao() {
    setTimeout(function () {
        for (var i = 0; i < _tipoOperacao.length; i++) {
            if (_retiradaProduto.TipoOperacao.val() == _tipoOperacao[i].value) {
                return _tipoOperacao[i].text;
            }
        }
    }, 2000);
}

function obterModeloVeiculo() {
    for (var i = 0; i < _veiculos.length; i++) {
        if (_retiradaProduto.ModeloVeiculo.val() == _veiculos[i].value) {
            return _veiculos[i].text;
        }
    }
}

function carregarTransportadora() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
        return;

    if (_retiradaProduto.Transportador.codEntity() > 0) {
        _retiradaProduto.Transportador.visible(true);
        _retiradaProduto.Transportador.required(true);
        _retiradaProduto.NomeTransportadora.visible(false);
        _retiradaProduto.NomeTransportadora.required(false);
        _retiradaProduto.BotaoVisualizacaoTransportadora.text(Localization.Resources.Pedidos.RetiradaProduto.NaoPossuiNaoLocalizouTransportadora);
    }
    else {
        _retiradaProduto.Transportador.visible(false);
        _retiradaProduto.Transportador.required(false);
        _retiradaProduto.NomeTransportadora.visible(true);
        _retiradaProduto.NomeTransportadora.required(true);
        _retiradaProduto.BotaoVisualizacaoTransportadora.text(Localization.Resources.Pedidos.RetiradaProduto.DesejaLocalizarTransportadora);
    }
}

//*******EVENTOS*******
function loadRetiradaProduto() {
    _pesquisaRetiradaProduto = new PesquisaRetiradaProduto();
    KoBindings(_pesquisaRetiradaProduto, "knockoutPesquisaRetiradaProduto", false, _pesquisaRetiradaProduto.Pesquisar.id);
    _retiradaProduto = new RetiradaProduto();

    _retiradaProdutoTabs = new RetiradaProdutoTabs();

    KoBindings(_retiradaProduto, "knockoutRetiradaProduto");
    KoBindings(_retiradaProdutoTabs, "knoutRetiradaProdutoTabs");

    LocalizeCurrentPage();

    HeaderAuditoria("Carregamento", _retiradaProduto);
    carregarGridPedidos();

    _retiradaProduto.Filial.val.subscribe(function () {
        LimparCamposPedido()
        buscaModeloVeiculo().then();
    });

    _retiradaProduto.ModeloVeiculo.val.subscribe(AtualizaVeiculo);

    loadConfirmacao();
    loadDataCarregamento();
    LoadDadosParaPreenchimento();
    buscarRetiradaProduto();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        _retiradaProduto.BotaoVisualizacaoTransportadora.visible(false);
        _retiradaProduto.Transportador.required(true);

        buscaDadosUsuarioLogado().then(function () {
            if (_dadosUsuarioLogado) {
                _retiradaProduto.Transportador.val(_dadosUsuarioLogado.Empresa.Descricao);
                _retiradaProduto.Transportador.entityDescription(_dadosUsuarioLogado.Empresa.Descricao);
                _retiradaProduto.Transportador.codEntity(_dadosUsuarioLogado.Empresa.Codigo);
                _retiradaProduto.Transportador.visible(false);
            }
        });
    }

    if (_CONFIGURACAO_TMS.Pais != EnumPaises.Exterior)
        VisibilidadeMotorista(_CONFIGURACAO_TMS.PesquisarMotorista);

    new BuscarTransportadores(_retiradaProduto.Transportador, retornoTransportador);
    new BuscarMotoristasPorCPFPortalRetira(_retiradaProduto.Motorista, retornoMotorista, _retiradaProduto.Transportador);

    $("#tab-Pedidos").on('click', function () { DadosBasicosProximaEtapa(); });
}

function clickBotaoVisualizacaoTransportadora() {
    if (_retiradaProduto.Transportador.visible()) {
        _retiradaProduto.NomeTransportadora.visible(true);
        _retiradaProduto.NomeTransportadora.required(true);
        _retiradaProduto.Transportador.visible(false);
        _retiradaProduto.Transportador.required(false);
        _retiradaProduto.BotaoVisualizacaoTransportadora.text(Localization.Resources.Pedidos.RetiradaProduto.DesejaLocalizarTransportadora);
    } else {
        _retiradaProduto.NomeTransportadora.visible(false);
        _retiradaProduto.NomeTransportadora.required(false);
        _retiradaProduto.Transportador.visible(true);
        _retiradaProduto.Transportador.required(true);
        _retiradaProduto.BotaoVisualizacaoTransportadora.text(Localization.Resources.Pedidos.RetiradaProduto.NaoPossuiNaoLocalizouTransportadora);
    }
}

function clickBotaoVisualizacaoMotorista() {
    if (_retiradaProduto.Motorista.visible()) {
        _retiradaProduto.CpfMotorista.visible(true);
        _retiradaProduto.CpfMotorista.required(true);
        _retiradaProduto.NomeMotorista.enable(true);
        _retiradaProduto.NomeMotorista.required(true);
        _retiradaProduto.NomeMotorista.text(Localization.Resources.Pedidos.RetiradaProduto.NomeDoMotorista.getRequiredFieldDescription());
        _retiradaProduto.Motorista.visible(false);
        _retiradaProduto.Motorista.required(false);
        _retiradaProduto.BotaoVisualizacaoMotorista.text(Localization.Resources.Pedidos.RetiradaProduto.DesejaLocalizarMotorista);
        _retiradaProduto.MotoristaCadastrado.val(false);
    } else {
        _retiradaProduto.CpfMotorista.visible(false);
        _retiradaProduto.CpfMotorista.required(false);
        _retiradaProduto.NomeMotorista.enable(false);
        _retiradaProduto.NomeMotorista.required(false);
        _retiradaProduto.NomeMotorista.text(Localization.Resources.Pedidos.RetiradaProduto.NomeDoMotorista.getFieldDescription());
        _retiradaProduto.Motorista.visible(true);
        _retiradaProduto.Motorista.required(true);
        _retiradaProduto.BotaoVisualizacaoMotorista.text(Localization.Resources.Pedidos.RetiradaProduto.NaoPossuiNaoLocalizouMotorista);
        _retiradaProduto.MotoristaCadastrado.val(true);
    }
}

function confirmacaoTabClick() {
    if (_retiradaProduto.Codigo.val() <= 0) {
        adicionarClick();
    } else {
        carregarInformacoesConfirmacao();
        liberaBotoesCrud();
    }
}

function DadosBasicosProximaEtapa() {
    const ambientesPermitidosParaObterTransportador = [
        EnumTipoServicoMultisoftware.MultiEmbarcador,
        EnumTipoServicoMultisoftware.MultiTMS
    ]

    if (ambientesPermitidosParaObterTransportador.includes(_CONFIGURACAO_TMS.TipoServicoMultisoftware))
        obterTransportador();

    ExibirProximaEtapa("tabPedidos");

    confirmacaoTabClick();
}

function ExibirProximaEtapa(tab) {
    Global.ExibirAba(tab);
    //$(`#${tab}`).tab("show");
}

function LoadDadosParaPreenchimento() {
    Promise.all([
        buscaFiliais(),
        buscaTipoOperacao(),
        buscaModeloVeiculo()
    ]).then(function () {
        carregarRetiradaProdutoGlobal();
    })
}

function carregarRetiradaProdutoGlobal() {
    var codigo = window.GlobalCodigoRetiradaProduto;
    window.GlobalCodigoRetiradaProduto = 0;

    if (codigo > 0) {
        buscarRetiradaprodutoPorcodigo(codigo);
        return true;
    }
    return false;
}

function buscarRetiradaprodutoPorcodigo(codigo) {
    limparCamposRetiradaProduto();

    _retiradaProduto.Codigo.val(codigo);

    // Busca informacoes para edicao
    buscarPorCodigo();
}

function editarRetiradaProdutoClick(itemGrid) {
    // Limpa os campos
    buscarRetiradaprodutoPorcodigo(itemGrid.Codigo);
}

function AtualizaVeiculo() {
    var veiculo = getVeiculo();
    if (veiculo) {
        _pedido.CapacidadeVeiculo.val(veiculo.capacidade);
        calcularPesoProdutos();
    }
}

function adicionarClick(e, sender) {
    if (!ValidarCamposObrigatorios(_retiradaProduto)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        ResetarTabs();
        return;
    }
    if (_pedido.EspacoDisponivel.val() < 0 || _pedido.EspacoDisponivel.val() == undefined) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pedidos.RetiradaProduto.ACapacidadeDoCaminhaoFoiexcedida);
        return;
    }
    if (_retiradaProduto.DataRetirada.val() == '') {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pedidos.RetiradaProduto.DataRetiradaObrigatoria);
        return;
    }
    if (!validarProdutos()) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pedidos.RetiradaProduto.EObrigatorioTerAoMenosUmProdutoSelecionadoNoPedido);
        return;
    }

    executarReST("RetiradaProduto/Adicionar", ObterRetiradaProdutoSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                _gridRetiradaProduto.CarregarGrid();
                limparCamposRetiradaProduto();
                _retiradaProduto.Codigo.val(retorno.Data.Codigo);

                buscarPorCodigo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function atualizarClick(e, sender) {
    if (!ValidarCamposObrigatorios(_retiradaProduto)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        ResetarTabs();
        return;
    }
    if (_pedido.EspacoDisponivel.val() < 0 || _pedido.EspacoDisponivel.val() == undefined) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pedidos.RetiradaProduto.ACapacidadeDoCaminhaoFoiexcedida);
        return;
    }
    if (_retiradaProduto.DataRetirada.val() == '') {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pedidos.RetiradaProduto.DataRetiradaObrigatoria);
        return;
    }
    if (!validarProdutos()) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pedidos.RetiradaProduto.EObrigatorioTerAoMenosUmProdutoSelecionadoNoPedido);
        return;
    }

    // Assim o campo checkbox quando false fica vazio e não passa pela validação
    if (!_confirmacao.CargaComplexa.val())
        _confirmacao.CargaComplexa.val("");
    if (!_confirmacao.CargaNecessitaLonamento.val())
        _confirmacao.CargaNecessitaLonamento.val("");

    if (!ValidarCamposObrigatorios(_confirmacao)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    setEmailConfirmacao();
    setObservacaoTransportadorConfirmacao();

    executarReST("RetiradaProduto/Atualizar", ObterRetiradaProdutoSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.SituacaoCarregamento == EnumSituacaoCarregamento.FalhaIntegracao)
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pedidos.RetiradaProduto.AgendamentoIncompleto, Localization.Resources.Pedidos.RetiradaProduto.NaoFoiPossivelConcluirSeuAgendamentoFavorVerificarSeSeuPedidoEncontraSeLiberadoEntreEmContatoComEquipeLogisticaLocal, 1000 * 15);
                else
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Pedidos.RetiradaProduto.AgendamentoConfirmadoComSucesso);

                _gridRetiradaProduto.CarregarGrid();
                limparCamposRetiradaProduto();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.RetiradaProduto.RealmenteDesejaExcluirEsseCadastro, function () {
        ExcluirPorCodigo(_retiradaProduto, "RetiradaProduto/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridRetiradaProduto.CarregarGrid();
                    limparCamposRetiradaProduto();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }

        }, null);
    });
}

function ObterRetiradaProdutoSalvar() {
    _retiradaProduto.Pedidos.val(obterPedidos());
    var retiradaProduto = RetornarObjetoPesquisa(_retiradaProduto);

    return retiradaProduto;
}

function cancelarClick(e) {
    limparCamposRetiradaProduto();
}

function reenviarIntegracaoClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.RetiradaProduto.RealmenteDesejaReenviarIntegracao, function () {
        var dados = {
            Codigo: _retiradaProduto.Codigo.val(),
            DataRetirada: _retiradaProduto.DataRetirada.val()
        };
        executarReST("RetiradaProduto/ReenviarIntegracao", dados, function (retorno) {
            if (!retorno.Success)
                return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);

            if (!retorno.Data)
                return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);

            if (retorno.Data.SituacaoCarregamento == EnumSituacaoCarregamento.FalhaIntegracao)
                return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Pedidos.RetiradaProduto.FalhaAoIntegrarAgendamentoAoERP);

            buscarPorCodigo();
        });
    });
}

function editarRetiradaProdutoClick(itemGrid) {
    // Limpa os campos
    limparCamposRetiradaProduto();

    _retiradaProduto.Confirmar.visible(false);

    _retiradaProduto.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    buscarPorCodigo();
}

function setEmailConfirmacao() {
    _retiradaProduto.EmailNotificacao.val(_confirmacao.EmailNotificacao.val());
}

function setObservacaoTransportadorConfirmacao() {
    _retiradaProduto.ObservacaoTransportador.val(_confirmacao.ObservacaoTransportador.val());
}

//*******MÉTODOS*******
function buscarPorCodigo() {
    BuscarPorCodigo(_retiradaProduto, "RetiradaProduto/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _retiradaProdutoTabs.Confirmacao.enable(true);
                _pesquisaRetiradaProduto.ExibirFiltros.visibleFade(false);
                _infoCarregamento.DataCarregamentoDisponibilidade.val(_infoCarregamento.DataCarregamentoDisponibilidade.def);
                if (_retiradaProduto.Situacao.val() != EnumSituacaoCarregamento.FalhaIntegracao) {
                    _retiradaProduto.DataRetirada.val(_retiradaProduto.Data.val() + " " + _retiradaProduto.Hora.val());
                    _infoCarregamento.DataCarregamentoDisponibilidade.val(_retiradaProduto.Data.val());
                }

                recarregarGridPedido();

                if (!_infoCarregamento.DataCarregamentoDisponibilidade.val()) {
                    _infoCarregamento.DataCarregamentoDisponibilidade.val(moment().add(1, 'days').format("DD/MM/YYYY"));
                    _infoCarregamento.DataCarregamentoDisponibilidade.updateValue();
                }

                alterarDataCarregamentoLista();

                //$("a[href='#tabConfirmacao']").click();
                Global.ExibirAba("tabConfirmacao");
                carregarInformacoesConfirmacao();
                liberaBotoesCrud();
                carregarTransportadora();
                ControlarVisibilidadeCampos();

                if (arg.Data.NaoPermitirEditarAgendamento)
                    _confirmacao.Atualizar.enable(false);

                if (arg.Data.NaoPermitirExcluirAgendamento)
                    _confirmacao.Excluir.enable(false);

            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}

function carregarGridPedidos() {
    LoadPedido();
}

function ControlarVisibilidadeCampos() {
    if (_retiradaProduto.Situacao.val() == EnumSituacaoCarregamento.FalhaIntegracao) {
        BloquearCampos(_retiradaProduto);
        _pedido.Pedido.visible(false);
        _confirmacao.EmailNotificacao.enable(false);
    } else {
        DesbloquearCampos(_retiradaProduto);
        _pedido.Pedido.visible(true);
        _confirmacao.EmailNotificacao.enable(true);
    }
    VisibilidadeMotorista(_CONFIGURACAO_TMS.PesquisarMotorista);
}

function limparCamposRetiradaProduto() {
    _infoCarregamento.Adicionar.visible(true);
    _infoCarregamento.Proximo.visible(false);

    LimparHorarioSelecionado();

    LimparCampos(_retiradaProduto);
    LimparAbaConfirmaca();
    LimparCamposPedido();
    _retiradaProduto.DataRetirada.val(Global.DataAtual());
    recarregarGridPedido();
    _gridHorariosInfoCarregamento.CarregarGrid();
    ResetarTabs();
    ControlarVisibilidadeCampos();
}

function liberaBotoesCrud() {
    if (_retiradaProdutoTabs.Confirmacao.enable()) {
        if (_retiradaProduto.Codigo.val() > 0 && _retiradaProduto.Situacao.val() != EnumSituacaoCarregamento.Bloqueado) {
            _infoCarregamento.Adicionar.visible(false);
            _infoCarregamento.Proximo.visible(true);
        } else {
            _infoCarregamento.Adicionar.visible(false);
            _infoCarregamento.Proximo.visible(true);
        }
    }
}

function ResetarTabs() {
    Global.ResetarAbas();
    //$("#tab-DadosBasicos").tab("show");
}

function validarProdutos() {
    ObterRetiradaProdutoSalvar();
    var pedidos = JSON.parse(_retiradaProduto.Pedidos.val());

    for (var i = 0; i < pedidos.length; i++) {
        //console.log(pedidos[i]);
        if (pedidos[i].Produtos.length <= 0)
            return false;
    }

    return true;
}

function retornoMotorista(data) {
    if (data != null && data != undefined) {

        _retiradaProduto.Motorista.codEntity(data.Codigo);
        _retiradaProduto.Motorista.val(data.CPF);

        _retiradaProduto.NomeMotorista.val(data.Nome);
    } else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pedidos.RetiradaProduto.MotoristaNaoLocalizado);
}

function retornoTransportador(data) {
    if (_retiradaProduto.Transportador.codEntity() > 0 && _retiradaProduto.Transportador.codEntity() != data.Codigo) {
        _retiradaProduto.Motorista.codEntity(0);
        _retiradaProduto.Motorista.val("");
        _retiradaProduto.NomeMotorista.val("");
    }

    _retiradaProduto.Transportador.codEntity(data.Codigo);
    _retiradaProduto.Transportador.val(data.Descricao);
}

function VisibilidadeMotorista(status) {
    _retiradaProduto.Motorista.visible(status);
    _retiradaProduto.Motorista.required(status);

    _retiradaProduto.CpfMotorista.visible(!status);
    _retiradaProduto.CpfMotorista.required(!status);

    _retiradaProduto.NomeMotorista.enable(!status);
    _retiradaProduto.NomeMotorista.required(!status);
    _retiradaProduto.NomeMotorista.text(!status ? Localization.Resources.Pedidos.RetiradaProduto.NomeDoMotorista.getRequiredFieldDescription() : Localization.Resources.Pedidos.RetiradaProduto.NomeDoMotorista.getFieldDescription());

    _retiradaProduto.MotoristaCadastrado.val(status);
}
