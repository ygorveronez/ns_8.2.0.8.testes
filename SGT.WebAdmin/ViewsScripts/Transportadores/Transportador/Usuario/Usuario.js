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
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../Transportador.js" />
/// <reference path="SerieUsuario.js" />
/// <reference path="PermissaoUsuario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridUsuario;
var _usuario;
var _pesquisaUsuario;
var _crudUsuario;
var _politicaSenha;

var _statusChar = [{ text: Localization.Resources.Gerais.Geral.Ativo, value: "A" },
    { text: Localization.Resources.Gerais.Geral.Inativo, value: "I" }];

var _statusPesquisaChar = [{ text: Localization.Resources.Gerais.Geral.Todos, value: "" },
    { text: "Ativo", value: "A" },
    { text: "Inativo", value: "I" }];

var _usuarioAcessoSistema = [
    { text: "Liberado", value: false },
    { text: "Bloqueado", value: true }
];

var PesquisaUsuario = function (codigoTransportador) {
    this.CodigoTransportador = PropertyEntity({ val: ko.observable(codigoTransportador), type: types.map });
    this.Nome = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Nome.getFieldDescription() });
    this.CPFCNPJ = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CPFCNPJ.getFieldDescription(), getType: typesKnockout.cpfCnpj });
    this.Status = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Status.getFieldDescription(), val: ko.observable(""), options: _statusPesquisaChar, def: "" });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridUsuario.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Transportadores.Transportador.FiltrosPesquisa.getFieldDescription(), idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


var Usuario = function (codigoTransportador) {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoTransportador = PropertyEntity({ val: ko.observable(codigoTransportador), def: codigoTransportador, type: types.map });
    this.Nome = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Nome.getRequiredFieldDescription(), required: true });
    this.CPFCNPJ = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CNPJCPF.getRequiredFieldDescription(), required: true, getType: typesKnockout.cpfCnpj });
    this.RGIE = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.RgIe.getFieldDescription(), maxlength: 20 });
    this.DataNascimento = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.DataNascimento.getFieldDescription(), getType: typesKnockout.date });
    this.DataAdmissao = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.DataAdmissao.getFieldDescription(), getType: typesKnockout.date });
    this.Salario = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Salario.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 9 });
    this.Telefone = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Telefone.getFieldDescription(), getType: typesKnockout.phone });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Transportadores.Transportador.Cidade.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.Endereco = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Endereco.getFieldDescription() });
    this.Complemento = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Complemento.getRequiredFieldDescription() });
    this.Email = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Email.getRequiredFieldDescription(), required: true, getType: typesKnockout.email });
    this.Usuario = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Usuario.getRequiredFieldDescription(), required: true, maxlength: 20, visible: ko.observable(true), cssClass: ko.observable("col col-4"), cssGroupClass: ko.observable("") });
    this.Senha = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Senha.getRequiredFieldDescription(), required: false, maxlength: 15, visible: ko.observable(true) });
    this.ConfirmacaoSenha = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.ConfirmacaoSenha.getRequiredFieldDescription(), required: false, maxlength: 15, visible: ko.observable(true) });
    this.Status = PropertyEntity({ val: ko.observable("A"), options: _statusChar, def: "A", text: Localization.Resources.Transportadores.Transportador.Status.getRequiredFieldDescription(), required: true });
    this.PermiteAssinarAnuencia = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.PermiteAssinarAnuencia, visible: ko.observable(false) });

    this.RedefinirSenha = PropertyEntity({ eventClick: redefinirSenhaClick, type: types.event, text: Localization.Resources.Transportadores.Transportador.RedefinirSenha, visible: ko.observable(false) });

    this.UsuarioAcessoBloqueado = PropertyEntity({ val: ko.observable(false), options: _usuarioAcessoSistema, def: false, text: Localization.Resources.Transportadores.Transportador.AcessoSistema.getFieldDescription(), issue: 661, visible: ko.observable(true) });
    this.Series = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Permissoes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
}

var CRUDUsuario = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarUsuarioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarUsuarioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarUsuarioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.ResetarSenha = PropertyEntity({ eventClick: ResetarSenhaUsuarioClick, type: types.event, text: Localization.Resources.Transportadores.Transportador.ResetarSenha, visible: ko.observable(false) });
}

//*******EVENTOS*******


async function loadUsuario() {

    _usuario = new Usuario(_transportador.Codigo.val());
    KoBindings(_usuario, "knockoutCadastroUsuario");

    _pesquisaUsuario = new PesquisaUsuario(_transportador.Codigo.val());
    KoBindings(_pesquisaUsuario, "knockoutPesquisaUsuario", false, _pesquisaUsuario.Pesquisar.id);

    _crudUsuario = new CRUDUsuario()
    KoBindings(_crudUsuario, "knockoutCRUDUsuario");

    new BuscarLocalidades(_usuario.Localidade);

    loadPermissaoUsuario();
    loadSerieUsuario();
    ObterPoliticaSenha();

    const configuracaoConciliacaoTransportador = await buscarConfiguracaoConciliacaoTransportador();
    if (configuracaoConciliacaoTransportador.HabilitarGeracaoAutomatica) {
        _usuario.PermiteAssinarAnuencia.visible(true);
    }
}

