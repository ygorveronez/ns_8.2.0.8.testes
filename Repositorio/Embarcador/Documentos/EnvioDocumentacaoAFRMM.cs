using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Documentos
{
    public class EnvioDocumentacaoAFRMM : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM>
    {
        public EnvioDocumentacaoAFRMM(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<long> BuscarCodigosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentacaoAFRMM tipoDocumentacaoAFRMM, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote situacao, bool envioAutomatico, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoAFRMM>();

            query = query.Where(o => o.Situacao == situacao && o.TipoDocumentacaoAFRMM == tipoDocumentacaoAFRMM && o.EnvioAutomatico == envioAutomatico);

            return query.Select(o => o.Codigo).Skip(inicio).Take(limite).ToList();
        }
    }
}
