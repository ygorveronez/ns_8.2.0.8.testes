/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridContribuinteAnexo;
var _contribuinteAnexo;
var _dadosAdicionaisContribuinteAnexo;

/*
 * Declaração das Classes
 */

var ContribuinteAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Anexo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Anexos = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Anexos, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Arquivo.val.subscribe(function (novoValor) { _contribuinteAnexo.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });
    this.Anexos.val.subscribe(recarregarGridContribuinteAnexo);

    this.Adicionar = PropertyEntity({ eventClick: adicionarContribuinteAnexoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

var DadosAdicionaisContribuinteAnexo = function () {
    this.CodigoCargaCTe = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PermiteGerenciarAnexos = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool });
}

/*
 * Declaração das Funções de Inicialização
 */
function loadContribuinteAnexo() {
    _contribuinteAnexo = new ContribuinteAnexo();
    KoBindings(_contribuinteAnexo, "knockoutModalContribuinteAnexo");

    _dadosAdicionaisContribuinteAnexo = new DadosAdicionaisContribuinteAnexo();

    loadGridContribuinteAnexo();
}

function loadGridContribuinteAnexo() {
    let linhasPorPaginas = 7;
    let opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadContribuinteAnexoClick, icone: "" };
    let opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerContribuinteAnexoClick, icone: "", visibilidade: isPermitirGerenciarContribuinteAnexos };
    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 20, opcoes: [opcaoDownload, opcaoRemover] };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "25%", className: "text-align-left" }
    ];

    _gridContribuinteAnexo = new BasicDataTable(_contribuinteAnexo.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridContribuinteAnexo.CarregarGrid([]);
}

function AbrirModalAnexoContribuinteClick() {
    Global.abrirModal('divModalCargaContribuinteAnexo');
}

function FecharModalAnexoContribuinteClick() {
    Global.fecharModal('divModalCargaContribuinteAnexo');
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarContribuinteAnexoClick() {
    if(!isPermitirGerenciarContribuinteAnexos())
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, "Situação não permite gerenciar Anexos")

    let arquivo = document.getElementById(_contribuinteAnexo.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);

    let anexo = {
        Codigo: guid(),
        Descricao: _contribuinteAnexo.Descricao.val(),
        NomeArquivo: _contribuinteAnexo.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    if (_cargaAtual.Empresa.codEntity() > 0)
        enviarContribuinteAnexos(_cargaAtual.Empresa.codEntity(), [anexo]);
    else {
        let anexos = obterContribuinteAnexos();

        anexos.push(anexo);

        _contribuinteAnexo.Anexos.val(anexos.slice());
    }

    LimparCampos(_contribuinteAnexo);

    arquivo.value = null;
}

function downloadContribuinteAnexoClick(registroSelecionado) {
    executarDownload("ContribuinteCargaCTeAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function removerContribuinteAnexoClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerContribuinteAnexoLocal(registroSelecionado);
    else {
        executarReST("ContribuinteCargaCTeAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AnexoExcluidoComSucesso);
                    removerContribuinteAnexoLocal(registroSelecionado);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

/*
 * Declaração das Funções
 */

function enviarContribuinteAnexos(codigo, anexos) {
    let formData = obterFormDataContribuinteAnexo(anexos);

    if (formData) {
        enviarArquivo("ContribuinteCargaCTeAnexo/AnexarArquivos?callback=?", { Codigo: codigo, CargaCTe: _dadosAdicionaisContribuinteAnexo.CodigoCargaCTe.val() }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _contribuinteAnexo.Anexos.val(retorno.Data.Anexos);

                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ArquivoAnexadoComSucesso);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

function enviarArquivosContribuinteAnexados(codigo) {
    let anexos = obterContribuinteAnexos();

    enviarContribuinteAnexos(codigo, anexos);
}

function limparCamposContribuinteAnexo() {
    LimparCampos(_contribuinteAnexo);
    _contribuinteAnexo.Anexos.val(_contribuinteAnexo.Anexos.def);
    recarregarGridContribuinteAnexo();
}

function obterContribuinteAnexos() {
    return _contribuinteAnexo.Anexos.val().slice();
}

function obterFormDataContribuinteAnexo(anexos) {
    if (anexos.length > 0) {
        let formData = new FormData();

        anexos.forEach(function (anexo) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        });

        return formData;
    }

    return undefined;
}

function recarregarGridContribuinteAnexo() {
    let anexos = obterContribuinteAnexos();

    _gridContribuinteAnexo.CarregarGrid(anexos);
}

function removerContribuinteAnexoLocal(registroSelecionado) {
    let listaAnexos = obterContribuinteAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _contribuinteAnexo.Anexos.val(listaAnexos);
}

function isPossuiContribuinteAnexo() {
    return obterContribuinteAnexos().length > 0;
}

function isPermitirGerenciarContribuinteAnexos() {
    return _dadosAdicionaisContribuinteAnexo.PermiteGerenciarAnexos.val();
}