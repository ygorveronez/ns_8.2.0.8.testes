var _gridContato, _contato;

var opcoesSituacaoContato = [
    { text: "Ativo", value: true },
    { text: "Inativo", value: false }
];

var Contato = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Contato = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.GrupoPessoas.Contato.getRequiredFieldDescription()), required: true, maxlength: 150, enable: ko.observable(true) });
    this.Telefone = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.GrupoPessoas.Telefone.getFieldDescription()), required: false, maxlength: 20, getType: typesKnockout.phone, enable: ko.observable(true) });
    this.Email = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.GrupoPessoas.Email.getFieldDescription()), required: false, maxlength: 500, getType: typesKnockout.email, enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.GrupoPessoas.Situacao.getFieldDescription()), val: ko.observable(true), def: true, required: false, options: EnumSituacaoContato.obterOpcoes(), enable: ko.observable(true) });
    this.TipoContato = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: Localization.Resources.Pessoas.GrupoPessoas.TipoContato.getRequiredFieldDescription(), options: ko.observable([]), url: "TipoContato/BuscarTodos", visible: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarContatoClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarContatoClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Atualizar, visible: ko.observable(false), enable: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirContatoClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Excluir, visible: ko.observable(false), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarContatoClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Cancelar, visible: ko.observable(false), enable: ko.observable(true) });
}

function LoadContatoGrupoPessoas() {
    _contato = new Contato();
    KoBindings(_contato, "knockoutContato");

    CarregarGridContatos();
    RecarregarGridContato();
}

function CarregarGridContatos() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Pessoas.GrupoPessoas.Editar, id: guid(), metodo: EditarContatoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Contato", title: Localization.Resources.Pessoas.GrupoPessoas.Contato, width: "25%" },
        { data: "TipoContato", title: Localization.Resources.Pessoas.GrupoPessoas.TipoContato, width: "15%" },
        { data: "Email", title: Localization.Resources.Pessoas.GrupoPessoas.Email, width: "20%" },
        { data: "Telefone", title: Localization.Resources.Pessoas.GrupoPessoas.Telefone, width: "15%" },
        { data: "Situacao", title: Localization.Resources.Pessoas.GrupoPessoas.Situacao, width: "15%" }
    ];

    _gridContato = new BasicDataTable(_contato.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
}

function RecarregarGridContato() {
    var data = new Array();

    for (var i = 0; i < _grupoPessoas.Contatos.val().length; i++) {
        var contato = _grupoPessoas.Contatos.val()[i];

        data.push({
            Codigo: contato.Codigo,
            Contato: contato.Contato,
            TipoContato: contato.DescricaoTipoContato,
            Email: contato.Email,
            Telefone: contato.Telefone,
            Situacao: contato.DescricaoSituacao
        });
    }

    _gridContato.CarregarGrid(data);
}

function EditarContatoClick(contato) {

    for (var i = 0; i < _grupoPessoas.Contatos.val().length; i++) {
        var contatoExistente = _grupoPessoas.Contatos.val()[i];

        if (contatoExistente.Codigo == contato.Codigo) {
            _contato.Adicionar.visible(false);
            _contato.Atualizar.visible(true);
            _contato.Excluir.visible(true);
            _contato.Cancelar.visible(true);

            _contato.Codigo.val(contatoExistente.Codigo);
            _contato.Contato.val(contatoExistente.Contato);
            _contato.TipoContato.val(contatoExistente.TipoContato);
            _contato.Telefone.val(contatoExistente.Telefone);
            _contato.Email.val(contatoExistente.Email);
            _contato.Situacao.val(contatoExistente.Situacao);

            $("#" + _contato.TipoContato.id).change();

            break;
        }
    }

}

function AdicionarContatoClick(e, sender) {
    if (ValidarCamposObrigatorios(_contato) === false) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.GrupoPessoas.CamposObrigatorios, Localization.Resources.Pessoas.GrupoPessoas.InformeCamposObrigatorios);
        return;
    }

    if (_contato.TipoContato.val().length <= 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.GrupoPessoas.CamposObrigatorios, Localization.Resources.Pessoas.GrupoPessoas.PorfavorSelecioneTipoContrato);
        return;
    }

    _contato.Codigo.val(guid());

    _grupoPessoas.Contatos.val().push(ObterDadosContato());

    RecarregarGridContato();
    LimparCamposContato();
}

function AtualizarContatoClick(e, sender) {
    if (ValidarCamposObrigatorios(_contato) === false) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.GrupoPessoas.CamposObrigatorios, Localization.Resources.Pessoas.GrupoPessoas.InformeCamposObrigatorios);
        return;
    }

    if (_contato.TipoContato.val().length <= 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.GrupoPessoas.CamposObrigatorios, Localization.Resources.Pessoas.GrupoPessoas.PorfavorSelecioneTipoContrato);
        return;
    }

    for (var i = 0; i < _grupoPessoas.Contatos.val().length; i++) {
        var contatoExistente = _grupoPessoas.Contatos.val()[i];

        if (contatoExistente.Codigo == _contato.Codigo.val()) {
            _grupoPessoas.Contatos.val()[i] = ObterDadosContato();
            break;
        }
    }

    RecarregarGridContato();
    LimparCamposContato();
}

function ExcluirContatoClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Pessoas.GrupoPessoas.Atencao, Localization.Resources.Pessoas.GrupoPessoas.DesejaRealmenteExcluirContrato, function () {
        for (var i = 0; i < _grupoPessoas.Contatos.val().length; i++) {
            var contatoExistente = _grupoPessoas.Contatos.val()[i];

            if (contatoExistente.Codigo == _contato.Codigo.val()) {
                _grupoPessoas.Contatos.val().splice(i, 1);
                break;
            }
        }

        RecarregarGridContato();
        LimparCamposContato();
    });
}

function CancelarContatoClick(e, sender) {
    LimparCamposContato();
}

function LimparCamposContato() {
    _contato.Adicionar.visible(true);
    _contato.Atualizar.visible(false);
    _contato.Excluir.visible(false);
    _contato.Cancelar.visible(false);

    LimparCampos(_contato);
}

function ObterDadosContato() {
    return {
        Codigo: _contato.Codigo.val(),
        Contato: _contato.Contato.val(),
        TipoContato: _contato.TipoContato.val(),
        DescricaoTipoContato: ObterDescricaoTipoContato(_contato.TipoContato.val()),
        Email: _contato.Email.val(),
        Telefone: _contato.Telefone.val(),
        Situacao: _contato.Situacao.val(),
        DescricaoSituacao: _contato.Situacao.val() ? Localization.Resources.Pessoas.GrupoPessoas.Ativo : Localization.Resources.Pessoas.GrupoPessoas.Inativo
    };
}

function ObterDescricaoTipoContato(tipoContato) {
    var descricaoTipoContato = "";

    for (var i = 0; i < tipoContato.length; i++)
        descricaoTipoContato += $("#" + _contato.TipoContato.id + " option[value='" + tipoContato[i] + "']").text() + ", ";

    if (descricaoTipoContato.length > 0)
        descricaoTipoContato = descricaoTipoContato.substring(0, descricaoTipoContato.length - 2);

    return descricaoTipoContato;
}