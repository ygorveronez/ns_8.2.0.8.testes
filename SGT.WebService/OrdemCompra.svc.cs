using Dominio.Excecoes.Embarcador;
using CoreWCF;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class OrdemCompra(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IOrdemCompra
    {
        public Retorno<bool> AdicionarOrdemCompra(Dominio.ObjetosDeValor.WebService.OrdemCompra.AdicionarOrdemCompra adicionarOrdemCompra)
        {
            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            ValidarToken();

            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = new Dominio.Entidades.Embarcador.Compras.OrdemCompra();

                string mensagemErro = string.Empty;

                // Preenche entidade com dados
                if (!PreencheEntidade(ref ordemCompra, adicionarOrdemCompra, out mensagemErro, unitOfWork))
                    return Retorno<bool>.CriarRetornoDadosInvalidos(mensagemErro);

                // Persiste dados
                unitOfWork.Start();

                repOrdemCompra.Inserir(ordemCompra, Auditado);

                if (!SalvarMercadorias(ordemCompra, adicionarOrdemCompra, out mensagemErro, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return Retorno<bool>.CriarRetornoDadosInvalidos(mensagemErro);
                }

                Servicos.Auditoria.Auditoria.Auditar(this.Auditado, ordemCompra, "Recebeu ordem de compra via webservice", unitOfWork);
                
                unitOfWork.CommitChanges();

                Servicos.Embarcador.Compras.OrdemCompra.EtapaAprovacao(ref ordemCompra, unitOfWork, TipoServicoMultisoftware, Conexao.createInstance(_serviceProvider).StringConexao, Auditado, 0);

                repOrdemCompra.Atualizar(ordemCompra);

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (ImportacaoException ex)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoExcecao(ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao adicionar a ordem de compra!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceOrdemCompra;
        }

        #endregion

        #region Métodos privados 

        private bool PreencheEntidade(ref Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra, Dominio.ObjetosDeValor.WebService.OrdemCompra.AdicionarOrdemCompra adicionarOrdemCompra, out string mensagemErro, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);
            Repositorio.Embarcador.Compras.MotivoCompra repMotivoCompra = new Repositorio.Embarcador.Compras.MotivoCompra(unitOfWork);

            mensagemErro = string.Empty;

            if (string.IsNullOrEmpty(adicionarOrdemCompra.CnpjCpfFornecedor))
            {
                mensagemErro = "Fornecedor é obrigatório.";
                return false;
            }

            if (string.IsNullOrEmpty(adicionarOrdemCompra.DataEmissao))
            {
                mensagemErro = "Data de emissão é obrigatória.";
                return false;
            }

            if (string.IsNullOrEmpty(adicionarOrdemCompra.DataPrevisaoRetorno))
            {
                mensagemErro = "Data Previsão é obrigatória.";
                return false;
            }

            double cpfCnpjFornecedor = 0;
            if (!double.TryParse(Utilidades.String.OnlyNumbers(adicionarOrdemCompra.CnpjCpfFornecedor), out cpfCnpjFornecedor))
            {
                mensagemErro = "CnpjCpfFornecedor inválido.";
                return false;
            }

            double cpfCnpjTransportador = 0;
            if (!double.TryParse(Utilidades.String.OnlyNumbers(adicionarOrdemCompra.CnpjCpfTransportador), out cpfCnpjTransportador))
            {
                mensagemErro = "CnpjCpfTransportador inválido.";
                return false;
            }

            // Vincula dados
            ordemCompra.Fornecedor = repCliente.BuscarPorCPFCNPJ(cpfCnpjFornecedor);

            if (ordemCompra.Fornecedor == null)
            {
                mensagemErro = $"Fornecedor {cpfCnpjFornecedor} não localizado.";
                return false;
            }

            ordemCompra.Transportador = repCliente.BuscarPorCPFCNPJ(cpfCnpjTransportador);

            if (ordemCompra.Transportador == null)
            {
                mensagemErro = $"Transportador {cpfCnpjFornecedor} não localizado.";
                return false;
            }

            if (adicionarOrdemCompra.CodigoIntegracaoMotivoCompra != null)
            {
                ordemCompra.MotivoCompra = repMotivoCompra.BuscarPorCodigoIntegracao(adicionarOrdemCompra.CodigoIntegracaoMotivoCompra);

                if (ordemCompra.MotivoCompra == null)
                {
                    mensagemErro = $"Motivo código integração {adicionarOrdemCompra.CodigoIntegracaoMotivoCompra} não localizado.";
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(adicionarOrdemCompra.PlacaVeiculo))
            {
                ordemCompra.Veiculo = repVeiculo.BuscarPorPlaca(adicionarOrdemCompra.PlacaVeiculo);

                if (ordemCompra.Veiculo == null)
                {
                    mensagemErro = $"Veículo placa {adicionarOrdemCompra.PlacaVeiculo} não localizado.";
                    return false;
                }
            }

            if ((ordemCompra.MotivoCompra?.ExigeInformarVeiculoObrigatoriamente ?? false) && ordemCompra.Veiculo == null)
            {
                mensagemErro = $"É necessário informar um veículo.";
                return false;
            }

            if (!string.IsNullOrEmpty(adicionarOrdemCompra.CpfMotorista))
                ordemCompra.Motorista = repMotorista.BuscarMotoristaPorCPF(0, adicionarOrdemCompra.CpfMotorista);

            DateTime? dataEmissao = adicionarOrdemCompra.DataEmissao.ToNullableDateTime();

            if (!dataEmissao.HasValue)
            {
                mensagemErro = $"A data de emissão não está em um formato correto (dd/MM/yyyy HH:mm:ss).";
                return false;
            }

            DateTime? dataPrevisaoRetorno = adicionarOrdemCompra.DataPrevisaoRetorno.ToNullableDateTime();

            if (!dataPrevisaoRetorno.HasValue)
            {
                mensagemErro = $"A data de emissão não está em um formato correto (dd/MM/yyyy HH:mm:ss).";
                return false;
            }

            if (string.IsNullOrEmpty(adicionarOrdemCompra.CodigoIntegracaoOperador))
            {
                mensagemErro = "Operador é obrigatório.";
                return false;
            }

            ordemCompra.Usuario = repMotorista.BuscarPorCodigoIntegracao(adicionarOrdemCompra.CodigoIntegracaoOperador);

            if (ordemCompra.Usuario == null)
            {
                mensagemErro = $"Operador código integração {adicionarOrdemCompra.CodigoIntegracaoOperador} não localizado.";
                return false;
            }

            ordemCompra.Data = (DateTime)dataEmissao;
            ordemCompra.DataPrevisaoRetorno = (DateTime)dataPrevisaoRetorno;
            ordemCompra.Observacao = adicionarOrdemCompra.Observacao;
            ordemCompra.CondicaoPagamento = adicionarOrdemCompra.CondicaoPagamento;
            ordemCompra.Empresa = ordemCompra.Usuario.Empresa;

            int codigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? ordemCompra.Usuario.Empresa.Codigo : 0;
            ordemCompra.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra.Aberta;
            ordemCompra.Numero = repOrdemCompra.BuscarProximoNumero(codigoEmpresa);

            return true;
        }

        private bool SalvarMercadorias(Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra, Dominio.ObjetosDeValor.WebService.OrdemCompra.AdicionarOrdemCompra adicionarOrdemCompra, out string mensagemErro, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null)
        {
            // Instancia Repositorios
            Repositorio.Embarcador.Compras.OrdemCompraMercadoria repOrdemCompraMercadoria = new Repositorio.Embarcador.Compras.OrdemCompraMercadoria(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            mensagemErro = string.Empty;

            if (adicionarOrdemCompra.Produtos == null)
                return true;

            foreach (var produto in adicionarOrdemCompra.Produtos)
            {
                Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria mercadoria = new Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria();

                if (string.IsNullOrEmpty(produto.CodigoProduto))
                {
                    mensagemErro = "Produto é obrigatório.";
                    return false;
                }
                else
                {
                    mercadoria.Produto = repProduto.BuscarPorCodigoProduto(produto.CodigoProduto);
                }

                if (mercadoria.Produto == null)
                {
                    mensagemErro = $"Produto código {produto.CodigoProduto} não localizado.";
                    return false;
                }

                if (!string.IsNullOrEmpty(produto.PlacaVeiculoMercadoria))
                {
                    mercadoria.VeiculoMercadoria = repVeiculo.BuscarPorPlaca(produto.PlacaVeiculoMercadoria);

                    if (mercadoria.VeiculoMercadoria == null)
                    {
                        mensagemErro = $"Veículo placa {produto.PlacaVeiculoMercadoria} não localizado.";
                        return false;
                    }
                }

                mercadoria.OrdemCompra = ordemCompra;
                mercadoria.Quantidade = produto.Quantidade;
                mercadoria.ValorUnitario = produto.ValorUnitario;

                repOrdemCompraMercadoria.Inserir(mercadoria, auditado);
            }

            return true;
        }

        #endregion
    }
}
