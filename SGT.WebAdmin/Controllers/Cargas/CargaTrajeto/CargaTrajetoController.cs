using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Cargas.MontagemCarga
{
    [CustomAuthorize("Cargas/CargaTrajeto")]
    public class CargaTrajetoController : BaseController
    {
		#region Construtores

		public CargaTrajetoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.CargaTrajeto repositorioCargaTrajeto = new Repositorio.Embarcador.Cargas.CargaTrajeto(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaTrajetoCarga repositorioCargaTrajetoCarga = new Repositorio.Embarcador.Cargas.CargaTrajetoCarga(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaTrajeto filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = ObterParametrosConsulta();

                int quantidadeRegistros = repositorioCargaTrajeto.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.CargaTrajeto> listaCargaTrajeto = quantidadeRegistros > 0 ? repositorioCargaTrajeto.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaTrajeto>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaTrajetoCarga> listaCargaTrajetoCarga = repositorioCargaTrajetoCarga.BuscarPorCargaTrajeto(listaCargaTrajeto.Select(obj => obj.Codigo).ToList());

                dynamic retorno = (from o in listaCargaTrajeto
                                  select ObterTrajeto(o, listaCargaTrajetoCarga)).ToList();

                return new JsonpResult(new
                {
                    Trajetos = retorno
                }, quantidadeRegistros);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterTrajeto(Dominio.Entidades.Embarcador.Cargas.CargaTrajeto cargaTrajeto, List<Dominio.Entidades.Embarcador.Cargas.CargaTrajetoCarga> listaTrajetoCargaCarga)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaTrajetoCarga> listaFiltrada = listaTrajetoCargaCarga.Where(obj => obj.CargaTrajeto.Codigo == cargaTrajeto.Codigo).OrderBy(obj => obj.Ordem).ToList();

            Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedido = (from obj in listaFiltrada select obj.Carga.Pedidos.FirstOrDefault()).FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.CargaPedido ultimoCargaPedido = (from obj in listaFiltrada select obj.Carga.Pedidos.LastOrDefault()).LastOrDefault();

            Dominio.Entidades.Localidade cidadeInicial = primeiroCargaPedido?.Expedidor?.Localidade ?? primeiroCargaPedido?.Pedido?.Remetente?.Localidade ?? null;
            Dominio.Entidades.Localidade cidadeFinal = ultimoCargaPedido?.Expedidor?.Localidade ?? ultimoCargaPedido?.Pedido?.Remetente?.Localidade ?? null;

            return new
            {
                KmPercorrido = ObterDescricaoKmPercorrido(listaFiltrada),
                cargaTrajeto.Codigo,
                DataInicial = (from obj in listaFiltrada where obj.Ordem == 1 select obj.Carga.DataInicioViagem)?.FirstOrDefault()?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataFinal = (from obj in listaFiltrada select obj.Carga.DataFimViagem)?.LastOrDefault()?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                CidadeInicial = cidadeInicial?.DescricaoCidadeEstado ?? string.Empty,
                CidadeFinal = cidadeFinal?.DescricaoCidadeEstado ?? string.Empty,
                TransportadorAtual = (from obj in listaFiltrada where obj.SituacaoTrajetoCarga == SituacaoTrajetoCarga.EmTransporte select obj).FirstOrDefault()?.Carga?.Empresa?.NomeFantasia ?? string.Empty,
                Etapas = (
                    from obj in listaFiltrada
                    select new
                    {
                        obj.Codigo,
                        obj.SituacaoTrajetoCarga,
                        Carga = $"Carga {obj.Carga.CodigoCargaEmbarcador}",
                        Informacoes = ObterInformacoesToolTip(obj),
                        obj.Ordem
                    }
                ).ToList()
            };
        }

        private string ObterDescricaoKmPercorrido(List<Dominio.Entidades.Embarcador.Cargas.CargaTrajetoCarga> lista)
        {
            string descricao = string.Empty;
            decimal distanciaPrevistaTotal = lista.FirstOrDefault().CargaTrajeto.QuilometragemTotal;
            decimal distanciaTotalRealizada = lista.Select(obj => obj.Carga.Monitoramento?.Sum(x => x.DistanciaRealizada)).FirstOrDefault() ?? 0m;

            if (distanciaPrevistaTotal <= 0)
                distanciaPrevistaTotal = 1;

            decimal porcentagemRealizada = distanciaTotalRealizada * 100 / distanciaPrevistaTotal;

            descricao = $"{Math.Floor(porcentagemRealizada)}% ({distanciaTotalRealizada.ToString("F")} KM de {distanciaPrevistaTotal.ToString("F")} KM)";

            return descricao;
        }

        private string ObterInformacoesToolTip(Dominio.Entidades.Embarcador.Cargas.CargaTrajetoCarga cargaTrajetoCarga)
        {
            string informacoes = string.Empty;

            if (cargaTrajetoCarga.Carga.DataInicioViagem != null || cargaTrajetoCarga.Carga.DataInicioViagemPrevista != null)
                informacoes += $"Data Início Viagem: {(cargaTrajetoCarga.Carga.DataInicioViagem ?? cargaTrajetoCarga.Carga.DataInicioViagemPrevista)?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty}</br>";
            if (cargaTrajetoCarga.Carga.DataFimViagem != null || cargaTrajetoCarga.Carga.DataFimViagemPrevista != null)
                informacoes += $"Data Fim Viagem: {(cargaTrajetoCarga.Carga.DataFimViagem ?? cargaTrajetoCarga.Carga.DataFimViagemPrevista)?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty}</br>";

            return informacoes;
        }

        private Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta ObterParametrosConsulta()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
            {
                DirecaoOrdenar = "desc",
                InicioRegistros = Request.GetIntParam("inicio"),
                LimiteRegistros = Request.GetIntParam("limite"),
                PropriedadeOrdenar = "Codigo"
            };
        }
        
        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaTrajeto ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaTrajeto()
            {
                Carga = Request.GetStringParam("Carga"),
                SituacaoTrajeto = Request.GetNullableEnumParam<SituacaoTrajeto>("SituacaoTrajeto")
            };
        }

        #endregion
    }
}
