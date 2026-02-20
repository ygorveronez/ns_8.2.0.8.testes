namespace Servicos.Embarcador.Hubs
{
	public class AjusteTabela : HubBase<AjusteTabela>
	{
		public void InformarAjusteTabelaAtualizado(int codigo, Dominio.Entidades.Usuario usuario = null)
		{
			var retorno = new
			{
				Codigo = codigo
			};

			if (usuario != null)
				SendToAllExcept(usuario.Codigo.ToString(), "informarAjusteTabelaAtualizado", retorno);
			else
				SendToAll("informarAjusteTabelaAtualizado", retorno);
		}

		public void InformarAjusteTabelaAplicado(int codigo, Dominio.Entidades.Usuario usuario = null)
		{
			var retorno = new
			{
				Codigo = codigo
			};

			if (usuario != null)
				SendToAllExcept(usuario.Codigo.ToString(), "informarAjusteTabelaAplicado", retorno);
			else
				SendToAll("informarAjusteTabelaAplicado", retorno);
		}
	}
}
