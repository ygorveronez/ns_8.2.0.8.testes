using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;

namespace Repositorio
{
    public class DespesaAdicionalEmpresa : RepositorioBase<Dominio.Entidades.DespesaAdicionalEmpresa>, Dominio.Interfaces.Repositorios.DespesaAdicionalEmpresa
    {
        public DespesaAdicionalEmpresa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.DespesaAdicionalEmpresa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DespesaAdicionalEmpresa>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.DespesaAdicionalEmpresa> Consultar(int codigoEmpresaPai, string nomeEmpresa, string descricaoPlano, DateTime dataInicial, DateTime dataFinal, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DespesaAdicionalEmpresa>();
            var result = from obj in query where obj.Empresa.EmpresaPai.Codigo == codigoEmpresaPai select obj;

            if (!string.IsNullOrWhiteSpace(nomeEmpresa))
                result = result.Where(o => o.Empresa.NomeFantasia.Contains(nomeEmpresa) || o.Empresa.RazaoSocial.Contains(nomeEmpresa));

            if (!string.IsNullOrWhiteSpace(descricaoPlano))
                result = result.Where(o => o.Descricao.Contains(descricaoPlano));

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataInicial >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataFinal <= dataFinal.AddDays(1).Date);

            result = result.Fetch(o => o.Empresa);

            return result.OrderBy(o => o.Empresa.NomeFantasia).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresaPai, string nomeEmpresa, string descricaoPlano, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DespesaAdicionalEmpresa>();
            var result = from obj in query where obj.Empresa.EmpresaPai.Codigo == codigoEmpresaPai select obj;

            if (!string.IsNullOrWhiteSpace(nomeEmpresa))
                result = result.Where(o => o.Empresa.NomeFantasia.Contains(nomeEmpresa) || o.Empresa.RazaoSocial.Contains(nomeEmpresa));

            if (!string.IsNullOrWhiteSpace(descricaoPlano))
                result = result.Where(o => o.Descricao.Contains(descricaoPlano));

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataInicial >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataFinal <= dataFinal.AddDays(1).Date);

            result = result.Fetch(o => o.Empresa);

            return result.Count();
        }

        public List<Dominio.Entidades.DespesaAdicionalEmpresa> BuscarPorEmpresas(int codigoEmpresaPai, int[] codigosEmpresas, DateTime dataInicial, DateTime dataFinal, string status, string tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DespesaAdicionalEmpresa>();

            var result = from obj in query
                         where
                             obj.Empresa.EmpresaPai.Codigo == codigoEmpresaPai &&
                             codigosEmpresas.Contains(obj.Empresa.Codigo) &&
                             obj.DataInicial > dataInicial &&
                             obj.DataFinal <= dataFinal.AddDays(1).Date &&
                             obj.Status == status &&
                             obj.Tipo == tipo
                         select obj;

            return result.ToList();
        }

        public decimal BuscarTotalPorEmpresa(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, string status, string tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DespesaAdicionalEmpresa>();

            var result = from obj in query
                         where                             
                             obj.Empresa.Codigo == codigoEmpresa &&
                             obj.DataInicial >= dataInicial &&
                             obj.DataFinal < dataFinal.AddDays(1).Date &&
                             obj.Status == status &&
                             obj.Tipo == tipo
                         select obj;


            return result.Sum(o => (decimal?)o.Valor )?? 0m;
        }
    }
}
