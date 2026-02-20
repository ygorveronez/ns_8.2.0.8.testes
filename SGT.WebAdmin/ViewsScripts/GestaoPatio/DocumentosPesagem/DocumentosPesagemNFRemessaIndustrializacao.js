var _documentosPesagemNFRemessaIndustrializacao;
var _gridAnexoDocumentosNFRemessaIndustrializacao;
var _anexoDocumentosNFRemessaIndustrializacao;

var DocumentosPesagemNFRemessaIndustrializacao = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexoDocumentosNFRemessaIndustrializacao();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoDocumentoPesagemNFRemessaIndustrializacaoModalClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Fornecedor) });
};

var AnexoDocumentosPesagemNFRemessaIndustrializacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, required: true });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoDocumentosNFRemessaIndustrializacao.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoDocumentosPesagemNFRemessaIndustrializacaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};

function LoadDocumentosPesagemNFRemessaIndustrializacao() {
    _anexoDocumentosNFRemessaIndustrializacao = new AnexoDocumentosPesagemNFRemessaIndustrializacao();
    KoBindings(_anexoDocumentosNFRemessaIndustrializacao, "knockoutAnexoDocumentosPesagemNFRemessaIndustrializacao");

    _documentosPesagemNFRemessaIndustrializacao = new DocumentosPesagemNFRemessaIndustrializacao();
    KoBindings(_documentosPesagemNFRemessaIndustrializacao, "knockoutAnexosNFRemessaIndustrializacao");

    LoadGridAnexoDocumentosPesagemNFRemessaIndustrializacao();
}

function LoadGridAnexoDocumentosPesagemNFRemessaIndustrializacao() {
    var linhasPorPaginas = 2;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoDocumentosPesagemNFRemessaIndustrializacaoClick, icone: "", visibilidade: true };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoDocumentosPesagemNFRemessaIndustrializacaoClick, icone: "", visibilidade: _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Fornecedor };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexoDocumentosNFRemessaIndustrializacao = new BasicDataTable(_documentosPesagemNFRemessaIndustrializacao.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoDocumentosNFRemessaIndustrializacao.CarregarGrid([]);
}

function adicionarAnexoDocumentoPesagemNFRemessaIndustrializacaoModalClick() {
    Global.abrirModal('divModalAnexoDocumentosNFRemessaIndustrializacao');

    $("#divModalAnexoDocumentosNFRemessaIndustrializacao").one('hidden.bs.modal', function () {
        LimparCampos(_anexoDocumentosNFRemessaIndustrializacao);
    });
}

function adicionarAnexoDocumentosPesagemNFRemessaIndustrializacaoClick() {
    var arquivo = document.getElementById(_anexoDocumentosNFRemessaIndustrializacao.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
        return;
    }

    if (!ValidarCamposObrigatorios(_anexoDocumentosNFRemessaIndustrializacao)) {
        exibirMensagem(tipoMensagem.atencao, "Campo vazio", "Você precisa informar uma descrição.")
        return;
    }

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoDocumentosNFRemessaIndustrializacao.Descricao.val(),
        NomeArquivo: _anexoDocumentosNFRemessaIndustrializacao.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    enviarAnexosDocumentosPesagemNFRemessaIndustrializacao(_documentosPesagem.CodigoFluxoGestaoPatio.val(), [anexo]);

    var listaAnexos = obterAnexosDocumentosPesagemNFRemessaIndustrializacao();

    listaAnexos.push(anexo);

    _documentosPesagemNFRemessaIndustrializacao.Anexos.val(listaAnexos.slice());

    _anexoDocumentosNFRemessaIndustrializacao.Arquivo.val("");

    Global.fecharModal("divModalAnexoDocumentosNFRemessaIndustrializacao");
}

function downloadAnexoDocumentosPesagemNFRemessaIndustrializacaoClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };
    executarDownload("FluxoPatioDocumentosPesagemAnexoNFRemessaIndustrializacao/DownloadAnexo", dados);
}

function removerAnexoDocumentosPesagemNFRemessaIndustrializacaoClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir esse anexo?", function () {
        if (isNaN(registroSelecionado.Codigo))
            removerAnexoDocumentosPesagemNFRemessaIndustrializacaoLocal(registroSelecionado);
        else {
            executarReST("FluxoPatioDocumentosPesagemAnexoNFRemessaIndustrializacao/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                        removerAnexoDocumentosPesagemNFRemessaIndustrializacaoLocal(registroSelecionado);
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

function buscarDocumentosPesagemNFRemessaIndustrializacao(codigoFluxo) {
    executarReST("FluxoPatioDocumentosPesagemAnexoNFRemessaIndustrializacao/ObterAnexo", { Codigo: codigoFluxo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _documentosPesagemNFRemessaIndustrializacao.Anexos.val(retorno.Data.Anexos);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

//#region Funções privadas

function obterAnexosDocumentosPesagemNFRemessaIndustrializacao() {
    return _documentosPesagemNFRemessaIndustrializacao.Anexos.val().slice();
}

function recarregarGridAnexoDocumentosNFRemessaIndustrializacao() {
    var anexos = obterAnexosDocumentosPesagemNFRemessaIndustrializacao();

    _gridAnexoDocumentosNFRemessaIndustrializacao.CarregarGrid(anexos);
}

function removerAnexoDocumentosPesagemNFRemessaIndustrializacaoLocal(registroSelecionado) {
    var listaAnexos = obterAnexosDocumentosPesagemNFRemessaIndustrializacao();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _documentosPesagemNFRemessaIndustrializacao.Anexos.val(listaAnexos);
}

function enviarAnexosDocumentosPesagemNFRemessaIndustrializacao(codigo, anexos) {
    var formData = obterFormDataAnexosDocumentosPesagemNFRemessaIndustrializacao(anexos);

    if (formData) {
        enviarArquivo("FluxoPatioDocumentosPesagemAnexoNFRemessaIndustrializacao/AnexarArquivos", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");
                    buscarDocumentosPesagemNFRemessaIndustrializacao(codigo);
                }
                else
                    exibirMensagem(tipoMensagem.falha, (anexos.length > 1) ? "Não foi possível anexar os arquivos." : "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function obterFormDataAnexosDocumentosPesagemNFRemessaIndustrializacao(anexos) {
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