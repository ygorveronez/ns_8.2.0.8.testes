/// <reference path="Analise.js" />


//#region Objetos Globais do Arquivo
var _gridAnexoNivelAtendimento;
var _anexoNivelAtendimento;
var _listaGridsPorNivel = [];

//#endregion Objetos Globais do Arquivo

//#region Declaração das Classes

var AnexoNivelAtendimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, visible: ko.observable(true) });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string, visible: ko.observable(true) });
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.DescricaoModal = PropertyEntity({ text: ko.observable(""), type: types.map, getType: typesKnockout.string, maxlength: 150 });

    this.Arquivo.val.subscribe(function (novoValor) { _anexoAnalise.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });
    this.Anexos.val.subscribe(recarregarGridAnexo);

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoNivelAtendimentoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};
//#endregion Declaração das Classes

//#region Funções de Inicialização
function loadAnexoNivelAtendimento() {
    _anexoNivelAtendimento = new AnexoNivelAtendimento();
    KoBindings(_anexoNivelAtendimento, "knockoutNivelAtendimentoAnexo");

}
//#endregion Funções de Inicialização



//#region Funções Associadas a Eventos

function adicionarAnexoNivelAtendimentoClick() {

    var arquivo = document.getElementById(_anexoNivelAtendimento.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoNivelAtendimento.Descricao.val(),
        NomeArquivo: _anexoNivelAtendimento.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    if (_chamado.Codigo.val() > 0)
        enviarAnexosNivelAtendimento(_chamado.Codigo.val(), [anexo]);
    else {
        var anexos = obterAnexosNivelAtendimento();

        anexos.push(anexo);

        _anexoNivelAtendimento.Anexos.val(anexos.slice());
    }

    arquivo.value = null;
    Global.fecharModal('divModalAnexoEscalada');
}

function downloadAnexoClick(registroSelecionado) {
    executarDownload("ChamadoNivelAtendimentoAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function removerAnexoClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoNivelAtendimentoLocal(registroSelecionado);
    else {
        executarReST("ChamadoNivelAtendimentoAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                    removerAnexoNivelAtendimentoLocal(registroSelecionado);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

//#endregion Funções Associadas a Eventos

//#region Funções Públicas

function exibirAnexos() {
    _anexoNivelAtendimento.Adicionar.visible(false);
    _anexoNivelAtendimento.Arquivo.visible(false);
    _anexoNivelAtendimento.Descricao.visible(false);
    _anexoNivelAtendimento.DescricaoModal.text("Anexos da Escalada");

    $("#grids-anexos").show();
    obterAnexosNivelAtendimentPreencherGrid();
}
//#endRegion Funções Públicas

//#region Funções Privadas
function enviarAnexosNivelAtendimento(codigo, anexos) {
    var formData = obterFormDataAnexoNivelAtendimento(anexos);

    if (formData) {
        enviarArquivo("ChamadoNivelAtendimentoAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _anexoNivelAtendimento.Anexos.val(retorno.Data.Anexos);
                    executarRestEscalarNivel();
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

function obterFormDataAnexoNivelAtendimento(anexos) {
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



function obterAnexosNivelAtendimento() {
    return _anexoNivelAtendimento.Anexos.val().slice();
}

function removerAnexoNivelAtendimentoLocal(registroSelecionado) {
    var listaAnexos = obterAnexosNivelAtendimento();
    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });
    var listaAnexosAgrupada = Object.groupBy(listaAnexos, (x) => x.Nivel);

    for (i = 0; i < _listaGridsPorNivel.length; i++) {
        _listaGridsPorNivel[i].Grid.CarregarGrid([]);
        if (listaAnexosAgrupada[_listaGridsPorNivel[i].Nivel])
            _listaGridsPorNivel[i].Grid.CarregarGrid(listaAnexosAgrupada[_listaGridsPorNivel[i].Nivel]);
    }
    _anexoNivelAtendimento.Anexos.val(listaAnexos);
}

function obterAnexosNivelAtendimentPreencherGrid() {
    $("#grid-anexos").html("");
    _listaGridsPorNivel = [];

    executarReST("ChamadoNivelAtendimentoAnexo/ObterAnexos", { Codigo: _analise.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _anexoNivelAtendimento.Anexos.val(retorno.Data);
                var listaAnexosPorNivel = [];

                var header = [
                    { data: "Codigo", visible: false },
                    { data: "Descricao", title: "Descrição", width: "40%", className: "text-align-left" },
                    { data: "NomeArquivo", title: "Nome", width: "25%", className: "text-align-left" }
                ];

                var linhasPorPaginas = 5;
                var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoClick, icone: "" };
                var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoClick, icone: "" };
                var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 20, opcoes: [opcaoDownload, opcaoRemover] };

                listaAnexosPorNivel = Object.groupBy(retorno.Data, (x) => x.Nivel);
                var keys = Object.keys(listaAnexosPorNivel);

                for (i = 0; i < keys.length; i++) {
                    var key = keys[i];

                    var htmlTables = "";

                    htmlTables += ' <div>' +
                        '     <h4>Anexos nível ' + key + '</h4>' +
                        '     <table width="100%" class="table table-bordered table-hover"  id="' + key + '" cellspacing="0"></table>' +
                        '     <hr/>' +
                        ' </div>';

                    $("#grid-anexos").append(htmlTables);

                    _gridAnexoNivelAtendimento = new BasicDataTable(key, header, menuOpcoes, null, null, linhasPorPaginas);
                    _gridAnexoNivelAtendimento.CarregarGrid(listaAnexosPorNivel[key]);

                    _listaGridsPorNivel.push({ Id: key, Nivel: key, Anexos: listaAnexosPorNivel[key], Grid: _gridAnexoNivelAtendimento });
                }

            }
            else
                exibirMensagem(tipoMensagem.aviso, "Não foi possível anexar o arquivo.", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}
//#endregion Funções Privadas