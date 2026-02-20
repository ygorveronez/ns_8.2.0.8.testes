var EtapaDetalheEntrega = function () {
    var _etapasEntrega;
    var _transferencia;
    var _rota;

    /**
     * Definições Knockout
     */
    var EtapasEntrega = function () {
        this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("25%") });

        this.Etapa1 = PropertyEntity({
            text: "Pedido", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
            step: ko.observable(1),
        });
        this.Etapa2 = PropertyEntity({
            text: "Em transferência", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
            step: ko.observable(2),
        });
        this.Etapa3 = PropertyEntity({
            text: "Em rota", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
            step: ko.observable(3),
        });
        this.Etapa4 = PropertyEntity({
            text: "Entregue", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
            step: ko.observable(4),
        });
    };

    /**
     * Eventos Knockout
     */
    function centralizarMapaEmTransferencia() {
        setTimeout(function () {
            _transferencia.CentralizarMapa();
        }, 250);
    }

    function centralizarMapaEmRota() {
        setTimeout(function () {
            _rota.CentralizarMapa();
        }, 250);
    }


    /**
     * Métodos Público
     */
    this.Load = function (id, dependencias) {
        _etapasEntrega = new EtapasEntrega();
        KoBindings(_etapasEntrega, id);
        bloquarTodasEtapas();

        _transferencia = dependencias._transferencia;
        _rota = dependencias._rota;
    }

    this.CarregarEtapaPorSituacao = function (situacao) {
        switch (situacao) {
            case EnumSituacaoAcompanhamentoPedido.EmTransporte:
            case EnumSituacaoAcompanhamentoPedido.ProblemaNoTransporte:
                return etapa2Liberada();

            case EnumSituacaoAcompanhamentoPedido.EntregaRejeitada:
            case EnumSituacaoAcompanhamentoPedido.SaiuParaEntrega:
                return etapa3Liberada();

            case EnumSituacaoAcompanhamentoPedido.Entregue:
                return etapa4Liberada();

            default:
                //case EnumSituacaoAcompanhamentoPedido.AgColeta:
                //case EnumSituacaoAcompanhamentoPedido.ColetaAgendada:
                //case EnumSituacaoAcompanhamentoPedido.ColetaRejeitada:
                return etapa1Liberada();
        }
    }

    /**
     * Métodos Privados
     */
    var bloquarTodasEtapas = function () {
        etapa2Desabilitada();
        etapa3Desabilitada();
        etapa4Desabilitada();
    }


    var etapa1Liberada = function () {
        $("#" + _etapasEntrega.Etapa1.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapasEntrega.Etapa1.idTab + " .step").attr("class", "step yellow");
    }


    var etapa2Liberada = function () {
        $("#" + _etapasEntrega.Etapa2.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapasEntrega.Etapa2.idTab + " .step").attr("class", "step yellow");
        etapa1Liberada();
        _etapasEntrega.Etapa2.eventClick = centralizarMapaEmTransferencia;
    }

    var etapa2Desabilitada = function () {
        $("#" + _etapasEntrega.Etapa2.idTab).removeAttr("data-bs-toggle");
        $("#" + _etapasEntrega.Etapa2.idTab + " .step").attr("class", "step");
        _etapasEntrega.Etapa2.eventClick = null;
    }


    var etapa3Liberada = function () {
        $("#" + _etapasEntrega.Etapa3.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapasEntrega.Etapa3.idTab + " .step").attr("class", "step yellow");
        etapa1Liberada();
        etapa2Liberada();
        _etapasEntrega.Etapa3.eventClick = centralizarMapaEmRota;
    }

    var etapa3Desabilitada = function () {
        $("#" + _etapasEntrega.Etapa3.idTab).removeAttr("data-bs-toggle");
        $("#" + _etapasEntrega.Etapa3.idTab + " .step").attr("class", "step");
        _etapasEntrega.Etapa3.eventClick = null;
    }


    var etapa4Liberada = function () {
        $("#" + _etapasEntrega.Etapa4.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapasEntrega.Etapa4.idTab + " .step").attr("class", "step yellow");
        etapa1Liberada();
        etapa2Liberada();
        etapa3Liberada();
    }

    var etapa4Desabilitada = function () {
        $("#" + _etapasEntrega.Etapa4.idTab).removeAttr("data-bs-toggle");
        $("#" + _etapasEntrega.Etapa4.idTab + " .step").attr("class", "step");
    }
}