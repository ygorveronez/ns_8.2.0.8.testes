/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Globais.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroSolicitacaoFreteAnexo;
var _gridSolicitacaoFreteAnexo;

/*
 * Declaração das Classes
 */

var CadastroSolicitacaoFreteAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Arquivo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _cadastroSolicitacaoFreteAnexo.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarSolicitacaoFreteAnexoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadSolicitacaoFreteAnexo() {
    _cadastroSolicitacaoFreteAnexo = new CadastroSolicitacaoFreteAnexo();
    KoBindings(_cadastroSolicitacaoFreteAnexo, "knockoutCadastroSolicitacaoFreteAnexo");

    loadGridSolicitacaoFreteAnexo();
}

function loadGridSolicitacaoFreteAnexo() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: excluirSolicitacaoFreteAnexoClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "30%", className: "text-align-left" }
    ];

    _gridSolicitacaoFreteAnexo = new BasicDataTable(_solicitacaoFrete.ListaAnexo.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, linhasPorPaginas);
    _gridSolicitacaoFreteAnexo.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarSolicitacaoFreteAnexoClick() {
    var anexo = obterCadastroSolicitacaoFreteAnexoSalvar();

    if (!anexo)
        return;

    var listaSolicitacaoFreteAnexo = obterListaSolicitacaoFreteAnexo();

    listaSolicitacaoFreteAnexo.push(anexo);

    _solicitacaoFrete.ListaAnexo.val(listaSolicitacaoFreteAnexo);

    fecharModalCadastroSolicitacaoFreteAnexo();
}

function adicionarSolicitacaoFreteAnexoModalClick() {
    exibirModalCadastroSolicitacaoFreteAnexo();
}

function excluirSolicitacaoFreteAnexoClick(registroSelecionado) {
    var listaSolicitacaoFreteAnexo = obterListaSolicitacaoFreteAnexo();

    for (var i = 0; i < listaSolicitacaoFreteAnexo.length; i++) {
        if (registroSelecionado.Codigo == listaSolicitacaoFreteAnexo[i].Codigo) {
            listaSolicitacaoFreteAnexo.splice(i, 1);
            break;
        }
    }

    _solicitacaoFrete.ListaAnexo.val(listaSolicitacaoFreteAnexo);
}

/*
 * Declaração das Funções Públicas
 */

function enviarArquivosAnexadosSolicitacaoFrete(codigo) {
    var listaSolicitacaoFreteAnexo = obterListaSolicitacaoFreteAnexo();

    executarReST("CargaSolicitacaoFreteAnexo/ExcluirTodosAnexos", { Codigo: codigo }, function () {
        if (listaSolicitacaoFreteAnexo.length > 0) {
            var formData = obterFormDataSolicitacaoFreteAnexo(listaSolicitacaoFreteAnexo);

            enviarArquivo("CargaSolicitacaoFreteAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data)
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, (retorno.Data.Anexos.length > 1) ? Localization.Resources.Cargas.Carga.ArquivosAnexadosComSucesso : Localization.Resources.Gerais.Geral.ArquivoAnexadoComSucesso);
                    else
                        exibirMensagem(tipoMensagem.aviso, (listaSolicitacaoFreteAnexo.length > 1) ? Localization.Resources.Cargas.Carga.NaoFoiPossivelAnexarOsArquivos : Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            });
        }
    });
}

function obterListaSolicitacaoFreteAnexo() {
    return _solicitacaoFrete.ListaAnexo.val().slice();
}

function recarregarGridSolicitacaoFreteAnexo() {
    var listaSolicitacaoFreteAnexo = obterListaSolicitacaoFreteAnexo();

    _gridSolicitacaoFreteAnexo.CarregarGrid(listaSolicitacaoFreteAnexo);
}

/*
 * Declaração das Funções Privadas
 */

function exibirModalCadastroSolicitacaoFreteAnexo() {
    Global.abrirModal("divModalCadastroSolicitacaoFreteAnexo");
    $("#divModalCadastroSolicitacaoFreteAnexo").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroSolicitacaoFreteAnexo);
        _cadastroSolicitacaoFreteAnexo.Arquivo.val("");
    });
}

function fecharModalCadastroSolicitacaoFreteAnexo() {
    Global.fecharModal("divModalCadastroSolicitacaoFreteAnexo");
}

function obterCadastroSolicitacaoFreteAnexoSalvar() {
    var arquivo = document.getElementById(_cadastroSolicitacaoFreteAnexo.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);
        return undefined;
    }

    return {
        Codigo: guid(),
        Descricao: _cadastroSolicitacaoFreteAnexo.Descricao.val(),
        NomeArquivo: _cadastroSolicitacaoFreteAnexo.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };
}

function obterFormDataSolicitacaoFreteAnexo(anexos) {
    var formData = new FormData();

    anexos.forEach(function (anexo) {
        formData.append("Arquivo", anexo.Arquivo);
        formData.append("Descricao", anexo.Descricao);
    });

    return formData;
}
