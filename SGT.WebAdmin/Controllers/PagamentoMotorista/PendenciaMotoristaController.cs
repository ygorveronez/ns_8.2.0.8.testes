using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("PagamentosMotoristas/PendenciaMotorista")]
    public class PendenciaMotoristaController : BaseController
    {
		#region Construtores

		public PendenciaMotoristaController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Motorista", "Motorista", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.left, true);


                // Instancia repositorios
                Repositorio.Embarcador.PagamentoMotorista.PendenciaMotorista repAdiantamento = new Repositorio.Embarcador.PagamentoMotorista.PendenciaMotorista(unitOfWork);

                // Converte parametros
                int codigoMotorista = 0;
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);

                decimal valorInicial, valorFinal = 0;
                decimal.TryParse(Request.Params("ValorInicial"), out valorInicial);
                decimal.TryParse(Request.Params("ValorFinal"), out valorFinal);

                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicio);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFim);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPendenciaMotorista situacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPendenciaMotorista)int.Parse(Request.Params("Situacao"));


                // Ordenacao
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Motorista")
                    propOrdenar = "Motorista.Nome";

                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotorista> listaPendenciaMotorista = repAdiantamento.Consultar(codigoMotorista, valorInicial, valorFinal, dataInicio, dataFim, situacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAdiantamento.ContarConsultar(codigoMotorista, valorInicial, valorFinal, dataInicio, dataFim, situacao));

                var lista = (from p in listaPendenciaMotorista
                             select new
                             {
                                 p.Codigo,
                                 Motorista = p.Motorista?.Nome ?? string.Empty,
                                 Valor = p.Valor.ToString("n2"),
                                 Data = p.Data.ToString("dd/MM/yyyy"),
                                 Pendencia = p.Pendencia.ToString(),
                                 Justificativa = p.Justificativa.ToString(),
                                 Situacao = p.Situacao.ToString("G")
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.PagamentoMotorista.PendenciaMotorista repPendenciaMotorista = new Repositorio.Embarcador.PagamentoMotorista.PendenciaMotorista(unitOfWork);

                Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotorista pendenciaMotorista = repPendenciaMotorista.BuscarPorCodigo(codigo);

                var dyn = new
                {
                    pendenciaMotorista.Codigo,
                    Motorista = pendenciaMotorista.Motorista != null ? new { Codigo = pendenciaMotorista.Motorista.Codigo, Descricao = pendenciaMotorista.Motorista.Nome } : null,
                    Valor = pendenciaMotorista.Valor.ToString("n2"),
                    Data = pendenciaMotorista.Data.ToString("dd/MM/yyyy"),
                    Observacao = !string.IsNullOrEmpty(pendenciaMotorista.Observacao) ? pendenciaMotorista.Observacao : string.Empty,
                    ValorPendencia = pendenciaMotorista.Pendencia.Valor,
                    Situacao = pendenciaMotorista.Situacao,
                    DescricaoSituacao = pendenciaMotorista.Situacao.ToString("G"),
                    Justificativa = new
                    {
                        Codigo = pendenciaMotorista.Justificativa?.Codigo ?? 0,
                        Descricao = pendenciaMotorista.Justificativa?.Descricao ?? string.Empty
                    },
                    Pendencia = new
                    {
                        Codigo = pendenciaMotorista.Pendencia?.Codigo ?? 0,
                        Descricao = pendenciaMotorista.Pendencia?.Descricao ?? string.Empty,
                    },
                    Anexos = (
                        from obj in pendenciaMotorista.Anexos
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            obj.NomeArquivo,
                        }
                    ).ToList(),
                };

                return new JsonpResult(dyn);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia Repositorios/Entidade/ Servicos
                Repositorio.Embarcador.PagamentoMotorista.PendenciaMotorista repPendenciaMotorista = new Repositorio.Embarcador.PagamentoMotorista.PendenciaMotorista(unitOfWork);

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);

                Servicos.Embarcador.Financeiro.ProcessoMovimento serProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotorista pendenciaMotorista = new Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotorista();

                // Preenche a entidade
                PreencheEntidade(ref pendenciaMotorista, unitOfWork);

                // Seta a situacao como ativa
                pendenciaMotorista.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPendenciaMotorista.Ativo;

                // Valida dados
                string erro;
                if (!ValidaEntidade(pendenciaMotorista, out erro))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                if (pendenciaMotorista.Pendencia != null)
                {
                    Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotoristaIntegracaoEnvio = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS();
                    pagamentoMotoristaIntegracaoEnvio = repPagamentoMotoristaTMS.BuscarPorCodigo(pendenciaMotorista.Pendencia.Codigo);
                    pagamentoMotoristaIntegracaoEnvio.Pendente = true;

                    repPagamentoMotoristaTMS.Atualizar(pagamentoMotoristaIntegracaoEnvio, Auditado);
                }

                repPendenciaMotorista.Inserir(pendenciaMotorista, Auditado);

                string obsMovimentacao = pendenciaMotorista.Justificativa.TipoJustificativa + " ao motorista " + pendenciaMotorista.Motorista.Nome;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade tipoEntidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Nenhum;

                if (pendenciaMotorista.Justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto)
                    tipoEntidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Entrada;
                else
                    tipoEntidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Saida;

                if (!serProcessoMovimento.GerarMovimentacao(out erro, null, pendenciaMotorista.Data, pendenciaMotorista.Valor, pendenciaMotorista.Codigo.ToString(), obsMovimentacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.PendenciaMotorista, TipoServicoMultisoftware, pendenciaMotorista.Motorista.Codigo, pendenciaMotorista.Pendencia.PlanoDeContaDebito, pendenciaMotorista.Pendencia.PlanoDeContaCredito, 0, tipoEntidade))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(pendenciaMotorista.Codigo);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EstornarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Pega codigo
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Instancia Repositorios/Entidade
                Repositorio.Embarcador.PagamentoMotorista.PendenciaMotorista repPendenciaMotorista = new Repositorio.Embarcador.PagamentoMotorista.PendenciaMotorista(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem repConfiguracaoFinanceiraContratoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);

                Servicos.Embarcador.Financeiro.ProcessoMovimento serProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);

                Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotorista pendenciaMotorista = repPendenciaMotorista.BuscarPorCodigo(codigo, true);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem configAcerto = repConfiguracaoFinanceiraContratoAcertoViagem.BuscarPrimeiroRegistro();

                // Adiantamento nao encontrado
                if (pendenciaMotorista == null)
                    return new JsonpResult(false, true, "Pendência de Motorista não encontrado.");


                unitOfWork.Start();

                // Seta a situacao como estornado
                pendenciaMotorista.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPendenciaMotorista.Estornado;

                repPendenciaMotorista.Atualizar(pendenciaMotorista, Auditado);

                // Movimento financeiro
                string erro = string.Empty;
                string obsMovimentacao = "Estorno do " + pendenciaMotorista.Justificativa.TipoJustificativa + " ao motorista " + pendenciaMotorista.Motorista.Nome;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade tipoEntidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Nenhum;

                if (pendenciaMotorista.Justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto)
                    tipoEntidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Entrada;
                else
                    tipoEntidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Saida;

                if (!serProcessoMovimento.GerarMovimentacao(out erro, null, pendenciaMotorista.Data, pendenciaMotorista.Valor, pendenciaMotorista.Codigo.ToString(), obsMovimentacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.PendenciaMotorista, TipoServicoMultisoftware, pendenciaMotorista.Motorista.Codigo, pendenciaMotorista.Pendencia.PlanoDeContaDebito, pendenciaMotorista.Pendencia.PlanoDeContaCredito, 0, tipoEntidade))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }


                if (pendenciaMotorista.Pendencia != null)
                {
                    Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotoristaIntegracaoEnvio = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS();
                    pagamentoMotoristaIntegracaoEnvio = repPagamentoMotoristaTMS.BuscarPorCodigo(pendenciaMotorista.Pendencia.Codigo);
                    pagamentoMotoristaIntegracaoEnvio.Pendente = false;

                    repPagamentoMotoristaTMS.Atualizar(pagamentoMotoristaIntegracaoEnvio, Auditado);
                }


                Servicos.Auditoria.Auditoria.Auditar(Auditado, pendenciaMotorista, null, "Pendência de Motorista Estornado", unitOfWork);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao estornar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotorista pendenciaMotorista, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios/Entidade
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);

            // Converte parametros
            int codigoMotorista, codigoJustificativa, codigoPendencia;
            int.TryParse(Request.Params("Motorista"), out codigoMotorista);
            int.TryParse(Request.Params("Justificativa"), out codigoJustificativa);
            int.TryParse(Request.Params("Pendencia"), out codigoPendencia);

            decimal valor;
            decimal.TryParse(Request.Params("Valor"), out valor);

            DateTime data;
            DateTime.TryParseExact(Request.Params("Data"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);

            string observacao = Request.Params("Observacao") ?? "";

            // Cria entidade
            pendenciaMotorista.Motorista = repUsuario.BuscarPorCodigo(codigoMotorista);
            pendenciaMotorista.Valor = valor;
            pendenciaMotorista.Data = data;
            pendenciaMotorista.Observacao = observacao;
            pendenciaMotorista.Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);
            pendenciaMotorista.Pendencia = repPagamentoMotoristaTMS.BuscarPorCodigo(codigoPendencia);
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotorista pendenciaMotorista, out string erro)
        {
            erro = string.Empty;

            if (pendenciaMotorista.Motorista == null)
            {
                erro = "Motorista é obrigatório.";
                return false;
            }

            if (pendenciaMotorista.Valor <= 0)
            {
                erro = "Valor é obrigatório.";
                return false;
            }

            if (pendenciaMotorista.Data == DateTime.MinValue)
            {
                erro = "Data do Adiantamento é obrigatório.";
                return false;
            }

            if (pendenciaMotorista.Pendencia == null)
            {
                erro = "Pendência é obrigatório.";
                return false;
            }

            if (pendenciaMotorista.Justificativa == null)
            {
                erro = "Deve ser informado uma justificativa.";
                return false;
            }


            return true;
        }
    }
}
