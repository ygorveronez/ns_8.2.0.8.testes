using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Repositorio.Embarcador.Devolucao
{
	public class GestaoDevolucaoLaudoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto>
	{
		public GestaoDevolucaoLaudoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

		public List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto> BuscarPorGestaoDevolucaoLaudo(long codigo)
		{
			var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto>();
			query = query.Where(o => o.Laudo.Codigo == codigo);
			return query.ToList();
		}

		public Dominio.ObjetosDeValor.Embarcador.GestaoPallet.QuantidadeLaudoPallet BuscarQuantidadesPorProdutoELaudo(long codigoLaudo, int codigoProduto)
		{
			var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto>();

			query = query.Where(o => o.Laudo.Codigo == codigoLaudo && o.Produto.Codigo == codigoProduto);

			return query.Select(obj => new Dominio.ObjetosDeValor.Embarcador.GestaoPallet.QuantidadeLaudoPallet()
			{
				QuantidadeDescarte = obj.QuantidadeDescarte,
				QuantidadeDisponivel = obj.QuantidadeDevolvida,
				QuantidadeManutencao = obj.QuantidadeManutencao,
			}).FirstOrDefault();
		}
	}
}
