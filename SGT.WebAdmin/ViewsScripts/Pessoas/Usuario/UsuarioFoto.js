/// <reference path="Usuario.js" />

/*
 * Declaração das Funções
 */

function enviarFotoUsuario() {
    var formData = obterFormDataFotoUsuario();

    if (formData) {
        enviarArquivo("Usuario/AdicionarFoto?callback=?", { Codigo: _usuario.Codigo.val() }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    _usuario.FotoUsuario.val(retorno.Data.FotoUsuario);
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

function obterFormDataFotoUsuario() {
    var arquivo = document.getElementById(_usuario.ArquivoFoto.id);

    if (arquivo.files.length > 0) {
        var formData = new FormData();

        formData.append("ArquivoFoto", arquivo.files[0]);

        return formData;
    }

    return undefined;
}

function removerFotoUsuario() {
    executarReST("Usuario/ExcluirFoto", { Codigo: _usuario.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _usuario.FotoUsuario.val("");
                _usuario.ArquivoFoto.val("");
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}