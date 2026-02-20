var TransferenciaDetalheEntrega = function () {
    var _map;
    var _transferencia;
    var _gridNotasFiscaisTransferencia;
    
    /**
     * Definições Knockout
     */
    var Transferencia = function () {
        this.CodigoPedido = PropertyEntity({ val: ko.observable(0) });
        
        this.PossuiEtapaTransferencia = PropertyEntity({ val: ko.observable(true) });
        this.DataPrevista = PropertyEntity({ val: ko.observable("") });
        this.Ocorrencias = PropertyEntity({ val: ko.observableArray([]) });
        this.Mapa = PropertyEntity({});
        this.Grid = PropertyEntity({});
    };

    /**
     * Eventos Knockout
     */
    var NavegarParaTelaPedidos = function () {

    }

    /**
     * Métodos Público
     */
    this.Load = function (id) {
        _transferencia = new Transferencia();
        KoBindings(_transferencia, id);

        _map = new MapaDetalheEntrega(_transferencia.Mapa.id);
        carregarGridNotasTransferencia();
    }

    this.PreencherDados = function (dados) {
        PreencherObjetoKnout(_transferencia, { Data: dados });

        if (dados.Polilinhas == null)
            return;

        _map.carregarPolilinhasPlanejadas(dados.Polilinhas.Planejadas);
        _map.carregarPolilinhasRealizadas(dados.Polilinhas.Realizadas);

        this.CentralizarMapa();
        _gridNotasFiscaisTransferencia.CarregarGrid();
    }

    this.CentralizarMapa = function (dados) {
        _map.centralizarMapa();
    }

    /**
     * Métodos Privados
     */
    var loadDetalhesPedidos = function () {
    }

    var carregarGridNotasTransferencia = function () {
        _gridNotasFiscaisTransferencia = new GridView(_transferencia.Grid.id, "PedidoCliente/PesquisaNotas", { Codigo: _transferencia.CodigoPedido });

        _gridNotasFiscaisTransferencia.SetHabilitarLinhaClicavel(true);
        _gridNotasFiscaisTransferencia.OnClickLinha(function (data) {
            _modalCanhotosPedidos.CarregarCanhotoNota(data);
        });

        _gridNotasFiscaisTransferencia.SetDefinicaoColunas({
            DescricaoStatus: function (data, row) {
                return '<span class="label label-warning">' + data + '</span >';
            },
            DescricaoStatusCanhoto: function (data, row) {
                return '<span class="label label-warning">' + data + '</span >';
            },
        });

        _gridNotasFiscaisTransferencia.CarregarGrid();
    }
}