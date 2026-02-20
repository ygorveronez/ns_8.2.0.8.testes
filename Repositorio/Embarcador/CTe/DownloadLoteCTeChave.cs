using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.CTe
{
    public class DownloadLoteCTeChave : RepositorioBase<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTeChave>
    {
        public DownloadLoteCTeChave(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public int ContarConsulta(int codigo)
        {
            return Consulta(codigo)
                .Count();
        }

        public List<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTeChave> Consultar(int codigo)
        {
            return Consulta(codigo)
                .ToList();
        }


        public Dominio.Entidades.Embarcador.CTe.DownloadLoteCTeChave BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTeChave>();
            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTeChave> BuscarPendentesPorLote(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTeChave>();
            query = query.Where(o => o.Situacao == SituacaoDownloadLoteCTe.Pendente && o.DownloadLoteCTe.Codigo == codigo);
            query = query.Take(100);

            return query
                .ToList();
        }

        public List<int> BuscarCodigosPendentesPorLote(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTeChave>();
            query = query.Where(o => o.Situacao == SituacaoDownloadLoteCTe.Pendente && o.DownloadLoteCTe.Codigo == codigo);

            return query.Select(obj => obj.Codigo).ToList();
        }

        public int ContarTodosNaoPendentes(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTeChave>();
            query = query.Where(o => o.Situacao != SituacaoDownloadLoteCTe.Pendente && o.DownloadLoteCTe.Codigo == codigo);

            return query
                .Count();
        }
        
        public int ContarFalhas(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTeChave>();
            query = query.Where(o => o.Situacao == SituacaoDownloadLoteCTe.Falha && o.DownloadLoteCTe.Codigo == codigo);

            return query
                .Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTeChave> Consulta(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.DownloadLoteCTeChave>();

            query = query.Where(o => o.DownloadLoteCTe.Codigo == codigo);

            return query;
        }

        #endregion
    }
}
