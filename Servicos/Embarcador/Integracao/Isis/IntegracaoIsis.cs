using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Isis
{
    public class IntegracaoIsis
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoIsis(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repositorioCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIsis repIntegracaoIsis = new Repositorio.Embarcador.Configuracoes.IntegracaoIsis(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIsis configuracaoIntegracao = repIntegracaoIsis.Buscar();

            if (!(configuracaoIntegracao?.PossuiIntegracaoFTP ?? false))
            {
                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = "Não foi configurada a integração com a ISIS, por favor verifique.";
                return;
            }

            carregamentoIntegracao.NumeroTentativas += 1;
            carregamentoIntegracao.DataIntegracao = DateTime.Now;

            string csvRequisicao = string.Empty;

            try
            {
                string nomeArquivo = ObterNomeArquivoCarregamento(configuracaoIntegracao.NomenclaturaArquivoCarregamento, carregamentoIntegracao);

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Isis.Carregamento> carregamentos = ObterObjetoCarregamento(carregamentoIntegracao.Carregamento);
                if (carregamentos.Count == 0)
                    throw new ServicoException("Nenhum carregamento encontrado para gerar a planilha!");

                byte[] byteArrayObjeto = Utilidades.CSV.GerarCSV(carregamentos);

                csvRequisicao = System.Text.Encoding.Default.GetString(byteArrayObjeto);
                MemoryStream arquivoEnviar = new MemoryStream(byteArrayObjeto);

                string mensagemErro = string.Empty;
                Servicos.FTP.EnviarArquivo(arquivoEnviar, nomeArquivo, configuracaoIntegracao.EnderecoFTP, configuracaoIntegracao.Porta, configuracaoIntegracao.DiretorioCarregamento, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha, configuracaoIntegracao.Passivo, configuracaoIntegracao.SSL, out mensagemErro, configuracaoIntegracao.UtilizarSFTP);

                if (string.IsNullOrWhiteSpace(mensagemErro))
                {
                    carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    carregamentoIntegracao.ProblemaIntegracao = "Arquivo enviado com sucesso ao FTP.";
                }
                else
                {
                    carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    carregamentoIntegracao.ProblemaIntegracao = mensagemErro;
                }
            }
            catch (ServicoException excecao)
            {
                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a ISIS";
            }

            servicoArquivoTransacao.Adicionar(carregamentoIntegracao, csvRequisicao, string.Empty, "csv");

            repositorioCarregamentoIntegracao.Atualizar(carregamentoIntegracao);
        }

        public void IntegrarCarregamentoLote(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento carregamentoLoteIntegracao)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento repositorioLoteCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIsis repIntegracaoIsis = new Repositorio.Embarcador.Configuracoes.IntegracaoIsis(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIsis configuracaoIntegracao = repIntegracaoIsis.Buscar();

            if (!(configuracaoIntegracao?.PossuiIntegracaoFTP ?? false))
            {
                carregamentoLoteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoLoteIntegracao.ProblemaIntegracao = "Não foi configurada a integração com a ISIS, por favor verifique.";
                return;
            }

            carregamentoLoteIntegracao.NumeroTentativas += 1;
            carregamentoLoteIntegracao.DataIntegracao = DateTime.Now;

            string csvRequisicao = string.Empty;

            try
            {
                string nomeArquivo = ObterNomeArquivoCarregamentoLote(configuracaoIntegracao.NomenclaturaArquivoCarregamento, carregamentoLoteIntegracao);

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Isis.Carregamento> carregamentos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Isis.Carregamento>();
                foreach (var carregamentoIntegracao in carregamentoLoteIntegracao.Carregamentos)
                {
                    var carregamento = ObterObjetoCarregamento(carregamentoIntegracao);
                    carregamentos.AddRange(carregamento);
                }

                if (carregamentos.Count == 0)
                    throw new ServicoException("Nenhum carregamento encontrado para gerar a planilha!");

                byte[] byteArrayObjeto = Utilidades.CSV.GerarCSV(carregamentos);

                csvRequisicao = System.Text.Encoding.Default.GetString(byteArrayObjeto);
                MemoryStream arquivoEnviar = new MemoryStream(byteArrayObjeto);

                string mensagemErro = string.Empty;
                Servicos.FTP.EnviarArquivo(arquivoEnviar, nomeArquivo, configuracaoIntegracao.EnderecoFTP, configuracaoIntegracao.Porta, configuracaoIntegracao.DiretorioCarregamento, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha, configuracaoIntegracao.Passivo, configuracaoIntegracao.SSL, out mensagemErro, configuracaoIntegracao.UtilizarSFTP);

                if (string.IsNullOrWhiteSpace(mensagemErro))
                {
                    carregamentoLoteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    carregamentoLoteIntegracao.ProblemaIntegracao = "Arquivo enviado com sucesso ao FTP.";
                }
                else
                {
                    carregamentoLoteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    carregamentoLoteIntegracao.ProblemaIntegracao = mensagemErro;
                }
            }
            catch (ServicoException excecao)
            {
                carregamentoLoteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoLoteIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                carregamentoLoteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoLoteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a ISIS";
            }

            servicoArquivoTransacao.Adicionar(carregamentoLoteIntegracao, csvRequisicao, string.Empty, "csv");

            repositorioLoteCarregamentoIntegracao.Atualizar(carregamentoLoteIntegracao);
        }

        public void IntegrarChamado(Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao chamadoIntegracao)
        {
            Repositorio.Embarcador.Chamados.ChamadoIntegracao repositorioChamadoIntegracao = new Repositorio.Embarcador.Chamados.ChamadoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIsis repIntegracaoIsis = new Repositorio.Embarcador.Configuracoes.IntegracaoIsis(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIsis configuracaoIntegracao = repIntegracaoIsis.Buscar();

            if (!(configuracaoIntegracao?.PossuiIntegracaoFTP ?? false))
            {
                chamadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                chamadoIntegracao.ProblemaIntegracao = "Não foi configurada a integração com a ISIS, por favor verifique.";
                return;
            }

            chamadoIntegracao.NumeroTentativas += 1;
            chamadoIntegracao.DataIntegracao = DateTime.Now;

            string csvRequisicao = string.Empty;

            try
            {
                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = chamadoIntegracao.Chamado;
                string nomeArquivo = ObterNomeArquivo(configuracaoIntegracao.NomenclaturaArquivo, chamado);
                chamadoIntegracao.NomeArquivo = nomeArquivo;

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Isis.Chamado> objetoChamado = ObterObjetoChamado(chamado);
                if (objetoChamado.Count == 0)
                    throw new ServicoException("Nenhuma nota/produto encontrada para gerar a planilha!");

                byte[] byteArrayObjeto = Utilidades.CSV.GerarCSV(objetoChamado);

                csvRequisicao = System.Text.Encoding.Default.GetString(byteArrayObjeto);
                MemoryStream arquivoEnviar = new MemoryStream(byteArrayObjeto);

                string mensagemErro = string.Empty;
                Servicos.FTP.EnviarArquivo(arquivoEnviar, nomeArquivo, configuracaoIntegracao.EnderecoFTP, configuracaoIntegracao.Porta, configuracaoIntegracao.Diretorio, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha, configuracaoIntegracao.Passivo, configuracaoIntegracao.SSL, out mensagemErro, configuracaoIntegracao.UtilizarSFTP);

                if (string.IsNullOrWhiteSpace(mensagemErro))
                {
                    chamadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    chamadoIntegracao.ProblemaIntegracao = "Arquivo enviado com sucesso ao FTP.";
                }
                else
                {
                    chamadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    chamadoIntegracao.ProblemaIntegracao = mensagemErro;
                }
            }
            catch (ServicoException excecao)
            {
                chamadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                chamadoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                chamadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                chamadoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a ISIS";
            }

            servicoArquivoTransacao.Adicionar(chamadoIntegracao, csvRequisicao, string.Empty, "csv");

            repositorioChamadoIntegracao.Atualizar(chamadoIntegracao);
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Isis.Carregamento> ObterObjetoCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repositorioCarregamentoPedido.BuscarPorCarregamentoSemNotaParcial(carregamento.Codigo);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCarregamento(carregamento.Codigo);

            return (
                from carregamentoPedido in carregamentoPedidos
                select new Dominio.ObjetosDeValor.Embarcador.Integracao.Isis.Carregamento()
                {
                    AnoNota = (carregamentoPedido.Pedido.DataOrder.HasValue ? carregamentoPedido.Pedido.DataOrder.Value : carregamentoPedido.Pedido.DataCriacao.Value).ToString("yyyy"),
                    NumeroPedido = carregamentoPedido.Pedido.NumeroPedidoEmbarcador,
                    DataAlocacao = (carregamentoPedido.Pedido.DataCriacaoPedidoERP.HasValue ? carregamentoPedido.Pedido.DataCriacaoPedidoERP.Value : carregamentoPedido.Pedido.DataCriacao.Value).ToString("yyyyMMdd"),
                    DataFaturamento = (carregamentoPedido.Pedido.DataAlocacaoPedido.HasValue ? carregamentoPedido.Pedido.DataAlocacaoPedido.Value : carregamentoPedido.Pedido.DataCriacao.Value).ToString("yyyyMMdd"),
                    CodigoIntegracaoCliente = carregamentoPedido.Pedido.Destinatario?.CodigoIntegracao ?? string.Empty,
                    CodigoIntegracaoGrupoCliente = carregamentoPedido.Pedido.Destinatario?.GrupoPessoas?.CodigoIntegracao ?? string.Empty,
                    NomeCliente = carregamentoPedido.Pedido?.Destinatario?.Nome ?? string.Empty,
                    CNPJCliente = (carregamentoPedido.Pedido.Destinatario?.CPF_CNPJ ?? 0).ToString() ?? string.Empty,
                    Endereco = carregamentoPedido.Pedido.Destinatario?.Endereco ?? string.Empty,
                    Cidade = carregamentoPedido.Pedido.Destinatario?.Localidade?.Descricao ?? string.Empty,
                    Estado = carregamentoPedido.Pedido.Destinatario?.Localidade?.Estado?.Sigla ?? string.Empty,
                    Cep = carregamentoPedido.Pedido.Destinatario?.CEP ?? string.Empty,
                    CodigoRegiao = carregamentoPedido.Pedido.Adicional1 ?? string.Empty,
                    ObservacaoPedido = carregamentoPedido.Pedido.Observacao ?? string.Empty,
                    DataPrevisaoEntrega = carregamentoPedido.Pedido?.DataPrevisaoChegadaDestinatario?.ToString("yyyyMMdd") ?? string.Empty,
                    NumeroOrdem = carregamentoPedido.Pedido.NumeroOrdem ?? string.Empty,
                    Caixas = carregamentoPedido.Pedido.QtVolumes.ToString("n2"),
                    Volumes = carregamentoPedido.Pedido.CubagemTotal.ToString("n2"),
                    Peso = carregamentoPedido.Pedido.PesoTotal.ToString("n2"),
                    ValorNota = carregamentoPedido.Pedido.ValorTotalNotasFiscais.ToString("n2"),
                    GrossSales = carregamentoPedido.Pedido.GrossSales.ToString("n2"),
                    CarrierCode = carregamento?.Empresa?.CodigoIntegracao ?? string.Empty,
                    TipoVeiculo = carregamento?.ModeloVeicularCarga?.CodigoIntegracao ?? string.Empty,
                    Modal = carregamento?.TipoOperacao?.CodigoIntegracao ?? string.Empty,
                    Etiquetagem = ((carregamento?.Empresa?.ExigeEtiquetagem ?? false) ? true : (carregamentoPedido.Pedido?.Destinatario?.ExigeEtiquetagem ?? false)) ? carregamento?.TipoDeCarga.CodigoTipoCargaEmbarcador : "N",
                    Isca = (carregamento.ExigeIsca ? "Y" : "N"),
                    NroTransporte = carga?.CodigoCargaEmbarcador ?? string.Empty,
                    BulkOfPallet = carregamento?.TipoDeCarga?.CodigoTipoCargaEmbarcador ?? string.Empty,
                    DataExpedicao = (carregamento?.DataCarregamentoCarga ?? DateTime.Now).ToString("yyyyMMdd"),
                    HoraExpedicao = (carregamento?.DataCarregamentoCarga ?? DateTime.Now).ToString("HHmm"),
                    Filial = carregamentoPedido.Pedido?.Filial?.CodigoFilialEmbarcador ?? string.Empty,
                    FiscalFlag = carregamentoPedido.Pedido.Adicional2 ?? string.Empty,
                    Inventario = carregamentoPedido.Pedido.Adicional3 ?? string.Empty,
                    SequenciaVeiculo = carga?.CodigoAlfanumericoEmpresa ?? string.Empty,
                }
            ).ToList();
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Isis.Chamado> ObterObjetoChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repositorioCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repositorioCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Isis.Chamado> listaDados = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Isis.Chamado>();

            // Sim, isso vai ser fixo.
            string cnpjFilial6 = "54558002001363";

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = chamado.CargaEntrega;
            if (cargaEntrega == null)
                return listaDados;

            if (!cargaEntrega.DevolucaoParcial)//Se o "Tipo de Devolução" for Total, lista somente as notas
            {
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = repositorioCargaEntregaNotaFiscal.BuscarPorCargaEntrega(cargaEntrega.Codigo);

                listaDados.AddRange(ObterObjetoChamadoNotasDevolucaoTotal(chamado, cargaEntrega, cargaEntregaNotasFiscais, cnpjFilial6));

                return listaDados;
            }
            else//Sendo Parcial, verifica na lista se possui nota marcada como "Devolução Total" e preenche apenas 1 registro dela
            {
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = repositorioCargaEntregaNotaFiscal.BuscarNotasDevolucaoTotalPorCargaEntrega(cargaEntrega.Codigo, chamado.Codigo);

                listaDados.AddRange(ObterObjetoChamadoNotasDevolucaoTotal(chamado, cargaEntrega, cargaEntregaNotasFiscais, cnpjFilial6));
            }

            //Notas devolvidas parcialmente, envia os dados por produtos
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos = repositorioCargaEntregaProduto.BuscarProdutosDevolvidosPorCargaEntrega(cargaEntrega.Codigo, chamado.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto cargaEntregaProduto in cargaEntregaProdutos)
            {
                string codigoIntegracao = cargaEntrega.MotivoRejeicao?.CodigoIntegracao;
                string cnpj = cargaEntregaProduto.XMLNotaFiscal?.Emitente.CPF_CNPJ_SemFormato ?? string.Empty;
                string returnType = ObterReturnType(cnpjFilial6, cnpj, codigoIntegracao);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Isis.Chamado detalhe = new Dominio.ObjetosDeValor.Embarcador.Integracao.Isis.Chamado()
                {
                    Year = cargaEntregaProduto.XMLNotaFiscal?.DataEmissao.ToString("yyyy") ?? string.Empty,
                    Company = "BRZL",
                    OriginalInvoice = cargaEntregaProduto.XMLNotaFiscal?.Numero.ToString() ?? string.Empty,
                    DocumentoNumber = "",
                    CustomerNumber = chamado.Cliente?.CodigoIntegracao ?? string.Empty,
                    Sku = cargaEntregaProduto.Produto.CodigoProdutoEmbarcador.PadLeft(5),
                    Ammount = cargaEntregaProduto.QuantidadeDevolucao.ToString("n0"),
                    ReturnType = returnType,
                    CarrierInvoiceDT = chamado.DataCriacao.ToDateString(),
                    FreightCarrierCode = chamado.Carga?.Empresa?.CodigoIntegracao ?? string.Empty,
                    PartialFull = cargaEntregaProduto.XMLNotaFiscal?.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.DevolvidaParcial ? "P" : "F",
                    CNPJ = cnpj.ObterSomenteNumeros()
                };

                listaDados.Add(detalhe);
            }

            return listaDados;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Isis.Chamado> ObterObjetoChamadoNotasDevolucaoTotal(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais, string cnpjFilial6)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Isis.Chamado> listaDados = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Isis.Chamado>();

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal in cargaEntregaNotasFiscais)
            {
                string codigoIntegracao = cargaEntrega.MotivoRejeicao?.CodigoIntegracao;
                string cnpj = cargaEntrega.DevolucaoParcial ? cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.CPF_CNPJ_SemFormato : cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.CPF_CNPJ_SemFormato;
                string returnType = ObterReturnType(cnpjFilial6, cnpj, codigoIntegracao);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Isis.Chamado detalhe = new Dominio.ObjetosDeValor.Embarcador.Integracao.Isis.Chamado()
                {
                    Year = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.DataEmissao.ToString("yyyy") ?? chamado.DataCriacao.ToString("yyyy"),
                    Company = "BRZL",
                    OriginalInvoice = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero.ToString(),
                    DocumentoNumber = "",
                    CustomerNumber = chamado.Cliente?.CodigoIntegracao ?? string.Empty,
                    Sku = "",
                    Ammount = "",
                    ReturnType = returnType,
                    CarrierInvoiceDT = chamado.DataCriacao.ToDateString(),
                    FreightCarrierCode = chamado.Carga?.Empresa?.CodigoIntegracao ?? string.Empty,
                    PartialFull = "F",
                    CNPJ = cnpj.ObterSomenteNumeros()
                };

                listaDados.Add(detalhe);
            }

            return listaDados;
        }

        private string ObterReturnType(string cnpjFilial6, string cnpj, string codigoIntegracao)
        {
            string returnType = string.Empty;

            if (!string.IsNullOrEmpty(cnpj) && cnpj.Equals(cnpjFilial6) && !string.IsNullOrWhiteSpace(codigoIntegracao))
                returnType = $"{codigoIntegracao}06";
            else
                returnType = codigoIntegracao ?? string.Empty;

            return returnType;
        }

        private string ObterNomeArquivo(string nomenclaturaConfiguracao, Dominio.Entidades.Embarcador.Chamados.Chamado chamado)
        {
            string nome = nomenclaturaConfiguracao?.Trim() ?? string.Empty;

            nome = nome.Replace("#Dia", chamado.DataCriacao.ToString("dd"))
                        .Replace("#Mes", chamado.DataCriacao.ToString("MM"))
                        .Replace("#Ano", chamado.DataCriacao.ToString("yyyy"))
                        .Replace("#Hora", chamado.DataCriacao.ToString("HH"))
                        .Replace("#Minuto", chamado.DataCriacao.ToString("mm"))
                        .Replace("#Segundo", chamado.DataCriacao.ToString("ss"))
                        .Replace("#NumeroAtendimento", chamado.Numero.ToString());

            return nome + ".csv";
        }

        private string ObterNomeArquivoCarregamento(string nomenclaturaConfiguracao, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao)
        {
            string nome = nomenclaturaConfiguracao?.Trim() ?? string.Empty;

            nome = nome.Replace("#Dia", DateTime.Now.ToString("dd"))
                        .Replace("#Mes", DateTime.Now.ToString("MM"))
                        .Replace("#Ano", DateTime.Now.ToString("yyyy"))
                        .Replace("#Hora", DateTime.Now.ToString("HH"))
                        .Replace("#Minuto", DateTime.Now.ToString("mm"))
                        .Replace("#Segundo", DateTime.Now.ToString("ss"))
                        .Replace("#NumeroCarregamento", carregamentoIntegracao.Carregamento.NumeroCarregamento);
            return nome + ".csv";
        }

        private string ObterNomeArquivoCarregamentoLote(string nomenclaturaConfiguracao, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento lotecarregamentoIntegracao)
        {
            string nome = nomenclaturaConfiguracao?.Trim() ?? string.Empty;

            nome = nome.Replace("#Dia", DateTime.Now.ToString("dd"))
                        .Replace("#Mes", DateTime.Now.ToString("MM"))
                        .Replace("#Ano", DateTime.Now.ToString("yyyy"))
                        .Replace("#Hora", DateTime.Now.ToString("HH"))
                        .Replace("#Minuto", DateTime.Now.ToString("mm"))
                        .Replace("#Segundo", DateTime.Now.ToString("ss"))
                        .Replace("#NumeroCarregamento", lotecarregamentoIntegracao.Carregamentos.Select(x => x.NumeroCarregamento).FirstOrDefault());
            return nome + ".csv";
        }

        #endregion
    }
}
