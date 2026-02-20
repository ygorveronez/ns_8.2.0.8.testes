
/// <reference path="../../Enumeradores/EnumSituacaoDigitalizacaoCanhoto.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroAnexo;
var _gridAnexo;
var _anexo;

/*
 * Declaração das Classes
 */

var AnexoCanhoto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.SituacaoDigitalizacaoCanhoto = PropertyEntity({ val: ko.observable(EnumSituacaoDigitalizacaoCanhoto.Todas), options: EnumSituacaoDigitalizacaoCanhoto.ObterOpcoes(true), def: EnumSituacaoDigitalizacaoCanhoto.Todas });
    this.Anexos = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Anexos, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexo();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoModalClick, type: types.event, text: Localization.Resources.Gerais.Geral.AdicionarAnexo, visible: ko.observable(false) });
}

var CadastroAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });    
    this.Arquivo = PropertyEntity({ type: types.file, val: ko.observable(""), text: Localization.Resources.Gerais.Geral.Arquivo.getFieldDescription(), enable: ko.observable(true), codEntity: ko.observable(0), visible: ko.observable(true) });


    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _cadastroAnexo.NomeArquivo.val(nomeArquivo);
    });
    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAnexo() {
    _cadastroAnexo = new CadastroAnexo();
    KoBindings(_cadastroAnexo, "knockoutCadastroAnexo");

    _anexo = new AnexoCanhoto();
    KoBindings(_anexo, "knockoutAnexo");

    loadGridAnexo();
}

function loadGridAnexo() {
    var linhasPorPaginas = 5;
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadAnexoClick, icone: "" };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoClick, icone: "", visibilidade: isOpcaoRemoverAnexoVisivel };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "30%", className: "text-align-left" }
    ];

    _gridAnexo = new BasicDataTable(_anexo.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexo.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarAnexoClick() {
    if (!isPermitirGerenciarAnexos())
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Canhotos.Canhoto.NaoPossivelAnexarArquivosNaAtualSituacaoDaDigitalizacao);

    var arquivo = document.getElementById(_cadastroAnexo.Arquivo.id);

    if (arquivo.files.length == 0)
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);

    var anexo = {
        Codigo: guid(),
        Descricao: _cadastroAnexo.Descricao.val(),
        NomeArquivo: _cadastroAnexo.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    enviarAnexos(_anexo.Codigo.val(), [anexo]);

    _cadastroAnexo.Arquivo.val("");

    fecharModalCadastroAnexo();
}

function adicionarAnexoModalClick() {
    exibirModalCadastroAnexo();
}

function downloadAnexoClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("CanhotoAnexo/DownloadAnexo", dados);
}

function removerAnexoClick(registroSelecionado) {
    if (!isPermitirGerenciarAnexos())
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Canhotos.Canhoto.SituacaoDaDigitalizacaoNaoPermiteRemoverAnexo);

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.DesejaRealmenteRemoverDocumentoEmAnexo, function () {
        executarReST("CanhotoAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AnexoExcluidoComSucesso);
                    removerAnexoLocal(registroSelecionado);
                    recarregarGridAnexo();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

/*
 * Declaração das Funções Públicas
 */

function exibirAnexos(registroSelecionado) {
    _anexo.Codigo.val(registroSelecionado.Codigo);
    _anexo.SituacaoDigitalizacaoCanhoto.val(registroSelecionado.SituacaoDigitalizacaoCanhoto);
    _anexo.Adicionar.visible(isPermitirGerenciarAnexos());

    executarReST("CanhotoAnexo/ObterAnexo", { Codigo: _anexo.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _anexo.Anexos.val(retorno.Data.Anexos);
                recarregarGridAnexo();
                exibirModalAnexo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

/*
 * Declaração das Funções Privadas
 */

function enviarAnexos(codigo, anexos) {
    var formData = obterFormDataAnexos(anexos);

    if (!formData)
        return;

    enviarArquivo("CanhotoAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, (retorno.Data.Anexos.length > 1) ? Localization.Resources.Gerais.Geral.ArquivosAnexadosComSucesso : Localization.Resources.Gerais.Geral.ArquivoAnexadoComSucesso);
                _anexo.Anexos.val(retorno.Data.Anexos);
                recarregarGridAnexo();
            }
            else
                exibirMensagem(tipoMensagem.falha, (anexos.length > 1) ? Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarOsArquivos : Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function exibirModalAnexo() {
    Global.abrirModal('divModalAnexo');
    $("#divModalAnexo").one('hidden.bs.modal', function () {
        limparAnexo();
    });
}

function exibirModalCadastroAnexo() {
    Global.abrirModal('divModalCadastroAnexo');
    $("#divModalCadastroAnexo").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroAnexo);
        _cadastroAnexo.Arquivo.val("");
        _cadastroAnexo.Arquivo.val(Localization.Resources.Veiculos.VeiculoLicenca.SelecioneUmaImagemParaAnexar.getFieldDescription());
    });
}

function fecharModalCadastroAnexo() {
    Global.fecharModal('divModalCadastroAnexo');
}

function isOpcaoRemoverAnexoVisivel() {
    return isPermitirGerenciarAnexos();
}

function isPermitirGerenciarAnexos() {
    return (_anexo.SituacaoDigitalizacaoCanhoto.val() != EnumSituacaoDigitalizacaoCanhoto.Digitalizado);
}

function limparAnexo() {
    _anexo.Anexos.val(new Array());

    LimparCampos(_anexo);    
    recarregarGridAnexo();
}

function obterAnexos() {
    return _anexo.Anexos.val().slice();
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

function recarregarGridAnexo() {
    var anexos = obterAnexos();

    _gridAnexo.CarregarGrid(anexos);
}

function removerAnexoLocal(registroSelecionado) {
    var listaAnexos = obterAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _anexo.Anexos.val(listaAnexos);
}
