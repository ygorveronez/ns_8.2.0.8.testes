using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class PropostaItem : RepositorioBase<Dominio.Entidades.PropostaItem>
    {
        public PropostaItem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.PropostaItem BuscaPorCodigo(int codigo, int codigoProposta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PropostaItem>();

            var result = from obj in query where obj.Codigo == codigo && obj.Proposta.Codigo == codigoProposta select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.PropostaItem> BuscaPorProposta(int codigoProposta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PropostaItem>();

            var result = from obj in query where obj.Proposta.Codigo == codigoProposta select obj;

            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.VisualizacaoPropostaItem> RelatorioPorCodigo(int codigoProposta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PropostaItem>();

            var result = from obj in query where obj.Proposta.Codigo == codigoProposta select obj;

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.VisualizacaoPropostaItem()
            {
                Descricao = o.Descricao,
                Valor = o.Valor,
                Tipo = o.Tipo == Dominio.Enumeradores.TipoItemProposta.Percentual ? 1 : 0
            }).ToList();
        }
    }
}
