using Repositorio;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class ImportacaoPedido : LongRunningProcessBase<ImportacaoPedido>
    {
        #region Métodos protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            LocalizarProcessarImportacaoPedidoPendente(unitOfWork, cancellationToken);
            LocalizarProcessarImportacaoNotaFiscalPendente(unitOfWork);
            LocalizarProcessarImportacaoPrecoCombustivel(unitOfWork);
            LocalizarProcessarImportacaoPedagio(unitOfWork);
            LocalizarProcessarImportacaoCTeAnterior(unitOfWork, unitOfWorkAdmin);
            LocalizarProcessarImportacaoCTeEmitidoForaEmbarcadorPendente(unitOfWork);
        }

        #endregion

        #region Métodos privados

        private void LocalizarProcessarImportacaoPrecoCombustivel(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivel repImportacaoPrecoCombustivel = new Repositorio.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivel(unitOfWork);

            Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivel importacaoPrecoCombustivel = repImportacaoPrecoCombustivel.BuscarProximaImportacaoPendente();
            if (importacaoPrecoCombustivel != null)
            {
                IndicarInicioDoProcessamento(unitOfWork, repImportacaoPrecoCombustivel, importacaoPrecoCombustivel);

                ProcessarImportacaoPrecoCombustivel(unitOfWork, repImportacaoPrecoCombustivel, importacaoPrecoCombustivel);
            }
        }

        private void LocalizarProcessarImportacaoNotaFiscalPendente(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.ImportacaoNotaFiscal repositorioImportacaoNotaFiscal = new Repositorio.Embarcador.NotaFiscal.ImportacaoNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscal importacaoNotaFiscal = repositorioImportacaoNotaFiscal.BuscarProximaImportacaoPendente();
            if (importacaoNotaFiscal != null)
            {
                if (importacaoNotaFiscal.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoNotaFiscal.Pendente)
                    IndicarInicioDoProcessamentoNotaFiscal(unitOfWork, repositorioImportacaoNotaFiscal, importacaoNotaFiscal);

                ProcessarImportacaoNotaFiscal(unitOfWork, repositorioImportacaoNotaFiscal, importacaoNotaFiscal);
            }
        }

        public void LocalizarProcessarImportacaoPedidoPendente(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {

            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();
            if (configuracaoPedido.ImportarParalelizando)
            {
                Repositorio.Embarcador.Pedidos.ImportacaoPedido repImportacaoPedido = new Repositorio.Embarcador.Pedidos.ImportacaoPedido(unitOfWork);

                // Busca a próximoa planilha pendente
                List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido> lstImportacaoPedidoPendente = repImportacaoPedido.BuscarProximasImportacaoPendente();

                lstImportacaoPedidoPendente = new List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido>();

                if (lstImportacaoPedidoPendente != null)
                {
                    Parallel.ForEach(lstImportacaoPedidoPendente, new ParallelOptions { MaxDegreeOfParallelism = 5 }, (importacaoPedidoPendente, loopState) =>
                    {
                        IndicarInicioDoProcessamentoThread(unitOfWork.StringConexao, importacaoPedidoPendente, cancellationToken);
                    });
                }
            }
            else
            {
                Repositorio.Embarcador.Pedidos.ImportacaoPedido repImportacaoPedido = new Repositorio.Embarcador.Pedidos.ImportacaoPedido(unitOfWork);

                // Busca a próximoa planilha pendente
                Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido importacaoPedidoPendente = repImportacaoPedido.BuscarProximaImportacaoPendente();

                if (importacaoPedidoPendente != null)
                {
                    // Registra o início do processamento da planilha, de "Pendente" para "Processando"
                    IndicarInicioDoProcessamento(unitOfWork, repImportacaoPedido, importacaoPedidoPendente);

                    // Processa a planilha importada
                    ProcessarImportacaoPedido(unitOfWork, repImportacaoPedido, importacaoPedidoPendente, cancellationToken);

                }
            }
        }

        public void ProcessarImportacaoPedido(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Pedidos.ImportacaoPedido repImportacaoPedido, Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido importacaoPedidoPendente, CancellationToken cancellationToken)
        {
            try
            {
                // Importa a planilha gerando pedidos e cargas
                Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido();

                Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoImportacaoPedidoGerarCarga retorno = serPedido.ImportarPedidoGerarCargas(importacaoPedidoPendente, importacaoPedidoPendente.Usuario, _tipoServicoMultisoftware, _clienteMultisoftware, _stringConexaoAdmin, unitOfWork, cancellationToken);

                string mensagem;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido situacao;
                if (retorno.Sucesso)
                {
                    mensagem = $"{retorno.TotalPedidos} pedidos importados e {retorno.TotalCargas} cargas geradas. {retorno.Mensagem}";
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Sucesso;
                }
                else
                {
                    mensagem = retorno.Mensagem;
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro;
                }

                // Registra o fim do processamento da planilha com erro ou sucesso
                IndicarFimDoProcessamento(unitOfWork, repImportacaoPedido, importacaoPedidoPendente, situacao, mensagem);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                unitOfWork.FlushAndClear();

                unitOfWork.Start();
                IndicarFimDoProcessamento(unitOfWork, repImportacaoPedido, importacaoPedidoPendente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro, ex.Message);
                unitOfWork.CommitChanges();

                throw;
            }
        }

        public void ProcessarImportacaoNotaFiscal(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.NotaFiscal.ImportacaoNotaFiscal repositorio, Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscal importacao)
        {
            try
            {
                // Importa a planilha gerando pedidos e cargas
                Servicos.Embarcador.NotaFiscal.NotaFiscalImportacao servicoImportacaoNotaFiscal = new Servicos.Embarcador.NotaFiscal.NotaFiscalImportacao(unitOfWork, _auditado, _tipoServicoMultisoftware, new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarPrimeiroRegistro(), importacao.Usuario);

                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoImportacaoNotaFiscal retorno = servicoImportacaoNotaFiscal.ImportarNotaFiscal(importacao);

                if (retorno.TerminouProcessar)
                {
                    string mensagem;
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoNotaFiscal situacao;
                    if (retorno.Sucesso)
                    {
                        mensagem = $"{retorno.TotalNotas} notas fiscais importadas e {retorno.TotalPedidos} pedidos atualizados.";
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoNotaFiscal.Sucesso;
                    }
                    else
                    {
                        mensagem = retorno.Mensagem;
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoNotaFiscal.Erro;
                    }

                    // Registra o fim do processamento da planilha com erro ou sucesso
                    IndicarFimDoProcessamentoNotaFiscal(unitOfWork, repositorio, importacao, situacao, mensagem);
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                unitOfWork.FlushAndClear();

                unitOfWork.Start();
                IndicarFimDoProcessamentoNotaFiscal(unitOfWork, repositorio, importacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoNotaFiscal.Erro, ex.Message);
                unitOfWork.CommitChanges();

                throw;
            }
        }

        public void ProcessarImportacaoPrecoCombustivel(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivel repositorio, Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivel importacao)
        {
            try
            {
                Servicos.Embarcador.Frota.ImportacaoPrecoCombustivel servicoImportacaoPrecoCombustivel = new Servicos.Embarcador.Frota.ImportacaoPrecoCombustivel();

                Dominio.ObjetosDeValor.Embarcador.Frota.RetornoImportacaoPrecoCombustivel retorno = servicoImportacaoPrecoCombustivel.ImportarPrecoCombustivel(importacao, importacao.Usuario, _tipoServicoMultisoftware, unitOfWork);

                string mensagem;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido situacao;
                if (retorno.Sucesso)
                {
                    mensagem = $"{retorno.TotalTabelasPreco} tabelas de preços importadas.";
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Sucesso;
                }
                else
                {
                    mensagem = retorno.Mensagem;
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro;
                }

                // Registra o fim do processamento da planilha com erro ou sucesso
                IndicarFimDoProcessamentoTabelaPrecoCombustivel(unitOfWork, repositorio, importacao, situacao, mensagem);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                unitOfWork.FlushAndClear();

                unitOfWork.Start();
                IndicarFimDoProcessamentoTabelaPrecoCombustivel(unitOfWork, repositorio, importacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro, ex.Message);
                unitOfWork.CommitChanges();

                throw;
            }
        }

        public void ProcessarImportacaoPedagio(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Frota.ImportacaoPedagio repositorio, Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio importacao)
        {
            try
            {
                Servicos.Embarcador.Frota.ImportacaoPedagio servicoImportacaoPedagio = new Servicos.Embarcador.Frota.ImportacaoPedagio(unitOfWork, importacao.Usuario, _tipoServicoMultisoftware);

                Dominio.ObjetosDeValor.Embarcador.Frota.RetornoImportacaoPedagio retorno = servicoImportacaoPedagio.ImportarPedagio(importacao);

                string mensagem;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedagio situacao;

                if (retorno.Sucesso)
                {
                    mensagem = $"{retorno.TotalRegistros} pedágios importados.";
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedagio.Sucesso;
                }
                else
                {
                    mensagem = retorno.Mensagem;
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedagio.Erro;
                }

                IndicarFimDoProcessamentoPedagio(unitOfWork, repositorio, importacao, situacao, mensagem);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                unitOfWork.FlushAndClear();

                unitOfWork.Start();
                IndicarFimDoProcessamentoPedagio(unitOfWork, repositorio, importacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedagio.Erro, ex.Message);
                unitOfWork.CommitChanges();

                throw;
            }
        }

        private void IndicarInicioDoProcessamento(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivel repImportacaoPrecoCombustivel, Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivel importacaoPrecoCombustivel)
        {
            // Registra o início do processamento da planilha, de "Pendente" para "Processando"
            unitOfWork.Start();
            try
            {
                importacaoPrecoCombustivel.DataInicioProcessamento = DateTime.Now;
                importacaoPrecoCombustivel.DataFimProcessamento = null;
                importacaoPrecoCombustivel.TotalSegundosProcessamento = null;
                importacaoPrecoCombustivel.Mensagem = null;
                importacaoPrecoCombustivel.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Processando;
                repImportacaoPrecoCombustivel.Atualizar(importacaoPrecoCombustivel);
                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        private void IndicarFimDoProcessamentoTabelaPrecoCombustivel(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivel repositorio, Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivel importacaoPrecoCombustivel, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido situacao, string mensagem)
        {
            if (!importacaoPrecoCombustivel.DataInicioProcessamento.HasValue) importacaoPrecoCombustivel.DataInicioProcessamento = DateTime.Now;
            importacaoPrecoCombustivel.DataFimProcessamento = DateTime.Now;
            importacaoPrecoCombustivel.Situacao = situacao;
            importacaoPrecoCombustivel.Mensagem = mensagem;
            repositorio.Atualizar(importacaoPrecoCombustivel);
        }


        private void IndicarInicioDoProcessamentoThread(string strinconexao, Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido importacaoPedidoPendente, CancellationToken cancellationToken)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                Repositorio.Embarcador.Pedidos.ImportacaoPedido repImportacaoPedido = new Repositorio.Embarcador.Pedidos.ImportacaoPedido(unitOfWork);

                // Registra o início do processamento da planilha, de "Pendente" para "Processando"
                IndicarInicioDoProcessamento(unitOfWork, repImportacaoPedido, importacaoPedidoPendente);

                // Processa a planilha importada
                ProcessarImportacaoPedido(unitOfWork, repImportacaoPedido, importacaoPedidoPendente, cancellationToken);


            }
        }

        private void IndicarInicioDoProcessamento(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Pedidos.ImportacaoPedido repImportacaoPedido, Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido importacaoPedidoPendente)
        {
            // Registra o início do processamento da planilha, de "Pendente" para "Processando"
            unitOfWork.Start();
            try
            {
                if (importacaoPedidoPendente.Usuario != null)
                {
                    Repositorio.Usuario repUsuarios = new Repositorio.Usuario(unitOfWork);
                    importacaoPedidoPendente.Usuario = repUsuarios.BuscarPorCodigo(importacaoPedidoPendente.Usuario.Codigo);
                }
                importacaoPedidoPendente.DataInicioProcessamento = DateTime.Now;
                importacaoPedidoPendente.DataFimProcessamento = null;
                importacaoPedidoPendente.TotalSegundosProcessamento = null;
                importacaoPedidoPendente.Mensagem = null;
                importacaoPedidoPendente.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Processando;
                repImportacaoPedido.Atualizar(importacaoPedidoPendente);
                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        private void IndicarInicioDoProcessamentoNotaFiscal(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.NotaFiscal.ImportacaoNotaFiscal repositorio, Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscal importacaoNotaFiscal)
        {
            unitOfWork.Start();
            try
            {
                importacaoNotaFiscal.DataInicioProcessamento = DateTime.Now;
                importacaoNotaFiscal.DataFimProcessamento = null;
                importacaoNotaFiscal.Mensagem = null;
                importacaoNotaFiscal.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoNotaFiscal.Processando;
                repositorio.Atualizar(importacaoNotaFiscal);
                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        private void IndicarFimDoProcessamento(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Pedidos.ImportacaoPedido repImportacaoPedido, Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido importacaoPedidoPendente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido situacao, string mensagem)
        {
            // Registra o fim do processamento da planilha, de "Processando" para "Sucesso" ou "Erro"
            if (!importacaoPedidoPendente.DataInicioProcessamento.HasValue) importacaoPedidoPendente.DataInicioProcessamento = DateTime.Now;
            importacaoPedidoPendente.DataFimProcessamento = DateTime.Now;
            importacaoPedidoPendente.TotalSegundosProcessamento = (int)(importacaoPedidoPendente.DataFimProcessamento.Value - importacaoPedidoPendente.DataInicioProcessamento.Value).TotalSeconds;
            importacaoPedidoPendente.Situacao = situacao;
            importacaoPedidoPendente.Mensagem = mensagem;
            repImportacaoPedido.Atualizar(importacaoPedidoPendente);
        }

        private void IndicarFimDoProcessamentoNotaFiscal(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.NotaFiscal.ImportacaoNotaFiscal repositorio, Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscal importacaoNotaFiscal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoNotaFiscal situacao, string mensagem)
        {
            if (!importacaoNotaFiscal.DataInicioProcessamento.HasValue) importacaoNotaFiscal.DataInicioProcessamento = DateTime.Now;
            importacaoNotaFiscal.DataFimProcessamento = DateTime.Now;
            importacaoNotaFiscal.Situacao = situacao;
            importacaoNotaFiscal.Mensagem = mensagem;
            repositorio.Atualizar(importacaoNotaFiscal);
        }

        private void LocalizarProcessarImportacaoPedagio(UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frota.ImportacaoPedagio repositorio = new Repositorio.Embarcador.Frota.ImportacaoPedagio(unitOfWork);

            Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio proximaImportacao = repositorio.BuscarProximaImportacaoPendente();

            if (proximaImportacao != null)
            {

                IndicarInicioDoProcessamentoPedagio(unitOfWork, repositorio, proximaImportacao);

                ProcessarImportacaoPedagio(unitOfWork, repositorio, proximaImportacao);

            }
        }

        private void LocalizarProcessarImportacaoCTeAnterior(UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.BuscarPrimeiroRegistro();

            if (integracao?.PossuiIntegracaoDeCadastrosMulti ?? false)
            {
                if (integracao.RealizarIntegracaoDeCTeAnterior)
                    Servicos.Embarcador.Integracao.CadastrosMulti.ImportarCTeAnterior.ConsultarCargaPendenteConsultaCTeAnterior(out _, _tipoServicoMultisoftware, _auditado, unitOfWork, unitOfWorkAdmin.StringConexao);

                if (integracao.RealizarIntegracaoDeCTeParaComplementoOSMae)
                    Servicos.Embarcador.Integracao.CadastrosMulti.ImportarCTeAnterior.ConsultarCTeParaComplementoOSMae(out _, _tipoServicoMultisoftware, _auditado, unitOfWork, unitOfWorkAdmin.StringConexao);
            }
        }

        private void IndicarInicioDoProcessamentoPedagio(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Frota.ImportacaoPedagio repositorio, Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio importacao)
        {
            importacao.DataInicioProcessamento = DateTime.Now;
            importacao.DataFimProcessamento = null;
            importacao.Mensagem = null;
            importacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedagio.Processando;

            repositorio.Atualizar(importacao);
        }

        private void IndicarFimDoProcessamentoPedagio(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Frota.ImportacaoPedagio repositorio, Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio importacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedagio situacao, string mensagem)
        {
            if (!importacao.DataInicioProcessamento.HasValue)
                importacao.DataInicioProcessamento = DateTime.Now;

            importacao.DataFimProcessamento = DateTime.Now;
            importacao.Situacao = situacao;
            importacao.Mensagem = mensagem;

            repositorio.Atualizar(importacao);
        }

        private void LocalizarProcessarImportacaoCTeEmitidoForaEmbarcadorPendente(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador repImportacaoCTeEmitidoForaEmbarcador = new Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador(unitOfWork);

            // Busca a próxima planilha pendente
            Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador importacaoCTeEmitidoForaEmbarcadorPendente = repImportacaoCTeEmitidoForaEmbarcador.BuscarProximaImportacaoPendente();
            if (importacaoCTeEmitidoForaEmbarcadorPendente != null)
            {

                // Registra o início do processamento da planilha, de "Pendente" para "Processando"
                IndicarInicioDoProcessamento(unitOfWork, repImportacaoCTeEmitidoForaEmbarcador, importacaoCTeEmitidoForaEmbarcadorPendente);

                // Processa a planilha importada
                ProcessarImportacaoCTeEmitidoForaEmbarcador(unitOfWork, repImportacaoCTeEmitidoForaEmbarcador, importacaoCTeEmitidoForaEmbarcadorPendente);
            }
        }

        private void IndicarInicioDoProcessamento(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador repImportacaoCTeEmitidoForaEmbarcador, Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador importacaoCTeEmitidoForaEmbarcadorPendente)
        {
            // Registra o início do processamento da planilha, de "Pendente" para "Processando"
            unitOfWork.Start();
            try
            {
                importacaoCTeEmitidoForaEmbarcadorPendente.DataInicioProcessamento = DateTime.Now;
                importacaoCTeEmitidoForaEmbarcadorPendente.DataFimProcessamento = null;
                importacaoCTeEmitidoForaEmbarcadorPendente.TotalSegundosProcessamento = null;
                importacaoCTeEmitidoForaEmbarcadorPendente.Mensagem = null;
                importacaoCTeEmitidoForaEmbarcadorPendente.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador.Processando;
                repImportacaoCTeEmitidoForaEmbarcador.Atualizar(importacaoCTeEmitidoForaEmbarcadorPendente);
                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        public void ProcessarImportacaoCTeEmitidoForaEmbarcador(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador repImportacaoCTeEmitidoForaEmbarcador, Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador importacaoCTeEmitidoForaEmbarcadorPendente)
        {
            try
            {
                Servicos.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador serImportacaoCTeEmitidoForaEmbarcador = new Servicos.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.CTe.RetornoImportacaoCTeEmitidoForaEmbarcador retorno = serImportacaoCTeEmitidoForaEmbarcador.ImportarCTeEmitidoForaEmbarcador(importacaoCTeEmitidoForaEmbarcadorPendente, importacaoCTeEmitidoForaEmbarcadorPendente.Usuario);

                string mensagem;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador situacao;
                if (retorno.Sucesso)
                {
                    mensagem = $"{retorno.TotalCTesEmitidosForaEmbarcador} CT-es importados.";
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador.Sucesso;
                }
                else
                {
                    mensagem = retorno.Mensagem;
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador.Erro;
                }

                IndicarFimDoProcessamento(repImportacaoCTeEmitidoForaEmbarcador, importacaoCTeEmitidoForaEmbarcadorPendente, situacao, mensagem);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                unitOfWork.FlushAndClear();

                unitOfWork.Start();
                IndicarFimDoProcessamento(repImportacaoCTeEmitidoForaEmbarcador, importacaoCTeEmitidoForaEmbarcadorPendente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador.Erro, ex.Message);
                unitOfWork.CommitChanges();

                throw;
            }
        }

        private void IndicarFimDoProcessamento(Repositorio.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador repImportacaoCTeEmitidoForaEmbarcador, Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador importacaoCTeEmitidoForaEmbarcadorPendente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador situacao, string mensagem)
        {
            // Registra o fim do processamento da planilha, de "Processando" para "Sucesso" ou "Erro"
            if (!importacaoCTeEmitidoForaEmbarcadorPendente.DataInicioProcessamento.HasValue) importacaoCTeEmitidoForaEmbarcadorPendente.DataInicioProcessamento = DateTime.Now;
            importacaoCTeEmitidoForaEmbarcadorPendente.DataFimProcessamento = DateTime.Now;
            importacaoCTeEmitidoForaEmbarcadorPendente.TotalSegundosProcessamento = (int)(importacaoCTeEmitidoForaEmbarcadorPendente.DataFimProcessamento.Value - importacaoCTeEmitidoForaEmbarcadorPendente.DataInicioProcessamento.Value).TotalSeconds;
            importacaoCTeEmitidoForaEmbarcadorPendente.Situacao = situacao;
            importacaoCTeEmitidoForaEmbarcadorPendente.Mensagem = mensagem;
            repImportacaoCTeEmitidoForaEmbarcador.Atualizar(importacaoCTeEmitidoForaEmbarcadorPendente);
        }

        #endregion
    }
}
