using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaIntegracaoEvento : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento>
    {
        #region Atributos Privados Somente Leitura

        #endregion

        #region Construtores

        public CargaIntegracaoEvento(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }


        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento BuscarPorCargaEtapa(int codigoCarga, EtapaCarga etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Etapa == etapa select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento> BuscarListaIntegracaoPendente()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento>();

            var result = from obj in query
                         where ((!obj.Carga.CalculandoFrete) || (obj.Carga.CalculandoFrete && obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete )) &&
                         (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
                         || (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < 3))
                         select obj;

            return result.OrderBy(x => x.DataIntegracao).WithOptions(o => o.SetTimeout(120)).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaIntegracaoEvento filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento> consulta = Consultar(filtroPesquisa);
            return ObterLista(consulta, parametroConsulta);
        }

        public int ContarConsultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaIntegracaoEvento filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento> consulta = Consultar(filtroPesquisa);
            return consulta.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento> ListarPorCargaParaReenvio(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento>();

            var result = from obj in query
                         where obj.Carga.Codigo == codigoCarga && obj.Etapa == EtapaCarga.SalvarDadosTransporte && obj.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ArcelorMittal
                         select obj;

            return result.WithOptions(o => o.SetTimeout(120)).ToList();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaIntegracaoEvento filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento> consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento>();
            consulta = from obj in consulta select obj;

            if (!string.IsNullOrEmpty(filtroPesquisa.NumeroCarga))
                consulta = consulta.Where(obj => obj.Carga.CodigoCargaEmbarcador == filtroPesquisa.NumeroCarga);

            if (filtroPesquisa.SituacaoIntegracao.HasValue)
                consulta = consulta.Where(obj => obj.SituacaoIntegracao == filtroPesquisa.SituacaoIntegracao.Value);

            if (filtroPesquisa.DataIntegracaoInicial != DateTime.MinValue)
                consulta = consulta.Where(obj => obj.DataIntegracao >= filtroPesquisa.DataIntegracaoInicial);

            if (filtroPesquisa.DataIntegracaoFinal != DateTime.MinValue)
                consulta = consulta.Where(obj => obj.DataIntegracao <= filtroPesquisa.DataIntegracaoFinal);

            return consulta;
        }

        #endregion Métodos Públicos

    }
}
