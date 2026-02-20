//*******MAPEAMENTO KNOUCKOUT*******

var _configuracaoGNRERegistro, _gridConfiguracaoGNRERegistro;

var ConfiguracaoGNRERegistro = function () {
    this.Grid = PropertyEntity({ type: types.local });
    
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Estado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Estado:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: true });
    this.CFOP = PropertyEntity({ type: types.entity, codEntity: ko.observable(1), text: "CFOP:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: false });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pessoa:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: true });
    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: true });
    this.PorcentagemDesconto = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable(0), def: 0, text: "Porcentagem Desconto:", visible: ko.observable(true) });
    
    this.Atualizar = PropertyEntity({ eventClick: AtualizarConfiguracaoGNRERegistroClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirConfiguracaoGNRERegistroClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarConfiguracaoGNRERegistroClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarConfiguracaoGNRERegistroClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadConfiguracaoGNRERegistro() {

    _configuracaoGNRERegistro = new ConfiguracaoGNRERegistro();
    KoBindings(_configuracaoGNRERegistro, "knockoutConfiguracaoFinanceiraGNRERegistro");

    new BuscarTipoMovimento(_configuracaoGNRERegistro.TipoMovimento);
    new BuscarClientes(_configuracaoGNRERegistro.Pessoa);
    new BuscarEstados(_configuracaoGNRERegistro.Estado);
    new BuscarCFOPs(_configuracaoGNRERegistro.CFOP, null, null, true);

    LoadGridConfiguracaoGNRERegistro();
}

function LoadGridConfiguracaoGNRERegistro() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarConfiguracaoGNRERegistroClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Estado", title: "Estado", width: "10%" },
        { data: "CFOP", title: "CFOP", width: "20%" },
        { data: "Pessoa", title: "Pessoa", width: "25%" },
        { data: "TipoMovimento", title: "Tipo de Movimento", width: "25%" },
        { data: "PorcentagemDesconto", title: "Porcentagem Desconto", width: "10%" }
    ];

    _gridConfiguracaoGNRERegistro = new BasicDataTable(_configuracaoGNRERegistro.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridConfiguracaoGNRERegistro();
}

function RecarregarGridConfiguracaoGNRERegistro() {

    var data = new Array();

    $.each(_configuracaoGNRE.ConfiguracoesRegistros.val(), function (i, configuracao) {
        var configuracaoGrid = new Object();

        configuracaoGrid.Codigo = configuracao.Codigo;
        configuracaoGrid.Estado = configuracao.Estado.Descricao;
        configuracaoGrid.CFOP = configuracao.CFOP.Descricao;
        configuracaoGrid.Pessoa = configuracao.Pessoa.Descricao;
        configuracaoGrid.TipoMovimento = configuracao.TipoMovimento.Descricao;
        configuracaoGrid.PorcentagemDesconto = configuracao.PorcentagemDesconto;

        data.push(configuracaoGrid);
    });

    _gridConfiguracaoGNRERegistro.CarregarGrid(data);
}

function ObterConfiguracaoGNRERegistro(novoRegistro) {
    return {
        Codigo: novoRegistro ? guid() : _configuracaoGNRERegistro.Codigo.val(),
        PorcentagemDesconto: _configuracaoGNRERegistro.PorcentagemDesconto.val(),
        Estado: {
            Codigo: _configuracaoGNRERegistro.Estado.codEntity(),
            Descricao: _configuracaoGNRERegistro.Estado.val()
        },
        CFOP: {
            Codigo: _configuracaoGNRERegistro.CFOP.codEntity(),
            Descricao: _configuracaoGNRERegistro.CFOP.val()
        },
        Pessoa: {
            Codigo: _configuracaoGNRERegistro.Pessoa.codEntity(),
            Descricao: _configuracaoGNRERegistro.Pessoa.val()
        },
        TipoMovimento: {
            Codigo: _configuracaoGNRERegistro.TipoMovimento.codEntity(),
            Descricao: _configuracaoGNRERegistro.TipoMovimento.val()
        }
    };
}

function AdicionarConfiguracaoGNRERegistroClick(e, sender) {
    if (!ValidarCamposObrigatorios(_configuracaoGNRERegistro)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    var configuracoes = _configuracaoGNRE.ConfiguracoesRegistros.val();

    for (var i = 0; i < configuracoes.length; i++) {
        if (_configuracaoGNRERegistro.Estado.codEntity() === configuracoes[i].Estado.Codigo && _configuracaoGNRERegistro.CFOP.codEntity() === configuracoes[i].CFOP.Codigo) {
            exibirMensagem(tipoMensagem.atencao, "Configuração já existente!", "Já existe uma configuração para o Estado e CFOP selecionados!");
            return;
        }
    }

    configuracoes.push(ObterConfiguracaoGNRERegistro(true));

    _configuracaoGNRE.ConfiguracoesRegistros.val(configuracoes);

    RecarregarGridConfiguracaoGNRERegistro();
    LimparCamposConfiguracaoGNRERegistro();
}

function AtualizarConfiguracaoGNRERegistroClick(e, sender) {
    if (!ValidarCamposObrigatorios(_configuracaoGNRERegistro)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    var configuracoes = _configuracaoGNRE.ConfiguracoesRegistros.val();

    for (var i = 0; i < configuracoes.length; i++) {
        if (_configuracaoGNRERegistro.Codigo.val() !== configuracoes[i].Codigo && _configuracaoGNRERegistro.Estado.codEntity() === configuracoes[i].Estado.Codigo && _configuracaoGNRERegistro.CFOP.codEntity() === configuracoes[i].CFOP.Codigo) {
            exibirMensagem(tipoMensagem.atencao, "Configuração já existente!", "Já existe uma configuração para o Estado e CFOP selecionados!");
            return;
        }
    }

    for (i = 0; i < configuracoes.length; i++) {
        if (_configuracaoGNRERegistro.Codigo.val() === configuracoes[i].Codigo) {
            configuracoes[i] = ObterConfiguracaoGNRERegistro(false);
            break;
        }
    }

    _configuracaoGNRE.ConfiguracoesRegistros.val(configuracoes);

    RecarregarGridConfiguracaoGNRERegistro();
    LimparCamposConfiguracaoGNRERegistro();
}

function ExcluirConfiguracaoGNRERegistroClick(e, sender) {
    var configuracoes = _configuracaoGNRE.ConfiguracoesRegistros.val();

    for (var i = 0; i < configuracoes.length; i++) {
        if (_configuracaoGNRERegistro.Codigo.val() === configuracoes[i].Codigo) {
            configuracoes.splice(i, 1);
            break;
        }
    }

    _configuracaoGNRE.ConfiguracoesRegistros.val(configuracoes);

    RecarregarGridConfiguracaoGNRERegistro();
    LimparCamposConfiguracaoGNRERegistro();
}

function CancelarConfiguracaoGNRERegistroClick(e, sender) {
    LimparCamposConfiguracaoGNRERegistro();
}

function EditarConfiguracaoGNRERegistroClick(data) {

    var configuracoes = _configuracaoGNRE.ConfiguracoesRegistros.val();

    for (var i = 0; i < configuracoes.length; i++) {
        if (data.Codigo === configuracoes[i].Codigo) {
            PreencherObjetoKnout(_configuracaoGNRERegistro, { Data: configuracoes[i] });
            break;
        }
    }

    _configuracaoGNRERegistro.Atualizar.visible(true);
    _configuracaoGNRERegistro.Excluir.visible(true);
    _configuracaoGNRERegistro.Cancelar.visible(true);
    _configuracaoGNRERegistro.Adicionar.visible(false);
}

//*******MÉTODOS*******

function LimparCamposConfiguracaoGNRERegistro() {
    LimparCampos(_configuracaoGNRERegistro);

    _configuracaoGNRERegistro.Atualizar.visible(false);
    _configuracaoGNRERegistro.Excluir.visible(false);
    _configuracaoGNRERegistro.Cancelar.visible(false);
    _configuracaoGNRERegistro.Adicionar.visible(true);
}