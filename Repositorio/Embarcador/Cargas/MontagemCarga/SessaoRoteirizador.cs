using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class SessaoRoteirizador : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador>
    {
        #region Construtores

        public SessaoRoteirizador(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public SessaoRoteirizador(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaSessaoRoteirizador filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador>();

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador.Todas)
                query = query.Where(x => x.SituacaoSessaoRoteirizador == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.CodigoFilial > 0)
                query = query.Where(x => x.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoUsuario > 0)
                query = query.Where(x => x.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.NumeroSessao > 0)
                query = query.Where(x => x.Codigo == filtrosPesquisa.NumeroSessao); ;

            return query;
        }

        #endregion

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador>();
            query = query.Where(p => p.Codigo == codigo);
            query = query.Fetch(x => x.Filial)
                         .Fetch(x => x.Usuario);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador> BuscarPorFilial(int cod_filial)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador>();
            query = query.Where(p => p.Filial.Codigo == cod_filial).OrderByDescending(x => x.DataFinal);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador> BuscarPorUsuario(int cod_usuario)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador>();
            query = query.Where(p => p.Usuario.Codigo == cod_usuario).OrderByDescending(x => x.DataFinal);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador> BuscarPor(int cod_filial, int cod_usuario)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador>();
            query = query.Where(p => p.Usuario.Codigo == cod_usuario && p.Filial.Codigo == cod_filial).OrderByDescending(x => x.DataFinal);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaSessaoRoteirizador filtrosPesquisa, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = Consultar(filtrosPesquisa);
            query = query.Fetch(obj => obj.Filial)
                         .Fetch(obj => obj.Usuario);

            return ObterLista(query, propriedadeOrdenacao, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaSessaoRoteirizador filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);
            return query.Count();
        }

        public int QtdeCarregamentos(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
            query = query.Where(x => x.SessaoRoteirizador.Codigo == codigo &&
                                     x.SituacaoCarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado);
            return query.Count();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> QtdeCarregamentos(List<int> codigosSessoes)
        {
            var sqlQuery = @"
SELECT CAR.SRO_CODIGO as Codigo
     , count(1) as Total
  FROM T_CARREGAMENTO CAR
 WHERE CAR.CRG_SITUACAO <> :situacao
   AND CAR.SRO_CODIGO in ( :codigos )
 GROUP BY CAR.SRO_CODIGO
 ORDER BY 1; ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetParameter("situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado);
            query.SetParameterList("codigos", codigosSessoes);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento)));

            return query.List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento>();
        }

        public int MaximaReservaNumeroCarregamentoMontagem()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador>();

            int? retorno = query.Max(o => (int?)o.ReservaNumeroCarregamentoMontagem);

            return retorno.HasValue ? retorno.Value : 0;
        }
    }
}
