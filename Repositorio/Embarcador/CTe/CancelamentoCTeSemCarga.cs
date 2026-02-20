using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.CTe
{
    public class CancelamentoCTeSemCarga : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CancelamentoCTeSemCarga>
    {
        public CancelamentoCTeSemCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CancelamentoCTeSemCarga(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Publicos
        public Dominio.Entidades.Embarcador.CTe.CancelamentoCTeSemCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CancelamentoCTeSemCarga>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }


        public List<int> BuscarCteSemCargaPorStatus(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCTeSemCarga situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CancelamentoCTeSemCarga>();

            var result = from obj in query where obj.Status == situacao select obj.Codigo;

            return result.ToList();
        }


        public List<Dominio.Entidades.Embarcador.CTe.CancelamentoCTeSemCarga> Consultar (Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCancelamentoCTeSemCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var query = MontarConsulta(filtrosPesquisa);

            return ObterLista(query, parametroConsulta);
        }
        #endregion

        #region Métodos Privados
        private IQueryable<Dominio.Entidades.Embarcador.CTe.CancelamentoCTeSemCarga> MontarConsulta(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCancelamentoCTeSemCarga filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CancelamentoCTeSemCarga>();
            var queryCancelamentoCtes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CancelamentoCTe>();

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataInclusao >= filtrosPesquisa.DataInicial);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataInclusao <= filtrosPesquisa.DataFinal);

            if (filtrosPesquisa.NumeroInicial > 0)
                queryCancelamentoCtes = queryCancelamentoCtes.Where(c => c.CTe.Numero >= filtrosPesquisa.NumeroInicial);

            if (filtrosPesquisa.NumeroFinal > 0)
                queryCancelamentoCtes = queryCancelamentoCtes.Where(c => c.CTe.Numero <= filtrosPesquisa.NumeroFinal);

            if(filtrosPesquisa.NumeroInicial > 0 || filtrosPesquisa.NumeroInicial > 0)
                query = query.Where(c => queryCancelamentoCtes.Where(o => o.CancelamentoCTeSemCarga.Codigo == c.Codigo).Any());

            return query;
        }
        #endregion 
    }
}
