//*******MAPEAMENTO KNOUCKOUT*******

var _configuracaoContratoFreteTerceirosTipoOperacao, _gridConfiguracaoContratoFreteTerceirosTipoOperacao;

var ConfiguracaoContratoFreteTerceirosTipoOperacao = function () {

    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: false });
    this.TipoMovimentoPagamentoViaCIOT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pagamento via CIOT:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: false });
    this.TipoMovimentoReversaoPagamentoViaCIOT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Reversão do Pagamento via CIOT:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: false });
    this.TipoMovimentoGeracaoTitulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Geração de Título:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: true });
    this.TipoMovimentoReversaoGeracaoTitulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da Geração de Título:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: true });

    this.DiferenciarMovimentoValorINSS = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Diferenciar os movimentos financeiros para o INSS ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoValorINSS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });
    this.TipoMovimentoReversaoValorINSS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });

    this.DiferenciarMovimentoValorINSSPatronal = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Diferenciar os movimentos financeiros para o INSS patronal ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoValorINSSPatronal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });
    this.TipoMovimentoReversaoValorINSSPatronal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });

    this.DiferenciarMovimentoValorIRRF = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Diferenciar os movimentos financeiros para o IRRF ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoValorIRRF = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });
    this.TipoMovimentoReversaoValorIRRF = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });

    this.DiferenciarMovimentoValorSEST = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Diferenciar os movimentos financeiros para o SEST ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoValorSEST = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });
    this.TipoMovimentoReversaoValorSEST = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });

    this.DiferenciarMovimentoValorSENAT = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Diferenciar os movimentos financeiros para o SENAT ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoValorSENAT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });
    this.TipoMovimentoReversaoValorSENAT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });

    this.DiferenciarMovimentoValorTarifaSaque = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Diferenciar os movimentos financeiros para a tarifa de saque ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoValorTarifaSaque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });
    this.TipoMovimentoReversaoValorTarifaSaque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });

    this.DiferenciarMovimentoValorTarifaTransferencia = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Diferenciar os movimentos financeiros para a tarifa de transferência ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoValorTarifaTransferencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });
    this.TipoMovimentoReversaoValorTarifaTransferencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });

    this.DiferenciarMovimentoValorLiquido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Diferenciar os movimentos financeiros para o valor líquido ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoValorLiquido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });
    this.TipoMovimentoReversaoValorLiquido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });

    this.DiferenciarMovimentoValorAbastecimento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Diferenciar os movimentos financeiros para o valor de abastecimento ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoValorAbastecimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });
    this.TipoMovimentoReversaoValorAbastecimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });

    this.DiferenciarMovimentoValorAdiantamento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Diferenciar os movimentos financeiros para o valor de adiantamento ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoValorAdiantamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });
    this.TipoMovimentoReversaoValorAdiantamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });

    this.DiferenciarMovimentoValorSaldo = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Diferenciar os movimentos financeiros para o valor do saldo ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoValorSaldo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });
    this.TipoMovimentoReversaoValorSaldo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });

    this.DiferenciarMovimentoValorTotal = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Diferenciar os movimentos financeiros para o valor total ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoValorTotal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });
    this.TipoMovimentoReversaoValorTotal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: false });

    this.DiferenciarMovimentoValorINSS.val.subscribe(function (novoValor) {
        DiferenciarMovimentoConfiguracaoContratoTerceiroTipoOperacaoChange(novoValor, "INSS");
    });

    this.DiferenciarMovimentoValorINSSPatronal.val.subscribe(function (novoValor) {
        DiferenciarMovimentoConfiguracaoContratoTerceiroTipoOperacaoChange(novoValor, "INSSPatronal");
    });

    this.DiferenciarMovimentoValorIRRF.val.subscribe(function (novoValor) {
        DiferenciarMovimentoConfiguracaoContratoTerceiroTipoOperacaoChange(novoValor, "IRRF");
    });

    this.DiferenciarMovimentoValorSEST.val.subscribe(function (novoValor) {
        DiferenciarMovimentoConfiguracaoContratoTerceiroTipoOperacaoChange(novoValor, "SEST");
    });

    this.DiferenciarMovimentoValorSENAT.val.subscribe(function (novoValor) {
        DiferenciarMovimentoConfiguracaoContratoTerceiroTipoOperacaoChange(novoValor, "SENAT");
    });

    this.DiferenciarMovimentoValorTarifaSaque.val.subscribe(function (novoValor) {
        DiferenciarMovimentoConfiguracaoContratoTerceiroTipoOperacaoChange(novoValor, "TarifaSaque");
    });

    this.DiferenciarMovimentoValorTarifaTransferencia.val.subscribe(function (novoValor) {
        DiferenciarMovimentoConfiguracaoContratoTerceiroTipoOperacaoChange(novoValor, "TarifaTransferencia");
    });

    this.DiferenciarMovimentoValorLiquido.val.subscribe(function (novoValor) {
        DiferenciarMovimentoConfiguracaoContratoTerceiroTipoOperacaoChange(novoValor, "Liquido");
    });

    this.DiferenciarMovimentoValorAbastecimento.val.subscribe(function (novoValor) {
        DiferenciarMovimentoConfiguracaoContratoTerceiroTipoOperacaoChange(novoValor, "Abastecimento");
    });

    this.DiferenciarMovimentoValorAdiantamento.val.subscribe(function (novoValor) {
        DiferenciarMovimentoConfiguracaoContratoTerceiroTipoOperacaoChange(novoValor, "Adiantamento");
    });

    this.DiferenciarMovimentoValorSaldo.val.subscribe(function (novoValor) {
        DiferenciarMovimentoConfiguracaoContratoTerceiroTipoOperacaoChange(novoValor, "Saldo");
    });

    this.DiferenciarMovimentoValorTotal.val.subscribe(function (novoValor) {
        DiferenciarMovimentoConfiguracaoContratoTerceiroTipoOperacaoChange(novoValor, "Total");
    });

    this.Atualizar = PropertyEntity({ eventClick: AtualizarConfiguracaoContratoFreteTerceirosTipoOperacaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirConfiguracaoContratoFreteTerceirosTipoOperacaoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarConfiguracaoContratoFreteTerceirosTipoOperacaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarConfiguracaoContratoFreteTerceirosTipoOperacaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadConfiguracaoContratoFreteTerceirosTipoOperacao() {

    _configuracaoContratoFreteTerceirosTipoOperacao = new ConfiguracaoContratoFreteTerceirosTipoOperacao();
    KoBindings(_configuracaoContratoFreteTerceirosTipoOperacao, "knockoutConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao");

    new BuscarTiposOperacao(_configuracaoContratoFreteTerceirosTipoOperacao.TipoOperacao);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoGeracaoTitulo);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoGeracaoTitulo);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoPagamentoViaCIOT);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoPagamentoViaCIOT);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorAbastecimento);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorAdiantamento);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorINSS);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorINSSPatronal);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorIRRF);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorLiquido);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorSaldo);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorSENAT);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorSEST);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorTarifaSaque);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorTarifaTransferencia);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorTotal);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorAbastecimento);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorAdiantamento);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorINSS);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorINSSPatronal);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorIRRF);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorLiquido);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorSaldo);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorSENAT);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorSEST);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorTarifaSaque);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorTarifaTransferencia);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorTotal);

    LoadGridConfiguracoesContratoTerceiroTipoOperacao();
}

