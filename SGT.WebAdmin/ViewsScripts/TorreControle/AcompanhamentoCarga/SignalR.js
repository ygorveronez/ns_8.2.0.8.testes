
function LoadSignalRAcompanhamentoCarga() {
    SignalRAcompanhamentoCargaInformarCardAtualizado = AtualizarCardSignalR;
    SignalRAcompanhamentoCargaInformarListaCardAtualizado = AtualizarListaCardSignalR;
    SignalRPedidosMensagemChatEnviadaEvent = processarMensagemChatEnviadaEvent;
    SignalRPedidosMensagemRecebidaEvent = processarMensagemRecebidaEvent;
    SignalRAcompanhamentoCargaAtualizarCardMensagens = AtualizarMensagensCardSignalR;
}

function AtualizarMensagensCardSignalR() {
    ObterMensagensNaoLidasCards();
}


function AtualizarListaCardSignalR(dados) {
    // pegar o card q esta em _cardAcompanhamentoCarga.Cargas
    if (_cardAcompanhamentoCarga == null || _cardAcompanhamentoCarga.Cargas == null)
        return;

    console.log("SignalR ListaCards ativo.");

    if (dados.Cards.length > 0) {
        for (var i = 0; i < _cardAcompanhamentoCarga.Cargas().length; i++) {

            for (var j = 0; j < dados.Cards.length; j++) {

                //preencher ojeto _detalhesEntregas caso esteja aberto
                if (_detalhesEntregas != undefined && _detalhesEntregas.CodigoCarga.val() == dados.Cards[j].CodigoCarga) {
                    PreencherObjetoKnout(_detalhesEntregas, { Data: dados.Cards[j] });
                    _detalhesEntregas.Data = dados.Cards[j];
                    _detalhesEntregas.Entregas.val.removeAll();

                    for (var y = 0; y < dados.Cards[j].Entregas.length; y++) {
                        var entrega = dados.Cards[j].Entregas[y];
                        var EntregaKnockout = new AcompanhamentoEntrega(entrega);
                        _detalhesEntregas.Entregas.val.push(EntregaKnockout);
                    }
                }

                //preencher ojeto _detalhesCarga caso esteja aberto
                if (_detalhesCarga != undefined && _detalhesCarga.CodigoCarga.val() == dados.Cards[j].CodigoCarga) {
                    PreencherObjetoKnout(_detalhesCarga, { Data: dados.Cards[j] });
                    _detalhesCarga.Data = dados.Cards[j];
                    _detalhesCarga.Entregas.val.removeAll();

                    for (var y = 0; y < dados.Cards[j].Entregas.length; y++) {
                        var entrega = dados.Cards[j].Entregas[y];
                        var EntregaKnockout = new AcompanhamentoEntrega(entrega);
                        _detalhesCarga.Entregas.val.push(EntregaKnockout);
                    }
                }

                //preencher ojeto _cardAcompanhamentoCarga caso esteja aberto
                if (_cardAcompanhamentoCarga.Cargas()[i].CodigoCarga.val() == dados.Cards[j].CodigoCarga) {
                    PreencherObjetoKnout(_cardAcompanhamentoCarga.Cargas()[i], { Data: dados.Cards[j] });
                    _cardAcompanhamentoCarga.Cargas()[i].Data = dados.Cards[j];
                    _cardAcompanhamentoCarga.Cargas()[i].Entregas.val.removeAll();

                    for (var y = 0; y < dados.Cards[j].Entregas.length; y++) {
                        var entrega = dados.Cards[j].Entregas[y];

                        var EntregaKnockoutCard = new AcompanhamentoEntrega(entrega);
                        _cardAcompanhamentoCarga.Cargas()[i].Entregas.val.push(EntregaKnockoutCard);
                    }

                    setTimeout(function () {
                        $("[rel=popover-hover]").popover({ trigger: "hover", container: "body", delay: { "show": 1000, "hide": 0 } });
                    }, 1000);

                    break;
                }
            }
        }

        loadCargasNoMapa(true);
    }

}


