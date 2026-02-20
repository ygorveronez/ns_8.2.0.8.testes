var EnumTipoConfiguracaoAlertaHelper = function () {
    this.Todos = "";
    this.ApoliceSeguro = 1;
    this.CertificadoDigital = 2;
    this.RotaNaoCadastrada = 3;
    this.PedidoSemTabelaFrete = 4;
    this.Antt = 5;
    this.Cnh = 6;
    this.MDFEPendenteDeEncerramento = 7;
    this.RegraICMS = 8;
    this.PendenciaNfsManual = 9;
};

EnumTipoConfiguracaoAlertaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "ANTT", value: this.Antt },
            { text: "Apólice de Seguro", value: this.ApoliceSeguro },
            { text: "Certificado Digital", value: this.CertificadoDigital },
            { text: "CNH", value: this.Cnh },
            { text: "Rota não Cadastrada", value: this.RotaNaoCadastrada },
            { text: "Pedido sem tabela de frete", value: this.PedidoSemTabelaFrete },
            { text: "MDF-e pendente de Encerramento", value: this.MDFEPendenteDeEncerramento },
            { text: "Regra ICMS", value: this.RegraICMS },
            { text: "Pendência NFS Manual", value: this.PendenciaNfsManual }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoConfiguracaoAlerta = Object.freeze(new EnumTipoConfiguracaoAlertaHelper());