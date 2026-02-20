using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/AcordoFaturamentoCliente")]
    public class AcordoFaturamentoClienteController : BaseController
    {
		#region Construtores

		public AcordoFaturamentoClienteController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                double pessoa = Request.GetDoubleParam("Pessoa");
                int grupoPessoa = Request.GetIntParam("GrupoPessoa");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);
                TipoPessoa tipoPessoa = Request.GetEnumParam<TipoPessoa>("TipoPessoa");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo Pessoa", "TipoPessoa", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Pessoa / Grupo Pessoa", "PessoaGrupo", 60, Models.Grid.Align.left, false);
                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordoFaturamentoCliente = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);
                List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente> acordoFaturamentoClientes = repAcordoFaturamentoCliente.Consultar(pessoa, grupoPessoa, tipoPessoa, status, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAcordoFaturamentoCliente.ContarConsulta(pessoa, grupoPessoa, tipoPessoa, status));

                var lista = (from p in acordoFaturamentoClientes
                             select new
                             {
                                 p.Codigo,
                                 TipoPessoa = p.TipoPessoa.ObterDescricao(),
                                 PessoaGrupo = p.Pessoa?.Nome ?? p.GrupoPessoas?.Descricao,
                                 p.DescricaoStatus
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

                Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordoFaturamentoCliente = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoFaturamentoCliente = new Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente();

                double pessoa = Request.GetDoubleParam("Pessoa");
                int grupoPessoa = Request.GetIntParam("GrupoPessoa");

                PreencherAcordoFaturamentoCliente(acordoFaturamentoCliente, unitOfWork);

                if (repAcordoFaturamentoCliente.ContemAcordoFaturamentoClienteDuplicado(pessoa, grupoPessoa, acordoFaturamentoCliente.Codigo))
                    return new JsonpResult(false, true, "Não é possível inserir um novo acordo de faturamento para a mesma Pessoa/Grupo de Pessoa.");

                string retorno = SalvarAcordoFaturamentoClienteListasAuditarManual(acordoFaturamentoCliente, unitOfWork);
                if (!string.IsNullOrWhiteSpace(retorno))
                    return new JsonpResult(false, true, retorno);

                SalvarListaEmailsCabotagem(acordoFaturamentoCliente, unitOfWork);
                SalvarListaEmailsCustoExtra(acordoFaturamentoCliente, unitOfWork);
                SalvarListaEmailsLongoCurso(acordoFaturamentoCliente, unitOfWork);

                repAcordoFaturamentoCliente.Inserir(acordoFaturamentoCliente, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
        }

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordoFaturamentoCliente = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoFaturamentoCliente = repAcordoFaturamentoCliente.BuscarPorCodigo(codigo, true);

                PreencherAcordoFaturamentoCliente(acordoFaturamentoCliente, unitOfWork);

                double pessoa = Request.GetDoubleParam("Pessoa");
                int grupoPessoa = Request.GetIntParam("GrupoPessoa");

                if (repAcordoFaturamentoCliente.ContemAcordoFaturamentoClienteDuplicado(pessoa, grupoPessoa, acordoFaturamentoCliente.Codigo))
                    return new JsonpResult(false, true, "Não é possível inserir um novo acordo de faturamento para a mesma Pessoa/Grupo de Pessoa.");

                SalvarListaEmailsCabotagem(acordoFaturamentoCliente, unitOfWork);
                SalvarListaEmailsCustoExtra(acordoFaturamentoCliente, unitOfWork);
                SalvarListaEmailsLongoCurso(acordoFaturamentoCliente, unitOfWork);

                repAcordoFaturamentoCliente.Atualizar(acordoFaturamentoCliente, Auditado);

                string retorno = SalvarAcordoFaturamentoClienteListasAuditarManual(acordoFaturamentoCliente, unitOfWork);
                if (!string.IsNullOrWhiteSpace(retorno))
                    return new JsonpResult(false, true, retorno);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordoFaturamentoCliente = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoFaturamentoCliente = repAcordoFaturamentoCliente.BuscarPorCodigo(codigo, false);

                var dynAcordoFaturamentoCliente = new
                {
                    acordoFaturamentoCliente.Codigo,
                    acordoFaturamentoCliente.TipoPessoa,
                    acordoFaturamentoCliente.Status,
                    acordoFaturamentoCliente.FaturamentoPermissaoExclusiva,
                    acordoFaturamentoCliente.NaoEnviarEmailFaturaAutomaticamente,
                    Pessoa = acordoFaturamentoCliente.Pessoa != null ? new { acordoFaturamentoCliente.Pessoa.Codigo, acordoFaturamentoCliente.Pessoa.Descricao } : null,
                    GrupoPessoa = acordoFaturamentoCliente.GrupoPessoas != null ? new { acordoFaturamentoCliente.GrupoPessoas.Codigo, acordoFaturamentoCliente.GrupoPessoas.Descricao } : null,

                    //Cabotagem
                    acordoFaturamentoCliente.FaturamentoPermissaoExclusivaCabotagem,
                    acordoFaturamentoCliente.CabotagemGerarFaturamentoAVista,
                    acordoFaturamentoCliente.CabotagemDiasDePrazoFatura,
                    acordoFaturamentoCliente.CabotagemTipoPrazoFaturamento,
                    CabotagemDiasSemanaFatura = acordoFaturamentoCliente.CabotagemDiasSemanaFatura.Select(o => o).ToList(),
                    CabotagemDiasMesFatura = acordoFaturamentoCliente.CabotagemDiasMesFatura.Select(o => o).ToList(),
                    acordoFaturamentoCliente.CabotagemEmail,
                    acordoFaturamentoCliente.CabotagemNaoEnviarEmailFaturaAutomaticamente,

                    //LongoCurso
                    acordoFaturamentoCliente.FaturamentoPermissaoExclusivaLongoCurso,
                    acordoFaturamentoCliente.LongoCursoGerarFaturamentoAVista,
                    acordoFaturamentoCliente.LongoCursoDiasDePrazoFatura,
                    acordoFaturamentoCliente.LongoCursoTipoPrazoFaturamento,
                    LongoCursoDiasSemanaFatura = acordoFaturamentoCliente.LongoCursoDiasSemanaFatura.Select(o => o).ToList(),
                    LongoCursoDiasMesFatura = acordoFaturamentoCliente.LongoCursoDiasMesFatura.Select(o => o).ToList(),
                    acordoFaturamentoCliente.LongoCursoEmail,
                    acordoFaturamentoCliente.LongoCursoNaoEnviarEmailFaturaAutomaticamente,

                    //CustoExtra
                    acordoFaturamentoCliente.FaturamentoPermissaoExclusivaCustoExtra,
                    acordoFaturamentoCliente.CustoExtraGerarFaturamentoAVista,
                    acordoFaturamentoCliente.CustoExtraDiasDePrazoFatura,
                    acordoFaturamentoCliente.CustoExtraTipoPrazoFaturamento,
                    CustoExtraDiasSemanaFatura = acordoFaturamentoCliente.CustoExtraDiasSemanaFatura.Select(o => o).ToList(),
                    CustoExtraDiasMesFatura = acordoFaturamentoCliente.CustoExtraDiasMesFatura.Select(o => o).ToList(),
                    acordoFaturamentoCliente.CustoExtraEmail,
                    acordoFaturamentoCliente.CustoExtraNaoEnviarEmailFaturaAutomaticamente,

                    //TakeorPay
                    acordoFaturamentoCliente.ConsiderarParametrosDeFreteCabotagem,
                    acordoFaturamentoCliente.TakeOrPayDiasDePrazoFatura,
                    acordoFaturamentoCliente.DiasPrazoFaturamentoDnD,
                    acordoFaturamentoCliente.DiasPrazoVencimentoNotaDebito,

                    EmailsCabotagem = (from obj in acordoFaturamentoCliente.EmailsCabotagem
                                       select new
                                       {
                                           CodigoEmailCabotagem = obj.Codigo,
                                           EmailCabotagem = obj.Email,
                                           PessoaCabotagem = new
                                           {
                                               Codigo = obj.Pessoa?.Codigo ?? 0,
                                               Descricao = obj.Pessoa?.Descricao ?? string.Empty
                                           }
                                       }).ToList(),

                    EmailsLongoCurso = (from obj in acordoFaturamentoCliente.EmailsLongoCurso
                                       select new
                                       {
                                           CodigoEmailLongoCurso = obj.Codigo,
                                           EmailLongoCurso = obj.Email,
                                           PessoaLongoCurso = new
                                           {
                                               Codigo = obj.Pessoa?.Codigo ?? 0,
                                               Descricao = obj.Pessoa?.Descricao ?? string.Empty
                                           }
                                       }).ToList(),

                    EmailsCustoExtra = (from obj in acordoFaturamentoCliente.EmailsCustoExtra
                                       select new
                                       {
                                           CodigoEmailCustoExtra = obj.Codigo,
                                           EmailCustoExtra = obj.Email,
                                           PessoaCustoExtra = new
                                           {
                                               Codigo = obj.Pessoa?.Codigo ?? 0,
                                               Descricao = obj.Pessoa?.Descricao ?? string.Empty
                                           }
                                       }).ToList()
                };

                return new JsonpResult(dynAcordoFaturamentoCliente);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordoFaturamentoCliente = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoFaturamentoCliente = repAcordoFaturamentoCliente.BuscarPorCodigo(codigo, true);

                if (acordoFaturamentoCliente == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repAcordoFaturamentoCliente.Deletar(acordoFaturamentoCliente, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, false, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherAcordoFaturamentoCliente(Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoFaturamentoCliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

            double pessoa = Request.GetDoubleParam("Pessoa");
            int grupoPessoa = Request.GetIntParam("GrupoPessoa");
            bool.TryParse(Request.Params("Status"), out bool status);

            acordoFaturamentoCliente.Status = status;
            acordoFaturamentoCliente.TipoPessoa = Request.GetEnumParam<TipoPessoa>("TipoPessoa");
            acordoFaturamentoCliente.Pessoa = pessoa > 0 ? repPessoa.BuscarPorCPFCNPJ(pessoa) : null;
            acordoFaturamentoCliente.GrupoPessoas = grupoPessoa > 0 ? repGrupoPessoa.BuscarPorCodigo(grupoPessoa) : null;
            acordoFaturamentoCliente.FaturamentoPermissaoExclusiva = Request.GetBoolParam("FaturamentoPermissaoExclusiva");
            acordoFaturamentoCliente.NaoEnviarEmailFaturaAutomaticamente = Request.GetBoolParam("NaoEnviarEmailFaturaAutomaticamente");

            //Cabotagem
            acordoFaturamentoCliente.FaturamentoPermissaoExclusivaCabotagem = Request.GetBoolParam("FaturamentoPermissaoExclusivaCabotagem");
            acordoFaturamentoCliente.CabotagemGerarFaturamentoAVista = Request.GetBoolParam("CabotagemGerarFaturamentoAVista");
            acordoFaturamentoCliente.CabotagemDiasDePrazoFatura = Request.GetIntParam("CabotagemDiasDePrazoFatura");
            acordoFaturamentoCliente.CabotagemTipoPrazoFaturamento = Request.GetEnumParam<TipoPrazoFaturamento>("CabotagemTipoPrazoFaturamento");
            acordoFaturamentoCliente.CabotagemEmail = Request.GetStringParam("CabotagemEmail");
            acordoFaturamentoCliente.CabotagemNaoEnviarEmailFaturaAutomaticamente = Request.GetBoolParam("CabotagemNaoEnviarEmailFaturaAutomaticamente");

            //LongoCurso
            acordoFaturamentoCliente.FaturamentoPermissaoExclusivaLongoCurso = Request.GetBoolParam("FaturamentoPermissaoExclusivaLongoCurso");
            acordoFaturamentoCliente.LongoCursoGerarFaturamentoAVista = Request.GetBoolParam("LongoCursoGerarFaturamentoAVista");
            acordoFaturamentoCliente.LongoCursoDiasDePrazoFatura = Request.GetIntParam("LongoCursoDiasDePrazoFatura");
            acordoFaturamentoCliente.LongoCursoTipoPrazoFaturamento = Request.GetEnumParam<TipoPrazoFaturamento>("LongoCursoTipoPrazoFaturamento");
            acordoFaturamentoCliente.LongoCursoEmail = Request.GetStringParam("LongoCursoEmail");
            acordoFaturamentoCliente.LongoCursoNaoEnviarEmailFaturaAutomaticamente = Request.GetBoolParam("LongoCursoNaoEnviarEmailFaturaAutomaticamente");

            //CustoExtra
            acordoFaturamentoCliente.FaturamentoPermissaoExclusivaCustoExtra = Request.GetBoolParam("FaturamentoPermissaoExclusivaCustoExtra");
            acordoFaturamentoCliente.CustoExtraGerarFaturamentoAVista = Request.GetBoolParam("CustoExtraGerarFaturamentoAVista");
            acordoFaturamentoCliente.CustoExtraDiasDePrazoFatura = Request.GetIntParam("CustoExtraDiasDePrazoFatura");
            acordoFaturamentoCliente.CustoExtraTipoPrazoFaturamento = Request.GetEnumParam<TipoPrazoFaturamento>("CustoExtraTipoPrazoFaturamento");
            acordoFaturamentoCliente.CustoExtraEmail = Request.GetStringParam("CustoExtraEmail");
            acordoFaturamentoCliente.CustoExtraNaoEnviarEmailFaturaAutomaticamente = Request.GetBoolParam("CustoExtraNaoEnviarEmailFaturaAutomaticamente");

            //TakeOrPay
            acordoFaturamentoCliente.ConsiderarParametrosDeFreteCabotagem = Request.GetBoolParam("ConsiderarParametrosDeFreteCabotagem");
            acordoFaturamentoCliente.TakeOrPayDiasDePrazoFatura = Request.GetIntParam("TakeOrPayDiasDePrazoFatura");
            acordoFaturamentoCliente.DiasPrazoFaturamentoDnD = Request.GetIntParam("DiasPrazoFaturamentoDnD");
            acordoFaturamentoCliente.DiasPrazoVencimentoNotaDebito = Request.GetIntParam("DiasPrazoVencimentoNotaDebito");
        }

        private string SalvarAcordoFaturamentoClienteListasAuditarManual(Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoFaturamentoCliente, Repositorio.UnitOfWork unitOfWork)
        {
            string diasSemanaCabotagemAnterior = "", diasMesCabotagemAnterior = "", diasSemanaLongoCursoAnterior = "", diasMesLongoCursoAnterior = "", diasSemanaCustoExtraAnterior = "", diasMesCustoExtraAnterior = "";

            if (acordoFaturamentoCliente.CabotagemDiasSemanaFatura == null)
                acordoFaturamentoCliente.CabotagemDiasSemanaFatura = new List<DiaSemana>();
            else
            {
                diasSemanaCabotagemAnterior = string.Join(", ", acordoFaturamentoCliente.CabotagemDiasSemanaFatura);
                acordoFaturamentoCliente.CabotagemDiasSemanaFatura.Clear();
            }

            if (acordoFaturamentoCliente.LongoCursoDiasSemanaFatura == null)
                acordoFaturamentoCliente.LongoCursoDiasSemanaFatura = new List<DiaSemana>();
            else
            {
                diasSemanaLongoCursoAnterior = string.Join(", ", acordoFaturamentoCliente.LongoCursoDiasSemanaFatura);
                acordoFaturamentoCliente.LongoCursoDiasSemanaFatura.Clear();
            }

            if (acordoFaturamentoCliente.CustoExtraDiasSemanaFatura == null)
                acordoFaturamentoCliente.CustoExtraDiasSemanaFatura = new List<DiaSemana>();
            else
            {
                diasSemanaCustoExtraAnterior = string.Join(", ", acordoFaturamentoCliente.CustoExtraDiasSemanaFatura);
                acordoFaturamentoCliente.CustoExtraDiasSemanaFatura.Clear();
            }

            if (acordoFaturamentoCliente.CabotagemDiasMesFatura == null)
                acordoFaturamentoCliente.CabotagemDiasMesFatura = new List<int>();
            else
            {
                diasMesCabotagemAnterior = string.Join(", ", acordoFaturamentoCliente.CabotagemDiasMesFatura);
                acordoFaturamentoCliente.CabotagemDiasMesFatura.Clear();
            }

            if (acordoFaturamentoCliente.LongoCursoDiasMesFatura == null)
                acordoFaturamentoCliente.LongoCursoDiasMesFatura = new List<int>();
            else
            {
                diasMesLongoCursoAnterior = string.Join(", ", acordoFaturamentoCliente.LongoCursoDiasMesFatura);
                acordoFaturamentoCliente.LongoCursoDiasMesFatura.Clear();
            }

            if (acordoFaturamentoCliente.CustoExtraDiasMesFatura == null)
                acordoFaturamentoCliente.CustoExtraDiasMesFatura = new List<int>();
            else
            {
                diasMesCustoExtraAnterior = string.Join(", ", acordoFaturamentoCliente.CustoExtraDiasMesFatura);
                acordoFaturamentoCliente.CustoExtraDiasMesFatura.Clear();
            }

            List<DiaSemana> cabotagemDiasSemanaFatura = Request.GetListEnumParam<DiaSemana>("CabotagemDiasSemanaFatura");
            foreach (var diaSemanaFatura in cabotagemDiasSemanaFatura)
                acordoFaturamentoCliente.CabotagemDiasSemanaFatura.Add(diaSemanaFatura);

            List<DiaSemana> longoCursoDiasSemanaFatura = Request.GetListEnumParam<DiaSemana>("LongoCursoDiasSemanaFatura");
            foreach (var diaSemanaFatura in longoCursoDiasSemanaFatura)
                acordoFaturamentoCliente.LongoCursoDiasSemanaFatura.Add(diaSemanaFatura);

            List<DiaSemana> custoExtraDiasSemanaFatura = Request.GetListEnumParam<DiaSemana>("CustoExtraDiasSemanaFatura");
            foreach (var diaSemanaFatura in custoExtraDiasSemanaFatura)
                acordoFaturamentoCliente.CustoExtraDiasSemanaFatura.Add(diaSemanaFatura);

            List<int> cabotagemDiasMesFatura = Request.GetListParam<int>("CabotagemDiasMesFatura");
            foreach (var diaMesFatura in cabotagemDiasMesFatura)
                acordoFaturamentoCliente.CabotagemDiasMesFatura.Add(diaMesFatura);

            List<int> longoCursoDiasMesFatura = Request.GetListParam<int>("LongoCursoDiasMesFatura");
            foreach (var diaMesFatura in longoCursoDiasMesFatura)
                acordoFaturamentoCliente.LongoCursoDiasMesFatura.Add(diaMesFatura);

            List<int> custoExtraDiasMesFatura = Request.GetListParam<int>("CustoExtraDiasMesFatura");
            foreach (var diaMesFatura in custoExtraDiasMesFatura)
                acordoFaturamentoCliente.CustoExtraDiasMesFatura.Add(diaMesFatura);

            if (acordoFaturamentoCliente.Codigo > 0)
            {
                string diasSemanaAtuais = string.Join(", ", acordoFaturamentoCliente.CabotagemDiasSemanaFatura);
                if (!diasSemanaCabotagemAnterior.Equals(diasSemanaAtuais))
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, acordoFaturamentoCliente, null, "Alterou Cabotagem dias da semana de '" + diasSemanaCabotagemAnterior + "' para '" + diasSemanaAtuais + "'.", unitOfWork);

                string diasMesAtuais = string.Join(", ", acordoFaturamentoCliente.CabotagemDiasMesFatura);
                if (!diasMesCabotagemAnterior.Equals(diasMesAtuais))
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, acordoFaturamentoCliente, null, "Alterou Cabotagem dias do mês de '" + diasMesCabotagemAnterior + "' para '" + diasMesAtuais + "'.", unitOfWork);


                diasSemanaAtuais = string.Join(", ", acordoFaturamentoCliente.LongoCursoDiasSemanaFatura);
                if (!diasSemanaLongoCursoAnterior.Equals(diasSemanaAtuais))
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, acordoFaturamentoCliente, null, "Alterou Longo Curso dias da semana de '" + diasSemanaLongoCursoAnterior + "' para '" + diasSemanaAtuais + "'.", unitOfWork);

                diasMesAtuais = string.Join(", ", acordoFaturamentoCliente.LongoCursoDiasMesFatura);
                if (!diasMesLongoCursoAnterior.Equals(diasMesAtuais))
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, acordoFaturamentoCliente, null, "Alterou Longo Curso dias do mês de '" + diasMesLongoCursoAnterior + "' para '" + diasMesAtuais + "'.", unitOfWork);


                diasSemanaAtuais = string.Join(", ", acordoFaturamentoCliente.CustoExtraDiasSemanaFatura);
                if (!diasSemanaCustoExtraAnterior.Equals(diasSemanaAtuais))
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, acordoFaturamentoCliente, null, "Alterou Custo Extra dias da semana de '" + diasSemanaCustoExtraAnterior + "' para '" + diasSemanaAtuais + "'.", unitOfWork);

                diasMesAtuais = string.Join(", ", acordoFaturamentoCliente.CustoExtraDiasMesFatura);
                if (!diasMesCustoExtraAnterior.Equals(diasMesAtuais))
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, acordoFaturamentoCliente, null, "Alterou Custo Extra dias do mês de '" + diasMesCustoExtraAnterior + "' para '" + diasMesAtuais + "'.", unitOfWork);
            }

            if ((acordoFaturamentoCliente.Pessoa != null && acordoFaturamentoCliente.Pessoa.PermiteFinalDeSemana == false) ||
                (acordoFaturamentoCliente.GrupoPessoas != null && acordoFaturamentoCliente.GrupoPessoas.PermiteFinalDeSemana == false))
            {
                foreach (var diaSemana in acordoFaturamentoCliente.CabotagemDiasSemanaFatura)
                    if (diaSemana == DiaSemana.Sabado || diaSemana == DiaSemana.Domingo)
                        return "Foi selecionado um dia do final de semana para a Cabotagem.";

                foreach (var diaSemana in acordoFaturamentoCliente.LongoCursoDiasSemanaFatura)
                    if (diaSemana == DiaSemana.Sabado || diaSemana == DiaSemana.Domingo)
                        return "Foi selecionado um dia do final de semana para o Longo Curso.";

                foreach (var diaSemana in acordoFaturamentoCliente.CustoExtraDiasSemanaFatura)
                    if (diaSemana == DiaSemana.Sabado || diaSemana == DiaSemana.Domingo)
                        return "Foi selecionado um dia do final de semana para o Custo Extra.";
            }

            return string.Empty;
        }

        private void SalvarListaEmailsCabotagem(Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoFaturamentoCliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem repAcordoFaturamentoEmailCabotagem = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            dynamic emailsCabotagem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("EmailsCabotagem"));
            List<int> codigosEmailsCabotagem = new List<int>();

            foreach (var emailCabotagem in emailsCabotagem)
            {
                int? codigo = ((string)emailCabotagem.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    codigosEmailsCabotagem.Add(codigo.Value);
            }

            List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem> emailsCabotagemExistentes = repAcordoFaturamentoEmailCabotagem.BuscarPorAcordoFaturamento(acordoFaturamentoCliente.Codigo);
            List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem> emailsCabotagemDeletar = (from email in emailsCabotagemExistentes where !codigosEmailsCabotagem.Contains(email.Codigo) select email).ToList();

            foreach (Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem emailCabotagemDeletar in emailsCabotagemDeletar)
            {
                Servicos.Auditoria.Auditoria.Auditar(Auditado, acordoFaturamentoCliente, $"E-mail de Cabotagem {emailCabotagemDeletar.Descricao} deletada do acordo de faturamento.", unitOfWork);
                emailsCabotagemExistentes.Remove(emailCabotagemDeletar);
                repAcordoFaturamentoEmailCabotagem.Deletar(emailCabotagemDeletar);
            }

            foreach (var emailCabotagem in emailsCabotagem)
            {
                Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem emailCabotagemAdicionar = null;

                int? codigo = ((string)emailCabotagem.CodigoEmailCabotagem).ToNullableInt();

                if (codigo.HasValue)
                    emailCabotagemAdicionar = (from obj in emailsCabotagemExistentes where obj.Codigo == codigo.Value select obj).FirstOrDefault();

                if (emailCabotagemAdicionar == null)
                    emailCabotagemAdicionar = new Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem();
                else
                    emailCabotagemAdicionar.Initialize();

                double codigoCliente = ((string)emailCabotagem.PessoaCabotagem.Codigo).ToDouble();

                emailCabotagemAdicionar.AcordoFaturamentoCliente = acordoFaturamentoCliente;
                emailCabotagemAdicionar.Email = (string)emailCabotagem.EmailCabotagem;
                emailCabotagemAdicionar.Pessoa = codigoCliente > 0 ? repCliente.BuscarPorCPFCNPJ(codigoCliente) : null;

                if (emailCabotagemAdicionar.Codigo > 0)
                {
                    repAcordoFaturamentoEmailCabotagem.Atualizar(emailCabotagemAdicionar, Auditado);

                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = emailCabotagemAdicionar.GetChanges();

                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, acordoFaturamentoCliente, alteracoes, $"Alterou o e-mail de Cabotagem do acordo de faturamento {emailCabotagemAdicionar.Descricao}.", unitOfWork);
                }
                else
                {
                    repAcordoFaturamentoEmailCabotagem.Inserir(emailCabotagemAdicionar, Auditado);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, acordoFaturamentoCliente, $"Adicionou o e-mail de Cabotagem para o acordo de faturamento { emailCabotagemAdicionar.Descricao}.", unitOfWork);
                }
            }
        }

        private void SalvarListaEmailsLongoCurso(Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoFaturamentoCliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso repAcordoFaturamentoEmailLongoCurso = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            dynamic emailsLongoCurso = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("EmailsLongoCurso"));
            List<int> codigosEmailsLongoCurso = new List<int>();

            foreach (var emailLongoCurso in emailsLongoCurso)
            {
                int? codigo = ((string)emailLongoCurso.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    codigosEmailsLongoCurso.Add(codigo.Value);
            }

            List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso> emailsLongoCursoExistentes = repAcordoFaturamentoEmailLongoCurso.BuscarPorAcordoFaturamento(acordoFaturamentoCliente.Codigo);
            List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso> emailsLongoCursoDeletar = (from email in emailsLongoCursoExistentes where !codigosEmailsLongoCurso.Contains(email.Codigo) select email).ToList();

            foreach (Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso emailLongoCursoDeletar in emailsLongoCursoDeletar)
            {
                Servicos.Auditoria.Auditoria.Auditar(Auditado, acordoFaturamentoCliente, $"E-mail de Longo Curso {emailLongoCursoDeletar.Descricao} deletada do acordo de faturamento.", unitOfWork);
                emailsLongoCursoExistentes.Remove(emailLongoCursoDeletar);
                repAcordoFaturamentoEmailLongoCurso.Deletar(emailLongoCursoDeletar);
            }

            foreach (var emailLongoCurso in emailsLongoCurso)
            {
                Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso emailLongoCursoAdicionar = null;

                int? codigo = ((string)emailLongoCurso.CodigoEmailLongoCurso).ToNullableInt();

                if (codigo.HasValue)
                    emailLongoCursoAdicionar = (from obj in emailsLongoCursoExistentes where obj.Codigo == codigo.Value select obj).FirstOrDefault();

                if (emailLongoCursoAdicionar == null)
                    emailLongoCursoAdicionar = new Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso();
                else
                    emailLongoCursoAdicionar.Initialize();

                double codigoCliente = ((string)emailLongoCurso.PessoaLongoCurso.Codigo).ToDouble();

                emailLongoCursoAdicionar.AcordoFaturamentoCliente = acordoFaturamentoCliente;
                emailLongoCursoAdicionar.Email = (string)emailLongoCurso.EmailLongoCurso;
                emailLongoCursoAdicionar.Pessoa = codigoCliente > 0 ? repCliente.BuscarPorCPFCNPJ(codigoCliente) : null;

                if (emailLongoCursoAdicionar.Codigo > 0)
                {
                    repAcordoFaturamentoEmailLongoCurso.Atualizar(emailLongoCursoAdicionar, Auditado);

                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = emailLongoCursoAdicionar.GetChanges();

                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, acordoFaturamentoCliente, alteracoes, $"Alterou o e-mail de Longo Curso do acordo de faturamento {emailLongoCursoAdicionar.Descricao}.", unitOfWork);
                }
                else
                {
                    repAcordoFaturamentoEmailLongoCurso.Inserir(emailLongoCursoAdicionar, Auditado);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, acordoFaturamentoCliente, $"Adicionou o e-mail de Longo Curso para o acordo de faturamento { emailLongoCursoAdicionar.Descricao}.", unitOfWork);
                }
            }
        }

        private void SalvarListaEmailsCustoExtra(Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoFaturamentoCliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra repAcordoFaturamentoEmailCustoExtra = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            dynamic emailsCustoExtra = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("EmailsCustoExtra"));
            List<int> codigosEmailsCustoExtra = new List<int>();

            foreach (var emailCustoExtra in emailsCustoExtra)
            {
                int? codigo = ((string)emailCustoExtra.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    codigosEmailsCustoExtra.Add(codigo.Value);
            }

            List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra> emailsCustoExtraExistentes = repAcordoFaturamentoEmailCustoExtra.BuscarPorAcordoFaturamento(acordoFaturamentoCliente.Codigo);
            List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra> emailsCustoExtraDeletar = (from email in emailsCustoExtraExistentes where !codigosEmailsCustoExtra.Contains(email.Codigo) select email).ToList();

            foreach (Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra emailCustoExtraDeletar in emailsCustoExtraDeletar)
            {
                Servicos.Auditoria.Auditoria.Auditar(Auditado, acordoFaturamentoCliente, $"E-mail de Custo Extra {emailCustoExtraDeletar.Descricao} deletada do acordo de faturamento.", unitOfWork);
                emailsCustoExtraExistentes.Remove(emailCustoExtraDeletar);
                repAcordoFaturamentoEmailCustoExtra.Deletar(emailCustoExtraDeletar);
            }

            foreach (var emailCustoExtra in emailsCustoExtra)
            {
                Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra emailCustoExtraAdicionar = null;

                int? codigo = ((string)emailCustoExtra.CodigoEmailCustoExtra).ToNullableInt();

                if (codigo.HasValue)
                    emailCustoExtraAdicionar = (from obj in emailsCustoExtraExistentes where obj.Codigo == codigo.Value select obj).FirstOrDefault();

                if (emailCustoExtraAdicionar == null)
                    emailCustoExtraAdicionar = new Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra();
                else
                    emailCustoExtraAdicionar.Initialize();

                double codigoCliente = ((string)emailCustoExtra.PessoaCustoExtra.Codigo).ToDouble();

                emailCustoExtraAdicionar.AcordoFaturamentoCliente = acordoFaturamentoCliente;
                emailCustoExtraAdicionar.Email = (string)emailCustoExtra.EmailCustoExtra;
                emailCustoExtraAdicionar.Pessoa = codigoCliente > 0 ? repCliente.BuscarPorCPFCNPJ(codigoCliente) : null;

                if (emailCustoExtraAdicionar.Codigo > 0)
                {
                    repAcordoFaturamentoEmailCustoExtra.Atualizar(emailCustoExtraAdicionar, Auditado);

                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = emailCustoExtraAdicionar.GetChanges();

                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, acordoFaturamentoCliente, alteracoes, $"Alterou o e-mail de Custo Extra do acordo de faturamento {emailCustoExtraAdicionar.Descricao}.", unitOfWork);
                }
                else
                {
                    repAcordoFaturamentoEmailCustoExtra.Inserir(emailCustoExtraAdicionar, Auditado);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, acordoFaturamentoCliente, $"Adicionou o e-mail de Custo Extr para o acordo de faturamento { emailCustoExtraAdicionar.Descricao}.", unitOfWork);
                }
            }
        }


        #endregion
    }
}
