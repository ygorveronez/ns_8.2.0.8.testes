
var _contestacao;
var _cadastroAnexoContestacao;


function Contestacao() {
    this.Codigo = PropertyEntity({ val: ko.observable(0) })
    this.MotivoContestacao = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Motivo Contestação" });
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
       
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoModalClick, type: types.event, text: Localization.Resources.Gerais.Geral.AdicionarAnexo, visible: ko.observable(true) });

    this.SolicitarContestacao = PropertyEntity({
        eventClick: function (e) {
            EnviarContestacao()
        }, type: types.event, text: "Solicitar Contestação", idGrid: guid(), visible: ko.observable(true)
    });
}

var CadastroAnexoContestacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Arquivo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _cadastroAnexoContestacao.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
}



function loadContestacao() {
    _contestacao = new Contestacao();
    KoBindings(_contestacao, "knoutContestacao");

    _cadastroAnexoContestacao = new CadastroAnexoContestacao();
    KoBindings(_cadastroAnexoContestacao, "knockoutCadastroAnexoContestacao");

    loadGridAnexoContestacao();
}

function loadGridAnexoContestacao() {
    var linhasPorPaginas = 5;
    var opcaoDownload = { descricao: "Donwload", id: guid(), metodo: downloadAnexoClick , icone: "" };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoClick, icone: "", visibilidade: true };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "30%", className: "text-align-left" }
    ];

    _gridAnexo = new BasicDataTable(_contestacao.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexo.CarregarGrid([]);
}


/*
 * Declaração das Funções Associadas a Eventos
 */

function abrirModalContestacao(devolucaoPalletsGrid) {
    LimparCamposContestacao();
    _contestacao.Codigo.val(devolucaoPalletsGrid.Codigo);
    Global.abrirModal('divModalContestacao');
}

/*
 * Declaração das Funções
 */
function LimparCamposContestacao() {
    LimparCampos(_contestacao);
}


function adicionarAnexoModalClick() {
    Global.abrirModal('divModalCadastroAnexoContestacao');
    $("#divModalCadastroAnexoContestacao").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroAnexoContestacao);
    });
}

function adicionarAnexoClick() {
  

    var arquivo = document.getElementById(_cadastroAnexoContestacao.Arquivo.id);

    if (arquivo.files.length == 0)
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Gerais.Geral.NenhumArquivoSelecionado);

    var anexo = {
        Codigo: guid(),
        Descricao: _cadastroAnexoContestacao.Descricao.val(),
        NomeArquivo: _cadastroAnexoContestacao.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    enviarAnexosContestacao(_contestacao.Codigo.val(), [anexo]);

    _cadastroAnexoContestacao.Arquivo.val("");

    Global.fecharModal('divModalCadastroAnexoContestacao');

}


function downloadAnexoClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("Contestacao/DownloadAnexo", dados);
}

function removerAnexoClick(registroSelecionado) {
  

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Deseja realmente remover o anexo ?", function () {
        executarReST("Contestacao/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AnexoExcluidoComSucesso);
                    removerAnexoLocal(registroSelecionado);
                    recarregarGridAnexoContestacao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function removerAnexoLocal(registroSelecionado) {
    var listaAnexos = obterAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _contestacao.Anexos.val(listaAnexos);
}


function recarregarGridAnexoContestacao() {
    var anexos = obterAnexos();

    _gridAnexo.CarregarGrid(anexos);
}

function obterAnexos() {
    return _contestacao.Anexos.val().slice();
}

function enviarAnexosContestacao(codigo, anexos) {
    var formData = obterFormDataAnexosContestacao(anexos);

    if (!formData)
        return;

    enviarArquivo("Contestacao/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso,"Arquivo procesado com sucesso");
                _contestacao.Anexos.val(retorno.Data.Anexos);
                recarregarGridAnexoContestacao();
            }
            else
                exibirMensagem(tipoMensagem.falha, "Não foi possivel processar arquivo", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function obterFormDataAnexosContestacao(anexos) {
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

function EnviarContestacao() {
    executarReST('Contestacao/SolicitarContestacao', { Codigo: _contestacao.Codigo.val(), Motivo: _contestacao.MotivoContestacao.val() }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, 'Error', arg.msg);

        exibirMensagem(tipoMensagem.ok, 'Sucesso', 'Solicitação de contestação enviada');
        Global.fecharModal("divModalContestacao");


    });
}