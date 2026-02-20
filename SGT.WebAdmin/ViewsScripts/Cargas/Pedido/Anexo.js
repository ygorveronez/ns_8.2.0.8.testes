/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="DetalhesPedido.js" />

// #region Objetos Globais do Arquivo

var _detalhePedidoAnexo;
var _gridDetalhePedidoAnexo;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DetalhePedidoAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Anexo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (novoValor) { _detalhePedidoAnexo.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });

    this.Adicionar = PropertyEntity({ eventClick: adicionarDetalhePedidoAnexoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadDetalhePedidoAnexo() {
    _detalhePedidoAnexo = new DetalhePedidoAnexo();
    KoBindings(_detalhePedidoAnexo, "divModalDetalhesPedidoAnexo");

    loadGridDetalhePedidoAnexo();
}

function loadGridDetalhePedidoAnexo() {
    var linhasPorPaginas = 7;
    var auditar = { descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), metodo: OpcaoAuditoria("PedidoAnexo"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadDetalhePedidoAnexoClick, icone: "", visibilidade: isExibirOpcaoDownloadDetalhePedidoAnexo };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerDetalhePedidoAnexoClick, icone: "", visibilidade: isPermitirRemoverDetalhesPedidoAnexos };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 20, opcoes: [opcaoDownload, opcaoRemover, auditar] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "25%", className: "text-align-left" }
    ];

    _gridDetalhePedidoAnexo = new BasicDataTable(_detalhePedidoContainer.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridDetalhePedidoAnexo.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarDetalhePedidoAnexoClick() {
    if (!isPermitirGerenciarDetalhePedidoAnexos())
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, "Status não permite adicionar anexo");

    var arquivo = document.getElementById(_detalhePedidoAnexo.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, "Nenhum arquivo selecionado.");

    var anexo = {
        Codigo: guid(),
        Descricao: _detalhePedidoAnexo.Descricao.val(),
        NomeArquivo: _detalhePedidoAnexo.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    if (_detalhePedidoContainer.Pedidos != null && _detalhePedidoContainer.Pedidos != undefined && _detalhePedidoContainer.Pedidos.slice().length > 0 && _detalhePedidoContainer.Pedidos.slice(0)[0].Codigo.val() > 0)
        enviarDetalhePedidoAnexos(_detalhePedidoContainer.Pedidos.slice(0)[0].Codigo.val(), [anexo]);
    else
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, "Nenhum pedido selecionado para esta carga.");
}

function downloadDetalhePedidoAnexoClick(registroSelecionado) {
    executarDownload("PedidoAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function removerDetalhePedidoAnexoClick(registroSelecionado) {
    if (isPermitirRemoverDetalhesPedidoAnexos()) {
        executarReST("PedidoAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Anexo excluído com sucesso");
                    removerDetalhePedidoAnexoLocal(registroSelecionado);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
    else
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, "Status não permite remover anexo");
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function adicionarDetalhePedidoAnexo() {
    exibirModalDetalhePedidoAnexo();
}

function recarregarGridDetalhePedidoAnexo() {
    var anexos = obterDetalhePedidoAnexos();

    _gridDetalhePedidoAnexo.CarregarGrid(anexos);
}

// #endregion Funções Públicas

// #region Funções Privadas

function enviarDetalhePedidoAnexos(codigo, anexos) {
    var formData = obterFormDataDetalhePedidoAnexo(anexos);

    if (formData) {
        enviarArquivo("PedidoAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _detalhePedidoContainer.Anexos.val(retorno.Data.Anexos);
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Arquivo anexado com sucesso");
                    fecharModalDetalhePedidoAnexo();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

function exibirModalDetalhePedidoAnexo() {
    Global.abrirModal('divModalDetalhesPedidoAnexo');
    $("#divModalDetalhesPedidoAnexo").one('hidden.bs.modal', function () {
        LimparCampos(_detalhePedidoAnexo);
        _detalhePedidoAnexo.Arquivo.val("");
    });
}

function fecharModalDetalhePedidoAnexo() {
    Global.fecharModal('divModalDetalhesPedidoAnexo');
}

function isExibirOpcaoDownloadDetalhePedidoAnexo(registroSelecionado) {
    return !isNaN(registroSelecionado.Codigo);
}

function isPermitirGerenciarDetalhePedidoAnexos() {
    return true;
}

function isPermitirRemoverDetalhesPedidoAnexos() {
    
    if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_RemoverAnexosCarga, _PermissoesPersonalizadasCarga))
        return true;
    else
        return false;
}

function obterDetalhePedidoAnexos() {
    return _detalhePedidoContainer.Anexos.val().slice();
}

function obterFormDataDetalhePedidoAnexo(anexos) {
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

function removerDetalhePedidoAnexoLocal(registroSelecionado) {
    var listaAnexos = obterDetalhePedidoAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _detalhePedidoContainer.Anexos.val(listaAnexos);
}

// #endregion Funções Privadas
