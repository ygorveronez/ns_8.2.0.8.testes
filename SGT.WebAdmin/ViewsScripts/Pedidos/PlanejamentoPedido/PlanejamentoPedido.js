/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumRequisitanteColeta.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPlanejamentoPedido.js" />
/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../Enumeradores/EnumTipoTomador.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="PlanejamentoPedidoDefinicaoVeiculo.js" />
/// <reference path="PlanejamentoPedidoDisponibilidade.js" />
/// <reference path="PlanejamentoPedidoDisponibilidadeMotorista.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridPlanejamentoPedido;
var _pesquisaPlanejamentoPedido;
var _planejamentoPedidoDuplicarPedido;
var _planejamentoPedidoPlacaCarregamento;
var _planejamentoPedidoEmail;
var _pedidoGrupo = false;

/*
 * Declaração das Classes
 */

var PesquisaPlanejamentoPedido = function () {
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: "Tipo de Remetente: ", issue: 306, required: true, visible: ko.observable(true), eventChange: tipoPessoaChange, cssClass: ko.observable("col col-xs-12 col-md-6") });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-md-6") });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(false), cssClass: ko.observable("col col-xs-12 col-md-6") });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoPedido.Aberto), options: EnumSituacaoPedido.obterOpcoesPesquisa(), def: EnumSituacaoPedido.Aberto, text: "Situação: ", cssClass: ko.observable("col col-xs-12 col-md-2") });
    this.SituacaoPlanejamentoPedido = PropertyEntity({ val: ko.observable(EnumSituacaoPlanejamentoPedido.Todas), options: EnumSituacaoPlanejamentoPedido.obterOpcoesPesquisa(), def: EnumSituacaoPlanejamentoPedido.Todas, text: "Situação de Planejamento: ", cssClass: ko.observable("col col-xs-12 col-md-4") });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), cssClass: ko.observable("col col-xs-12 col-md-2") });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, val: ko.observable(Global.Data(EnumTipoOperacaoDate.Add, 1, 'd')), def: Global.Data(EnumTipoOperacaoDate.Add, 1, 'd'), cssClass: ko.observable("col col-xs-12 col-md-2") });
    this.NumeroPedidoEmbarcador = PropertyEntity({ val: ko.observable(""), def: "", text: "Número Pedido no Embarcador:", issue: 902, cssClass: ko.observable("col col-xs-12 col-md-2") });
    this.NumeroPedido = PropertyEntity({ val: ko.observable(""), def: "", text: "Número do Pedido:", configInt: { precision: 0, allowZero: true }, getType: typesKnockout.int, visible: ko.observable(true) });
    this.CodigoCargaEmbarcador = PropertyEntity({ val: ko.observable(""), def: "", text: "Número da Carga:", cssClass: ko.observable("col col-xs-12 col-md-2") });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), cssClass: ko.observable("col col-xs-12 col-md-6") });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", issue: 143, idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", issue: 145, idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });
    this.CidadePoloOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Polo de Origem:", issue: 831, idBtnSearch: guid() });
    this.CidadePoloDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Polo de Destino:", issue: 831, idBtnSearch: guid() });
    this.ProvedorOS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Provedor:", issue: 831, idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.UsuarioUtilizaSegregacaoPorProvedor) });
    this.PaisOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "País de Origem:", idBtnSearch: guid() });
    this.PaisDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "País de Destino:", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Filial:", issue: 70, idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Tipo de Operação:", issue: 121, visible: ko.observable(true), idBtnSearch: guid() });
    this.CategoriaOS = PropertyEntity({ getType: typesKnockout.selectMultiple, val: ko.observable(""), options: EnumCategoriaOS.obterOpcoes(), def: ko.observable([]), text: "Categoria OS: ", visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum)});
    this.TipoOSConvertido = PropertyEntity({ getType: typesKnockout.selectMultiple, val: ko.observable(""), options: EnumTipoOSConvertido.obterOpcoes(), def: ko.observable([]), text: "Tipo OS Convertido: ", visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) });

    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            _pesquisaPlanejamentoPedido.ExibirFiltros.visibleFade(false);
            limparSelecionados();
            _gridPlanejamentoPedido.CarregarGrid();

            fecharDisponibilidades();

        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            }
            else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ExibirDisponibilidade = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: ExibirPlanejamentoPedidoDisponibilidadeClick, text: ko.observable("Exibir Disponibilidade"), visible: ko.observable(true) });
    this.ExibirDisponibilidadeMotorista = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: ExibirPlanejamentoPedidoDisponibilidadeMotoristaClick, text: ko.observable("Exibir Disponibilidade Motorista"), visible: ko.observable(true) });
    this.ListaDisponibilidade = ko.observableArray();

    this.DuplicarPedidos = PropertyEntity({
        eventClick: duplicarPedidosClick, type: types.event, text: "Duplicar pedidos", idGrid: guid(), visible: ko.observable(false)
    });

    this.EnviarPedidosAurora = PropertyEntity({
        eventClick: enviarPedidosAuroraClick, type: types.event, text: "Enviar placas", idGrid: guid(), visible: ko.observable(false)
    });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todos", visible: ko.observable(true) });
};

