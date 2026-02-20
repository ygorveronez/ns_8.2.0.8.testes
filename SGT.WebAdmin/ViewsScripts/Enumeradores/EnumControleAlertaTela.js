var EnumControleAlertaTelaHelper = function () {
    this.Veiculo = 1;
    this.Pessoa = 2;
    this.Motorista = 3;
    this.TabelaFrete = 4;
    this.Pedido = 5;
    this.Manutencao = 6;
    this.RegraICMS = 7;
    this.EstoqueMinimo = 8;
    this.OrdemServicoInterna = 9;
    this.OrdemServicoExterna = 10;
    this.CheckList = 11;
};

EnumControleAlertaTelaHelper.prototype = {
    obterOpcoes: function () {
        var opcoes = [];

        opcoes.push({ text: "Motorista/Funcionário", value: this.Motorista });
        opcoes.push({ text: "Pessoa", value: this.Pessoa });
        opcoes.push({ text: "Veículo", value: this.Veiculo });
        opcoes.push({ text: "Manutenção", value: this.Manutencao });
        opcoes.push({ text: "Estoque Mínimo", value: this.EstoqueMinimo });

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe) {
            opcoes.push({ text: "Pedido", value: this.Pedido });
            opcoes.push({ text: "Tabela de Frete", value: this.TabelaFrete });
            opcoes.push({ text: "Regra de ICMS", value: this.RegraICMS });
            opcoes.push({ text: "Checklist", value: this.CheckList });
        }

        opcoes.push({ text: "Ordem de Serviço Interna", value: this.OrdemServicoInterna });
        opcoes.push({ text: "Ordem de Serviço Externa", value: this.OrdemServicoExterna });

        return opcoes;
    }
};

var EnumControleAlertaTela = Object.freeze(new EnumControleAlertaTelaHelper());