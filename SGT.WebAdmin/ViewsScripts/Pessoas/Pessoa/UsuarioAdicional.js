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

var _usuarioAdicional;
var _gridUsuariosAdicionais = new Array();


//*******MAPEAMENTO KNOUCKOUT*******

var usuarioAdicional = function () {

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.UsuarioAcesso = PropertyEntity({ val: ko.observable(""), def: "" });
    this.NomeColaborador = PropertyEntity({ text: `*${Localization.Resources.Pessoas.Pessoa.NomeColaboradorUsuarioAdicional.getFieldDescription()}`, required: true }); 
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription(), enable: ko.observable(false), visible: ko.observable(true) });
    this.Email = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Email.getRequiredFieldDescription()), required: true, maxlength: 500, getType: typesKnockout.email });
    this.CNPJ_CPF = PropertyEntity({ text: `*${Localization.Resources.Pessoas.Pessoa.CNPJCPF.getFieldDescription()}`, required: true, getType: _CONFIGURACAO_TMS.Pais == EnumPaises.Brasil ? typesKnockout.cpfCnpj : typesKnockout.string});
    this.Senha = PropertyEntity({ text: `*${Localization.Resources.Pessoas.Pessoa.SenhaParaAcesso.getFieldDescription()}`, required: true, getType: typesKnockout.string, maxlength: 15, visible: ko.observable(true) });
    this.ConfirmaSenha = PropertyEntity({ text: `*${Localization.Resources.Pessoas.Pessoa.ConfirmaSenhaParaAcesso.getFieldDescription()}`, required: true, getType: typesKnockout.string, maxlength: 15, visible: ko.observable(true) });

    this.GridUsuarioAdicionais = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: "", idGrid: guid() });

    this.UsuariosAdicionais = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarUsuarioAdicionalClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarUsuarioAdicionalClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadUsuarioAdicional() {

    _usuarioAdicional = new usuarioAdicional();
    KoBindings(_usuarioAdicional, "knockoutUsuarioAdicional");
    loadGridUsuarioAdicional();
}

function loadGridUsuarioAdicional() {

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [] };

    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarUsuarioAdicionalClick, tamanho: "15", icone: "" };
    var enviarPorEmail = { descricao: "Enviar por E-mail", id: guid(), evento: "onclick", metodo: enviarUsuarioPorEmailClick, tamanho: "15", icone: ""};

    menuOpcoes.opcoes.push(editar);
    menuOpcoes.opcoes.push(enviarPorEmail);

    var header = [
        { data: "Codigo", visible: false },
        { data: "NomeColaborador", title: "Nome Colaborador", width: "30%" },
        { data: "UsuarioAcesso", title: "Usuário para acesso", width: "30%" },
        { data: "CNPJ_CPF", title: "CPF/CNPJ", width: "10%" },
        { data: "Email", title: "Email", width: "30%" },
        { data: "Ativo", title: "Situação", width: "10%" },
        { data: "Senha", visible: false }
    ];

    _usuarioAdicional.GridUsuarioAdicionais.basicTable = new BasicDataTable(_usuarioAdicional.GridUsuarioAdicionais.idGrid, header, menuOpcoes);
    _usuarioAdicional.GridUsuarioAdicionais.basicTable.CarregarGrid(new Array());
}

function adicionarUsuarioAdicionalClick() {

    if (validarCamposObrigatoriosUsuarioAdicional(false)) {
       
        var usuarioAdicional = new Object();
       
        usuarioAdicional.Codigo = guid();
        usuarioAdicional.NomeColaborador = _usuarioAdicional.NomeColaborador.val();
        usuarioAdicional.UsuarioAcesso = retornaUsuarioAcesso();
        usuarioAdicional.CNPJ_CPF = _usuarioAdicional.CNPJ_CPF.val();
        usuarioAdicional.Email = _usuarioAdicional.Email.val();
        usuarioAdicional.Ativo = retornaSituacao();
        usuarioAdicional.Senha = _usuarioAdicional.Senha.val();

        _gridUsuariosAdicionais.push(usuarioAdicional);

        recarregarUsuariosAdicionaisGrid();
        LimparCamposUsuarioAdicionais();
    } 

}

