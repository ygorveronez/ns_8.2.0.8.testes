/// <reference path="../../../js/Global/Charts.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Tranportador.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cTeAguardandoIntegracao;
var _integracaoAutomaticaCTe;
var _pesquisaIndicadorIntegracaoCTe;

/*
 * Declaração das Classes
 */

var CTeAguardandoIntegracao = function () {
    var self = this;

    this._grid;
    this._idGrid = "grid-cte-aguardando-integracao";
    this._url = "IndicadorIntegracaoCTe/PesquisaCTeAguardandoIntegracao";
    this._urlExportacao = "IndicadorIntegracaoCTe/ExportarPesquisaCTeAguardandoIntegracao";

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            self.ExibirFiltros.visibleFade(!self.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this._init();
}

CTeAguardandoIntegracao.prototype = {
    carregarDados: function () {
        this._grid.CarregarGrid();
    },
    _init: function () {
        var totalRegistrosPorPagina = 10;
        var configuracaoExportacao = {
            url: this._urlExportacao,
            titulo: "CT-e Aguardando Integração"
        };

        this._grid = new GridViewExportacao(this._idGrid, this._url, _pesquisaIndicadorIntegracaoCTe, undefined, configuracaoExportacao, undefined, totalRegistrosPorPagina);
    }
}

var IntegracaoAutomaticaCTe = function () {
    var self = this;

    this._grafico;
    this._idGrafico = "grafico-integracao-automatica-cte";
    this._url = "IndicadorIntegracaoCTe/PesquisaGraficoIntegracaoAutomaticaCTe";
    this._urlExportacao = "IndicadorIntegracaoCTe/ExportarPesquisaGraficoIntegracaoAutomaticaCTe";

    this.Mensagem = PropertyEntity({});
    this.TotalEmitidos = PropertyEntity({});

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (self.ExibirFiltros.visibleFade())
                self.ExibirFiltros.visibleFade(false);
            else {
                self.ExibirFiltros.visibleFade(true);
                self._grafico.render();
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this._init();
}

IntegracaoAutomaticaCTe.prototype = {
    carregarDados: function () {
        var self = this;
        
        executarReST(self._url, RetornarObjetoPesquisa(_pesquisaIndicadorIntegracaoCTe), function (retorno) {
            if (retorno.Success) {
                PreencherObjetoKnout(self, { Data: retorno.Data.Informacoes });
  
                self.Mensagem.val("");
                self._grafico.updateOptions({ data: retorno.Data.Dados });
            }
            else
                self.Mensagem.val(retorno.Msg);

            self._grafico.search();
        });
    },
    _adicionarEventoBotaoExportar: function () {
        var self = this;

        $(document).on('click', "#exportar-" + self._idGrafico, function (event) {
            if (event && event.preventDefault)
                event.preventDefault();

            self._desabilitarBotaoExportar();

            executarDownload(
                self._urlExportacao,
                RetornarObjetoPesquisa(_pesquisaIndicadorIntegracaoCTe),
                function () {
                    self._habilitarBotaoExportar();
                },
                function (html) {
                    self._habilitarBotaoExportar();

                    try {
                        var retorno = JSON.parse(html.replace("(", "").replace(");", ""));

                        if (retorno.Data)
                            exibirMensagem(tipoMensagem.atencao, "Atenção!", retorno.Msg);
                        else
                            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
                    }
                    catch (excecao) {
                        exibirMensagem(tipoMensagem.falha, "Falha!", "Ocorreu uma falha ao exportar o arquivo.");
                    }
                }
            );
        });
    },
    _adicionarGrafico: function () {
        var options = {
            type: ChartType.Pie,
            idContainer: this._idGrafico,
            margin: {
                top: 50,
                right: 0,
                left: 0,
                bottom: 50
            },
            data: new Array(),
            title: "",
            width: 0,
            height: 350,
            pieLabels: {
                mainLabel: {
                    fontSize: 14
                }
            }
        };

        this._grafico = new Chart(options);
        this._grafico.init();
    },
    _desabilitarBotaoExportar: function () {
        $("#exportar-" + this._idGrafico)
            .attr('disabled', true)
            .addClass('disabled')
            .html("Exportando...");
    },
    _habilitarBotaoExportar: function () {
        $("#exportar-" + this._idGrafico)
            .attr('disabled', false)
            .removeClass('disabled')
            .html('<i class="fa fa-file-excel-o"></i> Exportar Excel');
    },
    _init: function () {
        this._adicionarGrafico();
        this._adicionarEventoBotaoExportar();
    }
}

var PesquisaIndicadorIntegracaoCTe = function () {
    this.DataEmissaoInicio = PropertyEntity({ text: "Data Emissão Início: ", getType: typesKnockout.date });
    this.DataEmissaoLimite = PropertyEntity({ text: "Data Emissão Limite: ", getType: typesKnockout.date });
    this.DataIntegracaoInicio = PropertyEntity({ text: "Data Integração Início: ", getType: typesKnockout.date });
    this.DataIntegracaoLimite = PropertyEntity({ text: "Data Integração Limite: ", getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });

    this.DataEmissaoInicio.dateRangeLimit = this.DataEmissaoLimite;
    this.DataEmissaoLimite.dateRangeInit = this.DataEmissaoInicio;

    this.DataIntegracaoInicio.dateRangeLimit = this.DataIntegracaoLimite;
    this.DataIntegracaoLimite.dateRangeInit = this.DataIntegracaoInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            $("#indicador-integracao-cte-container").removeClass("d-none");
            _pesquisaIndicadorIntegracaoCTe.ExibirFiltros.visibleFade(false);

            _integracaoAutomaticaCTe.carregarDados();
            _cTeAguardandoIntegracao.carregarDados();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadIndicadorIntegracaoCTe() {
    _pesquisaIndicadorIntegracaoCTe = new PesquisaIndicadorIntegracaoCTe();
    KoBindings(_pesquisaIndicadorIntegracaoCTe, "knockoutPesquisaIndicadorIntegracaoCTe", false, _pesquisaIndicadorIntegracaoCTe.Pesquisar.id);

    _integracaoAutomaticaCTe = new IntegracaoAutomaticaCTe();
    KoBindings(_integracaoAutomaticaCTe, "knockoutIntegracaoAutomaticaCTe");

    _cTeAguardandoIntegracao = new CTeAguardandoIntegracao();
    KoBindings(_cTeAguardandoIntegracao, "knockoutCTeAguardandoIntegracao");

    new BuscarFilial(_pesquisaIndicadorIntegracaoCTe.Filial);
    new BuscarTransportadores(_pesquisaIndicadorIntegracaoCTe.Transportador);
}
