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
/// <reference path="Motorista.js" />

var _gridMotoristaDadoBancarios;

//*******MAPEAMENTO KNOUCKOUT*******

var MotoristaDadoBancarioMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.CodigoBanco = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.TipoContaBanco = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Banco = PropertyEntity({ type: types.map, val: "" });
    this.Agencia = PropertyEntity({ type: types.map, val: "" });
    this.DigitoAgencia = PropertyEntity({ type: types.map, val: "" });
    this.NumeroConta = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoTipoContaBanco = PropertyEntity({ type: types.map, val: "" });
    this.ObservacaoConta = PropertyEntity({ type: types.map, val: "" });
    this.TipoChavePix = PropertyEntity({ type: types.map, val: "" });
    this.ChavePix = PropertyEntity({ type: types.map, val: "" });

}

//*******EVENTOS*******

function loadMotoristaDadoBancario() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarMotoristaDadoBancario, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoBanco", visible: false },
        { data: "TipoContaBanco", visible: false },
        { data: "Banco", title: Localization.Resources.Transportadores.Motorista.Banco, width: "30%" },
        { data: "Agencia", title: Localization.Resources.Transportadores.Motorista.Agencia, width: "15%" },
        { data: "DigitoAgencia", title: Localization.Resources.Transportadores.Motorista.DigitoAgencia, width: "10%" },
        { data: "NumeroConta", title: Localization.Resources.Transportadores.Motorista.NumeroConta, width: "15%" },
        { data: "DescricaoTipoContaBanco", title: Localization.Resources.Transportadores.Motorista.TipoContaBanco, width: "10%" },
        { data: "ChavePix", title: Localization.Resources.Transportadores.Motorista.ChavePix, width: "10%" },
        { data: "TipoChavePix", visible: false},
        { data: "ObservacaoConta", visible: false }
    ];

    _gridMotoristaDadoBancarios = new BasicDataTable(_motorista.GridMotoristaDadoBancarios.idGrid, header, menuOpcoes);
    recarregarGridMotoristaDadoBancarios();
}

