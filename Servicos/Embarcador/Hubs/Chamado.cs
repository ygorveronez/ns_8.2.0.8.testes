namespace Servicos.Embarcador.Hubs
{
	public class Chamado : HubBase<Chamado>
	{
		public void NotificarTodosChamadoAdicionadoOuAtualizado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado)
		{
			SendToAll("informarChamadoAdicionadoOuAlterado", new
			{
				CodigoFilial = chamado.Carga?.Filial?.Codigo ?? 0,
				CodigoResponsavel = chamado.Responsavel?.Codigo ?? 0,
				CodigoTransportador = chamado.Carga?.Empresa?.Codigo ?? chamado.Empresa?.Codigo ?? 0,
				DataCriacaoChamado = chamado.DataCriacao.ToString("dd/MM/yyyy"),
				NumeroChamado = chamado.Numero,
				SituacaoChamado = chamado.Situacao
			});
		}

        public void NotificarTempoExcedidoChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado)
        {
            SendToAll("escalarTempoExcedidoChamado", new
            {
                chamado.Codigo
            });
        }

        public void NotificarChamadoCancelado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado)
        {
            SendToAll("informarChamadoCancelado", new
            {
                CodigoChamado = chamado.Codigo
            });
        }

    }
}
