using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Usuarios.Colaborador
{
    public class ColaboradorSituacaoLancamentoIntegracaoArquivo : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo>
    {
        public ColaboradorSituacaoLancamentoIntegracaoArquivo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo> BuscarPorIntegracao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo>();
            var result = from obj in query where obj.ColaboradorSituacaoLancamentoIntegracao.Codigo == codigo select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }
    }
}
