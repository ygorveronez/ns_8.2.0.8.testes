using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Documentos
{
    public class FaturamentoCIOT : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.FaturamentoCIOT>
    {
        public FaturamentoCIOT(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public FaturamentoCIOT(UnitOfWork unitOfWork, CancellationToken cancelllationToken) : base(unitOfWork, cancelllationToken) { }

        public Dominio.Entidades.Embarcador.Documentos.FaturamentoCIOT BuscarPorNumero(long numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.FaturamentoCIOT>();

            var result = from o in query where o.Numero == numero select o;

            return result.FirstOrDefault();
        }
        private IQueryable<Dominio.Entidades.Embarcador.Documentos.FaturamentoCIOT> _Consultar(DateTime dataVencimentoInicial, DateTime dataVencimentoFinal)
        {
            var query = this.SessionNHiBernate.Query < Dominio.Entidades.Embarcador.Documentos.FaturamentoCIOT>();

            var result = from obj in query select obj;

            // Filtros
            if (dataVencimentoInicial != DateTime.MinValue)
                result = result.Where(o => o.Vencimento.Date >= dataVencimentoInicial);

            if (dataVencimentoFinal != DateTime.MinValue)
                result = result.Where(o => o.Vencimento.Date >= dataVencimentoFinal);

            return result;
        }

        public List<Dominio.Relatorios.Embarcador.DataSource.Documentos.FaturaCIOT> ConsultarRelatorio(DateTime dataVencimentoInicial, DateTime dataVencimentoFinal, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var result = _Consultar(dataVencimentoInicial, dataVencimentoFinal);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicio > 0)
                result = result.Skip(inicio);

            if (limite > 0)
                result = result.Take(limite);

            return result
                .Select(o => new Dominio.Relatorios.Embarcador.DataSource.Documentos.FaturaCIOT {
                    Fechamento = o.Fechamento,
                    Vencimento = o.Vencimento,
                    Transportadora = o.Transportadora.RazaoSocial,
                    Numero = o.Numero,
                    Taxa = o.Taxa,
                    Tarifa = o.Tarifa,
                    Tipo = o.Tipo,
                    Status = o.Status,
                })
                .ToList();
        }

        public int ContarConsultaRelatorio(DateTime dataVencimentoInicial, DateTime dataVencimentoFinal, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena)
        {
            var result = _Consultar(dataVencimentoInicial, dataVencimentoFinal);

            return result.Count();
        }
    }
}
