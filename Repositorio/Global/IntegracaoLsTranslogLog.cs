using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class IntegracaoLsTranslogLog : RepositorioBase<Dominio.Entidades.IntegracaoLsTranslogLog>, Dominio.Interfaces.Repositorios.IntegracaoLsTranslogLog
    {
        public IntegracaoLsTranslogLog(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.IntegracaoLsTranslogLog BuscaPorCodigo(int empresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoLsTranslogLog>();

            var result = from obj in query where obj.Codigo == codigo && obj.IntegracaoLsTranslog.Empresa.Codigo == empresa select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.IntegracaoLsTranslogLog> BuscaPorCodigoIntegracao(int codigo, Dominio.ObjetosDeValor.Enumerador.TipoIntegracaoLsTranslog tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoLsTranslogLog>();

            var result = from obj in query where obj.IntegracaoLsTranslog.Codigo == codigo && obj.Tipo == tipo && obj.Status == Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Sucesso select obj;

            return result.ToList();
        }

        private IQueryable<Dominio.Entidades.IntegracaoLsTranslogLog> _ConsultarLogs(int empresa, int integracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoLsTranslogLog>();

            var result = from obj in query
                         where 
                         obj.IntegracaoLsTranslog.Empresa.Codigo == empresa
                         && obj.IntegracaoLsTranslog.Codigo == integracao
                         select obj;

            return result;
        }

        public List<Dominio.Entidades.IntegracaoLsTranslogLog> ConsultarLogs(int empresa, int integracao, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarLogs(empresa, integracao);

            result = result.OrderBy("Codigo ascending");

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaLogs(int empresa, int integracao)
        {
            var result = _ConsultarLogs(empresa, integracao);

            return result.Count();
        }
    }
}
