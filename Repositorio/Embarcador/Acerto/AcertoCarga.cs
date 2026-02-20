using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Acerto
{
    public class AcertoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>
    {
        public AcertoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.AcertoCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public decimal BuscarSomaPercentualCompartilhamentoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.AcertoViagem.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Cancelado);

            return query.Sum(o => (decimal?)o.PercentualAcerto) ?? 0m;
        }

        public bool CargaEmOutroAcerto(int codigoAcerto, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();

            query = query.Where(obj => obj.AcertoViagem.Codigo != codigoAcerto && obj.Carga.Codigo == codigoCarga && obj.AcertoViagem.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Cancelado);

            return query.Any();
        }

        public bool VerificaCargaEmAcerto(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();

            query = query.Where(o => o.Carga.Codigo == carga.Codigo && o.AcertoViagem.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Cancelado);

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Acerto.AcertoCarga BuscarPorCodigoCargaSituacao(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem situacaoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();
            var result = from obj in query where obj.Carga.Codigo == codigo && obj.AcertoViagem.Situacao == situacaoAcerto select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoCarga> BuscarPorCodigosCargaSituacao(List<int> codigos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem situacaoAcerto)
        {
            if (codigos == null || !codigos.Any())
                return new List<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();

            var result = query.Where(obj => codigos.Contains(obj.Carga.Codigo) && obj.AcertoViagem.Situacao == situacaoAcerto);

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Acerto.AcertoCarga BuscarPorCodigoCarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();
            var result = from obj in query where obj.Carga.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoCarga> BuscarPorCodigoAcerto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigo);

            return query.Fetch(o => o.Carga).ThenFetch(o => o.Veiculo)
                        .Fetch(o => o.Carga).ThenFetch(o => o.SegmentoModeloVeicularCarga)
                        .Fetch(o => o.Carga).ThenFetch(o => o.SegmentoGrupoPessoas).ToList();

        }

        public List<int> BuscarCodigosTipoOperacao(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto && obj.Carga != null && obj.Carga.TipoOperacao != null select obj;
            return result.Select(c => c.Carga.TipoOperacao.Codigo)?.ToList();
        }

        public Dominio.Entidades.Embarcador.Acerto.AcertoCarga BuscarPorCodigoAcertoECarga(int codigoAcerto, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto && obj.Carga.Codigo == codigoCarga select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarCargasDoAcerto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigo);

            return query.Select(o => o.Carga.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Acerto.AcertoCarga BuscarPorCodigoAcertoCodigoCarga(int codigoAcerto, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            if (codigoAcerto > 0)
                result = result.Where(o => o.AcertoViagem.Codigo == codigoAcerto);

            return result.FirstOrDefault();
        }

        public int QuantidadeViagensCompartilhadas(int codigoacerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoacerto && obj.PercentualAcerto < 100m select obj;
            return result.Count();
        }

        public decimal ValorICMSAcerto(int codigoAcerto, bool buscarSemOcorrencia, int codigoVeiculo)
        {
            var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resultCarga = from obj in queryCarga where obj.AcertoViagem.Codigo == codigoAcerto && !obj.CargaFracionada && obj.PercentualAcerto == 100m select obj;

            if (codigoVeiculo > 0)
                resultCarga = from obj in resultCarga where obj.Carga.Veiculo.Codigo == codigoVeiculo select obj;

            var resultCargaCTe = resultCarga.Join(queryCargaCTe, vei => vei.Carga.Codigo, emp => emp.Carga.Codigo, (vei, emp) => emp);

            if (buscarSemOcorrencia)
                resultCargaCTe = from obj in resultCargaCTe where obj.CTe.CST != "60" && obj.CTe.TipoCTE != Dominio.Enumeradores.TipoCTE.Anulacao && obj.CargaCTeComplementoInfo == null && obj.CTe.Status == "A" select obj;
            else
                resultCargaCTe = from obj in resultCargaCTe where obj.CTe.CST != "60" && obj.CTe.TipoCTE != Dominio.Enumeradores.TipoCTE.Anulacao && obj.CargaCTeComplementoInfo != null && obj.CTe.Status == "A" select obj;

            return resultCargaCTe.Sum(o => (decimal?)o.CTe.ValorICMS) ?? 0m;
        }

        public decimal ValorICMSFracionadoAcerto(int codigoAcerto, int codigoVeiculo)
        {
            var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();

            queryCarga = queryCarga.Where(obj => obj.AcertoViagem.Codigo == codigoAcerto && (obj.CargaFracionada || obj.PercentualAcerto < 100m) && obj.ValorICMSCarga > 0m);

            if (codigoVeiculo > 0)
                queryCarga = queryCarga.Where(obj => obj.Carga.Veiculo.Codigo == codigoVeiculo);

            return queryCarga.Sum(o => (decimal?)o.ValorICMSCarga) ?? 0m;
        }

        public decimal ValorComponenteFreteVeiculo(int codigoAcerto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponenteFrete, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

            query = query.Where(obj => obj.Carga.AcertosViagem.Any(o => o.AcertoViagem.Codigo == codigoAcerto) && obj.Carga.Veiculo.Codigo == codigoVeiculo && obj.ComponenteFrete.TipoComponenteFrete == tipoComponenteFrete);

            return query.Sum(o => (decimal?)o.ValorComponente) ?? 0m;
        }

        public decimal ValorFreteLiquido(int codigoAcerto, int codigoVeiculo)
        {
            var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCarga = queryCarga.Where(obj => obj.AcertoViagem.Codigo == codigoAcerto && !obj.CargaFracionada && obj.PercentualAcerto == 100m && obj.ValorBrutoCarga > 0);

            var resultCargaCTe = queryCarga.Join(queryCargaCTe, vei => vei.Carga.Codigo, emp => emp.Carga.Codigo, (vei, emp) => emp)
                                            .Where(o => o.CargaCTeComplementoInfo == null && o.CTe.Status == "A" && o.CTe.TipoCTE != Dominio.Enumeradores.TipoCTE.Anulacao);

            if (codigoVeiculo > 0)
                resultCargaCTe = resultCargaCTe.Where(o => o.Carga.Veiculo.Codigo == codigoVeiculo);

            return resultCargaCTe.Sum(o => (decimal?)o.CTe.ValorFrete) ?? 0m;

        }

        public decimal ValorFreteLiquidoComplementar(int codigoAcerto, int codigoVeiculo)
        {
            var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOcorrencia>();
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resultCarga = from obj in queryCarga where obj.AcertoViagem.Codigo == codigoAcerto select obj;

            var resultCargaCTe = resultCarga.Join(queryCargaCTe, vei => vei.CargaOcorrencia.Carga.Codigo, emp => emp.Carga.Codigo, (vei, emp) => emp)
                                            .Where(o => o.CargaCTeComplementoInfo != null);

            if (codigoVeiculo > 0)
                resultCargaCTe = resultCargaCTe.Where(o => o.Carga.Veiculo.Codigo == codigoVeiculo);

            return resultCargaCTe.Sum(o => (decimal?)o.CTe.ValorFrete) ?? 0m;
        }

        public decimal ValorFreteLiquidoFracionadoAcerto(int codigoAcerto)
        {
            var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();

            queryCarga = queryCarga.Where(obj => obj.AcertoViagem.Codigo == codigoAcerto && (obj.CargaFracionada || obj.PercentualAcerto < 100m));

            return queryCarga.Sum(o => (decimal?)o.ValorBrutoCarga) ?? 0m;
        }

        public decimal ValorComponenteFreteVeiculo(int codigoAcerto, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

            query = query.Where(obj => obj.Carga.AcertosViagem.Any(o => o.AcertoViagem.Codigo == codigoAcerto) && obj.Carga.Veiculo.Codigo == codigoVeiculo);

            return query.Sum(o => (decimal?)o.ValorComponente) ?? 0m;
        }

        public decimal ValorComponenteFretePorTipoAcerto(int codigoAcerto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoAcertoViagem tipoCampo, bool semPedagio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

            query = query.Where(obj => obj.Carga.AcertosViagem.Any(o => o.AcertoViagem.Codigo == codigoAcerto) && obj.ComponenteFrete.TipoCampoAcertoViagem == tipoCampo);

            if (semPedagio)
                query = query.Where(o => o.ComponenteFrete.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO);

            return query.Sum(o => (decimal?)o.ValorComponente) ?? 0m;
        }

        public decimal ValorComponenteFrete(int codigoAcerto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponenteFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

            query = query.Where(obj => obj.Carga.AcertosViagem.Any(o => o.AcertoViagem.Codigo == codigoAcerto) && obj.ComponenteFrete.TipoComponenteFrete == tipoComponenteFrete);

            return query.Sum(o => (decimal?)o.ValorComponente) ?? 0m;
        }

        public decimal ValorBrutoCargasVeiculo(int codigoAcerto, int codigoVeiculo)
        {
            var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();

            queryCarga = queryCarga.Where(obj => obj.AcertoViagem.Codigo == codigoAcerto && obj.Carga.Veiculo.Codigo == codigoVeiculo);

            return queryCarga.Sum(o => (decimal?)o.ValorBrutoCarga) ?? 0m;
        }


        public decimal ValorICMSCargasVeiculo(int codigoAcerto, int codigoVeiculo)
        {
            var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();

            queryCarga = queryCarga.Where(obj => obj.AcertoViagem.Codigo == codigoAcerto && obj.Carga.Veiculo.Codigo == codigoVeiculo);

            return queryCarga.Sum(o => (decimal?)o.ValorICMSCarga) ?? 0m;
        }

        public List<int> BuscarNumeroAcertoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.AcertoViagem.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Cancelado);

            return query.Select(o => o.AcertoViagem.Numero).ToList();
        }

        public List<int> ObterDocumentosComCanhotoPendente(int codigoAcerto)
        {
            var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resultCarga = from obj in queryCarga where obj.AcertoViagem.Codigo == codigoAcerto && !obj.CargaFracionada && obj.PercentualAcerto == 100m select obj;

            var resultCargaCTe = resultCarga.Join(queryCargaCTe, vei => vei.Carga.Codigo, emp => emp.Carga.Codigo, (vei, emp) => emp);
            resultCargaCTe = resultCargaCTe.Where(o => o.CTe.XMLNotaFiscais.Any(xml => xml.Canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente));
            return resultCargaCTe.Select(o => o.CTe.Numero).ToList();
        }

        public List<string> ObterCargasComPalletPendente(int codigoAcerto)
        {
            var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();
            var queryDevolucaoPallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>();
            var resultCarga = from obj in queryCarga where obj.AcertoViagem.Codigo == codigoAcerto && !obj.CargaFracionada && obj.PercentualAcerto == 100m select obj;

            var resultCargaCTe = resultCarga.Join(queryDevolucaoPallet, vei => vei.Carga.Codigo, emp => emp.CargaPedido.Carga.Codigo, (vei, emp) => emp);

            resultCargaCTe = resultCargaCTe.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.AgEntrega);
            return resultCargaCTe.Select(o => o.CargaPedido.Carga.CodigoCargaEmbarcador).ToList();
        }
    }
}