/// <reference path="AutorizarRegras.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumTipoMotivoPedido.js" />
/// <reference path="../../Enumeradores/EnumResponsavelOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumTipoCarroceria.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pedido,
    _valores,
    _rejeicao,
    _aprovacao,
    _autorizacao,
    _gridPedido,
    _pesquisaPedidos,
    _modalDetalhesAutorizacaoPedido,
    _modalDetalhesAutorizacaoPedidoRejeitar,
    _modalDetalhesAutorizacaoPedidoDelegar;

var _responsavelPedido = [
    { text: "Remetente", value: EnumResponsavelOcorrencia.Remetente },
    { text: "Destinatário", value: EnumResponsavelOcorrencia.Destinatario }
];

var _responsavelPedido = [
    { text: "Remetente", value: EnumResponsavelOcorrencia.Remetente },
    { text: "Destinatário", value: EnumResponsavelOcorrencia.Destinatario }
];

var RegraPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var Autorizacao = function () {
    this.Regras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Tomador = PropertyEntity({ text: "Responsável: ", val: ko.observable(EnumResponsavelOcorrencia.Destinatario), options: _responsavelPedido, def: EnumResponsavelOcorrencia.Destinatario, visible: ko.observable(false), permiteSelecionarTomador: ko.observable(false) });
    this.Justificativa = PropertyEntity({ text: "*Justificativa:", type: types.entity, codEntity: ko.observable(0), required: true, enable: true, idBtnSearch: guid(), visible: ko.observable(false) });
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarPedidoClick, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: aprovarMultiplasRegrasClick, text: "Aprovar Regras" });
};

var RejeitarSelecionados = function () {
    this.Observacao = PropertyEntity({ text: "*Observação: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.MotivoRejeicao = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: "Motivo da Rejeição:", idBtnSearch: guid(),
        visible: ko.observable(UtilizarCampoDeMotivoDePedido),
        enable: ko.observable(UtilizarCampoDeMotivoDePedido)
    });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarPedidosSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var AprovarSelecionados = function () {
    this.Observacao = PropertyEntity({ text: "*Observação: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.MotivoAprovacao = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: "Motivo da Aprovação:", idBtnSearch: guid(),
        visible: ko.observable(UtilizarCampoDeMotivoDePedido),
        enable: ko.observable(UtilizarCampoDeMotivoDePedido)
    });
    this.Aprovar = PropertyEntity({ eventClick: aprovarMultiplasPedidosClick, type: types.event, text: "Aprovar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAprovacaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var Pedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ValorFrete = PropertyEntity({ text: "Valor do Frete: ", visible: ko.observable(true), val: ko.observable("") });
    this.NumeroPedido = PropertyEntity({ text: "Número do Pedido: ", visible: ko.observable(true), val: ko.observable("") });
    this.DataPedido = PropertyEntity({ text: "Data do Pedido: ", visible: ko.observable(true), val: ko.observable("") });

    this.Situacao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true), val: ko.observable("") });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: "Nº Pedido Embarcador: ", visible: ko.observable(true), val: ko.observable("") });
    this.TipoCarga = PropertyEntity({ text: "Tipo da Carga: ", visible: ko.observable(true), val: ko.observable("") });
    this.MotivoPedido = PropertyEntity({ text: "Motivo do Pedido: ", visible: ko.observable(true), val: ko.observable("") });
    this.Veiculo = PropertyEntity({ text: "Veículo: ", visible: ko.observable(true), val: ko.observable("") });

    this.TipoOperacao = PropertyEntity({ text: "Tipo da Operação: ", visible: ko.observable(true), val: ko.observable("") });
    this.Remetente = PropertyEntity({ text: "Remetente: ", visible: ko.observable(true), val: ko.observable("") });
    this.Destinatario = PropertyEntity({ text: "Destinatário: ", visible: ko.observable(true), val: ko.observable("") });

    this.Motorista = PropertyEntity({ text: "Motorista(s): ", visible: ko.observable(true), val: ko.observable("") });
    this.Solicitante = PropertyEntity({ text: "Solicitante: ", visible: ko.observable(true), val: ko.observable("") });

    this.Observacao = PropertyEntity({ text: "Observação: ", visible: ko.observable(true), val: ko.observable("") });
    this.MotivoCancelamento = PropertyEntity({ text: "Motivo do Cancelamento: ", visible: ko.observable(true), val: ko.observable("") });
    this.CidadeUfRemetente = PropertyEntity({ text: "Cidade/UF do Remetente: ", visible: ko.observable(true), val: ko.observable("") });
    this.CidadeUfDestinatario = PropertyEntity({ text: "Cidade/UF do Destinatário: ", visible: ko.observable(true), val: ko.observable("") });
    this.ModeloVeicularCarga = PropertyEntity({ text: "Modelo Veicular da Carga: ", visible: ko.observable(true), val: ko.observable("") });
    this.ModeloVeiculo = PropertyEntity({ text: "Modelo do Veículo: ", visible: ko.observable(true), val: ko.observable("") });
    this.TipoCarroceria = PropertyEntity({ text: "Tipo da Carroceria: ", visible: ko.observable(true), val: ko.observable("") });
    this.ModeloCarroceria = PropertyEntity({ text: "Modelo da Carroceria: ", visible: ko.observable(true), val: ko.observable("") });

    // Campos para filtro
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.EtapaAutorizacao = PropertyEntity({ val: ko.observable(0), def: 0 });
};

