/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />

/*
* Declaração de Objetos Globais do Arquivo
*/
var _gridNotificacaoApp;

/*
 * Declaração das Funções de Inicialização
 */
function loadGridNotificacaoApp(notificacoes) {
    var linhasPorPaginas = 5;
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarNotificacaoApp, icone: "" };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerNotificacaoApp, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoEditar, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Tipo", visible: false },
        { data: "TipoDescricao", title: Localization.Resources.Pedidos.TipoOperacao.TipoDaNotificacao, width: "15%", className: "text-align-left" },
        { data: "Titulo", title: Localization.Resources.Pedidos.TipoOperacao.TituloDaNotificacao, width: "30%", className: "text-align-left" },
        { data: "Mensagem", title: Localization.Resources.Pedidos.TipoOperacao.MensagemDaNotificacao, width: "55%", className: "text-align-left" }
    ];

    _gridNotificacaoApp = new BasicDataTable(_tipoOperacao.GridNotificacoesAppTrizy.id, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridNotificacaoApp.CarregarGrid(notificacoes);
}

/*
 * Declaração das Funções Públicas
 */
function limparCamposCadastroNotificacaoAppClick() {
    limparCamposCadastroNotificacaoApp();
}
function obterTipoOperacaoNotificacaoAppSalvar(tipoOperacao) {
    tipoOperacao["ListaNotificacoesApp"] = JSON.stringify(obterListaNotificacaoApp());
}

/*
 * Declaração das Funções
 */
function obterListaNotificacaoApp() {
    return _gridNotificacaoApp.BuscarRegistros();
}

function removerNotificacaoApp(registroSelecionado) {
    var listaNotificacaoApp = obterListaNotificacaoApp();

    for (var i = 0; i < listaNotificacaoApp.length; i++) {
        if (registroSelecionado.Codigo == listaNotificacaoApp[i].Codigo) {
            listaNotificacaoApp.splice(i, 1);
            break;
        }
    }
    _gridNotificacaoApp.CarregarGrid(listaNotificacaoApp);
}
function editarNotificacaoApp(notificacao) {
    var listaNotificacaoApp = obterListaNotificacaoApp();
    var cadastroNotificacaoApp;

    $.each(listaNotificacaoApp, function (i, item) {
        if (notificacao.Codigo == item.Codigo) {
            cadastroNotificacaoApp = item;
            return false;
        }
    });

    if (cadastroNotificacaoApp != null) {
        _tipoOperacao.CodigoNotificacaoAppTrizy.val(cadastroNotificacaoApp.Codigo);
        _tipoOperacao.TipoNotificacaoAppTrizy.val(cadastroNotificacaoApp.Tipo.toString());
        _tipoOperacao.TituloNotificacaoAppTrizy.val(cadastroNotificacaoApp.Titulo);
        _tipoOperacao.MensagemNotificacaoAppTrizy.val(cadastroNotificacaoApp.Mensagem);

        _tipoOperacao.AdicionarCadastroNotificacaoApp.visible(false);
        _tipoOperacao.AtualizarCadastroNotificacaoApp.visible(true);
        _tipoOperacao.CancelarCadastroNotificacaoApp.visible(true);
    }
}

function adicionarListaCadastroNotificacaoAppClick(dados) {
    var notificacao = obterObjetoNotificacaoApp(dados);
    if (notificacaoValida(notificacao)) {
        var listaNotificacaoApp = obterListaNotificacaoApp();
        listaNotificacaoApp.push(notificacao);
        _gridNotificacaoApp.CarregarGrid(listaNotificacaoApp);
        limparCamposCadastroNotificacaoApp();
    }
}
function atualizarListaCadastroNotificacaoAppClick(dados) {
    var notificacao = obterObjetoNotificacaoApp(dados);
    if (notificacaoValida(notificacao)) {
        var listaNotificacaoApp = obterListaNotificacaoApp();

        for (var i = 0; i < listaNotificacaoApp.length; i++) {
            if (notificacao.Codigo == listaNotificacaoApp[i].Codigo) {
                listaNotificacaoApp[i] = notificacao;
                break;
            }
        }
        _gridNotificacaoApp.CarregarGrid(listaNotificacaoApp);
        limparCamposCadastroNotificacaoApp();
    }
}

function obterObjetoNotificacaoApp(dados) {
    var codigo = dados.CodigoNotificacaoAppTrizy.val() ? dados.CodigoNotificacaoAppTrizy.val() : guid();
    return {
        Codigo: codigo,
        Tipo: dados.TipoNotificacaoAppTrizy.val(),
        TipoDescricao: EnumTipoNotificacaoApp.obterDescricao(dados.TipoNotificacaoAppTrizy.val()),
        Titulo: dados.TituloNotificacaoAppTrizy.val(),
        Mensagem: dados.MensagemNotificacaoAppTrizy.val()
    };
}

function limparCamposCadastroNotificacaoApp() {
    _tipoOperacao.CodigoNotificacaoAppTrizy.val(_tipoOperacao.CodigoNotificacaoAppTrizy.def());
    _tipoOperacao.TipoNotificacaoAppTrizy.val(_tipoOperacao.TipoNotificacaoAppTrizy.def());
    _tipoOperacao.TituloNotificacaoAppTrizy.val(_tipoOperacao.TituloNotificacaoAppTrizy.def());
    _tipoOperacao.MensagemNotificacaoAppTrizy.val(_tipoOperacao.MensagemNotificacaoAppTrizy.def());

    _tipoOperacao.AdicionarCadastroNotificacaoApp.visible(true);
    _tipoOperacao.AtualizarCadastroNotificacaoApp.visible(false);
    _tipoOperacao.CancelarCadastroNotificacaoApp.visible(false);
}

function notificacaoValida(notificacao) {
    if (typeof notificacao.Tipo != "number") {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Tipo da Notificação é obrigatório.");
        return false;
    }
    if (!notificacao.Titulo) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Título da Notificação é obrigatório.");
        return false;
    }
    if (!notificacao.Mensagem) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Mensagem da Notificação é obrigatória.");
        return false;
    }

    var listaNotificacaoApp = obterListaNotificacaoApp();
    for (var i = 0; i < listaNotificacaoApp.length; i++) {
        if (notificacao.Codigo != listaNotificacaoApp[i].Codigo && notificacao.Tipo == listaNotificacaoApp[i].Tipo) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Já existe uma Notificação cadastrada para esse Tipo.");
            return false;
        }
    }

    return true;
}

function preencherGridNotificacoesAppTrizy(notificacoes, duplicar) {
    // Caso seja duplicação, deve remover o id do que esta sendo duplicado
    if (duplicar) {
        notificacoes.forEach(notificacao => {
            notificacao.Codigo = guid();
        })
    }
    _gridNotificacaoApp.CarregarGrid(notificacoes);
}