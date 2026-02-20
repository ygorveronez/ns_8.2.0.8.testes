using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ExtratoValePedagio
{
    public class ExtratoCreditoValePedagioIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratoCreditoValePedagioIntegracao>
	{

		#region Construtores
		
		public ExtratoCreditoValePedagioIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

		#endregion

		#region Métodos Públicos
		
		public Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratoCreditoValePedagioIntegracao BuscarExtratoExistente(long numeroViagem, DateTime? dataCompra, DateTime? dataOperacao, string acao, string nomePraca, string placa)
		{
			var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratoCreditoValePedagioIntegracao>()
				.Where(o => o.NumeroViagem == numeroViagem && o.DataCompra == dataCompra && o.DataOperacao == dataOperacao && o.Acao == acao && o.NomePraca == nomePraca && o.Placa == placa);

			return query.FirstOrDefault();
		}

		public List<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratoCreditoValePedagioIntegracao> BuscarExtratosPendentesProcessamento()
		{
			var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratoCreditoValePedagioIntegracao>()
				.Where(o => o.SituacaoProcessamentoExtratoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoExtratoValePedagio.AguardandoProcessamento);

			return query.ToList();
		}

		#endregion

	}
}
