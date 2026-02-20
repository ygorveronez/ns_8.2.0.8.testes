using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.CTe
{
    public class DownloadLoteCTe : RepositorioBase<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTe>
    {
        #region Métodos Públicos

        public DownloadLoteCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaLoteCTe filtrosPesquisa)
        {
            return Consulta(filtrosPesquisa)
                .Count();
        }
        
        public List<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTe> Consultar(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaLoteCTe filtrosPesquisa, string indiceColunaOrdena, string gridOrdena, int inicio, int limite)
        {
            return ObterLista(Consulta(filtrosPesquisa), indiceColunaOrdena, gridOrdena, inicio, limite);
        }

        public Dominio.Entidades.Embarcador.CTe.DownloadLoteCTe BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTe>();
            query = query.Where(o => o.Codigo == codigo);
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CTe.DownloadLoteCTe BuscarPendente()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTe>();
            query = query.Where(o => o.Situacao == SituacaoDownloadLoteCTe.Pendente);
            return query.FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTe> Consulta(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaLoteCTe filtrosPesquisa)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTe>();

            if ((int)filtrosPesquisa.Situacao > 0)
                query = query.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.DataSolicitacao > DateTime.MinValue)
                query = query.Where(o => o.DataSolicitacao >= filtrosPesquisa.DataSolicitacao);

            if (filtrosPesquisa.DataTermino > DateTime.MinValue)
                query = query.Where(o => o.DataSolicitacao <= filtrosPesquisa.DataTermino);

            return query;
        }

        #endregion
    }
}