var PesquisaPedidos = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.NumeroPedido = PropertyEntity({ text: "Número do Pedido:", getType: typesKnockout.int, val: ko.observable(""), def: "" });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoPedido.AutorizacaoPendente), options: EnumSituacaoPedido.obterOpcoesPesquisa(), def: EnumSituacaoPedido.AutorizacaoPendente, text: "Situação: " });
    this.Situacao.val.subscribe(function () {
        exibirMultiplasOpcoes();
    });

    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo da Carga:", issue: 53, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa:", issue: 58, idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", issue: 121, idBtnSearch: guid() });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular da Carga:", idBtnSearch: guid() });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo do Veículo:", idBtnSearch: guid() });
    this.ModeloCarroceria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo da Carroceria:", idBtnSearch: guid() });
    this.TipoCarroceria = PropertyEntity({ val: ko.observable(EnumTipoCarroceria.Todos), options: EnumTipoCarroceria.obterOpcoesPesquisa(), def: EnumTipoCarroceria.Todos, text: "Tipo da Carroceria: " });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            buscarPedidos();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });

    this.AprovarTodas = PropertyEntity({
        type: types.event, val: ko.observable(false),
        eventClick: UtilizarCampoDeMotivoDePedido === false ? aprovarMultiplasRegrasClick : AbrirModalAprovacao,
        text: "Aprovar Pedidos", visible: ko.observable(false)
    });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: rejeitarMultiplasRegrasClick, text: "Rejeitar Pedidos" });
    this.DelegarPedido = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: delegarMultiplasPedidosClick, text: "Delegar Pedidos", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadAutorizacaoPedido() {
    _pedido = new Pedido();
    KoBindings(_pedido, "knockoutPedido");

    _autorizacao = new Autorizacao();
    KoBindings(_autorizacao, "knockoutAutorizacao");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoPedido");

    _aprovacao = new AprovarSelecionados();
    KoBindings(_aprovacao, "knockoutAprovacaoPedido");
    
    _pesquisaPedidos = new PesquisaPedidos();
    KoBindings(_pesquisaPedidos, "knockoutPesquisaPedido", false, _pesquisaPedidos.Pesquisar.id);

    // Busca componentes pesquisa
    new BuscarTipoDeCargaDoPedido(_pesquisaPedidos.TipoCarga);

    (UtilizarCampoDeMotivoDePedido === false) || new BuscarMotivoPedido(_rejeicao.MotivoRejeicao, null, EnumTipoMotivoPedido.RejeicaoPedido);
    (UtilizarCampoDeMotivoDePedido === false) || new BuscarMotivoPedido(_aprovacao.MotivoAprovacao, null, EnumTipoMotivoPedido.AprovacaoPedido);
    
    new BuscarTiposOperacao(_pesquisaPedidos.TipoOperacao);
    new BuscarGruposPessoas(_pesquisaPedidos.GrupoPessoa);
    new BuscarFuncionario(_pesquisaPedidos.Usuario);
    new BuscarModelosVeicularesCarga(_pesquisaPedidos.ModeloVeicularCarga);
    new BuscarModelosCarroceria(_pesquisaPedidos.ModeloCarroceria);
    new BuscarModelosVeiculo(_pesquisaPedidos.ModeloVeiculo);

    // Load modulos    
    loadRegras();

    // Filtrar Alcadas Do Usuario
    FiltrarAlcadasDoUsuario(buscarPedidos);

    _modalDetalhesAutorizacaoPedido = new bootstrap.Modal(document.getElementById("divModalPedido"), { backdrop: true, keyboard: true });
    _modalDetalhesAutorizacaoPedidoRejeitar = new bootstrap.Modal(document.getElementById("divModalRejeitarPedido"), { backdrop: true, keyboard: true });
    _modalDetalhesAutorizacaoPedidoAprovar = new bootstrap.Modal(document.getElementById("divModalAprovarPedido"), { backdrop: true, keyboard: true });
    _modalDetalhesAutorizacaoPedidoDelegar = new bootstrap.Modal(document.getElementById("divModalDelegarPedido"), { backdrop: true, keyboard: true });
}

