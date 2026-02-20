var _gridSubareaClienteAcoesFluxoDePatio;
var _listaSubareaClienteAcoesFluxoDePatio;

function recarregarGridListaSubareaClienteAcoesFluxoDePatio(dados) {
    if (!dados) dados = new Array();
    
    _gridSubareaClienteAcoesFluxoDePatio.CarregarGrid(dados);
}

function editarListaSubareaClienteAcoesFluxoDePatioClick(dados) {
    var acaoFluxoDePatio = null;

    $.each(_subarea.ListaSubareaClienteAcoesFluxoDePatio.list, function (i, ListaSubareaClienteAcoesFluxoDePatio) {
        if (dados.CodigoAcao == ListaSubareaClienteAcoesFluxoDePatio.CodigoAcao) {
            acaoFluxoDePatio = _subarea.ListaSubareaClienteAcoesFluxoDePatio.list[i];
            return false;
        }
    });

    if (acaoFluxoDePatio != null) {
        _subarea.CodigoAcao.val(acaoFluxoDePatio.CodigoAcao);

        _subarea.CodigoAcaoMonitoramentoFluxoDePatio.val(acaoFluxoDePatio.CodigoAcaoMonitoramentoFluxoDePatio);
        _subarea.AcaoMonitoramentoFluxoDePatio.val(acaoFluxoDePatio.AcaoMonitoramentoFluxoDePatio);

        _subarea.CodigoEtapaFluxoDePatio.val(acaoFluxoDePatio.CodigoEtapaFluxoDePatio);
        _subarea.EtapaFluxoDePatio.val(acaoFluxoDePatio.EtapaFluxoDePatio);

        _subarea.CodigoAcaoFluxoDePatio.val(acaoFluxoDePatio.CodigoAcaoFluxoDePatio);
        _subarea.AcaoFluxoDePatio.val(acaoFluxoDePatio.AcaoFluxoDePatio);

        _subarea.AdicionarAcaoFluxoPatio.visible(false);
        _subarea.CancelarAcaoFluxoPatio.visible(true);
        _subarea.ExcluirAcaoFluxoPatio.visible(true);
        _subarea.AtualizarAcaoFluxoPatio.visible(true);
    }
}

function adicionarListaSubareaClienteAcoesFluxoDePatioClick(dados) {
    var acaoFluxoDePatio = ObjetoAcaoFluxoDePatio(dados);

    if (ObjetoAcaoFluxoDePatioValido(acaoFluxoDePatio)) {
        _subarea.ListaSubareaClienteAcoesFluxoDePatio.list.push(acaoFluxoDePatio);

        recarregarGridListaSubareaClienteAcoesFluxoDePatio(_subarea.ListaSubareaClienteAcoesFluxoDePatio.list);
        limparCamposListaSubareaClienteAcoesFluxoDePatio();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.Pessoa.AcaoDuplicadao, Localization.Resources.Pessoas.Pessoa.JaExisteAcaoCadastradaParaAMovimentacaoDaEtapa);
    }
}

function atualizarListaSubareaClienteAcoesFluxoDePatioClick(dados) {
    var acaoFluxoDePatio = ObjetoAcaoFluxoDePatio(dados);

    if (ObjetoAcaoFluxoDePatioValido(acaoFluxoDePatio)) {
        var codigo = _subarea.CodigoAcao.val();

        $.each(_subarea.ListaSubareaClienteAcoesFluxoDePatio.list, function (i, ListaSubareaClienteAcoesFluxoDePatio) {
            if (codigo == ListaSubareaClienteAcoesFluxoDePatio.CodigoAcao) {
                _subarea.ListaSubareaClienteAcoesFluxoDePatio.list[i] = acaoFluxoDePatio;
                return false;
            }
        });

        recarregarGridListaSubareaClienteAcoesFluxoDePatio(_subarea.ListaSubareaClienteAcoesFluxoDePatio.list);
        limparCamposListaSubareaClienteAcoesFluxoDePatio();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.Pessoa.AcaoDuplicadao, Localization.Resources.Pessoas.Pessoa.JaExisteAcaoCadastradaParaAMovimentacaoDaEtapa);
    }
}

function excluirListaSubareaClienteAcoesFluxoDePatioClick(dados) {
    var acaoFluxoDePatio = ObjetoAcaoFluxoDePatio(dados);

    $.each(_subarea.ListaSubareaClienteAcoesFluxoDePatio.list, function (i, ListaSubareaClienteAcoesFluxoDePatio) {
        if (acaoFluxoDePatio.CodigoAcao == ListaSubareaClienteAcoesFluxoDePatio.CodigoAcao) {
            _subarea.ListaSubareaClienteAcoesFluxoDePatio.list.splice(i, 1);
            return false;
        }
    });

    recarregarGridListaSubareaClienteAcoesFluxoDePatio(_subarea.ListaSubareaClienteAcoesFluxoDePatio.list);
    limparCamposListaSubareaClienteAcoesFluxoDePatio();
}

function ObjetoAcaoFluxoDePatio(dados) {
    var codigo = dados.CodigoAcao.val();
    var fluxoDePatio = {
        CodigoAcao: codigo ? codigo : guid(),
        AcaoMonitoramentoFluxoDePatio: EnumMonitoramentoEventoData.obterDescricao(dados.CodigoAcaoMonitoramentoFluxoDePatio.val()),
        CodigoAcaoMonitoramentoFluxoDePatio: dados.CodigoAcaoMonitoramentoFluxoDePatio.val(),
        EtapaFluxoDePatio: EnumEtapaFluxoGestaoPatio.obterDescricao(dados.CodigoEtapaFluxoDePatio.val()),
        CodigoEtapaFluxoDePatio: dados.CodigoEtapaFluxoDePatio.val(),
        AcaoFluxoDePatio: EnumAcaoFluxoGestaoPatio.obterDescricao(dados.CodigoAcaoFluxoDePatio.val()),
        CodigoAcaoFluxoDePatio: dados.CodigoAcaoFluxoDePatio.val()
    };
    return fluxoDePatio;
}

function ObjetoAcaoFluxoDePatioValido(acaoFluxoDePatio) {
    var valido = true;
    for (var i = 0; i < _subarea.ListaSubareaClienteAcoesFluxoDePatio.list.length; i++) {
        if (acaoFluxoDePatio.CodigoAcao != _subarea.ListaSubareaClienteAcoesFluxoDePatio.list[i].CodigoAcao &&
            acaoFluxoDePatio.CodigoEtapaFluxoDePatio == _subarea.ListaSubareaClienteAcoesFluxoDePatio.list[i].CodigoEtapaFluxoDePatio) {

            valido = false;
            break;
        }
    }
    return valido;
}

function limparCamposListaSubareaClienteAcoesFluxoDePatio() {
    _subarea.CodigoAcaoFluxoDePatio.val(_subarea.CodigoAcaoFluxoDePatio.def());
    _subarea.CodigoAcaoMonitoramentoFluxoDePatio.val(_subarea.AcaoMonitoramentoFluxoDePatio.def());
    _subarea.CodigoEtapaFluxoDePatio.val(_subarea.EtapaFluxoDePatio.def());
    _subarea.CodigoAcaoFluxoDePatio.val(_subarea.AcaoFluxoDePatio.def());

    _subarea.AdicionarAcaoFluxoPatio.visible(true);
    _subarea.CancelarAcaoFluxoPatio.visible(false);
    _subarea.ExcluirAcaoFluxoPatio.visible(false);
    _subarea.AtualizarAcaoFluxoPatio.visible(false);
}