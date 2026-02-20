using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosEmissao
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class DadosLacreController : BaseController
    {
		#region Construtores

		public DadosLacreController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Lacre", "Numero", !ConfiguracaoEmbarcador.ExibirTipoLacre ? 80 : 20, Models.Grid.Align.left, true);
                if (ConfiguracaoEmbarcador.ExibirTipoLacre)
                {
                    grid.AdicionarCabecalho("Tipo Lacre", "TipoLacre", 20, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("Cliente", "Cliente", 40, Models.Grid.Align.left, false);
                }

                Repositorio.Embarcador.Cargas.CargaLacre repCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> listaLacres = repCargaLacre.Consultar(codigoCarga, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCargaLacre.ContarConsulta(codigoCarga));

                var lista = (from p in listaLacres
                             select new
                             {
                                 p.Codigo,
                                 p.Numero,
                                 TipoLacre = p.TipoLacre?.Descricao ?? string.Empty,
                                 Cliente = p.Cliente?.Descricao ?? string.Empty
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if ((!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AlterarLacres) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unidadeTrabalho);
                Servicos.Embarcador.Carga.Impressao serImpressao = new Servicos.Embarcador.Carga.Impressao(unidadeTrabalho);

                Repositorio.Embarcador.Cargas.CargaLacre repCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.TipoLacre repTipoLacre = new Repositorio.Embarcador.Cargas.TipoLacre(unidadeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

                int codigoCarga = Request.GetIntParam("Carga");
                int codigoTipoLacre = Request.GetIntParam("TipoLacre");
                double cliente = Request.GetDoubleParam("Cliente");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                serCarga.ValidarPermissaoAlterarDadosEtapaFrete(carga, unidadeTrabalho);

                string numero = Request.Params("Numero");

                Dominio.Entidades.Embarcador.Cargas.CargaLacre cargaLacre = new Dominio.Entidades.Embarcador.Cargas.CargaLacre
                {
                    Carga = carga,
                    Numero = numero,
                    TipoLacre = codigoTipoLacre > 0 ? repTipoLacre.BuscarPorCodigo(codigoTipoLacre, false) : null,
                    Cliente = cliente > 0 ? repCliente.BuscarPorCPFCNPJ(cliente) : null
                };

                repCargaLacre.Inserir(cargaLacre, Auditado);

                if (carga.SituacaoCarga == SituacaoCarga.EmTransporte || carga.SituacaoCarga == SituacaoCarga.Encerrada)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, Localization.Resources.Cargas.Carga.AlterouLacresCarga, unidadeTrabalho);

                    if (carga.TipoOperacao?.EnviarEmailPlanoViagemFinalizarCarga ?? false)
                    {
                        Servicos.Log.TratarErro($"Plano viagem envio e-mail alteração dados do lacre - {codigoCarga}");
                        serImpressao.EnviarPlanoViagemParaDestinatariosPorEmail(carga, "Plano de Viagem");
                    }
                }

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Excluir()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if ((!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AlterarLacres) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unidadeTrabalho);
                Servicos.Embarcador.Carga.Impressao serImpressao = new Servicos.Embarcador.Carga.Impressao(unidadeTrabalho);

                Repositorio.Embarcador.Cargas.CargaLacre repCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaLacre cargaLacre = repCargaLacre.BuscarPorCodigo(codigo);

                serCarga.ValidarPermissaoAlterarDadosEtapaFrete(cargaLacre.Carga, unidadeTrabalho);

                repCargaLacre.Deletar(cargaLacre, Auditado);

                if (cargaLacre.Carga.SituacaoCarga == SituacaoCarga.EmTransporte || cargaLacre.Carga.SituacaoCarga == SituacaoCarga.Encerrada)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaLacre.Carga, Localization.Resources.Cargas.Carga.AlterouLacresCarga, unidadeTrabalho);

                    if (cargaLacre.Carga.TipoOperacao?.EnviarEmailPlanoViagemFinalizarCarga ?? false)
                    {
                        Servicos.Log.TratarErro($"Plano viagem envio e-mail alteração dados do lacre - {cargaLacre.Carga.Codigo}");
                        serImpressao.EnviarPlanoViagemParaDestinatariosPorEmail(cargaLacre.Carga, "Plano de Viagem");
                    }
                }

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion
    }
}
