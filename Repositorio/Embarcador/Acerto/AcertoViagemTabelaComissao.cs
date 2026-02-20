using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class AcertoViagemTabelaComissao : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoViagemTabelaComissao>
    {
        public AcertoViagemTabelaComissao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.AcertoViagemTabelaComissao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagemTabelaComissao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Acerto.AcertoViagemTabelaComissao BuscarPorAcerto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagemTabelaComissao>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoViagemTabelaComissao> BuscarPorMedia(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagemTabelaComissao>();
            var result = from obj in query where obj.TabelaComissaoMotoristaMedia.Codigo == codigo select obj;
            return result.ToList();
        }

    }
}
