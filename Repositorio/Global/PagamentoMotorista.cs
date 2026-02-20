using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;

namespace Repositorio
{
    public class PagamentoMotorista : RepositorioBase<Dominio.Entidades.PagamentoMotorista>, Dominio.Interfaces.Repositorios.PagamentoMotorista
    {
        public PagamentoMotorista(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.PagamentoMotorista BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PagamentoMotorista>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.PagamentoMotorista> BuscarPorEmpresaSemNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PagamentoMotorista>();

            var result = from obj in query where  obj.Empresa.Codigo == codigoEmpresa && (obj.Numero == null  || obj.Numero == 0) select obj;

            return result.OrderBy(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.PagamentoMotorista> Consultar(int codigoEmpresa, int codigoCTe, int codigoMotorista, DateTime dataInicial, DateTime dataFinal, string status, int numeroInicial, int numeroFinal, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PagamentoMotorista>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;
            
            if (codigoCTe > 0)
            {
                var queryPagtCTes = this.SessionNHiBernate.Query<Dominio.Entidades.PagamentoMotoristaCtes>();
                result = result.Where(o => (from obj in queryPagtCTes where obj.ConhecimentoDeTransporteEletronico.Codigo == codigoCTe select obj.PagamentoMotorista.Codigo).Contains(o.Codigo));
            }

            if (codigoMotorista > 0)
                result = result.Where(o => o.Motorista.Codigo == codigoMotorista);

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);
            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataPagamento >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataPagamento < dataFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.OrderByDescending(o => o.Numero)
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros)                         
                         .Fetch(o => o.CTE)
                         .Fetch(o => o.Motorista)
                         .ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int codigoCTe, int codigoMotorista, DateTime dataInicial, DateTime dataFinal, string status, int numeroInicial, int numeroFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PagamentoMotorista>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (codigoCTe > 0)
                result = result.Where(o => o.CTE.Codigo == codigoCTe);

            if (codigoMotorista > 0)
                result = result.Where(o => o.Motorista.Codigo == codigoMotorista);

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);
            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataPagamento >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataPagamento < dataFinal.AddDays(1).Date);
        
            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.Count();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoMotorista> ReciboPagamento()
        {
            throw new NotImplementedException();
        }
        public int BuscarUltimoNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PagamentoMotorista>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj.Numero;

            return result.Max(o => (int?)o) ?? 0;
        }
        public List<Dominio.Entidades.PagamentoMotorista> Relatorio(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int codigoMotorista, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PagamentoMotorista>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataPagamento >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataPagamento < dataFinal.AddDays(1).Date);

            if (codigoMotorista > 0)
                result = result.Where(o => o.Motorista.Codigo == codigoMotorista);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.ToList();
        }


        public List<Dominio.Entidades.PagamentoMotorista> BuscarParcelasPendentes(int codigoEmpresa, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PagamentoMotorista>();

            var result = from obj in query where obj.Status.Equals("P") && obj.Empresa.Codigo == codigoEmpresa select obj;

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataPagamento < dataFinal.AddDays(1).Date);

            return result.OrderBy(o => o.DataPagamento).ToList();
        }
    
        public decimal BuscarTotalAcumuladoPeriodo(string cpfMotorista, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PagamentoMotorista>();

            var result = from obj in query where obj.Motorista.CPF == cpfMotorista && obj.DataPagamento >= dataInicial && obj.DataPagamento <= dataFinal select obj;

            return result.Sum(o => o.ValorFrete);
        }
    
    }
}
