/// <reference path="TabelaFrete.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridAnexo;
var _anexo;

/*
 * Declaração das Classes
 */

var Anexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFrete.Anexo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Anexos = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.Anexos, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Arquivo.val.subscribe(function (novoValor) { _anexo.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });
    this.Anexos.val.subscribe(recarregarGridAnexo);

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Adicionar, visible: ko.observable(true) });
}


/*
 * Declaração das Funções de Inicialização
 */

function loadAnexo() {
    _anexo = new Anexo();
    KoBindings(_anexo, "knockoutTabelaFreteAnexos");

    loadGridAnexo();
}

function loadGridAnexo() {
    var linhasPorPaginas = 7;
    var opcaoDownload = { descricao: Localization.Resources.Fretes.TabelaFrete.Download, id: guid(), metodo: downloadAnexoClick, icone: "", visibilidade: isExibirOpcaoDownloadAnexo };
    var opcaoRemover = { descricao: Localization.Resources.Fretes.TabelaFrete.Remover, id: guid(), metodo: removerAnexoClick, icone: "", visibilidade: isPermitirGerenciarAnexos };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Fretes.TabelaFrete.Opcoes, tamanho: 20, opcoes: [opcaoDownload, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Fretes.TabelaFrete.Nome, width: "25%", className: "text-align-left" }
    ];

    _gridAnexo = new BasicDataTable(_anexo.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexo.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarAnexoClick() {
    if (!isPermitirGerenciarAnexos())
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.Anexos, Localization.Resources.Fretes.TabelaFrete.StatusNaoPermiteAdicionarAnexo);

    var arquivo = document.getElementById(_anexo.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.Anexos, Localization.Resources.Fretes.TabelaFrete.NenhumArquivoSelecionado);

    var anexo = {
        Codigo: guid(),
        Descricao: _anexo.Descricao.val(),
        NomeArquivo: _anexo.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    if (_tabelaFrete.Codigo.val() > 0)
        enviarAnexos(_tabelaFrete.Codigo.val(), [anexo]);
    else {
        var anexos = obterAnexos();

        anexos.push(anexo);

        _anexo.Anexos.val(anexos.slice());
    }

    LimparCampos(_anexo);

    arquivo.value = null;
}

function downloadAnexoClick(registroSelecionado) {
    executarDownload("TabelaFreteAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function removerAnexoClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoLocal(registroSelecionado);
    else if (isPermitirGerenciarAnexos()) {
        executarReST("TabelaFreteAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Fretes.TabelaFrete.Sucesso, Localization.Resources.Fretes.TabelaFrete.AnexoExcluidoComSucesso);
                    removerAnexoLocal(registroSelecionado);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.Aviso, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Fretes.TabelaFrete.Falha, retorno.Msg);
        });
    }
    else
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.Anexos, Localization.Resources.Fretes.TabelaFrete.StatusNaoPermiteRemoverAnexo);
}

/*
 * Declaração das Funções
 */

function enviarAnexos(codigo, anexos) {
    var formData = obterFormDataAnexo(anexos);

    if (formData) {
        enviarArquivo("TabelaFreteAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _anexo.Anexos.val(retorno.Data.Anexos);

                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Fretes.TabelaFrete.Sucesso, Localization.Resources.Fretes.TabelaFrete.ArquivoAnexadoComSucesso);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.NaoFoiPossivelAnexarOArquivo, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Fretes.TabelaFrete.Falha, retorno.Msg);
        });
    }
}

function enviarArquivosAnexados(codigo) {
    var anexos = obterAnexos();

    enviarAnexos(codigo, anexos);
}

function isExibirOpcaoDownloadAnexo(registroSelecionado) {
    return !isNaN(registroSelecionado.Codigo);
}

function isPermitirGerenciarAnexos() {
    return true;
}

function limparCamposAnexo() {
    LimparCampos(_anexo);

    _anexo.Anexos.val(_anexo.Anexos.def);
}

function obterAnexos() {
    return _anexo.Anexos.val().slice();
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

    _anexo.Anexos.val(listaAnexos);
}