var PlanejamentoPedidoDuplicarPedido = function () {
    this.Confirmar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: confirmarDuplicarPedidoClick, text: "Confirmar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: fecharDuplicarPedidoClick, text: "Cancelar", visible: ko.observable(true) });

    this.DataPedido = PropertyEntity({ text: "*Data do Pedido: ", required: true, getType: typesKnockout.date, val: ko.observable(Global.Data(EnumTipoOperacaoDate.Add, 1, 'd')), def: Global.Data(EnumTipoOperacaoDate.Add, 1, 'd') });
    this.QuantidadeDuplicar = PropertyEntity({ val: ko.observable(1), def: 1, required: true, text: "*Quantidade a Duplicar:", configInt: { precision: 0, allowZero: false }, getType: typesKnockout.int, visible: ko.observable(true) });

    this.CodigoPedido = PropertyEntity({});
};

var PlanejamentoPedidoEmail = function () {
    this.Confirmar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: confirmarEmailClick, text: "Confirmar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: fecharEmailClick, text: "Cancelar", visible: ko.observable(true) });

    this.Email = PropertyEntity({ maxlength: 1000, type: types.entity, requerid: true, codEntity: ko.observable(0), text: "*E-mail:", idBtnSearch: guid() });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridPlanejamentoPedido() {
    var opcaoDefinirVeiculo = { descricao: "Definir Veículo", id: guid(), metodo: definirVeiculoClick, icone: "", visibilidade: controlarVisibilidadeDefinirVeiculo };
    var opcaoDefinirPlacaCarregamento = { descricao: "Definir Placa de Carregamento", id: guid(), metodo: definirPlacaCarregamentoClick, icone: "", visibilidade: controlarVisibilidadeDefinirPlacaCarregamento };
    var opcaoRemoverVeiculo = { descricao: "Remover Veículo", id: guid(), metodo: removerVeiculoClick, icone: "", visibilidade: controlarVisibilidadeRemoverVeiculo };
    var opcaoSubstituirVeiculo = { descricao: "Substituir Veículo", id: guid(), metodo: substituirVeiculoClick, icone: "", visibilidade: controlarVisibilidadeSubstituirVeiculo };
    var opcaoDefinirMotorista = { descricao: "Definir Motorista", id: guid(), metodo: definirMotoristaClick, icone: "", visibilidade: controlarVisibilidadeDefinirMotorista };
    var opcaoSubstituirMotorista = { descricao: "Substituir Motorista", id: guid(), metodo: substituirMotoristaClick, icone: "", visibilidade: controlarVisibilidadeSubstituirMotorista };
    var opcaoRemoverMotorista = { descricao: "Remover Motorista", id: guid(), metodo: removerMotoristaClick, icone: "", visibilidade: controlarVisibilidadeRemoverMotorista };
    var opcaoSituacaoPendente = { descricao: "Pendente", id: guid(), metodo: situacaoPendenteClick, icone: "", visibilidade: controlarVisibilidadeSituacaoPendente };
    //var opcaoSituacaoEntregueAoMotorista = { descricao: "Entregue ao Motorista", id: guid(), metodo: situacaoEntregueAoMotoristaClick, icone: "", visibilidade: controlarVisibilidadeSituacaoEntregueAoMotorista };
    var opcaoSituacaoMotoristaNoLocalCarregamento = { descricao: "CT-e Emitido", id: guid(), metodo: situacaoMotoristaNoLocalCarregamentoClick, icone: "", visibilidade: controlarVisibilidadeSituacaoMotoristaNoLocalCarregamento };
    var opcaoSituacaoProblema = { descricao: "OK", id: guid(), metodo: situacaoProblemaClick, icone: "", visibilidade: controlarVisibilidadeSituacaoProblema };
    var opcaoDuplicarPedido = { descricao: "Duplicar o Pedido", id: guid(), metodo: duplicarPedidoClick, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 5, opcoes: [opcaoDefinirVeiculo,opcaoDefinirPlacaCarregamento, opcaoSubstituirVeiculo, opcaoRemoverVeiculo, opcaoDefinirMotorista, opcaoSubstituirMotorista, opcaoRemoverMotorista, opcaoSituacaoPendente, /*opcaoSituacaoEntregueAoMotorista,*/ opcaoSituacaoMotoristaNoLocalCarregamento, opcaoSituacaoProblema, opcaoDuplicarPedido] };

    var quantidadePorPagina = 100;

    var editarColuna = {
        permite: true,
        callback: editarColunaRetorno,
        atualizarRow: false
    };

    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaPlanejamentoPedido.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    };

    _gridPlanejamentoPedido = new GridView(_pesquisaPlanejamentoPedido.Pesquisar.idGrid, "PlanejamentoPedido/Pesquisa", _pesquisaPlanejamentoPedido, menuOpcoes, null, quantidadePorPagina, null, null, null, multiplaEscolha, quantidadePorPagina, editarColuna);
    _gridPlanejamentoPedido.SetPermitirRedimencionarColunas(true);
    _gridPlanejamentoPedido.SetGroup({ enable: true, propAgrupa: "DataCarregamentoPedido", dirOrdena: orderDir.asc });

    _gridPlanejamentoPedido.CarregarGrid();
}

