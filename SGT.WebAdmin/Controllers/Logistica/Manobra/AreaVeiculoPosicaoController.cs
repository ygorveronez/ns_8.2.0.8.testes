using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/AreaVeiculoPosicao")]
    public class AreaVeiculoPosicaoController : BaseController
    {
		#region Construtores

		public AreaVeiculoPosicaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> BaixarQrCodePosicao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPosicao = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.AreaVeiculoPosicao repositorioPosicao = new Repositorio.Embarcador.Logistica.AreaVeiculoPosicao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao posicao = repositorioPosicao.BuscarPorCodigo(codigoPosicao);

                if (posicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                byte[] pdf = new Servicos.Embarcador.Logistica.AreaVeiculo().ObterPdfQRCodeAreaVeiculoPosicao(posicao);

                return Arquivo(pdf, "application/pdf", $"QR Code {posicao.DescricaoAcao}.pdf");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao baixar o QR Code.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAreaVeiculoPosicao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAreaVeiculoPosicao()
            {
                CodigoAreaPosicao = Request.GetIntParam("AreaVeiculo"),
                CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                CodigoTipoRetornoCarga = Request.GetIntParam("TipoRetornoCarga"),
                Descricao = Request.GetStringParam("Descricao"),
                TipoAreaVeiculo = Request.GetNullableEnumParam<TipoAreaVeiculo>("TipoAreaVeiculo")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("QRCode", false);
                grid.AdicionarCabecalho("Descrição", "DescricaoPosicao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Area", "AreaVeiculo", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 15, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAreaVeiculoPosicao filtrosPesquisa = ObterFiltrosPesquisa();
                int codigoPreCarga = Request.GetIntParam("PreCarga");

                if (codigoPreCarga > 0)
                {
                    Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork).BuscarPorCodigo(codigoPreCarga);

                    if (preCarga?.Filial != null)
                    {
                        Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork).BuscarPorFilial(preCarga.Filial.Codigo);

                        filtrosPesquisa.CodigoCentroCarregamento = centroCarregamento?.Codigo ?? 0;
                    }
                    else
                        filtrosPesquisa.CodigoCentroCarregamento = 0;
                }

                int totalRegistros = 0;
                List<Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao> listaAreaVeiculoPosicao = null;

                if ((codigoPreCarga > 0) && (filtrosPesquisa.CodigoCentroCarregamento == 0))
                    listaAreaVeiculoPosicao = new List<Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao>();
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                    Repositorio.Embarcador.Logistica.AreaVeiculoPosicao repositorio = new Repositorio.Embarcador.Logistica.AreaVeiculoPosicao(unitOfWork);
                    totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                    listaAreaVeiculoPosicao = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao>();
                }

                var listaAreaVeiculoPosicaoRetornar = (
                    from areaVeiculoPosicao in listaAreaVeiculoPosicao
                    select new
                    {
                        areaVeiculoPosicao.Codigo,
                        Descricao = $"{areaVeiculoPosicao.AreaVeiculo.Descricao} - {areaVeiculoPosicao.Descricao}",
                        areaVeiculoPosicao.QRCode,
                        DescricaoPosicao = areaVeiculoPosicao.Descricao,
                        AreaVeiculo = areaVeiculoPosicao.AreaVeiculo.Descricao,
                        Tipo = areaVeiculoPosicao.AreaVeiculo.Tipo.Obterdescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaAreaVeiculoPosicaoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoPosicao")
                return "Descricao";

            if (propriedadeOrdenar == "AreaVeiculo")
                return "AreaVeiculo.Descricao";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
