using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Relatorios
{
    public class AutomatizacaoGeracaoRelatorioConfiguracaoFTP : RepositorioBase<Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioConfiguracaoFTP>
    {
        #region Construtores

        public AutomatizacaoGeracaoRelatorioConfiguracaoFTP(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioConfiguracaoFTP BuscarPorAutomatizacao(int codigoAutomatizacao)
        {
            var consultaConfiguracaoFTP = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioConfiguracaoFTP>()
                .Where(o => o.AutomatizacaoGeracaoRelatorio.Codigo == codigoAutomatizacao);

            return consultaConfiguracaoFTP.FirstOrDefault();
        }

        public void DeletarPorAutomatizacao(int codigoAutomatizacao)
        {
            SessionNHiBernate
                .CreateQuery("delete from AutomatizacaoGeracaoRelatorioConfiguracaoFTP ConfiguracaoFTP where ConfiguracaoFTP.AutomatizacaoGeracaoRelatorio.Codigo = :codigoAutomatizacao")
                .SetInt32("codigoAutomatizacao", codigoAutomatizacao)
                .ExecuteUpdate();
        }

        #endregion Métodos Públicos
    }
}
