using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Seguros
{
    public class ApoliceSeguroDesconto : RepositorioBase<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto>
    {
        public ApoliceSeguroDesconto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();

        }

        private IQueryable<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto> Consultar(string descricao)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(descricao));


            return consulta;
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(descricao);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(string descricao)
        {
            var consulta = Consultar(descricao);

            return consulta.Count();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto> BuscaPorCodigoApoliceSeguro(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.ApoliceSeguro.Codigo == codigo);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto> BuscarDescontosPorApolices(List<int> codigosApoliceSeguro, int codigoModeloVeicular, int codigoFilial, int codigoTipoOperacao)
        {
            var consultaApoliceSeguroDesconto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto>()
                .Where(o => codigosApoliceSeguro.Contains(o.ApoliceSeguro.Codigo));

            if (codigoModeloVeicular > 0)
                consultaApoliceSeguroDesconto = consultaApoliceSeguroDesconto.Where(o => o.ModeloVeicularCarga.Codigo == codigoModeloVeicular);
            else
                consultaApoliceSeguroDesconto = consultaApoliceSeguroDesconto.Where(o => o.ModeloVeicularCarga == null);

            if (codigoFilial > 0)
                consultaApoliceSeguroDesconto = consultaApoliceSeguroDesconto.Where(o => o.Filial.Codigo == codigoFilial);
            else
                consultaApoliceSeguroDesconto = consultaApoliceSeguroDesconto.Where(o => o.Filial == null);

            if (codigoTipoOperacao > 0)
                consultaApoliceSeguroDesconto = consultaApoliceSeguroDesconto.Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao);
            else
                consultaApoliceSeguroDesconto = consultaApoliceSeguroDesconto.Where(o => o.TipoOperacao == null);

            return consultaApoliceSeguroDesconto.ToList();
        }

        public bool ExisteDescontoSeguro(string modeloVeicular, string filial, string tipoOperacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto>()
                .Where(d =>
                    d.ModeloVeicularCarga.CodigoIntegracao == modeloVeicular &&
                    d.Filial.CodigoFilialEmbarcador == filial &&
                    d.TipoOperacao.CodigoIntegracao == tipoOperacao
                );

            return query.Any();
        }
    }
}

