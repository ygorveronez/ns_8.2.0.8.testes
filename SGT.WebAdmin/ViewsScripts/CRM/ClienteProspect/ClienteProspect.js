/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumTipoContatoAtendimento.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _clienteProspect;
var _pesquisaClienteProspect;
var _gridClienteProspect;

var _tipoContato = [
    { text: "Telefone", value: EnumTipoContatoAtendimento.Telefone },
    { text: "Email", value: EnumTipoContatoAtendimento.Email },
    { text: "Skype", value: EnumTipoContatoAtendimento.Skype },
    { text: "Chat Web", value: EnumTipoContatoAtendimento.ChatWeb }
];

var ClienteProspect = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Nome = PropertyEntity({ text: "*Nome: ", required: true, val: ko.observable(""), def: "", getType: typesKnockout.string, enable: ko.observable(true) });
    this.CNPJ = PropertyEntity({ text: "CNPJ: ", val: ko.observable(""), def: "", getType: typesKnockout.cnpj, enable: ko.observable(true), required: false });
    this.Contato = PropertyEntity({ text: "*Contato: ", required: true, val: ko.observable(""), def: "", getType: typesKnockout.string, enable: ko.observable(true) });
    this.Email = PropertyEntity({ text: "*Email: ", required: true, val: ko.observable(""), def: "", getType: typesKnockout.email, enable: ko.observable(true) });
    this.Telefone = PropertyEntity({ text: "*Telefone: ", required: true, val: ko.observable(""), def: "", getType: typesKnockout.phone, enable: ko.observable(true) });
    this.Cidade = PropertyEntity({ text: "Cidade: ", val: ko.observable(""), codEntity: ko.observable(0), idBtnSearch: guid(), type: types.entity, enable: ko.observable(true) });
    this.TipoContato = PropertyEntity({ text: "Tipo do Contato: ", val: ko.observable(0), def: EnumTipoContatoAtendimento.Telefone, options: _tipoContato, enable: ko.observable(true) });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaClienteProspect = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: false, getType: typesKnockout.string, val: ko.observable("") });    

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridClienteProspect.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadClienteProspect() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaClienteProspect = new PesquisaClienteProspect();
    KoBindings(_pesquisaClienteProspect, "knockoutPesquisaClienteProspect", false, _pesquisaClienteProspect.Pesquisar.id);

    // Instancia objeto principal
    _clienteProspect = new ClienteProspect();
    KoBindings(_clienteProspect, "knockoutClienteProspect");

    new BuscarLocalidades(_clienteProspect.Cidade);

    HeaderAuditoria("ClienteProspect", _clienteProspect);

    // Inicia busca
    buscarClienteProspect();
}

function adicionarClick(e, sender) {
    Salvar(_clienteProspect, "ClienteProspect/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridClienteProspect.CarregarGrid();
                limparCamposClienteProspect();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_clienteProspect, "ClienteProspect/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridClienteProspect.CarregarGrid();
                limparCamposClienteProspect();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_clienteProspect, "ClienteProspect/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridClienteProspect.CarregarGrid();
                    limparCamposClienteProspect();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCamposClienteProspect();
}

function editarClienteProspectClick(itemGrid) {
    // Limpa os campos
    limparCamposClienteProspect();

    // Seta o codigo do objeto
    _clienteProspect.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_clienteProspect, "ClienteProspect/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaClienteProspect.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _clienteProspect.Atualizar.visible(true);
                _clienteProspect.Excluir.visible(true);
                _clienteProspect.Cancelar.visible(true);
                _clienteProspect.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarClienteProspect() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClienteProspectClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridClienteProspect = new GridView(_pesquisaClienteProspect.Pesquisar.idGrid, "ClienteProspect/Pesquisa", _pesquisaClienteProspect, menuOpcoes, null);
    _gridClienteProspect.CarregarGrid();
}

function limparCamposClienteProspect() {
    _clienteProspect.Atualizar.visible(false);
    _clienteProspect.Cancelar.visible(false);
    _clienteProspect.Excluir.visible(false);
    _clienteProspect.Adicionar.visible(true);
    LimparCampos(_clienteProspect);
}