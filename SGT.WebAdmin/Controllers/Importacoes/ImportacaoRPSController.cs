using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using System;

namespace SGT.WebAdmin.Controllers.Importacoes
{
    [CustomAuthorize("Importacoes/ImportacaoRPS")]
    public class ImportacaoRPSController : BaseController
    {
		#region Construtores

		public ImportacaoRPSController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoRPS();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios

                // Configuração de importacao
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoRPS();

                // Erro de campo
                string erro = string.Empty;

                // Lista integrada em cada linha
                List<Dictionary<string, dynamic>> dadosLinhas = new List<Dictionary<string, dynamic>>();

                // Entidade para importacao
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoRPS> tabelaRPS = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoRPS>();

                Servicos.Log.TratarErro("Importando planilha.", "ImportacaoRPS");

                // Chama serviço de importação
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.ImportarInformacoes(Request, configuracoes, ref tabelaRPS, ref dadosLinhas, out erro, ((dicionario) =>
                {
                    // Pesquisa por placa antes de criar uma nova
                    //if (!dicionario.TryGetValue("Placa", out dynamic placa)) placa = "";
                    Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoRPS tabela = null;// = repVeiculo.BuscarPorPlaca((string)placa);

                    // Cria nova instancia
                    if (tabela == null) tabela = new Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoRPS();

                    return tabela;
                }));

                if (retorno == null && !string.IsNullOrWhiteSpace(erro))
                    return new JsonpResult(false, true, erro);
                else if (retorno == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");

                Servicos.Log.TratarErro("Planilha importada, " + tabelaRPS.Count() + " registros.", "ImportacaoRPS");

                // Insere registros
                int contador = 0;
                for (var i = 0; i < tabelaRPS.Count; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoRPS linha = tabelaRPS[i];

                    Servicos.Log.TratarErro("Importando linha " + i.ToString() + " : " + linha.NumeroNFSe, "ImportacaoRPS");

                    if (GerarNFSe(linha, unitOfWork))
                        contador = contador + 1;

                    unitOfWork.FlushAndClear();
                }

                // Seta Retorno
                retorno.Importados = contador;

                Servicos.Log.TratarErro("Importaçao finalizada, registros importados: " + contador.ToString() + " de " + tabelaRPS.Count, "ImportacaoRPS");

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha na importação: "+ ex, "ImportacaoRPS");
                
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private bool GerarNFSe(Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoRPS dadosImportacao, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);
                Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);

                Dominio.ObjetosDeValor.NFSe.NFSeProcessada nfseProcessada = ConverteImportacaoNFSeProcessada(dadosImportacao, unitOfWork);
                if (nfseProcessada != null)
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(nfseProcessada.Emitente.CNPJ);
                    if (empresa == null)
                        throw new Exception("Emitente (" + nfseProcessada.Emitente.CNPJ + ") não cadastrado.");
                    Dominio.Entidades.Cliente tomador = repCliente.BuscarPorCPFCNPJ(double.Parse(nfseProcessada.Tomador.CPFCNPJ));
                    if (tomador == null)
                        throw new Exception("Tomador (" + nfseProcessada.Tomador.CPFCNPJ + ") não cadastrado.");

                    int.TryParse(nfseProcessada.Serie, out int serieNFSe);

                    Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);
                    Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorNumeroEStatus(empresa.Codigo, nfseProcessada.Numero, serieNFSe, null, nfseProcessada.Ambiente);

                    Dominio.Entidades.Veiculo veiculo = null;
                    if (!string.IsNullOrWhiteSpace(nfseProcessada.Placa))
                    {
                        veiculo = repVeiculo.BuscarPorPlaca(empresa.Codigo, nfseProcessada.Placa);

                        if (veiculo == null)
                        {
                            veiculo = new Dominio.Entidades.Veiculo();
                            veiculo.Placa = nfseProcessada.Placa;
                            veiculo.Empresa = empresa;
                            veiculo.Ativo = true;
                            veiculo.Estado = empresa.Localidade.Estado;
                            veiculo.Tipo = "P";
                            veiculo.TipoRodado = "01";
                            veiculo.TipoCarroceria = "00";

                            repVeiculo.Inserir(veiculo);
                        }
                    }

