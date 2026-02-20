using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio
{
    public class RelacaoCTesEntreguesCTes : RepositorioBase<Dominio.Entidades.RelacaoCTesEntreguesCTes>
    {

        public RelacaoCTesEntreguesCTes(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.RelacaoCTesEntreguesCTes BuscarPorCodigo(int codigo, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RelacaoCTesEntreguesCTes>();

            var result = from obj in query where obj.Codigo == codigo && obj.RelacaoCTesEntregues.Empresa.Codigo == empresa select obj;

            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.RelacaoCTesEntreguesCTes> BuscarPorRelacao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RelacaoCTesEntreguesCTes>();

            var result = from obj in query where obj.RelacaoCTesEntregues.Codigo == codigo  select obj;

            return result.ToList();
        }


        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesOrdenados(int codigo, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RelacaoCTesEntreguesCTes>();

            var result = from obj in query where obj.RelacaoCTesEntregues.Codigo == codigo && obj.RelacaoCTesEntregues.Empresa.Codigo == empresa select obj;

            result = result.OrderBy(o => o.Ordem);

            return result
                .Select(o => o.CTe)
                .Fetch(o => o.Remetente)
                .Fetch(o => o.Destinatario)
                .Fetch(o => o.Expedidor)
                .Fetch(o => o.OutrosTomador)
                .Fetch(o => o.Recebedor)
                .Fetch(o => o.Documentos)
                .ToList();
        }
     }
}
