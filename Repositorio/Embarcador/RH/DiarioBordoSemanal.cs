using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.RH
{
    public class DiarioBordoSemanal : RepositorioBase<Dominio.Entidades.Embarcador.RH.DiarioBordoSemanal>
    {
        public DiarioBordoSemanal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.RH.DiarioBordoSemanal BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.DiarioBordoSemanal>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.DiarioBordoSemanal>();
            
            if (query.Any())
                return query.Max(obj => obj.Numero) + 1;
            else
                return 1;
        }

        private IQueryable<Dominio.Entidades.Embarcador.RH.DiarioBordoSemanal> _Consultar(int codigoMotorista, int codigoCarga, int codigoVeiculo, int numero, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDiarioBordoSemanal situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.DiarioBordoSemanal>();

            var result = from obj in query select obj;

            // Filtros
            if (codigoMotorista > 0)
                result = result.Where(o => o.Motorista.Codigo == codigoMotorista);

            if (codigoCarga > 0)
                result = result.Where(o => o.Carga.Codigo == codigoCarga);

            if (codigoVeiculo > 0)
                result = result.Where(o => o.Veiculo.Codigo == codigoVeiculo);

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            if (dataInicio > DateTime.MinValue)
                result = result.Where(o => o.DataInicio.Date >= dataInicio.Date);

            if (dataFim > DateTime.MinValue)
                result = result.Where(o => o.DataFim.Date <= dataFim.Date);

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDiarioBordoSemanal.Todas)
                result = result.Where(o => o.SituacaoDiarioBordoSemanal == situacao);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.RH.DiarioBordoSemanal> Consultar(int codigoMotorista, int codigoCarga, int codigoVeiculo, int numero, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDiarioBordoSemanal situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoMotorista, codigoCarga, codigoVeiculo, numero, dataInicio, dataFim, situacao);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int codigoMotorista, int codigoCarga, int codigoVeiculo, int numero, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDiarioBordoSemanal situacao)
        {
            var result = _Consultar(codigoMotorista, codigoCarga, codigoVeiculo, numero, dataInicio, dataFim, situacao);

            return result.Count();
        }
    }
}
