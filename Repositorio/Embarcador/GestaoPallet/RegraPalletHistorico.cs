using System;
using System.Linq;

namespace Repositorio.Embarcador.GestaoPallet
{
	public class RegraPalletHistorico : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPallet.RegraPalletHistorico>
	{

		#region Construtores

		public RegraPalletHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }

		#endregion

		#region Métodos Públicos

		public Dominio.Entidades.Embarcador.GestaoPallet.RegraPalletHistorico BuscarRegraNoPeriodo(double codigoCliente, DateTime dataReferencia)
		{
			IQueryable<Dominio.Entidades.Embarcador.GestaoPallet.RegraPalletHistorico> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.RegraPalletHistorico>();

			query = query.Where(obj => obj.Cliente.CPF_CNPJ.Equals(codigoCliente) && (obj.DataInicial <= dataReferencia && (obj.DataFinal > dataReferencia || obj.DataFinal == null)));

			return query.FirstOrDefault();
		}

		public Dominio.Entidades.Embarcador.GestaoPallet.RegraPalletHistorico BuscarUltimaRegra(double codigoCliente)
		{
			IQueryable<Dominio.Entidades.Embarcador.GestaoPallet.RegraPalletHistorico> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.RegraPalletHistorico>();

			query = query.Where(obj => obj.Cliente.CPF_CNPJ.Equals(codigoCliente) && obj.DataFinal == null);

			return query.FirstOrDefault();
		}

		#endregion Métodos Públicos
	}
}
