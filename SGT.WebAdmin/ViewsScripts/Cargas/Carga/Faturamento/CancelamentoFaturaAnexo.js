/// <reference path="CancelamentoFatura.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridAnexoCancelamentoFaturaCarga;
var _anexoCancelamentoFaturaCarga;
var _modalAnexoCancelamentoFaturaCarga;

/*
 * Declaração das Classes
 */

var AnexoCancelamentoFaturaCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Anexo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Anexos = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Anexos, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Arquivo.val.subscribe(function (novoValor) { _anexoCancelamentoFaturaCarga.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });
    this.Anexos.val.subscribe(recarregarGridAnexoCancelamentoFaturaCarga);

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoCancelamentoFaturaCargaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadAnexoCancelamentoFaturaCarga() {
    _anexoCancelamentoFaturaCarga = new AnexoCancelamentoFaturaCarga();
    KoBindings(_anexoCancelamentoFaturaCarga, "knockoutCancelamentoFaturaAnexosCarga");

    _modalAnexoCancelamentoFaturaCarga = new bootstrap.Modal(document.getElementById("divModalCancelamentoFaturaAnexosCarga"), { backdrop: 'static' });

    loadGridAnexoCancelamentoFaturaCarga();
}

function loadGridAnexoCancelamentoFaturaCarga() {
    var linhasPorPaginas = 7;
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadAnexoCancelamentoFaturaCargaClick, icone: "", visibilidade: visibleDownloadAnexoCancelamentoFaturaCarga };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoCancelamentoFaturaCargaClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 20, opcoes: [opcaoDownload, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "25%", className: "text-align-left" }
    ];

    _gridAnexoCancelamentoFaturaCarga = new BasicDataTable(_anexoCancelamentoFaturaCarga.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoCancelamentoFaturaCarga.CarregarGrid([]);
}

function visibleDownloadAnexoCancelamentoFaturaCarga(dataRow) {
    return !isNaN(dataRow.Codigo);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function gerenciarAnexosCancelamentoFaturaCargaClick() {
    _modalAnexoCancelamentoFaturaCarga.show();
}

function adicionarAnexoCancelamentoFaturaCargaClick() {
    var arquivo = document.getElementById(_anexoCancelamentoFaturaCarga.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoCancelamentoFaturaCarga.Descricao.val(),
        NomeArquivo: _anexoCancelamentoFaturaCarga.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    var anexos = obterAnexosCancelamentoFaturaCarga();

    anexos.push(anexo);

    _anexoCancelamentoFaturaCarga.Anexos.val(anexos.slice());

    LimparCampos(_anexoCancelamentoFaturaCarga);

    arquivo.value = null;
}

function downloadAnexoCancelamentoFaturaCargaClick(registroSelecionado) {
    executarDownload("FaturaCancelamentoAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function removerAnexoCancelamentoFaturaCargaClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoCancelamentoFaturaCargaLocal(registroSelecionado);
    else {
        executarReST("FaturaCancelamentoAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AnexoExcluidoComSucesso);
                    removerAnexoCancelamentoFaturaCargaLocal(registroSelecionado);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

/*
 * Declaração das Funções
 */

function enviarAnexosCancelamentoFaturaCarga(codigo, anexos) {
    var formData = obterFormDataAnexoCancelamentoFaturaCarga(anexos);

    if (formData) {
        enviarArquivo("FaturaCancelamentoAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _anexoCancelamentoFaturaCarga.Anexos.val(retorno.Data.Anexos);

                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ArquivoAnexadoComSucesso);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarOsArquivos, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

function enviarArquivosAnexadosCancelamentoFaturaCarga(codigo) {
    var anexos = obterAnexosCancelamentoFaturaCarga();

    enviarAnexosCancelamentoFaturaCarga(codigo, anexos);
}

function limparCamposAnexoCancelamentoFaturaCarga() {
    LimparCampos(_anexoCancelamentoFaturaCarga);
    _anexoCancelamentoFaturaCarga.Anexos.val(_anexoCancelamentoFaturaCarga.Anexos.def);
    recarregarGridAnexoCancelamentoFaturaCarga();
}

function obterAnexosCancelamentoFaturaCarga() {
    return _anexoCancelamentoFaturaCarga.Anexos.val().slice();
}

function obterFormDataAnexoCancelamentoFaturaCarga(anexos) {
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

function recarregarGridAnexoCancelamentoFaturaCarga() {
    var anexos = obterAnexosCancelamentoFaturaCarga();

    _gridAnexoCancelamentoFaturaCarga.CarregarGrid(anexos);
}

function removerAnexoCancelamentoFaturaCargaLocal(registroSelecionado) {
    var listaAnexos = obterAnexosCancelamentoFaturaCarga();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _anexoCancelamentoFaturaCarga.Anexos.val(listaAnexos);
}