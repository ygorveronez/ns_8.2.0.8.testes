//#region Variáveis Globais

var _agendamentoColetaListaAnexos;
var _agendamentoColetaAnexosAdicionar;
var _gridAgendamentoColetaAnexos;

//#endregion

//#region Mapeamento Knockout

var AgendamentoColetaListaAnexo = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexo();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoModalClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(visibilidadeBotaoAdicionarAnexo) });
}

var AgendamentoColetaAnexoAdicionar = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _agendamentoColetaAnexosAdicionar.NomeArquivo.val(nomeArquivo);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAgendamentoColetaAnexoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

function loadAgendamentoColetaAnexos() {
    carregarHtmlAnexo();
}

function loadGridAnexo() {
    var linhasPorPaginas = 2;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoClick, icone: "", visibilidade: true };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoClick, icone: "", visibilidade: visibilidadeRemoverAnexo };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAgendamentoColetaAnexos = new BasicDataTable(_agendamentoColetaListaAnexos.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAgendamentoColetaAnexos.CarregarGrid([]);
}

//#endregion

//#region Métodos Privados

function carregarHtmlAnexo() {
    $.get("Content/Static/AgendamentoColeta/AgendamentoColetaAnexo.html?dyn=" + guid(), function (data) {
        $("#divAgendamentoColetaAnexos").html(data);
        
        _agendamentoColetaAnexosAdicionar = new AgendamentoColetaAnexoAdicionar();
        _agendamentoColetaListaAnexos = new AgendamentoColetaListaAnexo();

        KoBindings(_agendamentoColetaAnexosAdicionar, "knockoutAnexoAgendamentoColetaAdicionar");
        KoBindings(_agendamentoColetaListaAnexos, "knockoutAnexoAgendamentoColeta");

        loadGridAnexo();
    });
}

function downloadAnexoClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("AgendamentoColetaAnexo/DownloadAnexo", dados);
}

function enviarArquivosAnexados(codigo) {
    var anexos = obterAnexos();

    if (anexos.length > 0)
        enviarAnexos(codigo, anexos);
}

function isAnexosInformados() {
    return (obterAnexos().length > 0);
}

function limparAnexo() {
    _agendamentoColetaListaAnexos.Anexos.val(new Array());
}

function preencherAnexo(dadosAnexos) {
    _agendamentoColetaListaAnexos.Anexos.val(dadosAnexos);
}

function enviarAnexos(codigo, anexos) {
    var formData = obterFormDataAnexos(anexos);

    if (formData) {
        enviarArquivo("AgendamentoColetaAnexo/AnexarArquivos", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");
                }
                else
                    exibirMensagem(tipoMensagem.falha, (anexos.length > 1) ? "Não foi possível anexar os arquivos." : "Não foi possível anexar o arquivo.", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function obterAnexos() {
    return _agendamentoColetaListaAnexos.Anexos.val().slice();
}

function obterFormDataAnexos(anexos) {
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

function visibilidadeRemoverAnexo() {
    return typeof _agendamentoColeta !== 'undefined' ? !_agendamentoColeta.CodigoAgendamento.val() > 0 : false;
}

function recarregarGridAnexo() {
    var anexos = obterAnexos();

    _gridAgendamentoColetaAnexos.CarregarGrid(anexos);
}

function removerAnexoLocal(registroSelecionado) {
    var listaAnexos = obterAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _agendamentoColetaListaAnexos.Anexos.val(listaAnexos);
}

function visibilidadeBotaoAdicionarAnexo() {
    return _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe;
}

//#endregion

//#region Métodos Públicos

function adicionarAgendamentoColetaAnexoClick() {
    var arquivo = document.getElementById(_agendamentoColetaAnexosAdicionar.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
        return;
    }

    if (!ValidarCamposObrigatorios(_agendamentoColetaAnexosAdicionar)) {
        exibirMensagem(tipoMensagem.atencao, "Campo vazio", "Você precisa informar uma descrição.")
        return;
    }

    var anexo = {
        Codigo: guid(),
        Descricao: _agendamentoColetaAnexosAdicionar.Descricao.val(),
        NomeArquivo: _agendamentoColetaAnexosAdicionar.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    var listaAnexos = obterAnexos();

    listaAnexos.push(anexo);

    _agendamentoColetaListaAnexos.Anexos.val(listaAnexos.slice());

    _agendamentoColetaAnexosAdicionar.Arquivo.val("");

    Global.fecharModal('divModalAdicionarAnexoAgendamentoColeta');
}

function adicionarAnexoModalClick() {
    Global.abrirModal('divModalAdicionarAnexoAgendamentoColeta');
    $("#divModalAdicionarAnexoAgendamentoColeta").one("hidden.bs.modal", function () {
        LimparCampos(_agendamentoColetaAnexosAdicionar);
    });
}

function removerAnexoClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoLocal(registroSelecionado);
    else {
        executarReST("AgendamentoColetaAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
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

//#endregion