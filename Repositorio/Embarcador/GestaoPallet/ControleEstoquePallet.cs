using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.GestaoPallet;
using System.Linq;

namespace Repositorio.Embarcador.GestaoPallet
{
	public class ControleEstoquePallet : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet>
	{

		#region Construtores

		public ControleEstoquePallet(UnitOfWork unitOfWork) : base(unitOfWork) { }

		#endregion

		#region Métodos Públicos

		public Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet BuscarEstoquePallet(DadosControlePallet dadosControlePallet)
		{
			IQueryable<Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet>();

			if (dadosControlePallet.ResponsavelPallet == ResponsavelPallet.Transportador)
				query = query.Where(obj => obj.Transportador.Codigo == dadosControlePallet.CodigoTransportador);

			if (dadosControlePallet.ResponsavelPallet == ResponsavelPallet.Cliente)
				query = query.Where(obj => obj.Cliente.CPF_CNPJ.Equals(dadosControlePallet.CodigoCliente));

			if (dadosControlePallet.ResponsavelPallet == ResponsavelPallet.Filial)
				query = query.Where(obj => obj.Filial.Codigo == dadosControlePallet.CodigoFilial);

			query = query.Where(obj => obj.TipoEstoquePallet == dadosControlePallet.TipoEstoquePallet);

			return query.FirstOrDefault();
		}

		public int ObterSaldoAtual(DadosControlePallet dadosControlePallet)
		{
			IQueryable<Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet>();

			if (dadosControlePallet.ResponsavelPallet == ResponsavelPallet.Transportador)
				query = query.Where(obj => obj.Transportador.Codigo == dadosControlePallet.CodigoTransportador);

			if (dadosControlePallet.ResponsavelPallet == ResponsavelPallet.Cliente)
				query = query.Where(obj => obj.Cliente.CPF_CNPJ.Equals(dadosControlePallet.CodigoCliente));

			if (dadosControlePallet.ResponsavelPallet == ResponsavelPallet.Filial)
				query = query.Where(obj => obj.Filial.Codigo == dadosControlePallet.CodigoFilial);

			query = query.Where(obj => obj.TipoEstoquePallet == dadosControlePallet.TipoEstoquePallet);

			return query.Select(o => o.QuantidadeTotalPallets).FirstOrDefault();
		}

		#endregion Métodos Públicos

		#region Métodos Privados


		#endregion Métodos Privados
	}
}
