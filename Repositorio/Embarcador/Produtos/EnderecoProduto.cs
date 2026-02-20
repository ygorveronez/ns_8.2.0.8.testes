using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Produtos
{
    public class EnderecoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.EnderecoProduto>
    {

        public EnderecoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Produtos.EnderecoProduto BuscarPorCodigoIntegracao(string codigoIntegracao, int filial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.EnderecoProduto>();

            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao && obj.Ativo select obj;

            if (filial > 0)
                result = result.Where(obj => obj.Filial.Codigo == filial);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Produtos.EnderecoProduto BuscarPorTodasCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.EnderecoProduto>();

            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Produtos.EnderecoProduto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.EnderecoProduto>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.EnderecoProduto> Consultar(string descricao, string codigoIntegracao, int codigoFilial, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.EnderecoProduto>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao == codigoIntegracao);

            if (codigoFilial > 0)
                result = result.Where(x => x.Filial.Codigo == codigoFilial);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (maximoRegistros > 0)
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
            else
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).ToList();

        }

        public int ContarConsulta(string descricao, string codigoIntegracao, int codigoFilial, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.EnderecoProduto>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao == codigoIntegracao);

            if (codigoFilial > 0)
                result = result.Where(x => x.Filial.Codigo == codigoFilial);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.Count();
        }
    }
}
