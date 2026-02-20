using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace Servicos
{
    public class NFe : ServicoBase
    {
        #region Construtores
        /// <summary>
        /// Propriedade que pode ser null! valide antes de utilizar.
        /// </summary>
        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;        

        public NFe(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        public NFe(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : base(unitOfWork)
        {
            _auditado = auditado;
        }

        #endregion

        #region Propriedades Publicas
        
        public bool NaoAtualizarDadosPessoaEmImportacoesDeNotasFiscaisOuIntegracoes { get; set; } = false;

        #endregion

        #region Metodos Globais

        public object ObterDocumentoPorXMLFrimesa(System.IO.Stream xml, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);

            List<object> listaNotasRetorno = new List<object>();

            xml.Position = 0;
            XDocument doc = XDocument.Load(xml);

            var carga = "";
            var cnpjRemetente = "";
            var placaVeiculo = "";

            foreach (XElement xDadosCarga in doc.Element("loteCarga").Elements("dadosCarga"))
            {
                carga = xDadosCarga.Element("numeroCarga").Value;
            }

            foreach (XElement xDadosEmitente in doc.Element("loteCarga").Element("dadosCarga").Elements("dadosEmitente"))
            {
                cnpjRemetente = xDadosEmitente.Element("cnpj").Value;
                CadastrarClienteFrimesa(xDadosEmitente, codigoEmpresa, unitOfWork);
            }

            foreach (XElement xDadosVeiculo in doc.Element("loteCarga").Element("dadosCarga").Elements("dadosTransportador").Elements("dadosVeiculo"))
            {
                placaVeiculo = xDadosVeiculo.Element("placa").Value;
            }

            foreach (XElement xDadosLocalEntrega in doc.Element("loteCarga").Element("dadosCarga").Elements("dadosLocalEntrega"))
            {
                foreach (XElement xDadosDestinatario in xDadosLocalEntrega.Elements("dadosDestinatario"))
                {
                    var cnpjDestinatario = xDadosDestinatario.Element("cnpj").Value;
                    CadastrarClienteFrimesa(xDadosDestinatario, codigoEmpresa, unitOfWork);

                    foreach (XElement xDadosNFe in xDadosDestinatario.Elements("dadosNFe"))
                    {
                        var nota = new
                        {
                            Chave = xDadosNFe.Element("chaveAcesso").Value,
                            ValorTotal = decimal.Parse(xDadosNFe.Element("valorNf").Value, cultura),
                            DataEmissao = DateTime.Today.ToString("dd/MM/yyyy"),
                            Remetente = cnpjRemetente,
                            Destinatario = cnpjDestinatario,
                            Numero = xDadosNFe.Element("numeroNf").Value,
                            Peso = Decimal.Parse(xDadosNFe.Element("pesoBruto").Value, cultura),
                            FormaPagamento = Dominio.Enumeradores.TipoPagamento.Pago,
                            Placa = placaVeiculo,
                            NumeroDosCTesUtilizados = repDocumentosCTe.BuscarNumeroStatusDoCTePorChaveEEmpresa(codigoEmpresa, xDadosNFe.Element("chaveAcesso").Value),//repDocumentosCTe.BuscarNumeroDoCTePorChaveEEmpresa(codigoEmpresa, xDadosNFe.Element("chaveAcesso").Value),
                            Serie = 0,
                            Observacao = !string.IsNullOrWhiteSpace(carga) ? "Carga: " + carga : string.Empty,
                            Volume = Decimal.Parse(xDadosNFe.Element("quantidadeVolumes").Value, cultura)
                        };
                        listaNotasRetorno.Add(nota);
                    }
                }
            }
            return listaNotasRetorno;
        }


        public void CadastrarClienteFrimesa(XElement xDados, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

                double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(xDados.Element("cnpj").Value));

                bool inserir = false;

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                if (cliente == null)
                {
                    cliente = new Dominio.Entidades.Cliente();
                    inserir = true;
                }

                cliente.Bairro = xDados.Element("bairro").Value;
                cliente.CEP = xDados.Element("cep").Value;
                cliente.Complemento = string.Empty;
                cliente.CPF_CNPJ = cpfCnpj;
                cliente.Endereco = xDados.Element("logradouro").Value;
                if (xDados.Element("inscricaoEstadual").Value != null && xDados.Element("inscricaoEstadual").Value != "")
                    cliente.IE_RG = xDados.Element("inscricaoEstadual").Value;
                else
                {
                    cliente.IE_RG = "ISENTO";
                }
                cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(xDados.Element("codigoMunicipio").Value));
                cliente.Nome = xDados.Element("razaoSocial").Value;
                cliente.NomeFantasia = xDados.Element("razaoSocial").Value;
                cliente.Numero = xDados.Element("numero").Value;
                //cliente.Telefone1 = string.Empty;
                cliente.Tipo = Utilidades.String.OnlyNumbers(xDados.Element("cnpj").Value).Length == 14 ? "J" : "F";

                if (cliente.Atividade == null)
                    cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao, 0, unidadeDeTrabalho);

                if (cliente.Tipo == "F" && cliente.Atividade.Codigo == 7 && string.IsNullOrWhiteSpace(cliente.IE_RG))
                    cliente.IE_RG = "ISENTO";

                if (inserir)
                {
                    if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                    {
                        Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                        if (grupoPessoas != null)
                        {
                            cliente.GrupoPessoas = grupoPessoas;
                        }
                    }
                    if (cliente.IE_RG == "ISENTO")
                        cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte;
                    else
                        cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS;
                    cliente.Ativo = true;
                    cliente.DataCadastro = DateTime.Now;
                    cliente.DataUltimaAtualizacao = DateTime.Now;
                    cliente.Integrado = false;
                    repCliente.Inserir(cliente);
                }
                else
                {
                    cliente.DataUltimaAtualizacao = DateTime.Now;
                    cliente.Integrado = false;
                    repCliente.Atualizar(cliente);
                }
            }
            catch
            {

            }
        }

        public object ObterDocumentoPorXML(System.IO.Stream xml, int codigoEmpresa, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            object nfe = MultiSoftware.NFe.Servicos.Leitura.Ler(xml);

            if (nfe == null)
                return null;
            else if (nfe.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc))
                return this.ObterDocumentoPorXML((MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc)nfe, codigoEmpresa, unitOfWork);
            else if (nfe.GetType() == typeof(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc))
                return this.ObterDocumentoPorXML((MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)nfe, codigoEmpresa, usuario, unitOfWork);
            else if (nfe.GetType() == typeof(MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc))
                return this.ObterDocumentoPorXML((MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc)nfe, codigoEmpresa, unitOfWork);
            else if (nfe.GetType() == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFe))
                return this.ObterDocumentoPorXML((MultiSoftware.NFe.v400.NotaFiscal.TNFe)nfe, codigoEmpresa, unitOfWork);
            else
                return null;
        }

        public Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe ObterDocumentoPorXML(System.IO.Stream xml, Repositorio.UnitOfWork unitOfWork, bool buscarDocumentosAnterior = true, bool importarEmailCliente = false, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null)
        {
            object nfe = MultiSoftware.NFe.Servicos.Leitura.Ler(xml);

            if (nfe == null)
                throw new NullReferenceException("Não foi possível ler o XML da NF-e.");

            if (nfe.GetType() == typeof(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc))
                return this.ObterDocumentoPorXML((MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)nfe, unitOfWork, buscarDocumentosAnterior, importarEmailCliente, cargaPedido);
            if (nfe.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc))
                return this.ObterDocumentoPorXML((MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc)nfe, unitOfWork);
            else if (nfe.GetType() == typeof(MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc))
                return this.ObterDocumentoPorXML((MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc)nfe, unitOfWork);
            else if (nfe.GetType() == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFe))
                return this.ObterDocumentoPorXML((MultiSoftware.NFe.v400.NotaFiscal.TNFe)nfe, unitOfWork, importarEmailCliente);
            else
                return null;
        }

        public object ObterDocumentoPorObjeto(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nfe, int codigoEmpresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unidadeTrabalho);

            DateTime dataEmissao = nfe.DataEmissao;
            var notaRetorno = new
            {
                Chave = nfe.Chave,
                ValorTotal = nfe.Valor,
                DataEmissao = dataEmissao.ToString("dd/MM/yyyy"),
                Remetente = nfe.Emitente.CPF_CNPJ_SemFormato,
                Destinatario = nfe.Destinatario.CPF_CNPJ_SemFormato,
                Numero = nfe.Numero,
                Peso = nfe.Peso,
                UnidadeMedida = this.ObterUnidadeMedida(unidadeTrabalho),
                FormaPagamento = nfe.ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar ? Dominio.Enumeradores.TipoPagamento.A_Pagar : Dominio.Enumeradores.TipoPagamento.Pago,
                Placa = string.Empty,
                NumeroDosCTesUtilizados = repDocumentosCTe.BuscarNumeroStatusDoCTePorChaveEEmpresa(codigoEmpresa, nfe.Chave),
                Serie = nfe.Serie,
                Observacao = nfe.Observacao,
                Volume = nfe.Volumes,
                CFOP = nfe.CFOP,
                NumeroReferenciaEDI = "",
                PINSuframa = "",
                NCMPredominante = nfe.NCM,
                NumeroControleCliente = nfe.NumeroControleCliente
            };

            return notaRetorno;
        }

        public object ObterDocumentoPorObjeto(Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal nfe, int codigoEmpresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unidadeTrabalho);

            DateTime dataEmissao;
            if (!DateTime.TryParseExact(nfe.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                dataEmissao = DateTime.Now;

            var notaRetorno = new
            {
                Chave = nfe.Chave,
                ValorTotal = nfe.Valor,
                DataEmissao = dataEmissao.ToString("dd/MM/yyyy"),
                Remetente = this.ObterCNPJPessoa(nfe.Emitente, codigoEmpresa, unidadeTrabalho),
                Destinatario = this.ObterCNPJPessoa(nfe.Destinatario, codigoEmpresa, unidadeTrabalho),
                Numero = nfe.Numero,
                Peso = nfe.PesoBruto,
                UnidadeMedida = this.ObterUnidadeMedida(unidadeTrabalho),
                FormaPagamento = nfe.ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar ? Dominio.Enumeradores.TipoPagamento.A_Pagar : Dominio.Enumeradores.TipoPagamento.Pago,
                Placa = nfe.Veiculo?.Placa ?? string.Empty,
                NumeroDosCTesUtilizados = repDocumentosCTe.BuscarNumeroStatusDoCTePorChaveEEmpresa(codigoEmpresa, nfe.Chave),// repDocumentosCTe.BuscarNumeroDoCTePorChaveEEmpresa(codigoEmpresa, nfe.Chave),
                Serie = this.ObterSerie(nfe.Emitente, codigoEmpresa, unidadeTrabalho),
                Observacao = nfe.InformacoesComplementares,
                Volume = nfe.VolumesTotal,
                CFOP = nfe.CFOPPredominante,
                NumeroReferenciaEDI = "",
                PINSuframa = "",
                NCMPredominante = nfe.NCMPredominante,
                NumeroControleCliente = nfe.NumeroControleCliente
            };

            return notaRetorno;
        }

        public Dominio.Entidades.Cliente ObterEmitente(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeEmit infNFeEmit, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(infNFeEmit.Item));

            bool inserir = false;

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            if (cliente == null)
            {
                cliente = new Dominio.Entidades.Cliente();
                inserir = true;
            }
            else if (cliente.NaoAtualizarDados)
                return cliente;

            cliente.Bairro = infNFeEmit.enderEmit.xBairro.Length > 2 ? infNFeEmit.enderEmit.xBairro.Length > 40 ? infNFeEmit.enderEmit.xBairro.Substring(0, 40) : infNFeEmit.enderEmit.xBairro : "Não Informado";
            cliente.CEP = infNFeEmit.enderEmit.CEP;
            cliente.Complemento = infNFeEmit.enderEmit.xCpl;
            cliente.CPF_CNPJ = cpfCnpj;
            cliente.Endereco = infNFeEmit.enderEmit.xLgr;
            if (infNFeEmit.IE != null && infNFeEmit.IE != "")
                cliente.IE_RG = infNFeEmit.IE;
            else
            {
                cliente.IE_RG = "ISENTO";
            }
            cliente.InscricaoMunicipal = infNFeEmit.IM;
            cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(infNFeEmit.enderEmit.cMun));
            cliente.Nome = Utilidades.String.RemoverCaracteresEspecialSerpro(infNFeEmit.xNome?.Replace("&amp;", "")?.Replace(" amp ", ""));
            cliente.NomeFantasia = Utilidades.String.RemoverCaracteresEspecialSerpro(infNFeEmit.xFant?.Replace("&amp;", "")?.Replace(" amp ", ""));
            cliente.Numero = string.IsNullOrWhiteSpace(infNFeEmit.enderEmit.nro) ? "S/N" : infNFeEmit.enderEmit.nro;
            cliente.Telefone1 = infNFeEmit.enderEmit.fone == null || infNFeEmit.enderEmit.fone.StartsWith("00") ? string.Empty : infNFeEmit.enderEmit.fone;
            cliente.Tipo = Utilidades.String.OnlyNumbers(infNFeEmit.Item).Length == 14 ? "J" : "F";

            if (cliente.Atividade == null)
                cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao, 0, unidadeDeTrabalho);

            if (cliente.Tipo == "F" && cliente.Atividade.Codigo == 7 && string.IsNullOrWhiteSpace(cliente.IE_RG))
                cliente.IE_RG = "ISENTO";

            if (inserir)
            {
                if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                {
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                    if (grupoPessoas != null)
                    {
                        cliente.GrupoPessoas = grupoPessoas;
                    }
                }
                if (cliente.IE_RG == "ISENTO")
                    cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte;
                else
                    cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS;
                cliente.Ativo = true;
                cliente.AguardandoConferenciaInformacao = true;
                cliente.DataCadastro = DateTime.Now;
                cliente.DataUltimaAtualizacao = DateTime.Now;
                cliente.Integrado = false;
                repCliente.Inserir(cliente);
            }
            else
            {
                cliente.DataUltimaAtualizacao = DateTime.Now;
                cliente.Integrado = false;
                repCliente.Atualizar(cliente);
            }
            new Repositorio.Embarcador.Pessoas.PessoaIntegracao(unidadeDeTrabalho).GerarIntegracaoPessoa(unidadeDeTrabalho, cliente);

            return cliente;
        }

        public Dominio.Entidades.Cliente ObterEmitente(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeEmit infNFeEmit, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(infNFeEmit.Item));

            bool inserir = false;

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            if (cliente == null)
            {
                cliente = new Dominio.Entidades.Cliente();
                inserir = true;
            }
            else if (cliente.NaoAtualizarDados)
                return cliente;

            cliente.Bairro = infNFeEmit.enderEmit.xBairro.Length > 2 ? infNFeEmit.enderEmit.xBairro.Length > 40 ? infNFeEmit.enderEmit.xBairro.Substring(0, 40) : infNFeEmit.enderEmit.xBairro : "Não Informado";
            cliente.CEP = infNFeEmit.enderEmit.CEP;
            cliente.Complemento = infNFeEmit.enderEmit.xCpl;
            cliente.CPF_CNPJ = cpfCnpj;
            cliente.Endereco = infNFeEmit.enderEmit.xLgr;
            if (infNFeEmit.IE != null && infNFeEmit.IE != "")
                cliente.IE_RG = infNFeEmit.IE;
            else
            {
                cliente.IE_RG = "ISENTO";
            }
            cliente.InscricaoMunicipal = infNFeEmit.IM;
            cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(infNFeEmit.enderEmit.cMun));
            cliente.Nome = Utilidades.String.RemoverCaracteresEspecialSerpro(infNFeEmit.xNome?.Replace("&amp;", "")?.Replace(" amp ", ""));
            cliente.NomeFantasia = Utilidades.String.RemoverCaracteresEspecialSerpro(infNFeEmit.xFant?.Replace("&amp;", "")?.Replace(" amp ", ""));
            cliente.Numero = string.IsNullOrWhiteSpace(infNFeEmit.enderEmit.nro) ? "S/N" : infNFeEmit.enderEmit.nro;

            if (cliente.Numero == "0")
                cliente.Numero = "S/N";

            string telefone = (string.IsNullOrWhiteSpace(infNFeEmit.enderEmit.fone) || infNFeEmit.enderEmit.fone.StartsWith("00") ? string.Empty : infNFeEmit.enderEmit.fone);

            if (telefone.Length <= 15)
                cliente.Telefone1 = telefone;

            cliente.Tipo = Utilidades.String.OnlyNumbers(infNFeEmit.Item).Length == 14 ? "J" : "F";

            if (cliente.Atividade == null)
                cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao, 0, unidadeDeTrabalho);

            if (cliente.Tipo == "F" && cliente.Atividade.Codigo == 7 && string.IsNullOrWhiteSpace(cliente.IE_RG))
                cliente.IE_RG = "ISENTO";

            if (inserir)
            {
                if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                {
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                    if (grupoPessoas != null)
                    {
                        cliente.GrupoPessoas = grupoPessoas;
                    }
                }
                if (cliente.IE_RG == "ISENTO")
                    cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte;
                else
                    cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS;
                cliente.Ativo = true;
                cliente.DataCadastro = DateTime.Now;
                cliente.DataUltimaAtualizacao = DateTime.Now;
                cliente.Integrado = false;
                repCliente.Inserir(cliente);
            }
            else
            {
                cliente.DataUltimaAtualizacao = DateTime.Now;
                cliente.Integrado = false;
                repCliente.Atualizar(cliente);
            }
                new Repositorio.Embarcador.Pessoas.PessoaIntegracao(unidadeDeTrabalho).GerarIntegracaoPessoa(unidadeDeTrabalho, cliente);

            return cliente;
        }
        
        public Dominio.Entidades.Cliente ObterEmitente(dynamic infNFeEmit, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho = null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
        {
            if (unidadeDeTrabalho == null)
                unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers((string)infNFeEmit.CNPJ));

            bool inserir = false;

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            if (cliente == null)
            {
                cliente = new Dominio.Entidades.Cliente();
                inserir = true;
            }
            else if (cliente.NaoAtualizarDados)
                return cliente;

            cliente.Bairro = ((string)infNFeEmit.enderEmit.xBairro).Length > 2 ? ((string)infNFeEmit.enderEmit.xBairro).Length > 40 ? ((string)infNFeEmit.enderEmit.xBairro).Substring(0, 40) : ((string)infNFeEmit.enderEmit.xBairro) : "Não Informado";
            cliente.CEP = (string)infNFeEmit.enderEmit.CEP;
            cliente.Complemento = (string)infNFeEmit.enderEmit.xCpl;
            cliente.CPF_CNPJ = cpfCnpj;
            cliente.Endereco = (string)infNFeEmit.enderEmit.xLgr;

            if (infNFeEmit.IE != null && (string)infNFeEmit.IE != "")
                cliente.IE_RG = (string)infNFeEmit.IE;
            else
                cliente.IE_RG = "ISENTO";

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                cliente.InscricaoMunicipal = (string)infNFeEmit.IM;
            cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse((string)infNFeEmit.enderEmit.cMun));
            string nome = Utilidades.String.RemoveAllSpecialCharacters((string)infNFeEmit.xNome);
            if (string.IsNullOrWhiteSpace(cliente.Nome))
                cliente.Nome = Utilidades.String.RemoverCaracteresEspecialSerpro(nome?.Replace("&amp;", "")?.Replace(" amp ", ""));
            else if (nome == (string)infNFeEmit.xNome)
                cliente.Nome = Utilidades.String.RemoverCaracteresEspecialSerpro(nome?.Replace("&amp;", "")?.Replace(" amp ", ""));
            cliente.NomeFantasia = (string)infNFeEmit.xFant;
            cliente.NomeFantasia = Utilidades.String.RemoverCaracteresEspecialSerpro(cliente.NomeFantasia?.Replace("&amp;", "")?.Replace(" amp ", ""));
            cliente.Numero = string.IsNullOrWhiteSpace((string)infNFeEmit.enderEmit.nro) ? "S/N" : (string)infNFeEmit.enderEmit.nro;

            string telefone = (string.IsNullOrWhiteSpace((string)infNFeEmit.enderEmit.fone) || ((string)infNFeEmit.enderEmit.fone).StartsWith("00") ? string.Empty : (string)infNFeEmit.enderEmit.fone);

            if (telefone.Length <= 15)
                cliente.Telefone1 = telefone;

            cliente.Tipo = Utilidades.String.OnlyNumbers((string)infNFeEmit.CNPJ).Length == 14 ? "J" : "F";

            if (cliente.Atividade == null)
                cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao, 0, unidadeDeTrabalho);

            if (cliente.Tipo == "F" && cliente.Atividade.Codigo == 7 && string.IsNullOrWhiteSpace(cliente.IE_RG))
                cliente.IE_RG = "ISENTO";

            if (inserir)
            {
                if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                {
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                    if (grupoPessoas != null)
                    {
                        cliente.GrupoPessoas = grupoPessoas;
                    }
                }
                if (cliente.IE_RG == "ISENTO")
                    cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte;
                else
                    cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS;
                cliente.Ativo = true;
                cliente.AguardandoConferenciaInformacao = true;
                cliente.DataCadastro = DateTime.Now;
                cliente.DataUltimaAtualizacao = DateTime.Now;
                cliente.Integrado = false;
                repCliente.Inserir(cliente);
            }
            else
            {
                cliente.DataUltimaAtualizacao = DateTime.Now;
                cliente.Integrado = false;
                repCliente.Atualizar(cliente);
            }
                new Repositorio.Embarcador.Pessoas.PessoaIntegracao(unidadeDeTrabalho).GerarIntegracaoPessoa(unidadeDeTrabalho, cliente);

            return cliente;
        }

        public Dominio.Entidades.Cliente ObterEmitente(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeEmit infNFeEmit, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho = null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
        {
            if (unidadeDeTrabalho == null)
                unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

            double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(infNFeEmit.Item));

            bool inserir = false;

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            if (cliente == null)
            {
                cliente = new Dominio.Entidades.Cliente();
                inserir = true;
            }
            else if (cliente.NaoAtualizarDados || NaoAtualizarDadosPessoaEmImportacoesDeNotasFiscaisOuIntegracoes)
                return cliente;

            cliente.Tipo = Utilidades.String.OnlyNumbers(infNFeEmit.Item).Length == 14 ? "J" : "F";
            cliente.Bairro = infNFeEmit.enderEmit.xBairro.Length > 2 ? infNFeEmit.enderEmit.xBairro.Length > 40 ? infNFeEmit.enderEmit.xBairro.Substring(0, 40) : infNFeEmit.enderEmit.xBairro : "Não Informado";
            cliente.CEP = infNFeEmit.enderEmit.CEP;
            cliente.Complemento = infNFeEmit.enderEmit.xCpl;
            cliente.CPF_CNPJ = cpfCnpj;
            cliente.Endereco = infNFeEmit.enderEmit.xLgr;

            if (infNFeEmit.IE != null && infNFeEmit.IE != "")
                cliente.IE_RG = infNFeEmit.IE;
            else
                cliente.IE_RG = "ISENTO";

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                cliente.InscricaoMunicipal = infNFeEmit.IM;
            cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(infNFeEmit.enderEmit.cMun));

            cliente.Nome = Utilidades.String.RemoverCaracteresEspecialSerpro(infNFeEmit.xNome?.Replace("&amp;", "")?.Replace(" amp ", ""));
            cliente.NomeFantasia = Utilidades.String.RemoverCaracteresEspecialSerpro(infNFeEmit.xFant?.Replace("&amp;", "")?.Replace(" amp ", ""));
            cliente.Numero = string.IsNullOrWhiteSpace(infNFeEmit.enderEmit.nro) ? "S/N" : infNFeEmit.enderEmit.nro;

            string telefone = (string.IsNullOrWhiteSpace(infNFeEmit.enderEmit.fone) || infNFeEmit.enderEmit.fone.StartsWith("00") ? string.Empty : infNFeEmit.enderEmit.fone);

            if (telefone.Length <= 15)
                cliente.Telefone1 = telefone;

            cliente.Tipo = Utilidades.String.OnlyNumbers(infNFeEmit.Item).Length == 14 ? "J" : "F";

            if (cliente.Atividade == null)
                cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao, 0, unidadeDeTrabalho);

            if (cliente.Tipo == "F" && cliente.Atividade.Codigo == 7 && string.IsNullOrWhiteSpace(cliente.IE_RG))
                cliente.IE_RG = "ISENTO";

            if (inserir)
            {
                if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                {
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                    if (grupoPessoas != null)
                    {
                        cliente.GrupoPessoas = grupoPessoas;
                    }
                }
                if (cliente.IE_RG == "ISENTO")
                    cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte;
                else
                    cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS;
                cliente.Ativo = true;
                cliente.AguardandoConferenciaInformacao = true;
                cliente.DataCadastro = DateTime.Now;
                cliente.DataUltimaAtualizacao = DateTime.Now;
                cliente.Integrado = false;
                repCliente.Inserir(cliente);
                PreencherDadosFornecedor(cliente, unidadeDeTrabalho, true);

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                    if (empresa.VisualizarSomenteClientesAssociados)
                    {
                        Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unidadeDeTrabalho);
                        Dominio.Entidades.DadosCliente dadosCliente = new Dominio.Entidades.DadosCliente()
                        {
                            Cliente = cliente,
                            Empresa = empresa
                        };
                        repDadosCliente.Inserir(dadosCliente);
                    }
                }
            }
            else
            {
                cliente.DataUltimaAtualizacao = DateTime.Now;
                cliente.Integrado = false;
                repCliente.Atualizar(cliente);
            }
            new Repositorio.Embarcador.Pessoas.PessoaIntegracao(unidadeDeTrabalho).GerarIntegracaoPessoa(unidadeDeTrabalho, cliente);

            return cliente;
        }

        public Dominio.Entidades.Cliente ObterPessoa(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            string cpfCNPJPessoa = Utilidades.String.OnlyNumbers(pessoa.CPFCNPJ);

            double cpfCnpj = double.Parse(cpfCNPJPessoa);

            bool inserir = false;

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            if (cliente == null)
            {
                cliente = new Dominio.Entidades.Cliente();
                inserir = true;
            }
            else if (cliente.NaoAtualizarDados)
                return cliente;

            cliente.Bairro = pessoa.Endereco.Bairro.Length > 2 ? pessoa.Endereco.Bairro : "Não Informado";
            cliente.CEP = pessoa.Endereco.CEP;
            cliente.Complemento = pessoa.Endereco.Complemento;
            cliente.CPF_CNPJ = cpfCnpj;
            cliente.Endereco = pessoa.Endereco.Logradouro;

            if (!string.IsNullOrWhiteSpace(pessoa.RGIE))
                cliente.IE_RG = pessoa.RGIE;
            else
                cliente.IE_RG = "ISENTO";

            cliente.InscricaoMunicipal = pessoa.IM;

            cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(pessoa.Endereco.Cidade.IBGE);
            cliente.Nome = pessoa.RazaoSocial;
            cliente.NomeFantasia = pessoa.NomeFantasia;
            cliente.Numero = string.IsNullOrWhiteSpace(pessoa.Endereco.Numero) ? "S/N" : pessoa.Endereco.Numero;
            cliente.Telefone1 = string.IsNullOrWhiteSpace(pessoa.Endereco.Telefone) || pessoa.Endereco.Telefone.StartsWith("00") ? string.Empty : pessoa.Endereco.Telefone;
            cliente.Tipo = cpfCNPJPessoa.Length == 14 ? "J" : "F";

            if (cliente.Atividade == null)
                cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao, 0, unidadeDeTrabalho);

            if (cliente.Tipo == "F" && cliente.Atividade.Codigo == 7 && string.IsNullOrWhiteSpace(cliente.IE_RG))
                cliente.IE_RG = "ISENTO";

            if (inserir)
            {
                if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                {
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                    if (grupoPessoas != null)
                    {
                        cliente.GrupoPessoas = grupoPessoas;
                    }
                }
                if (cliente.IE_RG == "ISENTO")
                    cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte;
                else
                    cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS;
                cliente.Ativo = true;
                cliente.AguardandoConferenciaInformacao = true;
                cliente.DataCadastro = DateTime.Now;
                cliente.DataUltimaAtualizacao = DateTime.Now;
                cliente.Integrado = false;
                repCliente.Inserir(cliente);
            }
            else
            {
                cliente.DataUltimaAtualizacao = DateTime.Now;
                cliente.Integrado = false;
                repCliente.Atualizar(cliente);
            }
            new Repositorio.Embarcador.Pessoas.PessoaIntegracao(unidadeDeTrabalho).GerarIntegracaoPessoa(unidadeDeTrabalho, cliente);

            return cliente;
        }

        public Dominio.Entidades.Cliente ObterDestinatario(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDest infNFeDest, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho, string chaveNotaFiscal = "")
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(infNFeDest.Item));

            bool inserir = false;

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            if (cliente == null)
            {
                cliente = new Dominio.Entidades.Cliente();
                inserir = true;
            }
            else if (cliente.NaoAtualizarDados)
                return cliente;

            cliente.Bairro = infNFeDest.enderDest.xBairro.Length > 2 ? infNFeDest.enderDest.xBairro : "Não Informado";
            cliente.CEP = infNFeDest.enderDest.CEP;
            cliente.Complemento = infNFeDest.enderDest.xCpl;
            cliente.CPF_CNPJ = cpfCnpj;
            cliente.Endereco = infNFeDest.enderDest.xLgr;
            if (infNFeDest.IE != null && infNFeDest.IE != "")
                cliente.IE_RG = infNFeDest.IE;
            else
            {
                cliente.IE_RG = "ISENTO";
            }

            Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(infNFeDest.enderDest.cMun));
            if (localidade == null)
                throw new Exception("Localidade IBGE: " + infNFeDest.enderDest.cMun + " não encontrada.");

            cliente.Localidade = localidade;
            cliente.Nome = Utilidades.String.RemoverCaracteresEspecialSerpro(infNFeDest.xNome?.Replace("&amp;", "")?.Replace(" amp ", ""));
            cliente.Numero = string.IsNullOrWhiteSpace(infNFeDest.enderDest.nro) ? "S/N" : infNFeDest.enderDest.nro;
            cliente.Telefone1 = infNFeDest.enderDest.fone == null || infNFeDest.enderDest.fone.StartsWith("00") ? string.Empty : infNFeDest.enderDest.fone;
            cliente.Tipo = Utilidades.String.OnlyNumbers(infNFeDest.Item).Length == 14 ? "J" : "F";

            if (cliente.Atividade == null)
                cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao, 0, unidadeDeTrabalho);

            if (cliente.Tipo == "F" && cliente.Atividade.Codigo == 7 && string.IsNullOrWhiteSpace(cliente.IE_RG))
                cliente.IE_RG = "ISENTO";

            if (inserir)
            {
                if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                {
                    if (!string.IsNullOrWhiteSpace(cliente.CPF_CNPJ_Formatado))
                    {
                        Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                        if (grupoPessoas != null)
                        {
                            cliente.GrupoPessoas = grupoPessoas;
                        }
                    }
                }
                if (cliente.IE_RG == "ISENTO")
                    cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte;
                else
                    cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS;
                cliente.Ativo = true;
                cliente.AguardandoConferenciaInformacao = true;
                cliente.DataCadastro = System.DateTime.Now;
                cliente.DataUltimaAtualizacao = System.DateTime.Now;
                cliente.Integrado = false;
                repCliente.Inserir(cliente);
            }
            else
            {
                cliente.DataUltimaAtualizacao = System.DateTime.Now;
                cliente.Integrado = false;

                if (_auditado != null && !string.IsNullOrEmpty(chaveNotaFiscal))
                    Auditoria.Auditoria.Auditar(_auditado, cliente, $"Alteração via integração de Nota fiscal chave: {chaveNotaFiscal}", unidadeDeTrabalho);

                repCliente.Atualizar(cliente);
            }

            return cliente;
        }

        public Dominio.Entidades.Cliente ObterDestinatario(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDest infNFeDest, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho, string chaveNotaFiscal = "")
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            if (infNFeDest.Item.ToUpper() != "ESTRANGEIRO")
            {
                double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(infNFeDest.Item));

                bool inserir = false;

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                if (cliente == null)
                {
                    cliente = new Dominio.Entidades.Cliente();
                    inserir = true;
                }
                else if (cliente.NaoAtualizarDados)
                    return cliente;

                cliente.Bairro = infNFeDest.enderDest.xBairro.Length > 2 ? infNFeDest.enderDest.xBairro : "Não Informado";
                cliente.CEP = infNFeDest.enderDest.CEP;
                cliente.Complemento = infNFeDest.enderDest.xCpl;
                cliente.CPF_CNPJ = cpfCnpj;
                cliente.Endereco = infNFeDest.enderDest.xLgr;
                if (infNFeDest.IE != null && infNFeDest.IE != "")
                    cliente.IE_RG = infNFeDest.IE;
                else
                {
                    cliente.IE_RG = "ISENTO";
                }

                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(infNFeDest.enderDest.cMun));
                if (localidade == null)
                    throw new Exception("Localidade IBGE: " + infNFeDest.enderDest.cMun + " não encontrada.");

                cliente.Localidade = localidade;
                cliente.Nome = Utilidades.String.RemoverCaracteresEspecialSerpro(infNFeDest.xNome.Replace("&amp;", "")?.Replace(" amp ", ""));
                cliente.Numero = string.IsNullOrWhiteSpace(infNFeDest.enderDest.nro) ? "S/N" : infNFeDest.enderDest.nro;
                cliente.Telefone1 = infNFeDest.enderDest.fone == null || infNFeDest.enderDest.fone.StartsWith("00") ? string.Empty : infNFeDest.enderDest.fone;
                cliente.Tipo = Utilidades.String.OnlyNumbers(infNFeDest.Item).Length == 14 ? "J" : "F";

                if (cliente.Atividade == null)
                    cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao, 0, unidadeDeTrabalho);

                if (cliente.Tipo == "F" && cliente.Atividade.Codigo == 7 && string.IsNullOrWhiteSpace(cliente.IE_RG))
                    cliente.IE_RG = "ISENTO";

                if (inserir)
                {
                    if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                    {
                        Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho); //new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                        if (grupoPessoas != null)
                        {
                            cliente.GrupoPessoas = grupoPessoas;
                        }
                    }
                    if (cliente.IE_RG == "ISENTO")
                        cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte;
                    else
                        cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS;
                    cliente.Ativo = true;
                    cliente.DataCadastro = System.DateTime.Now;
                    cliente.DataUltimaAtualizacao = System.DateTime.Now;
                    cliente.Integrado = false;
                    repCliente.Inserir(cliente);
                }
                else
                {
                    cliente.DataUltimaAtualizacao = System.DateTime.Now;
                    cliente.Integrado = false;
                    repCliente.Atualizar(cliente);

                    if (_auditado != null && !string.IsNullOrEmpty(chaveNotaFiscal))
                        Auditoria.Auditoria.Auditar(_auditado, cliente, $"Alteração via integração de Nota fiscal chave: {chaveNotaFiscal}", unidadeDeTrabalho);
                }

                return cliente;
            }
            else
            {
                Dominio.Entidades.Cliente cliente = new Dominio.Entidades.Cliente();
                cliente.Bairro = infNFeDest.enderDest.xBairro.Length > 2 ? infNFeDest.enderDest.xBairro : "Não Informado";
                cliente.CEP = infNFeDest.enderDest.CEP;
                cliente.Complemento = infNFeDest.enderDest.xCpl;
                cliente.CPF_CNPJ = 0;
                cliente.DataCadastro = System.DateTime.Now;
                cliente.Endereco = infNFeDest.enderDest.xLgr;
                cliente.IE_RG = infNFeDest.IE;
                cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(infNFeDest.enderDest.cMun));
                cliente.Nome = Utilidades.String.RemoverCaracteresEspecialSerpro(infNFeDest.xNome.Replace("&amp;", "")?.Replace(" amp ", ""));
                cliente.Numero = infNFeDest.enderDest.nro;
                cliente.Telefone1 = infNFeDest.enderDest.fone == null || infNFeDest.enderDest.fone.StartsWith("00") ? string.Empty : infNFeDest.enderDest.fone;
                cliente.Tipo = "E";
                cliente.IE_RG = "ISENTO";
                return cliente;
            }


        }

        public Dominio.Entidades.Cliente ObterDestinatario(dynamic infNFeDest, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho = null, string chaveNotaFiscal = "")
        {
            if (unidadeDeTrabalho == null)
                unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            string cgcDestinatario = (string)infNFeDest.CNPJ;
            if (string.IsNullOrWhiteSpace(cgcDestinatario))
                cgcDestinatario = (string)infNFeDest.CPF;
            if (string.IsNullOrWhiteSpace(cgcDestinatario))
                cgcDestinatario = "00000000000000";

            if (cgcDestinatario != "00000000000000")
            {
                double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(cgcDestinatario));

                bool inserir = false;

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                if (cliente == null)
                {
                    cliente = new Dominio.Entidades.Cliente();
                    inserir = true;
                }
                else if (cliente.NaoAtualizarDados)
                    return cliente;

                cliente.Bairro = ((string)infNFeDest.enderDest.xBairro).Length > 2 ? (string)infNFeDest.enderDest.xBairro : "Não Informado";
                cliente.CEP = (string)infNFeDest.enderDest.CEP;
                cliente.Complemento = (string)infNFeDest.enderDest.xCpl;
                cliente.CPF_CNPJ = cpfCnpj;
                cliente.Endereco = (string)infNFeDest.enderDest.xLgr;
                if (infNFeDest.IE != null && (string)infNFeDest.IE != "")
                    cliente.IE_RG = (string)infNFeDest.IE;
                else
                {
                    cliente.IE_RG = "ISENTO";
                }

                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse((string)infNFeDest.enderDest.cMun));
                if (localidade == null)
                    throw new Exception("Localidade IBGE: " + (string)infNFeDest.enderDest.cMun + " não encontrada.");

                cliente.Localidade = localidade;
                cliente.Nome = (Utilidades.String.RemoverCaracteresEspecialSerpro((string)infNFeDest.xNome).Replace("&amp;", "")?.Replace(" amp ", ""));
                cliente.Numero = string.IsNullOrWhiteSpace((string)infNFeDest.enderDest.nro) ? "S/N" : (string)infNFeDest.enderDest.nro;
                cliente.Telefone1 = infNFeDest.enderDest.fone == null || ((string)infNFeDest.enderDest.fone).StartsWith("00") ? string.Empty : (string)infNFeDest.enderDest.fone;
                cliente.Tipo = Utilidades.String.OnlyNumbers(cgcDestinatario).Length == 14 ? "J" : "F";

                if (cliente.Atividade == null)
                    cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao, 0, unidadeDeTrabalho);

                if (cliente.Tipo == "F" && cliente.Atividade.Codigo == 7 && string.IsNullOrWhiteSpace(cliente.IE_RG))
                    cliente.IE_RG = "ISENTO";

                if (inserir)
                {
                    if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                    {
                        Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                        if (grupoPessoas != null)
                        {
                            cliente.GrupoPessoas = grupoPessoas;
                        }
                    }
                    if (cliente.IE_RG == "ISENTO")
                        cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte;
                    else
                        cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS;
                    cliente.Ativo = true;
                    cliente.AguardandoConferenciaInformacao = true;
                    cliente.DataCadastro = System.DateTime.Now;
                    cliente.DataUltimaAtualizacao = System.DateTime.Now;
                    cliente.Integrado = false;
                    repCliente.Inserir(cliente);
                }
                else
                {
                    cliente.DataUltimaAtualizacao = System.DateTime.Now;
                    cliente.Integrado = false;
                    repCliente.Atualizar(cliente);

                    if (_auditado != null && !string.IsNullOrEmpty(chaveNotaFiscal))
                        Auditoria.Auditoria.Auditar(_auditado, cliente, $"Alteração via integração de Nota fiscal chave: {chaveNotaFiscal}", unidadeDeTrabalho);
                }

                return cliente;
            }
            else
                return null;
        }

        public Dominio.Entidades.Cliente ObterDestinatario(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDest infNFeDest, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho = null, bool importarEmailCliente = false, string chaveNotaFiscal = "")
        {
            if (unidadeDeTrabalho == null)
                unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            if (infNFeDest.Item.ToUpper() != "ESTRANGEIRO")
            {
                double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(infNFeDest.Item));

                bool inserir = false;

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                if (cliente == null)
                {
                    cliente = new Dominio.Entidades.Cliente();
                    inserir = true;
                }
                else if (cliente.NaoAtualizarDados || NaoAtualizarDadosPessoaEmImportacoesDeNotasFiscaisOuIntegracoes)
                    return cliente;

                cliente.Bairro = infNFeDest.enderDest.xBairro.Length > 2 ? infNFeDest.enderDest.xBairro : "Não Informado";
                cliente.CEP = infNFeDest.enderDest.CEP;
                cliente.Complemento = infNFeDest.enderDest.xCpl;
                cliente.CPF_CNPJ = cpfCnpj;
                cliente.Endereco = infNFeDest.enderDest.xLgr;
                if (importarEmailCliente && !string.IsNullOrWhiteSpace(infNFeDest.email))
                    cliente.Email = infNFeDest.email;
                if (infNFeDest.IE != null && infNFeDest.IE != "")
                    cliente.IE_RG = infNFeDest.IE;
                else
                {
                    cliente.IE_RG = "ISENTO";
                }

                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(infNFeDest.enderDest.cMun));
                if (localidade == null)
                {
                    localidade = repLocalidade.BuscarPorDescricaoEUF(infNFeDest.enderDest.xMun, infNFeDest.enderDest.UF.ToString());
                    if (localidade == null)
                        throw new Exception("Localidade IBGE: " + infNFeDest.enderDest.cMun + " não encontrada.");
                }

                cliente.Localidade = localidade;
                cliente.Nome = Utilidades.String.RemoverCaracteresEspecialSerpro(infNFeDest.xNome?.Replace("&amp;", "")?.Replace(" amp ", ""));
                cliente.Numero = string.IsNullOrWhiteSpace(infNFeDest.enderDest.nro) ? "S/N" : infNFeDest.enderDest.nro;
                cliente.Telefone1 = infNFeDest.enderDest.fone == null || infNFeDest.enderDest.fone.StartsWith("00") ? string.Empty : infNFeDest.enderDest.fone;
                cliente.Tipo = Utilidades.String.OnlyNumbers(infNFeDest.Item).Length == 14 ? "J" : "F";

                if (cliente.Atividade == null)
                    cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao, 0, unidadeDeTrabalho);

                if (cliente.Tipo == "F" && cliente.Atividade.Codigo == 7 && string.IsNullOrWhiteSpace(cliente.IE_RG))
                    cliente.IE_RG = "ISENTO";

                if (inserir)
                {
                    if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                    {
                        Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                        if (grupoPessoas != null)
                        {
                            cliente.GrupoPessoas = grupoPessoas;
                        }
                    }
                    if (cliente.IE_RG == "ISENTO")
                        cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte;
                    else
                        cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS;
                    cliente.Ativo = true;
                    cliente.AguardandoConferenciaInformacao = true;
                    cliente.DataCadastro = System.DateTime.Now;
                    cliente.DataUltimaAtualizacao = System.DateTime.Now;
                    cliente.Integrado = false;
                    repCliente.Inserir(cliente);
                }
                else
                {
                    cliente.DataUltimaAtualizacao = System.DateTime.Now;
                    cliente.Integrado = false;
                    repCliente.Atualizar(cliente);

                    if (_auditado != null && !string.IsNullOrEmpty(chaveNotaFiscal))
                        Auditoria.Auditoria.Auditar(_auditado, cliente, $"Alteração via integração de Nota fiscal chave: {chaveNotaFiscal}", unidadeDeTrabalho);
                }

                return cliente;
            }
            else
            {
                Dominio.Entidades.Cliente cliente = new Dominio.Entidades.Cliente();
                cliente.Bairro = infNFeDest.enderDest.xBairro.Length > 2 ? infNFeDest.enderDest.xBairro : "Não Informado";
                cliente.CEP = infNFeDest.enderDest.CEP;
                cliente.Complemento = infNFeDest.enderDest.xCpl;
                cliente.CPF_CNPJ = 0;
                cliente.DataCadastro = System.DateTime.Now;
                cliente.Endereco = infNFeDest.enderDest.xLgr;
                cliente.IE_RG = infNFeDest.IE;
                cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(infNFeDest.enderDest.cMun));
                cliente.Nome = Utilidades.String.RemoverCaracteresEspecialSerpro(infNFeDest.xNome?.Replace("&amp;", "")?.Replace(" amp ", ""));
                cliente.Numero = infNFeDest.enderDest.nro;
                cliente.Telefone1 = infNFeDest.enderDest.fone == null || infNFeDest.enderDest.fone.StartsWith("00") ? string.Empty : infNFeDest.enderDest.fone;
                cliente.Tipo = "E";
                cliente.IE_RG = "ISENTO";
                return cliente;
            }


        }

        public Dominio.Entidades.Cliente ObterLocal(MultiSoftware.NFe.v400.NotaFiscal.TLocal local, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho = null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware = null)
        {
            if (unidadeDeTrabalho == null)
                unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            if (local.Item.ToUpper() != "ESTRANGEIRO")
            {
                double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(local.Item));

                bool inserir = false;

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                if (cliente == null)
                {
                    cliente = new Dominio.Entidades.Cliente();
                    inserir = true;
                }
                else if (cliente.NaoAtualizarDados)
                    return cliente;

                cliente.Bairro = local.xBairro.Length > 2 ? local.xBairro : "Não Informado";
                if (!string.IsNullOrWhiteSpace(local.CEP))
                    cliente.CEP = local.CEP;
                if (!string.IsNullOrWhiteSpace(local.xCpl))
                    cliente.Complemento = local.xCpl;
                cliente.CPF_CNPJ = cpfCnpj;
                cliente.Endereco = local.xLgr;

                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(local.cMun));

                cliente.Localidade = localidade ?? throw new Exception("Localidade IBGE: " + local.cMun + " não encontrada.");
                if (!string.IsNullOrWhiteSpace(local.xNome))
                    cliente.Nome = Utilidades.String.RemoverCaracteresEspecialSerpro(local.xNome?.Replace("&amp;", "")?.Replace(" amp ", ""));
                else if (inserir)
                    cliente.Nome = Utilidades.String.OnlyNumbers(local.Item);
                cliente.Numero = string.IsNullOrWhiteSpace(local.nro) ? "S/N" : local.nro;
                if (!string.IsNullOrWhiteSpace(local.fone))
                    cliente.Telefone1 = local.fone == null || local.fone.StartsWith("00") ? string.Empty : local.fone;
                cliente.Tipo = Utilidades.String.OnlyNumbers(local.Item).Length == 14 ? "J" : "F";

                if (inserir)
                {
                    if (cliente.Atividade == null)
                        cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao, 0, unidadeDeTrabalho);

                    cliente.IE_RG = "ISENTO";
                    cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte;

                    if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                    {
                        Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);

                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));

                        if (grupoPessoas != null)
                            cliente.GrupoPessoas = grupoPessoas;
                    }

                    cliente.Ativo = true;
                    cliente.AguardandoConferenciaInformacao = true;
                    cliente.DataCadastro = DateTime.Now;
                    cliente.DataUltimaAtualizacao = DateTime.Now;
                    cliente.Integrado = false;
                    repCliente.Inserir(cliente);
                }
                else
                {
                    cliente.DataUltimaAtualizacao = DateTime.Now;
                    cliente.Integrado = false;
                    repCliente.Atualizar(cliente);
                }

                return cliente;
            }
            else
            {
                Dominio.Entidades.Cliente cliente = new Dominio.Entidades.Cliente();
                cliente.Bairro = local.xBairro.Length > 2 ? local.xBairro : "Não Informado";
                cliente.CEP = local.CEP;
                cliente.Complemento = local.xCpl;
                cliente.CPF_CNPJ = 0;
                cliente.DataCadastro = DateTime.Now;
                cliente.Endereco = local.xLgr;
                cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(local.cMun));
                cliente.Nome = Utilidades.String.RemoverCaracteresEspecialSerpro(local.xNome?.Replace("&amp;", "")?.Replace(" amp ", ""));
                cliente.Numero = local.nro;
                cliente.Telefone1 = local.fone == null || local.fone.StartsWith("00") ? string.Empty : local.fone;
                cliente.Tipo = "E";
                cliente.IE_RG = "ISENTO";
                return cliente;
            }
        }

        public Dominio.Enumeradores.TipoPagamento ObterFormaPagamento(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null)
            {
                switch (infNFeTransp.modFrete)
                {
                    case MultiSoftware.NFe.NotaFiscal.TNFeInfNFeTranspModFrete.Item0:
                        return Dominio.Enumeradores.TipoPagamento.Pago;
                    case MultiSoftware.NFe.NotaFiscal.TNFeInfNFeTranspModFrete.Item1:
                        return Dominio.Enumeradores.TipoPagamento.A_Pagar;
                    case MultiSoftware.NFe.NotaFiscal.TNFeInfNFeTranspModFrete.Item2:
                        return Dominio.Enumeradores.TipoPagamento.Outros;
                    case MultiSoftware.NFe.NotaFiscal.TNFeInfNFeTranspModFrete.Item9:
                        return Dominio.Enumeradores.TipoPagamento.Outros;
                    default:
                        return Dominio.Enumeradores.TipoPagamento.Pago;
                }
            }
            return Dominio.Enumeradores.TipoPagamento.Pago;
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete ObterFormaPagamentoModalidade(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null)
            {
                switch (infNFeTransp.modFrete)
                {
                    case MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTranspModFrete.Item0:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;
                    case MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTranspModFrete.Item1:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar;
                    case MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTranspModFrete.Item2:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Outros;
                    case MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTranspModFrete.Item9:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido;
                    default:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido;
                }
            }

            return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido;
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete ObterFormaPagamentoModalidade(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null)
            {
                switch (infNFeTransp.modFrete)
                {
                    case MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTranspModFrete.Item0:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;
                    case MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTranspModFrete.Item1:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar;
                    case MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTranspModFrete.Item2:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Outros;
                    case MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTranspModFrete.Item9:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido;
                    case MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTranspModFrete.Item3:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;
                    case MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTranspModFrete.Item4:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar;
                    default:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido;
                }
            }

            return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido;
        }

        public Dominio.Enumeradores.TipoPagamento ObterFormaPagamento(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null)
            {
                switch (infNFeTransp.modFrete)
                {
                    case MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTranspModFrete.Item0:
                        return Dominio.Enumeradores.TipoPagamento.Pago;
                    case MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTranspModFrete.Item1:
                        return Dominio.Enumeradores.TipoPagamento.A_Pagar;
                    case MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTranspModFrete.Item2:
                        return Dominio.Enumeradores.TipoPagamento.Outros;
                    case MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTranspModFrete.Item9:
                        return Dominio.Enumeradores.TipoPagamento.Outros;
                    default:
                        return Dominio.Enumeradores.TipoPagamento.Pago;
                }
            }

            return Dominio.Enumeradores.TipoPagamento.Pago;
        }

        public Dominio.Enumeradores.TipoPagamento ObterFormaPagamento(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null)
            {
                switch (infNFeTransp.modFrete)
                {
                    case MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTranspModFrete.Item0:
                        return Dominio.Enumeradores.TipoPagamento.Pago;
                    case MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTranspModFrete.Item1:
                        return Dominio.Enumeradores.TipoPagamento.A_Pagar;
                    case MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTranspModFrete.Item2:
                        return Dominio.Enumeradores.TipoPagamento.Outros;
                    case MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTranspModFrete.Item9:
                        return Dominio.Enumeradores.TipoPagamento.Outros;
                    case MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTranspModFrete.Item3:
                        return Dominio.Enumeradores.TipoPagamento.Pago;
                    case MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTranspModFrete.Item4:
                        return Dominio.Enumeradores.TipoPagamento.A_Pagar;
                    default:
                        return Dominio.Enumeradores.TipoPagamento.Pago;
                }
            }

            return Dominio.Enumeradores.TipoPagamento.Pago;
        }

        public Dominio.Enumeradores.TipoTomador ObterTipoTomador(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            switch (infNFeTransp.modFrete)
            {
                case MultiSoftware.NFe.NotaFiscal.TNFeInfNFeTranspModFrete.Item0:
                    return Dominio.Enumeradores.TipoTomador.Remetente;
                default:
                    return Dominio.Enumeradores.TipoTomador.Destinatario;
            }
        }

        public Dominio.Enumeradores.TipoTomador ObterTipoTomador(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            switch (infNFeTransp.modFrete)
            {
                case MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTranspModFrete.Item0:
                    return Dominio.Enumeradores.TipoTomador.Remetente;
                default:
                    return Dominio.Enumeradores.TipoTomador.Destinatario;
            }
        }

        public Dominio.Enumeradores.TipoTomador ObterTipoTomador(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            switch (infNFeTransp.modFrete)
            {
                case MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTranspModFrete.Item0:
                    return Dominio.Enumeradores.TipoTomador.Remetente;
                default:
                    return Dominio.Enumeradores.TipoTomador.Destinatario;
            }
        }

        public decimal ObterQuantidadeVolumes(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null)
            {
                if (infNFeTransp.vol != null)
                {
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                    int volume = 0;
                    foreach (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeTranspVol vol in infNFeTransp.vol)
                        volume += !string.IsNullOrEmpty(vol.qVol) ? int.Parse(vol.qVol, cultura) : 0;

                    return volume;
                }
            }
            return 0;
        }

        public int ObterQuantidadeVolumes(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null)
            {
                if (infNFeTransp.vol != null)
                {
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                    int volume = 0;
                    foreach (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTranspVol vol in infNFeTransp.vol)
                        volume += !string.IsNullOrEmpty(vol.qVol) ? int.Parse(vol.qVol, cultura) : 0;

                    return volume;
                }
            }
            return 0;
        }

        public int ObterQuantidadeVolumes(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null)
            {
                if (infNFeTransp.vol != null)
                {
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                    int volume = 0;
                    foreach (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTranspVol vol in infNFeTransp.vol)
                        volume += !string.IsNullOrEmpty(vol.qVol) ? int.Parse(vol.qVol, cultura) : 0;

                    return volume;
                }
            }
            return 0;
        }

        public decimal ObterPesoLiquido(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null)
            {
                if (infNFeTransp.vol != null)
                {
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                    decimal peso = 0;
                    foreach (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeTranspVol vol in infNFeTransp.vol)
                        peso += !string.IsNullOrEmpty(vol.pesoL) ? decimal.Parse(vol.pesoL, cultura) : 0;
                    return peso;
                }
            }
            return 0;
        }

        public decimal ObterPesoLiquido(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null)
            {
                if (infNFeTransp.vol != null)
                {
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                    decimal peso = 0;
                    foreach (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTranspVol vol in infNFeTransp.vol)
                        peso += !string.IsNullOrEmpty(vol.pesoL) ? decimal.Parse(vol.pesoL, cultura) : 0;
                    return peso;
                }
            }
            return 0;
        }

        public decimal ObterPesoLiquido(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null)
            {
                if (infNFeTransp.vol != null)
                {
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                    decimal peso = 0;

                    foreach (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTranspVol vol in infNFeTransp.vol)
                        peso += !string.IsNullOrEmpty(vol.pesoL) ? decimal.Parse(vol.pesoL, cultura) : 0;

                    return peso;
                }
            }
            return 0;
        }

        public decimal ObterPeso(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null)
            {
                if (infNFeTransp.vol != null)
                {
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                    decimal peso = 0;
                    foreach (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeTranspVol vol in infNFeTransp.vol)
                        peso += !string.IsNullOrEmpty(vol.pesoB) ? decimal.Parse(vol.pesoB, cultura) : vol.pesoL != null ? decimal.Parse(vol.pesoL, cultura) : 0;
                    return peso;
                }
            }
            return 0;
        }

        public decimal ObterPeso(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTransp infNFeTransp, Repositorio.UnitOfWork unitOfWork, int codigoEmpresa = 0)
        {
            if (infNFeTransp != null)
            {
                if (infNFeTransp.vol != null)
                {
                    bool utilizarPesoLiquido = false;
                    if (codigoEmpresa != 0)
                    {
                        Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                        Dominio.Entidades.Empresa empresa = null;
                        empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                        utilizarPesoLiquido = empresa.Configuracao != null && empresa.Configuracao.TipoPesoNFe == Dominio.Enumeradores.TipoPesoNFe.Liquido;
                    }

                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                    decimal peso = 0;

                    foreach (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTranspVol vol in infNFeTransp.vol)
                    {
                        if (utilizarPesoLiquido)
                            peso += !string.IsNullOrEmpty(vol.pesoL) ? decimal.Parse(vol.pesoL, cultura) : !string.IsNullOrEmpty(vol.pesoB) ? decimal.Parse(vol.pesoB, cultura) : 0;
                        else
                            peso += !string.IsNullOrEmpty(vol.pesoB) ? decimal.Parse(vol.pesoB, cultura) : !string.IsNullOrEmpty(vol.pesoL) ? decimal.Parse(vol.pesoL, cultura) : 0;
                    }

                    return peso;
                }
            }
            return 0;
        }

        public decimal ObterPeso(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTransp infNFeTransp, int codigoEmpresa = 0, Repositorio.UnitOfWork unitOfWork = null, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null)
        {
            if (unitOfWork == null)
                unitOfWork = new Repositorio.UnitOfWork(StringConexao);

            if (infNFeTransp != null)
            {
                if (infNFeTransp.vol != null)
                {
                    bool utilizarPesoLiquido = false;
                    if (codigoEmpresa != 0)
                    {

                        if (cargaPedido != null)
                        {
                            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPesoConsideradoCarga repConfiguracaoTipoOperacaoPesoConsideradoCarga = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPesoConsideradoCarga(unitOfWork);
                            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPesoConsideradoCarga configuracaoTipoOperacaoPesoConsideradoCarga = repConfiguracaoTipoOperacaoPesoConsideradoCarga.BuscarPorTipoOperacao(cargaPedido.Carga.TipoOperacao);

                            utilizarPesoLiquido = configuracaoTipoOperacaoPesoConsideradoCarga?.PesoConsideradoNaCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumPesoConsideradoCarga.PesoLiquido;
                        }
                        else
                        {
                            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                            utilizarPesoLiquido = empresa.Configuracao != null && empresa.Configuracao.TipoPesoNFe == Dominio.Enumeradores.TipoPesoNFe.Liquido;
                        }

                    }

                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                    decimal peso = 0;

                    foreach (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTranspVol vol in infNFeTransp.vol)
                    {
                        if (utilizarPesoLiquido)
                            peso += !string.IsNullOrEmpty(vol.pesoL) ? decimal.Parse(vol.pesoL, cultura) : !string.IsNullOrWhiteSpace(vol.pesoB) ? decimal.Parse(vol.pesoB, cultura) : 0;
                        else
                            peso += !string.IsNullOrEmpty(vol.pesoB) ? decimal.Parse(vol.pesoB, cultura) : !string.IsNullOrEmpty(vol.pesoL) ? decimal.Parse(vol.pesoL, cultura) : 0;
                    }

                    return peso;
                }
            }
            return 0;
        }

        public System.IO.MemoryStream ObterLoteDeXML(List<int> codigos, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos repNotaFiscalArquivos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos(unitOfWork);

            MemoryStream fZip = new MemoryStream();
            ZipOutputStream zipOStream = new ZipOutputStream(fZip);
            zipOStream.SetLevel(9);

            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos> xmls = repNotaFiscalArquivos.BuscarPorNotas(codigos, codigoEmpresa);

            foreach (Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos xml in xmls)
            {
                if (!string.IsNullOrWhiteSpace(xml.XMLDistribuicao))
                {
                    byte[] arquivo = System.Text.Encoding.UTF8.GetBytes(xml.XMLDistribuicao);
                    ZipEntry entry = new ZipEntry(string.Concat("NFe", xml.NotaFiscal.Chave, ".xml"));
                    entry.DateTime = DateTime.Now;
                    zipOStream.PutNextEntry(entry);
                    zipOStream.Write(arquivo, 0, arquivo.Length);
                    zipOStream.CloseEntry();
                }
                if (!string.IsNullOrWhiteSpace(xml.XMLCancelamento))
                {
                    byte[] arquivo = System.Text.Encoding.UTF8.GetBytes(xml.XMLCancelamento);
                    ZipEntry entry = new ZipEntry(string.Concat("NFe", xml.NotaFiscal.Chave, "-procCancNFe.xml"));
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

        public System.IO.MemoryStream ObterLoteDeRemessa(List<int> codigos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.BoletoRemessa repBoletoRemessa = new Repositorio.Embarcador.Financeiro.BoletoRemessa(unitOfWork);

            MemoryStream fZip = new MemoryStream();
            ZipOutputStream zipOStream = new ZipOutputStream(fZip);
            zipOStream.SetLevel(9);

            List<Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa> remessas = repBoletoRemessa.BuscarPorNotas(codigos);

            foreach (Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa remessa in remessas)
            {
                byte[] arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(remessa.Observacao);
                ZipEntry entry = new ZipEntry(System.IO.Path.GetFileName(remessa.Observacao));
                entry.DateTime = DateTime.Now;
                zipOStream.PutNextEntry(entry);
                zipOStream.Write(arquivo, 0, arquivo.Length);
                zipOStream.CloseEntry();
            }

            remessas = null;

            zipOStream.IsStreamOwner = false;
            zipOStream.Close();

            fZip.Position = 0;

            return fZip;
        }

        public System.IO.MemoryStream ObterLoteDeBoleto(List<int> codigos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

            MemoryStream fZip = new MemoryStream();
            ZipOutputStream zipOStream = new ZipOutputStream(fZip);
            zipOStream.SetLevel(9);

            for (int i = 0; i < codigos.Count(); i++)
            {
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigos[i]);

                byte[] arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(titulo.CaminhoBoleto);
                ZipEntry entry = new ZipEntry(System.IO.Path.GetFileName(titulo.CaminhoBoleto));
                entry.DateTime = DateTime.Now;
                zipOStream.PutNextEntry(entry);
                zipOStream.Write(arquivo, 0, arquivo.Length);
                zipOStream.CloseEntry();
            }

            zipOStream.IsStreamOwner = false;
            zipOStream.Close();

            fZip.Position = 0;

            return fZip;
        }

        #endregion

        #region Métodos Privados

        private object ObterDocumentoPorXML(MultiSoftware.NFe.v400.NotaFiscal.TNFe nfe, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);

            List<string> cfops = new List<string>();
            List<string> ncms = new List<string>();
            string cfopPrincipal = "";
            string ncmPrincipal = "";

            if (nfe.infNFe.det != null)
            {
                foreach (var prod in nfe.infNFe.det)
                {
                    if (prod.prod.CFOP != null && !string.IsNullOrWhiteSpace(prod.prod.CFOP))
                        cfops.Add(prod.prod.CFOP);
                    if (prod.prod.NCM != null && !string.IsNullOrEmpty((string)prod.prod.NCM))
                        ncms.Add((string)prod.prod.NCM);
                }

                if (cfops != null && cfops.Count > 0)
                    cfopPrincipal = RetornaRegistroComMaiorQuantidade(cfops);

                if (ncms != null && ncms.Count > 0)
                {
                    ncmPrincipal = RetornaRegistroComMaiorQuantidade(ncms);

                    if (ncmPrincipal.Length > 4)
                        ncmPrincipal = ncmPrincipal.Substring(0, 4);
                }
            }

            string chaveNotaFiscal = Utilidades.String.OnlyNumbers(nfe.infNFe.Id);

            var notaRetorno = new
            {
                Chave = chaveNotaFiscal,
                ValorTotal = decimal.Parse(nfe.infNFe.total.ICMSTot.vNF, cultura),
                DataEmissao = DateTime.ParseExact(nfe.infNFe.ide.dhEmi.Substring(0, 19), "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None).ToString("dd/MM/yyyy HH:mm:ss"),
                Remetente = this.ObterCNPJEmitente(nfe.infNFe.emit, codigoEmpresa),
                RemetenteUF = this.ObterClienteEmitente(nfe.infNFe.emit, codigoEmpresa)?.Localidade?.Estado?.Sigla ?? string.Empty,
                Destinatario = !string.IsNullOrWhiteSpace(nfe.infNFe.dest.Item) ? this.ObterCPFCNPJDestinatario(nfe.infNFe.dest, codigoEmpresa, null, false, chaveNotaFiscal) : null,
                DestinatarioUF = !string.IsNullOrWhiteSpace(nfe.infNFe.dest.Item) ? this.ObterClienteDestinatario(nfe.infNFe.dest, codigoEmpresa, chaveNotaFiscal)?.Localidade?.Estado?.Sigla ?? string.Empty : null,
                DestinatarioExportacao = string.IsNullOrWhiteSpace(nfe.infNFe.dest.Item) ? this.ObterDadosDestinatarioExportacao(nfe.infNFe.dest) : null,
                Numero = nfe.infNFe.ide.nNF,
                Peso = this.ObterPeso(nfe.infNFe.transp),
                UnidadeMedida = this.ObterUnidadeMedida(unitOfWork),
                FormaPagamento = this.ObterFormaPagamento(nfe.infNFe.transp),
                Placa = this.ObterPlacaVeiculo(nfe.infNFe.transp),
                NumeroDosCTesUtilizados = repDocumentosCTe.BuscarNumeroStatusDoCTePorChaveEEmpresa(codigoEmpresa, chaveNotaFiscal),//repDocumentosCTe.BuscarNumeroDoCTePorChaveEEmpresa(codigoEmpresa, nfe.protNFe.infProt.chNFe),
                Serie = this.ObterSerie(nfe.infNFe.emit, codigoEmpresa, unitOfWork),
                Observacao = this.ObterObservacao(nfe.infNFe.infAdic),
                XPedido = ObterXPed(nfe.infNFe.compra),
                Volume = this.ObterQuantidadeVolumes(nfe.infNFe.transp),
                CFOP = cfopPrincipal,
                NumeroReferenciaEDI = "",
                PINSuframa = "",
                NCMPredominante = ncmPrincipal,
                NumeroControleCliente = "",
                UnidadeMedidaVolumes = this.ObterUnidadeMedidaUN(unitOfWork),
            };

            return notaRetorno;
        }

        private object ObterDocumentoPorXML(MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc nfe, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);

            List<string> cfops = new List<string>();
            List<string> ncms = new List<string>();
            string cfopPrincipal = "";
            string ncmPrincipal = "";

            if (nfe.NFe.infNFe.det != null)
            {
                foreach (var prod in nfe.NFe.infNFe.det)
                {
                    if (prod.prod.CFOP != null && (int)prod.prod.CFOP > 0)
                        cfops.Add(Convert.ToString((int)prod.prod.CFOP));
                    if (prod.prod.NCM != null && !string.IsNullOrEmpty((string)prod.prod.NCM))
                        ncms.Add((string)prod.prod.NCM);
                }

                if (cfops != null && cfops.Count > 0)
                    cfopPrincipal = RetornaRegistroComMaiorQuantidade(cfops);

                if (ncms != null && ncms.Count > 0)
                {
                    ncmPrincipal = RetornaRegistroComMaiorQuantidade(ncms);

                    if (ncmPrincipal.Length > 4)
                        ncmPrincipal = ncmPrincipal.Substring(0, 4);
                }
            }

            var notaRetorno = new
            {
                Chave = nfe.protNFe.infProt.chNFe,
                ValorTotal = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura),
                DataEmissao = DateTime.ParseExact(nfe.NFe.infNFe.ide.dEmi, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None).ToString("dd/MM/yyyy"),
                Remetente = this.ObterCNPJEmitente(nfe.NFe.infNFe.emit, codigoEmpresa),
                RemetenteUF = this.ObterClienteEmitente(nfe.NFe.infNFe.emit, codigoEmpresa)?.Localidade?.Estado?.Sigla ?? string.Empty,
                Destinatario = !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.dest.Item) ? this.ObterCPFCNPJDestinatario(nfe.NFe.infNFe.dest, codigoEmpresa, null, nfe.protNFe.infProt.chNFe) : null,
                DestinatarioUF = !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.dest.Item) ? this.ObterClienteDestinatario(nfe.NFe.infNFe.dest, codigoEmpresa, nfe.protNFe.infProt.chNFe)?.Localidade?.Estado?.Sigla ?? string.Empty : null,
                DestinatarioExportacao = string.IsNullOrWhiteSpace(nfe.NFe.infNFe.dest.Item) ? this.ObterDadosDestinatarioExportacao(nfe.NFe.infNFe.dest, unitOfWork) : null,
                Numero = nfe.NFe.infNFe.ide.nNF,
                Peso = this.ObterPeso(nfe.NFe.infNFe.transp),
                UnidadeMedida = this.ObterUnidadeMedida(unitOfWork),
                FormaPagamento = this.ObterFormaPagamento(nfe.NFe.infNFe.transp),
                Placa = this.ObterPlacaVeiculo(nfe.NFe.infNFe.transp),
                NumeroDosCTesUtilizados = repDocumentosCTe.BuscarNumeroStatusDoCTePorChaveEEmpresa(codigoEmpresa, nfe.protNFe.infProt.chNFe),//repDocumentosCTe.BuscarNumeroDoCTePorChaveEEmpresa(codigoEmpresa, nfe.protNFe.infProt.chNFe),
                Serie = this.ObterSerie(nfe.NFe.infNFe.emit, codigoEmpresa, unitOfWork),
                Observacao = this.ObterObservacao(nfe.NFe.infNFe.infAdic),
                XPedido = ObterXPed(nfe.NFe.infNFe.compra),
                Volume = this.ObterQuantidadeVolumes(nfe.NFe.infNFe.transp),
                CFOP = cfopPrincipal,
                NumeroReferenciaEDI = "",
                PINSuframa = "",
                NCMPredominante = ncmPrincipal,
                NumeroControleCliente = "",
                UnidadeMedidaVolumes = this.ObterUnidadeMedidaUN(unitOfWork),
            };

            return notaRetorno;
        }

        private object ObterDocumentoPorXML(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc nfe, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            List<string> cfops = new List<string>();
            List<string> ncms = new List<string>();
            string cfopPrincipal = "";
            string ncmPrincipal = "";

            if (nfe.NFe.infNFe.det != null)
            {
                foreach (var prod in nfe.NFe.infNFe.det)
                {
                    if (!string.IsNullOrWhiteSpace(prod.prod.CFOP))
                        cfops.Add(prod.prod.CFOP);
                    if (prod.prod.NCM != null && !string.IsNullOrEmpty((string)prod.prod.NCM))
                        ncms.Add((string)prod.prod.NCM);
                }

                if (cfops != null && cfops.Count > 0)
                    cfopPrincipal = RetornaRegistroComMaiorQuantidade(cfops);

                if (ncms != null && ncms.Count > 0)
                {
                    ncmPrincipal = RetornaRegistroComMaiorQuantidade(ncms);

                    if (ncmPrincipal.Length > 4)
                        ncmPrincipal = ncmPrincipal.Substring(0, 4);
                }
            }


            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);

            var notaRetorno = new
            {
                Chave = nfe.protNFe.infProt.chNFe,
                ValorTotal = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura),
                DataEmissao = DateTime.ParseExact(nfe.NFe.infNFe.ide.dhEmi.Substring(0, 19), "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None).ToString("dd/MM/yyyy HH:mm:ss"),
                Remetente = this.ObterCNPJEmitente(nfe.NFe.infNFe.emit, codigoEmpresa),
                RemetenteUF = this.ObterClienteEmitente(nfe.NFe.infNFe.emit, codigoEmpresa)?.Localidade?.Estado?.Sigla ?? string.Empty,
                Destinatario = !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.dest.Item) ? this.ObterCPFCNPJDestinatario(nfe.NFe.infNFe.dest, codigoEmpresa, null, nfe.protNFe.infProt.chNFe) : null,
                DestinatarioUF = !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.dest.Item) ? this.ObterClienteDestinatario(nfe.NFe.infNFe.dest, codigoEmpresa, nfe.protNFe.infProt.chNFe)?.Localidade?.Estado?.Sigla ?? string.Empty : null,
                DestinatarioExportacao = string.IsNullOrWhiteSpace(nfe.NFe.infNFe.dest.Item) ? this.ObterDadosDestinatarioExportacao(nfe.NFe.infNFe.dest) : null,
                Numero = nfe.NFe.infNFe.ide.nNF,
                Peso = this.ObterPeso(nfe.NFe.infNFe.transp, unitOfWork, codigoEmpresa),
                UnidadeMedida = this.ObterUnidadeMedida(unitOfWork),
                FormaPagamento = this.ObterFormaPagamento(nfe.NFe.infNFe.transp),
                Placa = this.ObterPlacaVeiculo(nfe.NFe.infNFe.transp),
                NumeroDosCTesUtilizados = repDocumentosCTe.BuscarNumeroStatusDoCTePorChaveEEmpresa(codigoEmpresa, nfe.protNFe.infProt.chNFe),//repDocumentosCTe.BuscarNumeroDoCTePorChaveEEmpresa(codigoEmpresa, nfe.protNFe.infProt.chNFe),
                Serie = this.ObterSerie(nfe.NFe.infNFe.emit, codigoEmpresa, unitOfWork),
                Observacao = this.ObterObservacao(nfe.NFe.infNFe.infAdic),
                XPedido = ObterXPed(nfe.NFe.infNFe.compra),
                Volume = this.ObterQuantidadeVolumes(nfe.NFe.infNFe.transp),
                CFOP = cfopPrincipal,
                NumeroReferenciaEDI = "",
                PINSuframa = "",
                NCMPredominante = ncmPrincipal,
                NumeroControleCliente = "",
                UnidadeMedidaVolumes = this.ObterUnidadeMedidaUN(unitOfWork),
            };

            return notaRetorno;
        }

        private object ObterDocumentoPorXML(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc nfe, int codigoEmpresa, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);

            bool naoCopiarTomadorCTeAnterior = usuario == null ? false : usuario.Empresa.Configuracao != null ? usuario.Empresa.Configuracao.NaoCarregarTomadorCTes : false;

            List<string> cfops = new List<string>();
            List<string> ncms = new List<string>();
            string cfopPrincipal = "";
            string ncmPrincipal = "";

            if (nfe.NFe.infNFe.det != null)
            {
                foreach (var prod in nfe.NFe.infNFe.det)
                {
                    if (!string.IsNullOrWhiteSpace(prod.prod.CFOP))
                        cfops.Add(prod.prod.CFOP);
                    if (prod.prod.NCM != null && !string.IsNullOrEmpty((string)prod.prod.NCM))
                        ncms.Add((string)prod.prod.NCM);
                }

                if (cfops != null && cfops.Count > 0)
                    cfopPrincipal = RetornaRegistroComMaiorQuantidade(cfops);

                if (ncms != null && ncms.Count > 0)
                {
                    ncmPrincipal = RetornaRegistroComMaiorQuantidade(ncms);

                    if (ncmPrincipal.Length > 4)
                        ncmPrincipal = ncmPrincipal.Substring(0, 4);
                }
            }


            var notaRetorno = new
            {
                Chave = nfe.protNFe.infProt.chNFe,
                ValorTotal = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura),
                DataEmissao = DateTime.ParseExact(nfe.NFe.infNFe.ide.dhEmi.Substring(0, 19), "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None).ToString("dd/MM/yyyy HH:mm:ss"),
                Remetente = this.ObterCNPJEmitente(nfe.NFe.infNFe.emit, codigoEmpresa),
                RemetenteUF = this.ObterClienteEmitente(nfe.NFe.infNFe.emit, codigoEmpresa)?.Localidade?.Estado?.Sigla ?? string.Empty,
                Destinatario = !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.dest.Item) ? this.ObterCPFCNPJDestinatario(nfe.NFe.infNFe.dest, codigoEmpresa, null, false, nfe.protNFe.infProt.chNFe) : null,
                DestinatarioUF = !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.dest.Item) ? this.ObterClienteDestinatario(nfe.NFe.infNFe.dest, codigoEmpresa, nfe.protNFe.infProt.chNFe)?.Localidade?.Estado?.Sigla ?? string.Empty : null,
                DestinatarioExportacao = string.IsNullOrWhiteSpace(nfe.NFe.infNFe.dest.Item) ? this.ObterDadosDestinatarioExportacao(nfe.NFe.infNFe.dest) : null,
                Numero = nfe.NFe.infNFe.ide.nNF,
                Peso = this.ObterPeso(nfe.NFe.infNFe.transp, codigoEmpresa),
                UnidadeMedida = this.ObterUnidadeMedida(unitOfWork),
                FormaPagamento = naoCopiarTomadorCTeAnterior ? null : this.ObterFormaPagamento(nfe.NFe.infNFe.transp).ToString(),
                Placa = this.ObterPlacaVeiculo(nfe.NFe.infNFe.transp),
                NumeroDosCTesUtilizados = repDocumentosCTe.BuscarNumeroStatusDoCTePorChaveEEmpresa(codigoEmpresa, nfe.protNFe.infProt.chNFe), //repDocumentosCTe.BuscarNumeroDoCTePorChaveEEmpresa(codigoEmpresa, nfe.protNFe.infProt.chNFe),
                Serie = this.ObterSerie(nfe.NFe.infNFe.emit, codigoEmpresa, unitOfWork),
                Observacao = this.ObterObservacao(nfe.NFe.infNFe.infAdic),
                XPedido = this.ObterXPed(nfe.NFe.infNFe.compra),
                Volume = this.ObterQuantidadeVolumes(nfe.NFe.infNFe.transp),
                CFOP = cfopPrincipal,
                NumeroReferenciaEDI = "",
                PINSuframa = "",
                NCMPredominante = ncmPrincipal,
                NumeroControleCliente = "",
                UnidadeMedidaVolumes = this.ObterUnidadeMedidaUN(unitOfWork),
            };

            return notaRetorno;
        }
        
        public string RetornaRegistroComMaiorQuantidade(List<string> lista)
        {
            var nameGroup = lista.GroupBy(x => x);
            var maxCount = nameGroup.Max(g => g.Count());
            return nameGroup.Where(x => x.Count() == maxCount).Select(x => x.Key).FirstOrDefault();
        }
        
        private Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe ObterDocumentoPorXML(MultiSoftware.NFe.v400.NotaFiscal.TNFe nfe, Repositorio.UnitOfWork unitOfWork, bool importarEmailCliente = false)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa empresa = null;
            string transportadorOrigem = "";
            if (nfe.infNFe.transp != null)
            {
                if (nfe.infNFe.transp.transporta != null)
                {
                    transportadorOrigem = nfe.infNFe.transp.transporta.Item;
                    empresa = repEmpresa.BuscarPorCNPJ(nfe.infNFe.transp.transporta.Item);
                }
            }

            bool utilizarNumeroFatura = double.TryParse(string.IsNullOrWhiteSpace(nfe.infNFe.cobr?.fat?.nFat) ? string.Empty : Utilidades.String.OnlyNumbers(nfe.infNFe.cobr?.fat?.nFat), out double numeroFatura);


            var notaRetorno = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe
            {
                Chave = Utilidades.String.OnlyNumbers(nfe.infNFe.Id),
                ValorTotal = decimal.Parse(nfe.infNFe.total.ICMSTot.vNF, cultura),
                BaseCalculoICMS = decimal.Parse(nfe.infNFe.total.ICMSTot.vBC, cultura),
                ValorICMS = decimal.Parse(nfe.infNFe.total.ICMSTot.vICMS, cultura),
                BaseCalculoST = decimal.Parse(nfe.infNFe.total.ICMSTot.vBCST, cultura),
                ValorST = decimal.Parse(nfe.infNFe.total.ICMSTot.vST, cultura),
                ValorTotalProdutos = decimal.Parse(nfe.infNFe.total.ICMSTot.vProd, cultura),
                ValorSeguro = decimal.Parse(nfe.infNFe.total.ICMSTot.vSeg, cultura),
                ValorDesconto = decimal.Parse(nfe.infNFe.total.ICMSTot.vDesc, cultura),
                ValorImpostoImportacao = decimal.Parse(nfe.infNFe.total.ICMSTot.vII, cultura),
                ValorIPI = decimal.Parse(nfe.infNFe.total.ICMSTot.vIPI, cultura),
                ValorPIS = decimal.Parse(nfe.infNFe.total.ICMSTot.vPIS, cultura),
                ValorCOFINS = decimal.Parse(nfe.infNFe.total.ICMSTot.vCOFINS, cultura),
                ValorOutros = decimal.Parse(nfe.infNFe.total.ICMSTot.vOutro, cultura),
                ValorFrete = decimal.Parse(nfe.infNFe.total.ICMSTot.vFrete, cultura),
                NaturezaOP = nfe.infNFe.ide.natOp,
                TipoOperacao = nfe.infNFe.ide.tpNF == MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeIdeTpNF.Item0 ? 0 : 1,
                Empresa = empresa != null ? empresa.CNPJ_Formatado : "",
                CNPJTransportador = transportadorOrigem,
                DataEmissao = DateTime.ParseExact(nfe.infNFe.ide.dhEmi.Substring(0, 19), "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None).ToString("dd/MM/yyyy HH:mm:ss"),
                Remetente = this.ObterEmitente(nfe.infNFe.emit, empresa?.Codigo ?? 0),
                Destinatario = !string.IsNullOrWhiteSpace(nfe.infNFe.dest.Item) ? this.ObterCPFCNPJDestinatario(nfe.infNFe.dest, empresa?.Codigo ?? 0, null, importarEmailCliente, Utilidades.String.OnlyNumbers(nfe.infNFe.Id)) : null,
                DestinatarioExportacao = string.IsNullOrWhiteSpace(nfe.infNFe.dest.Item) ? this.ObterDadosDestinatarioExportacao(nfe.infNFe.dest) : null,
                Numero = nfe.infNFe.ide.nNF,
                Modelo = nfe.infNFe.ide.mod.ToString().Replace("Item", ""),
                Serie = nfe.infNFe.ide.serie,
                Peso = this.ObterPeso(nfe.infNFe.transp),
                PesoLiquido = this.ObterPesoLiquido(nfe.infNFe.transp),
                FormaPagamento = this.ObterFormaPagamento(nfe.infNFe.transp),
                Produtos = this.ObterProdutos(nfe.infNFe.det),
                Placa = this.ObterPlacaVeiculo(nfe.infNFe.transp),
                NumeroDosCTesUtilizados = empresa != null ? repDocumentosCTe.BuscarNumeroStatusDoCTePorChaveEEmpresa(empresa.Codigo, Utilidades.String.OnlyNumbers(nfe.infNFe.Id)) : null,
                Observacao = this.ObterObservacao(nfe.infNFe.infAdic),
                ObservacaoContribuinte = this.ObterObservacaoContribuinte(nfe.infNFe.infAdic),
                XPedido = ObterXPed(nfe.infNFe.compra),
                Volume = this.ObterQuantidadeVolumes(nfe.infNFe.transp),
                Especie = nfe.infNFe.transp.vol?.Where(o => !string.IsNullOrWhiteSpace(o.esp))?.FirstOrDefault()?.esp ?? string.Empty,
                NumeroDaFatura = utilizarNumeroFatura ? numeroFatura : 0,
                ValorLiquido = string.IsNullOrWhiteSpace(nfe.infNFe.cobr?.fat?.vLiq) ? 0 : decimal.Parse(nfe.infNFe.cobr?.fat?.vLiq),
                Volumes = ObterVolumes(nfe.infNFe.transp)
            };
            notaRetorno.SetarPessoaDestinatario(nfe.infNFe.dest);
            notaRetorno.SetarPessoaRemetente(nfe.infNFe.emit);
            if (nfe.infNFe.retirada != null)
                notaRetorno.SetarRetirada(nfe.infNFe.retirada);

            return notaRetorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe ObterDocumentoPorXML(MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc nfe, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa empresa = null;
            string transportadorOrigem = "";
            if (nfe.NFe.infNFe.transp != null)
            {
                if (nfe.NFe.infNFe.transp.transporta != null)
                {
                    transportadorOrigem = nfe.NFe.infNFe.transp.transporta.Item;
                    empresa = repEmpresa.BuscarPorCNPJ(nfe.NFe.infNFe.transp.transporta.Item);
                }
            }
            bool utilizarNumeroFatura = double.TryParse(string.IsNullOrWhiteSpace(nfe.NFe.infNFe.cobr?.fat?.nFat) ? string.Empty : Utilidades.String.OnlyNumbers(nfe.NFe.infNFe.cobr?.fat?.nFat), out double numeroFatura);

            var notaRetorno = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe
            {
                Chave = nfe.protNFe.infProt.chNFe,
                ValorTotal = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura),
                BaseCalculoICMS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vBC, cultura),
                ValorICMS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vICMS, cultura),
                BaseCalculoST = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vBCST, cultura),
                ValorST = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vST, cultura),
                ValorTotalProdutos = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vProd, cultura),
                ValorSeguro = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vSeg, cultura),
                ValorDesconto = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vDesc, cultura),
                ValorImpostoImportacao = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vII, cultura),
                ValorIPI = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vIPI, cultura),
                ValorPIS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vPIS, cultura),
                ValorCOFINS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vCOFINS, cultura),
                ValorOutros = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vOutro, cultura),
                NaturezaOP = nfe.NFe.infNFe.ide.natOp,
                TipoOperacao = nfe.NFe.infNFe.ide.tpNF == MultiSoftware.NFe.NotaFiscal.TNFeInfNFeIdeTpNF.Item0 ? 0 : 1,
                Empresa = empresa != null ? empresa.CNPJ_Formatado : "",
                CNPJTransportador = transportadorOrigem,
                DataEmissao = DateTime.ParseExact(nfe.NFe.infNFe.ide.dEmi, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None).ToString("dd/MM/yyyy"),
                Remetente = empresa != null ? this.ObterEmitente(nfe.NFe.infNFe.emit, empresa.Codigo) : null,
                Destinatario = !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.dest.Item) ? empresa != null ? this.ObterCPFCNPJDestinatario(nfe.NFe.infNFe.dest, empresa.Codigo, null, nfe.protNFe.infProt.chNFe) : "" : null,
                DestinatarioExportacao = string.IsNullOrWhiteSpace(nfe.NFe.infNFe.dest.Item) ? this.ObterDadosDestinatarioExportacao(nfe.NFe.infNFe.dest, unitOfWork) : null,
                Numero = nfe.NFe.infNFe.ide.nNF,
                Modelo = nfe.NFe.infNFe.ide.mod.ToString().Replace("Item", ""),
                Serie = nfe.NFe.infNFe.ide.serie,
                Peso = this.ObterPeso(nfe.NFe.infNFe.transp),
                PesoLiquido = this.ObterPesoLiquido(nfe.NFe.infNFe.transp),
                Volume = this.ObterQuantidadeVolumes(nfe.NFe.infNFe.transp),
                FormaPagamento = this.ObterFormaPagamento(nfe.NFe.infNFe.transp),
                Produtos = this.ObterProdutos(nfe.NFe.infNFe.det),
                Placa = this.ObterPlacaVeiculo(nfe.NFe.infNFe.transp),
                NumeroDosCTesUtilizados = empresa != null ? repDocumentosCTe.BuscarNumeroStatusDoCTePorChaveEEmpresa(empresa.Codigo, nfe.protNFe.infProt.chNFe) : null,
                Observacao = this.ObterObservacao(nfe.NFe.infNFe.infAdic),
                ObservacaoContribuinte = this.ObterObservacaoContribuinte(nfe.NFe.infNFe.infAdic),
                XPedido = ObterXPed(nfe.NFe.infNFe.compra),
                Especie = nfe.NFe.infNFe.transp?.vol?.Where(o => !string.IsNullOrWhiteSpace(o.esp))?.FirstOrDefault()?.esp ?? string.Empty,
                NumeroDaFatura = utilizarNumeroFatura ? numeroFatura : 0,
                ValorLiquido = string.IsNullOrWhiteSpace(nfe.NFe.infNFe.cobr?.fat?.vLiq) ? 0 : decimal.Parse(nfe.NFe.infNFe.cobr?.fat?.vLiq),
            };

            return notaRetorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe ObterDocumentoPorXML(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc nfe, Repositorio.UnitOfWork unitOfWork)
        {

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            if (unitOfWork == null)
                unitOfWork = new Repositorio.UnitOfWork(StringConexao);

            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa empresa = null;

            string transportadorOrigem = "";
            if (nfe.NFe.infNFe.transp != null)
            {
                if (nfe.NFe.infNFe.transp.transporta != null)
                {
                    transportadorOrigem = nfe.NFe.infNFe.transp.transporta.Item;
                    empresa = repEmpresa.BuscarPorCNPJ(nfe.NFe.infNFe.transp.transporta.Item);
                }
            }

            //#66774 Ajustar Depois
            //if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever))
            //{
            //    if (string.IsNullOrEmpty(nfe?.protNFe?.infProt?.Id ?? ""))
            //        throw new Exception("Campo ID do infProt não foi informado");

            //    if (string.IsNullOrEmpty(nfe?.versao ?? string.Empty))
            //        throw new Exception("Versão do nfeProc não informada");
            //}
            bool utilizarNumeroFatura = double.TryParse(string.IsNullOrWhiteSpace(nfe.NFe.infNFe.cobr?.fat?.nFat) ? string.Empty : Utilidades.String.OnlyNumbers(nfe.NFe.infNFe.cobr?.fat?.nFat), out double numeroFatura);

            var notaRetorno = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe
            {
                Chave = nfe.protNFe.infProt.chNFe,
                ValorTotal = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura),
                BaseCalculoICMS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vBC, cultura),
                ValorICMS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vICMS, cultura),
                BaseCalculoST = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vBCST, cultura),
                ValorST = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vST, cultura),
                ValorTotalProdutos = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vProd, cultura),
                ValorSeguro = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vSeg, cultura),
                ValorDesconto = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vDesc, cultura),
                ValorImpostoImportacao = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vII, cultura),
                ValorIPI = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vIPI, cultura),
                ValorPIS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vPIS, cultura),
                ValorCOFINS = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vCOFINS, cultura),
                ValorOutros = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vOutro, cultura),
                ModalidadeFrete = this.ObterFormaPagamentoModalidade(nfe.NFe.infNFe.transp),
                NaturezaOP = nfe.NFe.infNFe.ide.natOp,
                TipoOperacao = nfe.NFe.infNFe.ide.tpNF == MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeIdeTpNF.Item0 ? 0 : 1,
                Empresa = empresa != null ? empresa.CNPJ_Formatado : "",
                CNPJTransportador = transportadorOrigem,
                DataEmissao = DateTime.ParseExact(nfe.NFe.infNFe.ide.dhEmi.Substring(0, 19), "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None).ToString("dd/MM/yyyy HH:mm:ss"),
                Remetente = this.ObterEmitente(nfe.NFe.infNFe.emit, empresa != null ? empresa.Codigo : 0, unitOfWork),
                Destinatario = !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.dest.Item) && (nfe.NFe.infNFe.dest.ItemElementName == MultiSoftware.NFe.v310.NotaFiscal.ItemChoiceType3.CPF || nfe.NFe.infNFe.dest.ItemElementName == MultiSoftware.NFe.v310.NotaFiscal.ItemChoiceType3.CNPJ) ? this.ObterCPFCNPJDestinatario(nfe.NFe.infNFe.dest, empresa != null ? empresa.Codigo : 0, unitOfWork) : "",
                DestinatarioExportacao = string.IsNullOrWhiteSpace(nfe.NFe.infNFe.dest.Item) || nfe.NFe.infNFe.dest.ItemElementName == MultiSoftware.NFe.v310.NotaFiscal.ItemChoiceType3.idEstrangeiro ? this.ObterDadosDestinatarioExportacao(nfe.NFe.infNFe.dest, unitOfWork) : null,
                Numero = nfe.NFe.infNFe.ide.nNF,
                Modelo = nfe.NFe.infNFe.ide.mod.ToString().Replace("Item", ""),
                Serie = nfe.NFe.infNFe.ide.serie,
                Peso = this.ObterPeso(nfe.NFe.infNFe.transp, unitOfWork),
                PesoLiquido = this.ObterPesoLiquido(nfe.NFe.infNFe.transp),
                FormaPagamento = this.ObterFormaPagamento(nfe.NFe.infNFe.transp),
                Placa = this.ObterPlacaVeiculo(nfe.NFe.infNFe.transp),
                Produtos = this.ObterProdutos(nfe.NFe.infNFe.det),
                NumeroDosCTesUtilizados = empresa != null ? repDocumentosCTe.BuscarNumeroStatusDoCTePorChaveEEmpresa(empresa.Codigo, nfe.protNFe.infProt.chNFe) : null,
                Observacao = this.ObterObservacao(nfe.NFe.infNFe.infAdic),
                ObservacaoContribuinte = this.ObterObservacaoContribuinte(nfe.NFe.infNFe.infAdic),
                XPedido = ObterXPed(nfe.NFe.infNFe.compra),
                Volume = this.ObterQuantidadeVolumes(nfe.NFe.infNFe.transp),
                Especie = nfe.NFe.infNFe.transp?.vol?.Where(o => !string.IsNullOrWhiteSpace(o.esp))?.FirstOrDefault()?.esp ?? string.Empty,
                NumeroDaFatura = utilizarNumeroFatura ? numeroFatura : 0,
                ValorLiquido = string.IsNullOrWhiteSpace(nfe.NFe.infNFe.cobr?.fat?.vLiq) ? 0 : decimal.Parse(nfe.NFe.infNFe.cobr?.fat?.vLiq),

            };

            return notaRetorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe ObterDocumentoPorXML(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc nfe, Repositorio.UnitOfWork unitOfWork = null, bool buscarDocumentosAnterior = true, bool importarEmailCliente = false, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null)
        {
            if (unitOfWork == null)
                unitOfWork = new Repositorio.UnitOfWork(StringConexao);

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Dominio.Entidades.Empresa empresa = null;

            string transportadorOrigem = "";
            if (nfe.NFe.infNFe.transp != null && nfe.NFe.infNFe.transp.transporta != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.transp.transporta.Item))
            {
                transportadorOrigem = nfe.NFe.infNFe.transp.transporta.Item;
                empresa = repEmpresa.BuscarPorCNPJ(nfe.NFe.infNFe.transp.transporta.Item);
            }

            //#66774 Ajustar depois
            //if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever))
            //{
            //    if (string.IsNullOrEmpty(nfe?.protNFe?.infProt?.Id ?? ""))
            //        throw new Exception("Campo ID do infProt não foi informado");

            //    if (string.IsNullOrEmpty(nfe?.versao ?? string.Empty))
            //        throw new Exception("Versão do nfeProc não informada");
            //}
            bool utilizarNumeroFatura = double.TryParse(string.IsNullOrWhiteSpace(nfe.NFe.infNFe.cobr?.fat?.nFat) ? string.Empty : Utilidades.String.OnlyNumbers(nfe.NFe.infNFe.cobr?.fat?.nFat), out double numeroFatura);
            var itemIBSCBS = nfe.NFe.infNFe.det[0].imposto.IBSCBS?.Item as MultiSoftware.NFe.v400.NotaFiscal.TCIBS;

            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe notaRetorno = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe
            {
                Chave = nfe.protNFe.infProt.chNFe,
                ValorTotal = nfe.NFe.infNFe.total.ICMSTot.vNF != null ? decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura) : 0m,
                BaseCalculoICMS = nfe.NFe.infNFe.total.ICMSTot.vBC != null ? decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vBC, cultura) : 0m,
                ValorICMS = nfe.NFe.infNFe.total.ICMSTot.vICMS != null ? decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vICMS, cultura) : 0m,
                BaseCalculoST = nfe.NFe.infNFe.total.ICMSTot.vBCST != null ? decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vBCST, cultura) : 0m,
                ValorST = nfe.NFe.infNFe.total.ICMSTot.vST != null ? decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vST, cultura) : 0m,
                ValorTotalProdutos = nfe.NFe.infNFe.total.ICMSTot.vProd != null ? decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vProd, cultura) : 0m,
                ValorSeguro = nfe.NFe.infNFe.total.ICMSTot.vSeg != null ? decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vSeg, cultura) : 0m,
                ValorDesconto = nfe.NFe.infNFe.total.ICMSTot.vDesc != null ? decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vDesc, cultura) : 0m,
                ValorImpostoImportacao = nfe.NFe.infNFe.total.ICMSTot.vII != null ? decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vII, cultura) : 0m,
                ValorIPI = nfe.NFe.infNFe.total.ICMSTot.vIPI != null ? decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vIPI, cultura) : 0m,
                ValorPIS = nfe.NFe.infNFe.total.ICMSTot.vPIS != null ? decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vPIS, cultura) : 0m,
                ValorCOFINS = nfe.NFe.infNFe.total.ICMSTot.vCOFINS != null ? decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vCOFINS, cultura) : 0m,
                ValorOutros = nfe.NFe.infNFe.total.ICMSTot.vOutro != null ? decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vOutro, cultura) : 0m,
                ValorFrete = nfe.NFe.infNFe.total.ICMSTot.vFrete != null ? decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vFrete, cultura) : 0m,
                ModalidadeFrete = this.ObterFormaPagamentoModalidade(nfe.NFe.infNFe.transp),
                NaturezaOP = nfe.NFe.infNFe.ide.natOp,
                CSTIBSCBS = nfe.NFe.infNFe.det[0].imposto.IBSCBS?.CST,
                ClassificacaoTributariaIBSCBS = nfe.NFe.infNFe.det[0].imposto.IBSCBS?.cClassTrib,
                BaseCalculoIBSCBS = itemIBSCBS?.vBC != null ? decimal.Parse(itemIBSCBS.vBC, cultura) : 0m,
                AliquotaIBSEstadual = itemIBSCBS?.gIBSUF?.pIBSUF != null ? decimal.Parse(itemIBSCBS.gIBSUF.pIBSUF, cultura) : 0m,
                PercentualReducaoIBSEstadual = itemIBSCBS?.gIBSUF?.gDif?.pDif != null ? decimal.Parse(itemIBSCBS.gIBSUF.gDif.pDif, cultura) : 0m,
                ValorReducaoIBSEstadual = itemIBSCBS?.gIBSUF?.gDif?.vDif != null ? decimal.Parse(itemIBSCBS.gIBSUF.gDif.vDif, cultura) : 0m,
                ValorIBSEstadual = itemIBSCBS?.gIBSUF?.vIBSUF != null ? decimal.Parse(itemIBSCBS.gIBSUF.vIBSUF, cultura) : 0m,
                AliquotaIBSMunicipal = itemIBSCBS?.gIBSMun?.pIBSMun != null ? decimal.Parse(itemIBSCBS.gIBSMun.pIBSMun, cultura) : 0m,
                PercentualReducaoIBSMunicipal = itemIBSCBS?.gIBSMun?.gDif?.pDif != null ? decimal.Parse(itemIBSCBS.gIBSMun.gDif.pDif, cultura) : 0m,
                ValorReducaoIBSMunicipal = itemIBSCBS?.gIBSMun?.gDif?.vDif != null ? decimal.Parse(itemIBSCBS.gIBSMun.gDif.vDif, cultura) : 0m,
                ValorIBSMunicipal = itemIBSCBS?.gIBSMun?.vIBSMun != null ? decimal.Parse(itemIBSCBS.gIBSMun.vIBSMun, cultura) : 0m,
                AliquotaCBS = itemIBSCBS?.gCBS?.pCBS != null ? decimal.Parse(itemIBSCBS.gCBS.pCBS, cultura) : 0m,
                PercentualReducaoCBS = itemIBSCBS?.gCBS?.gDif?.pDif != null ? decimal.Parse(itemIBSCBS.gCBS.gDif.pDif, cultura) : 0m,
                ValorReducaoCBS = itemIBSCBS?.gCBS?.gDif?.vDif != null ? decimal.Parse(itemIBSCBS.gCBS.gDif.vDif, cultura) : 0m,
                ValorCBS = itemIBSCBS?.gCBS?.vCBS != null ? decimal.Parse(itemIBSCBS.gCBS.vCBS, cultura) : 0m,
                TipoOperacao = nfe.NFe.infNFe.ide.tpNF == MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeIdeTpNF.Item0 ? 0 : 1,
                Empresa = empresa != null ? empresa.CNPJ_Formatado : "",
                CNPJTransportador = transportadorOrigem,
                DataEmissao = DateTime.ParseExact(nfe.NFe.infNFe.ide.dhEmi.Substring(0, 19), "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None).ToString("dd/MM/yyyy HH:mm:ss"),
                Remetente = this.ObterEmitente(nfe.NFe.infNFe.emit, empresa?.Codigo ?? 0, unitOfWork),
                LocalEntrega = this.ObterCPFCNPJLocalEntrega(nfe.NFe.infNFe.entrega, empresa?.Codigo ?? 0, unitOfWork) ?? string.Empty,
                Destinatario = !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.dest.Item) && (nfe.NFe.infNFe.dest.ItemElementName == MultiSoftware.NFe.v400.NotaFiscal.ItemChoiceType3.CPF || nfe.NFe.infNFe.dest.ItemElementName == MultiSoftware.NFe.v400.NotaFiscal.ItemChoiceType3.CNPJ) ? this.ObterCPFCNPJDestinatario(nfe.NFe.infNFe.dest, empresa?.Codigo ?? 0, unitOfWork, importarEmailCliente, nfe.protNFe.infProt.chNFe) : "",
                DestinatarioExportacao = string.IsNullOrWhiteSpace(nfe.NFe.infNFe.dest.Item) || nfe.NFe.infNFe.dest.ItemElementName == MultiSoftware.NFe.v400.NotaFiscal.ItemChoiceType3.idEstrangeiro ? this.ObterDadosDestinatarioExportacao(nfe.NFe.infNFe.dest, unitOfWork) : null,
                Numero = nfe.NFe.infNFe.ide.nNF,
                Modelo = nfe.NFe.infNFe.ide.mod.ToString().Replace("Item", ""),
                Serie = nfe.NFe.infNFe.ide.serie,
                Peso = this.ObterPeso(nfe.NFe.infNFe.transp, empresa?.Codigo ?? 0, unitOfWork, cargaPedido),
                PesoLiquido = this.ObterPesoLiquido(nfe.NFe.infNFe.transp),
                FormaPagamento = this.ObterFormaPagamento(nfe.NFe.infNFe.transp),
                Placa = this.ObterPlacaVeiculo(nfe.NFe.infNFe.transp),
                Produtos = this.ObterProdutos(nfe.NFe.infNFe.det),
                NumeroDosCTesUtilizados = buscarDocumentosAnterior && empresa != null ? repDocumentosCTe.BuscarNumeroStatusDoCTePorChaveEEmpresaQuery(empresa.Codigo, nfe.protNFe.infProt.chNFe).ToList() : null,
                Observacao = this.ObterObservacao(nfe.NFe.infNFe.infAdic),
                ObservacaoContribuinte = this.ObterObservacaoContribuinte(nfe.NFe.infNFe.infAdic),
                XPedido = ObterXPed(nfe.NFe.infNFe.compra),
                Protocolo = nfe.protNFe.infProt.nProt,
                Volume = this.ObterQuantidadeVolumes(nfe.NFe.infNFe.transp),
                Especie = nfe.NFe.infNFe.transp?.vol?.Where(o => !string.IsNullOrWhiteSpace(o.esp))?.FirstOrDefault()?.esp ?? string.Empty,
                NumeroDaFatura = utilizarNumeroFatura ? numeroFatura : 0,
                ValorLiquido = string.IsNullOrWhiteSpace(nfe.NFe.infNFe.cobr?.fat?.vLiq) ? 0 : decimal.Parse(nfe.NFe.infNFe.cobr?.fat?.vLiq),
                Volumes = ObterVolumes(nfe.NFe.infNFe.transp)
            };
            // TODO: ToList cast
            
            notaRetorno.SetarPessoaDestinatario(nfe.NFe.infNFe.dest);
            notaRetorno.SetarPessoaRemetente(nfe.NFe.infNFe.emit);

            if (nfe.NFe.infNFe.retirada != null)
                notaRetorno.SetarRetirada(nfe.NFe.infNFe.retirada);

            return notaRetorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> ObterProdutos(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDet[] det)
        {
            List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> produtos = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos>();
            if (det == null)
                return produtos;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");


            for (var i = 0; i < det.Length; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.NFe.Produtos produto = new Dominio.ObjetosDeValor.Embarcador.NFe.Produtos();
                produto.CFOP = int.Parse(det[i].prod.CFOP);
                produto.Codigo = det[i].prod.cProd;
                produto.Descricao = det[i].prod.xProd;
                produto.NCM = det[i].prod.NCM;
                produto.NumeroPedidoCompra = det[i].prod.xPed;
                produto.QuantidadeComercial = string.IsNullOrWhiteSpace(det[i].prod.qCom) ? 0m : decimal.Parse(det[i].prod.qCom, cultura);
                produto.QuantidadeTributaria = string.IsNullOrWhiteSpace(det[i].prod.qTrib) ? 0m : decimal.Parse(det[i].prod.qTrib, cultura);
                produto.UnidadeComercial = det[i].prod.uCom;
                produto.UnidadeTributaria = det[i].prod.uTrib;
                produto.CodigoCEAN = det[i].prod.cEAN;
                produto.ValorTotal = string.IsNullOrWhiteSpace(det[i].prod.vProd) ? 0m : decimal.Parse(det[i].prod.vProd, cultura);
                produto.ValorUnitarioComercial = string.IsNullOrWhiteSpace(det[i].prod.vUnCom) ? 0m : decimal.Parse(det[i].prod.vUnCom, cultura);
                produto.ValorUnitarioTributaria = string.IsNullOrWhiteSpace(det[i].prod.vUnTrib) ? 0m : decimal.Parse(det[i].prod.vUnTrib, cultura);
                produtos.Add(produto);
            }
            return produtos;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> ObterProdutos(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDet[] det)
        {
            List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> produtos = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos>();

            if (det == null)
                return produtos;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            for (var i = 0; i < det.Length; i++)
            {
                if (det[i].prod == null || det[i].prod.cProd == null)
                    continue;

                Dominio.ObjetosDeValor.Embarcador.NFe.Produtos produto = new Dominio.ObjetosDeValor.Embarcador.NFe.Produtos();
                produto.CFOP = int.Parse(det[i].prod.CFOP);
                produto.Codigo = det[i].prod.cProd;
                produto.Descricao = det[i].prod.xProd;
                produto.NCM = det[i].prod.NCM;
                produto.NumeroPedidoCompra = det[i].prod.xPed;
                produto.QuantidadeComercial = string.IsNullOrWhiteSpace(det[i].prod.qCom) ? 0m : decimal.Parse(det[i].prod.qCom, cultura);
                produto.QuantidadeTributaria = string.IsNullOrWhiteSpace(det[i].prod.qTrib) ? 0m : decimal.Parse(det[i].prod.qTrib, cultura);
                produto.UnidadeComercial = det[i].prod.uCom;
                produto.CodigoCEAN = det[i].prod.cEAN;
                produto.Lotes = ObterLotes(det[i].prod);
                produto.UnidadeTributaria = det[i].prod.uTrib;
                produto.CodigoNFCI = det[i].prod.nFCI;
                produto.ValorTotal = string.IsNullOrWhiteSpace(det[i].prod.vProd) ? 0m : decimal.Parse(det[i].prod.vProd, cultura);
                produto.ValorUnitarioComercial = string.IsNullOrWhiteSpace(det[i].prod.vUnCom) ? 0m : decimal.Parse(det[i].prod.vUnCom, cultura);
                produto.ValorUnitarioTributaria = string.IsNullOrWhiteSpace(det[i].prod.vUnTrib) ? 0m : decimal.Parse(det[i].prod.vUnTrib, cultura);
                try
                {
                    var icms = (from obj in det[i].imposto.Items where obj.GetType() == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMS) select (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMS)obj).FirstOrDefault();
                    if (icms != null)
                    {
                        var tipoICMS = icms.Item?.GetType();

                        if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00 impICMS00 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00)icms.Item;
                            produto.CSTICMS = string.Format("{0:00}", (int)impICMS00.CST);
                            produto.OrigemMercadoria = string.Format("{0:0}", (int)impICMS00.orig);
                        }
                        else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10 impICMS10 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10)icms.Item;
                            produto.CSTICMS = string.Format("{0:00}", (int)impICMS10.CST);
                            produto.OrigemMercadoria = string.Format("{0:0}", (int)impICMS10.orig);
                        }
                        else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20 impICMS20 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20)icms.Item;
                            produto.CSTICMS = string.Format("{0:00}", (int)impICMS20.CST);
                            produto.OrigemMercadoria = string.Format("{0:0}", (int)impICMS20.orig);
                        }
                        else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30 impICMS30 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30)icms.Item;
                            produto.CSTICMS = string.Format("{0:00}", (int)impICMS30.CST);
                            produto.OrigemMercadoria = string.Format("{0:0}", (int)impICMS30.orig);
                        }
                        else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40 impICMS40 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40)icms.Item;
                            produto.CSTICMS = string.Format("{0:00}", (int)impICMS40.CST);
                            produto.OrigemMercadoria = string.Format("{0:0}", (int)impICMS40.orig);
                        }
                        else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51 impICMS51 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51)icms.Item;
                            produto.CSTICMS = string.Format("{0:00}", (int)impICMS51.CST);
                            produto.OrigemMercadoria = string.Format("{0:0}", (int)impICMS51.orig);
                        }
                        else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60 impICMS60 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60)icms.Item;
                            produto.CSTICMS = string.Format("{0:00}", (int)impICMS60.CST);
                            produto.OrigemMercadoria = string.Format("{0:0}", (int)impICMS60.orig);
                        }
                        else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70 impICMS70 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70)icms.Item;
                            produto.CSTICMS = string.Format("{0:00}", (int)impICMS70.CST);
                            produto.OrigemMercadoria = string.Format("{0:0}", (int)impICMS70.orig);
                        }
                        else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90 impICMS90 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90)icms.Item;
                            produto.CSTICMS = string.Format("{0:00}", (int)impICMS90.CST);
                            produto.OrigemMercadoria = string.Format("{0:0}", (int)impICMS90.orig);
                        }
                        else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart impICMSPart = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart)icms.Item;
                            produto.CSTICMS = string.Format("{0:00}", (int)impICMSPart.CST);
                            produto.OrigemMercadoria = string.Format("{0:0}", (int)impICMSPart.orig);
                        }
                        else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST impICMSST = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST)icms.Item;
                            produto.CSTICMS = string.Format("{0:00}", (int)impICMSST.CST);
                            produto.OrigemMercadoria = string.Format("{0:0}", (int)impICMSST.orig);
                        }
                        else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101 impICMSSN101 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101)icms.Item;
                            produto.CSTICMS = string.Format("{0:000}", (int)impICMSSN101.CSOSN);
                            produto.OrigemMercadoria = string.Format("{0:0}", (int)impICMSSN101.orig);
                        }
                        else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102 impICMSSN102 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102)icms.Item;
                            produto.CSTICMS = string.Format("{0:000}", (int)impICMSSN102.CSOSN);
                            produto.OrigemMercadoria = string.Format("{0:0}", (int)impICMSSN102.orig);
                        }
                        else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201 impICMSSN201 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201)icms.Item;
                            produto.CSTICMS = string.Format("{0:000}", (int)impICMSSN201.CSOSN);
                            produto.OrigemMercadoria = string.Format("{0:0}", (int)impICMSSN201.orig);
                        }
                        else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202 impICMSSN202 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202)icms.Item;
                            produto.CSTICMS = string.Format("{0:000}", (int)impICMSSN202.CSOSN);
                            produto.OrigemMercadoria = string.Format("{0:0}", (int)impICMSSN202.orig);
                        }
                        else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500 impICMSSN500 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500)icms.Item;
                            produto.CSTICMS = string.Format("{0:000}", (int)impICMSSN500.CSOSN);
                            produto.OrigemMercadoria = string.Format("{0:0}", (int)impICMSSN500.orig);
                        }
                        else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900 impICMSSN900 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900)icms.Item;
                            produto.CSTICMS = string.Format("{0:000}", (int)impICMSSN900.CSOSN);
                            produto.OrigemMercadoria = string.Format("{0:0}", (int)impICMSSN900.orig);
                        }
                    }
                }
                catch
                {
                }


                produtos.Add(produto);
            }

            return produtos;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.NFe.Lote> ObterLotes(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetProd prod)
        {
            List<Dominio.ObjetosDeValor.Embarcador.NFe.Lote> lotes = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Lote>();

            if (prod?.rastro == null)
                return lotes;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            for (var i = 0; i < prod.rastro.Length; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.NFe.Lote lote = new Dominio.ObjetosDeValor.Embarcador.NFe.Lote();
                lote.NumeroLote = prod.rastro[i].nLote;
                lote.QuantidadeLote = string.IsNullOrWhiteSpace(prod.rastro[i].qLote) ? 0m : decimal.Parse(prod.rastro[i].qLote, cultura);

                DateTime dataFabricacao;
                if (!DateTime.TryParseExact(prod.rastro[i].dFab, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dataFabricacao))
                    dataFabricacao = DateTime.Now;

                DateTime dataValidade;
                if (!DateTime.TryParseExact(prod.rastro[i].dVal, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dataValidade))
                    dataFabricacao = DateTime.Now;

                lote.DataFabricacao = dataFabricacao;
                lote.DataValidade = dataValidade;

                lotes.Add(lote);
            }

            return lotes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> ObterProdutos(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDet[] det)
        {
            List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> produtos = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos>();
            if (det == null)
                return produtos;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");


            for (var i = 0; i < det.Length; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.NFe.Produtos produto = new Dominio.ObjetosDeValor.Embarcador.NFe.Produtos();
                produto.CFOP = (int)det[i].prod.CFOP;
                produto.Codigo = det[i].prod.cProd;
                produto.Descricao = det[i].prod.xProd;
                produto.NCM = det[i].prod.NCM;
                produto.NumeroPedidoCompra = det[i].prod.xPed;
                produto.QuantidadeComercial = string.IsNullOrWhiteSpace(det[i].prod.qCom) ? 0m : decimal.Parse(det[i].prod.qCom, cultura);
                produto.QuantidadeTributaria = string.IsNullOrWhiteSpace(det[i].prod.qTrib) ? 0m : decimal.Parse(det[i].prod.qTrib, cultura);
                produto.UnidadeComercial = det[i].prod.uCom;
                produto.CodigoCEAN = det[i].prod.cEAN;
                produto.UnidadeTributaria = det[i].prod.uTrib;
                produto.ValorTotal = string.IsNullOrWhiteSpace(det[i].prod.vProd) ? 0m : decimal.Parse(det[i].prod.vProd, cultura);
                produto.ValorUnitarioComercial = string.IsNullOrWhiteSpace(det[i].prod.vUnCom) ? 0m : decimal.Parse(det[i].prod.vUnCom, cultura);
                produto.ValorUnitarioTributaria = string.IsNullOrWhiteSpace(det[i].prod.vUnTrib) ? 0m : decimal.Parse(det[i].prod.vUnTrib, cultura);
                produtos.Add(produto);
            }
            return produtos;
        }

        private string ObterXPed(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeCompra compra)
        {
            if (compra != null)
            {
                if (compra.xPed != null && compra.xPed != "")
                    return compra.xPed;
            }
            return string.Empty;
        }

        private string ObterObservacao(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeInfAdic infAdic)
        {
            if (infAdic != null)
            {
                if (infAdic.infCpl != null && infAdic.infCpl != "")
                    return infAdic.infCpl;
            }
            return string.Empty;
        }

        private string ObterPlacaVeiculo(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null)
            {
                if (infNFeTransp.Items != null)
                {
                    string placa = string.Empty;
                    foreach (var item in infNFeTransp.Items)
                        if (item.GetType() == typeof(MultiSoftware.NFe.NotaFiscal.TVeiculo))
                        {
                            MultiSoftware.NFe.NotaFiscal.TVeiculo veiculo = (MultiSoftware.NFe.NotaFiscal.TVeiculo)item;
                            return veiculo.placa;
                        }
                }
            }
            return string.Empty;
        }

        private string ObterObservacao(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeInfAdic infAdic)
        {
            if (infAdic != null)
            {
                if (infAdic.infCpl != null && infAdic.infCpl != "")
                    return infAdic.infCpl;
            }
            return string.Empty;
        }

        private string ObterObservacao(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeInfAdic infAdic)
        {
            if (infAdic != null)
            {
                if (infAdic.infCpl != null && infAdic.infCpl != "")
                    return infAdic.infCpl;
            }
            return string.Empty;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.NFe.Volume> ObterVolumes(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null && infNFeTransp.vol != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");
                return infNFeTransp.vol.Select(o => new Dominio.ObjetosDeValor.Embarcador.NFe.Volume()
                {
                    Numeracao = o.nVol,
                    Especie = o.esp,
                    MarcaVolume = o.marca,
                    Quantidade = !string.IsNullOrWhiteSpace(o.qVol) ? int.Parse(o.qVol) : 0,
                    PesoLiquido = !string.IsNullOrEmpty(o.pesoL) ? decimal.Parse(o.pesoL, cultura) : 0,
                    PesoBruto = !string.IsNullOrEmpty(o.pesoB) ? decimal.Parse(o.pesoB, cultura) : 0
                }).ToList();
            }

            return null;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.NFe.Observacao> ObterObservacaoContribuinte(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeInfAdic infAdic)
        {
            if (infAdic != null && infAdic.obsCont != null)
                return infAdic.obsCont.Select(o => new Dominio.ObjetosDeValor.Embarcador.NFe.Observacao() { xCampo = o.xCampo, xTexto = o.xTexto }).ToList();

            return null;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.NFe.Observacao> ObterObservacaoContribuinte(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeInfAdic infAdic)
        {
            if (infAdic != null && infAdic.obsCont != null)
                return infAdic.obsCont.Select(o => new Dominio.ObjetosDeValor.Embarcador.NFe.Observacao() { xCampo = o.xCampo, xTexto = o.xTexto }).ToList();

            return null;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.NFe.Observacao> ObterObservacaoContribuinte(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeInfAdic infAdic)
        {
            if (infAdic != null && infAdic.obsCont != null)
                return infAdic.obsCont.Select(o => new Dominio.ObjetosDeValor.Embarcador.NFe.Observacao() { xCampo = o.xCampo, xTexto = o.xTexto }).ToList();

            return null;
        }

        private string ObterXPed(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeCompra compra)
        {
            if (compra != null)
            {
                if (compra.xPed != null && compra.xPed != "")
                    return compra.xPed;
            }
            return string.Empty;
        }

        private string ObterXPed(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeCompra compra)
        {
            if (compra != null)
            {
                if (compra.xPed != null && compra.xPed != "")
                    return compra.xPed;
            }
            return string.Empty;
        }

        private string ObterPlacaVeiculo(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null)
            {
                if (infNFeTransp.Items != null)
                {
                    string placa = string.Empty;

                    foreach (var item in infNFeTransp.Items)
                        if (item.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscal.TVeiculo))
                        {
                            MultiSoftware.NFe.v310.NotaFiscal.TVeiculo veiculo = (MultiSoftware.NFe.v310.NotaFiscal.TVeiculo)item;
                            return veiculo.placa;
                        }
                }
            }
            return string.Empty;
        }

        private string ObterPlacaVeiculo(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null)
            {
                if (infNFeTransp.Items != null)
                {
                    string placa = string.Empty;

                    foreach (var item in infNFeTransp.Items)
                        if (item.GetType() == typeof(MultiSoftware.NFe.v400.NotaFiscal.TVeiculo))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TVeiculo veiculo = (MultiSoftware.NFe.v400.NotaFiscal.TVeiculo)item;
                            return veiculo.placa;
                        }
                }
            }
            return string.Empty;
        }

        private string ObterCNPJEmitente(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeEmit infNFeEmit, int codigoEmpresa)
        {
            Dominio.Entidades.Cliente cliente = this.ObterEmitente(infNFeEmit, codigoEmpresa);
            return cliente != null ? cliente.CPF_CNPJ_Formatado : string.Empty;
        }

        private string ObterCNPJEmitente(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeEmit infNFeEmit, int codigoEmpresa)
        {
            Dominio.Entidades.Cliente cliente = this.ObterEmitente(infNFeEmit, codigoEmpresa);
            return cliente != null ? cliente.CPF_CNPJ_Formatado : string.Empty;
        }

        private Dominio.Entidades.Cliente ObterClienteEmitente(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeEmit infNFeEmit, int codigoEmpresa)
        {
            Dominio.Entidades.Cliente cliente = this.ObterEmitente(infNFeEmit, codigoEmpresa);
            return cliente;
        }
        
        private Dominio.Entidades.Cliente ObterClienteEmitente(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeEmit infNFeEmit, int codigoEmpresa)
        {
            Dominio.Entidades.Cliente cliente = this.ObterEmitente(infNFeEmit, codigoEmpresa);
            return cliente;
        }
        
        private Dominio.Entidades.Cliente ObterClienteEmitente(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeEmit infNFeEmit, int codigoEmpresa)
        {
            Dominio.Entidades.Cliente cliente = this.ObterEmitente(infNFeEmit, codigoEmpresa);
            return cliente;
        }

        private string ObterCNPJEmitente(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeEmit infNFeEmit, int codigoEmpresa)
        {
            Dominio.Entidades.Cliente cliente = this.ObterEmitente(infNFeEmit, codigoEmpresa);
            return cliente != null ? cliente.CPF_CNPJ_Formatado : string.Empty;
        }

        private string ObterCNPJPessoa(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa, int codigoEmpresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Dominio.Entidades.Cliente cliente = this.ObterPessoa(pessoa, codigoEmpresa, unidadeTrabalho);
            return cliente != null ? cliente.CPF_CNPJ_Formatado : string.Empty;
        }

        private string ObterCPFCNPJDestinatario(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDest infNFeDest, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork = null, string chaveNotaFiscal = "")
        {
            Dominio.Entidades.Cliente cliente = this.ObterDestinatario(infNFeDest, codigoEmpresa, unitOfWork, chaveNotaFiscal);
            return cliente.CPF_CNPJ_Formatado;
        }

        private string ObterCPFCNPJDestinatario(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDest infNFeDest, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork = null, string chaveNotaFiscal = "")
        {
            Dominio.Entidades.Cliente cliente = this.ObterDestinatario(infNFeDest, codigoEmpresa, unitOfWork, chaveNotaFiscal);
            return cliente.CPF_CNPJ_Formatado;
        }
        
        private Dominio.Entidades.Cliente ObterClienteDestinatario(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDest infNFeDest, int codigoEmpresa, string chaveNotaFiscal = "")
        {
            Dominio.Entidades.Cliente cliente = this.ObterDestinatario(infNFeDest, codigoEmpresa, null, chaveNotaFiscal);
            return cliente;
        }
       
        private Dominio.Entidades.Cliente ObterClienteDestinatario(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDest infNFeDest, int codigoEmpresa, string chaveNotaFiscal = "")
        {
            Dominio.Entidades.Cliente cliente = this.ObterDestinatario(infNFeDest, codigoEmpresa, null, false, chaveNotaFiscal);
            return cliente;
        }
        
        private Dominio.Entidades.Cliente ObterClienteDestinatario(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDest infNFeDest, int codigoEmpresa, string chaveNotaFiscal = "")
        {
            Dominio.Entidades.Cliente cliente = this.ObterDestinatario(infNFeDest, codigoEmpresa, null, chaveNotaFiscal);
            return cliente;
        }

        private string ObterCPFCNPJDestinatario(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDest infNFeDest, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork = null, bool importarEmailCliente = false, string chaveNotaFiscal = "")
        {
            Dominio.Entidades.Cliente cliente = this.ObterDestinatario(infNFeDest, codigoEmpresa, unitOfWork, importarEmailCliente, chaveNotaFiscal);
            return cliente.CPF_CNPJ_Formatado;
        }

        private string ObterCPFCNPJLocalEntrega(MultiSoftware.NFe.v400.NotaFiscal.TLocal local, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            if (local == null || string.IsNullOrWhiteSpace(local.Item))
                return null;

            Dominio.Entidades.Cliente cliente = ObterLocal(local, codigoEmpresa, unitOfWork);
            return cliente.CPF_CNPJ_Formatado;
        }

        private Dominio.ObjetosDeValor.Cliente ObterDadosDestinatarioExportacao(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDest infNFeDest, Repositorio.UnitOfWork unitOfWork = null)
        {
            if (unitOfWork == null)
                unitOfWork = new Repositorio.UnitOfWork(StringConexao);

            Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);

            Dominio.Entidades.Pais pais = repPais.BuscarPorCodigo((int)infNFeDest.enderDest.cPais);

            Dominio.ObjetosDeValor.Cliente clienteExportacao = new Dominio.ObjetosDeValor.Cliente();
            clienteExportacao.RazaoSocial = Utilidades.String.RemoverCaracteresEspecialSerpro(infNFeDest.xNome?.Replace("&amp;", "")?.Replace(" amp ", ""));
            clienteExportacao.Endereco = infNFeDest.enderDest.xLgr;
            clienteExportacao.Bairro = infNFeDest.enderDest.xBairro;
            clienteExportacao.Complemento = infNFeDest.enderDest.xCpl;
            clienteExportacao.Emails = infNFeDest.email;
            clienteExportacao.Numero = infNFeDest.enderDest.nro;
            clienteExportacao.Cidade = infNFeDest.enderDest.xMun;
            clienteExportacao.SiglaPais = pais.Sigla;
            clienteExportacao.CodigoPais = pais.Codigo;
            clienteExportacao.DescricaoPais = pais.Nome;
            clienteExportacao.Exportacao = true;

            return clienteExportacao;
        }

        private Dominio.ObjetosDeValor.Cliente ObterDadosDestinatarioExportacao(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDest infNFeDest, Repositorio.UnitOfWork unitOfWork = null)
        {
            if (unitOfWork == null)
                unitOfWork = new Repositorio.UnitOfWork(StringConexao);

            Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);

            Dominio.Entidades.Pais pais = repPais.BuscarPorCodigo(int.Parse(infNFeDest.enderDest.cPais));

            Dominio.ObjetosDeValor.Cliente clienteExportacao = new Dominio.ObjetosDeValor.Cliente();
            clienteExportacao.RazaoSocial = Utilidades.String.RemoverCaracteresEspecialSerpro(infNFeDest.xNome?.Replace("&amp;", "")?.Replace(" amp ", ""));
            clienteExportacao.Endereco = infNFeDest.enderDest.xLgr;
            clienteExportacao.Bairro = infNFeDest.enderDest.xBairro;
            clienteExportacao.Complemento = infNFeDest.enderDest.xCpl;
            clienteExportacao.Emails = infNFeDest.email;
            clienteExportacao.Numero = infNFeDest.enderDest.nro;
            clienteExportacao.Cidade = infNFeDest.enderDest.xMun;
            clienteExportacao.SiglaPais = pais.Sigla;
            clienteExportacao.CodigoPais = pais.Codigo;
            clienteExportacao.DescricaoPais = pais.Nome;
            clienteExportacao.Exportacao = true;

            return clienteExportacao;
        }

        private Dominio.ObjetosDeValor.Cliente ObterDadosDestinatarioExportacao(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeDest infNFeDest, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);

            Dominio.Entidades.Pais pais = repPais.BuscarPorCodigo((int)infNFeDest.enderDest.cPais);

            Dominio.ObjetosDeValor.Cliente clienteExportacao = new Dominio.ObjetosDeValor.Cliente();
            clienteExportacao.RazaoSocial = Utilidades.String.RemoverCaracteresEspecialSerpro(infNFeDest.xNome?.Replace("&amp;", "")?.Replace(" amp ", ""));
            clienteExportacao.Endereco = infNFeDest.enderDest.xLgr;
            clienteExportacao.Bairro = infNFeDest.enderDest.xBairro;
            clienteExportacao.Complemento = infNFeDest.enderDest.xCpl;
            clienteExportacao.Emails = infNFeDest.email;
            clienteExportacao.Numero = infNFeDest.enderDest.nro;
            clienteExportacao.Cidade = infNFeDest.enderDest.xMun;
            clienteExportacao.SiglaPais = pais.Sigla;
            clienteExportacao.CodigoPais = pais.Codigo;
            clienteExportacao.DescricaoPais = pais.Nome;
            clienteExportacao.Exportacao = true;

            return clienteExportacao;
        }

        private int ObterSerie(MultiSoftware.NFe.NotaFiscal.TNFeInfNFeEmit infNFeEmit, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            if (empresa == null || empresa.Configuracao == null || empresa.Configuracao.SerieInterestadual == null || empresa.Configuracao.SerieIntraestadual == null)
                return 0;

            double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(infNFeEmit.Item));

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            if (cliente == null)
                return 0;

            if (cliente.Localidade.Estado.Sigla != empresa.Localidade.Estado.Sigla)
                return empresa.Configuracao.SerieInterestadual.Codigo;
            else
                return empresa.Configuracao.SerieIntraestadual.Codigo;
        }

        private int ObterSerie(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa emitente, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            if (empresa == null || empresa.Configuracao == null || empresa.Configuracao.SerieInterestadual == null || empresa.Configuracao.SerieIntraestadual == null)
                return 0;

            double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(emitente.CPFCNPJ));

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            if (cliente == null)
                return 0;

            if (cliente.Localidade.Estado.Sigla != empresa.Localidade.Estado.Sigla)
                return empresa.Configuracao.SerieInterestadual.Codigo;
            else
                return empresa.Configuracao.SerieIntraestadual.Codigo;
        }

        private int ObterSerie(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeEmit infNFeEmit, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.EstadosDeEmissaoSerie repEstadosDeEmissaoSerie = new Repositorio.EstadosDeEmissaoSerie(unitOfWork);
            Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.Entidades.EstadosDeEmissaoSerie estadoDeEmissaoSerie = null;
            Dominio.Entidades.EmpresaSerie serie = null;

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            if (empresa == null || empresa.Configuracao == null || empresa.Configuracao.SerieInterestadual == null || empresa.Configuracao.SerieIntraestadual == null)
                return 0;

            double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(infNFeEmit.Item));

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            if (cliente == null)
                return 0;

            if (serie == null && empresa != null && empresa.Configuracao != null)
            {
                estadoDeEmissaoSerie = repEstadosDeEmissaoSerie.BuscarPorUF(empresa.Configuracao.Codigo, cliente.Localidade.Estado.Sigla);

                if (estadoDeEmissaoSerie != null)
                    serie = estadoDeEmissaoSerie.Serie;

                if (serie == null)
                {
                    if (empresa.Configuracao.SerieInterestadual != null && cliente.Localidade.Estado.Sigla != empresa.Localidade.Estado.Sigla)
                    {
                        serie = empresa.Configuracao.SerieInterestadual;
                    }
                    else if (empresa.Configuracao.SerieIntraestadual != null && cliente.Localidade.Estado.Sigla == empresa.Localidade.Estado.Sigla)
                    {
                        serie = empresa.Configuracao.SerieIntraestadual;
                    }
                }
            }

            if (serie == null)
            {
                serie = repCTe.BuscarSerieUltimoCTeEmitido(codigoEmpresa, null);

                if (serie == null)
                    serie = repSerie.BuscarUltimoRegistro(codigoEmpresa, null);
            }

            if (serie == null)
                serie = repSerie.BuscarPorEmpresaTipo(codigoEmpresa, Dominio.Enumeradores.TipoSerie.CTe);

            return serie.Codigo;
        }
        
        private int ObterSerie(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeEmit infNFeEmit, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.EstadosDeEmissaoSerie repEstadosDeEmissaoSerie = new Repositorio.EstadosDeEmissaoSerie(unitOfWork);
            Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.Entidades.EstadosDeEmissaoSerie estadoDeEmissaoSerie = null;
            Dominio.Entidades.EmpresaSerie serie = null;

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            if (empresa == null || empresa.Configuracao == null || empresa.Configuracao.SerieInterestadual == null || empresa.Configuracao.SerieIntraestadual == null)
                return 0;

            double cpfCnpj = double.Parse(Utilidades.String.OnlyNumbers(infNFeEmit.Item));

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            if (cliente == null)
                return 0;

            if (serie == null && empresa != null && empresa.Configuracao != null)
            {
                estadoDeEmissaoSerie = repEstadosDeEmissaoSerie.BuscarPorUF(empresa.Configuracao.Codigo, cliente.Localidade.Estado.Sigla);

                if (estadoDeEmissaoSerie != null)
                    serie = estadoDeEmissaoSerie.Serie;

                if (serie == null)
                {
                    if (empresa.Configuracao.SerieInterestadual != null && cliente.Localidade.Estado.Sigla != empresa.Localidade.Estado.Sigla)
                    {
                        serie = empresa.Configuracao.SerieInterestadual;
                    }
                    else if (empresa.Configuracao.SerieIntraestadual != null && cliente.Localidade.Estado.Sigla == empresa.Localidade.Estado.Sigla)
                    {
                        serie = empresa.Configuracao.SerieIntraestadual;
                    }
                }
            }

            if (serie == null)
            {
                serie = repCTe.BuscarSerieUltimoCTeEmitido(codigoEmpresa, null);

                if (serie == null)
                    serie = repSerie.BuscarUltimoRegistro(codigoEmpresa, null);
            }

            if (serie == null)
                serie = repSerie.BuscarPorEmpresaTipo(codigoEmpresa, Dominio.Enumeradores.TipoSerie.CTe);

            return serie.Codigo;
        }

        private object ObterUnidadeMedida(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.UnidadeDeMedida repUnidadeMedida = new Repositorio.UnidadeDeMedida(unitOfWork);

            Dominio.Entidades.UnidadeDeMedida unidadeMedida = repUnidadeMedida.BuscarPorCodigoUnidade("01"); //código do KG

            if (unidadeMedida == null)
                return new { Codigo = 0, Descricao = string.Empty };

            return new { Codigo = unidadeMedida.Codigo, Descricao = unidadeMedida.Descricao };
        }

        private object ObterUnidadeMedidaUN(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.UnidadeDeMedida repUnidadeMedida = new Repositorio.UnidadeDeMedida(unitOfWork);

            Dominio.Entidades.UnidadeDeMedida unidadeMedida = repUnidadeMedida.BuscarPorCodigoUnidade("03"); //código da UN

            if (unidadeMedida == null)
                return new { Codigo = 0, Descricao = string.Empty };

            return new { Codigo = unidadeMedida.Codigo, Descricao = unidadeMedida.Descricao };
        }

        private void PreencherDadosFornecedor(Dominio.Entidades.Cliente cliente, Repositorio.UnitOfWork unitOfWork, bool possuiFornecedor)
        {
            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = RetornarModalidadePessoa(cliente, TipoModalidade.Fornecedor, unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoas = repModalidadeFornecedorPessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

            if (modalidadeFornecedorPessoas == null)
                modalidadeFornecedorPessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas();
            else
                modalidadeFornecedorPessoas.Initialize();

            modalidadeFornecedorPessoas.ModalidadePessoas = modalidadePessoas;

            if (modalidadeFornecedorPessoas.Codigo == 0)
            {
                repModalidadeFornecedorPessoas.Inserir(modalidadeFornecedorPessoas, null);
            }
            else
            {
                repModalidadeFornecedorPessoas.Atualizar(modalidadeFornecedorPessoas, null);
            }
        }

        private Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas RetornarModalidadePessoa(Dominio.Entidades.Cliente cliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade tipoModalidade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = repModalidadePessoas.BuscarPorTipo(tipoModalidade, cliente.CPF_CNPJ);

            if (modalidadePessoas == null)
            {
                modalidadePessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas();
                modalidadePessoas.TipoModalidade = tipoModalidade;
                modalidadePessoas.Cliente = cliente;
                repModalidadePessoas.Inserir(modalidadePessoas);
            }

            return modalidadePessoas;
        }
        
        #endregion  
    }
}

