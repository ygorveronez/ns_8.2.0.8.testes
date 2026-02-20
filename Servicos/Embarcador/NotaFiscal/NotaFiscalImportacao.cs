using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Servicos.Embarcador.NotaFiscal
{
    public class NotaFiscalImportacao
    {
        #region Atributos

        private Repositorio.UnitOfWork _unitOfWork;
        private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Repositorio.Embarcador.Pedidos.XMLNotaFiscal _repositorio;
        private Repositorio.Embarcador.Pedidos.Pedido _repositorioPedido;
        private Repositorio.Embarcador.Pedidos.PedidoNotaParcial _repositorioPedidoNotaParcial;
        private Repositorio.Embarcador.Cargas.CargaPedido _repositorioCargaPedido;
        private Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial _repositorioCargaPedidoXMLNotaFiscalParcial;
        private Repositorio.Cliente _repositorioCliente;
        private Dominio.Entidades.Usuario _usuario;
        private Servicos.Embarcador.Logistica.AgendamentoEntregaPedido _servicoAgendamentoEntregaPedido;
        private Pedido.NotaFiscal _servicoNotaFiscal;

        #endregion

        #region Construtores

        public NotaFiscalImportacao(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Usuario usuario)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _repositorio = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            _repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            _servicoNotaFiscal = new Pedido.NotaFiscal(_unitOfWork);
            _repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            _repositorioPedidoNotaParcial = new Repositorio.Embarcador.Pedidos.PedidoNotaParcial(_unitOfWork);
            _repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            _repositorioCargaPedidoXMLNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(_unitOfWork);
            _servicoAgendamentoEntregaPedido = new Logistica.AgendamentoEntregaPedido(_unitOfWork, configuracao, tipoServicoMultisoftware, auditado, usuario);
            _configuracaoEmbarcador = configuracao;
            _usuario = usuario;
        }

        #endregion

        #region Métodos Públicos

        public void GerarImportacaoNotaFiscalPorFTP(string nomeArquivo, System.Data.DataTable dataTable, List<int> idsColunas, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacao();

            Repositorio.Embarcador.NotaFiscal.ImportacaoNotaFiscal repositorioImportacaoNotaFiscal = new Repositorio.Embarcador.NotaFiscal.ImportacaoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha repositorioImportacaoNotaFiscalLinha = new Repositorio.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinhaColuna repositorioImportacaoNotaFiscalLinhaColuna = new Repositorio.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinhaColuna(unitOfWork);

            if (dataTable.Rows.Count == 0)
                return;

            Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscal importacaoNotaFiscal = new Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscal
            {
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoNotaFiscal.Pendente,
                Planilha = nomeArquivo,
                QuantidadeLinhas = dataTable.Rows.Count,
                DataImportacao = DateTime.Now
            };

            repositorioImportacaoNotaFiscal.Inserir(importacaoNotaFiscal);

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha linha = new Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha()
                {
                    ImportacaoNotaFiscal = importacaoNotaFiscal,
                    Numero = i + 1,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoNotaFiscal.Pendente
                };

                repositorioImportacaoNotaFiscalLinha.Inserir(linha);

                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    if (j >= idsColunas.Count)
                        break;

                    Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinhaColuna coluna = new Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinhaColuna()
                    {
                        Linha = linha,
                        NomeCampo = configuracoes.Where(obj => obj.Id == idsColunas[j]).Select(obj => obj.Propriedade).FirstOrDefault(),
                        Valor = (dataTable.Rows[i][j]).ToString()
                    };

                    if (string.IsNullOrWhiteSpace(coluna.NomeCampo))
                        continue;

                    repositorioImportacaoNotaFiscalLinhaColuna.Inserir(coluna);
                }
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoImportacaoNotaFiscal ImportarNotaFiscal(Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscal importacao)
        {
            Repositorio.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha repositorioImportacaoLinha = new Repositorio.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha(_unitOfWork);
            Repositorio.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinhaColuna repositorioImportacaoLinhaColuna = new Repositorio.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinhaColuna(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalImportacao repositorioNotaFiscalImportacao = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalImportacao(_unitOfWork);

            Dominio.Entidades.Embarcador.NotaFiscal.ConfiguracaoFTPImportacaoNotaFiscal configuracao = new Repositorio.Embarcador.NotaFiscal.ConfiguracaoFTPImportacaoNotaFiscal(_unitOfWork).BuscarPrimeiroRegistro();

            int quantidade = repositorioImportacaoLinha.ContarLinhasPendentes(importacao.Codigo);
            List<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha> linhas = repositorioImportacaoLinha.BuscarLinhasPendentesImportacao(importacao.Codigo, 1000);
            List<int> codigos = (from obj in linhas select obj.Codigo).ToList();

            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoImportacaoNotaFiscal retorno = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoImportacaoNotaFiscal()
            {
                Sucesso = true
            };

            int contador = 0;

            try
            {


                List<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinhaColuna> colunasLinhasAgrupadas = repositorioImportacaoLinhaColuna.BuscarPorImportacaoLinhas(codigos);
                List<int> codigosDeletar = new List<int>();
                foreach (Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinha linha in linhas)
                {
                    //_unitOfWork.Start();
                    //linha.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoNotaFiscal.Processando;
                    //repositorioImportacaoLinha.Atualizar(linha);
                    //_unitOfWork.CommitChanges();

                    List<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinhaColuna> colunasLinha = (from obj in colunasLinhasAgrupadas where obj.Linha.Codigo == linha.Codigo select obj).ToList(); //repositorioImportacaoLinhaColuna.BuscarPorImportacaoLinha(linha.Codigo);
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha dadosLinha = ConverterParaImportacao(colunasLinha);

                    _unitOfWork.FlushAndClear();
                    _unitOfWork.Start();
                    try
                    {
                        (Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal NotaFiscal, Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido) retornoLinha = ImportarNotaFiscal(dadosLinha, ObterPropriedades(), !configuracao.ApenasAtualizarDadosPedido);

                        if (retornoLinha.retorno.contar)
                        {
                            contador++;
                            if (retornoLinha.retorno.codigo > 0)
                            {
                                linha.XMLNotaFiscal = retornoLinha.NotaFiscal;
                                linha.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoNotaFiscal.Sucesso;
                                linha.Mensagem = "Nota fiscal importada.";
                                repositorioImportacaoLinha.Atualizar(linha);
                                //retorno.TotalNotas++;
                            }
                            else
                            {
                                linha.Pedido = retornoLinha.Pedido;
                                linha.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoNotaFiscal.Sucesso;
                                linha.Mensagem = "Pedido atualizado.";
                                repositorioImportacaoLinha.Atualizar(linha);
                                //retorno.TotalPedidos++;
                            }

                            //repositorioImportacaoLinhaColuna.DeletarPorCodigos(colunasLinha.Select(obj => obj.Codigo).ToList());
                            repositorioImportacaoLinhaColuna.DeletarPorLinha(linha.Codigo);
                        }
                        else
                        {
                            _unitOfWork.Rollback();
                            _unitOfWork.Start();
                            linha.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoNotaFiscal.Erro;
                            linha.Mensagem = retornoLinha.retorno.mensagemFalha;
                            repositorioImportacaoLinha.Atualizar(linha);
                        }

                        _unitOfWork.CommitChanges();
                    }
                    catch (Exception ex)
                    {
                        _unitOfWork.Rollback();
                        _unitOfWork.Start();
                        linha.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoNotaFiscal.Erro;
                        linha.Mensagem = ex.Message;
                        repositorioImportacaoLinha.Atualizar(linha);
                        _unitOfWork.CommitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                retorno.Sucesso = false;
                retorno.Mensagem = ex.Message;
            }

            if (quantidade <= codigos.Count())
            {
                retorno.TotalNotas = repositorioImportacaoLinha.ContarNotasProcessadas(importacao.Codigo);
                retorno.TotalPedidos = repositorioImportacaoLinha.ContarPedidosProcessados(importacao.Codigo);
                retorno.TerminouProcessar = true;
                int total = repositorioImportacaoLinha.ContarLinhas(importacao.Codigo);
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalImportacao xmlNotaFiscalImportacao = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalImportacao()
                {
                    Data = DateTime.Now,
                    QuantidadeRegistrosTotal = total,
                    QuantidadeRegistrosImportados = (retorno.TotalNotas + retorno.TotalPedidos),
                    NomeArquivo = importacao.Planilha,
                    ImportacaoNotaFiscal = importacao
                };

                repositorioNotaFiscalImportacao.Inserir(xmlNotaFiscalImportacao);
            }
            return retorno;
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao ImportarNotasFiscais(Dominio.ObjetosDeValor.Embarcador.Importacao.ParametrosImportacao parametrosImportacao)
        {
            Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscal importacao = new Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscal()
            {
                DataImportacao = DateTime.Now,
                DataInicioProcessamento = DateTime.Now,
                Mensagem = "Planilha processada manualmente",
                Planilha = parametrosImportacao.Nome,
                QuantidadeLinhas = parametrosImportacao.Linhas.Count,
                Usuario = _usuario
            };

            Repositorio.Embarcador.NotaFiscal.ImportacaoNotaFiscal repositorioImportacaoNotaFiscal = new Repositorio.Embarcador.NotaFiscal.ImportacaoNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalImportacao repositorioXmlNotaFiscalImportacao = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalImportacao(_unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.ConfiguracaoFTPImportacaoNotaFiscal configuracao = new Repositorio.Embarcador.NotaFiscal.ConfiguracaoFTPImportacaoNotaFiscal(_unitOfWork).BuscarPrimeiroRegistro();

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao()
            {
                Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
            };

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            retornoImportacao.Total = parametrosImportacao.Linhas.Count;

            List<string> propriedades = ObterPropriedades();

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha dadosLinha;

            for (int indice = 0; indice < parametrosImportacao.Linhas.Count; indice++)
            {
                _unitOfWork.FlushAndClear();

                dadosLinha = parametrosImportacao.Linhas[indice];

                try
                {
                    _unitOfWork.Start();

                    (Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornoLinha, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal, Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido) retorno = ImportarNotaFiscal(dadosLinha, propriedades, !configuracao.ApenasAtualizarDadosPedido);

                    retorno.RetornoLinha.indice = indice;

                    if (retorno.RetornoLinha.contar)
                        retornoImportacao.Importados++;

                    retornoImportacao.Retornolinhas.Add(retorno.RetornoLinha);

                    if (retorno.RetornoLinha.processou)
                    {
                        if (retorno.XMLNotaFiscal?.Codigo > 0)
                        {
                            string a = retorno.XMLNotaFiscal.XML;
                            notasFiscais.Add(retorno.XMLNotaFiscal);
                        }

                        _unitOfWork.CommitChanges();
                        continue;
                    }

                    _unitOfWork.Rollback();
                }
                catch (ServicoException excecao)
                {
                    _unitOfWork.Rollback();
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(excecao.Message, indice));
                    continue;
                }
                catch (Exception excecao)
                {
                    _unitOfWork.Rollback();
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", indice));
                    continue;
                }
            }
            _unitOfWork.FlushAndClear();

            _unitOfWork.Start();

            importacao.DataFimProcessamento = DateTime.Now;

            repositorioImportacaoNotaFiscal.Inserir(importacao);

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalImportacao xmlNotaFiscalImportacao = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalImportacao()
            {
                Data = DateTime.Now,
                QuantidadeRegistrosTotal = retornoImportacao.Total,
                QuantidadeRegistrosImportados = retornoImportacao.Importados,
                NomeArquivo = parametrosImportacao.Nome,
                ImportacaoNotaFiscal = importacao
            };

            repositorioXmlNotaFiscalImportacao.Inserir(xmlNotaFiscalImportacao);

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);

            notasFiscais = notasFiscais.DistinctBy(nf => nf.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscaisInicilizadasNovamente = repositorioXmlNotaFiscal.BuscarPorCodigos(notasFiscais.Select(o => o.Codigo).ToList());

            notasFiscaisInicilizadasNovamente.ForEach(nf => nf.XMLNotaFiscalImportacao = xmlNotaFiscalImportacao);
            notasFiscaisInicilizadasNovamente.ForEach(nf => repositorioXmlNotaFiscal.Atualizar(nf));

            _unitOfWork.CommitChanges();

            return retornoImportacao;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacao()
        {
            return new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>()
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao(){Id = 1, Descricao = "Número NF-e", Propriedade = "NumeroNFe", Tamanho = 200},
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao(){Id = 2, Descricao = "Data Carregamento", Propriedade = "DataCarregamento", Tamanho = 200},
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao(){Id = 3, Descricao = "Valor NF-e", Propriedade = "ValorNFe", Tamanho = 200},
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao(){Id = 4, Descricao = "Valor NF-e sem impostos", Propriedade = "ValorNFeSemImpostos", Tamanho = 200},
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao(){Id = 5, Descricao = "Volume do pedido", Propriedade = "VolumePedido", Tamanho = 200},
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao(){Id = 6, Descricao = "Número de Caixas do Pedido", Propriedade = "NumeroCaixasPedido", Tamanho = 200},
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao(){Id = 7, Descricao = "Data de Entrada no Sistema", Propriedade = "DataEntradaSistema", Tamanho = 200},
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao(){Id = 8, Descricao = "Cód. Integração Emitente", Propriedade = "CodigoIntegracaoEmitente", Tamanho = 200},
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao(){Id = 9, Descricao = "CPF/CNPJ Destinatário", Propriedade = "CPFCNPJDestinatario", Tamanho = 200, Obrigatorio = true},
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao(){Id = 10, Descricao = "Data Emissão NF-e", Propriedade = "DataEmissaoNFe", Tamanho = 200, Obrigatorio = true},
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao(){Id = 11, Descricao = "Número Pedido", Propriedade = "NumeroPedidoEmbarcador", Tamanho = 200},
            };
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha ConverterParaImportacao(List<Dominio.Entidades.Embarcador.NotaFiscal.ImportacaoNotaFiscalLinhaColuna> colunas)
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

        private (Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal NotaFiscal, Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido) ImportarNotaFiscal(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, List<string> propriedades, bool gerarNota)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha();

            //Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();
            //xmlNotaFiscal.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.Outros;
            //xmlNotaFiscal.XML = string.Empty;
            //xmlNotaFiscal.CNPJTranposrtador = string.Empty;
            //xmlNotaFiscal.PlacaVeiculoNotaFiscal = string.Empty;
            //xmlNotaFiscal.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada;

            string valor;
            string numeroPedidoEmbarcador = string.Empty;
            int numeroNota = 0;
            decimal ValorTotalProdutos = 0;
            decimal MetrosCubicos = 0;
            DateTime? DataCarregamento = null;
            DateTime? DataHoraCriacaoEmbrcador = null;
            decimal Valor = 0;
            int Volumes = 0;
            foreach (string propriedade in propriedades)
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna coluna = (from obj in linha.Colunas where obj.NomeCampo == propriedade select obj).FirstOrDefault();

                if (coluna == null)
                    continue;

                valor = ((string)coluna.Valor);



                switch (propriedade)
                {
                    case "NumeroNFe":
                        if (Utilidades.String.OnlyNumbers(valor).Length == 0)
                            throw new ServicoException("Registro ignorado na importação");

                        numeroNota = valor.ToInt();

                        break;

                    case "DataCarregamento":
                        DataCarregamento = valor.ToNullableDateTime();
                        break;

                    case "ValorNFe":
                        if (Utilidades.String.OnlyNumbers(valor).Length == 0)
                            throw new ServicoException("Registro ignorado na importação");

                        Valor = valor.ToDecimal();
                        break;

                    case "ValorNFeSemImpostos":
                        if (Utilidades.String.OnlyNumbers(valor).Length == 0)
                            throw new ServicoException("Registro ignorado na importação");

                        ValorTotalProdutos = Utilidades.Decimal.Converter(valor.Replace("-", ""));
                        break;

                    case "VolumePedido":
                        if (Utilidades.String.OnlyNumbers(valor).Length == 0)
                            throw new ServicoException("Registro ignorado na importação");

                        MetrosCubicos = Utilidades.Decimal.Converter(valor.Replace("-", ""));
                        break;

                    case "NumeroCaixasPedido":
                        if (Utilidades.String.OnlyNumbers(valor).Length == 0)
                            throw new ServicoException("Registro ignorado na importação");

                        Volumes = valor.ToInt();
                        break;

                    case "DataEntradaSistema":
                        DataHoraCriacaoEmbrcador = DateTime.ParseExact(valor, "yyyyMMdd", CultureInfo.InvariantCulture);
                        break;

                    case "NumeroPedidoEmbarcador":
                        numeroPedidoEmbarcador = valor?.Trim() ?? string.Empty;
                        break;
                }
            }

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;
            if (!string.IsNullOrWhiteSpace(numeroPedidoEmbarcador))
                pedido = _repositorioPedido.BuscarPorNumeroEmbarcador(numeroPedidoEmbarcador);

            if (pedido != null && !_repositorioPedidoNotaParcial.PossuiNotaPorPedido(pedido.Codigo, numeroNota))
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial pedidoNotaParcial = new Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial()
                {
                    Pedido = pedido,
                    Numero = numeroNota, //xmlNotaFiscal.Numero,
                    NumeroPedido = numeroPedidoEmbarcador,
                    DataCriacao = DateTime.Now
                };
                _repositorioPedidoNotaParcial.Inserir(pedidoNotaParcial);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = _repositorioCargaPedido.BuscarPorPedidoComCargaAtiva(pedido.Codigo);
                Servicos.Log.TratarErro($"4 Adicionando Pedidos Parciais {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")} [{string.Join(", ", cargasPedido.Select(x => x.Pedido.Codigo))}]");
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedido)
                {
                    if (_repositorioCargaPedidoXMLNotaFiscalParcial.PossuiPorCargaPedidoENotaParcial(cargaPedido.Codigo, numeroNota))
                        continue;

                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoXMLNotaFiscalParcial = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial()
                    {
                        CargaPedido = cargaPedido,
                        Numero = numeroNota
                    };
                    _repositorioCargaPedidoXMLNotaFiscalParcial.Inserir(cargaPedidoXMLNotaFiscalParcial);
                }
            }

            if (pedido == null)
                pedido = _repositorioPedido.BuscarPedidoPorNumeroNotaFiscalParcial(numeroNota);

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalExistente = null;

            if (gerarNota)
            {
                xmlNotaFiscalExistente = _repositorio.BuscarPorNumero(numeroNota);
                if (xmlNotaFiscalExistente != null)
                    _servicoNotaFiscal.VincularXMLNotaFiscal(xmlNotaFiscalExistente, _configuracaoEmbarcador, _tipoServicoMultisoftware, _auditado, false, false);

                //Auditoria.Auditoria.Auditar(_auditado, xmlNotaFiscal, "Nota Fiscal enviada via importação", _unitOfWork);
            }

            if (pedido != null)
            {
                if (DataCarregamento.HasValue)
                {
                    if (!pedido.DataTerminoCarregamento.HasValue && !(pedido.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.UtilizarDataSaidaGuaritaComoTerminoCarregamento ?? false))
                        pedido.DataTerminoCarregamento = DataCarregamento.Value;

                    if (pedido.RotaFrete != null)
                        _servicoAgendamentoEntregaPedido.AlterarDataPrevisaoEntrega(pedido);
                }

                if (ValorTotalProdutos > 0)
                    pedido.GrossSales = ValorTotalProdutos;

                if (MetrosCubicos > 0)
                    pedido.CubagemTotal = MetrosCubicos;

                if (Volumes > 0)
                    pedido.QtVolumes = Volumes;

                _repositorioPedido.Atualizar(pedido);
            }
            else if (!gerarNota)
                throw new ServicoException("O pedido não foi encontrado na base multisoftware.");

            return (RetornarSucessoLinha(xmlNotaFiscalExistente?.Codigo ?? 0), xmlNotaFiscalExistente, pedido);
        }

        private List<string> ObterPropriedades()
        {
            return ObterConfiguracaoImportacao()
                .Select(t => t.Propriedade)
                .ToList();
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarSucessoLinha(int codigo)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { codigo = codigo, mensagemFalha = "", processou = true, contar = true };
        }

        #endregion
    }
}
