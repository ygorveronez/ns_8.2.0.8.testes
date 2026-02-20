using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class Subcontratacao : RepositorioBase<Dominio.Entidades.Subcontratacao>
    {
        public Subcontratacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Subcontratacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Subcontratacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Subcontratacao BuscarPorDocumentoSubcontratacao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Subcontratacao>();
            var result = from obj in query where obj.DocumentoSubcontratacao.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarPendentes( int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Subcontratacao>();
            var result = from obj in query where obj.Situacao == Dominio.Enumeradores.SituacaoSubcontratacao.AgProcessamento select obj;

            return result.OrderBy(o => o.Codigo).Select(o => o.Codigo).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Subcontratacao> Consultar(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal,  string status, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Subcontratacao>();

            var result = from obj in query where obj.Empresa.Codigo > 1 select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.EmpresaSubcontratada.Codigo == codigoEmpresa);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataImportacao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataImportacao <= dataFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "F")
                    result = result.Where(o => o.Situacao == Dominio.Enumeradores.SituacaoSubcontratacao.FalhaProcessamento);
                else
                    result = result.Where(o => o.DocumentoSubcontratacao.Status == status);
            }

            return result.OrderByDescending(o => o.DataImportacao)
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros)
                         .ToList();
        }

        public int ContarConsulta(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Subcontratacao>();

            var result = from obj in query where obj.Empresa.Codigo > 1 select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.EmpresaSubcontratada.Codigo == codigoEmpresa);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataImportacao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataImportacao <= dataFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "F")
                    result = result.Where(o => o.Situacao == Dominio.Enumeradores.SituacaoSubcontratacao.FalhaProcessamento);
                else
                    result = result.Where(o => o.DocumentoSubcontratacao.Status == status);
            }

            return result.Count();
        }
    }
}