function AtualizarCardSignalR(dados) {
    console.log("SignalR Mensages ativo.");
    // pegar o card q esta em _cardAcompanhamentoCarga.Cargas
    if (_cardAcompanhamentoCarga == null || _cardAcompanhamentoCarga.Cargas == null)
        return;

    if (dados.Inserido) {
        //insere a carga nos cards..
        inserirCardTela(dados)
        return;
    }

    for (var i = 0; i < _cardAcompanhamentoCarga.Cargas().length; i++) {

        //preencher ojeto _detalhesEntregas caso esteja aberto
        if (_detalhesEntregas != undefined && _detalhesEntregas.CodigoCarga.val() == dados.Card.CodigoCarga) {
            PreencherObjetoKnout(_detalhesEntregas, { Data: dados.Card });
            _detalhesEntregas.Data = dados.Card;
            _detalhesEntregas.Entregas.val.removeAll();

            for (var j = 0; j < dados.Card.Entregas.length; j++) {
                var entrega = dados.Card.Entregas[j];
                var EntregaKnockout = new AcompanhamentoEntrega(entrega);
                _detalhesEntregas.Entregas.val.push(EntregaKnockout);
            }
        }

        //preencher ojeto _detalhesCarga caso esteja aberto
        if (_detalhesCarga != undefined && _detalhesCarga.CodigoCarga.val() == dados.Card.CodigoCarga) {
            PreencherObjetoKnout(_detalhesCarga, { Data: dados.Card });
            _detalhesCarga.Data = dados.Card;
            _detalhesCarga.Entregas.val.removeAll();

            for (var j = 0; j < dados.Card.Entregas.length; j++) {
                var entrega = dados.Card.Entregas[j];
                var EntregaKnockout = new AcompanhamentoEntrega(entrega);
                _detalhesCarga.Entregas.val.push(EntregaKnockout);
            }
        }

        //preencher ojeto _cardAcompanhamentoCarga caso esteja aberto
        if (_cardAcompanhamentoCarga.Cargas()[i].CodigoCarga.val() == dados.Card.CodigoCarga) {
            PreencherObjetoKnout(_cardAcompanhamentoCarga.Cargas()[i], { Data: dados.Card });
            _cardAcompanhamentoCarga.Cargas()[i].Data = dados.Card;
            _cardAcompanhamentoCarga.Cargas()[i].Entregas.val.removeAll();

            for (var j = 0; j < dados.Card.Entregas.length; j++) {
                var entrega = dados.Card.Entregas[j];

                var EntregaKnockoutCard = new AcompanhamentoEntrega(entrega);
                _cardAcompanhamentoCarga.Cargas()[i].Entregas.val.push(EntregaKnockoutCard);
            }


            setTimeout(function () {
                $("[rel=popover-hover]").popover({ trigger: "hover", container: "body", delay: { "show": 1000, "hide": 0 } });
            }, 1000);

            break;
        }
    }

    loadCargasNoMapa(true);
}


function inserirCardTela(dados) {
    var data = RetornarObjetoPesquisa(_pesquisaAcompanhamentoCarga);
    data.FiltroPesquisa = RetornarJsonFiltroPesquisa(_pesquisaAcompanhamentoCarga);
    data.CodigoNovaCarga = dados.Card.CodigoCarga;

    executarReST("AcompanhamentoCarga/VerificarCargaFiltrosNovoCard", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                if (arg.QuantidadeRegistros > 0) {
                    var data = dados.Card;
                    var card = new CardCarga(data);
                    card.NovoCard.val(true);

                    _cardAcompanhamentoCarga.Cargas.splice(_posicaoPin, 0, card);//adiciona na primeira posicao apos os pins

                    if (_cardAcompanhamentoCarga.Cargas.length >= 50)
                        _cardAcompanhamentoCarga.Cargas.pop(); // remove o ultimo (somente se tem todos na tela)

                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.ControleEntrega.NovaCarga, Localization.Resources.Cargas.ControleEntrega.NovaCargaAdicionadaSuaPesquisaCard.format(card.CargaEmbarcador.val()));
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null, false);

}