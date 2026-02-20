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
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Enumeradores/EnumTipoConexaoEmail.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridConfigEmailDocTransporte;
var _configEmailDocTransporte;
var _pesquisaConfigEmailDocTransporte;
var _existeEmailAtivo;

var _emailAtivo = [
    { text: Localization.Resources.Gerais.Geral.Sim, value: true },
    { text: Localization.Resources.Gerais.Geral.Nao, value: false }
];

var PesquisaConfigEmailDocTransporte = function () {
    this.Email = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.Email.getFieldDescription() });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConfigEmailDocTransporte.CarregarGrid(verificarLocalAtual);
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltroPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var ConfigEmailDocTransporte = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Email = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.Email.getRequiredFieldDescription(), required: true, maxlength: 150 });
    this.DisplayEmail = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.DisplayEmail.getFieldDescription(), required: false, maxlength: 150 });
    
    this.Senha = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Senha.getRequiredFieldDescription(), required: true, maxlength: 500 });
    this.ConfimarSenha = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Confirmacao.getRequiredFieldDescription(), required: false, type: types.local, maxlength: 500 });

    this.LerDocumentos = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.EsteEmailSeraUtilizadoParaLeituraDocumentoFiscais, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool });
    this.EnviarDocumentos = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.EsteEmailSeraUtilizadoParaEnvioDocumentoFiscais, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.SmtpAtivo = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.HabilitarEnvioDesteEmailViaSMTP, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool });
    this.Smtp = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.ServidorSMTP.getRequiredFieldDescription(), required: false, maxlength: 150 });
    this.PortaSmtp = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.PortaSMTP.getFieldDescription(), required: false, maxlength: 5, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.RequerAutenticacaoSmtp = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.SMTPRequerAutenticacao, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool });
    this.MensagemRodape = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.MensagemRodape.getFieldDescription(), maxlength: 600 });

    this.Pop3Ativo = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.HabilitarLeituraDesteEmailViaPOP3, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool });
    this.Pop3 = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.ServidorPOP3.getRequiredFieldDescription(), required: false, maxlength: 150 });
    this.PortaPop3 = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.PortaPOP3.getFieldDescription(), required: false, maxlength: 5, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.RequerAutenticacaoPop3 = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.POP3RquerAutenticacao, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool });

    this.ApiEnvioEmail = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.ApiEnvioEmail, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool });
    this.ServidorEmail = PropertyEntity({ val: ko.observable(EnumTipoServidorEmail.Gmail), options: EnumTipoServidorEmail.obterOpcoes(), text: Localization.Resources.Email.ConfigEmailDocTransporte.ServidorEmail.getFieldDescription(), def: EnumTipoServidorEmail.Gmail, required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.UrlAutenticacao = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.UrlAutenticacao.getRequiredFieldDescription(), required: false, maxlength: 250 });
    this.ClientId = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.ClientId, maxlength: 600, visible: ko.observable(true) });
    this.ClientSecret = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.ClientSecret, maxlength: 600, visible: ko.observable(true) });
    this.UrlEnvio = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.UrlEnvio.getRequiredFieldDescription(), required: false, maxlength: 250 });
    this.caminhoTokenResposta = PropertyEntity({ type: types.file, id: "caminhoTokenResposta", codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), visible: ko.observable(true) });

    this.EmailAtivo = PropertyEntity({ val: ko.observable(true), options: _emailAtivo, def: true, text: Localization.Resources.Email.ConfigEmailDocTransporte.EmailAtivo.getRequiredFieldDescription() });
    this.EmailJaAtivo = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.TagQuebraLinha = PropertyEntity({ eventClick: function (e) { InserirTag(_configEmailDocTransporte.MensagemRodape.id, "#qLinha#"); }, type: types.event, text: Localization.Resources.Email.ConfigEmailDocTransporte.QuebraDeLinha, visible: ko.observable(true) });

    this.TipoConexaoEmail = PropertyEntity({ val: ko.observable(EnumTipoConexaoEmail.Padrao), options: EnumTipoConexaoEmail.obterOpcoes(), text: Localization.Resources.Email.ConfigEmailDocTransporte.TipoConexao.getFieldDescription(), def: EnumTipoConexaoEmail.Padrao, required: false, enable: ko.observable(true), visible: ko.observable(false) });
    
 
    this.TenantId = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.TenantId, maxlength: 600, visible: ko.observable(false) });
    this.RedirectUri = PropertyEntity({ text: Localization.Resources.Email.ConfigEmailDocTransporte.RedirectUri, maxlength: 600, visible: ko.observable(false) });

    this.TipoConexaoEmail.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoConexaoEmail.Padrao) {
            _configEmailDocTransporte.ClientId.visible(false);
            _configEmailDocTransporte.ClientSecret.visible(false);
            _configEmailDocTransporte.TenantId.visible(false);
            _configEmailDocTransporte.RedirectUri.visible(false);
        }
        else if (novoValor == EnumTipoConexaoEmail.Gmail) {
            _configEmailDocTransporte.ClientId.visible(false);
            _configEmailDocTransporte.ClientSecret.visible(false);
            _configEmailDocTransporte.TenantId.visible(false);
            _configEmailDocTransporte.RedirectUri.visible(false);
        }
        else if (novoValor == EnumTipoConexaoEmail.Exchange) {
            _configEmailDocTransporte.ClientId.visible(true);
            _configEmailDocTransporte.ClientSecret.visible(true);
            _configEmailDocTransporte.TenantId.visible(true);
            _configEmailDocTransporte.RedirectUri.visible(true);
        }
    });
}

