/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridPedidosSelecao;
var _selecaoPedidos;

var _tipoTomadorFechamento = [
    { value: EnumTipoTomador.Remetente, text: "Remetente" },
    { value: EnumTipoTomador.Destinatario, text: "Destinatário" }
    //{ value: EnumTipoTomador.Expedidor, text: "Expedidor" },
    //{ value: EnumTipoTomador.Recebedor, text: "Recebedor" },
    //{ value: EnumTipoTomador.Outros, text: "Outros" }
];

var SelecaoPedidos = function () {

    var dataAtual = moment().add(-2, 'days').format("DD/MM/YYYY");
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: true, text: "*Transportador:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: false, text: "Filial:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: false, text: "Tomador:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "Data Inicial :", getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoTomador = PropertyEntity({ val: ko.observable(EnumTipoTomador.Destinatario), def: EnumTipoTomador.Destinatario, options: _tipoTomadorFechamento, text: "*Tipo do Tomador:", required: true, enable: ko.observable(true) });
    this.SomenteFretePendente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Visualizar Somente Pedidos com Frete Pendente", def: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.DataFim = PropertyEntity({ text: "Data Final :  ", val: ko.observable(""), def: "", getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Filtro = PropertyEntity({ visible: ko.observable(true) });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), text: "Selecionar Todos" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPedidosSelecao.CarregarGrid(function () {
                // TODO:
                // Triar o setTimeout e chamar o selecionar todos após renderizar tabela
                // Limpar o selecionar todos quando algum campo do filtro for modificado

                // Só marca todos como selecionado quando Filial, Transportador e Tomador forem selecionados
                var busca = RetornarObjetoPesquisa(_selecaoPedidos);
                if ((busca.Filial > 0 && busca.Tomador > 0 && busca.Transportador > 0) && _selecaoPedidos.SelecionarTodos.val() == false) {
                    setTimeout(_selecaoPedidos.SelecionarTodos.eventClick, 100);
                }
                else if (_selecaoPedidos.SelecionarTodos.val() == true) {
                    setTimeout(_selecaoPedidos.SelecionarTodos.eventClick, 100);
                } else {
                    _selecaoPedidos.SelecionarTodos.val(false);
                    _selecaoPedidos.SelecionarTodos.visible(false);
                }
            });
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Cancelar = PropertyEntity({ eventClick: cancelarFechamentoColetaProdutorClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.Criar = PropertyEntity({ eventClick: criarClick, type: types.event, text: "Criar", visible: ko.observable(true) });
    this.GerarCarga = PropertyEntity({ eventClick: gerarCargaClick, type: types.event, text: "Gerar Carga", visible: ko.observable(false) });
    this.ApenasPendentes = PropertyEntity({ type: types.event, getType: typesKnockout.bool, text: "Apenas Fretes Pendentes", val: ko.observable(true) });
    this.ReprocessarFrete = PropertyEntity({ eventClick: reprocessarFreteClick, type: types.event, text: "Reprocessar Frete", visible: ko.observable(true) });
}

//*******EVENTOS*******
function loadSelecaoPedidos() {
    _selecaoPedidos = new SelecaoPedidos();
    KoBindings(_selecaoPedidos, "knockoutSelecaoPedidos");

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _selecaoPedidos.Filial.visible(false);
    }

    // Inicia as buscas
    new BuscarFilial(_selecaoPedidos.Filial);
    new BuscarTransportadores(_selecaoPedidos.Transportador);
    new BuscarClientes(_selecaoPedidos.Tomador);

    // Inicia grid de dados
    GridSelecaoPedidos();
}