function atualizarUsuarioAdicionalClick() {

    if (validarCamposObrigatoriosUsuarioAdicional(true)) {

        $.each(_gridUsuariosAdicionais, function (i, usuarioAdicional) {

            if (usuarioAdicional.Codigo == _usuarioAdicional.Codigo.val()) {

                usuarioAdicional.NomeColaborador = _usuarioAdicional.NomeColaborador.val();
                usuarioAdicional.UsuarioAcesso = retornaUsuarioAcesso();
                usuarioAdicional.CNPJ_CPF = _usuarioAdicional.CNPJ_CPF.val();
                usuarioAdicional.Email = _usuarioAdicional.Email.val();
                usuarioAdicional.Ativo = retornaSituacao();
                usuarioAdicional.Senha = _usuarioAdicional.Senha.val();

                LimparCamposUsuarioAdicionais();

                return false;
            }
        });

        recarregarUsuariosAdicionaisGrid();
    } 

}

function editarUsuarioAdicionalClick(item)
{
    _usuarioAdicional.Codigo.val(item.Codigo);
    _usuarioAdicional.NomeColaborador.val(item.NomeColaborador);
    _usuarioAdicional.UsuarioAcesso.val(item.UsuarioAcesso);
    _usuarioAdicional.CNPJ_CPF.val(item.CNPJ_CPF);
    _usuarioAdicional.Senha.val(item.Senha);
    _usuarioAdicional.ConfirmaSenha.val(item.Senha);
    _usuarioAdicional.Email.val(item.Email);
    _usuarioAdicional.Ativo.val(item.Ativo);

    _usuarioAdicional.Adicionar.visible(false);
    _usuarioAdicional.Atualizar.visible(true);
    _usuarioAdicional.Ativo.enable(true);
}

function enviarUsuarioPorEmailClick(valor) {

    executarReST("Pessoa/EnviarEmailUsuarioTerceiroAdicional", { CNPJ: valor.CNPJ_CPF }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Pessoas.Pessoa.EmailEnviadoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function validarCamposObrigatoriosUsuarioAdicional(alteracao) {
  
    if (ValidarCamposObrigatorios(_usuarioAdicional) === false) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return false;
    }

    if (!ValidarEmail(_usuarioAdicional.Email.val())) {
        exibirMensagem(tipoMensagem.atencao, "Email Inválido", "Email Inválido, verifique e tente novamente");
        return false;
    }

    if (!validarSenhaUsuarioTerceiroAdicional()) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.Pessoa.SenhasUsuarioTerceiro, Localization.Resources.Pessoas.Pessoa.SenhasDoUsuarioTerceiroNaoConferemVerifiqueTenteNovamente);
        return false;
    }

    var cnpj_cpf_sem_formatacao = _usuarioAdicional.CNPJ_CPF.val();
    cnpj_cpf_sem_formatacao = cnpj_cpf_sem_formatacao.replace(/\D/g, '');

    if (cnpj_cpf_sem_formatacao.length <= 11) {
        if (!ValidarCPF(cnpj_cpf_sem_formatacao)) {
            exibirMensagem(tipoMensagem.atencao, "CPF Inválido", "CPF Inválido, verifique e tente novamente");
            return false;
        }
    } else {
        if (!ValidarCNPJ(cnpj_cpf_sem_formatacao)) {
            exibirMensagem(tipoMensagem.atencao, "CNPJ Inválido", "CNPJ Inválido, verifique e tente novamente");
            return false;
        }
    }

    if (!alteracao) {
        if (!ValidaUsuarioTerceiroJaAdicionado(cnpj_cpf_sem_formatacao)) {
            exibirMensagem(tipoMensagem.atencao, "Usuário Inválido", `Já existe um Usuário Adicional com este CPF/CNPJ: ${_usuarioAdicional.CNPJ_CPF.val()}`);
            return false;
        }
    }

    return true 
}

