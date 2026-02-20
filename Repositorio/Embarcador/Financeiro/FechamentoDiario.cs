using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class FechamentoDiario : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.FechamentoDiario>
    {
        public FechamentoDiario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public DateTime? ObterUltimaDataFechamento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.FechamentoDiario>();

            return query.Max(o => (DateTime?)o.DataFechamento);
        }

        public bool VerificarSeExistePorDataFechamento(int codigoEmpresa, DateTime dataFechamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento? tipoDocumento = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.FechamentoDiario>();

            query = query.Where(o => o.DataFechamento >= dataFechamento.Date);

            if (tipoDocumento == null || tipoDocumento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada)
                query = query.Where(o => !o.BloquearApenasDocumentoEntrada);

            if (codigoEmpresa > 0)
                query = query.Where(c => c.Empresa.Codigo == codigoEmpresa);
            else
                query = query.Where(c => c.Empresa == null);

            return query.Any();
        }

        public bool VerificarSeExisteFechamentoPosterior(int codigoEmpresa, DateTime dataFechamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.FechamentoDiario>();

            query = query.Where(o => o.DataFechamento > dataFechamento.Date);

            if (codigoEmpresa > 0)
                query = query.Where(c => c.Empresa.Codigo == codigoEmpresa);

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.FechamentoDiario> Consultar(int codigoEmpresa, DateTime dataFechamentoInicial, DateTime dataFechamentoFinal, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.FechamentoDiario>();

            if (dataFechamentoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataFechamento >= dataFechamentoInicial.Date);

            if (dataFechamentoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataFechamento < dataFechamentoFinal.AddDays(1).Date);

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return query.Fetch(o => o.Usuario).OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, DateTime dataFechamentoInicial, DateTime dataFechamentoFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.FechamentoDiario>();

            if (dataFechamentoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataFechamento >= dataFechamentoInicial.Date);

            if (dataFechamentoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataFechamento < dataFechamentoFinal.AddDays(1).Date);

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return query.Count();
        }

        #endregion
    }
}