function loadPlanejamentoPedido() {
    _pesquisaPlanejamentoPedido = new PesquisaPlanejamentoPedido();
    KoBindings(_pesquisaPlanejamentoPedido, "knockoutPesquisaPlanejamentoPedido", false, _pesquisaPlanejamentoPedido.Pesquisar.id);

    _planejamentoPedidoDuplicarPedido = new PlanejamentoPedidoDuplicarPedido();
    KoBindings(_planejamentoPedidoDuplicarPedido, "knockoutPlanejamentoPedidoDuplicarPedido");

    _planejamentoPedidoEmail = new PlanejamentoPedidoEmail();
    KoBindings(_planejamentoPedidoEmail, "knockoutPlanejamentoPedidoEmail");

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _pesquisaPlanejamentoPedido.TipoPessoa.visible(false);
        _pesquisaPlanejamentoPedido.Origem.visible(false);
        _pesquisaPlanejamentoPedido.Filial.visible(true);
        _pesquisaPlanejamentoPedido.NumeroPedido.visible(false);

        _pesquisaPlanejamentoPedido.CodigoCargaEmbarcador.cssClass("col col-xs-12 col-md-2");
        _pesquisaPlanejamentoPedido.NumeroPedidoEmbarcador.cssClass("col col-xs-12 col-md-2");
        _pesquisaPlanejamentoPedido.Situacao.cssClass("col col-xs-12 col-md-6 col-lg-2");
        _pesquisaPlanejamentoPedido.SituacaoPlanejamentoPedido.cssClass("col col-xs-12 col-md-6 col-lg-4");
        _pesquisaPlanejamentoPedido.Remetente.cssClass("col col-xs-12 col-md-6 col-lg-4");
        _pesquisaPlanejamentoPedido.GrupoPessoa.cssClass("col col-xs-12 col-md-6 col-lg-4");
        _pesquisaPlanejamentoPedido.Destinatario.cssClass("col col-xs-12 col-md-6 col-lg-4");
        _pesquisaPlanejamentoPedido.DataInicio.cssClass("col col-xs-12 col-md-2");
        _pesquisaPlanejamentoPedido.DataFim.cssClass("col col-xs-12 col-md-2");
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaPlanejamentoPedido.NumeroPedido.visible(true);

        _pesquisaPlanejamentoPedido.CodigoCargaEmbarcador.cssClass("col col-xs-12 col-md-3 col-lg-2");
        _pesquisaPlanejamentoPedido.NumeroPedidoEmbarcador.cssClass("col col-xs-12 col-md-3 col-lg-2");
        _pesquisaPlanejamentoPedido.TipoPessoa.cssClass("col col-xs-12 col-md-6 col-lg-2");
        _pesquisaPlanejamentoPedido.Situacao.cssClass("col col-xs-12 col-md-6 col-lg-4");
        _pesquisaPlanejamentoPedido.SituacaoPlanejamentoPedido.cssClass("col col-xs-12 col-md-6 col-lg-4");
        _pesquisaPlanejamentoPedido.Remetente.cssClass("col col-xs-12 12 col-md-6");
        _pesquisaPlanejamentoPedido.GrupoPessoa.cssClass("col col-xs-12 12 col-md-6");
        _pesquisaPlanejamentoPedido.Destinatario.cssClass("col col-xs-12 col-md-6");
    }

    new BuscarClientes(_pesquisaPlanejamentoPedido.Remetente);
    new BuscarGruposPessoas(_pesquisaPlanejamentoPedido.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarLocalidades(_pesquisaPlanejamentoPedido.Origem);
    new BuscarClientes(_pesquisaPlanejamentoPedido.Destinatario);
    new BuscarClientes(_pesquisaPlanejamentoPedido.Tomador);
    new BuscarVeiculos(_pesquisaPlanejamentoPedido.Veiculo);
    new BuscarMotorista(_pesquisaPlanejamentoPedido.Motorista);
    new BuscarLocalidades(_pesquisaPlanejamentoPedido.Destino);
    new BuscarLocalidadesPolo(_pesquisaPlanejamentoPedido.CidadePoloOrigem);
    new BuscarLocalidadesPolo(_pesquisaPlanejamentoPedido.CidadePoloDestino);
    new BuscarClientes(_pesquisaPlanejamentoPedido.ProvedorOS);
    new BuscarFilial(_pesquisaPlanejamentoPedido.Filial);
    new BuscarPaises(_pesquisaPlanejamentoPedido.PaisOrigem);
    new BuscarPaises(_pesquisaPlanejamentoPedido.PaisDestino);
    new BuscarTiposdeCarga(_pesquisaPlanejamentoPedido.TipoCarga);
    new BuscarTiposOperacao(_pesquisaPlanejamentoPedido.TipoOperacao);

    loadPlanejamentoVeiculoDefinicaoVeiculo();
    loadGridPlanejamentoPedido();
    loadPlanejamentoVeiculoDefinicaoPLacaCarregamento();
}

