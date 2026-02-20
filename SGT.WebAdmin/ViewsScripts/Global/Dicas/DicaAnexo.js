/// <reference path="Dica.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _dicaAnexo;
var _gridDicaAnexos;
var _listaDicaAnexo;
var _modalDicaAnexo;

/*
 * Declaração das Classes
 */

var DicaAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable("Selecione um arquivo para anexar"), def: "Selecione um arquivo para anexar", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        if (nomeArquivoSelecionado == "")
            _dicaAnexo.NomeArquivo.val(_dicaAnexo.NomeArquivo.def);
        else
            _dicaAnexo.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarDicaAnexoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};

var ListaDicaAnexo = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexos();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarDicaAnexosClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadDicaAnexo() {
    _dicaAnexo = new DicaAnexo();
    KoBindings(_dicaAnexo, "knockoutDicaAnexo");

    _listaDicaAnexo = new ListaDicaAnexo();
    KoBindings(_listaDicaAnexo, "knockoutDicaListaAnexos");

    loadGridDicaAnexo();

    _modalDicaAnexo = new bootstrap.Modal(document.getElementById("divModalDicaAnexo"), { backdrop: 'static', keyboard: true });
}

function loadGridDicaAnexo() {
    var linhasPorPaginas = 5;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadDicaAnexoClick, icone: "", visibilidade: isOpcaoDownloadDicaAnexoVisivel };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerDicaAnexoClick, icone: "", visibilidade: IsOpcaoRemoverDicaAnexoVisivel };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridDicaAnexos = new BasicDataTable(_listaDicaAnexo.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridDicaAnexos.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarDicaAnexoClick() {
    var arquivo = document.getElementById(_dicaAnexo.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

    var anexo = {
        Codigo: guid(),
        Descricao: _dicaAnexo.Descricao.val(),
        NomeArquivo: _dicaAnexo.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };
    
    if (_dica.Codigo.val() > 0)
        enviarDicaAnexos(_dica.Codigo.val(), [anexo]);
    else {
        var listaAnexos = obterDicaAnexos();

        listaAnexos.push(anexo);

        _listaDicaAnexo.Anexos.val(listaAnexos.slice());
    }

    _dicaAnexo.Arquivo.val("");

    _modalDicaAnexo.hide();
}

function adicionarDicaAnexosClick() {
    _modalDicaAnexo.show();
    $("#divModalDicaAnexo").one('hidden.bs.modal', function () {
        LimparCampos(_dicaAnexo);
    });
}

function downloadDicaAnexoClick(registroSelecionado) {
    executarDownload("DicaAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function removerDicaAnexoClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerDicaAnexoLocal(registroSelecionado);
    else {
        executarReST("DicaAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                    removerDicaAnexoLocal(registroSelecionado);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

/*
 * Declaração das Funções
 */

function enviarDicaAnexos(codigo, anexos) {
    var formData = obterFormDataDicaAnexos(anexos);

    if (formData) {
        enviarArquivo("DicaAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _listaDicaAnexo.Anexos.val(retorno.Data.Anexos);

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo anexado com sucesso");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function enviarArquivosAnexadosDica(codigo) {
    var anexos = obterDicaAnexos();

    enviarDicaAnexos(codigo, anexos);
}

function isOpcaoDownloadDicaAnexoVisivel(registroSelecionado) {
    return !isNaN(registroSelecionado.Codigo);
}

function limparDicaAnexos() {
    _listaDicaAnexo.Adicionar.visible(true);
    _listaDicaAnexo.Anexos.val(new Array());
    recarregarGridAnexos();
}

function IsOpcaoRemoverDicaAnexoVisivel() {
    return _habilitarEdicao;
}

function obterDicaAnexos() {
    return _listaDicaAnexo.Anexos.val().slice();
}

function obterFormDataDicaAnexos(anexos) {
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

function preencherDicaAnexos(dadosAnexos) {
    _listaDicaAnexo.Adicionar.visible(_habilitarEdicao);
    _listaDicaAnexo.Anexos.val(dadosAnexos);
}

function recarregarGridAnexos() {
    var anexos = obterDicaAnexos();

    _gridDicaAnexos.CarregarGrid(anexos);
}

function removerDicaAnexoLocal(registroSelecionado) {
    var listaAnexos = obterDicaAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _listaDicaAnexo.Anexos.val(listaAnexos);
}