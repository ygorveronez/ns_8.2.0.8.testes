using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.WMS
{
    public class SeparacaoProdutoEmbarcador : RepositorioBase<Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador>
    {
        public SeparacaoProdutoEmbarcador(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote> BuscarPorProdutoECarga(int produto, int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador>();

            var result = from obj in query
                         where
                            obj.ProdutoEmbarcadorLote.ProdutoEmbarcador.Codigo == produto &&
                             obj.Separacao.SituacaoSelecaoSeparacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Finalizada &&
                             obj.Separacao.Selecao.Cargas.Any(c => c.Carga.Codigo == carga)
                         select obj.ProdutoEmbarcadorLote;

            return result.ToList();
        }

        public bool BuscarPorSeparacaoProdutoEPosicaoCodigoBarras(int separacao, int produto, int posicao, string codigoBarras)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador>();

            var result = from obj in query
                         where
                         obj.Separacao.Codigo == separacao &&
                         obj.ProdutoEmbarcadorLote.ProdutoEmbarcador.Codigo == produto &&
                         obj.ProdutoEmbarcadorLote.DepositoPosicao.Codigo == posicao &&
                         obj.CodigoBarrasConferido.Contains(codigoBarras)                         
                         select obj;

            return result.Any();
        }

        public Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador BuscarPorSeparacaoProdutoEPosicao(int separacao, int produto, int posicao, decimal quantidade, string codigoBarrasLocalizar)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador>();

            var result = from obj in query
                         where
                         obj.Separacao.Codigo == separacao &&
                         obj.ProdutoEmbarcadorLote.ProdutoEmbarcador.Codigo == produto &&
                         obj.ProdutoEmbarcadorLote.DepositoPosicao.Codigo == posicao &&
                         obj.Quantidade >= quantidade
                         select obj;

            if (!string.IsNullOrWhiteSpace(codigoBarrasLocalizar))
                result = result.Where(obj => obj.ProdutoEmbarcadorLote.CodigoBarras.Contains(codigoBarrasLocalizar));

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador BuscarPorCargaECodigoBarra(int carga, string produto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador>();

            var result = from obj in query
                         where
                         obj.Separacao.SituacaoSelecaoSeparacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Finalizada &&
                         obj.Separacao.Selecao.Cargas.Any(c => c.Carga.Codigo == carga) &&
                         //obj.ProdutoEmbarcadorLote.CodigoBarras.ToUpper() == produto.ToUpper()
                         obj.ProdutoEmbarcadorLote.CodigoBarras.ToUpper().Contains(produto.ToUpper())
                         select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador BuscarPorSeparacaoECodigoBarra(int separacao, string produto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador>();

            var result = from obj in query
                         where
                         obj.Separacao.Codigo == separacao &&
                         //produto.ToUpper().Contains(obj.ProdutoEmbarcadorLote.CodigoBarras.ToUpper())
                         obj.ProdutoEmbarcadorLote.CodigoBarras.ToUpper().Contains(produto.ToUpper())
                         select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador BuscarPorSeparacaoEPosicao(int separacao, string posicao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador>();

            var result = from obj in query
                         where
                         obj.Separacao.Codigo == separacao &&
                         obj.ProdutoEmbarcadorLote.DepositoPosicao.Abreviacao == posicao
                         select obj;

            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador> _Consultar(int separacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador>();

            var result = from obj in query select obj;

            // Filtros
            if (separacao > 0)
                result = result.Where(o => o.Separacao.Codigo == separacao);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador> Consultar(int separacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(separacao);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int separacao)
        {
            var result = _Consultar(separacao);

            return result.Count();
        }

        public bool ValidaCodigoBarrasPorCarga(string codigoBarras, int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador>();

            var result = from obj in query
                         where
                         obj.Separacao.SituacaoSelecaoSeparacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Finalizada &&
                         obj.Separacao.Selecao.Cargas.Any(c => c.Carga.Codigo == carga) &&
                         obj.ProdutoEmbarcadorLote.CodigoBarras.ToUpper() == codigoBarras.ToUpper()
                         select obj;

            return result.Count() > 0;
        }
    }
}