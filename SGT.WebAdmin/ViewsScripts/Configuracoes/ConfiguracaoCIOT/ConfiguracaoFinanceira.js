/// <reference path="../../Enumeradores/EnumTipoPagamentoCIOT.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />


var _configuracaoFinanceira = null;
var _gridConfiguracaoFinanceira;

var ConfiguracaoFinanceira = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });

    this.Adicionar = PropertyEntity({ type: types.event, text: "Adicionar", idBtnSearch: guid(), enable: ko.observable(true), eventClick: AdicionarTipoPagamentoClick });

    this.TipoDoPagamento = PropertyEntity({ text: "*Tipo Pagamento:", options: EnumTipoPagamentoCIOT.ObterOpcoes(), def: ko.observable(EnumTipoPagamentoCIOT.SemPgto), val: ko.observable(EnumTipoPagamentoCIOT.SemPgto), issue: 0, visible: ko.observable(true) });
    this.TipoMovimentoParaUso = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento Para Uso:", val: ko.observable(""), idBtnSearch: guid(), issue: 0, visible: ko.observable(true)});
    this.TipoMovimentoParaReversao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento Para Reversão:", val: ko.observable(""), idBtnSearch: guid(), issue: 0, visible: ko.observable(true)});

};

function LoadConfiguracaoFinanceira() {
    _configuracaoFinanceira = new ConfiguracaoFinanceira();
    KoBindings(_configuracaoFinanceira, "tabConfiguracaoFinanceira");

    new BuscarTipoMovimento(_configuracaoFinanceira.TipoMovimentoParaUso, "Tipo de Movimento para Uso", "Tipo de Movimento para Uso", retornoBuscaTipoMovimento);
    new BuscarTipoMovimento(_configuracaoFinanceira.TipoMovimentoParaReversao, "Tipo de Movimento para Reversão", "Tipo de Movimento para Reversão", retornoBuscaTipoMovimentoReversao);


    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: function (data) { ExcluirTipoConfiguracaoClick(_configuracaoFinanceira.Adicionar, data) } }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigosConfiguracao", visible: false },
        { data: "TipoPagamento", title: "Tipo pagamento", width: "30%" },
        { data: "TipoMovimentoUso", title: "Tipo de Movimento", width: "30%" },
        { data: "TipoMovimentoReversao", title: "T. Movimento Reversão", width: "30%" }
    ];
    _gridConfiguracaoFinanceira = new BasicDataTable(_configuracaoFinanceira.Grid.id, header, menuOpcoes);
    _configuracaoFinanceira.Adicionar.basicTable = _gridConfiguracaoFinanceira;

    RecarregarGridConfiguracaoFinanceira();
    $("#liConfiguracaoFinanceira").hide();
}

function LimparCamposConfiguracaoFinanceira() {
    LimparCampos(_configuracaoFinanceira);
}

function retornoBuscaTipoMovimento(data) {
    _configuracaoFinanceira.TipoMovimentoParaUso.codEntity(data.Codigo);
    _configuracaoFinanceira.TipoMovimentoParaUso.val(data.Descricao);
}

function retornoBuscaTipoMovimentoReversao(data) {
    _configuracaoFinanceira.TipoMovimentoParaReversao.codEntity(data.Codigo);
    _configuracaoFinanceira.TipoMovimentoParaReversao.val(data.Descricao);
}

function RecarregarGridConfiguracaoFinanceira() {
   
    var data = new Array();

    if (_configuracaoCIOT.ConfiguracaoFinanceira.val() != null && _configuracaoCIOT.ConfiguracaoFinanceira.val() != "") {
        $.each(_configuracaoCIOT.ConfiguracaoFinanceira.val(), function (i, configuracaoFinanceira) {
            var configuracaoFinanceiraGrid = {};

            configuracaoFinanceiraGrid.Codigo = configuracaoFinanceira.TipoPagamento.Codigo;
            configuracaoFinanceiraGrid.CodigosConfiguracao = configuracaoFinanceira.TipoPagamento.CodigosConfiguracao;
            configuracaoFinanceiraGrid.TipoPagamento = configuracaoFinanceira.TipoPagamento.TipoPagamento;
            configuracaoFinanceiraGrid.TipoMovimentoUso = configuracaoFinanceira.TipoPagamento.TipoMovimentoParaUso;
            configuracaoFinanceiraGrid.TipoMovimentoReversao = configuracaoFinanceira.TipoPagamento.TipoMovimentoParaReversao;

            data.push(configuracaoFinanceiraGrid);
        });
    }

    _gridConfiguracaoFinanceira.CarregarGrid(data);
}

