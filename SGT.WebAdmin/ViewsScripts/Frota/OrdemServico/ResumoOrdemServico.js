////*******MAPEAMENTO KNOUCKOUT*******

var _resumoOrdemServico;

var ResumoOrdemServico = function () {
    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(false) });
    this.Veiculo = PropertyEntity({ text: "Veículo: " });
    this.Motorista = PropertyEntity({ text: "Motorista: " });
    this.DataProgramada = PropertyEntity({ text: "Data Programada: " });
    this.Operador = PropertyEntity({ text: "Operador: " });
    this.TipoManutencao = PropertyEntity({ text: "Tipo de Manutenção: " });
    this.Situacao = PropertyEntity({ text: "Situação: " });
    this.LocalManutencao = PropertyEntity({ text: "Local da Manutenção: " });
    this.Motivo = PropertyEntity({ text: "Motivo: ", visible: ko.observable(false) });
}

//*******EVENTOS*******

function LoadResumoOrdemServico() {
    _resumoOrdemServico = new ResumoOrdemServico();
    KoBindings(_resumoOrdemServico, "knockoutResumoOrdemServico");
}

//*******MÉTODOS*******

function PreecherResumoOrdemServico(dados) {
    _resumoOrdemServico.Numero.visible(true);

    _resumoOrdemServico.Numero.val(dados.Numero);
    _resumoOrdemServico.Veiculo.val(dados.Veiculo.Descricao);
    _resumoOrdemServico.Motorista.val(dados.Motorista.Descricao);
    _resumoOrdemServico.DataProgramada.val(dados.DataProgramada);
    _resumoOrdemServico.Operador.val(dados.Operador.Descricao);
    _resumoOrdemServico.TipoManutencao.val(dados.DescricaoTipoManutencao);
    _resumoOrdemServico.Situacao.val(dados.DescricaoSituacao);
    _resumoOrdemServico.LocalManutencao.val(dados.LocalManutencao.Descricao);
    _resumoOrdemServico.Motivo.val(dados.Motivo);

    if (dados.Situacao == EnumSituacaoOrdemServicoFrota.Rejeitada || dados.Situacao == EnumSituacaoOrdemServicoFrota.Cancelada)
        _resumoOrdemServico.Motivo.visible(true);
    else
        _resumoOrdemServico.Motivo.visible(false);
}

function LimparResumoOrdemServico() {
    _resumoOrdemServico.Numero.visible(false);
    LimparCampos(_resumoOrdemServico);
}