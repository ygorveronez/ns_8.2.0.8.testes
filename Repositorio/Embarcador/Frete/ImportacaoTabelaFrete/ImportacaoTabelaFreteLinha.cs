using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frete.ImportacaoTabelaFrete
{
    public class ImportacaoTabelaFreteLinha : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha>
    {
        public ImportacaoTabelaFreteLinha(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha> BuscarPorImportacaoTabelaFrete(int codigoImportacaoTabelaFrete)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha>();

            query = query.Where(o => o.ImportacaoTabelaFrete.Codigo == codigoImportacaoTabelaFrete);

            return query.ToList();
        }

        public List<int> BuscarCodigosLinhasJaImportadasTabelaFrete(int codigoImportacaoTabelaFrete)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha>();

            query = query.Where(o => o.ImportacaoTabelaFrete.Codigo == codigoImportacaoTabelaFrete);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosTabelasFreteClientePermitemSolicitarAprovacao(int codigoImportacaoTabelaFrete)
        {
            var consultaImportacaoTabelaFreteLinha = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha>()
                .Where(o =>
                    o.ImportacaoTabelaFrete.Codigo == codigoImportacaoTabelaFrete &&
                    o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete.Sucesso &&
                    o.PermitirSolicitarAprovacao == true
                );

            return consultaImportacaoTabelaFreteLinha.Select(o => o.TabelaFreteCliente.Codigo).ToList();
        }

        public int contarLinhasImportadasporImportacaoTabelaFrete(int codigoImportacaoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha>();

            query = query.Where(o => o.ImportacaoTabelaFrete.Codigo == codigoImportacaoPedido && o.TabelaFreteCliente != null && o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete.Cancelado);

            return query.Count();
        }

        public int contarTodasLinhasporImportacaoTabelaFrete(int codigoImportacaoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha>();

            query = query.Where(o => o.ImportacaoTabelaFrete.Codigo == codigoImportacaoPedido && o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete.Cancelado);

            return query.Count();
        }

        public void ReprocessarLinhasPendentesImportacaoTabelaFrete(int codigoImportacao)
        {
            var query = this.SessionNHiBernate.CreateQuery("UPDATE ImportacaoTabelaFreteLinha SET Situacao = 0 WHERE ImportacaoTabelaFrete = :codigoImportacao ");
            query.SetInt32("codigoImportacao", codigoImportacao);

            query.ExecuteUpdate();
        }

        #endregion
    }
}
