/// <reference path="Pesagem.js" />

//#region Declaração de Variáveis Globais do Arquivo

var _anexoPesagem;
var _gridAnexoPesagem;
var _listaAnexoPesagem;

//#endregion

//#region Declarações dos Objetos

var AnexoPesagem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, required: true });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoPesagem.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoPesagemClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

var ListaAnexoPesagem = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexoPesagem();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoPesagemModalClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(true) });
}

//#endregion

//#region Inicializadores

function LoadAnexoPesagem() {
    _anexoPesagem = new AnexoPesagem();
    KoBindings(_anexoPesagem, "knockoutAnexoPesagem");

    _listaAnexoPesagem = new ListaAnexoPesagem();
    KoBindings(_listaAnexoPesagem, "knockoutListaAnexosPesagem");

    LoadGridAnexoPesagem();
}

function LoadGridAnexoPesagem() {
    var linhasPorPaginas = 2;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoPesagemClick, icone: "", visibilidade: true };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoPesagemClick, icone: "", visibilidade: true };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexoPesagem = new BasicDataTable(_listaAnexoPesagem.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoPesagem.CarregarGrid([]);
}

//#endregion

//#region Eventos

function downloadAnexoPesagemClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };
    executarDownload("GuaritaPesagemAnexo/DownloadAnexo", dados);
}

function removerAnexoPesagemClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoPesagemLocal(registroSelecionado);
    else {
        executarReST("GuaritaPesagemAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                    removerAnexoPesagemLocal(registroSelecionado);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function adicionarAnexoPesagemModalClick() {
    Global.abrirModal('divModalAnexoPesagem');

    $("#divModalAnexoPesagem").one('hidden.bs.modal', function () {
        LimparCampos(_anexoPesagem);
    });
}

function adicionarAnexoPesagemClick() {
    var arquivo = document.getElementById(_anexoPesagem.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
        return;
    }

    if (!ValidarCamposObrigatorios(_anexoPesagem)) {
        exibirMensagem(tipoMensagem.atencao, "Campo vazio", "Você precisa informar uma descrição.")
        return;
    }

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoPesagem.Descricao.val(),
        NomeArquivo: _anexoPesagem.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    var listaAnexos = obterAnexosPesagem();

    listaAnexos.push(anexo);

    _listaAnexoPesagem.Anexos.val(listaAnexos.slice());

    _anexoPesagem.Arquivo.val("");

    Global.fecharModal('divModalAnexoPesagem');
}

//#endregion

//#region Funções privadas

function obterAnexosPesagem() {
    return _listaAnexoPesagem.Anexos.val().slice();
}

function recarregarGridAnexoPesagem() {
    var anexos = obterAnexosPesagem();

    _gridAnexoPesagem.CarregarGrid(anexos);
}

function removerAnexoPesagemLocal(registroSelecionado) {
    var listaAnexos = obterAnexosPesagem();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _listaAnexoPesagem.Anexos.val(listaAnexos);
}

function enviarArquivosAnexadosPesagem(codigo) {
    var anexos = obterAnexosPesagem();
    
    anexos = anexos.filter(function (anexo) { return !(anexo.Codigo > 0) });

    if (anexos.length > 0)
        enviarAnexosPesagem(codigo, anexos);
}

function enviarAnexosPesagem(codigo, anexos) {
    var formData = obterFormDataAnexosPesagem(anexos);

    if (formData) {
        enviarArquivo("GuaritaPesagemAnexo/AnexarArquivos", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");
                }
                else
                    exibirMensagem(tipoMensagem.falha, (anexos.length > 1) ? "Não foi possível anexar os arquivos." : "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function obterFormDataAnexosPesagem(anexos) {
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