//*******EVENTOS*******

function loadConfigEmailDocTransporte() {

    _configEmailDocTransporte = new ConfigEmailDocTransporte();
    KoBindings(_configEmailDocTransporte, "knockoutCadastroConfigEmailDocTransporte");

    _pesquisaConfigEmailDocTransporte = new PesquisaConfigEmailDocTransporte();
    KoBindings(_pesquisaConfigEmailDocTransporte, "knockoutPesquisaConfigEmailDocTransporte", false, _pesquisaConfigEmailDocTransporte.Pesquisar.id);

    buscarConfigEmailDocTransportes();

    HeaderAuditoria("ConfigEmailDocTransporte", _configEmailDocTransporte);
}

function adicionarClick(e, sender) {
    if (_existeEmailAtivo && e.EmailAtivo.val() && e.EnviarDocumentos.val()) {
        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Email.ConfigEmailDocTransporte.RealmenteDesejaTornarEsseEmail + " " + e.Email.val() + " " + Localization.Resources.Email.ConfigEmailDocTransporte.ComoPadraoEnvioDocumentosTransporte, function () {
            adicionar(e, sender);
        });
    } else {
        adicionar(e, sender);
    }
}


function atualizarClick(e, sender) {
    if (e.EmailJaAtivo.val() && e.EmailAtivo.val() && e.EnviarDocumentos.val()) {
        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Email.ConfigEmailDocTransporte.RealmenteDesejaTornarEsseEmail + " " + e.Email.val() + " " + Localization.Resources.Email.ConfigEmailDocTransporte.ComoPadraoEnvioDocumentosTransporte, function () {
            atualizar(e, sender);
        });
    } else {
        atualizar(e, sender);
    }
}


