var _documentosPesagemTicketBalanca;
var _gridAnexoDocumentosPesagemTicketBalanca;
var _anexoDocumentosPesagemTicketBalanca;

var DocumentosPesagemTicketBalanca = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexoDocumentosPesagemTicketBalanca();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoDocumentoPesagemTicketBalancaModalClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Fornecedor) });
};

var AnexoDocumentosPesagemTicketBalanca = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, required: true });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoDocumentosPesagemTicketBalanca.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoDocumentosPesagemTicketBalancaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};

function LoadDocumentosPesagemTicketBalanca() {
    _anexoDocumentosPesagemTicketBalanca = new AnexoDocumentosPesagemTicketBalanca();
    KoBindings(_anexoDocumentosPesagemTicketBalanca, "knockoutAnexoDocumentosPesagemTicketBalanca");

    _documentosPesagemTicketBalanca = new DocumentosPesagemTicketBalanca();
    KoBindings(_documentosPesagemTicketBalanca, "knockoutAnexosTicketBalanca");

    LoadGridAnexoDocumentosPesagemTicketBalanca();
}

function LoadGridAnexoDocumentosPesagemTicketBalanca() {
    var linhasPorPaginas = 2;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoDocumentosPesagemTicketBalancaClick, icone: "", visibilidade: true };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoDocumentosPesagemTicketBalancaClick, icone: "", visibilidade: _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Fornecedor };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexoDocumentosPesagemTicketBalanca = new BasicDataTable(_documentosPesagemTicketBalanca.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoDocumentosPesagemTicketBalanca.CarregarGrid([]);
}

function adicionarAnexoDocumentoPesagemTicketBalancaModalClick() {
    Global.abrirModal('divModalAnexoDocumentosPesagemTicketBalanca');

    $("#divModalAnexoDocumentosPesagemTicketBalanca").one('hidden.bs.modal', function () {
        LimparCampos(_anexoDocumentosPesagemTicketBalanca);
    });
}

function adicionarAnexoDocumentosPesagemTicketBalancaClick() {
    var arquivo = document.getElementById(_anexoDocumentosPesagemTicketBalanca.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
        return;
    }

    if (!ValidarCamposObrigatorios(_anexoDocumentosPesagemTicketBalanca)) {
        exibirMensagem(tipoMensagem.atencao, "Campo vazio", "Você precisa informar uma descrição.")
        return;
    }

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoDocumentosPesagemTicketBalanca.Descricao.val(),
        NomeArquivo: _anexoDocumentosPesagemTicketBalanca.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    enviarAnexosDocumentosPesagemTicketBalanca(_documentosPesagem.CodigoFluxoGestaoPatio.val(), [anexo]);

    var listaAnexos = obterAnexosDocumentosPesagemTicketBalanca();

    listaAnexos.push(anexo);

    _documentosPesagemTicketBalanca.Anexos.val(listaAnexos.slice());

    _anexoDocumentosPesagemTicketBalanca.Arquivo.val("");

    Global.fecharModal("divModalAnexoDocumentosPesagemTicketBalanca");
}

function downloadAnexoDocumentosPesagemTicketBalancaClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };
    executarDownload("FluxoPatioDocumentosPesagemAnexoTicketBalanca/DownloadAnexo", dados);
}

function removerAnexoDocumentosPesagemTicketBalancaClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir esse anexo?", function () {
        if (isNaN(registroSelecionado.Codigo))
            removerAnexoDocumentosPesagemTicketBalancaLocal(registroSelecionado);
        else {
            executarReST("FluxoPatioDocumentosPesagemAnexoTicketBalanca/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                        removerAnexoDocumentosPesagemTicketBalancaLocal(registroSelecionado);
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

function buscarDocumentosPesagemTicketBalanca(codigoFluxo) {
    executarReST("FluxoPatioDocumentosPesagemAnexoTicketBalanca/ObterAnexo", { Codigo: codigoFluxo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _documentosPesagemTicketBalanca.Anexos.val(retorno.Data.Anexos);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

//#region Funções privadas

function obterAnexosDocumentosPesagemTicketBalanca() {
    return _documentosPesagemTicketBalanca.Anexos.val().slice();
}

function recarregarGridAnexoDocumentosPesagemTicketBalanca() {
    var anexos = obterAnexosDocumentosPesagemTicketBalanca();

    _gridAnexoDocumentosPesagemTicketBalanca.CarregarGrid(anexos);
}

function removerAnexoDocumentosPesagemTicketBalancaLocal(registroSelecionado) {
    var listaAnexos = obterAnexosDocumentosPesagemTicketBalanca();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _documentosPesagemTicketBalanca.Anexos.val(listaAnexos);
}

function enviarAnexosDocumentosPesagemTicketBalanca(codigo, anexos) {
    var formData = obterFormDataAnexosDocumentosPesagemTicketBalanca(anexos);

    if (formData) {
        enviarArquivo("FluxoPatioDocumentosPesagemAnexoTicketBalanca/AnexarArquivos", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");
                    buscarDocumentosPesagemTicketBalanca(codigo);
                }
                else
                    exibirMensagem(tipoMensagem.falha, (anexos.length > 1) ? "Não foi possível anexar os arquivos." : "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function obterFormDataAnexosDocumentosPesagemTicketBalanca(anexos) {
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