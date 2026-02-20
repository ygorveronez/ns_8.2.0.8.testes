using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Contabil
{
    [CustomAuthorize("Contabils/ImpostoValorAgregado")]
    public class ImpostoValorAgregadoController : BaseController
    {
		#region Construtores

		public ImpostoValorAgregadoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
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
                grid.AdicionarCabecalho("Código IVA", "CodigoIVA", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("% ICMS superior a 0?", "ImpostoMaiorQueZero", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Destinatário Exterior", "DestinatarioExterior", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Uso Material", "UsoMaterial", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Modelo Documento Fiscal", "ModeloDocumentoFiscal", 20, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaImpostoValorAgregado filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Contabeis.ImpostoValorAgregado repositorioImpostoValorAgregado = new Repositorio.Embarcador.Contabeis.ImpostoValorAgregado(unitOfWork);
                int totalRegistro = repositorioImpostoValorAgregado.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado> impostosValorAgregado = (totalRegistro > 0) ? repositorioImpostoValorAgregado.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado>();

                var impostosValorAgregadoRetornar = (
                    from impostoValorAgregado in impostosValorAgregado
                    select new
                    {
                        impostoValorAgregado.Codigo,
                        impostoValorAgregado.Descricao,
                        ImpostoMaiorQueZero = impostoValorAgregado.ImpostoMaiorQueZero.ObterDescricao(),
                        DestinatarioExterior = impostoValorAgregado.DestinatarioExterior.ObterDescricao(),
                        UsoMaterial = impostoValorAgregado.UsoMaterial.ObterDescricao(),
                        impostoValorAgregado.CodigoIVA,
                        ModeloDocumentoFiscal = impostoValorAgregado.ModeloDocumentoFiscal.Descricao
                    }
                ).ToList();

                grid.AdicionaRows(impostosValorAgregadoRetornar);
                grid.setarQuantidadeTotal(totalRegistro);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Contabeis.ImpostoValorAgregado repositorioImpostoValorAgregado = new Repositorio.Embarcador.Contabeis.ImpostoValorAgregado(unitOfWork);
                Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado impostoValorAgregado = repositorioImpostoValorAgregado.BuscarPorCodigo(codigo);

                if (impostoValorAgregado == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var retorno = new
                {
                    impostoValorAgregado.Codigo,
                    impostoValorAgregado.ImpostoMaiorQueZero,
                    impostoValorAgregado.DestinatarioExterior,
                    impostoValorAgregado.PermitirInformarManualmente,
                    impostoValorAgregado.CodigoIVA,
                    impostoValorAgregado.UsoMaterial,
                    ModeloDocumentoFiscal = new { impostoValorAgregado.ModeloDocumentoFiscal.Codigo, impostoValorAgregado.ModeloDocumentoFiscal.Descricao }
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Contabeis.ImpostoValorAgregado repositorioImpostoValorAgregado = new Repositorio.Embarcador.Contabeis.ImpostoValorAgregado(unitOfWork);
                Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado impostoValorAgregado = new Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado();

                PreencherImpostoValorAgregado(impostoValorAgregado, unitOfWork);
                ValidarImpostoValorAgregado(impostoValorAgregado, unitOfWork);

                repositorioImpostoValorAgregado.Inserir(impostoValorAgregado, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Contabeis.ImpostoValorAgregado repositorioImpostoValorAgregado = new Repositorio.Embarcador.Contabeis.ImpostoValorAgregado(unitOfWork);
                Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado impostoValorAgregado = repositorioImpostoValorAgregado.BuscarPorCodigo(codigo);

                if (impostoValorAgregado == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherImpostoValorAgregado(impostoValorAgregado, unitOfWork);
                ValidarImpostoValorAgregado(impostoValorAgregado, unitOfWork);

                repositorioImpostoValorAgregado.Atualizar(impostoValorAgregado, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Contabeis.ImpostoValorAgregado repositorioImpostoValorAgregado = new Repositorio.Embarcador.Contabeis.ImpostoValorAgregado(unitOfWork);
                Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado impostoValorAgregado = repositorioImpostoValorAgregado.BuscarPorCodigo(codigo);

                if (impostoValorAgregado == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repositorioImpostoValorAgregado.Deletar(impostoValorAgregado, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);

                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaImpostoValorAgregado ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaImpostoValorAgregado()
            {
                CodigoModeloDocumentoFiscal = Request.GetIntParam("ModeloDocumentoFiscal"),
                CodigoIVA = Request.GetStringParam("CodigoIVA"),
                DestinatarioExterior = Request.GetNullableBoolParam("DestinatarioExterior"),
                ImpostoMaiorQueZero = Request.GetNullableBoolParam("ImpostoMaiorQueZero"),
                PermitirInformarManualmente = Request.GetNullableBoolParam("PermitirInformarManualmente"),
                UsoMaterial = Request.GetNullableEnumParam<UsoMaterial>("UsoMaterial")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricatoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        private void PreencherImpostoValorAgregado(Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado impostoValorAgregado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ModeloDocumentoFiscal repositorioModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            int codigoModeloDocumentoFiscal = Request.GetIntParam("ModeloDocumentoFiscal");

            impostoValorAgregado.ImpostoMaiorQueZero = Request.GetBoolParam("ImpostoMaiorQueZero");
            impostoValorAgregado.DestinatarioExterior = Request.GetBoolParam("DestinatarioExterior");
            impostoValorAgregado.UsoMaterial = Request.GetEnumParam<UsoMaterial>("UsoMaterial");
            impostoValorAgregado.CodigoIVA = Request.GetStringParam("CodigoIVA");
            impostoValorAgregado.PermitirInformarManualmente = Request.GetBoolParam("PermitirInformarManualmente");
            impostoValorAgregado.ModeloDocumentoFiscal = (codigoModeloDocumentoFiscal > 0) ? repositorioModeloDocumentoFiscal.BuscarPorId(codigoModeloDocumentoFiscal) : null;
        }

        private void ValidarImpostoValorAgregado(Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado impostoValorAgregado, Repositorio.UnitOfWork unitOfWork)
        {
            if (impostoValorAgregado.ModeloDocumentoFiscal == null)
                throw new ControllerException("O modelo de documento fiscal deve ser informado");

            ValidarImpostoValorAgregadoDuplicado(impostoValorAgregado, unitOfWork);
        }

        private void ValidarImpostoValorAgregadoDuplicado(Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado impostoValorAgregado, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaImpostoValorAgregado filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaImpostoValorAgregado()
            {
                CodigoDesconsiderar = impostoValorAgregado.Codigo,
                CodigoModeloDocumentoFiscal = impostoValorAgregado.ModeloDocumentoFiscal?.Codigo ?? 0,
                DestinatarioExterior = impostoValorAgregado.DestinatarioExterior,
                ImpostoMaiorQueZero = impostoValorAgregado.ImpostoMaiorQueZero,
                UsoMaterial = impostoValorAgregado.UsoMaterial
            };
            Repositorio.Embarcador.Contabeis.ImpostoValorAgregado repositorioImpostoValorAgregado = new Repositorio.Embarcador.Contabeis.ImpostoValorAgregado(unitOfWork);

            if (repositorioImpostoValorAgregado.ContarConsulta(filtrosPesquisa) > 0)
                throw new ControllerException("Já existe um cadastro de IVA com os mesmos dados");
        }

        #endregion
    }
}
