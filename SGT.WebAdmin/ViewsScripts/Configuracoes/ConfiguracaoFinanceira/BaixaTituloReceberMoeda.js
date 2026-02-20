//*******MAPEAMENTO KNOUCKOUT*******

var _configuracaoBaixaTituloReceberMoeda, _gridConfiguracaoBaixaTituloReceberMoeda;

var ConfiguracaoBaixaTituloReceberMoeda = function () {

    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Moeda = PropertyEntity({ text: "Moeda:", options: EnumMoedaCotacaoBancoCentral.obterOpcoesMoedasEstrangeiras(), val: ko.observable(EnumMoedaCotacaoBancoCentral.DolarVenda), def: EnumMoedaCotacaoBancoCentral.DolarVenda, issue: 0, visible: ko.observable(true) });

    this.JustificativaAcrescimo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Justificativa para Acréscimo:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: true });
    this.JustificativaDesconto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Justificativa para Desconto:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: true });

    this.Atualizar = PropertyEntity({ eventClick: AtualizarConfiguracaoBaixaTituloReceberMoedaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirConfiguracaoBaixaTituloReceberMoedaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarConfiguracaoBaixaTituloReceberMoedaClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarConfiguracaoBaixaTituloReceberMoedaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadConfiguracaoBaixaTituloReceberMoeda() {

    _configuracaoBaixaTituloReceberMoeda = new ConfiguracaoBaixaTituloReceberMoeda();
    KoBindings(_configuracaoBaixaTituloReceberMoeda, "knockoutConfiguracaoFinanceiraBaixaTituloReceberMoeda");

    new BuscarJustificativas(_configuracaoBaixaTituloReceberMoeda.JustificativaAcrescimo, null, EnumTipoJustificativa.Acrescimo, [EnumTipoFinalidadeJustificativa.TitulosReceber]);
    new BuscarJustificativas(_configuracaoBaixaTituloReceberMoeda.JustificativaDesconto, null, EnumTipoJustificativa.Desconto, [EnumTipoFinalidadeJustificativa.TitulosReceber]);

    LoadGridConfiguracaoBaixaTituloReceberMoeda();
}

function LoadGridConfiguracaoBaixaTituloReceberMoeda() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarConfiguracaoBaixaTituloReceberMoedaClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Moeda", title: "Moeda", width: "90%" }
    ];

    _gridConfiguracaoBaixaTituloReceberMoeda = new BasicDataTable(_configuracaoBaixaTituloReceberMoeda.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridConfiguracaoBaixaTituloReceberMoeda();
}

function RecarregarGridConfiguracaoBaixaTituloReceberMoeda() {

    var data = new Array();

    $.each(_configuracaoBaixaTituloReceber.ConfiguracoesMoedas.val(), function (i, configuracao) {
        var configuracaoGrid = new Object();

        configuracaoGrid.Codigo = configuracao.Codigo;
        configuracaoGrid.Moeda = EnumMoedaCotacaoBancoCentral.obterDescricao(configuracao.Moeda);

        data.push(configuracaoGrid);
    });

    _gridConfiguracaoBaixaTituloReceberMoeda.CarregarGrid(data);
}

function ObterConfiguracaoBaixaTituloReceberMoeda(novoRegistro) {
    return {
        Codigo: novoRegistro ? guid() : _configuracaoBaixaTituloReceberMoeda.Codigo.val(),
        Moeda: _configuracaoBaixaTituloReceberMoeda.Moeda.val(),
        JustificativaAcrescimo: {
            Codigo: _configuracaoBaixaTituloReceberMoeda.JustificativaAcrescimo.codEntity(),
            Descricao: _configuracaoBaixaTituloReceberMoeda.JustificativaAcrescimo.val()
        },
        JustificativaDesconto: {
            Codigo: _configuracaoBaixaTituloReceberMoeda.JustificativaDesconto.codEntity(),
            Descricao: _configuracaoBaixaTituloReceberMoeda.JustificativaDesconto.val()
        },
    };
}

function AdicionarConfiguracaoBaixaTituloReceberMoedaClick(e, sender) {
    if (!ValidarCamposObrigatorios(_configuracaoBaixaTituloReceberMoeda)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    var configuracoes = _configuracaoBaixaTituloReceber.ConfiguracoesMoedas.val();

    for (var i = 0; i < configuracoes.length; i++) {
        if (_configuracaoBaixaTituloReceberMoeda.Moeda.val() === configuracoes[i].Moeda) {
            exibirMensagem(tipoMensagem.atencao, "Configuração já existente!", "Já existe uma configuração para a moeda selecionada!");
            return;
        }
    }

    configuracoes.push(ObterConfiguracaoBaixaTituloReceberMoeda(true));

    _configuracaoBaixaTituloReceber.ConfiguracoesMoedas.val(configuracoes);

    RecarregarGridConfiguracaoBaixaTituloReceberMoeda();
    LimparCamposConfiguracaoBaixaTituloReceberMoeda();
}

function AtualizarConfiguracaoBaixaTituloReceberMoedaClick(e, sender) {
    if (!ValidarCamposObrigatorios(_configuracaoBaixaTituloReceberMoeda)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    var configuracoes = _configuracaoBaixaTituloReceber.ConfiguracoesMoedas.val();

    for (var i = 0; i < configuracoes.length; i++) {
        if (_configuracaoBaixaTituloReceberMoeda.Moeda.val() === configuracoes[i].Moeda && _configuracaoBaixaTituloReceberMoeda.Codigo.val() !== configuracoes[i].Codigo) {
            exibirMensagem(tipoMensagem.atencao, "Configuração já existente!", "Já existe uma configuração para a moeda selecionada!");
            return;
        }
    }

    for (var i = 0; i < configuracoes.length; i++) {
        if (_configuracaoBaixaTituloReceberMoeda.Codigo.val() == configuracoes[i].Codigo) {
            configuracoes[i] = ObterConfiguracaoBaixaTituloReceberMoeda(false);
            break;
        }
    }

    _configuracaoBaixaTituloReceber.ConfiguracoesMoedas.val(configuracoes);

    RecarregarGridConfiguracaoBaixaTituloReceberMoeda();
    LimparCamposConfiguracaoBaixaTituloReceberMoeda();
}

function ExcluirConfiguracaoBaixaTituloReceberMoedaClick(e, sender) {
    var configuracoes = _configuracaoBaixaTituloReceber.ConfiguracoesMoedas.val();

    for (var i = 0; i < configuracoes.length; i++) {
        if (_configuracaoBaixaTituloReceberMoeda.Codigo.val() == configuracoes[i].Codigo) {
            configuracoes.splice(i, 1);
            break;
        }
    }

    _configuracaoBaixaTituloReceber.ConfiguracoesMoedas.val(configuracoes);

    RecarregarGridConfiguracaoBaixaTituloReceberMoeda();
    LimparCamposConfiguracaoBaixaTituloReceberMoeda();
}

function CancelarConfiguracaoBaixaTituloReceberMoedaClick(e, sender) {
    LimparCamposConfiguracaoBaixaTituloReceberMoeda();
}

function EditarConfiguracaoBaixaTituloReceberMoedaClick(data) {

    var configuracoes = _configuracaoBaixaTituloReceber.ConfiguracoesMoedas.val();

    for (var i = 0; i < configuracoes.length; i++) {
        if (data.Codigo == configuracoes[i].Codigo) {
            PreencherObjetoKnout(_configuracaoBaixaTituloReceberMoeda, { Data: configuracoes[i] });
            break;
        }
    }

    _configuracaoBaixaTituloReceberMoeda.Atualizar.visible(true);
    _configuracaoBaixaTituloReceberMoeda.Excluir.visible(true);
    _configuracaoBaixaTituloReceberMoeda.Cancelar.visible(true);
    _configuracaoBaixaTituloReceberMoeda.Adicionar.visible(false);
}

//*******MÉTODOS*******

function LimparCamposConfiguracaoBaixaTituloReceberMoeda() {
    LimparCampos(_configuracaoBaixaTituloReceberMoeda);

    _configuracaoBaixaTituloReceberMoeda.Atualizar.visible(false);
    _configuracaoBaixaTituloReceberMoeda.Excluir.visible(false);
    _configuracaoBaixaTituloReceberMoeda.Cancelar.visible(false);
    _configuracaoBaixaTituloReceberMoeda.Adicionar.visible(true);
}