function ObterPoliticaSenha() {

    executarReST("PoliticaSenha/BuscarPoliticaSenhaPorServicoMultiSoftware", { TipoServicoPoliticaSenha: 3 }, function (arg) {
        if (arg.Success) {
            _politicaSenha = arg.Data;

            if (!_politicaSenha.HabilitarPoliticaSenha) {
                _usuario.UsuarioAcessoBloqueado.visible(false);
            }

            if (_politicaSenha.ExigirTrocaSenhaPrimeiroAcesso || _politicaSenha.SenhaPadraoPrimeiroAcesso) {
                _usuario.Senha.visible(false);
                _usuario.Senha.required = false;
                _usuario.ConfirmacaoSenha.visible(false);
                _usuario.Usuario.cssClass("col col-6");
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

    });
}

function adicionarUsuarioClick(e, sender) {
    Salvar(_usuario, "UsuarioTransportador/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Transportadores.Transportador.DadosSalvosSucesso);
                _gridUsuario.CarregarGrid();
                limparCamposUsuario();
                if (!_politicaSenha.ExigirTrocaSenhaPrimeiroAcesso) {
                    _usuario.Senha.required = true;
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function atualizarUsuarioClick(e, sender) {
    Salvar(_usuario, "UsuarioTransportador/Salvar", function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Transportadores.Transportador.DadosSalvosSucesso);
            _gridUsuario.CarregarGrid();
            limparCamposUsuario();
            if (!_politicaSenha.ExigirTrocaSenhaPrimeiroAcesso) {
                _usuario.Senha.required = true;
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function cancelarUsuarioClick(e) {
    limparCamposUsuario();
    if (!_politicaSenha.ExigirTrocaSenhaPrimeiroAcesso) {
        _usuario.Senha.required = true;
    }
}

function ResetarSenhaUsuarioClick(e, sender) {
    Salvar(_usuario, "UsuarioTransportador/ResetarSenha", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Transportadores.Transportador.ResetarSenhaSucesso);
                _gridUsuario.CarregarGrid();
                limparCamposUsuario();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

//*******MÉTODOS*******

function buscarUsuarios() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarUsuario, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridUsuario = new GridView(_pesquisaUsuario.Pesquisar.idGrid, "UsuarioTransportador/Pesquisa", _pesquisaUsuario, menuOpcoes, null);
    _gridUsuario.CarregarGrid();
}

function editarUsuario(usuarioGrid) {
    limparCamposUsuario();
    _usuario.Senha.required = false;

    //if (!_politicaSenha.ExigirTrocaSenhaPrimeiroAcesso) {
    //    _usuario.Senha.required = true;
    //}
    _usuario.Codigo.val(usuarioGrid.Codigo);
    BuscarPorCodigo(_usuario, "UsuarioTransportador/BuscarPorCodigo?CodigoTransportador=" + _transportador.Codigo.val(), function (arg) {

        _pesquisaUsuario.ExibirFiltros.visibleFade(false);

        _crudUsuario.Atualizar.visible(true);
        _crudUsuario.Cancelar.visible(true);
        _crudUsuario.Adicionar.visible(false);

        if (_politicaSenha.HabilitarPoliticaSenha) {
            _crudUsuario.ResetarSenha.visible(true);
        }

        if (_politicaSenha.ExigirTrocaSenhaPrimeiroAcesso) {
            _usuario.Usuario.cssGroupClass("input-group");
            _usuario.RedefinirSenha.visible(true);
        }

        recarregarGridSerieUsuario();
        recarregarPermissoesUsuario();

    }, null);
}

function limparCamposUsuario() {

    _crudUsuario.Atualizar.visible(false);
    _crudUsuario.Cancelar.visible(false);
    _crudUsuario.Adicionar.visible(true);
    _crudUsuario.ResetarSenha.visible(false);

    LimparCampos(_usuario);

    limparCamposSerieUsuario();

    limparPermissoesUsuario();
    recarregarGridSerieUsuario();

    resetarTabsUsuario();
}

function resetarTabsUsuario() {
    $("#tabCadastroUsuario a:first").tab("show");
}

function alterarEstadoCadastroUsuario() {

    if (_transportador.Codigo.val() > 0) {

        loadUsuario();

        $("#liTabUsuarios").removeClass("d-none");

        limparCamposUsuario();

        buscarUsuarios();

    } else {
        $("#liTabUsuarios").addClass("d-none");
    }
}

function redefinirSenhaClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Transportadores.Transportador.RealmenteDesejaRedefinirSenhaDesteUsuarioLembrandoAposRedefinicaoSenhaUsuarioPerderSenhaAtualSeraObrigadoDefinarUmaNovaSenhaProximoAcesso, function () {
        Salvar(e, "Usuario/RedefinirSenha", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Transportadores.Transportador.SenhaRedefinidaSucesso);
                } else {
                    exibirMensagem("aviso", Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    });
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorio, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function buscarConfiguracaoConciliacaoTransportador() {
    return new Promise((resolve) => {
        executarReST("ConfiguracaoConciliacaoTransportador/ObterConfiguracao", {}, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    resolve(retorno.Data);
                }
                else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                    resolve(null);
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                resolve(null);
            }
        });
    })
}