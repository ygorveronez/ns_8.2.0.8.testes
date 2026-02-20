namespace Servicos.Embarcador.Hubs
{
	public class Avaria : HubBase<Avaria>
	{
		public void InformarLoteAtualizado(int codigolote, Dominio.Entidades.Usuario usuarioEnviouAvaria = null)
		{
			var retorno = new
			{
				CodigoLote = codigolote
			};

			if (usuarioEnviouAvaria != null)
				SendToAllExcept(usuarioEnviouAvaria.Codigo.ToString(), "informarAvariaAtualizada", retorno);
			else
				SendToAll("informarAvariaAtualizada", retorno);
		}
	}
}