/*
 * Declaração das Funções Associadas a Eventos
 */
function definirPlacaCarregamentoClick(pedidoSelecionado) {
    exibirModalPlanejamentoVeiculoDefinicaoPlacaCarregamento(pedidoSelecionado.Codigo);
}
function definirVeiculoClick(pedidoSelecionado) {
    _planejamentoVeiculoDefinicaoVeiculo.Codigo.val(pedidoSelecionado.Codigo);
    $("#" + _planejamentoVeiculoDefinicaoVeiculo.Veiculo.idBtnSearch).click();
}

function substituirVeiculoClick(pedidoSelecionado) {
    _planejamentoVeiculoDefinicaoVeiculo.Codigo.val(pedidoSelecionado.Codigo);
    $("#" + _planejamentoVeiculoDefinicaoVeiculo.Veiculo.idBtnSearch).click();
}

function removerVeiculoClick(pedidoSelecionado) {
    exibirConfirmacao("Atenção!", "Deseja realmente remover o veículo do pedido?", function () {
        executarReST("PlanejamentoPedido/RemoverVeiculo", { Codigo: pedidoSelecionado.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _gridPlanejamentoPedido.CarregarGrid();
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Veículo removido com sucesso.");

                    forcarRecarregarGridPlanejamentoPedidoDisponibilidade();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        });
    });
}