function reprocessarFreteClick(e, sender) {
    var dados = {
        ApenasPendentes: _selecaoPedidos.ApenasPendentes.val()
    };
    executarReST("FechamentoColetaProdutor/SolicitarCalculoFretePreCargasPendentes", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Recalculo solicitado com  sucesso");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function criarClick(e, sender) {
    if (ValidaPedidosSelecionados()) {
        if (ValidarCamposObrigatorios(e)) {
            exibirConfirmacao("Criar FechamentoColetaProdutor", "Você tem certeza que deseja criar o fechamento para os pedidos selecionados?", function () {
                var dados = RetornarObjetoPesquisa(_selecaoPedidos);

                dados.SelecionarTodos = _selecaoPedidos.SelecionarTodos.val();
                dados.PedidosSelecionadas = JSON.stringify(_gridPedidosSelecao.ObterMultiplosSelecionados());
                dados.PedidosNaoSelecionadas = JSON.stringify(_gridPedidosSelecao.ObterMultiplosNaoSelecionados());

                executarReST("FechamentoColetaProdutor/Adicionar", dados, function (arg) {
                    if (arg.Success) {
                        if (arg.Data) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Fechamento criado com sucesso");
                            _fechamentoColetaProdutor.Situacao.val(EnumSituacaoFechamentoColetaProdutor.AgGeracaoCarga);
                            _gridFechamentoColetaProdutor.CarregarGrid();
                            BuscarFechamentoColetaProdutorPorCodigo(arg.Data.Codigo);
                        } else {
                            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    }
                });
            });
        } else {
            exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Por favor, informe os campos obrigatórios");
        }
    }
}

function retornoMotivoRejeicaoCalculoFreteClick(e, sender) {
    $('#PMensagemMotivoPendenciaCalculoFrete').html(e.MotivoPendencia);
    Global.abrirModal('divModalMotivoPendenciaCalculoFrete');
}

function VisibilidadeMotivoRejeicaoCalculo(e) {
    if (e.PendenciaCalculoFrete)
        return true;
    else
        return false;
}

function VisibilidadeDetalhesFrete(e) {
    if (!e.PendenciaCalculoFrete && !e.CalculandoFrete)
        return true;
    else
        return false;
}

function visualizarDetalheFreteClick(e) {
    var dados = { Codigo: e.Codigo };
    executarReST("FechamentoColetaProdutor/ObterDetalheFrete", dados, function (arg) {
        if (arg.Success) {
            $("#contentDetalheComposicaoPedidoFechamento").html("");
            var composicoesFrete = arg.Data;
            for (var i = 0; i < composicoesFrete.length; i++) {
                var idDiv = guid();
                var knoutDetalheComposicao = new DetalheComposicaoFrete();
                var data = { Data: composicoesFrete[i] };
                PreencherObjetoKnout(knoutDetalheComposicao, data);
                $("#contentDetalheComposicaoPedidoFechamento").append(_HTMLComposicaoFrete.replace(/#knoutComposicaoFrete/g, idDiv));
                KoBindings(knoutDetalheComposicao, idDiv);
            }
            Global.abrirModal('divModalDetalheValorFretePedidoFechameto');

        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//*******MÉTODOS*******
function GridSelecaoPedidos() {
    //-- Cabecalho

    var MotivoRejeicao = { descricao: "Motivo Rejeição", id: guid(), metodo: retornoMotivoRejeicaoCalculoFreteClick, icone: "", visibilidade: VisibilidadeMotivoRejeicaoCalculo };
    var DetalhesFrete = { descricao: "Detalhe Frete", id: guid(), metodo: visualizarDetalheFreteClick, icone: "", visibilidade: VisibilidadeDetalhesFrete };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 7,
        opcoes: [MotivoRejeicao, DetalhesFrete]
    };

    //-- Multi escolha
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _selecaoPedidos.SelecionarTodos,
        callbackNaoSelecionado: function () {
            SelecaoModificado(false);
        },
        callbackSelecionado: function () {
            SelecaoModificado(true);
        },
        callbackSelecionarTodos: null,
        somenteLeitura: false
    }

    if (_fechamentoColetaProdutor.Situacao.val() != EnumSituacaoFechamentoColetaProdutor.Todos)
        multiplaescolha = null;

    var configExportacao = {
        url: "FechamentoColetaProdutor/ExportarPesquisaPedidos",
        titulo: "Pedidos",
        id: "btnExportarDocumento"
    };


    _gridPedidosSelecao = new GridView(_selecaoPedidos.Pesquisar.idGrid, "FechamentoColetaProdutor/PesquisaPedidos", _selecaoPedidos, menuOpcoes, null, 25, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridPedidosSelecao.SetPermitirRedimencionarColunas(true);
    _gridPedidosSelecao.CarregarGrid();
}

function SelecaoModificado(selecao) {
    // Quando o primeiro item é selecionado, seta os filtros de selecao
    var itens = _gridPedidosSelecao.ObterMultiplosSelecionados();
    if (itens.length == 1) {
        // Busca
        var busca = RetornarObjetoPesquisa(_selecaoPedidos);

        // Seta os dados do pedidos selecionados nos campos em branco

        var primeiroClick = false;

        //if (busca.Filial == 0) {
        //    primeiroClick = true;
        //    _selecaoPedidos.Filial.codEntity(itens[0].CodigoFilial).val(itens[0].Filial);
        //}
        if (busca.Transportador == 0) {
            primeiroClick = true;
            _selecaoPedidos.Transportador.codEntity(itens[0].CodigoTransportador).val(itens[0].Empresa);
        }

        //if (busca.Tomador == 0) {
        //    primeiroClick = true;
        //    _selecaoPedidos.Tomador.codEntity(itens[0].CodigoTomador).val(itens[0].Tomador);
        //}

        if (primeiroClick) {
            if (_selecaoPedidos.Transportador.codEntity() > 0) {
                _selecaoPedidos.SelecionarTodos.val(true);
                _selecaoPedidos.SelecionarTodos.visible(true);
            } else {
                _selecaoPedidos.SelecionarTodos.val(false);
                _selecaoPedidos.SelecionarTodos.visible(false);
            }
        }


        if (selecao)
            _gridPedidosSelecao.CarregarGrid();
    }
}

function ValidaPedidosSelecionados() {
    var valido = true;

    // Valida Quantidade
    var itens = _gridPedidosSelecao.ObterMultiplosSelecionados();
    // TODO: Se o btn SELECIONAR TODOS estiver clicado, 
    if (itens == null || (itens.length == 0 && !_selecaoPedidos.SelecionarTodos.val())) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Pedidos Selecionados", "Nenhum pedido selecionado.");
    }

    return valido;
}

function EditarSelecaoPedidos(data) {
    _selecaoPedidos.Transportador.enable(false);
    _selecaoPedidos.Filial.enable(false);
    _selecaoPedidos.Tomador.enable(false);
    _selecaoPedidos.DataInicio.enable(false);
    _selecaoPedidos.DataFim.enable(false);
    _selecaoPedidos.SomenteFretePendente.visible(false);
    _selecaoPedidos.TipoTomador.enable(false);

    _selecaoPedidos.Criar.visible(false);
    _selecaoPedidos.Cancelar.visible(false);

    _selecaoPedidos.Codigo.val(data.Codigo);
    GridSelecaoPedidos();
    _selecaoPedidos.DataInicio.val(data.DataInicial);
    _selecaoPedidos.TipoTomador.val(data.TipoTomador);
    _selecaoPedidos.DataFim.val(data.DataFinal);
    _selecaoPedidos.Carga.val(data.Carga);
    _selecaoPedidos.Transportador.val(data.Transportador.Descricao);
    _selecaoPedidos.Filial.val(data.Filial.Descricao);
    _selecaoPedidos.Tomador.val(data.Tomador.Descricao);
    _selecaoPedidos.Transportador.codEntity(data.Transportador.Codigo);
    _selecaoPedidos.Filial.codEntity(data.Filial.Codigo);
    _selecaoPedidos.Tomador.codEntity(data.Tomador.Codigo);
}

function LimparCamposSelecaoPedidos() {
    // Mostra
    _selecaoPedidos.Transportador.enable(true);
    _selecaoPedidos.Filial.enable(true);
    _selecaoPedidos.Tomador.enable(true);
    _selecaoPedidos.DataInicio.enable(true);
    _selecaoPedidos.DataFim.enable(true);
    _selecaoPedidos.SomenteFretePendente.visible(true);
    _selecaoPedidos.TipoTomador.enable(true);

    _selecaoPedidos.SelecionarTodos.val(false);
    _selecaoPedidos.SelecionarTodos.visible(false);
    _selecaoPedidos.Criar.visible(true);
    LimparCampos(_selecaoPedidos);
}
