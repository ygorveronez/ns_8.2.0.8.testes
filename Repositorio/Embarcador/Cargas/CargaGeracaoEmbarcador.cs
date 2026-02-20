using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaGeracaoEmbarcador : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaGeracaoEmbarcador>
    {
        public CargaGeracaoEmbarcador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos 

        public List<Dominio.Entidades.Embarcador.Cargas.CargaGeracaoEmbarcador> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaGeracaoEmbarcador filtros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaGeracaoEmbarcador> query = ObterQueryConsulta(filtros);

            return query.Fetch(o => o.Carga).ThenFetch(o => o.DadosSumarizados)
                        .OrderBy(parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar)
                        .Skip(parametrosConsulta.InicioRegistros)
                        .Take(parametrosConsulta.LimiteRegistros)
                        .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaGeracaoEmbarcador filtros)
        {
            return ObterQueryConsulta(filtros).Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaGeracaoEmbarcador> ObterQueryConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaGeracaoEmbarcador filtro)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaGeracaoEmbarcador> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaGeracaoEmbarcador>();

            if (filtro.CodigoMotorista > 0)
                query = query.Where(o => o.Motoristas.Any(m => m.Codigo == filtro.CodigoMotorista));

            if (filtro.CodigoVeiculo > 0)
                query = query.Where(o => o.Tracao.Codigo == filtro.CodigoVeiculo || o.Reboques.Any(r => r.Codigo == filtro.CodigoVeiculo));

            if (!string.IsNullOrWhiteSpace(filtro.NumeroCarga))
                query = query.Where(o => o.Carga.CodigoCargaEmbarcador == filtro.NumeroCarga);

            if (filtro.NumeroCTe > 0)
                query = query.Where(o => o.CTes.Any(c => c.Numero == filtro.NumeroCTe));

            if (filtro.NumeroMDFe > 0)
                query = query.Where(o => o.MDFes.Any(c => c.Numero == filtro.NumeroMDFe));

            return query;
        }

        #endregion
    }
}
