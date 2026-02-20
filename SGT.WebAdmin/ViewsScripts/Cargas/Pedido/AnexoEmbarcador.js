/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="DetalhesPedido.js" />

// #region Objetos Globais do Arquivo

var _detalhePedidoAnexoEmbarcador;
var _adicionarAnexoPedidoEmbarcador;
var _gridDetalhePedidoAnexoEmbarcador;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DetalhePedidoAnexoEmbarcador = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.Anexos = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Anexos, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: mostrarModalAdicionarAnexoPedidoEmbarcador, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

var AdicionarAnexoPedidoEmbarcador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Anexo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (novoValor) { _adicionarAnexoPedidoEmbarcador.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoPedidoEmbarcadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadDetalhePedidoAnexoEmbarcador() {

    LoadLocalizationResources("Cargas.Carga").then(function () {

        _detalhePedidoAnexoEmbarcador = new DetalhePedidoAnexoEmbarcador();
        KoBindings(_detalhePedidoAnexoEmbarcador, "divModalAnexosPedidoEmbarcador");

        _adicionarAnexoPedidoEmbarcador = new AdicionarAnexoPedidoEmbarcador();
        KoBindings(_adicionarAnexoPedidoEmbarcador, "divModalAdicionarAnexoPedidoEmbarcador");

        LocalizeCurrentPage();

        loadGridDetalhePedidoAnexoEmbarcador();

        LocalizeCurrentPage();
    });
};

function loadGridDetalhePedidoAnexoEmbarcador() {
    var linhasPorPaginas = 7;
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadDetalhePedidoAnexoClick, icone: "", visibilidade: true };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerDetalhePedidoEmbarcadorAnexoClick, icone: "", visibilidade: true };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 20, opcoes: [opcaoDownload, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "25%", className: "text-align-left" }
    ];

    _gridDetalhePedidoAnexoEmbarcador = new BasicDataTable(_detalhePedidoAnexoEmbarcador.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridDetalhePedidoAnexoEmbarcador.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarAnexoPedidoEmbarcadorClick() {
    var arquivo = document.getElementById(_adicionarAnexoPedidoEmbarcador.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);

    var anexo = {
        Codigo: guid(),
        Descricao: _adicionarAnexoPedidoEmbarcador.Descricao.val(),
        NomeArquivo: _adicionarAnexoPedidoEmbarcador.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    enviarDetalhePedidoEmbarcadorAnexos(_detalhePedidoAnexoEmbarcador.Codigo.val(), [anexo]);
}

function downloadDetalhePedidoEmbarcadorAnexoClick(registroSelecionado) {
    executarDownload("PedidoAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function mostrarModalAdicionarAnexoPedidoEmbarcador() {
    Global.abrirModal('divModalAdicionarAnexoPedidoEmbarcador');

    $("#divModalAdicionarAnexoPedidoEmbarcador").one('hidden.bs.modal', function () {
        LimparCampos(_adicionarAnexoPedidoEmbarcador);
        _adicionarAnexoPedidoEmbarcador.Arquivo.val("");
    });
}

function removerDetalhePedidoEmbarcadorAnexoClick(registroSelecionado) {
    executarReST("PedidoAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.AnexoExcluidoComSucesso);
                removerDetalhePedidoEmbarcadorAnexoLocal(registroSelecionado);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function recarregarGridDetalhePedidoEmbarcadorAnexo() {
    var anexos = obterDetalhePedidoEmbarcadorAnexos();

    _gridDetalhePedidoAnexoEmbarcador.CarregarGrid(anexos);
}

function carregarAnexosPedidoEmbarcador(codigo) {
    _detalhePedidoAnexoEmbarcador.Codigo.val(codigo);
    executarReST("PedidoAnexo/ObterAnexo", { Codigo: codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _gridDetalhePedidoAnexoEmbarcador.CarregarGrid(retorno.Data.Anexos);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function enviarDetalhePedidoEmbarcadorAnexos(codigo, anexos) {
    var formData = obterFormDataDetalhePedidoEmbarcadorAnexo(anexos);

    if (formData) {
        enviarArquivo("PedidoAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _gridDetalhePedidoAnexoEmbarcador.CarregarGrid(retorno.Data.Anexos);
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ArquivoAnexadoComSucesso);
                    $("#divModalAdicionarAnexoPedidoEmbarcador").modal("hide")
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Cargas.Carga.NaoFoiPossivelAnexarArquivo, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

function obterDetalhePedidoEmbarcadorAnexos() {
    return _gridDetalhePedidoAnexoEmbarcador.BuscarRegistros().slice();
}

function obterFormDataDetalhePedidoEmbarcadorAnexo(anexos) {
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

function removerDetalhePedidoEmbarcadorAnexoLocal(registroSelecionado) {
    var listaAnexos = obterDetalhePedidoEmbarcadorAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _gridDetalhePedidoAnexoEmbarcador.CarregarGrid(listaAnexos);
}

// #endregion Funções Privadas
