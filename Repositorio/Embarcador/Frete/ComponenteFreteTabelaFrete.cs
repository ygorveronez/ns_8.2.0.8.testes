using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Frete
{
    public class ComponenteFreteTabelaFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete>
    {
        public ComponenteFreteTabelaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete> BuscarPorTabelaFreteParaCalculo(int tabelaFrete, bool veiculoPossuiTagValePedagio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == tabelaFrete select obj;

            if (veiculoPossuiTagValePedagio)
                result = result.Where(o => o.IgnorarComponenteQuandoVeiculoPossuiTagValePedagio == false);

            return result
                .Fetch(obj => obj.ComponenteFrete)
                .Fetch(obj => obj.ModeloDocumentoFiscalRestringirQuantidade)
                .ToList();
        }

        public bool VerificarTabelaIgnoraValorPorTagValePedagio(int tabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == tabelaFrete select obj;

            result = result.Where(o => o.IgnorarComponenteQuandoVeiculoPossuiTagValePedagio);

            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete> BuscarPorTabelaFrete(int tabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == tabelaFrete select obj;

            return result
                .Fetch(obj => obj.ComponenteFrete)
                .Fetch(obj => obj.ModeloDocumentoFiscalRestringirQuantidade)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete BuscarPorTabelaFrete(int tabelaFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponenteFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete>();
            var result = from obj in query where obj.TabelaFrete.Codigo == tabelaFrete && obj.ComponenteFrete.TipoComponenteFrete == tipoComponenteFrete select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete> BuscarCodigosPorTabelaFreteValorNaoInformadoNaTabela(int tabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete>();

            query = from obj in query where obj.TabelaFrete.Codigo == tabelaFrete && obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteTabelaFrete.ValorCalculado && !obj.ValorInformadoNaTabela.Value select obj;

            return query
                .Distinct()
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete> BuscarComponentesComValorCalculadoTipoFixoPorTabelaFrete(int codigoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete>()
                .Where(obj => obj.TabelaFrete.Codigo == codigoTabelaFrete && obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteTabelaFrete.ValorCalculado && obj.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.ValorFixo);


            return query
                .Fetch(obj => obj.ComponenteFrete)
                .Fetch(obj => obj.ModeloDocumentoFiscalRestringirQuantidade)
                .ToList();
        }

        public bool VerificarSePossuiComponentePorDocumento(int tabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete>();

            var result = from o in query
                         where o.TabelaFrete.Codigo == tabelaFrete &&
                        o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteTabelaFrete.ValorCalculado
                        && o.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete.QuantidadeDocumentos
                        && o.TipoDocumentoQuantidadeDocumentos.HasValue
                        && o.TipoDocumentoQuantidadeDocumentos.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorDocumentoEmitido
                         select o;

            return result.Any();
        }

        public List<string> BuscarDescricaoComponentesPorTabelaFrete(int tabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == tabelaFrete select obj;

            return result.Select(obj => obj.ComponenteFrete.Descricao).ToList();
        }

        #endregion
    }
}
