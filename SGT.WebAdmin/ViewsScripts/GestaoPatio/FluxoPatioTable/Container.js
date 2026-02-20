/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="../../Enumeradores/EnumTipoFluxoGestaoPatio.js" />
/// <reference path="MontarEtapa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaFluxoPatio;
var _containerFluxoPatio;
var _fluxoAtual;
var _configuracaoGestaoPatio = {};
var _itensPorPagina = 10;
window._viewTabelada = true;

var _situacoesFluxo = [
    { text: "Todas", value: "" },
    { text: "Aguardando", value: EnumSituacaoEtapaFluxoGestaoPatio.Aguardando },
    { text: "Aprovado", value: EnumSituacaoEtapaFluxoGestaoPatio.Aprovado },
    { text: "Rejeitado", value: EnumSituacaoEtapaFluxoGestaoPatio.Rejeitado }
];

var _ordenacaoFluxo = [
    { text: "Crescente", value: "asc" },
    { text: "Decrescente", value: "desc" },
];

var ContainerFluxoPatio = function () {
    this.Fluxos = PropertyEntity({ val: ko.observable([]) });
};

var PesquisaFluxoPatio = function () {
    var tipoPadrao = _CONFIGURACAO_TMS.GerarFluxoPatioDestino ? EnumTipoFluxoGestaoPatio.Todos : EnumTipoFluxoGestaoPatio.Origem;

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "* Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.dateTime, def: Global.DataAtual(), val: ko.observable(Global.DataAtual()) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.dateTime });
    this.EtapaFluxoGestaoPatio = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: "Etapa:", options: ko.observableArray([]) });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: ko.observable("Número Carga:"), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Pedido = PropertyEntity({ text: ko.observable("Pedido:"), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.PreCarga = PropertyEntity({ text: ko.observable("Pré Carga:"), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Placa = PropertyEntity({ text: "Placa:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Ordenacao = PropertyEntity({ val: ko.observable("desc"), options: _ordenacaoFluxo, text: "Ordenação: ", def: "" });
    this.Situacao = PropertyEntity({ val: ko.observable("desc"), options: _situacoesFluxo, text: "Situação: ", def: "" });
    this.Tipo = PropertyEntity({ val: ko.observable(tipoPadrao), options: EnumTipoFluxoGestaoPatio.obterOpcoesPesquisa(), text: "Tipo: ", def: tipoPadrao, visible: _CONFIGURACAO_TMS.GerarFluxoPatioDestino });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Tipo.val.subscribe(recarregarSituacoesFilial);

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            PesquisarFluxoCarga(1, false);
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

function LoadEtapas() {
    loadTravamentoChave(false, _configuracaoGestaoPatio, OnLoadTravamentoChave);
    loadCheckList(false, _configuracaoGestaoPatio, OnLoadDocaCheckList, EnumEtapaChecklist.Checklist);
    loadCheckList(false, _configuracaoGestaoPatio, OnLoadDocaCheckList, EnumEtapaChecklist.AvaliacaoDescarga);
    loadDocaCarregamento(false, _configuracaoGestaoPatio, OnLoadDocaCarregamento);
    LoadGuaritaFluxoPatio();
    LoadExpedicaoFluxoPatio();
    LoadPosicao();
    LoadPesagemFluxoPatio();
    LoadDocumentosPesagem();
    LoadDeslocamentoPatio();
    LoadChegadaSaidaLojaLoja();
    LoadFimViagem();
    loadInicioHigienizacaoFluxoPatio();
    loadFimHigienizacaoFluxoPatio();
    loadInicioCarregamentoFluxoPatio();
    loadFimCarregamentoFluxoPatio();
    loadSolicitacaoVeiculoFluxoPatio();
    loadObservacoesEtapas();
    loadInicioDescarregamentoFluxoPatio();
    loadFimDescarregamentoFluxoPatio();
    loadDocumentoFiscalFluxoPatio();
    loadDocumentosTransporteFluxoPatio();
    loadMontagemCargaPatio();
    loadSeparacaoMercadoria();
    loadFaturamentoFluxoPatio();
    ControleCamposPesquisa();
}

function ControleCamposPesquisa() {
    if (_configuracaoGestaoPatio.HabilitarPreCarga) {
        _pesquisaFluxoPatio.CodigoCargaEmbarcador.text(" Carga (Romaneio):");
        _pesquisaFluxoPatio.PreCarga.text("Pré Carga (Rota):");
        _pesquisaFluxoPatio.Pedido.text("Pedido  (Missão):");
    } else {
        _pesquisaFluxoPatio.PreCarga.visible(false);
    }
}

function loadFluxoPatioTable() {
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {
            carregarHTMLModaisFluxoPatio(function () {
                RegistraComponente();
                CarregarConfiguracoesGestaoPatio(LoadEtapas);

                _pesquisaFluxoPatio = new PesquisaFluxoPatio();
                KoBindings(_pesquisaFluxoPatio, "knoutPesquisaFluxoPatio", false, _pesquisaFluxoPatio.Pesquisar.id);

                _pesquisaFluxoPatio.Placa.get$().mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });

                new BuscarFilial(_pesquisaFluxoPatio.Filial);
                _pesquisaFluxoPatio.Filial.codEntity.subscribe(recarregarSituacoesFilial);

                _containerFluxoPatio = new ContainerFluxoPatio();
                KoBindings(_containerFluxoPatio, "knockoutContainerFluxoPatio");

                BuscarFilialPadrao();
            });
        });
    });
}

function carregarHTMLModaisFluxoPatio(callback) {
    $.get("Content/Static/GestaoPatio/FluxoPatioModais.html?dyn=" + guid(), function (data) {
        $("#modaisFluxoPatio").html(data);

        callback();
    });
}

function RegistraComponente() {
    if (!ko.components.isRegistered('fluxo-patio')) {
        ko.components.register('fluxo-patio', {
            viewModel: FluxoPatio,
            template: {
                element: 'fluxo-patio-template'
            }
        });
    }

    if (!ko.components.isRegistered('etapa-fluxo-patio')) {
        ko.components.register('etapa-fluxo-patio', {
            viewModel: Etapa,
            template: {
                element: 'etapa-fluxo-patio-template'
            }
        });
    }
}

function BuscarFilialPadrao() {
    executarReST("DadosPadrao/ObterFilial", {}, function (r) {
        //_pesquisaFluxoPatio.Filial.val("TESTE");
        //_pesquisaFluxoPatio.Filial.codEntity(2018);
        //_pesquisaFluxoPatio.DataInicial.val("");
        //_pesquisaFluxoPatio.DataFinal.val("");
        //_pesquisaFluxoPatio.CodigoCargaEmbarcador.val("491335");
        //PesquisarFluxoCarga(0, false);

        if (r.Success && r.Data) {
            RetornoConsultaFilial(r.Data);
            PesquisarFluxoCarga(0, false);
        }
    });
}

function RetornoConsultaFilial(dados) {
    _pesquisaFluxoPatio.Filial.val(dados.Descricao);
    _pesquisaFluxoPatio.Filial.codEntity(dados.Codigo);
}

function PesquisarFluxoCarga(page, paginou) {
    if (_pesquisaFluxoPatio.Filial.codEntity() > 0) {
        var data = RetornarObjetoPesquisa(_pesquisaFluxoPatio);

        data.inicio = _itensPorPagina * (page - 1);
        data.limite = _itensPorPagina;

        executarReST("FluxoPatio/ObterFluxoPatio", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    $('[data-toggle="tooltip"]').tooltip('destroy');

                    _containerFluxoPatio.Fluxos.val(arg.Data);

                    setTimeout(function () {
                        $('[data-toggle="tooltip"]').tooltip({
                            container: 'body'
                        });
                    }, 100);

                    if (!paginou) {
                        ComponentePaginacao(arg.QuantidadeRegistros);
                    }
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })

    } else {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É obrigatório informar a filial");
    }

}

