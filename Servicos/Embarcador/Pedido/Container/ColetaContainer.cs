using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Container;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Pedido
{
    public sealed class ColetaContainer
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public ColetaContainer(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private void CriarHistoricoAtualizacaoContainer(Dominio.Entidades.Embarcador.Pedidos.ColetaContainer ColetaContainer, Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro parametros)
        {
            Repositorio.Embarcador.Pedidos.ColetaContainerHistorico repColetaContainerhistorico = new Repositorio.Embarcador.Pedidos.ColetaContainerHistorico(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico coletaHistoricoAnterior = repColetaContainerhistorico.BuscarUltimoHistoricoPorColetaContainer(ColetaContainer.Codigo);
            if (coletaHistoricoAnterior != null)
            {
                coletaHistoricoAnterior.DataFimHistorico = DateTime.Now;
                repColetaContainerhistorico.Atualizar(coletaHistoricoAnterior);
            }

            Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico coletaContainerHistorico = new Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico
            {
                ColetaContainer = ColetaContainer,
                DataHistorico = DateTime.Now,
                Local = ColetaContainer.LocalAtual ?? null,
                Carga = ColetaContainer.CargaAtual ?? ColetaContainer.CargaDeColeta,
                Status = ColetaContainer.Status,
                Usuario = parametros.Usuario,
                CodigoCargaEmbarcador = ColetaContainer.CargaAtual?.CodigoCargaEmbarcador ?? ColetaContainer.CargaDeColeta.CodigoCargaEmbarcador,
                OrigemMovimentacao = parametros.OrigemMonimentacaoContainer,
                InformacaoOrigemMovimentacao = parametros.InformacaoOrigemMonimentacaoContainer
            };

            repColetaContainerhistorico.Inserir(coletaContainerHistorico);
        }

        private void EnviarEmailNotificacaoDiariaContainer(dynamic coletaExcedida, string emailFilial)
        {
            if (string.IsNullOrEmpty(emailFilial))
                return;

            string assunto = $@"Diárias de container Excedidas";
            string mensagem = $@"
                        <p style=""margin: 0px; padding: 10px 0 10px 0; font - weight: bold; line - height: 150 %; ""> Relação de containers em diaria excedidas de acordo com FreeTime</p>                       
                        <table style=""border: 1px solid #C0C0C0; border-collapse: collapse;padding: 5px;""><thead>
                        <tr style=""font-family: Arial, Helvetica, sans-serif; font-size: 11px;"">
                        <th style=""width: 10%; border: 1px solid #C0C0C0; padding: 5px; background: #F0F0F0;"">Nº Container
                        <th style=""width: 10%; border: 1px solid #C0C0C0; padding: 5px; background: #F0F0F0;"">Data Atualização
                        </th><th style=""width: 15%; border: 1px solid #C0C0C0; padding: 5px; background: #F0F0F0;"">Situacao
                        </th><th style=""width: 8%; border: 1px solid #C0C0C0; padding: 5px; background: #F0F0F0;"">Data Coleta
                        </th><th style=""width: 15%; border: 1px solid #C0C0C0; padding: 5px; background: #F0F0F0;"">Local Coleta
                        </th><th style="" width: 15%; border: 1px solid #C0C0C0; padding: 5px; background: #F0F0F0;"">Local Atual
                        </th><th style=""width: 5%; border: 1px solid #C0C0C0; padding: 5px; background: #F0F0F0;"">Nº Dias
                        </th><th style="" width: 10%; border: 1px solid #C0C0C0; padding: 5px; background: #F0F0F0;"">Início Diária
                        </th><th style=""width: 10%; border: 1px solid #C0C0C0; padding: 5px; background: #F0F0F0;"">Valor Diária
                        </th><th style=""width: 10%; border: 1px solid #C0C0C0; padding: 5px; background: #F0F0F0;"">Total 
                        </th></tr></thead><tbody>";

            foreach (var coleta in coletaExcedida.Coletas)
            {
                mensagem += $@"<tr style=""font-family: Arial, Helvetica, sans-serif; font-size: 11px"">";
                mensagem += $@"<td style=""border: 1px solid #C0C0C0; padding: 5px; text-align:center"">{ coleta.NumeroContainer } </td><td style=""border: 1px solid #C0C0C0; padding: 5px; text-align:center"">{ coleta.DataUltimaMovimentacao } </td><td style="" border: 1px solid #C0C0C0; padding: 5px; text-align:center"">{ coleta.Status } </td><td style="" border: 1px solid #C0C0C0; padding: 5px; text-align:center"">{ coleta.DataColeta } </td><td style="" border: 1px solid #C0C0C0; padding: 5px;"">{ coleta.LocalColeta } </td><td style="" border: 1px solid #C0C0C0; padding: 5px;"">{ coleta.LocalAtual } </td><td style="" border: 1px solid #C0C0C0; padding: 5px; text-align:center"">{ coleta.DiasEmPosse } </td><td style="" border: 1px solid #C0C0C0; padding: 5px; text-align:center"">{ coleta.DataInicioDiaria } </td><td style="" border: 1px solid #C0C0C0; padding: 5px; text-align:center"">{ coleta.ValorDiaria } </td><td style="" border: 1px solid #C0C0C0; padding: 5px; text-align:center"">{ coleta.ValorDevido } </td>";
                mensagem += $@"</tr>";
            }

            mensagem += $@"</tbody></table><div style=""margin-top:20px; background-color:#FFF; font-family: Arial, Helvetica, sans-serif; font-size:14px; line-height: 150%;""><strong> MultiEmbarcador </strong><br /><span style=""font-style:italic""> Multisoftware </span></div><div style=""padding: 20px; font-family: Arial, Helvetica, sans-serif; font-size: 14px; text-align: center; font-size: 10px; color:#CCC"">E-mail enviado automaticamente.</div>";

            Servicos.Email servicoEmail = new Servicos.Email(_unitOfWork);

            servicoEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, emailFilial.Trim(), "", "", assunto, mensagem, string.Empty, null, "", true, string.Empty, 0, _unitOfWork, 0, false);
        }

        private void TrocarCargaAtualColetaContainerAnexo(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer)
        {
            Repositorio.Embarcador.Pedidos.ColetaContainerAnexo repositorioColetaContainerAnexo = new Repositorio.Embarcador.Pedidos.ColetaContainerAnexo(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo> coletaContainerAnexos = repositorioColetaContainerAnexo.BuscarPorColetaContainerECarga(coletaContainer.Codigo, coletaContainer.CargaAtual.Codigo);

            foreach (Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo coletaContainerAnexo in coletaContainerAnexos)
            {
                coletaContainerAnexo.Carga = carga;

                repositorioColetaContainerAnexo.Atualizar(coletaContainerAnexo);
            }
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void InformarEmbarqueContainer(DateTime dataEmbarque, Dominio.Entidades.Cliente LocalEmbarque, Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer, Dominio.Entidades.Usuario Usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMovimentacaoContainer OrigemMovimentacao)
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro parametros = new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro();
            parametros.coletaContainer = coletaContainer;
            parametros.DataEmbarque = dataEmbarque;
            parametros.Status = StatusColetaContainer.Porto;
            parametros.DataAtualizacao = DateTime.Now;
            parametros.LocalEmbarque = LocalEmbarque;
            parametros.LocalAtual = LocalEmbarque;
            parametros.Usuario = Usuario;
            parametros.OrigemMonimentacaoContainer = OrigemMovimentacao;
            parametros.InformacaoOrigemMonimentacaoContainer = InformacaoOrigemMovimentacaoContainer.InformarEmbarqueContainer;

            AtualizarSituacaoColetaContainerEGerarHistorico(parametros);
        }

        public void AtualizarSituacaoColetaContainerEGerarHistorico(Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro parametros)
        {
            if (parametros.coletaContainer == null)
                return;

            Repositorio.Embarcador.Pedidos.ColetaContainer repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(_unitOfWork);
            Repositorio.Embarcador.Pedidos.ColetaContainerHistorico repColetaContainerhistorico = new Repositorio.Embarcador.Pedidos.ColetaContainerHistorico(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = parametros.coletaContainer;
            StatusColetaContainer statusAnterior = coletaContainer.Status;

            //se o container ja esta embarcadoNavio sai fora, isso acontece pois a marfrig por varias vezes movimenta o container nao pelo fluxo correto de confirmar entrega ou demais situações (#58286) entretando a movimentacao MANUAL pode alterar #61110.
            //agora nao pode mais voltar para nenhum outro status diferente do embarcado caso nao for manual #66325
            if (statusAnterior == StatusColetaContainer.EmbarcadoNavio && parametros.Status != StatusColetaContainer.EmbarcadoNavio && parametros.OrigemMonimentacaoContainer != OrigemMovimentacaoContainer.AlteracaoManual)
                return;

            if (parametros.LocalAtual != null)
                coletaContainer.LocalAtual = parametros.LocalAtual;

            if (parametros.LocalEmbarque != null)
                coletaContainer.LocalEmbarque = parametros.LocalEmbarque;

            if (parametros.LocalColeta != null)
                coletaContainer.LocalColeta = parametros.LocalColeta;

            if (parametros.DataEmbarque.HasValue && parametros.DataEmbarque.Value != coletaContainer.DataEmbarque)
                coletaContainer.DataEmbarque = parametros.DataEmbarque.Value;

            if (parametros.DataEmbarqueNavio.HasValue && parametros.DataEmbarqueNavio.Value != coletaContainer.DataEmbarqueNavio)
                coletaContainer.DataEmbarqueNavio = parametros.DataEmbarqueNavio.Value;

            if (parametros.DiasFreeTime > 0)
                coletaContainer.FreeTime = parametros.DiasFreeTime;

            if (parametros.ValorDiaria > 0)
                coletaContainer.ValorDiaria = parametros.ValorDiaria;

            if (parametros.Container != null)
                coletaContainer.Container = parametros.Container;

            if (parametros.DataColeta.HasValue && parametros.DataColeta.Value != coletaContainer.DataColeta)
                coletaContainer.DataColeta = parametros.DataColeta.Value;

            if (parametros.AreaEsperaVazio != null)
                coletaContainer.AreaEsperaVazio = parametros.AreaEsperaVazio;

            coletaContainer.Status = parametros.Status;
            coletaContainer.DataUltimaMovimentacao = parametros.DataAtualizacao;

            if (coletaContainer.Status != StatusColetaContainer.EmAreaEsperaVazio)
                coletaContainer.AreaEsperaVazio = null;

            if (coletaContainer.Status != StatusColetaContainer.EmbarcadoNavio)
                coletaContainer.DataEmbarqueNavio = null;


            List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico> historicosAnteriores = repColetaContainerhistorico.BuscarPorColetaContainer(coletaContainer.Codigo);

            if (statusAnterior == StatusColetaContainer.Porto || (historicosAnteriores?.Any(x => x.Status == StatusColetaContainer.Porto) ?? false))
            {
                List<StatusColetaContainer> statusAnterioresAoEmbarcado = new List<StatusColetaContainer>()
                {
                    StatusColetaContainer.AgColeta,
                    StatusColetaContainer.EmDeslocamentoVazio,
                    StatusColetaContainer.EmAreaEsperaVazio,
                    StatusColetaContainer.EmDeslocamentoCarregamento,
                    StatusColetaContainer.EmCarregamento,
                    StatusColetaContainer.EmDeslocamentoCarregado,
                    StatusColetaContainer.EmAreaEsperaCarregado
                };

                if (statusAnterioresAoEmbarcado.Contains(coletaContainer.Status))
                {
                    coletaContainer.DataEmbarque = null;
                    coletaContainer.LocalEmbarque = null;
                }
            }

            repositorioColetaContainer.Atualizar(coletaContainer);

            CriarHistoricoAtualizacaoContainer(coletaContainer, parametros);
        }

        public void GerarSolicitacaoColetaContainer(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaCargaEntregas, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido)
        {
            if (listaCargaEntregas.Count <= 0)
                return;

            Repositorio.Embarcador.Pedidos.ColetaContainer repColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(_unitOfWork);
            Repositorio.Embarcador.Pedidos.ColetaContainerCargaEntrega repColetaContainerCargaEntrega = new Repositorio.Embarcador.Pedidos.ColetaContainerCargaEntrega(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = listaCargaEntregas.Select(x => x.Carga).FirstOrDefault();
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pessoas.PessoaArmador repPessoaArmador = new Repositorio.Embarcador.Pessoas.PessoaArmador(_unitOfWork);
            Servicos.Embarcador.Pedido.ColetaContainer servicoColetaContainer = new Servicos.Embarcador.Pedido.ColetaContainer(_unitOfWork);

            if (carga == null)
                return;

            Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = repColetaContainer.BuscarPorCargaDeColeta(carga.Codigo);

            if (coletaContainer == null)
                coletaContainer = new Dominio.Entidades.Embarcador.Pedidos.ColetaContainer()
                {
                    CargaDeColeta = carga,
                    CargaAtual = carga,
                    Status = StatusColetaContainer.AgColeta
                };

            coletaContainer.LocalColeta = carga.DadosSumarizados?.ClientesRemetentes?.FirstOrDefault() ?? null;
            coletaContainer.Filial = carga.Filial;

            if (coletaContainer.Codigo <= 0)
                repColetaContainer.Inserir(coletaContainer);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoComContainerInformado = cargasPedido.Where(x => x.Pedido?.Container != null && x.Pedido.DataColetaContainer.HasValue).Select(x => x.Pedido).FirstOrDefault(); //dados informados na importacao do pedido

            if (pedidoComContainerInformado != null)
            {
                //caso veio informacoes do container na importacao a carga deve avancar automaticamente e tambem ja setar valores e dias free time
                if (carga.ExigeNotaFiscalParaCalcularFrete)
                {
                    carga.DataInicioEmissaoDocumentos = DateTime.Now;
                    carga.DataInicioCalculoFrete = DateTime.Now;
                    carga.SituacaoCarga = SituacaoCarga.CalculoFrete;
                    carga.CalculandoFrete = true;
                }
                else
                {
                    carga.DataEnvioUltimaNFe = DateTime.Now;
                }

                repCarga.Atualizar(carga);

                List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoNotaFiscal> cargaPedidoNotaFiscais = repPedidoXMLNotaFiscal.BuscarPedidoXMLNotaFiscal(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    int numeroNotasFiscais = cargaPedidoNotaFiscais.Where(o => o.CargaPedido == cargaPedido.Codigo).Count();
                    if (numeroNotasFiscais == 0)
                    {
                        cargaPedido.PedidoSemNFe = true;
                        repCargaPedido.Atualizar(cargaPedido);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro parametrosColetaContainer = new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro()
                {
                    CargaDaColeta = carga,
                    coletaContainer = coletaContainer,
                    Container = pedidoComContainerInformado.Container,
                    DataAtualizacao = DateTime.Now,
                    LocalAtual = coletaContainer.LocalColeta,
                    DataColeta = pedidoComContainerInformado.DataColetaContainer.Value,
                    Status = StatusColetaContainer.EmDeslocamentoVazio,
                    OrigemMonimentacaoContainer = OrigemMovimentacaoContainer.Sistema,
                    InformacaoOrigemMonimentacaoContainer = InformacaoOrigemMovimentacaoContainer.AutomaticamenteContainerInformadoPedidoImportacao,
                };

                Dominio.Entidades.Embarcador.Pessoas.PessoaArmador pessoaArmador = repPessoaArmador.BuscarPorPessoaETipoContainer(coletaContainer.LocalColeta.CPF_CNPJ, pedidoComContainerInformado.Container.ContainerTipo?.Codigo ?? 0, DateTime.Now);
                if (pessoaArmador == null)
                    pessoaArmador = repPessoaArmador.BuscarPorPessoaETipoContainer(coletaContainer.LocalColeta.CPF_CNPJ, pedidoComContainerInformado.Container.ContainerTipo?.Codigo ?? 0);

                if (pessoaArmador != null)
                {
                    parametrosColetaContainer.DiasFreeTime = pessoaArmador.DiasFreetime ?? 0;
                    parametrosColetaContainer.ValorDiaria = pessoaArmador.ValorDariaAposFreetime ?? 0;
                }

                servicoColetaContainer.AtualizarSituacaoColetaContainerEGerarHistorico(parametrosColetaContainer);

                new Servicos.Embarcador.Carga.MensagemAlertaCarga(_unitOfWork).Confirmar(carga, TipoMensagemAlerta.CargaSemInformacaoContainer);//remover mensagem
            }

            ValidarTipoOperacaoCargaDuplicado(coletaContainer);

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in listaCargaEntregas)
            {
                Dominio.Entidades.Embarcador.Pedidos.ColetaContainerCargaEntrega coletacontainerCargaEntrega = new Dominio.Entidades.Embarcador.Pedidos.ColetaContainerCargaEntrega
                {
                    CargaEntrega = cargaEntrega,
                    ColetaContainer = coletaContainer
                };

                repColetaContainerCargaEntrega.Inserir(coletacontainerCargaEntrega);
            }
        }

        public Dominio.Entidades.Embarcador.Pedidos.ColetaContainer VincularContainerAoColetaContainerCargaColeta(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Container container, DateTime dataColetaContainer)
        {
            Repositorio.Embarcador.Pedidos.ColetaContainer repColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(_unitOfWork);
            Repositorio.Embarcador.Pessoas.PessoaArmador repPessoaArmador = new Repositorio.Embarcador.Pessoas.PessoaArmador(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer;

            if (carga.TipoOperacao?.OperacaoTransferenciaContainer ?? false)
            {
                coletaContainer = repColetaContainer.BuscarAtivoPorContainer(container.Codigo);

                if ((coletaContainer != null) && (coletaContainer.Status != StatusColetaContainer.EmAreaEsperaVazio))
                    throw new ServicoException("Container selecionado não está em área de espera vazio, verifique o status do container antes de vincular a uma carga de transferência");

                if (coletaContainer != null)
                {
                    TrocarCargaAtualColetaContainerAnexo(carga, coletaContainer);

                    coletaContainer.CargaDeColeta = carga;
                    coletaContainer.CargaAtual = carga;
                }
            }
            else
            {
                coletaContainer = repColetaContainer.BuscarPorCargaDeColeta(carga.Codigo);

                if (coletaContainer != null)
                    coletaContainer.DataColeta = dataColetaContainer;
            }

            if (coletaContainer == null)
                coletaContainer = new Dominio.Entidades.Embarcador.Pedidos.ColetaContainer()
                {
                    CargaDeColeta = carga,
                    CargaAtual = carga,
                    DataColeta = dataColetaContainer
                };

            ValidarTipoOperacaoCargaDuplicado(coletaContainer);

            coletaContainer.Initialize();
            coletaContainer.Container = container;
            coletaContainer.Filial = carga.Filial;

            Dominio.Entidades.Cliente ClienteArmador = coletaContainer.LocalColeta;

            if (ClienteArmador != null && ClienteArmador.Armador)
            {
                Dominio.Entidades.Embarcador.Pessoas.PessoaArmador pessoaArmador = repPessoaArmador.BuscarPorPessoaETipoContainer(ClienteArmador.CPF_CNPJ, container.ContainerTipo?.Codigo ?? 0, DateTime.Now);
                if (pessoaArmador == null)
                    pessoaArmador = repPessoaArmador.BuscarPorPessoaETipoContainer(ClienteArmador.CPF_CNPJ, container.ContainerTipo?.Codigo ?? 0);

                if (coletaContainer.IsChangedByPropertyName("Container"))//se mudou container
                {
                    coletaContainer.FreeTime = pessoaArmador?.DiasFreetime ?? 0;
                    coletaContainer.ValorDiaria = pessoaArmador?.ValorDariaAposFreetime ?? 0;

                    if (container.ClienteArmador == null)
                    {
                        container.ClienteArmador = ClienteArmador;
                        repContainer.Atualizar(container);
                    }
                }
            }

            if (coletaContainer.Codigo > 0)
                repColetaContainer.Atualizar(coletaContainer);
            else
                repColetaContainer.Inserir(coletaContainer);

            new Servicos.Embarcador.Carga.MensagemAlertaCarga(_unitOfWork).Confirmar(carga, TipoMensagemAlerta.CargaSemInformacaoContainer);//remover mensagem

            return coletaContainer;
        }

        public void RemoverContainerCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer, Dominio.Entidades.Usuario usuario)
        {
            if (
                (carga.SituacaoCarga == SituacaoCarga.Cancelada) ||
                (carga.SituacaoCarga == SituacaoCarga.Encerrada) ||
                (carga.SituacaoCarga == SituacaoCarga.EmTransporte) ||
                (carga.SituacaoCarga == SituacaoCarga.CalculoFrete) ||
                (carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos) ||
                (carga.SituacaoCarga == SituacaoCarga.Anulada)
            )
                throw new ServicoException("A situação atual da carga não permite remover o container.");

            Repositorio.Embarcador.Pedidos.ColetaContainer repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico ultimoHistoricoCargaDeColetaAnterior = null;

            if ((carga.TipoOperacao?.OperacaoTransferenciaContainer ?? false) && (coletaContainer.CargaDeColeta.Codigo == carga.Codigo))
            {
                Repositorio.Embarcador.Pedidos.ColetaContainerHistorico repositorioColetaContainerhistorico = new Repositorio.Embarcador.Pedidos.ColetaContainerHistorico(_unitOfWork);

                ultimoHistoricoCargaDeColetaAnterior = repositorioColetaContainerhistorico.BuscarUltimoHistoricoCargaAnteriorPorColetaContainer(coletaContainer.Codigo, carga.Codigo);
            }

            Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro parametrosColetaContainer = new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro()
            {
                coletaContainer = coletaContainer,
                DataAtualizacao = DateTime.Now,
                Usuario = usuario,
                OrigemMonimentacaoContainer = OrigemMovimentacaoContainer.UsuarioInterno,
                InformacaoOrigemMonimentacaoContainer = InformacaoOrigemMovimentacaoContainer.AoRemoverContainerCarga
            };

            if (ultimoHistoricoCargaDeColetaAnterior != null)
            {
                TrocarCargaAtualColetaContainerAnexo(ultimoHistoricoCargaDeColetaAnterior.Carga, coletaContainer);

                coletaContainer.CargaDeColeta = ultimoHistoricoCargaDeColetaAnterior.Carga;
                coletaContainer.CargaAtual = ultimoHistoricoCargaDeColetaAnterior.Carga;
                parametrosColetaContainer.LocalAtual = ultimoHistoricoCargaDeColetaAnterior.Local;
                parametrosColetaContainer.Status = StatusColetaContainer.EmAreaEsperaVazio;

                new Carga.MensagemAlertaCarga(_unitOfWork).RemoverConfirmacao(carga, TipoMensagemAlerta.CargaSemInformacaoContainer);
            }
            else
            {
                if (coletaContainer.CargaAtual?.Codigo == carga.Codigo)
                    coletaContainer.CargaAtual = null;

                if (coletaContainer.CargaDeColeta?.Codigo == carga.Codigo)
                {
                    coletaContainer.Container = null;
                    coletaContainer.LocalAtual = null;
                    coletaContainer.ValorDiaria = 0;
                    parametrosColetaContainer.Status = StatusColetaContainer.AgColeta;
                }
            }

            repositorioColetaContainer.Atualizar(coletaContainer);
            AtualizarSituacaoColetaContainerEGerarHistorico(parametrosColetaContainer);
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Pedido.ColetaContainerRetornoCalculoDiaria> ObterContainersComDiariaExcedidaNotificacao()
        {
            Repositorio.Embarcador.Pedidos.ColetaContainer repColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(_unitOfWork);
            IList<Dominio.ObjetosDeValor.Embarcador.Pedido.ColetaContainerRetornoCalculoDiaria> coletaContainerExcedidaEmPosseEmbarcador = repColetaContainer.BuscarContainersEmPosseExcedido();

            return coletaContainerExcedidaEmPosseEmbarcador;

        }

        public void NotificarDiariasContainer(IList<Dominio.ObjetosDeValor.Embarcador.Pedido.ColetaContainerRetornoCalculoDiaria> coletasContainerDiariasSumarizadas)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.ColetaContainerRetornoCalculoDiaria> listaOrdenadaFilial = coletasContainerDiariasSumarizadas.OrderBy(x => x.Filial).ToList();

            dynamic listaAgrupadaFilial = (
                from obj in listaOrdenadaFilial
                group obj by obj.Filial
                into grupo
                select new
                {
                    Filial = grupo.Key,
                    Coletas = grupo.Select(coletacontainer => new
                    {
                        coletacontainer.Codigo,
                        coletacontainer.NumeroContainer,
                        DataColeta = coletacontainer.DataColeta != DateTime.MinValue ? coletacontainer.DataColeta.ToString("dd/MM/yyyy") : "",
                        Status = coletacontainer.Status.ObterDescricao(),
                        DataUltimaMovimentacao = coletacontainer.DataUltimaMovimentacao.ToString("dd/MM/yyyy"),
                        LocalColeta = coletacontainer.LocalColeta > 0 ? coletacontainer.ClienteLocalColeta + "(" + coletacontainer.LocalColeta + ")" : "",
                        LocalAtual = coletacontainer.LocalAtual > 0 ? coletacontainer.ClienteLocalAtual + "(" + coletacontainer.LocalAtual + ")" : "",
                        LocalEmbarque = coletacontainer.LocalEmbarque > 0 ? coletacontainer.ClienteLocalEmbarque + "(" + coletacontainer.LocalEmbarque + ")" : "",
                        DataInicioDiaria = coletacontainer.DataLimiteFreeTime != DateTime.MinValue ? DateTime.Parse(coletacontainer.DataLimiteFreeTime.ToString()).AddDays(1).ToString("dd/MM/yyyy") : "",
                        ValorDevido = coletacontainer.ValorDevido.ToString("N2"),
                        DiasEmPosse = coletacontainer.DiasEmPosse,
                        ValorDiaria = coletacontainer.ValorDiaria.ToString("N2"),
                        DataLimiteFreeTime = coletacontainer.DataLimiteFreeTime != DateTime.MinValue ? coletacontainer.DataLimiteFreeTime.ToString("dd/MM/yyyy") : ""
                    }).ToList()
                }
            ).ToList();


            foreach (var coletaExcedida in listaAgrupadaFilial)
            {
                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigo(coletaExcedida.Filial);

                if (!string.IsNullOrWhiteSpace(filial.EmailDiariaContainer))
                    EnviarEmailNotificacaoDiariaContainer(coletaExcedida, filial.EmailDiariaContainer);
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.Cargas.Carga cargaAtual, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.Pedidos.ColetaContainer repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(_unitOfWork);
            Repositorio.Embarcador.Pedidos.RetiradaContainer repositorioRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer retiradaContainer = repositorioRetiradaContainer.BuscarPorCarga(cargaAtual.Codigo);
            Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer;

            if (retiradaContainer != null)
            {
                coletaContainer = retiradaContainer.ColetaContainer;
                retiradaContainer.Carga = cargaNova;

                repositorioRetiradaContainer.Atualizar(retiradaContainer);
            }
            else
                coletaContainer = repositorioColetaContainer.BuscarPorCargaAtual(cargaAtual.Codigo);

            if (coletaContainer != null)
            {
                if (coletaContainer.CargaDeColeta.Codigo == coletaContainer.CargaAtual.Codigo)
                    coletaContainer.CargaDeColeta = cargaNova;

                coletaContainer.CargaAtual = cargaNova;

                AtualizarSituacaoColetaContainerEGerarHistorico(new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro()
                {
                    coletaContainer = coletaContainer,
                    DataAtualizacao = DateTime.Now,
                    Status = coletaContainer.Status,
                    OrigemMonimentacaoContainer = OrigemMovimentacaoContainer.Sistema,
                    InformacaoOrigemMonimentacaoContainer = InformacaoOrigemMovimentacaoContainer.TrocarPreCargaCarga
                });
            }
        }

        public void CancelarColetaContainer(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Pedidos.ColetaContainer repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(_unitOfWork);
            Repositorio.Embarcador.Pedidos.RetiradaContainer repositorioRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer retiradaContainer = repositorioRetiradaContainer.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer;

            bool cargaRedespacho = carga.Redespacho != null;

            if (retiradaContainer != null)
                coletaContainer = retiradaContainer.ColetaContainer;
            else
                coletaContainer = repositorioColetaContainer.BuscarPorCargaAtual(carga.Codigo);

            if (coletaContainer == null)
                return;

            bool cancelamentoExpComCargaDeColeta = (
                (coletaContainer.CargaDeColeta.Codigo != coletaContainer.CargaAtual.Codigo) &&
                (coletaContainer.CargaDeColeta.TipoOperacao?.ObrigatorioVincularContainerCarga ?? false) &&
                new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork).ExisteNumeroEXPPorCarga(coletaContainer.CargaAtual.Codigo)
            );

            if (cancelamentoExpComCargaDeColeta)
            {
                Repositorio.Embarcador.Pedidos.ColetaContainerHistorico repositorioColetaContainerhistorico = new Repositorio.Embarcador.Pedidos.ColetaContainerHistorico(_unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico ultimoHistoricoCargaAnterior = repositorioColetaContainerhistorico.BuscarUltimoHistoricoCargaAnteriorPorColetaContainer(coletaContainer.Codigo, coletaContainer.CargaAtual.Codigo);

                if (cargaRedespacho && coletaContainer.Status == StatusColetaContainer.EmAreaEsperaCarregado)
                    return;

                coletaContainer.CargaAtual = ultimoHistoricoCargaAnterior?.Carga ?? coletaContainer.CargaDeColeta;

                AtualizarSituacaoColetaContainerEGerarHistorico(new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro()
                {
                    AreaEsperaVazio = ultimoHistoricoCargaAnterior?.Local ?? coletaContainer.LocalColeta,
                    coletaContainer = coletaContainer,
                    DataAtualizacao = DateTime.Now,
                    LocalAtual = ultimoHistoricoCargaAnterior?.Local ?? coletaContainer.LocalColeta,
                    Status = cargaRedespacho ? StatusColetaContainer.EmAreaEsperaCarregado : StatusColetaContainer.EmAreaEsperaVazio,
                    OrigemMonimentacaoContainer = OrigemMovimentacaoContainer.Sistema,
                    InformacaoOrigemMonimentacaoContainer = InformacaoOrigemMovimentacaoContainer.CancelarControleContainer
                });

                return;
            }

            AtualizarSituacaoColetaContainerEGerarHistorico(new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro()
            {
                coletaContainer = coletaContainer,
                DataAtualizacao = DateTime.Now,
                Status = StatusColetaContainer.Cancelado,
                OrigemMonimentacaoContainer = OrigemMovimentacaoContainer.Sistema,
                InformacaoOrigemMonimentacaoContainer = InformacaoOrigemMovimentacaoContainer.CancelarControleContainer
            });
        }

        public Dominio.Entidades.Embarcador.Pedidos.Container CadastrarContainer(CadastroDeContainer objContainer, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Dominio.Entidades.Embarcador.Pedidos.Container container = new Dominio.Entidades.Embarcador.Pedidos.Container();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

            var armador = repCliente.BuscarPorNomeIgualOuParecido(objContainer.ArmadorBooking);

            container.Descricao = objContainer.Container;
            container.Numero = Utilidades.String.SanitizeString(objContainer.Container);
            container.CodigoIntegracao = null;
            container.Status = true;
            container.PesoLiquido = 0M;
            container.Tara = objContainer.TaraContainer;
            container.Valor = 0M;
            container.TipoPropriedade = TipoPropriedadeContainer.Soc;
            container.TipoCarregamentoNavio = TipoCarregamentoNavio.NaoInformado;

            Repositorio.Embarcador.Pedidos.ContainerTipo repTipoContainer = new Repositorio.Embarcador.Pedidos.ContainerTipo(_unitOfWork);
            var tipoContainer = repTipoContainer.BuscarTodosPorCodigoIntegracao(objContainer.TipoContainerCodigoIntegracao);

            container.ContainerTipo = tipoContainer;
            container.MetrosCubicos = 0M;
            container.DataUltimaAtualizacao = DateTime.Now;
            container.Integrado = false;

            container.ClienteArmador = armador;

            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(_unitOfWork);

            repContainer.Inserir(container, auditado);

            if (container.TipoPropriedade == TipoPropriedadeContainer.Proprio && !string.IsNullOrWhiteSpace(container.Numero))
            {
                var servicoPedido = new Servicos.Embarcador.Pedido.Pedido(_unitOfWork);
                if (!servicoPedido.ValidarDigitoContainerNumero(container.Numero))
                    throw new Exception(Localization.Resources.Consultas.Container.NumeroDoContainerEstaInvalidoDeAcordoComSeuDigitoVerificado);
            }

            if (container.Tara <= 0)
                throw new ServicoException(Localization.Resources.Consultas.Container.FavorInformeTaraDoContainer);

            if (repContainer.ValidarDuplicidadeContainer(container.Numero, container.Codigo))
                throw new ServicoException(Localization.Resources.Consultas.Container.JaExisteUmContainerCadastradoComEsteNumero);

            return container;
        }

        public void ValidarTipoOperacaoCargaDuplicado(Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer)
        {
            if (coletaContainer?.CargaAtual?.TipoOperacao == null)
                return;

            Repositorio.Embarcador.Pedidos.ColetaContainerHistorico repositorioColetaContainerhistorico = new Repositorio.Embarcador.Pedidos.ColetaContainerHistorico(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico> historicos = repositorioColetaContainerhistorico.BuscarPorContainer(coletaContainer.Container?.Codigo ?? 0);

            //caso o container ja foi removido de uma carga anteriormente, deixa passar;
            if (historicos.Any(obj => obj.InformacaoOrigemMovimentacao == InformacaoOrigemMovimentacaoContainer.AoRemoverContainerCarga))
                return;

            IEnumerable<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAnterioresMesmoTipoOperacao = historicos
                .Where(historico =>
                    (historico.ColetaContainer.Codigo == coletaContainer.Codigo) &&
                    (historico.Carga != null) &&
                    (historico.Carga.SituacaoCarga != SituacaoCarga.Cancelada) &&
                    (historico.Carga.SituacaoCarga != SituacaoCarga.Anulada) &&
                    (historico.Carga.Codigo != coletaContainer.CargaAtual.Codigo) &&
                    (historico.Carga.TipoOperacao?.Codigo == coletaContainer.CargaAtual.TipoOperacao.Codigo)
                )
                .Select(historico => historico.Carga)
                .Distinct();

            if (cargasAnterioresMesmoTipoOperacao.Count() == 0)
                return;

            string numeroCarga = cargasAnterioresMesmoTipoOperacao.FirstOrDefault().CodigoCargaEmbarcador;

            throw new ServicoException($"Não é possível vincular o container a uma carga com o mesmo tipo de operação da carga {numeroCarga}");
        }

        public void EnviarEmaiCancelamentoCargaComContainer(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!(carga.Filial?.NotificarContainerCargaCancelada ?? false) || string.IsNullOrWhiteSpace(carga.Filial.EmailDiariaContainer))
                return;

            Repositorio.Embarcador.Pedidos.RetiradaContainer repRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer retiradaContainer = repRetiradaContainer.BuscarPorCarga(carga.Codigo);

            if (retiradaContainer == null)
                return;

            List<string> emails = new List<string>();

            emails.Add(carga.Filial.EmailDiariaContainer);

            string assunto = $"Notificação de Carga Cancelada com Container vinculado";
            string mensagem = $"A EXP {carga.CodigoCargaEmbarcador} que estava atrelado ao contêiner {retiradaContainer.Container.Numero} foi cancelada";

            Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamentoColeta = new Servicos.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
            servicoAgendamentoColeta.EnviarEmailAgendamento(assunto, mensagem, emails);
        }

        #endregion Métodos Públicos
    }
}
