/// <reference path="../../../js/Global/Charts.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _integracaoAutomatica;
var _integracaoPorEmail;
var _pesquisaIndicadorIntegracaoNFe;
var _tabelaIntegracaoRejeitada;

/*
 * Declaração das Classes
 */

var GraficoIndicadorIntegracaoNFe = function (opcoes) {
    var self = this;

    this._grafico;
    this._idGrafico = opcoes.idGrafico;
    this._url = opcoes.url;
    this._urlExportacao = opcoes.urlExportacao;

    this.Mensagem = PropertyEntity({});
    this.TotalRegistros = PropertyEntity({});

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

GraficoIndicadorIntegracaoNFe.prototype = {
    carregarDados: function () {
        var self = this;

        executarReST(self._url, RetornarObjetoPesquisa(_pesquisaIndicadorIntegracaoNFe), function (retorno) {
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
                RetornarObjetoPesquisa(_pesquisaIndicadorIntegracaoNFe),
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

var PesquisaIndicadorIntegracaoNFe = function () {
    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            $("#indicador-integracao-nfe-container").removeClass("d-none");
            _pesquisaIndicadorIntegracaoNFe.ExibirFiltros.visibleFade(false);

            _integracaoAutomatica.carregarDados();
            _integracaoPorEmail.carregarDados();
            _tabelaIntegracaoRejeitada.carregarDados();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var TabelaIntegracaoRejeitada = function () {
    var self = this;

    this._grid;
    this._idGrid = "grid-integracao-rejeitada";
    this._url = "IndicadorIntegracaoNFe/PesquisaIntegracaoRejeitada";
    this._urlExportacao = "IndicadorIntegracaoNFe/ExportarPesquisaIntegracaoRejeitada";

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            self.ExibirFiltros.visibleFade(!self.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this._init();
}

TabelaIntegracaoRejeitada.prototype = {
    carregarDados: function () {
        this._grid.CarregarGrid();
    },
    _init: function () {
        var totalRegistrosPorPagina = 10;
        var configuracaoExportacao = {
            url: this._urlExportacao,
            titulo: "NF-e Integração Rejeitada"
        };

        this._grid = new GridViewExportacao(this._idGrid, this._url, _pesquisaIndicadorIntegracaoNFe, undefined, configuracaoExportacao, undefined, totalRegistrosPorPagina);
    }
}

/*
 * Declaração das Funções de Inicialização
 */

function loadIndicadorIntegracaoNFe() {
    _pesquisaIndicadorIntegracaoNFe = new PesquisaIndicadorIntegracaoNFe();
    KoBindings(_pesquisaIndicadorIntegracaoNFe, "knockoutPesquisaIndicadorIntegracaoNFe", false, _pesquisaIndicadorIntegracaoNFe.Pesquisar.id);

    var opcoesIntegracaoAutomatica = {
        idGrafico: "grafico-integracao-automatica-nfe",
        url: "IndicadorIntegracaoNFe/PesquisaGraficoIntegracaoAutomatica",
        urlExportacao: "IndicadorIntegracaoNFe/ExportarPesquisaGraficoIntegracaoAutomatica"
    };
    _integracaoAutomatica = new GraficoIndicadorIntegracaoNFe(opcoesIntegracaoAutomatica);
    KoBindings(_integracaoAutomatica, "knockoutIntegracaoAutomatica");

    var opcoesIntegracaoPorEmail = {
        idGrafico: "grafico-integracao-email-nfe",
        url: "IndicadorIntegracaoNFe/PesquisaGraficoIntegracaoPorEmail",
        urlExportacao: "IndicadorIntegracaoNFe/ExportarPesquisaGraficoIntegracaoPorEmail"
    };
    _integracaoPorEmail= new GraficoIndicadorIntegracaoNFe(opcoesIntegracaoPorEmail);
    KoBindings(_integracaoPorEmail, "knockoutIntegracaoPorEmail");

    _tabelaIntegracaoRejeitada = new TabelaIntegracaoRejeitada();
    KoBindings(_tabelaIntegracaoRejeitada, "knockoutTabelaIntegracaoRejeitada");

    new BuscarFilial(_pesquisaIndicadorIntegracaoNFe.Filial);
}