function ComponentePaginacao(totalRegistros) {
    var clicouNoPaginar = false;
    if (totalRegistros > 0) {
        var $ul = $('<ul class="pagination"></ul>');
        var paginas = Math.ceil(totalRegistros / _itensPorPagina);

        $("#divPaginationGestaoPatio").empty().append($ul);

        $ul.twbsPagination({
            first: 'Primeiro',
            prev: 'Anterior',
            next: 'Próximo',
            last: 'Último',
            totalPages: paginas,
            visiblePages: 5,
            onPageClick: function (event, page) {
                if (clicouNoPaginar)
                    PesquisarFluxoCarga(page, true);
                clicouNoPaginar = true;
            }
        });

    } else {
        $("#divPaginationGestaoPatio").html('<span>Nenhum Registro Encontrado</span>');
    }
}

function setarSituacaoEtapa(fluxoCarregamento, etapa, objetoEtapa, etapaFluxo) {
    if (fluxoCarregamento.EtapaAtual > etapaFluxo) {
        if ((etapa.EtapaFluxoGestaoPatio == EnumEtapaFluxoGestaoPatio.Faturamento) && !fluxoCarregamento.FaturamentoFinalizado)
            SetarEtapaFluxoAprovadaComPendencia(objetoEtapa);
        else
            SetarEtapaFluxoAprovada(objetoEtapa);
    }
    else if (fluxoCarregamento.EtapaAtual == etapaFluxo) {
        if (!fluxoCarregamento.PossuiCarga && !EnumEtapaFluxoGestaoPatio.isEtapaHabilitadaPreCarga(etapa.EtapaFluxoGestaoPatio))
            SetarEtapaFluxoDesabilitada(objetoEtapa);
        else if (fluxoCarregamento.SituacaoEtapaFluxoGestaoPatio == EnumSituacaoEtapaFluxoGestaoPatio.Aguardando)
            SetarEtapaFluxoAguardando(objetoEtapa);
        else if (fluxoCarregamento.SituacaoEtapaFluxoGestaoPatio == EnumSituacaoEtapaFluxoGestaoPatio.Rejeitado)
            SetarEtapaFluxoProblema(objetoEtapa);
        else
            SetarEtapaFluxoAprovada(objetoEtapa);
    }
    else if (objetoEtapa.etapaLiberada)
        SetarEtapaFluxoAguardando(objetoEtapa);
    else
        SetarEtapaFluxoDesabilitada(objetoEtapa);
}

