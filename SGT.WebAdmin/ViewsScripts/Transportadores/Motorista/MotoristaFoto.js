/// <reference path="Motorista.js" />

/*
 * Declaração das Funções
 */

function enviarFotoMotorista() {
    var formData = obterFormDataFotoMotorista();

    if (formData) {
        enviarArquivo("Motorista/AdicionarFoto?callback=?", { Codigo: _motorista.Codigo.val() }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    _motorista.FotoMotorista.val(retorno.Data.FotoMotorista);
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

function excluirImagemGaleria(e) {
    var nomeArquivo = e.getAttribute("nomeArquivo");

    executarReST("Motorista/ExcluirFotoGaleria", { Codigo: _motorista.Codigo.val(), NomeArquivo: nomeArquivo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _motorista.GaleriaMotorista.val(retorno.Data.GaleriaMotorista);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function enviarFotoAdicionalMotorista() {
    var formData = obterFormDataFotoMotoristaAdicional();
    _motorista.AdicionarFoto.val('');

    if (formData) {
        enviarArquivo("Motorista/AdicionarFotoGaleria?callback=?", { Codigo: _motorista.Codigo.val() }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) 
                    _motorista.GaleriaMotorista.val(retorno.Data.GaleriaMotorista);
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

function obterFormDataFotoMotorista() {
    var arquivo = document.getElementById(_motorista.ArquivoFoto.id);

    if (arquivo.files.length > 0) {
        var formData = new FormData();

        formData.append("ArquivoFoto", arquivo.files[0]);

        return formData;
    }

    return undefined;
}

function obterFormDataFotoMotoristaAdicional() {
    var arquivo = document.getElementById(_motorista.AdicionarFoto.id);

    if (arquivo.files.length > 0) {
        var formData = new FormData();

        formData.append("ArquivoFoto", arquivo.files[0]);

        return formData;
    }

    return undefined;
}

function removerFotoMotorista() {
    executarReST("Motorista/ExcluirFoto", { Codigo: _motorista.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _motorista.FotoMotorista.val("");
                _motorista.ArquivoFoto.val("");
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// Busca no server central a foto do motorista. A foto é adicionada pelo motorista no app.
function carregarFotoDoAppMotorista() {
    if (!_motorista.CPF.val()) return;

    const cpfMotorista = _motorista.CPF.val().replace(/\./g, "").replace(/\-/g, "").replace(/\//g, "").replace(" ", "");
    //const urlServerCentral = "http://localhost:52952"; //Para testes em dev
    const urlServerCentral = "https://tms.multicte.com.br/SGT.Mobile.https";
    const urlObterImagem = `${urlServerCentral}/MTrack/v1/Usuario.svc/ObterImagemMotoristaPorCpf?cpf=${cpfMotorista}`

    $.get(urlObterImagem, function (data) {
        if (data.Imagem) {
            _motorista.FotoMotorista.val(data.Imagem);
        }
    });
}