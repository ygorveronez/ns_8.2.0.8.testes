/// <reference path="BiddingConvite.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _anexo, _gridAnexo, _listaAnexo;

/*
 * Declaração das Classes
 */

var Anexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, required: true });
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
        recarregarGridAnexo();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoModalClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAnexo() {
    _anexo = new Anexo();
    KoBindings(_anexo, "knockoutAnexoConvite");

    _listaAnexo = new ListaAnexo();
    KoBindings(_listaAnexo, "knockoutListaAnexosConvite");

    loadGridAnexo();
}

function loadGridAnexo() {
    const linhasPorPaginas = 2;
    const opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoClick, icone: "", visibilidade: true };
    const opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoClick, icone: "", visibilidade: true };
    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexo = new BasicDataTable(_listaAnexo.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexo.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarAnexoClick() {

    const arquivo = document.getElementById(_anexo.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
        return;
    }

    if (!ValidarCamposObrigatorios(_anexo)) {
        exibirMensagem(tipoMensagem.atencao, "Campo vazio", "Você precisa informar uma descrição.")
        return;
    }

    const anexo = {
        Codigo: guid(),
        Descricao: _anexo.Descricao.val(),
        NomeArquivo: _anexo.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    if (_biddingConvite.Codigo.val() > 0)
        enviarAnexos(_biddingConvite.Codigo.val(), [anexo]);

    const listaAnexos = obterAnexos();

    listaAnexos.push(anexo);

    _listaAnexo.Anexos.val(listaAnexos.slice());

    _anexo.Arquivo.val("");

    Global.fecharModal('divModalAnexo');
}

function adicionarAnexoModalClick() {
    Global.abrirModal('divModalAnexo');
    $("#divModalAnexo").one('hidden.bs.modal', function () {
        LimparCampos(_anexo);
    });
}

function downloadAnexoClick(registroSelecionado) {
    const dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("BiddingConviteAnexo/DownloadAnexo", dados);
}

function removerAnexoClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoLocal(registroSelecionado);
    else {
        executarReST("BiddingConviteAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
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
 * Declaração das Funções Públicas
 */

function enviarArquivosAnexados(codigo) {
    const anexos = obterAnexos();

    if (anexos.length > 0)
        enviarAnexos(codigo, anexos);
}

function isAnexosInformados() {
    return (obterAnexos().length > 0);
}

function limparAnexo() {
    //_listaAnexo.Adicionar.visible(!isAcessoTransportador());
    _listaAnexo.Anexos.val(new Array());
}

function preencherAnexo(dadosAnexos) {
    //_listaAnexo.Adicionar.visible(isPermitirGerenciarAnexos());
    _listaAnexo.Anexos.val(dadosAnexos);
}

/*
 * Declaração das Funções Privadas
 */

function enviarAnexos(codigo, anexos) {
    const formData = obterFormDataAnexos(anexos);

    if (!formData)
        return;

    enviarArquivo("BiddingConviteAnexo/AnexarArquivos", { Codigo: codigo }, formData, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                return exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");
            
            return exibirMensagem(tipoMensagem.falha, (anexos.length > 1) ? "Não foi possível anexar os arquivos." : "Não foi possível anexar o arquivo.", retorno.Msg);
        }
        exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function isOpcaoDownloadAnexoVisivel(registroSelecionado) {
    return !isNaN(registroSelecionado.Codigo);
}

function obterAnexos() {
    return _listaAnexo.Anexos.val().slice();
}

function obterFormDataAnexos(anexos) {
    if (anexos.length > 0) {
        const formData = new FormData();

        anexos.forEach(function (anexo) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        });

        return formData;
    }

    return undefined;
}

function recarregarGridAnexo() {
    const anexos = obterAnexos();

    _gridAnexo.CarregarGrid(anexos);
}

function removerAnexoLocal(registroSelecionado) {
    const listaAnexos = obterAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _listaAnexo.Anexos.val(listaAnexos);
}
