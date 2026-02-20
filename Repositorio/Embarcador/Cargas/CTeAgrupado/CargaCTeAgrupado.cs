using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.CTeAgrupado
{
    public class CargaCTeAgrupado : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado>
    {
        public CargaCTeAgrupado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado> Consultar(int codigoCarga, int codigoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado? situacao, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado> query = ObterConsulta(codigoCarga, codigoCTe, situacao, propriedadeOrdenar, dirOrdena, inicio, limite);

            return query.ToList();
        }

        public int ContarConsulta(int codigoCarga, int codigoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado? situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado> query = ObterConsulta(codigoCarga, codigoCTe, situacao);

            return query.Count();
        }

        public List<int> BuscarCodigosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado situacao, int quantidadeRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado>();

            query = query.Where(o => o.Situacao == situacao);

            return query.Select(o => o.Codigo).Take(quantidadeRegistros).ToList();
        }
        
        public int BuscarProximoNumero()
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado>();

            return (query.Max(o => (int?)o.Numero) ?? 0) + 1;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado> BuscarCargaCTeAgrupadoAgIntegracao(bool gerandoIntegracoes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado.AgIntegracao && obj.GerandoIntegracoes == gerandoIntegracoes select obj;

            return result.ToList();
        }

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado> ObterConsulta(int codigoCarga, int codigoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado? situacao, string propriedadeOrdenar = "", string dirOrdena = "", int inicio = 0, int limite = 0)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado>();

            if (codigoCarga > 0)
                query = query.Where(o => o.Cargas.Any(c => c.Carga.Codigo == codigoCarga));

            if (codigoCTe > 0)
                query = query.Where(o => o.CTes.Any(c => c.CTe.Codigo == codigoCTe));

            if (situacao.HasValue)
                query = query.Where(o => o.Situacao == situacao.Value);

            if (!string.IsNullOrWhiteSpace(propriedadeOrdenar) && !string.IsNullOrWhiteSpace(dirOrdena))
                query = query.OrderBy(propriedadeOrdenar + " " + dirOrdena);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query;
        }

        #endregion
    }
}
