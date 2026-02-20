/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaJanelaCarregamento.js" />
/// <reference path="CapacidadeCarregamentoDados.js" />
/// <reference path="CargaPendente.js" />
/// <reference path="DisponibilidadeCarregamento.js" />
/// <reference path="ListaCarregamento.js" />
/// <reference path="JanelaCarregamento.js" />

SignalRJanelaCarregamentoAlteradaEvent = function (dadosJanelaCarregamento) {
    var indiceCargasPendentes = -1;

    if (!_cargaPendente)
        return;

    for (var i = 0; i < _cargaPendente.CargasPendentes().length; i++) {
        if (_cargaPendente.CargasPendentes()[i].CodigoJanelaCarregamento.val() == dadosJanelaCarregamento.Codigo) {
            indiceCargasPendentes = i;
            break;
        }
    }

    var indiceCargasExcedentes = -1;

    for (var i = 0; i < _cargaPendente.CargasExcedentes().length; i++) {
        if (_cargaPendente.CargasExcedentes()[i].CodigoJanelaCarregamento.val() == dadosJanelaCarregamento.Codigo) {
            indiceCargasExcedentes = i;
            break;
        }
    }

    var listaApta = (_centroCarregamentoAtual.Codigo == dadosJanelaCarregamento.CodigoCentroCarregamento) && (dadosJanelaCarregamento.DataCarregamento == _listaCarregamento.ObterData());

    if (indiceCargasPendentes > -1 || indiceCargasExcedentes > -1 || listaApta) {
        executarReST("JanelaCarregamento/ObterInformacoes", { Codigo: dadosJanelaCarregamento.Codigo }, function (retorno) {
            if (retorno.Success && retorno.Data) {
                if (indiceCargasPendentes > -1) {
                    if (retorno.Data.Situacao != EnumSituacaoCargaJanelaCarregamento.ProntaParaCarregamento)
                        AdicionarCargaPendente(retorno.Data, indiceCargasPendentes);
                    else {
                        if (_cargaPendente.CargasPendentes().length > _quantidadeCargasPorVez)
                            _cargaPendente.CargasPendentes.remove(function (item) { return _cargaPendente.CargasPendentes()[indiceCargasPendentes].CodigoJanelaCarregamento.val() == item.CodigoJanelaCarregamento.val(); });
                        else
                            RecarregarCargasPendentes();
                    }
                }

                if (indiceCargasExcedentes > -1) {
                    if (retorno.Data.Excedente)
                        AdicionarCargaExcedente(retorno.Data, indiceCargasExcedentes);
                    else {
                        if (_cargaPendente.CargasExcedentes().length > _quantidadeCargasPorVez)
                            _cargaPendente.CargasExcedentes.remove(function (item) { return _cargaPendente.CargasExcedentes()[indiceCargasExcedentes].CodigoJanelaCarregamento.val() == item.CodigoJanelaCarregamento.val(); });
                        else
                            RecarregarCargasExcedentes();
                    }
                }
                else if (retorno.Data.Excedente && _listaCarregamento.CargaMovidaParaExcedente() != retorno.Data.Carga.Codigo) {
                    AdicionarCargaExcedente(retorno.Data, null);
                    _listaCarregamento.RemoverCarga(retorno.Data);
                }

                if (listaApta && !retorno.Data.Excedente)
                    _listaCarregamento.AdicionarOuAtualizarCarga(retorno.Data);
            }
        }, null, false);
    }
}
