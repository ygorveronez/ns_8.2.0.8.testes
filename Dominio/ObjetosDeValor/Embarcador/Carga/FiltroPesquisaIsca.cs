namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaIsca
    {
        public Enumeradores.SituacaoAtivoPesquisa Status { get; set; }
        public string Descricao { get; set; }
        public int CodigoCarga { get; set; }
        public string CodigoIntegracao { get; set; }
    }
}
