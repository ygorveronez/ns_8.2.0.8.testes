using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class NotaDeDebito : LongRunningProcessBase<NotaDeDebito>
    {
        #region Métodos protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            LocalizarProcessarImportacaoNotaDebitoPendente(unitOfWork);
            LocalizarProcessarImportacaoMontagemFeederPendente(unitOfWork);
            LocalizarProcessarImportacaRolagemContainerPendente(unitOfWork);
            LocalizarProcessarImportacaAssociacaoBalsaPendente(unitOfWork);
        }

        #endregion

        #region Métodos privados

        private void LocalizarProcessarImportacaoNotaDebitoPendente(Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Pedidos.NotaDeDebito.NotaDeDebito repNotaDeDebito = new Repositorio.Embarcador.Pedidos.NotaDeDebito.NotaDeDebito(unitOfWork);

            // Busca a próximoa planilha pendente
            Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebito importacaoPedidoPendente = repNotaDeDebito.BuscarProximaImportacaoPendente();
            if (importacaoPedidoPendente != null)
            {

                // Registra o início do processamento da planilha, de "Pendente" para "Processando"
                IndicarInicioDoProcessamento(unitOfWork, repNotaDeDebito, importacaoPedidoPendente);

                // Processa a planilha importada
                ProcessarImportacaoNotaDebito(unitOfWork, repNotaDeDebito, importacaoPedidoPendente);

            }
        }

        private void LocalizarProcessarImportacaRolagemContainerPendente(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer repRolagemContainer = new Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacaoPedidoPendente = repRolagemContainer.BuscarProximaImportacaoPendente();
            if (importacaoPedidoPendente != null)
            {
                IndicarInicioDoProcessamento(unitOfWork, repRolagemContainer, importacaoPedidoPendente);
                ProcessarImportacaoRolagemContainer(unitOfWork, repRolagemContainer, importacaoPedidoPendente);
            }
        }

        private void LocalizarProcessarImportacaAssociacaoBalsaPendente(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.AssociacaoBalsa repositorioAssociacaoBalsa = new Repositorio.Embarcador.Logistica.AssociacaoBalsa(unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa importacaoBalsaPendente = repositorioAssociacaoBalsa.BuscarProximaImportacaoPendente();
            if (importacaoBalsaPendente != null)
            {
                IndicarInicioDoProcessamentoImportacao(unitOfWork, repositorioAssociacaoBalsa, importacaoBalsaPendente);
                ProcessarImportacaoAssociacaoBalsa(unitOfWork, repositorioAssociacaoBalsa, importacaoBalsaPendente);
            }
        }

        public void ProcessarImportacaoNotaDebito(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Pedidos.NotaDeDebito.NotaDeDebito repNotaDeDebito, Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebito importacaoPedidoPendente)
        {
            try
            {
                // Importa a planilha gerando pedidos e cargas
                Servicos.Embarcador.Pedido.NotaDeDebito serPedido = new Servicos.Embarcador.Pedido.NotaDeDebito();

                Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoImportacaoPedidoGerarCarga retorno = serPedido.ImportarPedidoGerarCargas(importacaoPedidoPendente, importacaoPedidoPendente.Usuario, _tipoServicoMultisoftware, _stringConexaoAdmin, unitOfWork);

                string mensagem;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido situacao;
                if (retorno.Sucesso)
                {
                    mensagem = $"{retorno.TotalPedidos} pedidos importados e {retorno.TotalCargas} cargas geradas";
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Sucesso;
                }
                else
                {
                    mensagem = retorno.Mensagem;
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro;
                }

                // Registra o fim do processamento da planilha com erro ou sucesso
                IndicarFimDoProcessamento(unitOfWork, repNotaDeDebito, importacaoPedidoPendente, situacao, mensagem);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                unitOfWork.FlushAndClear();

                unitOfWork.Start();
                IndicarFimDoProcessamento(unitOfWork, repNotaDeDebito, importacaoPedidoPendente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro, ex.Message);
                unitOfWork.CommitChanges();

                throw;
            }
        }

        private void IndicarInicioDoProcessamento(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Pedidos.NotaDeDebito.NotaDeDebito repNotaDeDebito, Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebito importacaoPedidoPendente)
        {
            // Registra o início do processamento da planilha, de "Pendente" para "Processando"
            unitOfWork.Start();
            try
            {
                importacaoPedidoPendente.DataInicioProcessamento = DateTime.Now;
                importacaoPedidoPendente.DataFimProcessamento = null;
                importacaoPedidoPendente.TotalSegundosProcessamento = null;
                importacaoPedidoPendente.Mensagem = null;
                importacaoPedidoPendente.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Processando;
                repNotaDeDebito.Atualizar(importacaoPedidoPendente);
                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        private void IndicarFimDoProcessamento(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Pedidos.NotaDeDebito.NotaDeDebito repNotaDeDebito, Dominio.Entidades.Embarcador.Pedidos.NotaDeDebito.NotaDeDebito importacaoPedidoPendente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido situacao, string mensagem)
        {
            // Registra o fim do processamento da planilha, de "Processando" para "Sucesso" ou "Erro"
            if (!importacaoPedidoPendente.DataInicioProcessamento.HasValue) importacaoPedidoPendente.DataInicioProcessamento = DateTime.Now;
            importacaoPedidoPendente.DataFimProcessamento = DateTime.Now;
            importacaoPedidoPendente.TotalSegundosProcessamento = (int)(importacaoPedidoPendente.DataFimProcessamento.Value - importacaoPedidoPendente.DataInicioProcessamento.Value).TotalSeconds;
            importacaoPedidoPendente.Situacao = situacao;
            importacaoPedidoPendente.Mensagem = mensagem;
            repNotaDeDebito.Atualizar(importacaoPedidoPendente);
        }

        #endregion

        #region Métodos MontagemFeeder

        private void LocalizarProcessarImportacaoMontagemFeederPendente(Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder repMontagemFeeder = new Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder(unitOfWork);

            // Busca a próximoa planilha pendente
            Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder importacaoPedidoPendente = repMontagemFeeder.BuscarProximaImportacaoPendente();
            if (importacaoPedidoPendente != null)
            {

                // Registra o início do processamento da planilha, de "Pendente" para "Processando"
                IndicarInicioDoProcessamento(unitOfWork, repMontagemFeeder, importacaoPedidoPendente);

                // Processa a planilha importada
                ProcessarImportacaoMontagemFeeder(unitOfWork, repMontagemFeeder, importacaoPedidoPendente);

            }
        }

        public void ProcessarImportacaoMontagemFeeder(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder repMontagemFeeder, Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder importacaoPedidoPendente)
        {
            try
            {
                // Importa a planilha gerando pedidos e cargas
                Servicos.Embarcador.Carga.MontagemFeeder.MontagemFeeder serMontagemFeeder = new Servicos.Embarcador.Carga.MontagemFeeder.MontagemFeeder();
                Servicos.Embarcador.Notificacao.Notificacao serNotificaocao = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, null, _tipoServicoMultisoftware, "");
                Dominio.ObjetosDeValor.Embarcador.Carga.MontagemFeeder.RetornoImportacaoMontagemFeeder retorno = new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemFeeder.RetornoImportacaoMontagemFeeder();

                try
                {
                    retorno = serMontagemFeeder.ImportarMontagemFeeder(importacaoPedidoPendente, importacaoPedidoPendente.Usuario, _tipoServicoMultisoftware, _stringConexaoAdmin, unitOfWork, _urlAcesso);
                }
                catch (Exception ex)
                {
                    retorno.Sucesso = false;
                    if (string.IsNullOrWhiteSpace(retorno.Mensagem))
                        retorno.Mensagem = "Falha na importação do Feeder, favor tente novamente.";
                    Servicos.Log.TratarErro(ex);
                }

                string mensagem;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido situacao;
                if (retorno.Sucesso)
                {
                    mensagem = $"Montagem de cargas Feeder realizado com sucesso";
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Sucesso;
                    serNotificaocao.GerarNotificacao(importacaoPedidoPendente.Usuario, importacaoPedidoPendente.Codigo, "Pedidos/MontagemFeeder", "Montagem de Feeder importado com sucesso. Arquivo: " + importacaoPedidoPendente.Planilha, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.sucesso, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.alerta, _tipoServicoMultisoftware, unitOfWork);
                }
                else
                {
                    mensagem = retorno.Mensagem;
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro;
                    serNotificaocao.GerarNotificacao(importacaoPedidoPendente.Usuario, importacaoPedidoPendente.Codigo, "Pedidos/MontagemFeeder", "Atenção! Alguns registros não foram importados. Arquivo: " + importacaoPedidoPendente.Planilha, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.falha, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.alerta, _tipoServicoMultisoftware, unitOfWork);
                }

                // Registra o fim do processamento da planilha com erro ou sucesso
                IndicarFimDoProcessamento(true, unitOfWork, repMontagemFeeder, importacaoPedidoPendente, situacao, mensagem);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();

                unitOfWork.Start();
                IndicarFimDoProcessamento(false, unitOfWork, repMontagemFeeder, importacaoPedidoPendente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro, ex.Message);
                unitOfWork.CommitChanges();

                throw;
            }
        }

        private void IndicarInicioDoProcessamento(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder repMontagemFeeder, Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder importacaoPedidoPendente)
        {
            // Registra o início do processamento da planilha, de "Pendente" para "Processando"
            unitOfWork.Start();
            try
            {
                importacaoPedidoPendente.DataInicioProcessamento = DateTime.Now;
                importacaoPedidoPendente.DataFimProcessamento = null;
                importacaoPedidoPendente.TotalSegundosProcessamento = null;
                importacaoPedidoPendente.Mensagem = null;
                importacaoPedidoPendente.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Processando;
                repMontagemFeeder.Atualizar(importacaoPedidoPendente);
                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                throw;
            }
        }

        private void IndicarFimDoProcessamento(bool realizarCommit, Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder repMontagemFeeder, Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder importacaoPedidoPendente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido situacao, string mensagem)
        {
            if (realizarCommit)
                unitOfWork.Start();

            try
            {
                importacaoPedidoPendente = repMontagemFeeder.BuscarPorCodigo(importacaoPedidoPendente.Codigo);

                if (!importacaoPedidoPendente.DataInicioProcessamento.HasValue) importacaoPedidoPendente.DataInicioProcessamento = DateTime.Now;
                importacaoPedidoPendente.DataFimProcessamento = DateTime.Now;
                importacaoPedidoPendente.TotalSegundosProcessamento = (int)(importacaoPedidoPendente.DataFimProcessamento.Value - importacaoPedidoPendente.DataInicioProcessamento.Value).TotalSeconds;
                importacaoPedidoPendente.Situacao = situacao;
                importacaoPedidoPendente.Mensagem = mensagem;
                repMontagemFeeder.Atualizar(importacaoPedidoPendente);

                Servicos.Embarcador.Hubs.MontagemFeeder hubMontagemFeeder = new Servicos.Embarcador.Hubs.MontagemFeeder();

                hubMontagemFeeder.NotificarMontagemAtualizado(importacaoPedidoPendente);

                if (realizarCommit)
                    unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                if (realizarCommit)
                    unitOfWork.Rollback();
                throw;
            }

        }
        #endregion

        #region Métodos RolagemContainer

        private void IndicarInicioDoProcessamento(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer repRolagemContainer, Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacaoPendente)
        {
            unitOfWork.Start();

            try
            {
                importacaoPendente.DataInicioProcessamento = DateTime.Now;
                importacaoPendente.DataFimProcessamento = null;
                importacaoPendente.TotalSegundosProcessamento = null;
                importacaoPendente.Mensagem = null;
                importacaoPendente.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Processando;
                repRolagemContainer.Atualizar(importacaoPendente);
                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        private void IndicarFimDoProcessamento(Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer repRolagemContainer, Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacaoPendente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido situacao, string mensagem)
        {
            if (!importacaoPendente.DataInicioProcessamento.HasValue) importacaoPendente.DataInicioProcessamento = DateTime.Now;
            importacaoPendente.DataFimProcessamento = DateTime.Now;
            importacaoPendente.TotalSegundosProcessamento = (int)(importacaoPendente.DataFimProcessamento.Value - importacaoPendente.DataInicioProcessamento.Value).TotalSeconds;
            importacaoPendente.Situacao = situacao;
            importacaoPendente.Mensagem = string.Join(". ", importacaoPendente.Mensagem?.Split('.').Select(s => s.Trim('\r', '\n').Trim()).Distinct());
            repRolagemContainer.Atualizar(importacaoPendente);
        }

        public void ProcessarImportacaoRolagemContainer(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Cargas.RolagemContainer.RolagemContainer repRolagemContainer, Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacaoPendente)
        {
            try
            {
                Servicos.Embarcador.Pedido.RolagemContainer serRolagemContainer = new Servicos.Embarcador.Pedido.RolagemContainer(unitOfWork, _auditado, _tipoServicoMultisoftware);

                int contadorImportacoes = 0;
                StringBuilder sb = new StringBuilder();

                foreach (Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer importacao in importacaoPendente.RolagemContainers)
                {
                    if (importacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro)
                    {
                        sb.AppendLine(importacao.Mensagem);
                        continue;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoImportacaoPedidoGerarCarga retorno = serRolagemContainer.ProcessarRolagemContainer(importacao);

                    string mensagem;
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido situacao;

                    if (retorno.Sucesso)
                    {
                        mensagem = $"Carga rolada com sucesso";
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Sucesso;
                        contadorImportacoes++;

                        serRolagemContainer.CancelarCargaRolada(importacao.CargaPedidoAntigo?.Carga, unitOfWork);
                    }
                    else
                    {
                        mensagem = retorno.Mensagem;
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro;
                        sb.AppendLine(retorno.Mensagem);
                    }

                    IndicarFimDoProcessamento(repRolagemContainer, importacao, situacao, mensagem);
                }

                IndicarFimDoProcessamento(repRolagemContainer,
                    importacaoPendente,
                    contadorImportacoes > 0 ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Sucesso : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro,
                    contadorImportacoes > 0 ? $"{contadorImportacoes} pedido(s) importado(s)" : sb.ToString());
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                unitOfWork.FlushAndClear();

                unitOfWork.Start();
                IndicarFimDoProcessamento(repRolagemContainer, importacaoPendente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro, ex.Message);
                unitOfWork.CommitChanges();

                throw;
            }
        }
        #endregion

        #region Métodos AssociacaoBalsa
        private void IndicarInicioDoProcessamentoImportacao(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Logistica.AssociacaoBalsa repositorioAssociacaoBalsa, Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa importacaoPendente)
        {
            unitOfWork.Start();

            try
            {
                importacaoPendente.DataInicioProcessamento = DateTime.Now;
                importacaoPendente.DataFimProcessamento = null;
                importacaoPendente.TotalSegundosProcessamento = null;
                importacaoPendente.Mensagem = null;
                importacaoPendente.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Processando;
                repositorioAssociacaoBalsa.Atualizar(importacaoPendente);
                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        public void ProcessarImportacaoAssociacaoBalsa(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Logistica.AssociacaoBalsa repositorioAssociacaoBalsa, Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa importacaoPendente)
        {
            try
            {
                Servicos.Embarcador.Logistica.AssociacaoBalsa servicoAssociacaoBalsa = new Servicos.Embarcador.Logistica.AssociacaoBalsa(unitOfWork, _auditado, _tipoServicoMultisoftware);

                int contadorImportacoes = 0;
                StringBuilder sb = new StringBuilder();

                foreach (Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa importacao in importacaoPendente.AssociacaoBalsas)
                {
                    Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoImportacaoPedidoGerarCarga retorno = servicoAssociacaoBalsa.ProcessarAssociacaoBalsa(importacao);

                    string mensagem;
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido situacao;

                    if (retorno.Sucesso)
                    {
                        mensagem = $"Importações realizadas com sucesso.";
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Sucesso;
                        contadorImportacoes++;
                    }
                    else
                    {
                        mensagem = retorno.Mensagem;
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro;
                        sb.AppendLine(retorno.Mensagem);
                    }

                    IndicarFimDoProcessamentoImportacao(repositorioAssociacaoBalsa, importacao, situacao, mensagem);
                }

                IndicarFimDoProcessamentoImportacao(repositorioAssociacaoBalsa,
                    importacaoPendente,
                    contadorImportacoes > 0 ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Sucesso : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro,
                    contadorImportacoes > 0 ? $"{contadorImportacoes} importado(s)" : sb.ToString());
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                unitOfWork.FlushAndClear();

                unitOfWork.Start();
                IndicarFimDoProcessamentoImportacao(repositorioAssociacaoBalsa, importacaoPendente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro, ex.Message);
                unitOfWork.CommitChanges();

                throw;
            }
        }

        private void IndicarFimDoProcessamentoImportacao(Repositorio.Embarcador.Logistica.AssociacaoBalsa repositorioAssociacaoBalsa, Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa importacaoPendente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido situacao, string mensagem)
        {
            var situacaoErro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Erro;

            if (!importacaoPendente.DataInicioProcessamento.HasValue) importacaoPendente.DataInicioProcessamento = DateTime.Now;
            importacaoPendente.DataFimProcessamento = DateTime.Now;
            importacaoPendente.TotalSegundosProcessamento = (int)(importacaoPendente.DataFimProcessamento.Value - importacaoPendente.DataInicioProcessamento.Value).TotalSeconds;

            if (importacaoPendente.Situacao == situacaoErro || situacao == situacaoErro)
                importacaoPendente.Mensagem += mensagem;
            else
                importacaoPendente.Mensagem = mensagem;

            importacaoPendente.Mensagem = string.Join(". ", importacaoPendente.Mensagem?.Split('.').Select(s => s.Trim('\r', '\n').Trim()).Distinct()).Trim();
            importacaoPendente.Situacao = situacao;
            repositorioAssociacaoBalsa.Atualizar(importacaoPendente);
        }
        #endregion
    }
}
