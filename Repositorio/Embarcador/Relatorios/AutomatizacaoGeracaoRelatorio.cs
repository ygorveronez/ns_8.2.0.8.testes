using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;

namespace Repositorio.Embarcador.Relatorios
{
    public class AutomatizacaoGeracaoRelatorio : RepositorioBase<Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio>
    {
        #region Construtores

        public AutomatizacaoGeracaoRelatorio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio> Consultar(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio)
        {
            var consultaAutomatizacaoGeracaoRelatorio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio>()
                .Where(o => o.Relatorio.Ativo && o.Relatorio.CodigoControleRelatorios == codigoControleRelatorio);

            return consultaAutomatizacaoGeracaoRelatorio;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<int> BuscarCodigosAutomatizacoesPendentesGeracao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio> consultaAutomatizacaoGeracaoRelatorio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio>()
                .Where(o => o.DataProximaGeracao <= DateTime.Now);

            return consultaAutomatizacaoGeracaoRelatorio
                .OrderBy(o => o.DataProximaGeracao)
                .Select(o => o.Codigo)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio> Consultar(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio> consultaAutomatizacaoGeracaoRelatorio = Consultar(codigoControleRelatorio);

            consultaAutomatizacaoGeracaoRelatorio = consultaAutomatizacaoGeracaoRelatorio.Fetch(o => o.Relatorio);

            return ObterLista(consultaAutomatizacaoGeracaoRelatorio, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio)
        {
            IQueryable<Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio> consultaAutomatizacaoGeracaoRelatorio = Consultar(codigoControleRelatorio);

            return consultaAutomatizacaoGeracaoRelatorio.Count();
        }

        #endregion Métodos Públicos
    }
}
