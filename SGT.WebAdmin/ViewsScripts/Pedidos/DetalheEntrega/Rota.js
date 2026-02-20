var RotaDetalheEntrega = function () {
    var _map;
    var _rota;
    var _gridNotasFiscaisRota;

    /**
     * Definições Knockout
     */
    var Rota = function () {
        this.CodigoPedido = PropertyEntity({ val: ko.observable(0) });
        
        this.Ocorrencias = PropertyEntity({ val: ko.observableArray([]) });
        this.DataPrevista = PropertyEntity({ val: ko.observable("") });
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
        _rota = new Rota();
        KoBindings(_rota, id);

        _map = new MapaDetalheEntrega(_rota.Mapa.id);
        carregarGridNotasRota();
    }

    this.PreencherDados = function (dados) {
        PreencherObjetoKnout(_rota, { Data: dados });

        if (dados.Polilinhas == null)
            return;

        _map.carregarPolilinhasPlanejadas(dados.Polilinhas.Planejadas);
        _map.carregarPolilinhasRealizadas(dados.Polilinhas.Realizadas);

        this.CentralizarMapa();
        _gridNotasFiscaisRota.CarregarGrid();
    }

    this.CentralizarMapa = function (dados) {
        _map.centralizarMapa();
    }

    /**
     * Métodos Privados
     */
    var loadDetalhesPedidos = function () {
    }
    
    var carregarGridNotasRota = function () {
        _gridNotasFiscaisRota = new GridView(_rota.Grid.id, "PedidoCliente/PesquisaNotas", { Codigo: _rota.CodigoPedido });

        _gridNotasFiscaisRota.SetHabilitarLinhaClicavel(true);
        _gridNotasFiscaisRota.OnClickLinha(function (data) {
            _modalCanhotosPedidos.CarregarCanhotoNota(data);
        });

        _gridNotasFiscaisRota.SetDefinicaoColunas({
            DescricaoStatus: function (data, row) {
                return '<span class="label label-warning">' + data + '</span >';
            },
            DescricaoStatusCanhoto: function (data, row) {
                return '<span class="label label-warning">' + data + '</span >';
            },
        });

        _gridNotasFiscaisRota.CarregarGrid();
    }
}