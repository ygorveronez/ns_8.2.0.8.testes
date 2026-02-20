using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Servicos.Embarcador.Integracao
{
    public sealed class IntegracaoCarregamento
    {
        #region  Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public IntegracaoCarregamento(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private void AdicionarArquivoTransacao(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao, string nomeArquivo, System.IO.MemoryStream edi)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioArquivoIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            string conteudoArquivo;

            using (StreamReader reader = new StreamReader(edi))
                conteudoArquivo = reader.ReadToEnd();

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoTransacao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(conteudoArquivo, nomeArquivo, "txt", _unitOfWork),
                Data = carregamentoIntegracao.DataIntegracao,
                Mensagem = carregamentoIntegracao.ProblemaIntegracao ?? "",
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioArquivoIntegracao.Inserir(arquivoTransacao);

            if (carregamentoIntegracao.ArquivosTransacao == null)
                carregamentoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            carregamentoIntegracao.ArquivosTransacao.Add(arquivoTransacao);
        }

        private void EnviarArquivoParaFTP(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI layoutCarregamento, System.IO.MemoryStream arquivo, string nomeArquivo)
        {
            Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI servicoGrupoPessoaLayoutEDI = new Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI(_unitOfWork);
            try
            {
                Dominio.Entidades.LayoutEDI layoutEDI = layoutCarregamento?.LayoutEDI;

                string mensagemErro = "";
                string url = layoutCarregamento.EnderecoFTP;
                string usuario = layoutCarregamento.Usuario;
                string senha = layoutCarregamento.Senha;
                string diretorio = layoutCarregamento.Diretorio ?? "";
                string porta = layoutCarregamento.Porta;
                bool passivo = layoutCarregamento.Passivo;
                bool utilizarSFTP = layoutCarregamento.UtilizarSFTP;
                bool ssl = layoutCarregamento.SSL;
                string certificado = servicoGrupoPessoaLayoutEDI.ObtemCertificadoChavePrivadaAsync(layoutCarregamento).GetAwaiter().GetResult();

                Servicos.FTP.EnviarArquivo(arquivo, nomeArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagemErro, utilizarSFTP, false, certificado);

                if (!string.IsNullOrWhiteSpace(mensagemErro))
                    throw new ServicoException(mensagemErro);
            }
            catch (ServicoException)
            {
                throw;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                throw new ServicoException("Erro ao enviar FTP");
            }
        }

        public void Integrar(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repositorioCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(_unitOfWork);

            carregamentoIntegracao.DataIntegracao = DateTime.Now;
            carregamentoIntegracao.NumeroTentativas++;

            try
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI layoutCarregamento = carregamentoIntegracao?.Carregamento?.Pedidos?.FirstOrDefault()?.Pedido?.Remetente.GrupoPessoas.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.IntegracaoCarregamento).FirstOrDefault();

                if (layoutCarregamento?.LayoutEDI == null)
                    throw new ServicoException("Layout EDI não Encontrado");

                if (
                    (layoutCarregamento.TipoIntegracao.Tipo != TipoIntegracao.Michelin) &&
                    (layoutCarregamento.TipoIntegracao.Tipo != TipoIntegracao.FTP) &&
                    (layoutCarregamento.TipoIntegracao.Tipo != TipoIntegracao.NaoPossuiIntegracao)
                )
                    throw new ServicoException("Tipo de integração do layout deve ser FTP ou não possuir integração.");

                MemoryStream arquivoEdi = ObterArquivoEdi(carregamentoIntegracao.Carregamento, layoutCarregamento);
                string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(carregamentoIntegracao.Carregamento, layoutCarregamento.LayoutEDI, "");
                string msgRetorno = string.Empty;

                if (layoutCarregamento.TipoIntegracao.Tipo == TipoIntegracao.FTP)
                    EnviarArquivoParaFTP(layoutCarregamento, arquivoEdi, nomeArquivo);
                else if (layoutCarregamento.TipoIntegracao.Tipo == TipoIntegracao.Michelin)
                    new Michelin.IntegracaoMichelin(_unitOfWork).EnviarMontagemCarga(arquivoEdi, nomeArquivo, out msgRetorno);

                if (string.IsNullOrWhiteSpace(msgRetorno))
                {
                    carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    carregamentoIntegracao.ProblemaIntegracao = "Integração realizada com sucesso";
                }
                else
                {
                    carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    carregamentoIntegracao.ProblemaIntegracao = msgRetorno;
                }

                AdicionarArquivoTransacao(carregamentoIntegracao, nomeArquivo, arquivoEdi);
            }
            catch (ServicoException excecao)
            {
                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar";
            }

            repositorioCarregamentoIntegracao.Atualizar(carregamentoIntegracao);
        }

        private MemoryStream ObterArquivoEdi(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI layoutCarregamento)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repositorioCarregamentoPedido.BuscarPorCarregamentoPorOrdem(carregamento.Codigo);
            Dominio.Entidades.Embarcador.Pedidos.Pedido primeiroPedido = carregamentoPedidos.FirstOrDefault()?.Pedido;
            int quantidadeCarregamentoDia = repositorioCarregamento.BuscarQuantidadeCarregamentoDoDia(DateTime.Now.Date);

            Dominio.ObjetosDeValor.EDI.Carregamento.Carga carga = new Dominio.ObjetosDeValor.EDI.Carregamento.Carga()
            {
                CarrierCode = carregamento.Empresa?.CodigoEmpresa ?? primeiroPedido?.Empresa?.CodigoEmpresa ?? "",
                CNPJEmbarcador = carregamento.Empresa?.CNPJ_SemFormato ?? primeiroPedido?.Empresa?.CNPJ_SemFormato ?? "",
                CodigoParceiroComercial = carregamento.Empresa?.CodigoIntegracao ?? primeiroPedido?.Empresa?.CodigoIntegracao ?? "",
                CodigoTipoEquipamento = carregamento.ModeloVeicularCarga?.CodigoIntegracao ?? "",
                CodigoTransportadorRedespacho = "",
                LocalEmbarque = primeiroPedido?.Remetente?.CodigoIntegracao ?? "",
                ModeloCarregamento = carregamento.TipoOperacao?.CodigoIntegracao ?? "",
                ModeloTransporte = "",
                NivelServico = carregamento.TipoOperacao?.Descricao ?? "",
                NumeroCarregamento = carregamento.AutoSequenciaNumero,
                PaisEmbarque = primeiroPedido?.Remetente?.Localidade?.Pais?.Sigla ?? "BR",
                SequenciaChegada = Utilidades.String.OnlyNumbers(quantidadeCarregamentoDia.ToString("n0")),
                SiteEmbarque = primeiroPedido?.Remetente?.CodigoDocumento ?? "",
                TipoEquipamento = carregamento.ModeloVeicularCarga?.Descricao ?? "",
                TripName = ""
            };

            carga.Rodape = new Dominio.ObjetosDeValor.EDI.Carregamento.Rodape()
            {
                QuantidadeLinhas = carregamentoPedidos.Count + 2
            };

            carga.Pedidos = new List<Dominio.ObjetosDeValor.EDI.Carregamento.Pedido>();

            for (int i = 0; i < carregamentoPedidos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido pedidoCarregamento = carregamentoPedidos[i];
                int ordemCarregamento = i + 1;

                Dominio.ObjetosDeValor.EDI.Carregamento.Pedido pedido = new Dominio.ObjetosDeValor.EDI.Carregamento.Pedido()
                {
                    CNPJEmbarcador = pedidoCarregamento.Pedido.Remetente?.CPF_CNPJ_SemFormato ?? "",
                    CodigoDestinatario = pedidoCarregamento.Pedido.Destinatario?.CodigoIntegracao ?? "",
                    CodigoItem = pedidoCarregamento.Pedido.Produtos?.FirstOrDefault()?.Produto?.CodigoProdutoEmbarcador ?? "",
                    Cubagem = pedidoCarregamento.Pedido.CubagemTotal,
                    DataPrevistaEntrega = pedidoCarregamento.Pedido.PrevisaoEntrega.HasValue ? pedidoCarregamento.Pedido.PrevisaoEntrega.Value : DateTime.Now,
                    DataPrevistaPartida = pedidoCarregamento.Pedido.DataPrevisaoSaida.HasValue ? pedidoCarregamento.Pedido.DataPrevisaoSaida.Value : DateTime.Now,
                    Destinatario = pedidoCarregamento.Pedido.Destinatario?.Nome ?? "",
                    NumeroCarga = pedidoCarregamento.Pedido.NumeroPedidoEmbarcador,
                    NumeroPedido = pedidoCarregamento.Pedido.NumeroPedidoEmbarcador,
                    NumeroRomaneio = carregamento.NumeroCarregamento,
                    OrdemCarregamento = pedidoCarregamento.Ordem > 0 ? Utilidades.String.OnlyNumbers(pedidoCarregamento.Ordem.ToString("n0")) : Utilidades.String.OnlyNumbers(ordemCarregamento.ToString("n0")),
                    Peso = pedidoCarregamento.Pedido.PesoTotal,
                    SeriePedido = "P",
                    Situacao = "1",
                    Volumes = pedidoCarregamento.Pedido.QtVolumes
                };

                carga.Pedidos.Add(pedido);
            }

            GeracaoEDI servicoGeracaoEDI = new GeracaoEDI(_unitOfWork, layoutCarregamento.LayoutEDI);
            MemoryStream arquivoEdi = servicoGeracaoEDI.GerarArquivoRecursivo(carga);

            return arquivoEdi;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao AdicionarIntegracaoCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, StatusCarregamentoIntegracao status, TipoIntegracao tipo, SituacaoIntegracao situacaoPadrao = SituacaoIntegracao.AgIntegracao)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(tipo);

            if (!ValidarSePodeGerarIntegracao(tipoIntegracao))
                return null;

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repositoriocarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao = repositoriocarregamentoIntegracao.BuscarPorCarregamentoETipoIntegracao(carregamento.Codigo, tipoIntegracao.Codigo);

            if (carregamentoIntegracao != null)
            {
                if ((status == StatusCarregamentoIntegracao.Inserir) && (tipo == TipoIntegracao.KuehneNagel))
                {
                    carregamentoIntegracao.NumeroTentativas = 0;
                    carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;

                    repositoriocarregamentoIntegracao.Atualizar(carregamentoIntegracao);
                }
                else if (tipo == TipoIntegracao.TelhaNorte)
                {
                    if (carregamentoIntegracao.Carregamento?.NumeroCargaAlteradoViaIntegracao ?? false)
                        carregamentoIntegracao.Status = StatusCarregamentoIntegracao.Atualizar;
                    else
                        carregamentoIntegracao.Status = StatusCarregamentoIntegracao.Inserir;

                    carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    carregamentoIntegracao.ProblemaIntegracao = "";
                    carregamentoIntegracao.NumeroTentativas = 0;

                    repositoriocarregamentoIntegracao.Atualizar(carregamentoIntegracao);
                }

                return carregamentoIntegracao;
            }

            carregamentoIntegracao = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao()
            {
                DataIntegracao = DateTime.Now,
                ProblemaIntegracao = "",
                SituacaoIntegracao = situacaoPadrao,
                Status = status,
                TipoIntegracao = tipoIntegracao,
                Carregamento = carregamento,
            };

            repositoriocarregamentoIntegracao.Inserir(carregamentoIntegracao);

            return carregamentoIntegracao;
        }

        public void AdicionarIntegracoesCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos, StatusCarregamentoIntegracao status, bool carregamentoGeradoViaWebService)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas repositorioGrupoPessoasIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido = carregamentoPedidos.FirstOrDefault();
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            List<TipoIntegracao> tiposExistentes = repositorioTipoIntegracao.BuscarTipos();

            if (carregamentoPedido?.Pedido?.Remetente?.GrupoPessoas?.LayoutsEDI != null)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI layoutCarregamento = carregamentoPedido.Pedido.Remetente.GrupoPessoas.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.IntegracaoCarregamento).FirstOrDefault();

                if (layoutCarregamento != null)
                    AdicionarIntegracaoCarregamento(carregamento, status, layoutCarregamento.TipoIntegracao.Tipo);
            }

            if (!carregamentoGeradoViaWebService && (carregamentoPedido?.Pedido?.IDLoteTrizy ?? 0) > 0)
                AdicionarIntegracaoCarregamento(carregamento, status, TipoIntegracao.Trizy);

            if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracaoAX)
                AdicionarIntegracaoCarregamento(carregamento, status, TipoIntegracao.AX);

            List<TipoIntegracao> tiposIntegracoesDisponiveis = new List<TipoIntegracao>() {
                TipoIntegracao.Digibee,
                TipoIntegracao.TelhaNorte
            };


            if (carregamento?.TipoOperacao?.GrupoPessoas != null)
            {
                List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa> integracaoGrupoPessoas = repositorioGrupoPessoasIntegracao.BuscarPorGrupoPessoas(carregamento.TipoOperacao.GrupoPessoas.Codigo);
                if (integracaoGrupoPessoas.Count > 0)
                    foreach (Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa integracaoGrupoPessoa in integracaoGrupoPessoas)
                        if (!tiposIntegracoesDisponiveis.Contains(integracaoGrupoPessoa.TipoIntegracao.Tipo))
                            tiposIntegracoesDisponiveis.Add(integracaoGrupoPessoa.TipoIntegracao.Tipo);
            }

            if (!string.IsNullOrWhiteSpace(carregamentoPedido?.Pedido?.NumeroEXP) && !string.IsNullOrWhiteSpace(carregamentoPedido?.Pedido?.ViaTransporte?.CodigoIntegracaoEnvio))
                tiposIntegracoesDisponiveis.Add(TipoIntegracao.KuehneNagel);

            if ((carregamento.TipoOperacao?.SelecionarRetiradaProduto ?? false) && carregamentoGeradoViaWebService)
                tiposIntegracoesDisponiveis.Add(TipoIntegracao.SaintGobain);

            if (tiposExistentes.Contains(TipoIntegracao.Dexco))
                tiposIntegracoesDisponiveis.Add(TipoIntegracao.Dexco);

            foreach (TipoIntegracao tipoIntegracao in tiposIntegracoesDisponiveis)
                AdicionarIntegracaoCarregamento(carregamento, status, tipoIntegracao);
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao ObterIntegracaoCarregamento(int codigoCarregamento, TipoIntegracao tipo)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(tipo);

            if (tipoIntegracao == null)
                return null;

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repositoriocarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao = repositoriocarregamentoIntegracao.BuscarPorCarregamentoETipoIntegracao(codigoCarregamento, tipoIntegracao.Codigo);

            return carregamentoIntegracao;
        }

        public void VerificarIntegracoesPendentes()
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repositorioCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao> carregamentosIntegracao = repositorioCarregamentoIntegracao.BuscarCarregamentoIntegracaoPendente(limiteRegistros: 20);

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao in carregamentosIntegracao)
            {
                switch (carregamentoIntegracao.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.FTP:
                    case TipoIntegracao.NaoPossuiIntegracao:
                    case TipoIntegracao.Michelin:
                        Integrar(carregamentoIntegracao);
                        break;

                    case TipoIntegracao.Digibee:
                        Digibee.IntegracaoDigibee.IntegrarCarregamento(carregamentoIntegracao, _unitOfWork);
                        break;

                    case TipoIntegracao.Trizy:
                        Trizy.IntegracaoTrizy.IntegrarApiEncerraLote(carregamentoIntegracao, _unitOfWork);
                        break;

                    case TipoIntegracao.AX:
                        AX.IntegracaoAX.IntegrarTransportadora(carregamentoIntegracao, _unitOfWork);
                        break;

                    case TipoIntegracao.SaintGobain:
                        new SaintGobain.IntegracaoSaintGobain(_unitOfWork).IntegrarCarregamento(carregamentoIntegracao);
                        break;

                    case TipoIntegracao.KuehneNagel:
                        new KuehneNagel.IntegracaoKuehneNagel(_unitOfWork).IntegrarCarregamento(carregamentoIntegracao);
                        break;

                    case TipoIntegracao.TelhaNorte:
                        new TelhaNorte.IntegracaoTelhaNorte(_unitOfWork).IntegrarCarregamento(carregamentoIntegracao);
                        break;
                    case TipoIntegracao.Dexco:
                        new Dexco.IntegracaoDexco(_unitOfWork).IntegrarCarregamento(carregamentoIntegracao);
                        break;
                    case TipoIntegracao.Moniloc:
                        new Moniloc.IntegracaoMoniloc(_unitOfWork).IntegrarCarregamento(carregamentoIntegracao);
                        break;

                    default:
                        carregamentoIntegracao.NumeroTentativas++;
                        carregamentoIntegracao.DataIntegracao = DateTime.Now;
                        carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        carregamentoIntegracao.ProblemaIntegracao = "Tipo de integração não implementado para integração de carregamentos.";
                        repositorioCarregamentoIntegracao.Atualizar(carregamentoIntegracao);
                        break;
                }
            }
        }

        #endregion Métodos Públicos

        private bool ValidarSePodeGerarIntegracao(Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            if (tipoIntegracao != null)
            {
                if (tipoIntegracao.Tipo == TipoIntegracao.Digibee)
                {
                    Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();
                    if (string.IsNullOrWhiteSpace(integracao?.URLIntegracaoDigibee))
                        return false;
                    else
                        return true;
                }
                else
                    return true;

            }
            else return false;
        }
    }
}
