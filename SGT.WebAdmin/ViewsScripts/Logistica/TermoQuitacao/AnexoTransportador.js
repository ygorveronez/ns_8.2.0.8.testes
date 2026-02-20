/// <reference path="TermoQuitacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTermoQuitacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _anexoTransportador;
var _gridAnexoTransportador;
var _listaAnexoTransportador;

/*
 * Declaração das Classes
 */

var AnexoTransportador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoTransportador.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoTransportadorClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

var ListaAnexoTransportador = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexoTransportador();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoTransportadorModalClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAnexoTransportador() {
    _anexoTransportador = new AnexoTransportador();
    KoBindings(_anexoTransportador, "knockoutAnexoTransportador");

    _listaAnexoTransportador = new ListaAnexoTransportador();
    KoBindings(_listaAnexoTransportador, "knockoutListaAnexosTransportador");

    loadGridAnexoTransportador();
}

function loadGridAnexoTransportador() {
    var linhasPorPaginas = 2;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoTransportadorClick, icone: "" };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoTransportadorClick, icone: "", visibilidade: isOpcaoRemoverAnexoTransportadorVisivel };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexoTransportador = new BasicDataTable(_listaAnexoTransportador.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoTransportador.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarAnexoTransportadorClick() {
    if (!isPermitirGerenciarAnexosTransportador())
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Não é possível anexar arquivos na atual situação.");

    var arquivo = document.getElementById(_anexoTransportador.Arquivo.id);

    if (arquivo.files.length == 0)
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoTransportador.Descricao.val(),
        NomeArquivo: _anexoTransportador.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    enviarAnexosTransportador(_termoQuitacao.Codigo.val(), [anexo]);

    _anexoTransportador.Arquivo.val("");

    Global.fecharModal('divModalAnexoTransportador');
}

function adicionarAnexoTransportadorModalClick() {
    Global.abrirModal('divModalAnexoTransportador');
    $("#divModalAnexoTransportador").one('hidden.bs.modal', function () {
        LimparCampos(_anexoTransportador);
    });
}

function downloadAnexoTransportadorClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("TermoQuitacaoAnexoTransportador/DownloadAnexo", dados);
}

function removerAnexoTransportadorClick(registroSelecionado) {
    if (!isPermitirGerenciarAnexosTransportador())
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Não é possível remover arquivos na atual situação.");

    executarReST("TermoQuitacaoAnexoTransportador/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                removerAnexoTransportadorLocal(registroSelecionado);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções Públicas
 */

function isAnexosTransportadorInformados() {
    return (obterAnexosTransportador().length > 0);
}

function limparAnexoTransportador() {
    _listaAnexoTransportador.Adicionar.visible(false);
    _listaAnexoTransportador.Anexos.val(new Array());
}

function preencherAnexoTransportador(dadosAnexos) {
    _listaAnexoTransportador.Adicionar.visible(isPermitirGerenciarAnexosTransportador());
    _listaAnexoTransportador.Anexos.val(dadosAnexos);
}

/*
 * Declaração das Funções Privadas
 */

function enviarAnexosTransportador(codigo, anexos) {
    var formData = obterFormDataAnexosTransportador(anexos);

    if (formData) {
        enviarArquivo("TermoQuitacaoAnexoTransportador/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");

                    _listaAnexoTransportador.Anexos.val(retorno.Data.Anexos);
                }
                else
                    exibirMensagem(tipoMensagem.falha, (anexos.length > 1) ? "Não foi possível anexar os arquivos." : "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function isOpcaoRemoverAnexoTransportadorVisivel() {
    return isPermitirGerenciarAnexosTransportador();
}

function isPermitirGerenciarAnexosTransportador() {
    if (!isAcessoTransportador())
        return false;

    return (
        (_termoQuitacao.Situacao.val() == EnumSituacaoTermoQuitacao.AguardandoAceiteTransportador) ||
        (_termoQuitacao.Situacao.val() == EnumSituacaoTermoQuitacao.AprovacaoRejeitada)
    );
}

function obterAnexosTransportador() {
    return _listaAnexoTransportador.Anexos.val().slice();
}

function obterFormDataAnexosTransportador(anexos) {
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

function recarregarGridAnexoTransportador() {
    var anexos = obterAnexosTransportador();

    _gridAnexoTransportador.CarregarGrid(anexos);
}

function removerAnexoTransportadorLocal(registroSelecionado) {
    var listaAnexos = obterAnexosTransportador();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _listaAnexoTransportador.Anexos.val(listaAnexos);
}
