namespace Dominio.ObjetosDeValor.EDI.OCOREN
{
    public class Total
    {
        public int NumeroRegistroOcorrencia { get; set; }
        public int TotalLinhas
        {
            get
            {
                return this.NumeroRegistroOcorrencia + 1;
            }
        }

    }
}
