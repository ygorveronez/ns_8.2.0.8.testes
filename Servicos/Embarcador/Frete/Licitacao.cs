using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Frete
{
    public class Licitacao
    {
        public static void ReordenarRankingLicitacao(Dominio.Entidades.Embarcador.Frete.Licitacao licitacao, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
            Repositorio.Embarcador.Frete.LicitacaoParticipacao repositorioLicitacaoParticipacao = new Repositorio.Embarcador.Frete.LicitacaoParticipacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao> licitacaoParticipacaos = repositorioLicitacaoParticipacao.BuscarPorLicitacao(licitacao.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Frete.LicitacaoParticipanteMedia> licitacaoParticipanteMedias = new List<Dominio.ObjetosDeValor.Embarcador.Frete.LicitacaoParticipanteMedia>();

            foreach (Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao in licitacaoParticipacaos)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.LicitacaoParticipanteMedia licitacaoParticipanteMedia = new Dominio.ObjetosDeValor.Embarcador.Frete.LicitacaoParticipanteMedia();
                licitacaoParticipanteMedia.licitacaoParticipacao = licitacaoParticipacao;
                int totalRegistros = 0;
                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete> lista = Servicos.Embarcador.Frete.Licitacao.ObterConsultas(agrupamentos, licitacaoParticipacao.Licitacao.TabelaFrete.Codigo, licitacaoParticipacao.Codigo, null, unitOfWork, out totalRegistros);
                int qtdTotal = (from obj in lista where obj.ValorTotalDecimal > 0 select obj).Count();
                if (qtdTotal > 0)
                {
                    decimal valorTotal = (from obj in lista select obj.ValorTotalDecimal).Sum();
                    licitacaoParticipanteMedia.valorMedio = valorTotal / qtdTotal;
                }
                licitacaoParticipanteMedias.Add(licitacaoParticipanteMedia);
            }

            licitacaoParticipanteMedias = licitacaoParticipanteMedias.OrderBy(obj => obj.valorMedio).ToList();
            int ranking = 1;
            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.LicitacaoParticipanteMedia licitacaoParticipanteMedia in licitacaoParticipanteMedias)
            {
                if (licitacaoParticipanteMedia.valorMedio > 0)
                {
                    licitacaoParticipanteMedia.licitacaoParticipacao.Ranking = ranking;
                    ranking++;
                }
                else
                    licitacaoParticipanteMedia.licitacaoParticipacao.Ranking = 0;

                repositorioLicitacaoParticipacao.Atualizar(licitacaoParticipanteMedia.licitacaoParticipacao);
            }

        }
        public static IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete> ObterConsultas(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos, int codigoTabelaFrete, int codigoLicitacaoParticipacao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Repositorio.UnitOfWork unitOfWork, out int totalRegistros)
        {
            Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repositorioTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);

            Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente()
            {
                ParametroBase = tabelaFrete.ParametroBase,
                CodigoTabelaFrete = tabelaFrete.Codigo,
                CodigoLicitacaoParticipacao = codigoLicitacaoParticipacao
            };

            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
            totalRegistros = repositorioTabelaFreteCliente.ContarConsulta(filtrosPesquisa, agrupamentos);
            IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete> lista = totalRegistros > 0 ? repositorioTabelaFreteCliente.Consultar(filtrosPesquisa, agrupamentos, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete>();

            return lista;
        }
    }
}
