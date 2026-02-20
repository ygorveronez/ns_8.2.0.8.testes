using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Produtos
{
    public class UnidadeMedidaFornecedor : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor>
    {
        public UnidadeMedidaFornecedor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor BuscarPorDescricao(string descricaoFornecedor, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor>();
            var result = from obj in query where obj.DescricaoFornecedor == descricaoFornecedor select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor> Consultar(int codigoEmpresa, string descricaoFornecedor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida UnidadeDeMedida, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricaoFornecedor))
                result = result.Where(obj => obj.DescricaoFornecedor.Contains(descricaoFornecedor));

            if ((int)UnidadeDeMedida > 0)
                result = result.Where(obj => obj.UnidadeDeMedida == UnidadeDeMedida);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string descricaoFornecedor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida UnidadeDeMedida)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricaoFornecedor))
                result = result.Where(obj => obj.DescricaoFornecedor.Contains(descricaoFornecedor));

            if ((int)UnidadeDeMedida > 0)
                result = result.Where(obj => obj.UnidadeDeMedida == UnidadeDeMedida);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.Count();
        }

    }
}
