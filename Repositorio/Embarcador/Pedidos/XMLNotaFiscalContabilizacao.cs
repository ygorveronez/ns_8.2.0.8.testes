using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class XMLNotaFiscalContabilizacao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalContabilizacao>
    {
        public XMLNotaFiscalContabilizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalContabilizacao BuscarPorXMLNotaFiscal(int codigoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalContabilizacao>();
            var result = from obj in query where obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalContabilizacao> BuscarPorXMLNotasFiscais(List<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalContabilizacao> result = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalContabilizacao>();
            int take = 1000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalContabilizacao>();
                query = query.Where(o => tmp.Contains(o.XMLNotaFiscal.Codigo));
                //query = query.Fetch(obj => obj.Canhoto)
                //    .Fetch(obj => obj.Destinatario)
                //    .Fetch(obj => obj.Emitente);

                result.AddRange(query.ToList());
                start += take;
            }
            return result;
        }
    }
}
