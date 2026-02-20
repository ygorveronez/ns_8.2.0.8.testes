using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Compras
{
    public class FluxoCompra : RepositorioBase<Dominio.Entidades.Embarcador.Compras.FluxoCompra>
    {
        public FluxoCompra(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Compras.FluxoCompra BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.FluxoCompra>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Compras.FluxoCompra> BuscarPorOrdemCompra(int codigoOrdemCompra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.FluxoCompra>();
            var resut = from obj in query where obj.OrdensCompra.Where(x => x.Codigo == codigoOrdemCompra).Count() > 0 select obj;
            return resut.ToList();
        }
        public int BuscarProximoNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.FluxoCompra>();

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            int? ultimoNumero = query.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        public List<Dominio.Entidades.Embarcador.Compras.FluxoCompra> Consultar(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaFluxoCompra filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Compras.FluxoCompra> result = Consultar(filtrosPesquisa);

            result = result.Fetch(o => o.Usuario);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaFluxoCompra filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Compras.FluxoCompra> result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> ConsultarRequisicaoMercadoria(int codigoFluxoCompra, int numeroInicial, int numeroFinal, int codigoMotivo, int codigoProduto, int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> result = ConsultarRequisicaoMercadoria(codigoFluxoCompra, numeroInicial, numeroFinal, codigoMotivo, codigoProduto, codigoEmpresa);

            result = result
                .Fetch(o => o.MotivoCompra)
                .Fetch(o => o.Filial)
                .Fetch(o => o.Usuario);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsultaRequisicaoMercadoria(int codigoFluxoCompra, int numeroInicial, int numeroFinal, int codigoMotivo, int codigoProduto, int codigoEmpresa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> result = ConsultarRequisicaoMercadoria(codigoFluxoCompra, numeroInicial, numeroFinal, codigoMotivo, codigoProduto, codigoEmpresa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> ObterRequisicoesMercadoria(bool selecionarTodos, List<int> codigosRequisicoes, int numeroInicial, int numeroFinal, int codigoMotivo, int codigoProduto, int codigoEmpresa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> result = ConsultarRequisicaoMercadoria(0, numeroInicial, numeroFinal, codigoMotivo, codigoProduto, codigoEmpresa);

            if (selecionarTodos)
                result = result.Where(o => !codigosRequisicoes.Contains(o.Codigo));
            else
                result = result.Where(o => codigosRequisicoes.Contains(o.Codigo));

            return result.ToList();
        }

        public bool FluxoCompraDaOrdemFinalizado(int codigoOrdemCompra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.FluxoCompra>();

            var resut = from obj in query where obj.Situacao == SituacaoFluxoCompra.Finalizado && obj.OrdensCompra.Any(o => o.Codigo == codigoOrdemCompra) select obj;

            return resut.Any();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Compras.FluxoCompra> Consultar(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaFluxoCompra filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.FluxoCompra>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date >= filtrosPesquisa.DataInicial);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date <= filtrosPesquisa.DataFinal);

            if (filtrosPesquisa.NumeroInicial > 0 && filtrosPesquisa.NumeroFinal > 0)
                result = result.Where(obj => obj.Numero >= filtrosPesquisa.NumeroInicial && obj.Numero <= filtrosPesquisa.NumeroFinal);
            else if (filtrosPesquisa.NumeroInicial > 0)
                result = result.Where(obj => obj.Numero == filtrosPesquisa.NumeroInicial);
            else if (filtrosPesquisa.NumeroFinal > 0)
                result = result.Where(obj => obj.Numero == filtrosPesquisa.NumeroFinal);

            if (filtrosPesquisa.CodigoOrdemCompra > 0)
                result = result.Where(obj => obj.OrdensCompra.Any(o => o.Codigo == filtrosPesquisa.CodigoOrdemCompra));

            if (filtrosPesquisa.CodigoCotacao > 0)
                result = result.Where(obj => obj.Cotacao.Codigo == filtrosPesquisa.CodigoCotacao);

            if (filtrosPesquisa.CodigoRequisicaoMercadoria > 0)
                result = result.Where(obj => obj.RequisicoesMercadoria.Any(o => o.Codigo == filtrosPesquisa.CodigoRequisicaoMercadoria));

            if (filtrosPesquisa.CodigoUsuario > 0)
                result = result.Where(obj => obj.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.SituacaoTratativa != SituacaoTratativaFluxoCompra.Todos)
                result = result.Where(obj => obj.OrdensCompra.Any(o => o.SituacaoTratativa == filtrosPesquisa.SituacaoTratativa));

            if (filtrosPesquisa.Situacao != SituacaoFluxoCompra.Todos)
                result = result.Where(obj => obj.Situacao == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.EtapaAtual != EtapaFluxoCompra.Todos)
                result = result.Where(obj => obj.EtapaAtual == filtrosPesquisa.EtapaAtual);

            if (filtrosPesquisa.Produto > 0)
                result = result.Where(o => o.OrdensCompra.Any(m => m.Mercadorias.Any(p => p.Produto.Codigo == filtrosPesquisa.Produto)));

            if (filtrosPesquisa.Fornecedor > 0)
                result = result.Where(o => o.OrdensCompra.Any(m => m.Fornecedor.CPF_CNPJ == filtrosPesquisa.Fornecedor));

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> ConsultarRequisicaoMercadoria(int codigoFluxoCompra, int numeroInicial, int numeroFinal, int codigoMotivo, int codigoProduto, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria>();

            var result = from obj in query select obj;

            if (numeroInicial > 0)
                result = result.Where(obj => obj.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(obj => obj.Numero <= numeroFinal);

            if (codigoMotivo > 0)
                result = result.Where(obj => obj.MotivoCompra.Codigo == codigoMotivo);

            if (codigoProduto > 0)
                result = result.Where(obj => obj.Mercadorias.Any(o => o.ProdutoEstoque.Produto.Codigo == codigoProduto));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Filial.Codigo == codigoEmpresa);

            var queryFluxoCompra = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.FluxoCompra>();
            if (codigoFluxoCompra > 0)
            {
                var resultQueryFluxoCompra = from obj in queryFluxoCompra where obj.Codigo == codigoFluxoCompra select obj;

                result = result.Where(o => resultQueryFluxoCompra.Where(f => f.RequisicoesMercadoria.Any(r => r.Codigo == o.Codigo)).Any());
            }
            else
            {
                var resultQueryFluxoCompra = from obj in queryFluxoCompra select obj;

                result = result.Where(o => (!resultQueryFluxoCompra.Where(f => f.RequisicoesMercadoria.Any(r => r.Codigo == o.Codigo)).Any() && o.Situacao == SituacaoRequisicaoMercadoria.AgAprovacao)
                && !resultQueryFluxoCompra.Where(f => f.RequisicoesMercadoria.Any(r => r.Codigo == o.Codigo) && f.Situacao != SituacaoFluxoCompra.Cancelado).Any());
            }

            return result;
        }

        #endregion
    }
}
