namespace Servicos.Embarcador.Hubs
{
	public class Fechamento : HubBase<Fechamento>
	{
		public void InformarFechamentoAtualizada(int codigoFechamento, string stringConexao, Dominio.Entidades.Usuario usuarioAtualizouFechamento = null)
		{
			var retorno = new
			{
				CodigoFechamento = codigoFechamento
			};

			if (usuarioAtualizouFechamento != null)
				SendToClient(usuarioAtualizouFechamento.Codigo.ToString(), "informarFechamentoAtualizada", retorno);
			else
				SendToAll("informarFechamentoAtualizada", retorno);
		}
	}
}
