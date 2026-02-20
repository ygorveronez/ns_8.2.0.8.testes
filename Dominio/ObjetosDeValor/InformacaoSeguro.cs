namespace Dominio.ObjetosDeValor
{
    public class InformacaoSeguro
    {
        public int Id;
        public Dominio.Enumeradores.TipoSeguro Responsavel;
        public string CNPJResponsavel;
        public string DescricaoResponsavel;
        public string Seguradora;
        public string NumeroApolice;
        public string CNPJSeguradora;
        public string NumeroAverberacao;
        public decimal ValorMercadoria;
        public bool Excluir;
    }
}
