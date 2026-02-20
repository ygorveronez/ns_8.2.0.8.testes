/// <reference path="CentroCarregamento.js" />
/// <reference path="../../Consultas/Usuario.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridUsuariosNotificacao;
var _listaUsuariosNotificacao;

/*
 * Declaração das Classes
 */

var ListaUsuariosNotificacao = function () {
    this.NotificarSomenteAlteracaoCotacao = PropertyEntity({ val: _centroCarregamento.NotificarSomenteAlteracaoCotacao.val, getType: typesKnockout.bool, text: _centroCarregamento.NotificarSomenteAlteracaoCotacao.text, def: _centroCarregamento.NotificarSomenteAlteracaoCotacao.def });
    this.NaoEnviarNotificacaoCargaRejeitadaParaTransportador = PropertyEntity({ val: _centroCarregamento.NaoEnviarNotificacaoCargaRejeitadaParaTransportador.val, getType: typesKnockout.bool, text: _centroCarregamento.NaoEnviarNotificacaoCargaRejeitadaParaTransportador.text, def: _centroCarregamento.NaoEnviarNotificacaoCargaRejeitadaParaTransportador.def });
    this.Grid = PropertyEntity({ type: types.local });

    this.Adicionar = PropertyEntity({ idBtnSearch: guid(), type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: true });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadUsuariosNotificacao() {
    _listaUsuariosNotificacao = new ListaUsuariosNotificacao();
    KoBindings(_listaUsuariosNotificacao, "knockoutUsuariosNotificacao");

    loadGridUsuariosNotificacao();
    new BuscarFuncionario(_listaUsuariosNotificacao.Adicionar, null, _gridUsuariosNotificacao);
}

function loadGridUsuariosNotificacao() {
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerUsuariosNotificacaoClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Nome", title: Localization.Resources.Gerais.Geral.Nome, width: "60%" },
        { data: "CPF_Formatado", title: Localization.Resources.Logistica.CentroCarregamento.CPF, width: "30%" },
    ];

    _gridUsuariosNotificacao = new BasicDataTable(_listaUsuariosNotificacao.Grid.id, header, menuOpcoes);

    RecarregarGridUsuariosNotificacao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */
function removerUsuariosNotificacaoClick(registroSelecionado) {
    var usuarios = _gridUsuariosNotificacao.BuscarRegistros();

    for (var i = 0; i < usuarios.length; i++) {
        if (registroSelecionado.Codigo == usuarios[i].Codigo) {
            usuarios.splice(i, 1);
            break;
        }
    }

    _gridUsuariosNotificacao.CarregarGrid(usuarios);
}

/*
 * Declaração das Funções
 */

function ObterUsuariosNotificacoes() {
    return JSON.stringify(_gridUsuariosNotificacao.BuscarRegistros().map(function (u) { return u.Codigo }));
}

function RecarregarGridUsuariosNotificacao() {
    _gridUsuariosNotificacao.CarregarGrid(_centroCarregamento.UsuariosNotificacao.val() || []);
}