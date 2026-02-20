using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;

namespace Servicos.Embarcador.NotaFiscal
{
    public class NotaFiscalEletronica : ServicoBase
    {        

        public NotaFiscalEletronica() : base() { }

        public NotaFiscalEletronica(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public void SalvarDadosNFe(ref Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica nfeIntegracao, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoNFe> permissoesAlteracao, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Empresa empresa, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

            Servicos.Embarcador.NotaFiscal.NotaFiscalParcela serNotaFiscalParcela = new NotaFiscalParcela(unitOfWork);
            Servicos.Embarcador.NotaFiscal.NotaFiscalProduto serNotaFiscalProduto = new NotaFiscalProduto(unitOfWork);
            Servicos.Embarcador.NotaFiscal.NotaFiscalReferencia serNotaFiscalReferencia = new NotaFiscalReferencia(unitOfWork);

            bool permissaoTotal = false;
            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoNFe.total))
                permissaoTotal = true;

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoNFe.total) || permissaoTotal)
            {
                if (nfeIntegracao.CodigoEmpresa > 0)
                    nfe.Empresa = repEmpresa.BuscarPorCodigo(nfeIntegracao.CodigoEmpresa);
                else
                    nfe.Empresa = empresa;

                if (nfeIntegracao.Atividade != null)
                    nfe.Atividade = repAtividade.BuscarPorCodigo(nfeIntegracao.Atividade.Codigo);
                else
                    nfe.Atividade = repAtividade.BuscarPrimeiraAtividade();

                nfe.BCCOFINS = nfeIntegracao.BCCOFINS;
                nfe.BCDeducao = nfeIntegracao.BCDeducao;
                nfe.BCICMS = nfeIntegracao.BCICMS;
                nfe.BCICMSST = nfeIntegracao.BCICMSST;
                nfe.BCISSQN = nfeIntegracao.BCISSQN;
                nfe.BCPIS = nfeIntegracao.BCPIS;
                nfe.ModeloNotaFiscal = "55";
                nfe.VersaoNFe = nfe.Empresa.VersaoNFe;

                if (nfeIntegracao.Cliente.ClienteExterior)
                {
                    nfe.Cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(nfeIntegracao.Cliente.RGIE));
                    if (nfe.Cliente == null)
                        nfe.Cliente = repCliente.BuscarPorNome(nfeIntegracao.Cliente.RazaoSocial.ToUpper());
                }
                else
                    nfe.Cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(nfeIntegracao.Cliente.CPFCNPJ));
                //novo cliente
                if (nfe.Cliente == null)
                {
                    CadastrarPessoa(nfeIntegracao.Cliente, unitOfWork);
                    nfe.Cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(nfeIntegracao.Cliente.CPFCNPJ));
                }

                nfe.DataEmissao = nfeIntegracao.DataEmissao;
                if (nfeIntegracao.DataPrestacaoServico == null || nfeIntegracao.DataPrestacaoServico > DateTime.MinValue)
                    nfe.DataPrestacaoServico = DateTime.Now;
                else
                    nfe.DataPrestacaoServico = nfeIntegracao.DataPrestacaoServico;
                if (nfeIntegracao.DataProcessamento > DateTime.MinValue)
                    nfe.DataProcessamento = nfeIntegracao.DataProcessamento;
                else
                    nfe.DataProcessamento = null;
                nfe.TipoAmbiente = empresa.TipoAmbiente;
                if (nfeIntegracao.DataSaida != null && nfeIntegracao.DataSaida > DateTime.MinValue)
                    nfe.DataSaida = nfeIntegracao.DataSaida;
                else
                    nfe.DataSaida = null;
                if (nfeIntegracao.Codigo == -2)
                    nfe.EmpresaSerie = repEmpresaSerie.BuscarPorSerie(empresa.Codigo, nfeIntegracao.Serie, Dominio.Enumeradores.TipoSerie.NFe);
                else
                    nfe.EmpresaSerie = repEmpresaSerie.BuscarPorCodigo(nfeIntegracao.Serie);
                //cadastrar empresa serie se nao tem
                if (nfe.EmpresaSerie == null)
                {
                    CadastrarSerieEmpresa(nfeIntegracao.Serie, empresa, unitOfWork);
                    nfe.EmpresaSerie = repEmpresaSerie.BuscarPorSerie(empresa.Codigo, nfeIntegracao.Serie, Dominio.Enumeradores.TipoSerie.NFe);
                }

                nfe.Finalidade = nfeIntegracao.Finalidade;
                nfe.ICMSDesonerado = nfeIntegracao.ICMSDesonerado;
                nfe.IndicadorPresenca = nfeIntegracao.IndicadorPresenca;
                nfe.IndicadorIntermediador = nfeIntegracao.IndicadorIntermediador;
                if (nfeIntegracao.LocalidadePrestacaoServico != null && nfeIntegracao.LocalidadePrestacaoServico.Codigo > 0)
                    nfe.LocalidadePrestacaoServico = repLocalidade.BuscarPorCodigo(nfeIntegracao.LocalidadePrestacaoServico.Codigo);
                else if (nfeIntegracao.LocalidadePrestacaoServico != null && nfeIntegracao.LocalidadePrestacaoServico.IBGE > 0)
                    nfe.LocalidadePrestacaoServico = repLocalidade.BuscarPorCodigoIBGE(nfeIntegracao.LocalidadePrestacaoServico.IBGE);
                else
                    nfe.LocalidadePrestacaoServico = empresa.Localidade;

                nfe.NaturezaDaOperacao = repNaturezaDaOperacao.BuscarPorId(nfeIntegracao.NaturezaDaOperacao.Codigo);
                nfe.Intermediador = nfeIntegracao.Intermediador != null ? repCliente.BuscarPorCPFCNPJ(double.Parse(nfeIntegracao.Intermediador.CPFCNPJ)) : null;

                if (nfeIntegracao.Codigo == -2)
                {
                    nfe.Numero = nfeIntegracao.Numero;
                }
                else if (nfe.Codigo <= 0)
                {
                    nfe.Numero = repNFe.BuscarUltimoNumero(nfe.Empresa.Codigo, nfe.EmpresaSerie.Numero, empresa.TipoAmbiente, "55") + 1;
                    int proximoNumeroSerie = repEmpresaSerie.BuscarProximoNumeroDocumentoPorSerie(nfe.Empresa.Codigo, nfe.EmpresaSerie.Numero, Dominio.Enumeradores.TipoSerie.NFe);
                    if (nfe.Numero < proximoNumeroSerie)
                        nfe.Numero = proximoNumeroSerie;
                    nfe.Usuario = usuario;
                }
                nfe.ObservacaoNota = nfeIntegracao.ObservacaoNota?.Trim() ?? string.Empty;
                nfe.ObservacaoTributaria = nfeIntegracao.ObservacaoTributaria?.Trim() ?? string.Empty;

                nfe.UFEmbarque = nfeIntegracao.UFEmbarque;
                nfe.LocalEmbarque = nfeIntegracao.LocalEmbarque;
                nfe.LocalDespacho = nfeIntegracao.LocalDespacho;
                nfe.InformacaoCompraNotaEmpenho = nfeIntegracao.InformacaoCompraNotaEmpenho;
                nfe.InformacaoCompraPedido = nfeIntegracao.InformacaoCompraPedido;
                nfe.InformacaoCompraContrato = nfeIntegracao.InformacaoCompraContrato;

                nfe.Status = Dominio.Enumeradores.StatusNFe.Emitido;
                nfe.TipoEmissao = nfeIntegracao.TipoEmissao;
                nfe.TipoFrete = nfeIntegracao.TipoFrete;
                nfe.TranspANTTVeiculo = nfeIntegracao.TranspANTTVeiculo;
                nfe.TranspCNPJCPF = Utilidades.String.OnlyNumbers(nfeIntegracao.TranspCNPJCPF);
                nfe.TranspEmail = nfeIntegracao.TranspEmail;
                nfe.TranspEndereco = nfeIntegracao.TranspEndereco;
                if (!string.IsNullOrWhiteSpace(nfe.TranspEndereco) && nfe.TranspEndereco.Length >= 60)
                    nfe.TranspEndereco = nfe.TranspEndereco.Substring(0, 59);
                nfe.TranspEspecie = nfeIntegracao.TranspEspecie;
                nfe.TranspIE = nfeIntegracao.TranspIE;
                nfe.TranspMarca = nfeIntegracao.TranspMarca;
                if (nfeIntegracao.TranspMunicipio != null && nfeIntegracao.TranspMunicipio.Codigo > 0)
                    nfe.LocalidadeTranspMunicipio = repLocalidade.BuscarPorCodigo(nfeIntegracao.TranspMunicipio.Codigo);
                else
                    nfe.LocalidadeTranspMunicipio = null;
                if (!string.IsNullOrWhiteSpace(nfeIntegracao.TranspNome) && nfeIntegracao.TranspNome.Length >= 60)
                    nfe.TranspNome = nfeIntegracao.TranspNome.Substring(0, 59);
                else
                    nfe.TranspNome = nfeIntegracao.TranspNome;
                if (nfeIntegracao.Transportadora != null && !string.IsNullOrWhiteSpace(nfeIntegracao.Transportadora.CPFCNPJ) && nfeIntegracao.Transportadora.CPFCNPJ != "0")
                    nfe.Transportadora = repCliente.BuscarPorCPFCNPJ(double.Parse(nfeIntegracao.Transportadora.CPFCNPJ));
                else
                    nfe.Transportadora = null;
                nfe.TranspPesoBruto = nfeIntegracao.TranspPesoBruto;
                nfe.TranspPesoLiquido = nfeIntegracao.TranspPesoLiquido;
                nfe.TranspPlacaVeiculo = nfeIntegracao.TranspPlacaVeiculo;
                nfe.TranspQuantidade = nfeIntegracao.TranspQuantidade;
                nfe.TranspUF = nfeIntegracao.TranspUF;
                nfe.TranspUFVeiculo = nfeIntegracao.TranspUFVeiculo;
                nfe.TranspVolume = nfeIntegracao.TranspVolume;
                nfe.ValorCOFINS = nfeIntegracao.ValorCOFINS;
                nfe.ValorDesconto = nfeIntegracao.ValorDesconto;
                nfe.ValorDescontoCondicional = nfeIntegracao.ValorDescontoCondicional;
                nfe.ValorDescontoIncondicional = nfeIntegracao.ValorDescontoIncondicional;
                nfe.ValorFCP = nfeIntegracao.ValorFCP;
                nfe.ValorFrete = nfeIntegracao.ValorFrete;
                nfe.ValorICMS = nfeIntegracao.ValorICMS;
                nfe.ValorICMSDestino = nfeIntegracao.ValorICMSDestino;
                nfe.ValorICMSRemetente = nfeIntegracao.ValorICMSRemetente;
                nfe.ValorICMSST = nfeIntegracao.ValorICMSST;
                nfe.ValorII = nfeIntegracao.ValorII;
                nfe.ValorIPI = nfeIntegracao.ValorIPI;
                nfe.ValorISSQN = nfeIntegracao.ValorISSQN;
                nfe.ValorOutrasDespesas = nfeIntegracao.ValorOutrasDespesas;
                nfe.ValorOutrasRetencoes = nfeIntegracao.ValorOutrasRetencoes;
                nfe.ValorPIS = nfeIntegracao.ValorPIS;
                nfe.ValorProdutos = nfeIntegracao.ValorProdutos;
                nfe.ValorRetencaoISS = nfeIntegracao.ValorRetencaoISS;
                nfe.ValorSeguro = nfeIntegracao.ValorSeguro;
                nfe.ValorServicos = nfeIntegracao.ValorServicos;
                nfe.ValorTotalNota = nfeIntegracao.ValorTotalNota;
                nfe.ValorFCPICMS = nfeIntegracao.ValorFCPICMS;
                nfe.ValorFCPICMSST = nfeIntegracao.ValorFCPICMSST;
                nfe.ValorIPIDevolvido = nfeIntegracao.ValorIPIDevolvido;
                nfe.BCICMSSTRetido = nfeIntegracao.BCICMSSTRetido;
                nfe.ValorICMSSTRetido = nfeIntegracao.ValorICMSSTRetido;
                if (nfeIntegracao.Veiculo != null && !string.IsNullOrWhiteSpace(nfeIntegracao.Veiculo.Placa))
                    nfe.Veiculo = repVeiculo.BuscarPorCodigo(nfeIntegracao.Veiculo.Codigo);

                nfe.UtilizarEnderecoRetirada = nfeIntegracao.UtilizarEnderecoRetirada;
                if (nfeIntegracao.ClienteRetirada != null && !string.IsNullOrWhiteSpace(nfeIntegracao.ClienteRetirada.CPFCNPJ) && nfeIntegracao.ClienteRetirada.CPFCNPJ != ")")
                    nfe.ClienteRetirada = repCliente.BuscarPorCPFCNPJ(nfeIntegracao.ClienteRetirada.CPFCNPJ.ToDouble());
                if (nfeIntegracao.LocalidadeRetirada != null && nfeIntegracao.LocalidadeRetirada.Codigo > 0)
                    nfe.LocalidadeRetirada = repLocalidade.BuscarPorCodigo(nfeIntegracao.LocalidadeRetirada.Codigo);
                nfe.RetiradaLogradouro = nfeIntegracao.RetiradaLogradouro;
                nfe.RetiradaNumeroLogradouro = nfeIntegracao.RetiradaNumeroLogradouro;
                nfe.RetiradaComplementoLogradouro = nfeIntegracao.RetiradaComplementoLogradouro;
                nfe.RetiradaBairro = nfeIntegracao.RetiradaBairro;
                nfe.RetiradaCEP = nfeIntegracao.RetiradaCEP;
                nfe.RetiradaTelefone = nfeIntegracao.RetiradaTelefone;
                nfe.RetiradaEmail = nfeIntegracao.RetiradaEmail;
                nfe.RetiradaIE = nfeIntegracao.RetiradaIE;

                nfe.UtilizarEnderecoEntrega = nfeIntegracao.UtilizarEnderecoEntrega;
                if (nfeIntegracao.ClienteEntrega != null && !string.IsNullOrWhiteSpace(nfeIntegracao.ClienteEntrega.CPFCNPJ) && nfeIntegracao.ClienteEntrega.CPFCNPJ != ")")
                    nfe.ClienteEntrega = repCliente.BuscarPorCPFCNPJ(nfeIntegracao.ClienteEntrega.CPFCNPJ.ToDouble());
                if (nfeIntegracao.LocalidadeEntrega != null && nfeIntegracao.LocalidadeEntrega.Codigo > 0)
                    nfe.LocalidadeEntrega = repLocalidade.BuscarPorCodigo(nfeIntegracao.LocalidadeEntrega.Codigo);
                nfe.EntregaLogradouro = nfeIntegracao.EntregaLogradouro;
                nfe.EntregaNumeroLogradouro = nfeIntegracao.EntregaNumeroLogradouro;
                nfe.EntregaComplementoLogradouro = nfeIntegracao.EntregaComplementoLogradouro;
                nfe.EntregaBairro = nfeIntegracao.EntregaBairro;
                nfe.EntregaCEP = nfeIntegracao.EntregaCEP;
                nfe.EntregaTelefone = nfeIntegracao.EntregaTelefone;
                nfe.EntregaEmail = nfeIntegracao.EntregaEmail;
                nfe.EntregaIE = nfeIntegracao.EntregaIE;

            }

            var inserindo = false;
            if (nfe.Codigo <= 0)
            {
                inserindo = true;
                repNFe.Inserir(nfe);
            }
            else
                repNFe.Atualizar(nfe);

            decimal valorIBPT = 0;
            decimal valorCreditoICMS = 0;
            serNotaFiscalProduto.SalvarItensNFe(ref nfe, nfeIntegracao.ItensNFe, unitOfWork, empresa, out valorIBPT, out valorCreditoICMS, tipoServicoMultisoftware, configuracao);
            serNotaFiscalParcela.SalvarParcelasNFe(ref nfe, nfeIntegracao.ParcelasNFe, unitOfWork, empresa);
            serNotaFiscalReferencia.SalvarReferenciasNFe(ref nfe, nfeIntegracao.ReferenciaNFe, unitOfWork, empresa);

            if (!empresa.CalculaIBPTNFe)
                valorIBPT = 0;
            decimal valorIPBTNacional = 0;
            decimal valorIPBTEstadual = 0;
            decimal valorIPBTMunicipal = 0;
            int empresaPai = 0;
            if (empresa.EmpresaPai != null)
                empresaPai = empresa.EmpresaPai.Codigo;

            //PREENCHE OBSERVAÇÕES DA NF
            if (inserindo && valorCreditoICMS > 0 && !string.IsNullOrWhiteSpace(empresa.ObservacaoSimplesNacional))
            {
                var observacaoEmpresa = empresa.ObservacaoSimplesNacional;
                observacaoEmpresa = observacaoEmpresa.Replace("#ValorCreditoICMS", "R$ " + valorCreditoICMS.ToString("n2"));
                observacaoEmpresa = observacaoEmpresa.Replace("#AliquotaSimples", empresa.AliquotaICMSSimples.ToString("n2") + "%");
                nfe.ObservacaoNota = nfe.ObservacaoNota + " " + observacaoEmpresa;
            }

            if (inserindo)
                nfe.ObservacaoNota += BuscarObservacoesFiscais(nfe, nfeIntegracao, unitOfWork);

            if (empresa.CalculaIBPTNFe)
            {
                for (int i = 0; i < nfeIntegracao.ItensNFe.Count; i++)
                {
                    string ncm = "";
                    if (nfeIntegracao.ItensNFe[i].Produto != null && nfeIntegracao.ItensNFe[i].Produto.Codigo > 0)
                        ncm = repProduto.BuscarPorCodigo(nfeIntegracao.ItensNFe[i].Produto.Codigo).CodigoNCM;
                    else if (nfeIntegracao.ItensNFe[i].Servico != null && nfeIntegracao.ItensNFe[i].Servico.Codigo > 0)
                        ncm = Utilidades.String.OnlyNumbers(repServico.BuscarPorCodigo(nfeIntegracao.ItensNFe[i].Servico.Codigo).NumeroCodigoServico);
                    valorIPBTNacional += Math.Round(serNotaFiscalProduto.RetornaValorIBPT(empresaPai, empresa.Codigo, Utilidades.String.OnlyNumbers(ncm), nfeIntegracao.ItensNFe[i].ValorTotal, unitOfWork, 0), 2);
                    valorIPBTEstadual += Math.Round(serNotaFiscalProduto.RetornaValorIBPT(empresaPai, empresa.Codigo, Utilidades.String.OnlyNumbers(ncm), nfeIntegracao.ItensNFe[i].ValorTotal, unitOfWork, 1), 2);
                    valorIPBTMunicipal += Math.Round(serNotaFiscalProduto.RetornaValorIBPT(empresaPai, empresa.Codigo, Utilidades.String.OnlyNumbers(ncm), nfeIntegracao.ItensNFe[i].ValorTotal, unitOfWork, 2), 2);
                }
            }

            nfe.ValorImpostoIBPT = Math.Round(valorIBPT, 2);
            //valorIBPT = Math.Round(valorIPBTNacional + valorIPBTEstadual + valorIPBTMunicipal, 2);
            if (empresa.CalculaIBPTNFe)
            {
                if (!inserindo)
                {
                    if (nfe.ObservacaoNota.Contains("Valor aproximado dos tributos") && nfe.ObservacaoNota.Contains(" - Fonte: IBPT"))
                    {
                        int posInicial = nfe.ObservacaoNota.IndexOf("Valor aproximado dos tributos");
                        int posFinal = nfe.ObservacaoNota.IndexOf(" - Fonte: IBPT") + 14;
                        if ((posFinal - posInicial) > 0)
                            nfe.ObservacaoNota = nfe.ObservacaoNota.Remove(posInicial, posFinal - posInicial);
                    }
                }
            }

            if (valorIBPT > 0)
                nfe.ObservacaoNota = nfe.ObservacaoNota + " Valor aproximado dos tributos com base na Lei 12.741/2012 - R$ " + valorIBPT.ToString("n2") + " (" + ((valorIBPT * 100) / nfe.ValorTotalNota).ToString("n2") + " %) ";
            if (valorIPBTNacional > 0)
                nfe.ObservacaoNota = nfe.ObservacaoNota + " - Nacional R$ " + valorIPBTNacional.ToString("n2");
            if (valorIPBTEstadual > 0)
                nfe.ObservacaoNota = nfe.ObservacaoNota + " - Estadual R$ " + valorIPBTEstadual.ToString("n2");
            if (valorIPBTMunicipal > 0)
                nfe.ObservacaoNota = nfe.ObservacaoNota + " - Municipal R$ " + valorIPBTMunicipal.ToString("n2");
            if (valorIBPT > 0)
                nfe.ObservacaoNota = nfe.ObservacaoNota + " - Fonte: IBPT";

            nfe.ObservacaoNota = nfe.ObservacaoNota.TrimEnd();
            nfe.ObservacaoNota = nfe.ObservacaoNota.TrimStart();
            nfe.ObservacaoNota = nfe.ObservacaoNota.Trim();

            if (inserindo && (nfe.BCICMSSTRetido > 0 || nfe.ValorICMSSTRetido > 0) && nfe.Empresa?.Localidade?.Estado?.Sigla == "SC")//Observação de Imposto Retido
                nfe.ObservacaoTributaria += " IMPOSTO RETIDO POR SUBSTITUICAO TRIBUTARIA CONFORME ANEXO 3 DO RICMS/SC. BASE CALCULO ICMS ST ETAPA ANT. R$ " + nfe.BCICMSSTRetido.ToString("n2") +
                    ". VALOR ICMS ST ETAPA ANTERIOR R$ " + nfe.ValorICMSSTRetido.ToString("n2") + ".";
            nfe.ObservacaoTributaria = nfe.ObservacaoTributaria.Trim();

            repNFe.Atualizar(nfe);
        }

        public Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica ConverterEntidadeNFeParaObjeto(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, Repositorio.UnitOfWork unitOfWork)
        {

            Servicos.Embarcador.NotaFiscal.NotaFiscalParcela serNotaFiscalParcela = new NotaFiscalParcela(unitOfWork);
            Servicos.Embarcador.NotaFiscal.NotaFiscalProduto serNotaFiscalProduto = new NotaFiscalProduto(unitOfWork);
            Servicos.Embarcador.NotaFiscal.NotaFiscalReferencia serNotaFiscalReferencia = new NotaFiscalReferencia(unitOfWork);
            Servicos.WebService.Empresa.Empresa serEmpresa = new WebService.Empresa.Empresa(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Localidades.Localidade(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica nfeIntegracao = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica();

            nfeIntegracao.Empresa = serEmpresa.ConverterObjetoEmpresa(nfe.Empresa);
            nfeIntegracao.Atividade = ConverterObjetoAtividade(nfe.Atividade);
            nfeIntegracao.BCCOFINS = nfe.BCCOFINS;
            nfeIntegracao.BCDeducao = nfe.BCDeducao;
            nfeIntegracao.BCICMS = nfe.BCICMS;
            nfeIntegracao.BCICMSST = nfe.BCICMSST;
            nfeIntegracao.BCISSQN = nfe.BCISSQN;
            nfeIntegracao.BCPIS = nfe.BCPIS;
            nfeIntegracao.Cliente = ConverterObjetoPessoa(nfe.Cliente);
            nfeIntegracao.DataEmissao = nfe.DataEmissao;
            nfeIntegracao.DataPrestacaoServico = nfe.DataPrestacaoServico;
            nfeIntegracao.DataProcessamento = nfe.DataProcessamento;
            if (nfe.DataSaida != null && nfe.DataSaida > DateTime.MinValue)
                nfeIntegracao.DataSaida = nfe.DataSaida;
            else
                nfeIntegracao.DataSaida = null;
            nfeIntegracao.Serie = nfe.EmpresaSerie.Numero;
            nfeIntegracao.Finalidade = nfe.Finalidade;
            nfeIntegracao.ICMSDesonerado = nfe.ICMSDesonerado;
            nfeIntegracao.IndicadorPresenca = nfe.IndicadorPresenca;
            nfeIntegracao.LocalidadePrestacaoServico = serLocalidade.ConverterObjetoLocalidade(nfe.LocalidadePrestacaoServico);
            nfeIntegracao.ModeloDocumentoFiscal = ConverterObjetoModelo(nfe.ModeloDocumentoFiscal);
            nfeIntegracao.NaturezaDaOperacao = ConverterObjetoNatureza(nfe.NaturezaDaOperacao);
            nfeIntegracao.Numero = nfe.Numero;
            nfeIntegracao.ObservacaoNota = nfe.ObservacaoNota;
            nfeIntegracao.ObservacaoTributaria = nfe.ObservacaoTributaria;
            nfeIntegracao.UFEmbarque = nfe.UFEmbarque;
            nfeIntegracao.LocalEmbarque = nfe.LocalEmbarque;
            nfeIntegracao.LocalDespacho = nfe.LocalDespacho;
            nfeIntegracao.InformacaoCompraNotaEmpenho = nfe.InformacaoCompraNotaEmpenho;
            nfeIntegracao.InformacaoCompraPedido = nfe.InformacaoCompraPedido;
            nfeIntegracao.InformacaoCompraContrato = nfe.InformacaoCompraContrato;
            nfeIntegracao.Status = nfe.Status;
            nfeIntegracao.TipoEmissao = nfe.TipoEmissao;
            nfeIntegracao.TipoFrete = nfe.TipoFrete;
            nfeIntegracao.TranspANTTVeiculo = nfe.TranspANTTVeiculo;
            nfeIntegracao.TranspCNPJCPF = nfe.TranspCNPJCPF;
            nfeIntegracao.TranspEmail = nfe.TranspEmail;
            nfeIntegracao.TranspEndereco = nfe.TranspEndereco;
            nfeIntegracao.TranspEspecie = nfe.TranspEspecie;
            nfeIntegracao.TranspIE = nfe.TranspIE;
            nfeIntegracao.TranspMarca = nfe.TranspMarca;
            nfeIntegracao.TranspMunicipio = serLocalidade.ConverterObjetoLocalidade(nfe.LocalidadeTranspMunicipio);
            nfeIntegracao.TranspNome = nfe.TranspNome;
            nfeIntegracao.Transportadora = ConverterObjetoPessoa(nfe.Transportadora);
            nfeIntegracao.TranspPesoBruto = nfe.TranspPesoBruto;
            nfeIntegracao.TranspPesoLiquido = nfe.TranspPesoLiquido;
            nfeIntegracao.TranspPlacaVeiculo = nfe.TranspPlacaVeiculo;
            nfeIntegracao.TranspQuantidade = nfe.TranspQuantidade;
            nfeIntegracao.TranspUF = nfe.TranspUF;
            nfeIntegracao.TranspUFVeiculo = nfe.TranspUFVeiculo;
            nfeIntegracao.TranspVolume = nfe.TranspVolume;
            nfeIntegracao.ValorCOFINS = nfe.ValorCOFINS;
            nfeIntegracao.ValorDesconto = nfe.ValorDesconto;
            nfeIntegracao.ValorDescontoCondicional = nfe.ValorDescontoCondicional;
            nfeIntegracao.ValorDescontoIncondicional = nfe.ValorDescontoIncondicional;
            nfeIntegracao.ValorFCP = nfe.ValorFCP;
            nfeIntegracao.ValorFrete = nfe.ValorFrete;
            nfeIntegracao.ValorICMS = nfe.ValorICMS;
            nfeIntegracao.ValorICMSDestino = nfe.ValorICMSDestino;
            nfeIntegracao.ValorICMSRemetente = nfe.ValorICMSRemetente;
            nfeIntegracao.ValorICMSST = nfe.ValorICMSST;
            nfeIntegracao.ValorII = nfe.ValorII;
            nfeIntegracao.ValorIPI = nfe.ValorIPI;
            nfeIntegracao.ValorISSQN = nfe.ValorISSQN;
            nfeIntegracao.ValorOutrasDespesas = nfe.ValorOutrasDespesas;
            nfeIntegracao.ValorOutrasRetencoes = nfe.ValorOutrasRetencoes;
            nfeIntegracao.ValorPIS = nfe.ValorPIS;
            nfeIntegracao.ValorProdutos = nfe.ValorProdutos;
            nfeIntegracao.ValorRetencaoISS = nfe.ValorRetencaoISS;
            nfeIntegracao.ValorSeguro = nfe.ValorSeguro;
            nfeIntegracao.ValorServicos = nfe.ValorServicos;
            nfeIntegracao.ValorTotalNota = nfe.ValorTotalNota;
            nfeIntegracao.Veiculo = ConverterObjetoVeiculo(nfe.Veiculo);

            nfeIntegracao.ParcelasNFe = serNotaFiscalParcela.ConverterNotaFiscalParcela(nfe.ParcelasNFe.ToList(), unitOfWork);
            nfeIntegracao.ReferenciaNFe = serNotaFiscalReferencia.ConverterNotaFiscalReferencia(nfe.ReferenciaNFe.ToList(), unitOfWork);
            nfeIntegracao.ItensNFe = serNotaFiscalProduto.ConverterNotaFiscalProdutos(nfe.ItensNFe.ToList(), unitOfWork);

            return nfeIntegracao;
        }

        public Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NaturezaDaOperacao CadastrarNaturezaOperacao(string descricao, Repositorio.UnitOfWork unitOfWork, int codigoEmpresa)
        {
            Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.NaturezaDaOperacao natureza = null;
            natureza = repNaturezaDaOperacao.BuscarNaturezaNFe(descricao, codigoEmpresa);

            if (natureza != null)
                return new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NaturezaDaOperacao { Codigo = natureza.Codigo, CodigoIntegracao = natureza.Codigo.ToString(), Descricao = natureza.Descricao };
            else
            {
                natureza = new Dominio.Entidades.NaturezaDaOperacao();
                natureza.Descricao = descricao;
                natureza.Status = "A";
                natureza.DentroEstado = true;
                natureza.GeraTitulo = true;
                natureza.Garantia = false;
                natureza.Demonstracao = false;
                natureza.Bonificacao = false;
                natureza.Outras = false;
                natureza.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                repNaturezaDaOperacao.Inserir(natureza);

                return new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NaturezaDaOperacao { Codigo = natureza.Codigo, CodigoIntegracao = natureza.Codigo.ToString(), Descricao = natureza.Descricao };
            }
        }

        public int CadastrarCFOP(int numeroCFOP, int empresa, Dominio.Enumeradores.TipoCFOP tipoCFOP, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.CFOP cfop = repCFOP.BuscarPorCFOPEmpresa(numeroCFOP, empresa);
            if (cfop == null)
            {
                cfop = new Dominio.Entidades.CFOP();
                cfop.CodigoCFOP = numeroCFOP;
                cfop.Descricao = numeroCFOP.ToString();
                cfop.Status = "A";
                cfop.Tipo = tipoCFOP;
                cfop.Empresa = repEmpresa.BuscarPorCodigo(empresa);

                repCFOP.Inserir(cfop);
            }
            return cfop.Codigo;
        }

        public void CadastrarPessoa(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoaNFe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas repModalidadeClientePessoas = new Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas(unitOfWork);
            Dominio.Entidades.Cliente pessoa = new Dominio.Entidades.Cliente();
            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidade = new Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas();
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas modalidadeCliente = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas();

            pessoa.CPF_CNPJ = double.Parse(pessoaNFe.CPFCNPJ);
            pessoa.Atividade = repAtividade.BuscarPrimeiraAtividade();
            pessoa.Bairro = pessoaNFe.Endereco.Bairro;
            pessoa.CEP = pessoaNFe.Endereco.CEP;
            if (!string.IsNullOrWhiteSpace(pessoaNFe.Endereco.InscricaoEstadual) || pessoaNFe.Endereco.InscricaoEstadual == "ISENTO")
                pessoa.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteIsento;
            else
                pessoa.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS;
            pessoa.EnderecoDigitado = true;
            pessoa.Complemento = pessoaNFe.Endereco.Complemento;
            pessoa.DataCadastro = DateTime.Now;
            pessoa.Endereco = pessoaNFe.Endereco.Logradouro;
            pessoa.IE_RG = pessoaNFe.Endereco.InscricaoEstadual;
            pessoa.Email = pessoaNFe.Email;
            if (pessoaNFe.Endereco.Cidade.SiglaUF == "EX")
            {
                pessoa.RG_Passaporte = pessoaNFe.RGIE;
                pessoa.Localidade = CadastrarCidade(pessoaNFe.Endereco.Cidade.IBGE, pessoaNFe.Endereco.CEP, pessoaNFe.Endereco.Cidade.Descricao, pessoaNFe.Endereco.Cidade.SiglaUF, pessoaNFe.Endereco.Cidade.Pais.CodigoPais, unitOfWork);
            }
            else
                pessoa.Localidade = repLocalidade.BuscarPorCodigoIBGE(pessoaNFe.Endereco.Cidade.IBGE);
            pessoa.Nome = pessoaNFe.RazaoSocial;
            pessoa.NomeFantasia = pessoaNFe.NomeFantasia;
            pessoa.Numero = pessoaNFe.Endereco.Numero;
            pessoa.Telefone1 = pessoaNFe.Endereco.Telefone;
            pessoa.TipoLocalizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.endereco;
            pessoa.TipoContaBanco = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Corrente;
            pessoa.TipoEmail = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Principal;
            pessoa.ValorTDE = 0;
            pessoa.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.Normal;
            pessoa.TipoEmissaoCTeDocumentos = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado;

            if (pessoaNFe.ClienteExterior)
                pessoa.Tipo = "E";
            if (pessoaNFe.CPFCNPJ.Length == 11)
                pessoa.Tipo = "F";
            else
                pessoa.Tipo = "J";

            if (pessoa.Tipo == "J" && pessoa.GrupoPessoas == null)
            {
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(pessoa.CPF_CNPJ_Formatado).Remove(8, 6));
                if (grupoPessoas != null)
                {
                    pessoa.GrupoPessoas = grupoPessoas;
                }
            }
            pessoa.Ativo = true;
            repCliente.Inserir(pessoa);

            modalidade.Cliente = pessoa;
            modalidade.TipoModalidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Cliente;
            repModalidadePessoas.Inserir(modalidade);

            modalidadeCliente.ModalidadePessoas = modalidade;
            repModalidadeClientePessoas.Inserir(modalidadeCliente);
        }

        public Dominio.Entidades.Localidade CadastrarCidade(int codigoIBGE, string cep, string descricao, string estado, int codigoPais, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
            Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigoIBGE(codigoIBGE, codigoPais);
            if (localidade != null)
                return localidade;
            else
            {
                localidade = repLocalidade.BuscarPorPais(codigoPais);
                if (localidade != null)
                    return localidade;

                localidade = new Dominio.Entidades.Localidade();
                localidade.CEP = cep;
                localidade.CodigoIBGE = codigoIBGE;
                localidade.Descricao = descricao;
                localidade.Estado = repEstado.BuscarPorSigla(estado);
                localidade.Pais = repPais.BuscarPorCodigo(codigoPais);

                repLocalidade.Inserir(localidade);
                return localidade;
            }
        }

        public void CadastrarSerieEmpresa(int numeroSerie, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
            Dominio.Entidades.EmpresaSerie serie = new Dominio.Entidades.EmpresaSerie();

            serie.Empresa = empresa;
            serie.Numero = numeroSerie;
            serie.Status = "A";
            serie.Tipo = Dominio.Enumeradores.TipoSerie.NFe;

            repEmpresaSerie.Inserir(serie);
        }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa PreencherPessoa(string xNome, string ie, string isuf, string im, string email)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa
            {
                CodigoIntegracao = "0",
                NomeFantasia = xNome,
                RazaoSocial = xNome.ToUpper(),
                RGIE = ie,
                Email = email,
                IM = im
            };
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS? ConverterCSTPISCOFNS(string cst)
        {
            if (cst == "01")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01;
            else if (cst == "02")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST02;
            else if (cst == "03")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03;
            else if (cst == "04")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST04;
            else if (cst == "05")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST05;
            else if (cst == "06")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST06;
            else if (cst == "07")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST07;
            else if (cst == "08")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST08;
            else if (cst == "09")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST09;
            else if (cst == "49")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST49;
            else if (cst == "50")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST50;
            else if (cst == "51")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST51;
            else if (cst == "52")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST52;
            else if (cst == "53")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST53;
            else if (cst == "54")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST54;
            else if (cst == "55")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST55;
            else if (cst == "56")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST56;
            else if (cst == "60")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST60;
            else if (cst == "61")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST61;
            else if (cst == "62")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST62;
            else if (cst == "63")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST63;
            else if (cst == "64")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST64;
            else if (cst == "65")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST65;
            else if (cst == "66")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST66;
            else if (cst == "67")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST67;
            else if (cst == "70")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST70;
            else if (cst == "71")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST71;
            else if (cst == "72")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST72;
            else if (cst == "73")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST73;
            else if (cst == "74")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST74;
            else if (cst == "75")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST75;
            else if (cst == "98")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST98;
            else if (cst == "99")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST99;
            else
                return null;
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI? ConverterCSTIIPI(string cst)
        {
            if (cst == "00")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST00;
            else if (cst == "01")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST01;
            else if (cst == "02")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST02;
            else if (cst == "03")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST03;
            else if (cst == "04")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST04;
            else if (cst == "05")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST05;
            else if (cst == "49")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST49;
            else if (cst == "50")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST50;
            else if (cst == "51")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST51;
            else if (cst == "52")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST52;
            else if (cst == "53")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST53;
            else if (cst == "54")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST54;
            else if (cst == "55")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST55;
            else if (cst == "99")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST99;
            else
                return null;
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS? ConverterCSTICMS(string cst)
        {
            if (cst == "00")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST00;
            else if (cst == "10")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST10;
            else if (cst == "20")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST20;
            else if (cst == "30")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST30;
            else if (cst == "40")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST40;
            else if (cst == "41")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST41;
            else if (cst == "50")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST50;
            else if (cst == "51")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST51;
            else if (cst == "60")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST60;
            else if (cst == "70")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST70;
            else if (cst == "90")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST90;
            else if (cst == "101")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN101;
            else if (cst == "102")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN102;
            else if (cst == "103")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN103;
            else if (cst == "201")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN201;
            else if (cst == "202")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN202;
            else if (cst == "203")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN203;
            else if (cst == "300")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN300;
            else if (cst == "400")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN400;
            else if (cst == "500")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN500;
            else if (cst == "500")
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN900;
            else
                return null;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica> ConverterLayoutTXTParaNFe(Stream stream, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Empresa empresa)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Localidades.Localidade(unitOfWork);
            Servicos.Embarcador.NotaFiscal.NotaFiscalParcela serNotaFiscalParcela = new NotaFiscalParcela(unitOfWork);
            Servicos.Embarcador.NotaFiscal.NotaFiscalProduto serNotaFiscalProduto = new NotaFiscalProduto(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica> retornoNFe = new List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica>();
            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica nfe = null;
            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProduto item = null;
            StreamReader streamReader = new StreamReader(stream);
            int linha = 0;
            var cellValue = "";
            while ((cellValue = streamReader.ReadLine()) != null)
            {
                string[] linhaSeparada = cellValue.Split('|');
                if (linhaSeparada[0] == "NOTAFISCAL")//A|versão do schema|id| 
                {
                    nfe = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica();
                    nfe.ItensNFe = new List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProduto>();
                    nfe.TranspPesoBruto = 0;
                    nfe.TranspPesoLiquido = 0;
                    nfe.CodigoEmpresa = empresa.Codigo;
                    nfe.Codigo = -2;
                    nfe.IndicadorPresenca = Dominio.Enumeradores.IndicadorPresencaNFe.NaoSeAplica;
                }
                else if (linhaSeparada[0] == "A")//A|versão do schema|id| 
                {

                }
                else if (linhaSeparada[0] == "B")//B|cUF|cNF|natOp|indPag|mod|serie|nNF|dhEmi|dhSaiEnt|tpNF|idDest|cMunFG|tpImp|tpEmis|cDV|tpAmb|finNFe|indFinal|indPres|procEmi|verProc|dhCont|xJust|
                {
                    nfe.NaturezaDaOperacao = CadastrarNaturezaOperacao(linhaSeparada[3], unitOfWork, empresa.Codigo);
                    nfe.Serie = int.Parse(linhaSeparada[6]);
                    nfe.Numero = int.Parse(linhaSeparada[7]);

                    DateTime dataEmissao = new DateTime();
                    DateTime.TryParseExact(linhaSeparada[8].Substring(0, 16).Replace("T", " ").Replace("-", "/"), "yyyy/MM/dd HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                    nfe.DataEmissao = dataEmissao;

                    DateTime dataSaida = new DateTime();
                    DateTime.TryParseExact(linhaSeparada[9].Substring(0, 16).Replace("T", " ").Replace("-", "/"), "yyyy/MM/dd HH:mm", null, System.Globalization.DateTimeStyles.None, out dataSaida);
                    nfe.DataSaida = dataSaida;

                    nfe.TipoEmissao = (Dominio.Enumeradores.TipoEmissaoNFe)int.Parse(linhaSeparada[10]);
                    nfe.Finalidade = (Dominio.Enumeradores.FinalidadeNFe)int.Parse(linhaSeparada[17]);
                    //nfe.TipoAmbiente
                }
                else if (linhaSeparada[0] == "BA02")//BA02|refNFe|
                {
                    nfe.ReferenciaNFe = new List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia> { new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia {
                        Codigo = 0,
                        TipoDocumento = Dominio.Enumeradores.TipoDocumentoReferenciaNFe.NF,
                        Chave = linhaSeparada[1]
                    } };
                }
                else if (linhaSeparada[0] == "BA03")//BA03|cUF|AAMM|CNPJ|mod|serie|nNF|  
                {
                    nfe.ReferenciaNFe = new List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia> { new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia {
                        Codigo = 0,
                        TipoDocumento = Dominio.Enumeradores.TipoDocumentoReferenciaNFe.NFModelo1,
                        CNPJEmitente = linhaSeparada[3],
                        DataEmissao = DateTime.Parse(linhaSeparada[2]),
                        Modelo = linhaSeparada[4],
                        Numero = linhaSeparada[6],
                        Serie = linhaSeparada[5],
                        UF = linhaSeparada[1]
                    } };
                }
                else if (linhaSeparada[0] == "BA10")//BA10|cUF|AAMM|IE|mod|serie|nNF|refCTe|
                {
                    nfe.ReferenciaNFe = new List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia> { new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia {
                        Codigo = 0,
                        TipoDocumento = !string.IsNullOrWhiteSpace(linhaSeparada[7]) ? Dominio.Enumeradores.TipoDocumentoReferenciaNFe.NFProdutorRural : Dominio.Enumeradores.TipoDocumentoReferenciaNFe.CTe,
                        IEEmitente = linhaSeparada[3],
                        DataEmissao = DateTime.Parse(linhaSeparada[2]),
                        Modelo = linhaSeparada[4],
                        Numero = linhaSeparada[6],
                        Serie = linhaSeparada[5],
                        UF = linhaSeparada[1],
                        Chave = linhaSeparada[7]
                    } };
                }
                else if (linhaSeparada[0] == "BA13")//B20d|CNPJ| 
                {
                    if (nfe.ReferenciaNFe != null && nfe.ReferenciaNFe.Count > 0)
                        nfe.ReferenciaNFe[0].CNPJEmitente = linhaSeparada[1];
                }
                else if (linhaSeparada[0] == "BA14")//B20e|CPF|
                {
                    if (nfe.ReferenciaNFe != null && nfe.ReferenciaNFe.Count > 0)
                        nfe.ReferenciaNFe[0].CNPJEmitente = linhaSeparada[1];
                }
                else if (linhaSeparada[0] == "BA20")//BA20|mod|nECF|nCOO| 
                {
                    nfe.ReferenciaNFe = new List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia> { new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia {
                        Codigo = 0,
                        TipoDocumento = Dominio.Enumeradores.TipoDocumentoReferenciaNFe.CupomFiscal,
                        Modelo = linhaSeparada[1],
                        NumeroECF = linhaSeparada[2],
                        COO = linhaSeparada[3]
                } };
                }
                else if (linhaSeparada[0] == "C")//C|XNome|XFant|IE|IEST|IM|CNAE|CRT| 
                {
                    //dados da empresa logada
                }
                else if (linhaSeparada[0] == "C02")//C02|CNPJ|
                {
                    //dados da empresa logada
                }
                else if (linhaSeparada[0] == "C02a")//C02a|CPF|
                {
                    //dados da empresa logada
                }
                else if (linhaSeparada[0] == "C05")//C05|XLgr|Nro|Cpl|Bairro|CMun|XMun|UF|CEP|cPais|xPais|fone| 
                {
                    //dados da empresa logada
                }
                else if (linhaSeparada[0] == "D")//D|CNPJ|xOrgao|matr|xAgente|fone|UF|nDAR|dEmi|vDAR|repEmi|dPag| 
                {

                }
                else if (linhaSeparada[0] == "E")//E|xNome|indIEDest|IE|ISUF|IM|email| 
                {
                    nfe.Cliente = PreencherPessoa(linhaSeparada[1], linhaSeparada[3], linhaSeparada[4], linhaSeparada[5], linhaSeparada[6]);
                }
                else if (linhaSeparada[0] == "E02")//E02|CNPJ| 
                {
                    if (nfe.Cliente != null)
                        nfe.Cliente.CPFCNPJ = linhaSeparada[1];
                }
                else if (linhaSeparada[0] == "E03")//E03|CPF| 
                {
                    if (nfe.Cliente != null)
                        nfe.Cliente.CPFCNPJ = linhaSeparada[1];
                }
                else if (linhaSeparada[0] == "E03a")//E03a|idEstrangeiro| 
                {
                    if (nfe.Cliente != null)
                    {
                        nfe.Cliente.ClienteExterior = true;
                        nfe.Cliente.CPFCNPJ = repCliente.BuscarPorProximoExterior().ToString();
                        nfe.Cliente.RGIE = linhaSeparada[1];
                    }
                }
                else if (linhaSeparada[0] == "E05")//E05|xLgr|nro|xCpl|xBairro|cMun|xMun|UF|CEP|cPais|xPais|fone|
                {
                    if (nfe.Cliente != null)
                    {
                        nfe.Cliente.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco
                        {
                            Bairro = linhaSeparada[4],
                            CEP = Utilidades.String.OnlyNumbers(linhaSeparada[8]),
                            Cidade = new Dominio.ObjetosDeValor.Localidade { SiglaUF = linhaSeparada[7], IBGE = int.Parse(linhaSeparada[5]), Descricao = linhaSeparada[6], Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais { CodigoPais = int.Parse(linhaSeparada[9]), NomePais = linhaSeparada[10] } },
                            Complemento = linhaSeparada[3],
                            Logradouro = linhaSeparada[1],
                            Numero = linhaSeparada[2],
                            Telefone = linhaSeparada[11]
                        };
                        if (linhaSeparada[7] == "EX")
                        {
                            nfe.Cliente.ClienteExterior = true;
                            nfe.Cliente.CPFCNPJ = repCliente.BuscarPorProximoExterior().ToString();
                            nfe.Cliente.RGIE = nfe.Cliente.CPFCNPJ;
                            nfe.Cliente.Endereco.CEP = "00000000";
                        }
                    }
                }
                else if (linhaSeparada[0] == "F")//F|CNPJ|XLgr|Nro|XCpl|XBairro|CMun|XMun|UF|
                {

                }
                else if (linhaSeparada[0] == "F02")//F02|CNPJ 
                {

                }
                else if (linhaSeparada[0] == "F02a")//F02a|CPF 
                {

                }
                else if (linhaSeparada[0] == "G")//G|CNPJ|XLgr|Nro|XCpl|XBairro|CMun|XMun|UF|
                {

                }
                else if (linhaSeparada[0] == "G02")//G02|CNPJ 
                {

                }
                else if (linhaSeparada[0] == "G02a")//G02a|CPF 
                {

                }
                else if (linhaSeparada[0] == "H")//H|nItem|infAdProd|
                {
                    if (item != null)
                    {
                        nfe.ItensNFe.Add(item);
                        item = null;
                    }
                    item = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalProduto();
                }
                else if (linhaSeparada[0] == "I")//I|cProd|cEAN|xProd|NCM|EXTIPI|CFOP|uCom|qCom|vUnCom|vProd|cEANTrib|uTrib|qTrib|vUnTrib|vFrete|vSeg|vDesc|vOutro|indTot|xPed|nItemPed|nFCI|
                {
                    if (item != null)
                    {
                        if (linhaSeparada[4] == "00")
                            item.Servico = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.Servico { Codigo = 0, CodigoIntegracao = linhaSeparada[1], Descricao = linhaSeparada[3] };
                        else
                            item.Produto = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.Produto { Codigo = 0, CodigoIntegracao = linhaSeparada[1], Descricao = linhaSeparada[3], NCM = linhaSeparada[4] };
                        item.CFOP = CadastrarCFOP(int.Parse(linhaSeparada[6]), empresa.Codigo, nfe.TipoEmissao == Dominio.Enumeradores.TipoEmissaoNFe.Entrada ? Dominio.Enumeradores.TipoCFOP.Entrada : Dominio.Enumeradores.TipoCFOP.Saida, unitOfWork);
                        item.NumeroItemOrdemCompra = linhaSeparada[21];
                        item.NumeroOrdemCompra = linhaSeparada[20];
                        item.Quantidade = !string.IsNullOrWhiteSpace(linhaSeparada[8]) ? decimal.Parse(linhaSeparada[8].Replace(".", ",")) : 0;
                        item.ValorDesconto = !string.IsNullOrWhiteSpace(linhaSeparada[17]) ? decimal.Parse(linhaSeparada[17].Replace(".", ",")) : 0;
                        item.ValorFrete = !string.IsNullOrWhiteSpace(linhaSeparada[15]) ? decimal.Parse(linhaSeparada[15].Replace(".", ",")) : 0;
                        item.ValorOutrasDespesas = !string.IsNullOrWhiteSpace(linhaSeparada[18]) ? decimal.Parse(linhaSeparada[18].Replace(".", ",")) : 0;
                        item.ValorSeguro = !string.IsNullOrWhiteSpace(linhaSeparada[16]) ? decimal.Parse(linhaSeparada[16].Replace(".", ",")) : 0;
                        item.ValorUnitario = !string.IsNullOrWhiteSpace(linhaSeparada[9]) ? decimal.Parse(linhaSeparada[9].Replace(".", ",")) : 0;
                        item.ValorTotal = !string.IsNullOrWhiteSpace(linhaSeparada[10]) ? decimal.Parse(linhaSeparada[10].Replace(".", ",")) : 0;
                    }
                }
                else if (linhaSeparada[0] == "I18")//I18|nDI|dDI|xLocDesemb|UFDesemb|dDesemb|tpViaTransp|vAFRMM|tpIntermedio|CNPJ|UFTerceiro|cExportador| 
                {
                    if (item != null)
                    {
                        item.NumeroDocImportacao = linhaSeparada[1];
                        item.DataRegistroImportacao = DateTime.Parse(linhaSeparada[2]);
                        item.LocalDesembaraco = linhaSeparada[3];
                        item.UFDesembaraco = linhaSeparada[4];
                        item.DataDesembaraco = DateTime.Parse(linhaSeparada[5]);
                        item.ViaTransporteII = (Dominio.Enumeradores.ViaTransporteInternacional)int.Parse(linhaSeparada[6]);
                        item.IntermediacaoII = (Dominio.Enumeradores.IntermediacaoImportacao)int.Parse(linhaSeparada[8]);
                        item.CNPJAdquirente = linhaSeparada[9];
                    }
                }
                else if (linhaSeparada[0] == "I25")//I25|NAdicao|NSeqAdic|CFabricante|VDescDI| 
                {

                }
                else if (linhaSeparada[0] == "J")//J|TpOp|Chassi|CCor|XCor|Pot|cilin|pesoL|pesoB|NSerie|TpComb|NMotor|CMT|Dist|anoMod|anoFab|tpPint|tpVeic|espVeic|VIN | condVeic | cMod | cCorDENATRAN | lota | tpRest |
                {

                }
                else if (linhaSeparada[0] == "K")//K|NLote|QLote|DFab|DVal|VPMC|
                {

                }
                else if (linhaSeparada[0] == "L")//L|TpArma|NSerie|NCano|Descr|
                {

                }
                else if (linhaSeparada[0] == "L01")//L01|CProdANP|CODIF|QTemp|UFCons| 
                {

                }
                else if (linhaSeparada[0] == "L105")//L105|QBCProd|VAliqProd|VCIDE| 
                {

                }
                else if (linhaSeparada[0] == "N02")//N02|Orig|CST|ModBC|VBC|PICMS|VICMS|
                {
                    item.CSTICMS = ConverterCSTICMS(linhaSeparada[2]);
                    item.BCICMS = !string.IsNullOrWhiteSpace(linhaSeparada[4]) ? decimal.Parse(linhaSeparada[4].Replace(".", ",")) : 0;
                    item.AliquotaICMS = !string.IsNullOrWhiteSpace(linhaSeparada[5]) ? decimal.Parse(linhaSeparada[5].Replace(".", ",")) : 0;
                    item.ValorICMS = !string.IsNullOrWhiteSpace(linhaSeparada[6]) ? decimal.Parse(linhaSeparada[6].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "N03")//N03|Orig|CST|ModBC|VBC|PICMS|VICMS|ModBCST|PMVAST 8 |PRedBCST|VBCST|PICMSST|VICMSST| 
                {
                    item.CSTICMS = ConverterCSTICMS(linhaSeparada[2]);
                    item.BCICMS = !string.IsNullOrWhiteSpace(linhaSeparada[4]) ? decimal.Parse(linhaSeparada[4].Replace(".", ",")) : 0;
                    item.AliquotaICMS = !string.IsNullOrWhiteSpace(linhaSeparada[5]) ? decimal.Parse(linhaSeparada[5].Replace(".", ",")) : 0;
                    item.ValorICMS = !string.IsNullOrWhiteSpace(linhaSeparada[6]) ? decimal.Parse(linhaSeparada[6].Replace(".", ",")) : 0;
                    item.MVAICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[8]) ? decimal.Parse(linhaSeparada[8].Replace(".", ",")) : 0;
                    item.ReducaoBCICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[9]) ? decimal.Parse(linhaSeparada[9].Replace(".", ",")) : 0;
                    item.BCICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[10]) ? decimal.Parse(linhaSeparada[10].Replace(".", ",")) : 0;
                    item.AliquotaICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[11]) ? decimal.Parse(linhaSeparada[11].Replace(".", ",")) : 0;
                    item.AliquotaICMSSTInterestadual = !string.IsNullOrWhiteSpace(linhaSeparada[5]) ? decimal.Parse(linhaSeparada[5].Replace(".", ",")) : 0;
                    item.ValorICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[12]) ? decimal.Parse(linhaSeparada[12].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "N04")//N04|orig|CST|modBC|pRedBC|vBC|pICMS|vICMS|vICMSDeson|motDesICMS| 
                {
                    item.CSTICMS = ConverterCSTICMS(linhaSeparada[2]);
                    item.BCICMS = !string.IsNullOrWhiteSpace(linhaSeparada[5]) ? decimal.Parse(linhaSeparada[5].Replace(".", ",")) : 0;
                    item.AliquotaICMS = !string.IsNullOrWhiteSpace(linhaSeparada[6]) ? decimal.Parse(linhaSeparada[6].Replace(".", ",")) : 0;
                    item.ValorICMS = !string.IsNullOrWhiteSpace(linhaSeparada[7]) ? decimal.Parse(linhaSeparada[7].Replace(".", ",")) : 0;
                    item.ValorICMSDesonerado = !string.IsNullOrWhiteSpace(linhaSeparada[8]) ? decimal.Parse(linhaSeparada[8].Replace(".", ",")) : 0;
                    item.MotivoDesoneracao = (Dominio.Enumeradores.MotivoDesoneracaoICMS)int.Parse(linhaSeparada[9]);
                }
                else if (linhaSeparada[0] == "N05")//N05|Orig|CST|ModBCST|PMVAST|PRedBCST|VBCST|PICMSST|VICMSST|vICMSDeson|motDesICMS| 
                {
                    item.CSTICMS = ConverterCSTICMS(linhaSeparada[2]);
                    item.MVAICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[4]) ? decimal.Parse(linhaSeparada[4].Replace(".", ",")) : 0;
                    item.ReducaoBCICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[5]) ? decimal.Parse(linhaSeparada[5].Replace(".", ",")) : 0;
                    item.BCICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[6]) ? decimal.Parse(linhaSeparada[6].Replace(".", ",")) : 0;
                    item.AliquotaICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[7]) ? decimal.Parse(linhaSeparada[7].Replace(".", ",")) : 0;
                    item.ValorICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[8]) ? decimal.Parse(linhaSeparada[8].Replace(".", ",")) : 0;
                    item.ValorICMSDesonerado = !string.IsNullOrWhiteSpace(linhaSeparada[9]) ? decimal.Parse(linhaSeparada[9].Replace(".", ",")) : 0;
                    item.MotivoDesoneracao = (Dominio.Enumeradores.MotivoDesoneracaoICMS)int.Parse(linhaSeparada[10]);
                }
                else if (linhaSeparada[0] == "N06")//N06|orig|CST|vICMSDeson|motDesICMS|
                {
                    item.CSTICMS = ConverterCSTICMS(linhaSeparada[2]);
                    item.ValorICMSDesonerado = !string.IsNullOrWhiteSpace(linhaSeparada[3]) ? decimal.Parse(linhaSeparada[3].Replace(".", ",")) : 0;
                    //item.MotivoDesoneracao = (Dominio.Enumeradores.MotivoDesoneracaoICMS)int.Parse(linhaSeparada[4]);
                }
                else if (linhaSeparada[0] == "N07")//N07|orig|CST|modBC|pRedBC|vBC|pICMS|vICMSOp|pDif|vICMSDif|vICMS| 
                {
                    item.CSTICMS = ConverterCSTICMS(linhaSeparada[2]);
                    item.BCICMS = !string.IsNullOrWhiteSpace(linhaSeparada[5]) ? decimal.Parse(linhaSeparada[5].Replace(".", ",")) : 0;
                    item.AliquotaICMS = !string.IsNullOrWhiteSpace(linhaSeparada[6]) ? decimal.Parse(linhaSeparada[6].Replace(".", ",")) : 0;
                    item.ValorICMS = !string.IsNullOrWhiteSpace(linhaSeparada[10]) ? decimal.Parse(linhaSeparada[10].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "N08")//N08|orig|CST|vBCSTRet|vICMSSTRet| 
                {
                    item.CSTICMS = ConverterCSTICMS(linhaSeparada[2]);
                    item.BCICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[3]) ? decimal.Parse(linhaSeparada[3].Replace(".", ",")) : 0;
                    item.ValorICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[4]) ? decimal.Parse(linhaSeparada[4].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "N09")//N09|orig|CST|modBC|pRedBC|vBC|pICMS|vICMS|modBCST|pMVAST|pRedBCST|vBCST|pICMSST|vICMSST|vICMSDeson|motDesICMS|
                {
                    item.CSTICMS = ConverterCSTICMS(linhaSeparada[2]);
                    item.BCICMS = !string.IsNullOrWhiteSpace(linhaSeparada[5]) ? decimal.Parse(linhaSeparada[5].Replace(".", ",")) : 0;
                    item.AliquotaICMS = !string.IsNullOrWhiteSpace(linhaSeparada[6]) ? decimal.Parse(linhaSeparada[6].Replace(".", ",")) : 0;
                    item.ValorICMS = !string.IsNullOrWhiteSpace(linhaSeparada[7]) ? decimal.Parse(linhaSeparada[7].Replace(".", ",")) : 0;
                    item.MVAICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[9]) ? decimal.Parse(linhaSeparada[9].Replace(".", ",")) : 0;
                    item.ReducaoBCICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[10]) ? decimal.Parse(linhaSeparada[10].Replace(".", ",")) : 0;
                    item.BCICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[11]) ? decimal.Parse(linhaSeparada[11].Replace(".", ",")) : 0;
                    item.AliquotaICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[12]) ? decimal.Parse(linhaSeparada[12].Replace(".", ",")) : 0;
                    item.AliquotaICMSSTInterestadual = !string.IsNullOrWhiteSpace(linhaSeparada[6]) ? decimal.Parse(linhaSeparada[6].Replace(".", ",")) : 0;
                    item.ValorICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[13]) ? decimal.Parse(linhaSeparada[13].Replace(".", ",")) : 0;
                    item.ValorICMSDesonerado = !string.IsNullOrWhiteSpace(linhaSeparada[14]) ? decimal.Parse(linhaSeparada[14].Replace(".", ",")) : 0;
                    item.MotivoDesoneracao = (Dominio.Enumeradores.MotivoDesoneracaoICMS)int.Parse(linhaSeparada[15]);
                }
                else if (linhaSeparada[0] == "N10")//N10|Orig|CST|ModBC|PRedBC|VBC|PICMS|VICMS|ModBCST|PMVAST|PRedBCST|VBCST|PICMSST|VICMSST|vICMSDeson|motDesICMS|
                {
                    item.CSTICMS = ConverterCSTICMS(linhaSeparada[2]);
                    item.BCICMS = !string.IsNullOrWhiteSpace(linhaSeparada[5]) ? decimal.Parse(linhaSeparada[5].Replace(".", ",")) : 0;
                    item.AliquotaICMS = !string.IsNullOrWhiteSpace(linhaSeparada[6]) ? decimal.Parse(linhaSeparada[6].Replace(".", ",")) : 0;
                    item.ValorICMS = !string.IsNullOrWhiteSpace(linhaSeparada[7]) ? decimal.Parse(linhaSeparada[7].Replace(".", ",")) : 0;
                    item.MVAICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[9]) ? decimal.Parse(linhaSeparada[9].Replace(".", ",")) : 0;
                    item.ReducaoBCICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[10]) ? decimal.Parse(linhaSeparada[10].Replace(".", ",")) : 0;
                    item.BCICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[11]) ? decimal.Parse(linhaSeparada[11].Replace(".", ",")) : 0;
                    item.AliquotaICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[12]) ? decimal.Parse(linhaSeparada[12].Replace(".", ",")) : 0;
                    item.AliquotaICMSSTInterestadual = !string.IsNullOrWhiteSpace(linhaSeparada[6]) ? decimal.Parse(linhaSeparada[6].Replace(".", ",")) : 0;
                    item.ValorICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[13]) ? decimal.Parse(linhaSeparada[13].Replace(".", ",")) : 0;
                    item.ValorICMSDesonerado = !string.IsNullOrWhiteSpace(linhaSeparada[14]) ? decimal.Parse(linhaSeparada[14].Replace(".", ",")) : 0;
                    item.MotivoDesoneracao = (Dominio.Enumeradores.MotivoDesoneracaoICMS)int.Parse(linhaSeparada[15]);
                }
                else if (linhaSeparada[0] == "N10a")//N10a|Orig|CST|ModBC|PRedBC|VBC|PICMS|VICMS 7 |ModBCST|PMVAST|PRedBCST|VBCST|PICMSST|VICMSST|pBCOp|UFST| 
                {
                    item.CSTICMS = ConverterCSTICMS(linhaSeparada[2]);
                    item.BCICMS = !string.IsNullOrWhiteSpace(linhaSeparada[5]) ? decimal.Parse(linhaSeparada[5].Replace(".", ",")) : 0;
                    item.AliquotaICMS = !string.IsNullOrWhiteSpace(linhaSeparada[6]) ? decimal.Parse(linhaSeparada[6].Replace(".", ",")) : 0;
                    item.ValorICMS = !string.IsNullOrWhiteSpace(linhaSeparada[7]) ? decimal.Parse(linhaSeparada[7].Replace(".", ",")) : 0;
                    item.MVAICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[9]) ? decimal.Parse(linhaSeparada[9].Replace(".", ",")) : 0;
                    item.ReducaoBCICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[10]) ? decimal.Parse(linhaSeparada[10].Replace(".", ",")) : 0;
                    item.BCICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[11]) ? decimal.Parse(linhaSeparada[11].Replace(".", ",")) : 0;
                    item.AliquotaICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[12]) ? decimal.Parse(linhaSeparada[12].Replace(".", ",")) : 0;
                    item.AliquotaICMSSTInterestadual = !string.IsNullOrWhiteSpace(linhaSeparada[6]) ? decimal.Parse(linhaSeparada[6].Replace(".", ",")) : 0;
                    item.ValorICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[13]) ? decimal.Parse(linhaSeparada[13].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "N10b")//N10b|Orig|CST|vBCSTRet|vICMSSTRet|vBCSTDest|vICMSSTDest| 
                {
                    item.CSTICMS = ConverterCSTICMS(linhaSeparada[2]);
                }
                else if (linhaSeparada[0] == "N10c")//N10c|Orig|CSOSN|pCredSN|vCredICMSSN| 
                {
                    item.CSTICMS = ConverterCSTICMS(linhaSeparada[2]);
                }
                else if (linhaSeparada[0] == "N10d")//N10d|Orig|CSOSN| 
                {
                    item.CSTICMS = ConverterCSTICMS(linhaSeparada[2]);
                }
                else if (linhaSeparada[0] == "N10e")//N10e|Orig|CSOSN|modBCST|pMVAST|pRedBCST|vBCST|pICMSST|vICMSST|pCredSN|vCredICMSSN| 
                {
                    item.CSTICMS = ConverterCSTICMS(linhaSeparada[2]);
                    item.MVAICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[4]) ? decimal.Parse(linhaSeparada[4].Replace(".", ",")) : 0;
                    item.ReducaoBCICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[5]) ? decimal.Parse(linhaSeparada[5].Replace(".", ",")) : 0;
                    item.BCICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[6]) ? decimal.Parse(linhaSeparada[6].Replace(".", ",")) : 0;
                    item.AliquotaICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[7]) ? decimal.Parse(linhaSeparada[7].Replace(".", ",")) : 0;
                    item.ValorICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[8]) ? decimal.Parse(linhaSeparada[8].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "N10f")//N10f|Orig|CSOSN|modBCST|pMVAST|pRedBCST|vBCST|pICMSST|vICMSST| 
                {
                    item.CSTICMS = ConverterCSTICMS(linhaSeparada[2]);
                    item.MVAICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[4]) ? decimal.Parse(linhaSeparada[4].Replace(".", ",")) : 0;
                    item.ReducaoBCICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[5]) ? decimal.Parse(linhaSeparada[5].Replace(".", ",")) : 0;
                    item.BCICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[6]) ? decimal.Parse(linhaSeparada[6].Replace(".", ",")) : 0;
                    item.AliquotaICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[7]) ? decimal.Parse(linhaSeparada[7].Replace(".", ",")) : 0;
                    item.ValorICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[8]) ? decimal.Parse(linhaSeparada[8].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "N10g")//N10g|Orig|CSOSN|modBCST|vBCSTRet|vICMSSTRet| 
                {
                    item.CSTICMS = ConverterCSTICMS(linhaSeparada[2]);
                }
                else if (linhaSeparada[0] == "N10h")//N10h|Orig|CSOSN|modBC|vBC|pRedBC|pICMS|vICMS 7 |modBCST|pMVAST|pRedBCST|vBCST|pICMSST|vICMSST|pCredSN|vCredICMSSN |
                {
                    item.CSTICMS = ConverterCSTICMS(linhaSeparada[2]);
                    item.BCICMS = !string.IsNullOrWhiteSpace(linhaSeparada[5]) ? decimal.Parse(linhaSeparada[5].Replace(".", ",")) : 0;
                    item.AliquotaICMS = !string.IsNullOrWhiteSpace(linhaSeparada[6]) ? decimal.Parse(linhaSeparada[6].Replace(".", ",")) : 0;
                    item.ValorICMS = !string.IsNullOrWhiteSpace(linhaSeparada[7]) ? decimal.Parse(linhaSeparada[7].Replace(".", ",")) : 0;
                    item.MVAICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[9]) ? decimal.Parse(linhaSeparada[9].Replace(".", ",")) : 0;
                    item.ReducaoBCICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[10]) ? decimal.Parse(linhaSeparada[10].Replace(".", ",")) : 0;
                    item.BCICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[11]) ? decimal.Parse(linhaSeparada[11].Replace(".", ",")) : 0;
                    item.AliquotaICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[12]) ? decimal.Parse(linhaSeparada[12].Replace(".", ",")) : 0;
                    item.AliquotaICMSSTInterestadual = !string.IsNullOrWhiteSpace(linhaSeparada[6]) ? decimal.Parse(linhaSeparada[6].Replace(".", ",")) : 0;
                    item.ValorICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[13]) ? decimal.Parse(linhaSeparada[13].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "O")//O|ClEnq|CNPJProd|CSelo|QSelo|CEnq| 
                {

                }
                else if (linhaSeparada[0] == "O07")//O07|CST|VIPI|
                {
                    item.CSTIPI = ConverterCSTIIPI(linhaSeparada[2]);
                    item.ValorIPI = !string.IsNullOrWhiteSpace(linhaSeparada[3]) ? decimal.Parse(linhaSeparada[3].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "O10")//O10|VBC|PIPI| 
                {
                    item.BCIPI = !string.IsNullOrWhiteSpace(linhaSeparada[1]) ? decimal.Parse(linhaSeparada[1]) : 0;
                    item.AliquotaIPI = !string.IsNullOrWhiteSpace(linhaSeparada[2]) ? decimal.Parse(linhaSeparada[2].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "O11")//O11|QUnid|VUnid| 
                {

                }
                else if (linhaSeparada[0] == "O08")//O08|CST|
                {
                    item.CSTIPI = ConverterCSTIIPI(linhaSeparada[1]);
                }
                else if (linhaSeparada[0] == "P")//P|VBC|VDespAdu|VII|VIOF| 
                {
                    item.BaseII = !string.IsNullOrWhiteSpace(linhaSeparada[1]) ? decimal.Parse(linhaSeparada[1]) : 0;
                    item.ValorDespesaII = !string.IsNullOrWhiteSpace(linhaSeparada[2]) ? decimal.Parse(linhaSeparada[2].Replace(".", ",")) : 0;
                    item.ValorII = !string.IsNullOrWhiteSpace(linhaSeparada[3]) ? decimal.Parse(linhaSeparada[3].Replace(".", ",")) : 0;
                    item.ValorIOFII = !string.IsNullOrWhiteSpace(linhaSeparada[4]) ? decimal.Parse(linhaSeparada[4].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "Q02")//Q02|CST|VBC|PPIS|VPIS| 
                {
                    item.CSTPIS = ConverterCSTPISCOFNS(linhaSeparada[1]);
                    item.BCPIS = !string.IsNullOrWhiteSpace(linhaSeparada[2]) ? decimal.Parse(linhaSeparada[2].Replace(".", ",")) : 0;
                    item.AliquotaPIS = !string.IsNullOrWhiteSpace(linhaSeparada[3]) ? decimal.Parse(linhaSeparada[3].Replace(".", ",")) : 0;
                    item.ValorPIS = !string.IsNullOrWhiteSpace(linhaSeparada[4]) ? decimal.Parse(linhaSeparada[4].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "Q03")//Q03|CST|QBCProd|VAliqProd|VPIS
                {
                    item.CSTPIS = ConverterCSTPISCOFNS(linhaSeparada[1]);
                    item.BCPIS = !string.IsNullOrWhiteSpace(linhaSeparada[2]) ? decimal.Parse(linhaSeparada[2].Replace(".", ",")) : 0;
                    item.AliquotaPIS = !string.IsNullOrWhiteSpace(linhaSeparada[3]) ? decimal.Parse(linhaSeparada[3].Replace(".", ",")) : 0;
                    item.ValorPIS = !string.IsNullOrWhiteSpace(linhaSeparada[4]) ? decimal.Parse(linhaSeparada[4].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "Q04")//Q04|CST| 
                {
                    item.CSTPIS = ConverterCSTPISCOFNS(linhaSeparada[1]);
                }
                else if (linhaSeparada[0] == "Q05")//Q05|CST|VPIS| 
                {
                    item.CSTPIS = ConverterCSTPISCOFNS(linhaSeparada[1]);
                    item.ValorPIS = !string.IsNullOrWhiteSpace(linhaSeparada[2]) ? decimal.Parse(linhaSeparada[2]) : 0;
                }
                else if (linhaSeparada[0] == "Q07")//Q07|VBC|PPIS| 
                {
                    item.BCPIS = !string.IsNullOrWhiteSpace(linhaSeparada[1]) ? decimal.Parse(linhaSeparada[1]) : 0;
                    item.AliquotaPIS = !string.IsNullOrWhiteSpace(linhaSeparada[5]) ? decimal.Parse(linhaSeparada[2].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "Q10")//Q10|QBCProd|VAliqProd| 
                {
                    item.BCPIS = !string.IsNullOrWhiteSpace(linhaSeparada[1]) ? decimal.Parse(linhaSeparada[1]) : 0;
                    item.AliquotaPIS = !string.IsNullOrWhiteSpace(linhaSeparada[2]) ? decimal.Parse(linhaSeparada[2].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "R")//R|VPIS|
                {
                    item.ValorPIS = !string.IsNullOrWhiteSpace(linhaSeparada[1]) ? decimal.Parse(linhaSeparada[1].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "R02")//R02|VBC|PPIS|
                {
                    item.BCPIS = !string.IsNullOrWhiteSpace(linhaSeparada[1]) ? decimal.Parse(linhaSeparada[1].Replace(".", ",")) : 0;
                    item.AliquotaPIS = !string.IsNullOrWhiteSpace(linhaSeparada[2]) ? decimal.Parse(linhaSeparada[2].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "R04")//R04|qBCProd|vAliqProd|vPIS| 
                {
                    item.BCPIS = !string.IsNullOrWhiteSpace(linhaSeparada[1]) ? decimal.Parse(linhaSeparada[1].Replace(".", ",")) : 0;
                    item.AliquotaPIS = !string.IsNullOrWhiteSpace(linhaSeparada[2]) ? decimal.Parse(linhaSeparada[2].Replace(".", ",")) : 0;
                    item.ValorPIS = !string.IsNullOrWhiteSpace(linhaSeparada[3]) ? decimal.Parse(linhaSeparada[3].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "S02")//S02|CST|VBC|PCOFINS|VCOFINS| 
                {
                    item.CSTCOFINS = ConverterCSTPISCOFNS(linhaSeparada[1]);
                    item.BCCOFINS = !string.IsNullOrWhiteSpace(linhaSeparada[2]) ? decimal.Parse(linhaSeparada[2].Replace(".", ",")) : 0;
                    item.AliquotaCOFINS = !string.IsNullOrWhiteSpace(linhaSeparada[3]) ? decimal.Parse(linhaSeparada[3].Replace(".", ",")) : 0;
                    item.ValorCOFINS = !string.IsNullOrWhiteSpace(linhaSeparada[4]) ? decimal.Parse(linhaSeparada[4].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "S03")//S03|CST|QBCProd|VAliqProd|VCOFINS| 
                {
                    item.CSTCOFINS = ConverterCSTPISCOFNS(linhaSeparada[1]);
                    item.BCCOFINS = !string.IsNullOrWhiteSpace(linhaSeparada[2]) ? decimal.Parse(linhaSeparada[2].Replace(".", ",")) : 0;
                    item.AliquotaCOFINS = !string.IsNullOrWhiteSpace(linhaSeparada[3]) ? decimal.Parse(linhaSeparada[3].Replace(".", ",")) : 0;
                    item.ValorCOFINS = !string.IsNullOrWhiteSpace(linhaSeparada[4]) ? decimal.Parse(linhaSeparada[4].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "S04")//S04|CST|
                {
                    item.CSTCOFINS = ConverterCSTPISCOFNS(linhaSeparada[1]);
                }
                else if (linhaSeparada[0] == "S05")//S05|CST|VCOFINS| 
                {
                    item.CSTCOFINS = ConverterCSTPISCOFNS(linhaSeparada[1]);
                    item.ValorCOFINS = !string.IsNullOrWhiteSpace(linhaSeparada[2]) ? decimal.Parse(linhaSeparada[2].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "S07")//S07|VBC|PCOFINS| 
                {
                    item.BCCOFINS = !string.IsNullOrWhiteSpace(linhaSeparada[1]) ? decimal.Parse(linhaSeparada[1].Replace(".", ",")) : 0;
                    item.AliquotaCOFINS = !string.IsNullOrWhiteSpace(linhaSeparada[2]) ? decimal.Parse(linhaSeparada[2].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "S09")//S09|QBCProd|VAliqProd|
                {
                    item.BCCOFINS = !string.IsNullOrWhiteSpace(linhaSeparada[1]) ? decimal.Parse(linhaSeparada[1].Replace(".", ",")) : 0;
                    item.AliquotaCOFINS = !string.IsNullOrWhiteSpace(linhaSeparada[2]) ? decimal.Parse(linhaSeparada[2].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "T")//T|VCOFINS|
                {
                    item.ValorCOFINS = !string.IsNullOrWhiteSpace(linhaSeparada[1]) ? decimal.Parse(linhaSeparada[1].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "T02")//T02|VBC|PCOFINS| 
                {
                    item.BCCOFINS = !string.IsNullOrWhiteSpace(linhaSeparada[1]) ? decimal.Parse(linhaSeparada[1].Replace(".", ",")) : 0;
                    item.AliquotaCOFINS = !string.IsNullOrWhiteSpace(linhaSeparada[2]) ? decimal.Parse(linhaSeparada[2].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "T04")//T04|QBCProd|VAliqProd|
                {
                    item.BCCOFINS = !string.IsNullOrWhiteSpace(linhaSeparada[1]) ? decimal.Parse(linhaSeparada[1].Replace(".", ",")) : 0;
                    item.AliquotaCOFINS = !string.IsNullOrWhiteSpace(linhaSeparada[2]) ? decimal.Parse(linhaSeparada[2].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "U")//U|vBC|vAliq|vISSQN|cMunFG|cListServ|vDeducao|vOutro|vDescIncond|vDescCond|vISSRet|indISS|cServico|cMun|cPais|nProcesso|indIncentivo|
                {
                    if (item.Servico != null)
                        item.Servico.CodigoServico = linhaSeparada[5];
                    item.BaseISS = !string.IsNullOrWhiteSpace(linhaSeparada[1]) ? decimal.Parse(linhaSeparada[1].Replace(".", ",")) : 0;
                    item.AliquotaISS = !string.IsNullOrWhiteSpace(linhaSeparada[2]) ? decimal.Parse(linhaSeparada[2].Replace(".", ",")) : 0;
                    item.ValorISS = !string.IsNullOrWhiteSpace(linhaSeparada[3]) ? decimal.Parse(linhaSeparada[3].Replace(".", ",")) : 0;
                    nfe.LocalidadePrestacaoServico = new Dominio.ObjetosDeValor.Localidade { Codigo = 0, CodigoIntegracao = linhaSeparada[4], Descricao = "", IBGE = int.Parse(linhaSeparada[4]) };
                    item.BCDeducao = !string.IsNullOrWhiteSpace(linhaSeparada[6]) ? decimal.Parse(linhaSeparada[6].Replace(".", ",")) : 0;
                    item.OutrasRetencoes = !string.IsNullOrWhiteSpace(linhaSeparada[7]) ? decimal.Parse(linhaSeparada[7].Replace(".", ",")) : 0;
                    item.DescontoIncondicional = !string.IsNullOrWhiteSpace(linhaSeparada[8]) ? decimal.Parse(linhaSeparada[8].Replace(".", ",")) : 0;
                    item.DescontoCondicional = !string.IsNullOrWhiteSpace(linhaSeparada[9]) ? decimal.Parse(linhaSeparada[9].Replace(".", ",")) : 0;
                    item.RetencaoISS = !string.IsNullOrWhiteSpace(linhaSeparada[10]) ? decimal.Parse(linhaSeparada[10].Replace(".", ",")) : 0;
                    item.ExigibilidadeISS = (Dominio.Enumeradores.ExigibilidadeISS)int.Parse(linhaSeparada[11]);
                    item.ProcessoJudicial = linhaSeparada[15];
                    item.IncentivoFiscal = linhaSeparada[1] == "1";
                }
                else if (linhaSeparada[0] == "UA")//UA|impostoDevol|pDevol|IPI|vIPIDevol|
                {

                }
                else if (linhaSeparada[0] == "W02")//W02|vBC|vICMS|vICMSDeson|vBCST|vST|vProd|vFrete|vSeg|vDesc|vII|vIPI|vPIS|vCOFINS|vOutro|vNF|vTotTrib| 
                {
                    if (linhaSeparada.Count() >= 18)
                    {
                        nfe.BCICMS = !string.IsNullOrWhiteSpace(linhaSeparada[1]) ? decimal.Parse(linhaSeparada[1].Replace(".", ",")) : 0;
                        nfe.ValorICMS = !string.IsNullOrWhiteSpace(linhaSeparada[2]) ? decimal.Parse(linhaSeparada[2].Replace(".", ",")) : 0;
                        nfe.BCICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[4]) ? decimal.Parse(linhaSeparada[4].Replace(".", ",")) : 0;
                        nfe.ValorICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[5]) ? decimal.Parse(linhaSeparada[5].Replace(".", ",")) : 0;
                        nfe.ValorProdutos = !string.IsNullOrWhiteSpace(linhaSeparada[6]) ? decimal.Parse(linhaSeparada[6].Replace(".", ",")) : 0;
                        nfe.ValorFrete = !string.IsNullOrWhiteSpace(linhaSeparada[7]) ? decimal.Parse(linhaSeparada[7].Replace(".", ",")) : 0;
                        nfe.ValorSeguro = !string.IsNullOrWhiteSpace(linhaSeparada[8]) ? decimal.Parse(linhaSeparada[8].Replace(".", ",")) : 0;
                        nfe.ValorDesconto = !string.IsNullOrWhiteSpace(linhaSeparada[9]) ? decimal.Parse(linhaSeparada[9].Replace(".", ",")) : 0;
                        nfe.ValorII = !string.IsNullOrWhiteSpace(linhaSeparada[10]) ? decimal.Parse(linhaSeparada[10].Replace(".", ",")) : 0;
                        nfe.ValorIPI = !string.IsNullOrWhiteSpace(linhaSeparada[11]) ? decimal.Parse(linhaSeparada[11].Replace(".", ",")) : 0;
                        nfe.ValorPIS = !string.IsNullOrWhiteSpace(linhaSeparada[12]) ? decimal.Parse(linhaSeparada[12].Replace(".", ",")) : 0;
                        nfe.ValorCOFINS = !string.IsNullOrWhiteSpace(linhaSeparada[13]) ? decimal.Parse(linhaSeparada[13].Replace(".", ",")) : 0;
                        nfe.ValorOutrasDespesas = !string.IsNullOrWhiteSpace(linhaSeparada[14]) ? decimal.Parse(linhaSeparada[14].Replace(".", ",")) : 0;
                        nfe.ValorTotalNota = !string.IsNullOrWhiteSpace(linhaSeparada[15]) ? decimal.Parse(linhaSeparada[15].Replace(".", ",")) : 0;
                    }
                    else
                    {
                        nfe.BCICMS = !string.IsNullOrWhiteSpace(linhaSeparada[1]) ? decimal.Parse(linhaSeparada[1].Replace(".", ",")) : 0;
                        nfe.ValorICMS = !string.IsNullOrWhiteSpace(linhaSeparada[2]) ? decimal.Parse(linhaSeparada[2].Replace(".", ",")) : 0;
                        nfe.BCICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[3]) ? decimal.Parse(linhaSeparada[3].Replace(".", ",")) : 0;
                        nfe.ValorICMSST = !string.IsNullOrWhiteSpace(linhaSeparada[4]) ? decimal.Parse(linhaSeparada[4].Replace(".", ",")) : 0;
                        nfe.ValorProdutos = !string.IsNullOrWhiteSpace(linhaSeparada[5]) ? decimal.Parse(linhaSeparada[5].Replace(".", ",")) : 0;
                        nfe.ValorFrete = !string.IsNullOrWhiteSpace(linhaSeparada[6]) ? decimal.Parse(linhaSeparada[6].Replace(".", ",")) : 0;
                        nfe.ValorSeguro = !string.IsNullOrWhiteSpace(linhaSeparada[7]) ? decimal.Parse(linhaSeparada[7].Replace(".", ",")) : 0;
                        nfe.ValorDesconto = !string.IsNullOrWhiteSpace(linhaSeparada[8]) ? decimal.Parse(linhaSeparada[8].Replace(".", ",")) : 0;
                        nfe.ValorII = !string.IsNullOrWhiteSpace(linhaSeparada[9]) ? decimal.Parse(linhaSeparada[9].Replace(".", ",")) : 0;
                        nfe.ValorIPI = !string.IsNullOrWhiteSpace(linhaSeparada[10]) ? decimal.Parse(linhaSeparada[10].Replace(".", ",")) : 0;
                        nfe.ValorPIS = !string.IsNullOrWhiteSpace(linhaSeparada[11]) ? decimal.Parse(linhaSeparada[11].Replace(".", ",")) : 0;
                        nfe.ValorCOFINS = !string.IsNullOrWhiteSpace(linhaSeparada[12]) ? decimal.Parse(linhaSeparada[12].Replace(".", ",")) : 0;
                        nfe.ValorOutrasDespesas = !string.IsNullOrWhiteSpace(linhaSeparada[13]) ? decimal.Parse(linhaSeparada[13].Replace(".", ",")) : 0;
                        nfe.ValorTotalNota = !string.IsNullOrWhiteSpace(linhaSeparada[14]) ? decimal.Parse(linhaSeparada[14].Replace(".", ",")) : 0;
                    }
                }
                else if (linhaSeparada[0] == "W17")//W17|vServ|vBC|vISS|vPIS|vCOFINS|dCompet|vDeducao|vOutro|vDescIncond|vDescCond|vISSRet|cRegTrib|
                {
                    nfe.ValorServicos = !string.IsNullOrWhiteSpace(linhaSeparada[1]) ? decimal.Parse(linhaSeparada[1].Replace(".", ",")) : 0;
                    nfe.BCISSQN = !string.IsNullOrWhiteSpace(linhaSeparada[2]) ? decimal.Parse(linhaSeparada[2].Replace(".", ",")) : 0;
                    nfe.ValorISSQN = !string.IsNullOrWhiteSpace(linhaSeparada[3]) ? decimal.Parse(linhaSeparada[3].Replace(".", ",")) : 0;

                    DateTime dataCompetencia = new DateTime();
                    DateTime.TryParseExact(linhaSeparada[6].Substring(0, 16).Replace("T", " ").Replace("-", "/"), "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out dataCompetencia);
                    nfe.DataPrestacaoServico = dataCompetencia;

                    nfe.BCDeducao = !string.IsNullOrWhiteSpace(linhaSeparada[7]) ? decimal.Parse(linhaSeparada[7].Replace(".", ",")) : 0;
                    nfe.ValorOutrasRetencoes = !string.IsNullOrWhiteSpace(linhaSeparada[8]) ? decimal.Parse(linhaSeparada[8].Replace(".", ",")) : 0;
                    nfe.ValorDescontoIncondicional = !string.IsNullOrWhiteSpace(linhaSeparada[9]) ? decimal.Parse(linhaSeparada[9].Replace(".", ",")) : 0;
                    nfe.ValorDescontoCondicional = !string.IsNullOrWhiteSpace(linhaSeparada[10]) ? decimal.Parse(linhaSeparada[10].Replace(".", ",")) : 0;
                    nfe.ValorRetencaoISS = !string.IsNullOrWhiteSpace(linhaSeparada[11]) ? decimal.Parse(linhaSeparada[11].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "W23")//W23|VRetPIS|VRetCOFINS|VRetCSLL|VBCIRRF|VIRRF|VBCRetPrev|VRetPrev| 
                {

                }
                else if (linhaSeparada[0] == "X")//X|ModFrete|
                {
                    nfe.TipoFrete = (Dominio.Enumeradores.ModalidadeFrete)int.Parse(linhaSeparada[1]);
                }
                else if (linhaSeparada[0] == "X03")//X03|XNome|IE|XEnder|UF|XMun|
                {
                    nfe.TranspNome = linhaSeparada[1];
                    nfe.TranspIE = linhaSeparada[2];
                    nfe.TranspEndereco = linhaSeparada[3];
                    nfe.TranspUF = linhaSeparada[4];
                    //nfe.TranspMunicipio
                }
                else if (linhaSeparada[0] == "X04")//X04 | CNPJ |
                {
                    //transportadora
                    nfe.TranspCNPJCPF = linhaSeparada[1];
                }
                else if (linhaSeparada[0] == "X05")//X05|CPF|
                {
                    nfe.TranspCNPJCPF = linhaSeparada[1];
                    //transportadora
                }
                else if (linhaSeparada[0] == "X11")//X11|VServ|VBCRet|PICMSRet|VICMSRet|CFOP|CMunFG|
                {

                }
                else if (linhaSeparada[0] == "X18")//X18|Placa|UF|RNTC|
                {
                    nfe.TranspPlacaVeiculo = linhaSeparada[1];
                    nfe.TranspUFVeiculo = linhaSeparada[2];
                    nfe.TranspANTTVeiculo = linhaSeparada[3];
                }
                else if (linhaSeparada[0] == "X22")//X22|Placa|UF|RNTC|
                {

                }
                else if (linhaSeparada[0] == "X26")//X26|QVol|Esp|Marca|NVol|PesoL|PesoB|
                {
                    nfe.TranspQuantidade = linhaSeparada[1];
                    nfe.TranspEspecie = linhaSeparada[2];
                    nfe.TranspMarca = linhaSeparada[3];
                    nfe.TranspVolume = linhaSeparada[4];
                    nfe.TranspPesoLiquido += !string.IsNullOrWhiteSpace(linhaSeparada[5]) ? decimal.Parse(linhaSeparada[5].Replace(".", ",")) : 0;
                    nfe.TranspPesoBruto += !string.IsNullOrWhiteSpace(linhaSeparada[6]) ? decimal.Parse(linhaSeparada[6].Replace(".", ",")) : 0;
                }
                else if (linhaSeparada[0] == "X33")//X33|NLacre| 
                {

                }
                else if (linhaSeparada[0] == "Y02")//Y02|NFat|VOrig|VDesc|VLiq| 
                {
                    nfe.ParcelasNFe = new List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalParcela>();
                }
                else if (linhaSeparada[0] == "Y07")//Y07|NDup|DVenc|VDup| 
                {
                    nfe.ParcelasNFe.Add(new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalParcela
                    {
                        Acrescimo = 0,
                        Codigo = 0,
                        DataEmissao = nfe.DataEmissao,
                        DataVencimento = DateTime.Parse(linhaSeparada[2]),
                        Desconto = 0,
                        Sequencia = int.Parse(linhaSeparada[1]),
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela.EmAberto,
                        Valor = !string.IsNullOrWhiteSpace(linhaSeparada[3]) ? decimal.Parse(linhaSeparada[3].Replace(".", ",")) : 0
                    });
                }
                else if (linhaSeparada[0] == "Z")//Z|InfAdFisco|InfCpl|
                {
                    nfe.ObservacaoTributaria = linhaSeparada[1];
                    nfe.ObservacaoNota = linhaSeparada[2];

                    if (item != null)
                    {
                        nfe.ItensNFe.Add(item);
                        item = null;
                    }

                    retornoNFe.Add(nfe);
                }
                else if (linhaSeparada[0] == "Z04")//Z04|XCampo|XTexto| 
                {

                }
                else if (linhaSeparada[0] == "Z07")//Z07|XCampo|XTexto|
                {

                }
                else if (linhaSeparada[0] == "Z10")//Z10|NProc|IndProc|  
                {

                }
                else if (linhaSeparada[0] == "ZA")//ZA|UFEmbarq|XLocEmbarq|
                {

                }
                else if (linhaSeparada[0] == "ZB")//ZB|XNEmp|XPed|XCont|
                {

                }
                else if (linhaSeparada[0] == "ZC01")//ZC01|safra|ref|qTotMes|qTotAnt|qTotGer|vFor|vTotDed|vLiqFor| 
                {

                }
                else if (linhaSeparada[0] == "ZC04")//ZC04|dia|qtde|
                {

                }
                else if (linhaSeparada[0] == "ZC10")//ZC10|xDed|vDed|
                {

                }
                linha = linha + 1;
            }

            return retornoNFe;

        }

        public Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica ConverterProdutosXMLParaNFe(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> mercadorias, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Empresa empresa)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Embarcador.WMS.RecebimentoMercadoria repRecebimentoMercadoria = new Repositorio.Embarcador.WMS.RecebimentoMercadoria(unitOfWork);

            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Localidades.Localidade(unitOfWork);
            Servicos.Embarcador.NotaFiscal.NotaFiscalParcela serNotaFiscalParcela = new NotaFiscalParcela(unitOfWork);
            Servicos.Embarcador.NotaFiscal.NotaFiscalProduto serNotaFiscalProduto = new NotaFiscalProduto(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto primeiroRecebimento = mercadorias.FirstOrDefault();

            string notasVenda = string.Join(", ", mercadorias.Select(p => p.XMLNotaFiscal.Numero).Distinct().ToList());
            string notasEntrada = "";
            List<int> codigosRecebimentoMercadoria = new List<int>();
            decimal valorTotalMercadoria = 0;

            List<int> codigosProdutos = mercadorias.Where(p => p.ProdutoInterno != null).Select(p => p.ProdutoInterno.Codigo).Distinct().ToList();
            foreach (var codigoProduto in codigosProdutos)
            {
                decimal qtdProduto = mercadorias.Where(p => p.ProdutoInterno != null && p.ProdutoInterno.Codigo == codigoProduto).Select(p => p.Quantidade).Sum();
                decimal quantidadeProduto = qtdProduto;
                decimal valorUnitario = 0;
                while (quantidadeProduto > 0)
                {
                    Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria recebimentoMercadoria = repRecebimentoMercadoria.BuscarRecebimentoComSaldo(codigoProduto);
                    if (recebimentoMercadoria != null)
                    {
                        string numeroNota = !string.IsNullOrEmpty(recebimentoMercadoria.ChaveNFe) && recebimentoMercadoria.ChaveNFe.Length == 44 ? recebimentoMercadoria.ChaveNFe.Substring(25, 9) : recebimentoMercadoria.ChaveNFe;
                        if (recebimentoMercadoria.QuantidadeLote < quantidadeProduto)
                        {
                            codigosRecebimentoMercadoria.Add(recebimentoMercadoria.Codigo);
                            if (!notasEntrada.Contains(numeroNota))
                                notasEntrada += numeroNota + " ";

                            recebimentoMercadoria.QuantidadeLote = 0;
                            repRecebimentoMercadoria.Atualizar(recebimentoMercadoria);
                            if (valorUnitario > 0)
                                valorUnitario = (valorUnitario + recebimentoMercadoria.ValorUnitario) / 2;
                            else
                                valorUnitario = recebimentoMercadoria.ValorUnitario;

                            quantidadeProduto = quantidadeProduto - recebimentoMercadoria.QuantidadeLote;
                        }
                        else if (recebimentoMercadoria.QuantidadeLote >= quantidadeProduto)
                        {
                            codigosRecebimentoMercadoria.Add(recebimentoMercadoria.Codigo);
                            if (!notasEntrada.Contains(numeroNota))
                                notasEntrada += numeroNota + " ";

                            recebimentoMercadoria.QuantidadeLote = recebimentoMercadoria.QuantidadeLote - quantidadeProduto;
                            repRecebimentoMercadoria.Atualizar(recebimentoMercadoria);

                            if (valorUnitario > 0)
                                valorUnitario = (valorUnitario + recebimentoMercadoria.ValorUnitario) / 2;
                            else
                                valorUnitario = recebimentoMercadoria.ValorUnitario;

                            quantidadeProduto = 0;
                        }
                    }
                    else
                        quantidadeProduto = 0;
                }
                if (valorUnitario > 0)
                    valorTotalMercadoria += valorUnitario * qtdProduto;

            }
            notasEntrada = notasEntrada.Trim();

            Dominio.Entidades.NaturezaDaOperacao naturezaOperacao = null;
            Dominio.Entidades.CFOP cfop = null;

            if (primeiroRecebimento.XMLNotaFiscal.Emitente.Localidade.Estado.Sigla != carga.Empresa.Localidade.Estado.Sigla)
                naturezaOperacao = repNaturezaDaOperacao.BuscarTodosPorTipoEstado(true, false);
            else
                naturezaOperacao = repNaturezaDaOperacao.BuscarTodosPorTipoEstado(true, true);

            if (naturezaOperacao == null || naturezaOperacao.CFOPs == null || naturezaOperacao.CFOPs.Count <= 0)
                return null;

            cfop = naturezaOperacao.CFOPs.FirstOrDefault();

            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica nfe = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica();
            nfe.Codigo = 0;
            nfe.Numero = 0;
            nfe.Serie = repEmpresaSerie.BuscarPorEmpresaTipo(carga.Empresa.Codigo, Dominio.Enumeradores.TipoSerie.NFe)?.Numero ?? 0;
            nfe.CodigoEmpresa = carga.Empresa.Codigo;
            nfe.TipoEmissao = Dominio.Enumeradores.TipoEmissaoNFe.Saida;
            nfe.DataEmissao = DateTime.Now;

            nfe.DataSaida = DateTime.Now;
            nfe.Chave = "";
            nfe.Status = Dominio.Enumeradores.StatusNFe.Emitido;
            nfe.Protocolo = "";
            nfe.DataProcessamento = DateTime.Now;
            nfe.DataPrestacaoServico = DateTime.Now;
            nfe.Finalidade = Dominio.Enumeradores.FinalidadeNFe.Normal;
            nfe.IndicadorPresenca = Dominio.Enumeradores.IndicadorPresencaNFe.Outros;
            nfe.IndicadorIntermediador = Dominio.Enumeradores.IndicadorIntermediadorNFe.SemIntermediador;
            nfe.Cliente = ConverterObjetoPessoa(repCliente.BuscarPorCPFCNPJ(primeiroRecebimento.XMLNotaFiscal.Emitente.CPF_CNPJ));
            nfe.NaturezaDaOperacao = ConverterObjetoNatureza(repNaturezaDaOperacao.BuscarPorId(naturezaOperacao.Codigo));
            nfe.Atividade = ConverterObjetoAtividade(repAtividade.BuscarPorCodigo(primeiroRecebimento.XMLNotaFiscal.Emitente.Atividade.Codigo));
            nfe.LocalidadePrestacaoServico = serLocalidade.ConverterObjetoLocalidade(repLocalidade.BuscarPorCodigo(primeiroRecebimento.XMLNotaFiscal.Emitente.Localidade.Codigo));

            decimal baseICMS = 0, valorICMS = 0, valorICMSDesonerado = 0, valorII = 0, baseICMSST = 0, valorTotalProdutos = (valorTotalMercadoria > 0 ? valorTotalMercadoria : mercadorias.Sum(o => o.ValorProduto * o.Quantidade)), valorFrete = 0, valorSeguro = 0, valorDesconto = 0, valorOutrasDespesas = 0, valorIPI = 0,
                valorTotalNFe = (valorTotalMercadoria > 0 ? valorTotalMercadoria : mercadorias.Sum(o => o.ValorProduto * o.Quantidade)), valorTotalServicos = 0, baseISS = 0, valorISS = 0, baseDeducao = 0, valorOutrasRetencoes = 0, valorDescontoIncondicional = 0, valorDescontoCondicional = 0, valorRetencaoISS = 0,
                basePIS = 0, valorPIS = 0, baseCOFINS = 0, valorCOFINS = 0, valorFCP = 0, valorICMSDestino = 0, valorICMSRemetente = 0, valorICMSST = 0, valorFCPICMS = 0, valorFCPICMSST = 0, ValorIPIDevolvido = 0, baseICMSSTRetido = 0, valorICMSSTRetido = 0;

            nfe.BCICMS = baseICMS;
            nfe.ValorICMS = valorICMS;
            nfe.ICMSDesonerado = valorICMSDesonerado;
            nfe.ValorII = valorII;
            nfe.BCICMSST = baseICMSST;
            nfe.ValorICMSST = valorICMSST;
            nfe.ValorProdutos = valorTotalProdutos;
            nfe.ValorFrete = valorFrete;
            nfe.ValorSeguro = valorSeguro;
            nfe.ValorDesconto = valorDesconto;
            nfe.ValorOutrasDespesas = valorOutrasDespesas;
            nfe.ValorIPI = valorIPI;
            nfe.ValorTotalNota = valorTotalNFe;
            nfe.ValorServicos = valorTotalServicos;
            nfe.BCISSQN = baseISS;
            nfe.ValorISSQN = valorISS;
            nfe.BCDeducao = baseDeducao;
            nfe.ValorOutrasRetencoes = valorOutrasRetencoes;
            nfe.ValorDescontoIncondicional = valorDescontoIncondicional;
            nfe.ValorDescontoCondicional = valorDescontoCondicional;
            nfe.ValorRetencaoISS = valorRetencaoISS;
            nfe.BCPIS = basePIS;
            nfe.ValorPIS = valorPIS;
            nfe.BCCOFINS = baseCOFINS;
            nfe.ValorCOFINS = valorCOFINS;
            nfe.ValorFCP = valorFCP;
            nfe.ValorICMSDestino = valorICMSDestino;
            nfe.ValorICMSRemetente = valorICMSRemetente;
            nfe.ValorFCPICMS = valorFCPICMS;
            nfe.ValorFCPICMSST = valorFCPICMSST;
            nfe.ValorIPIDevolvido = ValorIPIDevolvido;
            nfe.BCICMSSTRetido = baseICMSSTRetido;
            nfe.ValorICMSSTRetido = valorICMSSTRetido;

            nfe.ObservacaoNota = "NFs Ent: " + notasEntrada + " NFs Venda:" + notasVenda;
            nfe.ObservacaoTributaria = "";

            nfe.UFEmbarque = "";
            nfe.LocalEmbarque = "";
            nfe.LocalDespacho = "";
            nfe.InformacaoCompraNotaEmpenho = "";
            nfe.InformacaoCompraPedido = "";
            nfe.InformacaoCompraContrato = "";

            double cnpjEmpresa = 0;
            double.TryParse(carga.Empresa.CNPJ, out cnpjEmpresa);
            if (cnpjEmpresa > 0)
            {
                nfe.Transportadora = ConverterObjetoPessoa(repCliente.BuscarPorCPFCNPJ(cnpjEmpresa));
                nfe.TranspCNPJCPF = carga.Empresa.CNPJ_SemFormato;
                nfe.TranspNome = carga.Empresa.RazaoSocial;
                nfe.TranspIE = carga.Empresa.InscricaoEstadual;
                nfe.TranspEndereco = carga.Empresa.Endereco;
                nfe.TranspUF = carga.Empresa.Localidade.Estado.Sigla;
                nfe.TranspMunicipio = serLocalidade.ConverterObjetoLocalidade(repLocalidade.BuscarPorCodigo(carga.Empresa.Localidade.Codigo));
                nfe.TranspEmail = carga.Empresa.Email;
                nfe.Veiculo = carga.Veiculo != null ? ConverterObjetoVeiculo(repVeiculo.BuscarPorCodigo(carga.Veiculo?.Codigo ?? 0)) : null;
                nfe.TranspPlacaVeiculo = nfe.Veiculo != null ? nfe.Veiculo.Placa : "";
                nfe.TranspUFVeiculo = nfe.TranspMunicipio != null ? nfe.TranspMunicipio.SiglaUF : "";
                nfe.TranspANTTVeiculo = nfe.Transportadora != null ? nfe.Transportadora.RNTRC : "";
                nfe.TranspQuantidade = mercadorias != null && mercadorias.Count > 0 ? Utilidades.String.OnlyNumbers(mercadorias.Count().ToString("n0")) : "";
                nfe.TranspEspecie = "";
                nfe.TranspMarca = "";
                nfe.TranspVolume = mercadorias != null && mercadorias.Count > 0 ? Utilidades.String.OnlyNumbers(mercadorias.Count().ToString("n0")) : "";
                decimal pesoBruto = mercadorias.Sum(o => o.XMLNotaFiscal.Peso), pesoLiquido = mercadorias.Sum(o => o.XMLNotaFiscal.Peso);
                nfe.TranspPesoBruto = pesoBruto;
                nfe.TranspPesoLiquido = pesoLiquido;
                nfe.TipoFrete = Dominio.Enumeradores.ModalidadeFrete.SemFrete;
            }

            serNotaFiscalProduto.SetarProdutoXMLParaProdutos(cfop, carga, mercadorias, ref nfe, unitOfWork, codigosRecebimentoMercadoria);

            return nfe;

        }

        public Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica ConverterMercadoriasParaNFe(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria> mercadorias, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Empresa empresa)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);

            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Localidades.Localidade(unitOfWork);
            Servicos.Embarcador.NotaFiscal.NotaFiscalParcela serNotaFiscalParcela = new NotaFiscalParcela(unitOfWork);
            Servicos.Embarcador.NotaFiscal.NotaFiscalProduto serNotaFiscalProduto = new NotaFiscalProduto(unitOfWork);

            Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria primeiroRecebimento = mercadorias.FirstOrDefault();

            Dominio.Entidades.NaturezaDaOperacao naturezaOperacao = null;
            Dominio.Entidades.CFOP cfop = null;

            if (primeiroRecebimento.Remetente.Localidade.Estado.Sigla != carga.Empresa.Localidade.Estado.Sigla)
                naturezaOperacao = repNaturezaDaOperacao.BuscarTodosPorTipoEstado(true, false);
            else
                naturezaOperacao = repNaturezaDaOperacao.BuscarTodosPorTipoEstado(true, true);

            if (naturezaOperacao == null || naturezaOperacao.CFOPs == null || naturezaOperacao.CFOPs.Count <= 0)
                return null;

            cfop = naturezaOperacao.CFOPs.FirstOrDefault();

            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica nfe = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica();
            nfe.Codigo = 0;
            nfe.Numero = 0;
            nfe.Serie = repEmpresaSerie.BuscarPorEmpresaTipo(carga.Empresa.Codigo, Dominio.Enumeradores.TipoSerie.NFe)?.Numero ?? 0;
            nfe.CodigoEmpresa = carga.Empresa.Codigo;
            nfe.TipoEmissao = Dominio.Enumeradores.TipoEmissaoNFe.Saida;
            nfe.DataEmissao = DateTime.Now;

            nfe.DataSaida = DateTime.Now;
            nfe.Chave = "";
            nfe.Status = Dominio.Enumeradores.StatusNFe.Emitido;
            nfe.Protocolo = "";
            nfe.DataProcessamento = DateTime.Now;
            nfe.DataPrestacaoServico = DateTime.Now;
            nfe.Finalidade = Dominio.Enumeradores.FinalidadeNFe.Devolucao;
            nfe.IndicadorPresenca = Dominio.Enumeradores.IndicadorPresencaNFe.Outros;
            nfe.Cliente = ConverterObjetoPessoa(repCliente.BuscarPorCPFCNPJ(primeiroRecebimento.Remetente.CPF_CNPJ));
            nfe.NaturezaDaOperacao = ConverterObjetoNatureza(repNaturezaDaOperacao.BuscarPorId(naturezaOperacao.Codigo));
            nfe.Atividade = ConverterObjetoAtividade(repAtividade.BuscarPorCodigo(primeiroRecebimento.Remetente.Atividade.Codigo));
            nfe.LocalidadePrestacaoServico = serLocalidade.ConverterObjetoLocalidade(repLocalidade.BuscarPorCodigo(primeiroRecebimento.Remetente.Localidade.Codigo));

            decimal baseICMS = 0, valorICMS = 0, valorICMSDesonerado = 0, valorII = 0, baseICMSST = 0, valorTotalProdutos = mercadorias.Sum(o => o.ValorUnitario * o.QuantidadeLote), valorFrete = 0, valorSeguro = 0, valorDesconto = 0, valorOutrasDespesas = 0, valorIPI = 0,
                valorTotalNFe = mercadorias.Sum(o => o.ValorUnitario * o.QuantidadeLote), valorTotalServicos = 0, baseISS = 0, valorISS = 0, baseDeducao = 0, valorOutrasRetencoes = 0, valorDescontoIncondicional = 0, valorDescontoCondicional = 0, valorRetencaoISS = 0,
                basePIS = 0, valorPIS = 0, baseCOFINS = 0, valorCOFINS = 0, valorFCP = 0, valorICMSDestino = 0, valorICMSRemetente = 0, valorICMSST = 0, valorFCPICMS = 0, valorFCPICMSST = 0, ValorIPIDevolvido = 0, baseICMSSTRetido = 0, valorICMSSTRetido = 0;

            nfe.BCICMS = baseICMS;
            nfe.ValorICMS = valorICMS;
            nfe.ICMSDesonerado = valorICMSDesonerado;
            nfe.ValorII = valorII;
            nfe.BCICMSST = baseICMSST;
            nfe.ValorICMSST = valorICMSST;
            nfe.ValorProdutos = valorTotalProdutos;
            nfe.ValorFrete = valorFrete;
            nfe.ValorSeguro = valorSeguro;
            nfe.ValorDesconto = valorDesconto;
            nfe.ValorOutrasDespesas = valorOutrasDespesas;
            nfe.ValorIPI = valorIPI;
            nfe.ValorTotalNota = valorTotalNFe;
            nfe.ValorServicos = valorTotalServicos;
            nfe.BCISSQN = baseISS;
            nfe.ValorISSQN = valorISS;
            nfe.BCDeducao = baseDeducao;
            nfe.ValorOutrasRetencoes = valorOutrasRetencoes;
            nfe.ValorDescontoIncondicional = valorDescontoIncondicional;
            nfe.ValorDescontoCondicional = valorDescontoCondicional;
            nfe.ValorRetencaoISS = valorRetencaoISS;
            nfe.BCPIS = basePIS;
            nfe.ValorPIS = valorPIS;
            nfe.BCCOFINS = baseCOFINS;
            nfe.ValorCOFINS = valorCOFINS;
            nfe.ValorFCP = valorFCP;
            nfe.ValorICMSDestino = valorICMSDestino;
            nfe.ValorICMSRemetente = valorICMSRemetente;
            nfe.ValorFCPICMS = valorFCPICMS;
            nfe.ValorFCPICMSST = valorFCPICMSST;
            nfe.ValorIPIDevolvido = ValorIPIDevolvido;
            nfe.BCICMSSTRetido = baseICMSSTRetido;
            nfe.ValorICMSSTRetido = valorICMSSTRetido;

            nfe.ObservacaoNota = "";
            nfe.ObservacaoTributaria = "";

            nfe.UFEmbarque = "";
            nfe.LocalEmbarque = "";
            nfe.LocalDespacho = "";
            nfe.InformacaoCompraNotaEmpenho = "";
            nfe.InformacaoCompraPedido = "";
            nfe.InformacaoCompraContrato = "";


            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia referencia = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia();
            nfe.ReferenciaNFe = new List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia>();
            referencia.TipoDocumento = Dominio.Enumeradores.TipoDocumentoReferenciaNFe.NF;
            referencia.Chave = primeiroRecebimento.ChaveNFe;
            nfe.ReferenciaNFe.Add(referencia);

            serNotaFiscalProduto.SetarRecebimentoMercadoriaParaProdutos(cfop, carga, mercadorias, ref nfe, unitOfWork);

            return nfe;

        }

        public Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica ConverterDynamicParaNFe(dynamic dynNFe, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Empresa empresa)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Localidades.Localidade(unitOfWork);
            Servicos.Embarcador.NotaFiscal.NotaFiscalParcela serNotaFiscalParcela = new NotaFiscalParcela(unitOfWork);
            Servicos.Embarcador.NotaFiscal.NotaFiscalProduto serNotaFiscalProduto = new NotaFiscalProduto(unitOfWork);


            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica nfe = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica();
            nfe.Codigo = dynNFe.NFe.Codigo != null ? (int)dynNFe.NFe.Codigo : 0;
            nfe.Numero = dynNFe.NFe.Numero != null ? (int)dynNFe.NFe.Numero : 0;
            nfe.Serie = dynNFe.NFe.Serie != null ? (int)dynNFe.NFe.Serie : 0;
            nfe.CodigoEmpresa = dynNFe.NFe.Empresa != null ? (int)dynNFe.NFe.Empresa : 0;
            nfe.TipoEmissao = (Dominio.Enumeradores.TipoEmissaoNFe)dynNFe.NFe.TipoEmissao;
            DateTime dataEmissao = new DateTime();
            DateTime.TryParseExact((string)dynNFe.NFe.DataEmissao, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
            nfe.DataEmissao = dataEmissao;
            DateTime dataSaida = new DateTime();
            DateTime.TryParseExact((string)dynNFe.NFe.DataSaida, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataSaida);
            if (dataSaida != null && dataSaida > DateTime.MinValue)
                nfe.DataSaida = dataSaida;
            else
                nfe.DataSaida = null;
            nfe.Chave = (string)dynNFe.NFe.Chave;
            nfe.Status = (Dominio.Enumeradores.StatusNFe)dynNFe.NFe.Status;
            nfe.Protocolo = (string)dynNFe.NFe.Protocolo;
            DateTime dataProcessamento = new DateTime();
            DateTime.TryParseExact((string)dynNFe.NFe.DataProcessamento, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataProcessamento);
            nfe.DataProcessamento = dataProcessamento;
            DateTime dataPrestacaoServico = new DateTime();
            DateTime.TryParseExact((string)dynNFe.NFe.DataPrestacaoServico, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataPrestacaoServico);
            nfe.DataPrestacaoServico = dataPrestacaoServico;
            nfe.Finalidade = (Dominio.Enumeradores.FinalidadeNFe)dynNFe.NFe.Finalidade;
            nfe.IndicadorPresenca = (Dominio.Enumeradores.IndicadorPresencaNFe)dynNFe.NFe.IndicadorPresenca;
            if (!string.IsNullOrWhiteSpace((string)dynNFe.NFe.IndicadorIntermediador))
                nfe.IndicadorIntermediador = (Dominio.Enumeradores.IndicadorIntermediadorNFe)dynNFe.NFe.IndicadorIntermediador;
            nfe.Cliente = ConverterObjetoPessoa(repCliente.BuscarPorCPFCNPJ((double)dynNFe.NFe.Pessoa));
            nfe.NaturezaDaOperacao = ConverterObjetoNatureza(repNaturezaDaOperacao.BuscarPorId((int)dynNFe.NFe.NaturezaOperacao));
            nfe.Atividade = ConverterObjetoAtividade(repAtividade.BuscarPorCodigo((int)dynNFe.NFe.Atividade));
            nfe.LocalidadePrestacaoServico = serLocalidade.ConverterObjetoLocalidade(repLocalidade.BuscarPorCodigo((int)dynNFe.NFe.CidadePrestacao));
            nfe.Intermediador = ((string)dynNFe.NFe.Intermediador).ToDouble() > 0 ? ConverterObjetoPessoa(repCliente.BuscarPorCPFCNPJ((double)dynNFe.NFe.Intermediador)) : null;

            decimal baseICMS, valorICMS, valorICMSDesonerado, valorII, baseICMSST, valorTotalProdutos, valorFrete, valorSeguro, valorDesconto, valorOutrasDespesas, valorIPI,
                valorTotalNFe, valorTotalServicos, baseISS, valorISS, baseDeducao, valorOutrasRetencoes, valorDescontoIncondicional, valorDescontoCondicional, valorRetencaoISS,
                basePIS, valorPIS, baseCOFINS, valorCOFINS, valorFCP, valorICMSDestino, valorICMSRemetente, valorICMSST, valorFCPICMS, valorFCPICMSST, ValorIPIDevolvido;

            decimal.TryParse((string)dynNFe.Totalizador.BaseICMS, out baseICMS);
            decimal.TryParse((string)dynNFe.Totalizador.ValorICMS, out valorICMS);
            decimal.TryParse((string)dynNFe.Totalizador.ValorICMSDesonerado, out valorICMSDesonerado);
            decimal.TryParse((string)dynNFe.Totalizador.ValorII, out valorII);
            decimal.TryParse((string)dynNFe.Totalizador.BaseICMSST, out baseICMSST);
            decimal.TryParse((string)dynNFe.Totalizador.ValorICMSST, out valorICMSST);
            decimal.TryParse((string)dynNFe.Totalizador.ValorTotalProdutos, out valorTotalProdutos);
            decimal.TryParse((string)dynNFe.Totalizador.ValorFrete, out valorFrete);
            decimal.TryParse((string)dynNFe.Totalizador.ValorSeguro, out valorSeguro);
            decimal.TryParse((string)dynNFe.Totalizador.ValorDesconto, out valorDesconto);
            decimal.TryParse((string)dynNFe.Totalizador.ValorOutrasDespesas, out valorOutrasDespesas);
            decimal.TryParse((string)dynNFe.Totalizador.ValorIPI, out valorIPI);
            decimal.TryParse((string)dynNFe.Totalizador.ValorFCPICMS, out valorFCPICMS);
            decimal.TryParse((string)dynNFe.Totalizador.ValorFCPICMSST, out valorFCPICMSST);
            decimal.TryParse((string)dynNFe.Totalizador.ValorIPIDevolvido, out ValorIPIDevolvido);
            decimal.TryParse((string)dynNFe.Totalizador.BCICMSSTRetido, out decimal baseICMSSTRetido);
            decimal.TryParse((string)dynNFe.Totalizador.ValorICMSSTRetido, out decimal valorICMSSTRetido);

            decimal.TryParse((string)dynNFe.Totalizador.ValorTotalNFe, out valorTotalNFe);
            decimal.TryParse((string)dynNFe.Totalizador.ValorTotalServicos, out valorTotalServicos);
            decimal.TryParse((string)dynNFe.Totalizador.BaseISS, out baseISS);
            decimal.TryParse((string)dynNFe.Totalizador.ValorISS, out valorISS);
            decimal.TryParse((string)dynNFe.Totalizador.BaseDeducao, out baseDeducao);
            decimal.TryParse((string)dynNFe.Totalizador.ValorOutrasRetencoes, out valorOutrasRetencoes);
            decimal.TryParse((string)dynNFe.Totalizador.ValorDescontoIncondicional, out valorDescontoIncondicional);
            decimal.TryParse((string)dynNFe.Totalizador.ValorDescontoCondicional, out valorDescontoCondicional);
            decimal.TryParse((string)dynNFe.Totalizador.ValorRetencaoISS, out valorRetencaoISS);

            decimal.TryParse((string)dynNFe.Totalizador.BasePIS, out basePIS);
            decimal.TryParse((string)dynNFe.Totalizador.ValorPIS, out valorPIS);
            decimal.TryParse((string)dynNFe.Totalizador.BaseCOFINS, out baseCOFINS);
            decimal.TryParse((string)dynNFe.Totalizador.ValorCOFINS, out valorCOFINS);
            decimal.TryParse((string)dynNFe.Totalizador.ValorFCP, out valorFCP);
            decimal.TryParse((string)dynNFe.Totalizador.ValorICMSDestino, out valorICMSDestino);
            decimal.TryParse((string)dynNFe.Totalizador.ValorICMSRemetente, out valorICMSRemetente);

            nfe.BCICMS = baseICMS;
            nfe.ValorICMS = valorICMS;
            nfe.ICMSDesonerado = valorICMSDesonerado;
            nfe.ValorII = valorII;
            nfe.BCICMSST = baseICMSST;
            nfe.ValorICMSST = valorICMSST;
            nfe.ValorProdutos = valorTotalProdutos;
            nfe.ValorFrete = valorFrete;
            nfe.ValorSeguro = valorSeguro;
            nfe.ValorDesconto = valorDesconto;
            nfe.ValorOutrasDespesas = valorOutrasDespesas;
            nfe.ValorIPI = valorIPI;
            nfe.ValorTotalNota = valorTotalNFe;
            nfe.ValorServicos = valorTotalServicos;
            nfe.BCISSQN = baseISS;
            nfe.ValorISSQN = valorISS;
            nfe.BCDeducao = baseDeducao;
            nfe.ValorOutrasRetencoes = valorOutrasRetencoes;
            nfe.ValorDescontoIncondicional = valorDescontoIncondicional;
            nfe.ValorDescontoCondicional = valorDescontoCondicional;
            nfe.ValorRetencaoISS = valorRetencaoISS;
            nfe.BCPIS = basePIS;
            nfe.ValorPIS = valorPIS;
            nfe.BCCOFINS = baseCOFINS;
            nfe.ValorCOFINS = valorCOFINS;
            nfe.ValorFCP = valorFCP;
            nfe.ValorICMSDestino = valorICMSDestino;
            nfe.ValorICMSRemetente = valorICMSRemetente;
            nfe.ValorFCPICMS = valorFCPICMS;
            nfe.ValorFCPICMSST = valorFCPICMSST;
            nfe.ValorIPIDevolvido = ValorIPIDevolvido;
            nfe.BCICMSSTRetido = baseICMSSTRetido;
            nfe.ValorICMSSTRetido = valorICMSSTRetido;

            nfe.ObservacaoNota = dynNFe.Observacao.ObservacaoNFe;
            nfe.ObservacaoTributaria = dynNFe.Observacao.ObservacaoTributaria;

            nfe.UFEmbarque = dynNFe.ExportacaoCompra.UFEmbarque;
            nfe.LocalEmbarque = dynNFe.ExportacaoCompra.LocalEmbarque;
            nfe.LocalDespacho = dynNFe.ExportacaoCompra.LocalDespacho;
            nfe.InformacaoCompraNotaEmpenho = dynNFe.ExportacaoCompra.InformacaoCompraNotaEmpenho;
            nfe.InformacaoCompraPedido = dynNFe.ExportacaoCompra.InformacaoCompraPedido;
            nfe.InformacaoCompraContrato = dynNFe.ExportacaoCompra.InformacaoCompraContrato;

            nfe.Transportadora = ConverterObjetoPessoa(repCliente.BuscarPorCPFCNPJ((double)dynNFe.Transporte.Transportadora));
            nfe.TranspCNPJCPF = dynNFe.Transporte.CNPJTransportadora;
            nfe.TranspNome = dynNFe.Transporte.NomeTransportadora;
            nfe.TranspIE = dynNFe.Transporte.IETransportadora;
            nfe.TranspEndereco = dynNFe.Transporte.EnderecoTransportadora;
            nfe.TranspUF = dynNFe.Transporte.EstadoTransportadora;
            nfe.TranspMunicipio = serLocalidade.ConverterObjetoLocalidade(repLocalidade.BuscarPorCodigo((int)dynNFe.Transporte.CidadeTransportadora));
            nfe.TranspEmail = dynNFe.Transporte.EmailTransportadora;
            nfe.Veiculo = ConverterObjetoVeiculo(repVeiculo.BuscarPorCodigo((int)dynNFe.Transporte.Veiculo));
            nfe.TranspPlacaVeiculo = dynNFe.Transporte.PlacaVeiculo;
            nfe.TranspUFVeiculo = dynNFe.Transporte.EstadoVeiculo;
            nfe.TranspANTTVeiculo = dynNFe.Transporte.ANTTVeiculo;
            nfe.TranspQuantidade = dynNFe.Transporte.Quantidade;
            nfe.TranspEspecie = dynNFe.Transporte.Especie;
            nfe.TranspMarca = dynNFe.Transporte.Marca;
            nfe.TranspVolume = dynNFe.Transporte.Volume;
            decimal pesoBruto, pesoLiquido = 0;
            decimal.TryParse((string)dynNFe.Transporte.PesoBruto, out pesoBruto);
            decimal.TryParse((string)dynNFe.Transporte.PesoLiquido, out pesoLiquido);
            nfe.TranspPesoBruto = pesoBruto;
            nfe.TranspPesoLiquido = pesoLiquido;
            if (dynNFe.Transporte != null && dynNFe.Transporte.TipoFrete != null && !string.IsNullOrWhiteSpace((string)dynNFe.Transporte.TipoFrete))
                nfe.TipoFrete = (Dominio.Enumeradores.ModalidadeFrete)dynNFe.Transporte.TipoFrete;
            else
                nfe.TipoFrete = Dominio.Enumeradores.ModalidadeFrete.SemFrete;

            bool.TryParse((string)dynNFe.RetiradaEntrega.UtilizarEnderecoRetirada, out bool utilizarEnderecoRetirada);
            nfe.UtilizarEnderecoRetirada = utilizarEnderecoRetirada;
            nfe.ClienteRetirada = ConverterObjetoPessoa(repCliente.BuscarPorCPFCNPJ((double)dynNFe.RetiradaEntrega.ClienteRetirada));
            nfe.LocalidadeRetirada = serLocalidade.ConverterObjetoLocalidade(repLocalidade.BuscarPorCodigo((int)dynNFe.RetiradaEntrega.LocalidadeRetirada));
            nfe.RetiradaLogradouro = dynNFe.RetiradaEntrega.RetiradaLogradouro;
            nfe.RetiradaNumeroLogradouro = dynNFe.RetiradaEntrega.RetiradaNumeroLogradouro;
            nfe.RetiradaComplementoLogradouro = dynNFe.RetiradaEntrega.RetiradaComplementoLogradouro;
            nfe.RetiradaBairro = dynNFe.RetiradaEntrega.RetiradaBairro;
            nfe.RetiradaCEP = dynNFe.RetiradaEntrega.RetiradaCEP;
            nfe.RetiradaTelefone = dynNFe.RetiradaEntrega.RetiradaTelefone;
            nfe.RetiradaEmail = dynNFe.RetiradaEntrega.RetiradaEmail;
            nfe.RetiradaIE = dynNFe.RetiradaEntrega.RetiradaIE;

            bool.TryParse((string)dynNFe.RetiradaEntrega.UtilizarEnderecoEntrega, out bool utilizarEnderecoEntrega);
            nfe.UtilizarEnderecoEntrega = utilizarEnderecoEntrega;
            nfe.ClienteEntrega = ConverterObjetoPessoa(repCliente.BuscarPorCPFCNPJ((double)dynNFe.RetiradaEntrega.ClienteEntrega));
            nfe.LocalidadeEntrega = serLocalidade.ConverterObjetoLocalidade(repLocalidade.BuscarPorCodigo((int)dynNFe.RetiradaEntrega.LocalidadeEntrega));
            nfe.EntregaLogradouro = dynNFe.RetiradaEntrega.EntregaLogradouro;
            nfe.EntregaNumeroLogradouro = dynNFe.RetiradaEntrega.EntregaNumeroLogradouro;
            nfe.EntregaComplementoLogradouro = dynNFe.RetiradaEntrega.EntregaComplementoLogradouro;
            nfe.EntregaBairro = dynNFe.RetiradaEntrega.EntregaBairro;
            nfe.EntregaCEP = dynNFe.RetiradaEntrega.EntregaCEP;
            nfe.EntregaTelefone = dynNFe.RetiradaEntrega.EntregaTelefone;
            nfe.EntregaEmail = dynNFe.RetiradaEntrega.EntregaEmail;
            nfe.EntregaIE = dynNFe.RetiradaEntrega.EntregaIE;

            if ((int)dynNFe.Referencia.TipoDocumento > 0)
            {
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia referencia = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia();
                nfe.ReferenciaNFe = new List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia>();
                referencia.TipoDocumento = (Dominio.Enumeradores.TipoDocumentoReferenciaNFe)dynNFe.Referencia.TipoDocumento;
                referencia.Chave = dynNFe.Referencia.Chave;
                referencia.UF = dynNFe.Referencia.Estado;
                DateTime dataEmissaoReferencia = new DateTime();
                DateTime.TryParseExact((string)dynNFe.Referencia.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoReferencia);
                referencia.DataEmissao = dataEmissaoReferencia;
                referencia.CPFEmitente = dynNFe.Referencia.CNPJCPFEmitente;
                referencia.CNPJEmitente = dynNFe.Referencia.CNPJCPFEmitente;
                referencia.IEEmitente = dynNFe.Referencia.IEEmitente;
                referencia.Serie = dynNFe.Referencia.Serie;
                referencia.Numero = dynNFe.Referencia.Numero;
                referencia.NumeroECF = dynNFe.Referencia.NumeroECF;
                referencia.COO = dynNFe.Referencia.NumeroCOO;
                referencia.Modelo = dynNFe.Referencia.Modelo;
                nfe.ReferenciaNFe.Add(referencia);
            }

            serNotaFiscalParcela.SetarDynamicParaParcelas(dynNFe, ref nfe, unitOfWork);
            serNotaFiscalProduto.SetarDynamicParaProdutos(dynNFe, ref nfe, unitOfWork);

            return nfe;

        }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ConverterObjetoPessoa(Dominio.Entidades.Cliente pessoa)
        {
            if (pessoa != null)
            {
                Servicos.Embarcador.Localidades.Localidade serLocalidade = new Embarcador.Localidades.Localidade();
                Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoaIntegracao = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                pessoaIntegracao.CodigoAtividade = pessoa.Atividade.Codigo;
                pessoaIntegracao.CPFCNPJ = Convert.ToString(pessoa.CPF_CNPJ);
                pessoaIntegracao.Email = pessoa.Email;
                pessoaIntegracao.EmailContador = pessoa.EmailContador;
                pessoaIntegracao.EmailContato = pessoa.EmailContato;
                pessoaIntegracao.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                pessoaIntegracao.Endereco.Bairro = pessoa.Bairro;
                pessoaIntegracao.Endereco.CEP = pessoa.CEP;
                pessoaIntegracao.Endereco.Cidade = serLocalidade.ConverterObjetoLocalidade(pessoa.Localidade);
                pessoaIntegracao.Endereco.Complemento = pessoa.Complemento;
                pessoaIntegracao.Endereco.Telefone = pessoa.Telefone1;
                pessoaIntegracao.IM = pessoa.InscricaoMunicipal;
                pessoaIntegracao.NomeFantasia = pessoa.NomeFantasia;
                pessoaIntegracao.RazaoSocial = pessoa.Nome.ToUpper();
                pessoaIntegracao.RGIE = pessoa.RG_Passaporte;
                if (pessoa.Tipo == "F")
                    pessoaIntegracao.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Fisica;
                else
                    pessoaIntegracao.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
                return pessoaIntegracao;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.NotaFiscal.Atividade ConverterObjetoAtividade(Dominio.Entidades.Atividade atividade)
        {
            if (atividade != null)
            {
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.Atividade atividadeIntegracao = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.Atividade();
                atividadeIntegracao.Codigo = atividade.Codigo;
                atividadeIntegracao.Descricao = atividade.Descricao;
                return atividadeIntegracao;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ModeloDocumentoFiscal ConverterObjetoModelo(Dominio.Entidades.ModeloDocumentoFiscal modelo)
        {
            if (modelo != null)
            {
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ModeloDocumentoFiscal modeloIntegracao = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ModeloDocumentoFiscal();
                modeloIntegracao.Codigo = modelo.Codigo;
                modeloIntegracao.Descricao = modelo.Descricao;
                modeloIntegracao.Numero = modelo.Numero;
                return modeloIntegracao;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NaturezaDaOperacao ConverterObjetoNatureza(Dominio.Entidades.NaturezaDaOperacao natureza)
        {
            if (natureza != null)
            {
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NaturezaDaOperacao naturezaIntegracao = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NaturezaDaOperacao();
                naturezaIntegracao.Codigo = natureza.Codigo;
                naturezaIntegracao.Descricao = natureza.Descricao;
                return naturezaIntegracao;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo ConverterObjetoVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            if (veiculo != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo();
                veiculoIntegracao.Codigo = veiculo.Codigo;
                veiculoIntegracao.AnoFabricacao = veiculo.AnoFabricacao;
                veiculoIntegracao.AnoModelo = veiculo.AnoModelo;
                veiculoIntegracao.Ativo = veiculo.Ativo;
                veiculoIntegracao.CapacidadeKG = veiculo.CapacidadeKG;
                veiculoIntegracao.CapacidadeM3 = veiculo.CapacidadeM3;
                veiculoIntegracao.DataAquisicao = Convert.ToString(veiculo.DataCompra);

                veiculoIntegracao.NumeroChassi = veiculo.Chassi;
                veiculoIntegracao.NumeroFrota = veiculo.NumeroFrota;
                veiculoIntegracao.NumeroMotor = veiculo.NumeroMotor;
                veiculoIntegracao.Placa = veiculo.Placa;
                veiculoIntegracao.Renavam = veiculo.Renavam;
                veiculoIntegracao.RNTC = Convert.ToString(veiculo.RNTRC);
                veiculoIntegracao.Tara = veiculo.Tara;

                if (veiculo.Estado != null)
                    veiculoIntegracao.UF = veiculo.Estado.Sigla;

                return veiculoIntegracao;
            }
            else
            {
                return null;
            }
        }

        public System.IO.MemoryStream ObterLoteDeDANFE(List<int> codigosNFes, string caminhoRelatoriosEmbarcador, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unidadeTrabalho);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unidadeTrabalho);
            Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica serNotaFiscalEletronica = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = repRelatorio.BuscarPorTitulo("DANFE");

            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = null;

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unidadeTrabalho);
            Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = new Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio();
            dynRelatorio.Codigo = relatorioOrigem.Codigo;
            dynRelatorio.CodigoControleRelatorios = relatorioOrigem.CodigoControleRelatorios;
            dynRelatorio.Titulo = relatorioOrigem.Titulo;
            dynRelatorio.Descricao = relatorioOrigem.Descricao;
            dynRelatorio.Padrao = relatorioOrigem.Padrao;
            dynRelatorio.ExibirSumarios = relatorioOrigem.ExibirSumarios;
            dynRelatorio.CortarLinhas = relatorioOrigem.CortarLinhas;
            dynRelatorio.FundoListrado = relatorioOrigem.FundoListrado;
            dynRelatorio.TamanhoPadraoFonte = relatorioOrigem.TamanhoPadraoFonte;
            dynRelatorio.FontePadrao = relatorioOrigem.FontePadrao;
            dynRelatorio.AgruparRelatorio = false;
            dynRelatorio.PropriedadeAgrupa = relatorioOrigem.PropriedadeAgrupa;
            dynRelatorio.OrdemAgrupamento = relatorioOrigem.OrdemAgrupamento;
            dynRelatorio.PropriedadeOrdena = relatorioOrigem.PropriedadeOrdena;
            dynRelatorio.OrdemOrdenacao = relatorioOrigem.OrdemOrdenacao;
            dynRelatorio.TipoArquivoRelatorio = Dominio.Enumeradores.TipoArquivoRelatorio.PDF;
            dynRelatorio.OrientacaoRelatorio = relatorioOrigem.OrientacaoRelatorio;
            dynRelatorio.Grid = "{\"draw\":0,\"inicio\":0,\"limite\":0,\"indiceColunaOrdena\":0,\"dirOrdena\":\"desc\",\"recordsTotal\":0,\"recordsFiltered\":0,\"group\":{\"enable\":false,\"propAgrupa\":null,\"dirOrdena\":null},\"header\":[{\"title\":\"Cód. Produto\",\"data\":\"CodigoProduto\",\"width\":\"10%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":0,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Descrição dos Produtos/Serviços\",\"data\":\"DescricaoItem\",\"width\":\"25%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-left\",\"tabletHide\":false,\"phoneHide\":false,\"position\":1,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"NCM\",\"data\":\"CodigoNCMItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":2,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"CST/CSOSN\",\"data\":\"DescricaoCSTItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":3,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"CFOP\",\"data\":\"CodigoCFOPItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":4,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Unid.\",\"data\":\"DescricaoUnidadeItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":5,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Quantidade\",\"data\":\"QuantidadeItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":6,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"V. Unitário\",\"data\":\"ValorUnitarioItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":7,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"V. Total\",\"data\":\"ValorTotalItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":8,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"B.C. ICMS\",\"data\":\"BCICMSItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":9,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Valor ICMS\",\"data\":\"ValorICMSItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":10,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Valor IPI\",\"data\":\"ValorIPIItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":11,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"% ICMS\",\"data\":\"AliquotaICMSItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":12,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"% IPI\",\"data\":\"AliquotaIPIItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":13,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0}],\"data\":null,\"dataSumarizada\":null,\"order\":[{\"column\":0,\"dir\":\"desc\"}]}";
            dynRelatorio.Report = relatorioOrigem.Codigo;
            dynRelatorio.NovoRelatorio = true;

            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorioOrigem, usuario, dynRelatorio.TipoArquivoRelatorio, unidadeTrabalho);
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);

            MemoryStream fZip = new MemoryStream();
            ZipOutputStream zipOStream = new ZipOutputStream(fZip);
            zipOStream.SetLevel(9);

            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal> nfes = repNFe.BuscarPorCodigo(codigosNFes);

            for (int i = 0; i < nfes.Count; i++)
            {
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = nfes[i];

                string nomeArquivo = nfe.Chave;
                serNotaFiscalEletronica.GerarRelatorioDANFE(nfe, agrupamentos, relatorioControleGeracao, relatorioTemp, unidadeTrabalho.StringConexao);

                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatoriosEmbarcador, relatorioControleGeracao.GuidArquivo) + ".pdf";
                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                {
                    byte[] dacte = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);

                    string nomeArquivoDownload = nomeArquivo + ".pdf";

                    if (string.IsNullOrWhiteSpace(nomeArquivoDownload))
                        nomeArquivoDownload = System.IO.Path.GetFileName(caminhoPDF);

                    ZipEntry entry = new ZipEntry(nomeArquivoDownload);
                    entry.DateTime = DateTime.Now;
                    zipOStream.PutNextEntry(entry);
                    zipOStream.Write(dacte, 0, dacte.Length);
                    zipOStream.CloseEntry();
                }
            }

            nfes = null;

            zipOStream.IsStreamOwner = false;
            zipOStream.Close();

            fZip.Position = 0;

            return fZip;
        }

        public void GerarRelatorioDANFE(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

            try
            {
                var result = ReportRequest.WithType(ReportType.GerarRelatorioDANFE)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("notaFiscal", notaFiscal.Codigo)
                    .AddExtraData("propriedades", propriedades.ToJson())
                    .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("relatorioTemp", serRelatorio.ObterConfiguracaoRelatorio(relatorioTemp).ToJson())
                    .CallReport();

                if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                    throw new Dominio.Excecoes.Embarcador.ServicoException(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public void GerarRelatorioCCeNFe(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao carta, int codigoEmpresa, 
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, 
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, 
            string stringConexao, CancellationToken cancellationToken = default)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

            try
            {
                var result = ReportRequest.WithType(ReportType.GerarRelatorioCCeNFe)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("carta", carta.Codigo)
                    .AddExtraData("codigoEmpresa", codigoEmpresa)
                    .AddExtraData("propriedades", propriedades.ToJson())
                    .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("relatorioTemp", serRelatorio.ObterConfiguracaoRelatorio(relatorioTemp).ToJson())
                    .CallReport();

                if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                    throw new Dominio.Excecoes.Embarcador.ServicoException(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                 serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public dynamic BuscarTributacaoItem(Repositorio.UnitOfWork unitOfWork, int codigoProduto, int codigoServico, int codigoCFOP, int codigoAtividade, double codigoCliente, decimal valorTotalItem, decimal valorIPI, decimal valorOutras, decimal valorDesconto, decimal valorFrete, decimal valorSeguro, string siglaEstadoEmpresa, string siglaEstadoUsuario, decimal quantidade, int codigoEmpresa)
        {
            try
            {
                decimal baseICMSDifal = valorTotalItem + valorSeguro + valorOutras + valorIPI + valorFrete - valorDesconto;

                if (codigoProduto == 0 && codigoServico == 0)
                    return "Sem produto e/ou serviço selecionado.";
                else if (valorTotalItem == 0 || codigoCliente == 0 || codigoAtividade == 0)
                    return "Sem valor, cliente ou atividade selecionada.";
                else
                {
                    Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                    Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                    Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
                    Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens repGrupoImpostoItens = new Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens(unitOfWork);

                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(codigoCliente);
                    Dominio.Entidades.CFOP cfop = repCFOP.BuscarPorCodigo(codigoCFOP);
                    Dominio.Entidades.Atividade atividade = repAtividade.BuscarPorCodigo(codigoAtividade);

                    Dominio.Entidades.Embarcador.NotaFiscal.Servico servico = null;
                    Dominio.Entidades.Produto produto = null;
                    Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens imposto = null;
                    if (codigoServico > 0)
                        servico = repServico.BuscarPorCodigo(codigoServico);
                    if (codigoProduto > 0)
                    {
                        produto = repProduto.BuscarPorCodigo(codigoProduto);
                        if (produto.GrupoImposto != null)
                            imposto = repGrupoImpostoItens.BuscarPorEstadosAtividade(cliente.Localidade.Estado.Sigla, siglaEstadoEmpresa, codigoAtividade, produto.GrupoImposto.Codigo);
                    }

                    var cfopProduto = 0;
                    var cfopProdutoNumero = 0;
                    var cfopServico = 0;
                    var cfopServicoNumero = 0;
                    if (servico != null)
                    {
                        if (siglaEstadoUsuario == cliente.Localidade.Estado.Sigla)
                        {
                            if (servico.CFOPVendaDentroEstado != null)
                            {
                                cfopServico = servico.CFOPVendaDentroEstado.Codigo;
                                cfopServicoNumero = servico.CFOPVendaDentroEstado.CodigoCFOP;
                            }
                        }
                        else
                        {
                            if (servico.CFOPVendaForaEstado != null)
                            {
                                cfopServico = servico.CFOPVendaForaEstado.Codigo;
                                cfopServicoNumero = servico.CFOPVendaForaEstado.CodigoCFOP;
                            }
                        }
                    }

                    if (imposto != null)
                    {
                        if (!string.IsNullOrWhiteSpace(imposto.CFOPVenda))
                        {
                            cfopProduto = repCFOP.BuscarPorNumero(Convert.ToInt16(imposto.CFOPVenda), codigoEmpresa)?.Codigo ?? 0;
                            cfopProdutoNumero = int.Parse(imposto.CFOPVenda);

                            if (cfopProduto == 0)
                                return "CFOP não localizada na base.";
                        }
                    }
                    if (cfop != null)
                    {
                        if (cfop.CSTCOFINS != null || cfop.CSTICMS != null || cfop.CSTPIS != null)
                        {
                            cfopProduto = cfop.Codigo;
                            cfopProdutoNumero = cfop.CodigoCFOP;
                        }
                    }

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS? cstICMS = servico != null ? 0 : cfop != null && cfop.CSTICMS != null ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS)cfop.CSTICMS : imposto != null && imposto.CSTICMSVenda != null ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS)imposto.CSTICMSVenda : 0;

                    decimal aliquotaInterna = 0, aliquotaInterestadual = 0, mvaOriginal = 0, mvaAjustado = 0;
                    if (CSTCalculaICMSST(cstICMS) && (cliente.Localidade.Estado.Sigla != siglaEstadoUsuario))
                    {
                        mvaOriginal = servico != null ? 0 : !CSTCalculaICMSST(cstICMS) ? 0 : cfop != null && cfop.MVA > 0 ? cfop.MVA : imposto != null && imposto.MVAVenda > 0 ? imposto.MVAVenda : 0;
                        aliquotaInterna = servico != null ? 0 : !CSTCalculaICMSST(cstICMS) ? 0 : cfop != null && cfop.AliquotaICMSInterna > 0 ? cfop.AliquotaICMSInterna : imposto != null && imposto.AliquotaICMSInternaVenda > 0 ? imposto.AliquotaICMSInternaVenda : 0;
                        aliquotaInterestadual = servico != null ? 0 : !CSTCalculaICMSST(cstICMS) ? 0 : cfop != null && cfop.AliquotaICMSInterestadual > 0 ? cfop.AliquotaICMSInterestadual : imposto != null && imposto.AliquotaICMSInterestadualVenda > 0 ? imposto.AliquotaICMSInterestadualVenda : 0;
                        if (aliquotaInterna > 0 && aliquotaInterestadual > 0 && mvaOriginal > 0)
                        {
                            if (aliquotaInterna > aliquotaInterestadual)
                            {
                                mvaAjustado = ((1 + (mvaOriginal / 100)) * ((1 - (aliquotaInterestadual / 100)) / (1 - (aliquotaInterna / 100)))) - 1;
                                mvaAjustado = mvaAjustado * 100;
                                mvaOriginal = mvaAjustado;
                            }
                        }
                    }
                    else if (CSTCalculaICMSST(cstICMS))
                    {
                        mvaOriginal = servico != null ? 0 : !CSTCalculaICMSST(cstICMS) ? 0 : cfop != null && cfop.MVA > 0 ? cfop.MVA : imposto != null && imposto.MVAVenda > 0 ? imposto.MVAVenda : 0;
                        aliquotaInterna = servico != null ? 0 : !CSTCalculaICMSST(cstICMS) ? 0 : cfop != null && cfop.AliquotaICMSInterna > 0 ? cfop.AliquotaICMSInterna : imposto != null && imposto.AliquotaICMSInternaVenda > 0 ? imposto.AliquotaICMSInternaVenda : 0;
                        aliquotaInterestadual = servico != null ? 0 : !CSTCalculaICMSST(cstICMS) ? 0 : cfop != null && cfop.AliquotaICMSInterestadual > 0 ? cfop.AliquotaICMSInterestadual : imposto != null && imposto.AliquotaICMSInterestadualVenda > 0 ? imposto.AliquotaICMSInterestadualVenda : 0;
                    }

                    decimal baseSTRetido = 0, aliquotaSTRetido = 0, valorSTRetido = 0;
                    decimal baseICMSEfetivo = 0, aliquotaICMSEfetivo = 0, valorICMSEfetivo = 0;
                    if (imposto != null && (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN500 ||
                        cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST60))//Imposto Retido e Efetivo
                    {
                        if (codigoAtividade == 7)//Não Contribuinte
                        {
                            //Efetivo
                            baseICMSEfetivo = valorTotalItem;
                            if (siglaEstadoUsuario == cliente.Localidade.Estado.Sigla)
                                aliquotaICMSEfetivo = imposto.AliquotaICMSInternaVenda;
                            else
                                aliquotaICMSEfetivo = imposto.AliquotaICMSInterestadualVenda;

                            if (baseICMSEfetivo > 0 && aliquotaICMSEfetivo > 0)
                                valorICMSEfetivo = Math.Round(baseICMSEfetivo * (aliquotaICMSEfetivo / 100), 2);
                            else
                            {
                                baseICMSEfetivo = 0;
                                aliquotaICMSEfetivo = 0;
                            }
                        }
                        else
                        {
                            //Retido
                            if (siglaEstadoUsuario == cliente.Localidade.Estado.Sigla)
                                baseSTRetido = quantidade * (produto.CustoMedio * ((imposto.MVACompra / 100) + 1));
                            else
                                baseSTRetido = quantidade * (produto.ValorVenda * ((imposto.MVACompra / 100) + 1));

                            valorSTRetido = (((produto.CustoMedio * quantidade) * (imposto.MVACompra / 100 + 1)) * (imposto.AliquotaICMSInternaVenda / 100)) -
                                            ((produto.CustoMedio * quantidade) * (imposto.AliquotaICMSCompra / 100));

                            baseSTRetido = Math.Round(baseSTRetido, 2);
                            valorSTRetido = Math.Round(valorSTRetido, 2);
                            if (baseSTRetido > 0 && valorSTRetido > 0)
                                aliquotaSTRetido = Math.Round((valorSTRetido / baseSTRetido) * 100, 2);
                        }
                    }

                    var dynTributacao = new
                    {
                        CodigoCFOP = servico != null ? cfopServico : cfopProduto != 0 ? cfopProduto : 0,
                        NumeroCFOP = servico != null ? cfopServicoNumero : cfopProdutoNumero != 0 ? cfopProdutoNumero : 0,

                        CSTCOFINS = cfop != null && cfop.CSTCOFINS != null ? (int)cfop.CSTCOFINS : imposto != null && imposto.CSTCOFINSVenda != null ? (int)imposto.CSTCOFINSVenda : 0,
                        BaseCOFINS = cfop != null && cfop.AliquotaCOFINS > 0 ? valorTotalItem.ToString("n2") : imposto != null && imposto.AliquotaCOFINSVenda > 0 ? valorTotalItem.ToString("n2") : 0.ToString("n2"),
                        ReducaoBaseCOFINS = cfop != null && cfop.ReducaoBCCOFINS > 0 ? cfop.ReducaoBCCOFINS.ToString("n2") : imposto != null && imposto.ReducaoBCCOFINSVenda > 0 ? imposto.ReducaoBCCOFINSVenda.ToString("n2") : 0.ToString("n2"),
                        AliquotaCOFINS = cfop != null && cfop.AliquotaCOFINS > 0 ? cfop.AliquotaCOFINS.ToString("n2") : imposto != null && imposto.AliquotaCOFINSVenda > 0 ? imposto.AliquotaCOFINSVenda.ToString("n2") : 0.ToString("n2"),
                        ValorCOFINS = 0.ToString("n2"),

                        CSTPIS = cfop != null && cfop.CSTPIS != null ? (int)cfop.CSTPIS : imposto != null && imposto.CSTPISVenda != null ? (int)imposto.CSTPISVenda : 0,
                        BasePIS = cfop != null && cfop.AliquotaPIS > 0 ? valorTotalItem.ToString("n2") : imposto != null && imposto.AliquotaPISVenda > 0 ? valorTotalItem.ToString("n2") : 0.ToString("n2"),
                        ReducaoBasePIS = cfop != null && cfop.ReducaoBCPIS > 0 ? cfop.ReducaoBCPIS.ToString("n2") : imposto != null && imposto.ReducaoBCPISVenda > 0 ? imposto.ReducaoBCPISVenda.ToString("n2") : 0.ToString("n2"),
                        AliquotaPIS = cfop != null && cfop.AliquotaPIS > 0 ? cfop.AliquotaPIS.ToString("n2") : imposto != null && imposto.AliquotaPISVenda > 0 ? imposto.AliquotaPISVenda.ToString("n2") : 0.ToString("n2"),
                        ValorPIS = 0.ToString("n2"),

                        CSTIPI = cfop != null && cfop.CSTIPI != null ? (int)cfop.CSTIPI : produto != null && produto.CSTIPIVenda != 0 ? (int)produto.CSTIPIVenda : 0,
                        BaseIPI = cfop != null && cfop.AliquotaIPI > 0 ? valorTotalItem.ToString("n2") : produto != null && produto.AliquotaIPIVenda > 0 ? valorTotalItem.ToString("n2") : 0.ToString("n2"),
                        ReducaoBaseIPI = cfop != null && cfop.ReducaoBCIPI > 0 ? cfop.ReducaoBCIPI.ToString("n2") : 0.ToString("n2"),
                        AliquotaIPI = cfop != null && cfop.AliquotaIPI > 0 ? cfop.AliquotaIPI.ToString("n2") : produto != null && produto.AliquotaIPIVenda > 0 ? produto.AliquotaIPIVenda.ToString("n2") : 0.ToString("n2"),
                        ValorIPI = 0.ToString("n2"),

                        BaseISS = servico != null && servico.AliquotaISS > 0 ? valorTotalItem.ToString("n2") : 0.ToString("n2"),
                        AliquotaISS = servico != null ? servico.AliquotaISS.ToString("n2") : 0.ToString("n2"),
                        ValorISS = 0.ToString("n2"),

                        CSTICMS = servico != null ? 0 : (int)cstICMS,
                        DescricaoCSTICMS = servico != null ? "" : Utilidades.String.OnlyNumbers(cstICMS.ToString()),
                        BaseICMS = servico != null ? 0.ToString("n2") : !CSTCalculaICMS(cstICMS) ? 0.ToString("n2") : cfop != null && cfop.AliquotaICMSInterna > 0 ? valorTotalItem.ToString("n2") : imposto != null && imposto.AliquotaICMSInternaVenda > 0 ? valorTotalItem.ToString("n2") : 0.ToString("n2"),
                        ReducaoBaseICMS = servico != null ? 0.ToString("n6") : !CSTCalculaICMS(cstICMS) ? 0.ToString("n6") : imposto != null && imposto.ReducaoBCICMSVenda > 0 ? imposto.ReducaoBCICMSVenda.ToString("n6") : 0.ToString("n6"),
                        AliquotaICMS = servico != null ? 0.ToString("n2") : !CSTCalculaICMS(cstICMS) ? 0.ToString("n2") : cfop != null && cfop.AliquotaICMSInterna > 0 ? cfop.AliquotaICMSInterna.ToString("n2") : imposto != null && imposto.AliquotaICMSInternaVenda > 0 ? imposto.AliquotaICMSInternaVenda.ToString("n2") : 0.ToString("n2"),
                        ValorICMS = 0.ToString("n2"),

                        BaseICMSDestino = servico != null ? 0.ToString("n2") : imposto != null && imposto.AliquotaDifalVenda > 0 ? baseICMSDifal.ToString("n2") : 0.ToString("n2"),
                        AliquotaICMSDestino = servico != null ? 0.ToString("n2") : imposto != null && imposto.AliquotaDifalVenda > 0 ? imposto.AliquotaDifalVenda.ToString("n2") : 0.ToString("n2"),
                        AliquotaICMSInterno = servico != null ? 0.ToString("n2") : imposto != null && imposto.AliquotaDifalVenda > 0 ? imposto.AliquotaICMSInternaVenda.ToString("n2") : 0.ToString("n2"),
                        PercentualPartilha = servico != null ? 0.ToString("n2") : imposto != null && imposto.AliquotaDifalVenda > 0 ? DateTime.Now.Year == 2016 ? 40.ToString("n2") : DateTime.Now.Year == 2017 ? 60.ToString("n2") : DateTime.Now.Year == 2018 ? 80.ToString("n2") : DateTime.Now.Year >= 2019 ? 100.ToString("n2") : 0.ToString("n2") : 0.ToString("n2"),
                        ValorICMSDestino = 0.ToString("n2"),
                        ValorICMSRemetente = 0.ToString("n2"),
                        BaseFCPDestino = 0.ToString("n2"),
                        PercentualFCP = servico != null ? 0.ToString("n2") : imposto != null && imposto.AliquotaFCPVenda > 0 ? imposto.AliquotaFCPVenda.ToString("n2") : 0.ToString("n2"),
                        ValorFCP = 0.ToString("n2"),

                        BaseICMSST = servico != null ? 0.ToString("n2") : !CSTCalculaICMSST(cstICMS) ? 0.ToString("n2") : cfop != null && cfop.AliquotaICMSInterna > 0 ? valorTotalItem.ToString("n2") : imposto != null && imposto.AliquotaICMSInternaVenda > 0 ? valorTotalItem.ToString("n2") : 0.ToString("n2"),
                        ReducaoBaseICMSST = servico != null ? 0.ToString("n2") : !CSTCalculaICMSST(cstICMS) ? 0.ToString("n2") : cfop != null && cfop.ReducaoMVA > 0 ? cfop.ReducaoMVA.ToString("n2") : imposto != null && imposto.ReducaoMVAVenda > 0 ? imposto.ReducaoMVAVenda.ToString("n2") : 0.ToString("n2"),
                        PercentualMVA = mvaOriginal.ToString("n2"),
                        AliquotaICMSST = aliquotaInterna.ToString("n2"),
                        AliquotaInterestadual = aliquotaInterestadual.ToString("n2"),
                        ValorICMSST = 0.ToString("n2"),

                        BCICMSSTRetido = baseSTRetido.ToString("n2"),
                        AliquotaICMSSTRetido = aliquotaSTRetido.ToString("n2"),
                        ValorICMSSTRetido = valorSTRetido.ToString("n2"),

                        BCICMSEfetivo = baseICMSEfetivo.ToString("n2"),
                        AliquotaICMSEfetivo = aliquotaICMSEfetivo.ToString("n2"),
                        ReducaoBCICMSEfetivo = 0.ToString("n2"),
                        ValorICMSEfetivo = valorICMSEfetivo.ToString("n2")
                    };

                    return dynTributacao;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return "Ocorreu uma falha ao carregar os impostos: " + ex.Message;
            }
        }

        public string GerarDanfeParaSMS(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal, Dominio.Entidades.Usuario usuario = null)
        {
            string caminhoRelatoriosEmbarcador = Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath();
            string caminhoDanfeSMS = Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoDanfeSMS;
            if (string.IsNullOrWhiteSpace(caminhoDanfeSMS))
                return string.Empty;

            string nomeArquivo = notaFiscal.Chave;
            string novoCaminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDanfeSMS, nomeArquivo) + ".pdf";
            if (notaFiscal.Status == Dominio.Enumeradores.StatusNFe.Cancelado)
            {
                nomeArquivo = nomeArquivo + "-canc";
                novoCaminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDanfeSMS, nomeArquivo) + ".pdf";
            }

            if (Utilidades.IO.FileStorageService.Storage.Exists(novoCaminho))
                return novoCaminho;

            Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = repRelatorio.BuscarPorTitulo("DANFE");
            if (relatorioOrigem != null)
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unidadeDeTrabalho);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = new Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio();
                dynRelatorio.Codigo = relatorioOrigem.Codigo;
                dynRelatorio.CodigoControleRelatorios = relatorioOrigem.CodigoControleRelatorios;
                dynRelatorio.Titulo = relatorioOrigem.Titulo;
                dynRelatorio.Descricao = relatorioOrigem.Descricao;
                dynRelatorio.Padrao = relatorioOrigem.Padrao;
                dynRelatorio.ExibirSumarios = relatorioOrigem.ExibirSumarios;
                dynRelatorio.CortarLinhas = relatorioOrigem.CortarLinhas;
                dynRelatorio.FundoListrado = relatorioOrigem.FundoListrado;
                dynRelatorio.TamanhoPadraoFonte = relatorioOrigem.TamanhoPadraoFonte;
                dynRelatorio.FontePadrao = relatorioOrigem.FontePadrao;
                dynRelatorio.AgruparRelatorio = false;
                dynRelatorio.PropriedadeAgrupa = relatorioOrigem.PropriedadeAgrupa;
                dynRelatorio.OrdemAgrupamento = relatorioOrigem.OrdemAgrupamento;
                dynRelatorio.PropriedadeOrdena = relatorioOrigem.PropriedadeOrdena;
                dynRelatorio.OrdemOrdenacao = relatorioOrigem.OrdemOrdenacao;
                dynRelatorio.TipoArquivoRelatorio = Dominio.Enumeradores.TipoArquivoRelatorio.PDF;
                dynRelatorio.OrientacaoRelatorio = relatorioOrigem.OrientacaoRelatorio;
                dynRelatorio.Grid = "{\"draw\":0,\"inicio\":0,\"limite\":0,\"indiceColunaOrdena\":0,\"dirOrdena\":\"desc\",\"recordsTotal\":0,\"recordsFiltered\":0,\"group\":{\"enable\":false,\"propAgrupa\":null,\"dirOrdena\":null},\"header\":[{\"title\":\"Cód. Produto\",\"data\":\"CodigoProduto\",\"width\":\"10%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":0,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Descrição dos Produtos/Serviços\",\"data\":\"DescricaoItem\",\"width\":\"25%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-left\",\"tabletHide\":false,\"phoneHide\":false,\"position\":1,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"NCM\",\"data\":\"CodigoNCMItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":2,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"CST/CSOSN\",\"data\":\"DescricaoCSTItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":3,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"CFOP\",\"data\":\"CodigoCFOPItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":4,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Unid.\",\"data\":\"DescricaoUnidadeItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":5,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Quantidade\",\"data\":\"QuantidadeItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":6,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"V. Unitário\",\"data\":\"ValorUnitarioItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":7,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"V. Total\",\"data\":\"ValorTotalItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":8,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"B.C. ICMS\",\"data\":\"BCICMSItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":9,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Valor ICMS\",\"data\":\"ValorICMSItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":10,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Valor IPI\",\"data\":\"ValorIPIItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":11,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"% ICMS\",\"data\":\"AliquotaICMSItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":12,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"% IPI\",\"data\":\"AliquotaIPIItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":13,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0}],\"data\":null,\"dataSumarizada\":null,\"order\":[{\"column\":0,\"dir\":\"desc\"}]}";
                dynRelatorio.Report = relatorioOrigem.Codigo;
                dynRelatorio.NovoRelatorio = true;

                usuario = usuario == null ? notaFiscal.Usuario : usuario;
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorioOrigem, usuario, dynRelatorio.TipoArquivoRelatorio, unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = null;

                relatorioTemp.PropriedadeOrdena = propOrdena;

                NotaFiscalEletronica serNotaFiscalEletronica = new NotaFiscalEletronica(unidadeDeTrabalho);
                serNotaFiscalEletronica.GerarRelatorioDANFE(notaFiscal, agrupamentos, relatorioControleGeracao, relatorioTemp, unidadeDeTrabalho.StringConexao);

                string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatoriosEmbarcador, relatorioControleGeracao.GuidArquivo) + ".pdf";
                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                {
                    if (Utilidades.IO.FileStorageService.Storage.Exists(novoCaminho))
                        Utilidades.IO.FileStorageService.Storage.Delete(novoCaminho);

                    Utilidades.IO.FileStorageService.Storage.Copy(caminhoArquivo, novoCaminho);
                    if (Utilidades.IO.FileStorageService.Storage.Exists(novoCaminho))
                        return novoCaminho;
                    else
                        return string.Empty;
                }
                else
                    return string.Empty;
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// Gerar registro na tabela 'T_XML_NOTA_FISCAL' para nota destinada
        /// </summary>
        /// <param name="nfeProc"></param>
        public bool GerarRegistroNotaFiscal(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc nfeProc, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Empresa empresa)
        {
            try
            {
                string notaJson = Newtonsoft.Json.JsonConvert.SerializeObject(nfeProc);
                ProcessarRegistroNotaFiscal(notaJson, unitOfWork, empresa);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Ocorreu uma falha ao gerar o registro da nota fiscal destinada: " + ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gerar registro na tabela 'T_XML_NOTA_FISCAL' para nota destinada
        /// </summary>
        /// <param name="nfeProc"></param>
        public bool GerarRegistroNotaFiscal(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc nfeProc, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Empresa empresa)
        {
            try
            {
                string notaJson = Newtonsoft.Json.JsonConvert.SerializeObject(nfeProc);
                ProcessarRegistroNotaFiscal(notaJson, unitOfWork, empresa);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Ocorreu uma falha ao gerar o registro da nota fiscal destinada: " + ex);
                return false;
            }
            return true;
        }

        public void ProcessarRegistroNotaFiscal(string notaJson, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Empresa empresa)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            dynamic dynNFe = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(notaJson);

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaExiste = repNotaFiscal.BuscarPorChave((string)dynNFe.protNFe.infProt.chNFe);

            if (notaExiste == null)
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal = ConverteDynamicNFeProcParaXMLNotaFiscal(dynNFe, unitOfWork, empresa);

                if (notaFiscal != null)
                    repNotaFiscal.Inserir(notaFiscal);
            }
        }

        public void AtualizarStatusCanceladaNotaFiscal(string chave, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nota = repNotaFiscal.BuscarPorChave(chave);
            if (nota != null && nota.CanceladaPeloEmitente == false)
            {
                nota.CanceladaPeloEmitente = true;
                repNotaFiscal.Atualizar(nota);
            }

        }


        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal ConverteDynamicNFeProcParaXMLNotaFiscal(dynamic dynNFe, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Empresa empresa)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nota = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();

            try
            {
                nota.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe;
                nota.Numero = dynNFe.NFe.infNFe.ide.nNF;
                nota.Chave = dynNFe.protNFe.infProt.chNFe;
                nota.Protocolo = dynNFe.protNFe.infProt.nProt;
                nota.Serie = dynNFe.NFe.infNFe.ide.serie;
                nota.Modelo = dynNFe.NFe.infNFe.ide.mod;
                nota.Valor = (decimal)dynNFe.NFe.infNFe.total.ICMSTot?.vNF;
                nota.ValorICMS = dynNFe.NFe.infNFe.total?.ICMSTot?.vICMS != null ? (decimal)dynNFe.NFe.infNFe.total.ICMSTot?.vICMS : decimal.Zero;
                nota.ValorST = dynNFe.NFe.infNFe.total?.ICMSTot?.vST != null ? (decimal)dynNFe.NFe.infNFe.total?.ICMSTot?.vST : decimal.Zero;
                nota.ValorTotalProdutos = dynNFe.NFe.infNFe.total?.ICMSTot?.vProd != null ? (decimal)dynNFe.NFe.infNFe.total?.ICMSTot?.vProd : decimal.Zero;
                nota.ValorSeguro = dynNFe.NFe.infNFe.total?.ICMSTot?.vSeg != null ? (decimal)dynNFe.NFe.infNFe.total.ICMSTot?.vSeg : decimal.Zero;
                nota.ValorDesconto = dynNFe.NFe.infNFe.total?.ICMSTot?.vDesc != null ? (decimal)dynNFe.NFe.infNFe.total.ICMSTot?.vDesc : decimal.Zero;
                nota.ValorIPI = dynNFe.NFe.infNFe.total?.ICMSTot?.vIPI != null ? (decimal)dynNFe.NFe.infNFe.total.ICMSTot?.vIPI : decimal.Zero;
                nota.ValorPIS = dynNFe.NFe.infNFe.total?.ICMSTot?.vPIS != null ? (decimal)dynNFe.NFe.infNFe.total.ICMSTot?.vPIS : decimal.Zero;
                nota.ValorCOFINS = dynNFe.NFe.infNFe.total?.ICMSTot?.vCOFINS != null ? (decimal)dynNFe.NFe.infNFe.total.ICMSTot?.vCOFINS : decimal.Zero;
                nota.ValorOutros = dynNFe.NFe.infNFe.total?.ICMSTot?.vOutro != null ? (decimal)dynNFe.NFe.infNFe.total.ICMSTot?.vOutro : decimal.Zero;
                nota.ValorFrete = dynNFe.NFe.infNFe.total?.ICMSTot?.vFrete != null ? (decimal)dynNFe.NFe.infNFe.total.ICMSTot?.vFrete : decimal.Zero;
                nota.CanceladaPeloEmitente = false;
                nota.NaturezaOP = dynNFe.NFe.infNFe.ide.natOp;
                nota.Empresa = empresa;
                nota.nfAtiva = true;

                string cnpjTransportador = null;
                if (dynNFe.NFe.infNFe.transp?.transporta != null)
                    cnpjTransportador = dynNFe.NFe.infNFe.transp?.transporta?.Item;

                nota.CNPJTranposrtador = !string.IsNullOrEmpty(cnpjTransportador) ? cnpjTransportador : string.Empty;

                decimal peso = 0;
                decimal pesoLiquido = 0;
                try
                {
                    if (dynNFe.NFe.infNFe.transp?.vol != null)
                    {
                        peso = dynNFe.NFe.infNFe.transp?.vol[0].pesoB != null ? (decimal)dynNFe.NFe.infNFe.transp?.vol[0].pesoB : decimal.Zero;
                        pesoLiquido = dynNFe.NFe.infNFe.transp?.vol[0].pesoL != null ? (decimal)dynNFe.NFe.infNFe.transp?.vol[0].pesoL : decimal.Zero;

                    }
                }
                catch (Exception ex)
                {
                }
                nota.Peso = peso;
                nota.PesoLiquido = pesoLiquido;

                nota.PlacaVeiculoNotaFiscal = dynNFe.NFe.infNFe.transp != null ? this.ObterPlacaVeiculo(dynNFe.NFe.infNFe.transp) : string.Empty;
                nota.XML = string.Empty;
                nota.ModalidadeFrete = dynNFe.NFe.infNFe.transp?.modFrete ?? null;
                nota.TipoOperacaoNotaFiscal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal)dynNFe.NFe.infNFe.ide.tpNF;

                DateTime dataEmissao = new DateTime();
                DateTime.TryParseExact((string)dynNFe.NFe.infNFe.ide.dhEmi, "MM/dd/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                nota.DataEmissao = dataEmissao;
                Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ((double)dynNFe.NFe.infNFe.dest.Item);

                if (destinatario == null)
                {
                    Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
                    Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                    Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);

                    destinatario = new Dominio.Entidades.Cliente();
                    destinatario.CPF_CNPJ = (long)dynNFe.NFe.infNFe.dest.Item;
                    destinatario.Nome = (Utilidades.String.RemoverCaracteresEspecialSerpro((string)(dynNFe.NFe.infNFe.dest.xNome)).Replace("&amp;", "")?.Replace(" amp ", ""));
                    destinatario.Endereco = dynNFe.NFe.infNFe.dest.enderDest.xLgr;
                    destinatario.Bairro = dynNFe.NFe.infNFe.dest.enderDest.xBairro;
                    destinatario.Cidade = dynNFe.NFe.infNFe.dest.enderDest.cMun;
                    destinatario.Complemento = dynNFe.NFe.infNFe.dest.enderDest.xCpl;
                    destinatario.Email = "";
                    destinatario.Numero = dynNFe.NFe.infNFe.dest.enderDest.nro;
                    destinatario.Pais = dynNFe.NFe.infNFe.dest.enderDest.cPais != null && (int)dynNFe.NFe.infNFe.dest.enderDest.cPais > 0 ? repPais.BuscarPorCodigo((int)dynNFe.NFe.infNFe.dest.enderDest.cPais) : null;
                    destinatario.Localidade = dynNFe.NFe.infNFe.dest.enderDest.cMun != null && (int)dynNFe.NFe.infNFe.dest.enderDest.cMun > 0 ? repLocalidade.BuscarPorCodigoIBGE((int)dynNFe.NFe.infNFe.dest.enderDest.cMun) : repLocalidade.BuscarPorCodigoIBGE(9999999);
                    destinatario.Atividade = repAtividade.BuscarPorCodigo(1);
                    destinatario.Ativo = true;
                    destinatario.Tipo = dynNFe.NFe.infNFe.dest.Item.Value.Length > 11 ? "J" : "F";
                    repCliente.Inserir(destinatario);
                }
                nota.Destinatario = destinatario;

                Dominio.Entidades.Cliente emitente = repCliente.BuscarPorCPFCNPJ((double)dynNFe.NFe.infNFe.emit.Item);

                if (emitente == null)
                {
                    Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
                    Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                    Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);

                    emitente = new Dominio.Entidades.Cliente();
                    emitente.CPF_CNPJ = (long)dynNFe.NFe.infNFe.emit.Item;
                    emitente.Nome = (Utilidades.String.RemoverCaracteresEspecialSerpro((string)(dynNFe.NFe.infNFe.emit.xNome)).Replace("&amp;", "")?.Replace(" amp ", ""));
                    emitente.Endereco = dynNFe.NFe.infNFe.emit.enderEmit.xLgr;
                    emitente.Bairro = dynNFe.NFe.infNFe.emit.enderEmit.xBairro;
                    emitente.Cidade = dynNFe.NFe.infNFe.emit.enderEmit.cMun;
                    emitente.Complemento = dynNFe.NFe.infNFe.emit.enderEmit.xCpl;
                    emitente.Email = "";
                    emitente.Numero = dynNFe.NFe.infNFe.emit.enderEmit.nro;
                    emitente.Pais = dynNFe.NFe.infNFe.emit.enderEmit.cPais != null && (int)dynNFe.NFe.infNFe.emit.enderEmit.cPais > 0 ? repPais.BuscarPorCodigo((int)dynNFe.NFe.infNFe.emit.enderEmit.cPais) : null;
                    emitente.Localidade = dynNFe.NFe.infNFe.emit.enderEmit.cMun != null && (int)dynNFe.NFe.infNFe.emit.enderEmit.cMun > 0 ? repLocalidade.BuscarPorCodigoIBGE((int)dynNFe.NFe.infNFe.emit.enderEmit.cMun) : repLocalidade.BuscarPorCodigoIBGE(9999999);
                    emitente.Atividade = repAtividade.BuscarPorCodigo(1);
                    emitente.Ativo = true;
                    emitente.Tipo = dynNFe.NFe.infNFe.emit.Item.Value.Length > 11 ? "J" : "F";
                    repCliente.Inserir(emitente);
                }
                nota.Emitente = emitente;

                try
                {
                    if (nota.Emitente != null)
                    {
                        Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoaEmitente = repGrupoPessoas.BuscarGrupoCliente(nota.Emitente.CPF_CNPJ);

                        if (grupoPessoaEmitente != null)
                        {
                            Repositorio.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml repGrupoPessoasLeituraDinamicaXml = new Repositorio.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml(unitOfWork);
                            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml> listaGrupoPessoasLeituraDinamicaXml = repGrupoPessoasLeituraDinamicaXml.BuscarPorGrupoPessoas(grupoPessoaEmitente.Codigo);
                            List<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml> listaLeituraDinamicaXml = null;

                            if (listaGrupoPessoasLeituraDinamicaXml?.Count() > 0)
                            {
                                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");
                                Servicos.Embarcador.NFe.NFe serNfe = new NFe.NFe();
                                listaLeituraDinamicaXml = serNfe.BuscarValoresLeituraDinamicaXML(listaGrupoPessoasLeituraDinamicaXml, dynNFe.NFe.infNFe);

                                var leituraXMLMetrosCubicos = listaLeituraDinamicaXml.Where(o => o.CampoDestino == "MetrosCubicos").FirstOrDefault();
                                if (leituraXMLMetrosCubicos != null)
                                {
                                    decimal metrosCubicosTryParse = 0;
                                    if (decimal.TryParse(leituraXMLMetrosCubicos.Valor, NumberStyles.Number, cultura, out metrosCubicosTryParse))
                                        nota.MetrosCubicos = metrosCubicosTryParse;
                                }

                                var leituraXMLQuantidadePallets = listaLeituraDinamicaXml.Where(o => o.CampoDestino == "QuantidadePallets").FirstOrDefault();
                                if (leituraXMLQuantidadePallets != null)
                                {
                                    decimal quantidadePalletsTryParse = 0;
                                    if (decimal.TryParse(leituraXMLQuantidadePallets.Valor, NumberStyles.Number, cultura, out quantidadePalletsTryParse))
                                        nota.QuantidadePallets = quantidadePalletsTryParse;
                                }

                                var leituraXMLNumeroControleCliente = listaLeituraDinamicaXml.Where(o => o.CampoDestino == "NumeroControleCliente").FirstOrDefault();
                                if (!string.IsNullOrEmpty(leituraXMLNumeroControleCliente?.Valor))
                                    nota.NumeroControleCliente = leituraXMLNumeroControleCliente.Valor;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Ocorreu uma falha ao converter dynamic de NFE para nota fiscal (BuscarValoresLeituraDinamicaXML): " + ex);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Ocorreu uma falha ao converter dynamic de NFE para nota fiscal: " + ex);
                return null;
            }

            return nota;
        }
        #endregion

        #region Métodos Privados

        private bool CSTCalculaICMS(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS? cstICMS)
        {
            if ((int)cstICMS == 0)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST00)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST10)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST20)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST30)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST40 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST41 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST50)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST51)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST60)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST70)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST90)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN101)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN102 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN103 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN300 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN400)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN201)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN202 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN203)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN500)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN900)
                return true;
            else
                return false;
        }

        private bool CSTCalculaICMSST(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS? cstICMS)
        {
            if ((int)cstICMS == 0)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST00)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST10)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST20)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST30)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST40 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST41 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST50)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST51)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST60)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST70)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST90)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN101)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN102 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN103 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN300 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN400)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN201)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN202 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN203)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN500)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN900)
                return true;
            else
                return false;
        }

        private string BuscarObservacoesFiscais(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica nfeIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.ObservacaoFiscal repObservacaoFiscal = new Repositorio.Embarcador.NotaFiscal.ObservacaoFiscal(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscalObservacaoFiscal repNotaFiscalObservacaoFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscalObservacaoFiscal(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);

            var observacao = string.Empty;
            List<string> ncms = new List<string>();
            List<int> cfops = new List<int>();
            List<int> csts = new List<int>();

            if (nfeIntegracao != null && nfeIntegracao.ItensNFe != null && nfeIntegracao.ItensNFe.Count > 0)
            {
                foreach (var item in nfeIntegracao.ItensNFe)
                {
                    if (item.Produto != null)
                    {
                        var ncm = repProduto.BuscarPorCodigo(item.Produto.Codigo)?.CodigoNCM ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(ncm))
                            ncms.Add(ncm);
                    }
                    if (item.CFOP != null)
                        cfops.Add(item.CFOP);
                    if (item.CSTICMS != null && item.CSTICMS.HasValue)
                        csts.Add((int)item.CSTICMS);
                }
                ncms = ncms.Distinct().ToList();
                cfops = cfops.Distinct().ToList();
                csts = csts.Distinct().ToList();
            }

            List<Dominio.Entidades.Embarcador.NotaFiscal.ObservacaoFiscal> observacaoFiscais = repObservacaoFiscal.BuscarPorDadosNota(nfe.Empresa.Codigo, nfe.Cliente.Localidade.Estado.Sigla, nfe.Atividade.Codigo, nfe.NaturezaDaOperacao.Codigo, ncms, cfops, csts);
            foreach (var obs in observacaoFiscais)
            {
                if (string.IsNullOrWhiteSpace(observacao))
                    observacao += " " + obs.Observacao;
                else
                    observacao += " - " + obs.Observacao;

                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoFiscal notaFiscalObservacaoFiscal = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoFiscal();
                notaFiscalObservacaoFiscal.NotaFiscal = nfe;
                notaFiscalObservacaoFiscal.ObservacaoFiscal = obs;

                repNotaFiscalObservacaoFiscal.Inserir(notaFiscalObservacaoFiscal);
            }

            return observacao;
        }

        private string ObterPlacaVeiculo(dynamic dynInfNFeTransp)
        {
            if (dynInfNFeTransp != null)
            {
                if (dynInfNFeTransp.Items != null)
                {
                    foreach (var item in dynInfNFeTransp.Items)
                    {
                        string placa = item.placa;
                        if (!string.IsNullOrEmpty(placa))
                            return item.placa;
                    }
                }
            }

            return string.Empty;
        }

        #endregion
    }
}