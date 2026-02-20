public class ConfiguracaoExibicaoDetalhesEntrega
{
    public ConfiguracaoExibicaoDetalhesEntregaCliente ConfiguracaoExibicaoDetalhesEntregaCliente { get; set; }
    public ConfiguracaoExibicaoDetalhesEntregaEntregaColeta ConfiguracaoExibicaoDetalhesEntregaEntregaColeta { get; set; }
    public ConfiguracaoExibicaoDetalhesEntregaCadeiaAjuda ConfiguracaoExibicaoDetalhesEntregaCadeiaAjuda { get; set; }
}

public class ConfiguracaoExibicaoDetalhesEntregaCliente
{
    public bool NomeRecebedor { get; set; }
    public bool EtapaStage { get; set; }
    public bool DocumentoRecebedor { get; set; }
    public bool EnderecoCliente { get; set; }
    public bool LocalidadeCliente { get; set; }
    public bool Localizacao { get; set; }
    public bool Email { get; set; }
    public bool DataEntradaRaio { get; set; }
    public bool DataSaidaRaio { get; set; }
    public bool JanelaDescarga { get; set; }
    public bool TelefoneCliente { get; set; }
    public bool DataPrevisaoEntrega { get; set; }
    public bool DataPrevisaoSaida { get; set; }
    public bool CodigoSap { get; set; }
    public bool InicioViagemPrevista { get; set; }
    public bool InicioViagemRealizada { get; set; }
    public bool DataEntregaReprogramada { get; set; }
    public bool Mesoregiao { get; set; }
    public bool Regiao { get; set; }
    public bool SelecionarTodos { get; set; }
    public bool CodigoIntegracaoFilial { get; set; }
    public bool CodigoIntegracaoCliente { get; set; }
}

public class ConfiguracaoExibicaoDetalhesEntregaEntregaColeta
{
    public bool EntregaNoPrazo { get; set; }
    public bool OrdemEntrega { get; set; }
    public bool Situacao { get; set; }
    public bool DistanciaOrigemXEntrega { get; set; }
    public bool LocalidadeEntrega { get; set; }
    public bool Pedidos { get; set; }
    public bool NumeroPedidoCliente { get; set; }
    public bool NotasFiscais { get; set; }
    public bool QuantidadePacotesColetados { get; set; }
    public bool Peso { get; set; }
    public bool Observacao { get; set; }
    public bool NumeroCTe { get; set; }
    public bool NumeroChamado { get; set; }
    public bool TempoRestanteChamado { get; set; }
    public bool ObservacoesAgendamento { get; set; }
    public bool DataProgramadaColeta { get; set; }
    public bool TempoProgramadaColeta { get; set; }
    public bool DataProgramadaDescarga { get; set; }
    public bool DataInicio { get; set; }
    public bool DataFim { get; set; }
    public bool DataConfirmacao { get; set; }
    public bool DataConfirmacaoApp { get; set; }
    public bool DataRejeitado { get; set; }
    public bool ResponsavelFinalizacaoManual { get; set; }
    public bool QuantidadePlanejada { get; set; }
    public bool QuantidadeTotal { get; set; }
    public bool InfoMotivoRejeicao { get; set; }
    public bool InfoMotivoRetificacao { get; set; }
    public bool DataReentregaMesmaCarga { get; set; }
    public bool DataEntregaNota { get; set; }
    public bool StatusEntregaNota { get; set; }
    public bool LeadTimeTransportador { get; set; }
    public bool ObservacoesPedidos { get; set; }
    public bool Assinatura { get; set; }
    public bool AlterarDataAgendamentoEntregaTransportador { get; set; }
    public bool DataAgendamentoDeEntrega { get; set; }
    public bool DataAgendamentoEntregaTransportador { get; set; }
    public bool DataPrevisaoEntrega { get; set; }
    public bool FilialVenda { get; set; }
    public bool DataEntregaReprogramada { get; set; }
    public bool DataPrevisaoEntregaTransportador { get; set; }
    public bool OrigemSituacaoDataAgendamentoEntrega { get; set; }
    public bool JustificativaOnTime { get; set; }
    public bool OnTime { get; set; }
    public bool DataInicioCarregamentoOuDescarregamento { get; set; }
    public bool DataTerminoCarregamentoOuDescarregamento { get; set; }
    public bool TempoCarregamentoOuDescarregamento { get; set; }
    public bool Parqueada { get; set; }
    public bool DataEmissaoNota { get; set; }
    public bool DataRejeicaoEntrega { get; set; }
    public bool StatusTendenciaEntrega { get; set; }
    public bool DataPrevisaoEntregaAjustada { get; set; }
    public bool DataConfirmacaoEntrega { get; set; }
    public bool SelecionarTodos { get; set; }
}

public class ConfiguracaoExibicaoDetalhesEntregaCadeiaAjuda
{
    public bool GerenteNacional { get; set; }
    public bool GerenteRegional { get; set; }
    public bool Gerente { get; set; }
    public bool Supervisor { get; set; }
    public bool Vendedor { get; set; }
    public bool EscritorioVenda { get; set; }
    public bool EquipeVendas { get; set; }
    public bool TipoMercadoria { get; set; }
    public bool CanalVenda { get; set; }
    public bool SelecionarTodos { get; set; }
    public bool MostrarNomeCadeiaAjuda { get; set; }
    public bool MostrarEmailCadeiaAjuda { get; set; }
    public bool MostrarTelefoneCadeiaAjuda { get; set; }
    public bool MostrarWhatsAppCadeiaAjuda { get; set; }
    public bool SelecionarTodosInformacoes { get; set; }
}
