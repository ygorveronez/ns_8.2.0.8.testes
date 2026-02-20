var EntregaDetalheEntrega = function () {
    var self = this;
    var _entrega;
    var _mapaEntrega;
    var _gridNotasFiscaisEntrega;

    /**
     * Definições Knockout
     */
    var Entrega = function () {
        this.CodigoPedido = PropertyEntity({ val: ko.observable(0), def: 0 });
        this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
        this.Avaliacao = PropertyEntity({ estrelas: [5, 4, 3, 2, 1], val: ko.observable(0), def: 0, enable: ko.observable(true) });
        this.Observacao = PropertyEntity({ val: ko.observable(""), def: "", enable: ko.observable(true) });
        this.Mapa = PropertyEntity({});
        this.Grid = PropertyEntity({});
        this.Ocorrencias = PropertyEntity({ val: ko.observableArray([]) });
        this.DataPrevista = PropertyEntity({ val: ko.observable("") });

        this.Salvar = PropertyEntity({ eventClick: onSalvarAvalicacaoClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
    };

    /**
     * Eventos Knockout
     */
    var onSalvarAvalicacaoClick = function () {
        SalvarAvaliacaoPedido();
    }

    /**
     * Métodos Público
     */
    this.Load = function (id) {
        _entrega = new Entrega();
        KoBindings(_entrega, id);

        _mapaEntrega = new MapaDetalheEntrega(_entrega.Mapa.id);
        carregarGridNotasEntrega();
    }

    this.PreencherDados = function (dados, dadosPedido) {
        PreencherObjetoKnout(_entrega, { Data: dados });

        if (dados.AvaliacaoEfetuada) {
            this.BloquearCampos();
        }

        _gridNotasFiscaisEntrega.CarregarGrid();

        if (dados.Polilinhas != null) {
            _mapaEntrega.carregarPolilinhasRealizadas(dados.Polilinhas.Realizadas);
            _mapaEntrega.carregarPolilinhasRealizadas(dados.Polilinhas.Planejadas);
        }
        
        adicionarUltimaPernaMapa(dados.InformacoesUltimaPerna);
    }

    this.BloquearCampos = function () {
        BloquearCampos(_entrega);
        _entrega.Salvar.visible(false);
    }

    /**
     * Métodos Privados
     */
    var SalvarAvaliacaoPedido = function () {
        var dados = {
            Codigo: _entrega.Codigo.val(),
            Avaliacao: _entrega.Avaliacao.get$().find('input:checked').val() || null,
            Observacao: _entrega.Observacao.val(),
        };

        executarReST("PedidoCliente/SalvarFeedback", dados, function (arg) {
            if (!arg.Success)
                return exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);

            if (!arg.Data)
                return exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);

            self.BloquearCampos();
        });
    }

    var carregarGridNotasEntrega = function () {
        _gridNotasFiscaisEntrega = new GridView(_entrega.Grid.id, "PedidoCliente/PesquisaNotas", { Codigo: _entrega.CodigoPedido });

        _gridNotasFiscaisEntrega.SetHabilitarLinhaClicavel(true);
        _gridNotasFiscaisEntrega.OnClickLinha(function (data) {
            _modalCanhotosPedidos.CarregarCanhotoNota(data);
        });

        _gridNotasFiscaisEntrega.SetDefinicaoColunas({
            DescricaoStatus: function (data, row) {
                return '<span class="label label-warning">' + data + '</span >';
            },
            DescricaoStatusCanhoto: function (data, row) {
                return '<span class="label label-warning">' + data + '</span >';
            },
        });

        _gridNotasFiscaisEntrega.CarregarGrid();
    }

    var adicionarUltimaPernaMapa = function (informacoesUltimaPerna) {
        var iconeInicio = "http://maps.google.com/mapfiles/kml/pal2/icon13.png";
        var iconeFim = MapaObterIconSVGMarcador(MapaObterSVGPin('#d45b5b'));
        
        _mapaEntrega._adicionarMarcadorPorCoordenadas(informacoesUltimaPerna.Remetente.Coordenadas.Latitude, informacoesUltimaPerna.Remetente.Coordenadas.Longitude, informacoesUltimaPerna.Remetente.Endereco, "Origem", iconeInicio);
        _mapaEntrega._adicionarMarcadorPorCoordenadas(informacoesUltimaPerna.Destinatario.Coordenadas.Latitude, informacoesUltimaPerna.Destinatario.Coordenadas.Longitude, informacoesUltimaPerna.Destinatario.Endereco, "Alvo Final", iconeFim);
    }
}