/// <reference path="CancelamentoFatura.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridAnexoCancelamentoTituloCarga;
var _anexoCancelamentoTituloCarga;
var _modalAnexoCancelamentoTituloCarga;

/*
 * Declaração das Classes
 */

var AnexoCancelamentoTituloCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Anexo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Anexos = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Anexos, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Arquivo.val.subscribe(function (novoValor) { _anexoCancelamentoTituloCarga.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });
    this.Anexos.val.subscribe(recarregarGridAnexoCancelamentoTituloCarga);

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoCancelamentoTituloCargaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadAnexoCancelamentoTituloCarga() {
    _anexoCancelamentoTituloCarga = new AnexoCancelamentoTituloCarga();
    KoBindings(_anexoCancelamentoTituloCarga, "knockoutCancelamentoTituloAnexosCarga");

    _modalAnexoCancelamentoTituloCarga = new bootstrap.Modal(document.getElementById("divModalCancelamentoTituloAnexosCarga"), { backdrop: 'static' });

    loadGridAnexoCancelamentoTituloCarga();
}

function loadGridAnexoCancelamentoTituloCarga() {
    var linhasPorPaginas = 7;
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadAnexoCancelamentoTituloCargaClick, icone: "", visibilidade: visibleDownloadAnexoCancelamentoTituloCarga };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoCancelamentoTituloCargaClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 20, opcoes: [opcaoDownload, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "25%", className: "text-align-left" }
    ];

    _gridAnexoCancelamentoTituloCarga = new BasicDataTable(_anexoCancelamentoTituloCarga.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoCancelamentoTituloCarga.CarregarGrid([]);
}

function visibleDownloadAnexoCancelamentoTituloCarga(dataRow) {
    return !isNaN(dataRow.Codigo);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function gerenciarAnexosCancelamentoTituloCargaClick() {
    _modalAnexoCancelamentoTituloCarga.show();
}

function adicionarAnexoCancelamentoTituloCargaClick() {
    var arquivo = document.getElementById(_anexoCancelamentoTituloCarga.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoCancelamentoTituloCarga.Descricao.val(),
        NomeArquivo: _anexoCancelamentoTituloCarga.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    var anexos = obterAnexosCancelamentoTituloCarga();

    anexos.push(anexo);

    _anexoCancelamentoTituloCarga.Anexos.val(anexos.slice());

    LimparCampos(_anexoCancelamentoTituloCarga);

    arquivo.value = null;
}

function downloadAnexoCancelamentoTituloCargaClick(registroSelecionado) {
    executarDownload("TituloCancelamentoAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function removerAnexoCancelamentoTituloCargaClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoCancelamentoTituloCargaLocal(registroSelecionado);
    else {
        executarReST("TituloCancelamentoAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AnexoExcluidoComSucesso);
                    removerAnexoCancelamentoTituloCargaLocal(registroSelecionado);
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

function enviarAnexosCancelamentoTituloCarga(codigo, anexos) {
    var formData = obterFormDataAnexoCancelamentoTituloCarga(anexos);

    if (formData) {
        enviarArquivo("TituloCancelamentoAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _anexoCancelamentoTituloCarga.Anexos.val(retorno.Data.Anexos);

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

function enviarArquivosAnexadosCancelamentoTituloCarga(codigo) {
    var anexos = obterAnexosCancelamentoTituloCarga();

    enviarAnexosCancelamentoTituloCarga(codigo, anexos);
}

function limparCamposAnexoCancelamentoTituloCarga() {
    LimparCampos(_anexoCancelamentoTituloCarga);
    _anexoCancelamentoTituloCarga.Anexos.val(_anexoCancelamentoTituloCarga.Anexos.def);
    recarregarGridAnexoCancelamentoTituloCarga();
}

function obterAnexosCancelamentoTituloCarga() {
    return _anexoCancelamentoTituloCarga.Anexos.val().slice();
}

function obterFormDataAnexoCancelamentoTituloCarga(anexos) {
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

function recarregarGridAnexoCancelamentoTituloCarga() {
    var anexos = obterAnexosCancelamentoTituloCarga();

    _gridAnexoCancelamentoTituloCarga.CarregarGrid(anexos);
}

function removerAnexoCancelamentoTituloCargaLocal(registroSelecionado) {
    var listaAnexos = obterAnexosCancelamentoTituloCarga();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _anexoCancelamentoTituloCarga.Anexos.val(listaAnexos);
}