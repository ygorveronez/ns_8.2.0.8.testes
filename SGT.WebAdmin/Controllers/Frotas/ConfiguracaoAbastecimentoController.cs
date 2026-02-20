using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frotas
{
    public class ConfiguracaoAbastecimentoController : BaseController
    {
		#region Construtores

		public ConfiguracaoAbastecimentoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento tipoImportacao;
                if (!string.IsNullOrWhiteSpace(Request.Params("TipoImportacao")))
                    tipoImportacao = !string.IsNullOrEmpty(Request.Params("TipoImportacao")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento)int.Parse(Request.Params("TipoImportacao")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento.EDI;
                else
                    tipoImportacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento)0;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Importação", "DescricaoTipoImportacaoAbastecimento", 20, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimento repConfiguracaoAbastecimento = new Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento> listaConfiguracaoAbastecimento = repConfiguracaoAbastecimento.Consultar(descricao, tipoImportacao, ativo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repConfiguracaoAbastecimento.ContarConsulta(descricao, tipoImportacao, ativo));

                var lista = (from p in listaConfiguracaoAbastecimento
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DescricaoTipoImportacaoAbastecimento
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
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimento repConfiguracaoAbastecimento = new Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimento(unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento configuracaoAbastecimento = new Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento();

                configuracaoAbastecimento.Descricao = Request.Params("Descricao");
                configuracaoAbastecimento.Ativo = bool.Parse(Request.Params("Ativo"));
                configuracaoAbastecimento.TipoImportacaoAbastecimento = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento)int.Parse(Request.Params("TipoImportacao"));

                if (!string.IsNullOrWhiteSpace(Request.Params("LayoutEDI")) && int.Parse(Request.Params("LayoutEDI")) > 0)
                    configuracaoAbastecimento.LayoutEDI = new Dominio.Entidades.LayoutEDI() { Codigo = int.Parse(Request.Params("LayoutEDI")) };
                if (!string.IsNullOrWhiteSpace(Request.Params("PostoInterno")) && double.Parse(Request.Params("PostoInterno")) > 0)
                    configuracaoAbastecimento.Posto = new Dominio.Entidades.Cliente() { CPF_CNPJ = double.Parse(Request.Params("PostoInterno")) };
                if (!string.IsNullOrWhiteSpace(Request.Params("TipoMovimento")) && int.Parse(Request.Params("TipoMovimento")) > 0)
                    configuracaoAbastecimento.TipoMovimento = new Dominio.Entidades.Embarcador.Financeiro.TipoMovimento() { Codigo = int.Parse(Request.Params("TipoMovimento")) };

                configuracaoAbastecimento.UtilizarPrecoDaTabelaDeValoresDoFornecedor = Request.GetBoolParam("UtilizarPrecoDaTabelaDeValoresDoFornecedor");
                configuracaoAbastecimento.GerarContasAPagarParaAbastecimentoExternos = Request.GetBoolParam("GerarContasAPagarParaAbastecimentoExternos");
                configuracaoAbastecimento.NaoImportarAbastecimentoDuplicado = Request.GetBoolParam("NaoImportarAbastecimentoDuplicado");
                configuracaoAbastecimento.NaoGerarMovimentoFinanceiroFechamentoExterno = Request.GetBoolParam("NaoGerarMovimentoFinanceiroFechamentoExterno");

                repConfiguracaoAbastecimento.Inserir(configuracaoAbastecimento, Auditado);
                SalvarListaColuna(configuracaoAbastecimento, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
        }

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimento repConfiguracaoAbastecimento = new Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimento(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento configuracaoAbastecimento = repConfiguracaoAbastecimento.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                configuracaoAbastecimento.Descricao = Request.Params("Descricao");
                configuracaoAbastecimento.Ativo = bool.Parse(Request.Params("Ativo"));
                configuracaoAbastecimento.TipoImportacaoAbastecimento = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento)int.Parse(Request.Params("TipoImportacao"));
                if (!string.IsNullOrWhiteSpace(Request.Params("LayoutEDI")) && int.Parse(Request.Params("LayoutEDI")) > 0)
                    configuracaoAbastecimento.LayoutEDI = new Dominio.Entidades.LayoutEDI() { Codigo = int.Parse(Request.Params("LayoutEDI")) };
                if (!string.IsNullOrWhiteSpace(Request.Params("PostoInterno")) && double.Parse(Request.Params("PostoInterno")) > 0)
                    configuracaoAbastecimento.Posto = new Dominio.Entidades.Cliente() { CPF_CNPJ = double.Parse(Request.Params("PostoInterno")) };
                if (!string.IsNullOrWhiteSpace(Request.Params("TipoMovimento")) && int.Parse(Request.Params("TipoMovimento")) > 0)
                    configuracaoAbastecimento.TipoMovimento = new Dominio.Entidades.Embarcador.Financeiro.TipoMovimento() { Codigo = int.Parse(Request.Params("TipoMovimento")) };

                configuracaoAbastecimento.UtilizarPrecoDaTabelaDeValoresDoFornecedor = Request.GetBoolParam("UtilizarPrecoDaTabelaDeValoresDoFornecedor");
                configuracaoAbastecimento.GerarContasAPagarParaAbastecimentoExternos = Request.GetBoolParam("GerarContasAPagarParaAbastecimentoExternos");
                configuracaoAbastecimento.NaoImportarAbastecimentoDuplicado = Request.GetBoolParam("NaoImportarAbastecimentoDuplicado");
                configuracaoAbastecimento.NaoGerarMovimentoFinanceiroFechamentoExterno = Request.GetBoolParam("NaoGerarMovimentoFinanceiroFechamentoExterno");

                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repConfiguracaoAbastecimento.Atualizar(configuracaoAbastecimento, Auditado);
                SalvarListaColuna(configuracaoAbastecimento, unitOfWork, historico);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimento repConfiguracaoAbastecimento = new Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimento(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento configuracaoAbastecimento = repConfiguracaoAbastecimento.BuscarPorCodigo(codigo);

                var dynAbastecimento = new
                {
                    configuracaoAbastecimento.Codigo,
                    configuracaoAbastecimento.Ativo,
                    configuracaoAbastecimento.Descricao,
                    TipoImportacao = configuracaoAbastecimento.TipoImportacaoAbastecimento,
                    LayoutEDI = configuracaoAbastecimento.LayoutEDI != null ? new { Codigo = configuracaoAbastecimento.LayoutEDI.Codigo, Descricao = configuracaoAbastecimento.LayoutEDI.Descricao } : null,
                    PostoInterno = configuracaoAbastecimento.Posto != null ? new { Codigo = configuracaoAbastecimento.Posto.CPF_CNPJ, Descricao = configuracaoAbastecimento.Posto.Nome } : null,
                    TipoMovimento = configuracaoAbastecimento.TipoMovimento != null ? new { Codigo = configuracaoAbastecimento.TipoMovimento.Codigo, Descricao = configuracaoAbastecimento.TipoMovimento.Descricao } : null,
                    configuracaoAbastecimento.UtilizarPrecoDaTabelaDeValoresDoFornecedor,
                    configuracaoAbastecimento.GerarContasAPagarParaAbastecimentoExternos,
                    configuracaoAbastecimento.NaoImportarAbastecimentoDuplicado,
                    configuracaoAbastecimento.NaoGerarMovimentoFinanceiroFechamentoExterno,
                    ListaColunas = (from obj in configuracaoAbastecimento.ConfiguracoesPlanilhas
                                    orderby obj.Posicao
                                    select new
                                    {
                                        Codigo = obj.Codigo,
                                        obj.DescricaoTipoCampo,
                                        obj.TipoCampo,
                                        obj.DescricaoColunaPlanilha,
                                        obj.ColunaPlanilha,
                                        obj.Posicao
                                    }).ToList(),
                };

                return new JsonpResult(dynAbastecimento);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
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

                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimento repConfiguracaoAbastecimento = new Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimento(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento configuracaoAbastecimento = repConfiguracaoAbastecimento.BuscarPorCodigo(codigo);

                repConfiguracaoAbastecimento.Deletar(configuracaoAbastecimento, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
        }

        private void SalvarListaColuna(Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento configuracaoAbastecimento, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Auditoria.HistoricoObjeto historico = null)
        {
            Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimentoPlanilha repConfiguracaoAbastecimentoPlanilha = new Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimentoPlanilha(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimentoPlanilha> listaConfiguracaoAbastecimentoPlanilha = repConfiguracaoAbastecimentoPlanilha.BuscarPorCodigoConfiguracao(configuracaoAbastecimento.Codigo);
            for (int i = 0; i < listaConfiguracaoAbastecimentoPlanilha.Count(); i++)
                repConfiguracaoAbastecimentoPlanilha.Deletar(listaConfiguracaoAbastecimentoPlanilha[i], Auditado, historico);

            dynamic listaColuna = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaColunas"));

            foreach (var coluna in listaColuna)
            {
                Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimentoPlanilha configuracaoAbastecimentoPlanilha = new Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimentoPlanilha();
                configuracaoAbastecimentoPlanilha.ColunaPlanilha = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha)int.Parse((string)coluna.ColunaPlanilha);
                configuracaoAbastecimentoPlanilha.TipoCampo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampo)int.Parse((string)coluna.TipoCampo);
                configuracaoAbastecimentoPlanilha.Posicao = int.Parse((string)coluna.Posicao);
                configuracaoAbastecimentoPlanilha.ConfiguracaoAbastecimento = configuracaoAbastecimento;
                repConfiguracaoAbastecimentoPlanilha.Inserir(configuracaoAbastecimentoPlanilha, Auditado, historico);
            }
        }
    }
}
