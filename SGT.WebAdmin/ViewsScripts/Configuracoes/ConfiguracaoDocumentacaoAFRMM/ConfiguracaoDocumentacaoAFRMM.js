var _configuracao;
var _crudConfiguracao;

var Configuracao = function () {
    this.QuantidadeDiasAposDescarga = PropertyEntity({ text: "Qtd. dias corridos após a descarga do navio no porto de destino toda a documentação:", getType: typesKnockout.int, val: ko.observable(0), def: 0, maxlength: 6 });
    this.EnderecoFTP = PropertyEntity({ text: "Endereço FTP:", getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 150 });
    this.PortaFTP = PropertyEntity({ text: "Porta FTP:", getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 10 });
    this.DiretorioFTP = PropertyEntity({ text: "Diretório FTP (Normal):", getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 400 });
    this.DiretorioFTPSubcontratacao = PropertyEntity({ text: "Diretório FTP (Subcontratação):", getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 400 });
    this.DiretorioFTPRedespacho = PropertyEntity({ text: "Diretório FTP (Redespacho / SVM Terceiro):", getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 400 });
    this.UsuarioFTP = PropertyEntity({ text: "Usuário FTP:", getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 50 });
    this.SenhaFTP = PropertyEntity({ text: "Senha FTP:", getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 50 });
    this.FTPPassivo = PropertyEntity({ text: "FTP Passivo", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.SFTP = PropertyEntity({ text: "SFTP", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SSL = PropertyEntity({ text: "SSL", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.EmailFalhaEnvio = PropertyEntity({ text: "E-mail para falha no envio:", getType: typesKnockout.multiplesEmails, val: ko.observable(""), def: "", maxlength: 400 });
};

var CRUDConfiguracao = function () {
    this.Salvar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
    this.TestarFTP = PropertyEntity({ eventClick: testarFTPClick, type: types.event, text: "Testar FTP", visible: ko.observable(true) });
};

function loadConfiguracao() {
    _configuracao = new Configuracao();
    KoBindings(_configuracao, "knockoutCadastro");

    _crudConfiguracao = new CRUDConfiguracao();
    KoBindings(_crudConfiguracao, "knockoutCRUD");

    carregarConfiguracao();
}

function atualizarClick() {
    if (ValidarMultiplosEmails(_configuracao.EmailFalhaEnvio.val()) === false) {
        exibirMensagem(tipoMensagem.atencao, "E-mail inválido!", "O e-mail " + _configuracao.EmailFalhaEnvio.val() + " está inválido!");
        return;
    } else {
        if (ValidarCamposObrigatorios(_configuracao)) {
            executarReST("ConfiguracaoDocumentacaoAFRMM/AtualizarConfiguracao", obterConfiguracaoSalvar(), function (arg) {
                if (arg.Success) {
                    if (arg.Data)
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                    else
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            });
        }
    }
}

function testarFTPClick() {
    executarReST("FTP/TestarConexao", {
        Host: _configuracao.EnderecoFTP.val(),
        Porta: _configuracao.PortaFTP.val(),
        Diretorio: _configuracao.DiretorioFTP.val(),
        Usuario: _configuracao.UsuarioFTP.val(),
        Senha: _configuracao.SenhaFTP.val(),
        Passivo: _configuracao.FTPPassivo.val(),
        UtilizarSFTP: _configuracao.SFTP.val(),
        SSL: _configuracao.SSL.val()
    }, function (r) {
        if (r.Success)
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Teste de conexão realizado com sucesso!");
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
    });
}

function carregarConfiguracao() {
    executarReST("ConfiguracaoDocumentacaoAFRMM/ObterConfiguracao", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                PreencherObjetoKnout(_configuracao, arg);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function obterConfiguracaoSalvar() {
    var configuracao = RetornarObjetoPesquisa(_configuracao);
    return configuracao;
}