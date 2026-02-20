/// <reference path="Infracao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoInfracao.js" />
/// <reference path="../../Enumeradores/EnumTipoHistoricoInfracao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridHistoricoInfracao;
var _historicoInfracao;
var _cadastroHistoricoInfracao;
var _modalCadastroHistoricoInfracao;
/*
 * Declaração das Classes
 */

var CadastroHistoricoInfracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.Data = PropertyEntity({ text: "*Data/Hora: ", getType: typesKnockout.dateTime, required: true });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.TipoHistoricoInfracao = PropertyEntity({ val: ko.observable(EnumTipoHistoricoInfracao.EmAberto), options: EnumTipoHistoricoInfracao.obterOpcoesPesquisa(), def: EnumTipoHistoricoInfracao.EmAberto, text: "*Tipo: ", required: true });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _cadastroHistoricoInfracao.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarHistoricoClick, type: types.event, text: ko.observable("Adicionar") });
}

var HistoricoInfracao = function () {
    this.ListaHistorico = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaHistorico.val.subscribe(function () {
        recarregarGridHistorico();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarHistoricoModalClick, type: types.event, text: ko.observable("Adicionar"), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridHistoricoInfracao() {
    var linhasPorPaginas = 5;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoHistoricoClick, icone: "", visibilidade: isHistoricoPossuiAnexo };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerHistoricoClick, icone: "", visibilidade: isSituacaoPermiteGerenciarHistorico };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 21, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoAnexo", visible: false },
        { data: "Tipo", title: "Tipo", width: "20%", className: "text-align-center" },
        { data: "Data", title: "Data", width: "20%", className: "text-align-center" },
        { data: "Operador", title: "Operador", width: "20%", className: "text-align-left" },
        { data: "Observacao", title: "Observação", width: "20%", className: "text-align-left" }
    ];

    _gridHistoricoInfracao = new BasicDataTable(_historicoInfracao.ListaHistorico.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridHistoricoInfracao.CarregarGrid([]);

    _modalCadastroHistoricoInfracao = new bootstrap.Modal(document.getElementById("divModalCadastroHistoricoInfracao"), { backdrop: 'static', keyboard: true });
}

function loadHistoricoInfracao() {
    _historicoInfracao = new HistoricoInfracao();
    KoBindings(_historicoInfracao, "knockoutHistoricoInfracao");

    _cadastroHistoricoInfracao = new CadastroHistoricoInfracao();
    KoBindings(_cadastroHistoricoInfracao, "knockoutCadastroHistoricoInfracao");

    loadGridHistoricoInfracao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarHistoricoClick() {
    if (isSituacaoPermiteGerenciarHistorico()) {
        if (ValidarCamposObrigatorios(_cadastroHistoricoInfracao)) {
            _cadastroHistoricoInfracao.Codigo.val(_infracao.Codigo.val());

            var historico = RetornarObjetoPesquisa(_cadastroHistoricoInfracao);

            executarReST("Infracao/AdicionarHistorico", historico, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Histórico adicionado com sucesso");

                        enviarAnexoHistorico(retorno.Data.Codigo);
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }, null);
        }
        else
            exibirMensagemCamposObrigatorio();
    }
}

function adicionarHistoricoModalClick() {
    _modalCadastroHistoricoInfracao.show();
    $("#divModalCadastroHistoricoInfracao").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroHistoricoInfracao);
    });
}

function downloadAnexoHistoricoClick(registroSelecionado) {
    executarDownload("InfracaoHistoricoAnexo/DownloadAnexo", { Codigo: registroSelecionado.CodigoAnexo });
}

function removerHistoricoClick(registroSelecionado) {
    if (isSituacaoPermiteGerenciarHistorico()) {
        exibirConfirmacao("Confirmação", "Realmente deseja excluir o histórico", function () {
            removerAnexoHistorico(registroSelecionado, removerHistorico)
        });
    }
}

/*
 * Declaração das Funções
 */

function buscarHistorico() {
    executarReST("Infracao/BuscarHistorico", { Codigo: _infracao.Codigo.val() }, function (retorno) {
        if (retorno.Data)
            preencherHistoricoInfracao(retorno.Data.ListaHistorico);
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function enviarAnexoHistorico(codigo) {
    var formData = obterFormDataAnexoHistorico();

    if (formData) {
        enviarArquivo("InfracaoHistoricoAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo anexado com sucesso");
                else
                    exibirMensagem(tipoMensagem.aviso, "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);

            fecharHistoricoInfracaoModal();
            buscarHistorico();
        });
    }
    else {
        fecharHistoricoInfracaoModal();
        buscarHistorico();
    }
}

function fecharHistoricoInfracaoModal() {
    _modalCadastroHistoricoInfracao.hide();
}

function isHistoricoPossuiAnexo(registroSelecionado) {
    return registroSelecionado.CodigoAnexo;
}

function isSituacaoPermiteGerenciarHistorico() {
    return _infracao.Situacao.val() !== EnumSituacaoInfracao.Cancelada;
}

function limparHistoricoInfracao() {
    _historicoInfracao.Adicionar.visible(false);
    _historicoInfracao.ListaHistorico.val(new Array());
}

function obterFormDataAnexoHistorico() {
    var arquivo = document.getElementById(_cadastroHistoricoInfracao.Arquivo.id);

    if (arquivo.files.length == 1) {
        var formData = new FormData();

        formData.append("Arquivo", arquivo.files[0]);
        formData.append("Descricao", "");

        return formData;
    }

    return undefined;
}

function obterListaHistorico() {
    return _historicoInfracao.ListaHistorico.val().slice();
}

function preencherHistoricoInfracao(dadosHistorico) {
    _historicoInfracao.ListaHistorico.val(dadosHistorico);
    _historicoInfracao.Adicionar.visible(isSituacaoPermiteGerenciarHistorico());
}

function recarregarGridHistorico() {
    var listaHistorico = obterListaHistorico();

    _gridHistoricoInfracao.CarregarGrid(listaHistorico);
}

function removerAnexoHistorico(registroSelecionado, callback) {
    if (registroSelecionado.CodigoAnexo > 0) {
        executarReST("InfracaoHistoricoAnexo/ExcluirAnexo", { Codigo: registroSelecionado.CodigoAnexo }, function (retorno) {
            if (!retorno.Data)
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);

            callback(registroSelecionado);
        }, null);
    }
    else
        callback(registroSelecionado);
}

function removerHistorico(registroSelecionado) {
    executarReST("Infracao/RemoverHistorico", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Data) {
            removerHistoricoLocal(registroSelecionado);

            exibirMensagem(tipoMensagem.ok, "Sucesso", "Histórico excluído com sucesso");
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function removerHistoricoLocal(registroSelecionado) {
    var listaHistorico = obterListaHistorico();

    listaHistorico.forEach(function (historico, i) {
        if (registroSelecionado.Codigo == historico.Codigo) {
            listaHistorico.splice(i, 1);
        }
    });

    _historicoInfracao.ListaHistorico.val(listaHistorico);
}