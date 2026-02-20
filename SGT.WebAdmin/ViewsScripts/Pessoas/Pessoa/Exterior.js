var _gridOutraDescricaoPessoaExterior, _outraDescricaoPessoaExterior;

var OutraDescricaoPessoaExterior = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.RazaoSocial = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.RazaoSocial.getRequiredFieldDescription(), required: true, maxlength: 150, enable: ko.observable(true) });
    this.Endereco = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Endereco.getRequiredFieldDescription(), required: true, maxlength: 150, enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarOutraDescricaoPessoaExteriorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarOutraDescricaoPessoaExteriorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirOutraDescricaoPessoaExteriorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarOutraDescricaoPessoaExteriorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}

function LoadOutraDescricaoPessoaExteriorPessoa() {
    _outraDescricaoPessoaExterior = new OutraDescricaoPessoaExterior();
    KoBindings(_outraDescricaoPessoaExterior, "knockoutExterior");

    CarregarGridOutraDescricaoPessoaExterior();
    RecarregarGridOutraDescricaoPessoaExterior();
}

function CarregarGridOutraDescricaoPessoaExterior() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: EditarOutraDescricaoPessoaExteriorClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "RazaoSocial", title: Localization.Resources.Pessoas.Pessoa.RazaoSocial, width: "45%" },
        { data: "Endereco", title: Localization.Resources.Pessoas.Pessoa.Endereco, width: "45%" },
    ];

    _gridOutraDescricaoPessoaExterior = new BasicDataTable(_outraDescricaoPessoaExterior.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
}

function RecarregarGridOutraDescricaoPessoaExterior() {
    var data = new Array();

    for (var i = 0; i < _pessoa.OutrasDescricoesPessoaExterior.val().length; i++) {
        var outraDescricaoPessoaExterior = _pessoa.OutrasDescricoesPessoaExterior.val()[i];

        data.push(outraDescricaoPessoaExterior);
    }

    _gridOutraDescricaoPessoaExterior.CarregarGrid(data);
}

function EditarOutraDescricaoPessoaExteriorClick(outraDescricaoPessoaExterior) {

    for (var i = 0; i < _pessoa.OutrasDescricoesPessoaExterior.val().length; i++) {
        var outraDescricaoPessoaExteriorExistente = _pessoa.OutrasDescricoesPessoaExterior.val()[i];

        if (outraDescricaoPessoaExteriorExistente.Codigo == outraDescricaoPessoaExterior.Codigo) {
            _outraDescricaoPessoaExterior.Adicionar.visible(false);
            _outraDescricaoPessoaExterior.Atualizar.visible(true);
            _outraDescricaoPessoaExterior.Excluir.visible(true);
            _outraDescricaoPessoaExterior.Cancelar.visible(true);

            _outraDescricaoPessoaExterior.Codigo.val(outraDescricaoPessoaExteriorExistente.Codigo);
            _outraDescricaoPessoaExterior.RazaoSocial.val(outraDescricaoPessoaExteriorExistente.RazaoSocial);
            _outraDescricaoPessoaExterior.Endereco.val(outraDescricaoPessoaExteriorExistente.Endereco);

            break;
        }
    }
}

function AdicionarOutraDescricaoPessoaExteriorClick(e, sender) {
    if (ValidarCamposObrigatorios(_outraDescricaoPessoaExterior) === false) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    _outraDescricaoPessoaExterior.Codigo.val(guid());

    _pessoa.OutrasDescricoesPessoaExterior.val().push(ObterDadosOutraDescricaoPessoaExterior());

    RecarregarGridOutraDescricaoPessoaExterior();
    LimparCamposOutraDescricaoPessoaExterior();
}

function AtualizarOutraDescricaoPessoaExteriorClick(e, sender) {
    if (ValidarCamposObrigatorios(_outraDescricaoPessoaExterior) === false) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    for (var i = 0; i < _pessoa.OutrasDescricoesPessoaExterior.val().length; i++) {
        var outraDescricaoPessoaExteriorExistente = _pessoa.OutrasDescricoesPessoaExterior.val()[i];

        if (outraDescricaoPessoaExteriorExistente.Codigo == _outraDescricaoPessoaExterior.Codigo.val()) {
            _pessoa.OutrasDescricoesPessoaExterior.val()[i] = ObterDadosOutraDescricaoPessoaExterior();
            break;
        }
    }

    RecarregarGridOutraDescricaoPessoaExterior();
    LimparCamposOutraDescricaoPessoaExterior();
}

function ExcluirOutraDescricaoPessoaExteriorClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.DesejaExcluirRegistro, function () {
        for (var i = 0; i < _pessoa.OutrasDescricoesPessoaExterior.val().length; i++) {
            var outraDescricaoPessoaExteriorExistente = _pessoa.OutrasDescricoesPessoaExterior.val()[i];

            if (outraDescricaoPessoaExteriorExistente.Codigo == _outraDescricaoPessoaExterior.Codigo.val()) {
                _pessoa.OutrasDescricoesPessoaExterior.val().splice(i, 1);
                break;
            }
        }

        RecarregarGridOutraDescricaoPessoaExterior();
        LimparCamposOutraDescricaoPessoaExterior();
    });
}

function CancelarOutraDescricaoPessoaExteriorClick(e, sender) {
    LimparCamposOutraDescricaoPessoaExterior();
}

function LimparCamposOutraDescricaoPessoaExterior() {
    _outraDescricaoPessoaExterior.Adicionar.visible(true);
    _outraDescricaoPessoaExterior.Atualizar.visible(false);
    _outraDescricaoPessoaExterior.Excluir.visible(false);
    _outraDescricaoPessoaExterior.Cancelar.visible(false);

    LimparCampos(_outraDescricaoPessoaExterior);
}

function ObterDadosOutraDescricaoPessoaExterior() {
    return {
        Codigo: _outraDescricaoPessoaExterior.Codigo.val(),
        RazaoSocial: _outraDescricaoPessoaExterior.RazaoSocial.val(),
        Endereco: _outraDescricaoPessoaExterior.Endereco.val()
    };
}