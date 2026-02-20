namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class retornoObterTelefone
    {
        public string ddi { get; set; }

        public string ddd { get; set; }

        public string prefixo { get; set; }

        public string numero { get; set; }

        public override string ToString()
        {
            return ddi + ddd + prefixo + numero;
        }
    }
}
