using Infrastructure.Services.Cache;
using System;
using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class Integracao : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.Integracao>
    {
        private const string ConfiguracaoIntegracaoKey = "ConfiguracaoIntegracao";
        #region Construtores

        public Integracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Integracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        //TODO: Ajustar para async
        public Dominio.Entidades.Embarcador.Configuracoes.Integracao Buscar()
        {
            //return CacheProvider.Instance.GetOrCreate(ConfiguracaoIntegracaoKey,
            //    () =>
            //    {
            //        var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Integracao>();
            //        var result = from obj in query select obj;
            //        return result.FirstOrDefault();
            //    }, TimeSpan.FromMinutes(1));

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Integracao>();
            var result = from obj in query select obj;
            return result.FirstOrDefault();
        }


        public string BuscarConfiguracaoIntegracaoServidorRouteOSM()
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Integracao>()
                .Select(o => o.ServidorRouteOSM)
                .FirstOrDefault();
        }

        #endregion Métodos Públicos
    }
}
