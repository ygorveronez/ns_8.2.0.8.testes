namespace Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade
{
    public sealed class Localidade
    {
        public int Codigo { get; set; }

        public string Descricao { get; set; }

        public int IBGE { get; set; }

        public string CodigoIntegracao { get; set; }

        public string SiglaUF { get; set; }

        public Pais Pais { get; set; }

        public Regiao Regiao { get; set; }

        public string CodigoDocumento { get; set; }
    }
}
