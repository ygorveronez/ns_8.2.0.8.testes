/// <reference path="JanelaCarregamento.js" />
/// <reference path="..\..\Enumeradores\EnumSituacaoCargaJanelaCarregamento.js" />
/// <reference path="..\..\Enumeradores\EnumSituacoesCarga.js" />
/// <reference path="..\..\Enumeradores\EnumTipoCargaJanelaCarregamento.js" />
/// <reference path="..\..\Enumeradores\EnumTipoCondicaoPagamento.js" />

function ControleCorCarga() {
}

ControleCorCarga.prototype = {
    ObterClasse: function (carga) {
        if ((carga.Editavel == false) || !this._isCargaCompativelFiltrosPesquisa(carga))
            return "well-gray";

        if (carga.Tipo == EnumTipoCargaJanelaCarregamento.Descarregamento)
            return "well-steel-blue";

        if (this._isTipoCondicaoPagamentoFOB(carga))
            return "well-aqua";

        return this._obterClasseSituacaoCarga(carga);
    },
    _isCargaCompativelFiltrosPesquisa: function (carga) {
        return (
            this._isCargaCompativelFiltrosPesquisaFluxoCaixa(carga) &&
            this._isCargaCompativelFiltrosPesquisaDestinatario(carga) &&
            this._isCargaCompativelFiltrosPesquisaDestino(carga) &&
            this._isCargaCompativelFiltrosPesquisaMotorista(carga) &&
            this._isCargaCompativelFiltrosPesquisaTransportador(carga) &&
            this._isCargaCompativelFiltrosPesquisaVeiculo(carga) &&
            this._isCargaCompativelFiltrosPesquisaRota(carga)
        );
    },
    _isCargaCompativelFiltrosPesquisaDestinatario: function (carga) {
        if (_dadosPesquisaCarregamento.Destinatario != "") {
            for (var d in carga.DestinatariosCarga) {
                if (carga.DestinatariosCarga[d].Codigo == _dadosPesquisaCarregamento.Destinatario)
                    return true;
            }

            return false;
        }

        return true;
    },
    _isCargaCompativelFiltrosPesquisaDestino: function (carga) {
        if (_dadosPesquisaCarregamento.Destino > 0) {
            for (var d in carga.DestinosCarga) {
                if (carga.DestinosCarga[d].Codigo == _dadosPesquisaCarregamento.Destino)
                    return true;
            }

            return false;
        }

        return true;
    },
    _isCargaCompativelFiltrosPesquisaFluxoCaixa: function (carga) {
        if ((_dadosPesquisaCarregamento.CodigoCargaEmbarcador != "") || (_dadosPesquisaCarregamento.CodigoPedidoEmbarcador != ""))
            return carga.Carga.Numero == _fluxoPorCarga.Numero;

        return true;
    },
    _isCargaCompativelFiltrosPesquisaMotorista: function (carga) {
        if (_dadosPesquisaCarregamento.Motorista > 0) {
            for (var m in carga.Motoristas) {
                if (carga.Motoristas[m].Codigo == _dadosPesquisaCarregamento.Motorista)
                    return true;
            }

            return false;
        }

        return true;
    },
    _isCargaCompativelFiltrosPesquisaRota: function (carga) {
        if (_dadosPesquisaCarregamento.Rota > 0) {
            if ((carga.Rota == null) || (carga.Rota.Codigo != _dadosPesquisaCarregamento.Rota))
                return false;
        }

        return true;
    },
    _isCargaCompativelFiltrosPesquisaTransportador: function (carga) {
        if (_dadosPesquisaCarregamento.Transportador > 0) {
            if ((carga.Transportador == null) || (carga.Transportador.Codigo != _dadosPesquisaCarregamento.Transportador))
                return false;
        }

        return true;
    },
    _isCargaCompativelFiltrosPesquisaVeiculo: function (carga) {
        if (_dadosPesquisaCarregamento.Veiculo > 0) {
            if ((carga.Veiculo == null) || (carga.Veiculo.Codigo != _dadosPesquisaCarregamento.Veiculo))
                return false;
        }

        return true;
    },
    _isSituacaoCargaFaturada: function (situacaoCarga) {
        var situacoesCargaNaoFaturada = EnumSituacoesCarga.obterSituacoesCargaNaoFaturada();
        var isSituacaoCargaFaturada = ($.inArray(situacaoCarga, situacoesCargaNaoFaturada) == -1);

        return isSituacaoCargaFaturada;
    },
    _isTipoCondicaoPagamentoFOB: function (carga) {
        return (carga.Carga.TipoCondicaoPagamento == EnumTipoCondicaoPagamento.FOB);
    },
    _obterClasseSituacaoCarga: function (carga) {
        if (this._isSituacaoCargaFaturada(carga.Carga.SituacaoCarga))
            return "well-darkGreen";

        return EnumSituacaoCargaJanelaCarregamento.obterClasseCor(carga.Situacao);
    }
}