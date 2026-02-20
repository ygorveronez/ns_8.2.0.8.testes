/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _configuracaoLogo;

var ConfiguracaoLogo = function () {
    this.ArquivoLogo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: ko.observable("Selecionar"), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true), accept: ko.observable(".bmp") });
    this.ArquivoLogoRemover = PropertyEntity({ eventClick: removerLogoEmpresa, type: types.event, text: ko.observable("Remover"), visible: ko.observable(true) });
    this.LogoEmpresa = PropertyEntity({});

    this.ArquivoLogo.val.subscribe(function (nomeArquivoLogoSelecionado) {
        if (nomeArquivoLogoSelecionado)
            enviarLogoEmpresa();
    });
};

//*******EVENTOS*******

function loadConfiguracaoLogo() {
    _configuracaoLogo = new ConfiguracaoLogo();
    KoBindings(_configuracaoLogo, "knockoutCadastroConfiguracaoLogo");

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFeAdmin)
        _configuracaoLogo.ArquivoLogo.accept(".png");
}

function enviarLogoEmpresa() {
    var formData = obterFormDataLogoEmpresa();

    if (formData) {
        enviarArquivo("Transportador/EnviarLogo?callback=?", { Codigo: _transportador.Codigo.val() }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    preencherLogoEmpresa(retorno.Data.LogoEmpresa);
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

function removerLogoEmpresa() {
    executarReST("Transportador/ExcluirLogo", { Codigo: _transportador.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                LimparCampos(_configuracaoLogo);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function obterFormDataLogoEmpresa() {
    var arquivo = document.getElementById(_configuracaoLogo.ArquivoLogo.id);

    if (arquivo.files.length > 0) {
        var formData = new FormData();

        formData.append("ArquivoLogo", arquivo.files[0]);

        return formData;
    }

    return undefined;
}

function preencherLogoEmpresa(logo) {
    _configuracaoLogo.LogoEmpresa.val(logo);
}

function limparConfiguracaoLogo() {
    LimparCampos(_configuracaoLogo);
    $("#liTabConfiguracaoLogo").hide();
}