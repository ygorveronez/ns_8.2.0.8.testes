/// <reference path="../DadosCarga/Carga.js" />
/// <reference path="../DadosCarga/SignalR.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />
/// <reference path="../../../Enumeradores/EnumPermissaoPersonalizada.js" />
/// <reference path="../../../Enumeradores/EnumTipoIntegracao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridCargaDadosTransporteLacres;

/*
 * Declaração das Funções de Inicialização
 */

function loadLacresCargaTransportador(e) {
    $("#liTabCargaDadosTransporteLacres_" + e.EtapaInicioTMS.idGrid).removeClass("d-none");

    var habilitar = VerificarSeCargaEstaNaLogistica(e);
    e.AdicionarLacre.enable(habilitar);

    var excluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: excluirCargaTransportadorLacre, tamanho: 7, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [excluir], tamanho: 15 };

    _gridCargaDadosTransporteLacres = new GridView(e.AdicionarLacre.idGrid, "DadosLacre/Pesquisa", { Carga: e.Codigo }, menuOpcoes, null);
    _gridCargaDadosTransporteLacres.CarregarGrid();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function excluirCargaTransportadorLacre(lacre, e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaExcluirLacre.format(lacre.Numero), function () {
        executarReST("DadosLacre/Excluir", { Codigo: lacre.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.LacreExcluidoComSucesso);
                    _gridCargaDadosTransporteLacres.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function AdicionarCargaTransportadorLacreClick(e) {
    var dados = {
        Carga: e.Codigo.val(),
        Numero: e.NumeroLacre.val()
    };

    executarReST("DadosLacre/Adicionar", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.LacreAdicionadoComSucesso);
                _gridCargaDadosTransporteLacres.CarregarGrid();

                e.NumeroLacre.val(e.NumeroLacre.def);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}


/*
 * Declaração das Funções
 */
