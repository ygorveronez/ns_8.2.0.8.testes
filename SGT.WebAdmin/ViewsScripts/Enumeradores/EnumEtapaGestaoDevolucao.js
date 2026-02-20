var EnumEtapaGestaoDevolucaoHelper = function () {
    this.GestaoDeDevolucao = 0;
    this.DefinicaoTipoDevolucao = 1;
    this.AprovacaoTipoDevolucao = 2;
    this.OrdemeRemessa = 3;
    this.GeracaoOcorrenciaDebito = 4;
    this.DefinicaoLocalColeta = 5;
    this.GeracaoCargaDevolucao = 6;
    this.AgendamentoParaDescarga = 7;
    this.GestaoCustoContabil = 8;
    this.Agendamento = 9;
    this.AprovacaoDataDescarga = 10;
    this.Monitoramento = 11;
    this.GeracaoLaudo = 12;
    this.AprovacaoLaudo = 13;
    this.IntegracaoLaudo = 14;
    this.CenarioPosEntrega = 15;
    this.DocumentacaoEntradaFiscal = 16;
};

EnumEtapaGestaoDevolucaoHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.GestaoDeDevolucao: return "Gestão de Devolução";
            case this.DefinicaoTipoDevolucao: return "Definir Tipo de Devolução";
            case this.AprovacaoTipoDevolucao: return "Aprovação";
            case this.OrdemeRemessa: return "Ordem e Remessa";
            case this.GeracaoOcorrenciaDebito: return "Geração Ocorrência Débito";
            case this.DefinicaoLocalColeta: return "Definição Local de Coleta";
            case this.GeracaoCargaDevolucao: return "Geração Carga de Devolução";
            case this.AgendamentoParaDescarga: return "Agendamento para Descarga";
            case this.GestaoCustoContabil: return "Gestão de Custo e Contábil";
            case this.Agendamento: return "Agendamento";
            case this.AprovacaoDataDescarga: return "Aprovação Data Descarga";
            case this.Monitoramento: return "Monitoramento";
            case this.GeracaoLaudo: return "Geração de Laudo";
            case this.AprovacaoLaudo: return "Aprovação Laudo";
            case this.IntegracaoLaudo: return "Integração Laudo";
            case this.CenarioPosEntrega: return "Pós Entrega";
            case this.DocumentacaoEntradaFiscal: return "Documentação de entrada fiscal";
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: this.obterDescricao(this.GestaoDeDevolucao), value: this.GestaoDeDevolucao },
            { text: this.obterDescricao(this.DefinicaoTipoDevolucao), value: this.DefinicaoTipoDevolucao },
            { text: this.obterDescricao(this.AprovacaoTipoDevolucao), value: this.AprovacaoTipoDevolucao },
            { text: this.obterDescricao(this.OrdemeRemessa), value: this.OrdemeRemessa },
            { text: this.obterDescricao(this.GeracaoOcorrenciaDebito), value: this.GeracaoOcorrenciaDebito },
            { text: this.obterDescricao(this.DefinicaoLocalColeta), value: this.DefinicaoLocalColeta },
            { text: this.obterDescricao(this.GeracaoCargaDevolucao), value: this.GeracaoCargaDevolucao },          
            { text: this.obterDescricao(this.AgendamentoParaDescarga), value: this.AgendamentoParaDescarga },
            { text: this.obterDescricao(this.GestaoCustoContabil), value: this.GestaoCustoContabil },
            { text: this.obterDescricao(this.Agendamento), value: this.Agendamento },
            { text: this.obterDescricao(this.AprovacaoDataDescarga), value: this.AprovacaoDataDescarga },
            { text: this.obterDescricao(this.Monitoramento), value: this.Monitoramento },
            { text: this.obterDescricao(this.GeracaoLaudo), value: this.GeracaoLaudo },
            { text: this.obterDescricao(this.AprovacaoLaudo), value: this.AprovacaoLaudo },
            { text: this.obterDescricao(this.IntegracaoLaudo), value: this.IntegracaoLaudo },
            { text: this.obterDescricao(this.CenarioPosEntrega), value: this.CenarioPosEntrega },
            { text: this.obterDescricao(this.DocumentacaoEntradaFiscal), value: this.DocumentacaoEntradaFiscal },
        ];
    }
};

var EnumEtapaGestaoDevolucao = Object.freeze(new EnumEtapaGestaoDevolucaoHelper());