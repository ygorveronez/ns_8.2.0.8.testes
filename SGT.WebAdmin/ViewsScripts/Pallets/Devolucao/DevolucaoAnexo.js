var _anexoDevolucaoPallet;

var AnexoDevolucaoPallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, required: true });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoDevolucaoPallet.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: salvarAnexoDevolucaoPalletAnexo, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};

function loadDevolucaoPalletAnexo() {
    _anexoDevolucaoPallet = new AnexoDevolucaoPallet();
    KoBindings(_anexoDevolucaoPallet, "knockoutDevolucaoPalletsAnexo");
}

function downloadAnexoDevolucaoPalletClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("DevolucaoAnexo/DownloadAnexo", dados);
}

function excluirAnexoDevolucaoPalletClick(registroSelecionado) {
    executarReST("DevolucaoAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                removerAnexoDevolucaoPalletLocal(registroSelecionado);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function salvarAnexoDevolucaoPalletAnexo() {
    if (!ValidarCamposObrigatorios(_anexoDevolucaoPallet)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    var formData = new FormData();

    formData.append("Arquivo", document.getElementById(_anexoDevolucaoPallet.Arquivo.id).files[0]);
    formData.append("Descricao", _anexoDevolucaoPallet.Descricao.val());
    
    enviarArquivo("DevolucaoAnexo/AnexarArquivos", { Codigo: _baixaPallets.Codigo.val() }, formData, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                adicionarAnexoDevolucaoPalletLocal(retorno.Data.Anexos);
                LimparCampos(_anexoDevolucaoPallet);
                Global.fecharModal("divModalDevolucaoPalletsAnexo");
                exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");
            }
            else
                exibirMensagem(tipoMensagem.falha, "Não foi possível anexar o arquivo.", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function removerAnexoDevolucaoPalletLocal(registroSelecionado) {
    var anexosGrid = _gridAnexoDevolucaoPallets.BuscarRegistros();

    for (var i = 0; i < anexosGrid.length; i++) {
        if (anexosGrid[i].Codigo == registroSelecionado.Codigo) {
            anexosGrid.splice(i, 1);
            break;
        }
    }
    
    _gridAnexoDevolucaoPallets.CarregarGrid(anexosGrid);
}

function adicionarAnexoDevolucaoPalletLocal(anexos) {
    var anexosGrid = _gridAnexoDevolucaoPallets.BuscarRegistros();
    anexos.forEach(function (anexo) {
        anexosGrid.push({
            Codigo: anexo.Codigo,
            Descricao: anexo.Descricao,
            NomeArquivo: anexo.NomeArquivo
        });
    });
    
    _gridAnexoDevolucaoPallets.CarregarGrid(anexosGrid);
}

function recarregarGridAnexosDevolucaoPallet() {
    var anexosGrid = _baixaPallets.Anexos.val();
    _gridAnexoDevolucaoPallets.CarregarGrid(anexosGrid);
}