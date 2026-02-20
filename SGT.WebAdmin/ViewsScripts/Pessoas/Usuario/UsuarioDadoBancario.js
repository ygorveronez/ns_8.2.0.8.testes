/// <reference path="Usuario.js" />
/// <reference path="../../Enumeradores/EnumTipoConta.js" />

var _gridUsuarioDadoBancarios;

//*******MAPEAMENTO KNOUCKOUT*******

var UsuarioDadoBancarioMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.CodigoBanco = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.TipoContaBanco = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Banco = PropertyEntity({ type: types.map, val: "" });
    this.Agencia = PropertyEntity({ type: types.map, val: "" });
    this.DigitoAgencia = PropertyEntity({ type: types.map, val: "" });
    this.NumeroConta = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoTipoContaBanco = PropertyEntity({ type: types.map, val: "" });
    this.ObservacaoConta = PropertyEntity({ type: types.map, val: "" });
};

//*******EVENTOS*******

function loadUsuarioDadoBancario() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarUsuarioDadoBancario, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoBanco", visible: false },
        { data: "TipoContaBanco", visible: false },
        { data: "Banco", title: Localization.Resources.Pessoas.Usuario.Banco, width: "30%" },
        { data: "Agencia", title: Localization.Resources.Pessoas.Usuario.Agencia, width: "15%" },
        { data: "DigitoAgencia", title: Localization.Resources.Pessoas.Usuario.DigitoAgencia, width: "10%" },
        { data: "NumeroConta", title: Localization.Resources.Pessoas.Usuario.NumeroConta, width: "15%" },
        { data: "DescricaoTipoContaBanco", title: Localization.Resources.Pessoas.Usuario.TipoContaBanco, width: "10%" },
        { data: "ObservacaoConta", visible: false }
    ];

    _gridUsuarioDadoBancarios = new BasicDataTable(_usuario.GridUsuarioDadoBancarios.idGrid, header, menuOpcoes);
    recarregarGridUsuarioDadoBancarios();
}