function AdicionarTipoPagamentoClick(e) {
    const aviso = VerificarCamposObrigatorios()
    if (aviso)
        return exibirMensagem(tipoMensagem.aviso, "Aviso", aviso);

    var tipoPagamentoAtuais = _gridConfiguracaoFinanceira.BuscarRegistros();

    const existeTipoPagamento = ExisteTipoPagamentoNaGrid(tipoPagamentoAtuais);

    if (existeTipoPagamento)
        return exibirMensagem(tipoMensagem.aviso, "Aviso", "Já existe tipo de pagamento na grid");

    tipoPagamentoAtuais.push({
        Codigo: e.Codigo.val(),
        CodigosConfiguracao: `${_configuracaoFinanceira.TipoDoPagamento.val()},${e.TipoMovimentoParaUso.codEntity()},${e.TipoMovimentoParaReversao.codEntity()}`,
        TipoPagamento: ObterTipoPagamento(),
        TipoMovimentoUso: e.TipoMovimentoParaUso.val(),
        TipoMovimentoReversao: e.TipoMovimentoParaReversao.val()
    });

    _gridConfiguracaoFinanceira.CarregarGrid(tipoPagamentoAtuais);
    LimparCamposConfiguracaoFinanceira();
}

function ExcluirTipoConfiguracaoClick(knoutTipoConfiguracao, data) {
    const listaConfiguracoes = knoutTipoConfiguracao.basicTable.BuscarRegistros();

    for (var i = 0; i < listaConfiguracoes.length; i++) {
        if (data.Codigo == listaConfiguracoes[i].Codigo) {
            listaConfiguracoes.splice(i, 1);
            break;
        }
    }

    knoutTipoConfiguracao.basicTable.CarregarGrid(listaConfiguracoes);
}

function VerificarCamposObrigatorios() {
    const possuiValorTipoPagamento = _configuracaoFinanceira.TipoDoPagamento.val() ;
    const possuiValorTipoMovimentoParaUso = _configuracaoFinanceira.TipoMovimentoParaUso.val();
    const possuiValorTipoMovimentoParaReversao = _configuracaoFinanceira.TipoMovimentoParaReversao.val();

    if (typeof possuiValorTipoPagamento !== "number")
        return "Informe o tipo de pagamento";

    if (!possuiValorTipoMovimentoParaUso)
        return "Informe o tipo de movimento para uso";

    if (!possuiValorTipoMovimentoParaReversao)
        return "Informe o tipo de movimento para reversão"

}

function ObterTipoPagamento() {
    const tipos = EnumTipoPagamentoCIOT.ObterOpcoes();
    const selecionados = tipos.filter(tipo => tipo.value === _configuracaoFinanceira.TipoDoPagamento.val());
    return selecionados[0].text
}

function ExisteTipoPagamentoNaGrid(listaTipoPagamento) {
    const existeTipo = listaTipoPagamento.filter(tipo => tipo.TipoPagamento === ObterTipoPagamento())
    return existeTipo.length > 0 ? true : false;
}

function CarregarConfiguracaoFinanceira() {
    const listaConfiguracaoFinanceira = new Array();

    $.each(_configuracaoFinanceira.Adicionar.basicTable.BuscarRegistros(), function (i, configuracao) {
         listaConfiguracaoFinanceira.push({ TipoPagamento: configuracao });
    });

    _configuracaoCIOT.ConfiguracaoFinanceira.val(JSON.stringify(listaConfiguracaoFinanceira));
}