using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Notificacao
{
    /// <summary>
    /// Essa classe é meio que uma gambiarra para criar um contrato padrão para o campo "Data" que vai mandar
    /// dados arbitrários dependendo do tipo de notificação do OneSignal.
    /// </summary>
    public class OneSignalData
    {
        public int? CodigoCarga;
        public int? CodigoCargaEntrega;
        public string Observacao;
        public SituacaoEntrega? SituacaoCargaEntrega;

        // Chamado
        public int? CodigoChamado;
        public string NomeAnalistaChamado;
        public SituacaoChamado? SituacaoChamado;
        public bool? DiferencaDevolucao;
        public string TratativaDevolucao;

        public MobileHubs? Tipo;
        public long DataCriacao;

        public long? DataComparecerEscalaPedido;
        public long? DataLimiteConfirmacaoMotorista;
    }
}
