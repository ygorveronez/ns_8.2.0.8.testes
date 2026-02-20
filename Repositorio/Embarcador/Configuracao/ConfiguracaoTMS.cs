using Infrastructure.Services.Cache;
using NHibernate.Linq;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracao
{
    public class ConfiguracaoTMS : RepositorioBase<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS>
    {
        private const string ConfiguracaoPadraoKey = "ConfiguracaoTMS";
        #region Construtores

        public ConfiguracaoTMS(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ConfiguracaoTMS(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS BuscarConfiguracaoPadrao(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = null)
        {
            //return configuracaoTMS ?? CacheProvider.Instance.GetOrCreate(ConfiguracaoPadraoKey,
            //    () => SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS>()
            //    .Fetch(x => x.ConfiguracaoTabelaFrete)
            //    .FirstOrDefault()
            //    , TimeSpan.FromMinutes(1));

            return configuracaoTMS ?? SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS>()
                .Fetch(x => x.ConfiguracaoTabelaFrete)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS BuscarConfiguracaoPadraoCalculoFrete(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = null)
        {
            return configuracaoTMS != null ? configuracaoTMS : SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS>().Fetch(x => x.ComponenteFreteDescontoSeguro).FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS> BuscarConfiguracaoPadraoAsync(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = null)
        {
            //return configuracaoTMS ?? await CacheProvider.Instance.GetOrCreateAsync(
            //    ConfiguracaoPadraoKey,
            //    async () =>
            //    {
            //        return await SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS>()
            //        .Fetch(x => x.ConfiguracaoTabelaFrete)
            //        .FirstOrDefaultAsync();
            //    },
            //    TimeSpan.FromMinutes(1)
            //);

            return configuracaoTMS ?? await SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS>()
                    .Fetch(x => x.ConfiguracaoTabelaFrete)
                    .FirstOrDefaultAsync();
        }

        public bool GerarCanhotoSempre()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS>();
            return query.Select(o => o.GerarCanhotoSempre).FirstOrDefault();
        }

        public bool ImportarCargasMultiEmbarcador()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS>();
            return query.Select(o => o.ImportarCargasMultiEmbarcador).FirstOrDefault();
        }

        public bool UtilizaEmissaoMultimodal()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS>();
            return query.Select(o => (bool?)o.UtilizaEmissaoMultimodal).FirstOrDefault() ?? false;
        }

        public bool ArmazenarXMLCTeEmArquivo()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS>();
            return query.Select(o => (bool?)o.ArmazenarXMLCTeEmArquivo).FirstOrDefault() ?? false;
        }

        public async Task<bool> UtilizaAppTrizy()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS>();
            return await query.Select(o => (bool?)o.UtilizaAppTrizy).FirstOrDefaultAsync() ?? false;
        }

        public bool UtilizaNumeroSequencialCargaCarregamento()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS>();
            return query.Select(configuracao => configuracao.NumeroCargaSequencialUnico && configuracao.UtilizarNumeroSequencialCargaNoCarregamento).FirstOrDefault();
        }

        public int ObterLimiteRegistrosRelatorio()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS>();
            return query.Select(o => (int?)o.QuantidadeMaximaRegistrosRelatorios).FirstOrDefault() ?? 0;
        }

        public bool NaoUtilizarRegraEntradaDocumentoGrupoNCM()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS>();
            return query.Select(o => o.NaoUtilizarRegraEntradaDocumentoGrupoNCM).FirstOrDefault();
        }

        public T ObterConfiguracaoPorNomePropriedade<T>(string nomeCampo)
        {
            try
            {
                var consultaConfiguracaoEmbarcador = this.SessionNHiBernate.CreateQuery($"select {nomeCampo} from ConfiguracaoEmbarcador"); 
                return consultaConfiguracaoEmbarcador.UniqueResult<T>();
            }
            catch
            {
                return default(T);
            }
        }

        #endregion Métodos Públicos
    }
}
