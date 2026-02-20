using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ExtratoValePedagio
{
    public class ExtratosValePedagio : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio>
    {

        #region Construtores

        public ExtratosValePedagio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.ExtratoValePedagio.FiltroPesquisaExtratosValePedagio filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = Consultar(filtroPesquisa);

            query.Fetch(obj => obj.ValePedagio)
                 .ThenFetch(obj => obj.Carga);

            return
                ObterLista(query, parametrosConsulta);

        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.ExtratoValePedagio.FiltroPesquisaExtratosValePedagio filtroPesquisa)
        {
            var query = Consultar(filtroPesquisa);
            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio> BuscarPorValePedagio(int codigoValePedagio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio>()
                .Where(o => o.ValePedagio.Codigo == codigoValePedagio && o.SituacaoProcessamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoExtratoValePedagio.Processado);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio BuscarPorNumeroViagem(long numeroViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio>()
                .Where(o => o.NumeroViagem == numeroViagem);

            return query.FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.ExtratoValePedagio.FiltroPesquisaExtratosValePedagio filtroPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio>();
            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtroPesquisa.CodigoCargaEmbarcador))
                result = result.Where(o => o.ValePedagio.Carga.CodigoCargaEmbarcador == filtroPesquisa.CodigoCargaEmbarcador);

            if (filtroPesquisa.DataCargaInicial.HasValue)
                result = result.Where(o => o.ValePedagio.Carga.DataCriacaoCarga >= filtroPesquisa.DataCargaInicial);

            if (filtroPesquisa.DataCargaFinal.HasValue)
                result = result.Where(o => o.ValePedagio.Carga.DataCriacaoCarga <= filtroPesquisa.DataCargaFinal);

            if (filtroPesquisa.NumeroValePedagio > 0l)
                result = result.Where(o => o.NumeroViagem == filtroPesquisa.NumeroValePedagio);

            if (filtroPesquisa.SituacaoExtrato.HasValue)
                result = result.Where(o => o.SituacaoExtrato == filtroPesquisa.SituacaoExtrato);


            return result;
        }

        #endregion
    }
}
