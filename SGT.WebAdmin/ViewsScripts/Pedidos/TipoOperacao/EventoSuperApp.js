/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />

/*
* Declaração de Objetos Globais do Arquivo
*/
var _gridEventoSuperApp;

/*
 * Declaração das Funções de Inicialização
 */
function loadGridEventoSuperApp(eventos) {
    var linhasPorPaginas = 5;
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarEventoSuperApp, icone: "" };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: function (id) { removerEventoSuperApp(id); }, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoEditar, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Tipo", visible: false },
        { data: "TipoDescricao", title: Localization.Resources.Pedidos.TipoOperacao.TipoEventoSuperApp, width: "20%", className: "text-align-left" },
        { data: "Titulo", title: Localization.Resources.Pedidos.TipoOperacao.TituloEventoSuperApp, width: "20%", className: "text-align-left" },
        { data: "Obrigatorio", visible: false },
        { data: "ObrigatorioDescricao", title: Localization.Resources.Pedidos.TipoOperacao.ObrigatorioEventoSuperApp, width: "15%", className: "text-align-left" },
        { data: "Ordem", title: Localization.Resources.Pedidos.TipoOperacao.OrdemEventoSuperApp, width: "5%", className: "text-align-left" },
        { data: "TipoEventoCustomizado", visible: false },
        { data: "TipoEventoCustomizadoDescricao", title: Localization.Resources.Pedidos.TipoOperacao.TipoEventoCustomizadoSuperApp, width: "20%", className: "text-align-left" },
        { data: "TipoParada", visible: false },
        { data: "TipoParadaDescricao", title: Localization.Resources.Pedidos.TipoOperacao.TipoParadaEventoSuperApp, width: "10%", className: "text-align-left" },
        { data: "ChecklistSuperApp.Descricao", title: Localization.Resources.Pedidos.TipoOperacao.ChecklistEventoSuperApp, width: "10%", className: "text-align-left" },
        { data: "ChecklistSuperApp.Codigo", visible: false },
        { data: "ChecklistSuperApp", visible: false },
    ];

    _gridEventoSuperApp = new BasicDataTable(_tipoOperacao.GridEventosSuperApp.id, header, menuOpcoes, [{ column: 6, dir: orderDir.desc }, { column: 10, dir: orderDir.desc }], null, linhasPorPaginas);
    _gridEventoSuperApp.CarregarGrid(eventos);
}

/*
 * Declaração das Funções Públicas
 */
function limparCamposCadastroEventoSuperAppClick() {
    limparCamposCadastroEventoSuperApp();
    Global.fecharModal('divModalCadastroEvento');
}
function obterTipoOperacaoEventoSuperAppSalvar(tipoOperacao) {
    const eventos = obterListaEventoSuperApp();
    tipoOperacao["ListaEventosSuperApp"] = JSON.stringify(eventos);
}

/*
 * Declaração das Funções
 */
function obterListaEventoSuperApp() {
    return _gridEventoSuperApp.BuscarRegistros();
}

function removerEventoSuperApp(registroSelecionado) {
    var listaEventoSuperApp = obterListaEventoSuperApp();

    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Evento: " + registroSelecionado.Titulo + "?", function () {

        for (var i = 0; i < listaEventoSuperApp.length; i++) {
            if (registroSelecionado.Codigo == listaEventoSuperApp[i].Codigo) {
                listaEventoSuperApp.splice(i, 1);
                break;
            }
        }
        _gridEventoSuperApp.CarregarGrid(listaEventoSuperApp);
    });
}
function editarEventoSuperApp(evento) {
    var listaEventoSuperApp = obterListaEventoSuperApp();
    var cadastroEventoSuperApp;

    $.each(listaEventoSuperApp, function (i, item) {
        if (evento.Codigo == item.Codigo) {
            cadastroEventoSuperApp = item;
            return false;
        }
    });
    if (cadastroEventoSuperApp != null) {
        _tipoOperacao.CodigoEventoSuperApp.val(cadastroEventoSuperApp.Codigo);
        _tipoOperacao.TipoEventoSuperApp.val(cadastroEventoSuperApp.Tipo.toString());
        _tipoOperacao.TituloEventoSuperApp.val(cadastroEventoSuperApp.Titulo);
        _tipoOperacao.ObrigatorioEventoSuperApp.val(cadastroEventoSuperApp.Obrigatorio);
        _tipoOperacao.OrdemEventoSuperApp.val(cadastroEventoSuperApp.Ordem);
        _tipoOperacao.TipoEventoCustomizadoSuperApp.val(cadastroEventoSuperApp.TipoEventoCustomizado.toString());
        _tipoOperacao.TipoParadaEventoSuperApp.val(cadastroEventoSuperApp.TipoParada.toString());
        PreencherObjetoKnout(_tipoOperacao, { Data: { ChecklistEventoSuperApp: cadastroEventoSuperApp.ChecklistSuperApp } });


        _tipoOperacao.AdicionarCadastroEventoSuperApp.visible(false);
        _tipoOperacao.AtualizarCadastroEventoSuperApp.visible(true);
        _tipoOperacao.CancelarCadastroEventoSuperApp.visible(true);
        exibirModalEventoCadastro();
    }
}

