/// <reference path="GrupoPessoas.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _configuracaoLogo;

var ConfiguracaoLogo = function () {
    this.ArquivoLogo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Pessoas.GrupoPessoas.Arquivo.getFieldDescription()), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.ArquivoLogoRemover = PropertyEntity({ eventClick: removerLogoGrupoPessoas, type: types.event, text: ko.observable(Localization.Resources.Pessoas.GrupoPessoas.Remover), visible: ko.observable(true) });
    this.LogoGrupoPessoas = PropertyEntity({});

    this.ArquivoLogo.val.subscribe(function (nomeArquivoLogoSelecionado) {
        if (nomeArquivoLogoSelecionado)
            enviarLogoGrupoPessoas();
    });
};

//*******EVENTOS*******

function loadConfiguracaoLogo() {
    _configuracaoLogo = new ConfiguracaoLogo();
    KoBindings(_configuracaoLogo, "knockoutCadastroConfiguracaoLogo");
}

function enviarLogoGrupoPessoas() {
    var formData = obterFormDataLogoGrupoPessoas();

    if (formData) {
        enviarArquivo("GrupoPessoas/EnviarLogo?callback=?", { Codigo: _grupoPessoas.Codigo.val() }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    preencherLogoGrupoPessoas(retorno.Data.LogoGrupoPessoas);
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.GrupoPessoas.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.GrupoPessoas.Falha, retorno.Msg);
        });
    }
}

function removerLogoGrupoPessoas() {
    executarReST("GrupoPessoas/ExcluirLogo", { Codigo: _grupoPessoas.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _configuracaoLogo.LogoGrupoPessoas.val("");
                _configuracaoLogo.ArquivoLogo.val("");
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.GrupoPessoas.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.GrupoPessoas.Falha, retorno.Msg);
    });
}

function obterFormDataLogoGrupoPessoas() {
    var arquivo = document.getElementById(_configuracaoLogo.ArquivoLogo.id);

    if (arquivo.files.length > 0) {
        var formData = new FormData();

        formData.append("ArquivoLogo", arquivo.files[0]);

        return formData;
    }

    return undefined;
}

function preencherLogoGrupoPessoas(logo) {
    _configuracaoLogo.LogoGrupoPessoas.val(logo);
    $("#liTabConfiguracaoLogo").show();
}

function limparConfiguracaoLogo() {
    LimparCampos(_configuracaoLogo);
    $("#liTabConfiguracaoLogo").hide();
}