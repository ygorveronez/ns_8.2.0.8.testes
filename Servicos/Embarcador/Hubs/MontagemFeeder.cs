namespace Servicos.Embarcador.Hubs
{
	public class MontagemFeeder : HubBase<MontagemFeeder>
	{
		public void NotificarMontagemAtualizado(Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder montagem)
		{
			SendToAll("informarMontagemFeederAtualizado", new { montagem.Codigo });
		}
	}
}
