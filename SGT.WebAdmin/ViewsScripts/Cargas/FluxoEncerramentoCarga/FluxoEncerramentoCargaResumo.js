//*******MAPEAMENTO KNOUCKOUT*******

var _resumoFluxoEncerramentoCarga;

var ResumoFluxoEncerramentoCarga = function () {
    this.NumeroCarga = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Carga.getFieldDescription(), visible: ko.observable(false) });
    this.Remetente = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Remetente.getFieldDescription() });
    this.Origem = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Origem.getFieldDescription() });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription() });
    this.Destino = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Destino.getFieldDescription() });
    this.DataEncerramento = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataCancelamento.getFieldDescription() });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.MotivoRejeicao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.MotivoRejeicao.getFieldDescription() });
}

//*******EVENTOS*******

function LoadResumoFluxoEncerramentoCarga() {
    _resumoFluxoEncerramentoCarga = new ResumoFluxoEncerramentoCarga();
    KoBindings(_resumoFluxoEncerramentoCarga, "knockoutResumoFluxoEncerramentoCarga");
}

//*******MÉTODOS*******

function PreecherResumoFluxoEncerramentoCarga(dados) {
    _resumoFluxoEncerramentoCarga.NumeroCarga.visible(true);

    _resumoFluxoEncerramentoCarga.NumeroCarga.val(dados.Carga.CodigoCargaEmbarcador);
    _resumoFluxoEncerramentoCarga.Remetente.val(dados.Carga.Remetente);
    _resumoFluxoEncerramentoCarga.Origem.val(dados.Carga.Origem);
    _resumoFluxoEncerramentoCarga.Destinatario.val(dados.Carga.Destinatario);
    _resumoFluxoEncerramentoCarga.Destino.val(dados.Carga.Destino);
    _resumoFluxoEncerramentoCarga.DataEncerramento.val(dados.DataEncerramento);
    _resumoFluxoEncerramentoCarga.Situacao.val(dados.DescricaoSituacao);
    _resumoFluxoEncerramentoCarga.MotivoRejeicao.val(dados.MotivoRejeicao);
}

function LimparResumoFluxoEncerramentoCarga() {
    _resumoFluxoEncerramentoCarga.NumeroCarga.visible(false);
    LimparCampos(_resumoFluxoEncerramentoCarga);
}