using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/Adiantamento")]
    public class AdiantamentoController : BaseController
    {
		#region Construtores

		public AdiantamentoController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Frota.Adiantamento repAdiantamento = new Repositorio.Embarcador.Frota.Adiantamento(unitOfWork);

                // Converte parametros
                int codigoMotorista = 0;
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);

                decimal valorInicial, valorFinal = 0;
                decimal.TryParse(Request.Params("ValorInicial"), out valorInicial);
                decimal.TryParse(Request.Params("ValorFinal"), out valorFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAdiantamento situacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAdiantamento)int.Parse(Request.Params("Situacao"));

                // Ordenacao
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Motorista")
                    propOrdenar = "Motorista.Nome";

                List<Dominio.Entidades.Embarcador.Frota.Adiantamento> listaAdiantamentos = repAdiantamento.Consultar(codigoMotorista, valorInicial, valorFinal, situacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAdiantamento.ContarConsultar(codigoMotorista, valorInicial, valorFinal, situacao));

                var lista = (from p in listaAdiantamentos
                             select new
                             {
                                 p.Codigo,
                                 Motorista = p.Motorista?.Nome ?? string.Empty,
                                 Valor = p.Valor.ToString("n2"),
                                 Data = p.Data.ToString("dd/MM/yyyy"),
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

                Repositorio.Embarcador.Frota.Adiantamento repAdiantamento = new Repositorio.Embarcador.Frota.Adiantamento(unitOfWork);

                Dominio.Entidades.Embarcador.Frota.Adiantamento adiantamento = repAdiantamento.BuscarPorCodigo(codigo);

                var dyn = new
                {
                    adiantamento.Codigo,
                    Motorista = adiantamento.Motorista != null ? new { Codigo = adiantamento.Motorista.Codigo, Descricao = adiantamento.Motorista.Nome } : null,
                    Valor = adiantamento.Valor.ToString("n2"),
                    Data = adiantamento.Data.ToString("dd/MM/yyyy"),
                    Observacao = !string.IsNullOrEmpty(adiantamento.Observacao) ? adiantamento.Observacao : string.Empty,
                    Situacao = adiantamento.Situacao,
                    DescricaoSituacao = adiantamento.Situacao.ToString("G"),
                    TipoPagamento = new
                    {
                        Codigo = adiantamento.TipoPagamento?.Codigo ?? 0,
                        Descricao = adiantamento.TipoPagamento?.Descricao ?? string.Empty
                    },
                    TipoMovimentoMotorista = new
                    {
                        Codigo = adiantamento.TipoMovimentoMotorista?.Codigo ?? 0,
                        Descricao = adiantamento.TipoMovimentoMotorista?.Descricao ?? string.Empty
                    }
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
                Repositorio.Embarcador.Frota.Adiantamento repAdiantamento = new Repositorio.Embarcador.Frota.Adiantamento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem repConfiguracaoFinanceiraContratoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem(unitOfWork);

                Servicos.Embarcador.Financeiro.ProcessoMovimento serProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem configAcerto = repConfiguracaoFinanceiraContratoAcertoViagem.BuscarPrimeiroRegistro();

                // Valida configuracao de adiantamento
                if (configAcerto == null || !configAcerto.GerarMovimentoAutomaticoNaGeracaoAcertoViagem || configAcerto.ContaEntradaAdiantamentoMotorista == null || configAcerto.ContaEntradaComissaoMotorista == null)
                    return new JsonpResult(false, true, "Nenhum movimento financeiro padrão configurado para essa operação.");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Frota.Adiantamento adiantamento = new Dominio.Entidades.Embarcador.Frota.Adiantamento();

                // Preenche a entidade
                PreencheEntidade(ref adiantamento, unitOfWork);

                if (adiantamento.TipoMovimentoMotorista == null)
                    return new JsonpResult(false, true, "Nenhum tipo de movimento de motorista informado.");

                // Seta a situacao como ativa
                adiantamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAdiantamento.Ativo;

                // Valida dados
                string erro;
                if (!ValidaEntidade(adiantamento, out erro))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                if (adiantamento.TipoMovimentoMotorista.TipoMovimentoEntidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Entrada)
                {
                    adiantamento.PlanoContaEntrada = adiantamento.TipoMovimentoMotorista.PlanoConta;//adiantamento.TipoMovimentoMotorista.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoAdiantamentoMotorista.Adiantamento ? configAcerto.ContaEntradaAdiantamentoMotorista : configAcerto.ContaEntradaComissaoMotorista;
                    adiantamento.PlanoContaSaida = adiantamento.TipoPagamento.PlanoConta;
                }
                else
                {
                    adiantamento.PlanoContaEntrada = adiantamento.TipoPagamento.PlanoConta;
                    adiantamento.PlanoContaSaida = adiantamento.TipoMovimentoMotorista.PlanoConta;//adiantamento.TipoMovimentoMotorista.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoAdiantamentoMotorista.Adiantamento ? configAcerto.ContaEntradaAdiantamentoMotorista : configAcerto.ContaEntradaComissaoMotorista;
                }

                repAdiantamento.Inserir(adiantamento, Auditado);

                string obsMovimentacao = adiantamento.TipoMovimentoMotorista.Descricao + " ao motorista " + adiantamento.Motorista.Nome;

                if (!serProcessoMovimento.GerarMovimentacao(out erro, null, adiantamento.Data, adiantamento.Valor, adiantamento.Codigo.ToString(), obsMovimentacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.AdiantamentoMotorista, TipoServicoMultisoftware, adiantamento.Motorista.Codigo, adiantamento.PlanoContaSaida, adiantamento.PlanoContaEntrada, 0, adiantamento.TipoMovimentoMotorista.TipoMovimentoEntidade))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                Repositorio.Embarcador.Frota.Adiantamento repAdiantamento = new Repositorio.Embarcador.Frota.Adiantamento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem repConfiguracaoFinanceiraContratoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem(unitOfWork);

                Servicos.Embarcador.Financeiro.ProcessoMovimento serProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);

                Dominio.Entidades.Embarcador.Frota.Adiantamento adiantamento = repAdiantamento.BuscarPorCodigo(codigo, true);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem configAcerto = repConfiguracaoFinanceiraContratoAcertoViagem.BuscarPrimeiroRegistro();

                // Adiantamento nao encontrado
                if (adiantamento == null)
                    return new JsonpResult(false, true, "Adiantamento não encontrado.");

                if (adiantamento.TipoMovimentoMotorista == null)
                    return new JsonpResult(false, true, "Nenhum tipo de movimento do motorista selecionado.");

                unitOfWork.Start();

                // Seta a situacao como estornado
                adiantamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAdiantamento.Estornado;

                repAdiantamento.Atualizar(adiantamento, Auditado);

                // Movimento financeiro
                string erro = string.Empty;
                string obsMovimentacao = "Estorno do " + adiantamento.TipoMovimentoMotorista.Descricao + " ao motorista " + adiantamento.Motorista.Nome;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade tipoEntidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Nenhum;
                if (adiantamento.TipoMovimentoMotorista.TipoMovimentoEntidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Saida)
                    tipoEntidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Entrada;
                else
                    tipoEntidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Saida;

                if (!serProcessoMovimento.GerarMovimentacao(out erro, null, adiantamento.Data, adiantamento.Valor, adiantamento.Codigo.ToString(), obsMovimentacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.AdiantamentoMotorista, TipoServicoMultisoftware, adiantamento.Motorista.Codigo, adiantamento.PlanoContaEntrada, adiantamento.PlanoContaSaida, 0, tipoEntidade))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, adiantamento, null, "Adiantamento Estornado", unitOfWork);
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

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Frota.Adiantamento adiantamento, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios/Entidade
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimentoMotorista repTipoMovimentoMotorista = new Repositorio.Embarcador.Financeiro.TipoMovimentoMotorista(unitOfWork);

            // Converte parametros
            int codigoMotorista, codigoTipoPagamento, codigoTipoMovimentoMotorista;
            int.TryParse(Request.Params("Motorista"), out codigoMotorista);
            int.TryParse(Request.Params("TipoPagamento"), out codigoTipoPagamento);
            int.TryParse(Request.Params("TipoMovimentoMotorista"), out codigoTipoMovimentoMotorista);

            decimal valor;
            decimal.TryParse(Request.Params("Valor"), out valor);

            DateTime data;
            DateTime.TryParseExact(Request.Params("Data"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);

            string observacao = Request.Params("Observacao") ?? "";

            // Cria entidade
            adiantamento.Motorista = repUsuario.BuscarPorCodigo(codigoMotorista);
            adiantamento.Valor = valor;
            adiantamento.Data = data;
            adiantamento.Observacao = observacao;
            adiantamento.TipoPagamento = repTipoPagamento.BuscarPorCodigo(codigoTipoPagamento);
            adiantamento.TipoMovimentoMotorista = repTipoMovimentoMotorista.BuscarPorCodigo(codigoTipoMovimentoMotorista);
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Frota.Adiantamento adiantamento, out string erro)
        {
            erro = string.Empty;

            if (adiantamento.Motorista == null)
            {
                erro = "Motorista é obrigatório.";
                return false;
            }

            if (adiantamento.Valor <= 0)
            {
                erro = "Valor é obrigatório.";
                return false;
            }

            if (adiantamento.Data == DateTime.MinValue)
            {
                erro = "Data do Adiantamento é obrigatório.";
                return false;
            }

            if (adiantamento.TipoMovimentoMotorista == null)
            {
                erro = "Tipo de movimento do motorista é obrigatório.";
                return false;
            }

            if (adiantamento.TipoPagamento == null)
            {
                erro = "Deve ser informado um tipo de pagamento.";
                return false;
            }

            if (adiantamento.TipoMovimentoMotorista.TipoMovimentoEntidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Nenhum)
            {
                erro = "Situação do movimento não pode ser Nenhum.";
                return false;
            }

            return true;
        }
    }
}
