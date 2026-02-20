/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroVigenciaAnexo;
var _gridVigenciaAnexo;

/*
 * Declaração das Classes
 */

var CadastroVigenciaAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFrete.Arquivo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _cadastroVigenciaAnexo.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarVigenciaAnexoClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Adicionar, visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadVigenciaAnexo() {
    _cadastroVigenciaAnexo = new CadastroVigenciaAnexo();
    KoBindings(_cadastroVigenciaAnexo, "knockoutCadastroVigenciaAnexo");

    loadGridVigenciaAnexo();
}

function loadGridVigenciaAnexo() {
    var linhasPorPaginas = 5;
    var opcaoDownload = { descricao: Localization.Resources.Fretes.TabelaFrete.Download, id: guid(), metodo: downloadVigenciaAnexoClick, icone: "", visibilidade: isOpcaoDownloadVigenciaAnexoVisivel };
    var opcaoRemover = { descricao: Localization.Resources.Fretes.TabelaFrete.Remover, id: guid(), metodo: excluirVigenciaAnexoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Fretes.TabelaFrete.Opcoes, tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Fretes.TabelaFrete.Nome, width: "30%", className: "text-align-left" }
    ];

    _gridVigenciaAnexo = new BasicDataTable(_cadastroVigencia.ListaAnexo.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridVigenciaAnexo.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarVigenciaAnexoClick() {
    if (isNaN(_cadastroVigencia.Codigo.val())) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.Aviso, Localization.Resources.Fretes.TabelaFrete.ParaAdicionarAnexosNestaVigenciaNecessarioSalvarAsAlteracoesTabelaFrete);
        fecharModalCadastroVigenciaAnexo();
        return;
    }

    var anexo = obterCadastroVigenciaAnexoSalvar();

    if (!anexo)
        return;

    enviarArquivosAnexadosVigencia(_cadastroVigencia.Codigo.val(), [anexo]);
}

function adicionarVigenciaAnexoModalClick() {
    exibirModalCadastroVigenciaAnexo();
}

function downloadVigenciaAnexoClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("VigenciaTabelaFreteAnexo/DownloadAnexo", dados);
}

function excluirVigenciaAnexoClick(registroSelecionado) {
    executarReST("VigenciaTabelaFreteAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Fretes.TabelaFrete.Sucesso, Localization.Resources.Fretes.TabelaFrete.AnexoExcluidoSucesso);
                excluirVigenciaAnexoLocal(registroSelecionado);
                atualizarVigenciaAnexo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.Aviso, retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Fretes.TabelaFrete.Falha, retorno.Msg);
    });
}

/*
 * Declaração das Funções Públicas
 */

function enviarArquivosAnexadosVigencia(codigo, anexos) {
    if (anexos.length > 0) {
        var formData = obterFormDataVigenciaAnexo(anexos);

        enviarArquivo("VigenciaTabelaFreteAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _cadastroVigencia.ListaAnexo.val(retorno.Data.Anexos);

                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Fretes.TabelaFrete.Sucesso, (retorno.Data.Anexos.length > 1) ? Localization.Resources.Fretes.TabelaFrete.ArquivosAnexadosSucesso : Localization.Resources.Fretes.TabelaFrete.ArquivoAnexadoComSucesso);
                    atualizarVigenciaAnexo();
                    fecharModalCadastroVigenciaAnexo();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, (anexos.length > 1) ? Localization.Resources.Fretes.TabelaFrete.NaoFoiPossivelAnexarArquivos : Localization.Resources.Fretes.TabelaFrete.NaoFoiPossivelAnexarOArquivo, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Fretes.TabelaFrete.Falha, retorno.Msg);
        });
    }
}

function limparCamposVigenciaAnexo() {
    _cadastroVigencia.ListaAnexo.val(new Array());
}

function obterListaVigenciaAnexo() {
    return _cadastroVigencia.ListaAnexo.val().slice();
}

function recarregarGridVigenciaAnexo() {
    var listaVigenciaAnexo = obterListaVigenciaAnexo();

    _gridVigenciaAnexo.CarregarGrid(listaVigenciaAnexo);
}

/*
 * Declaração das Funções Privadas
 */

function excluirVigenciaAnexoLocal(registroSelecionado) {
    var listaVigenciaAnexo = obterListaVigenciaAnexo();

    for (var i = 0; i < listaVigenciaAnexo.length; i++) {
        if (registroSelecionado.Codigo == listaVigenciaAnexo[i].Codigo) {
            listaVigenciaAnexo.splice(i, 1);
            break;
        }
    }

    _cadastroVigencia.ListaAnexo.val(listaVigenciaAnexo);
}

function exibirModalCadastroVigenciaAnexo() {
    Global.abrirModal('divModalCadastroVigenciaAnexo');
    $("#divModalCadastroVigenciaAnexo").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroVigenciaAnexo);
        _cadastroVigenciaAnexo.Arquivo.val("");
    });
}

function fecharModalCadastroVigenciaAnexo() {
    Global.fecharModal('divModalCadastroVigenciaAnexo');
}

function isOpcaoDownloadVigenciaAnexoVisivel(registroSelecionado) {
    return !isNaN(registroSelecionado.Codigo);
}

function obterFormDataVigenciaAnexo(anexos) {
    var formData = new FormData();

    anexos.forEach(function (anexo) {
        formData.append("Arquivo", anexo.Arquivo);
        formData.append("Descricao", anexo.Descricao);
    });

    return formData;
}

function obterCadastroVigenciaAnexoSalvar() {
    var arquivo = document.getElementById(_cadastroVigenciaAnexo.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.Anexos, Localization.Resources.Fretes.TabelaFrete.NenhumArquivoSelecionado);
        return undefined;
    }

    return {
        Codigo: guid(),
        Descricao: _cadastroVigenciaAnexo.Descricao.val(),
        NomeArquivo: _cadastroVigenciaAnexo.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };
}
