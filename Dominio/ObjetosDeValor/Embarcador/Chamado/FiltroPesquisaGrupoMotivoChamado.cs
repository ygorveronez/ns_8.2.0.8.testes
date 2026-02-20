namespace Dominio.ObjetosDeValor.Embarcador.Chamado
{
	public sealed class FiltroPesquisaGrupoMotivoChamado
	{
		public string Descricao { get; set; }
		public string CodigoIntegracao { get; set; }
		public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa Situacao { get; set; }
	}
}
