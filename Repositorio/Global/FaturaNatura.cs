using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class FaturaNatura : RepositorioBase<Dominio.Entidades.FaturaNatura>, Dominio.Interfaces.Repositorios.FaturaNatura
    {
        public FaturaNatura(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.FaturaNatura BuscarPorCodigo(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FaturaNatura>();

            var result = from obj in query where obj.Codigo == codigoFatura select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.FaturaNatura BuscarPorCodigo(int codigoEmpresa, int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FaturaNatura>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Codigo == codigoFatura select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.FaturaNatura BuscarPorPreFatura(long numPreFatura, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FaturaNatura>();

            var result = from obj in query where obj.NumeroPreFatura == numPreFatura && obj.Status != Dominio.ObjetosDeValor.Enumerador.StatusFaturaNatura.Cancelada && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public int ObterUltimoNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FaturaNatura>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj.Numero;

            return result.Max(o => (int?)o) ?? 0;
        }

        public List<Dominio.Entidades.FaturaNatura> Consultar(int codigoEmpresa, long numeroFatura, long numeroPreFatura, DateTime dataInicial, DateTime dataFinal, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FaturaNatura>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (numeroFatura > 0)
                result = result.Where(o => o.Numero == numeroFatura);

            if (numeroPreFatura > 0)
                result = result.Where(o => o.NumeroPreFatura == numeroPreFatura);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataPreFatura >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataPreFatura < dataFinal.AddDays(1).Date);

            return result.OrderByDescending(o => o.Numero).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, long numeroFatura, long numeroPreFatura, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FaturaNatura>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (numeroFatura > 0)
                result = result.Where(o => o.Numero == numeroFatura);

            if (numeroPreFatura > 0)
                result = result.Where(o => o.NumeroPreFatura == numeroPreFatura);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataPreFatura >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataPreFatura < dataFinal.AddDays(1).Date);

            return result.Count();
        }

        public List<Dominio.Entidades.FaturaNatura> BuscarPorSituacao(int codigoEmpresa, Dominio.ObjetosDeValor.Enumerador.StatusFaturaNatura situacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FaturaNatura>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == situacao select obj;

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarPorSituacao(int codigoEmpresa, Dominio.ObjetosDeValor.Enumerador.StatusFaturaNatura situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FaturaNatura>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == situacao select obj;

            return result.Count();
        }

    }
}
