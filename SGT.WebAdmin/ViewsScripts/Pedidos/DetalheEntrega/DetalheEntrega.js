var DetalheEntrega = function () {
    var _etapa;
    var _detalhes;
    var _transferencia;
    var _rota;
    var _entrega;

    /**
     * Métodos Público
     */
    this.Load = function () {
        _etapa = new EtapaDetalheEntrega();
        _detalhes = new PedidoDetalheEntrega();
        _transferencia = new TransferenciaDetalheEntrega();
        _rota = new RotaDetalheEntrega();
        _entrega = new EntregaDetalheEntrega();

        _etapa.Load('knockoutEtapaPedido', {
            _transferencia: _transferencia,
            _rota: _rota,
        });
        _detalhes.Load('knockoutPedido');
        _transferencia.Load('knockoutTransferencia');
        _rota.Load('knockoutRota');
        _entrega.Load('knockoutEntrega');
    }

    this.CarregarPorNotificacaoGlogal = function () {
        var codigo = _notificacaoPortal.GetCodigoGlobal();
        _notificacaoPortal.ClearCodigoGlobal();
        //codigo = 135025;
        if (codigo)
            carregarDadosPorCodigo(codigo);
    }

    /**
     * Métodos Privados
     */
    var carregarDadosPorCodigo = function (codigo) {
        executarReST("PedidoCliente/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
            if (!arg.Success)
                return exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);

            if (!arg.Data)
                return exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);

            var dados = arg.Data;

            _etapa.CarregarEtapaPorSituacao(dados.Situacao);

            _detalhes.PreencherDados(dados.Detalhes);
            _transferencia.PreencherDados(dados.Transferencia);
            _rota.PreencherDados(dados.Rota);
            _entrega.PreencherDados(dados.Entrega);
        });
    }
}

//135025