namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaCentroResultado
    {
        public int CodigoEmpresa { get; set; }
        public string Descricao { get; set; }
        public string Plano { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa Ativo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico Tipo { get; set; }
        public int CodigoTipoMovimento { get; set; }
        public int CodigoUsuario { get; set; }
    }
}
