var EnumTipoAlertaCargaHelper = function () {
    this.SemAlerta = 0;
    this.CargaSemTransportador = 1;
    this.CagraSemVeiculo = 2;
    this.VeiculoComInsumos = 3;
    this.VeiculoNaoMonitorado = 4;
    this.ValidacaoGerenciadoraRisco = 5;
    this.NaoAtendimentoAgenda = 6;
    this.AntecedenciaGrade = 7;
    this.AtrasoColetaDescarga = 8;
    this.AtrasoInicioViagem = 9;
    this.InicioViagem = 10;
    this.FimViagem = 11;
    this.ConfirmacaoColetaEntrega = 12;
    this.AtendimentoIniciado = 13;
}

EnumTipoAlertaCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.AtrasoInicioViagem, value: this.AtrasoInicioViagem },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.AntecedenciaGradeCarregamento, value: this.AntecedenciaGrade },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.CargaSemTransportador, value: this.CargaSemTransportador },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.CargaSemVeiculo, value: this.CagraSemVeiculo },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.NaoAtendimentoAgenda, value: this.NaoAtendimentoAgenda },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.ValidacaoGerenciadoraRisco, value: this.ValidacaoGerenciadoraRisco },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.VeiculoComInsumos, value: this.VeiculoComInsumos },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.VeiculoNaoMonitorado, value: this.VeiculoNaoMonitorado },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.InicioViagem, value: this.InicioViagem },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.FimViagem, value: this.FimViagem },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.ConfirmacaoColetaEntrega, value: this.ConfirmacaoColetaEntrega },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.AtendimentoChamadoIniciado, value: this.AtendimentoIniciado },
        ];
    },
    obterOpcoesTorreMonitoramento: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.AntecedenciaGradeCarregamento, value: this.AntecedenciaGrade },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.CargaSemTransportador, value: this.CargaSemTransportador },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.CargaSemVeiculo, value: this.CagraSemVeiculo },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.NaoAtendimentoAgenda, value: this.NaoAtendimentoAgenda },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.ValidacaoGerenciadoraRisco, value: this.ValidacaoGerenciadoraRisco },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.VeiculoComInsumos, value: this.VeiculoComInsumos },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.VeiculoNaoMonitorado, value: this.VeiculoNaoMonitorado },
        ];
    },
    obterOpcoesAcompanhamentoCarga: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.AtrasoInicioViagem, value: this.AtrasoInicioViagem },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.InicioViagem, value: this.InicioViagem },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.FimViagem, value: this.FimViagem },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.ConfirmacaoColetaEntrega, value: this.ConfirmacaoColetaEntrega },
            { text: Localization.Resources.Enumeradores.TipoAlertaCarga.AtendimentoChamadoIniciado, value: this.AtendimentoIniciado },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoAlertaCarga = Object.freeze(new EnumTipoAlertaCargaHelper());