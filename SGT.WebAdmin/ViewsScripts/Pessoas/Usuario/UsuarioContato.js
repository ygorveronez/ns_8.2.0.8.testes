/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumTipoParentesco.js" />
/// <reference path="Usuario.js" />

var _gridUsuarioContatos;

//*******MAPEAMENTO KNOUCKOUT*******

var UsuarioContatoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Nome = PropertyEntity({ type: types.map, val: "" });
    this.Telefone = PropertyEntity({ type: types.map, val: "" });
    this.Email = PropertyEntity({ type: types.map, val: "" });
    this.TipoParentesco = PropertyEntity({ type: types.map, val: "" });
    this.CPF = PropertyEntity({ type: types.map, val: "" });
    this.DataNascimento = PropertyEntity({ type: types.map, val: "" });
};

//*******EVENTOS*******

function loadUsuarioContato() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarUsuarioContato, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "TipoParentesco", visible: false },
        { data: "Nome", title: Localization.Resources.Gerais.Geral.Nome, width: "30%" },
        { data: "CPF", title: Localization.Resources.Pessoas.Usuario.CPF, width: "10%" },
        { data: "DataNascimento", title: Localization.Resources.Pessoas.Usuario.DataNascimento, width: "10%" },
        { data: "Telefone", title: Localization.Resources.Pessoas.Usuario.Telefone, width: "10%" },
        { data: "Email", title: Localization.Resources.Pessoas.Usuario.Email, width: "20%" },
        { data: "DescricaoParentesco", title: Localization.Resources.Pessoas.Usuario.GrauParentesco, width: "20%"}
    ];

    _gridUsuarioContatos = new BasicDataTable(_usuario.GridUsuarioContatos.idGrid, header, menuOpcoes);
    recarregarGridUsuarioContatos();
}

function adicionarUsuarioContatoClick() {
    if (validarCamposObrigatoriosUsuarioContato()) {
        var usuarioContato = new UsuarioContatoMap();
        usuarioContato.Codigo.val = guid();

        usuarioContato.TipoParentesco.val = _usuario.TipoParentescoContato.val();
        usuarioContato.Nome.val = _usuario.NomeContato.val();
        usuarioContato.Telefone.val = _usuario.TelefoneContato.val();
        usuarioContato.Email.val = _usuario.EmailContato.val();
        usuarioContato.CPF.val = _usuario.CPFContato.val();
        usuarioContato.DataNascimento.val = _usuario.DataNascimentoContato.val();

        _usuario.GridUsuarioContatos.list.push(usuarioContato);
        recarregarGridUsuarioContatos();
        $("#" + _usuario.NomeContato.id).focus();

        LimparCamposUsuarioContatos();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function atualizarUsuarioContatoClick() {
    if (validarCamposObrigatoriosUsuarioContato()) {
        $.each(_usuario.GridUsuarioContatos.list, function (i, usuarioContato) {
            if (usuarioContato.Codigo.val == _usuario.CodigoContato.val()) {

                usuarioContato.TipoParentesco.val = _usuario.TipoParentescoContato.val();
                usuarioContato.Nome.val = _usuario.NomeContato.val();
                usuarioContato.Telefone.val = _usuario.TelefoneContato.val();
                usuarioContato.Email.val = _usuario.EmailContato.val();
                usuarioContato.CPF.val = _usuario.CPFContato.val();
                usuarioContato.DataNascimento.val = _usuario.DataNascimentoContato.val();

                return false;
            }
        });
        recarregarGridUsuarioContatos();
        LimparCamposUsuarioContatos();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function excluirUsuarioContatoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pessoas.Usuario.RealmenteDesejaExcluirContatoSelecionado, function () {
        var listaAtualizada = new Array();
        $.each(_usuario.GridUsuarioContatos.list, function (i, usuarioContato) {
            if (usuarioContato.Codigo.val != _usuario.CodigoContato.val()) {
                listaAtualizada.push(usuarioContato);
            }
        });
        _usuario.GridUsuarioContatos.list = listaAtualizada;
        recarregarGridUsuarioContatos();
        LimparCamposUsuarioContatos();
    });
}

//*******MÉTODOS*******

function validarCamposObrigatoriosUsuarioContato() {
    var valido = true;
    if (_usuario.NomeContato.val() == "") {
        _usuario.NomeContato.requiredClass("form-control is-invalid");
        valido = false;
    }
    if (!string.IsNullOrWhiteSpace(_usuario.CPFContato.val()) && !ValidarCPF(_usuario.CPFContato.val())) {
        _usuario.CPFContato.requiredClass("form-control is-invalid");
        valido = false;
    }

    return valido;
}

function recarregarGridUsuarioContatos() {
    var data = new Array();
    $.each(_usuario.GridUsuarioContatos.list, function (i, usuario) {
        var usuarioContato = new Object();

        usuarioContato.Codigo = usuario.Codigo.val;
        usuarioContato.TipoParentesco = usuario.TipoParentesco.val;
        usuarioContato.Nome = usuario.Nome.val;
        usuarioContato.Telefone = usuario.Telefone.val;
        usuarioContato.Email = usuario.Email.val;
        usuarioContato.CPF = usuario.CPF.val;
        usuarioContato.DataNascimento = usuario.DataNascimento.val;
        usuarioContato.DescricaoParentesco = EnumTipoParentesco.obterDescricao(usuario.TipoParentesco.val);

        data.push(usuarioContato);
    });
    _gridUsuarioContatos.CarregarGrid(data);
}

function editarUsuarioContato(data) {
    LimparCamposUsuarioContatos();
    $.each(_usuario.GridUsuarioContatos.list, function (i, usuarioContato) {
        if (usuarioContato.Codigo.val == data.Codigo) {
            _usuario.CodigoContato.val(usuarioContato.Codigo.val);

            _usuario.TipoParentescoContato.val(usuarioContato.TipoParentesco.val);
            _usuario.NomeContato.val(usuarioContato.Nome.val);
            _usuario.TelefoneContato.val(usuarioContato.Telefone.val);
            _usuario.EmailContato.val(usuarioContato.Email.val);
            _usuario.CPFContato.val(usuarioContato.CPF.val);
            _usuario.DataNascimentoContato.val(usuarioContato.DataNascimento.val);

            return false;
        }
    });

    _usuario.AdicionarContato.visible(false);
    _usuario.AtualizarContato.visible(true);
    _usuario.ExcluirContato.visible(true);
    _usuario.CancelarContato.visible(true);
}

function LimparCamposUsuarioContatos() {
    _usuario.TipoParentescoContato.val(EnumTipoParentesco.Nenhum);
    LimparCampo(_usuario.NomeContato);
    _usuario.TelefoneContato.val("");
    _usuario.EmailContato.val("");
    LimparCampo(_usuario.CPFContato);
    LimparCampo(_usuario.DataNascimentoContato);

    _usuario.AdicionarContato.visible(true);
    _usuario.AtualizarContato.visible(false);
    _usuario.ExcluirContato.visible(false);
    _usuario.CancelarContato.visible(false);
}