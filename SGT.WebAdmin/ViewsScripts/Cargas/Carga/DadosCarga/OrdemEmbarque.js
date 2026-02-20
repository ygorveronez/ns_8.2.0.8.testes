// #region Objetos Globais do Arquivo

var _gridOrdemEmbarqueHistoricoIntegracao;
var _ordemEmbarqueContainer;
var _pesquisaOrdemEmbarqueHistoricoIntegracao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var OrdemEmbarque = function (ordemEmbarque) {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({});
    this.PermiteCancelar = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), visible: false });
    this.Situacao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ text: Localization.Resources.Cargas.Carga.CancelarOrdemDeEmbarque, type: types.event, eventClick: cancelarOrdemEmbarqueClick, visible: ko.observable(false), idGrid: guid() });
    
    this.GridSituacoesHistorico;
    this.GridHistoricosIntegracao;

    PreencherObjetoKnout(this, { Data: ordemEmbarque });

    this.Situacao.visible(!this.PermiteCancelar.val());
    this.Cancelar.visible(this.PermiteCancelar.val());
}

var OrdemEmbarqueContainer = function () {
    this.OrdensEmbarque = ko.observableArray(new Array());
}

var PesquisaOrdemEmbarqueHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

// #endregion Classes

// #region Funções de Inicialização

function loadOrdemEmbarque() {
    _ordemEmbarqueContainer = new OrdemEmbarqueContainer();
    KoBindings(_ordemEmbarqueContainer, "knockoutOrdemEmbarqueContainer");

    _pesquisaOrdemEmbarqueHistoricoIntegracao = new PesquisaOrdemEmbarqueHistoricoIntegracao();

    loadGridOrdemEmbarqueHistoricoIntegracao();
}

function loadGridOrdemEmbarqueHistoricoIntegracao() {
    var opcaoDownload = { descricao: Localization.Resources.Cargas.Carga.DownloadArquivos, id: guid(), evento: "onclick", metodo: downloadArquivosOrdemEmbarqueHistoricoIntegracaoClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDownload] };

    _gridOrdemEmbarqueHistoricoIntegracao = new GridView("grid-ordem-embarque-historico-integracao", "Carga/PesquisaOrdemEmbarqueHistoricoIntegracao", _pesquisaOrdemEmbarqueHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function downloadArquivosOrdemEmbarqueHistoricoIntegracaoClick(registroSelecionado) {
    executarDownload("Carga/DownloadArquivosOrdemEmbarqueHistoricoIntegracao", { Codigo: registroSelecionado.Codigo });
}

function exibirOrdemEmbarqueHistoricoIntegracaoClick(registroSelecionado) {
    _pesquisaOrdemEmbarqueHistoricoIntegracao.Codigo.val(registroSelecionado.Codigo);
    _gridOrdemEmbarqueHistoricoIntegracao.CarregarGrid();

    Global.abrirModal("divModalOrdemEmbarqueHistoricoIntegracao");
}

function cancelarOrdemEmbarqueClick(registroSelecionado, e) {
    e.stopPropagation();
    exibirConfirmacao(Localization.Resources.Cargas.Carga.CancelarOrdemDeEmbarque + registroSelecionado.Descricao.val() + "?", Localization.Resources.Cargas.Carga.SeNecessarioUmaOrdemDeveSerReenviadaParaIntegracao, function () {
        executarReST("Carga/CancelarOrdemEmbarque", { Codigo: registroSelecionado.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    Global.fecharModal("divModalOrdemEmbarqueDetalhes");
                }
                else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);   
                }            
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.Carga.Falha, retorno.Msg);
            }
        });  
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções de Públicas

function exibirDetalhesOrdemEmbarque(cargaSelecionada) {
    executarReST("Carga/ObterOrdemEmbarqueDetalhes", { Carga: cargaSelecionada.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            $("#knockoutOrdemEmbarqueContainer").off('click', '**');

            _ordemEmbarqueContainer.OrdensEmbarque.removeAll();
            
            for (var i = 0; i < retorno.Data.length; i++) {
                var ordemEmbarque = retorno.Data[i];
                var knoutOrdemEmbarque = new OrdemEmbarque(ordemEmbarque.Dados);

                _ordemEmbarqueContainer.OrdensEmbarque.push(knoutOrdemEmbarque);

                knoutOrdemEmbarque.GridSituacoesHistorico = CriarGridSituacoesHistorico(ordemEmbarque.Dados.Codigo);
                knoutOrdemEmbarque.GridSituacoesHistorico.CarregarGrid(ordemEmbarque.SituacoesHistorico);

                knoutOrdemEmbarque.GridHistoricosIntegracao = criarGridHistoricosIntegracao(ordemEmbarque.Dados.Codigo);
                knoutOrdemEmbarque.GridHistoricosIntegracao.CarregarGrid(ordemEmbarque.HistoricosIntegracao);
            }

            $("#knockoutOrdemEmbarqueContainer").on('click', '.dd-handle', function (e) {
                e.stopPropagation();
                $(e.currentTarget).parent().toggleClass('dd-collapsed');
            });

            Global.abrirModal("divModalOrdemEmbarqueDetalhes");
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.Carga.Falha, retorno.Msg);
    });   
}

// #endregion Funções de Públicas

// #region Funções de Privadas

function criarGridHistoricosIntegracao(codigoOrdemEmbarque) {
    var opcaoDetalhes = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), metodo: exibirOrdemEmbarqueHistoricoIntegracaoClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoDetalhes] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "DataIntegracao", title: Localization.Resources.Cargas.Carga.DataDeIntegracao, width: "20%", className: "text-align-center" },
        { data: "Tipo", title: Localization.Resources.Cargas.Carga.Tipo, width: "20%", className: "text-align-center" },
        { data: "SituacaoIntegracao", title: Localization.Resources.Gerais.Geral.Situacao, width: "20%", className: "text-align-center" },
        { data: "ProblemaIntegracao", title: Localization.Resources.Cargas.Carga.Retorno, width: "40%", className: "text-align-left" }
    ];

    return new BasicDataTable("grid-ordem-embarque-historico-integracao-" + codigoOrdemEmbarque, header, menuOpcoes);
}

function CriarGridSituacoesHistorico(codigoOrdemEmbarque) {
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoIntegracao", title: Localization.Resources.Cargas.Carga.DataDeIntegracao, width: "20%", className: "text-align-center" },
        { data: "Situacao", title: Localization.Resources.Gerais.Geral.Situacao, width: "40%", className: "text-align-left" },
        { data: "DataCriacao", title: Localization.Resources.Cargas.Carga.DataDeCriacao, width: "20%", className: "text-align-center" },
        { data: "DataAtualizacao", title: Localization.Resources.Cargas.Carga.DataDeAtualizacao, width: "20%", className: "text-align-center" }
    ];

    return new BasicDataTable("grid-ordem-embarque-situacao-historico-" + codigoOrdemEmbarque, header);
}

// #endregion Funções de Privadas