function definirMotoristaClick(pedidoSelecionado) {
    _planejamentoVeiculoDefinicaoVeiculo.Codigo.val(pedidoSelecionado.Codigo);
    $("#" + _planejamentoVeiculoDefinicaoVeiculo.Motorista.idBtnSearch).click();
}

function substituirMotoristaClick(pedidoSelecionado) {
    _planejamentoVeiculoDefinicaoVeiculo.Codigo.val(pedidoSelecionado.Codigo);
    $("#" + _planejamentoVeiculoDefinicaoVeiculo.Motorista.idBtnSearch).click();
}

function removerMotoristaClick(pedidoSelecionado) {
    exibirConfirmacao("Atenção!", "Deseja realmente remover o motorista do pedido?", function () {
        executarReST("PlanejamentoPedido/RemoverMotorista", { Codigo: pedidoSelecionado.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _gridPlanejamentoPedido.CarregarGrid();
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Motorista removido com sucesso.");

                    forcarRecarregarGridPlanejamentoPedidoDisponibilidadeMotorista();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        });
    });
}

function duplicarPedidoClick(pedidoSelecionado) {
    _pedidoGrupo = false;

    LimparCampos(_planejamentoPedidoDuplicarPedido);

    _planejamentoPedidoDuplicarPedido.CodigoPedido.val(pedidoSelecionado.Codigo);

    exibirModalDuplicarPedidos();
}

function situacaoEntregueAoMotoristaClick(pedidoSelecionado) {
    alterarSituacaoPlanejamentoPedido(pedidoSelecionado, EnumSituacaoPlanejamentoPedido.EntregueAoMotorista);
}

function situacaoMotoristaNoLocalCarregamentoClick(pedidoSelecionado) {
    alterarSituacaoPlanejamentoPedido(pedidoSelecionado, EnumSituacaoPlanejamentoPedido.MotoristaNoLocalCarregamento);
}

function situacaoPendenteClick(pedidoSelecionado) {
    alterarSituacaoPlanejamentoPedido(pedidoSelecionado, EnumSituacaoPlanejamentoPedido.Pendente);
}

function situacaoProblemaClick(pedidoSelecionado) {
    alterarSituacaoPlanejamentoPedido(pedidoSelecionado, EnumSituacaoPlanejamentoPedido.Problema);
}

function tipoPessoaChange(e) {
    if (e.TipoPessoa.val() === EnumTipoPessoaGrupo.Pessoa) {
        e.Remetente.visible(true);
        e.GrupoPessoa.visible(false);

        LimparCampoEntity(e.GrupoPessoa);
    }
    else if (e.TipoPessoa.val() === EnumTipoPessoaGrupo.GrupoPessoa) {
        e.Remetente.visible(false);
        e.GrupoPessoa.visible(true);

        LimparCampoEntity(e.Remetente);
    }
}

/*
 * Declaração das Funções
 */

