using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Arquivo
{
    public sealed class ControleGeracaoArquivo : RepositorioBase<Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo>
    {
        #region Construtores

        public ControleGeracaoArquivo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo> BuscarEmGeracaoPorUsuario(int codigoUsuario)
        {
            var consultaControleGeracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo>()
                .Where(o => (o.Usuario.Codigo == codigoUsuario) && (o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoArquivo.Gerando));

            return consultaControleGeracaoArquivo.ToList();
        }

        public Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo BuscarPorCodigo(int codigo)
        {
            var consultaControleGeracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo>()
                .Where(o => o.Codigo == codigo);

            return consultaControleGeracaoArquivo.FirstOrDefault();
        }

        #endregion
    }
}