function validarSequencia(ordens, tipoParada) {
    if (!ordens.length) return true;
    const max = ordens.length;
    for (let i = 1; i <= max; i++) {
        if (!ordens.find(ordem => ordem == i)) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, `A próxima ordem do evento de "${tipoParada}" precisa estar na ordem ${i}`);
            return false;
        }
    }
    return true
}
function validarOrdens(evento, estaAtualizando, listaEventoSuperApp) {

    let listaEvento = [...listaEventoSuperApp];

    if (estaAtualizando) {
        const index = listaEvento.findIndex(e => e.Codigo === evento.Codigo);
        if (index !== -1) {
            listaEvento[index] = evento;
        }
    } else {
        listaEvento.push(evento);
    }

    const ordens = listaEvento.map(e => e.Ordem);

    const ordensUnicas = [...new Set(ordens)];
    if (ordens.length !== ordensUnicas.length) {
        exibirMensagem(
            tipoMensagem.atencao,
            Localization.Resources.Gerais.Geral.Atencao,
            `Já existe um evento com a ordem ${evento.Ordem} cadastrado.`
        );
        return false;
    }

    return true;
}
function adicionarListaCadastroEventoSuperAppClick(dados) {
    var evento = obterObjetoEventoSuperApp(dados);
    var listaEventoSuperApp = obterListaEventoSuperApp();
    if (validarEvento(evento, listaEventoSuperApp) && validarOrdens(evento, false, listaEventoSuperApp))  {
        listaEventoSuperApp.push(evento);
        _gridEventoSuperApp.CarregarGrid(listaEventoSuperApp);
        limparCamposCadastroEventoSuperApp();
        Global.fecharModal('divModalCadastroEvento');
    }
}
function atualizarListaCadastroEventoSuperAppClick(dados) {
    var evento = obterObjetoEventoSuperApp(dados);
    var listaEventoSuperApp = obterListaEventoSuperApp();
    if (validarEvento(evento, listaEventoSuperApp) && validarOrdens(evento, true, listaEventoSuperApp)) {
        for (var i = 0; i < listaEventoSuperApp.length; i++) {
            if (evento.Codigo == listaEventoSuperApp[i].Codigo) {
                listaEventoSuperApp[i] = evento;
                break;
            }
        }
        _gridEventoSuperApp.CarregarGrid(listaEventoSuperApp);
        limparCamposCadastroEventoSuperApp();
        Global.fecharModal('divModalCadastroEvento');
    }
}

function obterObjetoEventoSuperApp(dados) {
    var codigo = dados.CodigoEventoSuperApp.val() ? dados.CodigoEventoSuperApp.val() : guid();
    return {
        Codigo: codigo,
        Tipo: dados.TipoEventoSuperApp.val(),
        TipoDescricao: EnumTipoEventoSuperApp.obterDescricao(dados.TipoEventoSuperApp.val()),
        Titulo: dados.TituloEventoSuperApp.val(),
        Obrigatorio: dados.ObrigatorioEventoSuperApp.val(),
        ObrigatorioDescricao: dados.ObrigatorioEventoSuperApp.val() ? 'Sim' : 'Não',
        Ordem: dados.OrdemEventoSuperApp.val(),
        TipoEventoCustomizado: dados.TipoEventoCustomizadoSuperApp.val(),
        TipoEventoCustomizadoDescricao: EnumTipoCustomEventAppTrizy.obterDescricao(dados.TipoEventoCustomizadoSuperApp.val()),
        TipoParada: dados.TipoParadaEventoSuperApp.val(),
        TipoParadaDescricao: EnumTipoParadaEventoSuperApp.obterDescricao(dados.TipoParadaEventoSuperApp.val()),
        ChecklistSuperApp: { Codigo: dados.ChecklistEventoSuperApp.codEntity(), Descricao: dados.ChecklistEventoSuperApp.val() },
    };
}