function LoadGridConfiguracoesContratoTerceiroTipoOperacao() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarConfiguracaoContratoFreteTerceirosTipoOperacaoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "TipoOperacao", title: "Tipo de Operação", width: "90%" }
    ];

    _gridConfiguracaoContratoFreteTerceirosTipoOperacao = new BasicDataTable(_configuracaoContratoFreteTerceirosTipoOperacao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.desc });

    RecarregarGridConfiguracaoContratoFreteTerceirosTipoOperacao();
}

function DiferenciarMovimentoConfiguracaoContratoTerceiroTipoOperacaoChange(novoValor, tipo) {
    _configuracaoContratoFreteTerceirosTipoOperacao["DiferenciarMovimentoValor" + tipo].visibleFade(novoValor);
    _configuracaoContratoFreteTerceirosTipoOperacao["TipoMovimentoValor" + tipo].required = novoValor;
    _configuracaoContratoFreteTerceirosTipoOperacao["TipoMovimentoReversaoValor" + tipo].required = novoValor;
}

function RecarregarGridConfiguracaoContratoFreteTerceirosTipoOperacao() {

    var data = new Array();

    $.each(_configuracaoContratoFreteTerceiros.ConfiguracoesTipoOperacao.val(), function (i, configuracao) {
        var configuracaoGrid = new Object();

        configuracaoGrid.Codigo = configuracao.Codigo;
        configuracaoGrid.TipoOperacao = configuracao.TipoOperacao.Descricao;

        data.push(configuracaoGrid);
    });

    _gridConfiguracaoContratoFreteTerceirosTipoOperacao.CarregarGrid(data);
}

