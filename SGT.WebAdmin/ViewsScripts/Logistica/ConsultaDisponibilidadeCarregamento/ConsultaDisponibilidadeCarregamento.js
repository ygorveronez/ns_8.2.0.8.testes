/// <reference path="DisponibilidadeCarregamento.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaConsultaDisponibilidadeCarregamento;
var _centroCarregamentoAtual;
var _disponibilidadeCarregamento;
var _dadosPesquisaCarregamento;

var ConsultaDisponibilidadeCarregamento = function () {
    this.DataCarregamento = PropertyEntity({ text: "Data de Carregamento:", getType: typesKnockout.date });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de Carregamento:", idBtnSearch: guid(), required: true });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            CarregarDadosPesquisa();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

//*******EVENTOS*******

function loadConsultaDisponibilidadeCarregamento() {
    // Pesquisa de centro
    _pesquisaConsultaDisponibilidadeCarregamento = new ConsultaDisponibilidadeCarregamento();
    KoBindings(_pesquisaConsultaDisponibilidadeCarregamento, "knockoutPesquisaConsultaDisponibilidadeCarregamento", false, _pesquisaConsultaDisponibilidadeCarregamento.Pesquisar.id);

    // Pesquisa de centro
    new BuscarCentrosCarregamento(_pesquisaConsultaDisponibilidadeCarregamento.CentroCarregamento, null, null, null, true);

    loadDetalhesReserva();
    loadDetalhesEmReserva();
}

function CarregarDadosPesquisa() {
    // Valida filtros
    var valido = ValidarCamposObrigatorios(_pesquisaConsultaDisponibilidadeCarregamento);

    if (!valido)
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");

    // Objeto da pesquisa
    _dadosPesquisaCarregamento = RetornarObjetoPesquisa(_pesquisaConsultaDisponibilidadeCarregamento);

    executarReST("ConsultaDisponibilidadeCarregamento/ObterInformacoesCentroCarregamento", _dadosPesquisaCarregamento, function (r) {
        if (r.Success) {
            _centroCarregamentoAtual = r.Data;
            RenderizarDadosCarregamento();
            _pesquisaConsultaDisponibilidadeCarregamento.ExibirFiltros.visibleFade(false);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function RenderizarDadosCarregamento() {
    $("#divGeralCarregamento").removeClass("d-none");

    if (_disponibilidadeCarregamento != null)
        _disponibilidadeCarregamento.Destroy();

    _disponibilidadeCarregamento = new DisponibilidadeCarregamento();
    _disponibilidadeCarregamento.CallbackDetalhesReserva = AbrirDetalhesReserva;
    _disponibilidadeCarregamento.CallbackDetalhesEmReserva = AbrirDetalhesEmReserva;
    _disponibilidadeCarregamento.Load();
}