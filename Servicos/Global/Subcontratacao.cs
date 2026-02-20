using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Servicos
{
    public class Subcontratacao : ServicoBase
    {
        public Subcontratacao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Metodos Globais

        public Dominio.ObjetosDeValor.Subcontratacao ObterDadosSubcontratacaoXNL(System.IO.Stream xml)
        {
            try
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                List<object> listaNotasRetorno = new List<object>();
                xml.Position = 0;

                XmlDocument document = new XmlDocument();
                document.Load(xml);

                Dominio.ObjetosDeValor.Subcontratacao subcontratacao = null;

                var items = document.GetElementsByTagName("EmiteContrato");
                if (items.Count > 0)
                {
                    subcontratacao = new Dominio.ObjetosDeValor.Subcontratacao();
                    subcontratacao.Documentos = new List<Dominio.ObjetosDeValor.SubcontratacaoDocumentos>();

                    foreach (XmlElement xDados in items)
                    {
                        foreach (XmlNode child in xDados.ChildNodes)
                        {
                            if (child.Name == "strCliente")
                                subcontratacao.Cliente = child.InnerText;
                            if (child.Name == "strAssinaturaDigital")
                                subcontratacao.AssinaturaDigital = child.InnerText;
                        }

                        var processoTransporte = xDados.GetElementsByTagName("processo_transporte");

                        foreach (XmlElement xProcessoTransporte in processoTransporte)
                        {
                            var expedidor = xProcessoTransporte.GetElementsByTagName("expedidor");
                            foreach (XmlElement xExpedidor in expedidor)
                            {
                                foreach (XmlNode child in xExpedidor.ChildNodes)
                                {
                                    if (child.Name == "cnpj_cpf")
                                        subcontratacao.CNPJExpedidor = child.InnerText;
                                }
                            }

                            var recebedor = xProcessoTransporte.GetElementsByTagName("recebedor");
                            foreach (XmlElement xRecebedor in recebedor)
                            {
                                foreach (XmlNode child in xRecebedor.ChildNodes)
                                {
                                    if (child.Name == "cnpj_cpf")
                                        subcontratacao.CNPJRecebedor = child.InnerText;
                                }
                            }

                            var dados_operacionais = xProcessoTransporte.GetElementsByTagName("dados_operacionais");
                            foreach (XmlElement xDadosOperacionais in dados_operacionais)
                            {
                                foreach (XmlNode child in xDadosOperacionais.ChildNodes)
                                {
                                    if (child.Name == "filial_codigo_cliente")
                                        subcontratacao.codigoCliente = child.InnerText;
                                }
                            }

                            var configuracoes_viagem = xProcessoTransporte.GetElementsByTagName("configuracoes_viagem");
                            foreach (XmlElement xConfiguracaoViagem in configuracoes_viagem)
                            {
                                foreach (XmlNode child in xConfiguracaoViagem.ChildNodes)
                                {
                                    if (child.Name == "data_saida")
                                        subcontratacao.DataViagem = child.InnerText;
                                    else if (child.Name == "hora_saida")
                                        subcontratacao.DataViagem = string.Concat(subcontratacao.DataViagem, " ", child.InnerText);
                                }
                            }

                            var conhecimentos = xProcessoTransporte.GetElementsByTagName("conhecimentos");
                            foreach (XmlElement xconhecimentos in conhecimentos)
                            {
                                var conhecimento = xconhecimentos.GetElementsByTagName("conhecimento");
                                foreach (XmlElement xCTEs in conhecimento)
                                {
                                    Dominio.ObjetosDeValor.SubcontratacaoDocumentos documento = new Dominio.ObjetosDeValor.SubcontratacaoDocumentos();
                                    foreach (XmlNode child in xCTEs.ChildNodes)
                                    {
                                        if (child.Name == "ctrc_codigo")
                                            documento.Numero = child.InnerText;
                                        else if (child.Name == "ctrc_serie")
                                            documento.Serie = child.InnerText;
                                    }
                                    subcontratacao.Documentos.Add(documento);
                                }
                            }

                            var dados_contratado = xProcessoTransporte.GetElementsByTagName("dados_contratado");
                            foreach (XmlElement xDadosContrato in dados_contratado)
                            {
                                foreach (XmlNode child in xDadosContrato.ChildNodes)
                                {
                                    if (child.Name == "contratado_cnpj_cpf")
                                        subcontratacao.CNPJSubcontratado = child.InnerText;
                                    if (child.Name == "carreta_proprietario_cnpj_cpf")
                                        subcontratacao.CNPJCTe = child.InnerText;
                                }
                            }

                            var dados_frete = xProcessoTransporte.GetElementsByTagName("dados_frete");
                            foreach (XmlElement xDadosFrete in dados_frete)
                            {
                                foreach (XmlNode child in xDadosFrete.ChildNodes)
                                {
                                    if (child.Name == "valor_frete")
                                        subcontratacao.ValorFrete = child.InnerText;
                                }
                            }
                        }
                    }
                }
                return subcontratacao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ObterDadosSubcontratacaoXNL");
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Subcontratacao ObterDadosXNLYamalog(System.IO.Stream xml)
        {
            try
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                List<object> listaNotasRetorno = new List<object>();
                xml.Position = 0;

                XmlDocument document = new XmlDocument();
                document.Load(xml);

                Dominio.ObjetosDeValor.Subcontratacao subcontratacao = new Dominio.ObjetosDeValor.Subcontratacao();
                subcontratacao.TipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                subcontratacao.Documentos = new List<Dominio.ObjetosDeValor.SubcontratacaoDocumentos>();

                //    subcontratacao = new Dominio.ObjetosDeValor.Subcontratacao();
                //    subcontratacao.Documentos = new List<Dominio.ObjetosDeValor.SubcontratacaoDocumentos>();

                //var items = document.GetElementsByTagName("EmiteContrato");
                //if (items.Count > 0)
                //{


                //    foreach (XmlElement xDados in items)
                //    {
                //        foreach (XmlNode child in xDados.ChildNodes)
                //        {
                //            if (child.Name == "strCliente")
                //                subcontratacao.Cliente = child.InnerText;
                //            if (child.Name == "strAssinaturaDigital")
                //                subcontratacao.AssinaturaDigital = child.InnerText;
                //        }

                var processoTransporte = document.GetElementsByTagName("processo_transporte");

                foreach (XmlElement xDados in processoTransporte)
                {
                    foreach (XmlNode child in xDados.ChildNodes)
                    {
                        if (child.Name == "processo_cliente_filial_codigo_cliente")
                            subcontratacao.codigoCliente = child.InnerText;
                        if (child.Name == "processo_transporte_codigo")
                            subcontratacao.codigoProcessoTransporte = child.InnerText;
                        if (child.Name == "data_emissao")
                            subcontratacao.DataEmissaoContrato = child.InnerText;
                    }

                    foreach (XmlElement xProcessoTransporte in processoTransporte)
                    {
                        var expedidor = xProcessoTransporte.GetElementsByTagName("expedidor");
                        foreach (XmlElement xExpedidor in expedidor)
                        {
                            foreach (XmlNode child in xExpedidor.ChildNodes)
                            {
                                if (child.Name == "cnpj_cpf")
                                    subcontratacao.CNPJExpedidor = child.InnerText;
                            }
                        }

                        var recebedor = xProcessoTransporte.GetElementsByTagName("recebedor");
                        foreach (XmlElement xRecebedor in recebedor)
                        {
                            foreach (XmlNode child in xRecebedor.ChildNodes)
                            {
                                if (child.Name == "cnpj_cpf")
                                    subcontratacao.CNPJRecebedor = child.InnerText;
                            }
                        }

                        //var configuracoes_viagem = xProcessoTransporte.GetElementsByTagName("configuracoes_viagem");
                        //foreach (XmlElement xConfiguracaoViagem in configuracoes_viagem)
                        //{
                        //    foreach (XmlNode child in xConfiguracaoViagem.ChildNodes)
                        //    {
                        //        if (child.Name == "data_saida")
                        //            subcontratacao.DataViagem = child.InnerText;
                        //        else if (child.Name == "hora_saida")
                        //            subcontratacao.DataViagem = string.Concat(subcontratacao.DataViagem, " ", child.InnerText);
                        //    }
                        //}

                        var documentos = xProcessoTransporte.GetElementsByTagName("documentos");
                        foreach (XmlElement xdocumentos in documentos)
                        {
                            var documento = xdocumentos.GetElementsByTagName("documento");
                            foreach (XmlElement xCTEs in documento)
                            {
                                Dominio.ObjetosDeValor.SubcontratacaoDocumentos doc = new Dominio.ObjetosDeValor.SubcontratacaoDocumentos();
                                foreach (XmlNode child in xCTEs.ChildNodes)
                                {
                                    if (child.Name == "codigo")
                                        doc.Numero = child.InnerText;
                                    else if (child.Name == "serie")
                                        doc.Serie = child.InnerText;
                                }
                                subcontratacao.Documentos.Add(doc);
                            }
                        }

                        var dados_contratado = xProcessoTransporte.GetElementsByTagName("contratado");
                        foreach (XmlElement xDadosContrato in dados_contratado)
                        {
                            foreach (XmlNode child in xDadosContrato.ChildNodes)
                            {
                                if (child.Name == "contratado_cnpj_cpf")
                                    subcontratacao.CNPJSubcontratado = child.InnerText;
                                //if (child.Name == "carreta_proprietario_cnpj_cpf")
                                //    subcontratacao.CNPJCTe = child.InnerText;
                            }
                        }

                        var dados_frete = xProcessoTransporte.GetElementsByTagName("dados_viagem");
                        foreach (XmlElement xDadosFrete in dados_frete)
                        {
                            foreach (XmlNode child in xDadosFrete.ChildNodes)
                            {
                                if (child.Name == "valor_frete")
                                    subcontratacao.ValorFrete = child.InnerText;
                                if (child.Name == "percurso_descricao")
                                    subcontratacao.DescricaoPercurso = child.InnerText;
                            }
                        }
                    }
                    //    }
                    //}
                }

                return subcontratacao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ObterDadosXNLYamalog");
                return null;
            }
        }

        public Dominio.Entidades.Subcontratacao SalvarSubcontratacao(Dominio.ObjetosDeValor.Subcontratacao dadosImportados, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Subcontratacao repSubcontratacao = new Repositorio.Subcontratacao(unitOfWork);
            Repositorio.SubcontratacaoDocumentos repSubcontratacaoDocumentos = new Repositorio.SubcontratacaoDocumentos(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            try
            {
                Dominio.Entidades.Subcontratacao subcontratacao = null;
                Dominio.Entidades.Empresa empresa = !string.IsNullOrWhiteSpace(dadosImportados.codigoCliente) ? repEmpresa.BuscarPorCodigoIntegracao(dadosImportados.codigoCliente) : repEmpresa.BuscarPorCNPJ(dadosImportados.CNPJCTe);

                if (empresa != null)
                {
                    unitOfWork.Start();

                    subcontratacao = new Dominio.Entidades.Subcontratacao();
                    subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.Pendente;
                    subcontratacao.DataImportacao = DateTime.Now;
                    subcontratacao.AgruparCTes = true;
                    subcontratacao.Empresa = empresa;
                    subcontratacao.CNPJSubcontratado = dadosImportados.CNPJSubcontratado;
                    subcontratacao.CodigoProcessoTransporte = dadosImportados.codigoProcessoTransporte;
                    subcontratacao.TipoServico = dadosImportados.TipoServico;
                    //decimal.TryParse(dadosImportados.ValorFrete.Replace(".", ","), out decimal valorFrete);
                    decimal valorFrete = 0;
                    if (!string.IsNullOrWhiteSpace(dadosImportados.ValorFrete))
                        valorFrete = decimal.Parse(dadosImportados.ValorFrete.Replace(",", "."), cultura);
                    subcontratacao.ValorFrete = valorFrete;
                    subcontratacao.EmpresaSubcontratada = repEmpresa.BuscarPorCNPJ(dadosImportados.CNPJSubcontratado);
                    subcontratacao.ObservacoesCTeSubcontratacao = dadosImportados.observacaoSubcontratacao;

                    subcontratacao.CNPJExpedidor = dadosImportados.CNPJExpedidor;
                    if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(subcontratacao.CNPJExpedidor)))
                        subcontratacao.ClienteExpedidor = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(subcontratacao.CNPJExpedidor)));

                    subcontratacao.CNPJRecebedor = dadosImportados.CNPJRecebedor;
                    if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(subcontratacao.CNPJRecebedor)))
                        subcontratacao.ClienteRecebedor = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(subcontratacao.CNPJRecebedor)));

                    subcontratacao.DataEmissaoContrato = dadosImportados.DataEmissaoContrato;
                    subcontratacao.DescricaoPercurso = dadosImportados.DescricaoPercurso;

                    repSubcontratacao.Inserir(subcontratacao);

                    string mensagemValidacaoDocumento = "";
                    foreach (Dominio.ObjetosDeValor.SubcontratacaoDocumentos documento in dadosImportados.Documentos)
                    {
                        Dominio.Entidades.SubcontratacaoDocumentos documentoSubcontratacao = new Dominio.Entidades.SubcontratacaoDocumentos();
                        documentoSubcontratacao.Subcontratacao = subcontratacao;
                        int.TryParse(documento.Numero, out int numeroDocumento);
                        int.TryParse(documento.Serie, out int serieDocumento);
                        documentoSubcontratacao.Numero = numeroDocumento;
                        documentoSubcontratacao.Serie = serieDocumento;
                        documentoSubcontratacao.Documento = repCTe.BuscarPorNumeroESerie(subcontratacao.Empresa.Codigo, numeroDocumento, serieDocumento, "57", subcontratacao.Empresa.TipoAmbiente);
                        repSubcontratacaoDocumentos.Inserir(documentoSubcontratacao);

                        if (documentoSubcontratacao.Documento == null)
                            mensagemValidacaoDocumento = string.IsNullOrWhiteSpace(mensagemValidacaoDocumento) ? documento.Numero + "/" + documento.Serie : mensagemValidacaoDocumento + " - " + documento.Numero + "/" + documento.Serie;
                    }

                    if (subcontratacao.EmpresaSubcontratada == null)
                    {
                        subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.FalhaProcessamento;
                        subcontratacao.DescricaoFalha = "Transportador " + dadosImportados.CNPJSubcontratado + " não possui cadastro";
                    }
                    else if (!string.IsNullOrWhiteSpace(mensagemValidacaoDocumento))
                    {
                        subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.FalhaProcessamento;
                        subcontratacao.DescricaoFalha = "CTes pendentes de importação: " + mensagemValidacaoDocumento;
                    }
                    else if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(dadosImportados.CNPJExpedidor)) && subcontratacao.ClienteExpedidor == null)
                    {
                        subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.FalhaProcessamento;
                        subcontratacao.DescricaoFalha = "Expedidor CNPJ " + dadosImportados.CNPJExpedidor + " não possui cadastro";
                    }
                    else if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(dadosImportados.CNPJRecebedor)) && subcontratacao.ClienteRecebedor == null)
                    {
                        subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.FalhaProcessamento;
                        subcontratacao.DescricaoFalha = "Recebedor CNPJ " + dadosImportados.CNPJRecebedor + " não possui cadastro";
                    }
                    else
                        subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.AgProcessamento;

                    repSubcontratacao.Atualizar(subcontratacao);

                    unitOfWork.CommitChanges();
                }
                else
                {
                    if (dadosImportados.CNPJCTe != null)
                        Servicos.Log.TratarErro("Não existe empresa cadastrada com CNPJ " + dadosImportados.CNPJCTe, "SalvarSubcontratacao");
                    else
                        Servicos.Log.TratarErro("Não existe empresa cadastrada com código integração " + dadosImportados.codigoCliente, "SalvarSubcontratacao");
                }

                return subcontratacao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();

                return null;
            }
        }

        public Dominio.ObjetosDeValor.CTe.CTe GerarObjetoCTe(Dominio.Entidades.Subcontratacao subcontratacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.SubcontratacaoDocumentos repSubcontratacaoDocumentos = new Repositorio.SubcontratacaoDocumentos(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unitOfWork);

            List<Dominio.Entidades.SubcontratacaoDocumentos> listaDocumentos = repSubcontratacaoDocumentos.BuscarPorSubcontratacao(subcontratacao.Codigo);
            if (listaDocumentos == null || listaDocumentos.Count == 0)
                return null;

            Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();

            Dominio.Entidades.ConhecimentoDeTransporteEletronico primeiroCTe = listaDocumentos.FirstOrDefault().Documento;

            cte.Emitente = new Dominio.ObjetosDeValor.CTe.Empresa
            {
                CNPJ = subcontratacao.CNPJSubcontratado
            };

            cte.TipoCTe = Dominio.Enumeradores.TipoCTE.Normal;
            cte.TipoImpressao = Dominio.Enumeradores.TipoImpressao.Retrato;
            cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
            cte.TipoServico = primeiroCTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal ? Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal : subcontratacao.TipoServico;

            cte.Remetente = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = primeiroCTe.Remetente.Bairro,
                CEP = Utilidades.String.OnlyNumbers(primeiroCTe.Remetente.CEP_SemFormato),
                CPFCNPJ = primeiroCTe.Remetente.CPF_CNPJ,
                CodigoAtividade = primeiroCTe.Remetente.Atividade.Codigo,
                CodigoIBGECidade = primeiroCTe.Remetente.Localidade.CodigoIBGE,
                Endereco = primeiroCTe.Remetente.Endereco,
                NomeFantasia = primeiroCTe.Remetente.NomeFantasia,
                Numero = primeiroCTe.Remetente.Numero,
                RGIE = primeiroCTe.Remetente.IE_RG == "ISENTO" && primeiroCTe.Remetente.Cliente != null ? primeiroCTe.Remetente.Cliente.IE_RG : primeiroCTe.Remetente.IE_RG,
                RazaoSocial = primeiroCTe.Remetente.Nome,
                NaoAtualizarEndereco = true
            };
            cte.Destinatario = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = primeiroCTe.Destinatario.Bairro,
                CEP = Utilidades.String.OnlyNumbers(primeiroCTe.Destinatario.CEP),
                CPFCNPJ = primeiroCTe.Destinatario.CPF_CNPJ,
                CodigoAtividade = primeiroCTe.Destinatario.Atividade.Codigo,
                CodigoIBGECidade = primeiroCTe.Destinatario.Localidade.CodigoIBGE,
                Endereco = primeiroCTe.Destinatario.Endereco,
                NomeFantasia = primeiroCTe.Destinatario.NomeFantasia,
                Numero = primeiroCTe.Destinatario.Numero,
                RGIE = primeiroCTe.Destinatario.IE_RG == "ISENTO" && primeiroCTe.Destinatario != null ? primeiroCTe.Destinatario.Cliente.IE_RG : primeiroCTe.Destinatario.IE_RG,
                RazaoSocial = primeiroCTe.Destinatario.Nome,
                NaoAtualizarEndereco = true
            };

            if (subcontratacao.ClienteExpedidor != null)
            {
                cte.Expedidor = new Dominio.ObjetosDeValor.CTe.Cliente()
                {
                    Bairro = subcontratacao.ClienteExpedidor.Bairro,
                    CEP = Utilidades.String.OnlyNumbers(subcontratacao.ClienteExpedidor.CEP),
                    CPFCNPJ = subcontratacao.ClienteExpedidor.CPF_CNPJ_SemFormato,
                    CodigoAtividade = subcontratacao.ClienteExpedidor.Atividade.Codigo,
                    CodigoIBGECidade = subcontratacao.ClienteExpedidor.Localidade.CodigoIBGE,
                    Endereco = subcontratacao.ClienteExpedidor.Endereco,
                    NomeFantasia = subcontratacao.ClienteExpedidor.NomeFantasia,
                    Numero = subcontratacao.ClienteExpedidor.Numero,
                    RGIE = subcontratacao.ClienteExpedidor.IE_RG,
                    RazaoSocial = subcontratacao.ClienteExpedidor.Nome,
                    NaoAtualizarEndereco = true
                };
            }

            if (subcontratacao.ClienteRecebedor != null)
            {
                cte.Recebedor = new Dominio.ObjetosDeValor.CTe.Cliente()
                {
                    Bairro = subcontratacao.ClienteRecebedor.Bairro,
                    CEP = Utilidades.String.OnlyNumbers(subcontratacao.ClienteRecebedor.CEP),
                    CPFCNPJ = subcontratacao.ClienteRecebedor.CPF_CNPJ_SemFormato,
                    CodigoAtividade = subcontratacao.ClienteRecebedor.Atividade.Codigo,
                    CodigoIBGECidade = subcontratacao.ClienteRecebedor.Localidade.CodigoIBGE,
                    Endereco = subcontratacao.ClienteRecebedor.Endereco,
                    NomeFantasia = subcontratacao.ClienteRecebedor.NomeFantasia,
                    Numero = subcontratacao.ClienteRecebedor.Numero,
                    RGIE = subcontratacao.ClienteRecebedor.IE_RG,
                    RazaoSocial = subcontratacao.ClienteRecebedor.Nome,
                    NaoAtualizarEndereco = true
                };
            }

            if (subcontratacao.Empresa.CNPJ == cte.Remetente.CPFCNPJ)
                cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
            else if (subcontratacao.Empresa.CNPJ == cte.Destinatario.CPFCNPJ)
                cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
            else if (cte.Expedidor != null && subcontratacao.Empresa.CNPJ == cte.Expedidor.CPFCNPJ)
                cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Expedidor;
            else if (cte.Recebedor != null && subcontratacao.Empresa.CNPJ == cte.Recebedor.CPFCNPJ)
                cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Recebedor;
            else
                cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;

            if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
            {
                cte.Tomador = new Dominio.ObjetosDeValor.CTe.Cliente()
                {
                    Bairro = subcontratacao.Empresa.Bairro,
                    CEP = Utilidades.String.OnlyNumbers(subcontratacao.Empresa.CEP),
                    CPFCNPJ = subcontratacao.Empresa.CNPJ,
                    CodigoAtividade = 1,
                    CodigoIBGECidade = subcontratacao.Empresa.Localidade.CodigoIBGE,
                    Endereco = subcontratacao.Empresa.Endereco,
                    NomeFantasia = subcontratacao.Empresa.NomeFantasia,
                    Numero = subcontratacao.Empresa.Numero,
                    RGIE = subcontratacao.Empresa.InscricaoEstadual,
                    RazaoSocial = subcontratacao.Empresa.RazaoSocial,
                    NaoAtualizarEndereco = true                   
                };
            }

            if (cte.Expedidor != null)
                cte.CodigoIBGECidadeInicioPrestacao = cte.Expedidor.CodigoIBGECidade;
            else
                cte.CodigoIBGECidadeInicioPrestacao = primeiroCTe.LocalidadeInicioPrestacao.CodigoIBGE;

            if (cte.Recebedor != null)
                cte.CodigoIBGECidadeTerminoPrestacao = cte.Recebedor.CodigoIBGECidade;
            else
                cte.CodigoIBGECidadeTerminoPrestacao = primeiroCTe.LocalidadeTerminoPrestacao.CodigoIBGE;

            decimal valorTotalMercadoria = 0;
            decimal pesoKg = 0;
            cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();
            cte.DocumentosTransporteAnteriores = new List<Dominio.ObjetosDeValor.CTe.DocumentoTransporteAnterior>();

            cte.QuantidadesCarga = new List<Dominio.ObjetosDeValor.CTe.QuantidadeCarga>();
            foreach (Dominio.Entidades.SubcontratacaoDocumentos documentoAnterior in listaDocumentos)
            {
                foreach (Dominio.Entidades.DocumentosCTE notaCTe in documentoAnterior.Documento.Documentos)
                {
                    if (!string.IsNullOrWhiteSpace(notaCTe.ChaveNFE))
                    {
                        Dominio.ObjetosDeValor.CTe.Documento nfe = new Dominio.ObjetosDeValor.CTe.Documento();
                        nfe.ChaveNFE = notaCTe.ChaveNFE;
                        nfe.Numero = notaCTe.Numero;
                        nfe.Serie = notaCTe.Serie;
                        nfe.Valor = notaCTe.Valor;
                        nfe.Peso = notaCTe.Peso;
                        nfe.NumeroReferenciaEDI = notaCTe.NumeroReferenciaEDI;
                        nfe.NumeroControleCliente = notaCTe.NumeroControleCliente;
                        cte.Documentos.Add(nfe);
                    }
                    else
                    {
                        Dominio.ObjetosDeValor.CTe.Documento nfe = new Dominio.ObjetosDeValor.CTe.Documento();
                        nfe.Descricao = notaCTe.Descricao;
                        nfe.Numero = notaCTe.Numero;
                        nfe.Serie = notaCTe.Serie;                        
                        nfe.Valor = notaCTe.Valor;
                        nfe.Peso = notaCTe.Peso;
                        nfe.ModeloDocumentoFiscal = notaCTe.ModeloDocumentoFiscal.Numero;                        
                        nfe.NumeroReferenciaEDI = notaCTe.NumeroReferenciaEDI;
                        nfe.NumeroControleCliente = notaCTe.NumeroControleCliente;
   
                        cte.Documentos.Add(nfe);
                    }
                }
                Dominio.ObjetosDeValor.CTe.DocumentoTransporteAnterior cteAnterior = new Dominio.ObjetosDeValor.CTe.DocumentoTransporteAnterior();
                cteAnterior.Chave = documentoAnterior.Documento.Chave;
                cteAnterior.TipoDocumento = Dominio.Enumeradores.TipoDocumentoAnteriorCTe.Eletronico;
                cteAnterior.Emissor = new Dominio.ObjetosDeValor.CTe.Cliente()
                {
                    Bairro = subcontratacao.Empresa.Bairro,
                    CEP = Utilidades.String.OnlyNumbers(subcontratacao.Empresa.CEP),
                    CPFCNPJ = subcontratacao.Empresa.CNPJ,
                    CodigoAtividade = 4,
                    CodigoIBGECidade = subcontratacao.Empresa.Localidade.CodigoIBGE,
                    Endereco = subcontratacao.Empresa.Endereco,
                    NomeFantasia = subcontratacao.Empresa.NomeFantasia,
                    Numero = subcontratacao.Empresa.Numero,
                    RGIE = subcontratacao.Empresa.InscricaoEstadual,
                    RazaoSocial = subcontratacao.Empresa.RazaoSocial
                };
                cte.DocumentosTransporteAnteriores.Add(cteAnterior);

                valorTotalMercadoria += documentoAnterior.Documento.ValorTotalMercadoria;

                //pesoKg += repInformacaoCargaCTE.ObterPesoKg(documentoAnterior.Documento.Codigo);
                List<Dominio.Entidades.InformacaoCargaCTE> informacoesCargaCTe = repInformacaoCargaCTE.BuscarPorCTe(documentoAnterior.Documento.Codigo);
                foreach (var infCarga in informacoesCargaCTe)
                {
                    Dominio.ObjetosDeValor.CTe.QuantidadeCarga quantidadeCarga = new Dominio.ObjetosDeValor.CTe.QuantidadeCarga();
                    quantidadeCarga.Descricao = infCarga.DescricaoUnidadeMedida;
                    quantidadeCarga.UnidadeMedida = infCarga.UnidadeMedida;
                    quantidadeCarga.Quantidade = infCarga.Quantidade;
                    cte.QuantidadesCarga.Add(quantidadeCarga);
                }
            }

            cte.ObservacoesGerais = subcontratacao.ObservacoesCTeSubcontratacao;

            cte.ValorTotalMercadoria = valorTotalMercadoria;
            cte.ValorFrete = subcontratacao.ValorFrete;

            if (cte.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
            {
                cte.ValorAReceber = subcontratacao.ValorFrete;
                cte.ValorTotalPrestacaoServico = subcontratacao.ValorFrete;
            }

            //Dominio.ObjetosDeValor.CTe.QuantidadeCarga quantidadeCarga = new Dominio.ObjetosDeValor.CTe.QuantidadeCarga();
            //quantidadeCarga.Descricao = "KG";
            //quantidadeCarga.UnidadeMedida = "01";
            //quantidadeCarga.Quantidade = pesoKg > 0 ? pesoKg : 1;
            //cte.QuantidadesCarga.Add(quantidadeCarga);

            cte.ExibeICMSNaDACTE = true;
            cte.IncluirICMSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Sim;
            cte.PercentualICMSIncluirNoFrete = 100;

            if (!string.IsNullOrWhiteSpace(subcontratacao.EmpresaSubcontratada?.Configuracao?.CSTCTeSubcontratacao))
            {
                cte.ICMS = new Dominio.ObjetosDeValor.CTe.ImpostoICMS()
                {
                    CST = subcontratacao.EmpresaSubcontratada.Configuracao.CSTCTeSubcontratacao
                };
            }

            return cte;
        }

        public bool ValidarDadosSubcontratacao(int codigoSubcontratacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Subcontratacao repSubcontratacao = new Repositorio.Subcontratacao(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.SubcontratacaoDocumentos repSubcontratacaoDocumentos = new Repositorio.SubcontratacaoDocumentos(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.Entidades.Subcontratacao subcontratacao = repSubcontratacao.BuscarPorCodigo(codigoSubcontratacao);

            if (subcontratacao.ValorFrete <= 0)
            {
                subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.FalhaProcessamento;
                subcontratacao.DescricaoFalha = "Valor do frete inválido.";
                repSubcontratacao.Atualizar(subcontratacao);

                return false;
            }

            if (subcontratacao.EmpresaSubcontratada == null)
            {
                subcontratacao.EmpresaSubcontratada = repEmpresa.BuscarPorCNPJ(subcontratacao.CNPJSubcontratado);
                if (subcontratacao.EmpresaSubcontratada == null)
                {
                    subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.FalhaProcessamento;
                    subcontratacao.DescricaoFalha = "Transportador " + subcontratacao.CNPJSubcontratado + " não possui cadastro";
                    repSubcontratacao.Atualizar(subcontratacao);

                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(subcontratacao.CNPJExpedidor)) && subcontratacao.ClienteExpedidor == null)
            {
                subcontratacao.ClienteExpedidor = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(subcontratacao.CNPJExpedidor)));
                if (subcontratacao.ClienteExpedidor == null)
                {
                    subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.FalhaProcessamento;
                    subcontratacao.DescricaoFalha = "Expedidor " + subcontratacao.CNPJExpedidor + " não possui cadastro";
                    repSubcontratacao.Atualizar(subcontratacao);

                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(subcontratacao.CNPJRecebedor)) && subcontratacao.ClienteRecebedor == null)
            {
                subcontratacao.ClienteRecebedor = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(subcontratacao.CNPJRecebedor)));
                if (subcontratacao.ClienteRecebedor == null)
                {
                    subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.FalhaProcessamento;
                    subcontratacao.DescricaoFalha = "Recebedor " + subcontratacao.CNPJRecebedor + " não possui cadastro";
                    repSubcontratacao.Atualizar(subcontratacao);

                    return false;
                }
            }

            List<Dominio.Entidades.SubcontratacaoDocumentos> subcontratacaoDocumentos = repSubcontratacaoDocumentos.BuscarPorSubcontratacao(subcontratacao.Codigo);
            string mensagemValidacaoDocumento = "";
            foreach (Dominio.Entidades.SubcontratacaoDocumentos documento in subcontratacaoDocumentos)
            {
                if (documento.Documento == null)
                {
                    Dominio.Enumeradores.TipoAmbiente ambiente = subcontratacao.Empresa.TipoAmbiente;
#if DEBUG
                    //ambiente = Dominio.Enumeradores.TipoAmbiente.Producao;
#endif

                    documento.Documento = repCTe.BuscarPorNumeroESerie(subcontratacao.Empresa.Codigo, documento.Numero, documento.Serie, "57", ambiente);
                    repSubcontratacaoDocumentos.Atualizar(documento);

                    if (documento.Documento == null)
                        mensagemValidacaoDocumento = string.IsNullOrWhiteSpace(mensagemValidacaoDocumento) ? documento.Numero + "/" + documento.Serie : mensagemValidacaoDocumento + " - " + documento.Numero + "/" + documento.Serie;
                }
            }

            if (!string.IsNullOrWhiteSpace(mensagemValidacaoDocumento))
            {
                subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.FalhaProcessamento;
                subcontratacao.DescricaoFalha = "CTes pendentes de importação: " + mensagemValidacaoDocumento;
                repSubcontratacao.Atualizar(subcontratacao);

                return false;
            }

            return true;
        }

        public void AtualizarEmissaoCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Subcontratacao repSubcontratacao = new Repositorio.Subcontratacao(unitOfWork);
            Dominio.Entidades.Subcontratacao subcontratacao = repSubcontratacao.BuscarPorDocumentoSubcontratacao(cte.Codigo);
            if (subcontratacao != null)
            {
                if (subcontratacao.Situacao == Dominio.Enumeradores.SituacaoSubcontratacao.RejeicaoCTe || subcontratacao.Situacao == Dominio.Enumeradores.SituacaoSubcontratacao.EmitindoCTes)
                {
                    if (cte.Status == "E" || cte.Status == "L")
                    {
                        subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.EmitindoCTes;
                        repSubcontratacao.Atualizar(subcontratacao);
                    }
                    else if (cte.Status == "R" || cte.Status == "S")
                    {
                        subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.RejeicaoCTe;
                        repSubcontratacao.Atualizar(subcontratacao);
                    }
                    else if (cte.Status == "A")
                    {
                        subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.Finalizado;
                        repSubcontratacao.Atualizar(subcontratacao);
                    }
                    else if (cte.Status == "I")
                    {
                        subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.DocumentosCancelados;
                        repSubcontratacao.Atualizar(subcontratacao);
                    }
                }
                else if (cte.Status == "C")
                {
                    subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.DocumentosCancelados;
                    repSubcontratacao.Atualizar(subcontratacao);
                }
            }
        }

        #endregion
    }
}
