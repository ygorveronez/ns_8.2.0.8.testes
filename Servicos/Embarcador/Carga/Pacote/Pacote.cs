using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Auditoria;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga.Pacote
{
    public class Pacote
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion Atributos Globais

        #region Construtores

        public Pacote(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Pacote(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void AvancarEtapaAutomaticamenteTempoParaRecebimentoDosPacotes(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            MensagemAlertaCarga mensagemAlertaCarga = new MensagemAlertaCarga(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
            List<int> listaCargas = repositorioCarga.BuscarTodasCargasPendentesAvancoPacotes(limiteRegistros: 10);

            for (int i = 0; i < listaCargas.Count; i++)
            {
                _unitOfWork.FlushAndClear();
                _unitOfWork.Start();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(listaCargas[i]);
                List<string> mensagensAlerta = new List<string>();

                try
                {
                    mensagemAlertaCarga.Confirmar(carga, TipoMensagemAlerta.CargaPedidoSemPacote);

                    if (carga.CargaEspelho != null && carga.TipoOperacao.ConfiguracaoCarga.AvancarCargaQuandoPedidoZeroPacotes)
                        LiberarPedidosSemPacote(carga.Codigo, carga.CargaEspelho.Codigo, mensagensAlerta);

                    if (carga.TipoOperacao.ConfiguracaoCarga.BuscarPacoteMesmoCNPJAdicionarCargaAposConsulta)
                        AdicionarPacoteCompativelAposConsulta(carga, mensagensAlerta, auditado, configuracaoTMS);

                    AdicionarDocumentosOriginaisCargaRetorno(carga.Codigo, configuracaoTMS);
                }
                catch (ServicoException servicoExeption)
                {
                    _unitOfWork.Rollback();
                    mensagensAlerta.Add(servicoExeption.Message);
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    Log.TratarErro(ex.Message);
                    mensagensAlerta.Add("Não foi possível concluir a busca pelos pacotes da Carga.");
                }

                if (mensagensAlerta.Count > 0)
                    mensagemAlertaCarga.Adicionar(carga, TipoMensagemAlerta.CargaPedidoSemPacote, mensagensAlerta);

                if (Servicos.Embarcador.Carga.Carga.AvancarEtapaDocumentosEmissaoCarga(out string erro, carga.Codigo, false, configuracaoTMS, _tipoServicoMultisoftware, _unitOfWork))
                {
                    carga.DataSalvouDadosCarga = null;
                    repositorioCarga.Atualizar(carga);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, carga, $"Avançado para etapa documentos automaticamente devido a configuração do Tipo Operação - Recebimento de Pacotes", _unitOfWork);
                }
                else
                    Servicos.Log.TratarErro($"Não foi possível avançar automaticamente a etapa de documentos para emissão da carga (código {carga.Codigo}), motivo: {erro}");

                _unitOfWork.CommitChanges();
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacao(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Log Key", Propriedade = "LogKey", Tamanho = 100, Obrigatorio = true });

            return configuracoes;
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao Importar(string dados, (string Nome, string Guid) arquivoGerador, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, int codigoCargaPedido)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfigGeral = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configGeral = repConfigGeral.BuscarConfiguracaoPadrao();
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao()
            {
                Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
            };

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCargaPedido(codigoCargaPedido);
            carga.ProcessandoImportacaoPacote = true;
            repositorioCarga.Atualizar(carga);

            int contador = 0;

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    unitOfWork.FlushAndClear();
                    unitOfWork.Start();

                    Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = ImportarLinha(linhas[i], arquivoGerador, usuario, tipoServicoMultisoftware, auditado, unitOfWork, configGeral, codigoCargaPedido);
                    retornoLinha.indice = i;
                    retornoImportacao.Retornolinhas.Add(retornoLinha);

                    if (retornoLinha.contar)
                    {
                        contador++;
                    }

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
                }
            }

            retornoImportacao.MensagemAviso = "";
            retornoImportacao.Total = linhas.Count();
            retornoImportacao.Importados = contador;

            carga = repositorioCarga.BuscarPorCargaPedido(codigoCargaPedido);
            carga.ProcessandoImportacaoPacote = false;
            repositorioCarga.Atualizar(carga);

            return retornoImportacao;
        }
        #endregion Métodos Públicos

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

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha ImportarLinha(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, (string Nome, string Guid) arquivoGerador, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configGeral, int codigoCargaPedido)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoPacote repCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinhaPedido = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha();

            try
            {
                #region Ler dados

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLogKey = (from obj in linha.Colunas where obj.NomeCampo == "LogKey" select obj).FirstOrDefault();
                string logKey = null;
                if (colLogKey.Valor != null)
                {
                    logKey = colLogKey.Valor;

                    if (string.IsNullOrEmpty(logKey))
                        return RetornarFalhaLinha("Coluna Log Key não está preenchida");
                }

                #endregion Ler dados

                #region Preencher e Salvar

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> cargaPedidosPacotes = repCargaPedidoPacote.BuscarCargaPedidoPacotePorLoggiKey(logKey);
                if (cargaPedidosPacotes.Count == 0)
                    return RetornarFalhaLinha("Log Key inválida, Pacote não encontrado");

                SalvarPacote(cargaPedidosPacotes, codigoCargaPedido, auditado, unitOfWork);

                #endregion Preencher e Salvar
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

        private void SalvarPacote(List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> cargaPedidosPacotes, int codigoCargaPedido, Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoPacote repCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

            Servicos.Embarcador.Pacote.Pacote servicoPacote = new Servicos.Embarcador.Pacote.Pacote(unitOfWork, _tipoServicoMultisoftware);

            if (cargaPedido == null)
                throw new ControllerException("Falha ao vincular à carga");

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote cargaPedidoPacote in cargaPedidosPacotes)
            {
                cargaPedidoPacote.CargaPedido = cargaPedido;

                repCargaPedidoPacote.Atualizar(cargaPedidoPacote);

                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceiro = repCTeTerceiro.BuscarPorIdentificacaoPacote(cargaPedidoPacote.Pacote.LogKey);
                foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro in ctesTerceiro)
                {
                    string retorno = new Servicos.Embarcador.Pacote.Pacote(_unitOfWork, _tipoServicoMultisoftware).VincularCTeCargaPedidoPacoteAsync(cargaPedidoPacote, cteTerceiro).GetAwaiter().GetResult();
                    if (!string.IsNullOrWhiteSpace(retorno))
                        throw new ServicoException(retorno);
                }

                servicoPacote.VerificarQuantidadePacotesCtesAvancaAutomaticoAsync(cargaPedido.Carga, auditado).GetAwaiter().GetResult();

                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedidoPacote.Pacote, null, "Vinculado manualmente via importação de Planilha", unitOfWork);
            }
        }

        private void AdicionarPacoteCompativelAposConsulta(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<string> mensagensAlerta, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoPacote repositorioCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(_unitOfWork);
            Repositorio.Embarcador.Cargas.Pacote repositorioPacote = new Repositorio.Embarcador.Cargas.Pacote(_unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repositorioCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosSemPacote = repositorioCargaPedidoPacote.BuscarCargaPedidoSemPacotePorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosSemPacote)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.Pacote.Pacote pacote = repositorioPacote.BuscarPacoteCompativelComCargaPedido(cargaPedido.Pedido);

                if (pacote == null)
                {
                    mensagensAlerta.Add($"Não foram encontrados pacotes compatíveis para vincular ao Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador}");
                    continue;
                }

                Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote cargaPedidoPacote = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote()
                {
                    Pacote = new Dominio.Entidades.Embarcador.Cargas.Pacote { Codigo = pacote.CodigoPacote },
                    CargaPedido = cargaPedido
                };

                repositorioCargaPedidoPacote.Inserir(cargaPedidoPacote);

                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceiro = repositorioCTeTerceiro.BuscarPorIdentificacaoPacote(pacote.LogKey);

                foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro in ctesTerceiro)
                {
                    string retorno = new Servicos.Embarcador.Pacote.Pacote(_unitOfWork, _tipoServicoMultisoftware).VincularCTeCargaPedidoPacoteAsync(cargaPedidoPacote, cteTerceiro, null, configuracaoTMS).GetAwaiter().GetResult();
                    
                    if (!string.IsNullOrWhiteSpace(retorno))
                        throw new ServicoException(retorno);
                }

                Servicos.Auditoria.Auditoria.Auditar(auditado, carga, $"Pacote {pacote.LogKey} vinculado ao Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador}", _unitOfWork);
            }
        }

        private void LiberarPedidosSemPacote(int codigoCargaAtual, int codigoCargaEspelho, List<string> mensagensAlerta)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosSemColetaPacote = repositorioCargaEntregaPedido.BuscarCargaPedidoSemColetaPacotePorCarga(codigoCargaEspelho, codigoCargaAtual);

            if (cargaPedidosSemColetaPacote.Count <= 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosSemColetaPacote)
            {
                cargaPedido.PedidoSemNFe = true;
                repositorioCargaPedido.Atualizar(cargaPedido);
            }

            mensagensAlerta.Add("Carga liberada automaticamente pois o pedido não tinha pacotes na entrega");
        }

        private void AdicionarDocumentosOriginaisCargaRetorno(int codigoCargaAtual, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Repositorio.Embarcador.Cargas.Retornos.RetornoCarga repositorioRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.RetornoCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoPacote repositorioCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga retornoCarga = repositorioRetornoCarga.BuscarPorCargaRetorno(codigoCargaAtual);

            if (retornoCarga == null)
                return;

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedidoPacote.BuscarPrimeiroCargaPedidoSemPacotePorCarga(codigoCargaAtual);

            if (cargaPedido == null)
                return;

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoAnterior = repositorioCargaPedido.BuscarUltimaEntregaCarga(retornoCarga.Carga.Codigo);

            Servicos.Embarcador.Carga.FilialEmissora servicoFilialEmissora = new Embarcador.Carga.FilialEmissora();

            servicoFilialEmissora.GerarCTesAnteriores(cargaPedidoAnterior, cargaPedido, _tipoServicoMultisoftware, _unitOfWork, configuracaoTMS);
        }

        #endregion
    }
}
