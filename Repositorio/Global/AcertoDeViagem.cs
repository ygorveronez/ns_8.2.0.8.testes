using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using NHibernate.Criterion;

namespace Repositorio
{
    public class AcertoDeViagem : RepositorioBase<Dominio.Entidades.AcertoDeViagem>, Dominio.Interfaces.Repositorios.AcertoDeViagem
    {
        public AcertoDeViagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.AcertoDeViagem> BuscarParcelasPendentes(int codigoEmpresa, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AcertoDeViagem>();

            var result = from obj in query where obj.Status == "A" && obj.Situacao != "F" && obj.Empresa.Codigo == codigoEmpresa select obj;

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataVencimento < dataFinal.AddDays(1).Date);

            return result.OrderBy(o => o.DataVencimento).ToList();
        }


        public Dominio.Entidades.AcertoDeViagem BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AcertoDeViagem>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.AcertoDeViagem> BuscarPorEmpresaPlaca (int codigoEmpresa, string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AcertoDeViagem>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Veiculo.Placa.Equals(placa) select obj;
            return result.ToList();
        }
        public int BuscarProximoNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AcertoDeViagem>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;
            int numero = result.Count() > 0 ? result.Max(o => o.Numero) : 0;
            return numero + 1;
        }

        public List<Dominio.Entidades.AcertoDeViagem> Consultar(int codigoEmpresa, string status, string nomeMotorista, string placaVeiculo, int numero, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AcertoDeViagem>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status == status);

            if (!string.IsNullOrWhiteSpace(nomeMotorista))
                result = result.Where(o => o.Motorista.Nome.Contains(nomeMotorista));

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                result = result.Where(o => o.Veiculo.Placa == placaVeiculo);

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            result = result.OrderBy("Numero descending");

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string status, string nomeMotorista, string placaVeiculo, int numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AcertoDeViagem>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status == status);

            if (!string.IsNullOrWhiteSpace(nomeMotorista))
                result = result.Where(o => o.Motorista.Nome.Contains(nomeMotorista));

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                result = result.Where(o => o.Veiculo.Placa == placaVeiculo);

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            return result.Count();
        }

        public IList<Dominio.Entidades.AcertoDeViagem> Relatorio(int codigoEmpresa, int codigoVeiculo, int codigoMotorista, DateTime dataInicialLancamento, DateTime dataFinalLancamento, string situacao)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.AcertoDeViagem>();

            criteria.Add(Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            criteria.Add(Restrictions.Eq("Status", "A"));

            if (codigoVeiculo > 0)
                criteria.Add(Restrictions.Eq("Veiculo.Codigo", codigoVeiculo));
            
            if (codigoMotorista > 0)
                criteria.Add(Restrictions.Eq("Motorista.Codigo", codigoMotorista));
            
            if (dataInicialLancamento != DateTime.MinValue)
                criteria.Add(Restrictions.Ge("DataLancamento", dataInicialLancamento.Date));
            
            if (dataFinalLancamento != DateTime.MinValue)
                criteria.Add(Restrictions.Lt("DataLancamento", dataFinalLancamento.Date.AddDays(1)));
            
            if (!string.IsNullOrWhiteSpace(situacao))
                criteria.Add(Restrictions.Eq("Situacao", situacao));

            return criteria.List<Dominio.Entidades.AcertoDeViagem>();
        }
    }
}
