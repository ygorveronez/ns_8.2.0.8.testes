using Dominio.Interfaces.Database;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Servicos.Embarcador.Credito
{
    public class CreditoMovimentacao : ServicoBase
    {
        public CreditoMovimentacao(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        public void ConfirmarUtilizacaoCreditos(List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> creditosUtilizadosDestino, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Creditos.CreditoDisponivel repCreditoDisponivel = new Repositorio.Embarcador.Creditos.CreditoDisponivel(unitOfWork);
            Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado repCreditoDisponivelUtilizado = new Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado(unitOfWork);
            List<Dominio.Entidades.Usuario> usuarios = new List<Dominio.Entidades.Usuario>();

            foreach (Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado creditoDisponivelUtilizado in creditosUtilizadosDestino)
            {
                Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel creditoUtilizado = repCreditoDisponivel.BuscarPorCodigo(creditoDisponivelUtilizado.CreditoDisponivelOrigem.Codigo);
                creditoUtilizado.ValorComprometido -= creditoDisponivelUtilizado.ValorComprometido;
                repCreditoDisponivel.Atualizar(creditoUtilizado);
                if (!usuarios.Contains(creditoUtilizado.Recebedor))
                    usuarios.Add(creditoUtilizado.Recebedor);

                creditoDisponivelUtilizado.ValorUtilizado = creditoDisponivelUtilizado.ValorComprometido;
                creditoDisponivelUtilizado.SituacaoCreditoUtilizado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoUtilizado.Utilizado;
                repCreditoDisponivelUtilizado.Atualizar(creditoDisponivelUtilizado);
            }

            informarAlteracaoCreditoViaSignalR(usuarios, unitOfWork);
        }

        public void ExtornarCreditos(List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> creditosUtilizadosDestino, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {

            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Notificacao.Notificacao(StringConexao, null, tipoServicoMultisoftware, "");

            Repositorio.Embarcador.Creditos.CreditoDisponivel repCreditoDisponivel = new Repositorio.Embarcador.Creditos.CreditoDisponivel(unitOfWork);
            Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado repCreditoDisponivelUtilizado = new Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

            List<Dominio.Entidades.Usuario> usuarios = new List<Dominio.Entidades.Usuario>();
            foreach (Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado creditoDisponivelUtilizado in creditosUtilizadosDestino)
            {
                Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel creditoUtilizado = repCreditoDisponivel.BuscarPorCodigo(creditoDisponivelUtilizado.CreditoDisponivelOrigem.Codigo);

                if (!usuarios.Contains(creditoUtilizado.Recebedor))
                    usuarios.Add(creditoUtilizado.Recebedor);

                decimal valorUtilizado = creditoDisponivelUtilizado.ValorUtilizado;
                if (creditoDisponivelUtilizado.SituacaoCreditoUtilizado == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoUtilizado.Comprometido)
                {
                    valorUtilizado = creditoDisponivelUtilizado.ValorComprometido;
                    creditoUtilizado.ValorComprometido -= creditoDisponivelUtilizado.ValorComprometido;
                }
                creditoUtilizado.ValorSaldo += valorUtilizado;

                if (creditoDisponivelUtilizado.CreditoDisponivelOrigem.DataFimCredito >= DateTime.Now)
                {
                    if (creditoDisponivelUtilizado.CargaOcorrencia != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = creditoDisponivelUtilizado.CargaOcorrencia.Carga != null ? repCargaCancelamento.BuscarPorCarga(creditoDisponivelUtilizado.CargaOcorrencia.Carga.Codigo) : null;
                        if (cargaCancelamento != null && (cargaCancelamento.Usuario == null || cargaCancelamento.Usuario.Codigo != creditoUtilizado.Recebedor.Codigo))
                        {
                            string nota = string.Format(Localization.Resources.Credito.CreditoMovimentacao.CreditoUtilizadoOcorrenciaCargaEstornado, valorUtilizado.ToString("n2"), creditoDisponivelUtilizado.CargaOcorrencia.Carga.CodigoCargaEmbarcador);
                            serNotificacao.GerarNotificacao(creditoUtilizado.Recebedor, cargaCancelamento.Usuario, creditoDisponivelUtilizado.CargaOcorrencia.Codigo, "Ocorrencias/Ocorrencia", nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.estornado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SmartAdminBgColor.blue, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
                        }
                    }
                    if (creditoDisponivelUtilizado.CargaComplementoFrete != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCarga(creditoDisponivelUtilizado.CargaComplementoFrete.Carga.Codigo);
                        if (cargaCancelamento != null && (cargaCancelamento.Usuario == null || cargaCancelamento.Usuario.Codigo != creditoUtilizado.Recebedor.Codigo))
                        {
                            string nota = string.Format(Localization.Resources.Credito.CreditoMovimentacao.CreditoUtilizadoComplementoFreteCargaEstornado, valorUtilizado.ToString("n2"), creditoDisponivelUtilizado.CargaComplementoFrete.Carga.CodigoCargaEmbarcador);
                            serNotificacao.GerarNotificacao(creditoUtilizado.Recebedor, cargaCancelamento.Usuario, creditoDisponivelUtilizado.CargaComplementoFrete.Carga.Codigo, "Cargas/Carga", nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.estornado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SmartAdminBgColor.blue, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
                        }
                    }
                }

                repCreditoDisponivel.Atualizar(creditoUtilizado);
                creditoDisponivelUtilizado.SituacaoCreditoUtilizado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoUtilizado.Estornado;
                repCreditoDisponivelUtilizado.Atualizar(creditoDisponivelUtilizado);
            }

            informarAlteracaoCreditoViaSignalR(usuarios, unitOfWork);

        }

        #region Utilizar

        public string UtilizarCreditos(List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado> creditosUtilizados, Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel creditoDestino, Repositorio.UnitOfWork unitOfWork)
        {
            return validarUtilizacaoCredito(creditosUtilizados, creditoDestino, null, null, null, null, false, unitOfWork);
        }

        public string UtilizarCreditos(List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado> creditosUtilizados, Dominio.Entidades.Embarcador.Creditos.CreditoExtra creditoExtra, Repositorio.UnitOfWork unitOfWork)
        {
            return validarUtilizacaoCredito(creditosUtilizados, null, null, null, null, creditoExtra, false, unitOfWork);
        }


        public string UtilizarCreditos(List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado> creditosUtilizados, Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete, Repositorio.UnitOfWork unitOfWork)
        {
            return validarUtilizacaoCredito(creditosUtilizados, null, null, cargaComplementoFrete, null, null, false, unitOfWork);
        }

        public string UtilizarCreditos(List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado> creditosUtilizados, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            return validarUtilizacaoCredito(creditosUtilizados, null, cargaOcorrencia, null, null, null, false, unitOfWork);
        }

        public string UtilizarCreditos(List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado> creditosUtilizados, Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito solicitacaoCredito, Repositorio.UnitOfWork unitOfWork)
        {
            return validarUtilizacaoCredito(creditosUtilizados, null, null, null, solicitacaoCredito, null, false, unitOfWork);
        }

        #endregion

        #region Comprometer

        public string ComprometerCreditos(List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado> creditosUtilizados, Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel creditoDestino, Repositorio.UnitOfWork unitOfWork)
        {
            return validarUtilizacaoCredito(creditosUtilizados, creditoDestino, null, null, null, null, true, unitOfWork);
        }

        public string ComprometerCreditos(List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado> creditosUtilizados, Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete, Repositorio.UnitOfWork unitOfWork)
        {
            return validarUtilizacaoCredito(creditosUtilizados, null, null, cargaComplementoFrete, null, null, true, unitOfWork);
        }

        public string ComprometerCreditos(List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado> creditosUtilizados, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            return validarUtilizacaoCredito(creditosUtilizados, null, cargaOcorrencia, null, null, null, true, unitOfWork);
        }

        public string ComprometerCreditos(List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado> creditosUtilizados, Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito solicitacaoCredito, Repositorio.UnitOfWork unitOfWork)
        {
            return validarUtilizacaoCredito(creditosUtilizados, null, null, null, solicitacaoCredito, null, true, unitOfWork);
        }

        public string ComprometerCreditos(List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado> creditosUtilizados, Dominio.Entidades.Embarcador.Creditos.CreditoExtra creditoExtra, Repositorio.UnitOfWork unitOfWork)
        {
            return validarUtilizacaoCredito(creditosUtilizados, null, null, null, null, creditoExtra, true, unitOfWork);
        }

        #endregion

        private string validarUtilizacaoCredito(List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado> creditosUtilizados, Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel creditoDestino, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete, Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito solicitacaoCredito, Dominio.Entidades.Embarcador.Creditos.CreditoExtra creditoExtra, bool apenasComprometer, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = "";
            Repositorio.Embarcador.Creditos.CreditoDisponivel repCreditoDisponivel = new Repositorio.Embarcador.Creditos.CreditoDisponivel(unitOfWork);
            Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito repHierarquiaSolicitacaoCredito = new Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito(unitOfWork);
            Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado repCreditoDisponivelUtilizado = new Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado(unitOfWork);

            if (creditosUtilizados != null)
            {
                List<Dominio.Entidades.Usuario> usuarios = new List<Dominio.Entidades.Usuario>();
                foreach (Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado creditoUtilizado in creditosUtilizados)
                {

                    Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel creditoDisponivelUt = repCreditoDisponivel.BuscarPorCodigo(creditoUtilizado.Codigo);

                    if (creditoDisponivelUt != null)
                    {
                        if ((creditoDisponivelUt.DataInicioCredito == DateTime.MinValue || creditoDisponivelUt.DataInicioCredito <= DateTime.Now) && (creditoDisponivelUt.DataFimCredito == DateTime.MinValue || creditoDisponivelUt.DataFimCredito >= DateTime.Now))
                        {

                            //todo:validar data limite para o credito de destino

                            if (creditoDisponivelUt.ValorSaldo >= creditoUtilizado.ValorUtilizado)
                            {
                                if (apenasComprometer)
                                    creditoDisponivelUt.ValorComprometido += creditoUtilizado.ValorUtilizado;

                                creditoDisponivelUt.ValorSaldo -= creditoUtilizado.ValorUtilizado;
                                repCreditoDisponivel.Atualizar(creditoDisponivelUt);

                                Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado creditoDisponivelUtilizado = new Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado();
                                creditoDisponivelUtilizado.CreditoDisponivelOrigem = creditoDisponivelUt;
                                creditoDisponivelUtilizado.DataUtilizacao = DateTime.Now;
                                creditoDisponivelUtilizado.CreditoDisponivelDestino = creditoDestino;
                                creditoDisponivelUtilizado.CargaOcorrencia = cargaOcorrencia;
                                creditoDisponivelUtilizado.CreditoExtra = creditoExtra;
                                creditoDisponivelUtilizado.CargaComplementoFrete = cargaComplementoFrete;
                                creditoDisponivelUtilizado.SolicitacaoCredito = solicitacaoCredito;

                                if (apenasComprometer)
                                {
                                    creditoDisponivelUtilizado.SituacaoCreditoUtilizado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoUtilizado.Comprometido;
                                    creditoDisponivelUtilizado.ValorComprometido = creditoUtilizado.ValorUtilizado;
                                }
                                else
                                {
                                    creditoDisponivelUtilizado.SituacaoCreditoUtilizado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoUtilizado.Utilizado;
                                    creditoDisponivelUtilizado.ValorUtilizado = creditoUtilizado.ValorUtilizado;
                                }


                                repCreditoDisponivelUtilizado.Inserir(creditoDisponivelUtilizado);

                                if (!usuarios.Contains(creditoDisponivelUt.Recebedor))
                                    usuarios.Add(creditoDisponivelUt.Recebedor);
                            }
                            else
                            {
                                retorno = "O crédito utilizado (" + creditoUtilizado.ValorUtilizado.ToString("n2") + ") é superior ao saldo disponivel (" + creditoDisponivelUt.ValorSaldo.ToString("n2") + ")  pelo o creditor (" + creditoDisponivelUt.Creditor.Nome + ").";
                            }
                        }
                        else
                        {
                            retorno = "O crédito que você está tentando utilizar já expirou. (Creditor: " + creditoDisponivelUt.Creditor.Nome + ")";
                        }
                    }
                    else
                    {
                        retorno = "O crédito informado é inválido";
                    }
                }

                informarAlteracaoCreditoViaSignalR(usuarios, unitOfWork);
            }

            return retorno;
        }


        private void informarAlteracaoCreditoViaSignalR(List<Dominio.Entidades.Usuario> usuarioSolicitarAtualizacaoCredito, Repositorio.UnitOfWork unitOfWork)
        {

            foreach (Dominio.Entidades.Usuario usuario in usuarioSolicitarAtualizacaoCredito)
            {
                Servicos.Embarcador.Hubs.ControleSaldo hubControleSaldo = new Hubs.ControleSaldo();
                hubControleSaldo.SolicitarAtualizacaoSaldo(usuario, unitOfWork);
            }
        }


        #region Obter

        public void VerificarSeOperadorObtveCreditoNaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador && carga.Operador != null)
            {
                Repositorio.Embarcador.Creditos.CreditoDisponivel repCreditoDisponivel = new Repositorio.Embarcador.Creditos.CreditoDisponivel(unitOfWork);
                Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito repHierarquiaSolicitacaoCredito = new Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito(unitOfWork);
                Repositorio.Embarcador.Creditos.CreditoDisponivelObtido repCreditoDisponivelObtido = new Repositorio.Embarcador.Creditos.CreditoDisponivelObtido(unitOfWork);

                List<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito> hierarquiasCredito = repHierarquiaSolicitacaoCredito.BuscarPorRecebedor(carga.Operador.Codigo);
                foreach (Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito hierarquiaSolicitacaoCredito in hierarquiasCredito)
                {
                    Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel creditoDisponivel = repCreditoDisponivel.BuscarRecebedorCredito(hierarquiaSolicitacaoCredito.Creditor.Codigo, hierarquiaSolicitacaoCredito.Solicitante.Codigo);
                    if (creditoDisponivel != null)
                    {
                        decimal economia = carga.ValorFreteTabelaFrete - carga.ValorFreteAPagar;
                        if (economia > 0)
                        {
                            Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelObtido creditoDisponivelObtido = new Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelObtido();
                            creditoDisponivelObtido.Carga = carga;
                            creditoDisponivelObtido.DataObtencao = DateTime.Now;
                            creditoDisponivelObtido.CreditoDisponivel = creditoDisponivel;
                            creditoDisponivelObtido.SituacaoCreditoObtido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoObtido.Obtido;
                            creditoDisponivelObtido.ValorObtido = economia;
                            repCreditoDisponivelObtido.Inserir(creditoDisponivelObtido);

                            //creditoDisponivel.ValorSaldo += economia;
                            creditoDisponivel.ValorObtido += economia;
                            repCreditoDisponivel.Atualizar(creditoDisponivel);
                        }
                        break;
                    }
                }

            }
        }

        public void ExtornarCreditoObtidoNaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Creditos.CreditoDisponivelObtido repCreditoDisponivelObtido = new Repositorio.Embarcador.Creditos.CreditoDisponivelObtido(unitOfWork);
            Repositorio.Embarcador.Creditos.CreditoDisponivel repCreditoDisponivel = new Repositorio.Embarcador.Creditos.CreditoDisponivel(unitOfWork);

            List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelObtido> creditosDisponivelObtido = repCreditoDisponivelObtido.BuscarPorCarga(carga.Codigo);
            foreach (Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelObtido creditoDisponivelObtido in creditosDisponivelObtido)
            {
                if (creditoDisponivelObtido.SituacaoCreditoObtido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoObtido.Obtido)
                {
                    creditoDisponivelObtido.SituacaoCreditoObtido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoObtido.Estornado;
                    repCreditoDisponivelObtido.Atualizar(creditoDisponivelObtido);

                    //creditoDisponivelObtido.CreditoDisponivel.ValorSaldo -= creditoDisponivelObtido.ValorObtido;
                    creditoDisponivelObtido.CreditoDisponivel.ValorObtido -= creditoDisponivelObtido.ValorObtido;
                    repCreditoDisponivel.Atualizar(creditoDisponivelObtido.CreditoDisponivel);
                }
            }
        }

        #endregion


    }
}
