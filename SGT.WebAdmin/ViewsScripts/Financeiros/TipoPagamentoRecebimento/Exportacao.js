//*******MAPEAMENTO KNOUCKOUT*******

var _gridConfiguracaoExportacao;
var _configuracaoExportacao;

var ConfiguracaoExportacao = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({});
    this.PlanoConta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Plano de Conta:", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.ContaContabil = PropertyEntity({ text: "Código da Conta:", required: false, enable: ko.observable(false) });
    this.Tipo = PropertyEntity({ text: "Tipo:", options: EnumDebitoCredito.ObterOpcoes(), val: ko.observable(EnumDebitoCredito.Debito), def: EnumDebitoCredito.Debito, issue: 0, visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.CodigoCentroResultado = PropertyEntity({ text: "Código do Centro de Resultado:", required: false, enable: ko.observable(false) });
    this.Reversao = PropertyEntity({ text: "Reversão", required: false, enable: ko.observable(false), getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarConfiguracaoExportacaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(false) });
};

//*******EVENTOS*******

function LoadConfiguracaoExportacao() {

    _configuracaoExportacao = new ConfiguracaoExportacao();
    KoBindings(_configuracaoExportacao, "tabExportacao");

    new BuscarPlanoConta(_configuracaoExportacao.PlanoConta, "Selecione a Conta Analítica para Exportação", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);
    new BuscarCentroResultado(_configuracaoExportacao.CentroResultado, "Selecione o Centro de Resultado para Exportação", "Centros de Resultado");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirConfiguracaoExportacaoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "PlanoConta", title: "Plano de Conta", width: "15%" },
        { data: "ContaContabil", title: "Código da Conta", width: "15%" },
        { data: "CentroResultado", title: "Centro de Resultado", width: "15%" },
        { data: "CodigoCentroResultado", title: "Código do Centro de Resultado", width: "20%" },
        { data: "Tipo", title: "Tipo", width: "10%" },
        { data: "Reversao", title: "Reversão", width: "10%" }
    ];

    _gridConfiguracaoExportacao = new BasicDataTable(_configuracaoExportacao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridConfiguracaoExportacao();
}

function RecarregarGridConfiguracaoExportacao() {

    var data = new Array();

    $.each(_tipoPagamentoRecebimento.ConfiguracoesExportacao.val(), function (i, config) {
        var configGrid = new Object();

        configGrid.Codigo = config.Codigo;
        configGrid.PlanoConta = config.PlanoConta.Descricao;
        configGrid.ContaContabil = config.ContaContabil;
        configGrid.Tipo = EnumDebitoCredito.ObterDescricao(config.Tipo);
        configGrid.CentroResultado = config.CentroResultado.Descricao;
        configGrid.CodigoCentroResultado = config.CodigoCentroResultado;
        configGrid.Reversao = config.Reversao ? "Sim" : "Não";

        data.push(configGrid);
    });

    _gridConfiguracaoExportacao.CarregarGrid(data);
}


function ExcluirConfiguracaoExportacaoClick(data) {
    var configuracoes = _tipoPagamentoRecebimento.ConfiguracoesExportacao.val();

    for (var i = 0; i < configuracoes.length; i++) {
        if (data.Codigo == configuracoes[i].Codigo) {
            configuracoes.splice(i, 1);
            break;
        }
    }

    _tipoPagamentoRecebimento.ConfiguracoesExportacao.val(configuracoes);

    RecarregarGridConfiguracaoExportacao();
}

function AdicionarConfiguracaoExportacaoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_configuracaoExportacao);

    if (valido) {
        var configuracoes = _tipoPagamentoRecebimento.ConfiguracoesExportacao.val();

        configuracoes.push({
            Codigo: guid(),
            PlanoConta: { Codigo: _configuracaoExportacao.PlanoConta.codEntity(), Descricao: _configuracaoExportacao.PlanoConta.val() },
            ContaContabil: _configuracaoExportacao.ContaContabil.val(),
            Tipo: _configuracaoExportacao.Tipo.val(),
            CentroResultado: { Codigo: _configuracaoExportacao.CentroResultado.codEntity(), Descricao: _configuracaoExportacao.CentroResultado.val() },
            CodigoCentroResultado: _configuracaoExportacao.CodigoCentroResultado.val(),
            Reversao: _configuracaoExportacao.Reversao.val()
        });

        _tipoPagamentoRecebimento.ConfiguracoesExportacao.val(configuracoes);

        RecarregarGridConfiguracaoExportacao();
        LimparCamposConfiguracaoExportacao();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function LimparCamposConfiguracaoExportacao() {
    LimparCampos(_configuracaoExportacao);
}