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
/// <reference path="Usuario.js" />

var _gridUsuarioLicencas;

//*******MAPEAMENTO KNOUCKOUT*******

var UsuarioLicencaMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ type: types.map, val: "" });
    this.Numero = PropertyEntity({ type: types.map, val: "" });
    this.DataEmissao = PropertyEntity({ type: types.map, val: "" });
    this.DataVencimento = PropertyEntity({ type: types.map, val: "" });
    this.FormaAlerta = PropertyEntity({ type: types.map, val: "" });
    this.Status = PropertyEntity({ type: types.map, val: "" });
    this.CodigoLicenca = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoLicenca = PropertyEntity({ type: types.map, val: "" });
}

//*******EVENTOS*******

function loadUsuarioLicenca() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarUsuarioLicenca, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoLicenca", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "Numero", title: "Número", width: "15%", className: "text-align-left" },
        { data: "DataEmissao", title: "Data Emissão", width: "15%", className: "text-align-center" },
        { data: "DataVencimento", title: "Data Vencimento", width: "15%", className: "text-align-center" },
        { data: "DescricaoLicenca", title: "Licença", width: "20%", className: "text-align-left" },        
        { data: "FormaAlerta", visible: false },
        { data: "Status", visible: false },
    ];

    _gridUsuarioLicencas = new BasicDataTable(_usuario.GridUsuarioLicencas.idGrid, header, menuOpcoes);
    recarregarGridUsuarioLicencas();
}

function adicionarUsuarioLicencaClick() {
    var tudoCerto = true;
    if (_usuario.Descricao.val() == "")
        tudoCerto = false;
    if (_usuario.Numero.val() == "")
        tudoCerto = false;
    if (_usuario.DataEmissao.val() == "")
        tudoCerto = false;
    if (_usuario.DataVencimento.val() == "")
        tudoCerto = false;

    if (tudoCerto) {
        var existe = false;
        if (!existe) {
            var usuarioLicenca = new UsuarioLicencaMap();
            usuarioLicenca.Codigo.val = guid();
            usuarioLicenca.Descricao.val = _usuario.Descricao.val();
            usuarioLicenca.Numero.val = _usuario.Numero.val();
            usuarioLicenca.DataEmissao.val = _usuario.DataEmissao.val();
            usuarioLicenca.DataVencimento.val = _usuario.DataVencimento.val();
            usuarioLicenca.Status.val = _usuario.StatusLicenca.val();
            usuarioLicenca.FormaAlerta.val = JSON.stringify(_usuario.FormaAlerta.val());
            usuarioLicenca.DescricaoLicenca.val = _usuario.Licenca.val();
            usuarioLicenca.CodigoLicenca.val = _usuario.Licenca.codEntity();

            _usuario.GridUsuarioLicencas.list.push(usuarioLicenca);
            recarregarGridUsuarioLicencas();
            $("#" + _usuario.Descricao.id).focus();
        }
        LimparCamposUsuarioLicencas();
    } else {
        exibirMensagem("atencao", "Campos Obrigatórios", "Informe os campos obrigatórios");
    }
}

function atualizarUsuarioLicencaClick() {
    var tudoCerto = true;
    if (_usuario.Descricao.val() == "")
        tudoCerto = false;
    if (_usuario.Numero.val() == "")
        tudoCerto = false;
    if (_usuario.DataEmissao.val() == "")
        tudoCerto = false;
    if (_usuario.DataVencimento.val() == "")
        tudoCerto = false;

    if (tudoCerto) {
        $.each(_usuario.GridUsuarioLicencas.list, function (i, usuarioLicenca) {
            if (usuarioLicenca.Codigo.val == _usuario.CodigoLicenca.val()) {

                usuarioLicenca.Numero.val = _usuario.Numero.val();
                usuarioLicenca.Descricao.val = _usuario.Descricao.val();
                usuarioLicenca.DataEmissao.val = _usuario.DataEmissao.val();
                usuarioLicenca.DataVencimento.val = _usuario.DataVencimento.val();
                usuarioLicenca.Status.val = _usuario.StatusLicenca.val();
                usuarioLicenca.FormaAlerta.val = JSON.stringify(_usuario.FormaAlerta.val());
                usuarioLicenca.DescricaoLicenca.val = _usuario.Licenca.val();
                usuarioLicenca.CodigoLicenca.val = _usuario.Licenca.codEntity();

                return false;
            }
        });
        recarregarGridUsuarioLicencas();
        LimparCamposUsuarioLicencas();
    } else {
        exibirMensagem("atencao", "Campos Obrigatórios", "Informe os campos obrigatórios");
    }
}

function excluirUsuarioLicencaClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a licença selecionada?", function () {
        var listaAtualizada = new Array();
        $.each(_usuario.GridUsuarioLicencas.list, function (i, usuarioLicenca) {
            if (usuarioLicenca.Codigo.val != _usuario.CodigoLicenca.val()) {
                listaAtualizada.push(usuarioLicenca);
            }
        });
        _usuario.GridUsuarioLicencas.list = listaAtualizada;
        recarregarGridUsuarioLicencas();
        LimparCamposUsuarioLicencas();
    });
}

//*******MÉTODOS*******

function recarregarGridUsuarioLicencas() {
    var data = new Array();
    $.each(_usuario.GridUsuarioLicencas.list, function (i, usuario) {
        var usuarioLicenca = new Object();

        usuarioLicenca.Codigo = usuario.Codigo.val;
        usuarioLicenca.Descricao = usuario.Descricao.val;
        usuarioLicenca.Numero = usuario.Numero.val;
        usuarioLicenca.DataEmissao = usuario.DataEmissao.val;
        usuarioLicenca.DataVencimento = usuario.DataVencimento.val;
        usuarioLicenca.Status = usuario.Status.val;
        usuarioLicenca.FormaAlerta = usuario.FormaAlerta.val;
        usuarioLicenca.CodigoLicenca = usuario.CodigoLicenca.val;
        usuarioLicenca.DescricaoLicenca = usuario.DescricaoLicenca.val;

        data.push(usuarioLicenca);
    });
    _gridUsuarioLicencas.CarregarGrid(data);
}

function editarUsuarioLicenca(data) {
    LimparCamposUsuarioLicencas();
    $.each(_usuario.GridUsuarioLicencas.list, function (i, usuarioLicenca) {
        if (usuarioLicenca.Codigo.val == data.Codigo) {
            _usuario.CodigoLicenca.val(usuarioLicenca.Codigo.val);
            _usuario.Descricao.val(usuarioLicenca.Descricao.val);
            _usuario.Numero.val(usuarioLicenca.Numero.val);
            _usuario.DataEmissao.val(usuarioLicenca.DataEmissao.val);
            _usuario.DataVencimento.val(usuarioLicenca.DataVencimento.val);
            _usuario.StatusLicenca.val(usuarioLicenca.Status.val);
            //_usuario.FormaAlerta.val(JSON.parse(usuarioLicenca.FormaAlerta.val));
            _usuario.Licenca.val(usuarioLicenca.DescricaoLicenca.val);
            _usuario.Licenca.codEntity(usuarioLicenca.CodigoLicenca.codEntity);

            $("#" + _usuario.FormaAlerta.id).selectpicker('val', JSON.parse(usuarioLicenca.FormaAlerta.val));

            return false;
        }
    });

    _usuario.AdicionarLicenca.visible(false);
    _usuario.AtualizarLicenca.visible(true);
    _usuario.ExcluirLicenca.visible(true);
    _usuario.CancelarLicenca.visible(true);
}

function LimparCamposUsuarioLicencas() {
    _usuario.Descricao.val("");
    _usuario.Numero.val("");
    _usuario.DataEmissao.val("");
    _usuario.DataVencimento.val("");
    _usuario.StatusLicenca.val(EnumStatusLicenca.Vigente);
    //_usuario.FormaAlerta.val(new Array());
    LimparCampoEntity(_usuario.Licenca);

    $("#" + _usuario.FormaAlerta.id).selectpicker('val', []);
    
    _usuario.AdicionarLicenca.visible(true);
    _usuario.AtualizarLicenca.visible(false);
    _usuario.ExcluirLicenca.visible(false);
    _usuario.CancelarLicenca.visible(false);
}