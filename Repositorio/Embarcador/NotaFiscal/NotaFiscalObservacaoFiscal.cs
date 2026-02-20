using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class NotaFiscalObservacaoFiscal : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoFiscal>
    {
        public NotaFiscalObservacaoFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoFiscal BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoFiscal>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoFiscal> BuscarPorNota(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoFiscal>();
            var result = from obj in query where obj.NotaFiscal.Codigo == codigo select obj;
            return result.ToList();
        }
    }
}
