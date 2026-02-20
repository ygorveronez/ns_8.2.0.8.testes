using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.ICMS
{
    public class PautaFiscal : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.ICMS.PautaFiscal>
    {
        #region Construtores

        public PautaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Publicos

        public Dominio.Entidades.Embarcador.ICMS.PautaFiscal BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.PautaFiscal>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.PautaFiscal> Consultar(string siglaEstado, string tarifa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = CriarConsulta(siglaEstado, tarifa, ativo);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string siglaEstado, string tarifa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = CriarConsulta(siglaEstado, tarifa, ativo);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.ICMS.PautaFiscal BuscarPorEstadoTipoDeCargaDistancia(string siglaEstado, int codigoTipoCarga, decimal distancia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.PautaFiscalTipoCarga>();
            var result = from obj in query
                         where obj.PautaFiscal.Estado.Sigla == siglaEstado &&
                               obj.TipoCarga.Codigo == codigoTipoCarga &&
                               obj.PautaFiscal.DistanciaKMInicial <= distancia &&
                               obj.PautaFiscal.DistanciaKMFinal >= distancia &&
                               ((bool?)obj.PautaFiscal.Ativo ?? true) == true
                         select obj.PautaFiscal;
            return result.FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.ICMS.PautaFiscal> CriarConsulta(string siglaEstado, string tarifa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var result = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.PautaFiscal>();

            if (!string.IsNullOrWhiteSpace(siglaEstado))
                result = result.Where(obj => obj.Estado.Sigla.Equals(siglaEstado));

            if (!string.IsNullOrWhiteSpace(tarifa))
                result = result.Where(obj => obj.Tarifa.Equals(tarifa));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => ((bool?)o.Ativo ?? true) == true);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Ativo);

            return result;
        }

        #endregion
    }
}
