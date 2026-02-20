using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Transportadores
{
    public class ConfiguracaoTipoOperacao : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoTipoOperacao>
    {
        public ConfiguracaoTipoOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoTipoOperacao> BuscarPorEmpresa(int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoTipoOperacao>();
            var result = from obj in query where obj.Empresa.Codigo == empresa select obj;
            return result.ToList();
        }

        public List<int> BuscarCodigosTipoOperacaoPorEmpresa(int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoTipoOperacao>();
            var result = from obj in query where obj.Empresa.Codigo == empresa select obj;
            return result.Select(o => o.Codigo).ToList();
        }
    }
}
