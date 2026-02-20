using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pallets
{
    public sealed class ControleTransferenciaPallet
    {
        public int Codigo { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public string DataRecebimento { get; set; }
        public string FilialSolicitacao { get; set; }
        public string FilialCnpjSolicitacao { get; set; }
        public string FilialCodigoIntegracaoSolicitacao { get; set; }
        public int QuantidadeSolicitacao { get; set; }
        public int QuantidadeEnvio { get; set; }
        public int QuantidadeRecebimento { get; set; }
        public string Recebedor { get; set; }
        public string Remetente { get; set; }
        public string ResponsavelEnvio { get; set; }
        public string SetorSolicitacao { get; set; }
        public string Situacao { get; set; }
        public string Solicitante { get; set; }
        public string TurnoSolicitacao { get; set; }
    }
}
