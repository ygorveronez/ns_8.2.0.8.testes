namespace Dominio.ObjetosDeValor.Relatorios
{
    public class DocumentoDACTE
    {
        public int Codigo { get; set; }

        public string CNPJEmitente { get; set; }

        public string NumeroModelo { get; set; }

        public string ChaveNFE { get; set; }

        public string Serie { get; set; }

        public string Numero { get; set; }

        public string Descricao { get; set; }

        public virtual string CPFCNPJEmitente_Formatado
        {
            get
            {
                long cpf_cnpj = 0L;

                if (long.TryParse(this.CNPJEmitente, out cpf_cnpj))
                {
                    return this.CNPJEmitente.Length > 11 ? string.Format(@"{0:00\.000\.000\/0000\-00}", cpf_cnpj) : string.Format(@"{0:000\.000\.000\-00}", cpf_cnpj);
                }
                else
                {
                    return this.CNPJEmitente;
                }
            }
        }
    }
}
