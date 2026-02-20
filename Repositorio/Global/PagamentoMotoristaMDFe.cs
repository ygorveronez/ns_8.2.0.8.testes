using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio
{
    public class PagamentoMotoristaMDFe : RepositorioBase<Dominio.Entidades.PagamentoMotoristaMDFe>, Dominio.Interfaces.Repositorios.PagamentoMotoristaMDFe
    {
        public PagamentoMotoristaMDFe(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.PagamentoMotoristaMDFe BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PagamentoMotoristaMDFe>();

            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.PagamentoMotoristaMDFe> Consultar(int codigoEmpresa, int codigoMDFe, int codigoMotorista, DateTime dataInicial, DateTime dataFinal, string status, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PagamentoMotoristaMDFe>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (codigoMDFe > 0)
                result = result.Where(o => o.MDFe.Codigo == codigoMDFe);

            if (codigoMotorista > 0)
                result = result.Where(o => o.Motorista.Codigo == codigoMotorista);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataPagamento >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataPagamento < dataFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.OrderByDescending(o => o.DataPagamento)
                         .ThenByDescending(o => o.Codigo)
                         .Skip(inicioRegistros).Take(maximoRegistros)
                         .Fetch(o => o.MDFe)
                         .Fetch(o => o.Motorista)
                         .ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int codigoMDFe, int codigoMotorista, DateTime dataInicial, DateTime dataFinal, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PagamentoMotoristaMDFe>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (codigoMDFe > 0)
                result = result.Where(o => o.MDFe.Codigo == codigoMDFe);

            if (codigoMotorista > 0)
                result = result.Where(o => o.Motorista.Codigo == codigoMotorista);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataPagamento >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataPagamento < dataFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.Count();
        }

        public List<Dominio.Entidades.PagamentoMotoristaMDFe> Relatorio(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int codigoMotorista, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PagamentoMotoristaMDFe>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataPagamento >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataPagamento <= dataFinal.AddDays(1).Date);

            if (codigoMotorista > 0)
                result = result.Where(o => o.Motorista.Codigo == codigoMotorista);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.ToList();
        }

        public decimal BuscarTotalFreteMotorista(int codigoPagamento, int codigoEmpresa, int codigoMotorista, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PagamentoMotoristaMDFe>();

            var result = from obj in query
                         where obj.Empresa.Codigo == codigoEmpresa &&
                               obj.Codigo != codigoPagamento &&
                               obj.Motorista.Codigo == codigoMotorista &&
                               obj.DataPagamento >= dataInicial && obj.DataPagamento <= dataFinal &&
                               obj.Deduzir == Dominio.Enumeradores.OpcaoSimNao.Sim
                         select obj;

            if (codigoPagamento > 0)
                result = result.Where(o => o.Codigo < codigoPagamento);

            if (result.Count() > 0)
                return (from obj in result select obj.ValorFrete).Sum();
            else
                return 0;
        }

        public decimal BuscarTotalINSSMotorista(int codigoPagamento, int codigoEmpresa, int codigoMotorista, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PagamentoMotoristaMDFe>();

            var result = from obj in query
                         where obj.Empresa.Codigo == codigoEmpresa &&
                               obj.Codigo != codigoPagamento &&
                               obj.Motorista.Codigo == codigoMotorista &&
                               obj.DataPagamento >= dataInicial && obj.DataPagamento <= dataFinal &&
                               obj.Deduzir == Dominio.Enumeradores.OpcaoSimNao.Sim
                         select obj;

            if (codigoPagamento > 0)
                result = result.Where(o => o.Codigo < codigoPagamento);

            if (result.Count() > 0)
                return (from obj in result select obj.ValorINSSSENAT).Sum();
            else
                return 0;
        }

        public decimal BuscarTotalIRMotorista(int codigoPagamento, int codigoEmpresa, int codigoMotorista, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PagamentoMotoristaMDFe>();

            var result = from obj in query
                         where obj.Empresa.Codigo == codigoEmpresa &&
                               obj.Codigo != codigoPagamento &&
                               obj.Motorista.Codigo == codigoMotorista &&
                               obj.DataPagamento >= dataInicial && obj.DataPagamento <= dataFinal &&
                               obj.Deduzir == Dominio.Enumeradores.OpcaoSimNao.Sim
                         select obj;

            if (codigoPagamento > 0)
                result = result.Where(o => o.Codigo < codigoPagamento);

            if (result.Count() > 0)
                return (from obj in result select obj.ValorImpostoRenda).Sum();
            else
                return 0;
        }
    }
}