function limparCamposCadastroEventoSuperApp() {
    _tipoOperacao.CodigoEventoSuperApp.val(_tipoOperacao.CodigoEventoSuperApp.def());
    _tipoOperacao.TipoEventoSuperApp.val(_tipoOperacao.TipoEventoSuperApp.def());
    _tipoOperacao.TituloEventoSuperApp.val(_tipoOperacao.TituloEventoSuperApp.def());
    _tipoOperacao.ObrigatorioEventoSuperApp.val(_tipoOperacao.ObrigatorioEventoSuperApp.def());
    _tipoOperacao.OrdemEventoSuperApp.val(_tipoOperacao.OrdemEventoSuperApp.def());
    _tipoOperacao.TipoEventoCustomizadoSuperApp.val(_tipoOperacao.TipoEventoCustomizadoSuperApp.def());
    _tipoOperacao.TipoParadaEventoSuperApp.val(_tipoOperacao.TipoParadaEventoSuperApp.def());
    LimparCampo(_tipoOperacao.ChecklistEventoSuperApp);

    _tipoOperacao.AdicionarCadastroEventoSuperApp.visible(true);
    _tipoOperacao.AtualizarCadastroEventoSuperApp.visible(false);
    _tipoOperacao.CancelarCadastroEventoSuperApp.visible(false);
}

function validarEvento(evento, listaEventoSuperApp) {
    if (typeof evento.Tipo != "number") {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Tipo do evento é obrigatório.");
        return false;
    }
    if (!evento.Titulo) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Título do evento é obrigatório.");
        return false;
    }
    if (!evento.Ordem) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Ordem do evento é obrigatória.");
        return false;
    }
    if (typeof evento.TipoParada != "number") {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Tipo de parada é obrigatório.");
        return false;
    }
    if (evento.Tipo === EnumTipoEventoSuperApp.Customizado && typeof evento.TipoEventoCustomizado != "number") {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Eventos customizados precisam definir o tipo do evento customizado.");
        return false;
    }

    if (evento.Tipo === EnumTipoEventoSuperApp.EvidenciasDaEntrega && !evento.ChecklistSuperApp.Descricao) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, `Para eventos do tipo ${EnumTipoEventoSuperApp.obterDescricao(EnumTipoEventoSuperApp.EvidenciasDaEntrega)} é necessário enviar um checklist.`);
        return false;
    }

    if (listaEventoSuperApp.length) {
        if (
            evento.Tipo !== EnumTipoEventoSuperApp.Customizado &&
            listaEventoSuperApp.filter(
                (eventoLista) =>
                    evento.Codigo !== eventoLista.Codigo &&
                    evento.Tipo === eventoLista.Tipo &&
                    (eventoLista.TipoParada === evento.TipoParada ||
                        eventoLista.TipoParada === EnumTipoParadaEventoSuperApp.Ambos)
            ).length
        ) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, `Já existe um evento do tipo ${EnumTipoEventoSuperApp.obterDescricao(evento.Tipo)} para esse tipo de parada.`);
            return false;
        }
        if (
            evento.Tipo === EnumTipoEventoSuperApp.Customizado &&
            listaEventoSuperApp.filter(
                (eventoLista) =>
                    evento.Codigo !== eventoLista.Codigo &&
                    evento.Tipo === eventoLista.Tipo &&
                    evento.TipoEventoCustomizado === eventoLista.TipoEventoCustomizado &&
                    (eventoLista.TipoParada === evento.TipoParada ||
                        eventoLista.TipoParada === EnumTipoParadaEventoSuperApp.Ambos)
            ).length
        ) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, `Já existe um evento customizado do tipo ${EnumTipoCustomEventAppTrizy.obterDescricao(evento.TipoEventoCustomizado)} para esse tipo de parada.`);
            return false;
        }
    }

    return true;
}

function preencherGridEventosSuperApp(eventos, duplicar) {
    // Caso seja duplicação, deve remover o id do que esta sendo duplicado
    if (duplicar) {
        eventos.forEach(evento => {
            evento.Codigo = guid();
        })
    }
    _gridEventoSuperApp.CarregarGrid(eventos);
}