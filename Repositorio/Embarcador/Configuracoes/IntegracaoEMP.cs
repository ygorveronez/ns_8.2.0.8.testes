using NHibernate.Linq;
using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoEMP : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP>
    {
        public IntegracaoEMP(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP>();
            return consultaIntegracao.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP BuscarComFetch()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP>();

            return consultaIntegracao
                .Fetch(o => o.CertificadoCRTServerRetina)
                .Fetch(o => o.CertificadoShemaRegistryRetina)
                .FirstOrDefault();
        }
    }
}
