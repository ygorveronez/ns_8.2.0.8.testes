/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="Veiculo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridMotoristas;

var MotoristaMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.CodigoMotorista = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Nome = PropertyEntity({ type: types.map, val: "" });
    this.CPF = PropertyEntity({ type: types.map, val: "" });
};

//*******EVENTOS*******

function loadMotorista() {
    //Motorista Principal
    new BuscarMotoristasPorCPF(_veiculo.CPFMotorista, retornoCPFMotorista, _veiculo.Empresa);
    //Motoristas Secundários
    new BuscarMotoristasPorCPF(_veiculo.CPFMotoristaSecundario, retornoCPFMotoristaSecundario, _veiculo.Empresa);

    loadGridMotorista();
}

function retornoCPFMotorista(data) {
    if (data != null) {
        _veiculo.CodigoMotorista.val(data.Codigo);
        _veiculo.NomeMotorista.val(data.Nome);
        _veiculo.CPFMotorista.val(data.CPF);
        _veiculo.CPFMotorista.codEntity(999);
        _veiculo.CPF.val(_veiculo.CPFMotorista.val());
    }
}

function retornoCPFMotoristaSecundario(data) {
    if (data != null) {
        _veiculo.CodigoMotoristaSecundario.val(data.Codigo);
        _veiculo.NomeMotoristaSecundario.val(data.Nome);
        _veiculo.CPFMotoristaSecundario.val(data.CPF);
        _veiculo.CPFMotoristaSecundario.codEntity(999);
        _veiculo.CPFSecundario.val(_veiculo.CPFMotoristaSecundario.val());
    }
}

function adicionarMotoristaClick() {
    _veiculo.CPFMotoristaSecundario.required(true);
    _veiculo.NomeMotoristaSecundario.required(true);

    var validaCPF = ValidarCampoObrigatorioEntity(_veiculo.CPFMotoristaSecundario);
    var validaNome = ValidarCampoObrigatorioMap(_veiculo.NomeMotoristaSecundario);
    var tudoCerto = validaCPF && validaNome;

    _veiculo.CPFMotoristaSecundario.required(false);
    _veiculo.NomeMotoristaSecundario.required(false);

    if (!tudoCerto) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    if (!possuiMotoristaPrincipal()) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Veiculos.Veiculo.MotoristaPrincipal, Localization.Resources.Veiculos.Veiculo.MotoristaPrincipalDeveSerInformadoAntes);
        return;
    }

    if (possuiMotoristaPrincipal() && _veiculo.CPFMotoristaSecundario.val() == _veiculo.CPF.val()) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Veiculos.Veiculo.MotoristaJaInformado, Localization.Resources.Veiculos.Veiculo.MotoristaMesmoDoPrincipal.format(_veiculo.NomeMotoristaSecundario.val()));
        return;
    }

    var existe = false;
    $.each(_veiculo.Motoristas.list, function (i, motorista) {
        if (motorista.CPF.val == _veiculo.CPFMotoristaSecundario.val()) {
            existe = true;
            return false;
        }
    });
    if (existe) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Veiculos.Veiculo.MotoristaJaInformado, Localization.Resources.Veiculos.Veiculo.MotoristaJaFoiInformadoParaEsteVeiculo.format(_veiculo.NomeMotoristaSecundario.val()));
        return;
    }

    var motorista = new MotoristaMap();
    motorista.Codigo.val = guid();
    motorista.CodigoMotorista.val = _veiculo.CodigoMotoristaSecundario.val();
    motorista.Nome.val = _veiculo.NomeMotoristaSecundario.val();
    motorista.CPF.val = _veiculo.CPFMotoristaSecundario.val();

    _veiculo.Motoristas.list.push(motorista);
    recarregarGridMotoristas();

    LimparCampoEntity(_veiculo.CPFMotoristaSecundario);
    LimparCampo(_veiculo.NomeMotoristaSecundario);
    _veiculo.CPFSecundario.val("");
    _veiculo.CodigoMotoristaSecundario.val(0);
}

function excluirMotoristaClick(data) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Veiculos.Veiculo.RealmenteDesejaExcluirMotorista.format(data.Nome), function () {
        var listaAtualizada = new Array();
        $.each(_veiculo.Motoristas.list, function (i, motorista) {
            if (motorista.Codigo.val != data.Codigo) {
                listaAtualizada.push(motorista);
            }
        });
        _veiculo.Motoristas.list = listaAtualizada;
        recarregarGridMotoristas();
    });
}

//*******MÉTODOS*******

function CodigoMotoristaVeiculoModificado(valor, knoutVeiculo) {
    if (valor == "") {
        knoutVeiculo.CPF.val("");
        knoutVeiculo.CodigoMotorista.val(0);
        knoutVeiculo.CPFMotorista.codEntity(0);
    }
}

function CodigoMotoristaSecundarioVeiculoModificado(valor, knoutVeiculo) {
    if (valor == "") {
        knoutVeiculo.CPFSecundario.val("");
        knoutVeiculo.CodigoMotoristaSecundario.val(0);
        knoutVeiculo.CPFMotoristaSecundario.codEntity(0);
    }
}

function loadGridMotorista() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: excluirMotoristaClick, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "CPF", title: Localization.Resources.Veiculos.Veiculo.CPF, width: "30%" },
        { data: "Nome", title: Localization.Resources.Gerais.Geral.Nome, width: "70%" }
    ];
    _gridMotoristas = new BasicDataTable(_veiculo.Motoristas.idGrid, header, menuOpcoes);
    recarregarGridMotoristas();
}

function recarregarGridMotoristas() {
    var data = new Array();
    $.each(_veiculo.Motoristas.list, function (i, motorista) {
        var obj = new Object();
        obj.Codigo = motorista.Codigo.val;
        obj.CPF = motorista.CPF.val;
        obj.Nome = motorista.Nome.val;
        data.push(obj);
    });
    _gridMotoristas.CarregarGrid(data);
}

function possuiMotoristaPrincipal() {
    return Boolean(_veiculo.CPF.val());
}

function possuiMotoristaSecundario() {
    return _veiculo.Motoristas.list.length > 0;
}