using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Produtos
{
    public class ProdutoNCMAbastecimento : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento>
    {
        public ProdutoNCMAbastecimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> BuscarNCMsAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento>();
            var result = from obj in query where obj.Ativo == true select obj;
            return result.ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> _Consultar(int codigoEmpresa, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.NCM.StartsWith(descricao));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo == true);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => o.Ativo == false);

            if (tipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla)
                result = result.Where(o => o.TipoAbastecimento == tipoAbastecimento);
            else if (tipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel)
                result = result.Where(o => o.TipoAbastecimento == tipoAbastecimento);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> Consultar(int codigoEmpresa, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoEmpresa, descricao, status, tipoAbastecimento);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento)
        {
            var result = _Consultar(codigoEmpresa, descricao, status, tipoAbastecimento);

            return result.Count();
        }
    }
}
