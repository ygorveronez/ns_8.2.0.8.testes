var EnumInformacoesAdicionaisEntregaTrizyHelper = function () {
    this.ObservacoesDoCliente = 1;
    this.TelefoneDoCliente = 2;
    this.NumeroDosPedidos = 3;
    this.Fardos = 4;
    this.RegrasPallet = 5;
    this.NotasFiscais = 6;
    this.NomeFantasia = 7;
    this.RazaoSocial = 8;
};

EnumInformacoesAdicionaisEntregaTrizyHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Pedidos.TipoOperacao.ObservacoesDoCliente, value: this.ObservacoesDoCliente },
            { text: Localization.Resources.Pedidos.TipoOperacao.TelefoneDoCliente, value: this.TelefoneDoCliente },
            { text: Localization.Resources.Pedidos.TipoOperacao.NumeroDosPedidos, value: this.NumeroDosPedidos },
            { text: Localization.Resources.Pedidos.TipoOperacao.Fardos, value: this.Fardos },
            { text: Localization.Resources.Pedidos.TipoOperacao.RegrasPallet, value: this.RegrasPallet },
            { text: Localization.Resources.Pedidos.TipoOperacao.NotasFiscais, value: this.NotasFiscais },
            { text: Localization.Resources.Pedidos.TipoOperacao.NomeFantasia, value: this.NomeFantasia },
            { text: Localization.Resources.Pedidos.TipoOperacao.RazaoSocial, value: this.RazaoSocial },
        ];
    },
    obterOpcoesSemTelefone: function () {
        return [
            { text: Localization.Resources.Pedidos.TipoOperacao.ObservacoesDoCliente, value: this.ObservacoesDoCliente },
            { text: Localization.Resources.Pedidos.TipoOperacao.NumeroDosPedidos, value: this.NumeroDosPedidos },
            { text: Localization.Resources.Pedidos.TipoOperacao.Fardos, value: this.Fardos },
            { text: Localization.Resources.Pedidos.TipoOperacao.RegrasPallet, value: this.RegrasPallet },
            { text: Localization.Resources.Pedidos.TipoOperacao.NotasFiscais, value: this.NotasFiscais },
            { text: Localization.Resources.Pedidos.TipoOperacao.NomeFantasia, value: this.NomeFantasia },
            { text: Localization.Resources.Pedidos.TipoOperacao.RazaoSocial, value: this.RazaoSocial },
        ];
    }
};

var EnumInformacoesAdicionaisEntregaTrizy = Object.freeze(new EnumInformacoesAdicionaisEntregaTrizyHelper());