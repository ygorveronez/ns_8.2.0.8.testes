var _documentosPesagemDevolucao;
var _gridAnexoDocumentosPesagemDevolucao;
var _anexoDocumentosPesagemDevolucao;

var DocumentosPesagemDevolucao = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexoDocumentosPesagemDevolucao();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoDocumentoPesagemDevolucaoModalClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Fornecedor) });
};

var AnexoDocumentosPesagemDevolucao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, required: true });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoDocumentosPesagemDevolucao.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoDocumentosPesagemDevolucaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};

function LoadDocumentosPesagemDevolucao() {
    _anexoDocumentosPesagemDevolucao = new AnexoDocumentosPesagemDevolucao();
    KoBindings(_anexoDocumentosPesagemDevolucao, "knockoutAnexoDocumentosPesagemDevolucao");

    _documentosPesagemDevolucao = new DocumentosPesagemDevolucao();
    KoBindings(_documentosPesagemDevolucao, "knockoutAnexosDevolucao");

    LoadGridAnexoDocumentosPesagemDevolucao();
}

function LoadGridAnexoDocumentosPesagemDevolucao() {
    var linhasPorPaginas = 2;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoDocumentosPesagemDevolucaoClick, icone: "", visibilidade: true };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoDocumentosPesagemDevolucaoClick, icone: "", visibilidade: _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Fornecedor };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexoDocumentosPesagemDevolucao = new BasicDataTable(_documentosPesagemDevolucao.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoDocumentosPesagemDevolucao.CarregarGrid([]);
}

function adicionarAnexoDocumentoPesagemDevolucaoModalClick() {
    Global.abrirModal('divModalAnexoDocumentosPesagemDevolucao');

    $("#divModalAnexoDocumentosPesagemDevolucao").one('hidden.bs.modal', function () {
        LimparCampos(_anexoDocumentosPesagemDevolucao);
    });
}

function adicionarAnexoDocumentosPesagemDevolucaoClick() {
    var arquivo = document.getElementById(_anexoDocumentosPesagemDevolucao.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
        return;
    }

    if (!ValidarCamposObrigatorios(_anexoDocumentosPesagemDevolucao)) {
        exibirMensagem(tipoMensagem.atencao, "Campo vazio", "Você precisa informar uma descrição.")
        return;
    }

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoDocumentosPesagemDevolucao.Descricao.val(),
        NomeArquivo: _anexoDocumentosPesagemDevolucao.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    enviarAnexosDocumentosPesagemDevolucao(_documentosPesagem.CodigoFluxoGestaoPatio.val(), [anexo]);

    var listaAnexos = obterAnexosDocumentosPesagemDevolucao();

    listaAnexos.push(anexo);

    _documentosPesagemDevolucao.Anexos.val(listaAnexos.slice());

    _anexoDocumentosPesagemDevolucao.Arquivo.val("");

    Global.fecharModal("divModalAnexoDocumentosPesagemDevolucao");
}

function downloadAnexoDocumentosPesagemDevolucaoClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };
    executarDownload("FluxoPatioDocumentosPesagemAnexoDevolucao/DownloadAnexo", dados);
}

function removerAnexoDocumentosPesagemDevolucaoClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir esse anexo?", function () {
        if (isNaN(registroSelecionado.Codigo))
            removerAnexoDocumentosPesagemDevolucaoLocal(registroSelecionado);
        else {
            executarReST("FluxoPatioDocumentosPesagemAnexoDevolucao/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                        removerAnexoDocumentosPesagemDevolucaoLocal(registroSelecionado);
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        }
    });
}

function buscarDocumentosPesagemDevolucao(codigoFluxo) {
    executarReST("FluxoPatioDocumentosPesagemAnexoDevolucao/ObterAnexo", { Codigo: codigoFluxo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _documentosPesagemDevolucao.Anexos.val(retorno.Data.Anexos);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

//#region Funções privadas

function obterAnexosDocumentosPesagemDevolucao() {
    return _documentosPesagemDevolucao.Anexos.val().slice();
}

function recarregarGridAnexoDocumentosPesagemDevolucao() {
    var anexos = obterAnexosDocumentosPesagemDevolucao();

    _gridAnexoDocumentosPesagemDevolucao.CarregarGrid(anexos);
}

function removerAnexoDocumentosPesagemDevolucaoLocal(registroSelecionado) {
    var listaAnexos = obterAnexosDocumentosPesagemDevolucao();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _documentosPesagemDevolucao.Anexos.val(listaAnexos);
}

function enviarAnexosDocumentosPesagemDevolucao(codigo, anexos) {
    var formData = obterFormDataAnexosDocumentosPesagemDevolucao(anexos);

    if (formData) {
        enviarArquivo("FluxoPatioDocumentosPesagemAnexoDevolucao/AnexarArquivos", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");
                    buscarDocumentosPesagemDevolucao(codigo);
                }
                else
                    exibirMensagem(tipoMensagem.falha, (anexos.length > 1) ? "Não foi possível anexar os arquivos." : "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function obterFormDataAnexosDocumentosPesagemDevolucao(anexos) {
    if (anexos.length > 0) {
        var formData = new FormData();

        anexos.forEach(function (anexo) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        });

        return formData;
    }

    return undefined;
}

//#endregion