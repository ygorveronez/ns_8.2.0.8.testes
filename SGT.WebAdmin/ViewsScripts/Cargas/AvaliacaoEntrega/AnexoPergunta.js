/// <reference path="AvaliacaoEntrega.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridAnexo;
var _anexoPergunta;
var _cabecalhoModal;

/*
 * Declaração das Classes
 */

var CabecalhoModal = function () {
    this.CodigoPergunta = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0)})
    this.Titulo = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Gerais.Geral.Anexos });
}

var AnexoPergunta = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.GridModal = PropertyEntity({ idGrid: guid(), text: Localization.Resources.Gerais.Geral.ArquivosAnexados });

    this.Arquivo.val.subscribe(function (novoValor) { _anexoPergunta.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoPerguntaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};


/*
 * Declaração das Funções de Inicialização
 */

function loadAnexo() {
    _anexoPergunta = new AnexoPergunta();
    KoBindings(_anexoPergunta, "knockoutPerguntaAnexo");

    _cabecalhoModal = new CabecalhoModal();
    KoBindings(_cabecalhoModal, "knockoutCabecalhoPerguntaAnexo");

    loadGridAnexo();
}

function loadGridAnexo() {
    var linhasPorPaginas = 7;
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Baixar, id: guid(), metodo: downloadAnexoClick, icone: "fal fa-download", visibilidade: visibleDownloadAnexo };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoClick, icone: "fal fa-trash" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 20, opcoes: [opcaoDownload, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "25%", className: "text-align-left" }
    ];

    _gridAnexo = new BasicDataTable(_anexoPergunta.GridModal.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexo.CarregarGrid([]);

}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarAnexoPerguntaClick() {

    var arquivo = document.getElementById(_anexoPergunta.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoPergunta.Descricao.val(),
        NomeArquivo: _anexoPergunta.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    for (let i in _avaliacaoEntrega.Perguntas()) {
        if (_avaliacaoEntrega.Perguntas()[i].Codigo.val() == _cabecalhoModal.CodigoPergunta.val()) {

            let anexos = obterAnexos();
            anexos.push(anexo);
            _avaliacaoEntrega.Perguntas()[i].Anexos.val(anexos.slice());

            break;
        }
    }

    limparCamposAnexo();
}

function downloadAnexoClick(registroSelecionado) {
    executarDownload("AvaliacaoEntregaPerguntaAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function removerAnexoClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoLocal(registroSelecionado);
    else {
        executarReST("AvaliacaoEntregaPerguntaAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
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
        enviarArquivo("AvaliacaoEntregaPerguntaAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {

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

function limparCamposAnexo() {
    LimparCampos(_anexoPergunta);
    recarregarGridAnexo();
    _anexoPergunta.Arquivo.val("");
}

function obterAnexos() {
    for (let i in _avaliacaoEntrega.Perguntas()) {
        if (_avaliacaoEntrega.Perguntas()[i].Codigo.val() == _cabecalhoModal.CodigoPergunta.val())
            return _avaliacaoEntrega.Perguntas()[i].Anexos.val().slice();
    }

    return new Array();
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
    let listaAnexos = obterAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _avaliacaoEntrega.Perguntas().forEach(function (pergunta, i) {
        if (_cabecalhoModal.CodigoPergunta.val() == pergunta.Codigo.val()) {
            pergunta.Anexos.val(listaAnexos.slice());
        }
    });

    recarregarGridAnexo();
}

function visibleDownloadAnexo(anexo) {
    return !isNaN(anexo.Codigo);
}