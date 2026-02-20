using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Pedido
{
    public class ImportacaoTakePay : ServicoBase
    {
        #region Construtores

        public ImportacaoTakePay(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ImportacaoTakePay() : base() { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoTakePay(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Número Referencia", Propriedade = "NumeroReferencia", Tamanho = 20, Obrigatorio = true, Regras = new List<string> { "required" }  },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Nome Cliente", Propriedade = "NomeCliente", Tamanho = 500, Obrigatorio = true, Regras = new List<string> { "required" }  },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "CNPJ Cliente", Propriedade = "CNPJCliente", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" }  },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Navio/Viagem/Direção", Propriedade = "Viagem", Tamanho = 300, Obrigatorio = true, Regras = new List<string> { "required" }  },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "POL", Propriedade = "PortoOrigem", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" }  },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Terminal Origem", Propriedade = "TerminalOrigem", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { "required" }  },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "POD", Propriedade = "PortoDestino", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" }  },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Terminal Destino", Propriedade = "TerminalDestino", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { "required" }  },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Tipo de Proposta", Propriedade = "TipoProposta", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" }  },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = "Quantidade de unidades disponibilzadas", Propriedade = "QtdDisponibilizada", Tamanho = 100, Obrigatorio = false  },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = "Quantidade de unidades não embarcadas", Propriedade = "QtdNaoEmbarcadas", Tamanho = 100, Obrigatorio = false},
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 12, Descricao = "Valor", Propriedade = "Valor", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" }  },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 13, Descricao = "Observação", Propriedade = "Observacao", Tamanho = 2000, Obrigatorio = true, Regras = new List<string> { "required" } },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 14, Descricao = "Unidades (Contabilidade)", Propriedade = "Unidades", Tamanho = 20, Obrigatorio = false},
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 15, Descricao = "Alíquota ICMS", Propriedade = "AliquotaICMS", Tamanho = 200, Obrigatorio = false},
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 16, Descricao = "Valor ICMS", Propriedade = "ValorICMS", Tamanho = 200, Obrigatorio = false},
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 17, Descricao = "PTAX", Propriedade = "PTAX", Tamanho = 200, Obrigatorio = false},
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 18, Descricao = "Valor (USD)", Propriedade = "ValorUSD", Tamanho = 200, Obrigatorio = false},
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 19, Descricao = "PO Number", Propriedade = "PONumber", Tamanho = 2000, Obrigatorio = false},
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 20, Descricao = "Descricao do Grupo Pessoas", Propriedade = "GrupoPessoas", Tamanho = 500, Obrigatorio = false}
            };

            return configuracoes;
        }

        public static bool GerarImportacaoTakePay(out Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno, string nomeArquivo, string dadosArquivo, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dadosArquivo);

            if (linhas.Count == 0)
            {
                retorno.MensagemAviso = "Nenhuma linha encontrada na planilha";
                return false;
            }

            Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay repImportacaoTakePay = new Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay(unitOfWork);
            Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha repImportacaoTakePayLinha = new Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha(unitOfWork);
            Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna repImportacaoTakePayLinhaColuna = new Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna(unitOfWork);

            unitOfWork.Start();

            Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay importacaoPedido = new Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay
            {
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente,
                Planilha = nomeArquivo,
                QuantidadeLinhas = linhas.Count,
                Usuario = usuario,
                DataImportacao = DateTime.Now
            };

            repImportacaoTakePay.Inserir(importacaoPedido, auditado);

            for (int i = 0; i < importacaoPedido.QuantidadeLinhas; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha dadosLinhaArquivo = linhas[i];

                Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha linha = new Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha()
                {
                    ImportacaoTakePay = importacaoPedido,
                    Numero = i + 1,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente
                };

                repImportacaoTakePayLinha.Inserir(linha);

                for (int j = 0; j < dadosLinhaArquivo.Colunas.Count; j++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna dadosColunaArquivo = dadosLinhaArquivo.Colunas[j];

                    Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna coluna = new Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna()
                    {
                        Linha = linha,
                        NomeCampo = dadosColunaArquivo.NomeCampo,
                        Valor = (string)dadosColunaArquivo.Valor
                    };

                    if (string.IsNullOrWhiteSpace(coluna.Valor))
                        coluna.Valor = "";

                    repImportacaoTakePayLinhaColuna.Inserir(coluna);
                }
            }

            unitOfWork.CommitChanges();

            retorno.MensagemAviso = "Planilha adicionada com sucesso à fila de processamento.";
            retorno.Total = linhas.Count;
            retorno.Importados = linhas.Count;

            return true;
        }

        public Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoImportacaoPedidoGerarCarga ImportarPedidoGerarCargas(Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay importacaoPedido, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string AdminStringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoImportacaoPedidoGerarCarga retorno = new Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoImportacaoPedidoGerarCarga();

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                Usuario = usuario,
                Empresa = usuario?.Empresa,
                Texto = ""
            };

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay repImportacaoTakePay = new Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay(unitOfWork);
            Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha repImportacaoTakePayLinha = new Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha(unitOfWork);
            Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna repImportacaoTakePayLinhaColuna = new Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna(unitOfWork);
            Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);

            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = usuario != null ? repOperadorLogistica.BuscarPorUsuario(usuario.Codigo) : null;

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
            retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas = new List<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();
            List<string> cargasParaCancelamento = new List<string>();
            string prefixoPreCarga = Guid.NewGuid().ToString().Replace("-", "").Substring(5, 10);//cria o prefixo randomico para não correr o risco de inserir um pedido em uma pre carga já existente
            string motivoCancelamentoCarga = "Carga cancelada via importação de planilha na tela de pedidos";

            List<int> codigosLinhasGerar = repImportacaoTakePayLinha.BuscarCodigosLinhasPendentesGeracaoPedido(importacaoPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna> colunasGerar = repImportacaoTakePayLinhaColuna.BuscarPorImportacaoPendentesGeracaoPedido(importacaoPedido.Codigo);
            int contador = 0;
            string retornoFinaliza = "";
            try
            {
                // Importa cada linha como um pedido
                for (int i = 0; i < codigosLinhasGerar.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha linha = repImportacaoTakePayLinha.BuscarPorCodigo(codigosLinhasGerar[i], false);

                    unitOfWork.Start();
                    linha.Situacao = SituacaoImportacaoPedido.Processando;
                    repImportacaoTakePayLinha.Atualizar(linha);
                    unitOfWork.CommitChanges();

                    List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna> colunas = colunasGerar.Where(o => o.Linha.Codigo == linha.Codigo).ToList();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha dadosLinha = Servicos.Embarcador.Pedido.ImportacaoTakePay.ConverterParaImportacao(colunas);

                    unitOfWork.Start();

                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = ImportarPedidoLinha(dadosLinha, ValueTuple.Create("", ""), prefixoPreCarga, preCargas, cargasParaCancelamento, motivoCancelamentoCarga, usuario, operadorLogistica, true, tipoServicoMultisoftware, configuracaoTMS, auditado, AdminStringConexao, unitOfWork);

                        if (retornoLinha.processou)
                        {
                            if (retornoLinha.codigo > 0)
                            {
                                linha.Pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido() { Codigo = retornoLinha.codigo };
                                linha.Situacao = SituacaoImportacaoPedido.Sucesso;
                                linha.Mensagem = "Pedido importado.";
                                repImportacaoTakePayLinha.Atualizar(linha);
                                contador++;
                            }
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            unitOfWork.Start();
                            linha.Situacao = SituacaoImportacaoPedido.Erro;
                            linha.Mensagem = retornoLinha.mensagemFalha;
                            retornoFinaliza = retornoLinha.mensagemFalha;
                            repImportacaoTakePayLinha.Atualizar(linha);
                        }

                        unitOfWork.CommitChanges();
                        unitOfWork.FlushAndClear();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        unitOfWork.Rollback();
                        unitOfWork.Start();
                        linha.Situacao = SituacaoImportacaoPedido.Erro;
                        linha.Mensagem = ex.Message;
                        retornoFinaliza = ex.Message;
                        repImportacaoTakePayLinha.Atualizar(linha);
                        unitOfWork.CommitChanges();
                    }
                }

                if (string.IsNullOrWhiteSpace(retornoFinaliza))//só vai gerar todas as cargas se todas as linhas forem processadas sem nenhum erro
                {
                    // Agrupa os pedidos por veículo e tipo de operação
                    List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha> linhasGeradas = repImportacaoTakePayLinha.BuscarSemCargaPorImportacaoTakePay(importacaoPedido.Codigo);

                    // Gera uma carga para cada grupo de pedidos agrupados por veículo e tipo de operação
                    foreach (Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha agrupamentoLinhas in linhasGeradas)
                    {
                        if (agrupamentoLinhas.Pedido == null)
                            continue;

                        int codigosPedidos = agrupamentoLinhas.Pedido.Codigo;
                        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigosPedidos);
                        List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinha> linhas = repImportacaoTakePayLinha.BuscarPorPedido(codigosPedidos);

                        unitOfWork.Start();
                        try
                        {
                            string mensagemRetornoCarga = Servicos.Embarcador.Pedido.Pedido.CriarCarga(out Dominio.Entidades.Embarcador.Cargas.Carga carga, new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>() { pedido }, unitOfWork, tipoServicoMultisoftware, null, configuracaoTMS, true, false, false, false);
                            if (!string.IsNullOrWhiteSpace(mensagemRetornoCarga))
                            {
                                unitOfWork.Rollback();

                                unitOfWork.Start();
                                int total = linhas.Count();
                                for (int i = 0; i < total; i++)
                                {
                                    linhas[i].Situacao = SituacaoImportacaoPedido.Erro;
                                    linhas[i].Mensagem = (linhas[i].Mensagem + " " + mensagemRetornoCarga).Trim();
                                    repImportacaoTakePayLinha.Atualizar(linhas[i]);
                                }
                            }
                            if (carga != null)
                            {
                                repImportacaoTakePayLinha.SetarCargaLinhas(agrupamentoLinhas.Codigo, carga.Codigo);
                            }
                            unitOfWork.CommitChanges();
                            unitOfWork.FlushAndClear();
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            unitOfWork.Rollback();
                            int total = linhas.Count();
                            for (int i = 0; i < total; i++)
                            {
                                linhas[i].Situacao = SituacaoImportacaoPedido.Erro;
                                linhas[i].Mensagem = (linhas[i].Mensagem + " " + ex.Message).Trim();
                                repImportacaoTakePayLinha.Atualizar(linhas[i]);
                            }
                            unitOfWork.CommitChanges();
                        }
                    }

                    retorno.TotalPedidos = repImportacaoTakePayLinha.ContarPedidosPorImportacaoTakePay(importacaoPedido.Codigo);
                    retorno.TotalCargas = repImportacaoTakePayLinha.ContarCargasPorImportacaoTakePay(importacaoPedido.Codigo);
                    retorno.Sucesso = (retorno.TotalPedidos > 0 || retorno.TotalCargas > 0);
                    retorno.Mensagem = string.IsNullOrWhiteSpace(retornoFinaliza) ? "Importação de pedidos Embarque Certo/No Show finalizado (acompanhe o status)." : retornoFinaliza;
                }
                else
                {
                    retorno.TotalPedidos = 0;
                    retorno.TotalCargas = 0;
                    retorno.Sucesso = false;
                    retorno.Mensagem = retornoFinaliza;
                }
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

        public static Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha ConverterParaImportacao(List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePayLinhaColuna> colunas)
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

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha ImportarPedidoLinha(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, (string Nome, string Guid) arquivoGerador, string prefixoPreCarga, List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas, List<string> cargasParaCancelamento, string motivoCancelamentoCarga, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, bool naoTentarGerarCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string AdminStringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinhaPedido = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha();

            try
            {
                Dominio.Entidades.Empresa empresa = null;
                Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem = null;
                Dominio.Entidades.Embarcador.Pedidos.Porto portoDestino = null;
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio = null;
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = null;
                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem = null;
                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino = null;
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = null;

                Dominio.Entidades.Cliente pessoa = null;
                Dominio.Enumeradores.TipoTomador tipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                int qtdDisponibilizada = 0;
                int qtdNaoEmbarcadas = 0;
                decimal valor = 0;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPortoOrigem = (from obj in linha.Colunas where obj.NomeCampo == "PortoOrigem" select obj).FirstOrDefault();
                if (colPortoOrigem != null)
                {
                    portoOrigem = repPorto.BuscarPorDescricao(colPortoOrigem.Valor);
                    if (portoOrigem != null && portoOrigem.Empresa != null)
                        empresa = portoOrigem.Empresa;
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoOperacao = (from obj in linha.Colunas where obj.NomeCampo == "TipoProposta" select obj).FirstOrDefault();
                if (colTipoOperacao != null)
                {
                    string strTipoOperacao = colTipoOperacao.Valor;
                    tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao(strTipoOperacao);
                }
                if (tipoOperacao == null)
                    return RetornarFalhaLinha("Não foi localizado o tipo da operação.");

                if (tipoOperacao?.TipoPropostaMultimodal == TipoPropostaMultimodal.FaturamentoContabilidade)
                {
                    if (empresa == null)
                        empresa = tipoOperacao?.EmpresaEmissora;
                    if (empresa == null)
                        return RetornarFalhaLinha("Não foi localizado a empresa no tipo de operação.");
                }
                else
                {
                    if (portoOrigem == null || empresa == null)
                        return RetornarFalhaLinha("Não foi localizado o porto de origem e sua empresa.");
                }

                bool temGrupoPessoa = false;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colGrupoPessoas = (from obj in linha.Colunas where obj.NomeCampo == "GrupoPessoas" select obj).FirstOrDefault();
                if (colGrupoPessoas != null)
                    grupoPessoas = repGrupoPessoas.BuscarPorDescricao(colGrupoPessoas.Valor);
                if (tipoOperacao?.TipoPropostaMultimodal == TipoPropostaMultimodal.FaturamentoContabilidade && (grupoPessoas == null || grupoPessoas.ClienteTomadorFatura == null))
                    return RetornarFalhaLinha("Grupo de pessoa não encontrado ou tomador da fatura não selecionado no grupo de pessoa! Obrigatório para o tipo de proposta Faturamento - Contabilidade");
                else if (grupoPessoas != null)
                    temGrupoPessoa = grupoPessoas.ClienteTomadorFatura != null && tipoOperacao?.TipoPropostaMultimodal == TipoPropostaMultimodal.FaturamentoContabilidade;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPortoDestino = (from obj in linha.Colunas where obj.NomeCampo == "PortoDestino" select obj).FirstOrDefault();
                if (colPortoDestino != null)
                    portoDestino = repPorto.BuscarPorDescricao(colPortoDestino.Valor);
                if (portoDestino == null && tipoOperacao?.TipoPropostaMultimodal != TipoPropostaMultimodal.FaturamentoContabilidade)
                    return RetornarFalhaLinha("Não foi localizado o porto de destino.");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colViagem = (from obj in linha.Colunas where obj.NomeCampo == "Viagem" select obj).FirstOrDefault();
                if (colViagem != null)
                    pedidoViagemNavio = repPedidoViagemNavio.BuscarPorDescricao(colViagem.Valor);
                if (pedidoViagemNavio == null)
                    return RetornarFalhaLinha("Não foi localizado a viagem.");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTerminalOrigem = (from obj in linha.Colunas where obj.NomeCampo == "TerminalOrigem" select obj).FirstOrDefault();
                if (colTerminalOrigem != null)
                    terminalOrigem = repTerminal.BuscarPorCodigoIntegracao(colTerminalOrigem.Valor);
                if (colTerminalOrigem != null && terminalOrigem == null)
                    terminalOrigem = repTerminal.BuscarPorDescricao(colTerminalOrigem.Valor);

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTerminalDestino = (from obj in linha.Colunas where obj.NomeCampo == "TerminalDestino" select obj).FirstOrDefault();
                if (colTerminalDestino != null)
                    terminalDestino = repTerminal.BuscarPorCodigoIntegracao(colTerminalDestino.Valor);
                if (colTerminalDestino != null && terminalDestino == null)
                    terminalDestino = repTerminal.BuscarPorDescricao(colTerminalDestino.Valor);

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPessoa = (from obj in linha.Colunas where obj.NomeCampo == "CNPJCliente" select obj).FirstOrDefault();
                if (colPessoa != null)
                {
                    string somenteNumeros = Utilidades.String.OnlyNumbers((string)colPessoa.Valor);
                    if (!string.IsNullOrEmpty(somenteNumeros))
                    {
                        double cpfCNPJRemetente = double.Parse(somenteNumeros);
                        pessoa = repCliente.BuscarPorCPFCNPJ(cpfCNPJRemetente);
                    }
                }
                if (pessoa == null && !temGrupoPessoa)
                    return RetornarFalhaLinha("Não foi localizado o cliente.");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQtdDisponibilizada = (from obj in linha.Colunas where obj.NomeCampo == "QtdDisponibilizada" select obj).FirstOrDefault();
                if (colQtdDisponibilizada != null && !string.IsNullOrWhiteSpace((string)colQtdDisponibilizada.Valor))
                {
                    string strQtdDisponibilizada = (string)colQtdDisponibilizada.Valor;
                    int.TryParse(strQtdDisponibilizada, out qtdDisponibilizada);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValor = (from obj in linha.Colunas where obj.NomeCampo == "Valor" select obj).FirstOrDefault();
                if (colValor != null && !string.IsNullOrWhiteSpace((string)colValor.Valor))
                {
                    string strValor = (string)colValor.Valor;
                    decimal.TryParse(strValor, out valor);
                }
                if (valor <= 0)
                    return RetornarFalhaLinha("Não foi informado o valor.");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQtdNaoEmbarcadas = (from obj in linha.Colunas where obj.NomeCampo == "QtdNaoEmbarcadas" select obj).FirstOrDefault();
                if (colQtdNaoEmbarcadas != null && !string.IsNullOrWhiteSpace((string)colQtdNaoEmbarcadas.Valor))
                {
                    string strQtdNaoEmbarcadas = (string)colQtdNaoEmbarcadas.Valor;
                    int.TryParse(strQtdNaoEmbarcadas, out qtdNaoEmbarcadas);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNotasParciais = (from obj in linha.Colunas where obj.NomeCampo == "NumeroReferencia" select obj).FirstOrDefault();
                List<int> notasParciais = new List<int>();
                if (colNotasParciais != null)
                {
                    if (!string.IsNullOrWhiteSpace((string)colNotasParciais.Valor))
                    {
                        string strNotasParciais = (string)colNotasParciais.Valor;
                        string[] splitNotaParcial = strNotasParciais.Split(',');
                        foreach (string strNfParcial in splitNotaParcial)
                        {
                            int numeroNF = 0;
                            int.TryParse(strNfParcial, out numeroNF);
                            if (numeroNF > 0) notasParciais.Add(numeroNF);
                        }
                    }
                }

                tipoCarga = repTipoDeCarga.BuscarPrimeira();
                if (tipoCarga == null && tipoOperacao?.TipoPropostaMultimodal != TipoPropostaMultimodal.FaturamentoContabilidade)
                    return RetornarFalhaLinha("Não foi localizado o tipo de carga.");

                int unidadesContabilidade = 0;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colUnidades = (from obj in linha.Colunas where obj.NomeCampo == "Unidades" select obj).FirstOrDefault();
                if (colUnidades != null && !string.IsNullOrWhiteSpace((string)colUnidades.Valor))
                {
                    string strunidadesContabilidade = (string)colUnidades.Valor;
                    int.TryParse(strunidadesContabilidade, out unidadesContabilidade);
                }
                if (unidadesContabilidade <= 0 && tipoOperacao?.TipoPropostaMultimodal == TipoPropostaMultimodal.FaturamentoContabilidade)
                    return RetornarFalhaLinha("Deve-se informar as Unidades para o tipo de proposta Faturamento - Contabilidade.");

                decimal aliquotaICMSContabilidade = 0;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colaliquotaICMSContabilidade = (from obj in linha.Colunas where obj.NomeCampo == "AliquotaICMS" select obj).FirstOrDefault();
                if (colaliquotaICMSContabilidade != null && !string.IsNullOrWhiteSpace((string)colaliquotaICMSContabilidade.Valor))
                {
                    string straliquotaICMSContabilidade = (string)colaliquotaICMSContabilidade.Valor;
                    decimal.TryParse(straliquotaICMSContabilidade, out aliquotaICMSContabilidade);
                }
                if (aliquotaICMSContabilidade <= 0 && tipoOperacao?.TipoPropostaMultimodal == TipoPropostaMultimodal.FaturamentoContabilidade)
                    return RetornarFalhaLinha("Deve-se informar a Alíquota de ICMS para o tipo de proposta Faturamento - Contabilidade.");

                decimal valorICMSContabilidade = 0;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colvalorICMSContabilidade = (from obj in linha.Colunas where obj.NomeCampo == "ValorICMS" select obj).FirstOrDefault();
                if (colvalorICMSContabilidade != null && !string.IsNullOrWhiteSpace((string)colvalorICMSContabilidade.Valor))
                {
                    string strvalorICMSContabilidade = (string)colvalorICMSContabilidade.Valor;
                    decimal.TryParse(strvalorICMSContabilidade, out valorICMSContabilidade);
                }
                if (valorICMSContabilidade <= 0 && tipoOperacao?.TipoPropostaMultimodal == TipoPropostaMultimodal.FaturamentoContabilidade)
                    return RetornarFalhaLinha("Deve-se informar o Valor de ICMS para o tipo de proposta Faturamento - Contabilidade.");

                decimal ptaxContabilidade = 0;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colptaxContabilidade = (from obj in linha.Colunas where obj.NomeCampo == "PTAX" select obj).FirstOrDefault();
                if (colptaxContabilidade != null && !string.IsNullOrWhiteSpace((string)colptaxContabilidade.Valor))
                {
                    string strptaxContabilidade = (string)colptaxContabilidade.Valor;
                    decimal.TryParse(strptaxContabilidade, out ptaxContabilidade);
                }
                if (ptaxContabilidade <= 0 && tipoOperacao?.TipoPropostaMultimodal == TipoPropostaMultimodal.FaturamentoContabilidade)
                    return RetornarFalhaLinha("Deve-se informar o PTAX para o tipo de proposta Faturamento - Contabilidade.");

                decimal valorUSDContabilidade = 0;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colvalorUSDContabilidade = (from obj in linha.Colunas where obj.NomeCampo == "ValorUSD" select obj).FirstOrDefault();
                if (colvalorUSDContabilidade != null && !string.IsNullOrWhiteSpace((string)colvalorUSDContabilidade.Valor))
                {
                    string strvalorUSDContabilidade = (string)colvalorUSDContabilidade.Valor;
                    decimal.TryParse(strvalorUSDContabilidade, out valorUSDContabilidade);
                }
                if (valorUSDContabilidade <= 0 && tipoOperacao?.TipoPropostaMultimodal == TipoPropostaMultimodal.FaturamentoContabilidade)
                    return RetornarFalhaLinha("Deve-se informar o Valor (USD) para o tipo de proposta Faturamento - Contabilidade.");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colpoNumberContabilidade = (from obj in linha.Colunas where obj.NomeCampo == "PONumber" select obj).FirstOrDefault();
                string poNumberContabilidade = "";
                if (colpoNumberContabilidade != null)
                    poNumberContabilidade = (string)colpoNumberContabilidade.Valor;

                if (string.IsNullOrWhiteSpace(poNumberContabilidade) && tipoOperacao?.TipoPropostaMultimodal == TipoPropostaMultimodal.FaturamentoContabilidade)
                    return RetornarFalhaLinha("Deve-se informar o PO Number para o tipo de proposta Faturamento - Contabilidade.");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroPedido = (from obj in linha.Colunas where obj.NomeCampo == "NumeroReferencia" select obj).FirstOrDefault();
                string numeroPedido = "";
                if (colNumeroPedido != null)
                    numeroPedido = (string)colNumeroPedido.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colObservacao = (from obj in linha.Colunas where obj.NomeCampo == "Observacao" select obj).FirstOrDefault();
                string observacaoParaFaturamento = "";
                if (colObservacao != null)
                    observacaoParaFaturamento = (string)colObservacao.Valor;
                else
                    return RetornarFalhaLinha("Observação é obrigatório para prosseguir!");

                if (string.IsNullOrWhiteSpace(observacaoParaFaturamento) && tipoOperacao?.TipoPropostaMultimodal == TipoPropostaMultimodal.FaturamentoContabilidade)
                    return RetornarFalhaLinha("Deve-se informar a observação para o tipo de proposta Faturamento - Contabilidade.");

                Dominio.Entidades.Cliente tomador = null;
                if (temGrupoPessoa)
                {
                    tomador = grupoPessoas.ClienteTomadorFatura;
                    pessoa = grupoPessoas.ClienteTomadorFatura;
                }
                else
                    tomador = pessoa;

                Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor freteValor = ObterFreteValor(linha, tipoOperacao, valorUSDContabilidade, unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> nfes = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
                int numeroNotaCliente = 0;
                Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal nfe = ObterNFe(linha, pessoa, pessoa, ref numeroNotaCliente);
                if (nfe != null)
                {
                    nfes.Add(nfe);
                    numeroNotaCliente = 0;//Só salva o número a parte se não gerar a nota
                }
                // Só é gerado o pedido quando for informado um numero e for diferente de vazio e quando não for para ativar ou cancelar o pedido Ou quando a coluna não for informada

                if (tomador != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoImportacaoSalvar pedidoImportacaoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoImportacaoSalvar()
                    {
                        Empresa = empresa,
                        PreCarga = null,
                        Nfes = nfes,
                        NotasParciais = notasParciais,
                        CtesParciais = null,
                        NumeroPedido = numeroPedido,
                        QuantidadePalletsFacionada = 0m,
                        PesoPedido = 0m,
                        CubagemPedido = 0m,
                        TipoCarga = tipoCarga,
                        ModeloVeicularCarga = null,
                        TipoOperacao = tipoOperacao,
                        Remetente = tomador,
                        Destinatario = tomador,
                        Expedidor = null,
                        Recebedor = null,
                        Filial = null,
                        RotaFrete = null,
                        FreteValor = freteValor,
                        FreteValorFilialEmissora = null,
                        Usuario = usuario,
                        DataHoraCarregamento = DateTime.Now,
                        TipoTomador = tipoTomador,
                        TipoEmbarque = "",
                        Observacao = "",
                        OrdemColeta = 1,
                        NomeArquivoGerador = arquivoGerador.Nome,
                        GuidArquivoGerador = arquivoGerador.Guid,
                        PedidoProduto = null,
                        CanalEntrega = null,
                        Veiculo = null,
                        Reboques = null,
                        Motoristas = null,
                        ObservacaoAdicional = "",
                        DataPrevEntrega = DateTime.Now,
                        NumeroCargaEncaixar = "",
                        NumeroControle = "",
                        Deposito = null,
                        QtdVolumes = 1,
                        CentroResultado = null,
                        GrupoPessoasRemetente = tomador.GrupoPessoas,
                        ProdutoPrincipal = null,
                        NumeroNotaCliente = numeroNotaCliente,
                        QtdDisponibilizada = qtdDisponibilizada,
                        QtdNaoEmbarcadas = qtdNaoEmbarcadas,
                        PortoDestino = portoDestino,
                        PortoOrigem = portoOrigem,
                        PedidoViagemNavio = pedidoViagemNavio,
                        TerminalDestino = terminalDestino,
                        TerminalOrigem = terminalOrigem,
                        ObservacaoParaFaturamento = observacaoParaFaturamento,
                        ObservacaoInterna = "",
                        UnidadesContabilidade = unidadesContabilidade,
                        AliquotaICMSContabilidade = aliquotaICMSContabilidade,
                        ValorICMSContabilidade = valorICMSContabilidade,
                        PTAXContabilidade = ptaxContabilidade,
                        ValorUSDContabilidade = valorUSDContabilidade,
                        PONumberContabilidade = poNumberContabilidade
                    };

                    retornoLinhaPedido = AdicionarPedidoImportacao(pedidoImportacaoAdicionar, naoTentarGerarCarga, unitOfWork, configuracaoTMS, tipoServicoMultisoftware, auditado);
                    if (!string.IsNullOrWhiteSpace(retornoLinhaPedido.mensagemFalha))
                        return RetornarFalhaLinha(retornoLinhaPedido.mensagemFalha);
                }
            }
            catch (Exception ex2)
            {
                Servicos.Log.TratarErro(ex2);
                return RetornarFalhaLinha("Ocorreu uma falha ao processar a linha (" + ex2.Message + ").");
            }

            return RetornarSucessoLinha(retornoLinhaPedido?.codigo ?? 0);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

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

        private Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor ObterFreteValor(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, decimal valorUSD, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorFrete = (from obj in linha.Colunas where obj.NomeCampo == "Valor" select obj).FirstOrDefault();

            decimal ValorFrete = 0;
            if (colValorFrete != null || (valorUSD > 0 && tipoOperacao?.TipoPropostaMultimodal == TipoPropostaMultimodal.FaturamentoContabilidade))
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor freteValor = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();
                if (colValorFrete != null || (valorUSD > 0 && tipoOperacao?.TipoPropostaMultimodal == TipoPropostaMultimodal.FaturamentoContabilidade))
                {
                    if (tipoOperacao?.TipoPropostaMultimodal == TipoPropostaMultimodal.FaturamentoContabilidade && valorUSD > 0)
                        ValorFrete = valorUSD;
                    else
                        ValorFrete = Utilidades.Decimal.Converter((string)colValorFrete.Valor);

                    freteValor.ValorTotalAReceber = ValorFrete;
                    freteValor.FreteProprio = ValorFrete;
                }
                return freteValor;

            }
            else
            {
                return null;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal ObterNFe(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, Dominio.Entidades.Cliente emitente, Dominio.Entidades.Cliente destinatario, ref int numeroNF)
        {
            Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal nfe = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal();

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroNFe = (from obj in linha.Colunas where obj.NomeCampo == "NumeroReferencia" select obj).FirstOrDefault();
            if (colNumeroNFe != null)
            {
                int.TryParse((string)colNumeroNFe.Valor, out numeroNF);
                nfe.Numero = numeroNF;
            }
            else
                return null;

            nfe.Serie = "1";
            nfe.Chave = "";
            nfe.Modelo = "99";
            nfe.DataEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            nfe.PesoBruto = (decimal)0.01;
            nfe.PesoLiquido = (decimal)0.01;
            nfe.Destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa() { CPFCNPJ = destinatario.CPF_CNPJ.ToString(), ClienteExterior = destinatario?.Tipo == "E" };
            nfe.Emitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa() { CPFCNPJ = emitente.CPF_CNPJ.ToString(), ClienteExterior = emitente?.Tipo == "E" };
            nfe.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;
            if (!string.IsNullOrWhiteSpace(nfe.Chave))
                nfe.Modelo = "55";
            else
                nfe.Modelo = "99";
            nfe.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;
            nfe.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;

            return nfe;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha AdicionarPedidoImportacao(Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoImportacaoSalvar pedidoImportacaoAdicionar, bool naoTentarGerarCarga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            string retorno = "";
            Servicos.Embarcador.Pedido.Pedido svcPedido = new Servicos.Embarcador.Pedido.Pedido();

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);


            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;
            bool addProduto = true;
            pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido();
            pedido.PedidoTakeOrPay = true;
            pedido.Numero = repPedido.BuscarProximoNumero();
            pedido.CodigoPedidoCliente = "";
            pedido.DataFinalColeta = DateTime.Now;
            pedido.DataInicialColeta = DateTime.Now;
            pedido.DataPrevisaoChegadaDestinatario = pedidoImportacaoAdicionar.DataHoraDescarga;
            pedido.Adicional1 = pedidoImportacaoAdicionar.Adicional1;
            pedido.Adicional2 = pedidoImportacaoAdicionar.Adicional2;
            pedido.Adicional3 = pedidoImportacaoAdicionar.Adicional3;
            pedido.Adicional4 = pedidoImportacaoAdicionar.Adicional4;
            pedido.Adicional5 = pedidoImportacaoAdicionar.Adicional5;
            pedido.Adicional6 = pedidoImportacaoAdicionar.Adicional6;
            pedido.Adicional7 = pedidoImportacaoAdicionar.Adicional7;
            pedido.NumeroCargaEncaixar = pedidoImportacaoAdicionar.NumeroCargaEncaixar;
            pedido.NumeroControle = pedidoImportacaoAdicionar.NumeroControle;
            pedido.TipoEmbarque = pedidoImportacaoAdicionar.TipoEmbarque;
            pedido.Observacao = pedidoImportacaoAdicionar.Observacao;
            pedido.ObservacaoInterna = pedidoImportacaoAdicionar.ObservacaoInterna;
            pedido.OrdemColetaProgramada = pedidoImportacaoAdicionar.OrdemColeta;
            pedido.QuebraMultiplosCarregamentos = pedidoImportacaoAdicionar.QuebraMultiplosCarregamentos;
            pedido.Deposito = pedidoImportacaoAdicionar.Deposito;
            pedido.QtVolumes = pedidoImportacaoAdicionar.QtdVolumes;
            pedido.SaldoVolumesRestante = pedidoImportacaoAdicionar.QtdVolumes;
            pedido.NumeroNotaCliente = pedidoImportacaoAdicionar.NumeroNotaCliente;
            pedido.ObservacaoAdicional = pedidoImportacaoAdicionar.ObservacaoAdicional ?? string.Empty;
            pedido.Porto = pedidoImportacaoAdicionar.PortoOrigem;
            pedido.PortoDestino = pedidoImportacaoAdicionar.PortoDestino;
            pedido.PedidoViagemNavio = pedidoImportacaoAdicionar.PedidoViagemNavio;
            pedido.QtdNaoEmbarcadas = pedidoImportacaoAdicionar.QtdNaoEmbarcadas;
            pedido.QtdDisponibilizada = pedidoImportacaoAdicionar.QtdDisponibilizada;
            pedido.TerminalOrigem = pedidoImportacaoAdicionar.TerminalOrigem;
            pedido.TerminalDestino = pedidoImportacaoAdicionar.TerminalDestino;
            pedido.TipoTerminalImportacao = pedidoImportacaoAdicionar.TerminalOrigem;
            pedido.ObservacaoParaFaturamento = pedidoImportacaoAdicionar.ObservacaoParaFaturamento;

            pedido.UnidadesContabilidade = pedidoImportacaoAdicionar.UnidadesContabilidade;
            pedido.AliquotaICMSContabilidade = pedidoImportacaoAdicionar.AliquotaICMSContabilidade;
            pedido.ValorICMSContabilidade = pedidoImportacaoAdicionar.ValorICMSContabilidade;
            pedido.PTAXContabilidade = pedidoImportacaoAdicionar.PTAXContabilidade;
            pedido.ValorUSDContabilidade = pedidoImportacaoAdicionar.ValorUSDContabilidade;
            pedido.PONumberContabilidade = pedidoImportacaoAdicionar.PONumberContabilidade;

            if (pedidoImportacaoAdicionar.Remetente == null && pedidoImportacaoAdicionar.GrupoPessoasRemetente != null)
            {
                pedido.TipoPessoa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa;
                pedido.GrupoPessoas = pedidoImportacaoAdicionar.GrupoPessoasRemetente;
            }

            if (pedidoImportacaoAdicionar.DataHoraCarregamento != DateTime.MinValue)
                pedido.DataCarregamentoPedido = pedidoImportacaoAdicionar.DataHoraCarregamento;
            else
                pedido.DataCarregamentoPedido = DateTime.Now;

            if (pedidoImportacaoAdicionar.DataPrevEntrega != DateTime.MinValue)
                pedido.PrevisaoEntrega = pedidoImportacaoAdicionar.DataPrevEntrega;

            if (pedidoImportacaoAdicionar.DataValidade != DateTime.MinValue)
                pedido.DataValidade = pedidoImportacaoAdicionar.DataValidade;

            if (pedidoImportacaoAdicionar.DataInicioJanelaDescarga != DateTime.MinValue)
                pedido.DataInicioJanelaDescarga = pedidoImportacaoAdicionar.DataInicioJanelaDescarga;

            pedido.ProdutoPrincipal = pedidoImportacaoAdicionar.ProdutoPrincipal;
            pedido.Remetente = pedidoImportacaoAdicionar.Remetente;
            pedido.Expedidor = pedidoImportacaoAdicionar.Expedidor;
            pedido.Recebedor = pedidoImportacaoAdicionar.Recebedor;
            pedido.Destinatario = pedidoImportacaoAdicionar.Destinatario;

            if (pedido.Remetente != null)
            {
                pedido.GrupoPessoas = pedidoImportacaoAdicionar.Remetente.GrupoPessoas;
                pedido.Origem = pedidoImportacaoAdicionar.Remetente.Localidade;
            }

            pedido.Veiculos = new List<Dominio.Entidades.Veiculo>();
            pedido.Empresa = pedidoImportacaoAdicionar.Empresa;

            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();

            if (pedido.Remetente != null)
                svcPedido.PreecherEnderecoPedido(ref pedidoEnderecoOrigem, pedido.Remetente);

            if (pedido.Destinatario != null)
                svcPedido.PreecherEnderecoPedido(ref pedidoEnderecoDestino, pedido.Destinatario);

            if (pedidoEnderecoOrigem.Localidade != null)
                repPedidoEndereco.Inserir(pedidoEnderecoOrigem);
            if (pedidoEnderecoDestino.Localidade != null)
                repPedidoEndereco.Inserir(pedidoEnderecoDestino);

            if (pedidoEnderecoOrigem.Localidade != null)
            {
                pedido.Origem = pedidoEnderecoOrigem.Localidade;
                pedido.EnderecoOrigem = pedidoEnderecoOrigem;
            }

            if (pedidoEnderecoDestino.Localidade != null)
            {
                pedido.Destino = pedidoEnderecoDestino.Localidade;
                pedido.EnderecoDestino = pedidoEnderecoDestino;
            }

            pedido.QtdEntregas = 1;
            pedido.PedidoTransbordo = false;
            pedido.UsarOutroEnderecoOrigem = false;
            pedido.UsarOutroEnderecoDestino = false;

            if (pedido.Destinatario != null)
            {
                Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = repClienteDescarga.BuscarPorPessoa(pedido.Destinatario.CPF_CNPJ);
                if (clienteDescarga != null)
                {
                    pedido.RestricoesDescarga = clienteDescarga.RestricoesDescarga.ToList();
                }
            }

            pedido.RotaFrete = pedidoImportacaoAdicionar.RotaFrete;
            if (pedido.RotaFrete == null && pedido.Destino != null)
            {
                pedido.RotaFrete = repRotaFrete.BuscarPorLocalidade(pedido.Destino, true);

                if (pedido.RotaFrete == null)
                    pedido.RotaFrete = repRotaFrete.BuscarPorEstado(pedido.Destino.Estado.Sigla, true);
            }

            Dominio.Entidades.Cliente tomador = pedido.ObterTomador();

            if (!string.IsNullOrWhiteSpace(pedidoImportacaoAdicionar.TipoCarga?.ProdutoPredominante))
                pedido.ProdutoPredominante = pedidoImportacaoAdicionar.TipoCarga.ProdutoPredominante;
            else if (!string.IsNullOrWhiteSpace(pedidoImportacaoAdicionar.TipoOperacao?.ProdutoPredominanteOperacao))
                pedido.ProdutoPredominante = pedidoImportacaoAdicionar.TipoOperacao.ProdutoPredominanteOperacao;
            else if (tomador?.GrupoPessoas != null && !string.IsNullOrWhiteSpace(tomador.GrupoPessoas.ProdutoPredominante))
                pedido.ProdutoPredominante = tomador.GrupoPessoas.ProdutoPredominante;
            else if (!string.IsNullOrWhiteSpace(configuracaoTMS.DescricaoProdutoPredominatePadrao))
                pedido.ProdutoPredominante = configuracaoTMS.DescricaoProdutoPredominatePadrao;
            else
                pedido.ProdutoPredominante = "Importação";

            if (pedidoImportacaoAdicionar.Filial != null)
                pedido.Filial = pedidoImportacaoAdicionar.Filial;

            pedido.AdicionadaManualmente = true;
            pedido.NumeroPaletesFracionado = pedidoImportacaoAdicionar.QuantidadePalletsFacionada;
            if (string.IsNullOrEmpty(pedidoImportacaoAdicionar.NumeroPedido))
            {
                if (configuracaoTMS.NumeroCargaSequencialUnico)
                    pedido.NumeroSequenciaPedido = repPedido.ObterProximoCodigo();
                else
                    pedido.NumeroSequenciaPedido = repPedido.ObterProximoCodigo(pedido.Filial);

                pedido.NumeroPedidoEmbarcador = pedido.NumeroSequenciaPedido.ToString();
            }
            else
                pedido.NumeroPedidoEmbarcador = pedidoImportacaoAdicionar.NumeroPedido;

            if (pedido.PesoTotal <= 0)
            {
                pedido.PesoTotal = pedidoImportacaoAdicionar.PesoPedido;
                pedido.PesoSaldoRestante = pedidoImportacaoAdicionar.PesoPedido;
            }

            pedido.CubagemTotal = pedidoImportacaoAdicionar.CubagemPedido;
            pedido.PreCarga = pedidoImportacaoAdicionar.PreCarga;
            pedido.ObservacaoCTe = configuracaoTMS.ObservacaoCTePadraoEmbarcador ?? "";
            pedido.Temperatura = "";

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !naoTentarGerarCarga)
            {
                if (pedido.TipoOperacao != null && !pedido.TipoOperacao.GeraCargaAutomaticamente)
                {
                    pedido.GerarAutomaticamenteCargaDoPedido = false;
                    pedido.PedidoTotalmenteCarregado = false;
                }
                else
                    pedido.GerarAutomaticamenteCargaDoPedido = true;

                pedido.PedidoIntegradoEmbarcador = !configuracaoTMS.UtilizarIntegracaoPedido;

                svcPedido.PreencherCodigoCargaEmbarcador(pedido, configuracaoTMS, unitOfWork);
            }
            else
            {
                pedido.PedidoIntegradoEmbarcador = false;
                pedido.GerarAutomaticamenteCargaDoPedido = false;
            }

            pedido.Requisitante = Dominio.ObjetosDeValor.Embarcador.Enumeradores.RequisitanteColeta.Remetente;
            pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;
            pedido.TipoDeCarga = pedidoImportacaoAdicionar.TipoCarga;
            pedido.ModeloVeicularCarga = pedidoImportacaoAdicionar.ModeloVeicularCarga;
            pedido.CanalEntrega = pedidoImportacaoAdicionar.CanalEntrega;
            pedido.TipoOperacao = pedidoImportacaoAdicionar.TipoOperacao;
            pedido.TipoTomador = pedidoImportacaoAdicionar.TipoTomador;
            if (pedidoImportacaoAdicionar.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
            else
                pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
            pedido.UltimaAtualizacao = DateTime.Now;
            pedido.Usuario = pedidoImportacaoAdicionar.Usuario;
            pedido.Autor = pedidoImportacaoAdicionar.Usuario;
            pedido.DataInicialViagemExecutada = pedido.DataPrevisaoSaida;
            pedido.DataFinalViagemExecutada = pedido.PrevisaoEntrega;
            pedido.DataInicialViagemFaturada = pedido.DataPrevisaoSaida;
            pedido.DataFinalViagemFaturada = pedido.PrevisaoEntrega;
            pedido.NomeArquivoGerador = pedidoImportacaoAdicionar.NomeArquivoGerador;
            pedido.GuidArquivoGerador = pedidoImportacaoAdicionar.GuidArquivoGerador;
            pedido.PedidoIntegradoEmbarcador = true;
            pedido.SituacaoAcompanhamentoPedido = SituacaoAcompanhamentoPedido.AgColeta;
            if (pedido.TipoOperacao != null && pedido.TipoOperacao.TipoPropostaMultimodal == TipoPropostaMultimodal.DemurrageCabotagem)
                pedido.PedidoDemurrage = true;
            if (pedido.TipoOperacao != null && pedido.TipoOperacao.TipoPropostaMultimodal == TipoPropostaMultimodal.DetentionCabotagem)
                pedido.PedidoDetention = true;

            repPedido.Inserir(pedido);
            pedido.Protocolo = pedido.Codigo;

            Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, null, "Criou Pedido via importação", unitOfWork);

            if (string.IsNullOrWhiteSpace(retorno))
            {
                if (addProduto && pedidoImportacaoAdicionar.PedidoProduto != null)
                {
                    if (addProduto)
                    {
                        pedidoImportacaoAdicionar.PedidoProduto.Pedido = pedido;
                        repPedidoProduto.Inserir(pedidoImportacaoAdicionar.PedidoProduto);
                        pedido.PesoTotal += pedidoImportacaoAdicionar.PedidoProduto.PesoUnitario * (pedidoImportacaoAdicionar.PedidoProduto.Quantidade == 0 ? 1 : pedidoImportacaoAdicionar.PedidoProduto.Quantidade);
                        pedido.PesoSaldoRestante += pedidoImportacaoAdicionar.PedidoProduto.PesoUnitario * (pedidoImportacaoAdicionar.PedidoProduto.Quantidade == 0 ? 1 : pedidoImportacaoAdicionar.PedidoProduto.Quantidade);
                    }
                }
                if (pedidoImportacaoAdicionar.FreteValor != null)
                {
                    pedido.ValorFreteFilialEmissora += pedidoImportacaoAdicionar.FreteValorFilialEmissora?.FreteProprio ?? 0;
                    pedido.ValorFreteAReceber += pedidoImportacaoAdicionar.FreteValor.ValorTotalAReceber;
                    pedido.ValorFreteNegociado += pedidoImportacaoAdicionar.FreteValor.FreteProprio;
                    if (pedidoImportacaoAdicionar.FreteValor.ICMS != null)
                    {
                        pedido.PercentualInclusaoBC = pedidoImportacaoAdicionar.FreteValor.ICMS.PercentualInclusaoBC;
                        pedido.PercentualAliquota = pedidoImportacaoAdicionar.FreteValor.ICMS.Aliquota;
                        pedido.BaseCalculoICMS += pedidoImportacaoAdicionar.FreteValor.ICMS.ValorBaseCalculoICMS;
                        pedido.ValorICMS += pedidoImportacaoAdicionar.FreteValor.ICMS.ValorICMS;
                        pedido.IncluirBaseCalculoICMS = pedidoImportacaoAdicionar.FreteValor.ICMS.IncluirICMSBC;
                        pedido.ImpostoNegociado = true;
                    }
                }

                repPedido.Atualizar(pedido);

                if (pedido.TipoOperacao?.ProdutoEmbarcadorPadraoColeta != null && pedidoImportacaoAdicionar.PedidoProduto == null)
                {
                    pedidoImportacaoAdicionar.PedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto();
                    pedidoImportacaoAdicionar.PedidoProduto.Pedido = pedido;
                    pedidoImportacaoAdicionar.PedidoProduto.Produto = pedido.TipoOperacao.ProdutoEmbarcadorPadraoColeta;
                    repPedidoProduto.Inserir(pedidoImportacaoAdicionar.PedidoProduto);
                }

                if (pedidoImportacaoAdicionar.NotasParciais?.Count > 0)
                    svcPedido.SalvarNotasParciais(pedido, pedidoImportacaoAdicionar.NotasParciais, unitOfWork);

                if (pedidoImportacaoAdicionar.Nfes != null && pedidoImportacaoAdicionar.Nfes.Count > 0)
                {
                    pedido.ValorTotalNotasFiscais = 0;
                    for (int i = 0; i < pedidoImportacaoAdicionar.Nfes.Count; i++)
                        svcPedido.AdicionarNotaFiscal(pedido, pedidoImportacaoAdicionar.Nfes[i], unitOfWork);
                }
            }

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (pedidoImportacaoAdicionar.Veiculo != null)
                    pedido.Veiculos.Add(pedidoImportacaoAdicionar.Veiculo);

                if (pedidoImportacaoAdicionar.Reboques != null)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in pedidoImportacaoAdicionar.Reboques)
                        pedido.Veiculos.Add(reboque);
                }
                else if (pedidoImportacaoAdicionar.Veiculo?.VeiculosVinculados.Count > 0)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in pedidoImportacaoAdicionar.Veiculo.VeiculosVinculados)
                        pedido.Veiculos.Add(reboque);
                }

                if (pedidoImportacaoAdicionar.Motoristas != null)
                {
                    if (pedido.Motoristas == null)
                        pedido.Motoristas = new List<Dominio.Entidades.Usuario>();

                    foreach (Dominio.Entidades.Usuario motorista in pedidoImportacaoAdicionar.Motoristas)
                        pedido.Motoristas.Add(motorista);
                }

                //if (string.IsNullOrWhiteSpace(retorno) && svcPedido.IsGerarCargaAutomaticamente(pedido))
                //    svcPedido.CriarCarga(out retorno, unitOfWork, new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>() { pedido }, configuracaoTMS, tipoServicoMultisoftware);
            }

            return new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha
            {
                codigo = pedido.Codigo,
                mensagemFalha = retorno
            };
        }

        #endregion Métodos Privados
    }
}