using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos
{
    public class ProdutoEmbarcador
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga _configuracaoGeralCarga;

        #endregion Atributos

        #region Construtores

        public ProdutoEmbarcador(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoGeralCarga: null) { }

        public ProdutoEmbarcador(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            _configuracaoGeralCarga = configuracaoGeralCarga;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga ObterConfiguracaoGeralCarga()
        {
            if (_configuracaoGeralCarga == null)
                _configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoGeralCarga;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Produtos.MarcaProduto> IntegrarMarcasProduto(List<Dominio.ObjetosDeValor.Embarcador.Pedido.MarcaProduto> marcas, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            List<Dominio.Entidades.Embarcador.Produtos.MarcaProduto> marcasProduto = new List<Dominio.Entidades.Embarcador.Produtos.MarcaProduto>();

            if (marcas.Count > 0)
            {
                Repositorio.Embarcador.Produtos.MarcaProduto repMarcaProduto = new Repositorio.Embarcador.Produtos.MarcaProduto(_unitOfWork);

                List<Dominio.Entidades.Embarcador.Produtos.MarcaProduto> marcasExiste = repMarcaProduto.BuscarPorCodigosIntegracao((from obj in marcas select obj.CodigoIntegracao).Distinct().ToList());

                foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.MarcaProduto marca in marcas)
                {
                    string codigoIntegracao = marca.CodigoIntegracao;
                    string descricao = marca.Descricao;

                    Dominio.Entidades.Embarcador.Produtos.MarcaProduto marcaProduto = (from obj in marcasExiste where obj.CodigoIntegracao == codigoIntegracao select obj).FirstOrDefault();

                    if (marcaProduto != null)
                    {
                        if (marcaProduto.Descricao != descricao)
                        {
                            marcaProduto.Descricao = descricao;
                            repMarcaProduto.Atualizar(marcaProduto, auditado);
                        }
                    }
                    else
                    {
                        marcaProduto = new Dominio.Entidades.Embarcador.Produtos.MarcaProduto();
                        marcaProduto.CodigoIntegracao = codigoIntegracao;
                        marcaProduto.Status = true;
                        marcaProduto.Descricao = descricao;
                        repMarcaProduto.Inserir(marcaProduto, auditado);
                    }

                    if (!marcasProduto.Contains(marcaProduto))
                        marcasProduto.Add(marcaProduto);
                }

            }


            return marcasProduto;

        }

        public List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> IntegrarGruposProduto(List<Dominio.ObjetosDeValor.Embarcador.NFe.GrupoProduto> gruposProdutos)
        {
            Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> grupoProdutos = new List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>();

            foreach (Dominio.ObjetosDeValor.Embarcador.NFe.GrupoProduto gp in gruposProdutos)
            {
                string codigoGrupoProdutoEmbarcador = gp.Codigo;
                string descricaoGrupoProdutoEmbarcador = gp.Descricao;

                Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = repGrupoProduto.BuscarPorCodigoEmbarcador(codigoGrupoProdutoEmbarcador);

                if (grupoProduto != null)
                {
                    if (grupoProduto.Descricao != descricaoGrupoProdutoEmbarcador)
                    {
                        grupoProduto.Descricao = descricaoGrupoProdutoEmbarcador;
                        repGrupoProduto.Atualizar(grupoProduto);
                    }
                }
                else
                {
                    grupoProduto = new Dominio.Entidades.Embarcador.Produtos.GrupoProduto();
                    grupoProduto.CodigoGrupoProdutoEmbarcador = codigoGrupoProdutoEmbarcador;
                    grupoProduto.Ativo = true;
                    grupoProduto.Descricao = descricaoGrupoProdutoEmbarcador;
                    repGrupoProduto.Inserir(grupoProduto);
                }

                if (!grupoProdutos.Contains(grupoProduto))
                    grupoProdutos.Add(grupoProduto);
            }

            return grupoProdutos;

        }

        public Dominio.Entidades.Embarcador.Produtos.GrupoProduto IntegrarGrupoProduto(string codigoGrupoProdutoEmbarcador, string descricaoGrupoProdutoEmbarcador)
        {
            Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(_unitOfWork);

            Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = repGrupoProduto.BuscarPorCodigoEmbarcador(codigoGrupoProdutoEmbarcador);
            if (grupoProduto != null)
            {
                if (grupoProduto.Descricao != descricaoGrupoProdutoEmbarcador)
                {
                    grupoProduto.Descricao = descricaoGrupoProdutoEmbarcador;
                    repGrupoProduto.Atualizar(grupoProduto);
                }
            }
            else
            {
                grupoProduto = new Dominio.Entidades.Embarcador.Produtos.GrupoProduto();
                grupoProduto.CodigoGrupoProdutoEmbarcador = codigoGrupoProdutoEmbarcador;
                grupoProduto.Ativo = true;
                grupoProduto.Descricao = descricaoGrupoProdutoEmbarcador;
                repGrupoProduto.Inserir(grupoProduto);
            }
            return grupoProduto;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao> IntegrarLinhasSeparacao(List<Dominio.ObjetosDeValor.Embarcador.Pedido.LinhaSeparacao> linhaSeparacaos, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Pedidos.LinhaSeparacao repLinhaSeparacao = new Repositorio.Embarcador.Pedidos.LinhaSeparacao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao> linhasSeparacao = new List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao>();

            Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);


            List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> grupoProdutos = new List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>();

            List<string> codigosLinhas = (from obj in linhaSeparacaos select obj.CodigoIntegracao).Distinct().ToList();
            List<string> codigosFiliais = (from obj in linhaSeparacaos where obj.Filial != null select obj.Filial.CodigoIntegracao).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao> linhasSeparacaoExistentes = repLinhaSeparacao.BuscarPorTodasCodigosIntegracoes(codigosLinhas);
            List<Dominio.Entidades.Embarcador.Filiais.Filial> eFiliais = repFilial.buscarPorCodigosEmbarcador(codigosFiliais);

            foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.LinhaSeparacao ls in linhaSeparacaos)
            {
                Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao linhaSeparacao = (from obj in linhasSeparacaoExistentes where obj.CodigoIntegracao == ls.CodigoIntegracao select obj).FirstOrDefault();
                Dominio.Entidades.Embarcador.Filiais.Filial eFilial = null;
                if (!string.IsNullOrWhiteSpace(ls.Filial?.CodigoIntegracao))
                    eFilial = (from obj in eFiliais where obj.CodigoFilialEmbarcador == ls.Filial.CodigoIntegracao select obj).FirstOrDefault(); //repFilial.buscarPorCodigoEmbarcador(ls.Filial.CodigoIntegracao);

                if (linhaSeparacao != null)
                {
                    if (linhaSeparacao.Descricao != ls.Descricao || linhaSeparacao.Filial?.Codigo != eFilial?.Codigo || linhaSeparacao.Roteiriza != ls.Roteiriza || !linhaSeparacao.Ativo)
                    {
                        linhaSeparacao.Descricao = ls.Descricao;
                        linhaSeparacao.Filial = eFilial;
                        linhaSeparacao.Roteiriza = ls.Roteiriza;
                        linhaSeparacao.Ativo = true;
                        linhaSeparacao.NivelPrioridade = (ls.NivelPrioridade == 0 ? 99 : ls.NivelPrioridade);
                        repLinhaSeparacao.Atualizar(linhaSeparacao, Auditado);
                    }
                }
                else
                {
                    linhaSeparacao = new Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao();
                    linhaSeparacao.CodigoIntegracao = ls.CodigoIntegracao;
                    linhaSeparacao.Ativo = true;
                    linhaSeparacao.Descricao = ls.Descricao;
                    linhaSeparacao.Roteiriza = ls.Roteiriza;
                    linhaSeparacao.NivelPrioridade = (ls.NivelPrioridade == 0 ? 99 : ls.NivelPrioridade);
                    linhaSeparacao.Filial = eFilial;
                    repLinhaSeparacao.Inserir(linhaSeparacao, Auditado);
                }

                if (!linhasSeparacao.Contains(linhaSeparacao))
                    linhasSeparacao.Add(linhaSeparacao);
            }
            return linhasSeparacao;
        }

        public List<Dominio.Entidades.Embarcador.Produtos.EnderecoProduto> IntegrarEnderecosProdutos(List<Dominio.ObjetosDeValor.Embarcador.Produtos.EnderecoProduto> enderecosProdutos, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Produtos.EnderecoProduto repositorioEnderecoProduto = new Repositorio.Embarcador.Produtos.EnderecoProduto(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Produtos.EnderecoProduto> enderecosProdutosCadastrados = new List<Dominio.Entidades.Embarcador.Produtos.EnderecoProduto>();

            foreach (Dominio.ObjetosDeValor.Embarcador.Produtos.EnderecoProduto endereco in enderecosProdutos)
            {

                Dominio.Entidades.Embarcador.Filiais.Filial eFilial = null;
                if (!string.IsNullOrWhiteSpace(endereco.Filial?.CodigoIntegracao))
                    eFilial = repositorioFilial.buscarPorCodigoEmbarcador(endereco.Filial.CodigoIntegracao);

                Dominio.Entidades.Embarcador.Produtos.EnderecoProduto enderecoProduto = repositorioEnderecoProduto.BuscarPorCodigoIntegracao(endereco.CodigoIntegracao, eFilial?.Codigo ?? 0);


                if (enderecoProduto != null)
                {
                    if (enderecoProduto.Descricao != endereco.Descricao || enderecoProduto.Filial?.Codigo != eFilial?.Codigo || enderecoProduto.NivelPrioridade != endereco.NivelPrioridade)
                    {
                        enderecoProduto.Descricao = endereco.Descricao;
                        enderecoProduto.Filial = eFilial;
                        enderecoProduto.NivelPrioridade = (endereco.NivelPrioridade == 0 ? 99 : endereco.NivelPrioridade);
                        enderecoProduto.Ativo = true;
                        repositorioEnderecoProduto.Atualizar(enderecoProduto, Auditado);
                    }
                }
                else
                {
                    enderecoProduto = new Dominio.Entidades.Embarcador.Produtos.EnderecoProduto();
                    enderecoProduto.CodigoIntegracao = endereco.CodigoIntegracao;
                    enderecoProduto.Ativo = true;
                    enderecoProduto.Descricao = endereco.Descricao;
                    enderecoProduto.NivelPrioridade = (endereco.NivelPrioridade == 0 ? 99 : endereco.NivelPrioridade);
                    enderecoProduto.Filial = eFilial;
                    repositorioEnderecoProduto.Inserir(enderecoProduto, Auditado);
                }

                if (!enderecosProdutosCadastrados.Contains(enderecoProduto))
                    enderecosProdutosCadastrados.Add(enderecoProduto);
            }
            return enderecosProdutosCadastrados;
        }

        public Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao IntegrarLinhaSeparacao(string codigoLinhaSeparacao, string descricaoLinhaSeparacao, bool roteiriza, string filialCodigoIntegracao, int nivelPrioridade, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Pedidos.LinhaSeparacao repLinhaSeparacao = new Repositorio.Embarcador.Pedidos.LinhaSeparacao(_unitOfWork);

            Dominio.Entidades.Embarcador.Filiais.Filial eFilial = null;
            Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao linhaSeparacao = repLinhaSeparacao.BuscarPorTodasCodigoIntegracao(codigoLinhaSeparacao);

            if (!string.IsNullOrWhiteSpace(filialCodigoIntegracao))
            {
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
                eFilial = repFilial.buscarPorCodigoEmbarcador(filialCodigoIntegracao);
            }

            if (linhaSeparacao != null)
            {
                if (linhaSeparacao.Descricao != descricaoLinhaSeparacao || linhaSeparacao.Filial?.Codigo != eFilial?.Codigo || linhaSeparacao.Roteiriza != roteiriza || (linhaSeparacao.NivelPrioridade != nivelPrioridade && nivelPrioridade > 0))
                {
                    linhaSeparacao.Descricao = descricaoLinhaSeparacao;
                    linhaSeparacao.Roteiriza = roteiriza;
                    linhaSeparacao.Filial = eFilial;
                    linhaSeparacao.NivelPrioridade = (nivelPrioridade == 0 ? 99 : nivelPrioridade);
                    repLinhaSeparacao.Atualizar(linhaSeparacao, auditado);
                }
            }
            else
            {
                linhaSeparacao = new Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao();
                linhaSeparacao.CodigoIntegracao = codigoLinhaSeparacao;
                linhaSeparacao.Ativo = true;
                linhaSeparacao.Descricao = descricaoLinhaSeparacao;
                linhaSeparacao.Roteiriza = roteiriza;
                linhaSeparacao.Filial = eFilial;
                linhaSeparacao.NivelPrioridade = (nivelPrioridade == 0 ? 99 : nivelPrioridade);
                repLinhaSeparacao.Inserir(linhaSeparacao, auditado);
            }
            return linhaSeparacao;
        }

        public Dominio.Entidades.Embarcador.Produtos.MarcaProduto IntegrarMarca(string codigoMarca, string descricaoMarca, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Produtos.MarcaProduto repMarcaProduto = new Repositorio.Embarcador.Produtos.MarcaProduto(_unitOfWork);
            Dominio.Entidades.Embarcador.Produtos.MarcaProduto marcaProduto = repMarcaProduto.BuscarPorCodigoIntegracao(codigoMarca);

            if (marcaProduto != null)
            {
                if (marcaProduto.Descricao != descricaoMarca)
                {
                    marcaProduto.Descricao = descricaoMarca;
                    repMarcaProduto.Atualizar(marcaProduto, auditado);
                }
            }
            else
            {
                marcaProduto = new Dominio.Entidades.Embarcador.Produtos.MarcaProduto();
                marcaProduto.CodigoIntegracao = codigoMarca;
                marcaProduto.Status = true;
                marcaProduto.Descricao = descricaoMarca;
                repMarcaProduto.Inserir(marcaProduto, auditado);
            }
            return marcaProduto;
        }

        public Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem IntegrarTipoEmbalagem(string codigoTipoEmbalagem, string descricaoTipoEmbalagem)
        {
            Repositorio.Embarcador.Produtos.TipoEmbalagem repTipoEmbalagem = new Repositorio.Embarcador.Produtos.TipoEmbalagem(_unitOfWork);


            Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem tipoEmbalagem = repTipoEmbalagem.BuscarPorCodigoIntegracao(codigoTipoEmbalagem);


            if (tipoEmbalagem != null)
            {
                if (tipoEmbalagem.Descricao != descricaoTipoEmbalagem)
                {
                    tipoEmbalagem.Descricao = descricaoTipoEmbalagem;
                }
            }
            else
            {
                tipoEmbalagem = new Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem();
                tipoEmbalagem.CodigoIntegracao = codigoTipoEmbalagem;
                tipoEmbalagem.Ativo = true;
                tipoEmbalagem.Descricao = descricaoTipoEmbalagem;
            }
            return tipoEmbalagem;
        }

        public List<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem> IntegrarTiposEmbalagem(List<Dominio.ObjetosDeValor.Embarcador.Pedido.TipoEmbalagem> tipoEmbalagens)
        {
            Repositorio.Embarcador.Produtos.TipoEmbalagem repTipoEmbalagem = new Repositorio.Embarcador.Produtos.TipoEmbalagem(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem> tiposEmbalagem = new List<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem>();


            foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.TipoEmbalagem ls in tipoEmbalagens)
            {
                Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem tipoEmbalagem = repTipoEmbalagem.BuscarPorCodigoIntegracao(ls.CodigoIntegracao);

                if (tipoEmbalagem != null)
                {
                    if (tipoEmbalagem.Descricao != ls.Descricao)
                    {
                        tipoEmbalagem.Descricao = ls.Descricao;
                    }
                }
                else
                {
                    tipoEmbalagem = new Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem();
                    tipoEmbalagem.CodigoIntegracao = ls.CodigoIntegracao;
                    tipoEmbalagem.Ativo = true;
                    tipoEmbalagem.Descricao = ls.Descricao;
                    repTipoEmbalagem.Inserir(tipoEmbalagem);
                }

                if (!tiposEmbalagem.Contains(tipoEmbalagem))
                    tiposEmbalagem.Add(tipoEmbalagem);
            }
            return tiposEmbalagem;
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador IntegrarProduto(List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtosEmbarcador, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, string codigoProdutoEmbarcador, string descricaoProdutoEmbarcador, decimal pesoUnitario, Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto, decimal metroCubito, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, string codigoDocumentacao, bool atualizar, string codigoNCM, int quantidadePorCaixa, int quantidadeCaixaPorPallet, decimal altura, decimal largura, decimal comprimento, Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao linhaSeparacao, Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem tipoEmbalagem, Dominio.Entidades.Embarcador.Produtos.MarcaProduto marcaProduto, string unidadeMedida, string observacaoProduto, string codigoProdutoCEAN = "", string codigoEAN = "")
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);
            Repositorio.UnidadeDeMedida repositorioUnidadeMedida = new Repositorio.UnidadeDeMedida(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);

            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = null;

            if (!configuracao.SempreInserirNovoProdutoPorIntegracao)
                produto = (from obj in produtosEmbarcador where obj.CodigoProdutoEmbarcador == codigoProdutoEmbarcador select obj).FirstOrDefault();

            int fator = configuracao.FatorMetroCubicoProdutoEmbarcadorIntegracao;
            //TODO: Criar configuração padrão no sistema para parametrizar o fator na importação de produtos.
            // ASSAI.
            if (configuracao.FatorMetroCubicoProdutoEmbarcadorIntegracao == 0)
                fator = 1; //1000000;

            decimal metroCubicoCalculado = 0m;

            if ((altura > 0) && (largura > 0) && (comprimento > 0))
                metroCubicoCalculado = Math.Round(((altura * largura * comprimento) / fator), 6, MidpointRounding.ToEven);

            if (produto != null)
            {
                if (!atualizar)
                    return produto;

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = ObterConfiguracaoGeralCarga();
                bool utilizarPesoProdutoParaCalcularPesoCarga = configuracaoGeralCarga?.UtilizarPesoProdutoParaCalcularPesoCarga ?? false;

                bool dadosProdutoAtualizados = (
                    (produto.MetroCubito != metroCubito && metroCubito != 0) ||
                    (produto.QuantidadeCaixa != quantidadePorCaixa && quantidadePorCaixa != 0) ||
                    (produto.QuantidadeCaixaPorPallet != quantidadeCaixaPorPallet && quantidadeCaixaPorPallet != 0) ||
                    (produto.CodigoNCM != codigoNCM) || (produto.CodigoCEAN != codigoProdutoCEAN) || (produto.CodigoEAN != codigoEAN) ||
                    (produto.Descricao != descricaoProdutoEmbarcador && !string.IsNullOrWhiteSpace(descricaoProdutoEmbarcador)) ||
                    (produto.AlturaCM != altura && altura != 0) ||
                    (produto.ComprimentoCM != comprimento && comprimento != 0) ||
                    (produto.LarguraCM != largura && largura != 0) ||
                    (metroCubito == 0 && metroCubicoCalculado != produto.MetroCubito) ||
                    (linhaSeparacao != null && produto.LinhaSeparacao?.Codigo != linhaSeparacao?.Codigo) ||
                    (tipoEmbalagem != null && produto.TipoEmbalagem?.Codigo != tipoEmbalagem?.Codigo) ||
                    (marcaProduto != null && produto.MarcaProduto?.Codigo != marcaProduto?.Codigo) ||
                    (grupoProduto != null && produto.GrupoProduto?.Codigo != grupoProduto?.Codigo) ||
                    (produto.PesoUnitario != pesoUnitario && pesoUnitario != 0 && !utilizarPesoProdutoParaCalcularPesoCarga) ||
                    (!string.IsNullOrWhiteSpace(unidadeMedida) && produto.SiglaUnidade != unidadeMedida) ||
                    (!string.IsNullOrWhiteSpace(observacaoProduto) && produto.Observacao != observacaoProduto)
                );

                if (!dadosProdutoAtualizados)
                    return produto;

                produto.Initialize();
                produto.Integrado = false;

                if (!string.IsNullOrWhiteSpace(descricaoProdutoEmbarcador))
                    produto.Descricao = descricaoProdutoEmbarcador;

                if ((pesoUnitario != 0) && !utilizarPesoProdutoParaCalcularPesoCarga)
                    produto.PesoUnitario = pesoUnitario;

                if (!string.IsNullOrWhiteSpace(observacaoProduto))
                    produto.Observacao = observacaoProduto;

                if (altura != 0)
                    produto.AlturaCM = altura;

                if (largura != 0)
                    produto.LarguraCM = largura;

                if (comprimento != 0)
                    produto.ComprimentoCM = comprimento;

                if (metroCubito != 0)
                    produto.MetroCubito = metroCubito;
                else if (produto.MetroCubito != metroCubicoCalculado)
                    produto.MetroCubito = metroCubicoCalculado;

                if (quantidadePorCaixa != 0)
                    produto.QuantidadeCaixa = quantidadePorCaixa;

                if (quantidadeCaixaPorPallet != 0)
                    produto.QuantidadeCaixaPorPallet = quantidadeCaixaPorPallet;

                if (!string.IsNullOrWhiteSpace(unidadeMedida) && unidadeMedida != produto.SiglaUnidade)
                {
                    produto.SiglaUnidade = unidadeMedida.Left(10);
                    Dominio.Entidades.UnidadeDeMedida unidadeDeMedida = repositorioUnidadeMedida.BuscarPorSigla(unidadeMedida);
                    produto.Unidade = unidadeDeMedida;
                }

                produto.CodigoNCM = codigoNCM;
                produto.CodigoDocumentacao = codigoDocumentacao;

                if (!string.IsNullOrWhiteSpace(codigoProdutoCEAN))
                    produto.CodigoCEAN = codigoProdutoCEAN;

                if (grupoProduto != null)
                    produto.GrupoProduto = grupoProduto;

                if (marcaProduto != null)
                    produto.MarcaProduto = marcaProduto;

                if (linhaSeparacao != null)
                    produto.LinhaSeparacao = linhaSeparacao;

                if (tipoEmbalagem != null)
                    produto.TipoEmbalagem = tipoEmbalagem;

                if (!string.IsNullOrWhiteSpace(codigoEAN))
                    produto.CodigoEAN = codigoEAN;

                repositorioProdutoEmbarcador.Atualizar(produto, Auditado);
            }
            else
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarConfiguracaoPadrao();

                if ((configuracaoWebService?.AtivarValidacaoDosProdutosNoAdicionarCarga ?? false) && string.IsNullOrEmpty(descricaoProdutoEmbarcador))
                    throw new ServicoException("Produto não cadastrado, descrição não informada, cadastro não foi realizado.");

                produto = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador();
                produto.Integrado = false;
                produto.CodigoProdutoEmbarcador = codigoProdutoEmbarcador;
                produto.Descricao = descricaoProdutoEmbarcador;
                produto.PesoUnitario = pesoUnitario;
                produto.MetroCubito = metroCubito;
                produto.AlturaCM = altura;
                produto.LarguraCM = largura;
                produto.ComprimentoCM = comprimento;
                produto.Observacao = observacaoProduto;

                if (produto.MetroCubito <= 0)
                    produto.MetroCubito = metroCubicoCalculado;

                if (!string.IsNullOrWhiteSpace(unidadeMedida))
                {
                    Dominio.Entidades.UnidadeDeMedida unidadeDeMedida = repositorioUnidadeMedida.BuscarPorSigla(unidadeMedida);
                    produto.Unidade = unidadeDeMedida;
                    produto.SiglaUnidade = unidadeMedida.Left(10);
                }

                produto.CodigoNCM = codigoNCM;
                produto.QuantidadeCaixa = quantidadePorCaixa;
                produto.QuantidadeCaixaPorPallet = quantidadeCaixaPorPallet;
                produto.CodigoDocumentacao = codigoDocumentacao;
                produto.CodigoCEAN = codigoProdutoCEAN;

                if (grupoProduto != null)
                {
                    produto.GrupoProduto = grupoProduto;
                    grupoProduto.Ativo = true;
                }

                produto.LinhaSeparacao = linhaSeparacao;
                produto.TipoEmbalagem = tipoEmbalagem;

                produto.Ativo = true;

                if (!string.IsNullOrWhiteSpace(codigoEAN))
                    produto.CodigoEAN = codigoEAN;

                if (Auditado != null)
                    repositorioProdutoEmbarcador.Inserir(produto, Auditado);
                else
                    repositorioProdutoEmbarcador.Inserir(produto);
            }

            return produto;
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador IntegrarProduto(string codigoProdutoEmbarcador, string codigoProdutoCEAN, int grupoPessoa, string descricaoProdutoEmbarcador, decimal pesoUnitario, Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto, decimal metroCubito, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, string codigoDocumento, bool inativarCadastro, bool atualizar, string codigoNCM, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int quantidadeCaixas, string siglaUnidade, string temperaturaTransporte, decimal pesoLiquidoUnitario, decimal qtdPalet, decimal alturaCM, decimal larguraCM, decimal comprimentoCM, string observacao, int quantidadeCaixaPorPallet)
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);
            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = null;

            if (grupoPessoa > 0)
            {
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    produto = repositorioProdutoEmbarcador.buscarPorCodigoEmbarcador(codigoProdutoEmbarcador, grupoPessoa);
                else
                    produto = repositorioProdutoEmbarcador.buscarPorCodigoEmbarcadorComGrupo(codigoProdutoEmbarcador, grupoPessoa);
            }
            else
                produto = repositorioProdutoEmbarcador.buscarPorCodigoEmbarcador(codigoProdutoEmbarcador);

            if (produto != null)
            {
                if (!atualizar)
                {
                    if (quantidadeCaixas > 0 && (produto.QuantidadeCaixa != quantidadeCaixas || produto.Descricao != descricaoProdutoEmbarcador))
                        atualizar = true;
                }

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = ObterConfiguracaoGeralCarga();
                bool utilizarPesoProdutoParaCalcularPesoCarga = configuracaoGeralCarga?.UtilizarPesoProdutoParaCalcularPesoCarga ?? false;

                bool dadosProdutoAtualizados = (
                    (produto.Ativo && inativarCadastro) ||
                    (!produto.Ativo && !inativarCadastro) ||
                    (codigoNCM != null && produto.CodigoNCM != codigoNCM) ||
                    (codigoDocumento != null && produto.CodigoDocumentacao != codigoDocumento) ||
                    (produto.MetroCubito != metroCubito) ||
                    (codigoProdutoCEAN != null && produto.CodigoCEAN != codigoProdutoCEAN) ||
                    (produto.Descricao != descricaoProdutoEmbarcador) ||
                    (grupoProduto != null && produto.GrupoProduto != null && !produto.GrupoProduto.Equals(grupoProduto)) ||
                    (produto.PesoUnitario != pesoUnitario && pesoUnitario > 0 && !utilizarPesoProdutoParaCalcularPesoCarga)
                );

                if (dadosProdutoAtualizados && !atualizar)
                    atualizar = true;

                if (!atualizar)
                    return produto;

                if (Auditado != null)
                    produto.Initialize();

                produto.Integrado = false;
                produto.Descricao = descricaoProdutoEmbarcador;
                produto.CodigoCEAN = codigoProdutoCEAN;
                produto.CodigoNCM = codigoNCM;
                produto.MetroCubito = metroCubito;
                produto.CodigoDocumentacao = codigoDocumento;
                produto.Ativo = !inativarCadastro;
                if (quantidadeCaixas > 0)
                    produto.QuantidadeCaixa = quantidadeCaixas;
                produto.SiglaUnidade = siglaUnidade;
                produto.TemperaturaTransporte = temperaturaTransporte;

                if (pesoLiquidoUnitario > 0)
                    produto.PesoLiquidoUnitario = pesoLiquidoUnitario;

                produto.QtdPalet = qtdPalet;
                produto.AlturaCM = alturaCM;
                produto.LarguraCM = larguraCM;
                produto.ComprimentoCM = comprimentoCM;
                produto.Observacao = observacao;
                if (quantidadeCaixaPorPallet > 0)
                    produto.QuantidadeCaixaPorPallet = quantidadeCaixaPorPallet;

                if (!utilizarPesoProdutoParaCalcularPesoCarga && pesoUnitario > 0)
                    produto.PesoUnitario = pesoUnitario;

                if (grupoProduto != null)
                    produto.GrupoProduto = grupoProduto;

                repositorioProdutoEmbarcador.Atualizar(produto, Auditado);
            }
            else
            {
                produto = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador();
                produto.Integrado = false;
                produto.CodigoProdutoEmbarcador = codigoProdutoEmbarcador;
                produto.Descricao = descricaoProdutoEmbarcador;
                produto.PesoUnitario = pesoUnitario;
                produto.MetroCubito = metroCubito;
                produto.CodigoNCM = codigoNCM;
                produto.GrupoPessoas = grupoPessoa > 0 ? new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas() { Codigo = grupoPessoa } : null;
                produto.CodigoCEAN = codigoProdutoCEAN;
                produto.CodigoDocumentacao = codigoDocumento;
                produto.Ativo = true;
                produto.QuantidadeCaixa = quantidadeCaixas;
                produto.SiglaUnidade = siglaUnidade;
                produto.TemperaturaTransporte = temperaturaTransporte;
                produto.PesoLiquidoUnitario = pesoLiquidoUnitario;
                produto.QtdPalet = qtdPalet;
                produto.AlturaCM = alturaCM;
                produto.LarguraCM = larguraCM;
                produto.ComprimentoCM = comprimentoCM;
                produto.Observacao = observacao;
                produto.QuantidadeCaixaPorPallet = quantidadeCaixaPorPallet;

                if (grupoProduto != null)
                {
                    produto.GrupoProduto = grupoProduto;
                    grupoProduto.Ativo = true;
                }

                repositorioProdutoEmbarcador.Inserir(produto, Auditado);
            }

            return produto;
        }

        #endregion Métodos Públicos
    }
}