function adicionarUsuarioDadoBancarioClick() {
    var tudoCerto = true;
    if (_usuario.BancoDadoBancario.val() == "")
        tudoCerto = false;
    if (_usuario.AgenciaDadoBancario.val() == "")
        tudoCerto = false;
    if (_usuario.DigitoDadoBancario.val() == "")
        tudoCerto = false;
    if (_usuario.NumeroContaDadoBancario.val() == "")
        tudoCerto = false;

    if (tudoCerto) {
        var existe = false;
        if (!existe) {
            var usuarioDadoBancario = new UsuarioDadoBancarioMap();
            usuarioDadoBancario.Codigo.val = guid();
            usuarioDadoBancario.CodigoBanco.val = _usuario.BancoDadoBancario.codEntity();
            usuarioDadoBancario.DescricaoTipoContaBanco.val = $('#' + _usuario.TipoContaDadoBancario.id + ' option:selected').text();

            usuarioDadoBancario.Banco.val = _usuario.BancoDadoBancario.val();
            usuarioDadoBancario.Agencia.val = _usuario.AgenciaDadoBancario.val();
            usuarioDadoBancario.DigitoAgencia.val = _usuario.DigitoDadoBancario.val();
            usuarioDadoBancario.NumeroConta.val = _usuario.NumeroContaDadoBancario.val();
            usuarioDadoBancario.TipoContaBanco.val = _usuario.TipoContaDadoBancario.val();
            usuarioDadoBancario.ObservacaoConta.val = _usuario.ObservacaoContaDadoBancario.val();

            _usuario.GridUsuarioDadoBancarios.list.push(usuarioDadoBancario);
            recarregarGridUsuarioDadoBancarios();
            $("#" + _usuario.BancoDadoBancario.id).focus();
        }
        LimparCamposUsuarioDadoBancarios();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function atualizarUsuarioDadoBancarioClick() {
    var tudoCerto = true;
    if (_usuario.BancoDadoBancario.val() == "")
        tudoCerto = false;
    if (_usuario.AgenciaDadoBancario.val() == "")
        tudoCerto = false;
    if (_usuario.DigitoDadoBancario.val() == "")
        tudoCerto = false;
    if (_usuario.NumeroContaDadoBancario.val() == "")
        tudoCerto = false;

    if (tudoCerto) {
        $.each(_usuario.GridUsuarioDadoBancarios.list, function (i, usuarioDadoBancario) {
            if (usuarioDadoBancario.Codigo.val == _usuario.CodigoDadoBancario.val()) {

                usuarioDadoBancario.CodigoBanco.val = _usuario.BancoDadoBancario.codEntity();
                usuarioDadoBancario.DescricaoTipoContaBanco.val = $('#' + _usuario.TipoContaDadoBancario.id + ' option:selected').text();

                usuarioDadoBancario.Banco.val = _usuario.BancoDadoBancario.val();
                usuarioDadoBancario.Agencia.val = _usuario.AgenciaDadoBancario.val();
                usuarioDadoBancario.DigitoAgencia.val = _usuario.DigitoDadoBancario.val();
                usuarioDadoBancario.NumeroConta.val = _usuario.NumeroContaDadoBancario.val();
                usuarioDadoBancario.TipoContaBanco.val = _usuario.TipoContaDadoBancario.val();
                usuarioDadoBancario.ObservacaoConta.val = _usuario.ObservacaoContaDadoBancario.val();

                return false;
            }
        });
        recarregarGridUsuarioDadoBancarios();
        LimparCamposUsuarioDadoBancarios();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function excluirUsuarioDadoBancarioClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pessoas.Usuario.RealmenteDesejaExcluirDadoBancarioSelecionado, function () {
        var listaAtualizada = new Array();
        $.each(_usuario.GridUsuarioDadoBancarios.list, function (i, usuarioDadoBancario) {
            if (usuarioDadoBancario.Codigo.val != _usuario.CodigoDadoBancario.val()) {
                listaAtualizada.push(usuarioDadoBancario);
            }
        });
        _usuario.GridUsuarioDadoBancarios.list = listaAtualizada;
        recarregarGridUsuarioDadoBancarios();
        LimparCamposUsuarioDadoBancarios();
    });
}

//*******MÉTODOS*******

function recarregarGridUsuarioDadoBancarios() {
    var data = new Array();
    $.each(_usuario.GridUsuarioDadoBancarios.list, function (i, usuario) {
        var usuarioDadoBancario = new Object();

        usuarioDadoBancario.Codigo = usuario.Codigo.val;
        usuarioDadoBancario.CodigoBanco = usuario.CodigoBanco.val;
        usuarioDadoBancario.DescricaoTipoContaBanco = usuario.DescricaoTipoContaBanco.val;
        usuarioDadoBancario.Banco = usuario.Banco.val;
        usuarioDadoBancario.Agencia = usuario.Agencia.val;
        usuarioDadoBancario.DigitoAgencia = usuario.DigitoAgencia.val;
        usuarioDadoBancario.NumeroConta = usuario.NumeroConta.val;
        usuarioDadoBancario.TipoContaBanco = usuario.TipoContaBanco.val;
        usuarioDadoBancario.ObservacaoConta = usuario.ObservacaoConta.val;

        data.push(usuarioDadoBancario);
    });
    _gridUsuarioDadoBancarios.CarregarGrid(data);
}

function editarUsuarioDadoBancario(data) {
    LimparCamposUsuarioDadoBancarios();
    $.each(_usuario.GridUsuarioDadoBancarios.list, function (i, usuarioDadoBancario) {
        if (usuarioDadoBancario.Codigo.val == data.Codigo) {
            _usuario.CodigoDadoBancario.val(usuarioDadoBancario.Codigo.val);

            _usuario.BancoDadoBancario.codEntity(usuarioDadoBancario.CodigoBanco.val);
            _usuario.BancoDadoBancario.val(usuarioDadoBancario.Banco.val);
            _usuario.AgenciaDadoBancario.val(usuarioDadoBancario.Agencia.val);
            _usuario.DigitoDadoBancario.val(usuarioDadoBancario.DigitoAgencia.val);
            _usuario.NumeroContaDadoBancario.val(usuarioDadoBancario.NumeroConta.val);
            _usuario.TipoContaDadoBancario.val(usuarioDadoBancario.TipoContaBanco.val);
            _usuario.ObservacaoContaDadoBancario.val(usuarioDadoBancario.ObservacaoConta.val);

            return false;
        }
    });

    _usuario.AdicionarDadoBancario.visible(false);
    _usuario.AtualizarDadoBancario.visible(true);
    _usuario.ExcluirDadoBancario.visible(true);
    _usuario.CancelarDadoBancario.visible(true);
}

function LimparCamposUsuarioDadoBancarios() {
    LimparCampoEntity(_usuario.BancoDadoBancario);
    _usuario.AgenciaDadoBancario.val("");
    _usuario.DigitoDadoBancario.val("");
    _usuario.NumeroContaDadoBancario.val("");
    _usuario.ObservacaoContaDadoBancario.val("");
    _usuario.TipoContaDadoBancario.val(EnumTipoConta.Corrente);

    _usuario.AdicionarDadoBancario.visible(true);
    _usuario.AtualizarDadoBancario.visible(false);
    _usuario.ExcluirDadoBancario.visible(false);
    _usuario.CancelarDadoBancario.visible(false);
}