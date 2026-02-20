namespace Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete
{
    public sealed class FaixaCEP
    {
        public int CEPFinal { get; set; }

        public int CEPInicial { get; set; }

        public int DiasUteis { get; set; }

        public string Descricao
        {
            get { return $"{string.Format("{0:00000-000}", CEPInicial)} à {string.Format("{0:00000-000}", CEPFinal)}"; }
        }
    }
}
