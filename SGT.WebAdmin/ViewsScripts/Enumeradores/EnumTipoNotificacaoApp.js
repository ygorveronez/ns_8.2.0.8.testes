const EnumTipoNotificacaoAppHelper = function () {
    this.Todas = "";
    this.MotoristaDentroDoRaioDoFornecedor = 0;
    this.MotoristaForaDoRaioDoFornecedor = 1;
    this.CarregamentoEmAvaliacaoPelaLogistica = 2;
    this.CarregamentoValidadoAguardandoCompraValePedagio = 3;
    this.ValePedagioCompradoComSucesso = 4;
    this.ValePedagioNaoComprado = 5;
    this.MotoristaPodeSeguirViagem = 6;
    this.TratativaDoAtendimento = 7;
    this.RejeicaoDadosNFeColeta = 8;
    this.Custom = 99;
};

EnumTipoNotificacaoAppHelper.prototype = {
    obterOpcoes: function () {
        const opcoes = [];

        opcoes.push({ text: this.obterDescricao(this.Todas), value: this.Todas });
        opcoes.push({ text: this.obterDescricao(this.MotoristaDentroDoRaioDoFornecedor), value: this.MotoristaDentroDoRaioDoFornecedor });
        opcoes.push({ text: this.obterDescricao(this.MotoristaForaDoRaioDoFornecedor), value: this.MotoristaForaDoRaioDoFornecedor });
        opcoes.push({ text: this.obterDescricao(this.CarregamentoEmAvaliacaoPelaLogistica), value: this.CarregamentoEmAvaliacaoPelaLogistica });
        opcoes.push({ text: this.obterDescricao(this.CarregamentoValidadoAguardandoCompraValePedagio), value: this.CarregamentoValidadoAguardandoCompraValePedagio });
        opcoes.push({ text: this.obterDescricao(this.ValePedagioCompradoComSucesso), value: this.ValePedagioCompradoComSucesso });
        opcoes.push({ text: this.obterDescricao(this.ValePedagioNaoComprado), value: this.ValePedagioNaoComprado });
        opcoes.push({ text: this.obterDescricao(this.MotoristaPodeSeguirViagem), value: this.MotoristaPodeSeguirViagem });
        opcoes.push({ text: this.obterDescricao(this.TratativaDoAtendimento), value: this.TratativaDoAtendimento });
        opcoes.push({ text: this.obterDescricao(this.RejeicaoDadosNFeColeta), value: this.RejeicaoDadosNFeColeta });
        opcoes.push({ text: this.obterDescricao(this.Custom), value: this.Custom });

        return opcoes;
    },
    opcoesCadastroNotificacoes: function () {
        const opcoes = [];

        opcoes.push({ text: "", value: "" });
        opcoes.push({ text: this.obterDescricao(this.MotoristaDentroDoRaioDoFornecedor), value: this.MotoristaDentroDoRaioDoFornecedor });
        opcoes.push({ text: this.obterDescricao(this.MotoristaForaDoRaioDoFornecedor), value: this.MotoristaForaDoRaioDoFornecedor });
        opcoes.push({ text: this.obterDescricao(this.CarregamentoEmAvaliacaoPelaLogistica), value: this.CarregamentoEmAvaliacaoPelaLogistica });
        opcoes.push({ text: this.obterDescricao(this.CarregamentoValidadoAguardandoCompraValePedagio), value: this.CarregamentoValidadoAguardandoCompraValePedagio });
        opcoes.push({ text: this.obterDescricao(this.ValePedagioCompradoComSucesso), value: this.ValePedagioCompradoComSucesso });
        opcoes.push({ text: this.obterDescricao(this.ValePedagioNaoComprado), value: this.ValePedagioNaoComprado });
        opcoes.push({ text: this.obterDescricao(this.MotoristaPodeSeguirViagem), value: this.MotoristaPodeSeguirViagem });
        opcoes.push({ text: this.obterDescricao(this.TratativaDoAtendimento), value: this.TratativaDoAtendimento });
        opcoes.push({ text: this.obterDescricao(this.RejeicaoDadosNFeColeta), value: this.RejeicaoDadosNFeColeta });

        return opcoes;
    },
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.MotoristaDentroDoRaioDoFornecedor: return "Foto da coleta registrada dentro do raio da origem";
            case this.MotoristaForaDoRaioDoFornecedor: return "Foto da coleta registrada fora do raio da origem";
            case this.CarregamentoEmAvaliacaoPelaLogistica: return "Foto da coleta recebida";
            case this.CarregamentoValidadoAguardandoCompraValePedagio: return "Carregamento avaliado aguardando compra de Vale pedágio";
            case this.ValePedagioCompradoComSucesso: return "Vale pedágio comprado com sucesso";
            case this.ValePedagioNaoComprado: return "Vale pedágio não comprado - Falha na compra";
            case this.MotoristaPodeSeguirViagem: return "Carga entrou na situação \"Em Transporte\"";
            case this.TratativaDoAtendimento: return "Tratativa do atendimento";
            case this.RejeicaoDadosNFeColeta: return "Rejeição dos dados de NF-e da coleta";
            case this.Custom: return "Notificação Customizada";
            case this.Todas: return "Todas";
            default: return "";
        }
    }
};

const EnumTipoNotificacaoApp = Object.freeze(new EnumTipoNotificacaoAppHelper());
