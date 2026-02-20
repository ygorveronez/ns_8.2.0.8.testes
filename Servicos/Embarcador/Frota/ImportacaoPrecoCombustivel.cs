using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Frota
{
    public class ImportacaoPrecoCombustivel : ServicoBase
    {
        #region Construtores

        public ImportacaoPrecoCombustivel(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ImportacaoPrecoCombustivel() : base() { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoPrecoCombustivel(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "CNPJ do Posto", Propriedade = "CNPJPosto", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" }  },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Código Integração Produto", Propriedade = "CodigoIntegracaoProduto", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" }  },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Preço de", Propriedade = "Preco", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" }  },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Preço até", Propriedade = "PrecoAte", Tamanho = 200, Obrigatorio = false},
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Unidade de Medida", Propriedade = "UnidadeDeMedida", Tamanho = 200, Obrigatorio = false },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Código Tabela", Propriedade = "CodigoTabela", Tamanho = 200, Obrigatorio = false },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "% Desconto", Propriedade = "PercentualDesconto", Tamanho = 200, Obrigatorio = false },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Data Inicial", Propriedade = "DataInicial", Tamanho = 200, Obrigatorio = false },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Data Final", Propriedade = "DataFinal", Tamanho = 200, Obrigatorio = false }
            };

            return configuracoes;
        }

        public static bool GerarImportacaoPrecoCombustivel(out Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno, string nomeArquivo, string dadosArquivo, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dadosArquivo);

            if (linhas.Count == 0)
            {
                retorno.MensagemAviso = "Nenhuma linha encontrada na planilha";
                return false;
            }

            Repositorio.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivel repImportacaoPrecoCombustivel = new Repositorio.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivel(unitOfWork);
            Repositorio.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha repImportacaoPrecoCombustivelLinha = new Repositorio.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha(unitOfWork);
            Repositorio.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna repImportacaoPrecoCombustivelLinhaColuna = new Repositorio.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna(unitOfWork);

            unitOfWork.Start();

            Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivel importacaoPedido = new Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivel
            {
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente,
                Planilha = nomeArquivo,
                QuantidadeLinhas = linhas.Count,
                Usuario = usuario,
                DataImportacao = DateTime.Now
            };

            repImportacaoPrecoCombustivel.Inserir(importacaoPedido, auditado);

            for (int i = 0; i < importacaoPedido.QuantidadeLinhas; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha dadosLinhaArquivo = linhas[i];

                Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha linha = new Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha()
                {
                    ImportacaoPrecoCombustivel = importacaoPedido,
                    Numero = i + 1,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente
                };

                repImportacaoPrecoCombustivelLinha.Inserir(linha);

                for (int j = 0; j < dadosLinhaArquivo.Colunas.Count; j++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna dadosColunaArquivo = dadosLinhaArquivo.Colunas[j];

                    Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna coluna = new Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna()
                    {
                        Linha = linha,
                        NomeCampo = dadosColunaArquivo.NomeCampo,
                        Valor = !string.IsNullOrWhiteSpace((string)dadosColunaArquivo.Valor) ? (string)dadosColunaArquivo.Valor : string.Empty
                    };

                    repImportacaoPrecoCombustivelLinhaColuna.Inserir(coluna);
                }
            }

            unitOfWork.CommitChanges();

            retorno.MensagemAviso = " Planilha adicionada com sucesso à fila de processamento.";
            retorno.Total = linhas.Count;
            retorno.Importados = linhas.Count;

            return true;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frota.RetornoImportacaoPrecoCombustivel ImportarPrecoCombustivel(Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivel importacaoPedido, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Frota.RetornoImportacaoPrecoCombustivel retorno = new Dominio.ObjetosDeValor.Embarcador.Frota.RetornoImportacaoPrecoCombustivel();

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                Usuario = usuario,
                Empresa = usuario?.Empresa,
                Texto = ""
            };

            Repositorio.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivel repImportacaoPrecoCombustivel = new Repositorio.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivel(unitOfWork);
            Repositorio.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha repImportacaoPrecoCombustivelLinha = new Repositorio.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha(unitOfWork);
            Repositorio.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna repImportacaoPrecoCombustivelLinhaColuna = new Repositorio.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
            retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();


            List<int> codigosLinhasGerar = repImportacaoPrecoCombustivelLinha.BuscarCodigosLinhasPendentesGeracaoPedido(importacaoPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna> colunasGerar = repImportacaoPrecoCombustivelLinhaColuna.BuscarPorImportacaoPendentesGeracaoPostoCombustivelTabelaValores(importacaoPedido.Codigo);
            int contador = 0;
            string retornoFinaliza = "";
            try
            {
                // Importa cada linha como uma tabela de preço
                for (int i = 0; i < codigosLinhasGerar.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinha linha = repImportacaoPrecoCombustivelLinha.BuscarPorCodigo(codigosLinhasGerar[i], false);

                    unitOfWork.Start();
                    linha.Situacao = SituacaoImportacaoPedido.Processando;
                    repImportacaoPrecoCombustivelLinha.Atualizar(linha);
                    unitOfWork.CommitChanges();

                    List<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna> colunas = colunasGerar.Where(o => o.Linha.Codigo == linha.Codigo).ToList();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha dadosLinha = Servicos.Embarcador.Frota.ImportacaoPrecoCombustivel.ConverterParaImportacao(colunas);

                    unitOfWork.Start();

                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = ImportarPrecoCombustivel(dadosLinha, ValueTuple.Create("", ""), usuario, tipoServicoMultisoftware, configuracaoTMS, auditado, unitOfWork);

                        if (retornoLinha.contar)
                        {
                            if (retornoLinha.codigo > 0)
                            {
                                linha.PostoCombustivelTabelaValores = new Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores() { Codigo = retornoLinha.codigo };
                                linha.Situacao = SituacaoImportacaoPedido.Sucesso;
                                linha.Mensagem = "Tabela de preço importada.";
                                repImportacaoPrecoCombustivelLinha.Atualizar(linha);
                                contador++;
                            }
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            unitOfWork.Start();
                            linha.Situacao = SituacaoImportacaoPedido.Erro;
                            linha.Mensagem = retornoLinha.mensagemFalha;
                            repImportacaoPrecoCombustivelLinha.Atualizar(linha);
                        }

                        unitOfWork.CommitChanges();
                        unitOfWork.FlushAndClear();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        unitOfWork.Start();
                        linha.Situacao = SituacaoImportacaoPedido.Erro;
                        linha.Mensagem = ex.Message;
                        repImportacaoPrecoCombustivelLinha.Atualizar(linha);
                        unitOfWork.CommitChanges();
                    }
                }

                retorno.TotalTabelasPreco = repImportacaoPrecoCombustivelLinha.ContarPostoCombustivelTabelaValoresPorImportacaoPrecoCombustivel(importacaoPedido.Codigo);
                retorno.Sucesso = (retorno.TotalTabelasPreco > 0);
                retorno.Mensagem = string.IsNullOrWhiteSpace(retornoFinaliza) ? "Importação de preços de combustíveis finalizado (acompanhe o status das linhas)." : retornoFinaliza;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                retorno.Sucesso = false;
                retorno.Mensagem = ex.Message;
            }

            return retorno;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha ConverterParaImportacao(List<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna> colunas)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha()
            {
                Colunas = colunas.Select(o => new Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna()
                {
                    NomeCampo = o.NomeCampo,
                    Valor = o.Valor
                }).ToList()
            };
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha ImportarPrecoCombustivel(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, (string Nome, string Guid) arquivoGerador, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores repPostoCombustivelTabelaValores = new Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinhaPedido = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha();

            try
            {
                Dominio.Entidades.Cliente fornecedor = null;
                decimal preco = 0;
                decimal precoAte = 0m;
                decimal percentualDesconto = 0;
                string codigoIntegracaoProduto = "";
                string codigoTabela = "";
                DateTime? dataInicial = null;
                DateTime? dataFinal = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida unidadeDeMedida = Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Litros;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPessoa = (from obj in linha.Colunas where obj.NomeCampo == "CNPJPosto" select obj).FirstOrDefault();
                string somenteNumeros = "";
                if (colPessoa != null)
                {
                    somenteNumeros = Utilidades.String.OnlyNumbers((string)colPessoa.Valor);
                    if (!string.IsNullOrEmpty(somenteNumeros))
                    {
                        double cpfCNPJRemetente = double.Parse(somenteNumeros);
                        fornecedor = repCliente.BuscarPorCPFCNPJ(cpfCNPJRemetente);
                    }
                }
                if (fornecedor == null)
                    return RetornarFalhaLinha("Não foi localizado o fornecedor com o CNPJ " + somenteNumeros + ".");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValor = (from obj in linha.Colunas where obj.NomeCampo == "Preco" select obj).FirstOrDefault();
                if (colValor != null && !string.IsNullOrWhiteSpace((string)colValor.Valor))
                {
                    string strValor = (string)colValor.Valor;
                    decimal.TryParse(strValor, out preco);
                }
                if (preco <= 0)
                    return RetornarFalhaLinha("Não foi informado o preço.");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorAte = (from obj in linha.Colunas where obj.NomeCampo == "PrecoAte" select obj).FirstOrDefault();
                if (colValorAte != null && !string.IsNullOrWhiteSpace((string)colValorAte.Valor))
                {
                    string strValorAte = (string)colValorAte.Valor;
                    decimal.TryParse(strValorAte, out precoAte);
                }
                if (preco > 0m && precoAte > 0m && preco > precoAte)
                    return RetornarFalhaLinha("O 'Preço até' deve ser maior que o 'Preço de'.");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoIntegracaoProduto = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracaoProduto" select obj).FirstOrDefault();
                if (colCodigoIntegracaoProduto != null)
                    codigoIntegracaoProduto = (string)colCodigoIntegracaoProduto.Valor;

                if (string.IsNullOrWhiteSpace(codigoIntegracaoProduto))
                    return RetornarFalhaLinha("Não foi informado o código de integração do produto.");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colUnidadeDeMedida = (from obj in linha.Colunas where obj.NomeCampo == "UnidadeDeMedida" select obj).FirstOrDefault();
                if (colUnidadeDeMedida != null)
                    unidadeDeMedida = ((string)colUnidadeDeMedida.Valor).ToEnum(Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Litros);

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPercentualDesconto = (from obj in linha.Colunas where obj.NomeCampo == "PercentualDesconto" select obj).FirstOrDefault();
                if (colPercentualDesconto != null && !string.IsNullOrWhiteSpace((string)colPercentualDesconto.Valor))
                {
                    string strValor = (string)colPercentualDesconto.Valor;
                    decimal.TryParse(strValor, out percentualDesconto);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoTabela = (from obj in linha.Colunas where obj.NomeCampo == "CodigoTabela" select obj).FirstOrDefault();
                if (colCodigoTabela != null)
                    codigoTabela = (string)colCodigoTabela.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataInicial = (from obj in linha.Colunas where obj.NomeCampo == "DataInicial" select obj).FirstOrDefault();
                string dataDescarga = "";
                if (colDataInicial != null)
                {
                    dataDescarga = colDataInicial.Valor;
                    double.TryParse(dataDescarga, out double dataFormatoExcel);
                    if (dataFormatoExcel > 0)
                        dataInicial = Utilidades.DateTime.ConverterDataExcelToDateTime(dataFormatoExcel);
                    else if (!string.IsNullOrWhiteSpace(dataDescarga))
                    {
                        DateTime dataInicialTest;
                        DateTime.TryParse(dataDescarga, out dataInicialTest);
                        if (dataInicialTest > DateTime.MinValue)
                            dataInicial = dataInicialTest;
                    }
                }
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataFinal = (from obj in linha.Colunas where obj.NomeCampo == "DataFinal" select obj).FirstOrDefault();
                dataDescarga = "";
                if (colDataFinal != null)
                {
                    dataDescarga = colDataFinal.Valor;
                    double.TryParse(dataDescarga, out double dataFormatoExcel);
                    if (dataFormatoExcel > 0)
                        dataFinal = Utilidades.DateTime.ConverterDataExcelToDateTime(dataFormatoExcel);
                    else if (!string.IsNullOrWhiteSpace(dataDescarga))
                    {
                        DateTime dataFinalTest;
                        DateTime.TryParse(dataDescarga, out dataFinalTest);
                        if (dataFinalTest > DateTime.MinValue)
                            dataFinal = dataFinalTest;
                    }
                }

                retornoLinhaPedido = AdicionarPostoCombustivelTabelaValores(fornecedor.CPF_CNPJ, codigoIntegracaoProduto, preco, precoAte, unidadeDeMedida, codigoTabela, percentualDesconto, dataInicial, dataFinal, unitOfWork, configuracaoTMS, tipoServicoMultisoftware, auditado);
                if (!string.IsNullOrWhiteSpace(retornoLinhaPedido.mensagemFalha))
                    return RetornarFalhaLinha(retornoLinhaPedido.mensagemFalha);

            }
            catch (Exception ex2)
            {
                Servicos.Log.TratarErro(ex2);
                return RetornarFalhaLinha("Ocorreu uma falha ao processar a linha (" + ex2.Message + ").");
            }

            return RetornarSucessoLinha(retornoLinhaPedido?.codigo ?? 0);

        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, bool contar = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { mensagemFalha = mensagem, processou = false, contar = contar };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarSucessoLinha(int codigo)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { codigo = codigo, mensagemFalha = "", processou = true, contar = true };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha AdicionarPostoCombustivelTabelaValores(double cnpjPosto, string codigoIntegracaoProduto, decimal preco, decimal precoAte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida unidadeDeMedida, string codigoTabela, decimal percentualDesconto, DateTime? dataInicial, DateTime? dataFinal, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            string retorno = "";

            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
            Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores repPostoCombustivelTabelaValores = new Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores tabelaValor = null;

            tabelaValor = repPostoCombustivelTabelaValores.BuscarTabelaPorParametros(cnpjPosto, codigoIntegracaoProduto, dataInicial, dataFinal);

            if (tabelaValor != null)
            {
                tabelaValor.Initialize();
                tabelaValor.ValorFixo = preco;
                tabelaValor.ValorAte = precoAte;
                tabelaValor.Integrado = false;
                if (dataInicial.HasValue && dataInicial.Value > DateTime.MinValue)
                    tabelaValor.DataInicial = dataInicial;
                if (dataFinal.HasValue && dataFinal.Value > DateTime.MinValue)
                    tabelaValor.DataFinal = dataFinal;

                repPostoCombustivelTabelaValores.Atualizar(tabelaValor, auditado);
            }
            else
            {
                Dominio.Entidades.Cliente posto = repCliente.BuscarPorCPFCNPJ(cnpjPosto);
                if (posto == null)
                {
                    return new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha
                    {
                        codigo = 0,
                        mensagemFalha = "Posto com o CNPJ " + Utilidades.String.OnlyNumbers(cnpjPosto.ToString()) + " não cadastrado. "
                    };
                }
                Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigoProduto(codigoIntegracaoProduto);
                if (produto == null)
                {
                    return new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha
                    {
                        codigo = 0,
                        mensagemFalha = "Produto com o código de integração " + codigoIntegracaoProduto + " não cadastrado. "
                    };
                }

                Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = RetornarModalidadePessoa(posto, TipoModalidade.Fornecedor, unitOfWork, auditado);
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoas = repModalidadeFornecedorPessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

                if (modalidadeFornecedorPessoas == null)
                    modalidadeFornecedorPessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas();
                else
                    modalidadeFornecedorPessoas.Initialize();

                modalidadeFornecedorPessoas.ModalidadePessoas = modalidadePessoas;
                modalidadeFornecedorPessoas.PostoConveniado = true;

                if (modalidadeFornecedorPessoas.Codigo == 0)
                {
                    repModalidadeFornecedorPessoas.Inserir(modalidadeFornecedorPessoas, auditado);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, posto, null, "Adicionou a Modalidade fornecedor " + modalidadeFornecedorPessoas.Descricao + ".", unitOfWork);
                }
                else
                {
                    repModalidadeFornecedorPessoas.Atualizar(modalidadeFornecedorPessoas, auditado);
                    var alteracoes = modalidadeFornecedorPessoas.GetChanges();
                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(auditado, posto, alteracoes, "Alterou a Modalidade fornecedor " + modalidadeFornecedorPessoas.Descricao + ".", unitOfWork);
                }

                tabelaValor = new Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores()
                {
                    CodigoIntegracao = codigoIntegracaoProduto,//codigoTabela,
                    DataFinal = dataFinal,
                    DataInicial = dataInicial,
                    ModalidadeFornecedorPessoas = modalidadeFornecedorPessoas,
                    PercentualDesconto = percentualDesconto,
                    Produto = produto,
                    UnidadeDeMedida = unidadeDeMedida,
                    ValorFixo = preco,
                    ValorAte = precoAte,
                    Integrado = false
                };
                repPostoCombustivelTabelaValores.Inserir(tabelaValor, auditado);
            }

            if (tabelaValor != null && tabelaValor.Codigo > 0)
            {
                return new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha
                {
                    codigo = tabelaValor.Codigo,
                    mensagemFalha = retorno
                };
            }
            else
            {
                return new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha
                {
                    codigo = 0,
                    mensagemFalha = "Tabela de preço não inserida, favor valide os dados informados"
                };
            }
        }

        private Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas RetornarModalidadePessoa(Dominio.Entidades.Cliente pessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade tipoModalidade, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = repModalidadePessoas.BuscarPorTipo(tipoModalidade, pessoa.CPF_CNPJ);

            if (modalidadePessoas == null)
            {
                modalidadePessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas();
                modalidadePessoas.Cliente = pessoa;
                modalidadePessoas.TipoModalidade = tipoModalidade;
                repModalidadePessoas.Inserir(modalidadePessoas);

                Servicos.Auditoria.Auditoria.Auditar(auditado, pessoa, null, "Adicionou a Modalidade " + modalidadePessoas.DescricaoTipoModalidade + " a pessoa", unitOfWork);
            }
            return modalidadePessoas;
        }

        #endregion
    }
}
