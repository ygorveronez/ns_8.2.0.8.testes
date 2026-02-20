using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class TratativaOcorrenciaEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega>
    {
        public TratativaOcorrenciaEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega> BuscarPorTipoOcorrencia(int codigoTipoOcorrencia)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega>();
            var result = query.Where(obj => obj.TipoDeOcorrencia.Codigo == codigoTipoOcorrencia);
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega ValidarSeExiste(bool devolucaoParcial, int codigoTipoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega tratativa)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega>();
            var result = query.Where(obj => obj.DevolucaoParcial == devolucaoParcial && obj.TipoDeOcorrencia.Codigo == codigoTipoOcorrencia && obj.TratativaDevolucao == tratativa);
            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega> Consultar(int codigoTipoOcorrencia, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega> query = ObterQueryConsulta(codigoTipoOcorrencia);

            if (!string.IsNullOrWhiteSpace(propriedadeOrdenar) && !string.IsNullOrWhiteSpace(dirOrdena))
                query = query.OrderBy(propriedadeOrdenar + " " + dirOrdena);

            if (inicio > 0)
                query = query.Skip(inicio);

            if (limite > 0)
                query = query.Take(limite);

            return query.ToList();
        }

        public int ContarConsulta(int codigoTipoOcorrencia)
        {
            return ObterQueryConsulta(codigoTipoOcorrencia).Count();
        }

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega> ObterQueryConsulta(int codigoTipoOcorrencia)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega>();

            if (codigoTipoOcorrencia > 0)
                query = query.Where(o => o.TipoDeOcorrencia.Codigo == codigoTipoOcorrencia);

            return query;
        }

        #endregion
    }
}
