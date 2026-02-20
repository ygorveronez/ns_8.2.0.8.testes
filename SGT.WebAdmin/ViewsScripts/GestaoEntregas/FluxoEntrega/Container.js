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
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _pesquisaFluxoEntrega;
var _containerFluxoEntrega;
var _fluxoAtual;
var _configuracaoGestaoEntrega = {};
var _itensPorPagina = 10;
var isMobile = false;
var _executarPesquisa = false;

var _etapasFluxoEntrega = [
    { text: "Todas", value: EnumEtapaFluxoGestaoPatio.Todas },
];

var _situacoesFluxo = [
    { text: "Todas", value: "" },
    { text: "Aguardando", value: EnumSituacaoEtapaFluxoGestaoEntrega.Aguardando },
    { text: "Finalizado", value: EnumSituacaoEtapaFluxoGestaoEntrega.Finalizado },
];

var _ordenacaoFluxo = [
    { text: "Crescente", value: "asc" },
    { text: "Decrescente", value: "desc" },
];

var PesquisaFluxoEntrega = function () {
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, def: Global.DataAtual(), val: ko.observable(Global.DataAtual()) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.EtapaFluxoGestaoEntrega = PropertyEntity({ val: ko.observable(EnumEtapaFluxoGestaoPatio.Todas), options: ko.observableArray(_etapasFluxoEntrega), text: "Etapa: ", def: EnumEtapaFluxoGestaoPatio.Todas });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: ko.observable("Número Carga:"), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Pedido = PropertyEntity({ text: ko.observable("Pedido:"), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Placa = PropertyEntity({ text: "Placa:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Ordenacao = PropertyEntity({ val: ko.observable("desc"), options: _ordenacaoFluxo, text: "Ordenação: ", def: "" });
    this.Situacao = PropertyEntity({ val: ko.observable("desc"), options: _situacoesFluxo, text: "Situação: ", def: "" });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            ObterFluxoEntregas(1, false);
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var ContainerFluxoEntrega = function () {
    this.Entregas = PropertyEntity({ val: ko.observable([]) });
}


//*******EVENTOS*******

function carregarHTMLComponenteFluxoEntrega(callback) {
   
    $.get('Content/Static/GestaoEntrega/ComponenteFluxoEntrega.html', function (html) {
        $('#FluxoEntregaContent').html(html);

        $.get('Content/Static/GestaoEntrega/Modal.html', function (html) {
            $('#FluxoEntregaModal').html(html);
            callback();
        })

    })
}

function loadFluxoEntrega() {
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {
            carregarHTMLComponenteFluxoEntrega(function () {

                RegistraComponente();
                CarregarConfiguracoesGestaoEntrega(LoadEtapas);
                isMobile = $(window).width() <= 980;

                _containerFluxoEntrega = new ContainerFluxoEntrega();
                KoBindings(_containerFluxoEntrega, "knoutContainerFluxoEntrega");

                _pesquisaFluxoEntrega = new PesquisaFluxoEntrega();
                KoBindings(_pesquisaFluxoEntrega, "knoutPesquisaFluxoEntrega");

                _pesquisaFluxoEntrega.Placa.get$().mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });

                new BuscarFilial(_pesquisaFluxoEntrega.Filial);
                new BuscarClientes(_pesquisaFluxoEntrega.Destinatario);
                new BuscarTransportadores(_pesquisaFluxoEntrega.Transportador);

                ObterSituacoesFluxo();
                BuscarFilialPadrao();

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
                    _pesquisaFluxoEntrega.Filial.visible(false);
            })
        });
    });
}


//*******MÉTODOS*******
function LoadEtapas() {
    LoadGuaritaFluxoEntrega();
    LoadPosicao();
    LoadInicioViagem();
    LoadFimViagem();
    LoadPedido();
}

function RegistraComponente() {
    if (ko.components.isRegistered('fluxo-entrega'))
        return;

    ko.components.register('fluxo-entrega', {
        viewModel: Entrega,
        template: {
            element: 'fluxo-entrega-template'
        }
    });
}

