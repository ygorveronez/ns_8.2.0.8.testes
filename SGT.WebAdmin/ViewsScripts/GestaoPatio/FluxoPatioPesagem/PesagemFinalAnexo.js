/// <reference path="Pesagem.js" />

//#region Declaração de Variáveis Globais do Arquivo

var _anexoPesagemFinal;
var _gridAnexoPesagemFinal;
var _listaAnexoPesagemFinal;

//#endregion

//#region Declarações dos Objetos

var AnexoPesagemFinal = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, required: true });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoPesagemFinal.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoPesagemFinalClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

var ListaAnexoPesagemFinal = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexoPesagemFinal();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoPesagemFinalModalClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(true) });
}

//#endregion

//#region Inicializadores

function loadAnexoPesagemFinal() {
    _anexoPesagemFinal = new AnexoPesagemFinal();
    KoBindings(_anexoPesagemFinal, "knockoutAnexoPesagemFinal");

    _listaAnexoPesagemFinal = new ListaAnexoPesagemFinal();
    KoBindings(_listaAnexoPesagemFinal, "knockoutListaAnexosPesagemFinal");

    loadGridAnexoPesagemFinal();
}

function loadGridAnexoPesagemFinal() {
    var linhasPorPaginas = 2;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoPesagemFinalClick, icone: "", visibilidade: true };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoPesagemFinalClick, icone: "", visibilidade: true };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexoPesagemFinal = new BasicDataTable(_listaAnexoPesagemFinal.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoPesagemFinal.CarregarGrid([]);
}

//#endregion

//#region Eventos

function downloadAnexoPesagemFinalClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };
    executarDownload("GuaritaPesagemFinalAnexo/DownloadAnexo", dados);
}

function removerAnexoPesagemFinalClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoPesagemFinalLocal(registroSelecionado);
    else {
        executarReST("GuaritaPesagemFinalAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                    removerAnexoPesagemFinalLocal(registroSelecionado);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function adicionarAnexoPesagemFinalModalClick() {
    Global.abrirModal('divModalAnexoPesagemFinal');

    $("#divModalAnexoPesagemFinal").one('hidden.bs.modal', function () {
        LimparCampos(_anexoPesagemFinal);
    });
}

function adicionarAnexoPesagemFinalClick() {
    var arquivo = document.getElementById(_anexoPesagemFinal.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
        return;
    }

    if (!ValidarCamposObrigatorios(_anexoPesagemFinal)) {
        exibirMensagem(tipoMensagem.atencao, "Campo vazio", "Você precisa informar uma descrição.")
        return;
    }

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoPesagemFinal.Descricao.val(),
        NomeArquivo: _anexoPesagemFinal.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    var listaAnexos = obterAnexosPesagemFinal();

    listaAnexos.push(anexo);

    _listaAnexoPesagemFinal.Anexos.val(listaAnexos.slice());

    _anexoPesagemFinal.Arquivo.val("");

    Global.fecharModal('divModalAnexoPesagemFinal');
}

//#endregion

//#region Funções privadas

function obterAnexosPesagemFinal() {
    return _listaAnexoPesagemFinal.Anexos.val().slice();
}

function recarregarGridAnexoPesagemFinal() {
    var anexos = obterAnexosPesagemFinal();

    _gridAnexoPesagemFinal.CarregarGrid(anexos);
}

function removerAnexoPesagemFinalLocal(registroSelecionado) {
    var listaAnexos = obterAnexosPesagemFinal();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _listaAnexoPesagemFinal.Anexos.val(listaAnexos);
}

function enviarArquivosAnexadosPesagemFinal(codigo) {
    var anexos = obterAnexosPesagemFinal();
    
    anexos = anexos.filter(function (anexo) { return !(anexo.Codigo > 0) });

    if (anexos.length > 0)
        enviarAnexosPesagemFinal(codigo, anexos);
}

function enviarAnexosPesagemFinal(codigo, anexos) {
    var formData = obterFormDataAnexosPesagemFinal(anexos);

    if (formData) {
        enviarArquivo("GuaritaPesagemFinalAnexo/AnexarArquivos", { Codigo: codigo }, formData, function (retorno) {
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

function obterFormDataAnexosPesagemFinal(anexos) {
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