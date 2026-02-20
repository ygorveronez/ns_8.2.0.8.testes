/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../Global/CentralProcessamento/CentralProcessamento.js" />

var _detalhesDespesa;
var _gridDetalhesDespesa;

function buscarCargasDespesa() {
    var detalhes = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), metodo: detalhesDespesaClick, icone: "", visibilidade: true };
    var imprimir = { descricao: Localization.Resources.Cargas.Carga.Imprimir, id: guid(), metodo: imprimirDespesaClick, icone: "", visibilidade: true };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [detalhes, imprimir] };
    
    _gridCargaIntegracaoDespesa = new GridView(_cargaCTe.PesquisarIntegracaoDespesa.idGrid, "CargaIntegracaoDespesa/ConsultarCargaDespesa", _cargaCTe, menuOpcoes);
    _gridCargaIntegracaoDespesa.CarregarGrid(callbackGridDespesa);

    if (_cargaAtual.ProblemaIntegracaoPagamentoMotorista.val() || _cargaAtual.LiberadoComProblemaPagamentoMotorista.val()) {
        _cargaCTe.LiberarComProblemaPagamentoMotorista.visible(true);

        if (_cargaAtual.LiberadoComProblemaPagamentoMotorista.val())
            _cargaCTe.LiberarComProblemaPagamentoMotorista.enable(false);
        else
            _cargaCTe.LiberarComProblemaPagamentoMotorista.enable(true);
    } else {
        _cargaCTe.LiberarComProblemaPagamentoMotorista.visible(false);
    }
}

var DetalhesDespesa = function () {
    this.Pagamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DetalhesDespesa = PropertyEntity({ eventClick: function (e) { _gridDetalhesDespesa.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
};

function detalhesDespesa() {
    _detalhesDespesa = new DetalhesDespesa();

    KoBindings(_detalhesDespesa, "knockoutDetalhesDespesa");

    _gridDetalhesDespesa = new GridView(_detalhesDespesa.DetalhesDespesa.idGrid, "CargaIntegracaoDespesa/BuscarDetalhesDespesa", _detalhesDespesa);
}

function callbackGridDespesa() {
    if (_gridCargaIntegracaoDespesa.NumeroRegistros() > 0)
        $("#tabIntegracaoDespesas_" + _cargaAtual.DadosCTes.id + "_li").show();
}

function detalhesDespesaClick(e) {
    detalhesDespesa();
    _detalhesDespesa.Pagamento.val(parseInt(e.Codigo));
    _gridDetalhesDespesa.CarregarGrid();
    Global.abrirModal("divModalDetalhesDespesa");
}

function imprimirDespesaClick(e) {
    var data = { Codigo: parseInt(e.Codigo), Carga: true };
    executarReST("MovimentoFinanceiro/GerarRecibo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.AguardeQueSeuRelatorioEstaSendoGerado);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        }
    });
}

function liberarComProblemaPagamentoMotoristaClick(e) {
    var data = {
        Carga: _cargaAtual.Codigo.val()
    }
    exibirConfirmacao(Localization.Resources.Cargas.Carga.AvancarEtapa, Localization.Resources.Cargas.Carga.VoceTemCertezaQueDesejaAvancarEtapaMesmoQueTenhaDespesasComFalhaNaIntegracao, function () {
        executarReST("CargaIntegracaoDespesa/LiberarComProblemaPagamentoMotorista", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _cargaCTe.LiberarComProblemaPagamentoMotorista.enable(false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}