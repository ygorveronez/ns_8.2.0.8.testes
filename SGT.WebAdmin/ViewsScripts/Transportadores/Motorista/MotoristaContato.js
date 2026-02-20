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
/// <reference path="Motorista.js" />

var _gridMotoristaContatos;

//*******MAPEAMENTO KNOUCKOUT*******

var MotoristaContatoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Nome = PropertyEntity({ type: types.map, val: "" });
    this.Telefone = PropertyEntity({ type: types.map, val: "" });
    this.Email = PropertyEntity({ type: types.map, val: "" });
    this.TipoParentesco = PropertyEntity({ type: types.map, val: "" });
    this.CPF = PropertyEntity({ type: types.map, val: "" });
    this.DataNascimento = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoParentesco = PropertyEntity({ type: types.map, val: " " });
};

//*******EVENTOS*******

function loadMotoristaContato() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarMotoristaContato, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "TipoParentesco", visible: false },
        { data: "Nome", title: Localization.Resources.Gerais.Geral.Nome, width: "30%" },
        { data: "CPF", title: Localization.Resources.Transportadores.Motorista.CPF, width: "10%" },
        { data: "DataNascimento", title: Localization.Resources.Transportadores.Motorista.DataNascimento, width: "10%" },
        { data: "Telefone", title: Localization.Resources.Transportadores.Motorista.Telefone, width: "10%" },
        { data: "Email", title: Localization.Resources.Transportadores.Motorista.Email, width: "20%" },
        { data: "DescricaoParentesco", title: Localization.Resources.Transportadores.Motorista.GrauParentesco, width: "20%" }
    ];

    _gridMotoristaContatos = new BasicDataTable(_motorista.GridMotoristaContatos.idGrid, header, menuOpcoes);
    recarregarGridMotoristaContatos();
}

function adicionarMotoristaContatoClick() {
    if (validarCamposObrigatoriosMotoristaContato()) {
        var motoristaContato = new MotoristaContatoMap();
        motoristaContato.Codigo.val = guid();

        motoristaContato.TipoParentesco.val = _motorista.TipoParentescoContato.val();
        motoristaContato.Nome.val = _motorista.NomeContato.val();
        motoristaContato.Telefone.val = _motorista.TelefoneContato.val();
        motoristaContato.Email.val = _motorista.EmailContato.val();
        motoristaContato.CPF.val = _motorista.CPFContato.val();
        motoristaContato.DataNascimento.val = _motorista.DataNascimentoContato.val();

        _motorista.GridMotoristaContatos.list.push(motoristaContato);
        recarregarGridMotoristaContatos();
        $("#" + _motorista.NomeContato.id).focus();

        LimparCamposMotoristaContatos();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function atualizarMotoristaContatoClick() {
    if (validarCamposObrigatoriosMotoristaContato()) {
        $.each(_motorista.GridMotoristaContatos.list, function (i, motoristaContato) {
            if (motoristaContato.Codigo.val == _motorista.CodigoContato.val()) {

                motoristaContato.TipoParentesco.val = _motorista.TipoParentescoContato.val();
                motoristaContato.Nome.val = _motorista.NomeContato.val();
                motoristaContato.Telefone.val = _motorista.TelefoneContato.val();
                motoristaContato.Email.val = _motorista.EmailContato.val();
                motoristaContato.CPF.val = _motorista.CPFContato.val();
                motoristaContato.DataNascimento.val = _motorista.DataNascimentoContato.val();

                return false;
            }
        });
        recarregarGridMotoristaContatos();
        LimparCamposMotoristaContatos();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function excluirMotoristaContatoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Transportadores.Motorista.RealmenteDesejaExcluirContatoSelecionado, function () {
        var listaAtualizada = new Array();
        $.each(_motorista.GridMotoristaContatos.list, function (i, motoristaContato) {
            if (motoristaContato.Codigo.val != _motorista.CodigoContato.val()) {
                listaAtualizada.push(motoristaContato);
            }
        });
        _motorista.GridMotoristaContatos.list = listaAtualizada;
        recarregarGridMotoristaContatos();
        LimparCamposMotoristaContatos();
    });
}

//*******MÉTODOS*******

function validarCamposObrigatoriosMotoristaContato() {
    var valido = true;
    if (_motorista.NomeContato.val() == "") {
        _motorista.NomeContato.requiredClass("form-control is-invalid ");
        valido = false;
    }
    if (!string.IsNullOrWhiteSpace(_motorista.CPFContato.val()) && !ValidarCPF(_motorista.CPFContato.val())) {
        _motorista.CPFContato.requiredClass("form-control ");
        valido = false;
    }

    return valido;
}

function recarregarGridMotoristaContatos() {
    var data = new Array();
    $.each(_motorista.GridMotoristaContatos.list, function (i, motorista) {
        var motoristaContato = new Object();

        motoristaContato.Codigo = motorista.Codigo.val;
        motoristaContato.TipoParentesco = motorista.TipoParentesco.val;
        motoristaContato.Nome = motorista.Nome.val;
        motoristaContato.Telefone = motorista.Telefone.val;
        motoristaContato.Email = motorista.Email.val;
        motoristaContato.CPF = motorista.CPF.val;
        motoristaContato.DataNascimento = motorista.DataNascimento.val;
        motoristaContato.DescricaoParentesco = EnumTipoParentesco.obterDescricao(motorista.TipoParentesco.val);

        data.push(motoristaContato);
    });
    _gridMotoristaContatos.CarregarGrid(data);
}

function editarMotoristaContato(data) {
    LimparCamposMotoristaContatos();
    $.each(_motorista.GridMotoristaContatos.list, function (i, motoristaContato) {
        if (motoristaContato.Codigo.val == data.Codigo) {
            _motorista.CodigoContato.val(motoristaContato.Codigo.val);

            _motorista.TipoParentescoContato.val(motoristaContato.TipoParentesco.val);
            _motorista.NomeContato.val(motoristaContato.Nome.val);
            _motorista.TelefoneContato.val(motoristaContato.Telefone.val);
            _motorista.EmailContato.val(motoristaContato.Email.val);
            _motorista.CPFContato.val(motoristaContato.CPF.val);
            _motorista.DataNascimentoContato.val(motoristaContato.DataNascimento.val);

            return false;
        }
    });

    _motorista.AdicionarContato.visible(false);
    _motorista.AtualizarContato.visible(true);
    _motorista.ExcluirContato.visible(true);
    _motorista.CancelarContato.visible(true);
}

function LimparCamposMotoristaContatos() {
    _motorista.TipoParentescoContato.val(EnumTipoParentesco.Nenhum);
    LimparCampo(_motorista.NomeContato);
    _motorista.TelefoneContato.val("");
    _motorista.EmailContato.val("");
    LimparCampo(_motorista.CPFContato);
    LimparCampo(_motorista.DataNascimentoContato);

    _motorista.AdicionarContato.visible(true);
    _motorista.AtualizarContato.visible(false);
    _motorista.ExcluirContato.visible(false);
    _motorista.CancelarContato.visible(false);
}