function LimparCamposUsuarioAdicionais() {

    _usuarioAdicional.Adicionar.visible(true);
    _usuarioAdicional.Atualizar.visible(false);
    _usuarioAdicional.Ativo.enable(false);

    LimparCampos(_usuarioAdicional);
}

function retornaUsuarioAcesso() {

    var usuarioAcesso = _usuarioAdicional.CNPJ_CPF.val();
    return usuarioAcesso.replace(/\D/g, '');
}

function retornaSituacao() {

    return _usuarioAdicional.Ativo.val() ? Localization.Resources.Gerais.Geral.Ativo : Localization.Resources.Gerais.Geral.Inativo
}

function preencherGridUsuariosAdicionais() {

    _gridUsuariosAdicionais = new Array();
    _usuarioAdicional.GridUsuarioAdicionais.basicTable.SetarRegistros(new Array());
    _usuarioAdicional.GridUsuarioAdicionais.basicTable.CarregarGrid(new Array());

    if (_pessoa.UsuariosAdicionais.val() == null || _pessoa.UsuariosAdicionais.val() == undefined)
        return;

    for (var i = 0; i < _pessoa.UsuariosAdicionais.val().length; i++) {
       
        
        var usuarioAdicional = _pessoa.UsuariosAdicionais.val()[i];

        _gridUsuariosAdicionais.push({
            Codigo: usuarioAdicional.Codigo,
            NomeColaborador: usuarioAdicional.NomeColaborador,
            UsuarioAcesso: usuarioAdicional.UsuarioAcesso,
            CNPJ_CPF: formatarCpfCnpj(usuarioAdicional.CNPJ_CPF),
            Email: usuarioAdicional.Email,
            Senha: usuarioAdicional.Senha,
            Ativo: usuarioAdicional.Ativo
        });
    }

    recarregarUsuariosAdicionaisGrid();
}

function recarregarUsuariosAdicionaisGrid() {
    _usuarioAdicional.GridUsuarioAdicionais.basicTable.SetarRegistros(_gridUsuariosAdicionais);
    _usuarioAdicional.GridUsuarioAdicionais.basicTable.CarregarGrid(_gridUsuariosAdicionais);
}

function validarSenhaUsuarioTerceiroAdicional() {
    return (_usuarioAdicional.Senha.val() == _usuarioAdicional.ConfirmaSenha.val());
}

function ValidaUsuarioTerceiroJaAdicionado(cnpjCpfSemFormatacao) {

    //válida se já existe um usuário adicionado a grid com o mesmo CNPJ/CPF
    return !_gridUsuariosAdicionais.some(usuario => {
        const cnpjCpfGrid = (usuario.CNPJ_CPF || '').replace(/\D/g, '');
        return cnpjCpfGrid === cnpjCpfSemFormatacao;
    });
}

function formatarCpfCnpj(valor) {
    // Remove tudo que não for número
    valor = valor.replace(/\D/g, '');

    if (valor.length <= 11) {
        // Aplica máscara de CPF: XXX.XXX.XXX-XX
        valor = valor.replace(/(\d{3})(\d)/, '$1.$2');
        valor = valor.replace(/(\d{3})(\d)/, '$1.$2');
        valor = valor.replace(/(\d{3})(\d{1,2})$/, '$1-$2');
    } else {
        // Aplica máscara de CNPJ: XX.XXX.XXX/0001-XX
        valor = valor.replace(/^(\d{2})(\d)/, '$1.$2');
        valor = valor.replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3');
        valor = valor.replace(/\.(\d{3})(\d)/, '.$1/$2');
        valor = valor.replace(/(\d{4})(\d{1,2})$/, '$1-$2');
    }

    return valor;
}