function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Email.ConfigEmailDocTransporte.RealmenteDesejaExcluirEmail + " " + _configEmailDocTransporte.Email.val() + "?", function () {
        ExcluirPorCodigo(_configEmailDocTransporte, "ConfigEmailDocTransporte/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Successo, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                _gridConfigEmailDocTransporte.CarregarGrid(verificarLocalAtual);
                limparCamposConfigEmailDocTransporte();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposConfigEmailDocTransporte();
}

//*******MÉTODOS*******

function adicionar(e, sender) {
    if (e.Senha.val() == e.ConfimarSenha.val()) {
        Salvar(e, "ConfigEmailDocTransporte/Adicionar", function (arg) {
            if (arg.Success) {
                enviarArquivos(e.Codigo);
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Successo, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                    _gridConfigEmailDocTransporte.CarregarGrid(verificarLocalAtual);
                    limparCamposConfigEmailDocTransporte();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender);
    } else {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Email.ConfigEmailDocTransporte.ValidacaoSenha, Localization.Resources.Email.ConfigEmailDocTransporte.ConfirmacaoDeveSerIgualSenha);
    }
}

function atualizar(e, sender) {
    if (e.Senha.val() == e.ConfimarSenha.val()) {
        Salvar(e,"ConfigEmailDocTransporte/Atualizar", function (arg) {
            if (arg.Success) {
                enviarArquivos(e.Codigo);
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                    _gridConfigEmailDocTransporte.CarregarGrid(verificarLocalAtual);
                    limparCamposConfigEmailDocTransporte();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender);
    } else {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Email.ConfigEmailDocTransporte.ValidacaoSenha, Localization.Resources.Email.ConfigEmailDocTransporte.ConfirmacaoDeveSerIgualSenha);
    }
}
function enviarArquivos(e, sender) {
    var file = document.getElementById(_configEmailDocTransporte.caminhoTokenResposta.id);
    if (!file) {
    var formData = new FormData();
    formData.append("upload", file.files[0]);
        enviarArquivo("ConfigEmailDocTransporte/EnviarArquivo?callback=?", { Codigo: _configEmailDocTransporte.Codigo.val() }, formData, function (arg) {
            if (arg.Success) {
                _configEmailDocTransporte.caminhoTokenResposta.val("");
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha ao Salvar Arquivo", arg.Msg);
            }
        });
    }
}
function buscarConfigEmailDocTransportes() {

    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarConfigEmailDocTransporte, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };
    _gridConfigEmailDocTransporte = new GridView(_pesquisaConfigEmailDocTransporte.Pesquisar.idGrid, "ConfigEmailDocTransporte/Pesquisa", _pesquisaConfigEmailDocTransporte, menuOpcoes);
    _gridConfigEmailDocTransporte.CarregarGrid(verificarLocalAtual);

}

function verificarLocalAtual() {
    executarReST("ConfigEmailDocTransporte/VerificarSeExisteEmailAtivo", null, function (arg) {
        if (arg.Success) {
            _existeEmailAtivo = arg.Data;
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function editarConfigEmailDocTransporte(configEmailDocTransporteGrid) {
    limparCamposConfigEmailDocTransporte();
    _configEmailDocTransporte.Codigo.val(configEmailDocTransporteGrid.Codigo);
    BuscarPorCodigo(_configEmailDocTransporte, "ConfigEmailDocTransporte/BuscarPorCodigo", function (arg) {
        _pesquisaConfigEmailDocTransporte.ExibirFiltros.visibleFade(false);
        _configEmailDocTransporte.Atualizar.visible(true);
        _configEmailDocTransporte.Cancelar.visible(true);
        _configEmailDocTransporte.Excluir.visible(true);
        _configEmailDocTransporte.Adicionar.visible(false);
        _configEmailDocTransporte.ConfimarSenha.val(_configEmailDocTransporte.Senha.val());
    }, null);
}

function limparCamposConfigEmailDocTransporte() {
    _configEmailDocTransporte.Atualizar.visible(false);
    _configEmailDocTransporte.Cancelar.visible(false);
    _configEmailDocTransporte.Excluir.visible(false);
    _configEmailDocTransporte.Adicionar.visible(true);
    _configEmailDocTransporte.ConfimarSenha.val(_configEmailDocTransporte.ConfimarSenha.def);
    LimparCampos(_configEmailDocTransporte);
}

