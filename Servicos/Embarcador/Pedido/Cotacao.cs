using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Pedido
{
    public class Cotacao
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos Globais

        #region Construtores

        public Cotacao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao> ObterCotacoes(Dominio.ObjetosDeValor.WebService.Pedido.Cotacao cotacao, string AdminStringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool apenasConsulta = false)
        {
            Servicos.Cliente servicoCliente = new Servicos.Cliente(_unitOfWork.StringConexao);
            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(_unitOfWork);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.RotaFreteEmpresaExclusiva repRotaFreteEmpresaExclusiva = new Repositorio.RotaFreteEmpresaExclusiva(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega repTabelaFreteClienteFrequenciaEntrega = new Repositorio.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega(_unitOfWork);
            Repositorio.RotaFreteFrequenciaEntrega repRotaFreteFrequenciaEntrega = new Repositorio.RotaFreteFrequenciaEntrega(_unitOfWork);
            Repositorio.Embarcador.Cotacao.RegraCotacao repRegraCotacao = new Repositorio.Embarcador.Cotacao.RegraCotacao(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);
            Repositorio.Embarcador.Logistica.RotaFreteCEP repRotaFreteCEP = new Repositorio.Embarcador.Logistica.RotaFreteCEP(_unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteFrequenciaCarregamento repClienteFrequenciaCarregamento = new Repositorio.Embarcador.Pessoas.ClienteFrequenciaCarregamento(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Aliquota repAliquota = new Repositorio.Aliquota(_unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(_unitOfWork);

            Servicos.Embarcador.Carga.ICMS serCargaICMS = new Servicos.Embarcador.Carga.ICMS(_unitOfWork);
            Servicos.Embarcador.Carga.ISS serCargaISS = new Carga.ISS(_unitOfWork);

            List<Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao> cotacoes = new List<Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao>();
            if (cotacao.EnderecoDestino == null)
                throw new ServicoException("É obrigatório indicar o endereço de entrega.");

            if (cotacao.Expedidor == null)
                throw new ServicoException("É obrigatório indicar o local da expedição de entrega.");

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
            if (cotacao.TipoOperacao != null)
            {
                tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao(cotacao.TipoOperacao.CodigoEmbarcador);
                if (tipoOperacao == null)
                    throw new ServicoException("Não foi localizado um tipo de operação compativel com o indicado na cotação.");
            }

            Dominio.Entidades.Cliente destinatario = null;
            if (!string.IsNullOrWhiteSpace(cotacao.Destinatario.RazaoSocial))
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversao = servicoCliente.ConverterObjetoValorPessoa(cotacao?.Destinatario ?? null, "Destinatario", _unitOfWork, 0, false, false, null, tipoServicoMultisoftware, false, false, null);
                if (retornoConversao.Status)
                    destinatario = retornoConversao.cliente;
            }
            else
            {
                double cnpjcpf = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(cotacao.Destinatario?.CPFCNPJ ?? string.Empty), out cnpjcpf);
                destinatario = repCliente.BuscarPorCPFCNPJ(cnpjcpf);
            }

            AdminMultisoftware.Dominio.Entidades.Localidades.Endereco enderecoCEP = null;

            using (AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoSessaoBancoDados.Nova))
            {
                AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);
                enderecoCEP = repEndereco.BuscarCEP(Utilidades.String.OnlyNumbers(cotacao.EnderecoDestino.CEP));

                if (enderecoCEP == null)
                    enderecoCEP = repEndereco.BuscarCEP(Utilidades.String.OnlyNumbers(destinatario.CEP));
            }

            if (enderecoCEP == null)
                throw new ServicoException("Não foi localizado um endereço válido para o CEP informado.");

            Dominio.Entidades.Cliente expedidor = null;
            if (!string.IsNullOrWhiteSpace(cotacao.Expedidor.RazaoSocial))
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversao = servicoCliente.ConverterObjetoValorPessoa(cotacao.Expedidor, "Expedidor", _unitOfWork, 0, false, false, null, tipoServicoMultisoftware, false, false, null);
                if (retornoConversao.Status)
                    expedidor = retornoConversao.cliente;
            }
            else
            {
                double cnpjcpf = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(cotacao.Expedidor.CPFCNPJ), out cnpjcpf);
                expedidor = repCliente.BuscarPorCPFCNPJ(cnpjcpf);
            }

            if (expedidor == null)
                throw new ServicoException("Não foi localizado um expedidor compatível com o indicado na cotação.");

            Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco enderecoEntrega = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
            enderecoEntrega.CEP = enderecoCEP.CEP;
            enderecoEntrega.Bairro = enderecoCEP.Bairro?.Descricao ?? "Não Informado";
            enderecoEntrega.Cidade = new Dominio.ObjetosDeValor.Localidade();
            enderecoEntrega.Cidade.Descricao = enderecoCEP.Localidade?.Descricao ?? "";
            enderecoEntrega.Cidade.SiglaUF = enderecoCEP.Localidade?.Estado?.UF ?? "";
            enderecoEntrega.Cidade.IBGE = int.Parse(enderecoCEP.Localidade?.CodigoIBGE ?? "0");
            enderecoEntrega.Logradouro = enderecoCEP.Logradouro;

            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointDestino = null;
            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointOrigem = null;
            if (enderecoCEP.Geo != null)
            {
                wayPointDestino = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(enderecoCEP.Geo.latitude, enderecoCEP.Geo.longitude);
                enderecoEntrega.Latitude = enderecoCEP.Geo.latitude.ToString().Replace(",", ".");
                enderecoEntrega.Longitude = enderecoCEP.Geo.longitude.ToString().Replace(",", ".");
            }

            if (!string.IsNullOrWhiteSpace(expedidor.Latitude))
                wayPointOrigem = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(expedidor.Latitude, expedidor.Longitude);

            double metros = 0;
            double km = 0;
            if (wayPointOrigem != null && wayPointDestino != null)
                metros = Servicos.Embarcador.Logistica.Polilinha.CalcularDistancia(wayPointOrigem, wayPointDestino);

            if (metros > 0)
                km = (metros / 1000);

            Dominio.Entidades.Localidade destino = repLocalidade.BuscarPorCodigoIBGE(enderecoEntrega.Cidade.IBGE);

            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new Servicos.Embarcador.Carga.FreteCliente(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = ObterParametrosCalculoFrete(cotacao, tipoOperacao, destino, expedidor, destinatario);

            List<Dominio.Entidades.Aliquota> aliquotas = repAliquota.BuscaAliquotas(expedidor.Localidade.Estado.Sigla, destino.Estado.Sigla, expedidor.Atividade.Codigo);

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteFrequenciaCarregamento> clienteFrequenciaCarregamento = repClienteFrequenciaCarregamento.BuscarPorCliente(expedidor.CPF_CNPJ);
            List<Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP> rotasFreteCEP = repRotaFreteCEP.BuscarPorCEP(expedidor.CPF_CNPJ, expedidor.Localidade.Estado.Sigla, Utilidades.String.OnlyNumbers(enderecoEntrega.CEP).ToInt());
            List<Dominio.Entidades.RotaFrete> rotaFretes = repRotaFrete.BuscarRotasPorOrigemDestino(expedidor.Localidade, destino);
            List<Dominio.Entidades.RotaFreteEmpresaExclusiva> rotaFreteEmpresaExclusivas = new List<Dominio.Entidades.RotaFreteEmpresaExclusiva>();

            if (rotasFreteCEP.Count > 0)
                rotaFretes.AddRange((from obj in rotasFreteCEP select obj.RotaFrete).Distinct().ToList());

            if (rotaFretes.Count > 0)
                rotaFreteEmpresaExclusivas = repRotaFreteEmpresaExclusiva.BuscarPorRotasFretes((from obj in rotaFretes select obj.Codigo).ToList());

            StringBuilder mensagemRetorno = new StringBuilder();

            List<Dominio.Entidades.Embarcador.Cotacao.RegraCotacao> regraCotacaos = repRegraCotacao.ObterRegrasAtivas();

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtosEmbarcador = null;

            if (apenasConsulta)
                produtosEmbarcador = repProdutoEmbarcador.buscarPorCodigosEmbarcador((from obj in cotacao.Produtos select obj.CodigoProduto).Distinct().ToList());
            else
                produtosEmbarcador = InserirProdutos(cotacao.Produtos, configuracao);

            List<int> grupoProdutosSimulado = new List<int>();
            foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto in cotacao.Produtos.ToList())
            {
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = (from obj in produtosEmbarcador where obj.CodigoProdutoEmbarcador == produto.CodigoProduto select obj).FirstOrDefault();
                decimal cubagemProduto = 0;
                decimal percentualCorrecao = produtoEmbarcador?.GrupoProduto?.PorcentagemCorrecao ?? 0m;

                decimal alturaCM = ObterMedidaComPorcentagemCorrecao(produto.Altura, percentualCorrecao);
                decimal larguraCM = ObterMedidaComPorcentagemCorrecao(produto.Largura, percentualCorrecao);
                decimal comprimentoCM = ObterMedidaComPorcentagemCorrecao(produto.Comprimento, percentualCorrecao);

                if (produto.ProdutoSimulado)
                {
                    cubagemProduto += (alturaCM * larguraCM * comprimentoCM) * produto.Quantidade;
                }
                else
                {
                    int quantidadePorCaixa = produtoEmbarcador.QuantidadeCaixa > 0 ? produtoEmbarcador.QuantidadeCaixa : (produtoEmbarcador.GrupoProduto?.QuantidadePorCaixa ?? 0);
                    if (configuracao.CubagemPorCaixa && quantidadePorCaixa > 0)
                    {
                        int resto = (int)Math.Ceiling(produto.Quantidade / quantidadePorCaixa);
                        cubagemProduto = produtoEmbarcador.MetroCubito * resto;
                    }
                    else
                        cubagemProduto = produtoEmbarcador.MetroCubito * produto.Quantidade;

                    produto.Altura = produtoEmbarcador.AlturaCM;
                    produto.Largura = produtoEmbarcador.LarguraCM;
                    produto.Comprimento = produtoEmbarcador.ComprimentoCM;
                }

                parametrosCalculo.Cubagem += cubagemProduto;
                parametrosCalculo.MaiorAlturaProdutoEmCentimetros = Math.Max(parametrosCalculo.MaiorAlturaProdutoEmCentimetros, alturaCM);
                parametrosCalculo.MaiorLarguraProdutoEmCentimetros = Math.Max(parametrosCalculo.MaiorLarguraProdutoEmCentimetros, larguraCM);
                parametrosCalculo.MaiorComprimentoProdutoEmCentimetros = Math.Max(parametrosCalculo.MaiorComprimentoProdutoEmCentimetros, comprimentoCM);
                parametrosCalculo.MaiorVolumeProdutoEmCentimetros = Math.Max(parametrosCalculo.MaiorVolumeProdutoEmCentimetros, (alturaCM + larguraCM + comprimentoCM));

                if (produto.ProdutoSimulado && !string.IsNullOrWhiteSpace(produto.CodigoGrupoProduto))
                    grupoProdutosSimulado.Add(produto.CodigoGrupoProduto.ToInt());
            }

            List<decimal> dimensoesMaiorProduto = new List<decimal>();
            dimensoesMaiorProduto.Add(parametrosCalculo.MaiorAlturaProdutoEmCentimetros);
            dimensoesMaiorProduto.Add(parametrosCalculo.MaiorLarguraProdutoEmCentimetros);
            dimensoesMaiorProduto.Add(parametrosCalculo.MaiorComprimentoProdutoEmCentimetros);

            if (parametrosCalculo.Cubagem > 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFreteQuantidade cubagem = new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFreteQuantidade();
                cubagem.Quantidade = parametrosCalculo.Cubagem;
                cubagem.UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.M3;
                parametrosCalculo.Quantidades.Add(cubagem);
            }

            RegraAutorizacao.AprovacaoAlcadaSemPadronizacao servicoAprovacao = new RegraAutorizacao.AprovacaoAlcadaSemPadronizacao();

            List<int> grupoProdutosCadastrados = (from obj in produtosEmbarcador where obj.GrupoProduto != null select obj.GrupoProduto.Codigo).Distinct().ToList();
            List<int> linhasSeparacao = (from obj in produtosEmbarcador where obj.LinhaSeparacao != null select obj.LinhaSeparacao.Codigo).Distinct().ToList();
            List<int> marcas = (from obj in produtosEmbarcador where obj.MarcaProduto != null select obj.MarcaProduto.Codigo).Distinct().ToList();
            List<int> produtos = (from obj in produtosEmbarcador select obj.Codigo).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Cotacao.RegraCotacao> regrasValidas = new List<Dominio.Entidades.Embarcador.Cotacao.RegraCotacao>();
            List<int> grupoProdutos = new List<int>(grupoProdutosSimulado);
            grupoProdutos.AddRange(grupoProdutosCadastrados);

            foreach (Dominio.Entidades.Embarcador.Cotacao.RegraCotacao regraCotacao in regraCotacaos)
            {
                if (regraCotacao.RegraPorCEPDestino && !servicoAprovacao.ValidarAlcadas(regraCotacao.RegrasCepDestino.ToList(), enderecoEntrega.CEP))
                    continue;
                if (regraCotacao.RegraPorCubagem && !servicoAprovacao.ValidarAlcadas(regraCotacao.RegrasCubagem.ToList(), parametrosCalculo.Cubagem))
                    continue;
                if (regraCotacao.RegraPorDistancia && !servicoAprovacao.ValidarAlcadas(regraCotacao.RegrasDistancia.ToList(), km))
                    continue;
                if (regraCotacao.RegraPorEstadoDestino && !servicoAprovacao.ValidarAlcadas(regraCotacao.RegrasEstadoDestino.ToList(), destino.Estado?.Sigla ?? ""))
                    continue;
                if (regraCotacao.RegraPorExpedidor && !servicoAprovacao.ValidarAlcadas(regraCotacao.RegrasExpedidor.ToList(), expedidor?.CPF_CNPJ ?? 0))
                    continue;
                if (regraCotacao.RegraPorGrupoProduto && !servicoAprovacao.ValidarAlcadas(regraCotacao.RegrasGrupoProduto.ToList(), grupoProdutos))
                    continue;
                if (regraCotacao.RegraPorLinhaSeparacao && !servicoAprovacao.ValidarAlcadas(regraCotacao.RegrasLinhaSeparacao.ToList(), linhasSeparacao))
                    continue;
                if (regraCotacao.RegraPorMarcaProduto && !servicoAprovacao.ValidarAlcadas(regraCotacao.RegrasMarcaProduto.ToList(), marcas))
                    continue;
                if (regraCotacao.RegraPorPeso && !servicoAprovacao.ValidarAlcadas(regraCotacao.RegrasPeso.ToList(), parametrosCalculo.Peso))
                    continue;
                if (regraCotacao.RegraPorProduto && !servicoAprovacao.ValidarAlcadas(regraCotacao.RegrasProduto.ToList(), produtos))
                    continue;
                if (regraCotacao.RegraPorValorMercadoria && !servicoAprovacao.ValidarAlcadas(regraCotacao.RegrasValorMercadoria.ToList(), parametrosCalculo.ValorNotasFiscais))
                    continue;
                if (regraCotacao.RegraPorVolume && !servicoAprovacao.ValidarAlcadas(regraCotacao.RegrasVolume.ToList(), parametrosCalculo.Volumes))
                    continue;
                if (regraCotacao.RegraPorArestaProduto && !servicoAprovacao.ValidarAlcadas(regraCotacao.RegrasArestaProduto.ToList(), dimensoesMaiorProduto.Max()))
                    continue;
                if (regraCotacao.RegraPorDestinatario && !servicoAprovacao.ValidarAlcadas(regraCotacao.RegrasDestinatario.ToList(), destinatario?.CPF_CNPJ ?? 0))
                    continue;

                regrasValidas.Add(regraCotacao);
            }

            if (regrasValidas.Any(obj => obj.TipoAplicacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoCotacao.UtilizarModeloVeicular))
                parametrosCalculo.ModeloVeiculo = regrasValidas.Where(obj => obj.TipoAplicacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoCotacao.UtilizarModeloVeicular).Select(obj => obj.ModeloVeicularCarga).FirstOrDefault();

            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasCliente = svcFreteCliente.ObterTabelasFrete(ref mensagemRetorno, parametrosCalculo, null, tipoServicoMultisoftware);
            tabelasCliente = tabelasCliente.Where(obj => obj.TabelaFrete.AplicacaoTabela != Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoTabela.Ocorrencia).Select(obj => obj).ToList();

            if (apenasConsulta && tabelasCliente.Count == 0)
                throw new ServicoException("Não foi localizado tabela de frete para os dados informados.");

            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe> transportadorConfiguracaoNFSes = new List<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe>();
            bool municiapal = false;
            if (expedidor.Localidade.Codigo == destino.Codigo)
            {
                transportadorConfiguracaoNFSes = repTransportadorConfiguracaoNFSe.BuscarTodas();
                municiapal = true;
            }

            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega> tabelaFreteClienteFrequenciasEntrega = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega>();
            if (tabelasCliente.Count > 0)
                tabelaFreteClienteFrequenciasEntrega = repTabelaFreteClienteFrequenciaEntrega.BuscarPortabelasCliente((from obj in tabelasCliente select obj.Codigo).ToList());

            List<Dominio.Entidades.RotaFreteFrequenciaEntrega> rotaFreteFrequenciasEntrega = new List<Dominio.Entidades.RotaFreteFrequenciaEntrega>();
            if (rotaFretes.Count > 0)
                rotaFreteFrequenciasEntrega = repRotaFreteFrequenciaEntrega.BuscarPorRotasFrete((from obj in rotaFretes select obj.Codigo).ToList());


            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = repTabelaFrete.BuscarPorCodigos((from obj in tabelasCliente select obj.TabelaFrete.Codigo).Distinct().ToList());

            List<int> codigosTransportadores = new List<int>();
            foreach (Dominio.Entidades.Embarcador.Frete.TabelaFrete tabela in tabelasFrete)
                codigosTransportadores.AddRange((from obj in tabela.Transportadores select obj.Codigo).ToList());

            List<Dominio.Entidades.Empresa> empresas = repEmpresa.BuscarPorCodigos(codigosTransportadores);

            List<int> codigosFiliais = new List<int>();
            foreach (Dominio.Entidades.Empresa emp in empresas)
                codigosFiliais.AddRange((from obj in emp.Filiais select obj.Codigo).ToList());
            List<Dominio.Entidades.Empresa> todasFiliaisEmpresas = repEmpresa.BuscarPorCodigos(codigosFiliais);

            List<Dominio.Entidades.Empresa> matrizes = new List<Dominio.Entidades.Empresa>();
            foreach (Dominio.Entidades.Empresa empresa in empresas)
            {
                if (!todasFiliaisEmpresas.Contains(empresa))
                    matrizes.Add(empresa);
            }
            empresas = matrizes;

            List<Dominio.Entidades.Empresa> transportadorUsar = new List<Dominio.Entidades.Empresa>();
            List<Dominio.Entidades.Empresa> transportadorExcluir = new List<Dominio.Entidades.Empresa>();

            List<Dominio.Entidades.Embarcador.Cotacao.RegraCotacao> regrasPorTransportador = new List<Dominio.Entidades.Embarcador.Cotacao.RegraCotacao>();

            foreach (Dominio.Entidades.Embarcador.Cotacao.RegraCotacao regraValida in regrasValidas)
            {
                if (regraValida.TipoAplicacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoCotacao.ExcluirTransportador)
                    transportadorExcluir.AddRange((from obj in regraValida.Transportadores select obj).ToList());
                else if (regraValida.TipoAplicacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoCotacao.UsarTransportador)
                {
                    transportadorUsar = new List<Dominio.Entidades.Empresa>(); //adicionar somente a de maior prioridade.
                    transportadorUsar.AddRange((from obj in regraValida.Transportadores select obj).ToList());
                }
                else
                    regrasPorTransportador.Add(regraValida);
            }

            transportadorExcluir = transportadorExcluir.Distinct().ToList();
            List<Dominio.Entidades.Empresa> transportadorValidarUsar = transportadorUsar.Distinct().ToList();

            //abaixo faz a seguinte validação, se o transportador está na lista de excluir ele é removido da lista de usar desta forma as demais transportadoras são eliminadas.
            transportadorUsar = new List<Dominio.Entidades.Empresa>();
            foreach (Dominio.Entidades.Empresa usar in transportadorValidarUsar)
            {
                if (!transportadorExcluir.Contains(usar))
                    transportadorUsar.Add(usar);
            }

            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelaFretes = new List<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            List<Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacaoComposto> coposicoesRetorno = new List<Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacaoComposto>();
            foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente in tabelasCliente)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = (from obj in tabelasFrete where obj.Codigo == tabelaFreteCliente.TabelaFrete.Codigo select obj).FirstOrDefault();

                if (!tabelaFretes.Contains(tabelaFrete))
                {
                    int diasAdd = 0;
                    bool usaDiasFixos = false;
                    bool usaRegraValorFixo = false;
                    int diasFixos = 0;
                    decimal percentualFrete = 0;
                    decimal valorCobranca = 0;
                    decimal valorFixoCotacaoFrete = 0;

                    if (tipoOperacao == null || tabelaFrete.TiposOperacao.Count == 0 || tabelaFrete.TiposOperacao.Contains(tipoOperacao))
                    {

                        if (tabelaFrete.Filiais.Count == 0 || tabelaFrete.Filiais.Any(obj => Utilidades.String.OnlyNumbers(obj.CNPJ) == expedidor.CPF_CNPJ_SemFormato))
                        {
                            tabelaFretes.Add(tabelaFrete);
                            //int codigoTransportador = tabelaFrete.Transportadores.FirstOrDefault()?.Codigo ?? 0;
                            List<int> codigosTransportador = (from obj in tabelaFrete.Transportadores select obj.Codigo).ToList();
                            int codigoTransportador = (from obj in empresas where codigosTransportador.Contains(obj.Codigo) && obj.UtilizaTransportadoraPadraoContratacao != true select obj.Codigo).OrderBy(obj => obj).FirstOrDefault();
                            Dominio.Entidades.Empresa empresa = (from obj in empresas where obj.Codigo == codigoTransportador select obj).FirstOrDefault();
                            if (empresa == null)
                                continue;

                            if (transportadorExcluir.Count > 0 && transportadorExcluir.Any(obj => obj.Codigo == empresa?.Codigo))
                                continue;

                            if (transportadorUsar.Count > 0 && !transportadorUsar.Any(obj => obj.Codigo == empresa?.Codigo))
                                continue;

                            Dominio.Entidades.RotaFrete rotaFrete = null;
                            Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP rotaFreteCEP = null;
                            Dominio.Entidades.RotaFreteEmpresaExclusiva rotaFreteEmpresaExclusiva = (from obj in rotaFreteEmpresaExclusivas where obj.Empresa.Codigo == (empresa?.Codigo ?? 0) select obj).FirstOrDefault();
                            if (rotaFreteEmpresaExclusiva != null)
                            {
                                rotaFrete = (from obj in rotaFretes where obj.Codigo == rotaFreteEmpresaExclusiva.RotaFrete.Codigo select obj).FirstOrDefault();
                                if (rotaFrete != null)
                                    rotaFreteCEP = (from obj in rotasFreteCEP where obj.RotaFrete.Codigo == rotaFrete.Codigo select obj).FirstOrDefault();
                            }

                            parametrosCalculo.PercentualFixoAdValorem = rotaFreteCEP?.PercentualADValorem ?? 0;
                            parametrosCalculo.CalcularFretePorPesoCubado = tabelaFrete.CalcularFretePorPesoCubado;
                            parametrosCalculo.AplicarMaiorValorEntrePesoEPesoCubado = tabelaFrete.AplicarMaiorValorEntrePesoEPesoCubado;
                            parametrosCalculo.PesoCubado = 0;
                            parametrosCalculo.IsencaoCubagem = 0;
                            if (parametrosCalculo.CalcularFretePorPesoCubado && tabelaFrete.FatorCubagem > 0 && parametrosCalculo.Cubagem > 0)
                            {
                                parametrosCalculo.PesoCubado = parametrosCalculo.Cubagem * tabelaFrete.FatorCubagem;
                                parametrosCalculo.IsencaoCubagem = tabelaFrete.IsencaoCubagem;
                            }

                            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();

                            dadosCalculoFrete.TabelaFrete = tabelaFrete;

                            if (tabelaFreteCliente != null)
                            {
                                if (tabelaFrete.ParametroBase.HasValue)
                                    svcFreteCliente.SetarValoresTabelaFreteComParametroBase(ref dadosCalculoFrete, parametrosCalculo, tabelaFreteCliente, tipoServicoMultisoftware, configuracao);
                                else
                                    svcFreteCliente.SetarValoresTabelaFreteSemParametroBase(ref dadosCalculoFrete, parametrosCalculo, tabelaFreteCliente, tipoServicoMultisoftware, configuracao);

                                dadosCalculoFrete.FreteCalculado = true;
                                dadosCalculoFrete.TabelaFreteCliente = tabelaFreteCliente;
                            }

                            if (dadosCalculoFrete.ValorTotal == 0)
                                continue;

                            Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao retornoCotacao = new Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao();
                            retornoCotacao.EnderecoDestino = enderecoEntrega;
                            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana> diasSemanaCarregamento = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana>();

                            diasSemanaCarregamento = (from obj in clienteFrequenciaCarregamento where obj.Empresa.Codigo == empresa.Codigo select obj.DiaSemana).ToList();

                            foreach (Dominio.Entidades.Embarcador.Cotacao.RegraCotacao regraTransportador in regrasPorTransportador)
                            {
                                if (regraTransportador.RegraPorTransportador && !servicoAprovacao.ValidarAlcadas(regraTransportador.RegrasCotacaoTransportador.ToList(), empresa.Codigo))
                                    continue;

                                if (regraTransportador.TipoAplicacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoCotacao.AdicionarDiasAoFrete)
                                    diasAdd += regraTransportador.NumeroDiasFrete.Value;
                                else if (regraTransportador.TipoAplicacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoCotacao.FixarDiasDeFrete)
                                {
                                    diasFixos = regraTransportador.NumeroDiasFrete.Value;
                                    usaDiasFixos = true;
                                }
                                else if (regraTransportador.TipoAplicacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoCotacao.ValorPercentualCobrancaCliente)
                                    percentualFrete = regraTransportador.PercentualCobranca.Value;
                                else if (regraTransportador.TipoAplicacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoCotacao.ValorParaCobranca)
                                    valorCobranca = regraTransportador.ValorCobranca;
                                else if (regraTransportador.TipoAplicacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoCotacao.FixarValorCotacaoFrete)
                                {
                                    usaRegraValorFixo = true;
                                    valorFixoCotacaoFrete = regraTransportador.ValorFixoCotacaoFrete;
                                }
                            }

                            DateTime dataBase = ObterDataBase(empresa);

                            if (diasSemanaCarregamento.Count > 0)
                                ObterDataBaseFrequenciaCarregamento(ref dataBase, diasSemanaCarregamento);

                            int quantidadeDias = diasFixos;

                            if (tabelaFreteCliente.CEPsDestino?.Count > 0)
                            {
                                int cep = enderecoCEP.CEP.ObterSomenteNumeros().ToInt();
                                quantidadeDias = tabelaFreteCliente.CEPsDestino.FirstOrDefault(o => cep >= o.CEPInicial && cep <= o.CEPFinal)?.DiasUteis ?? diasFixos;
                            }
                            else if (!usaDiasFixos)
                            {
                                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana> diasSemana = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana>();

                                if (tabelaFreteCliente.LeadTime > 0)
                                {
                                    quantidadeDias = tabelaFreteCliente.LeadTime;
                                    List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega> tabelaFreteFrequencia = (from obj in tabelaFreteClienteFrequenciasEntrega where obj.TabelaFreteCliente.Codigo == tabelaFreteCliente.Codigo select obj).ToList();
                                    diasSemana = (from obj in tabelaFreteFrequencia select obj.DiaSemana).ToList();
                                }
                                else if (rotaFrete != null)
                                {
                                    if (rotaFreteCEP != null && rotaFreteCEP.LeadTime.HasValue)
                                        quantidadeDias = rotaFreteCEP.LeadTime.Value;
                                    else
                                        quantidadeDias = rotaFrete.TempoDeViagemEmMinutos / 1440;

                                    List<Dominio.Entidades.RotaFreteFrequenciaEntrega> rotaFreteFrequencias = (from obj in rotaFreteFrequenciasEntrega where obj.RotaFrete.Codigo == rotaFrete.Codigo select obj).ToList();
                                    diasSemana = (from obj in rotaFreteFrequencias select obj.DiaSemana).ToList();
                                }
                                quantidadeDias += diasAdd;

                                if (diasSemana.Count > 0)
                                    ObterLeadTimeComFrequenciaEntrega(dataBase, diasSemana, ref quantidadeDias);
                            }

                            retornoCotacao.DataPrevisaoColeta = dataBase.ToString("dd/MM/yyyy");

                            DateTime previsao = dataBase;

                            for (int i = 1; i <= quantidadeDias; i++)
                            {
                                previsao = previsao.AddDays(1);

                                if (previsao.DayOfWeek == DayOfWeek.Saturday)
                                    previsao = previsao.AddDays(2);
                                else if (previsao.DayOfWeek == DayOfWeek.Sunday)
                                    previsao = previsao.AddDays(1);
                            }

                            retornoCotacao.DataPrazoEntrega = previsao.ToString("dd/MM/yyyy");
                            int prazoDiasUteis = 0;
                            obterPrazoDiasUteis(previsao.Date, DateTime.Now.Date.AddDays(1), ref prazoDiasUteis);
                            retornoCotacao.PrazoEntrega = prazoDiasUteis;
                            retornoCotacao.ValorCotacao = new Dominio.ObjetosDeValor.WebService.Pedido.ValorCotacao();
                            retornoCotacao.ValorCotacao.FreteProprio = dadosCalculoFrete.ValorFrete;
                            retornoCotacao.ValorCotacao.ValorTotalCotacao = dadosCalculoFrete.ValorTotal;
                            retornoCotacao.ValorCotacao.Valor = dadosCalculoFrete.ValorTotal;
                            retornoCotacao.DistanciaRaioKM = (decimal)km;
                            retornoCotacao.CodigoIntegracaoTabelaFreteCliente = tabelaFreteCliente.CodigoIntegracao;
                            retornoCotacao.CanalEntrega = tabelaFreteCliente.CanalEntrega?.Descricao ?? string.Empty;

                            List<int> codigosFiliaisEmpresa = (from obj in empresa.Filiais select obj.Codigo).ToList();
                            List<Dominio.Entidades.Empresa> filiaisEmpresa = new List<Dominio.Entidades.Empresa>();
                            foreach (int codigoFilialEmpresa in codigosFiliaisEmpresa)
                            {
                                Dominio.Entidades.Empresa empresaFilial = (from obj in todasFiliaisEmpresas where obj.Codigo == codigoFilialEmpresa select obj).FirstOrDefault();
                                if (empresaFilial != null)
                                    filiaisEmpresa.Add(empresaFilial);
                            }

                            Dominio.Entidades.Empresa empresaUtilizar = null;
                            if (empresa.Localidade.Estado.Sigla != expedidor.Localidade.Estado.Sigla)
                                empresaUtilizar = (from obj in filiaisEmpresa where obj.Localidade.Estado.Sigla == expedidor.Localidade.Estado.Sigla select obj).FirstOrDefault();

                            if (empresaUtilizar == null)
                                empresaUtilizar = empresa;

                            if (!apenasConsulta)
                                retornoCotacao.Transportador = serEmpresa.ConverterObjetoEmpresa(empresa);
                            else
                                retornoCotacao.Transportador = serEmpresa.ConverterObjetoEmpresa(empresaUtilizar);

                            if (!municiapal)
                            {
                                Dominio.Entidades.Aliquota aliquota = (from obj in aliquotas where obj.EstadoEmpresa.Sigla == empresaUtilizar.Localidade.Estado.Sigla select obj).FirstOrDefault();
                                if (aliquota != null)
                                {
                                    if (!empresaUtilizar.OptanteSimplesNacional || empresaUtilizar.Localidade.Estado.Sigla != expedidor.Localidade.Estado.Sigla)
                                    {
                                        decimal valorBaseCalculo = dadosCalculoFrete.BaseCalculoICMS;
                                        retornoCotacao.ValorCotacao.ValorICMS = serCargaICMS.CalcularICMSInclusoNoFrete(aliquota.CST, ref valorBaseCalculo, aliquota.Percentual, tabelaFrete.PercentualICMSIncluir, 0, tabelaFrete.IncluirICMSValorFrete, 0, 0);
                                        retornoCotacao.ValorCotacao.ValorICMS = Math.Round(retornoCotacao.ValorCotacao.ValorICMS, 2, MidpointRounding.AwayFromZero);
                                        retornoCotacao.ValorCotacao.Aliquota = aliquota.Percentual;
                                        retornoCotacao.ValorCotacao.BaseCalculo = valorBaseCalculo;
                                        retornoCotacao.ValorCotacao.Valor = valorBaseCalculo;
                                        retornoCotacao.ValorCotacao.ValorTotalCotacao = valorBaseCalculo + dadosCalculoFrete.ValorComponentesNaoIncluirBaseCalculoICMS;
                                    }
                                }
                            }
                            else
                            {
                                Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = (from obj in transportadorConfiguracaoNFSes where obj.Empresa.Codigo == empresaUtilizar.Codigo && obj.LocalidadePrestacao != null && obj.LocalidadePrestacao.Codigo == destino.Codigo select obj).FirstOrDefault();

                                if (transportadorConfiguracaoNFSe == null)
                                    transportadorConfiguracaoNFSe = (from obj in transportadorConfiguracaoNFSes where obj.Empresa.Codigo == empresaUtilizar.Codigo select obj).FirstOrDefault();

                                if (transportadorConfiguracaoNFSe != null)
                                {
                                    decimal valorBaseCalculo = retornoCotacao.ValorCotacao.Valor;
                                    retornoCotacao.ValorCotacao.Aliquota = transportadorConfiguracaoNFSe.AliquotaISS;

                                    decimal iss = Math.Round(serCargaISS.CalcularInclusaoISSNoFrete(ref valorBaseCalculo, transportadorConfiguracaoNFSe.AliquotaISS, transportadorConfiguracaoNFSe.IncluirISSBaseCalculo), 2, MidpointRounding.AwayFromZero);
                                    decimal retencaoISS = Math.Round(serCargaISS.CalcularRetencaoISSNoFrete(iss, transportadorConfiguracaoNFSe.RetencaoISS), 2, MidpointRounding.AwayFromZero);

                                    retornoCotacao.ValorCotacao.BaseCalculo = valorBaseCalculo;
                                    retornoCotacao.ValorCotacao.ValorISS = iss;
                                    retornoCotacao.ValorCotacao.ValorISSRetido = retencaoISS;

                                    if (transportadorConfiguracaoNFSe.IncluirISSBaseCalculo)
                                    {
                                        retornoCotacao.ValorCotacao.Valor += (iss - retencaoISS);
                                        retornoCotacao.ValorCotacao.ValorTotalCotacao += (iss - retencaoISS);
                                    }
                                }
                            }

                            if (usaRegraValorFixo)
                                retornoCotacao.ValorCotacao.ValorTotalCotacao = valorFixoCotacaoFrete;
                            else if (percentualFrete != 0)
                            {
                                decimal percentual = (100 + percentualFrete);
                                retornoCotacao.ValorCotacao.ValorTotalCotacao = Math.Round(((retornoCotacao.ValorCotacao.ValorTotalCotacao * percentual) / 100), 2);
                            }

                            retornoCotacao.ValorCotacao.ValorTotalCotacao += valorCobranca;

                            bool excluirCotacao = false;

                            foreach (Dominio.Entidades.Embarcador.Cotacao.RegraCotacao regraPorTransportador in regrasPorTransportador)
                            {
                                if (regraPorTransportador.TipoAplicacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoCotacao.ExcluirCotacao)
                                    continue;

                                if (!regraPorTransportador.RegraPorValorCotacao || servicoAprovacao.ValidarAlcadas(regraPorTransportador.RegrasValorCotacao.ToList(), retornoCotacao.ValorCotacao.FreteProprio))
                                {
                                    excluirCotacao = true;
                                    break;
                                }
                            }

                            if (excluirCotacao)
                                continue;

                            retornoCotacao.ValorCotacao.Componentes = new List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteValoresComponente>();

                            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componente in dadosCalculoFrete.Componentes)
                            {
                                Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteValoresComponente retornoCalculoFreteValoresComponente = new Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteValoresComponente();
                                retornoCalculoFreteValoresComponente.Descricao = componente.ComponenteFrete?.Descricao ?? "";
                                retornoCalculoFreteValoresComponente.Valor = componente.ValorComponente;
                                retornoCalculoFreteValoresComponente.IncluirBaseCalculoICMS = componente.IncluirBaseCalculoICMS;
                                retornoCotacao.ValorCotacao.Componentes.Add(retornoCalculoFreteValoresComponente);
                            }

                            Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacaoComposto retornoCotacaoComposto = new Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacaoComposto();
                            retornoCotacaoComposto.Empresa = empresaUtilizar;
                            retornoCotacaoComposto.RetornoCotacao = retornoCotacao;
                            coposicoesRetorno.Add(retornoCotacaoComposto);

                            cotacoes.Add(retornoCotacao);
                        }

                    }
                }
            }

            if (!apenasConsulta)
            {
                cotacoes = new List<Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao>();
                _unitOfWork.Start();

                Dominio.Entidades.Embarcador.CotacaoPedido.SolicitacaoCotacao solicitacaoCotacao = InserirSolicitacaoCotacao(cotacao, enderecoEntrega, expedidor, destino, km, produtosEmbarcador, cotacao.Produtos, configuracao);
                foreach (Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacaoComposto retornoCotacaoComposto in coposicoesRetorno)
                {
                    Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao retornoCotacao = retornoCotacaoComposto.RetornoCotacao;
                    InserirCotacaoPedido(parametrosCalculo, ref retornoCotacao, solicitacaoCotacao, retornoCotacaoComposto.Empresa, destino, expedidor, tipoOperacao);
                    cotacoes.Add(retornoCotacao);
                }
                _unitOfWork.CommitChanges();
            }


            return cotacoes;
        }

        public void obterPrazoDiasUteis(DateTime DataEntrega, DateTime dataReferencia, ref int diasUteis)
        {
            if (dataReferencia < DataEntrega)
            {
                if (dataReferencia.DayOfWeek != DayOfWeek.Sunday && dataReferencia.DayOfWeek != DayOfWeek.Saturday)
                    diasUteis++;

                dataReferencia = dataReferencia.AddDays(1);
                obterPrazoDiasUteis(DataEntrega, dataReferencia, ref diasUteis);
            }
        }

        public DateTime ObterDataBase(Dominio.Entidades.Empresa empresa)
        {
            DateTime dataBase = DateTime.Now.Date.AddDays(1);
            if (empresa?.HoraCorteCarregamento.HasValue ?? false)
            {
                if (DateTime.Now.TimeOfDay > empresa.HoraCorteCarregamento.Value)
                    dataBase = dataBase.AddDays(1);
            }

            if (dataBase.DayOfWeek == DayOfWeek.Saturday)
                dataBase = dataBase.AddDays(2);
            else if (dataBase.DayOfWeek == DayOfWeek.Sunday)
                dataBase = dataBase.AddDays(1);

            return dataBase;
        }

        public void GerarCotacao(Dominio.ObjetosDeValor.WebService.Pedido.Cotacao cotacao, Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao retornoCotacao)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtosEmbarcado = repProdutoEmbarcador.buscarPorCodigosEmbarcador((from obj in cotacao.Produtos select obj.CodigoProduto).Distinct().ToList());
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(retornoCotacao.Transportador.CNPJ);
            Dominio.Entidades.Cliente expedidor = repCliente.BuscarPorCPFCNPJ(cotacao.Expedidor.CPFCNPJ.ToDouble());
            Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(cotacao?.Destinatario?.CPFCNPJ.ToDouble() ?? 0);
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao(cotacao.TipoOperacao.CodigoEmbarcador);
            Dominio.Entidades.Localidade destino = repLocalidade.BuscarPorCodigoIBGE(retornoCotacao.EnderecoDestino.Cidade.IBGE);

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = ObterParametrosCalculoFrete(cotacao, tipoOperacao, destino, expedidor, destinatario);

            Dominio.Entidades.Embarcador.CotacaoPedido.SolicitacaoCotacao solicitacaoCotacao = InserirSolicitacaoCotacao(cotacao, retornoCotacao.EnderecoDestino, expedidor, destino, (int)retornoCotacao.DistanciaRaioKM, produtosEmbarcado, cotacao.Produtos, configuracao);
            InserirCotacaoPedido(parametrosCalculo, ref retornoCotacao, solicitacaoCotacao, empresa, destino, expedidor, tipoOperacao);
        }

        public void LimparBasePedidosContacao(bool excluirCotacaoPedido, int diasCriacaoPedido, List<int> pedidosErroExcluir, ref string msgErro)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repositorioCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosPendentesExcluir = repositorioPedido.BuscarPedidosEmContacaoParaExcluir(diasCriacaoPedido, 100);

            for (int i = 0; i < pedidosPendentesExcluir.Count; i++)
            {
                int codigoPedido = pedidosPendentesExcluir[i].Codigo;
                try
                {
                    _unitOfWork.FlushAndClear();
                    _unitOfWork.Start();

                    if (excluirCotacaoPedido)
                        repositorioCotacaoPedido.ExcluirPorCodigoPedido(codigoPedido);
                    else
                        repositorioCotacaoPedido.RemoverPedidoDeCotacoes(codigoPedido);

                    repositorioPedido.DeletaPedidoEProdutosPorCodigoPedido(codigoPedido);

                    _unitOfWork.CommitChanges();
                }
                catch (Exception ex)
                {
                    pedidosErroExcluir.Add(codigoPedido);
                    msgErro += ex.Message;
                    _unitOfWork.Rollback();
                }
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoCotacao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Data Previsão do Serviço", Propriedade = "PrevisaoServico", Tamanho = 100, Obrigatorio = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Tipo Cliente", Propriedade = "TipoCliente", Tamanho = 100, Obrigatorio = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Cliente (CPF/CNPJ) ou Cliente Prospect (Nome) ou Cliente Novo (Descrição) ou Grupo de Pessoas (Descrição)", Propriedade = "ClienteOuGupoPessoa", Tamanho = 100, Obrigatorio = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Origem", Propriedade = "Origem", Tamanho = 100, Obrigatorio = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "UF Origem", Propriedade = "UFOrigem", Tamanho = 100, Obrigatorio = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Destino", Propriedade = "Destino", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "UF Destino", Propriedade = "UFDestino", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Destinatário (CPF ou CNPJ)", Propriedade = "CNPJCPFDestinatario", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = "Tipo de Carga (Descrição ou Código Embarcador)", Propriedade = "TipoCarga", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = "Tipo de Operação (Descrição ou Código de Integracão)", Propriedade = "TipoOperacao", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 12, Descricao = "Solicitante", Propriedade = "Solicitante", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 13, Descricao = "Produto Embarcador (Código Embarcador)", Propriedade = "CodigoProdutoEmbarcador", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 14, Descricao = "Modelo Veicular (Descrição ou Código de Integração)", Propriedade = "ModeloVeicular", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 15, Descricao = "Prospecção (Nome)", Propriedade = "NomeProspeccao", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 16, Descricao = "E-mail Contato", Propriedade = "emailContato", Tamanho = 200, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 17, Descricao = "Telefone Contato", Propriedade = "TelefoneContato", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 18, Descricao = "Status", Propriedade = "Status", Tamanho = 100, Obrigatorio = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 19, Descricao = "Mudar endereço de Origem?", Propriedade = "MudarEndereçoOrigem", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 20, Descricao = "CEP Outra Localidade Origem", Propriedade = "CEPOutraLocalidadeOrigem", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 21, Descricao = "Número Outra Localidade Origem", Propriedade = "NumeroOutraLocalidadeOrigem", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 22, Descricao = "Localidade Origem", Propriedade = "LocalidadeOrigem", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 23, Descricao = "UF Localidade Origem", Propriedade = "UFLocalidadeOrigem", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 24, Descricao = "Localidade Polo Origem", Propriedade = "LocalidadePoloOrigem", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 25, Descricao = "Endereço Origem", Propriedade = "EndereçoOrigem", Tamanho = 200, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 26, Descricao = "Número Origem", Propriedade = "NumeroOrigem", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 27, Descricao = "Complemento Origem", Propriedade = "ComplementoOrigem", Tamanho = 200, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 28, Descricao = "Bairro Origem", Propriedade = "BairroOrigem", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 29, Descricao = "CEP Origem", Propriedade = "CEPOrigem", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 30, Descricao = "Telefone Origem", Propriedade = "TelefoneOrigem", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 31, Descricao = "I.E Origem", Propriedade = "IEOrigem", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 32, Descricao = "Mudar endereço de Destino?", Propriedade = "MudarEndereçoDestino", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 33, Descricao = "CEP Outra Localidade Destino", Propriedade = "CEPOutraLocalidadeDestino", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 34, Descricao = "Número Outra Localidade Destino", Propriedade = "NumeroOutraLocalidadeDestino", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 35, Descricao = "Localidade Destino", Propriedade = "LocalidadeDestino", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 36, Descricao = "UF Localidade Destino", Propriedade = "UFLocalidadeDestino", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 37, Descricao = "Localidade Polo Destino", Propriedade = "LocalidadePoloDestino", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 38, Descricao = "Endereço Destino", Propriedade = "EnderecoDestino", Tamanho = 200, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 39, Descricao = "Número Destino", Propriedade = "NumeroDestino", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 40, Descricao = "Complemento Destino", Propriedade = "ComplementoDestino", Tamanho = 200, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 41, Descricao = "Bairro Destino", Propriedade = "BairroDestino", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 42, Descricao = "CEP Destino", Propriedade = "CEPDestino", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 43, Descricao = "Telefone Destino", Propriedade = "TelefoneDestino", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 44, Descricao = "I.E Destino", Propriedade = "IEDestino", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 45, Descricao = "Data Previsão de Saída", Propriedade = "PrevisaoSaida", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 46, Descricao = "Data Previsão de Retorno", Propriedade = "PrevisaoRetorno", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 47, Descricao = "Tipo de Modal", Propriedade = "TipoModal", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 48, Descricao = "Nº Pallets", Propriedade = "NPallets", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 49, Descricao = "Peso Bruto", Propriedade = "PesoBruto", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 50, Descricao = "Valor Mercadoria", Propriedade = "ValorMercadoria", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 51, Descricao = "Qtd. Notas", Propriedade = "QtdNotas", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 52, Descricao = "Qtd. Entregas", Propriedade = "QtdEntregas", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 53, Descricao = "Temperatura", Propriedade = "Temperatura", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 54, Descricao = "KM Total", Propriedade = "KMTotal", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 55, Descricao = "Valor por KM", Propriedade = "ValorKM", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 56, Descricao = "Observação", Propriedade = "Observacao", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 57, Descricao = "Será rastreado?", Propriedade = "SeraRastreado", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 58, Descricao = "Necessário Escolta?", Propriedade = "NecessarioEscolta", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 59, Descricao = "Qtd. Escolta", Propriedade = "QtdEscolta", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 60, Descricao = "Terá Gerenciamento de Risco?", Propriedade = "TeraGerenciamentoRisco", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 61, Descricao = "Necessário Ajudantes?", Propriedade = "Ajudantes", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 62, Descricao = "Qtd. Ajudantes", Propriedade = "QtdAjudantes", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 63, Descricao = "Altura Cubagem", Propriedade = "AlturaCubagem", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 64, Descricao = "Comprimento Cubagem", Propriedade = "ComprimentoCubagem", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 65, Descricao = "Largura Cubagem", Propriedade = "LarguraCubagem", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 66, Descricao = "Qtd. Volume", Propriedade = "QtdVolumeCubagem", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 67, Descricao = "M. Cúbico", Propriedade = "MCubicoCubagem", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 68, Descricao = "Fator Cubico", Propriedade = "FatorCubicoCubagem", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 69, Descricao = "Buscar Valores da Tabela de Frete?", Propriedade = "BuscarValoresTabelaFrete", Tamanho = 200, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 70, Descricao = "Componente de Frete Descrição", Propriedade = "ComponenteFrete", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 71, Descricao = "Valor Componente Frete", Propriedade = "ValorCompFrete", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 72, Descricao = "% Acréscimo Componente Frete", Propriedade = "AcrescimoComponente", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 73, Descricao = "% Desconto Componente Frete", Propriedade = "DescontoComponente", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 74, Descricao = "% Acréscimo", Propriedade = "Acrescimo", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 75, Descricao = "% Desconto", Propriedade = "Desconto", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 76, Descricao = "Incluir valor do ICMS na Base de Cálculo ?", Propriedade = "IncluirValorICMSBaseCalculo", Tamanho = 200, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 77, Descricao = "BL Imp./Ex.", Propriedade = "BL", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 78, Descricao = "Navio Imp./Ex.", Propriedade = "Navio", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 79, Descricao = "Porto Imp./Ex.", Propriedade = "Porto", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 80, Descricao = "Terminal de Vazio Imp./Ex.", Propriedade = "TerminalImportacao", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 81, Descricao = "Data de vencimento do armazenamento Imp./Ex.", Propriedade = "DataVencimentoArmazenamento", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 82, Descricao = "Armador Imp./Ex.", Propriedade = "Armador", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 83, Descricao = "Endereço Entrega Imp./Ex.", Propriedade = "EnderecoImpEx", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 84, Descricao = "Bairro Imp./Ex.", Propriedade = "BairroImpEx", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 85, Descricao = "CEP Imp./Ex.", Propriedade = "CEPImpEx", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 86, Descricao = "Município Imp./Ex.", Propriedade = "MunicipioImpEx", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 87, Descricao = "UF Município Imp./Ex.", Propriedade = "UFMunicipioImpEx", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 88, Descricao = "N° DI", Propriedade = "NumeroDI", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 89, Descricao = "Cód. Importação DI", Propriedade = "CodImportacaoDI", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 90, Descricao = "Cód. Referência DI", Propriedade = "CodReferenciaDI", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 91, Descricao = "Valor Carga DI", Propriedade = "ValorCargaDI", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 92, Descricao = "Volume DI", Propriedade = "VolumeDI", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 93, Descricao = "Peso DI", Propriedade = "PesoDI", Tamanho = 100, Obrigatorio = false });

            return configuracoes;
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao ImportarCotacao(string dados, (string Nome, string Guid) arquivoGerador, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string StringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfigGeral = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configGeral = repConfigGeral.BuscarConfiguracaoPadrao();
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao()
            {
                Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
            };

            int contador = 0;

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    unitOfWork.FlushAndClear();
                    unitOfWork.Start();

                    // Processa linha do arquivo como uma cotacao isoladamente
                    Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = ImportarCotacaoLinha(linhas[i], arquivoGerador, usuario, tipoServicoMultisoftware, auditado, StringConexao, unitOfWork, configGeral);
                    retornoLinha.indice = i;
                    retornoImportacao.Retornolinhas.Add(retornoLinha);

                    // Deve contar como linha importada?
                    if (retornoLinha.contar)
                    {
                        contador++;
                    }

                    // Processou com sucesso?
                    if (retornoLinha.processou)
                        unitOfWork.CommitChanges();
                    else
                        unitOfWork.Rollback();
                }
                catch (Exception ex2)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex2);
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                    continue;
                }
            }

            retornoImportacao.MensagemAviso = "";
            retornoImportacao.Total = linhas.Count();
            retornoImportacao.Importados = contador;

            return retornoImportacao;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.CotacaoPedido.SolicitacaoCotacao InserirSolicitacaoCotacao(Dominio.ObjetosDeValor.WebService.Pedido.Cotacao cotacao, Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco enderecoEntrega, Dominio.Entidades.Cliente expedidor, Dominio.Entidades.Localidade destino, double km, List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtosEmbarcador, List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> produtosIntegracao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.CotacaoPedido.SolicitacaoCotacao repSolicitacaoCotacao = new Repositorio.Embarcador.CotacaoPedido.SolicitacaoCotacao(_unitOfWork);
            Repositorio.Embarcador.CotacaoPedido.SolicitacaoCotacaoProduto repSolicitacaoCotacaoProduto = new Repositorio.Embarcador.CotacaoPedido.SolicitacaoCotacaoProduto(_unitOfWork);
            Dominio.Entidades.Embarcador.CotacaoPedido.SolicitacaoCotacao solicitacaoCotacao = new Dominio.Entidades.Embarcador.CotacaoPedido.SolicitacaoCotacao();

            solicitacaoCotacao.Distancia = (int)km;
            solicitacaoCotacao.Remetente = expedidor;
            solicitacaoCotacao.Origem = expedidor.Localidade;
            solicitacaoCotacao.Destino = destino;
            solicitacaoCotacao.DataCriacao = DateTime.Now;
            if (cotacao.ValorTotalMercadoria > 0)
                solicitacaoCotacao.ValorTotalNotasFiscais = cotacao.ValorTotalMercadoria;

            solicitacaoCotacao.Bairro = enderecoEntrega.Bairro;
            solicitacaoCotacao.CEP = enderecoEntrega.CEP;
            solicitacaoCotacao.Endereco = enderecoEntrega.Logradouro;
            repSolicitacaoCotacao.Inserir(solicitacaoCotacao);

            foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto in produtosIntegracao)
            {
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = (from obj in produtosEmbarcador where obj.CodigoProdutoEmbarcador == produto.CodigoProduto select obj).FirstOrDefault();
                Dominio.Entidades.Embarcador.CotacaoPedido.SolicitacaoCotacaoProduto solicitacaoCotacaoProduto = new Dominio.Entidades.Embarcador.CotacaoPedido.SolicitacaoCotacaoProduto();
                solicitacaoCotacaoProduto.SolicitacaoCotacao = solicitacaoCotacao;
                solicitacaoCotacaoProduto.Produto = produtoEmbarcador;
                solicitacaoCotacaoProduto.AlturaCM = produtoEmbarcador.AlturaCM;
                solicitacaoCotacaoProduto.ComprimentoCM = produtoEmbarcador.ComprimentoCM;
                solicitacaoCotacaoProduto.LarguraCM = produtoEmbarcador.LarguraCM;
                solicitacaoCotacaoProduto.MetroCubico = produtoEmbarcador.MetroCubito;
                solicitacaoCotacaoProduto.PesoUnitario = produtoEmbarcador.PesoUnitario;
                solicitacaoCotacaoProduto.PrecoUnitario = produto.ValorUnitario;
                solicitacaoCotacaoProduto.Quantidade = produto.Quantidade;
                repSolicitacaoCotacaoProduto.Inserir(solicitacaoCotacaoProduto);
            }

            return solicitacaoCotacao;
        }

        private List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> InserirProdutos(List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> produtosIntegracao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (produtosIntegracao?.Count == 0)
                return null;

            WebService.Carga.ProdutosPedido servicoProdutosPedido = new Servicos.WebService.Carga.ProdutosPedido(_unitOfWork);
            Servicos.ProdutoEmbarcador servicoProdutoEmbarcador = new Servicos.ProdutoEmbarcador(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> gruposProduto = null;
            List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao> linhasSeparacao = null;
            List<Dominio.Entidades.Embarcador.Produtos.EnderecoProduto> enderecosProdutos = null;
            List<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem> tiposEmbalagem = null;
            List<Dominio.Entidades.Embarcador.Produtos.MarcaProduto> marcaProdutos = null;
            string retorno = servicoProdutosPedido.AjustarBasesParaProdutos(produtosIntegracao, ref gruposProduto, ref linhasSeparacao, ref enderecosProdutos, ref tiposEmbalagem, ref marcaProdutos, null, null, _unitOfWork);

            if (!string.IsNullOrWhiteSpace(retorno))
                throw new ServicoException(retorno);

            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtosExistentes = repProdutoEmbarcador.buscarPorCodigosEmbarcador((from obj in produtosIntegracao select obj.CodigoProduto).Distinct().ToList());
            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtosEmbarcado = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtocargaIntegracao in produtosIntegracao)
            {
                Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = null;
                Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao linhaSeparacao = null;
                Dominio.Entidades.Embarcador.Produtos.EnderecoProduto enderecoProduto = null;
                Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem tipoEmbalagem = null;
                Dominio.Entidades.Embarcador.Produtos.MarcaProduto marcaProduto = null;
                servicoProdutosPedido.ObterBasesParaCadastroProduto(produtocargaIntegracao, gruposProduto, linhasSeparacao, enderecosProdutos, tiposEmbalagem, marcaProdutos, ref grupoProduto, ref linhaSeparacao, ref enderecoProduto, ref tipoEmbalagem, ref marcaProduto);
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = servicoProdutoEmbarcador.IntegrarProduto(produtosExistentes, configuracao, produtocargaIntegracao.CodigoProduto, produtocargaIntegracao.DescricaoProduto, produtocargaIntegracao.PesoUnitario, grupoProduto, produtocargaIntegracao.MetroCubito, null, produtocargaIntegracao.CodigoDocumentacao, produtocargaIntegracao.Atualizar, produtocargaIntegracao.CodigoNCM, produtocargaIntegracao.QuantidadePorCaixa, produtocargaIntegracao.QuantidadeCaixaPorPallet, produtocargaIntegracao.Altura, produtocargaIntegracao.Largura, produtocargaIntegracao.Comprimento, linhaSeparacao, tipoEmbalagem, marcaProduto, produtocargaIntegracao.UnidadeMedida, produtocargaIntegracao.Observacao, "", produtocargaIntegracao?.CodigoEAN ?? string.Empty);
                if (produto != null)
                    produtosEmbarcado.Add(produto);
            }

            return produtosEmbarcado;
        }

        private void InserirCotacaoPedido(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo, ref Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao retornoCotacao, Dominio.Entidades.Embarcador.CotacaoPedido.SolicitacaoCotacao solicitacaoCotacao, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Localidade destino, Dominio.Entidades.Cliente expedidor, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(_unitOfWork);

            Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido = new Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido();
            cotacaoPedido.CubagemTotal = parametrosCalculo.Cubagem;
            cotacaoPedido.DataCriacao = DateTime.Now;
            cotacaoPedido.Destino = destino;
            cotacaoPedido.SolicitacaoCotacao = solicitacaoCotacao;
            cotacaoPedido.Empresa = empresa;
            cotacaoPedido.Origem = expedidor.Localidade;
            cotacaoPedido.PesoTotal = parametrosCalculo.Peso;
            cotacaoPedido.Previsao = retornoCotacao.DataPrazoEntrega.ToNullableDateTime();
            cotacaoPedido.DataColetaPrevista = retornoCotacao.DataPrevisaoColeta.ToNullableDateTime();
            cotacaoPedido.QtVolumes = (int)parametrosCalculo.Volumes;
            cotacaoPedido.TipoOperacao = tipoOperacao;
            cotacaoPedido.LeadTimeEntrega = retornoCotacao.PrazoEntrega;
            cotacaoPedido.ValorCotacao = retornoCotacao.ValorCotacao.ValorTotalCotacao;
            cotacaoPedido.ValorFrete = retornoCotacao.ValorCotacao.Valor;
            cotacaoPedido.ValorTotalNotasFiscais = solicitacaoCotacao.ValorTotalNotasFiscais;
            cotacaoPedido.KMTotal = (int)retornoCotacao.DistanciaRaioKM;
            cotacaoPedido.ModeloVeicularCarga = parametrosCalculo.ModeloVeiculo;
            retornoCotacao.ProtocoloCotacao = repCotacaoPedido.Inserir(cotacaoPedido);
            InserirComponentesCotacao(cotacaoPedido, retornoCotacao.ValorCotacao.Componentes);
        }

        private decimal ObterMedidaComPorcentagemCorrecao(decimal medida, decimal porcentagemCorrecao)
        {
            if (porcentagemCorrecao == 0)
                return medida;

            return medida + (medida * porcentagemCorrecao / 100);
        }

        private void InserirComponentesCotacao(Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido, List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteValoresComponente> componentes)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoComponente repCotacaoPedidoComponente = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoComponente(_unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(_unitOfWork);

            foreach (Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteValoresComponente componente in componentes)
            {
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = !string.IsNullOrWhiteSpace(componente.Descricao) ? repComponenteFrete.BuscarPorDescricao(componente.Descricao) : null;
                Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente cotacaoPedidoComponente = new Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente();
                cotacaoPedidoComponente.CotacaoPedido = cotacaoPedido;
                cotacaoPedidoComponente.ComponenteFrete = componenteFrete;
                cotacaoPedidoComponente.Valor = componente.Valor;
                cotacaoPedidoComponente.ValorTotal = componente.Valor;
                repCotacaoPedidoComponente.Inserir(cotacaoPedidoComponente);
            }
        }

        private void InserirComponentesCotacao(Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido, Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componente)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoComponente repCotacaoPedidoComponente = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoComponente(_unitOfWork);

            Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente cotacaoPedidoComponente = new Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente();
            cotacaoPedidoComponente.CotacaoPedido = cotacaoPedido;
            cotacaoPedidoComponente.ComponenteFrete = componente.ComponenteFrete;
            cotacaoPedidoComponente.Valor = componente.ValorComponente;
            cotacaoPedidoComponente.ValorTotal = componente.ValorComponente;
            repCotacaoPedidoComponente.Inserir(cotacaoPedidoComponente);
        }

        private void ObterLeadTimeComFrequenciaEntrega(DateTime dataBase, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana> diasSemana, ref int leadTime)
        {
            DateTime previsao = dataBase.AddDays(leadTime);
            if (previsao.DayOfWeek == DayOfWeek.Sunday)
            {
                previsao = previsao.AddDays(1);
                leadTime++;
            }
            else if (previsao.DayOfWeek == DayOfWeek.Saturday)
            {
                previsao = previsao.AddDays(2);
                leadTime = leadTime + 2;
            }

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana = Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaHelper.ObterDiaSemana(previsao);
            if (!diasSemana.Contains(diaSemana))
            {
                leadTime++;
                ObterLeadTimeComFrequenciaEntrega(dataBase, diasSemana, ref leadTime);
            }
        }

        private void ObterDataBaseFrequenciaCarregamento(ref DateTime dataBase, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana> diasSemana)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana = Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaHelper.ObterDiaSemana(dataBase);
            if (!diasSemana.Contains(diaSemana))
            {
                dataBase = dataBase.AddDays(1);
                ObterDataBaseFrequenciaCarregamento(ref dataBase, diasSemana);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete ObterParametrosCalculoFrete(Dominio.ObjetosDeValor.WebService.Pedido.Cotacao cotacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Localidade destino, Dominio.Entidades.Cliente expedidor, Dominio.Entidades.Cliente destinatario)
        {
            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete();
            parametrosCalculoFrete.Remetentes = new List<Dominio.Entidades.Cliente>();
            parametrosCalculoFrete.Remetentes.Add(expedidor);
            parametrosCalculoFrete.Origens = new List<Dominio.Entidades.Localidade>();
            parametrosCalculoFrete.Origens.Add(expedidor.Localidade);

            parametrosCalculoFrete.Destinos = new List<Dominio.Entidades.Localidade>();
            parametrosCalculoFrete.Destinos.Add(destino);
            parametrosCalculoFrete.RementesEDestinatariosOpcionaisQuandoExistirLocalidade = true;
            parametrosCalculoFrete.NaoValidarTransportador = true;
            parametrosCalculoFrete.CEPsDestinatarios = new List<int>();
            parametrosCalculoFrete.CEPsDestinatarios.Add(int.Parse(Utilidades.String.OnlyNumbers(cotacao.EnderecoDestino?.CEP != string.Empty ? cotacao.EnderecoDestino.CEP : destinatario.CEP)));
            parametrosCalculoFrete.TipoOperacao = tipoOperacao;
            parametrosCalculoFrete.DataVigencia = DateTime.Now;
            parametrosCalculoFrete.ValorNotasFiscais = cotacao.ValorTotalMercadoria;

            if (cotacao.Produtos != null)
            {
                parametrosCalculoFrete.Peso = (from obj in cotacao.Produtos select (obj.PesoUnitario * obj.Quantidade)).Sum();

                if (parametrosCalculoFrete.ValorNotasFiscais == 0)
                    parametrosCalculoFrete.ValorNotasFiscais = (from obj in cotacao.Produtos select (obj.ValorUnitario * obj.Quantidade)).Sum();

                parametrosCalculoFrete.Volumes = (from obj in cotacao.Produtos select obj.Quantidade).Sum();
            }

            parametrosCalculoFrete.Quantidades = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFreteQuantidade>();
            if (parametrosCalculoFrete.Peso > 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFreteQuantidade peso = new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFreteQuantidade();
                peso.Quantidade = parametrosCalculoFrete.Peso;
                peso.UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.KG;
                parametrosCalculoFrete.Quantidades.Add(peso);
            }

            if (parametrosCalculoFrete.Volumes > 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFreteQuantidade volume = new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFreteQuantidade();
                volume.Quantidade = parametrosCalculoFrete.Volumes;
                volume.UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.UN;
                parametrosCalculoFrete.Quantidades.Add(volume);
            }

            return parametrosCalculoFrete;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha ImportarCotacaoLinha(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, (string Nome, string Guid) arquivoGerador, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string StringConexao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configGeral)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);
            Repositorio.Embarcador.CRM.ClienteProspect repClienteProspect = new Repositorio.Embarcador.CRM.ClienteProspect(unitOfWork);
            Repositorio.Embarcador.CRM.Prospeccao repProspeccao = new Repositorio.Embarcador.CRM.Prospeccao(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repCompFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacao = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinhaPedido = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha();

            try
            {
                #region 1ª aba

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataPrevisaoServico = (from obj in linha.Colunas where obj.NomeCampo == "PrevisaoServico" select obj).FirstOrDefault();
                DateTime? dataPrevisaoServico = null;
                string dataPrevisaoServicoCol;
                if (colDataPrevisaoServico != null)
                {
                    dataPrevisaoServicoCol = colDataPrevisaoServico.Valor;
                    double.TryParse(dataPrevisaoServicoCol, out double dataPrevisaoServicoFormatoExcel);

                    if (dataPrevisaoServicoFormatoExcel > 0)
                        dataPrevisaoServico = DateTime.FromOADate(dataPrevisaoServicoFormatoExcel);
                    else
                        dataPrevisaoServico = dataPrevisaoServicoCol.ToDateTime();

                    if (dataPrevisaoServico == DateTime.MinValue)
                        return RetornarFalhaLinha("Data de Previsão de Serviço inválda");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoCliente = (from obj in linha.Colunas where obj.NomeCampo == "TipoCliente" select obj).FirstOrDefault();

                TipoClienteCotacaoPedido tipoCliente = TipoClienteCotacaoPedido.ClienteAtivo;

                if (colTipoCliente != null)
                {
                    string tipo = Utilidades.String.RemoveAccents((string)colTipoCliente.Valor.Trim());

                    switch (tipo.ToUpper())
                    {
                        case "CLIENTE ATIVO":
                            tipoCliente = TipoClienteCotacaoPedido.ClienteAtivo;
                            break;
                        case "CLIENTE INATIVO":
                            tipoCliente = TipoClienteCotacaoPedido.ClienteInativo;
                            break;
                        case "CLIENTE NOVO":
                            tipoCliente = TipoClienteCotacaoPedido.ClienteNovo;
                            break;
                        case "CLIENTE PROSPECT":
                            tipoCliente = TipoClienteCotacaoPedido.ClienteProspect;
                            break;
                        case "GRUPO DE PESSOA":
                            tipoCliente = TipoClienteCotacaoPedido.GrupoPessoa;
                            break;

                        default:
                            tipoCliente = TipoClienteCotacaoPedido.ClienteAtivo;
                            break;
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colClienteOuGrupoPessoa = (from obj in linha.Colunas where obj.NomeCampo == "ClienteOuGupoPessoa" select obj).FirstOrDefault();
                Dominio.Entidades.Cliente clienteAtivo = null;
                Dominio.Entidades.Cliente clienteInativo = null;
                Dominio.Entidades.Embarcador.CRM.ClienteProspect clienteProspect = null;
                string novoCliente = null;
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = null;
                if (colClienteOuGrupoPessoa != null)
                {
                    switch (tipoCliente)
                    {
                        case TipoClienteCotacaoPedido.ClienteAtivo:
                            double cpfCNPJClienteAtivo = Utilidades.String.OnlyNumbers((string)colClienteOuGrupoPessoa.Valor).ToDouble();
                            if (cpfCNPJClienteAtivo > 0d)
                            {
                                clienteAtivo = repCliente.BuscarPorCPFCNPJComStatus(cpfCNPJClienteAtivo, true);
                                if (clienteAtivo == null)
                                    return RetornarFalhaLinha("O Cliente Ativo informado não está cadastrado no sistema");
                            }
                            break;

                        case TipoClienteCotacaoPedido.ClienteInativo:
                            double cpfCNPJClienteInativo = Utilidades.String.OnlyNumbers((string)colClienteOuGrupoPessoa.Valor).ToDouble();
                            if (cpfCNPJClienteInativo > 0d)
                            {
                                clienteInativo = repCliente.BuscarPorCPFCNPJComStatus(cpfCNPJClienteInativo, false);
                                if (clienteInativo == null)
                                    return RetornarFalhaLinha("O Cliente Inativo informado não está cadastrado no sistema ou está Ativo");
                            }
                            break;

                        case TipoClienteCotacaoPedido.ClienteProspect:
                            string nomeClienteProspect = (string)colClienteOuGrupoPessoa.Valor;
                            if (!string.IsNullOrEmpty(nomeClienteProspect))
                            {
                                clienteProspect = repClienteProspect.BuscarPorNome(nomeClienteProspect);
                                if (clienteProspect == null)
                                    return RetornarFalhaLinha("O Cliente Prospect informado não está cadastrado no sistema");
                            }
                            break;

                        case TipoClienteCotacaoPedido.GrupoPessoa:
                            string descGrupoPessoas = (string)colClienteOuGrupoPessoa.Valor;
                            if (!string.IsNullOrEmpty(descGrupoPessoas))
                            {
                                grupoPessoas = repGrupoPessoas.BuscarPorDescricao(descGrupoPessoas);
                                if (grupoPessoas == null)
                                    return RetornarFalhaLinha("O Grupo de Pessoas informado não está cadastrado no sistema");
                            }
                            break;

                        case TipoClienteCotacaoPedido.ClienteNovo:
                            novoCliente = (string)colClienteOuGrupoPessoa.Valor;
                            break;
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colOrigem = (from obj in linha.Colunas where obj.NomeCampo == "Origem" select obj).FirstOrDefault();
                string origemString = "";
                Dominio.Entidades.Localidade Origem = null;
                if (colOrigem != null)
                {
                    origemString = colOrigem.Valor;

                    string ufOrigemString = "";

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colUFOrigem = (from obj in linha.Colunas where obj.NomeCampo == "UFOrigem" select obj).FirstOrDefault();
                    if (colUFOrigem != null)
                        ufOrigemString = colUFOrigem.Valor;

                    Origem = repositorioLocalidade.BuscarPorUFDescricao(ufOrigemString, origemString).FirstOrDefault();

                    if (Origem == null)
                        return RetornarFalhaLinha("A Origem informada não está cadastrada no sistema");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDestino = (from obj in linha.Colunas where obj.NomeCampo == "Destino" select obj).FirstOrDefault();
                string destinoString = "";
                Dominio.Entidades.Localidade Destino = null;
                if (colDestino != null)
                {
                    destinoString = colDestino.Valor;

                    string ufDestinoString = "";

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colUFDestino = (from obj in linha.Colunas where obj.NomeCampo == "UFDestino" select obj).FirstOrDefault();
                    if (colUFDestino != null)
                        ufDestinoString = colUFDestino.Valor;

                    Destino = repositorioLocalidade.BuscarPorUFDescricao(ufDestinoString, destinoString).FirstOrDefault();

                    if (Destino == null)
                        return RetornarFalhaLinha("O Destino informado não está cadastrada no sistema");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDestinatario = (from obj in linha.Colunas where obj.NomeCampo == "CNPJCPFDestinatario" select obj).FirstOrDefault();
                Dominio.Entidades.Cliente destinatario = null;
                if (colDestinatario != null && colDestinatario.Valor != null)
                {
                    double cpfCNPJDestinatario = Utilidades.String.OnlyNumbers((string)colDestinatario.Valor).ToDouble();
                    if (cpfCNPJDestinatario > 0d)
                    {
                        destinatario = repCliente.BuscarPorCPFCNPJ(cpfCNPJDestinatario);
                        if (destinatario == null)
                            return RetornarFalhaLinha("O Destinatário informado não está cadastrado no sistema");
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoCarga = (from obj in linha.Colunas where obj.NomeCampo == "TipoCarga" select obj).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = null;
                if (colTipoCarga != null)
                {
                    tipoCarga = repTipoDeCarga.BuscarPorCodigoEmbarcador((string)colTipoCarga.Valor);

                    if (tipoCarga == null)
                        tipoCarga = repTipoDeCarga.BuscarPorDescricao((string)colTipoCarga.Valor, true);

                    if (tipoCarga == null) return RetornarFalhaLinha("O tipo de Carga informado não está cadastrado no sistema");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoOperacao = (from obj in linha.Colunas where obj.NomeCampo == "TipoOperacao" select obj).FirstOrDefault();
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
                if (colTipoOperacao != null)
                {
                    tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao((string)colTipoOperacao.Valor);

                    if (tipoOperacao == null)
                        tipoOperacao = repTipoOperacao.BuscarPorDescricao((string)colTipoOperacao.Valor);

                    if (tipoOperacao == null) return RetornarFalhaLinha("O tipo de operação informado está cadastrado no sistema");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSolicitante = (from obj in linha.Colunas where obj.NomeCampo == "Solicitante" select obj).FirstOrDefault();
                string solicitante = null;
                if (colSolicitante != null)
                {
                    solicitante = (string)colSolicitante.Valor;
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colProdutoEmbarcador = (from obj in linha.Colunas where obj.NomeCampo == "CodigoProdutoEmbarcador" select obj).FirstOrDefault();
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = null;
                if (colProdutoEmbarcador != null)
                {
                    produtoEmbarcador = repProdutoEmbarcador.buscarPorCodigoEmbarcador(colProdutoEmbarcador.Valor);

                    if (produtoEmbarcador == null)
                        RetornarFalhaLinha("o Produto Embarcador informado não está cadastrado no sistema");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModeloVeicular = (from obj in linha.Colunas where obj.NomeCampo == "ModeloVeicular" select obj).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = null;
                if (colModeloVeicular != null)
                {
                    modeloVeicular = repModeloVeicularCarga.buscarPorCodigoIntegracao((string)colModeloVeicular.Valor);

                    if (modeloVeicular == null)
                        modeloVeicular = repModeloVeicularCarga.buscarPorDescricao((string)colModeloVeicular.Valor);

                    if (modeloVeicular == null)
                        return RetornarFalhaLinha("O Modelo Veicular informado não está cadastrado no sistema");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colProspeccao = (from obj in linha.Colunas where obj.NomeCampo == "NomeProspeccao" select obj).FirstOrDefault();
                Dominio.Entidades.Embarcador.CRM.Prospeccao prospeccao = null;
                if (colProspeccao != null)
                {
                    prospeccao = repProspeccao.BuscarPorNome(colProspeccao.Valor);

                    if (prospeccao == null)
                        return RetornarFalhaLinha("A Prospecção informada não está cadastrada no sistema");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colmailContato = (from obj in linha.Colunas where obj.NomeCampo == "EmailContato" select obj).FirstOrDefault();
                string emailContato = null;
                if (colmailContato != null)
                {
                    emailContato = colmailContato.Valor;
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTelefoneContato = (from obj in linha.Colunas where obj.NomeCampo == "TelefoneContato" select obj).FirstOrDefault();
                string telefone = null;
                if (colTelefoneContato != null)
                {
                    telefone = colTelefoneContato.Valor;
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colStatus = (from obj in linha.Colunas where obj.NomeCampo == "Status" select obj).FirstOrDefault();
                StatusCotacaoPedido status = StatusCotacaoPedido.EmAnalise;
                if (colStatus != null)
                {

                    switch (Utilidades.String.RemoveAccents((string)colStatus.Valor.Trim().ToUpper()))
                    {
                        case "FECHADA":
                            status = StatusCotacaoPedido.Fechada;
                            break;
                        case "PERDA POR PRECO":
                            status = StatusCotacaoPedido.PerdaPorPreco;
                            break;
                        case "PERDA POR DESISTENCIA DO SERVICO":
                            status = StatusCotacaoPedido.PerdaPorDesistenciaDoServico;
                            break;
                        case "PERDA POR NAO JUSTIFICATIVA PELO CLIENTE":
                            status = StatusCotacaoPedido.PerdaPorNaoJustificativaPeloCliente;
                            break;
                        case "PERDA POR PRAZO DE RESPOSTA":
                            status = StatusCotacaoPedido.PerdaPorPrazoDeResposta;
                            break;
                        case "PERTA POR QUALIFICACAO DOCUMENTAL":
                            status = StatusCotacaoPedido.PertaPorQualificacaoDocumental;
                            break;
                        case "PERDA POR QUALIFICACAO TECNICA":
                            status = StatusCotacaoPedido.PerdaPorQualificacaoTecnica;
                            break;
                        case "PERDA POR INFRAESTRUTURA":
                            status = StatusCotacaoPedido.PerdaPorInfraestrutura;
                            break;
                        case "PERDA POR ANALISE CADASTRAL":
                            status = StatusCotacaoPedido.PerdaPorAnaliseCadastral;
                            break;
                        case "EM ANALISE":
                            status = StatusCotacaoPedido.EmAnalise;
                            break;
                        case "SONDAGEM":
                            status = StatusCotacaoPedido.Sondagem;
                            break;

                        default:
                            status = StatusCotacaoPedido.EmAnalise;
                            break;
                    }
                }

                #endregion

                #region 2ª aba

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colMudarEnderecoOrigem = (from obj in linha.Colunas where obj.NomeCampo == "MudarEndereçoOrigem" select obj).FirstOrDefault();
                Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoEndereco cotacaoPedidoEnderecoOrigem = null;
                bool mudarEnderecoOrigem = false;
                if (colMudarEnderecoOrigem != null)
                {
                    var tmp = (string)colMudarEnderecoOrigem.Valor.Trim();
                    mudarEnderecoOrigem = (tmp.ToUpper()[0] == 'S' || tmp.ToUpper() == "TRUE" ? true : false);

                    if (mudarEnderecoOrigem)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCEPOutraLocalidadeOrigem = (from obj in linha.Colunas where obj.NomeCampo == "CEPOutraLocalidadeOrigem" select obj).FirstOrDefault();
                        Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco OutraLocalidadeOrigem = null;
                        if (colCEPOutraLocalidadeOrigem != null)
                        {
                            string cepOutraLocalidadeOrigem = (string)colCEPOutraLocalidadeOrigem.Valor;

                            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroOutraLocalidadeOrigem = (from obj in linha.Colunas where obj.NomeCampo == "NumeroOutraLocalidadeOrigem" select obj).FirstOrDefault();
                            string NumeroOutraLocalidadeOrigem = string.Empty;
                            if (colNumeroOutraLocalidadeOrigem != null)
                            {
                                NumeroOutraLocalidadeOrigem = colNumeroOutraLocalidadeOrigem.Valor;
                            }

                            OutraLocalidadeOrigem = repOutroEndereco.BuscarPorCEPNumeroLocalidade(cepOutraLocalidadeOrigem, NumeroOutraLocalidadeOrigem, 0, clienteAtivo?.CPF_CNPJ ?? clienteInativo?.CPF_CNPJ ?? 0d);

                            if (OutraLocalidadeOrigem == null)
                                return RetornarFalhaLinha("Outra Localidade da Origem não encontrada através do CEP e Número informado");
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLocalidadeOrigem = (from obj in linha.Colunas where obj.NomeCampo == "LocalidadeOrigem" select obj).FirstOrDefault();
                        Dominio.Entidades.Localidade LocalidadeOrigem = null;
                        if (colLocalidadeOrigem != null)
                        {
                            string locOrigem = colLocalidadeOrigem.Valor;

                            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colUFLocalidadeOrigem = (from obj in linha.Colunas where obj.NomeCampo == "UFLocalidadeOrigem" select obj).FirstOrDefault();
                            string UFLocOrigem = string.Empty;
                            if (colUFLocalidadeOrigem != null)
                            {
                                UFLocOrigem = (string)colUFLocalidadeOrigem.Valor;
                            }

                            LocalidadeOrigem = repositorioLocalidade.BuscarPorCidadeUF(locOrigem, UFLocOrigem);

                            if (LocalidadeOrigem == null)
                                return RetornarFalhaLinha("Localidade Substituta da Origem não está cadastrada no sistema");
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLocalidadePolo = (from obj in linha.Colunas where obj.NomeCampo == "LocalidadePoloOrigem" select obj).FirstOrDefault();
                        string localidadePoloOrigem = null;
                        if (colLocalidadePolo != null)
                        {
                            localidadePoloOrigem = (string)colLocalidadePolo.Valor;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEnderecoOrigem = (from obj in linha.Colunas where obj.NomeCampo == "EnderecoOrigem" select obj).FirstOrDefault();
                        string enderecoOrigem = null;
                        if (colEnderecoOrigem != null)
                        {
                            enderecoOrigem = (string)colEnderecoOrigem.Valor;

                            if (string.IsNullOrEmpty(enderecoOrigem))
                                return RetornarFalhaLinha("O Endereço de Origem deve ser preenchido");
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroOrigem = (from obj in linha.Colunas where obj.NomeCampo == "NumeroOrigem" select obj).FirstOrDefault();
                        string numeroOrigem = null;
                        if (colNumeroOrigem != null)
                        {
                            numeroOrigem = (string)colNumeroOrigem.Valor;

                            if (string.IsNullOrEmpty(numeroOrigem))
                                return RetornarFalhaLinha("O Número do Endereço de Origem deve ser preenchido");
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colComplementoOrigem = (from obj in linha.Colunas where obj.NomeCampo == "ComplementoOrigem" select obj).FirstOrDefault();
                        string complementoOrigem = null;
                        if (colComplementoOrigem != null)
                        {
                            complementoOrigem = (string)colComplementoOrigem.Valor;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colBairroOrigem = (from obj in linha.Colunas where obj.NomeCampo == "BairroOrigem" select obj).FirstOrDefault();
                        string bairroOrigem = null;
                        if (colBairroOrigem != null)
                        {
                            bairroOrigem = (string)colBairroOrigem.Valor;

                            if (string.IsNullOrEmpty(bairroOrigem))
                                return RetornarFalhaLinha("O Bairro do Endereço de Origem deve ser preenchido");
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCEPOrigem = (from obj in linha.Colunas where obj.NomeCampo == "CEPOrigem" select obj).FirstOrDefault();
                        string cepOrigem = null;
                        if (colCEPOrigem != null)
                        {
                            cepOrigem = (string)colCEPOrigem.Valor;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTelefoneOrigem = (from obj in linha.Colunas where obj.NomeCampo == "TelefoneOrigem" select obj).FirstOrDefault();
                        string telefoneOrigem = null;
                        if (colTelefoneOrigem != null)
                        {
                            telefoneOrigem = (string)colTelefoneOrigem.Valor;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colIE = (from obj in linha.Colunas where obj.NomeCampo == "IEOrigem" select obj).FirstOrDefault();
                        string ieOrigem = null;
                        if (colIE != null)
                        {
                            ieOrigem = (string)colIE.Valor;
                        }

                        cotacaoPedidoEnderecoOrigem = new Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoEndereco()
                        {
                            Localidade = LocalidadeOrigem,
                            ClienteOutroEndereco = OutraLocalidadeOrigem,
                            Bairro = bairroOrigem,
                            CEP = cepOrigem,
                            Numero = numeroOrigem,
                            Complemento = complementoOrigem,
                            IE_RG = ieOrigem,
                            Endereco = enderecoOrigem,
                            Telefone = telefone
                        };
                    }
                }

                #endregion

                #region 3ª aba

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colMudarEnderecoDestino = (from obj in linha.Colunas where obj.NomeCampo == "MudarEndereçoDestino" select obj).FirstOrDefault();
                Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoEndereco cotacaoPedidoEnderecoDestino = null;
                bool mudarEnderecoDestino = false;
                if (colMudarEnderecoDestino != null)
                {
                    var tmp2 = (string)colMudarEnderecoDestino.Valor.Trim();
                    mudarEnderecoDestino = (tmp2.ToUpper()[0] == 'S' || tmp2.ToUpper() == "TRUE" ? true : false);

                    if (mudarEnderecoDestino)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCEPOutraLocalidadeDestino = (from obj in linha.Colunas where obj.NomeCampo == "CEPOutraLocalidadeDestino" select obj).FirstOrDefault();
                        Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco OutraLocalidadeDestino = null;
                        if (colCEPOutraLocalidadeDestino != null)
                        {
                            string cepOutraLocalidadeDestino = (string)colCEPOutraLocalidadeDestino.Valor;

                            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroOutraLocalidadeDestino = (from obj in linha.Colunas where obj.NomeCampo == "NumeroOutraLocalidadeDestino" select obj).FirstOrDefault();
                            string NumeroOutraLocalidadeDestino = string.Empty;
                            if (colNumeroOutraLocalidadeDestino != null)
                            {
                                NumeroOutraLocalidadeDestino = (string)colNumeroOutraLocalidadeDestino.Valor;
                            }

                            OutraLocalidadeDestino = repOutroEndereco.BuscarPorCEPNumeroLocalidade(cepOutraLocalidadeDestino, NumeroOutraLocalidadeDestino, 0, clienteAtivo?.CPF_CNPJ ?? clienteInativo?.CPF_CNPJ ?? 0d);

                            if (OutraLocalidadeDestino == null)
                                return RetornarFalhaLinha("Outra Localidade do Destino não encontrada através do CEP e Número informado");
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLocalidadeDestino = (from obj in linha.Colunas where obj.NomeCampo == "LocalidadeDestino" select obj).FirstOrDefault();
                        Dominio.Entidades.Localidade LocalidadeDestino = null;
                        if (colLocalidadeDestino != null)
                        {
                            string locDestino = (string)colLocalidadeDestino.Valor;

                            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colUFLocalidadeDestino = (from obj in linha.Colunas where obj.NomeCampo == "UFLocalidadeDestino" select obj).FirstOrDefault();
                            string UFLocDestino = string.Empty;
                            if (colUFLocalidadeDestino != null)
                            {
                                UFLocDestino = (string)colUFLocalidadeDestino.Valor;
                            }

                            LocalidadeDestino = repositorioLocalidade.BuscarPorCidadeUF(locDestino, UFLocDestino);

                            if (LocalidadeDestino == null)
                                return RetornarFalhaLinha("Localidade Substituta do Destino não está cadastrada no sistema");
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLocalidadePolo = (from obj in linha.Colunas where obj.NomeCampo == "LocalidadePoloDestino" select obj).FirstOrDefault();
                        string localidadePoloDestino = null;
                        if (colLocalidadePolo != null)
                        {
                            localidadePoloDestino = (string)colLocalidadePolo.Valor;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEnderecoDestino = (from obj in linha.Colunas where obj.NomeCampo == "EnderecoDestino" select obj).FirstOrDefault();
                        string enderecoDestino = null;
                        if (colEnderecoDestino != null)
                        {
                            enderecoDestino = (string)colEnderecoDestino.Valor;

                            if (string.IsNullOrEmpty(enderecoDestino))
                                return RetornarFalhaLinha("O Endereço de Destino deve ser preenchido");
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroDestino = (from obj in linha.Colunas where obj.NomeCampo == "NumeroDestino" select obj).FirstOrDefault();
                        string numeroDestino = null;
                        if (colNumeroDestino != null)
                        {
                            numeroDestino = (string)colNumeroDestino.Valor;

                            if (string.IsNullOrEmpty(numeroDestino))
                                return RetornarFalhaLinha("O Número do Endereço de Destino deve ser preenchido");
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colComplementoDestino = (from obj in linha.Colunas where obj.NomeCampo == "ComplementoDestino" select obj).FirstOrDefault();
                        string complementoDestino = null;
                        if (colComplementoDestino != null)
                        {
                            complementoDestino = (string)colComplementoDestino.Valor;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colBairroDestino = (from obj in linha.Colunas where obj.NomeCampo == "BairroDestino" select obj).FirstOrDefault();
                        string bairroDestino = null;
                        if (colBairroDestino != null)
                        {
                            bairroDestino = (string)colBairroDestino.Valor;

                            if (string.IsNullOrEmpty(bairroDestino))
                                return RetornarFalhaLinha("O Bairro do Endereço de Origem deve ser preenchido");
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCEPDestino = (from obj in linha.Colunas where obj.NomeCampo == "CEPDestino" select obj).FirstOrDefault();
                        string cepDestino = null;
                        if (colCEPDestino != null)
                        {
                            cepDestino = (string)colCEPDestino.Valor;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTelefoneDestino = (from obj in linha.Colunas where obj.NomeCampo == "TelefoneDestino" select obj).FirstOrDefault();
                        string telefoneDestino = null;
                        if (colTelefoneDestino != null)
                        {
                            telefoneDestino = (string)colTelefoneDestino.Valor;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colIE = (from obj in linha.Colunas where obj.NomeCampo == "IEDestino" select obj).FirstOrDefault();
                        string ieDestino = null;
                        if (colIE != null)
                        {
                            ieDestino = (string)colIE.Valor;
                        }

                        cotacaoPedidoEnderecoDestino = new Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoEndereco()
                        {
                            Localidade = LocalidadeDestino,
                            ClienteOutroEndereco = OutraLocalidadeDestino,
                            Bairro = bairroDestino,
                            CEP = cepDestino,
                            Numero = numeroDestino,
                            Complemento = complementoDestino,
                            IE_RG = ieDestino,
                            Endereco = enderecoDestino,
                            Telefone = telefone
                        };
                    }
                }

                #endregion

                #region 4ª aba

                #region Dados Adicionais

                DateTime? dataPrevisaoSaida = null;
                string dataPrevisaoSaidaSTR;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataPrevisaoSaida = (from obj in linha.Colunas where obj.NomeCampo == "PrevisaoSaida" select obj).FirstOrDefault();
                if (colDataPrevisaoSaida != null)
                {
                    dataPrevisaoSaidaSTR = colDataPrevisaoSaida.Valor;
                    double.TryParse(dataPrevisaoSaidaSTR, out double dataPrevisaoSaidaFormatoExcel);

                    if (dataPrevisaoSaidaFormatoExcel > 0)
                        dataPrevisaoSaida = DateTime.FromOADate(dataPrevisaoSaidaFormatoExcel);
                    else
                        dataPrevisaoSaida = dataPrevisaoSaidaSTR.ToDateTime();

                    if (dataPrevisaoSaida == DateTime.MinValue)
                        return RetornarFalhaLinha("Data de Previsão de Serviço inválda");
                }

                DateTime? dataPrevisaoRetorno = null;
                string dataPrevisaoRetornoSTR;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataPrevisaoRetorno = (from obj in linha.Colunas where obj.NomeCampo == "PrevisaoSaida" select obj).FirstOrDefault();
                if (colDataPrevisaoRetorno != null)
                {
                    dataPrevisaoRetornoSTR = colDataPrevisaoRetorno.Valor;
                    double.TryParse(dataPrevisaoRetornoSTR, out double dataPrevisaoRetornoFormatoExcel);

                    if (dataPrevisaoRetornoFormatoExcel > 0)
                        dataPrevisaoRetorno = DateTime.FromOADate(dataPrevisaoRetornoFormatoExcel);
                    else
                        dataPrevisaoRetorno = dataPrevisaoRetornoSTR.ToDateTime();

                    if (dataPrevisaoRetorno == DateTime.MinValue)
                        return RetornarFalhaLinha("Data de Previsão de Serviço inválda");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoModal = (from obj in linha.Colunas where obj.NomeCampo == "TipoModal" select obj).FirstOrDefault();
                TipoModal tipoModal = TipoModal.Rodoviario;
                if (colStatus != null)
                {
                    switch (Utilidades.String.RemoveAccents((string)colStatus.Valor.Trim().ToUpper()))
                    {
                        case "RODOVIARIO":
                            tipoModal = TipoModal.Rodoviario;
                            break;
                        case "AEREO":
                            tipoModal = TipoModal.Aereo;
                            break;
                        case "AQUAVIARIO":
                            tipoModal = TipoModal.Aquaviario;
                            break;
                        case "FERROVIARIO":
                            tipoModal = TipoModal.Ferroviario;
                            break;
                        case "DUTOVIARIO":
                            tipoModal = TipoModal.Dutoviario;
                            break;
                        case "MULTIMODAL":
                            tipoModal = TipoModal.Multimodal;
                            break;

                        default:
                            tipoModal = TipoModal.Rodoviario;
                            break;
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNPallets = (from obj in linha.Colunas where obj.NomeCampo == "NPallets" select obj).FirstOrDefault();
                int nPallets = 0;
                if (colNPallets != null)
                {
                    string tmp = (string)colNPallets.Valor;
                    nPallets = tmp.ToInt();
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPesoBruto = (from obj in linha.Colunas where obj.NomeCampo == "PesoBruto" select obj).FirstOrDefault();
                decimal PesoBruto = 0m;
                if (colPesoBruto != null)
                {
                    string tmp = (string)colPesoBruto.Valor;
                    PesoBruto = tmp.ToDecimal();
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorMercadoria = (from obj in linha.Colunas where obj.NomeCampo == "ValorMercadoria" select obj).FirstOrDefault();
                decimal valorMercadoria = 0m;
                if (colValorMercadoria != null)
                {
                    valorMercadoria = Utilidades.Decimal.Converter((string)colValorMercadoria.Valor);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQtdNotas = (from obj in linha.Colunas where obj.NomeCampo == "QtdNotas" select obj).FirstOrDefault();
                int qtdNotas = 0;
                if (colQtdNotas != null)
                {
                    string tmp = (string)colQtdNotas.Valor;
                    qtdNotas = tmp.ToInt();
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQtdEntregas = (from obj in linha.Colunas where obj.NomeCampo == "QtdEntregas" select obj).FirstOrDefault();
                int qtdEntregas = 0;
                if (colQtdEntregas != null)
                {
                    string tmp = (string)colQtdEntregas.Valor;
                    qtdEntregas = tmp.ToInt();
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTemperatura = (from obj in linha.Colunas where obj.NomeCampo == "Temperatura" select obj).FirstOrDefault();
                string temperatura = null;
                if (colQtdEntregas != null)
                {
                    temperatura = (string)colQtdEntregas.Valor;
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colKMTotal = (from obj in linha.Colunas where obj.NomeCampo == "KMTotal" select obj).FirstOrDefault();
                int KMTotal = RetornarKMTotal(Origem != null ? Origem : cotacaoPedidoEnderecoOrigem.Localidade, Destino != null ? Destino : cotacaoPedidoEnderecoDestino?.Localidade, repRotaFrete);
                if (colKMTotal != null)
                {
                    string tmp = (string)colKMTotal.Valor;
                    KMTotal = tmp.ToInt();
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorKM = (from obj in linha.Colunas where obj.NomeCampo == "ValorKM" select obj).FirstOrDefault();
                decimal valorKM = 0m;
                if (colValorMercadoria != null)
                {
                    valorKM = Utilidades.Decimal.Converter((string)colValorKM.Valor);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colObservacao = (from obj in linha.Colunas where obj.NomeCampo == "Observacao" select obj).FirstOrDefault();
                string observacao = null;
                if (colObservacao != null)
                {
                    observacao = (string)colObservacao.Valor;
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSeraRastreado = (from obj in linha.Colunas where obj.NomeCampo == "SeraRastreado" select obj).FirstOrDefault();
                bool seraRastreado = false;
                if (colSeraRastreado != null)
                {
                    var tmp = (string)colSeraRastreado.Valor.Trim();
                    seraRastreado = (tmp.ToUpper()[0] == 'S' || tmp.ToUpper() == "TRUE" ? true : false);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNecessarioEscolta = (from obj in linha.Colunas where obj.NomeCampo == "NecessarioEscolta" select obj).FirstOrDefault();
                bool NecessarioEscolta = false;
                int qtdEscolta = 0;
                if (colNecessarioEscolta != null)
                {
                    var tmp = (string)colNecessarioEscolta.Valor.Trim();
                    NecessarioEscolta = (tmp.ToUpper()[0] == 'S' || tmp.ToUpper() == "TRUE" ? true : false);

                    if (NecessarioEscolta)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQtdEscolta = (from obj in linha.Colunas where obj.NomeCampo == "QtdEscolta" select obj).FirstOrDefault();
                        if (colQtdEscolta != null)
                        {
                            string tmp2 = (string)colQtdEscolta.Valor;
                            qtdEscolta = tmp2.ToInt();
                        }
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTeraGerenciamentoRisco = (from obj in linha.Colunas where obj.NomeCampo == "TeraGerenciamentoRisco" select obj).FirstOrDefault();
                bool teraGerenciamentoRisco = false;
                if (colSeraRastreado != null)
                {
                    var tmp = (string)colTeraGerenciamentoRisco.Valor.Trim();
                    teraGerenciamentoRisco = (tmp.ToUpper()[0] == 'S' || tmp.ToUpper() == "TRUE" ? true : false);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAjudante = (from obj in linha.Colunas where obj.NomeCampo == "Ajudante" select obj).FirstOrDefault();
                bool ajudante = false;
                int qtdAjudantes = 0;
                if (colAjudante != null)
                {
                    var tmp = (string)colAjudante.Valor.Trim();
                    ajudante = (tmp.ToUpper()[0] == 'S' || tmp.ToUpper() == "TRUE" ? true : false);

                    if (ajudante)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQtdAjudantes = (from obj in linha.Colunas where obj.NomeCampo == "QtdAjudantes" select obj).FirstOrDefault();
                        if (colQtdAjudantes != null)
                        {
                            string tmp2 = (string)colQtdAjudantes.Valor;
                            qtdAjudantes = tmp2.ToInt();
                        }
                    }
                }

                #endregion

                #region Cálculo de Cubagem

                List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoCubagem> calculosCubagem = new List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoCubagem>();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAltura = (from obj in linha.Colunas where obj.NomeCampo == "AlturaCubagem" select obj).FirstOrDefault();
                if (colAltura != null)
                {
                    decimal altura = Utilidades.Decimal.Converter((string)colAltura.Valor);


                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colComprimento = (from obj in linha.Colunas where obj.NomeCampo == "ComprimentoCubagem" select obj).FirstOrDefault();
                    decimal comprimento = 0m;
                    if (colComprimento != null)
                    {
                        comprimento = Utilidades.Decimal.Converter((string)colComprimento.Valor);
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLargura = (from obj in linha.Colunas where obj.NomeCampo == "LarguraCubagem" select obj).FirstOrDefault();
                    decimal largura = 0m;
                    if (colLargura != null)
                    {
                        largura = Utilidades.Decimal.Converter((string)colLargura.Valor);
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQtdVolume = (from obj in linha.Colunas where obj.NomeCampo == "QtdVolumeCubagem" select obj).FirstOrDefault();
                    int qtdVolume = 0;
                    if (colQtdVolume != null)
                    {
                        string tmp = (string)colQtdVolume.Valor;
                        qtdVolume = tmp.ToInt();
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFatorCubico = (from obj in linha.Colunas where obj.NomeCampo == "FatorCubicoCubagem" select obj).FirstOrDefault();
                    decimal fatorCubico = 0m;
                    if (colFatorCubico != null)
                    {
                        fatorCubico = Utilidades.Decimal.Converter((string)colFatorCubico.Valor);
                    }

                    decimal pesoCubado = 0m;
                    decimal mCubico = 0m;

                    if (altura > 0m && largura > 0m && comprimento > 0m && qtdVolume > 0)
                    {
                        mCubico = (altura * largura * comprimento * qtdVolume);

                        if (fatorCubico > 0)
                            pesoCubado = mCubico * fatorCubico;
                        else
                            pesoCubado = mCubico;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colMCubico = (from obj in linha.Colunas where obj.NomeCampo == "MCubicoCubagem" select obj).FirstOrDefault();
                    if (colMCubico != null)
                    {
                        mCubico = Utilidades.Decimal.Converter((string)colMCubico.Valor);
                    }

                    calculosCubagem.Add(new Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoCubagem
                    {
                        Altura = altura,
                        Comprimento = comprimento,
                        Largura = largura,
                        QtdVolume = qtdVolume,
                        MetroCubico = mCubico,
                        FatorCubico = fatorCubico,
                        PesoCubado = pesoCubado
                    });
                }

                decimal mCubicoTotal = 0m;
                decimal pesoCubadoTotal = 0m;
                int totalVolumes = 0;

                PreencherTotaisCubagem(ref mCubicoTotal, ref pesoCubadoTotal, ref totalVolumes, calculosCubagem);

                #endregion

                #endregion

                #region 5ª aba

                List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente> componentes = new List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente>();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colComponenteFrete = (from obj in linha.Colunas where obj.NomeCampo == "ComponenteFrete" select obj).FirstOrDefault();
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete;
                if (colComponenteFrete != null)
                {
                    decimal valor = 0m;
                    decimal acrescimoComp = 0m;
                    decimal descontoComp = 0m;
                    decimal totalComp = 0m;

                    string compFrete = (string)colComponenteFrete.Valor;

                    componenteFrete = repCompFrete.BuscarPorDescricao(compFrete) ?? throw new ControllerException("O Componente Frete informado não está cadastrado no sistema");

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValor = (from obj in linha.Colunas where obj.NomeCampo == "ValorCompFrete" select obj).FirstOrDefault();
                    if (colValor != null)
                    {
                        valor = Utilidades.Decimal.Converter((string)colValor.Valor);
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAcrescimoComp = (from obj in linha.Colunas where obj.NomeCampo == "AcrescimoCompFrete" select obj).FirstOrDefault();
                    if (colAcrescimoComp != null)
                    {
                        acrescimoComp = Utilidades.Decimal.Converter((string)colAcrescimoComp.Valor);
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDescontoComp = (from obj in linha.Colunas where obj.NomeCampo == "DescontoCompFrete" select obj).FirstOrDefault();
                    if (colDescontoComp != null)
                    {
                        descontoComp = Utilidades.Decimal.Converter((string)colDescontoComp.Valor);
                    }

                    if (valor > 0m || acrescimoComp > 0m || descontoComp > 0m)
                    {
                        if (acrescimoComp > 0m)
                            valor = valor + ((valor * acrescimoComp) / 100);
                        if (descontoComp > 0)
                            valor = valor - ((valor * descontoComp) / 100);

                        totalComp = valor;
                    }

                    componentes.Add(new Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente
                    {
                        Valor = valor,
                        PercentualAcrescimo = acrescimoComp,
                        PercentualDesconto = descontoComp,
                        ValorTotal = totalComp,
                        ComponenteFrete = componenteFrete
                    });
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colBuscarValoresTabelaFrete = (from obj in linha.Colunas where obj.NomeCampo == "BuscarValoresTabelaFrete" select obj).FirstOrDefault();
                bool buscarValoresTabelaFrete = false;
                decimal valorFrete = 0m;
                if (colBuscarValoresTabelaFrete != null)
                {
                    var tmp = (string)colTeraGerenciamentoRisco.Valor.Trim();
                    buscarValoresTabelaFrete = (tmp.ToUpper()[0] == 'S' || tmp.ToUpper() == "TRUE" ? true : false);

                    if (buscarValoresTabelaFrete)
                    {
                        PreencherComponentesEValorFrete(ref componentes, ref valorFrete, Servicos.Embarcador.Carga.Frete.CalcularFretePorCotacaoPedido(unitOfWork,
                                                    StringConexao, tipoServicoMultisoftware, dataPrevisaoServico, dataPrevisaoRetorno,
                                                    dataPrevisaoSaida, DateTime.Now, clienteAtivo?.CPF_CNPJ ?? 0d, clienteInativo?.CPF_CNPJ ?? 0d,
                                                    Destino.Codigo, 0, Origem.Codigo, grupoPessoas?.Codigo ?? 0, modeloVeicular?.Codigo ?? 0, KMTotal,
                                                    NecessarioEscolta, teraGerenciamentoRisco, qtdAjudantes, qtdEntregas, 0, 0, nPallets, 1, PesoBruto, pesoCubadoTotal,
                                                    qtdNotas, seraRastreado, destinatario?.CPF_CNPJ ?? 0, tipoCarga?.Codigo ?? 0, tipoOperacao?.Codigo ?? 0,
                                                    valorMercadoria, totalVolumes, PesoBruto, configGeral, false));
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAcrescimo = (from obj in linha.Colunas where obj.NomeCampo == "Acrescimo" select obj).FirstOrDefault();
                decimal acrescimo = 0m;
                if (colAcrescimo != null)
                {
                    acrescimo = Utilidades.Decimal.Converter((string)colAcrescimo.Valor);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDesconto = (from obj in linha.Colunas where obj.NomeCampo == "Desconto" select obj).FirstOrDefault();
                decimal desconto = 0m;
                if (colDesconto != null)
                {
                    desconto = Utilidades.Decimal.Converter((string)colDesconto.Valor);
                }

                decimal totalComponentes = RetornarValorTotalComponentes(componentes);
                decimal totalCotacao = RetornarTotalCotacao(valorFrete, totalComponentes, acrescimo, desconto);
                decimal totalCotacaoComICMS = totalCotacao;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colIncluirValorICMSBaseCalculo = (from obj in linha.Colunas where obj.NomeCampo == "IncluirValorICMSBaseCalculo" select obj).FirstOrDefault();
                bool incluirValorICMSBaseCalculo = false;
                if (colIncluirValorICMSBaseCalculo != null)
                {
                    var tmp = (string)colIncluirValorICMSBaseCalculo.Valor.Trim();
                    incluirValorICMSBaseCalculo = (tmp.ToUpper()[0] == 'S' || tmp.ToUpper() == "TRUE" ? true : false);
                }

                decimal aliqICMS = RetornarPercentualAliquota(clienteAtivo != null ? clienteAtivo : clienteInativo, destinatario, StringConexao, unitOfWork);
                decimal valorICMS = 0m;

                CalcularICMS(aliqICMS, ref valorICMS, totalCotacao, ref totalCotacaoComICMS, incluirValorICMSBaseCalculo);

                #endregion

                #region 7ª aba

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colBL = (from obj in linha.Colunas where obj.NomeCampo == "BL" select obj).FirstOrDefault();
                string BL = null;
                if (colBL != null)
                {
                    BL = (string)colBL.Valor;
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNavio = (from obj in linha.Colunas where obj.NomeCampo == "Navio" select obj).FirstOrDefault();
                string navio = null;
                if (colNavio != null)
                {
                    navio = (string)colNavio.Valor;
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPorto = (from obj in linha.Colunas where obj.NomeCampo == "Porto" select obj).FirstOrDefault();
                Dominio.Entidades.Cliente porto = null;
                if (colPorto != null && colPorto.Valor != null)
                {
                    double cpfCNPJPorto = Utilidades.String.OnlyNumbers((string)colPorto.Valor).ToDouble();
                    if (cpfCNPJPorto > 0d)
                    {
                        porto = repCliente.BuscarPorCPFCNPJ(cpfCNPJPorto);
                        if (porto == null)
                            return RetornarFalhaLinha("O Porto informado não está cadastrado no sistema");
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTerminalImportacao = (from obj in linha.Colunas where obj.NomeCampo == "TerminalImportacao" select obj).FirstOrDefault();
                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalImportacao = null;
                if (colTerminalImportacao != null)
                {
                    string descTerminal = (string)colTerminalImportacao.Valor;
                    if (!string.IsNullOrEmpty(descTerminal))
                    {
                        terminalImportacao = repTerminalImportacao.BuscarPorDescricao(descTerminal);
                        if (terminalImportacao == null)
                            return RetornarFalhaLinha("O Porto informado não está cadastrado no sistema");
                    }
                }

                DateTime? dataVencimentoArmazenamento = null;
                string dataVencimentoArmazenamentoCol;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataVencimentoArmazenamento = (from obj in linha.Colunas where obj.NomeCampo == "DataVencimentoArmazenamento" select obj).FirstOrDefault();
                if (colDataVencimentoArmazenamento != null)
                {
                    dataVencimentoArmazenamentoCol = (string)colDataVencimentoArmazenamento.Valor;
                    double.TryParse(dataVencimentoArmazenamentoCol, out double dataVencimentoFormatoExcel);

                    if (dataVencimentoFormatoExcel > 0)
                        dataVencimentoArmazenamento = DateTime.FromOADate(dataVencimentoFormatoExcel);
                    else
                        dataVencimentoArmazenamento = dataVencimentoArmazenamentoCol.ToDateTime();

                    if (dataVencimentoArmazenamento == DateTime.MinValue)
                        return RetornarFalhaLinha("Data de Vencimento do Armazenamento inválda");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colArmador = (from obj in linha.Colunas where obj.NomeCampo == "Armador" select obj).FirstOrDefault();
                string armador = null;
                if (colArmador != null)
                {
                    armador = (string)colArmador.Valor;
                }

                #region Endereço Entrega

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEnderecoImpEx = (from obj in linha.Colunas where obj.NomeCampo == "EnderecoImpEx" select obj).FirstOrDefault();
                string enderecoImpEx = null;
                if (colEnderecoImpEx != null)
                {
                    enderecoImpEx = (string)colEnderecoImpEx.Valor;
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colBairroImpEx = (from obj in linha.Colunas where obj.NomeCampo == "BairroImpEx" select obj).FirstOrDefault();
                string bairroImpEx = null;
                if (colBairroImpEx != null)
                {
                    bairroImpEx = (string)colBairroImpEx.Valor;
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCEPImpEx = (from obj in linha.Colunas where obj.NomeCampo == "CEPImpEx" select obj).FirstOrDefault();
                string CEPImpEx = null;
                if (colCEPImpEx != null)
                {
                    CEPImpEx = (string)colCEPImpEx.Valor;
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colMunicipioImpEx = (from obj in linha.Colunas where obj.NomeCampo == "MunicipioImpEx" select obj).FirstOrDefault();
                string municipioImpExSTR = "";
                Dominio.Entidades.Localidade municipioImpEx = null;
                if (colMunicipioImpEx != null)
                {
                    municipioImpExSTR = colMunicipioImpEx.Valor;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colUFMunicipio = (from obj in linha.Colunas where obj.NomeCampo == "UFMunicipioImpEx" select obj).FirstOrDefault();
                    string ufMunicipioImpExSTR = "";
                    if (colUFMunicipio != null)
                        ufMunicipioImpExSTR = colUFMunicipio.Valor;

                    municipioImpEx = repositorioLocalidade.BuscarPorUFDescricao(ufMunicipioImpExSTR, municipioImpExSTR).FirstOrDefault();

                    if (municipioImpEx == null)
                        return RetornarFalhaLinha("O Município informado não está cadastrado no sistema");
                }

                #endregion

                #region Dados de importação

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroDI = (from obj in linha.Colunas where obj.NomeCampo == "NumeroDI" select obj).FirstOrDefault();
                List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoImportacao> dadosImportacao = new List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoImportacao>();
                if (colNumeroDI != null)
                {
                    string numeroDI = null;
                    string codImportacaoDI = null;
                    string codReferenciaDI = null;
                    decimal valorCargaDI = 0m;
                    decimal volumeDI = 0m;
                    decimal pesoDI = 0m;

                    numeroDI = (string)colNumeroDI.Valor;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodImpDI = (from obj in linha.Colunas where obj.NomeCampo == "CodImportacaoDI" select obj).FirstOrDefault();
                    if (colCodImpDI != null)
                    {
                        codImportacaoDI = (string)colCodImpDI.Valor;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodRefDI = (from obj in linha.Colunas where obj.NomeCampo == "CodReferenciaDI" select obj).FirstOrDefault();
                    if (colCodRefDI != null)
                    {
                        codReferenciaDI = (string)colCodRefDI.Valor;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorCargaDI = (from obj in linha.Colunas where obj.NomeCampo == "ValorCargaDI" select obj).FirstOrDefault();
                    if (colValorCargaDI != null)
                    {
                        valorCargaDI = Utilidades.Decimal.Converter((string)colValorCargaDI.Valor);
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colVolDI = (from obj in linha.Colunas where obj.NomeCampo == "VolumeDI" select obj).FirstOrDefault();
                    if (colVolDI != null)
                    {
                        volumeDI = Utilidades.Decimal.Converter((string)colVolDI.Valor);
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPesoDI = (from obj in linha.Colunas where obj.NomeCampo == "PesoDI" select obj).FirstOrDefault();
                    if (colPesoDI != null)
                    {
                        pesoDI = Utilidades.Decimal.Converter((string)colPesoDI.Valor);
                    }

                    dadosImportacao.Add(new Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoImportacao
                    {
                        NumeroDI = numeroDI,
                        CodigoImportacao = codImportacaoDI,
                        CodigoReferencia = codReferenciaDI,
                        ValorCarga = valorCargaDI,
                        Volume = volumeDI,
                        Peso = pesoDI
                    });
                }

                #endregion

                #endregion

                #region Preencher e Salvar

                Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacao = new Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido()
                {
                    Numero = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? repCotacao.BuscarProximoNumero(usuario.Empresa.Codigo) : repCotacao.BuscarProximoNumero(0),
                    Usuario = usuario,
                    Empresa = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? usuario.Empresa : null,
                    Data = DateTime.Now,
                    Previsao = dataPrevisaoServico,
                    TipoClienteCotacaoPedido = tipoCliente,
                    StatusCotacaoPedido = status,
                    SituacaoPedido = status == StatusCotacaoPedido.Fechada ? SituacaoPedido.AgAprovacao : status == StatusCotacaoPedido.EmAnalise || status == StatusCotacaoPedido.Sondagem ? SituacaoPedido.Aberto : SituacaoPedido.Cancelado,
                    ClienteAtivo = clienteAtivo,
                    ClienteInativo = clienteInativo,
                    ClienteProspect = clienteProspect,
                    ClienteNovo = novoCliente,
                    GrupoPessoas = grupoPessoas,
                    Origem = Origem,
                    Destino = Destino,
                    Destinatario = destinatario,
                    TipoDeCarga = tipoCarga,
                    TipoOperacao = tipoOperacao,
                    Prospeccao = prospeccao,
                    Produto = produtoEmbarcador,
                    ModeloVeicularCarga = modeloVeicular,
                    Solicitante = solicitante,
                    EmailContato = emailContato,
                    TelefoneContato = telefone,

                    //Aba Origem
                    UsarOutroEnderecoOrigem = mudarEnderecoOrigem,

                    //Aba Destino
                    UsarOutroEnderecoDestino = mudarEnderecoDestino,

                    //Aba Adicionais
                    DataInicialColeta = dataPrevisaoSaida,
                    DataFinalColeta = dataPrevisaoRetorno,
                    TipoModal = tipoModal,
                    NumeroPaletes = nPallets,
                    PesoTotal = PesoBruto,
                    ValorTotalNotasFiscais = valorMercadoria,
                    QuantidadeNotas = qtdNotas,
                    QtdEntregas = qtdEntregas,
                    Temperatura = temperatura,
                    KMTotal = KMTotal,
                    ValorPorKM = valorKM,
                    Rastreado = seraRastreado,
                    EscoltaArmada = NecessarioEscolta,
                    GerenciamentoRisco = teraGerenciamentoRisco,
                    Ajudante = ajudante,
                    QtdAjudantes = qtdAjudantes,
                    Observacao = observacao,
                    CubagemTotal = mCubicoTotal,
                    PesoCubado = pesoCubadoTotal,
                    QtVolumes = totalVolumes,

                    //Aba Valores
                    ValorFrete = valorFrete,
                    PercentualAcrescimo = acrescimo,
                    PercentualDesconto = desconto,
                    ValorTotalCotacao = totalCotacao,
                    AliquotaICMS = aliqICMS,
                    ValorICMS = valorICMS,
                    ValorTotalCotacaoComICMS = totalCotacaoComICMS,
                    IncluirValorICMSBaseCalculo = incluirValorICMSBaseCalculo,

                    //Aba Importação Exportação
                    NumeroBL = BL,
                    NumeroNavio = navio,
                    Porto = porto,
                    TipoTerminalImportacao = terminalImportacao,
                    DataVencimentoArmazenamentoImportacao = dataVencimentoArmazenamento,
                    ArmadorImportacao = armador,
                    EnderecoEntregaImportacao = enderecoImpEx,
                    BairroEntregaImportacao = bairroImpEx,
                    CEPEntregaImportacao = CEPImpEx,
                    LocalidadeEntregaImportacao = municipioImpEx
                };

                repCotacao.Inserir(cotacao, auditado);

                if (mudarEnderecoOrigem)
                {
                    SalvarOrigem(ref cotacao, cotacaoPedidoEnderecoOrigem, unitOfWork);
                }

                if (mudarEnderecoDestino)
                {
                    SalvarDestino(ref cotacao, cotacaoPedidoEnderecoDestino, unitOfWork);
                }

                SalvarListaCubagemAbaAdicionais(ref cotacao, calculosCubagem, unitOfWork);
                SalvarListaComponentesFreteAbaValores(ref cotacao, componentes, unitOfWork);
                SalvarListaDadosImportacaoAbaImportacaoExportacao(ref cotacao, dadosImportacao, unitOfWork);

                #endregion
            }

            catch (BaseException excecao)
            {
                return RetornarFalhaLinha(excecao.Message);
            }
            catch (Exception ex2)
            {
                Servicos.Log.TratarErro(ex2);
                return RetornarFalhaLinha("Ocorreu uma falha ao processar a linha (" + ex2.Message + ").");
            }

            return RetornarSucessoLinha(retornoLinhaPedido?.codigo ?? 0);
        }

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

        private decimal RetornarValorTotalComponentes(List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente> comps)
        {
            decimal total = 0m;

            foreach (var comp in comps)
            {
                total = total + comp.ValorTotal;
            }

            return total;
        }

        private decimal RetornarTotalCotacao(decimal valorFrete, decimal totalComp, decimal acrescimo, decimal desconto)
        {
            decimal total = valorFrete + totalComp;

            if (acrescimo > 0)
                total = total + ((total * acrescimo) / 100);

            if (desconto > 0)
                total = total - ((total * desconto) / 100);

            return total;
        }

        private void CalcularICMS(decimal aliq, ref decimal valorICMS, decimal total, ref decimal totalComICMS, bool incluirCalculo)
        {
            if (aliq > 0 && total > 0)
            {
                if (incluirCalculo)
                    valorICMS = (total / ((100 - aliq) / 100)) * (aliq / 100);
                else
                    valorICMS = total * (aliq / 100);

                if (valorICMS > 0)
                    totalComICMS = total + valorICMS;
            }
        }

        private void PreencherTotaisCubagem(ref decimal mCubicoTotal, ref decimal pesoCubadoTotal, ref int totalVolumes, List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoCubagem> calculosCubagem)
        {
            foreach (var calc in calculosCubagem)
            {
                mCubicoTotal = mCubicoTotal + calc.MetroCubico;
                pesoCubadoTotal = pesoCubadoTotal + calc.PesoCubado;
                totalVolumes = totalVolumes + calc.QtdVolume;
            }
        }

        public void SalvarOrigem(ref Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacao, Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoEndereco outroEnderecoOrigem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoEndereco repEnderecoCotacao = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoEndereco(unitOfWork);

            repEnderecoCotacao.Inserir(outroEnderecoOrigem);
            cotacao.EnderecoOrigem = outroEnderecoOrigem;
        }

        public void SalvarDestino(ref Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacao, Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoEndereco outroEnderecoDestino, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoEndereco repEnderecoCotacao = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoEndereco(unitOfWork);

            repEnderecoCotacao.Inserir(outroEnderecoDestino);
            cotacao.EnderecoDestino = outroEnderecoDestino;
        }

        public void SalvarListaCubagemAbaAdicionais(ref Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacao, List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoCubagem> calculosCubagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoCubagem repCubagem = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoCubagem(unitOfWork);

            foreach (var calcCub in calculosCubagem)
            {
                calcCub.CotacaoPedido = cotacao;
                repCubagem.Inserir(calcCub);
            }

        }

        public void SalvarListaComponentesFreteAbaValores(ref Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacao, List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente> componentes, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoComponente repCubagem = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoComponente(unitOfWork);

            foreach (var comp in componentes)
            {
                comp.CotacaoPedido = cotacao;
                repCubagem.Inserir(comp);
            }
        }

        public void SalvarListaDadosImportacaoAbaImportacaoExportacao(ref Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacao, List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoImportacao> DIs, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoImportacao repCubagem = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedidoImportacao(unitOfWork);

            foreach (var di in DIs)
            {
                di.CotacaoPedido = cotacao;
                repCubagem.Inserir(di);
            }
        }

        public decimal RetornarPercentualAliquota(Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, string conexao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.ICMS serICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);

            if (remetente != null && destinatario != null)
                return serICMS.ObterAliquota(remetente.Localidade.Estado, remetente.Localidade.Estado, destinatario.Localidade.Estado, remetente.Atividade, destinatario.Atividade, unitOfWork)?.Percentual ?? 0m;

            else
                return 0m;

        }

        public int RetornarKMTotal(Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, Repositorio.RotaFrete repRotaFrete)
        {
            if (origem != null && destino != null)
                return Convert.ToInt32(repRotaFrete.BuscarPorOrigemEDestino(origem, destino).FirstOrDefault()?.Quilometros ?? 0);
            else
                return 0;
        }

        public void PreencherComponentesEValorFrete(ref List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente> comps, ref decimal valorFrete, Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dados)
        {
            if (dados?.Componentes != null)
            {
                valorFrete = dados.ValorFrete;

                foreach (var comp in dados.Componentes)
                {
                    var compRecebido = new Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoComponente()
                    {
                        Valor = comp.ValorComponente,
                        PercentualAcrescimo = 0m,
                        PercentualDesconto = 0m,
                        ValorTotal = comp.ValorComponente,
                        ComponenteFrete = comp?.ComponenteFrete ?? null
                    };

                    comps.Add(compRecebido);
                }
            }
        }

        #endregion Métodos Privados
    }
}
