/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />

/*
* Declaração de Objetos Globais do Arquivo
*/
var _gridInformacoesAdicionaisRelatorioViagemSuperApp;

/*
 * Declaração das Funções de Inicialização
 */
function loadGridInformacoesAdicionaisRelatorioViagemSuperApp(informacoesAdicionais) {
    var linhasPorPaginas = 5;
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarInformacaoAdicionalRelatorioViagem, icone: "" };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerInformacaoAdicionalRelatorioViagem, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoEditar, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Rotulo", title: Localization.Resources.Pedidos.TipoOperacao.RotuloInformacaoAdicionalReciboViagemSuperApp, width: "40%", className: "text-align-left" },
        { data: "Descricao", title: Localization.Resources.Pedidos.TipoOperacao.DescricaoInformacaoAdicionalReciboViagemSuperApp, width: "40%", className: "text-align-left" },
    ];

    _gridInformacoesAdicionaisRelatorioViagemSuperApp = new BasicDataTable(_tipoOperacao.GridInformacoesAdicionaisRelatorioViagemSuperApp.id, header, menuOpcoes, [{ column: 1, dir: orderDir.desc }], null, linhasPorPaginas);
    _gridInformacoesAdicionaisRelatorioViagemSuperApp.CarregarGrid(informacoesAdicionais);
}

/*
 * Declaração das Funções Públicas
 */
function limparCamposCadastroInformacaoAdicionalRelatorioViagemClick() {
    limparCamposCadastroInformacaoAdicionalRelatorioViagem();
    Global.fecharModal('divModalCadastroInformacaoAdicionalRelatorioViagem');
}
function obterTipoOperacaoInformacaoAdicionalRelatorioViagemSuperAppSalvar(tipoOperacao) {
    const informacoesAdicionais = obterListaInformacaoAdicionalRelatorioViagem();
    tipoOperacao["ListaInformacaoAdicionalRelatorioViagemSuperApp"] = JSON.stringify(informacoesAdicionais);
}

/*
 * Declaração das Funções
 */
function obterListaInformacaoAdicionalRelatorioViagem() {
    return _gridInformacoesAdicionaisRelatorioViagemSuperApp.BuscarRegistros();
}

function removerInformacaoAdicionalRelatorioViagem(registroSelecionado) {
    var listaInformacoesAdicionais = obterListaInformacaoAdicionalRelatorioViagem();

    for (var i = 0; i < listaInformacoesAdicionais.length; i++) {
        if (registroSelecionado.Codigo == listaInformacoesAdicionais[i].Codigo) {
            listaInformacoesAdicionais.splice(i, 1);
            break;
        }
    }
    _gridInformacoesAdicionaisRelatorioViagemSuperApp.CarregarGrid(listaInformacoesAdicionais);
}
function editarInformacaoAdicionalRelatorioViagem(registroSelecionado) {
    var listaInformacoesAdicionais = obterListaInformacaoAdicionalRelatorioViagem();
    var cadastroInformacaoAdicional;

    $.each(listaInformacoesAdicionais, function (i, item) {
        if (registroSelecionado.Codigo == item.Codigo) {
            cadastroInformacaoAdicional = item;
            return false;
        }
    });
    if (cadastroInformacaoAdicional != null) {
        _tipoOperacao.CodigoInformacaoAdicionalReciboViagemSuperApp.val(cadastroInformacaoAdicional.Codigo);
        _tipoOperacao.RotuloInformacaoAdicionalReciboViagemSuperApp.val(cadastroInformacaoAdicional.Rotulo);
        _tipoOperacao.DescricaoInformacaoAdicionalReciboViagemSuperApp.val(cadastroInformacaoAdicional.Descricao);

        _tipoOperacao.AdicionarCadastroInformacaoAdicionalRelatorioViagemSuperApp.visible(false);
        _tipoOperacao.AtualizarCadastroInformacaoAdicionalRelatorioViagemSuperApp.visible(true);
        _tipoOperacao.CancelarCadastroInformacaoAdicionalRelatorioViagemSuperApp.visible(true);
        exibirModalInformacaoAdicionalRelatorioViagem();
    }
}

