function atualizarDadosRecebedor(entrega) {

    var temNome = ValidarCampoObrigatorioMap(entrega.DadosRecebedorNome);
    var temCPF = ValidarCampoObrigatorioMap(entrega.DadosRecebedorCPF);
    var temDataEntrega = ValidarCampoObrigatorioMap(entrega.DadosRecebedorDataEntrega);

    if (!temNome || !temCPF || !temDataEntrega) {
        return;
    }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.ControleEntrega.DesejaRealmenteAtualizarOsDadosDeRecebedor, function () {
        var dados = {
            Codigo: entrega.Codigo.val(),
            DadosRecebedorNome: entrega.DadosRecebedorNome.val(),
            DadosRecebedorCPF: entrega.DadosRecebedorCPF.val(),
            DadosRecebedorDataEntrega: entrega.DadosRecebedorDataEntrega.val(),
        }

        executarReST("ControleEntregaEntrega/AtualizarDadosRecebedor", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.DadosDeRecebedorAtualizadosComSucesso);
                    atualizarControleEntrega();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, null);
    });
}

function enviarImagemAssinatura() {
    if (!ValidarAcaoMultiCTe()) return;

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.ControleEntrega.DesejaRealmenteAtualizarImagemDeAssinatura, function () {
        var imagem = obterDataImagemAssinatura();

        if (imagem) {
            executarReST("ControleEntregaEntrega/EnviarImagemAssinatura", { Codigo: _entrega.Codigo.val(), Imagem: imagem }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ArquivoAnexadoComSucesso);
                        _entrega.Assinatura.val(retorno.Data);
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);

            });
        }
    });

}

function obterDataImagemAssinatura() {
    if (_entrega.Assinatura.val() != "") {
        return _entrega.Assinatura.val();
    }

    //var arquivo = document.getElementById(_entrega.DadosRecebedorArquivoAssinatura.id);

    //if (arquivo.files.length == 1) {
    //    var formData = new FormData();

    //    formData.append("Arquivo", arquivo.files[0]);
    //    formData.append("Descricao", "");

    //    return formData;
    //}

    return undefined;
}