//*******MÉTODOS*******

function buscarPedidos() {
    //-- Cabecalho
    let detalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: detalharPedido, tamanho: "7", icone: "" };
    let menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(detalhes);

    //-- Reseta
    _pesquisaPedidos.SelecionarTodos.val(false);
    _pesquisaPedidos.AprovarTodas.visible(false);
    _pesquisaPedidos.RejeitarTodas.visible(false);
    _pesquisaPedidos.DelegarPedido.visible(false);

    //-- Multi escolha
    let multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaPedidos.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    let ordenacaoPadrao = { column: 2, dir: orderDir.asc };

    let configExportacao = {
        url: "AutorizacaoPedido/ExportarPesquisa",
        titulo: "Autorização Pedido"
    };

    _gridPedido = new GridView(_pesquisaPedidos.Pesquisar.idGrid, "AutorizacaoPedido/Pesquisa", _pesquisaPedidos, menuOpcoes, ordenacaoPadrao, 25, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridPedido.CarregarGrid();
}

function exibirMultiplasOpcoes(e) {
    let situacaoPesquisa = _pesquisaPedidos.Situacao.val();
    let possuiSelecionado = _gridPedido.ObterMultiplosSelecionados().length > 0;
    let selecionadoTodos = _pesquisaPedidos.SelecionarTodos.val();
    let situacaoPermiteSelecao = (situacaoPesquisa == EnumSituacaoPedido.AgAprovacao || situacaoPesquisa == EnumSituacaoPedido.AutorizacaoPendente);
    let situacaoPermiteSelecaoDelegar = (situacaoPesquisa == EnumSituacaoPedido.AgAprovacao || situacaoPesquisa == EnumSituacaoPedido.AutorizacaoPendente);

    // Esconde todas opções
    _pesquisaPedidos.AprovarTodas.visible(false);
    _pesquisaPedidos.RejeitarTodas.visible(false);
    _pesquisaPedidos.DelegarPedido.visible(false);

    if (possuiSelecionado || selecionadoTodos) {
        if (situacaoPermiteSelecao) {
            _pesquisaPedidos.AprovarTodas.visible(true);
            _pesquisaPedidos.RejeitarTodas.visible(true);
        }
        if (situacaoPermiteSelecaoDelegar) {
            _pesquisaPedidos.DelegarPedido.visible(false);
        }
    }
}

function rejeitarPedidosSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas os Pedidos selecionadas?", function () {
        let dados = RetornarObjetoPesquisa(_pesquisaPedidos);
        let rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Justificativa = rejeicao.Justificativa;
        dados.Motivo = rejeicao.Observacao;
        dados.CodigoMotivoPedido = rejeicao.MotivoRejeicao;
        dados.SelecionarTodos = _pesquisaPedidos.SelecionarTodos.val();
        dados.PedidosSelecionadas = JSON.stringify(_gridPedido.ObterMultiplosSelecionados());
        dados.PedidosNaoSelecionadas = JSON.stringify(_gridPedido.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoPedido/ReprovarMultiplasPedidos", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçadas de Pedidos foram reprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçada de Pedido foi reprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    buscarPedidos();
                    cancelarRejeicaoSelecionadosClick();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function rejeitarMultiplasRegrasClick() {
    LimparCampos(_rejeicao);
    _modalDetalhesAutorizacaoPedidoRejeitar.show();
}

function AbrirModalAprovacao() {
    LimparCampos(_aprovacao);
    _modalDetalhesAutorizacaoPedidoAprovar.show();
}

function delegarMultiplasPedidosClick() {
    _modalDetalhesAutorizacaoPedidoDelegar.show();
}

function aprovarMultiplasPedidosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas os Pedidos selecionadas?", function () {
        let dados = RetornarObjetoPesquisa(_pesquisaPedidos);
        let aprovacao = RetornarObjetoPesquisa(_aprovacao);

        dados.Justificativa = aprovacao.Justificativa;
        dados.Motivo = aprovacao.Observacao;
        dados.CodigoMotivoPedido = aprovacao.MotivoAprovacao;
        dados.SelecionarTodos = _pesquisaPedidos.SelecionarTodos.val();
        dados.PedidosSelecionadas = JSON.stringify(_gridPedido.ObterMultiplosSelecionados());
        dados.PedidosNaoSelecionadas = JSON.stringify(_gridPedido.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoPedido/AprovarMultiplasPedidos", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçadas de Pedidos foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçada de Pedido foi aprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    buscarPedidos();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function detalharPedido(ocorrenciaGrid) {
    limparCamposPedido();
    let pesquisa = RetornarObjetoPesquisa(_pesquisaPedidos);
    _pedido.Codigo.val(ocorrenciaGrid.Codigo);
    _pedido.Usuario.val(pesquisa.Usuario);
    _pedido.EtapaAutorizacao.val(pesquisa.EtapaAutorizacao);

    BuscarPorCodigo(_pedido, "AutorizacaoPedido/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {

                AtualizarGridRegras();

                if (arg.Data.PermiteSelecionarTomador) {
                    _autorizacao.Tomador.val(arg.Data.Tomador);
                    _autorizacao.Tomador.permiteSelecionarTomador(true);
                }
                else
                    _autorizacao.Tomador.permiteSelecionarTomador(false);

                _modalDetalhesAutorizacaoPedido.show();

                $("#divModalPedido").one('hidden.bs.modal', function () {
                    limparCamposPedido();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, null);
}

function cancelarRejeicaoSelecionadosClick() {
    LimparCampos(_rejeicao);
    _modalDetalhesAutorizacaoPedidoRejeitar.hide();
}

function cancelarAprovacaoSelecionadosClick() {
    LimparCampos(_aprovacao);
    _modalDetalhesAutorizacaoPedidoAprovar.hide();
}

function limparCamposPedido() {
    resetarTabs();
    limparRegras();
}

function resetarTabs() {
    Global.ResetarAbas();
    /*$("#myTab a:first").tab("show");*/
}

function FiltrarAlcadasDoUsuario(callback) {
    // Oculta campos conforme configurações
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario) {
        executarReST("Usuario/DadosUsuarioLogado", {}, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false && arg.Data != null) {
                    _pesquisaPedidos.Usuario.codEntity(arg.Data.Codigo);
                    _pesquisaPedidos.Usuario.val(arg.Data.Nome);
                    callback();
                }
            }
        })
    } else {
        callback();
    }
}