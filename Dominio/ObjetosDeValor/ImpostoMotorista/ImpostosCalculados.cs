namespace Dominio.ObjetosDeValor.ImpostoMotorista
{
    public class ImpostosCalculados
    {
        public int Protocolo { get; set; }
        public decimal ValorINSSContratado { get; set; }
        public decimal AliquotaINSSContratado { get; set; }
        public decimal AliquotaIRRFContratado { get; set; }
        public decimal ValorIRRFContratado { get; set; }
        public decimal AliquotaSESTContratado { get; set; }
        public decimal ValorSESTContratado { get; set; }
        public decimal AliquotaSENATContratado { get; set; }
        public decimal ValorSENATContratado { get; set; }

        public decimal ValorINSSContratante { get; set; }
        public decimal AliquotaINSSContratante { get; set; }
        //public decimal AliquotaIRRFContratante { get; set; }
        //public decimal ValorIRRFContratante { get; set; }
        public decimal AliquotaSESTContratante { get; set; }
        public decimal ValorSESTContratante { get; set; }
        public decimal AliquotaSENATContratante { get; set; }
        public decimal ValorSENATContratante { get; set; }
        //public decimal AliquotaSalarioEducacaoContratante { get; set; }
        //public decimal ValorSalarioEducacaoContratante { get; set; }

        //public decimal AliquotaINCRAContratante { get; set; }
        //public decimal ValorINCRAContratante { get; set; }

        public string DescricaoTabelaUtilizada { get; set; }
        public decimal ValorDescontoIRRFDependentes { get; set; }
    }
}