function alterarSituacaoPlanejamentoPedido(pedidoSelecionado, novaSituacao) {
    var dadosPedidoAtualizar = {
        Codigo: pedidoSelecionado.Codigo,
        SituacaoPlanejamentoPedido: novaSituacao
    };

    executarReST("PlanejamentoPedido/AlterarSituacaoPlanejamentoPedido", dadosPedidoAtualizar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                _gridPlanejamentoPedido.CarregarGrid();
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function controlarVisibilidadeSituacaoEntregueAoMotorista(pedidoSelecionado) {
    return pedidoSelecionado.SituacaoPlanejamentoPedido != EnumSituacaoPlanejamentoPedido.EntregueAoMotorista;
}

function controlarVisibilidadeSituacaoMotoristaNoLocalCarregamento(pedidoSelecionado) {
    return pedidoSelecionado.SituacaoPlanejamentoPedido != EnumSituacaoPlanejamentoPedido.MotoristaNoLocalCarregamento;
}

function controlarVisibilidadeSituacaoPendente(pedidoSelecionado) {
    return pedidoSelecionado.SituacaoPlanejamentoPedido != EnumSituacaoPlanejamentoPedido.Pendente;
}

function controlarVisibilidadeSituacaoProblema(pedidoSelecionado) {
    return pedidoSelecionado.SituacaoPlanejamentoPedido != EnumSituacaoPlanejamentoPedido.Problema;
}

function controlarVisibilidadeDefinirVeiculo(pedidoSelecionado) {
    return !pedidoSelecionado.PossuiVeiculo;
}
function controlarVisibilidadeDefinirPlacaCarregamento(pedidoSelecionado) {
    return pedidoSelecionado.PossuiVeiculo && pedidoSelecionado.NecessitaInformarPlacaCarregamento;
}
function controlarVisibilidadeSubstituirVeiculo(pedidoSelecionado) {
    return pedidoSelecionado.PossuiVeiculo;
}

function controlarVisibilidadeRemoverVeiculo(pedidoSelecionado) {
    return pedidoSelecionado.PossuiVeiculo;
}

function controlarVisibilidadeDefinirMotorista(pedidoSelecionado) {
    return !pedidoSelecionado.PossuiMotorista;
}

function controlarVisibilidadeSubstituirMotorista(pedidoSelecionado) {
    return pedidoSelecionado.PossuiMotorista;
}

function controlarVisibilidadeRemoverMotorista(pedidoSelecionado) {
    return pedidoSelecionado.PossuiMotorista;
}

function editarColunaRetorno(pedidoSelecionado, linhaSelecionada, cabecalho, callbackTabPress) {
    var dadosPedidoAtualizar = {
        Codigo: pedidoSelecionado.Codigo,
        DataCarregamento: pedidoSelecionado.DataCarregamentoPedido,
        Ordem: pedidoSelecionado.Ordem,
        ObservacaoInterna: pedidoSelecionado.ObservacaoInterna,
        NumeroFrota: pedidoSelecionado.NumeroFrota
    };

    executarReST("PlanejamentoPedido/AlterarDadosPedido", dadosPedidoAtualizar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _gridPlanejamentoPedido.AtualizarDataRow(linhaSelecionada, retorno.Data, callbackTabPress);

                forcarRecarregarGridPlanejamentoPedidoDisponibilidade();
                forcarRecarregarGridPlanejamentoPedidoDisponibilidadeMotorista();

                if (retorno.Msg != "")
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
            }
            else {
                _gridPlanejamentoPedido.DesfazerAlteracaoDataRow(linhaSelecionada);
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
            }
        }
        else {
            _gridPlanejamentoPedido.DesfazerAlteracaoDataRow(linhaSelecionada);
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
        }
    });
}

function exibirModalDuplicarPedidos() {
    Global.abrirModal('divModalDuplicarPedido');
    $("#divModalDuplicarPedido").one('hidden.bs.modal', function () {

    });
}

function exibirModalEmail() {
    Global.abrirModal('divModalEmail');
    $("#divModalEmail").one('hidden.bs.modal', function () {

    });
}

function limparSelecionados() {
    _gridPlanejamentoPedido.AtualizarRegistrosSelecionados([]);
    _gridPlanejamentoPedido.AtualizarRegistrosNaoSelecionados([]);
    _pesquisaPlanejamentoPedido.SelecionarTodos.val(false);

    exibirMultiplasOpcoes();
}

