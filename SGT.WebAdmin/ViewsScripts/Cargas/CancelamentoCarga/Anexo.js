/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridAnexo, _anexo;

/*
 * Declaração das Classes
 */

var Anexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Anexo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Anexos = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Anexos, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Arquivo.val.subscribe(function (novoValor) { _anexo.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });
    this.Anexos.val.subscribe(RecarregarGridAnexo);

    this.Adicionar = PropertyEntity({ eventClick: AdicionarAnexoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), icon: "fal fa-plus" });
    this.Finalizar = PropertyEntity({ eventClick: FecharTelaAnexoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Finalizar, visible: ko.observable(true), icon: "fal fa-chevron-down" });
    this.Fechar = PropertyEntity({ eventClick: FecharTelaAnexoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Fechar, visible: ko.observable(true), icon: "fal fa-window-close" });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadAnexo() {
    _anexo = new Anexo();
    KoBindings(_anexo, "knockoutCancelamentoCargaAnexo");

    LoadGridAnexo();
}

function LoadGridAnexo() {
    var linhasPorPagina = 7;
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: DownloadAnexoClick, icone: "", visibilidade: isExibirOpcaoDownloadAnexo };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: RemoverAnexoClick, icone: "", visibilidade: isPermitirGerenciarAnexos };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 20, opcoes: [opcaoDownload, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "25%", className: "text-align-left" }
    ];

    _gridAnexo = new BasicDataTable(_anexo.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPagina);
    _gridAnexo.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AbrirTelaAnexoClick() {
    Global.abrirModal('knockoutCancelamentoCargaAnexo');
}

function FecharTelaAnexoClick() {
    Global.fecharModal('knockoutCancelamentoCargaAnexo');
}

function AdicionarAnexoClick() {
    if (!isPermitirGerenciarAnexos())
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NaoPermiteAdicionarAnexo);

    var arquivo = document.getElementById(_anexo.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);

    var anexo = {
        Codigo: guid(),
        Descricao: _anexo.Descricao.val(),
        NomeArquivo: _anexo.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    if (_cancelamento.Codigo.val() > 0)
        EnviarAnexos(_cancelamento.Codigo.val(), [anexo]);
    else {
        var anexos = ObterAnexos();

        anexos.push(anexo);

        _anexo.Anexos.val(anexos.slice());
    }

    LimparCampos(_anexo);

    arquivo.value = null;
}

function DownloadAnexoClick(registroSelecionado) {
    executarDownload("CancelamentoCargaAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function RemoverAnexoClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        RemoverAnexoLocal(registroSelecionado);
    else if (isPermitirGerenciarAnexos()) {
        executarReST("CancelamentoCargaAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AnexoExcluidoComSucesso);
                    RemoverAnexoLocal(registroSelecionado);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
    else
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NaoPermiteRemoverAnexo);
}

/*
 * Declaração das Funções
 */

function EnviarAnexos(codigo, anexos) {
    var formData = ObterFormDataAnexo(anexos);

    var p = new promise.Promise();

    if (formData) {

        enviarArquivo("CancelamentoCargaAnexo/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _anexo.Anexos.val(retorno.Data.Anexos);

                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ArquivosAnexadosSucesso);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);

            p.done();
        });

    } else {
        p.done();
    }

    return p;
}

function EnviarArquivosAnexados(codigo) {
    var anexos = ObterAnexos();

    return EnviarAnexos(codigo, anexos);
}

function isExibirOpcaoDownloadAnexo(registroSelecionado) {
    return !isNaN(registroSelecionado.Codigo);
}

function isPermitirGerenciarAnexos() {
    var status = _cancelamento.Situacao.val();

    return (status !== EnumSituacaoCancelamentoCarga.EmCancelamento);
}

function LimparCamposAnexo() {
    LimparCampos(_anexo);

    _anexo.Anexos.val(_anexo.Anexos.def);

    _anexo.Anexos.visible(true);
    _anexo.Finalizar.visible(true);
}

function ObterAnexos() {
    return _anexo.Anexos.val().slice();
}

function ObterFormDataAnexo(anexos) {
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

function RecarregarGridAnexo() {
    var anexos = ObterAnexos();

    _gridAnexo.CarregarGrid(anexos);
}

function RemoverAnexoLocal(registroSelecionado) {
    var listaAnexos = ObterAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _anexo.Anexos.val(listaAnexos);
}
