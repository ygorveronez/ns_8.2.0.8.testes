using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Servicos.Embarcador.NFe
{
    public class NFe : ServicoBase
    {

        #region Construtores        

        public NFe() : base() { }

        public NFe(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public NFe(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public void SalvarProdutosNota(string xml, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, TipoServicoMultisoftware tipoServico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoProduto repProduto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoProduto(unitOfWork);
            Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal servicosPedidos = new Pedido.PedidoXMLNotaFiscal(unitOfWork);
            var configuracao = repProduto.BuscarConfiguracaoPadrao();
            if (configuracao == null || configuracao.SalvarProdutosDaNotaFiscal == false)
                return;

            servicosPedidos.ArmazenarProdutosXML(xml, xMLNotaFiscal, auditado, tipoServico);
        }

        public bool BuscarDadosNotaFiscal(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, System.IO.StreamReader xml, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe notaFiscal = null, bool armazenarXML = true, bool manterCNPJTransportadorXML = false, bool somarProdutosComoPallet = false, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware = null, bool importarEmailCliente = false, bool importarFreteNota = false, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = null, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, bool cadastroAutomaticoPessoaExterior = false)
        {
            xmlNotaFiscal = null;

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

            xml.BaseStream.Position = 0;

            string stringXMLNotaFiscalUTF = xml.ReadToEnd();

            if ((configuracaoGeralCarga?.ConverterXMLNotaFiscalParaByteArrayAoImportarNaCarga ?? false))
            {
                //Favor não alterar código abaixo até o outro comentário, temos alguma linha suspeita que da um erro, se tiver algum problema chame o Leonardo Z. - Tarefa #71840
                //Chame o Leonardo Z. antes de fazer qualquer coisa aqui
                byte[] byteArrayXMLNotaFiscal = Encoding.UTF8.GetBytes(stringXMLNotaFiscalUTF);
                MemoryStream memoryStreamXMLNotaFiscal = new MemoryStream(byteArrayXMLNotaFiscal);
                StreamWriter streamWriterXMLNotaFiscal = new StreamWriter(memoryStreamXMLNotaFiscal, Encoding.Default);
                StreamReader streamReaderXMLNotaFiscal = new StreamReader(streamWriterXMLNotaFiscal.BaseStream);
                string stringXMLNotaFiscalANSI = streamReaderXMLNotaFiscal.ReadToEnd();

                string xmlNotaFiscalANSIValidado = Utilidades.String.RemoveSpecifiedStringAndReplace("ï»¿", stringXMLNotaFiscalANSI, "");

                byte[] byteArrayXMLNotaFiscalUTF = Encoding.UTF8.GetBytes(xmlNotaFiscalANSIValidado);
                MemoryStream memoryStreamXMLNotaFiscalUTF = new MemoryStream(byteArrayXMLNotaFiscalUTF);
                StreamWriter streamWriterXMLNotaFiscalUTF = new StreamWriter(memoryStreamXMLNotaFiscalUTF, Encoding.UTF8);
                StreamReader streamReaderXMLNotaFiscalUTF = new StreamReader(streamWriterXMLNotaFiscalUTF.BaseStream);

                stringXMLNotaFiscalUTF = streamReaderXMLNotaFiscalUTF.ReadToEnd();
                //Fim alteração de correção XMLNotaFiscal
            }

            if (notaFiscal == null)
            {
                Servicos.NFe serNFe = new Servicos.NFe(unitOfWork, auditado);

                xml.BaseStream.Position = 0;

                notaFiscal = serNFe.ObterDocumentoPorXML(xml.BaseStream, unitOfWork, true, importarEmailCliente, cargaPedido);
            }

            xml.Dispose();
            xml.Close();

            Dominio.Entidades.Cliente destinatario = null;
            Dominio.ObjetosDeValor.Cliente clienteExportacao = notaFiscal.DestinatarioExportacao;
            if (clienteExportacao != null)
            {
                destinatario = repCliente.BuscarPorNomeEndereco(clienteExportacao.RazaoSocial, clienteExportacao.Endereco);

                if (destinatario == null)
                {
                    Repositorio.Embarcador.Pessoas.PessoaExteriorOutraDescricao repPessoaExteriorOutraDescricao = new Repositorio.Embarcador.Pessoas.PessoaExteriorOutraDescricao(unitOfWork);

                    destinatario = repPessoaExteriorOutraDescricao.BuscarPessoaPorRazaoSocialEEndereco(clienteExportacao.RazaoSocial, clienteExportacao.Endereco);
                }

                if (destinatario == null && !cadastroAutomaticoPessoaExterior)
                {
                    erro = $"Não foi encontrada uma pessoa do exterior com o nome {clienteExportacao.RazaoSocial} e o endereço {clienteExportacao.Endereco}.";
                    return false;
                }

                if (destinatario == null)
                {
                    Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
                    Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                    Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);

                    destinatario = new Dominio.Entidades.Cliente();
                    destinatario.Nome = clienteExportacao.RazaoSocial;
                    destinatario.Endereco = clienteExportacao.Endereco;
                    destinatario.Bairro = clienteExportacao.Bairro;
                    destinatario.Cidade = clienteExportacao.Cidade;
                    destinatario.Complemento = clienteExportacao.Complemento;
                    destinatario.Email = clienteExportacao.Emails;
                    destinatario.Numero = clienteExportacao.Numero;
                    destinatario.Cidade = clienteExportacao.Cidade;
                    destinatario.Pais = repPais.BuscarPorSigla(clienteExportacao.SiglaPais);
                    destinatario.Tipo = "E";
                    destinatario.CPF_CNPJ = repCliente.BuscarPorProximoExterior();
                    destinatario.Localidade = repLocalidade.BuscarPorCodigoIBGE(9999999);
                    destinatario.Atividade = repAtividade.BuscarPorCodigo(1);
                    destinatario.Ativo = true;
                    repCliente.Inserir(destinatario);
                }
            }
            else
            {
                string cgcDestinatario = notaFiscal.Destinatario;
                if (!string.IsNullOrWhiteSpace(cgcDestinatario))
                {
                    if (cgcDestinatario == "00.000.000/0000-00")
                    {
                        //destinatario = repCliente.BuscarPorNomeEndereco(clienteExportacao.RazaoSocial, clienteExportacao.Endereco);
                        //if (destinatario == null)
                        //{
                        destinatario = new Dominio.Entidades.Cliente();
                        destinatario.Tipo = "E";
                        //}
                    }
                    else
                    {
                        destinatario = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(cgcDestinatario)));
                    }
                }
            }

            List<string> cfops = new List<string>();
            List<string> ncms = new List<string>();
            int quantidadePallets = 0;

            foreach (var prod in notaFiscal.Produtos)
            {
                if (somarProdutosComoPallet)
                    quantidadePallets += (int)prod.QuantidadeComercial;

                if (prod.CFOP > 0)
                    cfops.Add(prod.CFOP.ToString());

                if (!string.IsNullOrEmpty(prod.NCM) && prod.NCM != "00000000")
                    ncms.Add(prod.NCM);
            }

            string cfopPrincipal = "";
            string ncmPrincipal = "";

            if (cfops != null && cfops.Count > 0)
                cfopPrincipal = RetornaRegistroComMaiorQuantidade(cfops);

            if (ncms != null && ncms.Count > 0)
            {
                ncmPrincipal = RetornaRegistroComMaiorQuantidade(ncms);

                if (ncmPrincipal.Length > 4)
                    ncmPrincipal = ncmPrincipal.Substring(0, 4);
            }

            if (!string.IsNullOrWhiteSpace(notaFiscal.Chave))
                xmlNotaFiscal = repXmlNotaFiscal.BuscarPorChave(notaFiscal.Chave);

            if (xmlNotaFiscal == null)
                xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();
            else
                xmlNotaFiscal.NotaJaEstavaNaBase = true;

            xmlNotaFiscal.CFOP = cfopPrincipal;

            if (!string.IsNullOrWhiteSpace(ncmPrincipal))
                xmlNotaFiscal.NCM = ncmPrincipal;

            xmlNotaFiscal.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe;
            xmlNotaFiscal.XML = armazenarXML ? stringXMLNotaFiscalUTF : "";
            xmlNotaFiscal.Chave = notaFiscal.Chave;
            xmlNotaFiscal.TipoEmissao = Utilidades.Chave.ObterTipoEmissao(xmlNotaFiscal.Chave).ToString().ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoNotaFiscal>();
            xmlNotaFiscal.Numero = int.Parse(notaFiscal.Numero);
            xmlNotaFiscal.Modelo = notaFiscal.Modelo;
            xmlNotaFiscal.Serie = notaFiscal.Serie;
            xmlNotaFiscal.Valor = notaFiscal.ValorTotal;
            xmlNotaFiscal.BaseCalculoICMS = notaFiscal.BaseCalculoICMS;
            xmlNotaFiscal.ValorICMS = notaFiscal.ValorICMS;
            xmlNotaFiscal.BaseCalculoST = notaFiscal.BaseCalculoST;
            xmlNotaFiscal.ValorST = notaFiscal.ValorST;
            xmlNotaFiscal.ValorTotalProdutos = notaFiscal.ValorTotalProdutos;
            xmlNotaFiscal.ValorSeguro = notaFiscal.ValorSeguro;
            xmlNotaFiscal.ValorDesconto = notaFiscal.ValorDesconto;
            xmlNotaFiscal.ValorImpostoImportacao = notaFiscal.ValorImpostoImportacao;
            xmlNotaFiscal.ValorIPI = notaFiscal.ValorIPI;
            xmlNotaFiscal.ValorPIS = notaFiscal.ValorPIS;
            xmlNotaFiscal.ValorCOFINS = notaFiscal.ValorCOFINS;
            xmlNotaFiscal.ValorOutros = notaFiscal.ValorOutros;
            xmlNotaFiscal.CSTIBSCBS = notaFiscal.CSTIBSCBS;
            xmlNotaFiscal.ClassificacaoTributariaIBSCBS = notaFiscal.ClassificacaoTributariaIBSCBS;
            xmlNotaFiscal.BaseCalculoIBSCBS = notaFiscal.BaseCalculoIBSCBS;
            xmlNotaFiscal.AliquotaIBSEstadual = notaFiscal.AliquotaIBSEstadual;
            xmlNotaFiscal.PercentualReducaoIBSEstadual = notaFiscal.PercentualReducaoIBSEstadual;
            xmlNotaFiscal.ValorReducaoIBSEstadual = notaFiscal.ValorReducaoIBSEstadual;
            xmlNotaFiscal.ValorIBSEstadual = notaFiscal.ValorIBSEstadual;
            xmlNotaFiscal.AliquotaIBSMunicipal = notaFiscal.AliquotaIBSMunicipal;
            xmlNotaFiscal.PercentualReducaoIBSMunicipal = notaFiscal.PercentualReducaoIBSMunicipal;
            xmlNotaFiscal.ValorReducaoIBSMunicipal = notaFiscal.ValorReducaoIBSMunicipal;
            xmlNotaFiscal.ValorIBSMunicipal = notaFiscal.ValorIBSMunicipal;
            xmlNotaFiscal.AliquotaCBS = notaFiscal.AliquotaCBS;
            xmlNotaFiscal.PercentualReducaoCBS = notaFiscal.PercentualReducaoCBS;
            xmlNotaFiscal.ValorReducaoCBS = notaFiscal.ValorReducaoCBS;
            xmlNotaFiscal.ValorCBS = notaFiscal.ValorCBS;
            if (importarFreteNota)
                xmlNotaFiscal.ValorFrete = notaFiscal.ValorFrete;
            xmlNotaFiscal.Peso = notaFiscal.Peso;
            xmlNotaFiscal.PesoBaseParaCalculo = xmlNotaFiscal.Peso;
            xmlNotaFiscal.PesoLiquido = notaFiscal.PesoLiquido;
            xmlNotaFiscal.Volumes = (int)notaFiscal.Volume;
            xmlNotaFiscal.NaturezaOP = notaFiscal.NaturezaOP;
            xmlNotaFiscal.NomeDestinatario = Utilidades.String.Left(notaFiscal.PessoaDestinatario?.RazaoSocial, 150);
            xmlNotaFiscal.IEDestinatario = notaFiscal.PessoaDestinatario?.RGIE ?? "";
            xmlNotaFiscal.IERemetente = notaFiscal.PessoaRemetente?.RGIE ?? "";
            xmlNotaFiscal.Especie = notaFiscal.Especie;
            xmlNotaFiscal.ValorLiquido = notaFiscal.ValorLiquido;
            xmlNotaFiscal.NumeroDaFatura = notaFiscal.NumeroDaFatura;
            xmlNotaFiscal.DataRecebimento = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(notaFiscal.Empresa))
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                xmlNotaFiscal.CNPJTranposrtador = notaFiscal.Empresa;
                xmlNotaFiscal.Empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(notaFiscal.Empresa));
            }
            else
            {
                if (!manterCNPJTransportadorXML)
                    xmlNotaFiscal.CNPJTranposrtador = "";
                else
                    xmlNotaFiscal.CNPJTranposrtador = (string)notaFiscal.CNPJTransportador;
            }

            if (notaFiscal.Retirada != null)
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Dominio.Entidades.Localidade localidade = new Dominio.Entidades.Localidade();

                if (!string.IsNullOrWhiteSpace(notaFiscal.Retirada.CEP))
                    localidade = repLocalidade.BuscarPorCEP(notaFiscal.Retirada.CEP.ObterSomenteNumeros());

                if (localidade == null && !string.IsNullOrWhiteSpace(notaFiscal.Retirada.Municipio) && !string.IsNullOrWhiteSpace(notaFiscal.Retirada.UF.ToString("D")))
                    localidade = repLocalidade.BuscarPorCidadeUF(notaFiscal.Retirada.Municipio, notaFiscal.Retirada.UF.ToString("D"));

                if (localidade == null && !string.IsNullOrWhiteSpace(notaFiscal.Retirada.Municipio) && !string.IsNullOrWhiteSpace(notaFiscal.Retirada.UF.ToString("D")))
                    localidade = repLocalidade.BuscarPorDescricaoEUF(notaFiscal.Retirada.Municipio, notaFiscal.Retirada.UF.ToString("D"));

                if (localidade != null && cargaPedido != null)
                {
                    double.TryParse(notaFiscal.Retirada.CNPJ, out double cnpj);

                    xmlNotaFiscal.Expedidor = repCliente.BuscarPorCPFCNPJ(cnpj);

                    if (xmlNotaFiscal.Expedidor == null)
                    {
                        Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                        Dominio.Entidades.Cliente expedidor = new Dominio.Entidades.Cliente
                        {
                            CPF_CNPJ = double.Parse(notaFiscal.Retirada.CNPJ),
                            Nome = notaFiscal.Retirada.Nome,
                            Endereco = notaFiscal.Retirada.Logradouro,
                            Bairro = notaFiscal.Retirada.Bairro,
                            Numero = notaFiscal.Retirada.Numero,
                            Cidade = notaFiscal.Retirada.Municipio,
                            Tipo = cnpj.ToString().Length == 14 ? "J" : "F",
                            Localidade = localidade,
                            Pais = localidade?.Pais ?? null,
                            Telefone1 = notaFiscal.Retirada.Telefone?.ObterSomenteNumeros(),
                            Atividade = svcCTe.ObterAtividade(xmlNotaFiscal.Empresa?.Codigo ?? 0, unitOfWork),
                            Ativo = true,
                        };

                        repCliente.Inserir(expedidor);
                        xmlNotaFiscal.Expedidor = expedidor;
                    }
                }
            }

            if (notaFiscal.Produtos?.Count > 1)
                xmlNotaFiscal.Produto = "Diversos";
            else
                xmlNotaFiscal.Produto = notaFiscal.Produtos.FirstOrDefault()?.Descricao?.Left(150) ?? null;

            if ((int)notaFiscal.ModalidadeFrete == 3)
                xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Outros;
            else
                xmlNotaFiscal.ModalidadeFrete = notaFiscal.ModalidadeFrete;

            xmlNotaFiscal.NumeroDT = notaFiscal.XPedido;
            xmlNotaFiscal.QuantidadePallets = quantidadePallets;
            xmlNotaFiscal.Observacao = notaFiscal.Observacao;

            if (notaFiscal.Produtos.Count > 0 && !string.IsNullOrWhiteSpace(notaFiscal.Produtos.FirstOrDefault().NumeroPedidoCompra))
                xmlNotaFiscal.NumeroOrdemPedidoIntegracaoUnilever = notaFiscal.Produtos.FirstOrDefault().NumeroPedidoCompra;

            if (!string.IsNullOrWhiteSpace(notaFiscal.Placa))
                xmlNotaFiscal.PlacaVeiculoNotaFiscal = notaFiscal.Placa;
            else
                xmlNotaFiscal.PlacaVeiculoNotaFiscal = "";

            xmlNotaFiscal.TipoOperacaoNotaFiscal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal)notaFiscal.TipoOperacao;

            DateTime dataEmissao;
            if (!DateTime.TryParseExact(notaFiscal.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
            {
                if (!DateTime.TryParseExact(notaFiscal.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                    dataEmissao = DateTime.Now;
            }

            xmlNotaFiscal.DataEmissao = dataEmissao;
            xmlNotaFiscal.nfAtiva = true;

            xmlNotaFiscal.Emitente = notaFiscal.Remetente;
            xmlNotaFiscal.Destinatario = destinatario;

            double.TryParse(Utilidades.String.OnlyNumbers((string)notaFiscal.LocalEntrega), out double cpfCnpjLocalEntrega);

            if (cpfCnpjLocalEntrega > 0D && xmlNotaFiscal.Destinatario?.CPF_CNPJ != cpfCnpjLocalEntrega)
                xmlNotaFiscal.Recebedor = repCliente.BuscarPorCPFCNPJ(cpfCnpjLocalEntrega);

            if (xmlNotaFiscal.Emitente != null)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoaEmitente = repGrupoPessoas.BuscarGrupoCliente(xmlNotaFiscal.Emitente.CPF_CNPJ);
                if (grupoPessoaEmitente != null)
                {
                    //Busca a observação do XML pois o objeto xmlNotaFiscal.Observacao vem com caracteres inválidos
                    XDocument documentoXml = XDocument.Parse(stringXMLNotaFiscalUTF);
                    string observacaoNfe = (from XElement elemento in documentoXml.Descendants() where elemento.Name.LocalName == "infCpl" select elemento)?.FirstOrDefault()?.Value ?? "";

                    Repositorio.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml repGrupoPessoasLeituraDinamicaXml = new Repositorio.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml> listaGrupoPessoasLeituraDinamicaXml = repGrupoPessoasLeituraDinamicaXml.BuscarPorGrupoPessoas(grupoPessoaEmitente.Codigo);
                    List<Dominio.ObjetosDeValor.Embarcador.NFe.Observacao> observacoes = null;
                    List<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml> listaLeituraDinamicaXml = null;

                    if (listaGrupoPessoasLeituraDinamicaXml?.Count() > 0)
                    {
                        observacoes = (List<Dominio.ObjetosDeValor.Embarcador.NFe.Observacao>)notaFiscal.ObservacaoContribuinte;
                        listaLeituraDinamicaXml = BuscarValoresLeituraDinamicaXML(listaGrupoPessoasLeituraDinamicaXml, notaFiscal, observacaoNfe);
                    }

                    if (grupoPessoaEmitente.LerNumeroPedidoObservacaoContribuinteNota && !string.IsNullOrWhiteSpace(grupoPessoaEmitente.IdentificadorNumeroPedidoObservacaoContribuinteNota) && notaFiscal.ObservacaoContribuinte != null)
                    {
                        observacoes = (List<Dominio.ObjetosDeValor.Embarcador.NFe.Observacao>)notaFiscal.ObservacaoContribuinte;

                        xmlNotaFiscal.NumeroDT = observacoes.Where(o => o.xCampo == grupoPessoaEmitente.IdentificadorNumeroPedidoObservacaoContribuinteNota).Select(o => o.xTexto).FirstOrDefault();

                        if (int.TryParse(xmlNotaFiscal.NumeroDT, out int numeroPedido))
                            xmlNotaFiscal.NumeroPedido = numeroPedido;
                    }
                    else if (grupoPessoaEmitente.LerNumeroPedidoDaObservacaoDaNota && !string.IsNullOrWhiteSpace(observacaoNfe)) //todo: pegar numero DT, está temparario para notas NATURAL ONE DANONE
                    {
                        if (!string.IsNullOrWhiteSpace(grupoPessoaEmitente.LerNumeroPedidoDaObservacaoDaNotaInicio) || !string.IsNullOrWhiteSpace(grupoPessoaEmitente.LerNumeroPedidoDaObservacaoDaNotaFim))
                        {
                            string observacao = observacaoNfe.ToLower();

                            if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                            {
                                string strInicio = grupoPessoaEmitente.LerNumeroPedidoDaObservacaoDaNotaInicio.ToLower();
                                string strFim = grupoPessoaEmitente.LerNumeroPedidoDaObservacaoDaNotaFim.ToLower();

                                int idxInicio = observacao.IndexOf(strInicio);

                                if (idxInicio > -1)
                                {
                                    idxInicio += strInicio.Length;

                                    int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                    if (idxFim > 0)
                                    {
                                        string numeroPedido = Utilidades.String.OnlyNumbersAndChars(observacao.Substring(idxInicio, idxFim - idxInicio));

                                        if (!string.IsNullOrWhiteSpace(numeroPedido) && numeroPedido.Length > 0)
                                            xmlNotaFiscal.NumeroDT = numeroPedido;
                                    }
                                }
                            }
                        }
                        else
                        {
                            string observacao = observacaoNfe;
                            if (grupoPessoaEmitente.TipoLeituraNumeroCargaNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLeituraNumeroCargaNotaFiscal.Carrefour)
                            {
                                string[] romanovski = new string[] { "Romaneio: " };
                                string[] splitOBS = observacao.Split(romanovski, StringSplitOptions.RemoveEmptyEntries);

                                if (splitOBS.Length > 1)
                                {
                                    string[] splitPedido = splitOBS[1].Split(' ');
                                    if (splitPedido.Length > 0)
                                    {
                                        string pedido = Utilidades.String.RemoveAllSpecialCharacters(splitPedido[0].Trim().Replace(";", ""));
                                        xmlNotaFiscal.NumeroDT = pedido;
                                        int numeroPedido = 0;
                                        int.TryParse(xmlNotaFiscal.NumeroDT, out numeroPedido);
                                        xmlNotaFiscal.NumeroPedido = numeroPedido;
                                    }
                                }
                            }
                            else
                            {
                                string[] splitOBS = observacao.Split(',');

                                if (splitOBS.Length > 1)
                                {
                                    string[] splitPedido = splitOBS[1].Split(':');
                                    if (splitPedido.Length > 0)
                                    {
                                        string pedido = splitPedido[1].Trim();
                                        xmlNotaFiscal.NumeroDT = pedido;
                                        int numeroPedido = 0;
                                        int.TryParse(xmlNotaFiscal.NumeroDT, out numeroPedido);

                                        xmlNotaFiscal.NumeroPedido = numeroPedido;
                                    }
                                }
                            }
                        }
                    }

                    if (grupoPessoaEmitente.LerPlacaDaObservacaoDaNota && string.IsNullOrWhiteSpace(xmlNotaFiscal.PlacaVeiculoNotaFiscal))
                    {
                        string observacao = observacaoNfe.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = grupoPessoaEmitente.LerPlacaDaObservacaoDaNotaInicio.ToLower();
                            string strFim = grupoPessoaEmitente.LerPlacaDaObservacaoDaNotaFim.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                if (idxFim > 0)
                                {
                                    string placa = Utilidades.String.OnlyNumbersAndChars(observacao.Substring(idxInicio, idxFim - idxInicio));

                                    if (!string.IsNullOrWhiteSpace(placa) && placa.Length == 7)
                                        xmlNotaFiscal.PlacaVeiculoNotaFiscal = placa.ToUpper();
                                }
                            }
                        }
                    }

                    if (grupoPessoaEmitente.LerPlacaDaObservacaoContribuinteDaNota && string.IsNullOrWhiteSpace(xmlNotaFiscal.PlacaVeiculoNotaFiscal) && notaFiscal.ObservacaoContribuinte != null)
                    {
                        string placa = Utilidades.String.OnlyNumbersAndChars(notaFiscal.ObservacaoContribuinte.Where(o => o.xCampo.ToLower().Equals(grupoPessoaEmitente.LerPlacaDaObservacaoContribuinteDaNotaIdentificacao.ToLower())).Select(o => o.xTexto).FirstOrDefault());

                        if (!string.IsNullOrWhiteSpace(placa) && placa.Length == 7)
                            xmlNotaFiscal.PlacaVeiculoNotaFiscal = placa.ToUpper();
                    }

                    if (!string.IsNullOrWhiteSpace(grupoPessoaEmitente.ExpressaoBooking))
                    {
                        string observacaoBooking = observacaoNfe.ToLower();
                        System.Text.RegularExpressions.Regex patternBooking = new System.Text.RegularExpressions.Regex(grupoPessoaEmitente.ExpressaoBooking, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        System.Text.RegularExpressions.Match matchBooking = patternBooking.Match(observacaoBooking);

                        if (matchBooking != null && !string.IsNullOrWhiteSpace(matchBooking.Value))
                            xmlNotaFiscal.NumeroBooking = matchBooking.Value.ToUpper();
                    }

                    if (!string.IsNullOrWhiteSpace(grupoPessoaEmitente.ExpressaoContainer))
                    {
                        string observacaoContainer = observacaoNfe.ToLower();
                        System.Text.RegularExpressions.Regex patternContainer = new System.Text.RegularExpressions.Regex(grupoPessoaEmitente.ExpressaoContainer, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        System.Text.RegularExpressions.Match matchContainer = patternContainer.Match(observacaoContainer);

                        if (matchContainer != null && !string.IsNullOrWhiteSpace(matchContainer.Value))
                        {
                            string numeroContainer = matchContainer.Value.ToUpper();
                            numeroContainer = numeroContainer.Replace(" ", "").Replace("-", "").Replace(".", "").Replace("/", "").Replace("\\", "").Trim();
                            xmlNotaFiscal.NumeroContainer = numeroContainer;
                        }
                    }

                    if (grupoPessoaEmitente.NCMsPallet?.Count > 0 && ncms?.Count > 0)
                    {
                        if (grupoPessoaEmitente.NCMsPallet.Any(o => ncms.Contains(o.NCM)))
                            xmlNotaFiscal.TipoNotaFiscalIntegrada = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.RemessaPallet;
                    }

                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

                    if (listaLeituraDinamicaXml == null)
                        listaLeituraDinamicaXml = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml>();

                    var leituraXMLMetrosCubicos = listaLeituraDinamicaXml.Where(o => o.CampoDestino == "MetrosCubicos").FirstOrDefault();
                    if (leituraXMLMetrosCubicos != null)
                    {
                        decimal metrosCubicosTryParse = 0;
                        if (decimal.TryParse(leituraXMLMetrosCubicos.Valor, System.Globalization.NumberStyles.Number, cultura, out metrosCubicosTryParse))
                            xmlNotaFiscal.MetrosCubicos = metrosCubicosTryParse;
                    }

                    var leituraXMLQuantidadePallets = listaLeituraDinamicaXml.Where(o => o.CampoDestino == "QuantidadePallets").FirstOrDefault();
                    if (leituraXMLQuantidadePallets != null)
                    {
                        decimal quantidadePalletsTryParse = 0;
                        if (decimal.TryParse(leituraXMLQuantidadePallets.Valor, System.Globalization.NumberStyles.Number, cultura, out quantidadePalletsTryParse))
                            xmlNotaFiscal.QuantidadePallets = quantidadePalletsTryParse;
                    }

                    Pessoa.GrupoPessoasObservacaoNfe servicoObservacaoNFe = new Pessoa.GrupoPessoasObservacaoNfe();

                    string numeroControlePedido = servicoObservacaoNFe.ObterNumeroControlePedido(grupoPessoaEmitente, observacaoNfe, xmlNotaFiscal.NumeroControlePedido);
                    string numeroControleCliente = servicoObservacaoNFe.ObterNumeroControleCliente(grupoPessoaEmitente, observacaoNfe, xmlNotaFiscal.NumeroControleCliente);
                    string numeroReferenciaEDI = servicoObservacaoNFe.ObterNumeroReferenciaEDI(grupoPessoaEmitente, observacaoNfe, xmlNotaFiscal.NumeroReferenciaEDI);

                    var leituraXMLNumeroControleCliente = listaLeituraDinamicaXml.Where(o => o.CampoDestino == "NumeroControleCliente").FirstOrDefault();

                    if (!string.IsNullOrEmpty(leituraXMLNumeroControleCliente?.Valor))
                        xmlNotaFiscal.NumeroControleCliente = leituraXMLNumeroControleCliente.Valor;
                    else if (!string.IsNullOrWhiteSpace(numeroControleCliente))
                        xmlNotaFiscal.NumeroControleCliente = numeroControleCliente;

                    if (!string.IsNullOrWhiteSpace(numeroReferenciaEDI))
                        xmlNotaFiscal.NumeroReferenciaEDI = numeroReferenciaEDI;

                    if (!string.IsNullOrWhiteSpace(numeroControlePedido))
                        xmlNotaFiscal.NumeroControlePedido = numeroControlePedido;
                }
            }
            erro = string.Empty;
            return true;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml> BuscarValoresLeituraDinamicaXML(List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml> listaGrupoPessoasLeituraDinamicaXml, dynamic infNFe)
        {
            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe notaFiscal = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe();
            notaFiscal.ObservacaoContribuinte = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Observacao>();
            notaFiscal.Volumes = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Volume>();


            if (infNFe.infAdic != null)
            {
                notaFiscal.Observacao = (string)infNFe.infAdic.infCpl;

                if (infNFe.infAdic.obsCont != null)
                {
                    foreach (dynamic obsCont in infNFe.infAdic.obsCont)
                    {
                        Dominio.ObjetosDeValor.Embarcador.NFe.Observacao observacao = new Dominio.ObjetosDeValor.Embarcador.NFe.Observacao();
                        observacao.xCampo = (string)obsCont.xCampo;
                        observacao.xTexto = (string)obsCont.xTexto;
                        notaFiscal.ObservacaoContribuinte.Add(observacao);
                    }
                }
            }

            if (infNFe.transp != null && infNFe.transp.vol != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");
                foreach (var vol in infNFe.transp.vol)
                {
                    Dominio.ObjetosDeValor.Embarcador.NFe.Volume volume = new Dominio.ObjetosDeValor.Embarcador.NFe.Volume();
                    volume.Numeracao = (string)vol.nVol;
                    volume.Especie = (string)vol.esp;
                    volume.MarcaVolume = (string)vol.marca;
                    volume.Quantidade = !string.IsNullOrWhiteSpace((string)vol.qVol) ? int.Parse((string)vol.qVol) : 0;
                    volume.PesoLiquido = !string.IsNullOrEmpty((string)vol.pesoL) ? decimal.Parse(((string)vol.pesoL).Replace(".", ","), cultura) : 0;
                    volume.PesoBruto = !string.IsNullOrEmpty((string)vol.pesoB) ? decimal.Parse(((string)vol.pesoB).Replace(".", ","), cultura) : 0;
                    notaFiscal.Volumes.Add(volume);
                }
            }

            return BuscarValoresLeituraDinamicaXML(listaGrupoPessoasLeituraDinamicaXml, notaFiscal, notaFiscal.Observacao);
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml> BuscarValoresLeituraDinamicaXML(List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml> listaGrupoPessoasLeituraDinamicaXml, Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe notaFiscal, string infCpl)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml>();

            foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml grupoPessoasLeituraDinamicaXml in listaGrupoPessoasLeituraDinamicaXml.Where(o => o.LeituraDinamicaXmlOrigem.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe))
            {
                try
                {
                    Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml campoLeitura = new Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml();


                    campoLeitura.CampoOrigem = grupoPessoasLeituraDinamicaXml.LeituraDinamicaXmlOrigem.Descricao;
                    campoLeitura.CampoDestino = grupoPessoasLeituraDinamicaXml.LeituraDinamicaXmlDestino.NomeCampo;
                    campoLeitura.Valor = string.Empty;

                    #region Buscar Valor

                    if (campoLeitura.CampoOrigem == "InfCpl")
                    {
                        campoLeitura.Valor = infCpl;
                    }
                    else if (campoLeitura.CampoOrigem == "ObsCont" && notaFiscal.ObservacaoContribuinte?.Count() > 0)
                    {
                        if (grupoPessoasLeituraDinamicaXml.FiltrarPrimeiroDisponivel)
                            campoLeitura.Valor = notaFiscal.ObservacaoContribuinte.FirstOrDefault()?.xTexto;
                        else
                            campoLeitura.Valor = notaFiscal.ObservacaoContribuinte.Where(o => o.xCampo.ToUpper() == grupoPessoasLeituraDinamicaXml.FiltrarTag.ToUpper()).FirstOrDefault()?.xTexto;
                    }
                    else if (campoLeitura.CampoOrigem == "Vol" && grupoPessoasLeituraDinamicaXml.LeituraDinamicaXmlOrigemTagFilha != null)
                    {
                        string campoTagFilha = grupoPessoasLeituraDinamicaXml.LeituraDinamicaXmlOrigemTagFilha.Descricao;

                        if (campoTagFilha == "qVol")
                        {
                            if (grupoPessoasLeituraDinamicaXml.FiltrarPrimeiroDisponivel)
                                campoLeitura.Valor = notaFiscal.Volumes.FirstOrDefault()?.Quantidade.ToString();
                            else
                                campoLeitura.Valor = notaFiscal.Volumes.Where(o => o.Especie == grupoPessoasLeituraDinamicaXml.FiltrarTag).FirstOrDefault()?.Quantidade.ToString();
                        }
                        else if (campoTagFilha == "marca")
                        {
                            if (grupoPessoasLeituraDinamicaXml.FiltrarPrimeiroDisponivel)
                                campoLeitura.Valor = notaFiscal.Volumes.FirstOrDefault()?.MarcaVolume;
                            else
                                campoLeitura.Valor = notaFiscal.Volumes.Where(o => o.Especie == grupoPessoasLeituraDinamicaXml.FiltrarTag).FirstOrDefault()?.MarcaVolume;
                        }
                        else if (campoTagFilha == "nVol")
                        {
                            if (grupoPessoasLeituraDinamicaXml.FiltrarPrimeiroDisponivel)
                                campoLeitura.Valor = notaFiscal.Volumes.FirstOrDefault()?.Numeracao;
                            else
                                campoLeitura.Valor = notaFiscal.Volumes.Where(o => o.Especie == grupoPessoasLeituraDinamicaXml.FiltrarTag).FirstOrDefault()?.Numeracao;
                        }
                    }

                    #endregion Buscar Valor


                    #region Filtrar Conteúdo

                    if (grupoPessoasLeituraDinamicaXml.HabilitarFiltrarConteudo)
                    {
                        if (grupoPessoasLeituraDinamicaXml.TipoFiltrarConteudo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFiltrarConteudo.ExpressaoRegular)
                        {
                            Match resultado = Regex.Match(campoLeitura.Valor, grupoPessoasLeituraDinamicaXml.FiltrarConteudoTextoInicial, RegexOptions.IgnoreCase);

                            if (resultado.Success)
                            {
                                if (resultado.Groups?.Count > 1)
                                    campoLeitura.Valor = resultado.Groups[1].Value;
                                else
                                    campoLeitura.Valor = resultado.Value;
                            }
                            else
                                campoLeitura.Valor = string.Empty;
                        }
                        else
                        {
                            /*
                            string pattern = grupoPessoasLeituraDinamicaXml.FiltrarConteudoTextoInicial + "(.*?)" + grupoPessoasLeituraDinamicaXml.FiltrarConteudoTextoFinal;
                            Match resultado = Regex.Match(campoLeitura.Valor, pattern, RegexOptions.IgnoreCase);

                            if (resultado.Success)
                                campoLeitura.Valor = resultado.Groups[1].Value;
                            else
                                campoLeitura.Valor = string.Empty;
                            */

                            int idxInicio = campoLeitura.Valor.IndexOf(grupoPessoasLeituraDinamicaXml.FiltrarConteudoTextoInicial);

                            if (idxInicio > -1)
                            {
                                idxInicio += grupoPessoasLeituraDinamicaXml.FiltrarConteudoTextoInicial.Length;

                                int idxFim = campoLeitura.Valor.IndexOf(grupoPessoasLeituraDinamicaXml.FiltrarConteudoTextoFinal, idxInicio);

                                if (idxFim > 0)
                                    campoLeitura.Valor = campoLeitura.Valor.Substring(idxInicio, idxFim - idxInicio);
                                else
                                    campoLeitura.Valor = string.Empty;
                            }
                            else
                                campoLeitura.Valor = string.Empty;
                        }
                    }

                    #endregion Filtrar Conteúdo


                    #region Tratar Conteúdo

                    if (!string.IsNullOrEmpty(grupoPessoasLeituraDinamicaXml.RemoverCaracteres))
                        campoLeitura.Valor = Regex.Replace(campoLeitura.Valor, $"[{grupoPessoasLeituraDinamicaXml.RemoverCaracteres}]", "");

                    if (grupoPessoasLeituraDinamicaXml.SubstituirVirgulaPorPonto)
                        campoLeitura.Valor = campoLeitura.Valor.Replace(".", ",");

                    #endregion Tratar Conteúdo

                    retorno.Add(campoLeitura);

                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
            }

            return retorno;
        }

        public Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa BuscarDadosNotaFiscalDestinada(System.IO.StreamReader xml, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe notaFiscal = null)
        {
            try
            {
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                xml.BaseStream.Position = 0;

                string stringNotaFiscal = xml.ReadToEnd();

                if (notaFiscal == null)
                {
                    Servicos.NFe serNFe = new Servicos.NFe(unitOfWork);

                    xml.BaseStream.Position = 0;

                    notaFiscal = serNFe.ObterDocumentoPorXML(xml.BaseStream, unitOfWork);
                }
                if (notaFiscal == null)
                {
                    Servicos.Log.TratarErro("Não foi possíbel converter o arquvi do anexo para NF-e.", "XMLEmail");
                    return null;
                }

                xml.Dispose();
                xml.Close();

                Dominio.Entidades.Empresa empresa = null;
                if (!string.IsNullOrWhiteSpace(notaFiscal.Destinatario))
                {
                    string cgcDestinatario = notaFiscal.Destinatario;
                    if (!string.IsNullOrWhiteSpace(cgcDestinatario))
                    {
                        if (cgcDestinatario != "00.000.000/0000-00")
                            empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cgcDestinatario));
                    }
                }

                Dominio.Entidades.Cliente remetente = notaFiscal.Remetente;

                if (empresa == null || remetente == null)
                {
                    Servicos.Log.TratarErro("Não foi possível localizar a empresa/destinatário e o cliente/remetente da NF-e.", "XMLEmail");
                    return null;
                }

                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = null;

                if (!string.IsNullOrWhiteSpace(notaFiscal.Chave))
                    documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorChave(notaFiscal.Chave);

                if (documentoDestinado == null)
                {
                    documentoDestinado = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                    documentoDestinado.DataIntegracao = DateTime.Now;
                }
                else
                {
                    Servicos.Log.TratarErro("NF-e já se encontra nos documentos destinados.", "XMLEmail");
                    return documentoDestinado;
                }

                DateTime dataEmissao;
                if (!DateTime.TryParseExact(notaFiscal.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                {
                    if (!DateTime.TryParseExact(notaFiscal.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                        dataEmissao = DateTime.Now;
                }

                documentoDestinado.Empresa = empresa;
                documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                documentoDestinado.Chave = notaFiscal.Chave;
                documentoDestinado.CPFCNPJEmitente = remetente.CPF_CNPJ_SemFormato;
                documentoDestinado.DataAutorizacao = dataEmissao;//DateTime.ParseExact(notaFiscal.DataEmissao, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None);
                documentoDestinado.DataEmissao = dataEmissao;//DateTime.ParseExact(notaFiscal.DataEmissao, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None);
                documentoDestinado.IEEmitente = remetente.IE_RG;
                documentoDestinado.NomeEmitente = remetente.Nome;
                documentoDestinado.Numero = int.Parse(notaFiscal.Numero);
                documentoDestinado.Serie = int.Parse(notaFiscal.Serie);
                documentoDestinado.NumeroSequencialUnico = 0;
                documentoDestinado.Protocolo = notaFiscal.Protocolo;
                documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeDestinada;
                documentoDestinado.TipoOperacao = notaFiscal.TipoOperacao == 0 ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Entrada : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Saida;
                documentoDestinado.Valor = notaFiscal.ValorTotal;
                documentoDestinado.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe;
                if (!string.IsNullOrWhiteSpace(documentoDestinado.Chave) && repDocumentoDestinadoEmpresa.ContemEventoCancelamentoPorChave(documentoDestinado.Chave))
                    documentoDestinado.Cancelado = true;

                repDocumentoDestinadoEmpresa.Inserir(documentoDestinado);

                new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unitOfWork).GerarIntegracaoDocumentoDestinado(documentoDestinado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeDestinada);

                return documentoDestinado;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao processar XML de NT-e.", "XMLEmail");
                Servicos.Log.TratarErro("erro: " + ex, "XMLEmail");
                return null;
            }
        }

        public Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa BuscarDadosCTeDestinada(System.IO.StreamReader xml, Repositorio.UnitOfWork unitOfWork, dynamic nfXml = null)
        {
            try
            {
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Servicos.Embarcador.CTe.CTe serCte = new Servicos.Embarcador.CTe.CTe(unitOfWork);

                xml.BaseStream.Position = 0;
                var cteLido = MultiSoftware.CTe.Servicos.Leitura.Ler(xml.BaseStream);
                if (cteLido == null)
                {
                    Servicos.Log.TratarErro("Não foi possível converter xml em CTe.", "XMLEmail");
                    return null;
                }
                else if (cteLido.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc))
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)cteLido;

                    Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = serCte.ConverterProcCTeParaCTe(cteProc);
                    if (cte == null)
                    {
                        Servicos.Log.TratarErro("Não conseguiu gerar objeto do CT-e do arquivo do anexo.", "XMLEmail");
                        return null;
                    }
                    Dominio.Entidades.Empresa empresa = null;
                    if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                        empresa = repEmpresa.BuscarPorCNPJ(cte.Destinatario.CPFCNPJ);
                    else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                        empresa = repEmpresa.BuscarPorCNPJ(cte.Remetente.CPFCNPJ);
                    else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                        empresa = repEmpresa.BuscarPorCNPJ(cte.Tomador.CPFCNPJ);
                    else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                        empresa = repEmpresa.BuscarPorCNPJ(cte.Recebedor.CPFCNPJ);
                    else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                        empresa = repEmpresa.BuscarPorCNPJ(cte.Expedidor.CPFCNPJ);

                    Dominio.Entidades.Cliente emitente = repCliente.BuscarPorCPFCNPJ(double.Parse(cte.Emitente.CNPJ));
                    if (emitente == null)
                    {
                        emitente = Servicos.Embarcador.Pessoa.Pessoa.Converter(cte.Emitente, unitOfWork);
                        repCliente.Inserir(emitente);
                    }

                    if (empresa != null && emitente != null)
                    {
                        Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = null;
                        if (!string.IsNullOrWhiteSpace(cte.Chave))
                            documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorChave(cte.Chave);

                        if (documentoDestinado == null)
                        {
                            documentoDestinado = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                            documentoDestinado.DataIntegracao = DateTime.Now;
                        }
                        else
                        {
                            Servicos.Log.TratarErro("Arquivo CT-e já existente na base de dados.", "XMLEmail");
                            return documentoDestinado;
                        }

                        documentoDestinado.Empresa = empresa;
                        documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                        documentoDestinado.Chave = cte.Chave;
                        documentoDestinado.CPFCNPJEmitente = emitente.CPF_CNPJ_SemFormato;
                        documentoDestinado.DataAutorizacao = cte.DataEmissao;
                        documentoDestinado.DataEmissao = cte.DataEmissao;
                        documentoDestinado.IEEmitente = emitente.IE_RG;
                        documentoDestinado.NomeEmitente = emitente.Nome;
                        documentoDestinado.Numero = cte.Numero;
                        documentoDestinado.Serie = int.Parse(cte.Serie);
                        documentoDestinado.NumeroSequencialUnico = 0;
                        documentoDestinado.Protocolo = cte.Protocolo;
                        //if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                        //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoDestinatario;
                        //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                        //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRemetente;
                        //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                        //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                        //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                        //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRecebedor;
                        //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                        //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoExpedidor;
                        //todo o cte recebido é como tomador da empresa
                        documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                        documentoDestinado.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Entrada;
                        documentoDestinado.Valor = cte.ValorFrete.ValorPrestacaoServico;
                        documentoDestinado.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.CTe;
                        if (!string.IsNullOrWhiteSpace(documentoDestinado.Chave) && repDocumentoDestinadoEmpresa.ContemEventoCancelamentoPorChave(documentoDestinado.Chave))
                            documentoDestinado.Cancelado = true;

                        if (cteProc.CTe.infCte.rem != null)
                        {
                            documentoDestinado.CPFCNPJRemetente = cteProc.CTe.infCte.rem.Item;
                            documentoDestinado.NomeRemetente = cteProc.CTe.infCte.rem.xNome;
                        }

                        if (cteProc.CTe.infCte.dest != null)
                        {
                            documentoDestinado.CPFCNPJDestinatario = cteProc.CTe.infCte.dest.Item;
                            documentoDestinado.NomeDestinatario = cteProc.CTe.infCte.dest.xNome;
                            documentoDestinado.UFDestinatario = cteProc.CTe.infCte.dest.enderDest.UF.ToString("g");
                        }

                        if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                        {
                            MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 tomador = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)cteProc.CTe.infCte.ide.Item;

                            documentoDestinado.CPFCNPJTomador = tomador.Item;
                            documentoDestinado.NomeTomador = tomador.xNome;
                        }
                        else
                        {
                            MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3 tomador = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3)cteProc.CTe.infCte.ide.Item;

                            if (tomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item0)
                            {
                                documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.rem.Item;
                                documentoDestinado.NomeTomador = cteProc.CTe.infCte.rem.xNome;
                            }
                            else if (tomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item1)
                            {
                                documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.exped.Item;
                                documentoDestinado.NomeTomador = cteProc.CTe.infCte.exped.xNome;
                            }
                            else if (tomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item2)
                            {
                                documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.receb.Item;
                                documentoDestinado.NomeTomador = cteProc.CTe.infCte.receb.xNome;
                            }
                            else if (tomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item3)
                            {
                                documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.dest.Item;
                                documentoDestinado.NomeTomador = cteProc.CTe.infCte.dest.xNome;
                            }
                        }

                        repDocumentoDestinadoEmpresa.Inserir(documentoDestinado);
                        new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unitOfWork).GerarIntegracaoDocumentoDestinado(documentoDestinado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador);
                        return documentoDestinado;
                    }
                    else
                    {
                        Servicos.Log.TratarErro("Não foi encontrado a empresa/tomador ou emitente cadastrado do CT-e.", "XMLEmail");
                        return null;
                    }
                }
                else if (cteLido.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)cteLido;

                    Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = serCte.ConverterProcCTeParaCTe(cteProc);
                    if (cte == null)
                    {
                        Servicos.Log.TratarErro("Não conseguiu gerar objeto do CT-e do arquivo do anexo.", "XMLEmail");
                        return null;
                    }
                    Dominio.Entidades.Empresa empresa = null;
                    if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                        empresa = repEmpresa.BuscarPorCNPJ(cte.Destinatario.CPFCNPJ);
                    else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                        empresa = repEmpresa.BuscarPorCNPJ(cte.Remetente.CPFCNPJ);
                    else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                        empresa = repEmpresa.BuscarPorCNPJ(cte.Tomador.CPFCNPJ);
                    else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                        empresa = repEmpresa.BuscarPorCNPJ(cte.Recebedor.CPFCNPJ);
                    else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                        empresa = repEmpresa.BuscarPorCNPJ(cte.Expedidor.CPFCNPJ);

                    Dominio.Entidades.Cliente emitente = repCliente.BuscarPorCPFCNPJ(double.Parse(cte.Emitente.CNPJ));
                    if (emitente == null)
                    {
                        emitente = Servicos.Embarcador.Pessoa.Pessoa.Converter(cte.Emitente, unitOfWork);
                        repCliente.Inserir(emitente);
                    }

                    if (empresa != null && emitente != null)
                    {
                        Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = null;
                        if (!string.IsNullOrWhiteSpace(cte.Chave))
                            documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorChave(cte.Chave);

                        if (documentoDestinado == null)
                        {
                            documentoDestinado = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                            documentoDestinado.DataIntegracao = DateTime.Now;
                        }
                        else
                        {
                            Servicos.Log.TratarErro("Arquivo CT-e já existente na base de dados.", "XMLEmail");
                            return documentoDestinado;
                        }

                        documentoDestinado.Empresa = empresa;
                        documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                        documentoDestinado.Chave = cte.Chave;
                        documentoDestinado.CPFCNPJEmitente = emitente.CPF_CNPJ_SemFormato;
                        documentoDestinado.DataAutorizacao = cte.DataEmissao;
                        documentoDestinado.DataEmissao = cte.DataEmissao;
                        documentoDestinado.IEEmitente = emitente.IE_RG;
                        documentoDestinado.NomeEmitente = emitente.Nome;
                        documentoDestinado.Numero = cte.Numero;
                        documentoDestinado.Serie = int.Parse(cte.Serie);
                        documentoDestinado.NumeroSequencialUnico = 0;
                        documentoDestinado.Protocolo = cte.Protocolo;
                        //if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                        //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoDestinatario;
                        //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                        //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRemetente;
                        //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                        //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                        //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                        //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRecebedor;
                        //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                        //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoExpedidor;
                        //todo o cte recebido é como tomador da empresa
                        documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                        documentoDestinado.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Entrada;
                        documentoDestinado.Valor = cte.ValorFrete.ValorPrestacaoServico;
                        documentoDestinado.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.CTe;
                        if (!string.IsNullOrWhiteSpace(documentoDestinado.Chave) && repDocumentoDestinadoEmpresa.ContemEventoCancelamentoPorChave(documentoDestinado.Chave))
                            documentoDestinado.Cancelado = true;

                        if (cteProc.CTe.infCte.rem != null)
                        {
                            documentoDestinado.CPFCNPJRemetente = cteProc.CTe.infCte.rem.Item;
                            documentoDestinado.NomeRemetente = cteProc.CTe.infCte.rem.xNome;
                        }

                        if (cteProc.CTe.infCte.dest != null)
                        {
                            documentoDestinado.CPFCNPJDestinatario = cteProc.CTe.infCte.dest.Item;
                            documentoDestinado.NomeDestinatario = cteProc.CTe.infCte.dest.xNome;
                            documentoDestinado.UFDestinatario = cteProc.CTe.infCte.dest.enderDest.UF.ToString("g");
                        }

                        if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                        {
                            MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 tomador = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)cteProc.CTe.infCte.ide.Item;

                            documentoDestinado.CPFCNPJTomador = tomador.Item;
                            documentoDestinado.NomeTomador = tomador.xNome;
                        }
                        else
                        {
                            MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3 tomador = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3)cteProc.CTe.infCte.ide.Item;

                            if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item0)
                            {
                                documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.rem.Item;
                                documentoDestinado.NomeTomador = cteProc.CTe.infCte.rem.xNome;
                            }
                            else if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item1)
                            {
                                documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.exped.Item;
                                documentoDestinado.NomeTomador = cteProc.CTe.infCte.exped.xNome;
                            }
                            else if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item2)
                            {
                                documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.receb.Item;
                                documentoDestinado.NomeTomador = cteProc.CTe.infCte.receb.xNome;
                            }
                            else if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item3)
                            {
                                documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.dest.Item;
                                documentoDestinado.NomeTomador = cteProc.CTe.infCte.dest.xNome;
                            }
                        }

                        repDocumentoDestinadoEmpresa.Inserir(documentoDestinado);
                        new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unitOfWork).GerarIntegracaoDocumentoDestinado(documentoDestinado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador);
                        return documentoDestinado;
                    }
                    else
                    {
                        Servicos.Log.TratarErro("Não foi encontrado a empresa/tomador ou emitente cadastrado do CT-e.", "XMLEmail");
                        return null;
                    }
                }
                else if (cteLido.GetType() == typeof(MultiSoftware.CTe.v200.Eventos.TProcEvento))
                {
                    MultiSoftware.CTe.v200.Eventos.TProcEvento procEvento = (MultiSoftware.CTe.v200.Eventos.TProcEvento)cteLido;

                    if (procEvento.retEventoCTe.infEvento.cStat == "135")
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave(procEvento.retEventoCTe.infEvento.chCTe);

                        if (cte != null)
                        {

                            Dominio.Entidades.Empresa empresa = null;
                            if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                                empresa = repEmpresa.BuscarPorCNPJ(cte.Destinatario.CPF_CNPJ);
                            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                                empresa = repEmpresa.BuscarPorCNPJ(cte.Remetente.CPF_CNPJ);
                            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                                empresa = repEmpresa.BuscarPorCNPJ(cte.Tomador.CPF_CNPJ);
                            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                                empresa = repEmpresa.BuscarPorCNPJ(cte.Recebedor.CPF_CNPJ);
                            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                                empresa = repEmpresa.BuscarPorCNPJ(cte.Expedidor.CPF_CNPJ);

                            Dominio.Entidades.Cliente emitente = repCliente.BuscarPorCPFCNPJ(double.Parse(cte.Remetente.CPF_CNPJ));

                            if (empresa != null && emitente != null)
                            {
                                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(
                                         new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe }, cte.Chave);

                                if (documentoDestinado == null)
                                {
                                    documentoDestinado = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                                    documentoDestinado.DataIntegracao = DateTime.Now;
                                }
                                else
                                {
                                    Servicos.Log.TratarErro("Arquivo CT-e já existente na base de dados.", "XMLEmail");
                                    return documentoDestinado;
                                }

                                documentoDestinado.Empresa = empresa;
                                documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                                documentoDestinado.Chave = cte.Chave;
                                documentoDestinado.CPFCNPJEmitente = emitente.CPF_CNPJ_SemFormato;
                                documentoDestinado.DataAutorizacao = cte.DataEmissao;
                                documentoDestinado.DataEmissao = cte.DataEmissao;
                                documentoDestinado.IEEmitente = emitente.IE_RG;
                                documentoDestinado.NomeEmitente = emitente.Nome;
                                documentoDestinado.Numero = cte.Numero;
                                documentoDestinado.Serie = cte.Serie.Numero;
                                documentoDestinado.NumeroSequencialUnico = 0;
                                documentoDestinado.Protocolo = cte.Protocolo;
                                //if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                                //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoDestinatario;
                                //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                                //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRemetente;
                                //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                                //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                                //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                                //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRecebedor;
                                //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                                //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoExpedidor;
                                //todo o cte recebido é como tomador da empresa
                                documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe;
                                documentoDestinado.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Entrada;
                                documentoDestinado.Valor = cte.ValorPrestacaoServico;
                                documentoDestinado.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.CTe;
                                documentoDestinado.Cancelado = true;

                                if (cte.Remetente != null)
                                {
                                    documentoDestinado.CPFCNPJRemetente = cte.Remetente.CPF_CNPJ;
                                    documentoDestinado.NomeRemetente = cte.Remetente.Descricao;
                                }

                                if (cte.Destinatario != null)
                                {
                                    documentoDestinado.CPFCNPJDestinatario = cte.Destinatario.CPF_CNPJ;
                                    documentoDestinado.NomeDestinatario = cte.Destinatario.Descricao;
                                    documentoDestinado.UFDestinatario = cte.Destinatario.Localidade?.Estado?.Sigla;
                                }

                                //if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                                //{
                                //    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 tomador = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)cteProc.CTe.infCte.ide.Item;

                                //    documentoDestinado.CPFCNPJTomador = tomador.Item;
                                //    documentoDestinado.NomeTomador = tomador.xNome;
                                //}
                                //else
                                //{

                                documentoDestinado.CPFCNPJTomador = empresa.Descricao;
                                documentoDestinado.NomeTomador = empresa.NomeFantasia;

                                //MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3 tomador = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3)cteProc.CTe.infCte.ide.Item;

                                //if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item0)
                                //{
                                //    documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.rem.Item;
                                //    documentoDestinado.NomeTomador = cteProc.CTe.infCte.rem.xNome;
                                //}
                                //else if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item1)
                                //{
                                //    documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.exped.Item;
                                //    documentoDestinado.NomeTomador = cteProc.CTe.infCte.exped.xNome;
                                //}
                                //else if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item2)
                                //{
                                //    documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.receb.Item;
                                //    documentoDestinado.NomeTomador = cteProc.CTe.infCte.receb.xNome;
                                //}
                                //else if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item3)
                                //{
                                //    documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.dest.Item;
                                //    documentoDestinado.NomeTomador = cteProc.CTe.infCte.dest.xNome;
                                //}
                                //}

                                repDocumentoDestinadoEmpresa.Inserir(documentoDestinado);
                                new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unitOfWork).GerarIntegracaoDocumentoDestinado(documentoDestinado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe);
                                new Servicos.Embarcador.Documentos.ControleDocumento(unitOfWork).ReprocessarRegistroControleDocumento(cte);
                                return documentoDestinado;

                            }
                            else
                            {
                                Servicos.Log.TratarErro("Não foi encontrado a empresa/tomador ou emitente cadastrado do CT-e.", "XMLEmail");
                                return null;
                            }
                        }
                        else
                        {
                            Servicos.Log.TratarErro("Não foi encontrado o CT-e pela chave", "XMLEmail");
                            return null;
                        }
                    }
                    else
                    {
                        Servicos.Log.TratarErro("Status de cancelamento do CT-e não equivalente.", "XMLEmail");
                        return null;
                    }
                }
                else if (cteLido.GetType() == typeof(MultiSoftware.CTe.v300.Eventos.TProcEvento))
                {
                    MultiSoftware.CTe.v300.Eventos.TProcEvento procEvento = (MultiSoftware.CTe.v300.Eventos.TProcEvento)cteLido;

                    if (procEvento.retEventoCTe.infEvento.cStat == "135")
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave(procEvento.retEventoCTe.infEvento.chCTe);

                        if (cte != null)
                        {

                            Dominio.Entidades.Empresa empresa = null;
                            if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                                empresa = repEmpresa.BuscarPorCNPJ(cte.Destinatario.CPF_CNPJ);
                            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                                empresa = repEmpresa.BuscarPorCNPJ(cte.Remetente.CPF_CNPJ);
                            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                                empresa = repEmpresa.BuscarPorCNPJ(cte.Tomador.CPF_CNPJ);
                            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                                empresa = repEmpresa.BuscarPorCNPJ(cte.Recebedor.CPF_CNPJ);
                            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                                empresa = repEmpresa.BuscarPorCNPJ(cte.Expedidor.CPF_CNPJ);

                            Dominio.Entidades.Cliente emitente = repCliente.BuscarPorCPFCNPJ(double.Parse(cte.Remetente.CPF_CNPJ));

                            if (empresa != null && emitente != null)
                            {

                                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(
                                        new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe }, cte.Chave);

                                if (documentoDestinado == null)
                                {
                                    documentoDestinado = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                                    documentoDestinado.DataIntegracao = DateTime.Now;
                                }
                                else
                                {
                                    Servicos.Log.TratarErro("Arquivo CT-e já existente na base de dados.", "XMLEmail");
                                    return documentoDestinado;
                                }

                                documentoDestinado.Empresa = empresa;
                                documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                                documentoDestinado.Chave = cte.Chave;
                                documentoDestinado.CPFCNPJEmitente = emitente.CPF_CNPJ_SemFormato;
                                documentoDestinado.DataAutorizacao = cte.DataEmissao;
                                documentoDestinado.DataEmissao = cte.DataEmissao;
                                documentoDestinado.IEEmitente = emitente.IE_RG;
                                documentoDestinado.NomeEmitente = emitente.Nome;
                                documentoDestinado.Numero = cte.Numero;
                                documentoDestinado.Serie = cte.Serie.Numero;
                                documentoDestinado.NumeroSequencialUnico = 0;
                                documentoDestinado.Protocolo = cte.Protocolo;
                                //if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                                //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoDestinatario;
                                //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                                //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRemetente;
                                //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                                //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                                //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                                //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRecebedor;
                                //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                                //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoExpedidor;
                                //todo o cte recebido é como tomador da empresa
                                documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe;
                                documentoDestinado.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Entrada;
                                documentoDestinado.Valor = cte.ValorPrestacaoServico;
                                documentoDestinado.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.CTe;
                                documentoDestinado.Cancelado = true;

                                if (cte.Remetente != null)
                                {
                                    documentoDestinado.CPFCNPJRemetente = cte.Remetente.CPF_CNPJ;
                                    documentoDestinado.NomeRemetente = cte.Remetente.Descricao;
                                }

                                if (cte.Destinatario != null)
                                {
                                    documentoDestinado.CPFCNPJDestinatario = cte.Destinatario.CPF_CNPJ;
                                    documentoDestinado.NomeDestinatario = cte.Destinatario.Descricao;
                                    documentoDestinado.UFDestinatario = cte.Destinatario.Localidade?.Estado?.Sigla;
                                }

                                //if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                                //{
                                //    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 tomador = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)cteProc.CTe.infCte.ide.Item;

                                //    documentoDestinado.CPFCNPJTomador = tomador.Item;
                                //    documentoDestinado.NomeTomador = tomador.xNome;
                                //}
                                //else
                                //{

                                documentoDestinado.CPFCNPJTomador = empresa.CNPJ_SemFormato;
                                documentoDestinado.NomeTomador = empresa.NomeFantasia;

                                //MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3 tomador = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3)cteProc.CTe.infCte.ide.Item;

                                //if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item0)
                                //{
                                //    documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.rem.Item;
                                //    documentoDestinado.NomeTomador = cteProc.CTe.infCte.rem.xNome;
                                //}
                                //else if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item1)
                                //{
                                //    documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.exped.Item;
                                //    documentoDestinado.NomeTomador = cteProc.CTe.infCte.exped.xNome;
                                //}
                                //else if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item2)
                                //{
                                //    documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.receb.Item;
                                //    documentoDestinado.NomeTomador = cteProc.CTe.infCte.receb.xNome;
                                //}
                                //else if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item3)
                                //{
                                //    documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.dest.Item;
                                //    documentoDestinado.NomeTomador = cteProc.CTe.infCte.dest.xNome;
                                //}
                                //}

                                repDocumentoDestinadoEmpresa.Inserir(documentoDestinado);
                                new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unitOfWork).GerarIntegracaoDocumentoDestinado(documentoDestinado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe);
                                new Servicos.Embarcador.Documentos.ControleDocumento(unitOfWork).ReprocessarRegistroControleDocumento(cte);
                                return documentoDestinado;

                            }
                            else
                            {
                                Servicos.Log.TratarErro("Não foi encontrado a empresa/tomador ou emitente cadastrado do CT-e.", "XMLEmail");
                                return null;
                            }
                        }
                        else
                        {
                            Servicos.Log.TratarErro("Não foi encontrado o CT-e pela chave", "XMLEmail");
                            return null;
                        }
                    }
                    else
                    {
                        Servicos.Log.TratarErro("Status de cancelamento do CT-e não equivalente.", "XMLEmail");
                        return null;
                    }
                }
                else if (cteLido.GetType() == typeof(MultiSoftware.CTe.v400.Eventos.TProcEvento))
                {
                    MultiSoftware.CTe.v400.Eventos.TProcEvento procEvento = (MultiSoftware.CTe.v400.Eventos.TProcEvento)cteLido;

                    if (procEvento.retEventoCTe.infEvento.cStat == "135")
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave(procEvento.retEventoCTe.infEvento.chCTe);

                        if (cte != null)
                        {

                            Dominio.Entidades.Empresa empresa = null;
                            if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                                empresa = repEmpresa.BuscarPorCNPJ(cte.Destinatario.CPF_CNPJ);
                            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                                empresa = repEmpresa.BuscarPorCNPJ(cte.Remetente.CPF_CNPJ);
                            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                                empresa = repEmpresa.BuscarPorCNPJ(cte.Tomador.CPF_CNPJ);
                            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                                empresa = repEmpresa.BuscarPorCNPJ(cte.Recebedor.CPF_CNPJ);
                            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                                empresa = repEmpresa.BuscarPorCNPJ(cte.Expedidor.CPF_CNPJ);

                            Dominio.Entidades.Cliente emitente = repCliente.BuscarPorCPFCNPJ(double.Parse(cte.Remetente.CPF_CNPJ));

                            if (empresa != null && emitente != null)
                            {
                                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(
                                        new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe }, cte.Chave);

                                if (documentoDestinado == null)
                                {
                                    documentoDestinado = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa();
                                    documentoDestinado.DataIntegracao = DateTime.Now;
                                }
                                else
                                {
                                    Servicos.Log.TratarErro("Arquivo CT-e já existente na base de dados.", "XMLEmail");
                                    return documentoDestinado;
                                }

                                documentoDestinado.Empresa = empresa;
                                documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.SemManifestacao;
                                documentoDestinado.Chave = cte.Chave;
                                documentoDestinado.CPFCNPJEmitente = emitente.CPF_CNPJ_SemFormato;
                                documentoDestinado.DataAutorizacao = cte.DataEmissao;
                                documentoDestinado.DataEmissao = cte.DataEmissao;
                                documentoDestinado.IEEmitente = emitente.IE_RG;
                                documentoDestinado.NomeEmitente = emitente.Nome;
                                documentoDestinado.Numero = cte.Numero;
                                documentoDestinado.Serie = cte.Serie.Numero;
                                documentoDestinado.NumeroSequencialUnico = 0;
                                documentoDestinado.Protocolo = cte.Protocolo;
                                //if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                                //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoDestinatario;
                                //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                                //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRemetente;
                                //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                                //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador;
                                //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                                //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRecebedor;
                                //else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                                //    documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoExpedidor;
                                //todo o cte recebido é como tomador da empresa
                                documentoDestinado.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe;
                                documentoDestinado.TipoOperacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNFe.Entrada;
                                documentoDestinado.Valor = cte.ValorPrestacaoServico;
                                documentoDestinado.ModeloDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.CTe;
                                documentoDestinado.Cancelado = true;

                                if (cte.Remetente != null)
                                {
                                    documentoDestinado.CPFCNPJRemetente = cte.Remetente.CPF_CNPJ;
                                    documentoDestinado.NomeRemetente = cte.Remetente.Descricao;
                                }

                                if (cte.Destinatario != null)
                                {
                                    documentoDestinado.CPFCNPJDestinatario = cte.Destinatario.CPF_CNPJ;
                                    documentoDestinado.NomeDestinatario = cte.Destinatario.Descricao;
                                    documentoDestinado.UFDestinatario = cte.Destinatario.Localidade?.Estado?.Sigla;
                                }

                                //if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                                //{
                                //    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 tomador = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)cteProc.CTe.infCte.ide.Item;

                                //    documentoDestinado.CPFCNPJTomador = tomador.Item;
                                //    documentoDestinado.NomeTomador = tomador.xNome;
                                //}
                                //else
                                //{

                                documentoDestinado.CPFCNPJTomador = empresa.Descricao;
                                documentoDestinado.NomeTomador = empresa.NomeFantasia;

                                //MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3 tomador = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3)cteProc.CTe.infCte.ide.Item;

                                //if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item0)
                                //{
                                //    documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.rem.Item;
                                //    documentoDestinado.NomeTomador = cteProc.CTe.infCte.rem.xNome;
                                //}
                                //else if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item1)
                                //{
                                //    documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.exped.Item;
                                //    documentoDestinado.NomeTomador = cteProc.CTe.infCte.exped.xNome;
                                //}
                                //else if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item2)
                                //{
                                //    documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.receb.Item;
                                //    documentoDestinado.NomeTomador = cteProc.CTe.infCte.receb.xNome;
                                //}
                                //else if (tomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item3)
                                //{
                                //    documentoDestinado.CPFCNPJTomador = cteProc.CTe.infCte.dest.Item;
                                //    documentoDestinado.NomeTomador = cteProc.CTe.infCte.dest.xNome;
                                //}
                                //}

                                repDocumentoDestinadoEmpresa.Inserir(documentoDestinado);
                                new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unitOfWork).GerarIntegracaoDocumentoDestinado(documentoDestinado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoCTe);
                                new Servicos.Embarcador.Documentos.ControleDocumento(unitOfWork).ReprocessarRegistroControleDocumento(cte);

                                return documentoDestinado;

                            }
                            else
                            {
                                Servicos.Log.TratarErro("Não foi encontrado a empresa/tomador ou emitente cadastrado do CT-e.", "XMLEmail");
                                return null;
                            }
                        }
                        else
                        {
                            Servicos.Log.TratarErro("Não foi encontrado o CT-e pela chave", "XMLEmail");
                            return null;
                        }
                    }
                    else
                    {
                        Servicos.Log.TratarErro("Status de cancelamento do CT-e não equivalente.", "XMLEmail");
                        return null;
                    }
                }
                else
                {
                    Servicos.Log.TratarErro("O arquivo XML não é do modelo CT-e 3.0.", "XMLEmail");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao processar XML de CT-e.", "XMLEmail");
                Servicos.Log.TratarErro("erro: " + ex, "XMLEmail");
                return null;
            }
        }

        public static MultiSoftware.NFe.NFeDownloadNF.DownloadNF.TRetDownloadNFe DownloadXMLNFeSEFAZ(int codigoEmpresa, string chaveNFe, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.ManifestacaoDestinatario repManifestacaoDestinatario = new Repositorio.Embarcador.Documentos.ManifestacaoDestinatario(unidadeTrabalho);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            if (empresa == null || string.IsNullOrWhiteSpace(empresa.NomeCertificado) || !Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado) || string.IsNullOrWhiteSpace(empresa.SenhaCertificado))
                throw new Exception("Certificado da empresa é inválido ou inexistente.");

            MultiSoftware.NFe.NFeDownloadNF.DownloadNF.TAmb tipoAmbiente = MultiSoftware.NFe.NFeDownloadNF.DownloadNF.TAmb.Item1; //sempre produção

            MultiSoftware.NFe.NFeDownloadNF.DownloadNF.TRetDownloadNFe retorno = MultiSoftware.NFe.NFeDownloadNF.Servicos.DownloadNFe.RealizarDownload(empresa.CNPJ, tipoAmbiente, chaveNFe, empresa.NomeCertificado, empresa.SenhaCertificado);

            return retorno;
        }

        public static byte[] ObterXMLNFe(MultiSoftware.NFe.NFeDownloadNF.DownloadNF.TRetDownloadNFe retorno)
        {
            if (retorno.retNFe[0].Item.GetType() == typeof(byte[]))
            {
                using (MemoryStream compressedStream = new MemoryStream((byte[])retorno.retNFe[0].Item))
                {
                    using (GZipStream zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                    {
                        using (MemoryStream resultStream = new MemoryStream())
                        {
                            zipStream.CopyTo(resultStream);

                            resultStream.Position = 0;

                            return resultStream.ToArray();
                        }
                    }
                }
            }
            else if (retorno.retNFe[0].Item.GetType() == typeof(MultiSoftware.NFe.NFeDownloadNF.DownloadNF.TRetDownloadNFeRetNFeProcNFe))
            {
                MultiSoftware.NFe.NFeDownloadNF.DownloadNF.TRetDownloadNFeRetNFeProcNFe notaFiscal = (MultiSoftware.NFe.NFeDownloadNF.DownloadNF.TRetDownloadNFeRetNFeProcNFe)retorno.retNFe[0].Item;

                return System.Text.Encoding.UTF8.GetBytes(notaFiscal.Any.OuterXml);
            }
            else if (retorno.retNFe[0].Item.GetType() == typeof(MultiSoftware.NFe.NFeDownloadNF.DownloadNF.TRetDownloadNFeRetNFeProcNFeGrupoZip))
            {
                MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc nfeProc = new MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc();

                MultiSoftware.NFe.NFeDownloadNF.DownloadNF.TRetDownloadNFeRetNFeProcNFeGrupoZip notaFiscal = (MultiSoftware.NFe.NFeDownloadNF.DownloadNF.TRetDownloadNFeRetNFeProcNFeGrupoZip)retorno.retNFe[0].Item;

                using (MemoryStream compressedStream = new MemoryStream(notaFiscal.NFeZip))
                {
                    using (GZipStream zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                    {
                        using (MemoryStream resultStream = new MemoryStream())
                        {
                            zipStream.CopyTo(resultStream);

                            resultStream.Position = 0;

                            XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFe));

                            nfeProc.NFe = (ser.Deserialize(resultStream) as MultiSoftware.NFe.v310.NotaFiscal.TNFe);
                        }
                    }
                }

                using (MemoryStream compressedStream = new MemoryStream(notaFiscal.protNFeZip))
                {
                    using (GZipStream zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                    {
                        using (MemoryStream resultStream = new MemoryStream())
                        {
                            zipStream.CopyTo(resultStream);

                            resultStream.Position = 0;

                            XmlSerializer ser = new XmlSerializer(typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TProtNFe));

                            nfeProc.protNFe = (ser.Deserialize(resultStream) as MultiSoftware.NFe.v310.NotaFiscalProcessada.TProtNFe);
                        }
                    }
                }

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc));

                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "http://www.portalfiscal.inf.br/nfe");

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(memoryStream, nfeProc, namespaces);

                    memoryStream.Position = 0;

                    return memoryStream.ToArray();
                }
            }

            return null;
        }

        public static ServicoConsultaNFe.RetornoOfRequisicaoSefazgj5B5PAD ObterRequisicaoConsultaNFeSiteSEFAZ(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.WebServicesConsultaNFe webService = Servicos.Embarcador.Documentos.ConsultaReceita.ObterWebService(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceConsultaReceita.NFe, unitOfWork);

            ServicoConsultaNFe.ConsultaNFeClient svcConsultaNFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoConsultaNFe.ConsultaNFeClient, ServicoConsultaNFe.IConsultaNFe>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SGTWebService_ConsultaNFe);

            if (webService != null)
                svcConsultaNFe.Endpoint.Address = new EndpointAddress(webService.WebService);

            OperationContextScope scope = new OperationContextScope(svcConsultaNFe.InnerChannel);
            MessageHeader header = MessageHeader.CreateHeader("Token", "Token", "4ed60154d2f04201ab8b57ed4198da32");
            OperationContext.Current.OutgoingMessageHeaders.Add(header);

            ServicoConsultaNFe.RetornoOfRequisicaoSefazgj5B5PAD requisicaoSefaz = svcConsultaNFe.SolicitarRequisicaoSefaz();

            Servicos.Embarcador.Documentos.ConsultaReceita.AjustarConsulta(webService, requisicaoSefaz.Status, unitOfWork);

            return requisicaoSefaz;
        }

        public static ServicoConsultaNFe.RetornoOfConsultaSefazgj5B5PAD ConsultarNFeSiteSEFAZ(Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz requisicaoSefaz, string chaveNFe, string captcha, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.WebServicesConsultaNFe webService = Servicos.Embarcador.Documentos.ConsultaReceita.ObterWebService(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceConsultaReceita.NFe, unitOfWork);

            ServicoConsultaNFe.ConsultaNFeClient svcConsultaNFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoConsultaNFe.ConsultaNFeClient, ServicoConsultaNFe.IConsultaNFe>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SGTWebService_ConsultaNFe);

            if (webService != null)
                svcConsultaNFe.Endpoint.Address = new EndpointAddress(webService.WebService);

            OperationContextScope scope = new OperationContextScope(svcConsultaNFe.InnerChannel);
            MessageHeader header = MessageHeader.CreateHeader("Token", "Token", "4ed60154d2f04201ab8b57ed4198da32");
            OperationContext.Current.OutgoingMessageHeaders.Add(header);

            ServicoConsultaNFe.RetornoOfConsultaSefazgj5B5PAD retornoConsultaSEFAZ = svcConsultaNFe.ConsultarSefaz(requisicaoSefaz, chaveNFe, captcha);

            return retornoConsultaSEFAZ;
        }

        public static bool ObterDadosNFePelaChave(string chave, out string erro, out object dadosNFe, int codigoEmpresa, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Servicos.Embarcador.Documentos.ConsultaDocumento srvConsultaDocumento = new Documentos.ConsultaDocumento(unidadeTrabalho, tipoServicoMultisoftware, auditado);

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = srvConsultaDocumento.ObterNotaFiscal(chave);

            if (xmlNotaFiscal == null)
                xmlNotaFiscal = srvConsultaDocumento.ObterNotaFiscalPorDocumentoDestinado(chave, tipoServicoMultisoftware);

            if (xmlNotaFiscal == null)
                xmlNotaFiscal = srvConsultaDocumento.ObterNotaFiscalPorSerpro(chave);

            if (xmlNotaFiscal == null)
            {
                erro = "Não foi possível encontrar nenhuma nota fiscal com a chave informada";
                dadosNFe = null;
                return false;
            }

            Servicos.NFe svcNFe = new Servicos.NFe(unidadeTrabalho);
            dadosNFe = svcNFe.ObterDocumentoPorObjeto(xmlNotaFiscal, codigoEmpresa, unidadeTrabalho);
            erro = string.Empty;

            return true;
        }

        public static bool ObterDadosNFeSiteSEFAZ(ServicoConsultaNFe.RetornoOfConsultaSefazgj5B5PAD retornoConsulta, out string erro, out object dadosNFe, int codigoEmpresa, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao)
        {
            if (!retornoConsulta.Status)
            {
                erro = HttpUtility.HtmlDecode(retornoConsulta.Mensagem);
                dadosNFe = null;
                return false;
            }

            if (!retornoConsulta.Objeto.ConsultaValida)
            {
                erro = HttpUtility.HtmlDecode(retornoConsulta.Objeto.MensagemSefaz);
                dadosNFe = null;
                return false;
            }

            Servicos.NFe svcNFe = new Servicos.NFe(unidadeTrabalho);

            dadosNFe = svcNFe.ObterDocumentoPorObjeto(retornoConsulta.Objeto.NotaFiscal, codigoEmpresa, unidadeTrabalho);
            erro = string.Empty;

            return true;
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal CriarXMLNotaFiscal(string cpfCnpjRementete, string cpfCnpjDestinatario, decimal peso, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();
            Dominio.Entidades.Cliente emitente = repCliente.BuscarPorCPFCNPJ(double.Parse(cpfCnpjRementete));

            Dominio.Entidades.Cliente destinatario;

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
                destinatario = repCliente.BuscarPorCPFCNPJ(double.Parse(cpfCnpjDestinatario));
            else
                destinatario = cargaPedido.Pedido.Destinatario;

            xmlNotaFiscal.TipoOperacaoNotaFiscal = emitente.Tipo != "E" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada;
            xmlNotaFiscal.Emitente = xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? emitente : destinatario;
            xmlNotaFiscal.Destinatario = xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? destinatario : emitente;
            xmlNotaFiscal.Filial = cargaPedido.Carga.Filial;
            xmlNotaFiscal.XML = "";
            xmlNotaFiscal.CNPJTranposrtador = "";
            xmlNotaFiscal.PlacaVeiculoNotaFiscal = "";
            xmlNotaFiscal.Rota = "";
            xmlNotaFiscal.NaturezaOP = "";
            xmlNotaFiscal.Peso = peso;
            xmlNotaFiscal.PesoBaseParaCalculo = xmlNotaFiscal.Peso;
            xmlNotaFiscal.Descricao = "";
            xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido;
            xmlNotaFiscal.Chave = "";
            xmlNotaFiscal.nfAtiva = true;

            return xmlNotaFiscal;
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal CriarXMLNotaFiscal(Dominio.ObjetosDeValor.Cliente objetoEmitente, Dominio.ObjetosDeValor.Cliente objetoDestinatario, decimal peso, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();

            Dominio.Entidades.Cliente emitente = new Dominio.Entidades.Cliente() { CPF_CNPJ = objetoEmitente.CPFCNPJ.ToDouble() };
            Dominio.Entidades.Cliente destinatario = new Dominio.Entidades.Cliente() { CPF_CNPJ = objetoDestinatario.CPFCNPJ.ToDouble() };

            xmlNotaFiscal.TipoOperacaoNotaFiscal = objetoEmitente.Tipo != "E" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada;
            xmlNotaFiscal.Emitente = xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? emitente : destinatario;
            xmlNotaFiscal.Destinatario = xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? destinatario : emitente;
            xmlNotaFiscal.Filial = cargaPedido.Carga.Filial;
            xmlNotaFiscal.XML = "";
            xmlNotaFiscal.CNPJTranposrtador = "";
            xmlNotaFiscal.PlacaVeiculoNotaFiscal = "";
            xmlNotaFiscal.Rota = "";
            xmlNotaFiscal.NaturezaOP = "";
            xmlNotaFiscal.Peso = peso;
            xmlNotaFiscal.PesoBaseParaCalculo = xmlNotaFiscal.Peso;
            xmlNotaFiscal.Descricao = "";
            xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido;
            xmlNotaFiscal.Chave = "";
            xmlNotaFiscal.nfAtiva = true;

            return xmlNotaFiscal;
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal CriarXMLNotaFiscal(Dominio.Entidades.Cliente emitente, Dominio.Entidades.Cliente destinatario, decimal peso, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();

            if (destinatario == null)
                destinatario = cargaPedido.Pedido.Destinatario;

            xmlNotaFiscal.TipoOperacaoNotaFiscal = emitente.Tipo != "E" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada;
            xmlNotaFiscal.Emitente = xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? emitente : destinatario;
            xmlNotaFiscal.Destinatario = xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? destinatario : emitente;
            xmlNotaFiscal.Filial = cargaPedido.Carga.Filial;
            xmlNotaFiscal.XML = "";
            xmlNotaFiscal.CNPJTranposrtador = "";
            xmlNotaFiscal.PlacaVeiculoNotaFiscal = "";
            xmlNotaFiscal.Rota = "";
            xmlNotaFiscal.NaturezaOP = "";
            xmlNotaFiscal.Peso = peso;
            xmlNotaFiscal.PesoBaseParaCalculo = xmlNotaFiscal.Peso;
            xmlNotaFiscal.Descricao = "";
            xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido;
            xmlNotaFiscal.Chave = "";
            xmlNotaFiscal.nfAtiva = true;

            return xmlNotaFiscal;
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal CriarXMLNotaFiscal(Dominio.Entidades.Cliente emitente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Embarcador.Filiais.Filial filial, string chave, int numeroNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            if (!string.IsNullOrWhiteSpace(chave))
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalExiste = repXMLNotaFiscal.BuscarPorChave(chave);
                if (xmlNotaFiscalExiste != null)
                    throw new Dominio.Excecoes.Embarcador.ServicoException($"Já existe um canhoto para essa nota fiscal: {chave}");
            }

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();

            xmlNotaFiscal.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;
            xmlNotaFiscal.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe;
            xmlNotaFiscal.Emitente = emitente;
            xmlNotaFiscal.Destinatario = destinatario;
            xmlNotaFiscal.Filial = filial;
            xmlNotaFiscal.XML = "";
            xmlNotaFiscal.CNPJTranposrtador = "";
            xmlNotaFiscal.PlacaVeiculoNotaFiscal = "";
            xmlNotaFiscal.Rota = "";
            xmlNotaFiscal.NaturezaOP = "";
            xmlNotaFiscal.Peso = 1;
            xmlNotaFiscal.PesoBaseParaCalculo = 1;
            xmlNotaFiscal.Descricao = "";
            xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido;
            xmlNotaFiscal.Chave = chave;
            xmlNotaFiscal.nfAtiva = true;
            xmlNotaFiscal.DataEmissao = DateTime.Now;
            xmlNotaFiscal.Numero = numeroNotaFiscal;

            repXMLNotaFiscal.Inserir(xmlNotaFiscal);
            return xmlNotaFiscal;
        }

        public Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal ConverterCanhotoEmNota(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = ConverterXMLEmNota(canhoto.XMLNotaFiscal, "", unitOfWork);
            notaFiscal.Canhoto = new Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto();
            notaFiscal.Canhoto.DataEnvioCanhoto = canhoto.DataEnvioCanhoto.ToString("dd/MM/yyyy");
            notaFiscal.Canhoto.Observacao = canhoto.Observacao;
            notaFiscal.Canhoto.SituacaoCanhoto = canhoto.SituacaoCanhoto;
            return notaFiscal;
        }

        public Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal ConverterXMLEmNota(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, string produtoPredominante, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = null, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> ctes = null, int codigoContaier = 0, int protocoloPedido = 0, Dominio.Entidades.Embarcador.Cargas.Carga carga = null)
        {
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);
            Servicos.WebService.Carga.Carga serWSCarga = new WebService.Carga.Carga(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.ContainerCTE repContainerCTE = new Repositorio.ContainerCTE(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal();
            notaFiscal.BaseCalculoICMS = xmlNotaFiscal.BaseCalculoICMS;
            notaFiscal.BaseCalculoST = xmlNotaFiscal.BaseCalculoST;
            notaFiscal.Chave = xmlNotaFiscal.Chave;
            notaFiscal.DataEmissao = xmlNotaFiscal.DataEmissao.ToString("dd/MM/yyyy HH:mm");
            notaFiscal.Destinatario = serPessoa.ConverterObjetoPessoa(xmlNotaFiscal.Destinatario);
            notaFiscal.Emitente = serPessoa.ConverterObjetoPessoa(xmlNotaFiscal.Emitente);
            notaFiscal.InformacoesComplementares = "";
            if ((int)xmlNotaFiscal.ModalidadeFrete == 3 || (int)xmlNotaFiscal.ModalidadeFrete == 4)
                notaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Outros;
            else
                notaFiscal.ModalidadeFrete = xmlNotaFiscal.ModalidadeFrete;
            notaFiscal.MetroCubico = xmlNotaFiscal.MetrosCubicos;
            notaFiscal.Modelo = xmlNotaFiscal.Modelo;
            notaFiscal.NaturezaOP = xmlNotaFiscal.NaturezaOP;
            notaFiscal.Numero = xmlNotaFiscal.Numero;
            notaFiscal.PesoBruto = xmlNotaFiscal.Peso;
            notaFiscal.PesoLiquido = xmlNotaFiscal.PesoLiquido;
            notaFiscal.Protocolo = xmlNotaFiscal.Codigo;
            notaFiscal.ProtocoloPedido = protocoloPedido;
            notaFiscal.Rota = "";
            notaFiscal.Serie = xmlNotaFiscal.Serie;
            notaFiscal.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;
            notaFiscal.TipoOperacaoNotaFiscal = xmlNotaFiscal.TipoOperacaoNotaFiscal;
            notaFiscal.Valor = xmlNotaFiscal.Valor;
            notaFiscal.ValorCOFINS = xmlNotaFiscal.ValorCOFINS;
            notaFiscal.ValorDesconto = xmlNotaFiscal.ValorDesconto;
            notaFiscal.ValorFrete = xmlNotaFiscal.ValorFrete;
            notaFiscal.ValorICMS = xmlNotaFiscal.ValorICMS;
            notaFiscal.ValorImpostoImportacao = xmlNotaFiscal.ValorImpostoImportacao;
            notaFiscal.ValorIPI = xmlNotaFiscal.ValorIPI;
            notaFiscal.ValorOutros = xmlNotaFiscal.ValorOutros;
            notaFiscal.ValorPIS = xmlNotaFiscal.ValorOutros;
            notaFiscal.ValorSeguro = xmlNotaFiscal.ValorSeguro;
            notaFiscal.ValorST = xmlNotaFiscal.ValorST;
            notaFiscal.ValorTotalProdutos = xmlNotaFiscal.ValorTotalProdutos;
            notaFiscal.VolumesTotal = xmlNotaFiscal.Volumes;
            notaFiscal.CFOPPredominante = xmlNotaFiscal.CFOP;
            notaFiscal.NCMPredominante = xmlNotaFiscal.NCM;
            if (string.IsNullOrWhiteSpace(notaFiscal.NCMPredominante) && !string.IsNullOrWhiteSpace(xmlNotaFiscal.XML) && xmlNotaFiscal.XML.Contains("NCM"))
            {
                if (xmlNotaFiscal.XML.Contains("<NCM>"))
                    notaFiscal.NCMPredominante = xmlNotaFiscal.XML.Substring(xmlNotaFiscal.XML.IndexOf("<NCM>") + 6, 4);
                else if (xmlNotaFiscal.XML.Contains("'NCM':"))
                    notaFiscal.NCMPredominante = xmlNotaFiscal.XML.Substring(xmlNotaFiscal.XML.IndexOf("'NCM':") + 6, 4);
            }
            if (string.IsNullOrWhiteSpace(notaFiscal.NCMPredominante) && produtoEmbarcador != null && !string.IsNullOrWhiteSpace(produtoEmbarcador.CodigoNCM) && produtoEmbarcador.CodigoNCM.Length == 4)
                notaFiscal.NCMPredominante = produtoEmbarcador.CodigoNCM;
            notaFiscal.CodigoProduto = produtoEmbarcador != null ? produtoEmbarcador.CodigoDocumentacao : "";

            notaFiscal.NumeroControleCliente = xmlNotaFiscal.NumeroControleCliente;
            if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.NumeroCanhoto))
                notaFiscal.NumeroCanhoto = xmlNotaFiscal.NumeroCanhoto;
            else if (xmlNotaFiscal.Numero > 0 && xmlNotaFiscal.Emitente != null)
                notaFiscal.NumeroCanhoto = xmlNotaFiscal.Numero.ToString("D").PadLeft(9, '0') + xmlNotaFiscal.SerieOuSerieDaChave.PadLeft(3, '0') + xmlNotaFiscal.Emitente.CPF_CNPJ_SemFormato.PadLeft(14, '0');
            notaFiscal.NumeroReferenciaEDI = xmlNotaFiscal.NumeroReferenciaEDI;
            notaFiscal.PINSuframa = xmlNotaFiscal.PINSUFRAMA;
            notaFiscal.PesoAferido = xmlNotaFiscal.Peso;
            notaFiscal.DescricaoMercadoria = produtoPredominante;
            notaFiscal.Volumes = null;
            notaFiscal.FormaIntegracao = carga?.FormaIntegracao;

            notaFiscal.Containeres = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Container>();
            if (ctes != null && ctes.Count > 0)
            {
                foreach (var cte in ctes)
                {
                    if (cte.CargaCTe != null && cte.CargaCTe.CTe != null && cte.CargaCTe.CTe.Containers != null && cte.CargaCTe.CTe.Containers.Count > 0)
                    {
                        foreach (var container in cte.CargaCTe.CTe.Containers)
                        {
                            if (container.Container != null && container.Documentos != null && container.Documentos.Count > 0 && !string.IsNullOrWhiteSpace(notaFiscal.Chave) && container.Documentos.Any(d => d.Chave == notaFiscal.Chave) && cte.CargaCTe.CTe.Status == "A")
                            {
                                Dominio.ObjetosDeValor.Embarcador.Carga.Container cont = new Dominio.ObjetosDeValor.Embarcador.Carga.Container();
                                cont = serWSCarga.ConverterObjetoContainer(container.Container);
                                if (cont != null)
                                {
                                    cont.Lacre1 = container.Lacre1;
                                    cont.Lacre2 = container.Lacre2;
                                    cont.Lacre3 = container.Lacre3;
                                    cont.Volume = 0;
                                    if (produtoEmbarcador != null && produtoEmbarcador.MetroCubito > 0)
                                    {
                                        decimal pesoContainer = repCTe.BuscarPesoNotasConhecimento(container.Codigo, cte.CargaCTe.CTe.Codigo);

                                        if (pesoContainer <= 0)
                                            pesoContainer = repCTe.BuscarPesoBrutoContainer(container.Codigo, cte.CargaCTe.CTe.Codigo);

                                        if (pesoContainer <= 0 && cte.CargaCTe.CTe.QuantidadesCarga != null && cte.CargaCTe.CTe.QuantidadesCarga.Any(o => o.UnidadeMedida == "01"))
                                            pesoContainer = cte.CargaCTe.CTe.QuantidadesCarga.Where(o => o.UnidadeMedida == "01").Sum(o => o.Quantidade);

                                        if (pesoContainer > 0)
                                        {
                                            cont.Volume = pesoContainer / produtoEmbarcador.MetroCubito;
                                            cont.PesoLiquido = pesoContainer;
                                            cont.DencidadeProduto = produtoEmbarcador.MetroCubito;
                                        }
                                    }
                                    notaFiscal.Containeres.Add(cont);
                                }
                            }
                            else if (codigoContaier > 0 && container.Container != null && container.Container.Codigo == codigoContaier)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Carga.Container cont = new Dominio.ObjetosDeValor.Embarcador.Carga.Container();
                                cont = serWSCarga.ConverterObjetoContainer(container.Container);
                                if (cont != null)
                                {
                                    cont.Lacre1 = container.Lacre1;
                                    cont.Lacre2 = container.Lacre2;
                                    cont.Lacre3 = container.Lacre3;
                                    cont.Volume = 0;
                                    if (produtoEmbarcador != null && produtoEmbarcador.MetroCubito > 0)
                                    {
                                        decimal pesoContainer = repCTe.BuscarPesoNotasConhecimento(container.Codigo, cte.CargaCTe.CTe.Codigo);

                                        if (pesoContainer <= 0)
                                            pesoContainer = repCTe.BuscarPesoBrutoContainer(container.Codigo, cte.CargaCTe.CTe.Codigo);

                                        if (pesoContainer <= 0 && cte.CargaCTe.CTe.QuantidadesCarga != null && cte.CargaCTe.CTe.QuantidadesCarga.Any(o => o.UnidadeMedida == "01"))
                                            pesoContainer = cte.CargaCTe.CTe.QuantidadesCarga.Where(o => o.UnidadeMedida == "01").Sum(o => o.Quantidade);

                                        if (pesoContainer > 0)
                                        {
                                            cont.Volume = pesoContainer / produtoEmbarcador.MetroCubito;
                                            cont.PesoLiquido = pesoContainer;
                                            cont.DencidadeProduto = produtoEmbarcador.MetroCubito;
                                        }
                                    }
                                    notaFiscal.Containeres.Add(cont);
                                }
                            }
                            //else if (codigoContaier > 0)
                            //{
                            //    Dominio.Entidades.ContainerCTE containerCTE = repContainerCTE.BuscarPorContainerENota(codigoContaier, xmlNotaFiscal.Chave);
                            //    Dominio.ObjetosDeValor.Embarcador.Carga.Container cont = new Dominio.ObjetosDeValor.Embarcador.Carga.Container();
                            //    cont = serWSCarga.ConverterObjetoContainer(containerCTE.Container);
                            //    if (cont != null)
                            //    {
                            //        cont.Lacre1 = containerCTE.Lacre1;
                            //        cont.Lacre2 = containerCTE.Lacre2;
                            //        cont.Lacre3 = containerCTE.Lacre3;
                            //        cont.Volume = 0;
                            //        if (produtoEmbarcador != null && produtoEmbarcador.MetroCubito > 0)
                            //        {
                            //            decimal pesoContainer = repCTe.BuscarPesoNotasConhecimento(containerCTE.Codigo, cte.CargaCTe.CTe.Codigo);

                            //            if (pesoContainer <= 0)
                            //                pesoContainer = repCTe.BuscarPesoBrutoContainer(containerCTE.Codigo, cte.CargaCTe.CTe.Codigo);

                            //            if (pesoContainer <= 0 && cte.CargaCTe.CTe.QuantidadesCarga != null && cte.CargaCTe.CTe.QuantidadesCarga.Any(o => o.UnidadeMedida == "01"))
                            //                pesoContainer = cte.CargaCTe.CTe.QuantidadesCarga.Where(o => o.UnidadeMedida == "01").Sum(o => o.Quantidade);

                            //            if (pesoContainer > 0)
                            //            {
                            //                cont.Volume = pesoContainer / produtoEmbarcador.MetroCubito;
                            //                cont.PesoLiquido = pesoContainer;
                            //                cont.DencidadeProduto = produtoEmbarcador.MetroCubito;
                            //            }
                            //        }
                            //        notaFiscal.Containeres.Add(cont);
                            //    }
                            //}
                        }

                    }
                }
            }

            return notaFiscal;
        }

        public Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal ConverterXMLEmNota(Dominio.Entidades.DocumentosCTE xmlNotaFiscal, string produtoPredominante, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = null, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null)
        {
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);
            Servicos.WebService.Carga.Carga serWSCarga = new WebService.Carga.Carga(unitOfWork);

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal();
            notaFiscal.BaseCalculoICMS = xmlNotaFiscal.BaseCalculoICMS;
            notaFiscal.BaseCalculoST = xmlNotaFiscal.BaseCalculoICMSST;
            notaFiscal.Chave = xmlNotaFiscal.ChaveNFE;
            notaFiscal.DataEmissao = xmlNotaFiscal.DataEmissao.ToString("dd/MM/yyyy HH:mm");
            notaFiscal.Destinatario = null;// serPessoa.ConverterObjetoPessoa(xmlNotaFiscal.Destinatario);
            if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.CNPJRemetente))
            {
                double.TryParse(xmlNotaFiscal.CNPJRemetente, out double cnpjRemetente);
                if (cnpjRemetente > 0)
                {
                    Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(cnpjRemetente);
                    if (remetente != null)
                        notaFiscal.Emitente = serPessoa.ConverterObjetoPessoa(remetente);
                }
            }
            notaFiscal.InformacoesComplementares = "";
            notaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Outros;
            notaFiscal.Modelo = xmlNotaFiscal.NumeroModelo;
            notaFiscal.NaturezaOP = null;//xmlNotaFiscal.NaturezaOP;
            if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(xmlNotaFiscal.Numero)))
            {
                int.TryParse(Utilidades.String.OnlyNumbers(xmlNotaFiscal.Numero), out int numeroNota);
                notaFiscal.Numero = numeroNota;
            }
            notaFiscal.MetroCubico = 0m;
            notaFiscal.PesoBruto = xmlNotaFiscal.Peso;
            notaFiscal.PesoLiquido = xmlNotaFiscal.Peso;
            notaFiscal.Protocolo = xmlNotaFiscal.Codigo;
            notaFiscal.Rota = "";
            notaFiscal.Serie = xmlNotaFiscal.Serie;
            notaFiscal.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;
            notaFiscal.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;
            notaFiscal.Valor = xmlNotaFiscal.Valor;
            notaFiscal.ValorCOFINS = 0;// xmlNotaFiscal.ValorCOFINS;
            notaFiscal.ValorDesconto = 0;// xmlNotaFiscal.ValorDesconto;
            notaFiscal.ValorFrete = 0;// xmlNotaFiscal.ValorFrete;
            notaFiscal.ValorICMS = xmlNotaFiscal.ValorICMS;
            notaFiscal.ValorImpostoImportacao = 0;// xmlNotaFiscal.ValorImpostoImportacao;
            notaFiscal.ValorIPI = 0;// xmlNotaFiscal.ValorIPI;
            notaFiscal.ValorOutros = 0;//xmlNotaFiscal.ValorOutros;
            notaFiscal.ValorPIS = 0;// xmlNotaFiscal.ValorOutros;
            notaFiscal.ValorSeguro = 0;// xmlNotaFiscal.ValorSeguro;
            notaFiscal.ValorST = 0;// xmlNotaFiscal.ValorST;
            notaFiscal.ValorTotalProdutos = 0;//xmlNotaFiscal.ValorTotalProdutos;
            notaFiscal.VolumesTotal = xmlNotaFiscal.Volume;
            notaFiscal.CFOPPredominante = xmlNotaFiscal.CFOP;
            notaFiscal.NCMPredominante = xmlNotaFiscal.NCMPredominante;
            notaFiscal.NumeroReferenciaEDI = xmlNotaFiscal.NumeroReferenciaEDI;
            notaFiscal.NumeroControleCliente = xmlNotaFiscal.NumeroControleCliente;

            if (string.IsNullOrWhiteSpace(notaFiscal.NCMPredominante) || string.IsNullOrWhiteSpace(notaFiscal.NumeroReferenciaEDI))
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNota = repXMLNotaFiscal.BuscarPorChave(xmlNotaFiscal.ChaveNFE);
                if (xmlNota != null && string.IsNullOrWhiteSpace(notaFiscal.NCMPredominante) && !string.IsNullOrWhiteSpace(xmlNota.XML) && xmlNota.XML.Contains("NCM"))
                {
                    if (xmlNota.XML.Contains("<NCM>"))
                        notaFiscal.NCMPredominante = xmlNota.XML.Substring(xmlNota.XML.IndexOf("<NCM>") + 6, 4);
                    else if (xmlNota.XML.Contains("\"NCM\":"))
                        notaFiscal.NCMPredominante = xmlNota.XML.Substring(xmlNota.XML.IndexOf("\"NCM\":") + 6, 4);
                }
                if (xmlNota != null && string.IsNullOrWhiteSpace(notaFiscal.NumeroReferenciaEDI) && !string.IsNullOrWhiteSpace(xmlNota.NumeroReferenciaEDI))
                    notaFiscal.NumeroReferenciaEDI = xmlNota.NumeroReferenciaEDI;
            }

            if (string.IsNullOrWhiteSpace(notaFiscal.NCMPredominante) && produtoEmbarcador != null && !string.IsNullOrWhiteSpace(produtoEmbarcador.CodigoNCM) && produtoEmbarcador.CodigoNCM.Length == 4)
                notaFiscal.NCMPredominante = produtoEmbarcador.CodigoNCM;
            notaFiscal.CodigoProduto = produtoEmbarcador != null ? produtoEmbarcador.CodigoDocumentacao : "";

            if (string.IsNullOrWhiteSpace(notaFiscal.NumeroControleCliente))
                notaFiscal.NumeroControleCliente = xmlNotaFiscal.NumeroRomaneio;
            if (notaFiscal.Numero > 0 && notaFiscal.Emitente != null)
                notaFiscal.NumeroCanhoto = notaFiscal.Numero.ToString("D").PadLeft(9, '0') + xmlNotaFiscal.SerieOuSerieDaChave.PadLeft(3, '0') + notaFiscal.Emitente.CPFCNPJSemFormato.PadLeft(14, '0');

            notaFiscal.PINSuframa = "";// xmlNotaFiscal.PINSUFRAMA;
            notaFiscal.PesoAferido = xmlNotaFiscal.Peso;
            notaFiscal.DescricaoMercadoria = produtoPredominante;
            notaFiscal.Volumes = null;

            notaFiscal.Containeres = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Container>();
            if (cte != null && cte.Containers != null && cte.Containers.Count > 0)
            {
                foreach (var container in cte.Containers)
                {
                    if (container.Container != null && container.Documentos != null && container.Documentos.Count > 0 && container.Documentos.Any(d => d.Chave == notaFiscal.Chave))
                    {
                        Dominio.ObjetosDeValor.Embarcador.Carga.Container cont = new Dominio.ObjetosDeValor.Embarcador.Carga.Container();
                        cont = serWSCarga.ConverterObjetoContainer(container.Container);
                        if (cont != null)
                        {
                            cont.Lacre1 = container.Lacre1;
                            cont.Lacre2 = container.Lacre2;
                            cont.Lacre3 = container.Lacre3;
                            cont.Volume = 0;
                            if (produtoEmbarcador != null && produtoEmbarcador.MetroCubito > 0)
                            {
                                decimal pesoContainer = repCTe.BuscarPesoNotasConhecimento(container.Codigo, cte.Codigo);

                                if (pesoContainer <= 0)
                                    pesoContainer = repCTe.BuscarPesoBrutoContainer(container.Codigo, cte.Codigo);

                                if (pesoContainer <= 0 && cte.QuantidadesCarga != null && cte.QuantidadesCarga.Any(o => o.UnidadeMedida == "01"))
                                    pesoContainer = cte.QuantidadesCarga.Where(o => o.UnidadeMedida == "01").Sum(o => o.Quantidade);

                                if (pesoContainer > 0)
                                {
                                    cont.Volume = pesoContainer / produtoEmbarcador.MetroCubito;
                                    cont.PesoLiquido = pesoContainer;
                                    cont.DencidadeProduto = produtoEmbarcador.MetroCubito;
                                }
                            }
                            notaFiscal.Containeres.Add(cont);
                        }
                    }
                }
            }

            return notaFiscal;
        }

        public void VincularNotasDeRedespachoVinculadas(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoVinculada, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
            Servicos.Embarcador.Pedido.Produto serProduto = new Servicos.Embarcador.Pedido.Produto(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repConfiguracaoCanhoto.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidoXMLNotaFiscaisVinculadas = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedidoVinculada.Codigo);

            if (cargaPedidoXMLNotaFiscaisVinculadas.Count > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> produtos = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos>();
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal cargaPedidoXMLNotaFiscalVinculado in cargaPedidoXMLNotaFiscaisVinculadas)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal cargaPedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCargaPedidoEXMLNotaFiscal(cargaPedido.Codigo, cargaPedidoXMLNotaFiscalVinculado.XMLNotaFiscal.Codigo);
                    if (cargaPedidoXMLNotaFiscal == null)
                    {
                        cargaPedidoXMLNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal();
                        cargaPedidoXMLNotaFiscal.XMLNotaFiscal = cargaPedidoXMLNotaFiscalVinculado.XMLNotaFiscal;
                        cargaPedidoXMLNotaFiscal.CargaPedido = cargaPedido;
                        repPedidoXMLNotaFiscal.Inserir(cargaPedidoXMLNotaFiscal);

                        serCanhoto.SalvarCanhotoNota(cargaPedidoXMLNotaFiscal.XMLNotaFiscal, cargaPedido, cargaPedido.Carga.FreteDeTerceiro && cargaPedido.Carga.Veiculo != null ? cargaPedido.Carga.Veiculo.Proprietario : cargaPedido.Carga.ProvedorOS, cargaPedido.Carga.Motoristas != null ? cargaPedido.Carga.Motoristas.ToList() : new List<Dominio.Entidades.Usuario>(), tipoServicoMultisoftware, configuracaoTMS, unitOfWork, configuracaoCanhoto);

                        if (cargaPedido.Carga.ExigeNotaFiscalParaCalcularFrete && !string.IsNullOrWhiteSpace(cargaPedidoXMLNotaFiscalVinculado.XMLNotaFiscal?.XML))
                        {
                            byte[] byteArray = Encoding.UTF8.GetBytes(cargaPedidoXMLNotaFiscalVinculado.XMLNotaFiscal.XML);
                            MemoryStream stream = new MemoryStream(byteArray);
                            Servicos.NFe svcNFe = new Servicos.NFe(unitOfWork);
                            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe nfXml = svcNFe.ObterDocumentoPorXML(stream, unitOfWork);

                            produtos.AddRange(nfXml.Produtos);
                        }
                    }
                }

                if (produtos.Count > 0)
                    serProduto.AtualizarProdutosCargaPedidoPorNotaFiscal(produtos, cargaPedido, unitOfWork, Auditado);

                decimal peso = 0m;
                decimal pesoLiquido = 0m;

                serCargaPedido.AdicionarCargaPedidoQuantidades(cargaPedido, null, null, tipoServicoMultisoftware, configuracaoTMS, ref peso, ref pesoLiquido, configuracaoTMS.NaoAtualizarPesoPedidoPelaNFe, unitOfWork);

                repCargaPedido.Atualizar(cargaPedido);

                cargaPedido.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada;
                repCargaPedido.Atualizar(cargaPedido);

                if (repCargaPedido.VerificarSeTodosPedidosEstaoAutorizadosPorCarga(cargaPedido.Carga.Codigo))
                {
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    cargaPedido.Carga.DataEnvioUltimaNFe = DateTime.Now;
                    cargaPedido.Carga.DataRecebimentoUltimaNFe = DateTime.Now;
                    cargaPedido.Carga.DataInicioEmissaoDocumentos = DateTime.Now;

                    if (cargaPedido.Carga.ExigeNotaFiscalParaCalcularFrete)
                    {
                        if (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
                        {
                            bool liberar = true;
                            if (cargaPedido.PossuiNFS)
                            {
                                Servicos.Embarcador.NFSe.NFSe serNFSe = new Servicos.Embarcador.NFSe.NFSe(unitOfWork);
                                Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

                                Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe configNFSe = serNFSe.BuscarConfiguracaoEmissaoNFSe(cargaPedido.Carga.Empresa.Codigo, cargaPedido.Destino.Codigo, tomador?.Localidade?.Estado?.Sigla ?? "", tomador?.GrupoPessoas?.Codigo ?? 0, tomador?.Localidade?.Codigo ?? 0, cargaPedido.Carga.TipoOperacao?.Codigo ?? 0, tomador?.CPF_CNPJ ?? 0, 0, unitOfWork);
                                if (configNFSe == null)
                                    liberar = false;

                            }
                            cargaPedido.Carga.ProcessandoDocumentosFiscais = true;

                            if (liberar)
                            {
                                cargaPedido.Carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.NenhumPendencia;
                                cargaPedido.Carga.PossuiPendencia = false;
                                cargaPedido.Carga.MotivoPendencia = "";
                            }
                            else
                            {
                                cargaPedido.Carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.ProblemaCalculoFrete;
                                cargaPedido.Carga.PossuiPendencia = true;
                                cargaPedido.Carga.MotivoPendencia = "O transportado informado não está apto a emitir notas fiscais de serviço em " + cargaPedido.Destino.DescricaoCidadeEstado + ".";
                            }
                        }
                    }

                    repCarga.Atualizar(cargaPedido.Carga);
                }
            }
        }

        public void VerificarSeNecessariaAutorizacaoModalidadePagamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoModalidadePagamentoNFAprovacao repCargaPedidoModalidadePagamentoNFAprovacao = new Repositorio.Embarcador.Cargas.CargaPedidoModalidadePagamentoNFAprovacao(unitOfWork);

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoModalidadePagamentoNFAprovacao> cargasPedidoModalidadePagamentoNFAprovacao = repCargaPedidoModalidadePagamentoNFAprovacao.BuscarPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                bool existe = true;
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoModalidadePagamentoNFAprovacao cargaPedidoModalidadePagamentoNFAprovacao = (from obj in cargasPedidoModalidadePagamentoNFAprovacao where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).FirstOrDefault();
                if (cargaPedidoModalidadePagamentoNFAprovacao == null)
                {
                    existe = false;
                    cargaPedidoModalidadePagamentoNFAprovacao = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoModalidadePagamentoNFAprovacao();
                }

                if (cargaPedido.Pedido.UsarTipoPagamentoNF)
                {
                    int quantidade = repPedidoXMLNotaFiscal.ContarModalidadesDePagamentoPorCargaPedido(cargaPedido.Codigo);
                    if (quantidade > 1)
                    {
                        cargaPedidoModalidadePagamentoNFAprovacao.SituacaoAutorizacaoModalidadePagamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoModalidadePagamento.AgLiberacao;
                        cargaPedidoModalidadePagamentoNFAprovacao.MotivoExtornoAutorizacao = "";
                        cargaPedidoModalidadePagamentoNFAprovacao.DataHora = DateTime.Now;
                        cargaPedidoModalidadePagamentoNFAprovacao.CargaPedido = cargaPedido;
                        if (existe)
                            repCargaPedidoModalidadePagamentoNFAprovacao.Atualizar(cargaPedidoModalidadePagamentoNFAprovacao);
                        else
                            repCargaPedidoModalidadePagamentoNFAprovacao.Inserir(cargaPedidoModalidadePagamentoNFAprovacao);
                    }
                    else
                    {
                        if (existe)
                        {
                            cargaPedidoModalidadePagamentoNFAprovacao.SituacaoAutorizacaoModalidadePagamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoModalidadePagamento.AutorizacaoExtornada;
                            cargaPedidoModalidadePagamentoNFAprovacao.MotivoExtornoAutorizacao = "Não existem mais notas com diferentes formas de pagamento para o pedido.";
                            repCargaPedidoModalidadePagamentoNFAprovacao.Atualizar(cargaPedidoModalidadePagamentoNFAprovacao);
                        }
                    }
                }
                else
                {
                    if (existe)
                    {
                        cargaPedidoModalidadePagamentoNFAprovacao.SituacaoAutorizacaoModalidadePagamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoModalidadePagamento.AutorizacaoExtornada;
                        cargaPedidoModalidadePagamentoNFAprovacao.MotivoExtornoAutorizacao = "O pedido não utiliza mais a forma de pagamento das notas.";
                        repCargaPedidoModalidadePagamentoNFAprovacao.Atualizar(cargaPedidoModalidadePagamentoNFAprovacao);
                    }
                }
            }
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarDadosNotaFiscal(string objetoJSON, Repositorio.UnitOfWork unitOfWork)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");

            dynamic objNFe = null;
            try
            {
                objNFe = JsonConvert.DeserializeObject<dynamic>(objetoJSON);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return null;
            }

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            Dominio.Entidades.Cliente destinatario = null;
            Dominio.Entidades.Cliente remetente = null;

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Servicos.NFe serNFe = new Servicos.NFe(unitOfWork);

            bool inserirDestinatario = false;
            string ieDestinatario = "";
            string ieEmitente = "";
            string nomeDestinatario = objNFe.nfeProc.NFe.infNFe.dest.xNome;
            nomeDestinatario = Utilidades.String.RemoverCaracteresEspecialSerpro(nomeDestinatario.Replace("&amp;", "")?.Replace(" amp ", ""));
            string cgcDestinatario = objNFe.nfeProc.NFe.infNFe.dest.CNPJ;
            if (string.IsNullOrWhiteSpace(cgcDestinatario))
                cgcDestinatario = objNFe.nfeProc.NFe.infNFe.dest.CPF;
            if (string.IsNullOrWhiteSpace(cgcDestinatario))
                cgcDestinatario = "00000000000000";
            try
            {
                ieDestinatario = objNFe.nfeProc.NFe.infNFe.dest.IE;
                ieEmitente = objNFe.nfeProc.NFe.infNFe.emit.IE;
            }
            catch
            {
            }

            string cgcRemetente = objNFe.nfeProc.NFe.infNFe.emit.CNPJ;
            if (string.IsNullOrWhiteSpace(cgcRemetente))
                cgcRemetente = objNFe.nfeProc.NFe.infNFe.emit.CPF;

            remetente = serNFe.ObterEmitente(objNFe.nfeProc.NFe.infNFe.emit, 0);//repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(cgcRemetente)));
            if (remetente == null)
            {
                Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);

                remetente = new Dominio.Entidades.Cliente();
                remetente.CPF_CNPJ = double.Parse(Utilidades.String.OnlyNumbers(cgcRemetente));
                if (Utilidades.String.OnlyNumbers(cgcRemetente).Length == 11)
                    remetente.Tipo = "F";
                else
                    remetente.Tipo = "J";

                remetente.Nome = (Utilidades.String.RemoverCaracteresEspecialSerpro((string)objNFe.nfeProc.NFe.infNFe.emit.xNome).Replace("&amp;", "")?.Replace(" amp ", ""));
                remetente.Endereco = objNFe.nfeProc.NFe.infNFe.emit.enderEmit.xLgr;
                remetente.Bairro = objNFe.nfeProc.NFe.infNFe.emit.enderEmit.xBairro;
                remetente.Cidade = objNFe.nfeProc.NFe.infNFe.emit.enderEmit.cMun;
                remetente.Complemento = objNFe.nfeProc.NFe.infNFe.emit.enderEmit.xCpl;
                remetente.Email = "";
                remetente.Numero = objNFe.nfeProc.NFe.infNFe.emit.enderEmit.nro;
                remetente.Pais = objNFe.nfeProc.NFe.infNFe.emit.enderEmit.cPais != null && (int)objNFe.nfeProc.NFe.infNFe.emit.enderEmit.cPais > 0 ? repPais.BuscarPorCodigo((int)objNFe.nfeProc.NFe.infNFe.emit.enderEmit.cPais) : null;
                remetente.Localidade = objNFe.nfeProc.NFe.infNFe.emit.enderEmit.cMun != null && (int)objNFe.nfeProc.NFe.infNFe.emit.enderEmit.cMun > 0 ? repLocalidade.BuscarPorCodigoIBGE((int)objNFe.nfeProc.NFe.infNFe.emit.enderEmit.cMun) : repLocalidade.BuscarPorCodigoIBGE(9999999);
                remetente.Atividade = repAtividade.BuscarPorCodigo(1);
                remetente.Ativo = true;
                repCliente.Inserir(remetente);
            }

            if (!string.IsNullOrWhiteSpace(cgcDestinatario))
            {
                if (cgcDestinatario == "00000000000000")
                {
                    string nome = (Utilidades.String.RemoverCaracteresEspecialSerpro((string)objNFe.nfeProc.NFe.infNFe.dest.xNome).Replace("&amp;", "")?.Replace(" amp ", ""));
                    string endereco = (string)objNFe.nfeProc.NFe.infNFe.dest.enderDest.xLgr;
                    destinatario = repCliente.BuscarPorNomeEndereco(nome, endereco);
                    if (destinatario == null)
                    {
                        destinatario = new Dominio.Entidades.Cliente();
                        destinatario.Tipo = "E";
                        destinatario.CPF_CNPJ = repCliente.BuscarPorProximoExterior();
                        inserirDestinatario = true;
                    }
                }
                else
                {
                    destinatario = serNFe.ObterDestinatario(objNFe.nfeProc.NFe.infNFe.dest, 0); //repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(cgcDestinatario)));
                    if (destinatario == null)
                    {
                        destinatario = new Dominio.Entidades.Cliente();
                        destinatario.CPF_CNPJ = double.Parse(Utilidades.String.OnlyNumbers(cgcDestinatario));
                        inserirDestinatario = true;
                        if (Utilidades.String.OnlyNumbers(cgcDestinatario).Length == 11)
                            destinatario.Tipo = "F";
                        else
                            destinatario.Tipo = "J";
                    }

                }
                if (inserirDestinatario)
                {
                    Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
                    Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                    Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);

                    destinatario.Nome = (Utilidades.String.RemoverCaracteresEspecialSerpro((string)(objNFe.nfeProc.NFe.infNFe.dest.xNome)).Replace("&amp;", "")?.Replace(" amp ", ""));
                    destinatario.Endereco = objNFe.nfeProc.NFe.infNFe.dest.enderDest.xLgr;
                    destinatario.Bairro = objNFe.nfeProc.NFe.infNFe.dest.enderDest.xBairro;
                    destinatario.Cidade = objNFe.nfeProc.NFe.infNFe.dest.enderDest.cMun;
                    destinatario.Complemento = objNFe.nfeProc.NFe.infNFe.dest.enderDest.xCpl;
                    destinatario.Email = "";
                    destinatario.Numero = objNFe.nfeProc.NFe.infNFe.dest.enderDest.nro;
                    destinatario.Pais = objNFe.nfeProc.NFe.infNFe.dest.enderDest.cPais != null && (int)objNFe.nfeProc.NFe.infNFe.dest.enderDest.cPais > 0 ? repPais.BuscarPorCodigo((int)objNFe.nfeProc.NFe.infNFe.dest.enderDest.cPais) : null;
                    destinatario.Localidade = objNFe.nfeProc.NFe.infNFe.dest.enderDest.cMun != null && (int)objNFe.nfeProc.NFe.infNFe.dest.enderDest.cMun > 0 ? repLocalidade.BuscarPorCodigoIBGE((int)objNFe.nfeProc.NFe.infNFe.dest.enderDest.cMun) : repLocalidade.BuscarPorCodigoIBGE(9999999);
                    destinatario.Atividade = repAtividade.BuscarPorCodigo(1);
                    destinatario.Ativo = true;
                    repCliente.Inserir(destinatario);
                }
            }

            var notaFiscal = new
            {
                Empresa = (string)objNFe.nfeProc.NFe.infNFe.emit.CNPJ,
                CNPJTransportador = objNFe.nfeProc.NFe.infNFe.transp.transportadora != null ? (string)objNFe.nfeProc.NFe.infNFe.transp?.transporta?.CNPJ ?? "" : "",
                Placa = "",
                DataEmissao = ((DateTime)objNFe.nfeProc.NFe.infNFe.ide.dhEmi).ToString("dd/MM/yyyy HH:mm:ss"),
                Chave = (string)objNFe.nfeProc.protNFe.infProt.chNFe,
                Numero = (string)objNFe.nfeProc.NFe.infNFe.ide.nNF,
                Modelo = (string)objNFe.nfeProc.NFe.infNFe.ide.mod,
                Serie = (string)objNFe.nfeProc.NFe.infNFe.ide.serie,
                Remetente = remetente,
                ValorTotal = (decimal)(objNFe.nfeProc.NFe.infNFe.total.ICMSTot.vNF),
                BaseCalculoICMS = (decimal)(objNFe.nfeProc.NFe.infNFe.total.ICMSTot.vBC),
                ValorICMS = (decimal)(objNFe.nfeProc.NFe.infNFe.total.ICMSTot.vICMS),
                BaseCalculoST = (decimal)(objNFe.nfeProc.NFe.infNFe.total.ICMSTot.vBCST),
                ValorST = (decimal)(objNFe.nfeProc.NFe.infNFe.total.ICMSTot.vST),
                ValorTotalProdutos = (decimal)(objNFe.nfeProc.NFe.infNFe.total.ICMSTot.vProd),
                ValorSeguro = (decimal)(objNFe.nfeProc.NFe.infNFe.total.ICMSTot.vSeg),
                ValorDesconto = (decimal)(objNFe.nfeProc.NFe.infNFe.total.ICMSTot.vDesc),
                ValorImpostoImportacao = (decimal)(objNFe.nfeProc.NFe.infNFe.total.ICMSTot.vII),
                ValorIPI = (decimal)(objNFe.nfeProc.NFe.infNFe.total.ICMSTot.vIPI),
                ValorPIS = (decimal)(objNFe.nfeProc.NFe.infNFe.total.ICMSTot.vPIS),
                ValorCOFINS = (decimal)(objNFe.nfeProc.NFe.infNFe.total.ICMSTot.vCOFINS),
                ValorOutros = (decimal)(objNFe.nfeProc.NFe.infNFe.total.ICMSTot.vOutro),
                TipoOperacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal)(int)(objNFe.nfeProc.NFe.infNFe.ide.tpNF),
                NaturezaOP = (string)objNFe.nfeProc.NFe.infNFe.ide.natOp,
                Volumes = objNFe.nfeProc.NFe.infNFe.transp != null && objNFe.nfeProc.NFe.infNFe.transp.vol != null ? objNFe.nfeProc.NFe.infNFe.transp.vol : null,
                ModalidadeFrete = objNFe.nfeProc.NFe.infNFe.transp != null ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete)(int)objNFe.nfeProc.NFe.infNFe.transp.modFrete : Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido,
                Observacao = ((string)objNFe.nfeProc.NFe.infNFe.infAdic?.infCpl ?? "") + " " + ((string)objNFe.nfeProc.NFe.infNFe.infAdic?.infAdFisco ?? ""),
                XPedido = "",
                Produtos = objNFe.nfeProc.NFe.infNFe.det != null ? objNFe.nfeProc.NFe.infNFe.det : null,
                LocalEntrega = objNFe.nfeProc.NFe.infNFe.entrega != null ? (string)objNFe.nfeProc.NFe.infNFe.entrega.CNPJ : ""
            };

            List<string> cfops = new List<string>();
            List<string> ncms = new List<string>();
            int quantidadePallets = 0;
            foreach (var prod in notaFiscal.Produtos)
            {
                quantidadePallets += (int)prod.prod.qCom;
                if (prod.prod.CFOP != null && (int)prod.prod.CFOP > 0)
                    cfops.Add(Convert.ToString((int)prod.prod.CFOP));
                if (prod.prod.NCM != null && !string.IsNullOrEmpty((string)prod.prod.NCM) && (string)prod.prod.NCM != "00000000")
                    ncms.Add((string)prod.prod.NCM);
            }
            string cfopPrincipal = "";
            string ncmPrincipal = "";
            if (cfops != null && cfops.Count > 0)
                cfopPrincipal = RetornaRegistroComMaiorQuantidade(cfops);
            if (ncms != null && ncms.Count > 0)
            {
                ncmPrincipal = RetornaRegistroComMaiorQuantidade(ncms);
                if (ncmPrincipal.Length > 4)
                    ncmPrincipal = ncmPrincipal.Substring(0, 4);
            }

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;
            if (!string.IsNullOrWhiteSpace(notaFiscal.Chave))
                xmlNotaFiscal = repXmlNotaFiscal.BuscarPorChave(notaFiscal.Chave);

            if (xmlNotaFiscal == null)
                xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();

            xmlNotaFiscal.CFOP = cfopPrincipal;
            if (!string.IsNullOrWhiteSpace(ncmPrincipal))
                xmlNotaFiscal.NCM = ncmPrincipal;
            xmlNotaFiscal.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe;
            xmlNotaFiscal.XML = objetoJSON;
            xmlNotaFiscal.Chave = notaFiscal.Chave;
            xmlNotaFiscal.TipoEmissao = Utilidades.Chave.ObterTipoEmissao(xmlNotaFiscal.Chave).ToString().ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoNotaFiscal>();
            xmlNotaFiscal.Numero = int.Parse(notaFiscal.Numero);
            xmlNotaFiscal.Modelo = notaFiscal.Modelo;
            xmlNotaFiscal.Serie = notaFiscal.Serie;
            xmlNotaFiscal.Valor = (decimal)notaFiscal.ValorTotal;
            xmlNotaFiscal.BaseCalculoICMS = (decimal)notaFiscal.BaseCalculoICMS;
            xmlNotaFiscal.ValorICMS = (decimal)notaFiscal.ValorICMS;
            xmlNotaFiscal.BaseCalculoST = (decimal)notaFiscal.BaseCalculoST;
            xmlNotaFiscal.ValorST = (decimal)notaFiscal.ValorST;
            xmlNotaFiscal.ValorTotalProdutos = (decimal)notaFiscal.ValorTotalProdutos;
            xmlNotaFiscal.ValorSeguro = (decimal)notaFiscal.ValorSeguro;
            xmlNotaFiscal.ValorDesconto = (decimal)notaFiscal.ValorDesconto;
            xmlNotaFiscal.ValorImpostoImportacao = (decimal)notaFiscal.ValorImpostoImportacao;
            xmlNotaFiscal.ValorIPI = (decimal)notaFiscal.ValorIPI;
            xmlNotaFiscal.ValorPIS = (decimal)notaFiscal.ValorPIS;
            xmlNotaFiscal.ValorCOFINS = (decimal)notaFiscal.ValorCOFINS;
            xmlNotaFiscal.ValorOutros = (decimal)notaFiscal.ValorOutros;
            xmlNotaFiscal.NaturezaOP = (string)notaFiscal.NaturezaOP;
            if ((int)notaFiscal.ModalidadeFrete == 3)
                xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Outros;
            else
                xmlNotaFiscal.ModalidadeFrete = notaFiscal.ModalidadeFrete;
            xmlNotaFiscal.NumeroDT = (string)notaFiscal.XPedido;
            xmlNotaFiscal.QuantidadePallets = quantidadePallets;
            xmlNotaFiscal.Observacao = (string)notaFiscal.Observacao;
            xmlNotaFiscal.Peso = 0;
            xmlNotaFiscal.PesoBaseParaCalculo = 0;
            xmlNotaFiscal.PesoLiquido = 0;
            xmlNotaFiscal.Volumes = 0;
            xmlNotaFiscal.NomeDestinatario = nomeDestinatario;
            xmlNotaFiscal.IEDestinatario = ieDestinatario;
            xmlNotaFiscal.IERemetente = ieEmitente;

            if (notaFiscal.Volumes != null)
            {
                try
                {
                    foreach (var vol in notaFiscal.Volumes)
                    {
                        if (vol != null)
                        {
                            xmlNotaFiscal.Peso += (decimal)vol.pesoB;
                            xmlNotaFiscal.PesoBaseParaCalculo += xmlNotaFiscal.Peso;
                            xmlNotaFiscal.PesoLiquido += (decimal)vol.pesoL;
                            xmlNotaFiscal.Volumes += (vol.qVol != null ? (int)vol.qVol : 0);
                        }
                    }
                }
                catch
                {
                    //erro quando volumes vem null
                }
            }

            if (!string.IsNullOrWhiteSpace(notaFiscal.Empresa))
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                xmlNotaFiscal.CNPJTranposrtador = notaFiscal.Empresa;
                xmlNotaFiscal.Empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(notaFiscal.Empresa));
            }
            else
            {
                xmlNotaFiscal.CNPJTranposrtador = "";
            }


            if (!string.IsNullOrWhiteSpace(notaFiscal.Placa))
                xmlNotaFiscal.PlacaVeiculoNotaFiscal = notaFiscal.Placa;
            else
                xmlNotaFiscal.PlacaVeiculoNotaFiscal = "";

            xmlNotaFiscal.TipoOperacaoNotaFiscal = notaFiscal.TipoOperacao;

            DateTime dataEmissao;
            if (!DateTime.TryParseExact(notaFiscal.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
            {
                if (!DateTime.TryParseExact(notaFiscal.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                {
                    if (!DateTime.TryParseExact(notaFiscal.DataEmissao, "MM/dd/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                    {
                        if (!DateTime.TryParseExact(notaFiscal.DataEmissao.Substring(0, 19), "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                            dataEmissao = DateTime.Now;
                    }
                    ;
                }
                ;
            }
            ;

            xmlNotaFiscal.DataEmissao = dataEmissao;
            xmlNotaFiscal.nfAtiva = true;

            xmlNotaFiscal.Emitente = notaFiscal.Remetente;
            xmlNotaFiscal.Destinatario = destinatario;

            double.TryParse(Utilidades.String.OnlyNumbers((string)notaFiscal.LocalEntrega), out double cpfCnpjLocalEntrega);

            if (cpfCnpjLocalEntrega > 0D && xmlNotaFiscal.Destinatario?.CPF_CNPJ != cpfCnpjLocalEntrega)
                xmlNotaFiscal.Recebedor = repCliente.BuscarPorCPFCNPJ(cpfCnpjLocalEntrega);

            if (xmlNotaFiscal.Emitente != null && xmlNotaFiscal.Emitente.GrupoPessoas != null)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoaEmitente = xmlNotaFiscal.Emitente.GrupoPessoas;

                if (grupoPessoaEmitente.LerNumeroPedidoObservacaoContribuinteNota && !string.IsNullOrWhiteSpace(grupoPessoaEmitente.IdentificadorNumeroPedidoObservacaoContribuinteNota) && objNFe.nfeProc.NFe.infNFe.infAdic?.obsCont != null)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.NFe.Observacao> observacoes = (List<Dominio.ObjetosDeValor.Embarcador.NFe.Observacao>)objNFe.nfeProc.NFe.infNFe.infAdic?.obsCont;

                    xmlNotaFiscal.NumeroDT = observacoes.Where(o => o.xCampo == grupoPessoaEmitente.IdentificadorNumeroPedidoObservacaoContribuinteNota).Select(o => o.xTexto).FirstOrDefault();

                    if (int.TryParse(xmlNotaFiscal.NumeroDT, out int numeroPedido))
                        xmlNotaFiscal.NumeroPedido = numeroPedido;
                }
                else if (grupoPessoaEmitente.LerNumeroPedidoDaObservacaoDaNota && !string.IsNullOrWhiteSpace((string)notaFiscal.Observacao)) //todo: pegar numero DT, está temparario para notas NATURAL ONE DANONE
                {
                    string observacao = (string)notaFiscal.Observacao;
                    if (grupoPessoaEmitente.TipoLeituraNumeroCargaNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLeituraNumeroCargaNotaFiscal.Carrefour)
                    {
                        string[] romanovski = new string[] { "Romaneio: " };
                        string[] splitOBS = observacao.Split(romanovski, StringSplitOptions.RemoveEmptyEntries);

                        if (splitOBS.Length > 1)
                        {
                            string[] splitPedido = splitOBS[1].Split(' ');
                            if (splitPedido.Length > 0)
                            {
                                string pedido = Utilidades.String.RemoveAllSpecialCharacters(splitPedido[0].Trim().Replace(";", ""));
                                xmlNotaFiscal.NumeroDT = pedido;
                                int numeroPedido = 0;
                                int.TryParse(xmlNotaFiscal.NumeroDT, out numeroPedido);
                                xmlNotaFiscal.NumeroPedido = numeroPedido;
                            }
                        }
                    }
                    else
                    {
                        string[] splitOBS = observacao.Split(',');

                        if (splitOBS.Length > 1)
                        {
                            string[] splitPedido = splitOBS[1].Split(':');
                            if (splitPedido.Length > 0)
                            {
                                string pedido = splitPedido[1].Trim();
                                xmlNotaFiscal.NumeroDT = pedido;
                                int numeroPedido = 0;
                                int.TryParse(xmlNotaFiscal.NumeroDT, out numeroPedido);

                                xmlNotaFiscal.NumeroPedido = numeroPedido;
                            }
                        }
                    }
                }

                if (grupoPessoaEmitente.LerPlacaDaObservacaoDaNota && string.IsNullOrWhiteSpace(xmlNotaFiscal.PlacaVeiculoNotaFiscal))
                {
                    string observacao = ((string)xmlNotaFiscal.Observacao).ToLower();

                    if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                    {
                        string strInicio = grupoPessoaEmitente.LerPlacaDaObservacaoDaNotaInicio.ToLower();
                        string strFim = grupoPessoaEmitente.LerPlacaDaObservacaoDaNotaFim.ToLower();

                        int idxInicio = observacao.IndexOf(strInicio);

                        if (idxInicio > -1)
                        {
                            idxInicio += strInicio.Length;

                            int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                            if (idxFim > 0)
                            {
                                string placa = Utilidades.String.OnlyNumbersAndChars(observacao.Substring(idxInicio, idxFim - idxInicio));

                                if (!string.IsNullOrWhiteSpace(placa) && placa.Length == 7)
                                    xmlNotaFiscal.PlacaVeiculoNotaFiscal = placa.ToUpper();
                            }
                        }
                    }
                }

                if (grupoPessoaEmitente.LerPlacaDaObservacaoContribuinteDaNota && string.IsNullOrWhiteSpace(xmlNotaFiscal.PlacaVeiculoNotaFiscal) && objNFe.nfeProc.NFe.infNFe.infAdic?.obsCont != null)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.NFe.Observacao> observacoes = (List<Dominio.ObjetosDeValor.Embarcador.NFe.Observacao>)objNFe.nfeProc.NFe.infNFe.infAdic.obsCont;

                    string placa = Utilidades.String.OnlyNumbersAndChars(observacoes.Where(o => o.xCampo.ToLower().Equals(grupoPessoaEmitente.LerPlacaDaObservacaoContribuinteDaNotaIdentificacao.ToLower())).Select(o => o.xTexto).FirstOrDefault());

                    if (!string.IsNullOrWhiteSpace(placa) && placa.Length == 7)
                        xmlNotaFiscal.PlacaVeiculoNotaFiscal = placa.ToUpper();
                }

                if (!string.IsNullOrWhiteSpace(grupoPessoaEmitente.ExpressaoBooking))
                {
                    string observacaoBooking = ((string)xmlNotaFiscal.Observacao).ToLower();
                    System.Text.RegularExpressions.Regex patternBooking = new System.Text.RegularExpressions.Regex(grupoPessoaEmitente.ExpressaoBooking, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    System.Text.RegularExpressions.Match matchBooking = patternBooking.Match(observacaoBooking);

                    if (matchBooking != null && !string.IsNullOrWhiteSpace(matchBooking.Value))
                        xmlNotaFiscal.NumeroBooking = matchBooking.Value.ToUpper();
                }

                if (!string.IsNullOrWhiteSpace(grupoPessoaEmitente.ExpressaoContainer))
                {
                    string observacaoContainer = ((string)xmlNotaFiscal.Observacao).ToLower();
                    System.Text.RegularExpressions.Regex patternContainer = new System.Text.RegularExpressions.Regex(grupoPessoaEmitente.ExpressaoContainer, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    System.Text.RegularExpressions.Match matchContainer = patternContainer.Match(observacaoContainer);

                    if (matchContainer != null && !string.IsNullOrWhiteSpace(matchContainer.Value))
                    {
                        string numeroContainer = matchContainer.Value.ToUpper();
                        numeroContainer = numeroContainer.Replace(" ", "").Replace("-", "").Replace(".", "").Replace("/", "").Replace("\\", "").Trim();
                        xmlNotaFiscal.NumeroContainer = numeroContainer;
                    }
                }

                if (grupoPessoaEmitente.NCMsPallet?.Count > 0 && ncms?.Count > 0)
                {
                    if (grupoPessoaEmitente.NCMsPallet.Any(o => ncms.Contains(o.NCM)))
                        xmlNotaFiscal.TipoNotaFiscalIntegrada = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.RemessaPallet;
                }
            }

            return xmlNotaFiscal;
        }

        public byte[] DownloadXml(int codigoXml)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repositorioXMLNotaFiscal.BuscarXMLPorCodigo(codigoXml);

            if (xmlNotaFiscal == null || string.IsNullOrWhiteSpace(xmlNotaFiscal.XML))
                return null;

            byte[] data = System.Text.Encoding.UTF8.GetBytes(xmlNotaFiscal.XML);
            return data;
        }

        public async Task<byte[]> DownloadXmlAsync(int codigoXml)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork, _cancellationToken);
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = await repositorioXMLNotaFiscal.BuscarXMLPorCodigoAsync(codigoXml);

            if (xmlNotaFiscal == null || string.IsNullOrWhiteSpace(xmlNotaFiscal.XML))
                return null;

            byte[] data = System.Text.Encoding.UTF8.GetBytes(xmlNotaFiscal.XML);
            return data;
        }

        public byte[] ObterDACCe(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento, Repositorio.UnitOfWork unitOfWork)
        {
            return ReportRequest.WithType(
                    ReportType.Dacce)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoDocumento", documento.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }

        public async Task<System.IO.MemoryStream> ObterLoteDeXML(List<int> codigos, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork, _cancellationToken);
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmls = await repositorioXMLNotaFiscal.BuscarListaXMLPorCodigoAsync(codigos);

            MemoryStream fZip = new MemoryStream();
            ZipOutputStream zipOStream = new ZipOutputStream(fZip);
            zipOStream.SetLevel(9);

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xml in xmls)
            {
                if (!string.IsNullOrWhiteSpace(xml.XML))
                {
                    byte[] arquivo = System.Text.Encoding.UTF8.GetBytes(xml.XML);
                    ZipEntry entry = new ZipEntry(string.Concat("NFe", xml.Chave, ".xml"));
                    entry.DateTime = DateTime.Now;
                    zipOStream.PutNextEntry(entry);
                    zipOStream.Write(arquivo, 0, arquivo.Length);
                    zipOStream.CloseEntry();
                }
            }

            xmls = null;

            zipOStream.IsStreamOwner = false;
            zipOStream.Close();

            fZip.Position = 0;

            return fZip;
        }

        #endregion

        #region Métodos Privados

        private string RetornaRegistroComMaiorQuantidade(List<string> lista)
        {
            var nameGroup = lista.GroupBy(x => x);
            var maxCount = nameGroup.Max(g => g.Count());
            return nameGroup.Where(x => x.Count() == maxCount).Select(x => x.Key).FirstOrDefault();
        }

        #endregion
    }
}
