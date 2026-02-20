/// <reference path="Analise.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridAnexo;
var _anexoAnalise;
var _pesquisaAnexo;

/*
 * Declaração das Classes
 */

var pesquisaAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var Anexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, visible: ko.observable(true) });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Arquivo.val.subscribe(function (novoValor) { _anexoAnalise.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });
    this.Anexos.val.subscribe(recarregarGridAnexo);

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoAnaliseClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};


/*
 * Declaração das Funções de Inicialização
 */

function loadAnexoAnalise() {
    _anexoAnalise = new Anexo();
    KoBindings(_anexoAnalise, "knockoutAnaliseAnexo");

    _pesquisaAnexo = new pesquisaAnexo();
    KoBindings(_pesquisaAnexo, "knockoutPesquisaAnaliseAnexo");

    loadGridAnexo();
}

function loadGridAnexo() {
    var linhasPorPaginas = 7;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoAnaliseClick, icone: "" };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoAnaliseClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 20, opcoes: [opcaoDownload, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "25%", className: "text-align-left" }
    ];

    _gridAnexo = new BasicDataTable(_anexoAnalise.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexo.CarregarGrid([]);

}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarAnexoAnaliseClick() {

    var arquivo = document.getElementById(_anexoAnalise.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoAnalise.Descricao.val(),
        NomeArquivo: _anexoAnalise.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    if (_analise.CodigoAnalise.val() > 0)
        enviarAnexos(_analise.CodigoAnalise.val(), [anexo]);
    else {
        var anexos = obterAnexos();

        anexos.push(anexo);

        _anexoAnalise.Anexos.val(anexos.slice());
    }

    arquivo.value = null;
}

function downloadAnexoAnaliseClick(registroSelecionado) {
    executarDownload("ChamadoAnaliseAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function removerAnexoAnaliseClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoLocal(registroSelecionado);
    else {
        executarReST("ChamadoAnaliseAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
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
    var formData = obterFormDataAnexo(anexos);

    if (formData) {
        enviarArquivo("ChamadoAnaliseAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _anexoAnalise.Anexos.val(retorno.Data.Anexos);

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

function limparCamposAnexo() {
    LimparCampos(_anexoAnalise);
    _anexoAnalise.Anexos.val(_anexoAnalise.Anexos.def);
    recarregarGridAnexo();
}

function obterAnexos() {
    return _anexoAnalise.Anexos.val().slice();
}

function obterFormDataAnexo(anexos) {
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

function recarregarGridAnexo() {
    var anexos = obterAnexos();

    _gridAnexo.CarregarGrid(anexos);
}

function removerAnexoLocal(registroSelecionado) {
    var listaAnexos = obterAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _anexoAnalise.Anexos.val(listaAnexos);
}