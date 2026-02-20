using Repositorio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Servicos
{
    public class Natura : ServicoBase
    {        
        public Natura(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #region Métodos Globais

        public void ProcessarLinha(string linha, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscalDocumentoTransporteNatura = new Repositorio.NotaFiscalDocumentoTransporteNatura(unitOfWork);
            Repositorio.DocumentoTransporteNatura repDocumentoTransporteNatura = new Repositorio.DocumentoTransporteNatura(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            string[] splitLinha = linha.Split('|');

            if (splitLinha.Length > 1)
            {
                long numeroDT;
                bool isNumeric = long.TryParse(splitLinha[1], out numeroDT);
                if (isNumeric)
                {
                    string codigoTranspNatura = splitLinha[3];
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigoTransportadorNatura(codigoTranspNatura);
                    if (empresa != null)
                    {
                        Dominio.Entidades.DocumentoTransporteNatura dtNatura = repDocumentoTransporteNatura.BuscarPorNumero(empresa.Codigo, numeroDT);
                        if (dtNatura == null)
                        {
                            dtNatura = new Dominio.Entidades.DocumentoTransporteNatura();
                            dtNatura.NumeroDT = numeroDT;
                            dtNatura.DataEmissao = new DateTime(int.Parse(splitLinha[9].Split('.')[2]), int.Parse(splitLinha[9].Split('.')[1]), int.Parse(splitLinha[9].Split('.')[0]), int.Parse(splitLinha[10].Split(':')[0]), int.Parse(splitLinha[10].Split(':')[1]), int.Parse(splitLinha[10].Split(':')[1]));
                            dtNatura.DataConsulta = DateTime.Now;
                            dtNatura.Empresa = empresa;
                            dtNatura.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura.WebService;
                            dtNatura.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmDigitacao;
                            repDocumentoTransporteNatura.Inserir(dtNatura);
                        }
                        string chave = splitLinha[11];

                        Dominio.Entidades.NotaFiscalDocumentoTransporteNatura nfNatura = repNotaFiscalDocumentoTransporteNatura.BuscarPorChaveNFe(empresa.Codigo, chave);
                        if (nfNatura == null)
                        {
                            nfNatura = new Dominio.Entidades.NotaFiscalDocumentoTransporteNatura();
                            nfNatura.DocumentoTransporte = dtNatura;
                            nfNatura.Chave = chave;
                            nfNatura.DataEmissao = new DateTime(int.Parse(splitLinha[25].Split('.')[2]), int.Parse(splitLinha[25].Split('.')[1]), int.Parse(splitLinha[25].Split('.')[0]));
                            dtNatura.DataConsulta = DateTime.Now;

                            ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDadosNfeInformacoesPedidoDestinatario destinatario = new ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDadosNfeInformacoesPedidoDestinatario();
                            destinatario.bairro = splitLinha[47].Trim();
                            destinatario.cep = splitLinha[52].Trim();
                            destinatario.cnpj = splitLinha[43].Trim();
                            destinatario.codMunicipio = splitLinha[49].Trim();
                            destinatario.codPais = splitLinha[53].Trim();
                            destinatario.cpf = splitLinha[44].Trim();
                            destinatario.descMunicipio = splitLinha[50].Trim();
                            destinatario.descPais = splitLinha[54].Trim();
                            destinatario.inscricaoEstadual = splitLinha[56].Trim();
                            destinatario.logradouro = splitLinha[46].Trim();
                            destinatario.nomeDestinatario = splitLinha[45].Trim();
                            destinatario.numero = splitLinha[48].Trim();
                            destinatario.telefone = splitLinha[55].Trim();
                            destinatario.uf = splitLinha[51].Trim();
                            nfNatura.Destinatario = ObterDestinatarioNotaFiscalContingencia(empresa.Codigo, destinatario, unitOfWork);

                            ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDadosNfeInformacoesPedidoEmitente emitente = new ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDadosNfeInformacoesPedidoEmitente();
                            emitente.bairro = splitLinha[32].Trim();
                            emitente.cep = splitLinha[36].Trim();
                            emitente.cnae = splitLinha[41].Trim();
                            emitente.cnpj = splitLinha[27].Trim();
                            emitente.codMunicipio = splitLinha[33].Trim();
                            emitente.crt = splitLinha[42].Trim();
                            emitente.descMunicipio = splitLinha[35].Trim();
                            emitente.inscricaoEstadual = splitLinha[39].Trim();
                            emitente.inscricaoMunicipal = splitLinha[40].Trim();
                            emitente.logradouro = splitLinha[30].Trim();
                            emitente.nomeEmitente = splitLinha[28].Trim();
                            emitente.nomeFantasia = splitLinha[29].Trim();
                            emitente.numero = splitLinha[31].Trim();
                            emitente.pais = splitLinha[37].Trim();
                            emitente.telefone = splitLinha[38].Trim();
                            emitente.uf = splitLinha[35].Trim();
                            nfNatura.Emitente = ObterEmitenteNotaFiscalContingencia(empresa.Codigo, emitente, unitOfWork);

                            nfNatura.Numero = int.Parse(splitLinha[24]);
                            nfNatura.Peso = decimal.Parse(splitLinha[65].Trim());
                            nfNatura.Quantidade = !string.IsNullOrEmpty(splitLinha[64].Trim()) ? int.Parse(splitLinha[64].Trim()) : 0;
                            nfNatura.Serie = int.Parse(splitLinha[23]);
                            nfNatura.Status = Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Pendente;
                            nfNatura.TipoPagamento = splitLinha[63].Trim() == "FOB" ? Dominio.Enumeradores.TipoPagamento.A_Pagar : Dominio.Enumeradores.TipoPagamento.Pago;
                            nfNatura.Valor = decimal.Parse(splitLinha[26].Trim());
                            nfNatura.ValorFrete = decimal.Parse(splitLinha[70].Trim());
                            repNotaFiscalDocumentoTransporteNatura.Inserir(nfNatura);
                        }
                        else
                        {
                            Servicos.Log.TratarErro("Nota (" + chave + ") já enviada para a DT " + nfNatura.DocumentoTransporte.NumeroDT);
                        }
                    }
                    else
                    {
                        Servicos.Log.TratarErro("Não encontrou o codigo transportador " + codigoTranspNatura);
                    }
                }
            }
        }

        public void GerarDocumentosTransporteViaArquivoContingencia(Stream arquivo, Repositorio.UnitOfWork unitOfWork)
        {

            System.Text.Encoding encoding = Servicos.Arquivo.GetEncodingOrDefault(arquivo);
            StreamReader sr = new StreamReader(arquivo, encoding);

            while (!sr.EndOfStream)
            {
                string linha = sr.ReadLine();
                ProcessarLinha(linha, unitOfWork);
            }
        }

        public void ConsultarDocumentosTransporte(int codigoEmpresa, long numeroDT, DateTime dataInicial, DateTime dataFinal, Repositorio.UnitOfWork unidadeDeTrabalho, int codigoUsuario = 0, bool atualizarDT = false, Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura statusPadrao = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmDigitacao)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            bool autorizaValorZerado = false;
            bool excluiuDT = false;
            string xmlConsultaDT = null;
            string xmlDT = null;

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.DocumentoTransporteNatura repDocumentoTransporte = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);
            Repositorio.NaturaXML repNaturaXML = new Repositorio.NaturaXML(unidadeDeTrabalho);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            Dominio.Entidades.NaturaXML naturaXML = new Dominio.Entidades.NaturaXML();

            naturaXML.Data = DateTime.Now;
            naturaXML.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.ConsultaDocumentoTransporte;
            naturaXML.Usuario = codigoUsuario == 0 ? repUsuario.BuscarPrimeiroPorEmpresa(empresa.Codigo, Dominio.Enumeradores.TipoAcesso.Emissao) : repUsuario.BuscarPorCodigo(codigoUsuario);
            repNaturaXML.Inserir(naturaXML);

            ServicoNatura.ProcessaConsultaNF.SI_ProcessaConsultaNFSync_OBClient svcConsultaNF = ObterClientNatura<ServicoNatura.ProcessaConsultaNF.SI_ProcessaConsultaNFSync_OBClient, ServicoNatura.ProcessaConsultaNF.SI_ProcessaConsultaNFSync_OB>(empresa.Configuracao.UsuarioNatura, empresa.Configuracao.SenhaNatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Natura_SI_ProcessaConsultaNF, unidadeDeTrabalho, out Servicos.Models.Integracao.InspectorBehavior inspectorProcessaConsultaNF);
            ServicoNatura.RecebeNotasFiscais.SI_RecebeNotaisFiscaisSync_OBClient svcRecebeNotasFiscais = ObterClientNatura<ServicoNatura.RecebeNotasFiscais.SI_RecebeNotaisFiscaisSync_OBClient, ServicoNatura.RecebeNotasFiscais.SI_RecebeNotaisFiscaisSync_OB>(empresa.Configuracao.UsuarioNatura, empresa.Configuracao.SenhaNatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Natura_SI_RecebeNotaisFiscais, unidadeDeTrabalho, out Servicos.Models.Integracao.InspectorBehavior inspectorRecebeNotasFiscais);

            var dadosConsulta = new ServicoNatura.ProcessaConsultaNF.DT_EnviaParamConsultaNFDados()
            {
                codTranspEmit = empresa.Configuracao.CodigoFilialNatura,
                codTranspMatriz = empresa.Configuracao.CodigoMatrizNatura
            };

            if (numeroDT > 0)
            {
                dadosConsulta.numTransporte = numeroDT.ToString();
                autorizaValorZerado = true;
            }
            else
            {
                if (dataFinal != DateTime.MinValue)
                    dadosConsulta.dataAte = dataFinal.ToString("yyyy-MM-dd");

                if (dataInicial != DateTime.MinValue)
                    dadosConsulta.dataDe = dataInicial.ToString("yyyy-MM-dd");
            }

            var retornoConsulta = svcConsultaNF.SI_ProcessaConsultaNFSync_OB(new ServicoNatura.ProcessaConsultaNF.DT_EnviaParamConsultaNF() { dados = dadosConsulta });
            xmlConsultaDT = inspectorProcessaConsultaNF.LastRequestXML;

            naturaXML.XMLEnvio = xmlConsultaDT;
            repNaturaXML.Atualizar(naturaXML);

            ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDados[] retornoNotasFiscais = null;
            int countExec = 0;
            do
            {
                if (countExec > 0)
                    System.Threading.Thread.Sleep(1000);

                retornoNotasFiscais = svcRecebeNotasFiscais.SI_RecebeNotaisFiscaisSync_OB(new ServicoNatura.RecebeNotasFiscais.DT_EnviaParamNotasFiscais()
                {
                    dados = new ServicoNatura.RecebeNotasFiscais.DT_EnviaParamNotasFiscaisDados()
                    {
                        codTranspMatriz = empresa.Configuracao.CodigoMatrizNatura,
                        protocolo = retornoConsulta.dados.protocolo
                    }
                });

                xmlDT = inspectorRecebeNotasFiscais.LastResponseXML;

                //if (numeroDT > 0 && xmlDT != null)
                //    Servicos.Log.TratarErro(string.Concat("DT ", numeroDT.ToString(), " XML Retorno ", xmlDT));

                countExec++;

            } while (retornoNotasFiscais[0].mensagem != null && retornoNotasFiscais[0].mensagem.ToLower() == "solicitação em processamento" && countExec < 50);

            naturaXML.XMLRetorno = xmlDT;
            repNaturaXML.Atualizar(naturaXML);

            var erroImportacaoDT = false;
            foreach (ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDados retorno in retornoNotasFiscais) //para cada documento de transporte (DT)
            {
                if (retorno.documentoTransporte != null)
                {
                    long numero = long.Parse(retorno.documentoTransporte);
                    DateTime dataEmissao = DateTime.Now;
                    DateTime.TryParse(retorno.data, out dataEmissao);

                    Dominio.Entidades.DocumentoTransporteNatura dtNatura = repDocumentoTransporte.BuscarPorNumeroNaoCancelado(codigoEmpresa, numero);

                    if (dtNatura == null || (atualizarDT && dtNatura.Status == Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmDigitacao) || (atualizarDT && dtNatura.Status == Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Erro))
                    {
                        if (atualizarDT && dtNatura != null && (dtNatura.Status == Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmDigitacao || dtNatura.Status == Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Erro))
                            excluiuDT = ExcluirDT(dtNatura, unidadeDeTrabalho);

                        if ((atualizarDT && excluiuDT) || dtNatura == null)
                        {
                            bool valorFreteNegativo = false;
                            decimal valorFrete = 0;
                            for (var i = 0; i < retorno.nfe.Count(); i++)
                            {
                                valorFrete += !string.IsNullOrWhiteSpace(retorno.nfe[i].informacoesTransporte.valorFrete) ? decimal.Parse(retorno.nfe[i].informacoesTransporte.valorFrete, cultura) : 0m;
                                if (valorFrete == 0)
                                    valorFreteNegativo = true;
                            }

                            if (autorizaValorZerado || !valorFreteNegativo)
                            {
                                if (valorFreteNegativo)
                                    Servicos.Log.TratarErro("DT Natura " + numero.ToString() + " com valor de frete zerado.");

                                unidadeDeTrabalho.Start();

                                Dominio.Entidades.DocumentoTransporteNatura documentoTransporte = new Dominio.Entidades.DocumentoTransporteNatura();
                                var erroDT = "";
                                try
                                {
                                    documentoTransporte.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura.WebService;
                                    documentoTransporte.Empresa = empresa;
                                    documentoTransporte.NumeroDT = numero;
                                    documentoTransporte.DataEmissao = dataEmissao;
                                    documentoTransporte.DataConsulta = DateTime.Now;
                                    documentoTransporte.Status = statusPadrao;// Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmDigitacao;
                                    documentoTransporte.NaturaXMLs = new List<Dominio.Entidades.NaturaXML>();
                                    documentoTransporte.NaturaXMLs.Add(naturaXML);

                                    repDocumentoTransporte.Inserir(documentoTransporte);

                                    unidadeDeTrabalho.CommitChanges();
                                    unidadeDeTrabalho.Start();

                                    for (var i = 0; i < retorno.nfe.Count(); i++)
                                    {
                                        if (string.IsNullOrWhiteSpace(retorno.nfe[i].xmlNFe))
                                            erroDT = string.Concat(erroDT + " XML NF-e inexistente no DT.");
                                        if (string.IsNullOrWhiteSpace(retorno.nfe[i].chaveNFe))
                                            erroDT = string.Concat(erroDT + " Chave NF-e inexistente no DT.");
                                        if (retorno.nfe[i].informacoesPedido.emitente == null)
                                            erroDT = string.Concat(erroDT + " Emitente inexistente no DT.");
                                        if (retorno.nfe[i].informacoesPedido.destinatario == null)
                                            erroDT = string.Concat(erroDT + " Destinatário inexistente no DT.");

                                        if (!string.IsNullOrWhiteSpace(retorno.nfe[i].xmlNFe)) //POSSUI XML DE NF-e
                                        {
                                            SalvarNotaFiscal(documentoTransporte, retorno.nfe[i], unidadeDeTrabalho);
                                        }
                                        else //NOTA EM CONTINGENCIA (não vem XML, só os dados principais)
                                        {
                                            SalvarNotaFiscalContingencia(documentoTransporte, retorno.nfe[i], unidadeDeTrabalho);
                                        }
                                    }

                                    unidadeDeTrabalho.CommitChanges();

                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro("DT Natura " + numero.ToString() + ": " + ex);

                                    if (empresa.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.Configuracao.EmailsNotificacaoNatura))
                                    {
                                        string assunto = "DT " + numero.ToString() + " falha na importação.";

                                        System.Text.StringBuilder st = new System.Text.StringBuilder();
                                        st.Append("Transportador: " + empresa.CNPJ_Formatado + " " + empresa.RazaoSocial).AppendLine();
                                        st.Append("DT Natura: " + numero.ToString()).AppendLine();
                                        st.Append("Motivo: " + erroDT).AppendLine();

                                        this.NotificarEmail(assunto, st, empresa.Configuracao.EmailsNotificacaoNatura, unidadeDeTrabalho);
                                    }

                                    unidadeDeTrabalho.Rollback();

                                    if (documentoTransporte != null || documentoTransporte.Codigo > 0)
                                    {
                                        documentoTransporte.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Erro;
                                        if (documentoTransporte.NaturaXMLs == null)
                                        {
                                            documentoTransporte.NaturaXMLs = new List<Dominio.Entidades.NaturaXML>();
                                            documentoTransporte.NaturaXMLs.Add(naturaXML);
                                        }
                                        documentoTransporte.Observacao = erroDT;
                                        repDocumentoTransporte.Atualizar(documentoTransporte);
                                    }

                                    erroImportacaoDT = true;
                                }
                            }
                            else
                            {
                                Dominio.Entidades.DocumentoTransporteNatura documentoTransporte = new Dominio.Entidades.DocumentoTransporteNatura();
                                documentoTransporte.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura.WebService;
                                documentoTransporte.Empresa = empresa;
                                documentoTransporte.NumeroDT = numero;
                                documentoTransporte.DataEmissao = dataEmissao;
                                documentoTransporte.DataConsulta = DateTime.Now;
                                documentoTransporte.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Erro;
                                if (documentoTransporte.NaturaXMLs == null)
                                {
                                    documentoTransporte.NaturaXMLs = new List<Dominio.Entidades.NaturaXML>();
                                    documentoTransporte.NaturaXMLs.Add(naturaXML);
                                }
                                documentoTransporte.Observacao = "DT possui notas com valor de frete zerado.";
                                repDocumentoTransporte.Inserir(documentoTransporte);

                                if (empresa.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.Configuracao.EmailsNotificacaoNatura))
                                {
                                    string assunto = "DT " + numero.ToString() + " possui notas com valor de frete zerado";

                                    System.Text.StringBuilder st = new System.Text.StringBuilder();
                                    st.Append("Transportador: " + empresa.CNPJ_Formatado + " " + empresa.RazaoSocial).AppendLine();
                                    st.Append("DT Natura: " + numero.ToString()).AppendLine();
                                    st.Append("Motivo: Notas com valor de frete zerado.").AppendLine();

                                    this.NotificarEmail(assunto, st, empresa.Configuracao.EmailsNotificacaoNatura, unidadeDeTrabalho);
                                }
                            }
                        }
                    }
                }
            }
            if (erroImportacaoDT)
                throw new Exception("Erro importando DT");
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> EmitirCTes(int codigoDocumentoTransporte, string observacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.DocumentoTransporteNatura repDocumento = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);

            Dominio.Entidades.DocumentoTransporteNatura documento = repDocumento.BuscarPorCodigo(codigoDocumentoTransporte);

            if (documento.Status != Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmDigitacao && documento.Status != Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.AguardandoEmissaoAutomatica)
                return null;

            Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscal = new Repositorio.NotaFiscalDocumentoTransporteNatura(unidadeDeTrabalho);

            List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> notas = repNotaFiscal.BuscarPorDocumentoTransporte(documento.Codigo);

            List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> notasFiscais = null;

            notasFiscais = notas;

            decimal valorFretePorCTe = Math.Floor((documento.ValorFrete / notasFiscais.Count()) * 100) / 100;
            decimal valorFreteUltimoCTe = documento.ValorFrete - (valorFretePorCTe * (notasFiscais.Count() - 1));

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            CTe svcCTe = new CTe(unidadeDeTrabalho);

            for (int i = 0; i < notasFiscais.Count(); i++)
            {
                if (notasFiscais[i].Status == Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Pendente)
                {
                    unidadeDeTrabalho.Start();

                    if (notasFiscais.Count() == (i + 1))
                        notasFiscais[i].ValorFrete = valorFreteUltimoCTe;
                    else
                        notasFiscais[i].ValorFrete = valorFretePorCTe;

                    repNotaFiscal.Atualizar(notasFiscais[i]);

                }
            }

            var grupoNotas = (from obj in notasFiscais group obj by new { Emitente = obj.Emitente.CPF_CNPJ, Destinatario = obj.Destinatario.CPF_CNPJ } into grupo select grupo.Key);

            foreach (var grupoNota in grupoNotas)
            {
                var notasDoGrupo = (from obj in notasFiscais where obj.Emitente.CPF_CNPJ == grupoNota.Emitente && obj.Destinatario.CPF_CNPJ == grupoNota.Destinatario && obj.Status == Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Pendente select obj).ToList();

                if (notasDoGrupo.Count > 0)
                {
                    if ((documento.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura.WebService) ||
                        (documento.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura.FTP && notasDoGrupo.FirstOrDefault().Emitente.Localidade.Codigo != notasDoGrupo.FirstOrDefault().Destinatario.Localidade.Codigo))
                    {
                        unidadeDeTrabalho.Start();

                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = GerarCTe(notasDoGrupo, observacao, unidadeDeTrabalho);

                        ctes.Add(cte);

                        foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscal in notasDoGrupo)
                        {
                            notaFiscal.CTe = cte;
                            notaFiscal.Status = Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Emitido;

                            repNotaFiscal.Atualizar(notaFiscal);
                        }

                        unidadeDeTrabalho.CommitChanges();

                        svcCTe.Emitir(cte.Codigo, cte.Empresa.Codigo, unidadeDeTrabalho);
                    }
                }
            }

            //documento.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Emitido;
            //repDocumento.Atualizar(documento);

            return ctes;
        }

        public List<Dominio.Entidades.NFSe> EmitirNFSe(int codigoDocumentoTransporte, UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.DocumentoTransporteNatura repDocumento = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);

            Dominio.Entidades.DocumentoTransporteNatura documento = repDocumento.BuscarPorCodigo(codigoDocumentoTransporte);

            if (documento.Status != Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmDigitacao && documento.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura.WebService)
                return null;

            Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscal = new Repositorio.NotaFiscalDocumentoTransporteNatura(unidadeDeTrabalho);

            List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> notas = repNotaFiscal.BuscarPorDocumentoTransporte(documento.Codigo);
            List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> notasFiscais = null;

            notasFiscais = notas;

            decimal valorFretePorNFSe = Math.Floor((documento.ValorFrete / notasFiscais.Count()) * 100) / 100;
            decimal valorFreteUltimaNFSe = documento.ValorFrete - (valorFretePorNFSe * (notasFiscais.Count() - 1));

            List<Dominio.Entidades.NFSe> nfses = new List<Dominio.Entidades.NFSe>();

            for (int i = 0; i < notasFiscais.Count(); i++)
            {
                if (notasFiscais[i].Status == Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Pendente)
                {
                    if ((documento.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura.WebService) ||
                        (documento.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura.FTP && notasFiscais[i].Emitente.Localidade.Codigo == notasFiscais[i].Destinatario.Localidade.Codigo))
                    {
                        unidadeDeTrabalho.Start();

                        if (notasFiscais.Count() == (i + 1))
                            notasFiscais[i].ValorFrete = valorFreteUltimaNFSe;
                        else
                            notasFiscais[i].ValorFrete = valorFretePorNFSe;

                        repNotaFiscal.Atualizar(notasFiscais[i]);

                        Dominio.Entidades.NFSe nfse = GerarNFSe(notasFiscais[i], unidadeDeTrabalho);

                        nfses.Add(nfse);

                        notasFiscais[i].NFSe = nfse;
                        notasFiscais[i].Status = Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Emitido;

                        repNotaFiscal.Atualizar(notasFiscais[i]);

                        unidadeDeTrabalho.CommitChanges();
                    }
                }
            }

            return nfses;
        }

        public ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaResponseDados[] EmitirFatura(int codigoFatura, Repositorio.UnitOfWork unitOfWork, int codigoUsuario = 0)
        {

            Repositorio.FaturaNatura repFatura = new Repositorio.FaturaNatura(unitOfWork);
            Repositorio.NaturaXML repNaturaXML = new Repositorio.NaturaXML(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.ItemFaturaNatura repItemFatura = new Repositorio.ItemFaturaNatura(unitOfWork);

            Dominio.Entidades.FaturaNatura fatura = repFatura.BuscarPorCodigo(codigoFatura);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(fatura.Empresa.Codigo);
            Dominio.Entidades.NaturaXML naturaXML = new Dominio.Entidades.NaturaXML();
            List<Dominio.Entidades.ItemFaturaNatura> itensFatura = repItemFatura.BuscarPorFatura(fatura.Codigo);

            ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaResponseDados[] retorno = null;

            if (fatura.Status != Dominio.ObjetosDeValor.Enumerador.StatusFaturaNatura.Emitida && fatura.Status != Dominio.ObjetosDeValor.Enumerador.StatusFaturaNatura.Pendente)
                return null;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Servicos.Models.Integracao.InspectorBehavior inspector = new Models.Integracao.InspectorBehavior();
            string mensagemRetorno = string.Empty;

            bool utilizarWebServiceNaturaNovo = true;

            if (utilizarWebServiceNaturaNovo)
            {
                List<ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItens> itens = new List<ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItens>();

                itens = (from obj in itensFatura
                         where (obj.NotaFiscal.CTe != null && obj.NotaFiscal.CTe.Status.Equals("A")) || (obj.NotaFiscal.NFSe != null && obj.NotaFiscal.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado)
                         select new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItens()
                         {
                             chaveCTe = obj.NotaFiscal.CTe != null ? obj.NotaFiscal.CTe.Chave : string.Empty,
                             codTranspEmit = obj.NotaFiscal.CTe != null ? obj.NotaFiscal.CTe.Empresa.Configuracao.CodigoFilialNatura : obj.NotaFiscal.NFSe.Empresa.Configuracao.CodigoFilialNatura,
                             dataDocFiscal = obj.NotaFiscal.CTe?.DataEmissao.Value ?? obj.NotaFiscal.NFSe.DataEmissao,
                             docFiscalRef = obj.NotaFiscal.CTe != null && obj.NotaFiscal.CTe.Documentos.Count() > 0 && obj.NotaFiscal.CTe.Documentos.FirstOrDefault().ChaveNFE != null ?
                                            (from documento in obj.NotaFiscal.CTe.Documentos
                                             select new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItensDocFiscalRef()
                                             {
                                                 chaveNFeCTe = documento.ChaveNFE,
                                                 dataEmissao = documento.DataEmissao,
                                                 modelo = documento.ModeloDocumentoFiscal.Numero,
                                                 numero = documento.Numero,
                                                 serie = !string.IsNullOrWhiteSpace(documento.Serie) ? documento.Serie : obj.NotaFiscal.Serie.ToString()
                                             }).ToArray() :
                                            new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItensDocFiscalRef[] { new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItensDocFiscalRef() {
                                             chaveNFeCTe = obj.NotaFiscal.Chave,
                                             dataEmissao = obj.NotaFiscal.DataEmissao.Value,
                                             modelo = "55",
                                             numero = obj.NotaFiscal.Numero.ToString(),
                                             serie = obj.NotaFiscal.Serie.ToString()
                                        } },
                             impostos = new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItensImpostos()
                             {
                                 aliquotaCOFINS = obj.AliquotaCOFINS.ToString("0.00", cultura),
                                 aliquotaICMS = obj.AliquotaICMS.ToString("0.00", cultura),
                                 aliquotaICMSST = obj.AliquotaICMSST.ToString("0.00", cultura),
                                 aliquotaISS = obj.AliquotaISS.ToString("0.00", cultura),
                                 aliquotaPIS = obj.AliquotaPIS.ToString("0.00", cultura),
                                 baseCOFINS = obj.BaseCalculoCOFINS.ToString("0.00", cultura),
                                 baseICMS = obj.BaseCalculoICMS.ToString("0.00", cultura),
                                 baseICMSST = obj.BaseCalculoICMSST.ToString("0.00", cultura),
                                 baseISS = obj.ValorISS == 0 ? obj.ValorISS.ToString("0.00", cultura) : obj.BaseCalculoISS.ToString("0.00", cultura), //Solicitado pela Natura quando for ISS Zerado mandar a Base ISS Zerada (Empresas do Simples)
                                 basePIS = obj.BaseCalculoPIS.ToString("0.00", cultura),
                                 ivaICMSST = obj.IVAICMSST.ToString("0.00", cultura),
                                 valorCOFINS = obj.ValorCOFINS.ToString("0.00", cultura),
                                 valorICMS = obj.ValorICMS.ToString("0.00", cultura),
                                 valorICMSST = obj.ValorICMSST.ToString("0.00", cultura),
                                 valorISS = obj.ValorISS.ToString("0.00", cultura),
                                 valorPIS = obj.ValorPIS.ToString("0.00", cultura)
                             },
                             modeloDocFiscal = obj.NotaFiscal.CTe != null ? "57" : "07",
                             numeroDocFiscal = obj.NotaFiscal.CTe?.Numero.ToString() ?? obj.NotaFiscal.NFSe.Numero.ToString(),
                             serieDocFiscal = obj.NotaFiscal.CTe != null ? string.Format("{0:000}", obj.NotaFiscal.CTe.Serie.Numero) : obj.NotaFiscal.NFSe != null ? string.Format("{0:000}", obj.NotaFiscal.NFSe.Serie.Numero) : "000",
                             transporte = new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDadosItensTransporte()
                             {
                                 difValorFrete = obj.ValorDoDesconto.ToString("0.00", cultura),
                                 docTransporte = obj.NotaFiscal.DocumentoTransporte.NumeroDT.ToString(),
                                 valorFrete = obj.NotaFiscal.CTe != null ? obj.NotaFiscal.CTe.ValorAReceber.ToString("0.00", cultura) : obj.NotaFiscal.NFSe != null ? obj.NotaFiscal.NFSe.ValorServicos.ToString("0.00", cultura) : "0.00"
                             }
                         }).ToList();

                ServicoNaturaNovo.ProcessaFatura.SI_ProcessaFatura_SyncClient svcFatura = ObterClientNatura<ServicoNaturaNovo.ProcessaFatura.SI_ProcessaFatura_SyncClient, ServicoNaturaNovo.ProcessaFatura.SI_ProcessaFatura_Sync>(fatura.Empresa.Configuracao.UsuarioNatura, fatura.Empresa.Configuracao.SenhaNatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Natura_Novo_SI_ProcessaFatura, unitOfWork, out inspector);

                var dados = new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaDados()
                {
                    codTranspMatriz = fatura.Empresa.Configuracao.CodigoMatrizNatura,
                    dataFatura = fatura.DataEmissao.Value,
                    dataPreFatura = fatura.DataPreFatura,
                    dataPreFaturaSpecified = true,
                    dataVencFatura = fatura.DataVencimento.Value,
                    numeroFatura = fatura.Numero.ToString(),
                    numeroPreFatura = fatura.NumeroPreFatura.ToString(),
                    itens = itens.ToArray()
                };

                try
                {
                    retorno = svcFatura.SI_ProcessaFatura_Sync(new ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFatura()
                    {
                        dados = dados
                    });
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Erro Enviando Fatura " + ex);

                    var xmlEnvioErro = inspector.LastRequestXML;
                    Servicos.Log.TratarErro("XML Envio Fatura " + xmlEnvioErro);
                    var xmlRetornoErro = inspector.LastResponseXML;
                    Servicos.Log.TratarErro("XML Retorno Envio Fatura " + xmlRetornoErro);

                    throw;
                }

                if (retorno != null)
                    mensagemRetorno = string.Join(" / ", from obj in retorno select obj.number + " - " + obj.message);
            }
            else
            {
                List<ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItens> itens = new List<ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItens>();

                itens = (from obj in itensFatura
                         where (obj.NotaFiscal.CTe != null && obj.NotaFiscal.CTe.Status.Equals("A")) || (obj.NotaFiscal.NFSe != null && obj.NotaFiscal.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado)
                         select new ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItens()
                         {
                             chaveCTe = obj.NotaFiscal.CTe?.Chave,
                             codTranspEmit = obj.NotaFiscal.CTe != null ? obj.NotaFiscal.CTe.Empresa.Configuracao.CodigoFilialNatura : obj.NotaFiscal.NFSe.Empresa.Configuracao.CodigoFilialNatura,
                             dataDocFiscal = obj.NotaFiscal.CTe?.DataEmissao.Value ?? obj.NotaFiscal.NFSe.DataEmissao,
                             docFiscalRef = obj.NotaFiscal.CTe != null && obj.NotaFiscal.CTe.Documentos.FirstOrDefault().ChaveNFE != null ?
                                            (from documento in obj.NotaFiscal.CTe.Documentos
                                             select new ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensDocFiscalRef()
                                             {
                                                 chaveNFeCTe = documento.ChaveNFE,
                                                 dataEmissao = documento.DataEmissao,
                                                 modelo = documento.ModeloDocumentoFiscal.Numero,
                                                 numero = documento.Numero,
                                                 serie = !string.IsNullOrWhiteSpace(documento.Serie) ? documento.Serie : obj.NotaFiscal.Serie.ToString()
                                             }).ToArray() :
                                            new ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensDocFiscalRef[] { new ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensDocFiscalRef() {
                                             chaveNFeCTe = obj.NotaFiscal.Chave,
                                             dataEmissao = obj.NotaFiscal.DataEmissao.Value,
                                             modelo = "55",
                                             numero = obj.NotaFiscal.Numero.ToString(),
                                             serie = obj.NotaFiscal.Serie.ToString()
                                        } },
                             impostos = new ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensImpostos()
                             {
                                 aliquotaCOFINS = obj.AliquotaCOFINS.ToString("0.00", cultura),
                                 aliquotaICMS = obj.AliquotaICMS.ToString("0.00", cultura),
                                 aliquotaICMSST = obj.AliquotaICMSST.ToString("0.00", cultura),
                                 aliquotaISS = obj.AliquotaISS.ToString("0.00", cultura),
                                 aliquotaPIS = obj.AliquotaPIS.ToString("0.00", cultura),
                                 baseCOFINS = obj.BaseCalculoCOFINS.ToString("0.00", cultura),
                                 baseICMS = obj.BaseCalculoICMS.ToString("0.00", cultura),
                                 baseICMSST = obj.BaseCalculoICMSST.ToString("0.00", cultura),
                                 baseISS = obj.BaseCalculoISS.ToString("0.00", cultura),
                                 basePIS = obj.BaseCalculoPIS.ToString("0.00", cultura),
                                 ivaICMSST = obj.IVAICMSST.ToString("0.00", cultura),
                                 valorCOFINS = obj.ValorCOFINS.ToString("0.00", cultura),
                                 valorICMS = obj.ValorICMS.ToString("0.00", cultura),
                                 valorICMSST = obj.ValorICMSST.ToString("0.00", cultura),
                                 valorISS = obj.ValorISS.ToString("0.00", cultura),
                                 valorPIS = obj.ValorPIS.ToString("0.00", cultura)
                             },
                             modeloDocFiscal = obj.NotaFiscal.CTe != null ? "57" : "07",
                             numeroDocFiscal = obj.NotaFiscal.CTe?.Numero.ToString() ?? obj.NotaFiscal.NFSe.Numero.ToString(),
                             serieDocFiscal = obj.NotaFiscal.CTe != null ? string.Format("{0:000}", obj.NotaFiscal.CTe.Serie.Numero) : obj.NotaFiscal.NFSe != null ? string.Format("{0:000}", obj.NotaFiscal.NFSe.Serie.Numero) : "000",
                             transporte = new ServicoNatura.ProcessaFatura.DT_EnviaFaturaDadosItensTransporte()
                             {
                                 difValorFrete = obj.ValorDoDesconto.ToString("0.00", cultura),
                                 docTransporte = obj.NotaFiscal.DocumentoTransporte.NumeroDT.ToString(),
                                 valorFrete = obj.NotaFiscal.CTe != null ? obj.NotaFiscal.CTe.ValorAReceber.ToString("0.00", cultura) : obj.NotaFiscal.NFSe != null ? obj.NotaFiscal.NFSe.ValorServicos.ToString("0.00", cultura) : "0.00"
                             }
                         }).ToList();

                ServicoNatura.ProcessaFatura.SI_ProcessaFaturaAsync_OBClient svcFatura = ObterClientNatura<ServicoNatura.ProcessaFatura.SI_ProcessaFaturaAsync_OBClient, ServicoNatura.ProcessaFatura.SI_ProcessaFaturaAsync_OB>(fatura.Empresa.Configuracao.UsuarioNatura, fatura.Empresa.Configuracao.SenhaNatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Natura_SI_ProcessaFatura, unitOfWork, out inspector);

                svcFatura.SI_ProcessaFaturaAsync_OBAsync(new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFatura()
                {
                    dados = new Servicos.ServicoNatura.ProcessaFatura.DT_EnviaFaturaDados()
                    {
                        codTranspMatriz = fatura.Empresa.Configuracao.CodigoMatrizNatura,
                        dataFatura = fatura.DataEmissao.Value,
                        dataPreFatura = fatura.DataPreFatura,
                        dataPreFaturaSpecified = true,
                        dataVencFatura = fatura.DataVencimento.Value,
                        numeroFatura = fatura.Numero.ToString(),
                        numeroPreFatura = fatura.NumeroPreFatura.ToString(),
                        itens = itens.ToArray()
                    }
                });
            }

            var xmlEnvio = inspector.LastRequestXML;
            Servicos.Log.TratarErro("XML Envio Fatura " + xmlEnvio);
            var xmlRetorno = inspector.LastResponseXML;
            Servicos.Log.TratarErro("XML Retorno Envio Fatura " + xmlRetorno);

            naturaXML.Data = DateTime.Now;
            naturaXML.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.EnvioFatura;
            naturaXML.Usuario = codigoUsuario == 0 ? repUsuario.BuscarPrimeiroPorEmpresa(empresa.Codigo, Dominio.Enumeradores.TipoAcesso.Emissao) : repUsuario.BuscarPorCodigo(codigoUsuario);
            naturaXML.XMLEnvio = xmlEnvio;
            naturaXML.XMLRetorno = xmlRetorno;
            naturaXML.Mensagem = mensagemRetorno;
            repNaturaXML.Inserir(naturaXML);

            fatura.Status = Dominio.ObjetosDeValor.Enumerador.StatusFaturaNatura.Emitida;
            if (fatura.NaturaXMLs == null)
                fatura.NaturaXMLs = new List<Dominio.Entidades.NaturaXML>();
            fatura.NaturaXMLs.Add(naturaXML);
            repFatura.Atualizar(fatura);

            return retorno;
        }

        public void EnviarRetornoDocumentoTransporteCTeComplementar(int codigoDocumentoTransporte, Repositorio.UnitOfWork unitOfWork, int codigoUsuario = 0)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            string xmlEnvioDT = null;
            string xmlRetornoEnvioDT = null;
            string mensagemRetorno = string.Empty;

            Repositorio.DocumentoTransporteNatura repDocumento = new Repositorio.DocumentoTransporteNatura(unitOfWork);
            Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscal = new Repositorio.NotaFiscalDocumentoTransporteNatura(unitOfWork);
            Repositorio.NaturaXML repNaturaXML = new Repositorio.NaturaXML(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            Dominio.Entidades.DocumentoTransporteNatura documento = repDocumento.BuscarPorCodigo(codigoDocumentoTransporte);
            Dominio.Entidades.NaturaXML naturaXML = new Dominio.Entidades.NaturaXML();
            List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> notasFiscais = repNotaFiscal.BuscarPorDocumentoTransporte(codigoDocumentoTransporte);

            bool utilizarWebServiceNaturaNovo = true;

            if (utilizarWebServiceNaturaNovo)
            {
                List<ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscal> documentosEnvio = BuscarDocumentosEnvioNovo(Dominio.Enumeradores.TipoCTE.Complemento, notasFiscais, unitOfWork);

                if (documentosEnvio.Count > 0)
                {
                    naturaXML.Data = DateTime.Now;
                    naturaXML.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.RetornoDocumentoTransporteComplementar;
                    naturaXML.Usuario = codigoUsuario == 0 ? repUsuario.BuscarPrimeiroPorEmpresa(documento.Empresa.Codigo, Dominio.Enumeradores.TipoAcesso.Emissao) : repUsuario.BuscarPorCodigo(codigoUsuario);
                    repNaturaXML.Inserir(naturaXML);

                    EnviarRetornoNatura(documento, notasFiscais, documentosEnvio, unitOfWork, ref xmlEnvioDT, ref xmlRetornoEnvioDT, ref mensagemRetorno);

                    naturaXML.XMLEnvio = xmlEnvioDT;
                    naturaXML.XMLRetorno = xmlRetornoEnvioDT;
                    naturaXML.Mensagem = mensagemRetorno;

                    if (documento.NaturaXMLs == null)
                        documento.NaturaXMLs = new List<Dominio.Entidades.NaturaXML>();

                    documento.NaturaXMLs.Add(naturaXML);
                    repDocumento.Atualizar(documento);

                    foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscal in notasFiscais)
                    {
                        if (notaFiscal.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                        {
                            notaFiscal.Status = Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Retornado;
                            repNotaFiscal.Atualizar(notaFiscal);
                        }
                    }
                }
            }
            else
            {
                List<ServicoNatura.ProcessaCTeNFSe.DT_EnviaCteNfseDadosDocumentoFiscal> documentosEnvio = BuscarDocumentosEnvio(Dominio.Enumeradores.TipoCTE.Complemento, notasFiscais, unitOfWork);

                if (documentosEnvio.Count > 0)
                {
                    naturaXML.Data = DateTime.Now;
                    naturaXML.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.RetornoDocumentoTransporteComplementar;
                    naturaXML.Usuario = codigoUsuario == 0 ? repUsuario.BuscarPrimeiroPorEmpresa(documento.Empresa.Codigo, Dominio.Enumeradores.TipoAcesso.Emissao) : repUsuario.BuscarPorCodigo(codigoUsuario);
                    repNaturaXML.Inserir(naturaXML);

                    EnviarRetornoNatura(documento, notasFiscais, documentosEnvio, unitOfWork, ref xmlEnvioDT, ref xmlRetornoEnvioDT);

                    naturaXML.XMLEnvio = xmlEnvioDT;
                    naturaXML.XMLRetorno = xmlRetornoEnvioDT;

                    if (documento.NaturaXMLs == null)
                        documento.NaturaXMLs = new List<Dominio.Entidades.NaturaXML>();
                    documento.NaturaXMLs.Add(naturaXML);
                    repDocumento.Atualizar(documento);

                    foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscal in notasFiscais)
                    {
                        if (notaFiscal.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                        {
                            notaFiscal.Status = Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Retornado;
                            repNotaFiscal.Atualizar(notaFiscal);
                        }
                    }
                }
            }
        }

        public ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentResponseDados[] EnviarRetornoDocumentoTransporte(int codigoDocumentoTransporte, Repositorio.UnitOfWork unitOfWork, int codigoUsuario = 0)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            string xmlEnvioDT = null;
            string xmlRetornoEnvioDT = null;
            string mensagemRetorno = string.Empty;

            ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentResponseDados[] retorno = null;

            Repositorio.DocumentoTransporteNatura repDocumento = new Repositorio.DocumentoTransporteNatura(unitOfWork);
            Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscal = new Repositorio.NotaFiscalDocumentoTransporteNatura(unitOfWork);
            Repositorio.NaturaXML repNaturaXML = new Repositorio.NaturaXML(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            Dominio.Entidades.DocumentoTransporteNatura documento = repDocumento.BuscarPorCodigo(codigoDocumentoTransporte);

            List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> notasFiscais = repNotaFiscal.BuscarPorDocumentoTransporte(codigoDocumentoTransporte);

            bool utilizarWebServiceNaturaNovo = true;

            if (utilizarWebServiceNaturaNovo)
            {
                List<ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscal> documentosEnvio = BuscarDocumentosEnvioNovo(Dominio.Enumeradores.TipoCTE.Normal, notasFiscais, unitOfWork);

                if (documentosEnvio.Count > 0)
                {
                    Dominio.Entidades.NaturaXML naturaXML = new Dominio.Entidades.NaturaXML();
                    naturaXML.Data = DateTime.Now;
                    naturaXML.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.RetornoDocumentoTransporte;
                    naturaXML.Usuario = codigoUsuario == 0 ? repUsuario.BuscarPrimeiroPorEmpresa(documento.Empresa.Codigo, Dominio.Enumeradores.TipoAcesso.Emissao) : repUsuario.BuscarPorCodigo(codigoUsuario);
                    repNaturaXML.Inserir(naturaXML);

                    retorno = EnviarRetornoNatura(documento, notasFiscais, documentosEnvio, unitOfWork, ref xmlEnvioDT, ref xmlRetornoEnvioDT, ref mensagemRetorno);

                    naturaXML.XMLEnvio = xmlEnvioDT;
                    naturaXML.XMLRetorno = xmlRetornoEnvioDT;
                    naturaXML.Mensagem = mensagemRetorno.Length > 4000 ? mensagemRetorno.Substring(0, 4000) : mensagemRetorno;

                    if (documento.Status != Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Cancelado)
                    {
                        documento.DataRetorno = DateTime.Now;
                        documento.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Retornado;
                        if (documento.NaturaXMLs == null)
                            documento.NaturaXMLs = new List<Dominio.Entidades.NaturaXML>();
                        documento.NaturaXMLs.Add(naturaXML);
                        repDocumento.Atualizar(documento);
                    }

                    foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscal in notasFiscais)
                    {
                        if (notaFiscal.NFSe != null || notaFiscal.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal)
                        {
                            notaFiscal.Status = Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Retornado;
                            repNotaFiscal.Atualizar(notaFiscal);
                        }
                    }
                }
            }
            else
            {
                List<ServicoNatura.ProcessaCTeNFSe.DT_EnviaCteNfseDadosDocumentoFiscal> documentosEnvio = BuscarDocumentosEnvio(Dominio.Enumeradores.TipoCTE.Normal, notasFiscais, unitOfWork);

                if (documentosEnvio.Count > 0)
                {
                    Dominio.Entidades.NaturaXML naturaXML = new Dominio.Entidades.NaturaXML();
                    naturaXML.Data = DateTime.Now;
                    naturaXML.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.RetornoDocumentoTransporte;
                    naturaXML.Usuario = codigoUsuario == 0 ? repUsuario.BuscarPrimeiroPorEmpresa(documento.Empresa.Codigo, Dominio.Enumeradores.TipoAcesso.Emissao) : repUsuario.BuscarPorCodigo(codigoUsuario);
                    repNaturaXML.Inserir(naturaXML);

                    EnviarRetornoNatura(documento, notasFiscais, documentosEnvio, unitOfWork, ref xmlEnvioDT, ref xmlRetornoEnvioDT);

                    naturaXML.XMLEnvio = xmlEnvioDT;
                    naturaXML.XMLRetorno = xmlRetornoEnvioDT;

                    if (documento.Status != Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Cancelado)
                    {
                        documento.DataRetorno = DateTime.Now;
                        documento.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Retornado;
                        if (documento.NaturaXMLs == null)
                            documento.NaturaXMLs = new List<Dominio.Entidades.NaturaXML>();
                        documento.NaturaXMLs.Add(naturaXML);
                        repDocumento.Atualizar(documento);
                    }

                    foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscal in notasFiscais)
                    {
                        if (notaFiscal.NFSe != null || notaFiscal.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal)
                        {
                            notaFiscal.Status = Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Retornado;
                            repNotaFiscal.Atualizar(notaFiscal);
                        }
                    }
                }
            }
            return retorno;
        }

        public bool EnviarOcorrenciasDocumentoTransporteFTP(int codigoDocumentoTransporte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.DocumentoTransporteNatura repDocumento = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);
            Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscal = new Repositorio.NotaFiscalDocumentoTransporteNatura(unidadeDeTrabalho);

            Dominio.Entidades.DocumentoTransporteNatura documento = repDocumento.BuscarPorCodigo(codigoDocumentoTransporte);
            List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> notasFiscais = repNotaFiscal.BuscarPorDocumentoTransporte(codigoDocumentoTransporte);
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> nfsesNatura = new List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura>();

            foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscal in notasFiscais)
            {
                if (notaFiscal.CTe != null)
                    ctes.Add(notaFiscal.CTe);
                if (notaFiscal.NFSe != null)
                    nfsesNatura.Add(notaFiscal);
            }

            string nomeArquivo = "";
            string erro = "";

            string host = documento.Empresa.Configuracao.FTPNaturaHost;
            string porta = documento.Empresa.Configuracao.FTPNaturaPorta;
            string diretorio = documento.Empresa.Configuracao.FTPNaturaDiretorio;
            string usuario = documento.Empresa.Configuracao.FTPNaturaUsuario;
            string senha = documento.Empresa.Configuracao.FTPNaturaSenha;

            bool passivo = documento.Empresa.Configuracao.FTPNaturaPassivo;
            bool sFTP = documento.Empresa.Configuracao.FTPNaturaSeguro;

            if (ctes.Count > 0)
            {
                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unidadeDeTrabalho);
                Dominio.Entidades.LayoutEDI layoutEdi = repLayoutEDI.BuscarPorCodigo(documento.Empresa.Configuracao.LayoutEDIOcoren.Codigo);
                Servicos.GeracaoEDI svcEDI = new Servicos.GeracaoEDI(unidadeDeTrabalho, layoutEdi, documento.Empresa, ctes);

                System.IO.MemoryStream ediOcorrenciasCTes = new System.IO.MemoryStream();

                ediOcorrenciasCTes = svcEDI.GerarArquivo();

                // Define variaveis para conexao
                erro = "";
                nomeArquivo = "SAPOCO" + Utilidades.String.OnlyNumbers(documento.Empresa.Configuracao.CodigoFilialNatura) + DateTime.Now.ToString("yyMMddHHmm") + "0001.txt";

                // Verifica conexao
                if (!Servicos.FTP.TestarConexao(host, porta, diretorio, usuario, senha, passivo, false, out erro, sFTP))
                    new Exception(erro);

                // Envia arquivo ao FTP
                if (ediOcorrenciasCTes != null)
                {
                    if (!Servicos.FTP.EnviarArquivo(ediOcorrenciasCTes, nomeArquivo, host, porta, diretorio, usuario, senha, passivo, false, out erro, sFTP))
                        new Exception(erro);
                }
            }

            System.IO.MemoryStream ediOcorrenciasNFSes = new System.IO.MemoryStream();
            ediOcorrenciasNFSes = this.GerarOcorrenciaNFSeNatura(nfsesNatura, documento.Empresa.Configuracao.NaturaEnviaOcorrenciaEntreguePadrao);

            if (ediOcorrenciasNFSes != null && ediOcorrenciasNFSes.Length > 0)
            {
                nomeArquivo = "SAPOCO" + Utilidades.String.OnlyNumbers(documento.Empresa.Configuracao.CodigoFilialNatura) + DateTime.Now.ToString("yyMMddHHmm") + "0002.txt";
                if (!Servicos.FTP.EnviarArquivo(ediOcorrenciasNFSes, nomeArquivo, host, porta, diretorio, usuario, senha, passivo, false, out erro, sFTP))
                    new Exception(erro);
            }

            return true;
        }

        public void EnviarOcorrenciasDocumentoTransporte(int codigoDocumentoTransporte, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.OcorrenciaDeCTe repOcorrencia = new Repositorio.OcorrenciaDeCTe(unitOfWork);
            Repositorio.DocumentoTransporteNatura repDocumento = new Repositorio.DocumentoTransporteNatura(unitOfWork);
            Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscal = new Repositorio.NotaFiscalDocumentoTransporteNatura(unitOfWork);
            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);

            Dominio.Entidades.DocumentoTransporteNatura documento = repDocumento.BuscarPorCodigo(codigoDocumentoTransporte);

            List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> notasFiscais = repNotaFiscal.BuscarPorDocumentoTransporte(codigoDocumentoTransporte);

            Servicos.ServicoNatura.ProcessaOcorrencias.SI_ProcessaOcorrenciasAsync_OBClient svcOcorrencia = ObterClientNatura<ServicoNatura.ProcessaOcorrencias.SI_ProcessaOcorrenciasAsync_OBClient, ServicoNatura.ProcessaOcorrencias.SI_ProcessaOcorrenciasAsync_OB>(documento.Empresa.Configuracao.UsuarioNatura, documento.Empresa.Configuracao.SenhaNatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Natura_SI_ProcessaOcorrencias, unitOfWork, out Servicos.Models.Integracao.InspectorBehavior inspector);

            List<Servicos.ServicoNatura.ProcessaOcorrencias.DT_ListaOcorrenciasDadosOcorrencias> ocorrencias = new List<ServicoNatura.ProcessaOcorrencias.DT_ListaOcorrenciasDadosOcorrencias>();

            foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscal in notasFiscais)
            {
                if (notaFiscal.NFSe != null)
                {
                    ocorrencias.Add(new Servicos.ServicoNatura.ProcessaOcorrencias.DT_ListaOcorrenciasDadosOcorrencias()
                    {
                        chaveNFe = notaFiscal.Chave,
                        codigoOcorrencia = "01",
                        dataOcorrencia = DateTime.Now.ToString("yyyy-MM-dd"),
                        horaOcorrencia = DateTime.Now.ToString("HH:mm:ss"),
                        textoOcorrencia = ""
                    });
                }
                else
                {
                    Dominio.Entidades.OcorrenciaDeCTe ocorrencia = repOcorrencia.BuscarUltimaDoCTe(notaFiscal.CTe.Codigo);

                    if (ocorrencia != null)
                    {
                        ocorrencias.Add(new Servicos.ServicoNatura.ProcessaOcorrencias.DT_ListaOcorrenciasDadosOcorrencias()
                        {
                            chaveNFe = notaFiscal.Chave,
                            codigoOcorrencia = ocorrencia.Ocorrencia.CodigoProceda,
                            dataOcorrencia = ocorrencia.DataDaOcorrencia.ToString("yyyy-MM-dd"),
                            horaOcorrencia = ocorrencia.DataDaOcorrencia.ToString("HH:mm:ss") == "00:00:00" ? "00:00:01" : ocorrencia.DataDaOcorrencia.ToString("HH:mm:ss"),
                            textoOcorrencia = ocorrencia.Observacao
                        });
                    }
                    else
                    {
                        ocorrencias.Add(new Servicos.ServicoNatura.ProcessaOcorrencias.DT_ListaOcorrenciasDadosOcorrencias()
                        {
                            chaveNFe = notaFiscal.Chave,
                            codigoOcorrencia = "01",
                            dataOcorrencia = DateTime.Now.ToString("yyyy-MM-dd"),
                            horaOcorrencia = DateTime.Now.ToString("HH:mm:ss"),
                            textoOcorrencia = ""
                        });
                    }
                }
            }

            var dados = new Servicos.ServicoNatura.ProcessaOcorrencias.DT_ListaOcorrenciasDados()
            {
                codTranspMatriz = documento.Empresa.Configuracao.CodigoMatrizNatura,
                documentoTransporte = documento.NumeroDT.ToString(),
                ocorrencias = ocorrencias.ToArray()
            };

