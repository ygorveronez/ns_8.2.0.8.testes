namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaTrechoBalsa
    {
        public string Descricao { get; set; }
        public int CodigoPortoOrigem { get; set; }
        public int CodigoPortoDestino { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa Status { get; set; }
    }
}
