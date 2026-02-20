/*
 * Declaração de Objetos Globais do Arquivo
 */

var _anexoCheckList;
var _gridAnexoCheckList;
var _listaAnexoCheckList;

/*
 * Declaração das Classes
 */

var AnexoCheckList = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, required: true });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoCheckList.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoCheckListClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

var ListaAnexoCheckList = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexoCheckList();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoCheckListModalClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadCheckListAnexo() {
    _anexoCheckList = new AnexoCheckList();
    KoBindings(_anexoCheckList, "knockoutCheckListAnexoAdicionar");

    _listaAnexoCheckList = new ListaAnexoCheckList();
    KoBindings(_listaAnexoCheckList, "knockoutCheckListAnexo");

    loadGridAnexoCheckList();
}

function loadGridAnexoCheckList() {
    var linhasPorPaginas = 2;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoCheckListClick, icone: "", visibilidade: true };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoCheckListClick, icone: "", visibilidade: visibilidadeOpcaoRemover };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexoCheckList = new BasicDataTable(_listaAnexoCheckList.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoCheckList.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarAnexoCheckListClick() {
    var arquivo = document.getElementById(_anexoCheckList.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
        return;
    }

    if (!ValidarCamposObrigatorios(_anexoCheckList)) {
        exibirMensagem(tipoMensagem.atencao, "Campo vazio", "Você precisa informar uma descrição.")
        return;
    }

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoCheckList.Descricao.val(),
        NomeArquivo: _anexoCheckList.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    enviarAnexosCheckList(_checkList.Codigo.val(), [anexo]);

    var listaAnexos = obterAnexosCheckList();

    listaAnexos.push(anexo);

    _listaAnexoCheckList.Anexos.val(listaAnexos.slice());

    _anexoCheckList.Arquivo.val("");

    Global.fecharModal('divModalAdicionarAnexoCheckList');
}

function adicionarAnexoCheckListModalClick() {
    Global.abrirModal('divModalAdicionarAnexoCheckList');
    $("#divModalAdicionarAnexoCheckList").one('hidden.bs.modal', function () {
        LimparCampos(_anexoCheckList);
    });
}

function downloadAnexoCheckListClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("CheckListAnexo/DownloadAnexo", dados);
}

function removerAnexoCheckListClick(registroSelecionado) {
    executarReST("CheckListAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                removerAnexoLocalCheckList(registroSelecionado);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções Públicas
 */

function limparAnexoCheckList() {
    //_listaAnexo.Adicionar.visible(!isAcessoTransportador());
    _listaAnexoCheckList.Anexos.val(new Array());
}

function preencherAnexoCheckList(dadosAnexos) {
    _listaAnexoCheckList.Anexos.val(dadosAnexos);
}

/*
 * Declaração das Funções Privadas
 */

function visibilidadeOpcaoRemover() {
    return EnumSituacaoCheckList.isPermiteEdicao(_checkList.Situacao.val());
}

function enviarAnexosCheckList(codigo, anexos) {
    var formData = obterFormDataAnexosCheckList(anexos);

    if (formData) {
        enviarArquivo("CheckListAnexo/AnexarArquivos", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");
                    buscarAnexosCheckList();
                }
                else
                    exibirMensagem(tipoMensagem.falha, (anexos.length > 1) ? "Não foi possível anexar os arquivos." : "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function buscarAnexosCheckList() {
    executarReST("CheckListAnexo/ObterAnexo", { Codigo: _checkList.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _listaAnexoCheckList.Anexos.val(arg.Data.Anexos);
                recarregarGridAnexoCheckList();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function obterAnexosCheckList() {
    return _listaAnexoCheckList.Anexos.val().slice();
}

function obterFormDataAnexosCheckList(anexos) {
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

function recarregarGridAnexoCheckList() {
    var anexos = obterAnexosCheckList();

    _gridAnexoCheckList.CarregarGrid(anexos);
}

function removerAnexoLocalCheckList(registroSelecionado) {
    var listaAnexos = obterAnexosCheckList();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _listaAnexoCheckList.Anexos.val(listaAnexos);
}
