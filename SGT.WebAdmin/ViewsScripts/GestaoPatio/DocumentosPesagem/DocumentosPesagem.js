var _documentosPesagem;
var _gridAnexoDocumentosPesagem;
var _anexoDocumentosPesagem;

var DocumentosPesagem = function () {
    this.CodigoFluxoGestaoPatio = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.CodigoCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });

    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexoDocumentosPesagem();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoDocumentoPesagemModalClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Fornecedor) });
};

var AnexoDocumentosPesagem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, required: true });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoDocumentosPesagem.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoDocumentosPesagemClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};

function LoadDocumentosPesagem() {
    _anexoDocumentosPesagem = new AnexoDocumentosPesagem();
    KoBindings(_anexoDocumentosPesagem, "knockoutAnexoDocumentosPesagem");

    _documentosPesagem = new DocumentosPesagem();
    KoBindings(_documentosPesagem, "knockoutDocumentosPesagem");

    LoadAnexoFornecedorPesagem();
    LoadDocumentosPesagemDevolucao();
    LoadDocumentosPesagemTicketBalanca();
    LoadDocumentosPesagemNotaFiscalComplementar();
    LoadGridAnexoDocumentosPesagem();
    LoadDocumentosPesagemNFRemessaIndustrializacao();
}

function LoadGridAnexoDocumentosPesagem() {
    var linhasPorPaginas = 2;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoDocumentosPesagemClick, icone: "", visibilidade: true };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoDocumentosPesagemClick, icone: "", visibilidade: _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Fornecedor };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexoDocumentosPesagem = new BasicDataTable(_documentosPesagem.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoDocumentosPesagem.CarregarGrid([]);
}

function adicionarAnexoDocumentoPesagemModalClick() {
    Global.abrirModal('divModalAnexoDocumentosPesagem');

    $("#divModalAnexoDocumentosPesagem").one('hidden.bs.modal', function () {
        LimparCampos(_anexoDocumentosPesagem);
    });
}

function adicionarAnexoDocumentosPesagemClick() {
    var arquivo = document.getElementById(_anexoDocumentosPesagem.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
        return;
    }

    if (!ValidarCamposObrigatorios(_anexoDocumentosPesagem)) {
        exibirMensagem(tipoMensagem.atencao, "Campo vazio", "Você precisa informar uma descrição.")
        return;
    }

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoDocumentosPesagem.Descricao.val(),
        NomeArquivo: _anexoDocumentosPesagem.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    enviarAnexosDocumentosPesagem(_documentosPesagem.CodigoFluxoGestaoPatio.val(), [anexo]);

    var listaAnexos = obterAnexosDocumentosPesagem();

    listaAnexos.push(anexo);

    _documentosPesagem.Anexos.val(listaAnexos.slice());

    _anexoDocumentosPesagem.Arquivo.val("");

    Global.fecharModal('divModalAnexoDocumentosPesagem');
}

function downloadAnexoDocumentosPesagemClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };
    executarDownload("FluxoPatioDocumentosPesagemAnexo/DownloadAnexo", dados);
}

function removerAnexoDocumentosPesagemClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir esse anexo?", function () {
        if (isNaN(registroSelecionado.Codigo))
            removerAnexoDocumentosPesagemLocal(registroSelecionado);
        else {
            executarReST("FluxoPatioDocumentosPesagemAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                        removerAnexoDocumentosPesagemLocal(registroSelecionado);
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

function buscarDocumentosPesagem(codigoFluxo) {
    executarReST("FluxoPatioDocumentosPesagemAnexo/ObterAnexo", { Codigo: codigoFluxo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _documentosPesagem.Anexos.val(retorno.Data.Anexos);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

//#region Funções privadas

function obterAnexosDocumentosPesagem() {
    return _documentosPesagem.Anexos.val().slice();
}

function recarregarGridAnexoDocumentosPesagem() {
    var anexos = obterAnexosDocumentosPesagem();

    _gridAnexoDocumentosPesagem.CarregarGrid(anexos);
}

function removerAnexoDocumentosPesagemLocal(registroSelecionado) {
    var listaAnexos = obterAnexosDocumentosPesagem();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _documentosPesagem.Anexos.val(listaAnexos);
}

function enviarAnexosDocumentosPesagem(codigo, anexos) {
    var formData = obterFormDataAnexosDocumentosPesagem(anexos);

    if (formData) {
        enviarArquivo("FluxoPatioDocumentosPesagemAnexo/AnexarArquivos", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");
                    buscarDocumentosPesagem(codigo);
                }
                else
                    exibirMensagem(tipoMensagem.falha, (anexos.length > 1) ? "Não foi possível anexar os arquivos." : "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function obterFormDataAnexosDocumentosPesagem(anexos) {
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