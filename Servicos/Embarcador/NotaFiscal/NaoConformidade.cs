using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.NotaFiscal
{
    public sealed class NaoConformidade
    {
        #region Atributos

        private readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        private readonly NaoConformidadeAprovacao _servicoNaoConformidadeAprovacao;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidade> _itensNaoConformidade;

        #endregion Atributos

        #region Construtores

        public NaoConformidade(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, clienteMultisoftware: null) { }

        public NaoConformidade(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            _clienteMultisoftware = clienteMultisoftware;
            _unitOfWork = unitOfWork;
            _servicoNaoConformidadeAprovacao = new NaoConformidadeAprovacao(unitOfWork);
        }

        #endregion Construtores

        #region Métodos Privados

        private void Adicionar(Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.PedidoXmlNotaFiscal pedidoXmlNotaFiscal, Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidade itemNaoConformidade)
        {
            Adicionar(pedidoXmlNotaFiscal.CargaPedido.Codigo, itemNaoConformidade.Codigo, pedidoXmlNotaFiscal.XmlNotaFiscal.Codigo, tipoParticipante: null);
        }

        private void Adicionar(Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.PedidoXmlNotaFiscal pedidoXmlNotaFiscal, Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidade itemNaoConformidade, TipoParticipante? tipoParticipante)
        {
            Adicionar(pedidoXmlNotaFiscal.CargaPedido.Codigo, itemNaoConformidade.Codigo, pedidoXmlNotaFiscal.XmlNotaFiscal.Codigo, tipoParticipante);
        }

        private void Adicionar(int codigoCargaPedido, int codigoItemNaoConformidade, int codigoXmlNotaFiscal, TipoParticipante? tipoParticipante)
        {
            Repositorio.Embarcador.NotaFiscal.NaoConformidade repositorioNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNaoConformidade filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNaoConformidade()
            {
                CodigoCargaPedido = codigoCargaPedido,
                CodigoItemNaoConformidade = codigoItemNaoConformidade,
                CodigoXMLNotaFiscal = codigoXmlNotaFiscal,
                TipoParticipante = tipoParticipante
            };

            if (repositorioNaoConformidade.ExisteNaoConformidade(filtrosPesquisa))
                return;

            Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade itemNaoConformidade = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade(_unitOfWork).BuscarPorCodigo(codigoItemNaoConformidade);

            Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade = new Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade()
            {
                CargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido),
                ItemNaoConformidade = itemNaoConformidade,
                XMLNotaFiscal = (codigoXmlNotaFiscal > 0) ? new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal() { Codigo = codigoXmlNotaFiscal } : null,
                TipoParticipante = tipoParticipante,
                Situacao = (itemNaoConformidade.PermiteContingencia) ? SituacaoNaoConformidade.SemRegraAprovacao : SituacaoNaoConformidade.AguardandoTratativa
            };

            repositorioNaoConformidade.Inserir(naoConformidade);
            _servicoNaoConformidadeAprovacao.CriarAprovacao(naoConformidade.Codigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            NotificarStatusParaTransportador(naoConformidade, true);
        }

        private void AtualizarParaAjustadaAutomaticamente(Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.PedidoXmlNotaFiscal pedidoXmlNotaFiscal, Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidade itemNaoConformidade)
        {
            AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal.CargaPedido.Codigo, itemNaoConformidade.Codigo, pedidoXmlNotaFiscal.XmlNotaFiscal.Codigo, tipoParticipante: null);
        }

        private void AtualizarParaAjustadaAutomaticamente(Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.PedidoXmlNotaFiscal pedidoXmlNotaFiscal, Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidade itemNaoConformidade, TipoParticipante? tipoParticipante)
        {
            AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal.CargaPedido.Codigo, itemNaoConformidade.Codigo, pedidoXmlNotaFiscal.XmlNotaFiscal.Codigo, tipoParticipante);
        }

        private void AtualizarParaAjustadaAutomaticamente(int codigoCargaPedido, int codigoItemNaoConformidade, int codigoXmlNotaFiscal, TipoParticipante? tipoParticipante)
        {
            Repositorio.Embarcador.NotaFiscal.NaoConformidade repositorioNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNaoConformidade filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNaoConformidade()
            {
                CodigoCargaPedido = codigoCargaPedido,
                CodigoItemNaoConformidade = codigoItemNaoConformidade,
                CodigoXMLNotaFiscal = codigoXmlNotaFiscal,
                TipoParticipante = tipoParticipante
            };

            repositorioNaoConformidade.AtualizarSituacao(filtrosPesquisa, SituacaoNaoConformidade.ConcluidaPorIntegracao);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidade> ObterItensNaoConformidade()
        {
            if (_itensNaoConformidade != null)
                return _itensNaoConformidade;

            _itensNaoConformidade = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade(_unitOfWork).BuscarAtivos();

            if (_itensNaoConformidade.Count > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidadeParticipantes> itensNaoConformidadeParticipantes = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeParticipante(_unitOfWork).BuscarPorItensNaoConformidadeAtivos();
                List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidadeTiposOperacao> itensNaoConformidadeTiposOperacao = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeTiposOperacao(_unitOfWork).BuscarPorItensNaoConformidadeAtivos();
                List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidadeCFOP> itensNaoConformidadeCFOP = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeCFOP(_unitOfWork).BuscarPorItensNaoConformidadeAtivos();
                List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidadeFilial> itensNaoConformidadeFilial = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeFilial(_unitOfWork).BuscarPorItensNaoConformidadeAtivos();
                List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidadeFornecedor> itensNaoConformidadeFornecedor = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeFornecedor(_unitOfWork).BuscarPorItensNaoConformidadeAtivos();

                foreach (Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidade itemNaoConformidade in _itensNaoConformidade)
                {
                    itemNaoConformidade.Participantes = itensNaoConformidadeParticipantes.Where(item => item.CodigoItemNaoConformidade == itemNaoConformidade.Codigo).ToList();
                    itemNaoConformidade.TiposOperacao = itensNaoConformidadeTiposOperacao.Where(item => item.CodigoItemNaoConformidade == itemNaoConformidade.Codigo).ToList();
                    itemNaoConformidade.CFOP = itensNaoConformidadeCFOP.Where(item => item.CodigoItemNaoConformidade == itemNaoConformidade.Codigo).ToList();
                    itemNaoConformidade.Filial = itensNaoConformidadeFilial.Where(item => item.CodigoItemNaoConformidade == itemNaoConformidade.Codigo).ToList();
                    itemNaoConformidade.Fornecedor = itensNaoConformidadeFornecedor.Where(item => item.CodigoItemNaoConformidade == itemNaoConformidade.Codigo).ToList();
                }
            }

            return _itensNaoConformidade;
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Cliente ObterParticipante(Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.CargaPedido cargaPedido, TipoParticipante tipoParticipante)
        {
            switch (tipoParticipante)
            {
                case TipoParticipante.Remetente: return cargaPedido.Pedido.Remetente;
                case TipoParticipante.Destinatario: return cargaPedido.Pedido.Destinatario;
                case TipoParticipante.Expedidor: return cargaPedido.Expedidor;
                case TipoParticipante.Recebedor: return cargaPedido.Recebedor;
                default: return null;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Cliente ObterParticipante(Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.XmlNotaFiscal xmlNotaFiscal, TipoParticipante tipoParticipante)
        {
            switch (tipoParticipante)
            {
                case TipoParticipante.Remetente: return xmlNotaFiscal.Emitente;
                case TipoParticipante.Destinatario: return xmlNotaFiscal.Destinatario;
                case TipoParticipante.Expedidor: return xmlNotaFiscal.Expedidor;
                case TipoParticipante.Recebedor: return xmlNotaFiscal.Recebedor;
                default: return null;
            }
        }

        private TipoTomador ObterTipoTomador(Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.CargaPedido cargaPedido)
        {
            if ((cargaPedido.TipoTomador == TipoTomador.Intermediario) || (cargaPedido.TipoTomador == TipoTomador.NaoInformado) || (cargaPedido.TipoTomador == TipoTomador.Outros))
                return TipoTomador.Remetente;

            return cargaPedido.TipoTomador;
        }

        private TipoTomador ObterTipoTomador(Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.XmlNotaFiscal xmlNotaFiscal)
        {
            if (xmlNotaFiscal.ModalidadeFrete == ModalidadePagamentoFrete.A_Pagar)
                return (xmlNotaFiscal.Recebedor != null) ? TipoTomador.Recebedor : TipoTomador.Destinatario;

            return (xmlNotaFiscal.Expedidor != null) ? TipoTomador.Expedidor : TipoTomador.Remetente;
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Cliente ObterTomador(Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.CargaPedido cargaPedido)
        {
            switch (cargaPedido.TipoTomador)
            {
                case TipoTomador.Destinatario: return cargaPedido.Pedido.Destinatario;
                case TipoTomador.Expedidor: return cargaPedido.Expedidor;
                case TipoTomador.Recebedor: return cargaPedido.Recebedor;
                default: return cargaPedido.Pedido.Remetente;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Cliente ObterTomador(Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.XmlNotaFiscal xmlNotaFiscal)
        {
            switch (xmlNotaFiscal.ModalidadeFrete)
            {
                case ModalidadePagamentoFrete.A_Pagar: return xmlNotaFiscal.Recebedor ?? xmlNotaFiscal.Destinatario;
                default: return xmlNotaFiscal.Expedidor ?? xmlNotaFiscal.Emitente;
            }
        }

        private void EnviarEmailIndividual(List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade> naoConformidades)
        {
            Servicos.Email servicoEmail = new Servicos.Email(_unitOfWork);

            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NaoConformidade repNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(_unitOfWork);
            Repositorio.Embarcador.Notificacoes.ConfiguracaoNCPendente repConfiguracaoEnvioNC = new Repositorio.Embarcador.Notificacoes.ConfiguracaoNCPendente(_unitOfWork);

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();
            foreach (Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade in naoConformidades)
            {
                bool emailEnviado = false;
                List<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente> configuracaoEnvioNC = repConfiguracaoEnvioNC.BuscarConfiguracaoPorNaoConformidade(naoConformidade);

                List<Dominio.Entidades.Usuario> destinatarios = configuracaoEnvioNC.SelectMany(o => o?.Usuarios).ToList();

                foreach (Dominio.Entidades.Usuario destinatario in destinatarios)
                {
                    if (string.IsNullOrWhiteSpace(destinatario.Email))
                        continue;

                    string titulo = "Não Conformidades Pendentes";

                    StringBuilder mensagem = new StringBuilder();
                    mensagem.AppendLine($"Olá, {destinatario.Nome}");
                    mensagem.AppendLine();
                    mensagem.AppendLine("Você possui uma Não Conformidade pendente de tratativa que está relacionada a suas operações: ");
                    mensagem.AppendLine();

                    if ((naoConformidade.XMLNotaFiscal?.Numero ?? 0) > 0)
                        mensagem.AppendLine($"Número da Nota: {naoConformidade.XMLNotaFiscal.Numero}");

                    if (!string.IsNullOrWhiteSpace(naoConformidade.CargaPedido?.Carga?.CodigoCargaEmbarcador))
                        mensagem.AppendLine($"Número da Carga: {naoConformidade.CargaPedido.Carga.CodigoCargaEmbarcador}");

                    if (!string.IsNullOrWhiteSpace(naoConformidade.ItemNaoConformidade?.Descricao))
                        mensagem.AppendLine($"Descrição: {naoConformidade.ItemNaoConformidade.Descricao}");

                    mensagem.AppendLine($"Data de Geração: {naoConformidade.DataCriacao}");
                    mensagem.AppendLine($"Situação da Aprovação: {naoConformidade.Situacao.ObterDescricao()}");
                    mensagem.AppendLine();
                    mensagem.AppendLine("Envio de e-mail automático. Favor não responder");

                    servicoEmail.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, destinatario.Email, null, null, titulo, mensagem.ToString(), configuracaoEmail.Smtp, null, null, configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp, _unitOfWork);
                    emailEnviado = true;
                }
                if (emailEnviado)
                {
                    naoConformidade.EmailEnviado = true;
                    repNaoConformidade.Atualizar(naoConformidade);
                }
            }
        }

        private void EnviarEmailResumo(List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade> naoConformidades)
        {
            Servicos.Email servicoEmail = new Servicos.Email(_unitOfWork);

            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NaoConformidade repNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(_unitOfWork);
            Repositorio.Embarcador.Notificacoes.ConfiguracaoNCPendente repConfiguracaoEnvioNC = new Repositorio.Embarcador.Notificacoes.ConfiguracaoNCPendente(_unitOfWork);

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

            List<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente> configuracoesEnvioNC = new List<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente>();
            bool emailsEnviados = false;

            foreach (Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade in naoConformidades)
                configuracoesEnvioNC.AddRange(repConfiguracaoEnvioNC.BuscarConfiguracaoPorNaoConformidade(naoConformidade));
            configuracoesEnvioNC = configuracoesEnvioNC.Distinct().ToList();

            List<Dominio.Entidades.Usuario> destinatarios = configuracoesEnvioNC.SelectMany(obj => obj.Usuarios).Distinct().ToList();

            foreach (Dominio.Entidades.Usuario destinatario in destinatarios)
            {
                Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente configuracao = configuracoesEnvioNC.Where(obj => obj.Usuarios.Contains(destinatario)).FirstOrDefault();

                string titulo = "Não Conformidades Pendentes";

                List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade> naoConformidadesEnviar = naoConformidades.Where(obj => !(obj.EmailEnviado.HasValue) && configuracao.ItemNaoConformidade.Contains(obj.ItemNaoConformidade) && configuracao.TipoOperacao.Contains(obj.CargaPedido.Carga.TipoOperacao) && configuracao.Filial.Contains(obj.CargaPedido.Carga.Filial)).ToList();

                StringBuilder mensagem = new StringBuilder();
                mensagem.AppendLine($"Olá, {destinatario.Nome}");
                mensagem.AppendLine();
                mensagem.AppendLine("Existem as Não Conformidades pendentes de tratativa que estão relacionadas às suas operações: ");
                mensagem.AppendLine();

                foreach (Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade in naoConformidadesEnviar)
                {

                    if ((naoConformidade.XMLNotaFiscal?.Numero ?? 0) > 0)
                        mensagem.AppendLine($"Número da Nota: {naoConformidade.XMLNotaFiscal.Numero}");

                    if (!string.IsNullOrWhiteSpace(naoConformidade.CargaPedido?.Carga?.CodigoCargaEmbarcador))
                        mensagem.AppendLine($"Número da Carga: {naoConformidade.CargaPedido.Carga.CodigoCargaEmbarcador}");

                    if (!string.IsNullOrWhiteSpace(naoConformidade.ItemNaoConformidade?.Descricao))
                        mensagem.AppendLine($"Descrição: {naoConformidade.ItemNaoConformidade.Descricao}");

                    mensagem.AppendLine($"Data de Geração: {naoConformidade.DataCriacao}");
                    mensagem.AppendLine($"Situação da Aprovação: {naoConformidade.Situacao.ObterDescricao()}");
                    mensagem.AppendLine();
                }
                mensagem.AppendLine("Envio de e-mail automático. Favor não responder");
                servicoEmail.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, destinatario.Email, null, null, titulo, mensagem.ToString(), configuracaoEmail.Smtp, null, null, configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp, _unitOfWork);
                emailsEnviados = true;
            }

            if (emailsEnviados)
            {
                foreach (Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade in naoConformidades)
                {
                    naoConformidade.EmailEnviado = true;
                    repNaoConformidade.Atualizar(naoConformidade);
                }
            }
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public bool Validar(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!(carga.TipoOperacao?.ConfiguracaoCarga?.AtivoModuloNaoConformidades ?? false))
                return true;

            List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidade> itensNaoConformidade = ObterItensNaoConformidade();

            if (itensNaoConformidade.Count == 0)
                return true;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NaoConformidade repositorioNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa servicoDocumentoDestinadoEmpresa = new Documentos.DocumentoDestinadoEmpresa(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.FiltroPesquisaPedidoXmlNotaFiscal filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.FiltroPesquisaPedidoXmlNotaFiscal()
            {
                CodigoCarga = carga.Codigo
            };

            List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.PedidoXmlNotaFiscal> pedidoXmlNotasFiscais = repositorioPedidoXMLNotaFiscal.BuscarDadosPedidoXmlNotaFiscal(filtrosPesquisa);
            List<int> codigosCargaPedidos = repositorioCargaPedido.BuscarCodigosPorCarga(carga.Codigo);
            List<int> codigosCargaPedidosSemNotaFiscal = codigosCargaPedidos.Where(codigoCargaPedido => !pedidoXmlNotasFiscais.Any(o => o.CargaPedido.Codigo == codigoCargaPedido)).ToList();
            var itemNaoConformidadeProduto = itensNaoConformidade.Where(x => x.TipoRegra == TipoRegraNaoConformidade.Produto).FirstOrDefault();

            if (itemNaoConformidadeProduto != null)
            {
                List<XmlNotaFiscalProduto> produtosNotaFiscal = new List<XmlNotaFiscalProduto>();
                List<CargaPedidoProduto> produtosPedidos = new List<CargaPedidoProduto>();
                foreach (var pedidoXmlNotaFiscal in pedidoXmlNotasFiscais)
                {
                    produtosPedidos.AddRange(pedidoXmlNotaFiscal.CargaPedido.Produtos.ToList());
                    produtosNotaFiscal.AddRange(pedidoXmlNotaFiscal.XmlNotaFiscal.Produtos.ToList());
                }

                if (produtosNotaFiscal.Any(x => !produtosPedidos.Any(o => x.ProdutoEmbarcador.Codigo == o.ProdutoEmbarcador.Codigo)) || produtosPedidos.Any(x => !produtosNotaFiscal.Any(o => x.ProdutoEmbarcador.Codigo == o.ProdutoEmbarcador.Codigo)))
                    Adicionar(codigoCargaPedido: 0, itemNaoConformidadeProduto.Codigo, codigoXmlNotaFiscal: 0, tipoParticipante: null);
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidade itemNaoConformidade in itensNaoConformidade)
            {
                bool tipoOperacaoPermitido = (itemNaoConformidade.TiposOperacao.Count == 0) || itemNaoConformidade.TiposOperacao.Any(o => o.CodigoTipoOperacao == carga.TipoOperacao.Codigo);
                bool filialPermitido = (itemNaoConformidade.Filial.Count == 0) || itemNaoConformidade.Filial.Any(o => o.Filial == (carga?.Filial?.Codigo ?? 0));

                if (!tipoOperacaoPermitido || !filialPermitido)
                    continue;

                if (itemNaoConformidade.TipoRegra == TipoRegraNaoConformidade.ExisteXmlNota)
                {
                    foreach (int codigoCargaPedido in codigosCargaPedidosSemNotaFiscal)
                        Adicionar(codigoCargaPedido, itemNaoConformidade.Codigo, codigoXmlNotaFiscal: 0, tipoParticipante: null);

                    continue;
                }

                foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.PedidoXmlNotaFiscal pedidoXmlNotaFiscal in pedidoXmlNotasFiscais)
                {
                    IEnumerable<TipoParticipante> tiposParticipante = itemNaoConformidade.Participantes.Select(participante => participante.Participante);
                    bool permiteCFOP = (itemNaoConformidade.CFOP.Count == 0) || (itemNaoConformidade.CFOP.Any(x => x.CFOP == pedidoXmlNotaFiscal.XmlNotaFiscal.CodigoCFOP));
                    bool permiteFornecedor = (itemNaoConformidade.Fornecedor.Count == 0) || (itemNaoConformidade.Fornecedor.Any(x => x.Fornecedor == pedidoXmlNotaFiscal.XmlNotaFiscal.Emitente.CpfCnpj));

                    if (!permiteCFOP || !permiteFornecedor)
                        continue;

                    if (itemNaoConformidade.TipoRegra == TipoRegraNaoConformidade.ValidarRaiz)
                    {
                        foreach (TipoParticipante tipoParticipante in tiposParticipante)
                        {
                            if (ObterParticipante(pedidoXmlNotaFiscal.CargaPedido, tipoParticipante)?.CpfCnpj.ToString().Left(8) != ObterParticipante(pedidoXmlNotaFiscal.XmlNotaFiscal, tipoParticipante)?.CpfCnpj.ToString().Left(8))
                                Adicionar(pedidoXmlNotaFiscal, itemNaoConformidade, tipoParticipante);
                            else
                                AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal, itemNaoConformidade, tipoParticipante);
                        }
                    }
                    else if (itemNaoConformidade.TipoRegra == TipoRegraNaoConformidade.ValidarCNPJ)
                    {
                        if (pedidoXmlNotaFiscal.XmlNotaFiscal.TipoOperacaoNFe == TipoOperacaoNotaFiscal.Entrada) //Preciso rever  novamente esta regra
                        {
                            if (!tiposParticipante.Any(x => x == TipoParticipante.Destinatario))
                                continue;

                            foreach (TipoParticipante tipoParticipante in tiposParticipante)
                            {
                                TipoParticipante participanteComparar;

                                if (tipoParticipante == TipoParticipante.Destinatario)
                                    participanteComparar = TipoParticipante.Remetente;
                                else if (tipoParticipante == TipoParticipante.Remetente)
                                    participanteComparar = TipoParticipante.Destinatario;
                                else if (tipoParticipante == TipoParticipante.Recebedor)
                                    participanteComparar = TipoParticipante.Expedidor;
                                else
                                    participanteComparar = TipoParticipante.Recebedor;

                                if (ObterParticipante(pedidoXmlNotaFiscal.CargaPedido, participanteComparar)?.CpfCnpj != ObterParticipante(pedidoXmlNotaFiscal.XmlNotaFiscal, tipoParticipante)?.CpfCnpj)
                                    Adicionar(pedidoXmlNotaFiscal, itemNaoConformidade, tipoParticipante);
                                else
                                    AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal, itemNaoConformidade, tipoParticipante);
                            }
                            continue;
                        }

                        foreach (TipoParticipante tipoParticipante in tiposParticipante)
                        {
                            if (ObterParticipante(pedidoXmlNotaFiscal.CargaPedido, tipoParticipante)?.CpfCnpj != ObterParticipante(pedidoXmlNotaFiscal.XmlNotaFiscal, tipoParticipante)?.CpfCnpj)
                                Adicionar(pedidoXmlNotaFiscal, itemNaoConformidade, tipoParticipante);
                            else
                                AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal, itemNaoConformidade, tipoParticipante);
                        }
                    }
                    else if (itemNaoConformidade.TipoRegra == TipoRegraNaoConformidade.SituacaoCadastral)
                    {
                        foreach (TipoParticipante tipoParticipante in tiposParticipante)
                        {
                            if (!(ObterParticipante(pedidoXmlNotaFiscal.XmlNotaFiscal, tipoParticipante)?.Ativo ?? true))
                                Adicionar(pedidoXmlNotaFiscal, itemNaoConformidade, tipoParticipante);
                            else
                                AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal, itemNaoConformidade, tipoParticipante);
                        }
                    }
                    else if (itemNaoConformidade.TipoRegra == TipoRegraNaoConformidade.EstendidoFilial)
                    {
                        foreach (TipoParticipante tipoParticipante in tiposParticipante)
                        {
                            if (ObterParticipante(pedidoXmlNotaFiscal.XmlNotaFiscal, tipoParticipante)?.CpfCnpj != carga.Filial?.CNPJ.ToDouble())
                                Adicionar(pedidoXmlNotaFiscal, itemNaoConformidade, tipoParticipante);
                            else
                                AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal, itemNaoConformidade, tipoParticipante);
                        }
                    }
                    else if (itemNaoConformidade.TipoRegra == TipoRegraNaoConformidade.Nacionalizacao)
                    {
                        foreach (TipoParticipante tipoParticipante in tiposParticipante)
                        {
                            if (((ObterParticipante(pedidoXmlNotaFiscal.XmlNotaFiscal, tipoParticipante)?.Tipo ?? "E") != "E"))
                                Adicionar(pedidoXmlNotaFiscal, itemNaoConformidade, tipoParticipante);
                            else
                                AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal, itemNaoConformidade, tipoParticipante);
                        }
                    }
                    else if (itemNaoConformidade.TipoRegra == TipoRegraNaoConformidade.TipoTomador)
                    {
                        if (ObterTipoTomador(pedidoXmlNotaFiscal.CargaPedido) != ObterTipoTomador(pedidoXmlNotaFiscal.XmlNotaFiscal))
                            Adicionar(pedidoXmlNotaFiscal, itemNaoConformidade);
                        else
                            AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal, itemNaoConformidade);
                    }
                    else if (itemNaoConformidade.TipoRegra == TipoRegraNaoConformidade.Tomador)
                    {
                        if (ObterTomador(pedidoXmlNotaFiscal.CargaPedido)?.CpfCnpj != ObterTomador(pedidoXmlNotaFiscal.XmlNotaFiscal)?.CpfCnpj)
                            Adicionar(pedidoXmlNotaFiscal, itemNaoConformidade);
                        else
                            AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal, itemNaoConformidade);
                    }
                    else if (itemNaoConformidade.TipoRegra == TipoRegraNaoConformidade.LocalEntrega)
                    {
                        if(pedidoXmlNotaFiscal.CargaPedido.Recebedor == null && pedidoXmlNotaFiscal.XmlNotaFiscal.Recebedor == null)
                        {
                            if ((pedidoXmlNotaFiscal.CargaPedido.Recebedor == null || pedidoXmlNotaFiscal.XmlNotaFiscal.Recebedor == null) 
                                || (pedidoXmlNotaFiscal.CargaPedido.Recebedor.CpfCnpj != pedidoXmlNotaFiscal.XmlNotaFiscal.Recebedor.CpfCnpj))
                                Adicionar(pedidoXmlNotaFiscal, itemNaoConformidade);
                            else
                                AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal, itemNaoConformidade);
                        }
                    }
                    else if (itemNaoConformidade.TipoRegra == TipoRegraNaoConformidade.RecebedorArmazenagem)
                    {
                        if (pedidoXmlNotaFiscal.CargaPedido.Recebedor?.CpfCnpj != pedidoXmlNotaFiscal.XmlNotaFiscal.Recebedor?.CpfCnpj)
                            Adicionar(pedidoXmlNotaFiscal, itemNaoConformidade);
                        else
                            AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal, itemNaoConformidade);
                    }
                    else if (itemNaoConformidade.TipoRegra == TipoRegraNaoConformidade.Transportador) //Valida onde faz a verificação do transportador da nota e se impede que ele seja inserida
                    {
                        if (carga.Empresa?.Codigo != pedidoXmlNotaFiscal.XmlNotaFiscal.Empresa?.Codigo)
                            Adicionar(pedidoXmlNotaFiscal, itemNaoConformidade);
                        else
                            AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal, itemNaoConformidade);
                    }
                    else if (itemNaoConformidade.TipoRegra == TipoRegraNaoConformidade.NumeroPedido)
                    {
                        List<string> numeroPedidos = pedidoXmlNotaFiscal.XmlNotaFiscal.Produtos.Select(x => x.NumeroPedidoCompra).ToList();
                        if (!numeroPedidos.Contains(pedidoXmlNotaFiscal.CargaPedido.Pedido.NumeroOrdem))
                            Adicionar(pedidoXmlNotaFiscal, itemNaoConformidade);
                        else
                            AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal, itemNaoConformidade);
                    }
                    else if (itemNaoConformidade.TipoRegra == TipoRegraNaoConformidade.PesoLiquidoTotal)
                    {
                        if (pedidoXmlNotaFiscal.XmlNotaFiscal.PesoLiquido <= 0m)
                            Adicionar(pedidoXmlNotaFiscal, itemNaoConformidade);
                        else
                            AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal, itemNaoConformidade);
                    }
                    else if (itemNaoConformidade.TipoRegra == TipoRegraNaoConformidade.ProdutoDePara)
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.XmlNotaFiscalProduto> produtosNotaFiscal = pedidoXmlNotaFiscal.XmlNotaFiscal.Produtos.ToList();
                        bool existeProdutoSemDePara = produtosNotaFiscal.Any(produto => !produto.ProdutoEmbarcador.Fornecedores.Any(produtoFornecedor =>
                            produtoFornecedor.Fornecedor?.CpfCnpj == pedidoXmlNotaFiscal.XmlNotaFiscal.Emitente?.CpfCnpj &&
                            produtoFornecedor.CodigoInterno == produto.CodigoProduto
                        ));

                        if (existeProdutoSemDePara)
                            Adicionar(pedidoXmlNotaFiscal, itemNaoConformidade);
                        else
                            AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal, itemNaoConformidade);
                    }
                    else if (itemNaoConformidade.TipoRegra == TipoRegraNaoConformidade.ProdutoSituacao)
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.XmlNotaFiscalProduto> produtosNotaFiscal = pedidoXmlNotaFiscal.XmlNotaFiscal.Produtos.ToList();
                        bool existeProdutoDesativadoParaFilial = produtosNotaFiscal.Any(produto => produto.ProdutoEmbarcador.Filiais.Any(produtoFilial => produtoFilial.Filial.Codigo == carga.Filial?.Codigo && (produtoFilial.FilialSituacao?.Situacao == SituacaoFilial.Descontinuado || produtoFilial.FilialSituacao?.Situacao == SituacaoFilial.DesativadoCriadoComErro)));

                        if (existeProdutoDesativadoParaFilial)
                            Adicionar(pedidoXmlNotaFiscal, itemNaoConformidade);
                        else
                            AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal, itemNaoConformidade);
                    }
                    else if (itemNaoConformidade.TipoRegra == TipoRegraNaoConformidade.ProdutoFilial)
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.XmlNotaFiscalProduto> produtosNotaFiscal = pedidoXmlNotaFiscal.XmlNotaFiscal.Produtos.ToList();
                        bool existeProdutoSemCadastroParaFilial = (carga.Filial != null) && produtosNotaFiscal.Any(produto => !produto.ProdutoEmbarcador.Filiais.Any(produtoFilial => produtoFilial.Filial.Codigo == carga.Filial?.Codigo));

                        if (existeProdutoSemCadastroParaFilial)
                            Adicionar(pedidoXmlNotaFiscal, itemNaoConformidade);
                        else
                            AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal, itemNaoConformidade);
                    }
                    else if (itemNaoConformidade.TipoRegra == TipoRegraNaoConformidade.ProdutoFilialRecebedor)
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.XmlNotaFiscalProduto> produtosNotaFiscal = pedidoXmlNotaFiscal.XmlNotaFiscal.Produtos.ToList();
                        bool existeProdutoSemDeParaFornecedor = produtosNotaFiscal.Any(produto => !produto.ProdutoEmbarcador.Fornecedores.Any(produtoFornecedor =>
                            produtoFornecedor.Filial != null &&
                            produtoFornecedor.Filial?.Codigo == pedidoXmlNotaFiscal.XmlNotaFiscal.Recebedor?.CpfCnpj
                        ));

                        if (existeProdutoSemDeParaFornecedor)
                            Adicionar(pedidoXmlNotaFiscal, itemNaoConformidade);
                        else
                            AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal, itemNaoConformidade);
                    }
                    else if (itemNaoConformidade.TipoRegra == TipoRegraNaoConformidade.StatusSefaz)
                    {
                        var notaFiscal = pedidoXmlNotaFiscal.XmlNotaFiscal;
                        Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Empresa empresa = notaFiscal != null ? notaFiscal.Empresa : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Empresa();
                        Dominio.ObjetosDeValor.Embarcador.Documentos.ConsultaNotaSefaz notaConsultar = new Dominio.ObjetosDeValor.Embarcador.Documentos.ConsultaNotaSefaz()
                        {
                            Chave = notaFiscal.Chave,
                            CNPJ = empresa.CNPJ,
                            CodigoIBGE = (MultiSoftware.NFe.NFeDistribuicaoDFe.DFe.TCodUfIBGE)empresa.CodigoIBGE,
                            TipoAmbiente = (int)empresa.TipoAmbiente,
                            NomeCertificado = empresa.NomeCertificado,
                            SenhaCertificado = empresa.SenhaCertificado,
                            NomeCertificadoKeyVault = empresa.NomeCertificadoKeyVault
                        };

                        if (servicoDocumentoDestinadoEmpresa.ConsultarStatusNotaSefazPorChave(_unitOfWork, notaConsultar) != StatusNotaRetornoSefaz.Autorizado)
                            Adicionar(pedidoXmlNotaFiscal, itemNaoConformidade);
                        else
                            AtualizarParaAjustadaAutomaticamente(pedidoXmlNotaFiscal, itemNaoConformidade);

                    }
                }
            }

            return !repositorioNaoConformidade.ExisteNaoConformidadePendenteComBloqueio(carga.Codigo, carga.TipoOperacao?.Codigo ?? 0);
        }

        public void ValidarAutomaticamente(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.SituacaoCarga != SituacaoCarga.AgNFe)
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            if (!repositorioCargaPedido.VerificarSeTodosPedidosEstaoAutorizadosPorCarga(carga.Codigo))
                return;

            Repositorio.Embarcador.NotaFiscal.NaoConformidade repostiorioNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(_unitOfWork);

            if (!repostiorioNaoConformidade.ExisteNaoConformidadePendente(carga.Codigo))
                return;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;
            carga.ProcessandoDocumentosFiscais = true;

            repositorioCarga.Atualizar(carga);
        }

        public void EnviarNotificacaoMobile(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!(carga.TipoOperacao?.ConfiguracaoCarga?.AtivoModuloNaoConformidades ?? false))
                return;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NaoConformidade repositorioNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(_unitOfWork);

            Servicos.Embarcador.Chamado.NotificacaoMobile servicoNotificacaoMobile = new Servicos.Embarcador.Chamado.NotificacaoMobile(_unitOfWork, _clienteMultisoftware.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repositorioCargaEntregaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscal = repositorioPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
            List<(int CodigoXmlNotaFiscal, SituacaoNaoConformidade Situacao)> situacoesNaoConformidadeNotasFiscal = repositorioNaoConformidade.BuscarSituacoesNotasFiscaisPorCarga(carga.Codigo);

            List<string> numeroNotas = new List<string>();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = cargaEntregaPedidos.Select(o => o.CargaEntrega).Distinct().ToList();
            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = cargaEntregaPedidos.Where(o => o.CargaEntrega.Codigo == cargaEntrega.Codigo).Select(o => o.CargaPedido).Distinct().ToList();
                List<int> codigosCargaPedido = cargaPedidos.Select(c => c.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotaFiscais = pedidoXMLNotaFiscal.Where(o => codigosCargaPedido.Contains(o.CargaPedido.Codigo)).Select(o => o.XMLNotaFiscal).Distinct().ToList();
                List<int> codigosXmlNotaFiscal = xmlNotaFiscais.Select(o => o.Codigo).ToList();
                List<(int CodigoXmlNotaFiscal, SituacaoNaoConformidade Situacao)> situacoesNaoConformidadePorEntrega = situacoesNaoConformidadeNotasFiscal.Where(o => codigosXmlNotaFiscal.Contains(o.CodigoXmlNotaFiscal)).ToList();

                bool todasNotasLiberadas = !situacoesNaoConformidadePorEntrega.Any(o => SituacaoNaoConformidadeHelper.ObterSituacoesPendentes().Contains(o.Situacao));

                if (todasNotasLiberadas)
                {
                    dynamic conteudo = new { CodigoCargaEntrega = cargaEntrega.Codigo };
                    servicoNotificacaoMobile.NotificarMotoristasNaoConformidadeColetaAutorizada(cargaEntrega, carga.Motoristas.ToList(), conteudo);
                }
                else
                    numeroNotas.AddRange(xmlNotaFiscais.Where(o => situacoesNaoConformidadePorEntrega.Where(s => SituacaoNaoConformidadeHelper.ObterSituacoesPendentes().Contains(s.Situacao)).Select(s => s.CodigoXmlNotaFiscal).Contains(o.Codigo)).Select(o => o.Numero.ToString()));
            }

            if (numeroNotas.Count > 0)
            {
                Notificacao.NotificacaoMTrack serNotificacaoMTrack = new Notificacao.NotificacaoMTrack(_unitOfWork);
                serNotificacaoMTrack.NotificarMudancaNaoConformidade(carga.Motoristas.ToList(), AdminMultisoftware.Dominio.Enumeradores.MobileHubs.NaoConformidade, null, string.Join(", ", numeroNotas));
            }
        }

        public void NotificarStatusParaTransportador(Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade)
        {
            NotificarStatusParaTransportador(naoConformidade, false);
        }

        public void NotificarStatusParaTransportador(Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade, bool naoConformidadeCriada)
        {
            if (naoConformidade?.CargaPedido?.Carga?.Empresa == null)
                return;

            Dominio.Entidades.Empresa empresa = naoConformidade.CargaPedido.Carga.Empresa;
            Dominio.Entidades.Embarcador.Cargas.Carga carga = naoConformidade.CargaPedido.Carga;

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Servicos.Embarcador.Notificacao.Notificacao servicoNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe, adminStringConexao: string.Empty);

            List<Dominio.Entidades.Usuario> usuarios = repUsuario.BuscarPorEmpresa(empresa.Codigo, "U");

            foreach (Dominio.Entidades.Usuario usuario in usuarios)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append($"Não conformidade");
                if (naoConformidadeCriada)
                    sb.Append($" gerada para a carga {carga.CodigoCargaEmbarcador}, com o status {naoConformidade.Situacao.ObterDescricao()}");
                else
                    sb.Append($" da carga {carga.CodigoCargaEmbarcador}, atualizada para status {naoConformidade.Situacao.ObterDescricao()}");


                string mensagem = sb.ToString();
                servicoNotificacao.GerarNotificacao(usuario, naoConformidade.Codigo, "NotasFiscais/AutorizacaoNaoConformidade", mensagem, IconesNotificacao.sucesso, TipoNotificacao.naoConformidade, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe, _unitOfWork);

            }
        }

        public void VerificarNaoConformidadesPendentesDeEnvio()
        {
            Repositorio.Embarcador.NotaFiscal.NaoConformidade repNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(_unitOfWork);
            List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade> naoConformidades = repNaoConformidade.ObterPendentesDeEnvio(10);

            if (naoConformidades.Count > 0)
                EnviarEmailIndividual(naoConformidades);
        }

        public void VerificarNaoConformidadesPendentesDeEnvioResumo()
        {
            Repositorio.Embarcador.NotaFiscal.NaoConformidade repNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(_unitOfWork);
            List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade> naoConformidades = repNaoConformidade.ObterPendentesDeEnvio(0);

            if (naoConformidades.Count > 0)
                EnviarEmailResumo(naoConformidades);
        }

        #endregion Métodos Públicos
    }
}
