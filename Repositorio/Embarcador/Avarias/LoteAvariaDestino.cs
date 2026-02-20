using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Avarias
{
    public class LoteAvariaDestino : RepositorioBase<Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino>
    {
        public LoteAvariaDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino> BuscarPorLote(int codigoLote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino>();

            var result = from obj in query where obj.Lote.Codigo == codigoLote select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino> Consultar(int codigoLote, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino>();

            var result = from obj in query where obj.Lote.Codigo == codigoLote select obj;

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(int codigoLote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino>();

            var result = from obj in query where obj.Lote.Codigo == codigoLote select obj;

            return result.Count();
        }

        public bool ExisteProdutoNaoInformado(int codigoLote)
        {            
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino>();
            var queryProdutosAvariados = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>();

            var result = from obj in query where obj.Lote.Codigo == codigoLote select obj;
            var resultProdutosAvariados = from obj in queryProdutosAvariados where obj.SolicitacaoAvaria.Lote.Codigo == codigoLote select obj;

            resultProdutosAvariados = resultProdutosAvariados.Where(o => !result.Where(a => a.ProdutoEmbarcador.Codigo == o.ProdutoEmbarcador.Codigo).Any());

            return resultProdutosAvariados.Any();
        }

        public int QuantidadeProdutoInformado(int codigoLote, int codigoProduto, int codigoAvariaDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.LoteAvariaDestino>();

            var result = from obj in query where obj.Lote.Codigo == codigoLote && obj.ProdutoEmbarcador.Codigo == codigoProduto && obj.Codigo != codigoAvariaDestino select obj.Quantidade;

            return result.Count() > 0 ? result.Sum(o => o) : 0;
        }

        #endregion
    }
}
