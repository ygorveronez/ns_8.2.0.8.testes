namespace Servicos.Embarcador.Hubs
{
	public class ControleSaldo : HubBase<ControleSaldo>
	{
		public void SolicitarAtualizacaoSaldo(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
		{
			Servicos.Embarcador.Credito.ControleSaldo servicoControleSaldo = new Servicos.Embarcador.Credito.ControleSaldo(unitOfWork);

			var retorno = servicoControleSaldo.BuscarInformacoesSaldoCredito(usuario, unitOfWork);

			SendToClient(usuario.Login, "AtualizarSaldo", retorno);
		}
	}
}
