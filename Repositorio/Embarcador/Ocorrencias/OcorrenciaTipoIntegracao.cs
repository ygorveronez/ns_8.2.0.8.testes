using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class OcorrenciaTipoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao>
    {
        public OcorrenciaTipoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao> BuscarPorTipoOcorrencia(int codigoTipoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao>()
                .Where(o => o.TipoOcorrencia.Codigo == codigoTipoOcorrencia);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> BuscarIntegracaoPorTipoOcorrencia(int codigoTipoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao>()
                .Where(o => o.TipoOcorrencia.Codigo == codigoTipoOcorrencia);

            return query.Select(o => o.TipoIntegracao).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTiposIntegracaoPorTipoOcorrencia(int codigoTipoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao>()
                .Where(o => o.TipoOcorrencia.Codigo == codigoTipoOcorrencia);

            return query.Select(o => o.TipoIntegracao.Tipo).ToList();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao BuscarPorTipoOcorrenciaETipoIntegracao(int codigoTipoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao>()
                .Where(o => o.TipoOcorrencia.Codigo == codigoTipoOcorrencia && o.TipoIntegracao.Tipo == tipoIntegracao);

            return query.FirstOrDefault();
        }

        public bool PossuiIntegracaoPorTipoOcorrenciaETipoIntegracao(int codigoTipoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao>()
                .Where(o => o.TipoOcorrencia.Codigo == codigoTipoOcorrencia && o.TipoIntegracao.Tipo == tipoIntegracao);

            return query.Any();
        }

        public bool PossuiIntegracaoPorTipoOcorrenciaETipoIntegracao(int codigoTipoOcorrencia, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao>()
                .Where(o => o.TipoOcorrencia.Codigo == codigoTipoOcorrencia && tiposIntegracao.Contains(o.TipoIntegracao.Tipo));

            return query.Any();
        }

        #endregion
    }
}