function AtualizarFluxoEntrega() {
    if (_fluxoAtual != null) {
        var data = { Codigo: _fluxoAtual.Codigo.val() }
        executarReST("FluxoEntrega/BuscarPorCodigo", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    PreencherFluxoEntrega(_fluxoAtual, arg.Data);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function BuscarFilialPadrao() {
    executarReST("DadosPadrao/ObterFilial", {}, function (r) {
        //_pesquisaFluxoEntrega.Filial.val("3");
        //_pesquisaFluxoEntrega.Filial.codEntity(3036);
        //_pesquisaFluxoEntrega.DataInicial.val("");
        //_pesquisaFluxoEntrega.DataFinal.val("");
        //_pesquisaFluxoEntrega.CodigoCargaEmbarcador.val("");
        //ObterFluxoEntregas(0, false);

        if (r.Success && r.Data) {
            RetornoConsultaFilial(r.Data);
            ObterFluxoEntregas(0, false);
        }
    });
}

function RetornoConsultaFilial(dados) {
    _pesquisaFluxoEntrega.Filial.val(dados.Descricao);
    _pesquisaFluxoEntrega.Filial.codEntity(dados.Codigo);
}

function ObterFluxoEntregas(page, eventoPorPaginacao) {
    var data = RetornarObjetoPesquisa(_pesquisaFluxoEntrega);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS && data.Filial <= 0)
        return exibirMensagem(tipoMensagem.atencao, "Atenção", "É obrigatório informar a filial");

    data.inicio = _itensPorPagina * (page - 1);
    data.limite = _itensPorPagina;

    executarReST("FluxoEntrega/ObterFluxoEntrega", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _containerFluxoEntrega.Entregas.val(arg.Data);

                if (!eventoPorPaginacao) {
                    ComponentePaginacao(arg.QuantidadeRegistros);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function ComponentePaginacao(totalRegistros) {
    if (totalRegistros > 0) {
        var $ul = $('<ul class="pagination"></ul>');
        var paginas = Math.ceil(totalRegistros / _itensPorPagina);

        $("#paginacao-fluxo-entrega").empty().append($ul);

        _executarPesquisa = false;

        $ul.twbsPagination({
            first: 'Primeiro',
            prev: 'Anterior',
            next: 'Próximo',
            last: 'Último',
            totalPages: paginas,
            visiblePages: 5,
            onPageClick: null,
            onPageClick: function (event, page) {
                if (_executarPesquisa)
                    ObterFluxoEntregas(page, true);
            }
        });

        _executarPesquisa = true;

    } else {
        $("#paginacao-fluxo-entrega").html('<span>Nenhum Registro Encontrado</span>');
    }
}

function CarregarConfiguracoesGestaoEntrega(cb) {
    executarReST("FluxoEntrega/ConfiguracoesGestaoEntrega", {}, function (arg) {
        if (arg.Success && arg.Data !== false) {
            _configuracaoGestaoEntrega = arg.Data;
            cb();
        }
    });
}

function ObterSituacoesFluxo() {
    executarReST("FluxoEntrega/ObterEtapasFluxo", {}, function (arg) {
        if (arg.Success && arg.Data !== false) {
            var formatacaoOption = arg.Data.map(function (sit) {
                return {
                    text: sit.Descricao,
                    value: sit.Enumerador
                }
            });

            var situacoes = _etapasFluxoEntrega.concat(formatacaoOption);
            _pesquisaFluxoEntrega.EtapaFluxoGestaoEntrega.options(situacoes);
        }
    });
}

function ObterDetalhesCargaFluxoClick(e) {
    if (_configuracaoGestaoEntrega.OcultarFluxoCarga || e.Carga.val() == 0) return;
    ObterDetalhesCargaFluxo(e.Carga.val());
}

function ObterDetalhesCargaFluxo(codigo) {
    var data = { Carga: codigo };
    executarReST("Carga/BuscarCargaPorCodigo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
         $("#fdsCarga").html('<button type="button" class="btn-close" data-bs-dismiss="modal" aria-hidden="true" style="position: absolute; z-index: 9999; right: 18px; top: 6px;"><i class="fal fa-times"></i></button>');
                var knoutCarga = GerarTagHTMLDaCarga("fdsCarga", arg.Data, false);
                $("#fdsCarga .container-carga").addClass("mb-0");
                _cargaAtual = knoutCarga;                
                Global.abrirModal('divModalDetalhesCarga');
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function ExibeModalEtapa(id, onShow) {
    var $modal = $(id); 
    var $window = $(window); 
    var keyUpHandle = function (e) {
        if (e.keyCode == 27)            
            Global.fecharModal(id);
    }
    var unbindFecharModal = function () {
        $window.off('keyup', keyUpHandle);
    }

    $modal
        .modal({ keyboard: false })
        .on("shown.bs.modal", onShow || function () { })
        .on("hidden.bs.modal", unbindFecharModal)
        ;

    $window.one('keyup', keyUpHandle);
}