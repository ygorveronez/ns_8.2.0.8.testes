var _configuracaoAngelLira;
var _gridAngelLiraExcecoes;

var ConfiguracaoAngelLira = function () {
    this.IntegracaoProcedimentoEmbarque = PropertyEntity({ text: "Procedimento de Embarque:", val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 12, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) });
    this.TempoEntregaAngelLira = PropertyEntity({ text: "Tempo para Entrega (minutos):", val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 3, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) });
    this.CodigoModeloContratacao = PropertyEntity({ text: "Modelo de Contratação:", val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 2, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) });
    this.PossuiIntegracaoAngelLira = PropertyEntity({ text: "Possui integração com a AngelLira", val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
    this.NaoEnviarDataInicioETerminoViagemAngelLira = PropertyEntity({ text: "Não enviar data de início e término de viagem", val: ko.observable(false), def: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS), getType: typesKnockout.bool });
    this.IntegrarPreSMAngelLira = PropertyEntity({ text: "Integrar pré SM na carga", val: ko.observable(false), def: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS), getType: typesKnockout.bool });
    this.ReintegrarSMCargaAngelLira = PropertyEntity({ text: "Reenviar SM na etapa de Integração da Carga", val: ko.observable(false), def: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS), getType: typesKnockout.bool, visible: true });

    this.Excecoes = PropertyEntity({ type: types.event, val: ko.observableArray(), idGrid: guid(), text: "Adicionar Exceção", eventClick: abrirModalAngelLiraExcecao });
    
    this.Excecoes.val.subscribe(function () {
        recarregarGridExcecoesAngelLira();
    });
};

function LoadConfiguracaoAngelLira() {
    _configuracaoAngelLira = new ConfiguracaoAngelLira();
    KoBindings(_configuracaoAngelLira, "tabAngelLira");

    _tipoOperacao.IntegracaoProcedimentoEmbarque = _configuracaoAngelLira.IntegracaoProcedimentoEmbarque;
    _tipoOperacao.CodigoModeloContratacao = _configuracaoAngelLira.CodigoModeloContratacao;
    _tipoOperacao.TempoEntregaAngelLira = _configuracaoAngelLira.TempoEntregaAngelLira;
    _tipoOperacao.PossuiIntegracaoAngelLira = _configuracaoAngelLira.PossuiIntegracaoAngelLira;
    _tipoOperacao.NaoEnviarDataInicioETerminoViagemAngelLira = _configuracaoAngelLira.NaoEnviarDataInicioETerminoViagemAngelLira;
    _tipoOperacao.IntegrarPreSMAngelLira = _configuracaoAngelLira.IntegrarPreSMAngelLira;
    _tipoOperacao.ReintegrarSMCargaAngelLira = _configuracaoAngelLira.ReintegrarSMCargaAngelLira;

    loadGridAngelLiraExcecoes();
    loadAngelLiraExcecao();
}

function loadGridAngelLiraExcecoes() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoDestino", visible: false },
        { data: "Destino", title: "Destino", width: "33%", className: "text-align-left" },
        { data: "ValorMinimo", title: "Valor Mínimo", width: "33%", className: "text-align-right" },
        { data: "ProcedimentoEmbarque", title: "Procedimento de Embarque", width: "33%", className: "text-align-right" }
    ];
    
    var opcaoRemover = { descricao: "Remover", id: guid(), evento: "onclick", metodo: removerExcecaoAngelLiraClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoRemover] };
    
    _gridAngelLiraExcecoes = new BasicDataTable(_configuracaoAngelLira.Excecoes.idGrid, header, menuOpcoes, null, null, 5);
    recarregarGridExcecoesAngelLira();
}

function removerExcecaoAngelLiraClick(registroSelecionado) {
    var registros = _gridAngelLiraExcecoes.BuscarRegistros();

    for (var i = 0; i < registros.length; i++) {
        if (registros[i].Codigo == registroSelecionado.Codigo) {
            registros.splice(i, 1);
            break;
        }
    }
    
    _configuracaoAngelLira.Excecoes.val(registros);
}

function recarregarGridExcecoesAngelLira() {
    var registros = _configuracaoAngelLira.Excecoes.val();
    
    _gridAngelLiraExcecoes.CarregarGrid(registros);
}

function limparCamposAngelLira() {
    if (_configuracaoAngelLira == undefined)
        return;
    
    _configuracaoAngelLira.Excecoes.val([]);
}

function preencherListaExcecoesAngelLira(excecoes) {
    if (_configuracaoAngelLira == undefined)
        return;

    _configuracaoAngelLira.Excecoes.val(excecoes);
}