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
/// <reference path="Pedido.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _contatos, _cadastroContato, _gridContato, _CRUDCadastroContato;    

var Contatos = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarContatoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });
};

var CadastroContatos = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.Contato = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Contato.getRequiredFieldDescription(), required: true, maxlength: 150, enable: ko.observable(true) });
    this.Telefone = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Telefone.getRequiredFieldDescription(), required: true, maxlength: 20, getType: typesKnockout.phone, enable: ko.observable(true) });
    this.Email = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Email.getRequiredFieldDescription(), required: true, maxlength: 500, getType: typesKnockout.email, enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription(), val: ko.observable(true), def: true, required: false, options: EnumSituacaoContato.obterOpcoes(), enable: ko.observable(true) });
    this.TipoContato = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), required: true, getType: typesKnockout.selectMultiple, text: Localization.Resources.Pedidos.Pedido.TipoContato.getRequiredFieldDescription(), options: ko.observable([]), url: "TipoContato/BuscarTodos", visible: ko.observable(true), enable: ko.observable(true) });
    this.CPF = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Cpf.getRequiredFieldDescription(), required: true, maxlength: 14, getType: typesKnockout.cpf });
}

var CRUDCadastroContato = function () {
    this.Cancelar = PropertyEntity({ eventClick: cancelarSuprimentoGasClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Confirmar = PropertyEntity({ eventClick: salvarContatoClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadContatosPedido() {
    _contatos = new Contatos();
    KoBindings(_contatos, "knockoutContatos");

    _cadastroContato = new CadastroContatos();
    KoBindings(_cadastroContato, "knockoutCadastroContato");

    _CRUDCadastroContato = new CRUDCadastroContato();
    KoBindings(_CRUDCadastroContato, "knockoutCRUDCadastroContato");

    CarregarGridContatos();
}

function CarregarGridContatos() {
    let menuOpcoes = {
        tipo: TypeOptionMenu.list, tamanho: 7,
        opcoes: [
            { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: EditarContatoClick },
            { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: ExcluirContatoClick }
        ]
    };

    const header = [
        { data: "Codigo", visible: false },
        { data: "TipoContato", visible: false },
        { data: "Situacao", visible: false },
        { data: "Contato", title: Localization.Resources.Pedidos.Pedido.Contato, width: "25%" },
        { data: "CPF", title: Localization.Resources.Pedidos.Pedido.Cpf, width: "10%" },
        { data: "DescricaoTipoContato", title: Localization.Resources.Pedidos.Pedido.TipoContato, width: "15%" },
        { data: "Email", title: Localization.Resources.Pedidos.Pedido.Email, width: "20%" },
        { data: "Telefone", title: Localization.Resources.Pedidos.Pedido.Telefone, width: "15%" },
        { data: "DescricaoSituacao", title: Localization.Resources.Gerais.Geral.Situacao, width: "15%" }
    ];

    _gridContato = new BasicDataTable(_contatos.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridContato.CarregarGrid([]);
}

function EditarContatoClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroContato, { Data: registroSelecionado });

    _cadastroContato.Codigo.val(registroSelecionado.Codigo);
    _cadastroContato.Contato.val(registroSelecionado.Contato);
    _cadastroContato.CPF.val(registroSelecionado.CPF);
    _cadastroContato.TipoContato.val(registroSelecionado.TipoContato);
    _cadastroContato.Telefone.val(registroSelecionado.Telefone);
    _cadastroContato.Email.val(registroSelecionado.Email);
    _cadastroContato.Situacao.val(registroSelecionado.Situacao);

    $("#" + _cadastroContato.TipoContato.id).change();

    Global.abrirModal("divModalCadastroContato");
}

function ExcluirContatoClick(registroSelecionado) {
    let lista = _gridContato.BuscarRegistros();

    for (let i = 0; i < lista.length; i++) {
        if (lista[i].Codigo == registroSelecionado.Codigo) {
            lista.splice(i, 1);
            break;
        }
    }

    _gridContato.CarregarGrid(lista);
}

function AdicionarContatoClick() {
    Global.abrirModal("divModalCadastroContato");
    LimparCampos(_cadastroContato);
}

//*******MÉTODOS*******

function preencherContatos(dadosContatos) {
    _gridContato.CarregarGrid(dadosContatos);
}

function obterLisaContatos() {
    return JSON.stringify(_gridContato.BuscarRegistros());
}

function salvarContatoClick() {
    if (!ValidarContato())
        return;

    let registros = _gridContato.BuscarRegistros();

    let elementoGrid = {
        Codigo: !string.IsNullOrWhiteSpace((_cadastroContato.Codigo.val()).toString()) ? _cadastroContato.Codigo.val() : guid(),
        TipoContato: _cadastroContato.TipoContato.val(),
        Contato: _cadastroContato.Contato.val(),
        CPF: _cadastroContato.CPF.val(),
        DescricaoTipoContato: ObterDescricaoTipoContato(_cadastroContato.TipoContato.val()),
        Email: _cadastroContato.Email.val(),
        Telefone: _cadastroContato.Telefone.val(),
        Situacao: _cadastroContato.Situacao.val(),
        DescricaoSituacao: _cadastroContato.Situacao.val() ? Localization.Resources.Gerais.Geral.Ativo : Localization.Resources.Gerais.Geral.Inativo
    };

    let atualizando = false;

    for (let i = 0; i < registros.length; i++) {
        if (registros[i].Codigo == elementoGrid.Codigo) {
            atualizando = true;
            registros.splice(i, 1, elementoGrid);
            break;
        }
    }

    if (!atualizando)
        registros.push(elementoGrid);

    _gridContato.CarregarGrid(registros);

    Global.fecharModal("divModalCadastroContato");
    limparCamposCadastroContato();
}

function cancelarSuprimentoGasClick() {
    Global.fecharModal("divModalCadastroContato");
    limparCamposCadastroContato();
}

function ObterDescricaoTipoContato(tipoContato) {
    let descricaoTipoContato = "";

    for (let i = 0; i < tipoContato.length; i++)
        descricaoTipoContato += $("#" + _cadastroContato.TipoContato.id + " option[value='" + tipoContato[i] + "']").text() + ", ";

    if (descricaoTipoContato.length > 0)
        descricaoTipoContato = descricaoTipoContato.substring(0, descricaoTipoContato.length - 2);

    return descricaoTipoContato;
}

function ValidarContato() {
    if (ValidarCamposObrigatorios(_cadastroContato) === false) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return false;
    }

    let cpfCnpj = _cadastroContato.CPF.val().replace(/[^0-9]/g, '');
    if (cpfCnpj.length == 11) {
        if (!ValidarCPF(cpfCnpj)) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.CPFInformadoInvalido);
            $("#" + CPF.id).focus();
            return false;
        }
    }

    if (_cadastroContato.TipoContato.val().length <= 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pedidos.Pedido.PorFavorSelecioneUmTipoDeContato);
        return false;
    }

    return true;
}