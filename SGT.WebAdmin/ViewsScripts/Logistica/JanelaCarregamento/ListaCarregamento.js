/// <reference path="../../Enumeradores/EnumTipoJanelaCarregamento.js" />

var ListaCarregamento = function () {
    this._lista;
}

ListaCarregamento.prototype = {
    AdicionarOuAtualizarCarga: function (dadosCarga) {
        this._lista.AdicionarOuAtualizarCarga(dadosCarga);
    },
    RemoverCarga: function (dadosCarga) {
        this._lista.RemoverCarga(dadosCarga);
    },
    IsCalendario: function () {
        return (this._lista instanceof CalendarioCarregamento);
    },
    CargaMovidaParaExcedente: function () {
        if (this.IsCalendario())
            return this._lista.CargaMovidaParaExcedente.apply(this._lista, arguments);
    },
    Load: function () {
        this._getInstance().Load();
    },
    ObterData: function () {
        return this._getInstance().ObterData();
    },
    Render: function () {
        if (this.IsCalendario())
            this._lista.Render();
    },
    SetAltura: function (altura) {
        if (this.IsCalendario())
            this._lista.SetAltura(altura);
    },
    _getInstance: function () {
        if (_centroCarregamentoAtual.TipoJanelaCarregamento == EnumTipoJanelaCarregamento.Calendario) {
            if (this._lista instanceof CalendarioCarregamento)
                return this._lista;

            if (this._lista)
                this._lista.Destroy();

            this._lista = new CalendarioCarregamento();
        }
        else {
            if (this._lista instanceof TabelaCarregamento)
                return this._lista;

            if (this._lista)
                this._lista.Destroy();

            this._lista = new TabelaCarregamento();
        }

        return this._lista;
    }
}