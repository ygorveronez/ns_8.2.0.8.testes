using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using Dominio.Enumeradores;


namespace Servicos.Embarcador.Integracao.KMM
{
    public partial class IntegracaoKMM
    {
        #region Métodos Públicos

        public void IntegrarCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repositorioCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();          

                cargaCTeIntegracao.DataIntegracao = DateTime.Now;
                cargaCTeIntegracao.NumeroTentativas++;

            
                Hashtable parameters = ConverterCargaCTe(cargaCTeIntegracao.CargaCTe);

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "insDocumento" },
                    { "parameters", parameters }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                cargaCTeIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                cargaCTeIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaCTeIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(cargaCTeIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaCTeIntegracao.Atualizar(cargaCTeIntegracao);
        }

        public void IntegrarCargaCTeOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repositorioOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

                ocorrenciaCTeIntegracao.DataIntegracao = DateTime.Now;
                ocorrenciaCTeIntegracao.NumeroTentativas++;

                Hashtable parameters = ConverterCargaCTe(ocorrenciaCTeIntegracao.CargaCTe);

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "insDocumento" },
                    { "parameters", parameters }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                ocorrenciaCTeIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                ocorrenciaCTeIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                ocorrenciaCTeIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(ocorrenciaCTeIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);
        }

        public void IntegrarCargaCTeManual(Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao cargaCTeManualIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repositorioCargaCTeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

                int codigoCargaCTe = repositorioCargaCTe.BuscarCodigoPorCte(cargaCTeManualIntegracao.CTe?.Codigo ?? 0, cargaCTeManualIntegracao.Carga?.Codigo ?? 0);

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repositorioCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                #region Validar Situacao da integração da carga

                if (cargaCTe == null)
                {
                    cargaCTeManualIntegracao.NumeroTentativas++;
                    throw new Exception("CTe não possui carga vinculada");
                }

                #endregion

                cargaCTeManualIntegracao.DataIntegracao = DateTime.Now;
                cargaCTeManualIntegracao.NumeroTentativas++;

            
                Hashtable parameters = ConverterCargaCTe(cargaCTe);

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "insDocumento" },
                    { "parameters", parameters }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                cargaCTeManualIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                cargaCTeManualIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCTeManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaCTeManualIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(cargaCTeManualIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaCTeManualIntegracao.Atualizar(cargaCTeManualIntegracao);
        }

        public void IntegrarNFSeManual(Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfseManualCTeIntegracao)
        {
            Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repositorioNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
            Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual (_unitOfWork); 
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = repLancamentoNFSManual.BuscarPorCodigo(nfseManualCTeIntegracao.LancamentoNFSManual?.Codigo ?? 0);

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigoCTe(lancamentoNFSManual?.CTe?.Codigo ?? 0)?.FirstOrDefault();

                #region Validar Situacao da integração da carga

                if (cargaCTe == null || lancamentoNFSManual == null)
                {
                    nfseManualCTeIntegracao.NumeroTentativas++;
                    throw new Exception("CTe não possui carga vinculada");
                }

                #endregion

                nfseManualCTeIntegracao.DataIntegracao = DateTime.Now;
                nfseManualCTeIntegracao.NumeroTentativas++;

            
                Hashtable parameters = ConverterNFSeManual(cargaCTe, nfseManualCTeIntegracao.LancamentoNFSManual);

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "insDocumento" },
                    { "parameters", parameters }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                nfseManualCTeIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                nfseManualCTeIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                nfseManualCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                nfseManualCTeIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(nfseManualCTeIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioNFSManualCTeIntegracao.Atualizar(nfseManualCTeIntegracao);
        }

        public void IntegrarCargaCTeAgrupado(Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao cargaCTeAgrupadoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao repositorioCargaCTeAgrupadoIntegracao = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado repCargaCTeAgrupado = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();
                Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado = repCargaCTeAgrupado.BuscarPorCodigo(cargaCTeAgrupadoIntegracao.CargaCTeAgrupado?.Codigo ?? 0, false);

                if (cargaCTeAgrupado == null || cargaCTeAgrupado.CTes == null || cargaCTeAgrupado.CTes.Count == 0)
                    throw new Exception("Carga CT-e Agrupado não posssui CTE");
            
                foreach (var cte in cargaCTeAgrupado.CTes)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigoCTe(cte.Codigo)?.FirstOrDefault();

                    #region Validar Situacao da integração da carga

                    if (cargaCTe == null)
                    {
                        cargaCTeAgrupadoIntegracao.NumeroTentativas++;
                        throw new Exception("CTe não possui carga vinculada");
                    }

                    #endregion

                    cargaCTeAgrupadoIntegracao.DataIntegracao = DateTime.Now;
                    cargaCTeAgrupadoIntegracao.NumeroTentativas++;


                    Hashtable parameters = ConverterCargaCTe(cargaCTe);

                    Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "insDocumento" },
                    { "parameters", parameters }
                };

                    var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                    cargaCTeAgrupadoIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                    cargaCTeAgrupadoIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                    jsonRequisicao = retWS.jsonRequisicao;
                    jsonRetorno = retWS.jsonRetorno;
                }

                servicoArquivoTransacao.Adicionar(cargaCTeAgrupadoIntegracao, jsonRequisicao, jsonRetorno, "json");
                    
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCTeAgrupadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaCTeAgrupadoIntegracao.ProblemaIntegracao = message;
            }

            repositorioCargaCTeAgrupadoIntegracao.Atualizar(cargaCTeAgrupadoIntegracao);
        }
        #endregion Métodos Públicos

        #region Métodos Privados

        public SituacaoIntegracao BuscarSituacaoIntegracaoCargaKMM(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            return repositorioCargaIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM)?.SituacaoIntegracao ?? SituacaoIntegracao.ProblemaIntegracao;
        }

        private Hashtable ConverterCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();


            if (cargaCTe?.CTe == null)
            {
                throw new Exception("A Carga CTe não possui CTe vinculado");
            }

            var carga = cargaCTe.Carga;
            var cte = cargaCTe.CTe;
            var cargaPedido = carga.Pedidos?.ElementAt(0);

            List<String> composicao = new List<String>();
            string cavalo = cargaCTe.Carga.Veiculo?.Placa.ToString();
            if (!String.IsNullOrEmpty(cavalo))
            {
                composicao.Add(cavalo);
            }

            if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0)
            {
                for (int i = 0; i < carga.VeiculosVinculados.Count; i++)
                {
                    composicao.Add(carga.VeiculosVinculados?.ElementAt(i).Placa.ToString());
                }
            }

            // Centro de Resultado
            if (cargaPedido?.Pedido?.CentroResultado == null)
            {
                throw new Exception("O pedido da carga não possui um centro de resultado definido para ser possível realizar a integração.");
            }

            Hashtable cargaHash = new Hashtable();
            cargaHash.Add("codigo_carga_embarcador", servicoCarga.ObterNumeroCarga(carga, configuracaoEmbarcador));//carga.CodigoCargaEmbarcador?.ToString());
            cargaHash.Add("centro_custo", cargaPedido.Pedido?.CentroDeCustoViagem?.CodigoIntegracao);
            cargaHash.Add("centro_resultado", cargaPedido.Pedido?.CentroResultado?.Descricao.ToString() + "," + cargaPedido.Pedido?.CentroResultado?.Plano.ToString());
            cargaHash.Add("tipo_carga", carga.TipoDeCarga?.Descricao);
            cargaHash.Add("composicao", String.Join(",", composicao));

            Hashtable documentoHash = new Hashtable();
            documentoHash.Add("tipo", cte.ModeloDocumentoFiscal?.TipoDocumentoEmissao.ObterDescricao() ?? "");
            documentoHash.Add("protocolo", cte.Codigo.ToString());
            documentoHash.Add("modelo", cte.ModeloDocumentoFiscal?.Numero ?? "");
            documentoHash.Add("numero", cte.Numero.ToString() ?? "");
            documentoHash.Add("serie", cte.Serie?.Numero.ToString() ?? "");
            documentoHash.Add("cnpj_emitente", cte.Empresa?.CNPJ.ToString());
            documentoHash.Add("data_emissao", cte.DataEmissao?.ToString("yyyy-MM-dd HH:mm:ss"));

            string xmlString = "";

            if (cte.ModeloDocumentoFiscal.Abreviacao.Equals("CT-e"))
            {
                documentoHash.Add("chave", cte.Chave.ToString() ?? "");
                xmlString = new Servicos.WebService.CTe.CTe(_unitOfWork).ObterRetornoXMLAutorizacao(cargaCTe.CTe, _unitOfWork);
            }
            else if (cte.ModeloDocumentoFiscal.Abreviacao.Equals("NFS-e") || cte.ModeloDocumentoFiscal.Abreviacao.Equals("NFS"))
            {
                documentoHash.Add("chave", cte.Codigo.ToString());
                xmlString = this.ObterXMLDocumento(cte, cargaCTe.LancamentoNFSManual?.DadosNFS);
            }
            else if(cte.ModeloDocumentoFiscal.Abreviacao.Equals("ND")) 
            {
                documentoHash.Add("chave", cte.Codigo.ToString());
                xmlString = this.ObterXMLDocumento(cte, null);
            }
            else
            {
                throw new Exception("Não é permitido integração pra esse modelo de documento.");
            }

            documentoHash.Add("xml", Regex.Replace(xmlString, @"^\s*<\?xml.*?\?>", "", RegexOptions.IgnoreCase));


            Hashtable cargaCTeHash = new Hashtable();
            cargaCTeHash.Add("carga", cargaHash);
            cargaCTeHash.Add("documento", documentoHash);

            return cargaCTeHash;
        }

        private Hashtable ConverterNFSeManual(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.NFS.DadosNFSManual repDadosNFSManual = new Repositorio.Embarcador.NFS.DadosNFSManual(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            if (cargaCTe.CTe == null)
            {
                throw new Exception("A Carga CTe não possui CTe vinculado");
            }

            var carga = cargaCTe.Carga;
            var cte = cargaCTe.CTe;
            var cargaPedido = carga.Pedidos?.ElementAt(0);

            List<String> composicao = new List<String>();
            string cavalo = cargaCTe.Carga.Veiculo?.Placa.ToString();
            if (!String.IsNullOrEmpty(cavalo))
            {
                composicao.Add(cavalo);
            }

            if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0)
            {
                for (int i = 0; i < carga.VeiculosVinculados.Count; i++)
                {
                    composicao.Add(carga.VeiculosVinculados?.ElementAt(i).Placa.ToString());
                }
            }

            // Centro de Resultado
            if (cargaPedido?.Pedido?.CentroResultado == null)
            {
                throw new Exception("O pedido da carga não possui um centro de resultado definido para ser possível realizar a integração.");
            }

            Hashtable cargaHash = new Hashtable();
            cargaHash.Add("codigo_carga_embarcador", servicoCarga.ObterNumeroCarga(carga, configuracaoEmbarcador));//carga.CodigoCargaEmbarcador?.ToString());
            cargaHash.Add("centro_custo", cargaPedido.Pedido?.CentroDeCustoViagem?.CodigoIntegracao);
            cargaHash.Add("centro_resultado", cargaPedido.Pedido?.CentroResultado?.Descricao.ToString() + "," + cargaPedido.Pedido?.CentroResultado?.Plano.ToString());
            cargaHash.Add("tipo_carga", carga.TipoDeCarga?.Descricao);
            cargaHash.Add("composicao", String.Join(",", composicao));

            Hashtable documentoHash = new Hashtable();
            documentoHash.Add("tipo", lancamentoNFSManual?.DadosNFS?.ModeloDocumentoFiscal?.TipoDocumentoEmissao.ObterDescricao() ?? cte.ModeloDocumentoFiscal?.TipoDocumentoEmissao.ObterDescricao() ?? "");
            documentoHash.Add("protocolo", cte.Codigo.ToString());
            documentoHash.Add("modelo", lancamentoNFSManual?.DadosNFS?.ModeloDocumentoFiscal?.Numero ?? cte.ModeloDocumentoFiscal?.Numero ?? "");
            documentoHash.Add("numero", lancamentoNFSManual?.DadosNFS?.Numero.ToString() ?? cte.Numero.ToString() ?? "");
            documentoHash.Add("serie", lancamentoNFSManual?.DadosNFS?.Serie?.Numero.ToString() ?? cte.Serie?.Numero.ToString() ?? "");
            documentoHash.Add("cnpj_emitente", lancamentoNFSManual?.Transportador?.CNPJ ?? carga.Empresa?.CNPJ.ToString() ?? "");
            documentoHash.Add("data_emissao", (lancamentoNFSManual?.DadosNFS?.DataEmissao ?? cte.DataEmissao)?.ToString("yyyy-MM-dd HH:mm:ss"));
            documentoHash.Add("chave", cte.Codigo.ToString());
            string xmlString = "";

            xmlString = this.ObterXMLDocumento(cte, lancamentoNFSManual?.DadosNFS);

            documentoHash.Add("xml", Regex.Replace(xmlString, @"^\s*<\?xml.*?\?>", "", RegexOptions.IgnoreCase));

            Hashtable cargaCTeHash = new Hashtable();
            cargaCTeHash.Add("carga", cargaHash);
            cargaCTeHash.Add("documento", documentoHash);

            return cargaCTeHash;
        }


        #endregion Métodos Privados
    }
}