function CarregarConfiguracoesGestaoPatio(cb) {
    executarReST("FluxoPatio/ConfiguracoesGestaoPatio", {}, function (arg) {
        if (arg.Success && arg.Data !== false) {
            _configuracaoGestaoPatio = arg.Data;
            cb();
        }
    });
}

function recarregarSituacoesFilial() {
    var filial = _pesquisaFluxoPatio.Filial.codEntity();

    if (filial == 0) {
        _pesquisaFluxoPatio.EtapaFluxoGestaoPatio.options([]);
        SetarSelectMultiple(_pesquisaFluxoPatio.EtapaFluxoGestaoPatio);
        return;
    }

    executarReST("FluxoPatio/ObterEtapasDisponiveis", { Filial: filial, Tipo: _pesquisaFluxoPatio.Tipo.val() }, function (retorno) {
        if (retorno.Success && retorno.Data) {
            var etapasFluxoPatio = [];

            for (var i = 0; i < retorno.Data.length; i++) {
                etapasFluxoPatio.push({
                    text: retorno.Data[i].Descricao,
                    value: retorno.Data[i].Enumerador
                });
            }

            _pesquisaFluxoPatio.EtapaFluxoGestaoPatio.options(etapasFluxoPatio);
            SetarSelectMultiple(_pesquisaFluxoPatio.EtapaFluxoGestaoPatio);
        }
    });
}

function ObterDetalhesCargaFluxoClick(e) {
    if (_configuracaoGestaoPatio.OcultarFluxoCarga || e.Carga.val() == 0) return;
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

function edicaoEtapaFluxoPatioBloqueada() {
    return _fluxoAtual.CargaCancelada.val() || ((_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiEmbarcador) && (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiEmbarcador));
}

function ocultarBotoesEtapa(knoutEtapa) {
    for (var i in knoutEtapa) {
        var propriedadeEtapa = knoutEtapa[i];

        if ((propriedadeEtapa.type == types.event) && (propriedadeEtapa.visible instanceof Function))
            propriedadeEtapa.visible(false);
    }
}
