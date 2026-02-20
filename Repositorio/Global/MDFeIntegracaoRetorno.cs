using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class MDFeIntegracaoRetorno : RepositorioBase<Dominio.Entidades.MDFeIntegracaoRetorno>, Dominio.Interfaces.Repositorios.MDFeIntegracaoRetorno
    {
        public MDFeIntegracaoRetorno(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.MDFeIntegracaoRetorno BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeIntegracaoRetorno>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.MDFeIntegracaoRetorno> BuscarPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeIntegracaoRetorno>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result.ToList();
        }

        public List<int> BuscarPendentes(int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeIntegracaoRetorno>();
            var result = from obj in query where (obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado || obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado || obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmitidoContingencia ) && (obj.SituacaoIntegracao == Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Aguardando || (obj.SituacaoIntegracao == Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha && obj.NumeroTentativas < 3)) select obj;
            return result.OrderBy(o => o.Codigo).Select(o => o.Codigo).Take(maximoRegistros).ToList();
        }

        public Dominio.Entidades.MDFeIntegracaoRetorno BuscarUltipoPorPorMDFeTipo(int codigoMDFe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeIntegracaoRetorno>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe && obj.TipoIntegracao.Tipo == tipoIntegracao select obj;
            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

    }
}
