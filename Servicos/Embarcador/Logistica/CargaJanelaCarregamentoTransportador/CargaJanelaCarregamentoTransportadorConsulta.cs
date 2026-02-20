using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public sealed class CargaJanelaCarregamentoTransportadorConsulta
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public CargaJanelaCarregamentoTransportadorConsulta(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaJanelaCarregamentoTransportadorValorFrete ObterCargaJanelaCarregamentoTransportadorComMenorLance(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, bool considerarCargasVinculadas)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorInteressadosComValorInformado(cargaJanelaCarregamento.Codigo);

            if (cargasJanelaCarregamentoTransportador.Count == 0)
                return null;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete repositorioCargaJanelaCarregamentoTransportadorComponenteFrete = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportadorVinculadas;
       
            if (considerarCargasVinculadas)
            {
                Repositorio.Embarcador.Cargas.CargaVinculada repositorioCargaVinculada = new Repositorio.Embarcador.Cargas.CargaVinculada(_unitOfWork);
                List<int> codigosCargasVinculadas = repositorioCargaVinculada.BuscarCodigosCargasPorCarga(cargaJanelaCarregamento.Carga.Codigo);
                cargasJanelaCarregamentoTransportadorVinculadas = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargasESituacao(codigosCargasVinculadas, SituacaoCargaJanelaCarregamentoTransportador.ComInteresse);
            }
            else
                cargasJanelaCarregamentoTransportadorVinculadas = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            List<int> codigosCargaJanelaCarregamentoTransportador = cargasJanelaCarregamentoTransportador.Select(janelaTransportador => janelaTransportador.Codigo).Concat(cargasJanelaCarregamentoTransportadorVinculadas.Select(janelaTransportadorVinculada => janelaTransportadorVinculada.Codigo)).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete> componentesFrete = repositorioCargaJanelaCarregamentoTransportadorComponenteFrete.BuscarPorCargasJanelaCarregamentoTransportador(codigosCargaJanelaCarregamentoTransportador);
            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaJanelaCarregamentoTransportadorValorFrete> lancesTransportadores = new List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaJanelaCarregamentoTransportadorValorFrete>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargasJanelaCarregamentoTransportador)
            {
                Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaJanelaCarregamentoTransportadorValorFrete transportadorValorFrete = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaJanelaCarregamentoTransportadorValorFrete()
                {
                    CargaJanelaCarregamentoTransportador = cargaJanelaCarregamentoTransportador,
                    ValorFrete = cargaJanelaCarregamentoTransportador.ValorFreteTransportador,
                    ValorComponentesFrete = componentesFrete.Where(componente => componente.CargaJanelaCarregamentoTransportador.Codigo == cargaJanelaCarregamentoTransportador.Codigo).Sum(componente => componente.ValorComponente)
                };

                if (considerarCargasVinculadas)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> janelasVinculadas = cargasJanelaCarregamentoTransportadorVinculadas.Where(janelaTransportador => janelaTransportador.Transportador.Codigo == cargaJanelaCarregamentoTransportador.Transportador.Codigo).ToList();
                    List<int> codigosJanelasVinculadas = janelasVinculadas.Select(janelaTransportador => janelaTransportador.Codigo).ToList();

                    transportadorValorFrete.ValorFrete += janelasVinculadas.Sum(janelaTransportador => janelaTransportador.ValorFreteTransportador);
                    transportadorValorFrete.ValorComponentesFrete += componentesFrete.Where(componente => codigosJanelasVinculadas.Contains(componente.CargaJanelaCarregamentoTransportador.Codigo)).Sum(componente => componente.ValorComponente);
                }

                lancesTransportadores.Add(transportadorValorFrete);
            }

            decimal menorLance = lancesTransportadores.Min(lance => lance.ValorTotalFrete);
            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaJanelaCarregamentoTransportadorValorFrete> transportadoresComMenorLance = lancesTransportadores.Where(lance => lance.ValorTotalFrete == menorLance).ToList();

            if (transportadoresComMenorLance.Count == 1)
                return transportadoresComMenorLance.FirstOrDefault();

            int maiorPrioridadePorModeloCarroceria = transportadoresComMenorLance.Max(lance => lance.CargaJanelaCarregamentoTransportador.DadosTransporte?.ModeloCarroceria?.Prioridade ?? 0);
            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaJanelaCarregamentoTransportadorValorFrete> transportadoresComMenorLancePorModeloCarroceriaPrioritario = null;

            if (maiorPrioridadePorModeloCarroceria > 0)
            {
                transportadoresComMenorLancePorModeloCarroceriaPrioritario = transportadoresComMenorLance.Where(lance => lance.CargaJanelaCarregamentoTransportador.DadosTransporte?.ModeloCarroceria?.Prioridade == maiorPrioridadePorModeloCarroceria).ToList();

                if (transportadoresComMenorLancePorModeloCarroceriaPrioritario.Count == 1)
                    return transportadoresComMenorLancePorModeloCarroceriaPrioritario.FirstOrDefault();
            }
            else
                transportadoresComMenorLancePorModeloCarroceriaPrioritario = transportadoresComMenorLance.ToList();

            DateTime dia = DateTime.Now;
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            List<(Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaJanelaCarregamentoTransportadorValorFrete Lance, int Pontuacao)> pontuacoesDiariaPorTransportador = new List<(Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaJanelaCarregamentoTransportadorValorFrete Lance, int Pontuacao)>();

            foreach (Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaJanelaCarregamentoTransportadorValorFrete transportadorValorFrete in transportadoresComMenorLancePorModeloCarroceriaPrioritario)
            {
                int pontuacaoDiaria = transportadorValorFrete.CargaJanelaCarregamentoTransportador.Transportador.Pontuacao;

                if (pontuacaoDiaria > 0)
                    pontuacaoDiaria += 100000; // regra para garantir a que o transportador com pontuação tenha preferencia

                if (cargaJanelaCarregamento.CentroCarregamento.PontuacaoDescontarTransportadorPorEscolhaAutomaticaCotacao > 0)
                {
                    int totalCargasEscolhidas = repositorioCargaJanelaCarregamento.ContarCargasPorCotacaoGanhaAutomaticamente(transportadorValorFrete.CargaJanelaCarregamentoTransportador.Transportador.Codigo, dia);
                    pontuacaoDiaria -= (totalCargasEscolhidas * cargaJanelaCarregamento.CentroCarregamento.PontuacaoDescontarTransportadorPorEscolhaAutomaticaCotacao);
                }

                pontuacoesDiariaPorTransportador.Add(ValueTuple.Create(transportadorValorFrete, pontuacaoDiaria));
            }

            int maiorPontuacaoDiaria = pontuacoesDiariaPorTransportador.Max(pontuacaoDiaria => pontuacaoDiaria.Pontuacao);
            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaJanelaCarregamentoTransportadorValorFrete> transportadoresComMaiorPontuacaoDiaria = pontuacoesDiariaPorTransportador.Where(pontuacaoDiaria => pontuacaoDiaria.Pontuacao == maiorPontuacaoDiaria).Select(pontuacaoDiaria => pontuacaoDiaria.Lance).ToList();

            if (transportadoresComMaiorPontuacaoDiaria.Count == 1)
                return transportadoresComMaiorPontuacaoDiaria.FirstOrDefault();

            int maiorPontuacao = transportadoresComMaiorPontuacaoDiaria.Max(lance => lance.CargaJanelaCarregamentoTransportador.Transportador.Pontuacao);

            return transportadoresComMaiorPontuacaoDiaria.Where(lance => lance.CargaJanelaCarregamentoTransportador.Transportador.Pontuacao == maiorPontuacao).OrderBy(lance => lance.CargaJanelaCarregamentoTransportador.DataInteresse).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> ObterCargasJanelaCarregamentoTransportador(int codigoCarga, Dominio.Entidades.Empresa empresa, bool retornarCargasOriginais)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaVinculada repositorioCargaVinculada = new Repositorio.Embarcador.Cargas.CargaVinculada(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

            if (carga == null)
                return new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            List<int> codigosCargas = new List<int>() { carga.Codigo };
            List<int> codigosEmpresas = new List<int>() { empresa.Codigo };

            if (carga.CargaAgrupada && retornarCargasOriginais)
                codigosCargas.AddRange(repositorioCarga.BuscarCodigosCargasOriginais(carga.Codigo));

            codigosCargas.AddRange(repositorioCargaVinculada.BuscarCodigosCargasPorCarga(carga.Codigo));

            if (empresa.Filiais.ToList().Count > 0)
                codigosEmpresas.AddRange(empresa.Filiais.Select(o => o.Codigo).Distinct().ToList());

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador =  repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargasETransportadores(codigosCargas.Distinct().ToList(), codigosEmpresas);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportadorRetornar = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            foreach (int codigoCargaFiltrar in codigosCargas)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorRetornar = cargasJanelaCarregamentoTransportador
                    .Where(janelaTransportador => janelaTransportador.CargaJanelaCarregamento.Carga.Codigo == codigoCargaFiltrar)
                    .OrderByDescending(janelaTransportador => janelaTransportador.Transportador.Codigo == empresa.Codigo)
                    .FirstOrDefault();

                if (cargaJanelaCarregamentoTransportadorRetornar != null)
                    cargasJanelaCarregamentoTransportadorRetornar.Add(cargaJanelaCarregamentoTransportadorRetornar);
            }

            return cargasJanelaCarregamentoTransportadorRetornar;

        }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> ObterCargasJanelaCarregamentoTransportadorTerceiro(int codigoCarga, Dominio.Entidades.Cliente transportador, bool retornarCargasOriginais)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaVinculada repositorioCargaVinculada = new Repositorio.Embarcador.Cargas.CargaVinculada(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

            if (carga == null)
                return new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            List<int> codigosCargas = new List<int>() { carga.Codigo };

            if (carga.CargaAgrupada && retornarCargasOriginais)
                codigosCargas.AddRange(repositorioCarga.BuscarCodigosCargasOriginais(carga.Codigo));

            codigosCargas.AddRange(repositorioCargaVinculada.BuscarCodigosCargasPorCarga(carga.Codigo));

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargasETransportadorTerceiro(codigosCargas.Distinct().ToList(), transportador.CPF_CNPJ);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportadorRetornar = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            foreach (int codigoCargaFiltrar in codigosCargas)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorRetornar = cargasJanelaCarregamentoTransportador
                    .Where(janelaTransportador => janelaTransportador.CargaJanelaCarregamento.Carga.Codigo == codigoCargaFiltrar)
                    .OrderByDescending(janelaTransportador => janelaTransportador.Terceiro.CPF_CNPJ == transportador.CPF_CNPJ)
                    .FirstOrDefault();

                if (cargaJanelaCarregamentoTransportadorRetornar != null)
                    cargasJanelaCarregamentoTransportadorRetornar.Add(cargaJanelaCarregamentoTransportadorRetornar);
            }

            return cargasJanelaCarregamentoTransportadorRetornar;

        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> ObterCargasJanelaCarregamentoTransportadorOrdenadasPorPontuacao(List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> lista)
        {
            DateTime dia = DateTime.Now;
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            List<(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador janelaTransportador, int Pontuacao)> listaPontuacaoDiariaPorTransportador = new List<(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador janelaTransportador, int Pontuacao)>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in lista)
            {
                int pontuacaoDiaria = cargaJanelaCarregamentoTransportador.Transportador.Pontuacao;

                if (pontuacaoDiaria > 0)
                    pontuacaoDiaria += 100000; // regra para garantir a que o transportador com pontuação tenha preferencia

                if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento.PontuacaoDescontarTransportadorPorEscolhaAutomaticaCotacao > 0)
                {
                    int totalCargasEscolhidas = repositorioCargaJanelaCarregamento.ContarCargasPorCotacaoGanhaAutomaticamente(cargaJanelaCarregamentoTransportador.Transportador.Codigo, dia);
                    pontuacaoDiaria -= (totalCargasEscolhidas * cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento.PontuacaoDescontarTransportadorPorEscolhaAutomaticaCotacao);
                }

                listaPontuacaoDiariaPorTransportador.Add(ValueTuple.Create(cargaJanelaCarregamentoTransportador, pontuacaoDiaria));
            }

            return listaPontuacaoDiariaPorTransportador.OrderByDescending(o => o.Pontuacao).Select(o => o.janelaTransportador).ToList();
        }

        #endregion Métodos Públicos
    }
}
