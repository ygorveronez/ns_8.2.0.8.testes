using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoAPIConexaoGeradorExcelKMM : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAPIConexaoGeradorExcelKMM>
    {
        public ConfiguracaoAPIConexaoGeradorExcelKMM(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAPIConexaoGeradorExcelKMM BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAPIConexaoGeradorExcelKMM> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAPIConexaoGeradorExcelKMM>();

            return query.FirstOrDefault();
        }
    }
}