function adicionarMotoristaDadoBancarioClick() {
    var tudoCerto = true;
    if (_motorista.BancoDadoBancario.val() == "")
        tudoCerto = false;
    if (_motorista.AgenciaDadoBancario.val() == "")
        tudoCerto = false;    
    if (_motorista.NumeroContaDadoBancario.val() == "")
        tudoCerto = false;

    if (tudoCerto) {
        var existe = false;
        if (!existe) {
            var motoristaDadoBancario = new MotoristaDadoBancarioMap();
            motoristaDadoBancario.Codigo.val = guid();
            motoristaDadoBancario.CodigoBanco.val = _motorista.BancoDadoBancario.codEntity();
            motoristaDadoBancario.DescricaoTipoContaBanco.val = $('#' + _motorista.TipoContaDadoBancario.id + ' option:selected').text();

            motoristaDadoBancario.Banco.val = _motorista.BancoDadoBancario.val();
            motoristaDadoBancario.Agencia.val = _motorista.AgenciaDadoBancario.val();
            motoristaDadoBancario.DigitoAgencia.val = _motorista.DigitoDadoBancario.val();
            motoristaDadoBancario.NumeroConta.val = _motorista.NumeroContaDadoBancario.val();
            motoristaDadoBancario.TipoContaBanco.val = _motorista.TipoContaDadoBancario.val();
            motoristaDadoBancario.ObservacaoConta.val = _motorista.ObservacaoContaDadoBancario.val();
            motoristaDadoBancario.TipoChavePix.val = _motorista.TipoChavePixDadoBancario.val();
            motoristaDadoBancario.ChavePix.val = _motorista.ChavePixDadoBancario.val();

            _motorista.GridMotoristaDadoBancarios.list.push(motoristaDadoBancario);
            recarregarGridMotoristaDadoBancarios();
            $("#" + _motorista.BancoDadoBancario.id).focus();
        }
        LimparCamposMotoristaDadoBancarios();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function atualizarMotoristaDadoBancarioClick() {
    var tudoCerto = true;
    if (_motorista.BancoDadoBancario.val() == "")
        tudoCerto = false;
    if (_motorista.AgenciaDadoBancario.val() == "")
        tudoCerto = false;    
    if (_motorista.NumeroContaDadoBancario.val() == "")
        tudoCerto = false;

    if (tudoCerto) {
        $.each(_motorista.GridMotoristaDadoBancarios.list, function (i, motoristaDadoBancario) {
            if (motoristaDadoBancario.Codigo.val == _motorista.CodigoDadoBancario.val()) {

                motoristaDadoBancario.CodigoBanco.val = _motorista.BancoDadoBancario.codEntity();
                motoristaDadoBancario.DescricaoTipoContaBanco.val = $('#' + _motorista.TipoContaDadoBancario.id + ' option:selected').text();

                motoristaDadoBancario.Banco.val = _motorista.BancoDadoBancario.val();
                motoristaDadoBancario.Agencia.val = _motorista.AgenciaDadoBancario.val();
                motoristaDadoBancario.DigitoAgencia.val = _motorista.DigitoDadoBancario.val();
                motoristaDadoBancario.NumeroConta.val = _motorista.NumeroContaDadoBancario.val();
                motoristaDadoBancario.TipoContaBanco.val = _motorista.TipoContaDadoBancario.val();
                motoristaDadoBancario.ObservacaoConta.val = _motorista.ObservacaoContaDadoBancario.val();
                motoristaDadoBancario.TipoChavePix.val = _motorista.TipoChavePixDadoBancario.val();
                motoristaDadoBancario.ChavePix.val = _motorista.ChavePixDadoBancario.val();
                return false;
            }
        });
        recarregarGridMotoristaDadoBancarios();
        LimparCamposMotoristaDadoBancarios();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function excluirMotoristaDadoBancarioClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Transportadores.Motorista.RealmenteDesejaExcluirDadoBancarioSelecionado, function () {
        var listaAtualizada = new Array();
        $.each(_motorista.GridMotoristaDadoBancarios.list, function (i, motoristaDadoBancario) {
            if (motoristaDadoBancario.Codigo.val != _motorista.CodigoDadoBancario.val()) {
                listaAtualizada.push(motoristaDadoBancario);
            }
        });
        _motorista.GridMotoristaDadoBancarios.list = listaAtualizada;
        recarregarGridMotoristaDadoBancarios();
        LimparCamposMotoristaDadoBancarios();
    });
}

//*******MÉTODOS*******

function recarregarGridMotoristaDadoBancarios() {
    var data = new Array();
    $.each(_motorista.GridMotoristaDadoBancarios.list, function (i, motorista) {
        var motoristaDadoBancario = new Object();
        console.log(motorista);
        motoristaDadoBancario.Codigo = motorista.Codigo.val;
        motoristaDadoBancario.CodigoBanco = motorista.CodigoBanco.val;
        motoristaDadoBancario.DescricaoTipoContaBanco = motorista.DescricaoTipoContaBanco.val;
        motoristaDadoBancario.Banco = motorista.Banco.val;
        motoristaDadoBancario.Agencia = motorista.Agencia.val;
        motoristaDadoBancario.DigitoAgencia = motorista.DigitoAgencia.val;
        motoristaDadoBancario.NumeroConta = motorista.NumeroConta.val;
        motoristaDadoBancario.TipoContaBanco = motorista.TipoContaBanco.val;
        motoristaDadoBancario.ObservacaoConta = motorista.ObservacaoConta.val;
        motoristaDadoBancario.ChavePix = motorista.ChavePix.val;
        motoristaDadoBancario.TipoChavePix = motorista.TipoChavePix.val;
        data.push(motoristaDadoBancario);
    });
    _gridMotoristaDadoBancarios.CarregarGrid(data);
}

function editarMotoristaDadoBancario(data) {
    LimparCamposMotoristaDadoBancarios();
    $.each(_motorista.GridMotoristaDadoBancarios.list, function (i, motoristaDadoBancario) {
        if (motoristaDadoBancario.Codigo.val == data.Codigo) {
            _motorista.CodigoDadoBancario.val(motoristaDadoBancario.Codigo.val);

            _motorista.BancoDadoBancario.codEntity(motoristaDadoBancario.CodigoBanco.val);
            _motorista.BancoDadoBancario.val(motoristaDadoBancario.Banco.val);
            _motorista.AgenciaDadoBancario.val(motoristaDadoBancario.Agencia.val);
            _motorista.DigitoDadoBancario.val(motoristaDadoBancario.DigitoAgencia.val);
            _motorista.NumeroContaDadoBancario.val(motoristaDadoBancario.NumeroConta.val);
            _motorista.TipoContaDadoBancario.val(motoristaDadoBancario.TipoContaBanco.val);
            _motorista.ObservacaoContaDadoBancario.val(motoristaDadoBancario.ObservacaoConta.val);
            _motorista.TipoChavePixDadoBancario.val(motoristaDadoBancario.TipoChavePix.val);
            _motorista.ChavePixDadoBancario.val(motoristaDadoBancario.ChavePix.val);
            return false;
        }
    });

    _motorista.AdicionarDadoBancario.visible(false);
    _motorista.AtualizarDadoBancario.visible(true);
    _motorista.ExcluirDadoBancario.visible(true);
    _motorista.CancelarDadoBancario.visible(true);
}

function LimparCamposMotoristaDadoBancarios() {
    LimparCampoEntity(_motorista.BancoDadoBancario);
    _motorista.AgenciaDadoBancario.val("");
    _motorista.DigitoDadoBancario.val("");
    _motorista.NumeroContaDadoBancario.val("");
    _motorista.ObservacaoContaDadoBancario.val("");
    _motorista.ChavePixDadoBancario.val("");
    _motorista.TipoContaDadoBancario.val(EnumTipoConta.Corrente);
    _motorista.TipoChavePixDadoBancario.val(EnumTipoChavePix.CPFCNPJ);

    _motorista.AdicionarDadoBancario.visible(true);
    _motorista.AtualizarDadoBancario.visible(false);
    _motorista.ExcluirDadoBancario.visible(false);
    _motorista.CancelarDadoBancario.visible(false);
}