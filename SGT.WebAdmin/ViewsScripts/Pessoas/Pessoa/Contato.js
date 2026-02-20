var _gridContato, _contato;

var Contato = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Contato = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Contato.getRequiredFieldDescription()), required: true, maxlength: 150, enable: ko.observable(true) });
    this.Telefone = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Telefone.getRequiredFieldDescription()), required: false, maxlength: 20, getType: typesKnockout.phone });
    this.Email = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Email.getRequiredFieldDescription()), required: false, maxlength: 500, getType: typesKnockout.email });
    this.Situacao = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Situacao.getRequiredFieldDescription()), val: ko.observable(true), def: true, required: false, options: Global.ObterOpcoesBooleano(Localization.Resources.Enumeradores.InativoAtivo.Ativo, Localization.Resources.Enumeradores.InativoAtivo.Inativo) });
    this.TipoContato = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: Localization.Resources.Pessoas.Pessoa.TipoDeContato, options: ko.observable([]), url: "TipoContato/BuscarTodos", visible: ko.observable(true) });
    this.CPF = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.CPF.getRequiredFieldDescription()), required: false, maxlength: 14, getType: typesKnockout.cpf });
    this.Cargo = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Cargo.getRequiredFieldDescription()), required: false, maxlength: 200, enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarContatoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarContatoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirContatoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarContatoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}

function LoadContatoPessoa() {
    _contato = new Contato();
    KoBindings(_contato, "knockoutContato");

    CarregarGridContatos();
    RecarregarGridContato();
}

function CarregarGridContatos() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: EditarContatoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Contato", title: Localization.Resources.Pessoas.Pessoa.Contato, width: "25%" },
        { data: "CPF", title: Localization.Resources.Pessoas.Pessoa.CPF, width: "10%" },
        { data: "TipoContato", title: Localization.Resources.Pessoas.Pessoa.TipoDeContato, width: "10%" },
        { data: "Email", title: Localization.Resources.Pessoas.Pessoa.Email, width: "15%" },
        { data: "Telefone", title: Localization.Resources.Pessoas.Pessoa.Telefone, width: "10%" },
        { data: "Cargo", title: Localization.Resources.Pessoas.Pessoa.Cargo, width: "10%" },
        { data: "Situacao", title: Localization.Resources.Pessoas.Pessoa.Situacao, width: "10%" }
    ];

    _gridContato = new BasicDataTable(_contato.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
}

function RecarregarGridContato() {
    var data = new Array();

    for (var i = 0; i < _pessoa.Contatos.val().length; i++) {
        var contato = _pessoa.Contatos.val()[i];

        data.push({
            Codigo: contato.Codigo,
            Contato: contato.Contato,
            CPF: contato.CPF,
            TipoContato: contato.DescricaoTipoContato,
            Email: contato.Email,
            Telefone: contato.Telefone,
            Cargo: contato.Cargo,
            Situacao: contato.DescricaoSituacao
        });
    }

    _gridContato.CarregarGrid(data);
}

function EditarContatoClick(contato) {

    for (var i = 0; i < _pessoa.Contatos.val().length; i++) {
        var contatoExistente = _pessoa.Contatos.val()[i];

        if (contatoExistente.Codigo == contato.Codigo) {
            _contato.Adicionar.visible(false);
            _contato.Atualizar.visible(true);
            _contato.Excluir.visible(true);
            _contato.Cancelar.visible(true);

            _contato.Codigo.val(contatoExistente.Codigo);
            _contato.Contato.val(contatoExistente.Contato);
            _contato.CPF.val(contatoExistente.CPF);
            _contato.TipoContato.val(contatoExistente.TipoContato);
            _contato.Telefone.val(contatoExistente.Telefone);
            _contato.Email.val(contatoExistente.Email);
            _contato.Cargo.val(contatoExistente.Cargo);
            _contato.Situacao.val(contatoExistente.Situacao);

            $("#" + _contato.TipoContato.id).change();

            break;
        }
    }
}

function AdicionarContatoClick(e, sender) {
    if (ValidarCamposObrigatorios(_contato) === false) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    var cpfCnpj = _contato.CPF.val().replace(/[^0-9]/g, '');
    if (cpfCnpj.length == 11) {
        if (!ValidarCPF(cpfCnpj)) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pessoas.Pessoa.CPFInformadoInvalido);
            $("#" + cpfCnpj.id).focus();
            return;
        }
    }

    if (_contato.TipoContato.val().length <= 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pessoas.Pessoa.PorFavorSelecioneUmTipoDeContato);
        return;
    }

    _contato.Codigo.val(guid());

    _pessoa.Contatos.val().push(ObterDadosContato());

    RecarregarGridContato();
    LimparCamposContato();
}

function AtualizarContatoClick(e, sender) {
    if (ValidarCamposObrigatorios(_contato) === false) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    if (_contato.TipoContato.val().length <= 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pessoas.Pessoa.PorFavorSelecioneUmTipoDeContato);
        return;
    }

    for (var i = 0; i < _pessoa.Contatos.val().length; i++) {
        var contatoExistente = _pessoa.Contatos.val()[i];

        if (contatoExistente.Codigo == _contato.Codigo.val()) {
            _pessoa.Contatos.val()[i] = ObterDadosContato();
            break;
        }
    }

    RecarregarGridContato();
    LimparCamposContato();
}

function ExcluirContatoClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pessoas.Pessoa.DesejaRealmenteExcluirEsteContato, function () {
        for (var i = 0; i < _pessoa.Contatos.val().length; i++) {
            var contatoExistente = _pessoa.Contatos.val()[i];

            if (contatoExistente.Codigo == _contato.Codigo.val()) {
                _pessoa.Contatos.val().splice(i, 1);
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
        CPF: _contato.CPF.val(),
        TipoContato: _contato.TipoContato.val(),
        DescricaoTipoContato: ObterDescricaoTipoContato(_contato.TipoContato.val()),
        Email: _contato.Email.val(),
        Telefone: _contato.Telefone.val(),
        Cargo: _contato.Cargo.val(),
        Situacao: _contato.Situacao.val(),
        DescricaoSituacao: _contato.Situacao.val() ? Localization.Resources.Gerais.Geral.Ativo : Localization.Resources.Gerais.Geral.Inativo
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