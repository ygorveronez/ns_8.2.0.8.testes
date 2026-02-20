using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class ReservaCargaGrupoPessoa : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa>
    {

        public ReservaCargaGrupoPessoa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa> BuscarPorPrevisao(int codigoPrevisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa>();
            var result = from obj in query where obj.PrevisaoCarregamento.Codigo == codigoPrevisao select obj;
            return result.ToList();
        }


        private IQueryable<Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa> _Consultar(DateTime dataInicio, DateTime dataFim, int centroCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa>();

            var result = from obj in query select obj;

            // Filtros
            if (dataInicio != DateTime.MinValue)
                result = result.Where(o => o.DataReserva >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(o => o.DataReserva < dataFim.AddDays(1));

            if (centroCarregamento > 0)
                result = result.Where(o => o.CentroCarregamento.Codigo == centroCarregamento);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa> Consultar(DateTime dataInicio, DateTime dataFim, int centroCarregamento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(dataInicio, dataFim, centroCarregamento);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsulta(DateTime dataInicio, DateTime dataFim, int centroCarregamento)
        {
            var result = _Consultar(dataInicio, dataFim, centroCarregamento);

            return result.Count();
        }

        public int TotalReservadoPrevisao(int previsao, DateTime dataReserva, int excetoReserva)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa>();

            var result = from obj in query
                         where 
                            obj.PrevisaoCarregamento.Codigo == previsao &&
                            obj.DataReserva == dataReserva
                         select obj;

            // Para calcular a quantia, temos que desconsiderar a reserva que esta sendo atualizada
            if (excetoReserva > 0)
                result = result.Where(o => o.Codigo != excetoReserva);

            return result.Count() > 0 ? result.Sum(o => o.QuantidadeReservada) : 0;
        }

        public int ContarReservasPorGrupos(int centroCarregamento, int previsaoCarregamentoDia, DateTime dia, int [] grupos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa>();

            var result = from obj in query
                         where 
                             grupos.Contains(obj.GrupoPessoas.Codigo) && 
                             obj.DataReserva == dia.Date &&
                             obj.CentroCarregamento.Codigo == centroCarregamento &&
                             obj.PrevisaoCarregamento.Codigo == previsaoCarregamentoDia
                         select obj.QuantidadeReservada;

            return result.Count() > 0 ? result.Sum() : 0;
        }

        public int ContarReservasPorDia(int centroCarregamento, int previsaoCarregamentoDia, DateTime dia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa>();

            var result = from obj in query
                         where
                             obj.DataReserva == dia.Date &&
                             obj.CentroCarregamento.Codigo == centroCarregamento &&
                             obj.PrevisaoCarregamento.Codigo == previsaoCarregamentoDia
                         select obj.QuantidadeReservada;

            return result.Count() > 0 ? result.Sum() : 0;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa> ReservasDoDia(int centroCarregamento, int previsaoCarregamentoDia, DateTime dia, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa>();

            var result = from obj in query
                         where
                             obj.DataReserva == dia.Date &&
                             obj.CentroCarregamento.Codigo == centroCarregamento &&
                             obj.PrevisaoCarregamento.Codigo == previsaoCarregamentoDia
                         select obj;

            result = result.OrderBy(propOrdena + " " + dirOrdena);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> GruposReservadosDoDia(int centroCarregamento, int previsaoCarregamentoDia, DateTime dia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa>();

            var result = from obj in query
                         where
                             obj.DataReserva == dia.Date &&
                             obj.CentroCarregamento.Codigo == centroCarregamento &&
                             obj.PrevisaoCarregamento.Codigo == previsaoCarregamentoDia
                         select obj.GrupoPessoas;
            
            return result.Distinct().ToList();
        }


        public int ContarReservasDoDia(int centroCarregamento, int previsaoCarregamentoDia, DateTime dia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa>();

            var result = from obj in query
                         where
                             obj.DataReserva == dia.Date &&
                             obj.CentroCarregamento.Codigo == centroCarregamento &&
                             obj.PrevisaoCarregamento.Codigo == previsaoCarregamentoDia
                         select obj;

            return result.Count();
        }
    }
}
