using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro.DespesaMensal
{
    public class DespesaMensalProcessamento : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento>
    {
        public DespesaMensalProcessamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento> Consultar(int codigoEmpresa, int codigoTipoDespesa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.Mes? mes, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento>();

            var result = from obj in query select obj;

            if (mes.HasValue)
                result = result.Where(obj => obj.Mes == mes);

            if (codigoTipoDespesa > 0)
                result = result.Where(obj => obj.TipoDespesaFinanceira.Codigo == codigoTipoDespesa);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int codigoTipoDespesa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.Mes? mes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamento>();

            var result = from obj in query select obj;

            if (mes.HasValue)
                result = result.Where(obj => obj.Mes == mes);

            if (codigoTipoDespesa > 0)
                result = result.Where(obj => obj.TipoDespesaFinanceira.Codigo == codigoTipoDespesa);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensal> ConsultarDespesasMensais(int codigoEmpresa, int codigoTipoDespesa, bool fazendoABusca, Dominio.ObjetosDeValor.Embarcador.Enumeradores.Mes mes, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensal>();

            var result = from obj in query where obj.Situacao && obj.Data.Year <= DateTime.Now.Year && (obj.Data.Month <= (int)mes || obj.Data.Year < DateTime.Now.Year) select obj;

            if (codigoTipoDespesa > 0)
                result = result.Where(obj => obj.TipoDespesaFinanceira.Codigo == codigoTipoDespesa);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (!fazendoABusca)
                result = result.Where(obj => obj.Codigo == -1);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaDespesasMensais(int codigoEmpresa, int codigoTipoDespesa, bool fazendoABusca, Dominio.ObjetosDeValor.Embarcador.Enumeradores.Mes mes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensal>();

            var result = from obj in query where obj.Situacao && obj.Data.Year <= DateTime.Now.Year && (obj.Data.Month <= (int)mes || obj.Data.Year < DateTime.Now.Year) select obj;

            if (codigoTipoDespesa > 0)
                result = result.Where(obj => obj.TipoDespesaFinanceira.Codigo == codigoTipoDespesa);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (!fazendoABusca)
                result = result.Where(obj => obj.Codigo == -1);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamentoDespesas> ConsultarPorCodigoDespesaMensalProcessamento(int codigo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamentoDespesas>();
            var result = from obj in query where obj.DespesaMensalProcessamento.Codigo == codigo select obj;

            if (maximoRegistros > 0)
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
            else
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).ToList();
        }

        public int ContarConsultarPorCodigoDespesaMensalProcessamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamentoDespesas>();
            var result = from obj in query where obj.DespesaMensalProcessamento.Codigo == codigo select obj;

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamentoDespesas VerificarPagamentoDespesaPorMesAno(int codigoEmpresa, int codigoDespesa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.Mes mes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensalProcessamentoDespesas>();
            var result = from obj in query
                         where
                           obj.DespesaMensal.Codigo == codigoDespesa &&
                           obj.DespesaMensalProcessamento.DataPagamento.Value.Month == (int)mes &&
                           obj.DespesaMensalProcessamento.DataPagamento.Value.Year == DateTime.Now.Year
                         select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.DespesaMensalProcessamento.Empresa.Codigo == codigoEmpresa);

            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }
    }
}