function exibirMultiplasOpcoes() {
    var existemRegistrosSelecionados = _gridPlanejamentoPedido.ObterMultiplosSelecionados().length > 0;

    if (existemRegistrosSelecionados) {

        _pesquisaPlanejamentoPedido.DuplicarPedidos.visible(true);
        _pesquisaPlanejamentoPedido.EnviarPedidosAurora.visible(true);
    }
    else {
        _pesquisaPlanejamentoPedido.DuplicarPedidos.visible(false);
        _pesquisaPlanejamentoPedido.EnviarPedidosAurora.visible(false);
    }
}

function fecharDuplicarPedidoClick() {
    Global.fecharModal("divModalDuplicarPedido");
}

function duplicarPedidoIndividual() {
    if (ValidarCamposObrigatorios(_planejamentoPedidoDuplicarPedido)) {
        executarReST("PlanejamentoPedido/DuplicarPedido", {
            Codigo: _planejamentoPedidoDuplicarPedido.CodigoPedido.val(),
            DataPedido: _planejamentoPedidoDuplicarPedido.DataPedido.val(),
            QuantidadeDuplicar: _planejamentoPedidoDuplicarPedido.QuantidadeDuplicar.val()
        }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    limparSelecionados();
                    _gridPlanejamentoPedido.CarregarGrid();
                    LimparCampos(_planejamentoPedidoDuplicarPedido);
                    Global.fecharModal("divModalDuplicarPedido");
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Pedido duplicado com sucesso.");
                    fecharDuplicarPedidoClick();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        });
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function confirmarEmailClick() {

    if (!ValidarCamposObrigatorios(_planejamentoPedidoEmail))
        return;

    var dados = RetornarObjetoPesquisa(_pesquisaPlanejamentoPedido);

    dados.ItensSelecionados = JSON.stringify(_gridPlanejamentoPedido.ObterMultiplosSelecionados());
    dados.Email = _planejamentoPedidoEmail.Email.val();

    executarReST("PlanejamentoPedido/EnviarPedidosAurora", dados, function (r) {
        if (r.Success) {
            if (r.Data) {
                limparSelecionados();
                _gridPlanejamentoPedido.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Pedidos enviados com sucesso.");
                fecharEmailClick();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
    });
}

function fecharEmailClick() {
    Global.fecharModal("divModalEmail");
}

function confirmarDuplicarPedidoClick() {
    if (_pedidoGrupo)
        duplicarPedidosGrupo();
    else
        duplicarPedidoIndividual();
}

function duplicarPedidosGrupo() {
    var dados = RetornarObjetoPesquisa(_pesquisaPlanejamentoPedido);

    dados.ItensSelecionados = JSON.stringify(_gridPlanejamentoPedido.ObterMultiplosSelecionados());
    dados.DataPedido = _planejamentoPedidoDuplicarPedido.DataPedido.val();
    dados.QuantidadeDuplicar = _planejamentoPedidoDuplicarPedido.QuantidadeDuplicar.val();

    executarReST("PlanejamentoPedido/DuplicarPedidosSelecionados", dados, function (r) {
        if (r.Success) {
            if (r.Data) {
                limparSelecionados();
                _gridPlanejamentoPedido.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Pedidos duplicados com sucesso.");
                fecharDuplicarPedidoClick();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
    });
}

function duplicarPedidosClick() {
    _pedidoGrupo = true;

    LimparCampos(_planejamentoPedidoDuplicarPedido);

    exibirModalDuplicarPedidos();
}

function enviarPedidosAuroraClick() {
    var dados = RetornarObjetoPesquisa(_pesquisaPlanejamentoPedido);

    dados.ItensSelecionados = JSON.stringify(_gridPlanejamentoPedido.ObterMultiplosSelecionados());

    executarReST("PlanejamentoPedido/ObterEmailPedidosSelecionados", dados, function (r) {
        if (r.Success) {
            if (r.Data) {
                _planejamentoPedidoEmail.Email.val(r.Data);
            }
        }

        exibirModalEmail();
    });
}
