using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class DiariaAutomatica : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica>
    {
        public DiariaAutomatica(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica BuscarPorChamado(int codigoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica>();
            var result = from obj in query where obj.Chamado.Codigo == codigoChamado select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica> BuscarPorMinutosSemAtualizacao(int minutosSemAtualizacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica>();

            var dataCorte = DateTime.Now.AddMinutes(-minutosSemAtualizacao);

            var result = from obj in query where obj.Status != StatusDiariaAutomatica.Finalizada 
                         && obj.Status != StatusDiariaAutomatica.Cancelada 
                         && (!obj.DataUltimaAtualizacao.HasValue || obj.DataUltimaAtualizacao.Value < dataCorte) select obj;

            return result.ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica> _Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaDiariaAutomatica filtroPesquisaDiariaAutomatica)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica>();

            // Filtros da ocorrencia
            if (filtroPesquisaDiariaAutomatica.DataInicial != DateTime.MinValue)
                query = query.Where(obj => obj.DataInicioCobranca.Date >= filtroPesquisaDiariaAutomatica.DataInicial);

            if (filtroPesquisaDiariaAutomatica.DataFinal != DateTime.MinValue)
                query = query.Where(obj => obj.DataInicioCobranca.Date <= filtroPesquisaDiariaAutomatica.DataFinal);

            if(filtroPesquisaDiariaAutomatica.CodigoTransportador > 0)
                query = query.Where(obj => obj.Carga.Empresa.Codigo == filtroPesquisaDiariaAutomatica.CodigoTransportador);

            if (filtroPesquisaDiariaAutomatica.CodigoFilial > 0)
                query = query.Where(obj => obj.Carga.Filial.Codigo == filtroPesquisaDiariaAutomatica.CodigoFilial);

            if (filtroPesquisaDiariaAutomatica.CodigoCarga > 0)
                query = query.Where(obj => obj.Carga.Codigo == filtroPesquisaDiariaAutomatica.CodigoCarga);

            if (filtroPesquisaDiariaAutomatica.LocalFreeTime != LocalFreeTime.Nenhum)
                query = query.Where(obj => obj.LocalFreeTime == filtroPesquisaDiariaAutomatica.LocalFreeTime);

            if (filtroPesquisaDiariaAutomatica.Status != StatusDiariaAutomatica.Nenhum)
                query = query.Where(obj => obj.Status == filtroPesquisaDiariaAutomatica.Status);

            return query;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaDiariaAutomatica filtroPesquisaDiariaAutomatica, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(filtroPesquisaDiariaAutomatica);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Filial)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Empresa)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaDiariaAutomatica filtroPesquisaDiariaAutomatica)
        {
            var result = _Consultar(filtroPesquisaDiariaAutomatica);

            return result.Count();
        }
    }
}
