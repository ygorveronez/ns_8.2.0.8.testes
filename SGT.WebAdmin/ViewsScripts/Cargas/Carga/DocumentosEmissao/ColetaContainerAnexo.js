/// <reference path="CargaColetaContainer.js" />

// #region Objetos Globais do Arquivo

var _coletaContainerAnexo;
var _gridColetaContainerAnexo;
var _pesquisaAnexo;

// #endregion Objetos Globais do Arquivo

// #region Classes

var ColetaContainerAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Anexo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (novoValor) { _coletaContainerAnexo.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });

    this.Adicionar = PropertyEntity({ eventClick: adicionarColetaContainerAnexoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

var pesquisaAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

// #endregion Classes

// #region Funções de Inicialização

function loadColetaContainerAnexo(carga, containerCarga) {
    _coletaContainerAnexo = new ColetaContainerAnexo();
    KoBindings(_coletaContainerAnexo, "divModalColetaContainerAnexo");

    _pesquisaAnexo = new pesquisaAnexo();
    KoBindings(_pesquisaAnexo, "knockoutPesquisaAnexo");

    _coletaContainerAnexo.CodigoCarga.val(carga.Codigo.val())

    _pesquisaAnexo.Codigo.val(containerCarga.Codigo.val());
    _pesquisaAnexo.CodigoCarga.val(carga.Codigo.val());

    loadGridColetaContainerAnexo();
}

function loadGridColetaContainerAnexo() {
    var linhasPorPaginas = 7;
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadColetaContainerAnexoClick, icone: "", visibilidade: true };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerColetaContainerAnexoClick, icone: "", visibilidade: true };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 20, opcoes: [opcaoDownload, opcaoRemover] };

    _gridColetaContainerAnexo = new GridView(_containerCarga.Anexos.idGrid, "ColetaContainerAnexo/PesquisaAnexo", _pesquisaAnexo, menuOpcoes, null, linhasPorPaginas);
    _gridColetaContainerAnexo.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarColetaContainerAnexoClick() {

    var arquivo = document.getElementById(_coletaContainerAnexo.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);

    var anexo = {
        Codigo: guid(),
        Descricao: _coletaContainerAnexo.Descricao.val(),
        NomeArquivo: _coletaContainerAnexo.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    if (_containerCarga != null && _containerCarga.Codigo.val() > 0)
        enviarColetaContainerAnexos(_containerCarga.Codigo.val(), [anexo]);
    else
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Cargas.Carga.NenhumContainerSelecionadoParaEstaCarga);
}

function downloadColetaContainerAnexoClick(registroSelecionado) {
    executarDownload("ColetaContainerAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function removerColetaContainerAnexoClick(registroSelecionado) {

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaExcluirAnexo, function () {
        executarReST("ColetaContainerAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.AnexoExcluidoComSucesso);
                    _gridColetaContainerAnexo.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function adicionarColetaContainerAnexo() {
    exibirModalColetaContainerAnexo();
}

function recarregarGridColetaContainerAnexo() {
    _gridColetaContainerAnexo.CarregarGrid();
}

// #endregion Funções Públicas

// #region Funções Privadas

function enviarColetaContainerAnexos(codigo, anexos) {
    var formData = obterFormDataColetaContainerAnexo(anexos);

    if (formData) {
        enviarArquivo("ColetaContainerAnexo/AnexarArquivos", { Codigo: codigo, CodigoCarga: _containerCarga.Carga.val() }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _containerCarga.Anexos.val(retorno.Data.Anexos);
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ArquivoAnexadoComSucesso);
                    fecharModalColetaContainerAnexo();
                    _gridColetaContainerAnexo.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.Carga.NaoFoiPossivelAnexarArquivo, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

function exibirModalColetaContainerAnexo() {
    Global.abrirModal("divModalColetaContainerAnexo");
    $("#divModalColetaContainerAnexo").one('hidden.bs.modal', function () {
        LimparCampos(_coletaContainerAnexo);
        _coletaContainerAnexo.Arquivo.val("");
    });
}

function fecharModalColetaContainerAnexo() {
    Global.fecharModal("divModalColetaContainerAnexo");
}

function obterFormDataColetaContainerAnexo(anexos) {
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


// #endregion Funções Privadas