                    Dominio.Entidades.EmpresaSerie serie = repEmpresaSerie.BuscarPorSerie(empresa.Codigo, serieNFSe, Dominio.Enumeradores.TipoSerie.NFSe);
                    if (serie == null)
                    {
                        serie = new Dominio.Entidades.EmpresaSerie();
                        serie.Empresa = empresa;
                        serie.Numero = serieNFSe;
                        serie.Tipo = Dominio.Enumeradores.TipoSerie.NFSe;
                        serie.Status = "A";
                        repEmpresaSerie.Inserir(serie);
                    }

                    if (nfse == null)
                        nfse = svcNFSe.GravarNFSeProcessada(nfseProcessada, empresa, veiculo, serie,  unitOfWork);

                    if (nfse != null)
                    {
                        AdicionarRegistroNFSeIntegrado(nfse, nfseProcessada.NumeroCarga, nfseProcessada.NumeroUnidade, unitOfWork);

                        string status = nfse.Status == Dominio.Enumeradores.StatusNFSe.Autorizado ? "A" : (nfse.Status == Dominio.Enumeradores.StatusNFSe.Rejeicao ? "R" : (nfse.Status == Dominio.Enumeradores.StatusNFSe.Cancelado ? "C" : "P"));
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarNFSePorEmpresaNumeroESerie(nfse.Empresa.Codigo, nfse.Numero, nfse.Serie.Numero, status, DateTime.MinValue);

                        if (cte == null)
                            cte = svcNFSe.ConverterNFSeEmCTe(nfse, unitOfWork);

                        if (cte != null)
                        {
                            if (servicoCTe.GerarCargaCTe(cte.Codigo, nfseProcessada.NumeroUnidade.ToString(), nfseProcessada.NumeroCarga.ToString(), string.Empty, "Todas", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte, unitOfWork.StringConexao, unitOfWork) <= 0)
                                throw new Exception("Problema ao gerar carga NFSe CTe codigo: " + cte.Codigo);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao Gerar NFSe da importação de planilha: " + ex, "ImportacaoRPS");
                return false;
            }

        }

        private Dominio.ObjetosDeValor.NFSe.NFSeProcessada ConverteImportacaoNFSeProcessada(Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoRPS nfseImportada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.ObjetosDeValor.NFSe.NFSeProcessada nfseProcessada = new Dominio.ObjetosDeValor.NFSe.NFSeProcessada();

            Dominio.ObjetosDeValor.CTe.Empresa emitente = new Dominio.ObjetosDeValor.CTe.Empresa();
            emitente.CNPJ = Utilidades.String.OnlyNumbers(nfseImportada.CNPJEmitenteRPSNFSe);
            if (emitente.CNPJ.Length > 14)
                emitente.CNPJ = Utilidades.String.Right(emitente.CNPJ, 14);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(emitente.CNPJ);

            Dominio.ObjetosDeValor.CTe.Cliente tomador = new Dominio.ObjetosDeValor.CTe.Cliente();
            tomador.CPFCNPJ = Utilidades.String.OnlyNumbers(nfseImportada.CNPJTomadorNFSe);
            if (tomador.CPFCNPJ.Length > 14)
                tomador.CPFCNPJ = Utilidades.String.Right(tomador.CPFCNPJ, 14);
            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(tomador.CPFCNPJ));
            if (cliente != null)
            {
                tomador.Bairro = cliente.Bairro;
                tomador.CEP = cliente.CEP;
                tomador.CodigoAtividade = cliente.Atividade.Codigo;
                tomador.CodigoIBGECidade = cliente.Localidade.CodigoIBGE;
                tomador.Endereco = cliente.Endereco;
                tomador.NomeFantasia = cliente.NomeFantasia;
                tomador.Numero = cliente.Numero;
                tomador.RazaoSocial = cliente.Nome;
                tomador.RGIE = cliente.IE_RG;
            }

