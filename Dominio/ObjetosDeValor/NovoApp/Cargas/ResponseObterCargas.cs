using System.Collections.Generic;
using Newtonsoft.Json;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.NovoApp.Cargas
{
    public partial class ResponseObterCargas
    {
        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("Configuracoes")]
        public Configuracoes Configuracoes { get; set; }

        [JsonProperty("Filial")]
        public Filial Filial { get; set; }

        [JsonProperty("NumeroCargaEmbarcador")]
        public string NumeroCargaEmbarcador { get; set; }

        [JsonProperty("Origens")]
        public string Origens { get; set; }

        [JsonProperty("Destinos")]
        public string Destinos { get; set; }

        [JsonProperty("Paradas")]
        public List<Parada> Paradas { get; set; }

        [JsonProperty("Peso")]
        public decimal Peso { get; set; }

        [JsonProperty("Polilinha")]
        public string Polilinha { get; set; }

        [JsonProperty("Situacao")]
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaColetaEntega Situacao { get; set; }

        [JsonProperty("TipoCarga")]
        public TipoCarga TipoCarga { get; set; }

        [JsonProperty("TipoOperacao")]
        public TipoOperacao TipoOperacao { get; set; }

        [JsonProperty("Veiculo")]
        public Veiculo Veiculo { get; set; }

        [JsonProperty("Motorista")]
        public List<Motorista> Motoristas { get; set; }

        [JsonProperty("ViagemIniciada")]
        public bool ViagemIniciada { get; set; }

        [JsonProperty("JustificativasTemperatura")]
        public List<JustificativaTemperatura> JustificativasTemperatura { get; set; }

        [JsonProperty("MotivosRejeicaoEntrega")]
        public List<MotivoRejeicaoEntrega> MotivosRejeicaoEntrega { get; set; }

        [JsonProperty("MotivosRejeicaoColeta")]
        public List<MotivoRejeicaoColeta> MotivosRejeicaoColeta { get; set; }

        [JsonProperty("MotivosRetificacaoColeta")]
        public List<Embarcador.Mobile.ControleEntrega.MotivoRetificacaoColeta> MotivosRetificacaoColeta { get; set; }

        [JsonProperty("TiposEventos")]
        public List<TipoEvento> TiposEventos { get; set; }

        [JsonProperty("MotivosFalhaGta")]
        public List<MotivoFalhaGta> MotivosFalhaGta { get; set; }

        [JsonProperty("MotivosFalhaNotaFiscal")]
        public List<MotivoFalhaNotaFiscal> MotivosFalhaNotaFiscal { get; set; }

        [JsonProperty("RegrasReconhecimentoCanhoto")]
        public List<RegraReconhecimentoCanhoto> RegrasReconhecimentoCanhoto { get; set; }

        [JsonProperty("DataCriacao")]
        public long DataCriacao;

        [JsonProperty("DataPrevisaoInicioViagem")]
        public long? DataPrevisaoInicioViagem;

        [JsonProperty("Temperatura")]
        public string Temperatura { get; set; }

        [JsonProperty("DistanciaTotal")]
        public decimal DistanciaTotal { get; set; }

        [JsonProperty("VolumeTotal")]
        public string VolumeTotal { get; set; }

        [JsonProperty("Duracao")]
        public string Duracao { get; set; }

        [JsonProperty("BloquearRastreamento")]
        public bool BloquearRastreamento { get; set; }

        [JsonProperty("DataLimiteConfirmacaoMotorista")]
        public long? DataLimiteConfirmacaoMotorista { get; set; }

        [JsonProperty("NecessarioConfirmacaoMotorista")]
        public bool NecessarioConfirmacaoMotorista { get; set; }

        [JsonProperty("ObservacaoTransportador")]
        public string ObservacaoTransportador { get; set; }

        [JsonProperty("DataCarregamento")]
        public long? DataCarregamento { get; set; }

        [JsonProperty("DataAgendamento")]
        public long? DataAgendamento { get; set; }

        [JsonProperty("URLAcessoDocumentos")]
        public string URLAcessoDocumentosCarga { get; set; }
    }

    public partial class Configuracoes
    {
        [JsonProperty("AtualizarCargaAutomaticamente")]
        public bool AtualizarCargaAutomaticamente { get; set; }

        [JsonProperty("AguardarAnaliseNaoConformidadesNFsCheckin")]
        public bool AguardarAnaliseNaoConformidadesNFsCheckin { get; set; }

        [JsonProperty("BloquearRastreamento")]
        public bool BloquearRastreamento { get; set; }

        [JsonProperty("ControlarTempoColeta")]
        public bool ControlarTempoColeta { get; set; }

        [JsonProperty("ControlarTempoEntrega")]
        public bool ControlarTempoEntrega { get; set; }

        [JsonProperty("DevolucaoProdutosPorPeso")]
        public bool DevolucaoProdutosPorPeso { get; set; }

        [JsonProperty("ExibirCalculadoraMobile")]
        public bool ExibirCalculadoraMobile { get; set; }

        [JsonProperty("ExibirRelatorio")]
        public bool ExibirRelatorio { get; set; }

        [JsonProperty("ForcarPreenchimentoSequencialMobile")]
        public bool ForcarPreenchimentoSequencialMobile { get; set; }

        [JsonProperty("HabilitarControleFluxoNFeDevolucaoChamado")]
        public bool HabilitarControleFluxoNFeDevolucaoChamado { get; set; }

        [JsonProperty("NaoPermiteRejeitarEntrega")]
        public bool NaoPermiteRejeitarEntrega { get; set; }

        [JsonProperty("NaoRetornarColetas")]
        public bool NaoRetornarColetas { get; set; }

        [JsonProperty("NaoUtilizarProdutosNaColeta")]
        public bool NaoUtilizarProdutosNaColeta { get; set; }

        [JsonProperty("ObrigarAssinaturaEntrega")]
        public bool ObrigarAssinaturaEntrega { get; set; }

        [JsonProperty("ObrigarAssinaturaProdutor")]
        public bool ObrigarAssinaturaProdutor { get; set; }

        [JsonProperty("ExibirAvaliacaoNaAssinatura")]
        public bool ExibirAvaliacaoNaAssinatura { get; set; }

        [JsonProperty("PermiteBaixarOsDocumentosDeTransporte")]
        public bool PermiteBaixarOsDocumentosDeTransporte { get; set; }

        [JsonProperty("PermitirEscanearChavesNfe")]
        public bool PermitirEscanearChavesNfe { get; set; }

        [JsonProperty("ObrigarEscanearChavesNfe")]
        public bool ObrigarEscanearChavesNfe { get; set; }

        [JsonProperty("ObrigarDadosRecebedor")]
        public bool ObrigarDadosRecebedor { get; set; }

        [JsonProperty("ObrigarFotoCanhoto")]
        public bool ObrigarFotoCanhoto { get; set; }

        [JsonProperty("ObrigarFotoNaDevolucao")]
        public bool ObrigarFotoNaDevolucao { get; set; }

        [JsonProperty("ObrigarInformarRIC")]
        public bool ObrigarInformarRIC { get; set; }

        [JsonProperty("ObrigarHandlingUnit")]
        public bool ObrigarHandlingUnit { get; set; }

        [JsonProperty("ObrigarJustificativaSolicitacoesForaAreaCliente")]
        public bool ObrigarJustificativaSolicitacoesForaAreaCliente { get; set; }

        [JsonProperty("PermiteCanhotoModoManual")]
        public bool PermiteCanhotoModoManual { get; set; }

        [JsonProperty("PermiteChat")]
        public bool PermiteChat { get; set; }

        [JsonProperty("PermiteConfirmarChegadaColeta")]
        public bool PermiteConfirmarChegadaColeta { get; set; }

        [JsonProperty("PermiteConfirmarChegadaEntrega")]
        public bool PermiteConfirmarChegadaEntrega { get; set; }

        [JsonProperty("PermiteConfirmarEntrega")]
        public bool PermiteConfirmarEntrega { get; set; }

        [JsonProperty("PermiteEntregaParcial")]
        public bool PermiteEntregaParcial { get; set; }

        [JsonProperty("PermiteEnviarNotasComplementaresAposEmissaoDocumentosTransporte")]
        public bool PermiteEnviarNotasComplementaresAposEmissaoDocumentosTransporte { get; set; }

        [JsonProperty("PermiteEventos")]
        public bool PermiteEventos { get; set; }

        [JsonProperty("PermiteFotos")]
        public bool PermiteFotosEntrega { get; set; }

        [JsonProperty("PermiteImpressaoMobile")]
        public bool PermiteImpressaoMobile { get; set; }

        [JsonProperty("PermiteQRCodeMobile")]
        public bool PermiteQrCodeMobile { get; set; }

        [JsonProperty("PermiteRetificarMobile")]
        public bool PermiteRetificarMobile { get; set; }

        [JsonProperty("PermiteSAC")]
        public bool PermiteSac { get; set; }

        [JsonProperty("PermitirVisualisarProgramacaoAntesViagem")]
        public bool PermitirVisualisarProgramacaoAntesViagem { get; set; }

        [JsonProperty("QuantidadeMinimasFotos")]
        public long QuantidadeMinimasFotosEntrega { get; set; }

        [JsonProperty("ServerChatURL")]
        public string ServerChatUrl { get; set; }

        [JsonProperty("SolicitarReconhecimentoFacialDoRecebedor")]
        public bool SolicitarReconhecimentoFacialDoRecebedor { get; set; }

        [JsonProperty("ValidarCapacidadeMaximaNoApp")]
        public bool ValidarCapacidadeMaximaNoApp { get; set; }

        [JsonProperty("ExigeSenhaClienteRecebimento")]
        public bool ExigeSenhaClienteRecebimento { get; set; }

        [JsonProperty("NumeroTentativasSenhaClientePermitidas")]
        public int NumeroTentativasSenhaClientePermitidas { get; set; }

        [JsonProperty("PermiteFotosColeta")]
        public bool PermiteFotosColeta { get; set; }

        [JsonProperty("QuantidadeMinimasFotosColeta")]
        public int QuantidadeMinimasFotosColeta { get; set; }

        [JsonProperty("NaoListarProdutosColetaEntrega")]
        public bool NaoListarProdutosColetaEntrega { get; set; }

        [JsonProperty("RetornarMultiplasCargasApp")]
        public bool RetornarMultiplasCargasApp { get; set; }

        [JsonProperty("NaoApresentarDataInicioViagem")]
        public bool NaoApresentarDataInicioViagem { get; set; }
    }

    public partial class Filial
    {
        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("Nome")]
        public string Nome { get; set; }
    }

    public partial class JustificativaTemperatura
    {
        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("Descricao")]
        public string Descricao { get; set; }
    }

    public partial class TipoCarga
    {
        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("Descricao")]
        public string Descricao { get; set; }
    }

    public partial class TipoOperacao
    {
        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("Descricao")]
        public string Descricao { get; set; }
    }

    public partial class TipoEvento
    {
        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("Descricao")]
        public string Descricao { get; set; }
    }

    public partial class MotivoFalhaGta
    {
        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("Descricao")]
        public string Descricao { get; set; }

        [JsonProperty("ExigirFotoGTA")]
        public bool ExigirFotoGTA { get; set; }
    }

    public partial class MotivoFalhaNotaFiscal
    {
        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("Descricao")]
        public string Descricao { get; set; }
    }

    public partial class MotivoRejeicaoColeta
    {
        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("Descricao")]
        public string Descricao { get; set; }

        [JsonProperty("CodigoTipoOperacaoColeta")]
        public long CodigoTipoOperacaoColeta { get; set; }

        [JsonProperty("NaoAlterarSituacaoColetaEntrega")]
        public bool NaoAlterarSituacaoColetaEntrega { get; set; }

        [JsonProperty("AguardandoTratativa")]
        public bool AguardarTratativa { get; set; }
    }

    public partial class MotivoRetificacaoColeta
    {
        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("Descricao")]
        public string Descricao { get; set; }
    }

    public partial class MotivoRejeicaoEntrega
    {
        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("Descricao")]
        public string Descricao { get; set; }

        [JsonProperty("AguardandoTratativa")]
        public bool AguardarTratativa { get; set; }

        [JsonProperty("ObrigarFoto")]
        public bool ObrigarFoto { get; set; }

        [JsonProperty("Devolucao")]
        public bool Devolucao { get; set; }

        [JsonProperty("CodigoTipoOperacaoEntrega")]
        public long CodigoTipoOperacaoEntrega { get; set; }

        [JsonProperty("NaoAlterarSituacaoColetaEntrega")]
        public bool NaoAlterarSituacaoColetaEntrega { get; set; }

        [JsonProperty("Canais")]
        public List<CanalDeEntrega> Canais { get; set; }

        [JsonProperty("ObrigarInformarValorNaLiberacao")]
        public bool ObrigarInformarValorNaLiberacao { get; set; }

        [JsonProperty("ObrigarMotoristaInformarMultiMobile")]
        public bool ObrigarMotoristaInformarMultiMobile { get; set; }
    }

    public class CanalDeEntrega
    {
        [JsonProperty("Codigo")]
        public int Codigo { get; set; }

        [JsonProperty("Descricao")]
        public string Descricao { get; set; }
    }

    public partial class Parada
    {
        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("Tipo")]
        public TipoCargaEntrega Tipo { get; set; }

        [JsonProperty("Coleta")]
        public bool Coleta { get; set; }

        [JsonProperty("ColetaEquipamento")]
        public bool ColetaEquipamento { get; set; }

        [JsonProperty("DataInicio")]
        public long? dataInicio { get; set; }

        [JsonProperty("DataFim")]
        public long? dataFim { get; set; }

        [JsonProperty("DataConfirmacao")]
        public long? dataConfirmacao { get; set; }

        [JsonProperty("DataProgramada")]
        public long? DataProgramada { get; set; }

        [JsonProperty("dataConfirmacaoChegada")]
        public long? dataConfirmacaoChegada { get; set; }

        [JsonProperty("EstouIndo ")]
        public bool EstouIndo { get; set; }

        [JsonProperty("DiferencaDevolucao")]
        public bool DiferencaDevolucao { get; set; }

        [JsonProperty("Endereco")]
        public string Endereco { get; set; }

        [JsonProperty("DataEntradaPropriedade")]
        public long? DataEntradaPropriedade { get; set; }

        [JsonProperty("JanelaDescarga")]
        public object JanelaDescarga { get; set; }

        [JsonProperty("Ordem")]
        public long Ordem { get; set; }

        [JsonProperty("MotivoDevolucao")]
        public MotivoDevolucao MotivoDevolucao { get; set; }

        [JsonProperty("MotivoRejeicaoColeta")]
        public MotivoRejeicaoColeta MotivoRejeicaoColeta { get; set; }

        [JsonProperty("MotivoRetificacaoColeta")]
        public MotivoRetificacaoColeta MotivoRetificacaoColeta { get; set; }

        [JsonProperty("Canhotos")]
        public List<Canhoto> Canhotos { get; set; }

        [JsonProperty("Notas")]
        public List<Nota> Notas { get; set; }

        [JsonProperty("Questionario")]
        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.CheckList> Questionario { get; set; }

        [JsonProperty("Cliente")]
        public Cliente Cliente { get; set; }

        [JsonProperty("Pedidos")]
        public List<Pedido> Pedidos { get; set; }

        [JsonProperty("Peso")]
        public decimal Peso { get; set; }

        [JsonProperty("PossuiReentrega")]
        public bool PossuiReentrega { get; set; }

        [JsonProperty("Situacao")]
        public SituacaoEntrega Situacao { get; set; }

        [JsonProperty("Ocorrencias")]
        public List<Ocorrencia> Ocorrencias { get; set; }

        [JsonProperty("ObservacoesPedidos")]
        public string ObservacoesPedidos { get; set; }

        [JsonProperty("DataAgendamento")]
        public long? DataAgendamento { get; set; }

        [JsonProperty("ObservacaoAgendamento")]
        public string ObservacaoAgendamento { get; set; }

        [JsonProperty("ResponsavelAgendamento")]
        public string ResponsavelAgendamento { get; set; }

        [JsonProperty("chavesNFe")]
        public List<string> chavesNFe { get; set; }

        [JsonProperty("NaoExigeDigitalizacaoDoCanhotoNaConfirmacaoDaEntrega")]
        public bool NaoExigeDigitalizacaoDoCanhotoNaConfirmacaoDaEntrega { get; set; }
    }

    public partial class Ocorrencia
    {
        public long Codigo { get; set; }
        public int Numero { get; set; }
        public bool DevolucaoParcial { get; set; }
        public string Observacao { get; set; }
        public SituacaoChamado Situacao { get; set; }
        public long? data { get; set; }
        public string TratativaDevolucao { get; set; }
    }

    public partial class Canhoto
    {
        [JsonProperty("Codigo")]
        public int Codigo { get; set; }

        [JsonProperty("Numero")]
        public long Numero { get; set; }

        [JsonProperty("TipoCanhoto")]
        public ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto TipoCanhoto { get; set; }

        [JsonProperty("DigitalizacaoCanhotoInteiro")]
        public bool DigitalizacaoCanhotoInteiro { get; set; }

        [JsonProperty("Identificacao")]
        public string Identificacao { get; set; }
    }

    public partial class Cliente
    {
        [JsonProperty("CodigoIntegracao")]
        public string CodigoIntegracao { get; set; }

        [JsonProperty("RaioEmMetros")]
        public long RaioEmMetros { get; set; }

        [JsonProperty("RazaoSocial")]
        public string RazaoSocial { get; set; }

        [JsonProperty("Observacao")]
        public string Observacao { get; set; }

        [JsonProperty("Endereco")]
        public Endereco Endereco { get; set; }

        [JsonProperty("NomeFantasia")]
        public string NomeFantasia { get; set; }

        [JsonProperty("Telefone")]
        public Telefone Telefone { get; set; }

        [JsonProperty("SenhaConfirmacaoColetaEntrega")]
        public string SenhaConfirmacaoColetaEntrega { get; set; }

        [JsonProperty("NaoEObrigatorioInformarNfeNaColeta")]
        public bool NaoEObrigatorioInformarNfeNaColeta { get; set; }

        [JsonProperty("NaoExigePreenchimentoDeChecklistEntrega")]
        public bool NaoExigePreenchimentoDeChecklistEntrega { get; set; }

        [JsonProperty("NaoAplicarChecklist")]
        public bool NaoAplicarChecklist { get; set; }
    }

    public partial class Endereco
    {
        [JsonProperty("Cidade")]
        public string Cidade { get; set; }

        [JsonProperty("Uf")]
        public string Uf { get; set; }

        [JsonProperty("Latitude")]
        public string Latitude { get; set; }

        [JsonProperty("Longitude")]
        public string Longitude { get; set; }
    }

    public partial class Telefone
    {
        [JsonProperty("CodigoPais")]
        public string CodigoPais { get; set; }

        [JsonProperty("Numero")]
        public string Numero { get; set; }
    }

    public partial class MotivoDevolucao
    {
        [JsonProperty("AguardarTratativa")]
        public bool AguardarTratativa { get; set; }

        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("Devolucao")]
        public bool Devolucao { get; set; }

        [JsonProperty("Motivo")]
        public string Motivo { get; set; }

        [JsonProperty("ObrigarFoto")]
        public bool ObrigarFoto { get; set; }
    }

    public partial class Nota
    {
        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("Chave")]
        public string Chave { get; set; }

        [JsonProperty("NumeroNota")]
        public string NumeroNota { get; set; }

        [JsonProperty("Devolvida")]
        public bool Devolvida { get; set; }

        [JsonProperty("DevolvidaParcial")]
        public bool DevolvidaParcial { get; set; }
    }

    public partial class Pedido
    {
        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("NumeroPedido")]
        public string NumeroPedido { get; set; }

        [JsonProperty("Produtos")]
        public List<Produto> Produtos { get; set; }

        [JsonProperty("Observacao")]
        public string Observacao { get; set; }

        [JsonProperty("CanalEntrega")]
        public CanalEntrega CanalEntrega { get; set; }

        [JsonProperty("QuantidadeVolumesDestino")]
        public int QuantidadeVolumesDestino { get; set; }
    }

    public partial class CanalEntrega
    {
        [JsonProperty("Codigo")]
        public string Codigo { get; set; }

        [JsonProperty("Descricao")]
        public string Descricao { get; set; }
    }

    public partial class Produto
    {
        [JsonProperty("Codigo")]
        public string Codigo { get; set; }

        [JsonProperty("Descricao")]
        public string Descricao { get; set; }

        [JsonProperty("ImunoPlanejado")]
        public int? ImunoPlanejado { get; set; }

        [JsonProperty("ImunoRealizado")]
        public int? ImunoRealizado { get; set; }

        [JsonProperty("InformarDadosColeta")]
        public bool InformarDadosColeta { get; set; }

        [JsonProperty("InformarTemperatura")]
        public bool InformarTemperatura { get; set; }

        [JsonProperty("ExigeInformarImunos")]
        public bool ExigeInformarImunos { get; set; }

        [JsonProperty("ExigeInformarCaixas")]
        public bool ExigeInformarCaixas { get; set; }

        [JsonProperty("ObrigatorioGuiaTransporteAnimal")]
        public bool ObrigatorioGuiaTransporteAnimal { get; set; }

        [JsonProperty("ObrigatorioNFProdutor")]
        public bool ObrigatorioNfProdutor { get; set; }

        [JsonProperty("Observacao")]
        public string Observacao { get; set; }

        [JsonProperty("PesoUnitario")]
        public decimal PesoUnitario { get; set; }

        [JsonProperty("NumeroAndares")]
        public long NumeroAndares { get; set; }

        [JsonProperty("NumeroLinhas")]
        public long NumeroLinhas { get; set; }

        [JsonProperty("NumeroColunas")]
        public long NumeroColunas { get; set; }

        [JsonProperty("Divisoes")]
        public List<Divisao> Divisoes { get; set; }

        [JsonProperty("Protocolo")]
        public long Protocolo { get; set; }

        [JsonProperty("ProtocoloCargaEntregaProduto")]
        public long ProtocoloCargaEntregaProduto { get; set; }

        [JsonProperty("Quantidade")]
        public decimal Quantidade { get; set; }

        [JsonProperty("QuantidadeCaixa")]
        public long QuantidadeCaixa { get; set; }

        [JsonProperty("QuantidadePorCaixaRealizada")]
        public long? QuantidadePorCaixaRealizada { get; set; }

        [JsonProperty("QuantidadeCaixasVazias")]
        public long QuantidadeCaixasVazias { get; set; }

        [JsonProperty("QuantidadeCaixasVaziasRealizada")]
        public long? QuantidadeCaixasVaziasRealizada { get; set; }

        [JsonProperty("QuantidadeDevolucao")]
        public decimal QuantidadeDevolucao { get; set; }

        [JsonProperty("QuantidadePlanejada")]
        public decimal QuantidadePlanejada { get; set; }

        [JsonProperty("Temperatura")]
        public decimal Temperatura { get; set; }

        [JsonProperty("UnidadeDeMedida")]
        public UnidadeDeMedida UnidadeDeMedida { get; set; }

        [JsonProperty("MotivoTemperatura")]
        public long? MotivoTemperatura { get; set; }

        [JsonProperty("Nota")]
        public string Nota { get; set; }
    }

    public partial class Divisao
    {
        [JsonProperty("Codigo")]
        public int Codigo { get; set; }

        [JsonProperty("Andar")]
        public int Andar { get; set; }

        [JsonProperty("Linha")]
        public int Linha { get; set; }

        [JsonProperty("Coluna")]
        public int Coluna { get; set; }

        [JsonProperty("Quantidade")]
        public decimal Quantidade { get; set; }

        [JsonProperty("QuantidadePlanejada")]
        public decimal QuantidadePlanejada { get; set; }

        [JsonProperty("Capacidade")]
        public decimal Capacidade { get; set; }

        [JsonProperty("UnidadeDeMedida")]
        public UnidadeDeMedida UnidadeDeMedida { get; set; }
    }

    public partial class UnidadeDeMedida
    {
        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("Descricao")]
        public string Descricao { get; set; }

        [JsonProperty("UnidadeMedida")]
        public Dominio.Enumeradores.UnidadeMedida UnidadeMedida { get; set; }
    }

    public partial class RegraReconhecimentoCanhoto
    {
        [JsonProperty("Codigo")]
        public long Codigo { get; set; }

        [JsonProperty("PalavrasChaves")]
        public List<string> PalavrasChaves { get; set; }
    }

    public partial class Veiculo
    {
        [JsonProperty("Placa")]
        public string Placa { get; set; }

        [JsonProperty("NumeroAndares")]
        public int NumeroAndares { get; set; }

        [JsonProperty("NumeroColunas")]
        public int NumeroColunas { get; set; }

        [JsonProperty("NumeroLinhas")]
        public int NumeroLinhas { get; set; }

        [JsonProperty("Divisoes")]
        public List<DivisaoCapacidadeModeloVeicular> Divisoes { get; set; }
    }

    public partial class Transportadora
    {
        [JsonProperty("Codigo")]
        public int Codigo { get; set; }

        [JsonProperty("Nome")]
        public string Nome { get; set; }
    }

    public partial class Motorista
    {
        [JsonProperty("Codigo")]
        public int Codigo { get; set; }

        [JsonProperty("Nome")]
        public string Nome { get; set; }

        [JsonProperty("Transportadora")]
        public Transportadora Transportadora { get; set; }
    }

    public partial class DivisaoCapacidadeModeloVeicular
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public decimal Capacidade { get; set; }
        public int? Andar { get; set; }
        public int? Coluna { get; set; }
        public int? Linha { get; set; }
    }
}
