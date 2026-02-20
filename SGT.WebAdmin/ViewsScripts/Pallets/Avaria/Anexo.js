/// <reference path="../../Enumeradores/EnumSituacaoAvariaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _anexo;
var _gridAnexos;
var _listaAnexo;
var _urlAnexar = "AvariaAnexo/AnexarArquivos?callback=?";
var _urlDownload = "AvariaAnexo/DownloadAnexo";

/*
 * Declaração das Classes
 */

var Anexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexo.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

var ListaAnexo = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexos();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexosClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAnexo() {
    _anexo = new Anexo();
    KoBindings(_anexo, "knockoutAnexo");

    _listaAnexo = new ListaAnexo();
    KoBindings(_listaAnexo, "knockoutListaAnexos");

    loadGridAnexo();
}

function loadGridAnexo() {
    var linhasPorPaginas = 2;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoClick, icone: "", visibilidade: isOpcaoDownloadVisivel };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoClick, icone: "", visibilidade: isOpcaoRemoverVisivel };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexos = new BasicDataTable(_listaAnexo.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexos.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarAnexoClick() {
    if (isSituacaoPermiteGerenciarAnexos()) {
        var arquivo = document.getElementById(_anexo.Arquivo.id);

        if (arquivo.files.length == 0)
            exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
        else {
            var anexo = {
                Codigo: guid(),
                Descricao: _anexo.Descricao.val(),
                NomeArquivo: _anexo.NomeArquivo.val(),
                Arquivo: arquivo.files[0]
            };
            
            var listaAnexos = obterAnexos();

            listaAnexos.push(anexo);

            _listaAnexo.Anexos.val(listaAnexos.slice());
            _anexo.Arquivo.val("");

            Global.fecharModal('divModalAnexo');
        }
    }
    else
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Não é possível anexar arquivos na atual situação.");
}

function adicionarAnexosClick() {
    Global.abrirModal('divModalAnexo');
    $("#divModalAnexo").one('hidden.bs.modal', function () {
        LimparCampos(_anexo);
    });
}

function downloadAnexoClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };

    executarDownload(_urlDownload, dados);
}

function removerAnexoClick(registroSelecionado) {
    var listaAnexos = obterAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _listaAnexo.Anexos.val(listaAnexos);
}

/*
 * Declaração das Funções
 */

function enviarArquivosAnexados(codigo, anexos) {
    if (anexos.length > 0) {
        var dados = { Codigo: codigo };
        var formData = obterFormDataAnexos(anexos);

        enviarArquivo(_urlAnexar, dados, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");
                else
                    exibirMensagem(tipoMensagem.falha, (anexos.length > 1) ? "Não foi possível anexar os arquivos." : "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function isOpcaoDownloadVisivel(registroSelecionado) {
    return !isNaN(registroSelecionado.Codigo);
}

function isOpcaoRemoverVisivel() {
    return isSituacaoPermiteGerenciarAnexos();
}

function isSituacaoPermiteGerenciarAnexos() {
    return _avariaPallet.Situacao.val() === EnumSituacaoAvariaPallet.Todas;
}

function limparAnexos() {
    _listaAnexo.Adicionar.visible(true);
    _listaAnexo.Anexos.val(new Array());
}

function obterAnexos() {
    return _listaAnexo.Anexos.val().slice();
}

function obterFormDataAnexos(anexos) {
    var formData = new FormData();

    anexos.forEach(function (anexo) {
        formData.append("Arquivo", anexo.Arquivo);
        formData.append("Descricao", anexo.Descricao);
    });

    return formData;
}

function preencherAnexos(dadosAnexos) {
    _listaAnexo.Adicionar.visible(false);
    _listaAnexo.Anexos.val(dadosAnexos);
}

function recarregarGridAnexos() {
    var anexos = obterAnexos();

    _gridAnexos.CarregarGrid(anexos);
}
