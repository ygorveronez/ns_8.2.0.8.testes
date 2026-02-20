using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.PagamentoMotorista
{
    [CustomAuthorize("PagamentosMotoristas/PagamentoMotoristaTMSLote")]
    public class PagamentoMotoristaTMSLoteController : BaseController
    {
		#region Construtores

		public PagamentoMotoristaTMSLoteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarMovimentacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                string mensagemRetorno = string.Empty;
                if (!SalvarMovimentosSelecionados(unitOfWork, out string msgErro, out mensagemRetorno, out int quantiaMotoristasSemAcertoEmAndamento) && quantiaMotoristasSemAcertoEmAndamento == 0)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, msgErro);
                }

                unitOfWork.CommitChanges();

                if (quantiaMotoristasSemAcertoEmAndamento > 0)
                    return new JsonpResult(false, true, msgErro);
                else
                    return new JsonpResult(new { MensagemRetorno = mensagemRetorno });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar os movimentos dos motoristas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaMotoristasAtivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(unitOfWork);
                Repositorio.Usuario repUsuarios = new Repositorio.Usuario(unitOfWork);

                Models.Grid.EditableCell editableValor = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 8);
                Models.Grid.EditableCell editableValorString = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aString, 500);

                //Campos para o pagamento
                int.TryParse(Request.Params("TipoPagamentoMotorista"), out int codigoTipoPagamentoMotorista);
                int.TryParse(Request.Params("PlanoDeContaDebito"), out int codigoContaDebito);
                int.TryParse(Request.Params("PlanoDeContaCredito"), out int codigoContaCredito);

                decimal.TryParse(Request.Params("Valor"), out decimal valor);

                DateTime.TryParse(Request.Params("DataPagamento"), out DateTime dataPagamento);
                string observacao = Request.Params("Observacao");

                //Filtros
                int codigoCentroResultado = Request.GetIntParam("CentroResultado");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoTipoPagamentoMotorista", false);
                grid.AdicionarCabecalho("CodigoContaDebito", false);
                grid.AdicionarCabecalho("CodigoContaCredito", false);
                grid.AdicionarCabecalho("CPF", "CPF_Formatado", 9, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nome", "Nome", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data", "Data", 9, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Tipo do Pagamento", "TipoDoPagamento", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veículos", "Veiculos", 11, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "Valor", 9, Models.Grid.Align.right, false, false, false, false, true, editableValor);
                grid.AdicionarCabecalho("Observação", "Observacao", 30, Models.Grid.Align.left, false, false, false, false, true, editableValorString);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "CPF_Formatado")
                    propOrdenar = "CPF";

                int codigoMotorista = 0;
                if (codigoContaCredito == 0)
                    codigoMotorista = -5;

                grid.limite = 1000;
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo pagamentoMotoristaTipo = repPagamentoMotoristaTipo.BuscarPorCodigo(codigoTipoPagamentoMotorista);

                IList<Dominio.ObjetosDeValor.Embarcador.Transportadores.ConsultaPagamentoMotorista> listaMotoristas = repUsuarios.ConsultarConsultaPagamentoMotorista(observacao, valor.ToString("n2"), pagamentoMotoristaTipo?.Descricao, dataPagamento.ToString("dd/MM/yyyy"), codigoContaCredito, codigoContaDebito, codigoTipoPagamentoMotorista, codigoMotorista, TipoMotorista.Proprio, true, codigoCentroResultado, grid.inicio, 1000, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena);
                grid.setarQuantidadeTotal(repUsuarios.ContarConsultarConsultaPagamentoMotorista(codigoMotorista, TipoMotorista.Proprio, true, codigoCentroResultado));
                var lista = (from p in listaMotoristas
                             orderby p.Nome
                             select new
                             {
                                 p.Codigo,
                                 CodigoTipoPagamentoMotorista = codigoTipoPagamentoMotorista,
                                 CodigoContaDebito = codigoContaDebito,
                                 CodigoContaCredito = codigoContaCredito,
                                 p.CPF_Formatado,
                                 p.Nome,
                                 Data = dataPagamento.ToString("dd/MM/yyyy HH:mm"),
                                 TipoDoPagamento = pagamentoMotoristaTipo?.Descricao,
                                 Valor = valor.ToString("n2"),
                                 Observacao = observacao,
                                 p.Veiculos
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os motoristas ativos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AlterarValorMotorista()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            try
            {
                unitOfWork.Start();
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                decimal valor = 0;
                decimal.TryParse(Request.Params("Valor"), out valor);

                Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(codigo);

                var retorno = RetornarObjetoDadosGrid(unitOfWork, codigo, valor, motorista);
                unitOfWork.CommitChanges();
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar o valor a ser movimentado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic RetornarObjetoDadosGrid(Repositorio.UnitOfWork unitOfWork, int codigo, decimal valor, Dominio.Entidades.Usuario motorista)
        {
            int codioTipoPagamentoMotorista = 0, codigoContaDebito = 0, codigoContaCredito = 0, codigoMotorista = 0;
            int.TryParse(Request.Params("CodigoTipoPagamentoMotorista"), out codioTipoPagamentoMotorista);
            int.TryParse(Request.Params("CodigoContaDebito"), out codigoContaDebito);
            int.TryParse(Request.Params("CodigoContaCredito"), out codigoContaCredito);
            int.TryParse(Request.Params("Codigo"), out codigoMotorista);
            DateTime dataMovimento;
            DateTime.TryParse(Request.Params("Data"), out dataMovimento);
            string observacao = Request.Params("Observacao");
            string documento = Request.Params("Documento");

            var retorno = new
            {
                Codigo = codigo,
                CodigoTipoPagamentoMotorista = codioTipoPagamentoMotorista,
                CodigoContaDebito = codigoContaDebito,
                CodigoContaCredito = codigoContaCredito,
                motorista.CPF_Formatado,
                motorista.Nome,
                Valor = valor.ToString("n2"),
                Data = dataMovimento.ToString("dd/MM/yyyy"),
                Observacao = observacao
            };
            return retorno;
        }

        private bool SalvarMovimentosSelecionados(Repositorio.UnitOfWork unidadeDeTrabalho, out string msgErro, out string mensagemRetorno, out int quantiaMotoristasSemAcertoEmAndamento)
        {
            msgErro = "";
            mensagemRetorno = "";
            System.Text.StringBuilder mensagem = new System.Text.StringBuilder();
            var quantiaMotoristas = 0;
            quantiaMotoristasSemAcertoEmAndamento = 0;

            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unidadeDeTrabalho);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unidadeDeTrabalho);

            dynamic listaMotoristas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("MotoristasSelecionados"));
            if (listaMotoristas != null)
            {
                foreach (var mot in listaMotoristas)
                {
                    Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS();

                    if ((int)mot.CodigoContaDebito.val > 0)
                        pagamentoMotorista.PlanoDeContaDebito = repPlanoConta.BuscarPorCodigo((int)mot.CodigoContaDebito.val);
                    else
                        pagamentoMotorista.PlanoDeContaDebito = null;
                    if ((int)mot.CodigoContaCredito.val > 0)
                        pagamentoMotorista.PlanoDeContaCredito = repPlanoConta.BuscarPorCodigo((int)mot.CodigoContaCredito.val);
                    else
                        pagamentoMotorista.PlanoDeContaCredito = null;
                    pagamentoMotorista.Carga = null;
                    pagamentoMotorista.Chamado = null;
                    if ((int)mot.Codigo.val > 0)
                        pagamentoMotorista.Motorista = repUsuario.BuscarPorCodigo((int)mot.Codigo.val);
                    if ((int)mot.CodigoTipoPagamentoMotorista.val > 0)
                        pagamentoMotorista.PagamentoMotoristaTipo = repPagamentoMotoristaTipo.BuscarPorCodigo((int)mot.CodigoTipoPagamentoMotorista.val);

                    DateTime dataPagamento;
                    DateTime.TryParse((string)mot.Data.val, out dataPagamento);
                    pagamentoMotorista.DataPagamento = dataPagamento;

                    decimal valor = 0;
                    if (!string.IsNullOrWhiteSpace((string)mot.Valor.val))
                        valor = Utilidades.Decimal.Converter((string)mot.Valor.val);
                    pagamentoMotorista.Valor = valor;

                    pagamentoMotorista.Observacao = (string)mot.Observacao.val;
                    pagamentoMotorista.DataVencimentoTituloPagar = dataPagamento;
                    pagamentoMotorista.SaldoDescontado = 0;
                    pagamentoMotorista.SaldoDiariaMotorista = 0;
                    pagamentoMotorista.Usuario = this.Usuario;
                    pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.AgInformacoes;
                    pagamentoMotorista.Data = DateTime.Now.Date;
                    pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Iniciada;
                    pagamentoMotorista.Numero = repPagamentoMotorista.BuscarProximoNumero();
                    pagamentoMotorista.PagamentoLiberado = true;

                    TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = pagamentoMotorista.PagamentoMotoristaTipo?.TipoIntegracaoPagamentoMotorista ?? TipoIntegracaoPagamentoMotorista.SemIntegracao;

                    if (pagamentoMotorista.PagamentoMotoristaTipo != null && pagamentoMotorista.Motorista != null && pagamentoMotorista.PagamentoMotoristaTipo.PermitirLancarPagamentoContendoAcertoEmAndamento)
                    {
                        if (!repAcertoViagem.ContemAcertoEmAndamento(pagamentoMotorista.Motorista.Codigo))
                        {
                            if (quantiaMotoristas == 0)
                                mensagem.AppendLine($"Não foi localizado um Acerto de Viagem com o status Em Andamento para - {pagamentoMotorista.Motorista.Nome}");
                            else if (quantiaMotoristas > 0)
                                mensagem.Append($", {pagamentoMotorista.Motorista.Nome}");

                            quantiaMotoristas++;

                            continue;
                        }
                    }

                    if (pagamentoMotorista.PagamentoMotoristaTipo != null && pagamentoMotorista.PagamentoMotoristaTipo.GerarMovimentoAutomatico && (pagamentoMotorista.PlanoDeContaCredito == null || pagamentoMotorista.PlanoDeContaDebito == null))
                    {
                        msgErro = "Favor selecione os Planos de Contas para este Tipo de Pagamento.";
                        return false;
                    }

                    if (!ConfiguracaoEmbarcador.PermitirPagamentoMotoristaSemCarga && pagamentoMotorista.Carga == null && pagamentoMotorista.PagamentoMotoristaTipo != null &&
                        (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PamCard || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PagBem || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Extratta
                        || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Target))
                    {
                        msgErro = "Favor selecione uma Carga para este Tipo de Pagamento.";
                        return false;
                    }

                    if (ConfiguracaoEmbarcador.PermitirPagamentoMotoristaSemCarga && pagamentoMotorista.Motorista != null && pagamentoMotorista.Carga == null && pagamentoMotorista.PagamentoMotoristaTipo != null &&
                        (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PamCard || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PagBem || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Extratta
                        || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Target))
                    {
                        Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorMotorista(pagamentoMotorista.Motorista.Codigo, "0");
                        if (veiculo == null)
                        {
                            msgErro = "O motorista: " + pagamentoMotorista.Motorista.Descricao + " selecionado não está vinculado a nenhum veículo do tipo tração.";
                            return false;
                        }
                    }

                    if (repPagamentoMotorista.ContemPagamentoEmAberto(pagamentoMotorista.Motorista.Codigo))
                    {
                        msgErro = "Já existe um pagamento em aberto para o motorista: " + pagamentoMotorista.Motorista.Descricao + ", favor finalize o mesmo antes de iniciar um novo.";
                        return false;
                    }

                    if (repPagamentoMotorista.ContemPagamentoIdentico(pagamentoMotorista.DataPagamento.Date, pagamentoMotorista.Motorista.Codigo, pagamentoMotorista.PagamentoMotoristaTipo.Codigo, pagamentoMotorista.Valor))
                    {
                        msgErro = "Já existe um pagamento com a mesma Data do Pagamento, Tipo do Pagamento e Valor para o motorista: " + pagamentoMotorista.Motorista.Descricao + ".";
                        return false;
                    }

                    repPagamentoMotorista.Inserir(pagamentoMotorista, Auditado);

                    if (Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.VerificarRegrasAutorizacaoPagamentoMotorista(pagamentoMotorista, TipoServicoMultisoftware, unidadeDeTrabalho, this.Usuario, _conexao.StringConexao, Auditado, out bool contemAprovadorIgualAoOperador))
                    {
                        pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.AutorizacaoPendente;
                        pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.AgAutorizacao;
                    }
                    else
                    {
                        pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.AgIntegracao;
                        pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Integracao;
                    }

                    if (contemAprovadorIgualAoOperador)
                    {
                        Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unidadeDeTrabalho);
                        Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao pagamentoMotoristaAutorizacao = repPagamentoMotoristaAutorizacao.BuscarPrimeiroPorPagamentoUsuario(pagamentoMotorista.Codigo, pagamentoMotorista.Usuario.Codigo);

                        Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.EfetuarAprovacao(pagamentoMotoristaAutorizacao, pagamentoMotorista.Usuario, unidadeDeTrabalho, _conexao.StringConexao, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                        string msgRetorno = "";
                        Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.VerificarSituacaoPagamento(pagamentoMotoristaAutorizacao.PagamentoMotoristaTMS, unidadeDeTrabalho, ref msgRetorno, TipoServicoMultisoftware, Auditado, _conexao.StringConexao, ConfiguracaoEmbarcador, pagamentoMotorista.Usuario);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamentoMotorista, null, "Aprovou o pagamento pelo mesmo operadora da alçada.", unidadeDeTrabalho);
                    }

                    if (tipoIntegracaoPagamentoMotorista.PossuiIntegracao())
                    {
                        Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoMotoristaIntegracaoEnvio = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio();
                        pagamentoMotoristaIntegracaoEnvio.Data = DateTime.Now.Date;
                        pagamentoMotoristaIntegracaoEnvio.NumeroTentativas = 0;
                        pagamentoMotoristaIntegracaoEnvio.PagamentoMotoristaTMS = pagamentoMotorista;
                        pagamentoMotoristaIntegracaoEnvio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                        pagamentoMotoristaIntegracaoEnvio.TipoIntegracaoPagamentoMotorista = tipoIntegracaoPagamentoMotorista;

                        repPagamentoMotoristaIntegracaoEnvio.Inserir(pagamentoMotoristaIntegracaoEnvio);
                    }
                    else if (pagamentoMotorista.SituacaoPagamentoMotorista == SituacaoPagamentoMotorista.AgIntegracao)
                    {
                        pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.Finalizada;
                        pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Integracao;

                        if (ConfiguracaoEmbarcador.ConfirmarPagamentoMotoristaAutomaticamente)
                        {
                            string msgRetorno = "";
                            Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.ConfirmarPagamentoMotorista(ref msgRetorno, pagamentoMotorista.Codigo, ConfiguracaoEmbarcador.TipoMovimentoPagamentoMotorista, Auditado, pagamentoMotorista.Usuario, unidadeDeTrabalho, _conexao.StringConexao, TipoServicoMultisoftware);

                            if (!string.IsNullOrWhiteSpace(msgRetorno))
                                mensagemRetorno += msgRetorno;
                        }
                    }
                }
            }

            if (quantiaMotoristas > 0)
            {
                quantiaMotoristasSemAcertoEmAndamento = quantiaMotoristas;
                mensagem.Append(" - sendo assim não é possível gerar o pagamento.");
                msgErro = mensagem.ToString();
                return false;
            }

            if (quantiaMotoristas == 0)
                quantiaMotoristasSemAcertoEmAndamento = -1;

            return true;
        }

        #endregion
    }
}
