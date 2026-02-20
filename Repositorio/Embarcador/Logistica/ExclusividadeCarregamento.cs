using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class ExclusividadeCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento>
    {
        #region Construtores

        public ExclusividadeCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaExclusividadeCarregamento filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento>();

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                query = query.Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);

            if (filtrosPesquisa.Transportador > 0)
                query = query.Where(o => o.Transportador.Codigo == filtrosPesquisa.Transportador);

            if (filtrosPesquisa.Cliente > 0)
                query = query.Where(o => o.Cliente.Codigo == filtrosPesquisa.Cliente);

            if (filtrosPesquisa.DataInicial.HasValue)
                query = query.Where(o => o.DataInicial.Date >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                query = query.Where(o => o.DataInicial.Date <= filtrosPesquisa.DataLimite.Value.Date);

            return query;
        }

        #endregion

        #region Métodos Públicos
        public List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaExclusividadeCarregamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = Consultar(filtrosPesquisa);

            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaExclusividadeCarregamento filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> BuscarExclusividadePorPeriodo(int codigoCentroCarregamento, DateTime data, DiaSemana diaSemana)
        {
            var result = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento>()
                .Where(o => o.DataInicial.Date <= data.Date && o.DataFinal.Date >= data.Date)
                //.Where(o => )
                .Where(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento);

            if (diaSemana == DiaSemana.Segunda) result = result.Where(o => o.DisponivelSegunda);
            else if (diaSemana == DiaSemana.Terca) result = result.Where(o => o.DisponivelTerca);
            else if (diaSemana == DiaSemana.Quarta) result = result.Where(o => o.DisponivelQuarta);
            else if (diaSemana == DiaSemana.Quinta) result = result.Where(o => o.DisponivelQuinta);
            else if (diaSemana == DiaSemana.Sexta) result = result.Where(o => o.DisponivelSexta);
            else if (diaSemana == DiaSemana.Sabado) result = result.Where(o => o.DisponivelSabado);
            else if (diaSemana == DiaSemana.Domingo) result = result.Where(o => o.DisponivelDomingo);

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento BuscarExclusividadeIncompativel(Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento exclusividade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>();
            var queryExclusividade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>();

            DateTime dataInicial = exclusividade.DataInicial;
            DateTime dataFinal = exclusividade.DataFinal;

            var resultExclusividade = from obj in queryExclusividade where obj.ExclusividadeCarregamento.Codigo == exclusividade.Codigo select obj;

            var result = from obj in query
                         where
                           obj.ExclusividadeCarregamento != null
                           && obj.ExclusividadeCarregamento.Codigo != exclusividade.Codigo
                           && obj.CentroCarregamento.Codigo == exclusividade.CentroCarregamento.Codigo
                         select obj;

            if (exclusividade.Transportador != null)
                result = result.Where(o => o.ExclusividadeCarregamento.Transportador.Codigo == exclusividade.Transportador.Codigo);
            else
                result = result.Where(o => o.ExclusividadeCarregamento.Transportador == null);

            if (exclusividade.Cliente != null)
                result = result.Where(o => o.ExclusividadeCarregamento.Cliente.CPF_CNPJ == exclusividade.Cliente.Codigo);
            else
                result = result.Where(o => o.ExclusividadeCarregamento.Cliente == null);

            result = result.Where(o =>
                (
                    (o.ExclusividadeCarregamento.DataInicial.Date <= dataInicial.Date && o.ExclusividadeCarregamento.DataFinal.Date >= dataFinal.Date) ||
                    (o.ExclusividadeCarregamento.DataInicial.Date >= dataInicial.Date && o.ExclusividadeCarregamento.DataFinal.Date <= dataFinal.Date) ||
                    (o.ExclusividadeCarregamento.DataInicial.Date >= dataInicial.Date && o.ExclusividadeCarregamento.DataInicial.Date <= dataFinal.Date) ||
                    (o.ExclusividadeCarregamento.DataFinal.Date >= dataInicial.Date && o.ExclusividadeCarregamento.DataFinal.Date <= dataFinal.Date)
                ) && (
                    (o.ExclusividadeCarregamento.DisponivelSegunda == exclusividade.DisponivelSegunda && exclusividade.DisponivelSegunda) ||
                    (o.ExclusividadeCarregamento.DisponivelTerca == exclusividade.DisponivelTerca && exclusividade.DisponivelTerca) ||
                    (o.ExclusividadeCarregamento.DisponivelQuarta == exclusividade.DisponivelQuarta && exclusividade.DisponivelQuarta) ||
                    (o.ExclusividadeCarregamento.DisponivelQuinta == exclusividade.DisponivelQuinta && exclusividade.DisponivelQuinta) ||
                    (o.ExclusividadeCarregamento.DisponivelSexta == exclusividade.DisponivelSexta && exclusividade.DisponivelSexta) ||
                    (o.ExclusividadeCarregamento.DisponivelSabado == exclusividade.DisponivelSabado && exclusividade.DisponivelSabado) ||
                    (o.ExclusividadeCarregamento.DisponivelDomingo == exclusividade.DisponivelDomingo && exclusividade.DisponivelDomingo)
                )
            );

            result = result.Where(o => resultExclusividade.Any(periodo =>
                (o.HoraInicio <= periodo.HoraInicio && o.HoraTermino >= periodo.HoraTermino) ||
                (o.HoraInicio >= periodo.HoraInicio && o.HoraInicio <= periodo.HoraTermino) ||
                (o.HoraTermino >= periodo.HoraInicio && o.HoraTermino <= periodo.HoraTermino)
            ));

            return result.Select(o => o.ExclusividadeCarregamento).FirstOrDefault();
        }

        #endregion
    }
}
