/// <reference path="ContratoFreteAcrescimoDesconto.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _anexo;
var _gridAnexos;
var _listaAnexo;
var _modalAnexoContratoFreteAcescimoDesconto;

/*
 * Declaração das Classes
 */

var Anexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable("Selecione um arquivo para anexar"), def: "Selecione um arquivo para anexar", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        if (nomeArquivoSelecionado == "")
            _anexo.NomeArquivo.val(_anexo.NomeArquivo.def);
        else
            _anexo.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};

var ListaAnexo = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexos();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexosClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadAnexo() {
    _anexo = new Anexo();
    KoBindings(_anexo, "knockoutAnexo");

    _listaAnexo = new ListaAnexo();
    KoBindings(_listaAnexo, "knockoutListaAnexos");

    loadGridAnexo();

    _modalAnexoContratoFreteAcescimoDesconto = new bootstrap.Modal(document.getElementById("divModalAnexo"), { backdrop: 'static', keyboard: true });
}

function loadGridAnexo() {
    var linhasPorPaginas = 5;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoClick, icone: "", visibilidade: isOpcaoDownloadVisivel };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoClick, icone: "", visibilidade: IsOpcaoRemoverVisivel };
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
    var arquivo = document.getElementById(_anexo.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

    var anexo = {
        Codigo: guid(),
        Descricao: _anexo.Descricao.val(),
        NomeArquivo: _anexo.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    if (_contratoFreteAcrescimoDesconto.Codigo.val() > 0)
        enviarAnexos(_contratoFreteAcrescimoDesconto.Codigo.val(), [anexo]);
    else {
        var listaAnexos = obterAnexos();

        listaAnexos.push(anexo);

        _listaAnexo.Anexos.val(listaAnexos.slice());
    }

    _anexo.Arquivo.val("");

    _modalAnexoContratoFreteAcescimoDesconto.hide();
}

function adicionarAnexosClick() {
    _modalAnexoContratoFreteAcescimoDesconto.show();
    $("#divModalAnexo").one('hidden.bs.modal', function () {
        LimparCampos(_anexo);
    });
}

function downloadAnexoClick(registroSelecionado) {
    executarDownload("ContratoFreteAcrescimoDescontoAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function removerAnexoClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoLocal(registroSelecionado);
    else {
        executarReST("ContratoFreteAcrescimoDescontoAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                    removerAnexoLocal(registroSelecionado);
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

function enviarAnexos(codigo, anexos) {
    var formData = obterFormDataAnexos(anexos);

    if (formData) {
        enviarArquivo("ContratoFreteAcrescimoDescontoAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _listaAnexo.Anexos.val(retorno.Data.Anexos);

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

function enviarArquivosAnexados(codigo) {
    var anexos = obterAnexos();

    enviarAnexos(codigo, anexos);
}

function isOpcaoDownloadVisivel(registroSelecionado) {
    return !isNaN(registroSelecionado.Codigo);
}

function limparAnexos() {
    _listaAnexo.Adicionar.visible(true);
    _listaAnexo.Anexos.val(new Array());
    recarregarGridAnexos();
}

function IsOpcaoRemoverVisivel() {
    return _contratoFreteAcrescimoDesconto.Situacao.val() === EnumSituacaoContratoFreteAcrescimoDesconto.Todos || _contratoFreteAcrescimoDesconto.Situacao.val() === EnumSituacaoContratoFreteAcrescimoDesconto.AgAprovacao || _contratoFreteAcrescimoDesconto.Situacao.val() === EnumSituacaoContratoFreteAcrescimoDesconto.SemRegra;
}

function obterAnexos() {
    return _listaAnexo.Anexos.val().slice();
}

function obterFormDataAnexos(anexos) {
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

function preencherAnexos(dadosAnexos) {
    _listaAnexo.Adicionar.visible(_contratoFreteAcrescimoDesconto.Situacao.val() === EnumSituacaoContratoFreteAcrescimoDesconto.Todos || _contratoFreteAcrescimoDesconto.Situacao.val() === EnumSituacaoContratoFreteAcrescimoDesconto.AgAprovacao || _contratoFreteAcrescimoDesconto.Situacao.val() === EnumSituacaoContratoFreteAcrescimoDesconto.SemRegra);
    _listaAnexo.Anexos.val(dadosAnexos);
}

function recarregarGridAnexos() {
    var anexos = obterAnexos();

    _gridAnexos.CarregarGrid(anexos);
}

function removerAnexoLocal(registroSelecionado) {
    var listaAnexos = obterAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _listaAnexo.Anexos.val(listaAnexos);
}