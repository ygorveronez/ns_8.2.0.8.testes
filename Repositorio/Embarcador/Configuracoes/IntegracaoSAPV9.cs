using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoSAPV9 : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAPV9>
    {
        #region Construtores

        public IntegracaoSAPV9(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoSAPV9(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        public string BuscarProtocoloPorCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoExcluirido)
        {
            var cargaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>().
            Where(obj => obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Tipo != tipoExcluirido &&
              (obj.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_V9 ||
                obj.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_AV ||
                obj.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_SU ||
                obj.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_ST)).OrderByDescending(x => x.DataIntegracao).ToList();
            return cargaIntegracao.Where(x => x.Protocolo != null && x.Protocolo != "").FirstOrDefault()?.Protocolo ?? "";
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao BuscarProtocoloPorCargaTipo(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoExcluirido)
        {
            var cargaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>().
            Where(obj => obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Tipo == tipoExcluirido).OrderByDescending(x => x.DataIntegracao);
            return cargaIntegracao.Where(x => x.Protocolo != null && x.Protocolo != "").FirstOrDefault();
        }
    }
}
