using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaResponsavel : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaResponsavel>
    {
        #region Construtores

        public CargaResponsavel(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaResponsavel> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaResponsavel filtrosPesquisa)
        {
            var consultaCargaResponsavel = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaResponsavel>();

            if (filtrosPesquisa.CodigoCategoriaResponsavel > 0)
                consultaCargaResponsavel = consultaCargaResponsavel.Where(o => o.CategoriaResponsavel.Codigo == filtrosPesquisa.CodigoCategoriaResponsavel);

            if (filtrosPesquisa.CodigoFuncionario > 0)
                consultaCargaResponsavel = consultaCargaResponsavel.Where(o => o.Funcionario.Codigo == filtrosPesquisa.CodigoFuncionario);

            if (filtrosPesquisa.DataVigenciaInicial.HasValue)
                consultaCargaResponsavel = consultaCargaResponsavel.Where(o => o.VigenciaInicio >= filtrosPesquisa.DataVigenciaInicial.Value.Date);

            if (filtrosPesquisa.DataVigenciaFinal.HasValue)
                consultaCargaResponsavel = consultaCargaResponsavel.Where(o => o.VigenciaFinal <= filtrosPesquisa.DataVigenciaFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            return consultaCargaResponsavel;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaResponsavel> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaResponsavel filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaResponsavel = Consultar(filtrosPesquisa);

            return ObterLista(consultaCargaResponsavel, parametrosConsulta);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaResponsavel> BuscarTodosResponsaveis(DateTime vigencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaResponsavel>();

            query = query.Where(o => !o.VigenciaInicio.HasValue || o.VigenciaInicio <= vigencia.Date);

            query = query.Where(o => !o.VigenciaFinal.HasValue || o.VigenciaFinal >= vigencia.Date);

            var result = from obj in query select obj;

            return result.ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaResponsavel filtrosPesquisa)
        {
            var consultaCargaResponsavel = Consultar(filtrosPesquisa);

            return consultaCargaResponsavel.Count();
        }

        #endregion
    }
}