function ObterConfiguracaoContratoFreteTerceirosTipoOperacao(novoRegistro) {
    return {
        Codigo: novoRegistro ? guid() : _configuracaoContratoFreteTerceirosTipoOperacao.Codigo.val(),
        TipoOperacao: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoOperacao.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoOperacao.val()
        },
        TipoMovimentoGeracaoTitulo: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoGeracaoTitulo.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoGeracaoTitulo.val()
        },
        TipoMovimentoReversaoGeracaoTitulo: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoGeracaoTitulo.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoGeracaoTitulo.val()
        },
        TipoMovimentoPagamentoViaCIOT: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoPagamentoViaCIOT.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoPagamentoViaCIOT.val()
        },
        TipoMovimentoReversaoPagamentoViaCIOT: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoPagamentoViaCIOT.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoPagamentoViaCIOT.val()
        },
        DiferenciarMovimentoValorINSS: _configuracaoContratoFreteTerceirosTipoOperacao.DiferenciarMovimentoValorINSS.val(),
        TipoMovimentoValorINSS: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorINSS.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorINSS.val()
        },
        TipoMovimentoReversaoValorINSS: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorINSS.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorINSS.val()
        },
        DiferenciarMovimentoValorINSSPatronal: _configuracaoContratoFreteTerceirosTipoOperacao.DiferenciarMovimentoValorINSSPatronal.val(),
        TipoMovimentoValorINSSPatronal: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorINSSPatronal.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorINSSPatronal.val()
        },
        TipoMovimentoReversaoValorINSSPatronal: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorINSSPatronal.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorINSSPatronal.val()
        },
        DiferenciarMovimentoValorIRRF: _configuracaoContratoFreteTerceirosTipoOperacao.DiferenciarMovimentoValorIRRF.val(),
        TipoMovimentoValorIRRF: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorIRRF.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorIRRF.val()
        },
        TipoMovimentoReversaoValorIRRF: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorIRRF.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorIRRF.val()
        },
        DiferenciarMovimentoValorSEST: _configuracaoContratoFreteTerceirosTipoOperacao.DiferenciarMovimentoValorSEST.val(),
        TipoMovimentoValorSEST: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorSEST.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorSEST.val()
        },
        TipoMovimentoReversaoValorSEST: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorSEST.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorSEST.val()
        },
        DiferenciarMovimentoValorSENAT: _configuracaoContratoFreteTerceirosTipoOperacao.DiferenciarMovimentoValorSENAT.val(),
        TipoMovimentoValorSENAT: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorSENAT.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorSENAT.val()
        },
        TipoMovimentoReversaoValorSENAT: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorSENAT.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorSENAT.val()
        },
        DiferenciarMovimentoValorTarifaSaque: _configuracaoContratoFreteTerceirosTipoOperacao.DiferenciarMovimentoValorTarifaSaque.val(),
        TipoMovimentoValorTarifaSaque: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorTarifaSaque.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorTarifaSaque.val()
        },
        TipoMovimentoReversaoValorTarifaSaque: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorTarifaSaque.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorTarifaSaque.val()
        },
        DiferenciarMovimentoValorTarifaTransferencia: _configuracaoContratoFreteTerceirosTipoOperacao.DiferenciarMovimentoValorTarifaTransferencia.val(),
        TipoMovimentoValorTarifaTransferencia: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorTarifaTransferencia.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorTarifaTransferencia.val()
        },
        TipoMovimentoReversaoValorTarifaTransferencia: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorTarifaTransferencia.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorTarifaTransferencia.val()
        },
        DiferenciarMovimentoValorLiquido: _configuracaoContratoFreteTerceirosTipoOperacao.DiferenciarMovimentoValorLiquido.val(),
        TipoMovimentoValorLiquido: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorLiquido.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorLiquido.val()
        },
        TipoMovimentoReversaoValorLiquido: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorLiquido.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorLiquido.val()
        },
        DiferenciarMovimentoValorAbastecimento: _configuracaoContratoFreteTerceirosTipoOperacao.DiferenciarMovimentoValorAbastecimento.val(),
        TipoMovimentoValorAbastecimento: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorAbastecimento.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorAbastecimento.val()
        },
        TipoMovimentoReversaoValorAbastecimento: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorAbastecimento.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorAbastecimento.val()
        },
        DiferenciarMovimentoValorAdiantamento: _configuracaoContratoFreteTerceirosTipoOperacao.DiferenciarMovimentoValorAdiantamento.val(),
        TipoMovimentoValorAdiantamento: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorAdiantamento.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorAdiantamento.val()
        },
        TipoMovimentoReversaoValorAdiantamento: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorAdiantamento.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorAdiantamento.val()
        },
        DiferenciarMovimentoValorSaldo: _configuracaoContratoFreteTerceirosTipoOperacao.DiferenciarMovimentoValorSaldo.val(),
        TipoMovimentoValorSaldo: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorSaldo.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorSaldo.val()
        },
        TipoMovimentoReversaoValorSaldo: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorSaldo.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorSaldo.val()
        },
        DiferenciarMovimentoValorTotal: _configuracaoContratoFreteTerceirosTipoOperacao.DiferenciarMovimentoValorTotal.val(),
        TipoMovimentoValorTotal: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorTotal.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoValorTotal.val()
        },
        TipoMovimentoReversaoValorTotal: {
            Codigo: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorTotal.codEntity(),
            Descricao: _configuracaoContratoFreteTerceirosTipoOperacao.TipoMovimentoReversaoValorTotal.val()
        }
    };
}

