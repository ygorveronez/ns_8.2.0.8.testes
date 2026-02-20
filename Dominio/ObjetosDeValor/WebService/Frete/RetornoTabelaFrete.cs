namespace Dominio.ObjetosDeValor.WebService.Frete
{
    public class RetornoTabelaFrete
    {
        public string IDTarifaMulti { get; set; }

        public string IDTarifaJaggaer { get; set; }

        /// <summary>
        /// Status retornado com os possíveis valores:
        /// (D) Draft: Quando enviada a integração, não permitir editar até receber retorno
        /// (I) In Approval Process: Esta em aprovação
        /// (P) Proposed: Esta em aprovação
        /// (J) Rejected: Reprovada
        /// (A) Accepted: Aprovada
        /// (R) Resubmission Required: Esta em aprovação
        /// (T) Retired: Foi inativada
        /// </summary>
        public string Status { get; set; }
    }
}