#if DEBUG
            dados.codTranspMatriz = "T982";
#endif

            svcOcorrencia.SI_ProcessaOcorrenciasAsync_OBAsync(new Servicos.ServicoNatura.ProcessaOcorrencias.DT_ListaOcorrencias()
            {
                dados = dados
            });

            var xmlEnvio = inspector.LastRequestXML;
            Servicos.Log.TratarErro("XML Envio Ocorrencia Natura " + xmlEnvio);
            var xmlRetorno = inspector.LastResponseXML;
            Servicos.Log.TratarErro("XML Retorno Ocorrencia Natura " + xmlRetorno);

            documento.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Finalizado;
            repDocumento.Atualizar(documento);

            foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscal in notasFiscais)
            {
                notaFiscal.Status = Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Finalizado;
                repNotaFiscal.Atualizar(notaFiscal);
            }
        }

        public void ConsultarPreFaturas(int codigoEmpresa, long numeroPreFatura, DateTime dataInicial, DateTime dataFinal, bool atualizarPreFatura, int codigoUsuario, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.NaturaXML repNaturaXML = new Repositorio.NaturaXML(unidadeDeTrabalho);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            Dominio.Entidades.NaturaXML naturaXML = new Dominio.Entidades.NaturaXML();

            Servicos.ServicoNatura.ProcessaPreFatura.SI_ProcessaPreFaturaSync_OBClient svcPreFatura = ObterClientNatura<Servicos.ServicoNatura.ProcessaPreFatura.SI_ProcessaPreFaturaSync_OBClient, Servicos.ServicoNatura.ProcessaPreFatura.SI_ProcessaPreFaturaSync_OB>(empresa.Configuracao.UsuarioNatura, empresa.Configuracao.SenhaNatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Natura_SI_ProcessaPreFatura, unidadeDeTrabalho, out Servicos.Models.Integracao.InspectorBehavior inspector);

            var dados = new Servicos.ServicoNatura.ProcessaPreFatura.DT_EnviaParamPreFaturaDados()
            {
                codTranspMatriz = empresa.Configuracao.CodigoFilialNatura // empresa.Configuracao.CodigoMatrizNatura
            };

            if (numeroPreFatura > 0)
                dados.numeroPreFatura = numeroPreFatura.ToString();
            else
            {
                if (dataInicial != DateTime.MinValue)
                    dados.dataDe = dataInicial.ToString("yyyy-MM-dd");

                if (dataFinal != DateTime.MinValue)
                    dados.dataAte = dataFinal.ToString("yyyy-MM-dd");
            }

            var retorno = svcPreFatura.SI_ProcessaPreFaturaSync_OB(new Servicos.ServicoNatura.ProcessaPreFatura.DT_EnviaParamPreFatura() { dados = dados });

            var xmlConsulta = inspector.LastRequestXML;
            //Servicos.Log.TratarErro("XML consulta Pré-Fatura " + xmlConsulta);            
            var xmlPreFatura = inspector.LastResponseXML;
            //Servicos.Log.TratarErro("XML retorno Pré-Fatura " + xmlPreFatura);

            naturaXML.Data = DateTime.Now;
            naturaXML.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.ConsultaPreFatura;
            naturaXML.Usuario = codigoUsuario == 0 ? repUsuario.BuscarPrimeiroPorEmpresa(empresa.Codigo, Dominio.Enumeradores.TipoAcesso.Emissao) : repUsuario.BuscarPorCodigo(codigoUsuario);
            naturaXML.XMLEnvio = xmlConsulta;
            naturaXML.XMLRetorno = xmlPreFatura;
            repNaturaXML.Inserir(naturaXML);

            if (retorno[0].numeroPreFatura == null)
                return;

            Repositorio.FaturaNatura repFatura = new Repositorio.FaturaNatura(unidadeDeTrabalho);
            Repositorio.DocumentoTransporteNatura repDocumentoTransporte = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);
            Repositorio.ItemFaturaNatura repItemFatura = new Repositorio.ItemFaturaNatura(unidadeDeTrabalho);
            Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscal = new Repositorio.NotaFiscalDocumentoTransporteNatura(unidadeDeTrabalho);

            List<long> numerosPreFatura = (from obj in retorno select long.Parse(obj.numeroPreFatura)).Distinct().ToList();

            foreach (long numPreFatura in numerosPreFatura)
            {
                unidadeDeTrabalho.Start();

                List<ServicoNatura.ProcessaPreFatura.DT_RecebePreFaturaDados> preFaturas = (from obj in retorno where obj.numeroPreFatura == numPreFatura.ToString() select obj).ToList();

                Dominio.Entidades.FaturaNatura fatura = repFatura.BuscarPorPreFatura(numPreFatura, empresa.Codigo);

                bool novaPreFatura = false;

                if (fatura == null)
                {
                    novaPreFatura = true;
                    fatura = new Dominio.Entidades.FaturaNatura();

                    fatura.DataPreFatura = DateTime.ParseExact(preFaturas.First().dataPreFatura, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None);
                    fatura.Empresa = empresa;
                    fatura.Numero = repFatura.ObterUltimoNumero(empresa.Codigo) + 1;
                    fatura.NumeroPreFatura = numPreFatura;
                    fatura.Status = Dominio.ObjetosDeValor.Enumerador.StatusFaturaNatura.Pendente;

                    repFatura.Inserir(fatura);
                }

                if (novaPreFatura || atualizarPreFatura)
                {
                    foreach (ServicoNatura.ProcessaPreFatura.DT_RecebePreFaturaDados preFatura in preFaturas)
                    {
                        foreach (var item in preFatura.itens)
                        {
                            Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscal = item.modeloDocFiscal == 57 ? repNotaFiscal.Buscar(empresa.Codigo, item.chaveCTe, long.Parse(item.transporte.docTransporte)) : repNotaFiscal.Buscar(empresa.Codigo, int.Parse(item.numeroDocFiscal), int.Parse(item.serieDocFiscal), long.Parse(item.transporte.docTransporte));

                            if (notaFiscal == null)
                                continue;

                            Dominio.Entidades.ItemFaturaNatura itemFatura = repItemFatura.BuscarPorNotaFiscal(notaFiscal.Codigo, fatura.Codigo);

                            if (itemFatura != null)
                                continue;

                            itemFatura = new Dominio.Entidades.ItemFaturaNatura();

                            itemFatura.AliquotaCOFINS = item.impostos.aliquotaCOFINS;
                            itemFatura.AliquotaICMS = item.impostos.aliquotaICMS;
                            itemFatura.AliquotaICMSST = item.impostos.aliquotaICMSST;
                            itemFatura.AliquotaISS = item.impostos.aliquotaISS;
                            itemFatura.AliquotaPIS = item.impostos.aliquotaPIS;
                            itemFatura.ValorDoDesconto = string.IsNullOrWhiteSpace(item.transporte.difValorFrete) ? 0 : decimal.Parse(item.transporte.difValorFrete, cultura);
                            itemFatura.BaseCalculoCOFINS = string.IsNullOrWhiteSpace(item.impostos.baseCOFINS) ? 0 : decimal.Parse(item.impostos.baseCOFINS, cultura);
                            itemFatura.BaseCalculoICMS = string.IsNullOrWhiteSpace(item.impostos.baseICMS) ? 0 : decimal.Parse(item.impostos.baseICMS, cultura);
                            itemFatura.BaseCalculoICMSST = string.IsNullOrWhiteSpace(item.impostos.baseICMSST) ? 0 : decimal.Parse(item.impostos.baseICMSST, cultura);
                            itemFatura.BaseCalculoISS = string.IsNullOrWhiteSpace(item.impostos.baseISS) ? 0 : decimal.Parse(item.impostos.baseISS, cultura);
                            itemFatura.BaseCalculoPIS = string.IsNullOrWhiteSpace(item.impostos.basePIS) ? 0 : decimal.Parse(item.impostos.basePIS, cultura);
                            itemFatura.IVAICMSST = string.IsNullOrWhiteSpace(item.impostos.ivaICMSST) ? 0 : decimal.Parse(item.impostos.ivaICMSST, cultura);
                            itemFatura.ValorCOFINS = string.IsNullOrWhiteSpace(item.impostos.valorCOFINS) ? 0 : decimal.Parse(item.impostos.valorCOFINS, cultura);
                            itemFatura.ValorICMS = string.IsNullOrWhiteSpace(item.impostos.valorICMS) ? 0 : decimal.Parse(item.impostos.valorICMS, cultura);
                            itemFatura.ValorICMSST = string.IsNullOrWhiteSpace(item.impostos.valorICMSST) ? 0 : decimal.Parse(item.impostos.valorICMSST, cultura);
                            itemFatura.ValorISS = string.IsNullOrWhiteSpace(item.impostos.valorISS) ? 0 : decimal.Parse(item.impostos.valorISS, cultura);
                            itemFatura.ValorPIS = string.IsNullOrWhiteSpace(item.impostos.valorPIS) ? 0 : decimal.Parse(item.impostos.valorPIS, cultura);

                            itemFatura.Fatura = fatura;

                            itemFatura.NotaFiscal = notaFiscal;

                            repItemFatura.Inserir(itemFatura);

                            fatura.ValorFrete += decimal.Parse(item.transporte.valorFrete, cultura);
                        }
                    }
                }

                if (fatura.NaturaXMLs == null)
                    fatura.NaturaXMLs = new List<Dominio.Entidades.NaturaXML>();
                fatura.NaturaXMLs.Add(naturaXML);
                repFatura.Atualizar(fatura);

                unidadeDeTrabalho.CommitChanges();
            }
        }

        #endregion

        #region Métodos Privados

        private System.IO.MemoryStream GerarOcorrenciaNFSeNatura(List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> notasNatura, bool gerarOcorrenciaEntreguePadrao)
        {
            if (notasNatura == null || notasNatura.Count == 0)
                return null;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.OcorrenciaDeNFSe repOcorrenciaDeNFSe = new Repositorio.OcorrenciaDeNFSe(unitOfWork);

            MemoryStream memoStream = new MemoryStream();
            StringBuilder RegistroEDI = new StringBuilder();

            string linha341, linha342;
            linha341 = "341" + notasNatura.FirstOrDefault().DocumentoTransporte.Empresa.CNPJ_SemFormato + PreencherEspacoDireita(notasNatura.FirstOrDefault().DocumentoTransporte.Empresa.RazaoSocial, 40);
            RegistroEDI.AppendLine(linha341);
            foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura nota in notasNatura)
            {
                Dominio.Entidades.OcorrenciaDeNFSe ocorrenNfse = null;
                if (nota.NFSe != null)
                    ocorrenNfse = repOcorrenciaDeNFSe.BuscarUltimaOcorrenciaPorNFSe(nota.NFSe.Codigo);

                if ((ocorrenNfse != null) || (ocorrenNfse == null && gerarOcorrenciaEntreguePadrao))
                {
                    linha342 = "342";
                    linha342 = string.Concat(linha342, PreencherEspacoDireita(nota.Emitente.CPF_CNPJ_SemFormato, 14));
                    linha342 = string.Concat(linha342, "012");
                    linha342 = string.Concat(linha342, PreencherZeroEsquerda(nota.Numero.ToString(), 9));
                    linha342 = string.Concat(linha342, ocorrenNfse != null && !string.IsNullOrWhiteSpace(ocorrenNfse.Ocorrencia.CodigoProceda) ? PreencherZeroEsquerda(ocorrenNfse.Ocorrencia.CodigoProceda, 2) : "01");
                    linha342 = string.Concat(linha342, ocorrenNfse != null ? ocorrenNfse.DataDaOcorrencia.ToString("ddMMyyyy") : DateTime.Today.ToString("ddMMyyyy"));
                    linha342 = string.Concat(linha342, ocorrenNfse != null ? ocorrenNfse.DataDaOcorrencia.ToString("HHmm") : DateTime.Now.ToString("HHmm"));
                    linha342 = string.Concat(linha342, "  ");
                    linha342 = string.Concat(linha342, PreencherEspacoDireita(ocorrenNfse?.Observacao ?? "", 76));
                    RegistroEDI.AppendLine(linha342);
                }
            }

            string arquivo = RegistroEDI.ToString();

            memoStream.Write(System.Text.Encoding.UTF8.GetBytes(arquivo), 0, arquivo.Length);

            memoStream.Position = 0;
            unitOfWork.Dispose();

            return memoStream;
        }

        private string PreencherEspacoDireita(string dado, int numeroCaracteres)
        {
            if (!string.IsNullOrWhiteSpace(dado))
            {
                if (dado.Length > numeroCaracteres)
                {
                    return dado.Remove(numeroCaracteres, (dado.Length - numeroCaracteres));
                }
                else
                {
                    if (dado.Length != numeroCaracteres)
                        return string.Concat(dado, new string(' ', numeroCaracteres - dado.Length));
                    else
                        return dado;
                }
            }
            return new string(' ', numeroCaracteres);
        }

        private string PreencherZeroEsquerda(string dado, int numeroCaracteres)
        {
            if (!string.IsNullOrWhiteSpace(dado))
            {
                if (dado.Length > numeroCaracteres)
                {
                    return dado.Remove(numeroCaracteres, (dado.Length - numeroCaracteres));
                }
                else if (dado.Length < numeroCaracteres)
                {
                    return string.Concat(new string('0', numeroCaracteres - dado.Length), dado);
                }
                else if (dado.Length == numeroCaracteres)
                    return dado;
            }
            return new string(' ', numeroCaracteres);
        }

        private List<ServicoNatura.ProcessaCTeNFSe.DT_EnviaCteNfseDadosDocumentoFiscal> BuscarDocumentosEnvio(Dominio.Enumeradores.TipoCTE tipoCTe, List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> notasFiscais, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            List<ServicoNatura.ProcessaCTeNFSe.DT_EnviaCteNfseDadosDocumentoFiscal> documentosEnvio = new List<ServicoNatura.ProcessaCTeNFSe.DT_EnviaCteNfseDadosDocumentoFiscal>();

            foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura doc in notasFiscais)
            {
                ServicoNatura.ProcessaCTeNFSe.DT_EnviaCteNfseDadosDocumentoFiscal docPreenchido = null;

                if (doc.CTe != null)
                    docPreenchido = BuscarDocumentosEnvioCTe(doc, tipoCTe, cultura, unitOfWork);
                else if (doc.NFSe != null)
                    docPreenchido = BuscarDocumentosEnvioNFSe(doc, cultura, unitOfWork);

                if (docPreenchido != null)
                    documentosEnvio.Add(docPreenchido);
            }

            return documentosEnvio;
        }

        private List<ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscal> BuscarDocumentosEnvioNovo(Dominio.Enumeradores.TipoCTE tipoCTe, List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> notasFiscais, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            List<ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscal> documentosEnvio = new List<ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscal>();

            foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura doc in notasFiscais)
            {
                ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscal docPreenchido = null;

                if (doc.CTe != null)
                {
                    bool cteInserido = (from o in documentosEnvio where o.cte != null && o.cte.chaveCTe == doc.CTe.Chave select o).Count() > 0;
                    if (!cteInserido)
                    {
                        docPreenchido = BuscarDocumentosEnvioCTeNovo(doc, tipoCTe, cultura, unitOfWork);
                        if (docPreenchido != null)
                            documentosEnvio.Add(docPreenchido);
                    }
                }
                else if (doc.NFSe != null)
                {
                    docPreenchido = BuscarDocumentosEnvioNFSeNovo(doc, cultura, unitOfWork);
                    if (docPreenchido != null)
                        documentosEnvio.Add(docPreenchido);
                }
            }

            return documentosEnvio;
        }

        private ServicoNatura.ProcessaCTeNFSe.DT_EnviaCteNfseDadosDocumentoFiscal BuscarDocumentosEnvioCTe(Dominio.Entidades.NotaFiscalDocumentoTransporteNatura doc, Dominio.Enumeradores.TipoCTE tipoCTe, System.Globalization.CultureInfo cultura, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            if (doc.CTe.TipoCTE == tipoCTe)
            {
                string stringXMLCTe = doc.CTe.Status == "A" ? repXMLCTe.BuscarPorCTe(doc.CTe.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao)?.XML : repXMLCTe.BuscarPorCTe(doc.CTe.Codigo, Dominio.Enumeradores.TipoXMLCTe.Cancelamento)?.XML;
                List<Dominio.Entidades.DocumentosCTE> documentos = null;
                if (tipoCTe == Dominio.Enumeradores.TipoCTE.Complemento)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico ctePai = repCTe.BuscarPorChave(doc.CTe.ChaveCTESubComp);
                    documentos = ctePai.Documentos.ToList();
                }
                else
                {
                    documentos = doc.CTe.Documentos.ToList();
                }

                XmlDocument xDoc = new XmlDocument();

                return new ServicoNatura.ProcessaCTeNFSe.DT_EnviaCteNfseDadosDocumentoFiscal()
                {
                    dataEmissao = doc.CTe.DataEmissao.Value,
                    destino = string.Format("{0:0000000}", doc.CTe.LocalidadeTerminoPrestacao.CodigoIBGE),
                    modelo = "57",
                    numero = doc.CTe.Numero.ToString(),
                    serie = doc.CTe.Serie.Numero.ToString(),
                    origem = doc.QtdRevista != null && doc.QtdRevista != "" && doc.QtdRevista.Length > 7 ? doc.QtdRevista.Substring(doc.QtdRevista.Length - 7, 7) : string.Format("{0:0000000}", doc.CTe.LocalidadeInicioPrestacao.CodigoIBGE),
                    tipoDoc = doc.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Anulacao ? "A" :
                              doc.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento ? "C" :
                              doc.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Substituto ? "S" :
                              doc.CTe.Status == "A" ? "N" : "A",
                    valorFrete = doc.CTe.ValorFrete.ToString("0.00", cultura),
                    valorImpostos = doc.CTe.ValorICMS.ToString("0.00", cultura),
                    cte = new ServicoNatura.ProcessaCTeNFSe.DT_EnviaCteNfseDadosDocumentoFiscalCte()
                    {
                        chaveCTe = doc.CTe.Chave,
                        xmlCTe = xDoc.CreateCDataSection(stringXMLCTe.Replace("<![CDATA[", "").Replace("]]>", ""))
                    },
                    notasTransportadas = (from obj in documentos select new ServicoNatura.ProcessaCTeNFSe.DT_EnviaCteNfseDadosDocumentoFiscalNotasTransportadas() { chaveNFe = obj.ChaveNFE, dataEmissao = obj.DataEmissao, dataEmissaoSpecified = true }).ToArray()
                };
            }

            return null;
        }

        private ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscal BuscarDocumentosEnvioCTeNovo(Dominio.Entidades.NotaFiscalDocumentoTransporteNatura doc, Dominio.Enumeradores.TipoCTE tipoCTe, System.Globalization.CultureInfo cultura, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            if (doc.CTe.TipoCTE == tipoCTe)
            {
                string stringXMLCTe = doc.CTe.Status == "A" ? repXMLCTe.BuscarPorCTe(doc.CTe.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao)?.XML : repXMLCTe.BuscarPorCTe(doc.CTe.Codigo, Dominio.Enumeradores.TipoXMLCTe.Cancelamento)?.XML;
                List<Dominio.Entidades.DocumentosCTE> documentos = null;
                if (tipoCTe == Dominio.Enumeradores.TipoCTE.Complemento)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico ctePai = repCTe.BuscarPorChave(doc.CTe.ChaveCTESubComp);
                    documentos = ctePai.Documentos.ToList();
                }
                else
                {
                    documentos = doc.CTe.Documentos.ToList();
                }

                XmlDocument xDoc = new XmlDocument();

                return new ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscal()
                {
                    dataEmissao = doc.CTe.DataEmissao.Value,
                    destino = string.Format("{0:0000000}", doc.CTe.LocalidadeTerminoPrestacao.CodigoIBGE),
                    modelo = "57",
                    numero = doc.CTe.Numero.ToString(),
                    serie = doc.CTe.Serie.Numero.ToString(),
                    origem = doc.QtdRevista != null && doc.QtdRevista != "" && doc.QtdRevista.Length > 7 ? doc.QtdRevista.Substring(doc.QtdRevista.Length - 7, 7) : string.Format("{0:0000000}", doc.CTe.LocalidadeInicioPrestacao.CodigoIBGE),
                    tipoDoc = doc.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Anulacao ? "A" :
                              doc.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento ? "C" :
                              doc.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Substituto ? "S" :
                              doc.CTe.Status == "A" ? "N" : "A",
                    valorFrete = (doc.CTe.ValorPrestacaoServico - doc.CTe.ValorICMS).ToString("0.00", cultura), //doc.CTe.ValorFrete.ToString("0.00", cultura),
                    valorImpostos = doc.CTe.ValorICMS.ToString("0.00", cultura),
                    cte = new ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscalCte()
                    {
                        chaveCTe = doc.CTe.Chave,
                        xmlCTe = xDoc.CreateCDataSection(stringXMLCTe.Replace("<![CDATA[", "").Replace("]]>", ""))
                    },
                    notasTransportadas = (from obj in documentos select new ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscalNotasTransportadas() { chaveNFe = obj.ChaveNFE, dataEmissao = obj.DataEmissao, dataEmissaoSpecified = true }).ToArray()
                };
            }

            return null;
        }

        private ServicoNatura.ProcessaCTeNFSe.DT_EnviaCteNfseDadosDocumentoFiscal BuscarDocumentosEnvioNFSe(Dominio.Entidades.NotaFiscalDocumentoTransporteNatura doc, System.Globalization.CultureInfo cultura, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.XMLNFSe repXMLNFSe = new Repositorio.XMLNFSe(unitOfWork);

            string stringXML = doc.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado ?
                                                  repXMLNFSe.BuscarPorNFSe(doc.NFSe.Codigo, Dominio.Enumeradores.TipoXMLNFSe.Autorizacao)?.XML :
                                                  repXMLNFSe.BuscarPorNFSe(doc.NFSe.Codigo, Dominio.Enumeradores.TipoXMLNFSe.Cancelamento)?.XML;

            XmlDocument xDoc = new XmlDocument();

            return new ServicoNatura.ProcessaCTeNFSe.DT_EnviaCteNfseDadosDocumentoFiscal()
            {
                dataEmissao = doc.NFSe.DataEmissao,
                destino = string.Format("{0:0000000}", doc.NFSe.LocalidadePrestacaoServico.CodigoIBGE),
                modelo = "07",
                numero = doc.NFSe.Numero.ToString(),
                serie = doc.NFSe.Serie.Numero.ToString(),
                origem = string.Format("{0:0000000}", doc.NFSe.LocalidadePrestacaoServico.CodigoIBGE),
                tipoDoc = doc.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado ? "N" : "A",
                valorFrete = (doc.NFSe.ValorServicos - doc.NFSe.ValorISS).ToString("0.00", cultura),
                valorImpostos = doc.NFSe.ValorISS.ToString("0.00", cultura),
                nfse = new ServicoNatura.ProcessaCTeNFSe.DT_EnviaCteNfseDadosDocumentoFiscalNfse()
                {
                    aliqCOFINS = "0.00",
                    aliqCSLL = "0.00",
                    aliqIR = "0.00",
                    aliqISS = doc.NFSe.ValorISS > 0 ? doc.NFSe.AliquotaISS.ToString("0.00", cultura) : "0.00",
                    aliqPIS = "0.00",
                    baseCOFINS = "0.00",
                    baseCSLL = "0.00",
                    baseIR = "0.00",
                    baseISS = doc.NFSe.BaseCalculoISS.ToString("0.00", cultura),
                    basePIS = "0.00",
                    retencao = doc.NFSe.ValorISSRetido.ToString("0.00", cultura),
                    valorCOFINS = "0.00",
                    valorCSLL = doc.NFSe.ValorCSLL.ToString("0.00", cultura),
                    valorIR = doc.NFSe.ValorIR.ToString("0.00", cultura),
                    valorISS = doc.NFSe.ValorISS.ToString("0.00", cultura),
                    valorPIS = doc.NFSe.ValorPIS.ToString("0.00", cultura)
                },
                notasTransportadas = new ServicoNatura.ProcessaCTeNFSe.DT_EnviaCteNfseDadosDocumentoFiscalNotasTransportadas[]
                {
                     new ServicoNatura.ProcessaCTeNFSe.DT_EnviaCteNfseDadosDocumentoFiscalNotasTransportadas()
                     {
                          chaveNFe = doc.Chave,
                          dataEmissao = doc.DataEmissao.Value,
                          dataEmissaoSpecified = true
                     }
                }
            };
        }

        private ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscal BuscarDocumentosEnvioNFSeNovo(Dominio.Entidades.NotaFiscalDocumentoTransporteNatura doc, System.Globalization.CultureInfo cultura, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.XMLNFSe repXMLNFSe = new Repositorio.XMLNFSe(unitOfWork);

            string stringXML = doc.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado ?
                                                  repXMLNFSe.BuscarPorNFSe(doc.NFSe.Codigo, Dominio.Enumeradores.TipoXMLNFSe.Autorizacao)?.XML :
                                                  repXMLNFSe.BuscarPorNFSe(doc.NFSe.Codigo, Dominio.Enumeradores.TipoXMLNFSe.Cancelamento)?.XML;

            XmlDocument xDoc = new XmlDocument();

            return new ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscal()
            {
                dataEmissao = doc.NFSe.DataEmissao,
                destino = string.Format("{0:0000000}", doc.NFSe.LocalidadePrestacaoServico.CodigoIBGE),
                modelo = "07",
                numero = doc.NFSe.Numero.ToString(),
                serie = doc.NFSe.Serie.Numero.ToString(),
                origem = string.Format("{0:0000000}", doc.NFSe.LocalidadePrestacaoServico.CodigoIBGE),
                tipoDoc = doc.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado ? "N" : "A",
                valorFrete = (doc.NFSe.ValorServicos - doc.NFSe.ValorISS).ToString("0.00", cultura),
                valorImpostos = doc.NFSe.ValorISS.ToString("0.00", cultura),
                nfse = new ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscalNfse()
                {
                    aliqCOFINS = "0.00",
                    aliqCSLL = "0.00",
                    aliqIR = "0.00",
                    aliqISS = doc.NFSe.ValorISS > 0 ? doc.NFSe.AliquotaISS.ToString("0.00", cultura) : "0.00",
                    aliqPIS = "0.00",
                    baseCOFINS = "0.00",
                    baseCSLL = "0.00",
                    baseIR = "0.00",
                    baseISS = doc.NFSe.BaseCalculoISS.ToString("0.00", cultura),
                    basePIS = "0.00",
                    retencao = doc.NFSe.ValorISSRetido.ToString("0.00", cultura),
                    valorCOFINS = "0.00",
                    valorCSLL = doc.NFSe.ValorCSLL.ToString("0.00", cultura),
                    valorIR = doc.NFSe.ValorIR.ToString("0.00", cultura),
                    valorISS = doc.NFSe.ValorISS.ToString("0.00", cultura),
                    valorPIS = doc.NFSe.ValorPIS.ToString("0.00", cultura)
                },
                notasTransportadas = new ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscalNotasTransportadas[]
                {
                     new ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscalNotasTransportadas()
                     {
                          chaveNFe = doc.Chave,
                          dataEmissao = doc.DataEmissao.Value,
                          dataEmissaoSpecified = true
                     }
                }
            };
        }

        private void EnviarRetornoNatura(Dominio.Entidades.DocumentoTransporteNatura documento, List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> notasFiscais, List<ServicoNatura.ProcessaCTeNFSe.DT_EnviaCteNfseDadosDocumentoFiscal> documentosEnvio, Repositorio.UnitOfWork unitOfWork, ref string xmlEnvioDT, ref string xmlRetornoEnvioDT)
        {
            ServicoNatura.ProcessaCTeNFSe.SI_ProcessaCteNfseAsync_OBClient svcEnvioRetorno = ObterClientNatura<ServicoNatura.ProcessaCTeNFSe.SI_ProcessaCteNfseAsync_OBClient, ServicoNatura.ProcessaCTeNFSe.SI_ProcessaCteNfseAsync_OB>(documento.Empresa.Configuracao.UsuarioNatura, documento.Empresa.Configuracao.SenhaNatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Natura_SI_ProcessaCteNfse, unitOfWork, out Servicos.Models.Integracao.InspectorBehavior inspector);

            svcEnvioRetorno.SI_ProcessaCteNfseAsync_OBAsync(new ServicoNatura.ProcessaCTeNFSe.DT_EnviaCteNfse()
            {
                dados = new ServicoNatura.ProcessaCTeNFSe.DT_EnviaCteNfseDados()
                {
                    cnpjEmitente = documento.Empresa.CNPJ,
                    cnpjTomador = notasFiscais[0].CTe != null ? (notasFiscais[0].CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? notasFiscais[0].CTe.Remetente.CPF_CNPJ : notasFiscais[0].CTe.Destinatario.CPF_CNPJ) : (notasFiscais[0].NFSe.Tomador.CPF_CNPJ),
                    codTranspEmit = documento.Empresa.Configuracao.CodigoFilialNatura,
                    codTranspMatriz = documento.Empresa.Configuracao.CodigoMatrizNatura,
                    numeroTransporte = documento.NumeroDT.ToString(),
                    documentoFiscal = documentosEnvio.ToArray()
                }
            });

            xmlEnvioDT = inspector.LastRequestXML;
            xmlRetornoEnvioDT = inspector.LastResponseXML;
        }

        private ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentResponseDados[] EnviarRetornoNatura(Dominio.Entidades.DocumentoTransporteNatura documento, List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> notasFiscais, List<ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDadosDocumentoFiscal> documentosEnvio, Repositorio.UnitOfWork unitOfWork, ref string xmlEnvioDT, ref string xmlRetornoEnvioDT, ref string mensagemRetorno)
        {
            ServicoNaturaNovo.ProcessaCTeNFSe.SI_ProcessaCteNfse_SyncClient svcEnvioRetorno = ObterClientNatura<ServicoNaturaNovo.ProcessaCTeNFSe.SI_ProcessaCteNfse_SyncClient, ServicoNaturaNovo.ProcessaCTeNFSe.SI_ProcessaCteNfse_Sync>(documento.Empresa.Configuracao.UsuarioNatura, documento.Empresa.Configuracao.SenhaNatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Natura_Novo_SI_ProcessaCteNfse, unitOfWork, out Servicos.Models.Integracao.InspectorBehavior inspector);

            var dados = new ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequestDados()
            {
                cnpjEmitente = documento.Empresa.CNPJ,
                cnpjTomador = notasFiscais[0].CTe != null ? (notasFiscais[0].CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? notasFiscais[0].CTe.Remetente.CPF_CNPJ : notasFiscais[0].CTe.Destinatario.CPF_CNPJ) : (notasFiscais[0].NFSe.Tomador.CPF_CNPJ),
                codTranspEmit = documento.Empresa.Configuracao.CodigoFilialNatura,
                codTranspMatriz = documento.Empresa.Configuracao.CodigoMatrizNatura,
                numeroTransporte = documento.NumeroDT.ToString(),
                documentoFiscal = documentosEnvio.ToArray()
            };

#if DEBUG
            dados.cnpjEmitente = "82809088000666";
            dados.codTranspEmit = "T982";
            dados.codTranspMatriz = "T982";
#endif
            try
            {
                ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentResponseDados[] retorno = svcEnvioRetorno.SI_ProcessaCteNfse_Sync(new ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentRequest()
                {
                    dados = dados
                });

                xmlEnvioDT = inspector.LastRequestXML;
                xmlRetornoEnvioDT = inspector.LastResponseXML;

                if (retorno != null)
                    mensagemRetorno = string.Join(" / ", from obj in retorno select obj.number + " - " + obj.message);

                return retorno;
            }
            catch (Exception)
            {
                xmlEnvioDT = inspector.LastRequestXML;
                Servicos.Log.TratarErro(xmlEnvioDT);
                throw;
            }

        }

        private Dominio.Entidades.NFSe GerarNFSe(Dominio.Entidades.NotaFiscalDocumentoTransporteNatura nfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(nfe.DocumentoTransporte.Empresa.Codigo);
            Dominio.ObjetosDeValor.NFSe.NFSe nfse = new Dominio.ObjetosDeValor.NFSe.NFSe();

            nfse.CodigoIBGECidadePrestacaoServico = nfe.Emitente.Localidade.CodigoIBGE;
            nfse.ISSRetido = false;
            nfse.OutrasInformacoes = "DT " + nfe.DocumentoTransporte.NumeroDT.ToString() + " ref. Nota fiscal " + nfe.Numero;
            if (!string.IsNullOrWhiteSpace(nfe.Chave))
                nfse.OutrasInformacoes = nfse.OutrasInformacoes + " " + nfe.Chave;

            if (!string.IsNullOrWhiteSpace(nfe.SolicitacaoNumero))
                nfse.OutrasInformacoes = string.Concat(nfse.OutrasInformacoes, " / Solicitação número: ", nfe.SolicitacaoNumero);
            if (!string.IsNullOrWhiteSpace(nfe.PedidoNumero))
                nfse.OutrasInformacoes = string.Concat(nfse.OutrasInformacoes, " / Pedido número: ", nfe.PedidoNumero);

            if (nfe.Destinatario != null)
            {
                string nomeDestinatario = !string.IsNullOrWhiteSpace(nfe.Destinatario.NomeFantasia) ? nfe.Destinatario.NomeFantasia : nfe.Destinatario.Nome;
                if (!string.IsNullOrWhiteSpace(nfe.CodigoCF))
                    nomeDestinatario = "(" + nfe.CodigoCF + ") " + nomeDestinatario;
                nfse.OutrasInformacoes = nfse.OutrasInformacoes + " - Destino: " + nomeDestinatario;

                if (nfe.Destinatario.Localidade != null)
                    nfse.OutrasInformacoes = nfse.OutrasInformacoes + " - " + nfe.Destinatario.Localidade.Descricao + " " + nfe.Destinatario.Localidade.Estado.Sigla;
                if (!string.IsNullOrWhiteSpace(nfe.Destinatario.CEP))
                    nfse.OutrasInformacoes = nfse.OutrasInformacoes + " CEP: " + nfe.Destinatario.CEP;
            }

            nfse.Emitente = new Dominio.ObjetosDeValor.CTe.Empresa()
            {
                CNPJ = nfe.DocumentoTransporte.Empresa.CNPJ,
                Atualizar = false
            };

            nfse.Tomador = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = nfe.Emitente.Bairro,
                CEP = nfe.Emitente.CEP,
                CodigoAtividade = nfe.Emitente.Atividade.Codigo,
                CodigoIBGECidade = nfe.Emitente.Localidade.CodigoIBGE,
                CodigoPais = nfe.Emitente.Localidade.Estado.Pais.Codigo.ToString(),
                Complemento = nfe.Emitente.Complemento,
                CPFCNPJ = nfe.Emitente.CPF_CNPJ_SemFormato,
                Endereco = nfe.Emitente.Endereco,
                Exportacao = false,
                NomeFantasia = nfe.Emitente.NomeFantasia,
                RazaoSocial = nfe.Emitente.Nome,
                RGIE = nfe.Emitente.IE_RG,
                Numero = !string.IsNullOrWhiteSpace(nfe.Emitente.Numero) && nfe.Emitente.Numero.Length > 2 ? nfe.Emitente.Numero : "S/N",
                Telefone1 = nfe.Emitente.Telefone1,
                Telefone2 = nfe.Emitente.Telefone2
            };

            nfse.Itens = new List<Dominio.ObjetosDeValor.NFSe.Item>();

            Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);
            Dominio.Entidades.ServicoNFSe servicoMultiCTe = servicoNFSe.ObterServicoNFSe(empresa, empresa.Localidade.CodigoIBGE != nfse.CodigoIBGECidadePrestacaoServico, nfse.CodigoIBGECidadePrestacaoServico, unidadeDeTrabalho);

            Dominio.ObjetosDeValor.NFSe.Servico servico = null;
            if (servicoMultiCTe != null)
            {
                servico = new Dominio.ObjetosDeValor.NFSe.Servico();
                servico.Numero = servicoMultiCTe.Numero;
                servico.Descricao = servicoMultiCTe.Descricao;
                servico.Aliquota = servicoMultiCTe.Aliquota;
                servico.CNAE = servicoMultiCTe.CNAE;
                servico.CodigoTributacao = servicoMultiCTe.CodigoTributacao;
            }

            nfse.Itens.Add(new Dominio.ObjetosDeValor.NFSe.Item()
            {
                CodigoIBGECidade = nfse.CodigoIBGECidadePrestacaoServico,
                CodigoIBGECidadeIncidencia = nfse.CodigoIBGECidadePrestacaoServico,
                Discriminacao = "",
                CodigoPaisPrestacaoServico = int.Parse(nfe.Emitente.Localidade.Pais.Sigla),
                Quantidade = 1,
                ServicoPrestadoNoPais = true,
                ExigibilidadeISS = 1,
                ValorServico = nfe.ValorFrete,// nfe.ValorFrete / (1 - (empresa.Configuracao.ServicoNFSe.Aliquota / 100)),
                BaseCalculoISS = nfe.ValorFrete, // nfe.ValorFrete / (1 - (empresa.Configuracao.ServicoNFSe.Aliquota / 100)),
                AliquotaISS = servicoMultiCTe?.Aliquota ?? 0,
                ValorISS = servicoMultiCTe != null ? !empresa.OptanteSimplesNacional ? nfe.ValorFrete * servicoMultiCTe.Aliquota / 100 : 0 : 0,// nfe.ValorFrete * empresa.Configuracao.ServicoNFSe.Aliquota / 100,  //Prefeitura Ginfes -> Enviar valor de ISS Zerado quando empresa é do simples
                ValorTotal = nfe.ValorFrete,// nfe.ValorFrete / (1 - (empresa.Configuracao.ServicoNFSe.Aliquota / 100)),
                Servico = servico
            });

            nfse.Documentos = new List<Dominio.ObjetosDeValor.NFSe.Documentos>() {
                new Dominio.ObjetosDeValor.NFSe.Documentos() {
                    ChaveNFE = nfe.Chave,
                    Numero = nfe.Numero.ToString(),
                    Serie = nfe.Serie.ToString(),
                    Peso = nfe.Peso,
                    Valor = nfe.Valor,
                    DataEmissao = nfe.DataEmissao.HasValue ? nfe.DataEmissao.Value.ToString("dd/MM/yyyy") : ""
                }
            };

            NFSe svcNFSe = new NFSe(unidadeDeTrabalho);

            Dominio.Entidades.NFSe nfseIntegrada = svcNFSe.GerarNFSePorObjeto(nfse, unidadeDeTrabalho, Dominio.Enumeradores.StatusNFSe.EmDigitacao);

            return nfseIntegrada;
        }

        private Dominio.Entidades.ConhecimentoDeTransporteEletronico GerarCTe(List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> nfes, string observacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Dominio.Entidades.NotaFiscalDocumentoTransporteNatura nfe = nfes[0];

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

            Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();

            cte.CodigoIBGECidadeInicioPrestacao = nfe.Emitente.Localidade.CodigoIBGE;
            cte.CodigoIBGECidadeTerminoPrestacao = nfe.Destinatario.Localidade.CodigoIBGE;

            //cte.DataEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            cte.Destinatario = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = nfe.Destinatario.Bairro,
                CEP = nfe.Destinatario.CEP,
                CodigoAtividade = nfe.Destinatario.Atividade.Codigo,
                CodigoIBGECidade = nfe.Destinatario.Localidade.CodigoIBGE,
                CodigoPais = nfe.Destinatario.Localidade.Estado.Pais.Codigo.ToString(),
                Complemento = nfe.Destinatario.Complemento,
                CPFCNPJ = nfe.Destinatario.CPF_CNPJ_SemFormato,
                Endereco = nfe.Destinatario.Endereco,
                Exportacao = false,
                NomeFantasia = nfe.Destinatario.NomeFantasia,
                RazaoSocial = !string.IsNullOrWhiteSpace(nfe.CodigoCF) ? "(" + nfe.CodigoCF + ") " + nfe.Destinatario.Nome : nfe.Destinatario.Nome,
                RGIE = nfe.Destinatario.IE_RG,
                Numero = !string.IsNullOrWhiteSpace(nfe.Destinatario.Numero) && nfe.Destinatario.Numero.Length > 2 ? nfe.Destinatario.Numero : "S/N",
                Telefone1 = nfe.Destinatario.Telefone1,
                Telefone2 = nfe.Destinatario.Telefone2
            };

            cte.Documentos = (from obj in nfes
                              select new Dominio.ObjetosDeValor.CTe.Documento()
                              {
                                  ChaveNFE = obj.Chave,
                                  DataEmissao = obj.DataEmissao?.ToString("dd/MM/yyyy HH:mm:ss") ?? DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                                  ModeloDocumentoFiscal = "55",
                                  Peso = Math.Round(obj.Peso, 4, MidpointRounding.ToEven),
                                  Numero = obj.Numero.ToString(),
                                  Serie = obj.Serie.ToString(),
                                  Tipo = Dominio.Enumeradores.TipoDocumentoCTe.NFe,
                                  Volume = obj.Quantidade,
                                  Valor = obj.Valor
                              }).ToList();

            cte.Emitente = new Dominio.ObjetosDeValor.CTe.Empresa()
            {
                CNPJ = nfe.DocumentoTransporte.Empresa.CNPJ,
                Atualizar = false
            };

            cte.ProdutoPredominante = nfe.DocumentoTransporte.Empresa.Configuracao.ProdutoPredominante;
            cte.OutrasCaracteristicasDaCarga = nfe.DocumentoTransporte.Empresa.Configuracao.OutrasCaracteristicas;

            cte.QuantidadesCarga = new List<Dominio.ObjetosDeValor.CTe.QuantidadeCarga>()
            {
                new Dominio.ObjetosDeValor.CTe.QuantidadeCarga(){
                        Descricao = "Kilograma",
                        Quantidade = Math.Round((from obj in nfes select obj.Peso).Sum(), 4, MidpointRounding.ToEven),
                        UnidadeMedida = "01"
                },
                new Dominio.ObjetosDeValor.CTe.QuantidadeCarga(){
                        Descricao = "Volumes",
                        Quantidade = (from obj in nfes select obj.Quantidade).Sum(),
                        UnidadeMedida = "03"
                }
            };


            cte.Remetente = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = nfe.Emitente.Bairro,
                CEP = nfe.Emitente.CEP,
                Cidade = nfe.Emitente.Localidade.Descricao,
                CodigoAtividade = nfe.Emitente.Atividade.Codigo,
                CodigoIBGECidade = nfe.Emitente.Localidade.CodigoIBGE,
                CodigoPais = nfe.Emitente.Localidade.Estado.Pais.Codigo.ToString(),
                Complemento = nfe.Emitente.Complemento,
                CPFCNPJ = nfe.Emitente.CPF_CNPJ_SemFormato,
                Endereco = nfe.Emitente.Endereco,
                Exportacao = false,
                NomeFantasia = nfe.Emitente.NomeFantasia,
                RazaoSocial = nfe.Emitente.Nome,
                Numero = !string.IsNullOrWhiteSpace(nfe.Emitente.Numero) && nfe.Emitente.Numero.Length > 2 ? nfe.Emitente.Numero : "S/N",
                RGIE = nfe.Emitente.IE_RG,
                Telefone1 = nfe.Emitente.Telefone1,
                Telefone2 = nfe.Emitente.Telefone2
            };

            cte.Seguros = new List<Dominio.ObjetosDeValor.CTe.Seguro>()
            {
                new Dominio.ObjetosDeValor.CTe.Seguro(){
                        NomeSeguradora = "",
                        NumeroApolice = "",
                        NumeroAverbacao = "",
                        Tipo = Dominio.Enumeradores.TipoSeguro.Remetente,
                        Valor = 0m
                }
            };

            if (nfe.DocumentoTransporte.Veiculo != null)
            {
                cte.Veiculos = new List<Dominio.ObjetosDeValor.CTe.Veiculo>()
                {
                    new Dominio.ObjetosDeValor.CTe.Veiculo(){
                        Placa = nfe.DocumentoTransporte.Veiculo.Placa
                    }
                };
            }

            if (nfe.DocumentoTransporte.Motorista != null)
            {
                cte.Motoristas = new List<Dominio.ObjetosDeValor.CTe.Motorista>()
                {
                    new Dominio.ObjetosDeValor.CTe.Motorista(){
                         CPF = nfe.DocumentoTransporte.Motorista.CPF,
                         Nome = nfe.DocumentoTransporte.Motorista.Nome
                    }
                };
            }

            if (cte.Motoristas != null && cte.Motoristas.Count > 0 && cte.Veiculos != null && cte.Veiculos.Count > 0)
                cte.Lotacao = Dominio.Enumeradores.OpcaoSimNao.Sim;

            cte.PercentualICMSIncluirNoFrete = 100;
            cte.IncluirICMSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Sim;
            cte.TipoCTe = Dominio.Enumeradores.TipoCTE.Normal;
            cte.TipoImpressao = nfe.DocumentoTransporte.Empresa.Configuracao.TipoImpressao;
            cte.TipoPagamento = nfe.TipoPagamento;
            cte.TipoServico = Dominio.Enumeradores.TipoServico.Normal;

            if (cte.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
                cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
            else
                cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;

            cte.ValorAReceber = (from obj in nfes select obj.ValorFrete).Sum();
            cte.ValorFrete = cte.ValorAReceber;
            cte.ValorTotalPrestacaoServico = cte.ValorAReceber;
            cte.ValorTotalMercadoria = (from obj in nfes select obj.Valor).Sum();
            cte.ObservacoesGerais = "DT Nº " + nfe.DocumentoTransporte.NumeroDT;

            if (!string.IsNullOrWhiteSpace(observacao))
                cte.ObservacoesGerais = string.Concat(cte.ObservacoesGerais, " / ", observacao);

            if (nfes != null && nfes.Count > 0)
            {
                for (var i = 0; i < nfes.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(nfes[i].SolicitacaoNumero))
                        cte.ObservacoesGerais = string.Concat(cte.ObservacoesGerais, " / Solicitação número: ", nfes[i].SolicitacaoNumero);
                    if (!string.IsNullOrWhiteSpace(nfes[i].PedidoNumero))
                        cte.ObservacoesGerais = string.Concat(cte.ObservacoesGerais, " / Pedido número: ", nfes[i].PedidoNumero);
                }
            }

            if (!string.IsNullOrWhiteSpace(cte.ObservacoesGerais) && cte.ObservacoesGerais.Length > 1500)
                cte.ObservacoesGerais = cte.ObservacoesGerais.Substring(0, 1500);

            Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = svcCTe.GerarCTePorObjeto(cte, 0, unidadeDeTrabalho, "1", 0);

            return cteIntegrado;
        }

        private void SalvarNotaFiscal(Dominio.Entidades.DocumentoTransporteNatura documentoTransporte, ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDadosNfe dados, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            using (System.IO.Stream stream = ObterStream(dados.xmlNFe))
            {
                object nota = MultiSoftware.NFe.Servicos.Leitura.Ler(stream);

                if (nota.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc))
                    SalvarNotaFiscal(documentoTransporte, (MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc)nota, dados, unidadeDeTrabalho);
                else if (nota.GetType() == typeof(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc))
                    SalvarNotaFiscal(documentoTransporte, (MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)nota, dados, unidadeDeTrabalho);
                else
                    throw new Exception("Versão da NF-e não suportada. Implementar.");
            }
        }

        private void SalvarNotaFiscal(Dominio.Entidades.DocumentoTransporteNatura documentoTransporte, MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc notaFiscal, ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDadosNfe dados, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscalDT = new Repositorio.NotaFiscalDocumentoTransporteNatura(unidadeDeTrabalho);
            NFe svcNFe = new NFe(unidadeDeTrabalho);
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscalDT = new Dominio.Entidades.NotaFiscalDocumentoTransporteNatura();

            notaFiscalDT.Chave = dados.chaveNFe;
            notaFiscalDT.DataEmissao = DateTime.ParseExact(notaFiscal.NFe.infNFe.ide.dhEmi.Substring(0, 19), "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None);

            notaFiscalDT.Destinatario = svcNFe.ObterDestinatario(notaFiscal.NFe.infNFe.dest, documentoTransporte.Empresa.Codigo);

            notaFiscalDT.DocumentoTransporte = documentoTransporte;

            notaFiscalDT.Emitente = svcNFe.ObterEmitente(notaFiscal.NFe.infNFe.emit, documentoTransporte.Empresa.Codigo);

            notaFiscalDT.Numero = int.Parse(notaFiscal.NFe.infNFe.ide.nNF);
            notaFiscalDT.Peso = Math.Round(svcNFe.ObterPeso(notaFiscal.NFe.infNFe.transp, unidadeDeTrabalho), 4, MidpointRounding.ToEven);

            if (notaFiscalDT.Peso <= 0m)
                notaFiscalDT.Peso = Math.Round((decimal.Parse(dados.informacoesTransporte.pesoBruto.Trim(), cultura) / 1000), 4, MidpointRounding.ToEven);

            notaFiscalDT.Quantidade = svcNFe.ObterQuantidadeVolumes(notaFiscal.NFe.infNFe.transp);

            if (dados.informacoesTransporte.condFrete == "FOB")
                notaFiscalDT.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
            else
                notaFiscalDT.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;

            notaFiscalDT.Serie = int.Parse(notaFiscal.NFe.infNFe.ide.serie);
            notaFiscalDT.Valor = decimal.Parse(notaFiscal.NFe.infNFe.total.ICMSTot.vNF, cultura);
            notaFiscalDT.ValorFrete = !string.IsNullOrWhiteSpace(dados.informacoesTransporte.valorFrete) ? decimal.Parse(dados.informacoesTransporte.valorFrete, cultura) : 0m;
            notaFiscalDT.XML = dados.xmlNFe;

            notaFiscalDT.QtdRevista = dados.informacoesAdicItemNF[0].qtdRevista; //retorno.nfe[i].informacoesAdicItemNF[0].qtdRevista

            notaFiscalDT.Status = Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Pendente;

            repNotaFiscalDT.Inserir(notaFiscalDT);
        }

        private void SalvarNotaFiscal(Dominio.Entidades.DocumentoTransporteNatura documentoTransporte, MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc notaFiscal, ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDadosNfe dados, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscalDT = new Repositorio.NotaFiscalDocumentoTransporteNatura(unidadeDeTrabalho);
            NFe svcNFe = new NFe(unidadeDeTrabalho);
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscalDT = new Dominio.Entidades.NotaFiscalDocumentoTransporteNatura();

            notaFiscalDT.Chave = dados.chaveNFe;
            notaFiscalDT.DataEmissao = DateTime.ParseExact(notaFiscal.NFe.infNFe.ide.dhEmi.Substring(0, 19), "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None);

            notaFiscalDT.Destinatario = svcNFe.ObterDestinatario(notaFiscal.NFe.infNFe.dest, documentoTransporte.Empresa.Codigo);

            notaFiscalDT.DocumentoTransporte = documentoTransporte;

            notaFiscalDT.Emitente = svcNFe.ObterEmitente(notaFiscal.NFe.infNFe.emit, documentoTransporte.Empresa.Codigo);

            notaFiscalDT.Numero = int.Parse(notaFiscal.NFe.infNFe.ide.nNF);
            notaFiscalDT.Peso = Math.Round(svcNFe.ObterPeso(notaFiscal.NFe.infNFe.transp), 4, MidpointRounding.ToEven);

            if (notaFiscalDT.Peso <= 0m)
                notaFiscalDT.Peso = Math.Round((decimal.Parse(dados.informacoesTransporte.pesoBruto.Trim(), cultura) / 1000), 4, MidpointRounding.ToEven);

            notaFiscalDT.Quantidade = svcNFe.ObterQuantidadeVolumes(notaFiscal.NFe.infNFe.transp);

            if (dados.informacoesTransporte.condFrete == "FOB")
                notaFiscalDT.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
            else
                notaFiscalDT.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;

            notaFiscalDT.Serie = int.Parse(notaFiscal.NFe.infNFe.ide.serie);
            notaFiscalDT.Valor = decimal.Parse(notaFiscal.NFe.infNFe.total.ICMSTot.vNF, cultura);
            notaFiscalDT.ValorFrete = !string.IsNullOrWhiteSpace(dados.informacoesTransporte.valorFrete) ? decimal.Parse(dados.informacoesTransporte.valorFrete, cultura) : 0m;
            notaFiscalDT.XML = dados.xmlNFe;

            notaFiscalDT.QtdRevista = dados.informacoesAdicItemNF[0].qtdRevista; //retorno.nfe[i].informacoesAdicItemNF[0].qtdRevista

            notaFiscalDT.Status = Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Pendente;

            repNotaFiscalDT.Inserir(notaFiscalDT);
        }

        private void SalvarNotaFiscalContingencia(Dominio.Entidades.DocumentoTransporteNatura documentoTransporte, ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDadosNfe dados, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscalDT = new Repositorio.NotaFiscalDocumentoTransporteNatura(unidadeDeTrabalho);
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscalDT = new Dominio.Entidades.NotaFiscalDocumentoTransporteNatura();

            notaFiscalDT.Chave = dados.chaveNFe;
            if (dados.informacoesPedido.nfe.dataEmissao != "0000-00-00")
                notaFiscalDT.DataEmissao = DateTime.ParseExact(dados.informacoesPedido.nfe.dataEmissao, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None);
            else
                notaFiscalDT.DataEmissao = DateTime.Now;

            notaFiscalDT.Destinatario = this.ObterDestinatarioNotaFiscalContingencia(documentoTransporte.Empresa.Codigo, dados.informacoesPedido.destinatario, unidadeDeTrabalho);

            notaFiscalDT.Emitente = this.ObterEmitenteNotaFiscalContingencia(documentoTransporte.Empresa.Codigo, dados.informacoesPedido.emitente, unidadeDeTrabalho);

            notaFiscalDT.DocumentoTransporte = documentoTransporte;

            notaFiscalDT.Numero = int.Parse(dados.informacoesPedido.nfe.nNF);
            notaFiscalDT.Peso = Math.Round(decimal.Parse(dados.informacoesTransporte.pesoBruto, cultura), 4, MidpointRounding.ToEven);

            if (dados.informacoesTransporte.condFrete == "FOB")
                notaFiscalDT.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
            else
                notaFiscalDT.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;

            notaFiscalDT.Quantidade = int.Parse(dados.informacoesTransporte.qtdVolumes);
            notaFiscalDT.Serie = int.Parse(dados.informacoesPedido.nfe.serie);
            notaFiscalDT.Valor = decimal.Parse(dados.informacoesPedido.nfe.icmsTot.vNF.Trim(), cultura);

            notaFiscalDT.ValorFrete = decimal.Parse(dados.informacoesTransporte.valorFrete, cultura);

            repNotaFiscalDT.Inserir(notaFiscalDT);
        }

        private Dominio.Entidades.Cliente ObterEmitenteNotaFiscalContingencia(int codigoEmpresa, ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDadosNfeInformacoesPedidoEmitente emitente, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

            double cpfCnpj;
            double.TryParse(Utilidades.String.OnlyNumbers(emitente.cnpj), out cpfCnpj);

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            if (cliente != null)
                return cliente;

            cliente = new Dominio.Entidades.Cliente();
            cliente.Bairro = emitente.bairro.Length > 2 ? emitente.bairro : "Não Informado";
            cliente.CEP = emitente.cep;
            cliente.CPF_CNPJ = cpfCnpj;
            cliente.DataCadastro = DateTime.Now;
            cliente.Endereco = emitente.logradouro;
            cliente.IE_RG = emitente.inscricaoEstadual;
            cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(emitente.codMunicipio));
            cliente.Nome = emitente.nomeEmitente;
            cliente.NomeFantasia = emitente.nomeFantasia;
            cliente.Numero = emitente.numero;
            cliente.Telefone1 = string.IsNullOrWhiteSpace(emitente.telefone) || emitente.telefone.StartsWith("00") ? string.Empty : emitente.telefone;
            cliente.Tipo = "J";
            cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao);

            if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
            {
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                if (grupoPessoas != null)
                {
                    cliente.GrupoPessoas = grupoPessoas;
                }
            }
            cliente.Ativo = true;
            repCliente.Inserir(cliente);

            return cliente;
        }

        private Dominio.Entidades.Cliente ObterDestinatarioNotaFiscalContingencia(int codigoEmpresa, ServicoNatura.RecebeNotasFiscais.DT_RecebeNotasFiscaisDadosNfeInformacoesPedidoDestinatario destinatario, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            bool pf = true;

            double cpfCnpj;
            double.TryParse(Utilidades.String.OnlyNumbers(destinatario.cpf), out cpfCnpj);

            if (cpfCnpj <= 0)
            {
                double.TryParse(Utilidades.String.OnlyNumbers(destinatario.cnpj), out cpfCnpj);
                pf = false;
            }

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            if (cliente != null)
                return cliente;

            cliente = new Dominio.Entidades.Cliente();
            cliente.Bairro = destinatario.bairro.Length > 2 ? destinatario.bairro : "Não Informado";
            cliente.CEP = destinatario.cep;
            cliente.CPF_CNPJ = cpfCnpj;
            cliente.DataCadastro = DateTime.Now;
            cliente.Endereco = destinatario.logradouro;
            cliente.IE_RG = destinatario.inscricaoEstadual;
            cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(destinatario.codMunicipio));
            cliente.Nome = destinatario.nomeDestinatario;
            cliente.Numero = destinatario.numero;
            cliente.Telefone1 = string.IsNullOrWhiteSpace(destinatario.telefone) || destinatario.telefone.StartsWith("00") ? string.Empty : destinatario.telefone;
            cliente.Tipo = pf ? "F" : "J";
            cliente.Atividade = Atividade.ObterAtividade(codigoEmpresa, cliente.Tipo, StringConexao);

            if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
            {
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                if (grupoPessoas != null)
                {
                    cliente.GrupoPessoas = grupoPessoas;
                }
            }
            cliente.Ativo = true;
            repCliente.Inserir(cliente);

            return cliente;
        }

        private bool ExcluirDT(Dominio.Entidades.DocumentoTransporteNatura dtNatura, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {


                Repositorio.DocumentoTransporteNatura repDocumentoTransoirte = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);
                Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscal = new Repositorio.NotaFiscalDocumentoTransporteNatura(unidadeDeTrabalho);

                List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> listaNotasFiscais = repNotaFiscal.BuscarPorDocumentoTransporte(dtNatura.Codigo);
                foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscal in listaNotasFiscais)
                    repNotaFiscal.Deletar(notaFiscal);

                if (dtNatura.NaturaXMLs != null && dtNatura.NaturaXMLs.Count() > 0)
                {
                    dtNatura.NaturaXMLs.Clear();
                    repDocumentoTransoirte.Atualizar(dtNatura);
                }

                repDocumentoTransoirte.Deletar(dtNatura);

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(string.Concat("Problemas ao excluir Documento Transporte Natura: ", ex));
                return false;
            }
        }

        public void NotificarEmail(string assunto, System.Text.StringBuilder sb, string emails, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(emails))
                {
                    List<string> listaEmails = emails.Split(';').ToList();

                    Email svcEmail = new Servicos.Email(unidadeDeTrabalho);

                    string ambiente = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().IdentificacaoAmbiente;
                    assunto = assunto + " - " + ambiente;

                    sb.Append("<br /> <br />");

                    sb.Append("Favor não responder! E-mail enviado automaticamente para: " + emails).Append("<br /> <br />");

                    System.Text.StringBuilder ss = new System.Text.StringBuilder();
                    ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

                    for (var j = 0; j < listaEmails.Distinct().Count(); j++)
                    {
                        svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, listaEmails[j], "", "", assunto, sb.ToString(), string.Empty, null, ss.ToString(), true, listaEmails[j], 0, unidadeDeTrabalho);
                    }

                }
            }
            catch (Exception exptEmail)
            {
                Servicos.Log.TratarErro("Erro ao enviar e-mail notificação natura:" + exptEmail);
            }
        }

        private System.IO.Stream ObterStream(string s)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();

            System.IO.StreamWriter writer = new System.IO.StreamWriter(stream);

            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public T ObterClientNatura<T, TChannel>(string usuario, string senha, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao tipoWebService, Repositorio.UnitOfWork unitOfWork, out Servicos.Models.Integracao.InspectorBehavior inspectorBehavior) where TChannel : class where T : System.ServiceModel.ClientBase<TChannel>, new()
        {
            T svcNatura = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<T, TChannel>(tipoWebService, out inspectorBehavior);

#if DEBUG
            usuario = "T990";
            senha = "Pipf@2016";
#endif

            svcNatura.ClientCredentials.UserName.UserName = usuario; //"133881059";
            svcNatura.ClientCredentials.UserName.Password = senha; //"natura15";

            return svcNatura;
        }

        #endregion
    }
}
