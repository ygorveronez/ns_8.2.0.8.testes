using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using System.Linq;

namespace Servicos.Embarcador.Pallets
{
    public class DevolucaoPallets
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public DevolucaoPallets(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Private

        private bool IsUtilizarControlePallets()
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorio = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            return repositorio.BuscarConfiguracaoPadrao().UtilizarControlePallets;
        }

        #endregion

        #region Métodos Publicos

        public void Adicionar(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracao = repConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            if ((tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || (configuracao?.PadraoVisualizacaoOperadorLogistico ?? false)) && IsUtilizarControlePallets())
            {
                Repositorio.Embarcador.Pallets.DevolucaoPallet repositorioDevolucaoPallets = new Repositorio.Embarcador.Pallets.DevolucaoPallet(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                EstoquePallet servicoEstoquePallet = new EstoquePallet(_unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in carga.Pedidos.ToList())
                {
                    if (cargaPedido.Pedido.NumeroPaletes > 0)
                    {
                        Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet devolucao = new Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet()
                        {
                            CargaPedido = cargaPedido,
                            Cliente = cargaPedido.Pedido.Remetente,
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.AgEntrega,
                            XMLNotaFiscal = null,
                            Observacao = "",
                            QuantidadePallets = cargaPedido.Pedido.NumeroPaletes,
                            PesoTotalPallets = cargaPedido.Peso,
                            ValorTotalPallets = repositorioPedidoXMLNotaFiscal.BuscarValorTotalPorCargaPedido(cargaPedido.Codigo),
                            Transportador = cargaPedido.Carga.Empresa
                        };

                        repositorioDevolucaoPallets.Inserir(devolucao);

                        Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque = new Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet()
                        {
                            CpfCnpjCliente = cargaPedido.Pedido.Remetente.CPF_CNPJ,
                            CodigoTransportador = devolucao.Transportador.Codigo,
                            DevolucaoPallet = devolucao,
                            Quantidade = devolucao.QuantidadePallets,
                            TipoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento.Automatico,
                            TipoOperacaoMovimentacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.ClienteTransportador,
                            TipoServicoMultisoftware = tipoServicoMultisoftware,
                            CodigoGrupoPessoas = devolucao?.CargaPedido?.Carga?.GrupoPessoaPrincipal?.Codigo ?? 0
                        };

                        servicoEstoquePallet.InserirMovimentacao(dadosMovimentacaoEstoque);
                    }
                }
            }
        }

        public void Atualizar(Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet devolucao, int quantidadePalletsAnterior, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || !IsUtilizarControlePallets())
                throw new ServicoException("Seu ambiente não permite atualizar a devolução.");

            EstoquePallet servicoEstoquePallet = new EstoquePallet(_unitOfWork);

            //Estorna quantidade anterior
            Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosReversaoMovimentacaoEstoque = new Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet()
            {
                CpfCnpjCliente = devolucao.Cliente?.CPF_CNPJ ?? 0d,
                CodigoTransportador = devolucao.Transportador?.Codigo ?? 0,
                DevolucaoPallet = devolucao,
                Quantidade = quantidadePalletsAnterior,
                TipoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento.Automatico,
                TipoOperacaoMovimentacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.TransportadorCliente,
                TipoServicoMultisoftware = tipoServicoMultisoftware,
                CodigoGrupoPessoas = devolucao?.CargaPedido?.Carga?.GrupoPessoaPrincipal?.Codigo ?? 0
            };

            servicoEstoquePallet.InserirMovimentacao(dadosReversaoMovimentacaoEstoque);

            //Gera a movimentação com a nova quantidade
            Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque = new Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet()
            {
                CpfCnpjCliente = devolucao.Cliente?.CPF_CNPJ ?? 0d,
                CodigoTransportador = devolucao.Transportador?.Codigo ?? 0,
                DevolucaoPallet = devolucao,
                Quantidade = devolucao.QuantidadePallets,
                TipoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento.Automatico,
                TipoOperacaoMovimentacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.ClienteTransportador,
                TipoServicoMultisoftware = tipoServicoMultisoftware,
                CodigoGrupoPessoas = devolucao?.CargaPedido?.Carga?.GrupoPessoaPrincipal?.Codigo ?? 0
            };

            servicoEstoquePallet.InserirMovimentacao(dadosMovimentacaoEstoque);
        }

