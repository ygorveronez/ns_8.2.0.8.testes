//#region Declaração de Variáveis Globais do Arquivo

var _gridAnexoFornecedorPesagem;
var _anexosFornecedorAnexo;
var _listaAnexoFornecedorPesagem;

//#endregion

//#region Declarações dos Objetos

var AnexoProdutorAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, required: true });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexosProdutorAnexo.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoProdutorClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}


var ListaAnexoFornecedorPesagem = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexoFornecedorPesagem();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoProdutorModalClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Fornecedor) });
}

//#endregion

//#region Inicializadores

function LoadAnexoFornecedorPesagem() {
    _listaAnexoFornecedorPesagem = new ListaAnexoFornecedorPesagem();
    KoBindings(_listaAnexoFornecedorPesagem, "knockoutAnexosFornecedor");

    _anexosProdutorAnexo = new AnexoProdutorAnexo();
    KoBindings(_anexosProdutorAnexo, "knockoutAnexosProdutorAdicionarAnexo");

    LoadGridAnexoFornecedorPesagem();
}

function LoadGridAnexoFornecedorPesagem() {
    var linhasPorPaginas = 2;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoFornecedorPesagemClick, icone: "", visibilidade: true };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoFornecedorPesagemClick, icone: "", visibilidade: _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Fornecedor };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexoFornecedorPesagem = new BasicDataTable(_listaAnexoFornecedorPesagem.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoFornecedorPesagem.CarregarGrid([]);
}

//#endregion

//#region Eventos

function downloadAnexoFornecedorPesagemClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };
    executarDownload("AnexosProdutorAnexo/DownloadAnexo", dados);
}

function adicionarAnexoProdutorModalClick() {
    Global.abrirModal('divModalAdicionarAnexoProdutor');

    $("#divModalAdicionarAnexoProdutor").one('hidden.bs.modal', function () {
        LimparCampos(_anexosProdutorAnexo);
    });
}

function adicionarAnexoProdutorClick() {
    var arquivo = document.getElementById(_anexosProdutorAnexo.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
        return;
    }

    if (!ValidarCamposObrigatorios(_anexosProdutorAnexo)) {
        exibirMensagem(tipoMensagem.atencao, "Campo vazio", "Você precisa informar uma descrição.")
        return;
    }

    var anexo = {
        Codigo: guid(),
        Descricao: _anexosProdutorAnexo.Descricao.val(),
        NomeArquivo: _anexosProdutorAnexo.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    enviarAnexosFornecedorPesagem(_documentosPesagem.CodigoCarga.val(), [anexo]);

    var listaAnexos = obterAnexosFornecedorPesagem();

    listaAnexos.push(anexo);

    _listaAnexoFornecedorPesagem.Anexos.val(listaAnexos.slice());

    _anexosProdutorAnexo.Arquivo.val("");

    Global.fecharModal("divModalAdicionarAnexoProdutor");
}

//#endregion

//#region Métodos Públicos

function buscarDocumentosFornecedor(codigoCarga) {
    executarReST("AnexosProdutorAnexo/ObterAnexo", { Codigo: codigoCarga }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _listaAnexoFornecedorPesagem.Anexos.val(retorno.Data.Anexos);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function removerAnexoFornecedorPesagemClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir esse anexo?", function () {
        if (isNaN(registroSelecionado.Codigo))
            removerAnexoPesagemFornecedorLocal(registroSelecionado);
        else {
            executarReST("AnexosProdutorAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                        removerAnexoPesagemFornecedorLocal(registroSelecionado);
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

//#endregion

//#region Métodos Privados

function removerAnexoPesagemFornecedorLocal(registroSelecionado) {
    var listaAnexos = obterAnexosFornecedorPesagem();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _listaAnexoFornecedorPesagem.Anexos.val(listaAnexos);
}

function obterAnexosFornecedorPesagem() {
    return _listaAnexoFornecedorPesagem.Anexos.val().slice();
}

function recarregarGridAnexoFornecedorPesagem() {
    var anexos = obterAnexosFornecedorPesagem();

    _gridAnexoFornecedorPesagem.CarregarGrid(anexos);
}

function enviarAnexosFornecedorPesagem(codigo, anexos) {
    var formData = obterFormDataAnexosPesagemFornecedor(anexos);

    if (formData) {
        enviarArquivo("AnexosProdutorAnexo/AnexarArquivos", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");
                    Global.fecharModal("divModalAnexosProdutor");
                }
                else
                    exibirMensagem(tipoMensagem.falha, (anexos.length > 1) ? "Não foi possível anexar os arquivos." : "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function obterFormDataAnexosPesagemFornecedor(anexos) {
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