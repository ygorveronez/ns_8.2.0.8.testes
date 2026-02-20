var PedidoDetalheEntrega = function () {
    var _detalheEntrega;
    var _detalheEntregaResumo;
    var _gridNotasFiscais;
    var _mapaOrigemCarga;

    /**
     * Definições Knockout
     */
    var DetalheEntrega = function () {
        this.Codigo = PropertyEntity({ val: ko.observable(0) });
        this.Grid = PropertyEntity({});
        this.Mapa = PropertyEntity({});
    };

    var DetalheEntregaResumo = function () {
        this.Cliente = PropertyEntity({ val: ko.observable(""), text: "Cliente" });
        this.DocumentoCliente = PropertyEntity({ val: ko.observable(""), text: "CNPJ" });
        this.Destino = PropertyEntity({ val: ko.observable(""), text: "Destino" });

        this.CTe = PropertyEntity({ val: ko.observable(""), text: "CTe" });
        this.DataEmissao = PropertyEntity({ val: ko.observable(""), text: "Emissão" });
        this.DataEntrega = PropertyEntity({ val: ko.observable(""), text: "Data da entrega" });
        this.Contato = PropertyEntity({ val: ko.observable(""), text: "Contato" });
    };

    /**
     * Eventos Knockout
     */


    /**
     * Métodos Público
     */
    this.Load = function (id) {
        _detalheEntrega = new DetalheEntrega();
        KoBindings(_detalheEntrega, id);

        _detalheEntregaResumo = new DetalheEntregaResumo();
        KoBindings(_detalheEntregaResumo, id + "Resumo");

        loadDetalhesPedidos();
        carregarGridNotas();
        
        _mapaOrigemCarga = new MapaDetalheEntrega(_detalheEntrega.Mapa.id);
    }

    this.PreencherDados = function (dados) {
        PreencherObjetoKnout(_detalheEntrega, { Data: dados });
        PreencherObjetoKnout(_detalheEntregaResumo, { Data: dados });

        adicionarOrigemMapa(dados.InformacoesLocalizacaoCliente);

        _gridNotasFiscais.CarregarGrid();
    }

    /**
     * Métodos Privados
     */
    var loadDetalhesPedidos = function () {
    }

    var carregarGridNotas = function () {
        _gridNotasFiscais = new GridView(_detalheEntrega.Grid.id, "PedidoCliente/PesquisaNotas", { Codigo: _detalheEntrega.Codigo });

        _gridNotasFiscais.SetHabilitarLinhaClicavel(true);
        _gridNotasFiscais.OnClickLinha(function (data) {
            _modalCanhotosPedidos.CarregarCanhotoNota(data);
        });

        _gridNotasFiscais.SetDefinicaoColunas({
            DescricaoStatus: function (data, row) {
                return '<span class="label label-warning">' + data + '</span >';
            },
            DescricaoStatusCanhoto: function (data, row) {
                return '<span class="label label-warning">' + data + '</span >';
            },
        });
    }

    var adicionarOrigemMapa = function (InformacoesLocalizacaoCliente) {
        var icone = "http://maps.google.com/mapfiles/kml/pal2/icon13.png";
        var titulo = "Origem da Carga";
        
        _mapaOrigemCarga._adicionarMarcadorPorCoordenadas(InformacoesLocalizacaoCliente.Coordenadas.Latitude, InformacoesLocalizacaoCliente.Coordenadas.Longitude, InformacoesLocalizacaoCliente.Endereco, titulo, icone);
    }
}