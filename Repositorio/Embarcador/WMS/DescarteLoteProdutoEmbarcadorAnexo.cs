using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.WMS
{
    public class DescarteLoteProdutoEmbarcadorAnexo : RepositorioBase<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo>
    {
        public DescarteLoteProdutoEmbarcadorAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo> BuscarPorDescarteLote(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo>();
            var result = from obj in query where obj.Lote.Codigo == codigo && obj.Ativo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo> Consultar(int descarteLote, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo>();
            var result = from obj in query where obj.Lote.Codigo == descarteLote && obj.Ativo select obj;

            return result
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .ToList();
        }

        public int ContarConsulta(int descarteLote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo>();
            var result = from obj in query where obj.Lote.Codigo == descarteLote && obj.Ativo select obj;

            return result.Count();
        }
    }
}
