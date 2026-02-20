using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;


namespace Repositorio.Embarcador.Produtos
{
    public class GrupoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>
    {
        public GrupoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public GrupoProduto(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }


        public Dominio.Entidades.Embarcador.Produtos.GrupoProduto BuscarGrupoPadrao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>();
            var result = from obj in query where obj.Ativo select obj;
            return result.FirstOrDefault();
        }


        public Dominio.Entidades.Embarcador.Produtos.GrupoProduto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>> BuscarPorCodigosAsync(List<int> codigos)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>()
                .Where(x => codigos.Contains(x.Codigo)).ToListAsync();
        }

        public Dominio.Entidades.Embarcador.Produtos.GrupoProduto BuscarPorCodigoEmbarcador(string codigoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>();
            var result = from obj in query where obj.CodigoGrupoProdutoEmbarcador == codigoEmbarcador select obj;
            return result.FirstOrDefault();
        }

        public Task<List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>> BuscarPorCodigosEmbarcadorAsync(List<string> codigosEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>();
            var result = from obj in query where codigosEmbarcador.Contains(obj.CodigoGrupoProdutoEmbarcador) select obj;
            return result.ToListAsync(CancellationToken);
        }


        public List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> Consultar(string descricao, string codigoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, bool somenteChecklist, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoEmbarcador))
                result = result.Where(obj => obj.CodigoGrupoProdutoEmbarcador == codigoEmbarcador);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => !obj.Ativo);

            if (somenteChecklist)
                result = result.Where(obj => obj.RetornarNoChecklist);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(string descricao, string codigoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, bool somenteChecklist)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoEmbarcador))
                result = result.Where(obj => obj.CodigoGrupoProdutoEmbarcador == codigoEmbarcador);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => !obj.Ativo);

            if (somenteChecklist)
                result = result.Where(obj => obj.RetornarNoChecklist);

            return result.Count();
        }

    }
}
