using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using System.IO;


namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize(new string[] { "DownloadXML", "DownloadDANFSE" }, "Ocorrencias/Ocorrencia")]
    public class OcorrenciaNFSeManualController : BaseController
    {
		#region Construtores

		public OcorrenciaNFSeManualController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos 
        public async Task<IActionResult> Anexar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
                Repositorio.Embarcador.NFS.DadosNFSManual repDadosNFSManual = new Repositorio.Embarcador.NFS.DadosNFSManual(unitOfWork);
                Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo complemento = repCargaCTeComplementoInfo.BuscarPorCodigo(codigo);

                // Valida
                if (complemento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] tipos = Request.TryGetArrayParam<string>("Tipo");
                if (arquivos.Count <= 0 || arquivos.Count > 2)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                if (complemento.CTe == null)
                    return new JsonpResult(false, true, "Não é possível anexar arquivos.");

                for (var i = 0; i < arquivos.Count(); i++)
                {
                    // Extrai dados
                    Servicos.DTO.CustomFile file = arquivos[i];
                    string tipo = i < tipos.Length ? tipos[i] : string.Empty;

                    if (tipo.Equals("XML"))
                    {
                        // Valida arquivo
                        if (Path.GetExtension(file.FileName).ToLower() != ".xml")
                            return new JsonpResult(false, true, "Arquivo de XML inválido.");

                        // Converte o xml em string
                        string xml = ConverteStreamEmString(file.InputStream);

                        if (!string.IsNullOrWhiteSpace(xml))
                        {
                            Dominio.Entidades.XMLCTe xmlCTe = new Dominio.Entidades.XMLCTe
                            {
                                XML = xml,
                                Tipo = Dominio.Enumeradores.TipoXMLCTe.Autorizacao,
                                CTe = complemento.CTe
                            };
                            repXMLCTe.Inserir(xmlCTe);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, complemento.CargaOcorrencia, null, "Anexou o XML.", unitOfWork);
                        }
                    }
                    else if (tipo.Equals("DANFSE"))
                    {
                        // Valida arquivo
                        if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
                            return new JsonpResult(false, true, "Arquivo de PDF inválido.");

                        // Converte arquivo em bytes
                        var reader = new BinaryReader(file.InputStream);
                        byte[] pdfData = reader.ReadBytes((int)file.Length);

                        // Salva DANFSE
                        this.SalvarDACTE(complemento.CTe, pdfData, unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, complemento.CargaOcorrencia, null, "Anexou a DANFSE.", unitOfWork);
                    }

                }

                // Persiste dados
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Emitir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo complemento = repCargaCTeComplementoInfo.BuscarPorCodigo(codigo);

                // Valida
                if (complemento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (complemento.CTe != null)
                    return new JsonpResult(false, true, "O documento já foi gerado.");

                //Validar se já existe um documento por Empresa, Modelo, Numero, Serie, Ambiente (pegar da empresa)

                // Preenche os dados
                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.NFSeManual dadosNFSe = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.NFSeManual();
                PreencherEntidade(ref dadosNFSe, complemento.CargaOcorrencia.ObterEmitenteOcorrencia(), unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(dadosNFSe, out string erro))
                    return new JsonpResult(false, true, erro);

                Servicos.Embarcador.Carga.Ocorrencia.GerarObjetoCTeNFSeManual(complemento, dadosNFSe, Dominio.Enumeradores.TipoCTE.Complemento, TipoServicoMultisoftware, unitOfWork);

                Servicos.Embarcador.Carga.PreCTe.VerificarEnviouTodosPreDocumentos(complemento.CargaOcorrencia, unitOfWork);

                // Persiste dados
                Servicos.Auditoria.Auditoria.Auditar(Auditado, complemento.CargaOcorrencia, null, "Adicionou uma NFS-e manual.", unitOfWork);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadDANFSE()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo complemento = repCargaCTeComplementoInfo.BuscarPorCodigo(codigo);

                // Valida
                if (complemento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                string caminho = ArquivoDANFSE(complemento.CTe, unitOfWork);

                if (!string.IsNullOrWhiteSpace(caminho))
                {
                    byte[] pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);
                    return Arquivo(pdf, "application/pdf", System.IO.Path.GetFileName(caminho));
                }
                else
                {
                    return new JsonpResult(false, false, "Ainda não foi enviada a imagem da NFS gerada.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao Baixar da DANFSE.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
                Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo complemento = repCargaCTeComplementoInfo.BuscarPorCodigo(codigo);

                // Valida
                if (complemento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.XMLCTe xml = repXMLCTe.BuscarPorCTe(complemento.CTe.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);

                if (xml != null)
                {
                    byte[] data = System.Text.Encoding.Default.GetBytes(xml.XML);
                    return Arquivo(data, "text/xml", string.Concat("NFSe_", complemento.CTe.Numero, ".xml"));
                }
                else
                {
                    return new JsonpResult(false, false, "Ainda não foi enviado o xml da NFS gerada.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao Baixar o xml.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo complemento = repCargaCTeComplementoInfo.BuscarPorCodigo(codigo);

                // Valida
                if (complemento == null || complemento.PreCTe == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                dynamic retorno = new
                {
                };

                if (complemento.CTe == null)
                    retorno = new {
                        ValorPrestacaoServico = complemento.PreCTe.ValorAReceber.ToString("n2"),
                        DataEmissao = complemento.PreCTe.DataEmissao.ToString("dd/MM/yyyy"),
                        ValorTotal = complemento.PreCTe.ValorAReceber.ToString("n2")
                    };
                else
                    retorno = new
                    {
                        complemento.Codigo,
                        Documento = complemento.CTe?.Numero ?? 0,

                        ValorTotal = complemento.CTe.ValorAReceber.ToString("n2"),
                        Numero = complemento.CTe.Numero.ToString(),
                        Serie = complemento.CTe.Serie?.Numero.ToString() ?? string.Empty,
                        ValorPrestacaoServico = complemento.CTe.ValorFrete.ToString("n2"),
                        IncluirValorBC = complemento.CTe.IncluirISSNoFrete,
                        DataEmissao = complemento.CTe.DataEmissao.HasValue ? complemento.CTe.DataEmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                        AliquotaISS = complemento.CTe.AliquotaISS.ToString("n4"),
                        ValorISS = complemento.CTe.ValorISS.ToString("n2"),
                        BaseCalculo = complemento.CTe.BaseCalculoISS.ToString("n2"),
                        PercentualRetencao = complemento.CTe.PercentualISSRetido.ToString("n2"),
                        ValorRetencao = complemento.CTe.ValorISSRetido.ToString("n2"),
                        Observacao = complemento.CTe.ObservacoesGerais
                    };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados 
        private void PreencherEntidade(ref Dominio.ObjetosDeValor.Embarcador.Ocorrencia.NFSeManual dadosNFSe, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            // Repositorios
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

            // Dados CTe
            int.TryParse(Request.Params("Numero"), out int numero);
            int.TryParse(Request.Params("Serie"), out int numeroSerie);

            bool.TryParse(Request.Params("IncluirValorBC"), out bool incluirValorBC);

            decimal.TryParse(Request.Params("ValorPrestacaoServico"), out decimal valorPrestacao);
            decimal.TryParse(Request.Params("AliquotaISS"), out decimal aliquotaISS);
            decimal.TryParse(Request.Params("ValorISS"), out decimal valorISS);
            decimal.TryParse(Request.Params("BaseCalculo"), out decimal baseCalculo);
            decimal.TryParse(Request.Params("ValorRetencao"), out decimal valorRetencao);
            decimal.TryParse(Request.Params("PercentualRetencao"), out decimal percentualRetencao);

            DateTime? dataEmissao = null;
            if (DateTime.TryParse(Request.Params("DataEmissao"), out DateTime dataEmissaoAux))
                dataEmissao = dataEmissaoAux;

            string observacao = Request.Params("Observacao") ?? string.Empty;

            // Cria serie
            Dominio.Entidades.EmpresaSerie serie = repEmpresaSerie.BuscarPorSerie(empresa.Codigo, numeroSerie, Dominio.Enumeradores.TipoSerie.NFSe);
            if (serie == null && numeroSerie > 0)
            {
                serie = new Dominio.Entidades.EmpresaSerie()
                {
                    Empresa = empresa,
                    Numero = numeroSerie,
                    Status = "A",
                    Tipo = Dominio.Enumeradores.TipoSerie.NFSe,
                };
                repEmpresaSerie.Inserir(serie, Auditado);
            }

            dadosNFSe.Numero = numero;
            dadosNFSe.Serie = serie.Numero;
            dadosNFSe.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.NFS);
            dadosNFSe.IncluirISSBC = incluirValorBC;
            dadosNFSe.DataEmissao = dataEmissao;
            dadosNFSe.ValorFrete = valorPrestacao;
            dadosNFSe.AliquotaISS = aliquotaISS;
            dadosNFSe.ValorISS = valorISS;
            dadosNFSe.ValorBaseCalculo = baseCalculo;
            dadosNFSe.PercentualRetencao = percentualRetencao;
            dadosNFSe.ValorRetido = valorRetencao;
            dadosNFSe.Observacao = observacao;
        }

        private bool ValidaEntidade(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.NFSeManual dadosNFSe, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            if (dadosNFSe.Numero == 0)
            {
                msgErro = "Número é obrigatório.";
                return false;
            }

            //if (dadosNFSe.Serie == null)
            //{
            //    msgErro = "Série é obrigatório.";
            //    return false;
            //}

            if (dadosNFSe.ValorFrete == 0)
            {
                msgErro = "Valor Prestação do Serviço é obrigatório.";
                return false;
            }

            if (dadosNFSe.AliquotaISS == 0)
            {
                msgErro = "Aliquota ISS é obrigatório.";
                return false;
            }

            /*if (dados.ValorISS == 0)
            {
                msgErro = "Valor ISS é obrigatório.";
                return false;
            }

            if (dados.ValorBaseCalculo == 0)
            {
                msgErro = "Base de Cálculo é obrigatório.";
                return false;
            }*/

            //if (dados.PercentualRetencao == 0)
            //{
            //    msgErro = "Percentual de Retenção é obrigatório.";
            //    return false;
            //}

            return true;
        }

        public string ArquivoDANFSE(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            if (!string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios))
            {
                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios, "NFSe", cte.Empresa.CNPJ);
                //C:/Relatorios/NFSe/00000000000000/
                string arquivoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoPDF, cte.Codigo.ToString() + "_" + cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString()) + ".pdf";
                //C:/Relatorios/NFSe/00000000000000/100_20.pdf

                return arquivoPDF;
            }
            else
            {
                return "";
            }
        }

        public string SalvarDACTE(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, byte[] pdfData, Repositorio.UnitOfWork unitOfWork)
        {
            if (!string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios))
            {
                string arquivoPDF = ArquivoDANFSE(cte, unitOfWork);

                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(arquivoPDF, pdfData);
                return arquivoPDF;
            }
            else
            {
                return "";
            }
        }

        private string ConverteStreamEmString(Stream filestream)
        {
            List<string> xmlData = new List<string>();
            StreamReader reader = new StreamReader(filestream);
            while (!reader.EndOfStream) xmlData.Add(reader.ReadLine());
            
            return String.Join("", xmlData);
        }
        #endregion
    }
}