function validarInformacaoAdicional(informacaoAdicional) {
    if (!informacaoAdicional.Rotulo) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "O Rótulo é obrigatório.");
        return false;
    }
    if (!informacaoAdicional.Descricao) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "A descrição é obrigatória.");
        return false;
    }
    return true;
}

function adicionarListaCadastroInformacaoAdicionalRelatorioViagemClick(dados) {
    var informacaoAdicional = obterObjetoInformacaoAdicionalRelatorioViagem(dados);
    var listaInformacoesAdicionais = obterListaInformacaoAdicionalRelatorioViagem();
    if (validarInformacaoAdicional(informacaoAdicional))  {
        listaInformacoesAdicionais.push(informacaoAdicional);
        _gridInformacoesAdicionaisRelatorioViagemSuperApp.CarregarGrid(listaInformacoesAdicionais);
        limparCamposCadastroInformacaoAdicionalRelatorioViagem();
        Global.fecharModal('divModalCadastroInformacaoAdicionalRelatorioViagem');
    }
}
function atualizarListaCadastroInformacaoAdicionalRelatorioViagemClick(dados) {
    var informacaoAdicional = obterObjetoInformacaoAdicionalRelatorioViagem(dados);
    var listaInformacoesAdicionais = obterListaInformacaoAdicionalRelatorioViagem();
    if (validarInformacaoAdicional(informacaoAdicional)) {
        for (var i = 0; i < listaInformacoesAdicionais.length; i++) {
            if (informacaoAdicional.Codigo == listaInformacoesAdicionais[i].Codigo) {
                listaInformacoesAdicionais[i] = informacaoAdicional;
                break;
            }
        }
        _gridInformacoesAdicionaisRelatorioViagemSuperApp.CarregarGrid(listaInformacoesAdicionais);
        limparCamposCadastroInformacaoAdicionalRelatorioViagem();
        Global.fecharModal('divModalCadastroInformacaoAdicionalRelatorioViagem');
    }
}
function obterObjetoInformacaoAdicionalRelatorioViagem(dados) {
    var codigo = dados.CodigoInformacaoAdicionalReciboViagemSuperApp.val() ? dados.CodigoInformacaoAdicionalReciboViagemSuperApp.val() : guid();
    return {
        Codigo: codigo,
        Rotulo: dados.RotuloInformacaoAdicionalReciboViagemSuperApp.val(),
        Descricao: dados.DescricaoInformacaoAdicionalReciboViagemSuperApp.val(),
    };
}

function limparCamposCadastroInformacaoAdicionalRelatorioViagem() {
    _tipoOperacao.CodigoInformacaoAdicionalReciboViagemSuperApp.val(_tipoOperacao.CodigoInformacaoAdicionalReciboViagemSuperApp.def());
    _tipoOperacao.RotuloInformacaoAdicionalReciboViagemSuperApp.val(_tipoOperacao.RotuloInformacaoAdicionalReciboViagemSuperApp.def());
    _tipoOperacao.DescricaoInformacaoAdicionalReciboViagemSuperApp.val(_tipoOperacao.DescricaoInformacaoAdicionalReciboViagemSuperApp.def());

    _tipoOperacao.AdicionarCadastroInformacaoAdicionalRelatorioViagemSuperApp.visible(true);
    _tipoOperacao.AtualizarCadastroInformacaoAdicionalRelatorioViagemSuperApp.visible(false);
    _tipoOperacao.CancelarCadastroInformacaoAdicionalRelatorioViagemSuperApp.visible(false);
}

function preencherGridInformacaoAdicionalRelatorioViagem(informacoesAdicionais, duplicar) {
    // Caso seja duplicação, deve remover o id do que esta sendo duplicado
    if (duplicar) {
        informacoesAdicionais.forEach(informacaoAdicional => {
            informacaoAdicional.Codigo = guid();
        })
    }
    _gridInformacoesAdicionaisRelatorioViagemSuperApp.CarregarGrid(informacoesAdicionais);
}