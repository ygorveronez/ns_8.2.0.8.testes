using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Usuarios
{
    public class CaixaFuncionarioMovimentoFinanceiro : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioMovimentoFinanceiro>
    {
        public CaixaFuncionarioMovimentoFinanceiro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioMovimentoFinanceiro BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioMovimentoFinanceiro>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioMovimentoFinanceiro> BuscarPorCaixa(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioMovimentoFinanceiro>();
            var result = from obj in query where obj.CaixaFuncionario.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito> Consultar(int codigoCaixa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioMovimentoFinanceiro>();

            var result = from obj in query select obj;

            if (codigoCaixa > 0)
                result = result.Where(obj => obj.CaixaFuncionario.Codigo == codigoCaixa);

            var queryContas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito>();
            var resultContas = from obj in queryContas select obj;
            resultContas = resultContas.Where(o => result.Select(p => p.MovimentoFinanceiroDebitoCredito).Contains(o));

            return resultContas.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoCaixa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioMovimentoFinanceiro>();

            var result = from obj in query select obj;

            if (codigoCaixa > 0)
                result = result.Where(obj => obj.CaixaFuncionario.Codigo == codigoCaixa);

            var queryContas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito>();
            var resultContas = from obj in queryContas select obj;
            resultContas = resultContas.Where(o => result.Select(p => p.MovimentoFinanceiroDebitoCredito).Contains(o));

            return resultContas.Count();
        }
    }
}
