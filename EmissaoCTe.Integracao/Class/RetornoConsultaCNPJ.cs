namespace EmissaoCTe.Integracao
{
    public class RetornoConsultaCNPJ
    {
        public string CNPJ { get; set; }

        public string  Estado { get; set; }

        public string IE { get; set; }

        public string StatusIE { get; set; }

        public string Regime { get; set; }
        
        public Dominio.Enumeradores.StatusConsultaCNPJ StatusConsulta { get; set; }

        public string ErroConsulta { get; set; }
    }
}

