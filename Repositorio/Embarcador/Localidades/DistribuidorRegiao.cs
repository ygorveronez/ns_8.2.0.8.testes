using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Localidades
{
    public class DistribuidorRegiao : RepositorioBase<Dominio.Entidades.Embarcador.Localidades.DistribuidorRegiao>
    {
        #region Constructores
        public DistribuidorRegiao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos
        public Dominio.Entidades.Embarcador.Localidades.DistribuidorRegiao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.DistribuidorRegiao>();
            query = query.Where(m => m.Codigo == codigo);
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Cliente BuscarDistribuidorAtivoPorTipoCargaERegiao(int codigoTipoCarga, int codigoRegiao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Localidades.DistribuidorRegiao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.DistribuidorRegiao>();
            query = query.Where(d => d.TiposDeCargas.Any(t => t.Codigo == codigoTipoCarga) &&
            d.Regiao.Codigo == codigoRegiao &&
            d.Situacao == true);

            return query.Select(d => d.ClienteDistribuidor).FirstOrDefault();
        }

        public bool ExisteRegistroDuplicado(Dominio.Entidades.Embarcador.Localidades.DistribuidorRegiao distribuidorPorRegiao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.DistribuidorRegiao>();

            if (distribuidorPorRegiao.Regiao != null)
                query = query.Where(r => r.Regiao.Codigo == distribuidorPorRegiao.Regiao.Codigo &&
                r.ClienteDistribuidor.CPF_CNPJ == distribuidorPorRegiao.ClienteDistribuidor.CPF_CNPJ &&
                r.TiposDeCargas.Select(t => t.Codigo).Contains(distribuidorPorRegiao.TiposDeCargas.FirstOrDefault().Codigo));

            var result = query.FirstOrDefault();
            return result == null ? false : true;
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Localidade.FiltroPesquisaDistribuidorRegiao filtrosPesquisa)
        {
            var consulta = Consultar(filtrosPesquisa);

            return consulta.Count();
        }

        public List<Dominio.Entidades.Embarcador.Localidades.DistribuidorRegiao> Consultar(Dominio.ObjetosDeValor.Embarcador.Localidade.FiltroPesquisaDistribuidorRegiao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var distribuidoresRegiao = Consultar(filtrosPesquisa);

            return ObterLista(distribuidoresRegiao, parametrosConsulta);
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Localidades.DistribuidorRegiao> Consultar(Dominio.ObjetosDeValor.Embarcador.Localidade.FiltroPesquisaDistribuidorRegiao filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.DistribuidorRegiao>();

            if (filtrosPesquisa.CodigoRegiao > 0)
                query = query.Where(o => o.Regiao.Codigo == filtrosPesquisa.CodigoRegiao);

            if (filtrosPesquisa.CodigoDistribuidor > 0)
                query = query.Where(o => o.Distribuidor.Codigo == filtrosPesquisa.CodigoDistribuidor);

            return query;
        }

        #endregion
    }
}
