var EnumDescricaoUsoEmpresaPagamentoEletronicoHelper = function () {    
    this.Nenhum = 0;
    this.PagamentoSalario = 1;
    this.PagamentoFerias = 2;
    this.AdiantamentoSalario = 3;
    this.DecimoTerceiroSalario = 4;
    this.AdiantamentoDecimoTerceiroSalario = 5;
    this.BonusMetaVenda = 6;
    this.BonusPerformance = 7;
    this.ComissaoSobreVendas = 8;
    this.ParticipacaoLucrosDaEmpresa = 9;
    this.BonusPorProducao = 10;
    this.DecimoQuartoSalario = 11;
    this.HoraExtra = 12;
    this.GratificacaoPremio = 13;
    this.RescisaoContratual = 14;
    this.AdiantamentoDiarias = 15;
};

EnumDescricaoUsoEmpresaPagamentoEletronicoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "0 - Não informado", value: this.Nenhum },
            { text: "1 - Pagamento de salários", value: this.PagamentoSalario  },
            { text: "2 - Pagamento de férias", value: this.PagamentoFerias },
            { text: "3 - Adiantamento de salário", value: this.AdiantamentoSalario  },
            { text: "4 - 13º Salário", value: this.DecimoTerceiroSalario  },
            { text: "5 - Adiantamento de 13º Salário", value: this.AdiantamentoDecimoTerceiroSalario  },
            { text: "6 - Bônus por metas de vendas", value: this.BonusMetaVenda  },
            { text: "7 - Bônus por performance", value: this.BonusPerformance  },
            { text: "8 - Comissão sobre vendas", value: this.ComissaoSobreVendas  },
            { text: "9 - Participação lucros da empresa", value: this.ParticipacaoLucrosDaEmpresa  },
            { text: "10 - Bônus por produção / serviço", value: this.BonusPorProducao  },
            { text: "11 - 14º Salário", value: this.DecimoQuartoSalario  },
            { text: "12 - Hora extra", value: this.HoraExtra  },
            { text: "13 - Gratificação/ Prêmio", value: this.GratificacaoPremio  },
            { text: "14 - Rescisão contratual", value: this.RescisaoContratual },
            { text: "15 - Adiantamento de diárias", value: this.AdiantamentoDiarias }
        ];
    }
};

var EnumDescricaoUsoEmpresaPagamentoEletronico = Object.freeze(new EnumDescricaoUsoEmpresaPagamentoEletronicoHelper());