            Dominio.ObjetosDeValor.CTe.Cliente remetenteRPS = null;
            if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(nfseImportada.CNPJRemetente)))
            {
                string cnpjRemetente = Utilidades.String.OnlyNumbers(nfseImportada.CNPJRemetente);
                cnpjRemetente = nfseImportada.CNPJRemetente.Length > 14 ?  Utilidades.String.Right(nfseImportada.CNPJRemetente, 14) : nfseImportada.CNPJRemetente;
                Dominio.Entidades.Cliente clienteRemetente = repCliente.BuscarPorCPFCNPJ(double.Parse(cnpjRemetente));
                if (clienteRemetente != null)
                {
                    remetenteRPS = new Dominio.ObjetosDeValor.CTe.Cliente();
                    remetenteRPS.CPFCNPJ = cnpjRemetente;
                    remetenteRPS.Bairro = clienteRemetente.Bairro;
                    remetenteRPS.CEP = clienteRemetente.CEP;
                    remetenteRPS.CodigoAtividade = clienteRemetente.Atividade.Codigo;
                    remetenteRPS.CodigoIBGECidade = clienteRemetente.Localidade.CodigoIBGE;
                    remetenteRPS.Endereco = clienteRemetente.Endereco;
                    remetenteRPS.NomeFantasia = clienteRemetente.NomeFantasia;
                    remetenteRPS.Numero = clienteRemetente.Numero;
                    remetenteRPS.RazaoSocial = clienteRemetente.Nome;
                    remetenteRPS.RGIE = clienteRemetente.IE_RG;
                }
            }

            Dominio.ObjetosDeValor.NFSe.Natureza natureza = new Dominio.ObjetosDeValor.NFSe.Natureza();
            natureza.Numero = 1;
            natureza.Descricao = "Prestação de serviço de transporte";

            Dominio.ObjetosDeValor.NFSe.Servico servico = new Dominio.ObjetosDeValor.NFSe.Servico();
            servico.Aliquota = nfseImportada.AliquotaISS;
            servico.CodigoTributacao = "1601";
            servico.Descricao = "Serviço de transporte";
            servico.Numero = "16.01";

            Dominio.ObjetosDeValor.NFSe.Item item = new Dominio.ObjetosDeValor.NFSe.Item();
            item.Servico = servico;
            item.AliquotaISS = nfseImportada.AliquotaISS;
            item.BaseCalculoISS = nfseImportada.BaseISS;
            item.ValorISS = nfseImportada.ValorISS;
            item.Discriminacao = "Serviço de transporte";
            item.Quantidade = 1;
            item.ValorServico = nfseImportada.ValorServico;
            item.ValorTotal = nfseImportada.ValorServico;
            item.CodigoIBGECidade = empresa != null ? empresa.Localidade.CodigoIBGE : 0;
            item.CodigoIBGECidadeIncidencia = cliente != null ? cliente.Localidade.CodigoIBGE : empresa != null ? empresa.Localidade.CodigoIBGE : 0;
            item.CodigoPaisPrestacaoServico = 1058;

            Dominio.ObjetosDeValor.NFSe.Documentos documento = new Dominio.ObjetosDeValor.NFSe.Documentos();
            documento.ChaveNFE = "00000000000000000000000000000000000000000001";
            documento.Numero = "1";
            documento.Serie = "1";
            documento.Peso = nfseImportada.Peso;
            documento.Valor = nfseImportada.ValorMercadoria;
            documento.EmitenteNFe = remetenteRPS != null ? remetenteRPS : null;
            documento.DestinatarioNFe = tomador != null ? tomador : null;

            nfseProcessada.Emitente = emitente;
            nfseProcessada.Tomador = tomador;

            nfseProcessada.CodigoIBGECidadePrestacaoServico = cliente != null ? cliente.Localidade.CodigoIBGE : empresa != null ? empresa.Localidade.CodigoIBGE : 0;

            nfseProcessada.Natureza = natureza;
            nfseProcessada.Itens = new List<Dominio.ObjetosDeValor.NFSe.Item>();
            nfseProcessada.Itens.Add(item);

            nfseProcessada.Documentos = new List<Dominio.ObjetosDeValor.NFSe.Documentos>();
            nfseProcessada.Documentos.Add(documento);

            int.TryParse(nfseImportada.SerieNFSe, out int serieNFSe);

            nfseProcessada.Numero = nfseImportada.NumeroNFSe;
            nfseProcessada.Serie = serieNFSe.ToString();
            nfseProcessada.NumeroRPS = nfseImportada.NumeroRPS;
            nfseProcessada.SerieRPS = nfseImportada.SerieRPS;
            nfseProcessada.DataEmissao = nfseImportada.DataEmissao.ToString("dd/MM/yyyy");
            nfseProcessada.ValorServicos = nfseImportada.ValorServico;
            nfseProcessada.AliquotaISS = nfseImportada.AliquotaISS;
            nfseProcessada.BaseCalculoISS = nfseImportada.BaseISS;
            nfseProcessada.ValorISS = nfseImportada.ValorISS;
            nfseProcessada.Placa = nfseImportada.Veiculo;

            nfseProcessada.NumeroCarga = nfseImportada.Rota;
            nfseProcessada.NumeroUnidade = 0;

            nfseProcessada.Ambiente = empresa != null ? empresa.TipoAmbiente : Dominio.Enumeradores.TipoAmbiente.Homologacao;

            nfseProcessada.IBSCBS = new Dominio.ObjetosDeValor.CTe.IBSCBS();

            nfseProcessada.IBSCBS.NBS = nfseImportada.NBS;
            nfseProcessada.IBSCBS.CodigoIndicadorOperacao = nfseImportada.CodigoIndicadorOperacao;
            nfseProcessada.IBSCBS.CSTIBSCBS = nfseImportada.CSTIBSCBS;
            nfseProcessada.IBSCBS.ClassificacaoTributariaIBSCBS = nfseImportada.ClassificacaoTributariaIBSCBS;
            nfseProcessada.IBSCBS.BaseCalculoIBSCBS = nfseImportada.BaseCalculoIBSCBS;
            nfseProcessada.IBSCBS.AliquotaIBSEstadual = nfseImportada.AliquotaIBSEstadual;
            nfseProcessada.IBSCBS.PercentualReducaoIBSEstadual = nfseImportada.PercentualReducaoIBSEstadual;
            nfseProcessada.IBSCBS.ValorIBSEstadual = nfseImportada.ValorIBSEstadual;
            nfseProcessada.IBSCBS.AliquotaIBSMunicipal = nfseImportada.AliquotaIBSMunicipal;
            nfseProcessada.IBSCBS.PercentualReducaoIBSMunicipal = nfseImportada.PercentualReducaoIBSMunicipal;
            nfseProcessada.IBSCBS.ValorIBSMunicipal = nfseImportada.ValorIBSMunicipal;
            nfseProcessada.IBSCBS.AliquotaCBS = nfseImportada.AliquotaCBS;
            nfseProcessada.IBSCBS.PercentualReducaoCBS = nfseImportada.PercentualReducaoCBS;
            nfseProcessada.IBSCBS.ValorCBS = nfseImportada.ValorCBS;

            return nfseProcessada;
        }

        private bool AdicionarRegistroNFSeIntegrado(Dominio.Entidades.NFSe nfse, int numeroDaCarga, int numeroDaUnidade, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.IntegracaoNFSe repIntegracao = new Repositorio.IntegracaoNFSe(unitOfWork);

                Dominio.Entidades.IntegracaoNFSe integracao = new Dominio.Entidades.IntegracaoNFSe();

                integracao.NFSe = nfse;
                integracao.Status = Dominio.Enumeradores.StatusIntegracao.Pendente;
                integracao.TipoArquivo = Dominio.Enumeradores.TipoArquivoIntegracao.Objeto;
                integracao.Tipo = Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao;
                integracao.NumeroDaCarga = numeroDaCarga;
                integracao.NumeroDaUnidade = numeroDaUnidade;
                integracao.GerouCargaEmbarcador = true;

                repIntegracao.Inserir(integracao);

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoRPS()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Chave", Propriedade = "Chave", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "CNPJ Remetente", Propriedade = "CNPJRemetente", Tamanho = 80, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Rota", Propriedade = "Rota", Tamanho = 80, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Loja", Propriedade = "Loja", Tamanho = 80, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Número NFSe", Propriedade = "NumeroNFSe", Tamanho = 80, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Série NFSe", Propriedade = "SerieNFSe", Tamanho = 80, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Número RPS", Propriedade = "NumeroRPS", Tamanho = 80, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Série RPS", Propriedade = "SerieRPS", Tamanho = 80, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Data Emissão", Propriedade = "DataEmissao", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = "CNPJ Emitente RPS/NFSe", Propriedade = "CNPJEmitenteRPSNFSe", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = "CNPJ Tomador NFSe", Propriedade = "CNPJTomadorNFSe", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 12, Descricao = "Valor Mercadoria", Propriedade = "ValorMercadoria", Tamanho = 80, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 13, Descricao = "Peso", Propriedade = "Peso", Tamanho = 80, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 14, Descricao = "Veículo", Propriedade = "Veiculo", Tamanho = 80, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 15, Descricao = "Valor Serviço (Valor Frete)", Propriedade = "ValorServico", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 16, Descricao = "Alíquota ISS", Propriedade = "AliquotaISS", Tamanho = 80, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 17, Descricao = "Base ISS", Propriedade = "BaseISS", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 18, Descricao = "Valor ISS", Propriedade = "ValorISS", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });

            return configuracoes;
        }
    }
}