        public void AdicionarPallets(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Empresa transportador, int quantidade, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (!IsUtilizarControlePallets())
                return;

            Repositorio.Embarcador.Pallets.DevolucaoPallet repositorioDevolucaoPallets = new Repositorio.Embarcador.Pallets.DevolucaoPallet(_unitOfWork);
            Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet devolucao = repositorioDevolucaoPallets.BuscarPorCargaPedidoEXMLNotaFiscal(cargaPedido?.Codigo ?? 0, xmlNotaFiscal.Codigo);

            if (devolucao != null)
            {
                if (devolucao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.Cancelado)
                    return;
                else
                {
                    devolucao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.AgEntrega;
                    repositorioDevolucaoPallets.Atualizar(devolucao);
                }
            }
            else
            {
                devolucao = new Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet()
                {
                    CargaPedido = cargaPedido,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.AgEntrega,
                    XMLNotaFiscal = xmlNotaFiscal,
                    Observacao = "",
                    QuantidadePallets = quantidade,
                    PesoTotalPallets = xmlNotaFiscal.Peso,
                    ValorTotalPallets = (xmlNotaFiscal.Valor - xmlNotaFiscal.ValorST),
                    Transportador = transportador
                };

                repositorioDevolucaoPallets.Inserir(devolucao);
            }

            EstoquePallet servicoEstoquePallet = new EstoquePallet(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque = new Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet()
            {
                CodigoFilial = filial?.Codigo ?? 0,
                CodigoTransportador = transportador.Codigo,
                DevolucaoPallet = devolucao,
                Quantidade = quantidade,
                TipoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento.Automatico,
                TipoOperacaoMovimentacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.FilialTransportador,
                CodigoGrupoPessoas = devolucao.CargaPedido?.Carga?.GrupoPessoaPrincipal?.Codigo ?? 0
            };

            servicoEstoquePallet.InserirMovimentacao(dadosMovimentacaoEstoque);
        }

        public void CancelarPallets(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var repositorioDevolucaoPallets = new Repositorio.Embarcador.Pallets.DevolucaoPallet(_unitOfWork);
            var devolucoes = repositorioDevolucaoPallets.BuscarPorCarga(carga.Codigo);

            if (devolucoes.Count > 0)
            {
                foreach (var devolucao in devolucoes)
                {
                    devolucao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.Cancelado;

                    repositorioDevolucaoPallets.Atualizar(devolucao);

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet tipoOperacaoMovimentacaoEstoquePallet = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.TransportadorCliente : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.TransportadorFilial;
                    EstoquePallet servicoEstoquePallet = new EstoquePallet(_unitOfWork);
                    Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet dadosMovimentacaoEstoque = new Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet()
                    {
                        CpfCnpjCliente = devolucao.Cliente?.CPF_CNPJ ?? 0d,
                        CodigoFilial = devolucao.CargaPedido.Carga.Filial?.Codigo ?? 0,
                        CodigoTransportador = devolucao.Transportador?.Codigo ?? 0,
                        DevolucaoPallet = devolucao,
                        Quantidade = devolucao.QuantidadePallets,
                        TipoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento.Automatico,
                        TipoOperacaoMovimentacao = tipoOperacaoMovimentacaoEstoquePallet,
                        TipoServicoMultisoftware = tipoServicoMultisoftware,
                        CodigoGrupoPessoas = devolucao?.CargaPedido?.Carga?.GrupoPessoaPrincipal?.Codigo ?? 0
                    };

                    servicoEstoquePallet.InserirMovimentacao(dadosMovimentacaoEstoque);
                }
            }
        }

        public void CancelarPallets(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal)
        {
            var repositorioDevolucaoPallets = new Repositorio.Embarcador.Pallets.DevolucaoPallet(_unitOfWork);
            var filial = xmlNotaFiscal.Filial;

            if (filial == null)
            {
                var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);

                filial = repositorioFilial.BuscarMatriz();
            }

            var devolucao = repositorioDevolucaoPallets.BuscarPorXMLNotaFiscal(xmlNotaFiscal.Codigo);

            devolucao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.Cancelado;

            repositorioDevolucaoPallets.Atualizar(devolucao);

            var servicoEstoquePallet = new EstoquePallet(_unitOfWork);
            var dadosMovimentacaoEstoque = new Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet()
            {
                CodigoFilial = filial?.Codigo ?? 0,
                CodigoTransportador = devolucao.Transportador.Codigo,
                DevolucaoPallet = devolucao,
                Quantidade = devolucao.QuantidadePallets,
                TipoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento.Automatico,
                TipoOperacaoMovimentacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.TransportadorFilial,
                CodigoGrupoPessoas = devolucao.CargaPedido?.Carga?.GrupoPessoaPrincipal?.Codigo ?? 0
            };

            servicoEstoquePallet.InserirMovimentacao(dadosMovimentacaoEstoque);
        }

        public byte[] GerarComprovanteEntrega(int codigoDevolucao, out string mensagemErro)
        {
            var result = ReportRequest.WithType(ReportType.ComprovanteEntrega)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoDevolucao", codigoDevolucao.ToString())
                .CallReport();

            mensagemErro = result.ErrorMessage;
            return result.GetContentFile();
        }

        #endregion
    }
}