function AdicionarConfiguracaoContratoFreteTerceirosTipoOperacaoClick(e, sender) {
    if (!ValidarCamposObrigatorios(_configuracaoContratoFreteTerceirosTipoOperacao)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    var configuracoes = _configuracaoContratoFreteTerceiros.ConfiguracoesTipoOperacao.val();

    configuracoes.push(ObterConfiguracaoContratoFreteTerceirosTipoOperacao(true));

    _configuracaoContratoFreteTerceiros.ConfiguracoesTipoOperacao.val(configuracoes);

    RecarregarGridConfiguracaoContratoFreteTerceirosTipoOperacao();
    LimparCamposConfiguracaoContratoFreteTerceirosTipoOperacao();
}

function AtualizarConfiguracaoContratoFreteTerceirosTipoOperacaoClick(e, sender) {
    if (!ValidarCamposObrigatorios(_configuracaoContratoFreteTerceirosTipoOperacao)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    var configuracoes = _configuracaoContratoFreteTerceiros.ConfiguracoesTipoOperacao.val();

    for (var i = 0; i < configuracoes.length; i++) {
        if (_configuracaoContratoFreteTerceirosTipoOperacao.Codigo.val() == configuracoes[i].Codigo) {
            configuracoes[i] = ObterConfiguracaoContratoFreteTerceirosTipoOperacao(false);
            break;
        }
    }

    _configuracaoContratoFreteTerceiros.ConfiguracoesTipoOperacao.val(configuracoes);

    RecarregarGridConfiguracaoContratoFreteTerceirosTipoOperacao();
    LimparCamposConfiguracaoContratoFreteTerceirosTipoOperacao();
}

function ExcluirConfiguracaoContratoFreteTerceirosTipoOperacaoClick(e, sender) {
    var configuracoes = _configuracaoContratoFreteTerceiros.ConfiguracoesTipoOperacao.val();

    for (var i = 0; i < configuracoes.length; i++) {
        if (_configuracaoContratoFreteTerceirosTipoOperacao.Codigo.val() == configuracoes[i].Codigo) {
            configuracoes.splice(i, 1);
            break;
        }
    }

    _configuracaoContratoFreteTerceiros.ConfiguracoesTipoOperacao.val(configuracoes);

    RecarregarGridConfiguracaoContratoFreteTerceirosTipoOperacao();
    LimparCamposConfiguracaoContratoFreteTerceirosTipoOperacao();
}

function CancelarConfiguracaoContratoFreteTerceirosTipoOperacaoClick(e, sender) {
    LimparCamposConfiguracaoContratoFreteTerceirosTipoOperacao();
}

function EditarConfiguracaoContratoFreteTerceirosTipoOperacaoClick(data) {

    var configuracoes = _configuracaoContratoFreteTerceiros.ConfiguracoesTipoOperacao.val();

    for (var i = 0; i < configuracoes.length; i++) {
        if (data.Codigo == configuracoes[i].Codigo) {
            PreencherObjetoKnout(_configuracaoContratoFreteTerceirosTipoOperacao, { Data: configuracoes[i] });
            break;
        }
    }

    _configuracaoContratoFreteTerceirosTipoOperacao.Atualizar.visible(true);
    _configuracaoContratoFreteTerceirosTipoOperacao.Excluir.visible(true);
    _configuracaoContratoFreteTerceirosTipoOperacao.Cancelar.visible(true);
    _configuracaoContratoFreteTerceirosTipoOperacao.Adicionar.visible(false);
}

//*******MÉTODOS*******

function LimparCamposConfiguracaoContratoFreteTerceirosTipoOperacao() {
    LimparCampos(_configuracaoContratoFreteTerceirosTipoOperacao);

    _configuracaoContratoFreteTerceirosTipoOperacao.Atualizar.visible(false);
    _configuracaoContratoFreteTerceirosTipoOperacao.Excluir.visible(false);
    _configuracaoContratoFreteTerceirosTipoOperacao.Cancelar.visible(false);
    _configuracaoContratoFreteTerceirosTipoOperacao.Adicionar.visible(true);
}