using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Avarias
{
    public class ProdutosAvariados : RepositorioBase<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>
    {
        public ProdutosAvariados(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> BuscarPorAvarias(List<int> solicitacoes)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>();
            var result = from obj in query
                         where solicitacoes.Contains(obj.SolicitacaoAvaria.Codigo)
                         select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados BuscarPorSolicitacaoEProduto(int codSolicitacao, int codProduto)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>();
            var result = from obj in query
                         where obj.SolicitacaoAvaria.Codigo == codSolicitacao && obj.Codigo == codProduto
                         select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> BuscarPorSolicitacaoENota(int codSolicitacao, int nota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>();
            var result = from obj in query
                         where obj.SolicitacaoAvaria.Codigo == codSolicitacao && obj.ProdutoNotaFiscal.XMLNotaFiscal.Codigo == nota
                         select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados BuscarPorSolicitacaoEProdutoEmbarcador(int codSolicitacao, int codProduto)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>();
            var result = from obj in query
                         where obj.SolicitacaoAvaria.Codigo == codSolicitacao && obj.ProdutoEmbarcador.Codigo == codProduto
                         select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados BuscarPorSolicitacaoPorNumeroNotaECarga(int carga, string numeroNota)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>();
            var result = from obj in query
                         where obj.SolicitacaoAvaria.Carga.Codigo == carga && obj.NotaFiscal == numeroNota
                         && obj.SolicitacaoAvaria.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.Cancelada
                         && obj.SolicitacaoAvaria.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.RejeitadaAutorizacao
                         select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> BuscarProdutosSolicitacao(int codSolicitacao)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>();
            var result = from obj in query
                         where obj.SolicitacaoAvaria.Codigo == codSolicitacao
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> BuscarPorProdutoDeLote(int codLote, int codProduto)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>();
            var result = from obj in query
                         where obj.SolicitacaoAvaria.Lote.Codigo == codLote && obj.ProdutoEmbarcador.Codigo == codProduto
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> BuscarProdutosAvariadosDeLote(int codigoLote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>();

            var result = from obj in query where obj.SolicitacaoAvaria.Lote.Codigo == codigoLote select obj;

            return result.ToList();
        }

        public bool ProdutoAvariadoExisteNoLote(int codigoLote, int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>();

            var result = from obj in query where obj.SolicitacaoAvaria.Lote.Codigo == codigoLote && obj.ProdutoEmbarcador.Codigo == codigoProduto select obj;

            return result.Any();
        }

        public int QuantidadeProdutoAvariadoNoLote(int codigoLote, int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>();

            var result = from obj in query where obj.SolicitacaoAvaria.Lote.Codigo == codigoLote && obj.ProdutoEmbarcador.Codigo == codigoProduto select obj;

            return result.Sum(o => o.UnidadesAvariadas);
        }

        public List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> BuscarPorSolicitacao(int codSolicitacao, string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _BuscarPorSolicitacao(codSolicitacao, descricao);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);
            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.ProdutoEmbarcador)
                .ToList();
        }

        public int ContarBuscaPorSolicitacao(int codSolicitacao, string descricao)
        {
            var result = _BuscarPorSolicitacao(codSolicitacao, descricao);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        IQueryable<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> _BuscarPorSolicitacao(int codSolicitacao, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>();
            var result = from obj in query select obj;

            result = result.Where(o => o.SolicitacaoAvaria.Codigo == codSolicitacao);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.ProdutoEmbarcador.Descricao.Contains(descricao));

            return result;
        }

        #endregion
    }
}
