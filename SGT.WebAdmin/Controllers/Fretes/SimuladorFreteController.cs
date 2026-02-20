using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/SimuladorFrete", "Fretes/TabelaFrete")]
    public class SimuladorFreteController : BaseController
    {
		#region Construtores

		public SimuladorFreteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(new Models.Grid.Grid());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar a simulação de frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SimularCalculoFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.CotacaoFreteCarregamento cotacaoFreteCarregamento = ObterCotacaoFreteCarregamento();
                Dominio.Entidades.RotaFrete rotaFrete = ObterRotaFrete(cotacaoFreteCarregamento, unitOfWork);

                if (rotaFrete?.Quilometros > 0m)
                    cotacaoFreteCarregamento.Distancia = (int)rotaFrete.Quilometros;

                string msgFrete = string.Empty;
                List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete> listaDadosCalculoFrete = Servicos.Embarcador.Carga.Frete.CalcularFreteSimulacao(cotacaoFreteCarregamento, out msgFrete, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                if ((listaDadosCalculoFrete?.Count ?? 0) == 0)
                    throw new ControllerException("Não foi localizada uma tabela de frete compatível com as configurações informadas.");

                var itens = (
                    from dadosCalculoFrete in listaDadosCalculoFrete
                    select new
                    {
                        DescricaoRota = rotaFrete?.Descricao ?? dadosCalculoFrete.TabelaFrete?.DescricaoTipoTabelaFrete ?? "",
                        Distancia = cotacaoFreteCarregamento.Distancia.ToString("n0"),
                        Tabela = dadosCalculoFrete.TabelaFrete?.Descricao ?? "",
                        ValorFrete = dadosCalculoFrete.ValorFrete.ToString("c2")
                    }
                ).ToList();

                return new JsonpResult(itens);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao simular o calculo de frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.CotacaoFreteCarregamento ObterCotacaoFreteCarregamento()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.CotacaoFreteCarregamento()
            {
                Filial = Request.GetIntParam("Filial"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                ModeloVeicularCarga = Request.GetIntParam("ModeloVeicular"),
                Origem = Request.GetIntParam("Origem"),
                Destinos = Request.GetListParam<int>("Localidades"),
                PesoBruto = Request.GetDecimalParam("PesoBruto"),
                Distancia = Request.GetIntParam("Distancia")
            };
        }

        private Dominio.Entidades.RotaFrete ObterRotaFrete(Dominio.ObjetosDeValor.Embarcador.Carga.CotacaoFreteCarregamento cotacaoFreteCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRotaFrete filtrosPesquisaRotaFrete = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRotaFrete()
            {
                Ativo = SituacaoAtivoPesquisa.Ativo,
                CodigoOrigem = cotacaoFreteCarregamento.Origem,
                CodigosCidadeDestinatario = cotacaoFreteCarregamento.Destinos,
                SituacaoRoteirizacao = SituacaoRoteirizacao.Concluido
                // Nçao vamos filrar.. pois pode não ter para o tipo de operação e devemos achar outra...
                //CodigoTipoOperacao = cotacaoFreteCarregamento.TipoOperacao
            };

            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            List<Dominio.Entidades.RotaFrete> rotasFrete = repositorioRotaFrete.Consultar(filtrosPesquisaRotaFrete, "Codigo", "asc", 0, 1000);

            List<Dominio.Entidades.RotaFrete> rotasFreteTemp = rotasFrete;
            if (cotacaoFreteCarregamento.TipoOperacao > 0)
                rotasFreteTemp = (from obj in rotasFrete
                                  where (obj.TipoOperacao?.Codigo ?? 0) == cotacaoFreteCarregamento.TipoOperacao
                                  select obj).ToList();

            if (rotasFreteTemp.Count == 0)
                rotasFreteTemp = rotasFrete;

            rotasFrete = rotasFreteTemp;

            //#48952 - Aqui, vamos verificar se tem destinos iguais. ou seja, todos os destinos da rota os mesmos da simulação.
            if (rotasFrete.Count > 0 && (cotacaoFreteCarregamento.Destinos?.Count ?? 0) > 0)
            {
                List<Dominio.Entidades.RotaFrete> temp = (from obj in rotasFrete
                                                          where cotacaoFreteCarregamento.Destinos.OrderBy(x => x).Except(obj.Localidades.Select(x => x.Localidade.Codigo).OrderBy(y => y).ToList()).Count() == 0
                                                          select obj).ToList();

                //if (temp.Count > 0) // Se os destinos forem diferentes.. deve pegar tabela por cliente...
                rotasFrete = temp;
            }

            if (rotasFrete.Count >= 1)
            {
                List<Dominio.Entidades.RotaFrete> rotasFiltradas = repositorioRotaFrete.BuscarRotasFreteFiltradas(rotasFrete, codigoEmpresa: 0);
                rotasFiltradas = rotasFiltradas.OrderBy(x => x.Quilometros).ToList();
                //#66035 - Rota de 3500 km esta obtendo uma RotaFrete de 2600 km ocasionando divergência na simulação.. vamos obter a maior km menor que o km roteirizado.
                // COm isso vamos obter a rota mais aproximada da distancia roteirizada.
                Dominio.Entidades.RotaFrete retornar = rotasFiltradas.FirstOrDefault();
                foreach (Dominio.Entidades.RotaFrete rotaFrete in rotasFiltradas)
                {
                    if (rotaFrete.Quilometros <= cotacaoFreteCarregamento.Distancia)
                        retornar = rotaFrete;
                }
                return retornar;
            }

            return null;
        }

        #endregion Métodos Privados
    }
}
