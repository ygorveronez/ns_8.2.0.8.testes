using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Produtos
{
    public class GrupoProdutoTMS : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS>
    {
        public GrupoProdutoTMS(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS BuscarGrupoPadrao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS>();
            var result = from obj in query where obj.Ativo select obj;
            return result.FirstOrDefault();
        }


        public Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS>();
            var result = from obj in query where obj.Descricao == descricao select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS BuscarPorGrupoPai(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS>();
            var result = from obj in query where obj.GrupoProdutoTMSPai.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<string> BuscarDescricaoPorCodigo(List<int> codigosGruposProdutos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS>();

            var result = from obj in query where codigosGruposProdutos.Contains(obj.Codigo) select obj;

            return result.Select(o => o.Descricao).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS> Consultar(string descricao, int codigoEmpresa, int codigoGrupoPai, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (codigoGrupoPai > 0)
                result = result.Where(obj => obj.GrupoProdutoTMSPai.Codigo == codigoGrupoPai);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);


            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(string descricao, int codigoEmpresa, int codigoGrupoPai, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (codigoGrupoPai > 0)
                result = result.Where(obj => obj.GrupoProdutoTMSPai.Codigo == codigoGrupoPai);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.Count();
        }

    }
}
